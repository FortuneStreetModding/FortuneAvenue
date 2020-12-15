using MiscUtil.Conversion;
using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomStreetManager
{
    public class FrbMapTable : DolIOTable
    {
        protected VAVAddr writeTable(List<MapDescriptor> mapDescriptors)
        {
            var ruleSetTable = new List<VAVAddr>();
            foreach (var mapDescriptor in mapDescriptors)
            {
                ruleSetTable.Add(allocate(mapDescriptor.FrbFile1));
                ruleSetTable.Add(allocate(mapDescriptor.FrbFile2));
                ruleSetTable.Add(allocate(mapDescriptor.FrbFile3));
                ruleSetTable.Add(allocate(mapDescriptor.FrbFile4));
            }
            return allocate(ruleSetTable, "FrbMapTable");
        }
        protected override void writeAsm(EndianBinaryWriter stream, AddressMapper addressMapper, List<MapDescriptor> mapDescriptors)
        {
            var tableAddr = writeTable(mapDescriptors);
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair((UInt32)tableAddr);

            // --- Game::GetMapFrbName ---
            // mulli r3,r3,0x38  ->  mulli r3,r3,0x10
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccab0), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(3, 3, 0x10));
            // r5 <- 0x80428e50  ->  r5 <- tableAddr
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccab4), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(5, v.upper16Bit)); stream.Seek(0x4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi(5, 5, v.lower16Bit));
            // lwz r3,0x18(r3)   ->  lwz r3,0x0(r3)
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccac8), SeekOrigin.Begin); stream.Write(PowerPcAsm.lwz(3, 0x0, 3));

            // --- Game::GetMapMapNum ---
            // mulli r0,r3,0x38  ->  mulli r0,r3,0x10
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccad0), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(0, 3, 0x10));
            // r4 <- 0x80428e50  ->  r4 <- tableAddr
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccad4), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(4, v.upper16Bit)); stream.Seek(0x4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi(4, 4, v.lower16Bit));
            // lwz r0,0x18(r4)   ->  lwz r0,0x0(r4)
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccae4), SeekOrigin.Begin); stream.Write(PowerPcAsm.lwz(0, 0x0, 4));
            // lwz r0,0x1c(r4)   ->  lwz r0,0x4(r4)
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccaf0), SeekOrigin.Begin); stream.Write(PowerPcAsm.lwz(0, 0x4, 4));
            // lwz r0,0x20(r4)   ->  lwz r0,0x8(r4)
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccb00), SeekOrigin.Begin); stream.Write(PowerPcAsm.lwz(0, 0x8, 4));
            // lwz r0,0x24(r4)   ->  lwz r0,0xc(r4)
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccb10), SeekOrigin.Begin); stream.Write(PowerPcAsm.lwz(0, 0xc, 4));

        }

        protected override void readAsm(EndianBinaryReader s, List<MapDescriptor> mapDescriptors, AddressMapper addressMapper, bool isVanilla)
        {
            if (isVanilla)
            {
                s.Seek(0x18, SeekOrigin.Current); // offset
            }
            foreach (var mapDescriptor in mapDescriptors)
            {
                mapDescriptor.FrbFile1 = resolveAddressToString((VAVAddr)s.ReadUInt32(), s, addressMapper);
                mapDescriptor.FrbFile2 = resolveAddressToString((VAVAddr)s.ReadUInt32(), s, addressMapper);
                mapDescriptor.FrbFile3 = resolveAddressToString((VAVAddr)s.ReadUInt32(), s, addressMapper);
                mapDescriptor.FrbFile4 = resolveAddressToString((VAVAddr)s.ReadUInt32(), s, addressMapper);
                if (isVanilla)
                {
                    // in vanilla main.dol the table has other stuff in it like bgm id, map frb files, etc.
                    // this we need to skip to go the next target amount in the table
                    s.Seek(0x38 - 0x10, SeekOrigin.Current);
                }
            }
        }
        protected override VAVAddr readTableAddr(EndianBinaryReader stream, AddressMapper addressMapper, bool isVanilla)
        {
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccab4), SeekOrigin.Begin);
            var lis_opcode = stream.ReadUInt32();
            _ = stream.ReadUInt32();
            var addi_opcode = stream.ReadUInt32();
            return (VAVAddr)PowerPcAsm.make32bitValueFromPair(lis_opcode, addi_opcode);
        }
        protected override Int16 readTableRowCount(EndianBinaryReader stream, AddressMapper addressMapper, bool isVanilla)
        {
            return -1;
        }
        protected override bool readIsVanilla(EndianBinaryReader stream, AddressMapper addressMapper)
        {
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccad0), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            // mulli r0,r3,0x38
            return opcode == PowerPcAsm.mulli(0, 3, 0x38);
        }
    }
}
