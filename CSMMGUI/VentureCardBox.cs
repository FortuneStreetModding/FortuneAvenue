using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CustomStreetMapManager
{
    public partial class VentureCardBox : Form
    {
        private readonly byte[] ventureCards;

        public VentureCardBox(byte[] ventureCards)
        {
            InitializeComponent();
            this.ventureCards = ventureCards;
        }


        private void VentureCardBox_Shown(object sender, System.EventArgs e)
        {
            for (int i = 0; i < ventureCards.Length; i++)
            {
                var row = this.dataGridView1.Rows[i];
                var cell = row.Cells[1];
                var checkbox = cell as DataGridViewCheckBoxCell;
                checkbox.Value = ventureCards[i] != 0;
            }
        }
    }
}
