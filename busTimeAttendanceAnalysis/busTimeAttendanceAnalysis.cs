using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busTimeAttendanceAnalysis
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busTimeAttendanceAnalysis()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parInt64CompanyNo, string parstrFromProgram)
        {
            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND TIME_ATTENDANCE_RUN_IND = 'Y'");
            }
            else
            {
                strQry.AppendLine(" AND WAGE_RUN_IND = 'Y'");
            }

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

            if (DataSet.Tables["Temp"].Rows.Count == 0)
            {
                goto Get_Form_Records_Continue;
            }
   
            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT ");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" 'Time Attendance' AS PAYROLL_TYPE ");
            }
            else
            {

                strQry.AppendLine(" 'Wages' AS PAYROLL_TYPE ");
            }
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");
        
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");
            strQry.AppendLine(" ON C.COMPANY_NO = PCPC.COMPANY_NO ");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = 'W'");
            }

            strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P'");
            
            strQry.AppendLine(" WHERE C.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayrollType", parInt64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" PPC.COMPANY_NO");
            strQry.AppendLine(",PPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PPC.PAY_PERIOD_DATE");
            strQry.AppendLine(",PPC.PAY_PERIOD_DATE_FROM");
            strQry.AppendLine(",PPC.PAY_PUBLIC_HOLIDAY_IND");
 
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PPC");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON PPC.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND PPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
                     
            strQry.AppendLine(" WHERE PPC.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PPC.PAY_CATEGORY_NO > 0");
           
            strQry.AppendLine(" AND PPC.RUN_TYPE = 'P'");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PPC.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PPC.PAY_CATEGORY_TYPE = 'W'");
            }
          
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategory", parInt64CompanyNo);
           
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PCPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PCPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PHC.PUBLIC_HOLIDAY_DATE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_CURRENT PHC");
            strQry.AppendLine(" ON PCPC.COMPANY_NO = PHC.COMPANY_NO");
            strQry.AppendLine(" AND PCPC.RUN_NO = PHC.RUN_NO");
            strQry.AppendLine(" AND PCPC.PAY_PERIOD_DATE_FROM <= PHC.PUBLIC_HOLIDAY_DATE");
            strQry.AppendLine(" AND PCPC.PAY_PERIOD_DATE >= PHC.PUBLIC_HOLIDAY_DATE");

            strQry.AppendLine(" WHERE PCPC.COMPANY_NO = " + parInt64CompanyNo);

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = 'T' ");
            }
            else
            {
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = 'W' ");
            }

            strQry.AppendLine(" AND PCPC.PAY_PUBLIC_HOLIDAY_IND = 'Y' ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PublicHoliday", parInt64CompanyNo);

            string parstrPayCategoryType = "";
            string parstrPayCategoryNoIN = "";

            if (DataSet.Tables["PayCategory"].Rows.Count > 0)
            {
                for (int intPayrollTypeRow = 0; intPayrollTypeRow < DataSet.Tables["PayrollType"].Rows.Count; intPayrollTypeRow++)
                {
                    parstrPayCategoryType = DataSet.Tables["PayrollType"].Rows[intPayrollTypeRow]["PAYROLL_TYPE"].ToString().Substring(0,1);
                    
                    parstrPayCategoryNoIN = "";

                    for (int intRow = 0; intRow < DataSet.Tables["PayCategory"].Rows.Count; intRow++)
                    {
                        if (DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == parstrPayCategoryType)
                        {
                            if (intRow == 0)
                            {
                                parstrPayCategoryNoIN = "(" + DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_NO"].ToString();
                            }
                            else
                            {
                                parstrPayCategoryNoIN += "," + DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_NO"].ToString();
                            }
                        }
                    }

                    if (parstrPayCategoryNoIN == "")
                    {
                        continue;
                    }
                    else
                    {
                        parstrPayCategoryNoIN += ")";
                    }

                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EPCC.COMPANY_NO ");
                    strQry.AppendLine(",EPCC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(",EPCC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(",D.DAY_DATE");
                    strQry.AppendLine(",D.DAY_NO");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P' ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");
                    strQry.AppendLine(" ON D.DAY_DATE <= '" + Convert.ToDateTime(DataSet.Tables["PayCategory"].Rows[0]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND D.DAY_DATE > ");

                    strQry.AppendLine(" CASE ");

                    //Errol - 2015-02-17
                    strQry.AppendLine(" WHEN E.FIRST_RUN_COMPLETED_IND = 'Y'");

                    strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE ");

                    strQry.AppendLine(" ELSE DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" END  ");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" EPCC.COMPANY_NO ");
                    strQry.AppendLine(",EPCC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(",EPCC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(",D.DAY_DATE");
                    strQry.AppendLine(",D.DAY_NO");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Dates", parInt64CompanyNo);

                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" PCB.COMPANY_NO ");
                    strQry.AppendLine(",PCB.PAY_CATEGORY_NO");
                    strQry.AppendLine(",PCB.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",PCB.WORKED_TIME_MINUTES");
                    strQry.AppendLine(",PCB.BREAK_MINUTES");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK_CURRENT PCB");

                    strQry.AppendLine(" WHERE PCB.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PCB.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
                    strQry.AppendLine(" AND PCB.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
                    
                    strQry.AppendLine(" UNION ");

                    //Create 1 Row with 0 WORKED_TIME_MINUTES and 0 BREAK_MINUTES
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" PC.COMPANY_NO");
                    strQry.AppendLine(",PC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(",0 AS WORKED_TIME_MINUTES");
                    strQry.AppendLine(",0 AS BREAK_MINUTES");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");

                    strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
                    strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" ORDER BY ");

                    strQry.AppendLine(" 1");
                    strQry.AppendLine(",2");
                    strQry.AppendLine(",3");
                    strQry.AppendLine(",4");
                    strQry.AppendLine(",5");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "BreakRange", parInt64CompanyNo);

                    strQry.Clear();

                    strQry.AppendLine(" SELECT");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",EPCC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EPCC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",E.EMPLOYEE_CODE");
                    strQry.AppendLine(",E.EMPLOYEE_SURNAME");
                    strQry.AppendLine(",E.EMPLOYEE_NAME");
                    strQry.AppendLine(",E.EMPLOYEE_NO");

                    //Errol 2013-06-15
                    strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

                    strQry.AppendLine(" CASE ");

                    //Errol - 2015-02-17
                    strQry.AppendLine(" WHEN E.FIRST_RUN_COMPLETED_IND = 'Y'");

                    strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE ");

                    strQry.AppendLine(" ELSE DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                    strQry.AppendLine(" ORDER BY E.EMPLOYEE_CODE");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parInt64CompanyNo);

                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EEC.COMPANY_NO");
                    strQry.AppendLine(",EEC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EEC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EEC.EMPLOYEE_NO");
                    strQry.AppendLine(",EEC.EARNING_NO");
                    strQry.AppendLine(",EEC.MINUTES");
                    strQry.AppendLine(",EEC.MINUTES_ROUNDED");
                    strQry.AppendLine(",EEC.HOURS_DECIMAL");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EEC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EEC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EEC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EEC.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
                    strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));

                    strQry.AppendLine(" AND EEC.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EEC.EARNING_NO IN (2,3,4,5,9)");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                    strQry.AppendLine(" ORDER BY  ");
                    strQry.AppendLine(" EEC.EMPLOYEE_NO");
                    strQry.AppendLine(",EEC.EARNING_NO");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeEarning", parInt64CompanyNo);

                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" TEMP_TABLE.COMPANY_NO");
                    strQry.AppendLine(",TEMP_TABLE.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",TEMP_TABLE.PAY_CATEGORY_NO");
                    strQry.AppendLine(",TEMP_TABLE.WEEK_DATE");
                    strQry.AppendLine(",TEMP_TABLE.WEEK_DATE_FROM");
                    strQry.AppendLine(",TEMP_TABLE.MON_TIME_MINUTES");
                    strQry.AppendLine(",TEMP_TABLE.TUE_TIME_MINUTES");
                    strQry.AppendLine(",TEMP_TABLE.WED_TIME_MINUTES");
                    strQry.AppendLine(",TEMP_TABLE.THU_TIME_MINUTES");
                    strQry.AppendLine(",TEMP_TABLE.FRI_TIME_MINUTES");
                    strQry.AppendLine(",TEMP_TABLE.SAT_TIME_MINUTES");
                    strQry.AppendLine(",TEMP_TABLE.SUN_TIME_MINUTES");
                    strQry.AppendLine(",TEMP_TABLE.OVERTIME1_RATE");
                    strQry.AppendLine(",TEMP_TABLE.OVERTIME2_RATE");
                    strQry.AppendLine(",TEMP_TABLE.OVERTIME3_RATE");
                    strQry.AppendLine(",TEMP_TABLE.PAIDHOLIDAY_RATE");
                    strQry.AppendLine(",TEMP_TABLE.TOTAL_DAILY_TIME_OVERTIME");
                    strQry.AppendLine(",TEMP_TABLE.DAILY_ROUNDING_IND");
                    strQry.AppendLine(",TEMP_TABLE.DAILY_ROUNDING_MINUTES");
                    strQry.AppendLine(",TEMP_TABLE.PAY_PERIOD_ROUNDING_IND");
                    strQry.AppendLine(",TEMP_TABLE.PAY_PERIOD_ROUNDING_MINUTES");
                    strQry.AppendLine(",TEMP_TABLE.EXCEPTION_SUN_ABOVE_MINUTES");
                    strQry.AppendLine(",TEMP_TABLE.EXCEPTION_SUN_BELOW_MINUTES");
                    strQry.AppendLine(",TEMP_TABLE.EXCEPTION_MON_ABOVE_MINUTES");
                    strQry.AppendLine(",TEMP_TABLE.EXCEPTION_MON_BELOW_MINUTES");
                    strQry.AppendLine(",TEMP_TABLE.EXCEPTION_TUE_ABOVE_MINUTES");
                    strQry.AppendLine(",TEMP_TABLE.EXCEPTION_TUE_BELOW_MINUTES");
                    strQry.AppendLine(",TEMP_TABLE.EXCEPTION_WED_ABOVE_MINUTES");
                    strQry.AppendLine(",TEMP_TABLE.EXCEPTION_WED_BELOW_MINUTES");
                    strQry.AppendLine(",TEMP_TABLE.EXCEPTION_THU_ABOVE_MINUTES");
                    strQry.AppendLine(",TEMP_TABLE.EXCEPTION_THU_BELOW_MINUTES");
                    strQry.AppendLine(",TEMP_TABLE.EXCEPTION_FRI_ABOVE_MINUTES");
                    strQry.AppendLine(",TEMP_TABLE.EXCEPTION_FRI_BELOW_MINUTES");
                    strQry.AppendLine(",TEMP_TABLE.EXCEPTION_SAT_ABOVE_MINUTES");
                    strQry.AppendLine(",TEMP_TABLE.EXCEPTION_SAT_BELOW_MINUTES");
                    strQry.AppendLine(",TEMP_TABLE.EXCEPTION_SHIFT_ABOVE_PERCENT");
                    strQry.AppendLine(",TEMP_TABLE.EXCEPTION_SHIFT_BELOW_PERCENT");
                    strQry.AppendLine(",TEMP_TABLE.OVERTIME_IND");
                    strQry.AppendLine(",TEMP_TABLE.SATURDAY_PAY_RATE");
                    strQry.AppendLine(",TEMP_TABLE.SATURDAY_PAY_RATE_IND");
                    strQry.AppendLine(",TEMP_TABLE.SUNDAY_PAY_RATE");
                    strQry.AppendLine(",TEMP_TABLE.SUNDAY_PAY_RATE_IND");

                    strQry.AppendLine(",TEMP_TABLE.OVERTIME1_MINUTES");
                    strQry.AppendLine(",TEMP_TABLE.OVERTIME2_MINUTES");
                    strQry.AppendLine(",TEMP_TABLE.OVERTIME3_MINUTES");

                    strQry.AppendLine(",PUBLIC_HOLIDAY_IND = ");

                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN NOT PUBLIC_HOLIDAY_DATE IS NULL THEN 'Y'");

                    strQry.AppendLine(" ELSE 'N'");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(" FROM ");

                    strQry.AppendLine("(SELECT ");
                    strQry.AppendLine(" PCWC.COMPANY_NO");
                    strQry.AppendLine(",PCWC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",PCWC.PAY_CATEGORY_NO");
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
                    strQry.AppendLine(",PCPC.EXCEPTION_SHIFT_ABOVE_PERCENT");
                    strQry.AppendLine(",PCPC.EXCEPTION_SHIFT_BELOW_PERCENT");
                    strQry.AppendLine(",PCPC.OVERTIME_IND");
                    strQry.AppendLine(",PCPC.SATURDAY_PAY_RATE");
                    strQry.AppendLine(",PCPC.SATURDAY_PAY_RATE_IND");
                    strQry.AppendLine(",PCPC.SUNDAY_PAY_RATE");
                    strQry.AppendLine(",PCPC.SUNDAY_PAY_RATE_IND");

                    strQry.AppendLine(",PCWC.OVERTIME1_MINUTES");
                    strQry.AppendLine(",PCWC.OVERTIME2_MINUTES");
                    strQry.AppendLine(",PCWC.OVERTIME3_MINUTES");

                    strQry.AppendLine(",MAX(PHC.PUBLIC_HOLIDAY_DATE) AS PUBLIC_HOLIDAY_DATE");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_CURRENT PCWC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");
                    strQry.AppendLine(" ON PCWC.COMPANY_NO = PCPC.COMPANY_NO ");
                    strQry.AppendLine(" AND PCWC.PAY_CATEGORY_NO = PCPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND PCWC.PAY_CATEGORY_TYPE = PCPC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P' ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C");
                    strQry.AppendLine(" ON PCWC.COMPANY_NO = C.COMPANY_NO ");
                    strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_CURRENT PHC");
                    strQry.AppendLine(" ON PCPC.COMPANY_NO = PHC.COMPANY_NO");
                    strQry.AppendLine(" AND PCPC.RUN_NO = PHC.RUN_NO");
                    strQry.AppendLine(" AND PCPC.PAY_PERIOD_DATE_FROM <= PHC.PUBLIC_HOLIDAY_DATE");
                    strQry.AppendLine(" AND PCPC.PAY_PERIOD_DATE >= PHC.PUBLIC_HOLIDAY_DATE");
                    strQry.AppendLine(" AND PCPC.PAY_PUBLIC_HOLIDAY_IND = 'Y'");

                    strQry.AppendLine(" WHERE PCWC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PCWC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
                    strQry.AppendLine(" AND PCWC.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" PCWC.COMPANY_NO");
                    strQry.AppendLine(",PCWC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",PCWC.PAY_CATEGORY_NO");
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
                    strQry.AppendLine(",PCPC.EXCEPTION_SHIFT_ABOVE_PERCENT");
                    strQry.AppendLine(",PCPC.EXCEPTION_SHIFT_BELOW_PERCENT");
                    strQry.AppendLine(",PCPC.OVERTIME_IND");
                    strQry.AppendLine(",PCPC.SATURDAY_PAY_RATE");
                    strQry.AppendLine(",PCPC.SATURDAY_PAY_RATE_IND");
                    strQry.AppendLine(",PCPC.SUNDAY_PAY_RATE");
                    strQry.AppendLine(",PCPC.SUNDAY_PAY_RATE_IND");

                    strQry.AppendLine(",PCWC.OVERTIME1_MINUTES");
                    strQry.AppendLine(",PCWC.OVERTIME2_MINUTES");
                    strQry.AppendLine(",PCWC.OVERTIME3_MINUTES) AS TEMP_TABLE");

                    strQry.AppendLine(" ORDER BY ");
                    strQry.AppendLine(" 1");
                    strQry.AppendLine(",2");
                    strQry.AppendLine(",3");
                    strQry.AppendLine(",4");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategoryWeek", parInt64CompanyNo);

                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" COMPANY_NO ");
                    strQry.AppendLine(",EMPLOYEE_NO ");
                    strQry.AppendLine(",PAY_CATEGORY_NO ");
                    strQry.AppendLine(",'" + parstrPayCategoryType + "' AS PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",WEEK_DATE ");
                    strQry.AppendLine(",WEEK_DATE_FROM ");
                    strQry.AppendLine(",NORMALTIME_MINUTES");
                    strQry.AppendLine(",OVERTIME1_MINUTES");
                    strQry.AppendLine(",OVERTIME2_MINUTES");
                    strQry.AppendLine(",OVERTIME3_MINUTES");
                    strQry.AppendLine(",PAIDHOLIDAY_MINUTES");
                    strQry.AppendLine(",TOTAL_MINUTES");
                    strQry.AppendLine(",EXCEPTION_INDICATOR");
                    strQry.AppendLine(",NORMAL_INDICATOR");
                    strQry.AppendLine(",BLANK_INDICATOR");
                    strQry.AppendLine(",PAIDHOLIDAY_INDICATOR");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_WEEK_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
                    strQry.AppendLine(" ORDER BY");
                    strQry.AppendLine(" EMPLOYEE_NO ");
                    strQry.AppendLine(",WEEK_DATE ");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeWeek", parInt64CompanyNo);

                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" ETC.COMPANY_NO");
                    strQry.AppendLine(",ETC.EMPLOYEE_NO");
                    strQry.AppendLine(",ETC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",'" + parstrPayCategoryType + "' AS PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",ETC.TIMESHEET_DATE");
                    strQry.AppendLine(",ETC.TIMESHEET_TIME_IN_MINUTES");
                    strQry.AppendLine(",ETC.TIMESHEET_TIME_OUT_MINUTES");
                    strQry.AppendLine(",ETC.CLOCKED_TIME_IN_MINUTES");
                    strQry.AppendLine(",ETC.CLOCKED_TIME_OUT_MINUTES");
                    //ELR - 2015-05-02
                    strQry.AppendLine(",ETC.INCLUDED_IN_RUN_IND");
                    strQry.AppendLine(",ETC.TIMESHEET_SEQ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

                    if (parstrPayCategoryType == "W")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
                        strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO ");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                    }
                    else
                    {
                        if (parstrPayCategoryType == "S")
                        {
                            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC");
                            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO ");
                            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
                            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        }
                        else
                        {
                            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC");
                            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO ");
                            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
                            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        }
                    }

                    strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
                    strQry.AppendLine(" AND ETC.TIMESHEET_DATE > ");

                    strQry.AppendLine(" CASE ");

                    //Errol - 2015-02-17
                    strQry.AppendLine(" WHEN E.FIRST_RUN_COMPLETED_IND = 'Y'");

                    strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE ");

                    strQry.AppendLine(" ELSE DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" END  ");

                    strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= '" + Convert.ToDateTime(DataSet.Tables["PayCategory"].Rows[0]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                    strQry.AppendLine(" ORDER BY ");
                    strQry.AppendLine(" ETC.EMPLOYEE_NO");
                    strQry.AppendLine(",ETC.TIMESHEET_DATE");
                    //Errol 2015-02-11
                    //strQry.AppendLine(",ETC.TIMESHEET_SEQ");
                    strQry.AppendLine(",ETC.TIMESHEET_TIME_IN_MINUTES");
                    strQry.AppendLine(",ETC.TIMESHEET_TIME_OUT_MINUTES");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "TimeSheet", parInt64CompanyNo);

                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EBC.COMPANY_NO");
                    strQry.AppendLine(",EBC.EMPLOYEE_NO");
                    strQry.AppendLine(",EBC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",'" + parstrPayCategoryType + "' AS PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EBC.BREAK_DATE");
                    strQry.AppendLine(",EBC.BREAK_TIME_IN_MINUTES");
                    strQry.AppendLine(",EBC.BREAK_TIME_OUT_MINUTES");
                    strQry.AppendLine(",EBC.CLOCKED_TIME_IN_MINUTES");
                    strQry.AppendLine(",EBC.CLOCKED_TIME_OUT_MINUTES");
                    //ELR - 2015-05-02
                    strQry.AppendLine(",EBC.INCLUDED_IN_RUN_IND");
                    strQry.AppendLine(",EBC.BREAK_SEQ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

                    if (parstrPayCategoryType == "W")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT EBC");
                        strQry.AppendLine(" ON E.COMPANY_NO = EBC.COMPANY_NO ");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EBC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                    }
                    else
                    {
                        if (parstrPayCategoryType == "S")
                        {
                            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT EBC");
                            strQry.AppendLine(" ON E.COMPANY_NO = EBC.COMPANY_NO ");
                            strQry.AppendLine(" AND E.EMPLOYEE_NO = EBC.EMPLOYEE_NO ");
                            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        }
                        else
                        {
                            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT EBC");
                            strQry.AppendLine(" ON E.COMPANY_NO = EBC.COMPANY_NO ");
                            strQry.AppendLine(" AND E.EMPLOYEE_NO = EBC.EMPLOYEE_NO ");
                            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        }
                    }

                    //Errol 2015-02-11
                    strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
                    strQry.AppendLine(" AND EBC.BREAK_DATE > ");

                    strQry.AppendLine(" CASE ");

                    //Errol - 2015-02-17
                    strQry.AppendLine(" WHEN E.FIRST_RUN_COMPLETED_IND = 'Y'");

                    strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE ");

                    strQry.AppendLine(" ELSE DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" END  ");

                    strQry.AppendLine(" AND EBC.BREAK_DATE <= '" + Convert.ToDateTime(DataSet.Tables["PayCategory"].Rows[0]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'");
                    
                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                    strQry.AppendLine(" ORDER BY ");
                    strQry.AppendLine(" EBC.EMPLOYEE_NO");
                    strQry.AppendLine(",EBC.BREAK_DATE");
                    //Errol 2015-02-11
                    //strQry.AppendLine(",EBC.BREAK_SEQ");
                    strQry.AppendLine(",EBC.BREAK_TIME_IN_MINUTES");
                    strQry.AppendLine(",EBC.BREAK_TIME_OUT_MINUTES");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Break", parInt64CompanyNo);

                    strQry.Clear();

                    //Where Only a Break Record exists for the Day (NO Timesheet Record)
                    strQry.AppendLine(" SELECT "); 
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",BREAK_SUMMARY_TABLE.PAY_CATEGORY_NO");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",BREAK_SUMMARY_TABLE.DAY_DATE");
                    strQry.AppendLine(",D.DAY_NO");
                    strQry.AppendLine(",0 AS DAY_PAID_MINUTES");
                    strQry.AppendLine(",BREAK_SUMMARY_TABLE.INDICATOR"); 
                    strQry.AppendLine(",BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES");
                 
                    strQry.AppendLine(",'' AS BREAK_INDICATOR ");
                    
                    strQry.AppendLine(",PAIDHOLIDAY_INDICATOR = ");
                    strQry.AppendLine(" CASE ");
                    
                    strQry.AppendLine(" WHEN NOT PHC.PUBLIC_HOLIDAY_DATE IS NULL "); 
                    strQry.AppendLine(" THEN 'Y' ");
                    
                    strQry.AppendLine(" ELSE ''");
                    
                    strQry.AppendLine(" END"); 
                    strQry.AppendLine(",BREAK_SUMMARY_TABLE.BREAK_INCLUDED_IN_RUN_IND AS INCLUDED_IN_RUN_INDICATOR ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");
                    
                    strQry.AppendLine(" INNER JOIN "); 
                    
                    strQry.AppendLine("(");

                    strQry.AppendLine("--2Start Break UNION 1");

                    strQry.AppendLine("SELECT ");
                    
                    strQry.AppendLine(" BREAK_TABLE.COMPANY_NO");
                    strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_NO");
                    strQry.AppendLine(",BREAK_TABLE.EMPLOYEE_NO");
                    strQry.AppendLine(",BREAK_TABLE.DAY_DATE");
                    strQry.AppendLine(",SUM(BREAK_TABLE.BREAK_ACCUM_MINUTES) AS BREAK_ACCUM_MINUTES"); 
                    strQry.AppendLine(",'' AS INDICATOR"); 
                    strQry.AppendLine(",MIN(ISNULL(BREAK_TABLE.INCLUDED_IN_RUN_IND,'N')) AS BREAK_INCLUDED_IN_RUN_IND");

                    strQry.AppendLine(" FROM ");
                    strQry.AppendLine("(");

                    strQry.AppendLine("--1Start Break UNION 1");

                    strQry.AppendLine(" SELECT DISTINCT");
                    strQry.AppendLine(" EBC.COMPANY_NO");

                    if (parstrPayCategoryType == "W")
                    {
                        strQry.AppendLine(",'W' AS PAY_CATEGORY_TYPE");
                    }
                    else
                    {
                        if (parstrPayCategoryType == "S")
                        {
                            strQry.AppendLine(",'S' AS PAY_CATEGORY_TYPE");
                        }
                        else
                        {
                            strQry.AppendLine(",'T' AS PAY_CATEGORY_TYPE");
                        }
                    }

                    strQry.AppendLine(",EBC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EBC.EMPLOYEE_NO"); 
                    strQry.AppendLine(",EBC.BREAK_DATE AS DAY_DATE");
                    strQry.AppendLine(",EBC.BREAK_SEQ");
                    
                    strQry.AppendLine(",BREAK_ACCUM_MINUTES = ");
                    
                    strQry.AppendLine(" CASE ");
                    
                    strQry.AppendLine(" WHEN ((EBC.BREAK_TIME_IN_MINUTES IS NULL");
                    strQry.AppendLine(" OR EBC.BREAK_TIME_OUT_MINUTES IS NULL)");
                    strQry.AppendLine(" OR (EBC.BREAK_TIME_IN_MINUTES > EBC2.BREAK_TIME_OUT_MINUTES");
                    strQry.AppendLine(" AND EBC.SORTED_REC <= EBC2.SORTED_REC)");
                    strQry.AppendLine(" OR (EBC.BREAK_TIME_IN_MINUTES < EBC2.BREAK_TIME_OUT_MINUTES");
                    strQry.AppendLine(" AND EBC.SORTED_REC > EBC2.SORTED_REC)) ");
                    
                    strQry.AppendLine(" THEN CASE ");
                    
                    strQry.AppendLine(" WHEN EBC.BREAK_TIME_OUT_MINUTES IS NULL OR EBC.BREAK_TIME_IN_MINUTES IS NULL"); 
                    strQry.AppendLine(" THEN 0 ");
                    
                    strQry.AppendLine(" WHEN EBC.BREAK_TIME_OUT_MINUTES > EBC.BREAK_TIME_IN_MINUTES ");
                    strQry.AppendLine(" THEN EBC.BREAK_TIME_OUT_MINUTES - EBC.BREAK_TIME_IN_MINUTES "); 
                    
                    strQry.AppendLine(" ELSE 0 ");
                    
                    strQry.AppendLine(" END ");
                    
                    strQry.AppendLine(" ELSE ");
                    strQry.AppendLine(" EBC.BREAK_TIME_OUT_MINUTES - EBC.BREAK_TIME_IN_MINUTES"); 
                    
                    strQry.AppendLine(" END ");
                    
                    strQry.AppendLine(",EBC.INCLUDED_IN_RUN_IND");
                    
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");
                    
                    strQry.AppendLine(" INNER JOIN  ");
                    
                    strQry.AppendLine("(SELECT  ");
                    
                    strQry.AppendLine(" ROW_NUMBER() OVER  ");
                    strQry.AppendLine(" (ORDER BY ");
                    strQry.AppendLine(" COMPANY_NO ");
                    strQry.AppendLine(",PAY_CATEGORY_NO ");
                    strQry.AppendLine(",EMPLOYEE_NO  ");
                    strQry.AppendLine(",BREAK_DATE   ");
                    strQry.AppendLine(",BREAK_TIME_IN_MINUTES ");
                    strQry.AppendLine(",BREAK_TIME_OUT_MINUTES) AS SORTED_REC  ");
                    
                    strQry.AppendLine(",COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO ");
                    strQry.AppendLine(",EMPLOYEE_NO"); 
                    strQry.AppendLine(",BREAK_DATE  ");
                    strQry.AppendLine(",BREAK_SEQ  ");
                    strQry.AppendLine(",BREAK_TIME_IN_MINUTES");
                    strQry.AppendLine(",BREAK_TIME_OUT_MINUTES ");
                    strQry.AppendLine(",INCLUDED_IN_RUN_IND ");

                    if (parstrPayCategoryType == "W")
                    {
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT) AS EBC");
                    }
                    else
                    {
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT) AS EBC");
                    }
              
                    strQry.AppendLine(" ON E.COMPANY_NO = EBC.COMPANY_NO");
                    strQry.AppendLine(" AND EBC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EBC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EBC.BREAK_DATE > ");

                    strQry.AppendLine(" CASE ");

                    //Errol - 2015-02-17
                    strQry.AppendLine(" WHEN E.FIRST_RUN_COMPLETED_IND = 'Y'");

                    strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE ");

                    strQry.AppendLine(" ELSE DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" END  ");

                    strQry.AppendLine(" AND EBC.BREAK_DATE <= '" + Convert.ToDateTime(DataSet.Tables["PayCategory"].Rows[0]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
                    
                    strQry.AppendLine(" INNER JOIN"); 
                    
                    strQry.AppendLine("(SELECT  ");
                    strQry.AppendLine(" ROW_NUMBER() OVER ");
                    strQry.AppendLine(" (ORDER BY ");
                    strQry.AppendLine(" COMPANY_NO ");
                    strQry.AppendLine(",PAY_CATEGORY_NO ");
                    strQry.AppendLine(",EMPLOYEE_NO  ");
                    strQry.AppendLine(",BREAK_DATE ");
                    strQry.AppendLine(",BREAK_TIME_IN_MINUTES ");
                    strQry.AppendLine(",BREAK_TIME_OUT_MINUTES) AS SORTED_REC ");
                    strQry.AppendLine(",COMPANY_NO ");
                    strQry.AppendLine(",PAY_CATEGORY_NO ");
                    strQry.AppendLine(",EMPLOYEE_NO ");
                    strQry.AppendLine(",BREAK_DATE ");
                    strQry.AppendLine(",BREAK_SEQ ");
                    strQry.AppendLine(",BREAK_TIME_IN_MINUTES");
                    strQry.AppendLine(",BREAK_TIME_OUT_MINUTES");
                   
                    if (parstrPayCategoryType == "W")
                    {
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT) AS EBC2");
                    }
                    else
                    {
                        if (parstrPayCategoryType == "S")
                        {
                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT) AS EBC2");
                        }
                        else
                        {
                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT) AS EBC2");
                        }
                    }
 
                    strQry.AppendLine(" ON EBC.COMPANY_NO = EBC2.COMPANY_NO");
                    strQry.AppendLine(" AND EBC.EMPLOYEE_NO = EBC2.EMPLOYEE_NO"); 
                    strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = EBC2.PAY_CATEGORY_NO"); 
                    strQry.AppendLine(" AND EBC.BREAK_DATE = EBC2.BREAK_DATE");
                    
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC"); 
                    strQry.AppendLine(" ON EBC.COMPANY_NO = EPC.COMPANY_NO"); 
                    strQry.AppendLine(" AND EBC.EMPLOYEE_NO = EPC.EMPLOYEE_NO"); 
                    strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

                    strQry.AppendLine("--1End Break UNION 1 ");

                    strQry.AppendLine(" ) AS BREAK_TABLE ");

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" BREAK_TABLE.COMPANY_NO");
                    strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_NO");
                    strQry.AppendLine(",BREAK_TABLE.EMPLOYEE_NO"); 
                    strQry.AppendLine(",BREAK_TABLE.DAY_DATE");

                    strQry.AppendLine("--2End Break UNION 1");

                    strQry.AppendLine(" ) AS BREAK_SUMMARY_TABLE");
                   
                    strQry.AppendLine(" ON E.COMPANY_NO = BREAK_SUMMARY_TABLE.COMPANY_NO");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = BREAK_SUMMARY_TABLE.EMPLOYEE_NO"); 
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = BREAK_SUMMARY_TABLE.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
                    strQry.AppendLine(" ON BREAK_SUMMARY_TABLE.COMPANY_NO = PC.COMPANY_NO");
                    strQry.AppendLine(" AND BREAK_SUMMARY_TABLE.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND BREAK_SUMMARY_TABLE.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
                    
                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");
                    strQry.AppendLine(" ON BREAK_SUMMARY_TABLE.DAY_DATE = D.DAY_DATE");

                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_CURRENT PHC ");
                    strQry.AppendLine(" ON BREAK_SUMMARY_TABLE.COMPANY_NO = PHC.COMPANY_NO");
                    strQry.AppendLine(" AND D.DAY_DATE = PHC.PUBLIC_HOLIDAY_DATE");
                    strQry.AppendLine(" AND PC.PAY_PUBLIC_HOLIDAY_IND = 'Y'");

                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
                    strQry.AppendLine(" ON BREAK_SUMMARY_TABLE.COMPANY_NO = ETC.COMPANY_NO");
                    strQry.AppendLine(" AND BREAK_SUMMARY_TABLE.EMPLOYEE_NO = ETC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND BREAK_SUMMARY_TABLE.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND BREAK_SUMMARY_TABLE.DAY_DATE = ETC.TIMESHEET_DATE"); 
                 
                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL  ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
                    strQry.AppendLine(" AND ETC.TIMESHEET_DATE IS NULL ");
                    
                    //goto Get_Form_Records_DayTotal_Continue;

                    //Where a Timesheet Exists (With or Without a Break Record)
                    //Where a Timesheet Exists (With or Without a Break Record)
                    //Where a Timesheet Exists (With or Without a Break Record)
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

                    //strQry.AppendLine(" WHEN (PC2.EXCEPTION_SUN_BELOW_MINUTES = 0 ");
                    //strQry.AppendLine(" OR PC2.EXCEPTION_MON_BELOW_MINUTES = 0 ");
                    //strQry.AppendLine(" OR PC2.EXCEPTION_TUE_BELOW_MINUTES = 0 ");
                    //strQry.AppendLine(" OR PC2.EXCEPTION_WED_BELOW_MINUTES = 0 ");
                    //strQry.AppendLine(" OR PC2.EXCEPTION_THU_BELOW_MINUTES = 0 ");
                    //strQry.AppendLine(" OR PC2.EXCEPTION_FRI_BELOW_MINUTES = 0 ");
                    //strQry.AppendLine(" OR PC2.EXCEPTION_SAT_BELOW_MINUTES = 0) ");
                    //strQry.AppendLine(" AND TEMP2_TABLE.DAY_PAID_MINUTES = 0 ");

                    //strQry.AppendLine(" THEN 'E'");

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

                    strQry.AppendLine(" ELSE ''");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(",TEMP2_TABLE.BREAK_ACCUM_MINUTES");
                    strQry.AppendLine(",TEMP2_TABLE.BREAK_INDICATOR");

                    strQry.AppendLine(",PAIDHOLIDAY_INDICATOR = ");

                    strQry.AppendLine(" CASE ");
                    
                    strQry.AppendLine(" WHEN NOT PHC.PUBLIC_HOLIDAY_DATE IS NULL ");
                    strQry.AppendLine(" THEN 'Y' ");

                    strQry.AppendLine(" ELSE '' ");

                    strQry.AppendLine(" END ");

                    //Errol - 2015-05-02
                    strQry.AppendLine(",TEMP2_TABLE.INCLUDED_IN_RUN_INDICATOR");

                    strQry.AppendLine(" FROM ");

                    strQry.AppendLine("(");

                    //Comment
                    strQry.AppendLine("--4 Start");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" TEMP1_TABLE.COMPANY_NO");
                    strQry.AppendLine(",TEMP1_TABLE.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",TEMP1_TABLE.PAY_CATEGORY_NO");
                    strQry.AppendLine(",TEMP1_TABLE.EMPLOYEE_NO ");
                    strQry.AppendLine(",TEMP1_TABLE.DAY_DATE");

                    strQry.AppendLine(",DAY_PAID_MINUTES = ");

                    strQry.AppendLine(" CASE ");

                    //Error or NO Rounding
                    strQry.AppendLine(" WHEN PC1.DAILY_ROUNDING_IND = 0");
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

                    strQry.AppendLine(",TEMP1_TABLE.BREAK_ACCUM_MINUTES");
                    strQry.AppendLine(",TEMP1_TABLE.BREAK_INDICATOR");
                    //Errol - 2015-05-02
                    strQry.AppendLine(",TEMP1_TABLE.INCLUDED_IN_RUN_INDICATOR");

                    strQry.AppendLine(" FROM ");

                    strQry.AppendLine("(");

                    //Comment
                    strQry.AppendLine("--3 Start");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" TIMESHEET_TOTAL_TABLE.COMPANY_NO");
                    strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.PAY_CATEGORY_NO");
                    strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.EMPLOYEE_NO ");
                    strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.DAY_DATE");
                   
                    strQry.AppendLine(",SUM(ISNULL(BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES,0)) AS BREAK_ACCUM_MINUTES ");

                    strQry.AppendLine(",DAY_PAID_MINUTES = ");
                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN SUM(BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES) > TIMESHEET_TOTAL_TABLE.BREAK_MINUTES ");
                    strQry.AppendLine(" AND SUM(BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES) < TIMESHEET_TOTAL_TABLE.TIMESHEET_ACCUM_MINUTES ");

                    strQry.AppendLine(" THEN TIMESHEET_TOTAL_TABLE.TIMESHEET_ACCUM_MINUTES - SUM(BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES)");

                    strQry.AppendLine(" WHEN NOT TIMESHEET_TOTAL_TABLE.TIMESHEET_ACCUM_MINUTES IS NULL AND NOT TIMESHEET_TOTAL_TABLE.BREAK_MINUTES IS NULL");

                    strQry.AppendLine(" THEN TIMESHEET_TOTAL_TABLE.TIMESHEET_ACCUM_MINUTES - TIMESHEET_TOTAL_TABLE.BREAK_MINUTES");

                    strQry.AppendLine(" ELSE 0");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(",BREAK_INDICATOR = ");

                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN SUM(BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES) > TIMESHEET_TOTAL_TABLE.BREAK_MINUTES ");
                    strQry.AppendLine(" THEN 'Y'");

                    strQry.AppendLine(" ELSE ''");

                    strQry.AppendLine(" END ");

                    //Errol - 2015-05-02
                    strQry.AppendLine(",INCLUDED_IN_RUN_INDICATOR = ");

                    strQry.AppendLine(" CASE ");

                    //BREAK_SUMMARY_TABLE IS USED IN LEFT JOIN
                    strQry.AppendLine(" WHEN NOT BREAK_SUMMARY_TABLE.COMPANY_NO IS NULL AND MIN(BREAK_SUMMARY_TABLE.BREAK_INCLUDED_IN_RUN_IND) < TIMESHEET_TOTAL_TABLE.TIMESHEET_INCLUDED_IN_RUN_IND");
                    strQry.AppendLine(" THEN MIN(ISNULL(BREAK_SUMMARY_TABLE.BREAK_INCLUDED_IN_RUN_IND,'N'))");

                    strQry.AppendLine(" ELSE ");
                    strQry.AppendLine(" TIMESHEET_TOTAL_TABLE.TIMESHEET_INCLUDED_IN_RUN_IND");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(" FROM ");

                    strQry.AppendLine("(");

                    //Comment
                    strQry.AppendLine("--2 Start");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" TIMESHEET_SUMMARY_TABLE.COMPANY_NO");
                    strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_NO");
                    strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.EMPLOYEE_NO ");
                    strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.DAY_DATE");

                    //Errol - 2015-05-02
                    strQry.AppendLine(",SUM(TIMESHEET_SUMMARY_TABLE.TIMESHEET_ACCUM_MINUTES) AS TIMESHEET_ACCUM_MINUTES");
                    //Errol - 2015-05-02
                    strQry.AppendLine(",MIN(TIMESHEET_SUMMARY_TABLE.TIMESHEET_INCLUDED_IN_RUN_IND) AS TIMESHEET_INCLUDED_IN_RUN_IND ");
                    strQry.AppendLine(",MAX(ISNULL(PCB.BREAK_MINUTES,0)) AS BREAK_MINUTES ");

                    strQry.AppendLine(" FROM ");

                    strQry.AppendLine("(");

                    //Comment
                    strQry.AppendLine("--1 Start");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" ETC.COMPANY_NO");
                    strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",ETC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",ETC.EMPLOYEE_NO ");
                    strQry.AppendLine(",ETC.TIMESHEET_DATE AS DAY_DATE");

                    //Errol - 2015-05-02
                    strQry.AppendLine(",TIMESHEET_ACCUM_MINUTES = ");

                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN ETC.INCLUDED_IN_RUN_IND = 'Y' THEN "); 
                    strQry.AppendLine(" SUM(ETC.TIMESHEET_TIME_OUT_MINUTES - ETC.TIMESHEET_TIME_IN_MINUTES) ");

                    strQry.AppendLine(" END ");
                    
                    //Errol - 2015-05-02
                    strQry.AppendLine(",MIN(ISNULL(ETC.INCLUDED_IN_RUN_IND,'N')) AS TIMESHEET_INCLUDED_IN_RUN_IND"); 

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

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
                    strQry.AppendLine(" AND ETC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");

                    //2017-07-29
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO ");
                    
                    strQry.AppendLine(" AND ETC.TIMESHEET_DATE > ");

                    strQry.AppendLine(" CASE ");

                    //Errol - 2015-02-17
                    strQry.AppendLine(" WHEN E.FIRST_RUN_COMPLETED_IND = 'Y'");

                    strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE ");

                    strQry.AppendLine(" ELSE DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" END  ");

                    strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= '" + Convert.ToDateTime(DataSet.Tables["PayCategory"].Rows[0]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" ETC.COMPANY_NO");
                    strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",ETC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",ETC.EMPLOYEE_NO ");
                    strQry.AppendLine(",ETC.TIMESHEET_DATE");
                    strQry.AppendLine(",ETC.INCLUDED_IN_RUN_IND");

                    //Comment
                    strQry.AppendLine("--1 End");

                    strQry.AppendLine(" ) AS TIMESHEET_SUMMARY_TABLE");

                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK_CURRENT PCB");

                    strQry.AppendLine(" ON TIMESHEET_SUMMARY_TABLE.COMPANY_NO = PCB.COMPANY_NO");
                    strQry.AppendLine(" AND TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_NO = PCB.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND PCB.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
                    strQry.AppendLine(" AND TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_TYPE = PCB.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND TIMESHEET_SUMMARY_TABLE.TIMESHEET_ACCUM_MINUTES >= PCB.WORKED_TIME_MINUTES");

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" TIMESHEET_SUMMARY_TABLE.COMPANY_NO");
                    strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_NO");
                    strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.EMPLOYEE_NO ");
                    strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.DAY_DATE");
                   
                    //Comment
                    strQry.AppendLine("--2End");

                    strQry.AppendLine(" ) AS TIMESHEET_TOTAL_TABLE ");

                    strQry.AppendLine(" LEFT JOIN  ");

                    strQry.AppendLine("(");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EBC.COMPANY_NO");
                    strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EBC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EBC.EMPLOYEE_NO ");
                    strQry.AppendLine(",EBC.BREAK_DATE AS DAY_DATE");

                    strQry.AppendLine(",BREAK_ACCUM_MINUTES = ");

                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN EBC.INCLUDED_IN_RUN_IND = 'Y' THEN "); 

                    strQry.AppendLine(" SUM(EBC.BREAK_TIME_OUT_MINUTES - EBC.BREAK_TIME_IN_MINUTES) ");

                    strQry.AppendLine(" END ");

                    //Errol - 2015-05-02
                    strQry.AppendLine(",MIN(ISNULL(EBC.INCLUDED_IN_RUN_IND,'N')) AS BREAK_INCLUDED_IN_RUN_IND"); 

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

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
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EBC.EMPLOYEE_NO ");

                    //2017-07-29
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = EBC.PAY_CATEGORY_NO ");

                    strQry.AppendLine(" AND EBC.BREAK_DATE > ");

                    strQry.AppendLine(" CASE ");

                    //Errol - 2015-02-17
                    strQry.AppendLine(" WHEN E.FIRST_RUN_COMPLETED_IND = 'Y'");

                    strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE ");

                    strQry.AppendLine(" ELSE DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" END  ");

                    strQry.AppendLine(" AND EBC.BREAK_DATE <= '" + Convert.ToDateTime(DataSet.Tables["PayCategory"].Rows[0]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" EBC.COMPANY_NO");
                    strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EBC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EBC.EMPLOYEE_NO ");
                    strQry.AppendLine(",EBC.BREAK_DATE");
                    strQry.AppendLine(",EBC.INCLUDED_IN_RUN_IND"); 

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
                    strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.BREAK_MINUTES ");
                    //Errol - 2015-05-02
                    strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.TIMESHEET_INCLUDED_IN_RUN_IND ");
                    //Errol - 2015-05-02
                    strQry.AppendLine(",BREAK_SUMMARY_TABLE.COMPANY_NO");
                    
                    //Comment
                    strQry.AppendLine("--3End");

                    strQry.AppendLine(") AS TEMP1_TABLE ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC1");

                    strQry.AppendLine(" ON TEMP1_TABLE.COMPANY_NO = PC1.COMPANY_NO");
                    strQry.AppendLine(" AND TEMP1_TABLE.PAY_CATEGORY_NO = PC1.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND TEMP1_TABLE.PAY_CATEGORY_TYPE = PC1.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND PC1.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
                    strQry.AppendLine(" AND PC1.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);

                    //Comment
                    strQry.AppendLine("--4End");

                    strQry.AppendLine(" ) AS TEMP2_TABLE");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC2");

                    strQry.AppendLine(" ON TEMP2_TABLE.COMPANY_NO = PC2.COMPANY_NO");
                    strQry.AppendLine(" AND TEMP2_TABLE.PAY_CATEGORY_NO = PC2.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND TEMP2_TABLE.PAY_CATEGORY_TYPE = PC2.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND PC2.PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
                    strQry.AppendLine(" AND PC2.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");
                    strQry.AppendLine(" ON TEMP2_TABLE.DAY_DATE = D.DAY_DATE");

                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_CURRENT PHC ");
                    strQry.AppendLine(" ON PC2.COMPANY_NO = PHC.COMPANY_NO");
                    strQry.AppendLine(" AND D.DAY_DATE = PHC.PUBLIC_HOLIDAY_DATE");
                    strQry.AppendLine(" AND PC2.PAY_PUBLIC_HOLIDAY_IND = 'Y'");

                Get_Form_Records_DayTotal_Continue:
                    
                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "DayTotal", parInt64CompanyNo);
                }
            }

            Get_Form_Records_Continue:

            DataSet.Tables.Remove("Temp");
            DataSet.AcceptChanges();

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
    }
}
