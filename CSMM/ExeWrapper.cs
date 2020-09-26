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
using CliWrap;

namespace CustomStreetManager
{
    class ExeWrapper
    {
        private static readonly bool continueOnCapturedContext = true;

        private static async Task<bool> downloadAndExtractZip(string name, string downloadUrl, string[] requiredFiles, CancellationToken cancelToken, IProgress<ProgressInfo> progress, int progressMin, int progressMax)
        {
            string zipFileName = name + ".zip";
            string zipFilePath = Path.Combine(Directory.GetCurrentDirectory(), zipFileName);
            string extractPath = Directory.GetCurrentDirectory();
            using (var client = new WebClient())
            {
                int progressMaxDownload = ProgressInfo.lerp(progressMin, progressMax, 0.5f);
                progress?.Report("Downloading " + name + "...");
                client.DownloadProgressChanged += (s, e) =>
                {
                    progress?.Report(ProgressInfo.lerp(progressMin, progressMaxDownload, e.ProgressPercentage / 100f));
                };
                await client.DownloadFileTaskAsync(downloadUrl, zipFilePath).ConfigureAwait(continueOnCapturedContext);
                progressMin = progressMaxDownload;
            }
            using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
            {
                int i = 0;
                progress?.Report("Extracting required files ...");
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
                                progress?.Report(ProgressInfo.lerp(progressMin, progressMax, i / (float)requiredFiles.Length));
                                i++;
                            }

                        }
                    }
                }
            }
            File.Delete(zipFilePath);
            return true;
        }
        private static async Task<bool> makeSureInstalled(string name, string[] requiredFiles, string host, string downloadUrl, CancellationToken cancelToken, IProgress<ProgressInfo> progress, int progressMin, int progressMax)
        {
            if (!requiredFilesExist(requiredFiles))
            {
                DialogResult dialogResult = MessageBox.Show("This application relies on " + name + " for patching. It is currently not residing next to this application. Do you want to automatically download " + name + " and extract the needed files next to this application from " + host + "?", "Install " + name, MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    await downloadAndExtractZip(name, downloadUrl, requiredFiles, cancelToken, progress, progressMin, progressMax).ConfigureAwait(continueOnCapturedContext);
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
        private static async Task<bool> makeSureWitInstalled(CancellationToken cancelToken, IProgress<ProgressInfo> progress, int progressMin, int progressMax)
        {
            string[] requiredFilesWit = new string[] { "wit.exe", "cyggcc_s-1.dll", "cygwin1.dll", "cygz.dll", "cygncursesw-10.dll" };
            return await makeSureInstalled("Wiimms ISO Tools", requiredFilesWit, "https://wit.wiimm.de/", "https://wit.wiimm.de/download/wit-v3.02a-r7679-cygwin.zip", cancelToken, progress, progressMin, progressMax).ConfigureAwait(continueOnCapturedContext);
        }
        private static async Task<bool> makeSureWszstInstalled(CancellationToken cancelToken, IProgress<ProgressInfo> progress, int progressMin, int progressMax)
        {
            string[] requiredFilesWszst = new string[] { "wszst.exe", "wimgt.exe", "cygpng16-16.dll", "cyggcc_s-1.dll", "cygwin1.dll", "cygz.dll", "cygncursesw-10.dll" };
            return await makeSureInstalled("Wiimms SZS Toolset", requiredFilesWszst, "https://szs.wiimm.de", "https://szs.wiimm.de/download/szs-v2.19b-r8243-cygwin.zip", cancelToken, progress, progressMin, progressMax).ConfigureAwait(continueOnCapturedContext);
        }
        private static async Task<bool> makeSureBenzinInstalled(CancellationToken cancelToken, IProgress<ProgressInfo> progress, int progressMin, int progressMax)
        {
            string[] requiredFilesBenzin = new string[] { "benzin.exe", "cygwin1.dll" };
            return await makeSureInstalled("Benzin", requiredFilesBenzin, "https://github.com/Deflaktor/benzin", "https://github.com/Deflaktor/benzin/releases/download/2.1.11Beta/benzin-2.1.11BETA-cygwin.zip", cancelToken, progress, progressMin, progressMax).ConfigureAwait(continueOnCapturedContext);
        }
        private static void parsePercentageValue(string line, IProgress<ProgressInfo> progress, int progressMin, int progressMax)
        {
            Match match = Regex.Match(line, @"\D(\d\d?)%");
            if (match.Success && match.Groups.Count == 2)
            {
                var percentage = match.Groups[1].Value;
                progress?.Report(ProgressInfo.lerp(progressMin, progressMax, int.Parse(percentage) / 100.0f));
            }
        }

        private static Command prepareCliWrap(string exeFileName, string arguments, StringBuilder resultBuilder, IProgress<ProgressInfo> progress, int progressMin, int progressMax)
        {
            string executablePath = Path.Combine(Directory.GetCurrentDirectory(), exeFileName);
            var stdOut = Console.OpenStandardOutput();
            return Cli.Wrap(executablePath)
                .WithWorkingDirectory(Directory.GetCurrentDirectory())
                .WithArguments(arguments)
                .WithStandardOutputPipe(
                    PipeTarget.Merge(
                        PipeTarget.ToDelegate((line) => parsePercentageValue(line, progress, progressMin, progressMax)),
                        PipeTarget.ToStringBuilder(resultBuilder),
                        PipeTarget.ToStream(stdOut)
                    )
                );
        }
        private static async Task<string> callWit(string arguments, CancellationToken cancelToken, IProgress<ProgressInfo> progress, int progressMin, int progressMax)
        {
            await makeSureWitInstalled(cancelToken, progress, progressMin, ProgressInfo.lerp(progressMin, progressMax, 0.5f)).ConfigureAwait(continueOnCapturedContext);
            StringBuilder resultBuilder = new StringBuilder();
            var result = await prepareCliWrap("wit.exe", arguments, resultBuilder, progress, progressMin, progressMax).ExecuteAsync(cancelToken);
            return resultBuilder.ToString();
        }
        private static async Task<string> callWszst(string arguments, CancellationToken cancelToken, IProgress<ProgressInfo> progress, int progressMin, int progressMax)
        {
            await makeSureWszstInstalled(cancelToken, progress, progressMin, ProgressInfo.lerp(progressMin, progressMax, 0.5f)).ConfigureAwait(continueOnCapturedContext);
            StringBuilder resultBuilder = new StringBuilder();
            var result = await prepareCliWrap("wszst.exe", arguments, resultBuilder, progress, progressMin, progressMax).ExecuteAsync(cancelToken);
            return resultBuilder.ToString();
        }
        private static async Task<string> callWimgt(string arguments, CancellationToken cancelToken, IProgress<ProgressInfo> progress, int progressMin, int progressMax)
        {
            await makeSureWszstInstalled(cancelToken, progress, progressMin, ProgressInfo.lerp(progressMin, progressMax, 0.5f)).ConfigureAwait(continueOnCapturedContext);
            StringBuilder resultBuilder = new StringBuilder();
            var result = await prepareCliWrap("wimgt.exe", arguments, resultBuilder, progress, progressMin, progressMax).ExecuteAsync(cancelToken);
            return resultBuilder.ToString();
        }
        private static async Task<string> callBenzin(string arguments, CancellationToken cancelToken, IProgress<ProgressInfo> progress, int progressMin, int progressMax)
        {
            await makeSureBenzinInstalled(cancelToken, progress, progressMin, ProgressInfo.lerp(progressMin, progressMax, 0.5f)).ConfigureAwait(continueOnCapturedContext);
            StringBuilder resultBuilder = new StringBuilder();
            var result = await prepareCliWrap("benzin.exe", arguments, resultBuilder, progress, progressMin, progressMax).ExecuteAsync(cancelToken);
            return resultBuilder.ToString();
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
        public static async Task<FileSet> extractFiles(string inputFile, CancellationToken cancelToken, IProgress<ProgressInfo> progress, int progressMin, int progressMax)
        {
            string tmpDirectory = Path.Combine(Directory.GetCurrentDirectory(), "tmp");
            if (Directory.Exists(tmpDirectory))
            {
                Directory.Delete(tmpDirectory, true);
            }

            string arguments = "COPY --progress --fst --psel DATA --files +/sys/main.dol;+/files/localize/ui_message;+/files/param/*.frb; \"" + inputFile + "\" tmp";
            var result = await callWit(arguments, cancelToken, progress, progressMin, progressMax).ConfigureAwait(continueOnCapturedContext);

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
        public static async Task<string> extractFullIsoAsync(string inputFile, CancellationToken cancelToken, IProgress<ProgressInfo> progress, int progressMin, int progressMax)
        {
            string cacheExtract = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileNameWithoutExtension(inputFile));
            if (!Directory.Exists(cacheExtract))
            {
                string arguments = "COPY --progress --fst --preserve --overwrite \"" + inputFile + "\" \"" + cacheExtract + "\"";
                await callWit(arguments, cancelToken, progress, progressMin, progressMax).ConfigureAwait(continueOnCapturedContext);
            }
            return cacheExtract;
        }
        public static async Task<string> packFullIso(string inputFile, string outputFile, CancellationToken cancelToken, IProgress<ProgressInfo> progress, int progressMin, int progressMax)
        {
            string tmpExtract = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileNameWithoutExtension(inputFile));
            string arguments = "COPY \"" + tmpExtract + "\" \"" + outputFile + "\" -P --id .....2 --overwrite --progress";
            return await callWit(arguments, cancelToken, progress, progressMin, progressMax).ConfigureAwait(continueOnCapturedContext);
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
        public static async Task<List<MainDolSection>> readSections(string inputFile, CancellationToken cancelToken, IProgress<ProgressInfo> progress, int progressMin, int progressMax)
        {
            string arguments = "DUMP -l \"" + inputFile + "\"";
            var result = await callWit(arguments, cancelToken, progress, progressMin, progressMax).ConfigureAwait(continueOnCapturedContext);

            List<MainDolSection> sections = new List<MainDolSection>();

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
        public static async Task<string> createNewTextSection(string mainDolFile, UInt32 virtualAddress, UInt32 size, CancellationToken cancelToken, IProgress<ProgressInfo> progress, int progressMin, int progressMax)
        {
            string arguments = "DOLPATCH \"" + mainDolFile + "\" new=TEXT," + virtualAddress.ToString("X8") + "," + size.ToString("X8") + " " + virtualAddress.ToString("X8") + "=00000001";
            return await callWit(arguments, cancelToken, progress, progressMin, progressMax).ConfigureAwait(continueOnCapturedContext);
        }
        public static async Task<string> applyPatch(string mainDolFile, string xmlPatchFile, CancellationToken cancelToken, IProgress<ProgressInfo> progress, int progressMin, int progressMax)
        {
            string arguments = "DOLPATCH \"" + mainDolFile + "\" xml=\"" + xmlPatchFile + "\"";
            return await callWit(arguments, cancelToken, progress, progressMin, progressMax).ConfigureAwait(continueOnCapturedContext);
        }
        public static async Task<string> extractArcFile(string arcFile, string destFolder, CancellationToken cancelToken, IProgress<ProgressInfo> progress, int progressMin, int progressMax)
        {
            string arguments = "EXTRACT --verbose \"" + arcFile + "\" --dest \"" + destFolder + "\"";
            return await callWszst(arguments, cancelToken, progress, progressMin, progressMax).ConfigureAwait(continueOnCapturedContext);
        }
        public static async Task<string> packDfolderToArc(string destFolder, string arcFile, CancellationToken cancelToken, IProgress<ProgressInfo> progress, int progressMin, int progressMax)
        {
            string arguments = "CREATE --verbose \"" + destFolder + "\" --dest \"" + arcFile + "\"";
            return await callWszst(arguments, cancelToken, progress, progressMin, progressMax).ConfigureAwait(continueOnCapturedContext);
        }
        public static async Task<string> convertBryltToXmlyt(string bryltFile, string xmlytFile, CancellationToken cancelToken, IProgress<ProgressInfo> progress, int progressMin, int progressMax)
        {
            string arguments = "r \"" + bryltFile + "\" \"" + xmlytFile + "\"";
            return await callBenzin(arguments, cancelToken, progress, progressMin, progressMax).ConfigureAwait(continueOnCapturedContext);
        }
        public static async Task<string> convertXmlytToBrylt(string xmlytFile, string brlytFile, CancellationToken cancelToken, IProgress<ProgressInfo> progress, int progressMin, int progressMax)
        {
            string arguments = "m \"" + xmlytFile + "\" \"" + brlytFile + "\"";
            return await callBenzin(arguments, cancelToken, progress, progressMin, progressMax).ConfigureAwait(continueOnCapturedContext);
        }
        public static async Task<string> convertTplToPng(string tplFile, string pngFile, CancellationToken cancelToken, IProgress<ProgressInfo> progress, int progressMin, int progressMax)
        {
            string arguments = "DECODE \"" + tplFile + "\" --dest \"" + pngFile + "\"";
            return await callWimgt(arguments, cancelToken, progress, progressMin, progressMax).ConfigureAwait(continueOnCapturedContext);
        }
        public static async Task<string> convertPngToTpl(string pngFile, string tplFile, CancellationToken cancelToken, IProgress<ProgressInfo> progress, int progressMin, int progressMax)
        {
            string arguments = "ENCODE \"" + pngFile + "\" --dest \"" + tplFile + "\"";
            return await callWimgt(arguments, cancelToken, progress, progressMin, progressMax).ConfigureAwait(continueOnCapturedContext);
        }
    }
}
