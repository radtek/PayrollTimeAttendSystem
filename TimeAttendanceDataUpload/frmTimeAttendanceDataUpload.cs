using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InteractPayroll;

namespace InteractPayrollClient
{
    public partial class frmTimeAttendanceDataUpload : Form
    {
        clsISUtilities clsISUtilities;
        //Local
        clsISClientUtilities clsISClientUtilities;
        
        ToolStripMenuItem miLinkedMenuItem;
        
        DataView pvtPayCategoryClientDataView;
        DataView pvtPayCategoryDataView;
        DataView pvtCompanyDataView;

        DataSet pvtDataSet;
        DataSet pvtTempDataSet;
        DataSet pvtDataSetClient;

        byte[] pvtbytCompress;

        private Int64 pvtint64CompanyNo = -1;

        private int pvtintCompanyDataGridViewRowIndex = -1;
        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
        
        private bool pvtblnLocalWebService = false;

        private string pvtstrPayrollType = "";

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnPayCategoryDataGridViewLoaded = false;
        private bool pvtblnCompanyDataGridViewLoaded = false;

        DataGridViewCellStyle NoPayrollDateOpenDataGridViewCellStyle;
        DataGridViewCellStyle RunTimeAttendancePendingDataGridViewCellStyle;
        DataGridViewCellStyle RunTimeAttendanceAlreadyRunDataGridViewCellStyle;

        public frmTimeAttendanceDataUpload()
        {
            InitializeComponent();
        }

