using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace InteractPayroll
{
    public partial class frmTimeAttendanceRun : Form
    {
        clsISUtilities clsISUtilities;

        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;

        private bool pvtblnTimeSheetError = false;
        
        private byte[] pvtbytCompress;
        private DataView pvtPayCategoryDataView;
        private DataView pvtPayCategoryWeekDataView;
        private DataView pvtPaidHolidayDataView;

        private int pvtintPayCategoryRow = -1;
        private int pvtintWeekRow = -1;
        
        private int pvtintLastUploadDateTimeCol = 4;
        private int pvtintLastUploadDateTimeForSortCol = 8;
   
        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
        private int pvtintPayCategoryDataGridViewRowIndex = -1;
        private int pvtintWeekDataGridViewRowIndex = -1;

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnPayCategoryDataGridViewLoaded = false;
        private bool pvtblnWeekDataGridViewLoaded = false;

        private bool pvtblnUploadWarningFormHasAlreadyBeenShown = false;

        DataGridViewCellStyle ErrorDataGridViewCellStyle;
        DataGridViewCellStyle PublicHolidayDataGridViewCellStyle;
        DataGridViewCellStyle OutsideBoundaryDataGridViewCellStyle;
        DataGridViewCellStyle LastUploadDateTimeWarningDataGridViewCellStyle;
        
        frmUploadWarning frmUploadWarning;

        private int pvtintNextQueueCheck = 0;
        
        public frmTimeAttendanceRun()
        {
            InitializeComponent();

            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 114;

                this.dgvPayCategoryDataGridView.Height += 114;

                this.grbStatus.Top += 114;

                this.lblDayThreshold.Top += 114;
                this.dgvDayThresholdDataGridView.Top += 114;

                this.lblOverTimeRate.Top += 114;
                this.dgvOvertimeRateDataGridView.Top += 114;

                this.lblWeekDesc.Top += 114;
                this.dgvWeekDataGridView.Top += 114;

                this.lblPublicHoliday.Top += 114;
                this.dgvPublicHolidayDataGridView.Top += 114;

                this.grbPublicHolidayInfo.Top += 114;
                this.grbLegend.Top += 114;

                this.grbTimesheetError.Top += 114;

                this.picWeekLock.Top += 114; ;
            }

            grbStatus.Top = grbTimesheetError.Top;
            grbStatus.Left = grbTimesheetError.Left;
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        public void btnRun_Click(object sender, System.EventArgs e)
        {
            try
            {
                DialogResult dlgResult;

                dlgResult = CustomMessageBox.Show("Are you sure you want to " + this.btnRun.Text + " '" + this.Text + "'\n\nfor Company\n\n'" + pvtDataSet.Tables["Company"].Rows[0]["COMPANY_DESC"].ToString() + "'",
                        this.Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                if (dlgResult == DialogResult.Yes)
                {
                    this.Refresh();

                    if (this.btnRun.Text == "Run")
                    {
                        Calculate_Payroll_From_TimeSheets();
                    }
                    else
                    {
                        object[] objParm = new object[3];
                        objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                        objParm[1] = this.dgvPayrollTypeDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0,1);
                        objParm[2] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();

                        pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Reset_Run", objParm);

                        pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                        this.Set_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView, 0);

                        this.btnRun.Text = "Run";
                        this.lblRunStatus.Text = "Run Pending";
                        this.picStatus.Image = global::TimeAttendanceRun.Properties.Resources.Question48;

                        CustomMessageBox.Show("Reset Successful.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
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
                    case "dgvPayrollTypeDataGridView":

                        pvtintPayrollTypeDataGridViewRowIndex = -1;
                        this.dgvPayrollTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPayCategoryDataGridView":

                        pvtintPayCategoryDataGridViewRowIndex = -1;
                        this.dgvPayCategoryDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvWeekDataGridView":

                        pvtintWeekDataGridViewRowIndex = -1;
                        this.dgvWeekDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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
            this.Text = this.Text.Substring(0, this.Text.IndexOf("-") - 1);

            Set_Form_For_Read();

            this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, pvtintPayCategoryRow);
            this.Set_DataGridView_SelectedRowIndex(this.dgvWeekDataGridView, pvtintWeekRow);
        }

        private void frmTimeAttendanceRun_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this,"busTimeAttendanceRun");

                int intTimeout = Convert.ToInt32(AppDomain.CurrentDomain.GetData("TimeSheetReadTimeoutSeconds")) * 1000;

                clsISUtilities.Set_WebService_Timeout_Value(intTimeout);

                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblWeekDesc.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblDayThreshold.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblOverTimeRate.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPublicHoliday.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
  
                ErrorDataGridViewCellStyle = new DataGridViewCellStyle();
                ErrorDataGridViewCellStyle.BackColor = Color.Red;
                ErrorDataGridViewCellStyle.SelectionBackColor = Color.Red;

                PublicHolidayDataGridViewCellStyle = new DataGridViewCellStyle();
                PublicHolidayDataGridViewCellStyle.BackColor = Color.SlateBlue;
                PublicHolidayDataGridViewCellStyle.SelectionBackColor = Color.SlateBlue;

                OutsideBoundaryDataGridViewCellStyle = new DataGridViewCellStyle();
                OutsideBoundaryDataGridViewCellStyle.BackColor = Color.Orange;
                OutsideBoundaryDataGridViewCellStyle.SelectionBackColor = Color.Orange;

                LastUploadDateTimeWarningDataGridViewCellStyle = new DataGridViewCellStyle();
                LastUploadDateTimeWarningDataGridViewCellStyle.BackColor = Color.Yellow;
                LastUploadDateTimeWarningDataGridViewCellStyle.SelectionBackColor = Color.Yellow;
                         
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

                this.Set_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView,0);

                if (pvtDataSet.Tables["PayrollQueue"] != null)
                {
                    if (pvtDataSet.Tables["PayrollQueue"].Rows.Count > 0)
                    {
                        if (pvtDataSet.Tables["PayrollQueue"].Rows[0]["PAYROLL_RUN_QUEUE_IND"].ToString() == ""
                        || pvtDataSet.Tables["PayrollQueue"].Rows[0]["PAYROLL_RUN_QUEUE_IND"].ToString() == "S")
                        {
                            Busy_With_Run();
                        }
                        else
                        {
                            if (pvtDataSet.Tables["PayrollQueue"].Rows[0]["PAYROLL_RUN_QUEUE_IND"].ToString() == "E")
                            {
                                Set_Timesheet_In_Error();
                            }
                            else
                            {
                                if (pvtDataSet.Tables["PayrollQueue"].Rows[0]["PAYROLL_RUN_QUEUE_IND"].ToString() == "F")
                                {
                                    Set_Run_Failed();
                                }
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

        private void Calculate_Payroll_From_TimeSheets()
        {
            try
            {
                //DateTime dtDateRun = Convert.ToDateTime(pvtPayCategoryWeekDataView[this.flxgWeek.Rows.Count - 2]["WEEK_DATE"]);
                DateTime dtDateRun = Convert.ToDateTime(pvtPayCategoryWeekDataView[this.dgvWeekDataGridView.Rows.Count - 1]["WEEK_DATE"]);

                string strPayCategoryNos = "";

                //2017-04-26
                for (int intCount = 0; intCount < this.dgvPayCategoryDataGridView.Rows.Count; intCount++)
                {
                    if (intCount == 0)
                    {
                        strPayCategoryNos = this.dgvPayCategoryDataGridView[6,intCount].Value.ToString();
                    }
                    else
                    {
                        strPayCategoryNos += "|" + this.dgvPayCategoryDataGridView[6, intCount].Value.ToString();
                    }
                }

                object[] objParm = new object[5];

                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[2] = strPayCategoryNos;
                objParm[3] = this.dgvPayrollTypeDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0,1);
                objParm[4] = dtDateRun;

                string strReturnPayCategoryNosInError = "";

                if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
                {
#if (DEBUG)
                    strReturnPayCategoryNosInError = (string)clsISUtilities.DynamicFunction("Calculate_TimeAttendance_From_TimeSheets", objParm);
#else
                    strReturnPayCategoryNosInError = (string)clsISUtilities.DynamicFunction("Insert_Run_Into_Queue", objParm);
#endif
                }
                else
                {
#if (DEBUG)
                    strReturnPayCategoryNosInError = (string)clsISUtilities.DynamicFunction("Calculate_Payroll_From_TimeSheets", objParm);
#else
                    strReturnPayCategoryNosInError = (string)clsISUtilities.DynamicFunction("Insert_Run_Into_Queue", objParm);
#endif
                }

                if (strReturnPayCategoryNosInError == "")
                {
#if (DEBUG)
                    this.btnRun.Text = "Reset";

                    lblRunStatus.Text = "Run Completed";

                    this.picStatus.Image = global::TimeAttendanceRun.Properties.Resources.tick48;
                    CustomMessageBox.Show("Run Successful.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
#else
                    Busy_With_Run();
#endif
                }
                else
                {
                    string[] strPayCategoryErrorNos = strReturnPayCategoryNosInError.Split('|');
                    
                    for (int intRowCount = 0; intRowCount < strPayCategoryErrorNos.Length; intRowCount++)
                    {
                        for (int intRow = 0; intRow < pvtPayCategoryDataView.Count; intRow++)
                        {
                            if (strPayCategoryErrorNos[intRowCount].ToString() == pvtPayCategoryDataView[intRow]["PAY_CATEGORY_NO"].ToString())
                            {
                                pvtPayCategoryDataView[intRow]["ERROR_IND"] = "Y";
                                break;
                            }
                        }

                        for (int intRow = 0; intRow < this.dgvPayCategoryDataGridView.Rows.Count; intRow++)
                        {
                            //2017-04-26
                            if (strPayCategoryErrorNos[intRowCount].ToString() == this.dgvPayCategoryDataGridView[6,intRow].Value.ToString())
                            {
                                this.dgvPayCategoryDataGridView[0,intRow].Style = ErrorDataGridViewCellStyle;

                                break;
                            }
                        }
                    }

                    this.grbStatus.Visible = false;
                    this.grbTimesheetError.Visible = true;

                    this.btnRun.Enabled = false;
                    this.btnUpdate.Enabled = false;

                    CustomMessageBox.Show("Errors in Timesheets.",
                           this.Text,
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Error);
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }
                
        private void Load_CostCentres()
        {
            this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
            this.Clear_DataGridView(this.dgvWeekDataGridView);

            if (pvtPayCategoryDataView.Count == 0)
            {
                Clear_Form_Fields();

                Set_Form_For_Read();
            }
            else
            {
                int intComboSelectedIndex = 0;

                pvtblnTimeSheetError = false;

                Set_Form_For_Read();

                pvtblnPayCategoryDataGridViewLoaded = false;

                string strLastUploadDateTime = "";
                string strLastUploadDateTimeForSort = "";
                bool blnLastUploadDateTimeWarning = false;

                for (int intRowCount = 0; intRowCount < pvtPayCategoryDataView.Count; intRowCount++)
                {
                    if (pvtPayCategoryDataView[intRowCount]["LAST_UPLOAD_DATETIME"] != System.DBNull.Value)
                    {
                        strLastUploadDateTime = Convert.ToDateTime(pvtPayCategoryDataView[intRowCount]["LAST_UPLOAD_DATETIME"]).ToString("dd MMM yyyy - HH:mm");
                        strLastUploadDateTimeForSort = Convert.ToDateTime(pvtPayCategoryDataView[intRowCount]["LAST_UPLOAD_DATETIME"]).ToString("yyyyMMddHHmmss");
                    }
                    else
                    {
                        strLastUploadDateTime = "";
                        strLastUploadDateTimeForSort = "";
                    }

                    this.dgvPayCategoryDataGridView.Rows.Add("",
                                                             "",
                                                             "", 
                                                             pvtPayCategoryDataView[intRowCount]["PAY_CATEGORY_DESC"].ToString(),
                                                             Convert.ToDateTime(pvtPayCategoryDataView[intRowCount]["PAY_PERIOD_DATE_FROM"]).ToString("dd MMM yyyy") + "   to  " + Convert.ToDateTime(pvtPayCategoryDataView[intRowCount]["PAY_PERIOD_DATE"]).ToString("dd MMM yyyy"),
                                                             strLastUploadDateTime,
                                                             pvtPayCategoryDataView[intRowCount]["PAY_CATEGORY_NO"].ToString(),
                                                             pvtPayCategoryDataView[intRowCount]["PAY_PUBLIC_HOLIDAY_IND"].ToString(),
                                                             strLastUploadDateTimeForSort);
               
                    if (pvtPayCategoryDataView[intRowCount]["ERROR_IND"].ToString() == "Y")
                    {
                        if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
                        {
                            //ELR - 2015-06-06
                            if (pvtDataSet.Tables["Company"].Rows[0]["TIME_ATTENDANCE_RUN_IND"].ToString() != "Y")
                            {
                                this.dgvPayCategoryDataGridView[0,this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = ErrorDataGridViewCellStyle;

                                pvtblnTimeSheetError = true;
                            }
                        }
                        else
                        {
                            //ELR - 2015-06-06
                            if (pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"].ToString() != "Y")
                            {
                                this.dgvPayCategoryDataGridView[0,this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = ErrorDataGridViewCellStyle;

                                pvtblnTimeSheetError = true;
                            }
                        }
                    }
                    
                    if (pvtPayCategoryDataView[intRowCount]["PUBLIC_HOLIDAY_FLAG_IND"].ToString() == "Y")
                    {
                        this.dgvPayCategoryDataGridView[1,this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = PublicHolidayDataGridViewCellStyle;
                    }

                    //Check If Last Upload DateTime > Run Date
                    if (pvtPayCategoryDataView[intRowCount]["LAST_UPLOAD_DATETIME"] != System.DBNull.Value)
                    {
                        if (Convert.ToDateTime(pvtPayCategoryDataView[intRowCount]["LAST_UPLOAD_DATETIME"]) > Convert.ToDateTime(pvtPayCategoryDataView[intRowCount]["PAY_PERIOD_DATE"]))
                        {
                        }
                        else
                        {
                            this.dgvPayCategoryDataGridView[2, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = LastUploadDateTimeWarningDataGridViewCellStyle;
                            blnLastUploadDateTimeWarning = true;

                            if (frmUploadWarning == null)
                            {
                                frmUploadWarning = new frmUploadWarning();
                                this.frmUploadWarning.lblCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                                if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
                                {
                                    frmUploadWarning.lblDateDescription.Text = "Time Attendance Run Date";

                                    frmUploadWarning.lblInfo1.Text = "The above Cost Centre/s Last Upload Date/Time of Timesheets does not Exceed the Time Attendance Run Date.";
                                    frmUploadWarning.lblInfo2.Text = "This Run could possibly exclude Time Sheets from these Cost Centres.";
                                }
                                else
                                {
                                    frmUploadWarning.lblDateDescription.Text = "Payroll Run Date (Wages)";

                                    frmUploadWarning.lblInfo1.Text = "The above Cost Centre/s Last Upload Date/Time of Timesheets does not Exceed the Payroll Run Date (Wages).";
                                    frmUploadWarning.lblInfo2.Text = "This Run could possibly exclude Time Sheets from these Cost Centres.";
                                }

                                frmUploadWarning.txtPayrollRunDate.Text = Convert.ToDateTime(pvtPayCategoryDataView[intRowCount]["PAY_PERIOD_DATE"]).ToString("d MMMM yyyy");
                            }
                        
                            this.frmUploadWarning.dgvPayCategoryDataGridView.Rows.Add("",
                                                                                      pvtPayCategoryDataView[intRowCount]["PAY_CATEGORY_DESC"].ToString(),
                                                                                      strLastUploadDateTime,
                                                                                      strLastUploadDateTimeForSort);

                            this.frmUploadWarning.dgvPayCategoryDataGridView[0, this.frmUploadWarning.dgvPayCategoryDataGridView.Rows.Count - 1].Style = LastUploadDateTimeWarningDataGridViewCellStyle;
                        }
                    }
                }

                if (blnLastUploadDateTimeWarning == true)
                {
                    if (pvtblnUploadWarningFormHasAlreadyBeenShown == false)
                    {
                        //Show frmUploadWarning
                        this.timer.Enabled = true;
                    }
                }

                pvtblnPayCategoryDataGridViewLoaded = true;

                this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, intComboSelectedIndex);
            }
        }

        private void Set_Form_For_Read()
        {
            this.dgvWeekDataGridView.Enabled = true;

            this.picPayCategoryLock.Visible = false;
            this.picPayrollTypeLock.Visible = false;
            this.picWeekLock.Visible = false;

            if (pvtblnTimeSheetError == false)
            {
                this.btnRun.Enabled = true;
            }

            this.btnUpdate.Enabled = true;

            this.dgvDayThresholdDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvDayThresholdDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

            this.dgvPublicHolidayDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvPublicHolidayDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

            this.dgvOvertimeRateDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvOvertimeRateDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

            if (pvtPayCategoryWeekDataView != null)
            {
                if (pvtPayCategoryWeekDataView.Count == 0)
                {
                    this.btnRun.Enabled = false;
                    this.btnUpdate.Enabled = false;
                }
            }
            else
            {
                this.btnRun.Enabled = false;
                this.btnUpdate.Enabled = false;
            }

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.dgvPayCategoryDataGridView.Enabled = true;
            this.dgvPayrollTypeDataGridView.Enabled = true;

            this.grbTimesheetError.Enabled = true;
            this.grbStatus.Enabled = true;
            this.grbPublicHolidayInfo.Enabled = true;
        }

        private void Set_Form_For_Edit()
        {
            this.btnRun.Enabled = false;
            this.btnUpdate.Enabled = false;

            this.picPayCategoryLock.Visible = true;
            this.picPayrollTypeLock.Visible = true;
            this.picWeekLock.Visible = true;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.dgvPayCategoryDataGridView.Enabled = false;
            this.dgvPayrollTypeDataGridView.Enabled = false; 
            this.dgvWeekDataGridView.Enabled = false;

            this.dgvDayThresholdDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dgvDayThresholdDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;

            this.dgvPublicHolidayDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dgvPublicHolidayDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;

            this.dgvOvertimeRateDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dgvOvertimeRateDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;
        }

        private void Clear_Form_Fields()
        {
            this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
            this.Clear_DataGridView(this.dgvWeekDataGridView);
            this.Clear_DataGridView(this.dgvDayThresholdDataGridView);
            this.Clear_DataGridView(this.dgvPublicHolidayDataGridView);
            this.Clear_DataGridView(this.dgvOvertimeRateDataGridView);
        }

        private void Load_CurrentForm_Records()
        {
            if (pvtDataSet.Tables["Company"].Rows.Count == 0)
            {
                this.btnRun.Enabled = false;
            }
            else
            {
                pvtPayCategoryDataView = null;
                pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"],
                    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")),
                    "",
                    DataViewRowState.CurrentRows);
                
                Load_CostCentres();

                if ((pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"].ToString() == "Y"
                    & this.dgvPayrollTypeDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0,1) == "W")
                    |  (pvtDataSet.Tables["Company"].Rows[0]["SALARY_RUN_IND"].ToString() == "Y"
                    & this.dgvPayrollTypeDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0,1) == "S")
                    | (pvtDataSet.Tables["Company"].Rows[0]["TIME_ATTENDANCE_RUN_IND"].ToString() == "Y"
                    & this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) == "T"))
                {
                    this.lblRunStatus.Text = "Run Completed Successfully";
                    this.picStatus.Image = global::TimeAttendanceRun.Properties.Resources.tick48; 

                    this.btnRun.Text = "Reset";

                    this.btnUpdate.Enabled = true;
                    this.btnRun.Enabled = true;
                }
                else
                {
                    this.btnRun.Text = "Run";

                    if (pvtblnTimeSheetError == true)
                    {
                        //this.lblRunStatus.Text = "Error/s in Timesheets. Fix to Continue.";

                        this.grbStatus.Visible = false;
                        this.grbTimesheetError.Visible = true;
                        this.btnRun.Enabled = false;
                    }
                    else
                    {
                        this.lblRunStatus.Text = "Run Pending";

                        this.picStatus.Image = global::TimeAttendanceRun.Properties.Resources.Question48; 

                        if (this.dgvWeekDataGridView.Rows.Count > 0)
                        {
                            //nb tHESE WILL HAVE TO BE CHANGED DUE TO sALARIES
                            if (this.pvtDataSet.Tables["Company"].Rows[0]["TIMESHEETS_AUTHORISED_IND"].ToString() == "Y"
                            & this.pvtDataSet.Tables["Company"].Rows[0]["LEAVE_AUTHORISED_IND"].ToString() == "Y")
                            {
                                this.btnRun.Enabled = true;
                            }
                            else
                            {
                                if (this.pvtDataSet.Tables["Company"].Rows[0]["TIMESHEETS_AUTHORISED_IND"].ToString() == "N"
                                & this.pvtDataSet.Tables["Company"].Rows[0]["LEAVE_AUTHORISED_IND"].ToString() == "N")
                                {
                                    grbTimesheetError.Text = "Timesheet and Leave Authorisation";
                                }
                                else
                                {
                                    if (this.pvtDataSet.Tables["Company"].Rows[0]["TIMESHEETS_AUTHORISED_IND"].ToString() == "N")
                                    {
                                        grbTimesheetError.Text = "Timesheet Authorisation";
                                    }
                                    else
                                    {
                                        grbTimesheetError.Text = "Leave Authorisation";
                                    }
                                }

                                this.lblErrors.Text = "There are Employees who have Not been Authorised for this Run. Run Button Not Available.";

                                this.grbTimesheetError.Visible = true;
                                this.grbStatus.Visible = false;
                                this.btnRun.Enabled = false;
                            }

                            this.btnUpdate.Enabled = true;
                        }
                    }
                }
            }
        }

        private string Return_Day_Of_Week(string parDayText)
        {
            string strDayDesc = "";

            if (parDayText.IndexOf(" - Mon") > -1)
            {
                strDayDesc = "MON";
            }
            else
            {
                if (parDayText.IndexOf(" - Tue") > -1)
                {
                    strDayDesc = "TUE";
                }
                else
                {
                    if (parDayText.IndexOf(" - Wed") > -1)
                    {
                        strDayDesc = "WED";
                    }
                    else
                    {
                        if (parDayText.IndexOf(" - Thu") > -1)
                        {
                            strDayDesc = "THU";
                        }
                        else
                        {
                            if (parDayText.IndexOf(" - Fri") > -1)
                            {
                                strDayDesc = "FRI";
                            }
                            else
                            {
                                if (parDayText.IndexOf(" - Sat") > -1)
                                {
                                    strDayDesc = "SAT";
                                }
                                else
                                {
                                    if (parDayText.IndexOf(" - Sun") > -1)
                                    {
                                        strDayDesc = "SUN";
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return strDayDesc;
        }

        public void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                int intTimeMinutes = 0;
                double dblTimeUp = 0;
                double dblTimeDown = 0;
                string strDayDesc = "";

                for (int intRow = 0; intRow < this.dgvDayThresholdDataGridView.Rows.Count; intRow++)
                {
                    intTimeMinutes = Convert.ToInt32(Convert.ToInt32(this.dgvDayThresholdDataGridView[5,intRow].Value) * 60) + Convert.ToInt32(this.dgvDayThresholdDataGridView[6,intRow].Value);
                    strDayDesc = this.dgvDayThresholdDataGridView[4,intRow].Value.ToString().Substring(0,3).ToUpper();
                    pvtPayCategoryWeekDataView[this.Get_DataGridView_SelectedRowIndex(this.dgvWeekDataGridView)][strDayDesc + "_TIME_MINUTES"] = intTimeMinutes;

                    dblTimeUp = Convert.ToDouble(pvtPayCategoryWeekDataView[this.Get_DataGridView_SelectedRowIndex(this.dgvWeekDataGridView)][strDayDesc + "_TIME_MINUTES"]) * Convert.ToDouble(pvtPayCategoryWeekDataView[this.Get_DataGridView_SelectedRowIndex(this.dgvWeekDataGridView)]["EXCEPTION_SHIFT_ABOVE_PERCENT"]) / 100;
                    dblTimeDown = Convert.ToDouble(pvtPayCategoryWeekDataView[this.Get_DataGridView_SelectedRowIndex(this.dgvWeekDataGridView)][strDayDesc + "_TIME_MINUTES"]) * Convert.ToDouble(pvtPayCategoryWeekDataView[this.Get_DataGridView_SelectedRowIndex(this.dgvWeekDataGridView)]["EXCEPTION_SHIFT_BELOW_PERCENT"]) / 100;
                    pvtPayCategoryWeekDataView[this.Get_DataGridView_SelectedRowIndex(this.dgvWeekDataGridView)]["EXCEPTION_" + strDayDesc + "_ABOVE_MINUTES"] = Convert.ToInt32(pvtPayCategoryWeekDataView[this.Get_DataGridView_SelectedRowIndex(this.dgvWeekDataGridView)][strDayDesc + "_TIME_MINUTES"]) + Convert.ToInt32(dblTimeUp);
                    pvtPayCategoryWeekDataView[this.Get_DataGridView_SelectedRowIndex(this.dgvWeekDataGridView)]["EXCEPTION_" + strDayDesc + "_BELOW_MINUTES"] = Convert.ToInt32(pvtPayCategoryWeekDataView[this.Get_DataGridView_SelectedRowIndex(this.dgvWeekDataGridView)][strDayDesc + "_TIME_MINUTES"]) - Convert.ToInt32(dblTimeDown);
                }

                for (int intRow = 1; intRow <= dgvOvertimeRateDataGridView.Rows.Count; intRow++)
                {
                    intTimeMinutes = (Convert.ToInt32(this.dgvOvertimeRateDataGridView[2, intRow - 1].Value) * 60) + Convert.ToInt32(this.dgvOvertimeRateDataGridView[3, intRow - 1].Value);

                    if (intTimeMinutes > 59940)
                    {
                        //hhh=999 then mm=00
                        intTimeMinutes = 59940;
                    }

                    pvtPayCategoryWeekDataView[this.Get_DataGridView_SelectedRowIndex(this.dgvWeekDataGridView)]["OVERTIME" + intRow.ToString() + "_MINUTES"] = intTimeMinutes;
                }

                for (int intRow = 1; intRow <= this.dgvPublicHolidayDataGridView.Rows.Count; intRow++)
                {
                    intTimeMinutes = Convert.ToInt32(Convert.ToInt32(this.dgvPublicHolidayDataGridView[3, intRow - 1].Value) * 60) + Convert.ToInt32(this.dgvPublicHolidayDataGridView[4, intRow - 1].Value);

                    pvtPayCategoryWeekDataView[this.Get_DataGridView_SelectedRowIndex(this.dgvWeekDataGridView)]["PAIDHOLIDAY_MINUTES" + intRow.ToString()] = intTimeMinutes;
                }

                if (this.dgvPayrollTypeDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0,1) == "W")
                {
                    pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"] = "N";
                }
                else
                {
                    if (this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) == "S")
                    {
                        pvtDataSet.Tables["Company"].Rows[0]["SALARY_RUN_IND"] = "N";
                    }
                    else
                    {
                        pvtDataSet.Tables["Company"].Rows[0]["TIME_ATTENDANCE_RUN_IND"] = "N";
                    }
                }

                pvtTempDataSet = null;
                pvtTempDataSet = new DataSet();

                DataTable myDataTable = this.pvtDataSet.Tables["PayCategoryWeek"].Clone();
                pvtTempDataSet.Tables.Add(myDataTable);
                pvtTempDataSet.Tables[0].ImportRow(pvtPayCategoryWeekDataView[this.Get_DataGridView_SelectedRowIndex(this.dgvWeekDataGridView)].Row);

                //Compress DataSet
                pvtbytCompress = clsISUtilities.Compress_DataSet(pvtTempDataSet);
                
                object[] objParm = new object[3];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = this.dgvPayrollTypeDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0,1);
                objParm[2] = pvtbytCompress;
               
                clsISUtilities.DynamicFunction("Update_Records", objParm);
                
                this.lblRunStatus.Text = "Run Pending";
                this.picStatus.Image = global::TimeAttendanceRun.Properties.Resources.Question48; 
                this.pvtDataSet.AcceptChanges();

                btnCancel_Click(sender, e);
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        public void btnUpdate_Click(object sender, System.EventArgs e)
        {
            this.Text += " - Update";

            pvtintPayCategoryRow = this.Get_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView);
            pvtintWeekRow = this.Get_DataGridView_SelectedRowIndex(this.dgvWeekDataGridView);

            Set_Form_For_Edit();
        }

        private void txtRate_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            TextBox myTextBox = (TextBox)sender;

            if (e.KeyChar == (char)8
                | e.KeyChar == (char)46
                | (e.KeyChar > (char)47
                & e.KeyChar < (char)58))
            {
                if (e.KeyChar == (char)46)
                {
                    if (myTextBox.Text.IndexOf(".", 0) > -1)
                    {
                        e.Handled = true;
                    }
                }
                else
                {
                    if (myTextBox.Text.IndexOf(".", 0) > -1
                        & e.KeyChar != (char)8)
                    {
                        if (myTextBox.SelectionStart > myTextBox.Text.IndexOf(".", 0))
                        {
                            //Only 2 Decimal Places
                            if (myTextBox.Text.Substring(myTextBox.Text.IndexOf(".", 0)).Length > 2)
                            {
                                e.Handled = true;
                            }
                        }
                    }
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtRate_Leave(object sender, System.EventArgs e)
        {
            TextBox myTextBox = (TextBox)sender;

            if (myTextBox.Text.Trim() == ""
                | myTextBox.Text.Trim() == ".")
            {
                myTextBox.Text = "100.00";

                return;
            }

            if (Convert.ToDouble(myTextBox.Text) == 0)
            {
                myTextBox.Text = "100.00";

                return;
            }
            else
            {
                if (Convert.ToDouble(myTextBox.Text) < 100)
                {
                    CustomMessageBox.Show("Value must be 100 or Greater",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    myTextBox.Focus();
                    return;
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

                    Load_CurrentForm_Records();
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

                    pvtPaidHolidayDataView = null;
                    pvtPaidHolidayDataView = new DataView(pvtDataSet.Tables["PaidHoliday"],
                        "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")),
                        "",
                        DataViewRowState.CurrentRows);

                    //2017-04-26
                    pvtPayCategoryWeekDataView = null;
                    pvtPayCategoryWeekDataView = new DataView(pvtDataSet.Tables["PayCategoryWeek"],
                        "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_NO = " + this.dgvPayCategoryDataGridView[6, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Value.ToString(),
                        "",
                        DataViewRowState.CurrentRows);

                    this.Clear_DataGridView(this.dgvWeekDataGridView);
                    this.Clear_DataGridView(this.dgvDayThresholdDataGridView);
                    this.Clear_DataGridView(this.dgvPublicHolidayDataGridView);
                    this.Clear_DataGridView(this.dgvOvertimeRateDataGridView);
                    
                    this.txtPaidHolidayRate.Text = "";

                    this.pvtblnWeekDataGridViewLoaded = false;
                    
                    for (int intRowCount = 0; intRowCount < pvtPayCategoryWeekDataView.Count; intRowCount++)
                    {
                        this.dgvWeekDataGridView.Rows.Add("",
                                                          "",
                                                          "",
                                                          Convert.ToDateTime(pvtPayCategoryWeekDataView[intRowCount]["WEEK_DATE_FROM"]).ToString("dd MMM yyyy"),
                                                          Convert.ToDateTime(pvtPayCategoryWeekDataView[intRowCount]["WEEK_DATE"]).ToString("dd MMM yyyy"));

                        //2017-04-26
                        if (this.dgvPayCategoryDataGridView[7, this.Get_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView)].Value.ToString() == "Y")
                        {
                            for (int intRow = 0; intRow < pvtPaidHolidayDataView.Count; intRow++)
                            {
                                if (Convert.ToDateTime(pvtPaidHolidayDataView[intRow]["PUBLIC_HOLIDAY_DATE"]) >= Convert.ToDateTime(pvtPayCategoryWeekDataView[intRowCount]["WEEK_DATE_FROM"])
                                && Convert.ToDateTime(pvtPaidHolidayDataView[intRow]["PUBLIC_HOLIDAY_DATE"]) <= Convert.ToDateTime(pvtPayCategoryWeekDataView[intRowCount]["WEEK_DATE"]))
                                {
                                    this.dgvWeekDataGridView[1,this.dgvWeekDataGridView.Rows.Count - 1].Style = PublicHolidayDataGridViewCellStyle;
                                }
                            }
                        }

                        if (pvtPayCategoryWeekDataView[intRowCount]["ERROR_IND"].ToString() == "X")
                        {
                            this.dgvWeekDataGridView[0,this.dgvWeekDataGridView.Rows.Count - 1].Style = this.ErrorDataGridViewCellStyle;
                        }
                        
                        if (Convert.ToDateTime(pvtPayCategoryWeekDataView[intRowCount]["WEEK_DATE_FROM"]) < Convert.ToDateTime(pvtPayCategoryDataView[this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)]["PAY_PERIOD_DATE_FROM"]))
                        {
                            this.dgvWeekDataGridView[2,this.dgvWeekDataGridView.Rows.Count - 1].Style = OutsideBoundaryDataGridViewCellStyle;
                        }
                    }

                    this.pvtblnWeekDataGridViewLoaded = true;

                    if (pvtPayCategoryWeekDataView.Count > 0)
                    {
                        if (pvtblnTimeSheetError == false)
                        {
                            this.btnRun.Enabled = true;
                        }

                        this.btnUpdate.Enabled = true;

                        this.Set_DataGridView_SelectedRowIndex(this.dgvWeekDataGridView, 0);
                    }
                    else
                    {
                        this.btnRun.Enabled = false;
                        this.btnUpdate.Enabled = false;
                    }
                }
            }
        }

        private void dgvWeekDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (this.pvtblnWeekDataGridViewLoaded == true)
            {
                if (pvtintWeekDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintWeekDataGridViewRowIndex = e.RowIndex;

                    DateTime dtDateFrom = Convert.ToDateTime(pvtPayCategoryWeekDataView[e.RowIndex]["WEEK_DATE_FROM"]);
                    DateTime dtDateTo = Convert.ToDateTime(pvtPayCategoryWeekDataView[e.RowIndex]["WEEK_DATE"]);

                    this.Clear_DataGridView(this.dgvDayThresholdDataGridView);
                    this.Clear_DataGridView(this.dgvOvertimeRateDataGridView);
                    this.Clear_DataGridView(this.dgvPublicHolidayDataGridView);

                    this.txtPaidHolidayRate.Text = "";

                    int intHH = -1;
                    int intMM = -1;
                    double dblDayTime = 0;

                    while (dtDateFrom <= dtDateTo)
                    {
                        if (Convert.ToInt32(pvtPayCategoryWeekDataView[e.RowIndex][dtDateFrom.ToString("ddd").ToUpper() + "_TIME_MINUTES"]) > 59)
                        {
                            intHH = Convert.ToInt32(Convert.ToInt32(pvtPayCategoryWeekDataView[e.RowIndex][dtDateFrom.ToString("ddd").ToUpper() + "_TIME_MINUTES"]) / 60);
                        }
                        else
                        {
                            intHH = 0;
                        }

                        intMM = Convert.ToInt32(Convert.ToInt32(pvtPayCategoryWeekDataView[e.RowIndex][dtDateFrom.ToString("ddd").ToUpper() + "_TIME_MINUTES"]) % 60);

                        this.dgvDayThresholdDataGridView.Rows.Add("",
                                                                  "",
                                                                  "", 
                                                                  dtDateFrom.ToString("dd MMM yyyy"),
                                                                  dtDateFrom.ToString("dddd"),
                                                                  intHH.ToString(),
                                                                  intMM.ToString("00"));
                        
                        DataView PayCategoryDayErrorDataView = new DataView(pvtDataSet.Tables["PayCategoryDayError"],
                            "PAY_CATEGORY_NO = " + pvtPayCategoryWeekDataView[e.RowIndex]["PAY_CATEGORY_NO"].ToString() + " AND TIMESHEET_DATE = '" + dtDateFrom.ToString("yyyy-MM-dd") + "'",
                            "",
                            DataViewRowState.CurrentRows);

                        if (PayCategoryDayErrorDataView.Count > 0)
                        {
                            this.dgvDayThresholdDataGridView[0,this.dgvDayThresholdDataGridView.Rows.Count - 1].Style = this.ErrorDataGridViewCellStyle;
                        }

                        //2013-07-02
                        if (dtDateFrom < Convert.ToDateTime(pvtPayCategoryDataView[this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)]["PAY_PERIOD_DATE_FROM"]))
                        {
                            this.dgvDayThresholdDataGridView[2,this.dgvDayThresholdDataGridView.Rows.Count - 1].Style = OutsideBoundaryDataGridViewCellStyle;
                        }

                        //2018-05-26
                        int intPaidHolidayNo = 0;

                        for (int intRow = 0; intRow < pvtPaidHolidayDataView.Count; intRow++)
                        {
                            //Company Pays Public Holidays 
                            //2017-04-26
                            if (dtDateFrom == Convert.ToDateTime(pvtPaidHolidayDataView[intRow]["PUBLIC_HOLIDAY_DATE"])
                            & this.dgvPayCategoryDataGridView[7, this.Get_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView)].Value.ToString() == "Y")
                            {
                                this.dgvDayThresholdDataGridView.Rows[this.dgvDayThresholdDataGridView.Rows.Count - 1].ReadOnly = true;
                                
                                this.dgvDayThresholdDataGridView[1,this.dgvDayThresholdDataGridView.Rows.Count - 1].Style = PublicHolidayDataGridViewCellStyle;

                                //2018-05-26
                                intPaidHolidayNo += 1;

                                if (Convert.ToInt32(pvtPayCategoryWeekDataView[e.RowIndex]["PAIDHOLIDAY_MINUTES" + intPaidHolidayNo.ToString()]) > 59)
                                {
                                    intHH = Convert.ToInt32(Convert.ToInt32(pvtPayCategoryWeekDataView[e.RowIndex]["PAIDHOLIDAY_MINUTES" + intPaidHolidayNo.ToString()]) / 60);
                                }
                                else
                                {
                                    intHH = 0;
                                }

                                intMM = Convert.ToInt32(Convert.ToInt32(pvtPayCategoryWeekDataView[e.RowIndex]["PAIDHOLIDAY_MINUTES" + intPaidHolidayNo.ToString()]) % 60);

                                dblDayTime = clsISUtilities.Convert_Time_To_Decimal(Convert.ToInt32(pvtPayCategoryWeekDataView[e.RowIndex]["PAIDHOLIDAY_MINUTES" + intPaidHolidayNo.ToString()]));

                                this.dgvPublicHolidayDataGridView.Rows.Add("",
                                                                           dtDateFrom.ToString("dd MMM yyyy"),
                                                                           dtDateFrom.ToString("dddd"),
                                                                           intHH.ToString(),
                                                                           intMM.ToString("00"),
                                                                           dblDayTime.ToString("#0.00"));

                                this.dgvPublicHolidayDataGridView[0,this.dgvPublicHolidayDataGridView.Rows.Count - 1].Style = PublicHolidayDataGridViewCellStyle;
                            }
                        }

                        dtDateFrom = dtDateFrom.AddDays(1);
                    }

                    this.txtPaidHolidayRate.Text = Convert.ToDouble(pvtPayCategoryWeekDataView[e.RowIndex]["PAIDHOLIDAY_RATE"]).ToString("##0.00");

                    if (Convert.ToInt32(pvtPayCategoryWeekDataView[e.RowIndex]["OVERTIME1_MINUTES"]) > 59)
                    {
                        intHH = Convert.ToInt32(Convert.ToInt32(pvtPayCategoryWeekDataView[e.RowIndex]["OVERTIME1_MINUTES"]) / 60);
                    }
                    else
                    {
                        intHH = 0;
                    }

                    intMM = Convert.ToInt32(Convert.ToInt32(pvtPayCategoryWeekDataView[e.RowIndex]["OVERTIME1_MINUTES"]) % 60);


                    this.dgvOvertimeRateDataGridView.Rows.Add("Overtime (1)",
                                                              Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["OVERTIME1_RATE"]).ToString("##0.00"),
                                                              intHH.ToString(),
                                                              intMM.ToString("00"));

                    //OverTime2
                    if (Convert.ToInt32(pvtPayCategoryWeekDataView[e.RowIndex]["OVERTIME2_MINUTES"]) > 59)
                    {
                        intHH = Convert.ToInt32(Convert.ToInt32(pvtPayCategoryWeekDataView[e.RowIndex]["OVERTIME2_MINUTES"]) / 60);
                    }
                    else
                    {
                        intHH = 0;
                    }

                    intMM = Convert.ToInt32(Convert.ToInt32(pvtPayCategoryWeekDataView[e.RowIndex]["OVERTIME2_MINUTES"]) % 60);


                    this.dgvOvertimeRateDataGridView.Rows.Add("Overtime (2)",
                                                                Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["OVERTIME2_RATE"]).ToString("##0.00"),
                                                                intHH.ToString(),
                                                                intMM.ToString("00"));

                    //OverTime3
                    if (Convert.ToInt32(pvtPayCategoryWeekDataView[e.RowIndex]["OVERTIME3_MINUTES"]) > 59)
                    {
                        intHH = Convert.ToInt32(Convert.ToInt32(pvtPayCategoryWeekDataView[e.RowIndex]["OVERTIME3_MINUTES"]) / 60);
                    }
                    else
                    {
                        intHH = 0;
                    }

                    intMM = Convert.ToInt32(Convert.ToInt32(pvtPayCategoryWeekDataView[e.RowIndex]["OVERTIME3_MINUTES"]) % 60);

                    this.dgvOvertimeRateDataGridView.Rows.Add("Overtime (3)",
                                                              Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["OVERTIME3_RATE"]).ToString("##0.00"),
                                                              intHH.ToString(),
                                                              intMM.ToString("00"));
                }
            }
        }

        private void dgvPublicHolidayDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                ComboBox myComboBox = (ComboBox)e.Control;

                if (myComboBox != null)
                {
                    myComboBox.SelectedIndexChanged -= new EventHandler(ComboBox_SelectedIndexChanged);

                    myComboBox.SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);
                }
            }
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int intHH = 0;
            int intMM = 0;
            int intSelectedIndex = ((ComboBox)sender).SelectedIndex;

            if (dgvPublicHolidayDataGridView.CurrentCell.ColumnIndex == 3)
            {
                intHH = intSelectedIndex * 60;
                intMM = Convert.ToInt32(dgvPublicHolidayDataGridView[4, this.Get_DataGridView_SelectedRowIndex(dgvPublicHolidayDataGridView)].Value);
            }
            else
            {
                intHH = Convert.ToInt32(dgvPublicHolidayDataGridView[3, this.Get_DataGridView_SelectedRowIndex(dgvPublicHolidayDataGridView)].Value) * 60;

                if (intSelectedIndex != 0)
                {
                    intMM = intSelectedIndex * 5;
                }
            }

            double dblDayTime = clsISUtilities.Convert_Time_To_Decimal(intHH + intMM);

            dgvPublicHolidayDataGridView[5, this.Get_DataGridView_SelectedRowIndex(dgvPublicHolidayDataGridView)].Value = dblDayTime.ToString("#0.00");
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            this.timer.Enabled = false;

            pvtblnUploadWarningFormHasAlreadyBeenShown = true;
               
            frmUploadWarning.ShowDialog();
        }
            

        private void dgvPayCategoryDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == pvtintLastUploadDateTimeCol)
            {
                if (dgvPayCategoryDataGridView[pvtintLastUploadDateTimeForSortCol, e.RowIndex1].Value.ToString() == "")
                {
                    e.SortResult = -1;
                }
                else
                {
                    if (dgvPayCategoryDataGridView[pvtintLastUploadDateTimeForSortCol, e.RowIndex2].Value.ToString() == "")
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(dgvPayCategoryDataGridView[pvtintLastUploadDateTimeForSortCol, e.RowIndex1].Value.ToString()) > double.Parse(dgvPayCategoryDataGridView[pvtintLastUploadDateTimeForSortCol, e.RowIndex2].Value.ToString()))
                        {
                            e.SortResult = 1;
                        }
                        else
                        {
                            if (double.Parse(dgvPayCategoryDataGridView[pvtintLastUploadDateTimeForSortCol, e.RowIndex1].Value.ToString()) < double.Parse(dgvPayCategoryDataGridView[pvtintLastUploadDateTimeForSortCol, e.RowIndex2].Value.ToString()))
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

        private void timerRun_Tick(object sender, EventArgs e)
        {
            if (pnlImage.Visible == true)
            {
                pnlImage.Visible = false;
            }
            else
            {
                pnlImage.Visible = true;
            }

            if (pvtintNextQueueCheck == 5)
            {
                object[] objParm = new object[2];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));

                if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
                {
                    objParm[1] = "T";
                }
                else
                {
                    objParm[1] = "W";
                }
                
                //Reset to Check in 5 Seconds
                pvtintNextQueueCheck = 0;
                string strReturn = (string)clsISUtilities.DynamicFunction("Check_Queue", objParm);
               
                //S=Started,E=Timesheets Error,F=Job Failed
                if (strReturn == "")
                {
                    //Successful Run
                    this.timerRun.Enabled = false;
                    Set_Form_For_Read();
                    this.btnRun.Text = "Reset";
                    lblRunStatus.Text = "Run Completed Successfully";
                    this.picStatus.Image = global::TimeAttendanceRun.Properties.Resources.tick48;

                    this.grbTimesheetError.Visible = false;
                    this.grbStatus.Visible = true;
                    this.pnlImage.Visible = true;
                }
                else
                {
                    if (strReturn == "E")
                    {
                        this.timerRun.Enabled = false;
                        Set_Form_For_Read();
                        Set_Timesheet_In_Error();
                    }
                    else
                    {
                        if (strReturn == "F")
                        {
                            this.timerRun.Enabled = false;
                            Set_Form_For_Read();
                            Set_Run_Failed();
                        }
                    }
                }
            }
            else
            {
                pvtintNextQueueCheck += 1;
            }
        }
        
        private void Busy_With_Run()
        {
            //Busy Running
            if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
            {
                lblRunStatus.Text = "Busy with Time Attendance Run...";
            }
            else
            {
                lblRunStatus.Text = "Busy with Wage Run...";
            }

            this.pnlImage.Visible = true;
            pvtintNextQueueCheck = 3;
            this.timerRun.Enabled = true;
        }

        private void Set_Timesheet_In_Error()
        {
            this.timerRun.Enabled = false;
            this.btnRun.Text = "Run";
            this.grbStatus.Visible = false;

            this.lblErrors.Text = "There are Timesheets with Errors.\nFix Errors to Continue.";

            this.grbTimesheetError.Visible = true;

            this.btnRun.Enabled = false;
        }

        private void Set_Run_Failed()
        {
            this.timerRun.Enabled = false;

            this.btnRun.Text = "Run";
            this.grbStatus.Visible = false;

            this.grbTimesheetError.Text = this.Text;

            if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
            {
                this.lblErrors.Text = "Time Attendance Run FAILED\n\nSpeak to Administrator.";
            }
            else
            {
                this.lblErrors.Text = "Wage Run FAILED\n\nSpeak to Administrator.";
            }

            this.grbTimesheetError.Visible = true;

            pnlImage.Visible = true;

            this.btnRun.Enabled = false;
        }

        private void frmTimeAttendanceRun_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (AppDomain.CurrentDomain.GetData("Globe") != null)
            {
                Panel myPanelGlobe = (Panel)AppDomain.CurrentDomain.GetData("Globe");
                
                myPanelGlobe.Visible = false;
                myPanelGlobe.Refresh();
                Application.DoEvents();
            }
        }

        private void lblPaidHoliday1_Click(object sender, EventArgs e)
        {

        }
    }
}
