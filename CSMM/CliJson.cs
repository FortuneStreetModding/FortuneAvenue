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
        public CliJson() : base("json") { }
        public override string GetHelp()
        {
            _ = @"
--------------------------------------------------------------------------------
";
            return @"
usage: csmm json [options] <input>

options:
   -v              verbose
   -q              quiet (overrides verbose)

   -d path         destination file
   -p              print to console
";
        }
        public override async Task Run(string input, Dictionary<string, string> options, ConsoleProgress progress, CancellationToken ct)
        {
            var print = options.ContainsKey("p");
            var destination = options.GetValueOrDefault("d", null);
            if (!print && string.IsNullOrEmpty(destination))
                throw new ArgumentException("Neither destination nor print is set. Nothing to do.");
            await Json(input, destination, print, progress, ct);
        }
        private async Task Json(string input, string destination, bool print, ConsoleProgress progress, CancellationToken ct)
        {
            var mapDescriptors = await PatchProcess.Load(input, progress, ct);
            await Task.Delay(500);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            if (print)
                Console.WriteLine(JsonSerializer.Serialize(mapDescriptors, options));
            if (!string.IsNullOrEmpty(destination))
                File.WriteAllBytes(destination, JsonSerializer.SerializeToUtf8Bytes(mapDescriptors, options));
        }
    }
}
