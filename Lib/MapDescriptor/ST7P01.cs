using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSEditor.MapDescriptor
{
    class ST7P01 : AddressConstants
    {
        public uint VANILLA_FIRST_MAP_DESC_MESSAGE_ID() { return 4416; }
        public uint VANILLA_FIRST_MAP_NAME_MESSAGE_ID() { return 5433; }
        public uint MAP_BGSEQUENCE_ADDR_MARIOSTADIUM() { return 0x80428968; }
        public uint MAP_SWITCH_PARAM_ADDR_COLLOSUS() { return 0x8047d598; }
        public uint MAP_SWITCH_PARAM_ADDR_MAGMAGEDDON() { return 0x806b8df0; }
        public uint MAP_SWITCH_PARAM_ADDR_OBSERVATORY() { return 0x8047d5b4; }
        public uint START_MAP_DATA_TABLE_VIRTUAL() { return 0x80428e50; }
        public uint START_MAP_DEFAULTS_TABLE_VIRTUAL() { return 0x804363c8; }
        public uint START_MAP_DESCRIPTION_MSG_TABLE_VIRTUAL() { return 0x80436bc0; }
        public uint START_VENTURE_CARD_TABLE_VIRTUAL() { return 0x80410648; }
        // map data string table repurposed as free space
        public uint UNUSED_SPACE_1_START_VIRTUAL() { return 0x80428978; }
        public uint UNUSED_SPACE_1_END_VIRTUAL() { return 0x80428e4F; }
        // unused costume string table 1
        public uint UNUSED_SPACE_2_START_VIRTUAL() { return 0x8042bc78; }
        public uint UNUSED_SPACE_2_END_VIRTUAL() { return 0x8042c23f; }
        // unused costume string table 2
        public uint UNUSED_SPACE_3_END_VIRTUAL() { return 0x8042e22f; }
        public uint UNUSED_SPACE_3_START_VIRTUAL() { return 0x8042dfc0; }
        // unused costume string table 3
        public uint UNUSED_SPACE_4_START_VIRTUAL() { return 0x8042ef30; }
        public uint UNUSED_SPACE_4_END_VIRTUAL() { return 0x8042f7ef; }

        public void writeHackExtendedMapDescriptions(EndianBinaryWriter stream, Func<UInt32, int> toFileAddress)
        {
            // HACK: Expand the description message ID table
            // subi r3,r3,0x15                                      ->   subi r3,r3,0x03
            stream.Seek(toFileAddress(0x8021214c), SeekOrigin.Begin); stream.Write(0x3863fffd);
            // cmpwi r3,0x12                                        ->   cmpwi r3,0x24
            stream.Seek(toFileAddress(0x80212158), SeekOrigin.Begin); stream.Write(0x2c030024);
            // lwzx r3=>DWORD_80436c08,r3,r0                        ->   li r3,0x1153
            stream.Seek(toFileAddress(0x80212238), SeekOrigin.Begin); stream.Write(0x38601153);
        }
        public bool isHackExtendedMapDescriptions(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            // has the hack for expanded Description message table already been applied?
            stream.Seek(toFileAddress(0x8021214c), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            return opcode == 0x3863fffd;
        }

        public void writeHackCustomMapIcons(EndianBinaryWriter stream, Func<UInt32, int> toFileAddress, Int16 mapIconAddrTableItemCount, UInt32 mapIconAddrTable)
        {
            // note: To add custom icons, the following files need to be editted as well:
            // - ui_menu_19_00a.brlyt
            // - 

            // custom map icon hack
            // bl GetMapOrigin                                     -> bl GetMapDifficulty
            stream.Seek(toFileAddress(0x8021e77c), SeekOrigin.Begin); stream.Write(0x4bff3629);
            // cmpw r28,r30                                        -> cmpw r29,r30
            stream.Seek(toFileAddress(0x8021e790), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpw_r29_r30);
            // cmplwi r28,0x12                                     -> cmplwi r28,mapIconAddrTableItemCount
            stream.Seek(toFileAddress(0x8021e7c0), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmplwi_r28(mapIconAddrTableItemCount));
            // bl GetMapOrigin                                     -> bl GetMapDifficulty
            stream.Seek(toFileAddress(0x8021e8a4), SeekOrigin.Begin); stream.Write(0x4bff3501);
            // cmpw r29,r28                                        -> cmpw r30,r28
            stream.Seek(toFileAddress(0x8021e8b8), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpw_r30_r28);
            // cmplwi r29,0x12                                     -> cmplwi r29,mapIconAddrTableItemCount
            stream.Seek(toFileAddress(0x8021e8e8), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmplwi_r29(mapIconAddrTableItemCount));
            // cmpw r29,r28                                        -> cmpw r30,r28
            stream.Seek(toFileAddress(0x8021e8b8), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpw_r30_r28);
            // cmplwi r28,0x12                                     -> cmplwi r28,mapIconAddrTableItemCount
            stream.Seek(toFileAddress(0x8021e84c), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmplwi_r28(mapIconAddrTableItemCount));
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make32bitValue(mapIconAddrTable);
            // r29 <- 0x8047f5c0                                   -> r29 <- mapIconAddrTable
            stream.Seek(toFileAddress(0x8021e780), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis_r29(v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi_r29(v.lower16Bit));
            // r30 <- 0x8047f5c0                                   -> r30 <- mapIconAddrTable
            stream.Seek(toFileAddress(0x8021e8a8), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis_r30(v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi_r30(v.lower16Bit));
            // r30 <- 0x8047f5c0                                   -> r30 <- mapIconAddrTable
            stream.Seek(toFileAddress(0x8021e830), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis_r30(v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi_r30(v.lower16Bit));

        }
        public bool isHackCustomMapIcons(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            // has the hack for expanded Description message table already been applied?
            stream.Seek(toFileAddress(0x8021e790), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            return opcode == PowerPcAsm.cmpw_r29_r30;
        }
    }
}
