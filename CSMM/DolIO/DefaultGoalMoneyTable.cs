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
    public class DefaultGoalMoneyTable : DolIOTable
    {
        protected VAVAddr writeTable(List<MapDescriptor> mapDescriptors)
        {
            var targetAmountTable = new List<UInt32>();
            foreach (var mapDescriptor in mapDescriptors)
            {
                targetAmountTable.Add(mapDescriptor.TargetAmount);
            }
            return allocate(targetAmountTable, "DefaultGoalMoneyTable");
        }
        protected override void writeAsm(EndianBinaryWriter stream, AddressMapper addressMapper, List<MapDescriptor> mapDescriptors)
        {
            var tableRowCount = mapDescriptors.Count;
            var tableAddr = writeTable(mapDescriptors);
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair((UInt32)tableAddr);

            // subi r30,r30,0x15                                  ->  nop
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020d0dc), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // cmpwi r30,0x12                                     ->  cmpwi r30,tableElementsCount
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020d0e8), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpwi(30, (short)(tableRowCount)));
            // li r0,0x15                                         ->  nop
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020d160), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // mulli r0,r0,0x24                                   ->  mulli r0,r0,0x04
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020d16c), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(0, 0, 0x04));
            // r4 <- 804363c8                                     ->  r4 <- tableAddr
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020d170), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(4, v.upper16Bit)); stream.Write(PowerPcAsm.addi(4, 4, v.lower16Bit));
            // mulli r3,r30,0x24                                  ->  mulli r3,r30,0x04
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020d178), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(3, 30, 0x04));

            // subi r31,r3,0x15                                   ->  nop
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211c04), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // cmpwi r31,0x12                                     ->  cmpwi r31,tableElementsCount
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211c10), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpwi(31, (short)(tableRowCount)));
            // li r3,0x15                                         ->  nop
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211c88), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // mulli r0,r3,0x24                                   ->  mulli r0,r3,0x04
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211c94), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(0, 3, 0x04));
            // r4 <- 804363c8                                     ->  r4 <- tableAddr
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211c98), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(4, v.upper16Bit)); stream.Write(PowerPcAsm.addi(4, 4, v.lower16Bit));
            // mulli r3,r31,0x24                                  ->  mulli r3,r31,0x04
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211ca0), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(3, 31, 0x04));
        }
        protected override void readAsm(EndianBinaryReader s, List<MapDescriptor> mapDescriptors, AddressMapper addressMapper, bool isVanilla)
        {
            foreach (var mapDescriptor in mapDescriptors)
            {
                mapDescriptor.TargetAmount = s.ReadUInt32();
                if (isVanilla)
                {
                    // in vanilla main.dol the table has other stuff in it like initial cash and tour opponents of the map
                    // this we need to skip to go the next target amount in the table
                    s.Seek(0x20, SeekOrigin.Current);
                }
            }
        }
        protected override VAVAddr readTableAddr(EndianBinaryReader stream, AddressMapper addressMapper, bool isVanilla)
        {
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020d170), SeekOrigin.Begin);
            var lis_opcode = stream.ReadUInt32();
            var addi_opcode = stream.ReadUInt32();
            return (VAVAddr)PowerPcAsm.make32bitValueFromPair(lis_opcode, addi_opcode);
        }
        protected override Int16 readTableRowCount(EndianBinaryReader stream, AddressMapper addressMapper, bool isVanilla)
        {
            UInt32 opcode;
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020d0e8), SeekOrigin.Begin); opcode = stream.ReadUInt32();
            return (Int16)(PowerPcAsm.getOpcodeParameter(opcode));
        }
        protected override bool readIsVanilla(EndianBinaryReader stream, AddressMapper addressMapper)
        {
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020d160), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            // li r0,0x15
            return opcode == PowerPcAsm.li(0, 0x15);
        }
    }
}
