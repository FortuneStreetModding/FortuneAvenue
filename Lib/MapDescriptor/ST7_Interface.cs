using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSEditor.MapDescriptor
{
    public interface ST7_Interface
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

        void writeHackExtendedMapDescriptions(EndianBinaryWriter stream, Func<uint, int> toFileAddress, short mapDescriptionTableSize, uint mapDescriptionTableAddr);
        bool isHackExtendedMapDescriptions(EndianBinaryReader stream, Func<uint, int> toFileAddress);
        Int16 readMapDescriptionTableSize(EndianBinaryReader stream, Func<uint, int> toFileAddress);
        UInt32 readMapDescriptionTableAddr(EndianBinaryReader stream, Func<uint, int> toFileAddress);
        void writeHackExtendedMapDefaultsTable(EndianBinaryWriter stream, Func<UInt32, int> toFileAddress);
        bool isHackExtendedMapDefaultsTable(EndianBinaryReader stream, Func<UInt32, int> toFileAddress);
        void writeHackExtendedMapSettingsTable(EndianBinaryWriter stream, Func<UInt32, int> toFileAddress);
        bool isHackExtendedMapSettingsTable(EndianBinaryReader stream, Func<UInt32, int> toFileAddress);
        void writeHackCustomMapIcons(EndianBinaryWriter stream, Func<UInt32, int> toFileAddress, Int16 mapIconAddrTableItemCount, UInt32 mapIconAddrTable);
        bool isHackCustomMapIcons(EndianBinaryReader stream, Func<uint, int> toFileAddress);
        void writeHackExtendedVentureCardTable(EndianBinaryWriter stream, Func<UInt32, int> toFileAddress, Int16 ventureCardTableCount, UInt32 ventureCardTableAddr);
        short readVentureCardTableEntryCount(EndianBinaryReader stream, Func<uint, int> toFileAddress);
        uint readVentureCardTableAddr(EndianBinaryReader stream, Func<uint, int> toFileAddress);
    }
}
