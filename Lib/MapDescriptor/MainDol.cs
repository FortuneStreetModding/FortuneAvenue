using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MiscUtil.IO;
using MiscUtil.Conversion;
using FSEditor.FSData;
using System.Text.RegularExpressions;

namespace FSEditor.MapDescriptor
{
    public class MainDol
    {
        private List<MainDolSection> sections;
        private Dictionary<byte[], UInt32> reuseValues;
        private UInt32 unusedSpacePositionVirtual;
        public int totalBytesWritten;
        public int totalBytesLeft;
        public AddressConstants data = new ST7P01();

        public MainDol(List<MainDolSection> sections)
        {
            this.sections = sections;
        }

        public void setSections(List<MainDolSection> sections)
        {
            this.sections = sections;
        }

        public bool sectionAvailable(string sectionName)
        {
            foreach (MainDolSection section in sections)
            {
                if (String.Equals(section.sectionName, sectionName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public int toFileAddress(uint virtualAddress)
        {
            foreach (MainDolSection section in sections)
            {
                if (section.containsVirtualAddress(virtualAddress))
                {
                    return (int)section.toFileAddress(virtualAddress);
                }
            }
            return -1;
        }

        public int toVirtualAddress(int fileAddress)
        {
            foreach (MainDolSection section in sections)
            {
                if (section.containsFileAddress(fileAddress))
                {
                    return (int)section.toVirtualAddress(fileAddress);
                }
            }
            return -1;
        }

        private string resolveAddressAddressToString(uint virtualAddressAddress, EndianBinaryReader binReader)
        {
            int fileAddress = toFileAddress(virtualAddressAddress);
            if (fileAddress >= 0)
            {
                binReader.Seek(fileAddress, SeekOrigin.Begin);
                var virtualAddress = binReader.ReadUInt32();
                return resolveAddressToString(virtualAddress, binReader);
            }
            else
            {
                return null;
            }
        }

        private string resolveAddressToString(uint virtualAddress, EndianBinaryReader binReader)
        {
            int fileAddress = toFileAddress(virtualAddress);
            if (fileAddress >= 0)
            {
                binReader.Seek(fileAddress, SeekOrigin.Begin);
                byte[] buff = binReader.ReadBytes(64);
                return HexUtil.byteArrayToString(buff);
            }
            else
            {
                return null;
            }
        }

        public List<MapDescriptor> readMainDol(EndianBinaryReader binReader)
        {
            List<MapDescriptor> mapDescriptors = new List<MapDescriptor>();
            // has the hack for unique map icons been applied?
            var hackExpandedMapIconsApplied = data.isHackCustomMapIcons(binReader, toFileAddress);
            // has the hack for expanded Description message table already been applied?
            var hackExpandedDescriptionMessageTableApplied = data.isHackExtendedMapDescriptions(binReader, toFileAddress);

            binReader.Seek(toFileAddress(data.START_MAP_DATA_TABLE_VIRTUAL()), SeekOrigin.Begin);
            for (int i = 0; i < 48; i++)
            {
                MapDescriptor mapDescriptor = new MapDescriptor();
                mapDescriptor.readMapDataFromStream(binReader);
                mapDescriptors.Add(mapDescriptor);
            }
            binReader.Seek(toFileAddress(data.START_MAP_DEFAULTS_TABLE_VIRTUAL()), SeekOrigin.Begin);
            for (int i = 0; i < 48; i++)
            {
                MapDescriptor mapDescriptor = mapDescriptors[i];
                mapDescriptor.readMapDefaultsFromStream(binReader);
            }
            foreach (MapDescriptor mapDescriptor in mapDescriptors)
            {
                var internalName = resolveAddressToString(mapDescriptor.InternalNameAddr, binReader).Trim();
                mapDescriptor.InternalName = Regex.Replace(internalName, @"[<>:/\|?*""]+", "");

                mapDescriptor.Background = resolveAddressToString(mapDescriptor.BackgroundAddr, binReader);
                mapDescriptor.FrbFile1 = resolveAddressToString(mapDescriptor.FrbFile1Addr, binReader);
                mapDescriptor.FrbFile2 = resolveAddressToString(mapDescriptor.FrbFile2Addr, binReader);
                mapDescriptor.FrbFile3 = resolveAddressToString(mapDescriptor.FrbFile3Addr, binReader);
                mapDescriptor.FrbFile4 = resolveAddressToString(mapDescriptor.FrbFile4Addr, binReader);
                mapDescriptor.readRotationOriginPoints(binReader, toFileAddress(mapDescriptor.MapSwitchParamAddr), data);
                mapDescriptor.readLoopingModeParams(binReader, toFileAddress(mapDescriptor.LoopingModeParamAddr));

                if (hackExpandedMapIconsApplied)
                {
                    mapDescriptor.MapIcon = resolveAddressAddressToString(mapDescriptor.MapIconAddrAddr, binReader);
                }
                else
                {
                    if (mapDescriptor.ID < 18)
                    {
                        mapDescriptor.MapIconAddrAddr = 0;
                        var number = Regex.Match(mapDescriptor.Background, @"\d+").Value;
                        mapDescriptor.MapIcon = "p_bg_" + number;
                    }
                }
            }
            UInt32[] descMsgIdTable = new UInt32[18 * 2];
            binReader.Seek(toFileAddress(data.START_MAP_DESCRIPTION_MSG_TABLE_VIRTUAL()), SeekOrigin.Begin);
            for (int i = 0; i < descMsgIdTable.Length; i++)
            {
                descMsgIdTable[i] = binReader.ReadUInt32();
            }
            var j = 0;
            foreach (MapDescriptor mapDescriptor in mapDescriptors)
            {
                if (mapDescriptor.ID < 18)
                {
                    mapDescriptor.Desc_MSG_ID = descMsgIdTable[j];
                    j++;
                    if (j == 18 && !hackExpandedDescriptionMessageTableApplied) // vanilla
                    {
                        j = 0;
                    }
                }
            }

            binReader.Seek(toFileAddress(data.START_VENTURE_CARD_TABLE_VIRTUAL()), SeekOrigin.Begin);
            for (int i = 0; i < 42; i++)
            {
                MapDescriptor mapDescriptor = mapDescriptors[i];
                mapDescriptor.readVentureCardTableFromStream(binReader);
            }

            return mapDescriptors;
        }

        public List<MapDescriptor> writeMainDol(EndianBinaryWriter stream, List<MapDescriptor> mapDescriptors)
        {
            if (mapDescriptors.Count != 48)
            {
                throw new ArgumentException("length of map descriptor list is not 48.");
            }

            // reset writing state
            reuseValues = new Dictionary<byte[], UInt32>(new ByteArrayComparer());
            unusedSpacePositionVirtual = data.UNUSED_SPACE_1_START_VIRTUAL();
            totalBytesWritten = 0;
            totalBytesLeft = 0;

            data.writeHackExtendedMapDescriptions(stream, toFileAddress);

            // HACK: remove the use of Map Difficulty and Map General Play Time to free up some space
            /* stream.Seek(toFileAddress(0x801fd9b8), SeekOrigin.Begin);
            stream.Write(NOP);
            stream.Seek(toFileAddress(0x80187168), SeekOrigin.Begin);
            stream.Write(NOP);
            stream.Seek(toFileAddress(0x801fd8b0), SeekOrigin.Begin);
            stream.Write(NOP);
            stream.Seek(toFileAddress(0x801fd954), SeekOrigin.Begin);
            stream.Write(NOP);
            */

            stream.Seek(toFileAddress(data.START_MAP_DATA_TABLE_VIRTUAL()), SeekOrigin.Begin);
            for (int i = 0; i < 48; i++)
            {
                int seek = (int)stream.BaseStream.Position;
                Console.WriteLine("WritePos=" + seek);
                MapDescriptor mapDescriptor = mapDescriptors[i];
                UInt32 internalNameAddr = 0;
                UInt32 backgroundAddr = 0;
                UInt32 frbFile1Addr = 0;
                UInt32 frbFile2Addr = 0;
                UInt32 frbFile3Addr = 0;
                UInt32 frbFile4Addr = 0;
                UInt32 mapSwitchParamAddr = 0;
                UInt32 loopingModeParamAddr = 0;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                    s.Write(mapDescriptor.InternalName);
                    s.Write((byte)0);
                    internalNameAddr = allocateUnusedSpace(memoryStream.ToArray(), stream);
                }
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                    s.Write(mapDescriptor.Background);
                    s.Write((byte)0);
                    backgroundAddr = allocateUnusedSpace(memoryStream.ToArray(), stream);
                }
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                    s.Write(mapDescriptor.FrbFile1);
                    s.Write((byte)0);
                    frbFile1Addr = allocateUnusedSpace(memoryStream.ToArray(), stream);
                }
                if (!string.IsNullOrEmpty(mapDescriptor.FrbFile2))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                        s.Write(mapDescriptor.FrbFile2);
                        s.Write((byte)0);
                        frbFile2Addr = allocateUnusedSpace(memoryStream.ToArray(), stream);
                    }
                }
                if (!string.IsNullOrEmpty(mapDescriptor.FrbFile3))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                        s.Write(mapDescriptor.FrbFile3);
                        s.Write((byte)0);
                        frbFile3Addr = allocateUnusedSpace(memoryStream.ToArray(), stream);
                    }
                }
                if (!string.IsNullOrEmpty(mapDescriptor.FrbFile4))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                        s.Write(mapDescriptor.FrbFile4);
                        s.Write((byte)0);
                        frbFile4Addr = allocateUnusedSpace(memoryStream.ToArray(), stream);
                    }
                }
                if (mapDescriptor.LoopingMode != LoopingMode.None)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                        mapDescriptor.writeLoopingModeParams(s);
                        loopingModeParamAddr = allocateUnusedSpace(memoryStream.ToArray(), stream);
                    }
                }
                if (mapDescriptor.SwitchRotationOriginPoints.Count != 0)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                        mapDescriptor.writeSwitchRotationOriginPoints(s);
                        mapSwitchParamAddr = allocateUnusedSpace(memoryStream.ToArray(), stream);
                    }
                }
                stream.Seek(seek, SeekOrigin.Begin);
                mapDescriptor.writeMapData(stream, internalNameAddr, backgroundAddr, frbFile1Addr, frbFile2Addr, frbFile3Addr, frbFile4Addr, mapSwitchParamAddr, loopingModeParamAddr, data.MAP_BGSEQUENCE_ADDR_MARIOSTADIUM());
            }

            stream.Seek(toFileAddress(data.START_MAP_DESCRIPTION_MSG_TABLE_VIRTUAL()), SeekOrigin.Begin);
            int j = 0;
            for (int i = 0; i < 48; i++)
            {
                if (mapDescriptors[i].ID < 18)
                {
                    stream.Write(mapDescriptors[i].Desc_MSG_ID);
                    j++;
                }
            }
            if (j != 18 * 2)
            {
                throw new DataMisalignedException("Expected to write " + 18 * 2 + " Description Message Ids, but wrote " + j + ".");
            }

            // Find out which map icons exist
            HashSet<string> allUniqueMapIcons = new HashSet<string>();
            for (int i = 0; i < 48; i++)
            {
                allUniqueMapIcons.Add(mapDescriptors[i].MapIcon);
            }
            allUniqueMapIcons.Add("p_bg_101");
            allUniqueMapIcons.Add("p_bg_109");
            allUniqueMapIcons.Add("p_bg_102");
            allUniqueMapIcons.Add("p_bg_105");
            allUniqueMapIcons.Add("p_bg_104");
            allUniqueMapIcons.Add("p_bg_106");
            allUniqueMapIcons.Add("p_bg_004");
            allUniqueMapIcons.Add("p_bg_008");
            allUniqueMapIcons.Add("p_bg_002");
            allUniqueMapIcons.Add("p_bg_001");
            allUniqueMapIcons.Add("p_bg_005");
            allUniqueMapIcons.Add("p_bg_003");
            allUniqueMapIcons.Add("p_bg_107");
            allUniqueMapIcons.Add("p_bg_006");
            allUniqueMapIcons.Add("p_bg_007");
            allUniqueMapIcons.Add("p_bg_009");
            allUniqueMapIcons.Add("p_bg_103");
            allUniqueMapIcons.Add("p_bg_108");
            var countUniqueMapIcons = allUniqueMapIcons.Count;

            // write each map icon into the main.dol and remember the location in the mapIcons dictionary
            Dictionary<string, UInt32> mapIcons = new Dictionary<string, UInt32>();
            foreach (string mapIcon in allUniqueMapIcons)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                    s.Write(mapIcon);
                    s.Write((byte)0);
                    var mapIconAddr = allocateUnusedSpace(memoryStream.ToArray(), stream);
                    mapIcons.Add(mapIcon, mapIconAddr);
                }
            }
            // write the map icon lookup table and remember the location of each pointer in the mapIconLookupTable dictionary
            UInt32 mapIconAddrTable = 0;
            Int16 mapIconAddrTableItemCount = (Int16)mapIcons.Count;
            Dictionary<string, UInt32> mapIconLookupTable = new Dictionary<string, UInt32>();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                UInt32 i = 0;
                foreach (var entry in mapIcons)
                {
                    var addr = entry.Value;
                    s.Write(addr);
                    mapIconLookupTable.Add(entry.Key, i);
                    i += 4;
                }
                mapIconAddrTable = allocateUnusedSpace(memoryStream.ToArray(), stream);
                foreach (var key in mapIconLookupTable.Keys.ToList())
                {
                    mapIconLookupTable[key] = mapIconLookupTable[key] + mapIconAddrTable;
                }
            }
            // pass the map icon addr as property for the map defaults table
            stream.Seek(toFileAddress(data.START_MAP_DEFAULTS_TABLE_VIRTUAL()), SeekOrigin.Begin);
            for (int i = 0; i < 48; i++)
            {
                mapDescriptors[i].writeMapDefaults(stream, mapIconLookupTable[mapDescriptors[i].MapIcon]);
            }

            data.writeHackCustomMapIcons(stream, toFileAddress, mapIconAddrTableItemCount, mapIconAddrTable);

            stream.Seek(toFileAddress(data.START_VENTURE_CARD_TABLE_VIRTUAL()), SeekOrigin.Begin);
            for (int i = 0; i < 42; i++)
            {
                mapDescriptors[i].writeVentureCardTable(stream);
            }

            nullTheFreeSpace(stream);

            return mapDescriptors;
        }

        private UInt32 allocateUnusedSpace(byte[] bytes, EndianBinaryWriter stream)
        {
            string str = HexUtil.byteArrayToString(bytes);
            if (reuseValues.ContainsKey(bytes))
            {
                Console.WriteLine("Allocate Reuse " + str + " at " + reuseValues[bytes].ToString("X"));
                return reuseValues[bytes];
            }
            else
            {
                UInt32 virtualAddr = unusedSpacePositionVirtual;
                unusedSpacePositionVirtual += (UInt32)bytes.Length;
                // align to 4
                while (unusedSpacePositionVirtual % 4 != 0)
                    unusedSpacePositionVirtual++;

                if (unusedSpacePositionVirtual > data.UNUSED_SPACE_4_END_VIRTUAL())
                {
                    throw new InsufficientMemoryException("There is not enough free space in the main.dol available.");
                }
                else if (unusedSpacePositionVirtual > data.UNUSED_SPACE_3_END_VIRTUAL() && unusedSpacePositionVirtual < data.UNUSED_SPACE_4_START_VIRTUAL())
                {
                    unusedSpacePositionVirtual = data.UNUSED_SPACE_4_START_VIRTUAL();
                    virtualAddr = unusedSpacePositionVirtual;
                    // align to 4
                    unusedSpacePositionVirtual += (UInt32)bytes.Length;
                    while (unusedSpacePositionVirtual % 4 != 0)
                        unusedSpacePositionVirtual++;
                }
                else if (unusedSpacePositionVirtual > data.UNUSED_SPACE_2_END_VIRTUAL() && unusedSpacePositionVirtual < data.UNUSED_SPACE_3_START_VIRTUAL())
                {
                    unusedSpacePositionVirtual = data.UNUSED_SPACE_3_START_VIRTUAL();
                    virtualAddr = unusedSpacePositionVirtual;
                    // align to 4
                    unusedSpacePositionVirtual += (UInt32)bytes.Length;
                    while (unusedSpacePositionVirtual % 4 != 0)
                        unusedSpacePositionVirtual++;
                }
                else if (unusedSpacePositionVirtual > data.UNUSED_SPACE_1_END_VIRTUAL() && unusedSpacePositionVirtual < data.UNUSED_SPACE_2_START_VIRTUAL())
                {
                    unusedSpacePositionVirtual = data.UNUSED_SPACE_2_START_VIRTUAL();
                    virtualAddr = unusedSpacePositionVirtual;
                    // align to 4
                    unusedSpacePositionVirtual += (UInt32)bytes.Length;
                    while (unusedSpacePositionVirtual % 4 != 0)
                        unusedSpacePositionVirtual++;
                }
                stream.Seek(toFileAddress(virtualAddr), SeekOrigin.Begin);
                stream.Write(bytes);
                totalBytesWritten += bytes.Length;
                byte[] fillUpToAlign = new byte[unusedSpacePositionVirtual - virtualAddr - bytes.Length];
                stream.Write(fillUpToAlign);
                totalBytesWritten += bytes.Length;

                reuseValues.Add(bytes, virtualAddr);
                Console.WriteLine("Allocate Fresh " + str + " at " + virtualAddr.ToString("X"));
                return virtualAddr;
            }
        }

        private void nullTheFreeSpace(EndianBinaryWriter stream)
        {
            UInt32 virtualAddr = unusedSpacePositionVirtual;
            stream.Seek(toFileAddress(virtualAddr), SeekOrigin.Begin);
            byte[] nullBytes;
            if (virtualAddr < data.UNUSED_SPACE_1_END_VIRTUAL())
            {
                nullBytes = new byte[data.UNUSED_SPACE_1_END_VIRTUAL() - virtualAddr];
                stream.Write(nullBytes);
                stream.Seek(toFileAddress(data.UNUSED_SPACE_2_START_VIRTUAL()), SeekOrigin.Begin);
                totalBytesLeft += nullBytes.Length;
            }
            if (virtualAddr < data.UNUSED_SPACE_2_END_VIRTUAL())
            {
                nullBytes = new byte[data.UNUSED_SPACE_2_END_VIRTUAL() - virtualAddr];
                stream.Write(nullBytes);
                stream.Seek(toFileAddress(data.UNUSED_SPACE_3_START_VIRTUAL()), SeekOrigin.Begin);
                totalBytesLeft += nullBytes.Length;
            }
            if (virtualAddr < data.UNUSED_SPACE_3_END_VIRTUAL())
            {
                nullBytes = new byte[data.UNUSED_SPACE_3_END_VIRTUAL() - virtualAddr];
                stream.Write(nullBytes);
                stream.Seek(toFileAddress(data.UNUSED_SPACE_4_START_VIRTUAL()), SeekOrigin.Begin);
                totalBytesLeft += nullBytes.Length;
            }
            if (virtualAddr < data.UNUSED_SPACE_4_END_VIRTUAL())
            {
                nullBytes = new byte[data.UNUSED_SPACE_4_END_VIRTUAL() - virtualAddr];
                stream.Write(nullBytes);
                totalBytesLeft += nullBytes.Length;
            }
        }

        private int search(byte[] src, byte[] pattern)
        {
            int c = src.Length - pattern.Length + 1;
            int j;
            for (int i = 0; i < c; i++)
            {
                if (src[i] != pattern[0]) continue;
                for (j = pattern.Length - 1; j >= 1 && src[i + j] == pattern[j]; j--) ;
                if (j == 0) return i;
            }
            return -1;
        }
    }
}
