using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Editor
{
    partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
            this.Text = String.Format("About {0}", "Fortune Avenue");
            this.labelProductName.Text = "Fortune Avenue";
            this.labelVersion.Text = "Version 9 Beta 6";
            this.labelCopyright.Text = "© 2012, 2013, 2019, 2020";
            this.labelCompanyName.Text = "";
            this.textBoxDescription.Text = "Based on the Fortune Street Wii Board Editor begun by Xanares, Amazing Ampharos, and zerozerozerozer.\r\nFurther work done by STJrInuyasha, Raspberryfloof, Deflaktor, Nikkums.\r\n\r\nFortune Street is ™ and © Nintendo, ARMOR PROJECT, and SQUARE ENIX.";
        }
    }
}
