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
    public class TourInitialCashTable : DolIOTable
    {
        protected VAVAddr writeTable(List<MapDescriptor> mapDescriptors)
        {
            var tourInitialCashTable = new List<UInt32>();
            foreach (var mapDescriptor in mapDescriptors)
            {
                tourInitialCashTable.Add(mapDescriptor.TourInitialCash);
            }
            return allocate(tourInitialCashTable, "TourInitialCashTable");
        }
        protected override void writeAsm(EndianBinaryWriter stream, AddressMapper addressMapper, List<MapDescriptor> mapDescriptors)
        {
            var tableRowCount = mapDescriptors.Count;
            var tableAddr = writeTable(mapDescriptors);
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair((UInt32)tableAddr);

            // subi r30,r30,0x15                                  ->  nop
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020d1c4), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // cmpwi r30,0x12                                     ->  cmpwi r30,tableElementsCount
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020d1d0), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpwi(30, (short)(tableRowCount)));
            // li r0,0x15                                         ->  nop
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020d248), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // mulli r4,r0,0x24                                   ->  mulli r4,r0,0x04
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020d254), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(4, 0, 0x04));
            // r3 <- 804363c8                                     ->  r3 <- tableAddr
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020d258), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(3, v.upper16Bit)); stream.Write(PowerPcAsm.addi(3, 3, v.lower16Bit));
            // mulli r0,r30,0x24                                  ->  mulli r0,r30,0x04
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020d260), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(0, 30, 0x04));
            // lwz r0,0x8(r3)                                     ->  lwz r0,0x0(r3)
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020d26c), SeekOrigin.Begin); stream.Write(PowerPcAsm.lwz(0, 0x0, 3));

            // subi r31,r3,0x15                                   ->  nop
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211ce4), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // cmpwi r31,0x12                                     ->  cmpwi r31,tableElementsCount
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211cf0), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpwi(31, (short)(tableRowCount)));
            // li r3,0x15                                         ->  nop
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211d68), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // mulli r4,r3,0x24                                   ->  mulli r4,r3,0x04
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211d74), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(4, 3, 0x04));
            // r3 <- 804363c8                                     ->  r3 <- tableAddr
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211d78), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(3, v.upper16Bit)); stream.Write(PowerPcAsm.addi(3, 3, v.lower16Bit));
            // mulli r0,r31,0x24                                  ->  mulli r0,r31,0x04
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211d80), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(0, 31, 0x04));
            // lwz r3,0x8(r3)                                     ->  lwz r3,0x0(r3)
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211d94), SeekOrigin.Begin); stream.Write(PowerPcAsm.lwz(3, 0x0, 3));
        }
        protected override void readAsm(EndianBinaryReader s, List<MapDescriptor> mapDescriptors, AddressMapper addressMapper, bool isVanilla)
        {
            if (isVanilla)
            {
                s.Seek(0x8, SeekOrigin.Current); // offset
            }
            foreach (var mapDescriptor in mapDescriptors)
            {
                mapDescriptor.TourBankruptcyLimit = s.ReadUInt32();
                if (isVanilla)
                {
                    // in vanilla main.dol the table has other stuff in it like initial cash and tour opponents of the map
                    // this we need to skip to go the next target amount in the table
                    s.Seek(0x24 - 0x4, SeekOrigin.Current);
                }
            }
        }
        protected override VAVAddr readTableAddr(EndianBinaryReader stream, AddressMapper addressMapper, bool isVanilla)
        {
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211d78), SeekOrigin.Begin);
            var lis_opcode = stream.ReadUInt32();
            var addi_opcode = stream.ReadUInt32();
            return (VAVAddr)PowerPcAsm.make32bitValueFromPair(lis_opcode, addi_opcode);
        }
        protected override Int16 readTableRowCount(EndianBinaryReader stream, AddressMapper addressMapper, bool isVanilla)
        {
            UInt32 opcode;
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211cf0), SeekOrigin.Begin); opcode = stream.ReadUInt32();
            return (Int16)(PowerPcAsm.getOpcodeParameter(opcode));
        }
        protected override bool readIsVanilla(EndianBinaryReader stream, AddressMapper addressMapper)
        {
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020d248), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            // li r0,0x15
            return opcode == PowerPcAsm.li(0, 0x15);
        }
    }
}
