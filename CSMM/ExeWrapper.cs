using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.IO.Compression;
using System.Windows.Forms;
using MiscUtil.Conversion;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Threading;
using FSEditor.FSData;

namespace CustomStreetManager
{
    class ExeWrapper
    {
        private static readonly bool continueOnCapturedContext = true;

        private static async Task<bool> downloadAndExtractZip(string name, string downloadUrl, string[] requiredFiles, CancellationToken cancelToken, IProgress<ProgressInfo> progress)
        {
            string zipFileName = name + ".zip";
            string zipFilePath = Path.Combine(Directory.GetCurrentDirectory(), zipFileName);
            string extractPath = Directory.GetCurrentDirectory();
            var progressSub = ProgressInfo.makeSubProgress(progress, 0, 50);
            using (var client = new WebClient())
            {
                progressSub?.Report("Downloading " + name + "...");
                client.DownloadProgressChanged += (s, e) =>
                {
                    progressSub?.Report(e.ProgressPercentage);
                };
                await client.DownloadFileTaskAsync(downloadUrl, zipFilePath).ConfigureAwait(continueOnCapturedContext);
            }
            progressSub = ProgressInfo.makeSubProgress(progress, 50, 100);
            using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
            {
                int i = 0;
                progressSub?.Report("Extracting " + name + " ...");
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    foreach (string requiredFile in requiredFiles)
                    {
                        if (entry.FullName.EndsWith(requiredFile, StringComparison.OrdinalIgnoreCase))
                        {
                            string destinationPath = Path.GetFullPath(Path.Combine(extractPath, entry.Name));
                            if (destinationPath.StartsWith(extractPath, StringComparison.Ordinal))
                            {
                                entry.ExtractToFile(destinationPath, true);
                                progressSub?.Report(100 * i / requiredFiles.Length);
                                i++;
                            }

                        }
                    }
                }
            }
            File.Delete(zipFilePath);
            return true;
        }
        private static async Task<bool> makeSureInstalled(string name, string[] requiredFiles, string host, string downloadUrl, CancellationToken cancelToken, IProgress<ProgressInfo> progress)
        {
            if (!requiredFilesExist(requiredFiles))
            {
                DialogResult dialogResult = MessageBox.Show("This application relies on " + name + " for patching. It is currently not residing next to this application. Do you want to automatically download " + name + " and extract the needed files next to this application from " + host + "?", "Install " + name, MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    await downloadAndExtractZip(name, downloadUrl, requiredFiles, cancelToken, progress).ConfigureAwait(continueOnCapturedContext);
                    if (requiredFilesExist(requiredFiles))
                    {
                        progress?.Report(name + " has been extracted next to this application.");
                    }
                    else
                    {
                        var errorMsg = "Unable to install " + name + ". The required files were not found.";
                        progress?.Report(errorMsg);
                        throw new Exception(errorMsg);
                    }
                }
                else if (dialogResult == DialogResult.No)
                {
                    var errorMsg = "This application cannot work without " + name;
                    progress?.Report(errorMsg);
                    throw new Exception(errorMsg);
                }
            }
            return true;
        }
        private static bool requiredFilesExist(string[] requiredFiles)
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
        public static async Task<bool> makeSureWitInstalled(CancellationToken cancelToken, IProgress<ProgressInfo> progress)
        {
            string[] requiredFilesWit = new string[] { "wit.exe", "cyggcc_s-1.dll", "cygwin1.dll", "cygz.dll", "cygncursesw-10.dll" };
            return await makeSureInstalled("Wiimms ISO Tools", requiredFilesWit, "https://wit.wiimm.de/", "https://wit.wiimm.de/download/wit-v3.02a-r7679-cygwin.zip", cancelToken, progress).ConfigureAwait(continueOnCapturedContext);
        }
        public static async Task<bool> makeSureWszstInstalled(CancellationToken cancelToken, IProgress<ProgressInfo> progress)
        {
            string[] requiredFilesWszst = new string[] { "wszst.exe", "wimgt.exe", "cygpng16-16.dll", "cyggcc_s-1.dll", "cygwin1.dll", "cygz.dll", "cygncursesw-10.dll" };
            return await makeSureInstalled("Wiimms SZS Toolset", requiredFilesWszst, "https://szs.wiimm.de", "https://szs.wiimm.de/download/szs-v2.19b-r8243-cygwin.zip", cancelToken, progress).ConfigureAwait(continueOnCapturedContext);
        }
        public static async Task<bool> makeSureBenzinInstalled(CancellationToken cancelToken, IProgress<ProgressInfo> progress)
        {
            string[] requiredFilesBenzin = new string[] { "benzin.exe", "cygwin1.dll" };
            return await makeSureInstalled("Benzin", requiredFilesBenzin, "https://github.com/Deflaktor/benzin", "https://github.com/Deflaktor/benzin/releases/download/2.1.11Beta/benzin-2.1.11BETA-cygwin.zip", cancelToken, progress).ConfigureAwait(continueOnCapturedContext);
        }
        private static void parsePercentageValue(string line, IProgress<ProgressInfo> progress)
        {
            var pi = new ProgressInfo();
            pi.progress = -1;
            pi.line = line;
            pi.verbose = true;
            Match match = Regex.Match(line, @"(\d\d\d?)%");
            if (match.Success && match.Groups.Count == 2)
            {
                var percentage = match.Groups[1].Value;
                pi.progress = int.Parse(percentage);
            }
            progress?.Report(pi);
        }

