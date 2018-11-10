using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace InteractPayrollClient
{
    public partial class frmConnectionSetup : Form
    {
        string[] strParts;
        string[] strIP;

        clsISClientUtilities clsISClientUtilities;
        
        public frmConnectionSetup()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                clsISClientUtilities = null;
                clsISClientUtilities = new clsISClientUtilities(this, "busTimeAttendanceLogon");

                int intReturnCode = clsISClientUtilities.WebService_Ping_Test();

                if (intReturnCode == 0)
                {
                    System.Windows.Forms.MessageBox.Show("Communication Successful.", "Communication", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    //Form with Unsuccessful would show
                }
            }
            catch (Exception eException)
            {
            }
        }

        private void frmConnectionSetup_Load(object sender, EventArgs e)
        {
            clsISClientUtilities = new clsISClientUtilities(this, "busTimeAttendanceLogon");

            this.lblHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(clsISClientUtilities.lblHeader_MouseDown);
            this.lblHeader.MouseMove += new System.Windows.Forms.MouseEventHandler(clsISClientUtilities.lblHeader_MouseMove);
            this.lblHeader.MouseUp += new System.Windows.Forms.MouseEventHandler(clsISClientUtilities.lblHeader_MouseUp);
            this.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Form_Paint);

            //Cause Repaint
            this.Refresh();

            if (AppDomain.CurrentDomain.GetData("URLClientPath").ToString() != "")
            {
                strParts = AppDomain.CurrentDomain.GetData("URLClientPath").ToString().Split(':');

                if (strParts.Length == 2)
                {
                    strIP = strParts[0].Split('.');

                    if (strIP.Length == 4)
                    {
                        Load_IP_And_Port_And_Disable();
   
                        this.btnTest.Enabled = true;
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Error in 'URLClientConfig.txt'.\n\nSpeak to System Administrator","File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Error in 'URLClientConfig.txt'.\n\nSpeak to System Administrator", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Load_IP_And_Port_And_Disable()
        {
            if (strParts[0] == "127.0.0.1")
            {
                this.rbnLocalHost.Checked = true;
            }
            else
            {
                this.rbnServer.Checked = true;
            }

            this.txtIP1.Text = strIP[0];
            this.txtIP2.Text = strIP[1];
            this.txtIP3.Text = strIP[2];
            this.txtIP4.Text = strIP[3];

            this.txtPortNumber.Text = strParts[1];

            this.rbnLocalHost.Enabled = false;
            this.rbnServer.Enabled = false;

            this.txtIP1.Enabled = false;
            this.txtIP2.Enabled = false;
            this.txtIP3.Enabled = false;
            this.txtIP4.Enabled = false;

            this.txtPortNumber.Enabled = false;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            this.btnUpdate.Enabled = false;
            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            if (this.rbnServer.Checked == true)
            {
                this.txtIP1.Enabled = true;
                this.txtIP2.Enabled = true;
                this.txtIP3.Enabled = true;
                this.txtIP4.Enabled = true;
            }

            this.rbnLocalHost.Enabled = true;
            this.rbnServer.Enabled = true;

            this.txtPortNumber.Enabled = true;

            this.btnTest.Enabled = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.btnUpdate.Enabled = true;
            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.btnTest.Enabled = true;
        
            Load_IP_And_Port_And_Disable();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (this.rbnLocalHost.Checked == false
                & this.rbnServer.Checked == false)
            {
                System.Windows.Forms.MessageBox.Show("Choose Local (LocalHost) or Web Server.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                //this.rbnLocalHost.Focus();
                return;
            }

            if (this.txtIP1.Text.Trim() == "")
            {
                System.Windows.Forms.MessageBox.Show("Enter IP Value.","Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.txtIP1.Focus();
                return;
            }

            if (this.txtIP2.Text.Trim() == "")
            {
                System.Windows.Forms.MessageBox.Show("Enter IP Value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.txtIP2.Focus();
                return;
            }

            if (this.txtIP3.Text.Trim() == "")
            {
                System.Windows.Forms.MessageBox.Show("Enter IP Value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.txtIP3.Focus();
                return;
            }

            if (this.txtIP4.Text.Trim() == "")
            {
                System.Windows.Forms.MessageBox.Show("Enter IP Value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.txtIP4.Focus();
                return;
            }

            if (this.txtPortNumber.Text.Trim() == "")
            {
                System.Windows.Forms.MessageBox.Show("Enter Port Number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.txtPortNumber.Focus();
                return;
            }

            string strNewIpPort = this.txtIP1.Text + "." + this.txtIP2.Text + "." + this.txtIP3.Text + "." + this.txtIP4.Text + ":" + this.txtPortNumber.Text;

            strIP = new string[4];

            strIP[0] = this.txtIP1.Text;
            strIP[1] = this.txtIP2.Text;
            strIP[2] = this.txtIP3.Text;
            strIP[3] = this.txtIP4.Text;

            strParts = new string[2];

            strParts[0] = strNewIpPort;
            strParts[1] = this.txtPortNumber.Text;

            FileInfo fiFileInfo = new FileInfo("URLClientConfig.txt");

            if (fiFileInfo.Exists == true)
            {
                File.Delete("URLClientConfig.txt");
            }

            StreamWriter swStreamWriter = fiFileInfo.AppendText();

            swStreamWriter.WriteLine(strNewIpPort);

            swStreamWriter.Close();

            AppDomain.CurrentDomain.SetData("URLClientPath", strNewIpPort);

            btnCancel_Click(sender, e);

            System.Windows.Forms.MessageBox.Show("Data Saved.", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void rbnLocalHost_Click(object sender, EventArgs e)
        {
            this.txtIP1.Text = "127";
            this.txtIP2.Text = "0";
            this.txtIP3.Text = "0";
            this.txtIP4.Text = "1";

            this.txtIP1.Enabled = false;
            this.txtIP2.Enabled = false;
            this.txtIP3.Enabled = false;
            this.txtIP4.Enabled = false;
        }

        private void rbnServer_Click(object sender, EventArgs e)
        {
            this.txtIP1.Enabled = true;
            this.txtIP2.Enabled = true;
            this.txtIP3.Enabled = true;
            this.txtIP4.Enabled = true;
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

        private void btnHeaderClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
