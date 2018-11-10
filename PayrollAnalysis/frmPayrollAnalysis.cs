using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace InteractPayroll
{
    public partial class frmPayrollAnalysis : Form
    {
        clsISUtilities clsISUtilities;
       
        private int pvtintNoPayslipCount = 0;

        private int pvtintPayrollTypeDataGridViewRowInex = -1;
        private int pvtintEmployeeDataGridViewRowInex = -1;
       
        clsTax clsTax;
        frmTax frmTax;

        string pvtstrMedicalAidInd = "";

        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;

        private DataView pvtEmployeeDataView;
        private DataView pvtCompanyDataView;

        private DataView pvtDeductionDescDataView;
        private DataView pvtEmployeeDeductionDataView;
        private DataView pvtEmployeePayCategoryDataView;
        private DataView pvtPayCategoryDataView;
        private DataView pvtTaxSpreadSheetDataView;
        private DataView pvtDeductionEarningPercentageDataView;
        private DataView pvtEmployeeNormalLeavePendingDataView;
        private DataView pvtEarningDescDataView;
        private DataView pvtEmployeeEarningDataView;

        private int pvtintEmployeeNameWidth = 0;
        private int pvtintDateColWidth = 0;
        
        private int pvtintEmployeeCodeCol = 3;
        private int pvtintEmployeeSurnameCol = 4;
        private int pvtintEmployeeNameCol = 5;
        private int pvtintEmployeePayslipCol = 6;
        private int pvtintEmployeePublicHolidayOrDoubleChequeCol = 7;
        private int pvtintEmployeeIndexCol = 8;

        private int pvtintEarningIRP5Col = 1;
        private int pvtintEarningDescCol = 2;
        private int pvtintEarningHoursCol = 3;
        private int pvtintEarningYTDbfCol = 4;
        private int pvtintEarningCurrentCol = 5;
        private int pvtintEarningYTDcfCol = 6;
        private int pvtintEarningIndexCol = 7;

        private int pvtintEarningOtherIRP5Col = 1;
        private int pvtintEarningOtherDescCol = 2;
        private int pvtintEarningOtherYTDbfCol = 3;
        private int pvtintEarningOtherCurrentCol = 4;
        private int pvtintEarningOtherYTDcfCol = 5;
        private int pvtintEarningOtherIndexCol = 6;
       
        private int pvtintEarningTotalYTDbfCol = 1;
        private int pvtintEarningTotalCurrentCol = 2;
        private int pvtintEarningTotalYTDcfCol = 3;
      
        private int pvtintDeductionOutstandingLoanCol = 3;
        private int pvtintDeductionYTDbfCol = 4;
        private int pvtintDeductionCurrentCol = 5;
        private int pvtintDeductionYTDcfCol = 6;
        private int pvtintDeductionIndexCol = 7;

        private string pvtstrEarningType = "";

        private int pvtintEarningsDataGridViewRowIndex = -1;
        private int pvtintDeductionsDataGridViewRowIndex = -1;
          
        double pvtdblEarningsBF = 0;
        double pvtdblEarningsTotal = 0;
        double pvtdblEarningsOtherBF = 0;
        double pvtdblEarningsOtherTotal = 0;
    
        double pvtdblTaxableEarningsYTD = 0;

        double pvtdblOtherTaxableEarningsYTD = 0;
        double pvtdblTotalNormalEarnings = 0;
        double pvtdblTotalNormalEarningsAnnualised = 0;
        double pvtdblTotalEarnedAccumAnnualInitial = 0;

        double pvtdblCommission = 0;
               
        private int pvtintEmployeePayCategoryRow = -1;

        private byte[] pvtbytCompress;

        private int pvtintCurrEmployeeRecordIndex = 0;
        private int pvtintEmployeeNo = 0;
        
        private int pvtintReturnCode = 0;

        private object[] pvtobjKey2 = new object[2];
        private object[] pvtobjKey3 = new object[3];
        private object[] pvtobjKey4 = new object[4];

        private DateTime pvtdttWageRunDate;
        private DateTime pvtdttEmployeeBirthDate;
        private DateTime pvtdttStartTaxYear;
        private DateTime pvtdttEndTaxYear;
        private DateTime pvtdttEmployeeTaxStartDate;
        private DateTime pvtdttEmployeeLastRunDate;

        private double pvtdblNumberPeriodsInYear;
        private double pvtdblEmployeeDaysWorked;
        private double pvtdblEmployeePortionOfYear;
        private double pvtdblAgeAtTaxYearEnd;
        private double pvtdblEmployeeAnnualisedFactor;

        private double pvtdblPensionYTD_Excl = 0;
        private double pvtdblRetireAnnuityYTD_Excl = 0;

        DataGridViewCellStyle TotalDataGridViewCellStyle;
        DataGridViewCellStyle HighLightDataGridViewCellStyle;
        DataGridViewCellStyle PublicHolidayDoubleChequeDataGridViewCellStyle;
        DataGridViewCellStyle PayoutLeaveDueDataGridViewCellStyle;
        DataGridViewCellStyle ReadOnlyDataGridViewCellStyle;
        DataGridViewCellStyle NotLinkedDataGridViewCellStyle;
        DataGridViewCellStyle NormalDataGridViewCellStyle;
        DataGridViewCellStyle PayLessZeroDataGridViewCellStyle;
        DataGridViewCellStyle NoPayslipDataGridViewCellStyle;
        DataGridViewCellStyle ClosedEmployeeDataGridViewCellStyle;
      
        //Values Returned From Tax Module for Show on Screen
        double[] pvtdblRetirementAnnuityAmount = new double[12];
        double pvtdblRetirementAnnuityTotal = 0;

        double[] pvtdblPensionFundAmount = new double[12];
        double pvtdblPensionFundTotal = 0;

        double[] pvtdblMedicalCappedAmount = new double[12];
        double[] pvtdblMedicalEmployerAmount = new double[12];
        double[] pvtdblMedicalEmployeeAmount = new double[12];
        double[] pvtdblMedicalFringeBenefitAmount = new double[12];
        double[] pvtdblMedicalFinalEmployeeAmount = new double[12];
        double[] pvtdblMedicalResultAmount = new double[12];

        double[] pvtdblTaxTotal = new double[11];

        double[] pvtdblUifTotal = new double[6];
        
        double pvtdblMedicalCappedTotal = 0;
        double pvtdblMedicalEmployerTotal = 0;
        double pvtdblMedicalEmployeeTotal = 0;
        double pvtdblMedicalFringeBenefitTotal = 0;
        double pvtdblMedicalFinalEmployeeTotal = 0;
        double pvtdblMedicalResultTotal = 0;

        double pvtdblEarnedYTD = 0;

        double pvtdblTotalDeductions = 0;
        
        double pvtdblTaxCalculatedRun = 0;

        private string pvtstrPayrollType = "";

        int pvtintAllTaxTableRow = -1;
        int pvtintEarningsTaxTableRow = -1;
        int pvtintMedicalAidNumberDependents = -1;
       
        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnEmployeeDataGridViewLoaded = false;
        private bool pvtblnEarningDataGridViewLoaded = false;
        private bool pvtblnEarningOtherDataGridViewLoaded = false;
        private bool pvtblnDeductionDataGridViewLoaded = false;
        
        public frmPayrollAnalysis()
        {
            InitializeComponent();
            
            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 114;

                this.dgvEmployeeDataGridView.Height += 114;

                this.lblEarning.Top += 114;
                this.picEarningsLock.Top += 114;
                this.dgvEarningsDataGridView.Top += 114;

                this.dgvEarningsTotalDataGridView.Top += 114;

                this.dgvEarningsOtherDataGridView.Top += 114;
                this.picEarningsOtherLock.Top += 114;
                this.dgvEarningsOtherTotalDataGridView.Top += 114;

                this.lblDeductionsDesc.Top = this.lblEarning.Top;
                this.picDeductionsLock.Top = this.picEarningsLock.Top;
                this.dgvDeductionsDataGridView.Top = this.dgvEarningsDataGridView.Top;
                this.dgvDeductionsDataGridView.Height = this.dgvEarningsDataGridView.Height + 57;
                this.dgvDeductionsTotalDataGridView.Top = dgvEarningsTotalDataGridView.Top + 57;
            }
        }
       
        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void frmPayrollAnalysis_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this,"busPayrollAnalysis");

                TotalDataGridViewCellStyle = new DataGridViewCellStyle();
                TotalDataGridViewCellStyle.BackColor = SystemColors.ControlDarkDark;
                TotalDataGridViewCellStyle.SelectionBackColor = SystemColors.ControlDarkDark;

                HighLightDataGridViewCellStyle = new DataGridViewCellStyle();
                HighLightDataGridViewCellStyle.BackColor = Color.Black;
                HighLightDataGridViewCellStyle.ForeColor = Color.White;
                HighLightDataGridViewCellStyle.SelectionBackColor = Color.Black;
                HighLightDataGridViewCellStyle.SelectionForeColor = Color.White;

                PublicHolidayDoubleChequeDataGridViewCellStyle = new DataGridViewCellStyle();
                PublicHolidayDoubleChequeDataGridViewCellStyle.BackColor = Color.SlateBlue;
                PublicHolidayDoubleChequeDataGridViewCellStyle.SelectionBackColor = Color.SlateBlue;

                PayoutLeaveDueDataGridViewCellStyle = new DataGridViewCellStyle();
                PayoutLeaveDueDataGridViewCellStyle.BackColor = Color.Plum;
                PayoutLeaveDueDataGridViewCellStyle.SelectionBackColor = Color.Plum;
                
                ReadOnlyDataGridViewCellStyle = new DataGridViewCellStyle();
                ReadOnlyDataGridViewCellStyle.BackColor = Color.Aqua;
                ReadOnlyDataGridViewCellStyle.SelectionBackColor = Color.Aqua;

                NotLinkedDataGridViewCellStyle = new DataGridViewCellStyle();
                NotLinkedDataGridViewCellStyle.BackColor = Color.Orange;
                NotLinkedDataGridViewCellStyle.SelectionBackColor = Color.Orange;

                NormalDataGridViewCellStyle = new DataGridViewCellStyle();
                NormalDataGridViewCellStyle.BackColor = SystemColors.Control;
                NormalDataGridViewCellStyle.SelectionBackColor = SystemColors.Control;

                PayLessZeroDataGridViewCellStyle = new DataGridViewCellStyle();
                PayLessZeroDataGridViewCellStyle.BackColor = Color.Yellow;
                PayLessZeroDataGridViewCellStyle.SelectionBackColor = Color.Yellow;

                NoPayslipDataGridViewCellStyle = new DataGridViewCellStyle();
                NoPayslipDataGridViewCellStyle.BackColor = Color.Olive;
                NoPayslipDataGridViewCellStyle.SelectionBackColor = Color.Olive;

                ClosedEmployeeDataGridViewCellStyle = new DataGridViewCellStyle();
                ClosedEmployeeDataGridViewCellStyle.BackColor = Color.Red;
                ClosedEmployeeDataGridViewCellStyle.SelectionBackColor = Color.Red;

                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEarning.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployeeInfoHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblDeductionsDesc.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                //Used For Employee CheckBoxes
                pvtintEmployeeNameWidth = this.dgvEmployeeDataGridView.Columns[pvtintEmployeeNameCol].Width;

                pvtintDateColWidth = this.dgvEmployeeInfoDataGridView.Columns[2].Width;
             
                this.dgvEarningsTotalDataGridView.Rows.Add("Earnings Sub-Total",
                                                           "0.00",
                                                           "0.00",
                                                           "0.00",
                                                           "0.00",
                                                           "0.00");

                this.dgvEarningsOtherTotalDataGridView.Rows.Add("Other Earnings Sub-Total",
                                                                "0.00",
                                                                "0.00",
                                                                "0.00",
                                                                "0.00",
                                                                "0.00");

                this.dgvEarningsOtherTotalDataGridView.Rows.Add("Earnings Total",
                                                                "0.00",
                                                                "0.00",
                                                                "0.00",
                                                                "0.00",
                                                                "0.00");

                this.dgvDeductionsTotalDataGridView.Rows.Add("Deductions Total",
                                                             "0.00",
                                                             "0.00",
                                                             "0.00");


                this.dgvDeductionsTotalDataGridView.Rows.Add("Nett Pay",
                                                             "0.00",
                                                             "0.00",
                                                             "0.00");

                this.dgvDeductionsTotalDataGridView.Rows.Add("Earnings Total",
                                                             "0.00",
                                                             "0.00",
                                                             "0.00");

                Clear_Controls();

                this.dgvEarningsOtherTotalDataGridView.Rows[1].DefaultCellStyle = this.TotalDataGridViewCellStyle;
                this.dgvDeductionsTotalDataGridView.Rows[1].DefaultCellStyle = this.TotalDataGridViewCellStyle;
               
                this.dgvEarningsTotalDataGridView[pvtintEarningTotalYTDcfCol, 0].Style = this.HighLightDataGridViewCellStyle;
                this.dgvEarningsOtherTotalDataGridView[pvtintEarningTotalYTDcfCol,0].Style = this.HighLightDataGridViewCellStyle;
                this.dgvEarningsOtherTotalDataGridView[pvtintEarningTotalCurrentCol, 1].Style = this.HighLightDataGridViewCellStyle;
                          
                object[] objParm = new object[1];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                this.Show();
                this.Refresh();
                               
                clsTax = new clsTax(pvtDataSet);

                for (int intRow = 0; intRow < pvtDataSet.Tables["PayrollType"].Rows.Count; intRow++)
                {
                    this.dgvPayrollTypeDataGridView.Rows.Add(pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE"].ToString());
                }

                this.pvtblnPayrollTypeDataGridViewLoaded = true;

                this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView,0);
                
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
                else
                {
                    if (myDataGridView.Rows.Count > 0)
                    {
                        myDataGridView.CurrentCell = myDataGridView[0, 0];

                        intReturnIndex = 0;
                    }
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

                        pvtintPayrollTypeDataGridViewRowInex = -1;
                        this.dgvPayrollTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEmployeeDataGridView":

                        pvtintEmployeeDataGridViewRowInex = -1;
                        this.dgvEmployeeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEarningsDataGridView":

                        this.dgvEarningsDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEarningsOtherDataGridView":

                        this.dgvEarningsOtherDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvDeductionsDataGridView":

                        this.dgvDeductionsDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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
            Clear_Controls();

            this.dgvEmployeeDataGridView.Columns[pvtintEmployeeNameCol].Width = pvtintEmployeeNameWidth; 

            if (pvtstrPayrollType == "W")
            {
                this.dgvEmployeeDataGridView.Columns[pvtintEmployeePayslipCol].Visible = true;

                this.lblOther.Text = "Public Holiday";

                if (this.pvtDataSet.Tables["Company"].Rows[0]["PUBLIC_HOLIDAY_IND"].ToString() == "Y")
                {
                    this.dgvEmployeeDataGridView.Columns[pvtintEmployeePublicHolidayOrDoubleChequeCol].HeaderText = "Public Holiday (Company)";
                }
                else
                {
                    this.dgvEmployeeDataGridView.Columns[pvtintEmployeePublicHolidayOrDoubleChequeCol].Visible = false;

                    this.dgvEmployeeDataGridView.Columns[pvtintEmployeeNameCol].Width += this.dgvEmployeeDataGridView.Columns[pvtintEmployeePublicHolidayOrDoubleChequeCol].Width;
                }

                this.dgvEmployeeInfoDataGridView.Columns[3].HeaderText = "Casual?";
            }
            else
            {
                this.lblOther.Text = "Double Cheque";

                //PaySlip Column
                this.dgvEmployeeDataGridView.Columns[pvtintEmployeePayslipCol].Visible = false;
                this.dgvEmployeeDataGridView.Columns[pvtintEmployeeNameCol].Width += this.dgvEmployeeDataGridView.Columns[pvtintEmployeePayslipCol].Width;

                this.dgvEmployeeDataGridView.Columns[pvtintEmployeePublicHolidayOrDoubleChequeCol].Visible = true;
                this.dgvEmployeeDataGridView.Columns[pvtintEmployeePublicHolidayOrDoubleChequeCol].HeaderText = "Double Cheque?";

                this.dgvEmployeeInfoDataGridView.Columns[3].HeaderText = "Salary";
            }

            //ELR 2014-05-10
            if (this.pvtDataSet.Tables["Company"] != null)
            {
                pvtCompanyDataView = null;
                pvtCompanyDataView = new DataView(this.pvtDataSet.Tables["Company"],
                    "PAY_CATEGORY_TYPE  = '" + pvtstrPayrollType + "'",
                    "",
                    DataViewRowState.CurrentRows);

                //End of Fiscal Year
                if (Convert.ToDateTime(pvtCompanyDataView[0]["PAY_PERIOD_DATE"]).Month > 2)
                {
                    pvtdttEndTaxYear = new DateTime(Convert.ToDateTime(pvtCompanyDataView[0]["PAY_PERIOD_DATE"]).Year + 1, 3, 1).AddDays(-1);
                }
                else
                {
                    pvtdttEndTaxYear = new DateTime(Convert.ToDateTime(pvtCompanyDataView[0]["PAY_PERIOD_DATE"]).Year, 3, 1).AddDays(-1);
                }

                pvtdttStartTaxYear = new DateTime(pvtdttEndTaxYear.Year - 1, 3, 1);

                if (pvtstrPayrollType == "S")
                {
                    pvtdblNumberPeriodsInYear = 12;
                }
                else
                {
                    TimeSpan tsDaysInYear = pvtdttEndTaxYear.Subtract(pvtdttStartTaxYear);
                    pvtdblNumberPeriodsInYear = tsDaysInYear.Days + 1;
                }

                pvtdttWageRunDate = Convert.ToDateTime(pvtCompanyDataView[0]["PAY_PERIOD_DATE"]);

                if (pvtstrPayrollType == "W")
                {
                    this.dgvEmployeeInfoDataGridView.Columns[1].HeaderText = "Days Completed";
                }
                else
                {
                    this.dgvEmployeeInfoDataGridView.Columns[1].HeaderText = "Periods Completed";
                }
                
                pvtDeductionDescDataView = new DataView(this.pvtDataSet.Tables["DeductionDesc"],
                   "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")),
                   "PAY_CATEGORY_TYPE,DEDUCTION_NO,DEDUCTION_SUB_ACCOUNT_NO",
                   DataViewRowState.CurrentRows);

                pvtEarningDescDataView = new DataView(this.pvtDataSet.Tables["EarningDesc"],
                    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")),
                    "PAY_CATEGORY_TYPE,EARNING_NO",
                    DataViewRowState.CurrentRows);


                string strFilter = "";

                if (this.rbnPayLessZero.Checked == true)
                {
                    strFilter = " AND PAY_TOTAL < 0 AND (CLOSE_IND = 'Y' OR PAYSLIP_IND = 'Y' OR PAYSLIP_IND = 'P')";
                }
                else
                {
                    if (this.rbnCloseEmployee.Checked == true)
                    {
                        strFilter = " AND CLOSE_IND = 'Y'";
                    }
                    else
                    {
                        if (this.rbnGetsPayslip.Checked == true)
                        {
                            strFilter = " AND (CLOSE_IND = 'Y' OR PAYSLIP_IND = 'Y' OR PAYSLIP_IND = 'P')";
                        }
                        else
                        {
                            if (this.rbnNoPayslip.Checked == true)
                            {
                                strFilter = " AND NOT (CLOSE_IND = 'Y' OR PAYSLIP_IND = 'Y' OR PAYSLIP_IND = 'P')";
                            }
                        }
                    }
                }

                pvtEmployeeDataView = new DataView(this.pvtDataSet.Tables["Employee"],
                    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'" + strFilter,
                    "",
                    DataViewRowState.CurrentRows);

                this.Clear_DataGridView(this.dgvEmployeeDataGridView);

                this.pvtblnEmployeeDataGridViewLoaded = false;

                bool blnPaySlip = false;
                bool blnPublicHoliday = false;
                int intPayLessThanZeroCount = 0;
                int intEmployeeRowIndex = 0;

                for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
                {
                    blnPaySlip = false;
                    blnPublicHoliday = false;

                    //P=Pass
                    if (pvtEmployeeDataView[intRow]["PAYSLIP_IND"].ToString() == "Y"
                        | pvtEmployeeDataView[intRow]["PAYSLIP_IND"].ToString() == "P"
                        | pvtEmployeeDataView[intRow]["CLOSE_IND"].ToString() == "Y")
                    {
                        blnPaySlip = true;
                    }
                    else
                    {
                        pvtintNoPayslipCount += 1;
                    }

                    if (pvtstrPayrollType == "W")
                    {
                        pvtEmployeeEarningDataView = null;
                        pvtEmployeeEarningDataView = new DataView(this.pvtDataSet.Tables["EmployeeEarning"],
                            "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND EMPLOYEE_NO = " + pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND EARNING_NO = 8",
                            "",
                            DataViewRowState.CurrentRows);

                        if (pvtEmployeeEarningDataView.Count > 0)
                        {
                            if (Convert.ToDouble(pvtEmployeeEarningDataView[0]["HOURS_DECIMAL_OTHER_VALUE"]) > 0)
                            {
                                if (Convert.ToDouble(pvtEmployeeDataView[intRow]["PUBLIC_HOLIDAY_HOURS_DECIMAL"]) > 0)
                                {
                                    blnPublicHoliday = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(pvtEmployeeDataView[intRow]["EMPLOYEE_NUMBER_CHEQUES"]) > 12)
                        {
                            if (Convert.ToInt32(pvtEmployeeDataView[intRow]["EMPLOYEE_NUMBER_CHEQUES"]) != 12 + Convert.ToInt32(pvtEmployeeDataView[intRow]["EXTRA_CHEQUES_HISTORY"]))
                            {
                                if (Convert.ToInt32(pvtEmployeeDataView[intRow]["EXTRA_CHEQUES_CURRENT"]) == 1)
                                {
                                    blnPublicHoliday = true;
                                }
                            }
                        }
                    }

                    this.dgvEmployeeDataGridView.Rows.Add("",
                                                          "",
                                                          "",
                                                          pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                          pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                          pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                          blnPaySlip,
                                                          blnPublicHoliday,
                                                          intRow.ToString());

                    if (pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString() == pvtintEmployeeNo.ToString())
                    {
                        intEmployeeRowIndex = intRow;
                    }

                    if (blnPublicHoliday == true)
                    {
                        dgvEmployeeDataGridView[0,this.dgvEmployeeDataGridView.Rows.Count - 1].Style = this.PublicHolidayDoubleChequeDataGridViewCellStyle;
                    }

                    if (pvtEmployeeDataView[intRow]["PAYSLIP_IND"].ToString() == "Y")
                    {
                        this.dgvEmployeeDataGridView[pvtintEmployeePayslipCol, this.dgvEmployeeDataGridView.Rows.Count - 1].ReadOnly = true;
                    }

                    if (Convert.ToDouble(pvtEmployeeDataView[intRow]["PAY_TOTAL"]) < 0
                        & blnPaySlip == true)
                    {
                        dgvEmployeeDataGridView[1, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = this.PayLessZeroDataGridViewCellStyle;
                        intPayLessThanZeroCount += 1;
                    }

                    if (pvtEmployeeDataView[intRow]["CLOSE_IND"].ToString() == "Y")
                    {
                        dgvEmployeeDataGridView[2, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = this.ClosedEmployeeDataGridViewCellStyle;
                    }
                    else
                    {
                        if (blnPaySlip == false)
                        {
                            dgvEmployeeDataGridView[2, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = this.NoPayslipDataGridViewCellStyle;
                        }
                    }

                    if (pvtstrPayrollType == "S")
                    {
                        if (Convert.ToInt32(pvtEmployeeDataView[intRow]["EMPLOYEE_NUMBER_CHEQUES"]) == 12 + Convert.ToInt32(pvtEmployeeDataView[intRow]["EXTRA_CHEQUES_HISTORY"]))
                        {
                            this.dgvEmployeeDataGridView[pvtintEmployeePublicHolidayOrDoubleChequeCol, this.dgvEmployeeDataGridView.Rows.Count - 1].ReadOnly = true;
                        }
                    }
                }

                this.pvtblnEmployeeDataGridViewLoaded = true;

                if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                {
                    this.btnTax.Enabled = true;

                    if (this.pvtCompanyDataView[0]["ACCESS_IND"].ToString() == "R")
                    {
                        this.btnUpdate.Enabled = false;
                    }
                    else
                    {
                        this.btnUpdate.Enabled = true;
                    }

                    this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, intEmployeeRowIndex);
                }
                else
                {
                    pvtintEmployeeNo = -1;
                    this.btnTax.Enabled = false;
                    this.btnUpdate.Enabled = false;

                    this.lblEmployeeInfoHeader.Text = "Parameters";
                }

                if (intPayLessThanZeroCount > 0)
                {
                    this.Show();
                    CustomMessageBox.Show("There are Employees with Pay < 0.\n\nSee Employee Rows Highlighted in Yellow.", "Pay Less Than Zero", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void Load_Employee()
        {
            DateTime dtPayPeriod = new DateTime(Convert.ToDateTime(pvtCompanyDataView[0]["PAY_PERIOD_DATE"]).Year, Convert.ToDateTime(pvtCompanyDataView[0]["PAY_PERIOD_DATE"]).Month, 1);

            //pvtEmployeeDeductionDataView = null;
            //pvtEmployeeDeductionDataView = new DataView(this.pvtDataSet.Tables["EmployeeDeduction"],
            //"COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND EMPLOYEE_NO = " + pvtintEmployeeNo.ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
            //"DEDUCTION_NO,DEDUCTION_SUB_ACCOUNT_NO",
            //DataViewRowState.CurrentRows);

            //pvtDeductionEarningPercentageDataView = null;
            //pvtDeductionEarningPercentageDataView = new DataView(this.pvtDataSet.Tables["DeductionEarningPercentage"],
            //"COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND EMPLOYEE_NO = " + pvtintEmployeeNo.ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
            //"DEDUCTION_NO,DEDUCTION_SUB_ACCOUNT_NO,EARNING_NO",
            //DataViewRowState.CurrentRows);

            //pvtEmployeeEarningDataView = null;
            //pvtEmployeeEarningDataView = new DataView(this.pvtDataSet.Tables["EmployeeEarning"],
            //    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND EMPLOYEE_NO = " + pvtintEmployeeNo.ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
            //    "",
            //    DataViewRowState.CurrentRows);


            pvtEmployeeDeductionDataView = null;
            pvtEmployeeDeductionDataView = new DataView(this.pvtDataSet.Tables["EmployeeDeduction"],
            "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND EMPLOYEE_NO = " + pvtintEmployeeNo.ToString(),
            "PAY_CATEGORY_TYPE,DEDUCTION_NO,DEDUCTION_SUB_ACCOUNT_NO",
            DataViewRowState.CurrentRows);

            pvtDeductionEarningPercentageDataView = null;
            pvtDeductionEarningPercentageDataView = new DataView(this.pvtDataSet.Tables["DeductionEarningPercentage"],
            "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND EMPLOYEE_NO = " + pvtintEmployeeNo.ToString(),
            "DEDUCTION_NO,DEDUCTION_SUB_ACCOUNT_NO,EARNING_NO",
            DataViewRowState.CurrentRows);

            pvtEmployeeEarningDataView = null;
            pvtEmployeeEarningDataView = new DataView(this.pvtDataSet.Tables["EmployeeEarning"],
                "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND EMPLOYEE_NO = " + pvtintEmployeeNo.ToString(),
                "",
                DataViewRowState.CurrentRows);

            this.chkCloseYes.Checked = false;
            this.chkZeroEarningAndLeave.Checked = false;

            if (pvtEmployeeDataView.Count != 0)
            {
                System.Exception ex = new System.Exception();

                if (pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["CLOSE_IND"].ToString() == "Y")
                {
                    this.chkCloseYes.Checked = true;
                }

                if (pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["CLOSE_REMOVE_EARNING_AND_LEAVE_IND"].ToString() == "Y")
                {
                    this.chkZeroEarningAndLeave.Checked = true;
                }

                pvtdttEmployeeBirthDate = Convert.ToDateTime(pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["EMPLOYEE_BIRTHDATE"]);
                pvtdttEmployeeTaxStartDate = Convert.ToDateTime(pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["EMPLOYEE_TAX_STARTDATE"]);
                pvtintMedicalAidNumberDependents = Convert.ToInt32(pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["NUMBER_MEDICAL_AID_DEPENDENTS"]);

                pvtdttEmployeeLastRunDate = Convert.ToDateTime(pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["EMPLOYEE_LAST_RUNDATE"]);
                
                clsTax.Employee_Date_Calculations(pvtdttWageRunDate, pvtdttEmployeeBirthDate,
                    pvtdttStartTaxYear, pvtdttEndTaxYear,
                    pvtdttEmployeeTaxStartDate, pvtdblNumberPeriodsInYear,
                    ref pvtdblEmployeePortionOfYear, ref pvtdblAgeAtTaxYearEnd,
                    ref pvtdblEmployeeAnnualisedFactor, pvtstrPayrollType,
                    pvtdttEmployeeLastRunDate, ref pvtdblEmployeeDaysWorked);

                this.lblEmployeeInfoHeader.Text = pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["EMPLOYEE_NAME"].ToString() + " " + pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["EMPLOYEE_SURNAME"].ToString() + " - Parameters";

                //pvtEmployeePayCategoryDataView = null;
                //pvtEmployeePayCategoryDataView = new DataView(this.pvtDataSet.Tables["EmployeePayCategory"],
                //    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND EMPLOYEE_NO = " + pvtintEmployeeNo.ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                //    "PAY_CATEGORY_NO",
                //    DataViewRowState.CurrentRows);

                pvtEmployeePayCategoryDataView = null;
                pvtEmployeePayCategoryDataView = new DataView(this.pvtDataSet.Tables["EmployeePayCategory"],
                    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND EMPLOYEE_NO = " + pvtintEmployeeNo.ToString(),
                    "PAY_CATEGORY_TYPE,PAY_CATEGORY_NO",
                    DataViewRowState.CurrentRows);

                string strValue = "";
                bool blnDefault = false;

                this.Clear_DataGridView(this.dgvEmployeeInfoDataGridView);
                this.Clear_DataGridView(this.dgvEmployeePayCategoryDataGridView);

                for (int intRow = 0; intRow < pvtEmployeePayCategoryDataView.Count; intRow++)
                {
                    blnDefault = false;
                    
                    pvtPayCategoryDataView = null;
                    pvtPayCategoryDataView = new DataView(this.pvtDataSet.Tables["Company"],
                        "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_NO = " + pvtEmployeePayCategoryDataView[intRow]["PAY_CATEGORY_NO"].ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtEmployeePayCategoryDataView[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'",
                        "",
                        DataViewRowState.CurrentRows);
                    
                    if (pvtstrPayrollType == pvtEmployeePayCategoryDataView[intRow]["PAY_CATEGORY_TYPE"].ToString()
                    && pvtEmployeePayCategoryDataView[intRow]["DEFAULT_IND"].ToString() == "Y")
                    {
                        blnDefault = true;
                    }

                    this.dgvEmployeePayCategoryDataGridView.Rows.Add(pvtPayCategoryDataView[0]["PAY_CATEGORY_DESC"].ToString(),
                                                                     Convert.ToDouble(pvtEmployeePayCategoryDataView[intRow]["HOURLY_RATE"]).ToString("#####0.00"),
                                                                     blnDefault);
                }

                if (pvtstrPayrollType == "W"
                && pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["PAY_CATEGORY_TYPE"].ToString() == "W")
                {
                    if (pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["TAX_TYPE_IND"].ToString() == "P")
                    {
                        strValue = "Yes";
                    }
                    else
                    {
                        strValue = "No";
                    }
                }

                this.dgvEmployeeInfoDataGridView.Rows.Add(pvtdblAgeAtTaxYearEnd.ToString(),
                                                          pvtdblEmployeePortionOfYear.ToString() + " of " + pvtdblNumberPeriodsInYear,
                                                          Convert.ToDateTime(pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["EMPLOYEE_TAX_STARTDATE"]).ToString("dd MMMM yyyy"),
                                                          strValue,
                                                          pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["EMPLOYEE_NUMBER_CHEQUES"].ToString(),
                                                          Convert.ToDateTime(pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["EMPLOYEE_LAST_RUNDATE"]).ToString("dd MMMM yyyy"));

                Load_Earnings_And_Deductions();
            }
        }

        private void Load_Earnings_And_Deductions()
        {
            pvtdblEarningsBF = 0;
            pvtdblEarningsTotal = 0;
            pvtdblEarningsOtherBF = 0;
            pvtdblEarningsOtherTotal = 0;
           
            double dblDeductionsBFTotal = 0;
            double dblDeductionsYTDTotal = 0;
            double dblDeductionsCFTotal = 0;
      
            this.Clear_DataGridView(this.dgvEarningsDataGridView);
            this.Clear_DataGridView(this.dgvEarningsOtherDataGridView);

            this.pvtblnEarningDataGridViewLoaded = false;
            this.pvtblnEarningOtherDataGridViewLoaded = false;

            int intEarningDescRow = 0;

            bool blnReadOnly = false;
            bool blnNotLinked = false;

            string strDesc = "";

            for (int intRow = 0; intRow < pvtEmployeeEarningDataView.Count; intRow++)
            {
                blnReadOnly = false;
                blnNotLinked = false;
                
                int intEarningNo = Convert.ToInt32(pvtEmployeeEarningDataView[intRow]["EARNING_NO"]);

                if (intEarningNo == 200)
                {
                    string strStop = "";
                }
                
                pvtobjKey2[0] = pvtEmployeeEarningDataView[intRow]["PAY_CATEGORY_TYPE"].ToString();
                pvtobjKey2[1] = pvtEmployeeEarningDataView[intRow]["EARNING_NO"].ToString();

                intEarningDescRow = pvtEarningDescDataView.Find(pvtobjKey2);
                
                if (pvtstrPayrollType != pvtEmployeeEarningDataView[intRow]["PAY_CATEGORY_TYPE"].ToString())
                {
                    //2017-02-22 Employee Changed PAY_CATEGORY_TYPE
                    if (Convert.ToDouble(pvtEmployeeEarningDataView[intRow]["TOTAL_YTD_BF"]) == 0
                        && Convert.ToDouble(pvtEmployeeEarningDataView[intRow]["TOTAL"]) == 0)
                    {
                        continue;
                    }
                }

                if (pvtEarningDescDataView[intEarningDescRow]["EARNING_NO"].ToString() == "7")
                {
                    this.dgvEarningsOtherDataGridView.Rows.Add("",
                                                               pvtEarningDescDataView[intEarningDescRow]["IRP5_CODE"].ToString(),
                                                               pvtEarningDescDataView[intEarningDescRow]["EARNING_DESC"].ToString(),
                                                               Convert.ToDouble(pvtEmployeeEarningDataView[intRow]["TOTAL_YTD_BF"]).ToString("#######0.00"),
                                                               Convert.ToDouble(pvtEmployeeEarningDataView[intRow]["TOTAL"]).ToString("#######0.00"),
                                                               Convert.ToDouble(Convert.ToDouble(pvtEmployeeEarningDataView[intRow]["TOTAL_YTD_BF"]) + Convert.ToDouble(pvtEmployeeEarningDataView[intRow]["TOTAL"])).ToString("#######0.00"),
                                                               intRow);

                    pvtdblEarningsOtherBF += Convert.ToDouble(pvtEmployeeEarningDataView[intRow]["TOTAL_YTD_BF"]);
                    pvtdblEarningsOtherTotal += Convert.ToDouble(pvtEmployeeEarningDataView[intRow]["TOTAL"]);
                }
                else
                {
                    if (Convert.ToInt32(pvtEarningDescDataView[intEarningDescRow]["LEAVE_PERCENTAGE"]) != 100
                        && intEarningNo >= 200)
                    {
                        strDesc = pvtEarningDescDataView[intEarningDescRow]["EARNING_DESC"].ToString() + " (" + Convert.ToInt32(pvtEarningDescDataView[intEarningDescRow]["LEAVE_PERCENTAGE"]).ToString("#00.00") + "%)";
                    }
                    else
                    {
                        if (Convert.ToInt32(pvtEmployeeEarningDataView[intRow]["EARNING_NO"]) == 9)
                        {
                            if (this.dgvEmployeePayCategoryDataGridView.Rows.Count > 1)
                            {
                                pvtPayCategoryDataView = null;
                                pvtPayCategoryDataView = new DataView(this.pvtDataSet.Tables["Company"],
                                    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_NO = " + pvtEmployeeEarningDataView[intRow]["PAY_CATEGORY_NO"].ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtEmployeeEarningDataView[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'",
                                    "",
                                    DataViewRowState.CurrentRows);

                                strDesc = pvtPayCategoryDataView[0]["PAY_CATEGORY_DESC"].ToString() + " - PH (" + Convert.ToDouble(pvtCompanyDataView[0]["PAIDHOLIDAY_RATE"]).ToString("0.00") + ")";
                            }
                            else
                            {
                                strDesc = pvtEarningDescDataView[intEarningDescRow]["EARNING_DESC"].ToString() + " (" + Convert.ToDouble(pvtCompanyDataView[0]["PAIDHOLIDAY_RATE"]).ToString("0.00") + ")";
                            }
                        }
                        else
                        {
                            if (Convert.ToInt32(pvtEmployeeEarningDataView[intRow]["EARNING_NO"]) > 1
                                & Convert.ToInt32(pvtEmployeeEarningDataView[intRow]["EARNING_NO"]) < 7
                                & (this.dgvEmployeePayCategoryDataGridView.Rows.Count > 1
                                | pvtstrPayrollType == "S"))
                            {
                                pvtPayCategoryDataView = null;
                                pvtPayCategoryDataView = new DataView(this.pvtDataSet.Tables["Company"],
                                    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_NO = " + pvtEmployeeEarningDataView[intRow]["PAY_CATEGORY_NO"].ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtEmployeeEarningDataView[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'",
                                    "",
                                    DataViewRowState.CurrentRows);

                                strDesc = pvtPayCategoryDataView[0]["PAY_CATEGORY_DESC"].ToString() + " - " + pvtEarningDescDataView[intEarningDescRow]["EARNING_DESC"].ToString().Replace("Overtime", "OT");
                            }
                            else
                            {
                                //Multiple of Value
                                if (pvtEmployeeEarningDataView[intRow]["EARNING_TYPE_IND"].ToString() == "X")
                                {
                                    strDesc = pvtEarningDescDataView[intEarningDescRow]["EARNING_DESC"].ToString() + " @ " + Convert.ToDouble(pvtEmployeeEarningDataView[intRow]["HOURS_DECIMAL_OTHER_VALUE"]).ToString("####0.0000");
                                }
                                else
                                {
                                    strDesc = pvtEarningDescDataView[intEarningDescRow]["EARNING_DESC"].ToString();
                                }
                            }
                        }
                    }

                    if (pvtEmployeeEarningDataView[intRow]["EARNING_NOT_LINKED"].ToString() == "Y")
                    {
                        blnNotLinked = true;
                    }
                    else
                    {
                        if ((Convert.ToInt32(pvtEmployeeEarningDataView[intRow]["EARNING_NO"]) > 0
                            & Convert.ToInt32(pvtEmployeeEarningDataView[intRow]["EARNING_NO"]) < 10))
                        {
                            if (this.pvtstrPayrollType == "S")
                            {
                                if ((Convert.ToInt32(pvtEmployeeEarningDataView[intRow]["EARNING_NO"]) > 1
                                & Convert.ToInt32(pvtEmployeeEarningDataView[intRow]["EARNING_NO"]) < 6))
                                {
                                    //OverTime
                                }
                                else
                                {
                                    blnReadOnly = true;
                                }
                            }
                            else
                            {
                                blnReadOnly = true;
                            }
                        }
                        else
                        {
                            if (Convert.ToDouble(pvtEmployeeEarningDataView[intRow]["TOTAL_DOUBLE"]) > 0
                                | Convert.ToInt32(pvtEmployeeEarningDataView[intRow]["EARNING_NO"]) > 199)
                            {
                                blnReadOnly = true;
                            }
                        }
                    }

                    if (pvtEmployeeEarningDataView[intRow]["EARNING_TYPE_IND"].ToString() == "X")
                    {
                        //Multiple
                        this.dgvEarningsDataGridView.Rows.Add("",
                                                              pvtEarningDescDataView[intEarningDescRow]["IRP5_CODE"].ToString(),
                                                              strDesc,
                                                              Convert.ToDouble(pvtEmployeeEarningDataView[intRow]["HOURS_DECIMAL"]).ToString("#######0"),
                                                              Convert.ToDouble(pvtEmployeeEarningDataView[intRow]["TOTAL_YTD_BF"]).ToString("#######0.00"),
                                                              Convert.ToDouble(pvtEmployeeEarningDataView[intRow]["TOTAL"]).ToString("#######0.00"),
                                                              Convert.ToDouble(Convert.ToDouble(pvtEmployeeEarningDataView[intRow]["TOTAL_YTD_BF"]) + Convert.ToDouble(pvtEmployeeEarningDataView[intRow]["TOTAL"])).ToString("#######0.00"),
                                                              intRow);
                    }
                    else
                    {
                        this.dgvEarningsDataGridView.Rows.Add("",
                                                              pvtEarningDescDataView[intEarningDescRow]["IRP5_CODE"].ToString(),
                                                              strDesc,
                                                              Convert.ToDouble(pvtEmployeeEarningDataView[intRow]["HOURS_DECIMAL"]).ToString("#####0.00"),
                                                              Convert.ToDouble(pvtEmployeeEarningDataView[intRow]["TOTAL_YTD_BF"]).ToString("#######0.00"),
                                                              Convert.ToDouble(pvtEmployeeEarningDataView[intRow]["TOTAL"]).ToString("#######0.00"),
                                                              Convert.ToDouble(Convert.ToDouble(pvtEmployeeEarningDataView[intRow]["TOTAL_YTD_BF"]) + Convert.ToDouble(pvtEmployeeEarningDataView[intRow]["TOTAL"])).ToString("#######0.00"),
                                                              intRow);
                    }

                    if (blnNotLinked == true)
                    {
                        dgvEarningsDataGridView[0,this.dgvEarningsDataGridView.Rows.Count - 1].Style = this.NotLinkedDataGridViewCellStyle;

                        this.dgvEarningsDataGridView.Rows[this.dgvEarningsDataGridView.Rows.Count - 1].ReadOnly = true;
                    }
                    else
                    {
                        if (blnReadOnly == true)
                        {
                            //Public Holiday (Company Paid) 
                            if (pvtEarningDescDataView[intEarningDescRow]["EARNING_NO"].ToString() == "8")
                            {
                                if (Convert.ToDouble(pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["PUBLIC_HOLIDAY_HOURS_DECIMAL"]) > 0)
                                {
                                    dgvEarningsDataGridView[0,this.dgvEarningsDataGridView.Rows.Count - 1].Style = this.PublicHolidayDoubleChequeDataGridViewCellStyle;
                                }
                                else
                                {
                                    dgvEarningsDataGridView[0,this.dgvEarningsDataGridView.Rows.Count - 1].Style = this.ReadOnlyDataGridViewCellStyle;
                                }
                            }
                            else
                            {
                                //Salary
                                if (pvtEarningDescDataView[intEarningDescRow]["EARNING_NO"].ToString() == "1")
                                {
                                    if (Convert.ToInt32(pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["EXTRA_CHEQUES_CURRENT"]) == 1)
                                    {
                                        dgvEarningsDataGridView[0,this.dgvEarningsDataGridView.Rows.Count - 1].Style = this.PublicHolidayDoubleChequeDataGridViewCellStyle;
                                    }
                                    else
                                    {
                                        dgvEarningsDataGridView[0,this.dgvEarningsDataGridView.Rows.Count - 1].Style = this.ReadOnlyDataGridViewCellStyle;
                                    }
                                }
                                else
                                {
                                    if (pvtEmployeeEarningDataView[intRow]["LEAVE_OPTION"].ToString() == "P")
                                    {
                                        dgvEarningsDataGridView[0,this.dgvEarningsDataGridView.Rows.Count - 1].Style = this.PayoutLeaveDueDataGridViewCellStyle;
                                    }
                                    else
                                    {
                                        dgvEarningsDataGridView[0,this.dgvEarningsDataGridView.Rows.Count - 1].Style = this.ReadOnlyDataGridViewCellStyle;
                                    }
                                }
                            }

                            this.dgvEarningsDataGridView.Rows[this.dgvEarningsDataGridView.Rows.Count - 1].ReadOnly = true;
                        }
                        else
                        {
                            if (pvtEmployeeEarningDataView[intRow]["EARNING_TYPE_IND"].ToString() == "X")
                            {
                                this.dgvEarningsDataGridView[pvtintEarningCurrentCol,this.dgvEarningsDataGridView.Rows.Count - 1].ReadOnly = true;
                            }
                            else
                            {
                                if (this.pvtstrPayrollType == "S"
                                & Convert.ToInt32(pvtEmployeeEarningDataView[intRow]["EARNING_NO"]) > 1
                                & Convert.ToInt32(pvtEmployeeEarningDataView[intRow]["EARNING_NO"]) < 6)
                                {
                                    this.dgvEarningsDataGridView[pvtintEarningCurrentCol, this.dgvEarningsDataGridView.Rows.Count - 1].ReadOnly = true;
                                    //OverTime
                                }
                                else
                                {
                                    this.dgvEarningsDataGridView[pvtintEarningHoursCol,this.dgvEarningsDataGridView.Rows.Count - 1].ReadOnly = true;
                                }
                            }
                        }
                    }
                   
                    pvtdblEarningsBF += Convert.ToDouble(pvtEmployeeEarningDataView[intRow]["TOTAL_YTD_BF"]);
                    pvtdblEarningsTotal += Convert.ToDouble(pvtEmployeeEarningDataView[intRow]["TOTAL"]);
                }
            }

            this.pvtblnEarningDataGridViewLoaded = true;
            this.pvtblnEarningOtherDataGridViewLoaded = true;

            this.dgvEarningsTotalDataGridView[pvtintEarningTotalYTDbfCol,0].Value = pvtdblEarningsBF.ToString("#######0.00");
            this.dgvEarningsTotalDataGridView[pvtintEarningTotalCurrentCol, 0].Value = pvtdblEarningsTotal.ToString("#######0.00");
            this.dgvEarningsTotalDataGridView[pvtintEarningTotalYTDcfCol, 0].Value = Convert.ToDouble(pvtdblEarningsTotal + pvtdblEarningsBF).ToString("#######0.00");
           
            this.dgvEarningsOtherTotalDataGridView[pvtintEarningTotalYTDbfCol, 0].Value = pvtdblEarningsOtherBF.ToString("#######0.00");
            this.dgvEarningsOtherTotalDataGridView[pvtintEarningTotalCurrentCol, 0].Value = pvtdblEarningsOtherTotal.ToString("#######0.00");
            this.dgvEarningsOtherTotalDataGridView[pvtintEarningTotalYTDcfCol, 0].Value = Convert.ToDouble(pvtdblEarningsOtherTotal + pvtdblEarningsOtherBF).ToString("#######0.00");
           
            this.dgvEarningsOtherTotalDataGridView[pvtintEarningTotalYTDbfCol, 1].Value = Convert.ToDouble(pvtdblEarningsBF + pvtdblEarningsOtherBF).ToString("#######0.00");
            this.dgvEarningsOtherTotalDataGridView[pvtintEarningTotalCurrentCol, 1].Value = Convert.ToDouble(pvtdblEarningsTotal + pvtdblEarningsOtherTotal).ToString("#######0.00");
            this.dgvEarningsOtherTotalDataGridView[pvtintEarningTotalYTDcfCol, 1].Value = Convert.ToDouble(pvtdblEarningsTotal + pvtdblEarningsOtherTotal + pvtdblEarningsBF + pvtdblEarningsOtherBF).ToString("#######0.00");
            
            //Deductions
            this.dgvDeductionsTotalDataGridView[1, 2].Value = Convert.ToDouble(pvtdblEarningsBF + pvtdblEarningsOtherBF).ToString("#######0.00");
            this.dgvDeductionsTotalDataGridView[2, 2].Value = Convert.ToDouble(pvtdblEarningsTotal + pvtdblEarningsOtherTotal).ToString("#######0.00");
            this.dgvDeductionsTotalDataGridView[3, 2].Value = Convert.ToDouble(pvtdblEarningsTotal + pvtdblEarningsOtherTotal + pvtdblEarningsBF + pvtdblEarningsOtherBF).ToString("#######0.00");

            this.Clear_DataGridView(this.dgvDeductionsDataGridView);

            pvtblnDeductionDataGridViewLoaded = false;
          
            int intDeductionRow;
            string strLoanOutstanding = "";

            for (int intRow = 0; intRow < pvtEmployeeDeductionDataView.Count; intRow++)
            {
                strLoanOutstanding = "";
                blnReadOnly = false;
                blnNotLinked = false;

                pvtobjKey3[0] = pvtEmployeeDeductionDataView[intRow]["PAY_CATEGORY_TYPE"].ToString();
                pvtobjKey3[1] = pvtEmployeeDeductionDataView[intRow]["DEDUCTION_NO"].ToString();
                pvtobjKey3[2] = pvtEmployeeDeductionDataView[intRow]["DEDUCTION_SUB_ACCOUNT_NO"].ToString();

                intDeductionRow = pvtDeductionDescDataView.Find(pvtobjKey3);

                if (pvtEmployeeDeductionDataView[intRow]["DEDUCTION_NOT_LINKED"].ToString() == "Y")
                {
                    blnNotLinked = true;
                }
                else
                {
                    //Temporary - Later to Include Macros etc
                    if (Convert.ToInt32(pvtDeductionDescDataView[intDeductionRow]["DEDUCTION_NO"]) == 1
                        | Convert.ToInt32(pvtDeductionDescDataView[intDeductionRow]["DEDUCTION_NO"]) == 2)
                    {
                        blnReadOnly = true;
                    }
                }

                if (Convert.ToInt32(pvtDeductionDescDataView[intDeductionRow]["DEDUCTION_SUB_ACCOUNT_COUNT"]) > 1)
                {
                    strDesc = pvtDeductionDescDataView[intDeductionRow]["DEDUCTION_DESC"].ToString() + " (" + pvtDeductionDescDataView[intDeductionRow]["DEDUCTION_SUB_ACCOUNT_NO"].ToString() + ")";
                }
                else
                {
                    strDesc = pvtDeductionDescDataView[intDeductionRow]["DEDUCTION_DESC"].ToString();
                }

                if (pvtEmployeeDeductionDataView[intRow]["TOTAL_OUTSTANDING_LOAN"] != System.DBNull.Value)
                {
                    if (Convert.ToDouble(pvtEmployeeDeductionDataView[intRow]["TOTAL_OUTSTANDING_LOAN"]) != 0)
                    {
                        strLoanOutstanding = Convert.ToDouble(pvtEmployeeDeductionDataView[intRow]["TOTAL_OUTSTANDING_LOAN"]).ToString("######0.00");
                    }
                    else
                    {
                        if (pvtEmployeeDeductionDataView[intRow]["DEDUCTION_LOAN_TYPE_IND"].ToString() == "Y")
                        {
                            strLoanOutstanding = Convert.ToDouble(pvtEmployeeDeductionDataView[intRow]["TOTAL_OUTSTANDING_LOAN"]).ToString("######0.00");
                        }
                    }
                }

                this.dgvDeductionsDataGridView.Rows.Add("",
                                                        pvtDeductionDescDataView[intDeductionRow]["IRP5_CODE"].ToString(),
                                                        strDesc,
                                                        strLoanOutstanding,
                                                        Convert.ToDouble(pvtEmployeeDeductionDataView[intRow]["TOTAL_YTD_BF"]).ToString("########0.00"),
                                                        Convert.ToDouble(pvtEmployeeDeductionDataView[intRow]["TOTAL"]).ToString("########0.00"),
                                                        Convert.ToDouble(Convert.ToDouble(pvtEmployeeDeductionDataView[intRow]["TOTAL_YTD_BF"]) + Convert.ToDouble(pvtEmployeeDeductionDataView[intRow]["TOTAL"])).ToString("########0.00"),
                                                        intRow.ToString());

                dblDeductionsBFTotal += Convert.ToDouble(pvtEmployeeDeductionDataView[intRow]["TOTAL_YTD_BF"]); 

                dblDeductionsCFTotal += Convert.ToDouble(pvtEmployeeDeductionDataView[intRow]["TOTAL_YTD_BF"]) + Convert.ToDouble(pvtEmployeeDeductionDataView[intRow]["TOTAL"]);

                dblDeductionsYTDTotal += Convert.ToDouble(pvtEmployeeDeductionDataView[intRow]["TOTAL"]);

                if (blnNotLinked == true)
                {
                    dgvDeductionsDataGridView[0,this.dgvDeductionsDataGridView.Rows.Count - 1].Style = this.NotLinkedDataGridViewCellStyle;

                    this.dgvDeductionsDataGridView.Rows[this.dgvDeductionsDataGridView.Rows.Count - 1].ReadOnly = true;
                }
                else
                {
                    if (blnReadOnly == true)
                    {
                        dgvDeductionsDataGridView[0,this.dgvDeductionsDataGridView.Rows.Count - 1].Style = this.ReadOnlyDataGridViewCellStyle;

                        this.dgvDeductionsDataGridView.Rows[this.dgvDeductionsDataGridView.Rows.Count - 1].ReadOnly = true;
                    }
                }
            }

            pvtblnDeductionDataGridViewLoaded = true;

            this.dgvDeductionsTotalDataGridView[1, 0].Value = dblDeductionsBFTotal.ToString("########0.00");
            this.dgvDeductionsTotalDataGridView[2,0].Value = dblDeductionsYTDTotal.ToString("########0.00");
            this.dgvDeductionsTotalDataGridView[3, 0].Value = dblDeductionsCFTotal.ToString("########0.00");
           
            this.dgvDeductionsTotalDataGridView[1, 1].Value = Convert.ToDouble(Convert.ToDouble(this.dgvDeductionsTotalDataGridView[1, 2].Value) - Convert.ToDouble(this.dgvDeductionsTotalDataGridView[1, 0].Value)).ToString("########0.00");
            this.dgvDeductionsTotalDataGridView[2, 1].Value = Convert.ToDouble(Convert.ToDouble(this.dgvDeductionsTotalDataGridView[2, 2].Value) - dblDeductionsYTDTotal).ToString("########0.00");
            this.dgvDeductionsTotalDataGridView[3, 1].Value = Convert.ToDouble(Convert.ToDouble(this.dgvDeductionsTotalDataGridView[3, 2].Value) - Convert.ToDouble(this.dgvDeductionsTotalDataGridView[3, 0].Value)).ToString("########0.00");

            pvtdblPensionYTD_Excl = 0;
            pvtdblRetireAnnuityYTD_Excl = 0;

            for (int intRow = 0; intRow < pvtEmployeeDeductionDataView.Count; intRow++)
            {
                //Pension Fund
                if (Convert.ToInt32(pvtEmployeeDeductionDataView[intRow]["DEDUCTION_NO"]) == 3)
                {
                    pvtdblPensionYTD_Excl = Convert.ToDouble(pvtEmployeeDeductionDataView[intRow]["TOTAL_YTD_BF"]);
                }

                //Retirement Annuity
                if (Convert.ToInt32(pvtEmployeeDeductionDataView[intRow]["DEDUCTION_NO"]) == 4)
                {
                    pvtdblRetireAnnuityYTD_Excl = Convert.ToDouble(pvtEmployeeDeductionDataView[intRow]["TOTAL_YTD_BF"]);
                }
            }

            if (this.dgvEarningsDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvEarningsDataGridView, 0);
            }

            //Set to First Row
            if (this.dgvDeductionsDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvDeductionsDataGridView,0);
            }
            else
            {
                //Zerorize Totals
                this.dgvDeductionsTotalDataGridView[3,0].Value = "0.00";
                //Needs To Be Looked At
                this.dgvDeductionsTotalDataGridView[3, 1].Value = Convert.ToDouble(this.dgvDeductionsTotalDataGridView[3, 0].Value).ToString("########0.00");
                this.dgvDeductionsTotalDataGridView[3, 2].Value = Convert.ToDouble(this.dgvDeductionsTotalDataGridView[3, 0].Value).ToString("########0.00");
            }
        }

        public void btnUpdate_Click(object sender, System.EventArgs e)
        {
            this.Text += " - Update";

            this.rbnNone.Enabled = false;
            this.rbnPayLessZero.Enabled = false;
            this.rbnNoPayslip.Enabled = false;
            this.rbnGetsPayslip.Enabled = false;
            this.rbnCloseEmployee.Enabled = false;

            this.dgvPayrollTypeDataGridView.Enabled = false;

            this.picPayrollTypeLock.Image = PayrollAnalysis.Properties.Resources.NewLock16;
            this.picEmployeeLock.Image = PayrollAnalysis.Properties.Resources.NewLock16;

            this.picPayrollTypeLock.Visible = true;

            this.btnUpdate.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            if (rbnNormal.Checked == true)
            {
                this.picEarningsLock.Visible = false;
                this.picEarningsOtherLock.Visible = false;
                this.picDeductionsLock.Visible = false;

                this.picEmployeeLock.Visible = true;

                this.dgvEarningsDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
                this.dgvEarningsDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;

                this.dgvEarningsDataGridView.CurrentCell = this.dgvEarningsDataGridView[3, 0];

                this.dgvEarningsOtherDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
                this.dgvEarningsOtherDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;

                this.dgvEarningsOtherDataGridView.CurrentCell = this.dgvEarningsOtherDataGridView[pvtintEarningOtherCurrentCol, 0];

                this.dgvDeductionsDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
                this.dgvDeductionsDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;

                this.dgvDeductionsDataGridView.CurrentCell = this.dgvDeductionsDataGridView[pvtintDeductionCurrentCol, 0];
    
                this.chkCloseYes.Enabled = true;

                if (this.chkCloseYes.Checked == true)
                {
                    this.chkZeroEarningAndLeave.Enabled = true;
                }

                this.dgvEmployeeDataGridView.Enabled = false;
            }
            else
            {
                this.picEarningsLock.Visible = true;
                this.picEarningsOtherLock.Visible = true;
                this.picDeductionsLock.Visible = true;

                this.picEmployeeLock.Visible = false;

                this.dgvEmployeeDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
                this.dgvEmployeeDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;

                int intColumn = 3;
     
                if (this.dgvEmployeeDataGridView.Columns[pvtintEmployeePublicHolidayOrDoubleChequeCol].Visible == true)
                {
                    intColumn = pvtintEmployeePublicHolidayOrDoubleChequeCol;
                }
                else
                {
                    if (this.dgvEmployeeDataGridView.Columns[pvtintEmployeePayslipCol].Visible == true)
                    {
                        intColumn = pvtintEmployeePayslipCol;
                    }
                }

                if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                {
                    dgvEmployeeDataGridView.CurrentCell = dgvEmployeeDataGridView[intColumn, 0];
                }
            }

            this.rbnNormal.Enabled = false;
            this.rbnOther.Enabled = false;

            this.btnTax.Enabled = false;
        }

        public void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.Text = this.Text.Substring(0, this.Text.IndexOf("- Update") - 1);

            this.rbnNone.Enabled = true;
            this.rbnPayLessZero.Enabled = true;
            this.rbnNoPayslip.Enabled = true;
            this.rbnGetsPayslip.Enabled = true;
            this.rbnCloseEmployee.Enabled = true;

            if (this.pvtCompanyDataView[0]["ACCESS_IND"].ToString() == "R")
            {
                this.btnUpdate.Enabled = false;
            }
            else
            {
                this.btnUpdate.Enabled = true;
            }

            this.rbnNormal.Checked = true;
            this.rbnNormal.Enabled = true;
            this.rbnOther.Enabled = true;

            this.picDeductionsLock.Visible = false;
            this.picEarningsLock.Visible = false;
            this.picEarningsOtherLock.Visible = false;

            this.picEmployeeLock.Visible = false;
            this.picPayrollTypeLock.Visible = false;
            
            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.btnTax.Enabled = true;

            this.dgvPayrollTypeDataGridView.Enabled = true;
            this.dgvEmployeeDataGridView.Enabled = true;

            this.pvtDataSet.RejectChanges();

            this.dgvEarningsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvEarningsDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

            this.dgvEarningsOtherDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvEarningsOtherDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

            this.dgvDeductionsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvDeductionsDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

            this.dgvEmployeeDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvEmployeeDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

            this.chkCloseYes.Enabled = false;
            this.chkZeroEarningAndLeave.Enabled = false;
          
            if (this.rbnNone.Checked == true)
            {
                for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
                {
                    if (pvtstrPayrollType == "W")
                    {
                        if (pvtEmployeeDataView[intRow]["PAYSLIP_IND"].ToString() == "Y"
                        | pvtEmployeeDataView[intRow]["PAYSLIP_IND"].ToString() == "P")
                        {
                            this.dgvEmployeeDataGridView[pvtintEmployeePayslipCol, intRow].Value = true;
                        }
                        else
                        {
                            this.dgvEmployeeDataGridView[pvtintEmployeePayslipCol, intRow].Value = false;
                        }

                        if (Convert.ToDouble(pvtEmployeeDataView[intRow]["PUBLIC_HOLIDAY_HOURS_DECIMAL"]) > 0)
                        {
                            this.dgvEmployeeDataGridView[pvtintEmployeePublicHolidayOrDoubleChequeCol, intRow].Value = true;

                            dgvEmployeeDataGridView[0,intRow].Style = this.PublicHolidayDoubleChequeDataGridViewCellStyle;
                        }
                        else
                        {
                            this.dgvEmployeeDataGridView[pvtintEmployeePublicHolidayOrDoubleChequeCol, intRow].Value = false;

                            dgvEmployeeDataGridView[0,intRow].Style = this.NormalDataGridViewCellStyle;
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(pvtEmployeeDataView[intRow]["EMPLOYEE_NUMBER_CHEQUES"]) > 12)
                        {
                            if (Convert.ToInt32(pvtEmployeeDataView[intRow]["EMPLOYEE_NUMBER_CHEQUES"]) != 12 + Convert.ToInt32(pvtEmployeeDataView[intRow]["EXTRA_CHEQUES_HISTORY"]))
                            {
                                if (Convert.ToInt32(pvtEmployeeDataView[intRow]["EXTRA_CHEQUES_CURRENT"]) == 1)
                                {
                                    dgvEmployeeDataGridView[0,intRow].Style = this.PublicHolidayDoubleChequeDataGridViewCellStyle;

                                    this.dgvEmployeeDataGridView[pvtintEmployeePublicHolidayOrDoubleChequeCol, intRow].Value = true;
                                }
                                else
                                {
                                    dgvEmployeeDataGridView[0,intRow].Style = this.NormalDataGridViewCellStyle;

                                    this.dgvEmployeeDataGridView[pvtintEmployeePublicHolidayOrDoubleChequeCol, intRow].Value = false;
                                }
                            }
                        }
                    }
                }

                if (Convert.ToDouble(dgvDeductionsTotalDataGridView[2, 1].Value) < 0)
                {
                    dgvEmployeeDataGridView[1, pvtintEmployeeDataGridViewRowInex].Style = this.PayLessZeroDataGridViewCellStyle;
                }
                else
                {
                    dgvEmployeeDataGridView[1, pvtintEmployeeDataGridViewRowInex].Style = this.NormalDataGridViewCellStyle;
                }

                //Reset Colours For Employee
                if ( pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["CLOSE_IND"].ToString() == "Y"
                | pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["PAYSLIP_IND"].ToString() == "Y"
                | pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["PAYSLIP_IND"].ToString() == "P")
                {
                    this.dgvEmployeeDataGridView[pvtintEmployeePayslipCol, pvtintEmployeeDataGridViewRowInex].Value = true;

                    if ( pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["CLOSE_IND"].ToString() == "Y")
                    {
                        dgvEmployeeDataGridView[2, pvtintEmployeeDataGridViewRowInex].Style = this.ClosedEmployeeDataGridViewCellStyle;
                    }
                    else
                    {
                        dgvEmployeeDataGridView[2, pvtintEmployeeDataGridViewRowInex].Style = this.NormalDataGridViewCellStyle;
                    }
                }
                else
                {
                    this.dgvEmployeeDataGridView[pvtintEmployeePayslipCol, pvtintEmployeeDataGridViewRowInex].Value = false;
                    dgvEmployeeDataGridView[2, pvtintEmployeeDataGridViewRowInex].Style = this.NoPayslipDataGridViewCellStyle;
                }

                if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView));
                }
            }
            else
            {
                Load_CurrentForm_Records();
            }
        }

        private void Calculate_Earnings()
        {
            double dblAmountEarned = 0;
            double dblUIFAmount = 0;

            double pvtdblEarningsBF = 0;
            double pvtdblEarningsTotal = 0;
                        
            double dblPensionYTD = 0;
            double dblRetireAnnuityYTD = 0;
            
            double dblPensionArrearYTD = 0;
            double dblRetireAnnuityArrearYTD = 0;
           
            string strPayUIFInd = "";
            string strDirectiveFilter = "";
            double dblWageDateMonth;
           
            double pvtdblTaxTakeOnYTD = 0;
            object[] objFind = new object[3];
            int intFindRow;
            int intEarningRow;
            int intEarningDescRow;

            pvtTaxSpreadSheetDataView = null;
            pvtTaxSpreadSheetDataView = new DataView(pvtDataSet.Tables["TaxSpreadSheet"],
                "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND RUN_TYPE = 'P' AND EMPLOYEE_NO = " + pvtintEmployeeNo.ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                "IRP5_CODE,PERIOD_YEAR,PERIOD_MONTH",
                DataViewRowState.CurrentRows);

            strDirectiveFilter = "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND EMPLOYEE_NO = " + pvtintEmployeeNo.ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND TAX_DIRECTIVE_PERCENTAGE > 0";
            
            pvtdblEarningsBF = 0;
            pvtdblEarningsTotal = 0;
            pvtdblEarningsOtherBF = 0;
            pvtdblEarningsOtherTotal = 0;
        
            pvtdblCommission = 0;

            for (int intRow = 0; intRow < this.dgvEarningsDataGridView.Rows.Count; intRow++)
            {
                intEarningRow = Convert.ToInt32(this.dgvEarningsDataGridView[pvtintEarningIndexCol, intRow].Value);

                pvtdblEarningsBF += Convert.ToDouble(pvtEmployeeEarningDataView[intEarningRow]["TOTAL_YTD_BF"]);
                pvtdblEarningsTotal += Convert.ToDouble(this.dgvEarningsDataGridView[pvtintEarningCurrentCol, intRow].Value);
                
                //2014-03-11
                if (Convert.ToInt32(pvtEmployeeEarningDataView[intEarningRow]["EARNING_NO"]) == 11)
                {
                    pvtdblCommission = Convert.ToDouble(pvtEmployeeEarningDataView[intEarningRow]["TOTAL"]); 
                }
            }

            for (int intRow = 0; intRow < this.dgvEarningsOtherDataGridView.Rows.Count; intRow++)
            {
                intEarningRow = Convert.ToInt32(this.dgvEarningsOtherDataGridView[pvtintEarningOtherIndexCol, intRow].Value);

                pvtobjKey2[0] = pvtEmployeeEarningDataView[intEarningRow]["PAY_CATEGORY_TYPE"].ToString();
                pvtobjKey2[1] = pvtEmployeeEarningDataView[intEarningRow]["EARNING_NO"].ToString();

                intEarningDescRow = pvtEarningDescDataView.Find(pvtobjKey2);

                pvtdblEarningsOtherBF += Convert.ToDouble(pvtEmployeeEarningDataView[intEarningRow]["TOTAL_YTD_BF"]);
                pvtdblEarningsOtherTotal += Convert.ToDouble(this.dgvEarningsOtherDataGridView[pvtintEarningOtherCurrentCol, intRow].Value);
            }

            this.dgvEarningsTotalDataGridView[pvtintEarningTotalCurrentCol, 0].Value = pvtdblEarningsTotal.ToString("#######0.00"); 
            this.dgvEarningsTotalDataGridView[pvtintEarningTotalYTDcfCol, 0].Value = Convert.ToDouble(pvtdblEarningsTotal + pvtdblEarningsBF).ToString("#######0.00");
            
            this.dgvEarningsOtherTotalDataGridView[pvtintEarningTotalCurrentCol, 0].Value = pvtdblEarningsOtherTotal.ToString("#######0.00");
            this.dgvEarningsOtherTotalDataGridView[pvtintEarningTotalYTDcfCol, 0].Value = Convert.ToDouble(pvtdblEarningsOtherTotal + pvtdblEarningsOtherBF).ToString("#######0.00");
            
            this.dgvEarningsOtherTotalDataGridView[pvtintEarningTotalCurrentCol, 1].Value = Convert.ToDouble(pvtdblEarningsTotal + pvtdblEarningsOtherTotal).ToString("#######0.00");
            this.dgvEarningsOtherTotalDataGridView[pvtintEarningTotalYTDcfCol, 1].Value = Convert.ToDouble(pvtdblEarningsTotal + pvtdblEarningsOtherTotal + pvtdblEarningsBF + pvtdblEarningsOtherBF).ToString("#######0.00");
            
            //Deductions
            this.dgvDeductionsTotalDataGridView[2, 2].Value = Convert.ToDouble(pvtdblEarningsTotal + pvtdblEarningsOtherTotal).ToString("#######0.00");
            this.dgvDeductionsTotalDataGridView[3, 2].Value = Convert.ToDouble(pvtdblEarningsTotal + pvtdblEarningsOtherTotal + pvtdblEarningsBF + pvtdblEarningsOtherBF).ToString("#######0.00");

            objFind[1] = Convert.ToDateTime(pvtCompanyDataView[0]["PAY_PERIOD_DATE"]).Year;
            objFind[2] = Convert.ToDateTime(pvtCompanyDataView[0]["PAY_PERIOD_DATE"]).Month;

            strPayUIFInd = "N";
            pvtstrMedicalAidInd = "N";
            int intTaxRowSaved = -1;
            int intUIFRowSaved = -1;
            int intEmployeeDeductionDataViewRowIndex = -1;

            for (int intRow = 0; intRow < this.dgvDeductionsDataGridView.Rows.Count; intRow++)
            {
                intEmployeeDeductionDataViewRowIndex = Convert.ToInt32(this.dgvDeductionsDataGridView[pvtintDeductionIndexCol, intRow].Value);

                //NB I'm Not Sure these are Correct - Need to Test
                switch (Convert.ToInt32(pvtEmployeeDeductionDataView[intEmployeeDeductionDataViewRowIndex]["DEDUCTION_NO"]))
                {
                    //Medical Aid
                    case 5:

                        if (pvtEmployeeDeductionDataView[intEmployeeDeductionDataViewRowIndex]["DEDUCTION_NOT_LINKED"].ToString() == "Y")
                        {
                        }
                        else
                        {
                            pvtstrMedicalAidInd = "Y"; 
                        }

                        break;

                    //Retirement Annuity
                    case 4:

                        objFind[0] = 4006;
                        intFindRow = pvtTaxSpreadSheetDataView.Find(objFind);
                        //Already Set
                        //pvtTaxSpreadSheetDataView[intFindRow]["TOTAL_VALUE"] = Convert.ToDouble(this.dgvDeductionsDataGridView[pvtintDeductionYTDcfCol, intRow].Value);

                        dblRetireAnnuityYTD = Convert.ToDouble(this.dgvDeductionsDataGridView[pvtintDeductionYTDcfCol,intRow].Value);

                        break;

                    //Pension Fund
                    case 3:

                        objFind[0] = 4001;
                        intFindRow = pvtTaxSpreadSheetDataView.Find(objFind);
                        pvtTaxSpreadSheetDataView[intFindRow]["TOTAL_VALUE"] = Convert.ToDouble(this.dgvDeductionsDataGridView[pvtintDeductionYTDcfCol, intRow].Value);

                        dblPensionYTD = Convert.ToDouble(this.dgvDeductionsDataGridView[pvtintDeductionYTDcfCol, intRow].Value);

                        break;

                    //Tax
                    case 1:

                        //Save Tax Row for later use
                        intTaxRowSaved = intRow;

                        break;

                    //UIF
                    case 2:

                        intUIFRowSaved = intRow;

                        //Save Tax Row for later use
                        if (pvtEmployeeDeductionDataView[intEmployeeDeductionDataViewRowIndex]["DEDUCTION_NOT_LINKED"].ToString() == "Y")
                        {
                        }
                        else
                        {
                            strPayUIFInd = "Y";
                        }

                        break;

                    default:

                        break;
                }
            }

            //Calculate tax
            pvtdblEarnedYTD = pvtdblEarningsTotal + pvtdblEarningsBF + pvtdblEarningsOtherTotal + pvtdblEarningsOtherBF;
            pvtdblOtherTaxableEarningsYTD = pvtdblEarningsOtherTotal + pvtdblEarningsOtherBF;

            dblPensionYTD = dblPensionYTD + pvtdblPensionYTD_Excl;
            dblRetireAnnuityYTD = dblRetireAnnuityYTD + pvtdblRetireAnnuityYTD_Excl;
          
            dblWageDateMonth = Convert.ToDateTime(pvtCompanyDataView[0]["PAY_PERIOD_DATE"]).Month;

            if (intTaxRowSaved > -1)
            {
                pvtdblTaxableEarningsYTD = Convert.ToDouble(pvtEmployeeDeductionDataView[Convert.ToInt32(this.dgvDeductionsDataGridView[pvtintDeductionIndexCol, intTaxRowSaved].Value)]["TOTAL_YTD_BF"]);
            }

            dblAmountEarned = pvtdblEarningsTotal + pvtdblEarningsOtherTotal;

            //NB NB Need To Be Set
            dblPensionArrearYTD = 0;
            dblRetireAnnuityArrearYTD = 0;
            
            pvtintReturnCode = clsTax.Calculate_Tax(pvtdblEarnedYTD, pvtdblOtherTaxableEarningsYTD,
                    ref pvtdblTaxCalculatedRun, pvtdblEmployeeAnnualisedFactor, pvtdblAgeAtTaxYearEnd,
                    dblWageDateMonth, pvtdblTaxableEarningsYTD,
                    "P", pvtdblTaxTakeOnYTD,
                    pvtstrPayrollType, strPayUIFInd,
                    pvtdblNumberPeriodsInYear, pvtdblEmployeeDaysWorked, dblAmountEarned, ref dblUIFAmount, Convert.ToDateTime(pvtCompanyDataView[0]["PAY_PERIOD_DATE"]), pvtstrMedicalAidInd,Convert.ToInt32(pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["NUMBER_MEDICAL_AID_DEPENDENTS"]),
                    pvtTaxSpreadSheetDataView, pvtEarningDescDataView, dblPensionArrearYTD, dblRetireAnnuityArrearYTD,
                    pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["TAX_TYPE_IND"].ToString(), Convert.ToDouble(pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["TAX_DIRECTIVE_PERCENTAGE"]),
                    pvtdblCommission,
                    ref pvtdblRetirementAnnuityAmount,
                    ref pvtdblRetirementAnnuityTotal,
                    ref pvtdblPensionFundAmount,
                    ref pvtdblPensionFundTotal,
                    ref pvtdblTotalNormalEarnings,
                    ref pvtdblTotalNormalEarningsAnnualised,
                    ref pvtdblTotalEarnedAccumAnnualInitial,
                    ref pvtdblTotalDeductions,
                    ref pvtdblTaxTotal,
                    ref pvtintAllTaxTableRow,
                    ref pvtintEarningsTaxTableRow,
                    ref pvtdblUifTotal
                   
                    );

            if (pvtintReturnCode != 0)
            {
                this.Dispose();
            }

            if (intTaxRowSaved > -1)
            {
                intEmployeeDeductionDataViewRowIndex = Convert.ToInt32(this.dgvDeductionsDataGridView[pvtintDeductionIndexCol, intTaxRowSaved].Value);

                //Tax
                this.dgvDeductionsDataGridView[pvtintDeductionCurrentCol, intTaxRowSaved].Value = Math.Round(pvtdblTaxCalculatedRun, 2).ToString("#########0.00");
                this.dgvDeductionsDataGridView[pvtintDeductionYTDcfCol, intTaxRowSaved].Value = Math.Round(pvtdblTaxCalculatedRun + Convert.ToDouble(pvtEmployeeDeductionDataView[intEmployeeDeductionDataViewRowIndex]["TOTAL_YTD_BF"]), 2).ToString("#########0.00");

                if (pvtdblTaxCalculatedRun != Convert.ToDouble(this.pvtEmployeeDeductionDataView[intEmployeeDeductionDataViewRowIndex]["TOTAL"]))
                {
                    this.pvtEmployeeDeductionDataView[intEmployeeDeductionDataViewRowIndex]["TOTAL"] = pvtdblTaxCalculatedRun;
                }
            }
               
            //UIF
            if (dblUIFAmount > -1
            && intUIFRowSaved > -1)
            {
                intEmployeeDeductionDataViewRowIndex = Convert.ToInt32(this.dgvDeductionsDataGridView[pvtintDeductionIndexCol, intUIFRowSaved].Value);

                this.dgvDeductionsDataGridView[pvtintDeductionCurrentCol, intUIFRowSaved].Value = Math.Round(dblUIFAmount, 2).ToString("#########0.00");
                this.dgvDeductionsDataGridView[pvtintDeductionYTDcfCol, intUIFRowSaved].Value = Math.Round(dblUIFAmount + Convert.ToDouble(pvtEmployeeDeductionDataView[intEmployeeDeductionDataViewRowIndex]["TOTAL_YTD_BF"]), 2).ToString("#########0.00");

                if (dblUIFAmount != Convert.ToDouble(this.pvtEmployeeDeductionDataView[intEmployeeDeductionDataViewRowIndex]["TOTAL"]))
                {
                    this.pvtEmployeeDeductionDataView[intEmployeeDeductionDataViewRowIndex]["TOTAL"] = dblUIFAmount;
                }
            }
                
            Calculate_Deductions();
        }
     
        private void Calculate_Deductions()
        {

            double dblTotal = 0;
            double dblTotalBF = 0;
            double dblDeductions = 0;
            double dblDeductionsTotal = 0;

            for (int intRow = 0; intRow < pvtEmployeeDeductionDataView.Count; intRow++)
            {
                //Errol to Fix
                dblTotal += Convert.ToDouble(this.dgvDeductionsDataGridView[pvtintDeductionCurrentCol,intRow ].Value);
                dblTotalBF += Convert.ToDouble(pvtEmployeeDeductionDataView[intRow]["TOTAL_YTD_BF"]);
            }

            this.dgvDeductionsTotalDataGridView[2,0].Value = dblTotal.ToString("#######0.00");
            this.dgvDeductionsTotalDataGridView[3, 0].Value = Convert.ToDouble(dblTotalBF + Convert.ToDouble(dblTotal)).ToString("########0.00");
           
            dblDeductions = Convert.ToDouble(this.dgvDeductionsTotalDataGridView[2, 0].Value);
            dblDeductionsTotal = Convert.ToDouble(this.dgvDeductionsTotalDataGridView[2, 2].Value);

            dblTotal = dblDeductionsTotal - dblDeductions;

            this.dgvDeductionsTotalDataGridView[2, 1].Value = dblTotal.ToString("#######0.00");

            this.dgvDeductionsTotalDataGridView[3, 1].Value = Convert.ToDouble(Convert.ToDouble(this.dgvDeductionsTotalDataGridView[3, 2].Value) - Convert.ToDouble(this.dgvDeductionsTotalDataGridView[3, 0].Value)).ToString("########0.00");
        }

        private void Set_Deduction_Values()
        {
            bool blnAddDeduction;
            DateTime dtDate;
            int intEmployeeDeductionDataviewIndex = -1;

            for (int intRow = 0; intRow < this.dgvDeductionsDataGridView.Rows.Count; intRow++)
            {
                intEmployeeDeductionDataviewIndex = Convert.ToInt32(this.dgvDeductionsDataGridView[pvtintDeductionIndexCol, intRow].Value);

                if (pvtEmployeeDeductionDataView[intEmployeeDeductionDataviewIndex]["DEDUCTION_LOAN_TYPE_IND"].ToString() == "Y"
                    & this.chkCloseYes.Checked == true)
                {
                    continue;
                }

                //Tax/UIF
                if (Convert.ToDouble(pvtEmployeeDeductionDataView[intEmployeeDeductionDataviewIndex]["DEDUCTION_NO"]) == 1
                    | Convert.ToDouble(pvtEmployeeDeductionDataView[intEmployeeDeductionDataviewIndex]["DEDUCTION_NO"]) == 2)
                {
                    continue;
                }

                if (pvtEmployeeDeductionDataView[intEmployeeDeductionDataviewIndex]["DEDUCTION_TYPE_IND"].ToString() == "P")
                {
                    blnAddDeduction = true;

                    if (pvtEmployeeDeductionDataView[intEmployeeDeductionDataviewIndex]["DEDUCTION_PERIOD_IND"].ToString() == "M")
                    {
                        //Start day
                        dtDate = Convert.ToDateTime(pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["EMPLOYEE_LAST_RUNDATE"]).AddDays(1);

                        if (Convert.ToInt32(pvtEmployeeDeductionDataView[intEmployeeDeductionDataviewIndex]["DEDUCTION_DAY_VALUE"]) == 99)
                        {
                            //Set To Month End
                            dtDate = dtDate.AddMonths(1);
                            dtDate = new DateTime(dtDate.Year, dtDate.Month, 1).AddDays(-1);
                        }
                        else
                        {
                            if (Convert.ToInt32(pvtEmployeeDeductionDataView[intEmployeeDeductionDataviewIndex]["DEDUCTION_DAY_VALUE"]) != dtDate.Day)
                            {
                                if (Convert.ToInt32(pvtEmployeeDeductionDataView[intEmployeeDeductionDataviewIndex]["DEDUCTION_DAY_VALUE"]) < dtDate.Day)
                                {
                                    dtDate = dtDate.AddMonths(1);
                                }

                                dtDate = new DateTime(dtDate.Year, dtDate.Month, Convert.ToInt32(pvtEmployeeDeductionDataView[intEmployeeDeductionDataviewIndex]["DEDUCTION_DAY_VALUE"]));
                            }
                        }

                        if (dtDate > pvtdttWageRunDate)
                        {
                            blnAddDeduction = false;
                        }
                    }
                    else
                    {
                        if (pvtEmployeeDeductionDataView[intEmployeeDeductionDataviewIndex]["DEDUCTION_PERIOD_IND"].ToString() == "T"
                            | pvtEmployeeDeductionDataView[intEmployeeDeductionDataviewIndex]["DEDUCTION_PERIOD_IND"].ToString() == "F")
                        {
                            //Errol To Add Code for 2 / 4 Weeks
                        }
                    }

                    if (blnAddDeduction == true)
                    {
                        Calculate_Deduction_Values(intRow,intEmployeeDeductionDataviewIndex);
                    }
                }
            }
        }

        private void Calculate_Deduction_Values(int DeductionsDataGridViewRowIndex,int intDeductionIndex)
        {
            int intEarningRow = 0;
            int intDeductionEarningRow = 0;
            double dblTotal = 0;
            double dblCalcTotal = 0;

            for (int intRow = 0; intRow < this.dgvEarningsDataGridView.Rows.Count; intRow++)
            {
                intEarningRow = Convert.ToInt32(this.dgvEarningsDataGridView[pvtintEarningIndexCol, intRow].Value);

                pvtobjKey3[0] = pvtEmployeeDeductionDataView[intDeductionIndex]["DEDUCTION_NO"].ToString();
                pvtobjKey3[1] = pvtEmployeeDeductionDataView[intDeductionIndex]["DEDUCTION_SUB_ACCOUNT_NO"].ToString();
                pvtobjKey3[2] = pvtEmployeeEarningDataView[intEarningRow]["EARNING_NO"].ToString();

                intDeductionEarningRow = pvtDeductionEarningPercentageDataView.Find(pvtobjKey3);

                if (intDeductionEarningRow != -1)
                {
                    dblTotal += Convert.ToDouble(this.dgvEarningsDataGridView[pvtintEarningCurrentCol, intRow].Value);
                }
            }

            for (int intRow = 0; intRow < this.dgvEarningsOtherDataGridView.Rows.Count; intRow++)
            {
                intEarningRow = Convert.ToInt32(this.dgvEarningsOtherDataGridView[pvtintEarningOtherIndexCol, intRow].Value);

                pvtobjKey3[0] = pvtEmployeeDeductionDataView[intDeductionIndex]["DEDUCTION_NO"].ToString();
                pvtobjKey3[1] = pvtEmployeeDeductionDataView[intDeductionIndex]["DEDUCTION_SUB_ACCOUNT_NO"].ToString();
                pvtobjKey3[2] = pvtEmployeeEarningDataView[intEarningRow]["EARNING_NO"].ToString();

                intDeductionEarningRow = pvtDeductionEarningPercentageDataView.Find(pvtobjKey3);

                if (intDeductionEarningRow != -1)
                {
                    dblTotal += Convert.ToDouble(this.dgvEarningsOtherDataGridView[pvtintEarningOtherCurrentCol, intRow].Value);
                }
            }

            if (pvtEmployeeDeductionDataView[intDeductionIndex]["DEDUCTION_LOAN_TYPE_IND"].ToString() == "Y")
            {
                dblCalcTotal = Math.Round(dblTotal * (Convert.ToDouble(pvtEmployeeDeductionDataView[intDeductionIndex]["DEDUCTION_VALUE"]) / 100), 2);

                if (Convert.ToDouble(pvtEmployeeDeductionDataView[intDeductionIndex]["DEDUCTION_MIN_VALUE"]) > dblCalcTotal)
                {
                    dblCalcTotal = Convert.ToDouble(pvtEmployeeDeductionDataView[intDeductionIndex]["DEDUCTION_MIN_VALUE"]);
                }

                if (Convert.ToDouble(pvtEmployeeDeductionDataView[intDeductionIndex]["DEDUCTION_MAX_VALUE"]) < dblCalcTotal
                    & Convert.ToDouble(pvtEmployeeDeductionDataView[intDeductionIndex]["DEDUCTION_MAX_VALUE"]) != 0)
                {
                    dblCalcTotal = Convert.ToDouble(pvtEmployeeDeductionDataView[intDeductionIndex]["DEDUCTION_MAX_VALUE"]);
                }

                if (Convert.ToDouble(pvtEmployeeDeductionDataView[intDeductionIndex]["TOTAL_OUTSTANDING_LOAN"]) > 0)
                {
                    //Errol
                    if (dblCalcTotal > Convert.ToDouble(pvtEmployeeDeductionDataView[intDeductionIndex]["TOTAL_OUTSTANDING_LOAN"]))
                    {
                        this.dgvDeductionsDataGridView[pvtintDeductionCurrentCol, DeductionsDataGridViewRowIndex].Value = Math.Round(Convert.ToDouble(pvtEmployeeDeductionDataView[intDeductionIndex]["TOTAL_OUTSTANDING_LOAN"]), 2).ToString("#########0.00"); 
                    }
                    else
                    {
                        this.dgvDeductionsDataGridView[pvtintDeductionCurrentCol, DeductionsDataGridViewRowIndex].Value = Math.Round(dblCalcTotal, 2).ToString("#########0.00");
                    }
                }
                else
                {
                    this.dgvDeductionsDataGridView[pvtintDeductionCurrentCol, DeductionsDataGridViewRowIndex].Value = "0.00";
                }
            }
            else
            {
                dblCalcTotal = Math.Round(dblTotal * (Convert.ToDouble(pvtEmployeeDeductionDataView[intDeductionIndex]["DEDUCTION_VALUE"]) / 100), 2);

                if (Convert.ToDouble(pvtEmployeeDeductionDataView[intDeductionIndex]["DEDUCTION_MIN_VALUE"]) > dblCalcTotal)
                {
                    dblCalcTotal = Convert.ToDouble(pvtEmployeeDeductionDataView[intDeductionIndex]["DEDUCTION_MIN_VALUE"]);
                }

                if (Convert.ToDouble(pvtEmployeeDeductionDataView[intDeductionIndex]["DEDUCTION_MAX_VALUE"]) < dblCalcTotal
                    & Convert.ToDouble(pvtEmployeeDeductionDataView[intDeductionIndex]["DEDUCTION_MAX_VALUE"]) != 0)
                {
                    dblCalcTotal = Convert.ToDouble(pvtEmployeeDeductionDataView[intDeductionIndex]["DEDUCTION_MAX_VALUE"]);
                }

                //Errol
                this.dgvDeductionsDataGridView[pvtintDeductionCurrentCol, DeductionsDataGridViewRowIndex].Value = dblCalcTotal.ToString("#########0.00");
            }

            //Errol
            this.dgvDeductionsDataGridView[pvtintDeductionYTDcfCol, DeductionsDataGridViewRowIndex].Value = Math.Round(Convert.ToDouble(this.dgvDeductionsDataGridView[pvtintDeductionCurrentCol, DeductionsDataGridViewRowIndex].Value) + Convert.ToDouble(pvtEmployeeDeductionDataView[intDeductionIndex]["TOTAL_YTD_BF"]), 2).ToString("#########0.00");

            if (Convert.ToDouble(this.dgvDeductionsDataGridView[pvtintDeductionCurrentCol, DeductionsDataGridViewRowIndex].Value) != Convert.ToDouble(pvtEmployeeDeductionDataView[intDeductionIndex]["TOTAL"]))
            {
                pvtEmployeeDeductionDataView[intDeductionIndex]["TOTAL"] = Convert.ToDouble(this.dgvDeductionsDataGridView[pvtintDeductionCurrentCol, DeductionsDataGridViewRowIndex].Value);
            }
        }

        public void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                pvtTempDataSet = null;
                pvtTempDataSet = new DataSet();
                DataTable myDataTable;

                if (this.rbnNormal.Checked == true)
                {
                    if (this.chkCloseYes.Checked == false)
                    {
                        pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["CLOSE_IND"] = "N";
                    }
                    else
                    {
                        pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["CLOSE_IND"] = "Y";
                    }

                    if (this.chkZeroEarningAndLeave.Checked == false)
                    {
                        pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["CLOSE_REMOVE_EARNING_AND_LEAVE_IND"] = "N";
                    }
                    else
                    {
                        pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["CLOSE_REMOVE_EARNING_AND_LEAVE_IND"] = "Y";
                    }

                    if (Convert.ToBoolean(this.dgvEmployeeDataGridView[pvtintEmployeePayslipCol, pvtintEmployeeDataGridViewRowInex].Value) == true)
                    {
                        if (this.dgvEmployeeDataGridView[pvtintEmployeePayslipCol, this.dgvEmployeeDataGridView.Rows.Count - 1].ReadOnly == true)
                        {
                            if (Convert.ToDouble(dgvEarningsOtherTotalDataGridView[2, 1].Value) == 0)
                            {
                                pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["PAYSLIP_IND"] = "N";
                            }
                            else
                            {
                                pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["PAYSLIP_IND"] = "Y";
                            }
                        }
                        else
                        {
                            pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["PAYSLIP_IND"] = "Y";
                        }
                    }
                    else
                    {
                        if (Convert.ToDouble(dgvEarningsOtherTotalDataGridView[2, 1].Value) == 0)
                        {
                            pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["PAYSLIP_IND"] = "N";
                        }
                        else
                        {
                            pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["PAYSLIP_IND"] = "Y";
                        }
                        //    pvtEmployeeDataView[intEmployeeDataViewIndex]["PAYSLIP_IND"] = "P";

                    }
                }
            
                myDataTable = this.pvtDataSet.Tables["Employee"].Clone();

                pvtTempDataSet.Tables.Add(myDataTable);

                if (this.rbnNormal.Checked == true)
                {
                    pvtTempDataSet.Tables["Employee"].ImportRow(pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex].Row);
                }
                else
                {
                    for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
                    {
                        if (pvtEmployeeDataView[intRow].Row.RowState == DataRowState.Modified)
                        {
                            pvtTempDataSet.Tables["Employee"].ImportRow(pvtEmployeeDataView[intRow].Row);
                        }
                    }

                    pvtEmployeeDeductionDataView = null;
                    pvtEmployeeDeductionDataView = new DataView(this.pvtDataSet.Tables["EmployeeDeduction"],
                        "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                        "DEDUCTION_NO,DEDUCTION_SUB_ACCOUNT_NO",
                        DataViewRowState.ModifiedCurrent);

                    pvtEmployeeEarningDataView = null;
                    pvtEmployeeEarningDataView = new DataView(this.pvtDataSet.Tables["EmployeeEarning"],
                        "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                        "",
                        DataViewRowState.ModifiedCurrent);
                }

                myDataTable = this.pvtDataSet.Tables["EmployeeEarning"].Clone();

                pvtTempDataSet.Tables.Add(myDataTable);

                for (int intRow = 0; intRow < pvtEmployeeEarningDataView.Count; intRow++)
                {
                    if (pvtEmployeeEarningDataView[intRow].Row.RowState == DataRowState.Modified)
                    {
                        pvtTempDataSet.Tables["EmployeeEarning"].ImportRow(pvtEmployeeEarningDataView[intRow].Row);
                    }
                }

                myDataTable = this.pvtDataSet.Tables["EmployeeDeduction"].Clone();

                pvtTempDataSet.Tables.Add(myDataTable);

                for (int intRow = 0; intRow < pvtEmployeeDeductionDataView.Count; intRow++)
                {
                    if (pvtEmployeeDeductionDataView[intRow].Row.RowState == DataRowState.Modified)
                    {
                        pvtTempDataSet.Tables["EmployeeDeduction"].ImportRow(pvtEmployeeDeductionDataView[intRow].Row);
                    }
                }

                //Compress DataSet
                pvtbytCompress = clsISUtilities.Compress_DataSet(pvtTempDataSet);
                
                object[] objParm = new object[3];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = pvtbytCompress;
                objParm[2] = pvtstrPayrollType;

                clsISUtilities.DynamicFunction("Update_Records", objParm);

                pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["PAY_TOTAL"] = Convert.ToDouble(dgvDeductionsTotalDataGridView[2,1].Value);
                
                this.pvtDataSet.AcceptChanges();

                btnCancel_Click(null, null);
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }
        
        private void btnTax_Click(object sender, System.EventArgs e)
        {
            //Goes Through Tax Module and Return Values
            Calculate_Earnings();

            frmTax = new frmTax(pvtDataSet, pvtstrPayrollType, pvtdttWageRunDate, pvtdttEmployeeBirthDate,
                   pvtdttStartTaxYear, pvtdttEndTaxYear, pvtdttEmployeeTaxStartDate, pvtdblNumberPeriodsInYear, pvtdttEmployeeLastRunDate, pvtstrMedicalAidInd,pvtintMedicalAidNumberDependents
                  ,pvtdblRetirementAnnuityAmount
                  ,pvtdblRetirementAnnuityTotal
                  ,pvtdblPensionFundAmount
                  ,pvtdblPensionFundTotal
                  ,pvtdblEarnedYTD
                  ,pvtdblOtherTaxableEarningsYTD
                  ,pvtdblTotalNormalEarnings
                  ,pvtdblTotalNormalEarningsAnnualised
                  ,pvtdblTotalEarnedAccumAnnualInitial
                  ,pvtdblTotalDeductions
                  ,pvtdblTaxTotal
                  ,pvtintAllTaxTableRow
                  ,pvtintEarningsTaxTableRow
                  ,pvtdblUifTotal
            );
          
            //Casual Worker
            if (pvtstrPayrollType == "S")
            {
                frmTax.pubstrTaxInfo[0] = "N";
            }
            else
            {
                frmTax.pubstrTaxInfo[0] = this.dgvEmployeeInfoDataGridView[3,0].Value.ToString();
            }
            //Age at Year End
            frmTax.pubstrTaxInfo[1] = this.dgvEmployeeInfoDataGridView[0, 0].Value.ToString();
            //Start of Tax Year
            if (Convert.ToInt32(pvtdttStartTaxYear.ToString("yyyyMMdd")) > Convert.ToInt32(pvtdttEmployeeTaxStartDate.ToString("yyyyMMdd")))
            {
                frmTax.pubstrTaxInfo[2] = pvtdttStartTaxYear.ToString("dd MMMM yyyy");
            }
            else
            {
                frmTax.pubstrTaxInfo[2] = pvtdttEmployeeTaxStartDate.ToString("dd MMMM yyyy");
            }
            //Days Worked
            frmTax.pubstrTaxInfo[3] = this.dgvEmployeeInfoDataGridView[1, 0].Value.ToString().Substring(0, this.dgvEmployeeInfoDataGridView[1, 0].Value.ToString().IndexOf("of"));
            //Days in Year
            frmTax.pubstrTaxInfo[4] = this.dgvEmployeeInfoDataGridView[1, 0].Value.ToString().Substring(this.dgvEmployeeInfoDataGridView[1, 0].Value.ToString().IndexOf("of") + 2).Trim();

            frmTax.pubstrTaxInfo[5] = this.lblEmployeeInfoHeader.Text.Replace("Parameters", "Tax Parameters");
            
            frmTax.lblInfo.Text =  this.dgvEmployeeDataGridView[pvtintEmployeeNameCol,pvtintEmployeeDataGridViewRowInex].Value.ToString() + " " +  this.dgvEmployeeDataGridView[pvtintEmployeeSurnameCol,pvtintEmployeeDataGridViewRowInex].Value.ToString() + " - Tax Parameters";

            frmTax.ShowDialog();
        }

        private void chkCloseYes_CheckedChanged(object sender, System.EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                int intDeductionRow;
                bool blnLoansPending = false;
                bool blnLeavePending = false;
                int intEarningRow = 0;

                pvtEmployeeNormalLeavePendingDataView = null;
                pvtEmployeeNormalLeavePendingDataView = new DataView(this.pvtDataSet.Tables["EmployeeNormalLeavePending"],
                    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND EMPLOYEE_NO = " + pvtintEmployeeNo.ToString(),
                    "",
                    DataViewRowState.CurrentRows);

                if (pvtEmployeeNormalLeavePendingDataView.Count > 0)
                {
                    blnLeavePending = true;
                }

                for (int intRow = 0; intRow < this.dgvEarningsDataGridView.Rows.Count; intRow++)
                {
                    intEarningRow = Convert.ToInt32(dgvEarningsDataGridView[pvtintEarningIndexCol, intRow].Value);

                    //Salaries
                    if (Convert.ToInt32(pvtEmployeeEarningDataView[intEarningRow]["EARNING_NO"]) == 1)
                    {
                        if (chkZeroEarningAndLeave.Checked == true)
                        {
                            this.dgvEarningsDataGridView[pvtintEarningCurrentCol, intRow].Value = Convert.ToDouble(0).ToString("####0.00");
                        }
                        else
                        {
                            this.dgvEarningsDataGridView[pvtintEarningCurrentCol, intRow].Value = Convert.ToDouble(pvtEmployeeEarningDataView[intEarningRow]["TOTAL_ORIGINAL"]).ToString("####0.00");
                        }

                        pvtintEarningsDataGridViewRowIndex = intRow;

                        EarningsDataGridView_Cell_Changed();
                    }
                    else
                    {
                        //Normal Leave - Must be Paid Out
                        if (Convert.ToInt32(pvtEmployeeEarningDataView[intEarningRow]["EARNING_NO"]) == 200)
                        {
                            if (chkCloseYes.Checked == true)
                            {
                                if (this.chkZeroEarningAndLeave.Checked == true)
                                {
                                    this.dgvEarningsDataGridView[pvtintEarningHoursCol, intRow].Value = Convert.ToDouble(Convert.ToDouble(pvtEmployeeEarningDataView[intEarningRow]["HOURS_DECIMAL_OTHER_VALUE_ZERO"]) + Convert.ToDouble(pvtEmployeeEarningDataView[intEarningRow]["HOURS_DECIMAL_ORIGINAL"])).ToString("####0.00");
                                }
                                else
                                {
                                    this.dgvEarningsDataGridView[pvtintEarningHoursCol, intRow].Value = Convert.ToDouble(Convert.ToDouble(pvtEmployeeEarningDataView[intEarningRow]["HOURS_DECIMAL_OTHER_VALUE"]) + Convert.ToDouble(pvtEmployeeEarningDataView[intEarningRow]["HOURS_DECIMAL_ORIGINAL"])).ToString("####0.00");
                                }
                            }
                            else
                            {
                                this.dgvEarningsDataGridView[pvtintEarningHoursCol, intRow].Value = Convert.ToDouble(pvtEmployeeEarningDataView[intEarningRow]["HOURS_DECIMAL_ORIGINAL"]).ToString("####0.00");
                            }

                            pvtintEarningsDataGridViewRowIndex = intRow;

                            EarningsDataGridView_Cell_Changed();
                        }
                    }
                }

                if (this.chkCloseYes.Checked == true)
                {
                    this.dgvEmployeeDataGridView[pvtintEmployeePayslipCol, pvtintEmployeeDataGridViewRowInex].Value = true;
                    dgvEmployeeDataGridView[2, pvtintEmployeeDataGridViewRowInex].Style = this.ClosedEmployeeDataGridViewCellStyle;
                }
                else
                {
                    //P=Pass
                    if (pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["PAYSLIP_IND"].ToString() == "Y"
                        | pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["PAYSLIP_IND"].ToString() == "P")
                    {
                        this.dgvEmployeeDataGridView[pvtintEmployeePayslipCol, pvtintEmployeeDataGridViewRowInex].Value = true;
                        dgvEmployeeDataGridView[2, pvtintEmployeeDataGridViewRowInex].Style = this.NormalDataGridViewCellStyle;
                    }
                    else
                    {
                        this.dgvEmployeeDataGridView[pvtintEmployeePayslipCol, pvtintEmployeeDataGridViewRowInex].Value = false;
                        dgvEmployeeDataGridView[2, pvtintEmployeeDataGridViewRowInex].Style = this.NoPayslipDataGridViewCellStyle;
                    }
                }

                int intEmployeeDeductionDataViewRowIndex = -1;

                for (int intRow = 0; intRow < this.dgvDeductionsDataGridView.Rows.Count; intRow++)
                {
                    intEmployeeDeductionDataViewRowIndex = Convert.ToInt32(this.dgvDeductionsDataGridView[pvtintDeductionIndexCol, intRow].Value);

                    if (Convert.ToInt32(pvtEmployeeDeductionDataView[intEmployeeDeductionDataViewRowIndex]["DEDUCTION_NO"]) != 2)
                    {
                        if (chkCloseYes.Checked == true)
                        {
                            this.dgvDeductionsDataGridView[0,intRow].Style = this.NormalDataGridViewCellStyle;

                            this.dgvDeductionsDataGridView.Rows[intRow].ReadOnly = false;
                        }
                        else
                        {
                            pvtobjKey3[0] = pvtEmployeeDeductionDataView[intEmployeeDeductionDataViewRowIndex]["PAY_CATEGORY_TYPE"].ToString();
                            pvtobjKey3[1] = pvtEmployeeDeductionDataView[intEmployeeDeductionDataViewRowIndex]["DEDUCTION_NO"].ToString();
                            pvtobjKey3[2] = pvtEmployeeDeductionDataView[intEmployeeDeductionDataViewRowIndex]["DEDUCTION_SUB_ACCOUNT_NO"].ToString();

                            intDeductionRow = pvtDeductionDescDataView.Find(pvtobjKey3);

                            if (Convert.ToInt32(pvtDeductionDescDataView[intDeductionRow]["DEDUCTION_NO"]) == 1
                                | Convert.ToInt32(pvtDeductionDescDataView[intDeductionRow]["DEDUCTION_NO"]) == 2
                                | pvtEmployeeDeductionDataView[intEmployeeDeductionDataViewRowIndex]["DEDUCTION_TYPE_IND"].ToString() != "U")
                            {
                                this.dgvDeductionsDataGridView[0,intRow].Style = this.ReadOnlyDataGridViewCellStyle;

                                this.dgvDeductionsDataGridView.Rows[intRow].ReadOnly = true;
                            }
                        }

                        //Not Garnashee Order
                        if (Convert.ToDouble(pvtEmployeeDeductionDataView[intEmployeeDeductionDataViewRowIndex]["TOTAL_OUTSTANDING_LOAN"]) != 0
                        & Convert.ToDouble(pvtEmployeeDeductionDataView[intEmployeeDeductionDataViewRowIndex]["DEDUCTION_NO"]) != 7)
                        {
                            if (chkCloseYes.Checked == true)
                            {
                                dgvDeductionsDataGridView[pvtintDeductionCurrentCol, intRow].Value = Convert.ToDouble(pvtEmployeeDeductionDataView[intEmployeeDeductionDataViewRowIndex]["TOTAL_OUTSTANDING_LOAN"]).ToString("#########0.00");
                            }
                            else
                            {
                                dgvDeductionsDataGridView[pvtintDeductionCurrentCol, intRow].Value = Convert.ToDouble(pvtEmployeeDeductionDataView[intEmployeeDeductionDataViewRowIndex]["TOTAL_ORIGINAL"]).ToString("#########0.00");
                            }

                            pvtintDeductionsDataGridViewRowIndex = intRow;

                            DeductionssDataGridView_Cell_Changed();
                        }

                        if (Convert.ToDouble(pvtEmployeeDeductionDataView[intEmployeeDeductionDataViewRowIndex]["TOTAL_OUTSTANDING_LOAN_PENDING"]) != 0)
                        {
                            blnLoansPending = true;
                        }
                    }
                }

                if (chkCloseYes.Checked == true)
                {
                    this.chkZeroEarningAndLeave.Enabled = true;

                    if (blnLeavePending == true
                        | blnLoansPending == true)
                    {
                        string strLeaveMessage = "";
                        string strLoansMessage = "";
                        string strHeader = "";

                        if (blnLeavePending == true)
                        {
                            strHeader = "Leave";
                            strLeaveMessage = "NB. Normal Leave has Pending Records.\n\n";
                        }

                        if (blnLoansPending == true)
                        {
                            if (strHeader != "")
                            {
                                strHeader = " and Loans";
                            }
                            else
                            {
                                strHeader = "Loans";
                            }

                            strLoansMessage = "NB. Loans has Pending Records.\n\n";
                        }

                        CustomMessageBox.Show(strLeaveMessage + strLoansMessage + "There are Records Pending for this Employee.\n\nYou can Ignore this Message (Records will be Deleted) or you can Cancel Run and Fix this situation.", strHeader, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    this.chkZeroEarningAndLeave.Enabled = false;
                    this.chkZeroEarningAndLeave.Checked = false;
                }
            }
        }

        private void Clear_Controls()
        {
            this.Clear_DataGridView(this.dgvEmployeeInfoDataGridView);
            this.Clear_DataGridView(this.dgvEmployeePayCategoryDataGridView);
            this.Clear_DataGridView(this.dgvEmployeeDataGridView);
            this.Clear_DataGridView(this.dgvEarningsDataGridView);
            this.Clear_DataGridView(this.dgvEarningsOtherDataGridView);
            this.Clear_DataGridView(this.dgvDeductionsDataGridView);

            for (int intCol = 1; intCol < this.dgvEarningsTotalDataGridView.Columns.Count; intCol++)
            {
                //Errol - 2015-03-17
                //if (intCol == pvtintEarningOtherPercentTaxCol - 1)
                if (intCol == pvtintEarningOtherIndexCol - 1)
                {
                    continue;
                }

                this.dgvEarningsTotalDataGridView[intCol,0].Value = "0.00";
            }

            for (int intCol = 1; intCol < this.dgvEarningsOtherTotalDataGridView.Columns.Count; intCol++)
            {
                //Errol - 2015-03-17
                //if (intCol == pvtintEarningOtherPercentTaxCol - 1)
                if (intCol == pvtintEarningOtherIndexCol - 1)
                {
                    continue;
                }

                this.dgvEarningsOtherTotalDataGridView[intCol, 0].Value = "0.00";
                this.dgvEarningsOtherTotalDataGridView[intCol, 1].Value = "0.00";
            }

            for (int intCol = 1; intCol < this.dgvDeductionsTotalDataGridView.Columns.Count; intCol++)
            {
                this.dgvDeductionsTotalDataGridView[intCol, 0].Value = "0.00";
                this.dgvDeductionsTotalDataGridView[intCol, 1].Value = "0.00";
                this.dgvDeductionsTotalDataGridView[intCol, 2].Value = "0.00";
            }

            this.chkCloseYes.Checked = false;
        }

        private void dgvPayrollTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPayrollTypeDataGridViewLoaded == true)
            {
                if (pvtintPayrollTypeDataGridViewRowInex != e.RowIndex)
                {
                    pvtintPayrollTypeDataGridViewRowInex = e.RowIndex;

                    //Stops Spreadsheet Firing Twice
                    if (pvtstrPayrollType != this.dgvPayrollTypeDataGridView[0, e.RowIndex].Value.ToString().Substring(0, 1))
                    {
                        //pvtblnEmployeeDataGridViewLoaded = false IS Set here to stop Event Firing (Bug??)
                        pvtblnEmployeeDataGridViewLoaded = false;
                        pvtintNoPayslipCount = 0;

                        pvtstrPayrollType = this.dgvPayrollTypeDataGridView[0, e.RowIndex].Value.ToString().Substring(0, 1);

                        if (pvtstrPayrollType == "W")
                        {
                            dgvEmployeeInfoDataGridView.Columns[4].Visible = false;

                            dgvEmployeeInfoDataGridView.Columns[2].Width += dgvEmployeeInfoDataGridView.Columns[4].Width;

                            this.chkZeroEarningAndLeave.Visible = false;
                        }
                        else
                        {
                            dgvEmployeeInfoDataGridView.Columns[2].Width = pvtintDateColWidth;

                            dgvEmployeeInfoDataGridView.Columns[4].Visible = true;

                            this.chkZeroEarningAndLeave.Visible = true;
                        }

                        pvtCompanyDataView = null;
                        pvtCompanyDataView = new DataView(this.pvtDataSet.Tables["Company"],
                            "PAY_CATEGORY_TYPE  = '" + pvtstrPayrollType + "'",
                            "",
                            DataViewRowState.CurrentRows);

                        if (pvtCompanyDataView.Count == 0)
                        {
                            object[] objParm = new object[2];
                            objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                            objParm[1] = pvtstrPayrollType;

                            pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Company_Records", objParm);

                            pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                            pvtDataSet.Merge(pvtTempDataSet);
                        }

                        this.rbnNone.Checked = true;
                        
                        this.picPayrollTypeLock.Visible = false;
                        this.picEmployeeLock.Visible = false;

                        if (pvtCompanyDataView.Count > 0)
                        {
                            Load_CurrentForm_Records();
                        }

                        if (pvtintNoPayslipCount != 0)
                        {
                            string strDesc = "Employee Exists";

                            if (pvtintNoPayslipCount > 1)
                            {
                                strDesc = "Employees Exist";
                            }

                            CustomMessageBox.Show(pvtintNoPayslipCount.ToString() + " " + strDesc + " with NO Earnings.\n\nEmployees will NOT get a Payslip unless\nyou change their Payslip status.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }

        private void dgvEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnEmployeeDataGridViewLoaded == true)
            {
                if (pvtintEmployeeDataGridViewRowInex != e.RowIndex)
                {
                    pvtintEmployeeDataGridViewRowInex = e.RowIndex;

                    pvtintCurrEmployeeRecordIndex = Convert.ToInt32(this.dgvEmployeeDataGridView[pvtintEmployeeIndexCol, e.RowIndex].Value);

                    pvtintEmployeeNo = Convert.ToInt32(pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["EMPLOYEE_NO"]);

                    Load_Employee();
                }
            }
        }

        private void dgvEarningsDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvEarningsDataGridView.Rows.Count > 0
                & this.pvtblnEarningDataGridViewLoaded == true)
            {
            }
        }

        private void dgvEarningsOtherDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvEarningsOtherDataGridView.Rows.Count > 0
                & this.pvtblnEarningOtherDataGridViewLoaded == true)
            {
            }
        }

        private void dgvDeductionsDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvDeductionsDataGridView.Rows.Count > 0
            & pvtblnDeductionDataGridViewLoaded == true)
            {
            }
        }

        private void dgvEarningsOtherDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox)
            {
                e.Control.KeyPress -= new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
                e.Control.KeyPress += new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
            }
        }

        private void dgvEarningsOtherDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int intEarningRow = Convert.ToInt32(this.dgvEarningsOtherDataGridView[pvtintEarningOtherIndexCol, e.RowIndex].Value);

            double dblCurrentValue = 0;

            if (this.dgvEarningsOtherDataGridView[pvtintEarningOtherCurrentCol, e.RowIndex].Value == null)
            {
            }
            else
            {
                if (this.dgvEarningsOtherDataGridView[pvtintEarningOtherCurrentCol, e.RowIndex].Value.ToString().Replace(".", "").Trim() == "")
                {
                }
                else
                {
                    dblCurrentValue = Convert.ToDouble(this.dgvEarningsOtherDataGridView[pvtintEarningOtherCurrentCol, e.RowIndex].Value);
                }
            }

            this.dgvEarningsOtherDataGridView[pvtintEarningOtherCurrentCol, e.RowIndex].Value = dblCurrentValue.ToString("#######0.00");

            this.dgvEarningsOtherDataGridView[pvtintEarningOtherYTDcfCol, e.RowIndex].Value = Convert.ToDouble(dblCurrentValue + Convert.ToDouble(this.pvtEmployeeEarningDataView[intEarningRow]["TOTAL_YTD_BF"])).ToString("#######0.00");
                      
            if (dblCurrentValue != Convert.ToDouble(this.pvtEmployeeEarningDataView[intEarningRow]["TOTAL"]))
            {
                this.pvtEmployeeEarningDataView[intEarningRow]["TOTAL"] = dblCurrentValue;
            }

            if (Convert.ToInt32(this.pvtEmployeeEarningDataView[intEarningRow]["EARNING_NO"]) == 7)
            {
                Set_Deduction_Values();
            }

            Calculate_Earnings();
        }

        private void dgvEarningsOtherDataGridView_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (e.RowIndex > -1
                & e.ColumnIndex > -1)
                {
                    if (e.ColumnIndex == pvtintEarningOtherCurrentCol)
                    {
                        this.Cursor = Cursors.Default;
                    }
                    else
                    {
                        this.Cursor = Cursors.No;
                    }
                }
                else
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void dgvEarningsOtherDataGridView_MouseLeave(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void dgvDeductionsDataGridView_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (e.RowIndex > -1
                & e.ColumnIndex > -1)
                {
                    if (e.ColumnIndex == pvtintDeductionCurrentCol)
                    {
                        if (dgvDeductionsDataGridView[e.ColumnIndex, e.RowIndex].ReadOnly == false)
                        {
                            this.Cursor = Cursors.Default;
                        }
                        else
                        {
                            this.Cursor = Cursors.No;
                        }
                    }
                    else
                    {
                        this.Cursor = Cursors.No;
                    }
                }
                else
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void dgvDeductionsDataGridView_MouseLeave(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void dgvDeductionsDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox)
            {
                e.Control.KeyPress -= new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
                e.Control.KeyPress += new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
            }
        }

        private void dgvDeductionsDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            pvtintDeductionsDataGridViewRowIndex = e.RowIndex;

            DeductionssDataGridView_Cell_Changed();
        }

        private void DeductionssDataGridView_Cell_Changed()
        {
            int intDeductionRow = Convert.ToInt32(this.dgvDeductionsDataGridView[pvtintDeductionIndexCol, pvtintDeductionsDataGridViewRowIndex].Value);

            double dblCurrentValue = 0;
            
            if (this.dgvDeductionsDataGridView[pvtintDeductionCurrentCol, pvtintDeductionsDataGridViewRowIndex].Value.ToString().Replace(".", "").Trim() != "")
            {
                dblCurrentValue = Convert.ToDouble(this.dgvDeductionsDataGridView[pvtintDeductionCurrentCol, pvtintDeductionsDataGridViewRowIndex].Value);
            }

            if (dgvDeductionsDataGridView[pvtintDeductionOutstandingLoanCol, pvtintDeductionsDataGridViewRowIndex].Value != null)
            {
                if (dgvDeductionsDataGridView[pvtintDeductionOutstandingLoanCol, pvtintDeductionsDataGridViewRowIndex].Value.ToString() != "")
                {
                    if (dblCurrentValue > Convert.ToDouble(dgvDeductionsDataGridView[pvtintDeductionOutstandingLoanCol, pvtintDeductionsDataGridViewRowIndex].Value))
                    {
                        CustomMessageBox.Show("Amount Cannot Exceed Loan Outstanding Amount\nValue Set to Zero.",this.Text,MessageBoxButtons.OK,MessageBoxIcon.Error);
                        dblCurrentValue = 0;
                    }
                }
            }

            this.dgvDeductionsDataGridView[pvtintDeductionCurrentCol, pvtintDeductionsDataGridViewRowIndex].Value = dblCurrentValue.ToString("#######0.00");

            this.dgvDeductionsDataGridView[pvtintDeductionYTDcfCol, pvtintDeductionsDataGridViewRowIndex].Value = Convert.ToDouble(Convert.ToDouble(pvtEmployeeDeductionDataView[intDeductionRow]["TOTAL_YTD_BF"]) + dblCurrentValue).ToString("#######0.00");

            if (dblCurrentValue != Convert.ToDouble(this.pvtEmployeeDeductionDataView[intDeductionRow]["TOTAL"]))
            {
                this.pvtEmployeeDeductionDataView[intDeductionRow]["TOTAL"] = dblCurrentValue;
            }

            Calculate_Deductions();
        }

        private void dgvEarningsDataGridView_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (e.RowIndex > -1
                & e.ColumnIndex > -1)
                {
                    if (e.ColumnIndex == pvtintEarningHoursCol
                    | e.ColumnIndex == pvtintEarningCurrentCol)
                    {
                        if (dgvEarningsDataGridView[e.ColumnIndex, e.RowIndex].ReadOnly == false)
                        {
                            this.Cursor = Cursors.Default;
                        }
                        else
                        {
                            this.Cursor = Cursors.No;
                        }
                    }
                    else
                    {
                        this.Cursor = Cursors.No;
                    }
                }
                else
                {
                    this.Cursor = Cursors.No;
                }
            }
        }

        private void dgvEarningsDataGridView_MouseLeave(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void dgvEarningsDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox)
            {
                e.Control.KeyPress -= new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
                e.Control.KeyPress -= new KeyPressEventHandler(clsISUtilities.Numeric_KeyPress);

                if (pvtstrEarningType == "X")
                {
                    e.Control.KeyPress += new KeyPressEventHandler(clsISUtilities.Numeric_KeyPress);
                }
                else
                {
                    e.Control.KeyPress += new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
                }
            }
        }

        private void dgvEarningsDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            pvtintEarningsDataGridViewRowIndex = e.RowIndex;

            EarningsDataGridView_Cell_Changed();
        }

        private void EarningsDataGridView_Cell_Changed()
        {
            double dblValue = 0;
            double dblValueBF = 0;
           
            int intEmployeeEarningDataViewIndex = Convert.ToInt32(dgvEarningsDataGridView[pvtintEarningIndexCol, pvtintEarningsDataGridViewRowIndex].Value);

            //2017-02-21          
            pvtobjKey2[0] = pvtEmployeeEarningDataView[intEmployeeEarningDataViewIndex]["PAY_CATEGORY_TYPE"].ToString();
            pvtobjKey2[1] = pvtEmployeeEarningDataView[intEmployeeEarningDataViewIndex]["EARNING_NO"].ToString();

            int intEarningDescRow = pvtEarningDescDataView.Find(pvtobjKey2);

            //Wage Run
            if ((Convert.ToInt32(pvtEmployeeEarningDataView[intEmployeeEarningDataViewIndex]["EARNING_NO"]) > 1
                & Convert.ToInt32(pvtEmployeeEarningDataView[intEmployeeEarningDataViewIndex]["EARNING_NO"]) < 10)
                | pvtEmployeeEarningDataView[intEmployeeEarningDataViewIndex]["EARNING_TYPE_IND"].ToString() == "X"
                | Convert.ToInt32(pvtEmployeeEarningDataView[intEmployeeEarningDataViewIndex]["EARNING_NO"]) > 199)
            {
                dblValue = Convert.ToDouble(this.dgvEarningsDataGridView[pvtintEarningHoursCol, pvtintEarningsDataGridViewRowIndex].Value);
            }
            else
            {
                dblValue = Convert.ToDouble(this.dgvEarningsDataGridView[pvtintEarningCurrentCol, pvtintEarningsDataGridViewRowIndex].Value);
            }

            dblValueBF = Convert.ToDouble(pvtEmployeeEarningDataView[intEmployeeEarningDataViewIndex]["TOTAL_YTD_BF"]);
            
            pvtobjKey2[1] = pvtEmployeeEarningDataView[intEmployeeEarningDataViewIndex]["PAY_CATEGORY_NO"].ToString();
            
            pvtintEmployeePayCategoryRow = pvtEmployeePayCategoryDataView.Find(pvtobjKey2);

            switch (Convert.ToInt32(pvtEmployeeEarningDataView[intEmployeeEarningDataViewIndex]["EARNING_NO"]))
            {
                case 2:

                    this.dgvEarningsDataGridView[pvtintEarningCurrentCol, pvtintEarningsDataGridViewRowIndex].Value = Math.Round(dblValue * Convert.ToDouble(pvtEmployeePayCategoryDataView[pvtintEmployeePayCategoryRow]["HOURLY_RATE"]), 2).ToString("########0.00");
                    this.dgvEarningsDataGridView[pvtintEarningYTDcfCol, pvtintEarningsDataGridViewRowIndex].Value = Math.Round(Convert.ToDouble(this.dgvEarningsDataGridView[pvtintEarningCurrentCol, pvtintEarningsDataGridViewRowIndex].Value) + dblValueBF, 2).ToString("########0.00");

                    Set_Deduction_Values();

                    break;

                case 3:

                    this.dgvEarningsDataGridView[pvtintEarningCurrentCol, pvtintEarningsDataGridViewRowIndex].Value = Math.Round(dblValue * Convert.ToDouble(pvtEmployeePayCategoryDataView[pvtintEmployeePayCategoryRow]["HOURLY_RATE"]) * (Convert.ToDouble(pvtCompanyDataView[0]["OVERTIME1_RATE"])), 2).ToString("########0.00");
                    this.dgvEarningsDataGridView[pvtintEarningYTDcfCol, pvtintEarningsDataGridViewRowIndex].Value = Math.Round(Convert.ToDouble(this.dgvEarningsDataGridView[pvtintEarningCurrentCol, pvtintEarningsDataGridViewRowIndex].Value) + dblValueBF, 2).ToString("########0.00");

                    Set_Deduction_Values();

                    break;

                case 4:

                    this.dgvEarningsDataGridView[pvtintEarningCurrentCol, pvtintEarningsDataGridViewRowIndex].Value = Math.Round(dblValue * Convert.ToDouble(pvtEmployeePayCategoryDataView[pvtintEmployeePayCategoryRow]["HOURLY_RATE"]) * (Convert.ToDouble(pvtCompanyDataView[0]["OVERTIME2_RATE"])), 2).ToString("########0.00");
                    this.dgvEarningsDataGridView[pvtintEarningYTDcfCol, pvtintEarningsDataGridViewRowIndex].Value = Math.Round(Convert.ToDouble(this.dgvEarningsDataGridView[pvtintEarningCurrentCol, pvtintEarningsDataGridViewRowIndex].Value) + dblValueBF, 2).ToString("########0.00");

                    Set_Deduction_Values();

                    break;

                case 5:

                    this.dgvEarningsDataGridView[pvtintEarningCurrentCol, pvtintEarningsDataGridViewRowIndex].Value = Math.Round(dblValue * Convert.ToDouble(pvtEmployeePayCategoryDataView[pvtintEmployeePayCategoryRow]["HOURLY_RATE"]) * (Convert.ToDouble(pvtCompanyDataView[0]["OVERTIME3_RATE"])), 2).ToString("########0.00");
                    this.dgvEarningsDataGridView[pvtintEarningYTDcfCol, pvtintEarningsDataGridViewRowIndex].Value = Math.Round(Convert.ToDouble(this.dgvEarningsDataGridView[pvtintEarningCurrentCol, pvtintEarningsDataGridViewRowIndex].Value) + dblValueBF, 2).ToString("########0.00");

                    Set_Deduction_Values();

                    break;

                case 8:

                    this.dgvEarningsDataGridView[pvtintEarningCurrentCol, pvtintEarningsDataGridViewRowIndex].Value = Math.Round(dblValue * Convert.ToDouble(pvtEmployeePayCategoryDataView[pvtintEmployeePayCategoryRow]["HOURLY_RATE"]), 2).ToString("########0.00");
                    this.dgvEarningsDataGridView[pvtintEarningYTDcfCol, pvtintEarningsDataGridViewRowIndex].Value = Math.Round(Convert.ToDouble(this.dgvEarningsDataGridView[pvtintEarningCurrentCol, pvtintEarningsDataGridViewRowIndex].Value) + dblValueBF, 2).ToString("########0.00");

                    Set_Deduction_Values();

                    break;

                case 9:

                    this.dgvEarningsDataGridView[pvtintEarningCurrentCol, pvtintEarningsDataGridViewRowIndex].Value = Math.Round(dblValue * Convert.ToDouble(pvtEmployeePayCategoryDataView[pvtintEmployeePayCategoryRow]["HOURLY_RATE"]) * (Convert.ToDouble(pvtCompanyDataView[0]["PAIDHOLIDAY_RATE"])), 2).ToString("########0.00");
                    this.dgvEarningsDataGridView[pvtintEarningYTDcfCol, pvtintEarningsDataGridViewRowIndex].Value = Math.Round(Convert.ToDouble(this.dgvEarningsDataGridView[pvtintEarningCurrentCol, pvtintEarningsDataGridViewRowIndex].Value) + dblValueBF, 2).ToString("########0.00");

                    Set_Deduction_Values();

                    break;

                default:

                    //Leave
                    if (Convert.ToInt32(pvtEmployeeEarningDataView[intEmployeeEarningDataViewIndex]["EARNING_NO"]) > 199)
                    {
                        this.dgvEarningsDataGridView[pvtintEarningCurrentCol, pvtintEarningsDataGridViewRowIndex].Value = Math.Round(dblValue * Convert.ToDouble(pvtEmployeePayCategoryDataView[pvtintEmployeePayCategoryRow]["HOURLY_RATE"]) * (Convert.ToDouble(pvtEarningDescDataView[intEarningDescRow]["LEAVE_PERCENTAGE"]) / 100), 2).ToString("########0.00");
                        this.dgvEarningsDataGridView[pvtintEarningYTDcfCol, pvtintEarningsDataGridViewRowIndex].Value = Math.Round(Convert.ToDouble(this.dgvEarningsDataGridView[pvtintEarningCurrentCol, pvtintEarningsDataGridViewRowIndex].Value) + dblValueBF, 2).ToString("########0.00");

                        Set_Deduction_Values();
                    }
                    else
                    {
                        if (pvtEmployeeEarningDataView[intEmployeeEarningDataViewIndex]["EARNING_TYPE_IND"].ToString() == "X")
                        {
                            this.dgvEarningsDataGridView[pvtintEarningCurrentCol, pvtintEarningsDataGridViewRowIndex].Value = Math.Round(dblValue * Convert.ToDouble(pvtEmployeeEarningDataView[intEmployeeEarningDataViewIndex]["HOURS_DECIMAL_OTHER_VALUE"]), 2).ToString("########0.00");
                            this.dgvEarningsDataGridView[pvtintEarningYTDcfCol, pvtintEarningsDataGridViewRowIndex].Value = Math.Round(Convert.ToDouble(this.dgvEarningsDataGridView[pvtintEarningCurrentCol, pvtintEarningsDataGridViewRowIndex].Value) + dblValueBF, 2).ToString("########0.00");
                        }
                        else
                        {
                            this.dgvEarningsDataGridView[pvtintEarningYTDcfCol, pvtintEarningsDataGridViewRowIndex].Value = Math.Round(Convert.ToDouble(this.dgvEarningsDataGridView[pvtintEarningCurrentCol, pvtintEarningsDataGridViewRowIndex].Value) + dblValueBF, 2).ToString("########0.00");
                        }

                        Set_Deduction_Values();
                    }

                    break;
            }

            if (Convert.ToInt32(pvtEmployeeEarningDataView[intEmployeeEarningDataViewIndex]["EARNING_NO"]) == 1
            && this.chkCloseYes.Checked == true)    
            {
                if (this.chkZeroEarningAndLeave.Checked == true)
                {
                    if (Convert.ToDouble(this.dgvEarningsDataGridView[pvtintEarningCurrentCol, pvtintEarningsDataGridViewRowIndex].Value) != Convert.ToDouble(this.pvtEmployeeEarningDataView[intEmployeeEarningDataViewIndex]["TOTAL"]))
                    {
                        this.pvtEmployeeEarningDataView[intEmployeeEarningDataViewIndex]["TOTAL"] = 0;
                    }
                }
                else
                {
                    if (Convert.ToDouble(this.dgvEarningsDataGridView[pvtintEarningCurrentCol, pvtintEarningsDataGridViewRowIndex].Value) != Convert.ToDouble(this.pvtEmployeeEarningDataView[intEmployeeEarningDataViewIndex]["TOTAL"]))
                    {
                        this.pvtEmployeeEarningDataView[intEmployeeEarningDataViewIndex]["TOTAL"] = Convert.ToDouble(this.dgvEarningsDataGridView[pvtintEarningCurrentCol, pvtintEarningsDataGridViewRowIndex].Value);
                    }
                }
            }
            else
            {
                if (Convert.ToDouble(this.dgvEarningsDataGridView[pvtintEarningCurrentCol, pvtintEarningsDataGridViewRowIndex].Value) != Convert.ToDouble(this.pvtEmployeeEarningDataView[intEmployeeEarningDataViewIndex]["TOTAL"]))
                {
                    this.pvtEmployeeEarningDataView[intEmployeeEarningDataViewIndex]["HOURS_DECIMAL"] = Convert.ToDouble(this.dgvEarningsDataGridView[pvtintEarningHoursCol, pvtintEarningsDataGridViewRowIndex].Value);
                    this.pvtEmployeeEarningDataView[intEmployeeEarningDataViewIndex]["TOTAL"] = Convert.ToDouble(this.dgvEarningsDataGridView[pvtintEarningCurrentCol, pvtintEarningsDataGridViewRowIndex].Value);
                }
            }

            Calculate_Earnings();
        }

        private void dgvEarningsDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            int intEarningRow = Convert.ToInt32(this.dgvEarningsDataGridView[pvtintEarningIndexCol, e.RowIndex].Value);

            pvtstrEarningType = this.pvtEmployeeEarningDataView[intEarningRow]["EARNING_TYPE_IND"].ToString();
        }

        private void dgvEmployeeDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (e.RowIndex > -1
                &  (e.ColumnIndex == pvtintEmployeePayslipCol
                | e.ColumnIndex == pvtintEmployeePublicHolidayOrDoubleChequeCol))
                {
                    int intEmployeeDataViewIndex = Convert.ToInt32(this.dgvEmployeeDataGridView[pvtintEmployeeIndexCol, e.RowIndex].Value);

                    if (pvtstrPayrollType == "S")
                    {
                        if (e.ColumnIndex == pvtintEmployeePublicHolidayOrDoubleChequeCol)
                        {
                            if (this.dgvEmployeeDataGridView[e.ColumnIndex, e.RowIndex].ReadOnly == true)
                            {
                                return;
                            }
                            else
                            {
                                if (Convert.ToInt32(pvtEmployeeDataView[intEmployeeDataViewIndex]["EXTRA_CHEQUES_CURRENT"]) == 0)
                                {
                                    pvtEmployeeDataView[intEmployeeDataViewIndex]["EXTRA_CHEQUES_CURRENT"] = 1;
                                }
                                else
                                {
                                    pvtEmployeeDataView[intEmployeeDataViewIndex]["EXTRA_CHEQUES_CURRENT"] = 0;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (e.ColumnIndex == pvtintEmployeePayslipCol)
                        {
                            if (pvtEmployeeDataView[intEmployeeDataViewIndex]["PAYSLIP_IND"].ToString() != "Y")
                            {
                                //if (pvtEmployeeDataView[intEmployeeDataViewIndex]["PAYSLIP_IND"].ToString() == "P")
                                //{
                                //    pvtEmployeeDataView[intEmployeeDataViewIndex]["PAYSLIP_IND"] = "N";
                                //}
                                //else
                                //{
                                //    pvtEmployeeDataView[intEmployeeDataViewIndex]["PAYSLIP_IND"] = "P";
                                //}
                            }
                        }
                    }

                    if (e.ColumnIndex == pvtintEmployeePublicHolidayOrDoubleChequeCol)
                    {
                        int intEarningRow = 0;

                        for (int intRow = 0; intRow < this.dgvEarningsDataGridView.Rows.Count; intRow++)
                        {
                            intEarningRow = Convert.ToInt32(this.dgvEarningsDataGridView[pvtintEarningIndexCol, intRow].Value);

                            if (pvtstrPayrollType == "W")
                            {
                                if (Convert.ToInt32(pvtEmployeeEarningDataView[intEarningRow]["EARNING_NO"]) == 8)
                                {
                                    if (Convert.ToDouble(pvtEmployeeDataView[intEmployeeDataViewIndex]["PUBLIC_HOLIDAY_HOURS_DECIMAL"]) == 0)
                                    {
                                        pvtEmployeeDataView[intEmployeeDataViewIndex]["PUBLIC_HOLIDAY_HOURS_DECIMAL"] = Convert.ToDouble(pvtEmployeeEarningDataView[intEarningRow]["HOURS_DECIMAL_OTHER_VALUE"]);

                                        this.dgvEarningsDataGridView[pvtintEarningHoursCol, intRow].Value = Convert.ToDouble(pvtEmployeeEarningDataView[intEarningRow]["HOURS_DECIMAL_OTHER_VALUE"]).ToString("#########0.00");

                                        this.dgvEarningsDataGridView[0,intRow].Style = this.PublicHolidayDoubleChequeDataGridViewCellStyle;
                                        this.dgvEmployeeDataGridView[0,e.RowIndex].Style = this.PublicHolidayDoubleChequeDataGridViewCellStyle;
                                    }
                                    else
                                    {
                                        pvtEmployeeDataView[intEmployeeDataViewIndex]["PUBLIC_HOLIDAY_HOURS_DECIMAL"] = 0;

                                        this.dgvEarningsDataGridView[pvtintEarningHoursCol, intRow].Value = "0.00";
                                        this.dgvEarningsDataGridView[0,intRow].Style = this.ReadOnlyDataGridViewCellStyle;

                                        this.dgvEmployeeDataGridView[0,e.RowIndex].Style = this.NormalDataGridViewCellStyle;
                                    }

                                    pvtintEarningsDataGridViewRowIndex = intRow;

                                    EarningsDataGridView_Cell_Changed();

                                    break;
                                }
                            }
                            else
                            {
                                //Must Be Linked
                                if (pvtEmployeeEarningDataView[intEarningRow]["EARNING_NOT_LINKED"].ToString() != "Y")
                                {
                                    if (Convert.ToInt32(pvtEmployeeEarningDataView[intEarningRow]["EARNING_NO"]) == 1)
                                    {
                                        if (Convert.ToInt32(pvtEmployeeDataView[intEmployeeDataViewIndex]["EXTRA_CHEQUES_CURRENT"]) == 1)
                                        {
                                            this.dgvEarningsDataGridView[pvtintEarningCurrentCol, intRow].Value = Convert.ToDouble(2 * Convert.ToDouble(pvtEmployeeEarningDataView[intEarningRow]["TOTAL_DOUBLE"])).ToString("########0.00");
        
                                            this.dgvEarningsDataGridView[0,intRow].Style = this.PublicHolidayDoubleChequeDataGridViewCellStyle;

                                            this.dgvEmployeeDataGridView[0,e.RowIndex].Style = this.PublicHolidayDoubleChequeDataGridViewCellStyle;
                                        }
                                        else
                                        {
                                            this.dgvEarningsDataGridView[pvtintEarningCurrentCol, intRow].Value = Convert.ToDouble(pvtEmployeeEarningDataView[intEarningRow]["TOTAL_DOUBLE"]).ToString("########0.00");

                                            this.dgvEarningsDataGridView[0,intRow].Style = this.ReadOnlyDataGridViewCellStyle;

                                            this.dgvEmployeeDataGridView[0,e.RowIndex].Style = this.NormalDataGridViewCellStyle;
                                        }

                                        pvtintEarningsDataGridViewRowIndex = intRow;

                                        EarningsDataGridView_Cell_Changed();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void dgvEmployeeDataGridView_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (e.RowIndex > -1
                & e.ColumnIndex > -1)
                {
                    if (e.ColumnIndex == pvtintEmployeePayslipCol
                    | e.ColumnIndex == pvtintEmployeePublicHolidayOrDoubleChequeCol)
                    {
                        if (dgvEmployeeDataGridView[e.ColumnIndex, e.RowIndex].ReadOnly == false)
                        {
                            this.Cursor = Cursors.Default;
                        }
                        else
                        {
                            this.Cursor = Cursors.No;
                        }
                    }
                    else
                    {
                        this.Cursor = Cursors.No;
                    }
                }
                else
                {
                    this.Cursor = Cursors.No;
                }
            }
        }

        private void dgvEmployeeDataGridView_MouseLeave(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void rbn_Filter_Click(object sender, EventArgs e)
        {
            RadioButton myRadioButton = (RadioButton)sender;

            this.pvtintEmployeeNo = -1;

            Load_CurrentForm_Records();
        }

        private void chkZeroEarningAndLeave_CheckStateChanged(object sender, EventArgs e)
        {
            chkCloseYes_CheckedChanged(sender, e);
        }
        
        private void rbnCloseEmployee_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
