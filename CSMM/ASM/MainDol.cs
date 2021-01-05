using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MiscUtil.IO;
using MiscUtil.Conversion;
using FSEditor.FSData;
using System.Text.RegularExpressions;

namespace CustomStreetMapManager
{
    public class MainDol
    {
        public FreeSpaceManager freeSpaceManager;
        public AddressMapper addressMapper;

        private string resolveAddressToString(VAVAddr virtualAddress, EndianBinaryReader stream)
        {
            if (virtualAddress == VAVAddr.NullAddress)
                return null;
            int fileAddress = addressMapper.toFileAddress(virtualAddress);
            stream.Seek(fileAddress, SeekOrigin.Begin);
            byte[] buff = stream.ReadBytes(64);
            return HexUtil.byteArrayToString(buff);
        }

        public void setupAddressMapper(EndianBinaryReader stream, List<AddressSection> fileMappingSections, IProgress<ProgressInfo> progress)
        {
            addressMapper = new AddressMapper(fileMappingSections);
            // find out the version we are dealing with

            // Boom Street: 8007a314: lwz r0,-0x547c(r13)
            stream.Seek(addressMapper.toFileAddress((VAVAddr)0x8007a314), SeekOrigin.Begin);
            if (stream.ReadUInt32() == PowerPcAsm.lwz(0, -0x547c, 13))
            {
                progress?.Report("Detected game: Boom Street");
                // boom street address mapper is a no-op, since the ASM hacks use the boom street virtual addresses
                var versionMappingSections = new List<AddressSection> { AddressSection.identity() };
                addressMapper.setVersionMappingSections(versionMappingSections);
            }
            else
            {
                // Fortune Street: 8007a2c0: lwz r0,-0x547c(r13)
                stream.Seek(addressMapper.toFileAddress((VAVAddr)0x8007a2c0), SeekOrigin.Begin);
                if (stream.ReadUInt32() == PowerPcAsm.lwz(0, -0x547c, 13))
                {
                    progress?.Report("Detected game: Fortune Street");

                    var versionMappingSections = new List<AddressSection>();
                    // add mappings to translate boom street virtual addresses to fortune street virtual addresses
                    versionMappingSections.Add(new AddressSection(0x80000100, 0x8007a283, 0x0, ".text, .data0, .data1 and beginning of .text1 until InitSoftLanguage"));
                    versionMappingSections.Add(new AddressSection(0x8007a2f4, 0x80268717, 0x54, "continuation of .text1 until AIRegisterDMACallback"));
                    versionMappingSections.Add(new AddressSection(0x80268720, 0x8040d97b, 0x50, "continuation of .text1"));
                    versionMappingSections.Add(new AddressSection(0x8040d980, 0x8041027f, 0x40, ".data2, .data3 and beginning of .data4 until Boom Street / Fortune Street strings"));
                    versionMappingSections.Add(new AddressSection(0x804105f0, 0x8044ebe7, 0x188, "continuation of .data4"));
                    versionMappingSections.Add(new AddressSection(0x8044ec00, 0x804ac804, 0x1A0, ".data5"));
                    versionMappingSections.Add(new AddressSection(0x804ac880, 0x8081f013, 0x200, ".uninitialized0, .data6, .uninitialized1, .data7, .uninitialized2"));

                    addressMapper.setVersionMappingSections(versionMappingSections);
                }
                else
                {
                    // unsupported version
                    throw new ApplicationException("Only Boom Street (ST7P01) and Fortune Street (ST7E01) are supported.");
                }
            }
            freeSpaceManager = new FreeSpaceManager();
            // Venture Card Table
            freeSpaceManager.addFreeSpace(addressMapper.toVersionAgnosticAddress((BSVAddr)0x80410648), addressMapper.toVersionAgnosticAddress((BSVAddr)0x80411b9b));
            // Map Data String Table and Map Data Table
            freeSpaceManager.addFreeSpace(addressMapper.toVersionAgnosticAddress((BSVAddr)0x80428978), addressMapper.toVersionAgnosticAddress((BSVAddr)0x804298cf));
            // Map Default Settings Table
            freeSpaceManager.addFreeSpace(addressMapper.toVersionAgnosticAddress((BSVAddr)0x804363c8), addressMapper.toVersionAgnosticAddress((BSVAddr)0x80436a87));
            // Unused costume string table 1
            freeSpaceManager.addFreeSpace(addressMapper.toVersionAgnosticAddress((BSVAddr)0x8042bc78), addressMapper.toVersionAgnosticAddress((BSVAddr)0x8042c23f));
            // Unused costume string table 2
            freeSpaceManager.addFreeSpace(addressMapper.toVersionAgnosticAddress((BSVAddr)0x8042dfc0), addressMapper.toVersionAgnosticAddress((BSVAddr)0x8042e22f));
            // Unused costume string table 3
            freeSpaceManager.addFreeSpace(addressMapper.toVersionAgnosticAddress((BSVAddr)0x8042ef30), addressMapper.toVersionAgnosticAddress((BSVAddr)0x8042f7ef));
            // Unused menu id=0x06 (MapSelectScene_E3)
            freeSpaceManager.addFreeSpace(addressMapper.toVersionAgnosticAddress((BSVAddr)0x801f8520), addressMapper.toVersionAgnosticAddress((BSVAddr)0x801f94bb));
            // Unused menu id=0x38 (WorldMenuScene)
            freeSpaceManager.addFreeSpace(addressMapper.toVersionAgnosticAddress((BSVAddr)0x801ed6a8), addressMapper.toVersionAgnosticAddress((BSVAddr)0x801edab7));
            // Unused menu id=0x39 (FreePlayScene)
            freeSpaceManager.addFreeSpace(addressMapper.toVersionAgnosticAddress((BSVAddr)0x801edad4), addressMapper.toVersionAgnosticAddress((BSVAddr)0x801ee71f));
            // Unused menu class (SelectMapUI)
            freeSpaceManager.addFreeSpace(addressMapper.toVersionAgnosticAddress((BSVAddr)0x801fce28), addressMapper.toVersionAgnosticAddress((BSVAddr)0x801ff777));

            // used additional address:
            // 0x804363b4 (4 bytes):  force simulated button press
            // 0x804363b8 (12 bytes): pointer to internal name table
            // 0x804363c4 (4 bytes):  ForceVentureCardVariable

        }



