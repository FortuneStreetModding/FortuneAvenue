using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSEditor.MapDescriptor
{
    public interface AddressConstants
    {
        uint VANILLA_FIRST_MAP_NAME_MESSAGE_ID();
        uint VANILLA_FIRST_MAP_DESC_MESSAGE_ID();
        // map data string table
        UInt32 UNUSED_SPACE_1_START_VIRTUAL();
        UInt32 UNUSED_SPACE_1_END_VIRTUAL();
        // unused costume string table 1
        UInt32 UNUSED_SPACE_2_START_VIRTUAL();
        UInt32 UNUSED_SPACE_2_END_VIRTUAL();
        // unused costume string table 2
        UInt32 UNUSED_SPACE_3_START_VIRTUAL();
        UInt32 UNUSED_SPACE_3_END_VIRTUAL();
        // unused costume string table 3
        UInt32 UNUSED_SPACE_4_START_VIRTUAL();
        UInt32 UNUSED_SPACE_4_END_VIRTUAL();
        UInt32 MAP_SWITCH_PARAM_ADDR_MAGMAGEDDON();
        UInt32 MAP_SWITCH_PARAM_ADDR_COLLOSUS();
        UInt32 MAP_SWITCH_PARAM_ADDR_OBSERVATORY();
        UInt32 MAP_BGSEQUENCE_ADDR_MARIOSTADIUM();
        UInt32 START_MAP_DEFAULTS_TABLE_VIRTUAL();
        UInt32 START_MAP_DATA_TABLE_VIRTUAL();
        UInt32 START_VENTURE_CARD_TABLE_VIRTUAL();
        UInt32 START_MAP_DESCRIPTION_MSG_TABLE_VIRTUAL();
    }
}
