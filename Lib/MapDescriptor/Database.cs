using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSEditor.MapDescriptor
{
    public class Database
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
            table.Columns.Add("Entry", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Rows.Add("ui_menu007_bg101", "p_bg_101", "Trodain Castle");
            table.Rows.Add("ui_menu007_bg109", "p_bg_109", "The Observatory");
            table.Rows.Add("ui_menu007_bg102", "p_bg_102", "Ghost Ship");
            table.Rows.Add("ui_menu007_bg105", "p_bg_105", "Slimenia");
            table.Rows.Add("ui_menu007_bg104", "p_bg_104", "Mt. Magmageddon");
            table.Rows.Add("ui_menu007_bg106", "p_bg_106", "Robbin' Hood Ruins");
            table.Rows.Add("ui_menu007_bg004", "p_bg_004", "Mario Stadium");
            table.Rows.Add("ui_menu007_bg008", "p_bg_008", "Starship Mario");
            table.Rows.Add("ui_menu007_bg002", "p_bg_002", "Mario Circuit");
            table.Rows.Add("ui_menu007_bg001", "p_bg_001", "Yoshi's Island");
            table.Rows.Add("ui_menu007_bg005", "p_bg_005", "Delfino Plaza");
            table.Rows.Add("ui_menu007_bg003", "p_bg_003", "Peach's Castle");
            table.Rows.Add("ui_menu007_bg107", "p_bg_107", "Alefgard");
            table.Rows.Add("ui_menu007_bg006", "p_bg_006", "Super Mario Bros");
            table.Rows.Add("ui_menu007_bg007", "p_bg_007", "Bowser's Castle");
            table.Rows.Add("ui_menu007_bg009", "p_bg_009", "Good Egg Galaxy");
            table.Rows.Add("ui_menu007_bg103", "p_bg_103", "The Colossus");
            table.Rows.Add("ui_menu007_bg108", "p_bg_108", "Alltrades Abbey");
            return table;
        }
    }
}