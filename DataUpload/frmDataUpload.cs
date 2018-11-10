using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InteractPayrollClient;

namespace InteractPayroll
{
    public partial class frmDataUpload : Form
    {
        clsISUtilities clsISUtilities;
        //Local

        clsISClientUtilities clsISClientUtilities;

        private string pvtstrPayCategoriesSelected = "";

        private string pvtstrPayrollType = "";

        bool blnclsISClientUtilities = false;

        DataView pvtPayCategoryDataView;
     
        DataSet pvtDataSet;
        DataSet pvtTempDataSet;
        DataSet pvtDataSetClient;
       
        byte[] pvtbytCompress;

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnPayCategoryDataGridViewLoaded = false;

        private int pvtintPayrollTypeDataGridViewRowIndex = -1;

        DataGridViewCellStyle NoPayrollDateOpenDataGridViewCellStyle;
        DataGridViewCellStyle RunTimeAttendancePendingDataGridViewCellStyle;
        DataGridViewCellStyle RunTimeAttendanceAlreadyRunDataGridViewCellStyle;

        public frmDataUpload()
        {
            InitializeComponent();
        }

        private void frmDataUpload_Load(object sender, System.EventArgs e)
        {
            try
            {
                //Move Group Boxes Over each Other
                this.grbNoDateOpen.Top = this.grbRunTimeAttendance.Top;
                              
                clsISUtilities = new clsISUtilities(this,"busDataUpload");
                //Local
                clsISClientUtilities = new clsISClientUtilities(this, "busClientDataUpload");

                if (AppDomain.CurrentDomain.GetData("AccessInd").ToString() == "U")
                {
                    this.lblResetTimeAttendanceMenu.Visible = false;
                    this.lblOpenPayrollMenu.Visible = false;
                }
                
                //Get Pay Categories on Local Mahine
                pvtDataSetClient = new DataSet();

                object[] objParm = new object[1];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));

                blnclsISClientUtilities = true;
                pvtbytCompress = (byte[])this.clsISClientUtilities.DynamicFunction("Get_New_Company_Records", objParm, false);

                pvtDataSetClient = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                this.lblCostCentreSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayrollTypeSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                
                NoPayrollDateOpenDataGridViewCellStyle = new DataGridViewCellStyle();
                NoPayrollDateOpenDataGridViewCellStyle.BackColor = Color.Red;
                NoPayrollDateOpenDataGridViewCellStyle.SelectionBackColor = Color.Red;

                RunTimeAttendancePendingDataGridViewCellStyle = new DataGridViewCellStyle();
                RunTimeAttendancePendingDataGridViewCellStyle.BackColor = Color.Lime;
                RunTimeAttendancePendingDataGridViewCellStyle.SelectionBackColor = Color.Lime;

                RunTimeAttendanceAlreadyRunDataGridViewCellStyle = new DataGridViewCellStyle();
                RunTimeAttendanceAlreadyRunDataGridViewCellStyle.BackColor = Color.Orange;
                RunTimeAttendanceAlreadyRunDataGridViewCellStyle.SelectionBackColor = Color.Orange;
          
                objParm = new object[2];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();

                blnclsISClientUtilities = false;
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records_New", objParm);
             
                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                string strPayrollType = "";