        public List<MapDescriptor> readMainDol(EndianBinaryReader stream, List<AddressSection> fileMappingSections, IProgress<ProgressInfo> progress)
        {
            setupAddressMapper(stream, fileMappingSections, progress);

            // GetMapCount
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801cca30), SeekOrigin.Begin);
            UInt32 opcode = stream.ReadUInt32();
            var count = (Int16)(PowerPcAsm.getOpcodeParameter(opcode));

            List<MapDescriptor> mapDescriptors = new List<MapDescriptor>();
            for(int i = 0; i < count; i++)
            {
                MapDescriptor mapDescriptor = new MapDescriptor();
                mapDescriptors.Add(mapDescriptor);
            }

            new MapOriginTable().read(stream, addressMapper, mapDescriptors, null);
            // map description table must be after map origin table
            new MapDescriptionTable().read(stream, addressMapper, mapDescriptors, null);
            new BackgroundTable().read(stream, addressMapper, mapDescriptors, null);
            // map icon table must be after the map background table and map origin table
            new MapIconTable().read(stream, addressMapper, mapDescriptors, null);

            new MapSetZoneOrder().read(stream, addressMapper, mapDescriptors, null);
            // practice board comes after category zone order
            new PracticeBoard().read(stream, addressMapper, mapDescriptors, null);

            // the rest does not have any dependencies
            new DefaultTargetAmountTable().read(stream, addressMapper, mapDescriptors, null);
            new VentureCardTable().read(stream, addressMapper, mapDescriptors, null);
            new EventSquare().read(stream, addressMapper, mapDescriptors, null);
            new RuleSetTable().read(stream, addressMapper, mapDescriptors, null);
            new TourBankruptcyLimitTable().read(stream, addressMapper, mapDescriptors, null);
            new TourInitialCashTable().read(stream, addressMapper, mapDescriptors, null);
            new TourOpponentsTable().read(stream, addressMapper, mapDescriptors, null);
            new TourClearRankTable().read(stream, addressMapper, mapDescriptors, null);
            new StageNameIDTable().read(stream, addressMapper, mapDescriptors, null);
            new BGMIDTable().read(stream, addressMapper, mapDescriptors, null);
            new DesignTypeTable().read(stream, addressMapper, mapDescriptors, null);
            new FrbMapTable().read(stream, addressMapper, mapDescriptors, null);
            new MapSwitchParamTable().read(stream, addressMapper, mapDescriptors, null);
            new MapGalaxyParamTable().read(stream, addressMapper, mapDescriptors, null);
            new BGSequenceTable().read(stream, addressMapper, mapDescriptors, null);
            new InternalNameTable().read(stream, addressMapper, mapDescriptors, null);
            new ForceSimulatedButtonPress().read(stream, addressMapper, mapDescriptors, null);

            return mapDescriptors;
        }
        public List<MapDescriptor> writeMainDol(EndianBinaryWriter stream, List<MapDescriptor> mapDescriptors, IProgress<ProgressInfo> progress)
        {
            new DefaultTargetAmountTable().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);
            new TourBankruptcyLimitTable().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);
            new TourInitialCashTable().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);
            new MapDescriptionTable().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);
            new MapIconTable().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);
            new VentureCardTable().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);
            new EventSquare().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);
            new RuleSetTable().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);
            new TourOpponentsTable().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);
            new TourClearRankTable().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);
            new StageNameIDTable().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);
            new BGMIDTable().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);
            new BackgroundTable().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);
            new DesignTypeTable().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);
            new FrbMapTable().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);
            new MapSwitchParamTable().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);
            new MapGalaxyParamTable().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);
            new MapOriginTable().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);
            new BGSequenceTable().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);
            new InternalNameTable().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);
            new PracticeBoard().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);
            new MapSetZoneOrder().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);
            new ForceSimulatedButtonPress().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);

            freeSpaceManager.nullTheFreeSpace(stream, addressMapper);

            // Write the Map Count
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801cca30), SeekOrigin.Begin);
            stream.Write(PowerPcAsm.li(3, (short) mapDescriptors.Count));

            return mapDescriptors;
        }
    }
}
