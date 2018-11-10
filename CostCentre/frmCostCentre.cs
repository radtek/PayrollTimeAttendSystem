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
    public partial class frmCostCentre : Form
    {
        clsISUtilities clsISUtilities;
        
        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;
        private DataView pvtUserAuthoriseDataView;
        private DataView pvtTempDataView;

        private DataView pvtPayCategoryDataView;
        private DataView pvtBreakDataView;

        private DataTable pvtDataTable;
        private DataRow pvtdtDataRow;
        private DataRowView pvtDataRowView;

        private int pvtintPayCategoryNo = -1;
        private int pvtintReturnCode = 0;
        private byte[] pvtbytCompress;

        private bool pvtblnBreakTableErrors = false;

        private int pvtintHours = 0;
        private int pvtintMinutes = 0;

        private int pvtintBreakhhCol = 0;
        private int pvtintBreakmmCol = 1;
        private int pvtintTimeWorkedhhCol = 2;
        private int pvtintTimeWorkedmmCol = 3;
        private int pvtintPayCategoryBreakNoCol = 4;

        private string pvtstrPayrollType = "";
        private string pvtstrAuthoriseTypeInd = "";

        private int pvtintLevelNo = 1;

        DataGridViewCellStyle LockedPayrollRunDataGridViewCellStyle;

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnPayCategoryDataGridViewLoaded = false;
        private bool pvtblnAuthoriseTypeDataGridViewLoaded = false;
        private bool pvtblnAuthoriseLevelDataGridViewLoaded = false;
        private bool pvtblnUserDataGridViewLoaded = false;
        private bool pvtblnUserSelectedDataGridViewLoaded = false;
        private bool pvtblnBreakDataGridViewLoaded = false;

        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
        private int pvtintPayCategoryDataGridViewRowIndex = -1;
        private int pvtintAuthoriseTypeDataGridViewRowIndex = -1;
        private int pvtintAuthorisationLevelDataGridViewRowIndex = -1;

        private int pvtintUserDataGridViewRowIndex = -1;
        private int pvtintUserSelectedDataGridViewRowIndex = -1;
        private int pvtintBreakDataGridViewRowIndex = -1;
      
        public frmCostCentre()
        {
            InitializeComponent();
            
            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 95;

                this.dgvPayCategoryDataGridView.Height += 95;

                this.tabControl.Top += 95;
            }
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        public void btnNew_Click(object sender, System.EventArgs e)
        {
            this.Text += " - New";

            pvtDataRowView = this.pvtPayCategoryDataView.AddNew();

            pvtDataRowView.BeginEdit();

            pvtDataRowView["COMPANY_NO"] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
            pvtDataRowView["PAY_CATEGORY_NO"] = 0;
            pvtDataRowView["PAY_CATEGORY_TYPE"] = this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1);
            pvtDataRowView["PAY_CATEGORY_DESC"] = "";
            pvtDataRowView["CLOSED_IND"] = "N";
            pvtDataRowView["PAY_CATEGORY_DEL_IND"] = "Y";

            pvtDataRowView.EndEdit();

            clsISUtilities.DataViewIndex = 0;

            Set_Form_For_Edit();

            //45 Minute Break after 2 Hours
            this.dgvBreakDataGridView[1, 0].Value = "45";
            this.dgvBreakDataGridView[2, 0].Value = "02";

            //DataBind Layer sets all Numeric values to Zero (Must Be Null)
            pvtPayCategoryDataView[0]["PAYROLL_LINK"] = System.DBNull.Value;

            this.txtPayCategory.Focus();
        }

        public void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.pvtDataSet.RejectChanges();

            Set_Form_For_Read();

            //Needs to Be Fixed

            this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView));
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

                    case "dgvAuthorisationLevelDataGridView":

                        pvtintAuthorisationLevelDataGridViewRowIndex = -1;
                        this.dgvAuthorisationLevelDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPayrollTypeDataGridView":

                        pvtintPayrollTypeDataGridViewRowIndex = -1;
                        this.dgvPayrollTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPayCategoryDataGridView":

                        pvtintPayCategoryDataGridViewRowIndex = -1;
                        this.dgvPayCategoryDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvAuthoriseTypeDataGridView":

                        pvtintAuthoriseTypeDataGridViewRowIndex = -1;
                        this.dgvAuthoriseTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvUserDataGridView":

                        pvtintUserDataGridViewRowIndex = -1;
                        this.dgvUserDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvUserSelectedDataGridView":

                        pvtintUserSelectedDataGridViewRowIndex = -1;
                        this.dgvUserSelectedDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    default:

                        MessageBox.Show("No Entry for " + myDataGridView.Name + " in Set_DataGridView_SelectedRowIndex", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            else
            {
                if (myDataGridView.Name == "dgvPayCategoryDataGridView")
                {
                    pvtintPayCategoryDataGridViewRowIndex = -1;
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

        private void frmCostCentre_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this,"busCostCentre");

                LockedPayrollRunDataGridViewCellStyle = new DataGridViewCellStyle();
                LockedPayrollRunDataGridViewCellStyle.BackColor = Color.Magenta;
                LockedPayrollRunDataGridViewCellStyle.SelectionBackColor = Color.Magenta;

                this.lblPayrollTypeSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblCostCentreSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblAuthorisationTypeSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblAuthorisationLevelSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblSelectedUsersSpreadsheetHeaderes.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblUsersSpreadsheetHeaders.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblBreakDesc.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblTimeWorked.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                clsISUtilities.NotDataBound_ComboBox(this.cboOverTime1Hours, "Select Overtime 1 Hours.");
                clsISUtilities.NotDataBound_ComboBox(this.cboOverTime2Hours, "Select Overtime 2 Hours.");
               
                clsISUtilities.NotDataBound_Numeric_TextBox(this.txtPaidHolidayRate,"Public Holiday Rate Must be Greater than 0.",2,10);

                clsISUtilities.NotDataBound_ComboBox(this.cboMinutesExceeds, "Select Day Overtime Minutes.");
                clsISUtilities.NotDataBound_ComboBox(this.cboDailyRoundingMinutes, "Select Daily Rounding Minutes.");
                clsISUtilities.NotDataBound_ComboBox(this.cboPayPeriodRoundingMinutes, "Select Pay Period Rounding Minutes.");

                clsISUtilities.NotDataBound_ComboBox(this.cboSalaryDayPaidHours, "Select Salary Hours Paid per Day.");
                clsISUtilities.NotDataBound_ComboBox(this.cboSalaryDaysInYear, "Select Salary Hourly Rate : Periods in Year.");

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

                this.dgvAuthorisationLevelDataGridView.Rows.Add("First Level");
                this.dgvAuthorisationLevelDataGridView.Rows.Add("Second Level");
                this.dgvAuthorisationLevelDataGridView.Rows.Add("Third Level");

                pvtblnAuthoriseLevelDataGridViewLoaded = true;

                this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView,0);

                string strTimeAttendInd = "N";

                if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
                {
                    strTimeAttendInd = "Y";
                }
  
                object[] objParm = new object[4];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = strTimeAttendInd;
                objParm[2] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[3] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                //Add Here If New PayCategory - OtherWise it Will Fall Over 2011-12-05
                pvtBreakDataView = null;
                pvtBreakDataView = new DataView(this.pvtDataSet.Tables["PayCategoryBreak"],
                    "PAY_CATEGORY_NO = -1 ",
                    "WORKED_TIME_MINUTES,BREAK_MINUTES"
                    , DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < 301; intRow++)
                {
                    this.cboOverTime1Hours.Items.Add(intRow.ToString("00"));
                    this.cboOverTime2Hours.Items.Add(intRow.ToString("00"));
                    this.cboOverTime3Hours.Items.Add(intRow.ToString("00"));
                }

                this.cboOverTime1Hours.Items.Add("999");
                this.cboOverTime2Hours.Items.Add("999");
                this.cboOverTime3Hours.Items.Add("999");

                for (int intRow = 200; intRow < 366; intRow++)
                {
                    cboSalaryDaysInYear.Items.Add(intRow.ToString("00"));
                }

                Load_CurrentForm_Records();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void Load_PayCategory_SpreadSheet()
        {
            this.Clear_DataGridView(this.dgvPayCategoryDataGridView);

            pvtPayCategoryDataView = null;

            string strFilter = "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_TYPE = '" + this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) + "'";

            if (this.rbnActive.Checked == true)
            {
                strFilter += " AND CLOSED_IND <> 'Y' ";
            }
            else
            {
                strFilter += " AND CLOSED_IND = 'Y' ";
            }

            pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"],
                strFilter,
                "PAY_CATEGORY_DESC",
                DataViewRowState.CurrentRows);
            
            clsISUtilities.DataViewIndex = 0;

            if (clsISUtilities.DataBind_Form_And_DataView_To_Class() == false)
            {
                clsISUtilities.DataBind_DataView_And_Index(this, pvtPayCategoryDataView, "PAY_CATEGORY_NO");

                clsISUtilities.DataBind_DataView_To_TextBox(this.txtPayCategory, "PAY_CATEGORY_DESC", true, "Enter Cost Centre Description.", true);

                //Needs To Be Looked At For EFiling
                clsISUtilities.DataBind_DataView_To_TextBox(this.txtUnitNumber, "RES_UNIT_NUMBER", false, "", true);
                clsISUtilities.DataBind_DataView_To_TextBox(this.txtComplex, "RES_COMPLEX", false, "", true);
                clsISUtilities.DataBind_DataView_To_TextBox(this.txtStreetNumber, "RES_STREET_NUMBER", false, "", true);

                clsISUtilities.DataBind_DataView_To_TextBox(this.txtCostCentreStreetName, "RES_STREET_NAME", false, "", true);
                clsISUtilities.DataBind_Special_Field(this.txtCostCentreStreetName, 1);

                clsISUtilities.DataBind_DataView_To_TextBox(this.txtSuburb, "RES_SUBURB", false, "", true);
                clsISUtilities.DataBind_Special_Field(this.txtSuburb, 1);
                clsISUtilities.DataBind_DataView_To_TextBox(this.txtCity, "RES_CITY", false, "", true);
                clsISUtilities.DataBind_Special_Field(this.txtCity, 1);

                clsISUtilities.DataBind_DataView_To_TextBox(this.txtPhysicalCode, "RES_ADDR_CODE", false, "", true);
                clsISUtilities.DataBind_Special_Field(this.txtPhysicalCode, 1);

                clsISUtilities.DataBind_DataView_To_TextBox(txtPostAddr1, "POST_ADDR_LINE1", false, "", true);
                clsISUtilities.DataBind_Special_Field(this.txtPostAddr1, 2);
                clsISUtilities.DataBind_DataView_To_TextBox(txtPostAddr2, "POST_ADDR_LINE2", false, "", true);
                clsISUtilities.DataBind_Special_Field(this.txtPostAddr2, 2);
                clsISUtilities.DataBind_DataView_To_TextBox(txtPostAddr3, "POST_ADDR_LINE3", false, "", true);
                clsISUtilities.DataBind_Special_Field(this.txtPostAddr3, 2);
                clsISUtilities.DataBind_DataView_To_TextBox(this.txtPostAddr4, "POST_ADDR_LINE4", false, "", true);

                clsISUtilities.DataBind_DataView_To_TextBox(this.txtPostAddrCode, "POST_ADDR_CODE", false, "", true);
                clsISUtilities.DataBind_Special_Field(this.txtPostAddrCode, 2);
            }
            else
            {
                //Rebind dataView
                clsISUtilities.Re_DataBind_DataView(pvtPayCategoryDataView);
            }
            
            if (pvtPayCategoryDataView.Count == 0)
            {
                Clear_Form_Fields();

                Set_Form_For_Read();
            }
            else
            {
                int intSelectedRow = 0;

                Set_Form_For_Read();

                this.pvtblnPayCategoryDataGridViewLoaded = false;

                for (int intRowCount = 0; intRowCount < pvtPayCategoryDataView.Count; intRowCount++)
                {
                    this.dgvPayCategoryDataGridView.Rows.Add("",
                                                             pvtPayCategoryDataView[intRowCount]["PAY_CATEGORY_DESC"].ToString(),
                                                             intRowCount.ToString());


                    if (Convert.ToInt32(pvtPayCategoryDataView[intRowCount]["PAY_CATEGORY_NO"]) == pvtintPayCategoryNo)
                    {
                        intSelectedRow = intRowCount;
                    }

                    if (pvtPayCategoryDataView[intRowCount]["PAYROLL_LINK"] != System.DBNull.Value)
                    {
                        this.dgvPayCategoryDataGridView[0,this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = LockedPayrollRunDataGridViewCellStyle;
                        grbCostCentreLock.Visible = true;
                    }
                }

                this.pvtblnPayCategoryDataGridViewLoaded = true;

                this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, intSelectedRow);
            }
        }

        private void Set_Form_For_Read()
        {
            if (this.Text.IndexOf("- New") > -1)
            {
                this.Text = this.Text.Substring(0, this.Text.IndexOf("- New") - 1);
            }
            else
            {
                if (this.Text.IndexOf("- Update") > -1)
                {
                    this.Text = this.Text.Substring(0, this.Text.IndexOf("- Update") - 1);
                }
            }

            clsISUtilities.Paint_Parent_Marker(this.lblBreakDesc, false);
     
            this.txtPayCategory.Enabled = true;

            this.btnBreakDeleteRec.Enabled = false;

            this.cboMonTimeHours.Enabled = false;
            this.cboTueTimeHours.Enabled = false;
            this.cboWedTimeHours.Enabled = false;
            this.cboThuTimeHours.Enabled = false;
            this.cboFriTimeHours.Enabled = false;
            this.cboSatTimeHours.Enabled = false;
            this.cboSunTimeHours.Enabled = false;
            this.cboMonTimeMinutes.Enabled = false;
            this.cboTueTimeMinutes.Enabled = false;
            this.cboWedTimeMinutes.Enabled = false;
            this.cboThuTimeMinutes.Enabled = false;
            this.cboFriTimeMinutes.Enabled = false;
            this.cboSatTimeMinutes.Enabled = false;
            this.cboSunTimeMinutes.Enabled = false;

            this.cboShiftAbove.Enabled = false;
            this.cboShiftBelow.Enabled = false;

            this.cboOverTime1Hours.Enabled = false;
            this.cboOverTime1Minutes.Enabled = false;
            this.rbnHours1.Enabled = false;

            this.cboOverTime2Hours.Enabled = false;
            this.cboOverTime2Minutes.Enabled = false;
            this.rbnHours2.Enabled = false;

            this.cboOverTime3Hours.Enabled = false;
            this.cboOverTime3Minutes.Enabled = false;
            this.rbnHours3.Enabled = false;
       
            this.rbnDayAlways.Enabled = false;
            this.rbnPeriodExceeded.Enabled = false;

            this.cboHoursExceeds.Enabled = false;
            this.cboMinutesExceeds.Enabled = false;

            this.cboRateSat.Enabled = false;
            this.rbnSatAlways.Enabled = false;
            this.rbnSatUseOption.Enabled = false;

            this.cboRateSun.Enabled = false;
            this.rbnSunAlways.Enabled = false;
            this.rbnSunUseOption.Enabled = false;

            this.txtPaidHolidayRate.Enabled = false;
            this.chkPayPublicHoliday.Enabled = false;
            
            this.cboDailyRounding.Enabled = false;
            this.cboDailyRoundingMinutes.Enabled = false;

            this.cboPayPeriodRounding.Enabled = false;
            this.cboPayPeriodRoundingMinutes.Enabled = false;

            this.cboSalaryDayPaidHours.Enabled = false;
            this.cboSalaryDayPaidMinutes.Enabled = false;
            this.cboSalaryDaysInYear.Enabled = false;

            if (this.rbnActive.Checked == true)
            {
                this.btnUpdate.Enabled = true;
            }
            else
            {
                this.btnNew.Enabled = false;
                this.btnUpdate.Enabled = false;
            }
            
            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.dgvPayCategoryDataGridView.Enabled = true;
            this.dgvPayrollTypeDataGridView.Enabled = true;

            this.dgvBreakDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvBreakDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
         
            this.picPayCategoryLock.Visible = false;
            this.picPayrollTypeLock.Visible = false;

            this.btnAdd.Enabled = false;
            this.btnRemove.Enabled = false;

            this.rbnUploadNo.Enabled = false;
            this.rbnUploadYes.Enabled = false;

            clsISUtilities.Set_Form_For_Read();

            if (this.pvtDataSet.Tables["Company"].Rows[0]["ACCESS_IND"].ToString() == "A")
            {
                if (this.rbnActive.Checked == true)
                {
                    this.btnNew.Enabled = true;
                }
            }

            if (pvtPayCategoryDataView.Count == 0)
            {
                this.btnUpdate.Enabled = false;
                this.btnDelete.Enabled = false;
            }

            this.rbnActive.Enabled = true;
            this.rbnClosed.Enabled = true;

            if (this.rbnActive.Checked == true)
            {
                this.chkClose.Checked = false;
            }

            this.chkClose.Enabled = false;
        }

        private void Set_Form_For_Edit()
        {
            System.EventArgs e = new EventArgs();

            bool blnNew = true;

            this.btnNew.Enabled = false;
            this.btnUpdate.Enabled = false;
            this.btnDelete.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.rbnActive.Enabled = false;
            this.rbnClosed.Enabled = false;

            this.btnBreakDeleteRec.Enabled = true;

            this.dgvPayCategoryDataGridView.Enabled = false;
            this.dgvPayrollTypeDataGridView.Enabled = false;

            this.picPayCategoryLock.Visible = true;
            this.picPayrollTypeLock.Visible = true;
     
            this.cboMonTimeHours.Enabled = true;
            this.cboTueTimeHours.Enabled = true;
            this.cboWedTimeHours.Enabled = true;
            this.cboThuTimeHours.Enabled = true;
            this.cboFriTimeHours.Enabled = true;
            this.cboSatTimeHours.Enabled = true;
            this.cboSunTimeHours.Enabled = true;
            this.cboMonTimeMinutes.Enabled = true;
            this.cboTueTimeMinutes.Enabled = true;
            this.cboWedTimeMinutes.Enabled = true;
            this.cboThuTimeMinutes.Enabled = true;
            this.cboFriTimeMinutes.Enabled = true;
            this.cboSatTimeMinutes.Enabled = true;
            this.cboSunTimeMinutes.Enabled = true;

            this.cboShiftAbove.Enabled = true;
            this.cboShiftBelow.Enabled = true;

            this.cboOverTime1Hours.Enabled = true;
            this.cboOverTime1Minutes.Enabled = true;

            this.rbnDayAlways.Checked = true;

            //this.rbnDayAlways.Enabled = true;
            //this.rbnPeriodExceeded.Enabled = true;
          
            this.cboHoursExceeds.Enabled = true;
            this.cboRateSat.Enabled = true;
            this.cboRateSun.Enabled = true;
            this.cboDailyRounding.Enabled = true;
            this.cboPayPeriodRounding.Enabled = true;

            this.txtPaidHolidayRate.Enabled = true;
            
            if (this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) == "S")
            {
                this.cboSalaryDayPaidHours.Enabled = true;
                this.cboSalaryDayPaidMinutes.Enabled = true;

                this.cboSalaryDaysInYear.Enabled = true;
            }
            else
            {
                this.chkPayPublicHoliday.Enabled = true;
            }

            this.btnAdd.Enabled = true;
            this.btnRemove.Enabled = true;

            if (this.Text.EndsWith(" - New") == true)
            {
                if (this.pvtDataSet.Tables["Company"].Rows[0]["DYNAMIC_UPLOAD_KEY"].ToString() != "")
                {
                    this.rbnUploadNo.Enabled = true;
                    this.rbnUploadYes.Enabled = true;
                }

                this.rbnUploadNo.Checked = true;

                this.txtPaidHolidayRate.Text = "1.00";
                
                this.cboMonTimeHours.SelectedIndex = 8;
                this.cboTueTimeHours.SelectedIndex = 8;
                this.cboWedTimeHours.SelectedIndex = 8;
                this.cboThuTimeHours.SelectedIndex = 8;
                this.cboFriTimeHours.SelectedIndex = 8;
                this.cboSatTimeHours.SelectedIndex = 0;
                this.cboSunTimeHours.SelectedIndex = 0;

                this.cboMonTimeMinutes.SelectedIndex = 0;
                this.cboTueTimeMinutes.SelectedIndex = 0;
                this.cboWedTimeMinutes.SelectedIndex = 0;
                this.cboThuTimeMinutes.SelectedIndex = 0;
                this.cboFriTimeMinutes.SelectedIndex = 0;
                this.cboSatTimeMinutes.SelectedIndex = 0;
                this.cboSunTimeMinutes.SelectedIndex = 0;

                this.cboShiftAbove.SelectedIndex = 2;
                this.cboShiftBelow.SelectedIndex = 2;

                this.cboHoursExceeds.SelectedIndex = 0;
                this.cboRateSat.SelectedIndex = 0;
                this.cboRateSun.SelectedIndex = 0;
                this.cboDailyRounding.SelectedIndex = 0;
                this.cboPayPeriodRounding.SelectedIndex = 0;
                
                this.rbnHours1.Checked = true;
              
                if (this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) == "S")
                {
                    this.cboSalaryDayPaidHours.SelectedIndex = -1;
                    this.cboSalaryDayPaidMinutes.SelectedIndex = 0;

                    this.cboSalaryDaysInYear.SelectedIndex = -1;
                }
                else
                {
                    this.chkPayPublicHoliday.Checked = true;
                }

                this.Clear_DataGridView(this.dgvBreakDataGridView);
            }
            else
            {
                if (this.pvtDataSet.Tables["Company"].Rows[0]["DYNAMIC_UPLOAD_KEY"].ToString() != "")
                {
                    this.rbnUploadNo.Enabled = true;
                    this.rbnUploadYes.Enabled = true;
                }
                else
                {
                    this.rbnUploadNo.Checked = true;
                }

                blnNew = false;

                if (this.rbnActive.Checked == true)
                {
                    this.chkClose.Enabled = true;
                    this.chkClose.Checked = false;
                }
            }

            clsISUtilities.Set_Form_For_Edit(blnNew);

            this.rbnHours1.Enabled = true;
            this.rbnHours2.Enabled = true;
            this.rbnHours3.Enabled = true;
            
            cboHoursExceeds_SelectedIndexChanged(null, e);

            cboDailyRounding_SelectedIndexChanged(null, e);
            cboPayPeriodRounding_SelectedIndexChanged(null, e);

            this.cboRateSat_SelectedIndexChanged(null, e);
            this.cboRateSun_SelectedIndexChanged(null, e);

            if (this.rbnHours1.Checked == true)
            {
                this.rbnHours1_CheckedChanged(null, e);
            }
            else
            {
                if (this.rbnHours2.Checked == true)
                {
                    rbnHours2_CheckedChanged(null, e);
                }
                else
                {
                    rbnHours3_CheckedChanged(null, e);
                }
            }
           
            this.dgvAuthoriseTypeDataGridView.Enabled = true;
            this.dgvUserDataGridView.Enabled = true;
            this.dgvUserSelectedDataGridView.Enabled = true;
            
            this.dgvBreakDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dgvBreakDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;
    
            this.txtRate1.Enabled = false;
            this.txtRate2.Enabled = false;
            this.txtRate3.Enabled = false;
                               
            if (Convert.ToDouble(this.txtRate2.Text) == 0)
            {
                this.cboOverTime2Hours.SelectedIndex = 0;
                this.cboOverTime2Minutes.SelectedIndex = 0;
                this.cboOverTime2Hours.Enabled = false;
                this.cboOverTime2Minutes.Enabled = false;

                this.rbnHours2.Enabled = false;
            }

            if (Convert.ToDouble(this.txtRate3.Text) == 0)
            {
                this.cboOverTime3Hours.SelectedIndex = 0;
                this.cboOverTime3Minutes.SelectedIndex = 0;
                this.cboOverTime3Hours.Enabled = false;
                this.cboOverTime3Minutes.Enabled = false;

                this.rbnHours3.Enabled = false;
            }

            this.Check_To_Add_New_Break_Row();
        }

        private void Clear_Form_Fields()
        {
            this.txtPayCategory.Text = "";

            this.cboMonTimeHours.SelectedIndex = -1;
            this.cboTueTimeHours.SelectedIndex = -1;
            this.cboWedTimeHours.SelectedIndex = -1;
            this.cboThuTimeHours.SelectedIndex = -1;
            this.cboFriTimeHours.SelectedIndex = -1;
            this.cboSatTimeHours.SelectedIndex = -1;
            this.cboSunTimeHours.SelectedIndex = -1;
            this.cboMonTimeMinutes.SelectedIndex = -1;
            this.cboTueTimeMinutes.SelectedIndex = -1;
            this.cboWedTimeMinutes.SelectedIndex = -1;
            this.cboThuTimeMinutes.SelectedIndex = -1;
            this.cboFriTimeMinutes.SelectedIndex = -1;
            this.cboSatTimeMinutes.SelectedIndex = -1;
            this.cboSunTimeMinutes.SelectedIndex = -1;

            this.cboShiftAbove.SelectedIndex = -1;
            this.cboShiftBelow.SelectedIndex = -1;

            this.cboOverTime1Hours.SelectedIndex = -1;
            this.cboOverTime1Minutes.SelectedIndex = -1;

            this.cboOverTime2Hours.SelectedIndex = -1;
            this.cboOverTime2Minutes.SelectedIndex = -1;

            this.cboOverTime3Hours.SelectedIndex = -1;
            this.cboOverTime3Minutes.SelectedIndex = -1;

            this.cboHoursExceeds.SelectedIndex = -1;
            this.cboMinutesExceeds.SelectedIndex = -1;

            this.txtPaidHolidayRate.Text = "0.00";

            this.cboRateSat.SelectedIndex = -1;

            this.cboRateSun.SelectedIndex = -1;

            this.cboDailyRounding.SelectedIndex = -1;
            this.cboPayPeriodRounding.SelectedIndex = -1;

            this.Clear_DataGridView(this.dgvBreakDataGridView);
       }

        private void Load_CurrentForm_Records()
        {
            grbCostCentreLock.Visible = false;

            if (pvtDataSet.Tables["Company"].Rows.Count == 0)
            {
                this.btnNew.Enabled = false;
            }
            else
            {
                this.Clear_DataGridView(this.dgvBreakDataGridView);
                this.Clear_DataGridView(this.dgvAuthoriseTypeDataGridView);

                tabControl.SelectedIndex = 0;

                if (this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) != "S")
                {
                    this.chkPayPublicHoliday.Visible = true;
                    this.grbSalaryHourlyRate.Visible = false;

                    this.txtRate1.Text = Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["OVERTIME1_RATE"]).ToString("##0.00");
                    this.txtRate2.Text = Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["OVERTIME2_RATE"]).ToString("##0.00");
                    this.txtRate3.Text = Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["OVERTIME3_RATE"]).ToString("##0.00");

                    if (pvtDataSet.Tables["OVERTIME_RATE"] != null)
                    {
                        pvtDataSet.Tables.Remove("OVERTIME_RATE");
                    }

                    pvtDataTable = new DataTable("OVERTIME_RATE");
                    pvtDataTable.Columns.Add("OVERTIME_RATE", typeof(System.Double));
                    pvtDataTable.Columns.Add("OVERTIME_RATE_DESC", typeof(System.String));
                    pvtDataSet.Tables.Add(pvtDataTable);

                    pvtdtDataRow = pvtDataSet.Tables["OVERTIME_RATE"].NewRow();
                    pvtdtDataRow["OVERTIME_RATE"] = 0;
                    pvtdtDataRow["OVERTIME_RATE_DESC"] = "(None)";
                    pvtDataSet.Tables["OVERTIME_RATE"].Rows.Add(pvtdtDataRow);

                    if (Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["OVERTIME1_RATE"]) > 0)
                    {
                        pvtdtDataRow = pvtDataSet.Tables["OVERTIME_RATE"].NewRow();
                        pvtdtDataRow["OVERTIME_RATE"] = Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["OVERTIME1_RATE"]);
                        pvtdtDataRow["OVERTIME_RATE_DESC"] = Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["OVERTIME1_RATE"]).ToString("##0.00");
                        pvtDataSet.Tables["OVERTIME_RATE"].Rows.Add(pvtdtDataRow);
                    }

                    if (Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["OVERTIME2_RATE"]) > 0)
                    {
                        pvtdtDataRow = pvtDataSet.Tables["OVERTIME_RATE"].NewRow();
                        pvtdtDataRow["OVERTIME_RATE"] = Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["OVERTIME2_RATE"]);
                        pvtdtDataRow["OVERTIME_RATE_DESC"] = Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["OVERTIME2_RATE"]).ToString("##0.00");
                        pvtDataSet.Tables["OVERTIME_RATE"].Rows.Add(pvtdtDataRow);
                    }

                    if (Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["OVERTIME3_RATE"]) > 0)
                    {
                        pvtdtDataRow = pvtDataSet.Tables["OVERTIME_RATE"].NewRow();
                        pvtdtDataRow["OVERTIME_RATE"] = Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["OVERTIME3_RATE"]);
                        pvtdtDataRow["OVERTIME_RATE_DESC"] = Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["OVERTIME3_RATE"]).ToString("##0.00");
                        pvtDataSet.Tables["OVERTIME_RATE"].Rows.Add(pvtdtDataRow);
                    }

                    this.cboRateSat.Items.Clear();
                    this.cboRateSun.Items.Clear();

                    for (int intRow = 0; intRow < this.pvtDataSet.Tables["OVERTIME_RATE"].Rows.Count; intRow++)
                    {
                        this.cboRateSat.Items.Add(this.pvtDataSet.Tables["OVERTIME_RATE"].Rows[intRow]["OVERTIME_RATE_DESC"].ToString());
                        this.cboRateSun.Items.Add(this.pvtDataSet.Tables["OVERTIME_RATE"].Rows[intRow]["OVERTIME_RATE_DESC"].ToString());
                    }
                }
                else
                {
                    this.chkPayPublicHoliday.Visible = false;
                    this.grbSalaryHourlyRate.Visible = true;

                    this.txtRate1.Text = Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME1_RATE"]).ToString("##0.00");
                    this.txtRate2.Text = Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME2_RATE"]).ToString("##0.00");
                    this.txtRate3.Text = Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME3_RATE"]).ToString("##0.00");

                    if (pvtDataSet.Tables["OVERTIME_RATE"] != null)
                    {
                        pvtDataSet.Tables.Remove("OVERTIME_RATE");
                    }

                    pvtDataTable = new DataTable("OVERTIME_RATE");
                    pvtDataTable.Columns.Add("OVERTIME_RATE", typeof(System.Double));
                    pvtDataTable.Columns.Add("OVERTIME_RATE_DESC", typeof(System.String));
                    pvtDataSet.Tables.Add(pvtDataTable);

                    pvtdtDataRow = pvtDataSet.Tables["OVERTIME_RATE"].NewRow();
                    pvtdtDataRow["OVERTIME_RATE"] = 0;
                    pvtdtDataRow["OVERTIME_RATE_DESC"] = "(None)";
                    pvtDataSet.Tables["OVERTIME_RATE"].Rows.Add(pvtdtDataRow);

                    if (Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["OVERTIME1_RATE"]) > 0)
                    {
                        pvtdtDataRow = pvtDataSet.Tables["OVERTIME_RATE"].NewRow();
                        pvtdtDataRow["OVERTIME_RATE"] = Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME1_RATE"]);
                        pvtdtDataRow["OVERTIME_RATE_DESC"] = Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME1_RATE"]).ToString("##0.00");
                        pvtDataSet.Tables["OVERTIME_RATE"].Rows.Add(pvtdtDataRow);
                    }

                    if (Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["OVERTIME2_RATE"]) > 0)
                    {
                        pvtdtDataRow = pvtDataSet.Tables["OVERTIME_RATE"].NewRow();
                        pvtdtDataRow["OVERTIME_RATE"] = Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME2_RATE"]);
                        pvtdtDataRow["OVERTIME_RATE_DESC"] = Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME2_RATE"]).ToString("##0.00");
                        pvtDataSet.Tables["OVERTIME_RATE"].Rows.Add(pvtdtDataRow);
                    }

                    if (Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["OVERTIME3_RATE"]) > 0)
                    {
                        pvtdtDataRow = pvtDataSet.Tables["OVERTIME_RATE"].NewRow();
                        pvtdtDataRow["OVERTIME_RATE"] = Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME3_RATE"]);
                        pvtdtDataRow["OVERTIME_RATE_DESC"] = Convert.ToDouble(pvtDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME3_RATE"]).ToString("##0.00");
                        pvtDataSet.Tables["OVERTIME_RATE"].Rows.Add(pvtdtDataRow);
                    }

                    this.cboRateSat.Items.Clear();
                    this.cboRateSun.Items.Clear();

                    for (int intRow = 0; intRow < this.pvtDataSet.Tables["OVERTIME_RATE"].Rows.Count; intRow++)
                    {
                        this.cboRateSat.Items.Add(this.pvtDataSet.Tables["OVERTIME_RATE"].Rows[intRow]["OVERTIME_RATE_DESC"].ToString());
                        this.cboRateSun.Items.Add(this.pvtDataSet.Tables["OVERTIME_RATE"].Rows[intRow]["OVERTIME_RATE_DESC"].ToString());
                    }
               }

               Load_PayCategory_SpreadSheet();
            }
        }

        private void rbnHours3_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rbnHours3.Checked == true)
            {
                this.cboOverTime3Hours.SelectedIndex = this.cboOverTime3Hours.Items.Count - 1;
                this.cboOverTime3Minutes.SelectedIndex = 0;
                this.cboOverTime3Hours.Enabled = false;
                this.cboOverTime3Minutes.Enabled = false;

                if (this.btnSave.Enabled == true)
                {
                    this.cboOverTime1Hours.Enabled = true;
                    this.cboOverTime1Minutes.Enabled = true;

                    this.cboOverTime2Hours.Enabled = true;
                    this.cboOverTime2Minutes.Enabled = true;
                }
            }
        }

        private void rbnHours2_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rbnHours2.Checked == true)
            {
                this.cboOverTime2Hours.SelectedIndex = this.cboOverTime3Hours.Items.Count - 1;
                this.cboOverTime2Minutes.SelectedIndex = 0;
                this.cboOverTime2Hours.Enabled = false;
                this.cboOverTime2Minutes.Enabled = false;

                this.cboOverTime3Hours.SelectedIndex = 0;
                this.cboOverTime3Minutes.SelectedIndex = 0;
                this.cboOverTime3Hours.Enabled = false;
                this.cboOverTime3Minutes.Enabled = false;
                
                if (this.btnSave.Enabled == true)
                {
                    this.cboOverTime1Hours.Enabled = true;
                    this.cboOverTime1Minutes.Enabled = true;
                }
            }
        }

        private void rbnHours1_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rbnHours1.Checked == true)
            {
                this.cboOverTime1Hours.SelectedIndex = this.cboOverTime1Hours.Items.Count - 1;
                this.cboOverTime1Minutes.SelectedIndex = 0;
                this.cboOverTime1Hours.Enabled = false;
                this.cboOverTime1Minutes.Enabled = false;

                this.cboOverTime2Hours.SelectedIndex = 0;
                this.cboOverTime2Minutes.SelectedIndex = 0;
                this.cboOverTime2Hours.Enabled = false;
                this.cboOverTime2Minutes.Enabled = false;
                
                this.cboOverTime3Hours.SelectedIndex = 0;
                this.cboOverTime3Minutes.SelectedIndex = 0;
                this.cboOverTime3Hours.Enabled = false;
                this.cboOverTime3Minutes.Enabled = false;
            }
        }

        public void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (pvtblnBreakTableErrors == true)
                {
                    CustomMessageBox.Show("Errors Exist in the Breaks Spreadsheet.\nFix to Continue.",
                               this.Text,
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error);
                    return;
                }

                //Check Non-DataBound Fields
                int intReturnCode = clsISUtilities.DataBind_Save_Check();

                if (intReturnCode != 0)
                {
                    return;
                }
                else
                {
                    if (this.cboSalaryDayPaidHours.Visible == true)
                    {
                        //Salaries
                        if (Convert.ToInt32(this.cboSalaryDayPaidHours.SelectedItem) == 0
                            & Convert.ToInt32(this.cboSalaryDayPaidMinutes.SelectedItem) == 0)
                        {
                            CustomMessageBox.Show("Select Salary Hourly Rate : Hours Paid per Day (Cannot be Zero).",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                            this.cboSalaryDayPaidHours.Focus();

                            return;
                        }
                    }

                    if (this.chkClose.Checked == true)
                    {
                        DialogResult dlgResult = CustomMessageBox.Show("Are you sure you want to Close Cost Centre '" + this.dgvPayCategoryDataGridView[1,pvtintPayCategoryDataGridViewRowIndex].Value.ToString() + "'?",
                        this.Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                        if (dlgResult == DialogResult.Yes)
                        {
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                int intTime = 0;
                int intShiftAbove = Convert.ToInt32(this.cboShiftAbove.SelectedItem.ToString());
                int intShiftBelow = Convert.ToInt32(this.cboShiftBelow.SelectedItem.ToString());

                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["MON_TIME_MINUTES"] = (Convert.ToInt32(this.cboMonTimeHours.SelectedItem) * 60) + Convert.ToInt32(this.cboMonTimeMinutes.SelectedItem);
                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["TUE_TIME_MINUTES"] = (Convert.ToInt32(this.cboTueTimeHours.SelectedItem) * 60) + Convert.ToInt32(this.cboTueTimeMinutes.SelectedItem);
                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["WED_TIME_MINUTES"] = (Convert.ToInt32(this.cboWedTimeHours.SelectedItem) * 60) + Convert.ToInt32(this.cboWedTimeMinutes.SelectedItem);
                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["THU_TIME_MINUTES"] = (Convert.ToInt32(this.cboThuTimeHours.SelectedItem) * 60) + Convert.ToInt32(this.cboThuTimeMinutes.SelectedItem);
                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["FRI_TIME_MINUTES"] = (Convert.ToInt32(this.cboFriTimeHours.SelectedItem) * 60) + Convert.ToInt32(this.cboFriTimeMinutes.SelectedItem);
                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SAT_TIME_MINUTES"] = (Convert.ToInt32(this.cboSatTimeHours.SelectedItem) * 60) + Convert.ToInt32(this.cboSatTimeMinutes.SelectedItem);
                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SUN_TIME_MINUTES"] = (Convert.ToInt32(this.cboSunTimeHours.SelectedItem) * 60) + Convert.ToInt32(this.cboSunTimeMinutes.SelectedItem);

                if (this.cboHoursExceeds.SelectedIndex == 0)
                {
                    pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["TOTAL_DAILY_TIME_OVERTIME"] = 0;
                }
                else
                {
                    pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["TOTAL_DAILY_TIME_OVERTIME"] = (Convert.ToInt32(this.cboHoursExceeds.SelectedItem) * 60) + Convert.ToInt32(this.cboMinutesExceeds.SelectedItem);
                }

                intTime = (Convert.ToInt32(this.cboOverTime1Hours.SelectedItem) * 60) + Convert.ToInt32(this.cboOverTime1Minutes.SelectedItem);

                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["OVERTIME1_MINUTES"] = intTime;

                intTime = (Convert.ToInt32(this.cboOverTime2Hours.SelectedItem) * 60) + Convert.ToInt32(this.cboOverTime2Minutes.SelectedItem);

                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["OVERTIME2_MINUTES"] = Convert.ToDouble(intTime);

                intTime = (Convert.ToInt32(this.cboOverTime3Hours.SelectedItem) * 60) + Convert.ToInt32(this.cboOverTime3Minutes.SelectedItem);

                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["OVERTIME3_MINUTES"] = Convert.ToDouble(intTime);

                int intTimeUp = 0;
                int intTimeDown = 0;

                intTimeUp = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SUN_TIME_MINUTES"]) * intShiftAbove / 100;
                intTimeDown = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SUN_TIME_MINUTES"]) * intShiftBelow / 100;
                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["EXCEPTION_SUN_ABOVE_MINUTES"] = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SUN_TIME_MINUTES"]) + intTimeUp;
                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["EXCEPTION_SUN_BELOW_MINUTES"] = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SUN_TIME_MINUTES"]) - intTimeDown;

                intTimeUp = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["MON_TIME_MINUTES"]) * intShiftAbove / 100;
                intTimeDown = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["MON_TIME_MINUTES"]) * intShiftBelow / 100;
                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["EXCEPTION_MON_ABOVE_MINUTES"] = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["MON_TIME_MINUTES"]) + intTimeUp;
                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["EXCEPTION_MON_BELOW_MINUTES"] = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["MON_TIME_MINUTES"]) - intTimeDown;

                intTimeUp = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["TUE_TIME_MINUTES"]) * intShiftAbove / 100;
                intTimeDown = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["TUE_TIME_MINUTES"]) * intShiftBelow / 100;
                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["EXCEPTION_TUE_ABOVE_MINUTES"] = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["TUE_TIME_MINUTES"]) + intTimeUp;
                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["EXCEPTION_TUE_BELOW_MINUTES"] = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["TUE_TIME_MINUTES"]) - intTimeDown;

                intTimeUp = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["WED_TIME_MINUTES"]) * intShiftAbove / 100;
                intTimeDown = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["WED_TIME_MINUTES"]) * intShiftBelow / 100;
                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["EXCEPTION_WED_ABOVE_MINUTES"] = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["WED_TIME_MINUTES"]) + intTimeUp;
                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["EXCEPTION_WED_BELOW_MINUTES"] = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["WED_TIME_MINUTES"]) - intTimeDown;

                intTimeUp = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["THU_TIME_MINUTES"]) * intShiftAbove / 100;
                intTimeDown = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["THU_TIME_MINUTES"]) * intShiftBelow / 100;
                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["EXCEPTION_THU_ABOVE_MINUTES"] = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["THU_TIME_MINUTES"]) + intTimeUp;
                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["EXCEPTION_THU_BELOW_MINUTES"] = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["THU_TIME_MINUTES"]) - intTimeDown;

                intTimeUp = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["FRI_TIME_MINUTES"]) * intShiftAbove / 100;
                intTimeDown = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["FRI_TIME_MINUTES"]) * intShiftBelow / 100;
                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["EXCEPTION_FRI_ABOVE_MINUTES"] = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["FRI_TIME_MINUTES"]) + intTimeUp;
                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["EXCEPTION_FRI_BELOW_MINUTES"] = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["FRI_TIME_MINUTES"]) - intTimeDown;

                intTimeUp = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SAT_TIME_MINUTES"]) * intShiftAbove / 100;
                intTimeDown = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SAT_TIME_MINUTES"]) * intShiftBelow / 100;
                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["EXCEPTION_SAT_ABOVE_MINUTES"] = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SAT_TIME_MINUTES"]) + intTimeUp;
                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["EXCEPTION_SAT_BELOW_MINUTES"] = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SAT_TIME_MINUTES"]) - intTimeDown;

                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["DAILY_ROUNDING_IND"] = this.cboDailyRounding.SelectedIndex;
                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["DAILY_ROUNDING_MINUTES"] = 0;

                if (this.cboDailyRounding.SelectedIndex != 0)
                {
                    pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["DAILY_ROUNDING_MINUTES"] = Convert.ToDouble(this.cboDailyRoundingMinutes.SelectedItem);
                }

                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["PAY_PERIOD_ROUNDING_IND"] = this.cboPayPeriodRounding.SelectedIndex;
                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["PAY_PERIOD_ROUNDING_MINUTES"] = 0;

                if (this.cboPayPeriodRounding.SelectedIndex != 0)
                {
                    pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["PAY_PERIOD_ROUNDING_MINUTES"] = Convert.ToDouble(this.cboPayPeriodRoundingMinutes.SelectedItem);
                }

                if (this.rbnDayAlways.Checked == true)
                {
                    pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["OVERTIME_IND"] = "A";
                }
                else
                {
                    pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["OVERTIME_IND"] = "W";
                }

                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SATURDAY_PAY_RATE"] = 0;

                if (this.cboRateSat.SelectedIndex > 0)
                {
                    pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SATURDAY_PAY_RATE"] = Convert.ToDouble(this.cboRateSat.SelectedItem);
                }

                if (this.rbnSatAlways.Checked == true)
                {
                    pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SATURDAY_PAY_RATE_IND"] = "A";
                }
                else
                {
                    pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SATURDAY_PAY_RATE_IND"] = "E";
                }

                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SUNDAY_PAY_RATE"] = 0;

                if (this.cboRateSun.SelectedIndex > 0)
                {
                    pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SUNDAY_PAY_RATE"] = Convert.ToDouble(this.cboRateSun.SelectedItem);
                }

                if (this.rbnSunAlways.Checked == true)
                {
                    pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SUNDAY_PAY_RATE_IND"] = "A";
                }
                else
                {
                    pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SUNDAY_PAY_RATE_IND"] = "E";
                }
              
                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["PAIDHOLIDAY_RATE"] = Convert.ToDouble(this.txtPaidHolidayRate.Text);

                if (this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) != "S")
                {
                    if (this.chkPayPublicHoliday.Checked == true)
                    {
                        pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["PAY_PUBLIC_HOLIDAY_IND"] = "Y";
                    }
                    else
                    {
                        pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["PAY_PUBLIC_HOLIDAY_IND"] = "N";
                    }
                }
                else
                {
                    pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["PAY_PUBLIC_HOLIDAY_IND"] = "N";
                }
                                
                if (this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) == "S")
                {
                    pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SALARY_MINUTES_PAID_PER_DAY"] = (Convert.ToInt32(this.cboSalaryDayPaidHours.SelectedItem) * 60) + Convert.ToInt32(this.cboSalaryDayPaidMinutes.SelectedItem);
                    pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SALARY_DAYS_PER_YEAR"] = Convert.ToInt32(this.cboSalaryDaysInYear.SelectedItem);
                }

                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["EXCEPTION_SHIFT_ABOVE_PERCENT"] = intShiftAbove;
                pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["EXCEPTION_SHIFT_BELOW_PERCENT"] = intShiftBelow;

                if (this.rbnUploadYes.Checked == true)
                {
                    pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["NO_EDIT_IND"] = "Y";
                }
                else
                {
                    pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["NO_EDIT_IND"] = "N";
                }

                //Tables to Be Moved to Business Layer
                pvtTempDataSet = null;
                pvtTempDataSet = new DataSet();

                DataTable myDataTable = this.pvtDataSet.Tables["PayCategory"].Clone();
                pvtTempDataSet.Tables.Add(myDataTable);

                pvtTempDataSet.Tables["PayCategory"].ImportRow(pvtPayCategoryDataView[clsISUtilities.DataViewIndex].Row);

                pvtPayCategoryDataView[clsISUtilities.DataViewIndex].Row.Delete();
                
                myDataTable = this.pvtDataSet.Tables["PayCategoryBreak"].Clone();

                pvtTempDataSet.Tables.Add(myDataTable);

                string strFilter = "PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) + "'";

                pvtTempDataView = null;
                pvtTempDataView = new DataView(pvtDataSet.Tables["PayCategoryBreak"],
                      strFilter,
                      "",
                      DataViewRowState.Added | DataViewRowState.ModifiedCurrent | DataViewRowState.Deleted);

                for (int intRow = 0; intRow < pvtTempDataView.Count; intRow++)
                {
                    if (pvtTempDataView[intRow].Row.RowState == DataRowState.Added)
                    {
                        if (Convert.ToInt32(pvtTempDataView[intRow]["WORKED_TIME_MINUTES"]) == 0
                        & Convert.ToInt32(pvtTempDataView[intRow]["BREAK_MINUTES"]) == 0)
                        {
                        }
                        else
                        {
                            pvtTempDataSet.Tables["PayCategoryBreak"].ImportRow(pvtTempDataView[intRow].Row);
                        }

                        pvtTempDataView[intRow].Delete();
                        intRow -= 1;
                    }
                    else
                    {
                        pvtTempDataSet.Tables["PayCategoryBreak"].ImportRow(pvtTempDataView[intRow].Row);

                        pvtTempDataView[intRow].Delete();
                    }
                }

                myDataTable = this.pvtDataSet.Tables["UserAuthorise"].Clone();

                pvtTempDataSet.Tables.Add(myDataTable);

                pvtTempDataView = null;
                pvtTempDataView = new DataView(pvtDataSet.Tables["UserAuthorise"],
                      strFilter,
                      "",
                      DataViewRowState.Added | DataViewRowState.Deleted);

                for (int intRow = 0; intRow < pvtTempDataView.Count; intRow++)
                {
                    pvtTempDataSet.Tables["UserAuthorise"].ImportRow(pvtTempDataView[intRow].Row);
                    //pvtTempDataView[intRow].Row.Delete();
                }
              
                //Compress DataSet
                pvtbytCompress = clsISUtilities.Compress_DataSet(pvtTempDataSet);

                string pvtstrCloseCostCentreInd = "N";

                if (this.grbCloseCostCentre.Visible == true)
                {
                    if (this.chkClose.Checked == true)
                    {
                        pvtstrCloseCostCentreInd = "Y";
                    }
                }

                object[] objParm = new object[5];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[2] = pvtbytCompress;
                objParm[3] = this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1);
                objParm[4] = pvtstrCloseCostCentreInd;
                
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Update_Record", objParm,true);

                pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                pvtDataSet.Merge(pvtTempDataSet);

                if (this.pvtDataSet.Tables["Reply"].Rows[0]["PAYROLL_RUN_IND"].ToString() == "Y")
                {
                    this.pvtDataSet.RejectChanges();

                    this.clsISUtilities.pubintReloadSpreadsheet = false;

                    pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["PAYROLL_LINK"] = 0;

                    this.dgvPayCategoryDataGridView[0,this.Get_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView)].Style = LockedPayrollRunDataGridViewCellStyle;

                    DialogResult dlgResult = CustomMessageBox.Show("A Payroll Run is in Progress for this Cost Centre.\nUpdate Cancelled.",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else
                {
                    this.pvtDataSet.Tables.Remove("Reply");

                    if (this.Text.IndexOf("- New") > -1)
                    {
                        this.pvtintPayCategoryNo = Convert.ToInt32(pvtTempDataSet.Tables["PayCategory"].Rows[0]["PAY_CATEGORY_NO"]);
                    }
                }

                this.pvtDataSet.AcceptChanges();

                Load_PayCategory_SpreadSheet();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void cboDailyRounding_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.btnSave.Enabled == true
                & this.pvtDataSet.Tables["Company"].Rows[0]["ACCESS_IND"].ToString() != "R")
            {
                if (this.cboDailyRounding.SelectedIndex == 0)
                {
                    this.cboDailyRoundingMinutes.SelectedIndex = -1;
                    this.cboDailyRoundingMinutes.Enabled = false;

                }
                else
                {
                    this.cboDailyRoundingMinutes.Enabled = true;
                }
            }
        }

        public void btnUpdate_Click(object sender, System.EventArgs e)
        {
            this.Text += " - Update";

            Set_Form_For_Edit();

            this.txtPayCategory.Focus();
        }

        private void cboPayPeriodRounding_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.btnSave.Enabled == true
                & this.pvtDataSet.Tables["Company"].Rows[0]["ACCESS_IND"].ToString() != "S")
            {
                if (this.cboPayPeriodRounding.SelectedIndex == 0)
                {
                    this.cboPayPeriodRoundingMinutes.SelectedIndex = -1;
                    this.cboPayPeriodRoundingMinutes.Enabled = false;
                }
                else
                {
                    this.cboPayPeriodRoundingMinutes.Enabled = true;
                }
            }
        }

        public void btnDelete_Click(object sender, System.EventArgs e)
        {
            try
            {
                DialogResult dlgResult = CustomMessageBox.Show("Delete Cost Centre '" + pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["PAY_CATEGORY_DESC"].ToString() + "'",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dlgResult == DialogResult.Yes)
                {
                    object[] objParm = new object[4];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[1] =Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[2] = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["PAY_CATEGORY_NO"]);
                    objParm[3] = this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1);

                    int intReturnCode = (int)clsISUtilities.DynamicFunction("Delete_Record", objParm,true);

                    if (intReturnCode == 99)
                    {
                        CustomMessageBox.Show("There are Still Employees Linked to this Cost Centre.\nAction Cancelled.",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    }
                    else
                    {
                        CustomMessageBox.Show("Cost Centre '" + pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["PAY_CATEGORY_DESC"].ToString() + "' has been Deleted.",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                        pvtintPayCategoryNo = -1;

                        pvtPayCategoryDataView[clsISUtilities.DataViewIndex].Delete();

                        this.pvtDataSet.Tables["PayCategory"].AcceptChanges();

                        Load_PayCategory_SpreadSheet();
                    }
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }
       
        private void cboRateSat_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (cboRateSat.SelectedIndex == 0)
            {
                this.rbnSatAlways.Checked = false;
                this.rbnSatUseOption.Checked = false;

                this.rbnSatAlways.Enabled = false;
                this.rbnSatUseOption.Enabled = false;
            }
            else
            {
                if (this.btnSave.Enabled == true)
                {
                    this.rbnSatAlways.Enabled = true;
                    this.rbnSatUseOption.Enabled = true;

                    if (this.rbnSatAlways.Checked == false
                        & this.rbnSatUseOption.Checked == false)
                    {
                        this.rbnSatAlways.Checked = true;
                    }
                }
            }
        }

        private void cboRateSun_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (cboRateSun.SelectedIndex == 0)
            {
                this.rbnSunAlways.Checked = false;
                this.rbnSunUseOption.Checked = false;

                this.rbnSunAlways.Enabled = false;
                this.rbnSunUseOption.Enabled = false;
            }
            else
            {
                if (this.btnSave.Enabled == true)
                {
                    this.rbnSunAlways.Enabled = true;
                    this.rbnSunUseOption.Enabled = true;

                    if (this.rbnSunAlways.Checked == false
                        & this.rbnSunUseOption.Checked == false)
                    {
                        this.rbnSunAlways.Checked = true;
                    }
                }
            }
        }

        private void cboHoursExceeds_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (cboHoursExceeds.SelectedIndex == 0)
                {
                    this.cboMinutesExceeds.SelectedIndex = -1;
                    this.cboMinutesExceeds.Enabled = false;
                }
                else
                {
                    this.cboMinutesExceeds.Enabled = true;

                    if (this.cboMinutesExceeds.SelectedIndex == -1)
                    {
                        this.cboMinutesExceeds.SelectedIndex = 0;
                    }
                }
            }
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Authorise Tab
            if (tabControl.SelectedIndex == 2)
            {
                if (this.dgvAuthoriseTypeDataGridView.Rows.Count == 0)
                {
                    pvtblnAuthoriseTypeDataGridViewLoaded = false;

                    if (this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) != "T")
                    {
                        this.dgvAuthoriseTypeDataGridView.Rows.Add("Leave");
                    }

                    this.dgvAuthoriseTypeDataGridView.Rows.Add("Timesheet");
                    
                    pvtblnAuthoriseTypeDataGridViewLoaded = true;

                    this.Set_DataGridView_SelectedRowIndex(this.dgvAuthoriseTypeDataGridView, 0);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (this.dgvUserDataGridView.Rows.Count > 0)
            {
                DataRow drDataRow = this.pvtDataSet.Tables["UserAuthorise"].NewRow();

                drDataRow["PAY_CATEGORY_NO"] = pvtintPayCategoryNo;
                drDataRow["PAY_CATEGORY_TYPE"] = pvtstrPayrollType;
                drDataRow["AUTHORISE_TYPE_IND"] = pvtstrAuthoriseTypeInd;
                drDataRow["LEVEL_NO"] = pvtintLevelNo;
                drDataRow["USER_NO"] = this.dgvUserDataGridView[3, this.Get_DataGridView_SelectedRowIndex(this.dgvUserDataGridView)].Value.ToString();

                pvtDataSet.Tables["UserAuthorise"].Rows.Add(drDataRow);

                DataGridViewRow myDataGridViewRow = this.dgvUserDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvUserDataGridView)];

                this.dgvUserDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvUserSelectedDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvUserSelectedDataGridView.CurrentCell = this.dgvUserSelectedDataGridView[0, this.dgvUserSelectedDataGridView.Rows.Count - 1];
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (this.dgvUserSelectedDataGridView.Rows.Count > 0)
            {
                string strFilter = pvtUserAuthoriseDataView.RowFilter;

                strFilter += " AND USER_NO = " + this.dgvUserSelectedDataGridView[3, this.Get_DataGridView_SelectedRowIndex(this.dgvUserSelectedDataGridView)].Value.ToString();

                pvtTempDataView = null;
                pvtTempDataView = new DataView(pvtDataSet.Tables["UserAuthorise"],
                      strFilter,
                      "",
                      DataViewRowState.CurrentRows);

                pvtTempDataView.Delete(0);

                DataGridViewRow myDataGridViewRow = this.dgvUserSelectedDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvUserSelectedDataGridView)];

                this.dgvUserSelectedDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvUserDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvUserDataGridView.CurrentCell = this.dgvUserDataGridView[0, this.dgvUserDataGridView.Rows.Count - 1];
            }
        }

        private void Check_To_Add_New_Break_Row()
        {
            if (this.btnSave.Enabled == true)
            {
                int intPayCategoryBreakNo = 1;

                if (this.dgvBreakDataGridView.Rows.Count == 0)
                {
                }
                else
                {
                    if (Convert.ToInt32(this.dgvBreakDataGridView[pvtintBreakhhCol,this.dgvBreakDataGridView.Rows.Count - 1].Value) != 0
                        | Convert.ToInt32(this.dgvBreakDataGridView[pvtintBreakmmCol,this.dgvBreakDataGridView.Rows.Count - 1].Value) != 0)
                    {
                        if (Convert.ToInt32(this.dgvBreakDataGridView[pvtintTimeWorkedhhCol,this.dgvBreakDataGridView.Rows.Count - 1].Value) != 0
                        | Convert.ToInt32(this.dgvBreakDataGridView[pvtintTimeWorkedmmCol,this.dgvBreakDataGridView.Rows.Count - 1].Value) != 0)
                        {
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (this.dgvBreakDataGridView.Rows.Count == 2)
                        {
                            if (Convert.ToInt32(this.dgvBreakDataGridView[pvtintTimeWorkedhhCol, this.dgvBreakDataGridView.Rows.Count - 1].Value) == 0
                            & Convert.ToInt32(this.dgvBreakDataGridView[pvtintTimeWorkedmmCol, this.dgvBreakDataGridView.Rows.Count - 1].Value) == 0)
                            {
                                pvtblnBreakTableErrors = false;
                                clsISUtilities.Paint_Parent_Marker(this.lblBreakDesc, false);
                            }
                        }

                        return;
                    }

                    for (int intRow = 0; intRow < pvtBreakDataView.Count; intRow++)
                    {
                        if (Convert.ToInt32(pvtBreakDataView[intRow]["PAY_CATEGORY_BREAK_NO"]) >= intPayCategoryBreakNo)
                        {
                            intPayCategoryBreakNo = Convert.ToInt32(pvtBreakDataView[intRow]["PAY_CATEGORY_BREAK_NO"]) + 1;
                        }
                    }
                }

                clsISUtilities.Paint_Parent_Marker(this.lblBreakDesc, false);
            
                DataRowView drvDataRowView = this.pvtBreakDataView.AddNew();

                drvDataRowView.BeginEdit();

                //Set First Part of Key
                drvDataRowView["COMPANY_NO"] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                drvDataRowView["PAY_CATEGORY_TYPE"] = this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1);
                drvDataRowView["PAY_CATEGORY_NO"] = pvtintPayCategoryNo;

                drvDataRowView["PAY_CATEGORY_BREAK_NO"] = intPayCategoryBreakNo;

                drvDataRowView["WORKED_TIME_MINUTES"] = 0;
                drvDataRowView["BREAK_MINUTES"] = 0;

                drvDataRowView.EndEdit();

                this.pvtblnBreakDataGridViewLoaded = false;

                this.dgvBreakDataGridView.Rows.Add("00",
                                                   "00",
                                                   "00",
                                                   "00",
                                                    intPayCategoryBreakNo.ToString());

                this.pvtblnBreakDataGridViewLoaded = true;
            }
        }

        private void btnBreakDeleteRec_Click(object sender, EventArgs e)
        {
            if (this.dgvBreakDataGridView.Rows.Count > 0)
            {
                for (int intRow = 0; intRow < pvtBreakDataView.Count; intRow++)
                {
                    if (this.pvtBreakDataView[intRow]["PAY_CATEGORY_BREAK_NO"].ToString() == this.dgvBreakDataGridView[pvtintPayCategoryBreakNoCol,this.Get_DataGridView_SelectedRowIndex(this.dgvBreakDataGridView)].Value.ToString())
                    {
                        this.pvtBreakDataView[intRow].Delete();

                        DataGridViewRow myDataGridViewRow = this.dgvBreakDataGridView.Rows[this.dgvBreakDataGridView.CurrentRow.Index];

                        this.dgvBreakDataGridView.Rows.Remove(myDataGridViewRow);

                        Check_To_Add_New_Break_Row();

                        break;
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

                    pvtstrPayrollType = dgvPayrollTypeDataGridView[0, e.RowIndex].Value.ToString().Substring(0, 1);

                    if (pvtstrPayrollType == "W")
                    {
                        this.lblCostCentreLock.Text = "Some Cost Centre Records are Locked Due to Current Wage Run.";
                    }
                    else
                    {
                        if (pvtstrPayrollType == "S")
                        {
                            this.lblCostCentreLock.Text = "Some Cost Centre Records are Locked Due to Current Salary Run.";
                        }
                        else
                        {
                            this.lblCostCentreLock.Text = "Some Cost Centre Records are Locked Due to Current Time and Attendance Run.";

                        }
                    }

                    if (pvtDataSet != null)
                    {
                        Load_CurrentForm_Records();
                    }
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
               
                    clsISUtilities.DataViewIndex = Convert.ToInt32(this.dgvPayCategoryDataGridView[2, e.RowIndex].Value);
                    pvtintPayCategoryNo = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["PAY_CATEGORY_NO"]);

                    clsISUtilities.DataBind_DataView_Record_Show();

                    if (this.pvtDataSet.Tables["Company"].Rows[0]["ACCESS_IND"].ToString() == "R")
                    {
                        this.btnUpdate.Enabled = false;
                    }
                    else
                    {
                        if (pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["PAYROLL_LINK"] == System.DBNull.Value)
                        {
                            if (this.rbnActive.Checked == true)
                            {
                                this.btnNew.Enabled = true;
                                this.btnUpdate.Enabled = true;
                            }
                            else
                            {
                                this.btnNew.Enabled = false;
                                this.btnUpdate.Enabled = false;
                            }
                        }
                        else
                        {
                            this.btnUpdate.Enabled = false;
                        }
                    }

                    //this.txtPayCategory.Text = pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["PAY_CATEGORY_DESC"].ToString();

                    if (pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["PAY_CATEGORY_DEL_IND"].ToString() == "Y")
                    {
                        if (this.pvtDataSet.Tables["Company"].Rows[0]["ACCESS_IND"].ToString() == "R")
                        {
                            this.btnDelete.Enabled = false;
                        }
                        else
                        {
                            if (this.pvtDataSet.Tables["Company"].Rows[0]["ACCESS_IND"].ToString() == "A")
                            {
                                if (this.rbnActive.Checked == true)
                                {
                                    this.btnDelete.Enabled = true;
                                }
                                else
                                {
                                    this.btnDelete.Enabled = false;
                                }
                            }
                            else
                            {
                                this.btnDelete.Enabled = false;
                            }
                        }
                    }
                    else
                    {
                        this.btnDelete.Enabled = false;
                    }

                    pvtintHours = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["MON_TIME_MINUTES"]) / 60;
                    pvtintMinutes = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["MON_TIME_MINUTES"]) % 60;

                    this.cboMonTimeHours.SelectedIndex = pvtintHours;

                    for (int intRow = 0; intRow < 100; intRow++)
                    {
                        if (Convert.ToInt32(this.cboMonTimeMinutes.Items[intRow]) == pvtintMinutes)
                        {
                            this.cboMonTimeMinutes.SelectedIndex = intRow;

                            break;
                        }
                    }

                    pvtintHours = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["TUE_TIME_MINUTES"]) / 60;
                    pvtintMinutes = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["TUE_TIME_MINUTES"]) % 60;

                    this.cboTueTimeHours.SelectedIndex = pvtintHours;

                    for (int intRow = 0; intRow < 100; intRow++)
                    {
                        if (Convert.ToInt32(this.cboTueTimeMinutes.Items[intRow]) == pvtintMinutes)
                        {
                            this.cboTueTimeMinutes.SelectedIndex = intRow;

                            break;
                        }
                    }

                    pvtintHours = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["WED_TIME_MINUTES"]) / 60;
                    pvtintMinutes = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["WED_TIME_MINUTES"]) % 60;

                    this.cboWedTimeHours.SelectedIndex = pvtintHours;

                    for (int intRow = 0; intRow < 100; intRow++)
                    {
                        if (Convert.ToInt32(this.cboWedTimeMinutes.Items[intRow]) == pvtintMinutes)
                        {
                            this.cboWedTimeMinutes.SelectedIndex = intRow;

                            break;
                        }
                    }

                    pvtintHours = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["THU_TIME_MINUTES"]) / 60;
                    pvtintMinutes = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["THU_TIME_MINUTES"]) % 60;

                    this.cboThuTimeHours.SelectedIndex = pvtintHours;

                    for (int intRow = 0; intRow < 100; intRow++)
                    {
                        if (Convert.ToInt32(this.cboThuTimeMinutes.Items[intRow]) == pvtintMinutes)
                        {
                            this.cboThuTimeMinutes.SelectedIndex = intRow;

                            break;
                        }
                    }

                    pvtintHours = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["FRI_TIME_MINUTES"]) / 60;
                    pvtintMinutes = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["FRI_TIME_MINUTES"]) % 60;

                    this.cboFriTimeHours.SelectedIndex = pvtintHours;

                    for (int intRow = 0; intRow < 100; intRow++)
                    {
                        if (Convert.ToInt32(this.cboFriTimeMinutes.Items[intRow]) == pvtintMinutes)
                        {
                            this.cboFriTimeMinutes.SelectedIndex = intRow;

                            break;
                        }
                    }

                    pvtintHours = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SAT_TIME_MINUTES"]) / 60;
                    pvtintMinutes = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SAT_TIME_MINUTES"]) % 60;

                    this.cboSatTimeHours.SelectedIndex = pvtintHours;

                    for (int intRow = 0; intRow < 100; intRow++)
                    {
                        if (Convert.ToInt32(this.cboSatTimeMinutes.Items[intRow]) == pvtintMinutes)
                        {
                            this.cboSatTimeMinutes.SelectedIndex = intRow;

                            break;
                        }
                    }

                    pvtintHours = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SUN_TIME_MINUTES"]) / 60;
                    pvtintMinutes = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SUN_TIME_MINUTES"]) % 60;

                    this.cboSunTimeHours.SelectedIndex = pvtintHours;

                    for (int intRow = 0; intRow < 100; intRow++)
                    {
                        if (Convert.ToInt32(this.cboSunTimeMinutes.Items[intRow]) == pvtintMinutes)
                        {
                            this.cboSunTimeMinutes.SelectedIndex = intRow;

                            break;
                        }
                    }

                    //OverTime1
                    pvtintHours = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["OVERTIME1_MINUTES"]) / 60;

                    if (pvtintHours == 999)
                    {
                        this.cboOverTime1Hours.SelectedIndex = this.cboOverTime1Hours.Items.Count - 1;
                        this.cboOverTime1Minutes.SelectedIndex = 0;
                    }
                    else
                    {
                        pvtintMinutes = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["OVERTIME1_MINUTES"]) % 60;

                        this.cboOverTime1Hours.SelectedIndex = pvtintHours;

                        for (int intRow = 0; intRow < 100; intRow++)
                        {
                            if (Convert.ToInt32(this.cboOverTime1Minutes.Items[intRow]) == pvtintMinutes)
                            {
                                this.cboOverTime1Minutes.SelectedIndex = intRow;

                                break;
                            }
                        }
                    }

                    //OverTime2
                    pvtintHours = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["OVERTIME2_MINUTES"]) / 60;

                    if (pvtintHours == 999)
                    {
                        this.cboOverTime2Hours.SelectedIndex = this.cboOverTime2Hours.Items.Count - 1;
                        this.cboOverTime2Minutes.SelectedIndex = 0;
                    }
                    else
                    {
                        pvtintMinutes = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["OVERTIME2_MINUTES"]) % 60;

                        this.cboOverTime2Hours.SelectedIndex = pvtintHours;

                        for (int intRow = 0; intRow < 100; intRow++)
                        {
                            if (Convert.ToInt32(this.cboOverTime2Minutes.Items[intRow]) == pvtintMinutes)
                            {
                                this.cboOverTime2Minutes.SelectedIndex = intRow;

                                break;
                            }
                        }
                    }

                    //OverTime3
                    pvtintHours = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["OVERTIME3_MINUTES"]) / 60;

                    if (pvtintHours == 999)
                    {
                        this.cboOverTime3Hours.SelectedIndex = this.cboOverTime3Hours.Items.Count - 1;
                        this.cboOverTime3Minutes.SelectedIndex = 0;
                    }
                    else
                    {
                        pvtintMinutes = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["OVERTIME3_MINUTES"]) % 60;

                        this.cboOverTime3Hours.SelectedIndex = pvtintHours;

                        for (int intRow = 0; intRow < 100; intRow++)
                        {
                            if (Convert.ToInt32(this.cboOverTime3Minutes.Items[intRow]) == pvtintMinutes)
                            {
                                this.cboOverTime3Minutes.SelectedIndex = intRow;

                                break;
                            }
                        }
                    }

                    this.rbnHours1.Checked = false;
                    this.rbnHours2.Checked = false;
                    this.rbnHours3.Checked = false;

                    if (Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["OVERTIME1_MINUTES"]) / 60 == 999)
                    {
                        this.rbnHours1.Checked = true;
                    }
                    else
                    {
                        if (Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["OVERTIME2_MINUTES"]) / 60 == 999)
                        {
                            this.rbnHours2.Checked = true;
                        }
                        else
                        {
                            if (Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["OVERTIME3_MINUTES"]) / 60 == 999)
                            {
                                this.rbnHours3.Checked = true;
                            }
                        }
                    }

                    this.cboDailyRounding.SelectedIndex = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["DAILY_ROUNDING_IND"]);

                    this.cboDailyRoundingMinutes.SelectedIndex = -1;

                    for (int intIndex = 0; intIndex < this.cboDailyRoundingMinutes.Items.Count; intIndex++)
                    {
                        if (this.cboDailyRoundingMinutes.Items[intIndex].ToString() == Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["DAILY_ROUNDING_MINUTES"]).ToString("00"))
                        {
                            this.cboDailyRoundingMinutes.SelectedIndex = intIndex;
                            break;
                        }
                    }

                    this.cboPayPeriodRounding.SelectedIndex = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["PAY_PERIOD_ROUNDING_IND"]);

                    this.cboPayPeriodRoundingMinutes.SelectedIndex = -1;

                    for (int intIndex = 0; intIndex < this.cboPayPeriodRoundingMinutes.Items.Count; intIndex++)
                    {
                        if (this.cboPayPeriodRoundingMinutes.Items[intIndex].ToString() == Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["PAY_PERIOD_ROUNDING_MINUTES"]).ToString("00"))
                        {
                            this.cboPayPeriodRoundingMinutes.SelectedIndex = intIndex;
                            break;
                        }

                    }

                    this.cboRateSat.SelectedIndex = -1;

                    if (pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SATURDAY_PAY_RATE"].ToString() == "0")
                    {
                        this.cboRateSat.SelectedIndex = 0;
                        this.rbnSatAlways.Checked = false;
                        this.rbnSatUseOption.Checked = false;
                    }
                    else
                    {
                        for (int intIndex = 1; intIndex < this.cboRateSat.Items.Count; intIndex++)
                        {
                            if (Convert.ToDouble(this.cboRateSat.Items[intIndex].ToString()) == Convert.ToDouble(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SATURDAY_PAY_RATE"].ToString()))
                            {
                                this.cboRateSat.SelectedIndex = intIndex;
                                break;
                            }
                        }

                        if (pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SATURDAY_PAY_RATE_IND"].ToString() == "A")
                        {
                            this.rbnSatAlways.Checked = true;
                        }
                        else
                        {
                            this.rbnSatUseOption.Checked = true;
                        }
                    }

                    if (pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["OVERTIME_IND"].ToString() == "A")
                    {
                        this.rbnDayAlways.Checked = true;
                    }
                    else
                    {
                        this.rbnPeriodExceeded.Checked = true;
                    }

                    if (Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["TOTAL_DAILY_TIME_OVERTIME"]) == 0)
                    {
                        this.cboHoursExceeds.SelectedIndex = 0;
                    }
                    else
                    {
                        pvtintHours = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["TOTAL_DAILY_TIME_OVERTIME"]) / 60;
                        pvtintMinutes = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["TOTAL_DAILY_TIME_OVERTIME"]) % 60;

                        this.cboHoursExceeds.SelectedIndex = pvtintHours + 1;

                        for (int intRow = 0; intRow < 100; intRow++)
                        {
                            if (Convert.ToInt32(this.cboMinutesExceeds.Items[intRow]) == pvtintMinutes)
                            {
                                this.cboMinutesExceeds.SelectedIndex = intRow;

                                break;
                            }
                        }
                    }

                    this.cboRateSun.SelectedIndex = -1;

                    if (pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SUNDAY_PAY_RATE"].ToString() == "0")
                    {
                        this.cboRateSun.SelectedIndex = 0;
                        this.rbnSunAlways.Checked = false;
                        this.rbnSunUseOption.Checked = false;
                    }
                    else
                    {
                        for (int intIndex = 1; intIndex < this.cboRateSun.Items.Count; intIndex++)
                        {
                            if (Convert.ToDouble(this.cboRateSun.Items[intIndex].ToString()) == Convert.ToDouble(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SUNDAY_PAY_RATE"]))
                            {
                                this.cboRateSun.SelectedIndex = intIndex;
                                break;
                            }
                        }

                        if (pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SUNDAY_PAY_RATE_IND"].ToString() == "A")
                        {
                            this.rbnSunAlways.Checked = true;
                        }
                        else
                        {
                            this.rbnSunUseOption.Checked = true;
                        }
                    }

                    if (this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) == "S")
                    {
                        pvtintHours = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SALARY_MINUTES_PAID_PER_DAY"]) / 60;
                        pvtintMinutes = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SALARY_MINUTES_PAID_PER_DAY"]) % 60;

                        this.cboSalaryDayPaidHours.SelectedIndex = pvtintHours;

                        for (int intRow = 0; intRow < this.cboSalaryDayPaidMinutes.Items.Count; intRow++)
                        {
                            if (Convert.ToInt32(this.cboSalaryDayPaidMinutes.Items[intRow]) == pvtintMinutes)
                            {
                                this.cboSalaryDayPaidMinutes.SelectedIndex = intRow;

                                break;
                            }
                        }

                        for (int intRow = 0; intRow < this.cboSalaryDaysInYear.Items.Count; intRow++)
                        {
                            if (Convert.ToInt32(this.cboSalaryDaysInYear.Items[intRow]) == Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["SALARY_DAYS_PER_YEAR"]))
                            {
                                this.cboSalaryDaysInYear.SelectedIndex = intRow;

                                break;
                            }
                        }
                    }

                    this.cboShiftAbove.SelectedIndex = -1;

                    if (pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["EXCEPTION_SHIFT_ABOVE_PERCENT"].ToString() == "0")
                    {
                        this.cboShiftAbove.SelectedIndex = 0;
                    }
                    else
                    {
                        for (int intIndex = 1; intIndex < this.cboShiftAbove.Items.Count; intIndex++)
                        {
                            if (Convert.ToDouble(this.cboShiftAbove.Items[intIndex].ToString()) == Convert.ToDouble(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["EXCEPTION_SHIFT_ABOVE_PERCENT"]))
                            {
                                this.cboShiftAbove.SelectedIndex = intIndex;
                                break;
                            }
                        }
                    }

                    this.cboShiftBelow.SelectedIndex = -1;

                    if (pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["EXCEPTION_SHIFT_BELOW_PERCENT"].ToString() == "0")
                    {
                        this.cboShiftBelow.SelectedIndex = 0;
                    }
                    else
                    {
                        for (int intIndex = 1; intIndex < this.cboShiftBelow.Items.Count; intIndex++)
                        {
                            if (Convert.ToDouble(this.cboShiftBelow.Items[intIndex].ToString()) == Convert.ToDouble(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["EXCEPTION_SHIFT_BELOW_PERCENT"]))
                            {
                                this.cboShiftBelow.SelectedIndex = intIndex;
                                break;
                            }
                        }
                    }

                    this.txtPaidHolidayRate.Text = Convert.ToDouble(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["PAIDHOLIDAY_RATE"]).ToString("0.00");

                    if (this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) != "S")
                    {
                        if (pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["PAY_PUBLIC_HOLIDAY_IND"].ToString() == "Y")
                        {
                            this.chkPayPublicHoliday.Checked = true;
                        }
                        else
                        {
                            this.chkPayPublicHoliday.Checked = false;
                        }
                    }

                    if (pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["NO_EDIT_IND"].ToString() == "Y")
                    {
                        this.rbnUploadYes.Checked = true;
                    }
                    else
                    {
                        this.rbnUploadNo.Checked = true;
                    }

                    pvtBreakDataView = null;
                    pvtBreakDataView = new DataView(this.pvtDataSet.Tables["PayCategoryBreak"],
                        "PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) + "'",
                        "WORKED_TIME_MINUTES,BREAK_MINUTES"
                        , DataViewRowState.CurrentRows);

                    this.Clear_DataGridView(this.dgvBreakDataGridView);

                    this.pvtblnBreakDataGridViewLoaded = false;

                    for (int intRow = 0; intRow < pvtBreakDataView.Count; intRow++)
                    {
                        this.dgvBreakDataGridView.Rows.Add(Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_MINUTES"]) / 60).ToString("00"),
                                                           Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_MINUTES"]) % 60).ToString("00"),
                                                           Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["WORKED_TIME_MINUTES"]) / 60).ToString("00"),
                                                           Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["WORKED_TIME_MINUTES"]) % 60).ToString("00"),
                                                           pvtBreakDataView[intRow]["PAY_CATEGORY_BREAK_NO"].ToString());
                    }

                    this.pvtblnBreakDataGridViewLoaded = true;

                    if (this.tabControl.SelectedIndex == 1)
                    {
                        if (this.dgvAuthoriseTypeDataGridView.Rows.Count > 0)
                        {
                            this.Set_DataGridView_SelectedRowIndex(this.dgvAuthoriseTypeDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvAuthoriseTypeDataGridView));
                        }
                    }
                }
            }
        }

        private void dgvAuthoriseTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnAuthoriseTypeDataGridViewLoaded == true)
            {
                if (pvtintAuthoriseTypeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintAuthoriseTypeDataGridViewRowIndex = e.RowIndex;

                    this.Set_DataGridView_SelectedRowIndex(dgvAuthorisationLevelDataGridView, 0);
                }
            }
        }

        private void dgvUserDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvUserDataGridView.Rows.Count > 0
                & pvtblnUserDataGridViewLoaded == true)
            {
            }
        }

        private void dgvUserSelectedDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvUserSelectedDataGridView.Rows.Count > 0
                & pvtblnUserSelectedDataGridViewLoaded == true)
            {
            }
        }

        private void dgvUserDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                btnAdd_Click(sender, e);
            }
        }

        private void dgvUserSelectedDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                btnRemove_Click(sender, e);
            }
        }

        private void dgvBreakDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvBreakDataGridView.Rows.Count > 0
            & pvtblnBreakDataGridViewLoaded == true)
            {

            }
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                ComboBox myComboBox = (ComboBox)sender;

                pvtblnBreakTableErrors = false;

                for (int intRow = 0; intRow < pvtBreakDataView.Count; intRow++)
                {
                    if (pvtBreakDataView[intRow]["PAY_CATEGORY_BREAK_NO"].ToString() == this.dgvBreakDataGridView[pvtintPayCategoryBreakNoCol, this.Get_DataGridView_SelectedRowIndex(this.dgvBreakDataGridView)].Value.ToString())
                    {
                        if (this.dgvBreakDataGridView.CurrentCell.ColumnIndex == pvtintBreakhhCol
                            | this.dgvBreakDataGridView.CurrentCell.ColumnIndex == pvtintBreakmmCol)
                        {
                            if (this.dgvBreakDataGridView.CurrentCell.ColumnIndex == pvtintBreakhhCol)
                            {
                                dgvBreakDataGridView[pvtintBreakhhCol, this.Get_DataGridView_SelectedRowIndex(this.dgvBreakDataGridView)].Value = myComboBox.SelectedIndex;
                            }
                            else
                            {
                                dgvBreakDataGridView[pvtintBreakmmCol, this.Get_DataGridView_SelectedRowIndex(this.dgvBreakDataGridView)].Value = myComboBox.SelectedIndex;
                            }

                            int intHourMinutes = Convert.ToInt32(dgvBreakDataGridView[pvtintBreakhhCol, this.Get_DataGridView_SelectedRowIndex(this.dgvBreakDataGridView)].Value) * 60;
                            intHourMinutes += Convert.ToInt32(dgvBreakDataGridView[pvtintBreakmmCol, this.Get_DataGridView_SelectedRowIndex(this.dgvBreakDataGridView)].Value);

                            pvtBreakDataView[intRow]["BREAK_MINUTES"] = intHourMinutes;
                        }
                        else
                        {
                            if (this.dgvBreakDataGridView.CurrentCell.ColumnIndex == pvtintTimeWorkedhhCol
                            | this.dgvBreakDataGridView.CurrentCell.ColumnIndex == pvtintTimeWorkedmmCol)
                            {
                                if (this.dgvBreakDataGridView.CurrentCell.ColumnIndex == pvtintTimeWorkedhhCol)
                                {
                                    dgvBreakDataGridView[pvtintTimeWorkedhhCol, this.Get_DataGridView_SelectedRowIndex(this.dgvBreakDataGridView)].Value = myComboBox.SelectedIndex;
                                }
                                else
                                {
                                    dgvBreakDataGridView[pvtintTimeWorkedmmCol, this.Get_DataGridView_SelectedRowIndex(this.dgvBreakDataGridView)].Value = myComboBox.SelectedIndex;
                                }

                                int intHourMinutes = Convert.ToInt32(dgvBreakDataGridView[pvtintTimeWorkedhhCol, this.Get_DataGridView_SelectedRowIndex(this.dgvBreakDataGridView)].Value) * 60;
                                intHourMinutes += Convert.ToInt32(dgvBreakDataGridView[pvtintTimeWorkedmmCol, this.Get_DataGridView_SelectedRowIndex(this.dgvBreakDataGridView)].Value);

                                pvtBreakDataView[intRow]["WORKED_TIME_MINUTES"] = intHourMinutes;
                            }
                        }
                    }

                    if (Convert.ToInt32(pvtBreakDataView[intRow]["WORKED_TIME_MINUTES"]) == 0
                        | Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_MINUTES"]) == 0)
                    {
                        if (Convert.ToInt32(pvtBreakDataView[intRow]["WORKED_TIME_MINUTES"]) == 0
                        & Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_MINUTES"]) == 0)
                        {
                        }
                        else
                        {
                            pvtblnBreakTableErrors = true;
                        }
                    }
                }

                if (pvtblnBreakTableErrors == true)
                {
                    clsISUtilities.Paint_Parent_Marker(this.lblBreakDesc, true);
                }
                else
                {
                    clsISUtilities.Paint_Parent_Marker(this.lblBreakDesc, false);
                }

                Check_To_Add_New_Break_Row();
            }
        }

        private void CostCentreFilter_Click(object sender, EventArgs e)
        {
            if (this.rbnActive.Checked == true)
            {
                this.grbCloseCostCentre.Visible = true;
            }
            else
            {
                this.grbCloseCostCentre.Visible = false;
            }

            Load_PayCategory_SpreadSheet();
        }

        private void dgvBreakDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is ComboBox)
            {
                ComboBox myComboBox = (ComboBox)e.Control;

                if (myComboBox != null)
                {
                    myComboBox.SelectedIndexChanged -= new EventHandler(ComboBox_SelectedIndexChanged);

                    myComboBox.SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);
                }
            }
        }

        private void dgvAuthorisationLevelDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvAuthorisationLevelDataGridView.Rows.Count > 0
            & pvtblnAuthoriseLevelDataGridViewLoaded == true)
            {
                this.Clear_DataGridView(this.dgvUserDataGridView);
                this.Clear_DataGridView(this.dgvUserSelectedDataGridView);

                string strDesc = dgvAuthorisationLevelDataGridView[0, e.RowIndex].Value.ToString();

                if (e.RowIndex == 0)
                {
                    pvtintLevelNo = 1;
                }
                else
                {
                    if (e.RowIndex == 1)
                    {
                        pvtintLevelNo = 2;
                    }
                    else
                    {
                        if (e.RowIndex == 2)
                        {
                            pvtintLevelNo = 3;
                        }
                    }
                }

                pvtUserAuthoriseDataView = null;

                string strFilter = "PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'";

                //ELR 2014-05-03
                if (pvtintAuthoriseTypeDataGridViewRowIndex == 0
                    && pvtstrPayrollType != "T")
                {
                    pvtstrAuthoriseTypeInd = "L";

                    //Leave
                    this.lblUsersSpreadsheetHeaders.Text = "List of Users - Leave (" + strDesc + ")";
                    this.lblSelectedUsersSpreadsheetHeaderes.Text = "List of Selected Users - Leave (" + strDesc + ")";

                    strFilter += " AND AUTHORISE_TYPE_IND = 'L'";
                }
                else
                {
                    pvtstrAuthoriseTypeInd = "T";

                    //Timesheet
                    this.lblUsersSpreadsheetHeaders.Text = "List of Users - Timesheet (" + strDesc + ")";
                    this.lblSelectedUsersSpreadsheetHeaderes.Text = "List of Selected Users - Timesheet (" + strDesc + ")";

                    strFilter += " AND AUTHORISE_TYPE_IND = 'T'";
                }

                strFilter += " AND LEVEL_NO = " + pvtintLevelNo;

                pvtUserAuthoriseDataView = null;
                pvtUserAuthoriseDataView = new DataView(pvtDataSet.Tables["UserAuthorise"],
                    strFilter,
                    "USER_NO",
                    DataViewRowState.CurrentRows);

                this.pvtblnUserDataGridViewLoaded = false;
                this.pvtblnUserSelectedDataGridViewLoaded = false;

                int intFindRow = -1;

                for (int intRow = 0; intRow < this.pvtDataSet.Tables["User"].Rows.Count; intRow++)
                {
                    intFindRow = pvtUserAuthoriseDataView.Find(this.pvtDataSet.Tables["User"].Rows[intRow]["USER_NO"].ToString());

                    if (intFindRow == -1)
                    {
                        this.dgvUserDataGridView.Rows.Add(this.pvtDataSet.Tables["User"].Rows[intRow]["USER_ID"].ToString(),
                                                          this.pvtDataSet.Tables["User"].Rows[intRow]["SURNAME"].ToString(),
                                                          this.pvtDataSet.Tables["User"].Rows[intRow]["FIRSTNAME"].ToString(),
                                                          this.pvtDataSet.Tables["User"].Rows[intRow]["USER_NO"].ToString());
                    }
                    else
                    {
                        this.dgvUserSelectedDataGridView.Rows.Add(this.pvtDataSet.Tables["User"].Rows[intRow]["USER_ID"].ToString(),
                                                                  this.pvtDataSet.Tables["User"].Rows[intRow]["SURNAME"].ToString(),
                                                                  this.pvtDataSet.Tables["User"].Rows[intRow]["FIRSTNAME"].ToString(),
                                                                  this.pvtDataSet.Tables["User"].Rows[intRow]["USER_NO"].ToString());
                    }
                }

                this.pvtblnUserDataGridViewLoaded = true;
                this.pvtblnUserSelectedDataGridViewLoaded = true;

            }
        }
    }
}
