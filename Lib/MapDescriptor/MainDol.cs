using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MiscUtil.IO;
using MiscUtil.Conversion;
using FSEditor.FSData;

namespace FSEditor.MapDescriptor
{
    public class MainDol
    {
        private List<MainDolSection> sections;

        public static readonly UInt32 UNUSED_SPACE_1_START_VIRTUAL = 0x80428978;
        public static readonly UInt32 UNUSED_SPACE_1_END_VIRTUAL = 0x80428e4F;
        public static readonly UInt32 UNUSED_SPACE_2_START_VIRTUAL = 0x8042bc78;
        public static readonly UInt32 UNUSED_SPACE_2_END_VIRTUAL = 0x8042c23f;
        public static readonly UInt32 UNUSED_SPACE_3_START_VIRTUAL = 0x8042dfc0;
        public static readonly UInt32 UNUSED_SPACE_3_END_VIRTUAL = 0x8042e22f;
        public static readonly UInt32 UNUSED_SPACE_4_START_VIRTUAL = 0x8042ef30;
        public static readonly UInt32 UNUSED_SPACE_4_END_VIRTUAL = 0x8042f7ef;

        public static readonly UInt32 NOP = 0x60000000;

        public static readonly UInt32 MAP_SWITCH_PARAM_ADDR_MAGMAGEDDON = 0x806b8df0;
        public static readonly UInt32 MAP_SWITCH_PARAM_ADDR_COLLOSUS = 0x8047d598;
        public static readonly UInt32 MAP_SWITCH_PARAM_ADDR_OBSERVATORY = 0x8047d5b4;
        public static readonly UInt32 MAP_BGSEQUENCE_ADDR_MARIOSTADIUM = 0x80428968;

        public static readonly string KEYWORD_START_MAP_DEFAULTS_TABLE = "6e 5f 62 75 79 5f 6d 73 67 00 76 65 63 74 6f 72 20 6c 65 6e 67 74 68 20 65 72 72 6f 72 00 00 62 " +
            "61 73 69 63 5f 73 74 72 69 6e 67 3a 3a 72 65 73 65 72 76 65 20 6c 65 6e 67 74 68 5f 65 72 72 6f 72 00 62 61 73 69 63 5f 73 74 72 69 6e 67 3a 20 6f 75 74 5f " +
            "6f 66 5f 72 61 6e 67 65 00 62 61 73 69 63 5f 73 74 72 69 6e 67 3a 20 6c 65 6e 67 74 68 5f 65 72 72 6f 72 00";
        public static readonly string KEYWORD_START_MAP_DATA_TABLE = "20 2a 2a 64 65 62 75 67 2a 2a 20 32 20 44 51 00 20 2a 2a 64 65 62 75 67 2a 2a 20 33 20 44 51 00";
        public static readonly string KEYWORD_VENTURE_CARD_TABLE = "33 34 35 36 37 38 39 3a 3c 3d 3e 3f 40 41 48 4b 4d 4e 4f 51 55 56 58 59 5b 5c 5f 60 62 63 64 04 12 1d " +
            "22 27 28 29 2a 2c 2d 2e 2f 30 31 3b 42 43 44 45 46 47 49 4a 4c 50 52 53 54 57 5a 5d 5e 61";
        private int positionMapDefaultTable;
        private int positionMapDataTable;
        private int positionVentureCardTable;

        private UInt32 unusedSpacePositionVirtual = UNUSED_SPACE_1_START_VIRTUAL;

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

        private void findTablePositions(EndianBinaryReader binReader)
        {
            binReader.Seek(0, SeekOrigin.Begin);
            byte[] buff = StreamUtil.ReadFully(binReader.BaseStream);

            byte[] pattern = HexUtil.hexStringToByteArray(KEYWORD_START_MAP_DEFAULTS_TABLE);
            positionMapDefaultTable = search(buff, pattern) + pattern.Length;
            pattern = HexUtil.hexStringToByteArray(KEYWORD_START_MAP_DATA_TABLE);
            positionMapDataTable = search(buff, pattern) + pattern.Length;
            pattern = HexUtil.hexStringToByteArray(KEYWORD_VENTURE_CARD_TABLE);
            positionVentureCardTable = search(buff, pattern) + pattern.Length;
        }

