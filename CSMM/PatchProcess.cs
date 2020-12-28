using FSEditor.Exceptions;
using FSEditor.FSData;
using MiscUtil.Conversion;
using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomStreetManager
{
    public partial class PatchProcess
    {
        private MainDol mainDol;
        // Key = locale, Value = file contents
        private Dictionary<string, UI_Message> ui_messages = new Dictionary<string, UI_Message>();
        private string inputFile;
        // The cache directory is the directory for the extraced wbfs/iso file. It should not be modified and can be reused later 
        // to speed up the next patch process. Or if the user wishes to, can also be cleaned up at the end of the patch process. 
        // The directory which contains the final patched and new content to be inserted into the wbfs/iso. It contains only the delta to the cache directory.
        private readonly DataFileSet riivFileSet = new DataFileSet(Path.Combine(Directory.GetCurrentDirectory(), "fortunestreet"));
        // The directory to store intermediate files which are created and deleted again during the path process
        private readonly DataFileSet tmpFileSet = new DataFileSet(Path.Combine(Directory.GetCurrentDirectory(), "tmp"));
        private DataFileSet cacheFileSet;

        /// <summary>
        /// Cleans all temporary files which are generated during the patch proccess which do not belong in the iso
        /// </summary>
        public void cleanTemp()
        {
            if (Directory.Exists(tmpFileSet.rootDir))
            {
                Directory.Delete(tmpFileSet.rootDir, true);
            }
        }

        /// <summary>
        /// Cleans the folder which was extracted from the wbfs/iso for caching purposes.
        /// </summary>
        public void cleanCache()
        {
            if (Directory.Exists(cacheFileSet.rootDir))
            {
                Directory.Delete(cacheFileSet.rootDir, true);
            }
        }

        /// <summary>
        /// Cleans all files which are needed for the iso
        /// </summary>
        public void cleanRiivolution()
        {
            if (Directory.Exists(riivFileSet.rootDir))
            {
                Directory.Delete(riivFileSet.rootDir, true);
            }
        }

        public void cleanFull()
        {
            cleanTemp();
            cleanCache();
            cleanRiivolution();
        }

        // From: https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, bool overwrite, IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                if (ct.IsCancellationRequested)
                {
                    return;
                }

                string tempPath = Path.Combine(destDirName, file.Name);
                progress?.Report(new ProgressInfo(-1, "Copy " + file.FullName + " to " + tempPath + "...", true));
                file.CopyTo(tempPath, overwrite);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs, overwrite, progress, ct);
                }
            }
        }
    }
}
