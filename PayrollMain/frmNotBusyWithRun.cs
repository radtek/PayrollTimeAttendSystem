using InteractPayroll;
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
    public partial class frmNotBusyWithRun : Form
    {
        public frmNotBusyWithRun()
        {
            InitializeComponent();

            if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
            {
                this.lblMessage.Text = "A Time Attendance Run Date needs to be Open for this option to be available.";
            }

            clsISUtilities clsISUtilities = new clsISUtilities();
            this.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Form_Paint);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
