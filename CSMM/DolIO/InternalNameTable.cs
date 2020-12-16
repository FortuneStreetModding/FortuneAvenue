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
    public class InternalNameTable : DolIOTable
    {
        protected VAVAddr writeTable(List<MapDescriptor> mapDescriptors)
        {
            var internalNameTable = new List<VAVAddr>();
            foreach (var mapDescriptor in mapDescriptors)
            {
                internalNameTable.Add(allocate(mapDescriptor.InternalName));
            }
            return allocate(internalNameTable, "InternalNameTable");
        }
        protected override void writeAsm(EndianBinaryWriter stream, AddressMapper addressMapper, List<MapDescriptor> mapDescriptors)
        {
            var tableAddr = writeTable(mapDescriptors);
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair((UInt32)tableAddr);

            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x804363b8), SeekOrigin.Begin); stream.Write(Encoding.ASCII.GetBytes("NAME"));
            // Store the pointer to the table to some unused address. The game does not use the internal name, but CSMM uses it for the name of the map descriptor file.
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x804363bc), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(5, v.upper16Bit)); stream.Write(PowerPcAsm.addi(5, 5, v.lower16Bit));
        }

        protected override void readAsm(EndianBinaryReader s, List<MapDescriptor> mapDescriptors, AddressMapper addressMapper, bool isVanilla)
        {
            if (isVanilla)
            {
                s.Seek(0x08, SeekOrigin.Current); // offset
            }
            foreach (var mapDescriptor in mapDescriptors)
            {
                var internalName = resolveAddressToString((VAVAddr)s.ReadUInt32(), s, addressMapper).Trim();
                // clear the internal name of characters which are not allowed in a file system
                mapDescriptor.InternalName = Regex.Replace(internalName, @"[<>:/\|?*""]+", "");
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
            BSVAddr addr = isVanilla ? (BSVAddr)0x801cca70 : (BSVAddr)0x804363bc;
            stream.Seek(addressMapper.toFileAddress(addr), SeekOrigin.Begin);
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
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x804363b8), SeekOrigin.Begin);
            var bytes = stream.ReadBytes(4);
            return !new ByteArrayComparer().Equals(bytes, Encoding.ASCII.GetBytes("NAME"));
        }
    }
}
