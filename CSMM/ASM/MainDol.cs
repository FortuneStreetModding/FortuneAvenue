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
        public readonly AddressMapper addressMapper;
        public readonly FreeSpaceManager freeSpaceManager;
        public readonly List<DolIO> patches;

        public MainDol(EndianBinaryReader stream, List<AddressSection> fileMappingSections, IProgress<ProgressInfo> progress)
        {
            this.addressMapper = setupAddressMapper(stream, fileMappingSections, progress);
            this.freeSpaceManager = setupFreeSpaceManager(addressMapper);
            this.patches = setupPatches();
        }

        private AddressMapper setupAddressMapper(EndianBinaryReader stream, List<AddressSection> fileMappingSections, IProgress<ProgressInfo> progress)
        {
            var addressMapper = new AddressMapper(fileMappingSections);
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
            return addressMapper;
        }

        private FreeSpaceManager setupFreeSpaceManager(AddressMapper addressMapper)
        {
            var freeSpaceManager = new FreeSpaceManager();
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
            return freeSpaceManager;
        }

        private List<DolIO> setupPatches()
        {
            var patches = new List<DolIO>();
            patches.Add(new MapOriginTable());
            // map description table must be after map origin table
            patches.Add(new MapDescriptionTable());
            patches.Add(new BackgroundTable());
            // map icon table must be after the map background table and map origin table
            patches.Add(new MapIconTable());

            patches.Add(new MapSetZoneOrder());
            // practice board comes after category zone order
            patches.Add(new PracticeBoard());

            // the rest does not have any dependencies
            patches.Add(new DefaultTargetAmountTable());
            patches.Add(new VentureCardTable());
            patches.Add(new EventSquare());
            patches.Add(new RuleSetTable());
            patches.Add(new TourBankruptcyLimitTable());
            patches.Add(new TourInitialCashTable());
            patches.Add(new TourOpponentsTable());
            patches.Add(new TourClearRankTable());
            patches.Add(new StageNameIDTable());
            patches.Add(new BGMIDTable());
            patches.Add(new DesignTypeTable());
            patches.Add(new FrbMapTable());
            patches.Add(new MapSwitchParamTable());
            patches.Add(new MapGalaxyParamTable());
            patches.Add(new BGSequenceTable());
            patches.Add(new InternalNameTable());
            patches.Add(new ForceSimulatedButtonPress());
            return patches;
        }

        public List<MapDescriptor> readMainDol(EndianBinaryReader stream, IProgress<ProgressInfo> progress)
        {
            // GetMapCount
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801cca30), SeekOrigin.Begin);
            UInt32 opcode = stream.ReadUInt32();
            var count = (Int16)(PowerPcAsm.getOpcodeParameter(opcode));

            List<MapDescriptor> mapDescriptors = new List<MapDescriptor>();
            for (int i = 0; i < count; i++)
            {
                MapDescriptor mapDescriptor = new MapDescriptor();
                mapDescriptors.Add(mapDescriptor);
            }

            foreach (var patch in patches)
            {
                patch.read(stream, addressMapper, mapDescriptors, progress);
            }

            return mapDescriptors;
        }
        public List<MapDescriptor> writeMainDol(EndianBinaryWriter stream, List<MapDescriptor> mapDescriptors, IProgress<ProgressInfo> progress)
        {
            foreach (var patch in patches)
            {
                patch.write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);
            }

            freeSpaceManager.nullTheFreeSpace(stream, addressMapper);

            // Write the Map Count
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801cca30), SeekOrigin.Begin);
            stream.Write(PowerPcAsm.li(3, (short)mapDescriptors.Count));

            return mapDescriptors;
        }
    }
}
