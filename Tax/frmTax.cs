using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace InteractPayroll
{
    public partial class frmTax : Form
    {
        System.Windows.Forms.PaintEventArgs myPaintEventArgs;

        clsTax clsTax;

        private double pvtdblRebatePrimary = 0;

        private double pvtdblRebate65 = 0;
        private double pvtdblRebate65Age = 0;

        private double pvtdblRebate75 = 0;
        private double pvtdblRebate75Age = 0;

        private double pvtdblMedicalMajorAmount = 0;
        private double pvtdblMedicalMinorAmount = 0;

        private double pvtdblEmployeePortionOfYear = 0;
        private double pvtdblAgeAtTaxYearEnd = 0;
        private double pvtdblEmployeeAnnualisedFactor = 0;
        private double pvtdblEmployeeDaysWorked = 0;

        private double pvtdblPensionPercentage = 0;
        private double pvtdblPensionFixedAmount = 0;
        private double pvtdblPensionArrearFixedAmount = 0;

        private double pvtdblRetirementAnnuityPercentage = 0;
        private double pvtdblRetirementAnnuityFixedAmount = 0;
        private double pvtdblRetirementAnnuityArrearFixedAmount = 0;
        private double pvtdblRetirementAnnuityPensionFixedAmount = 0;

        private double pvtdblRetirementFundsPercentage = 0;
        private double pvtdblRetirementFundsMaxFixedAmount = 0;

        private double pvtdblAnnualUIFAmount = 0;
        private double pvtdblUIFPercentageAmount = 0;

        private double pvtdblRetirementFundsPercentageAmount = 0;
        private double pvtdblRetirementFundsLimitedAmount = 0;

        DataSet pvtDataSet;

        public string[] pubstrTaxInfo = new string[6];
        private string pvtstrPayrollType;

        private string[] pvtstrMonthDesc = new string[12] {"Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec","Jan","Feb"};

        DataGridViewCellStyle TotalDataGridViewCellStyle;
        DataGridViewCellStyle SubTotalDataGridViewCellStyle;
        DataGridViewCellStyle ControlColourDataGridViewCellStyle;
     
        public frmTax(DataSet parDataSet
                     ,string parstrPayrollType
                     ,DateTime parRunDate
                     ,DateTime pardttEmployeeBirthDate   
                     ,DateTime pardttStartTaxYear 
                     ,DateTime pardttEndTaxYear
                     ,DateTime pvtdttEmployeeTaxStartDate
                     ,Double pvtdblNumberPeriodsInYear
                     ,DateTime pvtdttEmployeeLastRunDate
                     ,string parstrMedicalAidInd
                     ,int parintMedicalAidNumberDependents
                     ,double[] pardblRetirementAnnuityAmount
                     ,double pardblRetirementAnnuityTotal
                     ,double[] pardblPensionFundAmount
                     ,double pardblPensionFundTotal
                     ,double pardblEarningsYTD
                     ,double pardblOtherTaxableEarningsYTD
                     ,double pardblTotalNormalEarnings
                     ,double pardblTotalNormalEarningsAnnualised
                     ,double pardblTotalEarnedAccumAnnualInitial
                     ,double pardblTotalDeductions
                     ,double[] pardblTaxTotal
                     ,int parintAllTaxTableRow
                     ,int parintEarningsTaxTableRow
                     ,double[] pardblUifTotal
                    )
        {
            InitializeComponent();
            
            TotalDataGridViewCellStyle = new DataGridViewCellStyle();
            TotalDataGridViewCellStyle.BackColor = Color.Black;
            TotalDataGridViewCellStyle.SelectionBackColor = Color.Black;
            TotalDataGridViewCellStyle.ForeColor = Color.White;

            SubTotalDataGridViewCellStyle = new DataGridViewCellStyle();
            SubTotalDataGridViewCellStyle.BackColor = SystemColors.ControlDark;
            SubTotalDataGridViewCellStyle.SelectionBackColor = SystemColors.ControlDark;

            ControlColourDataGridViewCellStyle = new DataGridViewCellStyle();
            ControlColourDataGridViewCellStyle.BackColor = SystemColors.Control;
            ControlColourDataGridViewCellStyle.SelectionBackColor = SystemColors.Control;

            pvtDataSet = parDataSet;

            if (pvtDataSet.Tables["UIF"].Rows.Count > 0)
            {
                pvtdblAnnualUIFAmount = Convert.ToDouble(pvtDataSet.Tables["UIF"].Rows[0]["UIF_YEAR_AMOUNT"]);
                pvtdblUIFPercentageAmount = Convert.ToDouble(pvtDataSet.Tables["UIF"].Rows[0]["UIF_PERCENTAGE"]);
            }

            if (pvtDataSet.Tables["TaxMedicalCap"].Rows.Count > 0)
            {
                pvtdblMedicalMajorAmount = Convert.ToDouble(pvtDataSet.Tables["TaxMedicalCap"].Rows[0]["MAJOR_AMOUNT"]);
                pvtdblMedicalMinorAmount = Convert.ToDouble(pvtDataSet.Tables["TaxMedicalCap"].Rows[0]["MINOR_AMOUNT"]);
            }

            int intRow = 0;

            for (intRow = 0; intRow < pvtDataSet.Tables["TaxDeduction"].Rows.Count; intRow++)
            {
                switch (pvtDataSet.Tables["TaxDeduction"].Rows[intRow]["TAX_DEDUCTION_NO"].ToString())
                {
                    case "1":

                        //Pension
                        pvtdblPensionPercentage = Convert.ToDouble(pvtDataSet.Tables["TaxDeduction"].Rows[intRow]["TAX_DEDUCTION_PERCENTAGE"]);
                        pvtdblPensionFixedAmount = Convert.ToDouble(pvtDataSet.Tables["TaxDeduction"].Rows[intRow]["TAX_DEDUCTION_FIXED_AMOUNT"]);
                        pvtdblPensionArrearFixedAmount = Convert.ToDouble(pvtDataSet.Tables["TaxDeduction"].Rows[intRow]["TAX_DEDUCTION_ARREAR_AMOUNT"]);

                        break;

                    case "2":

                        //Retirement Annuity
                        pvtdblRetirementAnnuityPercentage = Convert.ToDouble(pvtDataSet.Tables["TaxDeduction"].Rows[intRow]["TAX_DEDUCTION_PERCENTAGE"]);
                        pvtdblRetirementAnnuityFixedAmount = Convert.ToDouble(pvtDataSet.Tables["TaxDeduction"].Rows[intRow]["TAX_DEDUCTION_FIXED_AMOUNT"]);
                        pvtdblRetirementAnnuityPensionFixedAmount = Convert.ToDouble(pvtDataSet.Tables["TaxDeduction"].Rows[intRow]["TAX_DEDUCTION_PENSION_AMOUNT"]);
                        pvtdblRetirementAnnuityArrearFixedAmount = Convert.ToDouble(pvtDataSet.Tables["TaxDeduction"].Rows[intRow]["TAX_DEDUCTION_ARREAR_AMOUNT"]);

                        break;

                    case "3":

                        //Medical Aid
                        //pvtdblMedicalAidAge = Convert.ToDouble(pvtDataSet.Tables["TaxDeduction"].Rows[intRow]["TAX_DEDUCTION_AGE"]);

                        break;

                    case "4":

                        //2017-04-17 Retirement Funds All 
                        pvtdblRetirementFundsPercentage = Convert.ToDouble(pvtDataSet.Tables["TaxDeduction"].Rows[intRow]["TAX_DEDUCTION_PERCENTAGE"]);
                        pvtdblRetirementFundsMaxFixedAmount = Convert.ToDouble(pvtDataSet.Tables["TaxDeduction"].Rows[intRow]["TAX_DEDUCTION_FIXED_AMOUNT"]);

                        break;

                    default:

                        MessageBox.Show("Error with Tax Deduction Values Table - Invalid Deduction Number" + pvtDataSet.Tables["TaxDeduction"].Rows[intRow]["TAX_DEDUCTION_NO"].ToString(),
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        break;
                }
            }

            clsTax = new InteractPayroll.clsTax(parDataSet);

            clsTax.Employee_Date_Calculations
                  (parRunDate
                  ,pardttEmployeeBirthDate
                  ,pardttStartTaxYear
                  ,pardttEndTaxYear
                  ,pvtdttEmployeeTaxStartDate
                  ,pvtdblNumberPeriodsInYear
                  ,ref pvtdblEmployeePortionOfYear
                  ,ref pvtdblAgeAtTaxYearEnd
                  ,ref pvtdblEmployeeAnnualisedFactor
                  ,parstrPayrollType
                  ,pvtdttEmployeeLastRunDate
                  ,ref pvtdblEmployeeDaysWorked);

            string strDesc = "";
            
            for (int intRow1 = 0; intRow1 < 12; intRow1++)
            {
                this.dgvRetireAnnuityDataGridView.Rows.Add(pvtstrMonthDesc[intRow1],
                                                           pardblRetirementAnnuityAmount[intRow1].ToString("#######0.00"));
            }

            //Errol Added 20170417 - TOOOOOOOOOOOOOOOOOOOOOOO LOKKKKKKKKKKKKKKKKKKKKK ATTTTTTTTTT
            this.dgvRetireAnnuityDataGridView.Rows.Add("Total",
                                                        pardblRetirementAnnuityTotal.ToString("#######0.00"));
            
            
            this.dgvRetireAnnuityDataGridView[1, 12].Style = TotalDataGridViewCellStyle;

            for (int intRow1 = 0; intRow1 < 12; intRow1++)
            {
                this.dgvPensionDataGridView.Rows.Add(pvtstrMonthDesc[intRow1],
                                                     pardblPensionFundAmount[intRow1].ToString("#######0.00"));
            }


            this.dgvPensionDataGridView.Rows.Add("Total",
                                                 pardblPensionFundTotal.ToString("#######0.00"));

            
            this.dgvPensionDataGridView[1,12].Style = TotalDataGridViewCellStyle;
            
            this.dgvIncomeDataGridView.Rows.Add("",
                                                "Other Earnings",
                                                "", 
                                                pardblOtherTaxableEarningsYTD.ToString("#######0.00"));
            
            //2017-04-15
            double dblTotalNormalEarningsAnnualised = pardblTotalNormalEarningsAnnualised + pardblTotalDeductions;
            
            this.dgvIncomeDataGridView.Rows.Add("", 
                                                "Earnings",
                                                pardblTotalNormalEarnings.ToString("#######0.00"),
                                                dblTotalNormalEarningsAnnualised.ToString("#######0.00"));

            this.dgvIncomeDataGridView.Rows.Add("", 
                                                "Total",
                                                "", 
                                                pardblTotalEarnedAccumAnnualInitial.ToString("#######0.00"));


            this.dgvIncomeDataGridView[0, 1].Style = ControlColourDataGridViewCellStyle;
           
            this.dgvIncomeDataGridView[2, 1].Style = SubTotalDataGridViewCellStyle;
            this.dgvIncomeDataGridView[3,2].Style = TotalDataGridViewCellStyle; 

            this.dgvDeductionTotalDataGridView.Rows.Add("",
                                                        "Retirement Annuity Total",
                                                        "",
                                                        pardblRetirementAnnuityTotal.ToString("#########0.00"));

            this.dgvDeductionTotalDataGridView.Rows.Add("", 
                                                        "Pension Total",
                                                        "",
                                                        pardblPensionFundTotal.ToString("#########0.00"));

            double dblDeductionsTotal = pardblRetirementAnnuityTotal + pardblPensionFundTotal;

            this.dgvDeductionTotalDataGridView.Rows.Add("", 
                                                        "Deductions Total",
                                                         "",
                                                        dblDeductionsTotal.ToString("#########0.00"));


            pvtdblRetirementFundsPercentageAmount = Math.Round((pvtdblRetirementFundsPercentage / 100) * pardblTotalEarnedAccumAnnualInitial, 2);

            //errolo
            this.dgvDeductionTotalDataGridView.Rows.Add("",
                                                        pvtdblRetirementFundsPercentage.ToString("#########0.00") + " % of " + pardblTotalEarnedAccumAnnualInitial.ToString("#########0.00"),
                                                        pvtdblRetirementFundsPercentageAmount.ToString("#########0.00"),
                                                         "");

            this.dgvDeductionTotalDataGridView.Rows.Add("",
                                                       "Maximum Allowable Threshold",
                                                       pvtdblRetirementFundsMaxFixedAmount.ToString("#########0.00"),
                                                        "");

            if (pvtdblRetirementFundsPercentageAmount < pvtdblRetirementFundsMaxFixedAmount)
            {
                pvtdblRetirementFundsLimitedAmount = pvtdblRetirementFundsPercentageAmount;
            }
            else
            {
                pvtdblRetirementFundsLimitedAmount = pvtdblRetirementFundsMaxFixedAmount;

            }

            this.dgvDeductionTotalDataGridView.Rows.Add("",
                                                      "Limited To",
                                                      "",
                                                      pvtdblRetirementFundsLimitedAmount.ToString("#########0.00"));

            this.dgvDeductionTotalDataGridView.Rows.Add("",
                                                     "Total Taxable Allowable Deductions",
                                                     "",
                                                     pardblTotalDeductions.ToString("#########0.00"));
            
            this.dgvDeductionTotalDataGridView[3, 2].Style = TotalDataGridViewCellStyle;
            this.dgvDeductionTotalDataGridView[3, 6].Style = TotalDataGridViewCellStyle;

            this.dgvDeductionTotalDataGridView[0, 1].Style = ControlColourDataGridViewCellStyle;

            this.dgvMedicalDependantDataGridView.Rows.Add("",
                                                          "Main Member",
                                                          "0",
                                                          pvtdblMedicalMajorAmount.ToString("####0.00"),
                                                          "0.00");
            this.dgvMedicalDependantDataGridView.Rows.Add("",
                                                          "1st Dependent",
                                                           "0",
                                                          pvtdblMedicalMajorAmount.ToString("####0.00"),
                                                          "0.00");
            this.dgvMedicalDependantDataGridView.Rows.Add("",
                                                          "All Other Dependents",
                                                           "0",
                                                          pvtdblMedicalMinorAmount.ToString("####0.00"),
                                                          "0.00");
            this.dgvMedicalDependantDataGridView.Rows.Add("",
                                                          "Total",
                                                          "12",
                                                          "",
                                                          "0.00",
                                                          "0.00");

            this.dgvMedicalDependantDataGridView[4, 3].Style = TotalDataGridViewCellStyle;
            this.dgvMedicalDependantDataGridView[5, 3].Style = TotalDataGridViewCellStyle;

            //Override Alternating Row Colour
            this.dgvMedicalDependantDataGridView[0, 1].Style = ControlColourDataGridViewCellStyle;
            this.dgvMedicalDependantDataGridView[0, 3].Style = ControlColourDataGridViewCellStyle;

            if (parstrMedicalAidInd == "Y")
            {
                //Main Member
                this.dgvMedicalDependantDataGridView[2, 0].Value = "1";
                this.dgvMedicalDependantDataGridView[4, 0].Value = this.pvtdblMedicalMajorAmount.ToString("###0.00");

                int intMedicalAidNumberDependents = parintMedicalAidNumberDependents;

                if (intMedicalAidNumberDependents > 0)
                {
                    //1st Dependent
                    this.dgvMedicalDependantDataGridView[2, 1].Value = "1";
                    this.dgvMedicalDependantDataGridView[4, 1].Value = this.pvtdblMedicalMajorAmount.ToString("###0.00");

                    intMedicalAidNumberDependents -= 1;
                }

                if (intMedicalAidNumberDependents > 0)
                {
                    //Minor Dependent
                    this.dgvMedicalDependantDataGridView[2, 2].Value = intMedicalAidNumberDependents.ToString();

                    double dblMinorValue = intMedicalAidNumberDependents * this.pvtdblMedicalMinorAmount;

                    this.dgvMedicalDependantDataGridView[4, 2].Value = dblMinorValue.ToString("###0.00");
                }

                double dblMedicalTotal = 0;

                for (int intMedicalRow = 0; intMedicalRow < 3; intMedicalRow++)
                {
                    dblMedicalTotal += Convert.ToDouble(this.dgvMedicalDependantDataGridView[4, intMedicalRow].Value);
                }

                this.dgvMedicalDependantDataGridView[4, 3].Value = dblMedicalTotal.ToString("###0.00");

                //Annualise 
                dblMedicalTotal = dblMedicalTotal * 12;

                this.dgvMedicalDependantDataGridView[5, 3].Value = dblMedicalTotal.ToString("###0.00");
            }

            for (int intRow1 = 0; intRow1 < pardblTaxTotal.Length; intRow1++)
            {
                switch (intRow1)
                {
                    case 0:

                        this.dgvTaxDataGridView.Rows.Add("",
                                                         "Total Earnings Annualised",
                                                         pardblTaxTotal[intRow1].ToString("#########0.00"));

                        break;


                    case 1:

                        this.dgvTaxDataGridView.Rows.Add("", 
                                                         "- Total Taxable Allowable Deductions",
                                                         pardblTaxTotal[intRow1].ToString("#########0.00"));
                    
                        break;

                    case 2:

                        this.dgvTaxDataGridView.Rows.Add("",
                                                         "Final Taxable Earnings Annualised",
                                                         pardblTaxTotal[intRow1].ToString("#########0.00"));

                        break;

                    case 3:

                        this.dgvTaxDataGridView.Rows.Add("", 
                                                         "Tax (From Tax Tables)",
                                                         pardblTaxTotal[intRow1].ToString("#########0.00"));

                        break;

                    case 4:

                        this.dgvTaxDataGridView.Rows.Add("", 
                                                         "- Tax Rebate",
                                                         pardblTaxTotal[intRow1].ToString("#########0.00"));

                        break;



                    case 5:

                        this.dgvTaxDataGridView.Rows.Add("",
                                                         "Annualised Tax Due",
                                                         pardblTaxTotal[intRow1].ToString("#########0.00"));

                        break;

                    case 6:

                        this.dgvTaxDataGridView.Rows.Add("",
                                                         "- Annualised Medical Tax Credits",
                                                         pardblTaxTotal[intRow1].ToString("#########0.00"));

                        break;


                    case 7:

                        this.dgvTaxDataGridView.Rows.Add("", 
                                                         "Final Annualised Tax Due",
                                                         pardblTaxTotal[intRow1].ToString("#########0.00"));

                        break;

                    case 8:

                        this.dgvTaxDataGridView.Rows.Add("", 
                                                         "Total Tax Due (Pro Rata)",
                                                         pardblTaxTotal[intRow1].ToString("#########0.00"));

                        break;
     
                    case 9:

                        this.dgvTaxDataGridView.Rows.Add("", 
                                                         "- Tax Already Paid",
                                                         pardblTaxTotal[intRow1].ToString("#########0.00"));

                        break;

                    case 10:

                        this.dgvTaxDataGridView.Rows.Add("", 
                                                         "Tax Due",
                                                         pardblTaxTotal[intRow1].ToString("#########0.00"));

                        break;
                }
            }

            this.dgvTaxDataGridView[2, 10].Style = TotalDataGridViewCellStyle;
            //Override Alternating Row Colour
            this.dgvTaxDataGridView[0,1].Style = ControlColourDataGridViewCellStyle;
            this.dgvTaxDataGridView[0,3].Style = ControlColourDataGridViewCellStyle;
            this.dgvTaxDataGridView[0,5].Style = ControlColourDataGridViewCellStyle;
            this.dgvTaxDataGridView[0,7].Style = ControlColourDataGridViewCellStyle;
            this.dgvTaxDataGridView[0,9].Style = ControlColourDataGridViewCellStyle;

            pvtstrPayrollType = parstrPayrollType;

            if (parstrPayrollType == "W")
            {
                this.dgvInfoDataGridView.Columns[4].HeaderText = "Days Worked";
                this.dgvInfoDataGridView.Columns[5].HeaderText = "Days in Year";
                this.dgvInfoDataGridView.Columns[7].HeaderText = "Days in Run";
            }
            else
            {
                this.dgvInfoDataGridView.Columns[4].HeaderText = "Periods Worked";
                this.dgvInfoDataGridView.Columns[5].HeaderText = "Periods in Year";
                this.dgvInfoDataGridView.Columns[7].HeaderText = "Months in Run";
            }

            for (intRow = 0; intRow < pvtDataSet.Tables["TaxRebate"].Rows.Count; intRow++)
            {
                switch (pvtDataSet.Tables["TaxRebate"].Rows[intRow]["TAX_REBATE_TYPE"].ToString())
                {
                    case "P":

                        pvtdblRebatePrimary = Convert.ToDouble(pvtDataSet.Tables["TaxRebate"].Rows[intRow]["TAX_REBATE_VALUE"]);

                        break;

                    case "65":

                        pvtdblRebate65 = Convert.ToDouble(pvtDataSet.Tables["TaxRebate"].Rows[intRow]["TAX_REBATE_VALUE"]);
                        pvtdblRebate65Age = Convert.ToDouble(pvtDataSet.Tables["TaxRebate"].Rows[intRow]["TAX_REBATE_TYPE"]);

                        break;
                    
                    case "75":

                        pvtdblRebate75 = Convert.ToDouble(pvtDataSet.Tables["TaxRebate"].Rows[intRow]["TAX_REBATE_VALUE"]);
                        pvtdblRebate75Age = Convert.ToDouble(pvtDataSet.Tables["TaxRebate"].Rows[intRow]["TAX_REBATE_TYPE"]);

                        break;
                }
            }

            this.dgvRebatesDataGridView.Rows.Add("Primary",
                                                  pvtdblRebatePrimary);

            this.dgvRebatesDataGridView.Rows.Add("Over " + pvtdblRebate65Age.ToString(),
                                                  pvtdblRebate65);

            this.dgvRebatesDataGridView.Rows.Add("Over " + pvtdblRebate75Age.ToString(),
                                                  pvtdblRebate75);

            if (pvtdblAgeAtTaxYearEnd >= pvtdblRebate75Age)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvRebatesDataGridView, 2);
            }
            else
            {
                if (pvtdblAgeAtTaxYearEnd >= pvtdblRebate65Age)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvRebatesDataGridView, 1);
                }
                else
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvRebatesDataGridView, 0);
                }
            }
         
            for (int intRow1 = 0; intRow1 < pvtDataSet.Tables["Tax"].Rows.Count; intRow1++)
            {
                this.dgvTaxTablesDataGridView.Rows.Add(Convert.ToDouble(pvtDataSet.Tables["Tax"].Rows[intRow1]["TAX_VALUE_RANGE"]),
                                                       Convert.ToDouble(pvtDataSet.Tables["Tax"].Rows[intRow1]["TAX_FIXED_AMOUNT"]),
                                                       Convert.ToDouble(pvtDataSet.Tables["Tax"].Rows[intRow1]["TAX_PERCENTAGE"]));
            }

            if (parintAllTaxTableRow > -1)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvTaxTablesDataGridView,parintAllTaxTableRow);
            }


            this.dgvTaxFromTaxTablesGridView.Rows.Add("",
                                                      "Final Taxable Earnings Annualised",
                                                       pardblTaxTotal[2].ToString("#########0.00"));

            this.dgvTaxFromTaxTablesGridView.Rows.Add("",
                                                      "Tax Tables Row",
                                                      "- " + Convert.ToDouble(pvtDataSet.Tables["Tax"].Rows[parintAllTaxTableRow]["TAX_VALUE_RANGE"]).ToString("#########0.00"),
                                                      "",
                                                      Convert.ToDouble(pvtDataSet.Tables["Tax"].Rows[parintAllTaxTableRow]["TAX_FIXED_AMOUNT"]).ToString("#########0.00"));

            double dblAmount = pardblTaxTotal[2] - Convert.ToDouble(pvtDataSet.Tables["Tax"].Rows[parintAllTaxTableRow]["TAX_VALUE_RANGE"]);
            double dblCalculatedAmount = Math.Round(dblAmount * (Convert.ToDouble(pvtDataSet.Tables["Tax"].Rows[parintAllTaxTableRow]["TAX_PERCENTAGE"]) / 100),2);

            double dblTotalTaxAnnualisedAmount = Math.Round(Convert.ToDouble(pvtDataSet.Tables["Tax"].Rows[parintAllTaxTableRow]["TAX_FIXED_AMOUNT"]) + dblCalculatedAmount,2);

            this.dgvTaxFromTaxTablesGridView.Rows.Add("",
                                                      "Calculation",
                                                      dblAmount.ToString("#########0.00"),
                                                      Convert.ToDouble(pvtDataSet.Tables["Tax"].Rows[parintAllTaxTableRow]["TAX_PERCENTAGE"]).ToString("#########0.00"),
                                                      dblCalculatedAmount.ToString("#########0.00"),
                                                      dblTotalTaxAnnualisedAmount.ToString("#########0.00"));

            this.dgvTaxFromTaxTablesGridView[2, 2].Style = TotalDataGridViewCellStyle;
            this.dgvTaxFromTaxTablesGridView[5, 2].Style = TotalDataGridViewCellStyle;
           
            //Errol - 20150315 (New Tax Calculation for Lump sum Tax Payment for Other Earnings (Bonus)
            this.dgvTaxFromTaxTablesGridView.Rows.Add("",
                                                      "Earnings Annualised",
                                                       pardblTotalNormalEarningsAnnualised.ToString("#######0.00"));

            this.dgvTaxFromTaxTablesGridView.Rows.Add("",
                                                      "Tax Tables Row",
                                                      "- " + Convert.ToDouble(pvtDataSet.Tables["Tax"].Rows[parintEarningsTaxTableRow]["TAX_VALUE_RANGE"]).ToString("#########0.00"),
                                                      "",
                                                      Convert.ToDouble(pvtDataSet.Tables["Tax"].Rows[parintEarningsTaxTableRow]["TAX_FIXED_AMOUNT"]).ToString("#########0.00"));

            dblAmount = pardblTotalNormalEarningsAnnualised - Convert.ToDouble(pvtDataSet.Tables["Tax"].Rows[parintEarningsTaxTableRow]["TAX_VALUE_RANGE"]);
            dblCalculatedAmount = Math.Round(dblAmount * (Convert.ToDouble(pvtDataSet.Tables["Tax"].Rows[parintEarningsTaxTableRow]["TAX_PERCENTAGE"]) / 100),2);

            double dblEarningsTaxAnnualisedAmount = Convert.ToDouble(pvtDataSet.Tables["Tax"].Rows[parintEarningsTaxTableRow]["TAX_FIXED_AMOUNT"]) + dblCalculatedAmount;

            this.dgvTaxFromTaxTablesGridView.Rows.Add("",
                                                      "Calculation",
                                                      dblAmount.ToString("#########0.00"),
                                                      Convert.ToDouble(pvtDataSet.Tables["Tax"].Rows[parintEarningsTaxTableRow]["TAX_PERCENTAGE"]).ToString("#########0.00"),
                                                      dblCalculatedAmount.ToString("#########0.00"),
                                                      dblEarningsTaxAnnualisedAmount.ToString("#########0.00"));

            this.dgvTaxFromTaxTablesGridView[2, 5].Style = TotalDataGridViewCellStyle;
            this.dgvTaxFromTaxTablesGridView[5, 5].Style = TotalDataGridViewCellStyle;

            dblAmount = Math.Round(dblTotalTaxAnnualisedAmount - dblEarningsTaxAnnualisedAmount,2);

            this.dgvTaxFromTaxTablesGridView.Rows.Add("",
                                                     "Other Earnings (Lump Sum)",
                                                     "",
                                                     "",
                                                     "",
                                                     dblAmount.ToString("#########0.00"));

            this.dgvTaxFromTaxTablesGridView[5, 6].Style = SubTotalDataGridViewCellStyle;

            //Tax Pro Rata Calculation
            this.dgvTaxProRataDataGridView.Rows.Add("",
                                                    "Final Annualised Tax Due",
                                                     pardblTaxTotal[7].ToString("#########0.00"));

            double dblLumpSumPortionTax = dblAmount;

            if (dblLumpSumPortionTax > pardblTaxTotal[7])
            {
                dblLumpSumPortionTax = pardblTaxTotal[7];
            }

            //Tax Pro Rata Calculation
            this.dgvTaxProRataDataGridView.Rows.Add("",
                                                    "Other Earnings (Lump Sum)",
                                                     dblAmount.ToString("#########0.00"),
                                                     dblLumpSumPortionTax.ToString("#########0.00"));

            dblAmount = pardblTaxTotal[7] - dblAmount;

            if (dblAmount < 0)
            {
                dblAmount = 0;
            }

            double dblPortion = Math.Round(dblAmount / pvtdblEmployeeAnnualisedFactor,2);

            //Tax Pro Rata Calculation
            this.dgvTaxProRataDataGridView.Rows.Add("",
                                                    "Pro Rata (Portion of Year)",
                                                     dblAmount.ToString("#########0.00"),
                                                     dblPortion.ToString("#########0.00"));

            dblAmount = dblPortion + dblLumpSumPortionTax;

            //Tax Pro Rata Calculation
            this.dgvTaxProRataDataGridView.Rows.Add("",
                                                    "Total Tax Due (Pro Rata)",
                                                     "",
                                                     dblAmount.ToString("#########0.00"));

            this.dgvTaxProRataDataGridView[0, 1].Style = ControlColourDataGridViewCellStyle;
            this.dgvTaxProRataDataGridView[0, 3].Style = ControlColourDataGridViewCellStyle;

            this.dgvTaxProRataDataGridView[3, 3].Style = TotalDataGridViewCellStyle;

            if (pardblTaxTotal[8] != dblAmount)
            {
                MessageBox.Show("There is an Error in The Tax Calculation");
            }

            for (int intRow1 = 0; intRow1 < pardblUifTotal.Length; intRow1++)
            {
                switch (intRow1)
                {
                    case 0:

                        this.dgvUifDataGridView.Rows.Add("",
                                                         "Earnings for Period",
                                                         "",
                                                         "",
                                                          pardblUifTotal[intRow1].ToString("########0.00"));

                        break;

                    case 1:

                        this.dgvUifDataGridView.Rows.Add("", 
                                                         "- Commission",
                                                         "",
                                                         "",
                                                          pardblUifTotal[intRow1].ToString("########0.00"));

                        break;

                    case 2:

                        this.dgvUifDataGridView.Rows.Add("", 
                                                         "Total Earnings for Period",
                                                         "",
                                                         "",
                                                          pardblUifTotal[intRow1].ToString("########0.00"));

                        break;

                    case 3:

                        string strUifDesc = "";

                        if (pvtstrPayrollType == "W")
                        {
                            strUifDesc = pvtdblEmployeeDaysWorked.ToString() + "/" + pvtdblNumberPeriodsInYear + " Days";
                        }
                        else
                        {
                            strUifDesc = pvtdblEmployeeDaysWorked.ToString() + "/12 Months";
                        }

                        this.dgvUifDataGridView.Rows.Add("", 
                                                         strUifDesc,
                                                         "",
                                                         pvtdblAnnualUIFAmount.ToString("########0.00"),
                                                          pardblUifTotal[intRow1].ToString("########0.00"));

                        break;

                    case 4:

                        this.dgvUifDataGridView.Rows.Add("", 
                                                         "Lesser of 2 Values",
                                                         pvtdblUIFPercentageAmount.ToString("########0.00"),
                                                         "",
                                                          pardblUifTotal[intRow1].ToString("########0.00"));

                        break;

                    case 5:

                        this.dgvUifDataGridView.Rows.Add("", 
                                                         "Total UIF",
                                                         "",
                                                         "",
                                                          pardblUifTotal[intRow1].ToString("########0.00"));

                        break;
                }
            }

            this.dgvUifDataGridView[0, 1].Style = ControlColourDataGridViewCellStyle;
            this.dgvUifDataGridView[0, 3].Style = ControlColourDataGridViewCellStyle;
            this.dgvUifDataGridView[0, 5].Style = ControlColourDataGridViewCellStyle;

            this.dgvUifDataGridView[4, 2].Style = SubTotalDataGridViewCellStyle;
            this.dgvUifDataGridView[4, 3].Style = SubTotalDataGridViewCellStyle;

            this.dgvUifDataGridView[4, 5].Style = TotalDataGridViewCellStyle;
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
                    case "dgvRebatesDataGridView":

                        this.dgvRebatesDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvTaxTablesDataGridView":

                        this.dgvTaxTablesDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvIncomeDataGridView":

                        this.dgvIncomeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvDeductionTotalDataGridView":

                        this.dgvDeductionTotalDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        private void frmTax_Load(object sender, System.EventArgs e)
        {
#if(DEBUG)
            int intWidth = 0;
            int intHeight = 0;
            int intNewHeight = 0;

            DataGridView myDataGridView;

            foreach (Control myControl in this.Controls)
            {
                if (myControl is DataGridView)
                {
                    myDataGridView = null;
                    myDataGridView = (DataGridView)myControl;

                    if (myDataGridView.RowHeadersVisible == true)
                    {
                        intWidth = myDataGridView.RowHeadersWidth;
                    }
                    else
                    {
                        intWidth = 0;
                    }

                    if (myDataGridView.ScrollBars == ScrollBars.Vertical
                        | myDataGridView.ScrollBars == ScrollBars.Both)
                    {
                        intWidth += 19;
                    }

                    for (int intCol = 0; intCol < myDataGridView.ColumnCount; intCol++)
                    {
                        if (myDataGridView.Columns[intCol].Visible == true)
                        {
                            intWidth += myDataGridView.Columns[intCol].Width;
                        }
                    }

                    if (intWidth != myDataGridView.Width)
                    {
                        MessageBox.Show(myDataGridView.Name + " Width should be " + intWidth.ToString());
                    }

                    if (myDataGridView.ColumnHeadersVisible == true)
                    {
                        intHeight = myDataGridView.ColumnHeadersHeight + 2;
                    }
                    else
                    {
                        intHeight = 0;
                    }

                    intNewHeight = myDataGridView.RowTemplate.Height / 2;

                    for (int intRow = 0; intRow < 200; intRow++)
                    {
                        intHeight += myDataGridView.RowTemplate.Height;

                        if (intHeight == myDataGridView.Height)
                        {
                            break;
                        }
                        else
                        {
                            if (intHeight > myDataGridView.Height)
                            {
                                MessageBox.Show(myDataGridView.Name + " Height should be " + intHeight.ToString());
                                break;
                            }
                            else
                            {

                                if (intHeight + intNewHeight > myDataGridView.Height)
                                {
                                    MessageBox.Show(myDataGridView.Name + " Height should be " + intHeight.ToString());
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (myControl is GroupBox)
                    {
                        foreach (Control myControl1 in myControl.Controls)
                        {
                            if (myControl1 is DataGridView)
                            {
                                myDataGridView = null;
                                myDataGridView = (DataGridView)myControl1;

                                string myName = myDataGridView.Name;

                                if (myDataGridView.RowHeadersVisible == true)
                                {
                                    intWidth = myDataGridView.RowHeadersWidth;
                                }
                                else
                                {
                                    intWidth = 0;
                                }

                                if (myDataGridView.ScrollBars == ScrollBars.Vertical
                                    | myDataGridView.ScrollBars == ScrollBars.Both)
                                {
                                    intWidth += 19;
                                }

                                for (int intCol = 0; intCol < myDataGridView.ColumnCount; intCol++)
                                {
                                    if (myDataGridView.Columns[intCol].Visible == true)
                                    {
                                        intWidth += myDataGridView.Columns[intCol].Width;
                                    }
                                }

                                if (intWidth != myDataGridView.Width)
                                {
                                    MessageBox.Show(myDataGridView.Name + " Width should be " + intWidth.ToString());
                                }

                                if (myDataGridView.ColumnHeadersVisible == true)
                                {
                                    intHeight = myDataGridView.ColumnHeadersHeight + 2;
                                }
                                else
                                {
                                    intHeight = 0;
                                }

                                intNewHeight = myDataGridView.RowTemplate.Height / 2;

                                for (int intRow = 0; intRow < 200; intRow++)
                                {
                                    intHeight += myDataGridView.RowTemplate.Height;

                                    if (intHeight == myDataGridView.Height)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        if (intHeight > myDataGridView.Height)
                                        {
                                            MessageBox.Show(myDataGridView.Name + " Height should be " + intHeight.ToString());
                                            break;
                                        }
                                        else
                                        {

                                            if (intHeight + intNewHeight > myDataGridView.Height)
                                            {
                                                MessageBox.Show(myDataGridView.Name + " Height should be " + intHeight.ToString());
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (myControl is TabControl)
                        {
                            foreach (Control myControl2 in myControl.Controls)
                            {
                                if (myControl2 is TabPage)
                                {
                                    foreach (Control myControl3 in myControl2.Controls)
                                    {
                                        if (myControl3 is DataGridView)
                                        {
                                            myDataGridView = null;
                                            myDataGridView = (DataGridView)myControl3;

                                            if (myDataGridView.RowHeadersVisible == true)
                                            {
                                                intWidth = myDataGridView.RowHeadersWidth;
                                            }
                                            else
                                            {
                                                intWidth = 0;
                                            }

                                            if (myDataGridView.ScrollBars == ScrollBars.Vertical
                                                | myDataGridView.ScrollBars == ScrollBars.Both)
                                            {
                                                intWidth += 19;
                                            }

                                            for (int intCol = 0; intCol < myDataGridView.ColumnCount; intCol++)
                                            {
                                                if (myDataGridView.Columns[intCol].Visible == true)
                                                {
                                                    intWidth += myDataGridView.Columns[intCol].Width;
                                                }
                                            }

                                            if (intWidth != myDataGridView.Width)
                                            {
                                                MessageBox.Show(myDataGridView.Name + " Width should be " + intWidth.ToString());
                                            }

                                            if (myDataGridView.ColumnHeadersVisible == true)
                                            {
                                                intHeight = myDataGridView.ColumnHeadersHeight + 2;
                                            }
                                            else
                                            {
                                                intHeight = 0;
                                            }

                                            intNewHeight = myDataGridView.RowTemplate.Height / 2;

                                            for (int intRow = 0; intRow < 200; intRow++)
                                            {
                                                intHeight += myDataGridView.RowTemplate.Height;

                                                if (intHeight == myDataGridView.Height)
                                                {
                                                    break;
                                                }
                                                else
                                                {
                                                    if (intHeight > myDataGridView.Height)
                                                    {
                                                        MessageBox.Show(myDataGridView.Name + " Height should be " + intHeight.ToString());
                                                        break;
                                                    }
                                                    else
                                                    {

                                                        if (intHeight + intNewHeight > myDataGridView.Height)
                                                        {
                                                            MessageBox.Show(myDataGridView.Name + " Height should be " + intHeight.ToString());
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
#endif      

            this.lblDeductions.Paint += new System.Windows.Forms.PaintEventHandler(Label_Paint);
            this.lblEarnings.Paint += new System.Windows.Forms.PaintEventHandler(Label_Paint);
            this.lblInfo.Paint += new System.Windows.Forms.PaintEventHandler(Label_Paint);
            this.lblMedicalAid.Paint += new System.Windows.Forms.PaintEventHandler(Label_Paint);
            this.lblTaxFromTaxTables.Paint += new System.Windows.Forms.PaintEventHandler(Label_Paint);
            this.lblPensionFund.Paint += new System.Windows.Forms.PaintEventHandler(Label_Paint);
            this.lblRetirementAnnuity.Paint += new System.Windows.Forms.PaintEventHandler(Label_Paint);
            this.lblTaxCalculation.Paint += new System.Windows.Forms.PaintEventHandler(Label_Paint);
            this.lblTaxRebate.Paint += new System.Windows.Forms.PaintEventHandler(Label_Paint);
            this.lblTaxTables.Paint += new System.Windows.Forms.PaintEventHandler(Label_Paint);
            this.lblUIF.Paint += new System.Windows.Forms.PaintEventHandler(Label_Paint);
            this.lblTaxProRata.Paint += new System.Windows.Forms.PaintEventHandler(Label_Paint);

            this.dgvInfoDataGridView.Rows.Add("",
                                              pubstrTaxInfo[0],
                                              pubstrTaxInfo[1],
                                              pubstrTaxInfo[2],
                                              pubstrTaxInfo[3],
                                              pubstrTaxInfo[4],
                                              pvtdblEmployeeAnnualisedFactor.ToString("###0.00000000"),
                                              pvtdblEmployeeDaysWorked);
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        public void Label_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            myPaintEventArgs = e;

            System.Windows.Forms.Label lbl = (System.Windows.Forms.Label)sender;

            Paint_Label(lbl);
        }

        private void Paint_Label(System.Windows.Forms.Label lbl)
        {
            Paint_Label_Background_Area(lbl);

            Paint_Label_Text(lbl);
        }

        private void Paint_Label_Text(System.Windows.Forms.Label lbl)
        {
            System.Drawing.Rectangle rect = new Rectangle(lbl.Left, lbl.Top, lbl.Width, lbl.Height);
            System.Drawing.SizeF size = myPaintEventArgs.Graphics.MeasureString(lbl.Text, lbl.Font);

            System.Drawing.Point pt = new System.Drawing.Point((rect.Width - Convert.ToInt32(size.Width)) / 2, (rect.Height - Convert.ToInt32(size.Height)) / 2);

            myPaintEventArgs.Graphics.DrawString(lbl.Text.Replace("&", ""), lbl.Font, new SolidBrush(lbl.ForeColor), pt.X, pt.Y);
        }

        private void Paint_Label_Background_Area(System.Windows.Forms.Label lbl)
        {
            System.Drawing.Color color = lbl.BackColor;

            Color[] ColorArray = null;
            float[] PositionArray = null;

            ColorArray = new Color[]{ColourBlend(color,System.Drawing.SystemColors.HighlightText,20),
                                        ColourBlend(color,System.Drawing.SystemColors.HighlightText,30),
                                        ColourBlend(color,System.Drawing.SystemColors.HighlightText,20),
                                        ColourBlend(color,System.Drawing.SystemColors.HighlightText,00),               
                                        ColourBlend(color,System.Drawing.SystemColors.MenuText,20),
                                        ColourBlend(color,System.Drawing.SystemColors.MenuText,10),};

            PositionArray = new float[] { 0.0f, .15f, .40f, .65f, .80f, 1.0f };

            System.Drawing.Drawing2D.ColorBlend blend = new System.Drawing.Drawing2D.ColorBlend();
            blend.Colors = ColorArray;
            blend.Positions = PositionArray;

            System.Drawing.Rectangle rect = new Rectangle(0, 0, lbl.Width, lbl.Height);

            System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, System.Drawing.SystemColors.MenuText, ColourBlend(System.Drawing.SystemColors.MenuText, System.Drawing.SystemColors.MenuText, 10), System.Drawing.Drawing2D.LinearGradientMode.Vertical);
            brush.InterpolationColors = blend;

            myPaintEventArgs.Graphics.FillRectangle(brush, rect);

            brush.Dispose();
        }

        private static Color ColourBlend(Color SColor, Color DColor, int Percentage)
        {
            int r = SColor.R + ((DColor.R - SColor.R) * Percentage) / 100;
            int g = SColor.G + ((DColor.G - SColor.G) * Percentage) / 100;
            int b = SColor.B + ((DColor.B - SColor.B) * Percentage) / 100;
            return Color.FromArgb(r, g, b);
        }
        
        private void dgvRebatesDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void dgvTaxTablesDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void dgvIncomeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void dgvDeductionTotalDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void frmTax_Paint(object sender, PaintEventArgs e)
        {
            Pen Pen1Pixel = new Pen(Color.Black, 1);
            Pen Pen3Pixels = new Pen(Color.Black, 3);

            //Total Taxable Allowable Deduction - Begin
            //Total Taxable Allowable Deduction - Begin
            int x1 = 365;
            //Correct
            int y1 = 472;
            int x2 = 398;
            //Correct
            int y2 = 570;

            //Horizontal Line (Top)
            e.Graphics.DrawLine(Pen3Pixels, x1, y1, x2, y1);

            //Vertical Line (Only 1)
            e.Graphics.DrawLine(Pen3Pixels, x1, y1, x1, y2);
            
            ////Arrow
            e.Graphics.DrawLine(Pen1Pixel, x2 + 2, y1, x2 + 2, y1 + 2);
            e.Graphics.DrawLine(Pen1Pixel, x2 + 1, y1 - 1, x2 + 1, y1 + 3);
            e.Graphics.DrawLine(Pen1Pixel, x2, y1 - 2, x2, y1 + 4);
            e.Graphics.DrawLine(Pen1Pixel, x2 - 1, y1 - 3, x2 - 1, y1 + 5);
            e.Graphics.DrawLine(Pen1Pixel, x2 - 2, y1 - 4, x2 - 2, y1 + 6);

            x2 = 354;

            ////Horizontal Line (Bottom)
            e.Graphics.DrawLine(Pen3Pixels, x2, y2 - 2, x1, y2 - 2);

            //Total Taxable Allowable Deduction - End
            //Total Taxable Allowable Deduction - End
            
            //Other Earnings Lump Sum - Begin
            //Other Earnings Lump Sum - Begin
            x1 = 745;
            y1 = 373;
            x2 = 765;
            y2 = 445;

            //Vertical Line (Only 1)
            e.Graphics.DrawLine(Pen3Pixels, x1, y1, x1, y2);

            //Horizontal Line (Between Arrow and Vertical Line)
            e.Graphics.DrawLine(Pen3Pixels, x1, y2 - 2, x2, y2 - 2);

            //Arrow
            e.Graphics.DrawLine(Pen1Pixel, x2 + 2, y2 - 3, x2 + 2, y2 - 1);
            e.Graphics.DrawLine(Pen1Pixel, x2 + 1, y2 - 4, x2 + 1, y2);
            e.Graphics.DrawLine(Pen1Pixel, x2 , y2 - 5, x2, y2 + 1);
            e.Graphics.DrawLine(Pen1Pixel, x2 - 1, y2 - 6, x2 - 1, y2 + 2);
            e.Graphics.DrawLine(Pen1Pixel, x2 - 2, y2 - 7, x2 - 2, y2 + 3);
            //Other Earnings Lump Sum - End
            //Other Earnings Lump Sum - End

            //Total Tax Due (Pro Rata) - Begin
            //Total Tax Due (Pro Rata) - Begin
            x1 = 745;
            y1 = 482;
            x2 = 768;
            y2 = 608;
      
            //Horizontal Line (Top)
            e.Graphics.DrawLine(Pen3Pixels, x1, y1, x2, y1);

            //Vertical Line (Only 1)
            e.Graphics.DrawLine(Pen3Pixels, x1, y1, x1, y2);

            x2 = 715;

            //Horizontal Line (Bottom)
            e.Graphics.DrawLine(Pen3Pixels, x2, y2 - 2, x1, y2 - 2);

            //Arrow
            e.Graphics.DrawLine(Pen1Pixel, x2 + 2, y2 - 7, x2 + 2, y2 + 3);
            e.Graphics.DrawLine(Pen1Pixel, x2 + 1, y2 - 6, x2 + 1, y2 + 2);
            e.Graphics.DrawLine(Pen1Pixel, x2, y2 - 5, x2, y2 + 1);
            e.Graphics.DrawLine(Pen1Pixel, x2 - 1, y2 - 4, x2 - 1, y2);
            e.Graphics.DrawLine(Pen1Pixel, x2 - 2, y2 - 3, x2 - 2, y2 - 1);
            //Total Tax Due (Pro Rata) - End
            //Total Tax Due (Pro Rata) - End

            //Final Annualised Tax - Begin
            //Final Annualised Tax - Begin
            x1 = 725;
            y1 = 425;
            x2 = 765;
            y2 = 588;

            //Horizontal Line (Top)
            e.Graphics.DrawLine(Pen3Pixels, x1, y1, x2, y1);

            ////Vertical Line (Connector)
            e.Graphics.DrawLine(Pen3Pixels, x1 + 1, y1, x1 + 1, y2);

            ////Horizontal Line (Bottom - Small)
            e.Graphics.DrawLine(Pen3Pixels, x1 - 10, y2, x1 + 3, y2);

            ////Arrow
            e.Graphics.DrawLine(Pen1Pixel, x2 + 2, y1, x2 + 2, y1 + 2);
            e.Graphics.DrawLine(Pen1Pixel, x2 + 1, y1 - 1, x2 + 1, y1 + 3);
            e.Graphics.DrawLine(Pen1Pixel, x2, y1 - 2, x2, y1 + 4);
            e.Graphics.DrawLine(Pen1Pixel, x2 - 1, y1 - 3, x2 - 1, y1 + 5);
            e.Graphics.DrawLine(Pen1Pixel, x2 - 2, y1 - 4, x2 - 2, y1 + 6);
            //Final Annualised Tax - End
            //Final Annualised Tax - End
        }
    }
}
