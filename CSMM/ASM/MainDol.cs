using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MiscUtil.IO;
using MiscUtil.Conversion;
using FSEditor.FSData;
using System.Text.RegularExpressions;

namespace CustomStreetManager
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

        FreeSpaceManager createFreeSpaceManager(AddressMapper addressMapper)
        {
            var freeSpaceManager = new FreeSpaceManager();
            // Venture Card Table
            freeSpaceManager.addFreeSpace(addressMapper.toVersionAgnosticAddress((BSVAddr)0x80410648), addressMapper.toVersionAgnosticAddress((BSVAddr)0x80411b9b));
            // Map Data String Table
            freeSpaceManager.addFreeSpace(addressMapper.toVersionAgnosticAddress((BSVAddr)0x80428978), addressMapper.toVersionAgnosticAddress((BSVAddr)0x80428e4f));
            // Unused costume string table 1
            freeSpaceManager.addFreeSpace(addressMapper.toVersionAgnosticAddress((BSVAddr)0x8042bc78), addressMapper.toVersionAgnosticAddress((BSVAddr)0x8042c23f));
            // Unused costume string table 2
            freeSpaceManager.addFreeSpace(addressMapper.toVersionAgnosticAddress((BSVAddr)0x8042dfc0), addressMapper.toVersionAgnosticAddress((BSVAddr)0x8042e22f));
            // Unused costume string table 3
            freeSpaceManager.addFreeSpace(addressMapper.toVersionAgnosticAddress((BSVAddr)0x8042ef30), addressMapper.toVersionAgnosticAddress((BSVAddr)0x8042f7ef));
            // Expanded Rom
            // freeSpaceManager.addFreeSpace(0x80001800, 0x80001800 + 0x1800);
            return freeSpaceManager;
        }

        public void setupAddressMapper(EndianBinaryReader stream, List<AddressSection> fileMappingSections)
        {
            var versionMappingSections = new List<AddressSection> { AddressSection.identity() };
            addressMapper = new AddressMapper(fileMappingSections, versionMappingSections);
            freeSpaceManager = createFreeSpaceManager(addressMapper);
        }

        public List<MapDescriptor> readMainDol(EndianBinaryReader stream, List<AddressSection> fileMappingSections)
        {
            setupAddressMapper(stream, fileMappingSections);

            List<MapDescriptor> mapDescriptors = new List<MapDescriptor>();

            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80428e50), SeekOrigin.Begin);
            for (int i = 0; i < 48; i++)
            {
                MapDescriptor mapDescriptor = new MapDescriptor();
                mapDescriptor.readMapDataFromStream(stream);
                mapDescriptors.Add(mapDescriptor);
            }
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x804363c8), SeekOrigin.Begin);
            for (int i = 0; i < 48; i++)
            {
                MapDescriptor mapDescriptor = mapDescriptors[i];
                mapDescriptor.readMapDefaultsFromStream(stream);
            }
            new DefaultGoalMoneyTable().read(stream, addressMapper, mapDescriptors, null);

            new MapDescriptionTable().read(stream, addressMapper, mapDescriptors, null);

            foreach (MapDescriptor mapDescriptor in mapDescriptors)
            {
                var internalName = resolveAddressToString(mapDescriptor.InternalNameAddr, stream).Trim();
                mapDescriptor.InternalName = Regex.Replace(internalName, @"[<>:/\|?*""]+", "");

                mapDescriptor.Background = resolveAddressToString(mapDescriptor.BackgroundAddr, stream);
                mapDescriptor.FrbFile1 = resolveAddressToString(mapDescriptor.FrbFile1Addr, stream);
                mapDescriptor.FrbFile2 = resolveAddressToString(mapDescriptor.FrbFile2Addr, stream);
                mapDescriptor.FrbFile3 = resolveAddressToString(mapDescriptor.FrbFile3Addr, stream);
                mapDescriptor.FrbFile4 = resolveAddressToString(mapDescriptor.FrbFile4Addr, stream);
                mapDescriptor.readRotationOriginPoints(stream, addressMapper);
                mapDescriptor.readLoopingModeParams(stream, addressMapper);
            }

            new MapIconTable().read(stream, addressMapper, mapDescriptors, null);

            new VentureCardTable().read(stream, addressMapper, mapDescriptors, null);

            return mapDescriptors;
        }
        public List<MapDescriptor> writeMainDol(EndianBinaryWriter stream, List<MapDescriptor> mapDescriptors, IProgress<ProgressInfo> progress)
        {
            if (mapDescriptors.Count != 48)
            {
                throw new ArgumentException("length of map descriptor list is not 48.");
            }

            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80428e50), SeekOrigin.Begin);
            for (int i = 0; i < 48; i++)
            {
                int seek = (int)stream.BaseStream.Position;
                MapDescriptor mapDescriptor = mapDescriptors[i];
                VAVAddr internalNameAddr = VAVAddr.NullAddress;
                VAVAddr backgroundAddr = VAVAddr.NullAddress;
                VAVAddr frbFile1Addr = VAVAddr.NullAddress;
                VAVAddr frbFile2Addr = VAVAddr.NullAddress;
                VAVAddr frbFile3Addr = VAVAddr.NullAddress;
                VAVAddr frbFile4Addr = VAVAddr.NullAddress;
                VAVAddr mapSwitchParamAddr = VAVAddr.NullAddress;
                VAVAddr loopingModeParamAddr = VAVAddr.NullAddress;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                    s.Write(mapDescriptor.InternalName);
                    s.Write((byte)0);
                    internalNameAddr = freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, addressMapper, progress);
                }
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                    s.Write(mapDescriptor.Background);
                    s.Write((byte)0);
                    backgroundAddr = freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, addressMapper, progress);
                }
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                    s.Write(mapDescriptor.FrbFile1);
                    s.Write((byte)0);
                    frbFile1Addr = freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, addressMapper, progress);
                }
                if (!string.IsNullOrEmpty(mapDescriptor.FrbFile2))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                        s.Write(mapDescriptor.FrbFile2);
                        s.Write((byte)0);
                        frbFile2Addr = freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, addressMapper, progress);
                    }
                }
                if (!string.IsNullOrEmpty(mapDescriptor.FrbFile3))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                        s.Write(mapDescriptor.FrbFile3);
                        s.Write((byte)0);
                        frbFile3Addr = freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, addressMapper, progress);
                    }
                }
                if (!string.IsNullOrEmpty(mapDescriptor.FrbFile4))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                        s.Write(mapDescriptor.FrbFile4);
                        s.Write((byte)0);
                        frbFile4Addr = freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, addressMapper, progress);
                    }
                }
                if (mapDescriptor.LoopingMode != LoopingMode.None)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                        mapDescriptor.writeLoopingModeParams(s);
                        loopingModeParamAddr = freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, addressMapper, progress, "Looping Mode Config");
                    }
                }
                if (mapDescriptor.SwitchRotationOriginPoints.Count != 0)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                        mapDescriptor.writeSwitchRotationOriginPoints(s);
                        mapSwitchParamAddr = freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, addressMapper, progress, "Switch Rotation Origin Points");
                    }
                }
                stream.Seek(seek, SeekOrigin.Begin);
                mapDescriptor.writeMapData(stream, internalNameAddr, backgroundAddr, frbFile1Addr, frbFile2Addr, frbFile3Addr, frbFile4Addr, mapSwitchParamAddr, loopingModeParamAddr, addressMapper.toVersionAgnosticAddress((BSVAddr)0x80428968));
            }

            new DefaultGoalMoneyTable().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);
            new MapDescriptionTable().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);
            new MapIconTable().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);
            new VentureCardTable().write(stream, addressMapper, mapDescriptors, freeSpaceManager, progress);

            freeSpaceManager.nullTheFreeSpace(stream, addressMapper);

            return mapDescriptors;
        }
    }
}
