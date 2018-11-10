using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InteractPayrollClient
{
    public partial class frmMessage : Form
    {
        clsISClientUtilities clsISClientUtilities;

        public frmMessage()
        {
            InitializeComponent();

            clsISClientUtilities = new clsISClientUtilities();

            this.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Form_Paint);
        }
    }
}
