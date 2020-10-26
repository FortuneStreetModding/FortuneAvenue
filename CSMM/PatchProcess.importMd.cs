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
        public void importMd(string selectedFile, MapDescriptor mapDescriptor, IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            MapDescriptor mapDescriptorImport = new MapDescriptor();

            progress.Report(new ProgressInfo(0, "Parse Map Descriptor File..."));

            var mapDescriptorImportFile = selectedFile;
            var dir = Path.GetDirectoryName(selectedFile);

            var internalName = Path.GetFileNameWithoutExtension(mapDescriptorImportFile);
            if (internalName.ToLower() == "readme")
            {
                internalName = Path.GetFileName(dir);
            }

            mapDescriptorImport.readMapDescriptorFromFile(mapDescriptorImportFile, internalName);

            if (mapDescriptorImport.VentureCardActiveCount != 64)
            {
                progress.Report("Warning: The venture card count needs to be 64 or the game will choose a default venture card table." + Environment.NewLine);
            }

            progress.Report("Imported " + mapDescriptorImportFile);

            progress.Report(new ProgressInfo(20, "Imported " + mapDescriptorImportFile));
            mapDescriptorImport.readFrbFileInfo(dir);

            progress.Report(new ProgressInfo(40, "Copy frb file(s) to tmp..."));

            var frbFileName = mapDescriptorImport.FrbFile1;
            var importFile = Path.Combine(dir, frbFileName + ".frb");
            var destFile = Path.Combine(riivFileSet.param_folder, frbFileName + ".frb");
            Directory.CreateDirectory(riivFileSet.param_folder);
            File.Copy(importFile, destFile, true);

            progress.Report("Imported " + importFile);

            frbFileName = mapDescriptorImport.FrbFile2;
            if (frbFileName != null)
            {
                importFile = Path.Combine(dir, frbFileName + ".frb");
                destFile = Path.Combine(riivFileSet.param_folder, frbFileName + ".frb");
                File.Copy(importFile, destFile, true);
                progress.Report("Imported " + importFile);
            }
            frbFileName = mapDescriptorImport.FrbFile3;
            if (frbFileName != null)
            {
                importFile = Path.Combine(dir, frbFileName + ".frb");
                destFile = Path.Combine(riivFileSet.param_folder, frbFileName + ".frb");
                File.Copy(importFile, destFile, true);
                progress.Report("Imported " + importFile);
            }
            frbFileName = mapDescriptorImport.FrbFile4;
            if (frbFileName != null)
            {
                importFile = Path.Combine(dir, frbFileName + ".frb");
                destFile = Path.Combine(riivFileSet.param_folder, frbFileName + ".frb");
                File.Copy(importFile, destFile, true);
                progress.Report("Imported " + importFile);
            }
            var mapIcon = mapDescriptorImport.MapIcon;
            // only import the png if it is not a vanilla map icon
            if (mapIcon != null && !VanillaDatabase.getVanillaTpl(mapIcon).Any())
            {
                importFile = Path.Combine(dir, mapIcon + ".png");
                destFile = Path.Combine(tmpFileSet.param_folder, mapIcon + ".png");
                Directory.CreateDirectory(tmpFileSet.param_folder);
                File.Copy(importFile, destFile, true);
                progress.Report("Imported " + importFile);
            }
            mapDescriptor.set(mapDescriptorImport);
            mapDescriptor.Dirty = true;
            progress.Report(new ProgressInfo(100, "Done."));
        }
    }
}
