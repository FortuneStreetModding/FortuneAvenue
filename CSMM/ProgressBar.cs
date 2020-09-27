using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CustomStreetManager
{
    public partial class ProgressBar : Form
    {
        public Action<bool> callback;

        public ProgressBar()
        {
            InitializeComponent();
        }

        public void SetProgressBarValue(int num)
        {
            if (num != 100)
            {
                mapReplaceProgressBar.Value = num + 1;
                mapReplaceProgressBar.Value--;
            }
            else
            {
                mapReplaceProgressBar.Value = num;
                cancelButton.Enabled = true;
            }
        }

        public void EnableButton()
        {
            if (!cancelButton.IsDisposed)
                cancelButton.Enabled = true;
        }

        public void ShowCheckbox(string text, bool isChecked)
        {
            checkBox1.Text = text;
            checkBox1.Checked = isChecked;
            checkBox1.Visible = true;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void update(ProgressInfo progressInfo)
        {
            if (progressInfo.progress > 0)
            {
                SetProgressBarValue(progressInfo.progress);
            }
            string lastLine = "";
            if (textBox.Lines.Length >= 2)
            {
                lastLine = textBox.Lines[textBox.Lines.Length - 2];
            }
            if (progressInfo.stdLine != null)
            {
                if (lastLine.ToLower() != progressInfo.stdLine.ToLower())
                    textBox.AppendText(progressInfo.stdLine + Environment.NewLine);
            }
            Update();
        }

        public void appendText(string text)
        {
            if (!textBox.IsDisposed)
                textBox.AppendText(text);
        }

        private void ProgressBar_FormClosed(object sender, FormClosedEventArgs e)
        {
            callback?.Invoke(checkBox1.Checked);
        }
    }
}
