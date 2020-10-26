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
