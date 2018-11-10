using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InteractPayroll
{
    public partial class frmCloseOrSave : Form
    {
        public frmCloseOrSave()
        {
            InitializeComponent();

            clsISUtilities clsISUtilities = new clsISUtilities();
            this.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Form_Paint);
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
