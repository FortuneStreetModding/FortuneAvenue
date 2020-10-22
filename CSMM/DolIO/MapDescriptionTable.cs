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
    public class MapDescriptionTable : DolIO
    {
        protected UInt32 writeTable(List<MapDescriptor> mapDescriptors)
        {
            var mapDescriptionTable = new List<UInt32>();
            foreach (var mapDescriptor in mapDescriptors)
            {
                mapDescriptionTable.Add(mapDescriptor.Desc_MSG_ID);
            }
            return allocate(mapDescriptionTable, "MapDescriptionTable");
        }
        protected override void writeAsm(EndianBinaryWriter stream, Func<uint, int> toFileAddress, List<MapDescriptor> mapDescriptors)
        {
            short tableRowCount = (short)mapDescriptors.Count;
            var mapDescriptionTableAddr = writeTable(mapDescriptors);
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair(mapDescriptionTableAddr);
            // HACK: Expand the description message ID table
            // subi r3,r3,0x15                                     -> nop
            stream.Seek(toFileAddress(0x8021214c), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // cmpwi r3,0x12                                       -> cmpwi r3,tableRowCount
            stream.Seek(toFileAddress(0x80212158), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpwi(3, tableRowCount));
            // r4 <- 0x80436bc0                                    -> r4 <- mapDescriptionTableAddr
            stream.Seek(toFileAddress(0x80212164), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(4, v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi(4, 4, v.lower16Bit));
        }
        protected override void readTable(EndianBinaryReader s, List<MapDescriptor> mapDescriptors, Func<uint, int> toFileAddress, bool isVanilla)
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
                if (mapDescriptor.ID < 18)
                {
                    mapDescriptor.Desc_MSG_ID = descMsgIdTable[j];
                    j++;
                    if (j == 18)
                        j = 0;
                }
            }
        }
        protected override UInt32 readTableAddr(EndianBinaryReader stream, Func<uint, int> toFileAddress, bool isVanilla)
        {
            stream.Seek(toFileAddress(0x80212164), SeekOrigin.Begin);
            var lis_opcode = stream.ReadUInt32();
            stream.Seek(4, SeekOrigin.Current);
            var addi_opcode = stream.ReadUInt32();
            return PowerPcAsm.make32bitValueFromPair(lis_opcode, addi_opcode);
        }
        protected override Int16 readTableRowCount(EndianBinaryReader stream, Func<uint, int> toFileAddress, bool isVanilla)
        {
            stream.Seek(toFileAddress(0x80212158), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            return PowerPcAsm.getOpcodeParameter(opcode);
        }
        protected override bool readIsVanilla(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            stream.Seek(toFileAddress(0x8021214c), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            // subi r3,r3,0x15
            return opcode == PowerPcAsm.subi(3, 3, 0x15);
        }
    }
}
