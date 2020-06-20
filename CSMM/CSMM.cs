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

namespace CustomStreetManager
{
    public partial class CSMM : Form
    {
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
                    instance.SetButtonToClose();
                }
                else
                {
                    instance.SetProgressBarLabel("Error: The output file path cannot be blank!");
                    instance.SetProgressBarValue(100);
                    instance.SetButtonToGoBack();
                }
            }
            else
            {
                instance.SetProgressBarLabel("Error: The source ISO could not be opened.");
                instance.SetProgressBarValue(100);
                instance.SetButtonToGoBack();
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

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileSet fileSet = WitWrapper.extractFiles(openFileDialog1.FileName);
                List<MainDolSection> sections = WitWrapper.readSections(fileSet.main_dol);
                MainDol mainDol = new MainDol(sections);
                setInputISOLocation.Text = openFileDialog1.FileName;

                using (var stream = File.OpenRead(fileSet.main_dol))
                {
                    MiscUtil.IO.EndianBinaryReader binReader = new MiscUtil.IO.EndianBinaryReader(MiscUtil.Conversion.EndianBitConverter.Big, stream);
                    List<MapDescriptor> mapDescriptors = mainDol.ReadMainDol(binReader);
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
                        mapDescriptor.Name_DE = en.get(mapDescriptor.Name_MSG_ID);
                        mapDescriptor.Name_FR = en.get(mapDescriptor.Name_MSG_ID);
                        mapDescriptor.Name_IT = en.get(mapDescriptor.Name_MSG_ID);
                        mapDescriptor.Name_SU = en.get(mapDescriptor.Name_MSG_ID);
                        mapDescriptor.Name_JP = en.get(mapDescriptor.Name_MSG_ID);
                        mapDescriptor.Name_UK = en.get(mapDescriptor.Name_MSG_ID);

                        mapDescriptor.Desc_EN = en.get(mapDescriptor.Name_MSG_ID);
                        mapDescriptor.Desc_DE = en.get(mapDescriptor.Name_MSG_ID);
                        mapDescriptor.Desc_FR = en.get(mapDescriptor.Name_MSG_ID);
                        mapDescriptor.Desc_IT = en.get(mapDescriptor.Name_MSG_ID);
                        mapDescriptor.Desc_SU = en.get(mapDescriptor.Name_MSG_ID);
                        mapDescriptor.Desc_JP = en.get(mapDescriptor.Name_MSG_ID);
                        mapDescriptor.Desc_UK = en.get(mapDescriptor.Name_MSG_ID);
                    }
                    // Convert to DataTable.
                    DataTable table = DataTableHelper.ToDataTable(mapDescriptors);
                    dataGridView1.DataSource = table;
                    checkBox1_CheckedChanged(null, null);
                }
            }
            else
            {
                setInputISOLocation.Text = "None";
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
            if (checkBox1.Checked)
            {
                BindingSource bs = new BindingSource();
                bs.DataSource = dataGridView1.DataSource;
                bs.Filter = "";
                dataGridView1.DataSource = bs;
            }
            else
            {
                BindingSource bs = new BindingSource();
                bs.DataSource = dataGridView1.DataSource;
                bs.Filter = "ID >= 0 AND ID < 18";
                dataGridView1.DataSource = bs;
            }
        }
    }
}