using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
//using System.Data.DataSetExtensions;
using System.Linq;

namespace InteractPayroll
{
    public class busTimeSheet
    {
        clsDBConnectionObjects clsDBConnectionObjects;
        
        public busTimeSheet()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo, Int64 parint64CurrentUserNo, string parstrCurrentUserAccessInd, string parstrFromProgram)
        {
            Check_Company_Triggers(parint64CompanyNo);
            
            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();
            
            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
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
            
            strQry.AppendLine(",PAY_PERIOD_DATE = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN PC.PAY_CATEGORY_TYPE = 'S' AND NOT PCPC.SALARY_TIMESHEET_ENDDATE IS NULL");
            strQry.AppendLine(" THEN PCPC.SALARY_TIMESHEET_ENDDATE ");

            strQry.AppendLine(" ELSE PCPC.PAY_PERIOD_DATE ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",PCPC.PAY_PERIOD_DATE_FROM");

            strQry.AppendLine(",PC.LAST_UPLOAD_DATETIME");
 
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

            //2013-09-04
            strQry.AppendLine(",AUTHORISE_TEMP.LEVEL_NO AS MAX_AUTHORISE_LEVEL_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

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

                strQry.AppendLine(" ON PC.PAY_CATEGORY_NO = USER_TABLE.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = USER_TABLE.PAY_CATEGORY_TYPE");
            }
           
            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");
            strQry.AppendLine(" ON PC.COMPANY_NO = PCPC.COMPANY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = PCPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = PCPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND PCPC.PAY_CATEGORY_NO > 0");
            strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P'");

            //2013-09-04
            strQry.AppendLine(" LEFT JOIN ");

            strQry.AppendLine("(SELECT ");

            strQry.AppendLine(" PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",MAX(LEVEL_NO) AS LEVEL_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT  ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            
            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE) AS AUTHORISE_TEMP");

            strQry.AppendLine(" ON PC.PAY_CATEGORY_NO = AUTHORISE_TEMP.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = AUTHORISE_TEMP.PAY_CATEGORY_TYPE");
            
            strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE IN ('W','S')");
            }

            strQry.AppendLine(" AND ISNULL(CLOSED_IND,'N') <> 'Y'");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PC.COMPANY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE DESC");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategory", parint64CompanyNo);

            DataSet.Tables.Add("PayrollType");
            DataTable PayrollTypeDataTable = new DataTable("PayrollType");
            DataSet.Tables["PayrollType"].Columns.Add("PAYROLL_TYPE_DESC", typeof(String));
            DataSet.Tables["PayrollType"].Columns.Add("MIN_EMPLOYEE_LAST_RUNDATE", typeof(DateTime));

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

            //Errol 2013-06-15
            strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN E.EMPLOYEE_LAST_RUNDATE IS NULL ");

            strQry.AppendLine(" THEN DATEADD(DD,-40,'" + DateTime.Now.ToString("yyyy-MM-dd") + "')");

            strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y' ");

            strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

            strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

