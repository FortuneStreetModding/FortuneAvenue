using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CustomStreetMapManager
{
    class CliHelp : CliCommand
    {
        public CliHelp() : base("help") { }
        public override string GetHelp()
        {
            _ = @"
--------------------------------------------------------------------------------
";
            return @"
Use Case 1: Extract the standard yoshi map descriptor file to the folder ""Map Descriptors""

    csmm load ""Fortune Street[ST7E01].iso"" -d ""Extracted Fortune Street Folder""
    csmm export ""Extracted Fortune Street Folder"" -n ""Yosshi Island"" -d ""Map Descriptors""

Use Case 2: Extract all map descriptor files to the folder ""Map Descriptors"" and automatically overwrite existing files

    csmm load ""Fortune Street[ST7E01].iso"" -d ""Extracted Fortune Street Folder""
    csmm export ""Extracted Fortune Street Folder"" -a -d ""Map Descriptors"" -f

Use Case 3: Add a new map (map will be added to Standard Mode, Special Tour to the end) to a wbfs file

    csmm load ""Fortune Street[ST7E01].wbfs"" -d ""Extracted Fortune Street Folder""
    csmm import ""MyMap.md""
    csmm save ""Extracted Fortune Street Folder"" -d ""My Custom Street[ST7E02].wbfs""

Use Case 4: Replace an existing map (map will replace yoshis island) to an iso file

    csmm load ""Fortune Street[ST7E01].iso"" -d ""Extracted Fortune Street Folder""
    csmm import ""MyMap.md"" -i 0
    csmm save ""Extracted Fortune Street Folder"" -d ""My Custom Street[ST7E02].wbfs""

Use Case 5: Add a map to mapset 1 and mapset 2 of zone 1 to an iso file and save to file system

    csmm load ""Fortune Street[ST7E01].iso"" -d ""Extracted Fortune Street Folder""
    csmm import ""MyMap.md"" -m 0 -z 1
    csmm import ""MyMap.md"" -m 1 -z 1
    csmm save ""Extracted Fortune Street Folder""
";
        }
        public override Task Run(string input, Dictionary<string, string> options, ConsoleProgress progress, CancellationToken ct)
        {
            Console.WriteLine(GetHelp());
            return Task.CompletedTask;
        }
    }
}
