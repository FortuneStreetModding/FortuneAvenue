using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSEditor.MapDescriptor
{
    class ST7E01 : AddressConstants
    {
        public uint VANILLA_FIRST_MAP_DESC_MESSAGE_ID() { return 4416; }
        public uint VANILLA_FIRST_MAP_NAME_MESSAGE_ID() { return 5433; }
        public uint MAP_BGSEQUENCE_ADDR_MARIOSTADIUM() { return 0x0; }
        public uint MAP_SWITCH_PARAM_ADDR_COLLOSUS() { return 0x0; }
        public uint MAP_SWITCH_PARAM_ADDR_MAGMAGEDDON() { return 0x0; }
        public uint MAP_SWITCH_PARAM_ADDR_OBSERVATORY() { return 0x0; }
        public uint START_MAP_DATA_TABLE_VIRTUAL() { return 0x0; }
        public uint START_MAP_DEFAULTS_TABLE_VIRTUAL() { return 0x0; }
        public uint START_MAP_DESCRIPTION_MSG_TABLE_VIRTUAL() { return 0x0; }
        public uint START_VENTURE_CARD_TABLE_VIRTUAL() { return 0x0; }
        // map data string table repurposed as free space
        public uint UNUSED_SPACE_1_START_VIRTUAL() { return 0x0; }
        public uint UNUSED_SPACE_1_END_VIRTUAL() { return 0x0; }
        // unused costume string table 1
        public uint UNUSED_SPACE_2_START_VIRTUAL() { return 0x0; }
        public uint UNUSED_SPACE_2_END_VIRTUAL() { return 0x0; }
        // unused costume string table 2
        public uint UNUSED_SPACE_3_END_VIRTUAL() { return 0x0; }
        public uint UNUSED_SPACE_3_START_VIRTUAL() { return 0x0; }
        // unused costume string table 3
        public uint UNUSED_SPACE_4_START_VIRTUAL() { return 0x0; }
        public uint UNUSED_SPACE_4_END_VIRTUAL() { return 0x0; }

        public void writeHackExtendedMapDescriptions(EndianBinaryWriter stream, Func<UInt32, int> toFileAddress)
        {

        }
        public bool isHackExtendedMapDescriptions(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            return false;
        }

        public void writeHackCustomMapIcons(EndianBinaryWriter stream, Func<UInt32, int> toFileAddress, Int16 mapIconAddrTableItemCount, UInt32 mapIconAddrTable)
        {
        }
        public bool isHackCustomMapIcons(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            return false;
        }
    }
}
