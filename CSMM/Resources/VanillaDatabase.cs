using MiscUtil.Conversion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomStreetManager
{
    public class VanillaDatabase
    {
        public static DataTable getMapTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Background", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("Bgm Id", typeof(UInt32));
            table.Columns.Add("Map Icon", typeof(string));
            table.Columns.Add("Map Tpl", typeof(string));
            table.Rows.Add( "bg101"  , "Trodain Castle"    , 17 , "p_bg_101" , "ui_menu007_bg101.tpl" );
            table.Rows.Add( "bg109"  , "The Observatory"   , 21 , "p_bg_109" , "ui_menu007_bg109.tpl" );
            table.Rows.Add( "bg102"  , "Ghost Ship"        ,  3 , "p_bg_102" , "ui_menu007_bg102.tpl" );
            table.Rows.Add( "bg105"  , "Slimenia"          ,  6 , "p_bg_105" , "ui_menu007_bg105.tpl" );
            table.Rows.Add( "bg104"  , "Mt. Magmageddon"   ,  5 , "p_bg_104" , "ui_menu007_bg104.tpl" );
            table.Rows.Add( "bg106"  , "Robbin' Hood Ruins",  7 , "p_bg_106" , "ui_menu007_bg106.tpl" );
            table.Rows.Add( "bg004"  , "Mario Stadium"     , 12 , "p_bg_004" , "ui_menu007_bg004.tpl" );
            table.Rows.Add( "bg008"  , "Starship Mario"    , 15 , "p_bg_008" , "ui_menu007_bg008.tpl" );
            table.Rows.Add( "bg002"  , "Mario Circuit"     ,  0 , "p_bg_002" , "ui_menu007_bg002.tpl" );
            table.Rows.Add( "bg001"  , "Yoshi's Island"    , 11 , "p_bg_001" , "ui_menu007_bg001.tpl" );
            table.Rows.Add( "bg005"  , "Delfino Plaza"     , 13 , "p_bg_005" , "ui_menu007_bg005.tpl" );
            table.Rows.Add( "bg003"  , "Peach's Castle"    ,  1 , "p_bg_003" , "ui_menu007_bg003.tpl" );
            table.Rows.Add( "bg107"  , "Alefgard"          ,  9 , "p_bg_107" , "ui_menu007_bg107.tpl" );
            table.Rows.Add( "bg006"  , "Super Mario Bros"  , 14 , "p_bg_006" , "ui_menu007_bg006.tpl" );
            table.Rows.Add( "bg007"  , "Bowser's Castle"   ,  2 , "p_bg_007" , "ui_menu007_bg007.tpl" );
            table.Rows.Add( "bg009"  , "Good Egg Galaxy"   , 16 , "p_bg_009" , "ui_menu007_bg009.tpl" );
            table.Rows.Add( "bg103"  , "The Colossus"      ,  4 , "p_bg_103" , "ui_menu007_bg103.tpl" );
            table.Rows.Add( "bg103_e", "The Colossus Easy" ,  4 , "p_bg_103" , "ui_menu007_bg103.tpl" );
            table.Rows.Add( "bg108"  , "Alltrades Abbey"   , 19 , "p_bg_108" , "ui_menu007_bg108.tpl" );
            table.Rows.Add( "bg901"  , "Practice Board"    , 22 );
            return table;
        }

        public static DataTable getBgmTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Bgm Id", typeof(UInt32));
            table.Columns.Add("Brsar Mario", typeof(UInt32));
            table.Columns.Add("Brsar DragonQuest", typeof(UInt32));
            table.Columns.Add("Filename", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Rows.Add( 17 , 29 , 29 , "29_BGM_MAP_TRODAIN"       , "Trodain Castle"     );
            table.Rows.Add( 21 , 41 , 41 , "37_BGM_MAP_ANGEL"         , "The Observatory"    );
            table.Rows.Add(  3 , 31 , 31 , "30_BGM_MAP_GHOSTSHIP"     , "Ghost Ship"         );
            table.Rows.Add(  6 , 34 , 34 , "33_BGM_MAP_SLABACCA"      , "Slimenia"           );
            table.Rows.Add(  5 , 33 , 33 , "32_BGM_MAP_SINOKAZAN"     , "Mt. Magmageddon"    );
            table.Rows.Add(  7 , 35 , 35 , "34_BGM_MAP_KANDATA"       , "Robbin' Hood Ruins" );
            table.Rows.Add( 12 , 23 , 23 , "23_BGM_MAP_STADIUM"       , "Mario Stadium"      );
            table.Rows.Add( 15 , 27 , 27 , "27_BGM_MAP_STARSHIP"      , "Starship Mario"     );
            table.Rows.Add(  0 , 21 , 21 , "21_BGM_MAP_CIRCUIT"       , "Mario Circuit"      );
            table.Rows.Add( 11 , 20 , 20 , "20_BGM_MAP_YOSHI"         , "Yoshi's Island"     );
            table.Rows.Add( 13 , 24 , 24 , "24_BGM_MAP_DOLPIC"        , "Delfino Plaza"      );
            table.Rows.Add(  1 , 22 , 22 , "22_BGM_MAP_PEACH"         , "Peach's Castle"     );
            table.Rows.Add(  9 , 37 , 37 , "35_BGM_MAP_ALEFGARD"      , "Alefgard"           );
            table.Rows.Add( 14 , 25 , 25 , "25_BGM_MAP_SMB"           , "Super Mario Bros"   );
            table.Rows.Add(  2 , 26 , 26 , "26_BGM_MAP_KOOPA"         , "Bowser's Castle"    );
            table.Rows.Add( 16 , 28 , 28 , "28_BGM_MAP_EGG"           , "Good Egg Galaxy"    );
            table.Rows.Add(  4 , 32 , 32 , "31_BGM_MAP_MAJINZOU"      , "The Colossus"       );
            table.Rows.Add( 19 , 39 , 39 , "36_BGM_MAP_DHAMA"         , "Alltrades Abbey"    );
            table.Rows.Add( 22 ,  5 ,  5 , "05_BGM_MENU"              , "Practice Board"     );
            table.Rows.Add(  8 , 36 , 36 , "34_BGM_MAP_KANDATA_old"   , "Unused"             );
            table.Rows.Add( 10 , 38 , 38 , "35_BGM_MAP_ALEFGARD_old"  , "Unused"             );
            table.Rows.Add( 18 , 30 , 30 , "29_BGM_MAP_TRODAIN_old"   , "Unused"             );
            table.Rows.Add( 20 , 40 , 40 , "36_BGM_MAP_DHAMA_old"     , "Unused"             );
            table.Rows.Add( 23 , 42 , 43 , "38_BGM_GOALPROP_(M/D)"    , "Promotion"          );
            table.Rows.Add( 24 , 10 , 11 , "10_BGM_WINNER_(M/D)"      , "Winner"             );
            table.Rows.Add( 25 , 12 , 12 , "12_BGM_CHANCECARD"        , "Select Chancecard"  );
            table.Rows.Add( 26 , 13 , 13 , "13_BGM_STOCK"             , "Buy/Sell Stock"     );
            table.Rows.Add( 27 , 14 , 14 , "14_BGM_AUCTION"           , "Auction"            );
            table.Rows.Add( 28 , 15 , 16 , "15_BGM_CASINO_SLOT_(M/D)" , "Round The Blocks"   );
            table.Rows.Add( 29 , 17 , 17 , "15_BGM_CASINO_BLOCK"      , "Memory Block"       );
            table.Rows.Add( 30 , 12 , 12 , "12_BGM_CHANCECARD"        , "Dart of Gold"       );
            table.Rows.Add( 31 , 16 , 16 , "16_BGM_CASINO_SLOT_D"     , "Select your Slime"  );
            table.Rows.Add( 32 , 19 , 19 , "19_BGM_CASINO_RACE"       , "Racing Slimes"      );
            table.Rows.Add( 33 ,  0 ,  0 , "01_BGM_TITLE"             , "Title Screen"       );
            table.Rows.Add( 34 ,  5 ,  5 , "05_BGM_MENU"              , "Menu"               );
            table.Rows.Add( 35 ,  3 ,  3 , "04_BGM_SAVELOAD"          , "Save/Load Screen"   );
            table.Rows.Add( 36 ,  4 ,  4 , "04_BGM_SAVELOAD_old"      , "Unused"             );
            table.Rows.Add( 37 ,  6 ,  6 , "06_BGM_WIFI"              , "Wi-Fi"              );
            table.Rows.Add( 38 ,  3 ,  3 , "04_BGM_SAVELOAD"          , "Unknown"            );
            table.Rows.Add( 39 ,  7 ,  7 , "07_BGM_ENDING_M"          , "Credits"            );
            return table;
        }

        public static Optional<string> getMapIconFromVanillaBackground(string background)
        {
            var result = from row in VanillaDatabase.getMapTable().AsEnumerable()
                         where row.Field<string>("Background") == background
                         select row;
            if (result.Any())
                try { return Optional<string>.Create(result.Single().Field<string>("Map Icon")); } catch (IndexOutOfRangeException e) { }
            return Optional<string>.CreateEmpty();
        }

        public static Optional<UInt32> getBgmIdFromVanillaBackground(string background)
        {
            var result = from row in VanillaDatabase.getMapTable().AsEnumerable()
                         where row.Field<string>("Background") == background
                         select row;
            if (result.Any())
                try { return Optional<UInt32>.Create(result.Single().Field<UInt32>("Bgm Id")); } catch (IndexOutOfRangeException e) { }
            return Optional<UInt32>.CreateEmpty();
        }

        public static Optional<string> getVanillaTpl(string mapIcon)
        {
            var result = from row in VanillaDatabase.getMapTable().AsEnumerable()
                         where row.Field<string>("Map Icon") == mapIcon
                         select row;
            if (result.Any())
                try { return Optional<string>.Create(result.Single().Field<string>("Map Tpl")); } catch (IndexOutOfRangeException e) { }
            return Optional<string>.CreateEmpty();
        }

        public static Dictionary<string, string> getVanillaMapIconToTplNameDictionary()
        {
            var dict = new Dictionary<string, string>();
            foreach (var row in getMapTable().Rows)
            {
                var dataRow = (DataRow)row;
                dict.Add(dataRow.Field<string>("Map Icon"), dataRow.Field<string>("Map Tpl"));
            }
            return dict;
        }
    }
}