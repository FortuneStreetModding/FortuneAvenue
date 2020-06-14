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

        private void ProgressBar_Load(object sender, EventArgs e)
        {

        }

        public void SetProgressBarLabel(string text)
        {
            progressLabel.Text = text;
        }

        public void SetProgressBarValue(int num)
        {
            if(num != 100)
            {
                mapReplaceProgressBar.Value = num + 1;
                mapReplaceProgressBar.Value--;
            }
            else
            {
                mapReplaceProgressBar.Value = num;
            }
        }

        public void SetButtonToClose()
        {
            cancelButton.Text = "Close";
        }

        public void SetButtonToGoBack()
        {
            cancelButton.Text = "Go Back";
        }

        private void progressLabel_Click(object sender, EventArgs e)
        {

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
            if (cancelButton.Text == "Go Back")
            {
                //I don't want to quit the app if there's an error, but I do want to remove the working directory folder.
                if (Directory.Exists("..\\_working_directory\\"))
                {
                    Directory.Delete("..\\_working_directory\\");
                }
            }
            else if (cancelButton.Text == "Close") 
            {
                Application.Exit();
            }
        }
    }
}
