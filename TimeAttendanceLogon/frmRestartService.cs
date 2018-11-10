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
    public partial class frmRestartService : Form
    {
        public frmRestartService(string stMachineName, string stMachineIP)
        {
            InitializeComponent();

            clsISClientUtilities clsISClientUtilities = new clsISClientUtilities();
            this.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Form_Paint);

            //Causes Repaint
            this.Refresh();
            
            this.txtMachineName.Text = stMachineName;
            this.txtMachineIP.Text = stMachineIP;
        }
    }
}
