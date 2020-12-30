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
        private MapDescriptor editMd = new MapDescriptor();

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

        private void DataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e != null && e.RowIndex < dataGridView1.Rows.Count)
            {
                var editedRow = dataGridView1.Rows[e.RowIndex] as DataGridViewRow;
                var editedMd = editedRow.DataBoundItem as MapDescriptor;
                editMd.set(editedMd);
            }
        }

        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e != null && e.RowIndex < dataGridView1.Rows.Count)
            {
                var editedRow = dataGridView1.Rows[e.RowIndex] as DataGridViewRow;
                var editedMd = editedRow.DataBoundItem as MapDescriptor;
                if (!editedMd.Equals(editMd))
                    editedMd.Dirty = true;
            }
            BindingSource bs = dataGridView1.DataSource as BindingSource;
            if (bs.Count > 42)
                buttonRemoveMap.Enabled = true;
            else
                buttonRemoveMap.Enabled = false;
            clearValidationIssues();
            List<MapDescriptor> mapDescriptors = (List<MapDescriptor>)((BindingSource)dataGridView1.DataSource).List;
            bool atLeastOneDirty = false;
            foreach (var md in mapDescriptors)
            {
                if (md.Dirty)
                    atLeastOneDirty = true;
            }
            buttonSaveConfiguration.Enabled = atLeastOneDirty;
            var validation = MapDescriptor.getPracticeBoards(mapDescriptors, out _, out _);
            if (!validation.Passed)
            {
                addValidationIssues(validation);
            }
            var categories = new Dictionary<int, int>();
            validation = MapDescriptor.getMapSets(mapDescriptors, out categories);
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

        private async void Go_Click(object sender, EventArgs e)
        {
            var outputFile = setOutputPathLabel.Text;

            if (String.IsNullOrWhiteSpace(outputFile) || outputFile.ToLower() == "none")
            {
                var inputFile = setInputISOLocation.Text;
                DialogResult dialogResult = MessageBox.Show("Do you want to patch the existing location?" + Environment.NewLine + inputFile + Environment.NewLine + Environment.NewLine + "Make sure you have a backup.", "Files already exist", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                    outputFile = inputFile;
                else
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
                    await patchProcess.saveWbfsIso(outputFile, GetMapDescriptors(), this.patchWiimmfi.Checked, progress, ct);

                    // TODO, better cleanup
                    Invoke((MethodInvoker)delegate
                    {
                        progressBar.ShowCheckbox("Cleanup temporary files.", false);
                        progressBar.callback = (c) => { if (c) patchProcess.cleanFull(); else patchProcess.cleanTemp(); };
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

        private void SaveFileDialog(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "ISO/WBFS Image|*.iso;*.wbfs;*.ciso|Directory (do not pack ISO/WBFS)|*";
            saveFileDialog1.Title = "Where shall the output be saved?";
            saveFileDialog1.OverwritePrompt = false;
            saveFileDialog1.FileName = Path.GetFileName(setInputISOLocation.Text);

            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                var outputFile = saveFileDialog1.FileName;
                if (saveFileDialog1.FilterIndex == 2)
                {
                    // remove the extension
                    outputFile = Path.Combine(Directory.GetParent(outputFile).FullName, Path.GetFileNameWithoutExtension(outputFile));
                }
                if (patchProcess.isOutputImageFileExtension(outputFile))
                {
                    bool overwrite = File.Exists(outputFile);
                    if (overwrite)
                    {
                        DialogResult dialogResult = MessageBox.Show("An iso/wbfs already exists at " + Environment.NewLine + outputFile + Environment.NewLine + Environment.NewLine + "Do you want to overwrite this wbfs/iso? Make sure you have a backup.", "Files already exist", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            setOutputPathLabel.Text = outputFile;
                        }
                    }
                    else
                    {
                        setOutputPathLabel.Text = outputFile;
                    }
                }
                else
                {
                    bool overwrite;
                    outputFile = patchProcess.doOutputDirectoryPathCorrections(outputFile, out overwrite);
                    if (overwrite)
                    {
                        DialogResult dialogResult = MessageBox.Show("An extracted iso/wbfs directory already exists at " + Environment.NewLine + outputFile + Environment.NewLine + Environment.NewLine + "Do you want to patch this location? Make sure you have a backup.", "Files already exist", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            setOutputPathLabel.Text = outputFile;
                        }
                    }
                    else
                    {
                        setOutputPathLabel.Text = outputFile;
                    }
                }
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

                    Go.Enabled = true;
                    clearListButton.Enabled = true;
                    buttonAddMap.Enabled = true;
                    setOutputLocationButton.Enabled = true;
                    buttonSaveConfiguration.Enabled = false;
                    buttonLoadConfiguration.Enabled = true;
                    BindingSource bs = new BindingSource();
                    bs.DataSource = mapDescriptors;
                    dataGridView1.DataSource = bs;
                    DataGridView1_CellEndEdit(null, null);

                    for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
                    {
                        DataGridViewColumn column = dataGridView1.Columns[i];
                        if (column.Name == "VentureCardActiveCount")
                        {
                            dataGridView1.Columns.RemoveAt(i);
                            if (!dataGridView1.Columns.Contains(VentureCards))
                                dataGridView1.Columns.Insert(i, VentureCards);
                            break;
                        }
                    }

                    for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
                    {
                        DataGridViewColumn column = this.dataGridView1.Columns[i];
                        // set autosizing
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
                        //store autosized widths
                        int colw = column.Width;
                        //remove autosizing
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        //set width to calculated by autosize
                        if (colw > 75 && column.Name.StartsWith("SwitchRotation"))
                            colw = 75;
                        if (colw > 100 && column.Name != "Name_EN")
                            colw = 100;
                        column.Width = colw;

                        column.Resizable = DataGridViewTriState.True;
                        if (column.Name == "ExportMd" || column.Name == "ImportMd")
                        {
                            column.Frozen = true;
                            column.Resizable = DataGridViewTriState.False;
                        }
                        else if (column.ReadOnly)
                        {
                            column.DefaultCellStyle = readOnlyColumnStyle;
                        }
                        else
                        {
                            column.DefaultCellStyle = editColumnStyle;
                            column.Frozen = true;
                            column.Width += 15;
                        }
                        if (column.Name == "Name_EN")
                        {
                            column.Frozen = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    patchProcess = null;
                    setInputISOLocation.Text = "None";
                    Go.Enabled = false;
                    clearListButton.Enabled = false;
                    buttonAddMap.Enabled = false;
                    buttonRemoveMap.Enabled = false;
                    setOutputLocationButton.Enabled = false;
                    buttonSaveConfiguration.Enabled = false;
                    buttonLoadConfiguration.Enabled = false;
                    progressBar.appendText(e.Message);
                    progressBar.appendText(Environment.NewLine + Environment.NewLine + e.ToString());
                    progressBar.EnableButton();
                    Debug.WriteLine(e.ToString());
                }
            }
        }

        private List<MapDescriptor> GetMapDescriptors()
        {
            if (dataGridView1.DataSource != null)
            {
                var bs = (BindingSource)dataGridView1.DataSource;
                if (bs.DataSource != null)
                {
                    return (List<MapDescriptor>)bs.DataSource;
                }
            }
            return null;
        }

        private void OpenFileDialog(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "ISO/WBFS Image or main.dol of extracted image|*.iso;*.wbfs;*.ciso;main.dol";
            openFileDialog1.Title = "Which ISO image or WBFS image or extracted image folder should we use for patching?";

            if (openFileDialog1.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(openFileDialog1.FileName))
            {
                var input = openFileDialog1.FileName;
                if (Path.GetFileName(input).ToLower() == "main.dol")
                {
                    input = Directory.GetParent(input).FullName;
                    input = Directory.GetParent(input).FullName;
                }
                setInputISOLocation.Text = input;
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

        private void addMap_Click(object sender, EventArgs e)
        {
            importMd(null);
            BindingSource bs = dataGridView1.DataSource as BindingSource;
            DataGridView1_CellEndEdit(null, null);
        }

        private void removeMap_Click(object sender, EventArgs e)
        {
            var md = GetMapDescriptors()[GetMapDescriptors().Count - 1];

            DialogResult dialogResult = MessageBox.Show("Are you sure you want to remove the map " + md.Name_EN + "?", "Remove Map", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                BindingSource bs = dataGridView1.DataSource as BindingSource;
                bs.RemoveAt(bs.Count - 1);
                DataGridView1_CellEndEdit(null, null);
            }
        }

        private void ButtonSaveConfiguration_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "CSV file|*.csv";
            saveFileDialog1.Title = "Where shall the configuration be saved?";
            saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(setInputISOLocation.Text) + ".csv";
            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                var output = saveFileDialog1.FileName;
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
                        Configuration.save(output, GetMapDescriptors(), progress, ct);
                    }
                    catch (Exception e2)
                    {
                        progressBar.appendText(e2.Message);
                        progressBar.appendText(Environment.NewLine + Environment.NewLine + e2.ToString());
                        progressBar.EnableButton();
                        Debug.WriteLine(e2.ToString());
                    }
                }
            }
        }

        private void ButtonLoadConfiguration_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "CSV file|*.csv";
            openFileDialog1.Title = "Which configuration shall be loaded?";
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(openFileDialog1.FileName))
            {
                var input = openFileDialog1.FileName;
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
                        Configuration.load(input, GetMapDescriptors(), patchProcess, progress, ct);
                        var bs = (BindingSource)dataGridView1.DataSource;
                        bs.ResetBindings(true);
                        DataGridView1_CellEndEdit(null, null);
                    }
                    catch (Exception e2)
                    {
                        progressBar.appendText(e2.Message);
                        progressBar.appendText(Environment.NewLine + Environment.NewLine + e2.ToString());
                        progressBar.EnableButton();
                        Debug.WriteLine(e2.ToString());
                    }
                }
            }
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
                else if (senderGrid.Columns[e.ColumnIndex].Name.ToLower().Contains("venturecards"))
                {
                    new VentureCardBox(mapDescriptor.VentureCard).ShowDialog(this);
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
            if (mapDescriptor == null)
                return;
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
                        var importedMapDescriptor = patchProcess.importMd(openFileDialog1.FileName, progress, ct);
                        if (mapDescriptor != null)
                        {
                            mapDescriptor.setFromImport(importedMapDescriptor);
                        }
                        else
                        {
                            BindingSource bs = dataGridView1.DataSource as BindingSource;
                            bs.Add(importedMapDescriptor);
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