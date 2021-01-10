using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CustomStreetMapManager
{
    class CliClose : CliCommand
    {
        public CliClose() : base("close", s, c) { }
        public override string GetHelp()
        {
            _ = @"
--------------------------------------------------------------------------------
";
            return $@"
usage: csmm close {s.ToShortString()} [options]

Cleans up the files and folders which have been created for the patching process

options:
{GetOptionsHelp()}";
        }
        public override async Task Run(string subCommand, string fortuneStreetPath, string configPath, Dictionary<string, string> options, ConsoleProgress progress, CancellationToken ct)
        {
            await Close(fortuneStreetPath, configPath, progress, ct);
        }
        public Task Close(string fortuneStreetPath, string configPath, ConsoleProgress progress, CancellationToken ct)
        {
            if (File.Exists(configPath))
            {
                File.Delete(configPath);
                PatchProcess.CleanCache(fortuneStreetPath);
                progress?.Report("All cleaned up");
            }
            else
            {
                throw new ArgumentException(fortuneStreetPath + " was not open");
            }
            return Task.CompletedTask;
        }
    }
}
