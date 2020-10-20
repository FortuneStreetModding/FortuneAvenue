using MiscUtil.Conversion;
using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomStreetManager
{
    public class DefaultGoalMoneyTable : DolIO
    {
        protected override void writeTableRefs(EndianBinaryWriter stream, Func<uint, int> toFileAddress, Int16 tableRowCount, UInt32 tableAddr, UInt32 dataAddr, UInt32 subroutineAddr)
        {
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair(tableAddr);

            // subi r30,r30,0x15                                  ->  nop
            stream.Seek(toFileAddress(0x8020d0dc), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // cmpwi r30,0x12                                     ->  cmpwi r30,tableElementsCount
            stream.Seek(toFileAddress(0x8020d0e8), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpwi(30, (short)(tableRowCount)));
            // li r0,0x15                                         ->  nop
            stream.Seek(toFileAddress(0x8020d160), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // mulli r0,r0,0x24                                   ->  mulli r0,r0,0x04
            stream.Seek(toFileAddress(0x8020d16c), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(0, 0, 4));
            // r4 <- 804363c8                                     ->  r4 <- tableAddr
            stream.Seek(toFileAddress(0x8020d170), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(4, v.upper16Bit)); stream.Write(PowerPcAsm.addi(4, 4, v.lower16Bit));
            // mulli r3,r30,0x24                                  ->  mulli r3,r30,0x04
            stream.Seek(toFileAddress(0x8020d178), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(3, 30, 4));

            // subi r31,r3,0x15                                   ->  nop
            stream.Seek(toFileAddress(0x80211c04), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // cmpwi r31,0x12                                     ->  cmpwi r31,tableElementsCount
            stream.Seek(toFileAddress(0x80211c10), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpwi(31, (short)(tableRowCount)));
            // li r3,0x15                                         ->  nop
            stream.Seek(toFileAddress(0x80211c88), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // mulli r0,r3,0x24                                   ->  mulli r0,r3,0x04
            stream.Seek(toFileAddress(0x80211c94), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(0, 3, 4));
            // r4 <- 804363c8                                     ->  r4 <- tableAddr
            stream.Seek(toFileAddress(0x80211c98), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(4, v.upper16Bit)); stream.Write(PowerPcAsm.addi(4, 4, v.lower16Bit));
            // mulli r3,r31,0x24                                  ->  mulli r3,r31,0x04
            stream.Seek(toFileAddress(0x80211ca0), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(3, 31, 4));
        }

        protected override string writeTable(EndianBinaryWriter s, List<MapDescriptor> mapDescriptors)
        {
            foreach (var mapDescriptor in mapDescriptors)
            {
                s.Write(mapDescriptor.TargetAmount);
            }
            return "DefaultGoalMoneyTable";
        }
        protected override void readTable(EndianBinaryReader s, List<MapDescriptor> mapDescriptors, bool isVanilla)
        {
            int tableSize;
            if (isVanilla)
                tableSize = 0x24;
            else
                tableSize = 0x04;
            foreach (var mapDescriptor in mapDescriptors)
            {
                var pos = s.BaseStream.Position;
                mapDescriptor.TargetAmount = s.ReadUInt32();
                // go to the next target amount in the table
                s.Seek((int)(tableSize + pos - s.BaseStream.Position), SeekOrigin.Current);
            }
        }
        protected override UInt32 readTableAddr(EndianBinaryReader stream, Func<uint, int> toFileAddress, bool isVanilla)
        {
            stream.Seek(toFileAddress(0x8020d170), SeekOrigin.Begin);
            var lis_opcode = stream.ReadUInt32();
            var addi_opcode = stream.ReadUInt32();
            return PowerPcAsm.make32bitValueFromPair(lis_opcode, addi_opcode);
        }
        protected override Int16 readTableRowCount(EndianBinaryReader stream, Func<uint, int> toFileAddress, bool isVanilla)
        {
            UInt32 opcode;
            stream.Seek(toFileAddress(0x8020d0e8), SeekOrigin.Begin); opcode = stream.ReadUInt32();
            return (Int16)(PowerPcAsm.getOpcodeParameter(opcode));
        }
        protected override bool readIsVanilla(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            stream.Seek(toFileAddress(0x8020d0dc), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            return opcode != PowerPcAsm.nop();
        }
    }
}