        private string resolveAddressToString(uint address, EndianBinaryReader binReader)
        {
            int fileAddress = toFileAddress(address);
            if (fileAddress >= 0)
            {
                binReader.Seek(fileAddress, SeekOrigin.Begin);
                byte[] buff = binReader.ReadBytes(32);
                return HexUtil.byteArrayToString(buff);
            }
            else
            {
                return null;
            }
        }

        public List<MapDescriptor> readMainDol(EndianBinaryReader binReader)
        {
            findTablePositions(binReader);

            List<MapDescriptor> mapDescriptors = new List<MapDescriptor>();
            binReader.Seek(positionMapDataTable, SeekOrigin.Begin);
            for (int i = 0; i < 48; i++)
            {
                MapDescriptor mapDescriptor = new MapDescriptor();
                mapDescriptor.readMapDataFromStream(binReader);
                mapDescriptors.Add(mapDescriptor);
            }
            foreach (MapDescriptor mapDescriptor in mapDescriptors)
            {
                mapDescriptor.InternalName = resolveAddressToString(mapDescriptor.InternalNameAddr, binReader);
                mapDescriptor.Background = resolveAddressToString(mapDescriptor.BackgroundAddr, binReader);
                mapDescriptor.FrbFile1 = resolveAddressToString(mapDescriptor.FrbFile1Addr, binReader);
                mapDescriptor.FrbFile2 = resolveAddressToString(mapDescriptor.FrbFile2Addr, binReader);
                mapDescriptor.FrbFile3 = resolveAddressToString(mapDescriptor.FrbFile3Addr, binReader);
                mapDescriptor.FrbFile4 = resolveAddressToString(mapDescriptor.FrbFile4Addr, binReader);
                mapDescriptor.readRotationOriginPoints(binReader, toFileAddress(mapDescriptor.MapSwitchParamAddr));
                mapDescriptor.readLoopingModeParams(binReader, toFileAddress(mapDescriptor.LoopingModeParamAddr));
            }
            binReader.Seek(positionMapDefaultTable, SeekOrigin.Begin);
            for (int i = 0; i < 48; i++)
            {
                MapDescriptor mapDescriptor = mapDescriptors[i];
                mapDescriptor.readMapDefaultsFromStream(binReader);
            }

            binReader.Seek(positionVentureCardTable, SeekOrigin.Begin);
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

            stream.Seek(positionMapDataTable, SeekOrigin.Begin);
            for (int i = 0; i < 48; i++)
            {
                mapDescriptors[i].writeMapData(stream);
            }
            foreach (MapDescriptor mapDescriptor in mapDescriptors)
            {
                allocateUnusedSpace(mapDescriptor.InternalName.Length + 1, mapDescriptor.InternalNameAddrFilePos, stream);
                stream.Write(mapDescriptor.InternalName);
                stream.Write(0);
                allocateUnusedSpace(mapDescriptor.Background.Length + 1, mapDescriptor.BackgroundAddrFilePos, stream);
                stream.Write(mapDescriptor.Background);
                stream.Write(0);
                allocateUnusedSpace(mapDescriptor.FrbFile1.Length + 1, mapDescriptor.FrbFile1AddrFilePos, stream);
                stream.Write(mapDescriptor.FrbFile1);
                stream.Write(0);
                if (string.IsNullOrEmpty(mapDescriptor.FrbFile2))
                {
                    stream.Seek(mapDescriptor.FrbFile2AddrFilePos, SeekOrigin.Begin);
                    stream.Write(0);
                }
                else
                {
                    allocateUnusedSpace(mapDescriptor.FrbFile2.Length + 1, mapDescriptor.FrbFile2AddrFilePos, stream);
                    stream.Write(mapDescriptor.FrbFile2);
                }
                if (string.IsNullOrEmpty(mapDescriptor.FrbFile3))
                {
                    stream.Seek(mapDescriptor.FrbFile3AddrFilePos, SeekOrigin.Begin);
                    stream.Write(0);
                }
                else
                {
                    allocateUnusedSpace(mapDescriptor.FrbFile3.Length + 1, mapDescriptor.FrbFile3AddrFilePos, stream);
                    stream.Write(mapDescriptor.FrbFile3);
                }
                if (string.IsNullOrEmpty(mapDescriptor.FrbFile4))
                {
                    stream.Seek(mapDescriptor.FrbFile4AddrFilePos, SeekOrigin.Begin);
                    stream.Write(0);
                }
                else
                {
                    allocateUnusedSpace(mapDescriptor.FrbFile4.Length + 1, mapDescriptor.FrbFile4AddrFilePos, stream);
                    stream.Write(mapDescriptor.FrbFile4);
                }
                if (mapDescriptor.LoopingMode == LoopingMode.None)
                {
                    stream.Seek(mapDescriptor.LoopingModeParamAddrFilePos, SeekOrigin.Begin);
                    stream.Write(0);
                }
                else
                {
                    allocateUnusedSpace(mapDescriptor.getLoopingModeParamsSizeInBytes(), mapDescriptor.LoopingModeParamAddrFilePos, stream);
                    mapDescriptor.writeLoopingModeParams(stream);
                }
                if (mapDescriptor.SwitchRotationOriginPoints.Count == 0)
                {
                    stream.Seek(mapDescriptor.MapSwitchParamAddrFilePos, SeekOrigin.Begin);
                    stream.Write(0);
                }
                else
                {
                    allocateUnusedSpace(mapDescriptor.getSwitchRotationOriginPointsSizeInBytes(), mapDescriptor.MapSwitchParamAddrFilePos, stream);
                    mapDescriptor.writeSwitchRotationOriginPoints(stream);
                }
            }

            stream.Seek(positionMapDefaultTable, SeekOrigin.Begin);
            for (int i = 0; i < 48; i++)
            {
                mapDescriptors[i].writeMapDefaults(stream);
            }

            stream.Seek(positionVentureCardTable, SeekOrigin.Begin);
            for (int i = 0; i < 42; i++)
            {
                mapDescriptors[i].writeVentureCardTable(stream);
            }

            return mapDescriptors;
        }

