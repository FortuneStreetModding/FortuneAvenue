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

        void ST7_Interface.writeHackExtendedMapDescriptions(EndianBinaryWriter stream, Func<UInt32, int> toFileAddress, Int16 mapDescriptionTableSize, UInt32 mapDescriptionTableAddr)
        {
            // HACK: Expand the description message ID table
            // subi r3,r3,0x15                                     -> nop
            stream.Seek(toFileAddress(0x8021214c), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // cmpwi r3,0x12                                       -> cmpwi r3,mapDescriptionTableSize
            stream.Seek(toFileAddress(0x80212158), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpwi(3, mapDescriptionTableSize));
            // r4 <- 0x80436bc0                                    -> r4 <- mapDescriptionTableAddr
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair(mapDescriptionTableAddr);
            stream.Seek(toFileAddress(0x80212164), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(4, v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi(4, 4, v.lower16Bit));
        }
        bool ST7_Interface.isHackExtendedMapDescriptions(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            stream.Seek(toFileAddress(0x8021214c), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            return opcode == PowerPcAsm.nop();
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
            var lis_opcode = stream.ReadUInt32();
            stream.Seek(4, SeekOrigin.Current);
            var addi_opcode = stream.ReadUInt32();
            return PowerPcAsm.make32bitValueFromPair(lis_opcode, addi_opcode);
        }
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
        void ST7_Interface.writeHackExtendedVentureCardTable(EndianBinaryWriter stream, Func<UInt32, int> toFileAddress, Int16 ventureCardTableEntryCount, UInt32 ventureCardCompressedTableAddr, UInt32 ventureCardDecompressTableRoutine)
        {
            // cmplwi r24,0x29                                     -> cmplwi r24,ventureCardTableCount-1
            stream.Seek(toFileAddress(0x8007e104), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmplwi(24, (UInt16)(ventureCardTableEntryCount - 1)));
            // mulli r0,r24,0x82                                   -> mulli r0,r24,0x10
            stream.Seek(toFileAddress(0x8007e11c), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(0, 24, 16));
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair(ventureCardCompressedTableAddr);
            // r4 <- 0x80410648                                    -> r4 <- ventureCardTableAddr
            stream.Seek(toFileAddress(0x8007e120), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(4, v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi(4, 4, v.lower16Bit));
            // li r5,0x0                                           -> bl ventureCardDecompressTableRoutine
            stream.Seek(toFileAddress(0x8007e130), SeekOrigin.Begin); stream.Write(PowerPcAsm.bl(0x8007e130, ventureCardDecompressTableRoutine));
        }
        void ST7_Interface.writeDecompressVentureCardTableRoutine(EndianBinaryWriter s, UInt32 ventureCardDecompressedTableAddr)
        {
            PowerPcAsm.Pair16Bit ventureCardTableAddrPair = PowerPcAsm.make16bitValuePair(ventureCardDecompressedTableAddr);
            ///
            /// assume: 
            /// r6 = ventureCardCompressedTableAddr
            /// 
            /// variables:
            /// r4 = ventureCardId
            /// r5 = ventureCardCompressedTableAddr
            /// r6 = ventureCardDecompressedTableAddr
            /// r7 = ventureCardCompressedWord
            /// r8 = bitIndex
            /// r0 = tmp / currentByte
            ///
            /// return:
            /// r6 = ventureCardDecompressedTableAddr
            ///
            s.Write(PowerPcAsm.li(4, 0));                                                           // ventureCardId = 0
            s.Write(PowerPcAsm.mr(5, 6));                                                           // r6 is ventureCardCompressedTableAddr at this point. Copy it to r5 with which we will be working
            s.Write(PowerPcAsm.lis(6, ventureCardTableAddrPair.upper16Bit));                        // \ load the ventureCardDecompressedTableAddr into r6. This address is
            s.Write(PowerPcAsm.addi(6, 6, ventureCardTableAddrPair.lower16Bit));                    // /  where we will store the decompressed venture card table.
            uint whileVentureCardIdSmaller128 = (uint)s.BaseStream.Position;                        // do {
            {                                                                                       //      
                s.Write(PowerPcAsm.li(0, 0));                                                       //     \ load the next compressed word from ventureCardCompressedTableAddr 
                s.Write(PowerPcAsm.lwzx(7, 5, 0));                                                  //     /  into r7. We will decompress the venture card table word by word.
                s.Write(PowerPcAsm.li(8, 0));                                                       //     bitIndex = 0
                uint whileBitIndexSmaller32 = (uint)s.BaseStream.Position;                          //     do 
                {                                                                                   //     {
                    s.Write(PowerPcAsm.mr(0, 7));                                                   //         get the current compressed word
                    s.Write(PowerPcAsm.srw(0, 0, 8));                                               //         shift it bitIndex times to the right
                    s.Write(PowerPcAsm.andi(0, 0, 1));                                              //         retrieve the lowest bit of it -> r0 contains the decompressed venture card byte now.
                    s.Write(PowerPcAsm.stbx(0, 4, 6));                                              //         store it into ventureCardDecompressedTableAddr[ventureCardId]
                    s.Write(PowerPcAsm.addi(8, 8, 1));                                              //         bitIndex++
                    s.Write(PowerPcAsm.addi(4, 4, 1));                                              //         ventureCardId++
                    s.Write(PowerPcAsm.cmpwi(8, 32));                                               //      
                    s.Write(PowerPcAsm.blt((uint)s.BaseStream.Position, whileBitIndexSmaller32));   //     } while(bitIndex < 32)
                }                                                                                   //     
                s.Write(PowerPcAsm.addi(5, 5, 4));                                                  //     ventureCardCompressedTableAddr += 4
                s.Write(PowerPcAsm.cmpwi(4, 128));                                                  //     
                s.Write(PowerPcAsm.blt((uint)s.BaseStream.Position, whileVentureCardIdSmaller128)); // } while(ventureCardId < 128)
            }                                                                                       //
            s.Write(PowerPcAsm.li(4, 0));                                                           // \ reset r4 = 0
            s.Write(PowerPcAsm.li(5, 0));                                                           // / reset r5 = 0
            s.Write(PowerPcAsm.blr());                                                              // return
        }
        UInt32 ST7_Interface.readVentureCardTableAddr(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            stream.Seek(toFileAddress(0x8007e120), SeekOrigin.Begin);
            var lis_opcode = stream.ReadUInt32();
            stream.Seek(4, SeekOrigin.Current);
            var addi_opcode = stream.ReadUInt32();
            return PowerPcAsm.make32bitValueFromPair(lis_opcode, addi_opcode);
        }
        Int16 ST7_Interface.readVentureCardTableEntryCount(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            stream.Seek(toFileAddress(0x8007e104), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            return (Int16)(PowerPcAsm.getOpcodeParameter(opcode) + 1);
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
