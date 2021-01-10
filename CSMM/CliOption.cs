using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomStreetMapManager
{
    public class CliOption : IComparable<CliOption>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ArgumentName { get; set; }
        public string DefaultArgumentValueDescription { get; set; }

        public CliOption(string name, string description, string argumentName = null, string defaultArgumentValueDescription = null)
        {
            Name = name;
            Description = description;
            ArgumentName = argumentName;
            DefaultArgumentValueDescription = defaultArgumentValueDescription;
        }

        public override string ToString()
        {
            var str = new StringBuilder(string.Format("   -{0} {1,-13} ", Name, ArgumentName == null ? "" : "<" + ArgumentName + ">"));
            var desc = Wrap(Description, 80);
            for (int i = 0; i < desc.Length; i++)
            {
                str.Append(desc[i]);
                if (i != desc.Length - 1)
                {
                    str.AppendLine();
                    str.Append(new string(' ', 21));
                }
            }
            if (DefaultArgumentValueDescription != null)
            {
                str.AppendLine();
                str.Append(new string(' ', 21));
                str.Append("(default: ");
                desc = Wrap(DefaultArgumentValueDescription, 60);
                for (int i = 0; i < desc.Length; i++)
                {
                    str.Append(desc[i]);
                    if (i != desc.Length - 1)
                    {
                        str.AppendLine();
                        str.Append(new string(' ', 31));
                    }
                }
                str.Append(')');
            }
            return str.ToString();
        }

        public string ToShortString()
        {
            return string.Format("-{0} {1}", Name, ArgumentName == null ? "" : "<" + ArgumentName + ">");
        }

        public bool HasArgument()
        {
            return ArgumentName != null;
        }

        private string[] Wrap(string text, int max)
        {
            var charCount = 0;
            var lines = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return lines.GroupBy(w => (charCount += (((charCount % max) + w.Length + 1 >= max)
                            ? max - (charCount % max) : 0) + w.Length + 1) / max)
                        .Select(g => string.Join(" ", g.ToArray()))
                        .ToArray();
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Name == ((CliOption)obj).Name;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public int CompareTo(CliOption? other)
        {
            if (other == null)
                return 1;
            else
                return this.Name.CompareTo(other.Name);
        }
    }
}
