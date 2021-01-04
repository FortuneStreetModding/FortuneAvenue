using MiscUtil.Conversion;
using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CustomStreetManager
{
    public class ForceSimulatedButtonPress : DolIO
    {
        protected override void writeAsm(EndianBinaryWriter stream, AddressMapper addressMapper, List<MapDescriptor> mapDescriptors)
        {
            var hijackAddr = addressMapper.toVersionAgnosticAddress((BSVAddr)0x802bb120);
            var returnAddr = addressMapper.toVersionAgnosticAddress((BSVAddr)0x802bb124);

            var uploadSimulatedButtonPress = allocate(writeUploadSimulatedButtonPress(addressMapper, (VAVAddr)0, returnAddr), "UploadSimulatedButtonPress");
            stream.Seek(addressMapper.toFileAddress(uploadSimulatedButtonPress), SeekOrigin.Begin);
            stream.Write(writeUploadSimulatedButtonPress(addressMapper, uploadSimulatedButtonPress, returnAddr)); // re-write the routine again since now we know where it is located in the main dol
            // lwz r0,0x4(r3)                                                             -> b uploadSimulatedButtonPress
            stream.Seek(addressMapper.toFileAddress(hijackAddr), SeekOrigin.Begin); stream.Write(PowerPcAsm.b(hijackAddr, uploadSimulatedButtonPress));
        }


        private List<UInt32> writeUploadSimulatedButtonPress(AddressMapper addressMapper, VAVAddr routineStartAddress, VAVAddr returnAddr)
        {
            var asm = new List<UInt32>();
            // 0x804363b4 (4 bytes): force simulated button press
            var forceSimulatedButtonPress = PowerPcAsm.make16bitValuePair((UInt32)addressMapper.toVersionAgnosticAddress((BSVAddr)0x804363b4));
            var pressedButtonsBitArray = PowerPcAsm.make16bitValuePair((UInt32)addressMapper.toVersionAgnosticAddress((BSVAddr)0x8078C880));

            asm.Add(PowerPcAsm.lis(6, forceSimulatedButtonPress.upper16Bit));                // \
            asm.Add(PowerPcAsm.addi(6, 6, forceSimulatedButtonPress.lower16Bit));            // / r6 <- &forceSimulatedButtonPress
            asm.Add(PowerPcAsm.lis(7, pressedButtonsBitArray.upper16Bit));                   // \
            asm.Add(PowerPcAsm.addi(7, 7, pressedButtonsBitArray.lower16Bit));               // / r7 <- &pressedButtonsBitArray
            asm.Add(PowerPcAsm.lwz(0, 0x0, 6));                                              // r0 <- forceSimulatedButtonPress
            asm.Add(PowerPcAsm.cmpwi(0, 0x0));                                               // if (forceSimulatedButtonPress != 0)
            asm.Add(PowerPcAsm.beq(4));                                                      // {
            asm.Add(PowerPcAsm.stw(0, 0x0, 7));                                              //   pressedButtonsBitArray <- forceSimulatedButtonPress
            asm.Add(PowerPcAsm.li(0, 0x0));                                                  //   \ 
            asm.Add(PowerPcAsm.stw(0, 0x0, 6));                                              //   / forceSimulatedButtonPress <- 0
                                                                                             // }
            asm.Add(PowerPcAsm.lwz(0, 0x4, 3));                                              // *replaced opcode*
            asm.Add(PowerPcAsm.b(routineStartAddress, asm.Count, returnAddr));               // return

            return asm;
        }

        protected override void readAsm(EndianBinaryReader stream, List<MapDescriptor> mapDescriptors, AddressMapper addressMapper)
        {
        }
    }
}
