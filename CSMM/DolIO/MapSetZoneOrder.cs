using MiscUtil.Conversion;
using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CustomStreetMapManager
{
    public class MapSetZoneOrder : DolIOTable
    {

        protected VAVAddr writeTable(List<MapDescriptor> mapDescriptors)
        {
            var mapSetZoneOrderTable = new List<sbyte>();
            foreach (var mapDescriptor in mapDescriptors)
            {
                mapSetZoneOrderTable.Add(mapDescriptor.MapSet);
                var zone = mapDescriptor.Zone;
                if (zone == 0)
                    zone = 1;
                else if (zone == 1)
                    zone = 0;
                mapSetZoneOrderTable.Add(zone);
                mapSetZoneOrderTable.Add(mapDescriptor.Order);
            }
            return allocate(mapSetZoneOrderTable, "MapSetZoneOrderTable");
        }

        protected override void writeAsm(EndianBinaryWriter stream, AddressMapper addressMapper, List<MapDescriptor> mapDescriptors)
        {
            var tableAddr = writeTable(mapDescriptors);

            // --- Game::GameSequenceDataAdapter::GetNumMapsInZone ---
            var subroutineGetNumMapsInZone = allocate(writeSubroutineGetNumMapsInZone(mapDescriptors), "SubroutineGetNumMapsInZone");
            var hijackAddr = addressMapper.toVersionAgnosticAddress((BSVAddr)0x8020f3ac);
            stream.Seek(addressMapper.toFileAddress(hijackAddr), SeekOrigin.Begin);
            // li r3,0x6 ->  b subroutineGetNumMapsInZone
            stream.Write(PowerPcAsm.b(hijackAddr, subroutineGetNumMapsInZone));

            // --- Game::GameSequenceDataAdapter::GetMapsInZone ---
            hijackAddr = addressMapper.toVersionAgnosticAddress((BSVAddr)0x8020f454);
            var returnAddr = addressMapper.toVersionAgnosticAddress((BSVAddr)0x8020f554);
            var subroutineGetMapsInZone = allocate(writeSubroutineGetMapsInZone(addressMapper, mapDescriptors, tableAddr, VAVAddr.NullAddress, returnAddr), "SubroutineGetMapsInZone");
            stream.Seek(addressMapper.toFileAddress(subroutineGetMapsInZone), SeekOrigin.Begin);
            stream.Write(writeSubroutineGetMapsInZone(addressMapper, mapDescriptors, tableAddr, subroutineGetMapsInZone, returnAddr)); // re-write the routine again since now we know where it is located in the main dol
            stream.Seek(addressMapper.toFileAddress(hijackAddr), SeekOrigin.Begin);
            // cmpwi r29,0x0 ->  b subroutineGetMapsInZone
            stream.Write(PowerPcAsm.b(hijackAddr, subroutineGetMapsInZone));

            // --- Write Table Meta Information --- 
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020f458), SeekOrigin.Begin);
            stream.Write((uint)tableAddr);
            stream.Write((short)0);
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020F45E), SeekOrigin.Begin);
            stream.Write((short)mapDescriptors.Count);

            // --- Fix Wifi Map Selection ---
            // bl GameSequenceDataAdapter::GetMapOrigin(r3)                               -> nop
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80185ac4), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // or r31,r3,r3                                                               -> or r31,r4,r4
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80185ac8), SeekOrigin.Begin); stream.Write(PowerPcAsm.or(31, 4, 4));
            // li r5,0x1                                                                  -> li r5,0x2
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80185b10), SeekOrigin.Begin); stream.Write(PowerPcAsm.li(5, 2));
            // bl GameSequenceDataAdapter::GetMapOrigin(r3)                               -> nop
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8024b1b8), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // bl GameSequenceDataAdapter::GetMapOrigin(r3)                               -> nop
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x802498a8), SeekOrigin.Begin);
            for (int i = 0; i < 8; i++)
            {
                stream.Write(PowerPcAsm.nop());
            }
            // --- Default selected map button in wifi ---
            // since Standard Mode is selected on default, we use this mapset to find the standard map
            // TODO need asm hack to determine currently selected MapSet and use rather use that ID
            short defaultMap = 0;
            for (short i = 0; i < mapDescriptors.Count; i++)
            {
                var mapDescriptor = mapDescriptors[i];
                if (mapDescriptor.MapSet == 1 && mapDescriptor.Zone == 0 && mapDescriptor.Order == 0)
                { 
                    defaultMap = i;
                }
            }
            // 0x9 = Castle Trodain
            // li r3,0x9                                                                  -> li r3,0x9
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8024afc8), SeekOrigin.Begin); stream.Write(PowerPcAsm.li(3, defaultMap));
            // TODO 0x13 is some special handling map id. Need to check what is going on with it
            // li r3,0x13                                                                 -> li r3,0x13
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80243ae4), SeekOrigin.Begin); stream.Write(PowerPcAsm.li(5, 0x13));
            // --- Fix out of array bounds error when opening tour mode and viewing the zones and having more than 6 maps in a zone ---
            // bl Game::GameSequenceDataAdapter::GetNumMapsInZone    -> li r3,0x6
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8021f880), SeekOrigin.Begin); stream.Write(PowerPcAsm.li(3, 0x6));
            // bl Game::GameSequenceDataAdapter::GetNumMapsInZone    -> li r3,0x6
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8021ff4c), SeekOrigin.Begin); stream.Write(PowerPcAsm.li(3, 0x6));
        }

        private List<UInt32> writeSubroutineGetNumMapsInZone(List<MapDescriptor> mapDescriptors)
        {
            // precondition:  r3  _ZONE_TYPE
            // postcondition: r3  num maps
            var asm = new List<UInt32>();
            for (short i = 0; i < 6; i++)
            {
                asm.Add(PowerPcAsm.cmpwi(3, i));
                asm.Add(PowerPcAsm.bne(3));
                asm.Add(PowerPcAsm.li(3, (short)(from m in mapDescriptors where m.Zone == i && m.MapSet == 0 select m).Count()));
                asm.Add(PowerPcAsm.blr());
            }
            asm.Add(PowerPcAsm.blr());
            return asm;
        }

        private List<UInt32> writeSubroutineGetMapsInZone(AddressMapper addressMapper, List<MapDescriptor> mapDescriptors, VAVAddr mapSetZoneOrderTable, VAVAddr entryAddr, VAVAddr returnAddr)
        {
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair((UInt32)mapSetZoneOrderTable);
            // precondition:  r5  MapSet
            //               r29  _ZONE_TYPE
            //               r30  int* array containing map ids
            //                r3,r4,r6,r7,r31  unused
            // postcondition: r3  num maps (must be same as in SubroutineGetNumMapsInZone)
            var asm = new List<UInt32>();
            var asm_l2 = new List<UInt32>();
            var asm_l3 = new List<UInt32>();

            asm.Add(PowerPcAsm.li(3, 0));
            var mapSets = (from m in mapDescriptors where m.MapSet != -1 orderby m.MapSet select m.MapSet).Distinct();
            foreach (var mapSet in mapSets)
            {
                asm.Add(PowerPcAsm.cmpwi(5, mapSet));
                var zones = (from m in mapDescriptors where m.MapSet == mapSet && m.Zone != -1 orderby m.Zone select m.Zone).Distinct();
                asm_l2.Clear();
                foreach (var zone in zones)
                {
                    asm_l2.Add(PowerPcAsm.cmpwi(29, zone));
                    IOrderedEnumerable<MapDescriptor> maps;
                    if (zone == 0)
                        maps = from m in mapDescriptors where m.MapSet == mapSet && m.Zone == 1 orderby m.Order select m;
                    else if (zone == 1)
                        maps = from m in mapDescriptors where m.MapSet == mapSet && m.Zone == 0 orderby m.Order select m;
                    else
                        maps = from m in mapDescriptors where m.MapSet == mapSet && m.Zone == zone orderby m.Order select m;
                    short i = 0;
                    asm_l3.Clear();
                    asm_l3.Add(PowerPcAsm.li(3, (short)maps.Count()));
                    foreach (var map in maps)
                    {
                        short mapId = (short)mapDescriptors.IndexOf(map);
                        var mapDescriptor = mapDescriptors[i];
                        asm_l3.Add(PowerPcAsm.li(4, mapId));
                        asm_l3.Add(PowerPcAsm.stw(4, i, 30));
                        i += 4;
                    }
                    asm_l2.Add(PowerPcAsm.bne(asm_l3.Count + 1));
                    asm_l2.AddRange(asm_l3);
                }
                asm.Add(PowerPcAsm.bne(asm_l2.Count + 1));
                asm.AddRange(asm_l2);
            }
            asm.Add(PowerPcAsm.b(entryAddr, asm.Count, returnAddr));
            return asm;
        }

        protected override void readAsm(EndianBinaryReader stream, List<MapDescriptor> mapDescriptors, AddressMapper addressMapper, bool isVanilla)
        {
            if (isVanilla)
            {
                setVanillaMapSetZoneOrder(mapDescriptors);
            }
            else
            {
                for (int i = 0; i < mapDescriptors.Count; i++)
                {
                    var mapDescriptor = mapDescriptors[i];
                    mapDescriptor.MapSet = stream.ReadSByte();
                    mapDescriptor.Zone = stream.ReadSByte();
                    if (mapDescriptor.Zone == 0)
                        mapDescriptor.Zone = 1;
                    else if (mapDescriptor.Zone == 1)
                        mapDescriptor.Zone = 0;
                    mapDescriptor.Order = stream.ReadSByte();
                }
            }
        }

        protected override VAVAddr readTableAddr(EndianBinaryReader stream, AddressMapper addressMapper, bool isVanilla)
        {
            if (isVanilla)
                return VAVAddr.NullAddress;
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020f458), SeekOrigin.Begin);
            var addr = (VAVAddr)stream.ReadUInt32();
            return addr;
        }

        protected override short readTableRowCount(EndianBinaryReader stream, AddressMapper addressMapper, bool isVanilla)
        {
            if (isVanilla)
                return -1;
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020F45E), SeekOrigin.Begin);
            return stream.ReadInt16();
        }

        protected override bool readIsVanilla(EndianBinaryReader stream, AddressMapper addressMapper)
        {
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8020f454), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            return opcode == PowerPcAsm.cmpwi(29, 0);
        }

        private void setVanillaMapSetZoneOrder(List<MapDescriptor> mapDescriptors)
        {
            for (int i = 0; i < mapDescriptors.Count; i++)
            {
                var mapDescriptor = mapDescriptors[i];
                mapDescriptor.MapSet = VanillaDatabase.getVanillaMapSet(i);
                mapDescriptor.Zone = VanillaDatabase.getVanillaZone(i);
                if (mapDescriptor.Zone == 0)
                    mapDescriptor.Zone = 1;
                else if (mapDescriptor.Zone == 1)
                    mapDescriptor.Zone = 0;
                mapDescriptor.Order = VanillaDatabase.getVanillaOrder(i);
            }
        }

    }
}
