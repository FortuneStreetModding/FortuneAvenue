using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.IO.Compression;
using System.Windows.Forms;

namespace CustomStreetManager
{
    class WitWrapper
    {
        private static readonly string[] requiredFiles = new string[] { "wit.exe", "cyggcc_s-1.dll", "cygwin1.dll", "cygz.dll"};

        private static Boolean witExists()
        {
            foreach (string requiredFile in requiredFiles)
            {
                if(!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), requiredFile)))
                {
                    return false;
                }
            }
            return true;
        }

        private static void downloadWit()
        {
            string zipFileName = "wit.zip";
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
                                entry.ExtractToFile(destinationPath);
                        }
                    }
                }
            }
            File.Delete(zipFilePath);
        }

        private static void makeSureWitInstalled()
        {
            if (!witExists())
            {
                DialogResult dialogResult = MessageBox.Show("This application relies on WIT for patching. It is currently not residing next to this application. Do you want to automatically download WIT and extract the needed files next to this application from https://wit.wiimm.de/?", "Install WIT", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    downloadWit();
                    if (witExists())
                    {
                        MessageBox.Show("WIT has been extracted next to this application.");
                    } else
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

        public static string callWit(string arguments)
        {
            makeSureWitInstalled();

            string witFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wit.exe");
            ProcessStartInfo psi = new ProcessStartInfo(witFilePath, arguments);
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            psi.WorkingDirectory = Directory.GetCurrentDirectory();
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            string result = "";
            using (Process process = Process.Start(psi))
            {
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    string error;
                    using (StreamReader reader = process.StandardError)
                    {
                        error = reader.ReadToEnd();
                    }
                    throw new InvalidOperationException("WIT returned non-zero exit code: " + process.ExitCode + ". Output: " + error);
                }
                using (StreamReader reader = process.StandardOutput)
                {
                    result = reader.ReadToEnd();
                }
            }
            Console.Write(result);
            return result;
        }

        public static FileSet extractFiles(string inputFile)
        {
            string tmpDirectory = Path.Combine(Directory.GetCurrentDirectory(), "tmp");
            if(Directory.Exists(tmpDirectory)) { 
                Directory.Delete(tmpDirectory, true);
            }

            string arguments = "COPY --fst --psel DATA --files +/sys/main.dol;+/files/localize/ui_message; \"" + inputFile + "\" tmp";
            callWit(arguments);

            FileSet fileSet = new FileSet();
            fileSet.main_dol = Path.Combine(tmpDirectory, "sys", "main.dol");
            fileSet.ui_message_de_csv = Path.Combine(tmpDirectory, "files", "localize", "ui_message.de.csv");
            fileSet.ui_message_en_csv = Path.Combine(tmpDirectory, "files", "localize", "ui_message.en.csv");
            fileSet.ui_message_fr_csv = Path.Combine(tmpDirectory, "files", "localize", "ui_message.fr.csv");
            fileSet.ui_message_it_csv = Path.Combine(tmpDirectory, "files", "localize", "ui_message.it.csv");
            fileSet.ui_message_jp_csv = Path.Combine(tmpDirectory, "files", "localize", "ui_message.jp.csv");
            fileSet.ui_message_su_csv = Path.Combine(tmpDirectory, "files", "localize", "ui_message.su.csv");
            fileSet.ui_message_uk_csv = Path.Combine(tmpDirectory, "files", "localize", "ui_message.uk.csv");
            return fileSet;
        }
    }
}
