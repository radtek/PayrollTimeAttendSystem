using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busTimeSheetAuthorise
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busTimeSheetAuthorise()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo,Int64 parint64CurrentUserNo, string parstrCurrentUserAccessInd, string parstrFromProgram)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            //Empty Dates DataTable
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EPC.COMPANY_NO ");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",D.DAY_DATE");
            strQry.AppendLine(",D.DAY_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");
            strQry.AppendLine(" ON D.DAY_DATE > '2030-01-01'");

            strQry.AppendLine(" WHERE EPC.COMPANY_NO = -1 ");
         
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Dates", parint64CompanyNo);
           
            byte[] byteArrayDataset = Get_User_Level_Records(parint64CompanyNo, parint64CurrentUserNo, parstrCurrentUserAccessInd, parstrFromProgram);

            DataSet TempDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(byteArrayDataset);

            DataSet.Merge(TempDataSet);
         
            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" PC.COMPANY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PC.EXCEPTION_SUN_ABOVE_MINUTES");
            strQry.AppendLine(",PC.EXCEPTION_SUN_BELOW_MINUTES");
            strQry.AppendLine(",PC.EXCEPTION_MON_ABOVE_MINUTES");
            strQry.AppendLine(",PC.EXCEPTION_MON_BELOW_MINUTES");
            strQry.AppendLine(",PC.EXCEPTION_TUE_ABOVE_MINUTES");
            strQry.AppendLine(",PC.EXCEPTION_TUE_BELOW_MINUTES");
            strQry.AppendLine(",PC.EXCEPTION_WED_ABOVE_MINUTES");
            strQry.AppendLine(",PC.EXCEPTION_WED_BELOW_MINUTES");
            strQry.AppendLine(",PC.EXCEPTION_THU_ABOVE_MINUTES");
            strQry.AppendLine(",PC.EXCEPTION_THU_BELOW_MINUTES");
            strQry.AppendLine(",PC.EXCEPTION_FRI_ABOVE_MINUTES");
            strQry.AppendLine(",PC.EXCEPTION_FRI_BELOW_MINUTES");
            strQry.AppendLine(",PC.EXCEPTION_SAT_ABOVE_MINUTES");
            strQry.AppendLine(",PC.EXCEPTION_SAT_BELOW_MINUTES");
            strQry.AppendLine(",PC.DAILY_ROUNDING_IND");
            strQry.AppendLine(",PC.DAILY_ROUNDING_MINUTES");
            strQry.AppendLine(",PCPC.PAY_PERIOD_DATE");

            strQry.AppendLine(",RUN_IND = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN NOT PCPC.PAY_PERIOD_DATE IS NULL AND PC.PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" THEN C.WAGE_RUN_IND");

            strQry.AppendLine(" WHEN NOT PCPC.PAY_PERIOD_DATE IS NULL AND PC.PAY_CATEGORY_TYPE = 'S'");
            strQry.AppendLine(" THEN C.SALARY_RUN_IND");

            strQry.AppendLine(" WHEN NOT PCPC.PAY_PERIOD_DATE IS NULL AND PC.PAY_CATEGORY_TYPE = 'T'");
            strQry.AppendLine(" THEN C.TIME_ATTENDANCE_RUN_IND");

            strQry.AppendLine(" ELSE '' ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",PC.LAST_UPLOAD_DATETIME");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");
            strQry.AppendLine(" ON PC.COMPANY_NO = PCPC.COMPANY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = PCPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = PCPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P'");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C");
            strQry.AppendLine(" ON PC.COMPANY_NO = C.COMPANY_NO ");
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL ");

            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(SELECT DISTINCT");
                strQry.AppendLine(" PAY_CATEGORY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
        
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP ");
                strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo + ") AS USER_TABLE");

                strQry.AppendLine(" ON PCPC.PAY_CATEGORY_NO = USER_TABLE.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = USER_TABLE.PAY_CATEGORY_TYPE");
            }
   
            strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
           
            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PC.COMPANY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PC.EXCEPTION_SUN_ABOVE_MINUTES");
            strQry.AppendLine(",PC.EXCEPTION_SUN_BELOW_MINUTES");
            strQry.AppendLine(",PC.EXCEPTION_MON_ABOVE_MINUTES");
            strQry.AppendLine(",PC.EXCEPTION_MON_BELOW_MINUTES");
            strQry.AppendLine(",PC.EXCEPTION_TUE_ABOVE_MINUTES");
            strQry.AppendLine(",PC.EXCEPTION_TUE_BELOW_MINUTES");
            strQry.AppendLine(",PC.EXCEPTION_WED_ABOVE_MINUTES");
            strQry.AppendLine(",PC.EXCEPTION_WED_BELOW_MINUTES");
            strQry.AppendLine(",PC.EXCEPTION_THU_ABOVE_MINUTES");
            strQry.AppendLine(",PC.EXCEPTION_THU_BELOW_MINUTES");
            strQry.AppendLine(",PC.EXCEPTION_FRI_ABOVE_MINUTES");
            strQry.AppendLine(",PC.EXCEPTION_FRI_BELOW_MINUTES");
            strQry.AppendLine(",PC.EXCEPTION_SAT_ABOVE_MINUTES");
            strQry.AppendLine(",PC.EXCEPTION_SAT_BELOW_MINUTES");
            strQry.AppendLine(",PC.DAILY_ROUNDING_IND");
            strQry.AppendLine(",PC.DAILY_ROUNDING_MINUTES");
            strQry.AppendLine(",PCPC.PAY_PERIOD_DATE");

            strQry.AppendLine(",C.WAGE_RUN_IND");
            strQry.AppendLine(",C.SALARY_RUN_IND");
            strQry.AppendLine(",C.TIME_ATTENDANCE_RUN_IND");

            strQry.AppendLine(",PC.LAST_UPLOAD_DATETIME");
           
            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" PC.PAY_CATEGORY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategory", parint64CompanyNo);

            DataSet.Tables.Add("PayrollType");
            DataTable PayrollTypeDataTable = new DataTable("PayrollType");
            DataSet.Tables["PayrollType"].Columns.Add("PAYROLL_TYPE_DESC", typeof(String));

            if (DataSet.Tables["PayCategory"].Rows.Count > 0)
            {
                DataView PayrollTypeDataView = new DataView(DataSet.Tables["PayCategory"],
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
                PayrollTypeDataView = new DataView(DataSet.Tables["PayCategory"],
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
                PayrollTypeDataView = new DataView(DataSet.Tables["PayCategory"],
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

                if (DataSet.Tables["PayCategory"].Rows.Count > 0)
                {
                    byte[] bytTempCompress = Get_PayCategory_Records(Convert.ToInt64(DataSet.Tables["PayCategory"].Rows[0]["COMPANY_NO"]), DataSet.Tables["PayCategory"].Rows[0]["PAY_CATEGORY_TYPE"].ToString(), parint64CurrentUserNo, parstrCurrentUserAccessInd);

                    TempDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(bytTempCompress);
                    DataSet.Merge(TempDataSet);
                }

                //Link To User Authorised To Do Authorisations
                strQry.Clear();
                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" E.COMPANY_NO ");
                strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",E.EMPLOYEE_CODE");
                strQry.AppendLine(",E.EMPLOYEE_SURNAME");
                strQry.AppendLine(",E.EMPLOYEE_NAME");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",E.EMPLOYEE_LAST_RUNDATE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                strQry.AppendLine(" INNER JOIN  InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT EPCPAC  ");
                strQry.AppendLine(" ON E.COMPANY_NO = EPCPAC.COMPANY_NO");
                strQry.AppendLine(" AND EPCPAC.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCPAC.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCPAC.PAY_CATEGORY_TYPE ");
              
                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                strQry.AppendLine(" GROUP BY");
                strQry.AppendLine(" E.COMPANY_NO ");
                strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",E.EMPLOYEE_CODE");
                strQry.AppendLine(",E.EMPLOYEE_SURNAME");
                strQry.AppendLine(",E.EMPLOYEE_NAME");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",E.EMPLOYEE_LAST_RUNDATE");


                strQry.AppendLine(" ORDER BY");
                strQry.AppendLine(" E.COMPANY_NO ");
                strQry.AppendLine(",E.EMPLOYEE_CODE");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parint64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PC.PAY_CATEGORY_NO");
                strQry.AppendLine(",PH.PUBLIC_HOLIDAY_DATE");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY PH");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");

                strQry.AppendLine(" ON PC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");
                strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND PC.PAY_PUBLIC_HOLIDAY_IND = 'Y'");

                if (parstrFromProgram == "X")
                {
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = 'T'");
                }
                else
                {
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE IN ('W','S')");
                }

                strQry.AppendLine(" WHERE PH.PUBLIC_HOLIDAY_DATE > '" + DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd") + "'");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PublicHoliday", parint64CompanyNo);
            }

            DataSet.AcceptChanges();

            TempDataSet.Dispose();
            TempDataSet = null;

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
            strQry.AppendLine(" EPCPAC.COMPANY_NO ");
            strQry.AppendLine(",EPCPAC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",EPCPAC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",EPCPAC.LEVEL_NO");

            strQry.AppendLine(",LEVEL_DESC = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN EPCPAC.LEVEL_NO =  1 THEN 'First Level'");

            strQry.AppendLine(" WHEN EPCPAC.LEVEL_NO =  2 THEN 'Second Level'");

            strQry.AppendLine(" WHEN EPCPAC.LEVEL_NO =  3 THEN 'Third Level'");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",COUNT(DISTINCT EPCPAC.EMPLOYEE_NO) AS AUTHORISE_TOTAL");

            strQry.AppendLine(",COUNT(DISTINCT EPCPAC1.EMPLOYEE_NO) AS AUTHORISE_CURRENT");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT EPCPAC");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT EPCPAC1");

            strQry.AppendLine(" ON EPCPAC.COMPANY_NO = EPCPAC1.COMPANY_NO  ");
            strQry.AppendLine(" AND EPCPAC.EMPLOYEE_NO = EPCPAC1.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPCPAC.PAY_CATEGORY_NO = EPCPAC1.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPCPAC.PAY_CATEGORY_TYPE = EPCPAC1.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPCPAC.LEVEL_NO = EPCPAC1.LEVEL_NO");
            strQry.AppendLine(" AND EPCPAC1.AUTHORISED_IND = 'Y'");

            strQry.AppendLine(" WHERE EPCPAC.COMPANY_NO = " + parint64CompanyNo);
            
            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND EPCPAC.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND EPCPAC.PAY_CATEGORY_TYPE IN ('W','S')");
            }

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" EPCPAC.COMPANY_NO ");
            strQry.AppendLine(",EPCPAC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",EPCPAC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",EPCPAC.LEVEL_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "AuthorsiseLevel", parint64CompanyNo);

            strQry.Clear();
          
            //First Level
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EPC.COMPANY_NO ");
            strQry.AppendLine(",EPC.EMPLOYEE_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",EPCWAC1.LEVEL_NO");
            strQry.AppendLine(",'N' AS AUTHORISED_IND");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");

            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT EPCWAC1");
            strQry.AppendLine(" ON EPC.COMPANY_NO = EPCWAC1.COMPANY_NO");
            strQry.AppendLine(" AND EPC.EMPLOYEE_NO = EPCWAC1.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = EPCWAC1.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = EPCWAC1.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPCWAC1.LEVEL_NO = 1 ");
            strQry.AppendLine(" AND EPCWAC1.USER_NO = " + parint64CurrentUserNo);

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT EPCWAC1N");
            strQry.AppendLine(" ON EPCWAC1.COMPANY_NO = EPCWAC1N.COMPANY_NO");
            strQry.AppendLine(" AND EPCWAC1.EMPLOYEE_NO = EPCWAC1N.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPCWAC1.PAY_CATEGORY_NO = EPCWAC1N.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPCWAC1.PAY_CATEGORY_TYPE = EPCWAC1N.PAY_CATEGORY_TYPE ");
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
            strQry.AppendLine(",EPCWAC1.LEVEL_NO");

            strQry.AppendLine(" UNION ");

            //Second Level
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EPC.COMPANY_NO ");
            strQry.AppendLine(",EPC.EMPLOYEE_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",EPCWAC2.LEVEL_NO");
            strQry.AppendLine(",'N' AS AUTHORISED_IND");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");

            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            //First Level has been Authorised
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT EPCWAC1");
            strQry.AppendLine(" ON EPC.COMPANY_NO = EPCWAC1.COMPANY_NO");
            strQry.AppendLine(" AND EPC.EMPLOYEE_NO = EPCWAC1.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = EPCWAC1.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = EPCWAC1.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPCWAC1.LEVEL_NO = 1 ");
            strQry.AppendLine(" AND EPCWAC1.AUTHORISED_IND = 'Y'");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT EPCWAC2");
            strQry.AppendLine(" ON EPC.COMPANY_NO = EPCWAC2.COMPANY_NO");
            strQry.AppendLine(" AND EPC.EMPLOYEE_NO = EPCWAC2.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = EPCWAC2.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = EPCWAC2.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPCWAC2.LEVEL_NO = 2 ");
            strQry.AppendLine(" AND EPCWAC2.USER_NO = " + parint64CurrentUserNo);

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT EPCWAC2N");
            strQry.AppendLine(" ON EPCWAC2.COMPANY_NO = EPCWAC2N.COMPANY_NO");
            strQry.AppendLine(" AND EPCWAC2.EMPLOYEE_NO = EPCWAC2N.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPCWAC2.PAY_CATEGORY_NO = EPCWAC2N.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPCWAC2.PAY_CATEGORY_TYPE = EPCWAC2N.PAY_CATEGORY_TYPE ");
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
            strQry.AppendLine(",EPCWAC2.LEVEL_NO");

            strQry.AppendLine(" UNION ");

            //Third Level
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EPC.COMPANY_NO ");
            strQry.AppendLine(",EPC.EMPLOYEE_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",EPCWAC3.LEVEL_NO");
            strQry.AppendLine(",'N' AS AUTHORISED_IND");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");

            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            //First Level has been Authorised
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT EPCWAC1");
            strQry.AppendLine(" ON EPC.COMPANY_NO = EPCWAC1.COMPANY_NO");
            strQry.AppendLine(" AND EPC.EMPLOYEE_NO = EPCWAC1.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = EPCWAC1.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = EPCWAC1.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPCWAC1.LEVEL_NO = 1 ");
            strQry.AppendLine(" AND EPCWAC1.AUTHORISED_IND = 'Y'");

            //Second Level has been Authorised
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT EPCWAC2");
            strQry.AppendLine(" ON EPC.COMPANY_NO = EPCWAC2.COMPANY_NO");
            strQry.AppendLine(" AND EPC.EMPLOYEE_NO = EPCWAC2.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = EPCWAC2.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = EPCWAC2.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPCWAC2.LEVEL_NO = 2 ");
            strQry.AppendLine(" AND EPCWAC2.AUTHORISED_IND = 'Y'");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT EPCWAC3");
            strQry.AppendLine(" ON EPC.COMPANY_NO = EPCWAC3.COMPANY_NO");
            strQry.AppendLine(" AND EPC.EMPLOYEE_NO = EPCWAC3.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = EPCWAC3.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = EPCWAC3.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPCWAC3.LEVEL_NO = 3 ");
            strQry.AppendLine(" AND EPCWAC3.USER_NO = " + parint64CurrentUserNo);

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT EPCWAC3N");
            strQry.AppendLine(" ON EPCWAC3.COMPANY_NO = EPCWAC3N.COMPANY_NO");
            strQry.AppendLine(" AND EPCWAC3.EMPLOYEE_NO = EPCWAC3N.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPCWAC3.PAY_CATEGORY_NO = EPCWAC3N.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPCWAC3.PAY_CATEGORY_TYPE = EPCWAC3N.PAY_CATEGORY_TYPE ");
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
            strQry.AppendLine(",EPCWAC3.LEVEL_NO");
        
            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EPCWAC.COMPANY_NO ");
            strQry.AppendLine(",EPCWAC.EMPLOYEE_NO");
            strQry.AppendLine(",EPCWAC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",EPCWAC.PAY_CATEGORY_TYPE ");
            
            strQry.AppendLine(",EPCWAC.LEVEL_NO");
            strQry.AppendLine(",EPCWAC.AUTHORISED_IND");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT EPCWAC");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_AUTHORISE PCA");
            strQry.AppendLine(" ON EPCWAC.COMPANY_NO = PCA.COMPANY_NO");
            strQry.AppendLine(" AND EPCWAC.PAY_CATEGORY_NO = PCA.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPCWAC.PAY_CATEGORY_TYPE = PCA.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND PCA.AUTHORISE_TYPE_IND = 'T' ");
            strQry.AppendLine(" AND EPCWAC.LEVEL_NO = PCA.LEVEL_NO ");
            strQry.AppendLine(" AND PCA.USER_NO = " + parint64CurrentUserNo);

            strQry.AppendLine(" WHERE EPCWAC.COMPANY_NO = " + parint64CompanyNo);

            strQry.AppendLine(" AND EPCWAC.AUTHORISED_IND = 'Y'");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeePayCategoryLevel", parint64CompanyNo);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_PayCategory_Records(Int64 parint64CompanyNo, string parstrPayCategoryType, Int64 parintCurrentUserNo, string parstrCurrentUserAccessInd)
        {
            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();

            string strCurrenDate = DateTime.Now.ToString("yyyy-MM-dd");

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" TEMP_TABLE.COMPANY_NO ");
            strQry.AppendLine(",TEMP_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TEMP_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",D.DAY_DATE");
            strQry.AppendLine(",D.DAY_NO");

            strQry.AppendLine(" FROM ");

            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" E.COMPANY_NO ");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");

            //Errol 2013-06-15
            strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

            strQry.AppendLine(" MIN(CASE ");

            strQry.AppendLine(" WHEN EIH.PAY_PERIOD_DATE = E.EMPLOYEE_LAST_RUNDATE ");

            strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

            strQry.AppendLine(" WHEN EIH.PAY_PERIOD_DATE <> E.EMPLOYEE_LAST_RUNDATE ");

            strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE ");

            strQry.AppendLine(" ELSE DATEADD(DD,-40,'" + strCurrenDate + "')");

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
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            //Errol 2013-06-15
            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY EIH ");
            strQry.AppendLine(" ON EPC.COMPANY_NO = EIH.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.EMPLOYEE_NO = EIH.EMPLOYEE_NO ");
            //Take-On Record
            strQry.AppendLine(" AND EIH.RUN_TYPE = 'T' ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = EIH.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            //Errol 2013-06-15
            //strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" E.COMPANY_NO ");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");

            strQry.AppendLine(",EPC.PAY_CATEGORY_NO) AS TEMP_TABLE");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");

            strQry.AppendLine(" ON D.DAY_DATE > TEMP_TABLE.EMPLOYEE_LAST_RUNDATE");
            strQry.AppendLine(" AND D.DAY_DATE <= DATEADD(DD,15,'" + strCurrenDate + "')");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 3");
            strQry.AppendLine(",4 DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Dates", parint64CompanyNo);

            //This Part is Where NO Timesheets Existe for Break Records
            //This Part is Where NO Timesheets Existe for Break Records
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",BREAK_SUMMARY_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",E.EMPLOYEE_NO ");
            strQry.AppendLine(",BREAK_SUMMARY_TABLE.DAY_DATE");
            strQry.AppendLine(",D.DAY_NO");
            strQry.AppendLine(",0 AS DAY_PAID_MINUTES");
            strQry.AppendLine(",INDICATOR = 'X' ");

            strQry.AppendLine(",BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES");
            strQry.AppendLine(",BREAK_INDICATOR = 'Y'");

            strQry.AppendLine(",PAID_HOLIDAY_INDICATOR = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN NOT PUBLIC_HOLIDAY_TABLE.PUBLIC_HOLIDAY_DATE IS NULL ");

            strQry.AppendLine(" THEN 'Y'");

            strQry.AppendLine(" ELSE ''");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

            strQry.AppendLine(" INNER JOIN  ");

            strQry.AppendLine("(");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" BREAK_TABLE.COMPANY_NO");
            strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",BREAK_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",BREAK_TABLE.DAY_DATE");

            strQry.AppendLine(",SUM(BREAK_TABLE.BREAK_ACCUM_MINUTES) AS BREAK_ACCUM_MINUTES ");

            strQry.AppendLine(" FROM ");

            //Removes Duplicates where BREAK_SEQ is Not Used in Join
            strQry.AppendLine("(");
            strQry.AppendLine(" SELECT DISTINCT");

            strQry.AppendLine(" EBC.COMPANY_NO");
            strQry.AppendLine(",'" + parstrPayCategoryType + "' AS PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EBC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBC.EMPLOYEE_NO ");
            strQry.AppendLine(",EBC.BREAK_DATE AS DAY_DATE");

            //2014-05-03
            strQry.AppendLine(",EBC.BREAK_SEQ");

            strQry.AppendLine(",BREAK_ACCUM_MINUTES = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN ((EBC.BREAK_TIME_IN_MINUTES IS NULL");
            strQry.AppendLine(" OR EBC.BREAK_TIME_OUT_MINUTES IS NULL)");
            //Same Row / Next Row
            strQry.AppendLine(" OR (EBC.BREAK_TIME_IN_MINUTES > EBC2.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine(" AND EBC.BREAK_SEQ <= EBC2.BREAK_SEQ)");
            //Different Rows
            strQry.AppendLine(" OR (EBC.BREAK_TIME_IN_MINUTES < EBC2.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine(" AND EBC.BREAK_SEQ > EBC2.BREAK_SEQ))");

            strQry.AppendLine(" THEN 0 ");

            strQry.AppendLine(" ELSE ");

            strQry.AppendLine(" EBC.BREAK_TIME_OUT_MINUTES - EBC.BREAK_TIME_IN_MINUTES ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT EBC");
            }
            else
            {
                if (parstrPayCategoryType == "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT EBC");
                }
                else
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT EBC");
                }
            }

            strQry.AppendLine(" ON E.COMPANY_NO = EBC.COMPANY_NO");
            strQry.AppendLine(" AND EBC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EBC.EMPLOYEE_NO ");

            //2013-06-15 >= Cater For Employee Take-On 
            strQry.AppendLine(" AND EBC.BREAK_DATE >= ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrenDate + "'))");

            //Set Extra Days to 15
            strQry.AppendLine(" AND EBC.BREAK_DATE <= DATEADD(DD,15,'" + strCurrenDate + "')");

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT EBC2");
            }
            else
            {
                if (parstrPayCategoryType == "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT EBC2");
                }
                else
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT EBC2");
                }
            }
            strQry.AppendLine(" ON EBC.COMPANY_NO = EBC2.COMPANY_NO");
            strQry.AppendLine(" AND EBC.EMPLOYEE_NO = EBC2.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = EBC2.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EBC.BREAK_DATE = EBC2.BREAK_DATE");

            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND EBC.COMPANY_NO = UEPCT.COMPANY_NO ");
                strQry.AppendLine(" AND EBC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            }

            //Errol 2012-09-20 Fix Change of PAY_CATEGORY (Orphan Records)
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

            strQry.AppendLine(" ON EBC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND EBC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            //Errol 2013-04-12
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");

            strQry.AppendLine(" ON EBC.COMPANY_NO = PC.COMPANY_NO ");

            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            //1-End

            strQry.AppendLine(" ) AS BREAK_TABLE ");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" BREAK_TABLE.COMPANY_NO");
            strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",BREAK_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",BREAK_TABLE.DAY_DATE");

            //2-End

            strQry.AppendLine(" ) AS BREAK_SUMMARY_TABLE");

            strQry.AppendLine(" ON E.COMPANY_NO = BREAK_SUMMARY_TABLE.COMPANY_NO");

            strQry.AppendLine(" AND E.EMPLOYEE_NO = BREAK_SUMMARY_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = BREAK_SUMMARY_TABLE.PAY_CATEGORY_TYPE ");

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
            }
            else
            {
                if (parstrPayCategoryType == "S")
                {
                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC");
                }
                else
                {
                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC");
                }
            }

            strQry.AppendLine(" ON BREAK_SUMMARY_TABLE.COMPANY_NO = ETC.COMPANY_NO");

            strQry.AppendLine(" AND BREAK_SUMMARY_TABLE.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND BREAK_SUMMARY_TABLE.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND BREAK_SUMMARY_TABLE.DAY_DATE = ETC.TIMESHEET_DATE ");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");
            strQry.AppendLine(" ON BREAK_SUMMARY_TABLE.DAY_DATE = D.DAY_DATE");

            strQry.AppendLine(" LEFT JOIN ");
            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PH.PUBLIC_HOLIDAY_DATE");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY PH");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");

            strQry.AppendLine(" ON PC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND PC.PAY_PUBLIC_HOLIDAY_IND = 'Y'");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            strQry.AppendLine(" WHERE PH.PUBLIC_HOLIDAY_DATE > '" + DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd") + "') AS PUBLIC_HOLIDAY_TABLE");

            strQry.AppendLine(" ON BREAK_SUMMARY_TABLE.PAY_CATEGORY_NO = PUBLIC_HOLIDAY_TABLE.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND D.DAY_DATE = PUBLIC_HOLIDAY_TABLE.PUBLIC_HOLIDAY_DATE ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            strQry.AppendLine(" AND ETC.TIMESHEET_DATE IS NULL ");

            //This Part is Where Timesheets Exist with or Without Break Records
            //This Part is Where Timesheets Exist with or Without Break Records
            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" TEMP2_TABLE.COMPANY_NO");
            strQry.AppendLine(",TEMP2_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TEMP2_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",TEMP2_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",TEMP2_TABLE.DAY_DATE");
            strQry.AppendLine(",D.DAY_NO");
            strQry.AppendLine(",TEMP2_TABLE.DAY_PAID_MINUTES");

            strQry.AppendLine(",INDICATOR = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN TEMP2_TABLE.INDICATOR = 'X'");
            strQry.AppendLine(" THEN TEMP2_TABLE.INDICATOR");

            //Errol Fixed 
            strQry.AppendLine(" WHEN (PC2.EXCEPTION_SUN_BELOW_MINUTES = 0 ");
            strQry.AppendLine(" OR PC2.EXCEPTION_MON_BELOW_MINUTES = 0 ");
            strQry.AppendLine(" OR PC2.EXCEPTION_TUE_BELOW_MINUTES = 0 ");
            strQry.AppendLine(" OR PC2.EXCEPTION_WED_BELOW_MINUTES = 0 ");
            strQry.AppendLine(" OR PC2.EXCEPTION_THU_BELOW_MINUTES = 0 ");
            strQry.AppendLine(" OR PC2.EXCEPTION_FRI_BELOW_MINUTES = 0 ");
            strQry.AppendLine(" OR PC2.EXCEPTION_SAT_BELOW_MINUTES = 0) ");

            strQry.AppendLine(" AND TEMP2_TABLE.DAY_PAID_MINUTES = 0 ");

            strQry.AppendLine(" THEN 'E'");

            strQry.AppendLine(" WHEN D.DAY_NO = 0 ");
            strQry.AppendLine(" AND (TEMP2_TABLE.DAY_PAID_MINUTES > PC2.EXCEPTION_SUN_ABOVE_MINUTES");
            strQry.AppendLine(" OR TEMP2_TABLE.DAY_PAID_MINUTES < PC2.EXCEPTION_SUN_BELOW_MINUTES)");
            strQry.AppendLine(" THEN 'E'");

            strQry.AppendLine(" WHEN D.DAY_NO = 1 ");
            strQry.AppendLine(" AND (TEMP2_TABLE.DAY_PAID_MINUTES > PC2.EXCEPTION_MON_ABOVE_MINUTES");
            strQry.AppendLine(" OR TEMP2_TABLE.DAY_PAID_MINUTES < PC2.EXCEPTION_MON_BELOW_MINUTES)");
            strQry.AppendLine(" THEN 'E'");

            strQry.AppendLine(" WHEN D.DAY_NO = 2 ");
            strQry.AppendLine(" AND (TEMP2_TABLE.DAY_PAID_MINUTES > PC2.EXCEPTION_TUE_ABOVE_MINUTES");
            strQry.AppendLine(" OR TEMP2_TABLE.DAY_PAID_MINUTES < PC2.EXCEPTION_TUE_BELOW_MINUTES)");
            strQry.AppendLine(" THEN 'E'");

            strQry.AppendLine(" WHEN D.DAY_NO = 3 ");
            strQry.AppendLine(" AND (TEMP2_TABLE.DAY_PAID_MINUTES > PC2.EXCEPTION_WED_ABOVE_MINUTES");
            strQry.AppendLine(" OR TEMP2_TABLE.DAY_PAID_MINUTES < PC2.EXCEPTION_WED_BELOW_MINUTES)");
            strQry.AppendLine(" THEN 'E'");

            strQry.AppendLine(" WHEN D.DAY_NO = 4 ");
            strQry.AppendLine(" AND (TEMP2_TABLE.DAY_PAID_MINUTES > PC2.EXCEPTION_THU_ABOVE_MINUTES");
            strQry.AppendLine(" OR TEMP2_TABLE.DAY_PAID_MINUTES < PC2.EXCEPTION_THU_BELOW_MINUTES)");
            strQry.AppendLine(" THEN 'E'");

            strQry.AppendLine(" WHEN D.DAY_NO = 5 ");
            strQry.AppendLine(" AND (TEMP2_TABLE.DAY_PAID_MINUTES > PC2.EXCEPTION_FRI_ABOVE_MINUTES");
            strQry.AppendLine(" OR TEMP2_TABLE.DAY_PAID_MINUTES < PC2.EXCEPTION_FRI_BELOW_MINUTES)");
            strQry.AppendLine(" THEN 'E'");

            strQry.AppendLine(" WHEN D.DAY_NO = 6 ");
            strQry.AppendLine(" AND (TEMP2_TABLE.DAY_PAID_MINUTES > PC2.EXCEPTION_SAT_ABOVE_MINUTES");
            strQry.AppendLine(" OR TEMP2_TABLE.DAY_PAID_MINUTES < PC2.EXCEPTION_SAT_BELOW_MINUTES)");
            strQry.AppendLine(" THEN 'E'");

            strQry.AppendLine(" ELSE TEMP2_TABLE.INDICATOR");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",TEMP2_TABLE.BREAK_ACCUM_MINUTES");
            strQry.AppendLine(",TEMP2_TABLE.BREAK_INDICATOR");

            strQry.AppendLine(",PAID_HOLIDAY_INDICATOR = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN NOT PUBLIC_HOLIDAY_TABLE.PUBLIC_HOLIDAY_DATE IS NULL ");

            strQry.AppendLine(" THEN 'Y'");

            strQry.AppendLine(" ELSE ''");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM ");

            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" TEMP1_TABLE.COMPANY_NO");
            strQry.AppendLine(",TEMP1_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TEMP1_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",TEMP1_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",TEMP1_TABLE.DAY_DATE");

            strQry.AppendLine(",DAY_PAID_MINUTES = ");

            strQry.AppendLine(" CASE ");

            //Error or NO Rounding
            strQry.AppendLine(" WHEN TEMP1_TABLE.INDICATOR = 'X' OR PC1.DAILY_ROUNDING_IND = 0");
            strQry.AppendLine(" THEN TEMP1_TABLE.DAY_PAID_MINUTES");

            //Up
            strQry.AppendLine(" WHEN PC1.DAILY_ROUNDING_IND = 1");
            strQry.AppendLine(" THEN CASE ");

            strQry.AppendLine(" WHEN TEMP1_TABLE.DAY_PAID_MINUTES % PC1.DAILY_ROUNDING_MINUTES = 0");
            strQry.AppendLine(" THEN TEMP1_TABLE.DAY_PAID_MINUTES");

            strQry.AppendLine(" ELSE TEMP1_TABLE.DAY_PAID_MINUTES + (PC1.DAILY_ROUNDING_MINUTES - (TEMP1_TABLE.DAY_PAID_MINUTES % PC1.DAILY_ROUNDING_MINUTES))");

            strQry.AppendLine(" END");

            //Down
            strQry.AppendLine(" WHEN PC1.DAILY_ROUNDING_IND = 2");
            strQry.AppendLine(" THEN CASE ");

            strQry.AppendLine(" WHEN TEMP1_TABLE.DAY_PAID_MINUTES % PC1.DAILY_ROUNDING_MINUTES = 0");
            strQry.AppendLine(" THEN TEMP1_TABLE.DAY_PAID_MINUTES");

            strQry.AppendLine(" ELSE TEMP1_TABLE.DAY_PAID_MINUTES - (TEMP1_TABLE.DAY_PAID_MINUTES % PC1.DAILY_ROUNDING_MINUTES)");

            strQry.AppendLine(" END");

            //Closet
            strQry.AppendLine(" WHEN PC1.DAILY_ROUNDING_IND = 3");
            strQry.AppendLine(" THEN CASE ");

            strQry.AppendLine(" WHEN TEMP1_TABLE.DAY_PAID_MINUTES % PC1.DAILY_ROUNDING_MINUTES = 0");
            strQry.AppendLine(" THEN TEMP1_TABLE.DAY_PAID_MINUTES");

            //Closest - UP
            strQry.AppendLine(" WHEN TEMP1_TABLE.DAY_PAID_MINUTES % PC1.DAILY_ROUNDING_MINUTES > CONVERT(DECIMAL,PC1.DAILY_ROUNDING_MINUTES) / 2");
            strQry.AppendLine(" THEN TEMP1_TABLE.DAY_PAID_MINUTES + (PC1.DAILY_ROUNDING_MINUTES - (TEMP1_TABLE.DAY_PAID_MINUTES % PC1.DAILY_ROUNDING_MINUTES))");

            //Closest - Down
            strQry.AppendLine(" ELSE TEMP1_TABLE.DAY_PAID_MINUTES - (TEMP1_TABLE.DAY_PAID_MINUTES % PC1.DAILY_ROUNDING_MINUTES)");

            strQry.AppendLine(" END");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",TEMP1_TABLE.INDICATOR");
            strQry.AppendLine(",TEMP1_TABLE.BREAK_ACCUM_MINUTES");
            strQry.AppendLine(",TEMP1_TABLE.BREAK_INDICATOR");

            strQry.AppendLine(" FROM ");

            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" TIMESHEET_TOTAL_TABLE.COMPANY_NO");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.DAY_DATE");
            //strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.TIMESHEET_ACCUM_MINUTES ");
            //strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.INDICATOR ");
            //strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.BREAK_MINUTES ");

            strQry.AppendLine(",ISNULL(BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES,0) AS BREAK_ACCUM_MINUTES ");

            strQry.AppendLine(",DAY_PAID_MINUTES = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES > TIMESHEET_TOTAL_TABLE.BREAK_MINUTES ");
            strQry.AppendLine(" AND BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES < TIMESHEET_TOTAL_TABLE.TIMESHEET_ACCUM_MINUTES ");
            //strQry.AppendLine(" AND TIMESHEET_TOTAL_TABLE.BREAK_MINUTES <> 0 ");

            strQry.AppendLine(" THEN TIMESHEET_TOTAL_TABLE.TIMESHEET_ACCUM_MINUTES - BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES");

            strQry.AppendLine(" ELSE TIMESHEET_TOTAL_TABLE.TIMESHEET_ACCUM_MINUTES - TIMESHEET_TOTAL_TABLE.BREAK_MINUTES");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",INDICATOR = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN BREAK_SUMMARY_TABLE.INDICATOR = 'X' OR TIMESHEET_TOTAL_TABLE.TIMESHEET_ACCUM_MINUTES < BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES ");
            strQry.AppendLine(" THEN 'X'");

            strQry.AppendLine(" ELSE ISNULL(MAX(TIMESHEET_TOTAL_TABLE.INDICATOR),'')");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",BREAK_INDICATOR = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES > TIMESHEET_TOTAL_TABLE.BREAK_MINUTES ");
            strQry.AppendLine(" THEN 'Y'");

            strQry.AppendLine(" ELSE ''");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM ");

            strQry.AppendLine("(");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" TIMESHEET_SUMMARY_TABLE.COMPANY_NO");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.DAY_DATE");

            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.TIMESHEET_ACCUM_MINUTES ");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.INDICATOR ");

            strQry.AppendLine(",ISNULL(MAX(PCB.BREAK_MINUTES),0) AS BREAK_MINUTES ");

            strQry.AppendLine(" FROM ");

            strQry.AppendLine("(");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" TIMESHEET_TABLE.COMPANY_NO");
            strQry.AppendLine(",TIMESHEET_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TIMESHEET_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",TIMESHEET_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",TIMESHEET_TABLE.DAY_DATE");

            strQry.AppendLine(",SUM (TIMESHEET_ACCUM_MINUTES) AS TIMESHEET_ACCUM_MINUTES ");
            strQry.AppendLine(",ISNULL(MAX(INDICATOR),'') AS INDICATOR ");

            strQry.AppendLine(" FROM ");

            //Removes Duplicates where TIMESHEET_SEQ is Not Used in Join
            strQry.AppendLine("(");
            strQry.AppendLine(" SELECT DISTINCT");

            strQry.AppendLine(" ETC.COMPANY_NO");
            strQry.AppendLine(",'" + parstrPayCategoryType + "' AS PAY_CATEGORY_TYPE");
            strQry.AppendLine(",ETC.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETC.EMPLOYEE_NO ");
            strQry.AppendLine(",ETC.TIMESHEET_DATE AS DAY_DATE");

            //2014-05-03
            strQry.AppendLine(",ETC.TIMESHEET_SEQ");

            strQry.AppendLine(",TIMESHEET_ACCUM_MINUTES = ");
            strQry.AppendLine(" CASE ");

            //Errol Checked
            strQry.AppendLine(" WHEN ((ETC.TIMESHEET_TIME_IN_MINUTES IS NULL");
            strQry.AppendLine(" OR ETC.TIMESHEET_TIME_OUT_MINUTES IS NULL)");
            //Same Row / Next Row
            strQry.AppendLine(" OR (ETC.TIMESHEET_TIME_IN_MINUTES > ETC2.TIMESHEET_TIME_OUT_MINUTES");
            strQry.AppendLine(" AND ETC.TIMESHEET_SEQ <= ETC2.TIMESHEET_SEQ)");
            //Different Rows
            strQry.AppendLine(" OR (ETC.TIMESHEET_TIME_IN_MINUTES < ETC2.TIMESHEET_TIME_OUT_MINUTES");
            strQry.AppendLine(" AND ETC.TIMESHEET_SEQ > ETC2.TIMESHEET_SEQ))");

            strQry.AppendLine(" THEN 0 ");

            strQry.AppendLine(" ELSE ");

            strQry.AppendLine(" ETC.TIMESHEET_TIME_OUT_MINUTES - ETC.TIMESHEET_TIME_IN_MINUTES ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",INDICATOR = ");

            strQry.AppendLine(" CASE ");

            //Errol Checked
            strQry.AppendLine(" WHEN ((ETC.TIMESHEET_TIME_IN_MINUTES IS NULL");
            strQry.AppendLine(" OR ETC.TIMESHEET_TIME_OUT_MINUTES IS NULL)");
            //Same Row / Next Row
            strQry.AppendLine(" OR (ETC.TIMESHEET_TIME_IN_MINUTES > ETC2.TIMESHEET_TIME_OUT_MINUTES");
            strQry.AppendLine(" AND ETC.TIMESHEET_SEQ <= ETC2.TIMESHEET_SEQ)");
            //Different Rows
            strQry.AppendLine(" OR (ETC.TIMESHEET_TIME_IN_MINUTES < ETC2.TIMESHEET_TIME_OUT_MINUTES");
            strQry.AppendLine(" AND ETC.TIMESHEET_SEQ > ETC2.TIMESHEET_SEQ))");

            strQry.AppendLine(" THEN 'X' ");

            strQry.AppendLine(" ELSE '' ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
            }
            else
            {
                if (parstrPayCategoryType == "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC");
                }
                else
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC");
                }
            }

            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO");
            strQry.AppendLine(" AND ETC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
            //2013-06-15 >= Cater For Employee Take-On 
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE >= ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrenDate + "'))");

            //Set Extra Days to 15
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= DATEADD(DD,15,'" + strCurrenDate + "')");

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC2");
            }
            else
            {
                if (parstrPayCategoryType == "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC2");
                }
                else
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC2");
                }
            }

            strQry.AppendLine(" ON ETC.COMPANY_NO = ETC2.COMPANY_NO");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = ETC2.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = ETC2.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE = ETC2.TIMESHEET_DATE");

            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND ETC.COMPANY_NO = UEPCT.COMPANY_NO ");
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            }

            //Errol 2012-09-20 Fix Change of PAY_CATEGORY (Orphan Records)
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

            strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            //Errol 2013-04-12
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON ETC.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            //1-End

            strQry.AppendLine(" ) AS TIMESHEET_TABLE ");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" TIMESHEET_TABLE.COMPANY_NO");
            strQry.AppendLine(",TIMESHEET_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TIMESHEET_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",TIMESHEET_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",TIMESHEET_TABLE.DAY_DATE");

            //2-End

            strQry.AppendLine(" ) AS TIMESHEET_SUMMARY_TABLE");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK PCB");

            strQry.AppendLine(" ON TIMESHEET_SUMMARY_TABLE.COMPANY_NO = PCB.COMPANY_NO");
            strQry.AppendLine(" AND TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_NO = PCB.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_TYPE = PCB.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PCB.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND TIMESHEET_SUMMARY_TABLE.TIMESHEET_ACCUM_MINUTES >= PCB.WORKED_TIME_MINUTES");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" TIMESHEET_SUMMARY_TABLE.COMPANY_NO");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.DAY_DATE");

            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.TIMESHEET_ACCUM_MINUTES ");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.INDICATOR ");

            //3-End

            strQry.AppendLine(" ) AS TIMESHEET_TOTAL_TABLE ");

            strQry.AppendLine(" LEFT JOIN  ");

            strQry.AppendLine("(");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" BREAK_TABLE.COMPANY_NO");
            strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",BREAK_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",BREAK_TABLE.DAY_DATE");

            strQry.AppendLine(",SUM(BREAK_TABLE.BREAK_ACCUM_MINUTES) AS BREAK_ACCUM_MINUTES ");
            strQry.AppendLine(",ISNULL(MAX(BREAK_TABLE.INDICATOR),'') AS INDICATOR ");

            strQry.AppendLine(" FROM ");

            //Removes Duplicates where BREAK_SEQ is Not Used in Join
            strQry.AppendLine("(");
            strQry.AppendLine(" SELECT DISTINCT");

            strQry.AppendLine(" EBC.COMPANY_NO");
            strQry.AppendLine(",'" + parstrPayCategoryType + "' AS PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EBC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBC.EMPLOYEE_NO ");
            strQry.AppendLine(",EBC.BREAK_DATE AS DAY_DATE");

            //ELR - 2014-05-03
            strQry.AppendLine(",EBC.BREAK_SEQ");

            strQry.AppendLine(",BREAK_ACCUM_MINUTES = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN ((EBC.BREAK_TIME_IN_MINUTES IS NULL");
            strQry.AppendLine(" OR EBC.BREAK_TIME_OUT_MINUTES IS NULL)");
            //Same Row / Next Row
            strQry.AppendLine(" OR (EBC.BREAK_TIME_IN_MINUTES > EBC2.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine(" AND EBC.BREAK_SEQ <= EBC2.BREAK_SEQ)");
            //Different Rows
            strQry.AppendLine(" OR (EBC.BREAK_TIME_IN_MINUTES < EBC2.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine(" AND EBC.BREAK_SEQ > EBC2.BREAK_SEQ))");

            strQry.AppendLine(" THEN 0 ");

            strQry.AppendLine(" ELSE ");

            strQry.AppendLine(" EBC.BREAK_TIME_OUT_MINUTES - EBC.BREAK_TIME_IN_MINUTES ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",INDICATOR = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN ((EBC.BREAK_TIME_IN_MINUTES IS NULL");
            strQry.AppendLine(" OR EBC.BREAK_TIME_OUT_MINUTES IS NULL)");
            //Same Row / Next Row
            strQry.AppendLine(" OR (EBC.BREAK_TIME_IN_MINUTES > EBC2.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine(" AND EBC.BREAK_SEQ <= EBC2.BREAK_SEQ)");
            //Different Rows
            strQry.AppendLine(" OR (EBC.BREAK_TIME_IN_MINUTES < EBC2.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine(" AND EBC.BREAK_SEQ > EBC2.BREAK_SEQ))");

            strQry.AppendLine(" THEN 'X' ");

            strQry.AppendLine(" ELSE '' ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT EBC");
            }
            else
            {
                if (parstrPayCategoryType == "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT EBC");
                }
                else
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT EBC");
                }
            }

            strQry.AppendLine(" ON E.COMPANY_NO = EBC.COMPANY_NO");
            strQry.AppendLine(" AND EBC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EBC.EMPLOYEE_NO ");
            //2013-06-15 >= Cater For Employee Take-On 
            strQry.AppendLine(" AND EBC.BREAK_DATE >= ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrenDate + "'))");

            //Set Extra Days to 15
            strQry.AppendLine(" AND EBC.BREAK_DATE <= DATEADD(DD,15,'" + strCurrenDate + "')");

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT EBC2");
            }
            else
            {
                if (parstrPayCategoryType == "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT EBC2");
                }
                else
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT EBC2");
                }
            }

            strQry.AppendLine(" ON EBC.COMPANY_NO = EBC2.COMPANY_NO");
            strQry.AppendLine(" AND EBC.EMPLOYEE_NO = EBC2.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = EBC2.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EBC.BREAK_DATE = EBC2.BREAK_DATE");

            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND EBC.COMPANY_NO = UEPCT.COMPANY_NO ");
                strQry.AppendLine(" AND EBC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            }

            //Errol 2012-09-20 Fix Change of PAY_CATEGORY (Orphan Records)
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

            strQry.AppendLine(" ON EBC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND EBC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            //Errol 2013-04-12
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON EBC.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            //1-End

            strQry.AppendLine(" ) AS BREAK_TABLE ");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" BREAK_TABLE.COMPANY_NO");
            strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",BREAK_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",BREAK_TABLE.DAY_DATE");

            //2-End

            strQry.AppendLine(" ) AS BREAK_SUMMARY_TABLE");

            strQry.AppendLine(" ON TIMESHEET_TOTAL_TABLE.COMPANY_NO = BREAK_SUMMARY_TABLE.COMPANY_NO");
            strQry.AppendLine(" AND TIMESHEET_TOTAL_TABLE.PAY_CATEGORY_TYPE = BREAK_SUMMARY_TABLE.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND TIMESHEET_TOTAL_TABLE.PAY_CATEGORY_NO = BREAK_SUMMARY_TABLE.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND TIMESHEET_TOTAL_TABLE.EMPLOYEE_NO = BREAK_SUMMARY_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(" AND TIMESHEET_TOTAL_TABLE.DAY_DATE = BREAK_SUMMARY_TABLE.DAY_DATE");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" TIMESHEET_TOTAL_TABLE.COMPANY_NO");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.DAY_DATE");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.TIMESHEET_ACCUM_MINUTES ");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.INDICATOR ");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.BREAK_MINUTES ");

            strQry.AppendLine(",BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES ");
            strQry.AppendLine(",BREAK_SUMMARY_TABLE.INDICATOR) AS TEMP1_TABLE ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC1");

            strQry.AppendLine(" ON TEMP1_TABLE.COMPANY_NO = PC1.COMPANY_NO");
            strQry.AppendLine(" AND TEMP1_TABLE.PAY_CATEGORY_NO = PC1.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND TEMP1_TABLE.PAY_CATEGORY_TYPE = PC1.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PC1.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            strQry.AppendLine(" ) AS TEMP2_TABLE");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC2");

            strQry.AppendLine(" ON TEMP2_TABLE.COMPANY_NO = PC2.COMPANY_NO");
            strQry.AppendLine(" AND TEMP2_TABLE.PAY_CATEGORY_NO = PC2.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND TEMP2_TABLE.PAY_CATEGORY_TYPE = PC2.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PC2.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");

            strQry.AppendLine(" ON TEMP2_TABLE.DAY_DATE = D.DAY_DATE");

            strQry.AppendLine(" LEFT JOIN ");
            strQry.AppendLine(" (SELECT ");
            strQry.AppendLine("   PC.PAY_CATEGORY_NO");
            strQry.AppendLine("  ,PH.PUBLIC_HOLIDAY_DATE");
            strQry.AppendLine("  FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY PH");

            strQry.AppendLine("  INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");

            strQry.AppendLine(" ON PC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND PC.PAY_PUBLIC_HOLIDAY_IND = 'Y'");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            strQry.AppendLine(" WHERE PH.PUBLIC_HOLIDAY_DATE > '" + DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd") + "') AS PUBLIC_HOLIDAY_TABLE");

            strQry.AppendLine(" ON PC2.PAY_CATEGORY_NO = PUBLIC_HOLIDAY_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND D.DAY_DATE = PUBLIC_HOLIDAY_TABLE.PUBLIC_HOLIDAY_DATE");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "DayTotal", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ETC.COMPANY_NO");
            strQry.AppendLine(",'" + parstrPayCategoryType + "' AS PAY_CATEGORY_TYPE");
            strQry.AppendLine(",ETC.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETC.EMPLOYEE_NO ");
            strQry.AppendLine(",ETC.TIMESHEET_DATE");
            strQry.AppendLine(",ETC.TIMESHEET_SEQ");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_OUT_MINUTES");
            strQry.AppendLine(",ETC.CLOCKED_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC.CLOCKED_TIME_OUT_MINUTES");

            strQry.AppendLine(",TIMESHEET_ACCUM_MINUTES = ");
            strQry.AppendLine(" CASE ");
            strQry.AppendLine(" WHEN NOT TEMP.EMPLOYEE_NO IS NULL");

            strQry.AppendLine(" THEN 0 ");

            strQry.AppendLine(" ELSE ");

            strQry.AppendLine(" ETC.TIMESHEET_TIME_OUT_MINUTES - ETC.TIMESHEET_TIME_IN_MINUTES ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",INDICATOR = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN NOT TEMP.EMPLOYEE_NO IS NULL");

            strQry.AppendLine(" THEN 'X' ");

            strQry.AppendLine(" ELSE '' ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
            }
            else
            {
                if (parstrPayCategoryType == "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC");
                }
                else
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC");
                }
            }

            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO");
            strQry.AppendLine(" AND ETC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
            //2013-06-15 >= Cater For Employee Take-On 
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE >= ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrenDate + "'))");
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= DATEADD(DD,15,'" + strCurrenDate + "')");

            strQry.AppendLine(" LEFT JOIN ");

            //NB DISTINCT Removes Duplicates Generated when //strQry.AppendLine(" AND ETC.TIMESHEET_SEQ = ETC2.TIMESHEET_SEQ" is Removed
            strQry.AppendLine("(SELECT DISTINCT ");
            strQry.AppendLine(" ETC.EMPLOYEE_NO ");
            strQry.AppendLine(",ETC.TIMESHEET_DATE");
            strQry.AppendLine(",ETC.TIMESHEET_SEQ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
            }
            else
            {
                if (parstrPayCategoryType == "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC");
                }
                else
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC");
                }
            }

            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO");
            strQry.AppendLine(" AND ETC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
            //2013-06-15 >= Cater For Employee Take-On 
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE >= ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrenDate + "'))");
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= DATEADD(DD,15,'" + strCurrenDate + "')");

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC2");
            }
            else
            {
                if (parstrPayCategoryType == "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC2");
                }
                else
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC2");
                }
            }

            strQry.AppendLine(" ON ETC.COMPANY_NO = ETC2.COMPANY_NO");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = ETC2.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = ETC2.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE = ETC2.TIMESHEET_DATE");
            //strQry.AppendLine(" AND ETC.TIMESHEET_SEQ = ETC2.TIMESHEET_SEQ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            //Errol Checked
            strQry.AppendLine(" AND ((ETC.TIMESHEET_TIME_IN_MINUTES IS NULL");
            strQry.AppendLine(" OR ETC.TIMESHEET_TIME_OUT_MINUTES IS NULL)");
            //Same Row
            strQry.AppendLine(" OR (ETC.TIMESHEET_TIME_IN_MINUTES > ETC2.TIMESHEET_TIME_OUT_MINUTES");
            strQry.AppendLine(" AND ETC.TIMESHEET_SEQ <= ETC2.TIMESHEET_SEQ)");

            //Different Rows
            strQry.AppendLine(" OR (ETC.TIMESHEET_TIME_IN_MINUTES < ETC2.TIMESHEET_TIME_OUT_MINUTES");
            strQry.AppendLine(" AND ETC.TIMESHEET_SEQ > ETC2.TIMESHEET_SEQ))) AS TEMP");

            strQry.AppendLine(" ON ETC.EMPLOYEE_NO = TEMP.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE = TEMP.TIMESHEET_DATE");
            strQry.AppendLine(" AND ETC.TIMESHEET_SEQ = TEMP.TIMESHEET_SEQ ");

            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND ETC.COMPANY_NO = UEPCT.COMPANY_NO ");
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            }

            //Errol 2012-09-20 Fix Change of PAY_CATEGORY (Orphan Records)
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

            strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            //Errol 2013-04-12
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON ETC.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 1");
            strQry.AppendLine(",2");
            strQry.AppendLine(",3 ");
            strQry.AppendLine(",4");
            strQry.AppendLine(",5");
            strQry.AppendLine(",6");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "TimeSheet", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ETC.COMPANY_NO");
            strQry.AppendLine(",'" + parstrPayCategoryType + "' AS PAY_CATEGORY_TYPE");
            strQry.AppendLine(",ETC.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETC.EMPLOYEE_NO ");
            strQry.AppendLine(",ETC.BREAK_DATE");
            strQry.AppendLine(",ETC.BREAK_SEQ");
            strQry.AppendLine(",ETC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine(",ETC.CLOCKED_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC.CLOCKED_TIME_OUT_MINUTES");

            strQry.AppendLine(",BREAK_ACCUM_MINUTES = ");
            strQry.AppendLine(" CASE ");
            strQry.AppendLine(" WHEN NOT TEMP.EMPLOYEE_NO IS NULL");

            strQry.AppendLine(" THEN 0 ");

            strQry.AppendLine(" ELSE ");

            strQry.AppendLine(" ETC.BREAK_TIME_OUT_MINUTES - ETC.BREAK_TIME_IN_MINUTES ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",INDICATOR = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN NOT TEMP.EMPLOYEE_NO IS NULL");

            strQry.AppendLine(" THEN 'X' ");

            strQry.AppendLine(" ELSE '' ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT ETC");
            }
            else
            {
                if (parstrPayCategoryType == "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ETC");
                }
                else
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETC");
                }
            }

            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO");
            strQry.AppendLine(" AND ETC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
            //2013-06-15 >= Cater For Employee Take-On 
            strQry.AppendLine(" AND ETC.BREAK_DATE >= ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrenDate + "'))");
            strQry.AppendLine(" AND ETC.BREAK_DATE <= DATEADD(DD,15,'" + strCurrenDate + "')");

            strQry.AppendLine(" LEFT JOIN ");

            //NB DISTINCT Removes Duplicates Generated when //strQry.AppendLine(" AND ETC.BREAK_SEQ = ETC2.BREAK_SEQ" is Removed
            strQry.AppendLine("(SELECT DISTINCT ");
            strQry.AppendLine(" ETC.EMPLOYEE_NO ");
            strQry.AppendLine(",ETC.BREAK_DATE");
            strQry.AppendLine(",ETC.BREAK_SEQ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT ETC");
            }
            else
            {
                if (parstrPayCategoryType == "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ETC");
                }
                else
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETC");
                }
            }

            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO");
            strQry.AppendLine(" AND ETC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
            //2013-06-15 >= Cater For Employee Take-On 
            strQry.AppendLine(" AND ETC.BREAK_DATE >= ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrenDate + "'))");
            strQry.AppendLine(" AND ETC.BREAK_DATE <= DATEADD(DD,15,'" + strCurrenDate + "')");

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT ETC2");
            }
            else
            {
                if (parstrPayCategoryType == "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ETC2");
                }
                else
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETC2");
                }
            }

            strQry.AppendLine(" ON ETC.COMPANY_NO = ETC2.COMPANY_NO");
            strQry.AppendLine(" AND ETC2.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = ETC2.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = ETC2.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ETC.BREAK_DATE = ETC2.BREAK_DATE");
            //strQry.AppendLine(" AND ETC.BREAK_SEQ = ETC2.BREAK_SEQ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            strQry.AppendLine(" AND ((ETC.BREAK_TIME_IN_MINUTES IS NULL");
            strQry.AppendLine(" OR ETC.BREAK_TIME_OUT_MINUTES IS NULL)");
            //Same Row
            strQry.AppendLine(" OR (ETC.BREAK_TIME_IN_MINUTES > ETC2.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine(" AND ETC.BREAK_SEQ <= ETC2.BREAK_SEQ)");

            //Different Rows
            strQry.AppendLine(" OR (ETC.BREAK_TIME_IN_MINUTES < ETC2.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine(" AND ETC.BREAK_SEQ > ETC2.BREAK_SEQ))) AS TEMP");

            strQry.AppendLine(" ON ETC.EMPLOYEE_NO = TEMP.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ETC.BREAK_DATE = TEMP.BREAK_DATE");
            strQry.AppendLine(" AND ETC.BREAK_SEQ = TEMP.BREAK_SEQ ");

            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND ETC.COMPANY_NO = UEPCT.COMPANY_NO ");
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            }

            //Errol 2012-09-20 Fix Change of PAY_CATEGORY (Orphan Records)
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

            strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            //Errol 2013-04-12
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON ETC.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 1");
            strQry.AppendLine(",2");
            strQry.AppendLine(",3 ");
            strQry.AppendLine(",4");
            strQry.AppendLine(",5");
            strQry.AppendLine(",6");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Break", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PCB.COMPANY_NO ");
            strQry.AppendLine(",PCB.PAY_CATEGORY_NO");
            strQry.AppendLine(",PCB.PAY_CATEGORY_TYPE");

            strQry.AppendLine(",PCB.WORKED_TIME_MINUTES");
            strQry.AppendLine(",PCB.BREAK_MINUTES");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK PCB");

            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(SELECT DISTINCT");
                strQry.AppendLine(" PAY_CATEGORY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP ");

                strQry.AppendLine(" WHERE USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "') AS USER_TABLE");

                strQry.AppendLine(" ON PCB.PAY_CATEGORY_NO = USER_TABLE.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCB.PAY_CATEGORY_TYPE = USER_TABLE.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE PCB.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PCB.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            strQry.AppendLine(" AND PCB.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" UNION ");

            //Create 1 Row with 0 WORKED_TIME_MINUTES and 0 BREAK_MINUTES
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PC.COMPANY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");

            strQry.AppendLine(",0 AS WORKED_TIME_MINUTES");
            strQry.AppendLine(",0 AS BREAK_MINUTES");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");

            strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" ORDER BY ");

            strQry.AppendLine(" 1");
            strQry.AppendLine(",2");
            strQry.AppendLine(",3");
            strQry.AppendLine(",4");
            strQry.AppendLine(",5");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "BreakRange", parint64CompanyNo);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
              
        public byte[] Update_Records(Int64 parint64CompanyNo,int parPayCategoryNo,string parstrPayCategoryType,int parintAuthoriseLevel, Int64 parint64CurrentUserNo, string parstrCurrentUserAccessInd, string parstrFromProgram, byte[] parbyteDataSet)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);
            StringBuilder strQry = new StringBuilder();
           
            for (int intRow = 0; intRow < parDataSet.Tables["EmployeePayCategoryLevel"].Rows.Count; intRow++)
            {
                strQry.Clear();
                strQry.AppendLine(" UPDATE EPCPAC");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" AUTHORISED_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EmployeePayCategoryLevel"].Rows[intRow]["AUTHORISED_IND"].ToString()));

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT EPCPAC ");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT EPCPAC_T ");
                strQry.AppendLine(" ON EPCPAC.COMPANY_NO = EPCPAC_T.COMPANY_NO ");
                strQry.AppendLine(" AND EPCPAC.EMPLOYEE_NO = EPCPAC_T.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPCPAC.PAY_CATEGORY_NO = EPCPAC_T.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND EPCPAC.PAY_CATEGORY_TYPE = EPCPAC_T.PAY_CATEGORY_TYPE ");
               
                strQry.AppendLine(" AND EPCPAC.LEVEL_NO = EPCPAC_T.LEVEL_NO ");
                strQry.AppendLine(" AND EPCPAC.USER_NO = EPCPAC_T.USER_NO ");
                strQry.AppendLine(" AND EPCPAC_T.AUTHORISED_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EmployeePayCategoryLevel"].Rows[intRow]["AUTHORISED_IND"].ToString()));

                strQry.AppendLine(" WHERE EPCPAC.COMPANY_NO = " + parDataSet.Tables["EmployeePayCategoryLevel"].Rows[intRow]["COMPANY_NO"].ToString());
                strQry.AppendLine(" AND EPCPAC.EMPLOYEE_NO = " + parDataSet.Tables["EmployeePayCategoryLevel"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                strQry.AppendLine(" AND EPCPAC.PAY_CATEGORY_NO = " + parDataSet.Tables["EmployeePayCategoryLevel"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(" AND EPCPAC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EmployeePayCategoryLevel"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                if (parDataSet.Tables["EmployeePayCategoryLevel"].Rows[intRow]["AUTHORISED_IND"].ToString() == "Y")
                {
                    strQry.AppendLine(" AND EPCPAC.LEVEL_NO = " + parDataSet.Tables["EmployeePayCategoryLevel"].Rows[intRow]["LEVEL_NO"].ToString());

                    strQry.AppendLine(" AND EPCPAC.USER_NO = " + parint64CurrentUserNo);
                }
                else
                {
                    //Set All Higher Levels Also to NOT Authorised
                    strQry.AppendLine(" AND EPCPAC.LEVEL_NO >= " + parDataSet.Tables["EmployeePayCategoryLevel"].Rows[intRow]["LEVEL_NO"].ToString());
                }

                //Row is Different
                strQry.AppendLine(" AND EPCPAC_T.COMPANY_NO IS NULL ");
         
                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            }
 
            byte[] byteArrayDataSet = Get_User_Level_Records(parint64CompanyNo, parint64CurrentUserNo, parstrCurrentUserAccessInd, parstrFromProgram);

            DataSet DataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(byteArrayDataSet);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            return bytCompress;
        }
    }
}
