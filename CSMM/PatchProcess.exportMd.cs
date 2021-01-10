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
    public abstract partial class PatchProcess
    {
        public static void ExportMd(string destination, string cachePath, MapDescriptor mapDescriptor, bool overwrite, IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            var cacheFileSet = new DataFileSet(cachePath);
            if (Directory.Exists(destination) || string.IsNullOrEmpty(Path.GetExtension(destination)))
                destination = Path.Combine(destination, mapDescriptor.InternalName + ".md");
            var directory = Path.GetDirectoryName(destination);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);
            string fileNameMd = destination;
            string fileNameFrb1 = Path.Combine(directory, mapDescriptor.FrbFile1 + ".frb");
            string fileNameFrb2 = null;
            string fileNameFrb3 = null;
            string fileNameFrb4 = null;
            if (!string.IsNullOrWhiteSpace(mapDescriptor.FrbFile2))
                fileNameFrb2 = Path.Combine(directory, mapDescriptor.FrbFile2 + ".frb");
            if (!string.IsNullOrWhiteSpace(mapDescriptor.FrbFile3))
                fileNameFrb3 = Path.Combine(directory, mapDescriptor.FrbFile3 + ".frb");
            if (!string.IsNullOrWhiteSpace(mapDescriptor.FrbFile4))
                fileNameFrb4 = Path.Combine(directory, mapDescriptor.FrbFile4 + ".frb");

            string filesToBeReplacedMsg = "";
            var filesToBeReplaced = new List<string>();
            if (File.Exists(fileNameMd))
            {
                filesToBeReplacedMsg += fileNameMd + "\n";
                filesToBeReplaced.Add(fileNameMd);
            }
            if (File.Exists(fileNameFrb1))
            {
                filesToBeReplacedMsg += fileNameFrb1 + "\n";
                filesToBeReplaced.Add(fileNameFrb1);
            }
            if (fileNameFrb2 != null && File.Exists(fileNameFrb2))
            {
                filesToBeReplacedMsg += fileNameFrb2 + "\n";
                filesToBeReplaced.Add(fileNameFrb2);
            }
            if (fileNameFrb3 != null && File.Exists(fileNameFrb3))
            {
                filesToBeReplacedMsg += fileNameFrb3 + "\n";
                filesToBeReplaced.Add(fileNameFrb3);
            }
            if (fileNameFrb4 != null && File.Exists(fileNameFrb4))
            {
                filesToBeReplacedMsg += fileNameFrb4 + "\n";
                filesToBeReplaced.Add(fileNameFrb4);
            }

            if (filesToBeReplaced.Any())
            {
                if (overwrite)
                {
                    if (File.Exists(fileNameMd))
                    {
                        File.Delete(fileNameMd);
                    }
                    if (File.Exists(fileNameFrb1))
                    {
                        File.Delete(fileNameFrb1);
                    }
                    if (fileNameFrb2 != null && File.Exists(fileNameFrb2))
                    {
                        File.Delete(fileNameFrb2);
                    }
                    if (fileNameFrb3 != null && File.Exists(fileNameFrb3))
                    {
                        File.Delete(fileNameFrb3);
                    }
                    if (fileNameFrb4 != null && File.Exists(fileNameFrb4))
                    {
                        File.Delete(fileNameFrb4);
                    }
                }
                else
                {
                    throw new FileAlreadyExistException("The following files already exist:\n" + filesToBeReplacedMsg, filesToBeReplaced.ToArray());
                }
            }

            using (FileStream fs = File.Create(fileNameMd))
            {
                byte[] content = Encoding.UTF8.GetBytes(mapDescriptor.ToMD());
                fs.Write(content, 0, content.Length);
            }
            progress?.Report(new ProgressInfo(50, "Generated " + fileNameMd));
            File.Copy(Path.Combine(cacheFileSet.param_folder, mapDescriptor.FrbFile1 + ".frb"), fileNameFrb1);
            progress?.Report("Extracted " + fileNameFrb1);
            if (fileNameFrb2 != null)
            {
                File.Copy(Path.Combine(cacheFileSet.param_folder, mapDescriptor.FrbFile2 + ".frb"), fileNameFrb2);
                progress?.Report("Extracted " + fileNameFrb2);
            }
            if (fileNameFrb3 != null)
            {
                File.Copy(Path.Combine(cacheFileSet.param_folder, mapDescriptor.FrbFile3 + ".frb"), fileNameFrb3);
                progress?.Report("Extracted " + fileNameFrb3);
            }
            if (fileNameFrb4 != null)
            {
                File.Copy(Path.Combine(cacheFileSet.param_folder, mapDescriptor.FrbFile4 + ".frb"), fileNameFrb4);
                progress?.Report("Extracted " + fileNameFrb4);
            }
            progress?.Report(100);
        }
    }
}
