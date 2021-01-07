using MiscUtil.Conversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomStreetMapManager
{
    abstract class CliCommand
    {
        public string Name { get; set; }
        protected CliCommand(string name)
        {
            Name = name;
        }
        public abstract string GetHelp();
        public abstract Task Run(string input, Dictionary<string, string> options, ConsoleProgress progress, CancellationToken ct);
        protected Optional<int> GetIntParameter(Dictionary<string, string> options, string parameter)
        {
            if (!options.ContainsKey(parameter))
                return Optional<int>.CreateEmpty();
            return Optional<int>.Create(int.Parse(options[parameter]));
        }
        protected Optional<sbyte> GetSbyteParameter(Dictionary<string, string> options, string parameter)
        {
            if (!options.ContainsKey(parameter))
                return Optional<sbyte>.CreateEmpty();
            return Optional<sbyte>.Create(sbyte.Parse(options[parameter]));
        }

        protected Optional<bool> GetBoolParameter(Dictionary<string, string> options, string parameter)
        {
            if (!options.ContainsKey(parameter))
                return Optional<bool>.CreateEmpty();
            return Optional<bool>.Create(bool.Parse(options[parameter]));
        }
    }
}
