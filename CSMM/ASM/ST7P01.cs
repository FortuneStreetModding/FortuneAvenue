using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomStreetManager
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
        uint ST7_Interface.ROUTINE_GET_MAP_DIFFICULTY_VIRTUAL() { return 0x80211da4; }

        void ST7_Interface.writeHackCustomMapIcons(EndianBinaryWriter stream, Func<UInt32, int> toFileAddress, UInt16 mapIconAddrTableItemCount, UInt32 mapIconAddrTable)
        {
            // note: To add custom icons, the following files need to be editted as well:
            // - ui_menu_19_00a.brlyt with game_sequence.arc and within game_sequence_wifi.arc

            // custom map icon hack
            // bl GetMapOrigin                                     -> bl GetMapDifficulty
            stream.Seek(toFileAddress(0x8021e77c), SeekOrigin.Begin); stream.Write(PowerPcAsm.bl(0x8021e77c, 0x80211da4));//stream.Write(0x4bff3629);
            // cmpw r28,r30                                        -> cmpw r29,r30
            stream.Seek(toFileAddress(0x8021e790), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpw(29, 30));
            // cmplwi r28,0x12                                     -> cmplwi r28,mapIconAddrTableItemCount
            stream.Seek(toFileAddress(0x8021e7c0), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmplwi(28, mapIconAddrTableItemCount));
            // bl GetMapOrigin                                     -> bl GetMapDifficulty
            stream.Seek(toFileAddress(0x8021e8a4), SeekOrigin.Begin); stream.Write(0x4bff3501);
            // cmpw r29,r28                                        -> cmpw r30,r28
            stream.Seek(toFileAddress(0x8021e8b8), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpw(30, 28));
            // cmplwi r29,0x12                                     -> cmplwi r29,mapIconAddrTableItemCount
            stream.Seek(toFileAddress(0x8021e8e8), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmplwi(29, mapIconAddrTableItemCount));
            // cmpw r29,r28                                        -> cmpw r30,r28
            stream.Seek(toFileAddress(0x8021e8b8), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpw(30, 28));
            // cmplwi r28,0x12                                     -> cmplwi r28,mapIconAddrTableItemCount
            stream.Seek(toFileAddress(0x8021e84c), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmplwi(28, mapIconAddrTableItemCount));
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair(mapIconAddrTable);
            // r29 <- 0x8047f5c0                                   -> r29 <- mapIconAddrTable
            stream.Seek(toFileAddress(0x8021e780), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(29, v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi(29, 29, v.lower16Bit));
            // r30 <- 0x8047f5c0                                   -> r30 <- mapIconAddrTable
            stream.Seek(toFileAddress(0x8021e8a8), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(30, v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi(30, 30, v.lower16Bit));
            // r30 <- 0x8047f5c0                                   -> r30 <- mapIconAddrTable
            stream.Seek(toFileAddress(0x8021e830), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(30, v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi(30, 30, v.lower16Bit));

        }
        bool ST7_Interface.isHackCustomMapIcons(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            // has the hack for expanded Description message table already been applied?
            stream.Seek(toFileAddress(0x8021e790), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            return opcode == PowerPcAsm.cmpw(29, 30);
        }


        void ST7_Interface.writeHackMapDefaultGoalMoneyTable(EndianBinaryWriter stream, Func<uint, int> toFileAddress, Int16 tableSize, UInt32 tableAddr)
        {
            UInt32 initMapGoalMoneyTourMode = 0x8020d160;
            UInt32 getMapDefaultGoalMode = 0x80211c88;
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair(tableAddr);

            // subi r30,r30,0x15                                    -> nop
            stream.Seek(toFileAddress(0x8020d0dc), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            stream.Seek(toFileAddress(initMapGoalMoneyTourMode), SeekOrigin.Begin);
            // li r0,0x15       ->  nop
            stream.Write(PowerPcAsm.nop());
            stream.Seek(8, SeekOrigin.Current);
            // mulli r0,r0,0x24 ->  mulli r0,r0,0x04
            stream.Write(PowerPcAsm.mulli(0,0,4));
            // r4 <- 804363c8   ->  r4 <- tableAddr
            stream.Write(PowerPcAsm.lis(4, v.upper16Bit)); stream.Write(PowerPcAsm.addi(4, 4, v.lower16Bit));
            // mulli r3,r30,0x24 ->  mulli r3,r30,0x04
            stream.Write(PowerPcAsm.mulli(3, 30, 4));

            // subi r31,r3,0x15                                    -> nop
            stream.Seek(toFileAddress(0x80211c04), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            stream.Seek(toFileAddress(getMapDefaultGoalMode), SeekOrigin.Begin);
            // li r3,0x15       ->  nop
            stream.Write(PowerPcAsm.nop());
            stream.Seek(8, SeekOrigin.Current);
            // mulli r0,r3,0x24 ->  mulli r0,r3,0x04
            stream.Write(PowerPcAsm.mulli(0, 3, 4));
            // r4 <- 804363c8   ->  r4 <- tableAddr
            stream.Write(PowerPcAsm.lis(4, v.upper16Bit)); stream.Write(PowerPcAsm.addi(4, 4, v.lower16Bit));
            // mulli r3,r31,0x24 ->  mulli r3,r31,0x04
            stream.Write(PowerPcAsm.mulli(3, 31, 4));
        }

        void ST7_Interface.writeHackExtendedMapDefaultsTable(EndianBinaryWriter stream, Func<uint, int> toFileAddress, Int16 mapDefaultsTableSize, UInt32 mapDefaultsTableAddr)
        {
            // -- SetupWorldMap() --
            // subi r3,r3,0x15                                     -> nop
            stream.Seek(toFileAddress(0x8020cec8), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // cmpwi r3,0x12                                       -> cmpwi r3,mapDefaultsTableSize
            stream.Seek(toFileAddress(0x8020ced4), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpwi(3, mapDefaultsTableSize));
            // r3 <- 0x804363c8                                    -> r3 <- mapDefaultsTableAddr
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair(mapDefaultsTableAddr);
            stream.Seek(toFileAddress(0x8020cee4), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(3, v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi(3, 3, v.lower16Bit));
            // li r0,0x15                                          -> nop
            stream.Seek(toFileAddress(0x8020cf68), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());

            // r4 <- 0x804363c8                                    -> r4 <- mapDefaultsTableAddr
            stream.Seek(toFileAddress(0x8020d170), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(4, v.upper16Bit)); stream.Write(PowerPcAsm.addi(4, 4, v.lower16Bit));
            // li r0,0x15                                          -> nop
            stream.Seek(toFileAddress(0x8020d248), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // r3 <- 0x804363c8                                    -> r3 <- mapDefaultsTableAddr
            stream.Seek(toFileAddress(0x8020d258), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(3, v.upper16Bit)); stream.Write(PowerPcAsm.addi(3, 3, v.lower16Bit));
            // r4 <- 0x804363c8                                    -> r4 <- mapDefaultsTableAddr
            stream.Seek(toFileAddress(0x8020d344), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(4, v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi(4, 4, v.lower16Bit));

            // cmpwi r30,0x12                                      -> cmpwi r30,mapDefaultsTableSize
            stream.Seek(toFileAddress(0x8020d0e8), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpwi(30, mapDefaultsTableSize));
            // subi r30,r30,0x15                                   -> nop
            stream.Seek(toFileAddress(0x8020d1c4), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // cmpwi r30,0x12                                      -> cmpwi r30,mapDefaultsTableSize
            stream.Seek(toFileAddress(0x8020d1d0), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpwi(30, mapDefaultsTableSize));
            // subi r30,r30,0x15                                   -> nop
            stream.Seek(toFileAddress(0x8020d2b0), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // cmpwi r30,0x12                                      -> cmpwi r30,mapDefaultsTableSize
            stream.Seek(toFileAddress(0x8020d2bc), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpwi(30, mapDefaultsTableSize));
            // li r0,0x15                                          -> nop
            stream.Seek(toFileAddress(0x8020d334), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // -- GetMapOpponents() --
            // subi r4,r4,0x15                                     -> nop
            stream.Seek(toFileAddress(0x80211af4), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // cmpwi r4,0x12                                       -> cmpwi r4,mapDefaultsTableSize
            stream.Seek(toFileAddress(0x80211b00), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpwi(4, mapDefaultsTableSize));
            // r4 <- 0x804363c8                                    -> r4 <- mapDefaultsTableAddr
            stream.Seek(toFileAddress(0x80211b10), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(4, v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi(4, 4, v.lower16Bit));
            // li r3,0x15                                          -> nop
            stream.Seek(toFileAddress(0x80211b94), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
        }

        UInt32 ST7_Interface.readMapDefaultsTableAddr(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            stream.Seek(toFileAddress(0x8020cee4), SeekOrigin.Begin);
            var lis_opcode = stream.ReadUInt32();
            stream.Seek(4, SeekOrigin.Current);
            var addi_opcode = stream.ReadUInt32();
            return PowerPcAsm.make32bitValueFromPair(lis_opcode, addi_opcode);
        }

        void ST7_Interface.writeHackExtendedMapSettingsTable(EndianBinaryWriter stream, Func<uint, int> toFileAddress)
        {
            throw new NotImplementedException();
        }

        bool ST7_Interface.isHackExtendedMapSettingsTable(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            throw new NotImplementedException();
        }

        FreeSpaceManager ST7_Interface.getFreeSpaceManager()
        {
            var freeSpaceManager = new FreeSpaceManager();
            // Venture Card Table
            freeSpaceManager.addFreeSpace(0x80410648, 0x80411b9b);
            // Map Data String Table
            freeSpaceManager.addFreeSpace(0x80428978, 0x80428e4f);
            // Unused costume string table 1
            freeSpaceManager.addFreeSpace(0x8042bc78, 0x8042c23f);
            // Unused costume string table 2
            freeSpaceManager.addFreeSpace(0x8042dfc0, 0x8042e22f);
            // Unused costume string table 3
            freeSpaceManager.addFreeSpace(0x8042ef30, 0x8042f7ef);
            // Expanded Rom
            // freeSpaceManager.addFreeSpace(0x80001800, 0x80001800 + 0x1800);
            return freeSpaceManager;
        }
    }
}
