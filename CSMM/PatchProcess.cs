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

        public string GetDefaultTmpPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "tmp");
        }

        public string GetDefaultRiivPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "fortunestreet");
        }
        /// <summary>
        /// The cache directory is the directory for the extraced wbfs/iso file. It should not be modified and can be reused later 
        /// to speed up the next patch process. Or if the user wishes to, can also be cleaned up at the end of the patch process. 
        /// </summary>
        public string GetCachePath(string input)
        {
            input = DoDirectoryPathCorrections(input, false);
            if (!IsImageFileExtension(input))
            {
                return input;
            }
            else
            {
                return Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileNameWithoutExtension(input));
            }
        }

        /// <summary>
        /// Cleans all temporary files which are generated during the patch proccess which do not belong in the iso
        /// </summary>
        public void CleanTemp(DataFileSet tmpFileSet = null)
        {
            if (tmpFileSet == null)
            {
                tmpFileSet = new DataFileSet(GetDefaultTmpPath());
            }
            if (Directory.Exists(tmpFileSet.rootDir))
            {
                Directory.Delete(tmpFileSet.rootDir, true);
            }
        }

        /// <summary>
        /// Cleans the folder which was extracted from the wbfs/iso for caching purposes.
        /// </summary>
        public void CleanCache(string input)
        {
            if (!ShouldKeepCache(input))
            {
                input = GetCachePath(input);
                if (Directory.Exists(input))
                {
                    Directory.Delete(input, true);
                }
            }
        }

        /// <summary>
        /// Cleans all files which are needed for the iso
        /// </summary>
        public void CleanRiivolution(DataFileSet riivFileSet = null)
        {
            if (riivFileSet == null)
            {
                riivFileSet = new DataFileSet(GetDefaultRiivPath());
            }
            if (Directory.Exists(riivFileSet.rootDir))
            {
                Directory.Delete(riivFileSet.rootDir, true);
            }
        }

        private bool ShouldKeepCache(string input)
        {
            input = DoDirectoryPathCorrections(input, false);
            if (IsImageFileExtension(input))
            {
                return false;
            }
            else
            {
                return true;
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

        public bool IsImageFileExtension(string path)
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

        public string DoDirectoryPathCorrections(string file, bool output)
        {
            return DoDirectoryPathCorrections(file, output, out _);
        }

        private string GetExtractedIsoDir(string dir)
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

        public string DoDirectoryPathCorrections(string file, bool output, out bool alreadyExists)
        {
            ApplicationException exception;
            if (output)
            {
                exception = new ApplicationException("The path " + file + " cannot be used as output since the target directory must be empty");
            }
            else
            {
                exception = new ApplicationException("The path " + file + " is not a valid fortune street directory");
            }
            // do file path corrections
            if (File.Exists(file))
            {
                // get the directory of the selected file
                var directory = Directory.GetParent(file).FullName;
                if (GetExtractedIsoDir(directory) != null)
                {
                    alreadyExists = true;
                    return GetExtractedIsoDir(directory);
                }
                else
                    throw exception;
            }
            else
            {
                // get the directory of the selected file
                var directory = Directory.GetParent(file).FullName;
                if (IsDirectoryEmpty(directory))
                {
                    // the directory is empty -> use it as the output directory
                    alreadyExists = false;
                    return directory;
                }
                else if (GetExtractedIsoDir(directory) != null)
                {
                    alreadyExists = true;
                    return GetExtractedIsoDir(directory);
                }
                else
                {
                    // turn the filename into a directory
                    directory = Path.Combine(directory, Path.GetFileNameWithoutExtension(file));
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
                            if (GetExtractedIsoDir(directory) != null)
                            {
                                alreadyExists = true;
                                return GetExtractedIsoDir(directory);
                            }
                            else
                                throw exception;
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