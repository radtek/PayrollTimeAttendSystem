using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace InteractPayroll
{
    public class busPayrollRunFix
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busPayrollRunFix()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parInt64CompanyNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            
            strQry.AppendLine(" PCPH.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",LS.NORM_PAID_PER_PERIOD");
            strQry.AppendLine(",LS.SICK_PAID_PER_PERIOD");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON PCPH.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            //NB This Could be a Problem (Earning Deleted?)
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");


            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT LS");
            strQry.AppendLine(" ON PCPH.COMPANY_NO = LS.COMPANY_NO");
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = LS.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND LS.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" WHERE PCPH.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = 'S'");
            strQry.AppendLine(" AND PCPH.PAY_PERIOD_DATE > '2015-02-28'");
            strQry.AppendLine(" AND PCPH.RUN_TYPE = 'P'");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PCPH.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",LS.NORM_PAID_PER_PERIOD");
            strQry.AppendLine(",LS.SICK_PAID_PER_PERIOD");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategory", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EEH.EARNING_NO");
            strQry.AppendLine(",EEH.PAY_CATEGORY_NO");
            strQry.AppendLine(",E.EARNING_REPORT_HEADER1");
            strQry.AppendLine(",E.EARNING_REPORT_HEADER2");
            strQry.AppendLine(",E.EARNING_DESC");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING E");
            strQry.AppendLine(" ON EEH.COMPANY_NO = E.COMPANY_NO");
            strQry.AppendLine(" AND EEH.EARNING_NO = E.EARNING_NO");
            strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
            //NB This Could be a Problem (Earning Deleted?)
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" WHERE EEH.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = 'S'");
            strQry.AppendLine(" AND EEH.PAY_PERIOD_DATE > '2015-02-28'");
            strQry.AppendLine(" AND EEH.RUN_TYPE = 'P'");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" EEH.EARNING_NO");
            strQry.AppendLine(",EEH.PAY_CATEGORY_NO");

            strQry.AppendLine(",E.EARNING_REPORT_HEADER1");
            strQry.AppendLine(",E.EARNING_REPORT_HEADER2");
            strQry.AppendLine(",E.EARNING_DESC");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" EEH.EARNING_NO");
            strQry.AppendLine(",EEH.PAY_CATEGORY_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EarningDesc", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EDH.DEDUCTION_NO");
            strQry.AppendLine(",EDH.DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",D.DEDUCTION_REPORT_HEADER1");
            strQry.AppendLine(",D.DEDUCTION_REPORT_HEADER2");
            strQry.AppendLine(",D.DEDUCTION_DESC");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY EDH");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.DEDUCTION D");
            strQry.AppendLine(" ON EDH.COMPANY_NO = D.COMPANY_NO");
            strQry.AppendLine(" AND EDH.DEDUCTION_NO = D.DEDUCTION_NO");
            strQry.AppendLine(" AND EDH.DEDUCTION_SUB_ACCOUNT_NO = D.DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(" AND EDH.PAY_CATEGORY_TYPE = D.PAY_CATEGORY_TYPE");
            //NB This Could be a Problem (Earning Deleted?)
            strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" WHERE EDH.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EDH.PAY_CATEGORY_TYPE = 'S'");
            strQry.AppendLine(" AND EDH.PAY_PERIOD_DATE > '2015-02-28'");
            strQry.AppendLine(" AND EDH.RUN_TYPE = 'P'");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" EDH.DEDUCTION_NO");
            strQry.AppendLine(",EDH.DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",D.DEDUCTION_REPORT_HEADER1");
            strQry.AppendLine(",D.DEDUCTION_REPORT_HEADER2");
            strQry.AppendLine(",D.DEDUCTION_DESC");

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" 500 AS DEDUCTION_NO");
            strQry.AppendLine(",1 AS DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",'Rate' AS DEDUCTION_REPORT_HEADER1");
            strQry.AppendLine(",'' AS DEDUCTION_REPORT_HEADER2");
            strQry.AppendLine(",'Hourly Rate' AS DEDUCTION_DESC");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            //strQry.AppendLine(" UNION ");

            //strQry.AppendLine(" SELECT ");
            //strQry.AppendLine(" 600 AS DEDUCTION_NO");
            //strQry.AppendLine(",1 AS DEDUCTION_SUB_ACCOUNT_NO");
            //strQry.AppendLine(",'Shifts' AS DEDUCTION_REPORT_HEADER1");
            //strQry.AppendLine(",'' AS DEDUCTION_REPORT_HEADER2");
            //strQry.AppendLine(",'Shifts' AS DEDUCTION_DESC");
            //strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY ");

            //strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 1");
            strQry.AppendLine(",2");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "DeductionDesc", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EEH.PAY_PERIOD_DATE");
            strQry.AppendLine(",EEH.EMPLOYEE_NO");
            strQry.AppendLine(",EEH.EARNING_NO");
            strQry.AppendLine(",EEH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EEH.PAY_CATEGORY_NO");
            strQry.AppendLine(",EEH.TOTAL");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");

            strQry.AppendLine(" WHERE EEH.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = 'S'");
            strQry.AppendLine(" AND EEH.PAY_PERIOD_DATE > '2015-02-28'");
            strQry.AppendLine(" AND EEH.RUN_TYPE = 'P'");
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 1");
            strQry.AppendLine(",2");
            strQry.AppendLine(",3");
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Earning", parInt64CompanyNo);
          
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PAY_PERIOD_DATE");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",TOTAL");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
            strQry.AppendLine(" AND PAY_PERIOD_DATE > '2015-02-28'");
            strQry.AppendLine(" AND RUN_TYPE = 'P'");

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EPCH.PAY_PERIOD_DATE");
            strQry.AppendLine(",EPCH.EMPLOYEE_NO");
           
            strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",500 AS DEDUCTION_NO");
            strQry.AppendLine(",1 AS DEDUCTION_SUB_ACCOUNT_NO");

            //Salaries
            //strQry.AppendLine(",EPCH.HOURLY_RATE AS TOTAL");

            //Wages
            strQry.AppendLine(",EPCH.SALARY_MONTH_PAYMENT AS TOTAL");

            //Wages
            //strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");

            //Salaries
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY EPCH");

            strQry.AppendLine(" WHERE EPCH.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = 'S'");
            strQry.AppendLine(" AND EPCH.PAY_PERIOD_DATE > '2015-02-28'");
            strQry.AppendLine(" AND EPCH.RUN_TYPE = 'P'");

            //strQry.AppendLine(" UNION ");

            //strQry.AppendLine(" SELECT ");
            //strQry.AppendLine(" EPCH.PAY_PERIOD_DATE");
            //strQry.AppendLine(",EPCH.EMPLOYEE_NO");
            //strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE");
            //strQry.AppendLine(",600 AS DEDUCTION_NO");
            //strQry.AppendLine(",1 AS DEDUCTION_SUB_ACCOUNT_NO");
            
            //strQry.AppendLine(",CURRENT_YEAR_LEAVE_SHIFTS_PER_RUN AS TOTAL");

            //strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY EIH");

            //strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");
            //strQry.AppendLine(" ON EIH.COMPANY_NO = EPCH.COMPANY_NO");
            //strQry.AppendLine(" AND EIH.PAY_PERIOD_DATE = EPCH.PAY_PERIOD_DATE");
            //strQry.AppendLine(" AND EIH.EMPLOYEE_NO = EPCH.EMPLOYEE_NO");
            //strQry.AppendLine(" AND EIH.PAY_CATEGORY_TYPE = EPCH.PAY_CATEGORY_TYPE");

            //strQry.AppendLine(" WHERE EIH.COMPANY_NO = " + parInt64CompanyNo);
            //strQry.AppendLine(" AND EIH.PAY_CATEGORY_TYPE = 'S'");
            //strQry.AppendLine(" AND EIH.PAY_PERIOD_DATE > '2015-02-28'");
            //strQry.AppendLine(" AND EIH.RUN_TYPE = 'P'");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 1");
            strQry.AppendLine(",2");
            strQry.AppendLine(",3");
            strQry.AppendLine(",4");
            strQry.AppendLine(",5");
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Deduction", parInt64CompanyNo);
      
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
            //NB This Could be a Problem (Earning Deleted?)
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");
           
            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
                        
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parInt64CompanyNo);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public void Update_Employee_Earnings_Deductions(Int64 parInt64CompanyNo, int parPayCategoryNo, string parNormPaidPerPeriod, string parSickPaidPerPeriod, byte[] patDataSetArray)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(patDataSetArray);

            StringBuilder strQry = new StringBuilder();

            for (int intRow = 0; intRow < parDataSet.Tables["Earning"].Rows.Count; intRow++)
            {
                if (parDataSet.Tables["Earning"].Rows[intRow].RowState == DataRowState.Added)
                {
                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY");
                    strQry.AppendLine("(COMPANY_NO ");
                    strQry.AppendLine(",PAY_PERIOD_DATE");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",RUN_TYPE");
                    strQry.AppendLine(",EARNING_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",RUN_NO");

                    strQry.AppendLine(",MINUTES");
                    strQry.AppendLine(",MINUTES_ROUNDED");
                    strQry.AppendLine(",HOURS_DECIMAL");
                    strQry.AppendLine(",HOURS_DECIMAL_OTHER_VALUE");
                    strQry.AppendLine(",HOURS_DECIMAL_ORIGINAL");
                    strQry.AppendLine(",DAY_DECIMAL_OTHER_VALUE");

                    strQry.AppendLine(",TOTAL)");

                    strQry.AppendLine(" VALUES ");

                    strQry.AppendLine("(" + parInt64CompanyNo);
                    strQry.AppendLine(",'" + Convert.ToDateTime(parDataSet.Tables["Earning"].Rows[intRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine("," + parDataSet.Tables["Earning"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                    strQry.AppendLine(",'P'");
                    strQry.AppendLine("," + parDataSet.Tables["Earning"].Rows[intRow]["EARNING_NO"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["Earning"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Earning"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine(",1");

                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");

                    strQry.AppendLine("," + parDataSet.Tables["Earning"].Rows[intRow]["TOTAL"].ToString() + ")");
                }
                else
                {
                    //Updated
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY");

                    strQry.AppendLine(" SET ");

                    strQry.AppendLine(" TOTAL = " + parDataSet.Tables["Earning"].Rows[intRow]["TOTAL"].ToString());
                    
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_PERIOD_DATE = '" + Convert.ToDateTime(parDataSet.Tables["Earning"].Rows[intRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + parDataSet.Tables["Earning"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                    strQry.AppendLine(" AND RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EARNING_NO = " + parDataSet.Tables["Earning"].Rows[intRow]["EARNING_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parDataSet.Tables["Earning"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Earning"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine(" AND RUN_NO = 1");
                }

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            }

            for (int intRow = 0; intRow < parDataSet.Tables["Deduction"].Rows.Count; intRow++)
            {
                strQry.Clear();

                if (parDataSet.Tables["Deduction"].Rows[intRow].RowState == DataRowState.Added)
                {
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY");
                    strQry.AppendLine("(COMPANY_NO ");
                    strQry.AppendLine(",PAY_PERIOD_DATE");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",RUN_TYPE");
                    strQry.AppendLine(",DEDUCTION_NO");
                    strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",RUN_NO");
               
                    strQry.AppendLine(",TOTAL");
                    strQry.AppendLine(",TOTAL_ORIGINAL)");

                    strQry.AppendLine(" VALUES ");

                    strQry.AppendLine("(" + parInt64CompanyNo);
                    strQry.AppendLine(",'" + Convert.ToDateTime(parDataSet.Tables["Deduction"].Rows[intRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine("," + parDataSet.Tables["Deduction"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                    strQry.AppendLine(",'P'");
                    strQry.AppendLine("," + parDataSet.Tables["Deduction"].Rows[intRow]["DEDUCTION_NO"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["Deduction"].Rows[intRow]["DEDUCTION_SUB_ACCOUNT_NO"].ToString());
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Deduction"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine(",1");

                    strQry.AppendLine("," + parDataSet.Tables["Deduction"].Rows[intRow]["TOTAL"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["Deduction"].Rows[intRow]["TOTAL"].ToString() + ")");
                }
                else
                {
                    //Updated
                    if (parDataSet.Tables["Deduction"].Rows[intRow]["DEDUCTION_NO"].ToString() == "500")
                    {
                        if (parDataSet.Tables["Deduction"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                        {
                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY");

                            strQry.AppendLine(" SET HOURLY_RATE = " + parDataSet.Tables["Deduction"].Rows[intRow]["TOTAL"].ToString());

                            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                            strQry.AppendLine(" AND PAY_PERIOD_DATE = '" + Convert.ToDateTime(parDataSet.Tables["Deduction"].Rows[intRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'");
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + parDataSet.Tables["Deduction"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine(" AND RUN_TYPE = 'P'");
                            strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parPayCategoryNo);
                            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Deduction"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                            strQry.AppendLine(" AND RUN_NO = 1");
                        }
                        else
                        {
                            //Salaries
                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY");

                            strQry.AppendLine(" SET SALARY_MONTH_PAYMENT = " + parDataSet.Tables["Deduction"].Rows[intRow]["TOTAL"].ToString());

                            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                            strQry.AppendLine(" AND PAY_PERIOD_DATE = '" + Convert.ToDateTime(parDataSet.Tables["Deduction"].Rows[intRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'");
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + parDataSet.Tables["Deduction"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine(" AND RUN_TYPE = 'P'");
                            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Deduction"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                            strQry.AppendLine(" AND RUN_NO = 1");
                        }
                    }
                    else
                    {
                        if (parDataSet.Tables["Deduction"].Rows[intRow]["DEDUCTION_NO"].ToString() == "600")
                        {
                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY");

                            strQry.AppendLine(" SET LEAVE_ACCUM_DAYS = ROUND(" + parDataSet.Tables["Deduction"].Rows[intRow]["TOTAL"].ToString() + " * " + parNormPaidPerPeriod + ",2)");
                                                 
                            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                            strQry.AppendLine(" AND PAY_PERIOD_DATE = '" + Convert.ToDateTime(parDataSet.Tables["Deduction"].Rows[intRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'");
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + parDataSet.Tables["Deduction"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            //Normal Leave
                            strQry.AppendLine(" AND EARNING_NO = 200 ");
                            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Deduction"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                            strQry.AppendLine(" AND PROCESS_NO = 98");
                            strQry.AppendLine(" AND LEAVE_FROM_DATE > = '2015-03-01'");

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                            strQry.Clear();

                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY");

                            strQry.AppendLine(" SET LEAVE_ACCUM_DAYS = ROUND(" + parDataSet.Tables["Deduction"].Rows[intRow]["TOTAL"].ToString() + " * " + parSickPaidPerPeriod + ",2)");

                            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                            strQry.AppendLine(" AND PAY_PERIOD_DATE = '" + Convert.ToDateTime(parDataSet.Tables["Deduction"].Rows[intRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'");
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + parDataSet.Tables["Deduction"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            //Sick Leave
                            strQry.AppendLine(" AND EARNING_NO = 201 ");
                            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Deduction"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                            strQry.AppendLine(" AND PROCESS_NO = 98");
                            strQry.AppendLine(" AND LEAVE_FROM_DATE > = '2015-03-01'");

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                            strQry.Clear();

                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY");

                            strQry.AppendLine(" SET CURRENT_YEAR_LEAVE_SHIFTS_PER_RUN = " + parDataSet.Tables["Deduction"].Rows[intRow]["TOTAL"].ToString());

                            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                            strQry.AppendLine(" AND PAY_PERIOD_DATE = '" + Convert.ToDateTime(parDataSet.Tables["Deduction"].Rows[intRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'");
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + parDataSet.Tables["Deduction"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine(" AND RUN_TYPE = 'P'");
                            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Deduction"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                            strQry.AppendLine(" AND RUN_NO = 1");
                        }
                        else
                        {
                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY");

                            strQry.AppendLine(" SET ");

                            strQry.AppendLine(" TOTAL = " + parDataSet.Tables["Deduction"].Rows[intRow]["TOTAL"].ToString());
                            strQry.AppendLine(",TOTAL_ORIGINAL = " + parDataSet.Tables["Deduction"].Rows[intRow]["TOTAL"].ToString());

                            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                            strQry.AppendLine(" AND PAY_PERIOD_DATE = '" + Convert.ToDateTime(parDataSet.Tables["Deduction"].Rows[intRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'");
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + parDataSet.Tables["Deduction"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine(" AND RUN_TYPE = 'P'");
                            strQry.AppendLine(" AND DEDUCTION_NO = " + parDataSet.Tables["Deduction"].Rows[intRow]["DEDUCTION_NO"].ToString());
                            strQry.AppendLine(" AND DEDUCTION_SUB_ACCOUNT_NO = " + parDataSet.Tables["Deduction"].Rows[intRow]["DEDUCTION_SUB_ACCOUNT_NO"].ToString());
                            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Deduction"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                            strQry.AppendLine(" AND RUN_NO = 1");
                        }
                    }
                }

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
