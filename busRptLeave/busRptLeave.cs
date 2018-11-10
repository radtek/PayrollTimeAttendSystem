using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busRptLeave
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busRptLeave()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo, string parstrMenuId, Int64 parint64CurrentUserNo,string parstrCurrentUserAccess)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
            DataSet DataSetTemp = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.EARNING_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EARNING_DESC");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING E");

            //2013-08-30
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND E.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);

            //2013-06-21 Excludes T=Time Attendance
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE IN ('W','S')");

            strQry.AppendLine(" AND E.EARNING_NO >= 200");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.EARNING_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EARNING_DESC");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 4");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "LeaveType", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PCPH.COMPANY_NO");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",MAX(PCPH.PAY_PERIOD_DATE) AS PAY_PERIOD_DATE");
           
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH ");

            //2013-08-30
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND PCPH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE PCPH.COMPANY_NO = " + parint64CompanyNo);

            //2013-06-21 Excludes T=Time Attendance
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE IN ('W','S')");
            
            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PCPH.COMPANY_NO");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" UNION ");
           
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PCPH.COMPANY_NO");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",MIN(PCPH.PAY_PERIOD_DATE) AS PAY_PERIOD_DATE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH ");

            //2013-08-30
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND PCPH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE PCPH.COMPANY_NO = " + parint64CompanyNo);

            //2013-06-21 Excludes T=Time Attendance
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE IN ('W','S')");
            
            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PCPH.COMPANY_NO");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 1");
            strQry.AppendLine(",2");
            strQry.AppendLine(",3");
           
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Month", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT");

            strQry.AppendLine(" PCPH.COMPANY_NO");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PCPH.PAY_PERIOD_DATE");
            
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

            strQry.AppendLine(" WHERE PCPH.COMPANY_NO = " + parint64CompanyNo);

            //2013-06-21 Excludes T=Time Attendance
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE IN ('W','S')");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PCPH.PAY_PERIOD_DATE DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Dates", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.EMPLOYEE_ENDDATE");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            //2013-08-30
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND E.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                //strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);

            //2013-06-21 Excludes T=Time Attendance
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE IN ('W','S')");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

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

            //2013-06-21 Excludes T=Time Attendance
            strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE IN ('W','S')");

            strQry.AppendLine(" GROUP BY");
            strQry.AppendLine(" EPCH.COMPANY_NO");
            strQry.AppendLine(",EPCH.PAY_PERIOD_DATE");
            strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPCH.EMPLOYEE_NO");
            strQry.AppendLine(",EPCH.PAY_CATEGORY_NO");
           
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeePayCategory", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PCPH.COMPANY_NO");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON PCPH.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            //2013-08-30
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND PCPH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE PCPH.COMPANY_NO = " + parint64CompanyNo);

            //2013-06-21 Excludes T=Time Attendance
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE IN ('W','S')");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PCPH.COMPANY_NO");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategory", parint64CompanyNo);
           
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Print_Leave_Report(Int64 parint64CompanyNo, string parstrEarningNoIn, string parstrPayCategoryType, string parstrEmployeeIn, string parstrPayCategoryNoIn, string parstrDateOption, string parstrFromDate, string parstrToDate, string parstrDateSign, string parstrPrevYearBalance, string parstrActiveClosedInd, Int64 parint64CurrentUserNo, string parstrCurrentUserAccess)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
            
            DateTime dtStartLeaveTaxYear;

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ISNULL(MAX(PAY_PERIOD_DATE),GETDATE()) AS MAX_PAY_PERIOD_DATE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "MaxDate", parint64CompanyNo);
            
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" LEAVE_BEGIN_MONTH");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY E");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "LeaveStartDate", parint64CompanyNo);

            if (DataSet.Tables["LeaveStartDate"].Rows.Count > 0)
            {
                //Position Within Current Financial Year
                if (Convert.ToDateTime(DataSet.Tables["MaxDate"].Rows[0]["MAX_PAY_PERIOD_DATE"]).Month >= Convert.ToInt32(DataSet.Tables["LeaveStartDate"].Rows[0]["LEAVE_BEGIN_MONTH"]))
                {
                    dtStartLeaveTaxYear = new DateTime(Convert.ToDateTime(DataSet.Tables["MaxDate"].Rows[0]["MAX_PAY_PERIOD_DATE"]).Year, Convert.ToInt32(DataSet.Tables["LeaveStartDate"].Rows[0]["LEAVE_BEGIN_MONTH"]), 1);
                }
                else
                {
                    dtStartLeaveTaxYear = new DateTime(Convert.ToDateTime(DataSet.Tables["MaxDate"].Rows[0]["MAX_PAY_PERIOD_DATE"]).Year - 1, Convert.ToInt32(DataSet.Tables["LeaveStartDate"].Rows[0]["LEAVE_BEGIN_MONTH"]), 1);
                }
            }
            else
            {
                dtStartLeaveTaxYear = new DateTime(DateTime.Now.Year, 1, 1);
            }

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" C.COMPANY_DESC");
            strQry.AppendLine(",CL.DATE_FORMAT");
            strQry.AppendLine(",'' AS REPORT_DATETIME");
            strQry.AppendLine(",'' AS REPORT_HEADER");

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
      
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EN.EARNING_DESC");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            //Here Only For Sorting AND Creation of Report
            strQry.AppendLine(",CONVERT(VARCHAR(10),LH.PAY_PERIOD_DATE,120)");
            //Here Only For Sorting AND Creation of Report
            strQry.AppendLine(",LH.LEAVE_FROM_DATE AS LEAVE_FROM_DATE_SORT");
            //Here Only For Sorting AND Creation of Report
            strQry.AppendLine(",LH.LEAVE_TO_DATE AS LEAVE_TO_DATE_SORT");
            //Here Only For Sorting AND Creation of Report
            strQry.AppendLine(",LH.PROCESS_NO");
           
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");

            strQry.AppendLine(",LEAVE_PROCESSED_DATE = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN CL.DATE_FORMAT = 'yyyy-MM-dd'");
            strQry.AppendLine(" THEN CONVERT(VARCHAR(10),LH.PAY_PERIOD_DATE,120)");

            //dd-MMMM-yyyy
            strQry.AppendLine(" ELSE CONVERT(VARCHAR(10),LH.PAY_PERIOD_DATE,105)");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",LH.LEAVE_DESC");

            strQry.AppendLine(",LEAVE_FROM_DATE = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN CL.DATE_FORMAT = 'yyyy-MM-dd'");
            strQry.AppendLine(" THEN CONVERT(VARCHAR(10),LH.LEAVE_FROM_DATE,120)");
            
            strQry.AppendLine(" ELSE CONVERT(VARCHAR(10),LH.LEAVE_FROM_DATE,105)");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",LEAVE_TO_DATE = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN CL.DATE_FORMAT = 'yyyy-MM-dd'");
            strQry.AppendLine(" THEN CONVERT(VARCHAR(10),LH.LEAVE_TO_DATE,120)");
            
            strQry.AppendLine(" ELSE CONVERT(VARCHAR(10),LH.LEAVE_TO_DATE,105)");

            strQry.AppendLine(" END ");
        
            strQry.AppendLine(",LH.LEAVE_ACCUM_DAYS");
            strQry.AppendLine(",LH.LEAVE_PAID_DAYS");

            strQry.AppendLine(",LH.LEAVE_DAYS_DECIMAL");
            strQry.AppendLine(",LH.LEAVE_HOURS_DECIMAL");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY LH");

            if (parstrPayCategoryNoIn != "")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");
                strQry.AppendLine(" ON LH.COMPANY_NO =  EPCH.COMPANY_NO");
                strQry.AppendLine(" AND LH.PAY_PERIOD_DATE =  EPCH.PAY_PERIOD_DATE");
                strQry.AppendLine(" AND LH.EMPLOYEE_NO =  EPCH.EMPLOYEE_NO");

                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIn);

                strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE =  EPCH.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EPCH.RUN_TYPE = 'P'");
            }

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.COMPANY_LINK CL");
            strQry.AppendLine(" ON LH.COMPANY_NO =  CL.COMPANY_NO");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C");
            strQry.AppendLine(" ON LH.COMPANY_NO =  C.COMPANY_NO");
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON LH.COMPANY_NO = E.COMPANY_NO");
            strQry.AppendLine(" AND LH.EMPLOYEE_NO = E.EMPLOYEE_NO");
            //2017-01-28 (Allow Employee to Change PAY_CATEGORY_TYPE)  
            //strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");

            if (parstrActiveClosedInd == "A")
            {
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            }
            else
            {
                if (parstrActiveClosedInd == "C")
                {
                    strQry.AppendLine(" AND NOT E.EMPLOYEE_ENDDATE IS NULL");
                }
            }

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

                strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));


                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" COMPANY_NO");
                strQry.AppendLine(",EMPLOYEE_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE) AS TEMP_TABLE");

                strQry.AppendLine(" ON E.COMPANY_NO = TEMP_TABLE.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = TEMP_TABLE.EMPLOYEE_NO ");
                strQry.AppendLine(" AND TEMP_TABLE.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
            }

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EN ");
            strQry.AppendLine(" ON LH.COMPANY_NO = EN.COMPANY_NO");
            strQry.AppendLine(" AND LH.EARNING_NO = EN.EARNING_NO ");
            strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT LS ");
            strQry.AppendLine(" ON LS.COMPANY_NO = E.COMPANY_NO");
            strQry.AppendLine(" AND LS.LEAVE_SHIFT_NO = E.LEAVE_SHIFT_NO ");
            //2017-02-16 cATERS FOR WHEN Employee Changes
            //strQry.AppendLine(" AND LS.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));

            //Add Filter
            strQry.AppendLine(" WHERE LH.COMPANY_NO = " + parint64CompanyNo);

            if (parstrEmployeeIn != "")
            {
                strQry.AppendLine(" AND LH.EMPLOYEE_NO IN " + parstrEmployeeIn);
            }

            if (parstrEarningNoIn != "")
            {
                strQry.AppendLine(" AND LH.EARNING_NO IN " + parstrEarningNoIn);
            }

            if (parstrDateOption == "Z")
            {
                DataSet.Tables["ReportHeader"].Rows[0]["REPORT_HEADER"] = "Previous Year (" + dtStartLeaveTaxYear.AddYears(-1).ToString(DataSet.Tables["ReportHeader"].Rows[0]["DATE_FORMAT"].ToString()) + " to " + dtStartLeaveTaxYear.AddDays(-1).ToString(DataSet.Tables["ReportHeader"].Rows[0]["DATE_FORMAT"].ToString() + ")");
                
                strQry.AppendLine(" AND ((LH.PAY_PERIOD_DATE >= '" + dtStartLeaveTaxYear.AddYears(-1).ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" AND LH.PAY_PERIOD_DATE < '" + dtStartLeaveTaxYear.ToString("yyyy-MM-dd") + "'");
                //Covers YearEnd Boundary 
                //Extra Leave Carried over YearEnd Boundary
                strQry.AppendLine(" AND NOT (LH.PAY_PERIOD_DATE >= '" + dtStartLeaveTaxYear.AddYears(-1).ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" AND LH.PAY_PERIOD_DATE <= '" + dtStartLeaveTaxYear.AddYears(-1).AddMonths(1).ToString("yyyy-MM-dd") + "'");
                //Leave ToDate is End of Year
                strQry.AppendLine(" AND LH.LEAVE_TO_DATE = '" + dtStartLeaveTaxYear.AddYears(-1).AddDays(-1).ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" AND LH.PROCESS_NO = 98))");

                //Check Next Month to See if it Coveres YearEnd Boundary
                strQry.AppendLine(" OR (LH.PAY_PERIOD_DATE >= '" + dtStartLeaveTaxYear.ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" AND LH.PAY_PERIOD_DATE <= '" + dtStartLeaveTaxYear.AddMonths(1).ToString("yyyy-MM-dd") + "'");
                //Leave ToDate is End of Year
                strQry.AppendLine(" AND LH.LEAVE_TO_DATE = '" + dtStartLeaveTaxYear.AddDays(-1).ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" AND LH.PROCESS_NO = 98))");
            }
            else
            {
                if (parstrDateOption == "Y")
                {
                    DataSet.Tables["ReportHeader"].Rows[0]["REPORT_HEADER"] = "Year to Date (" + dtStartLeaveTaxYear.ToString(DataSet.Tables["ReportHeader"].Rows[0]["DATE_FORMAT"].ToString()) + " to " + dtStartLeaveTaxYear.AddYears(1).AddDays(-1).ToString(DataSet.Tables["ReportHeader"].Rows[0]["DATE_FORMAT"].ToString() + ")");

                    strQry.AppendLine(" AND (LH.PAY_PERIOD_DATE >= '" + dtStartLeaveTaxYear.ToString("yyyy-MM-dd") + "'");
                    //Covers YearEnd Boundary 
                    //Extra Leave Carried over YearEnd Boundary
                    strQry.AppendLine(" AND NOT (LH.PAY_PERIOD_DATE >= '" + dtStartLeaveTaxYear.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND LH.PAY_PERIOD_DATE <= '" + dtStartLeaveTaxYear.AddMonths(1).ToString("yyyy-MM-dd") + "'");
                    //Leave ToDate is End of Year
                    strQry.AppendLine(" AND LH.LEAVE_TO_DATE = '" + dtStartLeaveTaxYear.AddDays(-1).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND LH.PROCESS_NO = 98))");
                }
                else
                {
                    if (parstrDateOption == "P")
                    {
                        strQry.AppendLine(" AND LH.PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                    }
                    else
                    {
                        if (parstrDateOption == "M")
                        {
                            strQry.AppendLine(" AND DATEPART(yyyy,LH.PAY_PERIOD_DATE) = " + parstrFromDate.Substring(0, 4));
                            strQry.AppendLine(" AND DATEPART(mm,LH.PAY_PERIOD_DATE) = " + parstrFromDate.Substring(4, 2));
                        }
                        else
                        {
                            if (parstrDateOption == "O")
                            {
                                if (parstrDateSign == "G")
                                {
                                    strQry.AppendLine(" AND LH.PAY_PERIOD_DATE >= " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                                }
                                else
                                {
                                    strQry.AppendLine(" AND LH.PAY_PERIOD_DATE <= " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                                }

                                if (parstrToDate != "")
                                {
                                    strQry.AppendLine(" AND LH.PAY_PERIOD_DATE <= " + clsDBConnectionObjects.Text2DynamicSQL(parstrToDate));
                                }
                            }
                        }
                    }
                }
            }

            //2017-02-22 Allow For Employee to Change PAY_CATEGORY_TYPE
            //strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
                     
            //Year To Date - Accumulate for Normal Leave
            if (parstrDateOption == "Y"
                & parstrPrevYearBalance == "Y")
            {
                strQry.AppendLine(" UNION");

                strQry.AppendLine(" SELECT ");
                
                strQry.AppendLine(" EN.EARNING_DESC");

                strQry.AppendLine(",E.EMPLOYEE_CODE");
                //Here Only For Sorting AND Creation of Report
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(dtStartLeaveTaxYear.ToString("yyyy-MM-dd")) + " AS PAY_PERIOD_DATE");
                //Here Only For Sorting AND Creation of Report
                strQry.AppendLine(",'' AS LEAVE_FROM_DATE_SORT");
                //Here Only For Sorting AND Creation of Report
                strQry.AppendLine(",'' AS LEAVE_TO_DATE_SORT");
                //Here Only For Sorting AND Creation of Report
                strQry.AppendLine(",999 AS PROCESS_NO");
           
                strQry.AppendLine(",E.EMPLOYEE_NAME");
                strQry.AppendLine(",E.EMPLOYEE_SURNAME");
                strQry.AppendLine(",LEAVE_PROCESSED_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(dtStartLeaveTaxYear.ToString(DataSet.Tables["ReportHeader"].Rows[0]["DATE_FORMAT"].ToString())));
                
                strQry.AppendLine(",'Accumulated Balance c/f' AS LEAVE_DESC");

                strQry.AppendLine(",LEAVE_FROM_DATE = ");

                strQry.AppendLine(" CASE ");

                strQry.AppendLine(" WHEN CL.DATE_FORMAT = 'yyyy-MM-dd'");
                strQry.AppendLine(" THEN CONVERT(VARCHAR(10),MIN(LH.LEAVE_FROM_DATE),120)");

                strQry.AppendLine(" ELSE CONVERT(VARCHAR(10),MIN(LH.LEAVE_FROM_DATE),105)");

                strQry.AppendLine(" END ");

                strQry.AppendLine(",LEAVE_TO_DATE = ");

                strQry.AppendLine(" CASE ");

                strQry.AppendLine(" WHEN CL.DATE_FORMAT = 'yyyy-MM-dd'");
                strQry.AppendLine(" THEN CONVERT(VARCHAR(10),MAX(LH.LEAVE_TO_DATE),120)");

                strQry.AppendLine(" ELSE CONVERT(VARCHAR(10),MAX(LH.LEAVE_TO_DATE),105)");

                strQry.AppendLine(" END ");

                strQry.AppendLine(",SUM(LH.LEAVE_ACCUM_DAYS) AS LEAVE_ACCUM_DAYS");
                strQry.AppendLine(",SUM(LH.LEAVE_PAID_DAYS) AS LEAVE_PAID_DAYS");

                strQry.AppendLine(",0 AS LEAVE_DAYS_DECIMAL");
                strQry.AppendLine(",0 AS LEAVE_HOURS_DECIMAL");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY LH");

                if (parstrPayCategoryNoIn != "")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");
                    strQry.AppendLine(" ON LH.COMPANY_NO =  EPCH.COMPANY_NO");
                    strQry.AppendLine(" AND LH.PAY_PERIOD_DATE =  EPCH.PAY_PERIOD_DATE");
                    strQry.AppendLine(" AND LH.EMPLOYEE_NO =  EPCH.EMPLOYEE_NO");

                    strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIn);

                    strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE =  EPCH.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EPCH.RUN_TYPE = 'P'");
                }

                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.COMPANY_LINK CL");
                strQry.AppendLine(" ON LH.COMPANY_NO =  CL.COMPANY_NO");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C");
                strQry.AppendLine(" ON LH.COMPANY_NO =  C.COMPANY_NO");
                strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON LH.COMPANY_NO = E.COMPANY_NO");
                strQry.AppendLine(" AND LH.EMPLOYEE_NO = E.EMPLOYEE_NO");
                //2017-01-28 (Allow Employee to Change PAY_CATEGORY_TYPE)  
                //strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");

                if (parstrActiveClosedInd == "A")
                {
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                }
                else
                {
                    if (parstrActiveClosedInd == "C")
                    {
                        strQry.AppendLine(" AND NOT E.EMPLOYEE_ENDDATE IS NULL");
                    }
                }
                
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

                    strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));


                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE) AS TEMP_TABLE");

                    strQry.AppendLine(" ON E.COMPANY_NO = TEMP_TABLE.COMPANY_NO");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = TEMP_TABLE.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND TEMP_TABLE.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
                }

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EN ");
                strQry.AppendLine(" ON LH.COMPANY_NO = EN.COMPANY_NO");
                strQry.AppendLine(" AND LH.EARNING_NO = EN.EARNING_NO ");
                strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL");
                
                //Add Filter
                strQry.AppendLine(" WHERE LH.COMPANY_NO = " + parint64CompanyNo);

                if (parstrEmployeeIn != "")
                {
                    strQry.AppendLine(" AND LH.EMPLOYEE_NO IN " + parstrEmployeeIn);
                }

                if (parstrEarningNoIn != "")
                {
                    strQry.AppendLine(" AND LH.EARNING_NO IN " + parstrEarningNoIn);
                }

                strQry.AppendLine(" AND LH.EARNING_NO = 200");

                //2017-02-22 Allow For Employee to Change PAY_CATEGORY_TYPE
                //strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));

                //2017-01-10 - Covers YearEnd Boundary
                strQry.AppendLine(" AND ((LH.PAY_PERIOD_DATE < '" + dtStartLeaveTaxYear.ToString("yyyy-MM-dd") + "')");

                //Extra Leave Carried over YearEnd Boundary
                strQry.AppendLine(" OR (LH.PAY_PERIOD_DATE >= '" + dtStartLeaveTaxYear.ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" AND LH.PAY_PERIOD_DATE <= '" + dtStartLeaveTaxYear.AddMonths(1).ToString("yyyy-MM-dd") + "'");
                //Leave ToDate is End of Year
                strQry.AppendLine(" AND LH.LEAVE_TO_DATE = '" + dtStartLeaveTaxYear.AddDays(-1).ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" AND LH.PROCESS_NO = 98))");
                
                strQry.AppendLine(" GROUP BY"); 
                strQry.AppendLine(" EN.EARNING_DESC");
                strQry.AppendLine(",E.EMPLOYEE_CODE");
                strQry.AppendLine(",E.EMPLOYEE_NAME");
                strQry.AppendLine(",E.EMPLOYEE_SURNAME");
                strQry.AppendLine(",CL.DATE_FORMAT");
            }
            
            strQry.AppendLine(" ORDER BY ");

            //EARNING_DESC
            strQry.AppendLine(" 1");
            //EMPLOYEE_CODE");
            strQry.AppendLine(",2");
            //LEAVE_PROCESSED_DATE
            strQry.AppendLine(",3");
            //LEAVE_FROM_DATE
            strQry.AppendLine(",4");
            //LEAVE_TO_DATE
            strQry.AppendLine(",5");
            //PROCESS_NO
            strQry.AppendLine(",6 DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "ReportLeave", parint64CompanyNo);
           
            //DataSet.Tables["ReportLeave"].Rows[0]["LEAVE_DESC"] = "ZZZZZZZZZZZZZZZZZZZZZZ");
            //DataSet.Tables["ReportLeave"].Rows[0]["LEAVE_ACCUM_DAYS"] = 9999.99;
            //DataSet.Tables["ReportLeave"].Rows[0]["LEAVE_PAID_DAYS"] = 9999.99;
 
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
    }
}
