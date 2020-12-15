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
    public class MapSwitchParamTable : DolIOTable
    {
        protected VAVAddr writeTable(List<MapDescriptor> mapDescriptors)
        {
            var mapSwitchParamTable = new List<VAVAddr>();
            foreach (var mapDescriptor in mapDescriptors)
            {
                if (mapDescriptor.SwitchRotationOriginPoints.Count == 0)
                {
                    mapSwitchParamTable.Add(VAVAddr.NullAddress);
                }
                else
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                        s.Write((UInt32)mapDescriptor.SwitchRotationOriginPoints.Count);
                        for (int i = 0; i < mapDescriptor.SwitchRotationOriginPoints.Count; i++)
                        {
                            s.Write(mapDescriptor.SwitchRotationOriginPoints[i].X);
                            s.Write((UInt32)0);
                            s.Write(mapDescriptor.SwitchRotationOriginPoints[i].Y);
                        }
                        var loopingModeConfigAddr = allocate(memoryStream.ToArray(), "MapRotationOriginPoints for " + mapDescriptor.InternalName);
                        mapSwitchParamTable.Add(loopingModeConfigAddr);
                    }
                }
            }
            return allocate(mapSwitchParamTable, "MapSwitchParamTable");
        }
        protected override void writeAsm(EndianBinaryWriter stream, AddressMapper addressMapper, List<MapDescriptor> mapDescriptors)
        {
            var tableAddr = writeTable(mapDescriptors);
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair((UInt32)tableAddr);

            // --- Game::GetMapSwitchParam ---
            // mulli r0,r3,0x38  ->  mulli r0,r3,0x4
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccb28), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(0, 3, 0x4));
            // r3 <- 0x80428e50  ->  r3 <- tableAddr
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccb2c), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(3, v.upper16Bit)); stream.Write(PowerPcAsm.addi(3, 3, v.lower16Bit));
            // lwz r3,0x28(r3)   ->  lwz r3,0x0(r3)
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccb38), SeekOrigin.Begin); stream.Write(PowerPcAsm.lwz(3, 0x0, 3));

        }

        protected override void readAsm(EndianBinaryReader s, List<MapDescriptor> mapDescriptors, AddressMapper addressMapper, bool isVanilla)
        {
            if (isVanilla)
            {
                s.Seek(0x28, SeekOrigin.Current); // offset
            }
            foreach (var mapDescriptor in mapDescriptors)
            {
                var address = (VAVAddr) s.ReadUInt32();
                var pos = s.BaseStream.Position;
                readRotationOriginPoints(address, s, mapDescriptor, addressMapper);
                s.BaseStream.Seek(pos, SeekOrigin.Begin);

                if (isVanilla)
                {
                    // in vanilla main.dol the table has other stuff in it like bgm id, map frb files, etc.
                    // this we need to skip to go the next target amount in the table
                    s.Seek(0x38 - 0x4, SeekOrigin.Current);
                }
            }
        }

        private void readRotationOriginPoints(VAVAddr address, EndianBinaryReader s, MapDescriptor mapDescriptor, AddressMapper addressMapper)
        {
            mapDescriptor.SwitchRotationOriginPoints.Clear();
            // Special case handling: in the original game these values are initialized at run time only. So we need to hardcode them:
            if (address == addressMapper.toVersionAgnosticAddress((BSVAddr)0x806b8df0)) // magmageddon
            {
                // no points
            }
            else if (address == addressMapper.toVersionAgnosticAddress((BSVAddr)0x8047d598)) // collosus
            {
                mapDescriptor.SwitchRotationOriginPoints[0] = new OriginPoint(-288, -32);
                mapDescriptor.SwitchRotationOriginPoints[1] = new OriginPoint(288, -32);
            }
            else if (address == addressMapper.toVersionAgnosticAddress((BSVAddr)0x8047d5b4)) // observatory
            {
                mapDescriptor.SwitchRotationOriginPoints[0] = new OriginPoint(0, 0);
            }
            else if (addressMapper.canConvertToFileAddress(address))
            {
                s.Seek(addressMapper.toFileAddress(address), SeekOrigin.Begin);
                var originPointCount = s.ReadUInt32();
                for (int i = 0; i < originPointCount; i++)
                {
                    OriginPoint point = new OriginPoint();
                    point.X = s.ReadSingle();
                    var z = s.ReadSingle(); // ignore Z value
                    point.Y = s.ReadSingle();
                    mapDescriptor.SwitchRotationOriginPoints[i] = point;
                }
            }
        }

        protected override VAVAddr readTableAddr(EndianBinaryReader stream, AddressMapper addressMapper, bool isVanilla)
        {
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccb2c), SeekOrigin.Begin);
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
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801ccb28), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            // mulli r0,r3,0x38
            return opcode == PowerPcAsm.mulli(0, 3, 0x38);
        }
    }
}
