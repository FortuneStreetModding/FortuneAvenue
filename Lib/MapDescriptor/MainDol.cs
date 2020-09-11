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

        public static readonly uint VANILLA_FIRST_MAP_NAME_MESSAGE_ID = 5433;
        public static readonly uint VANILLA_FIRST_MAP_DESC_MESSAGE_ID = 4416;

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

        public static readonly UInt32 START_MAP_DEFAULTS_TABLE_VIRTUAL = 0x804363c8;
        public static readonly UInt32 START_MAP_DATA_TABLE_VIRTUAL = 0x80428e50;
        public static readonly UInt32 START_VENTURE_CARD_TABLE_VIRTUAL = 0x80410648;
        public static readonly UInt32 START_MAP_DESCRIPTION_MSG_TABLE_VIRTUAL = 0x80436bc0;

        public static readonly uint TABLE_MAP_ICON_VIRTUAL = 0x8047f5c0;

        private Dictionary<byte[], UInt32> reuseValues;
        private UInt32 unusedSpacePositionVirtual;
        public int totalBytesWritten;
        public int totalBytesLeft;

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
            binReader.Seek(toFileAddress(START_MAP_DATA_TABLE_VIRTUAL), SeekOrigin.Begin);
            for (int i = 0; i < 48; i++)
            {
                MapDescriptor mapDescriptor = new MapDescriptor();
                mapDescriptor.readMapDataFromStream(binReader);
                mapDescriptors.Add(mapDescriptor);
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
                mapDescriptor.readRotationOriginPoints(binReader, toFileAddress(mapDescriptor.MapSwitchParamAddr));
                mapDescriptor.readLoopingModeParams(binReader, toFileAddress(mapDescriptor.LoopingModeParamAddr));

                UInt32 mapIconPos = TABLE_MAP_ICON_VIRTUAL + 4 * mapDescriptor.ID;
                binReader.Seek(toFileAddress(mapIconPos), SeekOrigin.Begin);
                UInt32 mapIconAddr = binReader.ReadUInt32();
                mapDescriptor.MapIcon = resolveAddressToString(mapIconAddr, binReader);
            }
            binReader.Seek(toFileAddress(START_MAP_DEFAULTS_TABLE_VIRTUAL), SeekOrigin.Begin);
            for (int i = 0; i < 48; i++)
            {
                MapDescriptor mapDescriptor = mapDescriptors[i];
                mapDescriptor.readMapDefaultsFromStream(binReader);
            }
            UInt32[] descMsgIdTable = new UInt32[18 * 2];
            binReader.Seek(toFileAddress(START_MAP_DESCRIPTION_MSG_TABLE_VIRTUAL), SeekOrigin.Begin);
            for (int i = 0; i < descMsgIdTable.Length; i++)
            {
                descMsgIdTable[i] = binReader.ReadUInt32();
            }
            // has the hack for expanded Description message table already been applied?
            binReader.Seek(toFileAddress(0x8021214c), SeekOrigin.Begin);
            var opcode = binReader.ReadUInt32();
            var hackExpandedDescriptionMessageTableApplied = opcode == 0x3863fffd;
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

            binReader.Seek(toFileAddress(START_VENTURE_CARD_TABLE_VIRTUAL), SeekOrigin.Begin);
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
            unusedSpacePositionVirtual = UNUSED_SPACE_1_START_VIRTUAL;
            totalBytesWritten = 0;
            totalBytesLeft = 0;

            // HACK: Expand the description message ID table
            // subi r3,r3,0x15                                      ->   subi r3,r3,0x03
            stream.Seek(toFileAddress(0x8021214c), SeekOrigin.Begin); stream.Write(0x3863fffd);
            // cmpwi r3,0x12                                        ->   cmpwi r3,0x24
            stream.Seek(toFileAddress(0x80212158), SeekOrigin.Begin); stream.Write(0x2c030024);
            // lwzx r3=>DWORD_80436c08,r3,r0                        ->   li r3,0x1153
            stream.Seek(toFileAddress(0x80212238), SeekOrigin.Begin); stream.Write(0x38601153);

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

            stream.Seek(toFileAddress(START_MAP_DATA_TABLE_VIRTUAL), SeekOrigin.Begin);
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
                mapDescriptor.writeMapData(stream, internalNameAddr, backgroundAddr, frbFile1Addr, frbFile2Addr, frbFile3Addr, frbFile4Addr, mapSwitchParamAddr, loopingModeParamAddr);
            }

            stream.Seek(toFileAddress(START_MAP_DEFAULTS_TABLE_VIRTUAL), SeekOrigin.Begin);
            for (int i = 0; i < 48; i++)
            {
                mapDescriptors[i].writeMapDefaults(stream);
            }

            stream.Seek(toFileAddress(START_VENTURE_CARD_TABLE_VIRTUAL), SeekOrigin.Begin);
            for (int i = 0; i < 42; i++)
            {
                mapDescriptors[i].writeVentureCardTable(stream);
            }

            stream.Seek(toFileAddress(START_MAP_DESCRIPTION_MSG_TABLE_VIRTUAL), SeekOrigin.Begin);
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

            // custom map icon hack
            /*stream.Seek(toFileAddress(0x8021e77c), SeekOrigin.Begin);
            stream.Write(0x4bff3629);
            stream.Seek(toFileAddress(0x8021e790), SeekOrigin.Begin);
            stream.Write(0x7c1df000);
            stream.Seek(toFileAddress(0x8021e8a4), SeekOrigin.Begin);
            stream.Write(0x4bff3629);
            stream.Seek(toFileAddress(0x8021e8b8), SeekOrigin.Begin);
            stream.Write(0x7c1df000);*/

            Dictionary<string, string> mapIcons = new Dictionary<string, string>();
            HashSet<string> allMapIcons = new HashSet<string>();
            for (int i = 0; i < 48; i++)
            {
                allMapIcons.Add(mapDescriptors[i].MapIcon);
            }
            foreach (string mapIcon in allMapIcons)
            {
                // allocateUnusedSpace(mapIcon.Length + 1, , stream);
                // stream.Write(mapIcon);
                // stream.Write(0);
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

                if (unusedSpacePositionVirtual > UNUSED_SPACE_4_END_VIRTUAL)
                {
                    throw new InsufficientMemoryException("There is not enough free space in the main.dol available.");
                }
                else if (unusedSpacePositionVirtual > UNUSED_SPACE_3_END_VIRTUAL && unusedSpacePositionVirtual < UNUSED_SPACE_4_START_VIRTUAL)
                {
                    unusedSpacePositionVirtual = UNUSED_SPACE_4_START_VIRTUAL;
                    virtualAddr = unusedSpacePositionVirtual;
                    unusedSpacePositionVirtual += (UInt32)bytes.Length;
                }
                else if (unusedSpacePositionVirtual > UNUSED_SPACE_2_END_VIRTUAL && unusedSpacePositionVirtual < UNUSED_SPACE_3_START_VIRTUAL)
                {
                    unusedSpacePositionVirtual = UNUSED_SPACE_3_START_VIRTUAL;
                    virtualAddr = unusedSpacePositionVirtual;
                    unusedSpacePositionVirtual += (UInt32)bytes.Length;
                }
                else if (unusedSpacePositionVirtual > UNUSED_SPACE_1_END_VIRTUAL && unusedSpacePositionVirtual < UNUSED_SPACE_2_START_VIRTUAL)
                {
                    unusedSpacePositionVirtual = UNUSED_SPACE_2_START_VIRTUAL;
                    virtualAddr = unusedSpacePositionVirtual;
                    unusedSpacePositionVirtual += (UInt32)bytes.Length;
                }
                stream.Seek(toFileAddress(virtualAddr), SeekOrigin.Begin);
                stream.Write(bytes);
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
            if (virtualAddr < UNUSED_SPACE_1_END_VIRTUAL)
            {
                nullBytes = new byte[UNUSED_SPACE_1_END_VIRTUAL - virtualAddr];
                stream.Write(nullBytes);
                stream.Seek(toFileAddress(UNUSED_SPACE_2_START_VIRTUAL), SeekOrigin.Begin);
                totalBytesLeft += nullBytes.Length;
            }
            if (virtualAddr < UNUSED_SPACE_2_END_VIRTUAL)
            {
                nullBytes = new byte[UNUSED_SPACE_2_END_VIRTUAL - virtualAddr];
                stream.Write(nullBytes);
                stream.Seek(toFileAddress(UNUSED_SPACE_3_START_VIRTUAL), SeekOrigin.Begin);
                totalBytesLeft += nullBytes.Length;
            }
            if (virtualAddr < UNUSED_SPACE_3_END_VIRTUAL)
            {
                nullBytes = new byte[UNUSED_SPACE_3_END_VIRTUAL - virtualAddr];
                stream.Write(nullBytes);
                stream.Seek(toFileAddress(UNUSED_SPACE_4_START_VIRTUAL), SeekOrigin.Begin);
                totalBytesLeft += nullBytes.Length;
            }
            if (virtualAddr < UNUSED_SPACE_4_END_VIRTUAL)
            {
                nullBytes = new byte[UNUSED_SPACE_4_END_VIRTUAL - virtualAddr];
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
