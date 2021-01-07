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

   -d path         destination directory
   -a              all
   -i id1,...      ids of the maps to export
   -n name1,...    internal names of the map to export
   -o              overwrite existing files
";
        }
        public override async Task Run(string input, Dictionary<string, string> options, ConsoleProgress progress, CancellationToken ct)
        {
            await Export(input, options.GetValueOrDefault("d", null), options.GetValueOrDefault("i", null), options.ContainsKey("a"), options.GetValueOrDefault("n", null), options.ContainsKey("o"), progress, ct);
        }
        private async Task Export(string input, string destination, string idList, bool all, string internalNameList, bool overwrite, ConsoleProgress progress, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(idList) && string.IsNullOrEmpty(internalNameList) && !all)
                throw new ArgumentException("No map descriptor to export");

            var mapDescriptors = await PatchProcess.Load(input, progress, ct, input);
            await Task.Delay(500);

            List<string> ids = new List<string>();
            List<string> internalNames = new List<string>();
            if (!string.IsNullOrEmpty(idList))
                ids = new List<string>(idList.Split(','));
            if (!string.IsNullOrEmpty(internalNameList))
                internalNames = new List<string>(internalNameList.Split(','));

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
