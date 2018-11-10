using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Reporting.WinForms;
using System.IO;
using System.Net.Mail;
using System.Net;

namespace InteractPayroll
{
    public class busRptPaySlip
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        string pvtstrClassName = "busRptPaySlip";

        string pvtstrLogFileName = "";
        string pvtstrSmtpEmailAddressDescription = "";
        string pvtstrSmtpEmailAddress = "";
        string pvtstrSmtpEmailAddressPassword = "";
        string pvtstrEmailFolder = "";
        string pvtstrSmtpSysAdminEmailAddressFirstName = "";
        string pvtstrSmtpSysAdminEmailAddressLastName = "";
        string pvtstrSmtpSysAdminEmailAddress = "";
        string pvtstrSmtpHostName = "";
        int pvtintSmtpHostPort = 0;
                
        public busRptPaySlip()
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


            if (AppDomain.CurrentDomain.GetData("EmailFolder") != null)
            {
                pvtstrEmailFolder = AppDomain.CurrentDomain.GetData("EmailFolder").ToString();
            }

            if (AppDomain.CurrentDomain.GetData("SmtpSysAdminEmailAddressFirstName") != null)
            {
                pvtstrSmtpSysAdminEmailAddressFirstName = AppDomain.CurrentDomain.GetData("SmtpSysAdminEmailAddressFirstName").ToString();
            }

            if (AppDomain.CurrentDomain.GetData("SmtpSysAdminEmailAddressLastName") != null)
            {
                pvtstrSmtpSysAdminEmailAddressLastName = AppDomain.CurrentDomain.GetData("SmtpSysAdminEmailAddressLastName").ToString();
            }

