using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSEditor.MapDescriptor
{
    class ST7E01 : ST7_Interface
    {
        public uint VANILLA_FIRST_MAP_DESC_MESSAGE_ID() { return 4416; }
        public uint VANILLA_FIRST_MAP_NAME_MESSAGE_ID() { return 5433; }
        public uint MAP_BGSEQUENCE_ADDR_MARIOSTADIUM() { throw new NotImplementedException(); }
        public uint MAP_SWITCH_PARAM_ADDR_COLLOSUS() { throw new NotImplementedException(); }
        public uint MAP_SWITCH_PARAM_ADDR_MAGMAGEDDON() { throw new NotImplementedException(); }
        public uint MAP_SWITCH_PARAM_ADDR_OBSERVATORY() { throw new NotImplementedException(); }
        public uint START_MAP_DATA_TABLE_VIRTUAL() { throw new NotImplementedException(); }
        public uint START_MAP_DEFAULTS_TABLE_VIRTUAL() { throw new NotImplementedException(); }
        public uint START_MAP_DESCRIPTION_MSG_TABLE_VIRTUAL() { throw new NotImplementedException(); }
        public uint START_VENTURE_CARD_TABLE_VIRTUAL() { throw new NotImplementedException(); }
        // map data string table repurposed as free space
        public uint UNUSED_SPACE_1_START_VIRTUAL() { throw new NotImplementedException(); }
        public uint UNUSED_SPACE_1_END_VIRTUAL() { throw new NotImplementedException(); }
        // unused costume string table 1
        public uint UNUSED_SPACE_2_START_VIRTUAL() { throw new NotImplementedException(); }
        public uint UNUSED_SPACE_2_END_VIRTUAL() { throw new NotImplementedException(); }
        // unused costume string table 2
        public uint UNUSED_SPACE_3_END_VIRTUAL() { throw new NotImplementedException(); }
        public uint UNUSED_SPACE_3_START_VIRTUAL() { throw new NotImplementedException(); }
        // unused costume string table 3
        public uint UNUSED_SPACE_4_START_VIRTUAL() { throw new NotImplementedException(); }
        public uint UNUSED_SPACE_4_END_VIRTUAL() { throw new NotImplementedException(); }

        public void writeHackExtendedMapDescriptions(EndianBinaryWriter stream, Func<UInt32, int> toFileAddress)
        {
            throw new NotImplementedException();
        }
        public bool isHackExtendedMapDescriptions(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            throw new NotImplementedException();
        }
        public void writeHackCustomMapIcons(EndianBinaryWriter stream, Func<UInt32, int> toFileAddress, Int16 mapIconAddrTableItemCount, UInt32 mapIconAddrTable)
        {
            throw new NotImplementedException();
        }
        public bool isHackCustomMapIcons(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            throw new NotImplementedException();
        }

        public void writeHackExtendedVentureCardTable(EndianBinaryWriter stream, Func<uint, int> toFileAddress, Int16 ventureCardTableCount, UInt32 ventureCardTableAddr)
        {
            throw new NotImplementedException();
        }

        public bool isHackExtendedVentureCardTable(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            throw new NotImplementedException();
        }

        public void writeHackExtendedMapDescriptionTable(EndianBinaryWriter stream, Func<uint, int> toFileAddress)
        {
            throw new NotImplementedException();
        }

        public bool isHackExtendedMapDescriptionTable(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            throw new NotImplementedException();
        }

        public void writeHackExtendedMapDefaultsTable(EndianBinaryWriter stream, Func<uint, int> toFileAddress)
        {
            throw new NotImplementedException();
        }

        public bool isHackExtendedMapDefaultsTable(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            throw new NotImplementedException();
        }

        public void writeHackExtendedMapSettingsTable(EndianBinaryWriter stream, Func<uint, int> toFileAddress)
        {
            throw new NotImplementedException();
        }

        public bool isHackExtendedMapSettingsTable(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            throw new NotImplementedException();
        }
    }
}
