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
    public partial class frmEmployeeActivate : Form
    {
        clsISUtilities clsISUtilities;
       
        private byte[] pvtbytCompress;

        private DataSet pvtDataSet;
        private DataView pvtEmployeeDataView;

        private int pvtintProcess = 0;

        private int pvtintPayrollTypeDataGridViewRowIndex = -1;

        int pvtintTimerCount = 0;

        Pen Pen3Pixels;
        Pen Pen1Pixel;

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
              
        public frmEmployeeActivate()
        {
            InitializeComponent();
        }

        private void frmEmployeeActivate_Load(object sender, System.EventArgs e)
        {
            try
            {
                Pen3Pixels = new Pen(Color.Black, 3);
                Pen1Pixel = new Pen(Color.Black, 1);

                clsISUtilities = new clsISUtilities(this,"busEmployeeActivate");

                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblChosenEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                clsISUtilities.Create_Calender_Control_From_TextBox(this.txtDate);

                clsISUtilities.NotDataBound_Date_TextBox(txtDate, "Capture Tax Effective Date.");
                clsISUtilities.NotDataBound_ComboBox(this.cboRunDate, "Select Tax Effective Date.");

                object[] objParm = new object[1];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);
              
                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                if (pvtDataSet.Tables["PayrollType"].Rows.Count > 0)
                {
                    for (int intRow = 0; intRow < pvtDataSet.Tables["PayrollType"].Rows.Count; intRow++)
                    {
                        this.dgvPayrollTypeDataGridView.Rows.Add(pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE"].ToString());
                    }

                    pvtblnPayrollTypeDataGridViewLoaded = true;

                    this.Set_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView, 0);

                    this.btnUpdate.Enabled = true;
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

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void btnActivate_Click(object sender, System.EventArgs e)
        {
            
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

                        if (this.picEmployeeActivation.Visible == true)
                        {
                            this.picEmployeeActivation.Visible = false;
                        }
                        else
                        {
                            this.picEmployeeActivation.Visible = true;
                        }

                        break;

                    case 2:

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

                    if (this.dgvPayrollTypeDataGridView[0, e.RowIndex].Value.ToString().Substring(0, 1) == "S")
                    {
                        if (this.cboRunDate.Items.Count == 0)
                        {
                            this.cboRunDate.Items.Clear();
                            this.cboRunDate.BringToFront();

                            //this.btnDate.Visible = false;
                            this.txtDate.Visible = false;
                            this.cboRunDate.Visible = true;

                            DateTime myPrevDateTime = Convert.ToDateTime(pvtDataSet.Tables["PayrollType"].Rows[e.RowIndex]["LAST_PAY_PERIOD_DATE"]);

                            //Load Date ComboBox
                            DateTime myDateTime;

                            if (myPrevDateTime.Month > 2)
                            {
                                myDateTime = new DateTime(myPrevDateTime.Year, 3, 1);

                            }
                            else
                            {
                                //Take-On
                                myDateTime = new DateTime(myPrevDateTime.Year - 1, 3, 1);
                            }

                            while (true)
                            {
                                if (myDateTime > myPrevDateTime.AddMonths(1))
                                {
                                    break;
                                }

                                this.cboRunDate.Items.Add(myDateTime.ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()));

                                myDateTime = myDateTime.AddMonths(1);
                            }
                        }
                        else
                        {
                            this.cboRunDate.SelectedIndex = -1;
                        }

                        this.txtDate.Visible = false;
                        this.cboRunDate.Visible = true;
                    }
                    else
                    {
                        this.cboRunDate.Visible = false;
                        this.txtDate.Visible = true;
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime dtWageDate;

                if (this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) == "W")
                {
                    if (this.txtDate.Text == "")
                    {
                        CustomMessageBox.Show("Capture Tax Effective Date.",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                        return;
                    }
                    
                    dtWageDate = DateTime.ParseExact(this.txtDate.Text, AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);
                }
                else
                {
                    if (this.cboRunDate.SelectedIndex == -1)
                    {
                        CustomMessageBox.Show("Select Tax Effective Date.",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                        return;
                    }

                    dtWageDate = DateTime.ParseExact(this.cboRunDate.SelectedItem.ToString(), AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);
                }

                if (this.dgvChosenEmployeeDataGridView.Rows.Count == 0)
                {
                    CustomMessageBox.Show("Select Employee/s.",
                       this.Text,
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Error);

                    return;
                }

                DialogResult dlgResult = CustomMessageBox.Show("Are you sure you want to Activate these Employees?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (dlgResult == DialogResult.Yes)
                {
                    this.picBackupBefore.Image = null;
                    this.picEmployeeActivation.Image = (Image)global::EmployeeActivate.Properties.Resources.Question;
                    this.picBackupAfter.Image = null;

                    this.grbActivationProcess.Visible = true;

                    this.tmrTimer.Enabled = true;

                    object[] objParm = null;
                    int intReturnCode = 0;

                    objParm = new object[3];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1);
                    objParm[2] = "B";

                    intReturnCode = (int)clsISUtilities.DynamicFunction("Backup_DataBase", objParm);

                    this.tmrTimer.Enabled = false;
                    this.picBackupBefore.Visible = true;

                    if (intReturnCode == 0)
                    {
                        this.picBackupBefore.Image = (Image)global::EmployeeActivate.Properties.Resources.Ok;

                        this.pnlEmployeeActivation.Visible = true;

                        pvtintProcess += 1;
                        this.tmrTimer.Enabled = true;
                        string strEmployeeNos = "";

                        for (int intRow = 0; intRow < this.dgvChosenEmployeeDataGridView.Rows.Count; intRow++)
                        {
                            if (intRow == 0)
                            {
                                strEmployeeNos = this.dgvChosenEmployeeDataGridView[5, intRow].Value.ToString();
                            }
                            else
                            {
                                strEmployeeNos += "," + this.dgvChosenEmployeeDataGridView[5, intRow].Value.ToString();
                            }
                        }
                                                
                        objParm = new object[5];
                        objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                        objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                        objParm[2] = dtWageDate;
                        objParm[3] = this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1);
                        objParm[4] = strEmployeeNos;

                        clsISUtilities.DynamicFunction("Activate_Employees", objParm);

                        this.pvtDataSet.AcceptChanges();

                        this.tmrTimer.Enabled = false;
                        this.picEmployeeActivation.Visible = true;
                        this.picEmployeeActivation.Image = (Image)global::EmployeeActivate.Properties.Resources.Ok;

                        this.pnlDatabaseBackupAfter.Visible = true;

                        pvtintProcess += 1;
                        this.tmrTimer.Enabled = true;
#if(DEBUG)
#else
                        objParm = new object[3];
                        objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                        objParm[1] = this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1);
                        objParm[2] = "A";

                        intReturnCode = (int)clsISUtilities.DynamicFunction("Backup_DataBase", objParm);

#endif
                        this.tmrTimer.Enabled = false;
                        this.picBackupAfter.Visible = true;

                        if (intReturnCode == 0)
                        {
                            this.picBackupAfter.Image = (Image)global::EmployeeActivate.Properties.Resources.Ok;

                            CustomMessageBox.Show("Activation Successful.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        }
                        else
                        {
                            this.picBackupAfter.Image = (Image)global::EmployeeActivate.Properties.Resources.Error;

                            CustomMessageBox.Show("Activation Successful but Backup of Database Failed.\nSpeak to System Administrator",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                        }
                    }
                    else
                    {
                        this.picBackupBefore.Image = (Image)global::EmployeeActivate.Properties.Resources.Error;

                        CustomMessageBox.Show("Backup of Database Failed.Speak To System Administrator.",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    }
                }

                this.Close();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            this.Text += " - Update";

            if (this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) == "W")
            {
                this.cboRunDate.Visible = false;
                this.txtDate.Visible = true;
            }
            else
            {
                this.txtDate.Visible = false;
                this.cboRunDate.Visible = true;

                this.cboRunDate.Enabled = true;
            }

            clsISUtilities.Set_Form_For_Edit(false);

            this.picPayrollTypeLock.Visible = true;
            this.dgvPayrollTypeDataGridView.Enabled = false;

            this.btnUpdate.Enabled = false;
            
            this.btnCancel.Enabled = true;
            this.btnSave.Enabled = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Text = this.Text.Substring(0, this.Text.IndexOf("- Update") - 1);

            grbActivationProcess.Visible = false;

            this.btnUpdate.Enabled = true;

            this.btnCancel.Enabled = false;
            this.btnSave.Enabled = false;

            clsISUtilities.Set_Form_For_Read();

            this.picPayrollTypeLock.Visible = false;
            this.dgvPayrollTypeDataGridView.Enabled = true;

            this.Clear_DataGridView(this.dgvEmployeeDataGridView);
            this.Clear_DataGridView(this.dgvChosenEmployeeDataGridView);

            this.btnAdd.Enabled = false;
            this.btnAddAll.Enabled = false;
            this.btnRemove.Enabled = false;
            this.btnRemoveAll.Enabled = false;
        }

        private void cboRunDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.Clear_DataGridView(this.dgvEmployeeDataGridView);
                this.Clear_DataGridView(this.dgvChosenEmployeeDataGridView);

                DateTime myDateTime = DateTime.ParseExact(this.cboRunDate.SelectedItem.ToString(),AppDomain.CurrentDomain.GetData("DateFormat").ToString(),null);

                pvtEmployeeDataView = null;
                pvtEmployeeDataView = new DataView(this.pvtDataSet.Tables["Employee"],
                                                   "PAY_CATEGORY_TYPE = '" + this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) + "' AND EMPLOYEE_TAX_STARTDATE > '" + myDateTime.ToString("yyyy-MM-dd") + "'",
                                                   "",
                                                   DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
                {
                    this.dgvEmployeeDataGridView.Rows.Add(pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                          pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                          pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                          Convert.ToDateTime(pvtEmployeeDataView[intRow]["EMPLOYEE_TAX_STARTDATE"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()),
                                                          Convert.ToDateTime(pvtEmployeeDataView[intRow]["EMPLOYEE_TAX_STARTDATE"]).ToString("yyyyMMdd"),
                                                          pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString());
                }

                if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                {
                    this.btnAdd.Enabled = true;
                    this.btnAddAll.Enabled = true;
                    this.btnRemove.Enabled = true;
                    this.btnRemoveAll.Enabled = true;
                }
                else
                {
                    this.btnAdd.Enabled = false;
                    this.btnAddAll.Enabled = false;
                    this.btnRemove.Enabled = false;
                    this.btnRemoveAll.Enabled = false;

                    CustomMessageBox.Show("No Records found for Selected Tax Effective Date.",
                    this.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                }
            }
        }

        private void btnAdd_Click(object sender, System.EventArgs e)
        {
            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvEmployeeDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView)];

                this.dgvEmployeeDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvChosenEmployeeDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvChosenEmployeeDataGridView.CurrentCell = this.dgvChosenEmployeeDataGridView[0, this.dgvChosenEmployeeDataGridView.Rows.Count - 1];
            }
        }

        private void btnRemove_Click(object sender, System.EventArgs e)
        {
            if (this.dgvChosenEmployeeDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvChosenEmployeeDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvChosenEmployeeDataGridView)];

                this.dgvChosenEmployeeDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvEmployeeDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvEmployeeDataGridView.CurrentCell = this.dgvEmployeeDataGridView[0, this.dgvEmployeeDataGridView.Rows.Count - 1];
            }
        }

        private void btnAddAll_Click(object sender, System.EventArgs e)
        {
        btnAddAll_Click_Continue:

            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
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

        private void Clear_DataGridView(DataGridView myDataGridView)
        {
            myDataGridView.Rows.Clear();

            if (myDataGridView.SortedColumn != null)
            {
                myDataGridView.SortedColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
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

        private void dgvEmployeeDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 3)
            {
                if (dgvEmployeeDataGridView[4, e.RowIndex1].Value.ToString() == "")
                {
                    e.SortResult = -1;
                }
                else
                {
                    if (dgvEmployeeDataGridView[4, e.RowIndex2].Value.ToString() == "")
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(dgvEmployeeDataGridView[4, e.RowIndex1].Value.ToString()) > double.Parse(dgvEmployeeDataGridView[4, e.RowIndex2].Value.ToString()))
                        {
                            e.SortResult = 1;
                        }
                        else
                        {
                            if (double.Parse(dgvEmployeeDataGridView[4, e.RowIndex1].Value.ToString()) < double.Parse(dgvEmployeeDataGridView[4, e.RowIndex2].Value.ToString()))
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

        private void dgvChosenEmployeeDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 3)
            {
                if (dgvChosenEmployeeDataGridView[4, e.RowIndex1].Value.ToString() == "")
                {
                    e.SortResult = -1;
                }
                else
                {
                    if (dgvChosenEmployeeDataGridView[4, e.RowIndex2].Value.ToString() == "")
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(dgvChosenEmployeeDataGridView[4, e.RowIndex1].Value.ToString()) > double.Parse(dgvChosenEmployeeDataGridView[4, e.RowIndex2].Value.ToString()))
                        {
                            e.SortResult = 1;
                        }
                        else
                        {
                            if (double.Parse(dgvChosenEmployeeDataGridView[4, e.RowIndex1].Value.ToString()) < double.Parse(dgvChosenEmployeeDataGridView[4, e.RowIndex2].Value.ToString()))
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

        private void dgvEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                btnAdd_Click(sender, e);
            }
        }

        private void dgvChosenEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                btnRemove_Click(sender, e);
            }
        }

        private void txtDate_TextChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.Clear_DataGridView(this.dgvEmployeeDataGridView);
                this.Clear_DataGridView(this.dgvChosenEmployeeDataGridView);

                if (this.txtDate.Text != "")
                {
                    DateTime myDateTime = DateTime.ParseExact(this.txtDate.Text,AppDomain.CurrentDomain.GetData("DateFormat").ToString(),null);

                    pvtEmployeeDataView = null;
                    pvtEmployeeDataView = new DataView(this.pvtDataSet.Tables["Employee"],
                                                       "PAY_CATEGORY_TYPE = '" + this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) + "' AND EMPLOYEE_TAX_STARTDATE > '" + myDateTime.ToString("yyyy-MM-dd") + "'",
                                                       "",
                                                       DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
                    {
                        this.dgvEmployeeDataGridView.Rows.Add(pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                              pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                              pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                              Convert.ToDateTime(pvtEmployeeDataView[intRow]["EMPLOYEE_TAX_STARTDATE"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()),
                                                              Convert.ToDateTime(pvtEmployeeDataView[intRow]["EMPLOYEE_TAX_STARTDATE"]).ToString("yyyyMMdd"),
                                                              pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString());
                    }
                }

                if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                {
                    this.btnAdd.Enabled = true;
                    this.btnAddAll.Enabled = true;
                    this.btnRemove.Enabled = true;
                    this.btnRemoveAll.Enabled = true;
                }
                else
                {
                    this.btnAdd.Enabled = false;
                    this.btnAddAll.Enabled = false;
                    this.btnRemove.Enabled = false;
                    this.btnRemoveAll.Enabled = false;

                    CustomMessageBox.Show("No Records found for Selected Tax Effective Date.",
                    this.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                }
            }
        }

        private void grbSchema_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(Pen3Pixels, 49, 60, 301, 60);

            e.Graphics.DrawLine(Pen1Pixel, 305, 59, 305, 61);
            e.Graphics.DrawLine(Pen1Pixel, 304, 58, 304, 62);
            e.Graphics.DrawLine(Pen1Pixel, 303, 57, 303, 63);
            e.Graphics.DrawLine(Pen1Pixel, 302, 56, 302, 64);
            e.Graphics.DrawLine(Pen1Pixel, 301, 55, 301, 65);



            //First Vertical Line
            e.Graphics.DrawLine(Pen1Pixel, 49, 30, 51, 30);
            e.Graphics.DrawLine(Pen1Pixel, 48, 31, 52, 31);
            e.Graphics.DrawLine(Pen1Pixel, 47, 32, 53, 32);
            e.Graphics.DrawLine(Pen1Pixel, 46, 33, 54, 33);
            e.Graphics.DrawLine(Pen1Pixel, 45, 34, 55, 34);

            e.Graphics.DrawLine(Pen3Pixels, 50, 34, 50, 62);

            //Second Vertical Line
            e.Graphics.DrawLine(Pen1Pixel, 259, 30, 261, 30);
            e.Graphics.DrawLine(Pen1Pixel, 258, 31, 262, 31);
            e.Graphics.DrawLine(Pen1Pixel, 257, 32, 263, 32);
            e.Graphics.DrawLine(Pen1Pixel, 256, 33, 264, 33);
            e.Graphics.DrawLine(Pen1Pixel, 255, 34, 265, 34);

            e.Graphics.DrawLine(Pen3Pixels, 260, 34, 260, 62);
        }
    }
}
