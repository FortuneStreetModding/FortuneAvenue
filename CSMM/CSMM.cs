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

        public CSMM()
        {
            InitializeComponent();
            if (!System.Windows.Forms.SystemInformation.TerminalServerSession)
            {
                Type dgvType = dataGridView1.GetType();
                PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                  BindingFlags.Instance | BindingFlags.NonPublic);
                pi.SetValue(dataGridView1, true, null);
            }
        }

        private void Go_Click(object sender, EventArgs e)
        {
            // TODO
            string ExtractDiscFileName = "";
            string extractDiscBatFilePath = Path.Combine(Directory.GetCurrentDirectory(), ExtractDiscFileName);

            ProgressBar instance = new ProgressBar();
            instance.Show();

            UpdateProgressWindow(instance, "Extracting disc...", 25);

            if (File.Exists(setInputISOLocation.Text))
            {
                if (setOutputPathLabel.Text != "None")
                {
                    UpdateProgressWindow(instance, "Replacing maps...", 40);

                    UpdateProgressWindow(instance, "Setting Options...", 70);

                    UpdateProgressWindow(instance, "Finalizing changes...", 85);

                    UpdateProgressWindow(instance, "Re-compiling disc...", 90);

                    UpdateProgressWindow(instance, "Done!", 100);
                }
                else
                {
                    instance.SetProgressBarLabel("Error: The output file path cannot be blank!");
                    instance.SetProgressBarValue(100);
                }
            }
            else
            {
                instance.SetProgressBarLabel("Error: The source ISO could not be opened.");
                instance.SetProgressBarValue(100);
            }
        }

        private static void UpdateProgressWindow(ProgressBar instance, string status, int amount)
        {
            instance.SetProgressBarLabel(status);
            instance.SetProgressBarValue(amount);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutCustomStreetMapManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutPanel instance = new AboutPanel();
            instance.Show();
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

        private void OpenFileDialog(object sender, EventArgs e)
        {
            // Displays a SaveFileDialog so the user can save the Image
            // assigned to Button2.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "ISO/WBFS Image|*.iso;*.wbfs";
            openFileDialog1.Title = "Which ISO image or WBFS file should we patch?";

            if (openFileDialog1.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(openFileDialog1.FileName))
            {
                string warnings = "";

                ProgressBar progressBar = new ProgressBar();
                progressBar.Show();
                progressBar.SetProgress(0,"Extract relevant files from iso/wbfs...");

                fileSet = null;
                mapDescriptors = null;
                setInputISOLocation.Text = "None";

                try { 

                    fileSet = WitWrapper.extractFiles(openFileDialog1.FileName);

                    progressBar.SetProgress(20, "Detect the sections in main.dol file...");
                    List<MainDolSection> sections = WitWrapper.readSections(fileSet.main_dol);
                    MainDol mainDol = new MainDol(sections);
                    setInputISOLocation.Text = openFileDialog1.FileName;

                    progressBar.SetProgress(40, "Read data from main.dol file...");
                    using (var stream = File.OpenRead(fileSet.main_dol))
                    {
                        MiscUtil.IO.EndianBinaryReader binReader = new MiscUtil.IO.EndianBinaryReader(MiscUtil.Conversion.EndianBitConverter.Big, stream);
                        mapDescriptors = mainDol.ReadMainDol(binReader);

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

                            warnings += mapDescriptor.ReadFrbFileInfo(fileSet.param_folder);
                        }
                    }
                    progressBar.SetProgress(80, "Populate UI...");

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
                catch(Exception e2)
                {
                    fileSet = null;
                    mapDescriptors = null;
                    setInputISOLocation.Text = "None";

                    progressBar.SetProgressBarText(e2.Message);
                    progressBar.EnableButton();
                }
                checkBox1_CheckedChanged(null, null);
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

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            BindingSource bs = dataGridView1.DataSource as BindingSource;
            if(mapDescriptors != null)
            {
                if (checkBox1.Checked)
                {
                    bs.DataSource = mapDescriptors;
                }
                else
                {
                    bs.DataSource = mapDescriptors.FindAll(md => md.ID >= 0 && md.ID < 18);
                }
            } else
            {
                bs.DataSource = null;
            }

            dataGridView1.DataSource = bs;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                MapDescriptor mapDescriptor = senderGrid.Rows[e.RowIndex].DataBoundItem as MapDescriptor;
                if (senderGrid.Columns[e.ColumnIndex].Name.ToLower().Contains("export"))
                {
                    exportMd(mapDescriptor);
                }
                else if (senderGrid.Columns[e.ColumnIndex].Name.ToLower().Contains("import"))
                {
                    Console.WriteLine("Import " + mapDescriptor.Name_EN);
                }
            }
        }

        private void exportMd(MapDescriptor mapDescriptor)
        {
            using (var fbd = new FolderBrowserDialog())
            {
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
                        filesToBeReplaced += fileNameMd + '\n';
                    }
                    if (File.Exists(fileNameFrb1))
                    {
                        filesToBeReplaced += fileNameFrb1 + '\n';
                    }
                    if (fileNameFrb2 != null && File.Exists(fileNameFrb2))
                    {
                        filesToBeReplaced += fileNameFrb2 + '\n';
                    }
                    if (fileNameFrb3 != null && File.Exists(fileNameFrb3))
                    {
                        filesToBeReplaced += fileNameFrb3 + '\n';
                    }
                    if (fileNameFrb4 != null && File.Exists(fileNameFrb4))
                    {
                        filesToBeReplaced += fileNameFrb4 + '\n';
                    }

                    if(!string.IsNullOrWhiteSpace(filesToBeReplaced))
                    {
                        DialogResult dialogResult = MessageBox.Show("The following files already exist and will be replaced:" + '\n' + filesToBeReplaced, "Files already exist", MessageBoxButtons.OKCancel);

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
                        // byte[] content = Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(mapDescriptor.generateMapDescriptorFileContent()));
                        byte[] content = Encoding.UTF8.GetBytes(mapDescriptor.generateMapDescriptorFileContent());
                        fs.Write(content, 0, content.Length);
                    }
                    extractedFiles += fileNameMd + '\n';

                    progressBar.SetProgress(50, "Copying frb files...");

                    File.Copy(Path.Combine(fileSet.param_folder, mapDescriptor.FrbFile1 + ".frb"), fileNameFrb1);
                    extractedFiles += fileNameFrb1 + '\n';
                    if (fileNameFrb2 != null)
                    {
                        File.Copy(Path.Combine(fileSet.param_folder, mapDescriptor.FrbFile2 + ".frb"), fileNameFrb2);
                        extractedFiles += fileNameFrb2 + '\n';
                    }
                    if (fileNameFrb3 != null)
                    {
                        File.Copy(Path.Combine(fileSet.param_folder, mapDescriptor.FrbFile3 + ".frb"), fileNameFrb3);
                        extractedFiles += fileNameFrb3 + '\n';
                    }
                    if (fileNameFrb4 != null)
                    {
                        File.Copy(Path.Combine(fileSet.param_folder, mapDescriptor.FrbFile4 + ".frb"), fileNameFrb4);
                        extractedFiles += fileNameFrb4 + '\n';
                    }
                    progressBar.SetProgress(100, "Done");
                    progressBar.SetProgressBarText(extractedFiles);
                }
            }
        }
    }
}