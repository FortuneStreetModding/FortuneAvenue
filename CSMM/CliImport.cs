using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CustomStreetMapManager
{
    class CliImport : CliCommand
    {
        public CliImport() : base("import") { }
        public override string GetHelp()
        {
            _ = @"
--------------------------------------------------------------------------------
";
            return @"
usage: csmm import [options] <input>

options:
   -v              verbose
   -q              quiet (overrides verbose)

   -d <path>       extracted fortune street game directory
   -i <0,...>      map id
   -m <0,1>        mapset
   -z <0,1,2>      zone
   -o <0,...>      order
";
        }
        public override async Task Run(string input, Dictionary<string, string> options, ConsoleProgress progress, CancellationToken ct)
        {
            var destination = options.GetValueOrDefault("d", null);
            int mapId = int.Parse(options.GetValueOrDefault("i", null));
            sbyte mapSet = sbyte.Parse(options.GetValueOrDefault("m", null));
            sbyte zone = sbyte.Parse(options.GetValueOrDefault("z", null));
            sbyte order = sbyte.Parse(options.GetValueOrDefault("o", null));
            bool tutorial = options.ContainsKey("t");
            await Import(input, destination, mapId, mapSet, zone, order, tutorial, progress, ct);
        }
        private async Task Import(string inputMd, string destination, int mapId, sbyte mapSet, sbyte zone, sbyte order, bool tutorial, ConsoleProgress progress, CancellationToken ct)
        {
            var mapDescriptors = await PatchProcess.Load(destination, progress, ct, destination);

            await Task.Delay(500);

            MapDescriptor md = PatchProcess.ImportMd(inputMd, progress, ct);

            mapDescriptors[mapId].setFromImport(md);
            mapDescriptors[mapId].MapSet = mapSet;
            mapDescriptors[mapId].Zone = zone;
            mapDescriptors[mapId].Order = order;
            mapDescriptors[mapId].IsPracticeBoard = tutorial;

            await PatchProcess.Save(destination, destination, mapDescriptors, false, progress, ct, destination);

            await Task.Delay(500);
        }
    }
}
