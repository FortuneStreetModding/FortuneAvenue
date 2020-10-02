using FSEditor.Exceptions;
using FSEditor.FSData;
using FSEditor.MapDescriptor;
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
                loadUIMessages(mapDescriptors, mainDol.data, cacheFileSet);
            }

            progress?.Report(100);
            progress?.Report("Loaded successfully.");

            return mapDescriptors;
        }
        private string loadUIMessages(List<MapDescriptor> mapDescriptors, AddressConstants data, DataFileSet fileSet)
        {
            string warnings = "";
            foreach (string locale in Locale.ALL_WITHOUT_UK)
            {
                ui_messages[locale] = new UI_Message(fileSet.ui_message_csv[locale], locale);
            }
            // reuse the EN locale for the UK locale
            ui_messages[Locale.UK] = ui_messages[Locale.EN];

            // using the MSG ID in each map descriptor, load its actual string from the ui_message_XX.csv file
            foreach (MapDescriptor mapDescriptor in mapDescriptors)
            {
                foreach (string locale in Locale.ALL_WITHOUT_UK)
                {
                    mapDescriptor.Name[locale] = ui_messages[locale].get(mapDescriptor.Name_MSG_ID);
                    mapDescriptor.Desc[locale] = ui_messages[locale].get(mapDescriptor.Desc_MSG_ID);
                }
                warnings += mapDescriptor.readFrbFileInfo(fileSet.param_folder);
            }
            // -- vanilla special case handling --
            /* 
             * the normal game shares the name msg id and the description msg id over the easy and standard ruleset. 
             * As such we have only 18 msg ids but 36 maps (15 normal maps + 3 debug maps)
             * We go through the first 18 maps and assign them a new msg id so that each map can have its own name and description
             * even between different rulesets, easy or standard.
             */
            for (var i = 0; i <= 17; i++)
            {
                var mapDescriptor = mapDescriptors[i];
                if (mapDescriptor.Name_MSG_ID == data.VANILLA_FIRST_MAP_NAME_MESSAGE_ID() + i)
                {
                    var freeKey = ui_messages.Values.First().freeKey();
                    foreach (UI_Message ui_message in ui_messages.Values)
                    {
                        ui_message.set(freeKey, "");
                    }
                    mapDescriptor.Name_MSG_ID = freeKey;
                }
            }
            for (var i = 0; i <= 17; i++)
            {
                var mapDescriptor = mapDescriptors[i];
                if (mapDescriptor.Desc_MSG_ID == data.VANILLA_FIRST_MAP_DESC_MESSAGE_ID() + i)
                {
                    var freeKey = ui_messages.Values.First().freeKey();
                    foreach (UI_Message ui_message in ui_messages.Values)
                    {
                        ui_message.set(freeKey, "");
                    }
                    mapDescriptor.Desc_MSG_ID = freeKey;
                }
            }
            return warnings;
        }
    }
}
