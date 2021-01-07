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
    }
}
