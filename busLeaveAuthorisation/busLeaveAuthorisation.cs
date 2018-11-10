using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busLeaveAuthorisation
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busLeaveAuthorisation()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo, Int64 parint64CurrentUserNo, string parstrCurrentUserAccessInd, string parstrFromProgram)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" C.WAGE_RUN_IND");
            strQry.AppendLine(",C.SALARY_RUN_IND");
            strQry.AppendLine(",C.TIME_ATTENDANCE_RUN_IND");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

            strQry.AppendLine(" WHERE C.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");
          
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", parint64CompanyNo);

            byte[] byteArrayDataset = Get_User_Level_Records(parint64CompanyNo, parint64CurrentUserNo, parstrCurrentUserAccessInd, parstrFromProgram);

            DataSet TempDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(byteArrayDataset);

            DataSet.Merge(TempDataSet);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" LC.COMPANY_NO");
            strQry.AppendLine(",LC.EMPLOYEE_NO");
            strQry.AppendLine(",LC.EARNING_NO");
            strQry.AppendLine(",LC.LEAVE_REC_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",LC.LEAVE_DESC");
            strQry.AppendLine(",LC.PROCESS_NO");
            strQry.AppendLine(",LC.LEAVE_FROM_DATE");
            strQry.AppendLine(",LC.LEAVE_TO_DATE");

            strQry.AppendLine(",ISNULL(LC.LEAVE_OPTION,'D') AS LEAVE_OPTION");
            strQry.AppendLine(",ROUND(ISNULL(LC.LEAVE_HOURS_DECIMAL,0),2) AS LEAVE_HOURS_DECIMAL");
            strQry.AppendLine(",ROUND(ISNULL(LC.LEAVE_DAYS_DECIMAL,0),2) AS LEAVE_DAYS_DECIMAL");
            strQry.AppendLine(",DATEDIFF(d,LEAVE_FROM_DATE,LEAVE_TO_DATE ) + 1 AS DATE_DIFF_NO_DAYS");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT LC");

            //2013-09-10
            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN ");
             
                strQry.AppendLine("(SELECT DISTINCT");
                strQry.AppendLine(" EMPLOYEE_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP ");
                strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo + ") AS USER_TABLE");

                strQry.AppendLine(" ON LC.EMPLOYEE_NO = USER_TABLE.EMPLOYEE_NO");
                strQry.AppendLine(" AND LC.PAY_CATEGORY_TYPE = USER_TABLE.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON LC.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND LC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND LC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
            //Default Pay Category Parameters Apply to Leave
            strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" WHERE LC.COMPANY_NO = " + parint64CompanyNo);

            //Next Run
            strQry.AppendLine(" AND LC.PROCESS_NO = 0");


            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND LC.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND LC.PAY_CATEGORY_TYPE IN ('W','S')");
            }
            
            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" LEAVE_FROM_DATE");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Leave", parint64CompanyNo);

            DataSet.Tables.Add("PayrollType");
            DataTable PayrollTypeDataTable = new DataTable("PayrollType");
            DataSet.Tables["PayrollType"].Columns.Add("PAYROLL_TYPE_DESC", typeof(String));

            if (DataSet.Tables["AuthorsiseLevel"].Rows.Count > 0)
            {
                DataView PayrollTypeDataView = new DataView(DataSet.Tables["AuthorsiseLevel"],
                 "PAY_CATEGORY_TYPE = 'W'",
                 "",
                 DataViewRowState.CurrentRows);

                if (PayrollTypeDataView.Count > 0)
                {
                    DataRow drDataRow = DataSet.Tables["PayrollType"].NewRow();

                    drDataRow["PAYROLL_TYPE_DESC"] = "Wages";

                    DataSet.Tables["PayrollType"].Rows.Add(drDataRow);
                }

                PayrollTypeDataView = null;
                PayrollTypeDataView = new DataView(DataSet.Tables["AuthorsiseLevel"],
                    "PAY_CATEGORY_TYPE = 'S'",
                    "",
                    DataViewRowState.CurrentRows);

                if (PayrollTypeDataView.Count > 0)
                {
                    DataRow drDataRow = DataSet.Tables["PayrollType"].NewRow();

                    drDataRow["PAYROLL_TYPE_DESC"] = "Salaries";

                    DataSet.Tables["PayrollType"].Rows.Add(drDataRow);
                }

                PayrollTypeDataView = null;
                PayrollTypeDataView = new DataView(DataSet.Tables["AuthorsiseLevel"],
                    "PAY_CATEGORY_TYPE = 'T'",
                    "",
                    DataViewRowState.CurrentRows);

                if (PayrollTypeDataView.Count > 0)
                {
                    DataRow drDataRow = DataSet.Tables["PayrollType"].NewRow();

                    drDataRow["PAYROLL_TYPE_DESC"] = "Time Attendance";

                    DataSet.Tables["PayrollType"].Rows.Add(drDataRow);
                }

                DataSet.AcceptChanges();
            }

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" EN.COMPANY_NO");
            strQry.AppendLine(",EN.EARNING_NO");
            strQry.AppendLine(",EN.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EN.EARNING_DESC");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING EN ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT LC");
            strQry.AppendLine(" ON EN.COMPANY_NO = LC.COMPANY_NO ");
            strQry.AppendLine(" AND EN.EARNING_NO = LC.EARNING_NO ");
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = LC.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" WHERE EN.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EN.EARNING_NO >= 200");
            strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" EN.EARNING_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "LeaveType", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");

            //Used For Leave Totals
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");

            strQry.AppendLine(",LS.LEAVE_PAID_ACCUMULATOR_IND");
         
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
            //Default Pay Category Parameters Apply to Leave
            strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT LS");
            strQry.AppendLine(" ON E.COMPANY_NO = LS.COMPANY_NO");
            strQry.AppendLine(" AND E.LEAVE_SHIFT_NO = LS.LEAVE_SHIFT_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = LS.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND LS.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT LC");
            strQry.AppendLine(" ON E.COMPANY_NO = LC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = LC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = LC.PAY_CATEGORY_TYPE");
            //Next Run
            strQry.AppendLine(" AND LC.PROCESS_NO = 0");
         
            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            //Removed 2012-04-06 - Allow For Employees Clocked on Client That have Not been Activated to be seen
            //strQry.AppendLine(" AND EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" E.EMPLOYEE_CODE");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" PAY_CATEGORY_NO");
            strQry.AppendLine(",DAY_NO");
            strQry.AppendLine(",TIME_DECIMAL");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_TIME_DECIMAL");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" PAY_CATEGORY_NO");
            strQry.AppendLine(",DAY_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategoryTimeDecimal", parint64CompanyNo);
            
            DateTime dtDateNowAYearAgo = DateTime.Now.AddYears(-1);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" PUBLIC_HOLIDAY_DATE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY PH ");

            strQry.AppendLine(" WHERE  PUBLIC_HOLIDAY_DATE > '" + dtDateNowAYearAgo.ToString("yyyy-MM-dd") + "'");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PublicHoliday", parint64CompanyNo);
            
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);

            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
   
        private byte[] Get_User_Level_Records(Int64 parint64CompanyNo, Int64 parint64CurrentUserNo, string parstrCurrentUserAccessInd, string parstrFromProgram)
        {
            DataSet DataSet = new System.Data.DataSet();
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EPCLPAC.COMPANY_NO ");
            strQry.AppendLine(",EPCLPAC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",EPCLPAC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",EPCLPAC.LEVEL_NO");

            strQry.AppendLine(",LEVEL_DESC = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN EPCLPAC.LEVEL_NO =  1 THEN 'First Level'");

            strQry.AppendLine(" WHEN EPCLPAC.LEVEL_NO =  2 THEN 'Second Level'");

            strQry.AppendLine(" WHEN EPCLPAC.LEVEL_NO =  3 THEN 'Third Level'");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",COUNT(DISTINCT EPCLPAC.LEAVE_REC_NO) AS AUTHORISE_TOTAL");

            strQry.AppendLine(",COUNT(DISTINCT EPCLPAC1.LEAVE_REC_NO) AS AUTHORISE_CURRENT");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT EPCLPAC");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT EPCLPAC1");

            strQry.AppendLine(" ON EPCLPAC.COMPANY_NO = EPCLPAC1.COMPANY_NO  ");
            strQry.AppendLine(" AND EPCLPAC.EMPLOYEE_NO = EPCLPAC1.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPCLPAC.PAY_CATEGORY_NO = EPCLPAC1.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPCLPAC.PAY_CATEGORY_TYPE = EPCLPAC1.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPCLPAC.LEVEL_NO = EPCLPAC1.LEVEL_NO");
            //Actual Leave Record
            strQry.AppendLine(" AND EPCLPAC.LEAVE_REC_NO = EPCLPAC1.LEAVE_REC_NO");

            strQry.AppendLine(" AND EPCLPAC1.AUTHORISED_IND = 'Y'");

            strQry.AppendLine(" WHERE EPCLPAC.COMPANY_NO = " + parint64CompanyNo);

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND EPCLPAC.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND EPCLPAC.PAY_CATEGORY_TYPE IN ('W','S')");
            }

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" EPCLPAC.COMPANY_NO ");
            strQry.AppendLine(",EPCLPAC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",EPCLPAC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",EPCLPAC.LEVEL_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "AuthorsiseLevel", parint64CompanyNo);

            //First Level
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EPC.COMPANY_NO ");
            strQry.AppendLine(",EPC.EMPLOYEE_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",EPCWAC1.EARNING_NO");
            strQry.AppendLine(",EPCWAC1.LEVEL_NO");
            strQry.AppendLine(",EPCWAC1.LEAVE_REC_NO");
            strQry.AppendLine(",'N' AS AUTHORISED_IND");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");

            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT EPCWAC1");
            strQry.AppendLine(" ON EPC.COMPANY_NO = EPCWAC1.COMPANY_NO");
            strQry.AppendLine(" AND EPC.EMPLOYEE_NO = EPCWAC1.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = EPCWAC1.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = EPCWAC1.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPCWAC1.LEVEL_NO = 1 ");
            strQry.AppendLine(" AND EPCWAC1.USER_NO = " + parint64CurrentUserNo.ToString());

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT EPCWAC1N");
            strQry.AppendLine(" ON EPCWAC1.COMPANY_NO = EPCWAC1N.COMPANY_NO");
            strQry.AppendLine(" AND EPCWAC1.EMPLOYEE_NO = EPCWAC1N.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPCWAC1.PAY_CATEGORY_NO = EPCWAC1N.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPCWAC1.PAY_CATEGORY_TYPE = EPCWAC1N.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" AND EPCWAC1.EARNING_NO = EPCWAC1N.EARNING_NO ");
            strQry.AppendLine(" AND EPCWAC1.LEAVE_REC_NO = EPCWAC1N.LEAVE_REC_NO ");

            strQry.AppendLine(" AND EPCWAC1N.LEVEL_NO = 1 ");
         
            strQry.AppendLine(" AND EPCWAC1N.AUTHORISED_IND = 'Y'");

            strQry.AppendLine(" WHERE EPC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO > 0");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE IN ('W','S')");
            }

            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            //Employee Level1 has NOT been Authorised by Anybody
            strQry.AppendLine(" AND EPCWAC1N.COMPANY_NO IS NULL");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" EPC.COMPANY_NO ");
            strQry.AppendLine(",EPC.EMPLOYEE_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",EPCWAC1.EARNING_NO");
            strQry.AppendLine(",EPCWAC1.LEVEL_NO");
            strQry.AppendLine(",EPCWAC1.LEAVE_REC_NO");

            strQry.AppendLine(" UNION ");

            //Second Level
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EPC.COMPANY_NO ");
            strQry.AppendLine(",EPC.EMPLOYEE_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",EPCWAC2.EARNING_NO");
            strQry.AppendLine(",EPCWAC2.LEVEL_NO");
            strQry.AppendLine(",EPCWAC2.LEAVE_REC_NO");
            strQry.AppendLine(",'N' AS AUTHORISED_IND");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");

            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            //First Level has been Authorised
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT EPCWAC1");
            strQry.AppendLine(" ON EPC.COMPANY_NO = EPCWAC1.COMPANY_NO");
            strQry.AppendLine(" AND EPC.EMPLOYEE_NO = EPCWAC1.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = EPCWAC1.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = EPCWAC1.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPCWAC1.LEVEL_NO = 1 ");
            strQry.AppendLine(" AND EPCWAC1.AUTHORISED_IND = 'Y'");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT EPCWAC2");
            strQry.AppendLine(" ON EPC.COMPANY_NO = EPCWAC2.COMPANY_NO");
            strQry.AppendLine(" AND EPC.EMPLOYEE_NO = EPCWAC2.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = EPCWAC2.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = EPCWAC2.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" AND EPCWAC1.EARNING_NO = EPCWAC2.EARNING_NO ");
            strQry.AppendLine(" AND EPCWAC1.LEAVE_REC_NO = EPCWAC2.LEAVE_REC_NO ");

            strQry.AppendLine(" AND EPCWAC2.LEVEL_NO = 2 ");
            strQry.AppendLine(" AND EPCWAC2.USER_NO = " + parint64CurrentUserNo.ToString());

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT EPCWAC2N");
            strQry.AppendLine(" ON EPCWAC2.COMPANY_NO = EPCWAC2N.COMPANY_NO");
            strQry.AppendLine(" AND EPCWAC2.EMPLOYEE_NO = EPCWAC2N.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPCWAC2.PAY_CATEGORY_NO = EPCWAC2N.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPCWAC2.PAY_CATEGORY_TYPE = EPCWAC2N.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" AND EPCWAC2.EARNING_NO = EPCWAC2N.EARNING_NO ");
            strQry.AppendLine(" AND EPCWAC2.LEAVE_REC_NO = EPCWAC2N.LEAVE_REC_NO ");

            strQry.AppendLine(" AND EPCWAC2N.LEVEL_NO = 2 ");
            strQry.AppendLine(" AND EPCWAC2N.AUTHORISED_IND = 'Y'");

            strQry.AppendLine(" WHERE EPC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO > 0");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE IN ('W','S')");
            }

            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            //Employee Level2 has NOT been Authorised by Anybody
            strQry.AppendLine(" AND EPCWAC2N.COMPANY_NO IS NULL");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" EPC.COMPANY_NO ");
            strQry.AppendLine(",EPC.EMPLOYEE_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",EPCWAC2.EARNING_NO");
            strQry.AppendLine(",EPCWAC2.LEVEL_NO");
            strQry.AppendLine(",EPCWAC2.LEAVE_REC_NO");

            strQry.AppendLine(" UNION ");

            //Third Level
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EPC.COMPANY_NO ");
            strQry.AppendLine(",EPC.EMPLOYEE_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",EPCWAC3.EARNING_NO");
            strQry.AppendLine(",EPCWAC3.LEVEL_NO");
            strQry.AppendLine(",EPCWAC3.LEAVE_REC_NO");
           
            strQry.AppendLine(",'N' AS AUTHORISED_IND");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");

            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            //First Level has been Authorised
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT EPCWAC1");
            strQry.AppendLine(" ON EPC.COMPANY_NO = EPCWAC1.COMPANY_NO");
            strQry.AppendLine(" AND EPC.EMPLOYEE_NO = EPCWAC1.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = EPCWAC1.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = EPCWAC1.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPCWAC1.LEVEL_NO = 1 ");
            strQry.AppendLine(" AND EPCWAC1.AUTHORISED_IND = 'Y'");

            //Second Level has been Authorised
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT EPCWAC2");
            strQry.AppendLine(" ON EPC.COMPANY_NO = EPCWAC2.COMPANY_NO");
            strQry.AppendLine(" AND EPC.EMPLOYEE_NO = EPCWAC2.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = EPCWAC2.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = EPCWAC2.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" AND EPCWAC1.EARNING_NO = EPCWAC2.EARNING_NO ");
            strQry.AppendLine(" AND EPCWAC1.LEAVE_REC_NO = EPCWAC2.LEAVE_REC_NO ");

            strQry.AppendLine(" AND EPCWAC2.LEVEL_NO = 2 ");
            strQry.AppendLine(" AND EPCWAC2.AUTHORISED_IND = 'Y'");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT EPCWAC3");
            strQry.AppendLine(" ON EPC.COMPANY_NO = EPCWAC3.COMPANY_NO");
            strQry.AppendLine(" AND EPC.EMPLOYEE_NO = EPCWAC3.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = EPCWAC3.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = EPCWAC3.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" AND EPCWAC1.EARNING_NO = EPCWAC3.EARNING_NO ");
            strQry.AppendLine(" AND EPCWAC1.LEAVE_REC_NO = EPCWAC3.LEAVE_REC_NO ");

            strQry.AppendLine(" AND EPCWAC3.LEVEL_NO = 3 ");
            strQry.AppendLine(" AND EPCWAC3.USER_NO = " + parint64CurrentUserNo.ToString());

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT EPCWAC3N");
            strQry.AppendLine(" ON EPCWAC3.COMPANY_NO = EPCWAC3N.COMPANY_NO");
            strQry.AppendLine(" AND EPCWAC3.EMPLOYEE_NO = EPCWAC3N.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPCWAC3.PAY_CATEGORY_NO = EPCWAC3N.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPCWAC3.PAY_CATEGORY_TYPE = EPCWAC3N.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" AND EPCWAC1.EARNING_NO = EPCWAC3N.EARNING_NO ");
            strQry.AppendLine(" AND EPCWAC1.LEAVE_REC_NO = EPCWAC3N.LEAVE_REC_NO ");

            strQry.AppendLine(" AND EPCWAC3N.LEVEL_NO = 3 ");
            strQry.AppendLine(" AND EPCWAC3N.AUTHORISED_IND = 'Y'");

            strQry.AppendLine(" WHERE EPC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO > 0");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE IN ('W','S')");
            }

            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            //Employee Level3 has NOT been Authorised by Anybody
            strQry.AppendLine(" AND EPCWAC3N.COMPANY_NO IS NULL");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" EPC.COMPANY_NO ");
            strQry.AppendLine(",EPC.EMPLOYEE_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",EPCWAC3.EARNING_NO");
            strQry.AppendLine(",EPCWAC3.LEVEL_NO");
            strQry.AppendLine(",EPCWAC3.LEAVE_REC_NO");

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EPCPAC.COMPANY_NO ");
            strQry.AppendLine(",EPCPAC.EMPLOYEE_NO");
            strQry.AppendLine(",EPCPAC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",EPCPAC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",EPCPAC.EARNING_NO");
            strQry.AppendLine(",EPCPAC.LEVEL_NO");
            strQry.AppendLine(",EPCPAC.LEAVE_REC_NO");
            strQry.AppendLine(",EPCPAC.AUTHORISED_IND");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT EPCPAC");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_AUTHORISE PCA");
            strQry.AppendLine(" ON EPCPAC.COMPANY_NO = PCA.COMPANY_NO");
            strQry.AppendLine(" AND EPCPAC.PAY_CATEGORY_NO = PCA.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPCPAC.PAY_CATEGORY_TYPE = PCA.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND PCA.AUTHORISE_TYPE_IND = 'L' ");
            strQry.AppendLine(" AND EPCPAC.LEVEL_NO = PCA.LEVEL_NO ");
            strQry.AppendLine(" AND PCA.USER_NO = " + parint64CurrentUserNo.ToString());

            strQry.AppendLine(" WHERE EPCPAC.COMPANY_NO = " + parint64CompanyNo);
          
            strQry.AppendLine(" AND EPCPAC.AUTHORISED_IND = 'Y'");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeePayCategoryLevel", parint64CompanyNo);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Update_Record(Int64 parint64CompanyNo,string parstrPayrollType,int parintAuthoriseLevel,Int64 parint64CurrentUserNo,string parstrCurrentUserAccessInd, string parstrFromProgram, byte[] parbyteDataSet)
        {
            DataSet DataSet = new System.Data.DataSet();
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            StringBuilder strQry = new StringBuilder();

            for (int intRow = 0; intRow < parDataSet.Tables["EmployeePayCategoryLevel"].Rows.Count; intRow++)
            {
                strQry.Clear();
                strQry.AppendLine(" UPDATE EPCLAC ");

                strQry.AppendLine(" SET ");
                strQry.AppendLine(" EPCLAC.AUTHORISED_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EmployeePayCategoryLevel"].Rows[intRow]["AUTHORISED_IND"].ToString()));

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT EPCLAC ");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT EPCLAC_T ");
                strQry.AppendLine(" ON EPCLAC.COMPANY_NO = EPCLAC_T.COMPANY_NO ");
                strQry.AppendLine(" AND EPCLAC.EMPLOYEE_NO = EPCLAC_T.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPCLAC.PAY_CATEGORY_NO = EPCLAC_T.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND EPCLAC.PAY_CATEGORY_TYPE = EPCLAC_T.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EPCLAC.EARNING_NO = EPCLAC_T.EARNING_NO ");
                strQry.AppendLine(" AND EPCLAC.LEVEL_NO = EPCLAC_T.LEVEL_NO ");
                strQry.AppendLine(" AND EPCLAC.LEAVE_REC_NO = EPCLAC_T.LEAVE_REC_NO ");
                strQry.AppendLine(" AND EPCLAC.USER_NO = EPCLAC_T.USER_NO ");
                strQry.AppendLine(" AND EPCLAC_T.AUTHORISED_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EmployeePayCategoryLevel"].Rows[intRow]["AUTHORISED_IND"].ToString()));

                strQry.AppendLine(" WHERE EPCLAC.COMPANY_NO = " + parDataSet.Tables["EmployeePayCategoryLevel"].Rows[intRow]["COMPANY_NO"].ToString());
                strQry.AppendLine(" AND EPCLAC.EMPLOYEE_NO = " + parDataSet.Tables["EmployeePayCategoryLevel"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                strQry.AppendLine(" AND EPCLAC.PAY_CATEGORY_NO = " + parDataSet.Tables["EmployeePayCategoryLevel"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(" AND EPCLAC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EmployeePayCategoryLevel"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                strQry.AppendLine(" AND EPCLAC.EARNING_NO = " + parDataSet.Tables["EmployeePayCategoryLevel"].Rows[intRow]["EARNING_NO"].ToString());
                strQry.AppendLine(" AND EPCLAC.LEAVE_REC_NO = " + parDataSet.Tables["EmployeePayCategoryLevel"].Rows[intRow]["LEAVE_REC_NO"].ToString());

                if (parDataSet.Tables["EmployeePayCategoryLevel"].Rows[intRow]["AUTHORISED_IND"].ToString() == "Y")
                {
                    strQry.AppendLine(" AND EPCLAC.LEVEL_NO = " + parDataSet.Tables["EmployeePayCategoryLevel"].Rows[intRow]["LEVEL_NO"].ToString());
                    strQry.AppendLine(" AND EPCLAC.USER_NO = " + parint64CurrentUserNo.ToString());
                }
                else
                {
                    //Set All Higher Levels Also to NOT Authorised
                    strQry.AppendLine(" AND EPCLAC.LEVEL_NO >= " + parDataSet.Tables["EmployeePayCategoryLevel"].Rows[intRow]["LEVEL_NO"].ToString());
                }

                //Row is Different
                strQry.AppendLine(" AND EPCLAC_T.COMPANY_NO IS NULL ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            }

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            byte[] bytCompress = Get_User_Level_Records(parint64CompanyNo, parint64CurrentUserNo, parstrCurrentUserAccessInd, parstrFromProgram);

            return bytCompress;
        }
    }
}
