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
        public async Task<List<MapDescriptor>> loadWbfsIsoFile(string inputWbfsIso, IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            progress?.Report(0);
            progress?.Report("Extract iso/wbfs...");
            cleanUp(false, true);
            if (string.IsNullOrWhiteSpace(inputWbfsIso) || inputWbfsIso.ToLower() == "none")
            {
                throw new ArgumentNullException("Can't load wbfs or iso file as the input file name is not set.");
            }

            inputFile = inputWbfsIso;
            string extractDir = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileNameWithoutExtension(inputWbfsIso));
            await ExeWrapper.extractFullIsoAsync(inputWbfsIso, extractDir, ct, ProgressInfo.makeSubProgress(progress, 0, 90)).ConfigureAwait(false);

            cacheFileSet = new DataFileSet(Path.Combine(extractDir, "DATA"));
            progress?.Report("Detect the sections in main.dol file...");
            List<MainDolSection> sections = await ExeWrapper.readSections(cacheFileSet.main_dol, ct, ProgressInfo.makeSubProgress(progress, 90, 95)).ConfigureAwait(false);
            mainDol = new MainDol(sections);

            progress?.Report("Read data from main.dol file...");
            using (var stream = File.OpenRead(cacheFileSet.main_dol))
            {
                EndianBinaryReader binReader = new EndianBinaryReader(EndianBitConverter.Big, stream);
                mapDescriptors = mainDol.readMainDol(binReader);

                progress?.Report(97);
                progress?.Report("Read localization files...");
                progress?.Report(loadUIMessages(mapDescriptors, mainDol.data, cacheFileSet));
            }

            this.cleanUp(false, false);

            progress?.Report(100);
            progress?.Report("Loaded successfully.");

            return mapDescriptors;
        }
        private string loadUIMessages(List<MapDescriptor> mapDescriptors, ST7_Interface data, DataFileSet fileSet)
        {
            string warnings = "";
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
                // add warnings
                warnings += mapDescriptor.readFrbFileInfo(fileSet.param_folder);
            }
            return warnings;
        }
    }
}
