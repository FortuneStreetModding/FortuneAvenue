﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CustomStreetMapManager
{
    class CSMM
    {

        static async Task Main(string[] args)
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                var ct = cancellationTokenSource.Token;
                Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) =>
                {
                    // prevent terminating the application instantly if user requests it -> only once user requests twice
                    if (!cancellationTokenSource.IsCancellationRequested)
                    {
                        Console.Write("Aborting process...");
                        e.Cancel = true;
                        cancellationTokenSource.Cancel();
                    }
                    else
                    {
                        Console.Write("Killing process without cleanup...");
                    }
                };
                if (args.Length == 0)
                {
                    printHelp();
                    return;
                }
                var input = "";
                var options = new Dictionary<string, string>();
                for (int i = 1; i < args.Length; i++)
                {
                    if (args[i].StartsWith("-"))
                    {
                        var option = args[i].Substring(1);
                        if (hasParameter(option))
                        {
                            i++;
                            options.Add(option, args[i]);
                        }
                        else
                        {
                            options.Add(option, "");
                        }
                    }
                    else if (string.IsNullOrEmpty(input))
                    {
                        input = args[i];
                    }
                }
                using (ConsoleProgress progress = new ConsoleProgress(options.ContainsKey("v")))
                {
                    if (args[0].ToLower() == "read")
                    {
                        if (string.IsNullOrEmpty(input))
                            printReadHelp();
                        else
                            await read(input, progress, ct);
                    }
                }
            }
        }

        private IProgress<ProgressInfo> setupProgressReport(ASCIIProgressBar asciiprogress, bool verbose)
        {
            return new Progress<ProgressInfo>(progressInfo =>
            {
                if (!string.IsNullOrEmpty(progressInfo.line))
                {
                    if (!progressInfo.verbose || verbose)
                        Console.WriteLine(progressInfo.line);
                }
                asciiprogress?.Report(progressInfo.progress / 100.0);
            });
        }

        private static bool hasParameter(string option)
        {
            if (option.ToLower() == "d")
                return true;
            return false;
        }

        private static void printHelp()
        {
            Console.WriteLine("Help");
        }

        private static void printReadHelp()
        {
            Console.WriteLine("Read Help");
        }
        private static async Task read(string input, ConsoleProgress progress, CancellationToken ct)
        {
            PatchProcess patchProcess = new PatchProcess();
            var mapDescriptors = await patchProcess.loadWbfsIsoFile(input, progress, ct);
            foreach (var mapDescriptor in mapDescriptors)
            {
                mapDescriptor.ToMD();
            }
        }

    }
}
