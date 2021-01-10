using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

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

usage: csmm <command> [options] 

commands:
   open            open a Fortune Street game disc image or extracted directory
   export          export one or several map descriptor files (*.md)
   import          import a map descriptor file (*.md)
   save            save the changes to a Fortune Street game disc image or
                     extracted directory
   close           close a Fortune Street game disc image or extracted directory
   json            output an overview of the viewable data from a 
                     Fortune Street game disc directory to json
   help            to output a step by step guide how to use CSMM CLI

Use 'csmm help <command>' to read about a specific command.

options:
" + GetOptionsHelp() + @"

attention: csmm requires that Wiimms Iso Toolset, Wiimms Szs Toolset and Benzin 
           are either installed or the binaries located in the working directory
");
            _ = @"
--------------------------------------------------------------------------------
";
        }

        static async Task Main(string[] args)
        {
            // args = new string[] { "open" };
            using var cancellationTokenSource = new CancellationTokenSource();
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
            string subCommand = null;
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
                else if (string.IsNullOrEmpty(subCommand))
                {
                    subCommand = args[i];
                }
            }
            var quiet = options.ContainsKey("q");
            var verbose = options.ContainsKey("v");
            var commands = GetCommandList();
            using ConsoleProgress progress = new ConsoleProgress(verbose, quiet);
            try
            {
                bool commandGiven = false;
                foreach (var command in commands)
                {
                    if (args[0].ToLower() == command.Name)
                    {
                        commandGiven = true;
                        await command.Run(subCommand, options, progress, ct);
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

        public static List<CliCommand> GetCommandList()
        {
            var commands = new List<CliCommand>();
            commands.Add(new CliOpen());
            commands.Add(new CliExport());
            commands.Add(new CliImport());
            commands.Add(new CliSave());
            commands.Add(new CliJson());
            commands.Add(new CliHelp());
            commands.Add(new CliClose());
            return commands;
        }

        public static List<CliOption> GetOptionsList()
        {
            List<CliOption> options = new List<CliOption>();
            foreach (var c in GetCommandList())
            {
                foreach (var o in c.Options)
                {
                    if (!options.Contains(o))
                        options.Add(o);
                }
            }
            options.Sort();
            return options;
        }

        public static string GetOptionsHelp()
        {
            var str = new StringBuilder();
            foreach (var option in GetOptionsList())
            {
                str.AppendLine(option.ToString());
            }
            return str.ToString();
        }


        public static bool HasParameter(string optionStr)
        {
            foreach (var option in GetOptionsList())
            {
                if (option.Name == optionStr)
                    return option.HasArgument();
            }
            return false;
        }

    }
}
