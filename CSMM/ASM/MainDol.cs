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
        private List<MainDolSection> sections;
        public ST7_Interface data = new ST7P01();
        public FreeSpaceManager freeSpaceManager;

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

            int ventureCardTableCount = data.readVentureCardTableEntryCount(binReader, toFileAddress);
            binReader.Seek(toFileAddress(data.readVentureCardTableAddr(binReader, toFileAddress)), SeekOrigin.Begin);
            for (int i = 0; i < ventureCardTableCount; i++)
            {
                MapDescriptor mapDescriptor = mapDescriptors[i];
                mapDescriptor.readUncompressedVentureCardTableFromStream(binReader);
            }

            return mapDescriptors;
        }
        public List<MapDescriptor> writeMainDol(EndianBinaryWriter stream, List<MapDescriptor> mapDescriptors, IProgress<ProgressInfo> progress)
        {
            if (mapDescriptors.Count != 48)
            {
                throw new ArgumentException("length of map descriptor list is not 48.");
            }

            this.freeSpaceManager = data.getFreeSpaceManager();

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
                    internalNameAddr = freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, toFileAddress, progress);
                }
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                    s.Write(mapDescriptor.Background);
                    s.Write((byte)0);
                    backgroundAddr = freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, toFileAddress, progress);
                }
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                    s.Write(mapDescriptor.FrbFile1);
                    s.Write((byte)0);
                    frbFile1Addr = freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, toFileAddress, progress);
                }
                if (!string.IsNullOrEmpty(mapDescriptor.FrbFile2))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                        s.Write(mapDescriptor.FrbFile2);
                        s.Write((byte)0);
                        frbFile2Addr = freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, toFileAddress, progress);
                    }
                }
                if (!string.IsNullOrEmpty(mapDescriptor.FrbFile3))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                        s.Write(mapDescriptor.FrbFile3);
                        s.Write((byte)0);
                        frbFile3Addr = freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, toFileAddress, progress);
                    }
                }
                if (!string.IsNullOrEmpty(mapDescriptor.FrbFile4))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                        s.Write(mapDescriptor.FrbFile4);
                        s.Write((byte)0);
                        frbFile4Addr = freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, toFileAddress, progress);
                    }
                }
                if (mapDescriptor.LoopingMode != LoopingMode.None)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                        mapDescriptor.writeLoopingModeParams(s);
                        loopingModeParamAddr = freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, toFileAddress, progress, "Looping Mode Config");
                    }
                }
                if (mapDescriptor.SwitchRotationOriginPoints.Count != 0)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                        mapDescriptor.writeSwitchRotationOriginPoints(s);
                        mapSwitchParamAddr = freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, toFileAddress, progress, "Switch Rotation Origin Points");
                    }
                }
                stream.Seek(seek, SeekOrigin.Begin);
                mapDescriptor.writeMapData(stream, internalNameAddr, backgroundAddr, frbFile1Addr, frbFile2Addr, frbFile3Addr, frbFile4Addr, mapSwitchParamAddr, loopingModeParamAddr, data.MAP_BGSEQUENCE_ADDR_MARIOSTADIUM());
            }
            UInt32 mapDescriptionTableAddr;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                for (int i = 0; i < mapDescriptors.Count; i++)
                {
                    stream.Write(mapDescriptors[i].Desc_MSG_ID);
                }
                mapDescriptionTableAddr = freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, toFileAddress, progress, "Map Description Table");
            }
            data.writeHackExtendedMapDescriptions(stream, toFileAddress, (Int16)mapDescriptors.Count, mapDescriptionTableAddr);

            // Find out which map icons exist
            HashSet<string> allUniqueMapIcons = new HashSet<string>();
            for (int i = 0; i < 48; i++)
            {
                if (mapDescriptors[i].MapIcon != null)
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
                    var mapIconAddr = freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, toFileAddress, progress);
                    mapIcons.Add(mapIcon, mapIconAddr);
                }
            }
            // write the map icon lookup table and remember the location of each pointer in the mapIconLookupTable dictionary
            UInt32 mapIconAddrTable = 0;
            UInt16 mapIconAddrTableItemCount = (UInt16)mapIcons.Count;
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
                mapIconAddrTable = freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, toFileAddress, progress, "Map Icon String Lookup Table");
                foreach (var key in mapIconLookupTable.Keys.ToList())
                {
                    mapIconLookupTable[key] = mapIconLookupTable[key] + mapIconAddrTable;
                }
            }
            // pass the map icon addr as property for the map defaults table
            stream.Seek(toFileAddress(data.START_MAP_DEFAULTS_TABLE_VIRTUAL()), SeekOrigin.Begin);
            for (int i = 0; i < 48; i++)
            {
                if (string.IsNullOrEmpty(mapDescriptors[i].MapIcon))
                {
                    mapDescriptors[i].writeMapDefaults(stream, 0);
                }
                else
                {
                    mapDescriptors[i].writeMapDefaults(stream, mapIconLookupTable[mapDescriptors[i].MapIcon]);
                }
            }
            data.writeHackCustomMapIcons(stream, toFileAddress, mapIconAddrTableItemCount, mapIconAddrTable);

            // allocate working memory space for a single uncompressed venture card table which is passed on for the game to use. We will use it to store the result of decompressing a compressed venture card table
            UInt32 ventureCardDecompressedTableAddr;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                s.Write(new byte[130]);
                ventureCardDecompressedTableAddr = freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, toFileAddress, progress, "Venture Card Decompressed Table Reserved Space");
            }
            // write the decompressVentureCardTable routine which takes a compressed venture card table as input and writes it into ventureCardDecompressedTableAddr
            UInt32 ventureCardDecompressTableRoutine;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                data.writeDecompressVentureCardTableRoutine(s, ventureCardDecompressedTableAddr);
                ventureCardDecompressTableRoutine = freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, toFileAddress, progress, "Decompress Venture Card Code");
            }
            // write the compressed venture card table
            UInt32 ventureCardCompressedTableAddr;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                for (int i = 0; i < mapDescriptors.Count; i++)
                {
                    mapDescriptors[i].writeCompressedVentureCardTable(s);
                }
                ventureCardCompressedTableAddr = freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, toFileAddress, progress, "Compressed Venture Card Table");
            }
            // hijack the LoadBoard() routine. Intercept the moment when the (now compressed) ventureCardTable is passed on to the InitChanceBoard() routine. Call the 
            //  decompressVentureCardTable routine and pass the resulting decompressed ventureCardTable (located at ventureCardDecompressedTableAddr) to the InitChanceBoard() routine instead.
            data.writeHackExtendedVentureCardTable(stream, toFileAddress, (Int16)mapDescriptors.Count, ventureCardCompressedTableAddr, ventureCardDecompressTableRoutine);

            freeSpaceManager.nullTheFreeSpace(stream, toFileAddress);

            return mapDescriptors;
        }
    }
}
