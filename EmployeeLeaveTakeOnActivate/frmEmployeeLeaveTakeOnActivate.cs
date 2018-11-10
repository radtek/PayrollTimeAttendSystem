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
    public partial class frmEmployeeLeaveTakeOnActivate : Form
    {
        clsISUtilities clsISUtilities;

        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;
        private DataView pvtTempDataView;
        private DataView pvtEmployeeDataView;

        private byte[] pvtbytCompress;
        object[] objParm;
        private string pvtstrPayrollType = "";
       
        private int pvtintProcess = 0;

        private int pvtintPayrollTypeDataGridViewRowIndex = -1;

        int pvtintTimerCount = 0;
        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        
        public frmEmployeeLeaveTakeOnActivate()
        {
            InitializeComponent();
        }

        private void frmEmployeeLeaveTakeOnActivate_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this,"busEmployeeLeaveTakeOnActivate");

                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblChosenEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                object[] objParm = new object[1];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                                
                for (int intRow = 0; intRow < pvtDataSet.Tables["PayrollType"].Rows.Count; intRow++)
                {
                    this.dgvPayrollTypeDataGridView.Rows.Add(pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE_DESC"].ToString());
                }

                pvtblnPayrollTypeDataGridViewLoaded = true;
                                
                if (pvtDataSet.Tables["PayrollType"].Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView, 0);

                    this.tmrTimer.Enabled = true;
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
            else
            {
                if (myDataGridView.SelectionMode == DataGridViewSelectionMode.CellSelect)
                {
                    intReturnIndex = myDataGridView.CurrentCell.RowIndex;
                }
            }

            return intReturnIndex;
        }

        public void Set_DataGridView_SelectedRowIndex(DataGridView myDataGridView, int intRow)
        {
            //Fires DataGridView RowEnter Function
            if (this.Get_DataGridView_SelectedRowIndex(myDataGridView) == intRow)
            {
                DataGridViewCellEventArgs myDataGridViewCellEventArgs = new DataGridViewCellEventArgs(0, intRow);

                switch (myDataGridView.Name)
                {
                    case "dgvPayrollTypeDataGridView":

                        pvtintPayrollTypeDataGridViewRowIndex = -1;
                        this.dgvPayrollTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    default:

                        MessageBox.Show("No Entry for " + myDataGridView.Name + " in Set_DataGridView_SelectedRowIndex", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            else
            {
                myDataGridView.CurrentCell = myDataGridView[0, intRow];
            }
        }

        private void Load_CurrentForm_Records()
        {
            pvtTempDataView = null;
            pvtTempDataView = new DataView(pvtDataSet.Tables["Employee"],
                "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_TYPE = '" + this.dgvPayrollTypeDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0,1) + "'"
                ,
                "",
                DataViewRowState.CurrentRows);

            Set_Form_For_Read();

            Load_Employees();
        }

        private void btnAddAll_Click(object sender, System.EventArgs e)
        {
        btnAddAll_Click_Continue:

            if (this.dgvEmployeesDataGridView.Rows.Count > 0)
            {
                btnAdd_Click(null, null);

                goto btnAddAll_Click_Continue;
            }
        }

        private void btnRemoveAll_Click(object sender, System.EventArgs e)
        {
        btnRemoveAll_Click_Continue:

            if (this.dgvChosenEmployeeDataGridView.Rows.Count > 0)
            {
                btnRemove_Click(null, null);

                goto btnRemoveAll_Click_Continue;
            }
        }

        private void btnAdd_Click(object sender, System.EventArgs e)
        {
            if (this.dgvEmployeesDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvEmployeesDataGridView.Rows[this.dgvEmployeesDataGridView.CurrentRow.Index];

                this.dgvEmployeesDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvChosenEmployeeDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvChosenEmployeeDataGridView.CurrentCell = this.dgvChosenEmployeeDataGridView[0, this.dgvChosenEmployeeDataGridView.Rows.Count - 1];
            }
        }

        private void btnRemove_Click(object sender, System.EventArgs e)
        {
            if (this.dgvChosenEmployeeDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvChosenEmployeeDataGridView.Rows[this.dgvChosenEmployeeDataGridView.CurrentRow.Index];

                this.dgvChosenEmployeeDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvEmployeesDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvEmployeesDataGridView.CurrentCell = this.dgvEmployeesDataGridView[0, this.dgvEmployeesDataGridView.Rows.Count - 1];
            }
        }

        private void Clear_DataGridView(DataGridView myDataGridView)
        {
            myDataGridView.Rows.Clear();

            if (myDataGridView.SortedColumn != null)
            {
                myDataGridView.SortedColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
            }
        }

        private void Load_Employees()
        {
            this.btnUpdate.Enabled = false;

            this.Clear_DataGridView(this.dgvEmployeesDataGridView);
            this.Clear_DataGridView(this.dgvChosenEmployeeDataGridView);
 
            pvtEmployeeDataView = null;
            pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                "",
                DataViewRowState.CurrentRows);

            for (int intIndex = 0; intIndex < pvtEmployeeDataView.Count; intIndex++)
            {
                this.dgvEmployeesDataGridView.Rows.Add(pvtEmployeeDataView[intIndex]["EMPLOYEE_CODE"].ToString(),
                                                       pvtEmployeeDataView[intIndex]["EMPLOYEE_SURNAME"].ToString(),
                                                       pvtEmployeeDataView[intIndex]["EMPLOYEE_NAME"].ToString(),
                                                       pvtEmployeeDataView[intIndex]["EMPLOYEE_NO"].ToString());
            }

            if (pvtEmployeeDataView.Count > 0)
            {
                this.btnUpdate.Enabled = true;
            }
        }

        private void Set_Form_For_Read()
        {
            this.grbActivationProcess.Visible = false;

            this.dgvPayrollTypeDataGridView.Enabled = true;
            this.picPayrollTypeLock.Visible = false;

            this.btnUpdate.Enabled = true;
            
            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.btnAdd.Enabled = false;
            this.btnAddAll.Enabled = false;
            this.btnRemove.Enabled = false;
            this.btnRemoveAll.Enabled = false;
        }

        private void Set_Form_For_Edit()
        {
            this.dgvPayrollTypeDataGridView.Enabled = false;
            this.picPayrollTypeLock.Visible = true;

            this.btnUpdate.Enabled = false;
            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.btnAdd.Enabled = true;
            this.btnAddAll.Enabled = true;
            this.btnRemove.Enabled = true;
            this.btnRemoveAll.Enabled = true;
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        public void btnUpdate_Click(object sender, System.EventArgs e)
        {
            this.Text += " - Update";

            this.Set_Form_For_Edit();
        }

        public void btnCancel_Click(object sender, System.EventArgs e)
        {
            if (this.Text.IndexOf(" - Update", 0) > 0)
            {
                this.Text = this.Text.Substring(0, this.Text.LastIndexOf("-") - 1);
            }

            Set_Form_For_Read();

            Load_Employees();
        }

        public void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                int intReturnCode = 0;

                if (this.dgvChosenEmployeeDataGridView.Rows.Count == 0)
                {
                    CustomMessageBox.Show("No Employee/s Selected\nAction Cancelled.",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);

                    return;
                }

                this.grbLeaveReminder.Visible = false;

                this.picBackupBefore.Image = (Image)global::EmployeeLeaveTakeOnActivate.Properties.Resources.Question;

                this.grbActivationProcess.Visible = true;

                pvtintProcess = 1;
                this.tmrTimer.Enabled = true;
//#if(DEBUG)
//#else
                objParm = new object[3];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = pvtstrPayrollType;
                objParm[2] = "B";

                intReturnCode = (int)clsISUtilities.DynamicFunction("Backup_DataBase", objParm);
//#endif
                this.tmrTimer.Enabled = false;
                this.picBackupBefore.Visible = true;
                this.Refresh();

                string strEmployeeNoin = "";

                if (intReturnCode == 0)
                {
                    this.picBackupBefore.Image = (Image)global::EmployeeLeaveTakeOnActivate.Properties.Resources.Ok;

                    this.picEmployeeActivation.Image = (Image)global::EmployeeLeaveTakeOnActivate.Properties.Resources.Question;
                    this.pnlEmployeeActivation.Visible = true;
                    this.Refresh();

                    pvtintProcess += 1;
                    this.tmrTimer.Enabled = true;

                    for (int intRow = 0; intRow < this.dgvChosenEmployeeDataGridView.Rows.Count; intRow++)
                    {
                        if (intRow == 0)
                        {
                            strEmployeeNoin = this.dgvChosenEmployeeDataGridView[3, intRow].Value.ToString();
                        }
                        else
                        {
                            strEmployeeNoin += "," + this.dgvChosenEmployeeDataGridView[3, intRow].Value.ToString();
                        }
                    }

                    objParm = new object[4];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[2] = pvtstrPayrollType;
                    objParm[3] = strEmployeeNoin;

                    pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Update_Record", objParm);

                    pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                    
                    this.tmrTimer.Enabled = false;
                    this.picEmployeeActivation.Visible = true;
                    this.picEmployeeActivation.Image = (Image)global::EmployeeLeaveTakeOnActivate.Properties.Resources.Ok;

                    this.picBackupAfter.Image = (Image)global::EmployeeLeaveTakeOnActivate.Properties.Resources.Question;
                    this.pnlDatabaseBackupAfter.Visible = true;
                    this.Refresh();

                    pvtintProcess += 1;
                    this.tmrTimer.Enabled = true;
#if(DEBUG)
#else
                    objParm = new object[3];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = pvtstrPayrollType;
                    objParm[2] = "A";

                    intReturnCode = (int)clsISUtilities.DynamicFunction("Backup_DataBase", objParm);

#endif
                    this.tmrTimer.Enabled = false;
                    this.picBackupAfter.Visible = true;
                    this.Refresh();

                    if (intReturnCode == 0)
                    {
                        this.picBackupAfter.Image = (Image)global::EmployeeLeaveTakeOnActivate.Properties.Resources.Ok;

                        CustomMessageBox.Show("Leave Activation Successful.", this.Text,
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        this.picBackupAfter.Image = (Image)global::EmployeeLeaveTakeOnActivate.Properties.Resources.Error;

                        CustomMessageBox.Show("Leave Activation Successful but Backup of Database Failed.\nSpeak to System Administrator",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    this.picBackupBefore.Image = (Image)global::EmployeeLeaveTakeOnActivate.Properties.Resources.Error;

                    CustomMessageBox.Show("Backup of Database Failed.Speak To System Administrator.",
                    this.Text,
                        MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                }

                Set_Form_For_Read();

                this.Clear_DataGridView(this.dgvPayrollTypeDataGridView);
                this.Clear_DataGridView(this.dgvEmployeesDataGridView);
                this.Clear_DataGridView(this.dgvChosenEmployeeDataGridView);

                pvtblnPayrollTypeDataGridViewLoaded = false;

                for (int intRow = 0; intRow < pvtDataSet.Tables["PayrollType"].Rows.Count; intRow++)
                {
                    this.dgvPayrollTypeDataGridView.Rows.Add(pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE_DESC"].ToString());
                }

                pvtblnPayrollTypeDataGridViewLoaded = true;

                if (pvtDataSet.Tables["PayrollType"].Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView, 0);
                }
                else
                {
                    this.btnUpdate.Enabled = false;
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void tmrTimer_Tick(object sender, EventArgs e)
        {
            if (pvtintTimerCount == 2
                & pvtintProcess == 0)
            {
                this.tmrTimer.Enabled = false;
            }
            else
            {
                pvtintTimerCount += 1;

                switch (pvtintProcess)
                {
                    case 0:

                        if (this.picWarningPicture.Visible == true)
                        {
                            this.picWarningPicture.Visible = false;
                        }
                        else
                        {
                            this.picWarningPicture.Visible = true;
                        }

                        break;

                    case 1:

                        if (this.picBackupBefore.Visible == true)
                        {
                            this.picBackupBefore.Visible = false;
                        }
                        else
                        {
                            this.picBackupBefore.Visible = true;
                        }

                        break;

                    case 2:

                        if (this.picEmployeeActivation.Visible == true)
                        {
                            this.picEmployeeActivation.Visible = false;
                        }
                        else
                        {
                            this.picEmployeeActivation.Visible = true;
                        }

                        break;

                    case 3:

                        if (this.picBackupAfter.Visible == true)
                        {
                            this.picBackupAfter.Visible = false;
                        }
                        else
                        {
                            this.picBackupAfter.Visible = true;
                        }

                        break;
                }
            }
        }

        private void dgvPayrollTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPayrollTypeDataGridViewLoaded == true)
            {
                if (pvtintPayrollTypeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayrollTypeDataGridViewRowIndex = e.RowIndex;

                    pvtstrPayrollType = this.dgvPayrollTypeDataGridView[0, e.RowIndex].Value.ToString().Substring(0, 1);

                    Load_CurrentForm_Records();
                }
            }
        }

        private void dgvEmployeesDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.btnAdd_Click(sender, e);
            }
        }

        private void dgvChosenEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.btnRemove_Click(sender, e);
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
    }
}
