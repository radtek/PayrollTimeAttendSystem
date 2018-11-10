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
    public partial class frmRestoreDatabase : Form
    {
        clsISClientUtilities clsISClientUtilities;

        DataSet pvtDataSet;
        DataView pvtDataView;

        private int pvtintProcess = 0;

        ToolStripMenuItem miLinkedMenuItem = null;

        public frmRestoreDatabase()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult myDialogResult = CustomClientMessageBox.Show("Are you sure you want to Restore this Database?",
                             this.Text,
                             MessageBoxButtons.YesNo,
                             MessageBoxIcon.Question);

                if (myDialogResult == System.Windows.Forms.DialogResult.Yes)
                {
                    this.btnOK.Enabled = false;

                    pvtintProcess = 0;
                    grbActivationProcess.Visible = true;
                    this.pnlRestoreDatabase.Visible = false;

                    this.picBackupBefore.Image = (Image)global::InteractPayrollClient.Properties.Resources.Question;
                    this.picRestoreDatabase.Image = null;

                    DateTime dtWaitTime = DateTime.Now.AddSeconds(3);

                    this.tmrTimer.Enabled = true;

                    int intReturnCode = 0;

                    intReturnCode = (int)clsISClientUtilities.DynamicFunction("Backup_DataBase_Before_Restore", null, false);

                    this.tmrTimer.Enabled = false;
                    this.picBackupBefore.Visible = true;

                    if (intReturnCode == 0)
                    {
                        while (dtWaitTime > DateTime.Now)
                        {
                            Application.DoEvents();
                        }

                        dtWaitTime = DateTime.Now.AddSeconds(3);

                        this.picBackupBefore.Image = (Image)global::InteractPayrollClient.Properties.Resources.Ok;

                        pvtintProcess += 1;
                        this.pnlRestoreDatabase.Visible = true;
                        this.picRestoreDatabase.Image = (Image)global::InteractPayrollClient.Properties.Resources.Question;

                        this.tmrTimer.Enabled = true;

                        object[] objParm = new object[1];

                        objParm[0] = dgvFilesDataGridView[1, dgvFilesDataGridView.SelectedRows[0].Index].Value.ToString();

                        intReturnCode = (int)clsISClientUtilities.DynamicFunction("Restore_DataBase", objParm, false);

                        this.tmrTimer.Enabled = false;
                        this.picRestoreDatabase.Visible = true;

                        while (dtWaitTime > DateTime.Now)
                        {
                            Application.DoEvents();
                        }

                        if (intReturnCode == 0)
                        {
                            this.picRestoreDatabase.Image = (Image)global::InteractPayrollClient.Properties.Resources.Ok;
                            this.Refresh();


                            CustomClientMessageBox.Show("Restore Successful.",
                                     this.Text,
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                        }
                        else
                        {
                            this.picRestoreDatabase.Image = (Image)global::InteractPayrollClient.Properties.Resources.Error;
                            this.Refresh();

                            CustomClientMessageBox.Show("Restore Failed.",
                                      this.Text,
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        this.picBackupBefore.Image = (Image)global::InteractPayrollClient.Properties.Resources.Error;

                        CustomClientMessageBox.Show("Backup of Database Before Restore Failed.",
                                     this.Text,
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Error);
                    }

                    Load_Restore_Files();

                    this.grbActivationProcess.Visible = false;
                }
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
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
                | e.Column.Index == 2)
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

        private void Load_Restore_Files()
        {
            string strPayrollRunDate = "";

            byte[] byteCompress = (byte[])clsISClientUtilities.DynamicFunction("Get_Restore_Files", null, false);

            pvtDataSet = null;
            pvtDataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(byteCompress);

            this.dgvFilesDataGridView.Rows.Clear();

            pvtDataView = null;
            pvtDataView = new DataView(pvtDataSet.Tables["RestoreFiles"], "", "RESTORE_DATETIME DESC", DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < pvtDataView.Count; intRow++)
            {
                if (pvtDataView[intRow]["RESTORE_DATETIME"] != System.DBNull.Value)
                {
                    strPayrollRunDate = Convert.ToDateTime(pvtDataView[intRow]["RESTORE_DATETIME"]).ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    strPayrollRunDate = "";
                }

                this.dgvFilesDataGridView.Rows.Add(strPayrollRunDate,
                                                   pvtDataView[intRow]["RESTORE_FILE"].ToString());
            }

            if (pvtDataView.Count > 0)
            {
                this.btnOK.Enabled = true;
                this.btnDelete.Visible = true;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult myDialogResult = CustomClientMessageBox.Show("Are you sure you want to Delete the Backup File?",
                             this.Text,
                             MessageBoxButtons.YesNo,
                             MessageBoxIcon.Question);

                if (myDialogResult == System.Windows.Forms.DialogResult.Yes)
                {
                    string strFileName = this.dgvFilesDataGridView[1, this.Get_DataGridView_SelectedRowIndex(this.dgvFilesDataGridView)].Value.ToString();

                    object[] objParm = new object[1];
                    //#if(DEBUG)
                    //                    MessageBox.Show("Delete NOT Available in DEBUG Mode");
                    //#else
                    objParm[0] = strFileName;

                    clsISClientUtilities.DynamicFunction("Delete_Backup_File", objParm, false);

                    this.Refresh();

                    CustomClientMessageBox.Show("Delete of Backup File Successful.",
                                     this.Text,
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);

                    //#endif
                    this.dgvFilesDataGridView.Rows.RemoveAt(this.Get_DataGridView_SelectedRowIndex(this.dgvFilesDataGridView));

                    if (this.dgvFilesDataGridView.Rows.Count == 0)
                    {
                        this.btnOK.Enabled = false;
                        this.btnDelete.Visible = false;
                    }
                }
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmRestoreDatabase_Load(object sender, EventArgs e)
        {
            try
            {
                miLinkedMenuItem = (ToolStripMenuItem)AppDomain.CurrentDomain.GetData("LinkedMenuItem");

                clsISClientUtilities = new clsISClientUtilities(this, "busBackupRestoreClientDatabase");

                this.lblFilesHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);

                Load_Restore_Files();
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
        }

        private void frmRestoreDatabase_FormClosing(object sender, FormClosingEventArgs e)
        {
            miLinkedMenuItem.Enabled = true;
        }
    }
}
