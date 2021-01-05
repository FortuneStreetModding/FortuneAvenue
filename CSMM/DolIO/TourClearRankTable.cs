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
    public class TourClearRankTable : DolIOTable
    {
        protected VAVAddr writeTable(List<MapDescriptor> mapDescriptors)
        {
            var tourClearRankTable = new List<UInt32>();
            foreach (var mapDescriptor in mapDescriptors)
            {
                tourClearRankTable.Add(mapDescriptor.TourClearRank);
            }
            return allocate(tourClearRankTable, "TourClearRankTable");
        }
        protected override void writeAsm(EndianBinaryWriter stream, AddressMapper addressMapper, List<MapDescriptor> mapDescriptors)
        {
            var tableRowCount = mapDescriptors.Count;
            var tableAddr = writeTable(mapDescriptors);
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair((UInt32)tableAddr);
            // --- IsMapClear(int mapId,int normalMode,UserIndex userIndex) ---
            /*
            // subi r31,r3,0x15                                   ->  nop
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80210434), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // cmpwi r31,0x12                                     ->  cmpwi r31,tableElementsCount
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80210440), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpwi(31, (short)(tableRowCount)));
            // subi r29,r31,0x15                                  ->  nop
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80210504), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // cmpwi r29,0x12                                     ->  cmpwi r29,tableElementsCount
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80210510), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpwi(29, (short)(tableRowCount)));
            // li r0,0x15                                         ->  nop
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80210598), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            */
            // mulli r0,r0,0x24                                   ->  mulli r0,r0,0x04
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x802105a4), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(0, 0, 0x04));
            // r3 <- 804363c8                                     ->  r3 <- tableAddr
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x802105a8), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(3, v.upper16Bit)); stream.Seek(0x4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi(3, 3, v.lower16Bit));
            // mulli r0,r29,0x24                                  ->  mulli r0,r29,0x04
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x802105c4), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(0, 29, 0x04));
            // lwz r4,0x18(r3)                                    ->  lwz r4,0x0(r3)
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x802105d8), SeekOrigin.Begin); stream.Write(PowerPcAsm.lwz(4, 0x0, 3));

            // --- GetMapClearRank(int mapId,int normalMode) ---
            /*
            // subi r31,r3,0x15                                   ->  nop
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80212074), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // cmpwi r31,0x12                                     ->  cmpwi r31,tableElementsCount
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80212080), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpwi(31, (short)(tableRowCount)));
            // li r0,0x15                                         ->  nop
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80212100), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            */
            // mulli r4,r0,0x24                                   ->  mulli r4,r0,0x04
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8021210c), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(4, 0, 0x04));
            // r3 <- 804363c8                                     ->  r3 <- tableAddr
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80212110), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(3, v.upper16Bit)); stream.Write(PowerPcAsm.addi(3, 3, v.lower16Bit));
            // mulli r0,r31,0x24                                  ->  mulli r0,r31,0x04
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80212118), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(0, 31, 0x04));
            // lwz r3,0x18(r3)                                    ->  lwz r3,0x0(r3)
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8021212c), SeekOrigin.Begin); stream.Write(PowerPcAsm.lwz(3, 0x0, 3));
        }
        protected override void readAsm(EndianBinaryReader s, List<MapDescriptor> mapDescriptors, AddressMapper addressMapper, bool isVanilla)
        {
            if (isVanilla)
            {
                s.Seek(0x18, SeekOrigin.Current); // offset
            }
            foreach (var mapDescriptor in mapDescriptors)
            {
                mapDescriptor.TourClearRank = s.ReadUInt32();
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
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80212110), SeekOrigin.Begin);
            var lis_opcode = stream.ReadUInt32();
            var addi_opcode = stream.ReadUInt32();
            return (VAVAddr)PowerPcAsm.make32bitValueFromPair(lis_opcode, addi_opcode);
        }
        protected override Int16 readTableRowCount(EndianBinaryReader stream, AddressMapper addressMapper, bool isVanilla)
        {
            UInt32 opcode;
            // stream.Seek(addressMapper.toFileAddress((BSVAddr)0x), SeekOrigin.Begin); opcode = stream.ReadUInt32();
            // return (Int16)(PowerPcAsm.getOpcodeParameter(opcode));
            return -1;
        }
        protected override bool readIsVanilla(EndianBinaryReader stream, AddressMapper addressMapper)
        {
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8021210c), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            // mulli r4,r0,0x24
            return opcode == PowerPcAsm.mulli(4, 0, 0x24);
        }
    }
}
