using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Net;

namespace InteractPayroll
{
    public class busTimeAttendanceRun
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        string pvtstrClassName = "busTimeAttendanceRun";

        string pvtstrLogFileName = "";

        string pvtstrSmtpEmailAddressDescription = "";
        string pvtstrSmtpEmailAddress = "";
        string pvtstrSmtpEmailAddressPassword = "";
        string pvtstrSmtpHostName = "";
        int pvtintSmtpHostPort = 0;

        public busTimeAttendanceRun()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();

            if (AppDomain.CurrentDomain.GetData("LogFileName") != null)
            {
                pvtstrLogFileName = AppDomain.CurrentDomain.GetData("LogFileName").ToString();
            }
            else
            {
                pvtstrLogFileName = AppDomain.CurrentDomain.BaseDirectory + "RunTimeAttendanceWinService.txt";
            }

            if (AppDomain.CurrentDomain.GetData("SmtpEmailAddressDescription") != null)
            {
                pvtstrSmtpEmailAddressDescription = AppDomain.CurrentDomain.GetData("SmtpEmailAddressDescription").ToString();
            }

            if (AppDomain.CurrentDomain.GetData("SmtpEmailAddress") != null)
            {
                pvtstrSmtpEmailAddress = AppDomain.CurrentDomain.GetData("SmtpEmailAddress").ToString();
            }

            if (AppDomain.CurrentDomain.GetData("SmtpEmailAddressPassword") != null)
            {
                pvtstrSmtpEmailAddressPassword = AppDomain.CurrentDomain.GetData("SmtpEmailAddressPassword").ToString();
            }

            if (AppDomain.CurrentDomain.GetData("SmtpHostName") != null)
            {
                pvtstrSmtpHostName = AppDomain.CurrentDomain.GetData("SmtpHostName").ToString();
            }

