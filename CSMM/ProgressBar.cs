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
        public ProgressBar()
        {
            InitializeComponent();
        }

        public void SetProgress(int num, string label)
        {
            SetProgressBarLabel(label);
            SetProgressBarValue(num);
        }

        public void SetProgressBarLabel(string text)
        {
            progressLabel.Text = text;
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
            cancelButton.Enabled = true;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        internal void update(int percentage, string standardOutput, string errorOutput)
        {
            if (percentage != -1)
            {
                SetProgressBarValue(percentage);
            }
            if (standardOutput != null)
            {
                textArea.Text += standardOutput;
            }
            if (errorOutput != null)
            {
                textArea.Text += errorOutput;
            }
            textArea.Update();
        }
    }
}
