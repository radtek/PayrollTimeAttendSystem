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
    public partial class frmFingerPrintReader : Form
    {
        ToolStripMenuItem miLinkedMenuItem;
        clsISClientUtilities clsISClientUtilities;

        FileInfo fiFileInfo;

        string pvtstrFilePath = "";

        public frmFingerPrintReader()
        {
            InitializeComponent();
        }

        private void frmFingerPrintReader_Load(object sender, EventArgs e)
        {
            clsISClientUtilities = new clsISClientUtilities(this, "busRemotBackupSiteName");

            miLinkedMenuItem = (ToolStripMenuItem)AppDomain.CurrentDomain.GetData("LinkedMenuItem");

            object[] objParm = new object[0];
            
            byte[] bytCompress = (byte[])clsISClientUtilities.DynamicFunction("Get_Form_Records", objParm, false);
            DataSet DataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(bytCompress);

            if (DataSet.Tables["RemoteBackupSiteName"].Rows.Count > 0)
            {
                this.txtRemoteBackupSiteName.Text = DataSet.Tables["RemoteBackupSiteName"].Rows[0]["SITE_NAME"].ToString();
            }

            pvtstrFilePath = AppDomain.CurrentDomain.BaseDirectory + "FingerPrintReaderConfig.txt";

            Read_Reader_From_File();
        }

        private void Read_Reader_From_File()
        {
            //2013-02-18
            fiFileInfo = new FileInfo(pvtstrFilePath);

            if (fiFileInfo.Exists == true)
            {
                StreamReader srStreamReader = File.OpenText(pvtstrFilePath);

                string strReaderName = srStreamReader.ReadLine().Trim();

                srStreamReader.Close();
                               
                for (int intRow = 0; intRow < this.cboFingerPrintReader.Items.Count; intRow++)
                {
                    if (this.cboFingerPrintReader.Items[intRow].ToString().Trim() == strReaderName)
                    {
                        this.cboFingerPrintReader.SelectedIndex = intRow;
                        break;
                    }
                }
            }
        }

        private void frmFingerPrintReader_FormClosing(object sender, FormClosingEventArgs e)
        {
            miLinkedMenuItem.Enabled = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            this.Text += " - Update";

            this.btnUpdate.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.cboFingerPrintReader.Enabled = true;
            this.txtRemoteBackupSiteName.Enabled = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Text = this.Text.Substring(0, this.Text.IndexOf(" - Update"));

            this.btnUpdate.Enabled = true;

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.cboFingerPrintReader.Enabled = false;
            this.txtRemoteBackupSiteName.Enabled = false;

            Read_Reader_From_File();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.cboFingerPrintReader.SelectedIndex == -1)
            {
                MessageBox.Show("Select a Fingerprint Reader.",
                    this.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
            else
            {
                object[] objParm = new object[1];
                objParm[0] = this.txtRemoteBackupSiteName.Text.Trim();

                byte[] bytCompress = (byte[])clsISClientUtilities.DynamicFunction("Update_Record", objParm, false);

                if (fiFileInfo.Exists == true)
                {
                    File.Delete(pvtstrFilePath);
                }

                StreamWriter swStreamWriter = fiFileInfo.AppendText();

                swStreamWriter.WriteLine(this.cboFingerPrintReader.SelectedItem.ToString());

                swStreamWriter.Close();

                AppDomain.CurrentDomain.SetData("FingerPrintReader", this.cboFingerPrintReader.SelectedItem.ToString());

                btnCancel_Click(sender, e);
            }
        }

        private void txtRemoteBackupSiteName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 32)
            {
                e.Handled = true;
            }
        }
    }
}
