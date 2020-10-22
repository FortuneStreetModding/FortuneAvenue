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
    public class MapIconTable : DolIO
    {
        protected override void writeAsm(EndianBinaryWriter stream, Func<uint, int> toFileAddress, Int16 tableRowCount, UInt32 mapIconAddrTable)
        {
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair(mapIconAddrTable);
            // note: To add custom icons, the following files need to be editted as well:
            // - ui_menu_19_00a.brlyt within game_sequence.arc and within game_sequence_wifi.arc

            // custom map icon hack
            // bl GetMapOrigin                                     -> bl GetMapDifficulty
            stream.Seek(toFileAddress(0x8021e77c), SeekOrigin.Begin); stream.Write(PowerPcAsm.bl(0x8021e77c, 0x80211da4));
            // cmpw r28,r30                                        -> cmpw r29,r30
            stream.Seek(toFileAddress(0x8021e790), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpw(29, 30));
            // cmplwi r28,0x12                                     -> cmplwi r28,mapIconAddrTableItemCount
            stream.Seek(toFileAddress(0x8021e7c0), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmplwi(28, (ushort)tableRowCount));
            // bl GetMapOrigin                                     -> bl GetMapDifficulty
            stream.Seek(toFileAddress(0x8021e8a4), SeekOrigin.Begin); stream.Write(PowerPcAsm.bl(0x8021e8a4, 0x80211da4));
            // cmpw r29,r28                                        -> cmpw r30,r28
            stream.Seek(toFileAddress(0x8021e8b8), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpw(30, 28));
            // cmplwi r29,0x12                                     -> cmplwi r29,mapIconAddrTableItemCount
            stream.Seek(toFileAddress(0x8021e8e8), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmplwi(29, (ushort)tableRowCount));
            // cmpw r29,r28                                        -> cmpw r30,r28
            stream.Seek(toFileAddress(0x8021e8b8), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmpw(30, 28));
            // cmplwi r28,0x12                                     -> cmplwi r28,mapIconAddrTableItemCount
            stream.Seek(toFileAddress(0x8021e84c), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmplwi(28, (ushort)tableRowCount));
            // r29 <- 0x8047f5c0                                   -> r29 <- mapIconAddrTable
            stream.Seek(toFileAddress(0x8021e780), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(29, v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi(29, 29, v.lower16Bit));
            // r30 <- 0x8047f5c0                                   -> r30 <- mapIconAddrTable
            stream.Seek(toFileAddress(0x8021e8a8), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(30, v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi(30, 30, v.lower16Bit));
            // r30 <- 0x8047f5c0                                   -> r30 <- mapIconAddrTable
            stream.Seek(toFileAddress(0x8021e830), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(30, v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi(30, 30, v.lower16Bit));
        }
        protected override string writeTable(EndianBinaryWriter s, List<MapDescriptor> mapDescriptors)
        {
            foreach (var mapDescriptor in mapDescriptors)
            {
                s.Write(mapDescriptor.Desc_MSG_ID);
            }
            return "MapDescriptionTable";
        }
        protected override void readTable(EndianBinaryReader s, List<MapDescriptor> mapDescriptors, Func<uint, int> toFileAddress, bool isVanilla)
        {
            foreach (MapDescriptor mapDescriptor in mapDescriptors)
            {
                if (isVanilla)
                {
                    if (mapDescriptor.ID < 18)
                    {
                        mapDescriptor.MapIconAddrAddr = 0;
                        var number = Regex.Match(mapDescriptor.Background, @"\d+").Value;
                        mapDescriptor.MapIcon = "p_bg_" + number;
                    }
                }
                else
                {
                    mapDescriptor.MapIcon = resolveAddressAddressToString(mapDescriptor.MapIconAddrAddr, s, toFileAddress);
                }
            }
        }
        protected string resolveAddressAddressToString(uint virtualAddressAddress, EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            int fileAddress = toFileAddress(virtualAddressAddress);
            if (fileAddress >= 0)
            {
                stream.Seek(fileAddress, SeekOrigin.Begin);
                var virtualAddress = stream.ReadUInt32();
                return resolveAddressToString(virtualAddress, stream, toFileAddress);
            }
            else
            {
                return null;
            }
        }

        protected override UInt32 readTableAddr(EndianBinaryReader stream, Func<uint, int> toFileAddress, bool isVanilla)
        {
            stream.Seek(toFileAddress(0x8021e780), SeekOrigin.Begin);
            var lis_opcode = stream.ReadUInt32();
            stream.Seek(4, SeekOrigin.Current);
            var addi_opcode = stream.ReadUInt32();
            return PowerPcAsm.make32bitValueFromPair(lis_opcode, addi_opcode);
        }
        protected override Int16 readTableRowCount(EndianBinaryReader stream, Func<uint, int> toFileAddress, bool isVanilla)
        {
            stream.Seek(toFileAddress(0x8021e7c0), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            return PowerPcAsm.getOpcodeParameter(opcode);
        }
        protected override bool readIsVanilla(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            // has the hack for expanded Description message table already been applied?
            stream.Seek(toFileAddress(0x8021e790), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            return opcode == PowerPcAsm.cmpw(28, 30);
        }
    }
}
