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

        private string resolveAddressAddressToString(uint virtualAddressAddress, EndianBinaryReader stream)
        {
            int fileAddress = toFileAddress(virtualAddressAddress);
            if (fileAddress >= 0)
            {
                stream.Seek(fileAddress, SeekOrigin.Begin);
                var virtualAddress = stream.ReadUInt32();
                return resolveAddressToString(virtualAddress, stream);
            }
            else
            {
                return null;
            }
        }

        private string resolveAddressToString(uint virtualAddress, EndianBinaryReader stream)
        {
            int fileAddress = toFileAddress(virtualAddress);
            if (fileAddress >= 0)
            {
                stream.Seek(fileAddress, SeekOrigin.Begin);
                byte[] buff = stream.ReadBytes(64);
                return HexUtil.byteArrayToString(buff);
            }
            else
            {
                return null;
            }
        }

        public List<MapDescriptor> readMainDol(EndianBinaryReader stream)
        {
            List<MapDescriptor> mapDescriptors = new List<MapDescriptor>();
            // has the hack for unique map icons been applied?
            var hackExpandedMapIconsApplied = data.isHackCustomMapIcons(stream, toFileAddress);

            stream.Seek(toFileAddress(data.START_MAP_DATA_TABLE_VIRTUAL()), SeekOrigin.Begin);
            for (int i = 0; i < 48; i++)
            {
                MapDescriptor mapDescriptor = new MapDescriptor();
                mapDescriptor.readMapDataFromStream(stream);
                mapDescriptors.Add(mapDescriptor);
            }
            stream.Seek(toFileAddress(data.START_MAP_DEFAULTS_TABLE_VIRTUAL()), SeekOrigin.Begin);
            for (int i = 0; i < 48; i++)
            {
                MapDescriptor mapDescriptor = mapDescriptors[i];
                mapDescriptor.readMapDefaultsFromStream(stream);
            }
            new DefaultGoalMoneyTable().read(stream, toFileAddress, mapDescriptors, null);

            new MapDescriptionTable().read(stream, toFileAddress, mapDescriptors, null);

            foreach (MapDescriptor mapDescriptor in mapDescriptors)
            {
                var internalName = resolveAddressToString(mapDescriptor.InternalNameAddr, stream).Trim();
                mapDescriptor.InternalName = Regex.Replace(internalName, @"[<>:/\|?*""]+", "");

                mapDescriptor.Background = resolveAddressToString(mapDescriptor.BackgroundAddr, stream);
                mapDescriptor.FrbFile1 = resolveAddressToString(mapDescriptor.FrbFile1Addr, stream);
                mapDescriptor.FrbFile2 = resolveAddressToString(mapDescriptor.FrbFile2Addr, stream);
                mapDescriptor.FrbFile3 = resolveAddressToString(mapDescriptor.FrbFile3Addr, stream);
                mapDescriptor.FrbFile4 = resolveAddressToString(mapDescriptor.FrbFile4Addr, stream);
                mapDescriptor.readRotationOriginPoints(stream, toFileAddress(mapDescriptor.MapSwitchParamAddr), data);
                mapDescriptor.readLoopingModeParams(stream, toFileAddress(mapDescriptor.LoopingModeParamAddr));

                if (hackExpandedMapIconsApplied)
                {
                    mapDescriptor.MapIcon = resolveAddressAddressToString(mapDescriptor.MapIconAddrAddr, stream);
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


            new VentureCardTable().read(stream, toFileAddress, mapDescriptors, null);

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

            new MapDescriptionTable().write(stream, toFileAddress, mapDescriptors, freeSpaceManager, progress);

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

            new VentureCardTable().write(stream, toFileAddress, mapDescriptors, freeSpaceManager, progress);

            freeSpaceManager.nullTheFreeSpace(stream, toFileAddress);

            return mapDescriptors;
        }
    }
}
