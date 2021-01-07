using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CustomStreetMapManager
{
    class CliExport : CliCommand
    {
        public CliExport() : base("export") { }
        public override string GetHelp()
        {
            _ = @"
--------------------------------------------------------------------------------
";
            return @"
usage: csmm export [options] <input>

options:
   -v              verbose
   -q              quiet (overrides verbose)

   -d <path>       destination directory
   -a              export all maps
   -i <id1,...>    comma seperated list of ids of the maps to export
   -n <name1,...>  comma seperated list of internal names of the maps to export
   -f              force overwrite existing files
";
        }
        public override async Task Run(string input, Dictionary<string, string> options, ConsoleProgress progress, CancellationToken ct)
        {
            var destination = options.GetValueOrDefault("d", null);
            var idList = options.GetValueOrDefault("i", null);
            var all = options.ContainsKey("a");
            var internalNameList = options.GetValueOrDefault("n", null);
            var overwrite = options.ContainsKey("f");
            if (string.IsNullOrEmpty(idList) && string.IsNullOrEmpty(internalNameList) && !all)
                throw new ArgumentException("No map descriptor to export");
            List<string> ids = new List<string>();
            if (!string.IsNullOrEmpty(idList))
                ids = new List<string>(idList.Split(','));
            List<string> internalNames = new List<string>();
            if (!string.IsNullOrEmpty(internalNameList))
                internalNames = new List<string>(internalNameList.Split(','));
            await Export(input, destination, ids, all, internalNames, overwrite, progress, ct);
        }
        private async Task Export(string input, string destination, List<string> ids, bool all, List<string> internalNames, bool overwrite, ConsoleProgress progress, CancellationToken ct)
        {
            var mapDescriptors = await PatchProcess.Load(input, progress, ct, input);
            await Task.Delay(500);

            var mapDescriptorsToExport = new List<MapDescriptor>();
            for (int i = 0; i < mapDescriptors.Count; i++)
            {
                bool export = false;
                if (all)
                    export = true;
                if (internalNames.Contains(mapDescriptors[i].InternalName))
                    export = true;
                if (ids.Contains(i.ToString()))
                    export = true;
                if (export)
                    mapDescriptorsToExport.Add(mapDescriptors[i]);
            }
            if (mapDescriptorsToExport.Count > 1 && !string.IsNullOrEmpty(Path.GetExtension(destination)))
            {
                throw new ArgumentException("Multiple map descriptors are to be exported, however the given destination is a filename. Use a directory instead.");
            }
            foreach (var mapDescriptor in mapDescriptorsToExport)
            {
                PatchProcess.ExportMd(destination, input, mapDescriptor, overwrite, progress, ct);
            }
            await Task.Delay(500);
        }
    }
}
