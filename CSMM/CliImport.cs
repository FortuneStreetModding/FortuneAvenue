using MiscUtil.Conversion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CustomStreetMapManager
{
    class CliImport : CliCommand
    {
        public CliImport() : base("import", c, i, m, z, o, s, p) { }
        public override string GetHelp()
        {
            _ = @"
--------------------------------------------------------------------------------
";
            return $@"
usage: csmm import {s.ToShortString()} [options] <mdFilePath>

if -i <0|...> is provided, the map with the given id is replaced/updated
if -i <0|...> is not provided, a new map will be added

options:
{GetOptionsHelp()}";
        }
        public override Task Run(string inputMdFile, string fortuneStreetPath, string configPath, Dictionary<string, string> options, ConsoleProgress progress, CancellationToken ct)
        {
            var mapId = GetIntParameter(options, "i");
            var mapSet = GetSbyteParameter(options, "m");
            var zone = GetSbyteParameter(options, "z");
            var order = GetSbyteParameter(options, "o");
            var practiceBoard = GetBoolParameter(options, "p");
            Configuration.Import(configPath, inputMdFile, mapId, mapSet, zone, order, practiceBoard, progress, ct);
            return Task.CompletedTask;
        }
    }
}
