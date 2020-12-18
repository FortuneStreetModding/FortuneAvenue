using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
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
using FSEditor.Exceptions;
using System.Runtime.InteropServices;

namespace CustomStreetManager
{
    public partial class CSMM : Form
    {
        PatchProcess patchProcess;
        readonly CancellationTokenSource exitTokenSource;
        readonly Dictionary<string, ToolStripMenuItem> xmlFileToToolStripMenuItemDict = new Dictionary<string, ToolStripMenuItem>();

        public CSMM()
        {
            InitializeComponent();
            if (!SystemInformation.TerminalServerSession)
            {
                Type dgvType = dataGridView1.GetType();
                PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
                pi.SetValue(dataGridView1, true, null);
            }
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            exitTokenSource = new CancellationTokenSource();
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            exitTokenSource.Cancel();
        }

        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            clearValidationIssues();
            List<MapDescriptor> mapDescriptors = (List<MapDescriptor>)((BindingSource)dataGridView1.DataSource).List;
            var validation = MapDescriptor.getPracticeBoards(mapDescriptors, out _, out _);
            if (!validation.Passed)
            {
                addValidationIssues(validation);
            }
            var categories = new Dictionary<int, int>();
            validation = MapDescriptor.getCategories(mapDescriptors, out categories);
            if (validation.Passed)
            {
                foreach (int category in categories.Values.Distinct())
                {
                    var zones = new Dictionary<int, int>();
                    validation = MapDescriptor.getZones(mapDescriptors, category, out zones);
                    if (validation.Passed)
                    {
                        foreach (int zone in zones.Values.Distinct())
                        {
                            var ordering = new Dictionary<int, int>();
                            validation = MapDescriptor.getOrdering(mapDescriptors, category, zone, out ordering);
                            if (!validation.Passed)
                            {
                                addValidationIssues(validation);
                            }
                        }
                    }
                    else
                    {
                        addValidationIssues(validation);
                    }
                }
            }
            else
            {
                addValidationIssues(validation);
            }
        }

