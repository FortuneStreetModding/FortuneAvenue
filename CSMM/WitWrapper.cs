using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.IO.Compression;
using System.Windows.Forms;
using FSEditor.MapDescriptor;
using MiscUtil.Conversion;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Threading;
using FSEditor.FSData;

namespace CustomStreetManager
{
    class WitWrapper
    {
        private static readonly string[] requiredFilesWit = new string[] { "wit.exe", "cyggcc_s-1.dll", "cygwin1.dll", "cygz.dll", "cygncursesw-10.dll"};
        private static readonly string[] requiredFilesWszst = new string[] { "wszst.exe", "wimgt.exe", "cygpng16-16.dll", "cyggcc_s-1.dll", "cygwin1.dll", "cygz.dll", "cygncursesw-10.dll"};
        private static readonly string[] requiredFilesBenzin = new string[] { "benzin.exe", "cyggcc_s-1.dll", "cygwin1.dll" };

        private static Boolean requiredFilesExist(string[] requiredFiles)
        {
            foreach (string requiredFile in requiredFiles)
            {
                if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), requiredFile)))
                {
                    return false;
                }
            }
            return true;
        }

        private static void downloadAndExtractZip(string downloadUrl, string[] requiredFiles)
        {
            string zipFileName = "tmp.zip";
            string zipFilePath = Path.Combine(Directory.GetCurrentDirectory(), zipFileName);
            string extractPath = Directory.GetCurrentDirectory();
            using (var client = new WebClient())
            {
                client.DownloadFile("https://wit.wiimm.de/download/wit-v3.02a-r7679-cygwin.zip", zipFilePath);
            }
            using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    foreach (string requiredFile in requiredFiles)
                    {
                        if (entry.FullName.EndsWith(requiredFile, StringComparison.OrdinalIgnoreCase))
                        {
                            string destinationPath = Path.GetFullPath(Path.Combine(extractPath, entry.Name));
                            if (destinationPath.StartsWith(extractPath, StringComparison.Ordinal))
                                entry.ExtractToFile(destinationPath, true);
                        }
                    }
                }
            }
            File.Delete(zipFilePath);
        }

        private static void downloadWit()
        {
            downloadAndExtractZip("https://wit.wiimm.de/download/wit-v3.02a-r7679-cygwin.zip", requiredFilesWit);
        }

        public static void makeSureWitInstalled()
        {
            if (!requiredFilesExist(requiredFilesWit))
            {
                DialogResult dialogResult = MessageBox.Show("This application relies on WIT for patching. It is currently not residing next to this application. Do you want to automatically download WIT and extract the needed files next to this application from https://wit.wiimm.de/?", "Install WIT", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    downloadWit();
                    if (requiredFilesExist(requiredFilesWit))
                    {
                        MessageBox.Show("WIT has been extracted next to this application.");
                    }
                    else
                    {
                        throw new Exception("Unable to install WIT.");
                    }
                }
                else if (dialogResult == DialogResult.No)
                {
                    throw new Exception("This application cannot work without WIT.");
                }
            }
        }

        private static async Task<string> callWit(string arguments, CancellationToken cancelToken, Action<int, string, string> update, int progressMin, int progressMax)
        {
            makeSureWitInstalled();

            ProcessStartInfo psi = prepareCallWit(arguments);

            var exitCode = 0;
            string output = "";
            string error = "";
            using (Process process = Process.Start(psi))
            {
                var stdOut = process.StandardOutput;
                var stdErr = process.StandardError;
                while (!process.HasExited)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        process.Kill();
                        break;
                    }
                    string line = stdOut.ReadLine();
                    while (line != null)
                    {
                        Match match = Regex.Match(line, @"(\d+)%");
                        if (match.Success && match.Groups.Count == 2)
                        {
                            var percentage = match.Groups[1].Value;
                            float number = int.Parse(percentage) / 100.0f;
                            update?.Invoke((int)lerp(progressMin, progressMax, number), line, null);
                        } else
                        {
                            update?.Invoke(-1, line, null);
                        }
                        
                        Console.Out.WriteLine(line);
                        output += line + Environment.NewLine;
                        line = stdOut.ReadLine();
                    }
                    line = stdErr.ReadLine();
                    while (line != null)
                    {
                        update?.Invoke(-1, null, line);
                        Console.Error.WriteLine(line);
                        error += line + Environment.NewLine;
                        line = stdErr.ReadLine();
                    }
                    await Task.Delay(100);
                }
                exitCode = process.ExitCode;
            }

            if(exitCode != 0)
            {
                throw new ApplicationException("WIT returned non-zero exit code: " + exitCode + ". Output: " + error);
            }

            return output;
        }

        static float lerp(float v0, float v1, float t)
        {
            return (1 - t) * v0 + t * v1;
        }

        private static ProcessStartInfo prepareCallWit(string arguments)
        {
            string witFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wit.exe");
            ProcessStartInfo psi = new ProcessStartInfo(witFilePath, arguments);
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            psi.WorkingDirectory = Directory.GetCurrentDirectory();
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            return psi;
        }

        internal static void copyRelevantFilesForPacking(FileSet fileSet, string inputFile)
        {
            string tmpDirectory = Path.Combine(Directory.GetCurrentDirectory(), "tmp");
            string tmpExtract = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileNameWithoutExtension(inputFile), "DATA");
            var sourcePath = tmpDirectory;
            var destinationPath = tmpExtract;

            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
        }

        public static async Task<FileSet> extractFiles(string inputFile, CancellationToken cancelToken, Action<int, string, string> update, int progressMin, int progressMax)
        {
            string tmpDirectory = Path.Combine(Directory.GetCurrentDirectory(), "tmp");
            if (Directory.Exists(tmpDirectory))
            {
                Directory.Delete(tmpDirectory, true);
            }

            string arguments = "COPY --progress --fst --psel DATA --files +/sys/main.dol;+/files/localize/ui_message;+/files/param/*.frb; \"" + inputFile + "\" tmp";
            await callWit(arguments, cancelToken, update, progressMin, progressMax);

            FileSet fileSet = new FileSet();
            fileSet.main_dol = Path.Combine(tmpDirectory, "sys", "main.dol");
            fileSet.ui_message_csv[Locale.DE] = Path.Combine(tmpDirectory, "files", "localize", "ui_message.de.csv");
            fileSet.ui_message_csv[Locale.EN] = Path.Combine(tmpDirectory, "files", "localize", "ui_message.en.csv");
            fileSet.ui_message_csv[Locale.FR] = Path.Combine(tmpDirectory, "files", "localize", "ui_message.fr.csv");
            fileSet.ui_message_csv[Locale.IT] = Path.Combine(tmpDirectory, "files", "localize", "ui_message.it.csv");
            fileSet.ui_message_csv[Locale.JP] = Path.Combine(tmpDirectory, "files", "localize", "ui_message.jp.csv");
            fileSet.ui_message_csv[Locale.ES] = Path.Combine(tmpDirectory, "files", "localize", "ui_message.su.csv");
            fileSet.ui_message_csv[Locale.UK] = Path.Combine(tmpDirectory, "files", "localize", "ui_message.uk.csv");
            fileSet.param_folder = Path.Combine(tmpDirectory, "files", "param");
            return fileSet;
        }

        public static async Task<string> extractFullIsoAsync(string inputFile, CancellationToken cancelToken, Action<int, string, string> update, int progressMin, int progressMax)
        {
            string cacheExtract = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileNameWithoutExtension(inputFile));
            if (!Directory.Exists(cacheExtract))
            {
                string arguments = "COPY --progress --fst --preserve --overwrite \"" + inputFile + "\" \"" + cacheExtract + "\"";
                await callWit(arguments, cancelToken, update, progressMin, progressMax);
            }
            return cacheExtract;
        }

        public static async Task<string> packFullIso(string inputFile, string outputFile, CancellationToken cancelToken, Action<int, string, string> update, int progressMin, int progressMax)
        {
            string tmpExtract = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileNameWithoutExtension(inputFile));
            string arguments = "COPY \"" + tmpExtract + "\" \"" + outputFile + "\" -P --id .....2 --overwrite --progress";
            return await callWit(arguments, cancelToken, update, progressMin, progressMax);
        }

        public static void cleanup(string inputFile)
        {
            string tmpDirectory = Path.Combine(Directory.GetCurrentDirectory(), "tmp");
            if (Directory.Exists(tmpDirectory))
            {
                Directory.Delete(tmpDirectory, true);
            }
            string tmpExtract = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileNameWithoutExtension(inputFile));
            if (Directory.Exists(tmpExtract))
            {
                Directory.Delete(tmpExtract, true);
            }
        }

        public static async Task<List<MainDolSection>> readSections(string inputFile, CancellationToken cancelToken, Action<int, string, string> update, int progressMin, int progressMax)
        {
            string arguments = "DUMP -l \"" + inputFile + "\"";
            string witSectionDump = await callWit(arguments, cancelToken, update, progressMin, progressMax);

            List<MainDolSection> sections = new List<MainDolSection>();

            using (StringReader reader = new StringReader(witSectionDump))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("Delta between file offset and virtual address:"))
                    {
                        // remove next three lines to get to the section table
                        reader.ReadLine();
                        reader.ReadLine();
                        reader.ReadLine();
                        break;
                    }
                }
                EndianBitConverter endianBitConverter = EndianBitConverter.Big;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] columns = line.Split(':');
                    if (columns.Length == 5)
                    {
                        MainDolSection section = new MainDolSection();
                        string unused = columns[0];
                        string[] offsets = columns[1].Split(new string[] { ".." }, StringSplitOptions.None);
                        section.offsetBeg = endianBitConverter.ToUInt32(HexUtil.hexStringToByteArray(offsets[0]), 0);
                        section.offsetEnd = endianBitConverter.ToUInt32(HexUtil.hexStringToByteArray(offsets[1]), 0);
                        string size = columns[2];
                        section.fileDelta = endianBitConverter.ToUInt32(HexUtil.hexStringToByteArray(columns[3]), 0);
                        section.sectionName = columns[4].Trim();
                        sections.Add(section);
                    }
                }

            }
            return sections;
        }

        public static async Task<string> createNewTextSection(string mainDolFile, UInt32 virtualAddress, UInt32 size, CancellationToken cancelToken, Action<int, string, string> update, int progressMin, int progressMax)
        {
            string arguments = "DOLPATCH \"" + mainDolFile + "\" new=TEXT," + virtualAddress.ToString("X8") + "," + size.ToString("X8") + " " + virtualAddress.ToString("X8") + "=00000001";
            return await callWit(arguments, cancelToken, update, progressMin, progressMax);
        }

        public static async Task<string> applyPatch(string mainDolFile, string xmlPatchFile, CancellationToken cancelToken, Action<int, string, string> update, int progressMin, int progressMax)
        {
            string arguments = "DOLPATCH \"" + mainDolFile + "\" xml=\""+xmlPatchFile+"\"";
            return await callWit(arguments, cancelToken, update, progressMin, progressMax);
        }
    }
}
