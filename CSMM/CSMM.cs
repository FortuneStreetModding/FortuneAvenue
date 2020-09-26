﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using FSEditor.MapDescriptor;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Drawing;
using MiscUtil.IO;
using MiscUtil.Conversion;
using System.Threading.Tasks;
using FSEditor.FSData;
using System.Xml;

namespace CustomStreetManager
{
    public partial class CSMM : Form
    {
        private FileSet fileSet;
        private List<MapDescriptor> mapDescriptors;
        private MainDol mainDol;
        private Dictionary<string, UI_Message> ui_messages = new Dictionary<string, UI_Message>();

        public CSMM()
        {
            InitializeComponent();
            if (!SystemInformation.TerminalServerSession)
            {
                Type dgvType = dataGridView1.GetType();
                PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
                pi.SetValue(dataGridView1, true, null);
            }
        }

        private void reset()
        {
            fileSet = null;
            mapDescriptors = null;
            mainDol = null;
            setInputISOLocation.Text = "None";
            Go.Enabled = false;
        }

        private async void Go_Click(object sender, EventArgs e)
        {
            var inputfilename = setInputISOLocation.Text;
            var outputFilename = setOutputPathLabel.Text;

            if (String.IsNullOrWhiteSpace(outputFilename) || outputFilename.ToLower() == "none")
            {
                MessageBox.Show("Please set the output file.");
                return;
            }
            var tokenSource2 = new CancellationTokenSource();
            CancellationToken ct = tokenSource2.Token;

            ProgressBar progressBar = new ProgressBar();
            progressBar.callback = (b) => tokenSource2?.Cancel();

            progressBar.Show(this);
            var progress = new Progress<ProgressInfo>(progressInfo =>
            {
                progressBar.update(progressInfo);
            });

            // expand dol if not already expanded
            /*if (mainDol.toFileAddress(0x80001800) == -1)
            {
                WitWrapper.createNewTextSection(fileSet.main_dol, 0x80001800, 0x1800);
                mainDol.setSections(WitWrapper.readSections(fileSet.main_dol));
            }*/

            var tempMainDol = fileSet.main_dol + ".tmp";
            File.Copy(fileSet.main_dol, tempMainDol, true);

            try
            {

                progressBar.SetProgress(0, "Writing data to main.dol...");
                // HACK: expand the description message table
                using (Stream baseStream = File.Open(tempMainDol, FileMode.Open))
                {
                    EndianBinaryWriter stream = new EndianBinaryWriter(EndianBitConverter.Big, baseStream);
                    mainDol.writeMainDol(stream, mapDescriptors);

                    progressBar.appendText("Amount of free space used in main.dol: " + mainDol.totalBytesWritten + " bytes" + Environment.NewLine);
                    progressBar.appendText("Amount of free space left in main.dol: " + mainDol.totalBytesLeft + " bytes" + Environment.NewLine);
                }
                // everything went through successfully, copy the temp file
                File.Copy(tempMainDol, fileSet.main_dol, true);

                // lets get to the map icons
                _ = await injectMapIcons(progressBar, ct, progress, 5, 50);

                progressBar.SetProgress(46, "Writing localization files...");
                foreach (var entry in ui_messages)
                {
                    string fileSet_ui_message_csv = entry.Key;
                    UI_Message ui_message = entry.Value;
                    ui_message.set(mapDescriptors);
                    ui_message.writeToFile(fileSet_ui_message_csv);
                }

                progressBar.SetProgress(47, "Copying the modified files to be packed into the image...");
                ExeWrapper.copyRelevantFilesForPacking(fileSet, inputfilename);

                progressBar.SetProgress(50, "Packing ISO/WBFS file...");

                await ExeWrapper.packFullIso(inputfilename, outputFilename, ct, progress, 50, 100);

                progressBar.ShowCheckbox("Cleanup temporary files.", false);
                progressBar.callback = (c) => { if (c) ExeWrapper.cleanup(inputfilename); };
                progressBar.SetProgress(100, "Done.");
            }
            catch (Exception e2)
            {
                progressBar.appendText(e2.Message);
                progressBar.appendText(Environment.NewLine + Environment.NewLine + e2.ToString());
                progressBar.EnableButton();
                Console.Error.WriteLine(e2.ToString());
            }
            finally
            {
                File.Delete(tempMainDol);
                // tokenSource2.Dispose();
            }
        }

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

