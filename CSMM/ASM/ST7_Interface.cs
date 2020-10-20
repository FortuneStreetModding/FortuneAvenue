using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomStreetManager
{
    public interface ST7_Interface
    {
        uint VANILLA_FIRST_MAP_NAME_MESSAGE_ID();
        uint VANILLA_FIRST_MAP_DESC_MESSAGE_ID();
        FreeSpaceManager getFreeSpaceManager();
        UInt32 MAP_SWITCH_PARAM_ADDR_MAGMAGEDDON();
        UInt32 MAP_SWITCH_PARAM_ADDR_COLLOSUS();
        UInt32 MAP_SWITCH_PARAM_ADDR_OBSERVATORY();
        UInt32 MAP_BGSEQUENCE_ADDR_MARIOSTADIUM();
        UInt32 START_MAP_DEFAULTS_TABLE_VIRTUAL();
        UInt32 START_MAP_DATA_TABLE_VIRTUAL();
        UInt32 START_MAP_DESCRIPTION_MSG_TABLE_VIRTUAL();
        UInt32 ROUTINE_GET_MAP_DIFFICULTY_VIRTUAL();
        void writeHackMapDefaultGoalMoneyTable(EndianBinaryWriter stream, Func<uint, int> toFileAddress, Int16 tableSize, UInt32 tableAddr);
        void writeHackExtendedMapDefaultsTable(EndianBinaryWriter stream, Func<uint, int> toFileAddress, Int16 mapDefaultsTableSize, UInt32 mapDefaultsTableAddr);
        UInt32 readMapDefaultsTableAddr(EndianBinaryReader stream, Func<UInt32, int> toFileAddress);
        void writeHackExtendedMapSettingsTable(EndianBinaryWriter stream, Func<UInt32, int> toFileAddress);
        bool isHackExtendedMapSettingsTable(EndianBinaryReader stream, Func<UInt32, int> toFileAddress);
        void writeHackCustomMapIcons(EndianBinaryWriter stream, Func<UInt32, int> toFileAddress, UInt16 mapIconAddrTableItemCount, UInt32 mapIconAddrTable);
        bool isHackCustomMapIcons(EndianBinaryReader stream, Func<uint, int> toFileAddress);
    }
}