            if (AppDomain.CurrentDomain.GetData("SmtpHostPort") != null)
            {
                pvtintSmtpHostPort = Convert.ToInt32(AppDomain.CurrentDomain.GetData("SmtpHostPort"));
            }
        }

        public byte[] Get_Form_Records(Int64 parInt64CompanyNo, string parstrFromProgram)
        {
            StringBuilder strQry = new StringBuilder();
            string strPayCategoryIn = "";
            DataSet DataSet = new DataSet();
            DataView dtvPublicHolidayDataView;
            DataView DataViewTimeSheetErrors;
            DataView DataViewTimeSheetLeave;
            
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            
            strQry.AppendLine(" AND RUN_TYPE = 'P'");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
            }

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

            if (DataSet.Tables["Temp"].Rows.Count == 0)
            {
                goto Get_Form_Records_Leave_Cleanup_Tables;
            }
            else
            {
                DataSet.Tables.Remove("Temp");
            }

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT");
            
            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" 'Time Attendance' AS PAYROLL_TYPE ");
            }
            else
            {
                strQry.AppendLine(" 'Wages' AS PAYROLL_TYPE ");
            }
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_NO > 0");
           
            strQry.AppendLine(" AND RUN_TYPE = 'P'");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
            }

            strQry.AppendLine(" ORDER BY 1 DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayrollType", parInt64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" C.COMPANY_NO");
            strQry.AppendLine(",C.COMPANY_DESC");
            strQry.AppendLine(",C.OVERTIME1_RATE");
            strQry.AppendLine(",C.OVERTIME2_RATE");
            strQry.AppendLine(",C.OVERTIME3_RATE");
            strQry.AppendLine(",'A' AS ACCESS_IND");
            strQry.AppendLine(",C.WAGE_RUN_IND");
            strQry.AppendLine(",C.SALARY_RUN_IND");
            strQry.AppendLine(",C.TIME_ATTENDANCE_RUN_IND");
            strQry.AppendLine(",'Y' AS TIMESHEETS_AUTHORISED_IND");
            strQry.AppendLine(",'Y' AS LEAVE_AUTHORISED_IND");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PPC");
            strQry.AppendLine(" ON C.COMPANY_NO = PPC.COMPANY_NO");
            strQry.AppendLine(" AND PPC.PAY_CATEGORY_NO > 0");
            
            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PPC.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PPC.PAY_CATEGORY_TYPE = 'W'");
            }

            strQry.AppendLine(" AND PPC.RUN_TYPE = 'P'");

            strQry.AppendLine(" WHERE C.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" C.COMPANY_DESC");
           
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", parInt64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PHC.COMPANY_NO");
            strQry.AppendLine(",PHC.PUBLIC_HOLIDAY_DATE");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_CURRENT PHC");

            strQry.AppendLine(" WHERE PHC.COMPANY_NO = " + parInt64CompanyNo);
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PHC.COMPANY_NO");
            strQry.AppendLine(",PHC.PUBLIC_HOLIDAY_DATE");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PaidHoliday", parInt64CompanyNo);
            
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PPC.COMPANY_NO");
            strQry.AppendLine(",PPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PC.LAST_UPLOAD_DATETIME");
            strQry.AppendLine(",PPC.PAY_PERIOD_DATE");
            strQry.AppendLine(",PPC.PAY_PERIOD_DATE_FROM");
            strQry.AppendLine(",PPC.PAY_PUBLIC_HOLIDAY_IND");
                     
            strQry.AppendLine(",'' AS ERROR_IND");
            strQry.AppendLine(",'' AS PUBLIC_HOLIDAY_FLAG_IND");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PPC");
            
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON PPC.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND PPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = 'W'");
            }
 
            strQry.AppendLine(" WHERE PPC.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PPC.PAY_CATEGORY_NO > 0");
            strQry.AppendLine(" AND PPC.RUN_TYPE = 'P'");
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PPC.COMPANY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategory", parInt64CompanyNo);
        
            for (int intRow = 0; intRow < DataSet.Tables["PayCategory"].Rows.Count; intRow++)
            {
                dtvPublicHolidayDataView = new DataView(DataSet.Tables["PaidHoliday"]
                    , "PUBLIC_HOLIDAY_DATE >= '" + Convert.ToDateTime(DataSet.Tables["PayCategory"].Rows[intRow]["PAY_PERIOD_DATE_FROM"]).ToString("yyyy-MM-dd") + "' AND PUBLIC_HOLIDAY_DATE <= '" + Convert.ToDateTime(DataSet.Tables["PayCategory"].Rows[intRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'"
                    , ""
                    , DataViewRowState.CurrentRows);

                if (dtvPublicHolidayDataView.Count > 0)
                {
                    DataSet.Tables["PayCategory"].Rows[intRow]["PUBLIC_HOLIDAY_FLAG_IND"] = "Y";
                }

                if (intRow == 0)
                {
                    strPayCategoryIn = DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_NO"].ToString();
                }
                else
                {
                    strPayCategoryIn += "," + DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_NO"].ToString();
                }
            }

            bool blnErrorsInTimesheets = false;

            if (DataSet.Tables["PayCategory"].Rows.Count > 0)
            {
                string strPayCategoryType = "W";

                if (parstrFromProgram == "X")
                {
                    strPayCategoryType = "T";
                }

                Get_TimeSheet_Totals(DataSet, parInt64CompanyNo, Convert.ToDateTime(DataSet.Tables["PayCategory"].Rows[0]["PAY_PERIOD_DATE"]), strPayCategoryIn, strPayCategoryType);

                for (int intRow = 0; intRow < DataSet.Tables["PayCategory"].Rows.Count; intRow++)
                {
                    DataViewTimeSheetErrors = null;
                    DataViewTimeSheetErrors = new DataView(DataSet.Tables["TimeSheetDayTotals"],
                    "PAY_CATEGORY_NO = " + DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_NO"].ToString() + " AND INDICATOR = 'X'",
                    "",
                    DataViewRowState.CurrentRows);

                    if (DataViewTimeSheetErrors.Count > 0)
                    {
                        DataSet.Tables["PayCategory"].Rows[intRow]["ERROR_IND"] = "Y";
                        blnErrorsInTimesheets = true;
                    }
                }
            }

            Get_Form_Records_ReRead:

            //2017-07-11
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PAYROLL_RUN_QUEUE_IND");

            strQry.AppendLine(" FROM InteractPayroll.dbo.PAYROLL_RUN_QUEUE");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
            }

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayrollQueue", parInt64CompanyNo);

            if (blnErrorsInTimesheets == false)
            {
                if (DataSet.Tables["PayrollQueue"].Rows.Count > 0)
                {
                    if (DataSet.Tables["PayrollQueue"].Rows[0]["PAYROLL_RUN_QUEUE_IND"].ToString() == "E")
                    {
                        strQry.Clear();
                        
                        strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.PAYROLL_RUN_QUEUE");

                        strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                        if (parstrFromProgram == "X")
                        {
                            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");
                        }
                        else
                        {
                            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                        }

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                        DataSet.Tables.Remove("PayrollQueue");
                        
                        goto Get_Form_Records_ReRead;
                    }
                }
            }
            
            DataSet.AcceptChanges();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PCWC.COMPANY_NO ");
            strQry.AppendLine(",PCWC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PCWC.WEEK_DATE");
            strQry.AppendLine(",PCWC.WEEK_DATE_FROM");
            strQry.AppendLine(",PPC.PAIDHOLIDAY_RATE");
            strQry.AppendLine(",PPC.TOTAL_DAILY_TIME_OVERTIME");
            strQry.AppendLine(",PPC.EXCEPTION_SHIFT_ABOVE_PERCENT");
            strQry.AppendLine(",PPC.EXCEPTION_SHIFT_BELOW_PERCENT");
            strQry.AppendLine(",PCWC.MON_TIME_MINUTES");
            strQry.AppendLine(",PCWC.TUE_TIME_MINUTES");
            strQry.AppendLine(",PCWC.WED_TIME_MINUTES");
            strQry.AppendLine(",PCWC.THU_TIME_MINUTES");
            strQry.AppendLine(",PCWC.FRI_TIME_MINUTES");
            strQry.AppendLine(",PCWC.SAT_TIME_MINUTES");
            strQry.AppendLine(",PCWC.SUN_TIME_MINUTES");
            strQry.AppendLine(",PCWC.OVERTIME1_MINUTES");
            strQry.AppendLine(",PCWC.OVERTIME2_MINUTES");
            strQry.AppendLine(",PCWC.OVERTIME3_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_SUN_ABOVE_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_SUN_BELOW_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_MON_ABOVE_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_MON_BELOW_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_TUE_ABOVE_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_TUE_BELOW_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_WED_ABOVE_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_WED_BELOW_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_THU_ABOVE_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_THU_BELOW_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_FRI_ABOVE_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_FRI_BELOW_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_SAT_ABOVE_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_SAT_BELOW_MINUTES");
            strQry.AppendLine(",PCWC.PAIDHOLIDAY_MINUTES1");
            strQry.AppendLine(",PCWC.PAIDHOLIDAY_DAY1");
            strQry.AppendLine(",PCWC.PAIDHOLIDAY_MINUTES2");
            strQry.AppendLine(",PCWC.PAIDHOLIDAY_DAY2");
            strQry.AppendLine(",PCWC.PAIDHOLIDAY_MINUTES3");
            strQry.AppendLine(",PCWC.PAIDHOLIDAY_DAY3");
            strQry.AppendLine(",PCWC.PAIDHOLIDAY_MINUTES4");
            strQry.AppendLine(",PCWC.PAIDHOLIDAY_DAY4");
            strQry.AppendLine(",PCWC.PAIDHOLIDAY_MINUTES5");
            strQry.AppendLine(",PCWC.PAIDHOLIDAY_DAY5");

            strQry.AppendLine(",ISNULL(DAY_TABLE.INDICATOR, 'N') AS ERROR_IND");
       
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_CURRENT PCWC");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PPC");
            strQry.AppendLine(" ON PCWC.COMPANY_NO = PPC.COMPANY_NO ");
            strQry.AppendLine(" AND PCWC.PAY_CATEGORY_NO = PPC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PCWC.PAY_CATEGORY_TYPE = PPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PPC.RUN_TYPE = 'P'");
            
            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT AS DAY_TABLE ");
            }
            else
            {
                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT AS DAY_TABLE ");
            }
            
            strQry.AppendLine(" ON PCWC.COMPANY_NO = DAY_TABLE.COMPANY_NO");
            strQry.AppendLine(" AND PCWC.PAY_CATEGORY_NO = DAY_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND DAY_TABLE.TIMESHEET_DATE >= PCWC.WEEK_DATE_FROM");
            strQry.AppendLine(" AND DAY_TABLE.TIMESHEET_DATE <= PCWC.WEEK_DATE");
            //Error
            strQry.AppendLine(" AND DAY_TABLE.INDICATOR = 'X'");

            strQry.AppendLine(" WHERE PCWC.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PCWC.PAY_CATEGORY_NO > 0");
            
            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PCWC.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PCWC.PAY_CATEGORY_TYPE = 'W'");
            }

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PCWC.COMPANY_NO ");
            strQry.AppendLine(",PCWC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PCWC.WEEK_DATE");
            strQry.AppendLine(",PCWC.WEEK_DATE_FROM");
            strQry.AppendLine(",PPC.PAIDHOLIDAY_RATE");
            strQry.AppendLine(",PPC.TOTAL_DAILY_TIME_OVERTIME");
            strQry.AppendLine(",PPC.EXCEPTION_SHIFT_ABOVE_PERCENT");
            strQry.AppendLine(",PPC.EXCEPTION_SHIFT_BELOW_PERCENT");
            strQry.AppendLine(",PCWC.MON_TIME_MINUTES");
            strQry.AppendLine(",PCWC.TUE_TIME_MINUTES");
            strQry.AppendLine(",PCWC.WED_TIME_MINUTES");
            strQry.AppendLine(",PCWC.THU_TIME_MINUTES");
            strQry.AppendLine(",PCWC.FRI_TIME_MINUTES");
            strQry.AppendLine(",PCWC.SAT_TIME_MINUTES");
            strQry.AppendLine(",PCWC.SUN_TIME_MINUTES");
            strQry.AppendLine(",PCWC.OVERTIME1_MINUTES");
            strQry.AppendLine(",PCWC.OVERTIME2_MINUTES");
            strQry.AppendLine(",PCWC.OVERTIME3_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_SUN_ABOVE_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_SUN_BELOW_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_MON_ABOVE_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_MON_BELOW_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_TUE_ABOVE_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_TUE_BELOW_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_WED_ABOVE_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_WED_BELOW_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_THU_ABOVE_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_THU_BELOW_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_FRI_ABOVE_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_FRI_BELOW_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_SAT_ABOVE_MINUTES");
            strQry.AppendLine(",PCWC.EXCEPTION_SAT_BELOW_MINUTES");
            strQry.AppendLine(",PCWC.PAIDHOLIDAY_MINUTES1");
            strQry.AppendLine(",PCWC.PAIDHOLIDAY_DAY1");
            strQry.AppendLine(",PCWC.PAIDHOLIDAY_MINUTES2");
            strQry.AppendLine(",PCWC.PAIDHOLIDAY_DAY2");
            strQry.AppendLine(",PCWC.PAIDHOLIDAY_MINUTES3");
            strQry.AppendLine(",PCWC.PAIDHOLIDAY_DAY3");
            strQry.AppendLine(",PCWC.PAIDHOLIDAY_MINUTES4");
            strQry.AppendLine(",PCWC.PAIDHOLIDAY_DAY4");
            strQry.AppendLine(",PCWC.PAIDHOLIDAY_MINUTES5");
            strQry.AppendLine(",PCWC.PAIDHOLIDAY_DAY5");
            strQry.AppendLine(",DAY_TABLE.INDICATOR");
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PCWC.COMPANY_NO ");
            strQry.AppendLine(",PCWC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PCWC.WEEK_DATE");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategoryWeek", parInt64CompanyNo);

            if (DataSet.Tables["TimeSheetDayTotals"] != null)
            {
                for (int intRow = 0; intRow < DataSet.Tables["PayCategoryWeek"].Rows.Count; intRow++)
                {
                    DataViewTimeSheetLeave = null;
                    DataViewTimeSheetLeave = new DataView(DataSet.Tables["TimeSheetDayTotals"],
                    "PAY_CATEGORY_NO = " + DataSet.Tables["PayCategoryWeek"].Rows[intRow]["PAY_CATEGORY_NO"].ToString() + " AND DAY_DATE >= '" + Convert.ToDateTime(DataSet.Tables["PayCategoryWeek"].Rows[intRow]["WEEK_DATE_FROM"]).ToString("yyyy-MM-dd") + "' AND DAY_DATE <= '" + Convert.ToDateTime(DataSet.Tables["PayCategoryWeek"].Rows[intRow]["WEEK_DATE"]).ToString("yyyy-MM-dd") + "' AND LEAVE_INDICATOR = 'Y'",
                    "",
                    DataViewRowState.CurrentRows);

                    if (DataViewTimeSheetLeave.Count > 0)
                    {
                        DataSet.Tables["PayCategoryWeek"].Rows[intRow]["ERROR_IND"] = "X";
                        blnErrorsInTimesheets = true;
                    }
                }
            }

            //2017-04-26
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PCWC.PAY_CATEGORY_NO");
            strQry.AppendLine(",DAY_TABLE.TIMESHEET_DATE");
            strQry.AppendLine(",MAX(ISNULL(DAY_TABLE.INDICATOR,'')) AS INDICATOR");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_CURRENT PCWC");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PPC");
            strQry.AppendLine(" ON PCWC.COMPANY_NO = PPC.COMPANY_NO ");
            strQry.AppendLine(" AND PCWC.PAY_CATEGORY_NO = PPC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PCWC.PAY_CATEGORY_TYPE = PPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PPC.RUN_TYPE = 'P'");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT AS DAY_TABLE ");
            }
            else
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT AS DAY_TABLE ");
            }

            strQry.AppendLine(" ON PCWC.COMPANY_NO = DAY_TABLE.COMPANY_NO");
            strQry.AppendLine(" AND PCWC.PAY_CATEGORY_NO = DAY_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND DAY_TABLE.TIMESHEET_DATE >= PCWC.WEEK_DATE_FROM");
            strQry.AppendLine(" AND DAY_TABLE.TIMESHEET_DATE <= PCWC.WEEK_DATE");
         
            strQry.AppendLine(" WHERE PCWC.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PCWC.PAY_CATEGORY_NO > 0");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PCWC.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PCWC.PAY_CATEGORY_TYPE = 'W'");
            }

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PCWC.PAY_CATEGORY_NO");
            strQry.AppendLine(",DAY_TABLE.TIMESHEET_DATE");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PCWC.PAY_CATEGORY_NO");
            strQry.AppendLine(",DAY_TABLE.TIMESHEET_DATE");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategoryDayError", parInt64CompanyNo);

            for (int intRow = 0; intRow < DataSet.Tables["PayCategoryDayError"].Rows.Count; intRow++)
            {
                DataViewTimeSheetLeave = null;
                DataViewTimeSheetLeave = new DataView(DataSet.Tables["TimeSheetDayTotals"],
                "PAY_CATEGORY_NO = " + DataSet.Tables["PayCategoryDayError"].Rows[intRow]["PAY_CATEGORY_NO"].ToString() + " AND DAY_DATE = '" + Convert.ToDateTime(DataSet.Tables["PayCategoryDayError"].Rows[intRow]["TIMESHEET_DATE"]).ToString("yyyy-MM-dd") + "' AND LEAVE_INDICATOR = 'Y'",
                "",
                DataViewRowState.CurrentRows);

                if (DataViewTimeSheetLeave.Count > 0)
                {
                    DataSet.Tables["PayCategoryDayError"].Rows[intRow]["INDICATOR"] = "X";
                    blnErrorsInTimesheets = true;
                }
            }
        
            DataView DataViewPayCategoryDayError = new DataView(DataSet.Tables["PayCategoryDayError"],
            "INDICATOR <> 'X'",
            "",
            DataViewRowState.CurrentRows);

            //This Needs to Be Tested
            for (int intRow = 0; intRow < DataViewPayCategoryDayError.Count; intRow++)
            {
                DataViewPayCategoryDayError[intRow].Delete();

                intRow -= 1;
            }

            //Get Number of Cost Centre/Employee Records That Need to be Authorised 
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PCEWAC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",PCEWAC.LEVEL_NO ");
            strQry.AppendLine(",COUNT(DISTINCT PCEWAC.EMPLOYEE_NO) AS EMPLOYEE_COUNT");
         
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT PCEWAC ");

            strQry.AppendLine(" WHERE PCEWAC.COMPANY_NO = " + parInt64CompanyNo);

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PCEWAC.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PCEWAC.PAY_CATEGORY_TYPE = 'W'");
            }

            
            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PCEWAC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",PCEWAC.LEVEL_NO ");
           
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "TimeAuthoriseEmployeeCount", parInt64CompanyNo);

            for (int intRow = 0; intRow < DataSet.Tables["TimeAuthoriseEmployeeCount"].Rows.Count; intRow++)
            {
                if (DataSet.Tables["Temp"] != null)
                {
                    DataSet.Tables["Temp"].Rows.Clear();
                }
                
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EMPLOYEE_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT ");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables["TimeAuthoriseEmployeeCount"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(" AND LEVEL_NO = " + DataSet.Tables["TimeAuthoriseEmployeeCount"].Rows[intRow]["LEVEL_NO"].ToString());

                if (parstrFromProgram == "X")
                {
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");
                }
                else
                {
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                }

                strQry.AppendLine(" AND AUTHORISED_IND = 'Y'");
                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" EMPLOYEE_NO");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

                if (Convert.ToInt32(DataSet.Tables["TimeAuthoriseEmployeeCount"].Rows[intRow]["EMPLOYEE_COUNT"]) != DataSet.Tables["Temp"].Rows.Count)
                {
                    DataSet.Tables["Company"].Rows[0]["TIMESHEETS_AUTHORISED_IND"] = "N";

                    goto Get_Form_Records_Timesheet_Cleanup_Tables;
                }
            }

            //NB Cleanup Temp Tables 
        Get_Form_Records_Timesheet_Cleanup_Tables:

            if (DataSet.Tables["TimeAuthoriseEmployeeCount"] != null)
            {
                DataSet.Tables.Remove("TimeAuthoriseEmployeeCount");
            }

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EPCLAC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",EPCLAC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",EPCLAC.EARNING_NO ");
            strQry.AppendLine(",EPCLAC.LEVEL_NO ");
            strQry.AppendLine(",COUNT(DISTINCT EPCLAC.EMPLOYEE_NO) AS EMPLOYEE_COUNT");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT EPCLAC ");

            strQry.AppendLine(" WHERE EPCLAC.COMPANY_NO = " + parInt64CompanyNo);

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND EPCLAC.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND EPCLAC.PAY_CATEGORY_TYPE = 'W'");
            }

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" EPCLAC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",EPCLAC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",EPCLAC.EARNING_NO ");
            strQry.AppendLine(",EPCLAC.LEVEL_NO ");
           
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "LeaveAuthoriseEmployeeCount", parInt64CompanyNo);

            for (int intRow = 0; intRow < DataSet.Tables["LeaveAuthoriseEmployeeCount"].Rows.Count; intRow++)
            {
                if (DataSet.Tables["Temp"] != null)
                {
                    DataSet.Tables["Temp"].Rows.Clear();
                }

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EMPLOYEE_NO");
                    
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT ");
                    
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables["LeaveAuthoriseEmployeeCount"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["LeaveAuthoriseEmployeeCount"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                strQry.AppendLine(" AND EARNING_NO = " + DataSet.Tables["LeaveAuthoriseEmployeeCount"].Rows[intRow]["EARNING_NO"].ToString());
                strQry.AppendLine(" AND LEVEL_NO = " + DataSet.Tables["LeaveAuthoriseEmployeeCount"].Rows[intRow]["LEVEL_NO"].ToString());
                strQry.AppendLine(" AND AUTHORISED_IND = 'Y'");
                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" EMPLOYEE_NO");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

                if (Convert.ToInt32(DataSet.Tables["LeaveAuthoriseEmployeeCount"].Rows[intRow]["EMPLOYEE_COUNT"]) != DataSet.Tables["Temp"].Rows.Count)
                {
                    DataSet.Tables["Company"].Rows[0]["LEAVE_AUTHORISED_IND"] = "N";

                    goto Get_Form_Records_Leave_Cleanup_Tables;
                }
            }
            
            //NB Cleanup Temp Tables 
        Get_Form_Records_Leave_Cleanup_Tables:

            if (DataSet.Tables["LeaveAuthoriseEmployeeCount"] != null)
            {
                DataSet.Tables.Remove("LeaveAuthoriseEmployeeCount");
            }

            if (DataSet.Tables["Temp"] != null)
            {
                DataSet.Tables.Remove("Temp");
            }

            if (DataSet.Tables["TimeSheetDayTotals"] != null)
            {
                DataSet.Tables.Remove("TimeSheetDayTotals");
            }

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public void Update_Records(Int64 parInt64CompanyNo,string parstrPayrollType, byte[] parbyteDataSet)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            StringBuilder strQry = new StringBuilder();

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_CURRENT");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" MON_TIME_MINUTES = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["MON_TIME_MINUTES"]));
            strQry.AppendLine(",TUE_TIME_MINUTES = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["TUE_TIME_MINUTES"]));
            strQry.AppendLine(",WED_TIME_MINUTES = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["WED_TIME_MINUTES"]));
            strQry.AppendLine(",THU_TIME_MINUTES = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["THU_TIME_MINUTES"]));
            strQry.AppendLine(",FRI_TIME_MINUTES = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["FRI_TIME_MINUTES"]));
            strQry.AppendLine(",SAT_TIME_MINUTES = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["SAT_TIME_MINUTES"]));
            strQry.AppendLine(",SUN_TIME_MINUTES = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["SUN_TIME_MINUTES"]));
            strQry.AppendLine(",OVERTIME1_MINUTES = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["OVERTIME1_MINUTES"]));
            strQry.AppendLine(",OVERTIME2_MINUTES = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["OVERTIME2_MINUTES"]));
            strQry.AppendLine(",OVERTIME3_MINUTES = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["OVERTIME3_MINUTES"]));
            strQry.AppendLine(",PAIDHOLIDAY_MINUTES1 = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["PAIDHOLIDAY_MINUTES1"]));
            strQry.AppendLine(",PAIDHOLIDAY_MINUTES2 = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["PAIDHOLIDAY_MINUTES2"]));
            strQry.AppendLine(",PAIDHOLIDAY_MINUTES3 = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["PAIDHOLIDAY_MINUTES3"]));
            strQry.AppendLine(",PAIDHOLIDAY_MINUTES4 = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["PAIDHOLIDAY_MINUTES4"]));
            strQry.AppendLine(",PAIDHOLIDAY_MINUTES5 = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["PAIDHOLIDAY_MINUTES5"]));
            strQry.AppendLine(",EXCEPTION_SUN_ABOVE_MINUTES = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["EXCEPTION_SUN_ABOVE_MINUTES"]));
            strQry.AppendLine(",EXCEPTION_SUN_BELOW_MINUTES = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["EXCEPTION_SUN_BELOW_MINUTES"]));
            strQry.AppendLine(",EXCEPTION_MON_ABOVE_MINUTES = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["EXCEPTION_MON_ABOVE_MINUTES"]));
            strQry.AppendLine(",EXCEPTION_MON_BELOW_MINUTES = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["EXCEPTION_MON_BELOW_MINUTES"]));
            strQry.AppendLine(",EXCEPTION_TUE_ABOVE_MINUTES = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["EXCEPTION_TUE_ABOVE_MINUTES"]));
            strQry.AppendLine(",EXCEPTION_TUE_BELOW_MINUTES = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["EXCEPTION_TUE_BELOW_MINUTES"]));
            strQry.AppendLine(",EXCEPTION_WED_ABOVE_MINUTES = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["EXCEPTION_WED_ABOVE_MINUTES"]));
            strQry.AppendLine(",EXCEPTION_WED_BELOW_MINUTES = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["EXCEPTION_WED_BELOW_MINUTES"]));
            strQry.AppendLine(",EXCEPTION_THU_ABOVE_MINUTES = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["EXCEPTION_THU_ABOVE_MINUTES"]));
            strQry.AppendLine(",EXCEPTION_THU_BELOW_MINUTES = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["EXCEPTION_THU_BELOW_MINUTES"]));
            strQry.AppendLine(",EXCEPTION_FRI_ABOVE_MINUTES = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["EXCEPTION_FRI_ABOVE_MINUTES"]));
            strQry.AppendLine(",EXCEPTION_FRI_BELOW_MINUTES = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["EXCEPTION_FRI_BELOW_MINUTES"]));
            strQry.AppendLine(",EXCEPTION_SAT_ABOVE_MINUTES = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["EXCEPTION_SAT_ABOVE_MINUTES"]));
            strQry.AppendLine(",EXCEPTION_SAT_BELOW_MINUTES = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["EXCEPTION_SAT_BELOW_MINUTES"]));
            strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["COMPANY_NO"]));
            strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["PAY_CATEGORY_NO"]));
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
            strQry.AppendLine(" AND WEEK_DATE = '" + Convert.ToDateTime(parDataSet.Tables[0].Rows[0]["WEEK_DATE"]).ToString("yyyy-MM-dd") + "'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.COMPANY");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" WAGE_RUN_IND = 'N'");
            strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["COMPANY_NO"]));

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }

        public byte[] Reset_Run(Int64 parInt64CompanyNo,string parstrPayrollType, string parstrFromProgram)
        {
            string strClassNameFunctionAndParameters = pvtstrClassName + " Reset_Run CompanyNo=" + parInt64CompanyNo + ",PayrollType=" + parstrPayrollType + ",FromProgram=" + parstrFromProgram;
            
            StringBuilder strQry = new StringBuilder();

            try
            {
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.COMPANY");
                strQry.AppendLine(" SET ");

                if (parstrPayrollType == "W")
                {
                    strQry.AppendLine(" WAGE_RUN_IND = 'N'");
                }
                else
                {
                    if (parstrPayrollType == "S")
                    {
                        strQry.AppendLine(" SALARY_RUN_IND = 'N'");
                    }
                    else
                    {
                        strQry.AppendLine(" TIME_ATTENDANCE_RUN_IND = 'N'");
                    }
                }

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                
                strQry.Clear();

                strQry.AppendLine(" UPDATE EEC");

                strQry.AppendLine(" SET ");

                strQry.AppendLine(" MINUTES = 0");
                strQry.AppendLine(",MINUTES_ROUNDED = 0");

                strQry.AppendLine(",HOURS_DECIMAL = 0");
                strQry.AppendLine(",HOURS_DECIMAL_OTHER_VALUE = 0");
                strQry.AppendLine(",HOURS_DECIMAL_ORIGINAL = 0");

                strQry.AppendLine(",DAY_DECIMAL_OTHER_VALUE = 0");
                strQry.AppendLine(",TOTAL = 0");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");

                //2013-09-23
                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_BATCH_TEMP EEBT");
                strQry.AppendLine(" ON EEC.COMPANY_NO = EEBT.COMPANY_NO ");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = EEBT.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EEC.EARNING_NO = EEBT.EARNING_NO");
                strQry.AppendLine(" AND EEC.EMPLOYEE_NO = EEBT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EEBT.PROCESS_NO = 0");

                strQry.AppendLine(" WHERE EEC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND EEC.RUN_TYPE = 'P'");

                //2014-09-06
                strQry.AppendLine(" AND EEC.EARNING_NO < 10");
                strQry.AppendLine(" AND EEC.EARNING_NO > 199");

                //NO Records Exist
                strQry.AppendLine(" AND EEBT.COMPANY_NO IS NULL ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                
                strQry.Clear();

                strQry.AppendLine(" UPDATE EDC");

                strQry.AppendLine(" SET ");

                strQry.AppendLine(" TOTAL = 0");
                strQry.AppendLine(",TOTAL_ORIGINAL = 0");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT EDC");

                //2013-09-26
                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_BATCH_TEMP EDBT");
                strQry.AppendLine(" ON EDC.COMPANY_NO = EDBT.COMPANY_NO ");
                strQry.AppendLine(" AND EDC.PAY_CATEGORY_TYPE = EDBT.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EDC.DEDUCTION_NO = EDBT.DEDUCTION_NO ");
                strQry.AppendLine(" AND EDC.DEDUCTION_SUB_ACCOUNT_NO = EDBT.DEDUCTION_SUB_ACCOUNT_NO ");
                strQry.AppendLine(" AND EDC.EMPLOYEE_NO = EDBT.EMPLOYEE_NO ");
                //Next Run
                strQry.AppendLine(" AND EDBT.PROCESS_NO = 0 ");

                strQry.AppendLine(" WHERE EDC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EDC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND EDC.RUN_TYPE = 'P'");

                //NO Records Exist
                strQry.AppendLine(" AND EDBT.COMPANY_NO IS NULL ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                
                //2013-01-07
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT");
                strQry.AppendLine(" SET ");

                strQry.AppendLine(" CLOSE_IND = 'N'");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND RUN_TYPE = 'P'");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                
                strQry.Clear();

                if (parstrPayrollType == "W")
                {
                    strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ");
                    strQry.AppendLine(" DISABLE TRIGGER tgr_EMPLOYEE_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table ");
                }
                else
                {

                    strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ");
                    strQry.AppendLine(" DISABLE TRIGGER tgr_EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_Table ");
                }

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                
                strQry.Clear();

                //Set Timesheets Excluded from Run to 'N'
                strQry.AppendLine(" UPDATE ETC ");
                strQry.AppendLine(" SET ");

                strQry.AppendLine(" INCLUDED_IN_RUN_IND = 'N' ");

                if (parstrPayrollType == "W")
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC");
                }

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");
                strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO");
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC ");
                strQry.AppendLine(" ON ETC.COMPANY_NO = PCPC.COMPANY_NO");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = PCPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = PCPC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P' ");

                strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parInt64CompanyNo);

                strQry.AppendLine(" AND ETC.TIMESHEET_DATE > ");
                strQry.AppendLine(" CASE ");

                strQry.AppendLine(" WHEN E.FIRST_RUN_COMPLETED_IND = 'Y' ");
                strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE ");

                strQry.AppendLine(" ELSE DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                strQry.AppendLine(" END ");

                strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= PCPC.PAY_PERIOD_DATE ");
                strQry.AppendLine(" AND ETC.INCLUDED_IN_RUN_IND IS NULL ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();

                //Set Timesheets Included in Run to NULL
                strQry.AppendLine(" UPDATE ETC ");
                strQry.AppendLine(" SET ");

                strQry.AppendLine(" INCLUDED_IN_RUN_IND = NULL ");

                if (parstrPayrollType == "W")
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC");
                }

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC ");
                strQry.AppendLine(" ON ETC.COMPANY_NO = PCPC.COMPANY_NO");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = PCPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P' ");

                strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parInt64CompanyNo);
                //All TimeSheets Except those that were Previously Excluded from Run
                strQry.AppendLine(" AND ETC.INCLUDED_IN_RUN_IND <> 'N' ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();

                if (parstrPayrollType == "W")
                {
                    strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ");
                    strQry.AppendLine(" ENABLE TRIGGER tgr_EMPLOYEE_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table ");
                }
                else
                {
                    strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ");
                    strQry.AppendLine(" ENABLE TRIGGER tgr_EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_Table ");
                }

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();

                if (parstrPayrollType == "W")
                {
                    strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT ");
                    strQry.AppendLine(" DISABLE TRIGGER tgr_EMPLOYEE_BREAK_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table ");
                }
                else
                {
                    strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ");
                    strQry.AppendLine(" DISABLE TRIGGER tgr_EMPLOYEE_TIME_ATTEND_BREAK_CURRENT_Maintain_EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_Table ");
                }

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();

                //Set Breaks Excluded from Run to 'N'
                strQry.AppendLine(" UPDATE ETC ");
                strQry.AppendLine(" SET ");

                strQry.AppendLine(" INCLUDED_IN_RUN_IND = 'N' ");

                if (parstrPayrollType == "W")
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT ETC");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETC");
                }

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");
                strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO");
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC ");
                strQry.AppendLine(" ON ETC.COMPANY_NO = PCPC.COMPANY_NO");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = PCPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = PCPC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P' ");

                strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parInt64CompanyNo);

                strQry.AppendLine(" AND ETC.BREAK_DATE > ");
                strQry.AppendLine(" CASE ");

                strQry.AppendLine(" WHEN E.FIRST_RUN_COMPLETED_IND = 'Y' ");
                strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE ");

                strQry.AppendLine(" ELSE DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                strQry.AppendLine(" END ");

                strQry.AppendLine(" AND ETC.BREAK_DATE <= PCPC.PAY_PERIOD_DATE ");
                strQry.AppendLine(" AND ETC.INCLUDED_IN_RUN_IND IS NULL ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();

                //Set Breaks Included in Run to NULL
                strQry.AppendLine(" UPDATE ETC ");
                strQry.AppendLine(" SET ");

                strQry.AppendLine(" INCLUDED_IN_RUN_IND = NULL ");

                if (parstrPayrollType == "W")
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT ETC");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETC");
                }

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC ");
                strQry.AppendLine(" ON ETC.COMPANY_NO = PCPC.COMPANY_NO");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = PCPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P' ");

                strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parInt64CompanyNo);
                //All TimeSheets Except those that were Previously Excluded from Run
                strQry.AppendLine(" AND ETC.INCLUDED_IN_RUN_IND <> 'N' ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();

                if (parstrPayrollType == "W")
                {
                    strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT ");
                    strQry.AppendLine(" ENABLE TRIGGER tgr_EMPLOYEE_BREAK_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table ");
                }
                else
                {
                    strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ");
                    strQry.AppendLine(" ENABLE TRIGGER tgr_EMPLOYEE_TIME_ATTEND_BREAK_CURRENT_Maintain_EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_Table ");
                }

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
                strQry.AppendLine(" SET BACKUP_DB_IND = 1");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            }
            catch(Exception ex)
            {
                Write_Log(ex, strClassNameFunctionAndParameters, strQry.ToString(), true);

                throw;
            }
            
            //ELR - 2015-07-02
            byte[] bytCompress = Get_Form_Records(parInt64CompanyNo, parstrFromProgram);

            return bytCompress;
        }

        private void Get_TimeSheet_Totals(DataSet DataSet,Int64 parInt64CompanyNo, DateTime parPayPeriodDate, string strPayCategoryIn, string parstrPayCategoryType)
        {
            StringBuilder strQry = new StringBuilder();
                
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" DAY_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",DAY_TABLE.EMPLOYEE_NO");
            strQry.AppendLine(",DAY_TABLE.TIMESHEET_DATE AS DAY_DATE");
            strQry.AppendLine(",DAY_TABLE.DAY_PAID_MINUTES");
            strQry.AppendLine(",0 AS NORMALTIME_MINUTES");
            strQry.AppendLine(",0 AS OVERTIME1_MINUTES");
            strQry.AppendLine(",0 AS OVERTIME2_MINUTES");
            strQry.AppendLine(",0 AS OVERTIME3_MINUTES");

            strQry.AppendLine(",PAIDHOLIDAY_MINUTES = ");

            strQry.AppendLine(" CASE ");
            strQry.AppendLine(" WHEN TEMP_TABLE5.PUBLIC_HOLIDAY_DATE IS NULL ");
            strQry.AppendLine(" THEN 0 ");

            strQry.AppendLine(" ELSE DAY_TABLE.DAY_PAID_MINUTES");

            strQry.AppendLine(" END ");

            //2017-09-19
            strQry.AppendLine(",INDICATOR = ");

            strQry.AppendLine(" CASE ");
            
            strQry.AppendLine(" WHEN NOT FINAL_EMPLOYEE_LEAVE_TABLE.PAY_CATEGORY_NO IS NULL THEN 'X' ");

            strQry.AppendLine(" ELSE DAY_TABLE.INDICATOR ");

            strQry.AppendLine(" END ");
          
            strQry.AppendLine(",PAIDHOLIDAY_INDICATOR = ");

            strQry.AppendLine(" CASE ");
            strQry.AppendLine(" WHEN TEMP_TABLE5.PUBLIC_HOLIDAY_DATE IS NULL ");
            strQry.AppendLine(" THEN '' ");

            strQry.AppendLine(" ELSE 'Y'");

            strQry.AppendLine(" END ");
            
            //2017-09-19
            strQry.AppendLine(",LEAVE_INDICATOR = ");

            strQry.AppendLine(" CASE ");
            strQry.AppendLine(" WHEN NOT FINAL_EMPLOYEE_LEAVE_TABLE.PAY_CATEGORY_NO IS NULL ");
            strQry.AppendLine(" THEN 'Y' ");

            strQry.AppendLine(" ELSE ''");

            strQry.AppendLine(" END ");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");
            
            //2017-06-28 Fix for Employee That is Closed
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
            strQry.AppendLine(" ON PCPC.COMPANY_NO = EPCC.COMPANY_NO");
            strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");

            //2017-07-29 - Bug Fixed 
            strQry.AppendLine(" AND PCPC.PAY_CATEGORY_NO = EPCC.PAY_CATEGORY_NO");
            
            strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P'");
            //2017-06-28 Fix for Employee That is Closed
            
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");
            strQry.AppendLine(" ON EPCC.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND EPCC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            
            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT AS DAY_TABLE ");
            }
            else
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT AS DAY_TABLE ");
            }

            //End of Created Day Table From Timesheet 
            //End of Created Day Table From Timesheet 
            strQry.AppendLine(" ON PCPC.COMPANY_NO = DAY_TABLE.COMPANY_NO");

            strQry.AppendLine(" AND DAY_TABLE.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");

            //2017-06-28 Fix for Employee That is Closed
            strQry.AppendLine(" AND EPCC.EMPLOYEE_NO = DAY_TABLE.EMPLOYEE_NO");

            strQry.AppendLine(" AND DAY_TABLE.TIMESHEET_DATE <= PCPC.PAY_PERIOD_DATE");
            strQry.AppendLine(" AND PCPC.PAY_CATEGORY_NO = DAY_TABLE.PAY_CATEGORY_NO");

            //2017-09-16 - From
            strQry.AppendLine(" AND DAY_TABLE.TIMESHEET_DATE > ");
            
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN E.EMPLOYEE_LAST_RUNDATE IS NULL ");
            strQry.AppendLine(" THEN DATEADD(DD, -40, '" + DateTime.Now.ToString("yyyy-MM-dd") + "')");

            strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND, '') <> 'Y' ");
            strQry.AppendLine(" THEN DATEADD(DD,-1, E.EMPLOYEE_LAST_RUNDATE) ");

            strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

            strQry.AppendLine(" END ");
            //2017-09-16 - To
            
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_CURRENT PCWC");
            strQry.AppendLine(" ON DAY_TABLE.COMPANY_NO = PCWC.COMPANY_NO");
            strQry.AppendLine(" AND DAY_TABLE.PAY_CATEGORY_NO = PCWC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = PCWC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND DAY_TABLE.TIMESHEET_DATE >= PCWC.WEEK_DATE_FROM");
            strQry.AppendLine(" AND DAY_TABLE.TIMESHEET_DATE <= PCWC.WEEK_DATE");

            strQry.AppendLine(" LEFT JOIN ");

            //Public Holiday Dates
            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" PCPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",PHC.PUBLIC_HOLIDAY_DATE ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_CURRENT PHC");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");
            strQry.AppendLine(" ON PHC.COMPANY_NO = PCPC.COMPANY_NO");
            strQry.AppendLine(" AND PCPC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
            strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P'");
            //Pay Category Pays Public Holiday's
            strQry.AppendLine(" AND PCPC.PAY_PUBLIC_HOLIDAY_IND = 'Y'");

            strQry.AppendLine(" WHERE PHC.COMPANY_NO = " + parInt64CompanyNo + ") AS TEMP_TABLE5");

            strQry.AppendLine(" ON DAY_TABLE.PAY_CATEGORY_NO = TEMP_TABLE5.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND DAY_TABLE.TIMESHEET_DATE = TEMP_TABLE5.PUBLIC_HOLIDAY_DATE");
            
            //From Here
            strQry.AppendLine(" LEFT JOIN ");

            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" EMPLOYEE_LEAVE_TABLE.PAY_CATEGORY_NO ");
            strQry.AppendLine(",EMPLOYEE_LEAVE_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",D.DAY_DATE ");

            strQry.AppendLine(" FROM ");

            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" E.COMPANY_NO ");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",E.EMPLOYEE_NO ");
            strQry.AppendLine(",E.LEAVE_SHIFT_NO ");
            strQry.AppendLine(",LC.LEAVE_FROM_DATE ");
            strQry.AppendLine(",LC.LEAVE_TO_DATE ");
            strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN E.EMPLOYEE_LAST_RUNDATE IS NULL ");
            strQry.AppendLine(" THEN DATEADD(DD, -40, '" + DateTime.Now.ToString("yyyy-MM-dd") + "')");
            
            strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND, '') <> 'Y' ");
            strQry.AppendLine(" THEN DATEADD(DD, -1, E.EMPLOYEE_LAST_RUNDATE) ");

            strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT LC ");
            strQry.AppendLine(" ON E.COMPANY_NO = LC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = LC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = LC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND LC.PROCESS_NO < 99");
            strQry.AppendLine(" AND LC.LEAVE_OPTION = 'D'");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            
            strQry.AppendLine(" UNION");

            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.LEAVE_SHIFT_NO");
            strQry.AppendLine(",LH.LEAVE_FROM_DATE");
            strQry.AppendLine(",LH.LEAVE_TO_DATE");
            strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

            strQry.AppendLine(" CASE");

            strQry.AppendLine(" WHEN E.EMPLOYEE_LAST_RUNDATE IS NULL");
            strQry.AppendLine(" THEN DATEADD(DD, -40, '" + DateTime.Now.ToString("yyyy-MM-dd") + "')");

            strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND, '') <> 'Y'");
            strQry.AppendLine(" THEN DATEADD(DD, -1, E.EMPLOYEE_LAST_RUNDATE)");

            strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE");

            strQry.AppendLine(" END");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY LH");
            strQry.AppendLine(" ON E.COMPANY_NO = LH.COMPANY_NO");
            strQry.AppendLine(" AND LH.PAY_PERIOD_DATE >= '" + DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd") + "'");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = LH.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = LH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND LH.PROCESS_NO < 99");
            strQry.AppendLine(" AND LH.LEAVE_OPTION = 'D'");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            
            strQry.AppendLine(") AS EMPLOYEE_LEAVE_TABLE");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");
            strQry.AppendLine(" ON D.DAY_DATE >= EMPLOYEE_LEAVE_TABLE.LEAVE_FROM_DATE");
            strQry.AppendLine(" AND D.DAY_DATE <= EMPLOYEE_LEAVE_TABLE.LEAVE_TO_DATE");
            strQry.AppendLine(" AND D.DAY_DATE > EMPLOYEE_LEAVE_TABLE.EMPLOYEE_LAST_RUNDATE");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT LS");
            strQry.AppendLine(" ON EMPLOYEE_LEAVE_TABLE.COMPANY_NO = LS.COMPANY_NO");
            strQry.AppendLine(" AND EMPLOYEE_LEAVE_TABLE.LEAVE_SHIFT_NO = LS.LEAVE_SHIFT_NO");
            strQry.AppendLine(" AND EMPLOYEE_LEAVE_TABLE.PAY_CATEGORY_TYPE = LS.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND LS.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND((ISNULL(LS.LEAVE_PAID_ACCUMULATOR_IND, 1) = 1");
            strQry.AppendLine(" AND D.DAY_NO IN(1, 2, 3, 4, 5))");
            strQry.AppendLine(" OR(LS.LEAVE_PAID_ACCUMULATOR_IND = 2");
            strQry.AppendLine(" AND D.DAY_NO IN(1, 2, 3, 4, 5, 6))");
            strQry.AppendLine(" OR(LS.LEAVE_PAID_ACCUMULATOR_IND = 3");
            strQry.AppendLine(" AND D.DAY_NO IN(0, 1, 2, 3, 4, 5, 6)))");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY PH");
            strQry.AppendLine(" ON D.DAY_DATE = PH.PUBLIC_HOLIDAY_DATE");
            strQry.AppendLine(" WHERE PH.PUBLIC_HOLIDAY_DATE IS NULL) AS FINAL_EMPLOYEE_LEAVE_TABLE");
            strQry.AppendLine(" ON DAY_TABLE.PAY_CATEGORY_NO = FINAL_EMPLOYEE_LEAVE_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND DAY_TABLE.EMPLOYEE_NO = FINAL_EMPLOYEE_LEAVE_TABLE.EMPLOYEE_NO");
            strQry.AppendLine(" AND DAY_TABLE.TIMESHEET_DATE = FINAL_EMPLOYEE_LEAVE_TABLE.DAY_DATE");
            
            strQry.AppendLine(" WHERE PCPC.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PCPC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
            strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P'");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" DAY_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",DAY_TABLE.EMPLOYEE_NO");
            strQry.AppendLine(",DAY_TABLE.TIMESHEET_DATE");

        Create_DataTable:

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "TimeSheetDayTotals", parInt64CompanyNo);
        }

        public string Check_Queue(Int64 parInt64CompanyNo, string parstrPayrollType)
        {
            String strReturn = "S";

            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" PAYROLL_RUN_QUEUE_IND");

            strQry.AppendLine(" FROM InteractPayroll.dbo.PAYROLL_RUN_QUEUE");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayrollQueue", parInt64CompanyNo);
            
            if (DataSet.Tables["PayrollQueue"].Rows.Count > 0)
            {
                if (DataSet.Tables["PayrollQueue"].Rows[0]["PAYROLL_RUN_QUEUE_IND"].ToString() != "")
                {
                    strReturn = DataSet.Tables["PayrollQueue"].Rows[0]["PAYROLL_RUN_QUEUE_IND"].ToString();
                }
            }
            else
            {
                //Completed Successfully
                strReturn = "";
            }

            return strReturn;
        }
        
        public string Insert_Run_Into_Queue(Int64 parint64CurrentUserNo, Int64 parInt64CompanyNo, string parstrArrayPayCategoryNo, string parstrPayrollType, DateTime parPayPeriodDate)
        {
            string[] parstrPayCategoryNo = parstrArrayPayCategoryNo.Split('|');
            string strReturnPayCategoryNosInError = "";

            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new DataSet();
            DataView DataViewTimeSheetErrors;
            
            Get_TimeSheet_Totals(DataSet, parInt64CompanyNo, parPayPeriodDate, parstrArrayPayCategoryNo, parstrPayrollType);
            
            //Check For TimeSheet Errors
            for (int intCount = 0; intCount < parstrPayCategoryNo.Length; intCount++)
            {
                DataViewTimeSheetErrors = null;
                DataViewTimeSheetErrors = new DataView(DataSet.Tables["TimeSheetDayTotals"],
                    "PAY_CATEGORY_NO = " + parstrPayCategoryNo[intCount].ToString() + " AND INDICATOR = 'X'",
                    "",
                    DataViewRowState.CurrentRows);

                if (DataViewTimeSheetErrors.Count > 0)
                {
                    if (strReturnPayCategoryNosInError == "")
                    {
                        strReturnPayCategoryNosInError = parstrPayCategoryNo[intCount].ToString();
                    }
                    else
                    {
                        strReturnPayCategoryNosInError += "|" + parstrPayCategoryNo[intCount].ToString();
                    }
                }
            }

            if (strReturnPayCategoryNosInError != "")
            {
                //Errors in TimeSheet
                goto Insert_Run_Into_Queue_Continue;
            }
            
            strQry.Clear();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.PAYROLL_RUN_QUEUE");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CheckRecordExists", parInt64CompanyNo);

            strQry.Clear();

            if (DataSet.Tables["CheckRecordExists"].Rows.Count == 0)
            {
                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.PAYROLL_RUN_QUEUE");
                strQry.AppendLine("(USER_NO");
                strQry.AppendLine(",COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",PAY_PERIOD_DATE");
                strQry.AppendLine(",PAY_CATEGORY_NUMBERS)");

                strQry.AppendLine(" VALUES ");

                strQry.AppendLine("(" + parint64CurrentUserNo);
                strQry.AppendLine("," + parInt64CompanyNo);
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(",'" + parPayPeriodDate.ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrArrayPayCategoryNo) + ")");
            }
            else
            {
                strQry.AppendLine(" UPDATE InteractPayroll.dbo.PAYROLL_RUN_QUEUE");

                strQry.AppendLine(" SET ");

                strQry.AppendLine(" PAY_PERIOD_DATE = '" + parPayPeriodDate.ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(",PAY_CATEGORY_NUMBERS = " + clsDBConnectionObjects.Text2DynamicSQL(parstrArrayPayCategoryNo));
                strQry.AppendLine(",USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(",PAYROLL_RUN_QUEUE_IND = NULL");
                strQry.AppendLine(",START_RUN_DATE = NULL");
                strQry.AppendLine(",END_RUN_DATE = NULL");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            }

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

        Insert_Run_Into_Queue_Continue:

            return strReturnPayCategoryNosInError;
        }

        public string Calculate_Payroll_From_TimeSheets(Int64 parint64CurrentUserNo, Int64 parInt64CompanyNo, string parstrArrayPayCategoryNo,string parstrPayrollType, DateTime parPayPeriodDate)
        {
            string strClassNameFunctionAndParameters = pvtstrClassName + " Calculate_Payroll_From_TimeSheets CompanyNo=" + parInt64CompanyNo + ",ArrayPayCategoryNo=" + parstrArrayPayCategoryNo + ",PayrollType=" + parstrPayrollType + ",PayPeriodDate=" + parPayPeriodDate.ToString("yyyy-MM-dd");

            string strReturnPayCategoryNosInError = "";
            StringBuilder strQry = new StringBuilder();

            try
            {
                Write_Log("Entered " + strClassNameFunctionAndParameters);

                DataSet TempDataSet = new DataSet();

                string[] parstrPayCategoryNo = parstrArrayPayCategoryNo.Split('|');

                int intReturnCode = 0;
                DateTime dtEndTaxYear;
                DateTime dtStartTaxYear;
                DateTime dtStartLeaveTaxYear;

                if (parPayPeriodDate.Month > 2)
                {
                    dtStartTaxYear = new DateTime(parPayPeriodDate.Year, 3, 1);
                }
                else
                {
                    dtStartTaxYear = new DateTime(parPayPeriodDate.Year - 1, 3, 1);
                }

                dtEndTaxYear = dtStartTaxYear.AddYears(1).AddDays(-1);

                //2017-01-12
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" LEAVE_BEGIN_MONTH");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY E");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), TempDataSet, "LeaveStartDate", parInt64CompanyNo);

                if (TempDataSet.Tables["LeaveStartDate"].Rows.Count > 0)
                {
                    //Position Within Current Financial Year
                    if (parPayPeriodDate.Month >= Convert.ToInt32(TempDataSet.Tables["LeaveStartDate"].Rows[0]["LEAVE_BEGIN_MONTH"]))
                    {
                        dtStartLeaveTaxYear = new DateTime(parPayPeriodDate.Year, Convert.ToInt32(TempDataSet.Tables["LeaveStartDate"].Rows[0]["LEAVE_BEGIN_MONTH"]), 1);
                    }
                    else
                    {
                        dtStartLeaveTaxYear = new DateTime(parPayPeriodDate.Year - 1, Convert.ToInt32(TempDataSet.Tables["LeaveStartDate"].Rows[0]["LEAVE_BEGIN_MONTH"]), 1);
                    }
                }
                else
                {
                    dtStartLeaveTaxYear = dtStartTaxYear;
                }

                clsISUtilities clsISUtilities = new clsISUtilities();
                clsTaxTableRead clsTaxTableRead = new clsTaxTableRead();

                DataSet DataSet = clsTaxTableRead.Get_Tax_UIF_Tables(dtEndTaxYear.Year);

                clsTax Tax = new clsTax(DataSet);

                DataView DataViewEarningAmount = null;
                DataView DataViewCompanyPaidPublicHoliday;
                DataView DataViewEmployeeLeaveTotal;
                DataView DataViewEmployeeParameters;
                DataView DataViewTimeSheetDayTotal;
                DataView DataViewEmployeeDeduction;
                DataView DataViewEmployeeDeductionEarningPercentage;
                DataView DataViewEmployeeLoan;
                DataView DataViewPayCategoryWeek;
                DataView DataViewTaxSpreadSheet;
                DataView DataViewTimeSheetErrors;
                DataView DataViewCommission;
                DataView DataViewNormalLeaveClose;

                DataRowView drvDataRowView;

                DateTime dtTimeSheet = DateTime.Now;

                double dblEmployeePortionOfYear = 0;
                double dblAgeAtTaxYearEnd = 0;
                double dblEmployeeAnnualisedFactor = 0;

                string strPayUIFInd = "";
                string strMedicalAidInd = "";
                string strEmployeeExceptionIndicator = "";
                string strEmployeeNormalIndicator = "";
                string strEmployeeBlankIndicator = "";
                string strEmployeeWeekExceptionIndicator = "";
                string strEmployeeWeekNormalIndicator = "";
                string strEmployeeWeekBlankIndicator = "";

                bool blnWeekDays = true;
                bool blnBonusInd = false;
                bool blnEmployeeTimeSheetsForPayCategory = false;

                int intNormalHoursBoundary = 0;
                int intOverTime1HoursBoundary = 0;
                int intOverTime2HoursBoundary = 0;
                int intOverTime3HoursBoundary = 0;

                int intDayNormalHours = 0;
                int intDayOverTime1Hours = 0;
                int intDayOverTime2Hours = 0;
                int intDayOverTime3Hours = 0;

                int intTotalMinutesPaidForWeek = 0;

                int intWeekNormalMinutes = 0;
                int intWeekOverTime1Minutes = 0;
                int intWeekOverTime2Minutes = 0;
                int intWeekOverTime3Minutes = 0;
                int intWeekPaidHolidayWorkedMinutes = 0;

                int intPeriodNormalTimeMinutes = 0;
                int intPeriodOverTime1Minutes = 0;
                int intPeriodOverTime2Minutes = 0;
                int intPeriodOverTime3Minutes = 0;
                int intPeriodPaidHolidayWorkedMinutes = 0;

                int intNormalTimeMinutesRounded = 0;
                int intOverTime1MinutesRounded = 0;
                int intOverTime2MinutesRounded = 0;
                int intOverTime3MinutesRounded = 0;
                int intPaidHolidayWorkedMinutesRounded = 0;
                int intOverTimeCF = 0;
                int intEarningAmountRow = 0;

                double dblNormalHoursDecimal = 0;
                double dblOverTime1HoursDecimal = 0;
                double dblOverTime2HoursDecimal = 0;
                double dblOverTime3HoursDecimal = 0;
                double dblPaidHolidayWorkedHoursDecimal = 0;

                double dblNormalTotal = 0;
                double dblOverTime1Total = 0;
                double dblOverTime2Total = 0;
                double dblOverTime3Total = 0;

                double dblLeaveDecimal = 0;
                double dblLeaveTotal = 0;
                double dblLeaveHoursOtherValue = 0;
                double dblLeaveDayOtherValue = 0;

                double dblPaidHolidayTotal = 0;

                double dblPaidHolidayWorkedTotal = 0;

                double dblDeductionType3Total = 0;
                double dblDeductionFinalTotal = 0;

                double dblUIFAmount = 0;
                double dblCommissionAmount = 0;
                double dblEarningsCurrent = 0;
                double dblTaxEarningsYTD = 0;
                double dblTaxEarningsOtherYTD = 0;
                double dblCompanyPaidHoliday = 0;

                double dblTaxYTD = 0;

                double dblTaxSpreadSheetValue = 0;

                double dblTaxCalculatedRun = 0;

                double dblPensionArrearYTD = 0;
                double dblRetireAnnuityArrearYTD = 0;

                DateTime dtEmployeeLastRundate;

                double dblDaysInYear = 0;
                double dblEmployeeDaysWorked = 0;
                intReturnCode = -1;

                bool blnDeductionFound = false;
                int intLoanRow = -1;
                int inCompanyPaidPublicHolidayRow = -1;
                int intTaxSpreadSheetRow = -1;
                int intMedicalAidNumberDependents = 0;

                int intIRP5 = -1;
                object[] objKey = new object[2];
                object[] objFindTaxSpreadSheet = new object[3];

                //Values Returned From Tax Module for Show on Screen
                double[] dblRetirementAnnuityAmount = new double[12];
                double dblRetirementAnnuityTotal = 0;

                double[] dblPensionFundAmount = new double[12];
                double dblPensionFundTotal = 0;

                double[] dblTaxTotal = new double[11];

                double[] dblUifTotal = new double[6];

                //ELR 2014-04-19
                double dblOverTimeRate1 = 0;
                double dblOverTimeRate2 = 0;
                double dblOverTimeRate3 = 0;

                double dblTotalDeductions = 0;

                int intTaxTableRow = -1;
                int intEarningsTaxTableRow = -1;
                double dblTotalNormalEarnings = 0;
                double dblTotalNormalEarningsAnnualised = 0;
                double dblTotalEarnedAccumAnnualInitial = 0;

                DateTime dtDate;
                DateTime dtEmployeeBirthDate;
                DateTime dtEmployeeStartDate;
                DateTime dtEmployeeLastRunDate;

                TimeSpan tsDaysInYear = dtEndTaxYear.Subtract(dtStartTaxYear);
                dblDaysInYear = tsDaysInYear.Days + 1;
                string strPayCategoryIn = "";

                //Create SQL For Months of EMPLOYEE_TAX_SPREADHEET
                string strSelectAddonQry = "";
                DateTime dtTempDateTime = new DateTime(2007, 3, 1);

                for (int intRowMonth = dtTempDateTime.Month; intRowMonth < 20; intRowMonth++)
                {
                    if (intRowMonth == parPayPeriodDate.Month)
                    {
                        strSelectAddonQry += ",ISNULL(ETSH." + dtTempDateTime.ToString("MMM").ToUpper() + "_TOTAL,0) + XX ";
                    }
                    else
                    {
                        strSelectAddonQry += ",ISNULL(ETSH." + dtTempDateTime.ToString("MMM").ToUpper() + "_TOTAL,0)";
                    }

                    if (dtTempDateTime.Month == 2)
                    {
                        break;
                    }

                    dtTempDateTime = dtTempDateTime.AddMonths(1);
                }

                for (int intCount = 0; intCount < parstrPayCategoryNo.Length; intCount++)
                {
                    if (intCount == 0)
                    {
                        strPayCategoryIn = parstrPayCategoryNo[intCount].ToString();
                    }
                    else
                    {
                        strPayCategoryIn += "," + parstrPayCategoryNo[intCount].ToString();
                    }
                }

                DataRow dtDataRow;

                DataTable DataTable = new DataTable("DaysInWeek");
                DataTable.Columns.Add("DAY_DATE", typeof(System.DateTime));
                DataSet.Tables.Add(DataTable);

                Get_TimeSheet_Totals(DataSet, parInt64CompanyNo, parPayPeriodDate, strPayCategoryIn, parstrPayrollType);

                //Check For TimeSheet Errors
                for (int intCount = 0; intCount < parstrPayCategoryNo.Length; intCount++)
                {
                    DataViewTimeSheetErrors = null;
                    DataViewTimeSheetErrors = new DataView(DataSet.Tables["TimeSheetDayTotals"],
                        "PAY_CATEGORY_NO = " + parstrPayCategoryNo[intCount].ToString() + " AND INDICATOR = 'X'",
                        "",
                        DataViewRowState.CurrentRows);

                    if (DataViewTimeSheetErrors.Count > 0)
                    {
                        if (strReturnPayCategoryNosInError == "")
                        {
                            strReturnPayCategoryNosInError = parstrPayCategoryNo[intCount].ToString();
                        }
                        else
                        {
                            strReturnPayCategoryNosInError += "|" + parstrPayCategoryNo[intCount].ToString();
                        }
                    }
                }

                if (strReturnPayCategoryNosInError != "")
                {
                    //2017-07-11
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE InteractPayroll.dbo.PAYROLL_RUN_QUEUE");
                    //Errors in TimeSheets
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" PAYROLL_RUN_QUEUE_IND = 'E'");
                    strQry.AppendLine(",START_RUN_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    //Errors in TimeSheet
                    goto Calculate_Payroll_From_TimeSheets_Continue;
                }
                
                //2017-07-31
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT");

                strQry.AppendLine(" SET PAY_CATEGORY_USED_IND = 'Y'");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                strQry.AppendLine(" AND RUN_TYPE = 'P'");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EPCC.PAY_CATEGORY_NO ");
                strQry.AppendLine(",L.EMPLOYEE_NO");
                strQry.AppendLine(",L.DEDUCTION_NO");
                strQry.AppendLine(",L.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",SUM(L.LOAN_AMOUNT_PAID - L.LOAN_AMOUNT_RECEIVED) AS TOTAL_LOAN");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LOANS L");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                strQry.AppendLine(" ON L.COMPANY_NO = EPCC.COMPANY_NO");
                strQry.AppendLine(" AND L.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND L.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                //NB - Use Default
                strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y'");
                strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P'");

                //2017-01-24 (Employee Not Closed)
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON EPCC.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND EPCC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                strQry.AppendLine(" WHERE L.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND (NOT L.LOAN_PROCESSED_DATE IS NULL");
                strQry.AppendLine(" OR (L.LOAN_PROCESSED_DATE IS NULL");
                strQry.AppendLine(" AND L.PROCESS_NO = 0))");

                strQry.AppendLine(" AND L.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" EPCC.PAY_CATEGORY_NO ");
                strQry.AppendLine(",L.EMPLOYEE_NO");
                strQry.AppendLine(",L.DEDUCTION_NO");
                strQry.AppendLine(",L.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(" HAVING SUM(L.LOAN_AMOUNT_PAID - L.LOAN_AMOUNT_RECEIVED) <> 0");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" EPCC.PAY_CATEGORY_NO ");
                strQry.AppendLine(",L.EMPLOYEE_NO");
                strQry.AppendLine(",L.DEDUCTION_NO");
                strQry.AppendLine(",L.DEDUCTION_SUB_ACCOUNT_NO");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeLoan", parInt64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" EPCC.PAY_CATEGORY_NO ");
                strQry.AppendLine(",LC.EMPLOYEE_NO");
                strQry.AppendLine(",LC.EARNING_NO");
                strQry.AppendLine(",EN.LEAVE_PERCENTAGE");
                strQry.AppendLine(",SUM(ROUND(LC.LEAVE_HOURS_DECIMAL,2)) AS LEAVE_HOURS_DECIMAL");
                strQry.AppendLine(",SUM(ROUND(LC.LEAVE_DAYS_DECIMAL,2)) AS LEAVE_DAYS_DECIMAL");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT LC");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                strQry.AppendLine(" ON LC.COMPANY_NO = EPCC.COMPANY_NO");
                strQry.AppendLine(" AND LC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                strQry.AppendLine(" AND LC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                //NB - Use Default
                strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y'");
                strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P'");

                //2017-01-24 (Employee Not Closed)
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON EPCC.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND EPCC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EN ");

                strQry.AppendLine(" ON LC.COMPANY_NO = EN.COMPANY_NO");
                strQry.AppendLine(" AND LC.EARNING_NO = EN.EARNING_NO");
                strQry.AppendLine(" AND LC.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EN.LEAVE_PERCENTAGE > 0");
                strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL ");

                strQry.AppendLine(" WHERE LC.PROCESS_NO = 0");
                strQry.AppendLine(" AND LC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" EPCC.PAY_CATEGORY_NO ");
                strQry.AppendLine(",LC.EMPLOYEE_NO");
                strQry.AppendLine(",LC.EARNING_NO");
                strQry.AppendLine(",EN.LEAVE_PERCENTAGE");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeLeaveTotal", parInt64CompanyNo);

                //Currently Active Deductions
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EDC.EMPLOYEE_NO");
                strQry.AppendLine(",D.DEDUCTION_NO");
                strQry.AppendLine(",D.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",ED.DEDUCTION_VALUE");
                strQry.AppendLine(",ED.DEDUCTION_MIN_VALUE");
                strQry.AppendLine(",ED.DEDUCTION_MAX_VALUE");
                strQry.AppendLine(",ED.DEDUCTION_PERIOD_IND");
                strQry.AppendLine(",ED.DEDUCTION_DAY_VALUE");
                strQry.AppendLine(",ED.DEDUCTION_TYPE_IND");
                strQry.AppendLine(",D.DEDUCTION_LOAN_TYPE_IND");

                //Errol Added
                strQry.AppendLine(",ISNULL(SUM(EDH.TOTAL),0) AS TOTAL_YTD_BF ");
                //Errol Added

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT EDC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ED ");
                strQry.AppendLine(" ON EDC.COMPANY_NO = ED.COMPANY_NO");
                strQry.AppendLine(" AND EDC.PAY_CATEGORY_TYPE = ED.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EDC.DEDUCTION_NO = ED.DEDUCTION_NO");
                strQry.AppendLine(" AND EDC.DEDUCTION_SUB_ACCOUNT_NO = ED.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(" AND EDC.EMPLOYEE_NO = ED.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ED.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.DEDUCTION D");
                strQry.AppendLine(" ON EDC.COMPANY_NO = D.COMPANY_NO");
                strQry.AppendLine(" AND EDC.PAY_CATEGORY_TYPE = D.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EDC.DEDUCTION_NO = D.DEDUCTION_NO");
                strQry.AppendLine(" AND EDC.DEDUCTION_SUB_ACCOUNT_NO = D.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                strQry.AppendLine(" ON EDC.COMPANY_NO = EPCC.COMPANY_NO");
                strQry.AppendLine(" AND EDC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EDC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y' ");
                //2017-07-19
                strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P' ");
                
                //2017-01-24 (Employee Not Closed)
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON EPCC.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND EPCC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY EDH ");
                strQry.AppendLine(" ON EDC.COMPANY_NO = EDH.COMPANY_NO");
                //2017-02-22 Removed To Cater For Employee PAY_CATEGORY_TYPE Change
                //strQry.AppendLine(" AND EDC.PAY_CATEGORY_TYPE = EDH.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EDC.EMPLOYEE_NO = EDH.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EDC.DEDUCTION_NO = EDH.DEDUCTION_NO");
                strQry.AppendLine(" AND EDC.DEDUCTION_SUB_ACCOUNT_NO = EDH.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(" AND EDH.PAY_PERIOD_DATE >= '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");

                strQry.AppendLine(" WHERE EDC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EDC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND EDC.RUN_TYPE = 'P' ");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EDC.EMPLOYEE_NO");
                strQry.AppendLine(",D.DEDUCTION_NO");
                strQry.AppendLine(",D.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",ED.DEDUCTION_VALUE");
                strQry.AppendLine(",ED.DEDUCTION_MIN_VALUE");
                strQry.AppendLine(",ED.DEDUCTION_MAX_VALUE");
                strQry.AppendLine(",ED.DEDUCTION_PERIOD_IND");
                strQry.AppendLine(",ED.DEDUCTION_DAY_VALUE");
                strQry.AppendLine(",ED.DEDUCTION_TYPE_IND");
                strQry.AppendLine(",D.DEDUCTION_LOAN_TYPE_IND");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EDC.EMPLOYEE_NO");
                strQry.AppendLine(",D.DEDUCTION_NO");
                strQry.AppendLine(",D.DEDUCTION_SUB_ACCOUNT_NO");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeDeduction", parInt64CompanyNo);

                //Currently Active Deductions Earning Percentage Link 
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EDEPC.EMPLOYEE_NO");
                strQry.AppendLine(",EDEPC.DEDUCTION_NO");
                strQry.AppendLine(",EDEPC.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",EDEPC.EARNING_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE_CURRENT EDEPC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                strQry.AppendLine(" ON EDEPC.COMPANY_NO = EPCC.COMPANY_NO");
                strQry.AppendLine(" AND EDEPC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EDEPC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y' ");
                //2017-07-19
                strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P' ");

                //2017-01-24 (Employee Not Closed)
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON EPCC.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND EPCC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                strQry.AppendLine(" WHERE EDEPC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EDEPC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EDEPC.EMPLOYEE_NO");
                strQry.AppendLine(",EDEPC.DEDUCTION_NO");
                strQry.AppendLine(",EDEPC.DEDUCTION_SUB_ACCOUNT_NO");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeDeductionEarningPercentage", parInt64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" SELECT DISTINCT ");
                strQry.AppendLine(" E.EMPLOYEE_NO");

                //Errol 2013-06-15
                strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

                strQry.AppendLine(" CASE ");

                //Errol - 2015-02-17
                strQry.AppendLine(" WHEN E.FIRST_RUN_COMPLETED_IND = 'Y'");

                strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE ");

                strQry.AppendLine(" ELSE DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                strQry.AppendLine(" END ");

                strQry.AppendLine(",E.TAX_TYPE_IND");
                strQry.AppendLine(",E.TAX_DIRECTIVE_PERCENTAGE");
                strQry.AppendLine(",E.EMPLOYEE_TAX_STARTDATE");
                strQry.AppendLine(",E.EMPLOYEE_BIRTHDATE");
                strQry.AppendLine(",E.EMPLOYEE_NUMBER_CHEQUES");
                strQry.AppendLine(",E.NUMBER_MEDICAL_AID_DEPENDENTS");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC ");
                strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                //2017-07-19
                strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P' ");

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" E.EMPLOYEE_NO");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parInt64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EPCC.EMPLOYEE_NO");
                strQry.AppendLine(",EPCC.HOURLY_RATE");
                strQry.AppendLine(",EPCC.DEFAULT_IND");
                strQry.AppendLine(",EPCC.OVERTIME_VALUE_BF");
                strQry.AppendLine(",EPCC.RUN_NO");

                strQry.AppendLine(",EARNINGS_YTD = ");
                strQry.AppendLine(" SUM(CASE");

                strQry.AppendLine(" WHEN EEH1.EARNING_NO <> 7");

                strQry.AppendLine(" THEN ISNULL(EEH1.TOTAL,0)");

                strQry.AppendLine(" ELSE 0 ");

                strQry.AppendLine(" END)");

                strQry.AppendLine(",EARNINGS_OTHER_YTD = ");
                strQry.AppendLine(" SUM(CASE");

                strQry.AppendLine(" WHEN EEH1.EARNING_NO = 7");

                strQry.AppendLine(" THEN ISNULL(EEH1.TOTAL,0)");

                strQry.AppendLine(" ELSE 0 ");

                strQry.AppendLine(" END)");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC ");

                //2017-01-24 (Employee Not Closed)
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON EPCC.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND EPCC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH1 ");
                strQry.AppendLine(" ON EPCC.COMPANY_NO = EEH1.COMPANY_NO");
                strQry.AppendLine(" AND EPCC.EMPLOYEE_NO = EEH1.EMPLOYEE_NO");
                //2017-02-17 - Caters For Change Of Employee PAY_CATEGORY_TYPE
                //strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = EEH1.PAY_CATEGORY_NO");
                //strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = EEH1.PAY_CATEGORY_TYPE");
                //Below Removed becaues Employee Needs to include Take-On
                //strQry.AppendLine(" AND EEH1.RUN_TYPE = 'P' ");
                strQry.AppendLine(" AND EEH1.PAY_PERIOD_DATE >= '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");
                //Errol Removed
                //strQry.AppendLine(" AND EEH1.EARNING_NO <> 7");

                strQry.AppendLine(" WHERE EPCC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                //2017-07-19
                strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P' ");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EPCC.EMPLOYEE_NO");
                strQry.AppendLine(",EPCC.HOURLY_RATE");
                strQry.AppendLine(",EPCC.DEFAULT_IND");
                strQry.AppendLine(",EPCC.OVERTIME_VALUE_BF");
                strQry.AppendLine(",EPCC.RUN_NO");

                strQry.AppendLine(" ORDER BY ");
                //NB Order by DEFAULT_IND in This Position Very Important (Causes Default Pay Category to Run Last for Tax etc)
                strQry.AppendLine(" EPCC.DEFAULT_IND");
                strQry.AppendLine(",EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EPCC.EMPLOYEE_NO");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeParameters", parInt64CompanyNo);

                //Update Public Holiday Columns and Set All Other Totals To Zero
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" MINUTES = TEMP_TABLE1.MINUTES");
                strQry.AppendLine(",MINUTES_ROUNDED = TEMP_TABLE1.MINUTES");
                strQry.AppendLine(",HOURS_DECIMAL = ROUND(CONVERT(DECIMAL,TEMP_TABLE1.MINUTES) / 60 ,2)");
                strQry.AppendLine(",HOURS_DECIMAL_OTHER_VALUE = ROUND(CONVERT(DECIMAL,TEMP_TABLE1.MINUTES) / 60 ,2)");
                strQry.AppendLine(",HOURS_DECIMAL_ORIGINAL = ROUND(CONVERT(DECIMAL,TEMP_TABLE1.MINUTES) / 60 ,2)");

                strQry.AppendLine(",TOTAL = ROUND(ROUND(CONVERT(DECIMAL,TEMP_TABLE1.MINUTES) / 60 ,2) * TEMP_TABLE1.HOURLY_RATE * TEMP_TABLE1.PAIDHOLIDAY_RATE,2)");

                strQry.AppendLine(" FROM ");

                strQry.AppendLine("(");
                //2-Start

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EEC.COMPANY_NO ");
                strQry.AppendLine(",EEC.EMPLOYEE_NO");
                strQry.AppendLine(",EPCC.HOURLY_RATE");
                strQry.AppendLine(",EEC.EARNING_NO");
                strQry.AppendLine(",EEC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EEC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",PCPC.PAIDHOLIDAY_RATE");
                strQry.AppendLine(",EEC.RUN_TYPE");

                strQry.AppendLine(",SUM (");

                strQry.AppendLine(" CASE ");

                //ERROL Changed 2011-03-30
                strQry.AppendLine(" WHEN NOT PUBLIC_HOLIDAY_TABLE.PUBLIC_HOLIDAY_DATE IS NULL AND D.DAY_NO = 0 ");
                strQry.AppendLine(" THEN PC.SUN_TIME_MINUTES");

                strQry.AppendLine(" WHEN NOT PUBLIC_HOLIDAY_TABLE.PUBLIC_HOLIDAY_DATE IS NULL AND D.DAY_NO = 1 ");
                strQry.AppendLine(" THEN PC.MON_TIME_MINUTES");

                strQry.AppendLine(" WHEN NOT PUBLIC_HOLIDAY_TABLE.PUBLIC_HOLIDAY_DATE IS NULL AND D.DAY_NO = 2 ");
                strQry.AppendLine(" THEN PC.TUE_TIME_MINUTES");

                strQry.AppendLine(" WHEN NOT PUBLIC_HOLIDAY_TABLE.PUBLIC_HOLIDAY_DATE IS NULL AND D.DAY_NO = 3 ");
                strQry.AppendLine(" THEN PC.WED_TIME_MINUTES");

                strQry.AppendLine(" WHEN NOT PUBLIC_HOLIDAY_TABLE.PUBLIC_HOLIDAY_DATE IS NULL AND D.DAY_NO = 4 ");
                strQry.AppendLine(" THEN PC.THU_TIME_MINUTES");

                strQry.AppendLine(" WHEN NOT PUBLIC_HOLIDAY_TABLE.PUBLIC_HOLIDAY_DATE IS NULL AND D.DAY_NO = 5 ");
                strQry.AppendLine(" THEN PC.FRI_TIME_MINUTES");

                strQry.AppendLine(" WHEN NOT PUBLIC_HOLIDAY_TABLE.PUBLIC_HOLIDAY_DATE IS NULL AND D.DAY_NO = 6 ");
                strQry.AppendLine(" THEN PC.SAT_TIME_MINUTES");

                //No Public Holiday
                strQry.AppendLine(" ELSE 0");

                strQry.AppendLine(" END) AS MINUTES ");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON EEC.COMPANY_NO = E.COMPANY_NO");
                strQry.AppendLine(" AND EEC.EMPLOYEE_NO = E.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");
                strQry.AppendLine(" ON EEC.COMPANY_NO = PCPC.COMPANY_NO");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_NO = PCPC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = PCPC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EEC.RUN_NO = PCPC.RUN_NO");
                //2017-07-19
                strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P' ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
                strQry.AppendLine(" ON E.COMPANY_NO = PC.COMPANY_NO");
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                strQry.AppendLine(" ON PCPC.COMPANY_NO = EPCC.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_NO = EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P'");
                //Paid Holiday Linked to Employee's Default Cost Centre
                strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y'");
                //2017-07-19
                strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P' ");

                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");

                strQry.AppendLine(" ON D.DAY_DATE > ");

                strQry.AppendLine(" CASE ");

                //Errol - 2015-02-17
                strQry.AppendLine(" WHEN E.FIRST_RUN_COMPLETED_IND = 'Y'");

                strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE ");

                strQry.AppendLine(" ELSE DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                strQry.AppendLine(" END ");

                strQry.AppendLine(" AND D.DAY_DATE <= PCPC.PAY_PERIOD_DATE");

                strQry.AppendLine(" LEFT JOIN ");

                strQry.AppendLine("( ");
                //1-Start

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PCPC1.PAY_CATEGORY_NO ");
                strQry.AppendLine(",PCPC1.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",PHC.PUBLIC_HOLIDAY_DATE ");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_CURRENT PHC");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC1");
                strQry.AppendLine(" ON PHC.COMPANY_NO = PCPC1.COMPANY_NO");
                strQry.AppendLine(" AND PCPC1.PAY_CATEGORY_NO  IN (" + strPayCategoryIn + ")");
                strQry.AppendLine(" AND PCPC1.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND PCPC1.PAY_PUBLIC_HOLIDAY_IND = 'Y'");
                //2017-07-19
                strQry.AppendLine(" AND PCPC1.RUN_TYPE = 'P' ");

                strQry.AppendLine(" WHERE PHC.COMPANY_NO = " + parInt64CompanyNo);

                //1-End
                strQry.AppendLine(") AS PUBLIC_HOLIDAY_TABLE ");

                strQry.AppendLine(" ON EEC.PAY_CATEGORY_NO = PUBLIC_HOLIDAY_TABLE.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = PUBLIC_HOLIDAY_TABLE.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND D.DAY_DATE = PUBLIC_HOLIDAY_TABLE.PUBLIC_HOLIDAY_DATE");

                strQry.AppendLine(" WHERE EEC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                //Public Holiday Paid By Company
                strQry.AppendLine(" AND EEC.EARNING_NO = 8");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" EEC.COMPANY_NO ");
                strQry.AppendLine(",EEC.EMPLOYEE_NO");
                strQry.AppendLine(",EPCC.HOURLY_RATE");
                strQry.AppendLine(",EEC.EARNING_NO");
                strQry.AppendLine(",EEC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EEC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",PCPC.PAIDHOLIDAY_RATE");
                strQry.AppendLine(",EEC.RUN_TYPE");

                //2-End
                strQry.AppendLine(") AS TEMP_TABLE1");

                strQry.AppendLine(" WHERE EMPLOYEE_EARNING_CURRENT.COMPANY_NO = TEMP_TABLE1.COMPANY_NO ");
                strQry.AppendLine(" AND EMPLOYEE_EARNING_CURRENT.EMPLOYEE_NO = TEMP_TABLE1.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EMPLOYEE_EARNING_CURRENT.EARNING_NO = TEMP_TABLE1.EARNING_NO ");
                strQry.AppendLine(" AND EMPLOYEE_EARNING_CURRENT.PAY_CATEGORY_NO = TEMP_TABLE1.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND EMPLOYEE_EARNING_CURRENT.PAY_CATEGORY_TYPE = TEMP_TABLE1.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EMPLOYEE_EARNING_CURRENT.RUN_TYPE = TEMP_TABLE1.RUN_TYPE ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EEC.EMPLOYEE_NO");
                strQry.AppendLine(",EEC.TOTAL");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");

                //2017-01-24 (Employee Not Closed)
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON EEC.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND EEC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                strQry.AppendLine(" WHERE EEC.COMPANY_NO = " + parInt64CompanyNo);

                strQry.AppendLine(" AND EEC.EARNING_NO = 8 ");
                strQry.AppendLine(" AND EEC.TOTAL > 0");

                //2017-07-19
                strQry.AppendLine(" AND EEC.RUN_TYPE = 'P' ");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyPaidPublicHoliday", parInt64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" T.PAY_CATEGORY_NO");
                strQry.AppendLine(",T.WEEK_DATE");
                strQry.AppendLine(",T.WEEK_DATE_FROM");
                strQry.AppendLine(",T.MON_TIME_MINUTES");
                strQry.AppendLine(",T.TUE_TIME_MINUTES");
                strQry.AppendLine(",T.WED_TIME_MINUTES");
                strQry.AppendLine(",T.THU_TIME_MINUTES");
                strQry.AppendLine(",T.FRI_TIME_MINUTES");
                strQry.AppendLine(",T.SAT_TIME_MINUTES");
                strQry.AppendLine(",T.SUN_TIME_MINUTES");
                strQry.AppendLine(",T.OVERTIME1_RATE");
                strQry.AppendLine(",T.OVERTIME2_RATE");
                strQry.AppendLine(",T.OVERTIME3_RATE");
                strQry.AppendLine(",T.PAIDHOLIDAY_RATE");
                strQry.AppendLine(",T.TOTAL_DAILY_TIME_OVERTIME");

                strQry.AppendLine(",T.DAILY_ROUNDING_IND");
                strQry.AppendLine(",T.DAILY_ROUNDING_MINUTES");
                strQry.AppendLine(",T.PAY_PERIOD_ROUNDING_IND");
                strQry.AppendLine(",T.PAY_PERIOD_ROUNDING_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_SUN_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_SUN_BELOW_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_MON_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_MON_BELOW_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_TUE_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_TUE_BELOW_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_WED_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_WED_BELOW_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_THU_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_THU_BELOW_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_FRI_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_FRI_BELOW_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_SAT_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_SAT_BELOW_MINUTES");
                strQry.AppendLine(",T.OVERTIME_IND");
                strQry.AppendLine(",T.SATURDAY_PAY_RATE");
                strQry.AppendLine(",T.SATURDAY_PAY_RATE_IND");
                strQry.AppendLine(",T.SUNDAY_PAY_RATE");
                strQry.AppendLine(",T.SUNDAY_PAY_RATE_IND");
                strQry.AppendLine(",T.OVERTIME1_MINUTES");
                strQry.AppendLine(",T.OVERTIME2_MINUTES");
                strQry.AppendLine(",T.OVERTIME3_MINUTES");

                strQry.AppendLine(",SUM(T.WEEK_OVERTIME_BOUNDARY_MINUTES) AS WEEK_OVERTIME_BOUNDARY_MINUTES");

                strQry.AppendLine(" FROM ");

                strQry.AppendLine(" (SELECT ");
                strQry.AppendLine(" PCWC.PAY_CATEGORY_NO");
                strQry.AppendLine(",PCWC.WEEK_DATE");
                strQry.AppendLine(",PCWC.WEEK_DATE_FROM");
                strQry.AppendLine(",PCWC.MON_TIME_MINUTES");
                strQry.AppendLine(",PCWC.TUE_TIME_MINUTES");
                strQry.AppendLine(",PCWC.WED_TIME_MINUTES");
                strQry.AppendLine(",PCWC.THU_TIME_MINUTES");
                strQry.AppendLine(",PCWC.FRI_TIME_MINUTES");
                strQry.AppendLine(",PCWC.SAT_TIME_MINUTES");
                strQry.AppendLine(",PCWC.SUN_TIME_MINUTES");
                strQry.AppendLine(",C.OVERTIME1_RATE");
                strQry.AppendLine(",C.OVERTIME2_RATE");
                strQry.AppendLine(",C.OVERTIME3_RATE");
                strQry.AppendLine(",PCPC.PAIDHOLIDAY_RATE");
                strQry.AppendLine(",PCPC.TOTAL_DAILY_TIME_OVERTIME");

                strQry.AppendLine(",PCPC.DAILY_ROUNDING_IND");
                strQry.AppendLine(",PCPC.DAILY_ROUNDING_MINUTES");
                strQry.AppendLine(",PCPC.PAY_PERIOD_ROUNDING_IND");
                strQry.AppendLine(",PCPC.PAY_PERIOD_ROUNDING_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_SUN_ABOVE_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_SUN_BELOW_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_MON_ABOVE_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_MON_BELOW_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_TUE_ABOVE_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_TUE_BELOW_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_WED_ABOVE_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_WED_BELOW_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_THU_ABOVE_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_THU_BELOW_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_FRI_ABOVE_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_FRI_BELOW_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_SAT_ABOVE_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_SAT_BELOW_MINUTES");
                strQry.AppendLine(",PCPC.OVERTIME_IND");
                strQry.AppendLine(",PCPC.SATURDAY_PAY_RATE");
                strQry.AppendLine(",PCPC.SATURDAY_PAY_RATE_IND");
                strQry.AppendLine(",PCPC.SUNDAY_PAY_RATE");
                strQry.AppendLine(",PCPC.SUNDAY_PAY_RATE_IND");
                strQry.AppendLine(",PCWC.OVERTIME1_MINUTES");
                strQry.AppendLine(",PCWC.OVERTIME2_MINUTES");
                strQry.AppendLine(",PCWC.OVERTIME3_MINUTES");

                strQry.AppendLine(",WEEK_OVERTIME_BOUNDARY_MINUTES = ");
                strQry.AppendLine(" CASE ");
                //Overtime Paid After Week Hours has been worked (A = Accumulate)
                strQry.AppendLine(" WHEN OVERTIME_IND = 'A' ");

                strQry.AppendLine(" THEN CASE ");
                strQry.AppendLine(" WHEN D.DAY_NO = 0");
                strQry.AppendLine(" THEN PCWC.SUN_TIME_MINUTES");

                strQry.AppendLine(" WHEN D.DAY_NO = 1");
                strQry.AppendLine(" THEN PCWC.MON_TIME_MINUTES");

                strQry.AppendLine(" WHEN D.DAY_NO = 2");
                strQry.AppendLine(" THEN PCWC.TUE_TIME_MINUTES");

                strQry.AppendLine(" WHEN D.DAY_NO = 3");
                strQry.AppendLine(" THEN PCWC.WED_TIME_MINUTES");

                strQry.AppendLine(" WHEN D.DAY_NO = 4");
                strQry.AppendLine(" THEN PCWC.THU_TIME_MINUTES");

                strQry.AppendLine(" WHEN D.DAY_NO = 5");
                strQry.AppendLine(" THEN PCWC.FRI_TIME_MINUTES");

                strQry.AppendLine(" WHEN D.DAY_NO = 6");
                strQry.AppendLine(" THEN PCWC.SAT_TIME_MINUTES");
                strQry.AppendLine(" END ");

                //Overtime Paid After Day Hours has been Exceeded (Default Setting)
                strQry.AppendLine(" ELSE 0");
                strQry.AppendLine(" END ");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_CURRENT PCWC");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");
                strQry.AppendLine(" ON PCWC.COMPANY_NO = PCPC.COMPANY_NO ");
                strQry.AppendLine(" AND PCWC.PAY_CATEGORY_NO = PCPC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCWC.PAY_CATEGORY_TYPE = PCPC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P' ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C");
                strQry.AppendLine(" ON PCWC.COMPANY_NO = C.COMPANY_NO ");
                strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");
                strQry.AppendLine(" ON D.DAY_DATE >= PCWC.WEEK_DATE_FROM");
                strQry.AppendLine(" AND D.DAY_DATE <= PCWC.WEEK_DATE");

                strQry.AppendLine(" WHERE PCWC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PCWC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND PCWC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")) AS T");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" T.PAY_CATEGORY_NO");
                strQry.AppendLine(",T.WEEK_DATE");
                strQry.AppendLine(",T.WEEK_DATE_FROM");
                strQry.AppendLine(",T.MON_TIME_MINUTES");
                strQry.AppendLine(",T.TUE_TIME_MINUTES");
                strQry.AppendLine(",T.WED_TIME_MINUTES");
                strQry.AppendLine(",T.THU_TIME_MINUTES");
                strQry.AppendLine(",T.FRI_TIME_MINUTES");
                strQry.AppendLine(",T.SAT_TIME_MINUTES");
                strQry.AppendLine(",T.SUN_TIME_MINUTES");
                strQry.AppendLine(",T.OVERTIME1_RATE");
                strQry.AppendLine(",T.OVERTIME2_RATE");
                strQry.AppendLine(",T.OVERTIME3_RATE");
                strQry.AppendLine(",T.PAIDHOLIDAY_RATE");
                strQry.AppendLine(",T.TOTAL_DAILY_TIME_OVERTIME");

                strQry.AppendLine(",T.DAILY_ROUNDING_IND");
                strQry.AppendLine(",T.DAILY_ROUNDING_MINUTES");
                strQry.AppendLine(",T.PAY_PERIOD_ROUNDING_IND");
                strQry.AppendLine(",T.PAY_PERIOD_ROUNDING_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_SUN_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_SUN_BELOW_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_MON_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_MON_BELOW_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_TUE_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_TUE_BELOW_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_WED_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_WED_BELOW_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_THU_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_THU_BELOW_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_FRI_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_FRI_BELOW_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_SAT_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_SAT_BELOW_MINUTES");
                strQry.AppendLine(",T.OVERTIME_IND");
                strQry.AppendLine(",T.SATURDAY_PAY_RATE");
                strQry.AppendLine(",T.SATURDAY_PAY_RATE_IND");
                strQry.AppendLine(",T.SUNDAY_PAY_RATE");
                strQry.AppendLine(",T.SUNDAY_PAY_RATE_IND");
                strQry.AppendLine(",T.OVERTIME1_MINUTES");
                strQry.AppendLine(",T.OVERTIME2_MINUTES");
                strQry.AppendLine(",T.OVERTIME3_MINUTES");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" T.PAY_CATEGORY_NO");
                strQry.AppendLine(",T.WEEK_DATE");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategoryWeek", parInt64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",D.IRP5_CODE");
                strQry.AppendLine(",DATEPART(yyyy,EDH.PAY_PERIOD_DATE) AS PERIOD_YEAR");
                strQry.AppendLine(",DATEPART(mm,EDH.PAY_PERIOD_DATE) AS PERIOD_MONTH");
                strQry.AppendLine(",SUM(EDH.TOTAL) AS HISTORY_TOTAL_VALUE");
                strQry.AppendLine(",0 AS TOTAL_VALUE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC ");
                strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y' ");
                //2017-07-19
                strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P' ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.DEDUCTION D ");
                strQry.AppendLine(" ON E.COMPANY_NO = D.COMPANY_NO ");
                strQry.AppendLine(" AND D.IRP5_CODE IN (4001,4006,4005)");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = D.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY EDH");
                strQry.AppendLine(" ON E.COMPANY_NO = EDH.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EDH.EMPLOYEE_NO ");
                strQry.AppendLine(" AND D.DEDUCTION_NO = EDH.DEDUCTION_NO ");
                strQry.AppendLine(" AND EDH.PAY_PERIOD_DATE >= '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");
                // NB RUN_TYPE = All

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",D.IRP5_CODE");
                strQry.AppendLine(",DATEPART(yyyy,EDH.PAY_PERIOD_DATE)");
                strQry.AppendLine(",DATEPART(mm,EDH.PAY_PERIOD_DATE)");

                strQry.AppendLine(" UNION ");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",EN.IRP5_CODE");
                strQry.AppendLine(",DATEPART(yyyy,EEH.PAY_PERIOD_DATE) AS PERIOD_YEAR");
                strQry.AppendLine(",DATEPART(mm,EEH.PAY_PERIOD_DATE) AS PERIOD_MONTH");
                strQry.AppendLine(",SUM(EEH.TOTAL) AS HISTORY_TOTAL_VALUE");
                strQry.AppendLine(",0 AS TOTAL_VALUE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC ");
                strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y' ");
                //2017-07-19
                strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P' ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EN ");
                strQry.AppendLine(" ON E.COMPANY_NO = EN.COMPANY_NO ");
                strQry.AppendLine(" AND EN.IRP5_CODE IN (3810)");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
                strQry.AppendLine(" ON E.COMPANY_NO = EEH.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EEH.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EN.EARNING_NO = EEH.EARNING_NO ");
                //2017-02-17 Cater for Employee change PAY_CATEGORY_TYPE
                //strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EEH.PAY_PERIOD_DATE >= '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");
                // NB RUN_TYPE = All

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",EN.IRP5_CODE");
                strQry.AppendLine(",DATEPART(yyyy,EEH.PAY_PERIOD_DATE)");
                strQry.AppendLine(",DATEPART(mm,EEH.PAY_PERIOD_DATE)");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" 1");
                strQry.AppendLine(",2");
                strQry.AppendLine(",3");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "TaxSpreadSheet", parInt64CompanyNo);

                //2014-03-21 - Commission Used to Subtract from UIF
                strQry.Clear();

                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" EEC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EEC.EMPLOYEE_NO");
                strQry.AppendLine(",EEC.TOTAL");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");

                //2017-01-24 (Employee Not Closed)
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON EEC.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND EEC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                strQry.AppendLine(" WHERE EEC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EEC.RUN_TYPE = 'P' ");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE  = 'W'");

                strQry.AppendLine(" AND EEC.EARNING_NO = 11 ");
                strQry.AppendLine(" AND EEC.TOTAL > 0 ");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Commission", parInt64CompanyNo);

                //2014-09-07
                strQry.Clear();

                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" EEC.EMPLOYEE_NO");
                strQry.AppendLine(",EEC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EEC.EARNING_NO");
                strQry.AppendLine(",EEC.TOTAL AS EARNING_AMOUNT");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC ");

                //2017-01-24 (Employee Not Closed)
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON EEC.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND EEC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                strQry.AppendLine(" WHERE EEC.COMPANY_NO = " + parInt64CompanyNo);
                //7=Bonus
                strQry.AppendLine(" AND (EEC.EARNING_NO = 7 ");
                strQry.AppendLine(" OR (EEC.EARNING_NO > 9 ");
                strQry.AppendLine(" AND EEC.EARNING_NO < 200)) ");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE  = 'W'");
                strQry.AppendLine(" AND EEC.RUN_TYPE = 'P' ");

                strQry.AppendLine(" AND EEC.TOTAL <> 0 ");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EarningAmount", parInt64CompanyNo);

                //Calculates Normal Leave Payout Value when Employee is Closed
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT ");
                strQry.AppendLine(" SET ");

                //Errol - 2015-02-17  
                strQry.AppendLine(" HOURS_DECIMAL = ");
                strQry.AppendLine(" CASE ");

                strQry.AppendLine(" WHEN FINAL_LEAVE_CLOSE_TABLE.MAX_LEAVE_OPTION <> 'P'");

                strQry.AppendLine(" THEN 0 ");

                strQry.AppendLine(" ELSE FINAL_LEAVE_CLOSE_TABLE.HOURS_DECIMAL_OTHER_VALUE ");

                strQry.AppendLine(" END ");

                //Errol - 2015-02-17
                strQry.AppendLine(",HOURS_DECIMAL_ORIGINAL = ");
                strQry.AppendLine(" CASE ");

                strQry.AppendLine(" WHEN FINAL_LEAVE_CLOSE_TABLE.MAX_LEAVE_OPTION <> 'P'");

                strQry.AppendLine(" THEN 0 ");

                strQry.AppendLine(" ELSE FINAL_LEAVE_CLOSE_TABLE.HOURS_DECIMAL_OTHER_VALUE ");

                strQry.AppendLine(" END ");

                strQry.AppendLine(",DAY_DECIMAL_OTHER_VALUE = FINAL_LEAVE_CLOSE_TABLE.DAY_DECIMAL_OTHER_VALUE");

                strQry.AppendLine(",HOURS_DECIMAL_OTHER_VALUE = FINAL_LEAVE_CLOSE_TABLE.HOURS_DECIMAL_OTHER_VALUE ");

                strQry.AppendLine(",TOTAL = ");

                strQry.AppendLine(" CASE ");

                strQry.AppendLine(" WHEN FINAL_LEAVE_CLOSE_TABLE.MAX_LEAVE_OPTION <> 'P'");

                strQry.AppendLine(" THEN 0 ");

                strQry.AppendLine(" ELSE ROUND(FINAL_LEAVE_CLOSE_TABLE.HOURS_DECIMAL_OTHER_VALUE * FINAL_LEAVE_CLOSE_TABLE.HOURLY_RATE, 2)");

                strQry.AppendLine(" END ");

                strQry.AppendLine(" FROM ");

                strQry.AppendLine("(");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EPCC.COMPANY_NO");
                strQry.AppendLine(",EPCC.EMPLOYEE_NO");
                strQry.AppendLine(",LEAVE_CLOSE_TABLE.EARNING_NO");
                strQry.AppendLine(",EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EPCC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EPCC.RUN_TYPE");
                strQry.AppendLine(",EPCC.HOURLY_RATE");
                //2016-12-10 (Payout all Normal Leave Currently Due)
                strQry.AppendLine(",ISNULL(LEAVE_CLOSE_TABLE.MAX_LEAVE_OPTION,'') AS MAX_LEAVE_OPTION");

                strQry.AppendLine(",LEAVE_CLOSE_TABLE.TOTAL_LEAVE_DAYS AS DAY_DECIMAL_OTHER_VALUE ");
                strQry.AppendLine(",ROUND(LEAVE_CLOSE_TABLE.TOTAL_LEAVE_DAYS * EPCC.LEAVE_DAY_RATE_DECIMAL,2) AS HOURS_DECIMAL_OTHER_VALUE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(SELECT ");
                strQry.AppendLine(" LH.EMPLOYEE_NO");
                strQry.AppendLine(",LH.EARNING_NO");

                strQry.AppendLine(",ROUND(ISNULL(FINAL_EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.LEAVE_ACCUM_DAYS,0) + ISNULL(FINAL_EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.PREV_YEAR_LEAVE_ACCUM_DAYS,0) + SUM(LH.LEAVE_ACCUM_DAYS) - SUM(ROUND(LH.LEAVE_PAID_DAYS,2)) - ISNULL(CURRENT_LEAVE_TABLE.LEAVE_DAYS_DECIMAL,0),2) AS TOTAL_LEAVE_DAYS ");
                //2016-12-10 (Payout all Normal Leave Currently Due)
                strQry.AppendLine(",CURRENT_LEAVE_TABLE.MAX_LEAVE_OPTION ");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY LH");

                strQry.AppendLine(" LEFT JOIN ");

                strQry.AppendLine("(");
                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" LC.EMPLOYEE_NO");
                strQry.AppendLine(",SUM(ROUND(LC.LEAVE_DAYS_DECIMAL,2)) AS LEAVE_DAYS_DECIMAL");
                //2016-12-10 (Payout all Normal Leave Currently Due)
                strQry.AppendLine(",MAX(LEAVE_OPTION) AS MAX_LEAVE_OPTION");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT LC");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                strQry.AppendLine(" ON LC.COMPANY_NO = EPCC.COMPANY_NO");
                strQry.AppendLine(" AND LC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                strQry.AppendLine(" AND LC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                //NB - Use Default
                strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y'");
                strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P'");

                strQry.AppendLine(" WHERE LC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND LC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND LC.PROCESS_NO = 0");
                //Normal Leave
                strQry.AppendLine(" AND LC.EARNING_NO = 200");

                strQry.AppendLine(" GROUP BY");
                strQry.AppendLine(" LC.EMPLOYEE_NO");

                strQry.AppendLine(" ) AS CURRENT_LEAVE_TABLE");

                strQry.AppendLine(" ON LH.EMPLOYEE_NO = CURRENT_LEAVE_TABLE.EMPLOYEE_NO ");

                strQry.AppendLine(" LEFT JOIN ");

                //Only Employees Current Period
                strQry.AppendLine("( ");
                //5-Start

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.EMPLOYEE_NO ");

                strQry.AppendLine(",LEAVE_ACCUM_DAYS = ");

                strQry.AppendLine(" CASE ");

                strQry.AppendLine(" WHEN ROUND(ISNULL(EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.LEAVE_ACCUM_DAYS,0) + (ISNULL(EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.DAY_COUNT,0) * EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.NORM_PAID_PER_PERIOD),2) < EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.NORM_PAID_DAYS");
                strQry.AppendLine(" THEN ROUND(CONVERT(DECIMAL,EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.DAY_COUNT) * EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.NORM_PAID_PER_PERIOD,2)");

                strQry.AppendLine(" WHEN EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.NORM_PAID_DAYS < EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.LEAVE_ACCUM_DAYS");
                strQry.AppendLine(" THEN 0");

                strQry.AppendLine(" ELSE EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.NORM_PAID_DAYS - EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.LEAVE_ACCUM_DAYS");

                strQry.AppendLine(" END ");

                strQry.AppendLine(",PREV_YEAR_LEAVE_ACCUM_DAYS = ");

                strQry.AppendLine(" CASE ");

                strQry.AppendLine(" WHEN ROUND(ISNULL(EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.PREV_YEAR_LEAVE_ACCUM_DAYS,0) + (ISNULL(EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.PREV_YEAR_DAY_COUNT,0) * EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.NORM_PAID_PER_PERIOD),2) < EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.NORM_PAID_DAYS");
                strQry.AppendLine(" THEN ROUND(CONVERT(DECIMAL,EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.PREV_YEAR_DAY_COUNT) * EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.NORM_PAID_PER_PERIOD,2)");

                strQry.AppendLine(" WHEN EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.NORM_PAID_DAYS < EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.PREV_YEAR_LEAVE_ACCUM_DAYS");
                strQry.AppendLine(" THEN 0");

                strQry.AppendLine(" ELSE EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.NORM_PAID_DAYS - EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.PREV_YEAR_LEAVE_ACCUM_DAYS");

                strQry.AppendLine(" END ");

                strQry.AppendLine(" FROM");

                strQry.AppendLine("( ");
                //4-Start

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" LH.EMPLOYEE_NO");
                strQry.AppendLine(",EMPLOYEE_DAY_COUNT_TABLE.NORM_PAID_PER_PERIOD ");
                strQry.AppendLine(",EMPLOYEE_DAY_COUNT_TABLE.NORM_PAID_DAYS ");
                strQry.AppendLine(",EMPLOYEE_DAY_COUNT_TABLE.DAY_COUNT");
                strQry.AppendLine(",EMPLOYEE_DAY_COUNT_TABLE.PREV_YEAR_DAY_COUNT");

                strQry.AppendLine(",SUM(");
                strQry.AppendLine(" CASE ");

                strQry.AppendLine(" WHEN LH.PAY_PERIOD_DATE >= '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" THEN ROUND(LH.LEAVE_ACCUM_DAYS,2)");

                strQry.AppendLine(" END) AS LEAVE_ACCUM_DAYS");

                strQry.AppendLine(",SUM(");

                strQry.AppendLine(" CASE ");

                strQry.AppendLine(" WHEN LH.PAY_PERIOD_DATE < '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" THEN ROUND(LH.LEAVE_ACCUM_DAYS,2)");

                strQry.AppendLine(" END) AS PREV_YEAR_LEAVE_ACCUM_DAYS");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY LH");

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("( ");
                //3-Start

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" FINAL_DAY_WORKED_MINUTES_TABLE.EMPLOYEE_NO");
                strQry.AppendLine(",LS.NORM_PAID_DAYS");
                strQry.AppendLine(",LS.NORM_PAID_PER_PERIOD");
                strQry.AppendLine(",SUM (");

                strQry.AppendLine(" CASE ");

                strQry.AppendLine(" WHEN D.DAY_DATE >= '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");

                strQry.AppendLine(" THEN 1");

                strQry.AppendLine(" ELSE 0");

                strQry.AppendLine(" END) AS DAY_COUNT");

                strQry.AppendLine(",SUM (");

                strQry.AppendLine(" CASE ");

                strQry.AppendLine(" WHEN D.DAY_DATE >= '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");

                strQry.AppendLine(" THEN 0");

                strQry.AppendLine(" ELSE 1 ");

                strQry.AppendLine(" END) AS PREV_YEAR_DAY_COUNT ");

                strQry.AppendLine(" FROM ");

                strQry.AppendLine("(");

                //2-Start
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" E.EMPLOYEE_NO");
                //NB This Is the DEfault Pay Category for Employee and ALL Timesheets Across PayCategories are SUMMED
                strQry.AppendLine(",EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",E.LEAVE_SHIFT_NO");
                strQry.AppendLine(",DAY_WORKED_MINUTES_TABLE.TIMESHEET_DATE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("( ");
                //1-Start

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" ETC.EMPLOYEE_NO ");
                strQry.AppendLine(",ETC.TIMESHEET_DATE");
                strQry.AppendLine(",SUM(ETC.TIMESHEET_TIME_OUT_MINUTES) - SUM(ETC.TIMESHEET_TIME_IN_MINUTES) AS DAY_WORKED_MINUTES");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO");
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parInt64CompanyNo);
                //Errol - 2015-02-12
                //strQry.AppendLine(" AND ETC.TIMESHEET_DATE > E.EMPLOYEE_LAST_RUNDATE");

                //Errol - 2015-02-12
                strQry.AppendLine(" AND ((ETC.TIMESHEET_DATE > E.EMPLOYEE_LAST_RUNDATE");
                strQry.AppendLine(" AND E.FIRST_RUN_COMPLETED_IND = 'Y')");
                strQry.AppendLine(" OR (ETC.TIMESHEET_DATE >= E.EMPLOYEE_LAST_RUNDATE");
                strQry.AppendLine(" AND ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= '" + parPayPeriodDate.ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" ETC.EMPLOYEE_NO ");
                strQry.AppendLine(",ETC.TIMESHEET_DATE");
                //1-End
                strQry.AppendLine(") AS DAY_WORKED_MINUTES_TABLE");

                strQry.AppendLine(" ON E.EMPLOYEE_NO = DAY_WORKED_MINUTES_TABLE.EMPLOYEE_NO");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                //NB There is NO Join On PAY_CATEGORY_NO so That All Timesheets are Linked To Default PAY_CATEGORY_NO
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                //NB Default Employee Cost Centre
                strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y'");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT_CURRENT LSC");
                strQry.AppendLine(" ON E.COMPANY_NO = LSC.COMPANY_NO");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = LSC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND LSC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND E.LEAVE_SHIFT_NO = LSC.LEAVE_SHIFT_NO");
                strQry.AppendLine(" AND EPCC.RUN_NO = LSC.RUN_NO");
                //Exclude Records That don't Exceed Minimum Time for a Leave Shift
                strQry.AppendLine(" AND DAY_WORKED_MINUTES_TABLE.DAY_WORKED_MINUTES >= LSC.MIN_VALID_SHIFT_MINUTES");

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");

                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                //2-End
                strQry.AppendLine(") AS FINAL_DAY_WORKED_MINUTES_TABLE");

                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D ");
                strQry.AppendLine(" ON FINAL_DAY_WORKED_MINUTES_TABLE.TIMESHEET_DATE = D.DAY_DATE");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT_CURRENT LS");

                strQry.AppendLine(" ON LS.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND FINAL_DAY_WORKED_MINUTES_TABLE.PAY_CATEGORY_NO = LS.PAY_CATEGORY_NO");
                //Wages - WHAT ABOUT SALARIES
                strQry.AppendLine(" AND LS.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND FINAL_DAY_WORKED_MINUTES_TABLE.LEAVE_SHIFT_NO = LS.LEAVE_SHIFT_NO");
                strQry.AppendLine(" AND (((LS.LEAVE_PAID_ACCUMULATOR_IND = 1");
                strQry.AppendLine(" AND D.DAY_NO IN (1,2,3,4,5))");
                //Saturday Included
                strQry.AppendLine(" OR (LS.LEAVE_PAID_ACCUMULATOR_IND = 2");
                strQry.AppendLine(" AND D.DAY_NO IN (1,2,3,4,5,6)))");
                //Sunday Included
                strQry.AppendLine(" OR (LS.LEAVE_PAID_ACCUMULATOR_IND = 3");
                strQry.AppendLine(" AND D.DAY_NO IN (0,1,2,3,4,5,6)))");

                //Not Public Holiday
                strQry.AppendLine(" LEFT JOIN ");

                strQry.AppendLine("(SELECT ");
                strQry.AppendLine(" PCPC.PAY_CATEGORY_NO");
                strQry.AppendLine(",PHC.PUBLIC_HOLIDAY_DATE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_CURRENT PHC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC ");
                strQry.AppendLine(" ON PHC.COMPANY_NO = PCPC.COMPANY_NO ");
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P'");
                strQry.AppendLine(" AND PCPC.PAY_PUBLIC_HOLIDAY_IND = 'Y'");

                strQry.AppendLine(" WHERE PHC.COMPANY_NO = " + parInt64CompanyNo + ") AS TEMP_TABLE2");

                strQry.AppendLine(" ON LS.PAY_CATEGORY_NO = TEMP_TABLE2.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND  D.DAY_DATE = TEMP_TABLE2.PUBLIC_HOLIDAY_DATE");

                //Not Public Holiday
                strQry.AppendLine(" WHERE TEMP_TABLE2.PAY_CATEGORY_NO IS NULL");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" FINAL_DAY_WORKED_MINUTES_TABLE.EMPLOYEE_NO");
                strQry.AppendLine(",LS.NORM_PAID_DAYS");
                strQry.AppendLine(",LS.NORM_PAID_PER_PERIOD");
                //3-End

                strQry.AppendLine(" ) AS EMPLOYEE_DAY_COUNT_TABLE ");

                strQry.AppendLine(" ON LH.EMPLOYEE_NO = EMPLOYEE_DAY_COUNT_TABLE.EMPLOYEE_NO  ");

                strQry.AppendLine(" WHERE LH.COMPANY_NO = " + parInt64CompanyNo);
                //2017-02-21 - Removed to cater For Employee PAY_CATEGORY_TYPE Change
                //strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

                strQry.AppendLine(" AND LH.PAY_PERIOD_DATE >= '" + dtStartTaxYear.AddYears(-1).ToString("yyyy-MM-dd") + "'");

                //Accumulated Leave / Take-On Balance
                strQry.AppendLine(" AND LH.PROCESS_NO IN(98,99)");
                //Normal Leave
                strQry.AppendLine(" AND LH.EARNING_NO = 200");

                strQry.AppendLine(" GROUP BY ");

                strQry.AppendLine(" LH.EMPLOYEE_NO ");
                strQry.AppendLine(",EMPLOYEE_DAY_COUNT_TABLE.NORM_PAID_PER_PERIOD ");
                strQry.AppendLine(",EMPLOYEE_DAY_COUNT_TABLE.NORM_PAID_DAYS ");
                strQry.AppendLine(",EMPLOYEE_DAY_COUNT_TABLE.DAY_COUNT ");
                strQry.AppendLine(",EMPLOYEE_DAY_COUNT_TABLE.PREV_YEAR_DAY_COUNT");

                //4-End
                strQry.AppendLine(") AS EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE ");

                //5-End
                strQry.AppendLine(") AS FINAL_EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE ");

                strQry.AppendLine(" ON LH.EMPLOYEE_NO = FINAL_EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.EMPLOYEE_NO  ");

                strQry.AppendLine(" WHERE LH.COMPANY_NO = " + parInt64CompanyNo);
                //2017-02-21 - Removed to cater For Employee PAY_CATEGORY_TYPE Change
                //strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND LH.EARNING_NO = 200");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" LH.EMPLOYEE_NO");
                strQry.AppendLine(",LH.EARNING_NO");
                strQry.AppendLine(",FINAL_EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.LEAVE_ACCUM_DAYS");
                strQry.AppendLine(",FINAL_EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.PREV_YEAR_LEAVE_ACCUM_DAYS");
                strQry.AppendLine(",CURRENT_LEAVE_TABLE.LEAVE_DAYS_DECIMAL");

                strQry.AppendLine(",CURRENT_LEAVE_TABLE.MAX_LEAVE_OPTION) AS LEAVE_CLOSE_TABLE");

                strQry.AppendLine(" ON EPCC.EMPLOYEE_NO = LEAVE_CLOSE_TABLE.EMPLOYEE_NO");

                strQry.AppendLine(" WHERE EPCC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                //NB - Use Default
                strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y'");
                strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P') AS FINAL_LEAVE_CLOSE_TABLE");

                strQry.AppendLine(" WHERE EMPLOYEE_EARNING_CURRENT.COMPANY_NO = FINAL_LEAVE_CLOSE_TABLE.COMPANY_NO");
                strQry.AppendLine(" AND EMPLOYEE_EARNING_CURRENT.EMPLOYEE_NO = FINAL_LEAVE_CLOSE_TABLE.EMPLOYEE_NO");
                strQry.AppendLine(" AND EMPLOYEE_EARNING_CURRENT.EARNING_NO = FINAL_LEAVE_CLOSE_TABLE.EARNING_NO");
                strQry.AppendLine(" AND EMPLOYEE_EARNING_CURRENT.PAY_CATEGORY_NO = FINAL_LEAVE_CLOSE_TABLE.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EMPLOYEE_EARNING_CURRENT.PAY_CATEGORY_TYPE = FINAL_LEAVE_CLOSE_TABLE.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EMPLOYEE_EARNING_CURRENT.RUN_TYPE = FINAL_LEAVE_CLOSE_TABLE.RUN_TYPE");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //2016-12-10 - Payout of Normal Leave for Close or Pay Normal Leave Due
                strQry.Clear();

                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" EEC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EEC.EMPLOYEE_NO");
                strQry.AppendLine(",EEC.TOTAL");
                strQry.AppendLine(",EEC.HOURS_DECIMAL");
                strQry.AppendLine(",EEC.HOURS_DECIMAL_OTHER_VALUE");
                strQry.AppendLine(",LC.LEAVE_OPTION");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");

                //2017-01-24 (Employee Not Closed)
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON EEC.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND EEC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT LC");
                strQry.AppendLine(" ON EEC.COMPANY_NO = LC.COMPANY_NO");
                strQry.AppendLine(" AND EEC.EMPLOYEE_NO = LC.EMPLOYEE_NO");
                strQry.AppendLine(" AND EEC.EARNING_NO = LC.EARNING_NO");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = LC.PAY_CATEGORY_TYPE");

                strQry.AppendLine(" AND LC.PROCESS_NO = 0");
                strQry.AppendLine(" AND LC.LEAVE_OPTION = 'P'");

                strQry.AppendLine(" WHERE EEC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EEC.RUN_TYPE = 'P' ");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE  = 'W'");

                strQry.AppendLine(" AND EEC.EARNING_NO = 200 ");
                strQry.AppendLine(" AND EEC.HOURS_DECIMAL_OTHER_VALUE > 0 ");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "NormalLeaveClose", parInt64CompanyNo);

                //2017-07-11
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.PAYROLL_RUN_QUEUE");

                strQry.AppendLine(" SET ");
                strQry.AppendLine(" PAYROLL_RUN_QUEUE_IND = 'S'");
                strQry.AppendLine(",START_RUN_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                DataViewCompanyPaidPublicHoliday = null;
                DataViewCompanyPaidPublicHoliday = new DataView(DataSet.Tables["CompanyPaidPublicHoliday"],
                     "",
                    "EMPLOYEE_NO",
                    DataViewRowState.CurrentRows);

                for (int intEmployeeRow = 0; intEmployeeRow < DataSet.Tables["Employee"].Rows.Count; intEmployeeRow++)
                {
#if (DEBUG)
                    int intEmployeeNo = Convert.ToInt32(DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"]);

                    if (intEmployeeNo == 12)
                    {
                        string strStop = "";
                    }
#endif
                    intMedicalAidNumberDependents = Convert.ToInt32(DataSet.Tables["Employee"].Rows[intEmployeeRow]["NUMBER_MEDICAL_AID_DEPENDENTS"]);

                    dblTaxEarningsYTD = 0;
                    dblTaxEarningsOtherYTD = 0;
                    dblEarningsCurrent = 0;
                    dblCompanyPaidHoliday = 0;

                    DataViewEmployeeParameters = null;
                    DataViewEmployeeParameters = new DataView(DataSet.Tables["EmployeeParameters"],
                        "EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString(),
                        "",
                        DataViewRowState.CurrentRows);

                    //Loop Through Each Employee's Pay Categories
                    for (int intEmployeeParameterRow = 0; intEmployeeParameterRow < DataViewEmployeeParameters.Count; intEmployeeParameterRow++)
                    {
                        //DataSet.Tables["EarningAmount"].Clear();

                        DataViewEarningAmount = null;
                        DataViewEarningAmount = new DataView(DataSet.Tables["EarningAmount"],
                             "EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString() + " AND PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString(),
                             "EARNING_NO",
                             DataViewRowState.CurrentRows);

                        dtDataRow = DataSet.Tables["EarningAmount"].NewRow();
                        dtDataRow["EMPLOYEE_NO"] = DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString();
                        dtDataRow["PAY_CATEGORY_NO"] = DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString();
                        dtDataRow["EARNING_NO"] = 2;
                        dtDataRow["EARNING_AMOUNT"] = 0;
                        DataSet.Tables["EarningAmount"].Rows.Add(dtDataRow);

                        dtDataRow = DataSet.Tables["EarningAmount"].NewRow();
                        dtDataRow["EMPLOYEE_NO"] = DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString();
                        dtDataRow["PAY_CATEGORY_NO"] = DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString();
                        dtDataRow["EARNING_NO"] = 3;
                        dtDataRow["EARNING_AMOUNT"] = 0;
                        DataSet.Tables["EarningAmount"].Rows.Add(dtDataRow);

                        dtDataRow = DataSet.Tables["EarningAmount"].NewRow();
                        dtDataRow["EMPLOYEE_NO"] = DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString();
                        dtDataRow["PAY_CATEGORY_NO"] = DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString();
                        dtDataRow["EARNING_NO"] = 4;
                        dtDataRow["EARNING_AMOUNT"] = 0;
                        DataSet.Tables["EarningAmount"].Rows.Add(dtDataRow);

                        dtDataRow = DataSet.Tables["EarningAmount"].NewRow();
                        dtDataRow["EMPLOYEE_NO"] = DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString();
                        dtDataRow["PAY_CATEGORY_NO"] = DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString();
                        dtDataRow["EARNING_NO"] = 5;
                        dtDataRow["EARNING_AMOUNT"] = 0;
                        DataSet.Tables["EarningAmount"].Rows.Add(dtDataRow);

                        dblTaxYTD = 0;
                        dblTaxEarningsYTD += Convert.ToDouble(DataViewEmployeeParameters[intEmployeeParameterRow]["EARNINGS_YTD"]) + Convert.ToDouble(DataViewEmployeeParameters[intEmployeeParameterRow]["EARNINGS_OTHER_YTD"]);
                        dblTaxEarningsOtherYTD += Convert.ToDouble(DataViewEmployeeParameters[intEmployeeParameterRow]["EARNINGS_OTHER_YTD"]);

                        //2013-09-30
                        blnBonusInd = false;

                        strEmployeeExceptionIndicator = "";
                        strEmployeeNormalIndicator = "";
                        strEmployeeBlankIndicator = "";
                        strPayUIFInd = "N";
                        strMedicalAidInd = "N";

                        intPeriodNormalTimeMinutes = 0;
                        intPeriodOverTime1Minutes = 0;
                        intPeriodOverTime2Minutes = 0;
                        intPeriodOverTime3Minutes = 0;
                        intPeriodPaidHolidayWorkedMinutes = 0;

                        dblPaidHolidayWorkedHoursDecimal = 0;

                        dtEmployeeLastRundate = Convert.ToDateTime(DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_LAST_RUNDATE"]);

                        DataViewPayCategoryWeek = null;
                        DataViewPayCategoryWeek = new DataView(DataSet.Tables["PayCategoryWeek"],
                            "PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString(),
                            "",
                            DataViewRowState.CurrentRows);

                        blnEmployeeTimeSheetsForPayCategory = false;

                        //By Week//By Week
                        for (int intPayCategoryRow = 0; intPayCategoryRow < DataViewPayCategoryWeek.Count; intPayCategoryRow++)
                        {
                            DataViewTimeSheetDayTotal = null;
                            DataViewTimeSheetDayTotal = new DataView(DataSet.Tables["TimeSheetDayTotals"],
                                "PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString() + " AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString()
                                + " AND DAY_DATE <= '" + Convert.ToDateTime(DataViewPayCategoryWeek[intPayCategoryRow]["WEEK_DATE"]).ToString("yyyy-MM-dd")
                                + "' AND DAY_DATE >= '" + Convert.ToDateTime(DataViewPayCategoryWeek[intPayCategoryRow]["WEEK_DATE_FROM"]).ToString("yyyy-MM-dd") + "'",
                                "DAY_DATE",
                                DataViewRowState.CurrentRows);

                            if (DataViewTimeSheetDayTotal.Count == 0)
                            {
                                continue;
                            }
                            else
                            {
                                blnEmployeeTimeSheetsForPayCategory = true;
                            }

                            //Used To Work Out If Week has Blank Records
                            DataSet.Tables["DaysInWeek"].Rows.Clear();

                            strEmployeeWeekExceptionIndicator = "";
                            strEmployeeWeekNormalIndicator = "";
                            strEmployeeWeekBlankIndicator = "";

                            intTotalMinutesPaidForWeek = 0;
                            intNormalHoursBoundary = 0;
                            intOverTime1HoursBoundary = 0;
                            intOverTime2HoursBoundary = 0;
                            intOverTime3HoursBoundary = 0;
                            intOverTimeCF = 0;

                            //Accumulate Days Worked
                            if (DataViewPayCategoryWeek[intPayCategoryRow]["OVERTIME_IND"].ToString() == "A")
                            {
                                System.TimeSpan ts = Convert.ToDateTime(DataViewPayCategoryWeek[intPayCategoryRow]["WEEK_DATE"]).Subtract(Convert.ToDateTime(DataViewPayCategoryWeek[intPayCategoryRow]["WEEK_DATE_FROM"]));

                                if (ts.Days == 6
                                    | Convert.ToInt32(Convert.ToDateTime(DataViewPayCategoryWeek[intPayCategoryRow]["WEEK_DATE_FROM"]).DayOfWeek) == 1)
                                {
                                    intNormalHoursBoundary = Convert.ToInt32(DataViewPayCategoryWeek[intPayCategoryRow]["WEEK_OVERTIME_BOUNDARY_MINUTES"]);

                                    if (ts.Days != 6
                                        & Convert.ToInt32(Convert.ToDateTime(DataViewPayCategoryWeek[intPayCategoryRow]["WEEK_DATE_FROM"]).DayOfWeek) == 1)
                                    {
                                        //Needs To Be Checked
                                        intOverTimeCF = Convert.ToInt32(DataViewPayCategoryWeek[intPayCategoryRow]["WEEK_OVERTIME_BOUNDARY_MINUTES"]);
                                    }
                                }
                                else
                                {
                                    intNormalHoursBoundary = Convert.ToInt32(DataViewPayCategoryWeek[intPayCategoryRow]["WEEK_OVERTIME_BOUNDARY_MINUTES"]);
                                }
                            }
                            else
                            {
                                //Exceeds 
                            }

                            intWeekNormalMinutes = 0;
                            intWeekOverTime1Minutes = 0;
                            intWeekOverTime2Minutes = 0;
                            intWeekOverTime3Minutes = 0;
                            intWeekPaidHolidayWorkedMinutes = 0;

                            //Set to Do Week Day First (Eg Thursday,Friday,Monday,Tuesday.Wednesday before WeekEnd Days
                            blnWeekDays = true;

                        Calculate_Wages_From_TimeSheets_Do_WeekEnd:

                            for (int intDayRow = 0; intDayRow < DataViewTimeSheetDayTotal.Count; intDayRow++)
                            {
                                if (blnWeekDays == true)
                                {
                                    //WeekEnd
                                    if (Convert.ToInt32(Convert.ToDateTime(DataViewTimeSheetDayTotal[intDayRow]["DAY_DATE"]).DayOfWeek) == 0
                                    | Convert.ToInt32(Convert.ToDateTime(DataViewTimeSheetDayTotal[intDayRow]["DAY_DATE"]).DayOfWeek) == 6)
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    //WeekDays
                                    if (Convert.ToInt32(Convert.ToDateTime(DataViewTimeSheetDayTotal[intDayRow]["DAY_DATE"]).DayOfWeek) > 0
                                    & Convert.ToInt32(Convert.ToDateTime(DataViewTimeSheetDayTotal[intDayRow]["DAY_DATE"]).DayOfWeek) < 6)
                                    {
                                        continue;
                                    }
                                }

                                if (DataSet.Tables["DaysInWeek"].Rows.Count > 0)
                                {
                                    if (Convert.ToDateTime(DataSet.Tables["DaysInWeek"].Rows[DataSet.Tables["DaysInWeek"].Rows.Count - 1]["DAY_DATE"]) != Convert.ToDateTime(DataViewTimeSheetDayTotal[intDayRow]["DAY_DATE"]))
                                    {
                                        DataRow myDataRow = DataSet.Tables["DaysInWeek"].NewRow();
                                        myDataRow["DAY_DATE"] = Convert.ToDateTime(DataViewTimeSheetDayTotal[intDayRow]["DAY_DATE"]);
                                        DataSet.Tables["DaysInWeek"].Rows.Add(myDataRow);
                                    }
                                }
                                else
                                {
                                    DataRow myDataRow = DataSet.Tables["DaysInWeek"].NewRow();
                                    myDataRow["DAY_DATE"] = Convert.ToDateTime(DataViewTimeSheetDayTotal[intDayRow]["DAY_DATE"]);
                                    DataSet.Tables["DaysInWeek"].Rows.Add(myDataRow);
                                }

                                intTotalMinutesPaidForWeek += Convert.ToInt32(DataViewTimeSheetDayTotal[intDayRow]["DAY_PAID_MINUTES"]);

                                //Day is Public Holiday - Has it's Own Roleip Column
                                if (DataViewTimeSheetDayTotal[intDayRow]["PAIDHOLIDAY_INDICATOR"].ToString() == "Y")
                                {
                                    intWeekPaidHolidayWorkedMinutes += Convert.ToInt32(DataViewTimeSheetDayTotal[intDayRow]["DAY_PAID_MINUTES"]);
                                    continue;
                                }

                                clsISUtilities.Calculate_Wage_Time_Breakdown(DataViewPayCategoryWeek, intPayCategoryRow,
                                    Convert.ToInt32(Convert.ToDateTime(DataViewTimeSheetDayTotal[intDayRow]["DAY_DATE"]).DayOfWeek), Convert.ToInt32(DataViewTimeSheetDayTotal[intDayRow]["DAY_PAID_MINUTES"]),
                                    intNormalHoursBoundary,
                                    ref intOverTime1HoursBoundary, ref intOverTime2HoursBoundary,
                                    ref intOverTime3HoursBoundary,
                                    ref intDayNormalHours,
                                    ref intDayOverTime1Hours, ref intDayOverTime2Hours,
                                    ref intDayOverTime3Hours,
                                    ref intWeekNormalMinutes, ref intWeekOverTime1Minutes,
                                    ref intWeekOverTime2Minutes, ref intWeekOverTime3Minutes);

                                if (DataViewTimeSheetDayTotal[intDayRow]["INDICATOR"].ToString() == "E")
                                {
                                    strEmployeeWeekExceptionIndicator = "Y";
                                }
                                else
                                {
                                    strEmployeeWeekNormalIndicator = "Y";
                                }
                            }

                            if (blnWeekDays == true)
                            {
                                blnWeekDays = false;
                                goto Calculate_Wages_From_TimeSheets_Do_WeekEnd;
                            }

                            TimeSpan tsTimeSpan = Convert.ToDateTime(DataViewPayCategoryWeek[intPayCategoryRow]["WEEK_DATE"]).AddDays(1).Subtract(Convert.ToDateTime(DataViewPayCategoryWeek[intPayCategoryRow]["WEEK_DATE_FROM"]));

                            //Blank Records in Week for Employee
                            if (DataSet.Tables["DaysInWeek"].Rows.Count == tsTimeSpan.Days)
                            {
                                strEmployeeWeekBlankIndicator = "";
                            }
                            else
                            {
                                strEmployeeWeekBlankIndicator = "Y";
                            }

                            strQry.Clear();

                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_WEEK_CURRENT");
                            strQry.AppendLine(" SET ");
                            strQry.AppendLine(" NORMALTIME_MINUTES = " + intWeekNormalMinutes);
                            strQry.AppendLine(",OVERTIME1_MINUTES = " + intWeekOverTime1Minutes);
                            strQry.AppendLine(",OVERTIME2_MINUTES = " + intWeekOverTime2Minutes);
                            strQry.AppendLine(",OVERTIME3_MINUTES = " + intWeekOverTime3Minutes);
                            strQry.AppendLine(",PAIDHOLIDAY_MINUTES = " + intWeekPaidHolidayWorkedMinutes);
                            strQry.AppendLine(",TOTAL_MINUTES = " + intTotalMinutesPaidForWeek);
                            strQry.AppendLine(",EXCEPTION_INDICATOR = " + clsDBConnectionObjects.Text2DynamicSQL(strEmployeeWeekExceptionIndicator));
                            strQry.AppendLine(",NORMAL_INDICATOR = " + clsDBConnectionObjects.Text2DynamicSQL(strEmployeeWeekNormalIndicator));
                            strQry.AppendLine(",BLANK_INDICATOR = " + clsDBConnectionObjects.Text2DynamicSQL(strEmployeeWeekBlankIndicator));

                            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"]);
                            strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString());
                            strQry.AppendLine(" AND WEEK_DATE = '" + Convert.ToDateTime(DataViewPayCategoryWeek[intPayCategoryRow]["WEEK_DATE"]).ToString("yyyy-MM-dd") + "'");

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                            //Add Week Totals To Pay Period Totals
                            intPeriodNormalTimeMinutes += intWeekNormalMinutes;
                            intPeriodOverTime1Minutes += intWeekOverTime1Minutes;
                            intPeriodOverTime2Minutes += intWeekOverTime2Minutes;
                            intPeriodOverTime3Minutes += intWeekOverTime3Minutes;
                            intPeriodPaidHolidayWorkedMinutes += intWeekPaidHolidayWorkedMinutes;

                            if (strEmployeeWeekExceptionIndicator == "Y")
                            {
                                strEmployeeExceptionIndicator = "Y";
                            }
                            else
                            {
                                if (strEmployeeWeekBlankIndicator == "Y")
                                {
                                    strEmployeeBlankIndicator = "Y";
                                }
                                else
                                {
                                    strEmployeeNormalIndicator = "Y";
                                }
                            }
                        }

                        if (blnEmployeeTimeSheetsForPayCategory == false)
                        {
                            if (DataViewEmployeeParameters[intEmployeeParameterRow]["DEFAULT_IND"].ToString() != "Y")
                            {
                                //2017-07-31
                                strQry.Clear();

                                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT");

                                strQry.AppendLine(" SET PAY_CATEGORY_USED_IND = 'N'");

                                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                                strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString());
                                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString());
                                strQry.AppendLine(" AND RUN_TYPE = 'P'");


                                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                                continue;
                            }
                        }

                        //Round Hours 
                        intNormalTimeMinutesRounded = intPeriodNormalTimeMinutes;
                        intOverTime1MinutesRounded = intPeriodOverTime1Minutes;
                        intOverTime2MinutesRounded = intPeriodOverTime2Minutes;
                        intOverTime3MinutesRounded = intPeriodOverTime3Minutes;
                        intPaidHolidayWorkedMinutesRounded = intPeriodPaidHolidayWorkedMinutes;

                        if (intNormalTimeMinutesRounded > 0)
                        {
                            clsISUtilities.Round_For_Period(Convert.ToInt32(DataViewPayCategoryWeek[0]["PAY_PERIOD_ROUNDING_IND"]), Convert.ToInt32(DataViewPayCategoryWeek[0]["PAY_PERIOD_ROUNDING_MINUTES"]), ref intNormalTimeMinutesRounded);
                        }

                        if (intOverTime1MinutesRounded > 0)
                        {
                            clsISUtilities.Round_For_Period(Convert.ToInt32(DataViewPayCategoryWeek[0]["PAY_PERIOD_ROUNDING_IND"]), Convert.ToInt32(DataViewPayCategoryWeek[0]["PAY_PERIOD_ROUNDING_MINUTES"]), ref intOverTime1MinutesRounded);
                        }

                        if (intOverTime2MinutesRounded > 0)
                        {
                            clsISUtilities.Round_For_Period(Convert.ToInt32(DataViewPayCategoryWeek[0]["PAY_PERIOD_ROUNDING_IND"]), Convert.ToInt32(DataViewPayCategoryWeek[0]["PAY_PERIOD_ROUNDING_MINUTES"]), ref intOverTime2MinutesRounded);
                        }

                        if (intOverTime3MinutesRounded > 0)
                        {
                            clsISUtilities.Round_For_Period(Convert.ToInt32(DataViewPayCategoryWeek[0]["PAY_PERIOD_ROUNDING_IND"]), Convert.ToInt32(DataViewPayCategoryWeek[0]["PAY_PERIOD_ROUNDING_MINUTES"]), ref intOverTime3MinutesRounded);
                        }

                        if (intPaidHolidayWorkedMinutesRounded > 0)
                        {
                            clsISUtilities.Round_For_Period(Convert.ToInt32(DataViewPayCategoryWeek[0]["PAY_PERIOD_ROUNDING_IND"]), Convert.ToInt32(DataViewPayCategoryWeek[0]["PAY_PERIOD_ROUNDING_MINUTES"]), ref intPaidHolidayWorkedMinutesRounded);
                        }

                        dblNormalHoursDecimal = clsISUtilities.Convert_Time_To_Decimal(intNormalTimeMinutesRounded);
                        dblOverTime1HoursDecimal = clsISUtilities.Convert_Time_To_Decimal(intOverTime1MinutesRounded);
                        dblOverTime2HoursDecimal = clsISUtilities.Convert_Time_To_Decimal(intOverTime2MinutesRounded);
                        dblOverTime3HoursDecimal = clsISUtilities.Convert_Time_To_Decimal(intOverTime3MinutesRounded);
                        dblPaidHolidayWorkedHoursDecimal = clsISUtilities.Convert_Time_To_Decimal(intPaidHolidayWorkedMinutesRounded);

                        //ELR 2014-04-19
                        dblOverTimeRate1 = Math.Round(Convert.ToDouble(DataViewEmployeeParameters[intEmployeeParameterRow]["HOURLY_RATE"]) * (Convert.ToDouble(DataViewPayCategoryWeek[0]["OVERTIME1_RATE"])), 2);
                        dblOverTimeRate2 = Math.Round(Convert.ToDouble(DataViewEmployeeParameters[intEmployeeParameterRow]["HOURLY_RATE"]) * (Convert.ToDouble(DataViewPayCategoryWeek[0]["OVERTIME2_RATE"])), 2);
                        dblOverTimeRate3 = Math.Round(Convert.ToDouble(DataViewEmployeeParameters[intEmployeeParameterRow]["HOURLY_RATE"]) * (Convert.ToDouble(DataViewPayCategoryWeek[0]["OVERTIME3_RATE"])), 2);

                        dblNormalTotal = Math.Round(dblNormalHoursDecimal * Convert.ToDouble(DataViewEmployeeParameters[intEmployeeParameterRow]["HOURLY_RATE"]), 2);
                        dblOverTime1Total = Math.Round(dblOverTime1HoursDecimal * dblOverTimeRate1, 2);
                        dblOverTime2Total = Math.Round(dblOverTime2HoursDecimal * dblOverTimeRate2, 2);
                        dblOverTime3Total = Math.Round(dblOverTime3HoursDecimal * dblOverTimeRate3, 2);
                        dblPaidHolidayWorkedTotal = Math.Round(dblPaidHolidayWorkedHoursDecimal * Convert.ToDouble(DataViewEmployeeParameters[intEmployeeParameterRow]["HOURLY_RATE"]) * (Convert.ToDouble(DataViewPayCategoryWeek[0]["PAIDHOLIDAY_RATE"])), 2);

                        //Normal Time
                        strQry.Clear();

                        strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" MINUTES = " + intPeriodNormalTimeMinutes);
                        strQry.AppendLine(",MINUTES_ROUNDED = " + intNormalTimeMinutesRounded);
                        strQry.AppendLine(",HOURS_DECIMAL = " + dblNormalHoursDecimal);
                        strQry.AppendLine(",HOURS_DECIMAL_ORIGINAL = " + dblNormalHoursDecimal);
                        strQry.AppendLine(",TOTAL = " + dblNormalTotal);

                        strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"]);
                        strQry.AppendLine(" AND EARNING_NO = 2 ");
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                        strQry.AppendLine(" AND RUN_TYPE = 'P' ");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                        //OverTime1
                        strQry.Clear();

                        strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" MINUTES = " + intPeriodOverTime1Minutes);
                        strQry.AppendLine(",MINUTES_ROUNDED = " + intOverTime1MinutesRounded);
                        strQry.AppendLine(",HOURS_DECIMAL = " + dblOverTime1HoursDecimal);
                        strQry.AppendLine(",HOURS_DECIMAL_ORIGINAL = " + dblOverTime1HoursDecimal);
                        strQry.AppendLine(",TOTAL = " + dblOverTime1Total);

                        strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"]);
                        strQry.AppendLine(" AND EARNING_NO = 3 ");
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                        strQry.AppendLine(" AND RUN_TYPE = 'P' ");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                        //OverTime2
                        strQry.Clear();

                        strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" MINUTES = " + intPeriodOverTime2Minutes);
                        strQry.AppendLine(",MINUTES_ROUNDED = " + intOverTime2MinutesRounded);
                        strQry.AppendLine(",HOURS_DECIMAL = " + dblOverTime2HoursDecimal);
                        strQry.AppendLine(",HOURS_DECIMAL_ORIGINAL = " + dblOverTime2HoursDecimal);
                        strQry.AppendLine(",TOTAL = " + dblOverTime2Total);

                        strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"]);
                        strQry.AppendLine(" AND EARNING_NO = 4 ");
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                        strQry.AppendLine(" AND RUN_TYPE = 'P' ");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                        //OverTime3
                        strQry.Clear();

                        strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" MINUTES = " + intPeriodOverTime3Minutes);
                        strQry.AppendLine(",MINUTES_ROUNDED = " + intOverTime3MinutesRounded);
                        strQry.AppendLine(",HOURS_DECIMAL = " + dblOverTime3HoursDecimal);
                        strQry.AppendLine(",HOURS_DECIMAL_ORIGINAL = " + dblOverTime3HoursDecimal);
                        strQry.AppendLine(",TOTAL = " + dblOverTime3Total);

                        strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString());
                        strQry.AppendLine(" AND EARNING_NO = 5 ");
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                        strQry.AppendLine(" AND RUN_TYPE = 'P' ");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                        strQry.Clear();

                        strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT");
                        strQry.AppendLine(" SET ");

                        if (intOverTimeCF != 0)
                        {
                            strQry.AppendLine(" OVERTIME_VALUE_CF = " + (intOverTimeCF - intTotalMinutesPaidForWeek));
                            strQry.AppendLine(",EXCEPTION_INDICATOR = " + clsDBConnectionObjects.Text2DynamicSQL(strEmployeeExceptionIndicator));
                        }
                        else
                        {
                            strQry.AppendLine(" EXCEPTION_INDICATOR = " + clsDBConnectionObjects.Text2DynamicSQL(strEmployeeExceptionIndicator));
                        }

                        strQry.AppendLine(",NORMAL_INDICATOR = " + clsDBConnectionObjects.Text2DynamicSQL(strEmployeeNormalIndicator));
                        strQry.AppendLine(",BLANK_INDICATOR = " + clsDBConnectionObjects.Text2DynamicSQL(strEmployeeBlankIndicator));

                        strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                        intEarningAmountRow = DataViewEarningAmount.Find(2);
                        DataViewEarningAmount[intEarningAmountRow]["EARNING_AMOUNT"] = Convert.ToDouble(DataViewEarningAmount[intEarningAmountRow]["EARNING_AMOUNT"]) + dblNormalTotal;

                        intEarningAmountRow = DataViewEarningAmount.Find(3);
                        DataViewEarningAmount[intEarningAmountRow]["EARNING_AMOUNT"] = Convert.ToDouble(DataViewEarningAmount[intEarningAmountRow]["EARNING_AMOUNT"]) + dblOverTime1Total;

                        intEarningAmountRow = DataViewEarningAmount.Find(4);
                        DataViewEarningAmount[intEarningAmountRow]["EARNING_AMOUNT"] = Convert.ToDouble(DataViewEarningAmount[intEarningAmountRow]["EARNING_AMOUNT"]) + dblOverTime2Total;

                        intEarningAmountRow = DataViewEarningAmount.Find(5);
                        DataViewEarningAmount[intEarningAmountRow]["EARNING_AMOUNT"] = Convert.ToDouble(DataViewEarningAmount[intEarningAmountRow]["EARNING_AMOUNT"]) + dblOverTime3Total;

                        //NB The Default Pay Category Must be the last Pay Category for an Employee (Handled in the Sort)
                        if (DataViewEmployeeParameters[intEmployeeParameterRow]["DEFAULT_IND"].ToString() == "Y")
                        {
                            DataViewEmployeeLeaveTotal = null;
                            DataViewEmployeeLeaveTotal = new DataView(DataSet.Tables["EmployeeLeaveTotal"],
                                "PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString() + " AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString(),
                                "",
                                DataViewRowState.CurrentRows);

                            //2014-03-21
                            DataViewCommission = null;
                            DataViewCommission = new DataView(DataSet.Tables["Commission"],
                               "PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString() + " AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"],
                               "",
                               DataViewRowState.CurrentRows);

                            //Find Any Leave Records
                            for (int intLeaveRow = 0; intLeaveRow < DataViewEmployeeLeaveTotal.Count; intLeaveRow++)
                            {
                                dblLeaveDecimal = Convert.ToDouble(DataViewEmployeeLeaveTotal[intLeaveRow]["LEAVE_HOURS_DECIMAL"]);
                                dblLeaveTotal = Math.Round(dblLeaveDecimal * Convert.ToDouble(DataViewEmployeeParameters[intEmployeeParameterRow]["HOURLY_RATE"]) * (Convert.ToDouble(DataViewEmployeeLeaveTotal[intLeaveRow]["LEAVE_PERCENTAGE"]) / 100), 2);
                                dblLeaveHoursOtherValue = 0;
                                dblLeaveDayOtherValue = 0;

                                if (Convert.ToInt32(DataViewEmployeeLeaveTotal[intLeaveRow]["EARNING_NO"]) == 200)
                                {
                                    DataViewNormalLeaveClose = null;
                                    DataViewNormalLeaveClose = new DataView(DataSet.Tables["NormalLeaveClose"],
                                    "PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString() + " AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"],
                                    "",
                                    DataViewRowState.CurrentRows);

                                    if (DataViewNormalLeaveClose.Count > 0)
                                    {
                                        if (DataViewNormalLeaveClose[0]["LEAVE_OPTION"] != System.DBNull.Value)
                                        {
                                            //Pay Out Normal Leave Due
                                            dblLeaveDecimal = dblLeaveDecimal + Convert.ToDouble(DataViewNormalLeaveClose[0]["HOURS_DECIMAL"]);
                                            dblLeaveTotal = dblLeaveTotal + Convert.ToDouble(DataViewNormalLeaveClose[0]["TOTAL"]);

                                            dblLeaveHoursOtherValue = 0;
                                        }
                                        else
                                        {
                                            dblLeaveHoursOtherValue = Convert.ToDouble(DataViewNormalLeaveClose[0]["HOURS_DECIMAL_OTHER_VALUE"]);
                                        }

                                        //dblLeaveDayOtherValue = dblLeaveTotal + Convert.ToDouble(DataViewNormalLeaveClose[0]["TOTAL"]);
                                    }
                                }

                                dtDataRow = DataSet.Tables["EarningAmount"].NewRow();
                                dtDataRow["EMPLOYEE_NO"] = DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString();
                                dtDataRow["PAY_CATEGORY_NO"] = DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString();
                                dtDataRow["EARNING_NO"] = Convert.ToInt32(DataViewEmployeeLeaveTotal[intLeaveRow]["EARNING_NO"]);
                                dtDataRow["EARNING_AMOUNT"] = dblLeaveTotal;
                                DataSet.Tables["EarningAmount"].Rows.Add(dtDataRow);

                                strQry.Clear();

                                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
                                strQry.AppendLine(" SET ");
                                strQry.AppendLine(" HOURS_DECIMAL = " + dblLeaveDecimal);
                                strQry.AppendLine(",HOURS_DECIMAL_ORIGINAL = " + dblLeaveDecimal);
                                strQry.AppendLine(",TOTAL = " + dblLeaveTotal);

                                strQry.AppendLine(",HOURS_DECIMAL_OTHER_VALUE = " + dblLeaveHoursOtherValue);
                                //TO BE LOOKED AT
                                //strQry.AppendLine(",DAY_DECIMAL_OTHER_VALUE = " + dblLeaveDayOtherValue);

                                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                                strQry.AppendLine(" AND EMPLOYEE_NO = " + DataViewEmployeeLeaveTotal[intLeaveRow]["EMPLOYEE_NO"].ToString());
                                strQry.AppendLine(" AND EARNING_NO = " + DataViewEmployeeLeaveTotal[intLeaveRow]["EARNING_NO"].ToString());
                                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString());
                                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                                strQry.AppendLine(" AND RUN_TYPE = 'P' ");

                                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                            }

                            //Find Company Paid Public Holiday Row
                            inCompanyPaidPublicHolidayRow = DataViewCompanyPaidPublicHoliday.Find(DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString());

                            if (inCompanyPaidPublicHolidayRow > -1)
                            {
                                dblPaidHolidayTotal = Convert.ToDouble(DataViewCompanyPaidPublicHoliday[inCompanyPaidPublicHolidayRow]["TOTAL"]);
                            }
                            else
                            {
                                dblPaidHolidayTotal = 0;
                            }

                            dtDataRow = DataSet.Tables["EarningAmount"].NewRow();
                            dtDataRow["EMPLOYEE_NO"] = DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString();
                            dtDataRow["PAY_CATEGORY_NO"] = DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString();
                            dtDataRow["EARNING_NO"] = 8;
                            dtDataRow["EARNING_AMOUNT"] = dblPaidHolidayTotal;
                            DataSet.Tables["EarningAmount"].Rows.Add(dtDataRow);

                            dblPaidHolidayWorkedTotal = Math.Round(dblPaidHolidayWorkedHoursDecimal * Convert.ToDouble(DataViewEmployeeParameters[intEmployeeParameterRow]["HOURLY_RATE"]) * (Convert.ToDouble(DataViewPayCategoryWeek[0]["PAIDHOLIDAY_RATE"])), 2);

                            dtDataRow = DataSet.Tables["EarningAmount"].NewRow();
                            dtDataRow["EMPLOYEE_NO"] = DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString();
                            dtDataRow["PAY_CATEGORY_NO"] = DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString();
                            dtDataRow["EARNING_NO"] = 9;
                            dtDataRow["EARNING_AMOUNT"] = dblPaidHolidayWorkedTotal;
                            DataSet.Tables["EarningAmount"].Rows.Add(dtDataRow);

                            DataViewTaxSpreadSheet = null;
                            DataViewTaxSpreadSheet = new DataView(DataSet.Tables["TaxSpreadSheet"],
                                "PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString() + " AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"],
                                "IRP5_CODE,PERIOD_YEAR,PERIOD_MONTH",

                                DataViewRowState.CurrentRows);

                            //Employees Deductions
                            DataViewEmployeeDeduction = null;
                            DataViewEmployeeDeduction = new DataView(DataSet.Tables["EmployeeDeduction"],
                                "PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString() + " AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"],
                                "",
                                DataViewRowState.CurrentRows);

                            //Employees Loans
                            DataViewEmployeeLoan = null;
                            DataViewEmployeeLoan = new DataView(DataSet.Tables["EmployeeLoan"],
                                "PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString() + " AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"],
                                "DEDUCTION_NO,DEDUCTION_SUB_ACCOUNT_NO",
                                DataViewRowState.CurrentRows);

                            blnDeductionFound = false;
                            intLoanRow = -1;

                            for (int intEmployeeDeductionRow = 0; intEmployeeDeductionRow < DataViewEmployeeDeduction.Count; intEmployeeDeductionRow++)
                            {
                                if (Convert.ToInt32(DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_NO"]) == 1)
                                {
                                    dblTaxYTD = Convert.ToDouble(DataViewEmployeeDeduction[intEmployeeDeductionRow]["TOTAL_YTD_BF"]);
                                    continue;
                                }
                                else
                                {
                                    if (Convert.ToInt32(DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_NO"]) == 2)
                                    {
                                        strPayUIFInd = "Y";
                                        continue;
                                    }
                                    else
                                    {
                                        if (Convert.ToInt32(DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_NO"]) == 5)
                                        {
                                            strMedicalAidInd = "Y";
                                            continue;
                                        }
                                    }
                                }

                                //Errol Changed
                                if (DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_TYPE_IND"].ToString() != "U")
                                {
                                    blnDeductionFound = true;

                                    if (DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_PERIOD_IND"].ToString() == "M")
                                    {
                                        blnDeductionFound = false;

                                        string strDate = Convert.ToDateTime(DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_LAST_RUNDATE"]).ToString("yyyy-MM-") + Convert.ToInt32(DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_DAY_VALUE"]).ToString("00");

                                        dtDate = DateTime.ParseExact(strDate, "yyyy-MM-dd", null);

                                        if (Convert.ToDateTime(DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_LAST_RUNDATE"]) >= dtDate)
                                        {
                                            dtDate = dtDate.AddMonths(1);
                                        }

                                        if (dtDate > Convert.ToDateTime(DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_LAST_RUNDATE"])
                                        && dtDate <= parPayPeriodDate)
                                        {
                                            blnDeductionFound = true;
                                        }
                                    }

                                    //Deduction based on Formula against Earnings
                                    if (DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_TYPE_IND"].ToString() == "P")
                                    {
                                        if (blnDeductionFound == true)
                                        {
                                            dblDeductionType3Total = 0;

                                            DataViewEmployeeDeductionEarningPercentage = null;
                                            DataViewEmployeeDeductionEarningPercentage = new DataView(DataSet.Tables["EmployeeDeductionEarningPercentage"],
                                                "PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString() + " AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString() + " AND DEDUCTION_NO = " + DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_NO"].ToString(),
                                                "",
                                                DataViewRowState.CurrentRows);

                                            for (int intRow = 0; intRow < DataViewEmployeeDeductionEarningPercentage.Count; intRow++)
                                            {
                                                int intEarningNo = Convert.ToInt32(DataViewEmployeeDeductionEarningPercentage[intRow]["EARNING_NO"]);

                                                intEarningAmountRow = DataViewEarningAmount.Find(DataViewEmployeeDeductionEarningPercentage[intRow]["EARNING_NO"].ToString());

                                                if (intEarningAmountRow != -1)
                                                {
                                                    dblDeductionType3Total += Convert.ToDouble(DataViewEarningAmount[intEarningAmountRow]["EARNING_AMOUNT"]);
                                                }
                                            }

                                            //Calculate Using Percentage Value
                                            dblDeductionType3Total = Math.Round(dblDeductionType3Total * (Convert.ToDouble(DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_VALUE"]) / 100), 2);

                                            if (Convert.ToDouble(DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_MIN_VALUE"]) > dblDeductionType3Total)
                                            {
                                                dblDeductionType3Total = Convert.ToDouble(DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_MIN_VALUE"]);
                                            }

                                            if (Convert.ToDouble(DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_MAX_VALUE"]) < dblDeductionType3Total
                                                & Convert.ToDouble(DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_MAX_VALUE"]) != 0)
                                            {
                                                dblDeductionType3Total = Convert.ToDouble(DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_MAX_VALUE"]);
                                            }

                                            strQry.Clear();

                                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT");

                                            //Test Against Loan Balance
                                            if (DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_LOAN_TYPE_IND"].ToString() == "Y"
                                                & dblDeductionType3Total != 0)
                                            {
                                                objKey[0] = DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_NO"].ToString();
                                                objKey[1] = DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_SUB_ACCOUNT_NO"].ToString();

                                                intLoanRow = DataViewEmployeeLoan.Find(objKey);

                                                if (intLoanRow > -1)
                                                {
                                                    //Found
                                                    if (dblDeductionType3Total > Convert.ToDouble(DataViewEmployeeLoan[intLoanRow]["TOTAL_LOAN"]))
                                                    {
                                                        dblDeductionType3Total = Convert.ToDouble(DataViewEmployeeLoan[intLoanRow]["TOTAL_LOAN"]);
                                                    }
                                                }
                                                else
                                                {
                                                    dblDeductionType3Total = 0;
                                                }
                                            }

                                            dblDeductionFinalTotal = dblDeductionType3Total + Convert.ToDouble(DataViewEmployeeDeduction[intEmployeeDeductionRow]["TOTAL_YTD_BF"]);

                                            strQry.AppendLine(" SET ");
                                            strQry.AppendLine(" TOTAL = " + dblDeductionType3Total);
                                            strQry.AppendLine(",TOTAL_ORIGINAL = " + dblDeductionType3Total);

                                            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                                            strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString());
                                            strQry.AppendLine(" AND DEDUCTION_NO = " + DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_NO"].ToString());
                                            strQry.AppendLine(" AND DEDUCTION_SUB_ACCOUNT_NO = " + DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_SUB_ACCOUNT_NO"].ToString());
                                            strQry.AppendLine(" AND RUN_TYPE = 'P' ");
                                        }
                                        else
                                        {
                                            dblDeductionFinalTotal = Convert.ToDouble(DataViewEmployeeDeduction[intEmployeeDeductionRow]["TOTAL_YTD_BF"]);

                                            strQry.Clear();

                                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT");
                                            strQry.AppendLine(" SET ");
                                            strQry.AppendLine(" TOTAL = 0");
                                            strQry.AppendLine(",TOTAL_ORIGINAL = 0");

                                            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                                            strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"]);
                                            strQry.AppendLine(" AND DEDUCTION_NO = " + DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_NO"]);
                                            strQry.AppendLine(" AND DEDUCTION_SUB_ACCOUNT_NO = " + DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_SUB_ACCOUNT_NO"].ToString());
                                            strQry.AppendLine(" AND RUN_TYPE = 'P' ");
                                        }
                                    }
                                    else
                                    {
                                        //Deduction Type 2
                                        if (blnDeductionFound == true)
                                        {
                                            //Test Against Loan Balance
                                            if (DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_LOAN_TYPE_IND"].ToString() == "Y")
                                            {
                                                dblDeductionFinalTotal = Convert.ToDouble(DataViewEmployeeDeduction[intEmployeeDeductionRow]["TOTAL_YTD_BF"]);

                                                objKey[0] = DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_NO"].ToString();
                                                objKey[1] = DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_SUB_ACCOUNT_NO"].ToString();

                                                intLoanRow = DataViewEmployeeLoan.Find(objKey);

                                                strQry.Clear();

                                                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT");
                                                strQry.AppendLine(" SET ");

                                                if (intLoanRow > -1)
                                                {
                                                    if (Convert.ToDouble(DataViewEmployeeLoan[intLoanRow]["TOTAL_LOAN"]) > 0)
                                                    {
                                                        if (Convert.ToDouble(DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_VALUE"]) > Convert.ToDouble(DataViewEmployeeLoan[intLoanRow]["TOTAL_LOAN"]))
                                                        {
                                                            dblDeductionFinalTotal = Convert.ToDouble(DataViewEmployeeLoan[intLoanRow]["TOTAL_LOAN"]) + Convert.ToDouble(DataViewEmployeeDeduction[intEmployeeDeductionRow]["TOTAL_YTD_BF"]);

                                                            strQry.AppendLine(" TOTAL = " + DataViewEmployeeLoan[intLoanRow]["TOTAL_LOAN"].ToString());
                                                            strQry.AppendLine(",TOTAL_ORIGINAL = " + DataViewEmployeeLoan[intLoanRow]["TOTAL_LOAN"].ToString());
                                                        }
                                                        else
                                                        {
                                                            dblDeductionFinalTotal = Convert.ToDouble(DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_VALUE"]) + Convert.ToDouble(DataViewEmployeeDeduction[intEmployeeDeductionRow]["TOTAL_YTD_BF"]);

                                                            strQry.AppendLine(" TOTAL = " + DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_VALUE"].ToString());
                                                            strQry.AppendLine(",TOTAL_ORIGINAL = " + DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_VALUE"].ToString());
                                                        }
                                                    }
                                                    else
                                                    {
                                                        strQry.AppendLine(" TOTAL = 0");
                                                        strQry.AppendLine(",TOTAL_ORIGINAL = 0");
                                                    }
                                                }
                                                else
                                                {
                                                    strQry.AppendLine(" TOTAL = 0");
                                                    strQry.AppendLine(",TOTAL_ORIGINAL = 0");
                                                }

                                                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                                                strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString());
                                                strQry.AppendLine(" AND DEDUCTION_NO = " + DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_NO"].ToString());
                                                strQry.AppendLine(" AND DEDUCTION_SUB_ACCOUNT_NO = " + DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_SUB_ACCOUNT_NO"].ToString());
                                                strQry.AppendLine(" AND RUN_TYPE = 'P' ");

                                            }
                                            else
                                            {
                                                dblDeductionFinalTotal = Convert.ToDouble(DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_VALUE"]) + Convert.ToDouble(DataViewEmployeeDeduction[intEmployeeDeductionRow]["TOTAL_YTD_BF"]);

                                                strQry.Clear();

                                                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT");
                                                strQry.AppendLine(" SET ");
                                                strQry.AppendLine(" TOTAL = " + DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_VALUE"].ToString());
                                                strQry.AppendLine(",TOTAL_ORIGINAL = " + DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_VALUE"].ToString());

                                                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                                                strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString());
                                                strQry.AppendLine(" AND DEDUCTION_NO = " + DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_NO"].ToString());
                                                strQry.AppendLine(" AND DEDUCTION_SUB_ACCOUNT_NO = " + DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_SUB_ACCOUNT_NO"].ToString());
                                                strQry.AppendLine(" AND RUN_TYPE = 'P' ");
                                            }
                                        }
                                        else
                                        {
                                            dblDeductionFinalTotal = Convert.ToDouble(DataViewEmployeeDeduction[intEmployeeDeductionRow]["TOTAL_YTD_BF"]);

                                            strQry.Clear();

                                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT");
                                            strQry.AppendLine(" SET ");
                                            strQry.AppendLine(" TOTAL = 0");
                                            strQry.AppendLine(",TOTAL_ORIGINAL = 0");

                                            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                                            strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString());
                                            strQry.AppendLine(" AND DEDUCTION_NO = " + DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_NO"].ToString());
                                            strQry.AppendLine(" AND DEDUCTION_SUB_ACCOUNT_NO = " + DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_SUB_ACCOUNT_NO"].ToString());
                                            strQry.AppendLine(" AND RUN_TYPE = 'P' ");
                                        }
                                    }

                                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                                    if (DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_NO"].ToString() == "3"
                                        | DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_NO"].ToString() == "4"
                                        | DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_NO"].ToString() == "5")
                                    {
                                        string strQryNew = strQry.ToString();

                                        dblTaxSpreadSheetValue = Convert.ToDouble(strQryNew.Substring(strQryNew.IndexOf("=") + 1, strQryNew.IndexOf(",") - (strQryNew.IndexOf("=") + 1)).Trim());

                                        if (DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_NO"].ToString() == "3")
                                        {
                                            intIRP5 = 4001;
                                        }
                                        else
                                        {
                                            if (DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_NO"].ToString() == "4")
                                            {
                                                intIRP5 = 4006;
                                            }
                                            else
                                            {
                                                intIRP5 = 4005;
                                            }
                                        }

                                        objFindTaxSpreadSheet[0] = intIRP5;
                                        objFindTaxSpreadSheet[1] = parPayPeriodDate.Year;
                                        objFindTaxSpreadSheet[2] = parPayPeriodDate.Month;

                                        intTaxSpreadSheetRow = DataViewTaxSpreadSheet.Find(objFindTaxSpreadSheet);

                                        //Only be True on a Second Run
                                        if (intTaxSpreadSheetRow > -1)
                                        {
                                            DataViewTaxSpreadSheet[intTaxSpreadSheetRow]["TOTAL_VALUE"] = dblTaxSpreadSheetValue;
                                        }
                                        else
                                        {
                                            drvDataRowView = DataViewTaxSpreadSheet.AddNew();
                                            //Set Key for Find
                                            drvDataRowView["PAY_CATEGORY_NO"] = DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString();
                                            drvDataRowView["EMPLOYEE_NO"] = DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString();
                                            drvDataRowView["IRP5_CODE"] = intIRP5;
                                            drvDataRowView["PERIOD_YEAR"] = parPayPeriodDate.Year;
                                            drvDataRowView["PERIOD_MONTH"] = parPayPeriodDate.Month;
                                            drvDataRowView["HISTORY_TOTAL_VALUE"] = 0;
                                            drvDataRowView["TOTAL_VALUE"] = dblTaxSpreadSheetValue;

                                            drvDataRowView.EndEdit();
                                        }
                                    }

                                    //Add To YTD Totals - Used in Tax Calculation
                                    switch (Convert.ToInt32(DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_NO"]))
                                    {
                                        case 5:

                                            ///dblMedicalAidYTD = dblDeductionFinalTotal;

                                            break;


                                        default:

                                            break;
                                    }
                                }
                            }

                            dtEmployeeBirthDate = Convert.ToDateTime(DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_BIRTHDATE"]);
                            dtEmployeeStartDate = Convert.ToDateTime(DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_TAX_STARTDATE"]);
                            dtEmployeeLastRunDate = Convert.ToDateTime(DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_LAST_RUNDATE"]);

                            for (int intEarningRow = 0; intEarningRow < DataViewEarningAmount.Count; intEarningRow++)
                            {
                                dblTaxEarningsYTD += Convert.ToDouble(DataViewEarningAmount[intEarningRow]["EARNING_AMOUNT"]);

                                //2014-09-06
                                //7=Bonus
                                if (Convert.ToInt32(DataViewEarningAmount[intEarningRow]["EARNING_NO"]) == 7)
                                {
                                    dblTaxEarningsOtherYTD += Convert.ToDouble(DataViewEarningAmount[intEarningRow]["EARNING_AMOUNT"]);

                                    if (Convert.ToDouble(DataViewEarningAmount[intEarningRow]["EARNING_AMOUNT"]) > 0)
                                    {
                                        blnBonusInd = true;
                                    }
                                }

                                dblEarningsCurrent += Convert.ToDouble(DataViewEarningAmount[intEarningRow]["EARNING_AMOUNT"]);

                                if (Convert.ToInt32(DataViewEarningAmount[intEarningRow]["EARNING_NO"]) == 8)
                                {
                                    dblCompanyPaidHoliday = Convert.ToDouble(DataViewEarningAmount[intEarningRow]["EARNING_AMOUNT"]);
                                }
                            }

                            //Get Factors for Tax Calculation
                            Tax.Employee_Date_Calculations(parPayPeriodDate, dtEmployeeBirthDate,
                                dtStartTaxYear, dtEndTaxYear,
                                dtEmployeeStartDate, dblDaysInYear,
                                ref dblEmployeePortionOfYear, ref dblAgeAtTaxYearEnd,
                                ref dblEmployeeAnnualisedFactor, "W",
                                dtEmployeeLastRunDate,
                                ref dblEmployeeDaysWorked);

                            double dblWageMonth = parPayPeriodDate.Month;

                            if (DataViewCommission.Count > 0)
                            {
                                dblCommissionAmount = Convert.ToDouble(DataViewCommission[0]["TOTAL"]);
                            }
                            else
                            {
                                dblCommissionAmount = 0;
                            }

                            //Reset (To be Calculated)
                            dblUIFAmount = 0;

                            intReturnCode = Tax.Calculate_Tax(dblTaxEarningsYTD, dblTaxEarningsOtherYTD,
                                ref dblTaxCalculatedRun, dblEmployeeAnnualisedFactor,
                                dblAgeAtTaxYearEnd, dblWageMonth, dblTaxYTD,
                                "P", 0, "W", strPayUIFInd, dblDaysInYear, dblEmployeeDaysWorked, dblEarningsCurrent,
                                ref dblUIFAmount, parPayPeriodDate, strMedicalAidInd, intMedicalAidNumberDependents, DataViewTaxSpreadSheet,
                                null, dblPensionArrearYTD, dblRetireAnnuityArrearYTD,
                                DataSet.Tables["Employee"].Rows[intEmployeeRow]["TAX_TYPE_IND"].ToString(),
                                Convert.ToDouble(DataSet.Tables["Employee"].Rows[intEmployeeRow]["TAX_DIRECTIVE_PERCENTAGE"]),
                                dblCommissionAmount,
                                ref dblRetirementAnnuityAmount,
                                ref dblRetirementAnnuityTotal,
                                ref dblPensionFundAmount,
                                ref dblPensionFundTotal,
                                ref dblTotalNormalEarnings,
                                ref dblTotalNormalEarningsAnnualised,
                                ref dblTotalEarnedAccumAnnualInitial,
                                ref dblTotalDeductions,
                                ref dblTaxTotal,
                                ref intTaxTableRow,
                                ref intEarningsTaxTableRow,
                                ref dblUifTotal
                                );

                            strQry.Clear();

                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT");

                            if (dblEarningsCurrent != 0)
                            {
                                if (dblEarningsCurrent - dblCompanyPaidHoliday == 0)
                                {
                                    //P = Passed - Still can be deleted if Company Paid Holiday is Removed
                                    strQry.AppendLine(" SET PAYSLIP_IND = 'P'");
                                }
                                else
                                {
                                    strQry.AppendLine(" SET PAYSLIP_IND = 'Y'");
                                }
                            }
                            else
                            {
                                if (blnBonusInd == true)
                                {
                                    //Bonus
                                    strQry.AppendLine(" SET PAYSLIP_IND = 'Y'");
                                }
                                else
                                {
                                    strQry.AppendLine(" SET PAYSLIP_IND = 'N'");
                                }
                            }

                            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine(" AND RUN_TYPE = 'P' ");
                            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                            strQry.AppendLine(" AND RUN_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["RUN_NO"].ToString());

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                            //Tax Deduction Number = 1
                            dblDeductionFinalTotal = dblTaxCalculatedRun + dblTaxYTD;

                            strQry.Clear();

                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT");
                            strQry.AppendLine(" SET ");
                            strQry.AppendLine(" TOTAL = " + dblTaxCalculatedRun);
                            strQry.AppendLine(",TOTAL_ORIGINAL = " + dblTaxCalculatedRun);

                            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine(" AND DEDUCTION_NO = 1");
                            strQry.AppendLine(" AND DEDUCTION_SUB_ACCOUNT_NO = 1");
                            strQry.AppendLine(" AND RUN_TYPE = 'P' ");

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                            //UIF
                            strQry.Clear();

                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT");
                            strQry.AppendLine(" SET ");
                            strQry.AppendLine(" TOTAL = " + dblUIFAmount);
                            strQry.AppendLine(",TOTAL_ORIGINAL = " + dblUIFAmount);

                            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine(" AND DEDUCTION_NO = 2");
                            strQry.AppendLine(" AND DEDUCTION_SUB_ACCOUNT_NO = 1");
                            strQry.AppendLine(" AND RUN_TYPE = 'P' ");

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                            strQry.Clear();

                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
                            strQry.AppendLine(" SET ");
                            strQry.AppendLine(" MINUTES = " + intPeriodPaidHolidayWorkedMinutes);
                            strQry.AppendLine(",MINUTES_ROUNDED = " + intPaidHolidayWorkedMinutesRounded);
                            strQry.AppendLine(",HOURS_DECIMAL = " + dblPaidHolidayWorkedHoursDecimal);
                            strQry.AppendLine(",HOURS_DECIMAL_ORIGINAL = " + dblPaidHolidayWorkedHoursDecimal);
                            strQry.AppendLine(",TOTAL = " + dblPaidHolidayWorkedTotal);

                            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine(" AND EARNING_NO = 9 ");
                            strQry.AppendLine(" AND RUN_TYPE = 'P' ");

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                        }
                    }
                }

                ////Calculates Normal Leave Payout Value when Employee is Closed
                //strQry.Clear();

                //strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT ");
                //strQry.AppendLine(" SET ");

                ////Errol - 2015-02-17  
                //strQry.AppendLine(" HOURS_DECIMAL = ");
                //strQry.AppendLine(" CASE ");

                //strQry.AppendLine(" WHEN FINAL_LEAVE_CLOSE_TABLE.MAX_LEAVE_OPTION <> 'P'");

                //strQry.AppendLine(" THEN HOURS_DECIMAL ");

                //strQry.AppendLine(" ELSE HOURS_DECIMAL + FINAL_LEAVE_CLOSE_TABLE.HOURS_DECIMAL_OTHER_VALUE ");

                //strQry.AppendLine(" END ");

                ////Errol - 2015-02-17
                //strQry.AppendLine(",HOURS_DECIMAL_ORIGINAL = ");
                //strQry.AppendLine(" CASE ");

                //strQry.AppendLine(" WHEN FINAL_LEAVE_CLOSE_TABLE.MAX_LEAVE_OPTION <> 'P'");

                //strQry.AppendLine(" THEN HOURS_DECIMAL_ORIGINAL ");

                //strQry.AppendLine(" ELSE HOURS_DECIMAL_ORIGINAL + FINAL_LEAVE_CLOSE_TABLE.HOURS_DECIMAL_OTHER_VALUE ");

                //strQry.AppendLine(" END ");

                //strQry.AppendLine(",DAY_DECIMAL_OTHER_VALUE = FINAL_LEAVE_CLOSE_TABLE.DAY_DECIMAL_OTHER_VALUE");

                //strQry.AppendLine(",HOURS_DECIMAL_OTHER_VALUE = ");

                //strQry.AppendLine(" CASE ");

                //strQry.AppendLine(" WHEN FINAL_LEAVE_CLOSE_TABLE.MAX_LEAVE_OPTION <> 'P'");

                //strQry.AppendLine(" THEN FINAL_LEAVE_CLOSE_TABLE.HOURS_DECIMAL_OTHER_VALUE ");

                //strQry.AppendLine(" ELSE HOURS_DECIMAL_ORIGINAL + FINAL_LEAVE_CLOSE_TABLE.HOURS_DECIMAL_OTHER_VALUE ");

                //strQry.AppendLine(" END ");

                //strQry.AppendLine(",TOTAL = ");

                //strQry.AppendLine(" CASE ");

                //strQry.AppendLine(" WHEN FINAL_LEAVE_CLOSE_TABLE.MAX_LEAVE_OPTION <> 'P'");

                //strQry.AppendLine(" THEN TOTAL ");

                //strQry.AppendLine(" ELSE ROUND((HOURS_DECIMAL_ORIGINAL + FINAL_LEAVE_CLOSE_TABLE.HOURS_DECIMAL_OTHER_VALUE) * FINAL_LEAVE_CLOSE_TABLE.HOURLY_RATE, 2)");

                //strQry.AppendLine(" END ");

                //strQry.AppendLine(" FROM ");

                //strQry.AppendLine("(");

                //strQry.AppendLine(" SELECT ");
                //strQry.AppendLine(" EPCC.COMPANY_NO");
                //strQry.AppendLine(",EPCC.EMPLOYEE_NO");
                //strQry.AppendLine(",LEAVE_CLOSE_TABLE.EARNING_NO");
                //strQry.AppendLine(",EPCC.PAY_CATEGORY_NO");
                //strQry.AppendLine(",EPCC.PAY_CATEGORY_TYPE");
                //strQry.AppendLine(",EPCC.RUN_TYPE");
                //strQry.AppendLine(",EPCC.HOURLY_RATE");
                ////2016-12-10 (Payout all Normal Leave Currently Due)
                //strQry.AppendLine(",LEAVE_CLOSE_TABLE.MAX_LEAVE_OPTION");
                //strQry.AppendLine(",LEAVE_CLOSE_TABLE.TOTAL_LEAVE_DAYS AS DAY_DECIMAL_OTHER_VALUE ");
                //strQry.AppendLine(",ROUND(LEAVE_CLOSE_TABLE.TOTAL_LEAVE_DAYS * EPCC.LEAVE_DAY_RATE_DECIMAL,2) AS HOURS_DECIMAL_OTHER_VALUE");

                //strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");

                //strQry.AppendLine(" INNER JOIN ");

                //strQry.AppendLine("(SELECT ");
                //strQry.AppendLine(" LH.EMPLOYEE_NO");
                //strQry.AppendLine(",LH.EARNING_NO");

                //strQry.AppendLine(",ROUND(ISNULL(FINAL_EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.LEAVE_ACCUM_DAYS,0) + ISNULL(FINAL_EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.PREV_YEAR_LEAVE_ACCUM_DAYS,0) + SUM(LH.LEAVE_ACCUM_DAYS) - SUM(ROUND(LH.LEAVE_PAID_DAYS,2)) - ISNULL(CURRENT_LEAVE_TABLE.LEAVE_DAYS_DECIMAL,0),2) AS TOTAL_LEAVE_DAYS ");
                ////2016-12-10 (Payout all Normal Leave Currently Due)
                //strQry.AppendLine(",CURRENT_LEAVE_TABLE.MAX_LEAVE_OPTION ");

                //strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY LH");

                //strQry.AppendLine(" LEFT JOIN ");

                //strQry.AppendLine("(");
                //strQry.AppendLine(" SELECT");
                //strQry.AppendLine(" LC.EMPLOYEE_NO");
                //strQry.AppendLine(",SUM(ROUND(LC.LEAVE_DAYS_DECIMAL,2)) AS LEAVE_DAYS_DECIMAL");
                ////2016-12-10 (Payout all Normal Leave Currently Due)
                //strQry.AppendLine(",MAX(LEAVE_OPTION) AS MAX_LEAVE_OPTION");

                //strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT LC");

                //strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                //strQry.AppendLine(" ON LC.COMPANY_NO = EPCC.COMPANY_NO");
                //strQry.AppendLine(" AND LC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                //strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                //strQry.AppendLine(" AND LC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                ////NB - Use Default
                //strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y'");
                //strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P'");

                //strQry.AppendLine(" WHERE LC.COMPANY_NO = " + parInt64CompanyNo);
                //strQry.AppendLine(" AND LC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                //strQry.AppendLine(" AND LC.PROCESS_NO = 0");
                ////Normal Leave
                //strQry.AppendLine(" AND LC.EARNING_NO = 200");

                //strQry.AppendLine(" GROUP BY");
                //strQry.AppendLine(" LC.EMPLOYEE_NO");

                //strQry.AppendLine(" ) AS CURRENT_LEAVE_TABLE");

                //strQry.AppendLine(" ON LH.EMPLOYEE_NO = CURRENT_LEAVE_TABLE.EMPLOYEE_NO ");

                //strQry.AppendLine(" LEFT JOIN ");

                ////Only Employees Current Period
                //strQry.AppendLine("( ");
                ////5-Start

                //strQry.AppendLine(" SELECT ");
                //strQry.AppendLine(" EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.EMPLOYEE_NO ");

                //strQry.AppendLine(",LEAVE_ACCUM_DAYS = ");

                //strQry.AppendLine(" CASE ");

                //strQry.AppendLine(" WHEN ROUND(ISNULL(EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.LEAVE_ACCUM_DAYS,0) + (ISNULL(EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.DAY_COUNT,0) * EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.NORM_PAID_PER_PERIOD),2) < EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.NORM_PAID_DAYS");
                //strQry.AppendLine(" THEN ROUND(CONVERT(DECIMAL,EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.DAY_COUNT) * EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.NORM_PAID_PER_PERIOD,2)");

                //strQry.AppendLine(" WHEN EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.NORM_PAID_DAYS < EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.LEAVE_ACCUM_DAYS");
                //strQry.AppendLine(" THEN 0");

                //strQry.AppendLine(" ELSE EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.NORM_PAID_DAYS - EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.LEAVE_ACCUM_DAYS");

                //strQry.AppendLine(" END ");

                //strQry.AppendLine(",PREV_YEAR_LEAVE_ACCUM_DAYS = ");

                //strQry.AppendLine(" CASE ");

                //strQry.AppendLine(" WHEN ROUND(ISNULL(EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.PREV_YEAR_LEAVE_ACCUM_DAYS,0) + (ISNULL(EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.PREV_YEAR_DAY_COUNT,0) * EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.NORM_PAID_PER_PERIOD),2) < EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.NORM_PAID_DAYS");
                //strQry.AppendLine(" THEN ROUND(CONVERT(DECIMAL,EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.PREV_YEAR_DAY_COUNT) * EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.NORM_PAID_PER_PERIOD,2)");

                //strQry.AppendLine(" WHEN EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.NORM_PAID_DAYS < EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.PREV_YEAR_LEAVE_ACCUM_DAYS");
                //strQry.AppendLine(" THEN 0");

                //strQry.AppendLine(" ELSE EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.NORM_PAID_DAYS - EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.PREV_YEAR_LEAVE_ACCUM_DAYS");

                //strQry.AppendLine(" END ");

                //strQry.AppendLine(" FROM");

                //strQry.AppendLine("( ");
                ////4-Start

                //strQry.AppendLine(" SELECT ");
                //strQry.AppendLine(" LH.EMPLOYEE_NO");
                //strQry.AppendLine(",EMPLOYEE_DAY_COUNT_TABLE.NORM_PAID_PER_PERIOD ");
                //strQry.AppendLine(",EMPLOYEE_DAY_COUNT_TABLE.NORM_PAID_DAYS ");
                //strQry.AppendLine(",EMPLOYEE_DAY_COUNT_TABLE.DAY_COUNT");
                //strQry.AppendLine(",EMPLOYEE_DAY_COUNT_TABLE.PREV_YEAR_DAY_COUNT");

                //strQry.AppendLine(",SUM(");
                //strQry.AppendLine(" CASE ");

                //strQry.AppendLine(" WHEN LH.PAY_PERIOD_DATE >= '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");
                //strQry.AppendLine(" THEN ROUND(LH.LEAVE_ACCUM_DAYS,2)");

                //strQry.AppendLine(" END) AS LEAVE_ACCUM_DAYS");

                //strQry.AppendLine(",SUM(");

                //strQry.AppendLine(" CASE ");

                //strQry.AppendLine(" WHEN LH.PAY_PERIOD_DATE < '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");
                //strQry.AppendLine(" THEN ROUND(LH.LEAVE_ACCUM_DAYS,2)");

                //strQry.AppendLine(" END) AS PREV_YEAR_LEAVE_ACCUM_DAYS");

                //strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY LH");

                //strQry.AppendLine(" INNER JOIN ");

                //strQry.AppendLine("( ");
                ////3-Start

                //strQry.AppendLine(" SELECT ");
                //strQry.AppendLine(" FINAL_DAY_WORKED_MINUTES_TABLE.EMPLOYEE_NO");
                //strQry.AppendLine(",LS.NORM_PAID_DAYS");
                //strQry.AppendLine(",LS.NORM_PAID_PER_PERIOD");
                //strQry.AppendLine(",SUM (");

                //strQry.AppendLine(" CASE ");

                //strQry.AppendLine(" WHEN D.DAY_DATE >= '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");

                //strQry.AppendLine(" THEN 1");

                //strQry.AppendLine(" ELSE 0");

                //strQry.AppendLine(" END) AS DAY_COUNT");

                //strQry.AppendLine(",SUM (");

                //strQry.AppendLine(" CASE ");

                //strQry.AppendLine(" WHEN D.DAY_DATE >= '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");

                //strQry.AppendLine(" THEN 0");

                //strQry.AppendLine(" ELSE 1 ");

                //strQry.AppendLine(" END) AS PREV_YEAR_DAY_COUNT ");

                //strQry.AppendLine(" FROM ");

                //strQry.AppendLine("(");

                ////2-Start
                //strQry.AppendLine(" SELECT ");
                //strQry.AppendLine(" E.EMPLOYEE_NO");
                ////NB This Is the DEfault Pay Category for Employee and ALL Timesheets Across PayCategories are SUMMED
                //strQry.AppendLine(",EPCC.PAY_CATEGORY_NO");
                //strQry.AppendLine(",E.LEAVE_SHIFT_NO");
                //strQry.AppendLine(",DAY_WORKED_MINUTES_TABLE.TIMESHEET_DATE");

                //strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                //strQry.AppendLine(" INNER JOIN ");

                //strQry.AppendLine("( ");
                ////1-Start

                //strQry.AppendLine(" SELECT ");
                //strQry.AppendLine(" ETC.EMPLOYEE_NO ");
                //strQry.AppendLine(",ETC.TIMESHEET_DATE");
                //strQry.AppendLine(",SUM(ETC.TIMESHEET_TIME_OUT_MINUTES) - SUM(ETC.TIMESHEET_TIME_IN_MINUTES) AS DAY_WORKED_MINUTES");

                //strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC ");

                //strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                //strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO");
                //strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO");
                //strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                //strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                //strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                //strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                //strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parInt64CompanyNo);
                ////Errol - 2015-02-12
                ////strQry.AppendLine(" AND ETC.TIMESHEET_DATE > E.EMPLOYEE_LAST_RUNDATE");

                ////Errol - 2015-02-12
                //strQry.AppendLine(" AND ((ETC.TIMESHEET_DATE > E.EMPLOYEE_LAST_RUNDATE");
                //strQry.AppendLine(" AND E.FIRST_RUN_COMPLETED_IND = 'Y')");
                //strQry.AppendLine(" OR (ETC.TIMESHEET_DATE >= E.EMPLOYEE_LAST_RUNDATE");
                //strQry.AppendLine(" AND ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                //strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= '" + parPayPeriodDate.ToString("yyyy-MM-dd") + "'");
                //strQry.AppendLine(" GROUP BY ");
                //strQry.AppendLine(" ETC.EMPLOYEE_NO ");
                //strQry.AppendLine(",ETC.TIMESHEET_DATE");
                ////1-End
                //strQry.AppendLine(") AS DAY_WORKED_MINUTES_TABLE");

                //strQry.AppendLine(" ON E.EMPLOYEE_NO = DAY_WORKED_MINUTES_TABLE.EMPLOYEE_NO");

                //strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                //strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO");
                //strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                ////NB There is NO Join On PAY_CATEGORY_NO so That All Timesheets are Linked To Default PAY_CATEGORY_NO
                //strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                //strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                ////NB Default Employee Cost Centre
                //strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y'");

                //strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT_CURRENT LSC");
                //strQry.AppendLine(" ON E.COMPANY_NO = LSC.COMPANY_NO");
                //strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = LSC.PAY_CATEGORY_NO");
                //strQry.AppendLine(" AND LSC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                //strQry.AppendLine(" AND E.LEAVE_SHIFT_NO = LSC.LEAVE_SHIFT_NO");
                //strQry.AppendLine(" AND EPCC.RUN_NO = LSC.RUN_NO");
                ////Exclude Records That don't Exceed Minimum Time for a Leave Shift
                //strQry.AppendLine(" AND DAY_WORKED_MINUTES_TABLE.DAY_WORKED_MINUTES >= LSC.MIN_VALID_SHIFT_MINUTES");

                //strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                //strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                //strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");

                //strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                //strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                ////2-End
                //strQry.AppendLine(") AS FINAL_DAY_WORKED_MINUTES_TABLE");

                //strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D ");
                //strQry.AppendLine(" ON FINAL_DAY_WORKED_MINUTES_TABLE.TIMESHEET_DATE = D.DAY_DATE");

                //strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT_CURRENT LS");

                //strQry.AppendLine(" ON LS.COMPANY_NO = " + parInt64CompanyNo);
                //strQry.AppendLine(" AND FINAL_DAY_WORKED_MINUTES_TABLE.PAY_CATEGORY_NO = LS.PAY_CATEGORY_NO");
                ////Wages - WHAT ABOUT SALARIES
                //strQry.AppendLine(" AND LS.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                //strQry.AppendLine(" AND FINAL_DAY_WORKED_MINUTES_TABLE.LEAVE_SHIFT_NO = LS.LEAVE_SHIFT_NO");
                //strQry.AppendLine(" AND (((LS.LEAVE_PAID_ACCUMULATOR_IND = 1");
                //strQry.AppendLine(" AND D.DAY_NO IN (1,2,3,4,5))");
                ////Saturday Included
                //strQry.AppendLine(" OR (LS.LEAVE_PAID_ACCUMULATOR_IND = 2");
                //strQry.AppendLine(" AND D.DAY_NO IN (1,2,3,4,5,6)))");
                ////Sunday Included
                //strQry.AppendLine(" OR (LS.LEAVE_PAID_ACCUMULATOR_IND = 3");
                //strQry.AppendLine(" AND D.DAY_NO IN (0,1,2,3,4,5,6)))");

                ////Not Public Holiday
                //strQry.AppendLine(" LEFT JOIN ");

                //strQry.AppendLine("(SELECT ");
                //strQry.AppendLine(" PCPC.PAY_CATEGORY_NO");
                //strQry.AppendLine(",PHC.PUBLIC_HOLIDAY_DATE");

                //strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_CURRENT PHC ");

                //strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC ");
                //strQry.AppendLine(" ON PHC.COMPANY_NO = PCPC.COMPANY_NO ");
                //strQry.AppendLine(" AND PCPC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                //strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                //strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P'");
                //strQry.AppendLine(" AND PCPC.PAY_PUBLIC_HOLIDAY_IND = 'Y'");

                //strQry.AppendLine(" WHERE PHC.COMPANY_NO = " + parInt64CompanyNo + ") AS TEMP_TABLE2");

                //strQry.AppendLine(" ON LS.PAY_CATEGORY_NO = TEMP_TABLE2.PAY_CATEGORY_NO");
                //strQry.AppendLine(" AND  D.DAY_DATE = TEMP_TABLE2.PUBLIC_HOLIDAY_DATE");

                ////Not Public Holiday
                //strQry.AppendLine(" WHERE TEMP_TABLE2.PAY_CATEGORY_NO IS NULL");

                //strQry.AppendLine(" GROUP BY ");
                //strQry.AppendLine(" FINAL_DAY_WORKED_MINUTES_TABLE.EMPLOYEE_NO");
                //strQry.AppendLine(",LS.NORM_PAID_DAYS");
                //strQry.AppendLine(",LS.NORM_PAID_PER_PERIOD");
                ////3-End

                //strQry.AppendLine(" ) AS EMPLOYEE_DAY_COUNT_TABLE ");

                //strQry.AppendLine(" ON LH.EMPLOYEE_NO = EMPLOYEE_DAY_COUNT_TABLE.EMPLOYEE_NO  ");

                //strQry.AppendLine(" WHERE LH.COMPANY_NO = " + parInt64CompanyNo);
                //strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

                //strQry.AppendLine(" AND LH.PAY_PERIOD_DATE >= '" + dtStartTaxYear.AddYears(-1).ToString("yyyy-MM-dd") + "'");

                ////Accumulated Leave / Take-On Balance
                //strQry.AppendLine(" AND LH.PROCESS_NO IN(98,99)");
                ////Normal Leave
                //strQry.AppendLine(" AND LH.EARNING_NO = 200");

                //strQry.AppendLine(" GROUP BY ");

                //strQry.AppendLine(" LH.EMPLOYEE_NO ");
                //strQry.AppendLine(",EMPLOYEE_DAY_COUNT_TABLE.NORM_PAID_PER_PERIOD ");
                //strQry.AppendLine(",EMPLOYEE_DAY_COUNT_TABLE.NORM_PAID_DAYS ");
                //strQry.AppendLine(",EMPLOYEE_DAY_COUNT_TABLE.DAY_COUNT ");
                //strQry.AppendLine(",EMPLOYEE_DAY_COUNT_TABLE.PREV_YEAR_DAY_COUNT");

                ////4-End
                //strQry.AppendLine(") AS EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE ");

                ////5-End
                //strQry.AppendLine(") AS FINAL_EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE ");

                //strQry.AppendLine(" ON LH.EMPLOYEE_NO = FINAL_EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.EMPLOYEE_NO  ");

                //strQry.AppendLine(" WHERE LH.COMPANY_NO = " + parInt64CompanyNo);
                //strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                //strQry.AppendLine(" AND LH.EARNING_NO = 200");

                //strQry.AppendLine(" GROUP BY ");
                //strQry.AppendLine(" LH.EMPLOYEE_NO");
                //strQry.AppendLine(",LH.EARNING_NO");
                //strQry.AppendLine(",FINAL_EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.LEAVE_ACCUM_DAYS");
                //strQry.AppendLine(",FINAL_EMPLOYEE_CURRENT_RUN_ACCUM_LEAVE_TABLE.PREV_YEAR_LEAVE_ACCUM_DAYS");
                //strQry.AppendLine(",CURRENT_LEAVE_TABLE.LEAVE_DAYS_DECIMAL");

                //strQry.AppendLine(",CURRENT_LEAVE_TABLE.MAX_LEAVE_OPTION) AS LEAVE_CLOSE_TABLE");

                //strQry.AppendLine(" ON EPCC.EMPLOYEE_NO = LEAVE_CLOSE_TABLE.EMPLOYEE_NO");

                //strQry.AppendLine(" WHERE EPCC.COMPANY_NO = " + parInt64CompanyNo);
                //strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                //strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                ////NB - Use Default
                //strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y'");
                //strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P') AS FINAL_LEAVE_CLOSE_TABLE");

                //strQry.AppendLine(" WHERE EMPLOYEE_EARNING_CURRENT.COMPANY_NO = FINAL_LEAVE_CLOSE_TABLE.COMPANY_NO");
                //strQry.AppendLine(" AND EMPLOYEE_EARNING_CURRENT.EMPLOYEE_NO = FINAL_LEAVE_CLOSE_TABLE.EMPLOYEE_NO");
                //strQry.AppendLine(" AND EMPLOYEE_EARNING_CURRENT.EARNING_NO = FINAL_LEAVE_CLOSE_TABLE.EARNING_NO");
                //strQry.AppendLine(" AND EMPLOYEE_EARNING_CURRENT.PAY_CATEGORY_NO = FINAL_LEAVE_CLOSE_TABLE.PAY_CATEGORY_NO");
                //strQry.AppendLine(" AND EMPLOYEE_EARNING_CURRENT.PAY_CATEGORY_TYPE = FINAL_LEAVE_CLOSE_TABLE.PAY_CATEGORY_TYPE");
                //strQry.AppendLine(" AND EMPLOYEE_EARNING_CURRENT.RUN_TYPE = FINAL_LEAVE_CLOSE_TABLE.RUN_TYPE");

                //clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //ELR 2014-05-02
                strQry.Clear();

                strQry.AppendLine(" UPDATE EIC ");

                strQry.AppendLine(" SET ");

                strQry.AppendLine(" EIC.CURRENT_YEAR_LEAVE_SHIFTS_PER_RUN = ISNULL(EMPLOYEE_DAY_COUNT_TABLE.CURRENT_YEAR_DAY_COUNT,0)");
                strQry.AppendLine(",EIC.PREV_YEAR_LEAVE_SHIFTS_PER_RUN = ISNULL(EMPLOYEE_DAY_COUNT_TABLE.PREV_YEAR_DAY_COUNT,0) ");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC ");

                strQry.AppendLine(" LEFT JOIN ");

                strQry.AppendLine("(SELECT ");
                strQry.AppendLine(" FINAL_DAY_WORKED_MINUTES_TABLE.EMPLOYEE_NO");

                strQry.AppendLine(",SUM (");

                strQry.AppendLine(" CASE ");

                //strQry.AppendLine(" WHEN D.DAY_DATE >= '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" WHEN D.DAY_DATE >= '" + dtStartLeaveTaxYear.ToString("yyyy-MM-dd") + "'");

                strQry.AppendLine(" THEN 1");

                strQry.AppendLine(" ELSE 0");

                strQry.AppendLine(" END) AS CURRENT_YEAR_DAY_COUNT");

                strQry.AppendLine(",SUM (");

                strQry.AppendLine(" CASE ");

                strQry.AppendLine(" WHEN D.DAY_DATE < '" + dtStartLeaveTaxYear.ToString("yyyy-MM-dd") + "'");

                strQry.AppendLine(" THEN 1");

                strQry.AppendLine(" ELSE 0");

                strQry.AppendLine(" END) AS PREV_YEAR_DAY_COUNT");

                strQry.AppendLine(" FROM ");

                strQry.AppendLine("(");

                //2-Start
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" E.EMPLOYEE_NO");
                //NB This Is the DEfault Pay Category for Employee and ALL Timesheets Across PayCategories are SUMMED
                strQry.AppendLine(",EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",E.LEAVE_SHIFT_NO");
                strQry.AppendLine(",DAY_WORKED_MINUTES_TABLE.TIMESHEET_DATE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("( ");
                //1-Start

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" ETC.EMPLOYEE_NO ");
                strQry.AppendLine(",ETC.TIMESHEET_DATE");
                strQry.AppendLine(",SUM(ETC.TIMESHEET_TIME_OUT_MINUTES) - SUM(ETC.TIMESHEET_TIME_IN_MINUTES) AS DAY_WORKED_MINUTES");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO");
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parInt64CompanyNo);
                //Errol - 2015-02-12
                //strQry.AppendLine(" AND ETC.TIMESHEET_DATE > E.EMPLOYEE_LAST_RUNDATE");

                //Errol - 2015-02-12
                strQry.AppendLine(" AND ((ETC.TIMESHEET_DATE > E.EMPLOYEE_LAST_RUNDATE");
                strQry.AppendLine(" AND E.FIRST_RUN_COMPLETED_IND = 'Y')");
                strQry.AppendLine(" OR (ETC.TIMESHEET_DATE >= E.EMPLOYEE_LAST_RUNDATE");
                strQry.AppendLine(" AND ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= '" + parPayPeriodDate.ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" ETC.EMPLOYEE_NO ");
                strQry.AppendLine(",ETC.TIMESHEET_DATE");
                //1-End
                strQry.AppendLine(") AS DAY_WORKED_MINUTES_TABLE");

                strQry.AppendLine(" ON E.EMPLOYEE_NO = DAY_WORKED_MINUTES_TABLE.EMPLOYEE_NO");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                //NB There is NO Join On PAY_CATEGORY_NO so That All Timesheets are Linked To Default PAY_CATEGORY_NO
                //ELR 2014-05-20
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                //NB Default Employee Cost Centre
                strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y'");
                //2017-07-19
                strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P' ");
                
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT_CURRENT LSC");
                strQry.AppendLine(" ON E.COMPANY_NO = LSC.COMPANY_NO");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = LSC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND LSC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND E.LEAVE_SHIFT_NO = LSC.LEAVE_SHIFT_NO");
                strQry.AppendLine(" AND EPCC.RUN_NO = LSC.RUN_NO");
                //Exclude Records That don't Exceed Minimum Time for a Leave Shift
                strQry.AppendLine(" AND DAY_WORKED_MINUTES_TABLE.DAY_WORKED_MINUTES >= LSC.MIN_VALID_SHIFT_MINUTES");

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");

                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                //2-End
                strQry.AppendLine(") AS FINAL_DAY_WORKED_MINUTES_TABLE");

                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D ");
                strQry.AppendLine(" ON FINAL_DAY_WORKED_MINUTES_TABLE.TIMESHEET_DATE = D.DAY_DATE");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT_CURRENT LS");

                strQry.AppendLine(" ON LS.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND FINAL_DAY_WORKED_MINUTES_TABLE.PAY_CATEGORY_NO = LS.PAY_CATEGORY_NO");
                //Wages - WHAT ABOUT SALARIES
                strQry.AppendLine(" AND LS.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND FINAL_DAY_WORKED_MINUTES_TABLE.LEAVE_SHIFT_NO = LS.LEAVE_SHIFT_NO");
                strQry.AppendLine(" AND (((LS.LEAVE_PAID_ACCUMULATOR_IND = 1");
                strQry.AppendLine(" AND D.DAY_NO IN (1,2,3,4,5))");
                //Saturday Included
                strQry.AppendLine(" OR (LS.LEAVE_PAID_ACCUMULATOR_IND = 2");
                strQry.AppendLine(" AND D.DAY_NO IN (1,2,3,4,5,6)))");
                //Sunday Included
                strQry.AppendLine(" OR (LS.LEAVE_PAID_ACCUMULATOR_IND = 3");
                strQry.AppendLine(" AND D.DAY_NO IN (0,1,2,3,4,5,6)))");

                //Not Public Holiday
                strQry.AppendLine(" LEFT JOIN ");

                strQry.AppendLine("(SELECT ");
                strQry.AppendLine(" PCPC.PAY_CATEGORY_NO");
                strQry.AppendLine(",PHC.PUBLIC_HOLIDAY_DATE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_CURRENT PHC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC ");
                strQry.AppendLine(" ON PHC.COMPANY_NO = PCPC.COMPANY_NO ");
                //ELR 2014-05-20
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P'");
                strQry.AppendLine(" AND PCPC.PAY_PUBLIC_HOLIDAY_IND = 'Y'");

                strQry.AppendLine(" WHERE PHC.COMPANY_NO = " + parInt64CompanyNo + ") AS TEMP_TABLE2");

                strQry.AppendLine(" ON LS.PAY_CATEGORY_NO = TEMP_TABLE2.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND  D.DAY_DATE = TEMP_TABLE2.PUBLIC_HOLIDAY_DATE");

                //Not Public Holiday
                strQry.AppendLine(" WHERE TEMP_TABLE2.PAY_CATEGORY_NO IS NULL");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" FINAL_DAY_WORKED_MINUTES_TABLE.EMPLOYEE_NO");
                strQry.AppendLine(",LS.NORM_PAID_DAYS");
                strQry.AppendLine(",LS.NORM_PAID_PER_PERIOD");
                //3-End

                strQry.AppendLine(" ) AS EMPLOYEE_DAY_COUNT_TABLE ");

                strQry.AppendLine(" ON EIC.EMPLOYEE_NO = EMPLOYEE_DAY_COUNT_TABLE.EMPLOYEE_NO ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                strQry.AppendLine(" ON EIC.COMPANY_NO = EPCC.COMPANY_NO");
                strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                //NB There is NO Join On PAY_CATEGORY_NO so That All Timesheets are Linked To Default PAY_CATEGORY_NO
                //ELR 2014-05-20
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE ");
                //NB Default Employee Cost Centre
                strQry.AppendLine(" AND EIC.RUN_NO = EPCC.RUN_NO");
                strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y'");

                strQry.AppendLine(" WHERE EIC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");
                strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ");
                strQry.AppendLine(" DISABLE TRIGGER tgr_EMPLOYEE_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //Reset All 
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" INCLUDED_IN_RUN_IND = NULL");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" UPDATE ETC");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" INCLUDED_IN_RUN_IND = 'Y'");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO");
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                strQry.AppendLine(" AND ((ETC.TIMESHEET_DATE > E.EMPLOYEE_LAST_RUNDATE");
                strQry.AppendLine(" AND E.FIRST_RUN_COMPLETED_IND = 'Y')");
                strQry.AppendLine(" OR (ETC.TIMESHEET_DATE >= E.EMPLOYEE_LAST_RUNDATE");
                strQry.AppendLine(" AND ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= '" + parPayPeriodDate.ToString("yyyy-MM-dd") + "'");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ");
                strQry.AppendLine(" ENABLE TRIGGER tgr_EMPLOYEE_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT ");
                strQry.AppendLine(" DISABLE TRIGGER tgr_EMPLOYEE_BREAK_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //Reset All 
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" INCLUDED_IN_RUN_IND = NULL");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" UPDATE EBC");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" INCLUDED_IN_RUN_IND = 'Y'");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT EBC");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON EBC.COMPANY_NO = E.COMPANY_NO");
                strQry.AppendLine(" AND EBC.EMPLOYEE_NO = E.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" WHERE EBC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                strQry.AppendLine(" AND ((EBC.BREAK_DATE > E.EMPLOYEE_LAST_RUNDATE");
                strQry.AppendLine(" AND E.FIRST_RUN_COMPLETED_IND = 'Y')");
                strQry.AppendLine(" OR (EBC.BREAK_DATE >= E.EMPLOYEE_LAST_RUNDATE");
                strQry.AppendLine(" AND ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                strQry.AppendLine(" AND EBC.BREAK_DATE <= '" + parPayPeriodDate.ToString("yyyy-MM-dd") + "'");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT ");
                strQry.AppendLine(" ENABLE TRIGGER tgr_EMPLOYEE_BREAK_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.COMPANY");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" WAGE_RUN_IND = 'Y'");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //2017-07-11
                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.PAYROLL_RUN_QUEUE_COMPLETED");

                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",USER_NO");
                strQry.AppendLine(",PAY_PERIOD_DATE");
                strQry.AppendLine(",PAY_CATEGORY_NUMBERS");
                strQry.AppendLine(",PAYROLL_RUN_QUEUE_IND");
                strQry.AppendLine(",START_RUN_DATE");
                strQry.AppendLine(",END_RUN_DATE)");

                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",USER_NO");
                strQry.AppendLine(",PAY_PERIOD_DATE");
                strQry.AppendLine(",PAY_CATEGORY_NUMBERS");
                strQry.AppendLine(",'C'");
                strQry.AppendLine(",START_RUN_DATE");
                strQry.AppendLine(",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" FROM InteractPayroll.dbo.PAYROLL_RUN_QUEUE");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                //2017-07-11
                strQry.Clear();

                strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.PAYROLL_RUN_QUEUE");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            Calculate_Payroll_From_TimeSheets_Continue:

                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
                strQry.AppendLine(" SET BACKUP_DB_IND = 1");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            }
            catch (Exception ex)
            {
                Write_Log(ex, strClassNameFunctionAndParameters, strQry.ToString(), true);

                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.PAYROLL_RUN_QUEUE");

                strQry.AppendLine(" SET ");
                strQry.AppendLine(" PAYROLL_RUN_QUEUE_IND = 'F'");
                strQry.AppendLine(",END_RUN_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                throw;
            }
            
            return strReturnPayCategoryNosInError;
        }

        public string Calculate_TimeAttendance_From_TimeSheets(Int64 parint64CurrentUserNo, Int64 parInt64CompanyNo, string parstrArrayPayCategoryNo, string parstrPayrollType, DateTime parPayPeriodDate)
        {
            string strClassNameFunctionAndParameters = pvtstrClassName + " Calculate_TimeAttendance_From_TimeSheets CompanyNo=" + parInt64CompanyNo + ",ArrayPayCategoryNo=" + parstrArrayPayCategoryNo + ",PayrollType=" + parstrPayrollType + ",PayPeriodDate=" + parPayPeriodDate.ToString("yyyy-MM-dd");
            
            string strReturnPayCategoryNosInError = "";
            StringBuilder strQry = new StringBuilder();

            try
            {
                Write_Log("Entered " + strClassNameFunctionAndParameters);
                
                string[] parstrPayCategoryNo = parstrArrayPayCategoryNo.Split('|');
              
                DataSet DataSet = new System.Data.DataSet();

                clsISUtilities clsISUtilities = new clsISUtilities();

                DataSet TempDataSet = new DataSet();
                DataView DataViewCompanyPaidPublicHoliday;
                DataView DataViewEmployeeParameters;
                DataView DataViewTimeSheetDayTotal;
                DataView DataViewPayCategoryWeek;
                DataView DataViewTimeSheetErrors;

                DateTime dtTimeSheet = DateTime.Now;

                string strEmployeeExceptionIndicator = "";
                string strEmployeeNormalIndicator = "";
                string strEmployeeBlankIndicator = "";
                string strEmployeeWeekExceptionIndicator = "";
                string strEmployeeWeekNormalIndicator = "";
                string strEmployeeWeekBlankIndicator = "";

                bool blnWeekDays = true;

                bool blnEmployeeTimeSheetsForPayCategory = false;

                int intNormalHoursBoundary = 0;
                int intOverTime1HoursBoundary = 0;
                int intOverTime2HoursBoundary = 0;
                int intOverTime3HoursBoundary = 0;

                int intDayNormalHours = 0;
                int intDayOverTime1Hours = 0;
                int intDayOverTime2Hours = 0;
                int intDayOverTime3Hours = 0;

                int intTotalMinutesPaidForWeek = 0;

                int intWeekNormalMinutes = 0;
                int intWeekOverTime1Minutes = 0;
                int intWeekOverTime2Minutes = 0;
                int intWeekOverTime3Minutes = 0;
                int intWeekPaidHolidayWorkedMinutes = 0;

                int intPeriodNormalTimeMinutes = 0;
                int intPeriodOverTime1Minutes = 0;
                int intPeriodOverTime2Minutes = 0;
                int intPeriodOverTime3Minutes = 0;
                int intPeriodPaidHolidayWorkedMinutes = 0;

                int intNormalTimeMinutesRounded = 0;
                int intOverTime1MinutesRounded = 0;
                int intOverTime2MinutesRounded = 0;
                int intOverTime3MinutesRounded = 0;
                int intPaidHolidayWorkedMinutesRounded = 0;
                int intOverTimeCF = 0;

                double dblNormalHoursDecimal = 0;
                double dblOverTime1HoursDecimal = 0;
                double dblOverTime2HoursDecimal = 0;
                double dblOverTime3HoursDecimal = 0;
                double dblPaidHolidayWorkedHoursDecimal = 0;

                DateTime dtEmployeeLastRundate;

                double dblDaysInYear = 0;

                object[] objKey = new object[2];

                DateTime dtEndTaxYear;
                DateTime dtStartTaxYear;

                if (parPayPeriodDate.Month > 2)
                {
                    dtStartTaxYear = new DateTime(parPayPeriodDate.Year, 3, 1);
                }
                else
                {
                    dtStartTaxYear = new DateTime(parPayPeriodDate.Year - 1, 3, 1);
                }

                dtEndTaxYear = dtStartTaxYear.AddYears(1).AddDays(-1);

                TimeSpan tsDaysInYear = dtEndTaxYear.Subtract(dtStartTaxYear);
                dblDaysInYear = tsDaysInYear.Days + 1;
                string strPayCategoryIn = "";

                //Create SQL For Months of EMPLOYEE_TAX_SPREADHEET
                string strSelectAddonQry = "";
                DateTime dtTempDateTime = new DateTime(2007, 3, 1);

                for (int intRowMonth = dtTempDateTime.Month; intRowMonth < 20; intRowMonth++)
                {
                    if (intRowMonth == parPayPeriodDate.Month)
                    {
                        strSelectAddonQry += ",ISNULL(ETSH." + dtTempDateTime.ToString("MMM").ToUpper() + "_TOTAL,0) + XX ";
                    }
                    else
                    {
                        strSelectAddonQry += ",ISNULL(ETSH." + dtTempDateTime.ToString("MMM").ToUpper() + "_TOTAL,0)";
                    }

                    if (dtTempDateTime.Month == 2)
                    {
                        break;
                    }

                    dtTempDateTime = dtTempDateTime.AddMonths(1);
                }

                for (int intCount = 0; intCount < parstrPayCategoryNo.Length; intCount++)
                {
                    if (intCount == 0)
                    {
                        strPayCategoryIn = parstrPayCategoryNo[intCount].ToString();
                    }
                    else
                    {
                        strPayCategoryIn += "," + parstrPayCategoryNo[intCount].ToString();
                    }
                }

                DataTable DataTable = new DataTable("DaysInWeek");
                DataTable.Columns.Add("DAY_DATE", typeof(System.DateTime));

                DataSet.Tables.Add(DataTable);

                Get_TimeSheet_Totals(DataSet, parInt64CompanyNo, parPayPeriodDate, strPayCategoryIn, parstrPayrollType);

                //Check For TimeSheet Errors
                for (int intCount = 0; intCount < parstrPayCategoryNo.Length; intCount++)
                {
                    DataViewTimeSheetErrors = null;
                    DataViewTimeSheetErrors = new DataView(DataSet.Tables["TimeSheetDayTotals"],
                        "PAY_CATEGORY_NO = " + parstrPayCategoryNo[intCount].ToString() + " AND INDICATOR = 'X'",
                        "",
                        DataViewRowState.CurrentRows);

                    if (DataViewTimeSheetErrors.Count > 0)
                    {
                        if (strReturnPayCategoryNosInError == "")
                        {
                            strReturnPayCategoryNosInError = parstrPayCategoryNo[intCount].ToString();
                        }
                        else
                        {
                            strReturnPayCategoryNosInError += "|" + parstrPayCategoryNo[intCount].ToString();
                        }
                    }
                }

                if (strReturnPayCategoryNosInError != "")
                {
                    //2017-07-11
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE InteractPayroll.dbo.PAYROLL_RUN_QUEUE");
                    //Errors in TimeSheets
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" PAYROLL_RUN_QUEUE_IND = 'E'");
                    strQry.AppendLine(",START_RUN_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                    
                    //Errors in TimeSheet
                    goto Calculate_TimeAttendance_From_TimeSheets_Continue;
                }
                
                //2017-07-31
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT");

                strQry.AppendLine(" SET PAY_CATEGORY_USED_IND = 'Y'");
                
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                strQry.AppendLine(" AND RUN_TYPE = 'P'");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                
                strQry.Clear();
                strQry.AppendLine(" SELECT DISTINCT ");
                strQry.AppendLine(" E.EMPLOYEE_NO");

                //Errol 2013-06-15
                strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

                strQry.AppendLine(" CASE ");

                //Errol - 2015-02-17
                strQry.AppendLine(" WHEN E.FIRST_RUN_COMPLETED_IND = 'Y'");

                strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE  ");

                strQry.AppendLine(" ELSE DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                strQry.AppendLine(" END ");

                strQry.AppendLine(",E.TAX_TYPE_IND");
                strQry.AppendLine(",E.TAX_DIRECTIVE_PERCENTAGE");
                strQry.AppendLine(",E.EMPLOYEE_TAX_STARTDATE");
                strQry.AppendLine(",E.EMPLOYEE_BIRTHDATE");
                strQry.AppendLine(",E.EMPLOYEE_NUMBER_CHEQUES");
                strQry.AppendLine(",E.NUMBER_MEDICAL_AID_DEPENDENTS");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC ");
                strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                //2017-07-19
                strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P'");
                
                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" E.EMPLOYEE_NO");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EPCC.EMPLOYEE_NO");
                strQry.AppendLine(",EPCC.HOURLY_RATE");
                strQry.AppendLine(",EPCC.DEFAULT_IND");
                strQry.AppendLine(",EPCC.OVERTIME_VALUE_BF");
                strQry.AppendLine(",EPCC.RUN_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC ");

                strQry.AppendLine(" WHERE EPCC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                //2017-07-19
                strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P'");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EPCC.EMPLOYEE_NO");
                strQry.AppendLine(",EPCC.HOURLY_RATE");
                strQry.AppendLine(",EPCC.DEFAULT_IND");
                strQry.AppendLine(",EPCC.OVERTIME_VALUE_BF");
                strQry.AppendLine(",EPCC.RUN_NO");

                strQry.AppendLine(" ORDER BY ");
                //NB Order by DEFAULT_IND in This Position Very Important (Causes Default Pay Category to Run Last for Tax etc)
                strQry.AppendLine(" EPCC.DEFAULT_IND");
                strQry.AppendLine(",EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EPCC.EMPLOYEE_NO");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeParameters", parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EMPLOYEE_NO");
                strQry.AppendLine(",TOTAL");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                strQry.AppendLine(" AND EARNING_NO = 8 ");
                strQry.AppendLine(" AND TOTAL > 0");

                //2017-07-19
                strQry.AppendLine(" AND RUN_TYPE = 'P'");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyPaidPublicHoliday", parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" T.PAY_CATEGORY_NO");
                strQry.AppendLine(",T.WEEK_DATE");
                strQry.AppendLine(",T.WEEK_DATE_FROM");
                strQry.AppendLine(",T.MON_TIME_MINUTES");
                strQry.AppendLine(",T.TUE_TIME_MINUTES");
                strQry.AppendLine(",T.WED_TIME_MINUTES");
                strQry.AppendLine(",T.THU_TIME_MINUTES");
                strQry.AppendLine(",T.FRI_TIME_MINUTES");
                strQry.AppendLine(",T.SAT_TIME_MINUTES");
                strQry.AppendLine(",T.SUN_TIME_MINUTES");
                strQry.AppendLine(",T.OVERTIME1_RATE");
                strQry.AppendLine(",T.OVERTIME2_RATE");
                strQry.AppendLine(",T.OVERTIME3_RATE");
                strQry.AppendLine(",T.PAIDHOLIDAY_RATE");
                strQry.AppendLine(",T.TOTAL_DAILY_TIME_OVERTIME");

                strQry.AppendLine(",T.DAILY_ROUNDING_IND");
                strQry.AppendLine(",T.DAILY_ROUNDING_MINUTES");
                strQry.AppendLine(",T.PAY_PERIOD_ROUNDING_IND");
                strQry.AppendLine(",T.PAY_PERIOD_ROUNDING_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_SUN_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_SUN_BELOW_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_MON_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_MON_BELOW_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_TUE_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_TUE_BELOW_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_WED_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_WED_BELOW_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_THU_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_THU_BELOW_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_FRI_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_FRI_BELOW_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_SAT_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_SAT_BELOW_MINUTES");
                strQry.AppendLine(",T.OVERTIME_IND");
                strQry.AppendLine(",T.SATURDAY_PAY_RATE");
                strQry.AppendLine(",T.SATURDAY_PAY_RATE_IND");
                strQry.AppendLine(",T.SUNDAY_PAY_RATE");
                strQry.AppendLine(",T.SUNDAY_PAY_RATE_IND");
                strQry.AppendLine(",T.OVERTIME1_MINUTES");
                strQry.AppendLine(",T.OVERTIME2_MINUTES");
                strQry.AppendLine(",T.OVERTIME3_MINUTES");

                strQry.AppendLine(",SUM(T.WEEK_OVERTIME_BOUNDARY_MINUTES) AS WEEK_OVERTIME_BOUNDARY_MINUTES");

                strQry.AppendLine(" FROM ");

                strQry.AppendLine(" (SELECT ");
                strQry.AppendLine(" PCWC.PAY_CATEGORY_NO");
                strQry.AppendLine(",PCWC.WEEK_DATE");
                strQry.AppendLine(",PCWC.WEEK_DATE_FROM");
                strQry.AppendLine(",PCWC.MON_TIME_MINUTES");
                strQry.AppendLine(",PCWC.TUE_TIME_MINUTES");
                strQry.AppendLine(",PCWC.WED_TIME_MINUTES");
                strQry.AppendLine(",PCWC.THU_TIME_MINUTES");
                strQry.AppendLine(",PCWC.FRI_TIME_MINUTES");
                strQry.AppendLine(",PCWC.SAT_TIME_MINUTES");
                strQry.AppendLine(",PCWC.SUN_TIME_MINUTES");
                strQry.AppendLine(",C.OVERTIME1_RATE");
                strQry.AppendLine(",C.OVERTIME2_RATE");
                strQry.AppendLine(",C.OVERTIME3_RATE");
                strQry.AppendLine(",PCPC.PAIDHOLIDAY_RATE");
                strQry.AppendLine(",PCPC.TOTAL_DAILY_TIME_OVERTIME");

                strQry.AppendLine(",PCPC.DAILY_ROUNDING_IND");
                strQry.AppendLine(",PCPC.DAILY_ROUNDING_MINUTES");
                strQry.AppendLine(",PCPC.PAY_PERIOD_ROUNDING_IND");
                strQry.AppendLine(",PCPC.PAY_PERIOD_ROUNDING_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_SUN_ABOVE_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_SUN_BELOW_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_MON_ABOVE_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_MON_BELOW_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_TUE_ABOVE_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_TUE_BELOW_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_WED_ABOVE_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_WED_BELOW_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_THU_ABOVE_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_THU_BELOW_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_FRI_ABOVE_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_FRI_BELOW_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_SAT_ABOVE_MINUTES");
                strQry.AppendLine(",PCWC.EXCEPTION_SAT_BELOW_MINUTES");
                strQry.AppendLine(",PCPC.OVERTIME_IND");
                strQry.AppendLine(",PCPC.SATURDAY_PAY_RATE");
                strQry.AppendLine(",PCPC.SATURDAY_PAY_RATE_IND");
                strQry.AppendLine(",PCPC.SUNDAY_PAY_RATE");
                strQry.AppendLine(",PCPC.SUNDAY_PAY_RATE_IND");
                strQry.AppendLine(",PCWC.OVERTIME1_MINUTES");
                strQry.AppendLine(",PCWC.OVERTIME2_MINUTES");
                strQry.AppendLine(",PCWC.OVERTIME3_MINUTES");

                strQry.AppendLine(",WEEK_OVERTIME_BOUNDARY_MINUTES = ");
                strQry.AppendLine(" CASE ");
                //Overtime Paid After Week Hours has been worked (A = Accumulate)
                strQry.AppendLine(" WHEN OVERTIME_IND = 'A' ");

                strQry.AppendLine(" THEN CASE ");
                strQry.AppendLine(" WHEN D.DAY_NO = 0");
                strQry.AppendLine(" THEN PCWC.SUN_TIME_MINUTES");

                strQry.AppendLine(" WHEN D.DAY_NO = 1");
                strQry.AppendLine(" THEN PCWC.MON_TIME_MINUTES");

                strQry.AppendLine(" WHEN D.DAY_NO = 2");
                strQry.AppendLine(" THEN PCWC.TUE_TIME_MINUTES");

                strQry.AppendLine(" WHEN D.DAY_NO = 3");
                strQry.AppendLine(" THEN PCWC.WED_TIME_MINUTES");

                strQry.AppendLine(" WHEN D.DAY_NO = 4");
                strQry.AppendLine(" THEN PCWC.THU_TIME_MINUTES");

                strQry.AppendLine(" WHEN D.DAY_NO = 5");
                strQry.AppendLine(" THEN PCWC.FRI_TIME_MINUTES");

                strQry.AppendLine(" WHEN D.DAY_NO = 6");
                strQry.AppendLine(" THEN PCWC.SAT_TIME_MINUTES");
                strQry.AppendLine(" END ");

                //Overtime Paid After Day Hours has been Exceeded (Default Setting)
                strQry.AppendLine(" ELSE 0");
                strQry.AppendLine(" END ");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_CURRENT PCWC");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");
                strQry.AppendLine(" ON PCWC.COMPANY_NO = PCPC.COMPANY_NO ");
                strQry.AppendLine(" AND PCWC.PAY_CATEGORY_NO = PCPC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCWC.PAY_CATEGORY_TYPE = PCPC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P' ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C");
                strQry.AppendLine(" ON PCWC.COMPANY_NO = C.COMPANY_NO ");
                strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");
                strQry.AppendLine(" ON D.DAY_DATE >= PCWC.WEEK_DATE_FROM");
                strQry.AppendLine(" AND D.DAY_DATE <= PCWC.WEEK_DATE");

                strQry.AppendLine(" WHERE PCWC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PCWC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND PCWC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")) AS T");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" T.PAY_CATEGORY_NO");
                strQry.AppendLine(",T.WEEK_DATE");
                strQry.AppendLine(",T.WEEK_DATE_FROM");
                strQry.AppendLine(",T.MON_TIME_MINUTES");
                strQry.AppendLine(",T.TUE_TIME_MINUTES");
                strQry.AppendLine(",T.WED_TIME_MINUTES");
                strQry.AppendLine(",T.THU_TIME_MINUTES");
                strQry.AppendLine(",T.FRI_TIME_MINUTES");
                strQry.AppendLine(",T.SAT_TIME_MINUTES");
                strQry.AppendLine(",T.SUN_TIME_MINUTES");
                strQry.AppendLine(",T.OVERTIME1_RATE");
                strQry.AppendLine(",T.OVERTIME2_RATE");
                strQry.AppendLine(",T.OVERTIME3_RATE");
                strQry.AppendLine(",T.PAIDHOLIDAY_RATE");
                strQry.AppendLine(",T.TOTAL_DAILY_TIME_OVERTIME");

                strQry.AppendLine(",T.DAILY_ROUNDING_IND");
                strQry.AppendLine(",T.DAILY_ROUNDING_MINUTES");
                strQry.AppendLine(",T.PAY_PERIOD_ROUNDING_IND");
                strQry.AppendLine(",T.PAY_PERIOD_ROUNDING_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_SUN_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_SUN_BELOW_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_MON_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_MON_BELOW_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_TUE_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_TUE_BELOW_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_WED_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_WED_BELOW_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_THU_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_THU_BELOW_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_FRI_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_FRI_BELOW_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_SAT_ABOVE_MINUTES");
                strQry.AppendLine(",T.EXCEPTION_SAT_BELOW_MINUTES");
                strQry.AppendLine(",T.OVERTIME_IND");
                strQry.AppendLine(",T.SATURDAY_PAY_RATE");
                strQry.AppendLine(",T.SATURDAY_PAY_RATE_IND");
                strQry.AppendLine(",T.SUNDAY_PAY_RATE");
                strQry.AppendLine(",T.SUNDAY_PAY_RATE_IND");
                strQry.AppendLine(",T.OVERTIME1_MINUTES");
                strQry.AppendLine(",T.OVERTIME2_MINUTES");
                strQry.AppendLine(",T.OVERTIME3_MINUTES");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" T.PAY_CATEGORY_NO");
                strQry.AppendLine(",T.WEEK_DATE");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategoryWeek", parInt64CompanyNo);

                //2017-07-11
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.PAYROLL_RUN_QUEUE");

                strQry.AppendLine(" SET ");
                strQry.AppendLine(" PAYROLL_RUN_QUEUE_IND = 'S'");
                strQry.AppendLine(",START_RUN_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                
                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                DataViewCompanyPaidPublicHoliday = null;
                DataViewCompanyPaidPublicHoliday = new DataView(DataSet.Tables["CompanyPaidPublicHoliday"],
                    "",
                    "EMPLOYEE_NO",
                    DataViewRowState.CurrentRows);

                for (int intEmployeeRow = 0; intEmployeeRow < DataSet.Tables["Employee"].Rows.Count; intEmployeeRow++)
                {
#if (DEBUG)
                    int intEmployeeNo = Convert.ToInt32(DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"]);

                    if (intEmployeeNo == 195)
                    {
                        string strStop = "";
                    }
#endif
                    DataViewEmployeeParameters = null;
                    DataViewEmployeeParameters = new DataView(DataSet.Tables["EmployeeParameters"],
                        "EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString(),
                        "",
                        DataViewRowState.CurrentRows);

                    //Loop Through Each Employee's Pay Categories
                    for (int intEmployeeParameterRow = 0; intEmployeeParameterRow < DataViewEmployeeParameters.Count; intEmployeeParameterRow++)
                    {
                        strEmployeeExceptionIndicator = "";
                        strEmployeeNormalIndicator = "";
                        strEmployeeBlankIndicator = "";

                        intPeriodNormalTimeMinutes = 0;
                        intPeriodOverTime1Minutes = 0;
                        intPeriodOverTime2Minutes = 0;
                        intPeriodOverTime3Minutes = 0;
                        intPeriodPaidHolidayWorkedMinutes = 0;

                        dblPaidHolidayWorkedHoursDecimal = 0;

                        dtEmployeeLastRundate = Convert.ToDateTime(DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_LAST_RUNDATE"]);

                        DataViewPayCategoryWeek = null;
                        DataViewPayCategoryWeek = new DataView(DataSet.Tables["PayCategoryWeek"],
                            "PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString(),
                            "",
                            DataViewRowState.CurrentRows);

                        blnEmployeeTimeSheetsForPayCategory = false;

                        //By Week//By Week
                        for (int intPayCategoryRow = 0; intPayCategoryRow < DataViewPayCategoryWeek.Count; intPayCategoryRow++)
                        {
                            //2017-07-31 - Moved Here
                            DataViewTimeSheetDayTotal = null;
                            DataViewTimeSheetDayTotal = new DataView(DataSet.Tables["TimeSheetDayTotals"],
                                "PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString() + " AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString()
                                + " AND DAY_DATE <= '" + Convert.ToDateTime(DataViewPayCategoryWeek[intPayCategoryRow]["WEEK_DATE"]).ToString("yyyy-MM-dd")
                                + "' AND DAY_DATE >= '" + Convert.ToDateTime(DataViewPayCategoryWeek[intPayCategoryRow]["WEEK_DATE_FROM"]).ToString("yyyy-MM-dd") + "'",
                                "DAY_DATE",
                                DataViewRowState.CurrentRows);
                            
                            if (DataViewTimeSheetDayTotal.Count == 0)
                            {
                                continue;
                            }
                            else
                            {
                                blnEmployeeTimeSheetsForPayCategory = true;
                            }

                            //Used To Work Out If Week has Blank Records
                            DataSet.Tables["DaysInWeek"].Rows.Clear();

                            strEmployeeWeekExceptionIndicator = "";
                            strEmployeeWeekNormalIndicator = "";
                            strEmployeeWeekBlankIndicator = "";

                            intTotalMinutesPaidForWeek = 0;
                            intNormalHoursBoundary = 0;
                            intOverTime1HoursBoundary = 0;
                            intOverTime2HoursBoundary = 0;
                            intOverTime3HoursBoundary = 0;
                            intOverTimeCF = 0;

                            //Accumulate Days Worked
                            if (DataViewPayCategoryWeek[intPayCategoryRow]["OVERTIME_IND"].ToString() == "A")
                            {
                                System.TimeSpan ts = Convert.ToDateTime(DataViewPayCategoryWeek[intPayCategoryRow]["WEEK_DATE"]).Subtract(Convert.ToDateTime(DataViewPayCategoryWeek[intPayCategoryRow]["WEEK_DATE_FROM"]));

                                if (ts.Days == 6
                                    | Convert.ToInt32(Convert.ToDateTime(DataViewPayCategoryWeek[intPayCategoryRow]["WEEK_DATE_FROM"]).DayOfWeek) == 1)
                                {
                                    intNormalHoursBoundary = Convert.ToInt32(DataViewPayCategoryWeek[intPayCategoryRow]["WEEK_OVERTIME_BOUNDARY_MINUTES"]);

                                    if (ts.Days != 6
                                        & Convert.ToInt32(Convert.ToDateTime(DataViewPayCategoryWeek[intPayCategoryRow]["WEEK_DATE_FROM"]).DayOfWeek) == 1)
                                    {
                                        //Needs To Be Checked
                                        intOverTimeCF = Convert.ToInt32(DataViewPayCategoryWeek[intPayCategoryRow]["WEEK_OVERTIME_BOUNDARY_MINUTES"]);
                                    }
                                }
                                else
                                {
                                    //Errol NEEDS to Check (OVERTIME_VALUE_BF from Previous Run Carried Forward)
                                    intNormalHoursBoundary = Convert.ToInt32(DataViewPayCategoryWeek[intPayCategoryRow]["WEEK_OVERTIME_BOUNDARY_MINUTES"]) + Convert.ToInt32(DataViewEmployeeParameters[intEmployeeParameterRow]["OVERTIME_VALUE_BF"]);
                                }
                            }

                            intWeekNormalMinutes = 0;
                            intWeekOverTime1Minutes = 0;
                            intWeekOverTime2Minutes = 0;
                            intWeekOverTime3Minutes = 0;
                            intWeekPaidHolidayWorkedMinutes = 0;

                            //Set to Do Week Day First (Eg Thursday,Friday,Monday,Tuesday.Wednesday before WeekEnd Days
                            blnWeekDays = true;

                        Calculate_Wages_From_TimeSheets_Do_WeekEnd:

                            for (int intDayRow = 0; intDayRow < DataViewTimeSheetDayTotal.Count; intDayRow++)
                            {
                                if (blnWeekDays == true)
                                {
                                    //WeekEnd
                                    if (Convert.ToInt32(Convert.ToDateTime(DataViewTimeSheetDayTotal[intDayRow]["DAY_DATE"]).DayOfWeek) == 0
                                    | Convert.ToInt32(Convert.ToDateTime(DataViewTimeSheetDayTotal[intDayRow]["DAY_DATE"]).DayOfWeek) == 6)
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    //WeekDays
                                    if (Convert.ToInt32(Convert.ToDateTime(DataViewTimeSheetDayTotal[intDayRow]["DAY_DATE"]).DayOfWeek) > 0
                                    & Convert.ToInt32(Convert.ToDateTime(DataViewTimeSheetDayTotal[intDayRow]["DAY_DATE"]).DayOfWeek) < 6)
                                    {
                                        continue;
                                    }
                                }

                                if (DataSet.Tables["DaysInWeek"].Rows.Count > 0)
                                {
                                    if (Convert.ToDateTime(DataSet.Tables["DaysInWeek"].Rows[DataSet.Tables["DaysInWeek"].Rows.Count - 1]["DAY_DATE"]) != Convert.ToDateTime(DataViewTimeSheetDayTotal[intDayRow]["DAY_DATE"]))
                                    {
                                        DataRow myDataRow = DataSet.Tables["DaysInWeek"].NewRow();
                                        myDataRow["DAY_DATE"] = Convert.ToDateTime(DataViewTimeSheetDayTotal[intDayRow]["DAY_DATE"]);
                                        DataSet.Tables["DaysInWeek"].Rows.Add(myDataRow);
                                    }
                                }
                                else
                                {
                                    DataRow myDataRow = DataSet.Tables["DaysInWeek"].NewRow();
                                    myDataRow["DAY_DATE"] = Convert.ToDateTime(DataViewTimeSheetDayTotal[intDayRow]["DAY_DATE"]);
                                    DataSet.Tables["DaysInWeek"].Rows.Add(myDataRow);
                                }

                                intTotalMinutesPaidForWeek += Convert.ToInt32(DataViewTimeSheetDayTotal[intDayRow]["DAY_PAID_MINUTES"]);

                                //Day is Public Holiday - Has it's Own Roleip Column
                                if (DataViewTimeSheetDayTotal[intDayRow]["PAIDHOLIDAY_INDICATOR"].ToString() == "Y")
                                {
                                    intWeekPaidHolidayWorkedMinutes += Convert.ToInt32(DataViewTimeSheetDayTotal[intDayRow]["DAY_PAID_MINUTES"]);
                                    continue;
                                }

                                clsISUtilities.Calculate_Wage_Time_Breakdown(DataViewPayCategoryWeek, intPayCategoryRow,
                                    Convert.ToInt32(Convert.ToDateTime(DataViewTimeSheetDayTotal[intDayRow]["DAY_DATE"]).DayOfWeek), Convert.ToInt32(DataViewTimeSheetDayTotal[intDayRow]["DAY_PAID_MINUTES"]),
                                    intNormalHoursBoundary,
                                    ref intOverTime1HoursBoundary, ref intOverTime2HoursBoundary,
                                    ref intOverTime3HoursBoundary,
                                    ref intDayNormalHours,
                                    ref intDayOverTime1Hours, ref intDayOverTime2Hours,
                                    ref intDayOverTime3Hours,
                                    ref intWeekNormalMinutes, ref intWeekOverTime1Minutes,
                                    ref intWeekOverTime2Minutes, ref intWeekOverTime3Minutes);

                                if (DataViewTimeSheetDayTotal[intDayRow]["INDICATOR"].ToString() == "E")
                                {
                                    strEmployeeWeekExceptionIndicator = "Y";
                                }
                                else
                                {
                                    strEmployeeWeekNormalIndicator = "Y";
                                }
                            }

                            if (blnWeekDays == true)
                            {
                                blnWeekDays = false;
                                goto Calculate_Wages_From_TimeSheets_Do_WeekEnd;
                            }

                            TimeSpan tsTimeSpan = Convert.ToDateTime(DataViewPayCategoryWeek[intPayCategoryRow]["WEEK_DATE"]).AddDays(1).Subtract(Convert.ToDateTime(DataViewPayCategoryWeek[intPayCategoryRow]["WEEK_DATE_FROM"]));

                            //Blank Records in Week for Employee
                            if (DataSet.Tables["DaysInWeek"].Rows.Count == tsTimeSpan.Days)
                            {
                                strEmployeeWeekBlankIndicator = "";
                            }
                            else
                            {
                                strEmployeeWeekBlankIndicator = "Y";
                            }

                            strQry.Clear();
                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_WEEK_CURRENT");
                            strQry.AppendLine(" SET ");
                            strQry.AppendLine(" NORMALTIME_MINUTES = " + intWeekNormalMinutes);
                            strQry.AppendLine(",OVERTIME1_MINUTES = " + intWeekOverTime1Minutes);
                            strQry.AppendLine(",OVERTIME2_MINUTES = " + intWeekOverTime2Minutes);
                            strQry.AppendLine(",OVERTIME3_MINUTES = " + intWeekOverTime3Minutes);
                            strQry.AppendLine(",PAIDHOLIDAY_MINUTES = " + intWeekPaidHolidayWorkedMinutes);
                            strQry.AppendLine(",TOTAL_MINUTES = " + intTotalMinutesPaidForWeek);
                            strQry.AppendLine(",EXCEPTION_INDICATOR = " + clsDBConnectionObjects.Text2DynamicSQL(strEmployeeWeekExceptionIndicator));
                            strQry.AppendLine(",NORMAL_INDICATOR = " + clsDBConnectionObjects.Text2DynamicSQL(strEmployeeWeekNormalIndicator));
                            strQry.AppendLine(",BLANK_INDICATOR = " + clsDBConnectionObjects.Text2DynamicSQL(strEmployeeWeekBlankIndicator));

                            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString());
                            strQry.AppendLine(" AND WEEK_DATE = '" + Convert.ToDateTime(DataViewPayCategoryWeek[intPayCategoryRow]["WEEK_DATE"]).ToString("yyyy-MM-dd") + "'");

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                            //Add Week Totals To Pay Period Totals
                            intPeriodNormalTimeMinutes += intWeekNormalMinutes;
                            intPeriodOverTime1Minutes += intWeekOverTime1Minutes;
                            intPeriodOverTime2Minutes += intWeekOverTime2Minutes;
                            intPeriodOverTime3Minutes += intWeekOverTime3Minutes;
                            intPeriodPaidHolidayWorkedMinutes += intWeekPaidHolidayWorkedMinutes;

                            if (strEmployeeWeekExceptionIndicator == "Y")
                            {
                                strEmployeeExceptionIndicator = "Y";
                            }
                            else
                            {
                                if (strEmployeeWeekBlankIndicator == "Y")
                                {
                                    strEmployeeBlankIndicator = "Y";
                                }
                                else
                                {
                                    strEmployeeNormalIndicator = "Y";
                                }
                            }
                        }

                        if (blnEmployeeTimeSheetsForPayCategory == false)
                        {
                            //2017-07-31
                            strQry.Clear();

                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT");

                            strQry.AppendLine(" SET PAY_CATEGORY_USED_IND = 'N'");

                            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                            strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString());
                            strQry.AppendLine(" AND RUN_TYPE = 'P'");

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                            
                            continue;
                        }

                        //Round Hours 
                        intNormalTimeMinutesRounded = intPeriodNormalTimeMinutes;
                        intOverTime1MinutesRounded = intPeriodOverTime1Minutes;
                        intOverTime2MinutesRounded = intPeriodOverTime2Minutes;
                        intOverTime3MinutesRounded = intPeriodOverTime3Minutes;
                        intPaidHolidayWorkedMinutesRounded = intPeriodPaidHolidayWorkedMinutes;

                        if (intNormalTimeMinutesRounded > 0)
                        {
                            clsISUtilities.Round_For_Period(Convert.ToInt32(DataViewPayCategoryWeek[0]["PAY_PERIOD_ROUNDING_IND"]), Convert.ToInt32(DataViewPayCategoryWeek[0]["PAY_PERIOD_ROUNDING_MINUTES"]), ref intNormalTimeMinutesRounded);
                        }

                        if (intOverTime1MinutesRounded > 0)
                        {
                            clsISUtilities.Round_For_Period(Convert.ToInt32(DataViewPayCategoryWeek[0]["PAY_PERIOD_ROUNDING_IND"]), Convert.ToInt32(DataViewPayCategoryWeek[0]["PAY_PERIOD_ROUNDING_MINUTES"]), ref intOverTime1MinutesRounded);
                        }

                        if (intOverTime2MinutesRounded > 0)
                        {
                            clsISUtilities.Round_For_Period(Convert.ToInt32(DataViewPayCategoryWeek[0]["PAY_PERIOD_ROUNDING_IND"]), Convert.ToInt32(DataViewPayCategoryWeek[0]["PAY_PERIOD_ROUNDING_MINUTES"]), ref intOverTime2MinutesRounded);
                        }

                        if (intOverTime3MinutesRounded > 0)
                        {
                            clsISUtilities.Round_For_Period(Convert.ToInt32(DataViewPayCategoryWeek[0]["PAY_PERIOD_ROUNDING_IND"]), Convert.ToInt32(DataViewPayCategoryWeek[0]["PAY_PERIOD_ROUNDING_MINUTES"]), ref intOverTime3MinutesRounded);
                        }

                        if (intPaidHolidayWorkedMinutesRounded > 0)
                        {
                            clsISUtilities.Round_For_Period(Convert.ToInt32(DataViewPayCategoryWeek[0]["PAY_PERIOD_ROUNDING_IND"]), Convert.ToInt32(DataViewPayCategoryWeek[0]["PAY_PERIOD_ROUNDING_MINUTES"]), ref intPaidHolidayWorkedMinutesRounded);
                        }

                        dblNormalHoursDecimal = clsISUtilities.Convert_Time_To_Decimal(intNormalTimeMinutesRounded);
                        dblOverTime1HoursDecimal = clsISUtilities.Convert_Time_To_Decimal(intOverTime1MinutesRounded);
                        dblOverTime2HoursDecimal = clsISUtilities.Convert_Time_To_Decimal(intOverTime2MinutesRounded);
                        dblOverTime3HoursDecimal = clsISUtilities.Convert_Time_To_Decimal(intOverTime3MinutesRounded);
                        dblPaidHolidayWorkedHoursDecimal = clsISUtilities.Convert_Time_To_Decimal(intPaidHolidayWorkedMinutesRounded);

                        //Normal Time
                        strQry.Clear();
                        strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" MINUTES = " + intPeriodNormalTimeMinutes);
                        strQry.AppendLine(",MINUTES_ROUNDED = " + intNormalTimeMinutesRounded);
                        strQry.AppendLine(",HOURS_DECIMAL = " + dblNormalHoursDecimal);
                        strQry.AppendLine(",HOURS_DECIMAL_ORIGINAL = " + dblNormalHoursDecimal);
                        strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString());
                        strQry.AppendLine(" AND EARNING_NO = 2 ");
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                        strQry.AppendLine(" AND RUN_TYPE = 'P' ");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                        //OverTime1
                        strQry.Clear();
                        strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" MINUTES = " + intPeriodOverTime1Minutes);
                        strQry.AppendLine(",MINUTES_ROUNDED = " + intOverTime1MinutesRounded);
                        strQry.AppendLine(",HOURS_DECIMAL = " + dblOverTime1HoursDecimal);
                        strQry.AppendLine(",HOURS_DECIMAL_ORIGINAL = " + dblOverTime1HoursDecimal);
                        strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString());
                        strQry.AppendLine(" AND EARNING_NO = 3 ");
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                        strQry.AppendLine(" AND RUN_TYPE = 'P' ");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                        //OverTime2
                        strQry.Clear();
                        strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" MINUTES = " + intPeriodOverTime2Minutes);
                        strQry.AppendLine(",MINUTES_ROUNDED = " + intOverTime2MinutesRounded);
                        strQry.AppendLine(",HOURS_DECIMAL = " + dblOverTime2HoursDecimal);
                        strQry.AppendLine(",HOURS_DECIMAL_ORIGINAL = " + dblOverTime2HoursDecimal);
                        strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString());
                        strQry.AppendLine(" AND EARNING_NO = 4 ");
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                        strQry.AppendLine(" AND RUN_TYPE = 'P' ");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                        //OverTime3
                        strQry.Clear();
                        strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" MINUTES = " + intPeriodOverTime3Minutes);
                        strQry.AppendLine(",MINUTES_ROUNDED = " + intOverTime3MinutesRounded);
                        strQry.AppendLine(",HOURS_DECIMAL = " + dblOverTime3HoursDecimal);
                        strQry.AppendLine(",HOURS_DECIMAL_ORIGINAL = " + dblOverTime3HoursDecimal);
                        strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString());
                        strQry.AppendLine(" AND EARNING_NO = 5 ");
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                        strQry.AppendLine(" AND RUN_TYPE = 'P' ");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                        //Public Holiday Worked
                        strQry.Clear();
                        strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" MINUTES = " + intPeriodPaidHolidayWorkedMinutes);
                        strQry.AppendLine(",MINUTES_ROUNDED = " + intPaidHolidayWorkedMinutesRounded);
                        strQry.AppendLine(",HOURS_DECIMAL = " + dblPaidHolidayWorkedHoursDecimal);
                        strQry.AppendLine(",HOURS_DECIMAL_ORIGINAL = " + dblPaidHolidayWorkedHoursDecimal);
                        strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString());
                        strQry.AppendLine(" AND EARNING_NO = 9 ");
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                        strQry.AppendLine(" AND RUN_TYPE = 'P' ");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                        strQry.Clear();
                        strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT");
                        strQry.AppendLine(" SET ");

                        if (intOverTimeCF != 0)
                        {
                            strQry.AppendLine(" OVERTIME_VALUE_CF = " + (intOverTimeCF - intTotalMinutesPaidForWeek));
                            strQry.AppendLine(",EXCEPTION_INDICATOR = " + clsDBConnectionObjects.Text2DynamicSQL(strEmployeeExceptionIndicator));
                        }
                        else
                        {
                            strQry.AppendLine(" EXCEPTION_INDICATOR = " + clsDBConnectionObjects.Text2DynamicSQL(strEmployeeExceptionIndicator));
                        }

                        strQry.AppendLine(",NORMAL_INDICATOR = " + clsDBConnectionObjects.Text2DynamicSQL(strEmployeeNormalIndicator));
                        strQry.AppendLine(",BLANK_INDICATOR = " + clsDBConnectionObjects.Text2DynamicSQL(strEmployeeBlankIndicator));

                        strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataViewEmployeeParameters[intEmployeeParameterRow]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                        //2017-07-19
                        strQry.AppendLine(" AND RUN_TYPE = 'P' ");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                    }
                }

                strQry.Clear();

                strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ");
                strQry.AppendLine(" DISABLE TRIGGER tgr_EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //Reset All 
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" INCLUDED_IN_RUN_IND = NULL");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" UPDATE ETC");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" INCLUDED_IN_RUN_IND = 'Y'");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO");
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                strQry.AppendLine(" AND ((ETC.TIMESHEET_DATE > E.EMPLOYEE_LAST_RUNDATE");
                strQry.AppendLine(" AND E.FIRST_RUN_COMPLETED_IND = 'Y')");
                strQry.AppendLine(" OR (ETC.TIMESHEET_DATE >= E.EMPLOYEE_LAST_RUNDATE");
                strQry.AppendLine(" AND ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= '" + parPayPeriodDate.ToString("yyyy-MM-dd") + "'");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ");
                strQry.AppendLine(" ENABLE TRIGGER tgr_EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ");
                strQry.AppendLine(" DISABLE TRIGGER tgr_EMPLOYEE_TIME_ATTEND_BREAK_CURRENT_Maintain_EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //Reset All 
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" INCLUDED_IN_RUN_IND = NULL");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" UPDATE EBC");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" INCLUDED_IN_RUN_IND = 'Y'");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT EBC");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON EBC.COMPANY_NO = E.COMPANY_NO");
                strQry.AppendLine(" AND EBC.EMPLOYEE_NO = E.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" WHERE EBC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                strQry.AppendLine(" AND ((EBC.BREAK_DATE > E.EMPLOYEE_LAST_RUNDATE");
                strQry.AppendLine(" AND E.FIRST_RUN_COMPLETED_IND = 'Y')");
                strQry.AppendLine(" OR (EBC.BREAK_DATE >= E.EMPLOYEE_LAST_RUNDATE");
                strQry.AppendLine(" AND ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                strQry.AppendLine(" AND EBC.BREAK_DATE <= '" + parPayPeriodDate.ToString("yyyy-MM-dd") + "'");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ");
                strQry.AppendLine(" ENABLE TRIGGER tgr_EMPLOYEE_TIME_ATTEND_BREAK_CURRENT_Maintain_EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.COMPANY");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" TIME_ATTENDANCE_RUN_IND = 'Y'");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //2017-07-11
                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.PAYROLL_RUN_QUEUE_COMPLETED");

                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",USER_NO");
                strQry.AppendLine(",PAY_PERIOD_DATE");
                strQry.AppendLine(",PAY_CATEGORY_NUMBERS");
                strQry.AppendLine(",PAYROLL_RUN_QUEUE_IND");
                strQry.AppendLine(",START_RUN_DATE");
                strQry.AppendLine(",END_RUN_DATE)");

                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",USER_NO");
                strQry.AppendLine(",PAY_PERIOD_DATE");
                strQry.AppendLine(",PAY_CATEGORY_NUMBERS");
                strQry.AppendLine(",'C'");
                strQry.AppendLine(",START_RUN_DATE");
                strQry.AppendLine(",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" FROM InteractPayroll.dbo.PAYROLL_RUN_QUEUE");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                //2017-07-11
                strQry.Clear();

                strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.PAYROLL_RUN_QUEUE");
                
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            Calculate_TimeAttendance_From_TimeSheets_Continue:

                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
                strQry.AppendLine(" SET BACKUP_DB_IND = 1");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            }
            catch (Exception ex)
            {
                //2017-07-11
                Write_Log(ex, strClassNameFunctionAndParameters, strQry.ToString(), true);

                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.PAYROLL_RUN_QUEUE");

                strQry.AppendLine(" SET ");
                strQry.AppendLine(" PAYROLL_RUN_QUEUE_IND = 'F'");
                strQry.AppendLine(",END_RUN_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                throw;
            }
            
            return strReturnPayCategoryNosInError;
        }

        public void Write_Log(string message)
        {
            FileInfo fiLogFile = new FileInfo(pvtstrLogFileName);

            StreamWriter swErrorStreamWriter = fiLogFile.AppendText();

            swErrorStreamWriter.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + message);

            swErrorStreamWriter.Close();
        }

        public void Write_Log(Exception exception, string classNameFunctionAndParameters, string sql, bool sendEmail)
        {
            string strMessage = classNameFunctionAndParameters + "\n\nSQL=" + sql + "\nException=" + exception.Message;

            if (exception.InnerException != null)
            {
                strMessage += "\n\n" + exception.InnerException.Message;
            }

            FileInfo fiLogFile = new FileInfo(pvtstrLogFileName);

            StreamWriter swErrorStreamWriter = fiLogFile.AppendText();

            swErrorStreamWriter.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + strMessage);

            swErrorStreamWriter.Close();

            if (sendEmail == true)
            {
                var smtp = new SmtpClient();

                try
                {
                    //Email
                    var fromAddress = new MailAddress(pvtstrSmtpEmailAddress, pvtstrSmtpEmailAddressDescription);
                    var toAddress = new MailAddress(pvtstrSmtpEmailAddress, "Errol Le Roux");

                    string subject = "Payroll/Time Attendance Run Error - " + DateTime.Now.ToString("dd MMMM yyyy");

                    smtp.Host = pvtstrSmtpHostName;
                    smtp.Port = pvtintSmtpHostPort;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(pvtstrSmtpEmailAddress, pvtstrSmtpEmailAddressPassword);

                    var message = new MailMessage(fromAddress, toAddress);

                    message.Subject = subject;
                    message.Body = strMessage;

                    smtp.Send(message);
                }
                catch
                {
                }
            }
        }
    }
}
