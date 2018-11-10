using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busRptTimeSheetTotals
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busRptTimeSheetTotals()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo, string parstrFromProgram, Int64 parint64CurrentUserNo, string parstrCurrentUserAccess)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PCPH.COMPANY_NO ");
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
            strQry.AppendLine(" AND PCPH.RUN_TYPE = 'P'");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = 'W'");
            }

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PCPH.COMPANY_NO ");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PCPH.PAY_PERIOD_DATE");
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PCPH.COMPANY_NO ");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PCPH.PAY_PERIOD_DATE DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayPeriodDate", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" E.COMPANY_NO ");
            strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");
            strQry.AppendLine(" ON E.COMPANY_NO = EPCH.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCH.EMPLOYEE_NO");

            //2017-01-28 (Allow Employee to Change PAY_CATEGORY_TYPE) 
            //strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCH.PAY_CATEGORY_TYPE");

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

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");
            strQry.AppendLine(" ON E.COMPANY_NO = PCPH.COMPANY_NO");
            strQry.AppendLine(" AND EPCH.PAY_PERIOD_DATE = PCPH.PAY_PERIOD_DATE");
            strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPCH.RUN_TYPE = PCPH.RUN_TYPE");
            strQry.AppendLine(" AND EPCH.RUN_NO = PCPH.RUN_NO");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = 'W'");
            }

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EPCH.COMPANY_NO");
            strQry.AppendLine(",EPCH.PAY_PERIOD_DATE");
            strQry.AppendLine(",EPCH.EMPLOYEE_NO");
            strQry.AppendLine(",EPCH.PAY_CATEGORY_NO");
            strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH ");

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

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");
            strQry.AppendLine(" ON EPCH.COMPANY_NO = PCPH.COMPANY_NO");
            strQry.AppendLine(" AND EPCH.PAY_PERIOD_DATE = PCPH.PAY_PERIOD_DATE");
            strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPCH.RUN_TYPE = PCPH.RUN_TYPE");
            strQry.AppendLine(" AND EPCH.RUN_NO = PCPH.RUN_NO");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = 'W'");
            }

            strQry.AppendLine(" WHERE EPCH.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EPCH.RUN_TYPE = 'P'");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeePayCategory", parint64CompanyNo);
            
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PCPH.COMPANY_NO");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

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

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON PCPH.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" WHERE PCPH.COMPANY_NO = " + parint64CompanyNo);

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = 'W'");
            }

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

        public byte[] Print_Report(Int64 parint64CompanyNo, string parstrPayrollType, string parstrReportType, string parstrEmployeeNoIN, string parstrPayCategoryNoIN,
                                   string parstrPayPeriodDate, string parstrFromProgram, string strReportOrder, string strGroupCostCentreInd, Int64 parint64CurrentUserNo, string parstrCurrentUserAccess)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" C.COMPANY_DESC");
            strQry.AppendLine(",CL.DATE_FORMAT");
            strQry.AppendLine(",'' AS REPORT_DATETIME");

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
            strQry.AppendLine(" TEMP_TABLE.EMPLOYEE_NO");

            if (strGroupCostCentreInd == "Y")
            {
                strQry.AppendLine(",TEMP_TABLE.PAY_CATEGORY_DESC");
                strQry.AppendLine(",TEMP_TABLE.PAY_CATEGORY_NO");
            }
            else
            {
                strQry.AppendLine(",0 AS PAY_CATEGORY_NO");
            }

            if (strReportOrder == "E")
            {
                strQry.AppendLine(",TEMP_TABLE.EMPLOYEE_CODE");
            }
            else
            {
                if (strReportOrder == "N")
                {
                    strQry.AppendLine(",TEMP_TABLE.EMPLOYEE_NAME");
                }
                else
                {
                    //Surname
                    strQry.AppendLine(",TEMP_TABLE.EMPLOYEE_SURNAME");
                }
            }

            strQry.AppendLine(",SUM(TEMP_TABLE.NT_HOURS_DECIMAL) AS NT_HOURS_DECIMAL");
            strQry.AppendLine(",SUM(TEMP_TABLE.OT1_HOURS_DECIMAL) AS OT1_HOURS_DECIMAL");
            strQry.AppendLine(",SUM(TEMP_TABLE.OT2_HOURS_DECIMAL) AS OT2_HOURS_DECIMAL");
            strQry.AppendLine(",SUM(TEMP_TABLE.OT3_HOURS_DECIMAL) AS OT3_HOURS_DECIMAL");
            strQry.AppendLine(",SUM(TEMP_TABLE.PH_HOURS_DECIMAL) AS PH_HOURS_DECIMAL");

            strQry.AppendLine(" FROM ");

            strQry.AppendLine("(SELECT ");
            
            strQry.AppendLine(" EEH.EMPLOYEE_NO");

            if (strGroupCostCentreInd == "Y")
            {
                strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
                strQry.AppendLine(",EEH.PAY_CATEGORY_NO");
            }

            if (strReportOrder == "E")
            {
                strQry.AppendLine(",E.EMPLOYEE_CODE");
            }
            else
            {
                if (strReportOrder == "N")
                {
                    strQry.AppendLine(",E.EMPLOYEE_NAME");
                }
                else
                {
                    //Surname
                    strQry.AppendLine(",E.EMPLOYEE_SURNAME");
                }
            }

            strQry.AppendLine(",NT_HOURS_DECIMAL = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN EEH.EARNING_NO IN (1,2) ");

            strQry.AppendLine(" THEN SUM(EEH.HOURS_DECIMAL)");

            strQry.AppendLine(" ELSE 0");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",OT1_HOURS_DECIMAL = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN EEH.EARNING_NO = 3 ");

            strQry.AppendLine(" THEN SUM(EEH.HOURS_DECIMAL)");

            strQry.AppendLine(" ELSE 0");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",OT2_HOURS_DECIMAL = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN EEH.EARNING_NO = 4 ");

            strQry.AppendLine(" THEN SUM(EEH.HOURS_DECIMAL)");

            strQry.AppendLine(" ELSE 0");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",OT3_HOURS_DECIMAL = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN EEH.EARNING_NO = 5 ");

            strQry.AppendLine(" THEN SUM(EEH.HOURS_DECIMAL)");

            strQry.AppendLine(" ELSE 0");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",PH_HOURS_DECIMAL = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN EEH.EARNING_NO = 9 ");

            strQry.AppendLine(" THEN SUM(EEH.HOURS_DECIMAL)");

            strQry.AppendLine(" ELSE 0");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
           
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON EEH.COMPANY_NO = E.COMPANY_NO");
            strQry.AppendLine(" AND EEH.EMPLOYEE_NO = E.EMPLOYEE_NO");
            //2017-01-28 (Allow Employee to Change PAY_CATEGORY_TYPE) 
            //strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
           
            if (strGroupCostCentreInd == "Y")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
                strQry.AppendLine(" ON EEH.COMPANY_NO = PC.COMPANY_NO");
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
            }

            //2017-07-29 - Fix From Here
            strQry.AppendLine(" INNER JOIN ");

            strQry.AppendLine("( SELECT ");

            strQry.AppendLine(" TH.COMPANY_NO");
            strQry.AppendLine(",TH.EMPLOYEE_NO");
            strQry.AppendLine(",TH.PAY_CATEGORY_NO");

            if (parstrPayrollType == "T")
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_HISTORY TH");
            }
            else
            {
                if (parstrPayrollType == "W")
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_HISTORY TH");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_HISTORY TH");
                }
            }

            strQry.AppendLine(" WHERE TH.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND TH.PAY_PERIOD_DATE = '" + parstrPayPeriodDate + "'");
            
            if (parstrEmployeeNoIN != ""
            | parstrPayCategoryNoIN != "")
            {
                if (parstrEmployeeNoIN != ""
                & parstrPayCategoryNoIN != "")
                {
                    strQry.AppendLine(" AND (TH.EMPLOYEE_NO IN " + parstrEmployeeNoIN);
                    strQry.AppendLine(" OR TH.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN + ")");
                }
                else
                {
                    if (parstrEmployeeNoIN != "")
                    {
                        strQry.AppendLine(" AND TH.EMPLOYEE_NO IN " + parstrEmployeeNoIN);
                    }

                    if (parstrPayCategoryNoIN != "")
                    {
                        strQry.AppendLine(" AND TH.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
                    }
                }
            }

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" TH.COMPANY_NO");
            strQry.AppendLine(",TH.EMPLOYEE_NO");
            strQry.AppendLine(",TH.PAY_CATEGORY_NO) AS TIMESHEET_HISTORY");

            strQry.AppendLine(" ON EEH.COMPANY_NO = TIMESHEET_HISTORY.COMPANY_NO");
            strQry.AppendLine(" AND EEH.EMPLOYEE_NO = TIMESHEET_HISTORY.EMPLOYEE_NO");
            strQry.AppendLine(" AND EEH.PAY_CATEGORY_NO = TIMESHEET_HISTORY.PAY_CATEGORY_NO");
            //2017-07-29 - Fix Up to Here

            //2013-08-30
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND EEH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EEH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE EEH.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EEH.PAY_PERIOD_DATE = '" + parstrPayPeriodDate + "'");

            strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = '"+ parstrPayrollType + "'");

            strQry.AppendLine(" AND EEH.EARNING_NO IN (1,2,3,4,5,9)");
            strQry.AppendLine(" AND EEH.RUN_TYPE = 'P'");

            if (parstrEmployeeNoIN != ""
            | parstrPayCategoryNoIN != "")
            {
                if (parstrEmployeeNoIN != ""
                & parstrPayCategoryNoIN != "")
                {
                    strQry.AppendLine(" AND (EEH.EMPLOYEE_NO IN " + parstrEmployeeNoIN);
                    strQry.AppendLine(" OR EEH.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN + ")");
                }
                else
                {
                    if (parstrEmployeeNoIN != "")
                    {
                        strQry.AppendLine(" AND EEH.EMPLOYEE_NO IN " + parstrEmployeeNoIN);
                    }

                    if (parstrPayCategoryNoIN != "")
                    {
                        strQry.AppendLine(" AND EEH.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
                    }
                }
            }

            strQry.AppendLine(" GROUP BY");
            strQry.AppendLine(" EEH.EMPLOYEE_NO");

            if (strGroupCostCentreInd == "Y")
            {
                strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
                strQry.AppendLine(",EEH.PAY_CATEGORY_NO");
            }

            if (strReportOrder == "E")
            {
                strQry.AppendLine(",E.EMPLOYEE_CODE");
            }
            else
            {
                if (strReportOrder == "N")
                {
                    strQry.AppendLine(",E.EMPLOYEE_NAME");
                }
                else
                {
                    //Surname
                    strQry.AppendLine(",E.EMPLOYEE_SURNAME");
                }
            }

            strQry.AppendLine(",EEH.EARNING_NO) AS TEMP_TABLE");

            strQry.AppendLine(" GROUP BY");
            strQry.AppendLine(" TEMP_TABLE.EMPLOYEE_NO");

            if (strGroupCostCentreInd == "Y")
            {
                strQry.AppendLine(",TEMP_TABLE.PAY_CATEGORY_DESC");
                strQry.AppendLine(",TEMP_TABLE.PAY_CATEGORY_NO");
            }

            if (strReportOrder == "E")
            {
                strQry.AppendLine(",TEMP_TABLE.EMPLOYEE_CODE");
            }
            else
            {
                if (strReportOrder == "N")
                {
                    strQry.AppendLine(",TEMP_TABLE.EMPLOYEE_NAME");
                }
                else
                {
                    //Surname
                    strQry.AppendLine(",TEMP_TABLE.EMPLOYEE_SURNAME");
                }
            }

            strQry.AppendLine(" HAVING SUM(TEMP_TABLE.NT_HOURS_DECIMAL) <> 0");
            strQry.AppendLine(" OR SUM(TEMP_TABLE.OT1_HOURS_DECIMAL)  <> 0");
            strQry.AppendLine(" OR SUM(TEMP_TABLE.OT2_HOURS_DECIMAL)  <> 0");
            strQry.AppendLine(" OR SUM(TEMP_TABLE.OT3_HOURS_DECIMAL)  <> 0");
            strQry.AppendLine(" OR SUM(TEMP_TABLE.PH_HOURS_DECIMAL)  <> 0");

            if (strGroupCostCentreInd == "Y")
            {
                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" TEMP_TABLE.PAY_CATEGORY_DESC");

                if (strReportOrder == "E")
                {
                    strQry.AppendLine(",TEMP_TABLE.EMPLOYEE_CODE");
                }
                else
                {
                    if (strReportOrder == "N")
                    {
                        strQry.AppendLine(",TEMP_TABLE.EMPLOYEE_NAME");
                    }
                    else
                    {
                        //Surname
                        strQry.AppendLine(",TEMP_TABLE.EMPLOYEE_SURNAME");
                    }
                }
            }
            else
            {
                strQry.AppendLine(" ORDER BY ");

                if (strReportOrder == "E")
                {
                    strQry.AppendLine(" TEMP_TABLE.EMPLOYEE_CODE");
                }
                else
                {
                    if (strReportOrder == "N")
                    {
                        strQry.AppendLine(" TEMP_TABLE.EMPLOYEE_NAME");
                    }
                    else
                    {
                        //Surname
                        strQry.AppendLine(" TEMP_TABLE.EMPLOYEE_SURNAME");
                    }
                }
            }

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Report", parint64CompanyNo);

            if (strGroupCostCentreInd == "Y")
            {
                DataSet.Tables["Report"].Columns.Remove("PAY_CATEGORY_DESC");
            }

            if (strReportOrder == "E")
            {
                DataSet.Tables["Report"].Columns.Remove("EMPLOYEE_CODE");
            }
            else
            {
                if (strReportOrder == "N")
                {
                    DataSet.Tables["Report"].Columns.Remove("EMPLOYEE_NAME");
                }
                else
                {
                    //Surname
                    DataSet.Tables["Report"].Columns.Remove("EMPLOYEE_SURNAME");
                }
            }

            DataSet.Tables["Report"].AcceptChanges();
            
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
    }
}
