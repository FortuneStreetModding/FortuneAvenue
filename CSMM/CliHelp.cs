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

    csmm open   -s ""Fortune Street[ST7E01].iso""
    csmm export -s ""Fortune Street[ST7E01].iso"" -n ""Yosshi Island"" -d ""Map Descriptors""

Use Case 2: Extract all map descriptor files to the folder ""Map Descriptors"" and automatically overwrite existing files

    csmm open   -s ""Fortune Street[ST7E01].iso""
    csmm export -s ""Fortune Street[ST7E01].iso"" -a -d ""Map Descriptors"" -f

Use Case 3: Add a new map (map will be added to Standard Mode, Special Tour to the end) to a wbfs file

    csmm open   -s ""Fortune Street[ST7E01].wbfs""
    csmm import -s ""Fortune Street[ST7E01].wbfs"" ""MyMap.md""
    csmm save   -s ""Fortune Street[ST7E01].wbfs"" -d ""My Custom Street[ST7E02].wbfs""
    csmm close  -s ""Fortune Street[ST7E01].wbfs""

Use Case 4: Replace an existing map (in this case: yoshis island) to an iso file

    csmm open   -s ""Fortune Street[ST7E01].iso""
    csmm import -s ""Fortune Street[ST7E01].iso"" -i 0 ""MyMap.md""
    csmm save   -s ""Fortune Street[ST7E01].iso"" -d ""My Custom Street[ST7E02].wbfs""
    csmm close  -s ""Fortune Street[ST7E01].iso""

Use Case 5: Add a map to mapset 1 and mapset 2 of zone 1 to an iso file and save to file system

    csmm open   -s ""Fortune Street[ST7E01].iso""
    csmm import -s ""Fortune Street[ST7E01].iso"" -m 0 -z 1 ""MyMap.md""
    csmm import -s ""Fortune Street[ST7E01].iso"" -m 1 -z 1 ""MyMap.md""
    csmm save   -s ""Fortune Street[ST7E01].iso"" -d ""Extracted Fortune Street Folder""
    csmm close  -s ""Fortune Street[ST7E01].iso""

Hint: If in the directory of csmm only one iso/wbfs or extracted game directory of fortune street is available, then the 
      -s parameter can be omitted. E.g.:

    csmm open
    csmm import -m 0 -z 1 ""MyMap.md""
    csmm import -m 1 -z 1 ""MyMap.md""
    csmm save   -d ""Custom Street[ST7E02]""
    csmm close
";
        }
        public override Task Run(string subCommand, string fortuneStreetPath, string configPath, Dictionary<string, string> options, ConsoleProgress progress, CancellationToken ct)
        {
            bool printGenericHelp = true;
            foreach (var command in Cli.GetCommandList())
            {
                if (subCommand == command.Name)
                {
                    printGenericHelp = false;
                    Console.WriteLine(command.GetHelp());
                }
            }
            if (printGenericHelp)
            {
                Console.WriteLine(GetHelp());
            }
            return Task.CompletedTask;
        }
    }
}
