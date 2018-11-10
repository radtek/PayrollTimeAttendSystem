using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using InteractPayroll;

namespace InteractPayrollClient
{
    public partial class frmSetupInternetWebServerIP : Form
    {
        ToolStripMenuItem miLinkedMenuItem;

        string[] strIP;

        string strURLPath = "";

        clsISUtilities clsISUtilities;

        Cursor csrSavedCusor;

        public frmSetupInternetWebServerIP()
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
                | double.TryParse(this.txtIP4.Text, out dblTest) == false)
                {
                    CustomMessageBox.Show("Setup a Valid IP Number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }
                else
                {
                    object[] obj = null;

                    string strOk = (string)clsISUtilities.DynamicFunction("Ping", obj);

                    if (strOk == "OK")
                    {
                        CustomMessageBox.Show("Communication Successful.", "Communication", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                clsISUtilities.ErrorHandler(ex);
            }
        }

        private void frmSetupInternetWebServerIP_Load(object sender, EventArgs e)
        {
            miLinkedMenuItem = (ToolStripMenuItem)AppDomain.CurrentDomain.GetData("LinkedMenuItem");

            clsISUtilities = null;
            clsISUtilities = new clsISUtilities(this, "busPayrollLogon");

            clsISUtilities.Set_WebService_Timeout_Value(50000);

            FileInfo fiFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "URLConfig.txt");

            if (fiFileInfo.Exists == true)
            {
                StreamReader srStreamReader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "URLConfig.txt");

                strURLPath = srStreamReader.ReadLine();

                srStreamReader.Close();

                strIP = strURLPath.Split('.');

                if (strIP.Length == 4)
                {
                    Load_IP_And_Disable();

                    this.btnTest.Enabled = true;
                }
                else
                {
                    CustomMessageBox.Show("Error in 'URLConfig.txt'.\n\nSpeak to System Administrator", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
        }

        private void Load_IP_And_Disable()
        {
            if (strURLPath != "")
            {
                this.txtIP1.Text = strIP[0];
                this.txtIP2.Text = strIP[1];
                this.txtIP3.Text = strIP[2];
                this.txtIP4.Text = strIP[3];
            }
            else
            {
                this.txtIP1.Text = "";
                this.txtIP2.Text = "";
                this.txtIP3.Text = "";
                this.txtIP4.Text = "";
            }

            this.txtIP1.Enabled = false;
            this.txtIP2.Enabled = false;
            this.txtIP3.Enabled = false;
            this.txtIP4.Enabled = false;
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
            
            this.txtIP1.Enabled = true;
            this.txtIP2.Enabled = true;
            this.txtIP3.Enabled = true;
            this.txtIP4.Enabled = true;
            
            this.btnTest.Enabled = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Text = this.Text.Substring(0, this.Text.IndexOf(" - Update"));

            this.btnUpdate.Enabled = true;
            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.btnTest.Enabled = true;

            Load_IP_And_Disable();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.txtIP1.Text.Trim() == "")
            {
                CustomMessageBox.Show("Enter IP Value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.txtIP1.Focus();
                return;
            }

            if (this.txtIP2.Text.Trim() == "")
            {
                CustomMessageBox.Show("Enter IP Value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                this.txtIP2.Focus();
                return;
            }

            if (this.txtIP3.Text.Trim() == "")
            {
                CustomMessageBox.Show("Enter IP Value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                this.txtIP3.Focus();
                return;
            }

            if (this.txtIP4.Text.Trim() == "")
            {
                CustomMessageBox.Show("Enter IP Value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                this.txtIP4.Focus();
                return;
            }

            strURLPath = this.txtIP1.Text + "." + this.txtIP2.Text + "." + this.txtIP3.Text + "." + this.txtIP4.Text;

            strIP = new string[4];

            strIP[0] = this.txtIP1.Text;
            strIP[1] = this.txtIP2.Text;
            strIP[2] = this.txtIP3.Text;
            strIP[3] = this.txtIP4.Text;

            FileInfo fiFileInfo = new FileInfo("URLConfig.txt");

            if (fiFileInfo.Exists == true)
            {
                File.Delete("URLConfig.txt");
            }

            StreamWriter swStreamWriter = fiFileInfo.AppendText();

            swStreamWriter.WriteLine(strURLPath);

            swStreamWriter.Close();

#if(DEBUG)
            if (AppDomain.CurrentDomain.GetData("URLPath").ToString() != "")
            {
                AppDomain.CurrentDomain.SetData("URLPath", strURLPath);
            }
            else
            {
                DialogResult myDialogResult = CustomMessageBox.Show("Would you like to use the CLIENT Web Service Layer?",
               "Error",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Warning);

                if (myDialogResult == System.Windows.Forms.DialogResult.Yes)
                {
                    AppDomain.CurrentDomain.SetData("URLPath", strURLPath);
                }
                else
                {
                    AppDomain.CurrentDomain.SetData("URLPath", "");
                }
            }
#else
            AppDomain.CurrentDomain.SetData("URLPath", strURLPath);
#endif

            btnCancel_Click(sender, e);

            CustomMessageBox.Show("Data Saved.", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.SetData("KillApp", "Y");

            this.Close();
        }

        private void frmSetupInternetWebServerIP_FormClosing(object sender, FormClosingEventArgs e)
        {
            miLinkedMenuItem.Enabled = true;
        }
      
        private void btnClose_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = csrSavedCusor;
        }

        private void btnClose_MouseEnter(object sender, EventArgs e)
        {
            csrSavedCusor = this.Cursor;

            this.Cursor = Cursors.Default;
        }
    }
}
