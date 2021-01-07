using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CustomStreetMapManager
{
    class Cli
    {
        private static void PrintHelp()
        {
            _ = @"
--------------------------------------------------------------------------------
";
            Console.WriteLine(@"
        ****************************************************************          
        *                                                              *     
        *           .-------.   _____  _____ __  __ __  __             *     
        *          /   o   /|  / ____|/ ____|  \/  |  \/  |            *     
        *         /_______/o| | |    | (___ | \  / | \  / |            *     
        *         | o   o | | | |     \___ \| |\/| | |\/| |            *     
        *         |   o   |o/ | |____ ____) | |  | | |  | |            *     
        *         | o   o |/   \_____|_____/|_|  |_|_|  |_|            *     
        *         '-------'                                            *     
        *                     Custom Street Map Manager                *     
        *                                                              *     
        *                     github.com/FortuneStreetModding          *     
        *                                                              *     
        ****************************************************************     

usage: csmm <command> [options] <input>

commands:
   extract         extracts the data of a Fortune Street game disc image to a 
                     directory
   export          export one or several map descriptor files (*.md) from a 
                     Fortune Street game disc directory
   import          import one or several map descriptor files (*.md) into a 
                     Fortune Street game disc directory
   pack            pack a Fortune Street game disc directory into an image file
   json            output the whole data from a Fortune Street game disc 
                     directory to json

Use 'csmm <command>' to read about a specific command.

options:
   -v              verbose
   -q              quiet (overrides verbose)
");
            _ = @"
--------------------------------------------------------------------------------
";
        }
        static async Task Main(string[] args)
        {
            //args = new string[] { "export", "Test", "-d", "MD", "-i", "0", "-v", "-o" };

            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                var ct = cancellationTokenSource.Token;
                Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) =>
                {
                    // prevent terminating the application instantly if user requests it -> only once user requests twice
                    if (!cancellationTokenSource.IsCancellationRequested)
                    {
                        Console.WriteLine("Aborting process...");
                        e.Cancel = true;
                        cancellationTokenSource.Cancel();
                    }
                    else
                    {
                        Console.WriteLine("Killing process without cleanup...");
                    }
                };
                if (args.Length == 0)
                {
                    PrintHelp();
                    return;
                }
                var input = "";
                var options = new Dictionary<string, string>();
                for (int i = 1; i < args.Length; i++)
                {
                    if (args[i].StartsWith("-"))
                    {
                        var option = args[i].Substring(1);
                        if (HasParameter(option))
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
                var quiet = options.ContainsKey("q");
                var verbose = options.ContainsKey("v");
                var commands = new List<CliCommand>();
                commands.Add(new CliExport());
                commands.Add(new CliJson());
                commands.Add(new CliExtract());
                using (ConsoleProgress progress = new ConsoleProgress(verbose, quiet))
                {
                    try
                    {
                        foreach (var command in commands)
                        {
                            if (args[0].ToLower() == command.Name)
                            {
                                if (string.IsNullOrEmpty(input))
                                    Console.WriteLine(command.GetHelp());
                                else
                                    await command.Run(input, options, progress, ct);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Environment.ExitCode = e.HResult;
                        Console.WriteLine(e.Message);
                        if (verbose)
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        // cleanup?
                    }
                }
            }
        }

        private static bool HasParameter(string option)
        {
            switch (option.ToLower())
            {
                case "d":
                case "i":
                case "n":
                    return true;
            }
            return false;
        }
    }
}
