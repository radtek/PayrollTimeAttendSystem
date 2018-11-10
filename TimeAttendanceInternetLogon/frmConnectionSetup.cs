using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace InteractPayroll
{
    public partial class frmConnectionSetup : Form
    {
        clsISUtilities clsISUtilities;

        private bool pvtblnNoUrl = false;

        Cursor csrSavedCusor;

        string[] strParts;
        string[] strIP;

        public frmConnectionSetup()
        {
            InitializeComponent();

            clsISUtilities = null;
            clsISUtilities = new clsISUtilities(this, "busPayrollLogon");

            clsISUtilities.Set_WebService_Timeout_Value(15000);

            this.lblHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(clsISUtilities.lblHeader_MouseDown);
            this.lblHeader.MouseMove += new System.Windows.Forms.MouseEventHandler(clsISUtilities.lblHeader_MouseMove);
            this.lblHeader.MouseUp += new System.Windows.Forms.MouseEventHandler(clsISUtilities.lblHeader_MouseUp);
            this.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Form_Paint);
        }

        private void frmConnectionSetup_Load(object sender, EventArgs e)
        {
#if(DEBUG)
            if (AppDomain.CurrentDomain.GetData("URLPath").ToString() == "")
            {
                AppDomain.CurrentDomain.SetData("URLPath", "196.220.34.143");

                pvtblnNoUrl = true;
            }
#endif
            if (AppDomain.CurrentDomain.GetData("URLPath").ToString() != "")
            {
                strIP = AppDomain.CurrentDomain.GetData("URLPath").ToString().Trim().Split('.');

                if (pvtblnNoUrl == true)
                {
                    AppDomain.CurrentDomain.SetData("URLPath", "");
                }
              
                if (strIP.Length == 4)
                {
                    Load_IP_Values();
                }
                else
                {
                    MessageBox.Show("Error in 'URLPath.txt'.\n\nSpeak to System Administrator", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
              
            }
        }

        private void Load_IP_Values()
        {
            this.txtIP1.Text = strIP[0];
            this.txtIP2.Text = strIP[1];
            this.txtIP3.Text = strIP[2];
            this.txtIP4.Text = strIP[3];
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.SetData("KillApp", "Y");
            this.Close();
        }
      
        private void Numeric_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= 48 && e.KeyChar <= 57)
            {
            }
            else
            {
                if (e.KeyChar == 8)
                {
                }
                else
                {
                    e.Handled = true;
                }
            }
        }
        
        private void btnTest_Click(object sender, EventArgs e)
        {
            if (this.txtIP1.Text.Trim() == "")
            {
                MessageBox.Show("Enter IP Value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.txtIP1.Focus();
                return;
            }

            if (this.txtIP2.Text.Trim() == "")
            {
                MessageBox.Show("Enter IP Value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.txtIP2.Focus();
                return;
            }

            if (this.txtIP3.Text.Trim() == "")
            {
                MessageBox.Show("Enter IP Value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.txtIP3.Focus();
                return;
            }

            if (this.txtIP4.Text.Trim() == "")
            {
                MessageBox.Show("Enter IP Value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.txtIP4.Focus();
                return;
            }

            this.txtIP1.Enabled = false;
            this.txtIP2.Enabled = false;
            this.txtIP3.Enabled = false;
            this.txtIP4.Enabled = false;
            
            string strNewURL = this.txtIP1.Text + "." + this.txtIP2.Text + "." + this.txtIP3.Text + "." + this.txtIP4.Text;
#if (DEBUG)
#else
            AppDomain.CurrentDomain.SetData("URLPath", strNewURL);
#endif
            string strOk = "";

            try
            {
                strOk = (string)clsISUtilities.DynamicFunction("Ping", null);
            }
            catch
            {
            }

            if (strOk == "OK")
            {
                MessageBox.Show("Communication Successful.", "Communication", MessageBoxButtons.OK, MessageBoxIcon.Information);
#if (DEBUG)
#else
                string strNewIpWebServerName = this.txtIP1.Text + "." + this.txtIP2.Text + "." + this.txtIP3.Text + "." + this.txtIP4.Text;

                //Set To New
                AppDomain.CurrentDomain.SetData("URLPath", strNewIpWebServerName);
#endif
                this.Close();
            }
            else
            {
                this.txtIP1.Enabled = true;
                this.txtIP2.Enabled = true;
                this.txtIP3.Enabled = true;
                this.txtIP4.Enabled = true;
            }
        }

        private void btnClose_MouseEnter(object sender, EventArgs e)
        {
            csrSavedCusor = this.Cursor;

            this.Cursor = Cursors.Default;
        }

        private void btnClose_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = csrSavedCusor;
        }

        private void btnHeaderClose_Click(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.SetData("KillApp", "Y");
            this.Close();
        }
    }
}
