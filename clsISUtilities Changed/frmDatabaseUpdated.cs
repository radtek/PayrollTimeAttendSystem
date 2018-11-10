using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace InteractPayroll
{
    public partial class frmDatabaseUpdated : Form
    {
        public frmDatabaseUpdated()
        {
            InitializeComponent();
        }

        private void frmDatabaseUpdated_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.Close();
        }

        private void frmDatabaseUpdated_MouseClick(object sender, MouseEventArgs e)
        {
            this.Close();
        }
    }
}
