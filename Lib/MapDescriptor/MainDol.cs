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
        private readonly List<MainDolSection> sections;

        public static readonly string KEYWORD_START_MAP_DEFAULTS_TABLE = "6e 5f 62 75 79 5f 6d 73 67 00 76 65 63 74 6f 72 20 6c 65 6e 67 74 68 20 65 72 72 6f 72 00 00 62 " +
            "61 73 69 63 5f 73 74 72 69 6e 67 3a 3a 72 65 73 65 72 76 65 20 6c 65 6e 67 74 68 5f 65 72 72 6f 72 00 62 61 73 69 63 5f 73 74 72 69 6e 67 3a 20 6f 75 74 5f " +
            "6f 66 5f 72 61 6e 67 65 00 62 61 73 69 63 5f 73 74 72 69 6e 67 3a 20 6c 65 6e 67 74 68 5f 65 72 72 6f 72 00";
        public static readonly string KEYWORD_START_MAP_DATA_TABLE = "20 2a 2a 64 65 62 75 67 2a 2a 20 32 20 44 51 00 20 2a 2a 64 65 62 75 67 2a 2a 20 33 20 44 51 00";
        public static readonly string KEYWORD_VENTURE_CARD_TABLE = "33 34 35 36 37 38 39 3a 3c 3d 3e 3f 40 41 48 4b 4d 4e 4f 51 55 56 58 59 5b 5c 5f 60 62 63 64 04 12 1d " +
            "22 27 28 29 2a 2c 2d 2e 2f 30 31 3b 42 43 44 45 46 47 49 4a 4c 50 52 53 54 57 5a 5d 5e 61";
        private int positionMapDefaultTable;
        private int positionMapDataTable;
        private int positionVentureCardTable;

        public MainDol(List<MainDolSection> sections)
        {
            this.sections = sections;
        }

        private int toFileAddress(uint virtualAddress)
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

        private int toVirtualAddress(int fileAddress)
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

            byte[] pattern = HexUtil.HexStringToByteArray(KEYWORD_START_MAP_DEFAULTS_TABLE);
            positionMapDefaultTable = Search(buff, pattern) + pattern.Length;
            pattern = HexUtil.HexStringToByteArray(KEYWORD_START_MAP_DATA_TABLE);
            positionMapDataTable = Search(buff, pattern) + pattern.Length;
            pattern = HexUtil.HexStringToByteArray(KEYWORD_VENTURE_CARD_TABLE);
            positionVentureCardTable = Search(buff, pattern) + pattern.Length;
        }

        private string resolveAddressToString(uint address, EndianBinaryReader binReader)
        {
            int fileAddress = toFileAddress(address);
            if (fileAddress >= 0)
            {
                binReader.Seek(fileAddress, SeekOrigin.Begin);
                byte[] buff = binReader.ReadBytes(32);
                return HexUtil.ByteArrayToString(buff);
            }
            else
            {
                return null;
            }
        }

        public List<MapDescriptor> ReadMainDol(EndianBinaryReader binReader)
        {
            findTablePositions(binReader);

            List<MapDescriptor> mapDescriptors = new List<MapDescriptor>();
            binReader.Seek(positionMapDataTable, SeekOrigin.Begin);
            for (int i = 0; i < 48; i++)
            {
                MapDescriptor mapDescriptor = new MapDescriptor();
                mapDescriptor.ReadMapDataFromStream(binReader);
                mapDescriptors.Add(mapDescriptor);
            }
            foreach (MapDescriptor mapDescriptor in mapDescriptors)
            {
                mapDescriptor.Background = resolveAddressToString(mapDescriptor.BackgroundAddr, binReader);
                mapDescriptor.FrbFile1 = resolveAddressToString(mapDescriptor.FrbFile1Addr, binReader);
                mapDescriptor.FrbFile2 = resolveAddressToString(mapDescriptor.FrbFile2Addr, binReader);
                mapDescriptor.FrbFile3 = resolveAddressToString(mapDescriptor.FrbFile3Addr, binReader);
                mapDescriptor.FrbFile4 = resolveAddressToString(mapDescriptor.FrbFile4Addr, binReader);
                mapDescriptor.InternalName = resolveAddressToString(mapDescriptor.InternalNameAddr, binReader);
            }

            binReader.Seek(positionMapDefaultTable, SeekOrigin.Begin);
            for (int i = 0; i < 48; i++)
            {
                MapDescriptor mapDescriptor = mapDescriptors[i];
                mapDescriptor.ReadMapDefaultsFromStream(binReader);
            }

            binReader.Seek(positionVentureCardTable, SeekOrigin.Begin);
            for (int i = 0; i < 42; i++)
            {
                MapDescriptor mapDescriptor = mapDescriptors[i];
                mapDescriptor.ReadVentureCardTableFromStream(binReader);
            }

            return mapDescriptors;
        }

        private int Search(byte[] src, byte[] pattern)
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
