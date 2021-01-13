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
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8021e828), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(30, v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi(30, 30, v.lower16Bit));
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

            // --- Hack to make icons invisible which do not have a map ---
            // -- Init Maps in the map array with -1 --
            var subroutineInitMapIdsForMapIcons = allocate(writeSubroutineInitMapIdsForMapIcons(addressMapper, VAVAddr.NullAddress), "SubroutineInitMapIdsForMapIcons");
            stream.Seek(addressMapper.toFileAddress(subroutineInitMapIdsForMapIcons), SeekOrigin.Begin);
            stream.Write(writeSubroutineInitMapIdsForMapIcons(addressMapper, subroutineInitMapIdsForMapIcons)); // re-write the routine again since now we know where it is located in the main dol
            // increase the array size
            // rlwinm r3,r16,0x2,0x0,0x1d                            -> r3,r16,0x3,0x0,0x1d
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80187794), SeekOrigin.Begin); stream.Write(PowerPcAsm.rlwinm(3, 16, 0x3, 0x0, 0x1d));
            var hijackAddr = addressMapper.toVersionAgnosticAddress((BSVAddr)0x8018779c);
            // cmpwi r3,0x0                                          ->  bl subroutineInitMapIdsForMapIcons
            stream.Seek(addressMapper.toFileAddress(hijackAddr), SeekOrigin.Begin); stream.Write(PowerPcAsm.bl(hijackAddr, subroutineInitMapIdsForMapIcons));
            // mr r24,r3                                             ->  cmpwi r3,0x0
            stream.Write(PowerPcAsm.cmpwi(3, 0));
            // increase the array size
            // rlwinm r3,r16,0x2,0x0,0x1d                            -> r3,r16,0x3,0x0,0x1d
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80187aa4), SeekOrigin.Begin); stream.Write(PowerPcAsm.rlwinm(3, 16, 0x3, 0x0, 0x1d));
            hijackAddr = addressMapper.toVersionAgnosticAddress((BSVAddr)0x80187aac);
            // cmpwi r3,0x0                                          ->  bl subroutineInitMapIdsForMapIcons
            stream.Seek(addressMapper.toFileAddress(hijackAddr), SeekOrigin.Begin); stream.Write(PowerPcAsm.bl(hijackAddr, subroutineInitMapIdsForMapIcons));
            // mr r24,r3                                             ->  cmpwi r3,0x0
            stream.Write(PowerPcAsm.cmpwi(3, 0));
            // -- if a map id is -1, make the map icon invisible --
            hijackAddr = addressMapper.toVersionAgnosticAddress((BSVAddr)0x8021e73c);
            var returnContinueAddr = addressMapper.toVersionAgnosticAddress((BSVAddr)0x8021e740);
            var returnMakeInvisibleAddr = addressMapper.toVersionAgnosticAddress((BSVAddr)0x8021e808);
            var subroutineMakeNoneMapIconsInvisible = allocate(writeSubroutineMakeNoneMapIconsInvisible(addressMapper, VAVAddr.NullAddress, returnContinueAddr, returnMakeInvisibleAddr), "SubroutineMakeNoneMapIconsInvisibleMultiplayer");
            stream.Seek(addressMapper.toFileAddress(subroutineMakeNoneMapIconsInvisible), SeekOrigin.Begin);
            stream.Write(writeSubroutineMakeNoneMapIconsInvisible(addressMapper, subroutineMakeNoneMapIconsInvisible, returnContinueAddr, returnMakeInvisibleAddr)); // re-write the routine again since now we know where it is located in the main dol
            // lwz r0,0x184(r3)                                      ->  b subroutineMakeNoneMapIconsInvisible
            stream.Seek(addressMapper.toFileAddress(hijackAddr), SeekOrigin.Begin); stream.Write(PowerPcAsm.b(hijackAddr, subroutineMakeNoneMapIconsInvisible));

            // -- if the map id is -1, do not check if it has been unlocked or not ---
            hijackAddr = addressMapper.toVersionAgnosticAddress((BSVAddr)0x8021e570);
            returnContinueAddr = addressMapper.toVersionAgnosticAddress((BSVAddr)0x8021e574);
            var returnSkipMapUnlockedCheck = addressMapper.toVersionAgnosticAddress((BSVAddr)0x8021e5a8);
            var subroutineWriteSubroutineSkipMapUnlockCheck = allocate(writeSubroutineSkipMapUnlockCheck(addressMapper, VAVAddr.NullAddress, returnContinueAddr, returnSkipMapUnlockedCheck), "SubroutineWriteSubroutineSkipMapUnlockCheck");
            stream.Seek(addressMapper.toFileAddress(subroutineWriteSubroutineSkipMapUnlockCheck), SeekOrigin.Begin);
            stream.Write(writeSubroutineSkipMapUnlockCheck(addressMapper, subroutineWriteSubroutineSkipMapUnlockCheck, returnContinueAddr, returnSkipMapUnlockedCheck)); // re-write the routine again since now we know where it is located in the main dol
            // or r3,r26,r26                                      ->  b subroutineWriteSubroutineSkipMapUnlockCheck
            stream.Seek(addressMapper.toFileAddress(hijackAddr), SeekOrigin.Begin); stream.Write(PowerPcAsm.b(hijackAddr, subroutineWriteSubroutineSkipMapUnlockCheck));

            // --- Various Map UI Fixes ---
            // -- if the map index is over the map array size, do not loop around to the first map index again --
            // ble 0x80187e1c                                        ->  b 0x80187e1c
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80187dfc), SeekOrigin.Begin); stream.Write(PowerPcAsm.b(8));
            // -- fix map selection going out of bounds in tour mode --
            // bne 0x80188258                                        ->  nop
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80188230), SeekOrigin.Begin); stream.Write(PowerPcAsm.nop());
        }

        private List<UInt32> writeSubroutineSkipMapUnlockCheck(AddressMapper addressMapper, VAVAddr entryAddr, VAVAddr returnContinueAddr, VAVAddr returnSkipMapUnlockedCheck)
        {
            // precondition:  r26  is mapid
            //                 r3  is unused
            // postcondition:  r3  is mapid
            var asm = new List<UInt32>();
            asm.Add(PowerPcAsm.or(3, 26, 26));                                         // r3 <- mapid
            asm.Add(PowerPcAsm.cmpwi(3, -1));                                          // mapid == -1 ?
            asm.Add(PowerPcAsm.beq(entryAddr, asm.Count, returnSkipMapUnlockedCheck)); //   goto skipMapUnlockedCheck
            asm.Add(PowerPcAsm.b(entryAddr, asm.Count, returnContinueAddr));           // else goto returnContinueAddr
            return asm;
        }

        private List<UInt32> writeSubroutineInitMapIdsForMapIcons(AddressMapper addressMapper, VAVAddr entryAddr)
        {
            var JUtility_memset = addressMapper.toVersionAgnosticAddress((BSVAddr)0x80004714);
            // precondition: r3 is newly created map icon array
            //               r16 is the amount of map ids in the array (size / 4)
            //               r24 is unused
            // postcondition: r24 is the map icon array
            var asm = new List<UInt32>();
            asm.Add(PowerPcAsm.mflr(24));                                     // save the link register
            asm.Add(PowerPcAsm.li(4, -1));                                    // fill with 0xff
            asm.Add(PowerPcAsm.rlwinm(5, 16, 0x3, 0x0, 0x1d));                // get the size of the array
            asm.Add(PowerPcAsm.bl(entryAddr, asm.Count, JUtility_memset));    // call JUtility_memset(array*, 0xff, array.size)
            asm.Add(PowerPcAsm.mtlr(24));                                     // restore the link register
            asm.Add(PowerPcAsm.mr(24, 3));                                    // move array* to r24
            asm.Add(PowerPcAsm.blr());                                        // return
            return asm;
        }

        private List<UInt32> writeSubroutineMakeNoneMapIconsInvisible(AddressMapper addressMapper, VAVAddr entryAddr, VAVAddr returnContinueAddr, VAVAddr returnMakeInvisibleAddr)
        {
            var Scene_Layout_Obj_SetVisible = addressMapper.toVersionAgnosticAddress((BSVAddr)0x8006f854);
            // precondition:  r31  MapIconButton*
            //                 r5  is unused
            // postcondition:  r0  is map icon type
            //                 r5  is 0
            var asm = new List<UInt32>();
            asm.Add(PowerPcAsm.lwz(5, 0x188, 31));                                     // get current map id into r5
            asm.Add(PowerPcAsm.cmpwi(5, -1));                                          // map id == -1 ?
            asm.Add(PowerPcAsm.bne(8));                                                // {
            asm.Add(PowerPcAsm.lwz(3, 0x28, 31));                                      //   \ 
            asm.Add(PowerPcAsm.li(5, 0));                                              //   | 
            asm.Add(PowerPcAsm.lwz(4, -0x6600, 13));                                   //   | make "NEW" text invisible
            asm.Add(PowerPcAsm.bl(entryAddr, asm.Count, Scene_Layout_Obj_SetVisible)); //   /
            asm.Add(PowerPcAsm.lwz(3, 0x28, 31));                                      //   \ 
            asm.Add(PowerPcAsm.li(5, 0));                                              //   / make Locked Map Icon "(?)" invisible
            asm.Add(PowerPcAsm.b(entryAddr, asm.Count, returnMakeInvisibleAddr));      //   returnMakeInvisibleAddr
                                                                                       // } else {
            asm.Add(PowerPcAsm.lwz(0, 0x184, 3));                                      //   get map icon type (replaced opcode)
            asm.Add(PowerPcAsm.b(entryAddr, asm.Count, returnContinueAddr));           //   returnContinueAddr
                                                                                       // }
            return asm;
        }

        protected override void readAsm(EndianBinaryReader s, List<MapDescriptor> mapDescriptors, AddressMapper addressMapper, bool isVanilla)
        {
            foreach (MapDescriptor mapDescriptor in mapDescriptors)
            {
                if (isVanilla)
                {
                    // in vanilla there is a mapping of bgXXX to p_bg_XXX which we will assume here without actually reading what is inside the main.dol
                    if (mapDescriptor.UnlockID < 18)
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
