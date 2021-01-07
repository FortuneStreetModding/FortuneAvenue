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

namespace CustomStreetMapManager
{
    public partial class PatchProcess
    {
        // Key = locale, Value = file contents
        private Dictionary<string, UI_Message> ui_messages = new Dictionary<string, UI_Message>();
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

        public bool ShouldKeepCache(string input)
        {
            if (File.Exists(Path.Combine(input, "sys", "main.dol")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
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
                progress?.Report(new ProgressInfo(-1, "Copy " + file.FullName + " to " + tempPath, true));
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

        public bool isOutputImageFileExtension(string path)
        {
            var extension = Path.GetExtension(path);
            return extension.ToLower() == ".wbfs" ||
                extension.ToLower() == ".iso" ||
                extension.ToLower() == ".ciso" ||
                extension.ToLower() == ".wdf" ||
                extension.ToLower() == ".wdf1" ||
                extension.ToLower() == ".wdf2" ||
                extension.ToLower() == ".wia" ||
                extension.ToLower() == ".wbi";
        }

        public string doOutputDirectoryPathCorrections(string outputFile)
        {
            return doOutputDirectoryPathCorrections(outputFile, out _);
        }

        public string getExtractedIsoDir(string dir)
        {
            if (File.Exists(Path.Combine(dir, "sys", "main.dol")) &&
                Directory.Exists(Path.Combine(dir, "files", "bg")) &&
                Directory.Exists(Path.Combine(dir, "files", "param")) &&
                Directory.Exists(Path.Combine(dir, "files", "sound")) &&
                Directory.Exists(Path.Combine(dir, "files", "chance_card")) &&
                Directory.Exists(Path.Combine(dir, "files", "scene")) &&
                Directory.Exists(Path.Combine(dir, "files", "localize")))
            {
                return dir;
            }
            else
            {
                dir = Directory.GetParent(dir).FullName;
                if (File.Exists(Path.Combine(dir, "sys", "main.dol")) &&
                Directory.Exists(Path.Combine(dir, "files", "bg")) &&
                Directory.Exists(Path.Combine(dir, "files", "param")) &&
                Directory.Exists(Path.Combine(dir, "files", "sound")) &&
                Directory.Exists(Path.Combine(dir, "files", "chance_card")) &&
                Directory.Exists(Path.Combine(dir, "files", "scene")) &&
                Directory.Exists(Path.Combine(dir, "files", "localize")))
                {
                    return dir;
                }
            }
            return null;
        }

        public string doOutputDirectoryPathCorrections(string outputFile, out bool alreadyExists)
        {
            // do file path corrections
            if (File.Exists(outputFile))
            {
                // get the directory of the selected file
                var directory = Directory.GetParent(outputFile).FullName;
                if (getExtractedIsoDir(directory) != null)
                {
                    alreadyExists = true;
                    return getExtractedIsoDir(directory);
                }
                else
                    throw new ApplicationException("The path " + outputFile + " cannot be used as output since the target directory must be empty");
            }
            else
            {
                // get the directory of the selected file
                var directory = Directory.GetParent(outputFile).FullName;
                if (IsDirectoryEmpty(directory))
                {
                    // the directory is empty -> use it as the output directory
                    alreadyExists = false;
                    return directory;
                }
                else if (getExtractedIsoDir(directory) != null)
                {
                    alreadyExists = true;
                    return getExtractedIsoDir(directory);
                }
                else
                {
                    // turn the filename into a directory
                    directory = Path.Combine(directory, Path.GetFileNameWithoutExtension(outputFile));
                    if (Directory.Exists(directory))
                    {
                        if (IsDirectoryEmpty(directory))
                        {
                            // the directory is empty -> use it as the output directory
                            alreadyExists = false;
                            return directory;
                        }
                        else
                        {
                            if (getExtractedIsoDir(directory) != null)
                            {
                                alreadyExists = true;
                                return getExtractedIsoDir(directory);
                            }
                            else
                                throw new ApplicationException("The path " + directory + " cannot be used as output since the target directory must be empty");
                        }
                    }
                    else
                    {
                        // the directory does not exist yet -> use it as the output
                        alreadyExists = false;
                        return directory;
                    }
                }
            }
        }
    }
}