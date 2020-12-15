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
    public class MapOriginTable : DolIOTable
    {
        protected VAVAddr writeTable(List<MapDescriptor> mapDescriptors)
        {
            var mapOriginTable = new List<UInt32>();
            foreach (var mapDescriptor in mapDescriptors)
            {
                mapOriginTable.Add((UInt32)mapDescriptor.ID);
            }
            return allocate(mapOriginTable, "MapOriginTable");
        }
        protected override void writeAsm(EndianBinaryWriter stream, AddressMapper addressMapper, List<MapDescriptor> mapDescriptors)
        {
            var tableAddr = writeTable(mapDescriptors);
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair((UInt32)tableAddr);

            // --- Update Table Addr ---
            // mulli r0,r3,0x38 ->  mulli r0,r3,0x04
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccb58), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(0, 3, 0x04));
            // r3 <- 0x80428e50 ->  r3 <- tableAddr
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccb5c), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(3, v.upper16Bit)); stream.Write(PowerPcAsm.addi(3, 3, v.lower16Bit));
            // lwz r3,0x30(r3)  ->  lwz r3,0x0(r3)
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccb68), SeekOrigin.Begin); stream.Write(PowerPcAsm.lwz(3, 0x0, 3));
        }

        protected override void readAsm(EndianBinaryReader s, List<MapDescriptor> mapDescriptors, AddressMapper addressMapper, bool isVanilla)
        {
            if (isVanilla) {
                s.Seek(0x30, SeekOrigin.Current); // offset
            }
            foreach (var mapDescriptor in mapDescriptors)
            {
                mapDescriptor.ID = s.ReadUInt32();
                if (isVanilla)
                {
                    // in vanilla main.dol the table has other stuff in it like bgm id, map frb files, etc.
                    // this we need to skip to go the next target amount in the table
                    s.Seek(0x38 - 0x04, SeekOrigin.Current);
                }
            }
        }
        protected override VAVAddr readTableAddr(EndianBinaryReader stream, AddressMapper addressMapper, bool isVanilla)
        {
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccb5c), SeekOrigin.Begin);
            var lis_opcode = stream.ReadUInt32();
            var addi_opcode = stream.ReadUInt32();
            return (VAVAddr)PowerPcAsm.make32bitValueFromPair(lis_opcode, addi_opcode);
        }
        protected override Int16 readTableRowCount(EndianBinaryReader stream, AddressMapper addressMapper, bool isVanilla)
        {
            return -1;
        }
        protected override bool readIsVanilla(EndianBinaryReader stream, AddressMapper addressMapper)
        {
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccb58), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            // mulli r0,r3,0x38
            return opcode == PowerPcAsm.mulli(0, 3, 0x38);
        }
    }
}