        private async Task<bool> injectMapIcons(ProgressBar progressBar, CancellationToken ct, Progress<ProgressInfo> progress, int minProgress, int maxProgress)
        {
            var minProgress1 = ProgressInfo.lerp(minProgress, maxProgress, 0.0f);
            var maxProgress1 = ProgressInfo.lerp(minProgress, maxProgress, 0.25f);
            var minProgress2 = ProgressInfo.lerp(minProgress, maxProgress, 0.25f);
            var maxProgress2 = ProgressInfo.lerp(minProgress, maxProgress, 0.50f);
            var minProgress3 = ProgressInfo.lerp(minProgress, maxProgress, 0.50f);
            var maxProgress3 = ProgressInfo.lerp(minProgress, maxProgress, 0.75f);
            var minProgress4 = ProgressInfo.lerp(minProgress, maxProgress, 0.75f);
            var maxProgress4 = ProgressInfo.lerp(minProgress, maxProgress, 1.0f);

            progressBar.SetProgress(5, "Extract game_sequence files...");
            // throw together the game_sequence and game_sequence_wifi files
            List<string> gameSequenceFiles = new List<string>();
            gameSequenceFiles.AddRange(fileSet.game_sequence_arc.Values);
            gameSequenceFiles.AddRange(fileSet.game_sequence_wifi_arc.Values);

            // extract the arc files
            List<Task<string>> extractArcFileTasks = new List<Task<string>>();
            foreach (var gameSequenceFile in gameSequenceFiles)
            {
                extractArcFileTasks.Add(ExeWrapper.extractArcFile(gameSequenceFile, toTmpDirectory(gameSequenceFile, null, "_d"), ct, progress, minProgress1, maxProgress1));
            }
            await Task.WhenAll(extractArcFileTasks);

            // convert the png files to tpl and copy them to the correct location
            Dictionary<string, string> mapIconToTplName = new Dictionary<string, string>();
            List<Task<string>> convertPngFileTasks = new List<Task<string>>();
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
                    Task task1 = ExeWrapper.convertPngToTpl(mapIconPng, mapIconTpl, ct, progress, minProgress2, maxProgress2);
                    Task task2 = task1.ContinueWith((t1) =>
                    {
                        foreach (var gameSequenceFile in gameSequenceFiles)
                        {
                            var gameSequenceExtractedDir = toTmpDirectory(gameSequenceFile, null, "_d");
                            var mapIconTplCopy = Path.Combine(gameSequenceExtractedDir, "arc", "timg", tplName);
                            File.Copy(mapIconTpl, mapIconTplCopy);
                        }
                    });
                    convertPngFileTasks.Add(ExeWrapper.convertPngToTpl(mapIconPng, toTmpDirectory(mapIconPng, null, ".tpl"), ct, progress, minProgress3, maxProgress3));
                }
            }

            // convert the brlyt files to xmlyt, inject the map icons and convert it back
            List<Task> injectMapIconsInBrlytTasks = new List<Task>();
            foreach (var gameSequenceFile in gameSequenceFiles)
            {
                var gameSequenceExtractedDir = toTmpDirectory(gameSequenceFile, null, "_d");
                var brlytFile = Path.Combine(gameSequenceExtractedDir, "arc", "blyt", "ui_menu_19_00a.brlyt");
                var xmlytFile = toTmpDirectory(brlytFile, gameSequenceExtractedDir, ".xmlyt");
                Task task1 = ExeWrapper.convertBryltToXmlyt(brlytFile, xmlytFile, ct, null, -1, -1);
                Task task2 = task1.ContinueWith((t1) => Ui_menu_19_00a_XMLYT.injectMapIcons(xmlytFile, mapIconToTplName));
                Task task3 = task2.ContinueWith((t2) => ExeWrapper.convertXmlytToBrylt(xmlytFile, brlytFile, ct, progress, minProgress4, maxProgress4));
                // Task task4 = task3.ContinueWith((t3) => File.Delete(xmlytFile));
                injectMapIconsInBrlytTasks.Add(task3);
            }

