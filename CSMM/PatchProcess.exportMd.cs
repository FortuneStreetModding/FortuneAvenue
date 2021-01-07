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
        public static async Task<string> ExportMd(string mdFileName, string input, MapDescriptor mapDescriptor, bool overwrite, IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            input = DoPathCorrections(input, false);
            var cacheFileSet = new DataFileSet(GetCachePath(input));

            var directory = Path.GetDirectoryName(mdFileName);
            string fileNameMd = mdFileName;
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
                filesToBeReplacedMsg += fileNameMd + Environment.NewLine;
                filesToBeReplaced.Add(fileNameMd);
            }
            if (File.Exists(fileNameFrb1))
            {
                filesToBeReplacedMsg += fileNameFrb1 + Environment.NewLine;
                filesToBeReplaced.Add(fileNameFrb1);
            }
            if (fileNameFrb2 != null && File.Exists(fileNameFrb2))
            {
                filesToBeReplacedMsg += fileNameFrb2 + Environment.NewLine;
                filesToBeReplaced.Add(fileNameFrb2);
            }
            if (fileNameFrb3 != null && File.Exists(fileNameFrb3))
            {
                filesToBeReplacedMsg += fileNameFrb3 + Environment.NewLine;
                filesToBeReplaced.Add(fileNameFrb3);
            }
            if (fileNameFrb4 != null && File.Exists(fileNameFrb4))
            {
                filesToBeReplacedMsg += fileNameFrb4 + Environment.NewLine;
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
                    throw new FileAlreadyExistException("The following files already exist and will be overwritten:" + Environment.NewLine + filesToBeReplacedMsg, filesToBeReplaced.ToArray());
                }
            }

            progress?.Report(new ProgressInfo(0, "Generating Map Descriptor File..."));

            string extractedFiles = "";
            using (FileStream fs = File.Create(fileNameMd))
            {
                byte[] content = Encoding.UTF8.GetBytes(mapDescriptor.ToMD());
                await fs.WriteAsync(content, 0, content.Length).ConfigureAwait(false);
            }
            extractedFiles += fileNameMd + Environment.NewLine;

            progress?.Report(new ProgressInfo(50, "Copying frb files..."));

            File.Copy(Path.Combine(cacheFileSet.param_folder, mapDescriptor.FrbFile1 + ".frb"), fileNameFrb1);
            extractedFiles += fileNameFrb1 + Environment.NewLine;
            if (fileNameFrb2 != null)
            {
                File.Copy(Path.Combine(cacheFileSet.param_folder, mapDescriptor.FrbFile2 + ".frb"), fileNameFrb2);
                extractedFiles += fileNameFrb2 + Environment.NewLine;
            }
            if (fileNameFrb3 != null)
            {
                File.Copy(Path.Combine(cacheFileSet.param_folder, mapDescriptor.FrbFile3 + ".frb"), fileNameFrb3);
                extractedFiles += fileNameFrb3 + Environment.NewLine;
            }
            if (fileNameFrb4 != null)
            {
                File.Copy(Path.Combine(cacheFileSet.param_folder, mapDescriptor.FrbFile4 + ".frb"), fileNameFrb4);
                extractedFiles += fileNameFrb4 + Environment.NewLine;
            }
            progress?.Report(new ProgressInfo(100, "Done. Generated md file and extracted frb file(s):"));
            return extractedFiles;
        }
    }
}
