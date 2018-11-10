using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace InteractPayroll
{
    public class busLoadNewDemoRun
    {
        clsDBConnectionObjects clsDBConnectionObjects;
        public busLoadNewDemoRun()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parintUserNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
            
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" CL.COMPANY_DESC ");

            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_COMPANY_TO_LOAD UCTL ");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.COMPANY_LINK CL ");
            strQry.AppendLine(" ON UCTL.COMPANY_NO = CL.COMPANY_NO ");
            
            strQry.AppendLine(" WHERE UCTL.USER_NO = " + parintUserNo.ToString());
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyName", -1);
            
            strQry.Clear();
            strQry.AppendLine(" SELECT TOP 3");
            strQry.AppendLine(" PAY_PERIOD_DATE ");
            
            strQry.AppendLine(" FROM InteractPayroll_00013.dbo.PAY_CATEGORY_PERIOD_HISTORY ");
            strQry.AppendLine(" WHERE COMPANY_NO = 13 ");
            
            strQry.AppendLine(" AND PAY_PERIOD_DATE > '2017-11-01' ");
            
            strQry.AppendLine(" AND RUN_TYPE = 'P' ");
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W' ");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PAY_PERIOD_DATE ");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PAY_PERIOD_DATE DESC");
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayDates", -1);
            
            DataSet.AcceptChanges();

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public void Load_New_Run(Int64 parintUserNo,string parstrDate)
        {

            StringBuilder strQry = new StringBuilder();
            StringBuilder strQryCol = new StringBuilder();
            DataSet DataSet = new DataSet();
            
            DateTime EndOfMonthDateTime = DateTime.ParseExact(parstrDate.Substring(0,8) + "01", "yyyy-MM-dd", null).AddMonths(1).AddDays(-1);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO ");

            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_COMPANY_TO_LOAD ");

            strQry.AppendLine(" WHERE USER_NO = " + parintUserNo.ToString());

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "UserCompanyNo", -1);

            string strTableNoToLoad = "InteractPayroll_" + Convert.ToInt32(DataSet.Tables["UserCompanyNo"].Rows[0]["COMPANY_NO"]).ToString("00000");
            
            strQry.Clear();

            strQry.AppendLine(" SELECT TABLE_NAME");
            strQry.AppendLine(" FROM " + strTableNoToLoad + ".INFORMATION_SCHEMA.TABLES ");

            strQry.AppendLine(" WHERE TABLE_NAME LIKE '%CURRENT%'");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CurrentTables", -1);

            for (int intRow = 0; intRow < DataSet.Tables["CurrentTables"].Rows.Count; intRow++)
            {
                strQry.Clear();

                strQry.AppendLine(" DELETE FROM " + strTableNoToLoad + ".dbo." + DataSet.Tables["CurrentTables"].Rows[intRow]["TABLE_NAME"].ToString());

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            }

            strQry.Clear();

            strQry.AppendLine(" INSERT INTO " + strTableNoToLoad + ".dbo.LEAVE_CURRENT");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",LEAVE_DESC");
            strQry.AppendLine(",PROCESS_NO");
            strQry.AppendLine(",LEAVE_FROM_DATE");
            strQry.AppendLine(",LEAVE_TO_DATE");
            strQry.AppendLine(",LEAVE_OPTION");
            strQry.AppendLine(",LEAVE_DAYS_DECIMAL");
            strQry.AppendLine(",LEAVE_HOURS_DECIMAL)");

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" 18");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",LEAVE_DESC");
            strQry.AppendLine(",PROCESS_NO");
            strQry.AppendLine(",LEAVE_FROM_DATE");
            strQry.AppendLine(",LEAVE_TO_DATE");
            strQry.AppendLine(",LEAVE_OPTION");
            strQry.AppendLine(",LEAVE_DAYS_DECIMAL");
            strQry.AppendLine(",LEAVE_HOURS_DECIMAL");

            strQry.AppendLine(" FROM InteractPayroll_00013.dbo.LEAVE_HISTORY");

            strQry.AppendLine(" WHERE COMPANY_NO = 13");
            strQry.AppendLine(" AND (PAY_PERIOD_DATE = '" + parstrDate + "'");
            strQry.AppendLine(" OR PAY_PERIOD_DATE = '" + EndOfMonthDateTime.ToString("yyyy-MM-dd") + "')");
            strQry.AppendLine(" AND PROCESS_NO = 0");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            //2018-02-03
            strQry.Clear();

            strQry.AppendLine(" INSERT INTO " + strTableNoToLoad + ".dbo.EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",TIMESHEET_DATE");
            strQry.AppendLine(",DAY_NO");
            strQry.AppendLine(",DAY_PAID_MINUTES");
            strQry.AppendLine(",BREAK_ACCUM_MINUTES");
            strQry.AppendLine(",INDICATOR");
            strQry.AppendLine(",BREAK_INDICATOR)");

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" 18");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",TIMESHEET_DATE");
            strQry.AppendLine(",DAY_NO");
            strQry.AppendLine(",DAY_PAID_MINUTES");
            strQry.AppendLine(",BREAK_ACCUM_MINUTES");
            strQry.AppendLine(",INDICATOR");
            strQry.AppendLine(",BREAK_INDICATOR");
          
            strQry.AppendLine(" FROM InteractPayroll_00013.dbo.EMPLOYEE_TIMESHEET_BREAK_DAY_HISTORY");

            strQry.AppendLine(" WHERE COMPANY_NO = 13");
            strQry.AppendLine(" AND PAY_PERIOD_DATE = '" + parstrDate + "'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            //Disable Trigger
            strQry.Clear();

            strQry.AppendLine(" ALTER TABLE " + strTableNoToLoad + ".dbo.EMPLOYEE_BREAK_CURRENT ");
            strQry.AppendLine(" DISABLE TRIGGER tgr_EMPLOYEE_BREAK_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table ");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            
            strQry.Clear();

            strQry.AppendLine(" INSERT INTO " + strTableNoToLoad + ".dbo.EMPLOYEE_BREAK_CURRENT");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",BREAK_DATE");
            strQry.AppendLine(",BREAK_SEQ");
            strQry.AppendLine(",BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine(",CLOCKED_TIME_IN_MINUTES");
            strQry.AppendLine(",CLOCKED_TIME_OUT_MINUTES");
            strQry.AppendLine(",INCLUDED_IN_RUN_IND");
            strQry.AppendLine(",INDICATOR");
            strQry.AppendLine(",BREAK_ACCUM_MINUTES");
            strQry.AppendLine(",USER_NO_TIME_IN");
            strQry.AppendLine(",USER_NO_TIME_OUT)");

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" 18");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",BREAK_DATE");
            strQry.AppendLine(",BREAK_SEQ");
            strQry.AppendLine(",BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine(",CLOCKED_TIME_IN_MINUTES");
            strQry.AppendLine(",CLOCKED_TIME_OUT_MINUTES");
            strQry.AppendLine(",NULL");
            strQry.AppendLine(",INDICATOR");
            strQry.AppendLine(",BREAK_ACCUM_MINUTES");
            strQry.AppendLine(",USER_NO_TIME_IN");
            strQry.AppendLine(",USER_NO_TIME_OUT");

            strQry.AppendLine(" FROM InteractPayroll_00013.dbo.EMPLOYEE_BREAK_HISTORY");

            strQry.AppendLine(" WHERE COMPANY_NO = 13");
            strQry.AppendLine(" AND PAY_PERIOD_DATE = '" + parstrDate + "'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            //Enable Trigger
            strQry.Clear();

            strQry.AppendLine(" ALTER TABLE " + strTableNoToLoad + ".dbo.EMPLOYEE_BREAK_CURRENT ");
            strQry.AppendLine(" ENABLE TRIGGER tgr_EMPLOYEE_BREAK_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table ");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            //Disable Trigger
            strQry.Clear();

            strQry.AppendLine(" ALTER TABLE " + strTableNoToLoad + ".dbo.EMPLOYEE_TIMESHEET_CURRENT ");
            strQry.AppendLine(" DISABLE TRIGGER tgr_EMPLOYEE_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table ");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            strQry.Clear();

            strQry.AppendLine(" INSERT INTO " + strTableNoToLoad + ".dbo.EMPLOYEE_TIMESHEET_CURRENT");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",TIMESHEET_DATE");
            strQry.AppendLine(",TIMESHEET_SEQ");
            strQry.AppendLine(",TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine(",TIMESHEET_TIME_OUT_MINUTES");
            strQry.AppendLine(",CLOCKED_TIME_IN_MINUTES");
            strQry.AppendLine(",CLOCKED_TIME_OUT_MINUTES");
            strQry.AppendLine(",INCLUDED_IN_RUN_IND");
            strQry.AppendLine(",INDICATOR");
            strQry.AppendLine(",TIMESHEET_ACCUM_MINUTES");
            strQry.AppendLine(",USER_NO_TIME_IN");
            strQry.AppendLine(",USER_NO_TIME_OUT)");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" 18");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",TIMESHEET_DATE");
            strQry.AppendLine(",TIMESHEET_SEQ");
            strQry.AppendLine(",TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine(",TIMESHEET_TIME_OUT_MINUTES");
            strQry.AppendLine(",CLOCKED_TIME_IN_MINUTES");
            strQry.AppendLine(",CLOCKED_TIME_OUT_MINUTES");
            strQry.AppendLine(",NULL");
            strQry.AppendLine(",INDICATOR");
            strQry.AppendLine(",TIMESHEET_ACCUM_MINUTES");
            strQry.AppendLine(",USER_NO_TIME_IN");
            strQry.AppendLine(",USER_NO_TIME_OUT");

            strQry.AppendLine(" FROM InteractPayroll_00013.dbo.EMPLOYEE_TIMESHEET_HISTORY");

            strQry.AppendLine(" WHERE COMPANY_NO = 13");
            strQry.AppendLine(" AND PAY_PERIOD_DATE = '" + parstrDate + "'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            //Enable Trigger
            strQry.Clear();

            strQry.AppendLine(" ALTER TABLE " + strTableNoToLoad + ".dbo.EMPLOYEE_TIMESHEET_CURRENT ");
            strQry.AppendLine(" ENABLE TRIGGER tgr_EMPLOYEE_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table ");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            //Public Holiday
            strQry.Clear();

            strQry.AppendLine(" DELETE FROM " + strTableNoToLoad + ".dbo.PUBLIC_HOLIDAY");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            
            strQry.Clear();

            strQry.AppendLine(" SET IDENTITY_INSERT " + strTableNoToLoad + ".dbo.PUBLIC_HOLIDAY ON");

            strQry.AppendLine(" INSERT INTO " + strTableNoToLoad + ".dbo.PUBLIC_HOLIDAY");

            strQry.AppendLine("(PUBLIC_HOLIDAY_NO");
            strQry.AppendLine(",PUBLIC_HOLIDAY_DESC");
            strQry.AppendLine(",PUBLIC_HOLIDAY_DATE)");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PUBLIC_HOLIDAY_NO");
            strQry.AppendLine(",PUBLIC_HOLIDAY_DESC");
            strQry.AppendLine(",PUBLIC_HOLIDAY_DATE ");

            strQry.AppendLine(" FROM InteractPayroll_00013.dbo.PUBLIC_HOLIDAY");
            
            strQry.AppendLine(" SET IDENTITY_INSERT " + strTableNoToLoad + ".dbo.PUBLIC_HOLIDAY OFF");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            strQry.Clear();

            strQry.AppendLine(" SELECT TABLE_NAME");
            strQry.AppendLine(" FROM " + strTableNoToLoad + ".INFORMATION_SCHEMA.TABLES ");

            strQry.AppendLine(" WHERE TABLE_NAME IN (");
            strQry.AppendLine(" 'PUBLIC_HOLIDAY_HISTORY'");
            strQry.AppendLine(",'LEAVE_HISTORY'");
            strQry.AppendLine(",'EMPLOYEE_BREAK_HISTORY'");
            strQry.AppendLine(",'OCCUPATION_HISTORY'");
            strQry.AppendLine(",'EMPLOYEE_INFO_HISTORY'");
            strQry.AppendLine(",'EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE_HISTORY'");
            strQry.AppendLine(",'EMPLOYEE_DEDUCTION_HISTORY'");
            strQry.AppendLine(",'EMPLOYEE_EARNING_WEEK_HISTORY'");
            strQry.AppendLine(",'EMPLOYEE_PAY_CATEGORY_HISTORY'");
            strQry.AppendLine(",'EMPLOYEE_EARNING_HISTORY'");
            strQry.AppendLine(",'EMPLOYEE_TIMESHEET_BREAK_DAY_HISTORY'");
            strQry.AppendLine(",'EMPLOYEE_TIMESHEET_HISTORY'");
            strQry.AppendLine(",'LEAVE_SHIFT_HISTORY'");
            strQry.AppendLine(",'PAY_CATEGORY_BREAK_HISTORY'");
            strQry.AppendLine(",'PAY_CATEGORY_PERIOD_HISTORY'");
            strQry.AppendLine(",'PAY_CATEGORY_WEEK_HISTORY')");
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Tables", -1);
            
            for (int intRow = 0; intRow < DataSet.Tables["Tables"].Rows.Count; intRow++)
            {
                strQry.Clear();

                strQry.AppendLine(" DELETE FROM " + strTableNoToLoad + ".dbo." + DataSet.Tables["Tables"].Rows[intRow]["TABLE_NAME"].ToString());

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                
                if (DataSet.Tables["Columns"] != null)
                {
                    DataSet.Tables.Remove("Columns");
                }

                strQry.Clear();
                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" COLUMN_NAME ");
                strQry.AppendLine(",DATA_TYPE ");

                strQry.AppendLine(" FROM " + strTableNoToLoad + ".INFORMATION_SCHEMA.COLUMNS ");
                strQry.AppendLine(" WHERE TABLE_NAME = '" + DataSet.Tables["Tables"].Rows[intRow]["TABLE_NAME"].ToString() + "'");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" ORDINAL_POSITION ");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Columns", -1);

                strQryCol.Clear();

                for (int intCol = 0; intCol < DataSet.Tables["Columns"].Rows.Count; intCol++)
                {
                    if (intCol == 0)
                    {
                        strQryCol.AppendLine(" " + DataSet.Tables["Columns"].Rows[intCol]["COLUMN_NAME"].ToString());
                    }
                    else
                    {
                        strQryCol.AppendLine("," + DataSet.Tables["Columns"].Rows[intCol]["COLUMN_NAME"].ToString());
                    }
                }

                strQry.Clear();

                if (DataSet.Tables["Tables"].Rows[intRow]["TABLE_NAME"].ToString() == "LEAVE_HISTORY")
                {
                    strQry.AppendLine("SET IDENTITY_INSERT " + strTableNoToLoad + ".dbo." + DataSet.Tables["Tables"].Rows[intRow]["TABLE_NAME"].ToString() + " ON");
                }

                strQry.AppendLine(" INSERT INTO " + strTableNoToLoad + ".dbo." + DataSet.Tables["Tables"].Rows[intRow]["TABLE_NAME"].ToString());
                strQry.AppendLine("(");

                strQry.Append(strQryCol.ToString());

                strQry.AppendLine(")");

                strQry.AppendLine(" SELECT ");

                strQry.Append(strQryCol.ToString().Replace("COMPANY_NO","18"));
                
                strQry.AppendLine(" FROM InteractPayroll_00013.dbo." + DataSet.Tables["Tables"].Rows[intRow]["TABLE_NAME"].ToString());

                strQry.AppendLine(" WHERE PAY_PERIOD_DATE < '" + parstrDate + "'");

                if (DataSet.Tables["Tables"].Rows[intRow]["TABLE_NAME"].ToString() == "LEAVE_HISTORY")
                {
                    strQry.AppendLine("SET IDENTITY_INSERT " + strTableNoToLoad + ".dbo." + DataSet.Tables["Tables"].Rows[intRow]["TABLE_NAME"].ToString() + " OFF");
                }
                
                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            }
            
            strQry.Clear();

            strQry.AppendLine(" UPDATE EN");

            strQry.AppendLine(" SET ");
            strQry.AppendLine(" EMPLOYEE_LAST_RUNDATE = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN E.EMPLOYEE_LAST_RUNDATE < TEMP_TABLE.MAX_PAY_PERIOD_DATE ");

            strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE");

            strQry.AppendLine(" ELSE ");
            
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN E.PAY_CATEGORY_TYPE = 'W' ");
            strQry.AppendLine(" THEN TEMP_TABLE.MAX_PAY_PERIOD_DATE ");

            strQry.AppendLine(" ELSE '" + EndOfMonthDateTime.AddDays(1).AddMonths(-1).AddDays(-1).ToString("yyyy-MM-dd") + "'");
            
            strQry.AppendLine(" END ");
            
            strQry.AppendLine(" END, ");

            strQry.AppendLine(" EMPLOYEE_ENDDATE = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN E.EMPLOYEE_ENDDATE IS NULL ");

            strQry.AppendLine(" THEN E.EMPLOYEE_ENDDATE");

            strQry.AppendLine(" WHEN E.EMPLOYEE_ENDDATE < TEMP_TABLE.MAX_PAY_PERIOD_DATE ");

            strQry.AppendLine(" THEN E.EMPLOYEE_ENDDATE");

            strQry.AppendLine(" ELSE ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN E.PAY_CATEGORY_TYPE = 'W' ");
            strQry.AppendLine(" THEN TEMP_TABLE.MAX_PAY_PERIOD_DATE ");

            strQry.AppendLine(" ELSE '" + EndOfMonthDateTime.AddDays(1).AddMonths(-1).AddDays(-1).ToString("yyyy-MM-dd") + "'");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM " + strTableNoToLoad + ".dbo.EMPLOYEE EN");
            
            strQry.AppendLine(" INNER JOIN InteractPayroll_00013.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON EN.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" INNER JOIN ");

            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" COMPANY_NO ");
            strQry.AppendLine(",MAX(PAY_PERIOD_DATE) AS MAX_PAY_PERIOD_DATE ");

            strQry.AppendLine(" FROM InteractPayroll_00013.dbo.PAY_CATEGORY_PERIOD_HISTORY ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = 13 ");

            strQry.AppendLine(" AND PAY_PERIOD_DATE < '" + parstrDate + "'");

            strQry.AppendLine(" AND RUN_TYPE = 'P' ");
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" COMPANY_NO ) AS TEMP_TABLE ");

            strQry.AppendLine(" ON E.COMPANY_NO = TEMP_TABLE.COMPANY_NO ");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            //2018-02-03
            strQry.Clear();

            strQry.AppendLine(" UPDATE " + strTableNoToLoad + ".dbo.PAY_CATEGORY");

            strQry.AppendLine(" SET LAST_UPLOAD_DATETIME = '" + DateTime.ParseExact(parstrDate, "yyyy-MM-dd", null).AddDays(35).AddMinutes(15).ToString("yyyy-MM-dd HH:mm:ss") + "'");

            //strQry.AppendLine(" WHERE NOT PAY_CATEGORY_TYPE = 'S'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }
    }
}
