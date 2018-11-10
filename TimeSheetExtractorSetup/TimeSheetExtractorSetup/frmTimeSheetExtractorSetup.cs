using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TimeSheetExtractorSetup
{
    public partial class frmTimeSheetExtractorSetup : Form
    {
        public frmTimeSheetExtractorSetup()
        {
            InitializeComponent();
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "All files (*.*)|*.*";
                dialog.Title = "Select a File to Upload.";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.txtFilePath.Text = dialog.FileName;

                    pvtstrFileName = dialog.SafeFileName;

                    this.btnUpload.Enabled = true;
                }
                else
                {
                    this.txtFilePath.Text = "";
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }
    }
}
