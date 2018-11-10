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
    public partial class frmTimeSheetBatch : Form
    {
        clsISUtilities clsISUtilities;

        private int pvtintPayCategoryNo;
        private int pvtintPayCategoryTableRowNo;

        private string pvtstrTableDef = "TIMESHEET";
        private string pvtstrTableName = "Timesheet";

        private DateTime pvtdtDayDateTime;

        private DataSet pvtDataSet;
       
        private DataView pvtPayCategoryDataView;
        private DataView pvtPayCategoryFromDateDataView;
        private DataView pvtEmployeeDataView;
        private DataView pvtEmployeeRejectedDataView;
        private DataView pvtTimesheetOrBreakDataView;
        private DataView pvtPublicHolidayDataView;
        private DataView pvtEmployeeLeaveDataView;

        private byte[] pvtbytCompress;

        private bool pvtblnAddAll = false;

        private DateTime pvtdtDateTimeTo;

        private string pvtstrPayrollType = "";
        private string pvtstrLockType = "";

        private int pvtintTimerCount = 0;

        private bool pvtblnPublicHoliday = false;
        
        private int pvtintPayrollTypeRowIndex = -1;
        private int pvtintPayCategoryDataGridViewRowIndex = -1;
        private int pvtintEmployeeDataGridViewRowIndex = -1;
        private int pvtintEmployeeChosenDataGridViewRowIndex = -1;
        private int pvtintDayChosenDataGridViewRowIndex = -1;
        private int pvtintDayDataGridViewRowIndex = -1;

        private int pvtintPayCategoryNoSaved = -1;
        private string pvtstrPayrollTypeSaved = "";
      
        DataGridViewCellStyle RejectedDataGridViewCellStyle;
        DataGridViewCellStyle NormalDataGridViewCellStyle;
        DataGridViewCellStyle PayrollRunDataGridViewCellStyle;
        DataGridViewCellStyle PayrollRunPendingDataGridViewCellStyle;
        DataGridViewCellStyle AuthorisedDataGridViewCellStyle;
        DataGridViewCellStyle PublicHolidayDataGridViewCellStyle;
        DataGridViewCellStyle LeaveDataGridViewCellStyle;
        
        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnPayCategoryDataGridViewLoaded = false;
        private bool pvtblnEmployeeDataGridViewLoaded = false;
        private bool pvtblnEmployeeChosenDataGridViewLoaded = true;
        private bool pvtblnEmployeeRejectedDataGridViewLoaded = false;
        private bool pvtblnDayDataGridViewLoaded = false;
        private bool pvtblnDayChosenDataGridViewLoaded = false;

        //dgvPayCategoryDataGridView Cols
        private int pvtintPayCategoryDescColPayCategoryDataGridView = 4;
        private int pvtintLastUploadDateTimeColPayCategoryDataGridView = 5;
        private int pvtintRowColPayCategoryDataGridView = 6;
        private int pvtintLastUploadDateTimeSortColPayCategoryDataGridView = 7;

        //dgvDayDataGridView and dgvDayChosenDataGridView Cols
        private int pvtintDateColDayDataGridView = 4;
        private int pvtintDateSortColDayDataGridView = 5;
        private int pvtintLockColDayDataGridView = 6;

        //dgvEmployeeDataGridView and dgvEmployeeChosenDataGridView Cols
        private int pvtintCodeColEmployeeDataGridView = 4;
        private int pvtintSurnameColEmployeeDataGridView = 5;
        private int pvtintNameColEmployeeDataGridView = 6;
        private int pvtintClockStartColEmployeeDataGridView = 7;
        private int pvtintStartColEmployeeDataGridView = 8;
        private int pvtintClockEndColEmployeeDataGridView = 9;
        private int pvtintEndColEmployeeDataGridView = 10;
        private int pvtintEmployeeNoColEmployeeDataGridView = 11;
        private int pvtintEmployeeSeqNoColEmployeeDataGridView = 12;
        
        public frmTimeSheetBatch()
        {
            InitializeComponent();
            
            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 114;

                this.dgvPayCategoryDataGridView.Height += 38;

                this.lblDate.Top += 38;
                this.lblChosenDates.Top += 38;

                this.dgvDayDataGridView.Top += 38;
                this.dgvDayDataGridView.Height += 38;
                this.dgvDayChosenDataGridView.Top += 38;
                this.dgvDayChosenDataGridView.Height += 38;

                this.picDayLock.Top += 38;

                this.btnDateAdd.Top += 57;
                this.btnDateRemove.Top += 57;
                
                this.lblEmployee.Top += 76;
                this.lblEmployeeStart.Top += 76;
                this.lblEmployeeEnd.Top += 76;
                this.lblEmployeeBlank.Top += 76;
                
                this.lblChosenEmployee.Top += 76;
                this.lblChosenEmployeeStart.Top += 76;
                this.lblChosenEmployeeEnd.Top += 76;
                this.lblChosenEmployeeBlank.Top += 76;

                this.dgvEmployeeDataGridView.Top += 76;
                this.dgvEmployeeDataGridView.Height += 38;

                this.btnAdd.Top += 88;
                this.btnAddAll.Top += 88;
                this.btnRemove.Top += 88;
                this.btnRemoveAll.Top += 88;
                
                this.lblEmployeeRejected.Top += 114;
                this.lblEmployeeRejectedStart.Top += 114;
                this.lblEmployeeRejectedEnd.Top += 114;
                this.lblEmployeeRejectedBlank.Top += 114;

                this.dgvEmployeeRejectedDataGridView.Top += 114;

                this.dgvEmployeeChosenDataGridView.Top += 76;
                this.dgvEmployeeChosenDataGridView.Height += 38;

                this.lblEmployeeChosenRejected.Top += 114;
                this.lblEmployeeChosenRejectedStart.Top += 114;
                this.lblEmployeeChosenRejectedEnd.Top += 114;
                this.lblEmployeeChosenRejectedBlank.Top += 114;

                this.dgvEmployeeChosenRejectedDataGridView.Top += 114;
            }
        }

        private void frmTimeSheetBatch_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this,"busTimeSheetBatch");

                pvtdtDayDateTime = DateTime.Now;
                
                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployeeStart.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployeeEnd.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployeeBlank.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployeeRejectedBlank.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblChosenEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblChosenEmployeeStart.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblChosenEmployeeEnd.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblChosenEmployeeBlank.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblDate.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblChosenDates.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployeeRejected.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployeeRejectedStart.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployeeRejectedEnd.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployeeChosenRejected.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployeeChosenRejectedStart.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployeeChosenRejectedEnd.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployeeChosenRejectedBlank.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                RejectedDataGridViewCellStyle = new DataGridViewCellStyle();
                RejectedDataGridViewCellStyle.BackColor = Color.Yellow;
                RejectedDataGridViewCellStyle.SelectionBackColor = Color.Yellow;

                NormalDataGridViewCellStyle = new DataGridViewCellStyle();
                NormalDataGridViewCellStyle.BackColor = SystemColors.Control;
                NormalDataGridViewCellStyle.SelectionBackColor = SystemColors.Control;

                PayrollRunDataGridViewCellStyle = new DataGridViewCellStyle();
                PayrollRunDataGridViewCellStyle.BackColor = Color.Magenta;
                PayrollRunDataGridViewCellStyle.SelectionBackColor = Color.Magenta;

                AuthorisedDataGridViewCellStyle = new DataGridViewCellStyle();
                AuthorisedDataGridViewCellStyle.BackColor = Color.Cyan;
                AuthorisedDataGridViewCellStyle.SelectionBackColor = Color.Cyan;

                PayrollRunPendingDataGridViewCellStyle = new DataGridViewCellStyle();
                PayrollRunPendingDataGridViewCellStyle.BackColor = Color.Orange;
                PayrollRunPendingDataGridViewCellStyle.SelectionBackColor = Color.Orange;
                
                PublicHolidayDataGridViewCellStyle = new DataGridViewCellStyle();
                PublicHolidayDataGridViewCellStyle.BackColor = Color.SlateBlue;
                PublicHolidayDataGridViewCellStyle.SelectionBackColor = Color.SlateBlue;

                LeaveDataGridViewCellStyle = new DataGridViewCellStyle();
                LeaveDataGridViewCellStyle.BackColor = Color.RoyalBlue;
                LeaveDataGridViewCellStyle.SelectionBackColor = Color.RoyalBlue;

                object[] objParm = new object[4];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[2] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[3] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                
                pvtDataSet.Tables.Add("PayrollType");
                DataTable PayrollTypeDataTable = new DataTable("PayrollType");
                pvtDataSet.Tables["PayrollType"].Columns.Add("PAYROLL_TYPE_DESC", typeof(String));

                if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
                {
                    DataRow drDataRow = pvtDataSet.Tables["PayrollType"].NewRow();
                    drDataRow["PAYROLL_TYPE_DESC"] = "Time Attendance";
                    pvtDataSet.Tables["PayrollType"].Rows.Add(drDataRow);
                }
                else
                {
                    DataRow drDataRow = pvtDataSet.Tables["PayrollType"].NewRow();
                    drDataRow["PAYROLL_TYPE_DESC"] = "Wages";
                    pvtDataSet.Tables["PayrollType"].Rows.Add(drDataRow);

                    drDataRow = pvtDataSet.Tables["PayrollType"].NewRow();
                    drDataRow["PAYROLL_TYPE_DESC"] = "Salaries";
                    pvtDataSet.Tables["PayrollType"].Rows.Add(drDataRow);
                }
                
                pvtDataSet.AcceptChanges();
                
                //2017-10-11
                pvtPublicHolidayDataView = new DataView(pvtDataSet.Tables["PublicHoliday"],
                        "",
                        "PUBLIC_HOLIDAY_DATE",
                        DataViewRowState.CurrentRows);
                
                for (int intRow = 0; intRow < pvtDataSet.Tables["PayrollType"].Rows.Count; intRow++)
                {
                    this.dgvPayrollTypeDataGridView.Rows.Add(pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE_DESC"].ToString());
                }

                this.pvtblnPayrollTypeDataGridViewLoaded = true;

                this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView, 0);
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void Date_DataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            DataGridView myDataGridView = (DataGridView)sender;

            if (double.Parse(myDataGridView[pvtintDateSortColDayDataGridView, e.RowIndex1].Value.ToString().Replace("-", "")) > double.Parse(myDataGridView[pvtintDateSortColDayDataGridView, e.RowIndex2].Value.ToString().Replace("-", "")))
            {
                e.SortResult = 1;
            }
            else
            {
                if (double.Parse(myDataGridView[pvtintDateSortColDayDataGridView, e.RowIndex1].Value.ToString().Replace("-", "")) < double.Parse(myDataGridView[pvtintDateSortColDayDataGridView, e.RowIndex2].Value.ToString().Replace("-", "")))
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
            this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
            this.Clear_DataGridView(this.dgvDayDataGridView);
            this.Clear_DataGridView(this.dgvEmployeeDataGridView);
            this.Clear_DataGridView(this.dgvEmployeeChosenDataGridView);
            this.Clear_DataGridView(this.dgvEmployeeRejectedDataGridView);
            this.Clear_DataGridView(this.dgvEmployeeChosenRejectedDataGridView);

            pvtdtDateTimeTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(5);

            this.btnDelete.Enabled = false;
            this.btnUpdate.Enabled = false;
            
            pvtPayCategoryDataView = null;
            pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"],
                "PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                "PAY_CATEGORY_DESC",
                DataViewRowState.CurrentRows);

            this.pvtblnPayCategoryDataGridViewLoaded = false;

            string strDateLastUploaded = "";
            string strDateTimeSort = "";

            for (int intRow = 0; intRow < pvtPayCategoryDataView.Count; intRow++)
            {
                if (pvtPayCategoryDataView[intRow]["LAST_UPLOAD_DATETIME"] != System.DBNull.Value)
                {
                    strDateLastUploaded = Convert.ToDateTime(pvtPayCategoryDataView[intRow]["LAST_UPLOAD_DATETIME"]).ToString("dd MMMM yyyy - HH:mm");
                    strDateTimeSort = Convert.ToDateTime(pvtPayCategoryDataView[intRow]["LAST_UPLOAD_DATETIME"]).ToString("yyyyMMddHHmm");
                }
                else
                {
                    strDateLastUploaded = "";
                    strDateTimeSort = "";
                }

                this.dgvPayCategoryDataGridView.Rows.Add("",
                                                         "",
                                                         "",
                                                         "",
                                                         pvtPayCategoryDataView[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                         strDateLastUploaded,
                                                         intRow.ToString(),
                                                         strDateTimeSort);
                
                if ((pvtPayCategoryDataView[intRow]["WAGE_RUN_IND"] != System.DBNull.Value
                    & pvtstrPayrollType == "W")
                    | (pvtPayCategoryDataView[intRow]["SALARY_RUN_IND"] != System.DBNull.Value
                    & pvtstrPayrollType == "S")
                    | (pvtPayCategoryDataView[intRow]["TIME_ATTENDANCE_RUN_IND"] != System.DBNull.Value
                    & pvtstrPayrollType == "T"))
                {
                    if ((pvtPayCategoryDataView[intRow]["WAGE_RUN_IND"].ToString() == "Y"
                    & pvtstrPayrollType == "W")
                    | (pvtPayCategoryDataView[intRow]["SALARY_RUN_IND"].ToString() == "Y"
                    & pvtstrPayrollType == "S")
                    | (pvtPayCategoryDataView[intRow]["TIME_ATTENDANCE_RUN_IND"].ToString() == "Y"
                    & pvtstrPayrollType == "T"))
                    {
                        this.dgvPayCategoryDataGridView[0,this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = PayrollRunDataGridViewCellStyle;
                    }
                    else
                    {
                        if (pvtPayCategoryDataView[intRow]["AUTHORISED_IND"].ToString() == "Y")
                        {
                            this.dgvPayCategoryDataGridView[0,this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = AuthorisedDataGridViewCellStyle;
                        }
                        else
                        {
                            this.dgvPayCategoryDataGridView[0,this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = PayrollRunPendingDataGridViewCellStyle;
                        }
                    }
                }
            }

            this.pvtblnPayCategoryDataGridViewLoaded = true;

            if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, 0);
            }
            else
            {
                this.btnUpdate.Enabled = false;
                this.btnDelete.Enabled = false;
            }
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
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

                    pvtintPayrollTypeRowIndex = -1;
                    break;

                case "dgvPayCategoryDataGridView":

                    pvtintPayCategoryDataGridViewRowIndex = -1;
                    break;

                case "dgvEmployeeDataGridView":

                    pvtintEmployeeDataGridViewRowIndex = -1;
                    break;

                case "dgvEmployeeChosenDataGridView":

                    pvtintEmployeeChosenDataGridViewRowIndex = -1;
                    break;

                case "dgvDayDataGridView":

                    pvtintDayDataGridViewRowIndex = -1;
                    break;

                case "dgvDayChosenDataGridView":

                    pvtintDayChosenDataGridViewRowIndex = -1;
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

                    case "dgvEmployeeChosenDataGridView":

                        this.dgvEmployeeChosenDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvDayDataGridView":

                        this.dgvDayDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvDayChosenDataGridView":

                        this.dgvDayChosenDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEmployeeRejectedDataGridView":

                        this.dgvEmployeeRejectedDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEmployeeChosenRejectedDataGridView":

                        this.dgvEmployeeChosenRejectedDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        public void btnCancel_Click(object sender, System.EventArgs e)
        {
            if (this.Text.IndexOf("- Update") > -1)
            {
                this.Text = this.Text.Substring(0, this.Text.IndexOf("- Update") - 1);
            }

            this.rbnTimesheet.Enabled = true;
            this.rbnBreak.Enabled = true;

            Remove_Selected_Dates();

            grbOption.Text = "Option";
          
            this.lblEmployeeRejected.Text = "";
            this.lblEmployeeChosenRejected.Text = "";

            this.dgvPayrollTypeDataGridView.Enabled = true;
            this.dgvPayCategoryDataGridView.Enabled = true;

            this.picPayrollTypeLock.Visible = false;
            this.picPayCategoryLock.Visible = false;

            this.btnDelete.Enabled = true;
            this.btnUpdate.Enabled = true;
            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.rbnStart.Enabled = true;
            this.rbnStop.Enabled = true;
            this.rbnBoth.Enabled = true;
            this.rbnDeleteRow.Enabled = true;

            if (this.rbnDeleteRow.Checked == false)
            {
                this.chkUpdate.Enabled = true;
            }

            this.btnAdd.Enabled = false;
            this.btnAddAll.Enabled = false;
            this.btnRemove.Enabled = false;
            this.btnRemoveAll.Enabled = false;

            this.txtIn.Text = "";
            this.txtOut.Text = "";

            this.txtIn.Enabled = false;
            this.txtOut.Enabled = false;

            this.picDayLock.Visible = false;
            this.dgvDayDataGridView.Enabled = true;

            pvtDataSet.RejectChanges();

            //Fire PayCategory so that Dates will Reload on Start and Stop Option
            if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView));
            }
        }

        public void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                int intReturnCode = 0;
                int intTimeInMinutes = 0;
                int intTimeOutMinutes = 0;

                if (this.txtIn.Enabled == true)
                {
                    intReturnCode = Check_Time(this.txtIn.Text);

                    if (intReturnCode != 0)
                    {
                        if (intReturnCode != 0)
                        {
                            CustomMessageBox.Show("Invalid Start Time.\n\nTime Format = 'hhmm'\nwhere 'hh' = hours (Max 24 hours)\n'mm' = minutes (Max 59 minutes)", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        this.txtIn.Focus();

                        return;
                    }
                    else
                    {
                        intTimeInMinutes = Return_Time_In_Minutes(this.txtIn.Text);
                    }
                }

                if (this.txtOut.Enabled == true)
                {
                    intReturnCode = Check_Time(this.txtOut.Text);

                    if (intReturnCode != 0)
                    {
                        if (intReturnCode != 0)
                        {
                            CustomMessageBox.Show("Invalid End Time.\n\nTime Format = 'hhmm'\nwhere 'hh' = hours (Max 24 hours)\n'mm' = minutes (Max 59 minutes)", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        this.txtOut.Focus();

                        return;
                    }
                    else
                    {
                        intTimeOutMinutes = Return_Time_In_Minutes(this.txtOut.Text);
                    }
                }

                if (this.txtIn.Enabled == true
                    & this.txtOut.Enabled == true)
                {
                    if (Convert.ToInt32(this.txtIn.Text) >= Convert.ToInt32(this.txtOut.Text))
                    {
                        CustomMessageBox.Show("End Time must be Greater than Start Time.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                string strOption = "";

                if (rbnStart.Checked == true)
                {
                    strOption = "I";
                }
                else
                {
                    if (rbnStop.Checked == true)
                    {
                        strOption = "O";
                    }
                    else
                    {
                        if (rbnBoth.Checked == true)
                        {
                            strOption = "B";
                        }
                        else
                        {
                            strOption = "D";
                        }
                    }
                }

                string strDates = "";

                if (this.rbnBoth.Checked == true
                    & this.chkUpdate.Checked == false)
                {
                    if (this.dgvDayChosenDataGridView.Rows.Count == 0)
                    {
                        CustomMessageBox.Show("Select Date/s.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        for (int intRow = 0; intRow < this.dgvDayChosenDataGridView.Rows.Count; intRow++)
                        {
                            if (intRow == 0)
                            {
                                strDates = this.dgvDayChosenDataGridView[pvtintDateSortColDayDataGridView, intRow].Value.ToString();
                            }
                            else
                            {
                                strDates += "|" + this.dgvDayChosenDataGridView[pvtintDateSortColDayDataGridView, intRow].Value.ToString();
                            }
                        }
                    }
                }
                else
                {
                    strDates = this.dgvDayDataGridView[pvtintDateSortColDayDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvDayDataGridView)].Value.ToString();
                }

                string strEmployeeNos = "";
                string strEmployeeSeqNos = "";

                if (this.dgvEmployeeChosenDataGridView.Rows.Count == 0)
                {
                    CustomMessageBox.Show("Select Employee/s.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    for (int intRow = 0; intRow < this.dgvEmployeeChosenDataGridView.Rows.Count; intRow++)
                    {
                        if (this.rbnStop.Checked == true)
                        {
                            if (Convert.ToInt32(this.txtOut.Text) <= Convert.ToInt32(this.dgvEmployeeChosenDataGridView[pvtintStartColEmployeeDataGridView, intRow].Value))
                            {
                                dgvEmployeeChosenDataGridView.CurrentCell = dgvEmployeeChosenDataGridView[0, intRow];

                                CustomMessageBox.Show("End Value must be Greater than Employee's Start Value.\n\nAction Cancelled.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

                                return;
                            }
                        }
                        else
                        {
                            if (this.rbnStart.Checked == true
                                & this.chkUpdate.Checked == true)
                            {
                                //Stop Value
                                if (this.dgvEmployeeChosenDataGridView[pvtintEndColEmployeeDataGridView, intRow].Value.ToString() != "")
                                {
                                    if (Convert.ToInt32(this.txtIn.Text) >= Convert.ToInt32(this.dgvEmployeeChosenDataGridView[pvtintEndColEmployeeDataGridView, intRow].Value))
                                    {
                                        dgvEmployeeChosenDataGridView.CurrentCell = dgvEmployeeChosenDataGridView[0, intRow];

                                        CustomMessageBox.Show("Start Value must be Less than Employee's End Value.\n\nAction Cancelled.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

                                        return;
                                    }
                                }
                            }
                        }

                        if (intRow == 0)
                        {
                            strEmployeeNos = this.dgvEmployeeChosenDataGridView[pvtintEmployeeNoColEmployeeDataGridView, intRow].Value.ToString();
                            strEmployeeSeqNos = this.dgvEmployeeChosenDataGridView[pvtintEmployeeSeqNoColEmployeeDataGridView, intRow].Value.ToString();
                        }
                        else
                        {
                            strEmployeeNos += "|" + this.dgvEmployeeChosenDataGridView[pvtintEmployeeNoColEmployeeDataGridView, intRow].Value.ToString();
                            strEmployeeSeqNos += "|" + this.dgvEmployeeChosenDataGridView[pvtintEmployeeSeqNoColEmployeeDataGridView, intRow].Value.ToString();
                        }
                    }
                }

                string strUpdateInd = "N";

                if (this.chkUpdate.Checked == true)
                {
                    strUpdateInd = "Y";
                }

                string strRecordType = "T";

                if (this.rbnBreak.Checked == true)
                {
                    strRecordType = "B";
                }

                pvtDataSet.Tables["PayCategoryLoaded"].Rows.Clear();
                pvtDataSet.Tables["FromDate"].Rows.Clear();
                pvtDataSet.Tables["Timesheet"].Rows.Clear();
                pvtDataSet.Tables["Break"].Rows.Clear();

                object[] objParm = new object[14];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = pvtintPayCategoryNo;
                objParm[2] = pvtstrPayrollType;
                objParm[3] = strDates;
                objParm[4] = strOption;
                objParm[5] = strEmployeeNos;
                objParm[6] = strEmployeeSeqNos;
                objParm[7] = intTimeInMinutes;
                objParm[8] = intTimeOutMinutes;
                objParm[9] = strUpdateInd;
                objParm[10] = strRecordType;
                objParm[11] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[12] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[13] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Insert_Update_Clocking_Records", objParm,true);

                DataSet TempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                this.pvtDataSet.Merge(TempDataSet);

                btnCancel_Click(sender, e);
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void chkUpdate_Click(object sender, EventArgs e)
        {
            if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView));
            }
        }

        public void btnDelete_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strMessage = "Delete ALL Timesheet Records for Cost Centre '" + this.dgvPayCategoryDataGridView[pvtintPayCategoryDescColPayCategoryDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView)].Value.ToString() + "' for the Date '" + pvtdtDayDateTime.ToString("dd MMMM yyyy") + "'.";

                if (this.rbnBreak.Checked == true)
                {
                    strMessage = strMessage.Replace("Timesheet", "Break");
                }

                DialogResult dlgResult = CustomMessageBox.Show(strMessage,
                       this.Text,
                       MessageBoxButtons.YesNo,
                       MessageBoxIcon.Warning);

                if (dlgResult == DialogResult.Yes)
                {
                    string strDates =  this.dgvDayDataGridView[pvtintDateSortColDayDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvDayDataGridView)].Value.ToString();
                   
                    string strRecordType = "T";

                    if (this.rbnBreak.Checked == true)
                    {
                        strRecordType = "B";
                    }

                    object[] objParm = new object[7];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = pvtintPayCategoryNo;
                    objParm[2] = pvtstrPayrollType;
                    objParm[3] = strDates;
                    objParm[4] = strRecordType;
                    objParm[5] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                    objParm[6] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                   
                    clsISUtilities.DynamicFunction("Delete_TimeSheet_Day_Records", objParm,true);

                    DataView myDeleteDataView = new DataView(pvtDataSet.Tables[pvtstrTableName],
                            "PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo.ToString() + " AND " + pvtstrTableDef + "_DATE = '" + pvtdtDayDateTime.ToString("yyyy-MM-dd") + "'",
                            "EMPLOYEE_NO",
                            DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < myDeleteDataView.Count; intRow++)
                    {
                        myDeleteDataView[intRow].Delete();

                        intRow -= 1;
                    }

                    pvtDataSet.AcceptChanges();
                 
                    btnCancel_Click(sender, e);
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private int Check_Time(string strTime)
        {
            int intReturnCode = 0;

            int intHH = 0;
            int intMM = 0;

            if (strTime.Length > 2)
            {
                intHH = Convert.ToInt32(strTime.Substring(0, strTime.Length - 2));
                intMM = Convert.ToInt32(strTime.Substring(strTime.Length - 2, 2));
            }
            else
            {
                if (strTime.Length == 0)
                {
                    intReturnCode = 1;
                }
                else
                {
                    intMM = Convert.ToInt32(strTime);
                }
            }

            if (intHH >= 24)
            {
                if (intHH == 24)
                {
                    if (intMM != 0)
                    {
                        intReturnCode = 1;
                    }
                }
                else
                {
                    intReturnCode = 1;
                }
            }

            if (intMM > 59)
            {
                intReturnCode = 1;
            }

            return intReturnCode;
        }

        private void ExceptionDate_Click(object sender, EventArgs e)
        {
            RadioButton myRadioButton = (RadioButton)sender;
            
            if (myRadioButton.Name == "rbnListDates")
            {
                if (this.dgvDayDataGridView.Rows.Count > 0)
                {
                    this.lblEmployeeRejected.Text = "";
                    this.lblEmployeeChosenRejected.Text = "";

                    this.Clear_DataGridView(this.dgvEmployeeRejectedDataGridView);
                    this.Clear_DataGridView(this.dgvEmployeeChosenRejectedDataGridView);

                    this.Set_DataGridView_SelectedRowIndex(this.dgvDayDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView));
                }
                else
                {
                    this.rbnSelectedDates.Checked = true;

                    return;
                }
            }
            else
            {
                if (this.dgvDayChosenDataGridView.Rows.Count > 0)
                {
                    this.lblEmployeeRejected.Text = "";
                    this.lblEmployeeChosenRejected.Text = "";

                    this.Clear_DataGridView(this.dgvEmployeeRejectedDataGridView);
                    this.Clear_DataGridView(this.dgvEmployeeChosenRejectedDataGridView);

                    this.Set_DataGridView_SelectedRowIndex(this.dgvDayChosenDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayChosenDataGridView));
                }
                else
                {
                    this.rbnListDates.Checked = true;

                    return;
                }
            }

            int intFindRow = -1;

            pvtEmployeeRejectedDataView = null;
            pvtEmployeeRejectedDataView = new DataView(pvtDataSet.Tables[pvtstrTableName],
                    "PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo.ToString() + " AND " + pvtstrTableDef + "_DATE = '" + pvtdtDayDateTime.ToString("yyyy-MM-dd") + "'",
                    "EMPLOYEE_NO",
                    DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < this.dgvEmployeeDataGridView.Rows.Count; intRow++)
            {
                intFindRow = pvtEmployeeRejectedDataView.Find(dgvEmployeeDataGridView[pvtintEmployeeNoColEmployeeDataGridView, intRow].Value.ToString());

                if (intFindRow > -1)
                {
                    this.dgvEmployeeDataGridView[1, intRow].Style = RejectedDataGridViewCellStyle;
                }
                else
                {
                    this.dgvEmployeeDataGridView[1, intRow].Style = NormalDataGridViewCellStyle;
                }
            }

            for (int intRow = 0; intRow < this.dgvEmployeeChosenDataGridView.Rows.Count; intRow++)
            {
                intFindRow = pvtEmployeeRejectedDataView.Find(dgvEmployeeChosenDataGridView[pvtintEmployeeNoColEmployeeDataGridView, intRow].Value.ToString());

                if (intFindRow > -1)
                {
                    this.dgvEmployeeChosenDataGridView[1, intRow].Style = RejectedDataGridViewCellStyle;
                }
                else
                {
                    this.dgvEmployeeChosenDataGridView[1, intRow].Style = NormalDataGridViewCellStyle;
                }
            }

            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView));
            }
           
            if (this.dgvEmployeeChosenDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvEmployeeChosenDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeChosenDataGridView));
            }
        }

        private int Return_Time_In_Minutes(string strTime)
        {
            int intTimeInMinutes = 0;
            int intHH = 0;
            int intMM = 0;

            if (strTime.Length > 2)
            {
                intHH = Convert.ToInt32(strTime.Substring(0, strTime.Length - 2));
                intMM = Convert.ToInt32(strTime.Substring(strTime.Length - 2, 2));
            }
            else
            {
                intMM = Convert.ToInt32(strTime);
            }

            intTimeInMinutes = (intHH * 60) + intMM;

            return intTimeInMinutes;
        }
       
        private void rbnStart_Click(object sender, EventArgs e)
        {
            this.tmrOptionTimer.Enabled = false;
            
            this.chkUpdate.Checked = false;
            this.chkUpdate.Enabled = true;

            if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView));
            }
        }

        private void Remove_Selected_Dates()
        {
            this.btnDateAdd.Enabled = false;
            this.btnDateRemove.Enabled = false;

            this.lblChosenDates.Text = "";
            this.dgvDayChosenDataGridView.Rows.Clear();
            
            this.grbDateException.Visible = false;

            this.rbnListDates.Checked = true;

            this.lblDate.Text = "Selected Date";
        }

        private void rbnStop_Click(object sender, EventArgs e)
        {
            this.chkUpdate.Checked = false;
            this.chkUpdate.Enabled = true;

            if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView));
            }
        }

        private void rbnBoth_Click(object sender, EventArgs e)
        {
            this.chkUpdate.Checked = false;
            this.chkUpdate.Enabled = true;

            if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView));
            }
        }

        private void Add_Selected_Dates()
        {
            this.btnDateAdd.Enabled = true;
            this.btnDateRemove.Enabled = true;

            this.lblChosenDates.Text = "Selected Dates";

            this.Clear_DataGridView(dgvDayChosenDataGridView);

            this.dgvDayChosenDataGridView.Visible = true;

            this.grbDateException.Visible = true;

            this.lblDate.Text = "List of Dates";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                bool blnAddEmployee = true;

                if (this.rbnDeleteRow.Checked == false)
                {
                    for (int intRow = 0; intRow < this.dgvEmployeeChosenDataGridView.Rows.Count; intRow++)
                    {
                        if (this.dgvEmployeeDataGridView[pvtintEmployeeNoColEmployeeDataGridView, Get_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView)].Value.ToString() == this.dgvEmployeeChosenDataGridView[pvtintEmployeeNoColEmployeeDataGridView, intRow].Value.ToString())
                        {
                            if (pvtblnAddAll == false)
                            {
                                CustomMessageBox.Show("A record for this Employee has already been selected", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }

                            blnAddEmployee = false;

                            break;
                        }
                    }
                }

                if (blnAddEmployee == true)
                {
                    pvtintEmployeeChosenDataGridViewRowIndex = -1;

                    DataGridViewRow myDataGridViewRow = this.dgvEmployeeDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView)];

                    this.dgvEmployeeDataGridView.Rows.Remove(myDataGridViewRow);

                    this.dgvEmployeeChosenDataGridView.Rows.Add(myDataGridViewRow);

                    this.dgvEmployeeChosenDataGridView.CurrentCell = this.dgvEmployeeChosenDataGridView[0, this.dgvEmployeeChosenDataGridView.Rows.Count - 1];

                    if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView, 0);
                    }
                    else
                    {
                        Clear_DataGridView(this.dgvEmployeeRejectedDataGridView);
                        this.lblEmployeeRejected.Text = "";
                    }
                }
                else
                {
                    Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView));
                }
            }
        }

        private void btnAddAll_Click(object sender, EventArgs e)
        {
            pvtblnAddAll = true;
            bool blnDuplicates = false;
            bool blnExists = false;
            int intMainEmployeeNo = -1;

            for (int intRow = 0; intRow < this.dgvEmployeeDataGridView.Rows.Count; intRow++)
            {
                blnExists = false;
                intMainEmployeeNo = Convert.ToInt32(this.dgvEmployeeDataGridView[pvtintEmployeeNoColEmployeeDataGridView, intRow].Value);

                if (this.rbnDeleteRow.Checked == false)
                {
                    for (int intCompareRow = 0; intCompareRow < this.dgvEmployeeDataGridView.Rows.Count; intCompareRow++)
                    {
                        if (intCompareRow == intRow)
                        {
                            continue;
                        }

                        if (intMainEmployeeNo == Convert.ToInt32(this.dgvEmployeeDataGridView[pvtintEmployeeNoColEmployeeDataGridView, intCompareRow].Value))
                        {
                            blnExists = true;
                            break;
                        }
                    }
                }

                if (blnExists == true)
                {
                    blnDuplicates = true;
                    continue;
                }

                dgvEmployeeDataGridView.CurrentCell = dgvEmployeeDataGridView[0, intRow];
                btnAdd_Click(null, null);
                intRow -= 1;
            }

            if (blnDuplicates == true)
            {
                CustomMessageBox.Show("Employee/s with 2 or More Records Exist.\nThese Records have been ignored.\n\nYou may manually select 1 of these Records.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            pvtblnAddAll = false;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (this.dgvEmployeeChosenDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvEmployeeChosenDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvEmployeeChosenDataGridView)];

                this.dgvEmployeeChosenDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvEmployeeDataGridView.Rows.Add(myDataGridViewRow);

                if (this.dgvEmployeeChosenDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvEmployeeChosenDataGridView, 0);
                }
                else
                {
                    Clear_DataGridView(this.dgvEmployeeChosenRejectedDataGridView);
                    this.lblEmployeeChosenRejected.Text = "";
                }

                this.Set_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1);
            }
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
        btnRemoveAll_Click_Continue:

            if (this.dgvEmployeeChosenDataGridView.Rows.Count > 0)
            {
                btnRemove_Click(null, null);

                goto btnRemoveAll_Click_Continue;
            }
        }

        public void btnUpdate_Click(object sender, EventArgs e)
        {
            this.Text += " - Update";

            this.dgvPayrollTypeDataGridView.Enabled = false;
            this.dgvPayCategoryDataGridView.Enabled = false;

            this.rbnTimesheet.Enabled = false;
            this.rbnBreak.Enabled = false;
            
            this.picPayCategoryLock.Visible = true;
            this.picPayrollTypeLock.Visible = true;

            this.btnDelete.Enabled = false;
            this.btnUpdate.Enabled = false;
            this.btnCancel.Enabled = true;

            this.rbnStart.Enabled = false;
            this.rbnStop.Enabled = false;
            this.rbnBoth.Enabled = false;
            this.rbnDeleteRow.Enabled = false;
            this.chkUpdate.Enabled = false;

            this.btnAdd.Enabled = true;
            this.btnAddAll.Enabled = true;
            this.btnRemove.Enabled = true;
            this.btnRemoveAll.Enabled = true;

            if (this.rbnStart.Checked == true)
            {
                this.txtIn.Enabled = true;

                this.picDayLock.Visible = true;
                this.dgvDayDataGridView.Enabled = false;

                this.txtIn.Focus();
            }
            else
            {
                if (this.rbnStop.Checked == true)
                {
                    this.txtOut.Enabled = true;

                    this.picDayLock.Visible = true;
                    this.dgvDayDataGridView.Enabled = false;

                    this.txtOut.Focus();
                }
                else
                {
                    if (this.rbnBoth.Checked == true)
                    {
                        this.txtIn.Enabled = true;
                        this.txtOut.Enabled = true;

                        if (this.chkUpdate.Checked == false)
                        {
                            Add_Selected_Dates();
                        }
                        else
                        {
                            this.picDayLock.Visible = true;
                            this.dgvDayDataGridView.Enabled = false;
                        }

                        this.txtIn.Focus();
                    }
                    else
                    {
                        this.picDayLock.Visible = true;
                        this.dgvDayDataGridView.Enabled = false;
                    }
                }
            }

            this.btnSave.Enabled = true;
        }

        private void Text_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox myTextBox = (TextBox)sender;

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
                                    if (strNewTextField.Length > 3)
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

        private void rbnDeleteRow_Click(object sender, EventArgs e)
        {
            this.chkUpdate.Checked = false;
            this.chkUpdate.Enabled = false;

            if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView));
            }
        }

        private void tmrOptionTimer_Tick(object sender, EventArgs e)
        {
            if (pvtintTimerCount == 3)
            {
                this.tmrOptionTimer.Enabled = false;
            }
            else
            {
                if (this.pnlBreak.Visible == true)
                {
                    this.pnlBreak.Visible = false;
                }
                else
                {
                    pvtintTimerCount += 1;
                    this.pnlBreak.Visible = true;
                }
            }
        }

        private void RecordType_Click(object sender, EventArgs e)
        {
            RadioButton myRadioButton = (RadioButton)sender;

            this.chkUpdate.Checked = false;
            this.rbnStart.Checked = true;
            this.chkUpdate.Enabled = true;

            if (myRadioButton.Name == "rbnTimesheet")
            {
                pvtstrTableDef = "TIMESHEET";
                pvtstrTableName = "Timesheet";
                this.pnlBreak.Visible = false;

                this.lblEmployee.Text = this.lblEmployee.Text.Replace("Break", "Timesheet");
                this.lblChosenEmployee.Text = this.lblChosenEmployee.Text.Replace("Break", "Timesheet");
                this.lblEmployeeRejected.Text = this.lblEmployeeRejected.Text.Replace("Break", "Timesheet");
            }
            else
            {
                pvtstrTableDef = "BREAK";
                pvtstrTableName = "Break";
                this.pnlBreak.Visible = true;

                this.lblEmployee.Text = this.lblEmployee.Text.Replace("Timesheet", "Break");
                this.lblChosenEmployee.Text = this.lblChosenEmployee.Text.Replace("Timesheet", "Break");
                this.lblEmployeeRejected.Text = this.lblEmployeeRejected.Text.Replace("Timesheet", "Break");

                pvtintTimerCount = 0;
                this.tmrOptionTimer.Enabled = true;
            }

            if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView));
            }
        }

        private void TimeField_Leave(object sender, EventArgs e)
        {
            TextBox myTextBox = (TextBox)sender;

            if (myTextBox.Text == ""
            | myTextBox.Text == "2400")
            {
            }
            else
            {
                if (myTextBox.Text.Length > 2)
                {
                    if (Convert.ToInt32(myTextBox.Text.Substring(0, myTextBox.Text.Length - 2)) > 23
                        | Convert.ToInt32(myTextBox.Text.Substring(myTextBox.Text.Length - 2, 2)) > 59)
                    {
                        myTextBox.Text = "";
                        System.Console.Beep();
                    }
                }
                else
                {
                    if (Convert.ToInt32(myTextBox.Text) > 59)
                    {
                        myTextBox.Text = "";
                        System.Console.Beep();
                    }
                }
            }
        }

        private void dgvPayrollTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (this.pvtblnPayrollTypeDataGridViewLoaded == true)
            {
                if (pvtintPayrollTypeRowIndex != e.RowIndex)
                {
                    pvtintPayrollTypeRowIndex = e.RowIndex;

                    this.chkUpdate.Checked = false;
                    this.rbnStart.Checked = true;
                    this.chkUpdate.Enabled = true;

                    //Put Here to Stop Events Firing 
                    this.Clear_DataGridView(this.dgvPayCategoryDataGridView);

                    if (pvtstrPayrollType != this.dgvPayrollTypeDataGridView[0, e.RowIndex].Value.ToString().Substring(0, 1))
                    {
                        pvtstrPayrollType = this.dgvPayrollTypeDataGridView[0, e.RowIndex].Value.ToString().Substring(0, 1);

                        Load_CurrentForm_Records();
                    }
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
                        
                        pvtintPayCategoryTableRowNo = Convert.ToInt32(this.dgvPayCategoryDataGridView[pvtintRowColPayCategoryDataGridView, e.RowIndex].Value);
                        pvtintPayCategoryNo = Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_CATEGORY_NO"]);

                        for (int intRow = 0; intRow < dgvPayCategoryDataGridView.Rows.Count; intRow++)
                        {
                            this.dgvPayCategoryDataGridView[1, intRow].Style = NormalDataGridViewCellStyle;
                            this.dgvPayCategoryDataGridView[2, intRow].Style = NormalDataGridViewCellStyle;
                            this.dgvPayCategoryDataGridView[3, intRow].Style = NormalDataGridViewCellStyle;
                        }

                        int intSelectedDayRow = 0;

                        this.Clear_DataGridView(this.dgvDayDataGridView);
                        this.Clear_DataGridView(this.dgvDayChosenDataGridView);
                        this.Clear_DataGridView(this.dgvEmployeeDataGridView);
                        this.Clear_DataGridView(this.dgvEmployeeChosenDataGridView);
                        this.Clear_DataGridView(this.dgvEmployeeRejectedDataGridView);
                        this.Clear_DataGridView(this.dgvEmployeeChosenRejectedDataGridView);

                        this.lblEmployeeRejected.Text = "";
                        this.lblEmployeeChosenRejected.Text = "";

                        DataView myPayCategoryDateDataView = new DataView(pvtDataSet.Tables["PayCategoryLoaded"],
                                                                            "PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                                                                            "",
                                                                            DataViewRowState.CurrentRows);

                        if (myPayCategoryDateDataView.Count == 0)
                        {
                            if (pvtintPayCategoryNoSaved == pvtintPayCategoryNo
                            & pvtstrPayrollTypeSaved == pvtstrPayrollType)
                            {
                                //This stops This Event Firing 2 times and Causing controld to Be Enabled = false
                                return;
                            }
                            else
                            {
                                pvtintPayCategoryNoSaved = pvtintPayCategoryNo;
                                pvtstrPayrollTypeSaved = pvtstrPayrollType;

                                pvtDataSet.Tables["PayCategoryLoaded"].Rows.Clear();
                                pvtDataSet.Tables["FromDate"].Rows.Clear();
                                pvtDataSet.Tables["Timesheet"].Rows.Clear();
                                pvtDataSet.Tables["Break"].Rows.Clear();

                                object[] objParm = new object[6];
                                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                                objParm[1] = pvtintPayCategoryNo;
                                objParm[2] = pvtstrPayrollType;
                                objParm[3] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("UserNo"));
                                objParm[4] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                                objParm[5] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();

                                byte[] bytCompress = (byte[])this.clsISUtilities.DynamicFunction("Get_PayCategory_Records", objParm, false);
                                DataSet TempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);

                                pvtDataSet.Merge(TempDataSet);
                            }
                        }

                        pvtPayCategoryFromDateDataView = null;
                        pvtPayCategoryFromDateDataView = new DataView(pvtDataSet.Tables["FromDate"],
                                                                                "PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                                                                                "",
                                                                                DataViewRowState.CurrentRows);

                        string strFilter = "";
                        int intFindRow = -1;

                        if (this.rbnDeleteRow.Checked == true
                            | this.chkUpdate.Checked == true)
                        {
                            pvtTimesheetOrBreakDataView = null;
                            pvtTimesheetOrBreakDataView = new DataView(pvtDataSet.Tables[pvtstrTableName],
                                "PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo,
                                pvtstrTableDef + "_DATE",
                                DataViewRowState.CurrentRows);

                            strFilter = " AND EMPLOYEE_NO = -1";
                        }
                        else
                        {
                            if (this.rbnStop.Checked == true)
                            {
                                pvtTimesheetOrBreakDataView = null;
                                pvtTimesheetOrBreakDataView = new DataView(pvtDataSet.Tables[pvtstrTableName],
                                    "PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND " + pvtstrTableDef + "_TIME_OUT_MINUTES IS NULL",
                                    pvtstrTableDef + "_DATE",
                                    DataViewRowState.CurrentRows);

                                strFilter = " AND NOT " + pvtstrTableDef + "_TIME_OUT_MINUTES IS NULL";
                            }
                        }

                        pvtEmployeeRejectedDataView = null;
                        pvtEmployeeRejectedDataView = new DataView(pvtDataSet.Tables[pvtstrTableName],
                                "PAY_CATEGORY_NO = " + pvtintPayCategoryNo + strFilter,
                                    pvtstrTableDef + "_DATE",
                                DataViewRowState.CurrentRows);

                        if (pvtPayCategoryFromDateDataView.Count > 0)
                        {
                            DateTime dtDateTimeFrom = Convert.ToDateTime(pvtPayCategoryFromDateDataView[0]["FROM_DATE"]);

                            if (dtDateTimeFrom > pvtdtDateTimeTo)
                            {
                                CustomMessageBox.Show("From Date = " + dtDateTimeFrom.ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()) + " > To Date " + pvtdtDateTimeTo.ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()),this.Text,MessageBoxButtons.OK,MessageBoxIcon.Error);
                                return;
                            }
                            
                            pvtEmployeeLeaveDataView = null;
                            pvtEmployeeLeaveDataView = new DataView(pvtDataSet.Tables["EmployeeLeave"],
                                    "PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo.ToString() + " AND PAY_CATEGORY_TYPE = '" + this.pvtstrPayrollType + "'",
                                    "DAY_DATE",
                                    DataViewRowState.CurrentRows);

                            this.btnUpdate.Enabled = false;
                            this.btnDelete.Enabled = false;

                            DateTime dtDateTime = pvtdtDateTimeTo;
                            string strLockType = "";

                            this.pvtblnDayDataGridViewLoaded = false;

                            while (dtDateTimeFrom <= dtDateTime)
                            {
                                if (this.rbnDeleteRow.Checked == true
                                | this.chkUpdate.Checked == true
                                | this.rbnStop.Checked == true)
                                {
                                    intFindRow = pvtTimesheetOrBreakDataView.Find(dtDateTime.ToString("yyyy-MM-dd"));

                                    if (intFindRow == -1)
                                    {
                                        goto dgvPayCategoryDataGridView_RowEnter_Date_Loop;
                                    }

                                    if (this.rbnStop.Checked == true)
                                    {
                                        for (int intRow = intFindRow; intRow < pvtTimesheetOrBreakDataView.Count; intRow++)
                                        {
                                            if (Convert.ToDateTime(pvtTimesheetOrBreakDataView[intRow][pvtstrTableDef + "_DATE"]).ToString("yyyy-MM-dd") != dtDateTime.ToString("yyyy-MM-dd"))
                                            {
                                                goto dgvPayCategoryDataGridView_RowEnter_Date_Loop;
                                            }
                                            else
                                            {
                                                //Find Employee 

                                                DataView myEmployeeTestDataView = new DataView(pvtDataSet.Tables[pvtstrTableName],
                                                "PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND EMPLOYEE_NO = " + pvtTimesheetOrBreakDataView[intRow]["EMPLOYEE_NO"].ToString() + " AND " + pvtstrTableDef + "_DATE = '" + dtDateTime.ToString("yyyy-MM-dd") + "' AND NOT " + pvtstrTableDef + "_TIME_OUT_MINUTES IS NULL",
                                                "",
                                                DataViewRowState.CurrentRows);

                                                if (myEmployeeTestDataView.Count > 0)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }

                                strLockType = "";

                                if ((pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["WAGE_RUN_IND"] != System.DBNull.Value
                                & pvtstrPayrollType == "W")
                                | (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["SALARY_RUN_IND"] != System.DBNull.Value
                                & pvtstrPayrollType == "S")
                                | (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["TIME_ATTENDANCE_RUN_IND"] != System.DBNull.Value
                                & pvtstrPayrollType == "T"))
                                {
                                    if (dtDateTime >= Convert.ToDateTime(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_PERIOD_DATE_FROM"])
                                    & dtDateTime <= Convert.ToDateTime(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_PERIOD_DATE"]))
                                    {
                                        if ((pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["WAGE_RUN_IND"].ToString() == "Y"
                                        & pvtstrPayrollType == "W")
                                        | (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["SALARY_RUN_IND"].ToString() == "Y"
                                        & pvtstrPayrollType == "S")
                                        | (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["TIME_ATTENDANCE_RUN_IND"].ToString() == "Y"
                                        & pvtstrPayrollType == "T"))
                                        {
                                            strLockType = "Y";
                                        }
                                        else
                                        {
                                            if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["AUTHORISED_IND"].ToString() == "Y")
                                            {
                                                strLockType = "A";
                                            }
                                            else
                                            {
                                                strLockType = "P";
                                            }
                                        }
                                    }
                                }

                                this.dgvDayDataGridView.Rows.Add("",
                                                                 "",
                                                                 "",
                                                                 "",
                                                                 dtDateTime.ToString("dd MMM yyyy - ddd"),
                                                                 dtDateTime.ToString("yyyy-MM-dd"),
                                                                 strLockType);

                                if (dtDateTime.ToString("yyyyMMdd") == pvtdtDayDateTime.ToString("yyyyMMdd"))
                                {
                                    intSelectedDayRow = this.dgvDayDataGridView.Rows.Count - 1;
                                }

                                if (strLockType == "Y")
                                {
                                    this.dgvDayDataGridView[0,this.dgvDayDataGridView.Rows.Count - 1].Style = PayrollRunDataGridViewCellStyle;
                                }
                                else
                                {
                                    if (strLockType == "A")
                                    {
                                        this.dgvDayDataGridView[0,this.dgvDayDataGridView.Rows.Count - 1].Style = AuthorisedDataGridViewCellStyle;
                                    }
                                    else
                                    {
                                        if (strLockType == "P")
                                        {
                                            this.dgvDayDataGridView[0,this.dgvDayDataGridView.Rows.Count - 1].Style = PayrollRunPendingDataGridViewCellStyle;
                                        }
                                    }
                                }

                                intFindRow = pvtEmployeeRejectedDataView.Find(dtDateTime.ToString("yyyy-MM-dd"));

                                if (intFindRow > -1)
                                {
                                    this.dgvDayDataGridView[1, this.dgvDayDataGridView.Rows.Count - 1].Style = this.RejectedDataGridViewCellStyle;
                                    this.dgvPayCategoryDataGridView[1, e.RowIndex].Style = RejectedDataGridViewCellStyle;
                                }

                                //2017-10-11
                                intFindRow = pvtPublicHolidayDataView.Find(dtDateTime.ToString("yyyy-MM-dd"));

                                if (intFindRow > -1)
                                {
                                    this.dgvDayDataGridView[2, this.dgvDayDataGridView.Rows.Count - 1].Style = this.PublicHolidayDataGridViewCellStyle;
                                    this.dgvPayCategoryDataGridView[2, e.RowIndex].Style = PublicHolidayDataGridViewCellStyle;
                                }
                                
                                //2017-10-13
                                intFindRow = pvtEmployeeLeaveDataView.Find(dtDateTime.ToString("yyyy-MM-dd"));

                                if (intFindRow > -1)
                                {
                                    this.dgvDayDataGridView[3, this.dgvDayDataGridView.Rows.Count - 1].Style = this.LeaveDataGridViewCellStyle;
                                    this.dgvPayCategoryDataGridView[3, e.RowIndex].Style = LeaveDataGridViewCellStyle;
                                }

                            dgvPayCategoryDataGridView_RowEnter_Date_Loop:

                                dtDateTime = dtDateTime.AddDays(-1);
                            }

                            this.pvtblnDayDataGridViewLoaded = true;

                            if (this.dgvDayDataGridView.Rows.Count > 0)
                            {
                                this.Set_DataGridView_SelectedRowIndex(this.dgvDayDataGridView, intSelectedDayRow);
                            }
                            else
                            {
                                this.lblEmployee.Text = "List of Employee Timesheets";
                                this.lblChosenEmployee.Text = "Selected Employees Timesheets";
                            }
                        }
                        else
                        {
                            this.btnUpdate.Enabled = false;
                            this.btnDelete.Enabled = false;
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

                    this.Clear_DataGridView(this.dgvEmployeeRejectedDataGridView);

                    if (this.rbnDeleteRow.Checked == true
                    | this.chkUpdate.Checked == true)
                    {
                    }
                    else
                    {
                        int intEmployeeNo = Convert.ToInt32(dgvEmployeeDataGridView[pvtintEmployeeNoColEmployeeDataGridView, e.RowIndex].Value);

                        pvtEmployeeDataView = null;
                        pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                        "EMPLOYEE_NO = " + intEmployeeNo.ToString() + " AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                        "",
                        DataViewRowState.CurrentRows);

                        string strFilter = "";

                        if (this.rbnStop.Checked == true)
                        {
                            strFilter = " AND NOT " + pvtstrTableDef + "_TIME_OUT_MINUTES IS NULL";
                        }

                        pvtEmployeeRejectedDataView = null;
                        pvtEmployeeRejectedDataView = new DataView(pvtDataSet.Tables[pvtstrTableName],
                                "PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND EMPLOYEE_NO = " + intEmployeeNo.ToString() + " AND " + pvtstrTableDef + "_DATE = '" + pvtdtDayDateTime.ToString("yyyy-MM-dd") + "'" + strFilter,
                                "EMPLOYEE_NO",
                                DataViewRowState.CurrentRows);

                        if (pvtEmployeeRejectedDataView.Count > 0)
                        {
                            if (pvtEmployeeDataView.Count > 0)
                            {
                                int intHH = 0;
                                int intMM = 0;
                                string strIn = "";
                                string strOut = "";
                                string strClockIn = "";
                                string strClockOut = "";
                                
                                for (int intRow = 0; intRow < pvtEmployeeRejectedDataView.Count; intRow++)
                                {
                                    if (pvtEmployeeRejectedDataView[intRow]["CLOCKED_TIME_IN_MINUTES"] != System.DBNull.Value)
                                    {
                                        intHH = Convert.ToInt32(pvtEmployeeRejectedDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) / 60;
                                        intMM = Convert.ToInt32(pvtEmployeeRejectedDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) % 60;

                                        strClockIn = intHH.ToString() + intMM.ToString("00");
                                    }
                                    else
                                    {
                                        strClockIn = "";
                                    }

                                    if (pvtEmployeeRejectedDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"] != System.DBNull.Value)
                                    {
                                        intHH = Convert.ToInt32(pvtEmployeeRejectedDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) / 60;
                                        intMM = Convert.ToInt32(pvtEmployeeRejectedDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) % 60;

                                        strClockOut = intHH.ToString() + intMM.ToString("00");
                                    }
                                    else
                                    {
                                        strClockOut = "";
                                    }
                                    
                                    if (pvtEmployeeRejectedDataView[intRow][pvtstrTableDef + "_TIME_IN_MINUTES"] != System.DBNull.Value)
                                    {
                                        intHH = Convert.ToInt32(pvtEmployeeRejectedDataView[intRow][pvtstrTableDef + "_TIME_IN_MINUTES"]) / 60;
                                        intMM = Convert.ToInt32(pvtEmployeeRejectedDataView[intRow][pvtstrTableDef + "_TIME_IN_MINUTES"]) % 60;

                                        strIn = intHH.ToString() + intMM.ToString("00");
                                    }
                                    else
                                    {
                                        strIn = "";
                                    }

                                    if (pvtEmployeeRejectedDataView[intRow][pvtstrTableDef + "_TIME_OUT_MINUTES"] != System.DBNull.Value)
                                    {
                                        intHH = Convert.ToInt32(pvtEmployeeRejectedDataView[intRow][pvtstrTableDef + "_TIME_OUT_MINUTES"]) / 60;
                                        intMM = Convert.ToInt32(pvtEmployeeRejectedDataView[intRow][pvtstrTableDef + "_TIME_OUT_MINUTES"]) % 60;

                                        strOut = intHH.ToString() + intMM.ToString("00");
                                    }
                                    else
                                    {
                                        strOut = "";
                                    }
                                    
                                    this.dgvEmployeeRejectedDataGridView.Rows.Add("",
                                                                                  "",
                                                                                  "", 
                                                                                  "",
                                                                                  pvtEmployeeDataView[0]["EMPLOYEE_CODE"].ToString(),
                                                                                  pvtEmployeeDataView[0]["EMPLOYEE_SURNAME"].ToString(),
                                                                                  pvtEmployeeDataView[0]["EMPLOYEE_NAME"].ToString(),
                                                                                  strClockIn,
                                                                                  strIn,
                                                                                  strClockOut,
                                                                                  strOut);
                                    
                                    if (pvtstrLockType == "Y")
                                    {
                                        this.dgvEmployeeRejectedDataGridView[0, this.dgvEmployeeRejectedDataGridView.Rows.Count - 1].Style = PayrollRunDataGridViewCellStyle;
                                    }
                                    else
                                    {
                                        if (pvtstrLockType == "A")
                                        {
                                            this.dgvEmployeeRejectedDataGridView[0, this.dgvEmployeeRejectedDataGridView.Rows.Count - 1].Style = AuthorisedDataGridViewCellStyle;
                                        }
                                        else
                                        {
                                            if (pvtstrLockType == "P")
                                            {
                                                this.dgvEmployeeRejectedDataGridView[0, this.dgvEmployeeRejectedDataGridView.Rows.Count - 1].Style = PayrollRunPendingDataGridViewCellStyle;
                                            }
                                        }
                                    }
                                    
                                    this.dgvEmployeeRejectedDataGridView[1, this.dgvEmployeeRejectedDataGridView.Rows.Count - 1].Style = RejectedDataGridViewCellStyle;
                                    
                                    if (pvtblnPublicHoliday == true)
                                    {
                                        this.dgvEmployeeRejectedDataGridView[2, this.dgvEmployeeRejectedDataGridView.Rows.Count - 1].Style = this.PublicHolidayDataGridViewCellStyle;
                                    }

                                    pvtEmployeeLeaveDataView = null;
                                    pvtEmployeeLeaveDataView = new DataView(pvtDataSet.Tables["EmployeeLeave"],
                                            "PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo.ToString() + " AND PAY_CATEGORY_TYPE = '" + this.pvtstrPayrollType + "' AND EMPLOYEE_NO = " + intEmployeeNo.ToString() + " AND DAY_DATE = '" + pvtdtDayDateTime.ToString("yyyy-MM-dd") + "'",
                                            "EMPLOYEE_NO",
                                            DataViewRowState.CurrentRows);

                                    if (pvtEmployeeLeaveDataView.Count > 0)
                                    {
                                        this.dgvEmployeeRejectedDataGridView[3, this.dgvEmployeeRejectedDataGridView.Rows.Count - 1].Style = this.LeaveDataGridViewCellStyle;
                                    }
                                }

                                if (this.dgvEmployeeRejectedDataGridView.Rows.Count > 0)
                                {
                                    if (this.rbnSelectedDates.Checked == true)
                                    {
                                        this.lblEmployeeRejected.Text = pvtEmployeeDataView[0]["EMPLOYEE_NAME"].ToString() + " " + pvtEmployeeDataView[0]["EMPLOYEE_SURNAME"].ToString() + " - " + dgvDayChosenDataGridView[pvtintDateColDayDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayChosenDataGridView)].Value.ToString();
                                    }
                                    else
                                    {
                                        this.lblEmployeeRejected.Text = pvtEmployeeDataView[0]["EMPLOYEE_NAME"].ToString() + " " + pvtEmployeeDataView[0]["EMPLOYEE_SURNAME"].ToString() + " - " + dgvDayDataGridView[pvtintDateColDayDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Value.ToString();
                                    }

                                    this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeRejectedDataGridView, 0);
                                }
                                else
                                {
                                    this.lblEmployeeRejected.Text = "";
                                }
                            }
                            else
                            {
                                this.lblEmployeeRejected.Text = "";
                            }
                        }
                        else
                        {
                            this.lblEmployeeRejected.Text = "";
                        }
                    }
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

                    this.Cursor = Cursors.WaitCursor;
                    
                    pvtdtDayDateTime = DateTime.ParseExact(this.dgvDayDataGridView[pvtintDateSortColDayDataGridView, e.RowIndex].Value.ToString(), "yyyy-MM-dd", null);

                    //2017-10-11
                    int intFindRow = pvtPublicHolidayDataView.Find(pvtdtDayDateTime.ToString("yyyy-MM-dd"));

                    if (intFindRow > -1)
                    {
                        pvtblnPublicHoliday = true;
                    }
                    else
                    {
                        pvtblnPublicHoliday = false;

                    }
                    
                    pvtEmployeeLeaveDataView = null;
                    pvtEmployeeLeaveDataView = new DataView(pvtDataSet.Tables["EmployeeLeave"],
                            "PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo.ToString() + " AND PAY_CATEGORY_TYPE = '" + this.pvtstrPayrollType + "' AND DAY_DATE = '" + pvtdtDayDateTime.ToString("yyyy-MM-dd") + "'",
                            "EMPLOYEE_NO",
                            DataViewRowState.CurrentRows);
                    
                    pvtstrLockType = this.dgvDayDataGridView[pvtintLockColDayDataGridView, e.RowIndex].Value.ToString();
                    
                    int intHH = 0;
                    int intMM = 0;
                    intFindRow = -1;

                    this.lblEmployee.Text = "List of Employee Timesheets - " + pvtdtDayDateTime.ToString("dd MMM yyyy - ddd");
                    this.lblChosenEmployee.Text = "Selected Employees Timesheets - " + pvtdtDayDateTime.ToString("dd MMM yyyy - ddd");

                    if (this.btnSave.Enabled == false)
                    {
                        Clear_DataGridView(this.dgvEmployeeDataGridView);
                        Clear_DataGridView(this.dgvEmployeeChosenDataGridView);
                        Clear_DataGridView(this.dgvEmployeeRejectedDataGridView);
                        Clear_DataGridView(this.dgvEmployeeChosenRejectedDataGridView);
                    }
                    else
                    {
                        //In Edit Mode and Start and Stop Chosen
                        if (this.rbnListDates.Checked == true)
                        {
                            pvtblnEmployeeDataGridViewLoaded = false;
                            
                            pvtEmployeeRejectedDataView = null;
                            pvtEmployeeRejectedDataView = new DataView(pvtDataSet.Tables[pvtstrTableName],
                                    "PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo.ToString() + " AND " + pvtstrTableDef + "_DATE = '" + pvtdtDayDateTime.ToString("yyyy-MM-dd") + "'",
                                    "EMPLOYEE_NO",
                                    DataViewRowState.CurrentRows);

                            for (int intRow = 0; intRow < this.dgvEmployeeDataGridView.Rows.Count; intRow++)
                            {
                                if (pvtstrLockType == "Y")
                                {
                                    this.dgvEmployeeDataGridView[0, intRow].Style = PayrollRunDataGridViewCellStyle;
                                }
                                else
                                {
                                    if (pvtstrLockType == "A")
                                    {
                                        this.dgvEmployeeDataGridView[0, intRow].Style = AuthorisedDataGridViewCellStyle;
                                    }
                                    else
                                    {
                                        if (pvtstrLockType == "P")
                                        {
                                            this.dgvEmployeeDataGridView[0, intRow].Style = PayrollRunPendingDataGridViewCellStyle;
                                        }
                                        else
                                        {
                                            this.dgvEmployeeDataGridView[0, intRow].Style = this.NormalDataGridViewCellStyle;
                                        }
                                    }
                                }

                                intFindRow = pvtEmployeeRejectedDataView.Find(dgvEmployeeDataGridView[pvtintEmployeeNoColEmployeeDataGridView, intRow].Value.ToString());

                                if (intFindRow > -1)
                                {
                                    this.dgvEmployeeDataGridView[1, intRow].Style = RejectedDataGridViewCellStyle;
                                }
                                else
                                {
                                    this.dgvEmployeeDataGridView[1, intRow].Style = NormalDataGridViewCellStyle;
                                }
                                
                                if (pvtblnPublicHoliday == true)
                                {
                                    this.dgvEmployeeDataGridView[2, intRow].Style = this.PublicHolidayDataGridViewCellStyle;
                                }
                                else
                                {
                                    this.dgvEmployeeDataGridView[2, intRow].Style = this.NormalDataGridViewCellStyle;

                                }

                                intFindRow = pvtEmployeeLeaveDataView.Find(dgvEmployeeDataGridView[pvtintEmployeeNoColEmployeeDataGridView, intRow].Value.ToString());

                                if (intFindRow > -1)
                                {
                                    this.dgvEmployeeDataGridView[3, intRow].Style = this.LeaveDataGridViewCellStyle;
                                }
                                else
                                {
                                    this.dgvEmployeeDataGridView[3, intRow].Style = this.NormalDataGridViewCellStyle;
                                }
                            }

                            for (int intRow = 0; intRow < this.dgvEmployeeChosenDataGridView.Rows.Count; intRow++)
                            {

                                if (pvtstrLockType == "Y")
                                {
                                    this.dgvEmployeeChosenDataGridView[0, intRow].Style = PayrollRunDataGridViewCellStyle;
                                }
                                else
                                {
                                    if (pvtstrLockType == "A")
                                    {
                                        this.dgvEmployeeChosenDataGridView[0, intRow].Style = AuthorisedDataGridViewCellStyle;
                                    }
                                    else
                                    {
                                        if (pvtstrLockType == "P")
                                        {
                                            this.dgvEmployeeChosenDataGridView[0, intRow].Style = PayrollRunPendingDataGridViewCellStyle;
                                        }
                                        else
                                        {
                                            this.dgvEmployeeChosenDataGridView[0, intRow].Style = this.NormalDataGridViewCellStyle;
                                        }
                                    }
                                }

                                intFindRow = pvtEmployeeRejectedDataView.Find(dgvEmployeeChosenDataGridView[pvtintEmployeeNoColEmployeeDataGridView, intRow].Value.ToString());

                                if (intFindRow > -1)
                                {
                                    this.dgvEmployeeChosenDataGridView[1, intRow].Style = RejectedDataGridViewCellStyle;
                                }
                                else
                                {
                                    this.dgvEmployeeChosenDataGridView[1, intRow].Style = NormalDataGridViewCellStyle;
                                }

                                if (pvtblnPublicHoliday == true)
                                {
                                    this.dgvEmployeeChosenDataGridView[2, intRow].Style = this.PublicHolidayDataGridViewCellStyle;
                                }
                                else
                                {
                                    this.dgvEmployeeChosenDataGridView[2, intRow].Style = this.NormalDataGridViewCellStyle;

                                }

                                intFindRow = pvtEmployeeLeaveDataView.Find(dgvEmployeeChosenDataGridView[pvtintEmployeeNoColEmployeeDataGridView, intRow].Value.ToString());

                                if (intFindRow > -1)
                                {
                                    this.dgvEmployeeChosenDataGridView[3, intRow].Style = this.LeaveDataGridViewCellStyle;
                                }
                                else
                                {
                                    this.dgvEmployeeChosenDataGridView[3, intRow].Style = this.NormalDataGridViewCellStyle;
                                }
                            }

                            pvtblnEmployeeDataGridViewLoaded = true;

                            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                            {
                                this.Set_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView));
                            }

                            if (this.dgvEmployeeChosenDataGridView.Rows.Count > 0)
                            {
                                this.Set_DataGridView_SelectedRowIndex(dgvEmployeeChosenDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeChosenDataGridView));
                            }
                        }
                        else
                        {
                            this.lblEmployee.Text = "List of Employee Timesheets";
                            this.lblChosenEmployee.Text = "Selected Employees Timesheets";
                        }

                        this.Cursor = Cursors.Default;
                        return;
                    }

                    this.lblEmployeeRejected.Text = "";
                    this.lblEmployeeChosenRejected.Text = "";

                    pvtEmployeeDataView = null;
                    pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                        "PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND EMPLOYEE_LAST_RUNDATE < '" + pvtdtDayDateTime.ToString("yyyy-MM-dd") + "'",
                        "",
                        DataViewRowState.CurrentRows);

                    pvtTimesheetOrBreakDataView = null;

                    string strFilter = "";

                    if (this.rbnDeleteRow.Checked == true
                           | this.chkUpdate.Checked == true)
                    {
                        strFilter = " AND EMPLOYEE_NO = -1";
                    }
                    else
                    {
                        if (this.rbnStop.Checked == true)
                        {
                            strFilter = " AND NOT " + pvtstrTableDef + "_TIME_OUT_MINUTES IS NULL";
                        }
                    }

                    pvtEmployeeRejectedDataView = new DataView(pvtDataSet.Tables[pvtstrTableName],
                            "PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND " + pvtstrTableDef + "_DATE = '" + pvtdtDayDateTime.ToString("yyyy-MM-dd") + "'" + strFilter,
                            "EMPLOYEE_NO",
                            DataViewRowState.CurrentRows);

                    string strIn = "";
                    string strOut = "";

                    string strClockIn = "";
                    string strClockOut = "";
                    
                    pvtblnEmployeeDataGridViewLoaded = false;

                    for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
                    {
                        if ((this.rbnStart.Checked == true
                            | this.rbnBoth.Checked == true)
                            & this.chkUpdate.Checked == false)
                        {
                            this.dgvEmployeeDataGridView.Rows.Add("",
                                                                  "",
                                                                  "",
                                                                  "",
                                                                  pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                                  pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                                  pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                                  "",
                                                                  "",
                                                                  "",
                                                                  "",
                                                                  pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString(),
                                                                  "-1");

                            if (pvtstrLockType == "Y")
                            {
                                this.dgvEmployeeDataGridView[0, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = PayrollRunDataGridViewCellStyle;
                            }
                            else
                            {
                                if (pvtstrLockType == "A")
                                {
                                    this.dgvEmployeeDataGridView[0, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = AuthorisedDataGridViewCellStyle;
                                }
                                else
                                {
                                    if (pvtstrLockType == "P")
                                    {
                                        this.dgvEmployeeDataGridView[0, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = PayrollRunPendingDataGridViewCellStyle;
                                    }
                                }
                            }

                            intFindRow = pvtEmployeeRejectedDataView.Find(pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString());

                            if (intFindRow > -1)
                            {
                                this.dgvEmployeeDataGridView[1,this.dgvEmployeeDataGridView.Rows.Count - 1].Style = RejectedDataGridViewCellStyle;
                            }

                            if (pvtblnPublicHoliday == true)
                            {
                                this.dgvEmployeeDataGridView[2, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = this.PublicHolidayDataGridViewCellStyle;
                            }

                            intFindRow = pvtEmployeeLeaveDataView.Find(pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString());

                            if (intFindRow > -1)
                            {
                                this.dgvEmployeeDataGridView[3, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = this.LeaveDataGridViewCellStyle;
                            }
                        }
                        else
                        {
                            if (this.rbnStop.Checked == true
                             & this.chkUpdate.Checked == false)
                            {
                                strFilter = " AND " + pvtstrTableDef + "_TIME_OUT_MINUTES IS NULL";
                            }
                            else
                            {
                                strFilter = "";
                            }

                            pvtTimesheetOrBreakDataView = null;
                            pvtTimesheetOrBreakDataView = new DataView(pvtDataSet.Tables[pvtstrTableName],
                            "PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND EMPLOYEE_NO = " + pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString() + " AND " + pvtstrTableDef + "_DATE = '" + pvtdtDayDateTime.ToString("yyyy-MM-dd") + "'" + strFilter,
                            "EMPLOYEE_NO",
                             DataViewRowState.CurrentRows);

                            for (int intTimesheetRow = 0; intTimesheetRow < pvtTimesheetOrBreakDataView.Count; intTimesheetRow++)
                            {
                                if (pvtTimesheetOrBreakDataView[intTimesheetRow]["CLOCKED_TIME_IN_MINUTES"] != System.DBNull.Value)
                                {
                                    intHH = Convert.ToInt32(pvtTimesheetOrBreakDataView[intTimesheetRow]["CLOCKED_TIME_IN_MINUTES"]) / 60;
                                    intMM = Convert.ToInt32(pvtTimesheetOrBreakDataView[intTimesheetRow]["CLOCKED_TIME_IN_MINUTES"]) % 60;

                                    strClockIn = intHH.ToString() + intMM.ToString("00");
                                }
                                else
                                {
                                    strClockIn = "";
                                }

                                if (pvtTimesheetOrBreakDataView[intTimesheetRow]["CLOCKED_TIME_OUT_MINUTES"] != System.DBNull.Value)
                                {
                                    intHH = Convert.ToInt32(pvtTimesheetOrBreakDataView[intTimesheetRow]["CLOCKED_TIME_OUT_MINUTES"]) / 60;
                                    intMM = Convert.ToInt32(pvtTimesheetOrBreakDataView[intTimesheetRow]["CLOCKED_TIME_OUT_MINUTES"]) % 60;

                                    strClockOut = intHH.ToString() + intMM.ToString("00");
                                }
                                else
                                {
                                    strClockOut = "";
                                }

                                if (pvtTimesheetOrBreakDataView[intTimesheetRow][pvtstrTableDef + "_TIME_IN_MINUTES"] != System.DBNull.Value)
                                {
                                    intHH = Convert.ToInt32(pvtTimesheetOrBreakDataView[intTimesheetRow][pvtstrTableDef + "_TIME_IN_MINUTES"]) / 60;
                                    intMM = Convert.ToInt32(pvtTimesheetOrBreakDataView[intTimesheetRow][pvtstrTableDef + "_TIME_IN_MINUTES"]) % 60;

                                    strIn = intHH.ToString() + intMM.ToString("00");
                                }
                                else
                                {
                                    strIn = "";
                                }

                                if (pvtTimesheetOrBreakDataView[intTimesheetRow][pvtstrTableDef + "_TIME_OUT_MINUTES"] != System.DBNull.Value)
                                {
                                    intHH = Convert.ToInt32(pvtTimesheetOrBreakDataView[intTimesheetRow][pvtstrTableDef + "_TIME_OUT_MINUTES"]) / 60;
                                    intMM = Convert.ToInt32(pvtTimesheetOrBreakDataView[intTimesheetRow][pvtstrTableDef + "_TIME_OUT_MINUTES"]) % 60;

                                    strOut = intHH.ToString() + intMM.ToString("00");
                                }
                                else
                                {
                                    strOut = "";
                                }

                                this.dgvEmployeeDataGridView.Rows.Add("",
                                                                      "",
                                                                      "",
                                                                      "",
                                                                      pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                                      pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                                      pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                                      strClockIn,
                                                                      strIn,
                                                                      strClockOut,
                                                                      strOut,
                                                                      pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString(),
                                                                      pvtTimesheetOrBreakDataView[intTimesheetRow][pvtstrTableDef + "_SEQ"].ToString());

                                if (pvtstrLockType == "Y")
                                {
                                    this.dgvEmployeeDataGridView[0, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = PayrollRunDataGridViewCellStyle;
                                }
                                else
                                {
                                    if (pvtstrLockType == "A")
                                    {
                                        this.dgvEmployeeDataGridView[0, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = AuthorisedDataGridViewCellStyle;
                                    }
                                    else
                                    {
                                        if (pvtstrLockType == "P")
                                        {
                                            this.dgvEmployeeDataGridView[0, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = PayrollRunPendingDataGridViewCellStyle;
                                        }
                                    }
                                }

                                intFindRow = pvtEmployeeRejectedDataView.Find(pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString());

                                if (intFindRow > -1)
                                {
                                    this.dgvEmployeeDataGridView[1,this.dgvEmployeeDataGridView.Rows.Count - 1].Style = RejectedDataGridViewCellStyle;
                                }

                                if (pvtblnPublicHoliday == true)
                                {
                                    this.dgvEmployeeDataGridView[2, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = this.PublicHolidayDataGridViewCellStyle;
                                }

                                intFindRow = pvtEmployeeLeaveDataView.Find(pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString());

                                if (intFindRow > -1)
                                {
                                    this.dgvEmployeeDataGridView[3, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = this.LeaveDataGridViewCellStyle;
                                }
                            }
                        }
                    }

                    pvtblnEmployeeDataGridViewLoaded = true;

                    if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                    {
                        if (pvtstrLockType == "Y"
                        || pvtstrLockType == "A")
                        {
                            this.btnUpdate.Enabled = false;
                            this.btnDelete.Enabled = false;
                        }
                        else
                        {
                            this.btnUpdate.Enabled = true;
                            this.btnDelete.Enabled = true;
                        }
                        
                        this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, 0);
                    }
                    else
                    {
                        this.btnUpdate.Enabled = false;
                        this.btnDelete.Enabled = false;
                    }

                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void dgvEmployeeChosenDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (this.pvtblnEmployeeChosenDataGridViewLoaded == true)
            {
                if (pvtintEmployeeChosenDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintEmployeeChosenDataGridViewRowIndex = e.RowIndex;

                    this.Clear_DataGridView(this.dgvEmployeeChosenRejectedDataGridView);

                    if (this.rbnDeleteRow.Checked == true
                    | this.chkUpdate.Checked == true)
                    {
                    }
                    else
                    {
                        int intEmployeeNo = Convert.ToInt32(dgvEmployeeChosenDataGridView[pvtintEmployeeNoColEmployeeDataGridView, e.RowIndex].Value);

                        pvtEmployeeDataView = null;
                        pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                        "EMPLOYEE_NO = " + intEmployeeNo.ToString() + " AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                        "",
                        DataViewRowState.CurrentRows);

                        string strFilter = "";

                        if (this.rbnStop.Checked == true)
                        {
                            strFilter = " AND NOT " + pvtstrTableDef + "_TIME_OUT_MINUTES IS NULL";
                        }

                        pvtEmployeeRejectedDataView = null;
                        pvtEmployeeRejectedDataView = new DataView(pvtDataSet.Tables[pvtstrTableName],
                                 "PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND EMPLOYEE_NO = " + intEmployeeNo.ToString() + " AND " + pvtstrTableDef + "_DATE = '" + pvtdtDayDateTime.ToString("yyyy-MM-dd") + "'" + strFilter,
                                 "EMPLOYEE_NO",
                                 DataViewRowState.CurrentRows);

                        if (pvtEmployeeRejectedDataView.Count > 0)
                        {
                            if (pvtEmployeeDataView.Count > 0)
                            {
                                int intHH = 0;
                                int intMM = 0;
                                string strIn = "";
                                string strOut = "";
                                string strClockIn = "";
                                string strClockOut = "";

                                for (int intRow = 0; intRow < pvtEmployeeRejectedDataView.Count; intRow++)
                                {
                                    if (pvtEmployeeRejectedDataView[intRow]["CLOCKED_TIME_IN_MINUTES"] != System.DBNull.Value)
                                    {
                                        intHH = Convert.ToInt32(pvtEmployeeRejectedDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) / 60;
                                        intMM = Convert.ToInt32(pvtEmployeeRejectedDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) % 60;

                                        strClockIn = intHH.ToString() + intMM.ToString("00");
                                    }
                                    else
                                    {
                                        strClockIn = "";
                                    }

                                    if (pvtEmployeeRejectedDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"] != System.DBNull.Value)
                                    {
                                        intHH = Convert.ToInt32(pvtEmployeeRejectedDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) / 60;
                                        intMM = Convert.ToInt32(pvtEmployeeRejectedDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) % 60;

                                        strClockOut = intHH.ToString() + intMM.ToString("00");
                                    }
                                    else
                                    {
                                        strClockOut = "";
                                    }

                                    if (pvtEmployeeRejectedDataView[intRow][pvtstrTableDef + "_TIME_IN_MINUTES"] != System.DBNull.Value)
                                    {
                                        intHH = Convert.ToInt32(pvtEmployeeRejectedDataView[intRow][pvtstrTableDef + "_TIME_IN_MINUTES"]) / 60;
                                        intMM = Convert.ToInt32(pvtEmployeeRejectedDataView[intRow][pvtstrTableDef + "_TIME_IN_MINUTES"]) % 60;

                                        strIn = intHH.ToString() + intMM.ToString("00");
                                    }
                                    else
                                    {
                                        strIn = "";
                                    }

                                    if (pvtEmployeeRejectedDataView[intRow][pvtstrTableDef + "_TIME_OUT_MINUTES"] != System.DBNull.Value)
                                    {
                                        intHH = Convert.ToInt32(pvtEmployeeRejectedDataView[intRow][pvtstrTableDef + "_TIME_OUT_MINUTES"]) / 60;
                                        intMM = Convert.ToInt32(pvtEmployeeRejectedDataView[intRow][pvtstrTableDef + "_TIME_OUT_MINUTES"]) % 60;

                                        strOut = intHH.ToString() + intMM.ToString("00");
                                    }
                                    else
                                    {
                                        strOut = "";
                                    }

                                    this.dgvEmployeeChosenRejectedDataGridView.Rows.Add("",
                                                                                        "",
                                                                                        "",
                                                                                        "",
                                                                                        pvtEmployeeDataView[0]["EMPLOYEE_CODE"].ToString(),
                                                                                        pvtEmployeeDataView[0]["EMPLOYEE_SURNAME"].ToString(),
                                                                                        pvtEmployeeDataView[0]["EMPLOYEE_NAME"].ToString(),
                                                                                        strClockIn,
                                                                                        strIn,
                                                                                        strClockOut,
                                                                                        strOut);

                                    if (pvtstrLockType == "Y")
                                    {
                                        this.dgvEmployeeChosenRejectedDataGridView[0, this.dgvEmployeeChosenRejectedDataGridView.Rows.Count - 1].Style = PayrollRunDataGridViewCellStyle;
                                    }
                                    else
                                    {
                                        if (pvtstrLockType == "A")
                                        {
                                            this.dgvEmployeeChosenRejectedDataGridView[0, this.dgvEmployeeChosenRejectedDataGridView.Rows.Count - 1].Style = AuthorisedDataGridViewCellStyle;
                                        }
                                        else
                                        {
                                            if (pvtstrLockType == "P")
                                            {
                                                this.dgvEmployeeChosenRejectedDataGridView[0, this.dgvEmployeeChosenRejectedDataGridView.Rows.Count - 1].Style = PayrollRunPendingDataGridViewCellStyle;
                                            }
                                        }
                                    }

                                    this.dgvEmployeeChosenRejectedDataGridView[1, this.dgvEmployeeChosenRejectedDataGridView.Rows.Count - 1].Style = RejectedDataGridViewCellStyle;

                                    if (pvtblnPublicHoliday == true)
                                    {
                                        this.dgvEmployeeChosenRejectedDataGridView[2, this.dgvEmployeeChosenRejectedDataGridView.Rows.Count - 1].Style = this.PublicHolidayDataGridViewCellStyle;
                                    }

                                    pvtEmployeeLeaveDataView = null;
                                    pvtEmployeeLeaveDataView = new DataView(pvtDataSet.Tables["EmployeeLeave"],
                                            "PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo.ToString() + " AND PAY_CATEGORY_TYPE = '" + this.pvtstrPayrollType + "' AND EMPLOYEE_NO = " + intEmployeeNo.ToString() + " AND DAY_DATE = '" + pvtdtDayDateTime.ToString("yyyy-MM-dd") + "'",
                                            "EMPLOYEE_NO",
                                            DataViewRowState.CurrentRows);

                                    if (pvtEmployeeLeaveDataView.Count > 0)
                                    {
                                        this.dgvEmployeeChosenRejectedDataGridView[3, this.dgvEmployeeChosenRejectedDataGridView.Rows.Count - 1].Style = this.LeaveDataGridViewCellStyle;
                                    }
                                }

                                if (this.dgvEmployeeChosenRejectedDataGridView.Rows.Count > 0)
                                {
                                    if (this.rbnSelectedDates.Checked == true)
                                    {
                                        this.lblEmployeeChosenRejected.Text = pvtEmployeeDataView[0]["EMPLOYEE_NAME"].ToString() + " " + pvtEmployeeDataView[0]["EMPLOYEE_SURNAME"].ToString() + " - " + dgvDayChosenDataGridView[pvtintDateColDayDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayChosenDataGridView)].Value.ToString();
                                    }
                                    else
                                    {
                                        this.lblEmployeeChosenRejected.Text = pvtEmployeeDataView[0]["EMPLOYEE_NAME"].ToString() + " " + pvtEmployeeDataView[0]["EMPLOYEE_SURNAME"].ToString() + " - " + dgvDayDataGridView[pvtintDateColDayDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Value.ToString();
                                    }

                                    this.Set_DataGridView_SelectedRowIndex(dgvEmployeeChosenRejectedDataGridView, 0);
                                }
                                else
                                {
                                    this.lblEmployeeChosenRejected.Text = "";
                                }
                            }
                            else
                            {
                                this.lblEmployeeChosenRejected.Text = "";
                            }
                        }
                        else
                        {
                            this.lblEmployeeChosenRejected.Text = "";
                        }
                    }
                }
            }
        }

        private void dgvEmployeeRejectedDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvEmployeeRejectedDataGridView.Rows.Count > 0
            & this.pvtblnEmployeeRejectedDataGridViewLoaded == true)
            {

            }
        }

        private void btnDateAdd_Click(object sender, EventArgs e)
        {
            if (this.dgvDayDataGridView.Rows.Count > 0)
            {
                if (this.dgvDayDataGridView[pvtintLockColDayDataGridView, Get_DataGridView_SelectedRowIndex(this.dgvDayDataGridView)].Value.ToString() == "Y"
                       | this.dgvDayDataGridView[pvtintLockColDayDataGridView, Get_DataGridView_SelectedRowIndex(this.dgvDayDataGridView)].Value.ToString() == "A")
                {
                    CustomMessageBox.Show("This Date has been Locked.\nAction Cancelled", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //Stops DataGridView from Falling Over
                pvtblnDayDataGridViewLoaded = false;
                pvtblnDayChosenDataGridViewLoaded = false;

                if (this.dgvDayChosenDataGridView.Rows.Count == 0)
                {
                    this.rbnSelectedDates.Checked = true;
                }

                DataGridViewRow myDataGridViewRow = this.dgvDayDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvDayDataGridView)];

                this.dgvDayDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvDayChosenDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvDayChosenDataGridView.CurrentCell = this.dgvDayChosenDataGridView[pvtintDateColDayDataGridView, this.dgvDayChosenDataGridView.Rows.Count - 1];

                pvtblnDayDataGridViewLoaded = true;
                pvtblnDayChosenDataGridViewLoaded = true;

                if (this.rbnSelectedDates.Checked == true)
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvDayChosenDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayChosenDataGridView));
                }
                else
                {
                    if (dgvDayDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(dgvDayDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView));
                    }
                }
            }
        }

        private void btnDateRemove_Click(object sender, EventArgs e)
        {
            if (this.dgvDayChosenDataGridView.Rows.Count > 0)
            {
                if (this.dgvDayChosenDataGridView.Rows.Count == 1)
                {
                    this.rbnListDates.Checked = true;
                }

                DataGridViewRow myDataGridViewRow = this.dgvDayChosenDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvDayChosenDataGridView)];

                pvtblnEmployeeDataGridViewLoaded = false;

                this.dgvDayChosenDataGridView.Rows.Remove(myDataGridViewRow);

                pvtblnEmployeeDataGridViewLoaded = true;

                this.dgvDayDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvDayDataGridView.CurrentCell = this.dgvDayDataGridView[pvtintDateColDayDataGridView, this.dgvDayDataGridView.Rows.Count - 1];
            }
        }

        private void dgvDayChosenDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (pvtblnDayChosenDataGridViewLoaded == true)
                {
                    if (pvtintDayChosenDataGridViewRowIndex != e.RowIndex)
                    {
                        pvtintDayChosenDataGridViewRowIndex = e.RowIndex;

                        if (this.rbnSelectedDates.Checked == true)
                        {
                            pvtdtDayDateTime = DateTime.ParseExact(this.dgvDayChosenDataGridView[pvtintDateSortColDayDataGridView, e.RowIndex].Value.ToString(), "yyyy-MM-dd", null);

                            this.lblEmployeeRejected.Text = "";
                            this.lblEmployeeChosenRejected.Text = "";

                            this.Clear_DataGridView(this.dgvEmployeeRejectedDataGridView);
                            this.Clear_DataGridView(this.dgvEmployeeChosenRejectedDataGridView);

                            int intFindRow = -1;

                            pvtEmployeeRejectedDataView = null;
                            pvtEmployeeRejectedDataView = new DataView(pvtDataSet.Tables[pvtstrTableName],
                                    "PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo.ToString() + " AND " + pvtstrTableDef + "_DATE = '" + pvtdtDayDateTime.ToString("yyyy-MM-dd") + "'",
                                    "EMPLOYEE_NO",
                                    DataViewRowState.CurrentRows);
                            
                            this.lblEmployee.Text = "List of Employee Timesheets - " + pvtdtDayDateTime.ToString("dd MMM yyyy - ddd");
                            this.lblChosenEmployee.Text = "Selected Employees Timesheets - " + pvtdtDayDateTime.ToString("dd MMM yyyy - ddd");
                            
                            intFindRow = pvtPublicHolidayDataView.Find(pvtdtDayDateTime.ToString("yyyy-MM-dd"));

                            if (intFindRow > -1)
                            {
                                pvtblnPublicHoliday = true;
                            }
                            else
                            {
                                pvtblnPublicHoliday = false;
                            }

                            pvtEmployeeLeaveDataView = null;
                            pvtEmployeeLeaveDataView = new DataView(pvtDataSet.Tables["EmployeeLeave"],
                                    "PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo.ToString() + " AND PAY_CATEGORY_TYPE = '" + this.pvtstrPayrollType + "' AND DAY_DATE = '" + pvtdtDayDateTime.ToString("yyyy-MM-dd") + "'",
                                    "EMPLOYEE_NO",
                                    DataViewRowState.CurrentRows);

                            pvtstrLockType = this.dgvDayChosenDataGridView[pvtintLockColDayDataGridView, e.RowIndex].Value.ToString();
                            
                            for (int intRow = 0; intRow < this.dgvEmployeeDataGridView.Rows.Count; intRow++)
                            {
                                if (pvtstrLockType == "Y")
                                {
                                    this.dgvEmployeeDataGridView[0, intRow].Style = PayrollRunDataGridViewCellStyle;
                                }
                                else
                                {
                                    if (pvtstrLockType == "A")
                                    {
                                        this.dgvEmployeeDataGridView[0, intRow].Style = AuthorisedDataGridViewCellStyle;
                                    }
                                    else
                                    {
                                        if (pvtstrLockType == "P")
                                        {
                                            this.dgvEmployeeDataGridView[0, intRow].Style = PayrollRunPendingDataGridViewCellStyle;
                                        }
                                        else
                                        {
                                            this.dgvEmployeeDataGridView[0, intRow].Style = this.NormalDataGridViewCellStyle;
                                        }
                                    }
                                }

                                intFindRow = pvtEmployeeRejectedDataView.Find(dgvEmployeeDataGridView[pvtintEmployeeNoColEmployeeDataGridView, intRow].Value.ToString());

                                if (intFindRow > -1)
                                {
                                    this.dgvEmployeeDataGridView[1,intRow].Style = RejectedDataGridViewCellStyle;
                                }
                                else
                                {
                                    this.dgvEmployeeDataGridView[1,intRow].Style = NormalDataGridViewCellStyle;
                                }

                                intFindRow = pvtEmployeeLeaveDataView.Find(dgvEmployeeDataGridView[pvtintEmployeeNoColEmployeeDataGridView, intRow].Value.ToString());

                                if (intFindRow > -1)
                                {
                                    this.dgvEmployeeDataGridView[3, intRow].Style = this.LeaveDataGridViewCellStyle;
                                }
                                else
                                {
                                    this.dgvEmployeeDataGridView[3, intRow].Style = this.NormalDataGridViewCellStyle;
                                }

                                if (pvtblnPublicHoliday == true)
                                {
                                    this.dgvEmployeeDataGridView[2, intRow].Style = this.PublicHolidayDataGridViewCellStyle;
                                }
                                else
                                {
                                    this.dgvEmployeeDataGridView[2, intRow].Style = this.NormalDataGridViewCellStyle;

                                }

                                intFindRow = pvtEmployeeLeaveDataView.Find(dgvEmployeeDataGridView[pvtintEmployeeNoColEmployeeDataGridView, intRow].Value.ToString());

                                if (intFindRow > -1)
                                {
                                    this.dgvEmployeeDataGridView[3, intRow].Style = this.LeaveDataGridViewCellStyle;
                                }
                                else
                                {
                                    this.dgvEmployeeDataGridView[3, intRow].Style = this.NormalDataGridViewCellStyle;
                                }
                            }

                            for (int intRow = 0; intRow < this.dgvEmployeeChosenDataGridView.Rows.Count; intRow++)
                            {
                                if (pvtstrLockType == "Y")
                                {
                                    this.dgvEmployeeChosenDataGridView[0, intRow].Style = PayrollRunDataGridViewCellStyle;
                                }
                                else
                                {
                                    if (pvtstrLockType == "A")
                                    {
                                        this.dgvEmployeeChosenDataGridView[0, intRow].Style = AuthorisedDataGridViewCellStyle;
                                    }
                                    else
                                    {
                                        if (pvtstrLockType == "P")
                                        {
                                            this.dgvEmployeeChosenDataGridView[0, intRow].Style = PayrollRunPendingDataGridViewCellStyle;
                                        }
                                        else
                                        {
                                            this.dgvEmployeeChosenDataGridView[0, intRow].Style = this.NormalDataGridViewCellStyle;
                                        }

                                    }
                                }
                                
                                intFindRow = pvtEmployeeRejectedDataView.Find(dgvEmployeeChosenDataGridView[pvtintEmployeeNoColEmployeeDataGridView, intRow].Value.ToString());

                                if (intFindRow > -1)
                                {
                                    this.dgvEmployeeChosenDataGridView[1,intRow].Style = RejectedDataGridViewCellStyle;
                                }
                                else
                                {
                                    this.dgvEmployeeChosenDataGridView[1,intRow].Style = NormalDataGridViewCellStyle;
                                }

                                intFindRow = pvtEmployeeLeaveDataView.Find(dgvEmployeeChosenDataGridView[pvtintEmployeeNoColEmployeeDataGridView, intRow].Value.ToString());

                                if (intFindRow > -1)
                                {
                                    this.dgvEmployeeChosenDataGridView[3, intRow].Style = this.LeaveDataGridViewCellStyle;
                                }
                                else
                                {
                                    this.dgvEmployeeChosenDataGridView[3, intRow].Style = this.NormalDataGridViewCellStyle;
                                }

                                if (pvtblnPublicHoliday == true)
                                {
                                    this.dgvEmployeeChosenDataGridView[2, intRow].Style = this.PublicHolidayDataGridViewCellStyle;
                                }
                                else
                                {
                                    this.dgvEmployeeChosenDataGridView[2, intRow].Style = this.NormalDataGridViewCellStyle;
                                }

                                intFindRow = pvtEmployeeLeaveDataView.Find(dgvEmployeeChosenDataGridView[pvtintEmployeeNoColEmployeeDataGridView, intRow].Value.ToString());

                                if (intFindRow > -1)
                                {
                                    this.dgvEmployeeChosenDataGridView[3, intRow].Style = this.LeaveDataGridViewCellStyle;
                                }
                                else
                                {
                                    this.dgvEmployeeChosenDataGridView[3, intRow].Style = this.NormalDataGridViewCellStyle;
                                }
                            }

                            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                            {
                                this.Set_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView));
                            }

                            if (this.dgvEmployeeChosenDataGridView.Rows.Count > 0)
                            {
                                this.Set_DataGridView_SelectedRowIndex(dgvEmployeeChosenDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeChosenDataGridView));
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

        private void dgvDayDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (this.btnDateAdd.Visible == true)
                {
                    btnDateAdd_Click(sender, e);
                }
            }
        }

        private void dgvDayChosenDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (this.btnDateRemove.Visible == true)
                {
                    btnDateRemove_Click(sender, e);
                }
            }
        }

        private void dgvEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                btnAdd_Click(sender, e);
            }
        }

        private void dgvEmployeeChosenDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                btnRemove_Click(sender, e);
            }
        }

        private void dgvEmployeeChosenRejectedDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
