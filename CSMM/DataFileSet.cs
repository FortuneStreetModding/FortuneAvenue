using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomStreetManager
{
    public class DataFileSet
    {
        public string main_dol
        { get; set; }
        public readonly Dictionary<string, string> ui_message_csv = new Dictionary<string, string>();
        public string param_folder
        { get; set; }
        public readonly Dictionary<string, string> game_sequence_arc = new Dictionary<string, string>();
        public readonly Dictionary<string, string> game_sequence_wifi_arc = new Dictionary<string, string>();
    }
}
