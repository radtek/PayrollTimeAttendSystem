using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using InteractPayrollClient;

namespace InteractPayrollClient
{
    public partial class frmReadWriteFile : Form
    {
        clsISClientUtilities clsISClientUtilities;

        public frmReadWriteFile()
        {
            InitializeComponent();

            clsISClientUtilities = new clsISClientUtilities();

            this.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Form_Paint);

            //Cause Paint
            this.Refresh();
        }
    }
}
