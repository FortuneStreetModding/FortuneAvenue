using FSEditor.Exceptions;
using FSEditor.FSData;
using MiscUtil.Conversion;
using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CustomStreetMapManager
{
    public abstract partial class PatchProcess
    {
        public static async Task<bool> Save(string inputFile, string outputFile, List<MapDescriptor> mapDescriptors, bool patchWiimmfi, IProgress<ProgressInfo> progress, CancellationToken ct, string cachePath = null, string riivPath = null, string tmpPath = null)
        {
            var packIso = IsImageFileExtension(outputFile);
            outputFile = DoPathCorrections(outputFile, true);
            inputFile = DoPathCorrections(inputFile, false);
            cachePath = Path.GetFullPath(GetCachePath(inputFile, cachePath));
            if (IsImageFileExtension(cachePath))
                throw new ArgumentException("cachePath must be a valid extracted fortune street game disc directory");
            var cacheFileSet = new DataFileSet(cachePath);
            var riivFileSet = new DataFileSet(GetDefaultRiivPath(riivPath));
            var tmpFileSet = new DataFileSet(GetDefaultTmpPath(tmpPath));

            progress?.Report(new ProgressInfo(0, "Writing localization files..."));
            WriteLocalizationFiles(mapDescriptors, cacheFileSet, riivFileSet, patchWiimmfi && packIso);

            progress?.Report(new ProgressInfo(5, "Writing main.dol..."));
            await PatchMainDolAsync(mapDescriptors, cacheFileSet, riivFileSet, ProgressInfo.makeSubProgress(progress, 0, 6), ct);

            // lets get to the map icons
            await InjectMapIcons(mapDescriptors, cacheFileSet, tmpFileSet, riivFileSet, ProgressInfo.makeSubProgress(progress, 7, 40), ct).ConfigureAwait(false);
            await Task.Delay(500);

            var packIsoInputPath = cacheFileSet.rootDir;
            using (CancellationTokenSource source = new CancellationTokenSource())
            {
                // start fake progress
                progress.Report(new ProgressInfo(45, "Copying the modified files..."));
                var fakeProgressTask = ProgressInfo.makeFakeProgress(ProgressInfo.makeSubProgress(progress, 45, packIso ? 60 : 99), source.Token);

                if (packIso)
                {
                    if (ShouldKeepCache(inputFile))
                    {
                        packIsoInputPath = Path.Combine(tmpFileSet.rootDir, Path.GetFileNameWithoutExtension("pack_" + outputFile));
                        DirectoryCopy(cacheFileSet.rootDir, packIsoInputPath, true, true, ProgressInfo.makeNoProgress(progress), ct);
                        DirectoryCopy(riivFileSet.rootDir, packIsoInputPath, true, true, ProgressInfo.makeNoProgress(progress), ct);
                    }
                    else
                    {
                        DirectoryCopy(riivFileSet.rootDir, packIsoInputPath, true, true, ProgressInfo.makeNoProgress(progress), ct);
                    }
                }
                else
                {
                    if (cacheFileSet.rootDir != outputFile)
                    {
                        DirectoryCopy(cacheFileSet.rootDir, outputFile, true, true, ProgressInfo.makeNoProgress(progress), ct);
                    }
                    DirectoryCopy(riivFileSet.rootDir, outputFile, true, true, ProgressInfo.makeNoProgress(progress), ct);
                }
                source.Cancel();
                await fakeProgressTask.ConfigureAwait(false);
            }

            await Task.Delay(500);

            if (packIso)
            {
                progress.Report(new ProgressInfo(60, "Packing ISO/WBFS file..."));
                await ExeWrapper.packFullIso(packIsoInputPath, outputFile, patchWiimmfi, ct, ProgressInfo.makeSubProgress(progress, 60, 100)).ConfigureAwait(true);
                if (patchWiimmfi)
                {
                    await Task.Delay(500);
                    progress.Report("Applying Wiimmfi...");
                    await ExeWrapper.applyWiimmfi(outputFile, ct, ProgressInfo.makeNoProgress(progress)).ConfigureAwait(false);
                }
            }
            else if (patchWiimmfi)
            {
                progress.Report("Warning: Wiimmfi is not applied as it can only be patched when packing into an iso/wbfs image.");
            }

            await Task.Delay(500);

            progress.Report(new ProgressInfo(100, "Done."));

            return true;
        }

        private static void WriteLocalizationFiles(List<MapDescriptor> mapDescriptors, DataFileSet fileSet, DataFileSet riivFileSet, bool patchWiimmfi)
        {
            // Key = locale, Value = file contents
            var ui_messages = new Dictionary<string, UI_Message>();
            foreach (string locale in Locale.ALL)
            {
                ui_messages[locale] = new UI_Message(fileSet.ui_message_csv[locale], locale);
            }
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
            uint msgId = 25000;
            foreach (var mapDescriptor in mapDescriptors)
            {
                mapDescriptor.Name_MSG_ID = msgId++;
                foreach (var entry in ui_messages)
                {
                    var locale = entry.Key;
                    var ui_message = entry.Value;
                    // write EN messages to the UK file as well (we are not differentiating here)
                    if (locale == Locale.UK)
                        locale = Locale.EN;
                    // if there is no localization for this locale, use the english variant as default
                    if (!mapDescriptor.Name.ContainsKey(locale) || string.IsNullOrWhiteSpace(mapDescriptor.Name[locale]))
                    {
                        ui_message.set(mapDescriptor.Name_MSG_ID, mapDescriptor.Name[Locale.EN]);
                    }
                    else
                    {
                        ui_message.set(mapDescriptor.Name_MSG_ID, mapDescriptor.Name[locale]);
                    }
                }
                mapDescriptor.Desc_MSG_ID = msgId++;
                foreach (var entry in ui_messages)
                {
                    var locale = entry.Key;
                    var ui_message = entry.Value;
                    // write EN messages to the UK file as well (we are not differentiating here)
                    if (locale == Locale.UK)
                        locale = Locale.EN;
                    // if there is no localization for this locale, use the english variant as default
                    if (!mapDescriptor.Desc.ContainsKey(locale) || string.IsNullOrWhiteSpace(mapDescriptor.Desc[locale]))
                    {
                        ui_message.set(mapDescriptor.Desc_MSG_ID, mapDescriptor.Desc[Locale.EN]);
                    }
                    else
                    {
                        ui_message.set(mapDescriptor.Desc_MSG_ID, mapDescriptor.Desc[locale]);
                    }
                }
            }
            // text replace Nintendo WFC -> Wiimmfi
            if (patchWiimmfi)
            {
                foreach (var entry in ui_messages)
                {
                    var locale = entry.Key;
                    var ui_message = entry.Value;
                    var keys = ui_message.getMap().Keys.ToList();
                    foreach (var id in keys)
                    {
                        var text = ui_message.get(id);

                        // need to use Regex, because there are different types of whitespaces in the messages (some are U+0020 while others are U+00A0)
                        if (locale == Locale.DE)
                        {
                            text = Regex.Replace(text, @"die\sNintendo\sWi-Fi\sConnection", "Wiimmfi", RegexOptions.IgnoreCase);
                            text = Regex.Replace(text, @"der\sNintendo\sWi-Fi\sConnection", "Wiimmfi", RegexOptions.IgnoreCase);
                            text = Regex.Replace(text, @"zur\sNintendo\sWi-Fi\sConnection", "Wiimmfi", RegexOptions.IgnoreCase);
                        }
                        if (locale == Locale.FR)
                        {
                            text = Regex.Replace(text, @"Wi-Fi\sNintendo", "Wiimmfi", RegexOptions.IgnoreCase);
                            text = Regex.Replace(text, @"CWF\sNintendo", "Wiimmfi", RegexOptions.IgnoreCase);
                            text = Regex.Replace(text, @"Connexion\sWi-Fi\sNintendo", "Wiimmfi", RegexOptions.IgnoreCase);
                        }
                        if (locale == Locale.ES)
                        {
                            text = Regex.Replace(text, @"Conexión\sWi-Fi\sde\sNintendo", "Wiimmfi", RegexOptions.IgnoreCase);
                            text = Regex.Replace(text, @"CWF\sde\sNintendo", "Wiimmfi", RegexOptions.IgnoreCase);
                            text = Regex.Replace(text, @"Conexión\sWi-Fi\sde<n>Nintendo", "Wiimmfi<n>", RegexOptions.IgnoreCase);
                            text = Regex.Replace(text, @"Conexión<n>Wi-Fi\sde\sNintendo", "Wiimmfi<n>", RegexOptions.IgnoreCase);
                        }
                        if (locale == Locale.JP)
                        {
                            text = text.Replace("Ｗｉ－Ｆｉ", "Ｗｉｉｍｍｆｉ", StringComparison.OrdinalIgnoreCase);
                        }
                        text = Regex.Replace(text, @"Nintendo\sWi-Fi\sConnection", "Wiimmfi", RegexOptions.IgnoreCase);
                        text = Regex.Replace(text, @"Nintendo\sWFC", "Wiimmfi", RegexOptions.IgnoreCase);
                        text = text.Replace("support.nintendo.com", "https://wiimmfi.de/error", StringComparison.OrdinalIgnoreCase);
                        ui_message.set(id, text);
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

        private static async Task PatchMainDolAsync(List<MapDescriptor> mapDescriptors, DataFileSet cacheFileSet, DataFileSet riivFileSet, IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(riivFileSet.main_dol));
            File.Copy(cacheFileSet.main_dol, riivFileSet.main_dol, true);

            // expand dol if not already expanded
            // ehmm.. actually lets not do this -> range 0x80001800 till 0x80003000 is already used by gecko codes
            //if (mainDol.toFileAddress(0x80001800) == -1)
            //{
            //    await ExeWrapper.createNewTextSection(riivFileSet.main_dol, 0x80001800, 0x1800, ct, progress);
            //    mainDol.setSections(await ExeWrapper.readSections(riivFileSet.main_dol, ct, progress));
            //}

            progress?.Report("Detect the sections in main.dol file...");
            List<AddressSection> sections = await ExeWrapper.readSections(riivFileSet.main_dol, ct, ProgressInfo.makeNoProgress(progress)).ConfigureAwait(false);
            MainDol mainDol;
            using (var stream = File.OpenRead(riivFileSet.main_dol))
            {
                EndianBinaryReader binReader = new EndianBinaryReader(EndianBitConverter.Big, stream);
                mainDol = new MainDol(binReader, sections, progress);
            }

            using (Stream baseStream = File.Open(riivFileSet.main_dol, FileMode.Open))
            {
                try
                {
                    EndianBinaryWriter stream = new EndianBinaryWriter(EndianBitConverter.Big, baseStream);
                    mainDol.writeMainDol(stream, mapDescriptors, progress);
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

        private static async Task<bool> InjectMapIcons(List<MapDescriptor> mapDescriptors, DataFileSet cacheFileSet, DataFileSet tmpFileSet, DataFileSet riivFileSet, IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            // first check if we need to inject any map icons in the first place. We do not need to if only vanilla map icons are used.
            bool allMapIconsVanilla = true;
            foreach (var mapDescriptor in mapDescriptors)
            {
                var mapIcon = mapDescriptor.MapIcon;
                if (string.IsNullOrEmpty(mapIcon))
                    continue;
                if (!VanillaDatabase.getVanillaTpl(mapIcon).Any())
                {
                    allMapIconsVanilla = false;
                    break;
                }
            }
            if (allMapIconsVanilla)
            {
                return true;
            }

            progress.Report("Extract game_sequence files...");

            // setup the directories for the game sequence files

            /** maps the path of the various variants of the game_sequenceXXXXXXXX.arc files to their respective extraction path in the tmp directory */
            var gameSequenceExtractPaths = new Dictionary<string, string>();
            /** maps the path of the various variants of the game_sequenceXXXXXXXX.arc files to their respective temporary path for the converted xmlyt base path */
            var gameSequenceToXmlytBasePaths = new Dictionary<string, string>();
            /** maps the path of the various variants of the game_sequenceXXXXXXXX.arc files to their target path where they will be packed again */
            var gameSequencePackPaths = new Dictionary<string, string>();
            foreach (var entry in cacheFileSet.game_sequence_arc)
            {
                var locale = entry.Key;
                var gameSequencePath = entry.Value;

                var extractPath = Path.Combine(tmpFileSet.rootDir, Path.GetFileNameWithoutExtension(gameSequencePath));
                Directory.CreateDirectory(Path.GetDirectoryName(extractPath));
                gameSequenceExtractPaths.Add(gameSequencePath, extractPath);

                var xmlytPath = Path.Combine(tmpFileSet.rootDir, Path.GetFileNameWithoutExtension(gameSequencePath) + ".");
                Directory.CreateDirectory(Path.GetDirectoryName(xmlytPath));
                gameSequenceToXmlytBasePaths.Add(gameSequencePath, xmlytPath);

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

                var xmlytPath = Path.Combine(tmpFileSet.rootDir, Path.GetFileNameWithoutExtension(gameSequencePath) + ".");
                Directory.CreateDirectory(Path.GetDirectoryName(xmlytPath));
                gameSequenceToXmlytBasePaths.Add(gameSequencePath, xmlytPath);

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

                    if (VanillaDatabase.getVanillaTpl(mapIcon).Any())
                    {
                        // its a vanilla map icon -> dont convert and inject it
                        VanillaDatabase.getVanillaTpl(mapIcon).IfPresent(value => mapIconToTplName[mapIcon] = value);
                    }
                    else
                    {
                        var mapIconPng = Path.Combine(tmpFileSet.param_folder, mapIcon + ".png");
                        var mapIconTpl = Path.ChangeExtension(mapIconPng, ".tpl");
                        var tplName = Ui_menu_19_00a.constructMapIconTplName(mapIcon);
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
                    var brlytFile = Path.Combine(gameSequenceExtractPath, "arc", "blyt", "ui_menu_19_00a.brlyt");
                    string xmlytFile = gameSequenceToXmlytBasePaths[gameSequencePath] + Path.GetFileNameWithoutExtension(brlytFile) + ".xmlyt";
                    Task task1 = ExeWrapper.convertBryltToXmlyt(brlytFile, xmlytFile, ct, ProgressInfo.makeNoProgress(progress));
                    Task task2 = task1.ContinueWith(async (t1) => { await t1.ConfigureAwait(false); Ui_menu_19_00a.injectMapIconsLayout(xmlytFile, mapIconToTplName); });
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
                        // wait till the handle has been disposed properly
                        await Task.Delay(500);
                    });
                    injectMapIconsInBrlytTasks.Add(task4);
                }
                // convert the brlan files to xmlan, inject the map icons and convert it back
                List<Task> injectMapIconsInBrlanTasks = new List<Task>();
                foreach (var entry in gameSequenceExtractPaths)
                {
                    string gameSequencePath = entry.Key;
                    string gameSequenceExtractPath = entry.Value;
                    foreach (var brlanFile in Directory.GetFiles(Path.Combine(gameSequenceExtractPath, "arc", "anim"), "ui_menu_19_00a_Tag_*.brlan"))
                    {
                        string xmlanFile = gameSequenceToXmlytBasePaths[gameSequencePath] + Path.GetFileNameWithoutExtension(brlanFile) + ".xmlan";
                        Task task1 = ExeWrapper.convertBryltToXmlyt(brlanFile, xmlanFile, ct, ProgressInfo.makeNoProgress(progress));
                        Task task2 = task1.ContinueWith(async (t1) => { await t1.ConfigureAwait(false); Ui_menu_19_00a.injectMapIconsAnimation(xmlanFile, mapIconToTplName); });
                        Task task3 = task2.ContinueWith(async (t2) => { await t2.ConfigureAwait(false); await ExeWrapper.convertXmlytToBrylt(xmlanFile, brlanFile, ct, ProgressInfo.makeNoProgress(progress)); });
                        Task task4 = task3.ContinueWith(async (t3) =>
                        {
                            await t3;
                            // strange phenomenon: when converting the xmlyt files back to brlyt using benzin, sometimes the first byte is not correctly written. This fixes it as the first byte must be an 'R'.
                            await Task.Delay(500);
                            using (var stream = File.OpenWrite(brlanFile))
                            {
                                stream.Seek(0, SeekOrigin.Begin);
                                stream.WriteByte((byte)'R');
                            }
                            // wait till the handle has been disposed properly
                            await Task.Delay(500);
                        });
                        injectMapIconsInBrlanTasks.Add(task4);
                    }
                }
                await Task.WhenAll(injectMapIconsInBrlytTasks).ConfigureAwait(false);
                await Task.WhenAll(injectMapIconsInBrlanTasks).ConfigureAwait(false);
                await Task.WhenAll(convertPngFileTasks).ConfigureAwait(false);
                source.Cancel();
                await fakeProgressTask.ConfigureAwait(false);
            }
            await Task.Delay(1000);
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
            await Task.Delay(1000);
            progress.Report(100);

            return true;
        }
    }
}