        private void allocateUnusedSpace(int amountBytes, int pointerFilePosition, EndianBinaryWriter stream)
        {
            UInt32 virtualAddr = unusedSpacePositionVirtual;
            unusedSpacePositionVirtual += (UInt32)amountBytes;

            if (unusedSpacePositionVirtual > UNUSED_SPACE_4_END_VIRTUAL)
            {
                throw new InsufficientMemoryException("There is not enough free space in the main.dol available.");
            }
            else if (unusedSpacePositionVirtual > UNUSED_SPACE_3_END_VIRTUAL)
            {
                unusedSpacePositionVirtual = UNUSED_SPACE_4_START_VIRTUAL;
                virtualAddr = unusedSpacePositionVirtual;
                unusedSpacePositionVirtual += (UInt32)amountBytes;
            }
            else if (unusedSpacePositionVirtual > UNUSED_SPACE_2_END_VIRTUAL)
            {
                unusedSpacePositionVirtual = UNUSED_SPACE_3_START_VIRTUAL;
                virtualAddr = unusedSpacePositionVirtual;
                unusedSpacePositionVirtual += (UInt32)amountBytes;
            }
            else if (unusedSpacePositionVirtual > UNUSED_SPACE_1_END_VIRTUAL)
            {
                unusedSpacePositionVirtual = UNUSED_SPACE_2_START_VIRTUAL;
                virtualAddr = unusedSpacePositionVirtual;
                unusedSpacePositionVirtual += (UInt32)amountBytes;
            }

            stream.Seek(pointerFilePosition, SeekOrigin.Begin);
            stream.Write(virtualAddr);
            stream.Seek(toFileAddress(virtualAddr), SeekOrigin.Begin);
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
