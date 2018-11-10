using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using InteractPayrollClient;

namespace InteractPayroll
{
    public partial class frmSetupLocalWebServer : Form
    {
        string[] strParts;
        string[] strIP;

        string strURLPath = "";

        clsISClientUtilities clsISClientUtilities;

        public frmSetupLocalWebServer()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                double dblTest = 0;

                if (double.TryParse(this.txtIP1.Text, out dblTest) == false
                | double.TryParse(this.txtIP2.Text, out dblTest) == false
                | double.TryParse(this.txtIP3.Text, out dblTest) == false
                | double.TryParse(this.txtIP4.Text, out dblTest) == false
                | double.TryParse(this.txtPortNumber.Text, out dblTest) == false)
                {
                    CustomClientMessageBox.Show("Setup a Valid IP / Port Number.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }
                else
                {
                    clsISClientUtilities = null;
                    clsISClientUtilities = new clsISClientUtilities(this, "busTimeAttendanceLogon");

                    int intReturnCode = clsISClientUtilities.WebService_Ping_Test();

                    if (intReturnCode == 0)
                    {
                        CustomClientMessageBox.Show("Communication Successful.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        //Form with Unsuccessful would show
                    }
                }
            }
            catch (Exception eException)
            {
            }
        }

        private void frmSetupLocalWebServer_Load(object sender, EventArgs e)
        {
            FileInfo fiFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "URLClientConfig.txt");

            if (fiFileInfo.Exists == true)
            {
                StreamReader srStreamReader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "URLClientConfig.txt");

                strURLPath = srStreamReader.ReadLine();

                srStreamReader.Close();
           
                strParts = strURLPath.Split(':');

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
                        CustomClientMessageBox.Show("Error in 'URLClientConfig.txt'.\n\nSpeak to System Administrator", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    CustomClientMessageBox.Show("Error in 'URLClientConfig.txt'.\n\nSpeak to System Administrator", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Load_IP_And_Port_And_Disable()
        {
            if (strURLPath != "")
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
            }
            else
            {
                this.rbnLocalHost.Checked = false;
                this.rbnServer.Checked = false;

                this.txtIP1.Text = "";
                this.txtIP2.Text = "";
                this.txtIP3.Text = "";
                this.txtIP4.Text = "";

                this.txtPortNumber.Text = "";
            }

            this.rbnLocalHost.Enabled = false;
            this.rbnServer.Enabled = false;

            this.txtIP1.Enabled = false;
            this.txtIP2.Enabled = false;
            this.txtIP3.Enabled = false;
            this.txtIP4.Enabled = false;

            this.txtPortNumber.Enabled = false;
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

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            this.Text += " - Update";

            this.btnUpdate.Enabled = false;
            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            if (this.rbnLocalHost.Checked == false
            && this.rbnServer.Checked == false)
            {
                this.rbnLocalHost.Checked = true;
                rbnLocalHost_Click(sender, e);
            }
            else
            {
                if (this.rbnServer.Checked == true)
                {
                    this.txtIP1.Enabled = true;
                    this.txtIP2.Enabled = true;
                    this.txtIP3.Enabled = true;
                    this.txtIP4.Enabled = true;
                }
            }
            
            this.rbnLocalHost.Enabled = true;
            this.rbnServer.Enabled = true;

            this.txtPortNumber.Enabled = true;

            this.btnTest.Enabled = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Text = this.Text.Substring(0, this.Text.IndexOf("- Update") - 1);

            this.btnUpdate.Enabled = true;
            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.btnTest.Enabled = true;

            Load_IP_And_Port_And_Disable();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.txtIP1.Text.Trim() == "")
            {
                CustomClientMessageBox.Show("Enter IP Value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.txtIP1.Focus();
                return;
            }

            if (this.txtIP2.Text.Trim() == "")
            {
                CustomClientMessageBox.Show("Enter IP Value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.txtIP2.Focus();
                return;
            }

            if (this.txtIP3.Text.Trim() == "")
            {
                CustomClientMessageBox.Show("Enter IP Value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.txtIP3.Focus();
                return;
            }

            if (this.txtIP4.Text.Trim() == "")
            {
                CustomClientMessageBox.Show("Enter IP Value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.txtIP4.Focus();
                return;
            }

            if (this.txtPortNumber.Text.Trim() == "")
            {
                CustomClientMessageBox.Show("Enter Port Number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

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

            strParts[0] = this.txtIP1.Text + "." + this.txtIP2.Text + "." + this.txtIP3.Text + "." + this.txtIP4.Text;
            strParts[1] = this.txtPortNumber.Text;

            FileInfo fiFileInfo = new FileInfo("URLClientConfig.txt");

            if (fiFileInfo.Exists == true)
            {
                File.Delete("URLClientConfig.txt");
            }

            StreamWriter swStreamWriter = fiFileInfo.AppendText();

            swStreamWriter.WriteLine(strNewIpPort);

            swStreamWriter.Close();

#if(DEBUG)
            if (AppDomain.CurrentDomain.GetData("URLClientPath").ToString() != "")
            {
                AppDomain.CurrentDomain.SetData("URLClientPath", strNewIpPort);
            }
            else
            {
                DialogResult myDialogResult = MessageBox.Show("Would you like to use the CLIENT Web Service Layer?",
               "Error",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Warning);

                if (myDialogResult == System.Windows.Forms.DialogResult.Yes)
                {
                    AppDomain.CurrentDomain.SetData("URLClientPath", strNewIpPort);
                }
                else
                {
                    AppDomain.CurrentDomain.SetData("URLClientPath", "");
                }
            }
#else
            AppDomain.CurrentDomain.SetData("URLClientPath", strNewIpPort);
#endif

            btnCancel_Click(sender, e);

            CustomClientMessageBox.Show("Data Saved.", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
