using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CustomStreetMapManager
{
    public class UI_Message
    {
        SortedDictionary<uint, string> map = new SortedDictionary<uint, string>();
        private string locale;

        public UI_Message(string fileName, string locale)
        {
            string[] lines = File.ReadAllLines(fileName);
            for (int i = 0; i < lines.Length; i += 1)
            {
                string line = lines[i];
                string[] columns = line.Split(new[] { ',' }, 2);
                uint key = uint.Parse(columns[0].Trim());
                string value = columns[1].Trim().Replace("\"", "");
                map.Add(key, value);
            }
            this.locale = locale;
        }

        public SortedDictionary<uint, string> getMap()
        {
            return map;
        }

        public string get(uint key)
        {
            return map[key];
        }

        public bool contains(uint key)
        {
            return map.ContainsKey(key);
        }
        /**
         * Get a free key in this maps by iterating until we find a free key
         */
        public uint freeKey()
        {
            uint i = 0;
            while (map.ContainsKey(i))
            {
                i++;
            }
            return i;
        }
        public void removeKey(uint key)
        {
            map.Remove(key);
        }

        public void add(uint freeKey, string v)
        {
            map.Add(freeKey, v);
        }

        public void set(uint freeKey, string v)
        {
            map[freeKey] = v;
        }

        public void writeToFile(string filename)
        {
            using (StreamWriter file = new StreamWriter(filename))
            {
                foreach (var entry in map)
                {
                    file.WriteLine("{0},\"{1}\"", entry.Key, entry.Value);
                }
            }
        }
    }
}
