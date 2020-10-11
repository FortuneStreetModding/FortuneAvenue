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
        public static DataTable getBackgroundTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Background", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Rows.Add("bg101", "Trodain Castle");
            table.Rows.Add("bg109", "The Observatory");
            table.Rows.Add("bg102", "Ghost Ship");
            table.Rows.Add("bg105", "Slimenia");
            table.Rows.Add("bg104", "Mt. Magmageddon");
            table.Rows.Add("bg106", "Robbin' Hood Ruins");
            table.Rows.Add("bg004", "Mario Stadium");
            table.Rows.Add("bg008", "Starship Mario");
            table.Rows.Add("bg002", "Mario Circuit");
            table.Rows.Add("bg001", "Yoshi's Island");
            table.Rows.Add("bg005", "Delfino Plaza");
            table.Rows.Add("bg003", "Peach's Castle");
            table.Rows.Add("bg107", "Alefgard");
            table.Rows.Add("bg006", "Super Mario Bros");
            table.Rows.Add("bg007", "Bowser's Castle");
            table.Rows.Add("bg009", "Good Egg Galaxy");
            table.Rows.Add("bg103", "The Colossus");
            table.Rows.Add("bg103_e", "The Colossus Easy");
            table.Rows.Add("bg108", "Alltrades Abbey");
            table.Rows.Add("bg901", "Practice Board");
            return table;
        }

        public static DataTable getMapIconTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Map Icon", typeof(string));
            table.Columns.Add("Map Tpl", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Rows.Add("p_bg_101", "ui_menu007_bg101.tpl", "Trodain Castle");
            table.Rows.Add("p_bg_109", "ui_menu007_bg109.tpl", "The Observatory");
            table.Rows.Add("p_bg_102", "ui_menu007_bg102.tpl", "Ghost Ship");
            table.Rows.Add("p_bg_105", "ui_menu007_bg105.tpl", "Slimenia");
            table.Rows.Add("p_bg_104", "ui_menu007_bg104.tpl", "Mt. Magmageddon");
            table.Rows.Add("p_bg_106", "ui_menu007_bg106.tpl", "Robbin' Hood Ruins");
            table.Rows.Add("p_bg_004", "ui_menu007_bg004.tpl", "Mario Stadium");
            table.Rows.Add("p_bg_008", "ui_menu007_bg008.tpl", "Starship Mario");
            table.Rows.Add("p_bg_002", "ui_menu007_bg002.tpl", "Mario Circuit");
            table.Rows.Add("p_bg_001", "ui_menu007_bg001.tpl", "Yoshi's Island");
            table.Rows.Add("p_bg_005", "ui_menu007_bg005.tpl", "Delfino Plaza");
            table.Rows.Add("p_bg_003", "ui_menu007_bg003.tpl", "Peach's Castle");
            table.Rows.Add("p_bg_107", "ui_menu007_bg107.tpl", "Alefgard");
            table.Rows.Add("p_bg_006", "ui_menu007_bg006.tpl", "Super Mario Bros");
            table.Rows.Add("p_bg_007", "ui_menu007_bg007.tpl", "Bowser's Castle");
            table.Rows.Add("p_bg_009", "ui_menu007_bg009.tpl", "Good Egg Galaxy");
            table.Rows.Add("p_bg_103", "ui_menu007_bg103.tpl", "The Colossus");
            table.Rows.Add("p_bg_108", "ui_menu007_bg108.tpl", "Alltrades Abbey");
            return table;
        }

        public static string getVanillaTpl(string mapIcon)
        {
            var result = from row in VanillaDatabase.getMapIconTable().AsEnumerable()
                         where row.Field<string>("Map Icon") == mapIcon
                         select row;
            if (result.Any())
                return result.Single().Field<string>("Map Tpl");
            return null;
        }

        public static Dictionary<string, string> getMapIconToTplNameDictionary()
        {
            var dict = new Dictionary<string, string>();
            foreach(var row in getMapIconTable().Rows)
            {
                var dataRow = (DataRow)row;
                dict.Add(dataRow.Field<string>("Map Icon"), dataRow.Field<string>("Map Tpl"));
            }
            return dict;
        }
    }
}