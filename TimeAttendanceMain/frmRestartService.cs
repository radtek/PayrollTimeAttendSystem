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

            this.txtMachineName.Text = stMachineName;
            this.txtMachineIP.Text = stMachineIP;
        }
    }
}
