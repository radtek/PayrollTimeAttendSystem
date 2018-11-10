using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using InteractPayroll;

namespace InteractPayrollClient
{
    public class busTimeSheetClient
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        //Use New Tables Creates by Triggers
        bool pvtblnUseNew = true;

        public busTimeSheetClient()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(int parintCurrentUserNo, string parstrAccessInd)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();
            
            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" C.COMPANY_NO");
            strQry.AppendLine(",C.COMPANY_DESC");

            if (parstrAccessInd == "U")
            {
                strQry.AppendLine(",'U' AS ACCESS_IND");
            }
            else
            {
               strQry.AppendLine(",'A' AS ACCESS_IND");
            }

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.COMPANY C");
             
            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND C.COMPANY_NO = UEPCT.COMPANY_NO");
            }

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" C.COMPANY_DESC");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Company");

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" PC.PAY_CATEGORY_TYPE");
          
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.COMPANY C");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON C.COMPANY_NO = PC.COMPANY_NO");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND C.COMPANY_NO = UEPCT.COMPANY_NO");
            }

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Temp");

            DataSet.Tables.Add("PayrollType");
            DataTable PayrollTypeDataTable = new DataTable("PayrollType");
            DataSet.Tables["PayrollType"].Columns.Add("PAYROLL_TYPE_DESC", typeof(String));

            DataView PayrollTypeDataView = new DataView(DataSet.Tables["Temp"],
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
            PayrollTypeDataView = new DataView(DataSet.Tables["Temp"],
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
            PayrollTypeDataView = new DataView(DataSet.Tables["Temp"],
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
            
            if (DataSet.Tables["Company"].Rows.Count > 0)
            {
                byte[] bytTempCompress = Get_Company_Records(Convert.ToInt64(DataSet.Tables["Company"].Rows[0]["COMPANY_NO"]), parintCurrentUserNo, parstrAccessInd);

                DataSet TempDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(bytTempCompress);
                DataSet.Merge(TempDataSet);
            }

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_Company_Records(Int64 parint64CompanyNo, int parintCurrentUserNo, string parstrAccessInd)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();

            string strCurrenDate = DateTime.Now.ToString("yyyy-MM-dd");
            
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
            strQry.AppendLine(",PC.LUNCH_TIME_MINUTES_DEDUCTED");
            strQry.AppendLine(",PC.LUNCH_TIME_WORKED_MINUTES");
            strQry.AppendLine(",PC.DAILY_ROUNDING_IND");
            strQry.AppendLine(",PC.DAILY_ROUNDING_MINUTES");
            strQry.AppendLine(",PC.LAST_UPLOAD_DATETIME");
            strQry.AppendLine(",PC.NO_EDIT_IND");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY PC");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(SELECT DISTINCT");
                strQry.AppendLine(" PAY_CATEGORY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP ");
                strQry.AppendLine(" WHERE USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo + ") AS USER_TABLE");

                strQry.AppendLine(" ON PC.PAY_CATEGORY_NO = USER_TABLE.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = USER_TABLE.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PC.COMPANY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE DESC");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            
            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PayCategory");
                 
            if (DataSet.Tables["PayCategory"].Rows.Count > 0)
            {
                byte[] bytTempCompress = Get_PayCategory_Records(Convert.ToInt64(DataSet.Tables["PayCategory"].Rows[0]["COMPANY_NO"]),  DataSet.Tables["PayCategory"].Rows[0]["PAY_CATEGORY_TYPE"].ToString(), parintCurrentUserNo, parstrAccessInd);

                DataSet TempDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(bytTempCompress);
                DataSet.Merge(TempDataSet);
            }

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO ");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrenDate + "')) AS EMPLOYEE_LAST_RUNDATE");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" E.COMPANY_NO ");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");
              
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public string Get_DayTotal_SQL(Int64 parint64CompanyNo, int pvtintPayCategoryNo, string parstrPayCategoryType, string parstrDate, int parintCurrentUserNo, string parstrAccessInd)
        {
            StringBuilder strQry = new StringBuilder();

            string strCurrenDate = DateTime.Now.ToString("yyyy-MM-dd");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ETATBDC.COMPANY_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",ETATBDC.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETATBDC.EMPLOYEE_NO ");
            strQry.AppendLine(",ETATBDC.TIMESHEET_DATE AS DAY_DATE");
            strQry.AppendLine(",ETATBDC.DAY_NO");
            strQry.AppendLine(",ETATBDC.DAY_PAID_MINUTES");
            strQry.AppendLine(",ETATBDC.INDICATOR ");
            strQry.AppendLine(",ETATBDC.BREAK_ACCUM_MINUTES");
            strQry.AppendLine(",ETATBDC.BREAK_INDICATOR");

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT ETATBDC");
            }
            else
            {
                if (parstrPayCategoryType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT ETATBDC");

                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT ETATBDC");
                }
            }

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON ETATBDC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND ETATBDC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ETATBDC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");
            strQry.AppendLine(" ON ETATBDC.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND ETATBDC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND ETATBDC.COMPANY_NO = UEPCT.COMPANY_NO ");
                strQry.AppendLine(" AND ETATBDC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ETATBDC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            }

            strQry.AppendLine(" WHERE ETATBDC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND ETATBDC.TIMESHEET_DATE > ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrenDate + "'))");
            strQry.AppendLine(" AND ETATBDC.TIMESHEET_DATE <= DATEADD(DD,15,'" + strCurrenDate + "')");

            if (pvtintPayCategoryNo != 0)
            {
                strQry.AppendLine(" AND ETATBDC.PAY_CATEGORY_NO = " + pvtintPayCategoryNo);
                strQry.AppendLine(" AND ETATBDC.TIMESHEET_DATE = '" + parstrDate + "'");
            }

            return strQry.ToString();
        }

        public string Get_Timesheet_SQL(Int64 parint64CompanyNo, int pvtintPayCategoryNo, string parstrPayCategoryType, string parstrDate, int parintCurrentUserNo, string parstrAccessInd)
        {
            StringBuilder strQry = new StringBuilder();

            string strCurrenDate = DateTime.Now.ToString("yyyy-MM-dd");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ETC.COMPANY_NO");
            strQry.AppendLine(",'" + parstrPayCategoryType + "' AS PAY_CATEGORY_TYPE");
            strQry.AppendLine(",ETC.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETC.EMPLOYEE_NO ");
            strQry.AppendLine(",ETC.TIMESHEET_DATE");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_OUT_MINUTES");
            strQry.AppendLine(",ETC.CLOCKED_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC.CLOCKED_TIME_OUT_MINUTES");
            strQry.AppendLine(",ETC.TIMESHEET_SEQ");
            strQry.AppendLine(",ETC.TIMESHEET_ACCUM_MINUTES");
            strQry.AppendLine(",ETC.INDICATOR ");

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
            }
            else
            {
                if (parstrPayCategoryType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC");
                }
            }

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");
            strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND ETC.COMPANY_NO = UEPCT.COMPANY_NO ");
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            }

            strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE > ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrenDate + "'))");
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= DATEADD(DD,15,'" + strCurrenDate + "')");

            if (pvtintPayCategoryNo != 0)
            {
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + pvtintPayCategoryNo);
                strQry.AppendLine(" AND ETC.TIMESHEET_DATE = '" + parstrDate + "'");
            }

            return strQry.ToString();
        }
        
        public string Get_Break_SQL(Int64 parint64CompanyNo, int pvtintPayCategoryNo, string parstrPayCategoryType, string parstrDate, int parintCurrentUserNo, string parstrAccessInd)
        {
            StringBuilder strQry = new StringBuilder();

            string strCurrenDate = DateTime.Now.ToString("yyyy-MM-dd");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ETC.COMPANY_NO");
            strQry.AppendLine(",'" + parstrPayCategoryType + "' AS PAY_CATEGORY_TYPE");
            strQry.AppendLine(",ETC.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETC.EMPLOYEE_NO ");
            strQry.AppendLine(",ETC.BREAK_DATE");
            strQry.AppendLine(",ETC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine(",ETC.CLOCKED_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC.CLOCKED_TIME_OUT_MINUTES");
            strQry.AppendLine(",ETC.BREAK_SEQ");
            strQry.AppendLine(",ETC.BREAK_ACCUM_MINUTES");
            strQry.AppendLine(",ETC.INDICATOR ");

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT ETC");
            }
            else
            {
                if (parstrPayCategoryType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ETC");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETC");
                }
            }

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");
            strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND ETC.COMPANY_NO = UEPCT.COMPANY_NO ");
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            }

            strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND ETC.BREAK_DATE > ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrenDate + "'))");
            strQry.AppendLine(" AND ETC.BREAK_DATE <= DATEADD(DD,15,'" + strCurrenDate + "')");

            if (pvtintPayCategoryNo != 0)
            {
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + pvtintPayCategoryNo);
                strQry.AppendLine(" AND ETC.BREAK_DATE = '" + parstrDate + "'");
            }

            return strQry.ToString();
        }

        public byte[] Get_PayCategory_Records(Int64 parint64CompanyNo,string parstrPayCategoryType,int parintCurrentUserNo, string parstrAccessInd)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();

            string strCurrenDate = DateTime.Now.ToString("yyyy-MM-dd");

            if (pvtblnUseNew == true)
            {
                strQry.Append(Get_DayTotal_SQL(parint64CompanyNo, 0, parstrPayCategoryType, "", parintCurrentUserNo, parstrAccessInd));
            }
            else
            {
                //Create Record Where No Timesheets Exist for Break Records
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

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E ");

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

                //ELR 2014-05-04
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

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                //2013-01-23
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");

                if (parstrPayCategoryType == "W")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT EBC");
                }
                else
                {
                    if (parstrPayCategoryType == "S")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT EBC");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT EBC");
                    }
                }

                strQry.AppendLine(" ON E.COMPANY_NO = EBC.COMPANY_NO");
                strQry.AppendLine(" AND EBC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EBC.EMPLOYEE_NO ");

                //2013-01-23
                strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");

                strQry.AppendLine(" AND EBC.BREAK_DATE > ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrenDate + "'))");

                //Set Extra Days to 15
                strQry.AppendLine(" AND EBC.BREAK_DATE <= DATEADD(DD,15,'" + strCurrenDate + "')");

                if (parstrPayCategoryType == "W")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT EBC2");
                }
                else
                {
                    if (parstrPayCategoryType == "S")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT EBC2");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT EBC2");
                    }
                }

                strQry.AppendLine(" ON EBC.COMPANY_NO = EBC2.COMPANY_NO");
                strQry.AppendLine(" AND EBC.EMPLOYEE_NO = EBC2.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = EBC2.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND EBC.BREAK_DATE = EBC2.BREAK_DATE");

                if (parstrAccessInd != "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                    strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo);
                    strQry.AppendLine(" AND EBC.COMPANY_NO = UEPCT.COMPANY_NO ");
                    strQry.AppendLine(" AND EBC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
                }

                strQry.AppendLine(" WHERE E.NOT_ACTIVE_IND IS NULL");
                strQry.AppendLine(" AND E.COMPANY_NO = " + parint64CompanyNo);
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
                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
                }
                else
                {
                    if (parstrPayCategoryType == "S")
                    {
                        strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC");
                    }
                    else
                    {
                        strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC");
                    }
                }

                strQry.AppendLine(" ON BREAK_SUMMARY_TABLE.COMPANY_NO = ETC.COMPANY_NO");

                strQry.AppendLine(" AND BREAK_SUMMARY_TABLE.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND BREAK_SUMMARY_TABLE.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND BREAK_SUMMARY_TABLE.DAY_DATE = ETC.TIMESHEET_DATE ");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.DATES D");
                strQry.AppendLine(" ON BREAK_SUMMARY_TABLE.DAY_DATE = D.DAY_DATE");

                strQry.AppendLine(" WHERE E.NOT_ACTIVE_IND IS NULL");
                strQry.AppendLine(" AND E.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

                //Remove Where Temesheets Exits
                strQry.AppendLine(" AND ETC.TIMESHEET_DATE IS NULL ");

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

                //ELR 2014-05-04
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

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                //2013-01-23
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");

                if (parstrPayCategoryType == "W")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
                }
                else
                {
                    if (parstrPayCategoryType == "S")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC");
                    }
                }

                strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO");
                strQry.AppendLine(" AND ETC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");

                //2013-01-23
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");

                strQry.AppendLine(" AND ETC.TIMESHEET_DATE > ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrenDate + "'))");

                //Set Extra Days to 15
                strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= DATEADD(DD,15,'" + strCurrenDate + "')");

                if (parstrPayCategoryType == "W")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC2");
                }
                else
                {
                    if (parstrPayCategoryType == "S")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC2");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC2");
                    }
                }

                strQry.AppendLine(" ON ETC.COMPANY_NO = ETC2.COMPANY_NO");
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = ETC2.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = ETC2.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND ETC.TIMESHEET_DATE = ETC2.TIMESHEET_DATE");

                if (parstrAccessInd != "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                    strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo);
                    strQry.AppendLine(" AND ETC.COMPANY_NO = UEPCT.COMPANY_NO ");
                    strQry.AppendLine(" AND ETC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
                }

                strQry.AppendLine(" WHERE E.NOT_ACTIVE_IND IS NULL");
                strQry.AppendLine(" AND E.COMPANY_NO = " + parint64CompanyNo);
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

                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.PAY_CATEGORY_BREAK PCB");

                strQry.AppendLine(" ON TIMESHEET_SUMMARY_TABLE.COMPANY_NO = PCB.COMPANY_NO");
                strQry.AppendLine(" AND TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_NO = PCB.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_TYPE = PCB.PAY_CATEGORY_TYPE");

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

                //ELR - 2014-04-23
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

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                if (parstrPayCategoryType == "W")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT EBC");
                }
                else
                {
                    if (parstrPayCategoryType == "S")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT EBC");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT EBC");
                    }
                }

                strQry.AppendLine(" ON E.COMPANY_NO = EBC.COMPANY_NO");
                strQry.AppendLine(" AND EBC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EBC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EBC.BREAK_DATE > ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrenDate + "'))");

                //Set Extra Days to 15
                strQry.AppendLine(" AND EBC.BREAK_DATE <= DATEADD(DD,15,'" + strCurrenDate + "')");

                if (parstrPayCategoryType == "W")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT EBC2");
                }
                else
                {
                    if (parstrPayCategoryType == "S")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT EBC2");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT EBC2");
                    }
                }

                strQry.AppendLine(" ON EBC.COMPANY_NO = EBC2.COMPANY_NO");
                strQry.AppendLine(" AND EBC.EMPLOYEE_NO = EBC2.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = EBC2.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND EBC.BREAK_DATE = EBC2.BREAK_DATE");

                if (parstrAccessInd != "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                    strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo);
                    strQry.AppendLine(" AND EBC.COMPANY_NO = UEPCT.COMPANY_NO ");
                    strQry.AppendLine(" AND EBC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
                }

                strQry.AppendLine(" WHERE E.NOT_ACTIVE_IND IS NULL");
                strQry.AppendLine(" AND E.COMPANY_NO = " + parint64CompanyNo);
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

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC1");
                strQry.AppendLine(" ON TEMP1_TABLE.COMPANY_NO = PC1.COMPANY_NO");
                strQry.AppendLine(" AND TEMP1_TABLE.PAY_CATEGORY_NO = PC1.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND TEMP1_TABLE.PAY_CATEGORY_TYPE = PC1.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND PC1.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

                strQry.AppendLine(" ) AS TEMP2_TABLE");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC2");
                strQry.AppendLine(" ON TEMP2_TABLE.COMPANY_NO = PC2.COMPANY_NO");
                strQry.AppendLine(" AND TEMP2_TABLE.PAY_CATEGORY_NO = PC2.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND TEMP2_TABLE.PAY_CATEGORY_TYPE = PC2.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND PC2.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.DATES D");
                strQry.AppendLine(" ON TEMP2_TABLE.DAY_DATE = D.DAY_DATE");
            }
            
            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "DayTotal");

            strQry.Clear();

            if (pvtblnUseNew == true)
            {
                strQry.Append(Get_Timesheet_SQL(parint64CompanyNo, 0, parstrPayCategoryType, "", parintCurrentUserNo, parstrAccessInd));
            }
            else
            {
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" ETC.COMPANY_NO");
                strQry.AppendLine(",'" + parstrPayCategoryType + "' AS PAY_CATEGORY_TYPE");
                strQry.AppendLine(",ETC.PAY_CATEGORY_NO");
                strQry.AppendLine(",ETC.EMPLOYEE_NO ");
                strQry.AppendLine(",ETC.TIMESHEET_DATE");
                strQry.AppendLine(",ETC.TIMESHEET_TIME_IN_MINUTES");
                strQry.AppendLine(",ETC.TIMESHEET_TIME_OUT_MINUTES");
                strQry.AppendLine(",ETC.TIMESHEET_SEQ");
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

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                if (parstrPayCategoryType == "W")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
                }
                else
                {
                    if (parstrPayCategoryType == "S")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC");
                    }
                }

                strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO");
                strQry.AppendLine(" AND ETC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");

                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO ");

                strQry.AppendLine(" AND ETC.TIMESHEET_DATE > ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrenDate + "'))");
                strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= DATEADD(DD,15,'" + strCurrenDate + "')");

                strQry.AppendLine(" LEFT JOIN ");

                //NB DISTINCT Removes Duplicates Generated when //strQry.AppendLine(" AND ETC.TIMESHEET_SEQ = ETC2.TIMESHEET_SEQ" is Removed
                strQry.AppendLine("(SELECT DISTINCT ");
                strQry.AppendLine(" ETC.EMPLOYEE_NO ");
                strQry.AppendLine(",ETC.TIMESHEET_DATE");
                strQry.AppendLine(",ETC.TIMESHEET_SEQ");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                if (parstrPayCategoryType == "W")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
                }
                else
                {
                    if (parstrPayCategoryType == "S")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC");
                    }
                }

                strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO");
                strQry.AppendLine(" AND ETC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");

                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");

                strQry.AppendLine(" AND ETC.TIMESHEET_DATE > ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrenDate + "'))");
                strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= DATEADD(DD,15,'" + strCurrenDate + "')");

                if (parstrPayCategoryType == "W")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC2");
                }
                else
                {
                    if (parstrPayCategoryType == "S")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC2");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC2");
                    }
                }

                strQry.AppendLine(" ON ETC.COMPANY_NO = ETC2.COMPANY_NO");
                strQry.AppendLine(" AND ETC2.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = ETC2.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = ETC2.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ETC.TIMESHEET_DATE = ETC2.TIMESHEET_DATE");
                //strQry.AppendLine(" AND ETC.TIMESHEET_SEQ = ETC2.TIMESHEET_SEQ");

                strQry.AppendLine(" WHERE E.NOT_ACTIVE_IND IS NULL");
                strQry.AppendLine(" AND E.COMPANY_NO = " + parint64CompanyNo);
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

                if (parstrAccessInd != "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                    strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo);
                    strQry.AppendLine(" AND ETC.COMPANY_NO = UEPCT.COMPANY_NO ");
                    strQry.AppendLine(" AND ETC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
                }

                strQry.AppendLine(" WHERE E.NOT_ACTIVE_IND IS NULL");
                strQry.AppendLine(" AND E.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            }

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 1");
            strQry.AppendLine(",2");
            strQry.AppendLine(",3 ");
            strQry.AppendLine(",4");
            strQry.AppendLine(",5");
            strQry.AppendLine(",6");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "TimeSheet");

            strQry.Clear();

            if (pvtblnUseNew == true)
            {
                strQry.Append(Get_Break_SQL(parint64CompanyNo, 0, parstrPayCategoryType, "", parintCurrentUserNo, parstrAccessInd));
            }
            else
            {
               
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

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                if (parstrPayCategoryType == "W")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT ETC");
                }
                else
                {
                    if (parstrPayCategoryType == "S")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ETC");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETC");
                    }
                }

                strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO");
                strQry.AppendLine(" AND ETC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");

                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");

                strQry.AppendLine(" AND ETC.BREAK_DATE > ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrenDate + "'))");
                strQry.AppendLine(" AND ETC.BREAK_DATE <= DATEADD(DD,15,'" + strCurrenDate + "')");

                strQry.AppendLine(" LEFT JOIN ");

                //NB DISTINCT Removes Duplicates Generated when //strQry.AppendLine(" AND ETC.BREAK_SEQ = ETC2.BREAK_SEQ" is Removed
                strQry.AppendLine("(SELECT DISTINCT ");
                strQry.AppendLine(" ETC.EMPLOYEE_NO ");
                strQry.AppendLine(",ETC.BREAK_DATE");
                strQry.AppendLine(",ETC.BREAK_SEQ");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                if (parstrPayCategoryType == "W")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT ETC");
                }
                else
                {
                    if (parstrPayCategoryType == "S")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ETC");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETC");
                    }
                }

                strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO");
                strQry.AppendLine(" AND ETC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");

                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");

                strQry.AppendLine(" AND ETC.BREAK_DATE > ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrenDate + "'))");
                strQry.AppendLine(" AND ETC.BREAK_DATE <= DATEADD(DD,15,'" + strCurrenDate + "')");

                if (parstrPayCategoryType == "W")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT ETC2");
                }
                else
                {
                    if (parstrPayCategoryType == "S")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ETC2");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETC2");
                    }
                }

                strQry.AppendLine(" ON ETC.COMPANY_NO = ETC2.COMPANY_NO");
                strQry.AppendLine(" AND ETC2.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = ETC2.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = ETC2.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ETC.BREAK_DATE = ETC2.BREAK_DATE");
                //strQry.AppendLine(" AND ETC.BREAK_SEQ = ETC2.BREAK_SEQ");

                strQry.AppendLine(" WHERE E.NOT_ACTIVE_IND IS NULL");
                strQry.AppendLine(" AND E.COMPANY_NO = " + parint64CompanyNo);
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

                if (parstrAccessInd != "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                    strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo);
                    strQry.AppendLine(" AND ETC.COMPANY_NO = UEPCT.COMPANY_NO ");
                    strQry.AppendLine(" AND ETC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
                }

                strQry.AppendLine(" WHERE E.NOT_ACTIVE_IND IS NULL");
                strQry.AppendLine(" AND E.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            }

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 1");
            strQry.AppendLine(",2");
            strQry.AppendLine(",3 ");
            strQry.AppendLine(",4");
            strQry.AppendLine(",5");
            strQry.AppendLine(",6");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Break");
                
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PCB.COMPANY_NO ");
            strQry.AppendLine(",PCB.PAY_CATEGORY_NO");
            strQry.AppendLine(",PCB.PAY_CATEGORY_TYPE");

            strQry.AppendLine(",PCB.WORKED_TIME_MINUTES");
            strQry.AppendLine(",PCB.BREAK_MINUTES");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY_BREAK PCB");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(SELECT DISTINCT");
                strQry.AppendLine(" PAY_CATEGORY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP ");

                strQry.AppendLine(" WHERE USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "') AS USER_TABLE");

                strQry.AppendLine(" ON PCB.PAY_CATEGORY_NO = USER_TABLE.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCB.PAY_CATEGORY_TYPE = USER_TABLE.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE PCB.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PCB.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" COMPANY_NO ");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");

            strQry.AppendLine(",0 AS WORKED_TIME_MINUTES");
            strQry.AppendLine(",0 AS BREAK_MINUTES");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
   
            strQry.AppendLine(" ORDER BY ");
           
            strQry.AppendLine(" 1");
            strQry.AppendLine(",2");
            strQry.AppendLine(",3");
            strQry.AppendLine(",4");
            strQry.AppendLine(",5");
          
            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "BreakRange");
                                                
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO ");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",D.DAY_DATE");
            strQry.AppendLine(",D.DAY_NO");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.DATES D");
            strQry.AppendLine(" ON D.DAY_DATE <= DATEADD(DD,15,'" + strCurrenDate + "')");
            strQry.AppendLine(" AND D.DAY_DATE > ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrenDate + "'))");
  
            strQry.AppendLine(" WHERE E.NOT_ACTIVE_IND IS NULL");
            strQry.AppendLine(" AND E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" E.COMPANY_NO ");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",D.DAY_DATE");
            strQry.AppendLine(",D.DAY_NO");
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 3");
            strQry.AppendLine(",4 DESC");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Dates");

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_Day_Timesheets_Records(Int64 parint64CompanyNo, int pvtintPayCategoryNo, string parstrPayCategoryType, string parstrDate, int parintCurrentUserNo, string parstrAccessInd)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();
           
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
            strQry.AppendLine(",PC.LUNCH_TIME_MINUTES_DEDUCTED");
            strQry.AppendLine(",PC.LUNCH_TIME_WORKED_MINUTES");
            strQry.AppendLine(",PC.DAILY_ROUNDING_IND");
            strQry.AppendLine(",PC.DAILY_ROUNDING_MINUTES");
            strQry.AppendLine(",PC.LAST_UPLOAD_DATETIME");
            strQry.AppendLine(",PC.NO_EDIT_IND");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY PC");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(SELECT DISTINCT");
                strQry.AppendLine(" PAY_CATEGORY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP ");
                strQry.AppendLine(" WHERE USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo + ") AS USER_TABLE");

                strQry.AppendLine(" ON PC.PAY_CATEGORY_NO = USER_TABLE.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = USER_TABLE.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = " + pvtintPayCategoryNo);

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PC.COMPANY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PayCategory");
                              
            //Create Record Where No Timesheets Exist for Break Records
            strQry.Clear();

            if (pvtblnUseNew == true)
            {
                strQry.Append(Get_DayTotal_SQL(parint64CompanyNo, pvtintPayCategoryNo, parstrPayCategoryType, parstrDate, parintCurrentUserNo, parstrAccessInd));
            }
            else
            {
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

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E ");

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

                //ELR 2014-05-04
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

                if (parstrPayCategoryType == "W")
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT EBC");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT EBC2");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT EBC");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT EBC2");
                }

                strQry.AppendLine(" ON EBC.COMPANY_NO = EBC2.COMPANY_NO");
                strQry.AppendLine(" AND EBC.EMPLOYEE_NO = EBC2.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = EBC2.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND EBC.BREAK_DATE = EBC2.BREAK_DATE");

                if (parstrAccessInd != "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                    strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo);
                    strQry.AppendLine(" AND EBC.COMPANY_NO = UEPCT.COMPANY_NO ");
                    strQry.AppendLine(" AND EBC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
                }

                strQry.AppendLine(" WHERE EBC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = " + pvtintPayCategoryNo);
                strQry.AppendLine(" AND EBC.BREAK_DATE = '" + parstrDate + "'");

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
                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
                }
                else
                {
                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC");
                }

                strQry.AppendLine(" ON BREAK_SUMMARY_TABLE.COMPANY_NO = ETC.COMPANY_NO");

                strQry.AppendLine(" AND BREAK_SUMMARY_TABLE.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND BREAK_SUMMARY_TABLE.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND BREAK_SUMMARY_TABLE.DAY_DATE = ETC.TIMESHEET_DATE ");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.DATES D");
                strQry.AppendLine(" ON BREAK_SUMMARY_TABLE.DAY_DATE = D.DAY_DATE");
                strQry.AppendLine(" AND D.DAY_DATE = '" + parstrDate + "'");

                //Remove Where Temesheets Exits
                strQry.AppendLine(" WHERE ETC.TIMESHEET_DATE IS NULL");

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
                strQry.AppendLine(",MAX(INDICATOR) AS INDICATOR ");

                strQry.AppendLine(" FROM ");

                //Removes Duplicates where TIMESHEET_SEQ is Not Used in Join
                strQry.AppendLine("(");
                strQry.AppendLine(" SELECT DISTINCT");

                strQry.AppendLine(" ETC.COMPANY_NO");
                strQry.AppendLine(",'" + parstrPayCategoryType + "' AS PAY_CATEGORY_TYPE");
                strQry.AppendLine(",ETC.PAY_CATEGORY_NO");
                strQry.AppendLine(",ETC.EMPLOYEE_NO ");
                strQry.AppendLine(",ETC.TIMESHEET_DATE AS DAY_DATE");

                //ELR 2014-05-04
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

                if (parstrPayCategoryType == "W")
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC2");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC2");
                }

                strQry.AppendLine(" ON ETC.COMPANY_NO = ETC2.COMPANY_NO");
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = ETC2.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = ETC2.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND ETC.TIMESHEET_DATE = ETC2.TIMESHEET_DATE");

                if (parstrAccessInd != "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                    strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo);
                    strQry.AppendLine(" AND ETC.COMPANY_NO = UEPCT.COMPANY_NO ");
                    strQry.AppendLine(" AND ETC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
                }

                strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + pvtintPayCategoryNo);
                strQry.AppendLine(" AND ETC.TIMESHEET_DATE = '" + parstrDate + "'");

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

                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.PAY_CATEGORY_BREAK PCB");

                strQry.AppendLine(" ON TIMESHEET_SUMMARY_TABLE.COMPANY_NO = PCB.COMPANY_NO");
                strQry.AppendLine(" AND TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_NO = PCB.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_TYPE = PCB.PAY_CATEGORY_TYPE");

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
                strQry.AppendLine(",MAX(BREAK_TABLE.INDICATOR) AS INDICATOR ");

                strQry.AppendLine(" FROM ");

                //Removes Duplicates where BREAK_SEQ is Not Used in Join
                strQry.AppendLine("(");
                strQry.AppendLine(" SELECT DISTINCT");

                strQry.AppendLine(" EBC.COMPANY_NO");
                strQry.AppendLine(",'" + parstrPayCategoryType + "' AS PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EBC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EBC.EMPLOYEE_NO ");
                strQry.AppendLine(",EBC.BREAK_DATE AS DAY_DATE");

                //ELR 2014-05-04
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

                if (parstrPayCategoryType == "W")
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT EBC");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT EBC2");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT EBC");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT EBC2");
                }

                strQry.AppendLine(" ON EBC.COMPANY_NO = EBC2.COMPANY_NO");
                strQry.AppendLine(" AND EBC.EMPLOYEE_NO = EBC2.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = EBC2.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND EBC.BREAK_DATE = EBC2.BREAK_DATE");

                if (parstrAccessInd != "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                    strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo);
                    strQry.AppendLine(" AND EBC.COMPANY_NO = UEPCT.COMPANY_NO ");
                    strQry.AppendLine(" AND EBC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
                }

                strQry.AppendLine(" WHERE EBC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = " + pvtintPayCategoryNo);
                strQry.AppendLine(" AND EBC.BREAK_DATE = '" + parstrDate + "'");

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

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC1");
                strQry.AppendLine(" ON TEMP1_TABLE.COMPANY_NO = PC1.COMPANY_NO");
                strQry.AppendLine(" AND TEMP1_TABLE.PAY_CATEGORY_NO = PC1.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND TEMP1_TABLE.PAY_CATEGORY_TYPE = PC1.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND PC1.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

                strQry.AppendLine(" ) AS TEMP2_TABLE");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC2");
                strQry.AppendLine(" ON TEMP2_TABLE.COMPANY_NO = PC2.COMPANY_NO");
                strQry.AppendLine(" AND TEMP2_TABLE.PAY_CATEGORY_NO = PC2.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND TEMP2_TABLE.PAY_CATEGORY_TYPE = PC2.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND PC2.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.DATES D");
                strQry.AppendLine(" ON TEMP2_TABLE.DAY_DATE = D.DAY_DATE");
                strQry.AppendLine(" AND D.DAY_DATE = '" + parstrDate + "'");

            }

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "DayTotal");

            strQry.Clear();

            if (pvtblnUseNew == true)
            {
                strQry.Append(Get_Timesheet_SQL(parint64CompanyNo, pvtintPayCategoryNo, parstrPayCategoryType, parstrDate, parintCurrentUserNo, parstrAccessInd));
            }
            else
            {
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

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                if (parstrPayCategoryType == "W")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
                }
                else
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC");
                }

                strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO");
                strQry.AppendLine(" AND ETC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + pvtintPayCategoryNo);
                strQry.AppendLine(" AND ETC.TIMESHEET_DATE = '" + parstrDate + "'");

                strQry.AppendLine(" LEFT JOIN ");

                //NB DISTINCT Removes Duplicates Generated when //strQry.AppendLine(" AND ETC.TIMESHEET_SEQ = ETC2.TIMESHEET_SEQ" is Removed
                strQry.AppendLine("(SELECT DISTINCT ");
                strQry.AppendLine(" ETC.EMPLOYEE_NO ");
                strQry.AppendLine(",ETC.TIMESHEET_DATE");
                strQry.AppendLine(",ETC.TIMESHEET_SEQ");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                if (parstrPayCategoryType == "W")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
                }
                else
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC");
                }

                strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO");
                strQry.AppendLine(" AND ETC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + pvtintPayCategoryNo);
                strQry.AppendLine(" AND ETC.TIMESHEET_DATE = '" + parstrDate + "'");

                if (parstrPayCategoryType == "W")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC2");
                }
                else
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC2");
                }

                strQry.AppendLine(" ON ETC.COMPANY_NO = ETC2.COMPANY_NO");
                strQry.AppendLine(" AND ETC2.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = ETC2.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND ETC2.PAY_CATEGORY_NO = " + pvtintPayCategoryNo);
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = ETC2.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ETC.TIMESHEET_DATE = ETC2.TIMESHEET_DATE");
                //strQry.AppendLine(" AND ETC.TIMESHEET_SEQ = ETC2.TIMESHEET_SEQ");

                strQry.AppendLine(" WHERE E.NOT_ACTIVE_IND IS NULL");
                strQry.AppendLine(" AND E.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

                //Errol Checked
                strQry.AppendLine(" AND ((ETC.TIMESHEET_TIME_IN_MINUTES IS NULL");
                strQry.AppendLine(" OR ETC.TIMESHEET_TIME_OUT_MINUTES IS NULL)");
                //Same Row / Next Row
                strQry.AppendLine(" OR (ETC.TIMESHEET_TIME_IN_MINUTES > ETC2.TIMESHEET_TIME_OUT_MINUTES");
                strQry.AppendLine(" AND ETC.TIMESHEET_SEQ <= ETC2.TIMESHEET_SEQ)");
                //Different Rows
                strQry.AppendLine(" OR (ETC.TIMESHEET_TIME_IN_MINUTES < ETC2.TIMESHEET_TIME_OUT_MINUTES");
                strQry.AppendLine(" AND ETC.TIMESHEET_SEQ > ETC2.TIMESHEET_SEQ))) AS TEMP");

                strQry.AppendLine(" ON ETC.EMPLOYEE_NO = TEMP.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ETC.TIMESHEET_DATE = TEMP.TIMESHEET_DATE");
                strQry.AppendLine(" AND ETC.TIMESHEET_SEQ = TEMP.TIMESHEET_SEQ ");

                if (parstrAccessInd != "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                    strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo);
                    strQry.AppendLine(" AND ETC.COMPANY_NO = UEPCT.COMPANY_NO ");
                    strQry.AppendLine(" AND ETC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
                }

                strQry.AppendLine(" WHERE E.NOT_ACTIVE_IND IS NULL");
                strQry.AppendLine(" AND E.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            }
           
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 1");
            strQry.AppendLine(",2");
            strQry.AppendLine(",3 ");
            strQry.AppendLine(",4");
            strQry.AppendLine(",5");
            strQry.AppendLine(",6");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "TimeSheet");
            
            strQry.Clear();

            if (pvtblnUseNew == true)
            {
                strQry.Append(Get_Break_SQL(parint64CompanyNo, pvtintPayCategoryNo, parstrPayCategoryType, parstrDate, parintCurrentUserNo, parstrAccessInd));
            }
            else
            {
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

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                if (parstrPayCategoryType == "W")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT ETC");
                }
                else
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ETC");
                }

                strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO");
                strQry.AppendLine(" AND ETC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + pvtintPayCategoryNo);
                strQry.AppendLine(" AND ETC.BREAK_DATE = '" + parstrDate + "'");

                strQry.AppendLine(" LEFT JOIN ");

                //NB DISTINCT Removes Duplicates Generated when //strQry.AppendLine(" AND ETC.BREAK_SEQ = ETC2.BREAK_SEQ" is Removed
                strQry.AppendLine("(SELECT DISTINCT ");
                strQry.AppendLine(" ETC.EMPLOYEE_NO ");
                strQry.AppendLine(",ETC.BREAK_DATE");
                strQry.AppendLine(",ETC.BREAK_SEQ");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                if (parstrPayCategoryType == "W")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT ETC");
                }
                else
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ETC");
                }

                strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO");
                strQry.AppendLine(" AND ETC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + pvtintPayCategoryNo);
                strQry.AppendLine(" AND ETC.BREAK_DATE = '" + parstrDate + "'");

                if (parstrPayCategoryType == "W")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT ETC2");
                }
                else
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ETC2");
                }

                strQry.AppendLine(" ON ETC.COMPANY_NO = ETC2.COMPANY_NO");
                strQry.AppendLine(" AND ETC2.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = ETC2.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND ETC2.PAY_CATEGORY_NO = " + pvtintPayCategoryNo);
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = ETC2.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ETC.BREAK_DATE = ETC2.BREAK_DATE");
                //strQry.AppendLine(" AND ETC.BREAK_SEQ = ETC2.BREAK_SEQ");

                strQry.AppendLine(" WHERE E.NOT_ACTIVE_IND IS NULL");
                strQry.AppendLine(" AND E.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

                strQry.AppendLine(" AND ((ETC.BREAK_TIME_IN_MINUTES IS NULL");
                strQry.AppendLine(" OR ETC.BREAK_TIME_OUT_MINUTES IS NULL)");
                //Same Row / Next Row
                strQry.AppendLine(" OR (ETC.BREAK_TIME_IN_MINUTES > ETC2.BREAK_TIME_OUT_MINUTES");
                strQry.AppendLine(" AND ETC.BREAK_SEQ <= ETC2.BREAK_SEQ)");

                //Different Rows
                strQry.AppendLine(" OR (ETC.BREAK_TIME_IN_MINUTES < ETC2.BREAK_TIME_OUT_MINUTES");
                strQry.AppendLine(" AND ETC.BREAK_SEQ > ETC2.BREAK_SEQ))) AS TEMP");

                strQry.AppendLine(" ON ETC.EMPLOYEE_NO = TEMP.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ETC.BREAK_DATE = TEMP.BREAK_DATE");
                strQry.AppendLine(" AND ETC.BREAK_SEQ = TEMP.BREAK_SEQ ");

                if (parstrAccessInd != "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                    strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo);
                    strQry.AppendLine(" AND ETC.COMPANY_NO = UEPCT.COMPANY_NO ");
                    strQry.AppendLine(" AND ETC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
                }

                strQry.AppendLine(" WHERE E.NOT_ACTIVE_IND IS NULL");
                strQry.AppendLine(" AND E.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            }

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 1");
            strQry.AppendLine(",2");
            strQry.AppendLine(",3 ");
            strQry.AppendLine(",4");
            strQry.AppendLine(",5");
            strQry.AppendLine(",6");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Break");

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Update_TimeSheet_Records(byte[] parbyteDataSet, string parstrPayCategoryType)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);
            DataSet DataSetTemp = new System.Data.DataSet();

            StringBuilder strQry = new StringBuilder();
                
            for (int intRow = 0; intRow < DataSet.Tables["TimeSheet"].Rows.Count; intRow++)
            {
                if (DataSet.Tables["TimeSheet"].Rows[intRow].RowState == DataRowState.Added)
                {
                    if (DataSetTemp.Tables["TimesheetTemp"] != null)
                    {
                        DataSetTemp.Tables["TimesheetTemp"].Clear();
                    }
                    
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" ISNULL(MAX(TIMESHEET_SEQ),0) + 1 AS MAX_TIMESHEET_SEQ");

                    switch (parstrPayCategoryType)
                    {
                        case "W":

                            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT");
                            break;

                        case "S":

                            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT");
                            break;

                        case "T":

                            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT");
                            break;
                    }
                        
                    strQry.AppendLine(" WHERE COMPANY_NO = " + DataSet.Tables["TimeSheet"].Rows[intRow]["COMPANY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables["TimeSheet"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["TimeSheet"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                    strQry.AppendLine(" AND TIMESHEET_DATE = '" + Convert.ToDateTime(DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_DATE"]).ToString("yyyy-MM-dd") + "'");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSetTemp, "TimesheetTemp");

                    strQry.Clear();

                    switch (parstrPayCategoryType)
                    {
                        case "W":

                            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT");
                            break;

                        case "S":

                            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT");
                            break;

                        case "T":

                            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT");
                            break;
                    }

                    //Added to be Passed back To Client Layer
                    DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_SEQ"] = Convert.ToInt32(DataSetTemp.Tables["TimesheetTemp"].Rows[0]["MAX_TIMESHEET_SEQ"]);

                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",TIMESHEET_DATE");
                    strQry.AppendLine(",TIMESHEET_SEQ");
                    strQry.AppendLine(",TIMESHEET_TIME_IN_MINUTES");
                    strQry.AppendLine(",TIMESHEET_TIME_OUT_MINUTES)");
                    strQry.AppendLine(" VALUES ");
                    strQry.AppendLine("(" + DataSet.Tables["TimeSheet"].Rows[intRow]["COMPANY_NO"].ToString());
                    strQry.AppendLine("," + DataSet.Tables["TimeSheet"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                    strQry.AppendLine("," + DataSet.Tables["TimeSheet"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(",'" + Convert.ToDateTime(DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_DATE"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine("," + DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_SEQ"].ToString());

                    if (DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_TIME_IN_MINUTES"] == System.DBNull.Value)
                    {
                        strQry.AppendLine(",Null");
                    }
                    else
                    {
                        strQry.AppendLine("," + DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_TIME_IN_MINUTES"].ToString());
                    }

                    if (DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_TIME_OUT_MINUTES"] == System.DBNull.Value)
                    {
                        strQry.AppendLine(",Null)");
                    }
                    else
                    {
                        strQry.AppendLine("," + DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_TIME_OUT_MINUTES"].ToString() + ")");
                    }
                }
                else
                {
                    if (DataSet.Tables["TimeSheet"].Rows[intRow].RowState == DataRowState.Modified)
                    {
                        strQry.Clear();

                        switch (parstrPayCategoryType)
                        {
                            case "W":

                                strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT");
                                break;

                            case "S":

                                strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT");
                                break;

                            case "T":

                                strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT");
                                break;
                        }

                        strQry.AppendLine(" SET ");

                        if (DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_TIME_IN_MINUTES"] == System.DBNull.Value)
                        {
                            strQry.AppendLine(" TIMESHEET_TIME_IN_MINUTES = Null");
                        }
                        else
                        {
                            strQry.AppendLine(" TIMESHEET_TIME_IN_MINUTES = " + DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_TIME_IN_MINUTES"].ToString());
                        }

                        if (DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_TIME_OUT_MINUTES"] == System.DBNull.Value)
                        {
                            strQry.AppendLine(",TIMESHEET_TIME_OUT_MINUTES = Null");
                        }
                        else
                        {
                            strQry.AppendLine(",TIMESHEET_TIME_OUT_MINUTES = " + DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_TIME_OUT_MINUTES"].ToString());
                        }

                        strQry.AppendLine(" WHERE COMPANY_NO = " + DataSet.Tables["TimeSheet"].Rows[intRow]["COMPANY_NO"].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables["TimeSheet"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["TimeSheet"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                        strQry.AppendLine(" AND TIMESHEET_DATE = '" + Convert.ToDateTime(DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_DATE"]).ToString("yyyy-MM-dd") + "'");
                        strQry.AppendLine(" AND TIMESHEET_SEQ = " + DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_SEQ"].ToString());
                    }
                    else
                    {
                        if (DataSet.Tables["TimeSheet"].Rows[intRow].RowState == DataRowState.Deleted)
                        {
                            strQry.Clear();

                            switch (parstrPayCategoryType)
                            {
                                case "W":

                                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT");
                                    break;

                                case "S":

                                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT");
                                    break;

                                case "T":

                                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT");
                                    break;
                            }

                            strQry.AppendLine(" WHERE COMPANY_NO = " + DataSet.Tables["TimeSheet"].Rows[intRow]["COMPANY_NO", DataRowVersion.Original].ToString());
                            strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables["TimeSheet"].Rows[intRow]["PAY_CATEGORY_NO", DataRowVersion.Original].ToString());
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["TimeSheet"].Rows[intRow]["EMPLOYEE_NO", DataRowVersion.Original].ToString());
                            strQry.AppendLine(" AND TIMESHEET_DATE = '" + Convert.ToDateTime(DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_DATE", DataRowVersion.Original]).ToString("yyyy-MM-dd") + "'");
                            strQry.AppendLine(" AND TIMESHEET_SEQ = " + DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_SEQ", DataRowVersion.Original].ToString());
                        }
                    }
                }

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            for (int intRow = 0; intRow < DataSet.Tables["Break"].Rows.Count; intRow++)
            {
                if (DataSet.Tables["Break"].Rows[intRow].RowState == DataRowState.Added)
                {
                    if (DataSetTemp.Tables["BreakTemp"] != null)
                    {
                        DataSetTemp.Tables["BreakTemp"].Clear();
                    }
                    
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" ISNULL(MAX(BREAK_SEQ),0) + 1 AS MAX_BREAK_SEQ");

                    switch (parstrPayCategoryType)
                    {
                        case "W":

                            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT");
                            break;

                        case "S":

                            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT");
                            break;

                        case "T":

                            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT");
                            break;
                    }
                        
                    strQry.AppendLine(" WHERE COMPANY_NO = " + DataSet.Tables["Break"].Rows[intRow]["COMPANY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Break"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                    strQry.AppendLine(" AND BREAK_DATE = '" + Convert.ToDateTime(DataSet.Tables["Break"].Rows[intRow]["BREAK_DATE"]).ToString("yyyy-MM-dd") + "'");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSetTemp, "BreakTemp");

                    strQry.Clear();

                    switch (parstrPayCategoryType)
                    {
                        case "W":

                            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT");
                            break;

                        case "S":

                            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT");
                            break;

                        case "T":

                            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT");
                            break;
                    }

                    //Added to be Passed back To Client Layer
                    DataSet.Tables["Break"].Rows[intRow]["BREAK_SEQ"] = Convert.ToInt32(DataSetTemp.Tables["BreakTemp"].Rows[0]["MAX_BREAK_SEQ"]);

                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",BREAK_DATE");
                    strQry.AppendLine(",BREAK_SEQ");
                    strQry.AppendLine(",BREAK_TIME_IN_MINUTES");
                    strQry.AppendLine(",BREAK_TIME_OUT_MINUTES)");
                    strQry.AppendLine(" VALUES ");
                    strQry.AppendLine("(" + DataSet.Tables["Break"].Rows[intRow]["COMPANY_NO"].ToString());
                    strQry.AppendLine("," + DataSet.Tables["Break"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                    strQry.AppendLine("," + DataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(",'" + Convert.ToDateTime(DataSet.Tables["Break"].Rows[intRow]["BREAK_DATE"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine("," + DataSet.Tables["Break"].Rows[intRow]["BREAK_SEQ"].ToString());

                    if (DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_IN_MINUTES"] == System.DBNull.Value)
                    {
                        strQry.AppendLine(",Null");
                    }
                    else
                    {
                        strQry.AppendLine("," + DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_IN_MINUTES"].ToString());
                    }

                    if (DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_OUT_MINUTES"] == System.DBNull.Value)
                    {
                        strQry.AppendLine(",Null)");
                    }
                    else
                    {
                        strQry.AppendLine("," + DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_OUT_MINUTES"].ToString() + ")");
                    }
                }
                else
                {
                    if (DataSet.Tables["Break"].Rows[intRow].RowState == DataRowState.Modified)
                    {
                        strQry.Clear();

                        switch (parstrPayCategoryType)
                        {
                            case "W":

                                strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT");
                                break;

                            case "S":

                                strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT");
                                break;

                            case "T":

                                strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT");
                                break;
                        }

                        strQry.AppendLine(" SET ");

                        if (DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_IN_MINUTES"] == System.DBNull.Value)
                        {
                            strQry.AppendLine(" BREAK_TIME_IN_MINUTES = Null");
                        }
                        else
                        {
                            strQry.AppendLine(" BREAK_TIME_IN_MINUTES = " + DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_IN_MINUTES"].ToString());
                        }

                        if (DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_OUT_MINUTES"] == System.DBNull.Value)
                        {
                            strQry.AppendLine(",BREAK_TIME_OUT_MINUTES = Null");
                        }
                        else
                        {
                            strQry.AppendLine(",BREAK_TIME_OUT_MINUTES = " + DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_OUT_MINUTES"].ToString());
                        }

                        strQry.AppendLine(" WHERE COMPANY_NO = " + DataSet.Tables["Break"].Rows[intRow]["COMPANY_NO"].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Break"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                        strQry.AppendLine(" AND BREAK_DATE = '" + Convert.ToDateTime(DataSet.Tables["Break"].Rows[intRow]["BREAK_DATE"]).ToString("yyyy-MM-dd") + "'");
                        strQry.AppendLine(" AND BREAK_SEQ = " + DataSet.Tables["Break"].Rows[intRow]["BREAK_SEQ"].ToString());
                    }
                    else
                    {
                        if (DataSet.Tables["Break"].Rows[intRow].RowState == DataRowState.Deleted)
                        {
                            strQry.Clear();

                            switch (parstrPayCategoryType)
                            {
                                case "W":

                                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT");
                                    break;

                                case "S":

                                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT");
                                    break;

                                case "T":

                                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT");
                                    break;
                            }

                            strQry.AppendLine(" WHERE COMPANY_NO = " + DataSet.Tables["Break"].Rows[intRow]["COMPANY_NO", DataRowVersion.Original].ToString());
                            strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_NO", DataRowVersion.Original].ToString());
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Break"].Rows[intRow]["EMPLOYEE_NO", DataRowVersion.Original].ToString());
                            strQry.AppendLine(" AND BREAK_DATE = '" + Convert.ToDateTime(DataSet.Tables["Break"].Rows[intRow]["BREAK_DATE", DataRowVersion.Original]).ToString("yyyy-MM-dd") + "'");
                            strQry.AppendLine(" AND BREAK_SEQ = " + DataSet.Tables["Break"].Rows[intRow]["BREAK_SEQ", DataRowVersion.Original].ToString());
                        }
                    }
                }

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }
  
            DataSetTemp.Dispose();
            DataSetTemp = null;
            
            DataSet.AcceptChanges();

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public void Delete_PayCategory_TimeSheet_Records(Int64 pvtint64CompanyNo, int pvtintPayCategoryNo, string parstrPayCategoryType, int parintCurrentUserNo, string parstrAccessInd)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(" DELETE ETC FROM InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
            }
            else
            {
                strQry.AppendLine(" DELETE ETC FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC ");
            }

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E");

            strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND ETC.COMPANY_NO = UEPCT.COMPANY_NO ");
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            }

            strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + pvtint64CompanyNo);
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + pvtintPayCategoryNo);
                       
            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(" DELETE ETC FROM InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT ETC");
            }
            else
            {
                strQry.AppendLine(" DELETE ETC FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ETC ");
            }

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E");

            strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND ETC.COMPANY_NO = UEPCT.COMPANY_NO ");
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            }

            strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + pvtint64CompanyNo);
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + pvtintPayCategoryNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
        }
    }
}
