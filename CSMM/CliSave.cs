using MiscUtil.Conversion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CustomStreetMapManager
{
    class CliSave : CliCommand
    {
        public CliSave() : base("save") { }
        public override string GetHelp()
        {
            _ = @"
--------------------------------------------------------------------------------
";
            return @"
usage: csmm save [options] <input>

options:
   -v              verbose
   -q              quiet (overrides verbose)

   -d <path>       destination directory or image file
   -c <config>     configuration file (default: config.csv)
   -w <true,false> patch wiimmfi (default: true)
";
        }
        public override async Task Run(string input, Dictionary<string, string> options, ConsoleProgress progress, CancellationToken ct)
        {
            var destination = options.GetValueOrDefault("d", input);
            var configuration = options.GetValueOrDefault("c", Path.Combine(Directory.GetCurrentDirectory(), "config.csv"));
            var patchWiimmfi = GetBoolParameter(options, "w");
            await Save(input, destination, configuration, patchWiimmfi, progress, ct);
        }
        private async Task Save(string input, string destination, string config, Optional<bool> wiimmfi, ConsoleProgress progress, CancellationToken ct)
        {
            var mapDescriptors = await PatchProcess.Open(input, progress, ct, input);

            await Task.Delay(500);

            Configuration.Load(config, mapDescriptors, progress, ct);

            await PatchProcess.Save(input, destination, mapDescriptors, wiimmfi.OrElse(true), progress, ct, input);

            await Task.Delay(500);
        }
    }
}
