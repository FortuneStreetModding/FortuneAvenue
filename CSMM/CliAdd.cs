using MiscUtil.Conversion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CustomStreetMapManager
{
    class CliAdd : CliCommand
    {
        public CliAdd() : base("add") { }
        public override string GetHelp()
        {
            _ = @"
--------------------------------------------------------------------------------
";
            return @"
usage: csmm add [options] <input>

options:
   -v              verbose
   -q              quiet (overrides verbose)

   -c <path>       configuration file
   -i <0,...>      map id
   -m <0,1>        mapset
   -z <0,1,2>      zone
   -o <0,...>      order
   -t <true,false> tutorial map
";
        }
        public override async Task Run(string input, Dictionary<string, string> options, ConsoleProgress progress, CancellationToken ct)
        {
            var configuration = options.GetValueOrDefault("c", Path.Combine(Directory.GetCurrentDirectory(), "config.csv"));
            await Add(input, configuration, GetIntParameter(options, "i"), GetSbyteParameter(options, "m"), GetSbyteParameter(options, "z"), GetSbyteParameter(options, "o"), GetBoolParameter(options, "t"), progress, ct);
        }
        private async Task Add(string inputMd, string configuration, Optional<int> mapId, Optional<sbyte> mapSet, Optional<sbyte> zone, Optional<sbyte> order, Optional<bool> tutorial, ConsoleProgress progress, CancellationToken ct)
        {
            Configuration.Add(configuration, inputMd, mapId, mapSet, zone, order, tutorial, progress, ct);
            await Task.Delay(500);
        }
    }
}
