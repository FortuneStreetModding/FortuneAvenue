using MiscUtil.Conversion;
using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CustomStreetManager
{
    public class EventSquare : DolIO
    {
        protected override void writeAsm(EndianBinaryWriter stream, AddressMapper addressMapper, List<MapDescriptor> mapDescriptors)
        {
            // this is the virtual address for the ForceVentureCardVariable: 0x804363c4
            // instead of allocating space for the venture card variable with the free space manager, we use a fixed virtual address
            // so that this variable can be reused by other hacks outside of CSMM. 
            var forceVentureCardVariable = addressMapper.toVersionAgnosticAddress((BSVAddr)0x804363c4);
            stream.Seek(addressMapper.toFileAddress(forceVentureCardVariable), SeekOrigin.Begin);
            // write zeroes to it
            stream.Write(new byte[4]);

            // --- Model ---
            // some examples:
            //   80087f88 = Property
            //   80088040 = Bank
            //   80088100 = Default
            //   80088050 = Take a break
            //   80088068 = Stockbroker
            //   80088048 = Arcade
            //   80088060 = Switch
            //   80088058 = Cannon
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80453330), SeekOrigin.Begin);
            stream.Write((UInt32)addressMapper.toVersionAgnosticAddress((BSVAddr)0x80088060));

            // --- Texture ---
            var customTextureHandler = allocate(writeGetTextureForCustomSquareRoutine(15, 14), "GetTextureForCustomSquareRoutine");
            var virtualPos = addressMapper.toVersionAgnosticAddress((BSVAddr)0x80086d98);
            stream.Seek(addressMapper.toFileAddress(virtualPos), SeekOrigin.Begin);
            // li r15,0x1        -> bl customTextureHandler
            stream.Write(PowerPcAsm.bl(virtualPos, customTextureHandler));

            customTextureHandler = allocate(writeGetTextureForCustomSquareRoutine(31, 30), "GetTextureForCustomSquareRoutine2");
            virtualPos = addressMapper.toVersionAgnosticAddress((BSVAddr)0x80087a24);
            stream.Seek(addressMapper.toFileAddress(virtualPos), SeekOrigin.Begin);
            // li r31,0x1        -> bl customTextureHandler
            stream.Write(PowerPcAsm.bl(virtualPos, customTextureHandler));

            // --- Icon ---
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x804160c8), SeekOrigin.Begin);
            stream.Write((uint)addressMapper.toVersionAgnosticAddress((BSVAddr)0x80415ee0)); // pointer to the texture name (0x80415ee0 points to the texture "p_mark_21" which is the switch square icon
            stream.Write((uint)addressMapper.toVersionAgnosticAddress((BSVAddr)0x80415ee0)); // we need to write it twice: once for each design type (Mario and DragonQuest)

            // --- Name ---
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80475580), SeekOrigin.Begin);
            stream.Write(3336); // id of the message in ui_message.csv (3336 = "Switch square")

            // --- Description ---
            var customDescriptionRoutine = allocate(writeGetDescriptionForCustomSquareRoutine(addressMapper, (VAVAddr)0), "GetDescriptionForCustomSquareRoutine");
            stream.Seek(addressMapper.toFileAddress(customDescriptionRoutine), SeekOrigin.Begin);
            stream.Write(writeGetDescriptionForCustomSquareRoutine(addressMapper, customDescriptionRoutine)); // re-write the routine again since now we know where it is located in the main dol

            virtualPos = addressMapper.toVersionAgnosticAddress((BSVAddr)0x800f8ce4);
            stream.Seek(addressMapper.toFileAddress(virtualPos), SeekOrigin.Begin);
            // bl Game::uitext::get_string   -> bl customDescriptionRoutine
            stream.Write(PowerPcAsm.bl(virtualPos, customDescriptionRoutine));

            virtualPos = addressMapper.toVersionAgnosticAddress((BSVAddr)0x800f8d6c);
            stream.Seek(addressMapper.toFileAddress(virtualPos), SeekOrigin.Begin);
            // bl Game::uitext::get_string   -> bl customDescriptionRoutine
            stream.Write(PowerPcAsm.bl(virtualPos, customDescriptionRoutine));

            virtualPos = addressMapper.toVersionAgnosticAddress((BSVAddr)0x800f8dd8);
            stream.Seek(addressMapper.toFileAddress(virtualPos), SeekOrigin.Begin);
            // bl Game::uitext::get_string   -> bl customDescriptionRoutine
            stream.Write(PowerPcAsm.bl(virtualPos, customDescriptionRoutine));

            virtualPos = addressMapper.toVersionAgnosticAddress((BSVAddr)0x800f8e5c);
            stream.Seek(addressMapper.toFileAddress(virtualPos), SeekOrigin.Begin);
            // bl Game::uitext::get_string   -> bl customDescriptionRoutine
            stream.Write(PowerPcAsm.bl(virtualPos, customDescriptionRoutine));

            virtualPos = addressMapper.toVersionAgnosticAddress((BSVAddr)0x800f8ee4);
            stream.Seek(addressMapper.toFileAddress(virtualPos), SeekOrigin.Begin);
            // bl Game::uitext::get_string   -> bl customDescriptionRoutine
            stream.Write(PowerPcAsm.bl(virtualPos, customDescriptionRoutine));

            virtualPos = addressMapper.toVersionAgnosticAddress((BSVAddr)0x800f8f4c);
            stream.Seek(addressMapper.toFileAddress(virtualPos), SeekOrigin.Begin);
            // bl Game::uitext::get_string   -> bl customDescriptionRoutine
            stream.Write(PowerPcAsm.bl(virtualPos, customDescriptionRoutine));

            // --- Behavior ---
            // the idea is that whenever someone stops at the event square, it sets our custom variable "ForceVentureCardVariable" to the id of the venture card and changes to the Venture Card Mode (0x1c). The custom variable is used to remember which venture card should be played the next time a venture card is executed.
            var procStopEventSquareRoutine = allocate(writeProcStopEventSquareRoutine(addressMapper, forceVentureCardVariable, (VAVAddr)0), "procStopEventSquareRoutine");
            stream.Seek(addressMapper.toFileAddress(procStopEventSquareRoutine), SeekOrigin.Begin);
            stream.Write(writeProcStopEventSquareRoutine(addressMapper, forceVentureCardVariable, procStopEventSquareRoutine)); // re-write the routine again since now we know where it is located in the main dol

            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80475838), SeekOrigin.Begin);
            stream.Write((UInt32)procStopEventSquareRoutine);

            // --- Hijack Venture Card Mode ---
            // We are hijacking the execute venture card mode (0x1f) to check if our custom variable "ForceVentureCardVariable" has been set to anything other than 0. If it was, then setup that specific venture card to be executed.
            var forceFetchFakeVentureCard = allocate(writeSubroutineForceFetchFakeVentureCard(forceVentureCardVariable), "forceFetchFakeVentureCard");
            virtualPos = addressMapper.toVersionAgnosticAddress((BSVAddr)0x801b7f44);
            stream.Seek(addressMapper.toFileAddress(virtualPos), SeekOrigin.Begin);
            // li r4,-0x1   -> bl forceFetchFakeVentureCard
            stream.Write(PowerPcAsm.bl(virtualPos, forceFetchFakeVentureCard));
            // li r8,0x3    -> nop
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801b7f74), SeekOrigin.Begin);
            stream.Write(PowerPcAsm.nop());
        }

        private List<UInt32> writeGetDescriptionForCustomSquareRoutine(AddressMapper addressMapper, VAVAddr routineStartAddress)
        {
            var asm = new List<UInt32>();
            var gameUiTextGetString = addressMapper.toVersionAgnosticAddress((BSVAddr)0x800f78dc);
            var gameUiTextGetCardMsg = addressMapper.toVersionAgnosticAddress((BSVAddr)0x800f837c);
            var gameBoard = PowerPcAsm.make16bitValuePair((UInt32)addressMapper.toVersionAgnosticAddress((BSVAddr)0x8054d018));

            asm.Add(PowerPcAsm.lis(7, gameBoard.upper16Bit));                                // 
            asm.Add(PowerPcAsm.addi(7, 7, gameBoard.lower16Bit));                            // r7 <- start of gameboard table containing all squares
            asm.Add(PowerPcAsm.mulli(8, 24, 0x54));                                          // r8 <- squareId * 0x54 (the size of each square)
            asm.Add(PowerPcAsm.add(6, 7, 8));                                                // r6 <- the current square
            asm.Add(PowerPcAsm.lbz(8, 0x4d, 6));                                             // r8 <- square.squareType
            asm.Add(PowerPcAsm.cmpwi(8, 0x2e));                                              // if(square.squareType == 0x2e) 
            asm.Add(PowerPcAsm.bne(3));                                                      // {
            asm.Add(PowerPcAsm.lbz(4, 0x18, 6));                                             //   r4 <- square.district_color
            asm.Add(PowerPcAsm.b(routineStartAddress, asm.Count, gameUiTextGetCardMsg));     //   goto Game::uitext::get_card_message(r4)
                                                                                             // }
            asm.Add(PowerPcAsm.li(6, 0x0));                                                  // \
            asm.Add(PowerPcAsm.li(7, 0x0));                                                  // | No message arguments
            asm.Add(PowerPcAsm.li(8, 0x0));                                                  // /
            asm.Add(PowerPcAsm.b(routineStartAddress, asm.Count, gameUiTextGetString));      // goto Game::uitext::get_string(r4, 0, 0, 0)

            return asm;
        }

        private List<UInt32> writeGetTextureForCustomSquareRoutine(byte register_textureType, byte register_squareType)
        {
            var asm = new List<UInt32>();
            asm.Add(PowerPcAsm.li(register_textureType, 0x1));    // textureType = 1
            asm.Add(PowerPcAsm.cmpwi(register_squareType, 0x2e)); // if(squareType == 0x2e)
            asm.Add(PowerPcAsm.beq(2));                           // {
            asm.Add(PowerPcAsm.blr());                            //   return textureType; 
                                                                  // } else {
            asm.Add(PowerPcAsm.li(register_textureType, 0x5));    //   textureType = 5
            asm.Add(PowerPcAsm.blr());                            //   return textureType;
                                                                  // }
            return asm;
        }

        private List<UInt32> writeProcStopEventSquareRoutine(AddressMapper addressMapper, VAVAddr forceVentureCardVariable, VAVAddr routineStartAddress)
        {
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair((UInt32)forceVentureCardVariable);
            var gameProgressChangeModeRoutine = addressMapper.toVersionAgnosticAddress((BSVAddr)0x800c093c);
            var endOfSwitchCase = addressMapper.toVersionAgnosticAddress((BSVAddr)0x800fac38);

            var asm = new List<UInt32>();
            asm.Add(PowerPcAsm.lwz(3, 0x188, 28));         // \
            asm.Add(PowerPcAsm.lwz(3, 0x74, 3));           // / r3_place = gameChara.currentPlace
            asm.Add(PowerPcAsm.lbz(6, 0x18, 3));           // | r6_ventureCardId = r3_place.districtId

            asm.Add(PowerPcAsm.lis(3, v.upper16Bit));      // \
            asm.Add(PowerPcAsm.addi(3, 3, v.lower16Bit));  // | forceVentureCardVariable <- r6_ventureCardId
            asm.Add(PowerPcAsm.stw(6, 0x0, 3));            // / 

            asm.Add(PowerPcAsm.lwz(3, 0x18, 20));          // \ lwz r3,0x18(r20)
            asm.Add(PowerPcAsm.li(4, 0x1f));               // | li r4,0x1f  (the GameProgress mode id 0x1f is for executing a venture card)
            asm.Add(PowerPcAsm.li(5, -0x1));               // | li r5,-0x1
            asm.Add(PowerPcAsm.li(6, -0x1));               // | li r6,-0x1
            asm.Add(PowerPcAsm.li(7, 0x0));                // | li r7,0x0
            asm.Add(PowerPcAsm.bl(routineStartAddress, asm.Count, gameProgressChangeModeRoutine));        // | bl Game::GameProgress::changeMode
            asm.Add(PowerPcAsm.b(routineStartAddress, asm.Count, endOfSwitchCase));        // / goto end of switch case 
            return asm;
        }

        private List<UInt32> writeSubroutineForceFetchFakeVentureCard(VAVAddr fakeVentureCard)
        {
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair((UInt32)fakeVentureCard);

            // precondition: r3 is ChanceCardUI *
            // ChanceCardUI->field_0x34 is ChanceBoard *
            // ChanceBoard->field_0x158 is current venture card id

            var asm = new List<UInt32>();
            asm.Add(PowerPcAsm.lis(6, v.upper16Bit));      // \
            asm.Add(PowerPcAsm.addi(6, 6, v.lower16Bit));  // / r6 <- forceVentureCardVariable
            asm.Add(PowerPcAsm.lwz(4, 0x0, 6));            // | r4 <- forceVentureCard
            asm.Add(PowerPcAsm.cmpwi(4, 0x0));             // | if(forceVentureCard != 0)
            asm.Add(PowerPcAsm.beq(7));                    // | {
            asm.Add(PowerPcAsm.lwz(5, 0x34, 3));           // |   r5 <- ChanceCardUI.ChanceBoard
            asm.Add(PowerPcAsm.stw(4, 0x158, 5));          // |   ChanceBoard.currentVentureCardId <- r4
            asm.Add(PowerPcAsm.li(5, 0));                  // |\  forceVentureCard <- 0
            asm.Add(PowerPcAsm.stw(5, 0x0, 6));            // |/ 
            asm.Add(PowerPcAsm.li(8, 0x0));                // |   r8 <- 0 (the venture card is initialized)
            asm.Add(PowerPcAsm.blr());                     // |   return r4 and r8
                                                           // | }
            asm.Add(PowerPcAsm.li(4, -0x1));               // | r4 <- -1
            asm.Add(PowerPcAsm.li(8, 0x3));                // | r8 <- 3 (the venture card is continued to be executed)
            asm.Add(PowerPcAsm.blr());                     // | return r4 and r8


            return asm;
        }

        protected override void readAsm(EndianBinaryReader stream, List<MapDescriptor> mapDescriptors, AddressMapper addressMapper)
        {
            Debug.WriteLine("0x471918 " + addressMapper.fileAddressToBoomStreetVirtualAddress(0x471918));
            Debug.WriteLine("0xf9e7c " + addressMapper.fileAddressToBoomStreetVirtualAddress(0xf9e7c));
            Debug.WriteLine("0x44f410 " + addressMapper.fileAddressToBoomStreetVirtualAddress(0x44f410));
            Debug.WriteLine("0x82138 " + addressMapper.fileAddressToBoomStreetVirtualAddress(0x82138));
            Debug.WriteLine("0xf9f00 " + addressMapper.fileAddressToBoomStreetVirtualAddress(0xf9f00));
            Debug.WriteLine("0x82dc4 " + addressMapper.fileAddressToBoomStreetVirtualAddress(0x82dc4));
            Debug.WriteLine("0xf9f38 " + addressMapper.fileAddressToBoomStreetVirtualAddress(0xf9f38));
            Debug.WriteLine("0x471660 " + addressMapper.fileAddressToBoomStreetVirtualAddress(0x471660));
            Debug.WriteLine("0xdf414 " + addressMapper.fileAddressToBoomStreetVirtualAddress(0xdf414));
            Debug.WriteLine("0xF9E44 " + addressMapper.fileAddressToBoomStreetVirtualAddress(0xF9E44));
            Debug.WriteLine("0xDF1B4 " + addressMapper.fileAddressToBoomStreetVirtualAddress(0xDF1B4));
            Debug.WriteLine("0xF9E60 " + addressMapper.fileAddressToBoomStreetVirtualAddress(0xF9E60));
            Debug.WriteLine("0xf40c4 " + addressMapper.fileAddressToBoomStreetVirtualAddress(0xf40c4));
            Debug.WriteLine("0xf40e4 " + addressMapper.fileAddressToBoomStreetVirtualAddress(0xf40e4));
            Debug.WriteLine("0xf9f80 " + addressMapper.fileAddressToBoomStreetVirtualAddress(0xf9f80));

            Debug.WriteLine("0xf5d90 " + addressMapper.fileAddressToBoomStreetVirtualAddress(0xf5d90));
            Debug.WriteLine("0x1b31c8 " + addressMapper.fileAddressToBoomStreetVirtualAddress(0x1b31c8));
            Debug.WriteLine("0xf9d84 " + addressMapper.fileAddressToBoomStreetVirtualAddress(0xf9d84));
            Debug.WriteLine("0x1b318c " + addressMapper.fileAddressToBoomStreetVirtualAddress(0x1b318c));
            Debug.WriteLine("0xf9e00 " + addressMapper.fileAddressToBoomStreetVirtualAddress(0xf9e00));
        }
    }
}