            if (AppDomain.CurrentDomain.GetData("SmtpSysAdminEmailAddress") != null)
            {
                pvtstrSmtpSysAdminEmailAddress = AppDomain.CurrentDomain.GetData("SmtpSysAdminEmailAddress").ToString();
            }
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo, Int64 parint64CurrentUserNo,string parstrCurrentUserAccess )
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" PC.COMPANY_NO ");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_TYPE");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");
            
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON PCPH.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");

            //2013-08-29
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND PCPH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE PCPH.COMPANY_NO = " + parint64CompanyNo);

            //2013-06-21 Exclude T=Time Attendance
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE IN ('W','S')");

            strQry.AppendLine(" AND PCPH.RUN_TYPE = 'P'");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PC.COMPANY_NO ");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategory", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PCPH.COMPANY_NO ");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PCPH.PAY_PERIOD_DATE");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");

            //2013-08-29
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND PCPH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }
   
            strQry.AppendLine(" WHERE PCPH.COMPANY_NO = " + parint64CompanyNo);

            //2013-06-21 Exclude T=Time Attendance
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE IN ('W','S')");
            strQry.AppendLine(" AND PCPH.RUN_TYPE = 'P'");
            
            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PCPH.COMPANY_NO ");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PCPH.PAY_PERIOD_DATE");
           
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PCPH.COMPANY_NO ");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PCPH.PAY_PERIOD_DATE DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Date", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO");
            //2017-01-28 (Allow Employee to Change PAY_CATEGORY_TYPE)  
            //strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.EMPLOYEE_NAME");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            //2013-08-29
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND E.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            //2013-06-21 Exclude T=Time Attendance
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE IN ('W','S')");

            strQry.AppendLine(" ORDER BY ");
            //strQry.AppendLine(" E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" E.EMPLOYEE_NO");
           
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parint64CompanyNo);
                        
            //Empty
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EPCH.COMPANY_NO");
            strQry.AppendLine(",EPCH.PAY_CATEGORY_NO");
            strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPCH.PAY_PERIOD_DATE");
            strQry.AppendLine(",EPCH.EMPLOYEE_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");

            strQry.AppendLine(" WHERE EPCH.COMPANY_NO = -1 ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeePayCategoryDate", parint64CompanyNo);

            //Empty
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EPCH.COMPANY_NO");
            strQry.AppendLine(",EPCH.PAY_CATEGORY_NO");
            strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPCH.PAY_PERIOD_DATE");
          
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");

            strQry.AppendLine(" WHERE EPCH.COMPANY_NO = -1 ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategoryDate", parint64CompanyNo);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_Employees_For_CostCentre_For_Date(Int64 parint64CompanyNo, string parstrPayCategoryType, DateTime pardtWageDate, Int64 parint64CurrentUserNo, string parstrCurrentUserAccess)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EIH.COMPANY_NO");
            strQry.AppendLine(",EPCH.PAY_CATEGORY_NO");
            strQry.AppendLine(",EIH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EIH.PAY_PERIOD_DATE");
            strQry.AppendLine(",EIH.EMPLOYEE_NO");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY EIH");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");
            strQry.AppendLine(" ON EIH.COMPANY_NO = EPCH.COMPANY_NO");
            strQry.AppendLine(" AND EIH.PAY_PERIOD_DATE = EPCH.PAY_PERIOD_DATE");
            strQry.AppendLine(" AND EIH.EMPLOYEE_NO = EPCH.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EIH.PAY_CATEGORY_TYPE = EPCH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EIH.RUN_TYPE = EPCH.RUN_TYPE");

            //2013-08-29
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND EPCH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPCH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }
            
            strQry.AppendLine(" WHERE EIH.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EIH.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
            strQry.AppendLine(" AND EIH.RUN_TYPE = 'P'");
            strQry.AppendLine(" AND EIH.PAY_PERIOD_DATE = '" + pardtWageDate.ToString("yyyy-MM-dd") + "'");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeePayCategoryDate", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" EIH.COMPANY_NO");
            strQry.AppendLine(",EPCH.PAY_CATEGORY_NO");
            strQry.AppendLine(",EIH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EIH.PAY_PERIOD_DATE");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY EIH");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");
            strQry.AppendLine(" ON EIH.COMPANY_NO = EPCH.COMPANY_NO");
            strQry.AppendLine(" AND EIH.PAY_PERIOD_DATE = EPCH.PAY_PERIOD_DATE");
            strQry.AppendLine(" AND EIH.EMPLOYEE_NO = EPCH.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EIH.PAY_CATEGORY_TYPE = EPCH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EIH.RUN_TYPE = EPCH.RUN_TYPE");

            //2013-08-29
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND EPCH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPCH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE EIH.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EIH.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
            strQry.AppendLine(" AND EIH.RUN_TYPE = 'P'");
            strQry.AppendLine(" AND EIH.PAY_PERIOD_DATE = '" + pardtWageDate.ToString("yyyy-MM-dd") + "'");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategoryDate", parint64CompanyNo);
           
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Print_PaySlip(Int64 parint64CurrentUserNo,string parstrCurrentUserAccess, Int64 parint64CompanyNo, string parstrPayCategoryType, DateTime pardtPaySlipDate, string parstrEmployeeNoIN, string parstrPayCategoryNoIN, string parstrPrintOrderInd,string parstrLayerFrom,string parstrOption)
        {
            StringBuilder strQry = new StringBuilder();
            string strOverTimevalue = "";
            
            DataSet DataSet = new DataSet();
            DataView EarningDataView;
            
            DateTime dtBeginFinancialYear;
            DateTime dtStartLeaveTaxYear;
            
            if (pardtPaySlipDate.Month <= 2)
            {
                dtBeginFinancialYear = new DateTime(pardtPaySlipDate.Year - 1, 3, 1);
            }
            else
            {
                dtBeginFinancialYear = new DateTime(pardtPaySlipDate.Year, 3, 1);
            }

            string strWageDateWhere = " AND EDH.PAY_PERIOD_DATE = '" + pardtPaySlipDate.ToString("yyyy-MM-dd") + "'";

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" LEAVE_BEGIN_MONTH");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY E");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "LeaveStartDate", parint64CompanyNo);

            if (DataSet.Tables["LeaveStartDate"].Rows.Count > 0)
            {
                //Position Within Current Financial Year
                if (pardtPaySlipDate.Month >= Convert.ToInt32(DataSet.Tables["LeaveStartDate"].Rows[0]["LEAVE_BEGIN_MONTH"]))
                {
                    dtStartLeaveTaxYear = new DateTime(pardtPaySlipDate.Year, Convert.ToInt32(DataSet.Tables["LeaveStartDate"].Rows[0]["LEAVE_BEGIN_MONTH"]), 1);
                }
                else
                {
                    dtStartLeaveTaxYear = new DateTime(pardtPaySlipDate.Year - 1, Convert.ToInt32(DataSet.Tables["LeaveStartDate"].Rows[0]["LEAVE_BEGIN_MONTH"]), 1);
                }
            }
            else
            {
                dtStartLeaveTaxYear = dtBeginFinancialYear;
            }

            strQry.Clear();

            strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.PRINT_PAYSLIPS");
            strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo.ToString());

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EDH.EMPLOYEE_NO");
            strQry.AppendLine(",D.DEDUCTION_DESC");
            strQry.AppendLine(",D.DEDUCTION_NO");
            strQry.AppendLine(",SUM(EDH.TOTAL) AS TOTAL");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY EDH");
           
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.DEDUCTION D");
            strQry.AppendLine(" ON EDH.COMPANY_NO = D.COMPANY_NO");
            strQry.AppendLine(" AND EDH.PAY_CATEGORY_TYPE = D.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EDH.DEDUCTION_NO = D.DEDUCTION_NO");
            strQry.AppendLine(" AND EDH.DEDUCTION_SUB_ACCOUNT_NO = D.DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL ");

            //2013-08-29
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND EDH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EDH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EDH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            if (parstrPayCategoryNoIN != "")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");
                strQry.AppendLine(" ON EDH.COMPANY_NO = EPCH.COMPANY_NO");
                strQry.AppendLine(" AND EDH.PAY_PERIOD_DATE = EPCH.PAY_PERIOD_DATE");
                strQry.AppendLine(" AND EDH.EMPLOYEE_NO = EPCH.EMPLOYEE_NO");

                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO IN (" + parstrPayCategoryNoIN + ")");
                
                strQry.AppendLine(" AND EDH.PAY_CATEGORY_TYPE = EPCH.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EDH.RUN_TYPE = EPCH.RUN_TYPE");
                strQry.AppendLine(" AND EDH.RUN_NO = EPCH.RUN_NO");
            }
          
            strQry.AppendLine(" WHERE EDH.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EDH.RUN_TYPE = 'P'");
            
            strQry.AppendLine(strWageDateWhere);

            if (parstrEmployeeNoIN != "")
            {
                strQry.AppendLine(" AND EDH.EMPLOYEE_NO IN (" + parstrEmployeeNoIN + ")");
            }

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" EDH.EMPLOYEE_NO");
            strQry.AppendLine(",D.DEDUCTION_DESC");
            strQry.AppendLine(",D.DEDUCTION_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Deduction", parint64CompanyNo);

            //Earning
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EEH.EMPLOYEE_NO");
            strQry.AppendLine(",E.EARNING_DESC");
            strQry.AppendLine(",EEH.EARNING_NO");
            strQry.AppendLine(",E.LEAVE_PERCENTAGE");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",EPCH.HOURLY_RATE");
            strQry.AppendLine(",SUM(EEH.HOURS_DECIMAL) / COUNT(EEH.EARNING_NO) AS HOURS_DECIMAL");
            strQry.AppendLine(",SUM(EEH.TOTAL) / COUNT(EEH.EARNING_NO) AS TOTAL");
           
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");

            //2013-08-29
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND EEH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EEH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON EEH.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND EEH.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");
            strQry.AppendLine(" ON PC.COMPANY_NO = EPCH.COMPANY_NO");
            strQry.AppendLine(" AND EEH.PAY_PERIOD_DATE = EPCH.PAY_PERIOD_DATE");
            strQry.AppendLine(" AND EEH.EMPLOYEE_NO = EPCH.EMPLOYEE_NO");
            strQry.AppendLine(" AND EEH.RUN_TYPE = EPCH.RUN_TYPE");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPCH.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPCH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EEH.RUN_NO = EPCH.RUN_NO");

            if (parstrPayCategoryNoIN != "")
            {
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO IN (" + parstrPayCategoryNoIN + ")");
            }
           
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING E");
            strQry.AppendLine(" ON EEH.COMPANY_NO = E.COMPANY_NO");
            strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EEH.EARNING_NO = E.EARNING_NO");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");
       
            strQry.AppendLine(" WHERE EEH.COMPANY_NO = " + parint64CompanyNo);
            
            strQry.AppendLine(strWageDateWhere.Replace("EDH","EEH"));
            
            strQry.AppendLine(" AND EEH.RUN_TYPE = 'P'");

            strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));

            if (parstrEmployeeNoIN != "")
            {
                strQry.AppendLine(" AND EEH.EMPLOYEE_NO IN (" + parstrEmployeeNoIN + ")");
            }
            
            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" EEH.EMPLOYEE_NO");
            strQry.AppendLine(",E.EARNING_DESC");
            strQry.AppendLine(",EEH.EARNING_NO");
            strQry.AppendLine(",E.LEAVE_PERCENTAGE");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",EPCH.HOURLY_RATE");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Earning", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EIH.EMPLOYEE_NO");
            strQry.AppendLine(",PCH.PAIDHOLIDAY_RATE");

            strQry.AppendLine(",C.OVERTIME1_RATE");
            strQry.AppendLine(",C.OVERTIME2_RATE");
            strQry.AppendLine(",C.OVERTIME3_RATE");

            strQry.AppendLine(",PAYSLIP_IND = ");

            strQry.AppendLine(" CASE");

            strQry.AppendLine(" WHEN ISNULL(E.EMAIL_VIA_PAYSLIP_IND, 'N') = 'Y' THEN 'Y'");

            strQry.AppendLine(" ELSE 'N'");

            strQry.AppendLine(" END");
            
            strQry.AppendLine(",MAX(PCH.PAY_PERIOD_DATE) AS WAGE_DATE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCH");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY EIH");
            strQry.AppendLine(" ON PCH.COMPANY_NO = EIH.COMPANY_NO");
            strQry.AppendLine(" AND PCH.PAY_PERIOD_DATE = EIH.PAY_PERIOD_DATE");

            if (parstrEmployeeNoIN != "")
            {
                strQry.AppendLine(" AND EIH.EMPLOYEE_NO IN (" + parstrEmployeeNoIN + ")");
            }
            
            strQry.AppendLine(" AND PCH.PAY_CATEGORY_TYPE = EIH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PCH.RUN_TYPE = EIH.RUN_TYPE");

            //2013-08-29
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND EIH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EIH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND PCH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EIH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            if (parstrPayCategoryNoIN != "")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");
                strQry.AppendLine(" ON PCH.COMPANY_NO = EPCH.COMPANY_NO");
                strQry.AppendLine(" AND PCH.PAY_PERIOD_DATE = EPCH.PAY_PERIOD_DATE");
                strQry.AppendLine(" AND EIH.EMPLOYEE_NO = EPCH.EMPLOYEE_NO");

                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO IN (" + parstrPayCategoryNoIN + ")");

                strQry.AppendLine(" AND EIH.PAY_CATEGORY_TYPE = EPCH.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EIH.RUN_TYPE = EPCH.RUN_TYPE");
                strQry.AppendLine(" AND EIH.RUN_NO = EPCH.RUN_NO");
            }
           
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C");
            strQry.AppendLine(" ON PCH.COMPANY_NO = C.COMPANY_NO");
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

            if (parstrOption == "A")
            {
                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            }
            else
            {
                //Emailed / NOT Emailed
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            }

            strQry.AppendLine(" ON EIH.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND EIH.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            //2017-01-28 (Allow Employee to Change PAY_CATEGORY_TYPE)  
            //strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));

            if (parstrOption == "E")
            {
                //Emailed
                strQry.AppendLine(" AND E.EMAIL_VIA_PAYSLIP_IND = 'Y' ");
            }
            else
            {
                if (parstrOption == "N")
                {
                    //Not Emailed
                    strQry.AppendLine(" AND ISNULL(E.EMAIL_VIA_PAYSLIP_IND,'N') <> 'Y' ");
                }
            }

            strQry.AppendLine(" WHERE PCH.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PCH.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));

            strQry.AppendLine(strWageDateWhere.Replace("EDH", "PCH"));
            
            strQry.AppendLine(" AND PCH.RUN_TYPE = 'P'");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" EIH.EMPLOYEE_NO");
            strQry.AppendLine(",PCH.PAIDHOLIDAY_RATE");
            strQry.AppendLine(",C.OVERTIME1_RATE");
            strQry.AppendLine(",C.OVERTIME2_RATE");
            strQry.AppendLine(",C.OVERTIME3_RATE");
            strQry.AppendLine(",E.EMAIL_VIA_PAYSLIP_IND");
            strQry.AppendLine(" ORDER BY EIH.EMPLOYEE_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parint64CompanyNo);

            string strPayslipEmployeeNoIn = "";
            string strNoPayslipEmployeeNoIn = "";
            
            if (parstrLayerFrom == "C")
            {
                //2017-09-23
                strQry.Clear();

                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.PAYSLIP_EMAIL_LAST_PRINT ");
                strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                DataView EmployeePayslipDataView = new DataView(DataSet.Tables["Employee"],
                    "PAYSLIP_IND = 'Y' ",
                    "",
                    DataViewRowState.CurrentRows);
                
                if (EmployeePayslipDataView.Count > 0)
                {
                    for (int intRow = 0; intRow < EmployeePayslipDataView.Count; intRow++)
                    {
                        if (intRow == 0)
                        {
                            strPayslipEmployeeNoIn = EmployeePayslipDataView[intRow]["EMPLOYEE_NO"].ToString();
                        }
                        else
                        {
                            strPayslipEmployeeNoIn += "," + EmployeePayslipDataView[intRow]["EMPLOYEE_NO"].ToString();
                        }
                    }

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAYSLIP_EMAIL_LAST_PRINT ");
                    strQry.AppendLine("(USER_NO ");
                    strQry.AppendLine(",COMPANY_NO ");
                    strQry.AppendLine(",PAY_PERIOD_DATE ");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(",EMPLOYEE_NO_IN) ");
                    
                    strQry.AppendLine(" VALUES ");

                    strQry.AppendLine("(" + parint64CurrentUserNo);
                    strQry.AppendLine("," + parint64CompanyNo);
                    strQry.AppendLine(",'" + pardtPaySlipDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strPayslipEmployeeNoIn) + ")");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                }

                DataView EmployeeNoPayslipDataView = new DataView(DataSet.Tables["Employee"],
                  "PAYSLIP_IND = 'N' ",
                  "",
                  DataViewRowState.CurrentRows);

                if (EmployeeNoPayslipDataView.Count > 0)
                {
                    for (int intRow = 0; intRow < EmployeeNoPayslipDataView.Count; intRow++)
                    {
                        if (intRow == 0)
                        {
                            strNoPayslipEmployeeNoIn = EmployeeNoPayslipDataView[intRow]["EMPLOYEE_NO"].ToString();
                        }
                        else
                        {
                            strNoPayslipEmployeeNoIn += "," + EmployeeNoPayslipDataView[intRow]["EMPLOYEE_NO"].ToString();
                        }
                    }
                }
            }

            for (int intRow = 0; intRow < DataSet.Tables["Employee"].Rows.Count; intRow++)
            {
                EarningDataView = null;
                EarningDataView = new DataView(DataSet.Tables["Earning"],
                    "EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString(),
                    "EARNING_NO",
                    DataViewRowState.CurrentRows);

                int intEarningsSeqNo = 0;

                for (int intEarningsRow = 0; intEarningsRow < EarningDataView.Count; intEarningsRow++)
                {
#if(DEBUG)
                    int intEarningNo = Convert.ToInt32(EarningDataView[intEarningsRow]["EARNING_NO"]);

                    if (intEarningNo == 11)
                    {
                        string strStop = "";
                    }

#endif
                    if (Convert.ToInt32(EarningDataView[intEarningsRow]["EARNING_NO"]) == 1
                        | Convert.ToInt32(EarningDataView[intEarningsRow]["EARNING_NO"]) == 2
                        | Convert.ToInt32(EarningDataView[intEarningsRow]["EARNING_NO"]) == 7
                        | Convert.ToInt32(EarningDataView[intEarningsRow]["EARNING_NO"]) == 8)
                    {
                        if (Convert.ToDouble(EarningDataView[intEarningsRow]["TOTAL"]) > 0)
                        {
                            intEarningsSeqNo = intEarningsSeqNo + 1;

                            //Tested 2012-08-23
                            strQry.Clear();

                            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PRINT_PAYSLIPS");
                            strQry.AppendLine("(USER_NO");
                            strQry.AppendLine(",COMPANY_NO");
                            strQry.AppendLine(",EMPLOYEE_NO");
                            strQry.AppendLine(",SEQ_NO");
                            strQry.AppendLine(",EARNINGS_DESC");
                            strQry.AppendLine(",EARNINGS_HOURS");
                            strQry.AppendLine(",EARNINGS_AMOUNT)");
                            strQry.AppendLine(" VALUES");
                            strQry.AppendLine("(" + parint64CurrentUserNo.ToString());
                            strQry.AppendLine("," + parint64CompanyNo);
                            strQry.AppendLine("," + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine("," + intEarningsSeqNo);

                            if (Convert.ToInt32(EarningDataView[intEarningsRow]["EARNING_NO"]) == 2
                                |  Convert.ToInt32(EarningDataView[intEarningsRow]["EARNING_NO"]) == 8)
                            {
                                if (Convert.ToInt32(EarningDataView[intEarningsRow]["EARNING_NO"]) == 2)
                                {
                                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(EarningDataView[intEarningsRow]["PAY_CATEGORY_DESC"].ToString() + " - NT (R" + Convert.ToDouble(EarningDataView[intEarningsRow]["HOURLY_RATE"]).ToString("#####0.00") + "/Hour)"));
                                }
                                else
                                {
                                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(EarningDataView[intEarningsRow]["EARNING_DESC"].ToString() + " (R" + Convert.ToDouble(EarningDataView[intEarningsRow]["HOURLY_RATE"]).ToString("#####0.00") + "/Hour)"));
                                }
                            }
                            else
                            {
                                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(EarningDataView[intEarningsRow]["EARNING_DESC"].ToString()));
                            }

                            strQry.AppendLine("," + EarningDataView[intEarningsRow]["HOURS_DECIMAL"]);
                       
                            strQry.AppendLine("," + EarningDataView[intEarningsRow]["TOTAL"]+ ")");
                            
                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(EarningDataView[intEarningsRow]["EARNING_NO"]) == 3
                            | Convert.ToInt32(EarningDataView[intEarningsRow]["EARNING_NO"]) == 4
                            | Convert.ToInt32(EarningDataView[intEarningsRow]["EARNING_NO"]) == 5)
                        {
                            if (Convert.ToDouble(EarningDataView[intEarningsRow]["TOTAL"]) > 0)
                            {
                                intEarningsSeqNo = intEarningsSeqNo + 1;

                                //Tested 2012-08-23
                                strQry.Clear();

                                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PRINT_PAYSLIPS");
                                strQry.AppendLine("(USER_NO");
                                strQry.AppendLine(",COMPANY_NO");
                                strQry.AppendLine(",EMPLOYEE_NO");
                                strQry.AppendLine(",SEQ_NO");
                                strQry.AppendLine(",EARNINGS_DESC");
                                strQry.AppendLine(",EARNINGS_HOURS");
                                strQry.AppendLine(",EARNINGS_AMOUNT)");
                                strQry.AppendLine(" VALUES");
                                strQry.AppendLine("(" + parint64CurrentUserNo);
                                strQry.AppendLine("," + parint64CompanyNo);
                                strQry.AppendLine("," + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                                strQry.AppendLine("," + intEarningsSeqNo);

                                if (Convert.ToInt32(EarningDataView[intEarningsRow]["EARNING_NO"]) == 3)
                                {
                                    strOverTimevalue = " - OT1 (R" + Convert.ToDouble(Convert.ToDouble(EarningDataView[intEarningsRow]["HOURLY_RATE"]) * (Convert.ToDouble(DataSet.Tables["Employee"].Rows[intRow]["OVERTIME1_RATE"]))).ToString("####0.00") + "/Hour)";
                                }
                                else
                                {
                                    if (Convert.ToInt32(EarningDataView[intEarningsRow]["EARNING_NO"]) == 4)
                                    {
                                        strOverTimevalue = " - OT2 (R" + Convert.ToDouble(Convert.ToDouble(EarningDataView[intEarningsRow]["HOURLY_RATE"]) * (Convert.ToDouble(DataSet.Tables["Employee"].Rows[intRow]["OVERTIME2_RATE"]))).ToString("####0.00") + "/Hour)";
                                    }
                                    else
                                    {
                                        strOverTimevalue = " - OT3 (R" + Convert.ToDouble(Convert.ToDouble(EarningDataView[intEarningsRow]["HOURLY_RATE"]) * (Convert.ToDouble(DataSet.Tables["Employee"].Rows[intRow]["OVERTIME3_RATE"]))).ToString("####0.00") + "/Hour)";
                                    }
                                }

                                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(EarningDataView[intEarningsRow]["PAY_CATEGORY_DESC"].ToString() + strOverTimevalue));
                                strQry.AppendLine("," + EarningDataView[intEarningsRow]["HOURS_DECIMAL"]);
                       
                                strQry.AppendLine("," + EarningDataView[intEarningsRow]["TOTAL"] + ")");
                               
                                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                            }
                        }
                        else
                        {
                            if (Convert.ToInt32(EarningDataView[intEarningsRow]["EARNING_NO"]) == 4)
                            {
                                if (Convert.ToDouble(EarningDataView[intEarningsRow]["TOTAL"]) > 0)
                                {
                                    intEarningsSeqNo = intEarningsSeqNo + 1;

                                    strQry.Clear();

                                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PRINT_PAYSLIPS");
                                    strQry.AppendLine("(USER_NO");
                                    strQry.AppendLine(",COMPANY_NO");
                                    strQry.AppendLine(",EMPLOYEE_NO");
                                    strQry.AppendLine(",SEQ_NO");
                                    strQry.AppendLine(",EARNINGS_DESC");
                                    strQry.AppendLine(",EARNINGS_HOURS");
                                    strQry.AppendLine(",EARNINGS_AMOUNT)");
                                    strQry.AppendLine(" VALUES");
                                    strQry.AppendLine("(" + parint64CurrentUserNo);
                                    strQry.AppendLine("," + parint64CompanyNo);
                                    strQry.AppendLine("," + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                                    strQry.AppendLine("," + intEarningsSeqNo);
                                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(EarningDataView[intEarningsRow]["EARNING_DESC"].ToString()));
                                    strQry.AppendLine("," + EarningDataView[intEarningsRow]["HOURS_DECIMAL"]);
                                    
                                    strQry.AppendLine("," + EarningDataView[intEarningsRow]["TOTAL"] + ")");
                                    
                                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                                }
                            }
                            else
                            {
                                if (Convert.ToInt32(EarningDataView[intEarningsRow]["EARNING_NO"]) == 5)
                                {
                                    if (Convert.ToDouble(EarningDataView[intEarningsRow]["TOTAL"]) > 0)
                                    {
                                        intEarningsSeqNo = intEarningsSeqNo + 1;

                                        strQry.Clear();

                                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PRINT_PAYSLIPS");
                                        strQry.AppendLine("(USER_NO");
                                        strQry.AppendLine(",COMPANY_NO");
                                        strQry.AppendLine(",EMPLOYEE_NO");
                                        strQry.AppendLine(",SEQ_NO");
                                        strQry.AppendLine(",EARNINGS_DESC");
                                        strQry.AppendLine(",EARNINGS_HOURS");
                                        strQry.AppendLine(",EARNINGS_AMOUNT)");
                                        strQry.AppendLine(" VALUES");
                                        strQry.AppendLine("(" + parint64CurrentUserNo.ToString());
                                        strQry.AppendLine("," + parint64CompanyNo);
                                        strQry.AppendLine("," + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                                        strQry.AppendLine("," + intEarningsSeqNo);
                                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(EarningDataView[intEarningsRow]["EARNING_DESC"].ToString()));
                                        strQry.AppendLine("," + EarningDataView[intEarningsRow]["HOURS_DECIMAL"]);
                                       
                                        strQry.AppendLine("," + EarningDataView[intEarningsRow]["TOTAL"] + ")");
                                       
                                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                                    }
                                }
                                else
                                {
                                    if (Convert.ToInt32(EarningDataView[intEarningsRow]["EARNING_NO"]) == 6)
                                    {
                                        if (Convert.ToDouble(EarningDataView[intEarningsRow]["TOTAL"]) > 0)
                                        {
                                            intEarningsSeqNo = intEarningsSeqNo + 1;

                                            strQry.Clear();

                                            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PRINT_PAYSLIPS");
                                            strQry.AppendLine("(USER_NO");
                                            strQry.AppendLine(",COMPANY_NO");
                                            strQry.AppendLine(",EMPLOYEE_NO");
                                            strQry.AppendLine(",SEQ_NO");
                                            strQry.AppendLine(",EARNINGS_DESC");
                                            strQry.AppendLine(",EARNINGS_HOURS");
                                            strQry.AppendLine(",EARNINGS_AMOUNT)");
                                            strQry.AppendLine(" VALUES");
                                            strQry.AppendLine("(" + parint64CurrentUserNo.ToString());
                                            strQry.AppendLine("," + parint64CompanyNo);
                                            strQry.AppendLine("," + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                                            strQry.AppendLine("," + intEarningsSeqNo);
                                            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(EarningDataView[intEarningsRow]["EARNING_DESC"].ToString()));
                                            strQry.AppendLine("," + EarningDataView[intEarningsRow]["HOURS_DECIMAL"]);
                                            strQry.AppendLine("," + EarningDataView[intEarningsRow]["TOTAL"] + ")");
                                           
                                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                                        }
                                    }
                                    else
                                    {
                                        if (Convert.ToInt32(EarningDataView[intEarningsRow]["EARNING_NO"]) == 9)
                                        {
                                            if (Convert.ToDouble(EarningDataView[intEarningsRow]["TOTAL"]) > 0)
                                            //| parintPayCategoryNo == 0)
                                            {
                                                intEarningsSeqNo = intEarningsSeqNo + 1;

                                                //Tested 2012-08-23
                                                strQry.Clear();

                                                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PRINT_PAYSLIPS");
                                                strQry.AppendLine("(USER_NO");
                                                strQry.AppendLine(",COMPANY_NO");
                                                strQry.AppendLine(",EMPLOYEE_NO");
                                                strQry.AppendLine(",SEQ_NO");
                                                strQry.AppendLine(",EARNINGS_DESC");
                                                strQry.AppendLine(",EARNINGS_HOURS");
                                                strQry.AppendLine(",EARNINGS_AMOUNT)");
                                                strQry.AppendLine(" VALUES");
                                                strQry.AppendLine("(" + parint64CurrentUserNo.ToString());
                                                strQry.AppendLine("," + parint64CompanyNo);
                                                strQry.AppendLine("," + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                                                strQry.AppendLine("," + intEarningsSeqNo);

                                                if (Convert.ToDouble(DataSet.Tables["Employee"].Rows[intRow]["PAIDHOLIDAY_RATE"]) > 0)
                                                {
                                                    strQry.AppendLine(",'" + EarningDataView[intEarningsRow]["EARNING_DESC"].ToString() + " (R" + Convert.ToDouble(Convert.ToDouble(EarningDataView[intEarningsRow]["HOURLY_RATE"]) * (Convert.ToDouble(DataSet.Tables["Employee"].Rows[intRow]["PAIDHOLIDAY_RATE"]))).ToString("####0.00") + "/Hour)'");

                                                }
                                                else
                                                {
                                                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(EarningDataView[intEarningsRow]["EARNING_DESC"].ToString()));
                                                }

                                                strQry.AppendLine("," + EarningDataView[intEarningsRow]["HOURS_DECIMAL"]);
                                                strQry.AppendLine("," + EarningDataView[intEarningsRow]["TOTAL"] + ")");
                                                
                                                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                                            }
                                        }
                                        else
                                        {
                                            //ELR 2014-05-24
                                            if (Convert.ToInt32(EarningDataView[intEarningsRow]["EARNING_NO"]) < 200)
                                            {
                                                if (Convert.ToDouble(EarningDataView[intEarningsRow]["TOTAL"]) > 0)
                                                {
                                                    intEarningsSeqNo = intEarningsSeqNo + 1;

                                                    strQry.Clear();

                                                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PRINT_PAYSLIPS");
                                                    strQry.AppendLine("(USER_NO");
                                                    strQry.AppendLine(",COMPANY_NO");
                                                    strQry.AppendLine(",EMPLOYEE_NO");
                                                    strQry.AppendLine(",SEQ_NO");
                                                    strQry.AppendLine(",EARNINGS_DESC");
                                                    strQry.AppendLine(",EARNINGS_HOURS");
                                                    strQry.AppendLine(",EARNINGS_AMOUNT)");
                                                    strQry.AppendLine(" VALUES");
                                                    strQry.AppendLine("(" + parint64CurrentUserNo.ToString());
                                                    strQry.AppendLine("," + parint64CompanyNo);
                                                    strQry.AppendLine("," + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                                                    strQry.AppendLine("," + intEarningsSeqNo);
                                                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(EarningDataView[intEarningsRow]["EARNING_DESC"].ToString()));
                                                    strQry.AppendLine("," + EarningDataView[intEarningsRow]["HOURS_DECIMAL"]);
                                                    strQry.AppendLine("," + EarningDataView[intEarningsRow]["TOTAL"] + ")");

                                                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                                                }
                                            }
                                            else
                                            {
                                                //Leave
                                                if (Convert.ToDouble(EarningDataView[intEarningsRow]["TOTAL"]) > 0)
                                                {
                                                    intEarningsSeqNo = intEarningsSeqNo + 1;

                                                    //Tested 2012-08-23
                                                    strQry.Clear();

                                                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PRINT_PAYSLIPS");
                                                    strQry.AppendLine("(USER_NO");
                                                    strQry.AppendLine(",COMPANY_NO");
                                                    strQry.AppendLine(",EMPLOYEE_NO");
                                                    strQry.AppendLine(",SEQ_NO");
                                                    strQry.AppendLine(",EARNINGS_DESC");
                                                    strQry.AppendLine(",EARNINGS_HOURS");
                                                    strQry.AppendLine(",EARNINGS_AMOUNT)");
                                                    strQry.AppendLine(" VALUES");
                                                    strQry.AppendLine("(" + parint64CurrentUserNo.ToString());
                                                    strQry.AppendLine("," + parint64CompanyNo);
                                                    strQry.AppendLine("," + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                                                    strQry.AppendLine("," + intEarningsSeqNo);

                                                    if (Convert.ToDouble(EarningDataView[intEarningsRow]["LEAVE_PERCENTAGE"]) != 100)
                                                    {
                                                        strQry.AppendLine(",'" + EarningDataView[intEarningsRow]["EARNING_DESC"].ToString() + " (" + Convert.ToDouble(EarningDataView[intEarningsRow]["LEAVE_PERCENTAGE"]).ToString("##0.00") + "%)'");
                                                    }
                                                    else
                                                    {
                                                        if (Convert.ToDouble(EarningDataView[intEarningsRow]["HOURS_DECIMAL"]) != 0)
                                                        {
                                                            strOverTimevalue = " (R" + Convert.ToDouble(EarningDataView[intEarningsRow]["HOURLY_RATE"]).ToString("####0.00") + "/Hour)";
                                                        }
                                                        else
                                                        {
                                                            strOverTimevalue = "";
                                                        }

                                                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(EarningDataView[intEarningsRow]["EARNING_DESC"].ToString() + strOverTimevalue));
                                                    }

                                                    strQry.AppendLine("," + EarningDataView[intEarningsRow]["HOURS_DECIMAL"]);
                                                    strQry.AppendLine("," + EarningDataView[intEarningsRow]["TOTAL"] + ")");

                                                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Insert_Deductions(ref DataSet, parint64CompanyNo, parint64CurrentUserNo, intEarningsSeqNo, intRow);
            }

            DataSet.Dispose();
            DataSet = null;
            DataSet = new DataSet();

            if (parstrLayerFrom == "C")
            {
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" E.EMPLOYEE_CODE");
                strQry.AppendLine(",E.EMPLOYEE_NAME");
                strQry.AppendLine(",E.EMPLOYEE_SURNAME");
                strQry.AppendLine(",E.EMPLOYEE_EMAIL");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));

                if (strPayslipEmployeeNoIn != "")
                {
                    strQry.AppendLine(" AND E.EMPLOYEE_NO IN (" + strPayslipEmployeeNoIn + ")");
                }
                else
                {
                    strQry.AppendLine(" AND E.EMPLOYEE_NO IN (-1)");
                }

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeePayslip", parint64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" E.EMPLOYEE_CODE");
                strQry.AppendLine(",E.EMPLOYEE_NAME");
                strQry.AppendLine(",E.EMPLOYEE_SURNAME");
                strQry.AppendLine(",E.EMPLOYEE_EMAIL");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));

                if (strNoPayslipEmployeeNoIn != "")
                {
                    strQry.AppendLine(" AND E.EMPLOYEE_NO IN (" + strNoPayslipEmployeeNoIn + ")");
                }
                else
                {
                    strQry.AppendLine(" AND E.EMPLOYEE_NO IN (-1)");
                }

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeNoPayslip", parint64CompanyNo);
            }
            
            strQry.Clear();

            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PRINT_PAYSLIPS");
            strQry.AppendLine("(USER_NO");
            strQry.AppendLine(",COMPANY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",SEQ_NO)");
        
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(parint64CurrentUserNo.ToString());
            strQry.AppendLine("," + parint64CompanyNo);
            strQry.AppendLine(",TEMP_PAYSLIP_LINE.EMPLOYEE_NO");
            strQry.AppendLine(",PAYSLIP_LINE_NO");
        
            strQry.AppendLine(" FROM InteractPayroll.dbo.PAYSLIP_LINE_NO");

            strQry.AppendLine(" INNER JOIN");

            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" PP.EMPLOYEE_NO");
            strQry.AppendLine(",MAX(PP.SEQ_NO) AS MAX_SEQ_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PRINT_PAYSLIPS PP");
           
            strQry.AppendLine(" WHERE PP.USER_NO = " + parint64CurrentUserNo.ToString());
            strQry.AppendLine(" AND PP.COMPANY_NO = " + parint64CompanyNo);

            strQry.AppendLine(" GROUP BY PP.EMPLOYEE_NO) AS TEMP_PAYSLIP_LINE ");

            strQry.AppendLine(" ON PAYSLIP_LINE_NO > TEMP_PAYSLIP_LINE.MAX_SEQ_NO");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" C.COMPANY_DESC");
            strQry.AppendLine(",'' AS COMPANY_POST_ADDR");
            strQry.AppendLine(",'' AS COMPANY_PHYSICAL_ADDR");
            strQry.AppendLine(",C.POST_ADDR_LINE1");
            strQry.AppendLine(",C.POST_ADDR_LINE2");
            strQry.AppendLine(",C.POST_ADDR_LINE3");
            strQry.AppendLine(",C.POST_ADDR_LINE4");
            strQry.AppendLine(",C.POST_ADDR_CODE");

            strQry.AppendLine(",C.RES_UNIT_NUMBER");
            strQry.AppendLine(",C.RES_COMPLEX");
            strQry.AppendLine(",C.RES_STREET_NUMBER");
            strQry.AppendLine(",C.RES_STREET_NAME");
            strQry.AppendLine(",C.RES_SUBURB");
            strQry.AppendLine(",C.RES_CITY");
            strQry.AppendLine(",C.RES_ADDR_CODE");
            
            strQry.AppendLine(",C.PAYSLIP_YTD_IND");
            strQry.AppendLine(",CL.DATE_FORMAT");

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(",'" + pardtPaySlipDate.ToString("d MMM yyyy") + "' AS PAYSLIP_HEADER");
            }
            else
            {
                strQry.AppendLine(",'" + pardtPaySlipDate.ToString("d MMMM yyyy") + "' AS PAYSLIP_HEADER");
            }

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.COMPANY_LINK CL");
            strQry.AppendLine(" ON C.COMPANY_NO = CL.COMPANY_NO ");

            strQry.AppendLine(" WHERE C.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "ReportHeader", parint64CompanyNo);

            //Data Is Passed to Reports
            if (DataSet.Tables["ReportHeader"].Rows[0]["POST_ADDR_LINE1"].ToString() != "")
            {
                DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_POST_ADDR"] = DataSet.Tables["ReportHeader"].Rows[0]["POST_ADDR_LINE1"].ToString();
            }

            if (DataSet.Tables["ReportHeader"].Rows[0]["POST_ADDR_LINE2"].ToString() != "")
            {
                if (DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_POST_ADDR"].ToString() != "")
                {
                    DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_POST_ADDR"] = DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_POST_ADDR"].ToString() + "\n" + DataSet.Tables["ReportHeader"].Rows[0]["POST_ADDR_LINE2"].ToString();
                }
                else
                {
                    DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_POST_ADDR"] = DataSet.Tables["ReportHeader"].Rows[0]["POST_ADDR_LINE2"].ToString();
                }
            }

            if (DataSet.Tables["ReportHeader"].Rows[0]["POST_ADDR_LINE3"].ToString() != "")
            {
                if (DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_POST_ADDR"].ToString() != "")
                {
                    DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_POST_ADDR"] = DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_POST_ADDR"].ToString() + "\n" + DataSet.Tables["ReportHeader"].Rows[0]["POST_ADDR_LINE3"].ToString();
                }
                else
                {
                    DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_POST_ADDR"] = DataSet.Tables["ReportHeader"].Rows[0]["POST_ADDR_LINE3"].ToString();
                }
            }

            if (DataSet.Tables["ReportHeader"].Rows[0]["POST_ADDR_LINE4"].ToString() != "")
            {
                if (DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_POST_ADDR"].ToString() != "")
                {
                    DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_POST_ADDR"] = DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_POST_ADDR"].ToString() + "\n" + DataSet.Tables["ReportHeader"].Rows[0]["POST_ADDR_LINE4"].ToString();
                }
                else
                {
                    DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_POST_ADDR"] = DataSet.Tables["ReportHeader"].Rows[0]["POST_ADDR_LINE4"].ToString();
                }
            }

            if (DataSet.Tables["ReportHeader"].Rows[0]["POST_ADDR_CODE"].ToString() != "")
            {
                if (DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_POST_ADDR"].ToString() != "")
                {
                    DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_POST_ADDR"] = DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_POST_ADDR"].ToString() + "\n" + DataSet.Tables["ReportHeader"].Rows[0]["POST_ADDR_CODE"].ToString();
                }
                else
                {
                    DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_POST_ADDR"] = DataSet.Tables["ReportHeader"].Rows[0]["POST_ADDR_CODE"].ToString();
                }
            }

            if (DataSet.Tables["ReportHeader"].Rows[0]["RES_UNIT_NUMBER"].ToString() != ""
                | DataSet.Tables["ReportHeader"].Rows[0]["RES_COMPLEX"].ToString() != "")
            {
                DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_PHYSICAL_ADDR"] = DataSet.Tables["ReportHeader"].Rows[0]["RES_UNIT_NUMBER"].ToString() + " " + DataSet.Tables["ReportHeader"].Rows[0]["RES_COMPLEX"].ToString();
            }

            if (DataSet.Tables["ReportHeader"].Rows[0]["RES_STREET_NUMBER"].ToString() != ""
                | DataSet.Tables["ReportHeader"].Rows[0]["RES_STREET_NAME"].ToString() != "")
            {
                if (DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_PHYSICAL_ADDR"].ToString() != "")
                {
                    DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_PHYSICAL_ADDR"] = DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_PHYSICAL_ADDR"].ToString() + "\n" + DataSet.Tables["ReportHeader"].Rows[0]["RES_STREET_NUMBER"].ToString() + " " + DataSet.Tables["ReportHeader"].Rows[0]["RES_STREET_NAME"].ToString();
                }
                else
                {
                    DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_PHYSICAL_ADDR"] = DataSet.Tables["ReportHeader"].Rows[0]["RES_STREET_NUMBER"].ToString() + " " + DataSet.Tables["ReportHeader"].Rows[0]["RES_STREET_NAME"].ToString();
                }
            }

            if (DataSet.Tables["ReportHeader"].Rows[0]["RES_SUBURB"].ToString() != "")
            {
                if (DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_PHYSICAL_ADDR"].ToString() != "")
                {
                    DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_PHYSICAL_ADDR"] = DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_PHYSICAL_ADDR"].ToString() + "\n" + DataSet.Tables["ReportHeader"].Rows[0]["RES_SUBURB"].ToString();
                }
                else
                {
                    DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_PHYSICAL_ADDR"] = DataSet.Tables["ReportHeader"].Rows[0]["RES_SUBURB"].ToString();
                }
            }

            if (DataSet.Tables["ReportHeader"].Rows[0]["RES_CITY"].ToString() != "")
            {
                if (DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_PHYSICAL_ADDR"].ToString() != "")
                {
                    DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_PHYSICAL_ADDR"] = DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_PHYSICAL_ADDR"].ToString() + "\n" + DataSet.Tables["ReportHeader"].Rows[0]["RES_CITY"].ToString();
                }
                else
                {
                    DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_PHYSICAL_ADDR"] = DataSet.Tables["ReportHeader"].Rows[0]["RES_CITY"].ToString();
                }
            }

            if (DataSet.Tables["ReportHeader"].Rows[0]["RES_ADDR_CODE"].ToString() != "")
            {
                if (DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_PHYSICAL_ADDR"].ToString() != "")
                {
                    DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_PHYSICAL_ADDR"] = DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_PHYSICAL_ADDR"].ToString() + "\n" + DataSet.Tables["ReportHeader"].Rows[0]["RES_ADDR_CODE"].ToString();
                }
                else
                {
                    DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_PHYSICAL_ADDR"] = DataSet.Tables["ReportHeader"].Rows[0]["RES_ADDR_CODE"].ToString();
                }
            }

            DataSet.Tables["ReportHeader"].Columns.Remove("POST_ADDR_LINE1");
            DataSet.Tables["ReportHeader"].Columns.Remove("POST_ADDR_LINE2");
            DataSet.Tables["ReportHeader"].Columns.Remove("POST_ADDR_LINE3");
            DataSet.Tables["ReportHeader"].Columns.Remove("POST_ADDR_LINE4");
            DataSet.Tables["ReportHeader"].Columns.Remove("POST_ADDR_CODE");
            DataSet.Tables["ReportHeader"].Columns.Remove("RES_UNIT_NUMBER");
            DataSet.Tables["ReportHeader"].Columns.Remove("RES_COMPLEX");
            DataSet.Tables["ReportHeader"].Columns.Remove("RES_STREET_NUMBER");
            DataSet.Tables["ReportHeader"].Columns.Remove("RES_STREET_NAME");
            DataSet.Tables["ReportHeader"].Columns.Remove("RES_SUBURB");
            DataSet.Tables["ReportHeader"].Columns.Remove("RES_CITY");
            DataSet.Tables["ReportHeader"].Columns.Remove("RES_ADDR_CODE");
          
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" '' AS COMPANY_DESC");
            //2017-01-28 (Allow Employee to Change PAY_CATEGORY_TYPE)  
            strQry.AppendLine(",EIH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",PP.SEQ_NO");
            strQry.AppendLine(",PP.EARNINGS_DESC ");
            strQry.AppendLine(",EARNINGS_HOURS = ");

            strQry.AppendLine(" CASE");

            strQry.AppendLine(" WHEN PP.EARNINGS_HOURS IS NULL");
            strQry.AppendLine(" THEN ''");

            //ELR 2014-05-24
            strQry.AppendLine(" WHEN PP.EARNINGS_HOURS = 0");
            strQry.AppendLine(" THEN ''");

            strQry.AppendLine(" ELSE CONVERT(VARCHAR, CAST(PP.EARNINGS_HOURS AS MONEY), 1)");

            strQry.AppendLine(" END");
            
            strQry.AppendLine(",PP.EARNINGS_AMOUNT ");
            strQry.AppendLine(",PP.DEDUCTIONS_DESC");
            strQry.AppendLine(",PP.DEDUCTIONS_AMOUNT");
            strQry.AppendLine(",ISNULL(PP.EARNINGS_AMOUNT,0) - ISNULL(PP.DEDUCTIONS_AMOUNT,0) AS NETT_AMOUNT");
            strQry.AppendLine(",'' AS COMPANY_POST_ADDR");
            strQry.AppendLine(",'' AS COMPANY_PHYSICAL_ADDR");

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(",EIH.EMPLOYEE_LAST_RUNDATE");
            }
           
            strQry.AppendLine(",'' AS PAYSLIP_HEADER");
            
            strQry.AppendLine(",EMPLOYEE_REF_NO = ");

            strQry.AppendLine(" CASE");

            strQry.AppendLine(" WHEN NOT E.EMPLOYEE_ID_NO IS NULL");
            strQry.AppendLine(" THEN E.EMPLOYEE_ID_NO");

            strQry.AppendLine(" ELSE E.EMPLOYEE_PASSPORT_NO");

            strQry.AppendLine(" END");

            strQry.AppendLine(",ISNULL(TEMP_ACCUM_LEAVE.TOTAL_LEAVE_DAYS,0) AS LEAVE_DAYS_DUE");

            strQry.AppendLine(",'Printed   " + DateTime.Now.ToString(DataSet.Tables["ReportHeader"].Rows[0]["DATE_FORMAT"].ToString() + "   HH:mm") + "' AS REPORT_DATETIME");

            if (parstrPayCategoryType == "W")
            {
                 strQry.AppendLine(",'Rate / Hour' AS PAYMENT_TYPE_DESC");
                 strQry.AppendLine(",EPCH.HOURLY_RATE AS PAYMENT_RATE");
            }
            else
            {
                strQry.AppendLine(",'Monthly Payment' AS PAYMENT_TYPE_DESC");
                strQry.AppendLine(",EIH.SALARY_MONTH_PAYMENT AS PAYMENT_RATE");
            }

            strQry.AppendLine(",OH.OCCUPATION_DESC");

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(",ISNULL(SUM(EIH1.CURRENT_YEAR_LEAVE_SHIFTS_PER_RUN),0) AS CURRENT_YEAR_LEAVE_SHIFTS ");
            }
            else
            {
                strQry.AppendLine(",0 AS CURRENT_YEAR_LEAVE_SHIFTS ");
            }
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PRINT_PAYSLIPS PP");
            
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON PP.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND PP.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            //2017-01-28 (Allow Employee to Change PAY_CATEGORY_TYPE)  
            //strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");
                strQry.AppendLine(" ON E.COMPANY_NO = EPCH.COMPANY_NO ");
                strQry.AppendLine(" AND EPCH.PAY_PERIOD_DATE = '" + pardtPaySlipDate.ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCH.EMPLOYEE_NO ");
                //2017-01-28 (Allow Employee to Change PAY_CATEGORY_TYPE) 
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
                strQry.AppendLine(" AND EPCH.RUN_TYPE = 'P' ");
                strQry.AppendLine(" AND EPCH.DEFAULT_IND = 'Y' ");
            }
           
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY EIH");
            strQry.AppendLine(" ON E.COMPANY_NO = EIH.COMPANY_NO ");
            strQry.AppendLine(" AND EIH.PAY_PERIOD_DATE = '" + pardtPaySlipDate.ToString("yyyy-MM-dd") + "'");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EIH.EMPLOYEE_NO ");
            //2017-01-28 (Allow Employee to Change PAY_CATEGORY_TYPE) 
            strQry.AppendLine(" AND EIH.PAY_CATEGORY_TYPE  = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
            strQry.AppendLine(" AND EIH.RUN_TYPE = 'P' ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.OCCUPATION_HISTORY OH");
            strQry.AppendLine(" ON EIH.COMPANY_NO = OH.COMPANY_NO ");
            strQry.AppendLine(" AND EIH.PAY_PERIOD_DATE = OH.PAY_PERIOD_DATE");
            strQry.AppendLine(" AND EIH.PAY_CATEGORY_TYPE = OH.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EIH.OCCUPATION_NO = OH.OCCUPATION_NO ");
      
            strQry.AppendLine(" LEFT JOIN ");
            
            strQry.AppendLine("(");

            //Normal Leave
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" LH.EMPLOYEE_NO");
            strQry.AppendLine(",ROUND(SUM(LH.LEAVE_ACCUM_DAYS - LH.LEAVE_PAID_DAYS),2) AS TOTAL_LEAVE_DAYS");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY LH");
            
            strQry.AppendLine(" WHERE LH.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND LH.EARNING_NO = 200");
            strQry.AppendLine(" AND LH.PAY_PERIOD_DATE <= '" + pardtPaySlipDate.ToString("yyyy-MM-dd") + "'");
   
            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" LH.EMPLOYEE_NO");

            strQry.AppendLine(") AS TEMP_ACCUM_LEAVE");

            strQry.AppendLine(" ON E.EMPLOYEE_NO = TEMP_ACCUM_LEAVE.EMPLOYEE_NO ");
            //2017-01-28 (Allow Employee to Change PAY_CATEGORY_TYPE) 
            //strQry.AppendLine(" AND EIH.PAY_CATEGORY_TYPE = TEMP_ACCUM_LEAVE.PAY_CATEGORY_TYPE ");

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY EIH1");
                strQry.AppendLine(" ON E.COMPANY_NO = EIH1.COMPANY_NO ");

                strQry.AppendLine(" AND EPCH.PAY_PERIOD_DATE >= EIH1.PAY_PERIOD_DATE ");
                strQry.AppendLine(" AND EIH1.PAY_PERIOD_DATE >= '" + dtStartLeaveTaxYear.ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" AND EIH1.PAY_PERIOD_DATE < '" + dtStartLeaveTaxYear.AddYears(1).ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" AND EIH1.PAY_PERIOD_DATE <= '" + pardtPaySlipDate.AddYears(1).ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EIH1.EMPLOYEE_NO ");
                //2017-01-28 (Allow Employee to Change PAY_CATEGORY_TYPE) 
                strQry.AppendLine(" AND EIH.PAY_CATEGORY_TYPE = EIH1.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EIH1.RUN_TYPE = 'P' ");

                //strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY LH");
                //strQry.AppendLine(" ON E.COMPANY_NO = LH.COMPANY_NO ");
                //strQry.AppendLine(" AND EPCH.PAY_PERIOD_DATE >= LH.PAY_PERIOD_DATE ");
                //strQry.AppendLine(" AND LH.PAY_PERIOD_DATE >= '" + dtStartLeaveTaxYear.ToString("yyyy-MM-dd") + "'");
                //strQry.AppendLine(" AND LH.PAY_PERIOD_DATE < '" + dtStartLeaveTaxYear.AddYears(1).ToString("yyyy-MM-dd") + "'");
                //strQry.AppendLine(" AND LH.PAY_PERIOD_DATE <= '" + pardtPaySlipDate.AddYears(1).ToString("yyyy-MM-dd") + "'");
                //strQry.AppendLine(" AND E.EMPLOYEE_NO = LH.EMPLOYEE_NO ");
                //strQry.AppendLine(" AND LH.PROCESS_NO = 98 ");
                //strQry.AppendLine(" AND LH.EARNING_NO = 200 ");
            }

            strQry.AppendLine(" WHERE PP.USER_NO = " + parint64CurrentUserNo.ToString());
            strQry.AppendLine(" AND PP.COMPANY_NO = " + parint64CompanyNo);

            strQry.AppendLine(" GROUP BY");

            //2017-01-28 (Allow Employee to Change PAY_CATEGORY_TYPE)  
            strQry.AppendLine(" EIH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",PP.SEQ_NO");
            strQry.AppendLine(",PP.EARNINGS_DESC");
            strQry.AppendLine(",PP.EARNINGS_HOURS");
            strQry.AppendLine(",PP.EARNINGS_AMOUNT");
            strQry.AppendLine(",PP.DEDUCTIONS_DESC");
            strQry.AppendLine(",PP.DEDUCTIONS_AMOUNT");
            strQry.AppendLine(",EIH.EMPLOYEE_LAST_RUNDATE");
            strQry.AppendLine(",E.EMPLOYEE_ID_NO");
            strQry.AppendLine(",E.EMPLOYEE_PASSPORT_NO");
            strQry.AppendLine(",TEMP_ACCUM_LEAVE.TOTAL_LEAVE_DAYS");

            if (parstrPayCategoryType == "W")
            {
                strQry.AppendLine(",EPCH.HOURLY_RATE");
            }
            else
            {
                strQry.AppendLine(",EIH.SALARY_MONTH_PAYMENT");
            }
                      
            strQry.AppendLine(",OH.OCCUPATION_DESC");

            strQry.AppendLine(" ORDER BY ");

            if (parstrPrintOrderInd == "C")
            {
                strQry.AppendLine(" E.EMPLOYEE_CODE");
            }
            else
            {
                if (parstrPrintOrderInd == "S")
                {
                    strQry.AppendLine(" E.EMPLOYEE_SURNAME");
                }
                else
                {
                    strQry.AppendLine(" E.EMPLOYEE_NAME");
                }
            }

            strQry.AppendLine(",PP.SEQ_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Report", parint64CompanyNo);

            for (int intRow = 0; intRow < DataSet.Tables["Report"].Rows.Count; intRow++)
            {
                //if (intRow == 0)
                //{
                //    DataSet.Tables["Report"].Rows[intRow]["EARNINGS_HOURS"] = 9999.99;
                //    DataSet.Tables["Report"].Rows[intRow]["EARNINGS_AMOUNT"] = 99999999.99;

                //    DataSet.Tables["Report"].Rows[intRow]["DEDUCTIONS_AMOUNT"] = 99999999.99;
                //    DataSet.Tables["Report"].Rows[intRow]["NETT_AMOUNT"] = 99999999.99;
                //}

                DataSet.Tables["Report"].Rows[intRow]["COMPANY_DESC"] = DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_DESC"].ToString();
                DataSet.Tables["Report"].Rows[intRow]["COMPANY_POST_ADDR"] = DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_POST_ADDR"].ToString();
                DataSet.Tables["Report"].Rows[intRow]["COMPANY_PHYSICAL_ADDR"] = DataSet.Tables["ReportHeader"].Rows[0]["COMPANY_PHYSICAL_ADDR"].ToString();

                if (parstrPayCategoryType == "W")
                {
                    DataSet.Tables["Report"].Rows[intRow]["PAYSLIP_HEADER"] = Convert.ToDateTime(DataSet.Tables["Report"].Rows[intRow]["EMPLOYEE_LAST_RUNDATE"]).AddDays(1).ToString("d MMM yyyy") + " - " + DataSet.Tables["ReportHeader"].Rows[0]["PAYSLIP_HEADER"].ToString();
                }
                else
                {
                    DataSet.Tables["Report"].Rows[intRow]["PAYSLIP_HEADER"] = DataSet.Tables["ReportHeader"].Rows[0]["PAYSLIP_HEADER"].ToString();
                }
            }

            DataSet.Tables.Remove("ReportHeader");

            DataSet.AcceptChanges();

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public int Put_Email_Payslips_On_Queue(Int64 parint64CompanyNo, Int64 parint64CurrentUserNo)
        {
            int intReturnCode = 0;
            
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.PAYSLIP_EMAIL_QUEUE ");

            strQry.AppendLine("(COMPANY_NO ");
            strQry.AppendLine(",USER_NO ");
            strQry.AppendLine(",PAY_PERIOD_DATE ");
            strQry.AppendLine(",PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",EMPLOYEE_NO_IN ) ");
            
            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" COMPANY_NO ");
            strQry.AppendLine(",USER_NO ");
            strQry.AppendLine(",PAY_PERIOD_DATE ");
            strQry.AppendLine(",PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",EMPLOYEE_NO_IN ");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAYSLIP_EMAIL_LAST_PRINT PELP ");
            
            strQry.AppendLine(" WHERE PELP.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PELP.USER_NO = " + parint64CurrentUserNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            
            strQry.Clear();

            strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.PAYSLIP_EMAIL_LAST_PRINT ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND USER_NO = " + parint64CurrentUserNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            return intReturnCode;
        }

        public void Email_Payslips(Int64 parint64CompanyNo, Int64 parint64CurrentUserNo,DateTime parPayPeriodDate,int parintPayslipEmailQueueNo)
        {
            string strClassNameFunctionAndParameters = pvtstrClassName + " Email_Payslips CompanyNo=" + parint64CompanyNo + ",parint64CurrentUserNo=" + parint64CurrentUserNo + ",parPayPeriodDate=" + parPayPeriodDate.ToString("yyyy-MM-dd") + ",parintPayslipEmailQueueNo=" + parintPayslipEmailQueueNo;
            
            WriteLog(strClassNameFunctionAndParameters);

            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
            
            int intPayslipEmailQueueCompletedNo = 0;
            
            string strFileName = "";
            string strToEmailAddress = "";
            string strToEmailAddressName = "";
            string strFromEmailAddress = "";
            string strFromEmailAddressName = "";

            try
            {
                //Remove Off Queue
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.PAYSLIP_EMAIL_QUEUE");

                strQry.AppendLine(" SET ");
                strQry.AppendLine(" START_RUN_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                strQry.AppendLine(",PAYSLIP_EMAIL_QUEUE_IND = 'S'");
                
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parPayPeriodDate.ToString("yyyy-MM-dd")));
                strQry.AppendLine(" AND PAYSLIP_EMAIL_QUEUE_NO = " + parintPayslipEmailQueueNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                
                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAYSLIP_EMAIL_QUEUE_COMPLETED  ");
                strQry.AppendLine("(COMPANY_NO ");
                strQry.AppendLine(",USER_NO ");
                strQry.AppendLine(",PAY_PERIOD_DATE ");
                strQry.AppendLine(",PAY_CATEGORY_TYPE ");
                strQry.AppendLine(",EMPLOYEE_NO_IN ");
                strQry.AppendLine(",START_RUN_DATE ");
                strQry.AppendLine(",PAYSLIP_EMAIL_QUEUE_IND)");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" COMPANY_NO ");
                strQry.AppendLine(",USER_NO ");
                strQry.AppendLine(",PAY_PERIOD_DATE ");
                strQry.AppendLine(",PAY_CATEGORY_TYPE ");
                strQry.AppendLine(",EMPLOYEE_NO_IN ");
                strQry.AppendLine(",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                strQry.AppendLine(",'S'");
                
                strQry.AppendLine(" FROM InteractPayroll.dbo.PAYSLIP_EMAIL_QUEUE ");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parPayPeriodDate.ToString("yyyy-MM-dd")));
                strQry.AppendLine(" AND PAYSLIP_EMAIL_QUEUE_NO = " + parintPayslipEmailQueueNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PAY_CATEGORY_TYPE");
                strQry.AppendLine(",MAX(PAYSLIP_EMAIL_QUEUE_COMPLETED_NO) AS PAYSLIP_EMAIL_QUEUE_COMPLETED_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAYSLIP_EMAIL_QUEUE_COMPLETED  ");
                
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parPayPeriodDate.ToString("yyyy-MM-dd")));

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" PAY_CATEGORY_TYPE");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "TieBreaker", parint64CompanyNo);

                intPayslipEmailQueueCompletedNo = Convert.ToInt32(DataSet.Tables["TieBreaker"].Rows[0]["PAYSLIP_EMAIL_QUEUE_COMPLETED_NO"]);

                //Remove From Main Queue
                strQry.Clear();

                strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.PAYSLIP_EMAIL_QUEUE");
                
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parPayPeriodDate.ToString("yyyy-MM-dd")));
                strQry.AppendLine(" AND PAYSLIP_EMAIL_QUEUE_NO = " + parintPayslipEmailQueueNo);
                
                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PEQC.COMPANY_NO ");
                strQry.AppendLine(",PEQC.PAY_PERIOD_DATE ");
                strQry.AppendLine(",PEQC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(",PEQC.EMPLOYEE_NO_IN ");
                strQry.AppendLine(",PEQC.USER_NO ");
                strQry.AppendLine(",C.COMPANY_DESC ");

                if (parint64CurrentUserNo != 0)
                {
                    strQry.AppendLine(",UI.FIRSTNAME AS USER_FIRSTNAME ");
                    strQry.AppendLine(",UI.SURNAME AS USER_SURNAME ");
                    strQry.AppendLine(",UI.EMAIL AS USER_EMAIL ");
                }
                else
                {
                    //Fix Later
                    strQry.AppendLine(",'" + pvtstrSmtpSysAdminEmailAddressFirstName + "' AS USER_FIRSTNAME ");
                    strQry.AppendLine(",'" + pvtstrSmtpSysAdminEmailAddressLastName + "' AS USER_SURNAME ");
                    strQry.AppendLine(",'" + pvtstrSmtpSysAdminEmailAddress + "' AS USER_EMAIL ");
                }

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAYSLIP_EMAIL_QUEUE_COMPLETED PEQC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C ");
                strQry.AppendLine(" ON PEQC.COMPANY_NO = C.COMPANY_NO ");
                
                if (parint64CurrentUserNo != 0)
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_ID UI ");
                    strQry.AppendLine(" ON PEQC.USER_NO = UI.USER_NO ");
                }

                strQry.AppendLine(" WHERE PEQC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PEQC.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND PEQC.PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parPayPeriodDate.ToString("yyyy-MM-dd")));
                //Tie Breaker for Company's Table
                strQry.AppendLine(" AND PEQC.PAYSLIP_EMAIL_QUEUE_COMPLETED_NO = " + intPayslipEmailQueueCompletedNo);

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmailPayslips", parint64CompanyNo);

                if (DataSet.Tables["EmailPayslips"].Rows.Count > 0)
                {
                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" E.EMPLOYEE_NO ");
                    strQry.AppendLine(",E.EMPLOYEE_NAME ");
                    strQry.AppendLine(",E.EMPLOYEE_SURNAME ");
                    strQry.AppendLine(",E.EMPLOYEE_EMAIL ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_NO IN (" + DataSet.Tables["EmailPayslips"].Rows[0]["EMPLOYEE_NO_IN"].ToString() + ")");
                    strQry.AppendLine(" AND ISNULL(E.EMPLOYEE_EMAIL,'') <> ''");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parint64CompanyNo);

                    //WriteLog("busRptPayslip Employee Count = " + DataSet.Tables["Employee"].Rows.Count.ToString());

                    for (int intEmployeeRow = 0; intEmployeeRow < DataSet.Tables["Employee"].Rows.Count; intEmployeeRow++)
                    {
                        //WriteLog("busRptPayslip Print_PaySlip");

                        byte[] byteCompressed = Print_PaySlip(parint64CurrentUserNo, "", parint64CompanyNo, DataSet.Tables["EmailPayslips"].Rows[0]["PAY_CATEGORY_TYPE"].ToString(), Convert.ToDateTime(DataSet.Tables["EmailPayslips"].Rows[0]["PAY_PERIOD_DATE"]), DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString(), "", "", "S","E");

                        DataSet TempDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(byteCompressed);

                        //WriteLog("busRptPayslip TempDataSet");

                        Microsoft.Reporting.WinForms.ReportDataSource myReportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource("Report", TempDataSet.Tables["Report"]);

                        ReportViewer reportViewer = new ReportViewer();

                        reportViewer.LocalReport.DataSources.Clear();
                        reportViewer.LocalReport.ReportEmbeddedResource = "InteractPayroll.Report.rdlc";
                        reportViewer.LocalReport.DataSources.Add(myReportDataSource);

                        ////Calculates Number of Pages in Report and Display in Viewer
                        reportViewer.PageCountMode = Microsoft.Reporting.WinForms.PageCountMode.Actual;

                        reportViewer.RefreshReport();
                        reportViewer.Focus();

                        Microsoft.Reporting.WinForms.Warning[] warnings;
                        string[] streamids;
                        string mimeType;
                        string encoding;
                        string filenameExtension;

                        //WriteLog("busRptPayslip reportViewer.LocalReport.Render");

                        byte[] byteArrayFile = reportViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

                        string strEmployeeNames = DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NAME"].ToString().Replace(" ", "") + "_" + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_SURNAME"].ToString().Replace(" ", "");
                        
                        strFileName = pvtstrEmailFolder + "\\" + DataSet.Tables["EmailPayslips"].Rows[0]["COMPANY_DESC"].ToString().Replace(" ", "") + "_Payslip_" + Convert.ToDateTime(DataSet.Tables["EmailPayslips"].Rows[0]["PAY_PERIOD_DATE"]).ToString("dMMMyyy") + "_" + strEmployeeNames + ".pdf";

                        if (File.Exists(strFileName) == true)
                        {
                            //WriteLog("busRptPayslip File.Delete");
                            File.Delete(strFileName);
                        }

                        try
                        {
                            //WriteLog("busRptPayslip before Create PDF");
                            //Create PDF File for Email
                            using (FileStream fs = new FileStream(strFileName, FileMode.Create))
                            {
                                fs.Write(byteArrayFile, 0, byteArrayFile.Length);
                            }

                            //WriteLog("busRptPayslip after Create PDF");

                            strFromEmailAddress = DataSet.Tables["EmailPayslips"].Rows[0]["USER_EMAIL"].ToString();
                            strFromEmailAddressName = DataSet.Tables["EmailPayslips"].Rows[0]["USER_FIRSTNAME"].ToString() + " " + DataSet.Tables["EmailPayslips"].Rows[0]["USER_SURNAME"].ToString();
                            
                            strToEmailAddress = DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_EMAIL"].ToString();
                            strToEmailAddressName = DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NAME"].ToString() + " " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_SURNAME"].ToString();
