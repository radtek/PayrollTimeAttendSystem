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
    public partial class frmLeaveProcess : Form
    {
        clsISUtilities clsISUtilities;

        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;
        private DataView pvtLeaveDataView;
        private DataView pvtEmployeeDataView;
        private DataView pvtLeaveTypeDataView;
        private DataView pvtLeaveTypeFindDataView;
        private DataView pvtLeaveProcessDataView;
        private DataView pvtUpdateDataView;
        private DataView pvtPayCategoryTimeDecimalDataView;
        private DataView pvtEmployeeLeaveTotalsDataView;
        private DataView pvtPublicHolidayDataView;
                                                 
        private byte[] pvtbytCompress;
        private int pvtintFindEmployeeRow;
        private int pvtintFindRow;

        private int pvtintProcessNo = 99;
        private string pvtstrDayHoursInd = "";
        private string pvtstrPayrollType = "W";

        private int pvtintColCode = 3;
        private int pvtintColSurname = 4;
        private int pvtintColName = 5;
        private int pvtintColLeaveType = 6;
        private int pvtintColDescription = 7;
        private int pvtintColFromDate = 8;
        private int pvtintColToDate = 9;
        private int pvtintColDays = 10;
        private int pvtintColHours = 11;
        private int pvtintColOption = 12;
        private int pvtintColProcessOrAuthorise = 13;

        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
        private int pvtintLeaveDataGridViewRowIndex = -1;
       
        private int pvtintLeaveColHoldingIndex = 16;

        DataGridViewCellStyle LockedPayrollRunDataGridViewCellStyle;
        DataGridViewCellStyle AuthorisedDataGridViewCellStyle;
        DataGridViewCellStyle ErrorDataGridViewCellStyle;
        DataGridViewCellStyle LeaveDaysExcludedDataGridViewCellStyle;
        DataGridViewCellStyle PublicHolidayDataGridViewCellStyle;
        DataGridViewCellStyle NormalDataGridViewCellStyle;
        DataGridViewCellStyle HoursOptionDataGridViewCellStyle;
        DataGridViewCellStyle PayOutOptionDataGridViewCellStyle;
        DataGridViewCellStyle LeaveZerorizeDataGridViewCellStyle;
        
        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnLeaveDataGridViewLoaded = false;
        private bool pvtblnEmployeeDataGridViewLoaded = false;

        private bool pvtblnDontFireEvents = false;
        
        public frmLeaveProcess()
        {
            InitializeComponent();
            
            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                dgvEmployeeDataGridView.Height += 38;
                this.dgvEmployeeSelectedDataGridView.Height += 38;

                this.lblLeaveDate.Top += 38;
                this.dgvLeaveDataGridView.Height += 38;

                this.dgvLeaveDateDataGridView.Top += 38;
                this.lblAmount.Top += 38;

                this.grbLegend.Top += 38;
                this.btnLeaveDeleteRec.Top += 38;

                this.Height += 38;
            }
            
            this.lblLeaveCaption.Top = 93;
            this.dgvLeaveDataGridView.Top = 111;

            this.picLeaveLock.Top = this.dgvLeaveDataGridView.Top + 3;
        }

        private void frmLeaveProcess_Load(object sender, System.EventArgs e)
        {
            try
            {
                LockedPayrollRunDataGridViewCellStyle = new DataGridViewCellStyle();
                LockedPayrollRunDataGridViewCellStyle.BackColor = Color.Magenta;
                LockedPayrollRunDataGridViewCellStyle.SelectionBackColor = Color.Magenta;

                AuthorisedDataGridViewCellStyle = new DataGridViewCellStyle();
                AuthorisedDataGridViewCellStyle.BackColor = Color.Cyan;
                AuthorisedDataGridViewCellStyle.SelectionBackColor = Color.Cyan;

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

                PayOutOptionDataGridViewCellStyle = new DataGridViewCellStyle();
                PayOutOptionDataGridViewCellStyle.BackColor = Color.Plum;
                PayOutOptionDataGridViewCellStyle.SelectionBackColor = Color.Plum;

                LeaveZerorizeDataGridViewCellStyle = new DataGridViewCellStyle();
                LeaveZerorizeDataGridViewCellStyle.BackColor = Color.CornflowerBlue;
                LeaveZerorizeDataGridViewCellStyle.SelectionBackColor = Color.CornflowerBlue;

                NormalDataGridViewCellStyle = new DataGridViewCellStyle();
                NormalDataGridViewCellStyle.BackColor = SystemColors.Control;
                NormalDataGridViewCellStyle.SelectionBackColor = SystemColors.Control;
     
                this.dgvEmployeeSelectedDataGridView.SendToBack();
                this.dgvEmployeeDataGridView.SendToBack();

                this.dgvLeaveDataGridView.BringToFront();
                this.dgvLeaveDataGridView.Refresh();

                this.picLeaveLock.BringToFront();

                this.lblLeaveCaption.BringToFront();
                this.lblLeaveCaption.Refresh();

                this.Refresh();

                //Set Hours Cell Formatting to ##0.00
                this.dgvLeaveDataGridView.Columns[pvtintColHours].ValueType = typeof(Decimal);
                this.dgvLeaveDataGridView.Columns[pvtintColHours].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                this.dgvLeaveDataGridView.Columns[pvtintColHours].DefaultCellStyle.Format = "N2";
                this.dgvLeaveDataGridView.Columns[pvtintColHours].DefaultCellStyle.NullValue = 0;

                clsISUtilities = new clsISUtilities(this,"busLeaveProcess");

                clsISUtilities.Create_Calender_Control_From_TextBox(this.txtFromDate);
                clsISUtilities.Create_Calender_Control_From_TextBox(this.txtToDate);

                this.txtPortionOfDay.KeyPress += new System.Windows.Forms.KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);

                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblLeaveCaption.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSelectedEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblLeaveDate.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.dgvPayrollTypeDataGridView.Rows.Add("",
                                                         "Wages");
                this.dgvPayrollTypeDataGridView.Rows.Add("",
                                                         "Salaries");
               
                this.pvtblnPayrollTypeDataGridViewLoaded = true;

                this.Set_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView,0);
     
                object[] objParm = new object[3];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[2] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);
              
                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                if (this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"].ToString() == "Y"
                   | this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"].ToString() == "N")
                {
                    this.dgvPayrollTypeDataGridView[0,0].Style = this.LockedPayrollRunDataGridViewCellStyle;
                }

                if (this.pvtDataSet.Tables["Company"].Rows[0]["SALARY_RUN_IND"].ToString() == "Y")
                {
                    this.dgvPayrollTypeDataGridView[0,1].Style = this.LockedPayrollRunDataGridViewCellStyle;
                }
                
                pvtPublicHolidayDataView = null;
                pvtPublicHolidayDataView = new DataView(pvtDataSet.Tables["PublicHoliday"],
                "",
                "PUBLIC_HOLIDAY_DATE",
                DataViewRowState.CurrentRows);

                Load_CurrentForm_Records();

                Set_Form_For_Read();
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
                    case "dgvPayrollTypeDataGridView":

                        pvtintPayrollTypeDataGridViewRowIndex = -1;
                        this.dgvPayrollTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        private void Set_Form_For_Read()
        {
            for (int intCol = 1; intCol < 11; intCol++)
            {
                this.dgvLeaveDataGridView.Columns[intCol].SortMode = DataGridViewColumnSortMode.Automatic;
            }

            this.lblAmount.Visible = true;

            this.grbLeaveAccumValue.Visible = false;

            this.dgvPayrollTypeDataGridView.Enabled = true;
            this.picPayrollTypeLock.Visible = false;

            this.dgvLeaveDateDataGridView.Enabled = true;
            this.picLeaveDateLock.Visible = false;

            this.dgvEmployeeSelectedDataGridView.Visible = false;
            this.dgvEmployeeDataGridView.Visible = false;

            this.Clear_DataGridView(dgvEmployeeDataGridView);
            this.Clear_DataGridView(dgvEmployeeSelectedDataGridView);

            if (this.Text.IndexOf("- New") > -1)
            {
                this.Text = this.Text.Substring(0, this.Text.IndexOf("- New") - 1);
                this.btnLeaveDeleteRec.Visible = true;
            }
            else
            {
                if (this.Text.IndexOf("- Update") > -1)
                {
                    this.Text = this.Text.Substring(0, this.Text.IndexOf("- Update") - 1);
                }
            }

            this.lblLeaveCaption.Visible = true;
            this.dgvLeaveDataGridView.Visible = true;

            this.grbLegend.Visible = true;
            this.lblLeaveDate.Visible = true;
            this.dgvLeaveDateDataGridView.Visible = true;

            this.dgvLeaveDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvLeaveDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

            //Sticks out Above Leave Spreadsheet
            this.grbFilter.Height = 86;

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
                this.btnNew.Enabled = false;
                this.btnUpdate.Enabled = false;
                this.btnDelete.Enabled = false;
            }
            else
            {
                grbLeaveLock.Visible = false;
                this.btnNew.Enabled = true;

                if (this.pvtDataSet.Tables["Company"].Rows.Count > 0)
                {
                    if (pvtLeaveDataView.Count == 0)
                    {
                        this.btnUpdate.Enabled = false;
                        this.btnDelete.Enabled = false;

                    }
                    else
                    {
                        this.btnUpdate.Enabled = true;
                        this.btnDelete.Enabled = true;
                    }
                }
                else
                {
                    this.btnUpdate.Enabled = false;
                    this.btnDelete.Enabled = false;

                }
            }

            clsISUtilities.Calender_Control_From_TextBox_Disable(this.txtFromDate);
            clsISUtilities.Calender_Control_From_TextBox_Disable(this.txtToDate);

            this.cboLeaveType.Enabled = true;
            this.cboOption.Enabled = true;
            this.cboProcess.Enabled = true;
            this.btnLeaveDeleteRec.Enabled = false;
            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.Refresh();
        }

        private void Set_Form_For_Edit()
        {
            for (int intCol = 1; intCol < 11; intCol++)
            {
                this.dgvLeaveDataGridView.Columns[intCol].SortMode = DataGridViewColumnSortMode.NotSortable; 
            }

            this.dgvPayrollTypeDataGridView.Enabled = false;

            this.picPayrollTypeLock.Image = LeaveProcess.Properties.Resources.NewLock16;

            this.picPayrollTypeLock.Visible = true;

            this.lblLeaveDate.Text = "Days in Leave Range";
            this.lblLeaveDate.BackColor = SystemColors.ControlDark;
            
            this.dgvLeaveDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dgvLeaveDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;
            
            //2013-09-20
            this.lblAmount.Text = "0.00";

            if (this.Text.IndexOf("- New") > -1)
            {
                this.lblAmount.Visible = false;

                this.picLeaveLock.Visible = false;

                this.txtDescription.Text = "";
                this.cboProcess.SelectedIndex = -1;
                this.cboOption.SelectedIndex = -1;

                this.txtPortionOfDay.Text = "";
                this.txtPortionOfDay.Visible = false;

                this.lblFromDate.Visible = true;
                this.lblToDate.Visible = true;

                this.txtFromDate.Text = "";
                this.txtToDate.Text = "";

                this.lblFromDate.Text = "From Date";
                this.lblToDate.Text = "To Date";

                clsISUtilities.Calender_Control_From_TextBox_Enable(this.txtFromDate);
                clsISUtilities.Calender_Control_From_TextBox_Enable(this.txtToDate);

                clsISUtilities.Calender_Control_From_TextBox_SetVisible(this.txtFromDate);
                clsISUtilities.Calender_Control_From_TextBox_SetVisible(this.txtToDate);

                this.dgvEmployeeSelectedDataGridView.Visible = true;
                this.dgvEmployeeDataGridView.Visible = true;
                this.grbFilter.Height = 148;

                this.grbLegend.Visible = false;
                this.lblLeaveCaption.Visible = false;
                this.dgvLeaveDataGridView.Visible = false;

                this.lblLeaveDate.Visible = false;
                this.dgvLeaveDateDataGridView.Visible = false;

                this.dgvEmployeeDataGridView.Columns[2].Width = 220;
                this.dgvEmployeeDataGridView.Columns[3].Visible = false;

                this.dgvEmployeeSelectedDataGridView.Columns[2].Width = 220;
                this.dgvEmployeeSelectedDataGridView.Columns[3].Visible = false;
                
                this.Refresh();
                
                this.cboProcess.Items.Clear();

                for (int intCount = 0; intCount < this.pvtDataSet.Tables["Process"].Rows.Count; intCount++)
                {
                    cboProcess.Items.Add(this.pvtDataSet.Tables["Process"].Rows[intCount]["PROCESS_DESC"].ToString());
                }

                this.cboOption.Items.Clear();

                this.cboOption.Items.Add("Day/s");
                this.cboOption.Items.Add("Hours/s");

                this.cboLeaveType.Items.Clear();

                for (int intIndex = 0; intIndex < pvtLeaveTypeDataView.Count; intIndex++)
                {
                    this.cboLeaveType.Items.Add(pvtLeaveTypeDataView[intIndex]["EARNING_DESC"].ToString());
                }

                this.btnLeaveDeleteRec.Visible = false;
            }
            else
            {
                this.dgvLeaveDateDataGridView.Enabled = false;
                this.picLeaveDateLock.Visible = true;

                this.Clear_DataGridView(dgvLeaveDateDataGridView);

                this.cboLeaveType.Enabled = false;
                this.cboProcess.Enabled = false;
                this.cboOption.Enabled = false;
                this.btnLeaveDeleteRec.Enabled = true;
            }

            this.btnNew.Enabled = false;
            this.btnUpdate.Enabled = false;
            this.btnDelete.Enabled = false;
            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void Load_CurrentForm_Records()
        {
            if (this.pvtDataSet.Tables["Company"].Rows.Count > 0)
            {
                pvtLeaveTypeFindDataView = null;
                pvtLeaveTypeFindDataView = new DataView(pvtDataSet.Tables["LeaveType"],
                   "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND PAY_CATEGORY_TYPE = '" + this.pvtstrPayrollType + "'",
                   "EARNING_NO",
                   DataViewRowState.CurrentRows);

                this.Clear_DataGridView(this.dgvLeaveDateDataGridView);
                this.Clear_DataGridView(this.dgvLeaveDataGridView);

                this.lblLeaveDate.Text = "Days in Leave Range";
                               
                this.cboProcess.Items.Clear();

                string strComboList = "";
                this.cboProcess.Items.Add("All");

                for (int intCount = 0; intCount < this.pvtDataSet.Tables["Process"].Rows.Count; intCount++)
                {
                    if (strComboList == "")
                    {
                        strComboList = this.pvtDataSet.Tables["Process"].Rows[intCount]["PROCESS_DESC"].ToString();
                    }
                    else
                    {
                        strComboList += "|" + this.pvtDataSet.Tables["Process"].Rows[intCount]["PROCESS_DESC"].ToString();
                    }

                    cboProcess.Items.Add(this.pvtDataSet.Tables["Process"].Rows[intCount]["PROCESS_DESC"].ToString());
                }
                
                cboProcess.SelectedIndex = 0;
                
                this.cboOption.Items.Clear();

                this.cboOption.Items.Add("All");
                this.cboOption.Items.Add("Day/s");
                this.cboOption.Items.Add("Hours/s");

                if (pvtstrPayrollType == "W")
                {
                    this.cboOption.Items.Add("Payout");
                }

                cboOption.SelectedIndex = 0;

                pvtLeaveTypeDataView = null;
                pvtLeaveTypeDataView = new DataView(pvtDataSet.Tables["LeaveType"],
                   "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                   "",
                   DataViewRowState.CurrentRows);

                //grbLeaveLock.Visible = false;
                this.cboLeaveType.Items.Clear();

                this.cboLeaveType.Items.Add("All");

                for (int intIndex = 0; intIndex < pvtLeaveTypeDataView.Count; intIndex++)
                {
                    this.cboLeaveType.Items.Add(pvtLeaveTypeDataView[intIndex]["EARNING_DESC"].ToString());
                }

                this.cboLeaveType.SelectedIndex = 0;
            }
        }

        private void Load_Leave()
        {
            pvtEmployeeDataView = null;
            pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
               "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND PAY_CATEGORY_TYPE = '" + this.pvtstrPayrollType + "'",
               "EMPLOYEE_NO",
               DataViewRowState.CurrentRows);

            string strFilter = "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND PAY_CATEGORY_TYPE = '" + this.pvtstrPayrollType + "'";

            if (this.cboLeaveType.SelectedIndex != 0)
            {
                strFilter += " AND EARNING_NO = " + pvtLeaveTypeDataView[this.cboLeaveType.SelectedIndex - 1]["EARNING_NO"].ToString();
            }
            
            pvtLeaveDataView = null;
            pvtLeaveDataView = new DataView(pvtDataSet.Tables["Leave"],
                strFilter,
                "",
                DataViewRowState.CurrentRows | DataViewRowState.Deleted);

            this.Clear_DataGridView(this.dgvLeaveDataGridView);
            this.Clear_DataGridView(this.dgvLeaveDateDataGridView);

            //2013-09-20
            this.lblAmount.Text = "0.00";

            pvtblnLeaveDataGridViewLoaded = false;

            string strDayHourOption = "";
            string strProcessDesc = "";
            int intLeaveIndex = 0;
                      
            for (int intIndex = 0; intIndex < pvtLeaveDataView.Count; intIndex++)
            {
                if (pvtintProcessNo != 99)
                {
                    if (Convert.ToInt32(pvtLeaveDataView[intIndex]["PROCESS_NO"]) != pvtintProcessNo)
                    {
                        continue;
                    }
                }

                if (this.pvtstrDayHoursInd != "")
                {
                    if (pvtLeaveDataView[intIndex]["LEAVE_OPTION"].ToString() != pvtstrDayHoursInd)
                    {
                        continue;
                    }
                }
               
                pvtintFindEmployeeRow = pvtEmployeeDataView.Find(pvtLeaveDataView[intIndex]["EMPLOYEE_NO"].ToString());

                pvtintFindRow = pvtLeaveTypeFindDataView.Find(pvtLeaveDataView[intIndex]["EARNING_NO"].ToString());

                if (pvtLeaveDataView[intIndex]["LEAVE_OPTION"].ToString() == "P")
                {
                    strDayHourOption = "Payout";
                }
                else
                {
                    if (pvtLeaveDataView[intIndex]["LEAVE_OPTION"].ToString() == "D")
                    {
                        strDayHourOption = "Day/s";
                    }
                    else
                    {
                        if (pvtLeaveDataView[intIndex]["LEAVE_OPTION"].ToString() == "H")
                        {
                            strDayHourOption = "Hour/s";
                        }
                        else
                        {
                            strDayHourOption = "Zerorize";
                        }
                    }
                }
                
                if (pvtLeaveDataView[intIndex]["PROCESS_NO"] != System.DBNull.Value)
                {
                    pvtLeaveProcessDataView = null;
                    pvtLeaveProcessDataView = new DataView(pvtDataSet.Tables["Process"],
                    "PROCESS_NO = " + pvtLeaveDataView[intIndex]["PROCESS_NO"].ToString(),
                    "",
                    DataViewRowState.CurrentRows);

                    if (pvtLeaveProcessDataView.Count > 0)
                    {
                        strProcessDesc = pvtLeaveProcessDataView[0]["PROCESS_DESC"].ToString();
                    }
                    else
                    {
                        strProcessDesc = "";
                    }
                }

                this.dgvLeaveDataGridView.Rows.Add("",
                                                   "",
                                                   "", 
                                                   pvtEmployeeDataView[pvtintFindEmployeeRow]["EMPLOYEE_CODE"].ToString(),
                                                   pvtEmployeeDataView[pvtintFindEmployeeRow]["EMPLOYEE_SURNAME"].ToString(),
                                                   pvtEmployeeDataView[pvtintFindEmployeeRow]["EMPLOYEE_NAME"].ToString(),
                                                   pvtLeaveTypeFindDataView[pvtintFindRow]["EARNING_DESC"].ToString(),
                                                   pvtLeaveDataView[intIndex]["LEAVE_DESC"].ToString(),
                                                   Convert.ToDateTime(pvtLeaveDataView[intIndex]["LEAVE_FROM_DATE"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()),
                                                   Convert.ToDateTime(pvtLeaveDataView[intIndex]["LEAVE_TO_DATE"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()),
                                                   Convert.ToDouble(pvtLeaveDataView[intIndex]["LEAVE_DAYS_DECIMAL"]).ToString("##0.00"),
                                                   Convert.ToDouble(pvtLeaveDataView[intIndex]["LEAVE_HOURS_DECIMAL"]).ToString("##0.00"),
                                                   strDayHourOption,
                                                   strProcessDesc,
                                                   Convert.ToDateTime(pvtLeaveDataView[intIndex]["LEAVE_FROM_DATE"]).ToString("yyyyMMdd"),
                                                   Convert.ToDateTime(pvtLeaveDataView[intIndex]["LEAVE_TO_DATE"]).ToString("yyyyMMdd"),
                                                   intIndex.ToString());

                if (pvtLeaveDataView[intIndex]["LEAVE_OPTION"].ToString() != "P"
                &&  pvtLeaveDataView[intIndex]["LEAVE_OPTION"].ToString() != "Z"
                && Convert.ToDouble(pvtLeaveDataView[intIndex]["LEAVE_DAYS_DECIMAL"]) == 0)
                {
                    this.dgvLeaveDataGridView[0,this.dgvLeaveDataGridView.Rows.Count - 1].Style = this.ErrorDataGridViewCellStyle;
                }

                if (pvtLeaveDataView[intIndex]["LEAVE_OPTION"].ToString() == "P"
                || pvtLeaveDataView[intIndex]["LEAVE_OPTION"].ToString() == "Z")
                {
                    if (pvtLeaveDataView[intIndex]["LEAVE_OPTION"].ToString() == "P")
                    {
                        dgvLeaveDataGridView[1, this.dgvLeaveDataGridView.Rows.Count - 1].Style = this.PayOutOptionDataGridViewCellStyle;
                    }
                    else
                    {
                        dgvLeaveDataGridView[1, this.dgvLeaveDataGridView.Rows.Count - 1].Style = this.LeaveZerorizeDataGridViewCellStyle;
                    }

                    dgvLeaveDataGridView[pvtintColFromDate, this.dgvLeaveDataGridView.Rows.Count - 1].ReadOnly = true;
                    dgvLeaveDataGridView[pvtintColToDate, this.dgvLeaveDataGridView.Rows.Count - 1].ReadOnly = true;
                    dgvLeaveDataGridView[pvtintColHours, this.dgvLeaveDataGridView.Rows.Count - 1].ReadOnly = true;
                }
                else
                {
                    if (pvtLeaveDataView[intIndex]["LEAVE_OPTION"].ToString() == "D")
                    {
                        //UnLock LEAVE_TO_DATE Cell
                        dgvLeaveDataGridView[pvtintColToDate, this.dgvLeaveDataGridView.Rows.Count - 1].ReadOnly = false;

                        //lock LEAVE_HOURS_DECIMAL Cell
                        dgvLeaveDataGridView[pvtintColHours, this.dgvLeaveDataGridView.Rows.Count - 1].ReadOnly = true;

                        if (Convert.ToInt32(pvtLeaveDataView[intIndex]["LEAVE_DAYS_DECIMAL"]) != Convert.ToInt32(pvtLeaveDataView[intIndex]["DATE_DIFF_NO_DAYS"]))
                        {
                            dgvLeaveDataGridView[1, this.dgvLeaveDataGridView.Rows.Count - 1].Style = this.LeaveDaysExcludedDataGridViewCellStyle;
                        }
                    }
                    else
                    {
                        //Lock LEAVE_TO_DATE Cell
                        dgvLeaveDataGridView[pvtintColToDate, this.dgvLeaveDataGridView.Rows.Count - 1].ReadOnly = true;

                        //Unlock LEAVE_HOURS_DECIMAL Cell
                        dgvLeaveDataGridView[pvtintColHours, this.dgvLeaveDataGridView.Rows.Count - 1].ReadOnly = false;

                        dgvLeaveDataGridView[1, this.dgvLeaveDataGridView.Rows.Count - 1].Style = this.HoursOptionDataGridViewCellStyle;
                    }
                }

                if (pvtLeaveDataView[intIndex]["LEAVE_OPTION"].ToString() != "P")
                {
                    DataView dtvPublicHolidayDataView = new DataView(pvtDataSet.Tables["PublicHoliday"]
                   , "PUBLIC_HOLIDAY_DATE >= '" + Convert.ToDateTime(pvtLeaveDataView[intIndex]["LEAVE_FROM_DATE"]).ToString("yyyy-MM-dd") + "' AND PUBLIC_HOLIDAY_DATE <= '" + Convert.ToDateTime(pvtLeaveDataView[intIndex]["LEAVE_TO_DATE"]).ToString("yyyy-MM-dd") + "'"
                   , ""
                   , DataViewRowState.CurrentRows);

                    if (dtvPublicHolidayDataView.Count > 0)
                    {
                        dgvLeaveDataGridView[2, this.dgvLeaveDataGridView.Rows.Count - 1].Style = this.PublicHolidayDataGridViewCellStyle;
                    }
                }
              
                if (this.pvtstrPayrollType == "W")
                {
                    if (this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"].ToString() != "")
                    {
                        this.dgvLeaveDataGridView[0,this.dgvLeaveDataGridView.Rows.Count - 1].Style = this.LockedPayrollRunDataGridViewCellStyle;
                        this.dgvLeaveDataGridView.Rows[this.dgvLeaveDataGridView.Rows.Count - 1].ReadOnly = true;
                    }
                    else
                    {
                        if (pvtLeaveDataView[intIndex]["AUTHORISED_IND"].ToString() == "Y")
                        {
                            this.dgvLeaveDataGridView[0,this.dgvLeaveDataGridView.Rows.Count - 1].Style = this.AuthorisedDataGridViewCellStyle;
                            this.dgvLeaveDataGridView.Rows[this.dgvLeaveDataGridView.Rows.Count - 1].ReadOnly = true;
                        }
                    }
                }
                else
                {
                    if (this.pvtDataSet.Tables["Company"].Rows[0]["SALARY_RUN_IND"].ToString() == "Y")
                    {
                        this.dgvLeaveDataGridView[0,this.dgvLeaveDataGridView.Rows.Count - 1].Style = this.LockedPayrollRunDataGridViewCellStyle;
                        this.dgvLeaveDataGridView.Rows[this.dgvLeaveDataGridView.Rows.Count - 1].ReadOnly = true;
                    }
                }
            }

            pvtblnLeaveDataGridViewLoaded = true;

            if (dgvLeaveDataGridView.Rows.Count > 0)
            {
                if (this.Text.IndexOf(" New") == -1)
                {
                    //NB This is Here To Cater for Rollback When Payroll is Opened and This Screen is Already Open

                    if (this.pvtstrPayrollType == "W")
                    {
                        if (this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"].ToString() == "")
                        {
                            this.btnUpdate.Enabled = true;
                            this.btnDelete.Enabled = true;
                        }
                    }
                    else
                    {
                        if (this.pvtDataSet.Tables["Company"].Rows[0]["SALARY_RUN_IND"].ToString() == "Y")
                        {
                            this.btnUpdate.Enabled = true;
                            this.btnDelete.Enabled = true;
                        }
                    }
                }

                this.Set_DataGridView_SelectedRowIndex(dgvLeaveDataGridView,intLeaveIndex);
            }
            else
            {
                this.lblLeaveDate.Text = "Days in Leave Range";
                
                this.btnUpdate.Enabled = false;
                this.btnDelete.Enabled = false;
            }
        }

        public void btnUpdate_Click(object sender, System.EventArgs e)
        {
            this.Text += " - Update";

            Set_Form_For_Edit();

            if (this.dgvLeaveDataGridView.Rows.Count > 0)
            {
                dgvLeaveDataGridView.CurrentCell = dgvLeaveDataGridView[pvtintColDescription, 0];
                dgvLeaveDataGridView.Focus();
            }
        }

        public void btnCancel_Click(object sender, System.EventArgs e)
        {
            if (this.pvtDataSet.Tables["Company"].Rows.Count > 0)
            {
                this.pvtDataSet.RejectChanges();

                Set_Form_For_Read();

                Load_CurrentForm_Records();
            }
        }

        public void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (this.Text.IndexOf("- New") > -1)
                {
                    Int16 int16LeaveType;
                    string strLeaveDescription;
                    Int16 int16ProcessNo;

                    DateTime dtFromDate = DateTime.Now;
                    DateTime dtToDate = DateTime.Now;

                    if (this.cboLeaveType.SelectedIndex == -1)
                    {
                        CustomMessageBox.Show("Select Leave Type.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.cboLeaveType.Focus();
                        return;
                    }
                    else
                    {
                        int16LeaveType = Convert.ToInt16(pvtLeaveTypeDataView[this.cboLeaveType.SelectedIndex]["EARNING_NO"]);
                    }

                    if (this.cboProcess.SelectedIndex == -1)
                    {
                        CustomMessageBox.Show("Select Process.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.cboProcess.Focus();
                        return;
                    }
                    else
                    {
                        int16ProcessNo = Convert.ToInt16(this.pvtDataSet.Tables["Process"].Rows[this.cboProcess.SelectedIndex]["PROCESS_NO"]);
                    }

                    if (this.cboOption.SelectedIndex == -1)
                    {
                        CustomMessageBox.Show("Select Option.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.cboOption.Focus();
                        return;
                    }

                    if (this.txtDescription.Text.Trim() == "")
                    {
                        CustomMessageBox.Show("Capture Leave Description.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.txtDescription.Focus();
                        return;
                    }
                    else
                    {
                        strLeaveDescription = this.txtDescription.Text.Trim();
                    }

                    if (this.cboOption.SelectedIndex != 2)
                    { 
                        if (this.txtFromDate.Text.Trim() == "")
                        {
                            CustomMessageBox.Show("Capture " + this.lblFromDate.Text + ".", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                        else
                        {
                            dtFromDate = DateTime.ParseExact(this.txtFromDate.Text, AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);
                        }
                    }

                    if (this.cboOption.SelectedIndex == 0)
                    {
                        if (this.txtToDate.Text.Trim() == "")
                        {
                            CustomMessageBox.Show("Capture To Date.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                        else
                        {
                            dtToDate = DateTime.ParseExact(this.txtToDate.Text, AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);
                        }

                        if (dtFromDate > dtToDate)
                        {
                            CustomMessageBox.Show("From Date cannot be Greater Than To Date.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                    }
                    else
                    {
                        if (this.cboOption.SelectedIndex == 1)
                        {
                            if (this.txtPortionOfDay.Text.Replace(".", "") == "")
                            {
                                CustomMessageBox.Show("Capture " + this.lblToDate.Text + ".", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                this.txtPortionOfDay.Focus();
                                return;
                            }
                            else
                            {
                                if (Convert.ToDouble(this.txtPortionOfDay.Text) > 0
                                    && Convert.ToDouble(this.txtPortionOfDay.Text) < 24)
                                {
                                }
                                else
                                {
                                    CustomMessageBox.Show(this.lblToDate.Text + " value must be Greater than 0 and less than 24.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    this.txtPortionOfDay.Focus();
                                    return;
                                }
                            }
                        }
                    }

                    if (this.dgvEmployeeSelectedDataGridView.Rows.Count == 0)
                    {
                        CustomMessageBox.Show("Select Employee/s.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.btnAdd.Focus();
                        return;
                    }

                    pvtTempDataSet = null;
                    pvtTempDataSet = new DataSet();

                    string strEmployeeNos = "";
                    string strEmployeePayCategoryNos = "";

                    for (int intRow = 0; intRow < this.dgvEmployeeSelectedDataGridView.Rows.Count; intRow++)
                    {
                        if (intRow == 0)
                        {
                            strEmployeeNos = this.dgvEmployeeSelectedDataGridView[4,intRow].Value.ToString();
                            strEmployeePayCategoryNos = this.dgvEmployeeSelectedDataGridView[5, intRow].Value.ToString();
                        }
                        else
                        {
                            strEmployeeNos += "|" + this.dgvEmployeeSelectedDataGridView[4,intRow].Value.ToString();
                            strEmployeePayCategoryNos += "|" + this.dgvEmployeeSelectedDataGridView[5, intRow].Value.ToString();
                        }
                    }

                    object[] objParm = new object[13];
                    objParm[0] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                    objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[2] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[3] = strEmployeeNos;
                    objParm[4] = strEmployeePayCategoryNos;
                    objParm[5] = int16LeaveType;
                    objParm[6] = this.pvtstrPayrollType;
                    objParm[7] = strLeaveDescription;
                    objParm[8] = int16ProcessNo;
                    objParm[9] = this.cboOption.SelectedItem.ToString().Substring(0,1);
                    objParm[10] = dtFromDate;
                    objParm[11] = dtToDate;
                    objParm[12] = this.txtPortionOfDay.Text;

                    pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Insert_Leave_Records", objParm, true);

                    pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                    if (pvtTempDataSet.Tables["CompanyCheck"].Rows[0]["RUN_IND"].ToString() == "")
                    {
                        pvtTempDataSet.Tables.Remove("CompanyCheck");

                        pvtDataSet.Merge(pvtTempDataSet);
                    }
                    else
                    {
                        CustomMessageBox.Show("Action Cancelled - Payroll Run in Progress.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

                        if (this.pvtstrPayrollType == "W")
                        {
                            this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"] = "Y";
                        }
                        else
                        {
                            this.pvtDataSet.Tables["Company"].Rows[0]["SALARY_RUN_IND"] = "Y";
                        }
                    }
                }
                else
                {
                    DateTime dtFromDateTime = DateTime.Now;
                    DateTime dtToDateTime = DateTime.Now;
                  
                    //Check for Errors In Spreadsheet
                    for (int intRow = 0; intRow < this.dgvLeaveDataGridView.Rows.Count; intRow++)
                    {
                        if (this.dgvLeaveDataGridView[pvtintColDescription, intRow].Value == null)
                        {
                            CustomMessageBox.Show("Enter Description.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                            dgvLeaveDataGridView.CurrentCell = dgvLeaveDataGridView[pvtintColDescription, intRow];
                            this.dgvLeaveDataGridView.Focus();
                            return;
                        }
                        else
                        {
                            if (this.dgvLeaveDataGridView[pvtintColDescription, intRow].Value.ToString().Trim() == "")
                            {
                                CustomMessageBox.Show("Enter Description.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                                dgvLeaveDataGridView.CurrentCell = dgvLeaveDataGridView[pvtintColDescription, intRow];
                                this.dgvLeaveDataGridView.Focus();
                                return;
                            }
                        }

                        dtFromDateTime = DateTime.ParseExact(this.dgvLeaveDataGridView[pvtintColFromDate, intRow].Value.ToString().Substring(0, 10), AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);
                        dtToDateTime = DateTime.ParseExact(this.dgvLeaveDataGridView[pvtintColToDate, intRow].Value.ToString().Substring(0, 10), AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);

                        if (dtToDateTime < dtFromDateTime)
                        {
                            CustomMessageBox.Show("To Date Cannot be Less Than From Date.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                            dgvLeaveDataGridView.CurrentCell = dgvLeaveDataGridView[pvtintColToDate, intRow];
                            this.dgvLeaveDataGridView.Focus();
                            return;
                        }

                        //NB Value Will Always be Numeric due To dgvLeaveDataGridView_DataError
                        if (Convert.ToDouble(this.dgvLeaveDataGridView[pvtintColHours, intRow].Value) == 0)
                        {
                            if (this.dgvLeaveDataGridView[pvtintColOption, intRow].Value.ToString().Substring(0, 1) == "H")
                            {
                                CustomMessageBox.Show("Value must be Greater than 0.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                                dgvLeaveDataGridView.CurrentCell = dgvLeaveDataGridView[pvtintColHours, intRow];
                                this.dgvLeaveDataGridView.Focus();
                                return;
                            }
                        }
                    }
                   
                    pvtUpdateDataView = null;
                    pvtUpdateDataView = new DataView(pvtDataSet.Tables["Leave"],
                        "",
                        "",
                        DataViewRowState.Deleted | DataViewRowState.ModifiedCurrent | DataViewRowState.Added);

                    pvtTempDataSet = null;
                    pvtTempDataSet = new DataSet();

                    DataTable myDataTable = this.pvtDataSet.Tables["Leave"].Clone();

                    pvtTempDataSet.Tables.Add(myDataTable);

                    for (int intRow = 0; intRow < pvtUpdateDataView.Count; intRow++)
                    {
                        pvtTempDataSet.Tables["Leave"].ImportRow(pvtUpdateDataView[intRow].Row);
                    }

                    if (pvtTempDataSet.Tables["Leave"].Rows.Count > 0)
                    {
                        //Compress DataSet
                        pvtbytCompress = clsISUtilities.Compress_DataSet(pvtTempDataSet);
                        
                        object[] objParm = new object[5];

                        objParm[0] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                        objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                        objParm[2] = this.pvtstrPayrollType;
                        objParm[3] = pvtbytCompress;
                        objParm[4] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                      
                        int intReturnCode = (int)clsISUtilities.DynamicFunction("Update_Record", objParm, true);

                        if (intReturnCode == 1)
                        {
                            this.pvtDataSet.RejectChanges();

                            CustomMessageBox.Show("Action Cancelled - Payroll Run in Progress.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

                            if (this.pvtstrPayrollType == "W")
                            {
                                this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"] = "Y";
                            }
                            else
                            {
                                if (this.pvtstrPayrollType == "S")
                                {
                                    this.pvtDataSet.Tables["Company"].Rows[0]["SALARY_RUN_IND"] = "Y";
                                }
                                else
                                {
                                    this.pvtDataSet.Tables["Company"].Rows[0]["TIME_ATTENDANCE_RUN_IND"] = "Y";
                                }
                            }
                        }
                    }
                }

                this.pvtDataSet.AcceptChanges();
              
                btnCancel_Click(sender, e);
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        public void btnNew_Click(object sender, EventArgs e)
        {
            this.Text += " - New";

            Set_Form_For_Edit();

            this.Clear_DataGridView(dgvEmployeeDataGridView);
            this.Clear_DataGridView(this.dgvEmployeeSelectedDataGridView);

            this.pvtblnEmployeeDataGridViewLoaded = false;

            //NB PAY_CATEGORY_NO is Default For Employee and Used in Insert For Leave Days Accumulated 
            for (int intIndex = 0; intIndex < pvtEmployeeDataView.Count; intIndex++)
            {
                this.dgvEmployeeDataGridView.Rows.Add(pvtEmployeeDataView[intIndex]["EMPLOYEE_CODE"].ToString(),
                                                      pvtEmployeeDataView[intIndex]["EMPLOYEE_SURNAME"].ToString(),
                                                      pvtEmployeeDataView[intIndex]["EMPLOYEE_NAME"].ToString(),
                                                      "",
                                                      pvtEmployeeDataView[intIndex]["EMPLOYEE_NO"].ToString(),
                                                      pvtEmployeeDataView[intIndex]["PAY_CATEGORY_NO"].ToString());
            }

            this.pvtblnEmployeeDataGridViewLoaded = true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvEmployeeDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView)];

                this.dgvEmployeeDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvEmployeeSelectedDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvEmployeeSelectedDataGridView.CurrentCell = this.dgvEmployeeSelectedDataGridView[0, this.dgvEmployeeSelectedDataGridView.Rows.Count - 1];
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (this.dgvEmployeeSelectedDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvEmployeeSelectedDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvEmployeeSelectedDataGridView)];

                this.dgvEmployeeSelectedDataGridView.Rows.Remove(myDataGridViewRow);

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

            if (this.dgvEmployeeSelectedDataGridView.Rows.Count > 0)
            {
                btnRemove_Click(null, null);

                goto btnRemoveAll_Click_Continue;
            }
        }

        private void cboLeaveType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pvtblnDontFireEvents == false)
            {
                if (this.btnSave.Enabled == false)
                {
                    Load_Leave();
                }
                else
                {
                    if (pvtLeaveTypeDataView[this.cboLeaveType.SelectedIndex]["EARNING_NO"].ToString() == "200"
                    | pvtLeaveTypeDataView[this.cboLeaveType.SelectedIndex]["EARNING_NO"].ToString() == "201")
                    {
                        lblLeaveAccumValue.Text = "This is the ";

                        if (pvtLeaveTypeDataView[this.cboLeaveType.SelectedIndex]["EARNING_NO"].ToString() == "200")
                        {
                            lblLeaveAccumValue.Text += "Normal Leave";
                        }
                        else
                        {
                            lblLeaveAccumValue.Text += "Sick Leave";
                        }

                        lblLeaveAccumValue.Text += " Balance for each Employee as at his/her Last Payroll Run.";

                        grbLeaveAccumValue.Visible = true;

                        for (int intIndex = 0; intIndex < this.dgvEmployeeDataGridView.Rows.Count; intIndex++)
                        {
                            pvtEmployeeLeaveTotalsDataView = null;
                            pvtEmployeeLeaveTotalsDataView = new DataView(pvtDataSet.Tables["EmployeeLeaveTotals"],
                              "PAY_CATEGORY_TYPE = '" + this.pvtstrPayrollType + "' AND EMPLOYEE_NO = " + this.dgvEmployeeDataGridView[4, intIndex].Value.ToString() + " AND EARNING_NO = " + pvtLeaveTypeDataView[this.cboLeaveType.SelectedIndex]["EARNING_NO"].ToString(),
                                "",
                            DataViewRowState.CurrentRows);

                            if (pvtEmployeeLeaveTotalsDataView.Count > 0)
                            {
                                this.dgvEmployeeDataGridView[3, intIndex].Value = Convert.ToDouble(pvtEmployeeLeaveTotalsDataView[0]["TOTAL_LEAVE_DAYS"]).ToString("##0.00");
                            }
                            else
                            {
                                this.dgvEmployeeDataGridView[3, intIndex].Value = "0.00";
                            }
                        }

                        this.dgvEmployeeDataGridView.Columns[2].Width = 180;
                        this.dgvEmployeeDataGridView.Columns[3].Visible = true;

                        for (int intIndex = 0; intIndex < this.dgvEmployeeSelectedDataGridView.Rows.Count; intIndex++)
                        {
                            pvtEmployeeLeaveTotalsDataView = null;
                            pvtEmployeeLeaveTotalsDataView = new DataView(pvtDataSet.Tables["EmployeeLeaveTotals"],
                              "PAY_CATEGORY_TYPE = '" + this.pvtstrPayrollType + "' AND EMPLOYEE_NO = " + this.dgvEmployeeSelectedDataGridView[4, intIndex].Value.ToString() + " AND EARNING_NO = " + pvtLeaveTypeDataView[this.cboLeaveType.SelectedIndex]["EARNING_NO"].ToString(),
                                "",
                            DataViewRowState.CurrentRows);

                            if (pvtEmployeeLeaveTotalsDataView.Count > 0)
                            {
                                this.dgvEmployeeSelectedDataGridView[3, intIndex].Value = Convert.ToDouble(pvtEmployeeLeaveTotalsDataView[0]["TOTAL_LEAVE_DAYS"]).ToString("##0.00");
                            }
                            else
                            {
                                this.dgvEmployeeSelectedDataGridView[3, intIndex].Value = "0.00";
                            }
                        }

                        this.dgvEmployeeSelectedDataGridView.Columns[2].Width = 180;
                        this.dgvEmployeeSelectedDataGridView.Columns[3].Visible = true;
                    }
                    else
                    {
                        grbLeaveAccumValue.Visible = false;

                        this.dgvEmployeeDataGridView.Columns[2].Width = 220;
                        this.dgvEmployeeDataGridView.Columns[3].Visible = false;

                        this.dgvEmployeeSelectedDataGridView.Columns[2].Width = 220;
                        this.dgvEmployeeSelectedDataGridView.Columns[3].Visible = false;
                    }

                    if (pvtstrPayrollType == "W")
                    {
                        this.cboOption.Items.Clear();

                        this.cboOption.Items.Add("Day/s");
                        this.cboOption.Items.Add("Hours/s");

                        if (pvtLeaveTypeDataView[this.cboLeaveType.SelectedIndex]["EARNING_NO"].ToString() == "200")
                        {
                            this.cboOption.Items.Add("Payout All Leave Due");
                        }
                        else
                        {
                            this.cboOption.Items.Add("Zerorize Leave Balance");
                        }
                    }
                }
            }
        }
       
        private void btnLeaveDeleteRec_Click(object sender, EventArgs e)
        {
            if (this.dgvLeaveDataGridView.Rows.Count > 0)
            {
                if (this.dgvLeaveDataGridView.Rows[pvtintLeaveDataGridViewRowIndex].ReadOnly == true)
                {
                    CustomMessageBox.Show("Row is Locked - It cannot be Deleted.",
                    this.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                }
                else
                {
                    pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, pvtintLeaveDataGridViewRowIndex].Value)].Delete();

                    this.dgvLeaveDataGridView.Rows.RemoveAt(pvtintLeaveDataGridViewRowIndex);
                }
            }
        }

        private void cboOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pvtblnDontFireEvents == false)
            {
                if (this.btnSave.Enabled == false)
                {
                    if (this.cboLeaveType.SelectedIndex > -1
                        & this.cboProcess.SelectedIndex > -1
                        & this.cboOption.SelectedIndex > -1)
                    {
                        if (cboOption.SelectedIndex == 0)
                        {
                            pvtstrDayHoursInd = "";
                        }
                        else
                        {
                            pvtstrDayHoursInd = cboOption.SelectedItem.ToString().Substring(0, 1);
                        }

                        cboLeaveType_SelectedIndexChanged(sender, e);
                    }
                }
                else
                {
                    this.lblFromDate.Visible = true;
                    this.lblToDate.Visible = true;
                    clsISUtilities.Calender_Control_From_TextBox_SetVisible(this.txtFromDate);

                    if (cboOption.SelectedIndex < 1)
                    {
                        this.lblFromDate.Text = "From Date";
                        this.lblToDate.Text = "To Date";

                        this.txtPortionOfDay.Visible = false;

                        clsISUtilities.Calender_Control_From_TextBox_SetVisible(this.txtToDate);
                    }
                    else
                    {
                        if (cboOption.SelectedIndex == 1)
                        {
                            this.lblFromDate.Text = "Date";
                            this.lblToDate.Text = "Hours";

                            this.txtToDate.Text = "";

                            this.txtPortionOfDay.Text = "";
                            this.txtPortionOfDay.Visible = true;

                            clsISUtilities.Calender_Control_From_TextBox_SetInvisible(this.txtToDate);
                        }
                        else
                        {
                            if (cboOption.SelectedIndex == 2)
                            {
                                this.lblFromDate.Visible = false;
                                this.lblToDate.Visible = false;
                                
                                this.txtPortionOfDay.Visible = false;

                                clsISUtilities.Calender_Control_From_TextBox_SetInvisible(this.txtToDate);
                                clsISUtilities.Calender_Control_From_TextBox_SetInvisible(this.txtFromDate);
                            }
                        }
                    }
                }
            }
        }

        private void cboProcess_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pvtblnDontFireEvents == false)
            {
                if (this.btnSave.Enabled == false)
                {
                    if (this.cboLeaveType.SelectedIndex > -1
                        & this.cboOption.SelectedIndex > -1
                        & this.cboProcess.SelectedIndex > -1)
                    {
                        if (cboProcess.SelectedIndex == 0)
                        {
                            pvtintProcessNo = 99;
                        }
                        else
                        {
                            pvtintProcessNo = cboProcess.SelectedIndex - 2;
                        }

                        cboLeaveType_SelectedIndexChanged(sender, e);
                    }
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

                    pvtstrPayrollType = this.dgvPayrollTypeDataGridView[1, e.RowIndex].Value.ToString().Substring(0, 1);

                    this.lblAmount.Text = "0.00";

                    if (pvtDataSet != null)
                    {
                        Load_CurrentForm_Records();

                        Set_Form_For_Read();
                    }
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

                        int intRow = Convert.ToInt32(dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, e.RowIndex].Value);
                        int intFindPayCategoryTimeDecimalRow = -1;
                        int intFindPublicHolidayRow = -1;
                        string strDayFilter = "";

                        if (pvtLeaveDataView[intRow]["LEAVE_OPTION"].ToString() == "P")
                        {
                            this.lblLeaveDate.Text = dgvLeaveDataGridView[pvtintColName, e.RowIndex].Value.ToString() + " " + dgvLeaveDataGridView[pvtintColSurname, e.RowIndex].Value.ToString() + " (Leave Due to be Calculated at Payrol Run Time)";
                        }
                        else
                        {

                            if (pvtLeaveDataView[intRow]["LEAVE_OPTION"].ToString() == "Z")
                            {
                                this.lblLeaveDate.Text = dgvLeaveDataGridView[pvtintColName, e.RowIndex].Value.ToString() + " " + dgvLeaveDataGridView[pvtintColSurname, e.RowIndex].Value.ToString() + " (Leave to be Zerorized at Payrol Run Time)";
                            }
                            else
                            {
                                if (pvtLeaveDataView[intRow]["LEAVE_OPTION"].ToString() == "H")
                                {
                                    this.lblLeaveDate.Text = dgvLeaveDataGridView[pvtintColName, e.RowIndex].Value.ToString() + " " + dgvLeaveDataGridView[pvtintColSurname, e.RowIndex].Value.ToString() + " (" + dgvLeaveDataGridView[pvtintColFromDate, e.RowIndex].Value.ToString() + ")";
                                }
                                else
                                {
                                    this.lblLeaveDate.Text = dgvLeaveDataGridView[pvtintColName, e.RowIndex].Value.ToString() + " " + dgvLeaveDataGridView[pvtintColSurname, e.RowIndex].Value.ToString() + " (" + dgvLeaveDataGridView[pvtintColFromDate, e.RowIndex].Value.ToString() + "  to  " + dgvLeaveDataGridView[pvtintColToDate, e.RowIndex].Value.ToString() + ")";
                                }
                            }
                        }

                        //2013-09-20
                        this.lblAmount.Text = Convert.ToDouble(pvtLeaveDataView[intRow]["LEAVE_HOURS_DECIMAL"]).ToString("##0.00");

                        if (pvtLeaveDataView[intRow]["LEAVE_OPTION"].ToString() != "P")
                        {
                            pvtintFindEmployeeRow = pvtEmployeeDataView.Find(pvtLeaveDataView[intRow]["EMPLOYEE_NO"].ToString());

                            if (pvtEmployeeDataView[pvtintFindEmployeeRow]["LEAVE_PAID_ACCUMULATOR_IND"].ToString() == "1")
                            {
                                //Week Days Only
                                strDayFilter = " AND DAY_NO IN(1,2,3,4,5)";
                            }
                            else
                            {
                                if (pvtEmployeeDataView[pvtintFindEmployeeRow]["LEAVE_PAID_ACCUMULATOR_IND"].ToString() == "2")
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
                                    this.dgvLeaveDateDataGridView.Rows.Add("",
                                                                           dgvLeaveDataGridView[pvtintColLeaveType, e.RowIndex].Value.ToString(),
                                                                           dgvLeaveDataGridView[pvtintColDescription, e.RowIndex].Value.ToString(),
                                                                           dtDateTimeFrom.ToString("dd MMMM yyyy"),
                                                                           dtDateTimeFrom.DayOfWeek.ToString(),
                                                                           "0.00",
                                                                           "0.00",
                                                                           "0.00",
                                                                           dtDateTimeFrom.ToString("yyyyMMdd"));

                                    this.dgvLeaveDateDataGridView[0,this.dgvLeaveDateDataGridView.Rows.Count - 1].Style = this.PublicHolidayDataGridViewCellStyle;
                                }
                                else
                                {
                                    intFindPayCategoryTimeDecimalRow = pvtPayCategoryTimeDecimalDataView.Find(Convert.ToInt32(dtDateTimeFrom.DayOfWeek));

                                    if (intFindPayCategoryTimeDecimalRow == -1)
                                    {
                                        this.dgvLeaveDateDataGridView.Rows.Add("",
                                                                               dgvLeaveDataGridView[pvtintColLeaveType, e.RowIndex].Value.ToString(),
                                                                               dgvLeaveDataGridView[pvtintColDescription, e.RowIndex].Value.ToString(),
                                                                               dtDateTimeFrom.ToString("dd MMMM yyyy"),
                                                                               dtDateTimeFrom.DayOfWeek.ToString(),
                                                                               "0.00",
                                                                               "0.00",
                                                                               "0.00",
                                                                                dtDateTimeFrom.ToString("yyyyMMdd"));

                                        this.dgvLeaveDateDataGridView[0,this.dgvLeaveDateDataGridView.Rows.Count - 1].Style = this.LeaveDaysExcludedDataGridViewCellStyle;
                                    }
                                    else
                                    {
                                        if (pvtLeaveDataView[intRow]["LEAVE_OPTION"].ToString() == "H")
                                        {
                                            this.dgvLeaveDateDataGridView.Rows.Add("",
                                                                                   dgvLeaveDataGridView[pvtintColLeaveType, e.RowIndex].Value.ToString(),
                                                                                   dgvLeaveDataGridView[pvtintColDescription, e.RowIndex].Value.ToString(),
                                                                                   dtDateTimeFrom.ToString("dd MMMM yyyy"),
                                                                                   dtDateTimeFrom.DayOfWeek.ToString(),
                                                                                   Convert.ToDouble(pvtLeaveDataView[intRow]["LEAVE_DAYS_DECIMAL"]).ToString("##0.00"),
                                                                                   Convert.ToDouble(pvtPayCategoryTimeDecimalDataView[intFindPayCategoryTimeDecimalRow]["TIME_DECIMAL"]).ToString("#0.00"),
                                                                                   Convert.ToDouble(pvtLeaveDataView[intRow]["LEAVE_HOURS_DECIMAL"]).ToString("##0.00"),
                                                                                   dtDateTimeFrom.ToString("yyyyMMdd"));

                                            this.dgvLeaveDateDataGridView[0,this.dgvLeaveDateDataGridView.Rows.Count - 1].Style = this.HoursOptionDataGridViewCellStyle;
                                        }
                                        else
                                        {

                                            this.dgvLeaveDateDataGridView.Rows.Add("",
                                                                                   dgvLeaveDataGridView[pvtintColLeaveType, e.RowIndex].Value.ToString(),
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
        }

        private void dgvEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvEmployeeDataGridView.Rows.Count > 0
                & pvtblnEmployeeDataGridViewLoaded == true)
            {

            }
        }

        private void dgvEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnAdd_Click(sender, e);
        }

        private void dgvEmployeeSelectedDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnRemove_Click(sender, e);
        }

        private void dgvLeaveDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (e.Control is ComboBox)
                {
                    ComboBox myComboBox = (ComboBox)e.Control;

                    if (myComboBox != null)
                    {
                        myComboBox.SelectedIndexChanged -= new EventHandler(ComboBox_SelectedIndexChanged);

                        if (this.dgvLeaveDataGridView.Rows[pvtintLeaveDataGridViewRowIndex].ReadOnly == false)
                        {
                            myComboBox.SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);
                        }
                    }
                }
                else
                {
                    if (e.Control is TextBox)
                    {
                        e.Control.KeyPress -= new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);

                        if (this.dgvLeaveDataGridView.CurrentCell.ColumnIndex == pvtintColHours)
                        {
                            e.Control.KeyPress += new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
                        }
                    }
                }
            }
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                ComboBox myComboBox = (ComboBox)sender;
           
                if (this.dgvLeaveDataGridView.CurrentCell.ColumnIndex == pvtintColProcessOrAuthorise)
                {
                    int intProcessNo = myComboBox.SelectedIndex - 1;

                    if (Convert.ToInt32(pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].Value)]["PROCESS_NO"]) != intProcessNo)
                    {
                        pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].Value)]["PROCESS_NO"] = intProcessNo;
                    }
                }
                else
                {
                    DateTime dtFromDateTime = DateTime.ParseExact(this.dgvLeaveDataGridView[pvtintColFromDate, this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].Value.ToString().Substring(0, 10), AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);
                    DateTime dtToDateTime = DateTime.ParseExact(this.dgvLeaveDataGridView[pvtintColToDate, this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].Value.ToString().Substring(0, 10), AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);

                    //0 = Day/s 1=Hours/s
                    if (myComboBox.SelectedIndex == 0)
                    {
                        //Day/s
                        pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].Value)]["LEAVE_OPTION"] = "D";

                        string strDayFilter = "";
                        double dblHoursDay = 0;
                        double dblDays = 0;

                        pvtintFindEmployeeRow = pvtEmployeeDataView.Find(pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].Value)]["EMPLOYEE_NO"].ToString());

                        //ERROL TO CHECK LOGIC
                        if (pvtEmployeeDataView[pvtintFindEmployeeRow]["LEAVE_PAID_ACCUMULATOR_IND"].ToString() == "1")
                        {
                            //Week Days Only
                            strDayFilter = " AND DAY_NO IN(1,2,3,4,5)";
                        }
                        else
                        {
                            if (pvtEmployeeDataView[pvtintFindEmployeeRow]["LEAVE_PAID_ACCUMULATOR_IND"].ToString() == "2")
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

                        int intFindPayCategoryTimeDecimalRow = pvtPayCategoryTimeDecimalDataView.Find(Convert.ToInt32(dtFromDateTime.DayOfWeek));

                        if (intFindPayCategoryTimeDecimalRow == -1)
                        {
                            this.dgvLeaveDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].Style = this.ErrorDataGridViewCellStyle;
                            dgvLeaveDataGridView[1, this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].Style = this.LeaveDaysExcludedDataGridViewCellStyle;
                        }
                        else
                        {
                            int intFindPublicHolidayRow = pvtPublicHolidayDataView.Find(dtFromDateTime.ToString("yyyy-MM-dd"));

                            if (intFindPublicHolidayRow > -1)
                            {
                                this.dgvLeaveDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].Style = this.ErrorDataGridViewCellStyle;
                                dgvLeaveDataGridView[1, this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].Style = this.LeaveDaysExcludedDataGridViewCellStyle;
                            }
                            else
                            {
                                dblDays = 1;
                                dblHoursDay = Convert.ToDouble(pvtPayCategoryTimeDecimalDataView[intFindPayCategoryTimeDecimalRow]["TIME_DECIMAL"]);

                                this.dgvLeaveDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].Style = this.NormalDataGridViewCellStyle;
                                dgvLeaveDataGridView[1, this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].Style = this.NormalDataGridViewCellStyle;
                            }
                        }

                        pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].Value)]["LEAVE_DAYS_DECIMAL"] = dblDays;

                        dgvLeaveDataGridView[pvtintColDays, this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].Value = dblDays.ToString("#0.00");

                        pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].Value)]["LEAVE_HOURS_DECIMAL"] = dblHoursDay;

                        this.dgvLeaveDataGridView[pvtintColHours, this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].Value = dblHoursDay.ToString("#0.00");
                        
                        //UnLock ToDate Cell
                        this.dgvLeaveDataGridView[pvtintColToDate, this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].ReadOnly = false;

                        //Lock Hours Cell
                        this.dgvLeaveDataGridView[pvtintColHours, this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].ReadOnly = true;
                    }
                    else
                    {
                        //Hour/s
                        //ToDate = FromDate
                        pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].Value)]["LEAVE_OPTION"] = "H";
                        
                        this.dgvLeaveDataGridView[pvtintColToDate, this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].Value = this.dgvLeaveDataGridView[pvtintColFromDate, this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].Value.ToString().Substring(0,10);

                        pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].Value)]["LEAVE_TO_DATE"] = dtFromDateTime.ToString("yyyy-MM-dd");

                        this.dgvLeaveDataGridView[pvtintColDays, this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].Value = "0.00";
                        this.dgvLeaveDataGridView[pvtintColHours, this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].Value = "0.00";

                        pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].Value)]["LEAVE_DAYS_DECIMAL"] = 0;
                        pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].Value)]["LEAVE_HOURS_DECIMAL"] = 0;
                      
                        //Lock ToDate Cell
                        this.dgvLeaveDataGridView[pvtintColToDate, this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].ReadOnly = true;

                        //Unlock Hours Cell
                        this.dgvLeaveDataGridView[pvtintColHours, this.Get_DataGridView_SelectedRowIndex(dgvLeaveDataGridView)].ReadOnly = false;
                    }
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
            else
            {
                if (e.Column.Index == pvtintColDays
                | e.Column.Index == pvtintColHours)
                {
                    if (dgvLeaveDataGridView[e.Column.Index, e.RowIndex1].Value.ToString() == "")
                    {
                        e.SortResult = -1;
                    }
                    else
                    {
                        if (dgvLeaveDataGridView[e.Column.Index, e.RowIndex2].Value.ToString() == "")
                        {
                            e.SortResult = 1;
                        }
                        else
                        {
                            if (double.Parse(dgvLeaveDataGridView[e.Column.Index, e.RowIndex1].Value.ToString()) > double.Parse(dgvLeaveDataGridView[e.Column.Index, e.RowIndex2].Value.ToString()))
                            {
                                e.SortResult = 1;
                            }
                            else
                            {
                                if (double.Parse(dgvLeaveDataGridView[e.Column.Index, e.RowIndex1].Value.ToString()) < double.Parse(dgvLeaveDataGridView[e.Column.Index, e.RowIndex2].Value.ToString()))
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

        private void dgvLeaveDataGridView_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (e.RowIndex > -1
                & e.ColumnIndex > -1)
                {
                    if (e.ColumnIndex == pvtintColCode
                    || e.ColumnIndex == pvtintColSurname
                    || e.ColumnIndex == pvtintColName
                    || e.ColumnIndex == pvtintColLeaveType
                    || e.ColumnIndex == pvtintColDays
                    || e.ColumnIndex == pvtintColOption)
                    {
                        this.Cursor = Cursors.No;
                    }
                    else
                    {
                        if (dgvLeaveDataGridView[e.ColumnIndex, e.RowIndex].ReadOnly == true)
                        {
                            this.Cursor = Cursors.No;
                        }
                        else
                        {
                            this.Cursor = Cursors.Default;
                        }
                    }
                }
                else
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void dgvLeaveDataGridView_MouseLeave(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void dgvLeaveDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (e.ColumnIndex == pvtintColHours)
                {
                    e.Cancel = true;
                }
            }
        }

        private void btnRemoveFilter_Click(object sender, EventArgs e)
        {
            pvtintProcessNo = 99;
            pvtstrDayHoursInd = "";

            pvtblnDontFireEvents = true;
            
            if (this.cboLeaveType.SelectedIndex > 0)
            {
                this.cboLeaveType.SelectedIndex = 0;
            }

            if (this.cboProcess.SelectedIndex > 0)
            {
                this.cboProcess.SelectedIndex = 0;
            }

            if (this.cboOption.SelectedIndex > 0)
            {
                this.cboOption.SelectedIndex = 0;
            }

            pvtblnDontFireEvents = false;

            cboLeaveType_SelectedIndexChanged(sender, e);
        }

        private void dgvLeaveDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == pvtintColDescription)
            {
                //Leave Description
                if (dgvLeaveDataGridView[e.ColumnIndex, e.RowIndex].Value == null)
                {
                }
                else
                {
                    if (pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, e.RowIndex].Value)]["LEAVE_DESC"].ToString() != dgvLeaveDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Trim())
                    {
                        pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, e.RowIndex].Value)]["LEAVE_DESC"] = dgvLeaveDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Trim();
                    }
                }
            }
            else
            {
                if (e.ColumnIndex == pvtintColHours)
                {
                    if (pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, e.RowIndex].Value)]["LEAVE_OPTION"].ToString() == "H")
                    {
                        //Hours Value - Uses Hour/s Option
                        if (dgvLeaveDataGridView[e.ColumnIndex, e.RowIndex].Value == System.DBNull.Value)
                        {
                            this.dgvLeaveDataGridView[0,e.RowIndex].Style = this.ErrorDataGridViewCellStyle;
                            dgvLeaveDataGridView[1, e.RowIndex].Style = this.LeaveDaysExcludedDataGridViewCellStyle;
                        }
                        else
                        {
                            if (dgvLeaveDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString() != ".")
                            {
                                if (Convert.ToDouble(pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, e.RowIndex].Value)]["LEAVE_HOURS_DECIMAL"]) != Convert.ToDouble(dgvLeaveDataGridView[e.ColumnIndex, e.RowIndex].Value))
                                {
                                    string strDayFilter = "";
                                    double dblPortionOfDay = 0;

                                    pvtintFindEmployeeRow = pvtEmployeeDataView.Find(pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, e.RowIndex].Value)]["EMPLOYEE_NO"].ToString());

                                    //ERROL TO CHECK LOGIC
                                    if (pvtEmployeeDataView[pvtintFindEmployeeRow]["LEAVE_PAID_ACCUMULATOR_IND"].ToString() == "1")
                                    {
                                        //Week Days Only
                                        strDayFilter = " AND DAY_NO IN(1,2,3,4,5)";
                                    }
                                    else
                                    {
                                        if (pvtEmployeeDataView[pvtintFindEmployeeRow]["LEAVE_PAID_ACCUMULATOR_IND"].ToString() == "2")
                                        {
                                            //Week Days + Saturday
                                            strDayFilter = " AND DAY_NO IN(1,2,3,4,5,6)";
                                        }
                                    }

                                    DateTime dtFromDateTime = DateTime.ParseExact(this.dgvLeaveDataGridView[pvtintColFromDate, e.RowIndex].Value.ToString().Substring(0, 10), AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);

                                    pvtPayCategoryTimeDecimalDataView = null;
                                    pvtPayCategoryTimeDecimalDataView = new DataView(pvtDataSet.Tables["PayCategoryTimeDecimal"],
                                    "PAY_CATEGORY_NO = " + pvtEmployeeDataView[pvtintFindEmployeeRow]["PAY_CATEGORY_NO"].ToString() + strDayFilter,
                                    "DAY_NO",
                                    DataViewRowState.CurrentRows);

                                    int intFindPayCategoryTimeDecimalRow = pvtPayCategoryTimeDecimalDataView.Find(Convert.ToInt32(dtFromDateTime.DayOfWeek));

                                    if (intFindPayCategoryTimeDecimalRow == -1)
                                    {
                                        this.dgvLeaveDataGridView[0,e.RowIndex].Style = this.ErrorDataGridViewCellStyle;
                                        dgvLeaveDataGridView[1, e.RowIndex].Style = this.LeaveDaysExcludedDataGridViewCellStyle;
                                    }
                                    else
                                    {
                                        int intFindPublicHolidayRow = pvtPublicHolidayDataView.Find(dtFromDateTime.ToString("yyyy-MM-dd"));

                                        if (intFindPublicHolidayRow > -1)
                                        {
                                            this.dgvLeaveDataGridView[0,e.RowIndex].Style = this.ErrorDataGridViewCellStyle;
                                            dgvLeaveDataGridView[1, e.RowIndex].Style = this.LeaveDaysExcludedDataGridViewCellStyle;
                                        }
                                        else
                                        {
                                            dblPortionOfDay = Math.Round(Convert.ToDouble(dgvLeaveDataGridView[e.ColumnIndex, e.RowIndex].Value) / Convert.ToDouble(pvtPayCategoryTimeDecimalDataView[intFindPayCategoryTimeDecimalRow]["TIME_DECIMAL"]), 2);

                                            this.dgvLeaveDataGridView[0,e.RowIndex].Style = this.NormalDataGridViewCellStyle;
                                            dgvLeaveDataGridView[1, e.RowIndex].Style = this.NormalDataGridViewCellStyle;
                                        }
                                    }

                                    pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, e.RowIndex].Value)]["LEAVE_DAYS_DECIMAL"] = dblPortionOfDay;

                                    dgvLeaveDataGridView[pvtintColDays, e.RowIndex].Value = dblPortionOfDay.ToString("#0.00");

                                    pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, e.RowIndex].Value)]["LEAVE_HOURS_DECIMAL"] = Convert.ToDouble(dgvLeaveDataGridView[e.ColumnIndex, e.RowIndex].Value);
                                }
                            }
                            else
                            {
                                this.dgvLeaveDataGridView[0,e.RowIndex].Style = this.ErrorDataGridViewCellStyle;
                                dgvLeaveDataGridView[1, e.RowIndex].Style = this.LeaveDaysExcludedDataGridViewCellStyle;
                            }
                        }
                    }
                }
            }
        }

        private void dgvLeaveDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (e.ColumnIndex == pvtintColFromDate
                    | e.ColumnIndex == pvtintColToDate)
                {
                    bool blnDayDatesChanged = false;
                    
                    string strDayFilter = "";
                    int intFindPublicHolidayRow = -1;
                    int intFindPayCategoryTimeDecimalRow = -1;

                    DateTime myDateTime;

                    try
                    {
                        myDateTime = DateTime.ParseExact(dgvLeaveDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString(), AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);
                    }
                    catch
                    {
                        dgvLeaveDataGridView[e.ColumnIndex, e.RowIndex].Value = "";
                        goto dgvLeaveDataGridView_CellValueChanged_Error;
                    }

                    if (this.dgvLeaveDataGridView.CurrentCell.ColumnIndex == pvtintColFromDate)
                    {
                        if (Convert.ToDateTime(pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, e.RowIndex].Value)]["LEAVE_FROM_DATE"]).ToString("yyyy-MM-dd") != myDateTime.ToString("yyyy-MM-dd"))
                        {
                            pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, e.RowIndex].Value)]["LEAVE_FROM_DATE"] = myDateTime.ToString("yyyy-MM-dd");

                            if (pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, e.RowIndex].Value)]["LEAVE_OPTION"].ToString() == "H")
                            {
                                bool blnDayError = false;

                                pvtintFindEmployeeRow = pvtEmployeeDataView.Find(pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, e.RowIndex].Value)]["EMPLOYEE_NO"].ToString());

                                //ERROL TO CHECK LOGIC
                                if (pvtEmployeeDataView[pvtintFindEmployeeRow]["LEAVE_PAID_ACCUMULATOR_IND"].ToString() == "1")
                                {
                                    //Week Days Only
                                    strDayFilter = " AND DAY_NO IN(1,2,3,4,5)";
                                }
                                else
                                {
                                    if (pvtEmployeeDataView[pvtintFindEmployeeRow]["LEAVE_PAID_ACCUMULATOR_IND"].ToString() == "2")
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

                                intFindPayCategoryTimeDecimalRow = pvtPayCategoryTimeDecimalDataView.Find(Convert.ToInt32(myDateTime.DayOfWeek));

                                if (intFindPayCategoryTimeDecimalRow > -1)
                                {
                                    intFindPublicHolidayRow = pvtPublicHolidayDataView.Find(myDateTime.ToString("yyyy-MM-dd"));

                                    if (intFindPublicHolidayRow > -1)
                                    {
                                        blnDayError = true;
                                    }
                                }
                                else
                                {
                                    blnDayError = true;
                                }

                                //Hours Option ToDate is Same as FromDate
                                this.dgvLeaveDataGridView[pvtintColToDate, e.RowIndex].Value = myDateTime.ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString());

                                pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, e.RowIndex].Value)]["LEAVE_TO_DATE"] = myDateTime.ToString("yyyy-MM-dd");

                                double dblPortionOfDay = 0;

                                if (blnDayError == false)
                                {
                                    this.dgvLeaveDataGridView[0,e.RowIndex].Style = this.NormalDataGridViewCellStyle;
                                    dgvLeaveDataGridView[1, e.RowIndex].Style = this.NormalDataGridViewCellStyle;

                                    dblPortionOfDay = Math.Round(Convert.ToDouble(dgvLeaveDataGridView[pvtintColHours, e.RowIndex].Value) / Convert.ToDouble(pvtPayCategoryTimeDecimalDataView[intFindPayCategoryTimeDecimalRow]["TIME_DECIMAL"]), 2);
                                }
                                else
                                {
                                    this.dgvLeaveDataGridView[0,e.RowIndex].Style = this.ErrorDataGridViewCellStyle;
                                    dgvLeaveDataGridView[1, e.RowIndex].Style = this.LeaveDaysExcludedDataGridViewCellStyle;
                                }

                                dgvLeaveDataGridView[pvtintColDays, e.RowIndex].Value = dblPortionOfDay.ToString("#0.00");

                                pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, e.RowIndex].Value)]["LEAVE_DAYS_DECIMAL"] = dblPortionOfDay;
                            }
                            else
                            {
                                blnDayDatesChanged = true;
                            }
                        }
                    }
                    else
                    {
                        if (Convert.ToDateTime(pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, e.RowIndex].Value)]["LEAVE_TO_DATE"]).ToString("yyyy-MM-dd") != myDateTime.ToString("yyyy-MM-dd"))
                        {
                            blnDayDatesChanged = true;
                            pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, e.RowIndex].Value)]["LEAVE_TO_DATE"] = myDateTime.ToString("yyyy-MM-dd");
                        }
                    }

                    if (blnDayDatesChanged == true)
                    {
                        double dblNumberHours = 0;
                        double dblNumberDays = 0;

                        pvtintFindEmployeeRow = pvtEmployeeDataView.Find(pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, e.RowIndex].Value)]["EMPLOYEE_NO"].ToString());

                        //ERROL TO CHECK LOGIC
                        if (pvtEmployeeDataView[pvtintFindEmployeeRow]["LEAVE_PAID_ACCUMULATOR_IND"].ToString() == "1")
                        {
                            //Week Days Only
                            strDayFilter = " AND DAY_NO IN(1,2,3,4,5)";
                        }
                        else
                        {
                            if (pvtEmployeeDataView[pvtintFindEmployeeRow]["LEAVE_PAID_ACCUMULATOR_IND"].ToString() == "2")
                            {
                                //Week Days + Saturday
                                strDayFilter = " AND DAY_NO IN(1,2,3,4,5,6)";
                            }
                        }

                        DateTime dtFromDateTime = DateTime.ParseExact(this.dgvLeaveDataGridView[pvtintColFromDate, e.RowIndex].Value.ToString().Substring(0, 10), AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);
                        DateTime dtToDateTime = DateTime.ParseExact(this.dgvLeaveDataGridView[pvtintColToDate, e.RowIndex].Value.ToString().Substring(0, 10), AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);

                        //Get Correct Value - Value has not been Commiteed
                        if (this.dgvLeaveDataGridView.CurrentCell.ColumnIndex == pvtintColFromDate)
                        {
                            dtFromDateTime = myDateTime;
                        }
                        else
                        {
                            dtToDateTime = myDateTime;
                        }

                        pvtPayCategoryTimeDecimalDataView = null;
                        pvtPayCategoryTimeDecimalDataView = new DataView(pvtDataSet.Tables["PayCategoryTimeDecimal"],
                        "PAY_CATEGORY_NO = " + pvtEmployeeDataView[pvtintFindEmployeeRow]["PAY_CATEGORY_NO"].ToString() + strDayFilter,
                        "DAY_NO",
                        DataViewRowState.CurrentRows);

                        int intDaysDiff = dtToDateTime.Subtract(dtFromDateTime).Days + 1;

                        while (dtFromDateTime <= dtToDateTime)
                        {
                            intFindPayCategoryTimeDecimalRow = pvtPayCategoryTimeDecimalDataView.Find(Convert.ToInt32(dtFromDateTime.DayOfWeek));

                            if (intFindPayCategoryTimeDecimalRow > -1)
                            {
                                intFindPublicHolidayRow = pvtPublicHolidayDataView.Find(dtFromDateTime.ToString("yyyy-MM-dd"));

                                if (intFindPublicHolidayRow == -1)
                                {
                                    dblNumberHours += Convert.ToDouble(pvtPayCategoryTimeDecimalDataView[intFindPayCategoryTimeDecimalRow]["TIME_DECIMAL"]);
                                    dblNumberDays += 1;
                                }
                            }

                            dtFromDateTime = dtFromDateTime.AddDays(1);
                        }

                        dgvLeaveDataGridView[pvtintColDays, e.RowIndex].Value = dblNumberDays.ToString("#0.00");
                        pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, e.RowIndex].Value)]["LEAVE_DAYS_DECIMAL"] = dblNumberDays;

                        dgvLeaveDataGridView[pvtintColHours, e.RowIndex].Value = dblNumberHours.ToString("#0.00");
                        pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, e.RowIndex].Value)]["LEAVE_HOURS_DECIMAL"] = dblNumberHours;

                        pvtLeaveDataView[Convert.ToInt32(this.dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, e.RowIndex].Value)]["DATE_DIFF_NO_DAYS"] = intDaysDiff;

                        if (dblNumberDays == 0)
                        {
                            this.dgvLeaveDataGridView[0,e.RowIndex].Style = this.ErrorDataGridViewCellStyle;
                        }
                        else
                        {
                            this.dgvLeaveDataGridView[0,e.RowIndex].Style = this.NormalDataGridViewCellStyle;

                            if (intDaysDiff == Convert.ToInt32(dblNumberDays))
                            {
                                dgvLeaveDataGridView[1, e.RowIndex].Style = this.NormalDataGridViewCellStyle;

                            }
                            else
                            {
                                dgvLeaveDataGridView[1, e.RowIndex].Style = this.LeaveDaysExcludedDataGridViewCellStyle;
                            }
                        }

                        dgvLeaveDataGridView.Refresh();
                    }

                dgvLeaveDataGridView_CellValueChanged_Error:

                    int intError = 0;
                }
            }
        }

        private void dgvLeaveDateDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 3)
            {
                if (double.Parse(dgvLeaveDateDataGridView[8, e.RowIndex1].Value.ToString()) > double.Parse(dgvLeaveDateDataGridView[8, e.RowIndex2].Value.ToString()))
                {
                    e.SortResult = 1;
                }
                else
                {
                    if (double.Parse(dgvLeaveDateDataGridView[8, e.RowIndex1].Value.ToString()) < double.Parse(dgvLeaveDateDataGridView[8, e.RowIndex2].Value.ToString()))
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dlgResult = CustomMessageBox.Show("Delete All Records in Current Spreadsheet?",
                this.Text,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

                if (dlgResult == DialogResult.Yes)
                {
                    int intLeaveRow = 0;

                    pvtTempDataSet = null;
                    pvtTempDataSet = new DataSet();

                    DataTable myDataTable = this.pvtDataSet.Tables["Leave"].Clone();

                    pvtTempDataSet.Tables.Add(myDataTable);

                    bool blnRecordLocked = false;

                    for (int intRow = 0; intRow < this.dgvLeaveDataGridView.Rows.Count; intRow++)
                    {
                        if (this.dgvLeaveDataGridView.Rows[intRow].ReadOnly == true)
                        {
                            blnRecordLocked = true;
                        }
                        else
                        {
                            intLeaveRow = Convert.ToInt32(dgvLeaveDataGridView[pvtintLeaveColHoldingIndex, intRow].Value);

                            pvtTempDataSet.Tables["Leave"].ImportRow(pvtLeaveDataView[intLeaveRow].Row);

                            pvtLeaveDataView[intLeaveRow].Delete();
                        }
                    }

                    if (blnRecordLocked == true)
                    {
                        dlgResult = CustomMessageBox.Show("There are Records That cannot be Deleted because they are LOCKED.\n\nDelete Records that are NOT Locked?",
                        this.Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                        if (dlgResult == DialogResult.No)
                        {
                            this.pvtDataSet.RejectChanges();
                            return;
                        }
                    }

                    //Compress DataSet
                    pvtbytCompress = clsISUtilities.Compress_DataSet(pvtTempDataSet);
                        
                    object[] objParm = new object[5];

                    objParm[0] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                    objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[2] = this.pvtstrPayrollType;
                    objParm[3] = pvtbytCompress;
                    objParm[4] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                      
                    int intReturnCode = (int)clsISUtilities.DynamicFunction("Delete_Record", objParm, true);

                    if (intReturnCode == 1)
                    {
                        this.pvtDataSet.RejectChanges();

                        CustomMessageBox.Show("Action Cancelled - Payroll Run in Progress.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

                        if (this.pvtstrPayrollType == "W")
                        {
                            this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"] = "Y";
                        }
                        else
                        {
                            if (this.pvtstrPayrollType == "S")
                            {
                                this.pvtDataSet.Tables["Company"].Rows[0]["SALARY_RUN_IND"] = "Y";
                            }
                            else
                            {
                                this.pvtDataSet.Tables["Company"].Rows[0]["TIME_ATTENDANCE_RUN_IND"] = "Y";
                            }
                        }
                    }
                }

                this.pvtDataSet.AcceptChanges();
              
                btnCancel_Click(sender, e);
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void txtDescription_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