                if (this.pvtDataSet.Tables["PayrollType"].Rows.Count > 0)
                {
                    for (int intRow = 0; intRow < this.pvtDataSet.Tables["PayrollType"].Rows.Count; intRow++)
                    {
                        this.dgvPayrollTypeDataGridView.Rows.Add("",
                                                                 this.pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE"].ToString());

                        strPayrollType = this.pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE"].ToString().Substring(0, 1);

                        pvtPayCategoryDataView = null;
                        pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"],
                        "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + "AND PAY_CATEGORY_TYPE = '" + strPayrollType + "'",
                        "",
                        DataViewRowState.CurrentRows);

                        if (pvtPayCategoryDataView.Count == 0)
                        {
                            this.dgvPayrollTypeDataGridView[0,intRow].Style = this.NoPayrollDateOpenDataGridViewCellStyle;
                        }
                        else
                        {
                            switch (strPayrollType)
                            {
                                case "W":

                                    if (this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"].ToString() == "N")
                                    {
                                        this.dgvPayrollTypeDataGridView[0,intRow].Style = this.RunTimeAttendancePendingDataGridViewCellStyle;
                                    }
                                    else
                                    {
                                        this.dgvPayrollTypeDataGridView[0,intRow].Style = this.RunTimeAttendanceAlreadyRunDataGridViewCellStyle;
                                    }

                                    break;

                                case "S":

                                    if (this.pvtDataSet.Tables["Company"].Rows[0]["SALARY_RUN_IND"].ToString() == "Y")
                                    {
                                        this.dgvPayrollTypeDataGridView[0,intRow].Style = this.RunTimeAttendancePendingDataGridViewCellStyle;
                                    }
                                    else
                                    {
                                        this.dgvPayrollTypeDataGridView[0,intRow].Style = this.RunTimeAttendanceAlreadyRunDataGridViewCellStyle;
                                    }

                                    break;

                                case "T":

                                    if (this.pvtDataSet.Tables["Company"].Rows[0]["TIME_ATTENDANCE_RUN_IND"].ToString() == "N")
                                    {
                                        this.dgvPayrollTypeDataGridView[0,intRow].Style = this.RunTimeAttendancePendingDataGridViewCellStyle;
                                    }
                                    else
                                    {
                                        this.dgvPayrollTypeDataGridView[0,intRow].Style = this.RunTimeAttendanceAlreadyRunDataGridViewCellStyle;
                                    }

                                    break;
                            }
                        }
                    }

                    pvtblnPayrollTypeDataGridViewLoaded = true;

                    this.Set_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView, 0);
                }
            }
            catch (Exception eException)
            {
                if (blnclsISClientUtilities == true)
                {
                    clsISClientUtilities.ErrorHandler(eException);
                }
                else
                {
                    clsISUtilities.ErrorHandler(eException);
                }
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

                    case "dgvPayCategoryDataGridView":

                        this.dgvPayCategoryDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    default:

                        CustomMessageBox.Show("No Entry for " + myDataGridView.Name + " in Set_DataGridView_SelectedRowIndex", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            else
            {
                myDataGridView.CurrentCell = myDataGridView[0, intRow];
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

        private void Load_CurrentForm_Records()
        {
            try
            {
                int intFindRow = -1;

                this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
                            
                DataView LocalPayCategoryDataView = new DataView(pvtDataSetClient.Tables["PayCategory"],
                    "PAY_CATEGORY_TYPE = '" + this.pvtstrPayrollType + "'",
                    "PAY_CATEGORY_NO",
                    DataViewRowState.CurrentRows);

                bool blnUploadOK = true;

                pvtblnPayCategoryDataGridViewLoaded = false;

                this.btnOK.Enabled = false;
                this.grbNoDateOpen.Visible = false;
                this.grbRunTimeAttendance.Visible = false;

                this.btnOpenPayrollRun.Enabled = true;
                this.lblNoOpenPayCategory.ForeColor = System.Drawing.Color.Black;

                if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
                {
                    this.lblNoOpenPayCategory.Text = "There is currently No Date Open for the " + this.dgvPayrollTypeDataGridView[1, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString() + " Type.";
                }
                else
                {
                    this.lblNoOpenPayCategory.Text = "There is currently No Payroll Date Open for the " + this.dgvPayrollTypeDataGridView[1, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString() + " Type.";
                }

                pvtPayCategoryDataView = null;
                pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"],
                    "COMPANY_NO = " + this.pvtDataSet.Tables["Company"].Rows[0]["COMPANY_NO"].ToString() + "AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                    "",
                    DataViewRowState.CurrentRows);

                string strLastUploadDateTime = "";
                string strLastUploadDateTimeYYMMDDHHMM = "";

                if (pvtPayCategoryDataView.Count > 0)
                {
                    pvtstrPayCategoriesSelected = "(";

                    for (int intRow = 0; intRow < this.pvtPayCategoryDataView.Count; intRow++)
                    {
                        intFindRow = LocalPayCategoryDataView.Find(this.pvtPayCategoryDataView[intRow]["PAY_CATEGORY_NO"].ToString());

                        if (intFindRow == -1)
                        {
                            continue;
                        }

                        if (this.pvtPayCategoryDataView[intRow]["LAST_UPLOAD_DATETIME"] == System.DBNull.Value)
                        {
                            strLastUploadDateTime = "";
                            strLastUploadDateTimeYYMMDDHHMM = "";
                        }
                        else
                        {
                            strLastUploadDateTime = Convert.ToDateTime(this.pvtPayCategoryDataView[intRow]["LAST_UPLOAD_DATETIME"]).ToString("dd MMMM yyyy - HH:mm");
                            strLastUploadDateTimeYYMMDDHHMM = Convert.ToDateTime(this.pvtPayCategoryDataView[intRow]["LAST_UPLOAD_DATETIME"]).ToString("yyyyMMddHHmm");
                        }

                        this.dgvPayCategoryDataGridView.Rows.Add("",
                                                                 this.pvtPayCategoryDataView[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                                 Convert.ToDateTime(this.pvtPayCategoryDataView[intRow]["PAY_PERIOD_DATE"]).ToString("d MMMM yyyy"),
                                                                 Convert.ToDateTime(this.pvtPayCategoryDataView[intRow]["TIMESHEET_UPLOAD_DATETIME"]).ToString("d MMMM yyyy") + " - 23:59:59",
                                                                 strLastUploadDateTime,
                                                                 this.pvtPayCategoryDataView[intRow]["PAY_CATEGORY_NO"].ToString(),

                                                                 Convert.ToDateTime(this.pvtPayCategoryDataView[intRow]["PAY_PERIOD_DATE"]).ToString("yyyyMMdd"),
                                                                 Convert.ToDateTime(this.pvtPayCategoryDataView[intRow]["TIMESHEET_UPLOAD_DATETIME"]).ToString("yyyyMMdd") + "235959",
                                                                 strLastUploadDateTimeYYMMDDHHMM);

                        if (pvtstrPayCategoriesSelected == "(")
                        {
                            pvtstrPayCategoriesSelected += this.pvtPayCategoryDataView[intRow]["PAY_CATEGORY_NO"].ToString();
                        }
                        else
                        {
                            pvtstrPayCategoriesSelected += "," + this.pvtPayCategoryDataView[intRow]["PAY_CATEGORY_NO"].ToString();
                        }

                        if ((pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"].ToString() == "N"
                            & pvtstrPayrollType == "W")
                            | (pvtDataSet.Tables["Company"].Rows[0]["SALARY_RUN_IND"].ToString() == "Y"
                            & pvtstrPayrollType == "S")
                            | (pvtDataSet.Tables["Company"].Rows[0]["TIME_ATTENDANCE_RUN_IND"].ToString() == "N"
                            & pvtstrPayrollType == "T"))
                        {
                            this.dgvPayCategoryDataGridView[0,this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = this.RunTimeAttendancePendingDataGridViewCellStyle;
                        }
                        else
                        {
                            this.dgvPayCategoryDataGridView[0,this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = this.RunTimeAttendanceAlreadyRunDataGridViewCellStyle;

                            blnUploadOK = false;
                        }
                    }

                    pvtstrPayCategoriesSelected += ")";

                    pvtblnPayCategoryDataGridViewLoaded = true;

                    if (dgvPayCategoryDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView, 0);
                    }

                    if (LocalPayCategoryDataView.Count == 0)
                    {
                        this.lblNoOpenPayCategory.Text = "There are No Employees on the Local Machine for the " + this.dgvPayrollTypeDataGridView[1, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString() + " Type.";
                        this.lblNoOpenPayCategory.ForeColor = System.Drawing.Color.Red;

                        //No Use OPen Payroll Date
                        this.btnOpenPayrollRun.Enabled = false;

                        this.grbNoDateOpen.Visible = true;
                    }
                    else
                    {
                        if (blnUploadOK == true)
                        {
                            this.btnOK.Enabled = true;
                        }
                        this.pgbProgressBar.Value = 0;
                    }
                }
                else
                {
                    this.grbNoDateOpen.Visible = true;
                }

                if ((this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"].ToString() == "Y"
                & pvtstrPayrollType == "W")
                | (this.pvtDataSet.Tables["Company"].Rows[0]["TIME_ATTENDANCE_RUN_IND"].ToString() == "Y"
                & pvtstrPayrollType == "T"))
                {
                    this.grbRunTimeAttendance.Visible = true;
                }
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            try
            {
                 DialogResult dlgResult = CustomMessageBox.Show("Are you sure you want to Upload this Data?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                 if (dlgResult == DialogResult.Yes)
                 {
                     string strPayCategoryWhere = "";
                    
                     //DataSetClient = null;
                     //DataSetClient = new DataSet();

                     for (int intRow = 0; intRow < this.dgvPayCategoryDataGridView.Rows.Count; intRow++)
                     {
                         if (intRow == 0)
                         {
                             strPayCategoryWhere = " AND EPC.PAY_CATEGORY_NO IN (" + this.dgvPayCategoryDataGridView[5,intRow].Value.ToString();
                         }
                         else
                         {
                             strPayCategoryWhere += "," + this.dgvPayCategoryDataGridView[5, intRow].Value.ToString();
                         }
                     }

                     strPayCategoryWhere += ")";

                     pvtTempDataSet = null;
                     pvtTempDataSet = new DataSet();

                     DataTable myEmployeeDataTable;
                     myEmployeeDataTable = pvtDataSet.Tables["Employee"].Copy();
                     pvtTempDataSet.Tables.Add(myEmployeeDataTable);

                     DataTable myEmployeePayCategoryDataTable;
                     myEmployeePayCategoryDataTable = pvtDataSet.Tables["EmployeePayCategory"].Copy();
                     pvtTempDataSet.Tables.Add(myEmployeePayCategoryDataTable);

                     //Compress DataSet
                     pvtbytCompress = clsISUtilities.Compress_DataSet(pvtTempDataSet);

                     object[] objParm = new object[5];
                     objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                     objParm[1] = pvtstrPayrollType;
                     objParm[2] = strPayCategoryWhere;
                     objParm[3] = Convert.ToDateTime(this.pvtPayCategoryDataView[0]["TIMESHEET_UPLOAD_DATETIME"]).ToString("yyyy-MM-dd");
                     objParm[4] = pvtbytCompress;

                     blnclsISClientUtilities = true;
                     pvtbytCompress = (byte[])this.clsISClientUtilities.DynamicFunction("Get_Upload_Data", objParm,false);

                     DataSet DataSetClient = clsISClientUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                     if (DataSetClient.Tables["ErrorCount"].Rows.Count > 0)
                     {
                         if (DataSetClient.Tables["ErrorCount"].Rows[0]["INDICATOR"].ToString() == "X")
                         {
                            CustomMessageBox.Show("There are Time Sheets with Errors on the Client Database.\n\nYou Need to Fix these TimeSheets on (Interact Time & Attendance Program) to Proceed.\n\nProcess Cancelled.",
                                 this.Text,
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Error);

                             return;
                         }
                     }

                     if (DataSetClient.Tables["Dates"].Rows.Count > 0)
                     {
                         this.grbUploadProgress.Visible = true;
                         this.pgbProgressBar.Maximum = DataSetClient.Tables["Dates"].Rows.Count + 1;

                         for (int intRow = 0; intRow < DataSetClient.Tables["Dates"].Rows.Count; intRow++)
                         {
                             //From Clint Database
                             objParm = new object[4];
                             objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                             objParm[1] = pvtstrPayrollType;
                             objParm[2] = strPayCategoryWhere;
                             objParm[3] = Convert.ToDateTime(DataSetClient.Tables["Dates"].Rows[intRow]["TIMESHEET_DATE"]).ToString("yyyy-MM-dd");

                             blnclsISClientUtilities = true;
                             pvtbytCompress = (byte[])this.clsISClientUtilities.DynamicFunction("Get_Upload_Data_For_Day", objParm,false);

                             //Internet Database Insert
                             objParm = new object[6];
                             objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                             objParm[1] = Convert.ToDateTime(DataSetClient.Tables["Dates"].Rows[intRow]["TIMESHEET_DATE"]);
                             objParm[2] = pvtbytCompress;
                             objParm[3] = pvtstrPayrollType;
                             objParm[4] = Convert.ToDateTime(this.pvtPayCategoryDataView[0]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd");
                             objParm[5] = Convert.ToDateTime(this.pvtPayCategoryDataView[0]["TIMESHEET_UPLOAD_DATETIME"]).ToString("yyyy-MM-dd");


                             blnclsISClientUtilities = false;
                             string strUploadDateTime = (string)clsISUtilities.DynamicFunction("Insert_TimeSheet_Records", objParm,false);

                             //Update Client PAYCATEGORY Records with Date TimeStamp
                             objParm = new object[3];
                             objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                             objParm[1] = strUploadDateTime;
                             objParm[2] = pvtbytCompress;
                           
                             blnclsISClientUtilities = true;
                             this.clsISClientUtilities.DynamicFunction("Update_PayCategory_Last_Upload_DateTime", objParm, false);

                             this.pgbProgressBar.Value += 1;
                         }

                         this.Refresh();

                          objParm = new object[3];
                          objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                          objParm[1] = pvtstrPayrollType;
                          objParm[2] = strPayCategoryWhere;

                          blnclsISClientUtilities = true;
                          this.clsISClientUtilities.DynamicFunction("Move_Data_To_History_And_Cleanup", objParm, false);

                         this.pgbProgressBar.Value += 1;
                         this.Refresh();

                        CustomMessageBox.Show("Upload Successful.",
                           this.Text,
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Information);
                     }
                     else
                     {
                        CustomMessageBox.Show("There is NO Data to Upload",
                             this.Text,
                             MessageBoxButtons.OK,
                             MessageBoxIcon.Information);
                     }

                    this.grbUploadProgress.Visible = false;
                }
            }
            catch (Exception eException)
            {
                if (blnclsISClientUtilities == true)
                {
                    clsISClientUtilities.ErrorHandler(eException);
                }
                else
                {
                    clsISUtilities.ErrorHandler(eException);
                }
            }
        }

        private void dgvPayrollTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPayrollTypeDataGridViewLoaded == true)
            {
                if (pvtintPayrollTypeDataGridViewRowIndex != e.RowIndex)
                {
                    this.grbNoDateOpen.Visible = false;
                    this.grbRunTimeAttendance.Visible = false;

                    pvtintPayrollTypeDataGridViewRowIndex = e.RowIndex;

                    pvtstrPayrollType = dgvPayrollTypeDataGridView[1, e.RowIndex].Value.ToString().Substring(0, 1);
                    
                    Load_CurrentForm_Records();
                    
                    if (pvtstrPayrollType == "W")
                    {
                        this.dgvPayCategoryDataGridView.Columns[2].HeaderText = "Wage Run Date";

                        lblNoPayrollRunDateOpen.Text = "NO Wage Run Date Open";
                    }
                    else
                    {
                        if (pvtstrPayrollType == "S")
                        {
                            this.dgvPayCategoryDataGridView.Columns[2].HeaderText = "Salary Run Date";

                            lblNoPayrollRunDateOpen.Text = "NO Salary Run Date Open";
                        }
                        else
                        {
                            this.dgvPayCategoryDataGridView.Columns[2].HeaderText = "Time Attendance Run Date";

                            lblNoPayrollRunDateOpen.Text = "NO Time Attendance Run Date Open";
                        }
                    }
                }
            }
        }

        private void dgvPayCategoryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPayCategoryDataGridView.Rows.Count > 0
                & pvtblnPayCategoryDataGridViewLoaded == true)
            {
               
            }
        }

        private void btnOpenPayrollRun_Click(object sender, EventArgs e)
        {
            if (AppDomain.CurrentDomain.GetData("AccessInd").ToString() != "U")
            {
                AppDomain.CurrentDomain.SetData("MenuClick", "openPayrollRunToolStripMenuItem");

                Timer TimerMenuClick = (Timer)AppDomain.CurrentDomain.GetData("TimerMenuClick");

                TimerMenuClick.Enabled = true;

                this.Close();
            }
        }

        private void btnRunTimeAttendance_Click(object sender, EventArgs e)
        {
            if (AppDomain.CurrentDomain.GetData("AccessInd").ToString() != "U")
            {
                AppDomain.CurrentDomain.SetData("MenuClick", "runTimeAndAttendanceToolStripMenuItem");

                Timer TimerMenuClick = (Timer)AppDomain.CurrentDomain.GetData("TimerMenuClick");

                TimerMenuClick.Enabled = true;

                this.Close();
            }
        }

        private void dgvPayCategoryDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 2
            || e.Column.Index == 3
            || e.Column.Index == 4)
            {
                if (dgvPayCategoryDataGridView[e.Column.Index + 5, e.RowIndex1].Value.ToString() == "")
                {
                    e.SortResult = -1;
                }
                else
                {
                    if (dgvPayCategoryDataGridView[e.Column.Index + 5, e.RowIndex2].Value.ToString() == "")
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(dgvPayCategoryDataGridView[e.Column.Index + 5, e.RowIndex1].Value.ToString()) > double.Parse(dgvPayCategoryDataGridView[e.Column.Index + 5, e.RowIndex2].Value.ToString()))
                        {
                            e.SortResult = 1;
                        }
                        else
                        {
                            if (double.Parse(dgvPayCategoryDataGridView[e.Column.Index + 5, e.RowIndex1].Value.ToString()) < double.Parse(dgvPayCategoryDataGridView[e.Column.Index + 5, e.RowIndex2].Value.ToString()))
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
    }
}