        private void clearValidationIssues()
        {
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.Cells[column.Index].ErrorText = "";
                }
            }
        }

        private void addValidationIssues(MapDescriptorValidation validation)
        {
            var issues = validation.getIssues();
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                for (int i = 0; i < issues.Count; i++)
                {
                    var issue = issues[i];
                    var index = issue.index;
                    var prop = issue.prop;
                    var reason = issue.reason;
                    if (column.DataPropertyName == prop.Name)
                    {
                        if (index != -1)
                        {
                            var errorText = dataGridView1.Rows[index].Cells[column.Index].ErrorText;
                            if (!string.IsNullOrEmpty(errorText)) errorText += Environment.NewLine;
                            dataGridView1.Rows[index].Cells[column.Index].ErrorText = errorText + reason.ToString();
                        }
                        else
                        {
                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                var errorText = row.Cells[column.Index].ErrorText;
                                if (!string.IsNullOrEmpty(errorText)) errorText += Environment.NewLine;
                                row.Cells[column.Index].ErrorText = errorText + reason.ToString();
                            }
                        }
                    }
                }
            }
        }

        private void reset()
        {
            patchProcess.cleanUp(false, true);
            patchProcess = null;
            setInputISOLocation.Text = "None";
            Go.Enabled = false;
        }

        private async void Go_Click(object sender, EventArgs e)
        {
            var inputFile = setInputISOLocation.Text;
            var outputFile = setOutputPathLabel.Text;

            if (String.IsNullOrWhiteSpace(outputFile) || outputFile.ToLower() == "none")
            {
                MessageBox.Show("Please set the output file.");
                return;
            }

            using (var cancelTokenSource = new CancellationTokenSource())
            using (var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(exitTokenSource.Token, cancelTokenSource.Token))
            {
                CancellationToken ct = linkedTokenSource.Token;
                ProgressBar progressBar = new ProgressBar(verboseToolStripMenuItem.Checked);
                progressBar.callback = (b) => { try { cancelTokenSource?.Cancel(); } catch (ObjectDisposedException) { } };
                progressBar.Show(this);
                var progress = new Progress<ProgressInfo>(progressInfo =>
                {
                    progressBar.update(progressInfo);
                    Debug.WriteLine(progressInfo.line);
                });

                try
                {
                    await patchProcess.saveWbfsIso(inputFile, outputFile, false, progress, ct);

                    // TODO, better cleanup
                    Invoke((MethodInvoker)delegate
                    {
                        progressBar.ShowCheckbox("Cleanup temporary files.", false);
                        progressBar.callback = (c) => { if (c) patchProcess.cleanUp(true, true); };
                    });
                }
                catch (Exception e2)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        progressBar.appendText(e2.Message);
                        progressBar.appendText(Environment.NewLine + Environment.NewLine + e2.ToString());
                        progressBar.EnableButton();
                    });

                    Debug.WriteLine(e2.ToString());
                }
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

        private async void reloadWbfsIsoFile()
        {
            using (var cancelTokenSource = new CancellationTokenSource())
            using (var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(exitTokenSource.Token, cancelTokenSource.Token))
            {
                CancellationToken ct = linkedTokenSource.Token;
                ProgressBar progressBar = new ProgressBar(verboseToolStripMenuItem.Checked);
                progressBar.callback = (b) => { try { cancelTokenSource?.Cancel(); } catch (ObjectDisposedException) { } };
                progressBar.Show(this);
                var progress = new Progress<ProgressInfo>(progressInfo =>
                {
                    progressBar.update(progressInfo);
                    Debug.WriteLine(progressInfo.line);
                });

                var inputWbfsIso = setInputISOLocation.Text;
                patchProcess = new PatchProcess();
                try
                {
                    var mapDescriptors = await patchProcess.loadWbfsIsoFile(inputWbfsIso, progress, ct);

                    Go.Enabled = false;
                    BindingSource bs = new BindingSource();
                    bs.DataSource = mapDescriptors;
                    dataGridView1.DataSource = bs;
                }
                catch (Exception e)
                {
                    reset();
                    progressBar.appendText(e.Message);
                    progressBar.appendText(Environment.NewLine + Environment.NewLine + e.ToString());
                    progressBar.EnableButton();
                    Debug.WriteLine(e.ToString());
                }
                updateDataGridData(null, null);
                this.dataGridView1.AllowUserToAddRows = true;
                this.dataGridView1.AllowUserToDeleteRows = true;
            }
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
                reloadWbfsIsoFile();
            }
        }

        private void addRiivolutionPatchXML_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Riivolution Patch XML file|*.xml";
            openFileDialog.Title = "Which Riivolution Patch XML should we include?";

            if (openFileDialog.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(openFileDialog.FileName))
            {
                var xmlFile = openFileDialog.FileName;
                var xmlBaseFileName = Path.GetFileNameWithoutExtension(xmlFile);

                if (xmlFileToToolStripMenuItemDict.ContainsKey(xmlBaseFileName))
                {
                    MessageBox.Show("A Riivolution Patch XML file with the name " + xmlBaseFileName + " has already been added. The existing one will be replaced with the newly selected one.");
                }

                var newItem = new ToolStripMenuItem();
                newItem.CheckOnClick = true;
                newItem.Checked = true;
                newItem.ImageScaling = ToolStripItemImageScaling.None;
                newItem.Name = xmlFile;
                newItem.Size = new Size(257, 22);
                newItem.Text = xmlBaseFileName;
                xmlFileToToolStripMenuItemDict[xmlBaseFileName] = newItem;
                optionsToolStripMenuItem.DropDownItems.Add(newItem);
            }
        }

        private void clearListButton_Click(object sender, EventArgs e)
        {
            reloadWbfsIsoFile();
        }

        private void updateDataGridData(object sender, EventArgs e)
        {
            BindingSource bs = dataGridView1.DataSource as BindingSource;
            if (patchProcess != null && patchProcess.mapDescriptors != null)
            {
                if (checkBox1.Checked)
                {
                    bs.DataSource = patchProcess.mapDescriptors;
                }
                else
                {
                    bs.DataSource = patchProcess.mapDescriptors.FindAll(md => md.UnlockID >= 0 && md.UnlockID < 18);
                }
            }
            else
            {
                bs.DataSource = null;
            }

            dataGridView1.DataSource = bs;
        }

        private async void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                MapDescriptor mapDescriptor = senderGrid.Rows[e.RowIndex].DataBoundItem as MapDescriptor;
                if (senderGrid.Columns[e.ColumnIndex].Name.ToLower().Contains("export"))
                {
                    await exportMdAsync(mapDescriptor);
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
            if (mapDescriptor != null && mapDescriptor.Dirty)
            {
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
            }
        }

        private async Task exportMdAsync(MapDescriptor mapDescriptor)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Map Descriptor file and accompanying files|*.md";
            saveFileDialog1.Title = "Where shall the map files be exported?";
            saveFileDialog1.FileName = mapDescriptor.InternalName + ".md";
            saveFileDialog1.OverwritePrompt = false;

            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(saveFileDialog1.FileName))
            {
                using (var cancelTokenSource = new CancellationTokenSource())
                using (var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(exitTokenSource.Token, cancelTokenSource.Token))
                {
                    CancellationToken ct = linkedTokenSource.Token;
                    ProgressBar progressBar = new ProgressBar(verboseToolStripMenuItem.Checked);
                    progressBar.callback = (b) => { try { cancelTokenSource?.Cancel(); } catch (ObjectDisposedException) { } };
                    progressBar.Show(this);
                    var progress = new Progress<ProgressInfo>(progressInfo =>
                    {
                        progressBar.update(progressInfo);
                        Debug.WriteLine(progressInfo.line);
                    });

                    bool overwrite = false;

                tryExportMd:
                    try
                    {
                        string extractedFiles = await patchProcess.exportMd(saveFileDialog1.FileName, mapDescriptor, overwrite, progress, ct);
                        progressBar.appendText(extractedFiles);
                    }
                    catch (FileAlreadyExistException e1)
                    {
                        DialogResult dialogResult = MessageBox.Show(e1.Message, "Files already exist", MessageBoxButtons.OKCancel);
                        if (dialogResult == DialogResult.OK)
                        {
                            overwrite = true;
                            goto tryExportMd;
                        }
                    }
                    catch (Exception e)
                    {
                        progressBar.appendText(e.Message);
                        progressBar.appendText(Environment.NewLine + Environment.NewLine + e.ToString());
                        progressBar.EnableButton();
                        Debug.WriteLine(e.ToString());
                    }
                }
            }
        }

        private void importMd(MapDescriptor mapDescriptor)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Map Descriptor File (.md)|*.md";
            openFileDialog1.Title = "Which Map to import?";

            if (openFileDialog1.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(openFileDialog1.FileName))
            {
                using (var cancelTokenSource = new CancellationTokenSource())
                using (var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(exitTokenSource.Token, cancelTokenSource.Token))
                {
                    CancellationToken ct = linkedTokenSource.Token;
                    ProgressBar progressBar = new ProgressBar(verboseToolStripMenuItem.Checked);
                    progressBar.callback = (b) => { try { cancelTokenSource?.Cancel(); } catch (ObjectDisposedException) { } };
                    progressBar.Show(this);
                    var progress = new Progress<ProgressInfo>(progressInfo =>
                    {
                        progressBar.update(progressInfo);
                        Debug.WriteLine(progressInfo.line);
                    });

                    try
                    {
                        patchProcess.importMd(openFileDialog1.FileName, mapDescriptor, progress, ct);

                        Go.Enabled = true;
                        updateDataGridData(null, null);
                    }
                    catch (Exception e)
                    {
                        progressBar.appendText(e.Message);
                        progressBar.appendText(Environment.NewLine + Environment.NewLine + e.ToString());
                        progressBar.EnableButton();
                        Debug.WriteLine(e.ToString());
                    }
                }
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenBrowser("https://github.com/FortuneStreetModding/FortuneAvenue/wiki/CSMM-User-Guide");
        }

        public static void OpenBrowser(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                MessageBox.Show("Visit " + url + " for help");
            }
        }
    }
}