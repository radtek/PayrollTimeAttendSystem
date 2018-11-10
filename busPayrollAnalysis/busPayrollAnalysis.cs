using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busPayrollAnalysis
    {
        clsDBConnectionObjects clsDBConnectionObjects;
        clsTaxTableRead clsTaxTableRead;

        public busPayrollAnalysis()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parInt64CompanyNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
            DataSet TempDataSet;

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND (WAGE_RUN_IND = 'Y'");
            strQry.AppendLine(" OR SALARY_RUN_IND = 'Y')");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

            if (DataSet.Tables["Temp"].Rows.Count == 0)
            {
                DataSet.Tables.Remove("Temp");

                goto Get_Form_Records_Continue;
            }

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" 'Wages' AS PAYROLL_TYPE ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C ");
            strQry.AppendLine(" ON PCPC.COMPANY_NO = C.COMPANY_NO ");
            strQry.AppendLine(" AND C.WAGE_RUN_IND = 'Y'");

            strQry.AppendLine(" WHERE PCPC.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PCPC.PAY_CATEGORY_NO > 0");
            strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P'");

            strQry.AppendLine(" UNION ");
            
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" 'Salaries' AS PAYROLL_TYPE ");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_NO > 0");
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
            strQry.AppendLine(" AND RUN_TYPE = 'P'");

            strQry.AppendLine(" ORDER BY 1 DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayrollType", parInt64CompanyNo);

            byte[] bytTempCompress = Get_Company_Records(parInt64CompanyNo, DataSet.Tables["PayrollType"].Rows[0]["PAYROLL_TYPE"].ToString().Substring(0,1));
            TempDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(bytTempCompress);
            DataSet.Merge(TempDataSet);

            Get_Form_Records_Continue:

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_Company_Records(Int64 parInt64CompanyNo, string parstrPayrollType)
        {
            StringBuilder strQry = new StringBuilder();
            DataView TaxSpreadSheetDataView;
            DataView EmployeeDeductionDataView;
            DataView EmployeeDataView;
            DataView EmployeeEarningDataView;
            DataRowView drvDataRowView;
            DataSet DataSet = new DataSet();
            DateTime dtStartTaxYear;
            object[] objFind = new object[2];
            int intFindRow;

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" C.COMPANY_DESC");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PPC.PAY_PERIOD_DATE");
            strQry.AppendLine(",PPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PPC.COMPANY_NO");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(",C.OVERTIME1_RATE");
                strQry.AppendLine(",C.OVERTIME2_RATE");
                strQry.AppendLine(",C.OVERTIME3_RATE");
            }
            else
            {
                strQry.AppendLine(",C.SALARY_OVERTIME1_RATE AS OVERTIME1_RATE");
                strQry.AppendLine(",C.SALARY_OVERTIME2_RATE AS OVERTIME2_RATE");
                strQry.AppendLine(",C.SALARY_OVERTIME3_RATE AS OVERTIME3_RATE");
            }

            strQry.AppendLine(",PPC.PAIDHOLIDAY_RATE");
            //strQry.AppendLine(",PPC.SALARY_OVERTIME_RATE");
            strQry.AppendLine(",PPC.SALARY_MINUTES_PAID_PER_DAY");
            strQry.AppendLine(",PPC.SALARY_DAYS_PER_YEAR");
            strQry.AppendLine(",PPC.TOTAL_DAILY_TIME_OVERTIME");
            strQry.AppendLine(",PPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",C.SALARY_DOUBLE_CHEQUE_BIRTHDAY_IND");
            strQry.AppendLine(",'A' AS ACCESS_IND");

            strQry.AppendLine(",C.WAGE_RUN_IND");
            strQry.AppendLine(",C.SALARY_RUN_IND");

            strQry.AppendLine(",PUBLIC_HOLIDAY_IND = ");
            strQry.AppendLine(" CASE");
            strQry.AppendLine(" WHEN PH.PUBLIC_HOLIDAY_DATE IS NULL ");
            strQry.AppendLine(" THEN 'N'");

            strQry.AppendLine(" ELSE 'Y'");

            strQry.AppendLine(" END");
 
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON C.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PPC");
            strQry.AppendLine(" ON PC.COMPANY_NO = PPC.COMPANY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = PPC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = PPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PPC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
            strQry.AppendLine(" AND PPC.PAY_CATEGORY_NO > 0");
            strQry.AppendLine(" AND PPC.RUN_TYPE = 'P'");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY PH");
            strQry.AppendLine(" ON PH.PUBLIC_HOLIDAY_DATE >= PPC.PAY_PERIOD_DATE_FROM");
            strQry.AppendLine(" AND PH.PUBLIC_HOLIDAY_DATE <= PPC.PAY_PERIOD_DATE");
            
            strQry.AppendLine(" WHERE C.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

           if (parstrPayrollType == "W")
           {
                strQry.AppendLine(" AND C.WAGE_RUN_IND = 'Y'");
            }
            else
            {
                strQry.AppendLine(" AND C.SALARY_RUN_IND = 'Y'");
            }
       
            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" C.COMPANY_DESC");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PPC.PAY_PERIOD_DATE");
            strQry.AppendLine(",PPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PPC.COMPANY_NO");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(",C.OVERTIME1_RATE");
                strQry.AppendLine(",C.OVERTIME2_RATE");
                strQry.AppendLine(",C.OVERTIME3_RATE");
            }
            else
            {
                strQry.AppendLine(",C.SALARY_OVERTIME1_RATE");
                strQry.AppendLine(",C.SALARY_OVERTIME2_RATE");
                strQry.AppendLine(",C.SALARY_OVERTIME3_RATE");
            }

            strQry.AppendLine(",PPC.PAIDHOLIDAY_RATE");
            strQry.AppendLine(",PPC.SALARY_MINUTES_PAID_PER_DAY");
            strQry.AppendLine(",PPC.SALARY_DAYS_PER_YEAR");
            strQry.AppendLine(",PPC.TOTAL_DAILY_TIME_OVERTIME");
            strQry.AppendLine(",PPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",C.SALARY_DOUBLE_CHEQUE_BIRTHDAY_IND");

            strQry.AppendLine(",C.WAGE_RUN_IND");
            strQry.AppendLine(",C.SALARY_RUN_IND");
            strQry.AppendLine(",PH.PUBLIC_HOLIDAY_DATE");
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", parInt64CompanyNo);

            if (DataSet.Tables["Company"].Rows.Count > 0)
            {
                if (Convert.ToDateTime(DataSet.Tables["Company"].Rows[0]["PAY_PERIOD_DATE"]).Month > 2)
                {
                    dtStartTaxYear = new DateTime(Convert.ToDateTime(DataSet.Tables["Company"].Rows[0]["PAY_PERIOD_DATE"]).Year, 3, 1);
                }
                else
                {
                    dtStartTaxYear = new DateTime(Convert.ToDateTime(DataSet.Tables["Company"].Rows[0]["PAY_PERIOD_DATE"]).Year - 1, 3, 1);
                }

                if (clsTaxTableRead == null)
                {
                    clsTaxTableRead = new clsTaxTableRead();
                    DataSet TempDataSet = clsTaxTableRead.Get_Tax_UIF_Tables(dtStartTaxYear.Year + 1);
                    DataSet.Merge(TempDataSet);
                }

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" E.COMPANY_NO");
                strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",E.EMPLOYEE_CODE");
                strQry.AppendLine(",E.EMPLOYEE_SURNAME");
                strQry.AppendLine(",E.EMPLOYEE_NAME");
                strQry.AppendLine(",E.TAX_TYPE_IND");
                strQry.AppendLine(",E.TAX_DIRECTIVE_PERCENTAGE");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",E.NUMBER_MEDICAL_AID_DEPENDENTS");
                strQry.AppendLine(",E.EMPLOYEE_TAX_STARTDATE");
                strQry.AppendLine(",E.EMPLOYEE_BIRTHDATE");
                strQry.AppendLine(",E.EMPLOYEE_ID_NO");
                strQry.AppendLine(",EIC.CLOSE_IND");
                strQry.AppendLine(",EIC.CLOSE_REMOVE_EARNING_AND_LEAVE_IND");
                strQry.AppendLine(",E.EMPLOYEE_LAST_RUNDATE");
                strQry.AppendLine(",E.EMPLOYEE_NUMBER_CHEQUES");
                strQry.AppendLine(",EIC.EXTRA_CHEQUES_HISTORY");
                strQry.AppendLine(",EIC.EXTRA_CHEQUES_CURRENT");
                strQry.AppendLine(",EIC.PAYSLIP_IND");
                strQry.AppendLine(",ISNULL(EEC.HOURS_DECIMAL,0) AS PUBLIC_HOLIDAY_HOURS_DECIMAL");
                strQry.AppendLine(",ISNULL(SUM(TEMP_EMPLOYEE_EARNING.PAY_EARNING_TOTAL),0) - ISNULL(SUM(TEMP_EMPLOYEE_DEDUCTION.PAY_DEDUCTION_TOTAL),0) AS PAY_TOTAL");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC ");
                strQry.AppendLine(" ON E.COMPANY_NO = EIC.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EIC.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EIC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC ");
                strQry.AppendLine(" ON E.COMPANY_NO = EEC.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EEC.EMPLOYEE_NO");
                strQry.AppendLine(" AND EEC.EARNING_NO = 8 ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EEC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EIC.RUN_TYPE = EEC.RUN_TYPE ");
                strQry.AppendLine(" AND EIC.RUN_NO = EEC.RUN_NO ");
        
                strQry.AppendLine(" LEFT JOIN ");

                strQry.AppendLine("(SELECT ");
                strQry.AppendLine(" EMPLOYEE_NO");
                strQry.AppendLine(",RUN_NO ");
                strQry.AppendLine(",ISNULL(SUM(TOTAL),0) AS PAY_EARNING_TOTAL");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT ");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND RUN_TYPE = 'P'");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" EMPLOYEE_NO");
                strQry.AppendLine(",RUN_NO) AS TEMP_EMPLOYEE_EARNING");

                strQry.AppendLine(" ON E.EMPLOYEE_NO = TEMP_EMPLOYEE_EARNING.EMPLOYEE_NO");
                strQry.AppendLine(" AND EIC.RUN_NO = TEMP_EMPLOYEE_EARNING.RUN_NO ");

                strQry.AppendLine(" LEFT JOIN ");

                strQry.AppendLine("(SELECT ");
                strQry.AppendLine(" EMPLOYEE_NO");
                strQry.AppendLine(",RUN_NO ");
                strQry.AppendLine(",ISNULL(SUM(TOTAL),0) AS PAY_DEDUCTION_TOTAL");
              
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT ");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND RUN_TYPE = 'P'");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" EMPLOYEE_NO");
                strQry.AppendLine(",RUN_NO)  AS TEMP_EMPLOYEE_DEDUCTION ");
             
                strQry.AppendLine(" ON E.EMPLOYEE_NO = TEMP_EMPLOYEE_DEDUCTION.EMPLOYEE_NO");
                strQry.AppendLine(" AND EIC.RUN_NO = TEMP_EMPLOYEE_DEDUCTION.RUN_NO ");
              
                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" E.COMPANY_NO");
                strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",E.EMPLOYEE_CODE");
                strQry.AppendLine(",E.EMPLOYEE_SURNAME");
                strQry.AppendLine(",E.EMPLOYEE_NAME");
                strQry.AppendLine(",E.TAX_TYPE_IND");
                strQry.AppendLine(",E.TAX_DIRECTIVE_PERCENTAGE");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",E.NUMBER_MEDICAL_AID_DEPENDENTS");
                strQry.AppendLine(",E.EMPLOYEE_TAX_STARTDATE");
                strQry.AppendLine(",E.EMPLOYEE_BIRTHDATE");
                strQry.AppendLine(",E.EMPLOYEE_ID_NO");
                strQry.AppendLine(",EIC.CLOSE_IND");
                strQry.AppendLine(",EIC.CLOSE_REMOVE_EARNING_AND_LEAVE_IND");
                strQry.AppendLine(",E.EMPLOYEE_LAST_RUNDATE");
                strQry.AppendLine(",E.EMPLOYEE_NUMBER_CHEQUES");
                strQry.AppendLine(",EIC.EXTRA_CHEQUES_HISTORY");
                strQry.AppendLine(",EIC.EXTRA_CHEQUES_CURRENT");
                strQry.AppendLine(",EIC.PAYSLIP_IND");
                strQry.AppendLine(",EEC.HOURS_DECIMAL");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parInt64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EPCC.COMPANY_NO");
                strQry.AppendLine(",EPCC.EMPLOYEE_NO");
                strQry.AppendLine(",EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EPCC.PAY_CATEGORY_TYPE");

                if (parstrPayrollType == "W")
                {
                    strQry.AppendLine(",EPCC.HOURLY_RATE");
                    strQry.AppendLine(",EPCC.HOURLY_RATE AS ANNUAL_SALARY");
                }
                else
                {
                    strQry.AppendLine(",EPCC.HOURLY_RATE");
                    strQry.AppendLine(",E.ANNUAL_SALARY");
                }

                strQry.AppendLine(",EPCC.DEFAULT_IND");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON EPCC.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND EPCC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");
                    strQry.AppendLine(" ON EPCC.COMPANY_NO = PCPC.COMPANY_NO ");
                    strQry.AppendLine(" AND EPCC.RUN_TYPE = PCPC.RUN_TYPE ");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = PCPC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = PCPC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P'");
                }

                strQry.AppendLine(" WHERE EPCC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

                //2017-02-20 Add History
                strQry.AppendLine(" UNION ");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EPCH.COMPANY_NO");
                strQry.AppendLine(",EPCH.EMPLOYEE_NO");
                strQry.AppendLine(",EPCH.PAY_CATEGORY_NO");
                strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE");

                if (parstrPayrollType == "W")
                {
                    strQry.AppendLine(",MAX(EPCH.HOURLY_RATE) AS HOURLY_RATE");
                    strQry.AppendLine(",MAX(EPCH.HOURLY_RATE) AS ANNUAL_SALARY");
                }
                else
                {
                    strQry.AppendLine(",MAX(EPCH.HOURLY_RATE) AS HOURLY_RATE");
                    strQry.AppendLine(",E.ANNUAL_SALARY");
                }

                strQry.AppendLine(",EPCH.DEFAULT_IND");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");
                strQry.AppendLine(" ON EPCC.COMPANY_NO = EPCH.COMPANY_NO ");
                strQry.AppendLine(" AND EPCC.EMPLOYEE_NO = EPCH.EMPLOYEE_NO ");

                if (parstrPayrollType == "W")
                {
                    strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = 'S' ");
                }
                else
                {
                    strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = 'W' ");
                }

                strQry.AppendLine(" AND EPCH.PAY_PERIOD_DATE >= '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");
                
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON EPCC.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND EPCC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                
                strQry.AppendLine(" WHERE EPCC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                
                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" EPCH.COMPANY_NO");
                strQry.AppendLine(",EPCH.EMPLOYEE_NO");
                strQry.AppendLine(",EPCH.PAY_CATEGORY_NO");
                strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE");

                if (parstrPayrollType == "S")
                { 
                    strQry.AppendLine(",E.ANNUAL_SALARY");
                }

                strQry.AppendLine(",EPCH.DEFAULT_IND");


                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" 1");
                strQry.AppendLine(",2");
                strQry.AppendLine(",3");
                strQry.AppendLine(",4");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeePayCategory", parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",DEDUCTION_NO");
                strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_COUNT");
                strQry.AppendLine(",DEDUCTION_DESC");
                strQry.AppendLine(",IRP5_CODE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.DEDUCTION ");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" COMPANY_NO");
                strQry.AppendLine(",DEDUCTION_NO");
                strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "DeductionDesc", parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EDEPC.COMPANY_NO");
                strQry.AppendLine(",EDEPC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EDEPC.EMPLOYEE_NO");
                strQry.AppendLine(",EDEPC.DEDUCTION_NO");
                strQry.AppendLine(",EDEPC.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",EDEPC.EARNING_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE_CURRENT EDEPC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON EDEPC.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND EDEPC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EDEPC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                strQry.AppendLine(" WHERE EDEPC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EDEPC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" EDEPC.EMPLOYEE_NO");
                strQry.AppendLine(",EDEPC.DEDUCTION_NO");
                strQry.AppendLine(",EDEPC.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",EDEPC.EARNING_NO");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "DeductionEarningPercentage", parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" E.COMPANY_NO");
                strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",E.EARNING_NO");
                strQry.AppendLine(",E.LEAVE_PERCENTAGE");
                strQry.AppendLine(",E.EARNING_DESC");
                strQry.AppendLine(",E.IRP5_CODE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING E ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C ");
                strQry.AppendLine(" ON E.COMPANY_NO = C.COMPANY_NO ");
                strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL ");

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

                if (parstrPayrollType == "S")
                {
                    //OverTime/Paid Holiday Worked (Not Leave)
                    strQry.AppendLine(" AND (E.EARNING_NO IN (1,2,3,4,5)");
                    strQry.AppendLine(" OR (E.EARNING_NO > 6 AND E.EARNING_NO < 200)) ");
                }
                else
                {
                    strQry.AppendLine(" AND ((E.EARNING_NO NOT IN (1,3,4,5)");
                    strQry.AppendLine(" AND E.EARNING_NO < 200)");

                    //Overtime Wages
                    strQry.AppendLine(" OR (E.EARNING_NO = 3");
                    strQry.AppendLine(" AND C.OVERTIME1_RATE <> 0)");

                    strQry.AppendLine(" OR (E.EARNING_NO = 4");
                    strQry.AppendLine(" AND C.OVERTIME2_RATE <> 0)");

                    strQry.AppendLine(" OR (E.EARNING_NO = 5");
                    strQry.AppendLine(" AND C.OVERTIME3_RATE <> 0))");
                }

                //Leave
                strQry.AppendLine(" UNION");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" E.COMPANY_NO");
                strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",E.EARNING_NO");
                strQry.AppendLine(",E.LEAVE_PERCENTAGE");
                strQry.AppendLine(",E.EARNING_DESC");
                strQry.AppendLine(",E.IRP5_CODE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING E ");

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND E.EARNING_NO > 199 ");
                strQry.AppendLine(" AND E.LEAVE_PERCENTAGE > 0 ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" E.COMPANY_NO");
                strQry.AppendLine(",E.EARNING_NO");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EarningDesc", parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" ED.COMPANY_NO");
                strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EDC.EMPLOYEE_NO");
                
                //Errol Add
                strQry.AppendLine(",ISNULL(SUM(EDH.TOTAL),0) AS TOTAL_YTD_BF");
                //Errol Add

                strQry.AppendLine(",EDC.TOTAL");
                strQry.AppendLine(",EDC.TOTAL_ORIGINAL");
                strQry.AppendLine(",D.DEDUCTION_NO");
                strQry.AppendLine(",D.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",D.IRP5_CODE");
                strQry.AppendLine(",ED.DEDUCTION_VALUE");
                strQry.AppendLine(",ED.DEDUCTION_MIN_VALUE");
                strQry.AppendLine(",ED.DEDUCTION_MAX_VALUE");
                strQry.AppendLine(",ED.DEDUCTION_TYPE_IND");
                strQry.AppendLine(",ED.DEDUCTION_PERIOD_IND");
                strQry.AppendLine(",ED.DEDUCTION_DAY_VALUE");
                strQry.AppendLine(",D.DEDUCTION_LOAN_TYPE_IND");
                strQry.AppendLine(",'' AS DEDUCTION_NOT_LINKED");
                strQry.AppendLine(",CONVERT(DECIMAL,0) AS TOTAL_OUTSTANDING_LOAN");
                strQry.AppendLine(",CONVERT(DECIMAL,0) AS TOTAL_OUTSTANDING_LOAN_PENDING");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT EDC");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ED");
                strQry.AppendLine(" ON EDC.COMPANY_NO = ED.COMPANY_NO");
                strQry.AppendLine(" AND EDC.PAY_CATEGORY_TYPE = ED.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EDC.EMPLOYEE_NO = ED.EMPLOYEE_NO");
                strQry.AppendLine(" AND EDC.DEDUCTION_NO = ED.DEDUCTION_NO");
                strQry.AppendLine(" AND EDC.DEDUCTION_SUB_ACCOUNT_NO = ED.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(" AND ED.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.DEDUCTION D");
                strQry.AppendLine(" ON EDC.COMPANY_NO = D.COMPANY_NO");
                strQry.AppendLine(" AND EDC.PAY_CATEGORY_TYPE = D.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EDC.DEDUCTION_NO = D.DEDUCTION_NO");
                strQry.AppendLine(" AND EDC.DEDUCTION_SUB_ACCOUNT_NO = D.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON EDC.COMPANY_NO = E.COMPANY_NO");
                strQry.AppendLine(" AND EDC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EDC.EMPLOYEE_NO = E.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY EDH");
                strQry.AppendLine(" ON EDC.COMPANY_NO = EDH.COMPANY_NO ");
                //2017-02-18 - Removed to Cater for Employee Change of PAY_CATEGORY_TYPE
                //strQry.AppendLine(" AND EDC.PAY_CATEGORY_TYPE = EDH.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EDC.EMPLOYEE_NO = EDH.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EDC.DEDUCTION_NO = EDH.DEDUCTION_NO ");
                strQry.AppendLine(" AND EDC.DEDUCTION_SUB_ACCOUNT_NO = EDH.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(" AND EDH.PAY_PERIOD_DATE >= '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");

                strQry.AppendLine(" WHERE EDC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EDC.RUN_TYPE = 'P'");
                strQry.AppendLine(" AND EDC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                                
                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" ED.COMPANY_NO");
                strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EDC.EMPLOYEE_NO");
                strQry.AppendLine(",EDC.TOTAL");
                strQry.AppendLine(",EDC.TOTAL_ORIGINAL");
                strQry.AppendLine(",D.DEDUCTION_NO");
                strQry.AppendLine(",D.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",D.IRP5_CODE");
                strQry.AppendLine(",ED.DEDUCTION_VALUE");
                strQry.AppendLine(",ED.DEDUCTION_MIN_VALUE");
                strQry.AppendLine(",ED.DEDUCTION_MAX_VALUE");
                strQry.AppendLine(",ED.DEDUCTION_TYPE_IND");
                strQry.AppendLine(",ED.DEDUCTION_PERIOD_IND");
                strQry.AppendLine(",ED.DEDUCTION_DAY_VALUE");
                strQry.AppendLine(",D.DEDUCTION_LOAN_TYPE_IND");

                //Include Employees with Previous Deduction Balance but has NO Current DEDUCTION lINK
                strQry.AppendLine(" UNION");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" ED.COMPANY_NO");
                strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EDC.EMPLOYEE_NO");
                strQry.AppendLine(",ISNULL(SUM(EDH.TOTAL),0) AS TOTAL_YTD_BF");
                strQry.AppendLine(",EDC.TOTAL");
                strQry.AppendLine(",EDC.TOTAL_ORIGINAL");
                strQry.AppendLine(",D.DEDUCTION_NO");
                strQry.AppendLine(",D.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",D.IRP5_CODE");
                strQry.AppendLine(",0 AS DEDUCTION_VALUE");
                strQry.AppendLine(",ED.DEDUCTION_MIN_VALUE");
                strQry.AppendLine(",ED.DEDUCTION_MAX_VALUE");
                //Set To Fixed Value 
                //ERROL cHANGED 2 = F
                strQry.AppendLine(",'F' AS DEDUCTION_TYPE_IND");
                strQry.AppendLine(",ED.DEDUCTION_PERIOD_IND");
                strQry.AppendLine(",ED.DEDUCTION_DAY_VALUE");
                strQry.AppendLine(",D.DEDUCTION_LOAN_TYPE_IND");
                strQry.AppendLine(",'Y' AS DEDUCTION_NOT_LINKED");
                strQry.AppendLine(",CONVERT(DECIMAL,0) AS TOTAL_OUTSTANDING_LOAN");
                strQry.AppendLine(",CONVERT(DECIMAL,0) AS TOTAL_OUTSTANDING_LOAN_PENDING");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT EDC");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ED");
                strQry.AppendLine(" ON EDC.COMPANY_NO = ED.COMPANY_NO");
                strQry.AppendLine(" AND EDC.PAY_CATEGORY_TYPE = ED.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EDC.DEDUCTION_NO = ED.DEDUCTION_NO");
                strQry.AppendLine(" AND EDC.DEDUCTION_SUB_ACCOUNT_NO = ED.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(" AND ED.EMPLOYEE_NO = 0");
                strQry.AppendLine(" AND ED.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.DEDUCTION D");
                strQry.AppendLine(" ON EDC.COMPANY_NO = D.COMPANY_NO");
                strQry.AppendLine(" AND EDC.PAY_CATEGORY_TYPE = D.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EDC.DEDUCTION_NO = D.DEDUCTION_NO");
                strQry.AppendLine(" AND EDC.DEDUCTION_SUB_ACCOUNT_NO = D.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON EDC.COMPANY_NO = E.COMPANY_NO");
                strQry.AppendLine(" AND EDC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EDC.EMPLOYEE_NO = E.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY EDH");
                strQry.AppendLine(" ON E.COMPANY_NO = EDH.COMPANY_NO ");
                //2017-02-18 - Removed to Cater for Employee Change of PAY_CATEGORY_TYPE
                //strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EDH.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EDH.EMPLOYEE_NO ");
                strQry.AppendLine(" AND D.DEDUCTION_NO = EDH.DEDUCTION_NO ");
                strQry.AppendLine(" AND D.DEDUCTION_SUB_ACCOUNT_NO = EDH.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(" AND EDH.PAY_PERIOD_DATE >= '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");

                strQry.AppendLine(" WHERE EDC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EDC.RUN_TYPE = 'P'");
                strQry.AppendLine(" AND EDC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                
                strQry.AppendLine(" AND CONVERT(CHAR(10),EDC.EMPLOYEE_NO) + CONVERT(CHAR(10),EDC.DEDUCTION_NO) + CONVERT(CHAR(10),EDC.DEDUCTION_SUB_ACCOUNT_NO) NOT IN ");
                strQry.AppendLine("(SELECT ");
                strQry.AppendLine(" CONVERT(CHAR(10),EMPLOYEE_NO) + CONVERT(CHAR(10),DEDUCTION_NO) + CONVERT(CHAR(10),DEDUCTION_SUB_ACCOUNT_NO)");
                
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ");
                
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL)");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" ED.COMPANY_NO");
                strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EDC.EMPLOYEE_NO");
                strQry.AppendLine(",EDC.TOTAL");
                strQry.AppendLine(",EDC.TOTAL_ORIGINAL");
                strQry.AppendLine(",D.DEDUCTION_NO");
                strQry.AppendLine(",D.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",D.IRP5_CODE");
                strQry.AppendLine(",ED.DEDUCTION_MIN_VALUE");
                strQry.AppendLine(",ED.DEDUCTION_MAX_VALUE");
                strQry.AppendLine(",ED.DEDUCTION_PERIOD_IND");
                strQry.AppendLine(",ED.DEDUCTION_DAY_VALUE");
                strQry.AppendLine(",D.DEDUCTION_LOAN_TYPE_IND");

                strQry.AppendLine(" ORDER BY 2,3,7,8");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeDeduction", parInt64CompanyNo);

                //Processed Loans or Next Run Loans (This Run)
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" E.EMPLOYEE_NO");
                strQry.AppendLine(",L.DEDUCTION_NO");
                strQry.AppendLine(",L.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",SUM(L.LOAN_AMOUNT_PAID - L.LOAN_AMOUNT_RECEIVED) AS TOTAL_OUTSTANDING_LOAN");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LOANS L");
                strQry.AppendLine(" ON E.COMPANY_NO = L.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = L.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = L.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND (NOT L.LOAN_PROCESSED_DATE IS NULL");
                strQry.AppendLine(" OR (L.LOAN_PROCESSED_DATE IS NULL");
                strQry.AppendLine(" AND L.PROCESS_NO = 0))");
                strQry.AppendLine(" AND L.DATETIME_DELETE_RECORD IS NULL");
                
                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                
                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" E.EMPLOYEE_NO");
                strQry.AppendLine(",L.DEDUCTION_NO");
                strQry.AppendLine(",L.DEDUCTION_SUB_ACCOUNT_NO");

                strQry.AppendLine(" HAVING SUM(L.LOAN_AMOUNT_PAID - L.LOAN_AMOUNT_RECEIVED) <> 0");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

                for (int intRow = 0; intRow < DataSet.Tables["Temp"].Rows.Count; intRow++)
                {
                    EmployeeDeductionDataView = null;
                    EmployeeDeductionDataView = new DataView(DataSet.Tables["EmployeeDeduction"],
                    "EMPLOYEE_NO = " + DataSet.Tables["Temp"].Rows[intRow]["EMPLOYEE_NO"].ToString() + " AND DEDUCTION_NO = " + DataSet.Tables["Temp"].Rows[intRow]["DEDUCTION_NO"].ToString() + " AND DEDUCTION_SUB_ACCOUNT_NO = " + DataSet.Tables["Temp"].Rows[intRow]["DEDUCTION_SUB_ACCOUNT_NO"].ToString(),
                    "",
                    DataViewRowState.CurrentRows);

                    if (EmployeeDeductionDataView.Count > 0)
                    {
                        EmployeeDeductionDataView[0]["TOTAL_OUTSTANDING_LOAN"] = Convert.ToDouble(DataSet.Tables["Temp"].Rows[intRow]["TOTAL_OUTSTANDING_LOAN"]);
                    }
                }

                DataSet.Tables.Remove("Temp");

                //Loans Pending or Not this Run Loans
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" E.EMPLOYEE_NO");
                strQry.AppendLine(",L.DEDUCTION_NO");
                strQry.AppendLine(",L.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",SUM(L.LOAN_AMOUNT_PAID - L.LOAN_AMOUNT_RECEIVED) AS TOTAL_OUTSTANDING_LOAN_PENDING");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LOANS L");
                strQry.AppendLine(" ON E.COMPANY_NO = L.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = L.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = L.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND L.LOAN_PROCESSED_DATE IS NULL");
                strQry.AppendLine(" AND L.PROCESS_NO <> 0");
                strQry.AppendLine(" AND L.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                
                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" E.EMPLOYEE_NO");
                strQry.AppendLine(",L.DEDUCTION_NO");
                strQry.AppendLine(",L.DEDUCTION_SUB_ACCOUNT_NO");

                strQry.AppendLine(" HAVING SUM(L.LOAN_AMOUNT_PAID - L.LOAN_AMOUNT_RECEIVED) <> 0");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

                for (int intRow = 0; intRow < DataSet.Tables["Temp"].Rows.Count; intRow++)
                {
                    EmployeeDeductionDataView = null;
                    EmployeeDeductionDataView = new DataView(DataSet.Tables["EmployeeDeduction"],
                    "EMPLOYEE_NO = " + DataSet.Tables["Temp"].Rows[intRow]["EMPLOYEE_NO"].ToString() + " AND DEDUCTION_NO = " + DataSet.Tables["Temp"].Rows[intRow]["DEDUCTION_NO"].ToString() + " AND DEDUCTION_SUB_ACCOUNT_NO = " + DataSet.Tables["Temp"].Rows[intRow]["DEDUCTION_SUB_ACCOUNT_NO"].ToString(),
                    "",
                    DataViewRowState.CurrentRows);

                    if (EmployeeDeductionDataView.Count > 0)
                    {
                        EmployeeDeductionDataView[0]["TOTAL_OUTSTANDING_LOAN_PENDING"] = Convert.ToDouble(DataSet.Tables["Temp"].Rows[intRow]["TOTAL_OUTSTANDING_LOAN_PENDING"]);
                    }
                }

                DataSet.Tables.Remove("Temp");

                //Income / Overtime / Bonus / Paid Holiday Income
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EEC.COMPANY_NO");
                strQry.AppendLine(",EEC.EMPLOYEE_NO");
                strQry.AppendLine(",EEC.EARNING_NO");
                strQry.AppendLine(",EEC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EEC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EN.IRP5_CODE");
                strQry.AppendLine(",'' AS EARNING_TYPE_IND");
                strQry.AppendLine(",EEC.HOURS_DECIMAL");
                strQry.AppendLine(",EEC.HOURS_DECIMAL_ORIGINAL ");
                strQry.AppendLine(",EEC.HOURS_DECIMAL_OTHER_VALUE");
                //2016-01-08 HOURS_DECIMAL_OTHER_VALUE_ZERO Stores value of Leave Paid out minus Current Month's Accum Leave
                strQry.AppendLine(",EEC.HOURS_DECIMAL_OTHER_VALUE_ZERO");

                strQry.AppendLine(",EEC.MINUTES_ROUNDED");
                strQry.AppendLine(",ISNULL(SUM(EEH.TOTAL),0) + ISNULL(SUM(EEH_Bonus.TOTAL),0) AS TOTAL_YTD_BF");
                strQry.AppendLine(",EEC.TOTAL");
                strQry.AppendLine(",EEC.TOTAL_ORIGINAL");
                //Double Cheque for Salaries
                strQry.AppendLine(",EPC.HOURLY_RATE AS TOTAL_DOUBLE");
                strQry.AppendLine(",'N' AS EARNING_NOT_LINKED");
                strQry.AppendLine(",'' AS LEAVE_OPTION");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON EEC.COMPANY_NO = EPC.COMPANY_NO");
                strQry.AppendLine(" AND EEC.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
                //Errol Added 2012-08-27
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");

                strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");
         
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C");
                strQry.AppendLine(" ON EEC.COMPANY_NO = C.COMPANY_NO");
                strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EN");
                strQry.AppendLine(" ON EEC.COMPANY_NO = EN.COMPANY_NO");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EEC.EARNING_NO = EN.EARNING_NO");
                strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON EEC.COMPANY_NO = E.COMPANY_NO");
                strQry.AppendLine(" AND EEC.EMPLOYEE_NO = E.EMPLOYEE_NO");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                
                //Errol Add
                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
                strQry.AppendLine(" ON EEC.COMPANY_NO = EEH.COMPANY_NO ");
                strQry.AppendLine(" AND EEC.EMPLOYEE_NO = EEH.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EEC.EARNING_NO = EEH.EARNING_NO ");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_NO = EEH.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EEH.PAY_PERIOD_DATE >= '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");
                //Errol Add

                //2017-02-18 - Include Bonus YTD for Employee change of PAY_CATEGORY_TYPE
                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH_Bonus");
                strQry.AppendLine(" ON EEC.COMPANY_NO = EEH_Bonus.COMPANY_NO ");
                strQry.AppendLine(" AND EEC.EMPLOYEE_NO = EEH_Bonus.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EEC.EARNING_NO = EEH_Bonus.EARNING_NO ");
                //Bonus
                strQry.AppendLine(" AND EEH_Bonus.EARNING_NO = 7 ");

                if (parstrPayrollType == "W")
                {
                    strQry.AppendLine(" AND EEH_Bonus.PAY_CATEGORY_TYPE = 'S' ");

                }
                else
                {
                    strQry.AppendLine(" AND EEH_Bonus.PAY_CATEGORY_TYPE = 'W' ");
                }
                
                strQry.AppendLine(" AND EEH_Bonus.PAY_PERIOD_DATE >= '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");
                //2017-02-18 - Include Bonus YTD for Employee change of PAY_CATEGORY_TYPE

                strQry.AppendLine(" WHERE EEC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND EEC.RUN_TYPE = 'P'");

                strQry.AppendLine(" AND (EEC.EARNING_NO IN (1,2,3,7,8,9)");

                strQry.AppendLine(" OR (EEC.EARNING_NO = 4");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND C.SALARY_OVERTIME2_RATE <> 0)");

                strQry.AppendLine(" OR (EEC.EARNING_NO = 5");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND C.SALARY_OVERTIME3_RATE <> 0)");

                strQry.AppendLine(" OR (EEC.EARNING_NO = 4");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND C.OVERTIME2_RATE <> 0)");

                strQry.AppendLine(" OR (EEC.EARNING_NO = 5");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND C.OVERTIME3_RATE <> 0))");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" EEC.COMPANY_NO");
                strQry.AppendLine(",EEC.EMPLOYEE_NO");
                strQry.AppendLine(",EEC.EARNING_NO");
                strQry.AppendLine(",EEC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EEC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EN.IRP5_CODE");
                strQry.AppendLine(",EEC.HOURS_DECIMAL");
                strQry.AppendLine(",EEC.HOURS_DECIMAL_ORIGINAL ");
                strQry.AppendLine(",EEC.HOURS_DECIMAL_OTHER_VALUE");
                //2016-01-08 HOURS_DECIMAL_OTHER_VALUE_ZERO Stores value of Leave Paid out minus Current Month's Accum Leave
                strQry.AppendLine(",EEC.HOURS_DECIMAL_OTHER_VALUE_ZERO");
                strQry.AppendLine(",EEC.MINUTES_ROUNDED");
                strQry.AppendLine(",EEC.TOTAL");
                strQry.AppendLine(",EEC.TOTAL_ORIGINAL");
                strQry.AppendLine(",EPC.HOURLY_RATE");
                //ELR - 2015-03-17
                //strQry.AppendLine(",EN.TAX_PERCENTAGE");

                strQry.AppendLine(" UNION ");

                //Linked Earnings - Exclude all Leave
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EEC.COMPANY_NO");
                strQry.AppendLine(",EEC.EMPLOYEE_NO");
                strQry.AppendLine(",EEC.EARNING_NO");
                strQry.AppendLine(",EEC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EEC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EN.IRP5_CODE");
                //Errol Added - To Find Macros
                strQry.AppendLine(",ISNULL(EME.EARNING_TYPE_IND,'') AS EARNING_TYPE_IND");
                strQry.AppendLine(",EEC.HOURS_DECIMAL");
                strQry.AppendLine(",EEC.HOURS_DECIMAL_ORIGINAL ");
                strQry.AppendLine(",EEC.HOURS_DECIMAL_OTHER_VALUE");
                //2016-01-08 HOURS_DECIMAL_OTHER_VALUE_ZERO Stores value of Leave Paid out minus Current Month's Accum Leave
                strQry.AppendLine(",EEC.HOURS_DECIMAL_OTHER_VALUE_ZERO");
                strQry.AppendLine(",EEC.MINUTES_ROUNDED");
                strQry.AppendLine(",ISNULL(SUM(EEH.TOTAL),0) AS TOTAL_YTD_BF");
                strQry.AppendLine(",EEC.TOTAL");
                strQry.AppendLine(",EEC.TOTAL_ORIGINAL");
                strQry.AppendLine(",0 AS TOTAL_DOUBLE");
                strQry.AppendLine(",'N' AS EARNING_NOT_LINKED");
                strQry.AppendLine(",'' AS LEAVE_OPTION");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EE");
                strQry.AppendLine(" ON EEC.COMPANY_NO = EE.COMPANY_NO");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = EE.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EEC.EMPLOYEE_NO = EE.EMPLOYEE_NO");
                strQry.AppendLine(" AND EEC.EARNING_NO = EE.EARNING_NO");
                strQry.AppendLine(" AND EE.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EN");
                strQry.AppendLine(" ON EEC.COMPANY_NO = EN.COMPANY_NO");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EEC.EARNING_NO = EN.EARNING_NO");
                strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON EEC.COMPANY_NO = E.COMPANY_NO");
                strQry.AppendLine(" AND EEC.EMPLOYEE_NO = E.EMPLOYEE_NO");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                
                //ERROL ADDING
                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EME");
                strQry.AppendLine(" ON EEC.COMPANY_NO = EME.COMPANY_NO");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = EME.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EME.EMPLOYEE_NO = 0");
                strQry.AppendLine(" AND EEC.EARNING_NO = EME.EARNING_NO");
                strQry.AppendLine(" AND EME.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
                strQry.AppendLine(" ON EEC.COMPANY_NO = EEH.COMPANY_NO ");
                strQry.AppendLine(" AND EEC.EMPLOYEE_NO = EEH.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EEC.EARNING_NO = EEH.EARNING_NO ");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_NO = EEH.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EEH.PAY_PERIOD_DATE >= '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");

                strQry.AppendLine(" WHERE EEC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND EEC.RUN_TYPE = 'P'");

                strQry.AppendLine(" AND EEC.EARNING_NO < 200");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" EEC.COMPANY_NO");
                strQry.AppendLine(",EEC.EMPLOYEE_NO");
                strQry.AppendLine(",EEC.EARNING_NO");
                strQry.AppendLine(",EEC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EEC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EN.IRP5_CODE");
                //Errol Added - To Find Macros
                strQry.AppendLine(",ISNULL(EME.EARNING_TYPE_IND,'')");
                strQry.AppendLine(",EEC.HOURS_DECIMAL");
                strQry.AppendLine(",EEC.HOURS_DECIMAL_ORIGINAL ");
                strQry.AppendLine(",EEC.HOURS_DECIMAL_OTHER_VALUE");
                //2016-01-08 HOURS_DECIMAL_OTHER_VALUE_ZERO Stores value of Leave Paid out minus Current Month's Accum Leave
                strQry.AppendLine(",EEC.HOURS_DECIMAL_OTHER_VALUE_ZERO");
                strQry.AppendLine(",EEC.MINUTES_ROUNDED");
                strQry.AppendLine(",EEC.TOTAL");
                strQry.AppendLine(",EEC.TOTAL_ORIGINAL");
              
                strQry.AppendLine(" UNION ");

                //Include Leave when Initialised
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EEC.COMPANY_NO");
                strQry.AppendLine(",EEC.EMPLOYEE_NO");
                strQry.AppendLine(",EEC.EARNING_NO");
                strQry.AppendLine(",EEC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EEC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EN.IRP5_CODE");
                strQry.AppendLine(",'' AS EARNING_TYPE_IND");
                strQry.AppendLine(",EEC.HOURS_DECIMAL");
                strQry.AppendLine(",EEC.HOURS_DECIMAL_ORIGINAL ");
                //NB For Normal Leave This Value Holds the Close Balance of an Employee (All Paid Out)
                strQry.AppendLine(",EEC.HOURS_DECIMAL_OTHER_VALUE");
                //2016-01-08 HOURS_DECIMAL_OTHER_VALUE_ZERO Stores value of Leave Paid out minus Current Month's Accum Leave
                strQry.AppendLine(",EEC.HOURS_DECIMAL_OTHER_VALUE_ZERO");
                strQry.AppendLine(",EEC.MINUTES_ROUNDED");
                strQry.AppendLine(",ISNULL(SUM(EEH.TOTAL),0) AS TOTAL_YTD_BF");
                strQry.AppendLine(",EEC.TOTAL");
                strQry.AppendLine(",EEC.TOTAL_ORIGINAL");
                strQry.AppendLine(",0 AS TOTAL_DOUBLE");
                strQry.AppendLine(",'N' AS EARNING_NOT_LINKED");
                strQry.AppendLine(",ISNULL(LC.LEAVE_OPTION,'') AS LEAVE_OPTION");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EN");
                strQry.AppendLine(" ON EEC.COMPANY_NO = EN.COMPANY_NO");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EEC.EARNING_NO = EN.EARNING_NO");
                strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON EEC.COMPANY_NO = E.COMPANY_NO");
                strQry.AppendLine(" AND EEC.EMPLOYEE_NO = E.EMPLOYEE_NO");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                             
                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
                strQry.AppendLine(" ON EEC.COMPANY_NO = EEH.COMPANY_NO ");
                strQry.AppendLine(" AND EEC.EMPLOYEE_NO = EEH.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EEC.EARNING_NO = EEH.EARNING_NO ");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_NO = EEH.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EEH.PAY_PERIOD_DATE >= '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT LC");
                strQry.AppendLine(" ON EEC.COMPANY_NO = LC.COMPANY_NO");
                strQry.AppendLine(" AND EEC.EMPLOYEE_NO = LC.EMPLOYEE_NO");
                strQry.AppendLine(" AND EEC.EARNING_NO = LC.EARNING_NO");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = LC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND LC.EARNING_NO = 200");
                strQry.AppendLine(" AND LC.PROCESS_NO = 0");
                strQry.AppendLine(" AND LC.LEAVE_OPTION = 'P'");

                strQry.AppendLine(" WHERE EEC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND EEC.RUN_TYPE = 'P'");
                strQry.AppendLine(" AND EEC.EARNING_NO > 199");
    
                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" EEC.COMPANY_NO");
                strQry.AppendLine(",EEC.EMPLOYEE_NO");
                strQry.AppendLine(",EEC.EARNING_NO");
                strQry.AppendLine(",EEC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EEC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EN.IRP5_CODE");
                strQry.AppendLine(",EEC.HOURS_DECIMAL");
                strQry.AppendLine(",EEC.HOURS_DECIMAL_ORIGINAL ");
                strQry.AppendLine(",EEC.HOURS_DECIMAL_OTHER_VALUE");
                //2016-01-08 HOURS_DECIMAL_OTHER_VALUE_ZERO Stores value of Leave Paid out minus Current Month's Accum Leave
                strQry.AppendLine(",EEC.HOURS_DECIMAL_OTHER_VALUE_ZERO");
                strQry.AppendLine(",EEC.MINUTES_ROUNDED");
                strQry.AppendLine(",EEC.TOTAL");
                strQry.AppendLine(",EEC.TOTAL_ORIGINAL");
                strQry.AppendLine(",LC.LEAVE_OPTION");
                
                //NB NB NB NB Needs to be Looked At
                //NB NB NB NB Needs to be Looked At
                strQry.AppendLine(" UNION");

                //Earnings NOT Currently Linked
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EEC.COMPANY_NO");
                strQry.AppendLine(",EEC.EMPLOYEE_NO");
                strQry.AppendLine(",EEC.EARNING_NO");
                strQry.AppendLine(",EEC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EEC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EN.IRP5_CODE");
                //Errol Added - To Find Macros
                strQry.AppendLine(",ISNULL(EME.EARNING_TYPE_IND,'') AS EARNING_TYPE_IND");
                strQry.AppendLine(",EEC.HOURS_DECIMAL");
                strQry.AppendLine(",EEC.HOURS_DECIMAL_ORIGINAL ");
                strQry.AppendLine(",EEC.HOURS_DECIMAL_OTHER_VALUE");
                //2016-01-08 HOURS_DECIMAL_OTHER_VALUE_ZERO Stores value of Leave Paid out minus Current Month's Accum Leave
                strQry.AppendLine(",EEC.HOURS_DECIMAL_OTHER_VALUE_ZERO");
                strQry.AppendLine(",EEC.MINUTES_ROUNDED");
                strQry.AppendLine(",ISNULL(SUM(EEH.TOTAL),0) AS TOTAL_YTD_BF");
                strQry.AppendLine(",EEC.TOTAL");
                strQry.AppendLine(",EEC.TOTAL_ORIGINAL");
                strQry.AppendLine(",0 AS TOTAL_DOUBLE");
                strQry.AppendLine(",'Y' AS EARNING_NOT_LINKED");
                strQry.AppendLine(",'' AS LEAVE_OPTION");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EN");
                strQry.AppendLine(" ON EEC.COMPANY_NO = EN.COMPANY_NO");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EEC.EARNING_NO = EN.EARNING_NO");
                strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL");

                //ERROL ADDING
                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EME");
                strQry.AppendLine(" ON EEC.COMPANY_NO = EME.COMPANY_NO");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = EME.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EEC.EMPLOYEE_NO = 0");
                strQry.AppendLine(" AND EEC.EARNING_NO = EME.EARNING_NO");
                strQry.AppendLine(" AND EME.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON EEC.COMPANY_NO = E.COMPANY_NO");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EEC.EMPLOYEE_NO = E.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                
                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
                strQry.AppendLine(" ON EEC.COMPANY_NO = EEH.COMPANY_NO ");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_NO = EEH.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EEC.EMPLOYEE_NO = EEH.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EEC.EARNING_NO = EEH.EARNING_NO ");
                strQry.AppendLine(" AND EEH.PAY_PERIOD_DATE >= '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");

                strQry.AppendLine(" WHERE EEC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND EEC.RUN_TYPE = 'P'");
                strQry.AppendLine(" AND EEC.EARNING_NO > 9");

                //Errol Added 2012-01-10
                strQry.AppendLine(" AND EEC.EARNING_NO < 200");

                //Errol Removed 2012-01-10
                //strQry.AppendLine(" AND EEC.EARNING_NO NOT IN (200,201)");

                strQry.AppendLine(" AND CONVERT(CHAR(10),EEC.EMPLOYEE_NO) + CONVERT(CHAR(10),EEC.EARNING_NO) NOT IN ");
                strQry.AppendLine("(SELECT ");
                strQry.AppendLine(" CONVERT(CHAR(10),EMPLOYEE_NO) + CONVERT(CHAR(10),EARNING_NO)");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING ");
                
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND EARNING_NO > 9");

                //Errol Added 2012-01-10
                strQry.AppendLine(" AND EARNING_NO < 200)");

                //Errol Removed 2012-01-10
                //strQry.AppendLine(" AND EARNING_NO NOT IN (200,201))");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" EEC.COMPANY_NO");
                strQry.AppendLine(",EEC.EMPLOYEE_NO");
                strQry.AppendLine(",EEC.EARNING_NO");
                strQry.AppendLine(",EEC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EEC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EN.IRP5_CODE");
                strQry.AppendLine(",ISNULL(EME.EARNING_TYPE_IND,'')");
                strQry.AppendLine(",EEC.HOURS_DECIMAL");
                strQry.AppendLine(",EEC.HOURS_DECIMAL_ORIGINAL ");
                strQry.AppendLine(",EEC.HOURS_DECIMAL_OTHER_VALUE");
                //2016-01-08 HOURS_DECIMAL_OTHER_VALUE_ZERO Stores value of Leave Paid out minus Current Month's Accum Leave
                strQry.AppendLine(",EEC.HOURS_DECIMAL_OTHER_VALUE_ZERO");
                strQry.AppendLine(",EEC.MINUTES_ROUNDED");
                strQry.AppendLine(",EEC.TOTAL");
                strQry.AppendLine(",EEC.TOTAL_ORIGINAL");
              
                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeEarning", parInt64CompanyNo);

                //2017-02-18 Add on If Employee Changed Type - Start
                //2017-02-18 Add on If Employee Changed Type - Start
                //2017-02-18 Add on If Employee Changed Type - Start
                strQry.Clear();

                strQry.AppendLine(" SELECT DISTINCT");
                strQry.AppendLine(" C.COMPANY_DESC");
                strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
                strQry.AppendLine(",PCPC.PAY_PERIOD_DATE");
                strQry.AppendLine(",EEH.PAY_CATEGORY_NO");
                strQry.AppendLine(",EEH.COMPANY_NO");

                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(",C.OVERTIME1_RATE");
                    strQry.AppendLine(",C.OVERTIME2_RATE");
                    strQry.AppendLine(",C.OVERTIME3_RATE");
                }
                else
                {
                    strQry.AppendLine(",C.SALARY_OVERTIME1_RATE AS OVERTIME1_RATE");
                    strQry.AppendLine(",C.SALARY_OVERTIME2_RATE AS OVERTIME2_RATE");
                    strQry.AppendLine(",C.SALARY_OVERTIME3_RATE AS OVERTIME3_RATE");
                }

                strQry.AppendLine(",0 AS PAIDHOLIDAY_RATE");
                strQry.AppendLine(",0 AS SALARY_MINUTES_PAID_PER_DAY");
                strQry.AppendLine(",0 AS SALARY_DAYS_PER_YEAR");
                strQry.AppendLine(",0 AS TOTAL_DAILY_TIME_OVERTIME");
                
                if (parstrPayrollType == "W")
                {
                    strQry.AppendLine(",'S' AS PAY_CATEGORY_TYPE");
                }
                else
                {
                    strQry.AppendLine(",'W' AS PAY_CATEGORY_TYPE");
                }
                
                strQry.AppendLine(",C.SALARY_DOUBLE_CHEQUE_BIRTHDAY_IND");
                strQry.AppendLine(",'A' AS ACCESS_IND");

                strQry.AppendLine(",C.WAGE_RUN_IND");
                strQry.AppendLine(",C.SALARY_RUN_IND");

                strQry.AppendLine(",'N' AS PUBLIC_HOLIDAY_IND");
              
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING EARN ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
                strQry.AppendLine(" ON EARN.COMPANY_NO = EEH.COMPANY_NO ");
                strQry.AppendLine(" AND EARN.EARNING_NO = EEH.EARNING_NO ");
                strQry.AppendLine(" AND EARN.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EEH.PAY_PERIOD_DATE >= '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");
                
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C ");
                strQry.AppendLine(" ON EARN.COMPANY_NO = C.COMPANY_NO ");
                strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL ");
                
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
                strQry.AppendLine(" ON EEH.COMPANY_NO = PC.COMPANY_NO ");
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0 ");
                strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
                
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");
                strQry.AppendLine(" ON EEH.COMPANY_NO = PCPC.COMPANY_NO ");
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE  = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON EEH.COMPANY_NO = E.COMPANY_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND EEH.EMPLOYEE_NO = E.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                strQry.AppendLine(" WHERE EARN.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EARN.DATETIME_DELETE_RECORD IS NULL ");

                if (parstrPayrollType == "W")
                {
                    strQry.AppendLine(" AND EARN.PAY_CATEGORY_TYPE = 'S' ");
                }
                else
                {
                    strQry.AppendLine(" AND EARN.PAY_CATEGORY_TYPE = 'W' ");
                }
                
                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" E.COMPANY_NO");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",EEH.EARNING_NO");
                strQry.AppendLine(",EEH.PAY_CATEGORY_NO");
                strQry.AppendLine(",EEH.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EARN.IRP5_CODE");
                //Errol Added - To Find Macros
                strQry.AppendLine(",'' AS EARNING_TYPE_IND");
                strQry.AppendLine(",0 AS HOURS_DECIMAL");
                strQry.AppendLine(",0 AS HOURS_DECIMAL_ORIGINAL ");
                strQry.AppendLine(",0 AS HOURS_DECIMAL_OTHER_VALUE");
                //2016-01-08 HOURS_DECIMAL_OTHER_VALUE_ZERO Stores value of Leave Paid out minus Current Month's Accum Leave
                strQry.AppendLine(",0 AS HOURS_DECIMAL_OTHER_VALUE_ZERO");
                strQry.AppendLine(",0 AS MINUTES_ROUNDED");
                strQry.AppendLine(",ISNULL(SUM(EEH.TOTAL),0) AS TOTAL_YTD_BF");
                strQry.AppendLine(",0 AS TOTAL");
                strQry.AppendLine(",0 AS TOTAL_ORIGINAL");
                strQry.AppendLine(",0 AS TOTAL_DOUBLE");
                strQry.AppendLine(",'Y' AS EARNING_NOT_LINKED");
                strQry.AppendLine(",'' AS LEAVE_OPTION");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");
                
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
                strQry.AppendLine(" ON E.COMPANY_NO = EEH.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EEH.EMPLOYEE_NO ");

                if (parstrPayrollType == "W")
                {
                    strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = 'S' ");
                }
                else
                {
                    strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = 'W' ");
                }

                //Exclude Bonus (Common and handled in Higher SQL)
                strQry.AppendLine(" AND EEH.EARNING_NO <> 7 ");

                strQry.AppendLine(" AND EEH.PAY_PERIOD_DATE >= '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");
                
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EARN");
                strQry.AppendLine(" ON E.COMPANY_NO = EARN.COMPANY_NO ");
                strQry.AppendLine(" AND EEH.EARNING_NO = EARN.EARNING_NO ");
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = EARN.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EARN.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
               
                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" E.COMPANY_NO");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",EEH.EARNING_NO");
                strQry.AppendLine(",EEH.PAY_CATEGORY_NO");
                strQry.AppendLine(",EEH.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EARN.IRP5_CODE");

                //strQry.AppendLine(" HAVING ISNULL(SUM(EEH.TOTAL),0) > 0 ");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeEarning", parInt64CompanyNo);
          
                strQry.Clear();
                strQry.AppendLine(" SELECT DISTINCT");
                strQry.AppendLine(" EARN.COMPANY_NO");
                strQry.AppendLine(",EARN.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EARN.EARNING_NO");
                strQry.AppendLine(",EARN.LEAVE_PERCENTAGE");
                strQry.AppendLine(",EARN.EARNING_DESC");
                strQry.AppendLine(",EARN.IRP5_CODE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING EARN ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
                strQry.AppendLine(" ON EARN.COMPANY_NO = EEH.COMPANY_NO ");
                strQry.AppendLine(" AND EARN.EARNING_NO = EEH.EARNING_NO ");
                strQry.AppendLine(" AND EARN.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EEH.PAY_PERIOD_DATE >= '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON EEH.COMPANY_NO = E.COMPANY_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND EEH.EMPLOYEE_NO = E.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                strQry.AppendLine(" WHERE EARN.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EARN.DATETIME_DELETE_RECORD IS NULL ");

                if (parstrPayrollType == "W")
                {
                    strQry.AppendLine(" AND EARN.PAY_CATEGORY_TYPE = 'S' ");
                }
                else
                {
                    strQry.AppendLine(" AND EARN.PAY_CATEGORY_TYPE = 'W' ");
                }

  
                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" EARN.COMPANY_NO");
                strQry.AppendLine(",EARN.EARNING_NO");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EarningDesc", parInt64CompanyNo);

                //2017-02-18 Add on If Employee Changed Type - End
                //2017-02-18 Add on If Employee Changed Type - End
                //2017-02-18 Add on If Employee Changed Type - End

                strQry.Clear();
                strQry.AppendLine(" SELECT DISTINCT");
                strQry.AppendLine(" LC.COMPANY_NO");
                strQry.AppendLine(",LC.EMPLOYEE_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT LC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON LC.COMPANY_NO = E.COMPANY_NO");
                strQry.AppendLine(" AND LC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND LC.EMPLOYEE_NO = E.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                strQry.AppendLine(" WHERE LC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND LC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND LC.EARNING_NO = 200");
                //NOT Next Run
                strQry.AppendLine(" AND LC.PROCESS_NO <> 0");
                
                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeNormalLeavePending", parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" E.COMPANY_NO");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",EPCC.RUN_TYPE");
                strQry.AppendLine(",EPCC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",D.IRP5_CODE");
                strQry.AppendLine(",DATEPART(yyyy,EDH.PAY_PERIOD_DATE) AS PERIOD_YEAR");
                strQry.AppendLine(",DATEPART(mm,EDH.PAY_PERIOD_DATE) AS PERIOD_MONTH");
                strQry.AppendLine(",SUM(EDH.TOTAL) AS HISTORY_TOTAL_VALUE");
                
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");
                
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC ");
                strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                //strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y' ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.DEDUCTION D ");
                strQry.AppendLine(" ON E.COMPANY_NO = D.COMPANY_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = D.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND D.IRP5_CODE IN (4001,4006,4005)");
                strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY EDH");
                strQry.AppendLine(" ON E.COMPANY_NO = EDH.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EDH.EMPLOYEE_NO ");
                strQry.AppendLine(" AND D.DEDUCTION_NO = EDH.DEDUCTION_NO ");
                strQry.AppendLine(" AND EDH.PAY_PERIOD_DATE >= '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");
                // NB RUN_TYPE = All

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" E.COMPANY_NO");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",EPCC.RUN_TYPE");
                strQry.AppendLine(",EPCC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",D.IRP5_CODE");
                strQry.AppendLine(",DATEPART(yyyy,EDH.PAY_PERIOD_DATE)");
                strQry.AppendLine(",DATEPART(mm,EDH.PAY_PERIOD_DATE)");

                strQry.AppendLine(" UNION ");
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" E.COMPANY_NO");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",EPCC.RUN_TYPE");
                strQry.AppendLine(",EPCC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EN.IRP5_CODE");
                strQry.AppendLine(",DATEPART(yyyy,EEH.PAY_PERIOD_DATE) AS PERIOD_YEAR");
                strQry.AppendLine(",DATEPART(mm,EEH.PAY_PERIOD_DATE) AS PERIOD_MONTH");
                strQry.AppendLine(",SUM(EEH.TOTAL) AS HISTORY_TOTAL_VALUE");
               
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC ");
                strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                //strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y' ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EN ");
                strQry.AppendLine(" ON E.COMPANY_NO = EN.COMPANY_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EN.IRP5_CODE IN (3810)");
                strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
                strQry.AppendLine(" ON E.COMPANY_NO = EEH.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EEH.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EN.EARNING_NO = EEH.EARNING_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EEH.PAY_PERIOD_DATE >= '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");
                // NB RUN_TYPE = All

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" E.COMPANY_NO");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",EPCC.RUN_TYPE");
                strQry.AppendLine(",EPCC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EN.IRP5_CODE");
                strQry.AppendLine(",DATEPART(yyyy,EEH.PAY_PERIOD_DATE)");
                strQry.AppendLine(",DATEPART(mm,EEH.PAY_PERIOD_DATE)");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "TaxSpreadSheet", parInt64CompanyNo);

                EmployeeDataView = null;
                EmployeeDataView = new DataView(DataSet.Tables["Employee"],
                    "",
                    "EMPLOYEE_NO",
                    DataViewRowState.CurrentRows);

                //Get TaxSpreadSheet For Current Month
                TaxSpreadSheetDataView = null;
                TaxSpreadSheetDataView = new DataView(DataSet.Tables["TaxSpreadSheet"],
                    "PERIOD_YEAR = " + Convert.ToDateTime(DataSet.Tables["Company"].Rows[0]["PAY_PERIOD_DATE"]).Year + " AND PERIOD_MONTH = " + Convert.ToDateTime(DataSet.Tables["Company"].Rows[0]["PAY_PERIOD_DATE"]).Month,
                    "EMPLOYEE_NO,IRP5_CODE",
                    DataViewRowState.CurrentRows);

                EmployeeDeductionDataView = null;
                EmployeeDeductionDataView = new DataView(DataSet.Tables["EmployeeDeduction"],
                    "IRP5_CODE IN (4001,4005,4006)",
                    "EMPLOYEE_NO,IRP5_CODE",
                    DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < EmployeeDeductionDataView.Count; intRow++)
                {
                    //Errol Added 2017-04-17
                    if (EmployeeDeductionDataView[intRow]["DEDUCTION_NO"].ToString() == "3"
                    || EmployeeDeductionDataView[intRow]["DEDUCTION_NO"].ToString() == "4")
                    {
                        objFind[0] = EmployeeDeductionDataView[intRow]["EMPLOYEE_NO"].ToString();
                        objFind[1] = EmployeeDeductionDataView[intRow]["IRP5_CODE"].ToString();

                        intFindRow = TaxSpreadSheetDataView.Find(objFind);

                        if (intFindRow == -1)
                        {
                            drvDataRowView = TaxSpreadSheetDataView.AddNew();

                            drvDataRowView["COMPANY_NO"] = parInt64CompanyNo;
                            drvDataRowView["EMPLOYEE_NO"] = EmployeeDeductionDataView[intRow]["EMPLOYEE_NO"].ToString();
                            drvDataRowView["RUN_TYPE"] = "P";
                            drvDataRowView["PAY_CATEGORY_TYPE"] = parstrPayrollType;
                            drvDataRowView["IRP5_CODE"] = EmployeeDeductionDataView[intRow]["IRP5_CODE"].ToString();
                            drvDataRowView["PERIOD_YEAR"] = Convert.ToDateTime(DataSet.Tables["Company"].Rows[0]["PAY_PERIOD_DATE"]).Year;
                            drvDataRowView["PERIOD_MONTH"] = Convert.ToDateTime(DataSet.Tables["Company"].Rows[0]["PAY_PERIOD_DATE"]).Month;
                            drvDataRowView["HISTORY_TOTAL_VALUE"] = Convert.ToDouble(EmployeeDeductionDataView[intRow]["TOTAL"]);

                            drvDataRowView.EndEdit();
                        }
                        else
                        {
                            TaxSpreadSheetDataView[intFindRow]["HISTORY_TOTAL_VALUE"] = Convert.ToDouble(EmployeeDeductionDataView[intRow]["TOTAL"]);
                        }
                    }
                }

                EmployeeEarningDataView = null;
                EmployeeEarningDataView = new DataView(DataSet.Tables["EmployeeEarning"],
                    "IRP5_CODE IN (3810)",
                    "EMPLOYEE_NO,IRP5_CODE",
                    DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < EmployeeEarningDataView.Count; intRow++)
                {
                    objFind[0] = EmployeeEarningDataView[intRow]["EMPLOYEE_NO"].ToString();
                    objFind[1] = EmployeeEarningDataView[intRow]["IRP5_CODE"].ToString();

                    intFindRow = TaxSpreadSheetDataView.Find(objFind);

                    if (intFindRow == -1)
                    {
                        drvDataRowView = TaxSpreadSheetDataView.AddNew();

                        drvDataRowView["COMPANY_NO"] = parInt64CompanyNo;
                        drvDataRowView["EMPLOYEE_NO"] = EmployeeEarningDataView[intRow]["EMPLOYEE_NO"].ToString();
                        drvDataRowView["RUN_TYPE"] = "P";
                        drvDataRowView["PAY_CATEGORY_TYPE"] = parstrPayrollType;
                        drvDataRowView["IRP5_CODE"] = EmployeeEarningDataView[intRow]["IRP5_CODE"].ToString();
                        drvDataRowView["PERIOD_YEAR"] = Convert.ToDateTime(DataSet.Tables["Company"].Rows[0]["PAY_PERIOD_DATE"]).Year;
                        drvDataRowView["PERIOD_MONTH"] = Convert.ToDateTime(DataSet.Tables["Company"].Rows[0]["PAY_PERIOD_DATE"]).Month;
                        drvDataRowView["HISTORY_TOTAL_VALUE"] = 0;
                        drvDataRowView["TOTAL_VALUE"] = Convert.ToDouble(EmployeeEarningDataView[intRow]["TOTAL"]);

                        drvDataRowView.EndEdit();
                    }
                    else
                    {
                        TaxSpreadSheetDataView[intFindRow]["TOTAL_VALUE"] = Convert.ToDouble(EmployeeEarningDataView[intRow]["TOTAL"]);
                    }

                    intFindRow = EmployeeDataView.Find(EmployeeEarningDataView[intRow]["EMPLOYEE_NO"].ToString());
                }
            }

            DataSet.AcceptChanges();

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public void Update_Records(Int64 parInt64CompanyNo, byte[] parbyteDataSet, string parstrPayrollType)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            StringBuilder strQry = new StringBuilder();

            for (int intRow = 0; intRow < parDataSet.Tables["Employee"].Rows.Count; intRow++)
            {
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" CLOSE_IND = " + this.clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[intRow]["CLOSE_IND"].ToString()));
                strQry.AppendLine(",CLOSE_REMOVE_EARNING_AND_LEAVE_IND = " + this.clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[intRow]["CLOSE_REMOVE_EARNING_AND_LEAVE_IND"].ToString()));
                strQry.AppendLine(",PAYSLIP_IND = " + this.clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[intRow]["PAYSLIP_IND"].ToString()));
                strQry.AppendLine(",EXTRA_CHEQUES_CURRENT = " + Convert.ToDouble(parDataSet.Tables["Employee"].Rows[intRow]["EXTRA_CHEQUES_CURRENT"]));

                strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToDouble(parDataSet.Tables["Employee"].Rows[intRow]["COMPANY_NO"]));
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + this.clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND EMPLOYEE_NO = " + Convert.ToDouble(parDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"]));
                strQry.AppendLine(" AND RUN_TYPE = 'P'");


                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            }

            for (int intRow = 0; intRow < parDataSet.Tables["EmployeeEarning"].Rows.Count; intRow++)
            {
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" TOTAL = " + Convert.ToDouble(parDataSet.Tables["EmployeeEarning"].Rows[intRow]["TOTAL"]));
                strQry.AppendLine(",HOURS_DECIMAL = " + Convert.ToDouble(parDataSet.Tables["EmployeeEarning"].Rows[intRow]["HOURS_DECIMAL"]));
                strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToDouble(parDataSet.Tables["EmployeeEarning"].Rows[intRow]["COMPANY_NO"]));
                strQry.AppendLine(" AND EARNING_NO = " + Convert.ToDouble(parDataSet.Tables["EmployeeEarning"].Rows[intRow]["EARNING_NO"]));
                strQry.AppendLine(" AND EMPLOYEE_NO = " + Convert.ToDouble(parDataSet.Tables["EmployeeEarning"].Rows[intRow]["EMPLOYEE_NO"]));
                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToDouble(parDataSet.Tables["EmployeeEarning"].Rows[intRow]["PAY_CATEGORY_NO"]));
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EmployeeEarning"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                strQry.AppendLine(" AND RUN_TYPE = 'P'");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            }

            for (int intRow = 0; intRow < parDataSet.Tables["EmployeeDeduction"].Rows.Count; intRow++)
            {
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" TOTAL = " + Convert.ToDouble(parDataSet.Tables["EmployeeDeduction"].Rows[intRow]["TOTAL"]));
                strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToDouble(parDataSet.Tables["EmployeeDeduction"].Rows[intRow]["COMPANY_NO"]));
                strQry.AppendLine(" AND EMPLOYEE_NO = " + Convert.ToDouble(parDataSet.Tables["EmployeeDeduction"].Rows[intRow]["EMPLOYEE_NO"]));
                strQry.AppendLine(" AND DEDUCTION_NO = " + Convert.ToDouble(parDataSet.Tables["EmployeeDeduction"].Rows[intRow]["DEDUCTION_NO"]));
                strQry.AppendLine(" AND DEDUCTION_SUB_ACCOUNT_NO = " + Convert.ToDouble(parDataSet.Tables["EmployeeDeduction"].Rows[intRow]["DEDUCTION_SUB_ACCOUNT_NO"]));
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + this.clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND RUN_TYPE = 'P'");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            }

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }
    }
}
