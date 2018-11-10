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
    public partial class frmRptTimeSheetSelection : Form
    {
        clsISUtilities clsISUtilities;
       
        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;
        private DataView pvtDateHistoryDataView;
        private DataView pvtDateCurrentDataView;
        private DataView pvtEmployeeDataView;
        private DataView pvtPayPeriodDataView;
        private DataView pvtPayCategoryDataView;
        private DataView pvtPayCategorySelectedDataView;
        private DataView pvtEmployeePayCategoryDataView;
        private DataView pvtTimeSheetRemoveDataView;
        private DataView pvtReportTimeSheetDataView;
        private DataView pvtPayCategoryExceptionDataView;
        private DataView pvtPayCategoryBreakDataView;
        private DataView pvtPayCategoryPublicHolidayDataView;
   
        private byte[] pvtbytCompress;

        private int pvtintDateCol0Width = 0;
        private int pvtintDateCol1Width = 0;
     
        private DateTime pvtDateTimeHistoryMin;
        private DateTime pvtDateTimeHistoryMax;

        private DateTime pvtDateTimeCurrentMin;
        private DateTime pvtDateTimeCurrentMax;

        private DateTime pvtDateTimeMinCompare = new DateTime(2200, 1, 1);

        private string pvtstrPayrollType = "";

        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
        private int pvtintDateDataGridViewRowIndex = -1;
        private int pvtintPayCategoryDataGridViewRowIndex = -1;

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnEmployeeDataGridViewLoaded = false;
        private bool pvtblnPayCategoryDataGridViewLoaded = false;
        private bool pvtblnCostCentreDataGridViewLoaded = false;
        private bool pvtblnDateDataGridViewLoaded = false;

        private int pvtintlblEmployeeTop = 0;
        private int pvtintdgvEmployeeDataGridViewTop = 0;
        private int pvtintdgvEmployeeDataGridViewHeight = 0;

        private int pvtintbtnEmployeeAddTop = 0;
        private int pvtintbtnEmployeeAddAllTop = 0;
        private int pvtintbtnEmployeeRemoveTop = 0;
        private int pvtintbtnEmployeeRemoveAllTop = 0;

        DateTime pvtdtWageDate = DateTime.Now.AddYears(-50);

        public frmRptTimeSheetSelection()
        {
            InitializeComponent();

            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 114;

                this.tabMainControl.Height += 114;
                this.tabControl.Height += 114;
                this.tabPage1.Height += 114;
                this.tabPage2.Height += 114;

                this.grbEmployee.Top += 114;
                this.grbCostCentre.Top += 114;
                this.grbTimeSheetOption.Top += 114;


                this.grbPeriodOption.Top += 114;
                this.grbDetailOption.Top += 114;
                this.grbPeriod.Top += 114;
                
                this.dgvPayCategoryDataGridView.Height += 38;
                this.dgvChosenEmployeeDataGridView.Height += 114;

                this.lblEmployee.Top += 38;
                this.dgvEmployeeDataGridView.Top += 38;
                this.dgvEmployeeDataGridView.Height += 76;

                this.btnEmployeeAdd.Top += 86;
                this.btnEmployeeAddAll.Top += 86;
                this.btnEmployeeRemove.Top += 86;
                this.btnEmployeeRemoveAll.Top += 86;

                this.btnCostCentreAdd.Top += 57;
                this.btnCostCentreAddAll.Top += 57;
                this.btnCostCentreRemove.Top += 57;
                this.btnCostCentreRemoveAll.Top += 57;

                this.dgvCostCentreDataGridView.Height += 114;
                this.dgvChosenCostCentreDataGridView.Height += 114;

                this.reportViewer.Height += 114;
            }

            pvtintlblEmployeeTop = this.lblEmployee.Top;
            pvtintdgvEmployeeDataGridViewTop = this.dgvEmployeeDataGridView.Top;
            pvtintdgvEmployeeDataGridViewHeight = this.dgvEmployeeDataGridView.Height;

            pvtintbtnEmployeeAddTop = this.btnEmployeeAdd.Top;
            pvtintbtnEmployeeAddAllTop = this.btnEmployeeAddAll.Top;
            pvtintbtnEmployeeRemoveTop = this.btnEmployeeRemove.Top;
            pvtintbtnEmployeeRemoveAllTop = this.btnEmployeeRemoveAll.Top;
        }

        private void frmRptTimeSheetSelection_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this,"busRptTimeSheet");

                pvtintDateCol0Width = this.dgvDateDataGridView.Columns[0].Width;
                pvtintDateCol1Width = this.dgvDateDataGridView.Columns[1].Width;
                
                clsISUtilities.Create_Calender_Control_From_TextBox(this.txtFromDate);
                clsISUtilities.Create_Calender_Control_From_TextBox(this.txtToDate);

                clsISUtilities.NotDataBound_Date_TextBox(txtFromDate, "");
                clsISUtilities.NotDataBound_Date_TextBox(txtToDate, "");

                clsISUtilities.NotDataBound_ComboBox(this.cboDateRule, "");

                clsISUtilities.Set_Form_For_Edit(false);

                clsISUtilities.Calender_Control_From_TextBox_Enable(this.txtFromDate);
                clsISUtilities.Calender_Control_From_TextBox_Enable(this.txtToDate);

                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSelectedEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPeriodHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayCategory.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSelectedCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                pvtDataSet = new DataSet();

                if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
                {
                    this.dgvPayrollTypeDataGridView.Rows.Add("Time Attendance");
                }
                else
                {
                    this.dgvPayrollTypeDataGridView.Rows.Add("Wages");
                    this.dgvPayrollTypeDataGridView.Rows.Add("Salaries");
                }

                pvtblnPayrollTypeDataGridViewLoaded = true;
                
                object[] objParm = new object[4];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();
                objParm[2] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[3] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);
                
                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView, 0);

                this.txtTime.KeyPress += new System.Windows.Forms.KeyPressEventHandler(clsISUtilities.Numeric_KeyPress);
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
            this.reportViewer.RefreshReport();
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

                    case "dgvEmployeeDataGridView":
                        
                        this.dgvEmployeeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvChosenEmployeeDataGridView":

                        this.dgvChosenEmployeeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvDateDataGridView":

                        pvtintDateDataGridViewRowIndex = -1;
                        this.dgvDateDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPayCategoryDataGridView":

                        pvtintPayCategoryDataGridViewRowIndex = -1;
                        this.dgvPayCategoryDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvCostCentreDataGridView":

                        this.dgvCostCentreDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvChosenCostCentreDataGridView":

                        this.dgvChosenCostCentreDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        private void rbnDateOther_Click(object sender, System.EventArgs e)
        {
            if (rbnDateOther.Checked == true)
            {
                this.dgvDateDataGridView.Visible = false;
                this.lblPeriodHeader.Visible = false;

                this.Clear_DataGridView(this.dgvDateDataGridView);

                this.txtFromDate.Text = "";
                this.txtToDate.Text = "";

                this.grbOtherDates.Visible = true;

                this.cboDateRule.SelectedIndex = -1;

                this.rbnShowDetailYes.Checked = false;
                this.rbnShowDetailNo.Checked = false;

                this.rbnShowDetailYes.Enabled = false;
                this.rbnShowDetailNo.Enabled = false;

                this.rbnEmployeeAll.Checked = true;
            }
            else
            {
                this.dgvDateDataGridView.Visible = true;
                this.lblPeriodHeader.Visible = true;
            }
        }

        private void cboDateRule_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.cboDateRule.SelectedIndex == -1)
            {
                this.pnlFromDate.Visible = false;
                this.pnlToDate.Visible = false;
            }
            else
            {
                this.pnlFromDate.Visible = true;

                if (cboDateRule.SelectedIndex == 3)
                {
                    this.pnlToDate.Visible = true;
                }
                else
                {
                    this.pnlToDate.Visible = false;
                }
            }
        }

        private void rbnMonth_Click(object sender, System.EventArgs e)
        {
            if (rbnMonth.Checked == true)
            {
                DateTime dtDateTime;

                this.lblPeriodHeader.Text = "Month";
                this.dgvDateDataGridView.Columns[0].HeaderText = "Description";

                this.dgvDateDataGridView.Columns[0].Width = pvtintDateCol0Width + pvtintDateCol1Width;
                this.dgvDateDataGridView.Columns[1].Visible = false;

                this.grbOtherDates.Visible = false;

                this.Clear_DataGridView(this.dgvDateDataGridView);

                pvtblnDateDataGridViewLoaded = false;

                if (this.rbnHistory.Checked == true)
                {
                    if (this.pvtDateHistoryDataView.Count > 0)
                    {
                        //if (this.pvtDayDataView[0].IsNull("MIN_TIMESHEET_DATE") == false)
                        if (pvtDateTimeHistoryMin != pvtDateTimeMinCompare)
                        {
                            dtDateTime = new DateTime(pvtDateTimeHistoryMax.Year, pvtDateTimeHistoryMax.Month, 1);

                            while (true)
                            {
                                this.dgvDateDataGridView.Rows.Add(dtDateTime.ToString("MMMM yyyy"),
                                                                                      "",
                                                                                      dtDateTime.ToString("yyyyMM"),
                                                                                      "");

                                dtDateTime = dtDateTime.AddMonths(-1);

                                if (Convert.ToInt32(dtDateTime.ToString("yyyyMM")) < Convert.ToInt32(pvtDateTimeHistoryMin.ToString("yyyyMM")))
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (this.pvtDateCurrentDataView.Count > 0)
                    {
                        //if (this.pvtDayDataView[0].IsNull("MIN_TIMESHEET_DATE") == false)
                        if (pvtDateTimeCurrentMin != pvtDateTimeMinCompare)
                        {
                            dtDateTime = new DateTime(pvtDateTimeCurrentMax.Year, pvtDateTimeCurrentMax.Month, 1);

                            while (true)
                            {
                                this.dgvDateDataGridView.Rows.Add(dtDateTime.ToString("MMMM yyyy"),
                                                                                      "",
                                                                                      dtDateTime.ToString("yyyyMM"),
                                                                                      "");

                                dtDateTime = dtDateTime.AddMonths(-1);

                                if (Convert.ToInt32(dtDateTime.ToString("yyyyMM")) < Convert.ToInt32(pvtDateTimeCurrentMin.ToString("yyyyMM")))
                                {
                                    break;
                                }
                            }
                        }
                    }

                }

                pvtblnDateDataGridViewLoaded = true;

                //Forces Show Of dgvDateDataGridView
                rbnDateOther_Click(sender, e);

                if (this.dgvDateDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvDateDataGridView, 0);

                    if (Convert.ToInt32(this.dgvDateDataGridView[2, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString() + "01") > 20121122)
                    {
                        if (this.rbnShowDetailYes.Checked == false
                            && this.rbnShowDetailNo.Checked == false)
                        {
                            this.rbnShowDetailNo.Checked = true;
                        }

                        this.rbnShowDetailYes.Enabled = true;
                        this.rbnShowDetailNo.Enabled = true;
                    }
                    else
                    {
                        this.rbnShowDetailYes.Checked = false;
                        this.rbnShowDetailNo.Checked = false;

                        this.rbnShowDetailYes.Enabled = false;
                        this.rbnShowDetailNo.Enabled = false;
                    }
                }
                else
                {
                    this.rbnShowDetailYes.Checked = false;
                    this.rbnShowDetailNo.Checked = false;

                    this.rbnShowDetailYes.Enabled = false;
                    this.rbnShowDetailNo.Enabled = false;
                }
            }
        }

        private void rbnWeek_Click(object sender, System.EventArgs e)
        {
            if (rbnWeek.Checked == true)
            {
                DateTime dtDateTime;

                this.grbOtherDates.Visible = false;

                this.lblPeriodHeader.Text = "Week";

                this.dgvDateDataGridView.Columns[0].HeaderText = "From Day";
                this.dgvDateDataGridView.Columns[1].HeaderText = "To Day";

                this.dgvDateDataGridView.Columns[0].Width = pvtintDateCol0Width;
                this.dgvDateDataGridView.Columns[1].Visible = true;

                this.Clear_DataGridView(this.dgvDateDataGridView);

                pvtblnDateDataGridViewLoaded = false;

                if (this.rbnHistory.Checked == true)
                {
                    if (this.pvtDateHistoryDataView.Count > 0)
                    {
                        //if (this.pvtDayDataView[0].IsNull("MIN_TIMESHEET_DATE") == false)
                        if (pvtDateTimeHistoryMin != pvtDateTimeMinCompare)
                        {
                            //Last Sunday
                            int intDayOfWeek = Convert.ToInt32(pvtDateTimeHistoryMax.DayOfWeek) * -1;


                            dtDateTime = pvtDateTimeHistoryMax.AddDays(intDayOfWeek + 1);

                            while (true)
                            {
                                this.dgvDateDataGridView.Rows.Add(dtDateTime.ToString("d MMMM yyyy"),
                                                                  dtDateTime.AddDays(6).ToString("d MMMM yyyy"),
                                                                  dtDateTime.ToString("yyyy-MM-dd"),
                                                                  dtDateTime.AddDays(6).ToString("yyyy-MM-dd"));

                                if (Convert.ToInt32(dtDateTime.ToString("yyyyMMdd")) < Convert.ToInt32(pvtDateTimeHistoryMin.ToString("yyyyMMdd")))
                                {
                                    break;
                                }

                                dtDateTime = dtDateTime.AddDays(-7);
                            }
                        }
                    }
                }
                else
                {
                    if (this.pvtDateCurrentDataView.Count > 0)
                    {
                        //if (this.pvtDayDataView[0].IsNull("MIN_TIMESHEET_DATE") == false)
                        if (pvtDateTimeCurrentMin != pvtDateTimeMinCompare)
                        {
                            //Last Sunday
                            int intDayOfWeek = Convert.ToInt32(pvtDateTimeCurrentMax.DayOfWeek) * -1;


                            dtDateTime = pvtDateTimeCurrentMax.AddDays(intDayOfWeek + 1);

                            while (true)
                            {
                                this.dgvDateDataGridView.Rows.Add(dtDateTime.ToString("d MMMM yyyy"),
                                                                  dtDateTime.AddDays(6).ToString("d MMMM yyyy"),
                                                                  dtDateTime.ToString("yyyy-MM-dd"),
                                                                  dtDateTime.AddDays(6).ToString("yyyy-MM-dd"));

                                if (Convert.ToInt32(dtDateTime.ToString("yyyyMMdd")) < Convert.ToInt32(pvtDateTimeCurrentMin.ToString("yyyyMMdd")))
                                {
                                    break;
                                }

                                dtDateTime = dtDateTime.AddDays(-7);
                            }
                        }
                    }

                }

                pvtblnDateDataGridViewLoaded = true;

                //Forces Show Of dgvDateDataGridView
                rbnDateOther_Click(sender, e);

                if (this.dgvDateDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvDateDataGridView, 0);

                    if (Convert.ToInt32(this.dgvDateDataGridView[2, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString().Replace("-", "")) > 20121122)
                    {
                        if (this.rbnShowDetailYes.Checked == false
                            && this.rbnShowDetailNo.Checked == false)
                        {
                            this.rbnShowDetailNo.Checked = true;
                        }

                        this.rbnShowDetailYes.Enabled = true;
                        this.rbnShowDetailNo.Enabled = true;
                    }
                    else
                    {
                        this.rbnShowDetailYes.Checked = false;
                        this.rbnShowDetailNo.Checked = false;

                        this.rbnShowDetailYes.Enabled = false;
                        this.rbnShowDetailNo.Enabled = false;
                    }
                }
                else
                {
                    this.rbnShowDetailYes.Checked = false;
                    this.rbnShowDetailNo.Checked = false;

                    this.rbnShowDetailYes.Enabled = false;
                    this.rbnShowDetailNo.Enabled = false;
                }
            }
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            try
            {
                DateTime dtFromDate = DateTime.Now;
                DateTime dtToDate = DateTime.Now;
                string strTableType = "";
                string strReportType = "";
                string strEmployeeNoIN = "";
                string strPayCategoryNoIN = "";
                string strFromDate = "";
                string strToDate = "";

                if (this.rbnEmployeeSelected.Checked == true)
                {
                    if (this.dgvChosenEmployeeDataGridView.Rows.Count == 0)
                    {
                        this.tabControl.SelectedIndex = 0;

                        CustomMessageBox.Show("Select Employee/s.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        this.btnEmployeeAdd.Focus();

                        return;
                    }
                    else
                    {
                        for (int intRow = 0; intRow < this.dgvChosenEmployeeDataGridView.Rows.Count; intRow++)
                        {
                            if (intRow == 0)
                            {
                                strEmployeeNoIN = "(" + this.dgvChosenEmployeeDataGridView[3, intRow].Value.ToString();
                            }
                            else
                            {
                                strEmployeeNoIN += "," + this.dgvChosenEmployeeDataGridView[3, intRow].Value.ToString();
                            }
                        }

                        strEmployeeNoIN += ") ";
                    }
                }

                if (this.rbnCostCentreSelected.Checked == true)
                {
                    if (this.dgvChosenCostCentreDataGridView.Rows.Count == 0)
                    {
                        this.tabControl.SelectedIndex = 1;

                        CustomMessageBox.Show("Select Cost Centre/s.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        this.btnEmployeeAdd.Focus();

                        return;
                    }
                    else
                    {
                        for (int intRow = 0; intRow < this.dgvChosenCostCentreDataGridView.Rows.Count; intRow++)
                        {
                            if (intRow == 0)
                            {
                                strPayCategoryNoIN = "(" + this.dgvChosenCostCentreDataGridView[1, intRow].Value.ToString();
                            }
                            else
                            {
                                strPayCategoryNoIN += "," + this.dgvChosenCostCentreDataGridView[1, intRow].Value.ToString();
                            }
                        }

                        strPayCategoryNoIN += ") ";
                    }
                }
               
                if (this.rbnDateOther.Checked == true)
                {
                    if (this.cboDateRule.SelectedIndex == -1)
                    {
                        CustomMessageBox.Show("Choose Other Dates Option (ComboBox).",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        return;
                    }

                    if (this.txtFromDate.Text.Length == 10)
                    {
                        dtFromDate = DateTime.ParseExact(this.txtFromDate.Text, AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);
                    }
                    else
                    {
                        CustomMessageBox.Show("Enter 'From Date'.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        return;
                    }

                    if (this.pnlToDate.Visible == true)
                    {
                        if (this.txtToDate.Text.Length == 10)
                        {
                            dtToDate = DateTime.ParseExact(this.txtToDate.Text, AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);

                            if (dtToDate <= dtFromDate)
                            {
                                CustomMessageBox.Show("'From Date' must be Less Than 'To Date'.",
                                this.Text,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);

                                return;
                            }
                        }
                        else
                        {
                            CustomMessageBox.Show("Enter 'To Date'.",
                                this.Text,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);

                            return;
                        }
                    }
                }
                else
                {
                    if (this.dgvDateDataGridView.Rows.Count == 0)
                    {
                        //return;
                    }
                }

                if (this.rbnFirstClockIn.Checked == true
                || this.rbnLastClockOut.Checked == true)
                {
                    if (this.cboTimeRule.SelectedIndex == -1)
                    {
                        CustomMessageBox.Show("Choose Time Sheet Options - Advanced (ComboBox).",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        this.cboTimeRule.Focus();

                        return;
                    }

                    if (this.txtTime.Text == "")
                    {
                        CustomMessageBox.Show("Enter Time.",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                        this.txtTime.Focus();

                        return;
                    }
                }

                if (this.rbnClockTypeAll.Checked == true
                || this.rbnClockTypeClock.Checked == true
                || this.rbnClockTypeUser.Checked == true)
                {
                    if (this.chkBreak.Checked == false
                    && this.chkTimeSheet.Checked == false)
                    {
                        CustomMessageBox.Show("Select Time Sheets / Breaks.",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                        this.chkTimeSheet.Focus();

                        return;
                    }

                    if (this.chkIn.Checked == false
                    && this.chkOut.Checked == false)
                    {
                        CustomMessageBox.Show("Select Start / End.",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                        this.chkIn.Focus();

                        return;
                    }
                }

                if (this.rbnCurrent.Checked == true)
                {
                    strTableType = "C";
                }
                else
                {
                    strTableType = "H";
                }

                if (this.rbnPayPeriod.Checked == true)
                {
                    strReportType = "P";

                    strFromDate = this.dgvDateDataGridView[2, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString();
                }
                else
                {
                    if (this.rbnWeek.Checked == true)
                    {
                        strReportType = "W";

                        strFromDate = this.dgvDateDataGridView[2, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString();
                        strToDate = this.dgvDateDataGridView[3, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString();
                    }
                    else
                    {
                        if (this.rbnMonth.Checked == true
                        || this.rbnPublicHoliday.Checked == true)
                        {
                            if (this.rbnMonth.Checked == true)
                            {
                                strReportType = "M";
                            }
                            else
                            {
                                //Public Holiday
                                strReportType = "H";
                            }

                            strFromDate = this.dgvDateDataGridView[2, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString();
                        }
                        else
                        {
                            if (this.rbnDateOther.Checked == true)
                            {
                                strFromDate = dtFromDate.ToString("yyyy-MM-dd");

                                if (this.cboDateRule.SelectedIndex == 0)
                                {
                                    strReportType = "E";
                                }
                                else
                                {
                                    if (this.cboDateRule.SelectedIndex == 1)
                                    {
                                        strReportType = "G";
                                    }
                                    else
                                    {
                                        if (this.cboDateRule.SelectedIndex == 2)
                                        {
                                            strReportType = "L";
                                        }
                                        else
                                        {
                                            strReportType = "B";

                                            strToDate = dtToDate.ToString("yyyy-MM-dd");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
           
                string strFilterType = "";
                string strTimeRule = "";
                int intTimeRule = 0;

                if (this.rbnNormal.Checked == true)
                {
                    strFilterType = "N";
                }
                else
                {
                    if (this.rbnExceptionAll.Checked == true)
                    {
                        strFilterType = "E";
                    }
                    else
                    {
                        if (this.rbnExceptionLow.Checked == true)
                        {
                            strFilterType = "L";
                        }
                        else
                        {
                            if (this.rbnExceptionHigh.Checked == true)
                            {
                                strFilterType = "H";
                            }
                            else
                            {
                                if (this.rbnErrors.Checked == true)
                                {
                                    strFilterType = "X";
                                }
                                else
                                {
                                    if (this.rbnFirstClockIn.Checked == true
                                    || this.rbnLastClockOut.Checked == true)
                                    {
                                        intTimeRule = Convert.ToInt32(this.txtTime.Text);

                                        if (this.cboTimeRule.SelectedIndex == 0)
                                        {
                                            strTimeRule = "G";
                                        }
                                        else
                                        {
                                            strTimeRule = "L";
                                        }

                                        if (this.rbnFirstClockIn.Checked == true)
                                        {
                                            strFilterType = "I";
                                        }
                                        else
                                        {
                                            strFilterType = "O";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                string strPrintOrder = "N";

                if (this.rbnPrintCode.Checked == true)
                {
                    strPrintOrder = "C";
                }
                else
                {
                    if (this.rbnPrintSurname.Checked == true)
                    {
                        strPrintOrder = "S";
                    }
                }

                this.reportViewer.Clear();
                this.reportViewer.Refresh();
                this.tabMainControl.SelectedIndex = 1;

                if (this.rbnClockTypeAll.Checked == true
                || this.rbnClockTypeClock.Checked == true
                || this.rbnClockTypeUser.Checked == true)
                {
                    string strTimeTypeInd = "A";

                    if (this.rbnClockTypeClock.Checked == true)
                    {
                        strTimeTypeInd = "C";
                    }
                    else
                    {
                        if (this.rbnClockTypeUser.Checked == true)
                        {
                            strTimeTypeInd = "U";
                        }
                    }

                    string strTimeTypeTableInd = "A";

                    if (this.chkBreak.Checked == true
                    && this.chkTimeSheet.Checked == true)
                    {
                    }
                    else
                    {
                        if (this.chkBreak.Checked == true)
                        {
                            strTimeTypeTableInd = "B";
                        }
                        else
                        {
                            strTimeTypeTableInd = "T";
                        }
                    }


                    string strTimeInOutInd = "A";

                    if (this.chkIn.Checked == true
                    && this.chkOut.Checked == true)
                    {
                    }
                    else
                    {
                        if (this.chkIn.Checked == true)
                        {
                            strTimeInOutInd = "I";
                        }
                        else
                        {
                            strTimeInOutInd = "O";
                        }
                    }
                    
                    object[] objParm = new object[14];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = pvtstrPayrollType;
                    objParm[2] = strTableType;
                    objParm[3] = strReportType;
                    objParm[4] = strEmployeeNoIN;
                    objParm[5] = strPayCategoryNoIN;
                    objParm[6] = strFromDate;
                    objParm[7] = strToDate;

                    objParm[8] = strFilterType;
                    objParm[9] = strTimeTypeInd;
                    objParm[10] = strTimeTypeTableInd;
                    objParm[11] = strTimeInOutInd;

                    objParm[12] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[13] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                    pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Print_Report_TimeType", objParm);

                    pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                    if (pvtTempDataSet.Tables["ReportTimeSheet"].Rows.Count == 0)
                    {
                        CustomMessageBox.Show("Empty Result Set.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        return;
                    }
                    else
                    {
                        this.tabPage2.Cursor = Cursors.WaitCursor;

                        pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("COMPANY_DESC", typeof(String));
                        pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("REPORT_HEADER_DESC", typeof(String));
                        pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("REPORT_DATETIME", typeof(String));
                        pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("REPORT_FILTER_DESC", typeof(String));
                        pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("REPORT_FILTER_APPLIED_DESC", typeof(String));

                        pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("PAY_CATEGORY_DESC", typeof(String));
                        pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("EMPLOYEE_CODE", typeof(String));
                        pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("EMPLOYEE_SURNAME", typeof(String));
                        pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("EMPLOYEE_NAME", typeof(String));

                        int intSavedEmployeeNo = -1;
                        int intSavedPayCategoryNo = -1;
                        int intFindRow = -1;
                        
                        DateTime dtSavedDayDate = DateTime.Now.AddYears(-50);
                        DateTime dtSavedPayPeriodDate = DateTime.Now.AddYears(-50);
                      
                        pvtReportTimeSheetDataView = null;
                        pvtReportTimeSheetDataView = new DataView(pvtTempDataSet.Tables["ReportTimeSheet"],
                            "",
                            "PAY_CATEGORY_NO,EMPLOYEE_NO,TIMESHEET_DATE",
                            DataViewRowState.CurrentRows);

                        for (int intRow = 0; intRow < pvtReportTimeSheetDataView.Count; intRow++)
                        {
                            if (Convert.ToInt32(pvtReportTimeSheetDataView[intRow]["PAY_CATEGORY_NO"]) != intSavedPayCategoryNo
                                | Convert.ToInt32(pvtReportTimeSheetDataView[intRow]["EMPLOYEE_NO"]) != intSavedEmployeeNo)
                            {
                                pvtPayCategoryExceptionDataView = null;
                                pvtPayCategoryExceptionDataView = new DataView(pvtTempDataSet.Tables["PayCategorySelected"],
                                      "PAY_CATEGORY_NO = " + pvtReportTimeSheetDataView[intRow]["PAY_CATEGORY_NO"].ToString(),
                                      "",
                                      DataViewRowState.CurrentRows);

                                intFindRow = pvtEmployeeDataView.Find(pvtReportTimeSheetDataView[intRow]["EMPLOYEE_NO"].ToString());


                                intSavedPayCategoryNo = Convert.ToInt32(pvtReportTimeSheetDataView[intRow]["PAY_CATEGORY_NO"]);
                                intSavedEmployeeNo = Convert.ToInt32(pvtReportTimeSheetDataView[intRow]["EMPLOYEE_NO"]);
                            }

                            pvtReportTimeSheetDataView[intRow]["PAY_CATEGORY_DESC"] = pvtPayCategoryExceptionDataView[0]["PAY_CATEGORY_DESC"].ToString();

                            pvtReportTimeSheetDataView[intRow]["EMPLOYEE_CODE"] = pvtEmployeeDataView[intFindRow]["EMPLOYEE_CODE"].ToString();
                            pvtReportTimeSheetDataView[intRow]["EMPLOYEE_SURNAME"] = pvtEmployeeDataView[intFindRow]["EMPLOYEE_SURNAME"].ToString();
                            pvtReportTimeSheetDataView[intRow]["EMPLOYEE_NAME"] = pvtEmployeeDataView[intFindRow]["EMPLOYEE_NAME"].ToString();
                        }

                        pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["COMPANY_DESC"] = pvtTempDataSet.Tables["ReportHeader"].Rows[0]["COMPANY_DESC"].ToString();
                        pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_DATETIME"] = pvtTempDataSet.Tables["ReportHeader"].Rows[0]["REPORT_DATETIME"].ToString();

                        pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_FILTER_DESC"] = "Filter : ";

                        if (this.chkBreak.Checked == true
                        && this.chkTimeSheet.Checked == true)
                        {
                            pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_FILTER_DESC"] += "All Records";
                        }
                        else
                        {
                            if (this.chkBreak.Checked == true)
                            {
                                pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_FILTER_DESC"] += "Break Records";
                            }
                            else
                            {
                                pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_FILTER_DESC"] += "Time Sheet Records";
                            }
                        }

                        if (this.rbnClockTypeAll.Checked == true)
                        {
                        }
                        else
                        {
                            if (this.rbnClockTypeClock.Checked == true)
                            {
                                pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_FILTER_DESC"] += " / Clock Only ";
                            }
                            else
                            {
                                pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_FILTER_DESC"] += " / User Only ";
                            }
                        }

                        if (this.chkIn.Checked == true
                        && this.chkOut.Checked == true)
                        {
                        }
                        else
                        {
                            if (this.chkIn.Checked == true)
                            {
                                pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_FILTER_DESC"] += " / Start";
                            }
                            else
                            {
                                pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_FILTER_DESC"] += " / End";
                            }
                        }
                        
                        if (this.rbnPayPeriod.Checked == true)
                        {
                            if (this.rbnHistory.Checked == true)
                            {
                                pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_HEADER_DESC"] = "Pay Period Date - " + this.dgvDateDataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString();
                            }
                            else
                            {
                                pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_HEADER_DESC"] = "All Current Records";
                            }
                        }
                        else
                        {
                            if (this.rbnWeek.Checked == true)
                            {
                                pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_HEADER_DESC"] = this.dgvDateDataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString() + " to " + this.dgvDateDataGridView[1, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString();
                            }
                            else
                            {
                                if (this.rbnMonth.Checked == true
                                || this.rbnPublicHoliday.Checked == true)
                                {
                                    if (this.rbnPublicHoliday.Checked == true)
                                    {
                                        pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_HEADER_DESC"] = "Public Holiday - " + this.dgvDateDataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString();
                                    }
                                    else
                                    {
                                        pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_HEADER_DESC"] = this.dgvDateDataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString();
                                    }
                                }
                                else
                                {
                                    pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_HEADER_DESC"] = this.cboDateRule.SelectedItem.ToString();

                                    if (this.cboDateRule.SelectedIndex == 3)
                                    {
                                        pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_HEADER_DESC"] += " - " + dtFromDate.ToString("d MMMM yyyy") + " and " + dtToDate.ToString("d MMMM yyyy");
                                    }
                                    else
                                    {
                                        pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_HEADER_DESC"] += " " + dtFromDate.ToString("d MMMM yyyy");
                                    }
                                }
                            }
                        }

                        this.tabPage2.Cursor = Cursors.Default;

                        pvtTempDataSet.AcceptChanges();

                        Microsoft.Reporting.WinForms.ReportDataSource myReportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource("Report", pvtTempDataSet.Tables["ReportTimeSheet"]);

                        this.reportViewer.LocalReport.ReportEmbeddedResource = "RptTimeSheet.ReportTimeSheetClockType.rdlc";
                      
                        this.reportViewer.LocalReport.DataSources.Clear();
                        this.reportViewer.LocalReport.DataSources.Add(myReportDataSource);

                        this.tabMainControl.SelectedIndex = 1;

                        this.reportViewer.RefreshReport();
                        this.reportViewer.Focus();
                    }
                }
                else
                {
                    if ((rbnShowDetailYes.Checked == true
                    || this.rbnFirstClockIn.Checked == true
                    || this.rbnLastClockOut.Checked == true)
                    && this.rbnErrors.Checked == false)
                    {
                        object[] objParm = new object[13];
                        objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                        objParm[1] = pvtstrPayrollType;
                        objParm[2] = strTableType;
                        objParm[3] = strReportType;
                        objParm[4] = strEmployeeNoIN;
                        objParm[5] = strPayCategoryNoIN;
                        objParm[6] = strFromDate;
                        objParm[7] = strToDate;

                        objParm[8] = strFilterType;
                        objParm[9] = strTimeRule;
                        objParm[10] = intTimeRule;

                        //objParm[11] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();
                        objParm[11] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                        objParm[12] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                        if (this.rbnErrors.Checked == true)
                        {
                            pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Print_Report_Simple_Errors", objParm);
                        }
                        else
                        {
                            pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Print_Report_Detailed", objParm);
                        }

                        pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                        if (pvtTempDataSet.Tables["ReportTimeSheet"].Rows.Count == 0)
                        {
                            CustomMessageBox.Show("Empty Result Set.",
                                this.Text,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                            return;
                        }
                        else
                        {
                            this.tabPage2.Cursor = Cursors.WaitCursor;

                            pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("COMPANY_DESC", typeof(String));
                            pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("REPORT_HEADER_DESC", typeof(String));
                            pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("REPORT_DATETIME", typeof(String));
                            pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("REPORT_FILTER_DESC", typeof(String));
                            pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("REPORT_FILTER_APPLIED_DESC", typeof(String));

                            pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("PAY_CATEGORY_DESC", typeof(String));
                            pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("EMPLOYEE_CODE", typeof(String));
                            pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("EMPLOYEE_SURNAME", typeof(String));
                            pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("EMPLOYEE_NAME", typeof(String));

                            if (rbnShowDetailYes.Checked == true
                            || this.rbnFirstClockIn.Checked == true
                            || this.rbnLastClockOut.Checked == true)
                            {
                                pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("WEEK_DATE_FROM", typeof(System.DateTime));
                                pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("WEEK_DATE", typeof(System.DateTime));

                                //pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("WEEK_ROUNDING_DESC", typeof(String));
                                pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("PAY_PERIOD_ROUNDING_DESC", typeof(String));

                                pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("WEEK_PAID_MINUTES_ROUNDED", typeof(System.Int32));
                                pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("WEEK_PAID_MINUTES_DECIMAL", typeof(System.Double));

                                pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("PAY_PERIOD_PAID_MINUTES_ROUNDED", typeof(System.Int32));
                                pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("PAY_PERIOD_PAID_MINUTES_DECIMAL", typeof(System.Double));
                            }

                            pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("DAY_ROUNDING_DESC", typeof(String));

                            pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("LUNCH_MINUTES", typeof(System.Int16));

                            pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("DAY_EXCEPTION_LOW_VALUE", typeof(System.Int16));
                            pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("DAY_EXCEPTION_HIGH_VALUE", typeof(System.Int16));

                            pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("BREAK_MINUTES", typeof(System.Int16));
                            pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("DAY_PAID_MINUTES", typeof(System.Int16));
                            pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("DAY_PAID_MINUTES_ROUNDED", typeof(System.Int16));

                            pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("DAY_PAID_MINUTES_DECIMAL", typeof(System.Double));
                            pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("EXCEPTION_INDICATOR", typeof(String));
                            pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("PUBLIC_HOLIDAY_INDICATOR", typeof(String));

                            int intSavedEmployeeNo = -1;
                            int intSavedPayCategoryNo = -1;
                            int intFindRow = -1;
                            int intPublicHolidayFindRow = -1;
                            object[] objPublicHolidayParm = new object[1];

                            int intFindPayCategoryExceptionRow = -1;

                            int intDayTotal = 0;
                            int intBreakDayTotal = 0;

                            DateTime dtSavedDayDate = DateTime.Now.AddYears(-50);
                            DateTime dtSavedPayPeriodDate = DateTime.Now.AddYears(-50);
                            int intDayExceptionLowValue = 0;
                            int intDayExceptionHighValue = 0;
                            string strDayRoundingDesc = "";

                            pvtReportTimeSheetDataView = null;
                            pvtReportTimeSheetDataView = new DataView(pvtTempDataSet.Tables["ReportTimeSheet"],
                                "",
                                "PAY_CATEGORY_NO,EMPLOYEE_NO,TIMESHEET_DATE",
                                DataViewRowState.CurrentRows);

                            for (int intRow = 0; intRow < pvtReportTimeSheetDataView.Count; intRow++)
                            {
                                if (intRow == 153)
                                {
                                    string strStop = "";
                                }


                                if (Convert.ToInt32(pvtReportTimeSheetDataView[intRow]["PAY_CATEGORY_NO"]) != intSavedPayCategoryNo
                                    | Convert.ToInt32(pvtReportTimeSheetDataView[intRow]["EMPLOYEE_NO"]) != intSavedEmployeeNo
                                    | Convert.ToDateTime(pvtReportTimeSheetDataView[intRow]["TIMESHEET_DATE"]) != dtSavedDayDate)
                                {
                                    if (intRow != 0)
                                    {
                                        Write_Employee_Day_Summary(intRow - 1, intFindPayCategoryExceptionRow, intDayTotal, intBreakDayTotal, intDayExceptionLowValue, intDayExceptionHighValue);
                                    }

                                    intFindRow = pvtEmployeeDataView.Find(pvtReportTimeSheetDataView[intRow]["EMPLOYEE_NO"].ToString());

                                    intDayTotal = 0;
                                    intBreakDayTotal = 0;

                                    if (this.rbnHistory.Checked == true)
                                    {
                                        if (intSavedPayCategoryNo != Convert.ToInt32(pvtReportTimeSheetDataView[intRow]["PAY_CATEGORY_NO"])
                                            | dtSavedPayPeriodDate != Convert.ToDateTime(pvtReportTimeSheetDataView[intRow]["PAY_PERIOD_DATE"]))
                                        {
                                            pvtPayCategoryExceptionDataView = null;
                                            pvtPayCategoryExceptionDataView = new DataView(pvtTempDataSet.Tables["PayCategoryException"],
                                                  "PAY_CATEGORY_NO = " + pvtReportTimeSheetDataView[intRow]["PAY_CATEGORY_NO"].ToString() + " AND PAY_PERIOD_DATE = '" + Convert.ToDateTime(pvtReportTimeSheetDataView[intRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'",
                                                  "WEEK_DATE_FROM",
                                                  DataViewRowState.CurrentRows);

                                            pvtPayCategoryBreakDataView = null;
                                            pvtPayCategoryBreakDataView = new DataView(pvtTempDataSet.Tables["PayCategoryBreak"],
                                                  "PAY_CATEGORY_NO = " + pvtReportTimeSheetDataView[intRow]["PAY_CATEGORY_NO"].ToString() + " AND PAY_PERIOD_DATE = '" + Convert.ToDateTime(pvtReportTimeSheetDataView[intRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'",
                                                  "",
                                                  DataViewRowState.CurrentRows);

                                            pvtPayCategoryPublicHolidayDataView = null;
                                            pvtPayCategoryPublicHolidayDataView = new DataView(pvtTempDataSet.Tables["PayCategoryPublicHoliday"],
                                                  "PAY_CATEGORY_NO = " + pvtReportTimeSheetDataView[intRow]["PAY_CATEGORY_NO"].ToString() + " AND PAY_PERIOD_DATE = '" + Convert.ToDateTime(pvtReportTimeSheetDataView[intRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'",
                                                  "PUBLIC_HOLIDAY_DATE",
                                                  DataViewRowState.CurrentRows);

                                            intSavedPayCategoryNo = Convert.ToInt32(pvtReportTimeSheetDataView[intRow]["PAY_CATEGORY_NO"]);
                                            dtSavedPayPeriodDate = Convert.ToDateTime(pvtReportTimeSheetDataView[intRow]["PAY_PERIOD_DATE"]);
                                        }
                                    }
                                    else
                                    {
                                        if (intSavedPayCategoryNo != Convert.ToInt32(pvtReportTimeSheetDataView[intRow]["PAY_CATEGORY_NO"]))
                                        {
                                            pvtPayCategoryExceptionDataView = null;
                                            pvtPayCategoryExceptionDataView = new DataView(pvtTempDataSet.Tables["PayCategoryException"],
                                                  "PAY_CATEGORY_NO = " + pvtReportTimeSheetDataView[intRow]["PAY_CATEGORY_NO"].ToString(),
                                                  "",
                                                  DataViewRowState.CurrentRows);

                                            pvtPayCategoryBreakDataView = null;
                                            pvtPayCategoryBreakDataView = new DataView(pvtTempDataSet.Tables["PayCategoryBreak"],
                                                  "PAY_CATEGORY_NO = " + pvtReportTimeSheetDataView[intRow]["PAY_CATEGORY_NO"].ToString(),
                                                  "",
                                                  DataViewRowState.CurrentRows);

                                            pvtPayCategoryPublicHolidayDataView = null;
                                            pvtPayCategoryPublicHolidayDataView = new DataView(pvtTempDataSet.Tables["PayCategoryPublicHoliday"],
                                                  "",
                                                  "PUBLIC_HOLIDAY_DATE",
                                                  DataViewRowState.CurrentRows);

                                            intSavedPayCategoryNo = Convert.ToInt32(pvtReportTimeSheetDataView[intRow]["PAY_CATEGORY_NO"]);
                                        }
                                    }

                                    intSavedEmployeeNo = Convert.ToInt32(pvtReportTimeSheetDataView[intRow]["EMPLOYEE_NO"]);

                                    //Force through Next Statement 
                                    dtSavedDayDate = DateTime.Now.AddYears(-50);
                                }

                                if (dtSavedDayDate != Convert.ToDateTime(pvtReportTimeSheetDataView[intRow]["TIMESHEET_DATE"]))
                                {
                                    dtSavedDayDate = Convert.ToDateTime(pvtReportTimeSheetDataView[intRow]["TIMESHEET_DATE"]);

                                    if (this.rbnHistory.Checked == true)
                                    {
                                        for (intFindPayCategoryExceptionRow = 0; intFindPayCategoryExceptionRow < pvtPayCategoryExceptionDataView.Count; intFindPayCategoryExceptionRow++)
                                        {
                                            if (dtSavedDayDate >= Convert.ToDateTime(pvtPayCategoryExceptionDataView[intFindPayCategoryExceptionRow]["WEEK_DATE_FROM"])
                                                & dtSavedDayDate <= Convert.ToDateTime(pvtPayCategoryExceptionDataView[intFindPayCategoryExceptionRow]["WEEK_DATE"]))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        intFindPayCategoryExceptionRow = 0;
                                    }

                                    intDayExceptionLowValue = Convert.ToInt16(pvtPayCategoryExceptionDataView[intFindPayCategoryExceptionRow]["EXCEPTION_" + dtSavedDayDate.DayOfWeek.ToString().Substring(0, 3).ToUpper() + "_BELOW_MINUTES"]);
                                    intDayExceptionHighValue = Convert.ToInt16(pvtPayCategoryExceptionDataView[intFindPayCategoryExceptionRow]["EXCEPTION_" + dtSavedDayDate.DayOfWeek.ToString().Substring(0, 3).ToUpper() + "_ABOVE_MINUTES"]);

                                    if (pvtPayCategoryExceptionDataView[intFindPayCategoryExceptionRow]["DAILY_ROUNDING_IND"].ToString() == "0")
                                    {
                                        strDayRoundingDesc = "Rounding - None";
                                    }
                                    else
                                    {
                                        if (pvtPayCategoryExceptionDataView[intFindPayCategoryExceptionRow]["DAILY_ROUNDING_IND"].ToString() == "1")
                                        {
                                            strDayRoundingDesc = "Round Up (" + pvtPayCategoryExceptionDataView[intFindPayCategoryExceptionRow]["DAILY_ROUNDING_MINUTES"].ToString() + ")";
                                        }
                                        else
                                        {
                                            if (pvtPayCategoryExceptionDataView[intFindPayCategoryExceptionRow]["DAILY_ROUNDING_IND"].ToString() == "2")
                                            {
                                                strDayRoundingDesc = "Round Down (" + pvtPayCategoryExceptionDataView[intFindPayCategoryExceptionRow]["DAILY_ROUNDING_MINUTES"].ToString() + ")";
                                            }
                                            else
                                            {
                                                if (pvtPayCategoryExceptionDataView[intFindPayCategoryExceptionRow]["DAILY_ROUNDING_IND"].ToString() == "3")
                                                {
                                                    strDayRoundingDesc = "Round Closest (" + pvtPayCategoryExceptionDataView[intFindPayCategoryExceptionRow]["DAILY_ROUNDING_MINUTES"].ToString() + ")";
                                                }
                                            }
                                        }
                                    }
                                }

                                objPublicHolidayParm[0] = Convert.ToDateTime(pvtReportTimeSheetDataView[intRow]["TIMESHEET_DATE"]);

                                intPublicHolidayFindRow = pvtPayCategoryPublicHolidayDataView.Find(objPublicHolidayParm);

                                if (intPublicHolidayFindRow > -1)
                                {
                                    pvtReportTimeSheetDataView[intRow]["PUBLIC_HOLIDAY_INDICATOR"] = "Y";
                                }

                                pvtReportTimeSheetDataView[intRow]["PAY_CATEGORY_DESC"] = pvtPayCategoryExceptionDataView[intFindPayCategoryExceptionRow]["PAY_CATEGORY_DESC"].ToString();

                                if (rbnShowDetailYes.Checked == true)
                                {
                                    if (this.rbnHistory.Checked == true)
                                    {
                                        pvtReportTimeSheetDataView[intRow]["WEEK_DATE_FROM"] = Convert.ToDateTime(pvtPayCategoryExceptionDataView[intFindPayCategoryExceptionRow]["WEEK_DATE_FROM"]);
                                        pvtReportTimeSheetDataView[intRow]["WEEK_DATE"] = Convert.ToDateTime(pvtPayCategoryExceptionDataView[intFindPayCategoryExceptionRow]["WEEK_DATE"]);
                                    }

                                    pvtReportTimeSheetDataView[intRow]["WEEK_PAID_MINUTES_ROUNDED"] = 0;
                                    pvtReportTimeSheetDataView[intRow]["WEEK_PAID_MINUTES_DECIMAL"] = 0;

                                    pvtReportTimeSheetDataView[intRow]["PAY_PERIOD_PAID_MINUTES_ROUNDED"] = 0;
                                    pvtReportTimeSheetDataView[intRow]["PAY_PERIOD_PAID_MINUTES_DECIMAL"] = 0;
                                }

                                pvtReportTimeSheetDataView[intRow]["DAY_ROUNDING_DESC"] = strDayRoundingDesc;
                                pvtReportTimeSheetDataView[intRow]["DAY_EXCEPTION_LOW_VALUE"] = intDayExceptionLowValue;
                                pvtReportTimeSheetDataView[intRow]["DAY_EXCEPTION_HIGH_VALUE"] = intDayExceptionHighValue;

                                pvtReportTimeSheetDataView[intRow]["EMPLOYEE_CODE"] = pvtEmployeeDataView[intFindRow]["EMPLOYEE_CODE"].ToString();
                                pvtReportTimeSheetDataView[intRow]["EMPLOYEE_SURNAME"] = pvtEmployeeDataView[intFindRow]["EMPLOYEE_SURNAME"].ToString();
                                pvtReportTimeSheetDataView[intRow]["EMPLOYEE_NAME"] = pvtEmployeeDataView[intFindRow]["EMPLOYEE_NAME"].ToString();

                                if (pvtReportTimeSheetDataView[intRow]["TIMESHEET_TIME_IN_MINUTES"] == System.DBNull.Value
                                || pvtReportTimeSheetDataView[intRow]["TIMESHEET_TIME_OUT_MINUTES"] == System.DBNull.Value)
                                {
                                }
                                else
                                {
                                    intDayTotal += Convert.ToInt32(pvtReportTimeSheetDataView[intRow]["TIMESHEET_TIME_OUT_MINUTES"]) - Convert.ToInt32(pvtReportTimeSheetDataView[intRow]["TIMESHEET_TIME_IN_MINUTES"]);
                                }

                                if (pvtReportTimeSheetDataView[intRow]["BREAK_TIME_IN_MINUTES"] == System.DBNull.Value
                                || pvtReportTimeSheetDataView[intRow]["BREAK_TIME_OUT_MINUTES"] == System.DBNull.Value)
                                {
                                }
                                else
                                {
                                    intBreakDayTotal += Convert.ToInt32(pvtReportTimeSheetDataView[intRow]["BREAK_TIME_OUT_MINUTES"]) - Convert.ToInt32(pvtReportTimeSheetDataView[intRow]["BREAK_TIME_IN_MINUTES"]);
                                }

                                if (intRow == pvtReportTimeSheetDataView.Count - 1)
                                {
                                    Write_Employee_Day_Summary(intRow, intFindPayCategoryExceptionRow, intDayTotal, intBreakDayTotal, intDayExceptionLowValue, intDayExceptionHighValue);
                                }
                                else
                                {
                                    pvtReportTimeSheetDataView[intRow]["LUNCH_MINUTES"] = 0;
                                    pvtReportTimeSheetDataView[intRow]["BREAK_MINUTES"] = 0;
                                    pvtReportTimeSheetDataView[intRow]["DAY_PAID_MINUTES"] = 0;
                                    pvtReportTimeSheetDataView[intRow]["DAY_PAID_MINUTES_ROUNDED"] = 0;
                                    pvtReportTimeSheetDataView[intRow]["DAY_PAID_MINUTES_DECIMAL"] = 0;
                                }
                            }

                            if (this.rbnNormal.Checked == true
                            | this.rbnExceptionAll.Checked == true
                            | this.rbnExceptionLow.Checked == true
                            | this.rbnExceptionHigh.Checked == true)
                            {
                                pvtTimeSheetRemoveDataView = null;

                                if (this.rbnNormal.Checked == true)
                                {
                                    pvtTimeSheetRemoveDataView = new DataView(pvtTempDataSet.Tables["ReportTimeSheet"],
                                    "EXCEPTION_INDICATOR <> 'Normal'",
                                    "",
                                    DataViewRowState.CurrentRows);
                                }
                                else
                                {
                                    if (this.rbnExceptionAll.Checked == true)
                                    {
                                        pvtTimeSheetRemoveDataView = new DataView(pvtTempDataSet.Tables["ReportTimeSheet"],
                                        "EXCEPTION_INDICATOR = 'Normal'",
                                        "",
                                        DataViewRowState.CurrentRows);
                                    }
                                    else
                                    {
                                        if (this.rbnExceptionLow.Checked == true)
                                        {
                                            pvtTimeSheetRemoveDataView = new DataView(pvtTempDataSet.Tables["ReportTimeSheet"],
                                            "EXCEPTION_INDICATOR <> 'Low'",
                                            "",
                                            DataViewRowState.CurrentRows);
                                        }
                                        else
                                        {
                                            if (this.rbnExceptionHigh.Checked == true)
                                            {
                                                pvtTimeSheetRemoveDataView = new DataView(pvtTempDataSet.Tables["ReportTimeSheet"],
                                                "EXCEPTION_INDICATOR <> 'High'",
                                                "",
                                                DataViewRowState.CurrentRows);
                                            }
                                        }
                                    }
                                }

                                for (int intRow = 0; intRow < pvtTimeSheetRemoveDataView.Count; intRow++)
                                {
                                    pvtTimeSheetRemoveDataView[intRow].Delete();

                                    intRow -= 1;
                                }

                                pvtTempDataSet.AcceptChanges();
                            }

                            int intPayPeriodPaidMinutesRounded = 0;

                            if (rbnShowDetailYes.Checked == true
                            || this.rbnFirstClockIn.Checked == true
                            || this.rbnLastClockOut.Checked == true)
                            {
                                if (this.rbnHistory.Checked == true)
                                {
                                    pvtPayCategoryExceptionDataView = null;
                                    pvtPayCategoryExceptionDataView = new DataView(pvtTempDataSet.Tables["PayCategoryException"],
                                          "",
                                          "PAY_CATEGORY_NO,PAY_PERIOD_DATE,WEEK_DATE_FROM",
                                          DataViewRowState.CurrentRows);

                                    object[] objectFind = new object[3];
                                    string strSavedEmployeeCode = "";

                                    var groupQuery = from row in pvtTempDataSet.Tables["ReportTimeSheet"].AsEnumerable()
                                                     group row by new
                                                     {
                                                         PayCategoryNo = row.Field<Int16>("PAY_CATEGORY_NO"),
                                                         PayCategoryDesc = row.Field<string>("PAY_CATEGORY_DESC"),
                                                         EmployeeCode = row.Field<string>("EMPLOYEE_CODE"),
                                                         PayPeriodDate = row.Field<DateTime>("PAY_PERIOD_DATE"),
                                                         WeekDateFrom = row.Field<DateTime>("WEEK_DATE_FROM")
                                                     } into grp
                                                     select new
                                                     {
                                                         PayCategoryNo = grp.Key.PayCategoryNo,
                                                         PayCategoryDesc = grp.Key.PayCategoryDesc,
                                                         EmployeeCode = grp.Key.EmployeeCode,
                                                         PayPeriodDate = grp.Key.PayPeriodDate,
                                                         WeekDateFrom = grp.Key.WeekDateFrom,
                                                         SumWeekPaidMinutesRounded = grp.Sum(r => r.Field<Int16>("DAY_PAID_MINUTES_ROUNDED"))
                                                     };

                                    foreach (var row in groupQuery)
                                    {
                                        if (row.EmployeeCode != strSavedEmployeeCode)
                                        {
                                            if (strSavedEmployeeCode != "")
                                            {
                                                if (pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_IND"].ToString() == "0")
                                                {
                                                    pvtReportTimeSheetDataView[0]["PAY_PERIOD_ROUNDING_DESC"] = "Pay Period Rounding - None";
                                                    pvtReportTimeSheetDataView[0]["PAY_PERIOD_PAID_MINUTES_ROUNDED"] = intPayPeriodPaidMinutesRounded;
                                                }
                                                else
                                                {
                                                    if (pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_IND"].ToString() == "1")
                                                    {
                                                        pvtReportTimeSheetDataView[0]["PAY_PERIOD_ROUNDING_DESC"] = "Pay Period Round Up (" + pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"].ToString() + ")";

                                                        if (intPayPeriodPaidMinutesRounded % Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"]) == 0)
                                                        {
                                                            pvtReportTimeSheetDataView[0]["PAY_PERIOD_PAID_MINUTES_ROUNDED"] = intPayPeriodPaidMinutesRounded;
                                                        }
                                                        else
                                                        {
                                                            pvtReportTimeSheetDataView[0]["PAY_PERIOD_PAID_MINUTES_ROUNDED"] = (intPayPeriodPaidMinutesRounded) + (Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"]) - ((intPayPeriodPaidMinutesRounded) % Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"])));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_IND"].ToString() == "2")
                                                        {
                                                            pvtReportTimeSheetDataView[0]["PAY_PERIOD_ROUNDING_DESC"] = "Pay Period Round Down (" + pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"].ToString() + ")";
                                                            pvtReportTimeSheetDataView[0]["PAY_PERIOD_PAID_MINUTES_ROUNDED"] = (intPayPeriodPaidMinutesRounded) - ((intPayPeriodPaidMinutesRounded) % Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"]));
                                                        }
                                                        else
                                                        {
                                                            if (pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_IND"].ToString() == "3")
                                                            {
                                                                pvtReportTimeSheetDataView[0]["PAY_PERIOD_ROUNDING_DESC"] = "Pay Period Round Closest (" + pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"].ToString() + ")";

                                                                if ((intPayPeriodPaidMinutesRounded) % Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"]) > Convert.ToDouble(Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"])) / 2)
                                                                {

                                                                    if (intPayPeriodPaidMinutesRounded % Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"]) == 0)
                                                                    {
                                                                        pvtReportTimeSheetDataView[0]["PAY_PERIOD_PAID_MINUTES_ROUNDED"] = intPayPeriodPaidMinutesRounded;
                                                                    }
                                                                    else
                                                                    {
                                                                        pvtReportTimeSheetDataView[0]["PAY_PERIOD_PAID_MINUTES_ROUNDED"] = (intPayPeriodPaidMinutesRounded) + (Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"]) - ((intPayPeriodPaidMinutesRounded) % Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"])));
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    pvtReportTimeSheetDataView[0]["PAY_PERIOD_PAID_MINUTES_ROUNDED"] = (intPayPeriodPaidMinutesRounded) - ((intPayPeriodPaidMinutesRounded) % Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"]));
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                                pvtReportTimeSheetDataView[0]["PAY_PERIOD_PAID_MINUTES_DECIMAL"] = Math.Round(Convert.ToDouble(pvtReportTimeSheetDataView[0]["PAY_PERIOD_PAID_MINUTES_ROUNDED"]) / 60, 2);
                                            }

                                            intPayPeriodPaidMinutesRounded = 0;
                                            strSavedEmployeeCode = row.EmployeeCode;
                                        }

                                        pvtReportTimeSheetDataView = null;
                                        pvtReportTimeSheetDataView = new DataView(pvtTempDataSet.Tables["ReportTimeSheet"],
                                            "PAY_CATEGORY_DESC = '" + row.PayCategoryDesc + "'"
                                             + " AND EMPLOYEE_CODE = '" + row.EmployeeCode + "'"
                                             + " AND PAY_PERIOD_DATE = '" + Convert.ToDateTime(row.PayPeriodDate).ToString("yyyy-MM-dd") + "'"
                                             + " AND WEEK_DATE_FROM = '" + Convert.ToDateTime(row.WeekDateFrom).ToString("yyyy-MM-dd") + "'",
                                            "EMPLOYEE_NO",
                                            DataViewRowState.CurrentRows);

                                        objectFind[0] = Convert.ToInt32(row.PayCategoryNo);
                                        objectFind[1] = Convert.ToDateTime(row.PayPeriodDate);
                                        objectFind[2] = Convert.ToDateTime(row.WeekDateFrom);

                                        intFindRow = pvtPayCategoryExceptionDataView.Find(objectFind);

                                        if (intFindRow > -1)
                                        {
                                            //pvtReportTimeSheetDataView[0]["WEEK_ROUNDING_DESC"] = "Week Rounding - None";
                                            pvtReportTimeSheetDataView[0]["WEEK_PAID_MINUTES_ROUNDED"] = Convert.ToInt32(row.SumWeekPaidMinutesRounded);

                                            pvtReportTimeSheetDataView[0]["WEEK_PAID_MINUTES_DECIMAL"] = Math.Round(Convert.ToDouble(pvtReportTimeSheetDataView[0]["WEEK_PAID_MINUTES_ROUNDED"]) / 60, 2);

                                            intPayPeriodPaidMinutesRounded += Convert.ToInt32(pvtReportTimeSheetDataView[0]["WEEK_PAID_MINUTES_ROUNDED"]);
                                        }
                                    }
                                }
                                else
                                {
                                    //Current
                                    pvtPayCategoryExceptionDataView = null;
                                    pvtPayCategoryExceptionDataView = new DataView(pvtTempDataSet.Tables["PayCategoryException"],
                                          "",
                                          "PAY_CATEGORY_NO",
                                          DataViewRowState.CurrentRows);

                                    object[] objectFind = new object[1];
                                    string strSavedEmployeeCode = "";

                                    var groupQuery = from row in pvtTempDataSet.Tables["ReportTimeSheet"].AsEnumerable()
                                                     group row by new
                                                     {
                                                         PayCategoryNo = row.Field<Int16>("PAY_CATEGORY_NO"),
                                                         PayCategoryDesc = row.Field<string>("PAY_CATEGORY_DESC"),
                                                         EmployeeCode = row.Field<string>("EMPLOYEE_CODE"),
                                                     } into grp
                                                     select new
                                                     {
                                                         PayCategoryNo = grp.Key.PayCategoryNo,
                                                         PayCategoryDesc = grp.Key.PayCategoryDesc,
                                                         EmployeeCode = grp.Key.EmployeeCode,
                                                         SumWeekPaidMinutesRounded = grp.Sum(r => r.Field<Int16>("DAY_PAID_MINUTES_ROUNDED"))
                                                     };

                                    foreach (var row in groupQuery)
                                    {
                                        if (row.EmployeeCode != strSavedEmployeeCode)
                                        {
                                            if (strSavedEmployeeCode != "")
                                            {
                                                if (pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_IND"].ToString() == "0")
                                                {
                                                    pvtReportTimeSheetDataView[0]["PAY_PERIOD_ROUNDING_DESC"] = "Pay Period Rounding - None";
                                                    pvtReportTimeSheetDataView[0]["PAY_PERIOD_PAID_MINUTES_ROUNDED"] = intPayPeriodPaidMinutesRounded;
                                                }
                                                else
                                                {
                                                    if (pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_IND"].ToString() == "1")
                                                    {
                                                        pvtReportTimeSheetDataView[0]["PAY_PERIOD_ROUNDING_DESC"] = "Pay Period Round Up (" + pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"].ToString() + ")";

                                                        if (intPayPeriodPaidMinutesRounded % Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"]) == 0)
                                                        {
                                                            pvtReportTimeSheetDataView[0]["PAY_PERIOD_PAID_MINUTES_ROUNDED"] = intPayPeriodPaidMinutesRounded;
                                                        }
                                                        else
                                                        {
                                                            pvtReportTimeSheetDataView[0]["PAY_PERIOD_PAID_MINUTES_ROUNDED"] = (intPayPeriodPaidMinutesRounded) + (Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"]) - ((intPayPeriodPaidMinutesRounded) % Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"])));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_IND"].ToString() == "2")
                                                        {
                                                            pvtReportTimeSheetDataView[0]["PAY_PERIOD_ROUNDING_DESC"] = "Pay Period Round Down (" + pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"].ToString() + ")";
                                                            pvtReportTimeSheetDataView[0]["PAY_PERIOD_PAID_MINUTES_ROUNDED"] = (intPayPeriodPaidMinutesRounded) - ((intPayPeriodPaidMinutesRounded) % Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"]));
                                                        }
                                                        else
                                                        {
                                                            if (pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_IND"].ToString() == "3")
                                                            {
                                                                pvtReportTimeSheetDataView[0]["PAY_PERIOD_ROUNDING_DESC"] = "Pay Period Round Closest (" + pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"].ToString() + ")";

                                                                if ((intPayPeriodPaidMinutesRounded) % Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"]) > Convert.ToDouble(Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"])) / 2)
                                                                {

                                                                    if (intPayPeriodPaidMinutesRounded % Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"]) == 0)
                                                                    {
                                                                        pvtReportTimeSheetDataView[0]["PAY_PERIOD_PAID_MINUTES_ROUNDED"] = intPayPeriodPaidMinutesRounded;
                                                                    }
                                                                    else
                                                                    {
                                                                        pvtReportTimeSheetDataView[0]["PAY_PERIOD_PAID_MINUTES_ROUNDED"] = (intPayPeriodPaidMinutesRounded) + (Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"]) - ((intPayPeriodPaidMinutesRounded) % Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"])));
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    pvtReportTimeSheetDataView[0]["PAY_PERIOD_PAID_MINUTES_ROUNDED"] = (intPayPeriodPaidMinutesRounded) - ((intPayPeriodPaidMinutesRounded) % Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"]));
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                                pvtReportTimeSheetDataView[0]["PAY_PERIOD_PAID_MINUTES_DECIMAL"] = Math.Round(Convert.ToDouble(pvtReportTimeSheetDataView[0]["PAY_PERIOD_PAID_MINUTES_ROUNDED"]) / 60, 2);
                                            }

                                            intPayPeriodPaidMinutesRounded = 0;
                                            strSavedEmployeeCode = row.EmployeeCode;
                                        }

                                        pvtReportTimeSheetDataView = null;
                                        pvtReportTimeSheetDataView = new DataView(pvtTempDataSet.Tables["ReportTimeSheet"],
                                            "PAY_CATEGORY_DESC = '" + row.PayCategoryDesc + "'"
                                             + " AND EMPLOYEE_CODE = '" + row.EmployeeCode + "'",
                                            "EMPLOYEE_NO",
                                            DataViewRowState.CurrentRows);

                                        objectFind[0] = Convert.ToInt32(row.PayCategoryNo);

                                        intFindRow = pvtPayCategoryExceptionDataView.Find(objectFind);

                                        if (intFindRow > -1)
                                        {
                                            //pvtReportTimeSheetDataView[0]["WEEK_ROUNDING_DESC"] = "Week Rounding - None";
                                            pvtReportTimeSheetDataView[0]["WEEK_PAID_MINUTES_ROUNDED"] = Convert.ToInt32(row.SumWeekPaidMinutesRounded);

                                            pvtReportTimeSheetDataView[0]["WEEK_PAID_MINUTES_DECIMAL"] = Math.Round(Convert.ToDouble(pvtReportTimeSheetDataView[0]["WEEK_PAID_MINUTES_ROUNDED"]) / 60, 2);

                                            intPayPeriodPaidMinutesRounded += Convert.ToInt32(pvtReportTimeSheetDataView[0]["WEEK_PAID_MINUTES_ROUNDED"]);
                                        }
                                    }
                                }

                                //Last Employee Rows Totals
                                //Last Employee Rows Totals
                                if (pvtReportTimeSheetDataView.Count > 0)
                                {
                                    if (pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_IND"].ToString() == "0")
                                    {
                                        pvtReportTimeSheetDataView[0]["PAY_PERIOD_ROUNDING_DESC"] = "Pay Period Rounding - None";
                                        pvtReportTimeSheetDataView[0]["PAY_PERIOD_PAID_MINUTES_ROUNDED"] = intPayPeriodPaidMinutesRounded;
                                    }
                                    else
                                    {
                                        if (pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_IND"].ToString() == "1")
                                        {
                                            pvtReportTimeSheetDataView[0]["PAY_PERIOD_ROUNDING_DESC"] = "Pay Period Round Up (" + pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"].ToString() + ")";
                                            pvtReportTimeSheetDataView[0]["PAY_PERIOD_PAID_MINUTES_ROUNDED"] = (intPayPeriodPaidMinutesRounded) + (Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"]) - ((intPayPeriodPaidMinutesRounded) % Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"])));
                                        }
                                        else
                                        {
                                            if (pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_IND"].ToString() == "2")
                                            {
                                                pvtReportTimeSheetDataView[0]["PAY_PERIOD_ROUNDING_DESC"] = "Pay Period Round Down (" + pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"].ToString() + ")";
                                                pvtReportTimeSheetDataView[0]["PAY_PERIOD_PAID_MINUTES_ROUNDED"] = (intPayPeriodPaidMinutesRounded) - ((intPayPeriodPaidMinutesRounded) % Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"]));
                                            }
                                            else
                                            {
                                                if (pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_IND"].ToString() == "3")
                                                {
                                                    pvtReportTimeSheetDataView[0]["PAY_PERIOD_ROUNDING_DESC"] = "Pay Period Round Closest (" + pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"].ToString() + ")";

                                                    if ((intPayPeriodPaidMinutesRounded) % Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"]) > Convert.ToDouble(Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"])) / 2)
                                                    {
                                                        pvtReportTimeSheetDataView[0]["PAY_PERIOD_PAID_MINUTES_ROUNDED"] = (intPayPeriodPaidMinutesRounded) + (Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"]) - ((intPayPeriodPaidMinutesRounded) % Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"])));
                                                    }
                                                    else
                                                    {
                                                        pvtReportTimeSheetDataView[0]["PAY_PERIOD_PAID_MINUTES_ROUNDED"] = (intPayPeriodPaidMinutesRounded) - ((intPayPeriodPaidMinutesRounded) % Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindRow]["PAY_PERIOD_ROUNDING_MINUTES"]));
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    pvtReportTimeSheetDataView[0]["PAY_PERIOD_PAID_MINUTES_DECIMAL"] = Math.Round(Convert.ToDouble(pvtReportTimeSheetDataView[0]["PAY_PERIOD_PAID_MINUTES_ROUNDED"]) / 60, 2);

                                    pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["COMPANY_DESC"] = pvtTempDataSet.Tables["ReportHeader"].Rows[0]["COMPANY_DESC"].ToString();
                                    pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_DATETIME"] = pvtTempDataSet.Tables["ReportHeader"].Rows[0]["REPORT_DATETIME"].ToString();

                                    if (this.rbnPayPeriod.Checked == true)
                                    {
                                        if (this.rbnHistory.Checked == true)
                                        {
                                            pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_HEADER_DESC"] = "Pay Period Date - " + this.dgvDateDataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString();
                                        }
                                        else
                                        {
                                            pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_HEADER_DESC"] = "All Current Records";
                                        }
                                    }
                                    else
                                    {
                                        if (this.rbnWeek.Checked == true)
                                        {
                                            pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_HEADER_DESC"] = this.dgvDateDataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString() + " to " + this.dgvDateDataGridView[1, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString();
                                        }
                                        else
                                        {
                                            if (this.rbnMonth.Checked == true
                                            || this.rbnPublicHoliday.Checked == true)
                                            {
                                                if (this.rbnPublicHoliday.Checked == true)
                                                {
                                                    pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_HEADER_DESC"] = "Public Holiday - " + this.dgvDateDataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString();
                                                }
                                                else
                                                {
                                                    pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_HEADER_DESC"] = this.dgvDateDataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString();
                                                }
                                            }
                                            else
                                            {
                                                pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_HEADER_DESC"] = this.cboDateRule.SelectedItem.ToString();

                                                if (this.cboDateRule.SelectedIndex == 3)
                                                {
                                                    pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_HEADER_DESC"] += " - " + dtFromDate.ToString("d MMMM yyyy") + " and " + dtToDate.ToString("d MMMM yyyy");
                                                }
                                                else
                                                {
                                                    pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_HEADER_DESC"] += " " + dtFromDate.ToString("d MMMM yyyy");
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    CustomMessageBox.Show("Empty Result Set.",
                                    this.Text,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                                    return;
                                }
                            }

                            if (this.rbnNormal.Checked == true
                            || this.rbnExceptionAll.Checked == true
                            || this.rbnExceptionLow.Checked == true
                            || this.rbnExceptionHigh.Checked == true
                            || this.rbnErrors.Checked == true
                            || this.rbnFirstClockIn.Checked == true
                            || this.rbnLastClockOut.Checked == true)
                            {
                                pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_FILTER_DESC"] = "Filter : ";

                                if (this.rbnNormal.Checked == true)
                                {
                                    pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_FILTER_DESC"] += "Normal";
                                }
                                else
                                {
                                    if (this.rbnExceptionAll.Checked == true)
                                    {
                                        pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_FILTER_DESC"] += "Exception (All)";
                                    }
                                    else
                                    {
                                        if (this.rbnExceptionLow.Checked == true)
                                        {
                                            pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_FILTER_DESC"] += "Exception (Low)";
                                        }
                                        else
                                        {
                                            if (this.rbnExceptionHigh.Checked == true)
                                            {
                                                pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_FILTER_DESC"] += "Exception (High)";
                                            }
                                            else
                                            {
                                                if (this.rbnErrors.Checked == true)
                                                {
                                                    pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_FILTER_DESC"] += "Errors";
                                                }
                                                else
                                                {
                                                    string strSign = " > ";

                                                    if (cboTimeRule.SelectedIndex == 1)
                                                    {
                                                        strSign = " < ";
                                                    }

                                                    strSign += Convert.ToInt32(txtTime.Text).ToString("#0:00");

                                                    if (this.rbnFirstClockIn.Checked == true)
                                                    {
                                                        pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_FILTER_DESC"] += "First Clock In " + strSign;
                                                    }
                                                    else
                                                    {
                                                        if (this.rbnLastClockOut.Checked == true)
                                                        {
                                                            pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_FILTER_DESC"] += "Last Clock Out " + strSign;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            this.tabPage2.Cursor = Cursors.Default;

                            pvtTempDataSet.AcceptChanges();

                            Microsoft.Reporting.WinForms.ReportDataSource myReportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource("Report", pvtTempDataSet.Tables["ReportTimeSheet"]);

                            if (rbnShowDetailYes.Checked == true)
                            {
                                if (this.rbnPayPeriod.Checked == true
                                && this.rbnHistory.Checked == true
                                && this.rbnAll.Checked == true)
                                {
                                    this.reportViewer.LocalReport.ReportEmbeddedResource = "RptTimeSheet.ReportTimeSheetWithDetailPayPeriod.rdlc";
                                }
                                else
                                {
                                    if (this.rbnErrors.Checked == true)
                                    {
                                        this.reportViewer.LocalReport.ReportEmbeddedResource = "RptTimeSheet.ReportTimeSheetWithDetailErrors.rdlc";
                                    }
                                    else
                                    {
                                        this.reportViewer.LocalReport.ReportEmbeddedResource = "RptTimeSheet.ReportTimeSheetWithDetail.rdlc";
                                    }
                                }
                            }
                            else
                            {
                                this.reportViewer.LocalReport.ReportEmbeddedResource = "RptTimeSheet.ReportTimeSheet.rdlc";
                            }

                            this.reportViewer.LocalReport.DataSources.Clear();
                            this.reportViewer.LocalReport.DataSources.Add(myReportDataSource);

                            this.tabMainControl.SelectedIndex = 1;

                            this.reportViewer.RefreshReport();
                            this.reportViewer.Focus();
                        }
                    }
                    else
                    {
                        object[] objParm = new object[14];
                        objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                        objParm[1] = pvtstrPayrollType;
                        objParm[2] = strTableType;
                        objParm[3] = strReportType;
                        objParm[4] = strEmployeeNoIN;
                        objParm[5] = strPayCategoryNoIN;
                        objParm[6] = strFromDate;
                        objParm[7] = strToDate;
                        objParm[8] = strFilterType;

                        objParm[9] = strTimeRule;
                        objParm[10] = intTimeRule;

                        objParm[11] = strPrintOrder;
                        objParm[12] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                        objParm[13] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                        if (this.rbnErrors.Checked == true)
                        {
                            pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Print_Report_Simple_Errors", objParm);
                        }
                        else
                        {
                            pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Print_Report_Simple", objParm);
                        }

                        pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                        if (pvtTempDataSet.Tables["ReportTimeSheet"].Rows.Count == 0)
                        {
                            CustomMessageBox.Show("Empty Result Set.",
                                this.Text,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                            return;
                        }
                        else
                        {
                            //Report
                            this.tabPage2.Cursor = Cursors.WaitCursor;

                            int intFindRow = -1;

                            pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("COMPANY_DESC", typeof(String));
                            pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("REPORT_HEADER_DESC", typeof(String));
                            pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("REPORT_DATETIME", typeof(String));
                            pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("REPORT_FILTER_APPLIED_DESC", typeof(String));
                            pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("REPORT_TIMESHEET_DESC", typeof(String));
                            pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("PAY_CATEGORY_DESC", typeof(String));
                            pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("EMPLOYEE_CODE", typeof(String));

                            if (this.rbnNormal.Checked == true
                            | this.rbnExceptionAll.Checked == true
                            | this.rbnExceptionLow.Checked == true
                            | this.rbnExceptionHigh.Checked == true)
                            {
                                pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("EMPLOYEE_NAMES", typeof(String));
                            }
                            else
                            {
                                pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("EMPLOYEE_SURNAME", typeof(String));
                                pvtTempDataSet.Tables["ReportTimeSheet"].Columns.Add("EMPLOYEE_NAME", typeof(String));
                            }

                            pvtReportTimeSheetDataView = null;
                            pvtReportTimeSheetDataView = new DataView(pvtTempDataSet.Tables["ReportTimeSheet"],
                                "",
                                "",
                                DataViewRowState.CurrentRows);

                            pvtPayCategorySelectedDataView = null;
                            pvtPayCategorySelectedDataView = new DataView(pvtTempDataSet.Tables["PayCategorySelected"],
                                        "",
                                        "PAY_CATEGORY_NO",
                                        DataViewRowState.CurrentRows);

                            int intPrevEmployeeNo = 0;
                            int intPrevPayCategoryNo = 0;

                            for (int intRow = 0; intRow < pvtReportTimeSheetDataView.Count; intRow++)
                            {
                                //if (intPrevPayCategoryNo != Convert.ToInt32(pvtReportTimeSheetDataView[intRow]["PAY_CATEGORY_NO"]))
                                //{
                                intPrevPayCategoryNo = Convert.ToInt32(pvtReportTimeSheetDataView[intRow]["PAY_CATEGORY_NO"]);

                                intFindRow = pvtPayCategorySelectedDataView.Find(intPrevPayCategoryNo);

                                pvtReportTimeSheetDataView[intRow]["PAY_CATEGORY_DESC"] = pvtPayCategorySelectedDataView[intFindRow]["PAY_CATEGORY_DESC"].ToString();
                                //}

                                if (this.rbnErrors.Checked == true)
                                {
                                    intPrevEmployeeNo = Convert.ToInt32(pvtReportTimeSheetDataView[intRow]["EMPLOYEE_NO"]);
                                }
                                else
                                {
                                    if (intPrevEmployeeNo == Convert.ToInt32(pvtReportTimeSheetDataView[intRow]["EMPLOYEE_NO"]))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        intPrevEmployeeNo = Convert.ToInt32(pvtReportTimeSheetDataView[intRow]["EMPLOYEE_NO"]);
                                    }
                                }

                                intFindRow = pvtEmployeeDataView.Find(pvtReportTimeSheetDataView[intRow]["EMPLOYEE_NO"].ToString());

                                pvtReportTimeSheetDataView[intRow]["EMPLOYEE_CODE"] = pvtEmployeeDataView[intFindRow]["EMPLOYEE_CODE"].ToString();

                                if (this.rbnNormal.Checked == true
                                || this.rbnExceptionAll.Checked == true
                                || this.rbnExceptionLow.Checked == true
                                || this.rbnExceptionHigh.Checked == true)
                                {
                                    if (this.rbnPrintName.Checked == true)
                                    {
                                        pvtReportTimeSheetDataView[intRow]["EMPLOYEE_NAMES"] = pvtEmployeeDataView[intFindRow]["EMPLOYEE_NAME"].ToString() + " " + pvtEmployeeDataView[intFindRow]["EMPLOYEE_SURNAME"].ToString();
                                    }
                                    else
                                    {
                                        pvtReportTimeSheetDataView[intRow]["EMPLOYEE_NAMES"] = pvtEmployeeDataView[intFindRow]["EMPLOYEE_SURNAME"].ToString() + ", " + pvtEmployeeDataView[intFindRow]["EMPLOYEE_NAME"].ToString();
                                    }
                                }
                                else
                                {
                                    pvtReportTimeSheetDataView[intRow]["EMPLOYEE_SURNAME"] = pvtEmployeeDataView[intFindRow]["EMPLOYEE_SURNAME"].ToString();
                                    pvtReportTimeSheetDataView[intRow]["EMPLOYEE_NAME"] = pvtEmployeeDataView[intFindRow]["EMPLOYEE_NAME"].ToString();
                                }
                            }

                            pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["COMPANY_DESC"] = pvtTempDataSet.Tables["ReportHeader"].Rows[0]["COMPANY_DESC"].ToString();
                            pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_DATETIME"] = pvtTempDataSet.Tables["ReportHeader"].Rows[0]["REPORT_DATETIME"].ToString();

                            if (this.rbnHistory.Checked == true)
                            {
                                pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_TIMESHEET_DESC"] = "History Time Sheets";
                            }
                            else
                            {
                                pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_TIMESHEET_DESC"] = "Current Time Sheets";
                            }

                            if (this.rbnCurrent.Checked == true)
                            {
                                pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_HEADER_DESC"] = "All Timesheets";
                            }
                            else
                            {
                                if (this.rbnPayPeriod.Checked == true)
                                {
                                    pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_HEADER_DESC"] = "Pay Period Date - " + this.dgvDateDataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString();
                                }
                                else
                                {
                                    if (this.rbnWeek.Checked == true)
                                    {
                                        pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_HEADER_DESC"] = this.dgvDateDataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString() + " to " + this.dgvDateDataGridView[1, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString();

                                    }
                                    else
                                    {
                                        if (this.rbnMonth.Checked == true
                                        || this.rbnPublicHoliday.Checked == true)
                                        {
                                            if (this.rbnPublicHoliday.Checked == true)
                                            {
                                                pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_HEADER_DESC"] = "Public Holiday - " + this.dgvDateDataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString();
                                            }
                                            else
                                            {
                                                pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_HEADER_DESC"] = this.dgvDateDataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString();
                                            }
                                        }
                                        else
                                        {
                                            pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_HEADER_DESC"] = this.cboDateRule.SelectedItem.ToString();

                                            if (this.cboDateRule.SelectedIndex == 3)
                                            {
                                                pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_HEADER_DESC"] += " - " + dtFromDate.ToString("d MMMM yyyy") + " and " + dtToDate.ToString("d MMMM yyyy");
                                            }
                                            else
                                            {
                                                pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_HEADER_DESC"] += " " + dtFromDate.ToString("d MMMM yyyy");
                                            }
                                        }
                                    }
                                }
                            }

                            if (this.rbnNormal.Checked == true
                            || this.rbnErrors.Checked == true
                            || this.rbnExceptionAll.Checked == true
                            || this.rbnExceptionLow.Checked == true
                            || this.rbnExceptionHigh.Checked == true)
                            {
                                pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_FILTER_APPLIED_DESC"] = "Filter : ";

                                if (this.rbnNormal.Checked == true)
                                {
                                    pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_FILTER_APPLIED_DESC"] += "Normal";
                                }
                                else
                                {
                                    if (this.rbnExceptionAll.Checked == true)
                                    {
                                        pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_FILTER_APPLIED_DESC"] += "Exception (All)";
                                    }
                                    else
                                    {
                                        if (this.rbnExceptionLow.Checked == true)
                                        {
                                            pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_FILTER_APPLIED_DESC"] += "Exception (Low)";
                                        }
                                        else
                                        {
                                            if (this.rbnExceptionHigh.Checked == true)
                                            {
                                                pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_FILTER_APPLIED_DESC"] += "Exception (High)";
                                            }
                                            else
                                            {
                                                if (this.rbnErrors.Checked == true)
                                                {
                                                    pvtTempDataSet.Tables["ReportTimeSheet"].Rows[0]["REPORT_FILTER_APPLIED_DESC"] += "Errors";
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            this.tabPage2.Cursor = Cursors.Default;

                            pvtTempDataSet.AcceptChanges();

                            Microsoft.Reporting.WinForms.ReportDataSource myReportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource("Report", pvtTempDataSet.Tables["ReportTimeSheet"]);

                            if (this.rbnErrors.Checked == true)
                            {
                                this.reportViewer.LocalReport.ReportEmbeddedResource = "RptTimeSheet.ReportTimeSheetWithDetailErrors.rdlc";
                            }
                            else
                            {
                                if (this.rbnNormal.Checked == true
                                || this.rbnExceptionAll.Checked == true
                                || this.rbnExceptionLow.Checked == true
                                || this.rbnExceptionHigh.Checked == true)
                                {
                                    this.reportViewer.LocalReport.ReportEmbeddedResource = "RptTimeSheet.ReportTimeSheetSimpleWithParameters.rdlc";
                                }
                                else
                                {
                                    this.reportViewer.LocalReport.ReportEmbeddedResource = "RptTimeSheet.ReportTimeSheetSimple.rdlc";
                                }
                            }

                            this.reportViewer.LocalReport.DataSources.Clear();
                            this.reportViewer.LocalReport.DataSources.Add(myReportDataSource);

                            this.tabMainControl.SelectedIndex = 1;
                            this.reportViewer.RefreshReport();
                            this.reportViewer.Focus();
                        }
                    }
                }
           }
            catch (Exception eException)
            {
                this.tabPage2.Cursor = Cursors.Default;
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void Write_Employee_Day_Summary(int intCurrentRow, int intFindPayCategoryExceptionRow, int intDayTotal, int intBreakDayTotal,int intDayExceptionLowValue,int intDayExceptionHighValue)
        {
            int intLunchMinutes = 0;
            int intBreakMinutes = 0;
            int intDayTotalRounded = 0;

            for (int intRow = 0; intRow < pvtPayCategoryBreakDataView.Count; intRow++)
            {
                if (intDayTotal >= Convert.ToInt32(pvtPayCategoryBreakDataView[intRow]["WORKED_TIME_MINUTES"]))
                {
                    intLunchMinutes = Convert.ToInt32(pvtPayCategoryBreakDataView[intRow]["BREAK_MINUTES"]);
                }
                else
                {
                    break;
                }
            }
           
            if (intBreakDayTotal > intLunchMinutes)
            {
                intBreakMinutes = intBreakDayTotal;
            }
            else
            {
                intBreakMinutes = intLunchMinutes;
            }

            intDayTotal -= intBreakMinutes;

            if (pvtPayCategoryExceptionDataView[intFindPayCategoryExceptionRow]["DAILY_ROUNDING_IND"].ToString() == "0")
            {
                intDayTotalRounded = intDayTotal;
            }
            else
            {
                if (pvtPayCategoryExceptionDataView[intFindPayCategoryExceptionRow]["DAILY_ROUNDING_IND"].ToString() == "1")
                {
                    intDayTotalRounded = (intDayTotal) + (Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindPayCategoryExceptionRow]["DAILY_ROUNDING_MINUTES"]) - ((intDayTotal) % Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindPayCategoryExceptionRow]["DAILY_ROUNDING_MINUTES"])));
                }
                else
                {
                    if (pvtPayCategoryExceptionDataView[intFindPayCategoryExceptionRow]["DAILY_ROUNDING_IND"].ToString() == "2")
                    {
                        intDayTotalRounded = (intDayTotal) - ((intDayTotal) % Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindPayCategoryExceptionRow]["DAILY_ROUNDING_MINUTES"]));
                    }
                    else
                    {
                        if (pvtPayCategoryExceptionDataView[intFindPayCategoryExceptionRow]["DAILY_ROUNDING_IND"].ToString() == "3")
                        {
                            if ((intDayTotal) % Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindPayCategoryExceptionRow]["DAILY_ROUNDING_MINUTES"]) > Convert.ToDouble(Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindPayCategoryExceptionRow]["DAILY_ROUNDING_MINUTES"])) / 2)
                            {
                                intDayTotalRounded = (intDayTotal) + (Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindPayCategoryExceptionRow]["DAILY_ROUNDING_MINUTES"]) - ((intDayTotal) % Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindPayCategoryExceptionRow]["DAILY_ROUNDING_MINUTES"])));
                            }
                            else
                            {
                                intDayTotalRounded = (intDayTotal) - ((intDayTotal) % Convert.ToInt32(pvtPayCategoryExceptionDataView[intFindPayCategoryExceptionRow]["DAILY_ROUNDING_MINUTES"]));
                            }
                        }
                    }
                }
            }

            DataView myDataView = new DataView(pvtTempDataSet.Tables["ReportTimeSheet"],
                                               "PAY_CATEGORY_NO = " + pvtReportTimeSheetDataView[intCurrentRow]["PAY_CATEGORY_NO"].ToString() +
                                               " AND EMPLOYEE_NO = " + pvtReportTimeSheetDataView[intCurrentRow]["EMPLOYEE_NO"].ToString() + 
                                               " AND TIMESHEET_DATE = '" + pvtReportTimeSheetDataView[intCurrentRow]["TIMESHEET_DATE"].ToString() + "'",
                       "PAY_CATEGORY_NO,EMPLOYEE_NO,TIMESHEET_DATE",
                       DataViewRowState.CurrentRows);


            if (intDayTotalRounded > intDayExceptionHighValue)
            {
                //pvtReportTimeSheetDataView[intCurrentRow]["EXCEPTION_INDICATOR"] = "High";

                for (int intRow = 0; intRow < myDataView.Count; intRow++)
                {
                    myDataView[intRow]["EXCEPTION_INDICATOR"] = "High";
                }
            }
            else
            {
                if (intDayTotalRounded < intDayExceptionLowValue)
                {
                    //pvtReportTimeSheetDataView[intCurrentRow]["EXCEPTION_INDICATOR"] = "Low";

                    for (int intRow = 0; intRow < myDataView.Count; intRow++)
                    {
                        myDataView[intRow]["EXCEPTION_INDICATOR"] = "Low";
                    }

                }
                else
                {
                    for (int intRow = 0; intRow < myDataView.Count; intRow++)
                    {
                        myDataView[intRow]["EXCEPTION_INDICATOR"] = "Normal";
                    }
                }
            }
         
            pvtReportTimeSheetDataView[intCurrentRow]["DAY_PAID_MINUTES"] = intDayTotal;
            pvtReportTimeSheetDataView[intCurrentRow]["DAY_PAID_MINUTES_ROUNDED"] = intDayTotalRounded;
            pvtReportTimeSheetDataView[intCurrentRow]["DAY_PAID_MINUTES_DECIMAL"] = Math.Round(Convert.ToDouble(intDayTotalRounded) / 60, 2);
            pvtReportTimeSheetDataView[intCurrentRow]["LUNCH_MINUTES"] = intLunchMinutes;
            pvtReportTimeSheetDataView[intCurrentRow]["BREAK_MINUTES"] = intBreakMinutes;
        }

        private void rbnEmployeeSelected_CheckedChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (rbnEmployeeSelected.Checked == true)
                {
                    pvtblnPayCategoryDataGridViewLoaded = false;

                    this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
                    this.Clear_DataGridView(this.dgvEmployeeDataGridView);
                    this.Clear_DataGridView(this.dgvChosenEmployeeDataGridView);

                    this.btnEmployeeAdd.Enabled = true;
                    this.btnEmployeeAddAll.Enabled = true;
                    this.btnEmployeeRemoveAll.Enabled = true;
                    this.btnEmployeeRemove.Enabled = true;

                    this.grbEmployeeSelection.Enabled = true;
                    this.rbnByPayCategory.Checked = true;
                   
                    for (int intIndex = 0; intIndex < pvtPayCategoryDataView.Count; intIndex++)
                    {
                        this.dgvPayCategoryDataGridView.Rows.Add(pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_DESC"].ToString(),
                                                                 pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_NO"].ToString());
                    }

                    pvtblnPayCategoryDataGridViewLoaded = true;

                    if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, 0);
                    }

                    this.tabControl.SelectedIndex = 0;
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void rbnEmployeeAll_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rbnEmployeeAll.Checked == true)
            {
                this.grbEmployeeSelection.Enabled = false;
                this.rbnByPayCategory.Checked = true;

                this.btnEmployeeAdd.Enabled = false;
                this.btnEmployeeAddAll.Enabled = false;
                this.btnEmployeeRemove.Enabled = false;
                this.btnEmployeeRemoveAll.Enabled = false;

                this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
                this.Clear_DataGridView(this.dgvEmployeeDataGridView);
                this.Clear_DataGridView(this.dgvChosenEmployeeDataGridView);

                this.tabControl.SelectedIndex = 0;
            }
        }

        private void btnEmployeeAdd_Click(object sender, System.EventArgs e)
        {
            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvEmployeeDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView)];

                this.dgvEmployeeDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvChosenEmployeeDataGridView.Rows.Add(myDataGridViewRow);

                if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, 0);
                }

                this.Set_DataGridView_SelectedRowIndex(this.dgvChosenEmployeeDataGridView, this.dgvChosenEmployeeDataGridView.Rows.Count - 1);
            }
        }

        private void btnEmployeeRemove_Click(object sender, System.EventArgs e)
        {
            if (this.dgvChosenEmployeeDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvChosenEmployeeDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvChosenEmployeeDataGridView)];

                int intRowPayCategoryNo = Convert.ToInt32(myDataGridViewRow.Cells[4].Value);
                 
                int intPayCategoryNo = Convert.ToInt32(dgvPayCategoryDataGridView[1, pvtintPayCategoryDataGridViewRowIndex].Value);

                this.dgvChosenEmployeeDataGridView.Rows.Remove(myDataGridViewRow);

                if (rbnByPayCategory.Checked == true)
                {
                    if (intRowPayCategoryNo == intPayCategoryNo)
                    {
                        this.dgvEmployeeDataGridView.Rows.Add(myDataGridViewRow);
                        this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1);
                    }
                }
                else
                {
                    this.dgvEmployeeDataGridView.Rows.Add(myDataGridViewRow);
                    this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1);
                }

                if (this.dgvChosenEmployeeDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvChosenEmployeeDataGridView, 0);
                }
            }
        }

        private void btnEmployeeRemoveAll_Click(object sender, System.EventArgs e)
        {
        btnRemoveAll_Click_Continue:

            if (this.dgvChosenEmployeeDataGridView.Rows.Count > 0)
            {
                btnEmployeeRemove_Click(null, null);

                goto btnRemoveAll_Click_Continue;
            }
        }

        private void btnEmployeeAddAll_Click(object sender, System.EventArgs e)
        {
        btnEmployeeAddAll_Click_Continue:

            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                btnEmployeeAdd_Click(null, null);

                goto btnEmployeeAddAll_Click_Continue;
            }
        }

        private void dgvPayrollTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPayrollTypeDataGridViewLoaded == true)
            {
                if (pvtintPayrollTypeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayrollTypeDataGridViewRowIndex = e.RowIndex;

                    pvtstrPayrollType = dgvPayrollTypeDataGridView[0, e.RowIndex].Value.ToString().Substring(0, 1);

                    string strFilter = "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'";

                    pvtEmployeeDataView = null;
                    pvtEmployeeDataView = new DataView(this.pvtDataSet.Tables["Employee"],
                        strFilter,
                        "EMPLOYEE_NO",
                        DataViewRowState.CurrentRows);

                    pvtPayCategoryDataView = null;
                    pvtPayCategoryDataView = new DataView(this.pvtDataSet.Tables["PayCategory"],
                       strFilter,
                       "PAY_CATEGORY_DESC",
                       DataViewRowState.CurrentRows);

                    pvtDateHistoryDataView = null;
                    pvtDateHistoryDataView = new DataView(this.pvtDataSet.Tables["Date"],
                        "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                        "",
                        DataViewRowState.CurrentRows);

                    pvtDateTimeHistoryMin = new DateTime(2200, 1, 1);
                    pvtDateTimeHistoryMax = new DateTime(1, 1, 1);

                    if (this.pvtDateHistoryDataView.Count > 0)
                    {
                        for (int intRow = 0; intRow < pvtDateHistoryDataView.Count; intRow++)
                        {
                            if (this.pvtDateHistoryDataView[intRow]["MIN_TIMESHEET_DATE"] != System.DBNull.Value)
                            {
                                if (intRow == 0)
                                {
                                    pvtDateTimeHistoryMin = Convert.ToDateTime(this.pvtDateHistoryDataView[intRow]["MIN_TIMESHEET_DATE"]);
                                    pvtDateTimeHistoryMax = Convert.ToDateTime(this.pvtDateHistoryDataView[intRow]["MAX_TIMESHEET_DATE"]);
                                }
                                else
                                {
                                    if (Convert.ToDateTime(this.pvtDateHistoryDataView[intRow]["MIN_TIMESHEET_DATE"]) < pvtDateTimeHistoryMin)
                                    {
                                        pvtDateTimeHistoryMin = Convert.ToDateTime(this.pvtDateHistoryDataView[intRow]["MIN_TIMESHEET_DATE"]);
                                    }

                                    if (Convert.ToDateTime(this.pvtDateHistoryDataView[intRow]["MAX_TIMESHEET_DATE"]) > pvtDateTimeHistoryMax)
                                    {
                                        pvtDateTimeHistoryMax = Convert.ToDateTime(this.pvtDateHistoryDataView[intRow]["MAX_TIMESHEET_DATE"]);
                                    }
                                }
                            }
                        }
                    }

                    pvtDateCurrentDataView = null;
                    pvtDateCurrentDataView = new DataView(this.pvtDataSet.Tables["DateCurrent"],
                        "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                        "",
                        DataViewRowState.CurrentRows);

                    pvtDateTimeCurrentMin = new DateTime(2200, 1, 1);
                    pvtDateTimeCurrentMax = new DateTime(1, 1, 1);

                    if (this.pvtDateCurrentDataView.Count > 0)
                    {
                        for (int intRow = 0; intRow < pvtDateCurrentDataView.Count; intRow++)
                        {
                            if (this.pvtDateCurrentDataView[intRow]["MIN_TIMESHEET_DATE"] != System.DBNull.Value)
                            {
                                if (intRow == 0)
                                {
                                    pvtDateTimeCurrentMin = Convert.ToDateTime(this.pvtDateCurrentDataView[intRow]["MIN_TIMESHEET_DATE"]);
                                    pvtDateTimeCurrentMax = Convert.ToDateTime(this.pvtDateCurrentDataView[intRow]["MAX_TIMESHEET_DATE"]);
                                }
                                else
                                {
                                    if (Convert.ToDateTime(this.pvtDateCurrentDataView[intRow]["MIN_TIMESHEET_DATE"]) < pvtDateTimeHistoryMin)
                                    {
                                        pvtDateTimeCurrentMin = Convert.ToDateTime(this.pvtDateCurrentDataView[intRow]["MIN_TIMESHEET_DATE"]);
                                    }

                                    if (Convert.ToDateTime(this.pvtDateCurrentDataView[intRow]["MAX_TIMESHEET_DATE"]) > pvtDateTimeHistoryMax)
                                    {
                                        pvtDateTimeCurrentMax = Convert.ToDateTime(this.pvtDateCurrentDataView[intRow]["MAX_TIMESHEET_DATE"]);
                                    }
                                }
                            }
                        }
                    }

                    this.rbnPayPeriod.Checked = true;
                    this.rbnPayPeriod_Click(sender, e);

                    if (this.pvtDataSet != null)
                    {
                        this.rbnCostCentreAll.Checked = true;
                        Clear_CostCentre();

                        if (this.rbnEmployeeAll.Checked == true)
                        {
                            rbnEmployeeAll_CheckedChanged(sender, e);
                        }
                        else
                        {
                            this.rbnEmployeeAll.Checked = true;
                        }
                    }
                }
            }
        }

        private void dgvEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnEmployeeDataGridViewLoaded == true)
            {
            }
        }

        private void dgvChosenEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnEmployeeAdd_Click(sender, e);
        }

        private void dgvChosenEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnEmployeeRemove_Click(sender, e);
        }

        private void rbnPayPeriod_Click(object sender, EventArgs e)
        {
            if (rbnPayPeriod.Checked == true)
            {
                if (this.rbnHistory.Checked == true)
                {
                    this.lblPeriodHeader.Text = "Pay Period";

                    this.grbOtherDates.Visible = false;

                    this.dgvDateDataGridView.Columns[0].HeaderText = "Description";

                    this.dgvDateDataGridView.Columns[0].Width = pvtintDateCol0Width + pvtintDateCol1Width;
                    this.dgvDateDataGridView.Columns[1].Visible = false;

                    this.Clear_DataGridView(this.dgvDateDataGridView);

                    pvtblnDateDataGridViewLoaded = false;

                    pvtPayPeriodDataView = null;
                    pvtPayPeriodDataView = new DataView(this.pvtDataSet.Tables["PayPeriodDate"],
                        "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                        "",
                        DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < pvtPayPeriodDataView.Count; intRow++)
                    {
                        this.dgvDateDataGridView.Rows.Add(Convert.ToDateTime(pvtPayPeriodDataView[intRow]["PAY_PERIOD_DATE"]).ToString("dd MMMM yyyy"),
                                                          "",
                                                          Convert.ToDateTime(pvtPayPeriodDataView[intRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd"),
                                                          "");
                    }

                    pvtblnDateDataGridViewLoaded = true;

                    //Forces Show Of dgvDateDataGridView
                    rbnDateOther_Click(sender, e);

                    if (this.dgvDateDataGridView.Rows.Count > 0)
                    {
                        this.btnOK.Enabled = true;
                        this.Set_DataGridView_SelectedRowIndex(this.dgvDateDataGridView, 0);

                        if (Convert.ToInt32(this.dgvDateDataGridView[2, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString().Replace("-", "")) > 20121122)
                        {
                            if (this.rbnShowDetailYes.Checked == false
                            && this.rbnShowDetailNo.Checked == false)
                            {
                                this.rbnShowDetailNo.Checked = true;
                            }

                            this.rbnShowDetailYes.Enabled = true;
                            this.rbnShowDetailNo.Enabled = true;
                        }
                        else
                        {
                            this.rbnShowDetailYes.Checked = false;
                            this.rbnShowDetailNo.Checked = false;

                            this.rbnShowDetailYes.Enabled = false;
                            this.rbnShowDetailNo.Enabled = false;
                        }
                    }
                    else
                    {
                        this.btnOK.Enabled = false;

                        this.rbnShowDetailYes.Checked = false;
                        this.rbnShowDetailNo.Checked = false;

                        this.rbnShowDetailYes.Enabled = false;
                        this.rbnShowDetailNo.Enabled = false;
                    }
                }
                else
                {
                    this.rbnErrors.Visible = true;

                    this.dgvDateDataGridView.Visible = false;
                    this.lblPeriodHeader.Visible = false;
                    this.grbOtherDates.Visible = false;
                }
            }
        }

        private void dgvDateDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnDateDataGridViewLoaded == true)
            {
                if (pvtintDateDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintDateDataGridViewRowIndex = e.RowIndex;

                    if (rbnPayPeriod.Checked == true)
                    {
                        pvtdtWageDate = DateTime.ParseExact(this.dgvDateDataGridView[2, e.RowIndex].Value.ToString(), "yyyy-MM-dd", null);
                    }

                    this.rbnEmployeeAll.Checked = true;
                }
            }
        }

        private void dgvPayCategoryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPayCategoryDataGridViewLoaded == true)
            {
                if (pvtintPayCategoryDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayCategoryDataGridViewRowIndex = e.RowIndex;

                    int intPayCategoryNo = Convert.ToInt32(dgvPayCategoryDataGridView[1, e.RowIndex].Value);

                    string strFilter = "COMPANY_NO = " + Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    strFilter += " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_CATEGORY_NO = " + intPayCategoryNo;
                    
                    if (rbnHistory.Checked == true)
                    {
                        if (rbnPayPeriod.Checked == true)
                        {
                            strFilter += " AND PAY_PERIOD_DATE = '" + pvtdtWageDate.ToString("yyyy-MM-dd") + "'";
                        }
                        
                        pvtEmployeePayCategoryDataView = null;
                        pvtEmployeePayCategoryDataView = new DataView(this.pvtDataSet.Tables["EmployeePayCategory"],
                            strFilter,
                            "EMPLOYEE_NO",
                            DataViewRowState.CurrentRows);
                    }
                    else
                    {
                        pvtEmployeePayCategoryDataView = null;
                        pvtEmployeePayCategoryDataView = new DataView(this.pvtDataSet.Tables["EmployeePayCategoryCurrent"],
                            strFilter,
                            "EMPLOYEE_NO",
                            DataViewRowState.CurrentRows);
                    }

                    int intFindRow = -1;
                    bool blnFound = false;

                    this.Clear_DataGridView(this.dgvEmployeeDataGridView);

                    for (int intIndex = 0; intIndex < pvtEmployeeDataView.Count; intIndex++)
                    {
                        intFindRow = pvtEmployeePayCategoryDataView.Find(Convert.ToInt32(pvtEmployeeDataView[intIndex]["EMPLOYEE_NO"]));

                        if (intFindRow == -1)
                        {
                            continue;
                        }
                        else
                        {
                            blnFound = false;

                            //Exclude These Employees 
                            for (int intRow = 0; intRow < this.dgvChosenEmployeeDataGridView.Rows.Count; intRow++)
                            {
                                if (this.dgvChosenEmployeeDataGridView[3, intRow].Value.ToString() == pvtEmployeeDataView[intIndex]["EMPLOYEE_NO"].ToString())
                                {
                                    blnFound = true;
                                    break;
                                }
                            }
                        }

                        if (blnFound == true)
                        {
                            continue;
                        }

                        this.dgvEmployeeDataGridView.Rows.Add(pvtEmployeeDataView[intIndex]["EMPLOYEE_CODE"].ToString(),
                                                              pvtEmployeeDataView[intIndex]["EMPLOYEE_SURNAME"].ToString(),
                                                              pvtEmployeeDataView[intIndex]["EMPLOYEE_NAME"].ToString(),
                                                              pvtEmployeeDataView[intIndex]["EMPLOYEE_NO"].ToString(),
                                                              intPayCategoryNo.ToString());
                    }

                    if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, 0);
                    }
                }
            }
        }

        private void dgvDateDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (dgvDateDataGridView[e.Column.Index + 2, e.RowIndex1].Value.ToString() == "")
            {
                e.SortResult = -1;
            }
            else
            {
                if (dgvDateDataGridView[e.Column.Index + 2, e.RowIndex2].Value.ToString() == "")
                {
                    e.SortResult = 1;
                }
                else
                {
                    if (double.Parse(dgvDateDataGridView[e.Column.Index + 2, e.RowIndex1].Value.ToString().Replace("-", "")) > double.Parse(dgvDateDataGridView[e.Column.Index + 2, e.RowIndex2].Value.ToString().Replace("-", "")))
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(dgvDateDataGridView[e.Column.Index + 2, e.RowIndex1].Value.ToString().Replace("-", "")) < double.Parse(dgvDateDataGridView[e.Column.Index + 2, e.RowIndex2].Value.ToString().Replace("-", "")))
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

        private void rbnCostCentreSelected_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rbnCostCentreSelected.Checked == true)
                {
                    pvtblnCostCentreDataGridViewLoaded = false;

                    this.Clear_DataGridView(this.dgvCostCentreDataGridView);
                    this.Clear_DataGridView(this.dgvChosenCostCentreDataGridView);

                    this.btnCostCentreAdd.Enabled = true;
                    this.btnCostCentreAddAll.Enabled = true;
                    this.btnCostCentreRemoveAll.Enabled = true;
                    this.btnCostCentreRemove.Enabled = true;

                    for (int intIndex = 0; intIndex < pvtPayCategoryDataView.Count; intIndex++)
                    {
                        this.dgvCostCentreDataGridView.Rows.Add(pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_DESC"].ToString(),
                                                                 pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_NO"].ToString());
                    }

                    pvtblnCostCentreDataGridViewLoaded = true;

                    if (this.dgvCostCentreDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvCostCentreDataGridView, 0);
                    }

                    this.tabControl.SelectedIndex = 1;
                }

            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void Clear_CostCentre()
        {
            this.Clear_DataGridView(this.dgvCostCentreDataGridView);
            this.Clear_DataGridView(this.dgvChosenCostCentreDataGridView);

            this.btnCostCentreAdd.Enabled = false;
            this.btnCostCentreAddAll.Enabled = false;
            this.btnCostCentreRemoveAll.Enabled = false;
            this.btnCostCentreRemove.Enabled = false;
        }

        private void dgvCostCentreDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvChosenCostCentreDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnCostCentreAdd_Click(object sender, EventArgs e)
        {
            if (this.dgvCostCentreDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvCostCentreDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvCostCentreDataGridView)];

                this.dgvCostCentreDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvChosenCostCentreDataGridView.Rows.Add(myDataGridViewRow);

                if (this.dgvCostCentreDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvCostCentreDataGridView, 0);
                }

                this.Set_DataGridView_SelectedRowIndex(this.dgvChosenCostCentreDataGridView, this.dgvChosenCostCentreDataGridView.Rows.Count - 1);
            }
        }

        private void btnCostCentreRemove_Click(object sender, EventArgs e)
        {
            if (this.dgvChosenCostCentreDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvChosenCostCentreDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvChosenCostCentreDataGridView)];

                this.dgvChosenCostCentreDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvCostCentreDataGridView.Rows.Add(myDataGridViewRow);

                if (this.dgvChosenCostCentreDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvChosenCostCentreDataGridView, 0);
                }

                this.Set_DataGridView_SelectedRowIndex(this.dgvCostCentreDataGridView, this.dgvCostCentreDataGridView.Rows.Count - 1);
            }
        }

        private void btnCostCentreAddAll_Click(object sender, EventArgs e)
        {
        btnCostCentreAddAll_Click_Continue:

            if (this.dgvCostCentreDataGridView.Rows.Count > 0)
            {
                btnCostCentreAdd_Click(null, null);

                goto btnCostCentreAddAll_Click_Continue;
            }
        }

        private void btnCostCentreRemoveAll_Click(object sender, EventArgs e)
        {
        btnCostCentreRemoveAll_Click_Continue:

            for (int intRowCount = 0; intRowCount < this.dgvChosenCostCentreDataGridView.Rows.Count;intRowCount++)
            {
                btnCostCentreRemove_Click(sender, e);

                goto btnCostCentreRemoveAll_Click_Continue;
            }
        }

        private void dgvCostCentreDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnCostCentreAdd_Click(sender, e);
        }

        private void dgvChosenCostCentreDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnCostCentreRemove_Click(sender, e);
        }

        private void rbnCostCentreAll_Click(object sender, EventArgs e)
        {
            Clear_CostCentre();

            this.tabControl.SelectedIndex = 1;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmRptTimeSheetSelection_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
            {
                //Cancel Button
                this.Close();
            }
        }

        private void tabMainControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
            {
                //Cancel Button
                this.Close();
            }
        }

        private void reportViewer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
            {
                //Cancel Button
                this.Close();
            }
        }

        private void txtDates_TextChanged(object sender, EventArgs e)
        {
            if (cboDateRule.SelectedIndex > -1)
            {
                if (cboDateRule.SelectedIndex == 3)
                {
                    if (this.txtFromDate.Text != ""
                    && this.txtToDate.Text != "")
                    {
                        return;
                    }
                }
                else
                {
                    if (this.txtFromDate.Text != "")
                    {
                    }
                    else
                    {
                        return;
                    }
                }

                DateTime dtFromDate = DateTime.ParseExact(this.txtFromDate.Text, AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);

                if (Convert.ToInt32(dtFromDate.ToString("yyyyMMdd")) > 20121122)
                {
                    this.rbnShowDetailNo.Checked = true;

                    this.rbnShowDetailYes.Enabled = true;
                    this.rbnShowDetailNo.Enabled = true;
                }
                else
                {
                    this.rbnShowDetailYes.Checked = false;
                    this.rbnShowDetailNo.Checked = false;

                    this.rbnShowDetailYes.Enabled = false;
                    this.rbnShowDetailNo.Enabled = false;
                }
            }
        }

        private void rbnShowDetailChoice_Click(object sender, EventArgs e)
        {
            RadioButton myRadioButton = (RadioButton)sender;

            if (myRadioButton.Name == "rbnShowDetailYes")
            {
                grbEmployeeOrder.Visible = false;
            }
            else
            {
                grbEmployeeOrder.Visible = true;
            }
        }

        private void rbnPublicHoliday_Click(object sender, EventArgs e)
        {
            if (rbnPublicHoliday.Checked == true)
            {
                this.lblPeriodHeader.Text = "Public Holiday";

                this.grbOtherDates.Visible = false;

                this.dgvDateDataGridView.Columns[0].HeaderText = "Date";
                this.dgvDateDataGridView.Columns[1].HeaderText = "Description";

                this.dgvDateDataGridView.Columns[0].Width = pvtintDateCol0Width;
                this.dgvDateDataGridView.Columns[1].Width = pvtintDateCol1Width;
                this.dgvDateDataGridView.Columns[1].Visible = true;

                this.Clear_DataGridView(this.dgvDateDataGridView);

                pvtblnDateDataGridViewLoaded = false;

                pvtPayPeriodDataView = null;
                pvtPayPeriodDataView = new DataView(this.pvtDataSet.Tables["PublicHolidayDate"],
                    "",
                    "",
                    DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < pvtPayPeriodDataView.Count; intRow++)
                {
                    this.dgvDateDataGridView.Rows.Add(Convert.ToDateTime(pvtPayPeriodDataView[intRow]["PUBLIC_HOLIDAY_DATE"]).ToString("dd MMMM yyyy"),
                                                      pvtPayPeriodDataView[intRow]["PUBLIC_HOLIDAY_DESC"].ToString(),
                                                      Convert.ToDateTime(pvtPayPeriodDataView[intRow]["PUBLIC_HOLIDAY_DATE"]).ToString("yyyy-MM-dd"),
                                                      "");
                }

                pvtblnDateDataGridViewLoaded = true;

                //Forces Show Of dgvDateDataGridView
                rbnDateOther_Click(sender, e);

                if (this.dgvDateDataGridView.Rows.Count > 0)
                {
                    this.btnOK.Enabled = true;
                    this.Set_DataGridView_SelectedRowIndex(this.dgvDateDataGridView, 0);

                    if (Convert.ToInt32(this.dgvDateDataGridView[2, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString().Replace("-", "")) > 20121122)
                    {
                        if (this.rbnShowDetailYes.Checked == false
                            && this.rbnShowDetailNo.Checked == false)
                        {
                            this.rbnShowDetailNo.Checked = true;
                        }

                        this.rbnShowDetailYes.Enabled = true;
                        this.rbnShowDetailNo.Enabled = true;
                    }
                    else
                    {
                        this.rbnShowDetailYes.Checked = false;
                        this.rbnShowDetailNo.Checked = false;

                        this.rbnShowDetailYes.Enabled = false;
                        this.rbnShowDetailNo.Enabled = false;
                    }
                }
                else
                {
                    this.btnOK.Enabled = false;

                    this.rbnShowDetailYes.Checked = false;
                    this.rbnShowDetailNo.Checked = false;

                    this.rbnShowDetailYes.Enabled = false;
                    this.rbnShowDetailNo.Enabled = false;
                }
            }
        }

        private void rbnHistoryCurrent_Click(object sender, EventArgs e)
        {
            RadioButton myRadioButton = (RadioButton)sender;

            if (myRadioButton.Name == "rbnHistory")
            {
                this.rbnPayPeriod.Text = "Pay Period";
            }
            else
            {
                this.rbnPayPeriod.Text = "All";
            }

            this.rbnErrors.Visible = false;

            if (this.rbnErrors.Checked == true)
            {
                this.rbnAll.Checked = true;
            }

            rbnPayPeriod.Checked = true;

            rbnPayPeriod_Click(rbnPayPeriod, e);

            this.rbnEmployeeAll.Checked = true;

            this.grbPeriod.Visible = true;
        }

        private void tabOptionControl_Click(object sender, EventArgs e)
        {
            TabControl myTabControl = (TabControl)sender;

            if (myTabControl.SelectedIndex == 0)
            {
                this.rbnFirstClockIn.Checked = false;
                this.rbnLastClockOut.Checked = false;

                this.rbnClockTypeAll.Checked = false;
                this.rbnClockTypeUser.Checked = false;
                this.rbnClockTypeClock.Checked = false;

                this.rbnAll.Checked = true;

                this.grbDetailOption.Visible = true;
            }
            else
            {
                if (myTabControl.SelectedIndex == 1)
                {
                    this.rbnFirstClockIn.Checked = false;
                    this.rbnLastClockOut.Checked = false;

                    this.rbnAll.Checked = false;
                    this.rbnNormal.Checked = false;
                    this.rbnExceptionAll.Checked = false;
                    this.rbnExceptionHigh.Checked = false;
                    this.rbnExceptionLow.Checked = false;
                    this.rbnErrors.Checked = false;

                    this.rbnClockTypeAll.Checked = true;

                    this.chkBreak.Checked = true;
                    this.chkTimeSheet.Checked = true;

                    this.chkIn.Checked = true;
                    this.chkOut.Checked = true;

                    this.grbDetailOption.Visible = false;
                }
                else
                {
                    this.rbnAll.Checked = false;
                    this.rbnNormal.Checked = false;
                    this.rbnExceptionAll.Checked = false;
                    this.rbnExceptionHigh.Checked = false;
                    this.rbnExceptionLow.Checked = false;
                    this.rbnErrors.Checked = false;

                    this.rbnClockTypeAll.Checked = false;
                    this.rbnClockTypeUser.Checked = false;
                    this.rbnClockTypeClock.Checked = false;

                    this.rbnFirstClockIn.Checked = true;

                    this.rbnShowDetailYes.Checked = true;

                    this.grbDetailOption.Visible = false;
                }
            }
        }

        private void rbnErrors_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton myRadioButton = (RadioButton)sender;

            if (myRadioButton.Name == "rbnErrors")
            {
                if (myRadioButton.Checked == true)
                {
                    this.grbDetailOption.Visible = false;
                }
                else
                {
                    this.grbDetailOption.Visible = true;
                }
            }
        }

        private void rbnByPayCategory_CheckedChanged(object sender, EventArgs e)
        {
            if (rbnByPayCategory.Checked == true)
            {
                this.lblEmployee.Top = pvtintlblEmployeeTop;
                this.dgvEmployeeDataGridView.Top = pvtintdgvEmployeeDataGridViewTop;
                this.dgvEmployeeDataGridView.Height = pvtintdgvEmployeeDataGridViewHeight;

                this.lblEmployee.BringToFront();

                this.btnEmployeeAdd.Top = pvtintbtnEmployeeAddTop;
                this.btnEmployeeAddAll.Top = pvtintbtnEmployeeAddAllTop;
                this.btnEmployeeRemove.Top = pvtintbtnEmployeeRemoveTop;
                this.btnEmployeeRemoveAll.Top = pvtintbtnEmployeeRemoveAllTop;

                if (dgvPayCategoryDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, 0);
                }

                tabControl.SelectedIndex = 0;
            }
        }

        private void rbnEmployeeOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (rbnEmployeeOnly.Checked == true)
            {
                this.lblEmployee.Top = lblSelectedEmployee.Top;

                this.dgvEmployeeDataGridView.Top = this.dgvChosenEmployeeDataGridView.Top;
                this.dgvEmployeeDataGridView.Height = this.dgvChosenEmployeeDataGridView.Height;
                this.dgvEmployeeDataGridView.BringToFront();

                this.lblEmployee.BringToFront();

                this.btnEmployeeAdd.Top = this.btnCostCentreAdd.Top;
                this.btnEmployeeAddAll.Top = this.btnCostCentreAddAll.Top;
                this.btnEmployeeRemove.Top = this.btnCostCentreRemove.Top;
                this.btnEmployeeRemoveAll.Top = this.btnCostCentreRemoveAll.Top;

                bool blnFound = false;
                
                this.Clear_DataGridView(this.dgvEmployeeDataGridView);
                
                string strFilter = "COMPANY_NO = " + Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo"));
                strFilter += " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'";

                if (rbnHistory.Checked == true)
                {
                    if (rbnPayPeriod.Checked == true)
                    {
                        strFilter += " AND PAY_PERIOD_DATE = '" + pvtdtWageDate.ToString("yyyy-MM-dd") + "'";
                    }

                    pvtEmployeePayCategoryDataView = null;
                    pvtEmployeePayCategoryDataView = new DataView(this.pvtDataSet.Tables["EmployeePayCategory"],
                        strFilter,
                        "EMPLOYEE_NO",
                        DataViewRowState.CurrentRows);
                }
                else
                {
                    pvtEmployeePayCategoryDataView = null;
                    pvtEmployeePayCategoryDataView = new DataView(this.pvtDataSet.Tables["EmployeePayCategoryCurrent"],
                        strFilter,
                        "EMPLOYEE_NO",
                        DataViewRowState.CurrentRows);
                }

                int intFindRow = -1;
                
                for (int intIndex = 0; intIndex < pvtEmployeeDataView.Count; intIndex++)
                {
                    intFindRow = pvtEmployeePayCategoryDataView.Find(Convert.ToInt32(pvtEmployeeDataView[intIndex]["EMPLOYEE_NO"]));

                    if (intFindRow == -1)
                    {
                        continue;
                    }
                    else
                    {
                        blnFound = false;

                        //Exclude These Employees 
                        for (int intRow = 0; intRow < this.dgvChosenEmployeeDataGridView.Rows.Count; intRow++)
                        {
                            if (this.dgvChosenEmployeeDataGridView[3, intRow].Value.ToString() == pvtEmployeeDataView[intIndex]["EMPLOYEE_NO"].ToString())
                            {
                                blnFound = true;
                                break;
                            }
                        }
                    }

                    if (blnFound == true)
                    {
                        continue;
                    }

                    this.dgvEmployeeDataGridView.Rows.Add(pvtEmployeeDataView[intIndex]["EMPLOYEE_CODE"].ToString(),
                                                            pvtEmployeeDataView[intIndex]["EMPLOYEE_SURNAME"].ToString(),
                                                            pvtEmployeeDataView[intIndex]["EMPLOYEE_NAME"].ToString(),
                                                            pvtEmployeeDataView[intIndex]["EMPLOYEE_NO"].ToString(),
                                                            "-1");
                }

                if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, 0);
                }

                tabControl.SelectedIndex = 0;
            }
        }
    }
}
