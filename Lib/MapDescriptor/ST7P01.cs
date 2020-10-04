using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSEditor.MapDescriptor
{
    class ST7P01 : ST7_Interface
    {
        uint ST7_Interface.VANILLA_FIRST_MAP_DESC_MESSAGE_ID() { return 4416; }
        uint ST7_Interface.VANILLA_FIRST_MAP_NAME_MESSAGE_ID() { return 5433; }
        uint ST7_Interface.MAP_BGSEQUENCE_ADDR_MARIOSTADIUM() { return 0x80428968; }
        uint ST7_Interface.MAP_SWITCH_PARAM_ADDR_COLLOSUS() { return 0x8047d598; }
        uint ST7_Interface.MAP_SWITCH_PARAM_ADDR_MAGMAGEDDON() { return 0x806b8df0; }
        uint ST7_Interface.MAP_SWITCH_PARAM_ADDR_OBSERVATORY() { return 0x8047d5b4; }
        uint ST7_Interface.START_MAP_DATA_TABLE_VIRTUAL() { return 0x80428e50; }
        uint ST7_Interface.START_MAP_DEFAULTS_TABLE_VIRTUAL() { return 0x804363c8; }
        uint ST7_Interface.START_MAP_DESCRIPTION_MSG_TABLE_VIRTUAL() { return 0x80436bc0; }
        // TODO: could be repurpused as unused space, since the venture card table hack is being applied
        uint ST7_Interface.START_VENTURE_CARD_TABLE_VIRTUAL() { return 0x80410648; }
        public uint END_VENTURE_CARD_TABLE_VIRTUAL() { return 0x80411b9b; }
        // map data string table repurposed as free space
        uint ST7_Interface.UNUSED_SPACE_1_START_VIRTUAL() { return 0x80428978; }
        uint ST7_Interface.UNUSED_SPACE_1_END_VIRTUAL() { return 0x80428e4F; }
        // unused costume string table 1
        uint ST7_Interface.UNUSED_SPACE_2_START_VIRTUAL() { return 0x8042bc78; }
        uint ST7_Interface.UNUSED_SPACE_2_END_VIRTUAL() { return 0x8042c23f; }
        // unused costume string table 2
        uint ST7_Interface.UNUSED_SPACE_3_END_VIRTUAL() { return 0x8042e22f; }
        uint ST7_Interface.UNUSED_SPACE_3_START_VIRTUAL() { return 0x8042dfc0; }
        // unused costume string table 3
        uint ST7_Interface.UNUSED_SPACE_4_START_VIRTUAL() { return 0x8042ef30; }
        uint ST7_Interface.UNUSED_SPACE_4_END_VIRTUAL() { return 0x8042f7ef; }

        void ST7_Interface.writeHackExtendedMapDescriptions(EndianBinaryWriter stream, Func<UInt32, int> toFileAddress, Int16 mapDescriptionTableSize, UInt32 mapDescriptionTableAddr)
        {
            // HACK: Expand the description message ID table
            // subi r3,r3,0x15                                     -> nop
            stream.Seek(toFileAddress(0x8021214c), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop);
            // cmpwi r3,0x12                                       -> cmpwi r3,mapDescriptionTableSize
            stream.Seek(toFileAddress(0x80212158), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpwi_r3(mapDescriptionTableSize));
            // r4 <- 0x80436bc0                                    -> r4 <- mapDescriptionTableAddr
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair(mapDescriptionTableAddr);
            stream.Seek(toFileAddress(0x80212164), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis_r4(v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi_r4(v.lower16Bit));
            
            // -- instead of having a map lock info text per map, make it generic -- 
            // lwzx r3=>DWORD_80436c08,r3,r0                        ->   li r3,0x1153
            // we relocate the map description table somewhere else, as such this hack is not needed anymore
            // that is the reason why it is commented out here
            // stream.Seek(toFileAddress(0x80212238), SeekOrigin.Begin); stream.Write(0x38601153);
        }
        bool ST7_Interface.isHackExtendedMapDescriptions(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            stream.Seek(toFileAddress(0x8021214c), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            return opcode == PowerPcAsm.nop;
        }
        Int16 ST7_Interface.readMapDescriptionTableSize(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            stream.Seek(toFileAddress(0x80212158), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            return PowerPcAsm.getOpcodeParameter(opcode);
        }
        UInt32 ST7_Interface.readMapDescriptionTableAddr(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            stream.Seek(toFileAddress(0x80212164), SeekOrigin.Begin);
            var opcode1 = stream.ReadUInt32();
            stream.Seek(4, SeekOrigin.Current);
            var opcode2 = stream.ReadUInt32();
            return PowerPcAsm.make32bitValueFromPair(opcode1, opcode2);
        }
        void ST7_Interface.writeHackCustomMapIcons(EndianBinaryWriter stream, Func<UInt32, int> toFileAddress, Int16 mapIconAddrTableItemCount, UInt32 mapIconAddrTable)
        {
            // note: To add custom icons, the following files need to be editted as well:
            // - ui_menu_19_00a.brlyt with game_sequence.arc and within game_sequence_wifi.arc

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
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair(mapIconAddrTable);
            // r29 <- 0x8047f5c0                                   -> r29 <- mapIconAddrTable
            stream.Seek(toFileAddress(0x8021e780), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis_r29(v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi_r29(v.lower16Bit));
            // r30 <- 0x8047f5c0                                   -> r30 <- mapIconAddrTable
            stream.Seek(toFileAddress(0x8021e8a8), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis_r30(v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi_r30(v.lower16Bit));
            // r30 <- 0x8047f5c0                                   -> r30 <- mapIconAddrTable
            stream.Seek(toFileAddress(0x8021e830), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis_r30(v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi_r30(v.lower16Bit));

        }
        bool ST7_Interface.isHackCustomMapIcons(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            // has the hack for expanded Description message table already been applied?
            stream.Seek(toFileAddress(0x8021e790), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            return opcode == PowerPcAsm.cmpw_r29_r30;
        }
        void ST7_Interface.writeHackExtendedVentureCardTable(EndianBinaryWriter stream, Func<UInt32, int> toFileAddress, Int16 ventureCardTableEntryCount, UInt32 ventureCardTableAddr)
        {
            // cmplwi r24,0x29                                     -> cmplwi r24,ventureCardTableCount-1
            stream.Seek(toFileAddress(0x8007e104), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmplwi_r24((Int16)(ventureCardTableEntryCount - 1)));
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair(ventureCardTableAddr);
            // r4 <- 0x80410648                                    -> r4 <- ventureCardTableAddr
            stream.Seek(toFileAddress(0x8007e120), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis_r4(v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi_r4(v.lower16Bit));
        }
        UInt32 ST7_Interface.readVentureCardTableAddr(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            stream.Seek(toFileAddress(0x8007e120), SeekOrigin.Begin);
            var opcode1 = stream.ReadUInt32();
            stream.Seek(4, SeekOrigin.Current);
            var opcode2 = stream.ReadUInt32();
            return PowerPcAsm.make32bitValueFromPair(opcode1, opcode2);
        }
        Int16 ST7_Interface.readVentureCardTableEntryCount(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            stream.Seek(toFileAddress(0x8007e104), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            return (Int16)(PowerPcAsm.getOpcodeParameter(opcode) + 1);
        }

        void ST7_Interface.writeHackExtendedMapDefaultsTable(EndianBinaryWriter stream, Func<uint, int> toFileAddress)
        {
            throw new NotImplementedException();
        }

        bool ST7_Interface.isHackExtendedMapDefaultsTable(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            throw new NotImplementedException();
        }

        void ST7_Interface.writeHackExtendedMapSettingsTable(EndianBinaryWriter stream, Func<uint, int> toFileAddress)
        {
            throw new NotImplementedException();
        }

        bool ST7_Interface.isHackExtendedMapSettingsTable(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            throw new NotImplementedException();
        }
    }
}
