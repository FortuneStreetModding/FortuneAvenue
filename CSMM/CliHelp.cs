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
Use Case 1: Add a new map (map will be added to Standard Mode, Special Tour to the end) to a wbfs file

    csmm load ""Fortune Street[ST7E01].wbfs"" -d ""Extracted Fortune Street Folder""
    csmm import ""MyMap.md""
    csmm save ""Extracted Fortune Street Folder"" -d ""My Custom Street[ST7E02].wbfs""

Use Case 2: Replace an existing map (map will replace yoshis island) to an iso file

    csmm load ""Fortune Street[ST7E01].iso"" -d ""Extracted Fortune Street Folder""
    csmm import ""MyMap.md"" -i 0
    csmm save ""Extracted Fortune Street Folder"" -d ""My Custom Street[ST7E02].wbfs""

Use Case 3: Add a map to mapset 1 and mapset 2 of zone 1 to an iso file and save to directory

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
