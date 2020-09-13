using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSEditor.MapDescriptor
{
    class ST7P01 : AddressConstants
    {
        uint AddressConstants.VANILLA_FIRST_MAP_DESC_MESSAGE_ID() { return 4416; }
        uint AddressConstants.VANILLA_FIRST_MAP_NAME_MESSAGE_ID() { return 5433; }
        uint AddressConstants.MAP_BGSEQUENCE_ADDR_MARIOSTADIUM() { return 0x80428968; }
        uint AddressConstants.MAP_SWITCH_PARAM_ADDR_COLLOSUS() { return 0x8047d598; }
        uint AddressConstants.MAP_SWITCH_PARAM_ADDR_MAGMAGEDDON() { return 0x806b8df0; }
        uint AddressConstants.MAP_SWITCH_PARAM_ADDR_OBSERVATORY() { return 0x8047d5b4; }
        uint AddressConstants.START_MAP_DATA_TABLE_VIRTUAL() { return 0x80428e50; }
        uint AddressConstants.START_MAP_DEFAULTS_TABLE_VIRTUAL() { return 0x804363c8; }
        uint AddressConstants.START_MAP_DESCRIPTION_MSG_TABLE_VIRTUAL() { return 0x80436bc0; }
        uint AddressConstants.START_VENTURE_CARD_TABLE_VIRTUAL() { return 0x80410648; }
        // map data string table repurposed as free space
        uint AddressConstants.UNUSED_SPACE_1_START_VIRTUAL() { return 0x80428978; }
        uint AddressConstants.UNUSED_SPACE_1_END_VIRTUAL() { return 0x80428e4F; }
        // unused costume string table 1
        uint AddressConstants.UNUSED_SPACE_2_START_VIRTUAL() { return 0x8042bc78; }
        uint AddressConstants.UNUSED_SPACE_2_END_VIRTUAL() { return 0x8042c23f; }
        // unused costume string table 2
        uint AddressConstants.UNUSED_SPACE_3_END_VIRTUAL() { return 0x8042e22f; }
        uint AddressConstants.UNUSED_SPACE_3_START_VIRTUAL() { return 0x8042dfc0; }
        // unused costume string table 3
        uint AddressConstants.UNUSED_SPACE_4_START_VIRTUAL() { return 0x8042ef30; }
        uint AddressConstants.UNUSED_SPACE_4_END_VIRTUAL() { return 0x8042f7ef; }
    }
}
