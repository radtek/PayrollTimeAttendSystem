using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace InteractPayroll
{
    public class clsTax
    {
        private DataSet pvtDataSet;

        private double pvtdblRebate65 = 0;
        private double pvtdblRebate65Age = 0;
        private double pvtdblRebate75 = 0;
        private double pvtdblRebate75Age = 0;
        private double pvtdblRebatePrimary = 0;
        private double pvtdblCasualPercent = 0;

        private double pvtdblAnnualUIFAmount = 0;
        private double pvtdblUIFPercentageAmount = 0;
        
        private double pvtdblPensionPercentage = 0;
        private double pvtdblPensionFixedAmount = 0;
        private double pvtdblPensionArrearFixedAmount = 0;

        private double pvtdblRetirementAnnuityPercentage = 0;
        private double pvtdblRetirementAnnuityFixedAmount = 0;
        private double pvtdblRetirementAnnuityArrearFixedAmount = 0;
        private double pvtdblRetirementAnnuityPensionFixedAmount = 0;

        private double pvtdblMedicalMajorAmount = 0;
        private double pvtdblMedicalMinorAmount = 0;

        private double pvtdblRetirementFundsPercentage = 0;
        private double pvtdblRetirementFundsMaxFixedAmount = 0;

        private double pvtdblRetirementFundsPercentageAmount = 0;
        private double pvtdblRetirementFundsLimitedAmount = 0;
        
        //Rows Tax Calculation
        int pvtintTotalAnnualisedEarningsRow = 0;
        int pvtintTotalTaxAllowableDeductionsRow = 1;
        int pvtintTotalTaxableAnnualisedEarningsRow = 2;
        int pvtintTaxFromTaxTablesRow = 3;
        int pvtintRebateRow = 4;
        int pvtintTaxAnnualisedTaxRow = 5;
        int pvtintMedicalTaxCreditsRow = 6;
        int pvtintFinalAnnualisedTaxRow = 7;
        int pvtintTaxProRateRow = 8;
        int pvtintTaxAlreadyPaidRow = 9;
        int pvtintTaxDueRow = 10;
        
        private double pvtdblMedicalAidAge = 0;

        private bool pvtblnTableError = false;

        public clsTax(DataSet parDataSet)
        {
            pvtDataSet = parDataSet;

            if (pvtDataSet.Tables["TaxRebate"].Rows.Count == 0)
            {
                MessageBox.Show("Error with Tax Rebate Table - Data Missing",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                pvtblnTableError = true;
            }
            else
            {
                for (int intRow = 0; intRow < pvtDataSet.Tables["TaxRebate"].Rows.Count; intRow++)
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
            }

            if (pvtDataSet.Tables["TaxCasual"].Rows.Count == 0)
            {
                MessageBox.Show("Error with Tax Casual Table - Data Missing",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                pvtblnTableError = true;
            }
            else
            {
                pvtdblCasualPercent = Convert.ToDouble(pvtDataSet.Tables["TaxCasual"].Rows[0]["TAX_CASUAL_PERCENT"]);
            }

            if (pvtDataSet.Tables["TaxDeduction"].Rows.Count == 0)
            {
                MessageBox.Show("Error with Tax Deduction Values Table - Data Missing",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                pvtblnTableError = true;
            }
            else
            {
                for (int intRow = 0; intRow < pvtDataSet.Tables["TaxDeduction"].Rows.Count; intRow++)
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
                            pvtdblMedicalAidAge = Convert.ToDouble(pvtDataSet.Tables["TaxDeduction"].Rows[intRow]["TAX_DEDUCTION_AGE"]);

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

                            pvtblnTableError = true;

                            break;
                    }
                }
            }

            if (pvtDataSet.Tables["Tax"].Rows.Count == 0)
            {
                MessageBox.Show("Error with Tax Table - Data Missing",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                pvtblnTableError = true;
            }

            if (pvtDataSet.Tables["UIF"].Rows.Count == 0)
            {
                MessageBox.Show("Error with UIF Table - Data Missing",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                pvtblnTableError = true;
            }
            else
            {
                pvtdblAnnualUIFAmount = Convert.ToDouble(pvtDataSet.Tables["UIF"].Rows[0]["UIF_YEAR_AMOUNT"]);
                pvtdblUIFPercentageAmount = Convert.ToDouble(pvtDataSet.Tables["UIF"].Rows[0]["UIF_PERCENTAGE"]);
            }


            if (pvtDataSet.Tables["TaxMedicalCap"].Rows.Count == 0)
            {
                MessageBox.Show("Error with MedicalAid Table - Data Missing",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                pvtblnTableError = true;
            }
            else
            {
                pvtdblMedicalMajorAmount = Convert.ToDouble(pvtDataSet.Tables["TaxMedicalCap"].Rows[0]["MAJOR_AMOUNT"]);
                pvtdblMedicalMinorAmount = Convert.ToDouble(pvtDataSet.Tables["TaxMedicalCap"].Rows[0]["MINOR_AMOUNT"]);
            }
        }

        private void Calculate_Fringe_Benefit(int parintRow, bool parblnShowInd, double[] pardblMedicalEmployerAmount, double[] pardblMedicalCappedAmount, ref double[] pardblMedicalFringeBenefitAmount, ref double[] pardblMedicalResultAmount, ref double pardblMedicalFringeBenefitTotal, ref double pardblMedicalResultTotal)
        {
            if (Convert.ToDouble(pardblMedicalEmployerAmount[parintRow - 1]) > Convert.ToDouble(pardblMedicalCappedAmount[parintRow - 1]))
            {
                pardblMedicalFringeBenefitAmount[parintRow - 1] = Math.Round(Convert.ToDouble(pardblMedicalEmployerAmount[parintRow - 1]) - Convert.ToDouble(pardblMedicalCappedAmount[parintRow - 1]), 2);

                pardblMedicalFringeBenefitTotal += pardblMedicalFringeBenefitAmount[parintRow - 1];
            }
            else
            {
                if (Convert.ToDouble(pardblMedicalCappedAmount[parintRow - 1]) > Convert.ToDouble(pardblMedicalEmployerAmount[parintRow - 1]))
                {
                    pardblMedicalResultAmount[parintRow - 1] = Math.Round(Convert.ToDouble(pardblMedicalCappedAmount[parintRow - 1]) - Convert.ToDouble(pardblMedicalEmployerAmount[parintRow - 1]), 2);

                    pardblMedicalResultTotal += pardblMedicalResultAmount[parintRow - 1];
                }
            }
        }

        private void Calculate_Per_Day_Value(DateTime parRunDateTime, double pardblValue, double pardblEmployeePeriodsWorked, ref double pardblValuePerDay)
        {
            pardblValuePerDay = Math.Round(pardblValue / pardblEmployeePeriodsWorked, 2);
        }

        public int Calculate_Tax(double pardblTaxableEarningsYTD, double pardblOtherTaxableEarningsYTD,
            ref double pardblTaxCalculatedRun, double pardblEmployeeAnnualisedFactor,
            double pardblAgeAtTaxYearEnd, double pardblMonthValue,
            double pardblTaxYTD, 
            string parstrRunType, double pardblTakeOnTaxYTD,
            string parstrPayrollType, string parPayUIFInd,
            double pardblPeriodsInTaxYear, double pardblEmployeePeriodsWorked,
            double pardblAmountEarnedCurrent, ref double pardblUIFAmount,
            DateTime parRunDate,
            string parstrMedicalAidInd,int parintTaxNumberDependants,
            DataView parTaxSpreadSheetDataview,
            DataView parEarningDescDataView, double pardblPensionArrearYTD,
            double parblRetirementAnnuityArrearYTD, string parstrTaxType,
            double pardblDirectivePercentage, double pardblCommission,
            ref double[] pardblRetirementAnnuityAmount,
            ref double pardblRetirementAnnuityTotal,
            ref double[] pardblPensionFundAmount,
            ref double pardblPensionFundTotal,
            ref double pardblTotalNormalEarnings,
            ref double pardblTotalNormalEarningsAnnualised,
            ref double pardblTotalEarnedAccumAnnualInitial,
            ref double pardblTotalDeductions,

            ref double[] pardblTaxTotal,
            ref int intAllTaxTableRow,
            ref int intEarningsTaxTableRow,
            ref double[] pardblUifTotal
            )
        {
            int intRow = 1;
            int intIRP5 = 0;
           
            double dblAmount = 0;
            double dblUIFThresholdAmount = 0;
            double dblUIFEarned = 0;
            double dblValuePerDay = 0;
            
            double dblTaxAnnualised = 0;
            double dblEarningsOtherTaxAnnualised = 0;
            double dblEarningsTaxAnnualised = 0;
            
            double dblPensionFundCalculatedTotal = 0;
            double dblRetirementAnnuityTotal = 0;
            double dblTotalEarnedAccumAnnual = 0;
            
            if (parstrPayrollType != "W"
                & parstrPayrollType != "S")
            {
                MessageBox.Show("Invalid Payroll type - clsTax.Calculate Tax");
                return 1;
            }

            pardblRetirementAnnuityTotal = 0;
            pardblPensionFundTotal = 0;

            //Initialise Values Passed through
            for (int intRow1 = 0; intRow1 < 12; intRow1++)
            {
                if (intRow1 < pardblRetirementAnnuityAmount.Length)
                {
                    pardblRetirementAnnuityAmount[intRow1] = 0;
                }

                if (intRow1 < pardblPensionFundAmount.Length)
                {
                    pardblPensionFundAmount[intRow1] = 0;
                }

                if (intRow1 < pardblTaxTotal.Length)
                {
                    pardblTaxTotal[intRow1] = 0;
                }

                if (intRow1 < pardblUifTotal.Length)
                {
                    pardblUifTotal[intRow1] = 0;
                }
            }

            pardblTotalNormalEarnings = 0;
            pardblTotalNormalEarningsAnnualised = 0;
            pardblTotalEarnedAccumAnnualInitial = 0;

            pardblTotalDeductions = 0;
            pardblTaxYTD = pardblTaxYTD * -1;

            pardblTotalNormalEarnings = pardblTaxableEarningsYTD - pardblOtherTaxableEarningsYTD;
            pardblTotalNormalEarningsAnnualised = Math.Round(pardblTotalNormalEarnings * pardblEmployeeAnnualisedFactor, 2);
            dblTotalEarnedAccumAnnual = pardblTotalNormalEarningsAnnualised + pardblOtherTaxableEarningsYTD;
            pardblTotalEarnedAccumAnnualInitial = dblTotalEarnedAccumAnnual;
            
            pardblTaxTotal[pvtintTotalAnnualisedEarningsRow] = pardblTotalEarnedAccumAnnualInitial;
          
            //Reset
            intRow = 1;

            if (parstrRunType != "T")
            {
                pardblTaxTotal[pvtintTaxAlreadyPaidRow] = pardblTaxYTD;
            }
            else
            {
                pardblTaxTotal[pvtintTaxAlreadyPaidRow] = pardblTakeOnTaxYTD;
            }

            DateTime dtTempDateTime = new DateTime(2007, 3, 1);
          
            for (int intSpreadRow = 0; intSpreadRow < parTaxSpreadSheetDataview.Count; intSpreadRow++)
            {
                if (Convert.ToInt32(parTaxSpreadSheetDataview[intSpreadRow]["IRP5_CODE"]) == 4005
                    | Convert.ToInt32(parTaxSpreadSheetDataview[intSpreadRow]["IRP5_CODE"]) == 3810)
                {
                    //ELR 2014-04-28
                }
                else
                {
                    intIRP5 = Convert.ToInt32(parTaxSpreadSheetDataview[intSpreadRow]["IRP5_CODE"]);

                    if (Convert.ToInt32(parTaxSpreadSheetDataview[intSpreadRow]["PERIOD_MONTH"]) > 2)
                    {
                        intRow = Convert.ToInt32(parTaxSpreadSheetDataview[intSpreadRow]["PERIOD_MONTH"]) - 2;
                    }
                    else
                    {
                        intRow = 10 + Convert.ToInt32(parTaxSpreadSheetDataview[intSpreadRow]["PERIOD_MONTH"]);
                    }

                    dtTempDateTime = new DateTime(Convert.ToInt32(parTaxSpreadSheetDataview[intSpreadRow]["PERIOD_YEAR"]), Convert.ToInt32(parTaxSpreadSheetDataview[intSpreadRow]["PERIOD_MONTH"]), 1).AddMonths(1).AddDays(-1);

                    if (Convert.ToInt32(parTaxSpreadSheetDataview[intSpreadRow]["PERIOD_MONTH"]) == parRunDate.Month)
                    {
                        if (parstrPayrollType == "W"
                            & parRunDate.Day < 30)
                        {
                            Calculate_Per_Day_Value(parRunDate, Convert.ToDouble(parTaxSpreadSheetDataview[intSpreadRow]["HISTORY_TOTAL_VALUE"]), pardblEmployeePeriodsWorked, ref dblValuePerDay);

                            dblAmount = Math.Round(Convert.ToDouble(parTaxSpreadSheetDataview[intSpreadRow]["HISTORY_TOTAL_VALUE"]) + ((dtTempDateTime.Day - parRunDate.Day) * dblValuePerDay), 2);

                            if (intIRP5 == 4001)
                            {
                                pardblPensionFundAmount[intRow - 1] = dblAmount;
                            }
                            else
                            {
                                pardblRetirementAnnuityAmount[intRow - 1] = dblAmount;
                            }
                        }
                        else
                        {
                            dblAmount = Convert.ToDouble(parTaxSpreadSheetDataview[intSpreadRow]["HISTORY_TOTAL_VALUE"]);

                            if (intIRP5 == 4001)
                            {
                                pardblPensionFundAmount[intRow - 1] = dblAmount;
                                dblPensionFundCalculatedTotal += dblAmount;
                            }
                            else
                            {
                                pardblRetirementAnnuityAmount[intRow - 1] = dblAmount;
                                dblRetirementAnnuityTotal += dblAmount;
                            }
                        }

                        for (intRow = intRow + 1; intRow < 13; intRow++)
                        {
                            if (parstrPayrollType == "W"
                                & parRunDate.Day < 30)
                            {
                                dtTempDateTime = dtTempDateTime.AddDays(1);
                                dtTempDateTime = dtTempDateTime.AddMonths(1).AddDays(-1);

                                dblAmount = Math.Round(dtTempDateTime.Day * dblValuePerDay, 2);
                            }

                            if (intIRP5 == 4001)
                            {
                                pardblPensionFundAmount[intRow - 1] = dblAmount;
                                dblPensionFundCalculatedTotal += dblAmount;
                            }
                            else
                            {
                                pardblRetirementAnnuityAmount[intRow - 1] = dblAmount;
                                dblRetirementAnnuityTotal += dblAmount;
                            }
                        }
                    }
                    else
                    {
                        if (intIRP5 == 4001)
                        {
                            pardblPensionFundAmount[intRow - 1] = Convert.ToDouble(parTaxSpreadSheetDataview[intSpreadRow]["HISTORY_TOTAL_VALUE"]);
                            dblPensionFundCalculatedTotal += Convert.ToDouble(parTaxSpreadSheetDataview[intSpreadRow]["HISTORY_TOTAL_VALUE"]);
                        }
                        else
                        {
                            pardblRetirementAnnuityAmount[intRow - 1] = Convert.ToDouble(parTaxSpreadSheetDataview[intSpreadRow]["HISTORY_TOTAL_VALUE"]);
                            dblRetirementAnnuityTotal += Convert.ToDouble(parTaxSpreadSheetDataview[intSpreadRow]["HISTORY_TOTAL_VALUE"]);
                        }
                    }
                }
            }
            
            pardblPensionFundTotal = dblPensionFundCalculatedTotal;
            pardblRetirementAnnuityTotal = dblRetirementAnnuityTotal;
            
            pardblTotalDeductions = pardblRetirementAnnuityTotal + pardblPensionFundTotal;

            if (pardblTotalDeductions != 0)
            {
                //Errol 
                pvtdblRetirementFundsPercentageAmount = Math.Round((pvtdblRetirementFundsPercentage / 100) * pardblTotalEarnedAccumAnnualInitial, 2);

                if (pvtdblRetirementFundsPercentageAmount < pvtdblRetirementFundsMaxFixedAmount)
                {
                    pvtdblRetirementFundsLimitedAmount = pvtdblRetirementFundsPercentageAmount;
                }
                else
                {
                    pvtdblRetirementFundsLimitedAmount = pvtdblRetirementFundsMaxFixedAmount;
                }

                if (pardblTotalDeductions > pvtdblRetirementFundsLimitedAmount)
                {
                    pardblTotalDeductions = pvtdblRetirementFundsLimitedAmount;
                }
            }

            dblTotalEarnedAccumAnnual = dblTotalEarnedAccumAnnual - pardblTotalDeductions;

            if (dblTotalEarnedAccumAnnual < 0)
            {
                dblTotalEarnedAccumAnnual = 0;
            }
                
            pardblTaxTotal[pvtintTotalTaxAllowableDeductionsRow] = pardblTotalDeductions;
            pardblTaxTotal[pvtintTotalTaxableAnnualisedEarningsRow] = dblTotalEarnedAccumAnnual;

            //2017-04-15 
            pardblTotalNormalEarningsAnnualised = pardblTotalNormalEarningsAnnualised - pardblTotalDeductions;  

            if (pardblTotalNormalEarningsAnnualised < 0)
            {
                pardblTotalNormalEarningsAnnualised = 0;
            }

            intAllTaxTableRow = -1;

            for (int intRow1 = 0; intRow1 < pvtDataSet.Tables["Tax"].Rows.Count; intRow1++)
            {
                if (dblTotalEarnedAccumAnnual < Convert.ToDouble(pvtDataSet.Tables["Tax"].Rows[intRow1]["TAX_VALUE_RANGE"]))
                {
                    intAllTaxTableRow = intRow1 - 1;
                    break;
                }
                else
                {
                    if (intRow1 == pvtDataSet.Tables["Tax"].Rows.Count - 1)
                    {
                        if (dblTotalEarnedAccumAnnual >= Convert.ToDouble(pvtDataSet.Tables["Tax"].Rows[intRow1]["TAX_VALUE_RANGE"]))
                        {
                            //ELR 2014-10-10
                            intAllTaxTableRow = intRow1;
                            break;
                        }
                    }
                }
            }

            if (pvtblnTableError == true)
            {
                //Error with Tax Tables
                return 1;
            }

            //Errol - 20150315 (New Tax Calculation for Lump sum Tax Payment for Other Earnings (Bonus)
            intEarningsTaxTableRow = -1;

            for (int intRow1 = 0; intRow1 < pvtDataSet.Tables["Tax"].Rows.Count; intRow1++)
            {
                if (pardblTotalNormalEarningsAnnualised < Convert.ToDouble(pvtDataSet.Tables["Tax"].Rows[intRow1]["TAX_VALUE_RANGE"]))
                {
                    intEarningsTaxTableRow = intRow1 - 1;
                    break;
                }
                else
                {
                    if (intRow1 == pvtDataSet.Tables["Tax"].Rows.Count - 1)
                    {
                        if (pardblTotalNormalEarningsAnnualised >= Convert.ToDouble(pvtDataSet.Tables["Tax"].Rows[intRow1]["TAX_VALUE_RANGE"]))
                        {
                            intEarningsTaxTableRow = intRow1;
                            break;
                        }
                    }
                }
            }

            if (pvtblnTableError == true)
            {
                //Error with Tax Tables
                return 1;
            }

            //Normal Tax
            if (parstrTaxType == "N")
            {
                intRow = 1;

                //Calculate expected TAX at year end before REBATES
                dblTaxAnnualised = Math.Round((Convert.ToDouble(pvtDataSet.Tables["Tax"].Rows[intAllTaxTableRow]["TAX_FIXED_AMOUNT"]) + ((dblTotalEarnedAccumAnnual - Convert.ToDouble(pvtDataSet.Tables["Tax"].Rows[intAllTaxTableRow]["TAX_VALUE_RANGE"])) * (Convert.ToDouble(pvtDataSet.Tables["Tax"].Rows[intAllTaxTableRow]["TAX_PERCENTAGE"]) / 100))), 2);

                //Total Tax Without Other Earnings Included (Bonusetc)
                dblEarningsTaxAnnualised  = Math.Round((Convert.ToDouble(pvtDataSet.Tables["Tax"].Rows[intEarningsTaxTableRow]["TAX_FIXED_AMOUNT"]) + ((pardblTotalNormalEarningsAnnualised - Convert.ToDouble(pvtDataSet.Tables["Tax"].Rows[intEarningsTaxTableRow]["TAX_VALUE_RANGE"])) * (Convert.ToDouble(pvtDataSet.Tables["Tax"].Rows[intEarningsTaxTableRow]["TAX_PERCENTAGE"]) / 100))), 2);

                dblEarningsOtherTaxAnnualised = Math.Round(dblTaxAnnualised - dblEarningsTaxAnnualised,2); 
           }
            else
            {
                //Part Time
                if (parstrTaxType == "P")
                {
                    dblTaxAnnualised = Math.Round(dblTotalEarnedAccumAnnual * (pvtdblCasualPercent / 100), 2);
                }
                else
                {
                    //Tax Directive
                    dblTaxAnnualised = Math.Round(dblTotalEarnedAccumAnnual * (pardblDirectivePercentage / 100), 2);
                }
            }

            pardblTaxTotal[pvtintTaxFromTaxTablesRow] = dblTaxAnnualised;

            double dblMedicalAidCredit = 0;

            if (parstrMedicalAidInd == "Y")
            {
                if (parintTaxNumberDependants == 0)
                {
                    dblMedicalAidCredit = pvtdblMedicalMajorAmount;
                }
                else
                {
                    if (parintTaxNumberDependants == 1)
                    {
                        dblMedicalAidCredit = 2 * pvtdblMedicalMajorAmount;
                    }
                    else
                    {
                        dblMedicalAidCredit = (2 * pvtdblMedicalMajorAmount) + ((parintTaxNumberDependants - 1) * pvtdblMedicalMinorAmount);
                    }
                }

                //Annualised
                dblMedicalAidCredit = dblMedicalAidCredit * 12;

                pardblTaxTotal[pvtintMedicalTaxCreditsRow] = dblMedicalAidCredit;

                dblTaxAnnualised = dblTaxAnnualised - dblMedicalAidCredit; 
            }

            //Calculate TAX for Pay Period
            if (pardblAgeAtTaxYearEnd < pvtdblRebate65Age)
            {
                pardblTaxTotal[pvtintRebateRow] = pvtdblRebatePrimary;

                pardblTaxCalculatedRun = Math.Round((dblTaxAnnualised - pvtdblRebatePrimary) / pardblEmployeeAnnualisedFactor, 2);
            }
            else
            {
                if (pardblAgeAtTaxYearEnd < pvtdblRebate75Age)
                {
                    pardblTaxTotal[pvtintRebateRow] = pvtdblRebate65;

                    pardblTaxCalculatedRun = Math.Round((dblTaxAnnualised - pvtdblRebate65) / pardblEmployeeAnnualisedFactor, 2);
                }
                else
                {
                    //75 and Older
                    pardblTaxTotal[pvtintRebateRow] = pvtdblRebate75;

                    pardblTaxCalculatedRun = Math.Round((dblTaxAnnualised - pvtdblRebate75) / pardblEmployeeAnnualisedFactor, 2);
                }
            }

            if (pardblTaxCalculatedRun < 0)
            {
                pardblTaxCalculatedRun = 0;
            }

            dblAmount = pardblTaxTotal[pvtintTaxFromTaxTablesRow] - pardblTaxTotal[pvtintRebateRow];

            if (dblAmount < 0)
            {
                dblAmount = 0;
            }

            pardblTaxTotal[pvtintTaxAnnualisedTaxRow] = dblAmount;

            //ELR 2014-04-28
            dblAmount = pardblTaxTotal[pvtintTaxAnnualisedTaxRow] - pardblTaxTotal[pvtintMedicalTaxCreditsRow];

            if (dblAmount < 0)
            {
                dblAmount = 0;
            }

            pardblTaxTotal[pvtintFinalAnnualisedTaxRow] = dblAmount;

            double dblNewEarningsTaxAnnualised = 0;

            if (dblAmount > dblEarningsOtherTaxAnnualised)
            {
                dblNewEarningsTaxAnnualised = Math.Round(dblAmount - dblEarningsOtherTaxAnnualised,2);
            }
            else
            {
                dblEarningsOtherTaxAnnualised = dblAmount;
                dblNewEarningsTaxAnnualised = 0;
            }

            if (dblNewEarningsTaxAnnualised > 0)
            {
                pardblTaxCalculatedRun = Math.Round(dblNewEarningsTaxAnnualised / pardblEmployeeAnnualisedFactor,2) + dblEarningsOtherTaxAnnualised; 
            }
            else
            {
                //Only Other Earning Lump Sum Portion
                pardblTaxCalculatedRun = dblEarningsOtherTaxAnnualised;  
            }
   
            pardblTaxTotal[pvtintTaxProRateRow] = pardblTaxCalculatedRun;

            if (parstrRunType == "T")
            {
                pardblTaxTotal[pvtintTaxAlreadyPaidRow] = pardblTakeOnTaxYTD;
                pardblTaxCalculatedRun = pardblTaxCalculatedRun - pardblTakeOnTaxYTD;
            }
            else
            {
                pardblTaxTotal[pvtintTaxAlreadyPaidRow] = pardblTaxYTD;
                pardblTaxCalculatedRun = pardblTaxCalculatedRun + pardblTaxYTD;
            }

            pardblTaxTotal[pvtintTaxDueRow] = pardblTaxCalculatedRun;

            if (parPayUIFInd == "Y")                                                              
            {
                pardblUifTotal[0] = pardblAmountEarnedCurrent;
                pardblUifTotal[1] = pardblCommission;

                //Total UIF Amount is Total Earnings minus Commission
                dblUIFEarned = Math.Round(pardblAmountEarnedCurrent - pardblCommission, 2);
                //UIF THreshold Value for Pay Period
                dblUIFThresholdAmount = Math.Round(pvtdblAnnualUIFAmount * (pardblEmployeePeriodsWorked / pardblPeriodsInTaxYear), 2);

                pardblUifTotal[2] = dblUIFEarned;
                pardblUifTotal[3] = dblUIFThresholdAmount;
               
                if (dblUIFEarned < dblUIFThresholdAmount)
                {
                    pardblUIFAmount = Math.Round(dblUIFEarned * (pvtdblUIFPercentageAmount / 100), 2);

                    pardblUifTotal[4] = dblUIFEarned;
                }
                else
                {
                    pardblUIFAmount = Math.Round(dblUIFThresholdAmount * (pvtdblUIFPercentageAmount / 100), 2);

                    pardblUifTotal[4] = dblUIFThresholdAmount;
                }

                pardblUifTotal[5] = pardblUIFAmount;
            }
           
            return 0;
        }

        public void Employee_Date_Calculations(DateTime dttWageRunDate, DateTime dttEmployeeBirthDate,
                                                            DateTime dttStartTaxYear, DateTime dttEndTaxYear,
                                                            DateTime dttEmployeeStartDate, double pardblPeriodsInTaxYear,
                                                            ref double pardblEmployeePortionOfYear, ref double pardblAgeAtTaxYearEnd,
                                                            ref double pardblEmployeeAnnualisedFactor, string parstrPayrollType,
                                                            DateTime dttEmployeeLastRunDate,
                                                            ref double pardblEmployeePeriodsWorked)
        {
            if (parstrPayrollType != "W"
            & parstrPayrollType != "S")
            {
                MessageBox.Show("Invalid Tax Run Type");
                return;
            }

            //Employee Start Date is set to Start of Tax Fiscal Year Date
            if (dttStartTaxYear > dttEmployeeStartDate)
            {
                dttEmployeeStartDate = dttStartTaxYear;
            }

            if (parstrPayrollType == "W")
            {
                TimeSpan tsDateTimeSpan;

                if (dttEmployeeLastRunDate >= dttEmployeeStartDate)
                {
                    //Days Employee has worked in Tax Year
                    tsDateTimeSpan = dttWageRunDate.AddDays(1).Subtract(dttEmployeeStartDate);
                }
                else
                {
                    //Days Employee has worked (Employee Run Covers 2 Fiscal Years) eg 2011-02-25 to 2011-03-03
                    tsDateTimeSpan = dttWageRunDate.Subtract(dttEmployeeLastRunDate);
                }

                pardblEmployeePortionOfYear = tsDateTimeSpan.Days;

                //Used to work out what an Employee would earn in a Year
                pardblEmployeeAnnualisedFactor = Math.Round(pardblPeriodsInTaxYear / pardblEmployeePortionOfYear,8);

                //Days Since Last Pay Cheque
                tsDateTimeSpan = dttWageRunDate.Subtract(dttEmployeeLastRunDate);
                //Annualised Factor for working out UIF
                pardblEmployeePeriodsWorked = tsDateTimeSpan.Days;
            }
            else
            {
                int intEmployeeStartYear = dttEmployeeStartDate.Year;
                int intRunYear = dttWageRunDate.Year;

                int intEmployeeStartMonth = dttEmployeeStartDate.Month;
                int intRunMonth = dttWageRunDate.Month;
                int intNumberMonths = 0;

                if (intEmployeeStartYear == intRunYear)
                {
                    intNumberMonths = intRunMonth - intEmployeeStartMonth;
                }
                else
                {
                    intNumberMonths = intRunMonth + 12 - intEmployeeStartMonth;
                }

                if (dttWageRunDate.Day != 1)
                {
                    //Not Take-on
                    intNumberMonths += 1;
                }

                if (intNumberMonths == 0)
                {
                    pardblEmployeeAnnualisedFactor = 1;
                    pardblEmployeePortionOfYear = 0;
                }
                else
                {
                    pardblEmployeeAnnualisedFactor = Math.Round(Convert.ToDouble(12) / Convert.ToDouble(intNumberMonths),8);
                    pardblEmployeePortionOfYear = intNumberMonths;
                }

                pardblEmployeePeriodsWorked = 1;
            }

            //Employee Age at End Of Year
            double dblEndTaxYear = dttEndTaxYear.Year;
            double dblEmployeeBirthDate = dttEmployeeBirthDate.Year;

            pardblAgeAtTaxYearEnd = dblEndTaxYear - dblEmployeeBirthDate;

            if (dttEmployeeBirthDate.AddMonths(12 * (int)pardblAgeAtTaxYearEnd) > dttEndTaxYear)
            {
                pardblAgeAtTaxYearEnd = pardblAgeAtTaxYearEnd - 1;
            }
        }
    }
}

