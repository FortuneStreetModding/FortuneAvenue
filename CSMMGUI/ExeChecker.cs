using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomStreetMapManager
{
    class ExeChecker
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

        private static async Task<bool> makeSureInstalled(string name, string[] requiredRunnables, string[] requiredFiles, string host, string downloadUrl, CancellationToken cancelToken, IProgress<ProgressInfo> progress)
        {
            bool canRun = await canRunAsync(requiredRunnables, cancelToken, progress);
            if (!canRun && !requiredFilesExist(requiredFiles))
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

        private static async Task<bool> canRunAsync(string[] requiredRunnables, CancellationToken cancelToken, IProgress<ProgressInfo> progress)
        {
            foreach (var requiredRunnable in requiredRunnables)
            {
                var psi = ExeWrapper.preparePsi(requiredRunnable, "");
                await ExeWrapper.execute(psi, cancelToken, ProgressInfo.makeNoProgress(progress));
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
            string[] requiredRunnables = new string[] { "wit" };
            string[] requiredFilesWit = new string[] { "wit.exe", "cyggcc_s-1.dll", "cygwin1.dll", "cygz.dll", "cygncursesw-10.dll" };
            return await makeSureInstalled("Wiimms ISO Tools", requiredRunnables, requiredFilesWit, "https://wit.wiimm.de/", "https://wit.wiimm.de/download/wit-v3.02a-r7679-cygwin.zip", cancelToken, progress).ConfigureAwait(continueOnCapturedContext);
        }
        public static async Task<bool> makeSureWszstInstalled(CancellationToken cancelToken, IProgress<ProgressInfo> progress)
        {
            string[] requiredRunnables = new string[] { "wszst", "wimgt" };
            string[] requiredFilesWszst = new string[] { "wszst.exe", "wimgt.exe", "cygpng16-16.dll", "cyggcc_s-1.dll", "cygwin1.dll", "cygz.dll", "cygncursesw-10.dll" };
            return await makeSureInstalled("Wiimms SZS Toolset", requiredRunnables, requiredFilesWszst, "https://szs.wiimm.de", "https://szs.wiimm.de/download/szs-v2.19b-r8243-cygwin.zip", cancelToken, progress).ConfigureAwait(continueOnCapturedContext);
        }
        public static async Task<bool> makeSureBenzinInstalled(CancellationToken cancelToken, IProgress<ProgressInfo> progress)
        {
            string[] requiredRunnables = new string[] { "benzin" };
            string[] requiredFilesBenzin = new string[] { "benzin.exe", "cygwin1.dll" };
            return await makeSureInstalled("Benzin", requiredRunnables, requiredFilesBenzin, "https://github.com/Deflaktor/benzin", "https://github.com/Deflaktor/benzin/releases/download/2.1.11Beta/benzin-2.1.11BETA-cygwin.zip", cancelToken, progress).ConfigureAwait(continueOnCapturedContext);
        }
    }
}