        private static async Task<string> execute(ProcessStartInfo psi, CancellationToken cancelToken, IProgress<ProgressInfo> progress)
        {
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
                    string line;
                    while ((line = await stdOut.ReadLineAsync()) != null)
                    {
                        parsePercentageValue(line, progress);
                        output += line + Environment.NewLine;
                    }
                    while ((line = await stdErr.ReadLineAsync()) != null)
                    {
                        parsePercentageValue(line, progress);
                        output += line + Environment.NewLine;
                    }
                    await Task.Delay(100);
                }
                exitCode = process.ExitCode;
            }

            if (exitCode != 0)
            {
                throw new ApplicationException(psi.FileName + " returned non-zero exit code: " + exitCode + ". Output: " + error);
            }

            return output;
        }

        private static ProcessStartInfo preparePsi(string executable, string arguments)
        {
            string executablePath = Path.Combine(Directory.GetCurrentDirectory(), executable);
            ProcessStartInfo psi = new ProcessStartInfo(executablePath, arguments);
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            psi.WorkingDirectory = Directory.GetCurrentDirectory();
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.StandardOutputEncoding = Encoding.UTF8;
            psi.StandardErrorEncoding = Encoding.UTF8;
            return psi;
        }

        private static async Task<string> callWit(string arguments, CancellationToken cancelToken, IProgress<ProgressInfo> progress)
        {
            await makeSureWitInstalled(cancelToken, ProgressInfo.makeSubProgress(progress, 0, 10)).ConfigureAwait(continueOnCapturedContext);
            var psi = preparePsi("wit.exe", arguments);
            return await execute(psi, cancelToken, ProgressInfo.makeSubProgress(progress, 10, 100));
        }
        private static async Task<string> callWszst(string arguments, CancellationToken cancelToken, IProgress<ProgressInfo> progress)
        {
            await makeSureWszstInstalled(cancelToken, ProgressInfo.makeSubProgress(progress, 0, 10)).ConfigureAwait(continueOnCapturedContext);
            var psi = preparePsi("wszst.exe", arguments);
            return await execute(psi, cancelToken, ProgressInfo.makeSubProgress(progress, 10, 100));
        }
        private static async Task<string> callWimgt(string arguments, CancellationToken cancelToken, IProgress<ProgressInfo> progress)
        {
            await makeSureWszstInstalled(cancelToken, ProgressInfo.makeSubProgress(progress, 0, 10)).ConfigureAwait(continueOnCapturedContext);
            var psi = preparePsi("wimgt.exe", arguments);
            return await execute(psi, cancelToken, ProgressInfo.makeSubProgress(progress, 10, 100));
        }
        private static async Task<string> callBenzin(string arguments, CancellationToken cancelToken, IProgress<ProgressInfo> progress)
        {
            await makeSureBenzinInstalled(cancelToken, ProgressInfo.makeSubProgress(progress, 0, 10)).ConfigureAwait(continueOnCapturedContext);
            var psi = preparePsi("benzin.exe", arguments);
            return await execute(psi, cancelToken, ProgressInfo.makeSubProgress(progress, 10, 100));
        }
        public static async Task<string> extractFiles(string inputFile, string outputDirectory, CancellationToken cancelToken, IProgress<ProgressInfo> progress)
        {
            string arguments = "COPY --progress --fst --preserve --overwrite --psel DATA --files +/sys/main.dol;+/files/localize/ui_message;+/files/param/*.frb;+/files/game/game_sequence*.arc;+/files/game/lang*/game_sequence*.arc; \"" + inputFile + "\" \"" + outputDirectory + "\"";
            var result = await callWit(arguments, cancelToken, progress).ConfigureAwait(continueOnCapturedContext);
            return result;
        }
        public static async Task<string> extractFullIsoAsync(string inputFile, string outputDirectory, CancellationToken cancelToken, IProgress<ProgressInfo> progress)
        {

            if (!Directory.Exists(outputDirectory))
            {
                string arguments = "COPY --progress --psel data --fst --preserve --overwrite \"" + inputFile + "\" \"" + outputDirectory + "\"";
                return await callWit(arguments, cancelToken, progress).ConfigureAwait(continueOnCapturedContext);
            }
            else
            {
                return await extractFiles(inputFile, outputDirectory, cancelToken, progress);
            }
        }
        public static async Task<string> packFullIso(string inputFile, string outputFile, CancellationToken cancelToken, IProgress<ProgressInfo> progress)
        {
            string tmpExtract = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileNameWithoutExtension(inputFile));
            string arguments = "COPY \"" + tmpExtract + "\" \"" + outputFile + "\" -P --id .....2 --overwrite --progress";
            return await callWit(arguments, cancelToken, progress).ConfigureAwait(continueOnCapturedContext);
        }

        public static async Task<List<AddressSection>> readSections(string inputFile, CancellationToken cancelToken, IProgress<ProgressInfo> progress)
        {
            string arguments = "DUMP -l \"" + inputFile + "\"";
            var result = await callWit(arguments, cancelToken, progress).ConfigureAwait(continueOnCapturedContext);

            List<AddressSection> sections = new List<AddressSection>();

            using (StringReader reader = new StringReader(result))
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
                        string unused = columns[0];
                        string[] offsets = columns[1].Split(new string[] { ".." }, StringSplitOptions.None);
                        var offsetBeg = endianBitConverter.ToUInt32(HexUtil.hexStringToByteArray(offsets[0]), 0);
                        var offsetEnd = endianBitConverter.ToUInt32(HexUtil.hexStringToByteArray(offsets[1]), 0);
                        string size = columns[2];
                        var fileDelta = endianBitConverter.ToUInt32(HexUtil.hexStringToByteArray(columns[3]), 0);
                        var sectionName = columns[4].Trim();
                        sections.Add(new AddressSection(offsetBeg, offsetEnd, fileDelta, sectionName));
                    }
                }

            }
            return sections;
        }
        public static async Task<string> createNewTextSection(string mainDolFile, UInt32 virtualAddress, UInt32 size, CancellationToken cancelToken, IProgress<ProgressInfo> progress)
        {
            string arguments = "DOLPATCH \"" + mainDolFile + "\" new=TEXT," + virtualAddress.ToString("X8") + "," + size.ToString("X8") + " " + virtualAddress.ToString("X8") + "=00000001";
            return await callWit(arguments, cancelToken, progress).ConfigureAwait(continueOnCapturedContext);
        }
        public static async Task<string> applyPatch(string mainDolFile, string xmlPatchFile, CancellationToken cancelToken, IProgress<ProgressInfo> progress)
        {
            string arguments = "DOLPATCH \"" + mainDolFile + "\" xml=\"" + xmlPatchFile + "\"";
            return await callWit(arguments, cancelToken, progress).ConfigureAwait(continueOnCapturedContext);
        }
        public static async Task<string> extractArcFile(string arcFile, string dFolder, CancellationToken cancelToken, IProgress<ProgressInfo> progress)
        {
            string arguments = "EXTRACT --verbose \"" + arcFile + "\" --dest \"" + dFolder + "\"";
            return await callWszst(arguments, cancelToken, progress).ConfigureAwait(continueOnCapturedContext);
        }
        public static async Task<string> packDfolderToArc(string dFolder, string arcFile, CancellationToken cancelToken, IProgress<ProgressInfo> progress)
        {
            string arguments = "CREATE --overwrite --verbose \"" + dFolder + "\" --dest \"" + arcFile + "\"";
            return await callWszst(arguments, cancelToken, progress).ConfigureAwait(continueOnCapturedContext);
        }
        public static async Task<string> convertBryltToXmlyt(string bryltFile, string xmlytFile, CancellationToken cancelToken, IProgress<ProgressInfo> progress)
        {
            string arguments = "r \"" + bryltFile + "\" \"" + xmlytFile + "\"";
            return await callBenzin(arguments, cancelToken, progress).ConfigureAwait(continueOnCapturedContext);
        }
        public static async Task<string> convertXmlytToBrylt(string xmlytFile, string brlytFile, CancellationToken cancelToken, IProgress<ProgressInfo> progress)
        {
            string arguments = "m \"" + xmlytFile + "\" \"" + brlytFile + "\"";
            return await callBenzin(arguments, cancelToken, progress).ConfigureAwait(continueOnCapturedContext);
        }
        public static async Task<string> convertTplToPng(string tplFile, string pngFile, CancellationToken cancelToken, IProgress<ProgressInfo> progress)
        {
            string arguments = "DECODE \"" + tplFile + "\" --dest \"" + pngFile + "\"";
            return await callWimgt(arguments, cancelToken, progress).ConfigureAwait(continueOnCapturedContext);
        }
        public static async Task<string> convertPngToTpl(string pngFile, string tplFile, CancellationToken cancelToken, IProgress<ProgressInfo> progress)
        {
            string arguments = "ENCODE \"" + pngFile + "\" --dest \"" + tplFile + "\"";
            return await callWimgt(arguments, cancelToken, progress).ConfigureAwait(continueOnCapturedContext);
        }
    }
}
