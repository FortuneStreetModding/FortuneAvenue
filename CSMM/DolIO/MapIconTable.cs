using MiscUtil.Conversion;
using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CustomStreetManager
{
    public class MapIconTable : DolIOTable
    {
        protected Dictionary<string, VAVAddr> writeIconStrings(List<MapDescriptor> mapDescriptors)
        {
            // Find out which map icons exist
            var allUniqueMapIcons = new HashSet<string>();
            foreach (var mapDescriptor in mapDescriptors)
            {
                if (mapDescriptor.MapIcon != null)
                    allUniqueMapIcons.Add(mapDescriptor.MapIcon);
            }

            // write each map icon into the main.dol and remember the location in the mapIcons dictionary
            var mapIcons = new Dictionary<string, VAVAddr>();
            foreach (string mapIcon in allUniqueMapIcons)
            {
                mapIcons.Add(mapIcon, allocate(mapIcon));
            }

            return mapIcons;
        }
        protected VAVAddr writeIconTable(Dictionary<string, VAVAddr> mapIcons, out Dictionary<string, VAVAddr> iconTableMap)
        {
            // write the map icon lookup table and remember the location of each pointer in the mapIconLookupTable dictionary
            var iconTableOffsetMap = new Dictionary<string, int>();
            iconTableMap = new Dictionary<string, VAVAddr>();
            var iconTable = new List<VAVAddr>();
            var i = 0;
            foreach (var entry in mapIcons)
            {
                var addr = entry.Value;
                iconTable.Add(addr);
                iconTableOffsetMap.Add(entry.Key, i);
                i += 4;
            }
            VAVAddr iconTableAddr = allocate(iconTable, "IconTable");
            foreach (var entry in iconTableOffsetMap)
            {
                string icon = entry.Key;
                int offset = entry.Value;
                iconTableMap[icon] = iconTableAddr + offset;
            }
            return iconTableAddr;
        }
        protected VAVAddr writeMapIconPointerTable(List<MapDescriptor> mapDescriptors, Dictionary<string, VAVAddr> iconTableMap)
        {
            var mapIconTable = new List<VAVAddr>();
            foreach (MapDescriptor mapDescriptor in mapDescriptors)
            {
                VAVAddr mapIconAddr = VAVAddr.NullAddress;
                if (mapDescriptor.MapIcon != null)
                {
                    mapIconAddr = iconTableMap[mapDescriptor.MapIcon];
                }
                mapIconTable.Add(mapIconAddr);
            }
            return allocate(mapIconTable, "MapIconPointerTable");
        }
        protected override void writeAsm(EndianBinaryWriter stream, AddressMapper addressMapper, List<MapDescriptor> mapDescriptors)
        {
            var mapIcons = writeIconStrings(mapDescriptors);
            Dictionary<string, VAVAddr> iconTableMap;
            VAVAddr iconTableAddr = writeIconTable(mapIcons, out iconTableMap);
            VAVAddr mapIconPointerTable = writeMapIconPointerTable(mapDescriptors, iconTableMap);
            ushort iconCount = (ushort)iconTableMap.Count;
            short tableRowCount = (short)mapDescriptors.Count;

            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair((UInt32)iconTableAddr);
            PowerPcAsm.Pair16Bit w = PowerPcAsm.make16bitValuePair((UInt32)mapIconPointerTable);
            // note: To add custom icons, the following files need to be editted as well:
            // - ui_menu_19_00a.brlyt within game_sequence.arc and within game_sequence_wifi.arc
            // - ui_menu_19_00a_Tag_*.brlan within game_sequence.arc and within game_sequence_wifi.arc

            // custom map icon hack (change it that way that it will call the GetMapDifficulty routine instead of the GetMapOrigin routine
            // the GetMapDifficulty routine is mostly unused by the game and we repurpose it to return the pointer to the pointer of the string of the map icon instead
            // then we go through all map icon pointer pointers and check if it is the same as the one retrieved. If it is then we make it visible, otherwise we set the visibility to false.
            var GetMapDifficulty = addressMapper.toVersionAgnosticAddress((BSVAddr)0x80211da4);

            // bl GetMapOrigin                                     -> bl GetMapDifficulty
            var offset = addressMapper.toVersionAgnosticAddress((BSVAddr)0x8021e77c);
            stream.Seek(addressMapper.toFileAddress(offset), SeekOrigin.Begin); stream.Write(PowerPcAsm.bl(offset, GetMapDifficulty));
            // cmpw r28,r30                                        -> cmpw r29,r30
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8021e790), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpw(29, 30));
            // cmplwi r28,0x12                                     -> cmplwi r28,iconCount
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8021e7c0), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmplwi(28, iconCount));
            // bl GetMapOrigin                                     -> bl GetMapDifficulty
            offset = addressMapper.toVersionAgnosticAddress((BSVAddr)0x8021e8a4);
            stream.Seek(addressMapper.toFileAddress(offset), SeekOrigin.Begin); stream.Write(PowerPcAsm.bl(offset, GetMapDifficulty));
            // cmpw r29,r28                                        -> cmpw r30,r28
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8021e8b8), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpw(30, 28));
            // cmplwi r29,0x12                                     -> cmplwi r29,iconCount
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8021e8e8), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmplwi(29, iconCount));
            // bl GetMapOrigin                                     -> bl GetMapDifficulty
            offset = addressMapper.toVersionAgnosticAddress((BSVAddr)0x8021e824);
            stream.Seek(addressMapper.toFileAddress(offset), SeekOrigin.Begin); stream.Write(PowerPcAsm.bl(offset, GetMapDifficulty));
            // cmplwi r28,0x12                                     -> cmplwi r28,iconCount
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8021e84c), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmplwi(28, iconCount));
            // r29 <- 0x8047f5c0                                   -> r29 <- iconTableAddr
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8021e780), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(29, v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi(29, 29, v.lower16Bit));
            // r30 <- 0x8047f5c0                                   -> r30 <- iconTableAddr
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8021e8a8), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(30, v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi(30, 30, v.lower16Bit));
            // r30 <- 0x8047f5c0                                   -> r30 <- iconTableAddr
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8021e830), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(30, v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi(30, 30, v.lower16Bit));
            // mr r3,r28                                           -> mr r3,r26
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8021e94c), SeekOrigin.Begin); stream.Write(PowerPcAsm.mr(3, 26));
            // mr r3,r28                                           -> mr r3,r26
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8021e968), SeekOrigin.Begin); stream.Write(PowerPcAsm.mr(3, 26));

            // Modify the GetMapDifficulty routine to retrieve the current map icon addr addr
            // subi r31,r3,0x15                                   ->  nop
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211dc8), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // cmpwi r31,0x12                                     ->  cmpwi r31,tableRowCount
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211dd4), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpwi(31, tableRowCount));
            // li r3,0x15                                         ->  nop
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211e4c), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
            // mulli r4,r3,0x24                                   ->  mulli r4,r3,0x04
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211e58), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(4, 3, 0x04));
            // r3 <- 804363c8                                     ->  r3 <- mapIconPointerTable
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211e5c), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(3, w.upper16Bit)); stream.Write(PowerPcAsm.addi(3, 3, w.lower16Bit));
            // mulli r0,r31,0x24                                  ->  mulli r0,r31,0x04
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211e64), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(0, 31, 0x04));
            // lwz r3,0x1c(r3)                                    ->  lwz r3,0x0(r3)
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211e78), SeekOrigin.Begin); stream.Write(PowerPcAsm.lwz(3, 0x0, 3));

        }
        protected override void readAsm(EndianBinaryReader s, List<MapDescriptor> mapDescriptors, AddressMapper addressMapper, bool isVanilla)
        {
            foreach (MapDescriptor mapDescriptor in mapDescriptors)
            {
                if (isVanilla)
                {
                    // in vanilla there is a mapping of bgXXX to p_bg_XXX which we will assume here without actually reading what is inside the main.dol
                    if (mapDescriptor.ID < 18)
                    {
                        var number = Regex.Match(mapDescriptor.Background, @"\d+").Value;
                        mapDescriptor.MapIcon = "p_bg_" + number;
                    }
                }
                else
                {
                    mapDescriptor.MapIcon = resolveAddressAddressToString((VAVAddr)s.ReadUInt32(), s, addressMapper);
                }
            }
        }
        protected string resolveAddressAddressToString(VAVAddr virtualAddressAddress, EndianBinaryReader stream, AddressMapper addressMapper)
        {
            if (virtualAddressAddress == VAVAddr.NullAddress)
                return null;
            int fileAddress = addressMapper.toFileAddress(virtualAddressAddress);
            var pos = stream.BaseStream.Position;
            stream.Seek(fileAddress, SeekOrigin.Begin);
            VAVAddr virtualAddress = (VAVAddr)stream.ReadUInt32();
            stream.BaseStream.Seek(pos, SeekOrigin.Begin);
            return resolveAddressToString(virtualAddress, stream, addressMapper);
        }

        protected override VAVAddr readTableAddr(EndianBinaryReader stream, AddressMapper addressMapper, bool isVanilla)
        {
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211e5c), SeekOrigin.Begin);
            var lis_opcode = stream.ReadUInt32();
            var addi_opcode = stream.ReadUInt32();
            return (VAVAddr)PowerPcAsm.make32bitValueFromPair(lis_opcode, addi_opcode);
        }
        protected override Int16 readTableRowCount(EndianBinaryReader stream, AddressMapper addressMapper, bool isVanilla)
        {
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80211dd4), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            return PowerPcAsm.getOpcodeParameter(opcode);
        }
        protected override bool readIsVanilla(EndianBinaryReader stream, AddressMapper addressMapper)
        {
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8021e790), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            return opcode == PowerPcAsm.cmpw(28, 30);
        }
    }
}
