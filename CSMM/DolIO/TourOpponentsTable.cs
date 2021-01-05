using MiscUtil.Conversion;
using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomStreetMapManager
{
    public class TourOpponentsTable : DolIOTable
    {
        protected VAVAddr writeTable(List<MapDescriptor> mapDescriptors)
        {
            var tourOpponentsTable = new List<UInt32>();
            foreach (var mapDescriptor in mapDescriptors)
            {
                tourOpponentsTable.Add((UInt32)mapDescriptor.TourOpponent1);
                tourOpponentsTable.Add((UInt32)mapDescriptor.TourOpponent2);
                tourOpponentsTable.Add((UInt32)mapDescriptor.TourOpponent3);
            }
            return allocate(tourOpponentsTable, "TourOpponentsTable");
        }
        protected override void writeAsm(EndianBinaryWriter stream, AddressMapper addressMapper, List<MapDescriptor> mapDescriptors)
        {
            var tableRowCount = mapDescriptors.Count;
            var tableAddr = writeTable(mapDescriptors);
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair((UInt32)tableAddr);

            // subi r3,r3,0x15                                    ->  nop
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020cec8), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // cmpwi r3,0x12                                      ->  cmpwi r3,tableElementsCount
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020ced4), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpwi(3, (short)(tableRowCount)));
            // li r0,0x15                                         ->  nop
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020cf68), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // mulli r0,r3,0x24                                   ->  mulli r0,r3,0x0c
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020cee0), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(0, 3, 0x0c));
            // r3 <- 804363c8                                     ->  r3 <- tableAddr
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020cee4), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(3, v.upper16Bit)); stream.Seek(0x4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi(3, 3, v.lower16Bit));
            // mulli r3,r0,0x24                                  ->  mulli r3,r0,0x0c
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020cf74), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(3, 0, 0x0c));
            // lwz r0,0xc(r3)                                     ->  lwz r0,0x0(r3)
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020cf8c), SeekOrigin.Begin); stream.Write(PowerPcAsm.lwz(0, 0x0, 3));

            // subi r4,r4,0x15                                   ->  nop
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211af4), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // cmpwi r4,0x12                                     ->  cmpwi r4,tableElementsCount
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211b00), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpwi(4, (short)(tableRowCount)));
            // li r3,0x15                                         ->  nop
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211b94), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // mulli r0,r4,0x24                                   ->  mulli r0,r4,0x0c
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211b0c), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(0, 4, 0x0c));
            // r4 <- 804363c8                                     ->  r4 <- tableAddr
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211b10), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(4, v.upper16Bit)); stream.Seek(0x4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi(4, 4, v.lower16Bit));
            // mulli r3,r3,0x24                                  ->  mulli r3,r3,0x0c
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211ba0), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(3, 3, 0x0c));
            // lwz r0,0xc(r3)                                     ->  lwz r0,0x0(r3)
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211bb8), SeekOrigin.Begin); stream.Write(PowerPcAsm.lwz(0, 0x0, 3));
        }
        protected override void readAsm(EndianBinaryReader s, List<MapDescriptor> mapDescriptors, AddressMapper addressMapper, bool isVanilla)
        {
            if (isVanilla)
            {
                s.Seek(0xc, SeekOrigin.Current); // offset
            }
            foreach (var mapDescriptor in mapDescriptors)
            {
                mapDescriptor.TourOpponent1 = (Character)s.ReadUInt32();
                mapDescriptor.TourOpponent2 = (Character)s.ReadUInt32();
                mapDescriptor.TourOpponent3 = (Character)s.ReadUInt32();
                if (isVanilla)
                {
                    // in vanilla main.dol the table has other stuff in it like initial cash and tour opponents of the map
                    // this we need to skip to go the next target amount in the table
                    s.Seek(0x24 - 0xc, SeekOrigin.Current);
                }
            }
        }
        protected override VAVAddr readTableAddr(EndianBinaryReader stream, AddressMapper addressMapper, bool isVanilla)
        {
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211b10), SeekOrigin.Begin);
            var lis_opcode = stream.ReadUInt32();
            stream.Seek(0x4, SeekOrigin.Current);
            var addi_opcode = stream.ReadUInt32();
            return (VAVAddr)PowerPcAsm.make32bitValueFromPair(lis_opcode, addi_opcode);
        }
        protected override Int16 readTableRowCount(EndianBinaryReader stream, AddressMapper addressMapper, bool isVanilla)
        {
            UInt32 opcode;
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211b00), SeekOrigin.Begin); opcode = stream.ReadUInt32();
            return (Int16)(PowerPcAsm.getOpcodeParameter(opcode));
        }
        protected override bool readIsVanilla(EndianBinaryReader stream, AddressMapper addressMapper)
        {
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020cf68), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            // li r0,0x15
            return opcode == PowerPcAsm.li(0, 0x15);
        }
    }
}
