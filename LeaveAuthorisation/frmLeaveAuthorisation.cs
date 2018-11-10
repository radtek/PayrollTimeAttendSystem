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
    public partial class frmLeaveAuthorisation : Form
    {
        clsISUtilities clsISUtilities;
        DataSet pvtDataSet;

        private DataView pvtEmployeeDataView;
        private DataView pvtEmployeePayCategoryLevelDataView;
        private DataView pvtLeaveDataView;
        private DataView pvtLeaveTypeDataView;
        private DataView pvtPayCategoryTimeDecimalDataView;
        private DataView pvtPublicHolidayDataView;
        private DataView pvtAuthorsiseLevelDataView;

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnAuthorisationLevelDataGridViewLoaded = false;
        private bool pvtblnLeaveDataGridViewLoaded = false;

        private byte[] pvtbytCompress;

        private string pvtstrPayrollType = "";
        private string pvtstrPayrollTypeFilter = "";

        private int pvtintColLeaveType = 5;
        private int pvtintColDescription = 6;
        private int pvtintColFromDate = 7;
        private int pvtintColToDate = 8;

        private int pvtintColAuthorisation = 12;
        private int pvtintColLeaveHoldingIndex = 15;

        private int pvtintFindEmployeeRow;

        private int pvtintAuthoriseLevel = -1;

        private Int64 pvtint64CompanyNo = -1;

        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
        private int pvtintAuthorisationLevelDataGridViewRowIndex = -1;
        private int pvtintLeaveDataGridViewRowIndex = -1;

        DataGridViewCellStyle LockedPayrollRunDataGridViewCellStyle;
        DataGridViewCellStyle ErrorDataGridViewCellStyle;
        DataGridViewCellStyle LeaveDaysExcludedDataGridViewCellStyle;
        DataGridViewCellStyle PublicHolidayDataGridViewCellStyle;
        DataGridViewCellStyle NormalDataGridViewCellStyle;
        DataGridViewCellStyle HoursOptionDataGridViewCellStyle;

        public frmLeaveAuthorisation()
        {
            InitializeComponent();
        }

        private void frmLeaveAuthorisation_Load(object sender, EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this, "busLeaveAuthorisation");

                LockedPayrollRunDataGridViewCellStyle = new DataGridViewCellStyle();
                LockedPayrollRunDataGridViewCellStyle.BackColor = Color.Magenta;
                LockedPayrollRunDataGridViewCellStyle.SelectionBackColor = Color.Magenta;

                ErrorDataGridViewCellStyle = new DataGridViewCellStyle();
                ErrorDataGridViewCellStyle.BackColor = Color.Red;
                ErrorDataGridViewCellStyle.SelectionBackColor = Color.Red;

                LeaveDaysExcludedDataGridViewCellStyle = new DataGridViewCellStyle();
                LeaveDaysExcludedDataGridViewCellStyle.BackColor = Color.Yellow;
                LeaveDaysExcludedDataGridViewCellStyle.SelectionBackColor = Color.Yellow;

                PublicHolidayDataGridViewCellStyle = new DataGridViewCellStyle();
                PublicHolidayDataGridViewCellStyle.BackColor = Color.SlateBlue;
                PublicHolidayDataGridViewCellStyle.SelectionBackColor = Color.SlateBlue;

                HoursOptionDataGridViewCellStyle = new DataGridViewCellStyle();
                HoursOptionDataGridViewCellStyle.BackColor = Color.Lime;
                HoursOptionDataGridViewCellStyle.SelectionBackColor = Color.Lime;

                NormalDataGridViewCellStyle = new DataGridViewCellStyle();
                NormalDataGridViewCellStyle.BackColor = SystemColors.Control;
                NormalDataGridViewCellStyle.SelectionBackColor = SystemColors.Control;

                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblAuthorisationLevelSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblLeaveCaption.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblLeaveDate.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                pvtint64CompanyNo = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));

                object[] objParm = new object[4];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[2] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[3] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();

                byte[] bytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);

                pvtPublicHolidayDataView = null;
                pvtPublicHolidayDataView = new DataView(pvtDataSet.Tables["PublicHoliday"],
                "",
                "PUBLIC_HOLIDAY_DATE",
                DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < pvtDataSet.Tables["PayrollType"].Rows.Count; intRow++)
                {
                    this.dgvPayrollTypeDataGridView.Rows.Add(pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE_DESC"].ToString());

                    if (((this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"].ToString() == "Y"
                   | this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"].ToString() == "N")
                   & pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE_DESC"].ToString().Substring(0, 1) == "W")
                   | (this.pvtDataSet.Tables["Company"].Rows[0]["SALARY_RUN_IND"].ToString() == "Y"
                   & pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE_DESC"].ToString().Substring(0, 1) == "S")
                   | (this.pvtDataSet.Tables["Company"].Rows[0]["TIME_ATTENDANCE_RUN_IND"].ToString() == "Y"
                   & pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE_DESC"].ToString().Substring(0, 1) == "T"))
                    {
                        this.dgvPayrollTypeDataGridView.Rows[this.dgvPayrollTypeDataGridView.Rows.Count - 1].HeaderCell.Style = this.LockedPayrollRunDataGridViewCellStyle;
                    }
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

        private void btnClose_Click(object sender, EventArgs e)
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

                    case "dgvAuthorisationLevelDataGridView":

                        pvtintAuthorisationLevelDataGridViewRowIndex = -1;
                        this.dgvAuthorisationLevelDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvLeaveDataGridView":

                        pvtintLeaveDataGridViewRowIndex = -1;
                        this.dgvLeaveDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    default:

                        MessageBox.Show("No Entry for " + myDataGridView.Name + " in Set_DataGridView_SelectedRowIndex", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            else
            {
                if (myDataGridView.Name == "dgvAuthorisationLevelDataGridView")
                {
                    pvtintAuthorisationLevelDataGridViewRowIndex = -1;
                }

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

        private void dgvPayrollTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPayrollTypeDataGridViewLoaded == true
                & pvtintPayrollTypeDataGridViewRowIndex != e.RowIndex)
            {
                pvtintPayrollTypeDataGridViewRowIndex = e.RowIndex;

                this.Cursor = Cursors.AppStarting;

                pvtstrPayrollType = this.dgvPayrollTypeDataGridView[0, e.RowIndex].Value.ToString().Substring(0, 1);

                if ((this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"].ToString() != ""
                & pvtstrPayrollType == "W")
                | (this.pvtDataSet.Tables["Company"].Rows[0]["SALARY_RUN_IND"].ToString() != ""
                & pvtstrPayrollType == "S"))
                {
                    if (pvtstrPayrollType == "W")
                    {
                        this.lblLeaveErrors.Text = "Leave Records are Locked due to Current Wage Run.";
                    }
                    else
                    {
                        this.lblLeaveErrors.Text = "Leave Records are Locked due to Current Salary Run.";
                    }

                    grbLeaveLock.Visible = true;
                }
                else
                {
                    grbLeaveLock.Visible = false;
                }

                pvtstrPayrollTypeFilter = "COMPANY_NO = " + this.pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'";

                this.pvtblnAuthorisationLevelDataGridViewLoaded = false;

                this.cboOption.Items.Clear();

                this.cboOption.Items.Add("All");
                this.cboOption.Items.Add("Day/s");
                this.cboOption.Items.Add("Hours/s");

                this.cboOption.SelectedIndex = 0;

                pvtLeaveTypeDataView = null;
                pvtLeaveTypeDataView = new DataView(pvtDataSet.Tables["LeaveType"],
                pvtstrPayrollTypeFilter,
                 "EARNING_DESC",
                 DataViewRowState.CurrentRows);

                this.cboLeaveType.Items.Clear();

                this.cboLeaveType.Items.Add("All");

                for (int intIndex = 0; intIndex < pvtLeaveTypeDataView.Count; intIndex++)
                {
                    this.cboLeaveType.Items.Add(pvtLeaveTypeDataView[intIndex]["EARNING_DESC"].ToString());
                }

                this.cboLeaveType.SelectedIndex = 0;

                this.Clear_DataGridView(this.dgvAuthorisationLevelDataGridView);

                pvtAuthorsiseLevelDataView = null;
                pvtAuthorsiseLevelDataView = new DataView(pvtDataSet.Tables["AuthorsiseLevel"],
                pvtstrPayrollTypeFilter,
                 "LEVEL_NO",
                 DataViewRowState.CurrentRows);

                string strCount = "";
                int intCurrentRow = 0;

                for (int intRow = 0; intRow < pvtAuthorsiseLevelDataView.Count; intRow++)
                {
                    strCount = pvtAuthorsiseLevelDataView[intRow]["AUTHORISE_CURRENT"].ToString() + " / " + pvtAuthorsiseLevelDataView[intRow]["AUTHORISE_TOTAL"].ToString();

                    this.dgvAuthorisationLevelDataGridView.Rows.Add(pvtAuthorsiseLevelDataView[intRow]["LEVEL_DESC"].ToString(),
                                                                    strCount,
                                                                    pvtAuthorsiseLevelDataView[intRow]["LEVEL_NO"].ToString());

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

                this.Cursor = Cursors.Default;
            }
        }

        private void dgvAuthorisationLevelDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (this.pvtblnAuthorisationLevelDataGridViewLoaded == true
            & pvtintAuthorisationLevelDataGridViewRowIndex != e.RowIndex)
            {
                pvtintAuthorisationLevelDataGridViewRowIndex = e.RowIndex;

                pvtintAuthoriseLevel = Convert.ToInt32(this.dgvAuthorisationLevelDataGridView[2, e.RowIndex].Value);

                this.Clear_DataGridView(this.dgvLeaveDataGridView);
                this.Clear_DataGridView(this.dgvLeaveDateDataGridView);

                this.lblAmount.Text = "0.00";
                
                //Set To Choose First Row In Spreadsheet
                int intEmployeeNoRow = 0;

                this.lblLeaveDate.Text = "";

                pvtEmployeeDataView = null;
                pvtEmployeeDataView = new DataView(this.pvtDataSet.Tables["Employee"],
                    pvtstrPayrollTypeFilter,
                    "EMPLOYEE_NO",
                    DataViewRowState.CurrentRows);

                string strAuthorisationType = "";
                bool blnFilterApplied = false;

                if (this.rbnAuthorised.Checked == true)
                {
                    strAuthorisationType = " AND AUTHORISED_IND = 'Y'";

                    blnFilterApplied = true;
                }
                else
                {
                    if (rbnNotAuthorised.Checked == true)
                    {
                        strAuthorisationType = " AND AUTHORISED_IND <> 'Y'";

                        blnFilterApplied = true;
                    }
                }

                pvtEmployeePayCategoryLevelDataView = null;
                pvtEmployeePayCategoryLevelDataView = new DataView(this.pvtDataSet.Tables["EmployeePayCategoryLevel"],
                   pvtstrPayrollTypeFilter + " AND LEVEL_NO = " + this.pvtintAuthoriseLevel + strAuthorisationType,
                   "EARNING_NO,PAY_CATEGORY_NO,EMPLOYEE_NO,LEAVE_REC_NO",
                   DataViewRowState.CurrentRows);

                string strFilter = "";

                if (this.cboLeaveType.SelectedIndex != 0)
                {
                    for (int intRow = 0; intRow < pvtLeaveTypeDataView.Count; intRow++)
                    {
                        if (pvtLeaveTypeDataView[this.cboLeaveType.SelectedIndex - 1]["EARNING_DESC"].ToString() == this.cboLeaveType.SelectedItem.ToString())
                        {
                            strFilter += " AND EARNING_NO = " + pvtLeaveTypeDataView[this.cboLeaveType.SelectedIndex - 1]["EARNING_NO"].ToString();

                            blnFilterApplied = true;

                            break;
                        }
                    }
                }

                if (this.cboOption.SelectedIndex == 1)
                {
                    strFilter += " AND LEAVE_OPTION = 'D'";

                    blnFilterApplied = true;
                }
                else
                {
                    if (this.cboOption.SelectedIndex == 2)
                    {
                        strFilter += " AND LEAVE_OPTION = 'H'";

                        blnFilterApplied = true;
                    }
                }

                pvtLeaveDataView = null;
                pvtLeaveDataView = new DataView(this.pvtDataSet.Tables["Leave"],
                   pvtstrPayrollTypeFilter + strFilter,
                   "EMPLOYEE_NO",
                   DataViewRowState.CurrentRows);

                this.pvtblnLeaveDataGridViewLoaded = false;
                pvtintLeaveDataGridViewRowIndex = -1;

                bool blnPayrollRun = false;
                bool blnAuthorised = false;
                int intFindRecord = -1;
                int intFindRow = -1;
                string strDayHourOption = "";
                object[] objFind = new object[4];
                
                //Set Error
                for (int intRow = 0; intRow < pvtLeaveDataView.Count; intRow++)
                {
                    objFind[0] = pvtLeaveDataView[intRow]["EARNING_NO"].ToString();
                    objFind[1] = pvtLeaveDataView[intRow]["PAY_CATEGORY_NO"].ToString();
                    objFind[2] = pvtLeaveDataView[intRow]["EMPLOYEE_NO"].ToString();
                    objFind[3] = pvtLeaveDataView[intRow]["LEAVE_REC_NO"].ToString();

                    intFindRecord = pvtEmployeePayCategoryLevelDataView.Find(objFind);

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

                    pvtintFindEmployeeRow = pvtEmployeeDataView.Find(pvtLeaveDataView[intRow]["EMPLOYEE_NO"].ToString());

                    if (pvtLeaveDataView[intRow]["LEAVE_OPTION"].ToString() == "D")
                    {
                        strDayHourOption = "Day/s";
                    }
                    else
                    {
                        strDayHourOption = "Hour/s";
                    }

                    for (intFindRow = 0; intFindRow < pvtLeaveTypeDataView.Count; intFindRow++)
                    {
                        if (pvtLeaveTypeDataView[intFindRow]["EARNING_NO"].ToString() == pvtLeaveDataView[intRow]["EARNING_NO"].ToString())
                        {
                            break;
                        }
                    }

                    this.dgvLeaveDataGridView.Rows.Add("",
                                                       "", 
                                                        pvtEmployeeDataView[pvtintFindEmployeeRow]["EMPLOYEE_CODE"].ToString(),
                                                        pvtEmployeeDataView[pvtintFindEmployeeRow]["EMPLOYEE_SURNAME"].ToString(),
                                                        pvtEmployeeDataView[pvtintFindEmployeeRow]["EMPLOYEE_NAME"].ToString(),
                                                        pvtLeaveTypeDataView[intFindRow]["EARNING_DESC"].ToString(),
                                                        pvtLeaveDataView[intRow]["LEAVE_DESC"].ToString(),
                                                        Convert.ToDateTime(pvtLeaveDataView[intRow]["LEAVE_FROM_DATE"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()),
                                                        Convert.ToDateTime(pvtLeaveDataView[intRow]["LEAVE_TO_DATE"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()),
                                                        Convert.ToDouble(pvtLeaveDataView[intRow]["LEAVE_DAYS_DECIMAL"]).ToString("##0.00"),
                                                        Convert.ToDouble(pvtLeaveDataView[intRow]["LEAVE_HOURS_DECIMAL"]).ToString("##0.00"),
                                                        strDayHourOption,
                                                        blnAuthorised,
                                                        Convert.ToDateTime(pvtLeaveDataView[intRow]["LEAVE_FROM_DATE"]).ToString("yyyyMMdd"),
                                                        Convert.ToDateTime(pvtLeaveDataView[intRow]["LEAVE_TO_DATE"]).ToString("yyyyMMdd"),
                                                        intRow.ToString());

                    if (Convert.ToDouble(pvtLeaveDataView[intRow]["LEAVE_DAYS_DECIMAL"]) == 0)
                    {
                        this.dgvLeaveDataGridView.Rows[this.dgvLeaveDataGridView.Rows.Count - 1].HeaderCell.Style = this.ErrorDataGridViewCellStyle;

                        this.dgvLeaveDataGridView.Rows[this.dgvLeaveDataGridView.Rows.Count - 1].ReadOnly = true;
                    }

                    if (pvtLeaveDataView[intRow]["LEAVE_OPTION"].ToString() == "D")
                    {
                        if (Convert.ToInt32(pvtLeaveDataView[intRow]["LEAVE_DAYS_DECIMAL"]) != Convert.ToInt32(pvtLeaveDataView[intRow]["DATE_DIFF_NO_DAYS"]))
                        {
                            dgvLeaveDataGridView[0, this.dgvLeaveDataGridView.Rows.Count - 1].Style = this.LeaveDaysExcludedDataGridViewCellStyle;
                        }
                    }
                    else
                    {
                        dgvLeaveDataGridView[0, this.dgvLeaveDataGridView.Rows.Count - 1].Style = this.HoursOptionDataGridViewCellStyle;
                    }

                    DataView dtvPublicHolidayDataView = new DataView(pvtDataSet.Tables["PublicHoliday"]
                     , "PUBLIC_HOLIDAY_DATE >= '" + Convert.ToDateTime(pvtLeaveDataView[intRow]["LEAVE_FROM_DATE"]).ToString("yyyy-MM-dd") + "' AND PUBLIC_HOLIDAY_DATE <= '" + Convert.ToDateTime(pvtLeaveDataView[intRow]["LEAVE_TO_DATE"]).ToString("yyyy-MM-dd") + "'"
                     , ""
                     , DataViewRowState.CurrentRows);

                    if (dtvPublicHolidayDataView.Count > 0)
                    {
                        dgvLeaveDataGridView[1, this.dgvLeaveDataGridView.Rows.Count - 1].Style = this.PublicHolidayDataGridViewCellStyle;
                    }

                    if (this.pvtstrPayrollType == "W")
                    {
                        if (this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"].ToString() != "")
                        {
                            this.dgvLeaveDataGridView.Rows[this.dgvLeaveDataGridView.Rows.Count - 1].HeaderCell.Style = this.LockedPayrollRunDataGridViewCellStyle;
                            this.dgvLeaveDataGridView.Rows[this.dgvLeaveDataGridView.Rows.Count - 1].ReadOnly = true;

                            blnPayrollRun = true;
                        }
                    }
                    else
                    {
                        if (this.pvtstrPayrollType == "S")
                        {
                            if (this.pvtDataSet.Tables["Company"].Rows[0]["SALARY_RUN_IND"].ToString() == "Y")
                            {
                                this.dgvLeaveDataGridView.Rows[this.dgvLeaveDataGridView.Rows.Count - 1].HeaderCell.Style = this.LockedPayrollRunDataGridViewCellStyle;
                                this.dgvLeaveDataGridView.Rows[this.dgvLeaveDataGridView.Rows.Count - 1].ReadOnly = true;

                                blnPayrollRun = true;
                            }
                        }
                    }
                }

                this.pvtblnLeaveDataGridViewLoaded = true;

                if (this.dgvLeaveDataGridView.Rows.Count > 0)
                {
                    if (blnPayrollRun == false)
                    {
                        this.btnUpdate.Enabled = true;
                    }
                    else
                    {
                        this.btnUpdate.Enabled = false;
                    }

                    this.Set_DataGridView_SelectedRowIndex(this.dgvLeaveDataGridView, intEmployeeNoRow);
                }
                else
                {
                    this.btnUpdate.Enabled = false;
                }

                if (blnFilterApplied == true)
                {
                    this.picPayrollTypeLock.Image = LeaveAuthorisation.Properties.Resources.filter16;
                    this.picAuthoriseLevelLock.Image = LeaveAuthorisation.Properties.Resources.filter16;

                    this.picPayrollTypeLock.Visible = true;
                    this.picAuthoriseLevelLock.Visible = true;
                    this.picLeaveFilterImage.Visible = true;
                   
                    grbFilterApplies.Visible = true;
                }
                else
                {
                    this.picPayrollTypeLock.Visible = false;
                    this.picAuthoriseLevelLock.Visible = false;
                    this.picLeaveFilterImage.Visible = false;
                   
                    grbFilterApplies.Visible = false;
                }
            }

        }

        private void cboOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.pvtblnAuthorisationLevelDataGridViewLoaded == true)
            {
                if (this.dgvAuthorisationLevelDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvAuthorisationLevelDataGridView, pvtintAuthorisationLevelDataGridViewRowIndex);
                }
            }
        }

        private void cboLeaveType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.pvtblnAuthorisationLevelDataGridViewLoaded == true)
            {
                if (this.dgvAuthorisationLevelDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvAuthorisationLevelDataGridView,   pvtintAuthorisationLevelDataGridViewRowIndex);
                }
            }
        }

        private void dgvLeaveDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnLeaveDataGridViewLoaded == true)
            {
                if (pvtintLeaveDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintLeaveDataGridViewRowIndex = e.RowIndex;

                    if (this.btnSave.Enabled == false)
                    {
                        this.Clear_DataGridView(this.dgvLeaveDateDataGridView);

                        int intRow = Convert.ToInt32(dgvLeaveDataGridView[pvtintColLeaveHoldingIndex, e.RowIndex].Value);
                        int intFindPayCategoryTimeDecimalRow = -1;
                        int intFindPublicHolidayRow = -1;
                        string strDayFilter = "";

                        if (pvtLeaveDataView[intRow]["LEAVE_OPTION"].ToString() == "H")
                        {
                            this.lblLeaveDate.Text = dgvLeaveDataGridView[4, e.RowIndex].Value.ToString() + " " + dgvLeaveDataGridView[3, e.RowIndex].Value.ToString() + " (" + dgvLeaveDataGridView[pvtintColFromDate, e.RowIndex].Value.ToString() + ")";
                        }
                        else
                        {
                            this.lblLeaveDate.Text = dgvLeaveDataGridView[4, e.RowIndex].Value.ToString() + " " +  dgvLeaveDataGridView[3, e.RowIndex].Value.ToString() + " (" + dgvLeaveDataGridView[pvtintColFromDate, e.RowIndex].Value.ToString() + "  to  " + dgvLeaveDataGridView[pvtintColToDate, e.RowIndex].Value.ToString() + ")";
                        }

                        //2013-09-20
                        this.lblAmount.Text = Convert.ToDouble(pvtLeaveDataView[intRow]["LEAVE_HOURS_DECIMAL"]).ToString("##0.00");

                        int intFindEmployeeRow = pvtEmployeeDataView.Find(pvtLeaveDataView[intRow]["EMPLOYEE_NO"].ToString());

                        if (pvtEmployeeDataView[intFindEmployeeRow]["LEAVE_PAID_ACCUMULATOR_IND"].ToString() == "1")
                        {
                            //Week Days Only
                            strDayFilter = " AND DAY_NO IN(1,2,3,4,5)";
                        }
                        else
                        {
                            if (pvtEmployeeDataView[intFindEmployeeRow]["LEAVE_PAID_ACCUMULATOR_IND"].ToString() == "2")
                            {
                                //Week Days + Saturday
                                strDayFilter = " AND DAY_NO IN(1,2,3,4,5,6)";
                            }
                        }

                        pvtPayCategoryTimeDecimalDataView = null;
                        pvtPayCategoryTimeDecimalDataView = new DataView(pvtDataSet.Tables["PayCategoryTimeDecimal"],
                        "PAY_CATEGORY_NO = " + pvtEmployeeDataView[pvtintFindEmployeeRow]["PAY_CATEGORY_NO"].ToString() + strDayFilter,
                        "DAY_NO",
                        DataViewRowState.CurrentRows);

                        DateTime dtDateTimeFrom = Convert.ToDateTime(pvtLeaveDataView[intRow]["LEAVE_FROM_DATE"]);
                        DateTime dtDateTimeTo = Convert.ToDateTime(pvtLeaveDataView[intRow]["LEAVE_TO_DATE"]);

                        while (dtDateTimeFrom <= Convert.ToDateTime(pvtLeaveDataView[intRow]["LEAVE_TO_DATE"]))
                        {
                            intFindPublicHolidayRow = pvtPublicHolidayDataView.Find(dtDateTimeFrom.ToString("yyyy-MM-dd"));

                            if (intFindPublicHolidayRow > -1)
                            {
                                this.dgvLeaveDateDataGridView.Rows.Add(dgvLeaveDataGridView[pvtintColLeaveType, e.RowIndex].Value.ToString(),
                                                                       dgvLeaveDataGridView[pvtintColDescription, e.RowIndex].Value.ToString(),
                                                                       dtDateTimeFrom.ToString("dd MMMM yyyy"),
                                                                       dtDateTimeFrom.DayOfWeek.ToString(),
                                                                       "0.00",
                                                                       "0.00",
                                                                       "0.00",
                                                                       dtDateTimeFrom.ToString("yyyyMMdd"));

                                this.dgvLeaveDateDataGridView.Rows[this.dgvLeaveDateDataGridView.Rows.Count - 1].HeaderCell.Style = this.PublicHolidayDataGridViewCellStyle;
                            }
                            else
                            {
                                intFindPayCategoryTimeDecimalRow = pvtPayCategoryTimeDecimalDataView.Find(Convert.ToInt32(dtDateTimeFrom.DayOfWeek));

                                if (intFindPayCategoryTimeDecimalRow == -1)
                                {
                                    this.dgvLeaveDateDataGridView.Rows.Add(dgvLeaveDataGridView[pvtintColLeaveType, e.RowIndex].Value.ToString(),
                                                                           dgvLeaveDataGridView[pvtintColDescription, e.RowIndex].Value.ToString(),
                                                                           dtDateTimeFrom.ToString("dd MMMM yyyy"),
                                                                           dtDateTimeFrom.DayOfWeek.ToString(),
                                                                           "0.00",
                                                                           "0.00",
                                                                           "0.00",
                                                                           dtDateTimeFrom.ToString("yyyyMMdd"));

                                    this.dgvLeaveDateDataGridView.Rows[this.dgvLeaveDateDataGridView.Rows.Count - 1].HeaderCell.Style = this.LeaveDaysExcludedDataGridViewCellStyle;
                                }
                                else
                                {
                                    if (pvtLeaveDataView[intRow]["LEAVE_OPTION"].ToString() == "H")
                                    {
                                        this.dgvLeaveDateDataGridView.Rows.Add(dgvLeaveDataGridView[pvtintColLeaveType, e.RowIndex].Value.ToString(),
                                                                                dgvLeaveDataGridView[pvtintColDescription, e.RowIndex].Value.ToString(),
                                                                                dtDateTimeFrom.ToString("dd MMMM yyyy"),
                                                                                dtDateTimeFrom.DayOfWeek.ToString(),
                                                                                 Convert.ToDouble(pvtLeaveDataView[intRow]["LEAVE_DAYS_DECIMAL"]).ToString("##0.00"),
                                                                                Convert.ToDouble(pvtPayCategoryTimeDecimalDataView[intFindPayCategoryTimeDecimalRow]["TIME_DECIMAL"]).ToString("#0.00"),
                                                                                Convert.ToDouble(pvtLeaveDataView[intRow]["LEAVE_HOURS_DECIMAL"]).ToString("##0.00"),
                                                                                dtDateTimeFrom.ToString("yyyyMMdd"));

                                        this.dgvLeaveDateDataGridView.Rows[this.dgvLeaveDateDataGridView.Rows.Count - 1].HeaderCell.Style = this.HoursOptionDataGridViewCellStyle;
                                    }
                                    else
                                    {

                                        this.dgvLeaveDateDataGridView.Rows.Add(dgvLeaveDataGridView[pvtintColLeaveType, e.RowIndex].Value.ToString(),
                                                                                dgvLeaveDataGridView[pvtintColDescription, e.RowIndex].Value.ToString(),
                                                                                dtDateTimeFrom.ToString("dd MMMM yyyy"),
                                                                                dtDateTimeFrom.DayOfWeek.ToString(),
                                                                                "1.00",
                                                                                Convert.ToDouble(pvtPayCategoryTimeDecimalDataView[intFindPayCategoryTimeDecimalRow]["TIME_DECIMAL"]).ToString("#0.00"),
                                                                                Convert.ToDouble(pvtPayCategoryTimeDecimalDataView[intFindPayCategoryTimeDecimalRow]["TIME_DECIMAL"]).ToString("#0.00"),
                                                                                dtDateTimeFrom.ToString("yyyyMMdd"));
                                    }
                                }
                            }

                            dtDateTimeFrom = dtDateTimeFrom.AddDays(1);
                        }
                    }
                }
            }
        }

        private void btnRemoveFilter_Click(object sender, EventArgs e)
        {
            pvtblnAuthorisationLevelDataGridViewLoaded = false;

            this.rbnAuthorisationNone.Checked = true;
            
            if (this.cboLeaveType.SelectedIndex > 0)
            {
                this.cboLeaveType.SelectedIndex = 0;
            }

            if (this.cboOption.SelectedIndex > 0)
            {
                this.cboOption.SelectedIndex = 0;
            }

            pvtblnAuthorisationLevelDataGridViewLoaded = true;

            cboLeaveType_SelectedIndexChanged(sender, e);

        }

        private void AuthorisationType_Click(object sender, EventArgs e)
        {
            if (this.pvtblnAuthorisationLevelDataGridViewLoaded == true)
            {
                if (this.dgvAuthorisationLevelDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvAuthorisationLevelDataGridView, pvtintAuthorisationLevelDataGridViewRowIndex);
                }
            }
        }

        private void dgvLeaveDateDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 2)
            {
                if (double.Parse(dgvLeaveDateDataGridView[7, e.RowIndex1].Value.ToString()) > double.Parse(dgvLeaveDateDataGridView[7, e.RowIndex2].Value.ToString()))
                {
                    e.SortResult = 1;
                }
                else
                {
                    if (double.Parse(dgvLeaveDateDataGridView[7, e.RowIndex1].Value.ToString()) < double.Parse(dgvLeaveDateDataGridView[7, e.RowIndex2].Value.ToString()))
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

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            this.Text += " - Update";

            this.dgvLeaveDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dgvLeaveDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;

            this.dgvAuthorisationLevelDataGridView.Enabled = false;
            this.dgvPayrollTypeDataGridView.Enabled = false;

            this.cboLeaveType.Enabled = false;
            this.cboOption.Enabled = false;

            Show_Update_Lock_Images();

            this.rbnAuthorised.Enabled = false;
            this.rbnNotAuthorised.Enabled = false;
            this.rbnAuthorisationNone.Enabled = false;

            this.btnRemoveFilter.Enabled = false;

            this.btnUpdate.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;
            this.btnAuthoriseAll.Enabled = true;

            if (this.dgvLeaveDataGridView.Rows.Count > 0)
            {
                dgvLeaveDataGridView.CurrentCell = dgvLeaveDataGridView[11, 0];
            }
        }

        private void Show_Update_Lock_Images()
        {
            this.picPayrollTypeLock.Image = LeaveAuthorisation.Properties.Resources.NewLock16;
            this.picAuthoriseLevelLock.Image = LeaveAuthorisation.Properties.Resources.NewLock16;

            this.picPayrollTypeLock.Visible = true;
            this.picAuthoriseLevelLock.Visible = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (this.Text.LastIndexOf("- Update") != -1)
            {
                this.Text = this.Text.Substring(0, this.Text.IndexOf("- Update") - 1);
            }

            this.dgvLeaveDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvLeaveDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

            this.dgvAuthorisationLevelDataGridView.Enabled = true;
            this.dgvPayrollTypeDataGridView.Enabled = true;

            this.cboLeaveType.Enabled = true;
            this.cboOption.Enabled = true;

            this.picPayrollTypeLock.Visible = false;
            this.picAuthoriseLevelLock.Visible = false;

            this.rbnAuthorised.Enabled = true;
            this.rbnNotAuthorised.Enabled = true;
            this.rbnAuthorisationNone.Enabled = true;

            this.btnRemoveFilter.Enabled = true;

            this.btnUpdate.Enabled = true;

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;
            this.btnAuthoriseAll.Enabled = false;

            if (this.dgvPayrollTypeDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView, pvtintPayrollTypeDataGridViewRowIndex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet TempDataSet = new DataSet();
                TempDataSet.Tables.Add(this.pvtDataSet.Tables["EmployeePayCategoryLevel"].Clone());

                for (int intRow = 0; intRow < this.dgvLeaveDataGridView.Rows.Count; intRow++)
                {
                    DataRow drDataRow = TempDataSet.Tables["EmployeePayCategoryLevel"].NewRow();

                    drDataRow["COMPANY_NO"] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    drDataRow["EMPLOYEE_NO"] = pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintColLeaveHoldingIndex, intRow].Value)]["EMPLOYEE_NO"].ToString();
                    drDataRow["PAY_CATEGORY_NO"] = pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintColLeaveHoldingIndex, intRow].Value)]["PAY_CATEGORY_NO"].ToString();
                    drDataRow["PAY_CATEGORY_TYPE"] = pvtstrPayrollType;
                    drDataRow["EARNING_NO"] = pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintColLeaveHoldingIndex, intRow].Value)]["EARNING_NO"].ToString();
                    drDataRow["LEVEL_NO"] = pvtintAuthoriseLevel;
                    drDataRow["LEAVE_REC_NO"] = pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintColLeaveHoldingIndex, intRow].Value)]["LEAVE_REC_NO"].ToString();

                    if (Convert.ToBoolean(this.dgvLeaveDataGridView[pvtintColAuthorisation, intRow].Value) == true)
                    {
                        drDataRow["AUTHORISED_IND"] = "Y";
                    }
                    else
                    {
                        drDataRow["AUTHORISED_IND"] = "N";
                    }

                    TempDataSet.Tables["EmployeePayCategoryLevel"].Rows.Add(drDataRow);
                }

                //Delete - New Records will be Passed Back for CostCentre
                for (int intRow = 0; intRow < pvtEmployeePayCategoryLevelDataView.Count; intRow++)
                {
                    pvtEmployeePayCategoryLevelDataView[intRow].Delete();

                    intRow -= 1;
                }

                for (int intRow = 0; intRow < pvtAuthorsiseLevelDataView.Count; intRow++)
                {
                    pvtAuthorsiseLevelDataView[intRow].Delete();

                    intRow -= 1;
                }

                //Compress DataSet
                pvtbytCompress = clsISUtilities.Compress_DataSet(TempDataSet);

                object[] objParm = new object[7];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = pvtstrPayrollType;
                objParm[2] = this.pvtintAuthoriseLevel;
                objParm[3] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[4] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[5] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();
                objParm[6] = pvtbytCompress;

                pvtbytCompress = null;
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Update_Record", objParm);

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

        private void btnAuthoriseAll_Click(object sender, EventArgs e)
        {
            for (int intRow = 0; intRow < this.dgvLeaveDataGridView.Rows.Count; intRow++)
            {
                if (this.dgvLeaveDataGridView.Rows[intRow].ReadOnly == false)
                {
                    this.dgvLeaveDataGridView[pvtintColAuthorisation, intRow].Value = true;
                }
            }
        }

        private void dgvLeaveDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == pvtintColFromDate
               | e.Column.Index == pvtintColToDate)
            {
                if (dgvLeaveDataGridView[e.Column.Index + 6, e.RowIndex1].Value.ToString() == "")
                {
                    e.SortResult = -1;
                }
                else
                {
                    if (dgvLeaveDataGridView[e.Column.Index + 6, e.RowIndex2].Value.ToString() == "")
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(dgvLeaveDataGridView[e.Column.Index + 6, e.RowIndex1].Value.ToString()) > double.Parse(dgvLeaveDataGridView[e.Column.Index + 6, e.RowIndex2].Value.ToString()))
                        {
                            e.SortResult = 1;
                        }
                        else
                        {
                            if (double.Parse(dgvLeaveDataGridView[e.Column.Index + 6, e.RowIndex1].Value.ToString()) < double.Parse(dgvLeaveDataGridView[e.Column.Index + 6, e.RowIndex2].Value.ToString()))
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