            await Task.WhenAll(injectMapIconsInBrlytTasks);
            await Task.WhenAll(convertPngFileTasks);

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


            progressBar.SetProgress(15, "Ex.");

            return true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void SaveFileDialog(object sender, EventArgs e) //set output location button
        {
            // Displays a SaveFileDialog so the user can save the Image
            // assigned to Button2.
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "ISO/WBFS Image|*.iso;*.wbfs";
            saveFileDialog1.Title = "Where shall the patches ISO/WBFS be saved?";
            saveFileDialog1.FileName = Path.GetFileName(setInputISOLocation.Text);

            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                setOutputPathLabel.Text = saveFileDialog1.FileName;
            }
            else
            {
                setOutputPathLabel.Text = "None";
            }
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

        private async void ReloadWbfsIsoFile()
        {
            var tokenSource2 = new CancellationTokenSource();
            CancellationToken ct = tokenSource2.Token;

            ProgressBar progressBar = new ProgressBar();
            progressBar.Show(this);
            progressBar.SetProgress(0, "Extract relevant files from iso/wbfs...");
            progressBar.callback = (b) => tokenSource2?.Cancel();
            var progress = new Progress<ProgressInfo>(progressInfo =>
            {
                progressBar.update(progressInfo);
            });

            if (setInputISOLocation.Text == "None")
            {
                progressBar.appendText("Can't load wbfs or iso file as the input file name is not set.");
                progressBar.EnableButton();
                return;
            }


            try
            {
                fileSet = await ExeWrapper.extractFiles(setInputISOLocation.Text, ct, progress, 0, 40);

                progressBar.SetProgressBarLabel("Detect the sections in main.dol file...");
                List<MainDolSection> sections = await ExeWrapper.readSections(fileSet.main_dol, ct, progress, 40, 45);
                mainDol = new MainDol(sections);

                progressBar.SetProgressBarLabel("Read data from main.dol file...");
                using (var stream = File.OpenRead(fileSet.main_dol))
                {
                    EndianBinaryReader binReader = new EndianBinaryReader(EndianBitConverter.Big, stream);
                    mapDescriptors = mainDol.readMainDol(binReader);

                    progressBar.SetProgress(47, "Read localization files...");
                    progressBar.appendText(reloadUIMessages(mapDescriptors, mainDol.data));
                }
                progressBar.SetProgress(49, "Populate UI...");

                Go.Enabled = false;
                BindingSource bs = new BindingSource();
                bs.DataSource = mapDescriptors;
                dataGridView1.DataSource = bs;

                progressBar.SetProgress(50, "Extract WBFS/ISO...");

                string cacheDirectory = await ExeWrapper.extractFullIsoAsync(setInputISOLocation.Text, ct, progress, 50, 100);

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


                progressBar.SetProgress(100, "Loaded successfully.");
            }
            catch (Exception e2)
            {
                reset();

                progressBar.appendText(e2.Message);
                progressBar.EnableButton();

                Console.Error.WriteLine(e2.ToString());
            }
            finally
            {
               // tokenSource2.Dispose();
            }
            updateDataGridData(null, null);
        }

        private void OpenFileDialog(object sender, EventArgs e)
        {
            // Displays a SaveFileDialog so the user can save the Image
            // assigned to Button2.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "ISO/WBFS Image|*.iso;*.wbfs";
            openFileDialog1.Title = "Which ISO image or WBFS file should we patch?";

            if (openFileDialog1.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(openFileDialog1.FileName))
            {
                setInputISOLocation.Text = openFileDialog1.FileName;
                ReloadWbfsIsoFile();
            }
        }

