using MiscUtil.Conversion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomStreetMapManager
{
    abstract class CliCommand
    {
        public static CliOption a = new CliOption("a", "all");
        public static CliOption c = new CliOption("c", "configuration file", "path", "<ExtractedGameDir>/config.csv");
        public static CliOption d = new CliOption("d", "destination directory", "path", "current directory");
        public static CliOption f = new CliOption("f", "force overwrite existing files");
        public static CliOption m = new CliOption("m", "mapset", "0|1", "1");
        public static CliOption n = new CliOption("n", "comma seperated list of internal names of the maps to export", "name");
        public static CliOption o = new CliOption("o", "order", "order", "auto assigned");
        public static CliOption i = new CliOption("i", "map id", "0,1,...");
        public static CliOption p = new CliOption("p", "practice board", "true|false");
        public static CliOption q = new CliOption("q", "quiet (overrides verbose)");
        public static CliOption s = new CliOption("s", "source Fortune Street game disc image or extracted directory", "path", "auto select candidate in current directory");
        public static CliOption t = new CliOption("t", "save to extracted directory only for testing purposes. Only valid if the source is a game disc image file");
        public static CliOption v = new CliOption("v", "verbose");
        public static CliOption w = new CliOption("w", "patch wiimmfi", "true|false", "true");
        public static CliOption z = new CliOption("z", "zone", "0|1|2", "2");

        public string Name { get; set; }
        public CliOption[] Options { get; set; }
        protected CliCommand(string name, params CliOption[] options)
        {
            Name = name;
            Options = options;
        }
        public abstract string GetHelp();
        protected string GetOptionsHelp()
        {
            var str = new StringBuilder();
            var sorted = from o in Options orderby o.ArgumentName select o;
            foreach (var option in sorted)
            {
                str.AppendLine(option.ToString());
            }
            return str.ToString();
        }
        public abstract Task Run(string subCommand, string fortuneStreetPath, string configPath, Dictionary<string, string> options, ConsoleProgress progress, CancellationToken ct);
        public async Task Run(string subCommand, Dictionary<string, string> options, ConsoleProgress progress, CancellationToken ct)
        {
            if (this is CliHelp)
            {
                await Run(subCommand, null, null, options, progress, ct);
                return;
            }
            if (options.ContainsKey("h") || options.ContainsKey("?") || options.ContainsKey("-help"))
            {
                Console.WriteLine(GetHelp());
                return;
            }
            var source = options.GetValueOrDefault("s", null);
            if (source != null)
            {
                var cachePath = PatchProcess.GetCachePath(source);
                var configPath = options.GetValueOrDefault("c", Path.Combine(cachePath, "config.csv"));
                await Run(subCommand, source, configPath, options, progress, ct);
            }
            else
            {
                List<string> candidates = await ExeWrapper.findCandidates("", ct, progress);
                // remove the candidates which are extracted versions of an image
                int removeId = -1;
                do
                {
                    for (int i = candidates.Count - 1; i >= 0; i--)
                    {
                        removeId = candidates.FindIndex((s) => PatchProcess.GetCachePath(s) == candidates[i] && s != candidates[i]);
                        if (removeId != -1)
                        {
                            candidates.RemoveAt(i);
                            break;
                        }
                    }
                } while (removeId != -1);

                if (candidates.Count == 1)
                {
                    var fortuneStreetPath = candidates.Single();
                    progress?.Report("Found suitable candidate in current directory: " + fortuneStreetPath);
                    var cachePath = PatchProcess.GetCachePath(fortuneStreetPath);
                    var configPath = options.GetValueOrDefault("c", Path.Combine(cachePath, "config.csv"));
                    await Run(subCommand, fortuneStreetPath, configPath, options, progress, ct);
                }
                else
                {
                    progress?.Report("Following candidates found:");
                    foreach (var candidate in candidates)
                    {
                        progress?.Report(candidate);
                    }
                    progress?.Report("Please specify the source Fortune Street game disc image or directory with -s <path>");
                }
            }
        }
        protected Optional<int> GetIntParameter(Dictionary<string, string> options, string parameter)
        {
            if (!options.ContainsKey(parameter))
                return Optional<int>.CreateEmpty();
            return Optional<int>.Create(int.Parse(options[parameter]));
        }
        protected Optional<sbyte> GetSbyteParameter(Dictionary<string, string> options, string parameter)
        {
            if (!options.ContainsKey(parameter))
                return Optional<sbyte>.CreateEmpty();
            return Optional<sbyte>.Create(sbyte.Parse(options[parameter]));
        }
        protected Optional<bool> GetBoolParameter(Dictionary<string, string> options, string parameter)
        {
            if (!options.ContainsKey(parameter))
                return Optional<bool>.CreateEmpty();
            return Optional<bool>.Create(bool.Parse(options[parameter]));
        }
    }
}
