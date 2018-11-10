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
    public partial class frmTimeSheet : Form
    {
        clsISUtilities clsISUtilities;

        private byte[] pvtbytCompress;
        private DataSet pvtDataSet;
        private DataView pvtPayCategoryDataView;
        private DataView pvtBreakRangeDataView;
        private DataView pvtEmployeeOrDateDataView;
        private DataView pvtTempDataView;
        private DataView pvtTempBlankDataView;
        private DataView pvtAuthorisedDataView;
        private DataView pvtEmployeeDataView;
        private DataView pvtDayTotalDataView;
        private DataView pvtBreakDataView;
        private DataView pvtTimeSheetDataView;

        private string pvtstrBlankFilter = "";
        private string pvtstrDataAndTypeFilter = "";
        private string pvtstrDataAndTypeAuthorisedFilter = "";
        private string pvtstrRemoveDataFilter = "";
        private string pvtstrPayCategoryFilter = "";
        private string pvtstrCategoryType = "";

        private DateTime pvtdtFilterEndDate;
        private DateTime pvtDateTime;

        private Int64 pvtint64CompanyNo = -1;
        private int pvtintPayCategoryNo = -1;
        private int pvtintEmployeeNo = -1;
           
        private int pvtintPayCategoryTableRowNo = -1;
        
        private int pvtintEmployeeNoWidth;
        private int pvtintEmployeeCodeWidth;
        private int pvtintEmployeeSurnameWidth;
        private int pvtintEmployeeNameWidth;
        private int pvtintDayExceptionWidth;
        private int pvtintDayPaidHoursWidth;
        private int pvtintDayBreakHoursWidth;

        private int pvtintTotalTimeSheetMinutes = 0;
        private int pvtintTotalBreakMinutes = 0;

        private string pvtstrPayrollType = "";

        private int pvtintPayCategoryDataGridViewRowIndex = -1;
        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
        private int pvtintEmployeeDataGridViewRowIndex = -1;
        private int pvtintDayDataGridViewRowIndex = -1;

        private bool pvtblnBusyLoading = true;
        private bool pvtblnTimeSheetInError = false;
        private bool pvtblnBreakInError = false;

        private bool pvtblnFilterClicked = false;
        
        //dgvTimeSheetDataGridView and dgvBreakDataGridView Cols
        private int pvtintIndicatorColTimeSheetOrBreakDataGridView = 0;
        private int pvtintExcludedFromRunColTimeSheetOrBreakDataGridView = 1;
        private int pvtintClockInColTimeSheetOrBreakDataGridView = 2;
        private int pvtintClockInSetColTimeSheetOrBreakDataGridView = 3;
        private int pvtintClockOutSetColTimeSheetOrBreakDataGridView = 4;
        private int pvtintClockOutColTimeSheetOrBreakDataGridView = 5;
        private int pvtintTotalColTimeSheetOrBreakDataGridView = 6;
        private int pvtintSeqNoColTimeSheetOrBreakDataGridView = 7;

        //Common DataGridView Cols
        private int pvtintIndicatorColDataGridView = 0;
        private int pvtintRunColDataGridView = 1;
        private int pvtintBreakExceptionColDataGridView = 2;
        private int pvtintPublicHolidayColDataGridView = 3;
        private int pvtintOnLeaveColDataGridView = 4;
        private int pvtintExcludedFromRunColDataGridView = 5;

        //dgvPayCategoryDataGridView Cols
        private int pvtintLastUploadDateTimeColPayCategoryDataGridView = 7;
        private int pvtintLastUploadDateTimeSortColPayCategoryDataGridView = 8;
        private int pvtintRowColPayCategoryDataGridView = 9;
        
        //dgvEmployeeDataGridView Cols
        private int pvtintNoColEmployeeDataGridView = 6;
        private int pvtintCodeColEmployeeDataGridView = 7;
        private int pvtintSurnameColEmployeeDataGridView = 8;
        private int pvtintNameColEmployeeDataGridView = 9;
        private int pvtintKeyColEmployeeDataGridView = 10;

        //dgvDayDataGridView Cols
        private int pvtintNoColDayDataGridView = 6;
        private int pvtintCodeColDayDataGridView = 7;
        private int pvtintSurnameColDayDataGridView = 8;
        private int pvtintNameColDayDataGridView = 9;
        private int pvtintExceptionColDayDataGridView = 10;
        private int pvtintTotalHoursColDayDataGridView = 11;
        private int pvtintBreakHoursColDayDataGridView = 12;
        private int pvtintRecordLockedColDayDataGridView = 13;
        private int pvtintKeyColDayDataGridView = 14;

        DataGridViewCellStyle ErrorDataGridViewCellStyle;
        DataGridViewCellStyle ExceptionDataGridViewCellStyle;
        DataGridViewCellStyle NoRecordDataGridViewCellStyle;
        DataGridViewCellStyle NormalDataGridViewCellStyle;
        DataGridViewCellStyle BreakExceptionDataGridViewCellStyle;
        DataGridViewCellStyle WeekEndDataGridViewCellStyle;
        DataGridViewCellStyle LunchTotalDataGridViewCellStyle;
        
        DataGridViewCellStyle PayrollLinkDataGridViewCellStyle;
        DataGridViewCellStyle AuthorisedDataGridViewCellStyle;
        DataGridViewCellStyle PayrollPendingDataGridViewCellStyle;
        DataGridViewCellStyle PublicHolidayDataGridViewCellStyle;
        DataGridViewCellStyle NotIncludedInRunDataGridViewCellStyle;
        DataGridViewCellStyle LeaveDataGridViewCellStyle;
        DataGridViewCellStyle TotalDataGridViewCellStyle;

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnPayCategoryDataGridViewLoaded = false;
        private bool pvtblnEmployeeDataGridViewLoaded = false;
        private bool pvtblnDayDataGridViewLoaded = false;
        private bool pvtblnBreakRangeDataGridViewLoaded = false;
        private bool pvtblnTimeSheetDataGridViewLoaded = false;
        private bool pvtblnBreakDataGridViewLoaded = false;

        private bool pvtblnAllowToEnter = true;

        private bool pvtblnRecordsExcludedFromRun = false;

        public frmTimeSheet()
        {
            InitializeComponent();
            
            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 118;

                this.dgvTimeSheetDataGridView.Height += 114;

                this.lblBreakRange.Top += 114;
                this.dgvBreakRangeDataGridView.Top += 114;
                this.dgvBreakExceptionDataGridView.Top += 114;
                this.dgvTimeSheetTotalsDataGridView.Top += 114;
                this.lblDeleteTimeSheetRowDesc.Top += 114;
                this.btnDeleteRow.Top += 114;

                this.dgvBreakDataGridView.Height += 114;
                this.dgvBreakTotalsDataGridView.Top += 114;

                this.grbBreakError.Top += 114;
                this.grbLeaveError.Top += 114;
                this.lblDeleteBreakRowDesc.Top += 114;
                this.btnDeleteBreakRow.Top += 114;
                this.btnRefresh.Top += 114;

                this.dgvPayCategoryDataGridView.Height += 38;

                int intEmployeeTop = 41;

                this.lblEmployee.Top += intEmployeeTop;
                this.picEmployeeLock.Top += intEmployeeTop;
                this.dgvEmployeeDataGridView.Top += intEmployeeTop;
                this.dgvEmployeeDataGridView.Height += 57;
                
                this.lblDate.Top += 99;
                this.dgvDayDataGridView.Top += 99;
                this.dgvDayDataGridView.Height += 19; 
            }

            if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
            {
                this.rbnGreaterWageDate.Text = "> Time Attendendance Run Date";
                this.rbnDateWageRun.Text = "Time Attendendance Run";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmTimeSheet_Load(object sender, EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this, "busTimeSheet");

                int intTimeout = Convert.ToInt32(AppDomain.CurrentDomain.GetData("TimeSheetReadTimeoutSeconds")) * 1000;

                clsISUtilities.Set_WebService_Timeout_Value(intTimeout);

                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblDate.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblDayDesc.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblTimesheets.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblBreaks.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblBreakBlank.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblBreakStart.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblBreakStop.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblBreakAccum.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                //this.lblBreakRange.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblTimeSheetBlank.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblTimeSheetStart.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblTimeSheetStop.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblTimeSheetAccum.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.dgvBreakExceptionDataGridView.Rows.Add("");

                this.dgvTimeSheetTotalsDataGridView.Rows.Add("Total Worked Hours");
                this.dgvTimeSheetTotalsDataGridView.Rows.Add("Break After 0:00");
                this.dgvTimeSheetTotalsDataGridView.Rows.Add("Total Paid Hours");
                this.dgvTimeSheetTotalsDataGridView.Rows.Add("");

                this.dgvBreakTotalsDataGridView.Rows.Add("",
                                                         "Total Break Hours",
                                                         "0:00");

                pvtintEmployeeNoWidth = this.dgvDayDataGridView.Columns[pvtintNoColDayDataGridView].Width;
                pvtintEmployeeCodeWidth = this.dgvDayDataGridView.Columns[pvtintCodeColDayDataGridView].Width;
                pvtintEmployeeSurnameWidth = this.dgvDayDataGridView.Columns[pvtintSurnameColDayDataGridView].Width;
                pvtintEmployeeNameWidth = this.dgvDayDataGridView.Columns[pvtintNameColDayDataGridView].Width;
                pvtintDayExceptionWidth = this.dgvDayDataGridView.Columns[pvtintExceptionColDayDataGridView].Width;
                pvtintDayPaidHoursWidth = this.dgvDayDataGridView.Columns[pvtintTotalHoursColDayDataGridView].Width;
                pvtintDayBreakHoursWidth = this.dgvDayDataGridView.Columns[pvtintBreakHoursColDayDataGridView].Width;
         
                this.pvtint64CompanyNo = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")); 
             
                ErrorDataGridViewCellStyle = new DataGridViewCellStyle();
                ErrorDataGridViewCellStyle.BackColor = Color.Red;
                ErrorDataGridViewCellStyle.SelectionBackColor = Color.Red;

                ExceptionDataGridViewCellStyle = new DataGridViewCellStyle();
                ExceptionDataGridViewCellStyle.BackColor = Color.Lime;
                ExceptionDataGridViewCellStyle.SelectionBackColor = Color.Lime;

                NoRecordDataGridViewCellStyle = new DataGridViewCellStyle();
                NoRecordDataGridViewCellStyle.BackColor = Color.Yellow;
                NoRecordDataGridViewCellStyle.SelectionBackColor = Color.Yellow;

                NormalDataGridViewCellStyle = new DataGridViewCellStyle();
                NormalDataGridViewCellStyle.BackColor = SystemColors.Control;
                NormalDataGridViewCellStyle.SelectionBackColor = SystemColors.Control;

                BreakExceptionDataGridViewCellStyle = new DataGridViewCellStyle();
                BreakExceptionDataGridViewCellStyle.BackColor = Color.Teal;
                BreakExceptionDataGridViewCellStyle.SelectionBackColor = Color.Teal;

                WeekEndDataGridViewCellStyle = new DataGridViewCellStyle();
                WeekEndDataGridViewCellStyle.BackColor = SystemColors.Info;

                LunchTotalDataGridViewCellStyle = new DataGridViewCellStyle();
                LunchTotalDataGridViewCellStyle.BackColor = Color.Silver;

                PayrollLinkDataGridViewCellStyle = new DataGridViewCellStyle();
                PayrollLinkDataGridViewCellStyle.BackColor = Color.Magenta;
                PayrollLinkDataGridViewCellStyle.SelectionBackColor = Color.Magenta;

                AuthorisedDataGridViewCellStyle = new DataGridViewCellStyle();
                AuthorisedDataGridViewCellStyle.BackColor = Color.Cyan;
                AuthorisedDataGridViewCellStyle.SelectionBackColor = Color.Cyan;

                PayrollPendingDataGridViewCellStyle = new DataGridViewCellStyle();
                PayrollPendingDataGridViewCellStyle.BackColor = Color.Olive;
                PayrollPendingDataGridViewCellStyle.SelectionBackColor = Color.Olive;

                PublicHolidayDataGridViewCellStyle = new DataGridViewCellStyle();
                PublicHolidayDataGridViewCellStyle.BackColor = Color.SlateBlue;
                PublicHolidayDataGridViewCellStyle.SelectionBackColor = Color.SlateBlue;
                
                LeaveDataGridViewCellStyle = new DataGridViewCellStyle();
                LeaveDataGridViewCellStyle.BackColor = Color.RoyalBlue;
                LeaveDataGridViewCellStyle.SelectionBackColor = Color.RoyalBlue;
                
                NotIncludedInRunDataGridViewCellStyle = new DataGridViewCellStyle();
                NotIncludedInRunDataGridViewCellStyle.BackColor = Color.Orange;
                NotIncludedInRunDataGridViewCellStyle.SelectionBackColor = Color.Orange;

                TotalDataGridViewCellStyle = new DataGridViewCellStyle();
                TotalDataGridViewCellStyle.BackColor = SystemColors.ControlDarkDark;
                TotalDataGridViewCellStyle.SelectionBackColor = SystemColors.ControlDarkDark;
                TotalDataGridViewCellStyle.ForeColor = Color.White;
                TotalDataGridViewCellStyle.SelectionForeColor = Color.White;

                this.dgvBreakTotalsDataGridView[2, 0].Style = this.TotalDataGridViewCellStyle;
                this.dgvTimeSheetTotalsDataGridView[1, 3].Style = this.TotalDataGridViewCellStyle;
          
                object[] objParm = new object[4];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[2] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[3] =  AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();
               
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                DataTable myDataTable = new DataTable("DateSelection");
                myDataTable.Columns.Add("ACTUAL_DATE", typeof(String));
                
                pvtDataSet.Tables.Add(myDataTable);
                
                if (pvtDataSet.Tables["PayCategory"].Rows.Count > 0)
                {
                    Create_DayBlank_Records("P",pvtDataSet.Tables["PayCategory"].Rows[0]["PAY_CATEGORY_TYPE"].ToString(),-1,DateTime.Now,-1);
                }

                pvtDataSet.AcceptChanges();
                
                //Sets Correct Column Headers when Fired this Way
                rbnDateEmployee_Click(sender, e);

                pvtblnBusyLoading = false;
                string strEndDateStart = "";
                
                for (int intRow = 0; intRow < pvtDataSet.Tables["PayrollType"].Rows.Count; intRow++)
                {
                    if (pvtDataSet.Tables["PayrollType"].Rows[intRow]["MIN_EMPLOYEE_LAST_RUNDATE"] == System.DBNull.Value)
                    {
                        strEndDateStart = DateTime.Now.AddDays(-30).ToString("yyyyMMdd");
                    }
                    else
                    {
                        strEndDateStart = Convert.ToDateTime(pvtDataSet.Tables["PayrollType"].Rows[intRow]["MIN_EMPLOYEE_LAST_RUNDATE"]).ToString("yyyyMMdd");
                    }

                    this.dgvPayrollTypeDataGridView.Rows.Add(pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE_DESC"].ToString()
                                                             ,strEndDateStart);
                }

                pvtblnPayrollTypeDataGridViewLoaded = true;

                if (this.dgvPayrollTypeDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView, 0);
                }

                if (pvtblnRecordsExcludedFromRun == true)
                {
                    tmrExcludedFromRun.Enabled = true;
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
            switch (myDataGridView.Name)
            {
                case "dgvPayrollTypeDataGridView":

                    pvtintPayrollTypeDataGridViewRowIndex = -1;
                    break;

                case "dgvPayCategoryDataGridView":

                    pvtintPayCategoryDataGridViewRowIndex = -1;
                    break;

                case "dgvEmployeeDataGridView":

                    pvtintEmployeeDataGridViewRowIndex = -1;
                    break;

                case "dgvDayDataGridView":

                    pvtintDayDataGridViewRowIndex = -1;
                    break;

                default:

                    break;
            }
          
            //Fires DataGridView RowEnter Function
            if (this.Get_DataGridView_SelectedRowIndex(myDataGridView) == intRow)
            {
                DataGridViewCellEventArgs myDataGridViewCellEventArgs = new DataGridViewCellEventArgs(0, intRow);

                switch (myDataGridView.Name)
                {
                    case "dgvPayrollTypeDataGridView":

                        dgvPayrollTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPayCategoryDataGridView":

                        this.dgvPayCategoryDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEmployeeDataGridView":

                        this.dgvEmployeeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvDayDataGridView":

                        this.dgvDayDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvBreakDataGridView":

                        this.dgvBreakDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvTimeSheetDataGridView":

                        this.dgvTimeSheetDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvBreakRangeDataGridView":

                        this.dgvBreakRangeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    default:

                        MessageBox.Show("No Entry for " + myDataGridView.Name + " in Set_DataGridView_SelectedRowIndex", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            else
            {
                if (myDataGridView.Name == "dgvTimeSheetDataGridView"
                    | myDataGridView.Name == "dgvBreakDataGridView")
                {
                    myDataGridView.CurrentCell = myDataGridView[1, intRow];
                }
                else
                {
                    myDataGridView.CurrentCell = myDataGridView[0, intRow];
                }
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
        
        private void rbnDateEmployee_Click(object sender, EventArgs e)
        {
            this.Clear_DataGridView(this.dgvEmployeeDataGridView);
            this.Clear_DataGridView(this.dgvDayDataGridView);

            this.Clear_DataGridView(this.dgvTimeSheetDataGridView);
            this.Clear_DataGridView(this.dgvBreakDataGridView);

            //Set so that First Rows will be Selected
            pvtintEmployeeNo = -1;
            pvtDateTime = DateTime.Now.AddYears(100);

            this.lblEmployee.Text = "Date";
            this.lblDate.Text = "Employee";

            //ELR - 2015-03-20
            this.dgvEmployeeDataGridView.Columns[pvtintNoColEmployeeDataGridView].HeaderText = "Description";
            this.dgvEmployeeDataGridView.Columns[pvtintNoColEmployeeDataGridView].Width = pvtintEmployeeNoWidth + pvtintEmployeeCodeWidth + pvtintEmployeeSurnameWidth + pvtintEmployeeNameWidth + pvtintDayExceptionWidth + pvtintDayBreakHoursWidth + pvtintDayPaidHoursWidth;

            this.dgvEmployeeDataGridView.Columns[pvtintCodeColEmployeeDataGridView].Visible = false;
            this.dgvEmployeeDataGridView.Columns[pvtintSurnameColEmployeeDataGridView].Visible = false;
            this.dgvEmployeeDataGridView.Columns[pvtintNameColEmployeeDataGridView].Visible = false;

            this.dgvDayDataGridView.Columns[pvtintNoColDayDataGridView].HeaderText = "No.";
            this.dgvDayDataGridView.Columns[pvtintNoColDayDataGridView].Width = pvtintEmployeeNoWidth;
            
            this.dgvDayDataGridView.Columns[pvtintCodeColDayDataGridView].HeaderText = "Code";
            this.dgvDayDataGridView.Columns[pvtintCodeColDayDataGridView].Width = pvtintEmployeeCodeWidth;
            this.dgvDayDataGridView.Columns[pvtintCodeColDayDataGridView].SortMode = DataGridViewColumnSortMode.Automatic;


            this.dgvDayDataGridView.Columns[pvtintSurnameColDayDataGridView].HeaderText = "Surname";
            this.dgvDayDataGridView.Columns[pvtintSurnameColDayDataGridView].Width = pvtintEmployeeSurnameWidth;
            this.dgvDayDataGridView.Columns[pvtintSurnameColDayDataGridView].SortMode = DataGridViewColumnSortMode.Automatic;
            
            this.dgvDayDataGridView.Columns[pvtintNameColDayDataGridView].HeaderText = "Name";
            this.dgvDayDataGridView.Columns[pvtintNameColDayDataGridView].Width = pvtintEmployeeNameWidth;
            this.dgvDayDataGridView.Columns[pvtintNameColDayDataGridView].SortMode = DataGridViewColumnSortMode.Automatic;

            this.dgvDayDataGridView.Columns[pvtintSurnameColDayDataGridView].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.dgvDayDataGridView.Columns[pvtintNameColDayDataGridView].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            this.dgvDayDataGridView.Columns[pvtintExceptionColDayDataGridView].HeaderText = "Exception";
            this.dgvDayDataGridView.Columns[pvtintExceptionColDayDataGridView].Width = pvtintDayExceptionWidth;
            
            this.dgvDayDataGridView.Columns[pvtintTotalHoursColDayDataGridView].HeaderText = "Total Hrs";
            this.dgvDayDataGridView.Columns[pvtintTotalHoursColDayDataGridView].Width = pvtintDayPaidHoursWidth;
            
            this.dgvDayDataGridView.Columns[pvtintBreakHoursColDayDataGridView].HeaderText = "Break Hrs";
            this.dgvDayDataGridView.Columns[pvtintBreakHoursColDayDataGridView].Width = pvtintDayBreakHoursWidth;

            this.dgvDayDataGridView.Columns[pvtintExceptionColDayDataGridView].Visible = true;
            this.dgvDayDataGridView.Columns[pvtintTotalHoursColDayDataGridView].Visible = true;
            this.dgvDayDataGridView.Columns[pvtintBreakHoursColDayDataGridView].Visible = true;

            if (pvtblnBusyLoading == false)
            {
                Load_PayCategory_Records();
            }
        }

        private void rbnEmployeeDate_Click(object sender, EventArgs e)
        {
            //Set so that First Rows will be Selected
            pvtintEmployeeNo = -1;
            pvtDateTime = DateTime.Now.AddYears(100);

            this.lblEmployee.Text = "Employee";
            this.lblDate.Text = "Date";

            //ELR - 2015-03-20
            this.dgvEmployeeDataGridView.Columns[pvtintNoColEmployeeDataGridView].HeaderText = "No.";
            this.dgvEmployeeDataGridView.Columns[pvtintNoColEmployeeDataGridView].Width = pvtintEmployeeNoWidth;
            
            this.dgvEmployeeDataGridView.Columns[pvtintCodeColEmployeeDataGridView].HeaderText = "Code";
            this.dgvEmployeeDataGridView.Columns[pvtintCodeColEmployeeDataGridView].Width = pvtintEmployeeCodeWidth;
            this.dgvEmployeeDataGridView.Columns[pvtintSurnameColEmployeeDataGridView].HeaderText = "Surname";
            this.dgvEmployeeDataGridView.Columns[pvtintSurnameColEmployeeDataGridView].Width = pvtintEmployeeSurnameWidth + pvtintDayPaidHoursWidth;
            this.dgvEmployeeDataGridView.Columns[pvtintNameColEmployeeDataGridView].HeaderText = "Name";
            this.dgvEmployeeDataGridView.Columns[pvtintNameColEmployeeDataGridView].Width = pvtintEmployeeNameWidth + pvtintDayExceptionWidth + pvtintDayBreakHoursWidth;

            this.dgvEmployeeDataGridView.Columns[pvtintCodeColEmployeeDataGridView].Visible = true;
            this.dgvEmployeeDataGridView.Columns[pvtintSurnameColEmployeeDataGridView].Visible = true;
            this.dgvEmployeeDataGridView.Columns[pvtintNameColEmployeeDataGridView].Visible = true;
            
            this.dgvDayDataGridView.Columns[pvtintNoColDayDataGridView].HeaderText = "Description";
            this.dgvDayDataGridView.Columns[pvtintNoColDayDataGridView].Width = pvtintEmployeeNoWidth + pvtintEmployeeCodeWidth + pvtintEmployeeSurnameWidth + pvtintEmployeeNameWidth;

            this.dgvDayDataGridView.Columns[pvtintCodeColDayDataGridView].HeaderText = "Exception";
            this.dgvDayDataGridView.Columns[pvtintCodeColDayDataGridView].Width = pvtintDayExceptionWidth;
            this.dgvDayDataGridView.Columns[pvtintCodeColDayDataGridView].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvDayDataGridView.Columns[pvtintSurnameColDayDataGridView].HeaderText = "Total Hrs";
            this.dgvDayDataGridView.Columns[pvtintSurnameColDayDataGridView].Width = pvtintDayPaidHoursWidth;
            this.dgvDayDataGridView.Columns[pvtintSurnameColDayDataGridView].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvDayDataGridView.Columns[pvtintNameColDayDataGridView].HeaderText = "Break Hrs";
            this.dgvDayDataGridView.Columns[pvtintNameColDayDataGridView].Width = pvtintDayBreakHoursWidth;
            this.dgvDayDataGridView.Columns[pvtintNameColDayDataGridView].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvDayDataGridView.Columns[pvtintExceptionColDayDataGridView].Visible = false;
            this.dgvDayDataGridView.Columns[pvtintTotalHoursColDayDataGridView].Visible = false;
            this.dgvDayDataGridView.Columns[pvtintBreakHoursColDayDataGridView].Visible = false;

            this.dgvDayDataGridView.Columns[pvtintSurnameColDayDataGridView].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvDayDataGridView.Columns[pvtintNameColDayDataGridView].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            if (pvtblnBusyLoading == false)
            {
                Load_PayCategory_Records();
            }
        }

        private void Load_PayCategory_Records()
        {
            if (pvtstrPayrollType == "")
            {
                return;
            }

            pvtblnBusyLoading = true;

            this.Cursor = Cursors.WaitCursor;

            this.pvtblnPayCategoryDataGridViewLoaded = false;
            pvtblnEmployeeDataGridViewLoaded = false;
            pvtblnDayDataGridViewLoaded = false;
            pvtblnBreakRangeDataGridViewLoaded = false;
            pvtblnTimeSheetDataGridViewLoaded = false;
            pvtblnBreakDataGridViewLoaded = false;

            pvtintPayCategoryDataGridViewRowIndex = -1;

            this.lblDayDesc.Text = "";

            //Clear Totals
            this.dgvTimeSheetTotalsDataGridView[1, 0].Value = "0.00";
            this.dgvTimeSheetTotalsDataGridView[1, 1].Value = "0.00";
            this.dgvTimeSheetTotalsDataGridView[1, 2].Value = "0.00";
            this.dgvTimeSheetTotalsDataGridView[1, 3].Value = "0.00";

            this.dgvTimeSheetTotalsDataGridView[0, 1].Value = "Break After 0:00";

            int intPayCategoryRow = 0;

            this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
            this.Clear_DataGridView(this.dgvEmployeeDataGridView);
            this.Clear_DataGridView(this.dgvDayDataGridView);

            Clear_DataGridView(dgvTimeSheetDataGridView);
            Clear_DataGridView(dgvBreakRangeDataGridView);
            Clear_DataGridView(dgvBreakDataGridView);

            this.dgvBreakTotalsDataGridView[2, 0].Value = "0.00";

            dgvBreakTotalsDataGridView[0, 0].Style = LunchTotalDataGridViewCellStyle;

            this.grbBreakError.Visible = false;
            this.grbLeaveError.Visible = false;

            this.btnDelete.Enabled = false;
            this.btnUpdate.Enabled = false;
            this.btnRefresh.Enabled = false;

            if (this.chkEndDateOnly.Checked == true)
            {
                pvtstrDataAndTypeFilter = " AND DAY_DATE = '" + pvtdtFilterEndDate.ToString("yyyy-MM-dd") + "'";
            }
            else
            {
                pvtstrDataAndTypeFilter = " AND DAY_DATE <= '" + pvtdtFilterEndDate.ToString("yyyy-MM-dd") + "'";
            }

            //NB At Take-On their will be No PayCategories
            if (pvtintPayCategoryTableRowNo > -1)
            {
                if (pvtPayCategoryDataView.Count > 0)
                {
                    if (this.rbnDateWageRun.Checked == true)
                    {
                        pvtstrDataAndTypeFilter += " AND DAY_DATE <= '" + Convert.ToDateTime(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'";
                    }
                    else
                    {
                        if (this.rbnGreaterWageDate.Checked == true)
                        {
                            pvtstrDataAndTypeFilter += " AND DAY_DATE >= '" + Convert.ToDateTime(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_PERIOD_DATE"]).AddDays(1).ToString("yyyy-MM-dd") + "'";
                        }
                    }
                }
            }

            pvtstrDataAndTypeAuthorisedFilter = pvtstrDataAndTypeFilter.Replace("DAY_DATE", "EMPLOYEE_LAST_RUNDATE");

            if (this.rbnBlank.Checked == true)
            {
                pvtstrBlankFilter = " AND NOT INDICATOR = 'X'";
                
                if (this.chkRemoveSat.Checked == true
                || this.chkRemoveSun.Checked == true)
                {
                    pvtstrDataAndTypeFilter += " AND NOT DAY_NO IN (";

                    if (this.chkRemoveSat.Checked == true)
                    {
                        pvtstrDataAndTypeFilter += "6";
                    }

                    if (this.chkRemoveSun.Checked == true)
                    {
                        if (this.chkRemoveSat.Checked == true)
                        {
                            pvtstrDataAndTypeFilter += ",0";
                        }
                        else
                        {
                            pvtstrDataAndTypeFilter += "0";
                        }
                    }

                    pvtstrDataAndTypeFilter += ")";
                }
            }
            else
            {
                if (this.rbnOnLeave.Checked == true)
                {
                    pvtstrBlankFilter = " AND LEAVE_INDICATOR = 'Y'";
                }
            }

            if (this.chkRemoveSat.Checked == true
            || this.chkRemoveSun.Checked == true)
            {
                pvtstrRemoveDataFilter = " AND NOT DAY_NO IN (";

                if (this.chkRemoveSat.Checked == true)
                {
                    pvtstrRemoveDataFilter += "6";
                }

                if (this.chkRemoveSun.Checked == true)
                {
                    if (this.chkRemoveSat.Checked == true)
                    {
                        pvtstrRemoveDataFilter += ",0";
                    }
                    else
                    {
                        pvtstrRemoveDataFilter += "0";
                    }
                }

                pvtstrRemoveDataFilter += ")";
            }
            else
            {
                pvtstrRemoveDataFilter = "";
            }
            
            if (this.chkRemovePublicHolidays.Checked == true)
            {
                pvtstrRemoveDataFilter = pvtstrRemoveDataFilter + " AND NOT PAID_HOLIDAY_INDICATOR = 'Y'";

                pvtstrBlankFilter += " AND NOT PAID_HOLIDAY_INDICATOR = 'Y'";
            }

            if (this.chkRemoveOnLeave.Checked == true)
            {
                pvtstrBlankFilter = pvtstrBlankFilter + " AND NOT LEAVE_INDICATOR = 'Y'";
            }

            if (this.rbnErrors.Checked == true)
            {
                pvtstrCategoryType = " AND INDICATOR = 'X'";
            }
            else
            {
                if (this.rbnException.Checked == true)
                {
                    pvtstrCategoryType = " AND INDICATOR = 'E'";
                }
                else
                {
                    if (this.rbnNormal.Checked == true)
                    {
                        pvtstrCategoryType = " AND INDICATOR = ''";
                    }
                    else
                    {
                        if (this.rbnBreakException.Checked == true)
                        {
                            pvtstrCategoryType = " AND BREAK_INDICATOR = 'Y'";
                        }
                        else
                        {
                            if (this.rbnPublicHoliday.Checked == true)
                            {
                                pvtstrCategoryType = " AND PAID_HOLIDAY_INDICATOR = 'Y'";
                            }
                            else
                            {
                                if (this.rbnExcludedFromRun.Checked == true)
                                {
                                    pvtstrCategoryType = " AND INCLUDED_IN_RUN_INDICATOR = 'N'";
                                }
                                else
                                {
                                    pvtstrCategoryType = "";
                                }
                            }
                        }
                    }
                }
            }

            string strWageRunFilter = "";

            if (this.rbnDateWageRun.Checked == true)
            {
                strWageRunFilter = " AND NOT PAY_PERIOD_DATE IS NULL";
            }

            pvtPayCategoryDataView = null;
            pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"],
                "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'" + strWageRunFilter,
                "",
                DataViewRowState.CurrentRows);

            string strLastUploadDateTime = "";
            string strLastUploadDateTimeSort = "";
            
            for (int intRow = 0; intRow < pvtPayCategoryDataView.Count; intRow++)
            {
                strLastUploadDateTime = "";
                strLastUploadDateTimeSort = "";
                
                DataView DataView = null;
                DataView BlankDataView = null;

                if (this.rbnBlank.Checked == true
                || this.rbnOnLeave.Checked == true)
                {
                    DataView = new System.Data.DataView(pvtDataSet.Tables["DayBlank"],
                                       "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_CATEGORY_NO = " + pvtPayCategoryDataView[intRow]["PAY_CATEGORY_NO"] + pvtstrDataAndTypeFilter + pvtstrBlankFilter,
                                       "INDICATOR DESC",
                                       DataViewRowState.CurrentRows);
                }
                else
                {
                    DataView = new System.Data.DataView(pvtDataSet.Tables["DayTotal"],
                                        "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_CATEGORY_NO = " + pvtPayCategoryDataView[intRow]["PAY_CATEGORY_NO"] + pvtstrDataAndTypeFilter + pvtstrRemoveDataFilter + pvtstrCategoryType,
                                        "INDICATOR DESC",
                                        DataViewRowState.CurrentRows);

                    BlankDataView = new System.Data.DataView(pvtDataSet.Tables["DayBlank"],
                                       "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_CATEGORY_NO = " + pvtPayCategoryDataView[intRow]["PAY_CATEGORY_NO"] + pvtstrDataAndTypeFilter + pvtstrRemoveDataFilter,
                                       "LEAVE_INDICATOR DESC",
                                       DataViewRowState.CurrentRows);
                }

                //NB Blank Cannot be Checked - it Wont Exist
                if ((this.rbnErrors.Checked == true
                || this.rbnException.Checked == true
                || this.rbnNormal.Checked == true
                || this.rbnBreakException.Checked == true
                || this.rbnPublicHoliday.Checked == true
                || this.rbnExcludedFromRun.Checked == true
                || this.rbnOnLeave.Checked == true
                || this.rbnBlank.Checked == true
                || this.chkRemoveBlanks.Checked == true)
                && DataView.Count == 0)
                {
                    continue;
                }

                if (pvtPayCategoryDataView[intRow]["LAST_UPLOAD_DATETIME"] != System.DBNull.Value)
                {
                    strLastUploadDateTime = Convert.ToDateTime(pvtPayCategoryDataView[intRow]["LAST_UPLOAD_DATETIME"]).ToString("dd MMM yyyy - HH:mm");

                    strLastUploadDateTimeSort = Convert.ToDateTime(pvtPayCategoryDataView[intRow]["LAST_UPLOAD_DATETIME"]).ToString("yyyyMMddHHmm");
                }

                this.dgvPayCategoryDataGridView.Rows.Add("",
                                                         "",
                                                         "",
                                                         "",
                                                         "",
                                                         "",
                                                         pvtPayCategoryDataView[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                         strLastUploadDateTime,
                                                         strLastUploadDateTimeSort,
                                                         intRow.ToString());

                if (this.rbnBlank.Checked == true)
                {
                    this.dgvPayCategoryDataGridView[pvtintIndicatorColDataGridView, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = NoRecordDataGridViewCellStyle;

                    if (DataView.Count > 0)
                    {
                        if (DataView[0]["INDICATOR"].ToString() == "X")
                        {
                            this.dgvPayCategoryDataGridView[pvtintIndicatorColDataGridView, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = ErrorDataGridViewCellStyle;
                        }
                    }
                }
                else
                {
                    if (DataView.Count > 0)
                    {
                        if (DataView[0]["INDICATOR"].ToString() == "X")
                        {
                            this.dgvPayCategoryDataGridView[pvtintIndicatorColDataGridView, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = ErrorDataGridViewCellStyle;
                        }
                        else
                        {
                            if (DataView[0]["INDICATOR"].ToString() == "E")
                            {
                                this.dgvPayCategoryDataGridView[pvtintIndicatorColDataGridView, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = ExceptionDataGridViewCellStyle;
                            }
                            else
                            {
                                if (DataView[0]["INDICATOR"].ToString() == "B")
                                {
                                    this.dgvPayCategoryDataGridView[pvtintIndicatorColDataGridView, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = NoRecordDataGridViewCellStyle;
                                }
                            }
                        }
                    }
                    else
                    {
                        this.dgvPayCategoryDataGridView[pvtintIndicatorColDataGridView, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = NoRecordDataGridViewCellStyle;
                    }
                }
                
                if (this.rbnGreaterWageDate.Checked == false)
                {
                    if (pvtPayCategoryDataView[intRow]["PAY_PERIOD_DATE"] != System.DBNull.Value)
                    {
                        if (pvtPayCategoryDataView[intRow]["RUN_IND"].ToString() == "Y")
                        {
                            if (this.rbnErrors.Checked == false)
                            {
                                dgvPayCategoryDataGridView[pvtintRunColDataGridView, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = PayrollLinkDataGridViewCellStyle;
                            }
                        }
                        else
                        {
                            pvtAuthorisedDataView = null;
                            pvtAuthorisedDataView = new DataView(this.pvtDataSet.Tables["Authorised"],
                                "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'" + pvtstrDataAndTypeAuthorisedFilter,
                                "",
                                DataViewRowState.CurrentRows);

                            if (pvtAuthorisedDataView.Count > 0)
                            {
                                dgvPayCategoryDataGridView[pvtintRunColDataGridView, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = AuthorisedDataGridViewCellStyle;
                            }
                            else
                            {
                                dgvPayCategoryDataGridView[pvtintRunColDataGridView, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = PayrollPendingDataGridViewCellStyle;
                            }
                        }
                    }
                }

                if (DataView.Count > 0)
                {
                    DataView.Sort = "BREAK_INDICATOR DESC";

                    if (DataView[0]["BREAK_INDICATOR"].ToString() == "Y")
                    {
                        dgvPayCategoryDataGridView[pvtintBreakExceptionColDataGridView, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = BreakExceptionDataGridViewCellStyle;
                    }

                    DataView.Sort = "PAID_HOLIDAY_INDICATOR DESC";

                    if (DataView[0]["PAID_HOLIDAY_INDICATOR"].ToString() == "Y")
                    {
                        dgvPayCategoryDataGridView[pvtintPublicHolidayColDataGridView, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = PublicHolidayDataGridViewCellStyle;
                    }
                    
                    DataView.Sort = "LEAVE_INDICATOR DESC";

                    if (DataView[0]["LEAVE_INDICATOR"].ToString() == "Y")
                    {
                        dgvPayCategoryDataGridView[pvtintOnLeaveColDataGridView, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = this.LeaveDataGridViewCellStyle;
                    }
                    else
                    {
                        if (this.rbnNone.Checked == true)
                        {
                            if (BlankDataView.Count > 0)
                            {
                                if (BlankDataView[0]["LEAVE_INDICATOR"].ToString() == "Y")
                                {
                                    dgvPayCategoryDataGridView[pvtintOnLeaveColDataGridView, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = this.LeaveDataGridViewCellStyle;
                                }
                            }
                        }
                    }
                    
                    //ELR - 20150516
                    DataView.RowFilter += " AND INCLUDED_IN_RUN_INDICATOR = 'N'";

                    if (DataView.Count > 0)
                    {
                        dgvPayCategoryDataGridView[pvtintExcludedFromRunColDataGridView, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = this.NotIncludedInRunDataGridViewCellStyle;
                        pvtblnRecordsExcludedFromRun = true;
                    }
                }
                else
                {
                    if (BlankDataView.Count > 0)
                    {
                        if (BlankDataView[0]["LEAVE_INDICATOR"].ToString() == "Y")
                        {
                            dgvPayCategoryDataGridView[pvtintOnLeaveColDataGridView, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = this.LeaveDataGridViewCellStyle;
                        }

                        BlankDataView.Sort = "PAID_HOLIDAY_INDICATOR DESC";

                        if (BlankDataView[0]["PAID_HOLIDAY_INDICATOR"].ToString() == "Y")
                        {
                            dgvPayCategoryDataGridView[pvtintPublicHolidayColDataGridView, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = PublicHolidayDataGridViewCellStyle;
                        }
                    }
                }

                if (Convert.ToInt32(pvtPayCategoryDataView[intRow]["PAY_CATEGORY_NO"]) == this.pvtintPayCategoryNo)
                {
                    intPayCategoryRow = this.dgvPayCategoryDataGridView.Rows.Count - 1;
                }
            }

            this.pvtblnPayCategoryDataGridViewLoaded = true;

            if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, intPayCategoryRow);
            }

            this.Cursor = Cursors.Default;

            pvtblnBusyLoading = false;
        }

        private void cboDateFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboDateFilter.SelectedIndex > -1)
            {
                pvtdtFilterEndDate = Convert.ToDateTime(pvtDataSet.Tables["DateSelection"].Rows[cboDateFilter.SelectedIndex]["ACTUAL_DATE"]);
                
                if (pvtblnBusyLoading == false)
                {
                    Load_PayCategory_Records();
                }
            }
        }

        private void Load_Employee_SpreadSheet()
        {
            this.Clear_DataGridView(this.dgvEmployeeDataGridView);
            this.Clear_DataGridView(this.dgvDayDataGridView);
            this.Clear_DataGridView(dgvTimeSheetDataGridView);
            this.Clear_DataGridView(dgvBreakDataGridView);
   
            //Set To Choose First Row In Spreadsheet
            int intEmployeeNoRow = 0;

            this.lblDayDesc.Text = "";

            if (this.rbnEmployeeDate.Checked == true)
            {
                pvtEmployeeOrDateDataView = null;
                pvtEmployeeOrDateDataView = new DataView(this.pvtDataSet.Tables["Employee"],
                    pvtstrPayCategoryFilter + " AND EMPLOYEE_LAST_RUNDATE < '" + pvtdtFilterEndDate.ToString("yyyy-MM-dd") + "'",
                    "EMPLOYEE_CODE",
                    DataViewRowState.CurrentRows);
            }
            else
            {
                pvtEmployeeOrDateDataView = null;
                pvtEmployeeOrDateDataView = new DataView(this.pvtDataSet.Tables["Dates"],
                    pvtstrPayCategoryFilter + pvtstrDataAndTypeFilter + pvtstrRemoveDataFilter,
                    "DAY_DATE DESC",
                    DataViewRowState.CurrentRows);
            }

            this.pvtblnEmployeeDataGridViewLoaded = false;
            pvtintEmployeeDataGridViewRowIndex = -1;
            string strRecordLocked = "";

            //Set Error
            for (int intRow = 0; intRow < pvtEmployeeOrDateDataView.Count; intRow++)
            {
                pvtTempDataView = null;
                pvtTempBlankDataView = null;
                strRecordLocked = "";

                if (rbnEmployeeDate.Checked == true)
                {
                    if (this.rbnBlank.Checked == true
                    || this.rbnOnLeave.Checked == true)
                    {
                        pvtTempDataView = new DataView(this.pvtDataSet.Tables["DayBlank"],
                        pvtstrPayCategoryFilter + " AND EMPLOYEE_NO = " + pvtEmployeeOrDateDataView[intRow]["EMPLOYEE_NO"].ToString() + " " + pvtstrDataAndTypeFilter + pvtstrBlankFilter,
                        "INDICATOR DESC",
                        DataViewRowState.CurrentRows);
                    }
                    else
                    {
                        pvtTempDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                        pvtstrPayCategoryFilter + " AND EMPLOYEE_NO = " + pvtEmployeeOrDateDataView[intRow]["EMPLOYEE_NO"].ToString() + " " + pvtstrDataAndTypeFilter + pvtstrRemoveDataFilter + pvtstrCategoryType,
                        "INDICATOR DESC",
                        DataViewRowState.CurrentRows);

                        pvtTempBlankDataView = new DataView(this.pvtDataSet.Tables["DayBlank"],
                        pvtstrPayCategoryFilter + " AND EMPLOYEE_NO = " + pvtEmployeeOrDateDataView[intRow]["EMPLOYEE_NO"].ToString() + " " + pvtstrDataAndTypeFilter + pvtstrRemoveDataFilter,
                        "LEAVE_INDICATOR DESC",
                        DataViewRowState.CurrentRows);
                    }
                }
                else
                {
                    if (this.rbnBlank.Checked == true
                    || this.rbnOnLeave.Checked == true)
                    {
                        pvtTempDataView = new System.Data.DataView(pvtDataSet.Tables["DayBlank"],
                                     pvtstrPayCategoryFilter + " AND DAY_DATE = '" + Convert.ToDateTime(pvtEmployeeOrDateDataView[intRow]["DAY_DATE"]).ToString("yyyy-MM-dd") + "' " + pvtstrDataAndTypeFilter + pvtstrBlankFilter,
                                     "INDICATOR DESC",
                                     DataViewRowState.CurrentRows);
                    }
                    else
                    {
                        pvtTempDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                            pvtstrPayCategoryFilter + " AND DAY_DATE = '" + Convert.ToDateTime(pvtEmployeeOrDateDataView[intRow]["DAY_DATE"]).ToString("yyyy-MM-dd") + "' " + pvtstrDataAndTypeFilter + pvtstrRemoveDataFilter + pvtstrCategoryType,
                            "INDICATOR DESC",
                                DataViewRowState.CurrentRows);

                        pvtTempBlankDataView = new System.Data.DataView(pvtDataSet.Tables["DayBlank"],
                                    pvtstrPayCategoryFilter + " AND DAY_DATE = '" + Convert.ToDateTime(pvtEmployeeOrDateDataView[intRow]["DAY_DATE"]).ToString("yyyy-MM-dd") + "' " + pvtstrDataAndTypeFilter + pvtstrRemoveDataFilter,
                                    "LEAVE_INDICATOR DESC",
                                    DataViewRowState.CurrentRows);
                    }
                }
                
                if (((this.rbnErrors.Checked == true
                || this.rbnException.Checked == true
                || this.rbnNormal.Checked == true
                || this.rbnBreakException.Checked == true
                || this.rbnPublicHoliday.Checked == true
                || this.rbnExcludedFromRun.Checked == true
                || this.rbnBlank.Checked == true
                || this.rbnOnLeave.Checked == true
                || this.chkRemoveBlanks.Checked == true)
                && pvtTempDataView.Count == 0))
                {
                    continue;
                }

                if (rbnEmployeeDate.Checked == true)
                {
                    this.dgvEmployeeDataGridView.Rows.Add("",
                                                          "",
                                                          "",
                                                          "",
                                                          "",
                                                          "",
                                                          pvtEmployeeOrDateDataView[intRow]["EMPLOYEE_NO"].ToString(),
                                                          pvtEmployeeOrDateDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                          pvtEmployeeOrDateDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                          pvtEmployeeOrDateDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                          intRow.ToString());

                    if (pvtintEmployeeNo == Convert.ToInt32(pvtEmployeeOrDateDataView[intRow]["EMPLOYEE_NO"]))
                    {
                        intEmployeeNoRow = this.dgvEmployeeDataGridView.Rows.Count - 1;
                    }

                    if (this.rbnGreaterWageDate.Checked == false)
                    {
                        if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_PERIOD_DATE"] != System.DBNull.Value)
                        {
                            if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["RUN_IND"].ToString() == "Y")
                            {
                                strRecordLocked = "Y";
                                dgvEmployeeDataGridView[pvtintRunColDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = PayrollLinkDataGridViewCellStyle;
                            }
                            else
                            {
                                pvtAuthorisedDataView = null;
                                pvtAuthorisedDataView = new DataView(this.pvtDataSet.Tables["Authorised"],
                                    pvtstrPayCategoryFilter + " AND EMPLOYEE_NO = " + pvtEmployeeOrDateDataView[intRow]["EMPLOYEE_NO"].ToString() + pvtstrDataAndTypeAuthorisedFilter,
                                    "",
                                    DataViewRowState.CurrentRows);

                                if (pvtAuthorisedDataView.Count > 0)
                                {
                                    strRecordLocked = "A";
                                    dgvEmployeeDataGridView[pvtintRunColDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = AuthorisedDataGridViewCellStyle;
                                }
                                else
                                {
                                    dgvEmployeeDataGridView[pvtintRunColDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = PayrollPendingDataGridViewCellStyle;
                                }
                            }
                        }
                    }
                }
                else
                {
                    this.dgvEmployeeDataGridView.Rows.Add("",
                                                          "",
                                                          "",
                                                          "",
                                                          "",
                                                          "",
                                                          Convert.ToDateTime(pvtEmployeeOrDateDataView[intRow]["DAY_DATE"]).ToString("dd MMMM yyyy - dddd"),
                                                          Convert.ToDateTime(pvtEmployeeOrDateDataView[intRow]["DAY_DATE"]).ToString("yyyyMMdd"),
                                                          "",
                                                          "",
                                                          intRow.ToString());

                    if (pvtDateTime == Convert.ToDateTime(pvtEmployeeOrDateDataView[intRow]["DAY_DATE"]))
                    {
                        intEmployeeNoRow = this.dgvEmployeeDataGridView.Rows.Count - 1;
                    }

                    if (Convert.ToInt32(Convert.ToDateTime(pvtEmployeeOrDateDataView[intRow]["DAY_DATE"]).DayOfWeek) == 6
                        | Convert.ToInt32(Convert.ToDateTime(pvtEmployeeOrDateDataView[intRow]["DAY_DATE"]).DayOfWeek) == 0)
                    {
                        this.dgvEmployeeDataGridView.Rows[dgvEmployeeDataGridView.Rows.Count - 1].DefaultCellStyle = WeekEndDataGridViewCellStyle;

                        this.dgvEmployeeDataGridView[pvtintIndicatorColDataGridView, dgvEmployeeDataGridView.Rows.Count - 1].Style = NormalDataGridViewCellStyle;
                        this.dgvEmployeeDataGridView[pvtintRunColDataGridView, dgvEmployeeDataGridView.Rows.Count - 1].Style = NormalDataGridViewCellStyle;
                        this.dgvEmployeeDataGridView[pvtintBreakExceptionColDataGridView, dgvEmployeeDataGridView.Rows.Count - 1].Style = NormalDataGridViewCellStyle;
                        this.dgvEmployeeDataGridView[pvtintPublicHolidayColDataGridView, dgvEmployeeDataGridView.Rows.Count - 1].Style = NormalDataGridViewCellStyle;
                        this.dgvEmployeeDataGridView[pvtintOnLeaveColDataGridView, dgvEmployeeDataGridView.Rows.Count - 1].Style = NormalDataGridViewCellStyle;
                        this.dgvEmployeeDataGridView[pvtintExcludedFromRunColDataGridView, dgvEmployeeDataGridView.Rows.Count - 1].Style = NormalDataGridViewCellStyle;
                    }

                    if (this.rbnGreaterWageDate.Checked == false)
                    {
                        if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_PERIOD_DATE"] != System.DBNull.Value)
                        {
                            if (Convert.ToDateTime(pvtEmployeeOrDateDataView[intRow]["DAY_DATE"]) <= Convert.ToDateTime(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_PERIOD_DATE"]))
                            {
                                if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["RUN_IND"].ToString() == "Y")
                                {
                                    strRecordLocked = "Y";
                                    dgvEmployeeDataGridView[pvtintRunColDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = PayrollLinkDataGridViewCellStyle;
                                }
                                else
                                {
                                    pvtAuthorisedDataView = null;
                                    pvtAuthorisedDataView = new DataView(this.pvtDataSet.Tables["Authorised"],
                                        pvtstrPayCategoryFilter + pvtstrDataAndTypeAuthorisedFilter,
                                        "",
                                        DataViewRowState.CurrentRows);

                                    if (pvtAuthorisedDataView.Count > 0)
                                    {
                                        strRecordLocked = "A";
                                        dgvEmployeeDataGridView[pvtintRunColDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = AuthorisedDataGridViewCellStyle;
                                    }
                                    else
                                    {
                                        dgvEmployeeDataGridView[pvtintRunColDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = PayrollPendingDataGridViewCellStyle;
                                    }
                                }
                            }
                        }
                    }
                }
                
                if (pvtTempDataView.Count > 0)
                {
                    if (this.rbnBlank.Checked == true)
                    {
                        this.dgvEmployeeDataGridView[pvtintIndicatorColDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = this.NoRecordDataGridViewCellStyle;
                    }

                    if (pvtTempDataView[0]["INDICATOR"].ToString() == "X")
                    {
                        this.dgvEmployeeDataGridView[pvtintIndicatorColDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = ErrorDataGridViewCellStyle;
                    }
                    else
                    {
                        if (pvtTempDataView[0]["INDICATOR"].ToString() == "E")
                        {
                            this.dgvEmployeeDataGridView[pvtintIndicatorColDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = ExceptionDataGridViewCellStyle;
                        }
                        else
                        {
                            if (pvtTempDataView[0]["INDICATOR"].ToString() == "B")
                            {
                                this.dgvEmployeeDataGridView[pvtintIndicatorColDataGridView ,this.dgvEmployeeDataGridView.Rows.Count - 1].Style = NoRecordDataGridViewCellStyle;
                            }
                        }
                    }
                    
                    pvtTempDataView.Sort = "BREAK_INDICATOR DESC";

                    if (pvtTempDataView[0]["BREAK_INDICATOR"].ToString() == "Y")
                    {
                        dgvEmployeeDataGridView[pvtintBreakExceptionColDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = BreakExceptionDataGridViewCellStyle;
                    }

                    pvtTempDataView.Sort = "PAID_HOLIDAY_INDICATOR DESC";

                    if (pvtTempDataView[0]["PAID_HOLIDAY_INDICATOR"].ToString() == "Y")
                    {
                        dgvEmployeeDataGridView[pvtintPublicHolidayColDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = PublicHolidayDataGridViewCellStyle;
                    }
                    
                    pvtTempDataView.Sort = "LEAVE_INDICATOR DESC";

                    if (pvtTempDataView[0]["LEAVE_INDICATOR"].ToString() == "Y")
                    {
                        dgvEmployeeDataGridView[pvtintOnLeaveColDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = this.LeaveDataGridViewCellStyle;
                    }
                    else
                    {
                        if (this.rbnNone.Checked == true)
                        {
                            if (pvtTempBlankDataView.Count > 0)
                            {
                                if (pvtTempBlankDataView[0]["LEAVE_INDICATOR"].ToString() == "Y")
                                {
                                    dgvEmployeeDataGridView[pvtintOnLeaveColDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = this.LeaveDataGridViewCellStyle;
                                }
                            }
                        }
                    }
                    
                    pvtTempDataView.RowFilter += " AND INCLUDED_IN_RUN_INDICATOR = 'N'";

                    if (pvtTempDataView.Count > 0)
                    {
                        this.dgvEmployeeDataGridView[pvtintExcludedFromRunColDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = this.NotIncludedInRunDataGridViewCellStyle;
                    }
                }
                else
                {
                    this.dgvEmployeeDataGridView[pvtintIndicatorColDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = this.NoRecordDataGridViewCellStyle;
                                     
                    if (pvtTempBlankDataView.Count > 0)
                    {
                        if (pvtTempBlankDataView[0]["LEAVE_INDICATOR"].ToString() == "Y")
                        {
                            dgvEmployeeDataGridView[pvtintOnLeaveColDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = this.LeaveDataGridViewCellStyle;
                        }

                        pvtTempBlankDataView.Sort = "PAID_HOLIDAY_INDICATOR DESC";

                        if (pvtTempBlankDataView[0]["PAID_HOLIDAY_INDICATOR"].ToString() == "Y")
                        {
                            dgvEmployeeDataGridView[pvtintPublicHolidayColDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = PublicHolidayDataGridViewCellStyle;
                        }
                    }
                }
            }

            this.pvtblnEmployeeDataGridViewLoaded = true;

            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                if (this.rbnDateWageRun.Checked == false
                && this.rbnExcludedFromRun.Checked == false)
                {
                    this.btnDelete.Enabled = true;
                }

                this.btnUpdate.Enabled = true;
                this.btnRefresh.Enabled = true;

                this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, intEmployeeNoRow);
            }
            else
            {
                //Clear Totals
                this.dgvTimeSheetTotalsDataGridView[1, 0].Value = "0.00";
                this.dgvTimeSheetTotalsDataGridView[1, 1].Value = "0.00";
                this.dgvTimeSheetTotalsDataGridView[1, 2].Value = "0.00";
                this.dgvTimeSheetTotalsDataGridView[1, 3].Value = "0.00";

                this.btnDelete.Enabled = false;
                this.btnUpdate.Enabled = false;
                this.btnRefresh.Enabled = false;
            }
        }
      
        private void Load_Day_SpreadSheet()
        {
            this.Clear_DataGridView(this.dgvDayDataGridView);

            Clear_DataGridView(dgvDayDataGridView);
            Clear_DataGridView(dgvTimeSheetDataGridView);
            Clear_DataGridView(dgvBreakDataGridView);

            //Set To Choose First Row In Spreadsheet
            int intSelectedDayRow = 0;
            int intBelowHH = 0;
            int intBelowMM = 0;
            int intAboveHH = 0;
            int intAboveMM = 0;

            this.pvtblnDayDataGridViewLoaded = false;
            pvtintDayDataGridViewRowIndex = -1;
            
            //Load Employees
            int intEmployeeNo = -1;
            int intTempDateViewIndex = -1;
            int intTempLeaveDateViewIndex = -1;

            string strExceptionCol = "";
            string strRecordLocked = "";
            string strCol5 = "";
            string strCol6 = "";
            string strCol7 = "";
            string strCol8 = "";

            if (this.rbnEmployeeDate.Checked == true)
            {
                if (this.rbnOnLeave.Checked == true
                || this.rbnBlank.Checked == true)
                {
                    pvtTempDataView = null;
                    pvtTempDataView = new DataView(this.pvtDataSet.Tables["DayBlank"],
                            pvtstrPayCategoryFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + pvtstrDataAndTypeFilter + pvtstrRemoveDataFilter + pvtstrBlankFilter,
                            "DAY_DATE",
                            DataViewRowState.CurrentRows);
                }
                else
                {
                    pvtTempDataView = null;
                    pvtTempDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                            pvtstrPayCategoryFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + pvtstrDataAndTypeFilter + pvtstrRemoveDataFilter + pvtstrCategoryType,
                            "DAY_DATE",
                            DataViewRowState.CurrentRows);
                    
                    pvtTempBlankDataView = null;
                    pvtTempBlankDataView = new DataView(this.pvtDataSet.Tables["DayBlank"],
                    pvtstrPayCategoryFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + pvtstrDataAndTypeFilter + pvtstrRemoveDataFilter,
                    "DAY_DATE",
                        DataViewRowState.CurrentRows);
                }
            }
            else
            {
                if (this.rbnOnLeave.Checked == true
                || this.rbnBlank.Checked == true)
                {
                    pvtTempDataView = null;
                    pvtTempDataView = new DataView(this.pvtDataSet.Tables["DayBlank"],
                    pvtstrPayCategoryFilter + " AND DAY_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'" + pvtstrDataAndTypeFilter + pvtstrRemoveDataFilter + pvtstrBlankFilter,
                    "EMPLOYEE_NO",
                        DataViewRowState.CurrentRows);
                }
                else
                {
                    pvtTempDataView = null;
                    pvtTempDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                            pvtstrPayCategoryFilter + " AND DAY_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'" + pvtstrDataAndTypeFilter + pvtstrRemoveDataFilter + pvtstrCategoryType,
                            "EMPLOYEE_NO",
                            DataViewRowState.CurrentRows);
                
                    pvtTempBlankDataView = null;
                    pvtTempBlankDataView = new DataView(this.pvtDataSet.Tables["DayBlank"],
                    pvtstrPayCategoryFilter + " AND DAY_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'" + pvtstrDataAndTypeFilter + pvtstrRemoveDataFilter,
                    "EMPLOYEE_NO",
                        DataViewRowState.CurrentRows);
                }
            }

            for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
            {
                strRecordLocked = "";
                strExceptionCol = "";

                if (this.rbnEmployeeDate.Checked == true)
                {
                    intTempDateViewIndex = pvtTempDataView.Find(Convert.ToDateTime(pvtEmployeeDataView[intRow]["DAY_DATE"]).ToString("yyyy-MM-dd"));
                    
                    if (this.rbnNone.Checked == true)
                    {
                        intTempLeaveDateViewIndex = pvtTempBlankDataView.Find(Convert.ToDateTime(pvtEmployeeDataView[intRow]["DAY_DATE"]).ToString("yyyy-MM-dd"));
                    }
                }
                else
                {
                    intEmployeeNo = Convert.ToInt32(pvtEmployeeDataView[intRow]["EMPLOYEE_NO"]);
#if (DEBUG)
                    if (intEmployeeNo == 354)
                    {
                        string strStop = "";
                    }
#endif
                    intTempDateViewIndex = pvtTempDataView.Find(intEmployeeNo);

                    if (this.rbnNone.Checked == true)
                    {
                        intTempLeaveDateViewIndex = pvtTempBlankDataView.Find(intEmployeeNo);
                    }
                }

                if ((this.rbnErrors.Checked == true
                || this.rbnException.Checked == true
                || this.rbnNormal.Checked == true
                || this.rbnBreakException.Checked == true
                || this.rbnPublicHoliday.Checked == true
                || this.rbnExcludedFromRun.Checked == true
                || this.rbnBlank.Checked == true
                || this.rbnOnLeave.Checked == true
                || this.chkRemoveBlanks.Checked == true)
                && intTempDateViewIndex == -1)
                {
                    continue;
                }

                if (this.rbnEmployeeDate.Checked == true)
                {
                    if (Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + Convert.ToDateTime(pvtEmployeeDataView[intRow]["DAY_DATE"]).ToString("ddd").ToUpper() + "_BELOW_MINUTES"]) == 0)
                    {
                        strExceptionCol = "> 0";
                    }
                    else
                    {
                        intBelowHH = Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + Convert.ToDateTime(pvtEmployeeDataView[intRow]["DAY_DATE"]).ToString("ddd").ToUpper() + "_BELOW_MINUTES"]) / 60;
                        intBelowMM = Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + Convert.ToDateTime(pvtEmployeeDataView[intRow]["DAY_DATE"]).ToString("ddd").ToUpper() + "_BELOW_MINUTES"]) % 60;
                        intAboveHH = Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + Convert.ToDateTime(pvtEmployeeDataView[intRow]["DAY_DATE"]).ToString("ddd").ToUpper() + "_ABOVE_MINUTES"]) / 60;
                        intAboveMM = Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + Convert.ToDateTime(pvtEmployeeDataView[intRow]["DAY_DATE"]).ToString("ddd").ToUpper() + "_ABOVE_MINUTES"]) % 60;

                        strExceptionCol = intBelowHH.ToString() + ":" + intBelowMM.ToString("00") + " - " + intAboveHH.ToString() + ":" + intAboveMM.ToString("00");
                    }

                    if (intTempDateViewIndex == -1)
                    {
                        strCol5 = "0:00";
                        strCol6 = "0:00";
                    }
                    else
                    {
                        strCol5 = Convert.ToInt32(Convert.ToInt32(pvtTempDataView[intTempDateViewIndex]["DAY_PAID_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtTempDataView[intTempDateViewIndex]["DAY_PAID_MINUTES"]) % 60).ToString("00");
                        strCol6 = Convert.ToInt32(Convert.ToInt32(pvtTempDataView[intTempDateViewIndex]["BREAK_ACCUM_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtTempDataView[intTempDateViewIndex]["BREAK_ACCUM_MINUTES"]) % 60).ToString("00");
                    }

                    if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_PERIOD_DATE"] != System.DBNull.Value)
                    {
                        if (Convert.ToDateTime(pvtEmployeeDataView[intRow]["DAY_DATE"]) <= Convert.ToDateTime(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_PERIOD_DATE"]))
                        {
                            if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["RUN_IND"].ToString() == "Y")
                            {
                                strRecordLocked = "Y";
                            }
                            else
                            {
                                pvtAuthorisedDataView = null;
                                pvtAuthorisedDataView = new DataView(this.pvtDataSet.Tables["Authorised"],
                                        pvtstrPayCategoryFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo,
                                        "",
                                        DataViewRowState.CurrentRows);
                                
                                if (pvtAuthorisedDataView.Count > 0)
                                {
                                    strRecordLocked = "A";
                                }
                                else
                                {
                                    strRecordLocked = "P";
                                }
                            }
                        }
                    }
                   
                    this.dgvDayDataGridView.Rows.Add("",
                                                     "",
                                                     "",
                                                     "",
                                                     "",
                                                     "",
                                                     Convert.ToDateTime(pvtEmployeeDataView[intRow]["DAY_DATE"]).ToString("dd MMMM yyyy - dddd"),
                                                     strExceptionCol,
                                                     strCol5,
                                                     strCol6,
                                                     "",
                                                     "",
                                                     "",
                                                     strRecordLocked,
                                                     Convert.ToDateTime(pvtEmployeeDataView[intRow]["DAY_DATE"]).ToString("yyyy-MM-dd"));

                    //Weekend
                    if (Convert.ToInt32(pvtEmployeeDataView[intRow]["DAY_NO"]) == 6
                    || Convert.ToInt32(pvtEmployeeDataView[intRow]["DAY_NO"]) == 0)
                    {
                        this.dgvDayDataGridView.Rows[dgvDayDataGridView.Rows.Count - 1].DefaultCellStyle = WeekEndDataGridViewCellStyle;

                        this.dgvDayDataGridView[pvtintIndicatorColDataGridView, dgvDayDataGridView.Rows.Count - 1].Style = NormalDataGridViewCellStyle;
                        this.dgvDayDataGridView[pvtintRunColDataGridView, dgvDayDataGridView.Rows.Count - 1].Style = NormalDataGridViewCellStyle;
                        this.dgvDayDataGridView[pvtintBreakExceptionColDataGridView, dgvDayDataGridView.Rows.Count - 1].Style = NormalDataGridViewCellStyle;
                        this.dgvDayDataGridView[pvtintPublicHolidayColDataGridView, dgvDayDataGridView.Rows.Count - 1].Style = NormalDataGridViewCellStyle;
                        this.dgvDayDataGridView[pvtintOnLeaveColDataGridView, dgvDayDataGridView.Rows.Count - 1].Style = NormalDataGridViewCellStyle;
                        this.dgvDayDataGridView[pvtintExcludedFromRunColDataGridView, dgvDayDataGridView.Rows.Count - 1].Style = NormalDataGridViewCellStyle;
                    }
                }
                else
                {
                    if (Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + pvtDateTime.ToString("ddd").ToUpper() + "_BELOW_MINUTES"]) == 0)
                    {
                        strCol6 = "> 0";
                    }
                    else
                    {
                        intBelowHH = Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + pvtDateTime.ToString("ddd").ToUpper() + "_BELOW_MINUTES"]) / 60;
                        intBelowMM = Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + pvtDateTime.ToString("ddd").ToUpper() + "_BELOW_MINUTES"]) % 60;
                        intAboveHH = Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + pvtDateTime.ToString("ddd").ToUpper() + "_ABOVE_MINUTES"]) / 60;
                        intAboveMM = Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + pvtDateTime.ToString("ddd").ToUpper() + "_ABOVE_MINUTES"]) % 60;

                        strCol6 = intBelowHH.ToString() + ":" + intBelowMM.ToString("00") + " - " + intAboveHH.ToString() + ":" + intAboveMM.ToString("00");
                    }

                    if (intTempDateViewIndex == -1)
                    {
                        strCol7 = "0:00";
                        strCol8 = "0:00";
                    }
                    else
                    {
                        strCol7 = Convert.ToInt32(Convert.ToInt32(pvtTempDataView[intTempDateViewIndex]["DAY_PAID_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtTempDataView[intTempDateViewIndex]["DAY_PAID_MINUTES"]) % 60).ToString("00");
                        strCol8 = Convert.ToInt32(Convert.ToInt32(pvtTempDataView[intTempDateViewIndex]["BREAK_ACCUM_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtTempDataView[intTempDateViewIndex]["BREAK_ACCUM_MINUTES"]) % 60).ToString("00");
                    }

                    if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_PERIOD_DATE"] != System.DBNull.Value)
                    {
                        if (this.pvtDateTime <= Convert.ToDateTime(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_PERIOD_DATE"]))
                        {
                            if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["RUN_IND"].ToString() == "Y")
                            {
                                strRecordLocked = "Y";
                            }
                            else
                            {
                                pvtAuthorisedDataView = null;
                                pvtAuthorisedDataView = new DataView(this.pvtDataSet.Tables["Authorised"],
                                        pvtstrPayCategoryFilter + " AND EMPLOYEE_NO = " + pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString(),
                                        "",
                                        DataViewRowState.CurrentRows);

                                if (pvtAuthorisedDataView.Count > 0)
                                {
                                    strRecordLocked = "A";
                                }
                                else
                                {
                                    strRecordLocked = "P";
                                }
                            }
                        }
                    }

                    this.dgvDayDataGridView.Rows.Add("",
                                                     "",
                                                     "",
                                                     "",
                                                     "",
                                                     "",
                                                     pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString(),
                                                     pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                     pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                     pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                     strCol6,
                                                     strCol7,
                                                     strCol8,
                                                     strRecordLocked,
                                                     intRow.ToString());
                }

                if (intTempDateViewIndex == -1)
                {   
                    this.dgvDayDataGridView[pvtintIndicatorColDataGridView, this.dgvDayDataGridView.Rows.Count - 1].Style = NoRecordDataGridViewCellStyle;
                    
                    if (pvtTempBlankDataView[intTempLeaveDateViewIndex]["PAID_HOLIDAY_INDICATOR"].ToString() == "Y")
                    {
                        dgvDayDataGridView[pvtintPublicHolidayColDataGridView, this.dgvDayDataGridView.Rows.Count - 1].Style = PublicHolidayDataGridViewCellStyle;
                    }

                    if (pvtTempBlankDataView[intTempLeaveDateViewIndex]["LEAVE_INDICATOR"].ToString() == "Y")
                    {
                        this.dgvDayDataGridView[pvtintOnLeaveColDataGridView, this.dgvDayDataGridView.Rows.Count - 1].Style = this.LeaveDataGridViewCellStyle;
                    }
                }
                else
                {
                    if (this.rbnBlank.Checked == true
                    || this.rbnOnLeave.Checked == true)
                    {
                        this.dgvDayDataGridView[pvtintIndicatorColDataGridView, this.dgvDayDataGridView.Rows.Count - 1].Style = NoRecordDataGridViewCellStyle;
                    }
                    
                    if (pvtTempDataView[intTempDateViewIndex]["INDICATOR"].ToString() == "X")
                    {
                        this.dgvDayDataGridView[pvtintIndicatorColDataGridView, this.dgvDayDataGridView.Rows.Count - 1].Style = ErrorDataGridViewCellStyle;
                    }
                    else
                    {
                        if (pvtTempDataView[intTempDateViewIndex]["INDICATOR"].ToString() == "E")
                        {
                            this.dgvDayDataGridView[pvtintIndicatorColDataGridView, this.dgvDayDataGridView.Rows.Count - 1].Style = ExceptionDataGridViewCellStyle;
                        }
                    }

                    if (pvtTempDataView[intTempDateViewIndex]["BREAK_INDICATOR"].ToString() == "Y")
                    {
                        dgvDayDataGridView[pvtintBreakExceptionColDataGridView, this.dgvDayDataGridView.Rows.Count - 1].Style = BreakExceptionDataGridViewCellStyle;
                    }

                    if (pvtTempDataView[intTempDateViewIndex]["PAID_HOLIDAY_INDICATOR"].ToString() == "Y")
                    {
                        dgvDayDataGridView[pvtintPublicHolidayColDataGridView, this.dgvDayDataGridView.Rows.Count - 1].Style = PublicHolidayDataGridViewCellStyle;
                    }
                    
                    //2017-09-08
                    if (pvtTempDataView[intTempDateViewIndex]["LEAVE_INDICATOR"].ToString() == "Y")
                    {
                        this.dgvDayDataGridView[pvtintOnLeaveColDataGridView, this.dgvDayDataGridView.Rows.Count - 1].Style = this.LeaveDataGridViewCellStyle;
                    }

                    //ELR - 20150516
                    if (pvtTempDataView[intTempDateViewIndex]["INCLUDED_IN_RUN_INDICATOR"].ToString() == "N")
                    {
                        this.dgvDayDataGridView[pvtintExcludedFromRunColDataGridView, this.dgvDayDataGridView.Rows.Count - 1].Style = this.NotIncludedInRunDataGridViewCellStyle;
                    }
                }
                
                if (strRecordLocked == "Y")
                {
                    dgvDayDataGridView[pvtintRunColDataGridView, this.dgvDayDataGridView.Rows.Count - 1].Style = PayrollLinkDataGridViewCellStyle;
                }
                else
                {
                    if (strRecordLocked == "A")
                    {
                        dgvDayDataGridView[pvtintRunColDataGridView, this.dgvDayDataGridView.Rows.Count - 1].Style = AuthorisedDataGridViewCellStyle;
                    }
                    else
                    {
                        if (strRecordLocked == "P")
                        {
                            dgvDayDataGridView[pvtintRunColDataGridView, this.dgvDayDataGridView.Rows.Count - 1].Style = PayrollPendingDataGridViewCellStyle;
                        }
                    }
                }

                if (this.rbnEmployeeDate.Checked == true)
                {
                    if (Convert.ToDateTime(pvtEmployeeDataView[intRow]["DAY_DATE"]) == pvtDateTime)
                    {
                        intSelectedDayRow = this.dgvDayDataGridView.Rows.Count - 1;
                    }
                }
                else
                {
                    if (intEmployeeNo == pvtintEmployeeNo)
                    {
                        intSelectedDayRow = this.dgvDayDataGridView.Rows.Count - 1;
                    }
                }
            }

            this.pvtblnDayDataGridViewLoaded = true;
          
            if (this.dgvDayDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvDayDataGridView, intSelectedDayRow);
            }
            else
            {
                //Clear Totals
                this.dgvTimeSheetTotalsDataGridView[1, 0].Value = "0.00";
                this.dgvTimeSheetTotalsDataGridView[1, 1].Value = "0.00";
                this.dgvTimeSheetTotalsDataGridView[1, 2].Value = "0.00";
                this.dgvTimeSheetTotalsDataGridView[1, 3].Value = "0.00";

                this.lblDayDesc.Text = "";

                this.btnUpdate.Enabled = false;
                this.btnRefresh.Enabled = false;
            }
        }
        
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            this.Text += " - Update";

            this.dgvTimeSheetDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dgvTimeSheetDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;
            
            this.dgvBreakDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dgvBreakDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;

            this.btnDeleteRow.Enabled = true;
            this.btnDeleteBreakRow.Enabled = true;

            this.dgvPayCategoryDataGridView.Enabled = false;
            this.dgvEmployeeDataGridView.Enabled = false;
            this.dgvPayrollTypeDataGridView.Enabled = false;

            Show_Update_Lock_Images();
            
            this.chkRemoveBlanks.Enabled = false;
            this.chkRemoveSat.Enabled = false;
            this.chkRemoveSun.Enabled = false;
            this.chkRemovePublicHolidays.Enabled = false;
            this.chkRemoveOnLeave.Enabled = false;
            this.chkEndDateOnly.Enabled = false;
            
            this.btnRefresh.Enabled = false;

            this.cboDateFilter.Enabled = false;

            this.rbnErrors.Enabled = false;
            this.rbnException.Enabled = false;
            this.rbnNone.Enabled = false;
            this.rbnBlank.Enabled = false;
            this.rbnNormal.Enabled = false;

            this.rbnBreakException.Enabled = false;
            this.rbnPublicHoliday.Enabled = false;
            this.rbnExcludedFromRun.Enabled = false;
            this.rbnOnLeave.Enabled = false;
            
            this.rbnDelCostCentre.Enabled = false;
            this.rbnDelEmployee.Enabled = false;

            this.rbnDateNormal.Enabled = false;
            this.rbnDateWageRun.Enabled = false;
            this.rbnGreaterWageDate.Enabled = false;

            this.btnUpdate.Enabled = false;
            this.btnDelete.Enabled = false;
            
            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.rbnDateEmployee.Enabled = false;
            this.rbnEmployeeDate.Enabled = false;

            if (this.dgvDayDataGridView.Rows.Count > 0)
            {
                //So that Event Fires
                pvtintDayDataGridViewRowIndex = -1;
                this.Set_DataGridView_SelectedRowIndex(this.dgvDayDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvDayDataGridView));
            }
        }

        private void rbnNone_Click(object sender, EventArgs e)
        {
            this.chkRemoveBlanks.Enabled = true;

            this.chkRemoveSat.Checked = false;
            this.chkRemoveSat.Enabled = false;
            this.chkRemoveSun.Checked = false;
            this.chkRemoveSun.Enabled = false;
            this.chkRemovePublicHolidays.Checked = false;
            this.chkRemovePublicHolidays.Enabled = false;
            this.chkRemoveOnLeave.Checked = false;
            this.chkRemoveOnLeave.Enabled = false;

            if (pvtblnBusyLoading == false)
            {
                Load_PayCategory_Records();
            }
        }

        private void Show_Update_Lock_Images()
        {
            this.picPayrollTypeLock.Visible = true;
            this.picPayCategoryLock.Visible = true;
            this.picEmployeeLock.Visible = true;
        }

        private void Hide_Update_Lock_Images()
        {
            this.picPayrollTypeLock.Visible = false;
            this.picPayCategoryLock.Visible = false;
            this.picEmployeeLock.Visible = false;
        }

        private void GeneralFilter_Click(object sender, EventArgs e)
        {
            this.chkRemoveBlanks.Checked = false;
            this.chkRemoveBlanks.Enabled = false;

            this.chkRemoveSat.Checked = false;
            this.chkRemoveSat.Enabled = false;
            this.chkRemoveSun.Checked = false;
            this.chkRemoveSun.Enabled = false;
            this.chkRemovePublicHolidays.Checked = false;
            this.chkRemovePublicHolidays.Enabled = false;
            this.chkRemoveOnLeave.Checked = false;
            this.chkRemoveOnLeave.Enabled = false;
            
            if (pvtblnBusyLoading == false)
            {
                Load_PayCategory_Records();
            }
        }

        private void rbnBlank_Click(object sender, EventArgs e)
        {
            this.chkRemoveBlanks.Checked = false;
            this.chkRemoveBlanks.Enabled = false;

            if (this.chkEndDateOnly.Checked == false)
            {
                this.chkRemoveSat.Enabled = true;
                this.chkRemoveSun.Enabled = true;
                this.chkRemovePublicHolidays.Enabled = true;
                this.chkRemoveOnLeave.Enabled = true;
            }

            if (pvtblnBusyLoading == false)
            {
                Load_PayCategory_Records();
            }
        }

        private void chkRemoveBlanks_Click(object sender, EventArgs e)
        {
            if (this.rbnNone.Checked == true)
            {
                Load_PayCategory_Records();
            }
        }
        
       
        private void Remove_Saturday_Sunday_Leave_Click(object sender, EventArgs e)
        {
            Load_PayCategory_Records();
        }

        private void Cell_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox myTextBox = (TextBox)sender;

            //NB When an Invalid Time Passes this Test eg 70 then it will be caught in The CellChanged Event
            //NB When an Invalid Time Passes this Test eg 70 then it will be caught in The CellChanged Event

            if (e.KeyChar == (char)8
                | (e.KeyChar > (char)47
                & e.KeyChar < (char)58))
            {
                if (e.KeyChar != (char)8
                    & myTextBox.Text.Length == 4)
                {
                    if (myTextBox.SelectionLength == 0)
                    {
                        System.Console.Beep();
                        e.Handled = true;
                    }
                }
                else
                {
                    //Build TextBox Field
                    if (myTextBox.Text != "")
                    {
                        string strNewTextField = "";
                        int intTimeMM = 0;
                        int intTimeHH = 0;

                        if (myTextBox.SelectionLength == myTextBox.Text.Length)
                        {
                            //Replace All Characters with 1 New Numeric Character 
                        }
                        else
                        {
                            if (myTextBox.SelectionLength > 0)
                            {
                                //Replace 
                                strNewTextField = myTextBox.Text.Remove(myTextBox.SelectionStart, myTextBox.SelectionLength);

                                if (myTextBox.SelectionStart == 0)
                                {
                                    strNewTextField = e.KeyChar + strNewTextField;
                                }
                                else
                                {

                                    if (myTextBox.SelectionStart == strNewTextField.Length)
                                    {
                                        //Add to End
                                        strNewTextField = strNewTextField + e.KeyChar;
                                    }
                                    else
                                    {
                                        //Middle
                                        strNewTextField = strNewTextField.Substring(0, myTextBox.SelectionStart) + e.KeyChar + strNewTextField.Substring(myTextBox.SelectionStart);
                                    }
                                }
                            }
                            else
                            {
                                if (myTextBox.Text.Length == myTextBox.SelectionStart)
                                {
                                    if (e.KeyChar == 8)
                                    {
                                        strNewTextField = myTextBox.Text.Substring(0, myTextBox.Text.Length - 1);
                                    }
                                    else
                                    {
                                        strNewTextField = myTextBox.Text + e.KeyChar;
                                    }
                                }
                                else
                                {
                                    if (e.KeyChar == 8)
                                    {
                                        strNewTextField = myTextBox.Text.Substring(0, myTextBox.SelectionStart - 1) + myTextBox.Text.Substring(myTextBox.SelectionStart);
                                    }
                                    else
                                    {
                                        strNewTextField = myTextBox.Text.Substring(0, myTextBox.SelectionStart) + e.KeyChar + myTextBox.Text.Substring(myTextBox.SelectionStart);
                                    }
                                }
                            }
                        }

                        if (strNewTextField != "")
                        {
                            //Check that Time is in Correct Format
                            if (strNewTextField.Length == 2
                                & strNewTextField.Substring(0, 1) == "0")
                            {
                                System.Console.Beep();
                                e.Handled = true;
                            }
                            else
                            {

                                if (strNewTextField.Length >= 3)
                                {
                                    intTimeMM = Convert.ToInt32(strNewTextField.Substring(strNewTextField.Length - 2, 2));
                                }
                                else
                                {
                                    intTimeMM = Convert.ToInt32(strNewTextField);
                                }

                                if (intTimeMM > 59)
                                {
                                    if (strNewTextField.Length > 3
                                        & myTextBox.SelectionLength == 0)
                                    {
                                        System.Console.Beep();
                                        e.Handled = true;
                                    }
                                    else
                                    {
                                        if (strNewTextField.Length == 3)
                                        {
                                            if (Convert.ToInt32(strNewTextField.Substring(2, 1)) > 5)
                                            {
                                                System.Console.Beep();
                                                e.Handled = true;
                                            }
                                            else
                                            {
                                                if (Convert.ToInt32(strNewTextField) > 259)
                                                {
                                                    System.Console.Beep();
                                                    e.Handled = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (Convert.ToInt32(strNewTextField.Substring(1, 1)) > 5)
                                            {
                                                System.Console.Beep();
                                                e.Handled = true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (strNewTextField.Length >= 3)
                                    {
                                        intTimeHH = Convert.ToInt32(strNewTextField.Substring(0, strNewTextField.Length - 2));

                                        if (intTimeHH > 24)
                                        {
                                            System.Console.Beep();
                                            e.Handled = true;
                                        }
                                        else
                                        {
                                            if (Convert.ToInt32(strNewTextField) > 2400)
                                            {
                                                System.Console.Beep();
                                                e.Handled = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (e.KeyChar == (char)13)
                {
                }
                else
                {
                    System.Console.Beep();
                    e.Handled = true;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (this.Text.LastIndexOf("- Update") != -1)
            {
                this.Text = this.Text.Substring(0, this.Text.IndexOf("- Update") - 1);
            }

            this.rbnErrors.Enabled = true;
            this.rbnException.Enabled = true;
            this.rbnNone.Enabled = true;
            this.rbnBlank.Enabled = true;
            this.rbnNormal.Enabled = true;
            this.rbnBreakException.Enabled = true;
            this.rbnOnLeave.Enabled = true;
            this.rbnPublicHoliday.Enabled = true;
            this.rbnExcludedFromRun.Enabled = true;
            this.chkEndDateOnly.Enabled = true;

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            Hide_Update_Lock_Images();

            this.dgvPayCategoryDataGridView.Enabled = true;
            this.dgvEmployeeDataGridView.Enabled = true;
            this.dgvPayrollTypeDataGridView.Enabled = true;
            
            this.btnRefresh.Enabled = true;

            this.cboDateFilter.Enabled = true;

            if (this.rbnNone.Checked == true)
            {
                this.chkRemoveBlanks.Enabled = true;
            }
            else
            {
                if (this.rbnBlank.Checked == true
                && this.chkEndDateOnly.Checked == false)
                {
                    this.chkRemoveSat.Enabled = true;
                    this.chkRemoveSun.Enabled = true;
                    this.chkRemovePublicHolidays.Enabled = true;
                    this.chkRemoveOnLeave.Enabled = true;
                }
            }
                        
            this.rbnDelCostCentre.Enabled = true;
            this.rbnDelEmployee.Enabled = true;

            this.rbnDateEmployee.Enabled = true;
            this.rbnEmployeeDate.Enabled = true;
            
            this.dgvTimeSheetDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvTimeSheetDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            this.dgvBreakDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvBreakDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

            this.btnDeleteRow.Enabled = false;
            this.btnDeleteBreakRow.Enabled = false;

            pvtDataSet.RejectChanges();

            Load_PayCategory_Records();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string strDeleteType = "C";
            string strPeriodOption = "";
            DialogResult dlgResult;
            string strDateFilter = "";
            string strHeader = "";
            string strWageRunExclude = "";

            if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_PERIOD_DATE"] != System.DBNull.Value)
            {
                if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["RUN_IND"].ToString() == "Y")
                {
                    strWageRunExclude = "Y";
                }
                else
                {
                    pvtAuthorisedDataView = null;
                    pvtAuthorisedDataView = new DataView(this.pvtDataSet.Tables["Authorised"],
                        pvtstrPayCategoryFilter,
                        "",
                        DataViewRowState.CurrentRows);

                    if (pvtAuthorisedDataView.Count > 0)
                    {
                        strWageRunExclude = "A";
                    }
                }
            }
            
            if (this.rbnDelCostCentre.Checked == true)
            {
                strHeader = "Delete Timesheets / Breaks for Cost Centre\n\n" + pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_CATEGORY_DESC"].ToString();
            }
            else
            {
                strDeleteType = "E";
                strHeader = "Delete Timesheets / Breaks for Employee\n\n";

                if (this.rbnDateEmployee.Checked == true)
                {
                    //ELR - 2015-03-20
                    strHeader += this.dgvDayDataGridView[pvtintNameColDayDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvDayDataGridView)].Value.ToString() + " " + this.dgvDayDataGridView[pvtintSurnameColDayDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvDayDataGridView)].Value.ToString();
                }
                else
                {
                    //ELR - 2015-03-20
                    strHeader += this.dgvEmployeeDataGridView[pvtintNameColEmployeeDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView)].Value.ToString() + " " + this.dgvEmployeeDataGridView[pvtintSurnameColEmployeeDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView)].Value.ToString();
                }
            }

            if (rbnGreaterWageDate.Checked == true)
            {
                strPeriodOption = "G";
                strHeader += "\n\n>= " + Convert.ToDateTime(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_PERIOD_DATE"]).AddDays(1).ToString("dd MMMM yyyy") + ".";
                strDateFilter = " AND TIMESHEET_DATE >= '" + Convert.ToDateTime(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_PERIOD_DATE"]).AddDays(1).ToString("yyyy-MM-dd") + "'"; 
            }
            else
            {
                if (rbnDateWageRun.Checked == true)
                {
                    strPeriodOption = "W";
                    strHeader += "\n\nfor Current Wage Run.";
                    strDateFilter = " AND TIMESHEET_DATE <= '" + Convert.ToDateTime(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'"; 
                }
                else
                {
                    strHeader += ".";
                }
            }

            if (strWageRunExclude == "Y")
            {
                strHeader += "\n\nExclude Wage Run Records (Locked - Payroll Run).";
            }
            else
            {
                if (strWageRunExclude == "A")
                {
                    strHeader += "\n\nExclude Wage Run Records (Locked - Authorised).";
                }
            }

            dlgResult = CustomMessageBox.Show(strHeader,
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
            
            if (dlgResult == DialogResult.Yes)
            {
                object[] objParm = new object[9];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[2] = strDeleteType;
                objParm[3] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[4] = pvtintPayCategoryNo;
                objParm[5] = pvtstrPayrollType;
                objParm[6] = this.pvtintEmployeeNo;
                objParm[7] = strPeriodOption;
                objParm[8] = strWageRunExclude;

                clsISUtilities.DynamicFunction("Delete_PayCategory_Employee_TimeSheet_Records", objParm, true);
                    
                //Remove All TimeSheets For Cost Centre
                pvtTempDataView = null;
                pvtTempDataView = new DataView(pvtDataSet.Tables["TimeSheet"],
                        "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' " + strDateFilter,
                    "",
                    DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < pvtTempDataView.Count; intRow++)
                {
                    pvtTempDataView[intRow].Delete();

                    intRow -= 1;
                }

                //Remove All Breaks For Cost Centre
                pvtTempDataView = null;
                pvtTempDataView = new DataView(pvtDataSet.Tables["Break"],
                        "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' " + strDateFilter.Replace("TIMESHEET_DATE", "BREAK_DATE"),
                    "",
                    DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < pvtTempDataView.Count; intRow++)
                {
                    pvtTempDataView[intRow].Delete();

                    intRow -= 1;
                }

                //Remove All DayTotal For Cost Centre
                pvtTempDataView = null;
                pvtTempDataView = new DataView(pvtDataSet.Tables["DayTotal"],
                        "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' " + strDateFilter.Replace("TIMESHEET_DATE", "DAY_DATE"),
                    "",
                    DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < pvtTempDataView.Count; intRow++)
                {
                    pvtTempDataView[intRow].Delete();

                    intRow -= 1;
                }

                this.pvtDataSet.AcceptChanges();

                Load_PayCategory_Records();
            }
        }

        private void rbn_DateFilter_Click(object sender, EventArgs e)
        {
            RadioButton myRadioButton = (RadioButton)sender;

            pvtblnFilterClicked = true;

            this.chkEndDateOnly.Checked = false;

            pvtDataSet.Tables["DateSelection"].Clear();
            this.cboDateFilter.Items.Clear();

            DateTime myStartDateTime = DateTime.Now.AddDays(-15);
            int intComboSelectIndex = 0;

            if (myRadioButton.Name == "rbnDateNormal")
            {
                while (myStartDateTime <= DateTime.Now.AddDays(15))
                {
                    DataRow MyDataRow = pvtDataSet.Tables["DateSelection"].NewRow();

                    MyDataRow["ACTUAL_DATE"] = myStartDateTime.ToString("yyyy-MM-dd");

                    pvtDataSet.Tables["DateSelection"].Rows.Add(MyDataRow);

                    this.cboDateFilter.Items.Add(myStartDateTime.ToString("dd MMM yyyy - dddd"));

                    if (myStartDateTime.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd"))
                    {
                        intComboSelectIndex = this.cboDateFilter.Items.Count - 1;
                    }

                    myStartDateTime = myStartDateTime.AddDays(1);
                }
            }
            else
            {
                if (myRadioButton.Name == "rbnDateWageRun")
                {
                    myStartDateTime = Convert.ToDateTime(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_PERIOD_DATE_FROM"]);

                    while (myStartDateTime <= Convert.ToDateTime(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_PERIOD_DATE"]))
                    {
                        DataRow MyDataRow = pvtDataSet.Tables["DateSelection"].NewRow();

                        MyDataRow["ACTUAL_DATE"] = myStartDateTime.ToString("yyyy-MM-dd");

                        pvtDataSet.Tables["DateSelection"].Rows.Add(MyDataRow);

                        this.cboDateFilter.Items.Add(myStartDateTime.ToString("dd MMM yyyy - dddd"));

                        myStartDateTime = myStartDateTime.AddDays(1);
                    }
                }
                else
                {
                    //Greater Than Wage Date
                    myStartDateTime = Convert.ToDateTime(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_PERIOD_DATE_FROM"]).AddDays(1);

                    while (myStartDateTime <= DateTime.Now.AddDays(15))
                    {
                        DataRow MyDataRow = pvtDataSet.Tables["DateSelection"].NewRow();

                        MyDataRow["ACTUAL_DATE"] = myStartDateTime.ToString("yyyy-MM-dd");

                        pvtDataSet.Tables["DateSelection"].Rows.Add(MyDataRow);

                        this.cboDateFilter.Items.Add(myStartDateTime.ToString("dd MMM yyyy - dddd"));

                        myStartDateTime = myStartDateTime.AddDays(1);
                    }
                }

                intComboSelectIndex = this.cboDateFilter.Items.Count - 1;
            }

            pvtDataSet.Tables["DateSelection"].AcceptChanges();

            if (this.cboDateFilter.Items.Count > 0)
            {
                this.cboDateFilter.SelectedIndex = intComboSelectIndex;
            }

            if (this.dgvPayrollTypeDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView));
            }

            pvtblnFilterClicked = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DataSet TempDataSet = new DataSet();
            TempDataSet.Tables.Add(this.pvtDataSet.Tables["TimeSheet"].Clone());

            //Remove Any New Records with Time In & Time Out = Null
            pvtTempDataView = null;
            pvtTempDataView = new DataView(pvtDataSet.Tables["TimeSheet"],
                "",
                "",
                DataViewRowState.ModifiedCurrent | DataViewRowState.Added | DataViewRowState.Deleted);

            for (int intRow = 0; intRow < pvtTempDataView.Count; intRow++)
            {
                if (pvtTempDataView[intRow].Row.RowState == DataRowState.Deleted)
                {
                    TempDataSet.Tables["TimeSheet"].ImportRow(pvtTempDataView[intRow].Row);
                }
                else
                {
                    if (pvtTempDataView[intRow]["TIMESHEET_TIME_IN_MINUTES"] == System.DBNull.Value
                    & pvtTempDataView[intRow]["TIMESHEET_TIME_OUT_MINUTES"] == System.DBNull.Value)
                    {
                        if (pvtTempDataView[intRow].Row.RowState == DataRowState.Added)
                        {
                            continue;
                        }
                        else
                        {
                            //Delete Row
                            pvtTempDataView[intRow].Delete();
                        }
                    }

                    TempDataSet.Tables["TimeSheet"].ImportRow(pvtTempDataView[intRow].Row);
                }
            }

            TempDataSet.Tables.Add(this.pvtDataSet.Tables["Break"].Clone());

            //Remove Any New Recods with Time In & Time Out = Null
            pvtTempDataView = null;
            pvtTempDataView = new DataView(pvtDataSet.Tables["Break"],
                "",
                "",
                DataViewRowState.ModifiedCurrent | DataViewRowState.Added | DataViewRowState.Deleted);

            for (int intRow = 0; intRow < pvtTempDataView.Count; intRow++)
            {
                if (pvtTempDataView[intRow].Row.RowState == DataRowState.Deleted)
                {
                    TempDataSet.Tables["Break"].ImportRow(pvtTempDataView[intRow].Row);
                }
                else
                {
                    if (pvtTempDataView[intRow]["BREAK_TIME_IN_MINUTES"] == System.DBNull.Value
                    & pvtTempDataView[intRow]["BREAK_TIME_OUT_MINUTES"] == System.DBNull.Value)
                    {
                        if (pvtTempDataView[intRow].Row.RowState == DataRowState.Added)
                        {
                            continue;
                        }
                        else
                        {
                            //Delete Row
                            pvtTempDataView[intRow].Delete();
                        }
                    }

                    TempDataSet.Tables["Break"].ImportRow(pvtTempDataView[intRow].Row);
                }
            }

            //Throw Away Cahnges
            this.pvtDataSet.RejectChanges();

            //Compress DataSet
            pvtbytCompress = clsISUtilities.Compress_DataSet(TempDataSet);

            object[] objParm = new object[8];
            objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
            objParm[1] = pvtstrPayrollType;
            objParm[2] = pvtintPayCategoryNo;

            if (this.rbnDateEmployee.Checked == true)
            {
                objParm[3] = "D";
                objParm[4] = pvtDateTime.ToString("yyyy-MM-dd");
                
                DataView myDataView = new DataView(this.pvtDataSet.Tables["DayBlank"],
                "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND DAY_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'",
                "",
                DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < myDataView.Count; intRow++)
                {
                    myDataView[intRow].Delete();

                    intRow -= 1;
                }

                myDataView = null;
                myDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND DAY_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'",
                    "",
                    DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < myDataView.Count; intRow++)
                {
                    myDataView[intRow].Delete();

                    intRow -= 1;
                }

                myDataView = null;
                myDataView = new DataView(this.pvtDataSet.Tables["TimeSheet"],
                "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND TIMESHEET_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'",
                    "TIMESHEET_SEQ DESC",
                    DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < myDataView.Count; intRow++)
                {
                    myDataView[intRow].Delete();

                    intRow -= 1;
                }

                myDataView = null;
                myDataView = new DataView(this.pvtDataSet.Tables["Break"],
                "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND BREAK_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'",
                    "",
                    DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < myDataView.Count; intRow++)
                {
                    myDataView[intRow].Delete();

                    intRow -= 1;
                }
            }
            else
            {
                //Employee
                objParm[3] = "E";
                objParm[4] = pvtintEmployeeNo.ToString();
                
                DataView myDataView = new DataView(this.pvtDataSet.Tables["DayBlank"],
                "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND EMPLOYEE_NO = " + pvtintEmployeeNo,
                "",
                DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < myDataView.Count; intRow++)
                {
                    myDataView[intRow].Delete();

                    intRow -= 1;
                }

                myDataView = null;
                myDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND EMPLOYEE_NO = " + pvtintEmployeeNo,
                    "",
                    DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < myDataView.Count; intRow++)
                {
                    myDataView[intRow].Delete();

                    intRow -= 1;
                }

                myDataView = null;
                myDataView = new DataView(this.pvtDataSet.Tables["TimeSheet"],
                "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND EMPLOYEE_NO = " + pvtintEmployeeNo,
                    "TIMESHEET_SEQ DESC",
                    DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < myDataView.Count; intRow++)
                {
                    myDataView[intRow].Delete();

                    intRow -= 1;
                }

                myDataView = null;
                myDataView = new DataView(this.pvtDataSet.Tables["Break"],
                "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND EMPLOYEE_NO = " + pvtintEmployeeNo,
                    "",
                    DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < myDataView.Count; intRow++)
                {
                    myDataView[intRow].Delete();

                    intRow -= 1;
                }
            }

            objParm[5] = pvtbytCompress;
            objParm[6] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
            objParm[7] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

            pvtbytCompress = null;
            pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Update_TimeSheet_Records", objParm, true);

            TempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
            pvtDataSet.Merge(TempDataSet);

            this.pvtDataSet.AcceptChanges();

            if (this.rbnDateEmployee.Checked == true)
            {
                Create_DayBlank_Records("D",pvtstrPayrollType, pvtintPayCategoryNo, pvtDateTime, -1);
            }
            else
            {
                Create_DayBlank_Records("E",pvtstrPayrollType, pvtintPayCategoryNo, DateTime.Now, pvtintEmployeeNo);
            }

            this.pvtDataSet.AcceptChanges();
            
            btnCancel_Click(sender, e);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            DataView myDataView = new DataView(this.pvtDataSet.Tables["DayBlank"],
                "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND DAY_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'",
                "",
                DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < myDataView.Count; intRow++)
            {
                myDataView[intRow].Delete();

                intRow -= 1;
            }

            myDataView = null;
            myDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND DAY_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'",
                "",
                DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < myDataView.Count; intRow++)
            {
                myDataView[intRow].Delete();

                intRow -= 1;
            }

            myDataView = null;
            myDataView = new DataView(this.pvtDataSet.Tables["TimeSheet"],
            "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND TIMESHEET_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'",
                "TIMESHEET_SEQ DESC",
                DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < myDataView.Count; intRow++)
            {
                myDataView[intRow].Delete();

                intRow -= 1;
            }

            myDataView = null;
            myDataView = new DataView(this.pvtDataSet.Tables["Break"],
            "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND BREAK_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'",
                "",
                DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < myDataView.Count; intRow++)
            {
                myDataView[intRow].Delete();

                intRow -= 1;
            }

            object[] objParm = new object[5];
            objParm[0] = pvtint64CompanyNo;
            objParm[1] = pvtstrPayrollType;
            objParm[2] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
            objParm[3] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
            objParm[4] = pvtDateTime.ToString("yyyy-MM-dd");
            
            byte[] bytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Day_Timesheets_Records", objParm);
            DataSet DataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);

            //Delete Rows
            pvtDataSet.AcceptChanges();

            pvtDataSet.Merge(DataSet);

            Create_DayBlank_Records("D",pvtstrPayrollType, -1, pvtDateTime,-1);

            pvtDataSet.AcceptChanges();

            this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView));
        }

        private void dgvPayrollTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (this.pvtblnPayrollTypeDataGridViewLoaded == true)
            {
                if (pvtintPayrollTypeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayrollTypeDataGridViewRowIndex = e.RowIndex;

                    this.chkEndDateOnly.Checked = false;
                    this.rbnNone.Checked = true;

                    if (pvtblnFilterClicked == false)
                    {
                        this.rbnDateNormal.Checked = true;
                    }

                    EventArgs ev = new EventArgs();

                    btnRemoveFilter_Click(null, ev);
                    
                    this.Cursor = Cursors.AppStarting;

                    pvtstrPayrollType = this.dgvPayrollTypeDataGridView[0, e.RowIndex].Value.ToString().Substring(0, 1);
                    
                    DateTime myStartDateTime = DateTime.ParseExact(this.dgvPayrollTypeDataGridView[1, e.RowIndex].Value.ToString(), "yyyyMMdd", null);

                    if (rbnDateNormal.Checked == true)
                    {
                        pvtDataSet.Tables["DateSelection"].Rows.Clear();
                        this.cboDateFilter.Items.Clear();

                        int intComboSelectIndex = 0;

                        while (myStartDateTime <= DateTime.Now.AddDays(15))
                        {
                            DataRow MyDataRow = pvtDataSet.Tables["DateSelection"].NewRow();

                            MyDataRow["ACTUAL_DATE"] = myStartDateTime.ToString("yyyy-MM-dd");

                            pvtDataSet.Tables["DateSelection"].Rows.Add(MyDataRow);

                            this.cboDateFilter.Items.Add(myStartDateTime.ToString("dd MMM yyyy - dddd"));

                            if (myStartDateTime.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd"))
                            {
                                intComboSelectIndex = this.cboDateFilter.Items.Count - 1;
                            }

                            myStartDateTime = myStartDateTime.AddDays(1);
                        }

#if (DEBUG)
                        //intComboSelectIndex = 63;
#endif

                        pvtDataSet.AcceptChanges();

                        pvtblnBusyLoading = true;

                        this.cboDateFilter.SelectedIndex = intComboSelectIndex;

                        pvtblnBusyLoading = false;
                    }

                    DataView DataView = new System.Data.DataView(pvtDataSet.Tables["Dates"], "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'", "", DataViewRowState.CurrentRows);

                    if (DataView.Count == 0)
                    {
                        object[] objParm = new object[4];
                        objParm[0] = pvtint64CompanyNo;
                        objParm[1] = pvtstrPayrollType;
                        objParm[2] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                        objParm[3] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                        byte[] bytCompress = (byte[])clsISUtilities.DynamicFunction("Get_PayCategory_Records", objParm);
                        DataSet TempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);

                        if (TempDataSet.Tables["Dates"].Rows.Count > 0)
                        {
                            pvtDataSet.Merge(TempDataSet);

                            Create_DayBlank_Records("P",pvtstrPayrollType,-1,DateTime.Now,-1);

                            pvtDataSet.AcceptChanges();
                        }
                    }

                    if (pvtblnBusyLoading == false)
                    {
                        Load_PayCategory_Records();
                    }

                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void dgvPayCategoryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (this.pvtblnPayCategoryDataGridViewLoaded == true)
            {
                if (pvtintPayCategoryDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayCategoryDataGridViewRowIndex = e.RowIndex;

                    this.Cursor = Cursors.AppStarting;

                    Clear_DataGridView(dgvEmployeeDataGridView);
                    Clear_DataGridView(dgvDayDataGridView);
                    Clear_DataGridView(dgvTimeSheetDataGridView);
                    Clear_DataGridView(dgvBreakRangeDataGridView);
                    Clear_DataGridView(dgvBreakDataGridView);

                    dgvBreakTotalsDataGridView[0, 0].Style = LunchTotalDataGridViewCellStyle;
                    this.dgvBreakTotalsDataGridView[0, 0].Value = "";
                    this.dgvTimeSheetTotalsDataGridView[0, 1].Value = "Break After 0:00";
                    this.dgvTimeSheetTotalsDataGridView[1, 1].Value = "0:00";

                    this.dgvBreakExceptionDataGridView[0, 0].Value = "";

                    this.grbBreakError.Visible = false;
                    this.grbLeaveError.Visible = false;

                    pvtintPayCategoryTableRowNo = Convert.ToInt32(this.dgvPayCategoryDataGridView[pvtintRowColPayCategoryDataGridView, e.RowIndex].Value);

                    pvtintPayCategoryNo = Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_CATEGORY_NO"]);

                    if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_PERIOD_DATE"] == System.DBNull.Value)
                    {
                        this.rbnDateNormal.Checked = true;
                        this.rbnDateNormal.Enabled = false;
                        this.rbnDateWageRun.Enabled = false;
                        this.rbnGreaterWageDate.Enabled = false;
                    }
                    else
                    {
                        this.rbnDateNormal.Enabled = true;
                        this.rbnDateWageRun.Enabled = true;
                        this.rbnGreaterWageDate.Enabled = true;
                    }

                    pvtstrPayCategoryFilter = "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo;

                    pvtBreakRangeDataView = null;
                    pvtBreakRangeDataView = new DataView(this.pvtDataSet.Tables["BreakRange"],
                    pvtstrPayCategoryFilter,
                    "WORKED_TIME_MINUTES,BREAK_MINUTES",
                    DataViewRowState.CurrentRows);

                    pvtblnBreakRangeDataGridViewLoaded = false;

                    for (int intRow = 0; intRow < pvtBreakRangeDataView.Count; intRow++)
                    {
                        this.dgvBreakRangeDataGridView.Rows.Add(Convert.ToInt32(Convert.ToInt32(pvtBreakRangeDataView[intRow]["WORKED_TIME_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtBreakRangeDataView[intRow]["WORKED_TIME_MINUTES"]) % 60).ToString("00"),
                                                                Convert.ToInt32(Convert.ToInt32(pvtBreakRangeDataView[intRow]["BREAK_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtBreakRangeDataView[intRow]["BREAK_MINUTES"]) % 60).ToString("00"));
                    }

                    pvtblnBreakRangeDataGridViewLoaded = true;

                    if (dgvBreakRangeDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(dgvBreakRangeDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvBreakRangeDataGridView));
                    }

                    string strDirection = "";

                    if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["DAILY_ROUNDING_IND"].ToString() == "0")
                    {
                        this.dgvTimeSheetTotalsDataGridView[0, 3].Value = "No Rounding";
                    }
                    else
                    {
                        if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["DAILY_ROUNDING_IND"].ToString() == "1")
                        {
                            strDirection = "Rnd Up";
                        }
                        else
                        {
                            if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["DAILY_ROUNDING_IND"].ToString() == "2")
                            {
                                strDirection = "Rnd Down";
                            }
                            else
                            {
                                if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["DAILY_ROUNDING_IND"].ToString() == "3")
                                {
                                    strDirection = "Rnd Closest";
                                }
                            }
                        }

                        this.dgvTimeSheetTotalsDataGridView[0, 3].Value = strDirection + " " + Convert.ToDouble(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["DAILY_ROUNDING_MINUTES"]).ToString("#0") + " Min";
                    }

                    Load_Employee_SpreadSheet();

                    this.Cursor = Cursors.Default;
                }
            }
        }
        private void Create_DayBlank_Records(string strType,string strPayCategoryType,int intPayCategoryNo,DateTime dtCurrentDate,int intEmployeeNo)
        {
            DataTable dayBlankDataTable = pvtDataSet.Tables["DayBlank"].Clone();
            bool blnHasData = false;

            if (strType == "P")
            {
                //For PAY_CATEGORY_TYPE
                var blankDataTable = (from employeePayCategory in pvtDataSet.Tables["EmployeePayCategory"].AsEnumerable()

                                     join dates in pvtDataSet.Tables["Dates"].AsEnumerable()
                                     on new { PAY_CATEGORY_TYPE = employeePayCategory["PAY_CATEGORY_TYPE"], PAY_CATEGORY_NO = employeePayCategory["PAY_CATEGORY_NO"] } equals new { PAY_CATEGORY_TYPE = dates["PAY_CATEGORY_TYPE"], PAY_CATEGORY_NO = dates["PAY_CATEGORY_NO"] }

                                     //Exclude Leave (Blank DataTable will have Leave record)
                                     join dayTotal in pvtDataSet.Tables["DayTotal"].AsEnumerable().Where(r => r["LEAVE_INDICATOR"].ToString() == "")
                                     on new { PAY_CATEGORY_TYPE = employeePayCategory["PAY_CATEGORY_TYPE"], PAY_CATEGORY_NO = employeePayCategory["PAY_CATEGORY_NO"], EMPLOYEE_NO = employeePayCategory["EMPLOYEE_NO"], DAY_DATE = dates["DAY_DATE"] } equals new { PAY_CATEGORY_TYPE = dayTotal["PAY_CATEGORY_TYPE"], PAY_CATEGORY_NO = dayTotal["PAY_CATEGORY_NO"], EMPLOYEE_NO = dayTotal["EMPLOYEE_NO"], DAY_DATE = dayTotal["DAY_DATE"] }
                                     into TempTable
                                     from tempTable in TempTable.DefaultIfEmpty()

                                     join publicHoliday in pvtDataSet.Tables["PublicHoliday"].AsEnumerable()
                                     on dates["DAY_DATE"] equals publicHoliday["PUBLIC_HOLIDAY_DATE"]
                                     into TempWithPublicHolidayTable
                                     from tempWithPublicHolidayTable in TempWithPublicHolidayTable.DefaultIfEmpty()

                                     join employeeLeave in pvtDataSet.Tables["EmployeeLeave"].AsEnumerable()
                                     on new { PAY_CATEGORY_TYPE = employeePayCategory["PAY_CATEGORY_TYPE"], PAY_CATEGORY_NO = employeePayCategory["PAY_CATEGORY_NO"], EMPLOYEE_NO = employeePayCategory["EMPLOYEE_NO"], DAY_DATE = dates["DAY_DATE"] } equals new { PAY_CATEGORY_TYPE = employeeLeave["PAY_CATEGORY_TYPE"], PAY_CATEGORY_NO = employeeLeave["PAY_CATEGORY_NO"], EMPLOYEE_NO = employeeLeave["EMPLOYEE_NO"], DAY_DATE = employeeLeave["DAY_DATE"] }
                                     into TempWithLeaveTable
                                     from tempWithLeaveTable in TempWithLeaveTable.DefaultIfEmpty()

                                     where employeePayCategory["PAY_CATEGORY_TYPE"].ToString() == strPayCategoryType
                                     && Convert.ToDateTime(employeePayCategory["EMPLOYEE_LAST_RUNDATE"]) < Convert.ToDateTime(dates["DAY_DATE"])

                                     //Exclude where DayTotal Record Exits 
                                     && tempTable == null

                                     select dayBlankDataTable.LoadDataRow(new object[]
                                     {
                                            Convert.ToInt32(employeePayCategory["COMPANY_NO"]),
                                            Convert.ToString(employeePayCategory["PAY_CATEGORY_TYPE"]),
                                            Convert.ToInt16(employeePayCategory["PAY_CATEGORY_NO"]),
                                            Convert.ToInt16(employeePayCategory["EMPLOYEE_NO"]),
                                            Convert.ToDateTime(dates["DAY_DATE"]),
                                            Convert.ToInt16(dates["DAY_NO"]),
                                            0,
                                            "",
                                            0,
                                            "",
                                            tempWithPublicHolidayTable != null ? "Y" : "",
                                            "",
                                            tempWithLeaveTable != null ? "Y" : "",
                                     }, false)).ToList<DataRow>();


                if (blankDataTable != null)
                {
                    if (blankDataTable.Count() > 0)
                    {
                        blnHasData = true;
                        dayBlankDataTable = blankDataTable.CopyToDataTable<DataRow>();
                    }
                }
            }
            else
            {
                if (strType == "E")
                {
                    //For Specific PAY_CATEGORY_TYPE,PAY_CATEGORY_NO,EMPLOYEE_NO
                    var blankDataTable = (from employeePayCategory in pvtDataSet.Tables["EmployeePayCategory"].AsEnumerable()

                                         join dates in pvtDataSet.Tables["Dates"].AsEnumerable()
                                         on new { PAY_CATEGORY_TYPE = employeePayCategory["PAY_CATEGORY_TYPE"], PAY_CATEGORY_NO = employeePayCategory["PAY_CATEGORY_NO"] } equals new { PAY_CATEGORY_TYPE = dates["PAY_CATEGORY_TYPE"], PAY_CATEGORY_NO = dates["PAY_CATEGORY_NO"] }

                                         //Exclude Leave (Blank DataTable will have Leave record)
                                         join dayTotal in pvtDataSet.Tables["DayTotal"].AsEnumerable().Where(r => r["LEAVE_INDICATOR"].ToString() == "")
                                         on new { PAY_CATEGORY_TYPE = employeePayCategory["PAY_CATEGORY_TYPE"], PAY_CATEGORY_NO = employeePayCategory["PAY_CATEGORY_NO"], EMPLOYEE_NO = employeePayCategory["EMPLOYEE_NO"], DAY_DATE = dates["DAY_DATE"] } equals new { PAY_CATEGORY_TYPE = dayTotal["PAY_CATEGORY_TYPE"], PAY_CATEGORY_NO = dayTotal["PAY_CATEGORY_NO"], EMPLOYEE_NO = dayTotal["EMPLOYEE_NO"], DAY_DATE = dayTotal["DAY_DATE"] }
                                         into TempTable
                                         from tempTable in TempTable.DefaultIfEmpty()

                                         join publicHoliday in pvtDataSet.Tables["PublicHoliday"].AsEnumerable()
                                         on dates["DAY_DATE"] equals publicHoliday["PUBLIC_HOLIDAY_DATE"]
                                         into TempWithPublicHolidayTable
                                         from tempWithPublicHolidayTable in TempWithPublicHolidayTable.DefaultIfEmpty()

                                         join employeeLeave in pvtDataSet.Tables["EmployeeLeave"].AsEnumerable()
                                         on new { PAY_CATEGORY_TYPE = employeePayCategory["PAY_CATEGORY_TYPE"], PAY_CATEGORY_NO = employeePayCategory["PAY_CATEGORY_NO"], EMPLOYEE_NO = employeePayCategory["EMPLOYEE_NO"], DAY_DATE = dates["DAY_DATE"] } equals new { PAY_CATEGORY_TYPE = employeeLeave["PAY_CATEGORY_TYPE"], PAY_CATEGORY_NO = employeeLeave["PAY_CATEGORY_NO"], EMPLOYEE_NO = employeeLeave["EMPLOYEE_NO"], DAY_DATE = employeeLeave["DAY_DATE"] }
                                         into TempWithLeaveTable
                                         from tempWithLeaveTable in TempWithLeaveTable.DefaultIfEmpty()

                                         where employeePayCategory["PAY_CATEGORY_TYPE"].ToString() == strPayCategoryType
                                         && Convert.ToInt32(employeePayCategory["PAY_CATEGORY_NO"]) == intPayCategoryNo
                                         && Convert.ToInt32(employeePayCategory["EMPLOYEE_NO"]) == intEmployeeNo

                                         //Exclude where DayTotal Record Exits 
                                         && tempTable == null

                                         select dayBlankDataTable.LoadDataRow(new object[]
                                         {
                                                Convert.ToInt32(employeePayCategory["COMPANY_NO"]),
                                                Convert.ToString(employeePayCategory["PAY_CATEGORY_TYPE"]),
                                                Convert.ToInt16(employeePayCategory["PAY_CATEGORY_NO"]),
                                                Convert.ToInt16(employeePayCategory["EMPLOYEE_NO"]),
                                                Convert.ToDateTime(dates["DAY_DATE"]),
                                                Convert.ToInt16(dates["DAY_NO"]),
                                                0,
                                                "",
                                                0,
                                                "",
                                                tempWithPublicHolidayTable != null ? "Y" : "",
                                                "",
                                                tempWithLeaveTable != null ? "Y" : "",
                                         }, false)).ToList<DataRow>();

                    if (blankDataTable != null)
                    {
                        if (blankDataTable.Count() > 0)
                        {
                            blnHasData = true;
                            dayBlankDataTable = blankDataTable.CopyToDataTable<DataRow>();
                        }
                    }
                }
                else
                {
                    if (strType == "D")
                    {
                        if (intPayCategoryNo == -1
                        && intEmployeeNo == -1)
                        {
                            //REFRESH - Specific Date
                            var blankDataTable = (from employeePayCategory in pvtDataSet.Tables["EmployeePayCategory"].AsEnumerable()

                                                 join dates in pvtDataSet.Tables["Dates"].AsEnumerable()
                                                 on new { PAY_CATEGORY_TYPE = employeePayCategory["PAY_CATEGORY_TYPE"], PAY_CATEGORY_NO = employeePayCategory["PAY_CATEGORY_NO"] } equals new { PAY_CATEGORY_TYPE = dates["PAY_CATEGORY_TYPE"], PAY_CATEGORY_NO = dates["PAY_CATEGORY_NO"] }

                                                 //Exclude Leave (Blank DataTable will have Leave record)
                                                 join dayTotal in pvtDataSet.Tables["DayTotal"].AsEnumerable().Where(r => r["LEAVE_INDICATOR"].ToString() == "")
                                                 on new { PAY_CATEGORY_TYPE = employeePayCategory["PAY_CATEGORY_TYPE"], PAY_CATEGORY_NO = employeePayCategory["PAY_CATEGORY_NO"], EMPLOYEE_NO = employeePayCategory["EMPLOYEE_NO"], DAY_DATE = dates["DAY_DATE"] } equals new { PAY_CATEGORY_TYPE = dayTotal["PAY_CATEGORY_TYPE"], PAY_CATEGORY_NO = dayTotal["PAY_CATEGORY_NO"], EMPLOYEE_NO = dayTotal["EMPLOYEE_NO"], DAY_DATE = dayTotal["DAY_DATE"] }
                                                 into TempTable
                                                 from tempTable in TempTable.DefaultIfEmpty()

                                                 join publicHoliday in pvtDataSet.Tables["PublicHoliday"].AsEnumerable()
                                                 on dates["DAY_DATE"] equals publicHoliday["PUBLIC_HOLIDAY_DATE"]
                                                 into TempWithPublicHolidayTable
                                                 from tempWithPublicHolidayTable in TempWithPublicHolidayTable.DefaultIfEmpty()

                                                 join employeeLeave in pvtDataSet.Tables["EmployeeLeave"].AsEnumerable()
                                                 on new { PAY_CATEGORY_TYPE = employeePayCategory["PAY_CATEGORY_TYPE"], PAY_CATEGORY_NO = employeePayCategory["PAY_CATEGORY_NO"], EMPLOYEE_NO = employeePayCategory["EMPLOYEE_NO"], DAY_DATE = dates["DAY_DATE"] } equals new { PAY_CATEGORY_TYPE = employeeLeave["PAY_CATEGORY_TYPE"], PAY_CATEGORY_NO = employeeLeave["PAY_CATEGORY_NO"], EMPLOYEE_NO = employeeLeave["EMPLOYEE_NO"], DAY_DATE = employeeLeave["DAY_DATE"] }
                                                 into TempWithLeaveTable
                                                 from tempWithLeaveTable in TempWithLeaveTable.DefaultIfEmpty()

                                                 where employeePayCategory["PAY_CATEGORY_TYPE"].ToString() == strPayCategoryType
                                                 && Convert.ToDateTime(dates["DAY_DATE"]) == dtCurrentDate
                                                 && Convert.ToDateTime(employeePayCategory["EMPLOYEE_LAST_RUNDATE"]) < Convert.ToDateTime(dates["DAY_DATE"])

                                                 //Exclude where DayTotal Record Exits 
                                                 && tempTable == null

                                                 select dayBlankDataTable.LoadDataRow(new object[]
                                                 {
                                        Convert.ToInt32(employeePayCategory["COMPANY_NO"]),
                                        Convert.ToString(employeePayCategory["PAY_CATEGORY_TYPE"]),
                                        Convert.ToInt16(employeePayCategory["PAY_CATEGORY_NO"]),
                                        Convert.ToInt16(employeePayCategory["EMPLOYEE_NO"]),
                                        Convert.ToDateTime(dates["DAY_DATE"]),
                                        Convert.ToInt16(dates["DAY_NO"]),
                                        0,
                                        "",
                                        0,
                                        "",
                                        tempWithPublicHolidayTable != null ? "Y" : "",
                                        "",
                                        tempWithLeaveTable != null ? "Y" : "",
                                                 }, false)).ToList<DataRow>();

                            if (blankDataTable != null)
                            {
                                if (blankDataTable.Count() > 0)
                                {
                                    blnHasData = true;
                                    dayBlankDataTable = blankDataTable.CopyToDataTable<DataRow>();
                                }
                            }

                        }
                        else
                        {
                            //For Specific PAY_CATEGORY_TYPE,PAY_CATEGORY_NO,[DATE}
                            var blankDataTable = (from employeePayCategory in pvtDataSet.Tables["EmployeePayCategory"].AsEnumerable()

                                                 join dates in pvtDataSet.Tables["Dates"].AsEnumerable()
                                                 on new { PAY_CATEGORY_TYPE = employeePayCategory["PAY_CATEGORY_TYPE"], PAY_CATEGORY_NO = employeePayCategory["PAY_CATEGORY_NO"] } equals new { PAY_CATEGORY_TYPE = dates["PAY_CATEGORY_TYPE"], PAY_CATEGORY_NO = dates["PAY_CATEGORY_NO"] }

                                                 //Exclude Leave (Blank DataTable will have Leave record)
                                                 join dayTotal in pvtDataSet.Tables["DayTotal"].AsEnumerable().Where(r => r["LEAVE_INDICATOR"].ToString() == "")
                                                 on new { PAY_CATEGORY_TYPE = employeePayCategory["PAY_CATEGORY_TYPE"], PAY_CATEGORY_NO = employeePayCategory["PAY_CATEGORY_NO"], EMPLOYEE_NO = employeePayCategory["EMPLOYEE_NO"], DAY_DATE = dates["DAY_DATE"] } equals new { PAY_CATEGORY_TYPE = dayTotal["PAY_CATEGORY_TYPE"], PAY_CATEGORY_NO = dayTotal["PAY_CATEGORY_NO"], EMPLOYEE_NO = dayTotal["EMPLOYEE_NO"], DAY_DATE = dayTotal["DAY_DATE"] }
                                                 into TempTable
                                                 from tempTable in TempTable.DefaultIfEmpty()

                                                 join publicHoliday in pvtDataSet.Tables["PublicHoliday"].AsEnumerable()
                                                 on dates["DAY_DATE"] equals publicHoliday["PUBLIC_HOLIDAY_DATE"]
                                                 into TempWithPublicHolidayTable
                                                 from tempWithPublicHolidayTable in TempWithPublicHolidayTable.DefaultIfEmpty()

                                                 join employeeLeave in pvtDataSet.Tables["EmployeeLeave"].AsEnumerable()
                                                 on new { PAY_CATEGORY_TYPE = employeePayCategory["PAY_CATEGORY_TYPE"], PAY_CATEGORY_NO = employeePayCategory["PAY_CATEGORY_NO"], EMPLOYEE_NO = employeePayCategory["EMPLOYEE_NO"], DAY_DATE = dates["DAY_DATE"] } equals new { PAY_CATEGORY_TYPE = employeeLeave["PAY_CATEGORY_TYPE"], PAY_CATEGORY_NO = employeeLeave["PAY_CATEGORY_NO"], EMPLOYEE_NO = employeeLeave["EMPLOYEE_NO"], DAY_DATE = employeeLeave["DAY_DATE"] }
                                                 into TempWithLeaveTable
                                                 from tempWithLeaveTable in TempWithLeaveTable.DefaultIfEmpty()

                                                 where employeePayCategory["PAY_CATEGORY_TYPE"].ToString() == strPayCategoryType
                                                 && Convert.ToInt32(employeePayCategory["PAY_CATEGORY_NO"]) == intPayCategoryNo
                                                 && Convert.ToDateTime(dates["DAY_DATE"]) == dtCurrentDate
                                                 && Convert.ToDateTime(employeePayCategory["EMPLOYEE_LAST_RUNDATE"]) < Convert.ToDateTime(dates["DAY_DATE"])

                                                 //Exclude where DayTotal Record Exits 
                                                 && tempTable == null

                                                 select dayBlankDataTable.LoadDataRow(new object[]
                                                 {
                                                Convert.ToInt32(employeePayCategory["COMPANY_NO"]),
                                                Convert.ToString(employeePayCategory["PAY_CATEGORY_TYPE"]),
                                                Convert.ToInt16(employeePayCategory["PAY_CATEGORY_NO"]),
                                                Convert.ToInt16(employeePayCategory["EMPLOYEE_NO"]),
                                                Convert.ToDateTime(dates["DAY_DATE"]),
                                                Convert.ToInt16(dates["DAY_NO"]),
                                                0,
                                                "",
                                                0,
                                                "",
                                                tempWithPublicHolidayTable != null ? "Y" : "",
                                                "",
                                                tempWithLeaveTable != null ? "Y" : "",
                                                 }, false)).ToList<DataRow>();

                            if (blankDataTable != null)
                            {
                                if (blankDataTable.Count() > 0)
                                {
                                    blnHasData = true;
                                    dayBlankDataTable = blankDataTable.CopyToDataTable<DataRow>();
                                }
                            }
                        }
                    }
                }
            }

            if (blnHasData == true)
            {
                dayBlankDataTable.TableName = "DayBlank";

                //Add Rows to DayBlank
                pvtDataSet.Tables["DayBlank"].Merge(dayBlankDataTable);

                DataView EmployeeLeaveDataView = null;

                if (strType == "P")
                {
                    //For PAY_CATEGORY_TYPE
                    EmployeeLeaveDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                                              "PAY_CATEGORY_TYPE = '" + strPayCategoryType + "' AND LEAVE_INDICATOR = 'Y'",
                                              "",
                                              DataViewRowState.CurrentRows);
                }
                else
                {
                    if (strType == "E")
                    {
                        //For Specific PAY_CATEGORY_TYPE,PAY_CATEGORY_NO,EMPLOYEE_NO
                        EmployeeLeaveDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                                            "PAY_CATEGORY_TYPE = '" + strPayCategoryType + "' AND PAY_CATEGORY_NO = " + intPayCategoryNo + " AND EMPLOYEE_NO = " + intEmployeeNo + " AND LEAVE_INDICATOR = 'Y'",
                                            "",
                                            DataViewRowState.CurrentRows);
                    }
                    else
                    {
                        if (strType == "D")
                        {
                            if (intPayCategoryNo == -1
                            && intEmployeeNo == -1)
                            {
                                //REFRESH - Specific Date
                                //For Specific PAY_CATEGORY_TYPE,PAY_CATEGORY_NO,EMPLOYEE_NO
                                EmployeeLeaveDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                                                    "PAY_CATEGORY_TYPE = '" + strPayCategoryType + "' AND DAY_DATE = '" + dtCurrentDate.ToString("yyyy-MM-dd") + "' AND LEAVE_INDICATOR = 'Y'",
                                                    "",
                                                    DataViewRowState.CurrentRows);
                            }
                            else
                            {
                                //For Specific PAY_CATEGORY_TYPE,PAY_CATEGORY_NO,[DATE}
                                EmployeeLeaveDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                                                  "PAY_CATEGORY_TYPE = '" + strPayCategoryType + "' AND PAY_CATEGORY_NO = " + intPayCategoryNo + " AND DAY_DATE = '" + dtCurrentDate.ToString("yyyy-MM-dd") + "' AND LEAVE_INDICATOR = 'Y'",
                                                  "",
                                                  DataViewRowState.CurrentRows);

                            }
                        }
                    }
                }

                for (int intLeaveRow = 0; intLeaveRow < EmployeeLeaveDataView.Count; intLeaveRow++)
                {
                    DataView BlankDataView = new DataView(this.pvtDataSet.Tables["DayBlank"],
                                             "COMPANY_NO = " + this.pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + EmployeeLeaveDataView[intLeaveRow]["PAY_CATEGORY_TYPE"].ToString()
                                             + "' AND PAY_CATEGORY_NO = " + EmployeeLeaveDataView[intLeaveRow]["PAY_CATEGORY_NO"].ToString() + " AND EMPLOYEE_NO = " + EmployeeLeaveDataView[intLeaveRow]["EMPLOYEE_NO"].ToString()
                                             + " AND DAY_DATE = '" + Convert.ToDateTime(EmployeeLeaveDataView[intLeaveRow]["DAY_DATE"]).ToString("yyyy-MM-dd") + "'",
                                              "",
                                              DataViewRowState.CurrentRows);

                    BlankDataView[0]["LEAVE_INDICATOR"] = "Y";

                    if (EmployeeLeaveDataView[intLeaveRow]["INDICATOR"].ToString() == "X")
                    {
                        BlankDataView[0]["INDICATOR"] = "X";
                    }
                }
            }
        }

        private void dgvPayCategoryDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == pvtintLastUploadDateTimeColPayCategoryDataGridView)
            {
                if (dgvPayCategoryDataGridView[pvtintLastUploadDateTimeSortColPayCategoryDataGridView, e.RowIndex1].Value.ToString() == "")
                {
                    e.SortResult = -1;
                }
                else
                {
                    if (dgvPayCategoryDataGridView[pvtintLastUploadDateTimeSortColPayCategoryDataGridView, e.RowIndex2].Value.ToString() == "")
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(dgvPayCategoryDataGridView[pvtintLastUploadDateTimeSortColPayCategoryDataGridView, e.RowIndex1].Value.ToString()) > double.Parse(dgvPayCategoryDataGridView[pvtintLastUploadDateTimeSortColPayCategoryDataGridView, e.RowIndex2].Value.ToString()))
                        {
                            e.SortResult = 1;
                        }
                        else
                        {
                            if (double.Parse(dgvPayCategoryDataGridView[pvtintLastUploadDateTimeSortColPayCategoryDataGridView, e.RowIndex1].Value.ToString()) < double.Parse(dgvPayCategoryDataGridView[pvtintLastUploadDateTimeSortColPayCategoryDataGridView, e.RowIndex2].Value.ToString()))
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

        private void dgvEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (this.pvtblnEmployeeDataGridViewLoaded == true)
            {
                if (pvtintEmployeeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintEmployeeDataGridViewRowIndex = e.RowIndex;

                    this.Cursor = Cursors.AppStarting;

                    if (rbnEmployeeDate.Checked == true)
                    {
                        //Get Employee Number
                        pvtintEmployeeNo = Convert.ToInt32(pvtEmployeeOrDateDataView[Convert.ToInt32(this.dgvEmployeeDataGridView[pvtintKeyColEmployeeDataGridView, e.RowIndex].Value)]["EMPLOYEE_NO"]);

                        pvtEmployeeDataView = null;
                        pvtEmployeeDataView = new DataView(this.pvtDataSet.Tables["Dates"],
                            pvtstrPayCategoryFilter + " AND DAY_DATE > '" + Convert.ToDateTime(pvtEmployeeOrDateDataView[Convert.ToInt32(this.dgvEmployeeDataGridView[pvtintKeyColEmployeeDataGridView, e.RowIndex].Value)]["EMPLOYEE_LAST_RUNDATE"]).ToString("yyyy-MM-dd") + "'" + pvtstrDataAndTypeFilter + pvtstrRemoveDataFilter,
                            "DAY_DATE DESC",
                            DataViewRowState.CurrentRows);
                    }
                    else
                    {
                        //Get Current Date
                        this.pvtDateTime = Convert.ToDateTime(pvtEmployeeOrDateDataView[Convert.ToInt32(this.dgvEmployeeDataGridView[pvtintKeyColEmployeeDataGridView, e.RowIndex].Value)]["DAY_DATE"]);

                        this.toolTip.SetToolTip(btnRefresh, "Refresh " + pvtDateTime.ToString("dd MMMM yyyy") + " (Selected Date Row)");

                        pvtEmployeeDataView = null;
                        pvtEmployeeDataView = new DataView(this.pvtDataSet.Tables["Employee"],
                            pvtstrPayCategoryFilter + " AND EMPLOYEE_LAST_RUNDATE < '" + pvtDateTime.ToString("yyyy-MM-dd") + "'",
                            "EMPLOYEE_CODE",
                            DataViewRowState.CurrentRows);
                    }

                    Load_Day_SpreadSheet();

                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void dgvDayDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (this.pvtblnDayDataGridViewLoaded == true)
            {
                if (pvtintDayDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintDayDataGridViewRowIndex = e.RowIndex;

                    this.grbBreakError.Visible = false;
                    this.grbLeaveError.Visible = false;

                    if (this.rbnEmployeeDate.Checked == true)
                    {
                        //Get Date
                        pvtDateTime = Convert.ToDateTime(this.dgvDayDataGridView[this.pvtintKeyColDayDataGridView, e.RowIndex].Value);

                        this.toolTip.SetToolTip(btnRefresh, "Refresh " + pvtDateTime.ToString("dd MMMM yyyy") + " (Selected Date Row)");

                        pvtDayTotalDataView = null;
                        pvtDayTotalDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                            "COMPANY_NO = " + this.pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND DAY_DATE = '" + this.pvtDateTime.ToString("yyyy-MM-dd") + "'",
                            "DAY_DATE",
                            DataViewRowState.CurrentRows);

                        //ELR - 2015-03-20
                        this.lblDayDesc.Text = this.dgvEmployeeDataGridView[pvtintNameColEmployeeDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Value.ToString() + " " + this.dgvEmployeeDataGridView[pvtintSurnameColEmployeeDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Value.ToString() + "    " + this.dgvDayDataGridView[pvtintNoColDayDataGridView, e.RowIndex].Value.ToString();
                    }
                    else
                    {
                        pvtintEmployeeNo = Convert.ToInt32(pvtEmployeeDataView[Convert.ToInt32(this.dgvDayDataGridView[pvtintKeyColDayDataGridView, e.RowIndex].Value)]["EMPLOYEE_NO"]);

                        pvtDayTotalDataView = null;
                        pvtDayTotalDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                            "COMPANY_NO = " + this.pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND DAY_DATE = '" + this.pvtDateTime.ToString("yyyy-MM-dd") + "'",
                            "DAY_DATE",
                            DataViewRowState.CurrentRows);
                      
                        //ELR - 2015-03-20
                        this.lblDayDesc.Text = this.dgvDayDataGridView[pvtintNameColDayDataGridView, e.RowIndex].Value.ToString() + " " + this.dgvDayDataGridView[pvtintSurnameColDayDataGridView, e.RowIndex].Value.ToString() + "    " + this.dgvEmployeeDataGridView[pvtintNoColEmployeeDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Value.ToString();
                    }

                    if (this.btnSave.Enabled == false)
                    {
                        if (this.dgvDayDataGridView[pvtintRecordLockedColDayDataGridView, e.RowIndex].Value.ToString() == "Y"
                            | this.dgvDayDataGridView[pvtintRecordLockedColDayDataGridView, e.RowIndex].Value.ToString() == "A")
                        {
                            this.btnUpdate.Enabled = false;
                        }
                        else
                        {
                            this.btnUpdate.Enabled = true;
                        }
                    }

                    //Errol To Look AT
                    Load_TimeSheets();
                }
            }
        }

        private void Load_TimeSheets()
        {
            pvtintTotalTimeSheetMinutes = 0;
            pvtintTotalBreakMinutes = 0;
            string strClockTimeIn = "";
            string strActualTimeIn = "";
            string strActualTimeOut = "";
            string strClockTimeOut = "";

            this.pvtblnBreakDataGridViewLoaded = false;
            this.pvtblnTimeSheetDataGridViewLoaded = false;

            pvtblnTimeSheetInError = false;
            pvtblnBreakInError = false;

            pvtBreakDataView = null;
            pvtBreakDataView = new DataView(pvtDataSet.Tables["Break"],
                "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND BREAK_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'",
                "BREAK_TIME_IN_MINUTES,BREAK_TIME_OUT_MINUTES",
                DataViewRowState.CurrentRows);

            Clear_DataGridView(dgvBreakDataGridView);

            //Set Indicator
            for (int intRow = 0; intRow < this.pvtBreakDataView.Count; intRow++)
            {
                if (pvtBreakDataView[intRow]["CLOCKED_TIME_IN_MINUTES"] == System.DBNull.Value)
                {
                    strClockTimeIn = "";
                }
                else
                {
                    if (Convert.ToInt32(pvtBreakDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) > 59)
                    {
                        strClockTimeIn = Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) / 60).ToString()
                                                               + Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strClockTimeIn = Convert.ToInt32(pvtBreakDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]).ToString();
                    }
                }

                if (pvtBreakDataView[intRow]["BREAK_TIME_IN_MINUTES"] != System.DBNull.Value)
                {
                    if (Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_TIME_IN_MINUTES"]) > 59)
                    {
                        strActualTimeIn = Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_TIME_IN_MINUTES"]) / 60).ToString()
                                                               + Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_TIME_IN_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strActualTimeIn = Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_TIME_IN_MINUTES"]).ToString();
                    }
                }
                else
                {
                    strActualTimeIn = "";
                }

                if (pvtBreakDataView[intRow]["BREAK_TIME_OUT_MINUTES"] != System.DBNull.Value)
                {
                    if (Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_TIME_OUT_MINUTES"]) > 59)
                    {
                        strActualTimeOut = Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_TIME_OUT_MINUTES"]) / 60).ToString()
                                                               + Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_TIME_OUT_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strActualTimeOut = Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_TIME_OUT_MINUTES"]).ToString();
                    }
                }
                else
                {
                    strActualTimeOut = "";
                }

                if (pvtBreakDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"] == System.DBNull.Value)
                {
                    strClockTimeOut = "";
                }
                else
                {
                    if (Convert.ToInt32(pvtBreakDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) > 59)
                    {
                        strClockTimeOut = Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) / 60).ToString()
                                                               + Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strClockTimeOut = Convert.ToInt32(pvtBreakDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]).ToString();
                    }
                }

                //Remove Row 
                if (strActualTimeIn == ""
                    && strActualTimeOut == "")
                {
                    this.pvtBreakDataView[intRow].Delete();
                    intRow -= 1;
                    continue;
                }

                this.dgvBreakDataGridView.Rows.Add("",
                                                   "",
                                                   strClockTimeIn,
                                                   strActualTimeIn,
                                                   strActualTimeOut,
                                                   strClockTimeOut,
                                                   Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_ACCUM_MINUTES"]) / 60).ToString() + ":"
                                                   + Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_ACCUM_MINUTES"]) % 60).ToString("00"),
                                                   pvtBreakDataView[intRow]["BREAK_SEQ"].ToString());

                if (pvtBreakDataView[intRow]["INDICATOR"].ToString() == "X")
                {
                    dgvBreakDataGridView[pvtintIndicatorColTimeSheetOrBreakDataGridView, dgvBreakDataGridView.Rows.Count - 1].Style = ErrorDataGridViewCellStyle;

                    pvtblnBreakInError = true;
                }

                //ELR - 20150516
                if (pvtBreakDataView[intRow]["INCLUDED_IN_RUN_IND"].ToString() == "N")
                {
                    dgvBreakDataGridView[pvtintExcludedFromRunColTimeSheetOrBreakDataGridView, this.dgvBreakDataGridView.Rows.Count - 1].Style = NotIncludedInRunDataGridViewCellStyle;
                }

                if (pvtBreakDataView[intRow]["INCLUDED_IN_RUN_IND"].ToString() == "N"
                && pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["RUN_IND"].ToString() == "Y")
                {
                }
                else
                {
                    pvtintTotalBreakMinutes += Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_ACCUM_MINUTES"]);
                }
            }

            if (pvtintTotalBreakMinutes == 0)
            {
                this.dgvBreakTotalsDataGridView[2, 0].Value = "0:00";
            }
            else
            {
                this.dgvBreakTotalsDataGridView[2, 0].Value = Convert.ToInt32(pvtintTotalBreakMinutes / 60).ToString() + ":" + Convert.ToInt32(pvtintTotalBreakMinutes % 60).ToString("00");
            }

            pvtTimeSheetDataView = null;
            pvtTimeSheetDataView = new DataView(pvtDataSet.Tables["TimeSheet"],
                "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND TIMESHEET_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'",
                "TIMESHEET_TIME_IN_MINUTES,TIMESHEET_TIME_OUT_MINUTES",
                DataViewRowState.CurrentRows);

            Clear_DataGridView(dgvTimeSheetDataGridView);

            //Set Indicator
            for (int intRow = 0; intRow < this.pvtTimeSheetDataView.Count; intRow++)
            {
                if (pvtTimeSheetDataView[intRow]["CLOCKED_TIME_IN_MINUTES"] == System.DBNull.Value)
                {
                    strClockTimeIn = "";
                }
                else
                {
                    if (Convert.ToInt32(pvtTimeSheetDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) > 59)
                    {
                        strClockTimeIn = Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) / 60).ToString()
                                                                       + Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strClockTimeIn = Convert.ToInt32(pvtTimeSheetDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]).ToString();
                    }
                }

                if (pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_IN_MINUTES"] != System.DBNull.Value)
                {
                    if (Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_IN_MINUTES"]) > 59)
                    {
                        strActualTimeIn = Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_IN_MINUTES"]) / 60).ToString()
                                                                       + Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_IN_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strActualTimeIn = Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_IN_MINUTES"]).ToString();
                    }
                }
                else
                {
                    strActualTimeIn = "";
                }

                if (pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_OUT_MINUTES"] != System.DBNull.Value)
                {
                    if (Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_OUT_MINUTES"]) > 59)
                    {
                        strActualTimeOut = Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_OUT_MINUTES"]) / 60).ToString()
                                                                       + Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_OUT_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strActualTimeOut = Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_OUT_MINUTES"]).ToString();
                    }
                }
                else
                {
                    strActualTimeOut = "";
                }

                if (pvtTimeSheetDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"] == System.DBNull.Value)
                {
                    strClockTimeOut = "";
                }
                else
                {
                    if (Convert.ToInt32(pvtTimeSheetDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) > 59)
                    {
                        strClockTimeOut = Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) / 60).ToString()
                                                                       + Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strClockTimeOut = Convert.ToInt32(pvtTimeSheetDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]).ToString();
                    }
                }

                //Remove Row 
                if (strActualTimeIn == ""
                    && strActualTimeOut == "")
                {
                    this.pvtTimeSheetDataView[intRow].Delete();
                    intRow -= 1;
                    continue;
                }

                this.dgvTimeSheetDataGridView.Rows.Add("",
                                                       "",
                                                       strClockTimeIn,
                                                       strActualTimeIn,
                                                       strActualTimeOut,
                                                       strClockTimeOut,
                                                       Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_ACCUM_MINUTES"]) / 60).ToString() + ":"
                                                       + Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_ACCUM_MINUTES"]) % 60).ToString("00"),
                                                       pvtTimeSheetDataView[intRow]["TIMESHEET_SEQ"].ToString());
                
                if (pvtTimeSheetDataView[intRow]["INDICATOR"].ToString() == "X")
                {
                    dgvTimeSheetDataGridView[pvtintIndicatorColTimeSheetOrBreakDataGridView, dgvTimeSheetDataGridView.Rows.Count - 1].Style = ErrorDataGridViewCellStyle;

                    pvtblnTimeSheetInError = true;
                }

                //ELR - 20150516
                if (pvtTimeSheetDataView[intRow]["INCLUDED_IN_RUN_IND"].ToString() == "N")
                {
                    dgvTimeSheetDataGridView[pvtintExcludedFromRunColTimeSheetOrBreakDataGridView, this.dgvTimeSheetDataGridView.Rows.Count - 1].Style = NotIncludedInRunDataGridViewCellStyle;
                }

                if (pvtTimeSheetDataView[intRow]["INCLUDED_IN_RUN_IND"].ToString() == "N"
                    && pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["RUN_IND"].ToString() == "Y")
                {
                }
                else
                {
                    pvtintTotalTimeSheetMinutes += Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_ACCUM_MINUTES"]);
                }
            }

            this.pvtblnBreakDataGridViewLoaded = true;
            this.pvtblnTimeSheetDataGridViewLoaded = true;

            if (this.dgvBreakDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvBreakDataGridView, 0);
            }

            //Set Break (NB intBreakMinutesDefault NOT Used in this Function)
            int intBreakMinutesDefault = 0;
            Set_Break_Value(ref intBreakMinutesDefault);


            if (this.grbBreakError.Visible == false)
            {
                if (pvtDayTotalDataView.Count == 1)
                {
                    if (pvtDayTotalDataView[0]["LEAVE_INDICATOR"].ToString() == "Y")
                    {
                        this.grbLeaveError.Visible = true;
                    }
                }
            }

            if (this.dgvTimeSheetDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvTimeSheetDataGridView, 0);
            }

            if (this.btnSave.Enabled == true)
            {
                Check_To_Add_New_TimeSheet_Row();
                Check_To_Add_New_Break_Row();
            }
        }

        private void dgvBreakRangeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private int Set_Break_Value(ref int intBreakTimeMinutesDefault)
        {
            //Set Break Value
            int intDayPaidHours = 0;
            int intBreakTimeMinutes = 0;
            intBreakTimeMinutesDefault = 0;

            this.grbBreakError.Visible = false;
            this.grbLeaveError.Visible = false;

            if (pvtBreakRangeDataView.Count > 0)
            {
                this.dgvBreakExceptionDataGridView[0, 0].Value = "=>";

                if (pvtintTotalTimeSheetMinutes == 0
                    & pvtintTotalBreakMinutes == 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvBreakRangeDataGridView, 0);

                    this.dgvTimeSheetTotalsDataGridView[0, 1].Value = "Break After 0:00";

                    dgvBreakTotalsDataGridView[0, 0].Style = LunchTotalDataGridViewCellStyle;

                    this.dgvBreakTotalsDataGridView[0, 0].Value = "";
                }
                else
                {
                    for (int intBreakRow = 0; intBreakRow < this.pvtBreakRangeDataView.Count; intBreakRow++)
                    {
                        if (Convert.ToInt32(pvtBreakRangeDataView[intBreakRow]["WORKED_TIME_MINUTES"]) <= pvtintTotalTimeSheetMinutes)
                        {
                            intBreakTimeMinutesDefault = Convert.ToInt32(pvtBreakRangeDataView[intBreakRow]["BREAK_MINUTES"]);
                            intBreakTimeMinutes = Convert.ToInt32(pvtBreakRangeDataView[intBreakRow]["BREAK_MINUTES"]);

                            this.dgvTimeSheetTotalsDataGridView[0, 1].Value = "Break After " + Convert.ToInt32(Convert.ToInt32(pvtBreakRangeDataView[intBreakRow]["WORKED_TIME_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtBreakRangeDataView[intBreakRow]["WORKED_TIME_MINUTES"]) % 60).ToString("00");

                            if (pvtintTotalTimeSheetMinutes < pvtintTotalBreakMinutes)
                            {
                                dgvBreakTotalsDataGridView[0, 0].Style = BreakExceptionDataGridViewCellStyle;
                                
                                this.dgvBreakTotalsDataGridView[0, 0].Value = "<=";

                                this.lblBreakHours.Text = "Total Break Hours " + Convert.ToInt32(pvtintTotalBreakMinutes / 60).ToString() + ":" + Convert.ToInt32(pvtintTotalBreakMinutes % 60).ToString("00");
                                this.lblTimesheetHours.Text = "Total Worked Hours " + Convert.ToInt32(pvtintTotalTimeSheetMinutes / 60).ToString() + ":" + Convert.ToInt32(pvtintTotalTimeSheetMinutes % 60).ToString("00");

                                this.dgvBreakExceptionDataGridView[0, 0].Value = "";

                                this.grbBreakError.Visible = true;
                            }
                            else
                            {
                                if (intBreakTimeMinutes < pvtintTotalBreakMinutes) 
                                {
                                    intBreakTimeMinutes = pvtintTotalBreakMinutes;

                                    dgvBreakTotalsDataGridView[0, 0].Style = BreakExceptionDataGridViewCellStyle;

                                    this.dgvBreakExceptionDataGridView[0, 0].Value = "";

                                    this.dgvBreakTotalsDataGridView[0, 0].Value = "<=";
                                }
                                else
                                {
                                    dgvBreakTotalsDataGridView[0, 0].Style = LunchTotalDataGridViewCellStyle;

                                    this.dgvBreakTotalsDataGridView[0, 0].Value = "";

                                    this.dgvBreakExceptionDataGridView[0, 0].Value = "=>";
                                }
                            }

                            this.Set_DataGridView_SelectedRowIndex(dgvBreakRangeDataGridView, intBreakRow);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                //Set All Break Values to Zero
                dgvBreakTotalsDataGridView[0, 0].Style = LunchTotalDataGridViewCellStyle;

                this.dgvBreakTotalsDataGridView[0, 0].Value = "";
                this.dgvTimeSheetTotalsDataGridView[0, 1].Value = "Break After 0:00";

                this.dgvBreakExceptionDataGridView[0, 0].Value = "";
            }

            this.dgvTimeSheetTotalsDataGridView[1, 0].Value = Convert.ToInt32(pvtintTotalTimeSheetMinutes / 60).ToString() + ":" + Convert.ToInt32(pvtintTotalTimeSheetMinutes % 60).ToString("00");

            if (pvtintTotalTimeSheetMinutes >= intBreakTimeMinutes)
            {
                intDayPaidHours = pvtintTotalTimeSheetMinutes - intBreakTimeMinutes;

                this.dgvTimeSheetTotalsDataGridView[1, 1].Value = Convert.ToInt32(intBreakTimeMinutes / 60).ToString() + ":" + Convert.ToInt32(intBreakTimeMinutes % 60).ToString("00");
                this.dgvTimeSheetTotalsDataGridView[1, 2].Value = Convert.ToInt32(intDayPaidHours / 60).ToString() + ":" + Convert.ToInt32(intDayPaidHours % 60).ToString("00");
            }
            else
            {
                this.dgvTimeSheetTotalsDataGridView[1, 1].Value = "0:00";
                this.dgvTimeSheetTotalsDataGridView[1, 2].Value = this.dgvTimeSheetTotalsDataGridView[1, 0].Value;

                intDayPaidHours = pvtintTotalTimeSheetMinutes;
            }

            //Round Day - Return 'intDayPaidHours'
            Round_For_Period(Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["DAILY_ROUNDING_IND"]), Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["DAILY_ROUNDING_MINUTES"]), ref intDayPaidHours);

            this.dgvTimeSheetTotalsDataGridView[1, 3].Value = Convert.ToInt32(intDayPaidHours / 60).ToString() + ":" + Convert.ToInt32(intDayPaidHours % 60).ToString("00");

            return intDayPaidHours;
        }

        private void Check_To_Add_New_TimeSheet_Row()
        {
            if (this.dgvTimeSheetDataGridView.Rows.Count > 0)
            {
                if (this.dgvTimeSheetDataGridView[pvtintClockInSetColTimeSheetOrBreakDataGridView, this.dgvTimeSheetDataGridView.Rows.Count - 1].Value.ToString() == ""
                    & this.dgvTimeSheetDataGridView[pvtintClockOutSetColTimeSheetOrBreakDataGridView, this.dgvTimeSheetDataGridView.Rows.Count - 1].Value.ToString() == "")
                {
                    return;
                }
            }

            DataView TempDataView = new DataView(this.pvtDataSet.Tables["TimeSheet"],
                "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND TIMESHEET_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "' AND INDICATOR = 'X'",
                "TIMESHEET_SEQ DESC",
                DataViewRowState.CurrentRows);

            if (TempDataView.Count == 0)
            {
                DataRowView drvDataRowView = this.pvtTimeSheetDataView.AddNew();

                drvDataRowView.BeginEdit();

                drvDataRowView["COMPANY_NO"] = pvtint64CompanyNo;
                //NB This Will Be Changed When A Time-In/Time-Out Is Added
                drvDataRowView["PAY_CATEGORY_NO"] = pvtintPayCategoryNo;
                drvDataRowView["PAY_CATEGORY_TYPE"] = pvtstrPayrollType;
                drvDataRowView["EMPLOYEE_NO"] = pvtintEmployeeNo;
                drvDataRowView["TIMESHEET_DATE"] = pvtDateTime;
                drvDataRowView["TIMESHEET_TIME_IN_MINUTES"] = System.DBNull.Value;
                drvDataRowView["TIMESHEET_TIME_OUT_MINUTES"] = System.DBNull.Value;
                drvDataRowView["TIMESHEET_ACCUM_MINUTES"] = 0;

                //Remove Filter
                TempDataView.RowFilter = TempDataView.RowFilter.Replace(" AND INDICATOR = 'X'", "");
                TempDataView.RowStateFilter = TempDataView.RowStateFilter | DataViewRowState.Deleted;

                if (TempDataView.Count == 0)
                {
                    drvDataRowView["TIMESHEET_SEQ"] = 1;
                }
                else
                {
                    drvDataRowView["TIMESHEET_SEQ"] = Convert.ToInt32(TempDataView[0]["TIMESHEET_SEQ"]) + 1;
                }

                TempDataView = null;

                this.pvtblnTimeSheetDataGridViewLoaded = false;

                this.dgvTimeSheetDataGridView.Rows.Add("",
                                                       "",
                                                       "",
                                                       "",
                                                       "",
                                                       "",
                                                       "0:00",
                                                       drvDataRowView["TIMESHEET_SEQ"].ToString());

                drvDataRowView.EndEdit();

                this.pvtblnTimeSheetDataGridViewLoaded = true;

                dgvTimeSheetDataGridView.CurrentCell = dgvTimeSheetDataGridView[pvtintClockInSetColTimeSheetOrBreakDataGridView, this.dgvTimeSheetDataGridView.Rows.Count - 1];

                SendKeys.Send("{Left}");
            }
            else
            {
                dgvTimeSheetDataGridView.CurrentCell = dgvTimeSheetDataGridView[pvtintClockInSetColTimeSheetOrBreakDataGridView, this.dgvTimeSheetDataGridView.Rows.Count - 1];
            }
        }

        private void Check_To_Add_New_Break_Row()
        {
            if (this.dgvBreakDataGridView.Rows.Count > 0)
            {
                if (this.dgvBreakDataGridView[pvtintClockInSetColTimeSheetOrBreakDataGridView, this.dgvBreakDataGridView.Rows.Count - 1].Value.ToString() == ""
                    & this.dgvBreakDataGridView[pvtintClockOutSetColTimeSheetOrBreakDataGridView, this.dgvBreakDataGridView.Rows.Count - 1].Value.ToString() == "")
                {
                    return;
                }
            }

            DataView myDataView = new DataView(this.pvtDataSet.Tables["Break"],
                "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND BREAK_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "' AND INDICATOR = 'X'",
                "BREAK_SEQ DESC",
                DataViewRowState.CurrentRows);

            if (myDataView.Count == 0)
            {
                DataRowView drvDataRowView = pvtBreakDataView.AddNew();

                drvDataRowView.BeginEdit();

                drvDataRowView["COMPANY_NO"] = pvtint64CompanyNo;
                //NB This Will Be Changed When A Time-In/Time-Out Is Added
                drvDataRowView["PAY_CATEGORY_NO"] = pvtintPayCategoryNo;
                drvDataRowView["PAY_CATEGORY_TYPE"] = pvtstrPayrollType;
                drvDataRowView["EMPLOYEE_NO"] = pvtintEmployeeNo;
                drvDataRowView["BREAK_DATE"] = pvtDateTime;
                drvDataRowView["BREAK_TIME_IN_MINUTES"] = System.DBNull.Value;
                drvDataRowView["BREAK_TIME_OUT_MINUTES"] = System.DBNull.Value;
                drvDataRowView["BREAK_ACCUM_MINUTES"] = 0;

                int intBreakSeq = 1;

                //Remove Filter
                myDataView.RowFilter = myDataView.RowFilter.Replace(" AND INDICATOR = 'X'", "");
                myDataView.RowStateFilter = myDataView.RowStateFilter | DataViewRowState.Deleted;

                if (myDataView.Count == 0)
                {
                    drvDataRowView["BREAK_SEQ"] = intBreakSeq;
                }
                else
                {
                    intBreakSeq = Convert.ToInt32(myDataView[0]["BREAK_SEQ"]) + 1;
                    drvDataRowView["BREAK_SEQ"] = intBreakSeq;
                }

                myDataView = null;

                this.pvtblnBreakDataGridViewLoaded = false;

                this.dgvBreakDataGridView.Rows.Add("",
                                                   "",
                                                   "",
                                                   "",
                                                   "",
                                                   "",
                                                   "0:00",
                                                   drvDataRowView["BREAK_SEQ"].ToString());

                drvDataRowView.EndEdit();

                this.pvtblnBreakDataGridViewLoaded = true;

                dgvBreakDataGridView.CurrentCell = dgvBreakDataGridView[pvtintClockInSetColTimeSheetOrBreakDataGridView, this.dgvBreakDataGridView.Rows.Count - 1];

                SendKeys.Send("{Left}");
            }
            else
            {
                dgvBreakDataGridView.CurrentCell = dgvBreakDataGridView[pvtintClockInSetColTimeSheetOrBreakDataGridView, this.dgvBreakDataGridView.Rows.Count - 1];
            }
        }

        private void Round_For_Period(int parintRoundInd, int parintRoundValue, ref int parintTotal)
        {
            if (parintRoundInd == 0)
            {
            }
            else
            {
                if (parintTotal % parintRoundValue == 0)
                {
                }
                else
                {
                    //Up
                    if (parintRoundInd == 1)
                    {
                        parintTotal = parintTotal + (parintRoundValue - (parintTotal % parintRoundValue));
                    }
                    else
                    {
                        //Down
                        if (parintRoundInd == 2)
                        {
                            parintTotal = parintTotal - (parintTotal % parintRoundValue);
                        }
                        else
                        {
                            //Closest
                            if (parintTotal % parintRoundValue >= Convert.ToDouble(parintRoundValue) / 2)
                            {
                                //Up
                                parintTotal = parintTotal + (parintRoundValue - (parintTotal % parintRoundValue));
                            }
                            else
                            {
                                //Down
                                parintTotal = parintTotal - (parintTotal % parintRoundValue);
                            }
                        }
                    }
                }
            }
        }

        private void dgvTimeSheetDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void dgvBreakDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void TimeSheet_Break_DataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Cell_KeyPress);
            e.Control.KeyPress += new KeyPressEventHandler(Cell_KeyPress);
        }

        private void TimeSheet_Or_Break_DataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (this.btnSave.Enabled == true
                && pvtblnAllowToEnter == true)
            {
                DataGridView myDataGridView = (DataGridView)sender;

                //Stop ReEntrant Call
                if (myDataGridView[e.ColumnIndex, e.RowIndex].Selected == false)
                {
                    return;
                }
                
                if (e.ColumnIndex == pvtintClockInSetColTimeSheetOrBreakDataGridView
                || e.ColumnIndex == pvtintClockOutSetColTimeSheetOrBreakDataGridView)
                {
                    if (myDataGridView[e.ColumnIndex, e.RowIndex].Value == null)
                    {
                        //Causes ReEntry to This Function
                        pvtblnAllowToEnter = false;
                        myDataGridView[e.ColumnIndex, e.RowIndex].Value = "";
                        pvtblnAllowToEnter = true;
                    }

                    if (myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString() == ""
                    | myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString() == "2400")
                    {
                    }
                    else
                    {
                        if (myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Length > 2)
                        {
                            if (Convert.ToInt32(myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Substring(0, myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Length - 2)) > 23
                                | Convert.ToInt32(myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Substring(myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Length - 2, 2)) > 59)
                            {
                                //Causes ReEntry to This Function
                                pvtblnAllowToEnter = false;
                                myDataGridView[e.ColumnIndex, e.RowIndex].Value = "";
                                pvtblnAllowToEnter = true;
                                System.Console.Beep();
                            }
                        }
                        else
                        {
                            if (Convert.ToInt32(myDataGridView[e.ColumnIndex, e.RowIndex].Value) > 59)
                            {
                                //Causes ReEntry to This Function
                                pvtblnAllowToEnter = false;
                                myDataGridView[e.ColumnIndex, e.RowIndex].Value = "";
                                pvtblnAllowToEnter = true;
                                System.Console.Beep();
                            }
                        }
                    }

                    int intTimeMinutes = 0;
                    string strFieldPrefix = "";
                    string strInOutDef = "";
                    string strRowValue = "";
                
                    DataView myDataView = null;

                    if (myDataGridView.Name == "dgvBreakDataGridView")
                    {
                        strRowValue = " AND BREAK_SEQ = " + myDataGridView[pvtintSeqNoColTimeSheetOrBreakDataGridView, e.RowIndex].Value.ToString();

                        myDataView = new DataView(this.pvtDataSet.Tables["Break"],
                        "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND BREAK_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'" + strRowValue,
                        "BREAK_TIME_IN_MINUTES,BREAK_TIME_OUT_MINUTES",
                        DataViewRowState.CurrentRows);

                        strFieldPrefix = "BREAK";
                    }
                    else
                    {
                        strRowValue = " AND TIMESHEET_SEQ = " + myDataGridView[pvtintSeqNoColTimeSheetOrBreakDataGridView, e.RowIndex].Value.ToString();

                        myDataView = new DataView(this.pvtDataSet.Tables["TimeSheet"],
                        "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND TIMESHEET_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'" + strRowValue,
                        "TIMESHEET_TIME_IN_MINUTES,TIMESHEET_TIME_OUT_MINUTES",
                        DataViewRowState.CurrentRows);

                        strFieldPrefix = "TIMESHEET";
                    }

                    if (e.ColumnIndex == pvtintClockInSetColTimeSheetOrBreakDataGridView)
                    {
                        strInOutDef = "IN";
                    }
                    else
                    {
                        strInOutDef = "OUT";
                    }

                    if (myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString() == "")
                    {
                        myDataView[0][strFieldPrefix + "_TIME_" + strInOutDef + "_MINUTES"] = System.DBNull.Value;

                        int intCheckCol = pvtintClockOutSetColTimeSheetOrBreakDataGridView;

                        if (e.ColumnIndex == pvtintClockOutSetColTimeSheetOrBreakDataGridView)
                        {
                            intCheckCol = pvtintClockInSetColTimeSheetOrBreakDataGridView;
                        }

                        //On Delete of Value it Can be null
                        if (myDataGridView[intCheckCol, e.RowIndex].Value == null)
                        {
                            myDataView[0]["INDICATOR"] = "";
                        }
                        else
                        {
                            if (myDataGridView[intCheckCol, e.RowIndex].Value.ToString() == "")
                            {
                                myDataView[0]["INDICATOR"] = "";
                            }
                            else
                            {
                                myDataView[0]["INDICATOR"] = "X";
                            }
                        }

                        myDataView[0][strFieldPrefix + "_ACCUM_MINUTES"] = 0;
                    }
                    else
                    {
                        if (myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Length >= 3)
                        {
                            intTimeMinutes = (Convert.ToInt32(myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Substring(0, myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Length - 2)) * 60)
                                                + Convert.ToInt32(myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Substring(myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Length - 2, 2));
                        }
                        else
                        {
                            intTimeMinutes = Convert.ToInt32(myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString());
                        }

                        myDataView[0][strFieldPrefix + "_TIME_" + strInOutDef + "_MINUTES"] = intTimeMinutes;
                    }

                    if (myDataView[0][strFieldPrefix + "_TIME_IN_MINUTES"] == System.DBNull.Value
                            | myDataView[0][strFieldPrefix + "_TIME_OUT_MINUTES"] == System.DBNull.Value)
                    {
                        if (myDataView[0][strFieldPrefix + "_TIME_IN_MINUTES"] == System.DBNull.Value
                            & myDataView[0][strFieldPrefix + "_TIME_OUT_MINUTES"] == System.DBNull.Value)
                        {
                        }
                        else
                        {
                            myDataView[0]["INDICATOR"] = "X";
                            myDataView[0][strFieldPrefix + "_ACCUM_MINUTES"] = 0;
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(myDataView[0][strFieldPrefix + "_TIME_OUT_MINUTES"]) > Convert.ToInt32(myDataView[0][strFieldPrefix + "_TIME_IN_MINUTES"]))
                        {
                            //Correct
                            myDataView[0]["INDICATOR"] = "";
                            myDataView[0][strFieldPrefix + "_ACCUM_MINUTES"] = Convert.ToInt32(myDataView[0][strFieldPrefix + "_TIME_OUT_MINUTES"]) - Convert.ToInt32(myDataView[0][strFieldPrefix + "_TIME_IN_MINUTES"]);
                        }
                        else
                        {
                            myDataView[0]["INDICATOR"] = "X";
                            myDataView[0][strFieldPrefix + "_ACCUM_MINUTES"] = 0;
                        }
                    }

                    //Get All Rows for Current Employee / Date
                    myDataView.RowFilter = myDataView.RowFilter.Replace(strRowValue, "");

                    RePaint_SpreadSheet_Indicators(myDataGridView, myDataView);
                }
            }
        }

        private void RePaint_SpreadSheet_Indicators(DataGridView myDataGridView, DataView myDataView)
        {
            int intTotalTimeMinutes = 0;
            bool blnError = false;
            string strFieldPrefix = "";

            if (myDataGridView.Name == "dgvBreakDataGridView")
            {
                this.dgvBreakDataGridView.Rows.Clear();
                strFieldPrefix = "BREAK";
                pvtblnBreakInError = false;
            }
            else
            {
                this.dgvTimeSheetDataGridView.Rows.Clear();
                strFieldPrefix = "TIMESHEET";
                pvtblnTimeSheetInError = false;
            }
    
            bool blnRowExistsWithInAndOutValues = false;

            string strClockTimeIn = "";
            string strActualTimeIn = "";
            string strActualTimeOut = "";
            string strClockTimeOut = "";
                                                     
            //Check For OverLap Errors
            for (int intRow = 0; intRow < myDataView.Count; intRow++)
            {
                if (myDataView[intRow][strFieldPrefix + "_TIME_IN_MINUTES"] == System.DBNull.Value
                 && myDataView[intRow][strFieldPrefix + "_TIME_OUT_MINUTES"] == System.DBNull.Value)
                {
                    myDataView[intRow].Delete();

                    intRow -= 1;

                    continue;
                }

                blnError = false;
                intTotalTimeMinutes += Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_ACCUM_MINUTES"]);

                if (intRow > 0)
                {
                    //Check OVerlap
                    if (myDataView[intRow][strFieldPrefix + "_TIME_IN_MINUTES"] == System.DBNull.Value
                        | myDataView[intRow - 1][strFieldPrefix + "_TIME_OUT_MINUTES"] == System.DBNull.Value)
                    {
                    }
                    else
                    {
                        if (Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_TIME_IN_MINUTES"]) < Convert.ToInt32(myDataView[intRow - 1][strFieldPrefix + "_TIME_OUT_MINUTES"]))
                        {
                            //Current Row
                            blnError = true;
                        }
                    }
                }

                if (myDataView[intRow][strFieldPrefix + "_TIME_IN_MINUTES"] == System.DBNull.Value
                    | myDataView[intRow][strFieldPrefix + "_TIME_OUT_MINUTES"] == System.DBNull.Value)
                {
                    if (myDataView[intRow][strFieldPrefix + "_TIME_IN_MINUTES"] == System.DBNull.Value
                    & myDataView[intRow][strFieldPrefix + "_TIME_OUT_MINUTES"] == System.DBNull.Value)
                    {
                    }
                    else
                    {
                        //Error
                        blnError = true;
                    }
                }
                else
                {
                    blnRowExistsWithInAndOutValues = true;

                    if (Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_TIME_IN_MINUTES"]) >= Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_TIME_OUT_MINUTES"]))
                    {
                        blnError = true;
                    }
                }

                if (myDataView[intRow]["CLOCKED_TIME_IN_MINUTES"] == System.DBNull.Value)
                {
                    strClockTimeIn = "";
                }
                else
                {
                    if (Convert.ToInt32(myDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) > 59)
                    {
                        strClockTimeIn = Convert.ToInt32(Convert.ToInt32(myDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) / 60).ToString()
                                                                + Convert.ToInt32(Convert.ToInt32(myDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strClockTimeIn = Convert.ToInt32(myDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]).ToString();
                    }
                }

                if (myDataView[intRow][strFieldPrefix + "_TIME_IN_MINUTES"] != System.DBNull.Value)
                {
                    if (Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_TIME_IN_MINUTES"]) > 59)
                    {
                        strActualTimeIn = Convert.ToInt32(Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_TIME_IN_MINUTES"]) / 60).ToString()
                                                                + Convert.ToInt32(Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_TIME_IN_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strActualTimeIn = Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_TIME_IN_MINUTES"]).ToString();
                    }
                }
                else
                {
                    strActualTimeIn = "";
                }

                if (myDataView[intRow][strFieldPrefix + "_TIME_OUT_MINUTES"] != System.DBNull.Value)
                {
                    if (Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_TIME_OUT_MINUTES"]) > 59)
                    {
                        strActualTimeOut = Convert.ToInt32(Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_TIME_OUT_MINUTES"]) / 60).ToString()
                                                                + Convert.ToInt32(Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_TIME_OUT_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strActualTimeOut = Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_TIME_OUT_MINUTES"]).ToString();
                    }
                }
                else
                {
                    strActualTimeOut = "";
                }

                if (myDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"] == System.DBNull.Value)
                {
                    strClockTimeOut = "";
                }
                else
                {
                    if (Convert.ToInt32(myDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) > 59)
                    {
                        strClockTimeOut = Convert.ToInt32(Convert.ToInt32(myDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) / 60).ToString()
                                                                + Convert.ToInt32(Convert.ToInt32(myDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strClockTimeOut = Convert.ToInt32(myDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]).ToString();
                    }
                }

                myDataGridView.Rows.Add("",
                                        "",
                                        strClockTimeIn,
                                        strActualTimeIn,
                                        strActualTimeOut,
                                        strClockTimeOut,
                                        Convert.ToInt32(Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_ACCUM_MINUTES"]) / 60).ToString() + ":"
                                        + Convert.ToInt32(Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_ACCUM_MINUTES"]) % 60).ToString("00"),
                                        myDataView[intRow][strFieldPrefix + "_SEQ"].ToString());

                if (blnError == true)
                {
                    myDataGridView[0,intRow].Style = ErrorDataGridViewCellStyle;

                    myDataView[intRow]["INDICATOR"] = "X";

                    if (myDataGridView.Name == "dgvBreakDataGridView")
                    {
                        this.pvtblnBreakInError = true;
                    }
                    else
                    {
                        this.pvtblnTimeSheetInError = true;
                    }
                }
                else
                {
                    myDataGridView[0,intRow].Style = NormalDataGridViewCellStyle;

                    myDataView[intRow]["INDICATOR"] = "";
                }
            }

            if (myDataGridView.Name == "dgvBreakDataGridView")
            {
                pvtintTotalBreakMinutes = intTotalTimeMinutes;
                this.dgvBreakTotalsDataGridView[2, 0].Value = Convert.ToInt32(intTotalTimeMinutes / 60).ToString() + ":" + Convert.ToInt32(intTotalTimeMinutes % 60).ToString("00");
            }
            else
            {
                pvtintTotalTimeSheetMinutes = intTotalTimeMinutes;
            }

            int intBreakMinutesDefault = 0;

            int intDayPaidMinutes = Set_Break_Value(ref intBreakMinutesDefault);
            string strIndicator = "";

            if (pvtDayTotalDataView.Count == 0)
            {
                //No Record because all Related Tables were Empty
                Add_DayTotal_Record();
            }
            else
            {
                if (pvtDayTotalDataView.Count > 1)
                {
                    CustomMessageBox.Show("Stop There is an Error",this.Text,MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }

            pvtDayTotalDataView[0]["DAY_PAID_MINUTES"] = intDayPaidMinutes;
            pvtDayTotalDataView[0]["BREAK_ACCUM_MINUTES"] = pvtintTotalBreakMinutes;

            if (this.rbnEmployeeDate.Checked == true)
            {
                this.dgvDayDataGridView[pvtintNameColDayDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Value = Convert.ToInt32(intDayPaidMinutes / 60).ToString() + ":" + Convert.ToInt32(intDayPaidMinutes % 60).ToString("00");
                this.dgvDayDataGridView[pvtintExceptionColDayDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Value = Convert.ToInt32(pvtintTotalBreakMinutes / 60).ToString() + ":" + Convert.ToInt32(pvtintTotalBreakMinutes % 60).ToString("00");
            }
            else
            {
                this.dgvDayDataGridView[pvtintTotalHoursColDayDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Value = Convert.ToInt32(intDayPaidMinutes / 60).ToString() + ":" + Convert.ToInt32(intDayPaidMinutes % 60).ToString("00");
                this.dgvDayDataGridView[pvtintBreakHoursColDayDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Value = Convert.ToInt32(pvtintTotalBreakMinutes / 60).ToString() + ":" + Convert.ToInt32(pvtintTotalBreakMinutes % 60).ToString("00");
            }

            if ((this.pvtblnBreakInError == true
            | this.pvtblnTimeSheetInError == true)
            | pvtintTotalTimeSheetMinutes < pvtintTotalBreakMinutes)
            {
                strIndicator = "X";
            }
            else
            {
                if (intDayPaidMinutes == 0
                & pvtintTotalTimeSheetMinutes == 0
                & pvtintTotalBreakMinutes == 0)
                {
                    if (blnRowExistsWithInAndOutValues == true)
                    {
                        //Row Where IN and Out have same Value (therefore Accum = 0)
                        strIndicator = "E";
                    }
                    else
                    {
                        //Remove DayTotal Record So That we can Get Blank again
                        pvtDayTotalDataView[0].Delete();

                        this.dgvDayDataGridView[pvtintIndicatorColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Style = NoRecordDataGridViewCellStyle;

                        if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_PERIOD_DATE"] == System.DBNull.Value)
                        {
                            this.dgvDayDataGridView[pvtintRunColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Style = NormalDataGridViewCellStyle;
                        }
                        else
                        {
                            if (rbnGreaterWageDate.Checked == true)
                            {
                                this.dgvDayDataGridView[pvtintRunColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Style = NormalDataGridViewCellStyle;
                            }
                            else
                            {
                                this.dgvDayDataGridView[pvtintRunColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Style = this.PayrollPendingDataGridViewCellStyle;
                            }
                        }

                        this.dgvDayDataGridView[pvtintBreakExceptionColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Style = NormalDataGridViewCellStyle;

                        goto RePaint_SpreadSheet_Indicators_Continue;
                    }
                }
                else
                {
                    if (Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + pvtDateTime.ToString("ddd").ToUpper() + "_BELOW_MINUTES"]) == 0)
                    {
                        strIndicator = "E";
                    }
                    else
                    {
                        if (intDayPaidMinutes < Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + pvtDateTime.ToString("ddd").ToUpper() + "_BELOW_MINUTES"])
                            | intDayPaidMinutes > Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + pvtDateTime.ToString("ddd").ToUpper() + "_ABOVE_MINUTES"]))
                        {
                            if (intDayPaidMinutes != 0
                                | (pvtintTotalTimeSheetMinutes > 0
                                & intDayPaidMinutes == 0))
                            {
                                strIndicator = "E";
                            }
                        }
                    }
                }
            }
            
            //2017-09-21
            DataView EmployeeLeaveDataView = new DataView(this.pvtDataSet.Tables["EmployeeLeave"],
                                                  "PAY_CATEGORY_TYPE = '" + this.pvtstrPayrollType + "' AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND EMPLOYEE_NO = " + this.pvtintEmployeeNo + " AND DAY_DATE = '" + this.pvtDateTime.ToString("yyyy-MM-dd") + "'",
                                                  "",
                                                  DataViewRowState.CurrentRows);

            if (EmployeeLeaveDataView.Count > 0)
            {
                this.grbLeaveError.Visible = true;
                strIndicator = "X";
            }
            else
            {
                this.grbLeaveError.Visible = false;
            }
            
            //Set Colour For Day Spreadsheet
            pvtDayTotalDataView[0]["INDICATOR"] = strIndicator;

            if (strIndicator == "X")
            {
                this.dgvDayDataGridView[pvtintIndicatorColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Style = ErrorDataGridViewCellStyle;
            }
            else
            {
                if (strIndicator == "E")
                {
                    this.dgvDayDataGridView[pvtintIndicatorColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Style = ExceptionDataGridViewCellStyle;
                }
                else
                {
                    if (strIndicator == "B")
                    {
                        this.dgvDayDataGridView[pvtintIndicatorColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Style = NoRecordDataGridViewCellStyle;
                    }
                    else
                    {
                        if (strIndicator == "")
                        {
                            this.dgvDayDataGridView[pvtintIndicatorColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Style = NormalDataGridViewCellStyle;
                        }
                    }
                }
            }

            if (pvtintTotalBreakMinutes > intBreakMinutesDefault)
            {
                pvtDayTotalDataView[0]["BREAK_INDICATOR"] = "Y";

                this.dgvDayDataGridView[pvtintBreakExceptionColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Style = BreakExceptionDataGridViewCellStyle;
            }
            else
            {
                pvtDayTotalDataView[0]["BREAK_INDICATOR"] = "";

                this.dgvDayDataGridView[pvtintBreakExceptionColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Style = NormalDataGridViewCellStyle;
            }

        RePaint_SpreadSheet_Indicators_Continue:

            //Set Colour For Employee Spreadsheet
            if (rbnEmployeeDate.Checked == true)
            {
                pvtTempDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                    pvtstrPayCategoryFilter + " AND EMPLOYEE_NO = " + this.pvtintEmployeeNo.ToString() + " " + pvtstrDataAndTypeFilter + pvtstrCategoryType,
                    "INDICATOR DESC",
                    DataViewRowState.CurrentRows);
            }
            else
            {
                pvtTempDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                    pvtstrPayCategoryFilter + " AND DAY_DATE = '" + this.pvtDateTime.ToString("yyyy-MM-dd") + "' " + pvtstrDataAndTypeFilter + pvtstrCategoryType,
                    "INDICATOR DESC",
                        DataViewRowState.CurrentRows);
            }

            if (pvtTempDataView.Count > 0)
            {
                if (pvtTempDataView[0]["INDICATOR"].ToString() == "X")
                {
                    this.dgvEmployeeDataGridView[pvtintIndicatorColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Style = ErrorDataGridViewCellStyle;
                }
                else
                {
                    if (pvtTempDataView[0]["INDICATOR"].ToString() == "E")
                    {
                        this.dgvEmployeeDataGridView[pvtintIndicatorColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Style = ExceptionDataGridViewCellStyle;
                    }
                    else
                    {
                        if (pvtTempDataView[0]["INDICATOR"].ToString() == "B")
                        {
                            this.dgvEmployeeDataGridView[pvtintIndicatorColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Style = NoRecordDataGridViewCellStyle;
                        }
                        else
                        {
                            if (pvtTempDataView[0]["INDICATOR"].ToString() == "")
                            {
                                this.dgvEmployeeDataGridView[pvtintIndicatorColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Style = NormalDataGridViewCellStyle;
                            }
                        }
                    }
                }

                pvtTempDataView.Sort = "BREAK_INDICATOR DESC";

                if (pvtTempDataView[0]["BREAK_INDICATOR"].ToString() == "Y")
                {
                    dgvEmployeeDataGridView[pvtintBreakExceptionColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Style = BreakExceptionDataGridViewCellStyle;
                }
                else
                {
                    dgvEmployeeDataGridView[pvtintBreakExceptionColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Style = NormalDataGridViewCellStyle;
                }
            }
            else
            {
                this.dgvEmployeeDataGridView[pvtintIndicatorColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Style = NoRecordDataGridViewCellStyle;

                if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_PERIOD_DATE"] == System.DBNull.Value)
                {
                    dgvEmployeeDataGridView[pvtintRunColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Style = NormalDataGridViewCellStyle;
                }
                else
                {
                    if (rbnGreaterWageDate.Checked == true)
                    {
                        dgvEmployeeDataGridView[pvtintRunColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Style = NormalDataGridViewCellStyle;
                    }
                    else
                    {
                        dgvEmployeeDataGridView[pvtintRunColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Style = this.PayrollPendingDataGridViewCellStyle;
                    }
                }

                dgvEmployeeDataGridView[pvtintBreakExceptionColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Style = NormalDataGridViewCellStyle;
            }

            //Set Colour For PayCategory Spreadsheet
            pvtTempDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
            pvtstrPayCategoryFilter + pvtstrDataAndTypeFilter + pvtstrCategoryType,
                    "INDICATOR DESC",
                    DataViewRowState.CurrentRows);

            if (pvtTempDataView.Count > 0)
            {
                if (pvtTempDataView[0]["INDICATOR"].ToString() == "X")
                {
                    this.dgvPayCategoryDataGridView[pvtintIndicatorColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = ErrorDataGridViewCellStyle;
                }
                else
                {
                    if (pvtTempDataView[0]["INDICATOR"].ToString() == "E")
                    {
                        this.dgvPayCategoryDataGridView[pvtintIndicatorColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = ExceptionDataGridViewCellStyle;
                    }
                    else
                    {
                        if (pvtTempDataView[0]["INDICATOR"].ToString() == "B")
                        {
                            this.dgvPayCategoryDataGridView[pvtintIndicatorColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = NoRecordDataGridViewCellStyle;
                        }
                        else
                        {
                            if (pvtTempDataView[0]["INDICATOR"].ToString() == "")
                            {
                                this.dgvPayCategoryDataGridView[pvtintIndicatorColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = NormalDataGridViewCellStyle;
                            }
                        }
                    }
                }

                pvtTempDataView.Sort = "BREAK_INDICATOR DESC";

                if (pvtTempDataView[0]["BREAK_INDICATOR"].ToString() == "Y")
                {
                    this.dgvPayCategoryDataGridView[pvtintBreakExceptionColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = BreakExceptionDataGridViewCellStyle;
                }
                else
                {
                    this.dgvPayCategoryDataGridView[pvtintBreakExceptionColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = NormalDataGridViewCellStyle;
                }
            }
            else
            {
                this.dgvPayCategoryDataGridView[pvtintIndicatorColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = NoRecordDataGridViewCellStyle;

                if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_PERIOD_DATE"] == System.DBNull.Value)
                {
                    dgvPayCategoryDataGridView[pvtintRunColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = NormalDataGridViewCellStyle;
                }
                else
                {
                    if (rbnGreaterWageDate.Checked == true)
                    {
                        dgvPayCategoryDataGridView[pvtintRunColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = NormalDataGridViewCellStyle;
                    }
                    else
                    {
                        dgvPayCategoryDataGridView[pvtintRunColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = this.PayrollPendingDataGridViewCellStyle;
                    }
                }

                this.dgvPayCategoryDataGridView[pvtintBreakExceptionColDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = NormalDataGridViewCellStyle;
            }

            if (myDataGridView.Name == "dgvBreakDataGridView")
            {
                this.Check_To_Add_New_Break_Row();
            }
            else
            {
                this.Check_To_Add_New_TimeSheet_Row();
            }
        }

        private void Add_DayTotal_Record()
        {
            DataRowView drvDataRowView = pvtDayTotalDataView.AddNew();

            drvDataRowView.BeginEdit();

            drvDataRowView["COMPANY_NO"] = pvtint64CompanyNo;
            drvDataRowView["PAY_CATEGORY_TYPE"] = pvtstrPayrollType;
            drvDataRowView["PAY_CATEGORY_NO"] = pvtintPayCategoryNo;
            drvDataRowView["EMPLOYEE_NO"] = pvtintEmployeeNo;
            drvDataRowView["DAY_DATE"] = pvtDateTime;

            drvDataRowView["DAY_NO"] = pvtDateTime.DayOfWeek;
            drvDataRowView["DAY_PAID_MINUTES"] = 0;
            drvDataRowView["INDICATOR"] = "";

            //Ended Here so that TIMESHEET_SEQ can be Added to TimeSheet Row
            drvDataRowView.EndEdit();
        }

        private void TimeSheet_Or_Break_DataGridView_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (e.RowIndex > -1
                & e.ColumnIndex > -1)
                {
                    if (e.ColumnIndex == pvtintClockInSetColTimeSheetOrBreakDataGridView
                        | e.ColumnIndex == pvtintClockOutSetColTimeSheetOrBreakDataGridView)
                    {
                        this.Cursor = System.Windows.Forms.Cursors.IBeam;
                    }
                    else
                    {
                        this.Cursor = Cursors.No;
                    }
                }
                else
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void TimeSheet_Or_Break_DataGridView_MouseLeave(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void btnDeleteRow_Click(object sender, EventArgs e)
        {
            string strRowValue = " AND TIMESHEET_SEQ = " + this.dgvTimeSheetDataGridView[pvtintSeqNoColTimeSheetOrBreakDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvTimeSheetDataGridView)].Value.ToString();

            DataView myDataView = new DataView(this.pvtDataSet.Tables["TimeSheet"],
            "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND TIMESHEET_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'" + strRowValue,
            "TIMESHEET_SEQ",
            DataViewRowState.CurrentRows);

            myDataView.Delete(0);

            //Get All Rows for Current Employee / Date
            myDataView.RowFilter = myDataView.RowFilter.Replace(strRowValue, "");

            RePaint_SpreadSheet_Indicators(dgvTimeSheetDataGridView, myDataView);
        }

        private void btnDeleteBreakRow_Click(object sender, EventArgs e)
        {
            string strRowValue = " AND BREAK_SEQ = " + this.dgvBreakDataGridView[pvtintSeqNoColTimeSheetOrBreakDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvBreakDataGridView)].Value.ToString();

            DataView myDataView = new DataView(this.pvtDataSet.Tables["Break"],
            "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND BREAK_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'" + strRowValue,
            "",
            DataViewRowState.CurrentRows);

            myDataView.Delete(0);

            //Get All Rows for Current Employee / Date
            myDataView.RowFilter = myDataView.RowFilter.Replace(strRowValue, "");

            RePaint_SpreadSheet_Indicators(dgvBreakDataGridView, myDataView);
        }

        private void dgvEmployeeDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (this.rbnDateEmployee.Checked == true)
            {
                if (e.Column.Index == pvtintNoColEmployeeDataGridView)
                {
                    if (dgvEmployeeDataGridView[pvtintCodeColEmployeeDataGridView, e.RowIndex1].Value.ToString() == "")
                    {
                        e.SortResult = -1;
                    }
                    else
                    {
                        if (dgvEmployeeDataGridView[pvtintCodeColEmployeeDataGridView, e.RowIndex2].Value.ToString() == "")
                        {
                            e.SortResult = 1;
                        }
                        else
                        {
                            if (double.Parse(dgvEmployeeDataGridView[pvtintCodeColEmployeeDataGridView, e.RowIndex1].Value.ToString()) > double.Parse(dgvEmployeeDataGridView[pvtintCodeColEmployeeDataGridView, e.RowIndex2].Value.ToString()))
                            {
                                e.SortResult = 1;
                            }
                            else
                            {
                                if (double.Parse(dgvEmployeeDataGridView[pvtintCodeColEmployeeDataGridView, e.RowIndex1].Value.ToString()) < double.Parse(dgvEmployeeDataGridView[pvtintCodeColEmployeeDataGridView, e.RowIndex2].Value.ToString()))
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
            else
            {
                if (e.Column.Index == pvtintNoColEmployeeDataGridView)
                {
                    if (dgvEmployeeDataGridView[pvtintNoColEmployeeDataGridView, e.RowIndex1].Value.ToString() == "")
                    {
                        e.SortResult = -1;
                    }
                    else
                    {
                        if (dgvEmployeeDataGridView[pvtintNoColEmployeeDataGridView, e.RowIndex2].Value.ToString() == "")
                        {
                            e.SortResult = 1;
                        }
                        else
                        {
                            if (double.Parse(dgvEmployeeDataGridView[pvtintNoColEmployeeDataGridView, e.RowIndex1].Value.ToString()) > double.Parse(dgvEmployeeDataGridView[pvtintNoColEmployeeDataGridView, e.RowIndex2].Value.ToString()))
                            {
                                e.SortResult = 1;
                            }
                            else
                            {
                                if (double.Parse(dgvEmployeeDataGridView[pvtintNoColEmployeeDataGridView, e.RowIndex1].Value.ToString()) < double.Parse(dgvEmployeeDataGridView[pvtintNoColEmployeeDataGridView, e.RowIndex2].Value.ToString()))
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

        private void dgvDayDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (this.rbnEmployeeDate.Checked == true)
            {
                if (e.Column.Index == pvtintNoColDayDataGridView)
                {
                    if (double.Parse(dgvDayDataGridView[pvtintKeyColDayDataGridView, e.RowIndex1].Value.ToString().Replace("-", "")) > double.Parse(dgvDayDataGridView[pvtintKeyColDayDataGridView, e.RowIndex2].Value.ToString().Replace("-", "")))
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(dgvDayDataGridView[pvtintKeyColDayDataGridView, e.RowIndex1].Value.ToString().Replace("-", "")) < double.Parse(dgvDayDataGridView[pvtintKeyColDayDataGridView, e.RowIndex2].Value.ToString().Replace("-", "")))
                        {
                            e.SortResult = -1;
                        }
                        else
                        {
                            e.SortResult = 0;
                        }
                    }
                  
                    e.Handled = true;
                }
            }
            else
            {
                if (e.Column.Index == pvtintNoColDayDataGridView)
                {
                    if (double.Parse(dgvDayDataGridView[pvtintNoColDayDataGridView, e.RowIndex1].Value.ToString().Replace("-", "")) > double.Parse(dgvDayDataGridView[pvtintNoColDayDataGridView, e.RowIndex2].Value.ToString().Replace("-", "")))
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(dgvDayDataGridView[pvtintNoColDayDataGridView, e.RowIndex1].Value.ToString().Replace("-", "")) < double.Parse(dgvDayDataGridView[pvtintNoColDayDataGridView, e.RowIndex2].Value.ToString().Replace("-", "")))
                        {
                            e.SortResult = -1;
                        }
                        else
                        {
                            e.SortResult = 0;
                        }
                    }

                    e.Handled = true;
                }
            }
        }

        private void btnRemoveFilter_Click(object sender, EventArgs e)
        {
            this.chkRemoveSat.Checked = false;
            this.chkRemoveSun.Checked = false;
            this.chkRemoveOnLeave.Checked = false;

            this.chkRemoveBlanks.Checked = false;
            
            this.rbnNone.Checked = true;
            rbnNone_Click(sender, e);
        }

        private void TimeSheet_Or_Break_DataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView myDataGridView = (DataGridView)sender;

            if (myDataGridView.Name == "dgvBreakDataGridView")
            {
                if (this.pvtblnBreakDataGridViewLoaded == false)
                {
                    return;
                }
            }
            else
            {
                if (this.pvtblnTimeSheetDataGridViewLoaded == false)
                {
                    return;
                }
            }
            
            if (e.ColumnIndex < pvtintClockInSetColTimeSheetOrBreakDataGridView)
            {
                SendKeys.Send("{Right}");
            }
            else
            {
                if (e.ColumnIndex > pvtintClockOutSetColTimeSheetOrBreakDataGridView)
                {
                    SendKeys.Send("{Left}");
                }
            }
        }

        private void tmrExcludedFromRun_Tick(object sender, EventArgs e)
        {
            tmrExcludedFromRun.Enabled = false;

            CustomMessageBox.Show("There are Timesheet/Break Records that are NOT Included in the Current Time Attendance Run.\n\nSpeak to Administrator for more Info.", "Records Excluded from Time Attendance Run", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void chkEndDateOnly_Click(object sender, EventArgs e)
        {
            if (this.chkEndDateOnly.Checked == true)
            {
                this.chkRemoveSat.Enabled = false;
                this.chkRemoveSun.Enabled = false;
                this.chkRemovePublicHolidays.Enabled = false;
                this.chkRemoveOnLeave.Enabled = false;

                this.chkRemoveSat.Checked = false;
                this.chkRemoveSun.Checked = false;
                this.chkRemovePublicHolidays.Checked = false;
                this.chkRemoveOnLeave.Checked = false;
            }
            else
            {
                if (this.rbnBlank.Checked == true)
                {
                    this.chkRemoveSat.Enabled = true;
                    this.chkRemoveSun.Enabled = true;
                    this.chkRemovePublicHolidays.Enabled = true;
                    this.chkRemoveOnLeave.Enabled = true;
                }
            }

            Load_PayCategory_Records();
        }
    }
}
