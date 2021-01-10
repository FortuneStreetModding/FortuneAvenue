using FSEditor.Exceptions;
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
        public CliExport() : base("export", s, d, a, i, n, f, c) { }
        public override string GetHelp()
        {
            _ = @"
--------------------------------------------------------------------------------
";
            return $@"
usage: csmm export {s.ToShortString()} [options] 

provide -i with a comma separated list of map ids to export multiple maps at once

options:
{GetOptionsHelp()}";
        }
        public override async Task Run(string subCommand, string fortuneStreetPath, string configPath, Dictionary<string, string> options, ConsoleProgress progress, CancellationToken ct)
        {
            var destination = options.GetValueOrDefault("d", Directory.GetCurrentDirectory());
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
            await Export(fortuneStreetPath, destination, ids, all, internalNames, overwrite, progress, ct);
        }
        private async Task Export(string fortuneStreetPath, string destination, List<string> ids, bool all, List<string> internalNames, bool overwrite, ConsoleProgress progress, CancellationToken ct)
        {
            var mapDescriptors = await PatchProcess.Open(fortuneStreetPath, progress, ct);
            await Task.Delay(500, ct);

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
            if (mapDescriptorsToExport.Count > 1 && !Directory.Exists(destination) && !string.IsNullOrEmpty(Path.GetExtension(destination)))
            {
                throw new ArgumentException("Multiple map descriptors are to be exported, however the given destination is a filename. Use a directory instead.");
            }
            foreach (var mapDescriptor in mapDescriptorsToExport)
            {
                try
                {
                    PatchProcess.ExportMd(destination, PatchProcess.GetCachePath(fortuneStreetPath), mapDescriptor, overwrite, progress, ct);
                }
                catch (FileAlreadyExistException)
                {
                    progress.Report("Use the switch -f to overwrite already existing files.");
                    throw;
                }
            }
            await Task.Delay(500, ct);
        }
    }
}
