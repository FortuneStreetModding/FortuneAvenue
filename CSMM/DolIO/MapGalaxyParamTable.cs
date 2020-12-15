using FSEditor.FSData;
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
    public class MapGalaxyParamTable : DolIOTable
    {
        protected VAVAddr writeTable(List<MapDescriptor> mapDescriptors)
        {
            var mapGalaxyParamTable = new List<VAVAddr>();
            foreach (var mapDescriptor in mapDescriptors)
            {
                if (mapDescriptor.LoopingMode == LoopingMode.None)
                {
                    mapGalaxyParamTable.Add(VAVAddr.NullAddress);
                }
                else
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                        s.Write(mapDescriptor.LoopingModeRadius);
                        s.Write(mapDescriptor.LoopingModeHorizontalPadding);
                        s.Write(mapDescriptor.LoopingModeVerticalSquareCount);
                        var loopingModeConfigAddr = allocate(memoryStream.ToArray(), "LoopingModeConfig for " + mapDescriptor.InternalName);
                        mapGalaxyParamTable.Add(loopingModeConfigAddr);
                    }
                }
            }
            return allocate(mapGalaxyParamTable, "MapGalaxyParamTable");
        }
        protected override void writeAsm(EndianBinaryWriter stream, AddressMapper addressMapper, List<MapDescriptor> mapDescriptors)
        {
            var tableAddr = writeTable(mapDescriptors);
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair((UInt32)tableAddr);

            // --- Game::GetMapGalaxyParam ---
            // mulli r0,r3,0x38  ->  mulli r0,r3,0x4
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccb40), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(0, 3, 0x4));
            // r3 <- 0x80428e50  ->  r3 <- tableAddr
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccb44), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(3, v.upper16Bit)); stream.Write(PowerPcAsm.addi(3, 3, v.lower16Bit));
            // lwz r3,0x2c(r3)   ->  lwz r3,0x0(r3)
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccb50), SeekOrigin.Begin); stream.Write(PowerPcAsm.lwz(3, 0x0, 3));

        }

        protected override void readAsm(EndianBinaryReader s, List<MapDescriptor> mapDescriptors, AddressMapper addressMapper, bool isVanilla)
        {
            if (isVanilla)
            {
                s.Seek(0x2c, SeekOrigin.Current); // offset
            }
            foreach (var mapDescriptor in mapDescriptors)
            {
                var address = (VAVAddr) s.ReadUInt32();
                var pos = s.BaseStream.Position;
                readLoopingModeConfig(address, s, mapDescriptor, addressMapper);
                s.BaseStream.Seek(pos, SeekOrigin.Begin);

                if (isVanilla)
                {
                    // in vanilla main.dol the table has other stuff in it like bgm id, map frb files, etc.
                    // this we need to skip to go the next target amount in the table
                    s.Seek(0x38 - 0x4, SeekOrigin.Current);
                }
            }
        }

        private void readLoopingModeConfig(VAVAddr address, EndianBinaryReader s, MapDescriptor mapDescriptor, AddressMapper addressMapper)
        {
            if (addressMapper.canConvertToFileAddress(address))
            {
                s.Seek(addressMapper.toFileAddress(address), SeekOrigin.Begin);
                mapDescriptor.LoopingModeRadius = s.ReadSingle();
                mapDescriptor.LoopingModeHorizontalPadding = s.ReadSingle();
                mapDescriptor.LoopingModeVerticalSquareCount = s.ReadSingle();
            }
        }

        protected override VAVAddr readTableAddr(EndianBinaryReader stream, AddressMapper addressMapper, bool isVanilla)
        {
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccb44), SeekOrigin.Begin);
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
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccb40), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            // mulli r0,r3,0x38
            return opcode == PowerPcAsm.mulli(0, 3, 0x38);
        }
    }
}
