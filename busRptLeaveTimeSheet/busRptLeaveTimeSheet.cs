using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busRptLeaveTimeSheet
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busRptLeaveTimeSheet()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo, string parstrMenuId, Int64 parint64CurrentUserNo, string parstrCurrentUserAccess)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" PC.COMPANY_NO ");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_TYPE");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");

            //2013-08-30
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND PCPH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }
            
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON PCPH.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" WHERE PCPH.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" AND PCPH.RUN_TYPE = 'P'");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PC.COMPANY_NO ");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategory", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PCPH.COMPANY_NO ");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PCPH.PAY_PERIOD_DATE");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY EIH");
            strQry.AppendLine(" ON PCPH.COMPANY_NO = EIH.COMPANY_NO");
            strQry.AppendLine(" AND PCPH.PAY_PERIOD_DATE = EIH.PAY_PERIOD_DATE");
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = EIH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PCPH.RUN_TYPE = EIH.RUN_TYPE");
            strQry.AppendLine(" AND EIH.LEAVE_SHIFT_NO > 0");

            //2013-08-30
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND PCPH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EIH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE PCPH.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" AND  PCPH.RUN_TYPE = 'P'");
            strQry.AppendLine(" GROUP BY ");

            strQry.AppendLine(" PCPH.COMPANY_NO ");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PCPH.PAY_PERIOD_DATE");
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PCPH.COMPANY_NO ");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PCPH.PAY_PERIOD_DATE DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Date", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.EMPLOYEE_NAME");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            //2013-08-30
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND E.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                //2017-01-28 (Allow Employee to Change PAY_CATEGORY_TYPE) 
                //strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            //2017-01-28 (Allow Employee to Change PAY_CATEGORY_TYPE) 
            //strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" E.EMPLOYEE_NO");
           
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parint64CompanyNo);
           
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EPCH.COMPANY_NO");
            strQry.AppendLine(",EPCH.PAY_PERIOD_DATE");
            strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPCH.EMPLOYEE_NO");
            strQry.AppendLine(",EPCH.PAY_CATEGORY_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");

            //2013-08-30
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND EPCH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPCH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE EPCH.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = 'W'");

            strQry.AppendLine(" GROUP BY");
            strQry.AppendLine(" EPCH.COMPANY_NO");
            strQry.AppendLine(",EPCH.PAY_PERIOD_DATE");
            strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPCH.EMPLOYEE_NO");
            strQry.AppendLine(",EPCH.PAY_CATEGORY_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeePayCategory", parint64CompanyNo);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Print_Report(Int64 parint64CompanyNo, string parstrEmployeeIn, string parstrPayCategoryNoIn, string parstrFromDate,Int64 parint64CurrentUserNo, string parstrCurrentUserAccess)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" C.COMPANY_DESC");
            strQry.AppendLine(",C.LEAVE_BEGIN_MONTH");
            strQry.AppendLine(",CL.DATE_FORMAT");
            strQry.AppendLine(",'' AS REPORT_DATETIME");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.COMPANY_LINK CL");
            strQry.AppendLine(" ON C.COMPANY_NO = CL.COMPANY_NO ");

            strQry.AppendLine(" WHERE C.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "ReportHeader", parint64CompanyNo);

            if (DataSet.Tables["ReportHeader"].Rows.Count > 0)
            {
                DataSet.Tables["ReportHeader"].Rows[0]["REPORT_DATETIME"] = "Printed   " + DateTime.Now.ToString(DataSet.Tables["ReportHeader"].Rows[0]["DATE_FORMAT"].ToString() + "   HH:mm");
            }

            DateTime dtRunDateTime = DateTime.ParseExact(parstrFromDate, "yyyy-MM-dd", null);
            DateTime dtBeginFinancialYear = DateTime.Now;
            DateTime dtEndFinancialYear = DateTime.Now;

            if (dtRunDateTime.Month >= Convert.ToInt32(DataSet.Tables["ReportHeader"].Rows[0]["LEAVE_BEGIN_MONTH"]))
            {
                dtBeginFinancialYear = new DateTime(dtRunDateTime.Year, Convert.ToInt32(DataSet.Tables["ReportHeader"].Rows[0]["LEAVE_BEGIN_MONTH"]), 1);
            }
            else
            {
                dtBeginFinancialYear = new DateTime(dtRunDateTime.Year - 1, Convert.ToInt32(DataSet.Tables["ReportHeader"].Rows[0]["LEAVE_BEGIN_MONTH"]), 1);
            }

            //Last Day Of Fiscal Year
            dtEndFinancialYear = dtBeginFinancialYear.AddYears(1).AddDays(-1);

            strQry.Clear();

            if (dtRunDateTime.Month == Convert.ToInt32(DataSet.Tables["ReportHeader"].Rows[0]["LEAVE_BEGIN_MONTH"]))
            {
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" ETH.PAY_CATEGORY_NO");
                strQry.AppendLine(",ETH.EMPLOYEE_NO ");
                strQry.AppendLine(",LEAVE_TO_DATE = " + dtBeginFinancialYear.AddDays(-1).ToString("yyyyMMdd"));
                strQry.AppendLine(",ETH.TIMESHEET_DATE");
                strQry.AppendLine(",LSH.NORM_PAID_PER_PERIOD");
                strQry.AppendLine(",LSH.SICK_PAID_PER_PERIOD");
                strQry.AppendLine(",LSH.MIN_VALID_SHIFT_MINUTES");

                strQry.AppendLine(",SUM(ETH.TIMESHEET_TIME_OUT_MINUTES - ETH.TIMESHEET_TIME_IN_MINUTES) AS DAY_TIMESHEET_ACCUM_MINUTES ");

                strQry.AppendLine(",ACCUM_COUNT = ");

                strQry.AppendLine(" CASE ");

                strQry.AppendLine(" WHEN SUM(ETH.TIMESHEET_TIME_OUT_MINUTES - ETH.TIMESHEET_TIME_IN_MINUTES) > LSH.MIN_VALID_SHIFT_MINUTES");

                strQry.AppendLine(" THEN 1");

                strQry.AppendLine(" ELSE 0");

                strQry.AppendLine(" END");

                //2014-04-12
                strQry.AppendLine(",ASTERISK_COL = ''");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_HISTORY ETH");

                //2013-08-30
                if (parstrCurrentUserAccess == "U")
                {
                    strQry.AppendLine(" INNER JOIN ");

                    strQry.AppendLine("(SELECT ");
                    strQry.AppendLine(" COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");

                    strQry.AppendLine(" WHERE UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                    strQry.AppendLine(" AND UEPCT.COMPANY_NO = " + parint64CompanyNo);

                    if (parstrEmployeeIn != "")
                    {
                        strQry.AppendLine(" AND UEPCT.EMPLOYEE_NO IN " + parstrEmployeeIn);
                    }

                    if (parstrPayCategoryNoIn != "")
                    {
                        strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIn);
                    }

                    strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("W"));


                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO) AS TEMP_TABLE");

                    strQry.AppendLine(" ON ETH.COMPANY_NO = TEMP_TABLE.COMPANY_NO");
                    strQry.AppendLine(" AND ETH.EMPLOYEE_NO = TEMP_TABLE.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = TEMP_TABLE.PAY_CATEGORY_NO");
                }

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY EIH");
                strQry.AppendLine(" ON ETH.COMPANY_NO = EIH.COMPANY_NO");
                strQry.AppendLine(" AND ETH.PAY_PERIOD_DATE = EIH.PAY_PERIOD_DATE");
                strQry.AppendLine(" AND ETH.EMPLOYEE_NO = EIH.EMPLOYEE_NO");

                if (parstrEmployeeIn != "")
                {
                    strQry.AppendLine(" AND EIH.EMPLOYEE_NO IN " + parstrEmployeeIn);
                }

                strQry.AppendLine(" AND EIH.PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND EIH.RUN_TYPE = 'P'");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT_HISTORY LSH");
                strQry.AppendLine(" ON ETH.COMPANY_NO = LSH.COMPANY_NO");
                strQry.AppendLine(" AND ETH.PAY_PERIOD_DATE = LSH.PAY_PERIOD_DATE");
                strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = LSH.PAY_CATEGORY_NO");

                if (parstrPayCategoryNoIn != "")
                {
                    strQry.AppendLine(" AND LSH.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIn);
                }

                strQry.AppendLine(" WHERE ETH.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND ETH.PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));

                //Previous Fiscal Year
                strQry.AppendLine(" AND ETH.TIMESHEET_DATE <= '" + dtBeginFinancialYear.AddDays(-1).ToString("yyyy-MM-dd") + "'");
                
                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" ETH.PAY_CATEGORY_NO");
                strQry.AppendLine(",ETH.EMPLOYEE_NO ");
                strQry.AppendLine(",ETH.TIMESHEET_DATE");
                strQry.AppendLine(",LSH.NORM_PAID_PER_PERIOD");
                strQry.AppendLine(",LSH.SICK_PAID_PER_PERIOD");
                strQry.AppendLine(",LSH.MIN_VALID_SHIFT_MINUTES");

                strQry.AppendLine(" UNION ");
            }

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ETH.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETH.EMPLOYEE_NO ");
            //strQry.AppendLine(",LEAVE_TO_DATE = " + dtEndFinancialYear.ToString("yyyyMMdd"));
            strQry.AppendLine(",LEAVE_TO_DATE = " + parstrFromDate.Replace("-",""));
            strQry.AppendLine(",ETH.TIMESHEET_DATE");
            strQry.AppendLine(",LSH.NORM_PAID_PER_PERIOD");
            strQry.AppendLine(",LSH.SICK_PAID_PER_PERIOD");
            strQry.AppendLine(",LSH.MIN_VALID_SHIFT_MINUTES");
         
            strQry.AppendLine(",SUM(ETH.TIMESHEET_TIME_OUT_MINUTES - ETH.TIMESHEET_TIME_IN_MINUTES) AS DAY_TIMESHEET_ACCUM_MINUTES ");

            strQry.AppendLine(",ACCUM_COUNT = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN SUM(ETH.TIMESHEET_TIME_OUT_MINUTES - ETH.TIMESHEET_TIME_IN_MINUTES) > LSH.MIN_VALID_SHIFT_MINUTES");

            strQry.AppendLine(" THEN 1");

            strQry.AppendLine(" ELSE 0");

            strQry.AppendLine(" END");

            //2014-04-12
            strQry.AppendLine(",ASTERISK_COL = ''");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_HISTORY ETH");

            //2013-08-30
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(SELECT ");
                strQry.AppendLine(" COMPANY_NO");
                strQry.AppendLine(",EMPLOYEE_NO");
                strQry.AppendLine(",PAY_CATEGORY_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");

                strQry.AppendLine(" WHERE UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND UEPCT.COMPANY_NO = " + parint64CompanyNo);

                if (parstrEmployeeIn != "")
                {
                    strQry.AppendLine(" AND UEPCT.EMPLOYEE_NO IN " + parstrEmployeeIn);
                }

                if (parstrPayCategoryNoIn != "")
                {
                    strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIn);
                }

                strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("W"));


                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" COMPANY_NO");
                strQry.AppendLine(",EMPLOYEE_NO");
                strQry.AppendLine(",PAY_CATEGORY_NO) AS TEMP_TABLE");

                strQry.AppendLine(" ON ETH.COMPANY_NO = TEMP_TABLE.COMPANY_NO");
                strQry.AppendLine(" AND ETH.EMPLOYEE_NO = TEMP_TABLE.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = TEMP_TABLE.PAY_CATEGORY_NO");
            }
           
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY EIH");
            strQry.AppendLine(" ON ETH.COMPANY_NO = EIH.COMPANY_NO");
            strQry.AppendLine(" AND ETH.PAY_PERIOD_DATE = EIH.PAY_PERIOD_DATE");
            strQry.AppendLine(" AND ETH.EMPLOYEE_NO = EIH.EMPLOYEE_NO");

            if (parstrEmployeeIn != "")
            {
                strQry.AppendLine(" AND EIH.EMPLOYEE_NO IN " + parstrEmployeeIn);
            }

            strQry.AppendLine(" AND EIH.PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" AND EIH.RUN_TYPE = 'P'");
    
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT_HISTORY LSH");
            strQry.AppendLine(" ON ETH.COMPANY_NO = LSH.COMPANY_NO");
            strQry.AppendLine(" AND ETH.PAY_PERIOD_DATE = LSH.PAY_PERIOD_DATE");
            strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = LSH.PAY_CATEGORY_NO");

            if (parstrPayCategoryNoIn != "")
            {
                strQry.AppendLine(" AND LSH.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIn);
            }

            strQry.AppendLine(" WHERE ETH.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND ETH.PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));

            strQry.AppendLine(" AND ETH.TIMESHEET_DATE >= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'");
            strQry.AppendLine(" AND ETH.TIMESHEET_DATE <= '" + dtEndFinancialYear.ToString("yyyy-MM-dd") + "'");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" ETH.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETH.EMPLOYEE_NO ");
            strQry.AppendLine(",ETH.TIMESHEET_DATE");
            strQry.AppendLine(",LSH.NORM_PAID_PER_PERIOD");
            strQry.AppendLine(",LSH.SICK_PAID_PER_PERIOD");
            strQry.AppendLine(",LSH.MIN_VALID_SHIFT_MINUTES");

            this.clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Report", parint64CompanyNo);

            strQry.Clear();

            if (dtRunDateTime.Month == Convert.ToInt32(DataSet.Tables["ReportHeader"].Rows[0]["LEAVE_BEGIN_MONTH"]))
            {
                //Build Dummy Record where does not Exist
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" LH.EMPLOYEE_NO ");
                //Go Back 45 Days - Should be enough
                strQry.AppendLine(",'" + dtBeginFinancialYear.AddDays(-45).ToString("yyyy-MM-dd") + "' AS LEAVE_FROM_DATE");
                strQry.AppendLine(",'" + dtBeginFinancialYear.AddDays(-1).ToString("yyyy-MM-dd") + "' AS LEAVE_TO_DATE");
                strQry.AppendLine(",0 AS LEAVE_DAYS_DECIMAL");
                strQry.AppendLine(",0 AS NORMAL_LEAVE_ACCUM_DAYS");
                strQry.AppendLine(",0 AS SICK_LEAVE_ACCUM_DAYS");

                strQry.AppendLine(" FROM  InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY LH");
                
                if (parstrCurrentUserAccess == "U")
                {
                    strQry.AppendLine(" INNER JOIN ");

                    strQry.AppendLine("(SELECT ");
                    strQry.AppendLine(" COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");


                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");

                    strQry.AppendLine(" WHERE UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                    strQry.AppendLine(" AND UEPCT.COMPANY_NO = " + parint64CompanyNo);

                    if (parstrEmployeeIn != "")
                    {
                        strQry.AppendLine(" AND UEPCT.EMPLOYEE_NO IN " + parstrEmployeeIn);
                    }

                    if (parstrPayCategoryNoIn != "")
                    {
                        strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIn);
                    }

                    strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("W"));


                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE) AS TEMP_TABLE");

                    strQry.AppendLine(" ON LH.COMPANY_NO = TEMP_TABLE.COMPANY_NO");
                    strQry.AppendLine(" AND LH.EMPLOYEE_NO = TEMP_TABLE.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = TEMP_TABLE.PAY_CATEGORY_TYPE");
                }

                if (parstrPayCategoryNoIn != "")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");
                    strQry.AppendLine(" ON LH.COMPANY_NO = EPCH.COMPANY_NO");
                    strQry.AppendLine(" AND LH.PAY_PERIOD_DATE = EPCH.PAY_PERIOD_DATE");
                    strQry.AppendLine(" AND LH.EMPLOYEE_NO = EPCH.EMPLOYEE_NO");
                    strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = EPCH.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EPCH.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIn);
                }


                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY LH1");
                strQry.AppendLine(" ON LH.COMPANY_NO = LH1.COMPANY_NO");
                strQry.AppendLine(" AND LH.PAY_PERIOD_DATE = LH1.PAY_PERIOD_DATE");
                strQry.AppendLine(" AND LH.EMPLOYEE_NO = LH1.EMPLOYEE_NO");
                strQry.AppendLine(" AND LH.EARNING_NO = LH1.EARNING_NO");
                strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = LH1.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND LH1.LEAVE_TO_DATE = '" + dtBeginFinancialYear.AddDays(-1).ToString("yyyy-MM-dd") + "'");

                strQry.AppendLine(" WHERE LH.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND LH.PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND LH.EARNING_NO IN (200,201)");

                if (parstrEmployeeIn != "")
                {
                    strQry.AppendLine(" AND LH.EMPLOYEE_NO IN " + parstrEmployeeIn);
                }

                strQry.AppendLine(" AND LH.PROCESS_NO = 98 ");
                strQry.AppendLine(" AND LH.LEAVE_FROM_DATE = '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'");

                //Record JOIN does NOT Exist
                strQry.AppendLine(" AND LH1.COMPANY_NO IS NULL ");

                strQry.AppendLine(" UNION ");
            }

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" TEMP_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",TEMP_TABLE.LEAVE_FROM_DATE ");
            strQry.AppendLine(",TEMP_TABLE.LEAVE_TO_DATE ");
            strQry.AppendLine(",MAX(TEMP_TABLE.LEAVE_DAYS_DECIMAL) AS LEAVE_DAYS_DECIMAL");

            strQry.AppendLine(",SUM(TEMP_TABLE.NORMAL_LEAVE_ACCUM_DAYS) AS NORMAL_LEAVE_ACCUM_DAYS");
            strQry.AppendLine(",SUM(TEMP_TABLE.SICK_LEAVE_ACCUM_DAYS) AS SICK_LEAVE_ACCUM_DAYS");

            strQry.AppendLine(" FROM ");

            strQry.AppendLine("(SELECT ");
           
            strQry.AppendLine(" LH.EMPLOYEE_NO ");
            strQry.AppendLine(",LH.LEAVE_FROM_DATE ");
            strQry.AppendLine(",LH.LEAVE_TO_DATE ");
            strQry.AppendLine(",LH.LEAVE_DAYS_DECIMAL");

            strQry.AppendLine(",NORMAL_LEAVE_ACCUM_DAYS = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN LH.EARNING_NO = 200");

            strQry.AppendLine(" THEN LH.LEAVE_ACCUM_DAYS");

            strQry.AppendLine(" ELSE 0 ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",SICK_LEAVE_ACCUM_DAYS = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN LH.EARNING_NO = 201");

            strQry.AppendLine(" THEN LH.LEAVE_ACCUM_DAYS");

            strQry.AppendLine(" ELSE 0 ");

            strQry.AppendLine(" END ");
            
            strQry.AppendLine(" FROM  InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY LH");
        
            //2013-08-30
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(SELECT ");
                strQry.AppendLine(" COMPANY_NO");
                strQry.AppendLine(",EMPLOYEE_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");

                strQry.AppendLine(" WHERE UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND UEPCT.COMPANY_NO = " + parint64CompanyNo);

                if (parstrEmployeeIn != "")
                {
                    strQry.AppendLine(" AND UEPCT.EMPLOYEE_NO IN " + parstrEmployeeIn);
                }

                if (parstrPayCategoryNoIn != "")
                {
                    strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIn);
                }

                strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("W"));


                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" COMPANY_NO");
                strQry.AppendLine(",EMPLOYEE_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE) AS TEMP_TABLE");

                strQry.AppendLine(" ON LH.COMPANY_NO = TEMP_TABLE.COMPANY_NO");
                strQry.AppendLine(" AND LH.EMPLOYEE_NO = TEMP_TABLE.EMPLOYEE_NO ");
                strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = TEMP_TABLE.PAY_CATEGORY_TYPE");
            }

            if (parstrPayCategoryNoIn != "")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");
                strQry.AppendLine(" ON LH.COMPANY_NO = EPCH.COMPANY_NO");
                strQry.AppendLine(" AND LH.PAY_PERIOD_DATE = EPCH.PAY_PERIOD_DATE");
                strQry.AppendLine(" AND LH.EMPLOYEE_NO = EPCH.EMPLOYEE_NO");
                strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = EPCH.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EPCH.RUN_TYPE = 'P'");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIn);
            }
         
            strQry.AppendLine(" WHERE LH.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND LH.PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
            strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" AND LH.EARNING_NO IN (200,201)");

            if (parstrEmployeeIn != "")
            {
                strQry.AppendLine(" AND LH.EMPLOYEE_NO IN " + parstrEmployeeIn);
            }

            strQry.AppendLine(" AND LH.PROCESS_NO = 98) AS TEMP_TABLE");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" TEMP_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",TEMP_TABLE.LEAVE_FROM_DATE ");
            strQry.AppendLine(",TEMP_TABLE.LEAVE_TO_DATE ");

            this.clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "ReportLeave", parint64CompanyNo);
            
            //2014-04-12
            //Check for Leave Accumulation Exceeding Max Accum Day
            Double dblAccumNormalLeave = 0;
            bool blnFirstErrorRecord = true;

            for (int intRow = 0; intRow < DataSet.Tables["ReportLeave"].Rows.Count; intRow++)
            {
                dblAccumNormalLeave = 0;
                blnFirstErrorRecord = true;

                double dblNormLeaveAccum = Math.Round(Convert.ToDouble(DataSet.Tables["ReportLeave"].Rows[intRow]["NORMAL_LEAVE_ACCUM_DAYS"]),2);

                DataView ReportDataView = new DataView(DataSet.Tables["Report"],
                "EMPLOYEE_NO = " + DataSet.Tables["ReportLeave"].Rows[intRow]["EMPLOYEE_NO"].ToString() + " AND TIMESHEET_DATE >= '" + Convert.ToDateTime(DataSet.Tables["ReportLeave"].Rows[intRow]["LEAVE_FROM_DATE"]).ToString("yyyy-MM-dd") + "' AND TIMESHEET_DATE <= '" + Convert.ToDateTime(DataSet.Tables["ReportLeave"].Rows[intRow]["LEAVE_TO_DATE"]).ToString("yyyy-MM-dd") + "'",
                "",
                DataViewRowState.CurrentRows);

                for (int intDataViewRow = 0; intDataViewRow < ReportDataView.Count; intDataViewRow++)
                {
                    dblAccumNormalLeave += Math.Round(Convert.ToDouble(ReportDataView[0]["NORM_PAID_PER_PERIOD"]),3);

                    double dblAccumNormalLeaveCheck = Math.Round(dblAccumNormalLeave, 2,MidpointRounding.AwayFromZero);

                    if (dblNormLeaveAccum < dblAccumNormalLeaveCheck)
                    {
                        if (blnFirstErrorRecord == true)
                        {
                            blnFirstErrorRecord = false;

                            if (dblNormLeaveAccum < dblAccumNormalLeaveCheck)
                            {
                                ReportDataView[intDataViewRow]["ASTERISK_COL"] = "*";

                                if (dblNormLeaveAccum == 0)
                                {
                                    ReportDataView[intDataViewRow]["ACCUM_COUNT"] = 0;
                                }
                            }
                        }
                        else
                        {
                            ReportDataView[intDataViewRow]["ACCUM_COUNT"] = 0;
                            ReportDataView[intDataViewRow]["ASTERISK_COL"] = "*";
                        }
                    }
                    else
                    {
                        //
                    }
                }
            }
 
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
    }
}
