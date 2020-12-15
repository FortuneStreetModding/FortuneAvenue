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
    public class BGSequenceTable : DolIOTable
    {
        protected VAVAddr writeTable(List<MapDescriptor> mapDescriptors, VAVAddr bgSequenceMarioStadium)
        {
            var bgSequenceTable = new List<VAVAddr>();
            foreach (var mapDescriptor in mapDescriptors)
            {
                // the BGSequence is only used for mario stadium to animate the Miis playing baseball in the background. As such this will be hardcoded whenever bg004 is selected.
                bgSequenceTable.Add(mapDescriptor.Background == "bg004" ? bgSequenceMarioStadium : VAVAddr.NullAddress);
            }
            return allocate(bgSequenceTable, "BGSequenceTable");
        }
        protected override void writeAsm(EndianBinaryWriter stream, AddressMapper addressMapper, List<MapDescriptor> mapDescriptors)
        {
            // hardcoded virtual address for the parameter table on how the Miis are being animated to play baseball in the background
            var bgSequenceMarioStadium = addressMapper.toVersionAgnosticAddress((BSVAddr)0x80428968);
            var tableAddr = writeTable(mapDescriptors, bgSequenceMarioStadium);
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair((UInt32)tableAddr);

            // --- Update Table Addr ---
            // mulli r0,r3,0x38 ->  mulli r0,r3,0x04
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccb70), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(0, 3, 0x04));
            // r3 <- 0x80428e50 ->  r3 <- tableAddr
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccb74), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(3, v.upper16Bit)); stream.Write(PowerPcAsm.addi(3, 3, v.lower16Bit));
            // lwz r3,0x34(r3)   ->  lwz r3,0x0(r3)
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccb80), SeekOrigin.Begin); stream.Write(PowerPcAsm.lwz(3, 0x0, 3));
        }

        protected override void readAsm(EndianBinaryReader s, List<MapDescriptor> mapDescriptors, AddressMapper addressMapper, bool isVanilla)
        {
            if (isVanilla) {
                s.Seek(0x4, SeekOrigin.Current); // offset
            }
            foreach (var mapDescriptor in mapDescriptors)
            {
                // the BGSequence is not supported to be viewed or editted. So we discard it.
                _ = s.ReadUInt32();
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
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccb74), SeekOrigin.Begin);
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
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccb70), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            // mulli r0,r3,0x38
            return opcode == PowerPcAsm.mulli(0, 3, 0x38);
        }
    }
}
