using FSEditor.Exceptions;
using FSEditor.FSData;
using MiscUtil.Conversion;
using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomStreetMapManager
{
    public abstract partial class PatchProcess
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedFile"></param>
        /// <param name="progress"></param>
        /// <param name="ct"></param>
        /// <param name="tmpFileSet">The directory to store intermediate files which are created and deleted again during the patch process</param>
        /// <param name="riivFileSet">The directory which contains the final patched and new content to be inserted into the wbfs/iso. It contains only the delta to the cache directory.</param>
        /// <returns></returns>
        public static MapDescriptor ImportMd(string selectedFile, IProgress<ProgressInfo> progress, CancellationToken ct, DataFileSet tmpFileSet = null, DataFileSet riivFileSet = null)
        {
            if (tmpFileSet == null)
            {
                tmpFileSet = new DataFileSet(GetDefaultTmpPath());
            }
            if (riivFileSet == null)
            {
                riivFileSet = new DataFileSet(GetDefaultRiivPath());
            }

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
            progress.Report(new ProgressInfo(20, "Imported " + mapDescriptorImportFile));

            var usedSquareTypes = mapDescriptorImport.readFrbFileInfo(dir, ProgressInfo.makeSubProgress(progress, 20, 60), ct);
            if (mapDescriptorImport.VentureCardActiveCount == 0)
            {
                progress.Report("The map " + internalName + " does not have a venture card table specified. A default venture card table will be used.");
                mapDescriptorImport.VentureCard = VanillaDatabase.getDefaultVentureCardTable(mapDescriptorImport.RuleSet, usedSquareTypes);
            }
            else if (mapDescriptorImport.VentureCardActiveCount < 64)
            {
                progress.Report("Warning: The map " + internalName + " has a venture card count smaller than 64. The behavior is undefined and glitchy.");
            }
            else if (mapDescriptorImport.VentureCardActiveCount > 64)
            {
                progress.Report("Warning: The map " + internalName + " has a venture card count larger than 64. Only the first 64 venture cards will be used.");
            }
            int problematicVentureCard = VanillaDatabase.hasProblemWithVentureCardMissingNeededSquareType(mapDescriptorImport.VentureCard, usedSquareTypes);
            if (problematicVentureCard != -1)
            {
                progress.Report("The map " + internalName + " uses venture card " + problematicVentureCard + ". This venture card needs certain square types which have not been placed on the map.");
                mapDescriptorImport.VentureCard = VanillaDatabase.getDefaultVentureCardTable(mapDescriptorImport.RuleSet, usedSquareTypes);
            }

            progress.Report(new ProgressInfo(60, "Copy frb file(s) to tmp..."));

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
                if (File.Exists(destFile))
                    File.Delete(destFile);
                // we have a map icon for the tutorial map ready, we can use it
                if (mapIcon == "p_bg_901")
                {
                    WriteResourceToFile("CustomStreetMapManager.Images.p_bg_901.png", destFile);
                }
                else
                {
                    File.Copy(importFile, destFile);
                    progress.Report("Imported " + importFile);
                }
            }
            mapDescriptorImport.Dirty = true;
            progress.Report(new ProgressInfo(100, "Done."));
            return mapDescriptorImport;
        }
        private static void WriteResourceToFile(string resourceName, string fileName)
        {
            using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    resource.CopyTo(file);
                }
            }
        }
    }
}
