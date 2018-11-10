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
    public partial class frmDateLoad : Form
    {
        clsISUtilities clsISUtilities;

        private string pvtstrType = "";

        public frmDateLoad(string parstrType)
        {
            pvtstrType = parstrType;
            clsISUtilities = new clsISUtilities(this,"busDateLoad");

            InitializeComponent();
        }

        private void tmrDateLoad_Tick(object sender, System.EventArgs e)
        {
            try
            {
                tmrDateLoad.Enabled = false;

                object[] objParm = new object[1];
                objParm[0] = pvtstrType;
                
                clsISUtilities.DynamicFunction("Insert_Dates", objParm);
                
                this.Close();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }
    }
}
