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
    }
}