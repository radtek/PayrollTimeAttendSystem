using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InteractPayroll
{
    public partial class frmRestartService : Form
    {
        public frmRestartService(string stMachineName, string stMachineIP)
        {
            InitializeComponent();

            clsISUtilities clsISUtilities = new clsISUtilities(this, "busPayrollLogon");
            this.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Form_Paint);
            
            this.txtMachineName.Text = stMachineName;
            this.txtMachineIP.Text = stMachineIP;
        }
    }
}
