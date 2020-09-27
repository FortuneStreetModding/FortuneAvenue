using FSEditor.Exceptions;
using FSEditor.FSData;
using FSEditor.MapDescriptor;
using MiscUtil.Conversion;
using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomStreetManager
{
    public class PatchProcess
    {
        public FileSet fileSet;
        public List<MapDescriptor> mapDescriptors;
        public MainDol mainDol;
        public Dictionary<string, UI_Message> ui_messages = new Dictionary<string, UI_Message>();

        private string toTmpDirectory(string file, string origin, string extensionWithDot)
        {
            if (origin == null)
            {
                origin = "";
            }
            else
            {
                origin = "." + Path.GetFileNameWithoutExtension(origin) + ".";
            }
            return Path.Combine(Directory.GetCurrentDirectory(), "tmp", Path.GetFileNameWithoutExtension(file) + origin + extensionWithDot);
        }
        private string toRiivolutionDirectory(string file)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "tmp", file);
        }
        private IProgress<ProgressInfo> makeSubProgress(IProgress<ProgressInfo> progress, int minProgress, int maxProgress)
        {
            return new Progress<ProgressInfo>(progressInfo =>
            {
                progressInfo.progress = ProgressInfo.lerp(minProgress, maxProgress, progressInfo.progress);
                progress.Report(progressInfo);
            });
        }

        public async Task<List<MapDescriptor>> ReloadWbfsIsoFile(string inputWbfsIso, IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            progress?.Report(0);
            progress?.Report("Extract relevant files from iso/wbfs...");
            if (string.IsNullOrWhiteSpace(inputWbfsIso) || inputWbfsIso.ToLower() == "none")
            {
                throw new ArgumentNullException("Can't load wbfs or iso file as the input file name is not set.");
            }

            fileSet = await ExeWrapper.extractFiles(inputWbfsIso, ct, ProgressInfo.makeSubProgress(progress, 0, 40));
            progress?.Report("Detect the sections in main.dol file...");
            List<MainDolSection> sections = await ExeWrapper.readSections(fileSet.main_dol, ct, ProgressInfo.makeSubProgress(progress, 40, 45));
            mainDol = new MainDol(sections);

            progress?.Report("Read data from main.dol file...");
            using (var stream = File.OpenRead(fileSet.main_dol))
            {
                EndianBinaryReader binReader = new EndianBinaryReader(EndianBitConverter.Big, stream);
                mapDescriptors = mainDol.readMainDol(binReader);

                progress?.Report(47);
                progress?.Report("Read localization files...");
                reloadUIMessages(mapDescriptors, mainDol.data);
            }

            progress?.Report(50);
            progress?.Report("Extract Iso...");

            string cacheDirectory = await ExeWrapper.extractFullIsoAsync(inputWbfsIso, ct, ProgressInfo.makeSubProgress(progress, 50, 100));

            fileSet.game_sequence_arc[Locale.EN] = Path.Combine(cacheDirectory, "DATA", "files", "game", "langEN", "game_sequence_EN.arc");
            fileSet.game_sequence_arc[Locale.DE] = Path.Combine(cacheDirectory, "DATA", "files", "game", "langDE", "game_sequence_DE.arc");
            fileSet.game_sequence_arc[Locale.ES] = Path.Combine(cacheDirectory, "DATA", "files", "game", "langES", "game_sequence_ES.arc");
            fileSet.game_sequence_arc[Locale.FR] = Path.Combine(cacheDirectory, "DATA", "files", "game", "langFR", "game_sequence_FR.arc");
            fileSet.game_sequence_arc[Locale.IT] = Path.Combine(cacheDirectory, "DATA", "files", "game", "langIT", "game_sequence_IT.arc");
            fileSet.game_sequence_arc[Locale.UK] = Path.Combine(cacheDirectory, "DATA", "files", "game", "langUK", "game_sequence_UK.arc");
            fileSet.game_sequence_arc[Locale.JP] = Path.Combine(cacheDirectory, "DATA", "files", "game", "game_sequence.arc");
            fileSet.game_sequence_wifi_arc[Locale.EN] = Path.Combine(cacheDirectory, "DATA", "files", "game", "langEN", "game_sequence_wifi_EN.arc");
            fileSet.game_sequence_wifi_arc[Locale.DE] = Path.Combine(cacheDirectory, "DATA", "files", "game", "langDE", "game_sequence_wifi_DE.arc");
            fileSet.game_sequence_wifi_arc[Locale.ES] = Path.Combine(cacheDirectory, "DATA", "files", "game", "langES", "game_sequence_wifi_ES.arc");
            fileSet.game_sequence_wifi_arc[Locale.FR] = Path.Combine(cacheDirectory, "DATA", "files", "game", "langFR", "game_sequence_wifi_FR.arc");
            fileSet.game_sequence_wifi_arc[Locale.IT] = Path.Combine(cacheDirectory, "DATA", "files", "game", "langIT", "game_sequence_wifi_IT.arc");
            fileSet.game_sequence_wifi_arc[Locale.UK] = Path.Combine(cacheDirectory, "DATA", "files", "game", "langUK", "game_sequence_wifi_UK.arc");
            fileSet.game_sequence_wifi_arc[Locale.JP] = Path.Combine(cacheDirectory, "DATA", "files", "game", "game_sequence_wifi.arc");

            progress?.Report(100);
            progress?.Report("Loaded successfully.");

            return mapDescriptors;
        }
        private string reloadUIMessages(List<MapDescriptor> mapDescriptors, AddressConstants data)
        {
            string warnings = "";
            foreach (string locale in Locale.ALL_WITHOUT_UK)
            {
                ui_messages[fileSet.ui_message_csv[locale]] = new UI_Message(fileSet.ui_message_csv[locale], locale);
            }
            ui_messages[fileSet.ui_message_csv[Locale.UK]] = ui_messages[fileSet.ui_message_csv[Locale.EN]];
            foreach (MapDescriptor mapDescriptor in mapDescriptors)
            {
                foreach (string locale in Locale.ALL_WITHOUT_UK)
                {
                    mapDescriptor.Name[locale] = ui_messages[fileSet.ui_message_csv[locale]].get(mapDescriptor.Name_MSG_ID);
                    mapDescriptor.Desc[locale] = ui_messages[fileSet.ui_message_csv[locale]].get(mapDescriptor.Desc_MSG_ID);
                }
                warnings += mapDescriptor.readFrbFileInfo(fileSet.param_folder);
            }
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

        public void patchMainDol(string fileMainDol, IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            var tempMainDol = fileMainDol + ".tmp";
            try
            {
                File.Copy(fileMainDol, tempMainDol, true);

                // expand dol if not already expanded
                /*if (mainDol.toFileAddress(0x80001800) == -1)
                {
                    WitWrapper.createNewTextSection(tempMainDol, 0x80001800, 0x1800);
                    mainDol.setSections(WitWrapper.readSections(tempMainDol));
                }*/

                using (Stream baseStream = File.Open(tempMainDol, FileMode.Open))
                {
                    EndianBinaryWriter stream = new EndianBinaryWriter(EndianBitConverter.Big, baseStream);
                    mainDol.writeMainDol(stream, mapDescriptors);

                    progress.Report("Amount of free space used in main.dol: " + mainDol.totalBytesWritten + " bytes");
                    progress.Report("Amount of free space left in main.dol: " + mainDol.totalBytesLeft + " bytes");
                }
                // everything went through successfully, use the temp file
                File.Copy(tempMainDol, fileMainDol, true);
            }
            finally
            {
                File.Delete(tempMainDol);
            }
        }

        public async Task<bool> saveWbfsIso(string inputFile, string outputFile, bool cleanup, IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            progress?.Report(new ProgressInfo(0, "Writing data to main.dol..."));
            patchMainDol(fileSet.main_dol, ProgressInfo.makeSubProgress(progress, 0, 5), ct);

            // lets get to the map icons
            _ = await injectMapIcons(ProgressInfo.makeSubProgress(progress, 5, 45), ct);

            progress.Report(new ProgressInfo(46, "Writing localization files..."));
            foreach (var entry in ui_messages)
            {
                string fileSet_ui_message_csv = entry.Key;
                UI_Message ui_message = entry.Value;
                ui_message.set(mapDescriptors);
                ui_message.writeToFile(fileSet_ui_message_csv);
            }
            progress.Report(new ProgressInfo(47, "Copying the modified files to be packed into the image..."));
            ExeWrapper.copyRelevantFilesForPacking(fileSet, inputFile);
            progress.Report(new ProgressInfo(48, "Packing ISO/WBFS file..."));
            await ExeWrapper.packFullIso(inputFile, outputFile, ct, ProgressInfo.makeSubProgress(progress, 50, 100));

            if (cleanup)
            {
                ExeWrapper.cleanup(inputFile);
            }
            progress.Report(new ProgressInfo(100, "Done."));
            return true;
        }

        /**
         * Useful for when a command line job does not report progress but we still want to show the user that work is being done.
         */
        private async Task<bool> makeFakeProgress(IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            for (int i = 1; i <= 100; i++)
            {
                try
                {
                    await Task.Delay(i * 100, ct);
                }
                catch (TaskCanceledException e)
                {
                    return true;
                }
                if (ct.IsCancellationRequested)
                    return true;
                progress.Report(i);
            }
            return true;
        }

        private async Task<bool> injectMapIcons(IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            progress.Report(new ProgressInfo(5, "Extract game_sequence files..."));
            // throw together the game_sequence and game_sequence_wifi files
            List<string> gameSequenceFiles = new List<string>();
            gameSequenceFiles.AddRange(fileSet.game_sequence_arc.Values);
            gameSequenceFiles.AddRange(fileSet.game_sequence_wifi_arc.Values);

            using (CancellationTokenSource source = new CancellationTokenSource())
            {
                // start fake progress
                var fakeProgressTask = makeFakeProgress(ProgressInfo.makeSubProgress(progress, 0, 25), source.Token);

                // extract the arc files
                List<Task<string>> extractArcFileTasks = new List<Task<string>>();
                foreach (var gameSequenceFile in gameSequenceFiles)
                {
                    extractArcFileTasks.Add(ExeWrapper.extractArcFile(gameSequenceFile, toTmpDirectory(gameSequenceFile, null, "_d"), ct, ProgressInfo.makeNoProgress(progress)));
                }
                await Task.WhenAll(extractArcFileTasks);
                source.Cancel();
                await fakeProgressTask;
            }
            using (CancellationTokenSource source = new CancellationTokenSource())
            {
                // start fake progress
                var fakeProgressTask = makeFakeProgress(ProgressInfo.makeSubProgress(progress, 25, 75), source.Token);

                // convert the png files to tpl and copy them to the correct location
                Dictionary<string, string> mapIconToTplName = new Dictionary<string, string>();
                List<Task> convertPngFileTasks = new List<Task>();
                foreach (var mapDescriptor in mapDescriptors)
                {
                    var mapIcon = mapDescriptor.MapIcon;
                    if (string.IsNullOrEmpty(mapIcon))
                        continue;
                    var mapIconPng = toTmpDirectory(mapIcon, null, ".png");
                    var mapIconTpl = toTmpDirectory(mapIcon, null, ".tpl");
                    var tplName = Ui_menu_19_00a_XMLYT.constructMapIconTplName(mapIcon);
                    if (!mapIconToTplName.ContainsKey(mapIcon))
                    {
                        mapIconToTplName.Add(mapIcon, tplName);
                    }
                    if (File.Exists(mapIconPng))
                    {
                        Task task1 = ExeWrapper.convertPngToTpl(mapIconPng, mapIconTpl, ct, ProgressInfo.makeNoProgress(progress));
                        Task task2 = task1.ContinueWith((t1) =>
                        {
                            foreach (var gameSequenceFile in gameSequenceFiles)
                            {
                                var gameSequenceExtractedDir = toTmpDirectory(gameSequenceFile, null, "_d");
                                var mapIconTplCopy = Path.Combine(gameSequenceExtractedDir, "arc", "timg", tplName);
                                File.Copy(mapIconTpl, mapIconTplCopy);
                            }
                        });
                        convertPngFileTasks.Add(task2);
                    }
                }


                // convert the brlyt files to xmlyt, inject the map icons and convert it back
                List<Task> injectMapIconsInBrlytTasks = new List<Task>();
                foreach (var gameSequenceFile in gameSequenceFiles)
                {
                    var gameSequenceExtractedDir = toTmpDirectory(gameSequenceFile, null, "_d");
                    var brlytFile = Path.Combine(gameSequenceExtractedDir, "arc", "blyt", "ui_menu_19_00a.brlyt");
                    var xmlytFile = toTmpDirectory(brlytFile, gameSequenceExtractedDir, "xmlyt");
                    Task task1 = ExeWrapper.convertBryltToXmlyt(brlytFile, xmlytFile, ct, ProgressInfo.makeNoProgress(progress));
                    Task task2 = task1.ContinueWith((t1) => Ui_menu_19_00a_XMLYT.injectMapIcons(xmlytFile, mapIconToTplName));
                    Task task3 = task2.ContinueWith((t2) => ExeWrapper.convertXmlytToBrylt(xmlytFile, brlytFile, ct, ProgressInfo.makeNoProgress(progress)));
                    // Task task4 = task3.ContinueWith((t3) => File.Delete(xmlytFile));
                    injectMapIconsInBrlytTasks.Add(task3);
                }
                await Task.WhenAll(injectMapIconsInBrlytTasks);
                await Task.WhenAll(convertPngFileTasks);
                source.Cancel();
                await fakeProgressTask;
            }

            // delete the unneeded png and tpl files
            /*
            foreach (var mapDescriptor in mapDescriptors)
            {
                var mapIcon = mapDescriptor.MapIcon;
            if (string.IsNullOrEmpty(mapIcon))
                    continue;
                var mapIconPng = toTmpDirectory(mapIcon, null, ".png");
                var mapIconTpl = toTmpDirectory(mapIcon, null, ".tpl");
                if (File.Exists(mapIconPng))
                {
                    File.Delete(mapIconPng);
                }
                if (File.Exists(mapIconTpl))
                {
                    File.Delete(mapIconTpl);
                }
            }*/

            progress.Report(new ProgressInfo(100, "Done"));

            return true;
        }

        public async Task<string> exportMd(string selectedFile, MapDescriptor mapDescriptor, bool overwrite, IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            var directory = Path.GetDirectoryName(selectedFile);
            string fileNameMd = selectedFile;
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
                byte[] content = Encoding.UTF8.GetBytes(mapDescriptor.generateMapDescriptorFileContent());
                await fs.WriteAsync(content, 0, content.Length);
            }
            extractedFiles += fileNameMd + Environment.NewLine;

            progress?.Report(new ProgressInfo(50, "Copying frb files..."));

            File.Copy(Path.Combine(fileSet.param_folder, mapDescriptor.FrbFile1 + ".frb"), fileNameFrb1);
            extractedFiles += fileNameFrb1 + Environment.NewLine;
            if (fileNameFrb2 != null)
            {
                File.Copy(Path.Combine(fileSet.param_folder, mapDescriptor.FrbFile2 + ".frb"), fileNameFrb2);
                extractedFiles += fileNameFrb2 + Environment.NewLine;
            }
            if (fileNameFrb3 != null)
            {
                File.Copy(Path.Combine(fileSet.param_folder, mapDescriptor.FrbFile3 + ".frb"), fileNameFrb3);
                extractedFiles += fileNameFrb3 + Environment.NewLine;
            }
            if (fileNameFrb4 != null)
            {
                File.Copy(Path.Combine(fileSet.param_folder, mapDescriptor.FrbFile4 + ".frb"), fileNameFrb4);
                extractedFiles += fileNameFrb4 + Environment.NewLine;
            }
            progress?.Report(new ProgressInfo(100, "Done. Generated md file and extracted frb file(s):"));
            return extractedFiles;
        }

        public void importMd(string selectedFile, MapDescriptor mapDescriptor, IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            MapDescriptor mapDescriptorImport = new MapDescriptor();

            progress.Report(new ProgressInfo(0, "Parse Map Descriptor File..."));

            var mapDescriptorImportFile = selectedFile;
            var dir = Path.GetDirectoryName(selectedFile);

            var name = Path.GetFileNameWithoutExtension(mapDescriptorImportFile);
            if (name.ToLower() == "readme")
            {
                name = Path.GetFileName(dir);
            }

            mapDescriptorImport.readMapDescriptorFromFile(mapDescriptorImportFile, name);

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
            var importFileTmp = Path.Combine(fileSet.param_folder, frbFileName + ".frb");
            File.Copy(importFile, importFileTmp, true);

            progress.Report("Imported " + importFile);

            frbFileName = mapDescriptorImport.FrbFile2;
            if (frbFileName != null)
            {
                importFile = Path.Combine(dir, frbFileName + ".frb");
                importFileTmp = Path.Combine(fileSet.param_folder, frbFileName + ".frb");
                File.Copy(importFile, importFileTmp, true);
                progress.Report("Imported " + importFile);
            }
            frbFileName = mapDescriptorImport.FrbFile3;
            if (frbFileName != null)
            {
                importFile = Path.Combine(dir, frbFileName + ".frb");
                importFileTmp = Path.Combine(fileSet.param_folder, frbFileName + ".frb");
                File.Copy(importFile, importFileTmp, true);
                progress.Report("Imported " + importFile);
            }
            frbFileName = mapDescriptorImport.FrbFile4;
            if (frbFileName != null)
            {
                importFile = Path.Combine(dir, frbFileName + ".frb");
                importFileTmp = Path.Combine(fileSet.param_folder, frbFileName + ".frb");
                File.Copy(importFile, importFileTmp, true);
                progress.Report("Imported " + importFile);
            }
            var mapIcon = mapDescriptorImport.MapIcon;
            if (mapIcon != null)
            {
                importFile = Path.Combine(dir, mapIcon + ".png");
                importFileTmp = toTmpDirectory(importFile, null, ".png");
                File.Copy(importFile, importFileTmp, true);
                progress.Report("Imported " + importFile);
            }
            mapDescriptor.set(mapDescriptorImport);
            mapDescriptor.Dirty = true;
            progress.Report(new ProgressInfo(100, "Done."));
        }
    }
}