#if (DEBUG)
                            //strToEmailAddress = pvtstrSmtpEmailAddress;
                            //strFromEmailAddress = pvtstrSmtpEmailAddress;
#endif
                            //Email
                            var fromAddress = new MailAddress(pvtstrSmtpEmailAddress, pvtstrSmtpEmailAddressDescription);
                            var toAddress = new MailAddress(strToEmailAddress, strToEmailAddressName);
                          
                            string subject = DataSet.Tables["EmailPayslips"].Rows[0]["COMPANY_DESC"].ToString() + " - Payslip for " + parPayPeriodDate.ToString("d MMMM yyyy");

                            var smtp = new SmtpClient();

                            smtp.Host = pvtstrSmtpHostName;
                            smtp.Port = pvtintSmtpHostPort;
                            smtp.EnableSsl = true;
                            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = new NetworkCredential(pvtstrSmtpEmailAddress, pvtstrSmtpEmailAddressPassword);

                            var message = new MailMessage(fromAddress, toAddress);

                            message.Subject = subject;
                            message.Body = "Dear " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NAME"].ToString() + " " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_SURNAME"].ToString() + "\n";
                            message.Body += "\n";
                            message.Body += "See attached Payslip.\n";
                            message.Body += "\n";
                            message.Body += "Kind Regards\n";
                            message.Body += DataSet.Tables["EmailPayslips"].Rows[0]["USER_FIRSTNAME"].ToString() + " " + DataSet.Tables["EmailPayslips"].Rows[0]["USER_SURNAME"].ToString() + "\n";
                            message.Body += "\n";
                            message.Body += "NB. DO NOT REPLY TO THIS EMAIL ADDRESS.\n";
                            message.Body += "\n";
                            message.Body += "If you need to email me, email me at\n";
                            message.Body += strFromEmailAddress;
                            
                            //Add File Attachment
                            System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(strFileName);

                            message.Attachments.Add(attachment);

                            bool blnSmtpSendSuccessful = true;

                            try
                            {
                                //WriteLog("busRptPayslip smtp.Send");

                                smtp.Send(message);
                            }
                            catch (SmtpFailedRecipientsException ex)
                            {
                                WriteLog("busRptPayslip smtp.Send Exception=" + ex.Message);
                                blnSmtpSendSuccessful = false;
                            }
                            finally
                            {
                                message.Attachments.Dispose();
                                smtp.Dispose();
                                smtp = null;
                            }
                            
                            strQry.Clear();

                            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAYSLIP_EMAIL_QUEUE_EMPLOYEE  ");
                            strQry.AppendLine("(COMPANY_NO ");
                            strQry.AppendLine(",USER_NO ");
                            strQry.AppendLine(",PAY_PERIOD_DATE ");
                            strQry.AppendLine(",PAYSLIP_EMAIL_QUEUE_COMPLETED_NO ");
                            strQry.AppendLine(",EMPLOYEE_NO ");
                            strQry.AppendLine(",PAY_CATEGORY_TYPE ");
                            strQry.AppendLine(",EMPLOYEE_EMAIL ");
                            strQry.AppendLine(",PAYSLIP_EMAIL_QUEUE_EMPLOYEE_IND)");

                            strQry.AppendLine(" VALUES ");

                            strQry.AppendLine("(" + parint64CompanyNo);
                            strQry.AppendLine("," + parint64CurrentUserNo);
                            strQry.AppendLine(",'" + parPayPeriodDate.ToString("yyyy-MM-dd") + "'");
                            strQry.AppendLine("," + intPayslipEmailQueueCompletedNo);
                            strQry.AppendLine("," + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["TieBreaker"].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
                            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_EMAIL"].ToString()));

                            //Completed Successfully
                            if (blnSmtpSendSuccessful == true)
                            {
                                strQry.AppendLine(",'C')");
                            }
                            else
                            {
                                //Failed
                                strQry.AppendLine(",'F')");
                            }

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                            //Remove Created PDF File
                            File.Delete(strFileName);
                        }
                        catch(Exception ex)
                        {
                            strQry.Clear();

                            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAYSLIP_EMAIL_QUEUE_EMPLOYEE  ");
                            strQry.AppendLine("(COMPANY_NO ");
                            strQry.AppendLine(",USER_NO ");
                            strQry.AppendLine(",PAY_PERIOD_DATE ");
                            strQry.AppendLine(",PAYSLIP_EMAIL_QUEUE_COMPLETED_NO ");
                            strQry.AppendLine(",EMPLOYEE_NO ");
                            strQry.AppendLine(",PAY_CATEGORY_TYPE ");
                            strQry.AppendLine(",EMPLOYEE_EMAIL ");
                            strQry.AppendLine(",PAYSLIP_EMAIL_QUEUE_EMPLOYEE_IND)");

                            strQry.AppendLine(" VALUES ");

                            strQry.AppendLine("(" + parint64CompanyNo);
                            strQry.AppendLine("," + parint64CurrentUserNo);
                            strQry.AppendLine(",'" + parPayPeriodDate.ToString("yyyy-MM-dd") + "'");
                            strQry.AppendLine("," + intPayslipEmailQueueCompletedNo);
                            strQry.AppendLine("," + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["TieBreaker"].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
                            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_EMAIL"].ToString()));
                            //Failed
                            strQry.AppendLine(",'F')");

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                            //Remove Created PDF File
                            if (File.Exists(strFileName) == true)
                            {
                                File.Delete(strFileName);
                            }
                        }
                    }

                    //Competed Successfully
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PAYSLIP_EMAIL_QUEUE_COMPLETED");

                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" PAYSLIP_EMAIL_QUEUE_IND = 'C'");
                    strQry.AppendLine(",END_RUN_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND USER_NO = " + parint64CurrentUserNo);
                    strQry.AppendLine(" AND PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parPayPeriodDate.ToString("yyyy-MM-dd")));
                    //Tie Breaker for Company's Table
                    strQry.AppendLine(" AND PAYSLIP_EMAIL_QUEUE_COMPLETED_NO = " + intPayslipEmailQueueCompletedNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                }
                else
                {
                    //No Employees to Email
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PAYSLIP_EMAIL_QUEUE_COMPLETED");

                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" PAYSLIP_EMAIL_QUEUE_IND = 'N'");
                    strQry.AppendLine(",END_RUN_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND USER_NO = " + parint64CurrentUserNo);
                    strQry.AppendLine(" AND PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parPayPeriodDate.ToString("yyyy-MM-dd")));
                    //Tie Breaker for Company's Table
                    strQry.AppendLine(" AND PAYSLIP_EMAIL_QUEUE_COMPLETED_NO = " + intPayslipEmailQueueCompletedNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                }

                //Set Backup Indicator
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
                strQry.AppendLine(" SET BACKUP_DB_IND = 1");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            }
            catch (Exception ex)
            {
                string strException = ex.Message;

                if (ex.InnerException != null)
                {
                    strException += " " + ex.InnerException;

                }

                Write_Log(ex, strClassNameFunctionAndParameters, strQry.ToString(), true);

                if (intPayslipEmailQueueCompletedNo != 0)
                {
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PAYSLIP_EMAIL_QUEUE_COMPLETED");

                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" PAYSLIP_EMAIL_QUEUE_IND = 'F'");
                    strQry.AppendLine(",END_RUN_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                    strQry.AppendLine(",PAYSLIP_EMAIL_QUEUE_EXCEPTION = " + clsDBConnectionObjects.Text2DynamicSQL(strException));
                    
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND USER_NO = " + parint64CurrentUserNo);
                    strQry.AppendLine(" AND PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parPayPeriodDate.ToString("yyyy-MM-dd")));
                    //Tie Breaker for Company's Table
                    strQry.AppendLine(" AND PAYSLIP_EMAIL_QUEUE_COMPLETED_NO = " + intPayslipEmailQueueCompletedNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                }
                else
                {
                    //In Case that has NOT been Deleted
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE InteractPayroll.dbo.PAYSLIP_EMAIL_QUEUE");

                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" PAYSLIP_EMAIL_QUEUE_IND = 'F'");
                    strQry.AppendLine(",END_RUN_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                    strQry.AppendLine(",PAYSLIP_EMAIL_QUEUE_EXCEPTION = " + clsDBConnectionObjects.Text2DynamicSQL(strException));

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND USER_NO = " + parint64CurrentUserNo);
                    strQry.AppendLine(" AND PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parPayPeriodDate.ToString("yyyy-MM-dd")));
                    strQry.AppendLine(" AND PAYSLIP_EMAIL_QUEUE_NO = " + parintPayslipEmailQueueNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                }
            }
        }
        
        private void Insert_Deductions(ref DataSet parDataSet, Int64 parint64CompanyNo, Int64 parint64CurrentUserNo, int parintEarningsSeqNo, int parintRow)
        {
            StringBuilder strQry = new StringBuilder();
            int intDeductionsSeqNo = 0;
            bool blnRecordsFound = false;
            
            for (int intRow = 0; intRow < parDataSet.Tables["Deduction"].Rows.Count; intRow++)
            {
                if (parDataSet.Tables["Deduction"].Rows[intRow]["EMPLOYEE_NO"].ToString() == parDataSet.Tables["Employee"].Rows[parintRow]["EMPLOYEE_NO"].ToString())
                {
                    blnRecordsFound = true;

                    if (Convert.ToDouble(parDataSet.Tables["Deduction"].Rows[intRow]["TOTAL"]) != 0)
                    {
                        intDeductionsSeqNo = intDeductionsSeqNo + 1;

                        if (intDeductionsSeqNo > parintEarningsSeqNo)
                        {
                            //Tested 2012-08-23
                            strQry.Clear();

                            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PRINT_PAYSLIPS");
                            strQry.AppendLine("(USER_NO");
                            strQry.AppendLine(",COMPANY_NO");
                            strQry.AppendLine(",EMPLOYEE_NO");
                            strQry.AppendLine(",SEQ_NO");
                            strQry.AppendLine(",DEDUCTIONS_DESC");
                            strQry.AppendLine(",DEDUCTIONS_AMOUNT)");
                            strQry.AppendLine(" VALUES");
                            strQry.AppendLine("(" + parint64CurrentUserNo);
                            strQry.AppendLine("," + parint64CompanyNo);
                            strQry.AppendLine("," + parDataSet.Tables["Employee"].Rows[parintRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine("," + intDeductionsSeqNo);

                            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Deduction"].Rows[intRow]["DEDUCTION_DESC"].ToString()));
                           
                            strQry.AppendLine("," + parDataSet.Tables["Deduction"].Rows[intRow]["TOTAL"] + ")");
                            
                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                        }
                        else
                        {
                            //Tested 2012-08-23
                            strQry.Clear();

                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PRINT_PAYSLIPS");
                            strQry.AppendLine(" SET ");
                            strQry.AppendLine(" DEDUCTIONS_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Deduction"].Rows[intRow]["DEDUCTION_DESC"].ToString()));
                            strQry.AppendLine(",DEDUCTIONS_AMOUNT = " + parDataSet.Tables["Deduction"].Rows[intRow]["TOTAL"]);

                            strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo.ToString());
                            strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + parDataSet.Tables["Employee"].Rows[parintRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine(" AND SEQ_NO = " + intDeductionsSeqNo);

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                        }
                    }
                }
                else
                {
                    if (blnRecordsFound == true)
                    {
                        break;
                    }
                }
            }

            //if (intDeductionsSeqNo < parintEarningsSeqNo)
            //{
            //    intDeductionsSeqNo = parintEarningsSeqNo;
            //}
        }
        
        private void WriteLog(string Message)
        {
            try
            {
                using (StreamWriter writeLog = new StreamWriter(pvtstrLogFileName, true))
                {
                    writeLog.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + Message);
                }
            }
            catch (Exception ex)
            {
                WriteExceptionErrorLog(Message, ex);
            }
        }

        private void WriteExceptionErrorLog(string Message, Exception ex)
        {
            try
            {
                string strExceptionMessage = ex.Message;

                if (ex.InnerException != null)
                {
                    strExceptionMessage = ex.InnerException.Message;
                }

                using (StreamWriter writeExceptionErrorLog = new StreamWriter(pvtstrLogFileName.Replace(".txt", "_Error.txt"), true))
                {
                    writeExceptionErrorLog.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + Message + " Exception = " + strExceptionMessage);
                }
            }
            catch
            {
            }
        }

        public void Write_Log(Exception exception, string classNameFunctionAndParameters, string sql, bool sendEmail)
        {
            string strMessage = classNameFunctionAndParameters + "\n\nSQL=" + sql + "\nException=" + exception.Message;

            if (exception.InnerException != null)
            {
                strMessage += "\n\n" + exception.InnerException.Message;
            }

            try
            {
                using (StreamWriter writeLog = new StreamWriter(pvtstrLogFileName, true))
                {
                    writeLog.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + strMessage);
                }
            }
            catch (Exception ex)
            {
                WriteExceptionErrorLog(strMessage, ex);
            }

            if (sendEmail == true)
            {
                var smtp = new SmtpClient();

                try
                {
                    //Email
                    var fromAddress = new MailAddress(pvtstrSmtpEmailAddress, "Errol Le Roux");
                    var toAddress = new MailAddress(pvtstrSmtpEmailAddress, "Errol Le Roux");

                    string subject = "Email Payslip Error - " + DateTime.Now.ToString("dd MMMM yyyy");

                    smtp.Host = pvtstrSmtpHostName;
                    smtp.Port = pvtintSmtpHostPort;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(fromAddress.Address, pvtstrSmtpEmailAddressPassword);

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
