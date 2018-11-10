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
    public partial class frmRptLeaveSelection : Form
    {
        clsISUtilities clsISUtilities;

        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
        private int pvtintPayCategoryDataGridViewRowIndex = -1;
       
        DataSet pvtDataSet;
        DataSet pvtTempDataSet;
        DataView pvtLeaveTypeDataView;
        DataView pvtEmployeeDataView;
        DataView pvtMonthDataView;
        DataView pvtDateDataView;
        DataView pvtPayCategoryDataView;
        DataView pvtEmployeePayCategoryDataView;

        DateTime dtFromDateTime = DateTime.Now;
        DateTime dtToDateTime = DateTime.Now;

        byte[] pvtbytCompress;

        private string pvtstrPayrollType = "";

        private int pvtintTabControlCurrentSelectedIndex = 0;

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnLeaveTypeDataGridViewLoaded = false;
        private bool pvtblnPayCategoryDataGridViewLoaded = false;

        private int pvtintlblEmployeeTop = 0;
        private int pvtintdgvEmployeeDataGridViewTop = 0;
        private int pvtintdgvEmployeeDataGridViewHeight = 0;

        private int pvtintbtnEmployeeAddTop = 0;
        private int pvtintbtnEmployeeAddAllTop = 0;
        private int pvtintbtnEmployeeRemoveTop = 0;
        private int pvtintbtnEmployeeRemoveAllTop = 0;

        DateTime pvtdtWageDate = DateTime.Now.AddYears(-50);

        public frmRptLeaveSelection()
        {
            InitializeComponent();

            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 114;

                this.tabControlMain.Height += 114;
                this.tabControl.Height += 114;
                this.tabPage1.Height += 114;
                this.tabPage2.Height += 114;

                this.grbEmployee.Top += 114;
                this.grbCostCentre.Top += 114;
                this.grbLeaveType.Top += 114;
                this.grbPeriod.Top += 114;

                this.dgvPayCategoryDataGridView.Height += 38;
                this.dgvChosenEmployeeDataGridView.Height += 114;

                this.lblEmployee.Top += 38;
                this.dgvEmployeeDataGridView.Top += 38;
                this.dgvEmployeeDataGridView.Height += 76;

                this.btnAdd.Top += 86;
                this.btnAddAll.Top += 86;
                this.btnRemove.Top += 86;
                this.btnRemoveAll.Top += 86;

                this.dgvCostCentreDataGridView.Height += 114;
                this.dgvChosenCostCentreDataGridView.Height += 114;

                this.dgvLeaveTypeDataGridView.Height += 114;
                this.dgvChosenLeaveTypeDataGridView.Height += 114;
                
                this.reportViewer.Height += 114;
            }

            pvtintlblEmployeeTop = this.lblEmployee.Top;
            pvtintdgvEmployeeDataGridViewTop = this.dgvEmployeeDataGridView.Top;
            pvtintdgvEmployeeDataGridViewHeight = this.dgvEmployeeDataGridView.Height;

            pvtintbtnEmployeeAddTop = this.btnAdd.Top;
            pvtintbtnEmployeeAddAllTop = this.btnAddAll.Top;
            pvtintbtnEmployeeRemoveTop = this.btnRemove.Top;
            pvtintbtnEmployeeRemoveAllTop = this.btnRemoveAll.Top;
        }

        private void frmRptLeaveSelection_Load(object sender, EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this,"busRptLeave");

                clsISUtilities.Create_Calender_Control_From_TextBox(this.txtFromDate);
                clsISUtilities.Create_Calender_Control_From_TextBox(this.txtToDate);

                clsISUtilities.NotDataBound_ComboBox(this.cboDateRule, "Select Option (Combo Box).");

                clsISUtilities.NotDataBound_Date_TextBox(this.txtFromDate, "Enter From Date.");
                clsISUtilities.NotDataBound_Date_TextBox(this.txtToDate, "Enter To Date.");

                clsISUtilities.Set_Form_For_Edit(false);

                clsISUtilities.Calender_Control_From_TextBox_Enable(this.txtFromDate);
                clsISUtilities.Calender_Control_From_TextBox_Enable(this.txtToDate);

                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSelectedEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblLeaveType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSelectedLeaveType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblMonth.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayCategory.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSelectedCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.dgvPayrollTypeDataGridView.Rows.Add("Wages");
                this.dgvPayrollTypeDataGridView.Rows.Add("Salaries");

                this.pvtblnPayrollTypeDataGridViewLoaded = true;

                object[] objParm = new object[4];
                objParm[0] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("MenuId").ToString();
                objParm[2] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[3] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
            
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);
                
                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                this.reportViewer.LocalReport.DataSources.Clear();
                
                this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView, 0);
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

                    case "dgvLeaveTypeDataGridView":

                        this.dgvLeaveTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvDateDataGridView":

                        this.dgvDateDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEmployeeDataGridView":

                        this.dgvEmployeeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvChosenEmployeeDataGridView":

                        this.dgvChosenEmployeeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvChosenLeaveTypeDataGridView":

                        this.dgvChosenLeaveTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        private void Load_CurrentForm_Records()
        {
            //All Employees
            this.rbnAllLeaveTypes.Checked = true;
            this.rbnAllEmployees.Checked = true;
            this.rbnAllCostCentre.Checked = true;

            EventArgs e = new EventArgs();

            this.rbnAllEmployees_Click(null, e);
            this.rbnAllCostCentre_Click(null, e);
            this.rbnAllLeaveTypes_Click(null, e);

            if (this.rbnPayPeriod.Checked == true)
            {
                DateOption_CheckedChanged(null, e);
            }
            else
            {
                this.rbnPayPeriod.Checked = true;
            }

            this.tabControl.SelectedIndex = 0;
        }

        private void rbnSelectedEmployees_Click(object sender, EventArgs e)
        {
            this.grbEmployeeSelection.Enabled = true;

            pvtblnPayCategoryDataGridViewLoaded = false;

            this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
            this.Clear_DataGridView(this.dgvEmployeeDataGridView);
            this.Clear_DataGridView(this.dgvChosenEmployeeDataGridView);
            
            pvtPayCategoryDataView = null;
            pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"],
            "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
            "PAY_CATEGORY_DESC",
            DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < pvtPayCategoryDataView.Count; intRow++)
            {
                this.dgvPayCategoryDataGridView.Rows.Add(pvtPayCategoryDataView[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                         pvtPayCategoryDataView[intRow]["PAY_CATEGORY_NO"].ToString());
            }

            pvtblnPayCategoryDataGridViewLoaded = true;

            if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, 0);
            }
            
            this.btnAdd.Enabled = true;
            this.btnAddAll.Enabled = true;
            this.btnRemove.Enabled = true;
            this.btnRemoveAll.Enabled = true;

            this.tabControl.SelectedIndex = 0;
        }

        private void DateOption_CheckedChanged(object sender, EventArgs e)
        {
            grbPrevYearBal.Visible = false;

            if (this.rbnYTD.Checked == true
                | this.rbnPrevYear.Checked == true
                | this.rbnDateAll.Checked == true)
            {
                this.lblMonth.Visible = false;
                this.dgvDateDataGridView.Visible = false;

                grbOtherDates.Visible = false;

                if (this.rbnYTD.Checked == true)
                {
                    this.rbnPrevYearYes.Checked = true;
                    grbPrevYearBal.Visible = true;
                }
            }
            else
            {
                if (this.rbnDateOther.Checked == true)
                {
                    grbOtherDates.Text = "Other Dates";

                    this.lblMonth.Visible = false;
                    this.dgvDateDataGridView.Visible = false;

                    this.txtFromDate.Text = "";
                    this.txtToDate.Text = "";

                    grbOtherDates.Visible = true;
                }
                else
                {
                    if (this.rbnPayPeriod.Checked == true
                        | this.rbnMonth.Checked == true)
                    {
                        this.Clear_DataGridView(this.dgvDateDataGridView);

                        grbOtherDates.Visible = false;

                        if (this.rbnPayPeriod.Checked == true)
                        {
                            this.rbnEmployeeAll.Checked = true;

                            this.lblMonth.Text = "Pay Period Date";

                            string strFilter = "COMPANY_NO = " + Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo"));
                            strFilter += " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'";

                            pvtDateDataView = null;
                            pvtDateDataView = new DataView(this.pvtDataSet.Tables["Dates"],
                                strFilter,
                                "PAY_PERIOD_DATE DESC",
                                DataViewRowState.CurrentRows);

                            //First Build Pay Category ListBox
                            for (int intRow = 0; intRow < pvtDateDataView.Count; intRow++)
                            {
                                this.dgvDateDataGridView.Rows.Add(Convert.ToDateTime(pvtDateDataView[intRow]["PAY_PERIOD_DATE"]).ToString("dd MMMM yyyy"),
                                                                  Convert.ToDateTime(pvtDateDataView[intRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd"));
                            }
                        }
                        else
                        {
                            this.lblMonth.Text = "Month";

                            pvtMonthDataView = new DataView(pvtDataSet.Tables["Month"],
                           "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                           "",
                           DataViewRowState.CurrentRows);

                            if (pvtMonthDataView.Count > 0)
                            {
                                DateTime dtMonthBegin = new DateTime(Convert.ToDateTime(pvtMonthDataView[0]["PAY_PERIOD_DATE"]).Year, Convert.ToDateTime(pvtMonthDataView[0]["PAY_PERIOD_DATE"]).Month, 1);
                                DateTime dtMonthEnd = new DateTime(Convert.ToDateTime(pvtMonthDataView[pvtMonthDataView.Count - 1]["PAY_PERIOD_DATE"]).Year, Convert.ToDateTime(pvtMonthDataView[pvtMonthDataView.Count - 1]["PAY_PERIOD_DATE"]).Month, 1);

                                while (true)
                                {
                                    this.dgvDateDataGridView.Rows.Add(dtMonthBegin.ToString("MMMM yyyy"),
                                                                       dtMonthBegin.ToString("yyyyMM"));

                                    dtMonthBegin = dtMonthBegin.AddMonths(1);

                                    if (dtMonthBegin > dtMonthEnd)
                                    {
                                        break;
                                    }
                                }

                                this.dgvDateDataGridView.Sort(this.dgvDateDataGridView.Columns[0],ListSortDirection.Descending);
                            }
                        }

                        if (this.dgvDateDataGridView.Rows.Count > 0)
                        {
                            this.btnOK.Enabled = true;

                            this.Set_DataGridView_SelectedRowIndex(this.dgvDateDataGridView, 0);
                        }
                        else
                        {
                            this.btnOK.Enabled = false;
                        }

                        this.lblMonth.Visible = true;

                        this.dgvDateDataGridView.Visible = true;
                    }
                }
            }

            this.rbnAllEmployees.Checked = true;
            EventArgs ev = new EventArgs();
            rbnAllEmployees_Click(sender, ev);
        }

        private void cboDateRule_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboDateRule.SelectedIndex == -1)
            {
                this.pnlFromDate.Visible = false;
                this.pnlToDate.Visible = false;
            }
            else
            {
                this.pnlFromDate.Visible = true;

                if (this.cboDateRule.SelectedIndex == 2)
                {
                    //Between
                    this.pnlToDate.Visible = true;
                }
                else
                {
                    this.pnlToDate.Visible = false;

                    if (this.cboDateRule.SelectedIndex == 0)
                    {
                    }
                    else
                    {
                        //Less Than
                    }
                }
            }
        }
        
        private void rbnAllEmployees_Click(object sender, EventArgs e)
        {
            this.grbEmployeeSelection.Enabled = false;
            this.rbnByPayCategory.Checked = true;

            this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
            this.Clear_DataGridView(this.dgvEmployeeDataGridView);
            this.Clear_DataGridView(this.dgvChosenEmployeeDataGridView);

            this.btnAdd.Enabled = false;
            this.btnAddAll.Enabled = false;
            this.btnRemove.Enabled = false;
            this.btnRemoveAll.Enabled = false;
        }

        private void btnAdd_Click(object sender, System.EventArgs e)
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

        private void btnAddAll_Click(object sender, System.EventArgs e)
        {
        btnAddAll_Click_Continue:

            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                btnAdd_Click(null, null);

                goto btnAddAll_Click_Continue;
            }
        }

        private void btnRemove_Click(object sender, System.EventArgs e)
        {
            if (this.dgvChosenEmployeeDataGridView.Rows.Count > 0)
            {
                int intFindRow = pvtEmployeePayCategoryDataView.Find(this.dgvChosenEmployeeDataGridView[3, this.Get_DataGridView_SelectedRowIndex(dgvChosenEmployeeDataGridView)].Value.ToString());

                DataGridViewRow myDataGridViewRow = this.dgvChosenEmployeeDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvChosenEmployeeDataGridView)];

                this.dgvChosenEmployeeDataGridView.Rows.Remove(myDataGridViewRow);

                if (intFindRow > -1)
                {
                    this.dgvEmployeeDataGridView.Rows.Add(myDataGridViewRow);
                }

                if (this.dgvChosenEmployeeDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvChosenEmployeeDataGridView, 0);
                }

                if (intFindRow > -1)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1);
                }
            }
        }

        private void btnRemoveAll_Click(object sender, System.EventArgs e)
        {
        btnRemoveAll_Click_Continue:

            if (this.dgvChosenEmployeeDataGridView.Rows.Count > 0)
            {
                btnRemove_Click(null, null);

                goto btnRemoveAll_Click_Continue;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (rbnSelectedEmployees.Checked == true
                      & this.dgvChosenEmployeeDataGridView.Rows.Count == 0)
                {
                    this.tabControl.SelectedIndex = 0;

                    CustomMessageBox.Show("Select Employees.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    this.btnAdd.Focus();

                    return;
                }

                if (this.rbnSelectedCostCentre.Checked == true
                      & this.dgvChosenCostCentreDataGridView.Rows.Count == 0)
                {
                    this.tabControl.SelectedIndex = 1;

                    CustomMessageBox.Show("Select Cost Centres.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    this.btnAdd.Focus();

                    return;
                }

                if (this.rbnSelectedLeaveTypes.Checked == true
                & dgvChosenLeaveTypeDataGridView.Rows.Count == 0)
                {
                    this.tabControl.SelectedIndex = 2;

                    CustomMessageBox.Show("Select Leave Types.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    this.dgvChosenLeaveTypeDataGridView.Focus();

                    return;
                }

                if (rbnDateOther.Checked == true)
                {
                    if (this.cboDateRule.SelectedIndex == -1)
                    {
                        CustomMessageBox.Show("Choose a Date Option.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        this.cboDateRule.Focus();

                        return;
                    }

                    if (this.txtFromDate.Text == "")
                    {
                        CustomMessageBox.Show("Select a From Date.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        return;
                    }
                    else
                    {
                        dtFromDateTime = DateTime.ParseExact(this.txtFromDate.Text, AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);
                    }

                    if (this.pnlToDate.Visible == true)
                    {
                        if (this.txtToDate.Text == "")
                        {
                            CustomMessageBox.Show("Select a To Date.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                            return;
                        }
                        else
                        {
                            dtToDateTime = DateTime.ParseExact(this.txtToDate.Text, AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);

                            if (dtToDateTime <= dtFromDateTime)
                            {
                                CustomMessageBox.Show("'From Date' must be Less Than 'To Date'.",
                               this.Text,
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Information);

                                return;
                            }
                        }
                    }
                }

                string strEarningIn = "";
                string strEmployeeIn = "";
                string strPayCategoryNoIn = "";
                //Year To Date
                string strDateOption = "Y";
                string strFromDate = "";
                string strToDate = "";

                if (this.rbnSelectedLeaveTypes.Checked == true)
                {
                    for (int intRow = 0; intRow < this.dgvChosenLeaveTypeDataGridView.Rows.Count; intRow++)
                    {
                        if (intRow == 0)
                        {
                            strEarningIn = "(" + this.dgvChosenLeaveTypeDataGridView[1, intRow].Value.ToString();
                        }
                        else
                        {
                            strEarningIn += "," + this.dgvChosenLeaveTypeDataGridView[1, intRow].Value.ToString();
                        }
                    }
                
                    strEarningIn += ")";
                }

                if (this.rbnDateAll.Checked == true)
                {
                    strDateOption = "A";
                }
                else
                {
                    if (this.rbnPayPeriod.Checked == true)
                    {
                        strDateOption = "P";
                        strFromDate = this.dgvDateDataGridView[1, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString();
                    }
                    else
                    {
                        if (this.rbnMonth.Checked == true)
                        {
                            strDateOption = "M";
                            strFromDate = this.dgvDateDataGridView[1, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString();
                        }
                        else
                        {
                            if (this.rbnPrevYear.Checked == true)
                            {
                                strDateOption = "Z";
                            }
                            else
                            {
                                if (rbnDateOther.Checked == true)
                                {
                                    strDateOption = "O";
                                    strFromDate = dtFromDateTime.ToString("yyyy-MM-dd");

                                    if (this.cboDateRule.SelectedIndex == 2)
                                    {
                                        strToDate = this.dtToDateTime.ToString("yyyy-MM-dd");
                                    }
                                }
                            }
                        }
                    }
                }
                
                if (rbnSelectedEmployees.Checked == true)
                {
                    for (int intRow = 0; intRow < this.dgvChosenEmployeeDataGridView.Rows.Count; intRow++)
                    {
                        if (intRow == 0)
                        {
                            strEmployeeIn = "(" + this.dgvChosenEmployeeDataGridView[3,intRow].Value.ToString();
                        }
                        else
                        {
                            strEmployeeIn += "," + this.dgvChosenEmployeeDataGridView[3,intRow].Value.ToString();

                        }
                    }

                    strEmployeeIn += ")";
                }

                if (this.rbnSelectedCostCentre.Checked == true)
                {
                    for (int intRow = 0; intRow < this.dgvChosenCostCentreDataGridView.Rows.Count; intRow++)
                    {
                        if (intRow == 0)
                        {
                            strPayCategoryNoIn = "(" + this.dgvChosenCostCentreDataGridView[1, intRow].Value.ToString();
                        }
                        else
                        {
                            strPayCategoryNoIn += "," + this.dgvChosenCostCentreDataGridView[1, intRow].Value.ToString();

                        }
                    }

                    strPayCategoryNoIn += ")";
                }

                string strDateSign = "G";

                if (this.cboDateRule.SelectedIndex == 1)
                {
                    strDateSign = "L";
                }

                string strPrevYearBalance = "N";

                if (this.rbnYTD.Checked == true)
                {
                    if (this.rbnPrevYearYes.Checked == true)
                    {
                        strPrevYearBalance = "Y";
                    }
                }

                string strActiveClosedInd = "";

                if (rbnEmployeeActive.Checked == true)
                {
                    strActiveClosedInd = "A";
                }
                else
                {
                    if (rbnEmployeeClosed.Checked == true)
                    {
                        strActiveClosedInd = "C";
                    }
                }

                if (pvtDataSet.Tables["Report"] != null)
                {
                    pvtDataSet.Tables.Remove("Report");
                }

                this.tabControlMain.SelectedIndex = 1;
                this.reportViewer.Clear();
             
                object[] objParm = new object[13];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = strEarningIn;
                objParm[2] = pvtstrPayrollType;
                objParm[3] = strEmployeeIn;
                objParm[4] = strPayCategoryNoIn;
                objParm[5] = strDateOption;
                objParm[6] = strFromDate;
                objParm[7] = strToDate;
                objParm[8] = strDateSign;
                objParm[9] = strPrevYearBalance;
                objParm[10] = strActiveClosedInd;
                objParm[11] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[12] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Print_Leave_Report", objParm);

                pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                if (pvtTempDataSet.Tables["ReportLeave"].Rows.Count > 0)
                {
                    pvtTempDataSet.Tables["ReportLeave"].Columns.Add("COMPANY_DESC", typeof(String));
                    pvtTempDataSet.Tables["ReportLeave"].Columns.Add("COMPANY_OPTION_DESC", typeof(String));
                    pvtTempDataSet.Tables["ReportLeave"].Columns.Add("REPORT_HEADER_DESC", typeof(String));
                    pvtTempDataSet.Tables["ReportLeave"].Columns.Add("REPORT_DATETIME", typeof(String));

                    pvtTempDataSet.Tables["ReportLeave"].Rows[0]["COMPANY_DESC"] = pvtTempDataSet.Tables["ReportHeader"].Rows[0]["COMPANY_DESC"].ToString();

                    pvtTempDataSet.Tables["ReportLeave"].Rows[0]["REPORT_DATETIME"] = pvtTempDataSet.Tables["ReportHeader"].Rows[0]["REPORT_DATETIME"].ToString();

                    if (this.rbnDateAll.Checked == true)
                    {
                        pvtTempDataSet.Tables["ReportLeave"].Rows[0]["REPORT_HEADER_DESC"] = "All Dates";
                    }
                    else
                    {
                        if (this.rbnPayPeriod.Checked == true)
                        {
                            pvtTempDataSet.Tables["ReportLeave"].Rows[0]["REPORT_HEADER_DESC"] = "Pay Period Date - " + this.dgvDateDataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString();
                        }
                        else
                        {
                            if (this.rbnYTD.Checked == true)
                            {
                                pvtTempDataSet.Tables["ReportLeave"].Rows[0]["REPORT_HEADER_DESC"] = pvtTempDataSet.Tables["ReportHeader"].Rows[0]["REPORT_HEADER"].ToString();
                            }
                            else
                            {
                                if (this.rbnPrevYear.Checked == true)
                                {
                                    pvtTempDataSet.Tables["ReportLeave"].Rows[0]["REPORT_HEADER_DESC"] = pvtTempDataSet.Tables["ReportHeader"].Rows[0]["REPORT_HEADER"].ToString();
                                }
                                else
                                {
                                    if (this.rbnMonth.Checked == true)
                                    {
                                        pvtTempDataSet.Tables["ReportLeave"].Rows[0]["REPORT_HEADER_DESC"] = this.dgvDateDataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString();
                                    }
                                    else
                                    {
                                        if (this.cboDateRule.SelectedIndex == 0)
                                        {
                                            pvtTempDataSet.Tables["ReportLeave"].Rows[0]["REPORT_HEADER_DESC"] = "Greater Than or Equal to " + dtFromDateTime.ToString("d MMMM yyyy");
                                        }
                                        else
                                        {
                                            if (this.cboDateRule.SelectedIndex == 1)
                                            {
                                                pvtTempDataSet.Tables["ReportLeave"].Rows[0]["REPORT_HEADER_DESC"] = "Less Than or Equal to " + dtFromDateTime.ToString("d MMMM yyyy");
                                            }
                                            else
                                            {
                                                pvtTempDataSet.Tables["ReportLeave"].Rows[0]["REPORT_HEADER_DESC"] = "Between " + dtFromDateTime.ToString("d MMMM yyyy") + " and " + this.dtToDateTime.ToString("d MMMM yyyy") + " (Inclusive)";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }


                    if (this.rbnEmployeeActive.Checked == true)
                    {
                        pvtTempDataSet.Tables["ReportLeave"].Rows[0]["COMPANY_OPTION_DESC"] = "Active Employees";
                    }
                    else
                    {
                        if (this.rbnEmployeeClosed.Checked == true)
                        {
                            pvtTempDataSet.Tables["ReportLeave"].Rows[0]["COMPANY_OPTION_DESC"] = "Closed Employees";
                        }
                    }
                    
                    pvtTempDataSet.AcceptChanges();

                    Microsoft.Reporting.WinForms.ReportDataSource myReportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource("ReportLeave", pvtTempDataSet.Tables["ReportLeave"]);

                    this.reportViewer.LocalReport.DataSources.Clear();
                    this.reportViewer.LocalReport.DataSources.Add(myReportDataSource);

                    this.reportViewer.RefreshReport();
                    this.reportViewer.Focus();
                }
                else
                {
                    CustomMessageBox.Show("Empty DataSet.",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void dgvPayrollTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPayrollTypeDataGridViewLoaded == true)
            {
                if (pvtintPayrollTypeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayrollTypeDataGridViewRowIndex = e.RowIndex;

                    if (e.RowIndex == 0)
                    {
                        pvtstrPayrollType = "W";
                    }
                    else
                    {
                        pvtstrPayrollType = "S";
                    }

                    Load_CurrentForm_Records();
                }
            }
        }

        private void dgvLeaveTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void dgvDateDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (rbnPayPeriod.Checked == true)
            {
                pvtdtWageDate = DateTime.ParseExact(this.dgvDateDataGridView[1, e.RowIndex].Value.ToString(), "yyyy-MM-dd", null);
            }

            this.rbnAllEmployees.Checked = true;
            EventArgs ev = new EventArgs();
            rbnAllEmployees_Click(sender, ev);
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

        private void dgvMonthDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (dgvDateDataGridView[e.Column.Index + 1, e.RowIndex1].Value.ToString() == "")
            {
                e.SortResult = -1;
            }
            else
            {
                if (dgvDateDataGridView[e.Column.Index + 1, e.RowIndex2].Value.ToString() == "")
                {
                    e.SortResult = 1;
                }
                else
                {
                    if (double.Parse(dgvDateDataGridView[e.Column.Index + 1, e.RowIndex1].Value.ToString().Replace("-", "")) > double.Parse(dgvDateDataGridView[e.Column.Index + 1, e.RowIndex2].Value.ToString().Replace("-", "")))
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(dgvDateDataGridView[e.Column.Index + 1, e.RowIndex1].Value.ToString().Replace("-", "")) < double.Parse(dgvDateDataGridView[e.Column.Index + 1, e.RowIndex2].Value.ToString().Replace("-", "")))
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

        private void dgvEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvChosenEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            this.btnAdd_Click(sender, e);
        }

        private void dgvChosenEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            this.btnRemove_Click(sender, e);
        }

        private void btnAddLeave_Click(object sender, EventArgs e)
        {
            if (this.dgvLeaveTypeDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvLeaveTypeDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvLeaveTypeDataGridView)];

                this.dgvLeaveTypeDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvChosenLeaveTypeDataGridView.Rows.Add(myDataGridViewRow);

                if (this.dgvLeaveTypeDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvLeaveTypeDataGridView, 0);
                }

                this.Set_DataGridView_SelectedRowIndex(this.dgvChosenLeaveTypeDataGridView, this.dgvChosenLeaveTypeDataGridView.Rows.Count - 1);
            }
        }

        private void btnAddAllLeave_Click(object sender, EventArgs e)
        {
        btnAddAllLeave_Click_Continue:

            if (this.dgvLeaveTypeDataGridView.Rows.Count > 0)
            {
                this.btnAddLeave_Click(null, null);

                goto btnAddAllLeave_Click_Continue;
            }
        }

        private void btnRemoveLeave_Click(object sender, EventArgs e)
        {
            if (this.dgvChosenLeaveTypeDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvChosenLeaveTypeDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvChosenLeaveTypeDataGridView)];

                this.dgvChosenLeaveTypeDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvLeaveTypeDataGridView.Rows.Add(myDataGridViewRow);

                if (this.dgvChosenLeaveTypeDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvChosenLeaveTypeDataGridView, 0);
                }
               
                this.Set_DataGridView_SelectedRowIndex(this.dgvLeaveTypeDataGridView, this.dgvLeaveTypeDataGridView.Rows.Count - 1);
            }
        }

        private void btnRemoveAllLeave_Click(object sender, EventArgs e)
        {
        btnRemoveAllLeave_Click_Continue:

            if (this.dgvChosenLeaveTypeDataGridView.Rows.Count > 0)
            {
                this.btnRemoveLeave_Click(null, null);

                goto btnRemoveAllLeave_Click_Continue;
            }

        }

        private void dgvChosenLeaveTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvLeaveTypeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            this.btnAddLeave_Click(sender, e);
        }

        private void dgvChosenLeaveTypeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            this.btnRemoveLeave_Click(sender, e);
        }

        private void rbnAllLeaveTypes_Click(object sender, EventArgs e)
        {
            this.Clear_DataGridView(this.dgvLeaveTypeDataGridView);
            this.Clear_DataGridView(this.dgvChosenLeaveTypeDataGridView);

            this.btnAddLeave.Enabled = false;
            this.btnAddAllLeave.Enabled = false;
            this.btnRemoveLeave.Enabled = false;
            this.btnRemoveAllLeave.Enabled = false;
        }

        private void rbnSelectedLeaveTypes_Click(object sender, EventArgs e)
        {
            pvtLeaveTypeDataView = null;
            pvtLeaveTypeDataView = new DataView(pvtDataSet.Tables["LeaveType"],
                "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                "",
                DataViewRowState.CurrentRows);

            pvtblnLeaveTypeDataGridViewLoaded = false;

            for (int intRowCount = 0; intRowCount < pvtLeaveTypeDataView.Count; intRowCount++)
            {
                //Added to Selected While Testing
                this.dgvLeaveTypeDataGridView.Rows.Add(pvtLeaveTypeDataView[intRowCount]["EARNING_DESC"].ToString(),
                                                       pvtLeaveTypeDataView[intRowCount]["EARNING_NO"].ToString());
            }

            pvtblnLeaveTypeDataGridViewLoaded = true;

            this.btnAddLeave.Enabled = true;
            this.btnAddAllLeave.Enabled = true;
            this.btnRemoveLeave.Enabled = true;
            this.btnRemoveAllLeave.Enabled = true;

            this.tabControl.SelectedIndex = 2;
        }

        private void grbOtherDates_VisibleChanged(object sender, EventArgs e)
        {
            if (grbOtherDates.Visible == true)
            {
                //NB Stops This Firing When we Switch to Another TabPage
                if (pvtintTabControlCurrentSelectedIndex == 0)
                {
                    if (this.cboDateRule.SelectedIndex == -1)
                    {
                        cboDateRule_SelectedIndexChanged(sender, e);
                    }
                    else
                    {
                        this.cboDateRule.SelectedIndex = -1;
                    }
                }
            }
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            pvtintTabControlCurrentSelectedIndex = tabControlMain.SelectedIndex;
        }

        private void rbnEmployee_All_Active_Closed_Click(object sender, EventArgs e)
        {
            this.Clear_DataGridView(this.dgvChosenEmployeeDataGridView);

            this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView,this.Get_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView));
        }

        private void dgvPayCategoryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPayCategoryDataGridViewLoaded == true)
            {
                if (pvtintPayCategoryDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayCategoryDataGridViewRowIndex = e.RowIndex;

                    this.Clear_DataGridView(this.dgvEmployeeDataGridView);

                    bool blnFound = false;
                    int intFindRow = -1;
                    int intPayCategoryNo = Convert.ToInt32(dgvPayCategoryDataGridView[1, e.RowIndex].Value);

                    string strEmployeePayCategoryFilter = "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_NO = " + intPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'";

                    if (rbnPayPeriod.Checked == true)
                    {
                        strEmployeePayCategoryFilter += " AND PAY_PERIOD_DATE = '" + pvtdtWageDate.ToString("yyyy-MM-dd") + "'";
                    }

                    pvtEmployeePayCategoryDataView = null;
                    pvtEmployeePayCategoryDataView = new DataView(pvtDataSet.Tables["EmployeePayCategory"],
                    strEmployeePayCategoryFilter,
                    "EMPLOYEE_NO",
                    DataViewRowState.CurrentRows);

                    string strEmployeeFilter = "";

                    if (this.rbnEmployeeActive.Checked == true)
                    {
                        strEmployeeFilter = " AND EMPLOYEE_ENDDATE IS NULL";
                    }
                    else
                    {
                        if (this.rbnEmployeeClosed.Checked == true)
                        {
                            strEmployeeFilter = " AND NOT EMPLOYEE_ENDDATE IS NULL";
                        }
                    }

                    pvtEmployeeDataView = null;
                    pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + strEmployeeFilter,
                    "EMPLOYEE_CODE",
                    DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
                    {
                        intFindRow = pvtEmployeePayCategoryDataView.Find(pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString());

                        if (intFindRow == -1)
                        {
                            continue;
                        }
                        else
                        {
                            blnFound = false;

                            //Exclude These Employees 
                            for (int intSelectedRow = 0; intSelectedRow < this.dgvChosenEmployeeDataGridView.Rows.Count; intSelectedRow++)
                            {
                                if (this.dgvChosenEmployeeDataGridView[3, intSelectedRow].Value.ToString() == pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString())
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


                        this.dgvEmployeeDataGridView.Rows.Add(pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                              pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                              pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                              pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString());
                    }
                }
            }
        }

        private void rbnSelectedCostCentre_Click(object sender, EventArgs e)
        {
            this.Clear_DataGridView(this.dgvCostCentreDataGridView);
            this.Clear_DataGridView(this.dgvChosenCostCentreDataGridView);

            pvtPayCategoryDataView = null;
            pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"],
            "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
            "PAY_CATEGORY_DESC",
            DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < pvtPayCategoryDataView.Count; intRow++)
            {
                this.dgvCostCentreDataGridView.Rows.Add(pvtPayCategoryDataView[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                         pvtPayCategoryDataView[intRow]["PAY_CATEGORY_NO"].ToString());
            }


            if (this.dgvCostCentreDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvCostCentreDataGridView, 0);
            }

            this.btnCostCentreAdd.Enabled = true;
            this.btnCostCentreAddAll.Enabled = true;
            this.btnCostCentreRemove.Enabled = true;
            this.btnCostCentreRemoveAll.Enabled = true;

            this.tabControl.SelectedIndex = 1;
        }

        private void dgvChosenCostCentreDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvCostCentreDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnCostCentreAdd_Click(object sender, System.EventArgs e)
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

        private void btnCostCentreRemove_Click(object sender, System.EventArgs e)
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

        private void dgvCostCentreDataGridView_DoubleClick(object sender, EventArgs e)
        {
            this.btnCostCentreAdd_Click(sender, e);
        }

        private void dgvChosenCostCentreDataGridView_DoubleClick(object sender, EventArgs e)
        {
            this.btnCostCentreRemove_Click(sender, e);
        }

        private void btnCostCentreAddAll_Click(object sender, EventArgs e)
        {
        btnCostCentreAddAll_Click_Continue:

            if (this.dgvCostCentreDataGridView.Rows.Count > 0)
            {
                this.btnCostCentreAdd_Click(null, null);

                goto btnCostCentreAddAll_Click_Continue;
            }
        }

        private void btnCostCentreRemoveAll_Click(object sender, EventArgs e)
        {
        btnCostCentreRemoveAll_Click_Continue:

            if (this.dgvChosenCostCentreDataGridView.Rows.Count > 0)
            {
                this.btnCostCentreRemove_Click(null, null);

                goto btnCostCentreRemoveAll_Click_Continue;
            }
        }

        private void rbnAllCostCentre_Click(object sender, EventArgs e)
        {
            this.Clear_DataGridView(this.dgvCostCentreDataGridView);
            this.Clear_DataGridView(this.dgvChosenCostCentreDataGridView);

            this.btnCostCentreAdd.Enabled = false;
            this.btnCostCentreAddAll.Enabled = false;
            this.btnCostCentreRemove.Enabled = false;
            this.btnCostCentreRemoveAll.Enabled = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmRptLeaveSelection_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
            {
                //Cancel Button
                this.Close();
            }
        }

        private void tabControlMain_KeyDown(object sender, KeyEventArgs e)
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

        private void rbnByPayCategory_CheckedChanged(object sender, EventArgs e)
        {
            if (rbnByPayCategory.Checked == true)
            {
                this.lblEmployee.Top = pvtintlblEmployeeTop;
                this.dgvEmployeeDataGridView.Top = pvtintdgvEmployeeDataGridViewTop;
                this.dgvEmployeeDataGridView.Height = pvtintdgvEmployeeDataGridViewHeight;

                this.lblEmployee.BringToFront();

                this.btnAdd.Top = pvtintbtnEmployeeAddTop;
                this.btnAddAll.Top = pvtintbtnEmployeeAddAllTop;
                this.btnRemove.Top = pvtintbtnEmployeeRemoveTop;
                this.btnRemoveAll.Top = pvtintbtnEmployeeRemoveAllTop;

                if (dgvPayCategoryDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, 0);
                }

                this.tabControl.SelectedIndex = 0;
            }
        }

        private void rbnEmployeeOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (rbnEmployeeOnly.Checked == true)
            {
                this.lblEmployee.Top = this.lblCostCentre.Top;

                this.dgvEmployeeDataGridView.Top = this.dgvChosenEmployeeDataGridView.Top;
                this.dgvEmployeeDataGridView.Height = this.dgvChosenEmployeeDataGridView.Height;
                this.dgvEmployeeDataGridView.BringToFront();

                this.lblEmployee.BringToFront();

                this.btnAdd.Top = this.btnCostCentreAdd.Top;
                this.btnAddAll.Top = this.btnCostCentreAddAll.Top;
                this.btnRemove.Top = this.btnCostCentreRemove.Top;
                this.btnRemoveAll.Top = this.btnCostCentreRemoveAll.Top;

                bool blnFound = false;
                int intFindRow = -1;

                this.Clear_DataGridView(this.dgvEmployeeDataGridView);
                
                string strFilter = "COMPANY_NO = " + Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo"));
                strFilter += " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'";

                if (rbnPayPeriod.Checked == true)
                {
                    strFilter += " AND PAY_PERIOD_DATE = '" + pvtdtWageDate.ToString("yyyy-MM-dd") + "'";
                }

                pvtEmployeePayCategoryDataView = null;
                pvtEmployeePayCategoryDataView = new DataView(this.pvtDataSet.Tables["EmployeePayCategory"],
                    strFilter,
                    "EMPLOYEE_NO",
                    DataViewRowState.CurrentRows);
                             
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
                                                            "-1");
                }

                if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, 0);
                }

                this.tabControl.SelectedIndex = 0;
            }
        }
    }
}
