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
    public class VentureCardSwitchSquare : DolIO
    {
        protected override void writeAsm(EndianBinaryWriter stream, AddressMapper addressMapper, List<MapDescriptor> mapDescriptors)
        {
            var fakeVentureCard = allocate(new byte[4], "FakeVentureCard");
            var setupCustomVentureCardRoutine = allocate(writeSubroutineProcSquare0x2E(addressMapper, fakeVentureCard, (VAVAddr)0), "SetupCustomVentureCardSubroutine");
            stream.Seek(addressMapper.toFileAddress(setupCustomVentureCardRoutine), SeekOrigin.Begin);
            stream.Write(writeSubroutineProcSquare0x2E(addressMapper, fakeVentureCard, setupCustomVentureCardRoutine)); // re-write the routine again since now we know where it is located in the main dol

            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80475838), SeekOrigin.Begin);
            stream.Write((UInt32)setupCustomVentureCardRoutine);

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
            stream.Write(PowerPcAsm.bl((uint)virtualPos, (uint)customTextureHandler));

            customTextureHandler = allocate(writeGetTextureForCustomSquareRoutine(31, 30), "GetTextureForCustomSquareRoutine2");
            virtualPos = addressMapper.toVersionAgnosticAddress((BSVAddr)0x80087a24);
            stream.Seek(addressMapper.toFileAddress(virtualPos), SeekOrigin.Begin);
            // li r31,0x1        -> bl customTextureHandler
            stream.Write(PowerPcAsm.bl((uint)virtualPos, (uint)customTextureHandler));

            // --- Name ---
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80475580), SeekOrigin.Begin);
            stream.Write(3336); // id of the message in ui_message.csv (3336 = "Switch square")

            // --- Icon ---
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x804160c8), SeekOrigin.Begin);
            stream.Write((uint)addressMapper.toVersionAgnosticAddress((BSVAddr)0x80415ee0)); // pointer to the texture name (0x80415ee0 points to the texture "p_mark_21" which is the switch square icon
            stream.Write((uint)addressMapper.toVersionAgnosticAddress((BSVAddr)0x80415ee0)); // we need to write it twice: once for each design type (Mario and DragonQuest)

        }

        private List<UInt32> writeGetTextureForCustomSquareRoutine(byte register_textureType, byte register_squareType)
        {
            var asm = new List<UInt32>();
            asm.Add(PowerPcAsm.li(register_textureType, 0x1));    // textureType = 1
            asm.Add(PowerPcAsm.cmpwi(register_squareType, 0x2e)); // if(squareType == 0x2e)
            asm.Add(PowerPcAsm.beq(asm.Count, asm.Count+2));      // {
            asm.Add(PowerPcAsm.blr());                            //   return textureType; 
                                                                  // } else {
            asm.Add(PowerPcAsm.li(register_textureType, 0x5));    //   textureType = 5
            asm.Add(PowerPcAsm.blr());                            //   return textureType;
                                                                  // }
            return asm;
        }

        private List<UInt32> writeSubroutineProcSquare0x2E(AddressMapper addressMapper, VAVAddr fakeVentureCard, VAVAddr routineStartAddress)
        {
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair((UInt32)fakeVentureCard);
            var gameProgressChangeModeRoutine = addressMapper.toVersionAgnosticAddress((BSVAddr)0x800c093c);

            var asm = new List<UInt32>();
            asm.Add(PowerPcAsm.lwz(3, 0x188, 28));         // \
            asm.Add(PowerPcAsm.lwz(3, 0x74, 3));           // / r3_place = gameChara.currentPlace
            asm.Add(PowerPcAsm.lbz(6, 0x18, 3));           // | r6_ventureCardId = r3_place.districtId

            asm.Add(PowerPcAsm.lis(3, v.upper16Bit));      // \
            asm.Add(PowerPcAsm.addi(3, 3, v.lower16Bit));  // | fakeVentureCard <- r6_ventureCardId
            asm.Add(PowerPcAsm.stw(6, 0x0, 3));            // / 

            asm.Add(PowerPcAsm.li(4, 0x1d));               // \ li r4,0x1d
            asm.Add(PowerPcAsm.li(5, -0x1));               // | li r5,-0x1
            asm.Add(PowerPcAsm.li(6, -0x1));               // | li r6,-0x1
            asm.Add(PowerPcAsm.li(7, 0x0));                // | li r7,0x0
            asm.Add(PowerPcAsm.bl((uint)(routineStartAddress + asm.Count), (uint)gameProgressChangeModeRoutine));        // / bl Game::GameProgress::changeMode
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
