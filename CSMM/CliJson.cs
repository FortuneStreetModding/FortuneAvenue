using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CustomStreetMapManager
{
    class CliJson : CliCommand
    {
        public CliJson() : base("json", d, s) { }
        public override string GetHelp()
        {
            _ = @"
--------------------------------------------------------------------------------
";
            return $@"
usage: csmm json {s.ToShortString()} [options]

options:
{GetOptionsHelp()}";
        }
        public override async Task Run(string subCommand, string fortuneStreetPath, string configPath, Dictionary<string, string> options, ConsoleProgress progress, CancellationToken ct)
        {
            var destination = options.GetValueOrDefault("d", null);
            await Json(fortuneStreetPath, configPath, destination, progress, ct);
        }
        private async Task Json(string fortuneStreetPath, string configPath, string destination, ConsoleProgress progress, CancellationToken ct)
        {
            var mapDescriptors = await PatchProcess.Open(fortuneStreetPath, progress, ct);
            await Task.Delay(500, ct);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            // Configuration.Load(configPath, mapDescriptors, progress, ct);
            progress?.Report(JsonSerializer.Serialize(mapDescriptors, options));
            if (!string.IsNullOrEmpty(destination))
                File.WriteAllBytes(destination, JsonSerializer.SerializeToUtf8Bytes(mapDescriptors, options));
        }
    }
}
