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

namespace CustomStreetManager
{
    public partial class CSMM : Form
    {
        private FileSet fileSet;
        private List<MapDescriptor> mapDescriptors;
        private MainDol mainDol;

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

        private void Go_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Proceeding will inject the imported map descriptors the input ISO/WBFS file. Please make sure to have a backup.", "Start Injection", MessageBoxButtons.OKCancel);
            if (dialogResult == DialogResult.OK)
            {
                ProgressBar instance = new ProgressBar();
                instance.Show();

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
            saveFileDialog1.Filter = "ISO Image|*.iso";
            saveFileDialog1.Title = "Where should I save the patched ISO?";
            saveFileDialog1.FileName = "UpdatedISOFile";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                setOutputPathLabel.Text = saveFileDialog1.FileName;
            }
            else
            {
                setOutputPathLabel.Text = "None";
            }
        }

        private void ReloadWbfsIsoFile()
        {
            string warnings = "";

            ProgressBar progressBar = new ProgressBar();
            progressBar.Show();
            progressBar.SetProgress(0, "Extract relevant files from iso/wbfs...");

            if (setInputISOLocation.Text == "None")
            {
                progressBar.SetProgressBarText("Can't load wbfs or iso file as the input file name is not set.");
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
                    MiscUtil.IO.EndianBinaryReader binReader = new MiscUtil.IO.EndianBinaryReader(MiscUtil.Conversion.EndianBitConverter.Big, stream);
                    mapDescriptors = mainDol.readMainDol(binReader);

                    progressBar.SetProgress(60, "Read localization files...");
                    UI_Message en = new UI_Message(fileSet.ui_message_en_csv);
                    UI_Message de = new UI_Message(fileSet.ui_message_de_csv);
                    UI_Message fr = new UI_Message(fileSet.ui_message_fr_csv);
                    UI_Message it = new UI_Message(fileSet.ui_message_it_csv);
                    UI_Message es = new UI_Message(fileSet.ui_message_su_csv);
                    UI_Message jp = new UI_Message(fileSet.ui_message_jp_csv);
                    UI_Message uk = new UI_Message(fileSet.ui_message_uk_csv);
                    foreach (MapDescriptor mapDescriptor in mapDescriptors)
                    {
                        mapDescriptor.Name_EN = en.get(mapDescriptor.Name_MSG_ID);
                        mapDescriptor.Name_DE = de.get(mapDescriptor.Name_MSG_ID);
                        mapDescriptor.Name_FR = fr.get(mapDescriptor.Name_MSG_ID);
                        mapDescriptor.Name_IT = it.get(mapDescriptor.Name_MSG_ID);
                        mapDescriptor.Name_SU = es.get(mapDescriptor.Name_MSG_ID);
                        mapDescriptor.Name_JP = jp.get(mapDescriptor.Name_MSG_ID);
                        mapDescriptor.Name_UK = uk.get(mapDescriptor.Name_MSG_ID);

                        mapDescriptor.Desc_EN = en.get(mapDescriptor.Desc_MSG_ID);
                        mapDescriptor.Desc_DE = de.get(mapDescriptor.Desc_MSG_ID);
                        mapDescriptor.Desc_FR = fr.get(mapDescriptor.Desc_MSG_ID);
                        mapDescriptor.Desc_IT = it.get(mapDescriptor.Desc_MSG_ID);
                        mapDescriptor.Desc_SU = es.get(mapDescriptor.Desc_MSG_ID);
                        mapDescriptor.Desc_JP = jp.get(mapDescriptor.Desc_MSG_ID);
                        mapDescriptor.Desc_UK = uk.get(mapDescriptor.Desc_MSG_ID);

                        warnings += mapDescriptor.readFrbFileInfo(fileSet.param_folder);
                    }
                }
                progressBar.SetProgress(80, "Populate UI...");

                Go.Enabled = false;
                BindingSource bs = new BindingSource();
                bs.DataSource = mapDescriptors;
                dataGridView1.DataSource = bs;

                if (!string.IsNullOrWhiteSpace(warnings))
                {
                    progressBar.SetProgress(100, "Loaded successfully. Warnings:");
                    progressBar.SetProgressBarText(warnings);
                }
                else
                {
                    progressBar.Close();
                }
            }
            catch (Exception e2)
            {
                reset();

                progressBar.SetProgressBarText(e2.Message);
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

            if (openFileDialog1.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(openFileDialog1.FileName))
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
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = Path.GetDirectoryName(setInputISOLocation.Text);
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string fileNameMd = Path.Combine(fbd.SelectedPath, mapDescriptor.InternalName + ".md");
                    string fileNameFrb1 = Path.Combine(fbd.SelectedPath, mapDescriptor.FrbFile1 + ".frb");
                    string fileNameFrb2 = null;
                    string fileNameFrb3 = null;
                    string fileNameFrb4 = null;
                    if (!string.IsNullOrWhiteSpace(mapDescriptor.FrbFile2))
                        fileNameFrb2 = Path.Combine(fbd.SelectedPath, mapDescriptor.FrbFile2 + ".frb");
                    if (!string.IsNullOrWhiteSpace(mapDescriptor.FrbFile3))
                        fileNameFrb3 = Path.Combine(fbd.SelectedPath, mapDescriptor.FrbFile3 + ".frb");
                    if (!string.IsNullOrWhiteSpace(mapDescriptor.FrbFile4))
                        fileNameFrb4 = Path.Combine(fbd.SelectedPath, mapDescriptor.FrbFile4 + ".frb");

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
                    progressBar.SetProgressBarText(extractedFiles);
                }
            }
        }

        private void importMd(MapDescriptor mapDescriptor)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Map Descriptor File (.md)|*.md";
            openFileDialog1.Title = "Which Map to import?";

            if (openFileDialog1.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(openFileDialog1.FileName))
            {

                ProgressBar progressBar = new ProgressBar();
                progressBar.Show();
                string importedFiles = "";
                try
                {
                    MapDescriptor mapDescriptorImport = new MapDescriptor();

                    progressBar.SetProgress(0, "Parse Map Descriptor File...");

                    var mapDescriptorImportFile = openFileDialog1.FileName;
                    var dir = Path.GetDirectoryName(openFileDialog1.FileName);
                    mapDescriptorImport.readMapDescriptorFromFile(mapDescriptorImportFile);
                    importedFiles += mapDescriptorImportFile + Environment.NewLine;

                    progressBar.SetProgress(30, "Read additional data from frb file(s)...");
                    mapDescriptorImport.readFrbFileInfo(dir);

                    progressBar.SetProgress(60, "Copy frb file(s) to tmp...");


                    var frbFileName = mapDescriptorImport.FrbFile1;
                    var importFile = Path.Combine(dir, frbFileName + ".frb");
                    var importFileTmp = Path.Combine(fileSet.param_folder, frbFileName + ".frb");
                    File.Copy(importFile, importFileTmp, true);
                    importedFiles += importFile + Environment.NewLine;

                    frbFileName = mapDescriptorImport.FrbFile2;
                    if (frbFileName != null)
                    {
                        importFile = Path.Combine(dir, frbFileName + ".frb");
                        importFileTmp = Path.Combine(fileSet.param_folder, frbFileName + ".frb");
                        File.Copy(importFile, importFileTmp, true);
                        importedFiles += importFile + Environment.NewLine;
                    }
                    frbFileName = mapDescriptorImport.FrbFile3;
                    if (frbFileName != null)
                    {
                        importFile = Path.Combine(dir, frbFileName + ".frb");
                        importFileTmp = Path.Combine(fileSet.param_folder, frbFileName + ".frb");
                        File.Copy(importFile, importFileTmp, true);
                        importedFiles += importFile + Environment.NewLine;
                    }
                    frbFileName = mapDescriptorImport.FrbFile4;
                    if (frbFileName != null)
                    {
                        importFile = Path.Combine(dir, frbFileName + ".frb");
                        importFileTmp = Path.Combine(fileSet.param_folder, frbFileName + ".frb");
                        File.Copy(importFile, importFileTmp, true);
                        importedFiles += importFile + Environment.NewLine;
                    }

                    progressBar.SetProgress(90, "Update UI...");
                    mapDescriptor.set(mapDescriptorImport);
                    mapDescriptor.Dirty = true;
                    Go.Enabled = true;
                    updateDataGridData(null, null);

                    progressBar.SetProgress(100, "Done. Following files have be processed and are ready to be injected:");
                    progressBar.SetProgressBarText(importedFiles);
                }
                catch (Exception e)
                {
                    progressBar.SetProgressBarText(e.Message);
                    progressBar.EnableButton();
                    Console.Error.WriteLine(e.ToString());
                }
            }
        }
    }
}