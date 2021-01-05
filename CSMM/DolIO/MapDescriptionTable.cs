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
    public class MapDescriptionTable : DolIOTable
    {
        protected VAVAddr writeTable(List<MapDescriptor> mapDescriptors)
        {
            var mapDescriptionTable = new List<UInt32>();
            foreach (var mapDescriptor in mapDescriptors)
            {
                mapDescriptionTable.Add(mapDescriptor.Desc_MSG_ID);
            }
            return allocate(mapDescriptionTable, "MapDescriptionTable");
        }
        protected override void writeAsm(EndianBinaryWriter stream, AddressMapper addressMapper, List<MapDescriptor> mapDescriptors)
        {
            short tableRowCount = (short)mapDescriptors.Count;
            var mapDescriptionTableAddr = writeTable(mapDescriptors);
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair((UInt32)mapDescriptionTableAddr);
            // HACK: Expand the description message ID table
            // subi r3,r3,0x15                                     -> nop
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8021214c), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // cmpwi r3,0x12                                       -> cmpwi r3,tableRowCount
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80212158), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpwi(3, tableRowCount));
            // r4 <- 0x80436bc0                                    -> r4 <- mapDescriptionTableAddr
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80212164), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(4, v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi(4, 4, v.lower16Bit));
        }
        protected override void readAsm(EndianBinaryReader s, List<MapDescriptor> mapDescriptors, AddressMapper addressMapper, bool isVanilla)
        {
            if (isVanilla)
                readVanillaTable(s, mapDescriptors);
            else
                foreach (MapDescriptor mapDescriptor in mapDescriptors)
                {
                    mapDescriptor.Desc_MSG_ID = s.ReadUInt32();
                }
        }
        protected void readVanillaTable(EndianBinaryReader s, List<MapDescriptor> mapDescriptors)
        {
            UInt32[] descMsgIdTable = new UInt32[18];
            for (int i = 0; i < descMsgIdTable.Length; i++)
            {
                descMsgIdTable[i] = s.ReadUInt32();
            }
            var j = 0;
            foreach (MapDescriptor mapDescriptor in mapDescriptors)
            {
                if (mapDescriptor.UnlockID < 18)
                {
                    mapDescriptor.Desc_MSG_ID = descMsgIdTable[j];
                    j++;
                    if (j == 18)
                        j = 0;
                }
            }
        }
        protected override VAVAddr readTableAddr(EndianBinaryReader stream, AddressMapper addressMapper, bool isVanilla)
        {
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80212164), SeekOrigin.Begin);
            var lis_opcode = stream.ReadUInt32();
            stream.Seek(4, SeekOrigin.Current);
            var addi_opcode = stream.ReadUInt32();
            return (VAVAddr)PowerPcAsm.make32bitValueFromPair(lis_opcode, addi_opcode);
        }
        protected override Int16 readTableRowCount(EndianBinaryReader stream, AddressMapper addressMapper, bool isVanilla)
        {
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80212158), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            return PowerPcAsm.getOpcodeParameter(opcode);
        }
        protected override bool readIsVanilla(EndianBinaryReader stream, AddressMapper addressMapper)
        {
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8021214c), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            // subi r3,r3,0x15
            return opcode == PowerPcAsm.subi(3, 3, 0x15);
        }
    }
}
