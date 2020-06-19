using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FSEditor.MapDescriptor
{
    public class UI_Message
    {
        SortedDictionary<uint, string> map = new SortedDictionary<uint, string>();
        public UI_Message(string fileName)
        {
            string[] lines = File.ReadAllLines(fileName);
            for (int i = 0; i < lines.Length; i += 1)
            {
                string line = lines[i];
                string[] columns = line.Split(',');
                uint key = uint.Parse(columns[0].Trim());
                string value = columns[1].Trim().Replace("\"", "");
                map.Add(key, value);
            }
        }

        public string get(uint key)
        {
            return map[key];
        }

        public void set(uint key, string value)
        {
            map[key] = value;
        }
    }
}