        private void deflaktorsASMHacksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (deflaktorsASMHacksToolStripMenuItem.Checked == true)
            {
                MessageBox.Show("Please be aware that I'm not checking for this -- " +
                    "but you need a PAL ISO to enable Deflaktor's ASM Hacks, otherwise " +
                    "your game will crash on startup.");
            }
        }

        private void clearListButton_Click(object sender, EventArgs e)
        {
            ReloadWbfsIsoFile();
        }

        private void updateDataGridData(object sender, EventArgs e)
        {
            BindingSource bs = dataGridView1.DataSource as BindingSource;
            if (mapDescriptors != null)
            {
                if (checkBox1.Checked)
                {
                    bs.DataSource = mapDescriptors;
                }
                else
                {
                    bs.DataSource = mapDescriptors.FindAll(md => md.ID >= 0 && md.ID < 18);
                }
            }
            else
            {
                bs.DataSource = null;
            }

            dataGridView1.DataSource = bs;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                MapDescriptor mapDescriptor = senderGrid.Rows[e.RowIndex].DataBoundItem as MapDescriptor;
                if (senderGrid.Columns[e.ColumnIndex].Name.ToLower().Contains("export"))
                {
                    exportMd(mapDescriptor);
                }
                else if (senderGrid.Columns[e.ColumnIndex].Name.ToLower().Contains("import"))
                {
                    importMd(mapDescriptor);
                }
            }
        }
        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            MapDescriptor mapDescriptor = dataGridView1.Rows[e.RowIndex].DataBoundItem as MapDescriptor;
            if (mapDescriptor.Dirty)
            {
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
            }
        }

        private void exportMd(MapDescriptor mapDescriptor)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Map Descriptor file and accompanying .frb files|*.md";
            saveFileDialog1.Title = "Where shall the map files be exported?";
            saveFileDialog1.FileName = mapDescriptor.InternalName + ".md";
            saveFileDialog1.OverwritePrompt = false;

            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(saveFileDialog1.FileName))
            {
                var selectedFile = saveFileDialog1.FileName;
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

                string filesToBeReplaced = "";
                if (File.Exists(fileNameMd))
                {
                    filesToBeReplaced += fileNameMd + Environment.NewLine;
                }
                if (File.Exists(fileNameFrb1))
                {
                    filesToBeReplaced += fileNameFrb1 + Environment.NewLine;
                }
                if (fileNameFrb2 != null && File.Exists(fileNameFrb2))
                {
                    filesToBeReplaced += fileNameFrb2 + Environment.NewLine;
                }
                if (fileNameFrb3 != null && File.Exists(fileNameFrb3))
                {
                    filesToBeReplaced += fileNameFrb3 + Environment.NewLine;
                }
                if (fileNameFrb4 != null && File.Exists(fileNameFrb4))
                {
                    filesToBeReplaced += fileNameFrb4 + Environment.NewLine;
                }

                if (!string.IsNullOrWhiteSpace(filesToBeReplaced))
                {
                    DialogResult dialogResult = MessageBox.Show("The following files already exist and will be replaced:" + Environment.NewLine + filesToBeReplaced, "Files already exist", MessageBoxButtons.OKCancel);

                    if (dialogResult == DialogResult.OK)
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
                    else if (dialogResult == DialogResult.Cancel)
                    {
                        return;
                    }
                }

                ProgressBar progressBar = new ProgressBar();
                progressBar.Show(this);
                progressBar.SetProgress(0, "Generating Map Descriptor File...");

                string extractedFiles = "";
                using (FileStream fs = File.Create(fileNameMd))
                {
                    byte[] content = Encoding.UTF8.GetBytes(mapDescriptor.generateMapDescriptorFileContent());
                    fs.Write(content, 0, content.Length);
                }
                extractedFiles += fileNameMd + Environment.NewLine;

                progressBar.SetProgress(50, "Copying frb files...");

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
                progressBar.SetProgress(100, "Done. Generated md file and extracted frb file(s):");
                progressBar.appendText(extractedFiles);
            }
        }

        private void importMd(MapDescriptor mapDescriptor)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Map Descriptor File (.md)|*.md";
            openFileDialog1.Title = "Which Map to import?";

            if (openFileDialog1.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(openFileDialog1.FileName))
            {
                ProgressBar progressBar = new ProgressBar();
                progressBar.Show(this);
                try
                {
                    MapDescriptor mapDescriptorImport = new MapDescriptor();

                    progressBar.SetProgress(0, "Parse Map Descriptor File...");

                    var mapDescriptorImportFile = openFileDialog1.FileName;
                    var dir = Path.GetDirectoryName(openFileDialog1.FileName);

                    var name = Path.GetFileNameWithoutExtension(mapDescriptorImportFile);
                    if (name.ToLower() == "readme")
                    {
                        name = Path.GetFileName(dir);
                    }

                    mapDescriptorImport.readMapDescriptorFromFile(mapDescriptorImportFile, name);

                    if (mapDescriptorImport.VentureCardActiveCount != 64)
                    {
                        progressBar.appendText("Warning: The venture card count needs to be 64 or the game will choose a default venture card table." + Environment.NewLine);
                    }

                    progressBar.appendText("Imported " + mapDescriptorImportFile + Environment.NewLine);

                    progressBar.SetProgress(20, "Read additional data from frb file(s)...");
                    mapDescriptorImport.readFrbFileInfo(dir);

                    progressBar.SetProgress(40, "Copy frb file(s) to tmp...");


                    var frbFileName = mapDescriptorImport.FrbFile1;
                    var importFile = Path.Combine(dir, frbFileName + ".frb");
                    var importFileTmp = Path.Combine(fileSet.param_folder, frbFileName + ".frb");
                    File.Copy(importFile, importFileTmp, true);
                    progressBar.appendText("Imported " + importFile + Environment.NewLine);

                    frbFileName = mapDescriptorImport.FrbFile2;
                    if (frbFileName != null)
                    {
                        importFile = Path.Combine(dir, frbFileName + ".frb");
                        importFileTmp = Path.Combine(fileSet.param_folder, frbFileName + ".frb");
                        File.Copy(importFile, importFileTmp, true);
                        progressBar.appendText("Imported " + importFile + Environment.NewLine);
                    }
                    frbFileName = mapDescriptorImport.FrbFile3;
                    if (frbFileName != null)
                    {
                        importFile = Path.Combine(dir, frbFileName + ".frb");
                        importFileTmp = Path.Combine(fileSet.param_folder, frbFileName + ".frb");
                        File.Copy(importFile, importFileTmp, true);
                        progressBar.appendText("Imported " + importFile + Environment.NewLine);
                    }
                    frbFileName = mapDescriptorImport.FrbFile4;
                    if (frbFileName != null)
                    {
                        importFile = Path.Combine(dir, frbFileName + ".frb");
                        importFileTmp = Path.Combine(fileSet.param_folder, frbFileName + ".frb");
                        File.Copy(importFile, importFileTmp, true);
                        progressBar.appendText("Imported " + importFile + Environment.NewLine);
                    }
                    var mapIcon = mapDescriptorImport.MapIcon;
                    if (mapIcon != null)
                    {
                        importFile = Path.Combine(dir, mapIcon + ".png");
                        importFileTmp = toTmpDirectory(importFile, null, ".png");
                        File.Copy(importFile, importFileTmp, true);
                        progressBar.appendText("Imported " + importFile + Environment.NewLine);
                    }
                    progressBar.SetProgress(80, "Update UI...");
                    mapDescriptor.set(mapDescriptorImport);
                    mapDescriptor.Dirty = true;
                    Go.Enabled = true;
                    updateDataGridData(null, null);

                    progressBar.SetProgress(100, "Done.");
                }
                catch (Exception e)
                {
                    progressBar.appendText(e.Message);
                    progressBar.EnableButton();
                    Console.Error.WriteLine(e.ToString());
                }
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/FortuneStreetModding/FortuneAvenue/tree/master/Doc/CSMM");
        }
    }
}