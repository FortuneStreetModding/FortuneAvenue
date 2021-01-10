using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CustomStreetMapManager
{
    class CliOpen : CliCommand
    {
        public CliOpen() : base("open", c, s, f) { }
        public override string GetHelp()
        {
            _ = @"
--------------------------------------------------------------------------------
";
            return $@"
usage: csmm open {s.ToShortString()} [options]

This command does three things:
(1) If the source is a game disc image (*.iso,*.wbfs,*.ciso,...) it will
     be extracted first.
(2) The extracted game disc directory is read
(3) A config.csv file is created in the root of the extracted directory which 
     contains information about the current map configuration. This file will be
     used by further commands.

options:
{GetOptionsHelp()}";
        }
        public override async Task Run(string subCommand, string fortuneStreetPath, string configPath, Dictionary<string, string> options, ConsoleProgress progress, CancellationToken ct)
        {
            if (File.Exists(configPath) && !options.ContainsKey("f"))
            {
                throw new ArgumentException("The source " + fortuneStreetPath + " is already open. Use the switch -f to close and reopen it losing pending changes.");
            }
            var mapDescriptors = await PatchProcess.Open(fortuneStreetPath, progress, ct);
            Configuration.Save(configPath, mapDescriptors, progress, ct);
            progress?.Report("Opened " + fortuneStreetPath);
        }
    }
}
