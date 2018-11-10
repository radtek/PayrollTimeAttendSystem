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
    public partial class frmRestoreDatabase : Form
    {
        clsISUtilities clsISUtilities;

        DataSet pvtDataSet;
        DataView pvtDataView;

        private int pvtintProcess = 0;

        public frmRestoreDatabase()
        {
            InitializeComponent();
        }

        private void frmRestoreDatabase_Load(object sender, EventArgs e)
        {
            try
            {

                if (AppDomain.CurrentDomain.GetData("AccessInd").ToString() == "S")
                {
                    this.btnDelete.Visible = true;
                }

                clsISUtilities = new clsISUtilities(this, "busBackupRestoreDatabase");

                //10 Times Normal Timeout
                int intTimeout = Convert.ToInt32(AppDomain.CurrentDomain.GetData("TimeSheetReadTimeoutSeconds")) * 1000 * 10;

                clsISUtilities.Set_WebService_Timeout_Value(intTimeout);

                this.lblFilesHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                object[] objParm = new object[1];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));

                byte[] byteCompress = (byte[])clsISUtilities.DynamicFunction("Get_Restore_Files", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(byteCompress);

                if (pvtDataSet.Tables["CompanyKey"].Rows.Count > 0)
                {
                    this.chkCopy.Checked = true;
                    this.grbDynamicTimeSheet.Visible = true;
                }

                string strPayrollRunDate = "";
                string strBackupDate = "";

                pvtDataView = new DataView(pvtDataSet.Tables["File"], "", "BACKUP_DATETIME DESC", DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < pvtDataView.Count; intRow++)
                {
                    if (pvtDataView[intRow]["PAYROLL_RUN_DATETIME"] != System.DBNull.Value)
                    {
                        strPayrollRunDate = Convert.ToDateTime(pvtDataView[intRow]["PAYROLL_RUN_DATETIME"]).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        strPayrollRunDate = "";
                    }

                    if (pvtDataView[intRow]["BACKUP_DATETIME"] != System.DBNull.Value)
                    {
                        strBackupDate = Convert.ToDateTime(pvtDataView[intRow]["BACKUP_DATETIME"]).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                    {
                        strBackupDate = "";
                    }

                    this.dgvFilesDataGridView.Rows.Add(strBackupDate,
                                                       pvtDataView[intRow]["PAY_CATEGORY_TYPE"].ToString(),
                                                       strPayrollRunDate, 
                                                       pvtDataView[intRow]["BACKUP_FILE_NAME"].ToString());
                }

                if (pvtDataView.Count > 0)
                {
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
                DialogResult myDialogResult = CustomMessageBox.Show("Are you sure you want to Restore this Database?",
                             this.Text,
                             MessageBoxButtons.YesNo,
                             MessageBoxIcon.Question);

                if (myDialogResult == System.Windows.Forms.DialogResult.Yes)
                {

                    this.btnOK.Enabled = false;

                    pvtintProcess = 0;
                    grbActivationProcess.Visible = true;
                    this.pnlRestoreDatabase.Visible = false;
                    
                    this.picBackupBefore.Image = (Image)global::InteractPayroll.Properties.Resources.Question;
                    this.picRestoreDatabase.Image = null;

                    this.tmrTimer.Enabled = true;

                    int intReturnCode = 0;
                    object[] objParm = new object[1];
#if(DEBUG)
#else
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    intReturnCode = (int)clsISUtilities.DynamicFunction("Backup_DataBase_Before_Restore", objParm);
#endif
                    this.tmrTimer.Enabled = false;
                    this.picBackupBefore.Visible = true;

                    if (intReturnCode == 0)
                    {
                        this.picBackupBefore.Image = (Image)global::InteractPayroll.Properties.Resources.Ok;

                        pvtintProcess += 1;
                        this.pnlRestoreDatabase.Visible = true;
                        this.picRestoreDatabase.Image = (Image)global::InteractPayroll.Properties.Resources.Question;

                        string strCopyTimeSheetsOver = "Y";

                        if (this.chkCopy.Checked == false)
                        {
                            strCopyTimeSheetsOver = "N";
                        }

                        this.tmrTimer.Enabled = true;

                        objParm = new object[4];
                        objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                        objParm[1] = this.dgvFilesDataGridView[3, Get_DataGridView_SelectedRowIndex(this.dgvFilesDataGridView)].Value.ToString();
                        objParm[2] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                        objParm[3] = strCopyTimeSheetsOver;

                        intReturnCode = (int)clsISUtilities.DynamicFunction("Restore_DataBase", objParm);

                        this.tmrTimer.Enabled = false;
                        this.picRestoreDatabase.Visible = true;

                        if (intReturnCode == 0)
                        {
                            this.picRestoreDatabase.Image = (Image)global::InteractPayroll.Properties.Resources.Ok;
                            this.Refresh();


                            CustomMessageBox.Show("Restore Successful.",
                                     this.Text,
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                        }
                        else
                        {
                            this.picRestoreDatabase.Image = (Image)global::InteractPayroll.Properties.Resources.Error;
                            this.Refresh();

                            CustomMessageBox.Show("Restore Failed.",
                                      this.Text,
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        this.picBackupBefore.Image = (Image)global::InteractPayroll.Properties.Resources.Error;

                        CustomMessageBox.Show("Backup of Database Before Restore Failed.",
                                     this.Text,
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Error);
                    }

                    this.grbActivationProcess.Visible = false;
                    this.btnOK.Enabled = true;
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        public int Get_DataGridView_SelectedRowIndex(DataGridView myDataGridView)
        {
            int intReturnIndex = -1;

            if (myDataGridView.SelectedRows.Count > 0)
            {
                if (myDataGridView.SelectedRows[0].Selected == true)
                {
                    intReturnIndex = myDataGridView.SelectedRows[0].Index;
                }
            }

            return intReturnIndex;
        }

        private void tmrTimer_Tick(object sender, EventArgs e)
        {
            switch (pvtintProcess)
            {
                case 0:

                    if (this.picBackupBefore.Visible == true)
                    {
                        this.picBackupBefore.Visible = false;
                    }
                    else
                    {
                        this.picBackupBefore.Visible = true;
                    }

                    break;

                case 1:

                    if (this.picRestoreDatabase.Visible == true)
                    {
                        this.picRestoreDatabase.Visible = false;
                    }
                    else
                    {
                        this.picRestoreDatabase.Visible = true;
                    }

                    break;
            }
        }

        private void DataGridView_Sorted(object sender, EventArgs e)
        {
            DataGridView myDataGridView = (DataGridView)sender;

            if (myDataGridView.Rows.Count > 0)
            {
                if (myDataGridView.SelectedRows.Count > 0)
                {
                    if (myDataGridView.SelectedRows[0].Selected == true)
                    {
                        myDataGridView.FirstDisplayedScrollingRowIndex = myDataGridView.SelectedRows[0].Index;
                    }
                }
            }
        }

        private void dgvFilesDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 0
                |  e.Column.Index == 2)
            {
                if (dgvFilesDataGridView[e.Column.Index, e.RowIndex1].Value.ToString() == "")
                {
                    e.SortResult = -1;
                }
                else
                {
                    if (dgvFilesDataGridView[e.Column.Index, e.RowIndex2].Value.ToString() == "")
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(dgvFilesDataGridView[e.Column.Index, e.RowIndex1].Value.ToString().Replace("-", "").Replace(":", "").Replace(" ", "")) > double.Parse(dgvFilesDataGridView[e.Column.Index, e.RowIndex2].Value.ToString().Replace("-", "").Replace(":", "").Replace(" ", "")))
                        {
                            e.SortResult = 1;
                        }
                        else
                        {
                            if (double.Parse(dgvFilesDataGridView[e.Column.Index, e.RowIndex1].Value.ToString().Replace("-", "").Replace(":", "").Replace(" ", "")) < double.Parse(dgvFilesDataGridView[e.Column.Index, e.RowIndex2].Value.ToString().Replace("-", "").Replace(":", "").Replace(" ", "")))
                            {
                                e.SortResult = -1;
                            }
                            else
                            {
                                e.SortResult = 0;
                            }
                        }
                    }
                }

                e.Handled = true;
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.dgvFilesDataGridView.Rows.Count > 0)
                {
                    DialogResult myDialogResult = CustomMessageBox.Show("Are you sure you want to Delete the Backup File?",
                                 this.Text,
                                 MessageBoxButtons.YesNo,
                                 MessageBoxIcon.Question);

                    if (myDialogResult == System.Windows.Forms.DialogResult.Yes)
                    {
                        string strFileName = this.dgvFilesDataGridView[3, this.Get_DataGridView_SelectedRowIndex(this.dgvFilesDataGridView)].Value.ToString();

                        object[] objParm = new object[1];
#if (DEBUG)
                        MessageBox.Show("Delete NOT Available in DEBUG Mode");
#else
                    objParm[0] = strFileName;

                    clsISUtilities.DynamicFunction("Delete_Backup_File", objParm);

                    this.Refresh();

                    CustomMessageBox.Show("Delete of Backup File Successful.",
                                     this.Text,
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);

#endif
                        this.dgvFilesDataGridView.Rows.RemoveAt(this.Get_DataGridView_SelectedRowIndex(this.dgvFilesDataGridView));

                        for (int intRow = 0; intRow < pvtDataView.Count; intRow++)
                        {
                            if (pvtDataView[intRow]["BACKUP_FILE_NAME"].ToString() == strFileName)
                            {
                                pvtDataView[intRow].Delete();
                                this.pvtDataSet.AcceptChanges();
                                break;
                            }
                        }
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
