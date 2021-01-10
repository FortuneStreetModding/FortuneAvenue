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
        public CliSave() : base("save", d, c, s, w, f) { }
        public override string GetHelp()
        {
            _ = @"
--------------------------------------------------------------------------------
";
            return $@"
usage: csmm save {s.ToShortString()} [options]

options:
{GetOptionsHelp()}";
        }
        public override async Task Run(string subCommand, string fortuneStreetPath, string configPath, Dictionary<string, string> options, ConsoleProgress progress, CancellationToken ct)
        {
            string destination = options.GetValueOrDefault("d", PatchProcess.GetCachePath(fortuneStreetPath));
            if (Path.GetFullPath(destination) == Path.GetFullPath(fortuneStreetPath) && !options.ContainsKey("f"))
            {
                throw new ArgumentException("This operation would overwrite the existing game disc directory at " + destination + ". Provide the switch -f to overwrite.");
            }
            var patchWiimmfi = GetBoolParameter(options, "w");
            await Save(fortuneStreetPath, destination, configPath, patchWiimmfi, progress, ct);
        }
        private async Task Save(string fortuneStreetPath, string destination, string config, Optional<bool> wiimmfi, ConsoleProgress progress, CancellationToken ct)
        {
            progress.Report("Saving at " + Path.GetFullPath(destination));
            var mapDescriptors = await PatchProcess.Open(fortuneStreetPath, progress, ct);

            await Task.Delay(500, ct);

            Configuration.Load(config, mapDescriptors, progress, ct);

            await PatchProcess.Save(fortuneStreetPath, destination, mapDescriptors, wiimmfi.OrElse(true), progress, ct);

            await Task.Delay(500, ct);
        }
    }
}
