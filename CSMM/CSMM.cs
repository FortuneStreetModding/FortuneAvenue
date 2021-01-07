using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CustomStreetMapManager
{
    class CSMM
    {

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
                using (ConsoleProgress progress = new ConsoleProgress(verbose, quiet))
                {
                    try
                    {
                        if (args[0].ToLower() == "json")
                        {
                            if (string.IsNullOrEmpty(input))
                                printJsonHelp();
                            else
                                await Json(input, options.GetValueOrDefault("d", null), options.ContainsKey("p"), progress, ct);
                        }
                        if (args[0].ToLower() == "extract")
                        {
                            if (string.IsNullOrEmpty(input))
                                printExtractHelp();
                            else
                                await Extract(input, options.GetValueOrDefault("d", null), progress, ct);
                        }
                        if (args[0].ToLower() == "export")
                        {
                            if (string.IsNullOrEmpty(input))
                                printExportHelp();
                            else
                                await Export(input, options.GetValueOrDefault("d", null), options.GetValueOrDefault("i", null), options.ContainsKey("a"), options.GetValueOrDefault("n", null), options.ContainsKey("o"), progress, ct);
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
        private static void printExtractHelp()
        {
            _ = @"
--------------------------------------------------------------------------------
";
            Console.WriteLine(@"
usage: csmm extract [options] <input>

options:
   -v              verbose
   -q              quiet (overrides verbose)

   -d path         destination
");
            _ = @"
--------------------------------------------------------------------------------
";
        }
        private static async Task Extract(string input, string destination, ConsoleProgress progress, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(destination))
                throw new ArgumentException("Destination not set");
            if (File.Exists(destination))
                throw new ArgumentException("Destination must be a directory");
            var mapDescriptors = await PatchProcess.Load(input, progress, ct, destination);
            await Task.Delay(500);
        }
        private static void printExportHelp()
        {
            _ = @"
--------------------------------------------------------------------------------
";
            Console.WriteLine(@"
usage: csmm export [options] <input>

options:
   -v              verbose
   -q              quiet (overrides verbose)

   -d path         destination directory
   -a              all
   -i id1,...      ids of the maps to export
   -n name1,...    internal names of the map to export
   -o              overwrite existing files
");
            _ = @"
--------------------------------------------------------------------------------
";
        }
        private static async Task Export(string input, string destination, string idList, bool all, string internalNameList, bool overwrite, ConsoleProgress progress, CancellationToken ct)
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

        private static void printJsonHelp()
        {
            _ = @"
--------------------------------------------------------------------------------
";
            Console.WriteLine(@"
usage: csmm json [options] <input>

options:
   -v              verbose
   -q              quiet (overrides verbose)

   -d path         destination file
   -p              print to console
");
            _ = @"
--------------------------------------------------------------------------------
";
        }
        private static async Task Json(string input, string destination, bool print, ConsoleProgress progress, CancellationToken ct)
        {
            if (!print && string.IsNullOrEmpty(destination))
                throw new ArgumentException("Neither destination nor print is set. Nothing to do.");
            var mapDescriptors = await PatchProcess.Load(input, progress, ct);
            await Task.Delay(500);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            if (print)
                Console.WriteLine(JsonSerializer.Serialize(mapDescriptors, options));
            if (!string.IsNullOrEmpty(destination))
                File.WriteAllBytes(destination, JsonSerializer.SerializeToUtf8Bytes(mapDescriptors, options));
        }

    }
}
