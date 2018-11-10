using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InteractPayrollClient
{
    public partial class frmBackupDatabase : Form
    {
        clsISClientUtilities clsISClientUtilities;
        ToolStripMenuItem miLinkedMenuItem;

        public frmBackupDatabase()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult myDialogResult = CustomClientMessageBox.Show("Are you sure you want to Backup the Database?",
                             this.Text,
                             MessageBoxButtons.YesNo,
                             MessageBoxIcon.Question);

                if (myDialogResult == System.Windows.Forms.DialogResult.Yes)
                {
                    int intReturnCode = (int)clsISClientUtilities.DynamicFunction("Backup_DataBase", null, false);

                    if (intReturnCode == 0)
                    {
                        CustomClientMessageBox.Show("Backup Successful.",
                                 this.Text,
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Information);

                    }
                    else
                    {
                        CustomClientMessageBox.Show("Backup Failed.",
                                  this.Text,
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);

                    }
                }
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmBackupDatabase_FormClosing(object sender, FormClosingEventArgs e)
        {
            miLinkedMenuItem.Enabled = true;
        }

        private void frmBackupDatabase_Load(object sender, EventArgs e)
        {
            try
            {
                miLinkedMenuItem = (ToolStripMenuItem)AppDomain.CurrentDomain.GetData("LinkedMenuItem");

                clsISClientUtilities = new clsISClientUtilities(this, "busBackupRestoreClientDatabase");

                this.lblDescription.Text = "Backup Database 'InteractPayrollClient'?";
                this.btnOK.Enabled = true;
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
        }
    }
}