            //Errol 2013-04-12
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE IN ('W','S')");
            }
            
            //2017-09-12
            strQry.AppendLine(" AND NOT ISNULL(PC.CLOSED_IND, '') = 'Y'");
            
            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" E.COMPANY_NO ");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parint64CompanyNo);
            
            for (int intRow = 0; intRow < DataSet.Tables["PayrollType"].Rows.Count; intRow++)
            {
                DataView EmployeeDataView = new DataView(DataSet.Tables["Employee"],
                                                        "PAY_CATEGORY_TYPE = '" + DataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE_DESC"].ToString().Substring(0,1) + "'",
                                                        "EMPLOYEE_LAST_RUNDATE",
                                                        DataViewRowState.CurrentRows);

                if (EmployeeDataView.Count > 0)
                {
                    DataSet.Tables["PayrollType"].Rows[intRow]["MIN_EMPLOYEE_LAST_RUNDATE"] = Convert.ToDateTime(EmployeeDataView[0]["EMPLOYEE_LAST_RUNDATE"]).AddDays(1);
                }
            }
            
            //2017-09-08
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO ");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");

            strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN E.EMPLOYEE_LAST_RUNDATE IS NULL ");

            strQry.AppendLine(" THEN DATEADD(DD,-40,'" + DateTime.Now.ToString("yyyy-MM-dd") + "')");

            strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y' ");

            strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

            strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

            //Errol 2013-04-12
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE IN ('W','S')");
            }

            //2017-09-12
            strQry.AppendLine(" AND NOT ISNULL(PC.CLOSED_IND, '') = 'Y'");
            
            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" E.COMPANY_NO ");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeePayCategory", parint64CompanyNo);
            
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PH.PUBLIC_HOLIDAY_DATE");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY PH");
            
            strQry.AppendLine(" WHERE PH.PUBLIC_HOLIDAY_DATE > '" + DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd") + "'");
            strQry.AppendLine(" AND PH.PUBLIC_HOLIDAY_DATE <= '" + DateTime.Now.AddDays(15).ToString("yyyy-MM-dd") + "'");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PH.PUBLIC_HOLIDAY_DATE");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PublicHoliday", parint64CompanyNo);
         
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        private void Check_Company_Triggers(Int64 parint64CompanyNo)
        {
            DataSet DataSet = new DataSet();
   
            clsInteractPayrollTriggers clsInteractPayrollTriggers = new InteractPayroll.clsInteractPayrollTriggers();

            StringBuilder strQry = new StringBuilder();

            string strConnectionString = clsDBConnectionObjects.Get_ConnectionString();
            string strNewDatabase = "InteractPayroll_" + parint64CompanyNo.ToString("00000");

            //Logon To Correct Database
            clsDBConnectionObjects.Set_ConnectionString(strConnectionString + "Database=" + strNewDatabase + ";");
            
            //strQry.AppendLine(" USE InteractPayrollClient ");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" TriggerName = t.name");
            strQry.AppendLine(",Defininion  = object_definition(t.object_id)");

            strQry.AppendLine(" FROM sys.triggers t ");

            strQry.AppendLine(" LEFT JOIN sys.all_objects o ");
            strQry.AppendLine(" ON t.parent_id = o.object_id ");

            strQry.AppendLine(" LEFT JOIN sys.schemas s ");
            strQry.AppendLine(" ON s.schema_id = o.schema_id ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Trigger",parint64CompanyNo);

            DataSet.AcceptChanges();

            bool blnCreateTrigger = true;

            DataView tempDataView = new DataView(DataSet.Tables["Trigger"],
                                                "TriggerName = 'tgr_Create_Payroll_Timesheet'",
                                                "",
                                                DataViewRowState.CurrentRows);

            if (tempDataView.Count > 0)
            {
                //Logic to Check Version No
                if (tempDataView[0]["Defininion"].ToString().IndexOf("--Version 1.5") > 0)
                {
                    blnCreateTrigger = false;
                }
                else
                {
                    strQry.Clear();
                    strQry.AppendLine(" DROP TRIGGER dbo.tgr_Create_Payroll_Timesheet ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                }
            }

            if (blnCreateTrigger == true)
            {
                //Create TRIGGER tgr_Create_Payroll_Timesheet
                strQry.Clear();
                strQry.Append(clsInteractPayrollTriggers.tgr_Create_Payroll_Timesheet());

                this.clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            }

            blnCreateTrigger = true;

            tempDataView = null;
            tempDataView = new DataView(DataSet.Tables["Trigger"],
                                                "TriggerName = 'tgr_Create_Payroll_Break'",
                                                "",
                                                DataViewRowState.CurrentRows);

            if (tempDataView.Count > 0)
            {
                //Logic to Check Version No
                if (tempDataView[0]["Defininion"].ToString().IndexOf("--Version 1.5") > 0)
                {
                    blnCreateTrigger = false;
                }
                else
                {
                    strQry.Clear();
                    strQry.AppendLine(" DROP TRIGGER dbo.tgr_Create_Payroll_Break ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                }
            }

            if (blnCreateTrigger == true)
            {
                //Create TRIGGER tgr_Create_Payroll_Break
                strQry.Clear();
                strQry.Append(clsInteractPayrollTriggers.tgr_Create_Payroll_Break());

                this.clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            }

            blnCreateTrigger = true;

            tempDataView = new DataView(DataSet.Tables["Trigger"],
                                        "TriggerName = 'tgr_EMPLOYEE_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table'",
                                        "",
                                        DataViewRowState.CurrentRows);

            if (tempDataView.Count > 0)
            {
                //Logic to Check Version No
                if (tempDataView[0]["Defininion"].ToString().IndexOf("--Version 1.6") > 0)
                {
                    blnCreateTrigger = false;
                }
                else
                {
                    strQry.Clear();
                    strQry.AppendLine(" DROP TRIGGER dbo.tgr_EMPLOYEE_SALARY_BREAK_CURRENT_Maintain_EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DROP TRIGGER dbo.tgr_EMPLOYEE_SALARY_TIMESHEET_CURRENT_Maintain_EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DROP TRIGGER dbo.tgr_EMPLOYEE_TIME_ATTEND_BREAK_CURRENT_Maintain_EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DROP TRIGGER dbo.tgr_EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DROP TRIGGER dbo.tgr_EMPLOYEE_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DROP TRIGGER dbo.tgr_EMPLOYEE_BREAK_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                }
            }

            if (blnCreateTrigger == true)
            {
                //Create TRIGGER tgr_EMPLOYEE_BREAK_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table
                strQry.Clear();
                strQry.Append(clsInteractPayrollTriggers.tgr_Create_For_Timesheet_Break_Table("EMPLOYEE_BREAK_CURRENT"));

                this.clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                //Create TRIGGER tgr_EMPLOYEE_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table
                strQry.Clear();
                strQry.Append(clsInteractPayrollTriggers.tgr_Create_For_Timesheet_Break_Table("EMPLOYEE_TIMESHEET_CURRENT"));

                this.clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                //Create TRIGGER tgr_EMPLOYEE_TIME_ATTEND_BREAK_CURRENT_Maintain_EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_Table
                strQry.Clear();
                strQry.Append(clsInteractPayrollTriggers.tgr_Create_For_Timesheet_Break_Table("EMPLOYEE_TIME_ATTEND_BREAK_CURRENT"));

                this.clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                //Create TRIGGER tgr_EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_Table
                strQry.Clear();
                strQry.Append(clsInteractPayrollTriggers.tgr_Create_For_Timesheet_Break_Table("EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT"));

                this.clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                //Create TRIGGER tgr_EMPLOYEE_SALARY_BREAK_CURRENT_Maintain_EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT_Table
                strQry.Clear();
                strQry.Append(clsInteractPayrollTriggers.tgr_Create_For_Timesheet_Break_Table("EMPLOYEE_SALARY_BREAK_CURRENT"));

                this.clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                //Create TRIGGER tgr_EMPLOYEE_SALARY_TIMESHEET_CURRENT_Maintain_EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT_Table
                strQry.Clear();
                strQry.Append(clsInteractPayrollTriggers.tgr_Create_For_Timesheet_Break_Table("EMPLOYEE_SALARY_TIMESHEET_CURRENT"));

                this.clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT ");
                strQry.AppendLine(" SET COMPANY_NO = COMPANY_NO ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ");
                strQry.AppendLine(" SET COMPANY_NO = COMPANY_NO ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ");
                strQry.AppendLine(" SET COMPANY_NO = COMPANY_NO ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ");
                strQry.AppendLine(" SET COMPANY_NO = COMPANY_NO ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ");
                strQry.AppendLine(" SET COMPANY_NO = COMPANY_NO ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ");
                strQry.AppendLine(" SET COMPANY_NO = COMPANY_NO ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            }
        }

        private StringBuilder DayTotal_SQL(Int64 parint64CompanyNo, string parstrPayCategoryType,int parintPayCategoryNo, string parstrCurrentUserAccessInd, Int64 parint64CurrentUserNo, string parstrSpecificDate, string parstrSpecificEmployee)
        {
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ETATBDC.COMPANY_NO");
            strQry.AppendLine(",'" + parstrPayCategoryType + "' AS PAY_CATEGORY_TYPE");
            strQry.AppendLine(",ETATBDC.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETATBDC.EMPLOYEE_NO ");
            strQry.AppendLine(",ETATBDC.TIMESHEET_DATE AS DAY_DATE");
            strQry.AppendLine(",ETATBDC.DAY_NO");
            strQry.AppendLine(",ETATBDC.DAY_PAID_MINUTES");
            //2017-09-8 Change Indicator to Error When Leave
            strQry.AppendLine(",INDICATOR = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN NOT FINAL_EMPLOYEE_LEAVE_TABLE.DAY_DATE IS NULL THEN 'X' ");
                
            strQry.AppendLine(" ELSE ETATBDC.INDICATOR ");

            strQry.AppendLine(" END ");
                
            strQry.AppendLine(",ETATBDC.BREAK_ACCUM_MINUTES");
            strQry.AppendLine(",ETATBDC.BREAK_INDICATOR ");

            strQry.AppendLine(",PAID_HOLIDAY_INDICATOR = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN PUBLIC_HOLIDAY_TABLE.PUBLIC_HOLIDAY_DATE > ");

            strQry.AppendLine("(CASE ");

            strQry.AppendLine(" WHEN E.EMPLOYEE_LAST_RUNDATE IS NULL THEN DATEADD(DD, -40, GETDATE()) ");

            strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND, '') <> 'Y' THEN DATEADD(DD,-1, E.EMPLOYEE_LAST_RUNDATE) ");

            strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

            strQry.AppendLine(" END) THEN 'Y' ");
                
            strQry.AppendLine(" ELSE ''");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",ETATBDC.INCLUDED_IN_RUN_INDICATOR ");

            //2017-09-08 Error Leave while Working
            strQry.AppendLine(",LEAVE_INDICATOR = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN NOT FINAL_EMPLOYEE_LEAVE_TABLE.DAY_DATE IS NULL THEN 'Y' ");
            
            strQry.AppendLine(" ELSE '' ");
                
            strQry.AppendLine(" END ");

            strQry.AppendLine("");
            
            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT ETATBDC");
            }
            else
            {
                if (parstrPayCategoryType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT ETATBDC");

                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT ETATBDC");
                }
            }

            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON ETATBDC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND ETATBDC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");

            if (parstrSpecificEmployee != "")
            {
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + parstrSpecificEmployee);
            }
            
            strQry.AppendLine(" AND ETATBDC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            
            if (parintPayCategoryNo != -1)
            {
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + parintPayCategoryNo);
            }
            
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");
            strQry.AppendLine(" ON ETATBDC.COMPANY_NO = E.COMPANY_NO ");

            if (parstrSpecificEmployee != "")
            {
                strQry.AppendLine(" AND E.EMPLOYEE_NO = " + parstrSpecificEmployee);
            }

            strQry.AppendLine(" AND ETATBDC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

            strQry.AppendLine("");
            strQry.AppendLine(" LEFT JOIN ");
            strQry.AppendLine("( ");
            strQry.AppendLine("--3Start");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PH.PUBLIC_HOLIDAY_DATE");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY PH");

            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON PC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND PC.PAY_PUBLIC_HOLIDAY_IND = 'Y'");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            if (parintPayCategoryNo != -1)
            {
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = " + parintPayCategoryNo);
            }

            if (parstrSpecificDate == "")
            {
                strQry.AppendLine(" WHERE PH.PUBLIC_HOLIDAY_DATE > '" + DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd") + "'");
            }
            else
            {
                strQry.AppendLine(" WHERE PH.PUBLIC_HOLIDAY_DATE = '" + parstrSpecificDate + "'");
            }

            strQry.AppendLine("--3End");

            strQry.AppendLine(") AS PUBLIC_HOLIDAY_TABLE");

            strQry.AppendLine(" ON ETATBDC.PAY_CATEGORY_NO = PUBLIC_HOLIDAY_TABLE.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND ETATBDC.TIMESHEET_DATE = PUBLIC_HOLIDAY_TABLE.PUBLIC_HOLIDAY_DATE ");

            strQry.AppendLine("");
            strQry.AppendLine(" LEFT JOIN ");

            strQry.AppendLine("");
            strQry.AppendLine("(");
            strQry.AppendLine("--2Start");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EMPLOYEE_LEAVE_TABLE.COMPANY_NO ");
            strQry.AppendLine(",EMPLOYEE_LEAVE_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_LEAVE_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EMPLOYEE_LEAVE_TABLE.EMPLOYEE_NO");
            strQry.AppendLine(",D.DAY_DATE");

            strQry.AppendLine("");
            strQry.AppendLine(" FROM ");

            strQry.AppendLine("");
            strQry.AppendLine("(");
            strQry.AppendLine("--1Start");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO ");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.LEAVE_SHIFT_NO");
            strQry.AppendLine(",LC.LEAVE_FROM_DATE");
            strQry.AppendLine(",LC.LEAVE_TO_DATE");

            strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN E.EMPLOYEE_LAST_RUNDATE IS NULL ");
            strQry.AppendLine(" THEN DATEADD(DD, -40, '" + DateTime.Now.ToString("yyyy-MM-dd") + "')");
            
            strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND, '') <> 'Y' ");
            strQry.AppendLine(" THEN DATEADD(DD,-1, E.EMPLOYEE_LAST_RUNDATE) ");

            strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

            strQry.AppendLine(" END ");

            strQry.AppendLine("");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");

            if (parstrSpecificEmployee != "")
            {
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + parstrSpecificEmployee);
            }

            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");

            if (parintPayCategoryNo != -1)
            {
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + parintPayCategoryNo);
            }

            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT LC ");
            strQry.AppendLine(" ON E.COMPANY_NO = LC.COMPANY_NO");

            if (parstrSpecificEmployee != "")
            {
                strQry.AppendLine(" AND LC.EMPLOYEE_NO = " + parstrSpecificEmployee);
            }
            
            strQry.AppendLine(" AND E.EMPLOYEE_NO = LC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = LC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND LC.PROCESS_NO < 99");
            strQry.AppendLine(" AND LC.LEAVE_OPTION = 'D'");

            strQry.AppendLine("");
            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);

            if (parstrSpecificEmployee != "")
            {
                strQry.AppendLine(" AND E.EMPLOYEE_NO = " + parstrSpecificEmployee);
            }

            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            strQry.AppendLine("");
            strQry.AppendLine(" UNION ");

            strQry.AppendLine("");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO ");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.LEAVE_SHIFT_NO");
            strQry.AppendLine(",LH.LEAVE_FROM_DATE");
            strQry.AppendLine(",LH.LEAVE_TO_DATE");

            strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN E.EMPLOYEE_LAST_RUNDATE IS NULL ");
            strQry.AppendLine(" THEN DATEADD(DD, -40, '" + DateTime.Now.ToString("yyyy-MM-dd") + "')");

            strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND, '') <> 'Y' ");
            strQry.AppendLine(" THEN DATEADD(DD,-1, E.EMPLOYEE_LAST_RUNDATE) ");

            strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

            strQry.AppendLine(" END ");

            strQry.AppendLine("");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");

            if (parstrSpecificEmployee != "")
            {
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + parstrSpecificEmployee);
            }

            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");

            if (parintPayCategoryNo != -1)
            {
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + parintPayCategoryNo);
            }

            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY LH ");
            strQry.AppendLine(" ON E.COMPANY_NO = LH.COMPANY_NO");
            strQry.AppendLine(" AND LH.PAY_PERIOD_DATE >= '" + DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd") + "'");

            if (parstrSpecificEmployee != "")
            {
                strQry.AppendLine(" AND LH.EMPLOYEE_NO = " + parstrSpecificEmployee);
            }
            
            strQry.AppendLine(" AND E.EMPLOYEE_NO = LH.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = LH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND LH.PROCESS_NO < 99");
            strQry.AppendLine(" AND LH.LEAVE_OPTION = 'D'");

            strQry.AppendLine("");
            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);

            if (parstrSpecificEmployee != "")
            {
                strQry.AppendLine(" AND E.EMPLOYEE_NO = " + parstrSpecificEmployee);
            }

            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            strQry.AppendLine("--1End");
            strQry.AppendLine(" ) AS EMPLOYEE_LEAVE_TABLE");

            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");
            strQry.AppendLine(" ON D.DAY_DATE >= EMPLOYEE_LEAVE_TABLE.LEAVE_FROM_DATE");
            strQry.AppendLine(" AND D.DAY_DATE <= EMPLOYEE_LEAVE_TABLE.LEAVE_TO_DATE");
            strQry.AppendLine(" AND D.DAY_DATE > EMPLOYEE_LEAVE_TABLE.EMPLOYEE_LAST_RUNDATE");

            if (parstrSpecificDate != "")
            {
                strQry.AppendLine(" AND D.DAY_DATE = '" + parstrSpecificDate + "'");
            }

            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT LS");
            strQry.AppendLine(" ON EMPLOYEE_LEAVE_TABLE.COMPANY_NO = LS.COMPANY_NO");
            strQry.AppendLine(" AND EMPLOYEE_LEAVE_TABLE.LEAVE_SHIFT_NO = LS.LEAVE_SHIFT_NO");
            strQry.AppendLine(" AND EMPLOYEE_LEAVE_TABLE.PAY_CATEGORY_TYPE = LS.PAY_CATEGORY_TYPE");
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

            strQry.AppendLine("");
            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY PH");
            strQry.AppendLine(" ON D.DAY_DATE = PH.PUBLIC_HOLIDAY_DATE");
            //Remove Public Holiday
            strQry.AppendLine(" WHERE PH.PUBLIC_HOLIDAY_DATE IS NULL");
            strQry.AppendLine("--2End");
            strQry.AppendLine(") AS FINAL_EMPLOYEE_LEAVE_TABLE");
            strQry.AppendLine(" ON ETATBDC.COMPANY_NO = FINAL_EMPLOYEE_LEAVE_TABLE.COMPANY_NO ");
            strQry.AppendLine(" AND ETATBDC.EMPLOYEE_NO = FINAL_EMPLOYEE_LEAVE_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = FINAL_EMPLOYEE_LEAVE_TABLE.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND ETATBDC.TIMESHEET_DATE = FINAL_EMPLOYEE_LEAVE_TABLE.DAY_DATE");

            strQry.AppendLine("");
            strQry.AppendLine(" WHERE ETATBDC.COMPANY_NO = " + parint64CompanyNo);
            
            if (parstrSpecificEmployee != "")
            {
                strQry.AppendLine(" AND ETATBDC.EMPLOYEE_NO = " + parstrSpecificEmployee);
            }
            
            if (parintPayCategoryNo != -1)
            {
                strQry.AppendLine(" AND ETATBDC.PAY_CATEGORY_NO = " + parintPayCategoryNo);
            }

            //2017-09-07
            strQry.AppendLine(" AND ETATBDC.TIMESHEET_DATE > ");

            strQry.AppendLine("(CASE ");

            strQry.AppendLine(" WHEN E.EMPLOYEE_LAST_RUNDATE IS NULL THEN DATEADD(DD, -40, GETDATE()) ");

            strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND, '') <> 'Y' THEN DATEADD(DD,-1, E.EMPLOYEE_LAST_RUNDATE) ");

            strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");
                
            strQry.AppendLine(" END) ");
            
            if (parstrSpecificDate != "")
            {
                strQry.AppendLine(" AND ETATBDC.TIMESHEET_DATE = '" + parstrSpecificDate + "'");
            }

            strQry.AppendLine("");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" ETATBDC.COMPANY_NO");
            strQry.AppendLine(",ETATBDC.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETATBDC.EMPLOYEE_NO ");
            strQry.AppendLine(",ETATBDC.TIMESHEET_DATE");
            
            return strQry;
        }
        
        private StringBuilder TimeSheet_SQL(Int64 parint64CompanyNo, string parstrPayCategoryType, int parintPayCategoryNo, string parstrCurrentUserAccessInd, Int64 parint64CurrentUserNo, string parstrSpecificDate, string parstrSpecificEmployee)
        {
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
                    
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
            strQry.AppendLine(",ISNULL(ETC.TIMESHEET_ACCUM_MINUTES,0) AS TIMESHEET_ACCUM_MINUTES");
            strQry.AppendLine(",ETC.INDICATOR");
            strQry.AppendLine(",ETC.INCLUDED_IN_RUN_IND ");

            strQry.AppendLine("");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine("");

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

            if (parstrSpecificEmployee != "")
            {
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = " + parstrSpecificEmployee);
            }

            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
            
            if (parintPayCategoryNo != -1)
            {
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + parintPayCategoryNo);
            }
            
            if (parstrSpecificDate == "")
            {
                strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= DATEADD(DD,15,'" + DateTime.Now.ToString("yyyy-MM-dd") + "')");
            }
            else
            {
                strQry.AppendLine(" AND ETC.TIMESHEET_DATE = '" + parstrSpecificDate + "'");
            }

            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");

            if (parstrSpecificEmployee != "")
            {
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + parstrSpecificEmployee);
            }
            
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");

            if (parintPayCategoryNo != -1)
            {
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + parintPayCategoryNo);
            }

            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine("");
            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);

            if (parstrSpecificEmployee != "")
            {
                strQry.AppendLine(" AND E.EMPLOYEE_NO = " + parstrSpecificEmployee);
            }
            
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            strQry.AppendLine("");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" ETC.COMPANY_NO");
            strQry.AppendLine(",ETC.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETC.EMPLOYEE_NO ");
            strQry.AppendLine(",ETC.TIMESHEET_DATE");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_OUT_MINUTES");
            
            return strQry;
        }

        private StringBuilder Break_SQL(Int64 parint64CompanyNo, string parstrPayCategoryType, int parintPayCategoryNo, string parstrCurrentUserAccessInd, Int64 parint64CurrentUserNo, string parstrSpecificDate, string parstrSpecificEmployee)
        {
            StringBuilder strQry = new StringBuilder();
            
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ETC.COMPANY_NO");
            strQry.AppendLine(",'" + parstrPayCategoryType + "' AS PAY_CATEGORY_TYPE");
            strQry.AppendLine(",ETC.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETC.EMPLOYEE_NO ");
            strQry.AppendLine(",ETC.BREAK_DATE");
            strQry.AppendLine(",ETC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine(",ETC.BREAK_SEQ");
            strQry.AppendLine(",ETC.CLOCKED_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC.CLOCKED_TIME_OUT_MINUTES");
            strQry.AppendLine(",ISNULL(ETC.BREAK_ACCUM_MINUTES,0) AS BREAK_ACCUM_MINUTES");
            strQry.AppendLine(",ETC.INDICATOR");
            strQry.AppendLine(",ETC.INCLUDED_IN_RUN_IND");

            strQry.AppendLine("");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine("");

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
            
            if (parstrSpecificEmployee != "")
            {
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = " + parstrSpecificEmployee);
            }
            
            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
            
            if (parintPayCategoryNo != -1)
            {
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + parintPayCategoryNo);
            }
            
            if (parstrSpecificDate == "")
            {
                strQry.AppendLine(" AND ETC.BREAK_DATE <= DATEADD(DD,15,'" + DateTime.Now.ToString("yyyy-MM-dd") + "')");
            }
            else
            {
                strQry.AppendLine(" AND ETC.BREAK_DATE = '" + parstrSpecificDate + "'");
            }

            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");

            if (parstrSpecificEmployee != "")
            {
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + parstrSpecificEmployee);
            }

            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            
            if (parintPayCategoryNo != -1)
            {
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + parintPayCategoryNo);
            }
            
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine("");
            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            
            if (parstrSpecificEmployee != "")
            {
                strQry.AppendLine(" AND E.EMPLOYEE_NO = " + parstrSpecificEmployee);
            }

            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            strQry.AppendLine("");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" ETC.COMPANY_NO");
            strQry.AppendLine(",ETC.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETC.EMPLOYEE_NO ");
            strQry.AppendLine(",ETC.BREAK_DATE");
            strQry.AppendLine(",ETC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC.BREAK_TIME_OUT_MINUTES");
            
            return strQry;
        }

        public byte[] Get_PayCategory_Records(Int64 parint64CompanyNo, string parstrPayCategoryType, Int64 parint64CurrentUserNo, string parstrCurrentUserAccessInd)
        {
            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();
            
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" TEMP_TABLE.COMPANY_NO ");
            strQry.AppendLine(",TEMP_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TEMP_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",D.DAY_DATE");
            strQry.AppendLine(",D.DAY_NO");
            strQry.AppendLine(",PAID_HOLIDAY_INDICATOR = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN PUBLIC_HOLIDAY_DATE IS NULL THEN '' ");

            strQry.AppendLine(" ELSE 'Y' ");

            strQry.AppendLine(" END ");
            
            strQry.AppendLine(" FROM ");
            
            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" E.COMPANY_NO ");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
          
            strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");
            
            strQry.AppendLine(" MIN(CASE ");

            //Errol 2015-02-12
            strQry.AppendLine(" WHEN E.EMPLOYEE_LAST_RUNDATE IS NULL ");

            strQry.AppendLine(" THEN DATEADD(DD,-40,'" + DateTime.Now.ToString("yyyy-MM-dd") + "')");

            strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y' ");

            strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

            strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");
            
            strQry.AppendLine(" END)");

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

                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }
  
            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            //Errol 2013-06-15
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" E.COMPANY_NO ");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");

            strQry.AppendLine(",EPC.PAY_CATEGORY_NO) AS TEMP_TABLE");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");

            strQry.AppendLine(" ON D.DAY_DATE > TEMP_TABLE.EMPLOYEE_LAST_RUNDATE");
            strQry.AppendLine(" AND D.DAY_DATE <= DATEADD(DD,15,'" + DateTime.Now.ToString("yyyy-MM-dd") + "')");

            strQry.AppendLine(" LEFT  JOIN InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY PH ");
            strQry.AppendLine(" ON D.DAY_DATE = PH.PUBLIC_HOLIDAY_DATE ");
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" TEMP_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",DAY_DATE DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Dates", parint64CompanyNo);

            //2017-09-16
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EMPLOYEE_TABLE.COMPANY_NO ");
            strQry.AppendLine(",EMPLOYEE_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EMPLOYEE_TABLE.EMPLOYEE_NO");
            strQry.AppendLine(",D.DAY_DATE");
         
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
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "') AS EMPLOYEE_TABLE");
            
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
            strQry.AppendLine(" EMPLOYEE_TABLE.COMPANY_NO ");
            strQry.AppendLine(",EMPLOYEE_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EMPLOYEE_TABLE.EMPLOYEE_NO");
            strQry.AppendLine(",D.DAY_DATE");
          
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
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "') AS EMPLOYEE_TABLE");

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

            int intTimeoutValue = 30;

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" TIMESHEET_READ_TIMEOUT_SECONDS ");
            
            strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "TimeOut", parint64CompanyNo);

            if (DataSet.Tables["TimeOut"].Rows[0]["TIMESHEET_READ_TIMEOUT_SECONDS"] != System.DBNull.Value)
            {
                intTimeoutValue = Convert.ToInt32(DataSet.Tables["TimeOut"].Rows[0]["TIMESHEET_READ_TIMEOUT_SECONDS"]);
            }

            strQry = DayTotal_SQL(parint64CompanyNo, parstrPayCategoryType,-1, parstrCurrentUserAccessInd, parint64CurrentUserNo,"","");

            //using (System.IO.StreamWriter file = new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "busTimeSheetInfo.txt", true))
            //{
            //    file.WriteLine("DayTotal");
            //}

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "DayTotal", parint64CompanyNo, intTimeoutValue);
            
            strQry = TimeSheet_SQL(parint64CompanyNo, parstrPayCategoryType,-1, parstrCurrentUserAccessInd, parint64CurrentUserNo,"","");

            //using (System.IO.StreamWriter file = new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "busTimeSheetInfo.txt", true))
            //{
            //    file.WriteLine("TimeSheet");
            //}

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "TimeSheet", parint64CompanyNo, intTimeoutValue);

            strQry = Break_SQL(parint64CompanyNo, parstrPayCategoryType,-1, parstrCurrentUserAccessInd, parint64CurrentUserNo,"","");

            //using (System.IO.StreamWriter file = new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "busTimeSheetInfo.txt", true))
            //{
            //    file.WriteLine("Break");
            //}

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Break", parint64CompanyNo, intTimeoutValue);

            strQry.Clear();

            //Create Empty BlankDay Table
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",'' AS PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_NO ");
            strQry.AppendLine(",TIMESHEET_DATE AS DAY_DATE");
            strQry.AppendLine(",DAY_NO");
            strQry.AppendLine(",DAY_PAID_MINUTES");
            strQry.AppendLine(",INDICATOR");
            strQry.AppendLine(",BREAK_ACCUM_MINUTES");
            strQry.AppendLine(",BREAK_INDICATOR ");
            strQry.AppendLine(",INDICATOR AS PAID_HOLIDAY_INDICATOR");
            strQry.AppendLine(",INCLUDED_IN_RUN_INDICATOR ");
            strQry.AppendLine(",INDICATOR AS LEAVE_INDICATOR");
            
            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT ETATBDC");
            }
            else
            {
                if (parstrPayCategoryType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT ETATBDC");

                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT ETATBDC");
                }
            }
            
            strQry.AppendLine(" WHERE COMPANY_NO = -1 ");
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "DayBlank", parint64CompanyNo);
            
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
                
                strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo);
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
           
            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" EPCWAC.COMPANY_NO");
            strQry.AppendLine(",EPCWAC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EPCWAC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPCWAC.EMPLOYEE_NO");

            //Errol 2013-06-15
            strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

            strQry.AppendLine(" CASE ");

            //Errol 2015-02-12
            strQry.AppendLine(" WHEN E.EMPLOYEE_LAST_RUNDATE IS NULL ");

            strQry.AppendLine(" THEN DATEADD(DD,-40,'" + DateTime.Now.ToString("yyyy-MM-dd") + "')");

            strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y' ");

            strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

            strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

            strQry.AppendLine(" END ");
         
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT EPCWAC");
         
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON EPCWAC.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND EPCWAC.EMPLOYEE_NO = E.EMPLOYEE_NO");
            strQry.AppendLine(" AND EPCWAC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" WHERE EPCWAC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EPCWAC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            strQry.AppendLine(" AND EPCWAC.AUTHORISED_IND = 'Y'");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Authorised", parint64CompanyNo);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_Day_Timesheets_Records(Int64 parint64CompanyNo, string parstrPayCategoryType, Int64 parint64CurrentUserNo, string parstrCurrentUserAccessInd, string parstrDate)
        {
            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();

            //strQry = DayBlank_SQL(parint64CompanyNo, parstrPayCategoryType, parstrCurrentUserAccessInd, parint64CurrentUserNo, parstrDate);

            //clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "DayBlank", parint64CompanyNo);

            strQry = DayTotal_SQL(parint64CompanyNo, parstrPayCategoryType,-1, parstrCurrentUserAccessInd, parint64CurrentUserNo, parstrDate,"");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "DayTotal", parint64CompanyNo);

            strQry = TimeSheet_SQL(parint64CompanyNo, parstrPayCategoryType,-1, parstrCurrentUserAccessInd, parint64CurrentUserNo, parstrDate,"");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "TimeSheet", parint64CompanyNo);

            strQry = Break_SQL(parint64CompanyNo, parstrPayCategoryType,-1, parstrCurrentUserAccessInd, parint64CurrentUserNo, parstrDate,"");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Break", parint64CompanyNo);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Update_TimeSheet_Records(Int64 parint64CompanyNo,string parstrPayCategoryType,int parintPayCategoryNo, string parType, string parstrTypeValue, byte[] parbyteDataSet, Int64 int64CurrentUserNo,string parstrCurrentUserAccessInd)
        {
            DataSet DataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);
            DataSet DataSetTemp = new System.Data.DataSet();
            DataSet DataSetReturn = new System.Data.DataSet();

            int intTimesheetBreakDeleteId = -1;

            StringBuilder strQry = new StringBuilder();
            StringBuilder strQryInsert = new StringBuilder();
            
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

                    if (parstrPayCategoryType == "W")
                    {
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT");
                    }
                    else
                    {
                        if (parstrPayCategoryType == "S")
                        {
                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT");
                        }
                        else
                        {
                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT");
                        }
                    }

                    strQry.AppendLine(" WHERE COMPANY_NO = " + DataSet.Tables["TimeSheet"].Rows[intRow]["COMPANY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables["TimeSheet"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["TimeSheet"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                    strQry.AppendLine(" AND TIMESHEET_DATE = '" + Convert.ToDateTime(DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_DATE"]).ToString("yyyy-MM-dd") + "'");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSetTemp, "TimesheetTemp", parint64CompanyNo);

                    strQry.Clear();

                    if (parstrPayCategoryType == "W")
                    {
                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT");
                    }
                    else
                    {
                        if (parstrPayCategoryType == "S")
                        {
                            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT");
                        }
                        else
                        {
                            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT");
                        }
                    }

                    //Added to be Passed back To Client Layer
                    DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_SEQ"] = Convert.ToInt32(DataSetTemp.Tables["TimesheetTemp"].Rows[0]["MAX_TIMESHEET_SEQ"]);

                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",TIMESHEET_DATE");
                    strQry.AppendLine(",TIMESHEET_SEQ");
                    strQry.AppendLine(",TIMESHEET_TIME_IN_MINUTES");
                    strQry.AppendLine(",USER_NO_TIME_IN");
                    strQry.AppendLine(",TIMESHEET_TIME_OUT_MINUTES");
                    strQry.AppendLine(",USER_NO_TIME_OUT)");
                    strQry.AppendLine(" VALUES ");
                    strQry.AppendLine("(" + DataSet.Tables["TimeSheet"].Rows[intRow]["COMPANY_NO"].ToString());
                    strQry.AppendLine("," + DataSet.Tables["TimeSheet"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                    strQry.AppendLine("," + DataSet.Tables["TimeSheet"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(",'" + Convert.ToDateTime(DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_DATE"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine("," + DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_SEQ"].ToString());

                    if (DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_TIME_IN_MINUTES"] == System.DBNull.Value)
                    {
                        strQry.AppendLine(",Null");
                        strQry.AppendLine(",Null");
                    }
                    else
                    {
                        strQry.AppendLine("," + DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_TIME_IN_MINUTES"].ToString());
                        strQry.AppendLine("," + int64CurrentUserNo.ToString());
                    }

                    if (DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_TIME_OUT_MINUTES"] == System.DBNull.Value)
                    {
                        strQry.AppendLine(",Null");
                        strQry.AppendLine(",Null)");
                    }
                    else
                    {
                        strQry.AppendLine("," + DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_TIME_OUT_MINUTES"].ToString());
                        strQry.AppendLine("," + int64CurrentUserNo.ToString() + ")");
                    }
                }
                else
                {
                    if (DataSet.Tables["TimeSheet"].Rows[intRow].RowState == DataRowState.Modified)
                    {
                        strQry.Clear();

                        if (parstrPayCategoryType == "W")
                        {
                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT");
                        }
                        else
                        {
                            if (parstrPayCategoryType == "S")
                            {
                                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT");
                            }
                            else
                            {
                                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT");
                            }
                        }

                        strQry.AppendLine(" SET ");

                        if (DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_TIME_IN_MINUTES"] == System.DBNull.Value)
                        {
                            strQry.AppendLine(" TIMESHEET_TIME_IN_MINUTES = Null");
                        }
                        else
                        {
                            strQry.AppendLine(" TIMESHEET_TIME_IN_MINUTES = " + DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_TIME_IN_MINUTES"].ToString());

                            if (DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_TIME_IN_MINUTES", DataRowVersion.Original].ToString() != DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_TIME_IN_MINUTES", DataRowVersion.Current].ToString())
                            {
                                strQry.AppendLine(",USER_NO_TIME_IN = " + int64CurrentUserNo.ToString());
                            }
                        }

                        if (DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_TIME_OUT_MINUTES"] == System.DBNull.Value)
                        {
                            strQry.AppendLine(",TIMESHEET_TIME_OUT_MINUTES = Null");
                        }
                        else
                        {
                            strQry.AppendLine(",TIMESHEET_TIME_OUT_MINUTES = " + DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_TIME_OUT_MINUTES"].ToString());

                            if (DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_TIME_OUT_MINUTES", DataRowVersion.Original].ToString() != DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_TIME_OUT_MINUTES", DataRowVersion.Current].ToString())
                            {
                                strQry.AppendLine(",USER_NO_TIME_OUT = " + int64CurrentUserNo.ToString());
                            }
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
                            if (intTimesheetBreakDeleteId == -1)
                            {
                                //2017-10-21
                                strQry.Clear();

                                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.TIMESHEET_BREAK_DELETE");
                                strQry.AppendLine("(USER_NO");
                                strQry.AppendLine(",TIMESHEET_BREAK_DELETE_DATETIME)");

                                strQry.AppendLine(" VALUES");

                                strQry.AppendLine("(" + int64CurrentUserNo);
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

                            if (parstrPayCategoryType == "W")
                            {
                                strQryInsert.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT_DELETE");
                            }
                            else
                            {
                                if (parstrPayCategoryType == "S")
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

                            strQryInsert.AppendLine(int64CurrentUserNo.ToString());
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
                            
                            strQry.Clear();

                            if (parstrPayCategoryType == "W")
                            {
                                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT");
                            }
                            else
                            {
                                if (parstrPayCategoryType == "S")
                                {
                                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT");
                                }
                                else
                                {
                                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT");
                                }
                            }

                            strQry.AppendLine(" WHERE COMPANY_NO = " + DataSet.Tables["TimeSheet"].Rows[intRow]["COMPANY_NO", DataRowVersion.Original].ToString());
                            strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables["TimeSheet"].Rows[intRow]["PAY_CATEGORY_NO", DataRowVersion.Original].ToString());
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["TimeSheet"].Rows[intRow]["EMPLOYEE_NO", DataRowVersion.Original].ToString());
                            strQry.AppendLine(" AND TIMESHEET_DATE = '" + Convert.ToDateTime(DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_DATE", DataRowVersion.Original]).ToString("yyyy-MM-dd") + "'");
                            strQry.AppendLine(" AND TIMESHEET_SEQ = " + DataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_SEQ", DataRowVersion.Original].ToString());
                            
                            //2017-10-21 - Insert into Backup Tables before Delete
                            strQryInsert.Append(strQry);
                            strQryInsert.Replace("DELETE ", "");

                            clsDBConnectionObjects.Execute_SQLCommand(strQryInsert.ToString(), parint64CompanyNo);
                        }
                    }
                }

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
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

                    if (parstrPayCategoryType == "W")
                    {
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT");
                    }
                    else
                    {
                        if (parstrPayCategoryType == "S")
                        {
                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT");
                        }
                        else
                        {
                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT");
                        }
                    }

                    strQry.AppendLine(" WHERE COMPANY_NO = " + DataSet.Tables["Break"].Rows[intRow]["COMPANY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Break"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                    strQry.AppendLine(" AND BREAK_DATE = '" + Convert.ToDateTime(DataSet.Tables["Break"].Rows[intRow]["BREAK_DATE"]).ToString("yyyy-MM-dd") + "'");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSetTemp, "BreakTemp", parint64CompanyNo);

                    //Added to be Passed back To Client Layer
                    DataSet.Tables["Break"].Rows[intRow]["BREAK_SEQ"] = Convert.ToInt32(DataSetTemp.Tables["BreakTemp"].Rows[0]["MAX_BREAK_SEQ"]);

                    strQry.Clear();

                    if (parstrPayCategoryType == "W")
                    {
                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT");
                    }
                    else
                    {
                        if (parstrPayCategoryType == "S")
                        {
                            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT");
                        }
                        else
                        {
                            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT");
                        }
                    }

                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",BREAK_DATE");
                    strQry.AppendLine(",BREAK_SEQ");
                    strQry.AppendLine(",BREAK_TIME_IN_MINUTES");
                    strQry.AppendLine(",USER_NO_TIME_IN");
                    strQry.AppendLine(",BREAK_TIME_OUT_MINUTES");
                    strQry.AppendLine(",USER_NO_TIME_OUT)");
                    strQry.AppendLine(" VALUES ");
                    strQry.AppendLine("(" + DataSet.Tables["Break"].Rows[intRow]["COMPANY_NO"].ToString());
                    strQry.AppendLine("," + DataSet.Tables["Break"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                    strQry.AppendLine("," + DataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(",'" + Convert.ToDateTime(DataSet.Tables["Break"].Rows[intRow]["BREAK_DATE"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine("," + DataSet.Tables["Break"].Rows[intRow]["BREAK_SEQ"].ToString());

                    if (DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_IN_MINUTES"] == System.DBNull.Value)
                    {
                        strQry.AppendLine(",Null");
                        strQry.AppendLine(",Null");
                    }
                    else
                    {
                        strQry.AppendLine("," + DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_IN_MINUTES"].ToString());
                        strQry.AppendLine("," + int64CurrentUserNo.ToString());
                    }

                    if (DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_OUT_MINUTES"] == System.DBNull.Value)
                    {
                        strQry.AppendLine(",Null");
                        strQry.AppendLine(",Null)");
                    }
                    else
                    {
                        strQry.AppendLine("," + DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_OUT_MINUTES"].ToString());
                        strQry.AppendLine("," + int64CurrentUserNo.ToString() + ")");
                    }
                }
                else
                {
                    if (DataSet.Tables["Break"].Rows[intRow].RowState == DataRowState.Modified)
                    {
                        strQry.Clear();

                        if (parstrPayCategoryType == "W")
                        {
                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT");
                        }
                        else
                        {
                            if (parstrPayCategoryType == "S")
                            {
                                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT");
                            }
                            else
                            {
                                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT");
                            }
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
                            if (intTimesheetBreakDeleteId == -1)
                            {
                                //2017-10-21
                                strQry.Clear();

                                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.TIMESHEET_BREAK_DELETE");
                                strQry.AppendLine("(USER_NO");
                                strQry.AppendLine(",TIMESHEET_BREAK_DELETE_DATETIME)");

                                strQry.AppendLine(" VALUES");

                                strQry.AppendLine("(" + int64CurrentUserNo);
                                strQry.AppendLine(",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')");

                                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                                strQry.Clear();

                                strQry.AppendLine(" SELECT ");
                                strQry.AppendLine(" MAX(TIMESHEET_BREAK_DELETE_ID) AS TIMESHEET_BREAK_DELETE_ID");

                                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.TIMESHEET_BREAK_DELETE");

                                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSetTemp, "TimesheetBreakDelete", parint64CompanyNo);

                                intTimesheetBreakDeleteId = Convert.ToInt32(DataSetTemp.Tables["TimesheetBreakDelete"].Rows[0]["TIMESHEET_BREAK_DELETE_ID"]);
                            }
                            
                            //2017-10-21
                            strQryInsert.Clear();

                            if (parstrPayCategoryType == "W")
                            {
                                strQryInsert.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT_DELETE");
                            }
                            else
                            {
                                if (parstrPayCategoryType == "S")
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

                            strQryInsert.AppendLine(int64CurrentUserNo.ToString());
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
                            
                            strQry.Clear();

                            if (parstrPayCategoryType == "W")
                            {
                                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT");
                            }
                            else
                            {
                                if (parstrPayCategoryType == "S")
                                {
                                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT");
                                }
                                else
                                {
                                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT");
                                }
                            }

                            strQry.AppendLine(" WHERE COMPANY_NO = " + DataSet.Tables["Break"].Rows[intRow]["COMPANY_NO", DataRowVersion.Original].ToString());
                            strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_NO", DataRowVersion.Original].ToString());
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Break"].Rows[intRow]["EMPLOYEE_NO", DataRowVersion.Original].ToString());
                            strQry.AppendLine(" AND BREAK_DATE = '" + Convert.ToDateTime(DataSet.Tables["Break"].Rows[intRow]["BREAK_DATE", DataRowVersion.Original]).ToString("yyyy-MM-dd") + "'");
                            strQry.AppendLine(" AND BREAK_SEQ = " + DataSet.Tables["Break"].Rows[intRow]["BREAK_SEQ", DataRowVersion.Original].ToString());
                            
                            //2017-10-21 - Insert into Backup Tables before Delete
                            strQryInsert.Append(strQry);
                            strQryInsert.Replace("DELETE ", "");

                            clsDBConnectionObjects.Execute_SQLCommand(strQryInsert.ToString(), parint64CompanyNo);
                        }
                    }
                }

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            }

            DataSetTemp.Dispose();
            DataSetTemp = null;

            DataSet.AcceptChanges();

            strQry.Clear();
            
            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " +  parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            
            if (parType == "D")
            {
                //Date
                strQry = DayTotal_SQL(parint64CompanyNo, parstrPayCategoryType, parintPayCategoryNo, parstrCurrentUserAccessInd, int64CurrentUserNo, parstrTypeValue, "");
                
                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSetReturn, "DayTotal", parint64CompanyNo);

                strQry = TimeSheet_SQL(parint64CompanyNo, parstrPayCategoryType, parintPayCategoryNo, parstrCurrentUserAccessInd, int64CurrentUserNo, parstrTypeValue, "");
                
                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSetReturn, "TimeSheet", parint64CompanyNo);

                strQry = Break_SQL(parint64CompanyNo, parstrPayCategoryType, parintPayCategoryNo, parstrCurrentUserAccessInd, int64CurrentUserNo, parstrTypeValue, "");
                
                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSetReturn, "Break", parint64CompanyNo);
            }
            else
            {
                //Employee
                strQry = DayTotal_SQL(parint64CompanyNo, parstrPayCategoryType, parintPayCategoryNo, parstrCurrentUserAccessInd, int64CurrentUserNo, "", parstrTypeValue);

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSetReturn, "DayTotal", parint64CompanyNo);

                strQry = TimeSheet_SQL(parint64CompanyNo, parstrPayCategoryType, parintPayCategoryNo, parstrCurrentUserAccessInd, int64CurrentUserNo, "", parstrTypeValue);

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSetReturn, "TimeSheet", parint64CompanyNo);

                strQry = Break_SQL(parint64CompanyNo, parstrPayCategoryType, parintPayCategoryNo, parstrCurrentUserAccessInd, int64CurrentUserNo, "", parstrTypeValue);

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSetReturn, "Break", parint64CompanyNo);
            }

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSetReturn);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public void Delete_PayCategory_Employee_TimeSheet_Records(Int64 parint64CurrentUserNo, string parstrCurrentUserAccessInd,string parstrDeleteType, Int64 parint64CompanyNo, int parintPayCategoryNo, string parstrPayCategoryType, int parintEmployeeNo, string parstrPeriodOption, string parstrWageRunExclude)
        {
            StringBuilder strQry = new StringBuilder();
            StringBuilder strQryInsert = new StringBuilder();
            DataSet DataSet = new DataSet();

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

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "TimesheetBreakDelete", parint64CompanyNo);

            int intTimesheetBreakDeleteId = Convert.ToInt32(DataSet.Tables["TimesheetBreakDelete"].Rows[0]["TIMESHEET_BREAK_DELETE_ID"]);
            
            strQryInsert.Clear();

            if (parstrPayCategoryType == "W")
            {
                strQryInsert.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT_DELETE");
            }
            else
            {
                if (parstrPayCategoryType == "S")
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
            
            strQry.Clear();

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(" DELETE ETC FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
            }
            else
            {
                if (parstrPayCategoryType == "S")
                {
                    strQry.AppendLine(" DELETE ETC FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC");
                }
                else
                {
                    strQry.AppendLine(" DELETE ETC FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC");
                }
            }

            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(SELECT ");
                strQry.AppendLine(" EMPLOYEE_NO ");
                strQry.AppendLine(",PAY_CATEGORY_NO");
            
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP ");
                strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "') AS USER_TABLE");

                strQry.AppendLine(" ON ETC.EMPLOYEE_NO = USER_TABLE.EMPLOYEE_NO");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = USER_TABLE.PAY_CATEGORY_NO");
            }
           
            if (parstrPeriodOption == "G"
                | parstrPeriodOption == "W"
                | parstrWageRunExclude != "") 
            {

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");

                strQry.AppendLine(" ON ETC.COMPANY_NO = PCPC.COMPANY_NO");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = PCPC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
                strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P'");

                if (parstrPeriodOption == "G"
                |  parstrWageRunExclude != "") 
                {
                    strQry.AppendLine(" AND ETC.TIMESHEET_DATE > PCPC.PAY_PERIOD_DATE");
                }
                else
                {
                    strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= PCPC.PAY_PERIOD_DATE");
                }
            }

            strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + parintPayCategoryNo);

            if (parstrDeleteType == "E")
            {
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = " + parintEmployeeNo);
            }

            //2017-10-21 - Insert into Backup Tables before Delete
            strQryInsert.Append(strQry);
            strQryInsert.Replace("DELETE ETC", "");

            clsDBConnectionObjects.Execute_SQLCommand(strQryInsert.ToString(), parint64CompanyNo);
            
            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            
            //2017-10-21
            strQryInsert.Clear();

            if (parstrPayCategoryType == "W")
            {
                strQryInsert.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT_DELETE");
            }
            else
            {
                if (parstrPayCategoryType == "S")
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
            strQryInsert.AppendLine(",EBC.COMPANY_NO");
            strQryInsert.AppendLine(",EBC.EMPLOYEE_NO ");
            strQryInsert.AppendLine(",EBC.PAY_CATEGORY_NO");
            strQryInsert.AppendLine(",EBC.BREAK_DATE");
            strQryInsert.AppendLine(",EBC.BREAK_SEQ");
            strQryInsert.AppendLine(",EBC.BREAK_TIME_IN_MINUTES");
            strQryInsert.AppendLine(",EBC.BREAK_TIME_OUT_MINUTES");
            strQryInsert.AppendLine(",EBC.CLOCKED_TIME_IN_MINUTES");
            strQryInsert.AppendLine(",EBC.CLOCKED_TIME_OUT_MINUTES");
            strQryInsert.AppendLine(",EBC.INCLUDED_IN_RUN_IND");
            strQryInsert.AppendLine(",EBC.INDICATOR");
            strQryInsert.AppendLine(",EBC.BREAK_ACCUM_MINUTES");
            strQryInsert.AppendLine(",EBC.USER_NO_TIME_IN");
            strQryInsert.AppendLine(",EBC.USER_NO_TIME_OUT");
            
            strQry.Clear();

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(" DELETE EBC FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT EBC");
            }
            else
            {
                if (parstrPayCategoryType == "S")
                {
                    strQry.AppendLine(" DELETE EBC FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT EBC");
                }
                else
                {
                    strQry.AppendLine(" DELETE EBC FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT EBC");
                }
            }

            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(SELECT ");
                strQry.AppendLine(" EMPLOYEE_NO ");
                strQry.AppendLine(",PAY_CATEGORY_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP ");
                strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "') AS USER_TABLE");

                strQry.AppendLine(" ON EBC.EMPLOYEE_NO = USER_TABLE.EMPLOYEE_NO");
                strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = USER_TABLE.PAY_CATEGORY_NO");
            }

            if (parstrPeriodOption == "G"
               | parstrPeriodOption == "W"
               | parstrWageRunExclude != "") 
            {

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");

                strQry.AppendLine(" ON EBC.COMPANY_NO = PCPC.COMPANY_NO");
                strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = PCPC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
                strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P'");

                if (parstrPeriodOption == "G"
                | parstrWageRunExclude != "") 
                {
                    strQry.AppendLine(" AND EBC.BREAK_DATE > PCPC.PAY_PERIOD_DATE");
                }
                else
                {
                    strQry.AppendLine(" AND EBC.BREAK_DATE <= PCPC.PAY_PERIOD_DATE");
                }
            }

            strQry.AppendLine(" WHERE EBC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = " + parintPayCategoryNo);

            if (parstrDeleteType == "E")
            {
                strQry.AppendLine(" AND EBC.EMPLOYEE_NO = " + parintEmployeeNo);
            }

            //2017-10-21 - Insert into Backup Tables before Delete
            strQryInsert.Append(strQry);
            strQryInsert.Replace("DELETE EBC", "");
            clsDBConnectionObjects.Execute_SQLCommand(strQryInsert.ToString(), parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }
    }
}
