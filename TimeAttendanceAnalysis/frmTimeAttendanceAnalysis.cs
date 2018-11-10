using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace InteractPayroll
{
    public partial class frmTimeAttendanceAnalysis : Form
    {
        clsISUtilities clsISUtilities;

        private byte[] pvtbytCompress;
        private DataSet pvtDataSet;
        private DataView pvtEmployeeDataView;
        private DataView pvtEmployeeEarningDataView;
        private DataView pvtPayCategoryDataView;
        private DataView pvtPayCategoryBreakRangeDataView;
        private DataView pvtPayCategoryWeekDataView;
        private DataView pvtEmployeeDayDataView;
        private DataView pvtEmployeeWeekDataView;
        private DataView pvtTimeSheetDataView;
        private DataView pvtBreakDataView;
        private DataView pvtTempDataView;
        private DataView pvtDayDataView;
        
        private int pvtintWeekNormalMinutes;
        private int pvtintWeekOverTime1Minutes;
        private int pvtintWeekOverTime2Minutes;
        private int pvtintWeekOverTime3Minutes;

        private int pvtintOverTime1HoursBoundary;
        private int pvtintOverTime2HoursBoundary;
        private int pvtintOverTime3HoursBoundary;

        private int pvtintDayNormalHours;
        private int pvtintDayOverTime1Hours;
        private int pvtintDayOverTime2Hours;
        private int pvtintDayOverTime3Hours;

        private DateTime pvtDateFind;

        private string pvtstrPayrollType = "";

        private int pvtintEmployeeNo = -1;
        private int pvtintPayCategoryNo = -1;
        private int pvtintPayCategoryTableRowNo = -1;
        
        private int pvtintEmployeeIndex = -1;

        private int pvtintFindPayCategoryWeekRow = -1;

        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
        private int pvtintEmployeeDataGridViewRowIndex = -1;
        private int pvtintDayDataGridViewRowIndex = -1;
        private int pvtintPayCategoryDataGridViewRowIndex = -1;
        private int pvtintWeekDataGridViewRowIndex = -1;

        private string pvtstrDataFilter = "";

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnEmployeeDataGridViewLoaded = false;
        private bool pvtblnPayCategoryDataGridViewLoaded = false;
        private bool pvtblnWeekDataGridViewLoaded = false;
        private bool pvtblnDayDataGridViewLoaded = false;

        //dgvEmployeeDataGridView Cols
        private int pvtintCodeColEmployeeDataGridView = 4;
        private int pvtintNamesColEmployeeDataGridView = 5;
        private int pvtintNTColEmployeeDataGridView = 6;
        private int pvtintOT1ColEmployeeDataGridView = 7;
        private int pvtintOT2ColEmployeeDataGridView = 8;
        private int pvtintOT3ColEmployeeDataGridView = 9;
        private int pvtintPHColEmployeeDataGridView = 10;
        private int pvtintIndexColEmployeeDataGridView = 11;

        //dgvWeekDataGridView Cols
        private int pvtintWeekDateColWeekDataGridView = 4;
        private int pvtintNTColWeekDataGridView = 5;
        private int pvtintOT1ColWeekDataGridView = 6;
        private int pvtintOT2ColWeekDataGridView = 7;
        private int pvtintOT3ColWeekDataGridView = 8;
        private int pvtintPHColWeekDataGridView = 9;
        private int pvtintIndexColWeekDataGridView = 10;

        //dgvDayDataGridView Cols
        private int pvtintDayDescColDayDataGridView = 4;
        private int pvtintExceptionColDayDataGridView = 5;
        private int pvtintOTBoundaryColDayDataGridView = 6;
        private int pvtintPaidHoursColDayDataGridView = 7;
        private int pvtintNTColDayDataGridView = 8;
        private int pvtintOT1ColDayDataGridView = 9;
        private int pvtintOT2ColDayDataGridView = 10;
        private int pvtintOT3ColDayDataGridView = 11;
        private int pvtintPHColDayDataGridView = 12;
        private int pvtintDateColDayDataGridView = 13;

        DataGridViewCellStyle ExceptionDataGridViewCellStyle;
        DataGridViewCellStyle BlankDataGridViewCellStyle;
        DataGridViewCellStyle PaidHolidayDataGridViewCellStyle;
        DataGridViewCellStyle PayTotalDataGridViewCellStyle;
        DataGridViewCellStyle WeekEndDataGridViewCellStyle;
        DataGridViewCellStyle NotIncludedInRunDataGridViewCellStyle;

        DataGridViewCellStyle BreakExceptionDataGridViewCellStyle;
        DataGridViewCellStyle LunchDataGridViewCellStyle;
        DataGridViewCellStyle NormalDataGridViewCellStyle;
        DataGridViewCellStyle OvertimeDataGridViewCellStyle;
        DataGridViewCellStyle TotalDataGridViewCellStyle;
        
        public frmTimeAttendanceAnalysis()
        {
            InitializeComponent();
            
            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 119;

                this.grbFilter.Top += 88;
                this.grbDataLegend.Top += 87;
                this.grbNamesOrder.Top += 87;

                this.lblDayDesc.Top += 87;
                this.lblTimesheets.Top += 87;
                this.lblTimeSheetStart.Top += 87;
                this.lblTimeSheetStop.Top += 87;
                this.lblTimeSheetAccum.Top += 87;
                this.dgvTimeSheetDataGridView.Top += 87;
                this.dgvTimeSheetDataGridView.Height += 19;

                this.lblBreakRange.Top += 106;
                this.dgvBreakRangeDataGridView.Top += 106;
                this.dgvBreakExceptionDataGridView.Top += 106;
                this.dgvTimeSheetTotalsDataGridView.Top += 106;
                
                this.lblBreaks.Top += 87;
                this.lblBreakStart.Top += 87;
                this.lblBreakStop.Top += 87;
                this.lblBreakAccum.Top += 87;
                this.dgvBreakDataGridView.Top += 87;
                this.dgvBreakDataGridView.Height += 19;

                this.dgvBreakTotalsDataGridView.Top += 100;

                this.dgvPayCategoryDataGridView.Height += 38;

                int intEmployeeTop = 41;

                this.lblEmployee.Top += intEmployeeTop;
                this.dgvEmployeeDataGridView.Top += intEmployeeTop;
                this.dgvEmployeeDataGridView.Height += 38;

                this.lblWeek.Top += 81;
                this.dgvWeekDataGridView.Top += 81;
                this.dgvWeekDataGridView.Height += 38;
                this.dgvWeekTotalDataGridView.Top += 119;

                this.lblDate.Top += 119;
                this.picDateFilter.Top += 119;
                this.dgvDayDataGridView.Top += 119;
                this.dgvDayTotalDataGridView.Top += 119;
            }
        }

        private void frmTimeAttendanceAnalysis_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this,"busTimeAttendanceAnalysis");
              
                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblDate.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblWeek.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblDayDesc.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblTimesheets.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblBreaks.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblTimeSheetStart.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblTimeSheetStop.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblTimeSheetAccum.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblBreakStart.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblBreakStop.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblBreakAccum.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblBreakRange.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                
                ExceptionDataGridViewCellStyle = new DataGridViewCellStyle();
                ExceptionDataGridViewCellStyle.BackColor = Color.Lime;
                ExceptionDataGridViewCellStyle.SelectionBackColor = Color.Lime;

                BlankDataGridViewCellStyle = new DataGridViewCellStyle();
                BlankDataGridViewCellStyle.BackColor = Color.Yellow;
                BlankDataGridViewCellStyle.SelectionBackColor = Color.Yellow;

                PaidHolidayDataGridViewCellStyle = new DataGridViewCellStyle();
                PaidHolidayDataGridViewCellStyle.BackColor = Color.SlateBlue;
                PaidHolidayDataGridViewCellStyle.SelectionBackColor = Color.SlateBlue;

                PayTotalDataGridViewCellStyle = new DataGridViewCellStyle();
                PayTotalDataGridViewCellStyle.BackColor = SystemColors.ControlDark;
                PayTotalDataGridViewCellStyle.SelectionBackColor = SystemColors.ControlDark;

                NotIncludedInRunDataGridViewCellStyle = new DataGridViewCellStyle();
                NotIncludedInRunDataGridViewCellStyle.BackColor = Color.Orange;
                NotIncludedInRunDataGridViewCellStyle.SelectionBackColor = Color.Orange;

                WeekEndDataGridViewCellStyle = new DataGridViewCellStyle();
                WeekEndDataGridViewCellStyle.BackColor = SystemColors.Info;
                
                BreakExceptionDataGridViewCellStyle = new DataGridViewCellStyle();
                BreakExceptionDataGridViewCellStyle.BackColor = Color.Teal;
                BreakExceptionDataGridViewCellStyle.SelectionBackColor = Color.Teal;

                LunchDataGridViewCellStyle = new DataGridViewCellStyle();
                LunchDataGridViewCellStyle.BackColor = Color.Silver;
                LunchDataGridViewCellStyle.SelectionBackColor = Color.Silver;

                NormalDataGridViewCellStyle = new DataGridViewCellStyle();
                NormalDataGridViewCellStyle.BackColor = SystemColors.Control;
                NormalDataGridViewCellStyle.SelectionBackColor = SystemColors.Control;

                OvertimeDataGridViewCellStyle = new DataGridViewCellStyle();
                OvertimeDataGridViewCellStyle.BackColor = Color.Black;
                OvertimeDataGridViewCellStyle.ForeColor = Color.White;
                OvertimeDataGridViewCellStyle.SelectionBackColor = Color.Black;
                OvertimeDataGridViewCellStyle.SelectionForeColor = Color.White;

                TotalDataGridViewCellStyle = new DataGridViewCellStyle();
                TotalDataGridViewCellStyle.BackColor = SystemColors.ControlDarkDark;
                TotalDataGridViewCellStyle.SelectionBackColor = SystemColors.ControlDarkDark;

                this.dgvWeekTotalDataGridView.Rows.Add("Pay Period Totals");
                this.dgvWeekTotalDataGridView.Rows.Add("Pay Period Totals (Rounded)");
                this.dgvWeekTotalDataGridView.Rows.Add("Pay Period Totals (Decimal)");

                this.dgvWeekTotalDataGridView.Rows[2].DefaultCellStyle = this.TotalDataGridViewCellStyle;

                this.dgvDayTotalDataGridView.Rows.Add("Week Totals"
                                                      , "");
             
                dgvDayTotalDataGridView[1, 0].Style = this.OvertimeDataGridViewCellStyle;
                
                this.dgvBreakTotalsDataGridView.Rows.Add("",
                                                        "Total Break Hours",
                                                        "0:00");

                this.dgvBreakExceptionDataGridView.Rows.Add("");

                this.dgvTimeSheetTotalsDataGridView.Rows.Add("Total Worked Hours");
                this.dgvTimeSheetTotalsDataGridView.Rows.Add("Break After 0:00");
                this.dgvTimeSheetTotalsDataGridView.Rows.Add("Total Paid Hours");
                this.dgvTimeSheetTotalsDataGridView.Rows.Add("");

                this.dgvTimeSheetTotalsDataGridView.Rows[3].DefaultCellStyle = this.TotalDataGridViewCellStyle;

                object[] objParm = new object[2];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();
                   
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                                
                for (int intRow = 0; intRow < pvtDataSet.Tables["PayrollType"].Rows.Count; intRow++)
                {
                    this.dgvPayrollTypeDataGridView.Rows.Add(pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE"].ToString());
                }

                this.pvtblnPayrollTypeDataGridViewLoaded = true;

                this.Set_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView, 0);
                
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
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
         
        private void Load_CurrentForm_Records()
        {
            this.lblDayDesc.Text = "";
            this.lblDate.Text = "";

            this.dgvBreakTotalsDataGridView[1, 0].Value = "Total Break Hours";
            this.dgvBreakTotalsDataGridView[2, 0].Value = "0:00";

            this.dgvTimeSheetTotalsDataGridView[0,0].Value = "Total Worked Hours";
            this.dgvTimeSheetTotalsDataGridView[0,1].Value = "Break After 0:00";
            this.dgvTimeSheetTotalsDataGridView[0,2].Value = "Total Paid Hours";
            this.dgvTimeSheetTotalsDataGridView[0,3].Value = "";

            this.dgvWeekTotalDataGridView[0, 1].Value = "";
            this.dgvDayTotalDataGridView[1,0].Value = "";

            Clear_Totals();

            string strFilter = "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'";

            pvtstrDataFilter = "";

            if (this.rbnBreakException.Checked == true)
            {
                pvtstrDataFilter = " AND BREAK_INDICATOR = 'Y'";
            }
            else
            {
                if (this.rbnException.Checked == true)
                {
                    pvtstrDataFilter = " AND INDICATOR = 'E'";
                }
                else
                {
                    if (this.rbnNormal.Checked == true)
                    {
                        pvtstrDataFilter = " AND INDICATOR = ''";
                    }
                    else
                    {
                        if (this.rbnPublicHoliday.Checked == true)
                        {
                            pvtstrDataFilter = " AND PAIDHOLIDAY_INDICATOR = 'Y'";
                        }
                        else
                        {
                            if (this.rbnExcludedRun.Checked == true)
                            {
                                pvtstrDataFilter = " AND INCLUDED_IN_RUN_INDICATOR = 'N'";
                            }
                        }
                    }
                }
            }

            if (this.chkRemoveSat.Checked == true
            | this.chkRemoveSun.Checked == true)
            {
                pvtstrDataFilter += " AND NOT DAY_NO IN (";

                if (this.chkRemoveSat.Checked == true)
                {
                    pvtstrDataFilter += "6";
                }

                if (this.chkRemoveSun.Checked == true)
                {
                    if (this.chkRemoveSat.Checked == true)
                    {
                        pvtstrDataFilter += ",0";
                    }
                    else
                    {
                        pvtstrDataFilter += "0";
                    }
                }

                pvtstrDataFilter += ")";
            }
   
            pvtPayCategoryDataView = null;
            pvtPayCategoryDataView = new DataView(this.pvtDataSet.Tables["PayCategory"],
                strFilter,
                "",
                DataViewRowState.CurrentRows);

            this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
            this.Clear_DataGridView(this.dgvEmployeeDataGridView);
            this.Clear_DataGridView(this.dgvWeekDataGridView);
            this.Clear_DataGridView(this.dgvDayDataGridView);
            this.Clear_DataGridView(this.dgvTimeSheetDataGridView);
            this.Clear_DataGridView(this.dgvBreakDataGridView);
            this.Clear_DataGridView(this.dgvBreakRangeDataGridView);

            this.pvtblnPayCategoryDataGridViewLoaded = false;

            //2012-10-25
            this.pvtblnEmployeeDataGridViewLoaded = false;
            this.pvtblnWeekDataGridViewLoaded = false;
            this.pvtblnDayDataGridViewLoaded = false;
                    
            for (int intRow = 0; intRow < pvtPayCategoryDataView.Count; intRow++)
            {
                pvtTempDataView = null;
                pvtTempDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                    strFilter + " AND PAY_CATEGORY_NO = " + pvtPayCategoryDataView[intRow]["PAY_CATEGORY_NO"].ToString() + pvtstrDataFilter,
                    "INDICATOR DESC",
                    DataViewRowState.CurrentRows);
              
                if ((this.rbnException.Checked == true
                || this.rbnNormal.Checked == true
                || this.rbnBreakException.Checked == true
                || this.rbnPublicHoliday.Checked == true
                || this.rbnExcludedRun.Checked == true
                || this.chkRemoveBlanks.Checked == true)
                && pvtTempDataView.Count == 0)
                {
                    continue;
                }
                
                this.dgvPayCategoryDataGridView.Rows.Add("",
                                                         "",
                                                         "",
                                                         "",
                                                         pvtPayCategoryDataView[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                         Convert.ToDateTime(pvtPayCategoryDataView[intRow]["PAY_PERIOD_DATE_FROM"]).ToString("dd MMM yyyy") + " - " + Convert.ToDateTime(pvtPayCategoryDataView[intRow]["PAY_PERIOD_DATE"]).ToString("dd MMM yyyy"),
                                                         intRow.ToString());

                if (pvtTempDataView.Count > 0)
                {
                    if (this.rbnBlank.Checked == true)
                    {
                        this.dgvPayCategoryDataGridView[0,this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = this.BlankDataGridViewCellStyle;
                    }
                    else
                    {
                        if (pvtTempDataView[0]["INDICATOR"].ToString() == "E")
                        {
                            this.dgvPayCategoryDataGridView[0,this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = this.ExceptionDataGridViewCellStyle;
                        }

                        pvtTempDataView.Sort = "BREAK_INDICATOR DESC";

                        if (pvtTempDataView[0]["BREAK_INDICATOR"].ToString() == "Y")
                        {
                            dgvPayCategoryDataGridView[1, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = this.BreakExceptionDataGridViewCellStyle;
                        }

                        pvtTempDataView.Sort = "PAIDHOLIDAY_INDICATOR DESC";

                        if (pvtTempDataView[0]["PAIDHOLIDAY_INDICATOR"].ToString() == "Y")
                        {
                            dgvPayCategoryDataGridView[2, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = this.PaidHolidayDataGridViewCellStyle;
                        }

                        pvtTempDataView.Sort = "INCLUDED_IN_RUN_INDICATOR";

                        if (pvtTempDataView[0]["INCLUDED_IN_RUN_INDICATOR"].ToString() == "N")
                        {
                            this.dgvPayCategoryDataGridView[3, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = this.NotIncludedInRunDataGridViewCellStyle;
                        }
                    }
                }
                else
                {
                    this.dgvPayCategoryDataGridView[0,this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = this.BlankDataGridViewCellStyle;
                }
            }

            this.pvtblnPayCategoryDataGridViewLoaded = true;

            //Select First Row
            if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView, 0);
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

                case "dgvWeekDataGridView":

                    pvtintWeekDataGridViewRowIndex = -1;
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

                        this.dgvPayrollTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPayCategoryDataGridView":

                        this.dgvPayCategoryDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEmployeeDataGridView":

                        this.dgvEmployeeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvWeekDataGridView":

                        this.dgvWeekDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvDayDataGridView":

                        this.dgvDayDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvTimeSheetDataGridView":

                        this.dgvTimeSheetDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvBreakDataGridView":

                        this.dgvBreakDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        private void Clear_Totals()
        {
            this.lblDayDesc.Text = "";
            this.lblDate.Text = "";

            this.dgvTimeSheetTotalsDataGridView[1, 0].Value = "0:00";
            this.dgvTimeSheetTotalsDataGridView[1, 1].Value = "0:00";
            this.dgvTimeSheetTotalsDataGridView[1, 2].Value = "0:00";
            this.dgvTimeSheetTotalsDataGridView[1, 3].Value = "0:00";
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void Load_TimeSheets()
        {
            int intTotalTimeSheetMinutes = 0;
            int intTotalBreakMinutes = 0;
            int intMinutesDifference = 0;
            string strClockIn = "";
            string strActualIn = "";
            string strActualOut = "";
            string strClockOut = "";
           
            pvtBreakDataView = null;
            pvtBreakDataView = new DataView(pvtDataSet.Tables["Break"],
                "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + this.dgvPayrollTypeDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) + "' AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND BREAK_DATE = '" + pvtDateFind.ToString("yyyy-MM-dd") + "'",
                "",
                DataViewRowState.CurrentRows);

            this.Clear_DataGridView(this.dgvBreakDataGridView);

            //Set Indicator
            for (int intRow = 0; intRow < this.pvtBreakDataView.Count; intRow++)
            {
                if (pvtBreakDataView[intRow]["CLOCKED_TIME_IN_MINUTES"] == System.DBNull.Value)
                {
                    strClockIn = "";
                }
                else
                {
                    if (Convert.ToInt32(pvtBreakDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) > 59)
                    {
                        strClockIn = Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) / 60).ToString()
                                                               + Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strClockIn = Convert.ToInt32(pvtBreakDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]).ToString();
                    }
                }

                if (pvtBreakDataView[intRow]["BREAK_TIME_IN_MINUTES"] != System.DBNull.Value)
                {
                    if (Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_TIME_IN_MINUTES"]) > 59)
                    {
                        strActualIn = Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_TIME_IN_MINUTES"]) / 60).ToString()
                                                               + Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_TIME_IN_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strActualIn = Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_TIME_IN_MINUTES"]).ToString();
                    }
                }
                else
                {
                    strActualIn = "";
                }

                if (pvtBreakDataView[intRow]["BREAK_TIME_OUT_MINUTES"] != System.DBNull.Value)
                {
                    if (Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_TIME_OUT_MINUTES"]) > 59)
                    {
                        strActualOut = Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_TIME_OUT_MINUTES"]) / 60).ToString()
                                                               + Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_TIME_OUT_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strActualOut = Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_TIME_OUT_MINUTES"]).ToString();
                    }
                }
                else
                {
                    strActualOut = "";
                }

                if (pvtBreakDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"] == System.DBNull.Value)
                {
                    strClockOut = "";
                }
                else
                {
                    if (Convert.ToInt32(pvtBreakDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) > 59)
                    {
                        strClockOut = Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) / 60).ToString()
                                                               + Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strClockOut = Convert.ToInt32(pvtBreakDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]).ToString();
                    }
                }


                if (pvtBreakDataView[intRow]["BREAK_TIME_IN_MINUTES"] != System.DBNull.Value
                && pvtBreakDataView[intRow]["BREAK_TIME_OUT_MINUTES"] != System.DBNull.Value)
                {
                    intMinutesDifference = Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_TIME_OUT_MINUTES"]) - Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_TIME_IN_MINUTES"]);
                }
                else
                {
                    intMinutesDifference = 0;
                }

                this.dgvBreakDataGridView.Rows.Add(strClockIn,
                                                   strActualIn,
                                                   strActualOut,
                                                   strClockOut,
                                                   Convert.ToInt32(intMinutesDifference / 60).ToString() + ":" + Convert.ToInt32(intMinutesDifference % 60).ToString("00"),
                                                   pvtBreakDataView[intRow]["BREAK_SEQ"].ToString());


                if (pvtBreakDataView[intRow]["INCLUDED_IN_RUN_IND"].ToString() != "Y")
                {
                    this.dgvBreakDataGridView.Rows[this.dgvBreakDataGridView.Rows.Count - 1].HeaderCell.Style = this.NotIncludedInRunDataGridViewCellStyle;
                }
                else
                {
                    intTotalBreakMinutes += intMinutesDifference;
                }
           }

            if (intTotalBreakMinutes == 0)
            {
                this.dgvBreakTotalsDataGridView[2,0].Value = "0:00";
            }
            else
            {
                this.dgvBreakTotalsDataGridView[2, 0].Value = Convert.ToInt32(intTotalBreakMinutes / 60).ToString() + ":" + Convert.ToInt32(intTotalBreakMinutes % 60).ToString("00");
            }

            if (pvtBreakDataView.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvBreakDataGridView, 0);
            }

            pvtTimeSheetDataView = null;
            pvtTimeSheetDataView = new DataView(pvtDataSet.Tables["TimeSheet"],
                "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + this.dgvPayrollTypeDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) + "' AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND TIMESHEET_DATE = '" + pvtDateFind.ToString("yyyy-MM-dd") + "'",
                "",
                DataViewRowState.CurrentRows);

            this.Clear_DataGridView(this.dgvTimeSheetDataGridView);

            //Set Indicator
            for (int intRow = 0; intRow < this.pvtTimeSheetDataView.Count; intRow++)
            {
                if (pvtTimeSheetDataView[intRow]["CLOCKED_TIME_IN_MINUTES"] == System.DBNull.Value)
                {
                    strClockIn = "";
                }
                else
                {
                    if (Convert.ToInt32(pvtTimeSheetDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) > 59)
                    {
                        strClockIn = Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) / 60).ToString()
                                                                       + Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strClockIn = Convert.ToInt32(pvtTimeSheetDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]).ToString();
                    }
                }

                if (pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_IN_MINUTES"] != System.DBNull.Value)
                {
                    if (Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_IN_MINUTES"]) > 59)
                    {
                        strActualIn = Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_IN_MINUTES"]) / 60).ToString()
                                                                       + Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_IN_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strActualIn = Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_IN_MINUTES"]).ToString();
                    }
                }
                else
                {
                    strActualIn = "";
                }

                if (pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_OUT_MINUTES"] != System.DBNull.Value)
                {
                    if (Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_OUT_MINUTES"]) > 59)
                    {
                        strActualOut = Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_OUT_MINUTES"]) / 60).ToString()
                                                                       + Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_OUT_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strActualOut = Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_OUT_MINUTES"]).ToString();
                    }
                }
                else
                {
                    strActualOut = "";
                }

                if (pvtTimeSheetDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"] == System.DBNull.Value)
                {
                    strClockOut = "";
                }
                else
                {
                    if (Convert.ToInt32(pvtTimeSheetDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) > 59)
                    {
                        strClockOut = Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) / 60).ToString()
                                                                       + Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strClockOut = Convert.ToInt32(pvtTimeSheetDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]).ToString();
                    }
                }

                if (pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_IN_MINUTES"] != System.DBNull.Value
                    && pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_OUT_MINUTES"] != System.DBNull.Value)
                {
                    intMinutesDifference = Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_OUT_MINUTES"]) - Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_IN_MINUTES"]);
                }
                else
                {
                    intMinutesDifference = 0;
                }
                
                this.dgvTimeSheetDataGridView.Rows.Add(strClockIn,
                                                       strActualIn,
                                                       strActualOut,
                                                       strClockOut,
                                                       Convert.ToInt32(intMinutesDifference / 60).ToString() + ":" + Convert.ToInt32(intMinutesDifference % 60).ToString("00"),
                                                       pvtTimeSheetDataView[intRow]["TIMESHEET_SEQ"].ToString());

                if (pvtTimeSheetDataView[intRow]["INCLUDED_IN_RUN_IND"].ToString() != "Y")
                {
                    this.dgvTimeSheetDataGridView.Rows[this.dgvTimeSheetDataGridView.Rows.Count - 1].HeaderCell.Style = this.NotIncludedInRunDataGridViewCellStyle;
                }
                else
                {
                    intTotalTimeSheetMinutes += intMinutesDifference;
                }
            }

            int intDayPaidHours = 0;
            int intBreakTimeMinutes = 0;
            
            if (pvtPayCategoryBreakRangeDataView.Count > 0)
            {
                this.dgvBreakExceptionDataGridView[0, 0].Value = "=>>";

                if (intTotalTimeSheetMinutes == 0
                    & intTotalBreakMinutes == 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvBreakRangeDataGridView, 0);

                    this.dgvTimeSheetTotalsDataGridView[0,1].Value = "Break After 0:00";

                    dgvBreakTotalsDataGridView[0, 0].Style = this.LunchDataGridViewCellStyle;

                    this.dgvBreakTotalsDataGridView[0, 0].Value = "";
                }
                else
                {
                    for (int intBreakRow = 0; intBreakRow < this.pvtPayCategoryBreakRangeDataView.Count; intBreakRow++)
                    {
                        if (Convert.ToInt32(pvtPayCategoryBreakRangeDataView[intBreakRow]["WORKED_TIME_MINUTES"]) <= intTotalTimeSheetMinutes)
                        {
                            intBreakTimeMinutes = Convert.ToInt32(pvtPayCategoryBreakRangeDataView[intBreakRow]["BREAK_MINUTES"]);

                            this.dgvTimeSheetTotalsDataGridView[0, 1].Value = "Break After " + Convert.ToInt32(Convert.ToInt32(pvtPayCategoryBreakRangeDataView[intBreakRow]["WORKED_TIME_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtPayCategoryBreakRangeDataView[intBreakRow]["WORKED_TIME_MINUTES"]) % 60).ToString("00");

                            if (intBreakTimeMinutes < intTotalBreakMinutes)
                            {
                                intBreakTimeMinutes = intTotalBreakMinutes;

                                dgvBreakTotalsDataGridView[0, 0].Style = this.BreakExceptionDataGridViewCellStyle;

                                this.dgvBreakExceptionDataGridView[0, 0].Value = "";

                                this.dgvBreakTotalsDataGridView[0, 0].Value = "<<=";
                            }
                            else
                            {
                                dgvBreakTotalsDataGridView[0, 0].Style = LunchDataGridViewCellStyle;
                                this.dgvBreakTotalsDataGridView[0, 0].Value = "";

                                this.dgvBreakExceptionDataGridView[0, 0].Value = "=>>";
                            }

                            this.Set_DataGridView_SelectedRowIndex(this.dgvBreakRangeDataGridView, intBreakRow);
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
                dgvBreakTotalsDataGridView[0, 0].Style = LunchDataGridViewCellStyle;
                this.dgvBreakTotalsDataGridView[0, 0].Value = "";
                this.dgvTimeSheetTotalsDataGridView[0, 1].Value = "Break After 0:00";

                this.dgvBreakExceptionDataGridView[0, 0].Value = "";
            }

            this.dgvTimeSheetTotalsDataGridView[1,0].Value = Convert.ToInt32(intTotalTimeSheetMinutes / 60).ToString() + ":" + Convert.ToInt32(intTotalTimeSheetMinutes % 60).ToString("00");

            if (intTotalTimeSheetMinutes >= intBreakTimeMinutes)
            {
                intDayPaidHours = intTotalTimeSheetMinutes - intBreakTimeMinutes;

                this.dgvTimeSheetTotalsDataGridView[1,1].Value = Convert.ToInt32(intBreakTimeMinutes / 60).ToString() + ":" + Convert.ToInt32(intBreakTimeMinutes % 60).ToString("00");
                this.dgvTimeSheetTotalsDataGridView[1,2].Value = Convert.ToInt32(intDayPaidHours / 60).ToString() + ":" + Convert.ToInt32(intDayPaidHours % 60).ToString("00");
            }
            else
            {
                this.dgvTimeSheetTotalsDataGridView[1, 1].Value = "0:00";
                this.dgvTimeSheetTotalsDataGridView[1,2].Value = this.dgvTimeSheetTotalsDataGridView[1,0].Value;

                intDayPaidHours = intTotalTimeSheetMinutes;
            }

            //Round Day - Return 'intDayPaidHours'
            clsISUtilities.Round_For_Period(Convert.ToInt32(pvtPayCategoryWeekDataView[this.Get_DataGridView_SelectedRowIndex(this.dgvWeekDataGridView)]["DAILY_ROUNDING_IND"]), Convert.ToInt32(pvtPayCategoryWeekDataView[this.Get_DataGridView_SelectedRowIndex(this.dgvWeekDataGridView)]["DAILY_ROUNDING_MINUTES"]), ref intDayPaidHours);

            this.dgvTimeSheetTotalsDataGridView[1,3].Value = Convert.ToInt32(intDayPaidHours / 60).ToString() + ":" + Convert.ToInt32(intDayPaidHours % 60).ToString("00");

            if (this.dgvTimeSheetDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvTimeSheetDataGridView, 0);
            }
        }

        private double Convert_HoursMinutes_To_Decimal(double pardblTime)
        {
            double dblTimeMM;
            double dblTimeHH;
            double dblNewTime;

            if (pardblTime.ToString().Length >= 3)
            {
                dblTimeMM = Convert.ToDouble(pardblTime.ToString().Substring(pardblTime.ToString().Length - 2, 2));
                dblTimeHH = Convert.ToDouble(pardblTime.ToString().Substring(0, pardblTime.ToString().Length - 2));
            }
            else
            {
                dblTimeMM = pardblTime;
                dblTimeHH = 0;
            }

            if (dblTimeMM != 0)
            {
                dblTimeMM = (dblTimeMM / 60) * 100;
            }

            dblNewTime = Convert.ToDouble(dblTimeHH.ToString() + "." + dblTimeMM.ToString("00"));

            return dblNewTime;
        }

        private void rbnNone_Click(object sender, System.EventArgs e)
        {
            Clear_SpreadSheet_Data();

            this.chkRemoveBlanks.Enabled = true;
            this.chkRemoveSat.Checked = false;
            this.chkRemoveSun.Checked = false;
            this.chkRemoveSat.Enabled = false;
            this.chkRemoveSun.Enabled = false;

            Load_CurrentForm_Records();
        }

        private void Clear_SpreadSheet_Data()
        {
            this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
            this.Clear_DataGridView(this.dgvEmployeeDataGridView);
            this.Clear_DataGridView(this.dgvWeekDataGridView);
            this.Clear_DataGridView(this.dgvDayDataGridView);
            this.Clear_DataGridView(this.dgvTimeSheetDataGridView);
            this.Clear_DataGridView(this.dgvBreakDataGridView);

            //Row 0
            this.dgvWeekTotalDataGridView[1, 0].Value = "";
            this.dgvWeekTotalDataGridView[2, 0].Value = "";
            this.dgvWeekTotalDataGridView[3, 0].Value = "";
            this.dgvWeekTotalDataGridView[4, 0].Value = "";
            this.dgvWeekTotalDataGridView[5, 0].Value = "";

            //Row 1
            this.dgvWeekTotalDataGridView[1, 1].Value = "";
            this.dgvWeekTotalDataGridView[2, 1].Value = "";
            this.dgvWeekTotalDataGridView[3, 1].Value = "";
            this.dgvWeekTotalDataGridView[4, 1].Value = "";
            this.dgvWeekTotalDataGridView[5, 1].Value = "";

            //Row 2
            this.dgvWeekTotalDataGridView[1, 2].Value = "";
            this.dgvWeekTotalDataGridView[2, 2].Value = "";
            this.dgvWeekTotalDataGridView[3, 2].Value = "";
            this.dgvWeekTotalDataGridView[4, 2].Value = "";
            this.dgvWeekTotalDataGridView[5, 2].Value = "";

            //Row 0
            this.dgvDayTotalDataGridView[1, 0].Value = "";
            this.dgvDayTotalDataGridView[2, 0].Value = "";
            this.dgvDayTotalDataGridView[3, 0].Value = "";
            this.dgvDayTotalDataGridView[4, 0].Value = "";
            this.dgvDayTotalDataGridView[5, 0].Value = "";
            this.dgvDayTotalDataGridView[6, 0].Value = "";
            this.dgvDayTotalDataGridView[7, 0].Value = "";

            this.dgvTimeSheetTotalsDataGridView[1, 0].Value = "";
            this.dgvTimeSheetTotalsDataGridView[1, 1].Value = "";
            this.dgvTimeSheetTotalsDataGridView[1, 2].Value = "";
            this.dgvTimeSheetTotalsDataGridView[1, 3].Value = "";

            dgvBreakTotalsDataGridView[0, 0].Style = this.LunchDataGridViewCellStyle;

            this.dgvBreakTotalsDataGridView[0, 0].Value = "";
        }

        private void rbnException_Click(object sender, System.EventArgs e)
        {
            Clear_SpreadSheet_Data();

            this.chkRemoveBlanks.Checked = false;
            this.chkRemoveBlanks.Enabled = false;
            this.chkRemoveSat.Checked = false;
            this.chkRemoveSun.Checked = false;
            this.chkRemoveSat.Enabled = false;
            this.chkRemoveSun.Enabled = false;

            Load_CurrentForm_Records();
        }

        private void rbnBlank_Click(object sender, EventArgs e)
        {
            Clear_SpreadSheet_Data();

            this.chkRemoveBlanks.Checked = false;
            this.chkRemoveBlanks.Enabled = false;
            this.chkRemoveSat.Enabled = true;
            this.chkRemoveSun.Enabled = true;

            Load_CurrentForm_Records();
        }

        private void rbnPublicHoliday_Click(object sender, EventArgs e)
        {
            Clear_SpreadSheet_Data();

            this.chkRemoveBlanks.Checked = false;
            this.chkRemoveBlanks.Enabled = false;
            this.chkRemoveSat.Checked = false;
            this.chkRemoveSun.Checked = false;
            this.chkRemoveSat.Enabled = false;
            this.chkRemoveSun.Enabled = false;

            Load_CurrentForm_Records();
        }

        private void rbnNormal_Click(object sender, EventArgs e)
        {
            Clear_SpreadSheet_Data();

            this.chkRemoveBlanks.Checked = false;
            this.chkRemoveBlanks.Enabled = false;
            this.chkRemoveSat.Checked = false;
            this.chkRemoveSun.Checked = false;
            this.chkRemoveSat.Enabled = false;
            this.chkRemoveSun.Enabled = false;

            Load_CurrentForm_Records();
        }

        private void chkRemoveSat_Click(object sender, EventArgs e)
        {
            Load_CurrentForm_Records();

            //int intWeekRow = this.Get_DataGridView_SelectedRowIndex(this.dgvWeekDataGridView);

            //if (this.dgvWeekDataGridView.Rows.Count > 0)
            //{
            //    this.Set_DataGridView_SelectedRowIndex(dgvWeekDataGridView, intWeekRow);
            //}
        }

        private void chkRemoveSun_Click(object sender, EventArgs e)
        {

            Load_CurrentForm_Records();

            //int intWeekRow = this.Get_DataGridView_SelectedRowIndex(this.dgvWeekDataGridView);

            //if (this.dgvWeekDataGridView.Rows.Count > 0)
            //{
            //    this.Set_DataGridView_SelectedRowIndex(dgvWeekDataGridView, intWeekRow);
            //}
        }

        private void chkRemoveBlanks_Click(object sender, EventArgs e)
        {
            this.Load_CurrentForm_Records();
        }

        private void rbnBreakException_Click(object sender, EventArgs e)
        {
            Clear_SpreadSheet_Data();

            this.chkRemoveBlanks.Checked = false;
            this.chkRemoveBlanks.Enabled = false;
            this.chkRemoveSat.Checked = false;
            this.chkRemoveSun.Checked = false;
            this.chkRemoveSat.Enabled = false;
            this.chkRemoveSun.Enabled = false;

            Load_CurrentForm_Records();
        }

        private void dgvPayrollTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPayrollTypeDataGridViewLoaded == true)
            {
                if (pvtintPayrollTypeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayrollTypeDataGridViewRowIndex = e.RowIndex;

                    pvtstrPayrollType = dgvPayrollTypeDataGridView[0, e.RowIndex].Value.ToString().Substring(0, 1);

                    Load_CurrentForm_Records();
                }
            }
        }

        private void dgvPayCategoryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (this.pvtblnPayCategoryDataGridViewLoaded == true)
                {
                    if (pvtintPayCategoryDataGridViewRowIndex != e.RowIndex)
                    {
                        pvtintPayCategoryDataGridViewRowIndex = e.RowIndex;

                        //2012-10-25
                        this.pvtblnEmployeeDataGridViewLoaded = false;
                        this.pvtblnWeekDataGridViewLoaded = false;
                        this.pvtblnDayDataGridViewLoaded = false;

                        //Zerorize Totals
                        Clear_Totals();

                        pvtintPayCategoryTableRowNo = Convert.ToInt32(this.dgvPayCategoryDataGridView[6, e.RowIndex].Value);

                        pvtintPayCategoryNo = Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_CATEGORY_NO"]);

                        string strFilter = "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo;

                        pvtPayCategoryBreakRangeDataView = null;
                        pvtPayCategoryBreakRangeDataView = new DataView(this.pvtDataSet.Tables["BreakRange"],
                           strFilter,
                           "",
                           DataViewRowState.CurrentRows);

                        this.Clear_DataGridView(this.dgvBreakRangeDataGridView);

                        for (int intRow = 0; intRow < pvtPayCategoryBreakRangeDataView.Count; intRow++)
                        {
                            this.dgvBreakRangeDataGridView.Rows.Add(Convert.ToInt32(Convert.ToInt32(pvtPayCategoryBreakRangeDataView[intRow]["WORKED_TIME_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtPayCategoryBreakRangeDataView[intRow]["WORKED_TIME_MINUTES"]) % 60).ToString("00"),
                                                                    Convert.ToInt32(Convert.ToInt32(pvtPayCategoryBreakRangeDataView[intRow]["BREAK_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtPayCategoryBreakRangeDataView[intRow]["BREAK_MINUTES"]) % 60).ToString("00"));
                        }

                        if (pvtPayCategoryBreakRangeDataView.Count > 0)
                        {
                            this.Set_DataGridView_SelectedRowIndex(dgvBreakRangeDataGridView, 0);
                        }

                        pvtPayCategoryWeekDataView = null;
                        pvtPayCategoryWeekDataView = new DataView(this.pvtDataSet.Tables["PayCategoryWeek"],
                            strFilter,
                            "WEEK_DATE",
                            DataViewRowState.CurrentRows);

                        pvtEmployeeDataView = null;
                        pvtEmployeeDataView = new DataView(this.pvtDataSet.Tables["Employee"],
                            strFilter,
                            "",
                            DataViewRowState.CurrentRows);

                        this.Clear_DataGridView(this.dgvEmployeeDataGridView);
                        this.Clear_DataGridView(this.dgvWeekDataGridView);
                        this.Clear_DataGridView(this.dgvDayDataGridView);
                        this.Clear_DataGridView(this.dgvTimeSheetDataGridView);
                        this.Clear_DataGridView(this.dgvBreakDataGridView);

                        this.pvtblnEmployeeDataGridViewLoaded = false;

                        int intHourColWidth = 55;
                        string strNames = "";

                        for (int intRow = 0; intRow < this.pvtEmployeeDataView.Count; intRow++)
                        {
                            if (intRow == 0)
                            {
                                int intPayCategoryWeekRow = 0;

                                this.dgvWeekDataGridView.Columns[pvtintWeekDateColWeekDataGridView].Width = 305;
                
                                this.dgvWeekTotalDataGridView.Columns[0].Width = 345;

                                this.dgvDayDataGridView.Columns[pvtintDayDescColDayDataGridView].Width = 140;

                                this.dgvDayTotalDataGridView.Columns[0].Width = 240;

                                //Errol Changed
                                this.dgvEmployeeDataGridView.Columns[pvtintNamesColEmployeeDataGridView].Width = 224;

                                //Fixed
                                if (Convert.ToDouble(pvtPayCategoryWeekDataView[intPayCategoryWeekRow]["OVERTIME1_RATE"]) > 0)
                                {
                                    this.dgvWeekDataGridView.Columns[pvtintOT1ColWeekDataGridView].Width = intHourColWidth;
                                    this.dgvWeekTotalDataGridView.Columns[2].Width = intHourColWidth;

                                    this.dgvDayDataGridView.Columns[pvtintOT1ColDayDataGridView].Width = intHourColWidth;
                                    this.dgvDayTotalDataGridView.Columns[4].Width = intHourColWidth;

                                    this.dgvDayDataGridView.Columns[pvtintOT1ColDayDataGridView].HeaderText = "OT " + Convert.ToDouble(pvtPayCategoryWeekDataView[intPayCategoryWeekRow]["OVERTIME1_RATE"]).ToString("0.00");
                                    this.dgvWeekDataGridView.Columns[pvtintOT1ColWeekDataGridView].HeaderText = "OT " + Convert.ToDouble(pvtPayCategoryWeekDataView[intPayCategoryWeekRow]["OVERTIME1_RATE"]).ToString("0.00");

                                    //Errol Changed
                                    this.dgvEmployeeDataGridView.Columns[pvtintOT1ColEmployeeDataGridView].Width = intHourColWidth;
                                    this.dgvEmployeeDataGridView.Columns[pvtintOT1ColEmployeeDataGridView].HeaderText = "OT " + Convert.ToDouble(pvtPayCategoryWeekDataView[intPayCategoryWeekRow]["OVERTIME1_RATE"]).ToString("0.00");
                                }
                                else
                                {

                                    this.dgvWeekDataGridView.Columns[pvtintWeekDateColWeekDataGridView].Width += intHourColWidth;
                                    this.dgvWeekDataGridView.Columns[pvtintOT1ColWeekDataGridView].Visible = false;
                                    this.dgvWeekTotalDataGridView.Columns[0].Width += intHourColWidth;
                                    this.dgvWeekTotalDataGridView.Columns[2].Visible = false;

                                    this.dgvDayDataGridView.Columns[pvtintDayDescColDayDataGridView].Width += intHourColWidth;
                                    this.dgvDayDataGridView.Columns[pvtintOT1ColDayDataGridView].Visible = false;

                                    this.dgvDayTotalDataGridView.Columns[0].Width += intHourColWidth;
                                    this.dgvDayTotalDataGridView.Columns[4].Visible = false;

                                    //Errol Changed
                                    this.dgvEmployeeDataGridView.Columns[pvtintNamesColEmployeeDataGridView].Width += intHourColWidth;
                                    this.dgvEmployeeDataGridView.Columns[pvtintOT1ColEmployeeDataGridView].Visible = false;
                                }

                                //Fixed
                                if (Convert.ToDouble(pvtPayCategoryWeekDataView[intPayCategoryWeekRow]["OVERTIME2_RATE"]) > 0)
                                {
                                    this.dgvWeekDataGridView.Columns[pvtintOT2ColWeekDataGridView].Width = intHourColWidth;
                                    this.dgvDayTotalDataGridView.Columns[4].Width = intHourColWidth;

                                    this.dgvDayDataGridView.Columns[pvtintOT2ColDayDataGridView].HeaderText = "OT " + Convert.ToDouble(pvtPayCategoryWeekDataView[intPayCategoryWeekRow]["OVERTIME2_RATE"]).ToString("0.00");
                                    this.dgvWeekDataGridView.Columns[pvtintOT2ColWeekDataGridView].HeaderText = "OT " + Convert.ToDouble(pvtPayCategoryWeekDataView[intPayCategoryWeekRow]["OVERTIME2_RATE"]).ToString("0.00");

                                    //Errol Changed
                                    this.dgvEmployeeDataGridView.Columns[pvtintOT2ColEmployeeDataGridView].Width = intHourColWidth;
                                    this.dgvEmployeeDataGridView.Columns[pvtintOT2ColEmployeeDataGridView].HeaderText = "OT " + Convert.ToDouble(pvtPayCategoryWeekDataView[intPayCategoryWeekRow]["OVERTIME2_RATE"]).ToString("0.00");
                                }
                                else
                                {
                                    this.dgvWeekDataGridView.Columns[pvtintWeekDateColWeekDataGridView].Width += intHourColWidth;
                                    this.dgvWeekDataGridView.Columns[pvtintOT2ColWeekDataGridView].Visible = false;

                                    this.dgvWeekTotalDataGridView.Columns[0].Width += intHourColWidth;
                                    this.dgvWeekTotalDataGridView.Columns[3].Visible = false;

                                    this.dgvDayDataGridView.Columns[pvtintDayDescColDayDataGridView].Width += intHourColWidth;
                                    this.dgvDayDataGridView.Columns[pvtintOT2ColDayDataGridView].Visible = false;

                                    this.dgvDayTotalDataGridView.Columns[0].Width += intHourColWidth;
                                    this.dgvDayTotalDataGridView.Columns[5].Visible = false;

                                    //Errol Changed
                                    this.dgvEmployeeDataGridView.Columns[pvtintNamesColEmployeeDataGridView].Width += intHourColWidth;
                                    this.dgvEmployeeDataGridView.Columns[pvtintOT2ColEmployeeDataGridView].Visible = false;
                                }

                                //Fixed
                                if (Convert.ToDouble(pvtPayCategoryWeekDataView[intPayCategoryWeekRow]["OVERTIME3_RATE"]) > 0)
                                {
                                    this.dgvWeekDataGridView.Columns[pvtintOT3ColWeekDataGridView].Width = intHourColWidth;
                                    this.dgvWeekTotalDataGridView.Columns[4].Width = intHourColWidth;

                                    this.dgvDayDataGridView.Columns[pvtintOT3ColDayDataGridView].Width = intHourColWidth;
                                    this.dgvDayTotalDataGridView.Columns[5].Width = intHourColWidth;

                                    this.dgvDayDataGridView.Columns[pvtintOT3ColDayDataGridView].HeaderText = "OT " + Convert.ToDouble(pvtPayCategoryWeekDataView[intPayCategoryWeekRow]["OVERTIME3_RATE"]).ToString("0.00");
                                    this.dgvWeekDataGridView.Columns[pvtintOT3ColWeekDataGridView].HeaderText = "OT " + Convert.ToDouble(pvtPayCategoryWeekDataView[intPayCategoryWeekRow]["OVERTIME3_RATE"]).ToString("0.00");

                                    //Errol Changed
                                    this.dgvEmployeeDataGridView.Columns[pvtintOT3ColEmployeeDataGridView].Width = intHourColWidth;
                                    this.dgvEmployeeDataGridView.Columns[pvtintOT3ColEmployeeDataGridView].HeaderText = "OT " + Convert.ToDouble(pvtPayCategoryWeekDataView[intPayCategoryWeekRow]["OVERTIME3_RATE"]).ToString("0.00");
                                }
                                else
                                {
                                    this.dgvWeekDataGridView.Columns[pvtintWeekDateColWeekDataGridView].Width += intHourColWidth;
                                    this.dgvWeekDataGridView.Columns[pvtintOT3ColWeekDataGridView].Visible = false;

                                    this.dgvWeekTotalDataGridView.Columns[0].Width += intHourColWidth;
                                    this.dgvWeekTotalDataGridView.Columns[4].Visible = false;

                                    this.dgvDayDataGridView.Columns[pvtintDayDescColDayDataGridView].Width += intHourColWidth;
                                    this.dgvDayDataGridView.Columns[pvtintOT3ColDayDataGridView].Visible = false;

                                    this.dgvDayTotalDataGridView.Columns[0].Width += intHourColWidth;
                                    this.dgvDayTotalDataGridView.Columns[6].Visible = false;

                                    //Errol Changed
                                    this.dgvEmployeeDataGridView.Columns[pvtintNamesColEmployeeDataGridView].Width += intHourColWidth;
                                    this.dgvEmployeeDataGridView.Columns[pvtintOT3ColEmployeeDataGridView].Visible = false;
                                }

                                //Fixed
                                this.dgvDayDataGridView.Columns[pvtintPHColDayDataGridView].HeaderText = "PH " + Convert.ToDouble(pvtPayCategoryWeekDataView[intPayCategoryWeekRow]["PAIDHOLIDAY_RATE"]).ToString("0.00");
                                this.dgvWeekDataGridView.Columns[pvtintPHColWeekDataGridView].HeaderText = "PH " + Convert.ToDouble(pvtPayCategoryWeekDataView[intPayCategoryWeekRow]["PAIDHOLIDAY_RATE"]).ToString("0.00");

                                //Errol Changed
                                this.dgvEmployeeDataGridView.Columns[pvtintPHColEmployeeDataGridView].HeaderText = "PH " + Convert.ToDouble(pvtPayCategoryWeekDataView[intPayCategoryWeekRow]["PAIDHOLIDAY_RATE"]).ToString("0.00");
                            }

                            pvtTempDataView = null;
                            pvtTempDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                                strFilter + " AND EMPLOYEE_NO = " + pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString() + pvtstrDataFilter,
                                "INDICATOR DESC",
                                DataViewRowState.CurrentRows);

                            if ((this.rbnException.Checked == true
                            || this.rbnNormal.Checked == true
                            || this.rbnBreakException.Checked == true
                            || this.rbnPublicHoliday.Checked == true
                            || this.rbnExcludedRun.Checked == true
                            || this.chkRemoveBlanks.Checked == true)
                            && pvtTempDataView.Count == 0)
                            {
                                continue;
                            }

                            if (this.rbnSurnameName.Checked == true)
                            {
                                strNames = pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString() + " / " + pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString();
                            }
                            else
                            {
                                strNames = pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString() + " / " + pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString();
                            }

                            this.dgvEmployeeDataGridView.Rows.Add("",
                                                                  "",
                                                                  "",
                                                                  "",
                                                                  pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                                  strNames,
                                                                  "",
                                                                  "",
                                                                  "",
                                                                  "",
                                                                  "",
                                                                  intRow.ToString());

                            if (pvtTempDataView.Count > 0)
                            {
                                if (this.rbnBlank.Checked == true)
                                {
                                    this.dgvEmployeeDataGridView[0,this.dgvEmployeeDataGridView.Rows.Count - 1].Style = this.BlankDataGridViewCellStyle;
                                }
                                else
                                {
                                    if (pvtTempDataView[0]["INDICATOR"].ToString() == "E")
                                    {
                                        this.dgvEmployeeDataGridView[0,this.dgvEmployeeDataGridView.Rows.Count - 1].Style = this.ExceptionDataGridViewCellStyle;
                                    }

                                    pvtTempDataView.Sort = "BREAK_INDICATOR DESC";

                                    if (pvtTempDataView[0]["BREAK_INDICATOR"].ToString() == "Y")
                                    {
                                        this.dgvEmployeeDataGridView[1, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = BreakExceptionDataGridViewCellStyle;
                                    }

                                    pvtTempDataView.Sort = "PAIDHOLIDAY_INDICATOR DESC";

                                    if (pvtTempDataView[0]["PAIDHOLIDAY_INDICATOR"].ToString() == "Y")
                                    {
                                        this.dgvEmployeeDataGridView[2, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = this.PaidHolidayDataGridViewCellStyle;
                                    }

                                    pvtTempDataView.Sort = "INCLUDED_IN_RUN_INDICATOR";

                                    if (pvtTempDataView[0]["INCLUDED_IN_RUN_INDICATOR"].ToString() == "N")
                                    {
                                        this.dgvEmployeeDataGridView[3, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = this.NotIncludedInRunDataGridViewCellStyle;
                                    }
                                }
                            }
                            else
                            {
                                this.dgvEmployeeDataGridView[0,this.dgvEmployeeDataGridView.Rows.Count - 1].Style = this.BlankDataGridViewCellStyle;
                            }

                            if (this.rbnNone.Checked == false)
                            {
                                continue;
                            }

                            //Normal Hours
                            pvtEmployeeEarningDataView = null;
                            pvtEmployeeEarningDataView = new DataView(this.pvtDataSet.Tables["EmployeeEarning"],
                                strFilter + " AND EMPLOYEE_NO = " + pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString(),
                                "EARNING_NO",
                                DataViewRowState.CurrentRows);

                            for (int intEmployeeRow = 0; intEmployeeRow < this.pvtEmployeeEarningDataView.Count; intEmployeeRow++)
                            {
                                if (pvtEmployeeEarningDataView[intEmployeeRow]["EARNING_NO"].ToString() == "2")
                                {
                                    //Errol Changed
                                    if (this.rbnBlank.Checked == true)
                                    {
                                        this.dgvEmployeeDataGridView[pvtintNTColEmployeeDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Value = "0.00";
                                    }
                                    else
                                    {
                                        this.dgvEmployeeDataGridView[pvtintNTColEmployeeDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Value = Convert.ToDouble(pvtEmployeeEarningDataView[intEmployeeRow]["HOURS_DECIMAL"]).ToString("###0.00");
                                    }
                                }
                                else
                                {
                                    if (pvtEmployeeEarningDataView[intEmployeeRow]["EARNING_NO"].ToString() == "3")
                                    {
                                        //Errol Changed
                                        if (this.rbnBlank.Checked == true)
                                        {
                                            this.dgvEmployeeDataGridView[pvtintOT1ColEmployeeDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Value = "0.00";
                                        }
                                        else
                                        {
                                            this.dgvEmployeeDataGridView[pvtintOT1ColEmployeeDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Value = Convert.ToDouble(pvtEmployeeEarningDataView[intEmployeeRow]["HOURS_DECIMAL"]).ToString("###0.00");
                                        }
                                    }
                                    else
                                    {
                                        if (pvtEmployeeEarningDataView[intEmployeeRow]["EARNING_NO"].ToString() == "4")
                                        {
                                            //Errol Changed
                                            if (this.rbnBlank.Checked == true)
                                            {
                                                this.dgvEmployeeDataGridView[pvtintOT2ColEmployeeDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Value = "0.00";
                                            }
                                            else
                                            {
                                                this.dgvEmployeeDataGridView[pvtintOT2ColEmployeeDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Value = Convert.ToDouble(pvtEmployeeEarningDataView[intEmployeeRow]["HOURS_DECIMAL"]).ToString("###0.00");
                                            }
                                        }
                                        else
                                        {
                                            if (pvtEmployeeEarningDataView[intEmployeeRow]["EARNING_NO"].ToString() == "5")
                                            {
                                                //Errol Changed
                                                if (this.rbnBlank.Checked == true)
                                                {
                                                    this.dgvEmployeeDataGridView[pvtintOT3ColEmployeeDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Value = "0.00";
                                                }
                                                else
                                                {
                                                    this.dgvEmployeeDataGridView[pvtintOT3ColEmployeeDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Value = Convert.ToDouble(pvtEmployeeEarningDataView[intEmployeeRow]["HOURS_DECIMAL"]).ToString("###0.00");
                                                }
                                            }
                                            else
                                            {
                                                //Errol Changed
                                                if (this.rbnBlank.Checked == true)
                                                {
                                                    this.dgvEmployeeDataGridView[pvtintPHColEmployeeDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Value = "0.00";
                                                }
                                                else
                                                {
                                                    this.dgvEmployeeDataGridView[pvtintPHColEmployeeDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1].Value = Convert.ToDouble(pvtEmployeeEarningDataView[intEmployeeRow]["HOURS_DECIMAL"]).ToString("###0.00");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        this.pvtblnEmployeeDataGridViewLoaded = true;

                        //Select First Row
                        if (pvtEmployeeDataView.Count > 0)
                        {
                            this.Set_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView, 0);
                        }
                    }
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void dgvEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (this.pvtblnEmployeeDataGridViewLoaded == true)
            {
                if (pvtintEmployeeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintEmployeeDataGridViewRowIndex = e.RowIndex;

                    int intWeekFindRow = 0;

                    //Errol Changed
                    pvtintEmployeeIndex = Convert.ToInt32(this.dgvEmployeeDataGridView[pvtintIndexColEmployeeDataGridView, e.RowIndex].Value);

                    //Get Employee Number
                    pvtintEmployeeNo = Convert.ToInt32(pvtEmployeeDataView[pvtintEmployeeIndex]["EMPLOYEE_NO"]);

                    string strFilter = "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND EMPLOYEE_NO = " + pvtintEmployeeNo;

                    if (this.rbnNone.Checked == true)
                    {
                        //Normal Hours
                        pvtEmployeeEarningDataView = null;
                        pvtEmployeeEarningDataView = new DataView(this.pvtDataSet.Tables["EmployeeEarning"],
                            strFilter + " AND EARNING_NO = 2",
                            "",
                            DataViewRowState.CurrentRows);

                        if (pvtEmployeeEarningDataView.Count > 0)
                        {
                            this.dgvWeekTotalDataGridView[1, 0].Value = Convert.ToInt32(Convert.ToInt32(pvtEmployeeEarningDataView[0]["MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtEmployeeEarningDataView[0]["MINUTES"]) % 60).ToString("00");
                            this.dgvWeekTotalDataGridView[1, 1].Value = Convert.ToInt32(Convert.ToInt32(pvtEmployeeEarningDataView[0]["MINUTES_ROUNDED"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtEmployeeEarningDataView[0]["MINUTES_ROUNDED"]) % 60).ToString("00");
                            this.dgvWeekTotalDataGridView[1, 2].Value = Convert.ToDouble(pvtEmployeeEarningDataView[0]["HOURS_DECIMAL"]).ToString("##0.00");
                        }

                        //Overtime1
                        pvtEmployeeEarningDataView = null;
                        pvtEmployeeEarningDataView = new DataView(this.pvtDataSet.Tables["EmployeeEarning"],
                            strFilter + " AND EARNING_NO = 3",
                            "",
                            DataViewRowState.CurrentRows);

                        if (pvtEmployeeEarningDataView.Count > 0)
                        {
                            this.dgvWeekTotalDataGridView[2, 0].Value = Convert.ToInt32(Convert.ToInt32(pvtEmployeeEarningDataView[0]["MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtEmployeeEarningDataView[0]["MINUTES"]) % 60).ToString("00");
                            this.dgvWeekTotalDataGridView[2, 1].Value = Convert.ToInt32(Convert.ToInt32(pvtEmployeeEarningDataView[0]["MINUTES_ROUNDED"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtEmployeeEarningDataView[0]["MINUTES_ROUNDED"]) % 60).ToString("00");
                            this.dgvWeekTotalDataGridView[2, 2].Value = Convert.ToDouble(pvtEmployeeEarningDataView[0]["HOURS_DECIMAL"]).ToString("##0.00");
                        }

                        //Overtime2
                        pvtEmployeeEarningDataView = null;
                        pvtEmployeeEarningDataView = new DataView(this.pvtDataSet.Tables["EmployeeEarning"],
                            strFilter + " AND EARNING_NO = 4",
                            "",
                            DataViewRowState.CurrentRows);

                        if (pvtEmployeeEarningDataView.Count > 0)
                        {
                            this.dgvWeekTotalDataGridView[3, 0].Value = Convert.ToInt32(Convert.ToInt32(pvtEmployeeEarningDataView[0]["MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtEmployeeEarningDataView[0]["MINUTES"]) % 60).ToString("00");
                            this.dgvWeekTotalDataGridView[3, 1].Value = Convert.ToInt32(Convert.ToInt32(pvtEmployeeEarningDataView[0]["MINUTES_ROUNDED"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtEmployeeEarningDataView[0]["MINUTES_ROUNDED"]) % 60).ToString("00");
                            this.dgvWeekTotalDataGridView[3, 2].Value = Convert.ToDouble(pvtEmployeeEarningDataView[0]["HOURS_DECIMAL"]).ToString("##0.00");
                        }
                        else
                        {
                            this.dgvWeekTotalDataGridView[3, 0].Value = "0.00";
                            this.dgvWeekTotalDataGridView[3, 1].Value = "0.00";
                            this.dgvWeekTotalDataGridView[3, 2].Value = "0.00";
                        }

                        //Overtime3
                        pvtEmployeeEarningDataView = null;
                        pvtEmployeeEarningDataView = new DataView(this.pvtDataSet.Tables["EmployeeEarning"],
                            strFilter + " AND EARNING_NO = 5",
                            "",
                            DataViewRowState.CurrentRows);

                        if (pvtEmployeeEarningDataView.Count > 0)
                        {
                            this.dgvWeekTotalDataGridView[4, 0].Value = Convert.ToInt32(Convert.ToInt32(pvtEmployeeEarningDataView[0]["MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtEmployeeEarningDataView[0]["MINUTES"]) % 60).ToString("00");
                            this.dgvWeekTotalDataGridView[4, 1].Value = Convert.ToInt32(Convert.ToInt32(pvtEmployeeEarningDataView[0]["MINUTES_ROUNDED"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtEmployeeEarningDataView[0]["MINUTES_ROUNDED"]) % 60).ToString("00");
                            this.dgvWeekTotalDataGridView[4, 2].Value = Convert.ToDouble(pvtEmployeeEarningDataView[0]["HOURS_DECIMAL"]).ToString("##0.00");
                        }
                        else
                        {
                            this.dgvWeekTotalDataGridView[4, 0].Value = "0.00";
                            this.dgvWeekTotalDataGridView[4, 1].Value = "0.00";
                            this.dgvWeekTotalDataGridView[4, 2].Value = "0.00";
                        }

                        //PaidHoliday Worked
                        pvtEmployeeEarningDataView = null;
                        pvtEmployeeEarningDataView = new DataView(this.pvtDataSet.Tables["EmployeeEarning"],
                            strFilter + " AND EARNING_NO = 9",
                            "",
                            DataViewRowState.CurrentRows);

                        if (pvtEmployeeEarningDataView.Count > 0)
                        {
                            this.dgvWeekTotalDataGridView[5, 0].Value = Convert.ToInt32(Convert.ToInt32(pvtEmployeeEarningDataView[0]["MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtEmployeeEarningDataView[0]["MINUTES"]) % 60).ToString("00");
                            this.dgvWeekTotalDataGridView[5, 1].Value = Convert.ToInt32(Convert.ToInt32(pvtEmployeeEarningDataView[0]["MINUTES_ROUNDED"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtEmployeeEarningDataView[0]["MINUTES_ROUNDED"]) % 60).ToString("00");
                            this.dgvWeekTotalDataGridView[5, 2].Value = Convert.ToDouble(pvtEmployeeEarningDataView[0]["HOURS_DECIMAL"]).ToString("###0.00");
                        }
                        else
                        {
                            this.dgvWeekTotalDataGridView[5, 0].Value = "0.00";
                            this.dgvWeekTotalDataGridView[5, 1].Value = "0.00";
                            this.dgvWeekTotalDataGridView[5, 2].Value = "0.00";
                        }
                    }
                    else
                    {
                        this.dgvWeekTotalDataGridView[1, 0].Value = "";
                        this.dgvWeekTotalDataGridView[2, 0].Value = "";
                        this.dgvWeekTotalDataGridView[3, 0].Value = "";
                        this.dgvWeekTotalDataGridView[4, 0].Value = "";
                        this.dgvWeekTotalDataGridView[5, 0].Value = "";

                        this.dgvWeekTotalDataGridView[1, 1].Value = "";
                        this.dgvWeekTotalDataGridView[2, 1].Value = "";
                        this.dgvWeekTotalDataGridView[3, 1].Value = "";
                        this.dgvWeekTotalDataGridView[4, 1].Value = "";
                        this.dgvWeekTotalDataGridView[5, 1].Value = "";

                        this.dgvWeekTotalDataGridView[1, 2].Value = "";
                        this.dgvWeekTotalDataGridView[2, 2].Value = "";
                        this.dgvWeekTotalDataGridView[3, 2].Value = "";
                        this.dgvWeekTotalDataGridView[4, 2].Value = "";
                        this.dgvWeekTotalDataGridView[5, 2].Value = "";
                    }

                    pvtEmployeeWeekDataView = null;
                    pvtEmployeeWeekDataView = new DataView(this.pvtDataSet.Tables["EmployeeWeek"],
                        strFilter,
                        "WEEK_DATE",
                        DataViewRowState.CurrentRows);

                    string strWeekFilter = "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo;

                    for (int intRow = 0; intRow < this.pvtEmployeeWeekDataView.Count; intRow++)
                    {
                        if (intRow == 0)
                        {
                            strWeekFilter += " AND WEEK_DATE IN ( ";
                        }
                        else
                        {
                            strWeekFilter += ",";
                        }

                        strWeekFilter += "'" + Convert.ToDateTime(pvtEmployeeWeekDataView[intRow]["WEEK_DATE"]).ToString("yyyy-MM-dd") + "'";

                        if (intRow == this.pvtEmployeeWeekDataView.Count - 1)
                        {
                            strWeekFilter += ")";
                        }
                    }

                    //NB  pvtEmployeeWeekDataView & pvtPayCategoryWeekDataView Will be Synchronised
                    pvtPayCategoryWeekDataView = null;
                    pvtPayCategoryWeekDataView = new DataView(this.pvtDataSet.Tables["PayCategoryWeek"],
                        strWeekFilter,
                        "WEEK_DATE",
                        DataViewRowState.CurrentRows);

                    this.Clear_DataGridView(this.dgvWeekDataGridView);
                    this.Clear_DataGridView(this.dgvDayDataGridView);

                    this.pvtblnWeekDataGridViewLoaded = false;

                    string strDayFilter = "";
                    int intDaysToSubtract = 0;

                    for (int intRow = 0; intRow < this.pvtEmployeeWeekDataView.Count; intRow++)
                    {
                        strDayFilter = "";
                        intDaysToSubtract = 0;

                        //ELR - 2015-05-08
                        if (this.chkRemoveSun.Checked == true
                            && this.chkRemoveSat.Checked == true)
                        {
                            strDayFilter = " AND DAY_NO NOT IN (0,6) ";
                            intDaysToSubtract = 2;
                        }
                        else
                        {
                            if (this.chkRemoveSun.Checked == true)
                            {
                                strDayFilter = " AND DAY_NO NOT IN (0) ";
                                intDaysToSubtract = 1;
                            }
                            else
                            {
                                if (this.chkRemoveSat.Checked == true)
                                {
                                    strDayFilter = " AND DAY_NO NOT IN (6) ";
                                    intDaysToSubtract = 1;
                                }
                            }
                        }

                        pvtTempDataView = null;
                        pvtTempDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                            strFilter + pvtstrDataFilter + strDayFilter + " AND DAY_DATE >= '" + Convert.ToDateTime(pvtEmployeeWeekDataView[intRow]["WEEK_DATE_FROM"]).ToString("dd MMM yyyy") + "' AND DAY_DATE <= '" + Convert.ToDateTime(pvtEmployeeWeekDataView[intRow]["WEEK_DATE"]).ToString("dd MMM yyyy") + "'",
                            "INDICATOR DESC",
                            DataViewRowState.CurrentRows);

                        if ((this.rbnException.Checked == true
                        || this.rbnNormal.Checked == true
                        || this.rbnBreakException.Checked == true
                        || this.rbnPublicHoliday.Checked == true
                        || this.rbnExcludedRun.Checked == true
                        || this.chkRemoveBlanks.Checked == true)
                        && pvtTempDataView.Count == 0)
                        {
                            continue;
                        }

                        if (this.rbnBlank.Checked == true)
                        {
                            string strFromDate = Convert.ToDateTime(pvtEmployeeWeekDataView[intRow]["WEEK_DATE_FROM"]).ToString("yyyy-MM-dd");
                            string strToDate = Convert.ToDateTime(pvtEmployeeWeekDataView[intRow]["WEEK_DATE"]).ToString("yyyy-MM-dd");

                            //ERROL CHANGED 2012-06-26
                            TimeSpan ts = Convert.ToDateTime(pvtEmployeeWeekDataView[intRow]["WEEK_DATE"]).Subtract(Convert.ToDateTime(pvtEmployeeWeekDataView[intRow]["WEEK_DATE_FROM"]));

                            if (ts.Days <= pvtTempDataView.Count - 1 + intDaysToSubtract)
                            {
                                continue;
                            }
                        }

                        if (this.rbnBlank.Checked == true
                            | this.rbnNone.Checked == false)
                        {
                            //Errol Changed
                            this.dgvWeekDataGridView.Rows.Add("",
                                                              "",
                                                              "",
                                                              "",
                                                              Convert.ToDateTime(pvtEmployeeWeekDataView[intRow]["WEEK_DATE_FROM"]).ToString("dd MMM yyyy") + " - " + Convert.ToDateTime(pvtEmployeeWeekDataView[intRow]["WEEK_DATE"]).ToString("dd MMM yyyy"),
                                                              "",
                                                              "",
                                                              "",
                                                              "",
                                                              "",
                                                              intRow.ToString());
                        }
                        else
                        {
                            //Errol Changed
                            this.dgvWeekDataGridView.Rows.Add("",
                                                              "",
                                                              "",
                                                              "",
                                                              Convert.ToDateTime(pvtEmployeeWeekDataView[intRow]["WEEK_DATE_FROM"]).ToString("dd MMM yyyy") + " - " + Convert.ToDateTime(pvtEmployeeWeekDataView[intRow]["WEEK_DATE"]).ToString("dd MMM yyyy"),
                                                              Convert.ToInt32(Convert.ToInt32(pvtEmployeeWeekDataView[intRow]["NORMALTIME_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtEmployeeWeekDataView[intRow]["NORMALTIME_MINUTES"]) % 60).ToString("00"),
                                                              Convert.ToInt32(Convert.ToInt32(pvtEmployeeWeekDataView[intRow]["OVERTIME1_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtEmployeeWeekDataView[intRow]["OVERTIME1_MINUTES"]) % 60).ToString("00"),
                                                              Convert.ToInt32(Convert.ToInt32(pvtEmployeeWeekDataView[intRow]["OVERTIME2_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtEmployeeWeekDataView[intRow]["OVERTIME2_MINUTES"]) % 60).ToString("00"),
                                                              Convert.ToInt32(Convert.ToInt32(pvtEmployeeWeekDataView[intRow]["OVERTIME3_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtEmployeeWeekDataView[intRow]["OVERTIME3_MINUTES"]) % 60).ToString("00"),
                                                              Convert.ToInt32(Convert.ToInt32(pvtEmployeeWeekDataView[intRow]["PAIDHOLIDAY_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtEmployeeWeekDataView[intRow]["PAIDHOLIDAY_MINUTES"]) % 60).ToString("00"),
                                                              intRow.ToString());
                        }

                        if (pvtTempDataView.Count > 0)
                        {
                            if (this.rbnBlank.Checked == true)
                            {
                                this.dgvWeekDataGridView[0,this.dgvWeekDataGridView.Rows.Count - 1].Style = this.BlankDataGridViewCellStyle;
                            }
                            else
                            {
                                if (pvtTempDataView[0]["INDICATOR"].ToString() == "E")
                                {
                                    this.dgvWeekDataGridView[0,this.dgvWeekDataGridView.Rows.Count - 1].Style = this.ExceptionDataGridViewCellStyle;
                                }

                                pvtTempDataView.Sort = "BREAK_INDICATOR DESC";

                                if (pvtTempDataView[0]["BREAK_INDICATOR"].ToString() == "Y")
                                {
                                    dgvWeekDataGridView[1, this.dgvWeekDataGridView.Rows.Count - 1].Style = BreakExceptionDataGridViewCellStyle;
                                }

                                pvtTempDataView.Sort = "PAIDHOLIDAY_INDICATOR DESC";

                                if (pvtTempDataView[0]["PAIDHOLIDAY_INDICATOR"].ToString() == "Y")
                                {
                                    dgvWeekDataGridView[2, this.dgvWeekDataGridView.Rows.Count - 1].Style = this.PaidHolidayDataGridViewCellStyle;
                                }

                                pvtTempDataView.Sort = "INCLUDED_IN_RUN_INDICATOR";

                                if (pvtTempDataView[0]["INCLUDED_IN_RUN_INDICATOR"].ToString() == "N")
                                {
                                    this.dgvWeekDataGridView[3, this.dgvWeekDataGridView.Rows.Count - 1].Style = this.NotIncludedInRunDataGridViewCellStyle;
                                }
                            }
                        }
                        else
                        {
                            this.dgvWeekDataGridView[0,this.dgvWeekDataGridView.Rows.Count - 1].Style = this.BlankDataGridViewCellStyle;
                        }

                        if (pvtDateFind >= Convert.ToDateTime(pvtPayCategoryWeekDataView[intRow]["WEEK_DATE_FROM"])
                        & pvtDateFind <= Convert.ToDateTime(pvtPayCategoryWeekDataView[intRow]["WEEK_DATE"]))
                        {
                            intWeekFindRow = dgvWeekDataGridView.Rows.Count - 1;
                        }
                    }

                    this.pvtblnWeekDataGridViewLoaded = true;

                    if (this.dgvWeekDataGridView.Rows.Count > 0)
                    {
                        if (intWeekFindRow != 0)
                        {
                            this.Set_DataGridView_SelectedRowIndex(dgvWeekDataGridView, intWeekFindRow);
                        }
                        else
                        {
                            this.Set_DataGridView_SelectedRowIndex(dgvWeekDataGridView, 0);
                        }
                    }
                }
            }
        }

        private void dgvWeekDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnWeekDataGridViewLoaded == true)
            {
                if (pvtintWeekDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintWeekDataGridViewRowIndex = e.RowIndex;

                    bool blnWeekDays = true;
                    string strExceptionDesc = "";

                    //int intTest = Convert.ToInt32(this.dgvWeekDataGridView[8, this.Get_DataGridView_SelectedRowIndex(dgvWeekDataGridView)].Value);

                    pvtintFindPayCategoryWeekRow = pvtPayCategoryWeekDataView.Find(Convert.ToDateTime(pvtEmployeeWeekDataView[Convert.ToInt32(this.dgvWeekDataGridView[pvtintIndexColWeekDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvWeekDataGridView)].Value)]["WEEK_DATE"]));

                    if (pvtintFindPayCategoryWeekRow == -1)
                    {
                        string a = "1";
                    }

                    try
                    {

                        if (this.rbnNone.Checked == true)
                        {
                            this.dgvDayTotalDataGridView[2, 0].Value = Convert.ToInt32(Convert.ToInt32(pvtEmployeeWeekDataView[pvtintFindPayCategoryWeekRow]["TOTAL_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtEmployeeWeekDataView[pvtintFindPayCategoryWeekRow]["TOTAL_MINUTES"]) % 60).ToString("00");
                            this.dgvDayTotalDataGridView[3, 0].Value = Convert.ToInt32(Convert.ToInt32(pvtEmployeeWeekDataView[pvtintFindPayCategoryWeekRow]["NORMALTIME_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtEmployeeWeekDataView[pvtintFindPayCategoryWeekRow]["NORMALTIME_MINUTES"]) % 60).ToString("00");
                            this.dgvDayTotalDataGridView[4, 0].Value = Convert.ToInt32(Convert.ToInt32(pvtEmployeeWeekDataView[pvtintFindPayCategoryWeekRow]["OVERTIME1_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtEmployeeWeekDataView[pvtintFindPayCategoryWeekRow]["OVERTIME1_MINUTES"]) % 60).ToString("00");
                            this.dgvDayTotalDataGridView[5, 0].Value = Convert.ToInt32(Convert.ToInt32(pvtEmployeeWeekDataView[pvtintFindPayCategoryWeekRow]["OVERTIME2_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtEmployeeWeekDataView[pvtintFindPayCategoryWeekRow]["OVERTIME2_MINUTES"]) % 60).ToString("00");
                            this.dgvDayTotalDataGridView[6, 0].Value = Convert.ToInt32(Convert.ToInt32(pvtEmployeeWeekDataView[pvtintFindPayCategoryWeekRow]["OVERTIME3_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtEmployeeWeekDataView[pvtintFindPayCategoryWeekRow]["OVERTIME3_MINUTES"]) % 60).ToString("00");
                            this.dgvDayTotalDataGridView[7, 0].Value = Convert.ToInt32(Convert.ToInt32(pvtEmployeeWeekDataView[pvtintFindPayCategoryWeekRow]["PAIDHOLIDAY_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtEmployeeWeekDataView[pvtintFindPayCategoryWeekRow]["PAIDHOLIDAY_MINUTES"]) % 60).ToString("00");
                        }
                        else
                        {
                            this.dgvDayTotalDataGridView[1, 0].Value = "";
                            this.dgvDayTotalDataGridView[2, 0].Value = "";
                            this.dgvDayTotalDataGridView[3, 0].Value = "";
                            this.dgvDayTotalDataGridView[4, 0].Value = "";
                            this.dgvDayTotalDataGridView[5, 0].Value = "";
                            this.dgvDayTotalDataGridView[6, 0].Value = "";
                            this.dgvDayTotalDataGridView[7, 0].Value = "";
                        }
                    }
                    catch (Exception ex)
                    {
                        int a = Convert.ToInt32(pvtEmployeeWeekDataView[pvtintFindPayCategoryWeekRow]["TOTAL_MINUTES"]);
                        a = Convert.ToInt32(pvtEmployeeWeekDataView[pvtintFindPayCategoryWeekRow]["NORMALTIME_MINUTES"]);
                        a = Convert.ToInt32(pvtEmployeeWeekDataView[pvtintFindPayCategoryWeekRow]["OVERTIME1_MINUTES"]);
                        a = Convert.ToInt32(pvtEmployeeWeekDataView[pvtintFindPayCategoryWeekRow]["OVERTIME2_MINUTES"]);
                        a = Convert.ToInt32(pvtEmployeeWeekDataView[pvtintFindPayCategoryWeekRow]["OVERTIME3_MINUTES"]);
                    }

                    this.lblDate.Text = this.dgvWeekDataGridView[pvtintWeekDateColWeekDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvWeekDataGridView)].Value.ToString();

                    string strDirection = "";
                    string strValue = "";

                    if (pvtPayCategoryWeekDataView[pvtintFindPayCategoryWeekRow]["DAILY_ROUNDING_IND"].ToString() == "0")
                    {
                        this.dgvTimeSheetTotalsDataGridView[0, 3].Value = "No Rounding";
                    }
                    else
                    {
                        if (pvtPayCategoryWeekDataView[pvtintFindPayCategoryWeekRow]["DAILY_ROUNDING_IND"].ToString() == "1")
                        {
                            strDirection = "Up";
                        }
                        else
                        {
                            if (pvtPayCategoryWeekDataView[pvtintFindPayCategoryWeekRow]["DAILY_ROUNDING_IND"].ToString() == "2")
                            {
                                strDirection = "Down";
                            }
                            else
                            {
                                if (pvtPayCategoryWeekDataView[pvtintFindPayCategoryWeekRow]["DAILY_ROUNDING_IND"].ToString() == "3")
                                {
                                    strDirection = "Closest";
                                }
                            }
                        }

                        strValue = Convert.ToDouble(pvtPayCategoryWeekDataView[pvtintFindPayCategoryWeekRow]["DAILY_ROUNDING_MINUTES"]).ToString("00");

                        this.dgvTimeSheetTotalsDataGridView[0, 3].Value = strDirection + " " + strValue + " Minutes";
                    }

                    if (pvtPayCategoryWeekDataView[pvtintFindPayCategoryWeekRow]["PAY_PERIOD_ROUNDING_IND"].ToString() == "0")
                    {
                        this.dgvWeekTotalDataGridView[0, 1].Value = "Pay Period Totals (No Rounding)";
                    }
                    else
                    {
                        if (pvtPayCategoryWeekDataView[pvtintFindPayCategoryWeekRow]["PAY_PERIOD_ROUNDING_IND"].ToString() == "1")
                        {
                            strDirection = "Up";
                        }
                        else
                        {
                            if (pvtPayCategoryWeekDataView[pvtintFindPayCategoryWeekRow]["PAY_PERIOD_ROUNDING_IND"].ToString() == "2")
                            {
                                strDirection = "Down";
                            }
                            else
                            {
                                if (pvtPayCategoryWeekDataView[pvtintFindPayCategoryWeekRow]["PAY_PERIOD_ROUNDING_IND"].ToString() == "3")
                                {
                                    strDirection = "Closest";
                                }
                            }
                        }

                        strValue = Convert.ToDouble(pvtPayCategoryWeekDataView[pvtintFindPayCategoryWeekRow]["PAY_PERIOD_ROUNDING_MINUTES"]).ToString("00");

                        this.dgvWeekTotalDataGridView[0, 1].Value = "Pay Period Totals (" + strDirection + " " + strValue + " Minutes)";
                    }

                    string strWhere = "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND DAY_DATE >= '" + Convert.ToDateTime(pvtEmployeeWeekDataView[this.pvtintFindPayCategoryWeekRow]["WEEK_DATE_FROM"]).ToString("yyyy-MM-dd") + "' AND DAY_DATE <= '" + Convert.ToDateTime(pvtEmployeeWeekDataView[this.pvtintFindPayCategoryWeekRow]["WEEK_DATE"]).ToString("yyyy-MM-dd") + "'";

                    if (this.chkRemoveSat.Checked == true
                        & this.chkRemoveSun.Checked == true)
                    {
                        strWhere += " AND NOT DAY_NO IN (0,6)";
                    }
                    else
                    {
                        if (this.chkRemoveSat.Checked == true)
                        {
                            strWhere += " AND DAY_NO <> 6";
                        }
                        else
                        {
                            if (this.chkRemoveSun.Checked == true)
                            {
                                strWhere += " AND DAY_NO <> 0";
                            }
                        }
                    }

                    int intDayOverTimeTotal = 0;
                    string strDayDesc = "";
                    string strFilter = "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND EMPLOYEE_NO = " + this.pvtintEmployeeNo;

                    this.Clear_DataGridView(this.dgvDayDataGridView);

                    this.pvtblnDayDataGridViewLoaded = false;

                    pvtDayDataView = null;
                    pvtDayDataView = new DataView(this.pvtDataSet.Tables["Dates"],
                        strWhere,
                        "DAY_DATE",
                        DataViewRowState.CurrentRows);

                flxgWeek_AfterRowColChange_Day_Continue:

                    for (int intRow = 0; intRow < pvtDayDataView.Count; intRow++)
                    {
                        //Move Saturday / Sunday to End (For Overtime
                        if (blnWeekDays == true)
                        {
                            if (Convert.ToInt32(pvtDayDataView[intRow]["DAY_NO"]) == 0
                                | Convert.ToInt32(pvtDayDataView[intRow]["DAY_NO"]) == 6)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (Convert.ToInt32(pvtDayDataView[intRow]["DAY_NO"]) != 0
                                & Convert.ToInt32(pvtDayDataView[intRow]["DAY_NO"]) != 6)
                            {
                                continue;
                            }
                        }

                        pvtTempDataView = null;
                        pvtTempDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                            strFilter + " AND DAY_DATE = '" + Convert.ToDateTime(pvtDayDataView[intRow]["DAY_DATE"]).ToString("yyyy-MM-dd") + "'" + pvtstrDataFilter,
                            "INDICATOR DESC",
                            DataViewRowState.CurrentRows);

                        if ((this.rbnException.Checked == true
                        || this.rbnNormal.Checked == true
                        || this.rbnBreakException.Checked == true
                        || this.rbnPublicHoliday.Checked == true
                        || this.rbnExcludedRun.Checked == true
                        || this.chkRemoveBlanks.Checked == true)
                        & pvtTempDataView.Count == 0)
                        {
                            continue;
                        }
                        else
                        {
                            if (this.rbnBlank.Checked == true
                                & pvtTempDataView.Count != 0)
                            {
                                continue;
                            }
                        }

                        strDayDesc = Convert.ToDateTime(pvtDayDataView[intRow]["DAY_DATE"]).DayOfWeek.ToString().ToUpper().Substring(0, 3);

                        if (Convert.ToDouble(pvtPayCategoryWeekDataView[this.pvtintFindPayCategoryWeekRow]["EXCEPTION_" + strDayDesc + "_BELOW_MINUTES"]) == Convert.ToDouble(pvtPayCategoryWeekDataView[this.pvtintFindPayCategoryWeekRow]["EXCEPTION_" + strDayDesc + "_ABOVE_MINUTES"]))
                        {
                            if (Convert.ToDouble(pvtPayCategoryWeekDataView[this.pvtintFindPayCategoryWeekRow]["EXCEPTION_SHIFT_ABOVE_PERCENT"]) == 0
                                & Convert.ToDouble(pvtPayCategoryWeekDataView[this.pvtintFindPayCategoryWeekRow]["EXCEPTION_SHIFT_BELOW_PERCENT"]) == 0)
                            {
                                strExceptionDesc = "N/A";
                            }
                            else
                            {
                                strExceptionDesc = "> 0";
                            }
                        }
                        else
                        {
                            strExceptionDesc = Convert.ToInt32(Convert.ToInt32(pvtPayCategoryWeekDataView[this.pvtintFindPayCategoryWeekRow]["EXCEPTION_" + strDayDesc + "_BELOW_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtPayCategoryWeekDataView[this.pvtintFindPayCategoryWeekRow]["EXCEPTION_" + strDayDesc + "_BELOW_MINUTES"]) % 60).ToString("00") + " - " + Convert.ToInt32(Convert.ToInt32(pvtPayCategoryWeekDataView[this.pvtintFindPayCategoryWeekRow]["EXCEPTION_" + strDayDesc + "_ABOVE_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtPayCategoryWeekDataView[this.pvtintFindPayCategoryWeekRow]["EXCEPTION_" + strDayDesc + "_ABOVE_MINUTES"]) % 60).ToString("00");
                        }

                        //Errol Changed
                        this.dgvDayDataGridView.Rows.Add("",
                                                         "",
                                                         "",
                                                         "",
                                                         Convert.ToDateTime(pvtDayDataView[intRow]["DAY_DATE"]).ToString("dd MMM yyyy - dddd"),
                                                         strExceptionDesc,
                                                         Convert.ToInt32(Convert.ToInt32(pvtPayCategoryWeekDataView[this.pvtintFindPayCategoryWeekRow][strDayDesc + "_TIME_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtPayCategoryWeekDataView[this.pvtintFindPayCategoryWeekRow][strDayDesc + "_TIME_MINUTES"]) % 60).ToString("00"),
                                                         "",
                                                         "",
                                                         "",
                                                         "",
                                                         "",
                                                         "",
                                                         Convert.ToDateTime(pvtDayDataView[intRow]["DAY_DATE"]).ToString("yyyy-MM-dd"));

                        if (Convert.ToInt32(Convert.ToDateTime(pvtDayDataView[intRow]["DAY_DATE"]).DayOfWeek) == 0
                        | Convert.ToInt32(Convert.ToDateTime(pvtDayDataView[intRow]["DAY_DATE"]).DayOfWeek) == 6)
                        {
                            this.dgvDayDataGridView.Rows[dgvDayDataGridView.Rows.Count - 1].DefaultCellStyle = WeekEndDataGridViewCellStyle;

                            //Repaint Columns Painted to WeekEndStyle
                            this.dgvDayDataGridView[0, dgvDayDataGridView.Rows.Count - 1].Style = NormalDataGridViewCellStyle;
                            this.dgvDayDataGridView[1, dgvDayDataGridView.Rows.Count - 1].Style = NormalDataGridViewCellStyle;
                            this.dgvDayDataGridView[2, dgvDayDataGridView.Rows.Count - 1].Style = NormalDataGridViewCellStyle;
                            this.dgvDayDataGridView[3, dgvDayDataGridView.Rows.Count - 1].Style = NormalDataGridViewCellStyle;
                        }

                        if (pvtPayCategoryWeekDataView[this.pvtintFindPayCategoryWeekRow]["OVERTIME_IND"].ToString() == "A")
                        {
                            intDayOverTimeTotal += Convert.ToInt32(pvtPayCategoryWeekDataView[this.pvtintFindPayCategoryWeekRow][strDayDesc + "_TIME_MINUTES"]);
                        }
                    }

                    //This is Used to Move Saturdays / Sundays to End
                    if (blnWeekDays == true)
                    {
                        blnWeekDays = false;
                        goto flxgWeek_AfterRowColChange_Day_Continue;
                    }

                    if (this.rbnNone.Checked == true)
                    {
                        this.dgvDayTotalDataGridView[1, 0].Value = Convert.ToInt32(intDayOverTimeTotal / 60).ToString() + ":" + Convert.ToInt32(intDayOverTimeTotal % 60).ToString("00");
                    }

                    pvtintOverTime1HoursBoundary = 0;
                    pvtintOverTime2HoursBoundary = 0;
                    pvtintOverTime3HoursBoundary = 0;
                    pvtintDayNormalHours = 0;
                    pvtintDayOverTime1Hours = 0;
                    pvtintDayOverTime2Hours = 0;
                    pvtintDayOverTime3Hours = 0;
                    pvtintWeekNormalMinutes = 0;
                    pvtintWeekOverTime1Minutes = 0;
                    pvtintWeekOverTime2Minutes = 0;
                    pvtintWeekOverTime3Minutes = 0;

                    DateTime dtDate = DateTime.Now;

                    for (int intRow = 0; intRow < this.dgvDayDataGridView.Rows.Count; intRow++)
                    {
                        dtDate = DateTime.ParseExact(this.dgvDayDataGridView[pvtintDateColDayDataGridView, intRow].Value.ToString(), "yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);

                        pvtEmployeeDayDataView = null;
                        pvtEmployeeDayDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                        "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND DAY_DATE = '" + this.dgvDayDataGridView[pvtintDateColDayDataGridView, intRow].Value.ToString() + "'",
                        "",
                        DataViewRowState.CurrentRows);

                        if (pvtEmployeeDayDataView.Count == 0)
                        {
                            this.dgvDayDataGridView[0,intRow].Style = this.BlankDataGridViewCellStyle;

                            this.dgvDayDataGridView[pvtintPaidHoursColDayDataGridView, intRow].Value = "0:00";
                            this.dgvDayDataGridView[pvtintNTColDayDataGridView, intRow].Value = "0:00";
                            this.dgvDayDataGridView[pvtintOT1ColDayDataGridView, intRow].Value = "0:00";
                            this.dgvDayDataGridView[pvtintOT2ColDayDataGridView, intRow].Value = "0:00";
                            this.dgvDayDataGridView[pvtintOT3ColDayDataGridView, intRow].Value = "0:00";
                            this.dgvDayDataGridView[pvtintPHColDayDataGridView, intRow].Value = "0:00";
                        }
                        else
                        {
                            if (this.rbnBlank.Checked == true)
                            {
                                this.dgvDayDataGridView[0,intRow].Style = this.BlankDataGridViewCellStyle;
                            }
                            else
                            {
                                if (pvtEmployeeDayDataView[0]["INDICATOR"].ToString() == "E")
                                {
                                    this.dgvDayDataGridView[0,intRow].Style = this.ExceptionDataGridViewCellStyle;
                                }
                            }

                            if (pvtEmployeeDayDataView[0]["BREAK_INDICATOR"].ToString() == "Y")
                            {
                                dgvDayDataGridView[1, intRow].Style = BreakExceptionDataGridViewCellStyle;
                            }

                            if (pvtEmployeeDayDataView[0]["PAIDHOLIDAY_INDICATOR"].ToString() == "Y")
                            {
                                dgvDayDataGridView[2, intRow].Style = this.PaidHolidayDataGridViewCellStyle;
                         
                                this.dgvDayDataGridView[pvtintPaidHoursColDayDataGridView, intRow].Value = Convert.ToInt32(Convert.ToInt32(pvtEmployeeDayDataView[0]["DAY_PAID_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtEmployeeDayDataView[0]["DAY_PAID_MINUTES"]) % 60).ToString("00");
                                this.dgvDayDataGridView[pvtintNTColDayDataGridView, intRow].Value = "0:00";
                                this.dgvDayDataGridView[pvtintOT1ColDayDataGridView, intRow].Value = "0:00";
                                this.dgvDayDataGridView[pvtintOT2ColDayDataGridView, intRow].Value = "0:00";
                                this.dgvDayDataGridView[pvtintOT3ColDayDataGridView, intRow].Value = "0:00";
                                this.dgvDayDataGridView[pvtintPHColDayDataGridView, intRow].Value = Convert.ToInt32(Convert.ToInt32(pvtEmployeeDayDataView[0]["DAY_PAID_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtEmployeeDayDataView[0]["DAY_PAID_MINUTES"]) % 60).ToString("00");
                            }
                            else
                            {
                                clsISUtilities.Calculate_Wage_Time_Breakdown(pvtPayCategoryWeekDataView, pvtintFindPayCategoryWeekRow,
                                Convert.ToInt32(dtDate.DayOfWeek), Convert.ToInt32(pvtEmployeeDayDataView[0]["DAY_PAID_MINUTES"]),
                                intDayOverTimeTotal,
                                ref pvtintOverTime1HoursBoundary, ref pvtintOverTime2HoursBoundary,
                                ref pvtintOverTime3HoursBoundary,
                                ref pvtintDayNormalHours,
                                ref pvtintDayOverTime1Hours, ref pvtintDayOverTime2Hours,
                                ref pvtintDayOverTime3Hours,
                                ref pvtintWeekNormalMinutes, ref pvtintWeekOverTime1Minutes,
                                ref pvtintWeekOverTime2Minutes, ref pvtintWeekOverTime3Minutes);

                                this.dgvDayDataGridView[pvtintPaidHoursColDayDataGridView, intRow].Value = Convert.ToInt32(Convert.ToInt32(pvtEmployeeDayDataView[0]["DAY_PAID_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtEmployeeDayDataView[0]["DAY_PAID_MINUTES"]) % 60).ToString("00");
                                this.dgvDayDataGridView[pvtintNTColDayDataGridView, intRow].Value = Convert.ToInt32(pvtintDayNormalHours / 60).ToString() + ":" + Convert.ToInt32(pvtintDayNormalHours % 60).ToString("00");
                                this.dgvDayDataGridView[pvtintOT1ColDayDataGridView, intRow].Value = Convert.ToInt32(pvtintDayOverTime1Hours / 60).ToString() + ":" + Convert.ToInt32(pvtintDayOverTime1Hours % 60).ToString("00");
                                this.dgvDayDataGridView[pvtintOT2ColDayDataGridView, intRow].Value = Convert.ToInt32(pvtintDayOverTime2Hours / 60).ToString() + ":" + Convert.ToInt32(pvtintDayOverTime2Hours % 60).ToString("00");
                                this.dgvDayDataGridView[pvtintOT3ColDayDataGridView, intRow].Value = Convert.ToInt32(pvtintDayOverTime3Hours / 60).ToString() + ":" + Convert.ToInt32(pvtintDayOverTime3Hours % 60).ToString("00");
                                this.dgvDayDataGridView[pvtintPHColDayDataGridView, intRow].Value = "0:00";
                            }
                          
                            if (pvtEmployeeDayDataView[0]["INCLUDED_IN_RUN_INDICATOR"].ToString() == "N")
                            {
                                this.dgvDayDataGridView[3, intRow].Style = this.NotIncludedInRunDataGridViewCellStyle;
                            }
                        }
                    }

                    this.pvtblnDayDataGridViewLoaded = true;

                    int intDayRow = 0;

                    for (int intRow = 0; intRow < this.dgvDayDataGridView.Rows.Count; intRow++)
                    {
                        if (this.pvtDateFind.ToString("dd MMM yyyy - dddd") == this.dgvDayDataGridView[pvtintDayDescColDayDataGridView, intRow].Value.ToString())
                        {
                            intDayRow = intRow;
                            break;
                        }
                    }

                    if (this.dgvDayDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(dgvDayDataGridView, intDayRow);
                    }
                }
            }
        }

        private void dgvDayDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnDayDataGridViewLoaded == true)
            {
                if (pvtintDayDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintDayDataGridViewRowIndex = e.RowIndex;

                    pvtDateFind = DateTime.ParseExact(this.dgvDayDataGridView[pvtintDateColDayDataGridView, e.RowIndex].Value.ToString(), "yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);

                    //Errol Changed
                    this.lblDayDesc.Text = this.dgvEmployeeDataGridView[pvtintNamesColEmployeeDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Value.ToString() + "   " + this.dgvDayDataGridView[pvtintDayDescColDayDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Value.ToString();

                    Load_TimeSheets();
                }
            }
        }

        private void dgvTimeSheetDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void dgvBreakDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void dgvBreakRangeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void Names_Order_Click(object sender, EventArgs e)
        {
            RadioButton myRadioButton = (RadioButton)sender;

            //Errol Changed
            if (myRadioButton.Name == "rbnSurnameName")
            {
                this.dgvEmployeeDataGridView.Columns[pvtintNamesColEmployeeDataGridView].HeaderText = "Surname / Name";
            }
            else
            {
                this.dgvEmployeeDataGridView.Columns[pvtintNamesColEmployeeDataGridView].HeaderText = "Name / Surname";
            }

            if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView,this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView));
            }
        }
        
        private void btnRemoveFilter_Click(object sender, EventArgs e)
        {
            this.chkRemoveSat.Checked = false;
            this.chkRemoveSun.Checked = false;
            this.chkRemoveBlanks.Checked = false;

            this.rbnNone.Checked = true;

            rbnNone_Click(sender, e);
        }

        private void rbnExcludedRun_Click(object sender, EventArgs e)
        {
            Clear_SpreadSheet_Data();

            this.chkRemoveBlanks.Checked = false;
            this.chkRemoveBlanks.Enabled = false;
            this.chkRemoveSat.Checked = false;
            this.chkRemoveSun.Checked = false;
            this.chkRemoveSat.Enabled = false;
            this.chkRemoveSun.Enabled = false;

            Load_CurrentForm_Records();
        }
    }
}
