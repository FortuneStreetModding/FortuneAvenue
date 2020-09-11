using System;
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

namespace CustomStreetManager
{
    public partial class CSMM : Form
    {
        private FileSet fileSet;
        private List<MapDescriptor> mapDescriptors;
        private MainDol mainDol;
        private Dictionary<string, UI_Message> ui_messages = new Dictionary<string, UI_Message>();
        Task<int> extractIsoTask;

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
            ProgressBar progressBar = new ProgressBar();
            progressBar.Show();

            // expand dol if not already expanded
            /*if (mainDol.toFileAddress(0x80001800) == -1)
            {
                WitWrapper.createNewTextSection(fileSet.main_dol, 0x80001800, 0x1800);
                mainDol.setSections(WitWrapper.readSections(fileSet.main_dol));
            }*/

            var tempMainDol = fileSet.main_dol + ".tmp";
            File.Copy(fileSet.main_dol, tempMainDol, true);

            progressBar.textArea.Text = "";
            try
            {
                progressBar.SetProgress(0, "Writing data to main.dol...");
                using (Stream baseStream = File.Open(tempMainDol, FileMode.Open))
                {
                    EndianBinaryWriter stream = new EndianBinaryWriter(EndianBitConverter.Big, baseStream);
                    mainDol.writeMainDol(stream, mapDescriptors);

                    progressBar.textArea.Text += "Amount of free space used in main.dol: " + mainDol.totalBytesWritten + " bytes" + Environment.NewLine;
                    progressBar.textArea.Text += "Amount of free space left in main.dol: " + mainDol.totalBytesLeft + " bytes" + Environment.NewLine;
                    progressBar.textArea.Update();
                }
                // everything went through successfully, copy the temp file
                File.Copy(tempMainDol, fileSet.main_dol, true);
                File.Delete(tempMainDol);

                progressBar.SetProgress(5, "Writing localization files...");
                foreach (var entry in ui_messages)
                {
                    string fileSet_ui_message_csv = entry.Key;
                    UI_Message ui_message = entry.Value;
                    ui_message.set(mapDescriptors);
                    ui_message.writeToFile(fileSet_ui_message_csv);
                }

                // check if iso has been extracted already
                if (extractIsoTask != null)
                {
                    progressBar.SetProgress(10, "Extracting ISO/WBFS file...");
                    int errorCode = await extractIsoTask;
                    if (errorCode == 0)
                    {
                        throw new ApplicationException("Wit extraction job returned non-zero exit code: " + errorCode);
                    }
                }

                progressBar.SetProgress(25, "Copying the modified files to be packed into the image...");
                WitWrapper.copyRelevantFilesForPacking(fileSet, inputfilename);

                progressBar.SetProgress(30, "Packing ISO/WBFS file...");
                await WitWrapper.packFullIso(inputfilename, outputFilename, progressBar.update, 30, 100);

                progressBar.SetProgress(100, "Done.");
            }
            catch (Exception e2)
            {
                progressBar.textArea.Text += e2.Message;
                progressBar.textArea.Text += Environment.NewLine + Environment.NewLine + e2.ToString();
                progressBar.textArea.Update();
                progressBar.EnableButton();
                Console.Error.WriteLine(e2.ToString());
            }
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

