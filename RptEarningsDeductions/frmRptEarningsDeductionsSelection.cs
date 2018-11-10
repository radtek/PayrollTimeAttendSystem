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
    public partial class frmRptEarningsDeductionsSelection : Form
    {
        //Visibility to Hide Column in Header
        //=IIf(IsNothing(First(Fields!PAY_CATEGORY_DESC.Value, "Report")), True, False)

        clsISUtilities clsISUtilities;

        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
        private int pvtintPayCategoryDataGridViewRowIndex = -1;

        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;
        private DataView pvtEarningDataView;
        private DataView pvtDeductionDataView;
        private DataView pvtEmployeeDataView;
        private DataView pvtPayCategoryDataView;
        private DataView pvtEmployeePayCategoryDataView;
        private DataView pvtCostCentreDataView;

        private DataView pvtDateDataView;

        private byte[] pvtbytCompress;
     
        DateTime dtStartDateTime = DateTime.Now;
        DateTime dtEndDateTime = DateTime.Now;

        private int pvtintNumberHorizontalSpreadSheets = 0;
        private int pvtintTabControlCurrentSelectedIndex = 0;

        private int pvtintHorizontalPage = 0;

        private int pvtintTimerCount = 0;

        private string pvtstrPayrollType = "";
        private string pvtstrRunType = "";

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnDateDataGridViewLoaded = false;
        private bool pvtblnPayCategoryDataGridViewLoaded = false;

        DateTime pvtdtWageDate = DateTime.Now.AddYears(-50);

        private int pvtintlblEmployeeTop = 0;
        private int pvtintdgvEmployeeDataGridViewTop = 0;
        private int pvtintdgvEmployeeDataGridViewHeight = 0;

        private int pvtintbtnEmployeeAddTop = 0;
        private int pvtintbtnEmployeeAddAllTop = 0;
        private int pvtintbtnEmployeeRemoveTop = 0;
        private int pvtintbtnEmployeeRemoveAllTop = 0;

        public frmRptEarningsDeductionsSelection()
        {
            InitializeComponent();

            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 114;

                this.tabSelection.Height += 114;
                this.tabControl.Height += 114;
                this.tabPage1.Height += 114;
                this.tabPage2.Height += 114;

                this.grbEmployee.Top += 114;
                this.grbCostCentre.Top += 114;
                this.grbEarning.Top += 114;
                this.grbDeduction.Top += 114;

                this.grbReportOptions.Top += 114;
                this.grbReportPeriod.Top += 114;
                this.grbReportType.Top += 114;

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

                this.btnEarningAdd.Top += 57;
                this.btnEarningAddAll.Top += 57;
                this.btnEarningRemove.Top += 57;
                this.btnEarningRemoveAll.Top += 57;

                this.btnDeductionAdd.Top += 57;
                this.btnDeductionAddAll.Top += 57;
                this.btnDeductionRemove.Top += 57;
                this.btnDeductionRemoveAll.Top += 57;
                
                this.dgvCostCentreDataGridView.Height += 114;
                this.dgvChosenCostCentreDataGridView.Height += 114;

                this.dgvEarningDataGridView.Height += 114;
                this.dgvChosenEarningDataGridView.Height += 114;

                this.dgvDeductionDataGridView.Height += 114;
                this.dgvChosenDeductionDataGridView.Height += 114;
                
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

        private void frmRptEarningsDeductionsSelection_Load(object sender, System.EventArgs e)
        {
            try
            {
                string strSearchDirectory =  AppDomain.CurrentDomain.BaseDirectory.Replace("PayrollIS\\bin\\Debug\\","RptEarningsDeductions");
                string strRptName = "";

                string[] strFiles = Directory.GetFiles(strSearchDirectory,"*.rdlc");

                foreach (string strRptPath in strFiles)
                {
                    strRptName = Path.GetFileNameWithoutExtension(strRptPath);
                }
             
                clsISUtilities = new clsISUtilities(this,"busRptEarningsDeductions");

                clsISUtilities.Create_Calender_Control_From_TextBox(this.txtStartDate);
                clsISUtilities.Create_Calender_Control_From_TextBox(this.txtEndDate);

                clsISUtilities.NotDataBound_Date_TextBox(txtStartDate, "");
                clsISUtilities.NotDataBound_Date_TextBox(txtEndDate, "");

                clsISUtilities.NotDataBound_ComboBox(this.cboDateRule,"");
                
                clsISUtilities.Set_Form_For_Edit(false);
                
                clsISUtilities.Calender_Control_From_TextBox_Enable(this.txtStartDate);
                clsISUtilities.Calender_Control_From_TextBox_Enable(this.txtEndDate);

                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblChosenEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEarning.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSelectedEarning.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblDeduction.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSelectedDeduction.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblDate.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayCategory.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSelectedCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                pvtDataSet = new DataSet();

                object[] objParm = new object[3];
                objParm[0] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[2] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                this.dgvPayrollTypeDataGridView.Rows.Add("Wages");
                this.dgvPayrollTypeDataGridView.Rows.Add("Salaries");
                this.dgvPayrollTypeDataGridView.Rows.Add("Wages Take-On");
                this.dgvPayrollTypeDataGridView.Rows.Add("Salaries Take-On");

                pvtblnPayrollTypeDataGridViewLoaded = true;

                this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView, 0);
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

                    case "dgvEarningDataGridView":

                        this.dgvEarningDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvChosenEarningDataGridView":

                        this.dgvChosenEarningDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvDeductionDataGridView":

                        this.dgvDeductionDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvChosenDeductionDataGridView":

                        this.dgvChosenDeductionDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvDateDataGridView":

                        this.dgvDateDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPayCategoryDataGridView":

                        pvtintPayCategoryDataGridViewRowIndex = -1;
                        this.dgvPayCategoryDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvChosenCostCentreDataGridView":

                        this.dgvChosenCostCentreDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvCostCentreDataGridView":

                        this.dgvCostCentreDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        private void rbnPayPeriod_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rbnPayPeriod.Checked == true)
            {
                this.lblDate.Text = "Pay Period Date";

                Clear_Old_Report();

                this.chkConsolidate.Enabled = false;
                this.chkConsolidate.Checked = true;

                this.chkByPayParameter.Enabled = true;

                this.Clear_DataGridView(this.dgvDateDataGridView);
                this.dgvDateDataGridView.Enabled = true;

                this.txtStartDate.Text = "";
                this.txtEndDate.Text = "";

                this.grbOtherDates.Visible = false;

                this.chkConsolidate.Enabled = false;
                this.chkConsolidate.Checked = true;

                string strFilter = "COMPANY_NO = " + Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo"));
                strFilter += " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND RUN_TYPE = '" + pvtstrRunType + "'";

                pvtDateDataView = null;
                pvtDateDataView = new DataView(this.pvtDataSet.Tables["Dates"],
                    strFilter,
                    "PAY_PERIOD_DATE DESC",
                    DataViewRowState.CurrentRows);

                pvtblnDateDataGridViewLoaded = false;

                //First Build Pay Category ListBox
                for (int intRow = 0; intRow < pvtDateDataView.Count; intRow++)
                {
                    if (intRow != 0)
                    {
                        if (Convert.ToDateTime(pvtDateDataView[intRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") == this.dgvDateDataGridView[1,this.dgvDateDataGridView.Rows.Count - 1].Value.ToString())
                        {
                            continue;
                        }
                    }

                    this.dgvDateDataGridView.Rows.Add(Convert.ToDateTime(pvtDateDataView[intRow]["PAY_PERIOD_DATE"]).ToString("dd MMMM yyyy"),
                                                      Convert.ToDateTime(pvtDateDataView[intRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd"));
                }

                pvtblnDateDataGridViewLoaded = true;

                if (this.dgvDateDataGridView.Rows.Count > 0)
                {
                    this.btnOK.Enabled = true;

                    this.Set_DataGridView_SelectedRowIndex(this.dgvDateDataGridView, 0);
                }
                else
                {
                    this.btnOK.Enabled = false;
                }

                this.rbnEmployeeAll.Checked = true;
            }
        }

        private void rbnYTD_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rbnYTD.Checked == true)
            {
                Clear_Old_Report();

                this.chkConsolidate.Checked = false;
                this.chkConsolidate.Enabled = true;

                this.chkByPayParameter.Enabled = true;

                this.lblDate.Visible = false;
               
                this.dgvDateDataGridView.Visible = false;

                this.Clear_DataGridView(this.dgvDateDataGridView);

                this.grbOtherDates.Visible = false;

                this.txtStartDate.Text = "";
                this.txtEndDate.Text = "";

                DateTime dtNow = DateTime.Now;

                this.rbnEmployeeAll.Checked = true;
            }
            else
            {
                this.dgvDateDataGridView.Visible = true;
                this.lblDate.Visible = true;
            }
        }

        private void rbnMonthly_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rbnMonthly.Checked == true)
            {
                this.lblDate.Text = "Month";

                Clear_Old_Report();

                this.chkConsolidate.Checked = false;
                this.chkConsolidate.Enabled = true;

                this.chkByPayParameter.Enabled = true;

                this.txtStartDate.Text = "";
                this.txtEndDate.Text = "";

                this.grbOtherDates.Visible = false;

                this.dgvDateDataGridView.Enabled = true;
                
                string strFilter = "COMPANY_NO = " + Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo"));
                strFilter += " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND RUN_TYPE = '" + pvtstrRunType + "'";

                pvtDateDataView = null;
                pvtDateDataView = new DataView(this.pvtDataSet.Tables["Dates"],
                    strFilter,
                    "PAY_PERIOD_DATE DESC",
                    DataViewRowState.CurrentRows);

                this.Clear_DataGridView(this.dgvDateDataGridView);


                this.pvtblnDateDataGridViewLoaded = false;

                for (int intRow = 0; intRow < pvtDateDataView.Count; intRow++)
                {
                    if (this.dgvDateDataGridView.Rows.Count > 0)
                    {
                        //Need to Look at
                        if (this.dgvDateDataGridView[1, this.dgvDateDataGridView.Rows.Count - 1].Value.ToString() == Convert.ToDateTime(pvtDateDataView[intRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM"))
                        {
                            continue;
                        }
                    }

                    this.dgvDateDataGridView.Rows.Add(Convert.ToDateTime(pvtDateDataView[intRow]["PAY_PERIOD_DATE"]).ToString("MMMM yyyy"),
                                                      Convert.ToDateTime(pvtDateDataView[intRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM"));
                }

                this.pvtblnDateDataGridViewLoaded = true;

                if (this.dgvDateDataGridView.Rows.Count > 0)
                {
                    this.btnOK.Enabled = true;
                    this.Set_DataGridView_SelectedRowIndex(this.dgvDateDataGridView, 0);
                }
                else
                {
                    this.btnOK.Enabled = false;
                }

                this.rbnEmployeeAll.Checked = true;
            }
        }

        private void rbnOther_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rbnOther.Checked == true)
            {
                Clear_Old_Report();

                this.chkConsolidate.Checked = false;
                this.chkConsolidate.Enabled = true;

                this.chkByPayParameter.Enabled = true;

                this.dgvDateDataGridView.Visible = false;
                this.lblDate.Visible = false;

                this.Clear_DataGridView(this.dgvDateDataGridView);

                this.grbOtherDates.Visible = true;

                this.rbnEmployeeAll.Checked = true;
            }
            else
            {
                this.dgvDateDataGridView.Visible = true;
                this.lblDate.Visible = true;
            }
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (this.rbnEmployeeSelected.Checked == true)
                {
                    if (this.dgvChosenEmployeeDataGridView.Rows.Count == 0)
                    {
                        this.tabSelection.SelectedIndex = 0;

                        CustomMessageBox.Show("Select Employees",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        return;
                    }
                }

                if (this.rbnCostCentreSelected.Checked == true)
                {
                    if (this.dgvChosenCostCentreDataGridView.Rows.Count == 0)
                    {
                        this.tabSelection.SelectedIndex = 1;

                        CustomMessageBox.Show("Select Cost Centres",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        return;
                    }
                }

                if (this.rbnEarningSelected.Checked == true)
                {
                    if (this.dgvChosenEarningDataGridView.Rows.Count == 0)
                    {
                        this.tabSelection.SelectedIndex = 2;

                        CustomMessageBox.Show("Select Earnings.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        return;
                    }
                }

                if (this.rbnDeductionSelected.Checked == true)
                {
                    if (this.dgvChosenDeductionDataGridView.Rows.Count == 0)
                    {
                        this.tabSelection.SelectedIndex = 3;

                        CustomMessageBox.Show("Select Deductions.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        return;
                    }
                }

                if (this.rbnEarningNone.Checked == true
                    & this.rbnDeductionNone.Checked == true)
                {
                    CustomMessageBox.Show("Options 'Earning - None' and 'Deduction - None' Cannot be Chosen at the Same Time.",
                           this.Text,
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Information);
                    return;
                }

                if (this.rbnOther.Checked == true)
                {
                    if (this.cboDateRule.SelectedIndex == -1)
                    {
                        CustomMessageBox.Show("Choose Other Dates Option (ComboBox).",
                           this.Text,
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Information);

                        return;
                    }

                    if (this.txtStartDate.Text.Length == 10)
                    {
                        dtStartDateTime = DateTime.ParseExact(this.txtStartDate.Text, AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);
                    }
                    else
                    {
                        CustomMessageBox.Show("Enter 'From Date'.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        return;
                    }

                    if (this.pnlToDate.Visible == true)
                    {
                        if (this.txtEndDate.Text.Length == 10)
                        {
                            dtEndDateTime = DateTime.ParseExact(this.txtEndDate.Text, AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);

                            if (dtEndDateTime <= dtStartDateTime)
                            {
                                CustomMessageBox.Show("'From Date' must be Less Than 'To Date'.",
                               this.Text,
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Information);

                                return;
                            }
                        }
                        else
                        {
                            CustomMessageBox.Show("Enter 'To Date'.",
                                this.Text,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            return;
                        }
                    }
                }
                else
                {
                    if (this.rbnPrevYear.Checked == true)
                    {
                    }
                    else
                    {
                        if (this.rbnYTD.Checked != true)
                        {
                            //There is NO Information for Selected Criteria

                            if (this.dgvDateDataGridView.Rows.Count == 0)
                            {
                                CustomMessageBox.Show("There is NO Information for the Selected Criteria",
                                    this.Text,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);

                                return;
                            }
                        }
                    }
                }

                Clear_Old_Report();
                
                //Build Query
                string strWhere = " WHERE EPCH.COMPANY_NO = " + Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo"));
                //2017 Remove to cater for Employee Change PAY_CATEGORY_TYPE
                //strWhere += " AND EPCH.PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'";
                //string strDeductionWhere = " AND EDH.PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'";
                string strDeductionWhere = "";

                if (pvtstrRunType == "T")
                {
                    strWhere += " AND EPCH.RUN_TYPE = '" + pvtstrRunType + "'";
                    strDeductionWhere = " AND EDH.RUN_TYPE = '" + pvtstrRunType + "'";
                }
               
                if (this.rbnPayPeriod.Checked == true)
                {
                    strWhere += " AND EPCH.PAY_PERIOD_DATE = '" + this.dgvDateDataGridView[1,this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString() + "'";

                    strDeductionWhere += " AND EDH.PAY_PERIOD_DATE = '" + this.dgvDateDataGridView[1, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString() + "'";
                }
                else
                {
                    if (this.rbnMonthly.Checked == true)
                    {
                        //NB Leave Space after Year Bracket for Replace Command on Web Service
                        strWhere += " AND YEAR(EPCH.PAY_PERIOD_DATE ) = " + this.dgvDateDataGridView[1, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString().Substring(0, 4) + " AND MONTH(EPCH.PAY_PERIOD_DATE ) = " + this.dgvDateDataGridView[1, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString().Substring(5);

                        strDeductionWhere += " AND YEAR(EDH.PAY_PERIOD_DATE ) = " + this.dgvDateDataGridView[1, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString().Substring(0, 4) + " AND MONTH(EDH.PAY_PERIOD_DATE ) = " + this.dgvDateDataGridView[1, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString().Substring(5);
                    }
                    else
                    {
                        if (this.rbnYTD.Checked == true)
                        {
                            strWhere += " AND EPCH.PAY_PERIOD_DATE >= 'BEGINOFFISCALYEAR'";
                            strDeductionWhere += " AND EDH.PAY_PERIOD_DATE >= 'BEGINOFFISCALYEAR'";
                        }
                        else
                        {
                            if (this.rbnPrevYear.Checked == true)
                            {
                                strWhere += " AND EPCH.PAY_PERIOD_DATE >= 'BEGINOFFISCALYEAR'";
                                strWhere += " AND EPCH.PAY_PERIOD_DATE < 'ENDOFFISCALYEAR'";

                                strDeductionWhere += " AND EDH.PAY_PERIOD_DATE >= 'BEGINOFFISCALYEAR'";
                                strDeductionWhere += " AND EDH.PAY_PERIOD_DATE < 'ENDOFFISCALYEAR'";
                            }
                            else
                            {
                                if (this.rbnOther.Checked == true)
                                {
                                    if (this.cboDateRule.SelectedIndex == 1)
                                    {
                                        strWhere += " AND EPCH.PAY_PERIOD_DATE <= '" + dtStartDateTime.ToString("yyyy-MM-dd") + "'";
                                        strDeductionWhere += " AND EDH.PAY_PERIOD_DATE <= '" + dtStartDateTime.ToString("yyyy-MM-dd") + "'";
                                    }
                                    else
                                    {
                                        strWhere += " AND EPCH.PAY_PERIOD_DATE >= '" + dtStartDateTime.ToString("yyyy-MM-dd") + "'";
                                        strDeductionWhere += " AND EDH.PAY_PERIOD_DATE >= '" + dtStartDateTime.ToString("yyyy-MM-dd") + "'";
                                    }

                                    if (this.pnlToDate.Visible == true)
                                    {
                                        strWhere += " AND EPCH.PAY_PERIOD_DATE <= '" + dtEndDateTime.ToString("yyyy-MM-dd") + "'";
                                        strDeductionWhere += " AND EDH.PAY_PERIOD_DATE <= '" + dtEndDateTime.ToString("yyyy-MM-dd") + "'";
                                    }
                                }
                            }
                        }
                    }
                }

                //Filter By Employee
                if (this.rbnEmployeeSelected.Checked == true)
                {
                    strWhere += " AND EPCH.EMPLOYEE_NO IN (";
                    strDeductionWhere += " AND EDH.EMPLOYEE_NO IN (";

                    for (int intRowCount = 0; intRowCount < this.dgvChosenEmployeeDataGridView.Rows.Count; intRowCount++)
                    {
                        if (intRowCount == 0)
                        {
                            strWhere += this.dgvChosenEmployeeDataGridView[3,intRowCount].Value.ToString();
                            strDeductionWhere += this.dgvChosenEmployeeDataGridView[3, intRowCount].Value.ToString();
                        }
                        else
                        {
                            strWhere += "," + this.dgvChosenEmployeeDataGridView[3, intRowCount].Value.ToString();
                            strDeductionWhere += "," + this.dgvChosenEmployeeDataGridView[3, intRowCount].Value.ToString();
                        }
                    }

                    strWhere += ") ";
                    strDeductionWhere += ") ";
                }

                string strPayCategoryNoIN = "";

                //Filter By PayCategory
                if (this.rbnCostCentreSelected.Checked == true)
                {
                    for (int intRowCount = 0; intRowCount < this.dgvChosenCostCentreDataGridView.Rows.Count; intRowCount++)
                    {
                        if (intRowCount == 0)
                        {
                            strPayCategoryNoIN += this.dgvChosenCostCentreDataGridView[1, intRowCount].Value.ToString();
                        }
                        else
                        {
                            strPayCategoryNoIN += "," + this.dgvChosenCostCentreDataGridView[1, intRowCount].Value.ToString();
                        }
                    }
                }

                //Pay Period Date
                string strDateOption = "P";

                if (this.rbnYTD.Checked == true)
                {
                    strDateOption = "Y";
                }
                else
                {
                    if (this.rbnPrevYear.Checked == true)
                    {
                        strDateOption = "B";
                    }
                    else
                    {
                        if (this.rbnMonthly.Checked == true)
                        {
                            strDateOption = "M";
                        }
                        else
                        {
                            if (this.rbnOther.Checked == true)
                            {
                                strDateOption = "O";
                            }
                        }
                    }
                }

                string strByPayParameterOption = "";

                if (this.chkByPayParameter.Checked == true)
                {
                    strByPayParameterOption = "Y";
                }
                else
                {
                    strByPayParameterOption = "N";
                }

                string strShowGrossEarningsDeductions = "";

                if (chkShowSpreadSheetTotals.Checked == true)
                {
                    strShowGrossEarningsDeductions = "Y";
                }
                else
                {
                    strShowGrossEarningsDeductions = "N";
                }

                string parstrEarningsSelectedIN = "";

                if (rbnEarningSelected.Checked == true)
                {
                    for (int intRow = 0; intRow < this.dgvChosenEarningDataGridView.Rows.Count; intRow++)
                    {
                        if (intRow == 0)
                        {
                            parstrEarningsSelectedIN += this.dgvChosenEarningDataGridView[1, intRow].Value.ToString();
                        }
                        else
                        {
                            parstrEarningsSelectedIN += "," + this.dgvChosenEarningDataGridView[1, intRow].Value.ToString();
                        }
                    }
                }
                else
                {
                    if (rbnEarningNone.Checked == true)
                    {
                        parstrEarningsSelectedIN = "-1";
                    }
                }

                string parstrDeductionsSelectedIN = "";

                if (this.rbnDeductionSelected.Checked == true)
                {
                    for (int intRow = 0; intRow < this.dgvChosenDeductionDataGridView.Rows.Count; intRow++)
                    {
                        if (intRow == 0)
                        {
                            parstrDeductionsSelectedIN += this.dgvChosenDeductionDataGridView[1, intRow].Value.ToString();
                        }
                        else
                        {
                            parstrDeductionsSelectedIN += "," + this.dgvChosenDeductionDataGridView[1, intRow].Value.ToString();
                        }
                    }
                }
                else
                {
                    if (rbnDeductionNone.Checked == true)
                    {
                        parstrDeductionsSelectedIN = "-1";
                    }
                }
 
                string strConsolidateOption = "N";

                if (this.chkConsolidate.Checked == true)
                {
                    strConsolidateOption = "Y";
                }

                this.reportViewer.LocalReport.DataSources.Clear();
                this.reportViewer.Reset();
                this.reportViewer.Refresh();

                if (this.rbnReportNormal.Checked == true)
                {
                    this.tabControl.SelectedIndex = 1;

                    //pvtstrPayrollType is Handled in Where Clause

                    object[] objParm = new object[12];
                    objParm[0] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[2] = pvtstrPayrollType;
                    objParm[3] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                    objParm[4] = strWhere;
                    objParm[5] = strDeductionWhere;
                    objParm[6] = strDateOption;
                    objParm[7] = strByPayParameterOption;
                    objParm[8] = parstrEarningsSelectedIN;
                    objParm[9] = parstrDeductionsSelectedIN;
                    objParm[10] = strPayCategoryNoIN;
                    objParm[11] = strConsolidateOption;
                
                    pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Print_Report_Normal", objParm);

                    pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                    if (pvtTempDataSet.Tables["Report"].Rows.Count > 0)
                    {
                        pvtTempDataSet.Tables["Report"].Columns.Add("COMPANY_DESC", typeof(String));
                        pvtTempDataSet.Tables["Report"].Columns.Add("REPORT_HEADER_DESC", typeof(String));
                        pvtTempDataSet.Tables["Report"].Columns.Add("REPORT_DATETIME", typeof(String));

                        pvtTempDataSet.Tables["Report"].Columns.Add("REPORT_PAY_CATEGORY_HEADER", typeof(String));
                        pvtTempDataSet.Tables["Report"].Columns.Add("REPORT_DATE_HEADER", typeof(String));
                
                        pvtTempDataSet.Tables["Report"].Rows[0]["COMPANY_DESC"] = pvtTempDataSet.Tables["ReportHeader"].Rows[0]["COMPANY_DESC"].ToString();

                        pvtTempDataSet.Tables["Report"].Rows[0]["REPORT_HEADER_DESC"] = Generate_Report_Header();

                        pvtTempDataSet.Tables["Report"].Rows[0]["REPORT_DATETIME"] = pvtTempDataSet.Tables["ReportHeader"].Rows[0]["REPORT_DATETIME"].ToString();
                        pvtTempDataSet.Tables["Report"].Rows[0]["REPORT_PAY_CATEGORY_HEADER"] = pvtTempDataSet.Tables["ReportHeader"].Rows[0]["REPORT_PAY_CATEGORY_HEADER"].ToString();
                        pvtTempDataSet.Tables["Report"].Rows[0]["REPORT_DATE_HEADER"] = pvtTempDataSet.Tables["ReportHeader"].Rows[0]["REPORT_DATE_HEADER"].ToString();

                        pvtTempDataSet.AcceptChanges();

                        Microsoft.Reporting.WinForms.ReportDataSource myReportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource("Report", pvtTempDataSet.Tables["Report"]);

                        this.reportViewer.LocalReport.ReportEmbeddedResource = "RptEarningsDeductions.ReportEarningsDeductions.rdlc";
                        this.reportViewer.LocalReport.DataSources.Clear();
                        this.reportViewer.LocalReport.DataSources.Add(myReportDataSource);

                        this.reportViewer.RefreshReport();
                    }
                    else
                    {
                        CustomMessageBox.Show("Empty DataSet.",
                           this.Text,
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Information);
                    }
                }
                else
                {
                    object[] objParm = new object[13];
                    objParm[0] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[2] = pvtstrPayrollType;
                    objParm[3] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                    objParm[4] = strWhere;
                    objParm[5] = strDeductionWhere;
                    objParm[6] = strDateOption;
                    objParm[7] = strByPayParameterOption;
                    objParm[8] = parstrEarningsSelectedIN;
                    objParm[9] = parstrDeductionsSelectedIN;
                    objParm[10] = strPayCategoryNoIN;
                    objParm[11] = strConsolidateOption;
                    objParm[12] = strShowGrossEarningsDeductions;

                    pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Print_Report_SpreadSheet", objParm);

                    pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                    pvtintNumberHorizontalSpreadSheets = Convert.ToInt32(pvtTempDataSet.Tables["NumberOfHorizontalPages"].Rows[0]["HORIZONTAL_PAGE_NUMBER"].ToString());

                    if (pvtintNumberHorizontalSpreadSheets == 1)
                    {
                        pvtintHorizontalPage = 1;

                        this.tabControl.SelectedIndex = 1;

                        Create_Spreadsheet_Report();
                    }
                    else
                    {
                        if (pvtintNumberHorizontalSpreadSheets == 2)
                        {
                            this.pbxHorzPage2.Visible = true;
                            this.pbxHorzPage3.Visible = false;
                            this.pbxHorzPage4.Visible = false;
                            this.lblPage2.Visible = true;
                            this.lblPage3.Visible = false;
                            this.lblPage4.Visible = false;

                            this.lblPage1.Text = "Page 1 of 2";
                            this.lblPage2.Text = "Page 2 of 2";
                        }
                        else
                        {
                            if (pvtintNumberHorizontalSpreadSheets == 3)
                            {
                                this.pbxHorzPage2.Visible = true;
                                this.pbxHorzPage3.Visible = true;
                                this.pbxHorzPage4.Visible = false;

                                this.lblPage2.Visible = true;
                                this.lblPage3.Visible = true;
                                this.lblPage4.Visible = false;

                                this.lblPage1.Text = "Page 1 of 3";
                                this.lblPage2.Text = "Page 2 of 3";
                                this.lblPage3.Text = "Page 3 of 3";
                            }
                            else
                            {
                                if (pvtintNumberHorizontalSpreadSheets == 4)
                                {
                                    this.pbxHorzPage2.Visible = true;
                                    this.pbxHorzPage3.Visible = true;
                                    this.pbxHorzPage4.Visible = true;

                                    this.lblPage2.Visible = true;
                                    this.lblPage3.Visible = true;
                                    this.lblPage4.Visible = true;

                                    this.lblPage1.Text = "Page 1 of 4";
                                    this.lblPage2.Text = "Page 2 of 4";
                                    this.lblPage3.Text = "Page 3 of 4";
                                    this.lblPage4.Text = "Page 4 of 4";
                                }
                            }
                        }

                        //Report
                        pvtintTimerCount = 0;
                        this.grbHorizontalPages.Visible = true;
                        this.tmrTimer.Enabled = true;
                    }
                }

                //So That Cancel Button can Fire
                this.reportViewer.Focus();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
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
        private void rbnEarningAllNone_CheckedChanged(object sender, System.EventArgs e)
        {
            Clear_Old_Report();

            if (rbnEarningAll.Checked == true
            |  rbnEarningNone.Checked == true)
            {
                this.tabSelection.SelectedIndex = 0;

                this.btnEarningAdd.Enabled = false;
                this.btnEarningRemove.Enabled = false;
                this.btnEarningAddAll.Enabled = false;
                this.btnEarningRemoveAll.Enabled = false;

                this.Clear_DataGridView(this.dgvEarningDataGridView);
                this.Clear_DataGridView(this.dgvChosenEarningDataGridView);
            }
            else
            {
                this.tabSelection.SelectedIndex = 2;
            }
        }

        private void rbnEarningSelected_Click(object sender, System.EventArgs e)
        {
            this.btnEarningAdd.Enabled = true;
            this.btnEarningRemove.Enabled = true;

            this.btnEarningAddAll.Enabled = true;
            this.btnEarningRemoveAll.Enabled = true;

            string strFilter = "COMPANY_NO = " + Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'";

            pvtEarningDataView = null;
            pvtEarningDataView = new DataView(this.pvtDataSet.Tables["Earning"],
                strFilter,
                "",
                DataViewRowState.CurrentRows);

            this.Clear_DataGridView(this.dgvEarningDataGridView);

            for (int intRowCount = 0; intRowCount < pvtEarningDataView.Count; intRowCount++)
            {
                this.dgvEarningDataGridView.Rows.Add(pvtEarningDataView[intRowCount]["EARNING_DESC"].ToString(),
                                                        pvtEarningDataView[intRowCount]["EARNING_NO"].ToString());
            }

            if (this.dgvEarningDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvEarningDataGridView, 0);
            }
        }

        private void btnEarningAdd_Click(object sender, System.EventArgs e)
        {
            if (this.dgvEarningDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvEarningDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvEarningDataGridView)];

                this.dgvEarningDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvChosenEarningDataGridView.Rows.Add(myDataGridViewRow);

                if (this.dgvEarningDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvEarningDataGridView, 0);
                }

                this.Set_DataGridView_SelectedRowIndex(this.dgvChosenEarningDataGridView, this.dgvChosenEarningDataGridView.Rows.Count - 1);
            }
        }

        private void btnEarningRemove_Click(object sender, System.EventArgs e)
        {
            if (this.dgvChosenEarningDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvChosenEarningDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvChosenEarningDataGridView)];

                this.dgvChosenEarningDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvEarningDataGridView.Rows.Add(myDataGridViewRow);

                if (this.dgvChosenEarningDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvChosenEarningDataGridView, 0);
                }

                this.Set_DataGridView_SelectedRowIndex(this.dgvEarningDataGridView, this.dgvEarningDataGridView.Rows.Count - 1);
            }
        }

        private void btnEmployeeRemoveAll_Click(object sender, System.EventArgs e)
        {
        btnEmployeeRemoveAll_Click_Continue:

            for (int intRowCount = 0; intRowCount < this.dgvChosenEmployeeDataGridView.Rows.Count; intRowCount++)
            {
                btnEmployeeRemove_Click(sender, e);

                goto btnEmployeeRemoveAll_Click_Continue;
            }
        }
      
        private void rbnEmployeeAll_CheckedChanged(object sender, System.EventArgs e)
        {
            Clear_Old_Report();

            if (rbnEmployeeAll.Checked == true)
            {
                this.tabSelection.SelectedIndex = 0;

                this.grbEmployeeSelection.Enabled = false;
                this.rbnByPayCategory.Checked = true;

                this.btnEmployeeAdd.Enabled = false;
                this.btnEmployeeAddAll.Enabled = false;
                this.btnEmployeeRemoveAll.Enabled = false;
                this.btnEmployeeRemove.Enabled = false;

                this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
                this.Clear_DataGridView(this.dgvEmployeeDataGridView);
                this.Clear_DataGridView(this.dgvChosenEmployeeDataGridView);
            }
            else
            {
                this.tabSelection.SelectedIndex = 0;
            }
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

                    string strFilter = "COMPANY_NO = " + Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    strFilter += " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND RUN_TYPE = '" + pvtstrRunType + "'";

                    string strSortField = "EMPLOYEE_CODE";

                    pvtEmployeeDataView = null;
                    pvtEmployeeDataView = new DataView(this.pvtDataSet.Tables["Employee"],
                        "COMPANY_NO = " + Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo")),
                        strSortField,
                        DataViewRowState.CurrentRows);

                    if (pvtEmployeeDataView.Count == 0)
                    {
                        object[] objParm = new object[3];
                        objParm[0] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo"));
                        objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                        objParm[2] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                        pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Employee_Records", objParm);

                        pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                        pvtDataSet.Merge(pvtTempDataSet);
                    }

                    pvtPayCategoryDataView = null;
                    pvtPayCategoryDataView = new DataView(this.pvtDataSet.Tables["PayCategory"],
                       strFilter,
                       "PAY_CATEGORY_DESC",
                       DataViewRowState.CurrentRows);

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
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void btnEmployeeAddAll_Click(object sender, System.EventArgs e)
        {
        btnEmployeeAddAll_Click_Continue:

            for (int intRowCount = 0; intRowCount < this.dgvEmployeeDataGridView.Rows.Count; intRowCount++)
            {
                btnEmployeeAdd_Click(sender, e);

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

                    if (e.RowIndex == 0
                        | e.RowIndex == 2)
                    {
                        pvtstrPayrollType = "W";
                    }
                    else
                    {
                        pvtstrPayrollType = "S";
                    }

                    if (e.RowIndex == 0
                    | e.RowIndex == 1)
                    {
                        pvtstrRunType = "P";
                    }
                    else
                    {
                        pvtstrRunType = "T";
                    }

                    this.rbnEmployeeAll.Checked = true;
                    this.rbnCostCentreAll.Checked = true;
                    this.rbnEarningAll.Checked = true;
                    this.rbnDeductionAll.Checked = true;

                    rbnEmployeeAll_CheckedChanged(sender, e);
                    rbnCostCentreAll_CheckedChanged(sender, e);
                    this.rbnEarningAllNone_CheckedChanged(sender, e);
                    this.rbnDeductionAllNone_CheckedChanged(sender, e);

                    if (rbnPayPeriod.Checked == true)
                    {
                        rbnPayPeriod_CheckedChanged(sender, e);
                    }
                    else
                    {
                        if (rbnMonthly.Checked == true)
                        {
                            rbnMonthly_CheckedChanged(sender, e);
                        }
                        else
                        {
                            //Force Pay Category
                            this.rbnPayPeriod.Checked = true;
                        }
                    }
                }
            }
        }

        private void dgvEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

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

        private void dgvEarningDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvChosenEarningDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvEarningDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnEarningAdd_Click(sender, e);
        }

        private void dgvChosenEarningDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnEarningRemove_Click(sender, e);
        }

        private void btnEarningAddAll_Click(object sender, EventArgs e)
        {
        btnEarningAddAll_Click_Continue:

            for (int intRowCount = 0; intRowCount < this.dgvEarningDataGridView.Rows.Count; intRowCount++)
            {
                btnEarningAdd_Click(sender, e);

                goto btnEarningAddAll_Click_Continue;
            }
        }

        private void btnEarningRemoveAll_Click(object sender, EventArgs e)
        {
        btnEarningRemoveAll_Click_Continue:

            for (int intRowCount = 0; intRowCount < this.dgvChosenEarningDataGridView.Rows.Count; intRowCount++)
            {
                btnEarningRemove_Click(sender, e);

                goto btnEarningRemoveAll_Click_Continue;
            }

        }

        private void dgvDeductionDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvChosenDeductionDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnDeductionAdd_Click(object sender, EventArgs e)
        {
            if (this.dgvDeductionDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvDeductionDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvDeductionDataGridView)];

                this.dgvDeductionDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvChosenDeductionDataGridView.Rows.Add(myDataGridViewRow);

                if (this.dgvDeductionDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvDeductionDataGridView, 0);
                }

                this.Set_DataGridView_SelectedRowIndex(this.dgvChosenDeductionDataGridView, this.dgvChosenDeductionDataGridView.Rows.Count - 1);
            }

        }

        private void btnDeductionAddAll_Click(object sender, EventArgs e)
        {
        btnDeductionAddAll_Click_Continue:

            for (int intRowCount = 0; intRowCount < this.dgvDeductionDataGridView.Rows.Count; intRowCount++)
            {
                btnDeductionAdd_Click(sender, e);

                goto btnDeductionAddAll_Click_Continue;
            }
        }

        private void btnDeductionRemove_Click(object sender, EventArgs e)
        {
            if (this.dgvChosenDeductionDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvChosenDeductionDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvChosenDeductionDataGridView)];

                this.dgvChosenDeductionDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvDeductionDataGridView.Rows.Add(myDataGridViewRow);

                if (this.dgvChosenDeductionDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvChosenDeductionDataGridView, 0);
                }

                this.Set_DataGridView_SelectedRowIndex(this.dgvDeductionDataGridView, this.dgvDeductionDataGridView.Rows.Count - 1);
            }

        }

        private void btnDeductionRemoveAll_Click(object sender, EventArgs e)
        {
        btnDeductionRemoveAll_Click_Continue:

            for (int intRowCount = 0; intRowCount < this.dgvChosenDeductionDataGridView.Rows.Count; intRowCount++)
            {
                btnDeductionRemove_Click(sender, e);

                goto btnDeductionRemoveAll_Click_Continue;
            }
        }

        private void rbnDeductionSelected_Click(object sender, EventArgs e)
        {
            this.btnDeductionAdd.Enabled = true;
            this.btnDeductionRemove.Enabled = true;

            this.btnDeductionAddAll.Enabled = true;
            this.btnDeductionRemoveAll.Enabled = true;

            string strFilter = "COMPANY_NO = " + Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'";

            pvtDeductionDataView = null;
            pvtDeductionDataView = new DataView(this.pvtDataSet.Tables["Deduction"],
                strFilter,
                "",
                DataViewRowState.CurrentRows);

            this.Clear_DataGridView(this.dgvDeductionDataGridView);

            for (int intRowCount = 0; intRowCount < pvtDeductionDataView.Count; intRowCount++)
            {
                this.dgvDeductionDataGridView.Rows.Add(pvtDeductionDataView[intRowCount]["DEDUCTION_DESC"].ToString(),
                                                        pvtDeductionDataView[intRowCount]["DEDUCTION_NO"].ToString());
            }

            if (this.dgvDeductionDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvDeductionDataGridView, 0);
            }
        }

        private void rbnDeductionAllNone_CheckedChanged(object sender, EventArgs e)
        {
            Clear_Old_Report();

            if (rbnDeductionAll.Checked == true
            | rbnDeductionNone.Checked == true)
            {
                this.tabSelection.SelectedIndex = 0;

                this.btnDeductionAdd.Enabled = false;
                this.btnDeductionRemove.Enabled = false;
                this.btnDeductionAddAll.Enabled = false;
                this.btnDeductionRemoveAll.Enabled = false;

                this.Clear_DataGridView(this.dgvDeductionDataGridView);
                this.Clear_DataGridView(this.dgvChosenDeductionDataGridView);
            }
            else
            {
                this.tabSelection.SelectedIndex = 3;
            }
        }

        private void dgvDeductionDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnDeductionAdd_Click(sender, e);
        }

        private void dgvChosenDeductionDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnDeductionRemove_Click(sender, e);
        }

        private void dgvDateDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvDateDataGridView.Rows.Count > 0
                & pvtblnDateDataGridViewLoaded == true)
            {
                if (rbnPayPeriod.Checked == true)
                {
                    pvtdtWageDate = DateTime.ParseExact(this.dgvDateDataGridView[1, e.RowIndex].Value.ToString(), "yyyy-MM-dd", null);
                }
                
                this.rbnEmployeeAll.Checked = true;
            }
        }

        private void pbxHorzPage_Click(object sender, EventArgs e)
        {
            try
            {
                PictureBox myPictureBox = (PictureBox)sender;

                pvtintHorizontalPage = Convert.ToInt32(myPictureBox.Tag);

                this.reportViewer.LocalReport.DataSources.Clear();
                this.reportViewer.Refresh();

                this.tabControl.SelectedIndex = 1;
                 
                object[] objParm = new object[4];
                objParm[0] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[2] = pvtintHorizontalPage;
                objParm[3] = pvtintNumberHorizontalSpreadSheets;
                
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Print_Horizontal_SpreadSheet_Page", objParm);

                pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                Create_Spreadsheet_Report();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void Create_Spreadsheet_Report()
        {
            if (pvtTempDataSet.Tables["PrintHorizontal"].Rows.Count > 0)
            {
                pvtTempDataSet.Tables["PrintHorizontal"].Columns.Add("COMPANY_DESC", typeof(String));
                pvtTempDataSet.Tables["PrintHorizontal"].Columns.Add("REPORT_HEADER_DESC", typeof(String));
                pvtTempDataSet.Tables["PrintHorizontal"].Columns.Add("HORIZONTAL_PAGE_NUMBER", typeof(String));
                pvtTempDataSet.Tables["PrintHorizontal"].Columns.Add("REPORT_DATETIME", typeof(String));
                pvtTempDataSet.Tables["PrintHorizontal"].Columns.Add("REPORT_DATE_HEADER", typeof(String));

                for (int intRow = 1; intRow < 11; intRow++)
                {
                    if (pvtintHorizontalPage == 1
                        & intRow == 10)
                    {
                        break;
                    }

                    pvtTempDataSet.Tables["PrintHorizontal"].Columns.Add("FIELD" + intRow + "_1_NAME", typeof(String));
                    pvtTempDataSet.Tables["PrintHorizontal"].Columns.Add("FIELD" + intRow + "_2_NAME", typeof(String));
                }

                pvtTempDataSet.Tables["PrintHorizontal"].Rows[0]["COMPANY_DESC"] = pvtTempDataSet.Tables["PrintHeader"].Rows[0]["COMPANY_DESC"].ToString();

                pvtTempDataSet.Tables["PrintHorizontal"].Rows[0]["REPORT_HEADER_DESC"] = Generate_Report_Header();

                pvtTempDataSet.Tables["PrintHorizontal"].Rows[0]["HORIZONTAL_PAGE_NUMBER"] = pvtTempDataSet.Tables["PrintHeader"].Rows[0]["HORIZONTAL_PAGE_NUMBER"].ToString();
                pvtTempDataSet.Tables["PrintHorizontal"].Rows[0]["REPORT_DATETIME"] = pvtTempDataSet.Tables["PrintHeader"].Rows[0]["REPORT_DATETIME"].ToString();
                pvtTempDataSet.Tables["PrintHorizontal"].Rows[0]["REPORT_DATE_HEADER"] = pvtTempDataSet.Tables["PrintHeader"].Rows[0]["REPORT_DATE_HEADER"].ToString();

                for (int intRow = 1; intRow < 11; intRow++)
                {
                    if (pvtintHorizontalPage == 1
                    & intRow == 10)
                    {
                        break;
                    }

                    pvtTempDataSet.Tables["PrintHorizontal"].Rows[0]["FIELD" + intRow + "_1_NAME"] = pvtTempDataSet.Tables["PrintHeader"].Rows[0]["FIELD" + intRow + "_1_NAME"].ToString();
                    pvtTempDataSet.Tables["PrintHorizontal"].Rows[0]["FIELD" + intRow + "_2_NAME"] = pvtTempDataSet.Tables["PrintHeader"].Rows[0]["FIELD" + intRow + "_2_NAME"].ToString();
                }

                Microsoft.Reporting.WinForms.ReportDataSource myReportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource("Report", pvtTempDataSet.Tables["PrintHorizontal"]);

                if (pvtintHorizontalPage == 1)
                {
                    this.reportViewer.LocalReport.ReportEmbeddedResource = "RptEarningsDeductions.ReportEarningsDeductionsHorizontalPage.rdlc";
                }
                else
                {
                    this.reportViewer.LocalReport.ReportEmbeddedResource = "RptEarningsDeductions.ReportEarningsDeductionsHorizontalPageSub.rdlc";
                }

                this.reportViewer.LocalReport.DataSources.Clear();
                this.reportViewer.LocalReport.DataSources.Add(myReportDataSource);

                this.reportViewer.RefreshReport();
            }
            else
            {
                CustomMessageBox.Show("Empty DataSet.",
                   this.Text,
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Information);
            }
        }

        private void rbnReportNormal_Click(object sender, EventArgs e)
        {
            Clear_Old_Report();

            this.chkShowSpreadSheetTotals.Checked = false;
            this.chkShowSpreadSheetTotals.Enabled = false;
        }

        private void rbnReportSpreadSheet_Click(object sender, EventArgs e)
        {
            Clear_Old_Report();

            this.chkShowSpreadSheetTotals.Checked = true;
            this.chkShowSpreadSheetTotals.Enabled = true;
        }

        private void Clear_Old_Report()
        {
            this.grbHorizontalPages.Visible = false;
            this.reportViewer.Clear();
        }

        private string Generate_Report_Header()
        {
            string strHeader = "";

            if (this.rbnPayPeriod.Checked == true)
            {
                strHeader = "Pay Period Date - " + this.dgvDateDataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString();
            }
            else
            {
                if (this.rbnYTD.Checked == true)
                {
                    DateTime dtBeginOfFiscalYear;

                    if (DateTime.Now.Month > 2)
                    {
                        dtBeginOfFiscalYear = new DateTime(DateTime.Now.Year, 3, 1);
                    }
                    else
                    {
                        dtBeginOfFiscalYear = new DateTime(DateTime.Now.Year - 1, 3, 1);
                    }

                    strHeader = "Year To Date (" + dtBeginOfFiscalYear.ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()) + " to " + dtBeginOfFiscalYear.AddYears(1).AddDays(-1).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()) + ")";
                }
                else
                {
                    if (this.rbnPrevYear.Checked == true)
                    {
                        DateTime dtBeginOfFiscalYear;

                        if (DateTime.Now.Month > 2)
                        {
                            dtBeginOfFiscalYear = new DateTime(DateTime.Now.Year - 1, 3, 1);
                        }
                        else
                        {
                            dtBeginOfFiscalYear = new DateTime(DateTime.Now.Year - 2, 3, 1);
                        }

                        strHeader = "Previous Year (" + dtBeginOfFiscalYear.ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()) + " to " + dtBeginOfFiscalYear.AddYears(1).AddDays(-1).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()) + ")";
                    }
                    else
                    {
                        if (this.rbnMonthly.Checked == true)
                        {
                            strHeader = this.dgvDateDataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvDateDataGridView)].Value.ToString();
                        }
                        else
                        {
                            if (this.cboDateRule.SelectedIndex == 0)
                            {
                                strHeader = "Greater Than or Equal to " + dtStartDateTime.ToString("d MMMM yyyy");
                            }
                            else
                            {
                                if (this.cboDateRule.SelectedIndex == 1)
                                {
                                    strHeader = "Less Than or Equal to " + dtStartDateTime.ToString("d MMMM yyyy");
                                }
                                else
                                {
                                    strHeader = "Between " + dtStartDateTime.ToString("d MMMM yyyy") + " and " + this.dtEndDateTime.ToString("d MMMM yyyy") + " (Inclusive)";
                                }
                            }
                        }
                    }
                }
            }

            return strHeader;
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

        private void cboDateRule_SelectedIndexChanged(object sender, EventArgs e)
        {
            Clear_Old_Report();

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

        private void dgvDateDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
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

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            pvtintTabControlCurrentSelectedIndex = tabControl.SelectedIndex;
        }

        private void tmrTimer_Tick(object sender, EventArgs e)
        {
            this.pvtintTimerCount += 1;

            if (this.grbHorizontalPages.Visible == true)
            {
                this.grbHorizontalPages.Visible = false;
            }
            else
            {
                this.grbHorizontalPages.Visible = true;
            }

            if (pvtintTimerCount == 2)
            {
                this.tmrTimer.Enabled = false;
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
                    strFilter += " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_CATEGORY_NO = " + intPayCategoryNo + " AND RUN_TYPE = '" + pvtstrRunType + "'";

                    if (rbnPayPeriod.Checked == true)
                    {
                        strFilter += " AND PAY_PERIOD_DATE = '" + pvtdtWageDate.ToString("yyyy-MM-dd") + "'";
                    }

                    pvtEmployeePayCategoryDataView = null;
                    pvtEmployeePayCategoryDataView = new DataView(this.pvtDataSet.Tables["EmployeePayCategory"],
                        strFilter,
                        "EMPLOYEE_NO",
                        DataViewRowState.CurrentRows);

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
                                                              pvtEmployeeDataView[intIndex]["EMPLOYEE_NO"].ToString());
                    }

                    if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, 0);
                    }
                }
            }
        }

        private void rbnCostCentreSelected_CheckedChanged(object sender, EventArgs e)
        {
             if (rbnCostCentreSelected.Checked == true)
             {
                this.Clear_DataGridView(this.dgvChosenCostCentreDataGridView);
                this.Clear_DataGridView(this.dgvCostCentreDataGridView);

                this.btnCostCentreAdd.Enabled = true;
                this.btnCostCentreAddAll.Enabled = true;
                this.btnCostCentreRemoveAll.Enabled = true;
                this.btnCostCentreRemove.Enabled = true;

                string strFilter = "COMPANY_NO = " + Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND RUN_TYPE = '" + pvtstrRunType + "'";

                pvtCostCentreDataView = null;
                pvtCostCentreDataView = new DataView(this.pvtDataSet.Tables["PayCategory"],
                    strFilter,
                    "PAY_CATEGORY_DESC",
                    DataViewRowState.CurrentRows);

                for (int intIndex = 0; intIndex < pvtCostCentreDataView.Count; intIndex++)
                {
                    this.dgvCostCentreDataGridView.Rows.Add(pvtCostCentreDataView[intIndex]["PAY_CATEGORY_DESC"].ToString(),
                                                            pvtCostCentreDataView[intIndex]["PAY_CATEGORY_NO"].ToString());
                }

                if (this.dgvCostCentreDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvCostCentreDataGridView, 0);
                }
            }
        }

        private void rbnCostCentreAll_CheckedChanged(object sender, EventArgs e)
        {
            Clear_Old_Report();

            if (rbnCostCentreAll.Checked == true)
            {
                this.tabSelection.SelectedIndex = 0;

                this.btnCostCentreAdd.Enabled = false;
                this.btnCostCentreAddAll.Enabled = false;
                this.btnCostCentreRemoveAll.Enabled = false;
                this.btnCostCentreRemove.Enabled = false;

                this.Clear_DataGridView(this.dgvCostCentreDataGridView);
                this.Clear_DataGridView(this.dgvChosenCostCentreDataGridView);
            }
            else
            {
                this.tabSelection.SelectedIndex = 1;
            }
        }

        private void dgvCostCentreDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvChosenCostCentreDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rbnPrevYear_CheckedChanged(object sender, EventArgs e)
        {
            if (rbnPrevYear.Checked == true)
            {
                Clear_Old_Report();

                this.chkConsolidate.Checked = false;
                this.chkConsolidate.Enabled = true;

                this.chkByPayParameter.Enabled = true;

                this.lblDate.Visible = false;

                this.dgvDateDataGridView.Visible = false;

                this.Clear_DataGridView(this.dgvDateDataGridView);

                this.grbOtherDates.Visible = false;

                this.txtStartDate.Text = "";
                this.txtEndDate.Text = "";

                DateTime dtNow = DateTime.Now;

                this.rbnEmployeeAll.Checked = true;
            }
            else
            {
                this.dgvDateDataGridView.Visible = true;
                this.lblDate.Visible = true;
            }
        }

        private void tabControl_KeyDown(object sender, KeyEventArgs e)
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

        private void frmRptEarningsDeductionsSelection_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
            {
                //Cancel Button
                this.Close();
            }
        }

        private void rbnEmployeeOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (rbnEmployeeOnly.Checked == true)
            {
                this.lblEmployee.Top = lblChosenEmployee.Top;

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
                strFilter += " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND RUN_TYPE = '" + pvtstrRunType + "'";

                if (rbnPayPeriod.Checked == true)
                {
                    strFilter += " AND PAY_PERIOD_DATE = '" + pvtdtWageDate.ToString("yyyy-MM-dd") + "'";
                }

                pvtEmployeePayCategoryDataView = null;
                pvtEmployeePayCategoryDataView = new DataView(this.pvtDataSet.Tables["EmployeePayCategory"],
                    strFilter,
                    "EMPLOYEE_NO",
                    DataViewRowState.CurrentRows);

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

        private void lblChosenEmployee_Click(object sender, EventArgs e)
        {

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
    }
}
