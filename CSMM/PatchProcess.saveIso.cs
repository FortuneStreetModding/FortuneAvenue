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

        public async Task<bool> saveWbfsIso(string inputFile, string outputFile, bool cleanUp, IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            progress?.Report(new ProgressInfo(0, "Writing localization files..."));
            writeLocalizationFiles();

            progress?.Report(new ProgressInfo(5, "Writing main.dol..."));
            await patchMainDolAsync(ProgressInfo.makeSubProgress(progress, 0, 6), ct);

            // lets get to the map icons
            progress?.Report("Writing map icons...");
            await injectMapIcons(ProgressInfo.makeSubProgress(progress, 7, 40), ct).ConfigureAwait(false);

            using (CancellationTokenSource source = new CancellationTokenSource())
            {
                // start fake progress
                progress.Report(new ProgressInfo(45, "Copying the modified files to be packed into the image..."));
                var fakeProgressTask = ProgressInfo.makeFakeProgress(ProgressInfo.makeSubProgress(progress, 45, 60), source.Token);

                DirectoryCopy(riivFileSet.rootDir, cacheFileSet.rootDir, true, true, ProgressInfo.makeNoProgress(progress), ct);
                // riivFileSet.copy(frbFilesToBeCopied, cacheFileSet, true);
                source.Cancel();
                await fakeProgressTask.ConfigureAwait(false);
            }

            progress.Report(new ProgressInfo(60, "Packing ISO/WBFS file..."));
            await ExeWrapper.packFullIso(inputFile, outputFile, ct, ProgressInfo.makeSubProgress(progress, 60, 100)).ConfigureAwait(false);

            // this.cleanUp(cleanUp, cleanUp);

            progress.Report(new ProgressInfo(100, "Done."));
            return true;
        }

        private void writeLocalizationFiles()
        {
            // free up the used MSG IDs
            foreach (MapDescriptor mapDescriptor in mapDescriptors)
            {
                foreach (string locale in Locale.ALL)
                {
                    if (mapDescriptor.Name_MSG_ID > 0)
                        ui_messages[locale].removeKey(mapDescriptor.Name_MSG_ID);
                    if (mapDescriptor.Desc_MSG_ID > 0)
                        ui_messages[locale].removeKey(mapDescriptor.Desc_MSG_ID);
                }
            }
            // make new msg ids
            foreach (var mapDescriptor in mapDescriptors)
            {
                mapDescriptor.Name_MSG_ID = ui_messages.Values.First().freeKey();
                foreach (var entry in ui_messages)
                {
                    var locale = entry.Key;
                    var ui_message = entry.Value;
                    // write EN messages to the UK file as well (we are not differentiating here)
                    if (locale == Locale.UK)
                        locale = Locale.EN;
                    // if there is no localization for this locale, use the english variant as default
                    if (string.IsNullOrWhiteSpace(mapDescriptor.Name[locale]))
                    {
                        ui_message.set(mapDescriptor.Name_MSG_ID, mapDescriptor.Name[Locale.EN]);
                    }
                    else
                    {
                        ui_message.set(mapDescriptor.Name_MSG_ID, mapDescriptor.Name[locale]);
                    }
                }
                mapDescriptor.Desc_MSG_ID = ui_messages.Values.First().freeKey();
                foreach (var entry in ui_messages)
                {
                    var locale = entry.Key;
                    var ui_message = entry.Value;
                    // write EN messages to the UK file as well (we are not differentiating here)
                    if (locale == Locale.UK)
                        locale = Locale.EN;
                    // if there is no localization for this locale, use the english variant as default
                    if (string.IsNullOrWhiteSpace(mapDescriptor.Desc[locale]))
                    {
                        ui_message.set(mapDescriptor.Desc_MSG_ID, mapDescriptor.Desc[Locale.EN]);
                    }
                    else
                    {
                        ui_message.set(mapDescriptor.Desc_MSG_ID, mapDescriptor.Desc[locale]);
                    }
                }
            }
            // write to files
            foreach (var entry in ui_messages)
            {
                string locale = entry.Key;
                UI_Message ui_message = entry.Value;
                Directory.CreateDirectory(Path.GetDirectoryName(riivFileSet.ui_message_csv[locale]));
                ui_message.writeToFile(riivFileSet.ui_message_csv[locale]);
            }
        }

        private async Task patchMainDolAsync(IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(riivFileSet.main_dol));
            File.Copy(cacheFileSet.main_dol, riivFileSet.main_dol, true);

            // expand dol if not already expanded
            if (mainDol.toFileAddress(0x80001800) == -1)
            {
                await ExeWrapper.createNewTextSection(riivFileSet.main_dol, 0x80001800, 0x1800, ct, progress);
                mainDol.setSections(await ExeWrapper.readSections(riivFileSet.main_dol, ct, progress));
            }

            using (Stream baseStream = File.Open(riivFileSet.main_dol, FileMode.Open))
            {
                try
                {
                    EndianBinaryWriter stream = new EndianBinaryWriter(EndianBitConverter.Big, baseStream);
                    mainDol.writeMainDol(stream, mapDescriptors);
                }
                finally
                {
                    progress.Report("Before Patch:");
                    progress.Report("Amount of free space available in main.dol: " + mainDol.freeSpaceManager.calculateTotalFreeSpace() + " bytes");
                    progress.Report("Amount of free space in the largest block in main.dol: " + mainDol.freeSpaceManager.calculateLargestFreeSpaceBlockSize() + " bytes");
                    progress.Report("After Patch:");
                    progress.Report("Amount of free space available in main.dol: " + mainDol.freeSpaceManager.calculateTotalRemainingFreeSpace() + " bytes");
                    progress.Report("Amount of free space in the largest block in main.dol: " + mainDol.freeSpaceManager.calculateLargestRemainingFreeSpaceBlockSize() + " bytes");
                    int bytesWritten = mainDol.freeSpaceManager.calculateTotalFreeSpace() - mainDol.freeSpaceManager.calculateTotalRemainingFreeSpace();
                    progress.Report("Total free space used: " + bytesWritten + " bytes");
                }
            }
        }

        private async Task<bool> injectMapIcons(IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            progress.Report("Checking needed installations...");

            await ExeWrapper.makeSureWszstInstalled(ct, ProgressInfo.makeSubProgress(progress, 0, 1)).ConfigureAwait(false);
            await ExeWrapper.makeSureBenzinInstalled(ct, ProgressInfo.makeSubProgress(progress, 1, 2)).ConfigureAwait(false);

            progress.Report("Extract game_sequence files...");

            // setup the directories for the game sequence files

            /** maps the path of the various variants of the game_sequenceXXXXXXXX.arc files to their respective extraction path in the tmp directory */
            var gameSequenceExtractPaths = new Dictionary<string, string>();
            /** maps the path of the various variants of the game_sequenceXXXXXXXX.arc files to their respective temporary path for the converted ui_menu_19_00a.xmlyt path */
            var gameSequenceToXmlytPaths = new Dictionary<string, string>();
            /** maps the path of the various variants of the game_sequenceXXXXXXXX.arc files to their target path where they will be packed again */
            var gameSequencePackPaths = new Dictionary<string, string>();
            foreach (var entry in cacheFileSet.game_sequence_arc)
            {
                var locale = entry.Key;
                var gameSequencePath = entry.Value;

                var extractPath = Path.Combine(tmpFileSet.rootDir, Path.GetFileNameWithoutExtension(gameSequencePath));
                Directory.CreateDirectory(Path.GetDirectoryName(extractPath));
                gameSequenceExtractPaths.Add(gameSequencePath, extractPath);

                var xmlytPath = Path.Combine(tmpFileSet.rootDir, Path.GetFileNameWithoutExtension(gameSequencePath) + ".ui_menu_19_00a.xmlyt");
                Directory.CreateDirectory(Path.GetDirectoryName(xmlytPath));
                gameSequenceToXmlytPaths.Add(gameSequencePath, xmlytPath);

                var packPath = riivFileSet.game_sequence_arc[locale];
                Directory.CreateDirectory(Path.GetDirectoryName(packPath));
                gameSequencePackPaths.Add(gameSequencePath, packPath);
            }
            foreach (var entry in cacheFileSet.game_sequence_wifi_arc)
            {
                var locale = entry.Key;
                var gameSequencePath = entry.Value;

                var extractPath = Path.Combine(tmpFileSet.rootDir, Path.GetFileNameWithoutExtension(gameSequencePath));
                Directory.CreateDirectory(Path.GetDirectoryName(extractPath));
                gameSequenceExtractPaths.Add(gameSequencePath, extractPath);

                var xmlytPath = Path.Combine(tmpFileSet.rootDir, Path.GetFileNameWithoutExtension(gameSequencePath) + ".ui_menu_19_00a.xmlyt");
                Directory.CreateDirectory(Path.GetDirectoryName(xmlytPath));
                gameSequenceToXmlytPaths.Add(gameSequencePath, xmlytPath);

                var packPath = riivFileSet.game_sequence_wifi_arc[locale];
                Directory.CreateDirectory(Path.GetDirectoryName(packPath));
                gameSequencePackPaths.Add(gameSequencePath, packPath);
            }

            using (CancellationTokenSource source = new CancellationTokenSource())
            {
                // start fake progress
                var fakeProgressTask = ProgressInfo.makeFakeProgress(ProgressInfo.makeSubProgress(progress, 2, 33), source.Token);

                // extract the arc files
                List<Task<string>> extractArcFileTasks = new List<Task<string>>();
                foreach (var entry in gameSequenceExtractPaths)
                {
                    string gameSequencePath = entry.Key;
                    string gameSequenceExtractPath = entry.Value;
                    extractArcFileTasks.Add(ExeWrapper.extractArcFile(gameSequencePath, gameSequenceExtractPath, ct, ProgressInfo.makeNoProgress(progress)));
                }
                await Task.WhenAll(extractArcFileTasks).ConfigureAwait(false);
                source.Cancel();
                await fakeProgressTask.ConfigureAwait(false);
            }
            progress.Report("Convert map icons and inject them...");
            using (CancellationTokenSource source = new CancellationTokenSource())
            {
                // start fake progress
                var fakeProgressTask = ProgressInfo.makeFakeProgress(ProgressInfo.makeSubProgress(progress, 33, 66), source.Token);

                // convert the png files to tpl and copy them to the correct location
                Dictionary<string, string> mapIconToTplName = new Dictionary<string, string>();
                List<Task> convertPngFileTasks = new List<Task>();
                foreach (var mapDescriptor in mapDescriptors)
                {
                    var mapIcon = mapDescriptor.MapIcon;
                    if (string.IsNullOrEmpty(mapIcon))
                        continue;

                    var tplName = VanillaDatabase.getVanillaTpl(mapIcon);
                    if (tplName != null)
                    {
                        // its a vanilla map icon -> dont convert and inject it
                        if (!mapIconToTplName.ContainsKey(mapIcon))
                        {
                            mapIconToTplName.Add(mapIcon, tplName + ".tpl");
                        }
                    }
                    else
                    {
                        var mapIconPng = Path.Combine(tmpFileSet.param_folder, mapIcon + ".png");
                        var mapIconTpl = Path.ChangeExtension(mapIconPng, ".tpl");
                        tplName = Ui_menu_19_00a_XMLYT.constructMapIconTplName(mapIcon);
                        if (!mapIconToTplName.ContainsKey(mapIcon))
                        {
                            mapIconToTplName.Add(mapIcon, tplName);
                        }
                        if (File.Exists(mapIconPng))
                        {
                            Task task1 = ExeWrapper.convertPngToTpl(mapIconPng, mapIconTpl, ct, ProgressInfo.makeNoProgress(progress));
                            Task task2 = task1.ContinueWith(async (t1) =>
                            {
                                await t1.ConfigureAwait(false);
                                foreach (var entry in gameSequenceExtractPaths)
                                {
                                    string gameSequencePath = entry.Key;
                                    string gameSequenceExtractPath = entry.Value;
                                    var mapIconTplCopy = Path.Combine(gameSequenceExtractPath, "arc", "timg", tplName);
                                    File.Copy(mapIconTpl, mapIconTplCopy, true);
                                }
                            });
                            convertPngFileTasks.Add(task2);
                        }
                    }
                }

                // convert the brlyt files to xmlyt, inject the map icons and convert it back
                List<Task> injectMapIconsInBrlytTasks = new List<Task>();
                foreach (var entry in gameSequenceExtractPaths)
                {
                    string gameSequencePath = entry.Key;
                    string gameSequenceExtractPath = entry.Value;
                    string xmlytFile = gameSequenceToXmlytPaths[gameSequencePath];
                    var brlytFile = Path.Combine(gameSequenceExtractPath, "arc", "blyt", "ui_menu_19_00a.brlyt");
                    Task task1 = ExeWrapper.convertBryltToXmlyt(brlytFile, xmlytFile, ct, ProgressInfo.makeNoProgress(progress));
                    Task task2 = task1.ContinueWith(async (t1) => { await t1.ConfigureAwait(false); Ui_menu_19_00a_XMLYT.injectMapIcons(xmlytFile, mapIconToTplName); });
                    Task task3 = task2.ContinueWith(async (t2) => { await t2.ConfigureAwait(false); await ExeWrapper.convertXmlytToBrylt(xmlytFile, brlytFile, ct, ProgressInfo.makeNoProgress(progress)); });
                    Task task4 = task3.ContinueWith(async (t3) =>
                    {
                        await t3;
                        // strange phenomenon: when converting the xmlyt files back to brlyt using benzin, sometimes the first byte is not correctly written. This fixes it as the first byte must be an 'R'.
                        await Task.Delay(500);
                        using (var stream = File.OpenWrite(brlytFile))
                        {
                            stream.Seek(0, SeekOrigin.Begin);
                            stream.WriteByte((byte)'R');
                        }
                    });
                    injectMapIconsInBrlytTasks.Add(task4);
                }
                await Task.WhenAll(injectMapIconsInBrlytTasks).ConfigureAwait(false);
                await Task.WhenAll(convertPngFileTasks).ConfigureAwait(false);
                source.Cancel();
                await fakeProgressTask.ConfigureAwait(false);
            }

            progress.Report("Pack game_sequence files...");
            using (CancellationTokenSource source = new CancellationTokenSource())
            {
                // start fake progress
                var fakeProgressTask = ProgressInfo.makeFakeProgress(ProgressInfo.makeSubProgress(progress, 66, 100), source.Token);

                // pack the arc files
                List<Task<string>> packArcFileTasks = new List<Task<string>>();
                foreach (var entry in gameSequenceExtractPaths)
                {
                    string gameSequencePath = entry.Key;
                    string gameSequenceExtractPath = entry.Value;
                    string gameSequencePackPath = gameSequencePackPaths[gameSequencePath];
                    packArcFileTasks.Add(ExeWrapper.packDfolderToArc(gameSequenceExtractPath, gameSequencePackPath, ct, ProgressInfo.makeNoProgress(progress)));
                }
                await Task.WhenAll(packArcFileTasks).ConfigureAwait(false);
                source.Cancel();
                await fakeProgressTask.ConfigureAwait(false);
            }

            progress.Report(100);

            return true;
        }
    }
}
