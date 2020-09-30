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
            progress?.Report(new ProgressInfo(0, "Writing data to main.dol..."));

            patchMainDol(ProgressInfo.makeSubProgress(progress, 0, 1), ct);

            // lets get to the map icons
            await injectMapIcons(ProgressInfo.makeSubProgress(progress, 1, 40), ct).ConfigureAwait(false);

            progress.Report(new ProgressInfo(40, "Writing localization files..."));
            foreach (var entry in ui_messages)
            {
                string locale = entry.Key;
                UI_Message ui_message = entry.Value;
                ui_message.set(mapDescriptors);
                Directory.CreateDirectory(Path.GetDirectoryName(riivFileSet.ui_message_csv[locale]));
                ui_message.writeToFile(riivFileSet.ui_message_csv[locale]);
            }

            /* using (CancellationTokenSource source = new CancellationTokenSource())
             {
                 // start fake progress
                 progress.Report(new ProgressInfo(45, "Copying the modified files to be packed into the image..."));
                 var fakeProgressTask = ProgressInfo.makeFakeProgress(ProgressInfo.makeSubProgress(progress, 45, 60), source.Token);*/

            var frbFilesToBeCopied = new HashSet<string>();
            foreach (MapDescriptor mapDescriptor in mapDescriptors)
            {
                frbFilesToBeCopied.Add(mapDescriptor.FrbFile1 + ".frb");
                if (!string.IsNullOrEmpty(mapDescriptor.FrbFile2))
                    frbFilesToBeCopied.Add(mapDescriptor.FrbFile2 + ".frb");
                if (!string.IsNullOrEmpty(mapDescriptor.FrbFile3))
                    frbFilesToBeCopied.Add(mapDescriptor.FrbFile3 + ".frb");
                if (!string.IsNullOrEmpty(mapDescriptor.FrbFile4))
                    frbFilesToBeCopied.Add(mapDescriptor.FrbFile4 + ".frb");
            }

            riivFileSet.copy(frbFilesToBeCopied, cacheFileSet, true);
            /*source.Cancel();
            await fakeProgressTask.ConfigureAwait(false);
        }*/

            progress.Report(new ProgressInfo(60, "Packing ISO/WBFS file..."));
            await ExeWrapper.packFullIso(inputFile, outputFile, ct, ProgressInfo.makeSubProgress(progress, 60, 100)).ConfigureAwait(false);

            // this.cleanUp(cleanUp, cleanUp);

            progress.Report(new ProgressInfo(100, "Done."));
            return true;
        }

        private void patchMainDol(IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(riivFileSet.main_dol));
            File.Copy(cacheFileSet.main_dol, riivFileSet.main_dol, true);

            // expand dol if not already expanded
            /*if (mainDol.toFileAddress(0x80001800) == -1)
            {
                WitWrapper.createNewTextSection(tempMainDol, 0x80001800, 0x1800);
                mainDol.setSections(WitWrapper.readSections(tempMainDol));
            }*/

            using (Stream baseStream = File.Open(riivFileSet.main_dol, FileMode.Open))
            {
                EndianBinaryWriter stream = new EndianBinaryWriter(EndianBitConverter.Big, baseStream);
                mainDol.writeMainDol(stream, mapDescriptors);

                progress.Report("Amount of free space used in main.dol: " + mainDol.totalBytesWritten + " bytes");
                progress.Report("Amount of free space left in main.dol: " + mainDol.totalBytesLeft + " bytes");
            }
        }

        private async Task<bool> injectMapIcons(IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            progress.Report("Checking needed installations...");

            await ExeWrapper.makeSureWszstInstalled(ct, ProgressInfo.makeSubProgress(progress, 0, 1)).ConfigureAwait(false);
            await ExeWrapper.makeSureBenzinInstalled(ct, ProgressInfo.makeSubProgress(progress, 1, 2)).ConfigureAwait(false);

            progress.Report("Extract game_sequence files...");

            using (CancellationTokenSource source = new CancellationTokenSource())
            {
                // start fake progress
                var fakeProgressTask = ProgressInfo.makeFakeProgress(ProgressInfo.makeSubProgress(progress, 2, 33), source.Token);

                // extract the arc files
                List<Task<string>> extractArcFileTasks = new List<Task<string>>();
                foreach (var entry in cacheFileSet.game_sequence_arc)
                {
                    string locale = entry.Key;
                    string gameSequenceFile = entry.Value;
                    string tmpGameSequenceDir = Path.Combine(Path.GetDirectoryName(tmpFileSet.game_sequence_arc[locale]), Path.GetFileNameWithoutExtension(tmpFileSet.game_sequence_arc[locale]));
                    extractArcFileTasks.Add(ExeWrapper.extractArcFile(gameSequenceFile, tmpGameSequenceDir, ct, ProgressInfo.makeNoProgress(progress)));
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
                                foreach (var entry in cacheFileSet.game_sequence_arc)
                                {
                                    string locale = entry.Key;
                                    string gameSequenceFile = entry.Value;
                                    string tmpGameSequenceDir = Path.Combine(Path.GetDirectoryName(tmpFileSet.game_sequence_arc[locale]), Path.GetFileNameWithoutExtension(tmpFileSet.game_sequence_arc[locale]));
                                    var mapIconTplCopy = Path.Combine(tmpGameSequenceDir, "arc", "timg", tplName);
                                    File.Copy(mapIconTpl, mapIconTplCopy, true);
                                }
                            });
                            convertPngFileTasks.Add(task2);
                        }
                    }
                }

                // convert the brlyt files to xmlyt, inject the map icons and convert it back
                List<Task> injectMapIconsInBrlytTasks = new List<Task>();
                foreach (var entry in cacheFileSet.game_sequence_arc)
                {
                    string locale = entry.Key;
                    string gameSequenceFile = entry.Value;
                    string tmpGameSequenceDir = Path.Combine(Path.GetDirectoryName(tmpFileSet.game_sequence_arc[locale]), Path.GetFileNameWithoutExtension(tmpFileSet.game_sequence_arc[locale]));
                    var brlytFile = Path.Combine(tmpGameSequenceDir, "arc", "blyt", "ui_menu_19_00a.brlyt");
                    var xmlytFile = Path.Combine(tmpFileSet.rootDir, Path.GetFileName(tmpGameSequenceDir) + "." + Path.GetFileNameWithoutExtension(brlytFile) + ".xmlyt");
                    Task task1 = ExeWrapper.convertBryltToXmlyt(brlytFile, xmlytFile, ct, ProgressInfo.makeNoProgress(progress));
                    Task task2 = task1.ContinueWith(async (t1) => { await t1.ConfigureAwait(false); Ui_menu_19_00a_XMLYT.injectMapIcons(xmlytFile, mapIconToTplName); });
                    Task task3 = task2.ContinueWith(async (t2) => { await t2.ConfigureAwait(false); await ExeWrapper.convertXmlytToBrylt(xmlytFile, brlytFile, ct, ProgressInfo.makeNoProgress(progress)); });
                    injectMapIconsInBrlytTasks.Add(task3);
                }
                await Task.WhenAll(injectMapIconsInBrlytTasks).ConfigureAwait(false);
                await Task.WhenAll(convertPngFileTasks).ConfigureAwait(false);
                source.Cancel();
                await fakeProgressTask.ConfigureAwait(false);
            }
            // strange phenomenon: when converting the xmlyt files back to brlyt using benzin, sometimes the first byte is not correctly written. This fixes it as the first byte must be an 'R'.
            foreach (var entry in cacheFileSet.game_sequence_arc)
            {
                string locale = entry.Key;
                string tmpGameSequenceDir = Path.Combine(Path.GetDirectoryName(tmpFileSet.game_sequence_arc[locale]), Path.GetFileNameWithoutExtension(tmpFileSet.game_sequence_arc[locale]));
                var brlytFile = Path.Combine(tmpGameSequenceDir, "arc", "blyt", "ui_menu_19_00a.brlyt");
                byte[] data = File.ReadAllBytes(brlytFile);
                data[0] = (byte)'R';
                File.WriteAllBytes(brlytFile, data);
            }


            progress.Report("Pack game_sequence files...");
            using (CancellationTokenSource source = new CancellationTokenSource())
            {
                // start fake progress
                var fakeProgressTask = ProgressInfo.makeFakeProgress(ProgressInfo.makeSubProgress(progress, 66, 100), source.Token);

                // extract the arc files
                List<Task<string>> packArcFileTasks = new List<Task<string>>();
                foreach (var entry in riivFileSet.game_sequence_arc)
                {
                    string locale = entry.Key;
                    string gameSequenceFile = entry.Value;
                    string tmpGameSequenceDir = Path.Combine(Path.GetDirectoryName(tmpFileSet.game_sequence_arc[locale]), Path.GetFileNameWithoutExtension(tmpFileSet.game_sequence_arc[locale]));
                    Directory.CreateDirectory(Path.GetDirectoryName(gameSequenceFile));
                    packArcFileTasks.Add(ExeWrapper.packDfolderToArc(tmpGameSequenceDir, gameSequenceFile, ct, ProgressInfo.makeNoProgress(progress)));
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