        private string reloadUIMessages(List<MapDescriptor> mapDescriptors)
        {
            string warnings = "";
            ui_messages[fileSet.ui_message_en_csv] = new UI_Message(fileSet.ui_message_en_csv, Locale.EN);
            ui_messages[fileSet.ui_message_de_csv] = new UI_Message(fileSet.ui_message_de_csv, Locale.DE);
            ui_messages[fileSet.ui_message_fr_csv] = new UI_Message(fileSet.ui_message_fr_csv, Locale.FR);
            ui_messages[fileSet.ui_message_it_csv] = new UI_Message(fileSet.ui_message_it_csv, Locale.IT);
            ui_messages[fileSet.ui_message_su_csv] = new UI_Message(fileSet.ui_message_su_csv, Locale.ES);
            ui_messages[fileSet.ui_message_jp_csv] = new UI_Message(fileSet.ui_message_jp_csv, Locale.JP);
            ui_messages[fileSet.ui_message_uk_csv] = ui_messages[fileSet.ui_message_en_csv];
            foreach (MapDescriptor mapDescriptor in mapDescriptors)
            {
                mapDescriptor.Name[Locale.EN] = ui_messages[fileSet.ui_message_en_csv].get(mapDescriptor.Name_MSG_ID);
                mapDescriptor.Name[Locale.DE] = ui_messages[fileSet.ui_message_de_csv].get(mapDescriptor.Name_MSG_ID);
                mapDescriptor.Name[Locale.FR] = ui_messages[fileSet.ui_message_fr_csv].get(mapDescriptor.Name_MSG_ID);
                mapDescriptor.Name[Locale.IT] = ui_messages[fileSet.ui_message_it_csv].get(mapDescriptor.Name_MSG_ID);
                mapDescriptor.Name[Locale.ES] = ui_messages[fileSet.ui_message_su_csv].get(mapDescriptor.Name_MSG_ID);
                mapDescriptor.Name[Locale.JP] = ui_messages[fileSet.ui_message_jp_csv].get(mapDescriptor.Name_MSG_ID);

                mapDescriptor.Desc[Locale.EN] = ui_messages[fileSet.ui_message_en_csv].get(mapDescriptor.Desc_MSG_ID);
                mapDescriptor.Desc[Locale.DE] = ui_messages[fileSet.ui_message_de_csv].get(mapDescriptor.Desc_MSG_ID);
                mapDescriptor.Desc[Locale.FR] = ui_messages[fileSet.ui_message_fr_csv].get(mapDescriptor.Desc_MSG_ID);
                mapDescriptor.Desc[Locale.IT] = ui_messages[fileSet.ui_message_it_csv].get(mapDescriptor.Desc_MSG_ID);
                mapDescriptor.Desc[Locale.ES] = ui_messages[fileSet.ui_message_su_csv].get(mapDescriptor.Desc_MSG_ID);
                mapDescriptor.Desc[Locale.JP] = ui_messages[fileSet.ui_message_jp_csv].get(mapDescriptor.Desc_MSG_ID);

                warnings += mapDescriptor.readFrbFileInfo(fileSet.param_folder);
            }
            for (var i = 0; i <= 17; i++)
            {
                var mapDescriptor = mapDescriptors[i];
                if (mapDescriptor.Name_MSG_ID == MainDol.VANILLA_FIRST_MAP_NAME_MESSAGE_ID + i)
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
                if (mapDescriptor.Desc_MSG_ID == MainDol.VANILLA_FIRST_MAP_DESC_MESSAGE_ID + i)
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

        private void ReloadWbfsIsoFile()
        {
            ProgressBar progressBar = new ProgressBar();
            progressBar.Show();
            progressBar.SetProgress(0, "Extract relevant files from iso/wbfs...");

            if (setInputISOLocation.Text == "None")
            {
                progressBar.textArea.Text += "Can't load wbfs or iso file as the input file name is not set.";
                progressBar.EnableButton();
                return;
            }

            try
            {
                fileSet = WitWrapper.extractFiles(setInputISOLocation.Text);

                progressBar.SetProgress(20, "Detect the sections in main.dol file...");
                List<MainDolSection> sections = WitWrapper.readSections(fileSet.main_dol);
                mainDol = new MainDol(sections);

                progressBar.SetProgress(40, "Read data from main.dol file...");
                using (var stream = File.OpenRead(fileSet.main_dol))
                {
                    EndianBinaryReader binReader = new EndianBinaryReader(EndianBitConverter.Big, stream);
                    mapDescriptors = mainDol.readMainDol(binReader);

                    progressBar.SetProgress(60, "Read localization files...");
                    progressBar.textArea.Text += reloadUIMessages(mapDescriptors);
                    progressBar.textArea.Update();
                }
                progressBar.SetProgress(80, "Populate UI...");

                Go.Enabled = false;
                BindingSource bs = new BindingSource();
                bs.DataSource = mapDescriptors;
                dataGridView1.DataSource = bs;

                progressBar.SetProgress(100, "Loaded successfully.");

                extractIsoTask = WitWrapper.extractFullIsoAsync(setInputISOLocation.Text, null, 0, 0);
            }
            catch (Exception e2)
            {
                reset();

                progressBar.textArea.Text += e2.Message;
                progressBar.EnableButton();

                Console.Error.WriteLine(e2.ToString());
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
                progressBar.Show();
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
                progressBar.textArea.Text += extractedFiles;
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
                progressBar.Show();
                try
                {
                    MapDescriptor mapDescriptorImport = new MapDescriptor();

                    progressBar.SetProgress(0, "Parse Map Descriptor File...");

                    var mapDescriptorImportFile = openFileDialog1.FileName;
                    var dir = Path.GetDirectoryName(openFileDialog1.FileName);
                    mapDescriptorImport.readMapDescriptorFromFile(mapDescriptorImportFile);

                    if (mapDescriptorImport.VentureCardActiveCount != 64)
                    {
                        progressBar.textArea.Text += "Warning: The venture card count needs to be 64 or the game will choose a default venture card table." + Environment.NewLine;
                    }

                    progressBar.textArea.Text += "Imported " + mapDescriptorImportFile + Environment.NewLine;

                    progressBar.SetProgress(20, "Read additional data from frb file(s)...");
                    mapDescriptorImport.readFrbFileInfo(dir);

                    progressBar.SetProgress(40, "Copy frb file(s) to tmp...");


                    var frbFileName = mapDescriptorImport.FrbFile1;
                    var importFile = Path.Combine(dir, frbFileName + ".frb");
                    var importFileTmp = Path.Combine(fileSet.param_folder, frbFileName + ".frb");
                    File.Copy(importFile, importFileTmp, true);
                    progressBar.textArea.Text += "Imported " + importFile + Environment.NewLine;

                    frbFileName = mapDescriptorImport.FrbFile2;
                    if (frbFileName != null)
                    {
                        importFile = Path.Combine(dir, frbFileName + ".frb");
                        importFileTmp = Path.Combine(fileSet.param_folder, frbFileName + ".frb");
                        File.Copy(importFile, importFileTmp, true);
                        progressBar.textArea.Text += "Imported " + importFile + Environment.NewLine;
                    }
                    frbFileName = mapDescriptorImport.FrbFile3;
                    if (frbFileName != null)
                    {
                        importFile = Path.Combine(dir, frbFileName + ".frb");
                        importFileTmp = Path.Combine(fileSet.param_folder, frbFileName + ".frb");
                        File.Copy(importFile, importFileTmp, true);
                        progressBar.textArea.Text += "Imported " + importFile + Environment.NewLine;
                    }
                    frbFileName = mapDescriptorImport.FrbFile4;
                    if (frbFileName != null)
                    {
                        importFile = Path.Combine(dir, frbFileName + ".frb");
                        importFileTmp = Path.Combine(fileSet.param_folder, frbFileName + ".frb");
                        File.Copy(importFile, importFileTmp, true);
                        progressBar.textArea.Text += "Imported " + importFile + Environment.NewLine;
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
                    progressBar.textArea.Text += e.Message;
                    progressBar.EnableButton();
                    Console.Error.WriteLine(e.ToString());
                }
            }
        }
    }
}