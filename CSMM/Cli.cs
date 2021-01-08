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
   load            extract the data of a Fortune Street game disc image to a
                     directory. Creates a configuration file to be used for 
                     further operations.
   export          export one or several map descriptor files (*.md) from an 
                     extracted Fortune Street game disc directory
   import          import a map descriptor file (*.md) to an existing 
                     configuration file
   save            save the changes to a Fortune Street game disc directory or
                     to an image file
   json            output an overview of the viewable data from a 
                     Fortune Street game disc directory to json
   help            to output a step by step guide how to use CSMM CLI

Use 'csmm <command>' to read about a specific command.

options:
   -a              all
   -c <config>     configuration file
   -d <path>       destination
   -f              force
   -i <id>         id
   -m <mapset>     mapset
   -n <name>       name
   -o <order>      order
   -q              quiet (overrides verbose)
   -t <true,false> tutorial map
   -v              verbose
   -w <true,false> patch wiimmfi (default: true)
   -z <zone>       zone

attention: csmm requires that Wiimms Iso Toolset, Wiimms Szs Toolset and Benzin 
   are either installed or the binaries located in the working directory
");
            _ = @"
--------------------------------------------------------------------------------
";
        }
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
                commands.Add(new CliLoad());
                commands.Add(new CliExport());
                commands.Add(new CliImport());
                commands.Add(new CliSave());
                commands.Add(new CliJson());
                commands.Add(new CliHelp());
                using (ConsoleProgress progress = new ConsoleProgress(verbose, quiet))
                {
                    try
                    {
                        bool commandGiven = false;
                        foreach (var command in commands)
                        {
                            if (args[0].ToLower() == command.Name)
                            {
                                commandGiven = true;
                                if (string.IsNullOrEmpty(input))
                                    Console.WriteLine(command.GetHelp());
                                else
                                    await command.Run(input, options, progress, ct);
                            }
                        }
                        if (!commandGiven)
                        {
                            PrintHelp();
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
                case "m":
                case "n":
                case "o":
                case "z":
                case "c":
                case "t":
                case "w":
                    return true;
            }
            return false;
        }

    }
}