        private void frmTimeAttendanceDataUpload_Load(object sender, System.EventArgs e)
        {
            try
            {
                //Move Group Boxes Over each Other
                miLinkedMenuItem = (ToolStripMenuItem)AppDomain.CurrentDomain.GetData("LinkedMenuItem");

                clsISUtilities = new clsISUtilities(this, "busDataUpload");
                //Local
                clsISClientUtilities = new clsISClientUtilities(this, "busClientDataUpload");

                object[] objParm = new object[2];
                objParm[0] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                pvtbytCompress = (byte[])this.clsISClientUtilities.DynamicFunction("Get_New_Company_Records_For_User", objParm, false);

                pvtDataSetClient = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                
                this.lblCompany.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
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
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[1] = "B";

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records_For_User_New", objParm);
                
                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                
                for (int intRow = 0; intRow < this.pvtDataSet.Tables["Company"].Rows.Count; intRow++)
                {
                    DataView PayCategoryClientDataView = new DataView(pvtDataSetClient.Tables["PayCategory"],
                    "COMPANY_NO = " + this.pvtDataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"].ToString(),
                     "",
                    DataViewRowState.CurrentRows);

                    if (PayCategoryClientDataView.Count > 0)
                    {
                        this.dgvCompanyDataGridView.Rows.Add(this.pvtDataSet.Tables["Company"].Rows[intRow]["COMPANY_DESC"].ToString(),
                                                             this.pvtDataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"].ToString());
                    }
                }
                
                pvtblnCompanyDataGridViewLoaded = true;

                if (this.dgvCompanyDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvCompanyDataGridView, 0);
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
                    case "dgvCompanyDataGridView":

                        pvtintCompanyDataGridViewRowIndex = -1;
                        this.dgvCompanyDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPayrollTypeDataGridView":

                        pvtintPayrollTypeDataGridViewRowIndex = -1;
                        this.dgvPayrollTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPayCategoryDataGridView":

                        this.dgvPayCategoryDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    default:

                        System.Windows.Forms.MessageBox.Show("No Entry for " + myDataGridView.Name + " in Set_DataGridView_SelectedRowIndex", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                
        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            try
            {
                DialogResult dlgResult = InteractPayroll.CustomMessageBox.Show("Are you sure you want to Upload this Data?",
                   this.Text,
                   MessageBoxButtons.YesNo,
                   MessageBoxIcon.Question);

                if (dlgResult == DialogResult.Yes)
                {
                    string strPayCategoryWhere = "";

                    int pvtPayCategoryDataViewIndex = -1;

                    for (int intRow = 0; intRow < this.dgvPayCategoryDataGridView.Rows.Count; intRow++)
                    {
                        if (intRow == 0)
                        {
                            strPayCategoryWhere = " AND EPC.PAY_CATEGORY_NO IN (" + this.dgvPayCategoryDataGridView[4, intRow].Value.ToString();

                            pvtPayCategoryDataViewIndex = Convert.ToInt32(this.dgvPayCategoryDataGridView[8, intRow].Value);
                        }
                        else
                        {
                            strPayCategoryWhere += "," + this.dgvPayCategoryDataGridView[4, intRow].Value.ToString();
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
                    objParm[0] = pvtint64CompanyNo;
                    objParm[1] = pvtstrPayrollType;
                    objParm[2] = strPayCategoryWhere;
                    objParm[3] = Convert.ToDateTime(this.pvtPayCategoryDataView[pvtPayCategoryDataViewIndex]["TIMESHEET_UPLOAD_DATETIME"]).ToString("yyyy-MM-dd");
                    objParm[4] = pvtbytCompress;

                    pvtblnLocalWebService = true;
                    pvtbytCompress = (byte[])this.clsISClientUtilities.DynamicFunction("Get_Upload_Data", objParm, false);

                    DataSet DataSetClient = clsISClientUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                    if (DataSetClient.Tables["ErrorCount"].Rows.Count > 0)
                    {
                        if (DataSetClient.Tables["ErrorCount"].Rows[0]["INDICATOR"].ToString() == "X")
                        {
                            //Test
                            InteractPayroll.CustomMessageBox.Show("There are Time Sheets with Errors on the Client Database.\n\nYou Need to Fix these TimeSheets on (Validite Client Program) to Proceed.\n\nProcess Cancelled.",
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
                            objParm[0] = pvtint64CompanyNo;
                            objParm[1] = pvtstrPayrollType;
                            objParm[2] = strPayCategoryWhere;
                            objParm[3] = Convert.ToDateTime(DataSetClient.Tables["Dates"].Rows[intRow]["TIMESHEET_DATE"]).ToString("yyyy-MM-dd");

                            pvtblnLocalWebService = true;
                            pvtbytCompress = (byte[])this.clsISClientUtilities.DynamicFunction("Get_Upload_Data_For_Day", objParm, false);

                            //Internet Database Insert
                            objParm = new object[6];
                            objParm[0] = pvtint64CompanyNo;
                            objParm[1] = Convert.ToDateTime(DataSetClient.Tables["Dates"].Rows[intRow]["TIMESHEET_DATE"]);
                            objParm[2] = pvtbytCompress;
                            objParm[3] = pvtstrPayrollType;
                            objParm[4] = Convert.ToDateTime(this.pvtPayCategoryDataView[pvtPayCategoryDataViewIndex]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd");
                            objParm[5] = Convert.ToDateTime(this.pvtPayCategoryDataView[pvtPayCategoryDataViewIndex]["TIMESHEET_UPLOAD_DATETIME"]).ToString("yyyy-MM-dd");

                            pvtblnLocalWebService = false;
                            string strUploadDateTime = (string)clsISUtilities.DynamicFunction("Insert_TimeSheet_Records", objParm);

                            //Update Client PAYCATEGORY Records with Date TimeStamp
                            objParm = new object[3];
                            objParm[0] = pvtint64CompanyNo;
                            objParm[1] = strUploadDateTime;
                            objParm[2] = pvtbytCompress;

                            pvtblnLocalWebService = true;
                            this.clsISClientUtilities.DynamicFunction("Update_PayCategory_Last_Upload_DateTime", objParm, false);

                            this.pgbProgressBar.Value += 1;
                        }

                        this.Refresh();

                        objParm = new object[3];
                        objParm[0] = pvtint64CompanyNo;
                        objParm[1] = pvtstrPayrollType;
                        objParm[2] = strPayCategoryWhere;

                        this.clsISClientUtilities.DynamicFunction("Move_Data_To_History_And_Cleanup", objParm, false);

                        this.pgbProgressBar.Value += 1;
                        this.Refresh();

                        InteractPayroll.CustomMessageBox.Show("Upload Successful.",
                          this.Text,
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Information);
                    }
                    else
                    {
                        InteractPayroll.CustomMessageBox.Show("There is NO Data to Upload",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }

                    this.Close();
                }
            }
            catch (Exception eException)
            {
                if (pvtblnLocalWebService == true)
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
                    int intFindRow = -1;

                    pvtintPayrollTypeDataGridViewRowIndex = e.RowIndex;

                    pvtstrPayrollType = dgvPayrollTypeDataGridView[0, e.RowIndex].Value.ToString().Substring(0, 1);

                    if (pvtstrPayrollType == "W")
                    {
                        this.dgvPayCategoryDataGridView.Columns[1].HeaderText = "Wage Run Date";

                        this.lblNoDateOpen.Text = "NO Wage Run Date Open";
                    }
                    else
                    {
                        if (pvtstrPayrollType == "S")
                        {
                            this.dgvPayCategoryDataGridView.Columns[1].HeaderText = "Salary Run Date";

                            this.lblNoDateOpen.Text = "NO Salary Run Date Open";
                        }
                        else
                        {
                            this.dgvPayCategoryDataGridView.Columns[1].HeaderText = "Time Attendance Run Date";

                            this.lblNoDateOpen.Text = "NO Time Attendance Run Date Open";
                        }
                    }
                    
                    this.btnOK.Enabled = false;
                    
                    this.Clear_DataGridView(this.dgvPayCategoryDataGridView);

                    pvtPayCategoryClientDataView = null;
                    pvtPayCategoryClientDataView = new DataView(pvtDataSetClient.Tables["PayCategory"],
                        "COMPANY_NO = " + pvtint64CompanyNo.ToString() + "AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                        "PAY_CATEGORY_NO",
                       DataViewRowState.CurrentRows);
                    
                    pvtPayCategoryDataView = null;
                    pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"],
                        "COMPANY_NO = " + pvtint64CompanyNo.ToString() + "AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                        "",
                        DataViewRowState.CurrentRows);

                    string strLastUploadDateTime = "";
                    string strLastUploadDateTimeYYYYMMDDHHMM = "";
                    string strPayPeriodDate = "";
                    string strPayPeriodDateYYMMDD = "";
                    string strTimesheetUploadDateTime = "";
                    string strTimesheetUploadDateTimeYYYYMMDDHHMM = "";

                    if (pvtPayCategoryDataView.Count > 0)
                    {
                        for (int intRow = 0; intRow < this.pvtPayCategoryDataView.Count; intRow++)
                        {
                            intFindRow = pvtPayCategoryClientDataView.Find(this.pvtPayCategoryDataView[intRow]["PAY_CATEGORY_NO"].ToString());

                            if (intFindRow == -1)
                            {
                                continue;
                            }

                            if (this.pvtPayCategoryDataView[intRow]["LAST_UPLOAD_DATETIME"] == System.DBNull.Value)
                            {
                                strLastUploadDateTime = "";
                                strLastUploadDateTimeYYYYMMDDHHMM = "";
                            }
                            else
                            {
                                strLastUploadDateTime = Convert.ToDateTime(this.pvtPayCategoryDataView[intRow]["LAST_UPLOAD_DATETIME"]).ToString("dd MMMM yyyy - HH:mm");
                                strLastUploadDateTimeYYYYMMDDHHMM = Convert.ToDateTime(this.pvtPayCategoryDataView[intRow]["LAST_UPLOAD_DATETIME"]).ToString("yyyyMMddHHmm");
                            }

                            if (this.pvtPayCategoryDataView[intRow]["PAY_PERIOD_DATE"] == System.DBNull.Value)
                            {
                                strPayPeriodDate = "";
                                strPayPeriodDateYYMMDD = "";
                            }
                            else
                            {
                                strPayPeriodDate = Convert.ToDateTime(this.pvtPayCategoryDataView[intRow]["PAY_PERIOD_DATE"]).ToString("dd MMMM yyyy");
                                strPayPeriodDateYYMMDD = Convert.ToDateTime(this.pvtPayCategoryDataView[intRow]["PAY_PERIOD_DATE"]).ToString("yyyyMMdd");
                            }

                            if (this.pvtPayCategoryDataView[intRow]["TIMESHEET_UPLOAD_DATETIME"] == System.DBNull.Value)
                            {
                                strTimesheetUploadDateTime = "";
                                strTimesheetUploadDateTimeYYYYMMDDHHMM = "";
                            }
                            else
                            {
                                strTimesheetUploadDateTime = Convert.ToDateTime(this.pvtPayCategoryDataView[intRow]["TIMESHEET_UPLOAD_DATETIME"]).ToString("dd MMMM yyyy - 23:59:59");
                                strTimesheetUploadDateTimeYYYYMMDDHHMM = Convert.ToDateTime(this.pvtPayCategoryDataView[intRow]["TIMESHEET_UPLOAD_DATETIME"]).ToString("yyyyMMdd");
                            }

                            this.dgvPayCategoryDataGridView.Rows.Add(this.pvtPayCategoryDataView[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                                     strPayPeriodDate,
                                                                     strTimesheetUploadDateTime,
                                                                     strLastUploadDateTime,
                                                                     this.pvtPayCategoryDataView[intRow]["PAY_CATEGORY_NO"].ToString(),
                                                                     strPayPeriodDateYYMMDD,
                                                                     strTimesheetUploadDateTimeYYYYMMDDHHMM,
                                                                     strLastUploadDateTimeYYYYMMDDHHMM,
                                                                     intRow.ToString());
                            
                            if ((pvtCompanyDataView[0]["WAGE_RUN_IND"].ToString() == "N"
                            && pvtstrPayrollType == "W")
                            || (pvtCompanyDataView[0]["SALARY_RUN_IND"].ToString() == "Y"
                            && pvtstrPayrollType == "S")
                            || (pvtCompanyDataView[0]["TIME_ATTENDANCE_RUN_IND"].ToString() == "N"
                            && pvtstrPayrollType == "T"))
                            {
                                this.dgvPayCategoryDataGridView.Rows[this.dgvPayCategoryDataGridView.Rows.Count - 1].HeaderCell.Style = this.RunTimeAttendancePendingDataGridViewCellStyle;
                                this.btnOK.Enabled = true;
                            }
                            else
                            {
                                if ((pvtstrPayrollType == "W"
                                && pvtCompanyDataView[0]["WAGE_RUN_IND"].ToString() == "")
                                || (pvtstrPayrollType == "T"
                                && pvtCompanyDataView[0]["TIME_ATTENDANCE_RUN_IND"].ToString() == "")
                                 || (pvtstrPayrollType == "S"
                                && pvtCompanyDataView[0]["SALARY_RUN_IND"].ToString() == ""))
                                {
                                    this.dgvPayCategoryDataGridView.Rows[this.dgvPayCategoryDataGridView.Rows.Count - 1].HeaderCell.Style = this.NoPayrollDateOpenDataGridViewCellStyle;
                                }
                                else
                                {
                                    this.dgvPayCategoryDataGridView.Rows[this.dgvPayCategoryDataGridView.Rows.Count - 1].HeaderCell.Style = this.RunTimeAttendanceAlreadyRunDataGridViewCellStyle;
                                }
                            }
                        }
                        
                        pvtblnPayCategoryDataGridViewLoaded = true;

                        if (dgvPayCategoryDataGridView.Rows.Count > 0)
                        {
                            this.Set_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView, 0);
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

        private void frmEmployee_FormClosing(object sender, FormClosingEventArgs e)
        {
            miLinkedMenuItem.Enabled = true;
        }

        private void dgvCompanyDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnCompanyDataGridViewLoaded == true)
            {
                if (pvtintCompanyDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintCompanyDataGridViewRowIndex = e.RowIndex;

                    this.btnOK.Enabled = false;

                    pvtint64CompanyNo = Convert.ToInt64(dgvCompanyDataGridView[1, e.RowIndex].Value);
                    string strPayrollType = "";
                    string strPayrollTypePrev = "";
                    string strPayrollTypeDesc = "";

                    this.Clear_DataGridView(this.dgvPayrollTypeDataGridView);
                    this.Clear_DataGridView(this.dgvPayCategoryDataGridView);

                    pvtblnPayrollTypeDataGridViewLoaded = false;
                    pvtblnPayCategoryDataGridViewLoaded = false;

                    pvtPayCategoryClientDataView = null;
                    pvtPayCategoryClientDataView = new DataView(pvtDataSetClient.Tables["PayCategory"],
                        "COMPANY_NO = " + pvtint64CompanyNo.ToString(),
                        "PAY_CATEGORY_TYPE DESC",
                        DataViewRowState.CurrentRows);

                    pvtCompanyDataView = null;
                    pvtCompanyDataView = new DataView(pvtDataSet.Tables["Company"],
                    "COMPANY_NO = " + pvtint64CompanyNo.ToString(),
                     "",
                    DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < this.pvtPayCategoryClientDataView.Count; intRow++)
                    {
                        strPayrollType = pvtPayCategoryClientDataView[intRow]["PAY_CATEGORY_TYPE"].ToString();

                        if (strPayrollType == strPayrollTypePrev)
                        {
                            continue;
                        }
                        else
                        {
                            strPayrollTypePrev = strPayrollType;
                        }

                        //Server 
                            pvtPayCategoryDataView = null;
                        pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"],
                        "COMPANY_NO = " + pvtint64CompanyNo.ToString() + "AND PAY_CATEGORY_TYPE = '" + strPayrollType + "'",
                        "",
                        DataViewRowState.CurrentRows);

                        if (pvtPayCategoryDataView.Count == 0)
                        {
                            //Out of Sync
                            continue;
                        }

                        switch (strPayrollType)
                        {
                            case "W":

                                strPayrollTypeDesc = "Wages";
                                break;

                            case "S":

                                strPayrollTypeDesc = "Salaries";
                                break;

                            case "T":

                                strPayrollTypeDesc = "Time Attendance";
                                break;
                        }

                        this.dgvPayrollTypeDataGridView.Rows.Add(strPayrollTypeDesc,
                                                                 strPayrollType);


                        switch (strPayrollType)
                        {
                            case "W":

                                if (pvtCompanyDataView[0]["WAGE_RUN_IND"].ToString() == "N")
                                {
                                    this.dgvPayrollTypeDataGridView.Rows[this.dgvPayrollTypeDataGridView.Rows.Count - 1].HeaderCell.Style = this.RunTimeAttendancePendingDataGridViewCellStyle;
                                }
                                else
                                {
                                    if (pvtCompanyDataView[0]["WAGE_RUN_IND"].ToString() == "")
                                    {
                                        this.dgvPayrollTypeDataGridView.Rows[this.dgvPayrollTypeDataGridView.Rows.Count - 1].HeaderCell.Style = this.NoPayrollDateOpenDataGridViewCellStyle;
                                    }
                                    else
                                    {
                                        this.dgvPayrollTypeDataGridView.Rows[this.dgvPayrollTypeDataGridView.Rows.Count - 1].HeaderCell.Style = this.RunTimeAttendanceAlreadyRunDataGridViewCellStyle;
                                    }
                                }

                                break;

                            case "S":

                                if (pvtCompanyDataView[0]["SALARY_RUN_IND"].ToString() == "Y")
                                {
                                    this.dgvPayrollTypeDataGridView.Rows[this.dgvPayrollTypeDataGridView.Rows.Count - 1].HeaderCell.Style = this.RunTimeAttendancePendingDataGridViewCellStyle;
                                }
                                else
                                {
                                    this.dgvPayrollTypeDataGridView.Rows[this.dgvPayrollTypeDataGridView.Rows.Count - 1].HeaderCell.Style = this.NoPayrollDateOpenDataGridViewCellStyle;
                                }

                                break;

                            case "T":

                                if (pvtCompanyDataView[0]["TIME_ATTENDANCE_RUN_IND"].ToString() == "N")
                                {
                                    this.dgvPayrollTypeDataGridView.Rows[this.dgvPayrollTypeDataGridView.Rows.Count - 1].HeaderCell.Style = this.RunTimeAttendancePendingDataGridViewCellStyle;
                                }
                                else
                                {
                                    if (pvtCompanyDataView[0]["TIME_ATTENDANCE_RUN_IND"].ToString() == "")
                                    {
                                        this.dgvPayrollTypeDataGridView.Rows[this.dgvPayrollTypeDataGridView.Rows.Count - 1].HeaderCell.Style = this.NoPayrollDateOpenDataGridViewCellStyle;

                                    }
                                    else
                                    {
                                        this.dgvPayrollTypeDataGridView.Rows[this.dgvPayrollTypeDataGridView.Rows.Count - 1].HeaderCell.Style = this.RunTimeAttendanceAlreadyRunDataGridViewCellStyle;
                                    }
                                }

                                break;
                        }
                    }

                    pvtblnPayrollTypeDataGridViewLoaded = true;
                    btnOK.Enabled = false;

                    if (this.dgvPayrollTypeDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView, 0);
                    }
                }
            }
        }

        private void dgvPayCategoryDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 1
            || e.Column.Index == 2
            ||  e.Column.Index == 3)
            {
                if (dgvPayCategoryDataGridView[e.Column.Index + 4, e.RowIndex1].Value.ToString() == "")
                {
                    e.SortResult = -1;
                }
                else
                {
                    if (dgvPayCategoryDataGridView[e.Column.Index + 4, e.RowIndex2].Value.ToString() == "")
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(dgvPayCategoryDataGridView[e.Column.Index + 4, e.RowIndex1].Value.ToString()) > double.Parse(dgvPayCategoryDataGridView[e.Column.Index + 4, e.RowIndex2].Value.ToString()))
                        {
                            e.SortResult = 1;
                        }
                        else
                        {
                            if (double.Parse(dgvPayCategoryDataGridView[e.Column.Index + 4, e.RowIndex1].Value.ToString()) < double.Parse(dgvPayCategoryDataGridView[e.Column.Index + 4, e.RowIndex2].Value.ToString()))
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
