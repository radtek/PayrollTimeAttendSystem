using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using InteractPayroll;

namespace InteractPayrollClient
{
    public class busTimeSheetBatchClient
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busTimeSheetBatchClient()
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
            
            //NB Still have to add User Filter - own companoies
            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" C.COMPANY_NO");
            strQry.AppendLine(",C.COMPANY_DESC");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.COMPANY C");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo.ToString());
                strQry.AppendLine(" AND C.COMPANY_NO = UEPCT.COMPANY_NO");
            }

            strQry.AppendLine(" ORDER BY");
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
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo.ToString());
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

            DataSet.Tables.Remove("Temp");

            DataSet.AcceptChanges();

            if (DataSet.Tables["Company"].Rows.Count > 0)
            {
                byte[] bytTempCompress = Get_New_Company_Records(Convert.ToInt64(DataSet.Tables["Company"].Rows[0]["COMPANY_NO"]), parintCurrentUserNo, parstrAccessInd,"");

                DataSet TempDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(bytTempCompress);
                DataSet.Merge(TempDataSet);
            }

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_New_Company_Records(Int64 parint64CompanyNo, int parintCurrentUserNo, string parstrAccessInd,string parstrFromUpdateInd)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();

            string strCurrentDate = DateTime.Now.ToString("yyyy-MM-dd");

            if (parstrFromUpdateInd != "Y")
            {
                strQry.Clear();
                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" PC.COMPANY_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",PC.PAY_CATEGORY_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
                strQry.AppendLine(",PC.NO_EDIT_IND");
                strQry.AppendLine(",PC.LAST_UPLOAD_DATETIME");

                strQry.AppendLine(",MIN(D.DAY_DATE) AS FROM_DATE");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY PC ");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON PC.COMPANY_NO = E.COMPANY_NO");
                strQry.AppendLine(" AND E.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
                strQry.AppendLine(" AND EPC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
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
                strQry.AppendLine(" ON D.DAY_DATE > ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrentDate + "'))");

                strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" PC.COMPANY_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",PC.PAY_CATEGORY_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
                strQry.AppendLine(",PC.NO_EDIT_IND");
                strQry.AppendLine(",PC.LAST_UPLOAD_DATETIME");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" PC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PayCategory");

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" E.COMPANY_NO ");
                strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                strQry.AppendLine(",E.EMPLOYEE_CODE");
                strQry.AppendLine(",E.EMPLOYEE_SURNAME");
                strQry.AppendLine(",E.EMPLOYEE_NAME");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrentDate + "')) AS EMPLOYEE_LAST_RUNDATE");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E ");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");

                if (parstrAccessInd != "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                    strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo.ToString());
                    strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                    strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
                }

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TEMP_TABLE.COMPANY_NO");
                strQry.AppendLine(",TEMP_TABLE.PAY_CATEGORY_NO");
                strQry.AppendLine(",TEMP_TABLE.PAY_CATEGORY_TYPE");

                strQry.AppendLine(",MIN(D.DAY_DATE) AS FROM_DATE");

                strQry.AppendLine(" FROM ");

                strQry.AppendLine("(SELECT ");
                strQry.AppendLine(" E.COMPANY_NO ");
                strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EPC.PAY_CATEGORY_NO");

                strQry.AppendLine(",MIN(ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrentDate + "'))) AS EMPLOYEE_LAST_RUNDATE");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");

                //Errol 2013-04-12
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC ");
                strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");

                if (parstrAccessInd != "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                    strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo.ToString());
                    strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                    strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
                }

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" E.COMPANY_NO ");
                strQry.AppendLine(",E.PAY_CATEGORY_TYPE");

                strQry.AppendLine(",EPC.PAY_CATEGORY_NO) AS TEMP_TABLE");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.DATES D");
                strQry.AppendLine(" ON D.DAY_DATE > TEMP_TABLE.EMPLOYEE_LAST_RUNDATE");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" TEMP_TABLE.COMPANY_NO");
                strQry.AppendLine(",TEMP_TABLE.PAY_CATEGORY_NO");
                strQry.AppendLine(",TEMP_TABLE.PAY_CATEGORY_TYPE");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "FromDate");
            }

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",ETC.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETC.EMPLOYEE_NO");
            strQry.AppendLine(",ETC.TIMESHEET_DATE");
            strQry.AppendLine(",ETC.TIMESHEET_SEQ");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_OUT_MINUTES");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN  InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");

            //Errol 2013-04-12
            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo.ToString());
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC ");
            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
            
            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",ETC.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETC.EMPLOYEE_NO");
            strQry.AppendLine(",ETC.TIMESHEET_DATE");
            strQry.AppendLine(",ETC.TIMESHEET_SEQ");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_OUT_MINUTES");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN  InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");

            //Errol 2013-04-12
            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo.ToString());
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC ");
            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
            
            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",ETC.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETC.EMPLOYEE_NO");
            strQry.AppendLine(",ETC.TIMESHEET_DATE");
            strQry.AppendLine(",ETC.TIMESHEET_SEQ");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_OUT_MINUTES");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN  InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");

            //Errol 2013-04-12
            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo.ToString());
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC ");
            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T'");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Timesheet");

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EBC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBC.EMPLOYEE_NO");

            strQry.AppendLine(",EBC.BREAK_DATE");
            strQry.AppendLine(",EBC.BREAK_SEQ");
            strQry.AppendLine(",EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",EBC.BREAK_TIME_OUT_MINUTES");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN  InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");

            //Errol 2013-04-12
            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo.ToString());
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT EBC ");
            strQry.AppendLine(" ON E.COMPANY_NO = EBC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EBC.EMPLOYEE_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = EBC.PAY_CATEGORY_NO ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EBC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBC.EMPLOYEE_NO");

            strQry.AppendLine(",EBC.BREAK_DATE");
            strQry.AppendLine(",EBC.BREAK_SEQ");
            strQry.AppendLine(",EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",EBC.BREAK_TIME_OUT_MINUTES");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN  InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");

            //Errol 2013-04-12
            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo.ToString());
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT EBC ");
            strQry.AppendLine(" ON E.COMPANY_NO = EBC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EBC.EMPLOYEE_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = EBC.PAY_CATEGORY_NO ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EBC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBC.EMPLOYEE_NO");

            strQry.AppendLine(",EBC.BREAK_DATE");
            strQry.AppendLine(",EBC.BREAK_SEQ");
            strQry.AppendLine(",EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",EBC.BREAK_TIME_OUT_MINUTES");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN  InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");

            //Errol 2013-04-12
            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo.ToString());
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT EBC ");
            
            strQry.AppendLine(" ON E.COMPANY_NO = EBC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EBC.EMPLOYEE_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = EBC.PAY_CATEGORY_NO ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T'");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Break");

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

            string strCurrentDate = DateTime.Now.ToString("yyyy-MM-dd");

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" PC.COMPANY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PC.NO_EDIT_IND");
            strQry.AppendLine(",PC.LAST_UPLOAD_DATETIME");

            strQry.AppendLine(",MIN(D.DAY_DATE) AS FROM_DATE");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY PC ");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON PC.COMPANY_NO = E.COMPANY_NO");
            strQry.AppendLine(" AND E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND EPC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.DATES D");
            strQry.AppendLine(" ON D.DAY_DATE > ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrentDate + "'))");

            strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PC.COMPANY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PC.NO_EDIT_IND");
            strQry.AppendLine(",PC.LAST_UPLOAD_DATETIME");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            
            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PayCategory");

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO ");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrentDate + "')) AS EMPLOYEE_LAST_RUNDATE");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E ");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parintCurrentUserNo.ToString());
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_TimeSheet_Records_For_Day(Int64 parint64CompanyNo, int parintPayCategoryNo, string parstrPayCategoryType, DateTime pardtDayDateTime, string parstrOption, int parintCurrentUserNo, string parstrAccessInd, string parstrUpdateInd, string parstrRecordtype)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();

            string strTableDef = "";

            if (parstrRecordtype == "T")
            {
                strTableDef = "TIMESHEET";
            }
            else
            {
                strTableDef = "BREAK";
            }

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.EMPLOYEE_NO");
            strQry.AppendLine(",ISNULL(CONVERT(INT,ETC." + strTableDef + "_SEQ),-1) AS " + strTableDef + "_SEQ ");
            strQry.AppendLine(",ETC." + strTableDef + "_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC." + strTableDef + "_TIME_OUT_MINUTES");
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E ");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + parintPayCategoryNo);
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ETC ");
            }
            else
            {
                if (parstrPayCategoryType == "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ETC ");
                }
                else
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_" + strTableDef + "_CURRENT ETC ");
                }
            }

            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + parintPayCategoryNo);
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND ETC." + strTableDef + "_DATE = '" + pardtDayDateTime.ToString("yyyy-MM-dd") + "'");

            if (parstrOption == "I")
            {
                strQry.AppendLine(" AND NOT ETC." + strTableDef + "_TIME_IN_MINUTES IS NULL");
            }
            else
            {
                if (parstrOption == "O")
                {
                    strQry.AppendLine(" AND NOT ETC." + strTableDef + "_TIME_OUT_MINUTES IS NULL");
                }
                else
                {
                    if (parstrOption == "B")
                    {
                        strQry.AppendLine(" AND ((ETC." + strTableDef + "_TIME_IN_MINUTES IS NULL");
                        strQry.AppendLine(" OR ETC." + strTableDef + "_TIME_OUT_MINUTES IS NULL)");

                        strQry.AppendLine(" OR (NOT ETC." + strTableDef + "_TIME_IN_MINUTES IS NULL");
                        strQry.AppendLine(" AND NOT ETC." + strTableDef + "_TIME_OUT_MINUTES IS NULL))");
                    }
                    else
                    {
                        //Rejected
                        strQry.AppendLine(" AND ETC.COMPANY_NO = -1 ");
                    }
                }
            }
            
            if (parstrAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_PAY_CATEGORY_DEPARTMENT UPCD");
                strQry.AppendLine(" ON EPC.COMPANY_NO = UPCD.COMPANY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UPCD.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UPCD.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND E.DEPARTMENT_NO = UPCD.DEPARTMENT_NO ");
                strQry.AppendLine(" AND UPCD.USER_NO = " + parintCurrentUserNo.ToString());

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL");

                if (parstrUpdateInd == "Y")
                {
                    //Make Sure That ResultSet is Empty
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = ''");
                }
                else
                {
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
                }

                strQry.AppendLine(" UNION ");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" E.EMPLOYEE_NO");
                strQry.AppendLine(",ISNULL(CONVERT(INT,ETC." + strTableDef + "_SEQ),-1) AS " + strTableDef + "_SEQ ");
                strQry.AppendLine(",ETC." + strTableDef + "_TIME_IN_MINUTES");
                strQry.AppendLine(",ETC." + strTableDef + "_TIME_OUT_MINUTES");
                
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E ");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                strQry.AppendLine(" AND EPC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + parintPayCategoryNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

                if (parstrPayCategoryType == "W")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ETC ");
                }
                else
                {
                    if (parstrPayCategoryType == "S")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ETC ");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_" + strTableDef + "_CURRENT ETC ");
                    }
                }

                strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO ");
                strQry.AppendLine(" AND ETC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + parintPayCategoryNo);
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND ETC." + strTableDef + "_DATE = '" + pardtDayDateTime.ToString("yyyy-MM-dd") + "'");

                if (parstrOption == "I")
                {
                    strQry.AppendLine(" AND NOT ETC." + strTableDef + "_TIME_IN_MINUTES IS NULL");
                }
                else
                {
                    if (parstrOption == "O")
                    {
                        strQry.AppendLine(" AND NOT ETC." + strTableDef + "_TIME_OUT_MINUTES IS NULL");
                    }
                    else
                    {
                        if (parstrOption == "B")
                        {
                            strQry.AppendLine(" AND ((ETC." + strTableDef + "_TIME_IN_MINUTES IS NULL");
                            strQry.AppendLine(" OR ETC." + strTableDef + "_TIME_OUT_MINUTES IS NULL)");

                            strQry.AppendLine(" OR (NOT ETC." + strTableDef + "_TIME_IN_MINUTES IS NULL");
                            strQry.AppendLine(" AND NOT ETC." + strTableDef + "_TIME_OUT_MINUTES IS NULL))");
                        }
                        else
                        {
                            //Rejected
                            strQry.AppendLine(" AND ETC.COMPANY_NO = -1 ");
                        }
                    }
                }

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE UE");
                strQry.AppendLine(" ON E.COMPANY_NO = UE.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UE.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UE.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND UE.USER_NO = " + parintCurrentUserNo.ToString());

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL");

                if (parstrUpdateInd == "Y")
                {
                    //Make Sure That ResultSet is Empty
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = ''");
                }
                else
                {
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
                }

                strQry.AppendLine(" UNION ");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" E.EMPLOYEE_NO");
                strQry.AppendLine(",ISNULL(CONVERT(INT,ETC." + strTableDef + "_SEQ),-1) AS " + strTableDef + "_SEQ ");
                strQry.AppendLine(",ETC." + strTableDef + "_TIME_IN_MINUTES");
                strQry.AppendLine(",ETC." + strTableDef + "_TIME_OUT_MINUTES");
                
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E ");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                strQry.AppendLine(" AND EPC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + parintPayCategoryNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

                if (parstrPayCategoryType == "W")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ETC ");
                }
                else
                {
                    if (parstrPayCategoryType == "S")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ETC ");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_" + strTableDef + "_CURRENT ETC ");
                    }
                }

                strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO ");
                strQry.AppendLine(" AND ETC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + parintPayCategoryNo);
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND ETC." + strTableDef + "_DATE = '" + pardtDayDateTime.ToString("yyyy-MM-dd") + "'");

                if (parstrOption == "I")
                {
                    strQry.AppendLine(" AND NOT ETC." + strTableDef + "_TIME_IN_MINUTES IS NULL");
                }
                else
                {
                    if (parstrOption == "O")
                    {
                        strQry.AppendLine(" AND NOT ETC." + strTableDef + "_TIME_OUT_MINUTES IS NULL");
                    }
                    else
                    {
                        if (parstrOption == "B")
                        {
                            strQry.AppendLine(" AND ((ETC." + strTableDef + "_TIME_IN_MINUTES IS NULL");
                            strQry.AppendLine(" OR ETC." + strTableDef + "_TIME_OUT_MINUTES IS NULL)");

                            strQry.AppendLine(" OR (NOT ETC." + strTableDef + "_TIME_IN_MINUTES IS NULL");
                            strQry.AppendLine(" AND NOT ETC." + strTableDef + "_TIME_OUT_MINUTES IS NULL))");
                        }
                        else
                        {
                            //Rejected
                            strQry.AppendLine(" AND ETC.COMPANY_NO = -1 ");
                        }
                    }
                }

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_PAY_CATEGORY UPC");
                strQry.AppendLine(" ON E.COMPANY_NO = UPC.COMPANY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UPC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND UPC.USER_NO = " + parintCurrentUserNo.ToString());
            }

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL");

            if (parstrUpdateInd == "Y")
            {
                //Make Sure That ResultSet is Empty
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = ''");
            }
            else
            {
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            }

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "EmployeeTimeSheetRejected");
            
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.EMPLOYEE_NO");
            strQry.AppendLine(",ISNULL(CONVERT(INT,ETC." + strTableDef + "_SEQ),-1) AS " + strTableDef + "_SEQ ");
            strQry.AppendLine(",ETC." + strTableDef + "_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC." + strTableDef + "_TIME_OUT_MINUTES");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E ");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + parintPayCategoryNo);
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            if (parstrPayCategoryType == "W")
            {
                if (parstrUpdateInd == "Y")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ETC ");
                }
                else
                {
                    if (parstrOption == "D")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ETC ");
                    }
                    else
                    {
                        strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ETC ");
                    }
                }
            }
            else
            {
                if (parstrPayCategoryType == "S")
                {
                    if (parstrUpdateInd == "Y")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ETC ");
                    }
                    else
                    {
                        if (parstrOption == "D")
                        {
                            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ETC ");
                        }
                        else
                        {
                            strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ETC ");
                        }
                    }
                }
                else
                {
                    if (parstrUpdateInd == "Y")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_" + strTableDef + "_CURRENT ETC ");
                    }
                    else
                    {
                        if (parstrOption == "D")
                        {
                            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_" + strTableDef + "_CURRENT ETC ");
                        }
                        else
                        {
                            strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_" + strTableDef + "_CURRENT ETC ");
                        }
                    }
                }
            }

            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + parintPayCategoryNo);
            strQry.AppendLine(" AND ETC." + strTableDef + "_DATE = '" + pardtDayDateTime.ToString("yyyy-MM-dd") + "'");

            if (parstrOption == "I")
            {
                if (parstrUpdateInd != "Y")
                {
                    strQry.AppendLine(" AND ETC." + strTableDef + "_TIME_IN_MINUTES IS NULL");
                }
            }
            else
            {
                if (parstrOption == "O")
                {
                    if (parstrUpdateInd != "Y")
                    {
                        strQry.AppendLine(" AND ETC." + strTableDef + "_TIME_OUT_MINUTES IS NULL");
                    }
                }
                else
                {
                    if (parstrOption == "B")
                    {
                        if (parstrUpdateInd != "Y")
                        {
                            //Not Possible - Empty Row
                            strQry.AppendLine(" AND ETC." + strTableDef + "_TIME_IN_MINUTES = 1500");
                        }
                    }
                }
            }

            if (parstrAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_PAY_CATEGORY_DEPARTMENT UPCD");
                strQry.AppendLine(" ON EPC.COMPANY_NO = UPCD.COMPANY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UPCD.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UPCD.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND E.DEPARTMENT_NO = UPCD.DEPARTMENT_NO ");
                strQry.AppendLine(" AND UPCD.USER_NO = " + parintCurrentUserNo.ToString());
                
                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

                strQry.AppendLine(" UNION ");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" E.EMPLOYEE_NO");
                strQry.AppendLine(",ISNULL(CONVERT(INT,ETC." + strTableDef + "_SEQ),-1) AS " + strTableDef + "_SEQ ");
                strQry.AppendLine(",ETC." + strTableDef + "_TIME_IN_MINUTES");
                strQry.AppendLine(",ETC." + strTableDef + "_TIME_OUT_MINUTES");
                
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E ");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                strQry.AppendLine(" AND EPC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + parintPayCategoryNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

                if (parstrPayCategoryType == "W")
                {
                    if (parstrUpdateInd == "Y")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ETC ");
                    }
                    else
                    {
                        if (parstrOption == "D")
                        {
                            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ETC ");
                        }
                        else
                        {
                            strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ETC ");
                        }
                    }
                }
                else
                {
                    if (parstrPayCategoryType == "S")
                    {
                        if (parstrUpdateInd == "Y")
                        {
                            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ETC ");
                        }
                        else
                        {
                            if (parstrOption == "D")
                            {
                                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ETC ");
                            }
                            else
                            {
                                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ETC ");
                            }
                        }
                    }
                    else
                    {
                        if (parstrUpdateInd == "Y")
                        {
                            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_" + strTableDef + "_CURRENT ETC ");
                        }
                        else
                        {
                            if (parstrOption == "D")
                            {
                                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_" + strTableDef + "_CURRENT ETC ");
                            }
                            else
                            {
                                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_" + strTableDef + "_CURRENT ETC ");
                            }
                        }
                    }
                }

                strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO ");
                strQry.AppendLine(" AND ETC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + parintPayCategoryNo);
                strQry.AppendLine(" AND ETC." + strTableDef + "_DATE = '" + pardtDayDateTime.ToString("yyyy-MM-dd") + "'");

                if (parstrOption == "I")
                {
                    if (parstrUpdateInd != "Y")
                    {
                        strQry.AppendLine(" AND ETC." + strTableDef + "_TIME_IN_MINUTES IS NULL");
                    }
                }
                else
                {
                    if (parstrOption == "O")
                    {
                        if (parstrUpdateInd != "Y")
                        {
                            strQry.AppendLine(" AND ETC." + strTableDef + "_TIME_OUT_MINUTES IS NULL");
                        }
                    }
                    else
                    {
                        if (parstrOption == "B")
                        {
                            if (parstrUpdateInd != "Y")
                            {
                                //Not Possible - Empty Row
                                strQry.AppendLine(" AND ETC." + strTableDef + "_TIME_IN_MINUTES = 1500");
                            }
                        }
                    }
                }

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE UE");
                strQry.AppendLine(" ON E.COMPANY_NO = UE.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UE.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UE.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND UE.USER_NO = " + parintCurrentUserNo.ToString());

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

                strQry.AppendLine(" UNION ");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" E.EMPLOYEE_NO");
                strQry.AppendLine(",ISNULL(CONVERT(INT,ETC." + strTableDef + "_SEQ),-1) AS " + strTableDef + "_SEQ ");
                strQry.AppendLine(",ETC." + strTableDef + "_TIME_IN_MINUTES");
                strQry.AppendLine(",ETC." + strTableDef + "_TIME_OUT_MINUTES");
                
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E ");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                strQry.AppendLine(" AND EPC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + parintPayCategoryNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

                if (parstrPayCategoryType == "W")
                {
                    if (parstrUpdateInd == "Y")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ETC ");
                    }
                    else
                    {
                        if (parstrOption == "D")
                        {
                            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ETC ");
                        }
                        else
                        {
                            strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ETC ");
                        }
                    }
                }
                else
                {
                    if (parstrPayCategoryType == "S")
                    {
                        if (parstrUpdateInd == "Y")
                        {
                            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ETC ");
                        }
                        else
                        {
                            if (parstrOption == "D")
                            {
                                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ETC ");
                            }
                            else
                            {
                                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ETC ");
                            }
                        }
                    }
                    else
                    {
                        if (parstrUpdateInd == "Y")
                        {
                            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_" + strTableDef + "_CURRENT ETC ");
                        }
                        else
                        {
                            if (parstrOption == "D")
                            {
                                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_" + strTableDef + "_CURRENT ETC ");
                            }
                            else
                            {
                                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_" + strTableDef + "_CURRENT ETC ");
                            }
                        }
                    }
                }

                strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO ");
                strQry.AppendLine(" AND ETC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + parintPayCategoryNo);
                strQry.AppendLine(" AND ETC." + strTableDef + "_DATE = '" + pardtDayDateTime.ToString("yyyy-MM-dd") + "'");

                if (parstrOption == "I")
                {
                    if (parstrUpdateInd != "Y")
                    {
                        strQry.AppendLine(" AND ETC." + strTableDef + "_TIME_IN_MINUTES IS NULL");
                    }
                }
                else
                {
                    if (parstrOption == "O")
                    {
                        if (parstrUpdateInd != "Y")
                        {
                            strQry.AppendLine(" AND ETC." + strTableDef + "_TIME_OUT_MINUTES IS NULL");
                        }
                    }
                    else
                    {
                        if (parstrOption == "B")
                        {
                            if (parstrUpdateInd != "Y")
                            {
                                //Not Possible - Empty Row
                                strQry.AppendLine(" AND ETC." + strTableDef + "_TIME_IN_MINUTES = 1500");
                            }
                        }
                    }
                }

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_PAY_CATEGORY UPC");
                strQry.AppendLine(" ON E.COMPANY_NO = UPC.COMPANY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UPC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND UPC.USER_NO = " + parintCurrentUserNo.ToString());
            }

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "EmployeeTimeSheet");
            
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        //2013-07-11
        public byte[] Update_New_Records(Int64 parint64CompanyNo, int parintCurrentUserNo, string parstrAccessInd,
            int parintPayCategoryNo, string parstrPayCategoryType,
            int parintTimeInMinutes, int parintTimeOutMinutes, string parstrArrayDateTime, string parstrOption,
            string parstrArrayEmployeeNos, string parstrArrayEmployeeRecordNos, string parstrUpdateInd, string parstrRecordtype)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            string[] strDayDateTime = parstrArrayDateTime.Split('|');
            string[] parintEmployeeNo = parstrArrayEmployeeNos.Split('|');
            string[] parintEmployeeRecordNo = parstrArrayEmployeeRecordNos.Split('|');

            string strTableDef = "";

            if (parstrRecordtype == "T")
            {
                strTableDef = "TIMESHEET";

            }
            else
            {
                strTableDef = "BREAK";
            }

            StringBuilder strQry = new StringBuilder();

            for (int intCount = 0; intCount < strDayDateTime.Length; intCount++)
            {
                for (int intRow = 0; intRow < parintEmployeeNo.Length; intRow++)
                {
                    if (parstrOption == "B")
                    {
                        strQry.Clear();

                        if (parstrUpdateInd == "Y")
                        {
                            if (parstrPayCategoryType == "W")
                            {
                                strQry.AppendLine(" UPDATE EMPLOYEE_" + strTableDef + "_CURRENT ");
                                strQry.AppendLine(" SET ");
                                strQry.AppendLine(strTableDef + "_TIME_IN_MINUTES = " + parintTimeInMinutes);
                                strQry.AppendLine("," + strTableDef + "_TIME_OUT_MINUTES = " + parintTimeOutMinutes);
                            }
                            else
                            {
                                strQry.AppendLine(" UPDATE EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ");
                                strQry.AppendLine(" SET ");
                                strQry.AppendLine(strTableDef + "_TIME_IN_MINUTES = " + parintTimeInMinutes);
                                strQry.AppendLine("," + strTableDef + "_TIME_OUT_MINUTES = " + parintTimeOutMinutes);
                            }
                        }
                        else
                        {

                            if (parstrPayCategoryType == "W")
                            {
                                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ");
                            }
                            else
                            {
                                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ");

                            }

                            strQry.AppendLine("(COMPANY_NO");
                            strQry.AppendLine(",EMPLOYEE_NO");
                            strQry.AppendLine(",PAY_CATEGORY_NO");
                            strQry.AppendLine("," + strTableDef + "_DATE");
                            strQry.AppendLine("," + strTableDef + "_SEQ");
                            strQry.AppendLine("," + strTableDef + "_TIME_IN_MINUTES");
                            strQry.AppendLine("," + strTableDef + "_TIME_OUT_MINUTES)");

                            strQry.AppendLine(" SELECT ");
                            strQry.AppendLine(parint64CompanyNo.ToString());
                            strQry.AppendLine("," + parintEmployeeNo[intRow]);
                            strQry.AppendLine("," + parintPayCategoryNo);
                            strQry.AppendLine(",'" + strDayDateTime[intCount].ToString() + "'");
                            strQry.AppendLine(",ISNULL(MAX(" + strTableDef + "_SEQ),0) + 1");
                            strQry.AppendLine("," + parintTimeInMinutes);
                            strQry.AppendLine("," + parintTimeOutMinutes);

                            strQry.AppendLine(" FROM ");

                            if (parstrPayCategoryType == "W")
                            {
                                strQry.AppendLine(" InteractPayrollClient.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ");
                            }
                            else
                            {
                                strQry.AppendLine(" InteractPayrollClient.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ");
                            }
                        }

                        strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo[intRow]);
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
                        strQry.AppendLine(" AND " + strTableDef + "_DATE = '" + strDayDateTime[intCount].ToString() + "'");

                        if (parstrUpdateInd == "Y")
                        {
                            strQry.AppendLine(" AND " + strTableDef + "_SEQ = " + parintEmployeeRecordNo[intRow]);
                        }
                    }
                    else
                    {
                        if (parintEmployeeRecordNo[intRow] == "-1")
                        {
                            //Insert
                            strQry.Clear();

                            if (parstrPayCategoryType == "W")
                            {
                                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ");
                            }
                            else
                            {
                                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ");
                            }

                            strQry.AppendLine("(COMPANY_NO");
                            strQry.AppendLine(",EMPLOYEE_NO");
                            strQry.AppendLine(",PAY_CATEGORY_NO");
                            strQry.AppendLine("," + strTableDef + "_DATE");
                            strQry.AppendLine("," + strTableDef + "_SEQ");

                            if (parstrOption == "I")
                            {
                                strQry.AppendLine("," + strTableDef + "_TIME_IN_MINUTES)");
                            }
                            else
                            {
                                strQry.AppendLine("," + strTableDef + "_TIME_OUT_MINUTES)");
                            }

                            strQry.AppendLine(" SELECT ");
                            strQry.AppendLine(parint64CompanyNo.ToString());
                            strQry.AppendLine("," + parintEmployeeNo[intRow]);
                            strQry.AppendLine("," + parintPayCategoryNo);
                            strQry.AppendLine(",'" + strDayDateTime[intCount].ToString() + "'");
                            strQry.AppendLine(",ISNULL(MAX(" + strTableDef + "_SEQ),0) + 1");

                            if (parstrOption == "I")
                            {
                                strQry.AppendLine("," + parintTimeInMinutes);
                            }
                            else
                            {
                                strQry.AppendLine("," + parintTimeOutMinutes);
                            }

                            strQry.AppendLine(" FROM ");

                            if (parstrPayCategoryType == "W")
                            {
                                strQry.AppendLine(" InteractPayrollClient.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ");
                            }
                            else
                            {
                                strQry.AppendLine(" InteractPayrollClient.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ");
                            }

                            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo[intRow]);
                            strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
                            strQry.AppendLine(" AND " + strTableDef + "_DATE = '" + strDayDateTime[intCount].ToString() + "'");
                        }
                        else
                        {
                            if (parstrOption == "I")
                            {
                                strQry.Clear();

                                if (parstrPayCategoryType == "W")
                                {
                                    strQry.AppendLine(" UPDATE EMPLOYEE_" + strTableDef + "_CURRENT ");
                                }
                                else
                                {
                                    strQry.AppendLine(" UPDATE EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ");
                                }

                                strQry.AppendLine(" SET " + strTableDef + "_TIME_IN_MINUTES = " + parintTimeInMinutes);
                                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                                strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo[intRow]);
                                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
                                strQry.AppendLine(" AND " + strTableDef + "_DATE = '" + strDayDateTime[intCount].ToString() + "'");
                                strQry.AppendLine(" AND " + strTableDef + "_SEQ = " + parintEmployeeRecordNo[intRow]);
                            }
                            else
                            {
                                strQry.Clear();

                                //Out
                                if (parstrOption == "O")
                                {
                                    if (parstrPayCategoryType == "W")
                                    {
                                        strQry.AppendLine(" UPDATE EMPLOYEE_" + strTableDef + "_CURRENT ");
                                    }
                                    else
                                    {
                                        strQry.AppendLine(" UPDATE EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ");
                                    }

                                    strQry.AppendLine(" SET " + strTableDef + "_TIME_OUT_MINUTES = " + parintTimeOutMinutes);
                                }
                                else
                                {
                                    if (parstrPayCategoryType == "W")
                                    {
                                        strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ");
                                    }
                                    else
                                    {
                                        strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ");
                                    }
                                }

                                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                                strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo[intRow]);
                                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
                                strQry.AppendLine(" AND " + strTableDef + "_DATE = '" + strDayDateTime[intCount].ToString() + "'");
                                strQry.AppendLine(" AND " + strTableDef + "_SEQ = " + parintEmployeeRecordNo[intRow]);
                            }
                        }
                    }

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }
            }

            byte[] bytCompress = Get_New_Company_Records(parint64CompanyNo, parintCurrentUserNo, parstrAccessInd,"Y");
            
            return bytCompress;
        }

        public void Update_Records(Int64 parint64CompanyNo, int parintPayCategoryNo, string parstrPayCategoryType,
            int parintTimeInMinutes, int parintTimeOutMinutes, string parstrArrayDateTime, string parstrOption,
            string parstrArrayEmployeeNos, string parstrArrayEmployeeRecordNos, string parstrUpdateInd, string parstrRecordtype)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            string[] strDayDateTime = parstrArrayDateTime.Split('|');
            string[] parintEmployeeNo = parstrArrayEmployeeNos.Split('|');
            string[] parintEmployeeRecordNo = parstrArrayEmployeeRecordNos.Split('|');

            string strTableDef = "";

            if (parstrRecordtype == "T")
            {
                strTableDef = "TIMESHEET";

            }
            else
            {
                strTableDef = "BREAK";
            }
            
            StringBuilder strQry = new StringBuilder();
                    
            for (int intCount = 0; intCount < strDayDateTime.Length; intCount++)
            {
                for (int intRow = 0; intRow < parintEmployeeNo.Length; intRow++)
                {
                    if (parstrOption == "B")
                    {
                        strQry.Clear();

                        if (parstrUpdateInd == "Y")
                        {
                            if (parstrPayCategoryType == "W")
                            {
                                strQry.AppendLine(" UPDATE EMPLOYEE_" + strTableDef + "_CURRENT ");
                                strQry.AppendLine(" SET ");
                                strQry.AppendLine(strTableDef + "_TIME_IN_MINUTES = " + parintTimeInMinutes);
                                strQry.AppendLine("," + strTableDef + "_TIME_OUT_MINUTES = " + parintTimeOutMinutes);
                            }
                            else
                            {
                                strQry.AppendLine(" UPDATE EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ");
                                strQry.AppendLine(" SET ");
                                strQry.AppendLine(strTableDef + "_TIME_IN_MINUTES = " + parintTimeInMinutes);
                                strQry.AppendLine("," + strTableDef + "_TIME_OUT_MINUTES = " + parintTimeOutMinutes);
                            }
                        }
                        else
                        {

                            if (parstrPayCategoryType == "W")
                            {
                                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ");
                            }
                            else
                            {
                                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ");

                            }

                            strQry.AppendLine("(COMPANY_NO");
                            strQry.AppendLine(",EMPLOYEE_NO");
                            strQry.AppendLine(",PAY_CATEGORY_NO");
                            strQry.AppendLine("," + strTableDef + "_DATE");
                            strQry.AppendLine("," + strTableDef + "_SEQ");
                            strQry.AppendLine("," + strTableDef + "_TIME_IN_MINUTES");
                            strQry.AppendLine("," + strTableDef + "_TIME_OUT_MINUTES)");

                            strQry.AppendLine(" SELECT ");
                            strQry.AppendLine(parint64CompanyNo.ToString());
                            strQry.AppendLine("," + parintEmployeeNo[intRow]);
                            strQry.AppendLine("," + parintPayCategoryNo);
                            strQry.AppendLine(",'" + strDayDateTime[intCount].ToString() + "'");
                            strQry.AppendLine(",ISNULL(MAX(" + strTableDef + "_SEQ),0) + 1");
                            strQry.AppendLine("," + parintTimeInMinutes);
                            strQry.AppendLine("," + parintTimeOutMinutes);

                            strQry.AppendLine(" FROM ");

                            if (parstrPayCategoryType == "W")
                            {
                                strQry.AppendLine(" InteractPayrollClient.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ");
                            }
                            else
                            {
                                strQry.AppendLine(" InteractPayrollClient.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ");
                            }
                        }

                        strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo[intRow]);
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
                        strQry.AppendLine(" AND " + strTableDef + "_DATE = '" + strDayDateTime[intCount].ToString() + "'");

                        if (parstrUpdateInd == "Y")
                        {
                            strQry.AppendLine(" AND " + strTableDef + "_SEQ = " + parintEmployeeRecordNo[intRow]);
                        }
                    }
                    else
                    {
                        if (parintEmployeeRecordNo[intRow] == "-1")
                        {
                            //Insert
                            strQry.Clear();

                            if (parstrPayCategoryType == "W")
                            {
                                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ");
                            }
                            else
                            {
                                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ");
                            }

                            strQry.AppendLine("(COMPANY_NO");
                            strQry.AppendLine(",EMPLOYEE_NO");
                            strQry.AppendLine(",PAY_CATEGORY_NO");
                            strQry.AppendLine("," + strTableDef + "_DATE");
                            strQry.AppendLine("," + strTableDef + "_SEQ");

                            if (parstrOption == "I")
                            {
                                strQry.AppendLine("," + strTableDef + "_TIME_IN_MINUTES)");
                            }
                            else
                            {
                                strQry.AppendLine("," + strTableDef + "_TIME_OUT_MINUTES)");
                            }

                            strQry.AppendLine(" SELECT ");
                            strQry.AppendLine(parint64CompanyNo.ToString());
                            strQry.AppendLine("," + parintEmployeeNo[intRow]);
                            strQry.AppendLine("," + parintPayCategoryNo);
                            strQry.AppendLine(",'" + strDayDateTime[intCount].ToString() + "'");
                            strQry.AppendLine(",ISNULL(MAX(" + strTableDef + "_SEQ),0) + 1");

                            if (parstrOption == "I")
                            {
                                strQry.AppendLine("," + parintTimeInMinutes);
                            }
                            else
                            {
                                strQry.AppendLine("," + parintTimeOutMinutes);
                            }

                            strQry.AppendLine(" FROM ");

                            if (parstrPayCategoryType == "W")
                            {
                                strQry.AppendLine(" InteractPayrollClient.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ");
                            }
                            else
                            {
                                strQry.AppendLine(" InteractPayrollClient.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ");
                            }

                            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo[intRow]);
                            strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
                            strQry.AppendLine(" AND " + strTableDef + "_DATE = '" + strDayDateTime[intCount].ToString() + "'");
                        }
                        else
                        {
                            if (parstrOption == "I")
                            {
                                strQry.Clear();

                                if (parstrPayCategoryType == "W")
                                {
                                    strQry.AppendLine(" UPDATE EMPLOYEE_" + strTableDef + "_CURRENT ");
                                }
                                else
                                {
                                    strQry.AppendLine(" UPDATE EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ");
                                }

                                strQry.AppendLine(" SET " + strTableDef + "_TIME_IN_MINUTES = " + parintTimeInMinutes);
                                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                                strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo[intRow]);
                                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
                                strQry.AppendLine(" AND " + strTableDef + "_DATE = '" + strDayDateTime[intCount].ToString() + "'");
                                strQry.AppendLine(" AND " + strTableDef + "_SEQ = " + parintEmployeeRecordNo[intRow]);
                            }
                            else
                            {
                                strQry.Clear();

                                //Out
                                if (parstrOption == "O")
                                {
                                    if (parstrPayCategoryType == "W")
                                    {
                                        strQry.AppendLine(" UPDATE EMPLOYEE_" + strTableDef + "_CURRENT ");
                                    }
                                    else
                                    {
                                        strQry.AppendLine(" UPDATE EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ");
                                    }

                                    strQry.AppendLine(" SET " + strTableDef + "_TIME_OUT_MINUTES = " + parintTimeOutMinutes);
                                }
                                else
                                {
                                    if (parstrPayCategoryType == "W")
                                    {
                                        strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_" + strTableDef + "_CURRENT ");
                                    }
                                    else
                                    {
                                        strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT ");
                                    }
                                }

                                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                                strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo[intRow]);
                                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
                                strQry.AppendLine(" AND " + strTableDef + "_DATE = '" + strDayDateTime[intCount].ToString() + "'");
                                strQry.AppendLine(" AND " + strTableDef + "_SEQ = " + parintEmployeeRecordNo[intRow]);
                            }
                        }
                    }

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }
            }
        }

        public void Delete_Records(Int64 parint64CompanyNo, int parintPayCategoryNo, string parstrPayCategoryType, string parstrArrayDayDateTimes, string parstrRecordtype)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            string[] pardtDayDateTime = parstrArrayDayDateTimes.Split('|');
            StringBuilder strQry = new StringBuilder();

            string strTableDef = "";

            if (parstrRecordtype == "T")
            {
                strTableDef = "TIMESHEET";

            }
            else
            {
                strTableDef = "BREAK";
            }

            for (int intCount = 0; intCount < pardtDayDateTime.Length; intCount++)
            {
                strQry.Clear();

                if (parstrPayCategoryType == "W")
                {
                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_" + strTableDef + "_CURRENT");
                }
                else
                {
                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_" + strTableDef + "_CURRENT");
                }

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
                strQry.AppendLine(" AND " + strTableDef + "_DATE = '" + pardtDayDateTime[intCount].ToString() + "'");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }
        }
    }
}
