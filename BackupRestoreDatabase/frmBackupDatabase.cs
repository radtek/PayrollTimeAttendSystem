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
    public partial class frmBackupDatabase : Form
    {
        clsISUtilities clsISUtilities;

        public frmBackupDatabase()
        {
            InitializeComponent();
        }

        private void frmBackupDatabase_Load(object sender, EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this,"busBackupRestoreDatabase");

                //10 Times Normal Timeout
                int intTimeout = Convert.ToInt32(AppDomain.CurrentDomain.GetData("TimeSheetReadTimeoutSeconds")) * 1000 * 10;

                clsISUtilities.Set_WebService_Timeout_Value(intTimeout);

                string strDatabaseName = "";

                DataSet DataSet = (DataSet)AppDomain.CurrentDomain.GetData("DataSet");

                for (int intRow = 0; intRow < DataSet.Tables["Company"].Rows.Count; intRow++)
                {
                    if (Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) == Convert.ToInt64(DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"]))
                    {
                        strDatabaseName = DataSet.Tables["Company"].Rows[intRow]["COMPANY_DESC"].ToString();

                        break;
                    }
                }

                if (strDatabaseName != "")
                {
                    this.lblDescription.Text = "Backup Database '" + strDatabaseName + "'?";
                    this.btnOK.Enabled = true;
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult myDialogResult = CustomMessageBox.Show("Are you sure you want to Backup the Company '" + AppDomain.CurrentDomain.GetData("CompanyDesc").ToString() + "' Database?",
                             this.Text,
                             MessageBoxButtons.YesNo,
                             MessageBoxIcon.Question);

                if (myDialogResult == System.Windows.Forms.DialogResult.Yes)
                {
                    object[] objParm = new object[1];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));

                    int intReturnCode = (int)clsISUtilities.DynamicFunction("Backup_DataBase", objParm);

                    if (intReturnCode == 0)
                    {
                        CustomMessageBox.Show("Backup Successful.",
                                 this.Text,
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Information);

                    }
                    else
                    {
                        CustomMessageBox.Show("Backup Failed.",
                                  this.Text,
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);

                    }
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }
    }
}
