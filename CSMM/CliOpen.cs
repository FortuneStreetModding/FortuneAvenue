using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CustomStreetMapManager
{
    class CliOpen : CliCommand
    {
        public CliOpen() : base("open") { }
        public override string GetHelp()
        {
            _ = @"
--------------------------------------------------------------------------------
";
            return @"
usage: csmm open [options] <input>

options:
   -v              verbose
   -q              quiet (overrides verbose)

   -d <path>       destination
   -c <path>       configuration file (default: config.csv)
";
        }
        public override async Task Run(string input, Dictionary<string, string> options, ConsoleProgress progress, CancellationToken ct)
        {
            var destination = options.GetValueOrDefault("d", null);
            var configuration = options.GetValueOrDefault("c", Path.Combine(Directory.GetCurrentDirectory(), "config.csv"));
            await Open(input, destination, configuration, progress, ct);
        }
        public async Task Open(string input, string destination, string configuration, ConsoleProgress progress, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(destination))
                throw new ArgumentException("Destination not set");
            if (File.Exists(destination))
                throw new ArgumentException("Destination must be a directory");
            var mapDescriptors = await PatchProcess.Open(input, progress, ct, destination);
            Configuration.Save(configuration, mapDescriptors, progress, ct);
            await Task.Delay(500);
        }
    }
}
