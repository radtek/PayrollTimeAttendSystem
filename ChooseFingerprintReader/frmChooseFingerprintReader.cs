using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace InteractPayroll
{
    public partial class frmChooseFingerprintReader : Form
    {
        ToolStripMenuItem miLinkedMenuItem;

        string pvtstrFileName = "FingerprintReaderChoice.txt";

        string strFingerprintName = "";

        public frmChooseFingerprintReader()
        {
            InitializeComponent();
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

            this.cboReader.Enabled = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Text = this.Text.Substring(0, this.Text.IndexOf("- Update") - 1);

            this.btnUpdate.Enabled = true;

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.cboReader.Enabled = false;
            
            for (int intCount = 0; intCount < this.cboReader.Items.Count; intCount++)
            {
                if (this.cboReader.Items[intCount].ToString() == strFingerprintName)
                {
                    this.cboReader.SelectedIndex = intCount;
                    break;
                }
            }
        }
        
        private void frmChooseFingerprintReader_Load(object sender, EventArgs e)
        {
            bool blnValueFound = false;

            if (AppDomain.CurrentDomain.GetData("LinkedMenuItem") != null)
            {
                miLinkedMenuItem = (ToolStripMenuItem)AppDomain.CurrentDomain.GetData("LinkedMenuItem");
            }

            FileInfo fiFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + pvtstrFileName);

            if (fiFileInfo.Exists == true)
            {
                StreamReader srStreamReader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + pvtstrFileName);

                strFingerprintName = srStreamReader.ReadLine();

                srStreamReader.Close();

                for (int intCount = 0; intCount < this.cboReader.Items.Count; intCount++)
                {
                    if (this.cboReader.Items[intCount].ToString() == strFingerprintName)
                    {
                        blnValueFound = true;

                        this.cboReader.SelectedIndex = intCount;
                        break;
                    }
                }
            }

            if (blnValueFound == false)
            {
                this.cboReader.SelectedIndex = 0;
                strFingerprintName = this.cboReader.Items[0].ToString();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                FileInfo fiFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + pvtstrFileName);

                if (fiFileInfo.Exists == true)
                {
                    File.Delete(fiFileInfo.FullName);
                }

                strFingerprintName = this.cboReader.Items[this.cboReader.SelectedIndex].ToString();

                using (StreamWriter writeRecord = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + pvtstrFileName, true))
                {
                    writeRecord.WriteLine(this.cboReader.Items[this.cboReader.SelectedIndex].ToString());
                }

                btnCancel_Click(sender, e);
            }
            catch
            {

            }
        }

        private void frmChooseFingerprintReader_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (miLinkedMenuItem != null)
            {
                miLinkedMenuItem.Enabled = true;
            }
        }
    }
}
