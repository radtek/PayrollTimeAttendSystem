using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busTimeSheetBatch
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busTimeSheetBatch()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo, string parstrCurrentUserAccessInd, Int64 parint64CurrentUserNo, string parstrFromProgram)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            string strCurrentDate = DateTime.Now.ToString("yyyy-MM-dd");

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PC.LAST_UPLOAD_DATETIME");
            strQry.AppendLine(",TEMP_TABLE1.PAY_PERIOD_DATE_FROM");
            strQry.AppendLine(",TEMP_TABLE1.PAY_PERIOD_DATE");
            strQry.AppendLine(",TEMP_TABLE1.WAGE_RUN_IND");
            strQry.AppendLine(",TEMP_TABLE1.SALARY_RUN_IND");
            strQry.AppendLine(",TEMP_TABLE1.TIME_ATTENDANCE_RUN_IND");
            
            strQry.AppendLine(",AUTHORISED_IND = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN TEMP_TABLE2.COMPANY_NO IS NULL ");

            strQry.AppendLine(" THEN 'N' ");

            strQry.AppendLine(" ELSE 'Y' ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");
                 
            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(SELECT DISTINCT");
                strQry.AppendLine(" PAY_CATEGORY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP ");
                strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo + ") AS USER_TABLE");

                strQry.AppendLine(" ON PC.PAY_CATEGORY_NO = USER_TABLE.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = USER_TABLE.PAY_CATEGORY_TYPE");
            }
           
            strQry.AppendLine(" LEFT JOIN ");

            strQry.AppendLine("(SELECT");
            strQry.AppendLine(" PCPC.COMPANY_NO");
            strQry.AppendLine(",PCPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PCPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PCPC.PAY_PERIOD_DATE_FROM");
         
            strQry.AppendLine(",PAY_PERIOD_DATE = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN PCPC.PAY_CATEGORY_TYPE = 'S' AND NOT PCPC.SALARY_TIMESHEET_ENDDATE IS NULL");
            strQry.AppendLine(" THEN PCPC.SALARY_TIMESHEET_ENDDATE ");

            strQry.AppendLine(" ELSE PCPC.PAY_PERIOD_DATE ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",C.WAGE_RUN_IND");
            strQry.AppendLine(",C.SALARY_RUN_IND");
            strQry.AppendLine(",C.TIME_ATTENDANCE_RUN_IND");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC ");
           
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C ");
            strQry.AppendLine(" ON PCPC.COMPANY_NO = C.COMPANY_NO ");
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" WHERE PCPC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PCPC.PAY_CATEGORY_NO > 0");
            strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P') AS TEMP_TABLE1");

            strQry.AppendLine(" ON PC.COMPANY_NO = TEMP_TABLE1.COMPANY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = TEMP_TABLE1.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = TEMP_TABLE1.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" LEFT JOIN ");

            strQry.AppendLine("(SELECT");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND AUTHORISED_IND = 'Y'");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE) AS TEMP_TABLE2");

            strQry.AppendLine(" ON PC.COMPANY_NO = TEMP_TABLE2.COMPANY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = TEMP_TABLE2.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = TEMP_TABLE2.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            //Errol 2013-04-12
            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE IN ('W','S')");
            }

            strQry.AppendLine(" AND ISNULL(CLOSED_IND,'N') <> 'Y'");
            
            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" PC.PAY_CATEGORY_TYPE DESC");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategory", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_NO");

            //Errol 2013-06-15
            strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

            strQry.AppendLine(" CASE ");

            //Errol 2015-02-12
            //strQry.AppendLine(" WHEN EIH.PAY_PERIOD_DATE = E.EMPLOYEE_LAST_RUNDATE ");

            //strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

            //strQry.AppendLine(" WHEN EIH.PAY_PERIOD_DATE <> E.EMPLOYEE_LAST_RUNDATE ");

            //strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE ");

            //strQry.AppendLine(" ELSE DATEADD(DD,-40,'" + DateTime.Now.ToString("yyyy-MM-dd") + "')");

            //Errol 2015-02-12
            strQry.AppendLine(" WHEN E.EMPLOYEE_LAST_RUNDATE IS NULL ");

            strQry.AppendLine(" THEN DATEADD(DD,-40,'" + DateTime.Now.ToString("yyyy-MM-dd") + "')");

            strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y' ");

            strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

            strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");
            
            strQry.AppendLine(" END ");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            //Errol 2013-04-12
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
            //2017-10-12
            strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");
            
            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE IN ('W','S')");
            }

            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            //Errol 2013-06-15
            //Errol 2015-02-12
            //strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY EIH ");
            //strQry.AppendLine(" ON EPC.COMPANY_NO = EIH.COMPANY_NO ");
            //strQry.AppendLine(" AND EPC.EMPLOYEE_NO = EIH.EMPLOYEE_NO ");
            ////Take-On Record
            //strQry.AppendLine(" AND EIH.RUN_TYPE = 'T' ");
            //strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = EIH.PAY_CATEGORY_TYPE");
           
            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            //Removed 2012-04-06 - Allow For Employees Clocked on Client That have Not been Activated to be seen
            //strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parint64CompanyNo);
            
            //2017-09-16
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
         
            strQry.AppendLine(" EMPLOYEE_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",D.DAY_DATE");
            strQry.AppendLine(",EMPLOYEE_TABLE.EMPLOYEE_NO");
         
            strQry.AppendLine(" FROM ");

            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" E.COMPANY_NO ");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");

            strQry.AppendLine(",E.LEAVE_SHIFT_NO");

            strQry.AppendLine(",LC.LEAVE_FROM_DATE");
            strQry.AppendLine(",LC.LEAVE_TO_DATE");

            strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

            strQry.AppendLine(" CASE ");

            //Errol 2015-02-12
            strQry.AppendLine(" WHEN E.EMPLOYEE_LAST_RUNDATE IS NULL ");

            strQry.AppendLine(" THEN DATEADD(DD,-40,'" + DateTime.Now.ToString("yyyy-MM-dd") + "')");

            strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y' ");

            strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

            strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

            strQry.AppendLine(" END");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT LC ");

            strQry.AppendLine(" ON E.COMPANY_NO = LC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = LC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = LC.PAY_CATEGORY_TYPE ");

            //Not Take_on
            strQry.AppendLine(" AND LC.PROCESS_NO < 99");
            //D=Day Option,H=Hour Option
            strQry.AppendLine(" AND LC.LEAVE_OPTION = 'D'");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            //Errol 2013-06-15

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T') AS EMPLOYEE_TABLE");
            }
            else
            {
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE IN ('W','S')) AS EMPLOYEE_TABLE");
            }

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");

            strQry.AppendLine(" ON D.DAY_DATE >= EMPLOYEE_TABLE.LEAVE_FROM_DATE");
            strQry.AppendLine(" AND D.DAY_DATE <= EMPLOYEE_TABLE.LEAVE_TO_DATE");
            strQry.AppendLine(" AND D.DAY_DATE > EMPLOYEE_TABLE.EMPLOYEE_LAST_RUNDATE");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT LS");
            strQry.AppendLine(" ON EMPLOYEE_TABLE.COMPANY_NO = LS.COMPANY_NO");
            strQry.AppendLine(" AND EMPLOYEE_TABLE.LEAVE_SHIFT_NO = LS.LEAVE_SHIFT_NO");
            strQry.AppendLine(" AND EMPLOYEE_TABLE.PAY_CATEGORY_TYPE = LS.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND LS.DATETIME_DELETE_RECORD IS NULL");

            //WeekDays
            strQry.AppendLine(" AND ((ISNULL(LS.LEAVE_PAID_ACCUMULATOR_IND, 1) = 1");
            strQry.AppendLine(" AND D.DAY_NO IN(1, 2, 3, 4, 5))");
            //WeekDays + Saturday
            strQry.AppendLine(" OR(LS.LEAVE_PAID_ACCUMULATOR_IND = 2");
            strQry.AppendLine(" AND D.DAY_NO IN(1, 2, 3, 4, 5, 6))");
            //WeekDays + Saturday + Sunday
            strQry.AppendLine(" OR(LS.LEAVE_PAID_ACCUMULATOR_IND = 3");
            strQry.AppendLine(" AND D.DAY_NO IN(0, 1, 2, 3, 4, 5, 6)))");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY PH");
            strQry.AppendLine(" ON D.DAY_DATE = PH.PUBLIC_HOLIDAY_DATE");
            //Remove Public Holiday
            strQry.AppendLine(" WHERE PH.PUBLIC_HOLIDAY_DATE IS NULL");

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EMPLOYEE_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",D.DAY_DATE");
            strQry.AppendLine(",EMPLOYEE_TABLE.EMPLOYEE_NO");
            
            strQry.AppendLine(" FROM ");

            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" E.COMPANY_NO ");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.LEAVE_SHIFT_NO");
            strQry.AppendLine(",LH.LEAVE_FROM_DATE");
            strQry.AppendLine(",LH.LEAVE_TO_DATE");

            strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

            strQry.AppendLine(" CASE ");

            //Errol 2015-02-12
            strQry.AppendLine(" WHEN E.EMPLOYEE_LAST_RUNDATE IS NULL ");

            strQry.AppendLine(" THEN DATEADD(DD,-40,'" + DateTime.Now.ToString("yyyy-MM-dd") + "')");

            strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y' ");

            strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

            strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

            strQry.AppendLine(" END");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY LH ");

            strQry.AppendLine(" ON E.COMPANY_NO = LH.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = LH.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = LH.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND LH.PAY_PERIOD_DATE >= '" + DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd") + "'");

            //Not Take_on
            strQry.AppendLine(" AND LH.PROCESS_NO < 99");
            //D=Day Option,H=Hour Option
            strQry.AppendLine(" AND LH.LEAVE_OPTION = 'D'");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            //Errol 2013-06-15
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            
            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T') AS EMPLOYEE_TABLE");
            }
            else
            {
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE IN ('W','S')) AS EMPLOYEE_TABLE");
            }
            
            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");
            strQry.AppendLine(" ON D.DAY_DATE >= EMPLOYEE_TABLE.LEAVE_FROM_DATE");
            strQry.AppendLine(" AND D.DAY_DATE <= EMPLOYEE_TABLE.LEAVE_TO_DATE");
            strQry.AppendLine(" AND D.DAY_DATE > EMPLOYEE_TABLE.EMPLOYEE_LAST_RUNDATE");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT LS");
            strQry.AppendLine(" ON EMPLOYEE_TABLE.COMPANY_NO = LS.COMPANY_NO");
            strQry.AppendLine(" AND EMPLOYEE_TABLE.LEAVE_SHIFT_NO = LS.LEAVE_SHIFT_NO");
            strQry.AppendLine(" AND EMPLOYEE_TABLE.PAY_CATEGORY_TYPE = LS.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND LS.DATETIME_DELETE_RECORD IS NULL");

            //WeekDays
            strQry.AppendLine(" AND ((ISNULL(LS.LEAVE_PAID_ACCUMULATOR_IND, 1) = 1");
            strQry.AppendLine(" AND D.DAY_NO IN(1, 2, 3, 4, 5))");
            //WeekDays + Saturday
            strQry.AppendLine(" OR(LS.LEAVE_PAID_ACCUMULATOR_IND = 2");
            strQry.AppendLine(" AND D.DAY_NO IN(1, 2, 3, 4, 5, 6))");
            //WeekDays + Saturday + Sunday
            strQry.AppendLine(" OR(LS.LEAVE_PAID_ACCUMULATOR_IND = 3");
            strQry.AppendLine(" AND D.DAY_NO IN(0, 1, 2, 3, 4, 5, 6)))");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY PH");
            strQry.AppendLine(" ON D.DAY_DATE = PH.PUBLIC_HOLIDAY_DATE");
            //Remove Public Holiday
            strQry.AppendLine(" WHERE PH.PUBLIC_HOLIDAY_DATE IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeLeave", parint64CompanyNo);
            
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PH.PUBLIC_HOLIDAY_DATE");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY PH");

            strQry.AppendLine(" WHERE PH.PUBLIC_HOLIDAY_DATE > '" + DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd") + "'");
            strQry.AppendLine(" AND PH.PUBLIC_HOLIDAY_DATE <= '" + DateTime.Now.AddDays(15).ToString("yyyy-MM-dd") + "'");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PH.PUBLIC_HOLIDAY_DATE");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PublicHoliday", parint64CompanyNo);
            
            if (DataSet.Tables["PayCategory"].Rows.Count > 0)
            {
                DataSet TempDataSet = Get_PayCategory_DataSet(parint64CompanyNo, Convert.ToInt32(DataSet.Tables["PayCategory"].Rows[0]["PAY_CATEGORY_NO"]), DataSet.Tables["PayCategory"].Rows[0]["PAY_CATEGORY_TYPE"].ToString(), parint64CurrentUserNo, parstrCurrentUserAccessInd,parstrFromProgram);
                DataSet.Merge(TempDataSet);
            }

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_PayCategory_Records(Int64 parint64CompanyNo, int parintPayCategoryNo, string parstrPayrollType, Int64 parint64CurrentUserNo, string parstrCurrentUserAccessInd, string parstrFromProgram)
        {
            DataSet DataSet = Get_PayCategory_DataSet(parint64CompanyNo, parintPayCategoryNo, parstrPayrollType, parint64CurrentUserNo, parstrCurrentUserAccessInd,parstrFromProgram);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        private DataSet Get_PayCategory_DataSet(Int64 parint64CompanyNo, int parintPayCategoryNo, string parstrPayrollType, Int64 parint64CurrentUserNo, string parstrCurrentUserAccessInd,string parstrFromProgram)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategoryLoaded", parint64CompanyNo);
              
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" TEMP_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",TEMP_TABLE.PAY_CATEGORY_TYPE");

            strQry.AppendLine(",MIN(D.DAY_DATE) AS FROM_DATE");

            strQry.AppendLine(" FROM ");

            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" E.COMPANY_NO ");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");

            //Errol 2013-06-15
            strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

            strQry.AppendLine(" MIN(CASE ");

            //Errol 2015-02-12
            //strQry.AppendLine(" WHEN EIH.PAY_PERIOD_DATE = E.EMPLOYEE_LAST_RUNDATE ");

            //strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

            //strQry.AppendLine(" WHEN EIH.PAY_PERIOD_DATE <> E.EMPLOYEE_LAST_RUNDATE ");

            //strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE ");

            //strQry.AppendLine(" ELSE DATEADD(DD,-40,'" + DateTime.Now.ToString("yyyy-MM-dd") + "')");

            //Errol 2015-02-12
            strQry.AppendLine(" WHEN E.EMPLOYEE_LAST_RUNDATE IS NULL ");

            strQry.AppendLine(" THEN DATEADD(DD,-40,'" + DateTime.Now.ToString("yyyy-MM-dd") + "')");

            strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y' ");

            strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

            strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

            strQry.AppendLine(" END) ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

            //Errol 2013-04-12
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE IN ('W','S')");
            }

            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            //Errol 2013-06-15
            //Errol 2015-02-12
            //strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY EIH ");
            //strQry.AppendLine(" ON EPC.COMPANY_NO = EIH.COMPANY_NO ");
            //strQry.AppendLine(" AND EPC.EMPLOYEE_NO = EIH.EMPLOYEE_NO ");
            ////Take-On Record
            //strQry.AppendLine(" AND EIH.RUN_TYPE = 'T' ");
            //strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = EIH.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            //Errol 2013-06-15
            //strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE IN ('W','S')");
            }

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" E.COMPANY_NO ");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");

            strQry.AppendLine(",EPC.PAY_CATEGORY_NO) AS TEMP_TABLE");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");
            strQry.AppendLine(" ON D.DAY_DATE > TEMP_TABLE.EMPLOYEE_LAST_RUNDATE");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" TEMP_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",TEMP_TABLE.PAY_CATEGORY_TYPE");
         
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "FromDate", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ETC.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETC.EMPLOYEE_NO");
            strQry.AppendLine(",ETC.TIMESHEET_DATE");
            strQry.AppendLine(",ETC.TIMESHEET_SEQ");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_OUT_MINUTES");
            strQry.AppendLine(",ETC.CLOCKED_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC.CLOCKED_TIME_OUT_MINUTES");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            
            strQry.AppendLine(" INNER JOIN  InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            //Errol 2013-04-12
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC ");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC ");
                }
                else
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC ");
                }
            }

            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO ");
           
            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

            //strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Timesheet", parint64CompanyNo);
            
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EBC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBC.EMPLOYEE_NO");

            strQry.AppendLine(",EBC.BREAK_DATE");
            strQry.AppendLine(",EBC.BREAK_SEQ");
            strQry.AppendLine(",EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",EBC.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine(",EBC.CLOCKED_TIME_IN_MINUTES");
            strQry.AppendLine(",EBC.CLOCKED_TIME_OUT_MINUTES");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN  InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            //Errol 2013-04-12
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT EBC ");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT EBC ");
                }
                else
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT EBC ");
                }
            }

            strQry.AppendLine(" ON E.COMPANY_NO = EBC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EBC.EMPLOYEE_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = EBC.PAY_CATEGORY_NO ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

            //strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Break", parint64CompanyNo);
           
            return DataSet;
        }

        public void Delete_TimeSheet_Day_Records(Int64 parint64CompanyNo, int parintPayCategoryNo, string parstrPayrollType, string parstrArrayDayDateTimes,
                                                string parstrRecordType,string parstrCurrentUserAccessInd, Int64 parint64CurrentUserNo)
        {
            string[] pardtDayDateTime = parstrArrayDayDateTimes.Split('|');
            StringBuilder strQry = new StringBuilder();
            StringBuilder strQryInsert = new StringBuilder();
            
            DataSet DataSetTemp = new DataSet();

            string strTableDef = "";

            int intTimesheetBreakDeleteId = -1;

            if (parstrRecordType == "T")
            {
                strTableDef = "TIMESHEET";
            }
            else
            {
                strTableDef = "BREAK";
            }

            for (int intCount = 0; intCount < pardtDayDateTime.Length; intCount++)
            {
                if (intTimesheetBreakDeleteId == -1)
                {
                    //2017-10-21
                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.TIMESHEET_BREAK_DELETE");
                    strQry.AppendLine("(USER_NO");
                    strQry.AppendLine(",TIMESHEET_BREAK_DELETE_DATETIME)");

                    strQry.AppendLine(" VALUES");

                    strQry.AppendLine("(" + parint64CurrentUserNo);
                    strQry.AppendLine(",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" MAX(TIMESHEET_BREAK_DELETE_ID) AS TIMESHEET_BREAK_DELETE_ID");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.TIMESHEET_BREAK_DELETE");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSetTemp, "TimesheetBreakDelete", parint64CompanyNo);

                    intTimesheetBreakDeleteId = Convert.ToInt32(DataSetTemp.Tables["TimesheetBreakDelete"].Rows[0]["TIMESHEET_BREAK_DELETE_ID"]);
                }

                strQryInsert.Clear();

                if (parstrRecordType == "T")
                {
                    if (parstrPayrollType == "W")
                    {
                        strQryInsert.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT_DELETE");
                    }
                    else
                    {
                        if (parstrPayrollType == "S")
                        {
                            strQryInsert.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT_DELETE");
                        }
                        else
                        {
                            strQryInsert.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT_DELETE");
                        }
                    }

                    strQryInsert.AppendLine("(USER_NO");
                    strQryInsert.AppendLine(",TIMESHEET_BREAK_DELETE_ID");
                    strQryInsert.AppendLine(",COMPANY_NO");
                    strQryInsert.AppendLine(",EMPLOYEE_NO ");
                    strQryInsert.AppendLine(",PAY_CATEGORY_NO");
                    strQryInsert.AppendLine(",TIMESHEET_DATE");
                    strQryInsert.AppendLine(",TIMESHEET_SEQ");
                    strQryInsert.AppendLine(",TIMESHEET_TIME_IN_MINUTES");
                    strQryInsert.AppendLine(",TIMESHEET_TIME_OUT_MINUTES");
                    strQryInsert.AppendLine(",CLOCKED_TIME_IN_MINUTES");
                    strQryInsert.AppendLine(",CLOCKED_TIME_OUT_MINUTES");
                    strQryInsert.AppendLine(",INCLUDED_IN_RUN_IND");
                    strQryInsert.AppendLine(",INDICATOR");
                    strQryInsert.AppendLine(",TIMESHEET_ACCUM_MINUTES");
                    strQryInsert.AppendLine(",USER_NO_TIME_IN");
                    strQryInsert.AppendLine(",USER_NO_TIME_OUT)");

                    strQryInsert.AppendLine(" SELECT ");

                    strQryInsert.AppendLine(parint64CurrentUserNo.ToString());
                    strQryInsert.AppendLine("," + intTimesheetBreakDeleteId);
                    strQryInsert.AppendLine(",ETC.COMPANY_NO");
                    strQryInsert.AppendLine(",ETC.EMPLOYEE_NO ");
                    strQryInsert.AppendLine(",ETC.PAY_CATEGORY_NO");
                    strQryInsert.AppendLine(",ETC.TIMESHEET_DATE");
                    strQryInsert.AppendLine(",ETC.TIMESHEET_SEQ");
                    strQryInsert.AppendLine(",ETC.TIMESHEET_TIME_IN_MINUTES");
                    strQryInsert.AppendLine(",ETC.TIMESHEET_TIME_OUT_MINUTES");
                    strQryInsert.AppendLine(",ETC.CLOCKED_TIME_IN_MINUTES");
                    strQryInsert.AppendLine(",ETC.CLOCKED_TIME_OUT_MINUTES");
                    strQryInsert.AppendLine(",ETC.INCLUDED_IN_RUN_IND");
                    strQryInsert.AppendLine(",ETC.INDICATOR");
                    strQryInsert.AppendLine(",ETC.TIMESHEET_ACCUM_MINUTES");
                    strQryInsert.AppendLine(",ETC.USER_NO_TIME_IN");
                    strQryInsert.AppendLine(",ETC.USER_NO_TIME_OUT");

                    strTableDef = "TIMESHEET";
                }
                else
                {
                    if (parstrPayrollType == "W")
                    {
                        strQryInsert.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT_DELETE");
                    }
                    else
                    {
                        if (parstrPayrollType == "S")
                        {
                            strQryInsert.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT_DELETE");
                        }
                        else
                        {
                            strQryInsert.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT_DELETE");
                        }
                    }

                    strQryInsert.AppendLine("(USER_NO");
                    strQryInsert.AppendLine(",TIMESHEET_BREAK_DELETE_ID");
                    strQryInsert.AppendLine(",COMPANY_NO");
                    strQryInsert.AppendLine(",EMPLOYEE_NO ");
                    strQryInsert.AppendLine(",PAY_CATEGORY_NO");

                    strQryInsert.AppendLine(",BREAK_DATE");
                    strQryInsert.AppendLine(",BREAK_SEQ");
                    strQryInsert.AppendLine(",BREAK_TIME_IN_MINUTES");
                    strQryInsert.AppendLine(",BREAK_TIME_OUT_MINUTES");
                    strQryInsert.AppendLine(",CLOCKED_TIME_IN_MINUTES");
                    strQryInsert.AppendLine(",CLOCKED_TIME_OUT_MINUTES");
                    strQryInsert.AppendLine(",INCLUDED_IN_RUN_IND");
                    strQryInsert.AppendLine(",INDICATOR");
                    strQryInsert.AppendLine(",BREAK_ACCUM_MINUTES");
                    strQryInsert.AppendLine(",USER_NO_TIME_IN");
                    strQryInsert.AppendLine(",USER_NO_TIME_OUT)");

                    strQryInsert.AppendLine(" SELECT ");

                    strQryInsert.AppendLine(parint64CurrentUserNo.ToString());
                    strQryInsert.AppendLine("," + intTimesheetBreakDeleteId);
                    strQryInsert.AppendLine(",ETC.COMPANY_NO");
                    strQryInsert.AppendLine(",ETC.EMPLOYEE_NO ");
                    strQryInsert.AppendLine(",ETC.PAY_CATEGORY_NO");
                    strQryInsert.AppendLine(",ETC.BREAK_DATE");
                    strQryInsert.AppendLine(",ETC.BREAK_SEQ");
                    strQryInsert.AppendLine(",ETC.BREAK_TIME_IN_MINUTES");
                    strQryInsert.AppendLine(",ETC.BREAK_TIME_OUT_MINUTES");
                    strQryInsert.AppendLine(",ETC.CLOCKED_TIME_IN_MINUTES");
                    strQryInsert.AppendLine(",ETC.CLOCKED_TIME_OUT_MINUTES");
                    strQryInsert.AppendLine(",ETC.INCLUDED_IN_RUN_IND");
                    strQryInsert.AppendLine(",ETC.INDICATOR");
                    strQryInsert.AppendLine(",ETC.BREAK_ACCUM_MINUTES");
                    strQryInsert.AppendLine(",ETC.USER_NO_TIME_IN");
                    strQryInsert.AppendLine(",ETC.USER_NO_TIME_OUT");

                    strTableDef = "BREAK";
                }

                strQry.Clear();

                if (parstrPayrollType == "W")
                {
                    strQry.AppendLine(" DELETE ETC FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ETC");
                }
                else
                {
                    if (parstrPayrollType == "S")
                    {
                        strQry.AppendLine(" DELETE ETC FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ETC");
                    }
                    else
                    {
                        strQry.AppendLine(" DELETE ETC FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_" + strTableDef + "_CURRENT ETC");
                    }
                }

                if (parstrCurrentUserAccessInd == "U")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                    strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                    strQry.AppendLine(" AND ETC.COMPANY_NO = UEPCT.COMPANY_NO ");
                    strQry.AppendLine(" AND ETC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                }

                strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + parintPayCategoryNo);
                strQry.AppendLine(" AND ETC." + strTableDef + "_DATE = '" + pardtDayDateTime[intCount].ToString() + "'");
                
                //2017-10-21 - Insert into Backup Tables before Delete
                strQryInsert.Append(strQry);
                strQryInsert.Replace("DELETE ETC ", "");

                clsDBConnectionObjects.Execute_SQLCommand(strQryInsert.ToString(), parint64CompanyNo);
                
                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            }

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }

        public byte[] Insert_Update_Clocking_Records(Int64 parint64CompanyNo, int parintPayCategoryNo, string parstrPayrollType, string parstrArrayDayDateTime, 
                                                   string parstrOptionInd, string parstrArrayEmployeeNo, string parstrArrayEmployeeSeqNo, int parintTimeInMinutes,
                                                   int parintTimeOutMinutes, string parstrUpdateInd, string parstrRecordtype, Int64 parint64CurrentUserNo,
                                                   string parstrCurrentUserAccessInd, string parstrFromProgram)
        {
            string[] strDayDateTime = parstrArrayDayDateTime.Split('|');
            string[] strEmployeeNo = parstrArrayEmployeeNo.Split('|');
            string[] strEmployeeSeqNo = parstrArrayEmployeeSeqNo.Split('|');

            string strTableDef = "";

            if (parstrRecordtype == "T")
            {
                strTableDef = "TIMESHEET";

            }
            else
            {
                strTableDef = "BREAK";
            }

            DataSet DataSetTemp = new DataSet();

            int intTimesheetBreakDeleteId = -1;

            StringBuilder strQry = new StringBuilder();
            StringBuilder strQryInsert = new StringBuilder();

            for (int intCount = 0; intCount < strDayDateTime.Length; intCount++)
            {
                for (int intRow = 0; intRow < strEmployeeNo.Length; intRow++)
                {
                    if (parstrOptionInd == "D")
                    {
                        if (intTimesheetBreakDeleteId == -1)
                        {
                            //2017-10-21
                            strQry.Clear();

                            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.TIMESHEET_BREAK_DELETE");
                            strQry.AppendLine("(USER_NO");
                            strQry.AppendLine(",TIMESHEET_BREAK_DELETE_DATETIME)");

                            strQry.AppendLine(" VALUES");

                            strQry.AppendLine("(" + parint64CurrentUserNo);
                            strQry.AppendLine(",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')");

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                            strQry.Clear();

                            strQry.AppendLine(" SELECT ");
                            strQry.AppendLine(" MAX(TIMESHEET_BREAK_DELETE_ID) AS TIMESHEET_BREAK_DELETE_ID");

                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.TIMESHEET_BREAK_DELETE");

                            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSetTemp, "TimesheetBreakDelete", parint64CompanyNo);

                            intTimesheetBreakDeleteId = Convert.ToInt32(DataSetTemp.Tables["TimesheetBreakDelete"].Rows[0]["TIMESHEET_BREAK_DELETE_ID"]);
                        }

                        strQryInsert.Clear();

                        if (parstrRecordtype == "T")
                        {
                            if (parstrPayrollType == "W")
                            {
                                strQryInsert.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT_DELETE");
                            }
                            else
                            {
                                if (parstrPayrollType == "S")
                                {
                                    strQryInsert.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT_DELETE");
                                }
                                else
                                {
                                    strQryInsert.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT_DELETE");
                                }
                            }

                            strQryInsert.AppendLine("(USER_NO");
                            strQryInsert.AppendLine(",TIMESHEET_BREAK_DELETE_ID");
                            strQryInsert.AppendLine(",COMPANY_NO");
                            strQryInsert.AppendLine(",EMPLOYEE_NO ");
                            strQryInsert.AppendLine(",PAY_CATEGORY_NO");
                            strQryInsert.AppendLine(",TIMESHEET_DATE");
                            strQryInsert.AppendLine(",TIMESHEET_SEQ");
                            strQryInsert.AppendLine(",TIMESHEET_TIME_IN_MINUTES");
                            strQryInsert.AppendLine(",TIMESHEET_TIME_OUT_MINUTES");
                            strQryInsert.AppendLine(",CLOCKED_TIME_IN_MINUTES");
                            strQryInsert.AppendLine(",CLOCKED_TIME_OUT_MINUTES");
                            strQryInsert.AppendLine(",INCLUDED_IN_RUN_IND");
                            strQryInsert.AppendLine(",INDICATOR");
                            strQryInsert.AppendLine(",TIMESHEET_ACCUM_MINUTES");
                            strQryInsert.AppendLine(",USER_NO_TIME_IN");
                            strQryInsert.AppendLine(",USER_NO_TIME_OUT)");

                            strQryInsert.AppendLine(" SELECT ");

                            strQryInsert.AppendLine(parint64CurrentUserNo.ToString());
                            strQryInsert.AppendLine("," + intTimesheetBreakDeleteId);
                            strQryInsert.AppendLine(",COMPANY_NO");
                            strQryInsert.AppendLine(",EMPLOYEE_NO ");
                            strQryInsert.AppendLine(",PAY_CATEGORY_NO");
                            strQryInsert.AppendLine(",TIMESHEET_DATE");
                            strQryInsert.AppendLine(",TIMESHEET_SEQ");
                            strQryInsert.AppendLine(",TIMESHEET_TIME_IN_MINUTES");
                            strQryInsert.AppendLine(",TIMESHEET_TIME_OUT_MINUTES");
                            strQryInsert.AppendLine(",CLOCKED_TIME_IN_MINUTES");
                            strQryInsert.AppendLine(",CLOCKED_TIME_OUT_MINUTES");
                            strQryInsert.AppendLine(",INCLUDED_IN_RUN_IND");
                            strQryInsert.AppendLine(",INDICATOR");
                            strQryInsert.AppendLine(",TIMESHEET_ACCUM_MINUTES");
                            strQryInsert.AppendLine(",USER_NO_TIME_IN");
                            strQryInsert.AppendLine(",USER_NO_TIME_OUT");
                        }
                        else
                        {
                            if (parstrPayrollType == "W")
                            {
                                strQryInsert.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT_DELETE");
                            }
                            else
                            {
                                if (parstrPayrollType == "S")
                                {
                                    strQryInsert.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT_DELETE");
                                }
                                else
                                {
                                    strQryInsert.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT_DELETE");
                                }
                            }

                            strQryInsert.AppendLine("(USER_NO");
                            strQryInsert.AppendLine(",TIMESHEET_BREAK_DELETE_ID");
                            strQryInsert.AppendLine(",COMPANY_NO");
                            strQryInsert.AppendLine(",EMPLOYEE_NO ");
                            strQryInsert.AppendLine(",PAY_CATEGORY_NO");

                            strQryInsert.AppendLine(",BREAK_DATE");
                            strQryInsert.AppendLine(",BREAK_SEQ");
                            strQryInsert.AppendLine(",BREAK_TIME_IN_MINUTES");
                            strQryInsert.AppendLine(",BREAK_TIME_OUT_MINUTES");
                            strQryInsert.AppendLine(",CLOCKED_TIME_IN_MINUTES");
                            strQryInsert.AppendLine(",CLOCKED_TIME_OUT_MINUTES");
                            strQryInsert.AppendLine(",INCLUDED_IN_RUN_IND");
                            strQryInsert.AppendLine(",INDICATOR");
                            strQryInsert.AppendLine(",BREAK_ACCUM_MINUTES");
                            strQryInsert.AppendLine(",USER_NO_TIME_IN");
                            strQryInsert.AppendLine(",USER_NO_TIME_OUT)");

                            strQryInsert.AppendLine(" SELECT ");

                            strQryInsert.AppendLine(parint64CurrentUserNo.ToString());
                            strQryInsert.AppendLine("," + intTimesheetBreakDeleteId);
                            strQryInsert.AppendLine(",COMPANY_NO");
                            strQryInsert.AppendLine(",EMPLOYEE_NO ");
                            strQryInsert.AppendLine(",PAY_CATEGORY_NO");
                            strQryInsert.AppendLine(",BREAK_DATE");
                            strQryInsert.AppendLine(",BREAK_SEQ");
                            strQryInsert.AppendLine(",BREAK_TIME_IN_MINUTES");
                            strQryInsert.AppendLine(",BREAK_TIME_OUT_MINUTES");
                            strQryInsert.AppendLine(",CLOCKED_TIME_IN_MINUTES");
                            strQryInsert.AppendLine(",CLOCKED_TIME_OUT_MINUTES");
                            strQryInsert.AppendLine(",INCLUDED_IN_RUN_IND");
                            strQryInsert.AppendLine(",INDICATOR");
                            strQryInsert.AppendLine(",BREAK_ACCUM_MINUTES");
                            strQryInsert.AppendLine(",USER_NO_TIME_IN");
                            strQryInsert.AppendLine(",USER_NO_TIME_OUT");
                        }

                        //Delete
                        strQry.Clear();

                        if (parstrPayrollType == "W")
                        {
                            strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ");
                        }
                        else
                        {
                            if (parstrPayrollType == "S")
                            {
                                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ");
                            }
                            else
                            {
                                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_" + strTableDef + "_CURRENT ");
                            }
                        }

                        strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + strEmployeeNo[intRow].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
                        strQry.AppendLine(" AND " + strTableDef + "_DATE = '" + strDayDateTime[intCount].ToString() + "'");
                        strQry.AppendLine(" AND " + strTableDef + "_SEQ = " + strEmployeeSeqNo[intRow].ToString());
                        
                        //2017-10-21 - Insert into Backup Tables before Delete
                        strQryInsert.Append(strQry);
                        strQryInsert.Replace("DELETE ", "");

                        clsDBConnectionObjects.Execute_SQLCommand(strQryInsert.ToString(), parint64CompanyNo);
                        
                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                    }
                    else
                    {
                        if (parstrOptionInd == "B")
                        {
                            strQry.Clear();

                            if (parstrUpdateInd == "Y")
                            {
                                if (parstrPayrollType == "W")
                                {
                                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ");
                                }
                                else
                                {
                                    if (parstrPayrollType == "S")
                                    {
                                        strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ");
                                    }
                                    else
                                    {
                                        strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_" + strTableDef + "_CURRENT ");
                                    }
                                }

                                strQry.AppendLine(" SET ");
                                strQry.AppendLine(strTableDef + "_TIME_IN_MINUTES = " + parintTimeInMinutes);
                                strQry.AppendLine(",USER_NO_TIME_IN = " + parint64CurrentUserNo.ToString());
                                strQry.AppendLine("," + strTableDef + "_TIME_OUT_MINUTES = " + parintTimeOutMinutes);
                                strQry.AppendLine(",USER_NO_TIME_OUT = " + parint64CurrentUserNo.ToString());
                            }
                            else
                            {
                                if (parstrPayrollType == "W")
                                {
                                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ");
                                }
                                else
                                {
                                    if (parstrPayrollType == "S")
                                    {
                                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ");
                                    }
                                    else
                                    {
                                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_" + strTableDef + "_CURRENT ");
                                    }
                                }

                                strQry.AppendLine("(COMPANY_NO");
                                strQry.AppendLine(",EMPLOYEE_NO");
                                strQry.AppendLine(",PAY_CATEGORY_NO");
                                strQry.AppendLine("," + strTableDef + "_DATE");
                                strQry.AppendLine("," + strTableDef + "_SEQ");
                                strQry.AppendLine("," + strTableDef + "_TIME_IN_MINUTES");
                                strQry.AppendLine(",USER_NO_TIME_IN");
                                strQry.AppendLine("," + strTableDef + "_TIME_OUT_MINUTES");
                                strQry.AppendLine(",USER_NO_TIME_OUT)");

                                strQry.AppendLine(" SELECT ");
                                strQry.AppendLine(parint64CompanyNo.ToString());

                                strQry.AppendLine("," + strEmployeeNo[intRow].ToString());
                                strQry.AppendLine("," + parintPayCategoryNo);
                                strQry.AppendLine(",'" + strDayDateTime[intCount].ToString() + "'");
                                strQry.AppendLine(",ISNULL(MAX(" + strTableDef + "_SEQ),0) + 1");
                                strQry.AppendLine("," + parintTimeInMinutes);
                                strQry.AppendLine("," + parint64CurrentUserNo.ToString());
                                strQry.AppendLine("," + parintTimeOutMinutes);
                                strQry.AppendLine("," + parint64CurrentUserNo.ToString());

                                strQry.AppendLine(" FROM ");

                                if (parstrPayrollType == "W")
                                {
                                    strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ");
                                }
                                else
                                {
                                    if (parstrPayrollType == "S")
                                    {
                                        strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ");
                                    }
                                    else
                                    {
                                        strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_" + strTableDef + "_CURRENT ");
                                    }
                                }
                            }

                            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + strEmployeeNo[intRow].ToString());
                            strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
                            strQry.AppendLine(" AND " + strTableDef + "_DATE = '" + strDayDateTime[intCount].ToString() + "'");

                            if (parstrUpdateInd == "Y")
                            {
                                strQry.AppendLine(" AND " + strTableDef + "_SEQ = " + parstrArrayEmployeeSeqNo[intRow]);
                            }

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                        }
                        else
                        {
                            if (strEmployeeSeqNo[intRow] == "-1")
                            {
                                //Insert
                                strQry.Clear();

                                if (parstrPayrollType == "W")
                                {
                                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ");
                                }
                                else
                                {
                                    if (parstrPayrollType == "S")
                                    {
                                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ");
                                    }
                                    else
                                    {
                                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_" + strTableDef + "_CURRENT ");
                                    }
                                }

                                strQry.AppendLine("(COMPANY_NO");
                                strQry.AppendLine(",EMPLOYEE_NO");
                                strQry.AppendLine(",PAY_CATEGORY_NO");
                                strQry.AppendLine("," + strTableDef + "_DATE");
                                strQry.AppendLine("," + strTableDef + "_SEQ");

                                if (parstrOptionInd == "I")
                                {
                                    strQry.AppendLine("," + strTableDef + "_TIME_IN_MINUTES");
                                    strQry.AppendLine(",USER_NO_TIME_IN)");
                                }
                                else
                                {
                                    strQry.AppendLine("," + strTableDef + "_TIME_OUT_MINUTES");
                                    strQry.AppendLine(",USER_NO_TIME_OUT)");
                                }

                                strQry.AppendLine(" SELECT ");
                                strQry.AppendLine(parint64CompanyNo.ToString());
                                strQry.AppendLine("," + strEmployeeNo[intRow].ToString());
                                strQry.AppendLine("," + parintPayCategoryNo);
                                strQry.AppendLine(",'" + strDayDateTime[intCount].ToString() + "'");
                                strQry.AppendLine(",ISNULL(MAX(" + strTableDef + "_SEQ),0) + 1");

                                if (parstrOptionInd == "I")
                                {
                                    strQry.AppendLine("," + parintTimeInMinutes);
                                }
                                else
                                {
                                    strQry.AppendLine("," + parintTimeOutMinutes);
                                }

                                strQry.AppendLine("," + parint64CurrentUserNo.ToString());

                                strQry.AppendLine(" FROM ");

                                if (parstrPayrollType == "W")
                                {
                                    strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ");
                                }
                                else
                                {
                                    if (parstrPayrollType == "S")
                                    {
                                        strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ");
                                    }
                                    else
                                    {
                                        strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_" + strTableDef + "_CURRENT ");
                                    }
                                }

                                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                                strQry.AppendLine(" AND EMPLOYEE_NO = " + strEmployeeNo[intRow].ToString());
                                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
                                strQry.AppendLine(" AND " + strTableDef + "_DATE = '" + strDayDateTime[intCount].ToString() + "'");

                                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                            }
                            else
                            {
                                if (parstrOptionInd == "I")
                                {
                                    strQry.Clear();

                                    if (parstrPayrollType == "W")
                                    {
                                        strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ");
                                    }
                                    else
                                    {
                                        if (parstrPayrollType == "S")
                                        {
                                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ");
                                        }
                                        else
                                        {
                                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_" + strTableDef + "_CURRENT ");
                                        }
                                    }

                                    strQry.AppendLine(" SET " + strTableDef + "_TIME_IN_MINUTES = " + parintTimeInMinutes);
                                    strQry.AppendLine(",USER_NO_TIME_IN = " + parint64CurrentUserNo.ToString());

                                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                                    strQry.AppendLine(" AND EMPLOYEE_NO = " + strEmployeeNo[intRow].ToString());
                                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
                                    strQry.AppendLine(" AND " + strTableDef + "_DATE = '" + strDayDateTime[intCount].ToString() + "'");
                                    strQry.AppendLine(" AND " + strTableDef + "_SEQ = " + strEmployeeSeqNo[intRow].ToString());

                                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                                }
                                else
                                {
                                    //Out
                                    strQry.Clear();

                                    if (parstrPayrollType == "W")
                                    {
                                        strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ");
                                    }
                                    else
                                    {
                                        if (parstrPayrollType == "S")
                                        {
                                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ");
                                        }
                                        else
                                        {
                                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_" + strTableDef + "_CURRENT ");
                                        }
                                    }

                                    strQry.AppendLine(" SET " + strTableDef + "_TIME_OUT_MINUTES = " + parintTimeOutMinutes);

                                    strQry.AppendLine(",USER_NO_TIME_OUT = " + parint64CurrentUserNo.ToString());
                                    
                                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                                    strQry.AppendLine(" AND EMPLOYEE_NO = " + strEmployeeNo[intRow].ToString());
                                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
                                    strQry.AppendLine(" AND " + strTableDef + "_DATE = '" + strDayDateTime[intCount].ToString() + "'");
                                    strQry.AppendLine(" AND " + strTableDef + "_SEQ = " + strEmployeeSeqNo[intRow].ToString());

                                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                                }
                            }
                        }
                    }
                }
            }

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            byte[] bytCompress = Get_PayCategory_Records(parint64CompanyNo, parintPayCategoryNo, parstrPayrollType, parint64CurrentUserNo, parstrCurrentUserAccessInd, parstrFromProgram);
            
            return bytCompress;
        }
    }
}
