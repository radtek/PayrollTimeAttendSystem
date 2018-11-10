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
    public partial class frmTimeSheetAuthorise : Form
    {
        clsISUtilities clsISUtilities;

        private byte[] pvtbytCompress;
        private DataSet pvtDataSet;
        private DataView pvtPayCategoryDataView;
        private DataView pvtBreakRangeDataView;
        private DataView pvtEmployeeDataView;
        private DataView pvtTempDataView;
        private DataView pvtDateDataView;
        private DataView pvtDayTotalDataView;
        private DataView pvtBreakDataView;
        private DataView pvtTimeSheetDataView;
        private DataView pvtEmployeePayCategoryLevelDataView;
          
        private string pvtstrDataAndTypeFilter = "";
        private string pvtstrWeekEndDataFilter = "";
        private string pvtstrPayCategoryFilter = "";
        private string pvtstrCategoryType = "";

        private DateTime pvtDateTime;

        private Int64 pvtint64CompanyNo = -1;
        private int pvtintPayCategoryNo = -1;
        private int pvtintEmployeeNo = -1;
        private int pvtintAuthoriseLevel = -1;
        private int pvtintEmployeeColNo = 7;
        private int pvtintDateColNo = 10;
        private int pvtintPayCategoryTableRowNo = -1;
    
        private int pvtintSeconds = 0;
        private int pvtintTotalTimeSheetMinutes = 0;
        private int pvtintTotalBreakMinutes = 0;

        private string pvtstrPayrollType = "";

        private int pvtintPayCategoryDataGridViewRowIndex = -1;
        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
        private int pvtintAuthorisationLevelDataGridViewRowIndex = -1;
        private int pvtintEmployeeDataGridViewRowIndex = -1;
        private int pvtintDayDataGridViewRowIndex = -1;

        private bool pvtblnTimeSheetInError = false;
        private bool pvtblnBreakInError = false;

        DataGridViewCellStyle PayrollLinkDataGridViewCellStyle;
        DataGridViewCellStyle ErrorDataGridViewCellStyle;
        DataGridViewCellStyle ExceptionDataGridViewCellStyle;
        DataGridViewCellStyle NoRecordDataGridViewCellStyle;
        DataGridViewCellStyle NormalDataGridViewCellStyle;
        DataGridViewCellStyle BreakExceptionDataGridViewCellStyle;
        DataGridViewCellStyle WeekEndDataGridViewCellStyle;
        DataGridViewCellStyle LunchTotalDataGridViewCellStyle;
        DataGridViewCellStyle PublicHolidayDataGridViewCellStyle;

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnPayCategoryDataGridViewLoaded = false;
        private bool pvtblnAuthorisationLevelDataGridViewLoaded = false;

        private bool pvtblnEmployeeDataGridViewLoaded = false;
        private bool pvtblnDayDataGridViewLoaded = false;
        private bool pvtblnBreakRangeDataGridViewLoaded = false;
        private bool pvtblnTimeSheetDataGridViewLoaded = false;
        private bool pvtblnBreakDataGridViewLoaded = false;
                
        public frmTimeSheetAuthorise()
        {
            InitializeComponent();

            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 100;

                this.grbFilter.Top += 100;
                this.grbRowLegend.Top += 100;

                this.btnAuthoriseAll.Top += 100;
                this.grbAuthorisationFilter.Top += 100;
                this.grbFormatTimeDesc.Top += 100;

                this.lblDayDesc.Top += 100;
                this.lblTimesheets.Top += 100;
                this.lblTimeSheetStart.Top += 100;
                this.lblTimeSheetStop.Top += 100;
                this.lblTimeSheetAccum.Top += 100;
                this.dgvTimeSheetDataGridView.Top += 100;

                this.lblBreakRange.Top += 100;
                this.dgvBreakRangeDataGridView.Top += 100;
                this.dgvBreakExceptionDataGridView.Top += 100;
                this.dgvTimeSheetTotalsDataGridView.Top += 100;
                
                this.lblBreaks.Top += 100;
                this.lblBreakStart.Top += 100;
                this.lblBreakStop.Top += 100;
                this.lblBreakAccum.Top += 100;
                this.dgvBreakDataGridView.Top += 100;
                this.dgvBreakTotalsDataGridView.Top += 100;

                this.grbBreakError.Top += 100;
                
                this.dgvPayCategoryDataGridView.Height += 38;

                int intEmployeeTop = 41;

                this.lblEmployee.Top += intEmployeeTop;
                this.picEmployeeFilter.Top += intEmployeeTop;
                this.dgvEmployeeDataGridView.Top += intEmployeeTop;
                this.dgvEmployeeDataGridView.Height += 57;

                this.lblDate.Top += 100;
                this.picDayFilter.Top += 100;
                this.dgvDayDataGridView.Top += 100;
                //this.dgvDayDataGridView.Height += 19;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmTimeSheetAuthorise_Load(object sender, EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this, "busTimeSheetAuthorise");

                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblAuthorisationLevelSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblDate.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblDayDesc.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblTimesheets.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblBreaks.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblBreakStart.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblBreakStop.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblBreakAccum.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblBreakRange.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

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
             
                this.pvtint64CompanyNo = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));

                PayrollLinkDataGridViewCellStyle = new DataGridViewCellStyle();
                PayrollLinkDataGridViewCellStyle.BackColor = Color.Magenta;
                PayrollLinkDataGridViewCellStyle.SelectionBackColor = Color.Magenta;
             
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

                PublicHolidayDataGridViewCellStyle = new DataGridViewCellStyle();
                PublicHolidayDataGridViewCellStyle.BackColor = Color.SlateBlue;
                PublicHolidayDataGridViewCellStyle.SelectionBackColor = Color.SlateBlue;
               
                object[] objParm = new object[4];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[2] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[3] =  AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();
               
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                pvtDateTime = DateTime.Now.AddYears(100);
            
                for (int intRow = 0; intRow < pvtDataSet.Tables["PayrollType"].Rows.Count; intRow++)
                {
                    this.dgvPayrollTypeDataGridView.Rows.Add(pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE_DESC"].ToString());
                }

                pvtblnPayrollTypeDataGridViewLoaded = true;

                if (this.dgvPayrollTypeDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView, 0);
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
                case "dgvAuthorisationLevelDataGridView":

                    pvtintAuthorisationLevelDataGridViewRowIndex = -1;
                    break;

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
                    case "dgvAuthorisationLevelDataGridView":

                        this.dgvAuthorisationLevelDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

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
        
        private void Load_PayCategory_Records()
        {
            this.Cursor = Cursors.WaitCursor;

            this.pvtblnPayCategoryDataGridViewLoaded = false;
            this.pvtblnAuthorisationLevelDataGridViewLoaded = false;
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
            this.Clear_DataGridView(this.dgvAuthorisationLevelDataGridView);
            this.Clear_DataGridView(this.dgvEmployeeDataGridView);
            this.Clear_DataGridView(this.dgvDayDataGridView);
            this.Clear_DataGridView(this.dgvTimeSheetDataGridView);
            this.Clear_DataGridView(this.dgvBreakRangeDataGridView);
            this.Clear_DataGridView(this.dgvBreakDataGridView);

            dgvBreakTotalsDataGridView[0, 0].Style = LunchTotalDataGridViewCellStyle;

            this.grbBreakError.Visible = false;

            this.btnUpdate.Enabled = false;

            if (this.chkRemoveSat.Checked == true
            | this.chkRemoveSun.Checked == true)
            {
                pvtstrWeekEndDataFilter = " AND NOT DAY_NO IN (";

                if (this.chkRemoveSat.Checked == true)
                {
                    pvtstrWeekEndDataFilter += "6";
                }

                if (this.chkRemoveSun.Checked == true)
                {
                    if (this.chkRemoveSat.Checked == true)
                    {
                        pvtstrWeekEndDataFilter += ",0";
                    }
                    else
                    {
                        pvtstrWeekEndDataFilter += "0";
                    }
                }

                pvtstrWeekEndDataFilter += ")";
            }
            else
            {
                pvtstrWeekEndDataFilter = "";
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
                                pvtstrCategoryType = "";
                            }
                        }
                    }
                }
            }

            pvtPayCategoryDataView = null;
            pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"],
                "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                "",
                DataViewRowState.CurrentRows);

            string strLastUploadDateTime = "";
            string strLastUploadDateTimeSort = "";

            for (int intRow = 0; intRow < pvtPayCategoryDataView.Count; intRow++)
            {
                pvtstrDataAndTypeFilter = " AND DAY_DATE <= '" + Convert.ToDateTime(pvtPayCategoryDataView[intRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'";

                strLastUploadDateTime = "";
                strLastUploadDateTimeSort = "";

                DataView DataView = new System.Data.DataView(pvtDataSet.Tables["DayTotal"],
                                    "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_CATEGORY_NO = " + pvtPayCategoryDataView[intRow]["PAY_CATEGORY_NO"] + pvtstrDataAndTypeFilter + pvtstrWeekEndDataFilter + pvtstrCategoryType,
                                    "INDICATOR DESC",
                                    DataViewRowState.CurrentRows);

                //NB Blank Cannot be Checked - it Wont Exist
                if ((this.rbnErrors.Checked == true
                | this.rbnException.Checked == true
                | this.rbnNormal.Checked == true
                | this.rbnBreakException.Checked == true
                | this.rbnPublicHoliday.Checked == true
                | this.chkRemoveBlanks.Checked == true)
                & DataView.Count == 0)
                {
                    continue;
                }

                if (pvtPayCategoryDataView[intRow]["LAST_UPLOAD_DATETIME"] != System.DBNull.Value)
                {
                    strLastUploadDateTime = Convert.ToDateTime(pvtPayCategoryDataView[intRow]["LAST_UPLOAD_DATETIME"]).ToString("dd MMMM yyyy - HH:mm");

                    strLastUploadDateTimeSort = Convert.ToDateTime(pvtPayCategoryDataView[intRow]["LAST_UPLOAD_DATETIME"]).ToString("yyyyMMddHHmm");
                }

                this.dgvPayCategoryDataGridView.Rows.Add("",
                                                         "",
                                                         "",
                                                         pvtPayCategoryDataView[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                         strLastUploadDateTime,
                                                         strLastUploadDateTimeSort,
                                                         intRow.ToString());

                if (pvtPayCategoryDataView[intRow]["PAY_PERIOD_DATE"] != System.DBNull.Value)
                {
                    if (pvtPayCategoryDataView[intRow]["RUN_IND"].ToString() == "Y")
                    {
                        dgvPayCategoryDataGridView[0, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = PayrollLinkDataGridViewCellStyle;

                    }
                }
                 
                if (this.rbnBlank.Checked == true)
                {
                    this.dgvPayCategoryDataGridView.Rows[this.dgvPayCategoryDataGridView.Rows.Count - 1].HeaderCell.Style = NoRecordDataGridViewCellStyle;
                }
                else
                {
                    if (DataView.Count > 0)
                    {
                        if (DataView[0]["INDICATOR"].ToString() == "X")
                        {
                            this.dgvPayCategoryDataGridView.Rows[this.dgvPayCategoryDataGridView.Rows.Count - 1].HeaderCell.Style = ErrorDataGridViewCellStyle;
                        }
                        else
                        {
                            if (DataView[0]["INDICATOR"].ToString() == "E")
                            {
                                this.dgvPayCategoryDataGridView.Rows[this.dgvPayCategoryDataGridView.Rows.Count - 1].HeaderCell.Style = ExceptionDataGridViewCellStyle;
                            }
                            else
                            {
                                if (DataView[0]["INDICATOR"].ToString() == "B")
                                {
                                    this.dgvPayCategoryDataGridView.Rows[this.dgvPayCategoryDataGridView.Rows.Count - 1].HeaderCell.Style = NoRecordDataGridViewCellStyle;
                                }
                            }
                        }
                    }
                    else
                    {
                        this.dgvPayCategoryDataGridView.Rows[this.dgvPayCategoryDataGridView.Rows.Count - 1].HeaderCell.Style = NoRecordDataGridViewCellStyle;
                    }
                }

                if (this.rbnBlank.Checked == false
                & DataView.Count > 0)
                {
                    DataView.Sort = "BREAK_INDICATOR DESC";

                    if (DataView[0]["BREAK_INDICATOR"].ToString() == "Y")
                    {
                        dgvPayCategoryDataGridView[1, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = BreakExceptionDataGridViewCellStyle;
                    }

                    DataView.Sort = "PAID_HOLIDAY_INDICATOR DESC";

                    if (DataView[0]["PAID_HOLIDAY_INDICATOR"].ToString() == "Y")
                    {
                        dgvPayCategoryDataGridView[2, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = PublicHolidayDataGridViewCellStyle;
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
  
            pvtEmployeeDataView = null;
            pvtEmployeeDataView = new DataView(this.pvtDataSet.Tables["Employee"],
                "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                "",
                DataViewRowState.CurrentRows);

            string strAuthorisationType = "";

            if (this.rbnAuthorised.Checked == true)
            {
                strAuthorisationType = " AND AUTHORISED_IND = 'Y'";
            }
            else
            {
                if (rbnNotAuthorised.Checked == true)
                {
                    strAuthorisationType = " AND AUTHORISED_IND <> 'Y'";
                }
            }

            pvtEmployeePayCategoryLevelDataView = null;
            pvtEmployeePayCategoryLevelDataView = new DataView(this.pvtDataSet.Tables["EmployeePayCategoryLevel"],
               pvtstrPayCategoryFilter + " AND LEVEL_NO = " + this.pvtintAuthoriseLevel + strAuthorisationType,
               "EMPLOYEE_NO",
               DataViewRowState.CurrentRows);
        
            this.pvtblnEmployeeDataGridViewLoaded = false;
            pvtintEmployeeDataGridViewRowIndex = -1;

            bool blnAuthorised = false;
            int intFindRecord = -1;

            //Set Error
            for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
            {
                intFindRecord = pvtEmployeePayCategoryLevelDataView.Find(pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString());

                if (intFindRecord == -1)
                {
                    continue;
                }
                else
                {
                    if (pvtEmployeePayCategoryLevelDataView[intFindRecord]["AUTHORISED_IND"].ToString() == "Y")
                    {
                        blnAuthorised = true;
                    }
                    else
                    {
                        blnAuthorised = false;
                    }
                }

                pvtTempDataView = null;
                pvtTempDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                    pvtstrPayCategoryFilter + " AND EMPLOYEE_NO = " + pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString() + " " + pvtstrDataAndTypeFilter + pvtstrWeekEndDataFilter + pvtstrCategoryType,
                    "INDICATOR DESC",
                    DataViewRowState.CurrentRows);

                if (((this.rbnErrors.Checked == true
                | this.rbnException.Checked == true
                | this.rbnNormal.Checked == true
                | this.rbnBreakException.Checked == true
                | this.rbnPublicHoliday.Checked == true
                | this.chkRemoveBlanks.Checked == true)
                & pvtTempDataView.Count == 0))
                {
                    continue;
                }
                
                this.dgvEmployeeDataGridView.Rows.Add("",
                                                      "",
                                                      "",
                                                      pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                      pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                      pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                      blnAuthorised,
                                                      intRow.ToString());

                if (pvtintEmployeeNo == Convert.ToInt32(pvtEmployeeDataView[intRow]["EMPLOYEE_NO"]))
                {
                    intEmployeeNoRow = this.dgvEmployeeDataGridView.Rows.Count - 1;
                }

                if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_PERIOD_DATE"] != System.DBNull.Value)
                {
                    if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["RUN_IND"].ToString() == "Y")
                    {
                        dgvEmployeeDataGridView[0, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = PayrollLinkDataGridViewCellStyle;
                        this.dgvEmployeeDataGridView.Rows[this.dgvEmployeeDataGridView.Rows.Count - 1].ReadOnly = true;
                    }
                }
                
                if (this.rbnBlank.Checked == true
                | pvtTempDataView.Count == 0)
                {
                    this.dgvEmployeeDataGridView.Rows[this.dgvEmployeeDataGridView.Rows.Count - 1].HeaderCell.Style = NoRecordDataGridViewCellStyle;
                }
                else
                {
                    if (pvtTempDataView[0]["INDICATOR"].ToString() == "X")
                    {
                        this.dgvEmployeeDataGridView.Rows[this.dgvEmployeeDataGridView.Rows.Count - 1].HeaderCell.Style = ErrorDataGridViewCellStyle;

                        this.dgvEmployeeDataGridView.Rows[this.dgvEmployeeDataGridView.Rows.Count - 1].ReadOnly = true;
                    }
                    else
                    {
                        if (pvtTempDataView[0]["INDICATOR"].ToString() == "E")
                        {
                            this.dgvEmployeeDataGridView.Rows[this.dgvEmployeeDataGridView.Rows.Count - 1].HeaderCell.Style = ExceptionDataGridViewCellStyle;
                        }
                        else
                        {
                            if (pvtTempDataView[0]["INDICATOR"].ToString() == "B")
                            {
                                this.dgvEmployeeDataGridView.Rows[this.dgvEmployeeDataGridView.Rows.Count - 1].HeaderCell.Style = NoRecordDataGridViewCellStyle;
                            }
                        }
                    }
                }

                if (this.rbnBlank.Checked == false
                & pvtTempDataView.Count > 0)
                {
                    pvtTempDataView.Sort = "BREAK_INDICATOR DESC";

                    if (pvtTempDataView[0]["BREAK_INDICATOR"].ToString() == "Y")
                    {
                        dgvEmployeeDataGridView[1, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = BreakExceptionDataGridViewCellStyle;
                    }

                    pvtTempDataView.Sort = "PAID_HOLIDAY_INDICATOR DESC";

                    if (pvtTempDataView[0]["PAID_HOLIDAY_INDICATOR"].ToString() == "Y")
                    {
                        dgvEmployeeDataGridView[2, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = PublicHolidayDataGridViewCellStyle;
                    }
                }
            }

            this.pvtblnEmployeeDataGridViewLoaded = true;

            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_PERIOD_DATE"] != System.DBNull.Value)
                {
                    if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["RUN_IND"].ToString() == "Y")
                    {
                        this.btnUpdate.Enabled = false;
                    }
                    else
                    {
                        this.btnUpdate.Enabled = true;
                    }
                }
                else
                {
                    this.btnUpdate.Enabled = true;
                }
               
                this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, intEmployeeNoRow);
            }
            else
            {
                //Clear Totals
                this.dgvTimeSheetTotalsDataGridView[1, 0].Value = "0.00";
                this.dgvTimeSheetTotalsDataGridView[1, 1].Value = "0.00";
                this.dgvTimeSheetTotalsDataGridView[1, 2].Value = "0.00";
                this.dgvTimeSheetTotalsDataGridView[1, 3].Value = "0.00";

                this.dgvPayCategoryDataGridView.Rows[pvtintPayCategoryDataGridViewRowIndex].HeaderCell.Style = NormalDataGridViewCellStyle;
                this.dgvPayCategoryDataGridView[0, pvtintPayCategoryDataGridViewRowIndex].Style = NormalDataGridViewCellStyle;
                this.dgvPayCategoryDataGridView[1, pvtintPayCategoryDataGridViewRowIndex].Style = NormalDataGridViewCellStyle;

                this.btnUpdate.Enabled = false;
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
          
            int intTempDateViewIndex = -1;

            string strExceptionCol = "";
            string strRecordLocked = "";
            string strCol5 = "";
            string strCol6 = "";
         
            pvtTempDataView = null;
            pvtTempDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                    pvtstrPayCategoryFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + pvtstrDataAndTypeFilter + pvtstrWeekEndDataFilter + pvtstrCategoryType,
                    "DAY_DATE",
                    DataViewRowState.CurrentRows);
            

            for (int intRow = 0; intRow < pvtDateDataView.Count; intRow++)
            {
                strRecordLocked = "";
                strExceptionCol = "";

                intTempDateViewIndex = pvtTempDataView.Find(Convert.ToDateTime(pvtDateDataView[intRow]["DAY_DATE"]).ToString("yyyy-MM-dd"));

                if (((this.rbnErrors.Checked == true
                | this.rbnException.Checked == true
                | this.rbnNormal.Checked == true
                | this.rbnBreakException.Checked == true
                | this.rbnPublicHoliday.Checked == true
                | this.chkRemoveBlanks.Checked == true)
                & intTempDateViewIndex == -1)
                | (this.rbnBlank.Checked == true
                & intTempDateViewIndex > -1))
                {
                    continue;
                }

                if (Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + Convert.ToDateTime(pvtDateDataView[intRow]["DAY_DATE"]).ToString("ddd").ToUpper() + "_BELOW_MINUTES"]) == 0)
                {
                    strExceptionCol = ">= 0";
                }
                else
                {
                    intBelowHH = Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + Convert.ToDateTime(pvtDateDataView[intRow]["DAY_DATE"]).ToString("ddd").ToUpper() + "_BELOW_MINUTES"]) / 60;
                    intBelowMM = Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + Convert.ToDateTime(pvtDateDataView[intRow]["DAY_DATE"]).ToString("ddd").ToUpper() + "_BELOW_MINUTES"]) % 60;
                    intAboveHH = Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + Convert.ToDateTime(pvtDateDataView[intRow]["DAY_DATE"]).ToString("ddd").ToUpper() + "_ABOVE_MINUTES"]) / 60;
                    intAboveMM = Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + Convert.ToDateTime(pvtDateDataView[intRow]["DAY_DATE"]).ToString("ddd").ToUpper() + "_ABOVE_MINUTES"]) % 60;

                    strExceptionCol = intBelowHH.ToString() + ":" + intBelowMM.ToString("00") + " - " + intAboveHH.ToString() + ":" + intAboveMM.ToString("00");
                }

                if (this.rbnBlank.Checked == true
                | intTempDateViewIndex == -1)
                {
                    strCol5 = "0:00";
                    strCol6 = "0:00";
                }
                else
                {
                    strCol5 = Convert.ToInt32(Convert.ToInt32(pvtTempDataView[intTempDateViewIndex]["DAY_PAID_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtTempDataView[intTempDateViewIndex]["DAY_PAID_MINUTES"]) % 60).ToString("00");
                    strCol6 = Convert.ToInt32(Convert.ToInt32(pvtTempDataView[intTempDateViewIndex]["BREAK_ACCUM_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtTempDataView[intTempDateViewIndex]["BREAK_ACCUM_MINUTES"]) % 60).ToString("00");
                }
     
                this.dgvDayDataGridView.Rows.Add("",
                                                 "",
                                                 "",
                                                 Convert.ToDateTime(pvtDateDataView[intRow]["DAY_DATE"]).ToString("dd MMMM yyyy - dddd"),
                                                 strExceptionCol,
                                                 strCol5,
                                                 strCol6,
                                                 "",
                                                 "",
                                                 strRecordLocked,
                                                 Convert.ToDateTime(pvtDateDataView[intRow]["DAY_DATE"]).ToString("yyyy-MM-dd"));

                //Weekend
                if (Convert.ToInt32(pvtDateDataView[intRow]["DAY_NO"]) == 6
                    | Convert.ToInt32(pvtDateDataView[intRow]["DAY_NO"]) == 0)
                {
                    this.dgvDayDataGridView.Rows[dgvDayDataGridView.Rows.Count - 1].DefaultCellStyle = WeekEndDataGridViewCellStyle;

                    this.dgvDayDataGridView[0, dgvDayDataGridView.Rows.Count - 1].Style = NormalDataGridViewCellStyle;
                    this.dgvDayDataGridView[1, dgvDayDataGridView.Rows.Count - 1].Style = NormalDataGridViewCellStyle;
                    this.dgvDayDataGridView[2, dgvDayDataGridView.Rows.Count - 1].Style = NormalDataGridViewCellStyle;
                }

                if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_PERIOD_DATE"] != System.DBNull.Value)
                {
                    if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["RUN_IND"].ToString() == "Y")
                    {
                        dgvDayDataGridView[0, this.dgvDayDataGridView.Rows.Count - 1].Style = PayrollLinkDataGridViewCellStyle;
                    }
                }
               
                if (this.rbnBlank.Checked == true
                   | intTempDateViewIndex == -1)
                {
                    this.dgvDayDataGridView.Rows[this.dgvDayDataGridView.Rows.Count - 1].HeaderCell.Style = NoRecordDataGridViewCellStyle;
                }
                else
                {
                    if (pvtTempDataView[intTempDateViewIndex]["INDICATOR"].ToString() == "X")
                    {
                        this.dgvDayDataGridView.Rows[this.dgvDayDataGridView.Rows.Count - 1].HeaderCell.Style = ErrorDataGridViewCellStyle;
                    }
                    else
                    {
                        if (pvtTempDataView[intTempDateViewIndex]["INDICATOR"].ToString() == "E")
                        {
                            this.dgvDayDataGridView.Rows[this.dgvDayDataGridView.Rows.Count - 1].HeaderCell.Style = ExceptionDataGridViewCellStyle;
                        }
                    }

                    if (pvtTempDataView[intTempDateViewIndex]["BREAK_INDICATOR"].ToString() == "Y")
                    {
                        dgvDayDataGridView[1, this.dgvDayDataGridView.Rows.Count - 1].Style = BreakExceptionDataGridViewCellStyle;
                    }

                    if (pvtTempDataView[intTempDateViewIndex]["PAID_HOLIDAY_INDICATOR"].ToString() == "Y")
                    {
                        dgvDayDataGridView[2, this.dgvDayDataGridView.Rows.Count - 1].Style = PublicHolidayDataGridViewCellStyle;
                    }
                }

                if (Convert.ToDateTime(pvtDateDataView[intRow]["DAY_DATE"]) == pvtDateTime)
                {
                    intSelectedDayRow = this.dgvDayDataGridView.Rows.Count - 1;
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
            }
        }
        
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            this.Text += " - Update";

            this.dgvEmployeeDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dgvEmployeeDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;
            
            this.dgvPayCategoryDataGridView.Enabled = false;
            this.dgvAuthorisationLevelDataGridView.Enabled = false;
            this.dgvPayrollTypeDataGridView.Enabled = false;

            Show_Update_Lock_Images();
            
            this.chkRemoveBlanks.Enabled = false;
            this.chkRemoveSat.Enabled = false;
            this.chkRemoveSun.Enabled = false;

            this.rbnErrors.Enabled = false;
            this.rbnException.Enabled = false;
            this.rbnNone.Enabled = false;
            this.rbnBlank.Enabled = false;
            this.rbnNormal.Enabled = false;

            this.rbnBreakException.Enabled = false;
            this.rbnPublicHoliday.Enabled = false;

            this.rbnAuthorised.Enabled = false;
            this.rbnNotAuthorised.Enabled = false;
            this.rbnAuthorisationNone.Enabled = false;

            this.btnUpdate.Enabled = false;
            
            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;
            this.btnAuthoriseAll.Enabled = true;

            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                dgvEmployeeDataGridView.CurrentCell = dgvEmployeeDataGridView[6, 0];
            }
        }

        private void rbnNone_Click(object sender, EventArgs e)
        {
            if (this.rbnAuthorisationNone.Checked == true)
            {
                this.grbFilterDisplay.Visible = false;
                this.tmrFilter.Enabled = false;
                this.pvtintSeconds = 0;
            }

            this.chkRemoveBlanks.Enabled = true;

            this.chkRemoveSat.Checked = false;
            this.chkRemoveSat.Enabled = false;
            this.chkRemoveSun.Checked = false;
            this.chkRemoveSun.Enabled = false;

            Load_PayCategory_Records();
        }

        private void Show_Update_Lock_Images()
        {
            this.picPayrollTypeLock.Image = global::TimeSheetAuthorise.Properties.Resources.NewLock16;
            this.picPayrollTypeLock.Visible = true;

            this.picPayCategoryLock.Image = global::TimeSheetAuthorise.Properties.Resources.NewLock16;
            this.picPayCategoryLock.Visible = true;

            this.picAuthoriseLevelLock.Image = global::TimeSheetAuthorise.Properties.Resources.NewLock16;
            this.picAuthoriseLevelLock.Visible = true;

            this.picEmployeeFilter.Visible = false;
            this.picDayFilter.Visible = false;
        }

        private void GeneralFilter_Click(object sender, EventArgs e)
        {
            if (this.dgvPayrollTypeDataGridView.Rows.Count > 0)
            {
                this.grbFilterDisplay.Visible = true;

                this.chkRemoveBlanks.Checked = false;
                this.chkRemoveBlanks.Enabled = false;

                this.chkRemoveSat.Checked = false;
                this.chkRemoveSat.Enabled = false;
                this.chkRemoveSun.Checked = false;
                this.chkRemoveSun.Enabled = false;

                Load_PayCategory_Records();
            }
        }

        private void rbnBlank_Click(object sender, EventArgs e)
        {
            if (this.dgvPayrollTypeDataGridView.Rows.Count > 0)
            {
                this.grbFilterDisplay.Visible = true;

                this.chkRemoveBlanks.Checked = false;
                this.chkRemoveBlanks.Enabled = false;

                this.chkRemoveSat.Enabled = true;
                this.chkRemoveSun.Enabled = true;

                Load_PayCategory_Records();
            }
        }

        private void chkRemoveBlanks_Click(object sender, EventArgs e)
        {
            if (this.rbnNone.Checked == true)
            {
                if (chkRemoveBlanks.Checked == true)
                {
                    this.grbFilterDisplay.Visible = true;
                }
                else
                {
                    if (this.rbnAuthorisationNone.Checked == true)
                    {
                        this.tmrFilter.Enabled = false;
                        this.pvtintSeconds = 0;
                        this.grbFilterDisplay.Visible = false;
                    }
                }

                Load_PayCategory_Records();
            }
        }

        private void tmrFilter_Tick(object sender, EventArgs e)
        {
            pvtintSeconds -= 1;

            if (pvtintSeconds == 0)
            {
                this.tmrFilter.Enabled = false;
            }
            else
            {
                if (pvtintSeconds % 2 == 0)
                {
                    grbFilterDisplay.Visible = false;
                }

                else
                {
                    grbFilterDisplay.Visible = true;
                }
            }
        }

        private void grbFilterDisplay_VisibleChanged(object sender, EventArgs e)
        {
            if (this.grbFilterDisplay.Visible == true)
            {
                if (pvtintSeconds == 0)
                {
                    pvtintSeconds = 3;
                    this.tmrFilter.Enabled = true;
                }

                this.picPayrollTypeLock.Image = global::TimeSheetAuthorise.Properties.Resources.filter16;
                this.picPayrollTypeLock.Visible = true;

                this.picPayCategoryLock.Image = global::TimeSheetAuthorise.Properties.Resources.filter16;
                this.picPayCategoryLock.Visible = true;

                this.picAuthoriseLevelLock.Image = global::TimeSheetAuthorise.Properties.Resources.filter16;
                this.picAuthoriseLevelLock.Visible = true;
                this.picEmployeeFilter.Visible = true;
                this.picDayFilter.Visible = true;
            }
            else
            {
                this.picPayrollTypeLock.Visible = false;
                this.picPayCategoryLock.Visible = false;
                this.picAuthoriseLevelLock.Visible = false;
                this.picEmployeeFilter.Visible = false;
                this.picDayFilter.Visible = false;
            }
        }

        private void Remove_Saturday_Sunday_Click(object sender, EventArgs e)
        {
            Load_PayCategory_Records();
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
            this.rbnPublicHoliday.Enabled = true;

            this.rbnAuthorised.Enabled = true;
            this.rbnNotAuthorised.Enabled = true;
            this.rbnAuthorisationNone.Enabled = true;

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.btnAuthoriseAll.Enabled = false;

            this.dgvPayCategoryDataGridView.Enabled = true;
            this.dgvAuthorisationLevelDataGridView.Enabled = true;
            this.dgvPayrollTypeDataGridView.Enabled = true;
            
            //Reset Filter Icons
            this.grbFilterDisplay_VisibleChanged(sender, e);
           
            if (this.rbnNone.Checked == true)
            {
                this.chkRemoveBlanks.Enabled = true;
            }
            else
            {
                if (this.rbnBlank.Checked == true)
                {
                    this.chkRemoveSat.Enabled = true;
                    this.chkRemoveSun.Enabled = true;
                }
            }
          
            this.dgvEmployeeDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvEmployeeDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
          
            pvtDataSet.RejectChanges();

            Load_PayCategory_Records();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet TempDataSet = new DataSet();
                TempDataSet.Tables.Add(this.pvtDataSet.Tables["EmployeePayCategoryLevel"].Clone());

                int intFindRecord = -1;

                for (int intRow = 0; intRow < this.dgvEmployeeDataGridView.Rows.Count; intRow++)
                {
                    DataRow drDataRow = TempDataSet.Tables["EmployeePayCategoryLevel"].NewRow();

                    drDataRow["COMPANY_NO"] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    drDataRow["EMPLOYEE_NO"] = pvtEmployeeDataView[Convert.ToInt32(this.dgvEmployeeDataGridView[7, intRow].Value)]["EMPLOYEE_NO"].ToString();
                    drDataRow["PAY_CATEGORY_NO"] = pvtintPayCategoryNo;
                    drDataRow["PAY_CATEGORY_TYPE"] = pvtstrPayrollType;
                    drDataRow["LEVEL_NO"] = pvtintAuthoriseLevel;

                    if (Convert.ToBoolean(this.dgvEmployeeDataGridView[6, intRow].Value) == true)
                    {
                        drDataRow["AUTHORISED_IND"] = "Y";
                    }
                    else
                    {
                        drDataRow["AUTHORISED_IND"] = "N";
                    }

                    TempDataSet.Tables["EmployeePayCategoryLevel"].Rows.Add(drDataRow);
                }

                pvtEmployeePayCategoryLevelDataView = null;
                pvtEmployeePayCategoryLevelDataView = new DataView(this.pvtDataSet.Tables["EmployeePayCategoryLevel"],
                   pvtstrPayCategoryFilter,
                   "EMPLOYEE_NO",
                   DataViewRowState.CurrentRows);

                //Delete - New Records will be Passed Back for CostCentre
                for (int intRow = 0; intRow < pvtEmployeePayCategoryLevelDataView.Count; intRow++)
                {
                    pvtEmployeePayCategoryLevelDataView[intRow].Delete();

                    intRow -= 1;
                }

                //Delete - New Records will be Passed Back for CostCentre
                DataView DataViewAuthorsiseLevel = new System.Data.DataView(pvtDataSet.Tables["AuthorsiseLevel"],
                                     pvtstrPayCategoryFilter, 
                                      "", 
                                      DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < DataViewAuthorsiseLevel.Count; intRow++)
                {
                    DataViewAuthorsiseLevel[intRow].Delete();

                    intRow -= 1;
                }

                //Compress DataSet
                pvtbytCompress = clsISUtilities.Compress_DataSet(TempDataSet);

                object[] objParm = new object[8];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = this.pvtintPayCategoryNo;
                objParm[2] = pvtstrPayrollType;
                objParm[3] = this.pvtintAuthoriseLevel;
                objParm[4] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[5] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[6] =  AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();
                objParm[7] = pvtbytCompress;

                pvtbytCompress = null;
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Update_Records", objParm);

                TempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                pvtDataSet.Merge(TempDataSet);

                this.pvtDataSet.AcceptChanges();

                btnCancel_Click(sender, e);
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void dgvPayrollTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (this.pvtblnPayrollTypeDataGridViewLoaded == true)
                {
                    if (pvtintPayrollTypeDataGridViewRowIndex != e.RowIndex)
                    {
                        pvtintPayrollTypeDataGridViewRowIndex = e.RowIndex;

                        this.Cursor = Cursors.AppStarting;

                        pvtstrPayrollType = this.dgvPayrollTypeDataGridView[0, e.RowIndex].Value.ToString().Substring(0, 1);

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

                            pvtDataSet.Merge(TempDataSet);
                        }

                        Load_PayCategory_Records();

                        this.Cursor = Cursors.Default;
                    }
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
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

                    Clear_DataGridView(dgvAuthorisationLevelDataGridView);
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

                    pvtintPayCategoryTableRowNo = Convert.ToInt32(this.dgvPayCategoryDataGridView[6, e.RowIndex].Value);

                    pvtintPayCategoryNo = Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_CATEGORY_NO"]);

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

                    this.pvtblnAuthorisationLevelDataGridViewLoaded = false;

                    DataView AuthorsiseLevelDataView = new DataView(pvtDataSet.Tables["AuthorsiseLevel"],
                    pvtstrPayCategoryFilter,
                     "LEVEL_NO",
                     DataViewRowState.CurrentRows);

                    string strCount = "";
                    int intCurrentRow = 0;

                    for (int intRow = 0; intRow < AuthorsiseLevelDataView.Count; intRow++)
                    {
                        strCount = AuthorsiseLevelDataView[intRow]["AUTHORISE_CURRENT"].ToString() + " / " + AuthorsiseLevelDataView[intRow]["AUTHORISE_TOTAL"].ToString();

                        this.dgvAuthorisationLevelDataGridView.Rows.Add(AuthorsiseLevelDataView[intRow]["LEVEL_DESC"].ToString(),
                                                                        strCount,
                                                                        AuthorsiseLevelDataView[intRow]["LEVEL_NO"].ToString());

                        if (pvtintAuthorisationLevelDataGridViewRowIndex == intRow)
                        {
                            intCurrentRow = intRow;
                        }
                    }

                    this.pvtblnAuthorisationLevelDataGridViewLoaded = true;

                    if (this.dgvAuthorisationLevelDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(dgvAuthorisationLevelDataGridView, intCurrentRow);
                    }

                    //Load_Employee_SpreadSheet();

                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void dgvPayCategoryDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 3)
            {
                if (dgvPayCategoryDataGridView[5, e.RowIndex1].Value.ToString() == "")
                {
                    e.SortResult = -1;
                }
                else
                {
                    if (dgvPayCategoryDataGridView[5, e.RowIndex2].Value.ToString() == "")
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(dgvPayCategoryDataGridView[5, e.RowIndex1].Value.ToString()) > double.Parse(dgvPayCategoryDataGridView[5, e.RowIndex2].Value.ToString()))
                        {
                            e.SortResult = 1;
                        }
                        else
                        {
                            if (double.Parse(dgvPayCategoryDataGridView[5, e.RowIndex1].Value.ToString()) < double.Parse(dgvPayCategoryDataGridView[5, e.RowIndex2].Value.ToString()))
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

                    //Get Employee Number
                    pvtintEmployeeNo = Convert.ToInt32(pvtEmployeeDataView[Convert.ToInt32(this.dgvEmployeeDataGridView[pvtintEmployeeColNo, e.RowIndex].Value)]["EMPLOYEE_NO"]);

                    pvtDateDataView = null;
                    pvtDateDataView = new DataView(this.pvtDataSet.Tables["Dates"],
                        pvtstrPayCategoryFilter + " AND DAY_DATE > '" + Convert.ToDateTime(pvtEmployeeDataView[Convert.ToInt32(this.dgvEmployeeDataGridView[pvtintEmployeeColNo, e.RowIndex].Value)]["EMPLOYEE_LAST_RUNDATE"]).ToString("yyyy-MM-dd") + "'" + pvtstrDataAndTypeFilter + pvtstrWeekEndDataFilter,
                        "DAY_DATE DESC",
                        DataViewRowState.CurrentRows);

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

                    //Get Date
                    pvtDateTime = Convert.ToDateTime(this.dgvDayDataGridView[this.pvtintDateColNo, e.RowIndex].Value);

                    pvtDayTotalDataView = null;
                    pvtDayTotalDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                        "COMPANY_NO = " + this.pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND DAY_DATE = '" + this.pvtDateTime.ToString("yyyy-MM-dd") + "'",
                        "DAY_DATE",
                        DataViewRowState.CurrentRows);

                    this.lblDayDesc.Text = this.dgvEmployeeDataGridView[5, pvtintEmployeeDataGridViewRowIndex].Value.ToString() + " " + this.dgvEmployeeDataGridView[4, pvtintEmployeeDataGridViewRowIndex].Value.ToString() + "    " + this.dgvDayDataGridView[3, e.RowIndex].Value.ToString();

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
                "",
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

                pvtintTotalBreakMinutes += Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_ACCUM_MINUTES"]);

                this.dgvBreakDataGridView.Rows.Add(strClockTimeIn,
                                                   strActualTimeIn,
                                                   strActualTimeOut,
                                                   strClockTimeOut,
                                                   Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_ACCUM_MINUTES"]) / 60).ToString() + ":"
                                                   + Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_ACCUM_MINUTES"]) % 60).ToString("00"),
                                                   pvtBreakDataView[intRow]["BREAK_SEQ"].ToString());

                if (pvtBreakDataView[intRow]["INDICATOR"].ToString() == "X")
                {
                    dgvBreakDataGridView.Rows[dgvBreakDataGridView.Rows.Count - 1].HeaderCell.Style = ErrorDataGridViewCellStyle;

                    pvtblnBreakInError = true;
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
                "",
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
                        strClockTimeOut = Convert.ToInt32(pvtTimeSheetDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]).ToString();
                    }
                }

                pvtintTotalTimeSheetMinutes += Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_ACCUM_MINUTES"]);

                this.dgvTimeSheetDataGridView.Rows.Add(strClockTimeIn,
                                                       strActualTimeIn,
                                                       strActualTimeOut,
                                                       strClockTimeOut,
                                                       Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_ACCUM_MINUTES"]) / 60).ToString() + ":"
                                                       + Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_ACCUM_MINUTES"]) % 60).ToString("00"),
                                                       pvtTimeSheetDataView[intRow]["TIMESHEET_SEQ"].ToString());

                if (pvtTimeSheetDataView[intRow]["INDICATOR"].ToString() == "X")
                {
                    dgvTimeSheetDataGridView.Rows[dgvTimeSheetDataGridView.Rows.Count - 1].HeaderCell.Style = ErrorDataGridViewCellStyle;

                    pvtblnTimeSheetInError = true;
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

            if (this.dgvTimeSheetDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvTimeSheetDataGridView, 0);
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

            if (pvtBreakRangeDataView.Count > 0)
            {
                this.dgvBreakExceptionDataGridView[0, 0].Value = "=>>";

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

                                this.dgvBreakTotalsDataGridView[0, 0].Value = "<<=";

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

                                    this.dgvBreakTotalsDataGridView[0, 0].Value = "<<=";
                                }
                                else
                                {
                                    dgvBreakTotalsDataGridView[0, 0].Style = LunchTotalDataGridViewCellStyle;

                                    this.dgvBreakTotalsDataGridView[0, 0].Value = "";

                                    this.dgvBreakExceptionDataGridView[0, 0].Value = "=>>";
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

        private void dgvDayDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 3)
            {
                if (double.Parse(dgvDayDataGridView[pvtintDateColNo, e.RowIndex1].Value.ToString().Replace("-", "")) > double.Parse(dgvDayDataGridView[pvtintDateColNo, e.RowIndex2].Value.ToString().Replace("-", "")))
                {
                    e.SortResult = 1;
                }
                else
                {
                    if (double.Parse(dgvDayDataGridView[pvtintDateColNo, e.RowIndex1].Value.ToString().Replace("-", "")) < double.Parse(dgvDayDataGridView[pvtintDateColNo, e.RowIndex2].Value.ToString().Replace("-", "")))
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

        private void dgvAuthorisationLevelDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (this.pvtblnAuthorisationLevelDataGridViewLoaded == true
            & pvtintAuthorisationLevelDataGridViewRowIndex != e.RowIndex)
            {
                pvtintAuthorisationLevelDataGridViewRowIndex = e.RowIndex;

                pvtintAuthoriseLevel = Convert.ToInt32(this.dgvAuthorisationLevelDataGridView[2, e.RowIndex].Value);

                Load_Employee_SpreadSheet();
            }
        }

        private void btnAuthoriseAll_Click(object sender, EventArgs e)
        {
            for (int intRow = 0; intRow < this.dgvEmployeeDataGridView.Rows.Count; intRow++)
            {
                if (this.dgvEmployeeDataGridView.Rows[intRow].ReadOnly == false)
                {
                    this.dgvEmployeeDataGridView[6, intRow].Value = true;
                }
            }
        }

        private void AuthorisationType_Click(object sender, EventArgs e)
        {
            if (this.dgvAuthorisationLevelDataGridView.Rows.Count > 0)
            {
                if (this.rbnAuthorisationNone.Checked == false)
                {
                    this.grbFilterDisplay.Visible = true;
                    this.Set_DataGridView_SelectedRowIndex(dgvAuthorisationLevelDataGridView, this.pvtintAuthorisationLevelDataGridViewRowIndex);
                }
                else
                {
                    if (this.rbnNone.Checked == true
                        & this.chkRemoveBlanks.Checked == false)
                    {
                        rbnNone_Click(sender, e);
                    }
                    else
                    {
                        this.Set_DataGridView_SelectedRowIndex(dgvAuthorisationLevelDataGridView, this.pvtintAuthorisationLevelDataGridViewRowIndex);
                    }
                }
            }
        }

        private void btnRemoveFilter_Click(object sender, EventArgs e)
        {
            this.rbnAuthorisationNone.Checked = true;

            this.chkRemoveSat.Checked = false;
            this.chkRemoveSun.Checked = false;
            this.chkRemoveBlanks.Checked = false;

            this.rbnNone.Checked = true;

            rbnNone_Click(sender, e);
        }
    }
}
