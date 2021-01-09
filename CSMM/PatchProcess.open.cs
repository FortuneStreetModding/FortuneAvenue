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
        public static async Task<List<MapDescriptor>> Open(string input, IProgress<ProgressInfo> progress, CancellationToken ct, string cachePath = null)
        {
            progress?.Report(0);

            if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "none")
            {
                throw new ArgumentNullException("Can't load wbfs or iso file as the input file name is not set.");
            }

            input = DoPathCorrections(input, false);
            var cacheFileSet = new DataFileSet(GetCachePath(input, cachePath));

            if (IsImageFileExtension(input))
            {
                progress?.Report("Extract iso/wbfs...");
                await ExeWrapper.extractFullIsoAsync(input, cacheFileSet.rootDir, ct, ProgressInfo.makeSubProgress(progress, 0, 90)).ConfigureAwait(false);
            }

            progress?.Report("Detect the sections in main.dol file...");
            List<AddressSection> sections = await ExeWrapper.readSections(cacheFileSet.main_dol, ct, ProgressInfo.makeSubProgress(progress, 90, 95)).ConfigureAwait(false);

            progress?.Report("Read data from main.dol file...");

            List<MapDescriptor> mapDescriptors;
            using (var stream = File.OpenRead(cacheFileSet.main_dol))
            {
                EndianBinaryReader binReader = new EndianBinaryReader(EndianBitConverter.Big, stream);
                var mainDol = new MainDol(binReader, sections, progress);
                mapDescriptors = mainDol.readMainDol(binReader, progress);

                progress?.Report(97);
                progress?.Report("Read localization files...");
                LoadUIMessages(mapDescriptors, cacheFileSet, ProgressInfo.makeSubProgress(progress, 20, 60), ct);
            }

            progress?.Report(100);
            progress?.Report("Loaded successfully.");
            CleanTemp();
            CleanRiivolution();

            return mapDescriptors;
        }
        private static void LoadUIMessages(List<MapDescriptor> mapDescriptors, DataFileSet fileSet, IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            // Key = locale, Value = file contents
            var ui_messages = new Dictionary<string, UI_Message>();
            foreach (string locale in Locale.ALL)
            {
                ui_messages[locale] = new UI_Message(fileSet.ui_message_csv[locale], locale);
            }

            // using the MSG ID in each map descriptor, load its actual string from the ui_message_XX.csv file
            foreach (MapDescriptor mapDescriptor in mapDescriptors)
            {
                foreach (string locale in Locale.ALL)
                {
                    var msgId = mapDescriptor.Name_MSG_ID;
                    if (msgId > 0)
                    {
                        mapDescriptor.Name[locale] = ui_messages[locale].get(mapDescriptor.Name_MSG_ID);
                    }
                    else
                    {
                        mapDescriptor.Name[locale] = "";
                    }
                    msgId = mapDescriptor.Desc_MSG_ID;
                    if (msgId > 0)
                    {
                        mapDescriptor.Desc[locale] = ui_messages[locale].get(mapDescriptor.Desc_MSG_ID);
                    }
                    else
                    {
                        mapDescriptor.Desc[locale] = "";
                    }
                }
                mapDescriptor.readFrbFileInfo(fileSet.param_folder, progress, ct);
            }
        }
    }
}
