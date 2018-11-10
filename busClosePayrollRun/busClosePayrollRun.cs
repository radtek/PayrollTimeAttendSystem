using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Net;

namespace InteractPayroll
{
    public class busClosePayrollRun
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        string pvtstrClassName = "busClosePayrollRun";

        string pvtstrLogFileName = "";
        string pvtstrSmtpEmailAddressDescription = "";
        string pvtstrSmtpEmailAddress = "";
        string pvtstrSmtpEmailAddressPassword = "";
        string pvtstrSmtpHostName = "";
        int pvtintSmtpHostPort = 0;

        public busClosePayrollRun()
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
        
        public int Insert_Close_Into_Queue(Int64 parInt64CompanyNo, DateTime pardtWageRunDate, DateTime pardtSalaryRunDate, string partstrRunType,string parstrWagesPayCategoryNumbers, string parstrSalariesPayCategoryNumbers, Int64 parint64CurrentUserNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.CLOSE_RUN_QUEUE");
            strQry.AppendLine("(USER_NO");
            strQry.AppendLine(",COMPANY_NO");

            if (partstrRunType == "B")
            {
                strQry.AppendLine(",WAGE_PAY_PERIOD_DATE");
                strQry.AppendLine(",SALARY_PAY_PERIOD_DATE");
            }
            else
            {
                if (partstrRunType == "S")
                {
                    strQry.AppendLine(",SALARY_PAY_PERIOD_DATE");
                }
                else
                {
                    strQry.AppendLine(",WAGE_PAY_PERIOD_DATE");
                }
            }

            strQry.AppendLine(",RUN_TYPE");
            strQry.AppendLine(",WAGES_PAY_CATEGORY_NUMBERS");
            strQry.AppendLine(",SALARIES_PAY_CATEGORY_NUMBERS)");

            strQry.AppendLine(" VALUES ");

            strQry.AppendLine("(" + parint64CurrentUserNo);
            strQry.AppendLine("," + parInt64CompanyNo);
                
            if (partstrRunType == "B")
            {
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pardtWageRunDate.ToString("yyyy-MM-dd")));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pardtSalaryRunDate.ToString("yyyy-MM-dd")));
            }
            else
            {
                if (partstrRunType == "S")
                {
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pardtSalaryRunDate.ToString("yyyy-MM-dd")));
                }
                else
                {
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pardtWageRunDate.ToString("yyyy-MM-dd")));
                }
            }
                                
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(partstrRunType.ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrWagesPayCategoryNumbers));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrSalariesPayCategoryNumbers) + ")");
            
            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            
            return 0;
        }

        public string Check_Queue(Int64 parInt64CompanyNo, string partstrRunType)
        {
            String strReturn = "S";

            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" CLOSE_RUN_QUEUE_IND");

            strQry.AppendLine(" FROM InteractPayroll.dbo.CLOSE_RUN_QUEUE");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND RUN_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(partstrRunType));
         
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayrollQueue", parInt64CompanyNo);

            if (DataSet.Tables["PayrollQueue"].Rows.Count > 0)
            {
                if (DataSet.Tables["PayrollQueue"].Rows[0]["CLOSE_RUN_QUEUE_IND"].ToString() != "")
                {
                    strReturn = DataSet.Tables["PayrollQueue"].Rows[0]["CLOSE_RUN_QUEUE_IND"].ToString();
                }
            }
            else
            {
                //Completed Successfully
                strReturn = "";
            }

            return strReturn;
        }

        public byte[] Get_Form_Records(Int64 parInt64CompanyNo, string parstrFromProgram)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
            
            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" COMPLETED_IND = ");

            strQry.AppendLine(" CASE ");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" WHEN ISNULL(TIME_ATTENDANCE_RUN_IND,'N') = 'Y' THEN 'Y'");
                strQry.AppendLine(" ELSE 'N'");
            }
            else
            {
                strQry.AppendLine(" WHEN ISNULL(WAGE_RUN_IND,'N') = 'Y' THEN 'Y'");
                strQry.AppendLine(" ELSE 'N'");
            }

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC ");
            strQry.AppendLine(" ON C.COMPANY_NO = PCPC.COMPANY_NO");

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

            if (parstrFromProgram != "X")
            {
                strQry.AppendLine(" UNION ");

                strQry.AppendLine(" SELECT DISTINCT ");
                strQry.AppendLine(" COMPLETED_IND = ");

                strQry.AppendLine(" CASE ");


                strQry.AppendLine(" WHEN ISNULL(SALARY_RUN_IND,'N') = 'Y' THEN 'Y'");
                strQry.AppendLine(" ELSE 'N'");

                strQry.AppendLine(" END ");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC ");
                strQry.AppendLine(" ON C.COMPANY_NO = PCPC.COMPANY_NO");
                
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = 'S'");
               
                strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P'");

                strQry.AppendLine(" WHERE C.COMPANY_NO = " + parInt64CompanyNo);
            }
  
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

            if (DataSet.Tables["Temp"].Rows.Count == 0)
            {
                DataSet.Tables.Remove("Temp");

                goto Get_Form_Records_Continue;
            }
            else
            {
                bool blnCompleted = true;
                
                for (int intRow = 0; intRow < DataSet.Tables["Temp"].Rows.Count; intRow++)
                {
                    if (DataSet.Tables["Temp"].Rows[intRow]["COMPLETED_IND"].ToString() == "N")
                    {
                        blnCompleted = false;
                        break;
                    }
                }

                if (blnCompleted == false)
                {
                    DataSet.Tables.Remove("Temp");

                    goto Get_Form_Records_Continue;
                }
            }

            strQry.Clear();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" RUN_TYPE ");
            strQry.AppendLine(",ISNULL(CLOSE_RUN_QUEUE_IND,'S') AS CLOSE_RUN_QUEUE_IND ");

            strQry.AppendLine(" FROM InteractPayroll.dbo.CLOSE_RUN_QUEUE");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND RUN_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND NOT RUN_TYPE = 'T'");
            }
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CloseQueue", -1);
            
            //2017-04-15 Fix IRP5 Code for Leave (Have No Entry field for IRP5 Code)
            strQry.Clear();

            strQry.AppendLine(" UPDATE EARN");

            strQry.AppendLine(" SET EARN.IRP5_CODE = 3601");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EARN");
            strQry.AppendLine(" ON C.COMPANY_NO = EARN.COMPANY_NO");
            //Leave
            strQry.AppendLine(" AND EARN.EARNING_NO >= 200");
            strQry.AppendLine(" AND EARN.IRP5_CODE IS NULL");

            strQry.AppendLine(" WHERE C.COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PC.COMPANY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PCC.PAY_PERIOD_DATE");
            strQry.AppendLine(",PCC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PCC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",MAX(PCH.PAY_PERIOD_DATE) AS PREV_PAY_PERIOD_DATE");
            strQry.AppendLine(",MIN(E.EMPLOYEE_LAST_RUNDATE) AS PREV_EMPLOYEE_LAST_RUNDATE");

            //ELR - 2015-07-02
            strQry.AppendLine(",'' AS RECORDS_EXCLUDED_FROM_RUN");

            //ELR - 2016-07-29
            strQry.AppendLine(",'' AS SALARY_TIMESHEET_UPLOAD_REQUIRED_IND");
                        
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCC");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON PCC.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND PCC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PCC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
        
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON PCC.COMPANY_NO = E.COMPANY_NO");
            strQry.AppendLine(" AND PCC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
            strQry.AppendLine(" ON PC.COMPANY_NO = EPCC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPCC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P'");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCH");
            strQry.AppendLine(" ON PCC.COMPANY_NO = PCH.COMPANY_NO");
            strQry.AppendLine(" AND PCC.PAY_CATEGORY_NO = PCH.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PCC.PAY_CATEGORY_TYPE = PCH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PCH.RUN_TYPE = 'P'");

            strQry.AppendLine(" WHERE PCC.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PCC.PAY_CATEGORY_NO <> 0");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PCC.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PCC.PAY_CATEGORY_TYPE IN ('W','S')");
            }
           
            strQry.AppendLine(" AND PCC.RUN_TYPE = 'P'");
            
            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PC.COMPANY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PCC.PAY_PERIOD_DATE");
            strQry.AppendLine(",PCC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PCC.PAY_CATEGORY_TYPE");
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PC.COMPANY_NO");
            strQry.AppendLine(",PCC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategory", parInt64CompanyNo);

            DataView PayCategoryDataView = new DataView(DataSet.Tables["PayCategory"],
                "PAY_CATEGORY_TYPE IN ('W','T')",
                "",
                DataViewRowState.CurrentRows);

            if (PayCategoryDataView.Count > 0)
            {
                for (int intRow = 0; intRow < PayCategoryDataView.Count; intRow++)
                {
                    if (DataSet.Tables["Check"] != null)
                    {
                        DataSet.Tables.Remove("Check");
                    }

                    strQry.Clear();

                    strQry.AppendLine(" SELECT TOP 1 ");
                    strQry.AppendLine(" ETC.COMPANY_NO");
                  
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    if (PayCategoryDataView[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT AS ETC");
                    }

                    strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + PayCategoryDataView[intRow]["PAY_CATEGORY_NO"].ToString());

                    strQry.AppendLine(" AND ETC.TIMESHEET_DATE > ");

                    strQry.AppendLine(" CASE ");

                    //Errol - 2015-02-17
                    strQry.AppendLine(" WHEN E.FIRST_RUN_COMPLETED_IND = 'Y'");

                    strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE ");

                    strQry.AppendLine(" ELSE DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" END  ");

                    strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= '" + Convert.ToDateTime(PayCategoryDataView[intRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(" AND ETC.INCLUDED_IN_RUN_IND is NULL");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPCC.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P'");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(PayCategoryDataView[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                    strQry.AppendLine(" UNION ");

                    strQry.AppendLine(" SELECT TOP 1 ");
                    strQry.AppendLine(" ETC.COMPANY_NO");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    if (PayCategoryDataView[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT ETC");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT AS ETC");
                    }

                    strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + PayCategoryDataView[intRow]["PAY_CATEGORY_NO"].ToString());

                    strQry.AppendLine(" AND ETC.BREAK_DATE > ");

                    strQry.AppendLine(" CASE ");

                    //Errol - 2015-02-17
                    strQry.AppendLine(" WHEN E.FIRST_RUN_COMPLETED_IND = 'Y'");

                    strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE ");

                    strQry.AppendLine(" ELSE DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" END  ");

                    strQry.AppendLine(" AND ETC.BREAK_DATE <= '" + Convert.ToDateTime(PayCategoryDataView[intRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(" AND ETC.INCLUDED_IN_RUN_IND is NULL");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPCC.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P'");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(PayCategoryDataView[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Check", parInt64CompanyNo);

                    if (DataSet.Tables["Check"].Rows.Count > 0)
                    {
                        PayCategoryDataView[intRow]["RECORDS_EXCLUDED_FROM_RUN"] = "Y";
                    }
                }

                DataSet.AcceptChanges();
            }

            //ELR - 2016-07-29
            PayCategoryDataView = new DataView(DataSet.Tables["PayCategory"],
                "PAY_CATEGORY_TYPE = 'S'",
                "",
                DataViewRowState.CurrentRows);

            if (PayCategoryDataView.Count > 0)
            {
                for (int intRow = 0; intRow < PayCategoryDataView.Count; intRow++)
                {
                    if (DataSet.Tables["Check"] != null)
                    {
                        DataSet.Tables.Remove("Check");
                    }

                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" PCPC.COMPANY_NO");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_HISTORY ESTH ");
                    strQry.AppendLine(" ON PCPC.COMPANY_NO = ESTH.COMPANY_NO ");
                    strQry.AppendLine(" AND DATEADD(d,-1,PCPC.PAY_PERIOD_DATE_FROM) = ESTH.PAY_PERIOD_DATE ");
                    strQry.AppendLine(" AND PCPC.PAY_CATEGORY_NO = ESTH.PAY_CATEGORY_NO ");

                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ESTC ");
                    strQry.AppendLine(" ON PCPC.COMPANY_NO = ESTC.COMPANY_NO ");
                    strQry.AppendLine(" AND PCPC.PAY_CATEGORY_NO = ESTC.PAY_CATEGORY_NO ");

                    strQry.AppendLine(" WHERE PCPC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PCPC.PAY_CATEGORY_NO = " + PayCategoryDataView[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = 'S' ");

                    strQry.AppendLine(" AND ESTC.COMPANY_NO IS NULL ");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Check", parInt64CompanyNo);

                    if (DataSet.Tables["Check"].Rows.Count > 0)
                    {
                        PayCategoryDataView[intRow]["SALARY_TIMESHEET_UPLOAD_REQUIRED_IND"] = "Y";
                      
                        break;
                    }
                }
            }

            if (DataSet.Tables["Check"] != null)
            {
                DataSet.Tables.Remove("Check");
            }

            Get_Form_Records_Continue:

            DataSet.AcceptChanges();

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public int Backup_DataBase(Int64 parInt64CompanyNo,string parstrPayrollType)
        {
            int intReturnCode = 9;
            StringBuilder strQry = new StringBuilder();
          
            try
            {
#if(DEBUG)
                intReturnCode = 0;
#else
                DataSet pvtDataSet = new DataSet();

                strQry.Clear();
                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" BACKUP_DATABASE_PATH");
                strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_PATH");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), pvtDataSet, "Directory", -1);

                string strFileDirectory = pvtDataSet.Tables["Directory"].Rows[0]["BACKUP_DATABASE_PATH"].ToString();
                   
                string strDataBaseName = "InteractPayroll_" + parInt64CompanyNo.ToString("00000");
                string strBackupFileName = strDataBaseName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_BeforeClose.bak";

                strQry.Clear();
                strQry.AppendLine("BACKUP DATABASE " + strDataBaseName + " TO DISK = '" + strFileDirectory + "\\" + strBackupFileName + "' WITH CHECKSUM");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                strQry.Clear();
                strQry.AppendLine("INSERT INTO InteractPayroll.dbo.BACKUP_DATABASE_DATETIME");
                strQry.AppendLine("(BACKUP_DATABASE_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",BACKUP_DATABASE_NAME");
                strQry.AppendLine(",PAYROLL_RUN_DATETIME");
                strQry.AppendLine(",BACKUP_DATETIME");
                strQry.AppendLine(",BACKUP_FILE_NAME)");
              
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" ISNULL(MAX(BDD.BACKUP_DATABASE_NO),0) + 1");
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strDataBaseName));

                strQry.AppendLine(",PCPC.PAY_PERIOD_DATE");
                strQry.AppendLine(",GETDATE()");
                strQry.AppendLine(",'" + strFileDirectory + "\\" + strBackupFileName + "'");
                
                strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_DATETIME BDD");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");
                strQry.AppendLine(" ON PCPC.COMPANY_NO = " + parInt64CompanyNo);

                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

                strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P' ");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" PCPC.PAY_PERIOD_DATE");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                
                intReturnCode = 0;
#endif
            }
            catch (Exception ex)
            {
                string strStop = "";
            }

            return intReturnCode;
        }

        public int Close_Wage_Run(Int64 parInt64CompanyNo,DateTime pardtRunDate, string parstrWagesPayCategoryNumbers, Int64 parint64CurrentUserNo)
        {
            string strClassNameFunctionAndParameters = pvtstrClassName + " Close_Wage_Run CompanyNo=" + parInt64CompanyNo + ",pardtRunDate=" + pardtRunDate.ToString("yyyy-MM-dd") + ",parstrWagesPayCategoryNumbers=" + parstrWagesPayCategoryNumbers + ",parint64CurrentUserNo=" + parint64CurrentUserNo.ToString();

            int intReturnCode = 0;
            StringBuilder strQry = new StringBuilder();

            try
            {
                //2017-08-05
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.CLOSE_RUN_QUEUE");

                strQry.AppendLine(" SET ");
                //S=Started
                strQry.AppendLine(" CLOSE_RUN_QUEUE_IND = 'S'");
                strQry.AppendLine(",START_RUN_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND RUN_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("W"));
                strQry.AppendLine(" AND WAGE_PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(pardtRunDate.ToString("yyyy-MM-dd")));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                
                string[] strPayCategoryArray = parstrWagesPayCategoryNumbers.Split(',');
          
                DataSet DataSet = new DataSet();
                           
                StringBuilder strFieldNamesInitialised = new StringBuilder();
                
                DateTime dtFiscalEndDateTime;
                DateTime dtFiscalBeginDateTime;
                DateTime dtStartLeaveTaxYear;

                //Find End of Fiscal Year
                if (pardtRunDate.Month > 2)
                {
                    dtFiscalBeginDateTime = new DateTime(pardtRunDate.Year, 3, 1);
                }
                else
                {
                    dtFiscalBeginDateTime = new DateTime(pardtRunDate.Year - 1, 3, 1);
                }

                dtFiscalEndDateTime = dtFiscalBeginDateTime.AddYears(1).AddDays(-1);

                //2017-01-11
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" LEAVE_BEGIN_MONTH");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY E");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "LeaveStartDate", parInt64CompanyNo);

                if (DataSet.Tables["LeaveStartDate"].Rows.Count > 0)
                {
                    //Position Within Current Financial Year
                    if (pardtRunDate.Month >= Convert.ToInt32(DataSet.Tables["LeaveStartDate"].Rows[0]["LEAVE_BEGIN_MONTH"]))
                    {
                        dtStartLeaveTaxYear = new DateTime(pardtRunDate.Year, Convert.ToInt32(DataSet.Tables["LeaveStartDate"].Rows[0]["LEAVE_BEGIN_MONTH"]), 1);
                    }
                    else
                    {
                        dtStartLeaveTaxYear = new DateTime(pardtRunDate.Year - 1, Convert.ToInt32(DataSet.Tables["LeaveStartDate"].Rows[0]["LEAVE_BEGIN_MONTH"]), 1);
                    }
                }
                else
                {
                    dtStartLeaveTaxYear = dtFiscalBeginDateTime;
                }
                
                //Run Through Pay Categories
                for (int intRow = 0; intRow < strPayCategoryArray.Length; intRow++)
                {
                    //Insert Last Part of Previous Year (IF Possible) Happens when Payroll covers 2 Different Fiscal Years 
                    //eg x - yyyy-02-28
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",EARNING_NO");
                    strQry.AppendLine(",LEAVE_DESC");

                    strQry.AppendLine(",LEAVE_FROM_DATE");
                    strQry.AppendLine(",LEAVE_TO_DATE");

                    strQry.AppendLine(",LEAVE_ACCUM_DAYS");
                    strQry.AppendLine(",LEAVE_PAID_DAYS");

                    strQry.AppendLine(",LEAVE_OPTION");
                    strQry.AppendLine(",LEAVE_DAYS_DECIMAL");
                    strQry.AppendLine(",LEAVE_HOURS_DECIMAL");
                    strQry.AppendLine(",PROCESS_NO)");

                    ////4-Start
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" E.COMPANY_NO");

                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                                       
                    strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",LH.EARNING_NO");

                    strQry.AppendLine(",'Accumulated Days'");

                    //2013-07-02
                    strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

                    strQry.AppendLine(" CASE ");

                    //Errol - 2015-02-17
                    strQry.AppendLine(" WHEN E.FIRST_RUN_COMPLETED_IND = 'Y'");

                    strQry.AppendLine(" THEN DATEADD(d,1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

                    strQry.AppendLine(" END ");
  
                    //To Last Day in Fiscal Year
                    //strQry.AppendLine(",'" + dtFiscalBeginDateTime.AddDays(-1).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",'" + dtStartLeaveTaxYear.AddDays(-1).ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(",LEAVE_ACCUM_DAYS = ");

                    //Start-Main CASE
                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN LH.EARNING_NO = 200 AND ISNULL(LEAVE_HISTORY_TABLE.LEAVE_ACCUM_DAYS,0) + ROUND(ISNULL(EMPLOYEE_DAY_COUNT_TABLE.DAY_COUNT,0) * LSC.NORM_PAID_PER_PERIOD,2) < LSC.NORM_PAID_DAYS ");
                    strQry.AppendLine(" THEN ROUND(ISNULL(EMPLOYEE_DAY_COUNT_TABLE.DAY_COUNT,0) * LSC.NORM_PAID_PER_PERIOD,2)");

                    strQry.AppendLine(" WHEN LH.EARNING_NO = 200 AND ISNULL(LEAVE_HISTORY_TABLE.LEAVE_ACCUM_DAYS,0) + ROUND(ISNULL(EMPLOYEE_DAY_COUNT_TABLE.DAY_COUNT,0) * LSC.NORM_PAID_PER_PERIOD,2) >= LSC.NORM_PAID_DAYS ");
                   
                    strQry.AppendLine(" THEN CASE ");

                    strQry.AppendLine(" WHEN LEAVE_HISTORY_TABLE.LEAVE_ACCUM_DAYS > LSC.NORM_PAID_DAYS");

                    strQry.AppendLine(" THEN 0");

                    strQry.AppendLine(" ELSE LSC.NORM_PAID_DAYS - LEAVE_HISTORY_TABLE.LEAVE_ACCUM_DAYS");
                                      
                    strQry.AppendLine(" END ");

                    strQry.AppendLine(" WHEN LH.EARNING_NO = 201 AND ISNULL(LEAVE_HISTORY_TABLE.LEAVE_ACCUM_DAYS,0) + ROUND(ISNULL(EMPLOYEE_DAY_COUNT_TABLE.DAY_COUNT,0) * LSC.SICK_PAID_PER_PERIOD,2) < LSC.SICK_PAID_DAYS ");
                    strQry.AppendLine(" THEN ROUND(ISNULL(EMPLOYEE_DAY_COUNT_TABLE.DAY_COUNT,0) * LSC.SICK_PAID_PER_PERIOD,2)");

                    strQry.AppendLine(" WHEN LH.EARNING_NO = 201 AND ISNULL(LEAVE_HISTORY_TABLE.LEAVE_ACCUM_DAYS,0) + ROUND(ISNULL(EMPLOYEE_DAY_COUNT_TABLE.DAY_COUNT,0) * LSC.SICK_PAID_PER_PERIOD,2) >= LSC.SICK_PAID_DAYS ");
                    
                    strQry.AppendLine(" THEN CASE ");

                    strQry.AppendLine(" WHEN LEAVE_HISTORY_TABLE.LEAVE_ACCUM_DAYS > LSC.SICK_PAID_DAYS");

                    strQry.AppendLine(" THEN 0");

                    strQry.AppendLine(" ELSE LSC.SICK_PAID_DAYS - LEAVE_HISTORY_TABLE.LEAVE_ACCUM_DAYS");

                    strQry.AppendLine(" END ");

                    //End-Main CASE
                    strQry.AppendLine(" END ");

                    strQry.AppendLine(",0");

                    strQry.AppendLine(",'D'");
                    strQry.AppendLine(",ISNULL(EMPLOYEE_DAY_COUNT_TABLE.DAY_COUNT,0)");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",98");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EIC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EIC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EIC.PAY_CATEGORY_TYPE");
                    //Leave Accumulated even if Employee does Not get Payslip
                    //strQry.AppendLine(" AND EIC.PAYSLIP_IND = 'Y'");
                    strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");

                    //2014-03-29
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC ");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");

                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE ");

                    strQry.AppendLine(" AND EIC.RUN_TYPE = EPCC.RUN_TYPE");
                    //Default For Leave
                    strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y'");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY LH");

                    strQry.AppendLine(" ON E.COMPANY_NO = LH.COMPANY_NO ");
                    strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND LH.EARNING_NO IN (200,201)");
                    //Take-On
                    strQry.AppendLine(" AND LH.PROCESS_NO = 99");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT_CURRENT LSC");
                    strQry.AppendLine(" ON E.COMPANY_NO = LSC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.LEAVE_SHIFT_NO = LSC.LEAVE_SHIFT_NO ");
                    strQry.AppendLine(" AND LSC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = LSC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" LEFT JOIN ");

                    strQry.AppendLine("( ");

                    strQry.AppendLine(" SELECT ");

                    strQry.AppendLine(" LH.COMPANY_NO");
                    strQry.AppendLine(",LH.EMPLOYEE_NO");
                    strQry.AppendLine(",LH.EARNING_NO");
                    strQry.AppendLine(",SUM(LH.LEAVE_ACCUM_DAYS) AS LEAVE_ACCUM_DAYS");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY LH");

                    strQry.AppendLine(" WHERE LH.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = 'W'");
                    //Go Back 1 Year
                    strQry.AppendLine(" AND LH.PAY_PERIOD_DATE >= '" + dtStartLeaveTaxYear.AddYears(-1).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND LH.PAY_PERIOD_DATE < '" + dtStartLeaveTaxYear.ToString("yyyy-MM-dd") + "'");

                    //Accumulated Leave / Take-On
                    strQry.AppendLine(" AND LH.PROCESS_NO IN (98,99)");
                    //Normal Leave / Sick Leave
                    strQry.AppendLine(" AND LH.EARNING_NO IN (200,201)");

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" LH.COMPANY_NO");
                    strQry.AppendLine(",LH.EMPLOYEE_NO");
                    strQry.AppendLine(",LH.EARNING_NO) AS LEAVE_HISTORY_TABLE");

                    strQry.AppendLine(" ON E.COMPANY_NO = LEAVE_HISTORY_TABLE.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = LEAVE_HISTORY_TABLE.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND LH.EARNING_NO = LEAVE_HISTORY_TABLE.EARNING_NO ");

                    strQry.AppendLine(" LEFT JOIN ");

                    strQry.AppendLine("( ");
                    //3-Start

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" TEMP_TABLE1.COMPANY_NO");
                    strQry.AppendLine(",TEMP_TABLE1.EMPLOYEE_NO");
                    strQry.AppendLine(",COUNT(TEMP_TABLE1.TIMESHEET_DATE) AS DAY_COUNT");

                    strQry.AppendLine(" FROM ");

                    strQry.AppendLine("(");

                    //2-Start
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    //NB This Is the DEfault Pay Category for Employee and ALL Timesheets Across PayCategories are SUMMED
                    strQry.AppendLine(",LSC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",E.LEAVE_SHIFT_NO");
                    strQry.AppendLine(",TEMP_TIMESHEET_TABLE.TIMESHEET_DATE");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN ");

                    strQry.AppendLine("( ");
                    //1-Start
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" ETC.COMPANY_NO ");
                    strQry.AppendLine(",ETC.EMPLOYEE_NO ");
                    strQry.AppendLine(",ETC.TIMESHEET_DATE");
                    strQry.AppendLine(",SUM(ETC.TIMESHEET_TIME_OUT_MINUTES) - SUM(ETC.TIMESHEET_TIME_IN_MINUTES) AS DAY_WORKED_MINUTES");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO");
                    strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");

                    strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parInt64CompanyNo);
                    //strQry.AppendLine(" AND ETC.TIMESHEET_DATE > E.EMPLOYEE_LAST_RUNDATE");

                    //2013-07-02
                    strQry.AppendLine(" AND ETC.TIMESHEET_DATE > ");

                    strQry.AppendLine(" CASE ");

                    //Errol - 2015-02-17
                    strQry.AppendLine(" WHEN E.FIRST_RUN_COMPLETED_IND = 'Y'");

                    strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE  ");

                    strQry.AppendLine(" ELSE DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" END ");
                    
                    strQry.AppendLine(" AND ETC.TIMESHEET_DATE < '" + dtStartLeaveTaxYear.ToString("yyyy-MM-dd") + "'");
                   
                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" ETC.COMPANY_NO ");
                    strQry.AppendLine(",ETC.EMPLOYEE_NO ");
                    strQry.AppendLine(",ETC.TIMESHEET_DATE");
                    //1-End
                    strQry.AppendLine(") AS TEMP_TIMESHEET_TABLE");

                    strQry.AppendLine(" ON E.COMPANY_NO = TEMP_TIMESHEET_TABLE.COMPANY_NO");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = TEMP_TIMESHEET_TABLE.EMPLOYEE_NO");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT_CURRENT LSC");
                    strQry.AppendLine(" ON E.COMPANY_NO = LSC.COMPANY_NO");
                    strQry.AppendLine(" AND LSC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = LSC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND E.LEAVE_SHIFT_NO = LSC.LEAVE_SHIFT_NO");
                    strQry.AppendLine(" AND TEMP_TIMESHEET_TABLE.DAY_WORKED_MINUTES >= LSC.MIN_VALID_SHIFT_MINUTES");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL ");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    //2-End
                    strQry.AppendLine(") AS TEMP_TABLE1");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D ");
                    strQry.AppendLine(" ON TEMP_TABLE1.TIMESHEET_DATE = D.DAY_DATE");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT_CURRENT LS");

                    strQry.AppendLine(" ON LS.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND TEMP_TABLE1.PAY_CATEGORY_NO = LS.PAY_CATEGORY_NO");
                    //Wages - WHAT ABOUT SALARIES
                    strQry.AppendLine(" AND LS.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND TEMP_TABLE1.LEAVE_SHIFT_NO = LS.LEAVE_SHIFT_NO");
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
                    strQry.AppendLine(" AND PCPC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND PCPC.PAY_PUBLIC_HOLIDAY_IND = 'Y'");

                    strQry.AppendLine(" WHERE PHC.COMPANY_NO = " + parInt64CompanyNo + ") AS TEMP_TABLE2");

                    strQry.AppendLine(" ON LS.PAY_CATEGORY_NO = TEMP_TABLE2.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND  D.DAY_DATE = TEMP_TABLE2.PUBLIC_HOLIDAY_DATE");

                    //Not Public Holiday
                    strQry.AppendLine(" WHERE TEMP_TABLE2.PAY_CATEGORY_NO IS NULL");

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" TEMP_TABLE1.COMPANY_NO");
                    strQry.AppendLine(",TEMP_TABLE1.EMPLOYEE_NO");
                    //3-End

                    strQry.AppendLine(" ) AS EMPLOYEE_DAY_COUNT_TABLE ");

                    strQry.AppendLine(" ON E.COMPANY_NO = EMPLOYEE_DAY_COUNT_TABLE.COMPANY_NO");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EMPLOYEE_DAY_COUNT_TABLE.EMPLOYEE_NO");

                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY LHT");

                    strQry.AppendLine(" ON E.COMPANY_NO = LHT.COMPANY_NO ");
                    strQry.AppendLine(" AND LHT.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND LH.EARNING_NO = LHT.EARNING_NO");
                    strQry.AppendLine(" AND LHT.PROCESS_NO = 98");
                    //Last Day of Fiscal Year
                    strQry.AppendLine(" AND LHT.LEAVE_TO_DATE = '" + dtStartLeaveTaxYear.AddDays(-1).ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    //2012-11-20
                    strQry.AppendLine(" AND E.EMPLOYEE_LAST_RUNDATE < '" + dtStartLeaveTaxYear.ToString("yyyy-MM-dd") + "'");
                   
                    //NO Record for Close Off of Normal Leave (End of Fiscal Year)
                    strQry.AppendLine(" AND LHT.COMPANY_NO IS NULL ");

                    strQry.AppendLine(" GROUP BY ");

                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",E.EMPLOYEE_LAST_RUNDATE");
                    //Errol - 2015-02-18
                    //strQry.AppendLine(",EIHT.PAY_PERIOD_DATE");
                    //Errol - 2015-02-18
                    strQry.AppendLine(",E.FIRST_RUN_COMPLETED_IND");

                    strQry.AppendLine(",LH.EARNING_NO");
                    strQry.AppendLine(",LSC.NORM_PAID_PER_PERIOD ");
                    strQry.AppendLine(",LSC.MAX_SHIFTS_YEAR ");
                    strQry.AppendLine(",LSC.NORM_PAID_DAYS ");
                    strQry.AppendLine(",LSC.SICK_PAID_DAYS ");
                    strQry.AppendLine(",LSC.SICK_PAID_PER_PERIOD ");
                    strQry.AppendLine(",EMPLOYEE_DAY_COUNT_TABLE.DAY_COUNT");
                    strQry.AppendLine(",LEAVE_HISTORY_TABLE.LEAVE_ACCUM_DAYS");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                    
                    //Insert Current Portion For Pay Period or In some cases start of Fiscal Year to Pay Period Date
                    //eg 1) Employee Last Rundate + 1 Day to Pay Period Date
                    //   2) yyyy-03-01 to Pay Period Date
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",EARNING_NO");
                   
                    strQry.AppendLine(",LEAVE_DESC");

                    strQry.AppendLine(",LEAVE_FROM_DATE");
                    strQry.AppendLine(",LEAVE_TO_DATE");

                    strQry.AppendLine(",LEAVE_ACCUM_DAYS");
                    strQry.AppendLine(",LEAVE_PAID_DAYS");

                    strQry.AppendLine(",LEAVE_OPTION");
                    strQry.AppendLine(",LEAVE_DAYS_DECIMAL");
                    strQry.AppendLine(",LEAVE_HOURS_DECIMAL");
                    strQry.AppendLine(",PROCESS_NO)");
                  
                    ////4-Start
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" E.COMPANY_NO");

                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",LH.EARNING_NO");

                    strQry.AppendLine(",'Accumulated Days'");

                    strQry.AppendLine(",FROM_RUNDATE = ");
                    
                    strQry.AppendLine(" CASE ");
                    //Run Stretches over 2 Different Fiscal Years
                    strQry.AppendLine(" WHEN E.EMPLOYEE_LAST_RUNDATE < '" + dtStartLeaveTaxYear.ToString("yyyy-MM-dd") + "'"); 
                    
                    strQry.AppendLine(" THEN '" + dtStartLeaveTaxYear.ToString("yyyy-MM-dd") + "'");

                    //Errol - 2015-02-17
                    strQry.AppendLine(" WHEN E.FIRST_RUN_COMPLETED_IND = 'Y'");

                    strQry.AppendLine(" THEN  DATEADD(DD,1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

                    strQry.AppendLine(" END ");

                    //To Date
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    
                    strQry.AppendLine(",LEAVE_ACCUM_DAYS = ");
                  
                    strQry.AppendLine(" CASE ");

                    //strQry.AppendLine(" WHEN LH.EARNING_NO = 200 AND ISNULL(LEAVE_HISTORY_TABLE.LEAVE_ACCUM_DAYS,0) + ROUND(ISNULL(EMPLOYEE_DAY_COUNT_TABLE.DAY_COUNT,0) * LSC.NORM_PAID_PER_PERIOD,2) < LSC.MAX_SHIFTS_YEAR ");

                    strQry.AppendLine(" WHEN LH.EARNING_NO = 200 AND ISNULL(LEAVE_HISTORY_TABLE.LEAVE_ACCUM_DAYS,0) + ROUND(ISNULL(EMPLOYEE_DAY_COUNT_TABLE.DAY_COUNT,0) * LSC.NORM_PAID_PER_PERIOD,2) < LSC.NORM_PAID_DAYS ");
                    strQry.AppendLine(" THEN ROUND(ISNULL(EMPLOYEE_DAY_COUNT_TABLE.DAY_COUNT,0) * LSC.NORM_PAID_PER_PERIOD,2)");

                    strQry.AppendLine(" WHEN LH.EARNING_NO = 200 AND ISNULL(LEAVE_HISTORY_TABLE.LEAVE_ACCUM_DAYS,0) + ROUND(ISNULL(EMPLOYEE_DAY_COUNT_TABLE.DAY_COUNT,0) * LSC.NORM_PAID_PER_PERIOD,2) >= LSC.NORM_PAID_DAYS ");
                    
                    strQry.AppendLine(" THEN CASE");

                    strQry.AppendLine(" WHEN LEAVE_HISTORY_TABLE.LEAVE_ACCUM_DAYS > LSC.NORM_PAID_DAYS");

                    strQry.AppendLine(" THEN 0");

                    strQry.AppendLine(" ELSE LSC.NORM_PAID_DAYS - LEAVE_HISTORY_TABLE.LEAVE_ACCUM_DAYS");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(" WHEN LH.EARNING_NO = 201 AND ISNULL(LEAVE_HISTORY_TABLE.LEAVE_ACCUM_DAYS,0) + ROUND(ISNULL(EMPLOYEE_DAY_COUNT_TABLE.DAY_COUNT,0) * LSC.SICK_PAID_PER_PERIOD,2) < LSC.SICK_PAID_DAYS ");
                    strQry.AppendLine(" THEN ROUND(ISNULL(EMPLOYEE_DAY_COUNT_TABLE.DAY_COUNT,0) * LSC.SICK_PAID_PER_PERIOD,2)");

                    strQry.AppendLine(" WHEN LH.EARNING_NO = 201 AND ISNULL(LEAVE_HISTORY_TABLE.LEAVE_ACCUM_DAYS,0) + ROUND(ISNULL(EMPLOYEE_DAY_COUNT_TABLE.DAY_COUNT,0) * LSC.SICK_PAID_PER_PERIOD,2) >= LSC.SICK_PAID_DAYS ");
                    
                    strQry.AppendLine(" THEN CASE");

                    strQry.AppendLine(" WHEN LEAVE_HISTORY_TABLE.LEAVE_ACCUM_DAYS > LSC.SICK_PAID_DAYS");
                    strQry.AppendLine(" THEN 0");

                    strQry.AppendLine(" ELSE LSC.SICK_PAID_DAYS - LEAVE_HISTORY_TABLE.LEAVE_ACCUM_DAYS");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(" END ");
                    
                    strQry.AppendLine(",0");

                    strQry.AppendLine(",'D'");
                    strQry.AppendLine(",ISNULL(EMPLOYEE_DAY_COUNT_TABLE.DAY_COUNT,0)");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",98");
                   
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EIC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EIC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EIC.PAY_CATEGORY_TYPE");
                    //Leave Accumulated even if Employee does Not get Payslip
                    //strQry.AppendLine(" AND EIC.PAYSLIP_IND = 'Y'");
                    strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");

                    //2014-03-29
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC ");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");

                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE ");

                    strQry.AppendLine(" AND EIC.RUN_TYPE = EPCC.RUN_TYPE");
                    //Default For Leave
                    strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y'");
                  
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY LH");

                    strQry.AppendLine(" ON E.COMPANY_NO = LH.COMPANY_NO ");
                    strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = 'W'");
                    //Only Get 1 Record
                    strQry.AppendLine(" AND LH.EARNING_NO IN (200,201)");
                    //Take-On
                    strQry.AppendLine(" AND LH.PROCESS_NO = 99");
                    
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT_CURRENT LSC");
                    strQry.AppendLine(" ON E.COMPANY_NO = LSC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.LEAVE_SHIFT_NO = LSC.LEAVE_SHIFT_NO ");
                    strQry.AppendLine(" AND LSC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = LSC.PAY_CATEGORY_TYPE");
                    
                    strQry.AppendLine(" LEFT JOIN ");

                    strQry.AppendLine("( ");
                    
                    strQry.AppendLine(" SELECT ");
                   
                    strQry.AppendLine(" LH.COMPANY_NO");
                    strQry.AppendLine(",LH.EMPLOYEE_NO");
                    strQry.AppendLine(",LH.EARNING_NO");
                    strQry.AppendLine(",SUM(LH.LEAVE_ACCUM_DAYS) AS LEAVE_ACCUM_DAYS");  
                    
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY LH");

                    strQry.AppendLine(" WHERE LH.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND LH.PAY_PERIOD_DATE >= '" + dtStartLeaveTaxYear.ToString("yyyy-MM-dd") + "'");

                    //Accumulated Leave / Take-On
                    strQry.AppendLine(" AND LH.PROCESS_NO IN (98,99)");
                    //Normal Leave / Sick Leave
                    strQry.AppendLine(" AND LH.EARNING_NO IN (200,201)");

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" LH.COMPANY_NO");
                    strQry.AppendLine(",LH.EMPLOYEE_NO");
                    strQry.AppendLine(",LH.EARNING_NO) AS LEAVE_HISTORY_TABLE");

                    strQry.AppendLine(" ON E.COMPANY_NO = LEAVE_HISTORY_TABLE.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = LEAVE_HISTORY_TABLE.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND LH.EARNING_NO = LEAVE_HISTORY_TABLE.EARNING_NO ");
                    
                    strQry.AppendLine(" LEFT JOIN ");

                    strQry.AppendLine("( ");
                    //3-Start

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" TEMP_TABLE1.COMPANY_NO");
                    strQry.AppendLine(",TEMP_TABLE1.EMPLOYEE_NO");
                    strQry.AppendLine(",COUNT(TEMP_TABLE1.TIMESHEET_DATE) AS DAY_COUNT");

                    strQry.AppendLine(" FROM ");

                    strQry.AppendLine("(");

                    //2-Start
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    //NB This Is the DEfault Pay Category for Employee and ALL Timesheets Across PayCategories are SUMMED
                    strQry.AppendLine(",LSC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",E.LEAVE_SHIFT_NO");
                    strQry.AppendLine(",TEMP_TIMESHEET_TABLE.TIMESHEET_DATE");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN ");

                    strQry.AppendLine("( ");
                    //1-Start
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" ETC.COMPANY_NO ");
                    strQry.AppendLine(",ETC.EMPLOYEE_NO ");
                    strQry.AppendLine(",ETC.TIMESHEET_DATE");
                    strQry.AppendLine(",SUM(ETC.TIMESHEET_TIME_OUT_MINUTES) - SUM(ETC.TIMESHEET_TIME_IN_MINUTES) AS DAY_WORKED_MINUTES");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO");
                    strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");

                    strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parInt64CompanyNo);

                    //Errol 2013-07-02
                    //strQry.AppendLine(" AND ETC.TIMESHEET_DATE > E.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ETC.TIMESHEET_DATE > ");
                    
                    strQry.AppendLine(" CASE ");

                    //Errol - 2015-02-17
                    strQry.AppendLine(" WHEN E.FIRST_RUN_COMPLETED_IND = 'Y'");

                    strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE   ");

                    strQry.AppendLine(" ELSE DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(" AND ETC.TIMESHEET_DATE >= '" + dtStartLeaveTaxYear.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" ETC.COMPANY_NO ");
                    strQry.AppendLine(",ETC.EMPLOYEE_NO ");
                    strQry.AppendLine(",ETC.TIMESHEET_DATE");
                    //1-End
                    strQry.AppendLine(") AS TEMP_TIMESHEET_TABLE");

                    strQry.AppendLine(" ON E.COMPANY_NO = TEMP_TIMESHEET_TABLE.COMPANY_NO");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = TEMP_TIMESHEET_TABLE.EMPLOYEE_NO");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT_CURRENT LSC");
                    strQry.AppendLine(" ON E.COMPANY_NO = LSC.COMPANY_NO");
                    strQry.AppendLine(" AND LSC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = LSC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND E.LEAVE_SHIFT_NO = LSC.LEAVE_SHIFT_NO");
                    strQry.AppendLine(" AND TEMP_TIMESHEET_TABLE.DAY_WORKED_MINUTES >= LSC.MIN_VALID_SHIFT_MINUTES");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL ");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    //2-End
                    strQry.AppendLine(") AS TEMP_TABLE1");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D ");
                    strQry.AppendLine(" ON TEMP_TABLE1.TIMESHEET_DATE = D.DAY_DATE");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT_CURRENT LS");

                    strQry.AppendLine(" ON LS.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND TEMP_TABLE1.PAY_CATEGORY_NO = LS.PAY_CATEGORY_NO");
                    //Wages - WHAT ABOUT SALARIES
                    strQry.AppendLine(" AND LS.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND TEMP_TABLE1.LEAVE_SHIFT_NO = LS.LEAVE_SHIFT_NO");
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
                    strQry.AppendLine(" AND PCPC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND PCPC.PAY_PUBLIC_HOLIDAY_IND = 'Y'");

                    strQry.AppendLine(" WHERE PHC.COMPANY_NO = " + parInt64CompanyNo + ") AS TEMP_TABLE2");

                    strQry.AppendLine(" ON LS.PAY_CATEGORY_NO = TEMP_TABLE2.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND  D.DAY_DATE = TEMP_TABLE2.PUBLIC_HOLIDAY_DATE");

                    //Not Public Holiday
                    strQry.AppendLine(" WHERE TEMP_TABLE2.PAY_CATEGORY_NO IS NULL");

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" TEMP_TABLE1.COMPANY_NO");
                    strQry.AppendLine(",TEMP_TABLE1.EMPLOYEE_NO");
                    //3-End

                    strQry.AppendLine(" ) AS EMPLOYEE_DAY_COUNT_TABLE ");

                    strQry.AppendLine(" ON E.COMPANY_NO = EMPLOYEE_DAY_COUNT_TABLE.COMPANY_NO");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EMPLOYEE_DAY_COUNT_TABLE.EMPLOYEE_NO");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    
                    strQry.AppendLine(" GROUP BY ");

                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    //Errol - 2015-02-18
                    //strQry.AppendLine(",EIHT.PAY_PERIOD_DATE");
                    //Errol - 2015-02-18
                    strQry.AppendLine(",E.FIRST_RUN_COMPLETED_IND");

                    strQry.AppendLine(",E.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(",LH.EARNING_NO");
                    strQry.AppendLine(",LSC.NORM_PAID_PER_PERIOD ");
                    strQry.AppendLine(",LSC.MAX_SHIFTS_YEAR ");
                    strQry.AppendLine(",LSC.NORM_PAID_DAYS ");
                    strQry.AppendLine(",LSC.SICK_PAID_DAYS ");
                    strQry.AppendLine(",LSC.SICK_PAID_PER_PERIOD ");
                    strQry.AppendLine(",EMPLOYEE_DAY_COUNT_TABLE.DAY_COUNT");
                    strQry.AppendLine(",LEAVE_HISTORY_TABLE.LEAVE_ACCUM_DAYS");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
               
                    //Pay Out Normal Leave - EMPLOYEE Closed
                    //Pay Out Normal Leave - EMPLOYEE Closed
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",EARNING_NO");
                    
                    strQry.AppendLine(",LEAVE_DESC");
                    strQry.AppendLine(",LEAVE_FROM_DATE");
                    strQry.AppendLine(",LEAVE_TO_DATE");

                    strQry.AppendLine(",LEAVE_ACCUM_DAYS");
                    strQry.AppendLine(",LEAVE_PAID_DAYS");

                    strQry.AppendLine(",LEAVE_OPTION");
                    strQry.AppendLine(",LEAVE_DAYS_DECIMAL");
                    strQry.AppendLine(",LEAVE_HOURS_DECIMAL");
                    strQry.AppendLine(",PROCESS_NO)");
                                  
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EIC.COMPANY_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",EIC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EIC.EMPLOYEE_NO");
                    strQry.AppendLine(",EEC.EARNING_NO");
                    
                    strQry.AppendLine(",'via Payroll Run (Closed)'");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(",0");
                    strQry.AppendLine(",EEC.DAY_DECIMAL_OTHER_VALUE");
             
                    //Errol - 2012-01-11
                    strQry.AppendLine(",'D'");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",EEC.HOURS_DECIMAL_OTHER_VALUE");

                    strQry.AppendLine(",97");
                    
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC");

                    //Normal Leave - Total Oustanding Amount
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC ");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = EEC.COMPANY_NO");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EEC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EEC.EARNING_NO = 200");
                    strQry.AppendLine(" AND EEC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]); 
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EEC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EIC.RUN_TYPE = EEC.RUN_TYPE");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC ");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");

                    strQry.AppendLine(" AND EEC.PAY_CATEGORY_NO = EPCC.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE ");
                    
                    strQry.AppendLine(" AND EIC.RUN_TYPE = EPCC.RUN_TYPE");
                    //Default For Leave
                    strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y'");
                  
                    strQry.AppendLine(" WHERE EIC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = 'W'");
                    //Employee is To Be Closed
                    strQry.AppendLine(" AND EIC.CLOSE_IND = 'Y'");

                    strQry.AppendLine(" ORDER BY ");
                    strQry.AppendLine(" EPCC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EIC.EMPLOYEE_NO");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                    
                    //Move LEAVE_CURRENT to LEAVE_HISTORY
                    //Move LEAVE_CURRENT to LEAVE_HISTORY
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",EARNING_NO");
                    //strQry.AppendLine(",LEAVE_REC_NO");
                    strQry.AppendLine(",LEAVE_DESC");

                    strQry.AppendLine(",LEAVE_FROM_DATE");
                    strQry.AppendLine(",LEAVE_TO_DATE");

                    strQry.AppendLine(",LEAVE_ACCUM_DAYS");
                    strQry.AppendLine(",LEAVE_PAID_DAYS");

                    strQry.AppendLine(",LEAVE_OPTION");
                    strQry.AppendLine(",LEAVE_DAYS_DECIMAL");
                    strQry.AppendLine(",LEAVE_HOURS_DECIMAL");
                    strQry.AppendLine(",PROCESS_NO)");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EIC.COMPANY_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",EIC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",L.EMPLOYEE_NO");
                    strQry.AppendLine(",L.EARNING_NO");
                    //strQry.AppendLine(",100 + L.LEAVE_REC_NO");
                    strQry.AppendLine(",L.LEAVE_DESC");

                    strQry.AppendLine(",L.LEAVE_FROM_DATE");
                    strQry.AppendLine(",L.LEAVE_TO_DATE");

                    strQry.AppendLine(",0");
                    //NB LEAVE_DAYS_DECIMAL Gets moved to LEAVE_PAID_DAYS
                    strQry.AppendLine(",L.LEAVE_DAYS_DECIMAL");

                    strQry.AppendLine(",L.LEAVE_OPTION");
                    strQry.AppendLine(",L.LEAVE_DAYS_DECIMAL");
                    strQry.AppendLine(",LEAVE_HOURS_DECIMAL");

                    strQry.AppendLine(",L.PROCESS_NO");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y'");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT L");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = L.COMPANY_NO");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = L.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = L.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND L.PROCESS_NO = 0");
                    //Not Payout Normal Leave or Zerorize Leave
                    strQry.AppendLine(" AND NOT L.LEAVE_OPTION in ('P','Z')");
                    
                    strQry.AppendLine(" WHERE EIC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = 'W'");
                    //Gets a Payslip
                    strQry.AppendLine(" AND (EIC.PAYSLIP_IND = 'Y'");
                    strQry.AppendLine(" OR EIC.CLOSE_IND = 'Y')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //2016-12-12 PayOut Normal Leave
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",EARNING_NO");
                    strQry.AppendLine(",LEAVE_DESC");

                    strQry.AppendLine(",LEAVE_FROM_DATE");
                    strQry.AppendLine(",LEAVE_TO_DATE");

                    strQry.AppendLine(",LEAVE_ACCUM_DAYS");
                    strQry.AppendLine(",LEAVE_PAID_DAYS");

                    strQry.AppendLine(",LEAVE_OPTION");
                    strQry.AppendLine(",LEAVE_DAYS_DECIMAL");
                    strQry.AppendLine(",LEAVE_HOURS_DECIMAL");
                    strQry.AppendLine(",PROCESS_NO)");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EIC.COMPANY_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",EIC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",L.EMPLOYEE_NO");
                    strQry.AppendLine(",L.EARNING_NO");

                    strQry.AppendLine(",L.LEAVE_DESC");

                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(",0");
                    strQry.AppendLine(",EEC.DAY_DECIMAL_OTHER_VALUE");

                    strQry.AppendLine(",L.LEAVE_OPTION");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",EEC.HOURS_DECIMAL");

                    strQry.AppendLine(",L.PROCESS_NO");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y'");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT L");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = L.COMPANY_NO");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = L.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = L.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND L.PROCESS_NO = 0");
                    //Payout Leave Due
                    strQry.AppendLine(" AND L.LEAVE_OPTION = 'P'");
                    //Normal Leave
                    strQry.AppendLine(" AND L.EARNING_NO = 200");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");
                    strQry.AppendLine(" ON EPCC.COMPANY_NO = EEC.COMPANY_NO ");
                    strQry.AppendLine(" AND EPCC.EMPLOYEE_NO = EEC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND L.EARNING_NO = EEC.EARNING_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = EEC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = EEC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EIC.RUN_TYPE = EEC.RUN_TYPE");

                    strQry.AppendLine(" WHERE EIC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = 'W'");
                    //Gets a Payslip
                    strQry.AppendLine(" AND (EIC.PAYSLIP_IND = 'Y'");
                    //Not Close of Employee
                    strQry.AppendLine(" AND EIC.CLOSE_IND <> 'Y')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                    
                    //2016-12-12 Zerorize Leave
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",EARNING_NO");
                    //strQry.AppendLine(",LEAVE_REC_NO");
                    strQry.AppendLine(",LEAVE_DESC");

                    strQry.AppendLine(",LEAVE_FROM_DATE");
                    strQry.AppendLine(",LEAVE_TO_DATE");

                    strQry.AppendLine(",LEAVE_ACCUM_DAYS");
                    strQry.AppendLine(",LEAVE_PAID_DAYS");

                    strQry.AppendLine(",LEAVE_OPTION");
                    strQry.AppendLine(",LEAVE_DAYS_DECIMAL");
                    strQry.AppendLine(",LEAVE_HOURS_DECIMAL");
                    strQry.AppendLine(",PROCESS_NO)");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EIC.COMPANY_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",EIC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",L.EMPLOYEE_NO");
                    strQry.AppendLine(",L.EARNING_NO");

                    strQry.AppendLine(",L.LEAVE_DESC");

                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(",0");
                    //NB LEAVE_DAYS_DECIMAL Gets moved to LEAVE_PAID_DAYS
                    strQry.AppendLine(",SUM(ROUND(LH.LEAVE_ACCUM_DAYS - LH.LEAVE_PAID_DAYS, 2)) ");

                    strQry.AppendLine(",L.LEAVE_OPTION");
                    strQry.AppendLine(",0");
                    //2012-10-10 Errol Changed 0 to LEAVE_HOURS_DECIMAL
                    strQry.AppendLine(",0");

                    strQry.AppendLine(",L.PROCESS_NO");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y'");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT L");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = L.COMPANY_NO");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = L.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = L.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND L.PROCESS_NO = 0");
                    //Zerorize Leave Due
                    strQry.AppendLine(" AND L.LEAVE_OPTION = 'Z'");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY LH");
                    strQry.AppendLine(" ON EPCC.COMPANY_NO = LH.COMPANY_NO ");
                    strQry.AppendLine(" AND EPCC.EMPLOYEE_NO = LH.EMPLOYEE_NO");
                    strQry.AppendLine(" AND L.EARNING_NO = LH.EARNING_NO");

                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = LH.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND LH.LEAVE_FROM_DATE >= '" + dtFiscalBeginDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND((LH.PROCESS_NO = 98");
                    strQry.AppendLine(" AND LH.EARNING_NO = 201)");
                    strQry.AppendLine(" OR LH.PROCESS_NO <> 98)");
                    
                    strQry.AppendLine(" WHERE EIC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = 'W'");
                    //Gets a Payslip
                    strQry.AppendLine(" AND (EIC.PAYSLIP_IND = 'Y'");
                    strQry.AppendLine(" OR EIC.CLOSE_IND = 'Y')");

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" EIC.COMPANY_NO");
                    strQry.AppendLine(",EIC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",L.EMPLOYEE_NO");
                    strQry.AppendLine(",L.EARNING_NO");
                    strQry.AppendLine(",L.LEAVE_DESC");
                    strQry.AppendLine(",L.LEAVE_OPTION");
                    strQry.AppendLine(",L.PROCESS_NO");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //Delete LEAVE_CURRENT that was Moved to LEAVE_HISTORY
                    //Delete LEAVE_CURRENT that was Moved to LEAVE_HISTORY
                    strQry.Clear();
                    strQry.AppendLine(" DELETE LC");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT LC");

                    strQry.AppendLine(" INNER JOIN ");

                    strQry.AppendLine("(SELECT");

                    strQry.AppendLine(" L.COMPANY_NO");
                    strQry.AppendLine(",L.EMPLOYEE_NO");
                    strQry.AppendLine(",L.EARNING_NO");
                    strQry.AppendLine(",L.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",L.LEAVE_REC_NO");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y'");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT L");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = L.COMPANY_NO");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = L.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = L.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND L.PROCESS_NO = 0");

                    strQry.AppendLine(" WHERE EIC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = 'W'");

                    //Gets a Payslip
                    strQry.AppendLine(" AND (EIC.PAYSLIP_IND = 'Y'");
                    strQry.AppendLine(" OR EIC.CLOSE_IND = 'Y')) AS LEAVE_TEMP_TABLE");

                    strQry.AppendLine(" ON LC.COMPANY_NO = LEAVE_TEMP_TABLE.COMPANY_NO");
                    strQry.AppendLine(" AND LC.EMPLOYEE_NO = LEAVE_TEMP_TABLE.EMPLOYEE_NO");
                    strQry.AppendLine(" AND LC.EARNING_NO = LEAVE_TEMP_TABLE.EARNING_NO");
                    strQry.AppendLine(" AND LC.PAY_CATEGORY_TYPE = LEAVE_TEMP_TABLE.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND LC.LEAVE_REC_NO = LEAVE_TEMP_TABLE.LEAVE_REC_NO");

                    strQry.AppendLine(" WHERE LC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND LC.PAY_CATEGORY_TYPE = 'W'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                                        
                    strQry.Clear();
                    strQry.AppendLine(" DELETE EEC FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC ");
                    strQry.AppendLine(" ON EEC.COMPANY_NO = EIC.COMPANY_NO ");
                    strQry.AppendLine(" AND EEC.EMPLOYEE_NO = EIC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EEC.RUN_TYPE = EIC.RUN_TYPE");
                    strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = EIC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EEC.RUN_NO = EIC.RUN_NO ");
                    //No Payslip
                    strQry.AppendLine(" AND EIC.PAYSLIP_IND = 'N'");
                    strQry.AppendLine(" AND EIC.CLOSE_IND <> 'Y'");

                    strQry.AppendLine(" WHERE EEC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EEC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND EEC.RUN_TYPE = 'P'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE EEWC FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_WEEK_CURRENT EEWC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC ");
                    strQry.AppendLine(" ON EEWC.COMPANY_NO = EIC.COMPANY_NO ");
                    strQry.AppendLine(" AND EEWC.EMPLOYEE_NO = EIC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EEWC.RUN_NO = EIC.RUN_NO ");
                    //No Payslip
                    strQry.AppendLine(" AND EIC.PAYSLIP_IND = 'N'");
                    strQry.AppendLine(" AND EIC.CLOSE_IND <> 'Y'");
                    strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");

                    strQry.AppendLine(" WHERE EEWC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EEWC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE EPCC FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC ");
                    strQry.AppendLine(" ON EPCC.COMPANY_NO = EIC.COMPANY_NO ");
                    strQry.AppendLine(" AND EPCC.EMPLOYEE_NO = EIC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = EIC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EPCC.RUN_TYPE = EIC.RUN_TYPE ");
                    strQry.AppendLine(" AND EPCC.RUN_NO = EIC.RUN_NO ");
                    //No Payslip
                    strQry.AppendLine(" AND EIC.PAYSLIP_IND = 'N'");
                    strQry.AppendLine(" AND EIC.CLOSE_IND <> 'Y'");

                    strQry.AppendLine(" WHERE EPCC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE EDC FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT EDC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC ");
                    strQry.AppendLine(" ON EDC.COMPANY_NO = EIC.COMPANY_NO ");
                    strQry.AppendLine(" AND EDC.EMPLOYEE_NO = EIC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EDC.PAY_CATEGORY_TYPE = EIC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EDC.RUN_TYPE = EIC.RUN_TYPE ");
                    strQry.AppendLine(" AND EDC.RUN_NO = EIC.RUN_NO ");
                    //No Payslip
                    strQry.AppendLine(" AND EIC.PAYSLIP_IND = 'N'");
                    strQry.AppendLine(" AND EIC.CLOSE_IND <> 'Y'");

                    strQry.AppendLine(" WHERE EDC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EDC.RUN_TYPE = 'P'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE EDEPC FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE_CURRENT EDEPC ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC ");
                    strQry.AppendLine(" ON EDEPC.COMPANY_NO = EIC.COMPANY_NO ");
                    strQry.AppendLine(" AND EDEPC.EMPLOYEE_NO = EIC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EDEPC.PAY_CATEGORY_TYPE = EIC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EDEPC.RUN_NO = EIC.RUN_NO ");
                    strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");
                    //No Payslip
                    strQry.AppendLine(" AND EIC.PAYSLIP_IND = 'N'");
                    strQry.AppendLine(" AND EIC.CLOSE_IND <> 'Y'");

                    strQry.AppendLine(" WHERE EDEPC.COMPANY_NO = " + parInt64CompanyNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" UPDATE E");
                    strQry.AppendLine(" SET ");
                    //Errol - 2015-02-17
                    strQry.AppendLine(" E.EMPLOYEE_LAST_RUNDATE = '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",E.FIRST_RUN_COMPLETED_IND = 'Y'");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC ");
                    strQry.AppendLine(" ON E.COMPANY_NO = EIC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EIC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EIC.PAY_CATEGORY_TYPE ");
                    //No Payslip
                    strQry.AppendLine(" AND EIC.PAYSLIP_IND = 'N'");
                    strQry.AppendLine(" AND EIC.CLOSE_IND <> 'Y'");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND RUN_TYPE = 'P'");
                    //No Payslip
                    strQry.AppendLine(" AND PAYSLIP_IND = 'N'");
                    strQry.AppendLine(" AND CLOSE_IND <> 'Y'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //Delete Deduction Earning Percenatge Records Where Totals are Zero
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND STR(EMPLOYEE_NO) + STR(DEDUCTION_NO) + STR(DEDUCTION_SUB_ACCOUNT_NO) IN ");
                    strQry.AppendLine("(SELECT STR(EIC.EMPLOYEE_NO) + STR(EDC.DEDUCTION_NO) + STR(EDC.DEDUCTION_SUB_ACCOUNT_NO) ");

                    strQry.AppendLine(" FROM  InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT EDC");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = EDC.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EDC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EDC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EDC.RUN_TYPE = EIC.RUN_TYPE");
                    strQry.AppendLine(" AND EDC.TOTAL = 0 ");

                    strQry.AppendLine(" INNER JOIN ");
                    strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);

                    strQry.AppendLine(" INNER JOIN ");
                    strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" LEFT JOIN ");
                    strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY EDH");
                    strQry.AppendLine(" ON EDC.COMPANY_NO = EDH.COMPANY_NO ");
                    strQry.AppendLine(" AND EDC.EMPLOYEE_NO = EDH.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EDC.DEDUCTION_NO = EDH.DEDUCTION_NO");
                    strQry.AppendLine(" AND EDC.DEDUCTION_SUB_ACCOUNT_NO = EDH.DEDUCTION_SUB_ACCOUNT_NO");
                    //Removed to Add Take-On Totals
                    //strQry.AppendLine(" AND EDH.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EDH.PAY_PERIOD_DATE >= '" + dtFiscalBeginDateTime.ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(" WHERE EIC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND (EIC.CLOSE_IND <> 'Y'");
                    strQry.AppendLine(" OR EIC.CLOSE_IND IS NULL)");

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" EIC.EMPLOYEE_NO");
                    strQry.AppendLine(",EDC.DEDUCTION_NO");
                    strQry.AppendLine(",EDC.DEDUCTION_SUB_ACCOUNT_NO ");

                    strQry.AppendLine(" HAVING ISNULL(SUM(EDH.TOTAL),0) = 0)");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //Delete Deduction Records Where Totals are Zero
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND RUN_TYPE = 'P'");
                    //Tax Record is Used in JOIN for TAX YTD for Tax Module
                    strQry.AppendLine(" AND DEDUCTION_NO <> 1 ");
                    strQry.AppendLine(" AND STR(EMPLOYEE_NO) + STR(DEDUCTION_NO) + STR(DEDUCTION_SUB_ACCOUNT_NO) IN ");
                    strQry.AppendLine("(SELECT STR(EIC.EMPLOYEE_NO) + STR(EDC.DEDUCTION_NO) + STR(EDC.DEDUCTION_SUB_ACCOUNT_NO) ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC");

                    strQry.AppendLine(" INNER JOIN ");
                    strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT EDC");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = EDC.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EDC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EDC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EDC.RUN_TYPE = EIC.RUN_TYPE");
                    strQry.AppendLine(" AND EDC.TOTAL = 0 ");

                    strQry.AppendLine(" INNER JOIN ");
                    strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);

                    strQry.AppendLine(" INNER JOIN ");
                    strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" LEFT JOIN ");
                    strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY EDH");
                    strQry.AppendLine(" ON EDC.COMPANY_NO = EDH.COMPANY_NO ");
                    strQry.AppendLine(" AND EDC.EMPLOYEE_NO = EDH.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EDC.DEDUCTION_NO = EDH.DEDUCTION_NO");
                    strQry.AppendLine(" AND EDC.DEDUCTION_SUB_ACCOUNT_NO = EDH.DEDUCTION_SUB_ACCOUNT_NO");
                    //Removed to Add Take-On Totals
                    //strQry.AppendLine(" AND EDH.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EDH.PAY_PERIOD_DATE >= '" + dtFiscalBeginDateTime.ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(" WHERE EIC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND (EIC.CLOSE_IND <> 'Y'");
                    strQry.AppendLine(" OR EIC.CLOSE_IND IS NULL)");

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" EIC.EMPLOYEE_NO");
                    strQry.AppendLine(",EDC.DEDUCTION_NO");
                    strQry.AppendLine(",EDC.DEDUCTION_SUB_ACCOUNT_NO ");

                    strQry.AppendLine(" HAVING ISNULL(SUM(EDH.TOTAL),0) = 0)");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //Delete Leave Earning Records Where Totals are Zero
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND RUN_TYPE = 'P'");
                    //Leave Records
                    strQry.AppendLine(" AND EARNING_NO > 19");
                    strQry.AppendLine(" AND STR(EMPLOYEE_NO) + STR(EARNING_NO) IN ");
                    strQry.AppendLine("(SELECT STR(EIC.EMPLOYEE_NO) + STR(EEC.EARNING_NO) ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC");

                    strQry.AppendLine(" INNER JOIN ");
                    strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = EEC.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EEC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EEC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EEC.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EEC.RUN_TYPE = EIC.RUN_TYPE");
                    strQry.AppendLine(" AND EEC.TOTAL = 0");
                    //strQry.AppendLine(" AND EEC.TOTAL_YTD_BF = 0 ");

                    strQry.AppendLine(" INNER JOIN ");
                    strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);

                    strQry.AppendLine(" INNER JOIN ");
                    strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
                    strQry.AppendLine(" ON EEC.COMPANY_NO = EEH.COMPANY_NO ");
                    strQry.AppendLine(" AND EEC.EMPLOYEE_NO = EEH.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EEC.EARNING_NO = EEH.EARNING_NO");
                    strQry.AppendLine(" AND EEC.PAY_CATEGORY_NO = EEH.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE");
                    //Removed to Add Take-On Totals
                    //strQry.AppendLine(" AND EEH.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EEH.PAY_PERIOD_DATE >= '" + dtFiscalBeginDateTime.ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(" WHERE EIC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND (EIC.CLOSE_IND <> 'Y'");
                    strQry.AppendLine(" OR EIC.CLOSE_IND IS NULL)");

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" EIC.EMPLOYEE_NO");
                    strQry.AppendLine(",EEC.EARNING_NO ");

                    strQry.AppendLine(" HAVING ISNULL(SUM(EEH.TOTAL),0) = 0)");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY ");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_EARNING_HISTORY", ref strQry, ref strFieldNamesInitialised, "EEC", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EEC.COMPANY_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");
                    
                    //2017-07-31 - Only for Pay Categories with Time Sheets or Default PayCategory
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON EEC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EEC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EEC.PAY_CATEGORY_NO = EPCC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_USED_IND = 'Y'");
                    
                    strQry.AppendLine(" WHERE EEC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EEC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND EEC.RUN_TYPE = 'P'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                  
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK_HISTORY ");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("PAY_CATEGORY_BREAK_HISTORY", ref strQry, ref strFieldNamesInitialised, "PCBC", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" PCBC.COMPANY_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK_CURRENT PCBC");
                    strQry.AppendLine(" WHERE PCBC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PCBC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PCBC.PAY_CATEGORY_TYPE = 'W'");
                    
                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                  
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY ");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_DEDUCTION_CURRENT", ref strQry, ref strFieldNamesInitialised, "EDC", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EDC.COMPANY_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT EDC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON EDC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EDC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P'");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON EDC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND EDC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE EDC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EDC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND EDC.RUN_TYPE = 'P'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE_HISTORY ");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE_CURRENT", ref strQry, ref strFieldNamesInitialised, "EDEPC", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EDEPC.COMPANY_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE_CURRENT EDEPC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON EDEPC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EDEPC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EDEPC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P' ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON EDEPC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND EDEPC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE EDEPC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EDEPC.PAY_CATEGORY_TYPE = 'W'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                    
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_BREAK_DAY_HISTORY ");

                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT", ref strQry, ref strFieldNamesInitialised, "ETATBDC", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" ETATBDC.COMPANY_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT ETATBDC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                    strQry.AppendLine(" ON ETATBDC.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND ETATBDC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND ETATBDC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");

                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");
                    
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON ETATBDC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND ETATBDC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE ETATBDC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND ETATBDC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);

                    strQry.AppendLine(" AND ETATBDC.TIMESHEET_DATE > ");

                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN E.FIRST_RUN_COMPLETED_IND = 'Y'");

                    strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE   ");

                    strQry.AppendLine(" ELSE DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(" AND ETATBDC.TIMESHEET_DATE <= '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                    
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_HISTORY ");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_TIMESHEET_CURRENT", ref strQry, ref strFieldNamesInitialised, "ETC", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" ETC.COMPANY_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");

                    //Errol 2014-02-21
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                    strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");

                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");
                    //Errol 2014-02-21

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    
                    //2013-07-02
                    strQry.AppendLine(" AND ETC.TIMESHEET_DATE > ");

                    strQry.AppendLine(" CASE ");

                    //Errol - 2015-02-17
                    strQry.AppendLine(" WHEN E.FIRST_RUN_COMPLETED_IND = 'Y'");

                    strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE   ");

                    strQry.AppendLine(" ELSE DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //ELR - 2015-03-21
                    strQry.AppendLine(" AND ETC.INCLUDED_IN_RUN_IND = 'Y' ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //2012-11-20
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_HISTORY ");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_BREAK_CURRENT", ref strQry, ref strFieldNamesInitialised, "ETC", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" ETC.COMPANY_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT ETC");

                    //Errol 2014-02-21
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                    strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");

                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");
                    //Errol 2014-02-21

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);


                    strQry.AppendLine(" AND ETC.BREAK_DATE > ");

                    strQry.AppendLine(" CASE ");

                    //Errol - 2015-02-17
                    strQry.AppendLine(" WHEN E.FIRST_RUN_COMPLETED_IND = 'Y'");

                    strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE   ");

                    strQry.AppendLine(" ELSE DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" END ");
                    
                    strQry.AppendLine(" AND ETC.BREAK_DATE <= '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //ELR - 2015-03-21
                    strQry.AppendLine(" AND ETC.INCLUDED_IN_RUN_IND = 'Y' ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                  
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_HISTORY ");
                    strQry.AppendLine("(PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT", ref strQry, ref strFieldNamesInitialised, "", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine("'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                    //2013-09-04
                    strQry.AppendLine(" AND AUTHORISED_IND = 'Y'");
                        
                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                    
                    //Leave
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_HISTORY ");
                    strQry.AppendLine("(PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT", ref strQry, ref strFieldNamesInitialised, "EPCLAC", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine("'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT EPCLAC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON EPCLAC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND EPCLAC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCLAC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE EPCLAC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EPCLAC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EPCLAC.PAY_CATEGORY_TYPE = 'W'");
                    //Only Authorised Records
                    strQry.AppendLine(" AND EPCLAC.AUTHORISED_IND = 'Y'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_WEEK_HISTORY ");
                    strQry.AppendLine("(PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_EARNING_WEEK_CURRENT", ref strQry, ref strFieldNamesInitialised, "EEWC", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine("'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_WEEK_CURRENT EEWC");


                    //2017-07-31 - Only for Pay Categories with Time Sheets or Default PayCategory
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON EEWC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EEWC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EEWC.PAY_CATEGORY_NO = EPCC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_USED_IND = 'Y'");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON EEWC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND EEWC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE EEWC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EEWC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.LOANS ");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",DEDUCTION_NO");
                    strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
                    strQry.AppendLine(",LOAN_PROCESSED_DATE");
                    strQry.AppendLine(",LOAN_AMOUNT_RECEIVED");
                    strQry.AppendLine(",LOAN_REC_NO");
                    strQry.AppendLine(",LOAN_DESC");
                    strQry.AppendLine(",LOAN_AMOUNT_PAID");
                    strQry.AppendLine(",PROCESS_NO");
                    strQry.AppendLine(",USER_NO_NEW_RECORD");
                    strQry.AppendLine(",DATETIME_NEW_RECORD)");

                    strQry.AppendLine(" SELECT DISTINCT");
                    strQry.AppendLine(" EDC.COMPANY_NO");
                    strQry.AppendLine(",EDC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EDC.EMPLOYEE_NO");
                    strQry.AppendLine(",EDC.DEDUCTION_NO");
                    strQry.AppendLine(",EDC.DEDUCTION_SUB_ACCOUNT_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",EDC.TOTAL");
                    strQry.AppendLine(",ISNULL(MAX(L.LOAN_REC_NO),0) + 1");
                    strQry.AppendLine(",'via Payroll Run'");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",99");
                    strQry.AppendLine("," + parint64CurrentUserNo);
                    strQry.AppendLine(",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "'");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT EDC");
                    strQry.AppendLine(" ON EDC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND EDC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EDC.RUN_TYPE = 'P'");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON EDC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EDC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y'");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.DEDUCTION D");
                    strQry.AppendLine(" ON EDC.COMPANY_NO = D.COMPANY_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = D.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EDC.DEDUCTION_NO = D.DEDUCTION_NO");
                    strQry.AppendLine(" AND EDC.DEDUCTION_SUB_ACCOUNT_NO = D.DEDUCTION_SUB_ACCOUNT_NO");
                    strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND D.DEDUCTION_LOAN_TYPE_IND = 'Y'");

                    //All - Even Deleted Records
                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.LOANS L");
                    strQry.AppendLine(" ON EDC.COMPANY_NO = L.COMPANY_NO");
                    strQry.AppendLine(" AND EDC.PAY_CATEGORY_TYPE = L.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EDC.EMPLOYEE_NO = L.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EDC.DEDUCTION_NO = L.DEDUCTION_NO");
                    strQry.AppendLine(" AND EDC.DEDUCTION_SUB_ACCOUNT_NO = L.DEDUCTION_SUB_ACCOUNT_NO");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EDC.TOTAL <> 0");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" EDC.COMPANY_NO");
                    strQry.AppendLine(",EDC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EDC.EMPLOYEE_NO");
                    strQry.AppendLine(",EDC.DEDUCTION_NO");
                    strQry.AppendLine(",EDC.DEDUCTION_SUB_ACCOUNT_NO");
                    strQry.AppendLine(",EDC.TOTAL");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_HISTORY ");
                    strQry.AppendLine("(PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("PAY_CATEGORY_WEEK_CURRENT", ref strQry, ref strFieldNamesInitialised, "", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine("'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_CURRENT");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT_HISTORY ");
                    strQry.AppendLine("(PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("LEAVE_SHIFT_CURRENT", ref strQry, ref strFieldNamesInitialised, "", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine("'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT_CURRENT");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    //2014-03-25
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY ");
                    strQry.AppendLine("(PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("PAY_CATEGORY_PERIOD_CURRENT", ref strQry, ref strFieldNamesInitialised, "", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine("'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND RUN_TYPE = 'P'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.LOANS");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" LOAN_PROCESSED_DATE = '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND LOAN_PROCESSED_DATE IS NULL");
                    strQry.AppendLine(" AND PROCESS_NO = 0");
                    strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT EMPLOYEE_NO ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND DEFAULT_IND = 'Y')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" EMPLOYEE_LAST_RUNDATE = '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    //Errol - 2015-02-17
                    strQry.AppendLine(",FIRST_RUN_COMPLETED_IND = 'Y'");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT EMPLOYEE_NO ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND DEFAULT_IND = 'Y')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //Employee Closed Off
                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" EMPLOYEE_ENDDATE = '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT EPCC.EMPLOYEE_NO ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC");
                    strQry.AppendLine(" ON EPCC.COMPANY_NO = EIC.COMPANY_NO ");
                    strQry.AppendLine(" AND EPCC.EMPLOYEE_NO = EIC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = EIC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EIC.CLOSE_IND = 'Y'");
                    strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");

                    strQry.AppendLine(" WHERE EPCC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                    //ERROL Added 2011-06-29
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);

                    strQry.AppendLine(" AND RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT E.EMPLOYEE_NO ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT E.EMPLOYEE_NO ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON  E.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT E.EMPLOYEE_NO ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");

                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT E.EMPLOYEE_NO ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT EPCC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = 'W'");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");

                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT E.EMPLOYEE_NO ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT EPCLAC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPCLAC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCLAC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCLAC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EPCLAC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_WEEK_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT E.EMPLOYEE_NO ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND TIMESHEET_DATE <= '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT E.EMPLOYEE_NO ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //2012-11-20
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND BREAK_DATE <= '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT E.EMPLOYEE_NO ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                 
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //Errol Added 2011-11-02
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    //2014-03-25
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND RUN_TYPE = 'P'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //All Leave Records That were NOT set To Current RUN (PROCESS_NO = 0)
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT EIC.EMPLOYEE_NO ");
                    
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y'");

                    strQry.AppendLine(" WHERE EIC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = 'W'");
                    //Employee Closed
                    strQry.AppendLine(" AND EIC.CLOSE_IND = 'Y'");
                    strQry.AppendLine(" AND EIC.RUN_TYPE = 'P')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //END Of PAY_CATEGORY LOOP
                }

                //2018-09-06 Fix Duplicate PUBLIC_HOLIDAY_HISTORY Record Insert when 2 Different Runs Overlap
                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_HISTORY ");
                strQry.AppendLine("(PAY_PERIOD_DATE");
                strQry.AppendLine(",COMPANY_NO");
                strQry.AppendLine(",PUBLIC_HOLIDAY_DATE");
                strQry.AppendLine(",RUN_NO");
                strQry.AppendLine(",PUBLIC_HOLIDAY_DESC)");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine("'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(",PHC.COMPANY_NO");
                strQry.AppendLine(",PHC.PUBLIC_HOLIDAY_DATE");
                strQry.AppendLine(",PHC.RUN_NO");
                strQry.AppendLine(",PHC.PUBLIC_HOLIDAY_DESC");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_CURRENT PHC");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_HISTORY PHH");
                strQry.AppendLine(" ON PHC.COMPANY_NO = PHH.COMPANY_NO");
                strQry.AppendLine(" AND PHH.PAY_PERIOD_DATE = '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" AND PHC.PUBLIC_HOLIDAY_DATE = PHH.PUBLIC_HOLIDAY_DATE");
                strQry.AppendLine(" AND PHC.RUN_NO = PHH.RUN_NO");

                strQry.AppendLine(" WHERE PHC.COMPANY_NO = " + parInt64CompanyNo);

                //Record Does NOT Exist
                strQry.AppendLine(" AND PHH.COMPANY_NO IS NULL ");
                
                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                
                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_PERIOD_DATE");

                //Return strings of field names that need to be Initialised
                clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_INFO_CURRENT", ref strQry, ref strFieldNamesInitialised, "EIC", parInt64CompanyNo);

                strQry.AppendLine(")");
                strQry.AppendLine(" SELECT DISTINCT");
                strQry.AppendLine(" EIC.COMPANY_NO");
                strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                //Append Initialised Numeric Fields Names
                strQry.Append(strFieldNamesInitialised);

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");
                strQry.AppendLine(" ON EIC.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND EIC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                strQry.AppendLine(" ON EIC.COMPANY_NO = EPCC.COMPANY_NO ");
                strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + parstrWagesPayCategoryNumbers + ")");

                strQry.AppendLine(" WHERE EIC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_PERIOD_DATE");

                //Return strings of field names that need to be Initialised
                clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_PAY_CATEGORY_CURRENT", ref strQry, ref strFieldNamesInitialised, "EPCC", parInt64CompanyNo);

                //2017-07-31
                strQry = strQry.Replace(",PAY_CATEGORY_USED_IND", "");
                //2017-07-31
                strFieldNamesInitialised = strFieldNamesInitialised.Replace(",EPCC.PAY_CATEGORY_USED_IND", "");

                strQry.AppendLine(")");
                strQry.AppendLine(" SELECT DISTINCT");
                strQry.AppendLine(" E.COMPANY_NO");
                strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                //Append Initialised Numeric Fields Names
                strQry.Append(strFieldNamesInitialised);

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                strQry.AppendLine(" ON EPCC.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND EPCC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + parstrWagesPayCategoryNumbers + ")");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P'");
                //2017-07-31
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_USED_IND = 'Y'");

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                                
                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //ELR 2014-05-01
                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.OCCUPATION_HISTORY");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_PERIOD_DATE");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",OCCUPATION_NO");
                strQry.AppendLine(",OCCUPATION_DESC)");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" O.COMPANY_NO");
                strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(",'W'");
                strQry.AppendLine(",O.OCCUPATION_NO");
                strQry.AppendLine(",O.OCCUPATION_DESC ");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.OCCUPATION O ");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.OCCUPATION_HISTORY OH ");
                strQry.AppendLine(" ON O.COMPANY_NO = OH.COMPANY_NO ");
                strQry.AppendLine(" AND OH.PAY_PERIOD_DATE = '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" AND OH.PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND O.OCCUPATION_NO = OH.OCCUPATION_NO ");

                strQry.AppendLine(" WHERE O.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND O.DATETIME_DELETE_RECORD IS NULL");

                //Does Not Exist
                strQry.AppendLine(" AND OH.COMPANY_NO IS NULL ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //Delete Loans For Closed Employee 
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.LOANS");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" DATETIME_DELETE_RECORD = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "'");
                strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND LOAN_PROCESSED_DATE IS NULL");
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND PROCESS_NO <> 0");
                strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                strQry.AppendLine("(SELECT EMPLOYEE_NO ");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT ");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND RUN_TYPE = 'P'");
                strQry.AppendLine(" AND CLOSE_IND = 'Y')");
                               
                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //Delete Leave For Closed Employee
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND PROCESS_NO <> 0");
                strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                strQry.AppendLine("(SELECT EMPLOYEE_NO ");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT ");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND RUN_TYPE = 'P'");
                strQry.AppendLine(" AND CLOSE_IND = 'Y')");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //Rollup Loans for Active Employees 
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.LOANS");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" PROCESS_NO = PROCESS_NO - 1");
                strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND LOAN_PROCESSED_DATE IS NULL");
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND PROCESS_NO > 0");
                strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                strQry.AppendLine("(SELECT EMPLOYEE_NO ");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT ");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND RUN_TYPE = 'P'");
                strQry.AppendLine(" AND CLOSE_IND = 'N')");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //Rollup Leave for Active Employees 
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" PROCESS_NO = PROCESS_NO - 1");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND PROCESS_NO > 0");
                strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                strQry.AppendLine("(SELECT EMPLOYEE_NO ");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT ");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND RUN_TYPE = 'P'");
                strQry.AppendLine(" AND CLOSE_IND = 'N')");
               
                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //Delete These 2 Records Last - Used in Delete Queries
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND RUN_TYPE = 'P'");
                strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                strQry.AppendLine("(SELECT EMPLOYEE_NO ");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND RUN_TYPE = 'P'");
                strQry.AppendLine(" AND PAY_CATEGORY_NO IN (" + parstrWagesPayCategoryNumbers + "))");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //2013-09-30
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_BATCH_TEMP");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND PROCESS_NO = 0");

                strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                strQry.AppendLine("(SELECT EMPLOYEE_NO ");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND RUN_TYPE = 'P'");
                strQry.AppendLine(" AND PAY_CATEGORY_NO IN (" + parstrWagesPayCategoryNumbers + "))");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //2013-09-30
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_BATCH_TEMP");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND PROCESS_NO = 0");

                strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                strQry.AppendLine("(SELECT EMPLOYEE_NO ");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND RUN_TYPE = 'P'");
                strQry.AppendLine(" AND PAY_CATEGORY_NO IN (" + parstrWagesPayCategoryNumbers + "))");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                strQry.AppendLine("(SELECT EMPLOYEE_NO ");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND PAY_CATEGORY_NO IN (" + parstrWagesPayCategoryNumbers + "))");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //2014-03-29 (Cleanup Orphan Records)
                strQry.Clear();
                strQry.AppendLine(" DELETE ETC");

                strQry.AppendLine(" FROM  InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC   ");
           
                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EPC.COMPANY_NO IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //2014-03-29 (Cleanup Orphan Records)
                strQry.Clear();
                strQry.AppendLine(" DELETE ETC");

                strQry.AppendLine(" FROM  InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT ETC   ");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EPC.COMPANY_NO IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
             
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.COMPANY");

                strQry.AppendLine(" SET WAGE_RUN_IND = NULL");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                
                //2017-08-05
                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.CLOSE_RUN_QUEUE_COMPLETED");

                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",RUN_TYPE");
                strQry.AppendLine(",USER_NO");
                strQry.AppendLine(",WAGE_PAY_PERIOD_DATE");
                strQry.AppendLine(",SALARY_PAY_PERIOD_DATE");

                strQry.AppendLine(",WAGES_PAY_CATEGORY_NUMBERS");
                strQry.AppendLine(",SALARIES_PAY_CATEGORY_NUMBERS");
                strQry.AppendLine(",CLOSE_RUN_QUEUE_IND");

                strQry.AppendLine(",START_RUN_DATE");
                strQry.AppendLine(",END_RUN_DATE)");

                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" COMPANY_NO");
                strQry.AppendLine(",RUN_TYPE");
                strQry.AppendLine(",USER_NO");
                strQry.AppendLine(",WAGE_PAY_PERIOD_DATE");
                strQry.AppendLine(",SALARY_PAY_PERIOD_DATE");

                strQry.AppendLine(",WAGES_PAY_CATEGORY_NUMBERS");
                strQry.AppendLine(",SALARIES_PAY_CATEGORY_NUMBERS");
                strQry.AppendLine(",'C'");
                strQry.AppendLine(",START_RUN_DATE");
                strQry.AppendLine(",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" FROM InteractPayroll.dbo.CLOSE_RUN_QUEUE");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND RUN_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("W"));
                strQry.AppendLine(" AND WAGE_PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(pardtRunDate.ToString("yyyy-MM-dd")));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                //2017-08-05
                strQry.Clear();

                strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.CLOSE_RUN_QUEUE");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND RUN_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("W"));
                strQry.AppendLine(" AND WAGE_PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(pardtRunDate.ToString("yyyy-MM-dd")));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                //Cleanup of Backups (IF Fails - Who Cares?)
                try
                {
                    FileInfo fiFileInfo;
                   
                    strQry.Clear();
                    strQry.AppendLine(" SELECT");
                    strQry.AppendLine(" BACKUP_DATABASE_NO");
                    strQry.AppendLine(",BACKUP_FILE_NAME");

                    strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_DATETIME");

                    strQry.AppendLine(" WHERE BACKUP_DATABASE_NAME = 'InteractPayroll_" + parInt64CompanyNo.ToString("00000") + "'");
                    strQry.AppendLine(" AND BACKUP_DATETIME < '" + DateTime.Now.AddDays(-35).ToString("yyyy-MM-dd") + "'");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "File", -1);

                    for (int intRow = 0; intRow < DataSet.Tables["File"].Rows.Count; intRow++)
                    {
                        fiFileInfo = new FileInfo(DataSet.Tables["File"].Rows[intRow]["BACKUP_FILE_NAME"].ToString());

                        if (fiFileInfo.Exists == true)
                        {
                            File.Delete(DataSet.Tables["File"].Rows[intRow]["BACKUP_FILE_NAME"].ToString());
                        }

                        strQry.Clear();
                        strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.BACKUP_DATABASE_DATETIME");
                        strQry.AppendLine(" WHERE BACKUP_DATABASE_NO = " + DataSet.Tables["File"].Rows[intRow]["BACKUP_DATABASE_NO"].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                    }

                    //Cleanup Any Backup File Older than 35 Days
                    strQry.Clear();
                    strQry.AppendLine(" SELECT");
                    strQry.AppendLine(" BACKUP_DATABASE_PATH");
                    strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_PATH");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Directory", -1);

                    string strBaseDirectory = DataSet.Tables["Directory"].Rows[0]["BACKUP_DATABASE_PATH"].ToString();

                    Cleanup_Files_Older_35_Days(parInt64CompanyNo, strBaseDirectory);
                }
                catch (Exception ex1)
                {
                }

                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
                strQry.AppendLine(" SET BACKUP_DB_IND = 1");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            }
            catch(Exception ex)
            {
                Write_Log(ex, strClassNameFunctionAndParameters, strQry.ToString(), true);

                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.CLOSE_RUN_QUEUE");

                strQry.AppendLine(" SET ");
                strQry.AppendLine(" CLOSE_RUN_QUEUE_IND = 'F'");
                strQry.AppendLine(",END_RUN_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND RUN_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("W"));
                strQry.AppendLine(" AND WAGE_PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(pardtRunDate.ToString("yyyy-MM-dd")));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                intReturnCode = 1;
            }

            return intReturnCode;
        }

        private void Cleanup_Files_Older_35_Days(Int64 parInt64CompanyNo,string parstrBaseDirectory)
        {
            try
            {
                //Remov all in thiis Section
                string strCompanyPattern = "InteractPayroll_" + parInt64CompanyNo.ToString("00000") + "_*";

                string[] strFiles = Directory.GetFiles(parstrBaseDirectory, strCompanyPattern);

                foreach (string strFile in strFiles)
                {
                    try
                    {
                        DateTime myDateField = DateTime.ParseExact(strFile.Replace(parstrBaseDirectory + "\\InteractPayroll_00013_", "").Substring(0, 8), "yyyyMMdd", null);

                        if (DateTime.Now.AddDays(-35) > myDateField)
                        {
                            //Delete File
                            File.Delete(strFile);
                        }
                    }
                    catch
                    {
                        //Not in Format
                    }
                }
            }
            catch
            {
            }
        }

        public int Close_TimeAttendance_Run(Int64 parInt64CompanyNo, DateTime pardtRunDate, string parstrTimeAttendancePayCategoryNumbers, Int64 parint64CurrentUserNo)
        {
            string strClassNameFunctionAndParameters = pvtstrClassName + " Close_TimeAttendance_Run CompanyNo=" + parInt64CompanyNo + ",pardtRunDate=" + pardtRunDate.ToString("yyyy-MM-dd") + ",parstrTimeAttendancePayCategoryNumbers=" + parstrTimeAttendancePayCategoryNumbers + ",parint64CurrentUserNo=" + parint64CurrentUserNo.ToString();

            int intReturnCode = 0;
            StringBuilder strQry = new StringBuilder();

            try
            {
                //2017-08-05
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.CLOSE_RUN_QUEUE");

                strQry.AppendLine(" SET ");
                //S=Started
                strQry.AppendLine(" CLOSE_RUN_QUEUE_IND = 'S'");
                strQry.AppendLine(",START_RUN_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND RUN_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));
                strQry.AppendLine(" AND WAGE_PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(pardtRunDate.ToString("yyyy-MM-dd")));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
               
                string[] strPayCategoryArray = parstrTimeAttendancePayCategoryNumbers.Split(',');

                DataSet DataSet = new DataSet();

                StringBuilder strFieldNamesInitialised = new StringBuilder();

                //Run Through Pay Categories
                for (int intRow = 0; intRow < strPayCategoryArray.Length; intRow++)
                {
#if (DEBUG)
                    if (strPayCategoryArray[intRow] == "31"
                    || strPayCategoryArray[intRow] == "33"
                    || strPayCategoryArray[intRow] == "45"
                    || strPayCategoryArray[intRow] == "67")
                    {
                        string strStop = ""; 
                    }
#endif
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY ");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_EARNING_HISTORY", ref strQry, ref strFieldNamesInitialised, "EEC", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EEC.COMPANY_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");
                    
                    //2017-07-31 - Only for Pay Categories with Time Sheets
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON EEC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EEC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EEC.PAY_CATEGORY_NO = EPCC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = 'T'");
                    strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_USED_IND = 'Y'");

                    strQry.AppendLine(" WHERE EEC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EEC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = 'T'");
                    strQry.AppendLine(" AND EEC.RUN_TYPE = 'P'");
                    
                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK_HISTORY ");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("PAY_CATEGORY_BREAK_HISTORY", ref strQry, ref strFieldNamesInitialised, "PCBC", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" PCBC.COMPANY_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK_CURRENT PCBC");
                    strQry.AppendLine(" WHERE PCBC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PCBC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PCBC.PAY_CATEGORY_TYPE = 'T'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                    
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_HISTORY ");

                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT", ref strQry, ref strFieldNamesInitialised, "ETATBDC", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" ETATBDC.COMPANY_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT ETATBDC");

                    //Errol 2014-02-21
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                    strQry.AppendLine(" ON ETATBDC.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND ETATBDC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND ETATBDC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");

                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'T'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");
                    //Errol 2014-02-21

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON ETATBDC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND ETATBDC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE ETATBDC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND ETATBDC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);

                    //2013-07-02
                    strQry.AppendLine(" AND ETATBDC.TIMESHEET_DATE > ");

                    strQry.AppendLine(" CASE ");

                    //Errol - 2015-02-17
                    strQry.AppendLine(" WHEN E.FIRST_RUN_COMPLETED_IND = 'Y'");

                    strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE   ");

                    strQry.AppendLine(" ELSE DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(" AND ETATBDC.TIMESHEET_DATE <= '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                    
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_HISTORY ");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT", ref strQry, ref strFieldNamesInitialised, "ETC", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" ETC.COMPANY_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC");

                    //Errol 2014-02-21
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                    strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");

                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'T'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");
                    //Errol 2014-02-21

                    strQry.AppendLine(" INNER JOIN ");

                    strQry.AppendLine("(SELECT ");
                    strQry.AppendLine(" E.EMPLOYEE_NO ");

                    //Errol 2013-06-15
                    strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

                    strQry.AppendLine(" CASE ");

                    //Errol - 2015-02-17
                    strQry.AppendLine(" WHEN E.FIRST_RUN_COMPLETED_IND = 'Y'");

                    strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE   ");

                    strQry.AppendLine(" ELSE DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL) AS EMPLOYEE_TABLE");

                    strQry.AppendLine(" ON ETC.EMPLOYEE_NO = EMPLOYEE_TABLE.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND ETC.TIMESHEET_DATE > EMPLOYEE_TABLE.EMPLOYEE_LAST_RUNDATE");

                    strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //ELR - 2015-03-21
                    strQry.AppendLine(" AND ETC.INCLUDED_IN_RUN_IND = 'Y'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //2012-11-20
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_HISTORY ");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_TIME_ATTEND_BREAK_CURRENT", ref strQry, ref strFieldNamesInitialised, "ETC", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" ETC.COMPANY_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETC");

                    //Errol 2014-02-21
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                    strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");

                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'T'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");
                    //Errol 2014-02-21

                    strQry.AppendLine(" INNER JOIN ");

                    strQry.AppendLine("(SELECT ");
                    strQry.AppendLine(" E.EMPLOYEE_NO ");

                    //Errol 2013-06-15
                    strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

                    strQry.AppendLine(" CASE ");

                    //Errol - 2015-02-17
                    strQry.AppendLine(" WHEN E.FIRST_RUN_COMPLETED_IND = 'Y'");

                    strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE   ");

                    strQry.AppendLine(" ELSE DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL) AS EMPLOYEE_TABLE");

                    strQry.AppendLine(" ON ETC.EMPLOYEE_NO = EMPLOYEE_TABLE.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND ETC.BREAK_DATE > EMPLOYEE_TABLE.EMPLOYEE_LAST_RUNDATE");

                    strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND ETC.BREAK_DATE <= '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //ELR - 2015-03-21
                    strQry.AppendLine(" AND ETC.INCLUDED_IN_RUN_IND = 'Y'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_HISTORY ");
                    strQry.AppendLine("(PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT", ref strQry, ref strFieldNamesInitialised, "", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine("'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");
                    //2013-09-04
                    strQry.AppendLine(" AND AUTHORISED_IND = 'Y'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                    
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_WEEK_HISTORY ");
                    strQry.AppendLine("(PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_EARNING_WEEK_CURRENT", ref strQry, ref strFieldNamesInitialised, "EEWC", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine("'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_WEEK_CURRENT EEWC");

                    //2017-07-31 - Only for Pay Categories with Time Sheets
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON EEWC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EEWC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EEWC.PAY_CATEGORY_NO = EPCC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = 'T'");
                    strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P'");
                    //2017-07-31
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_USED_IND = 'Y'");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON EEWC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND EEWC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE EEWC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EEWC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                                        
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_HISTORY ");
                    strQry.AppendLine("(PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("PAY_CATEGORY_WEEK_CURRENT", ref strQry, ref strFieldNamesInitialised, "", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine("'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_CURRENT");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY ");
                    strQry.AppendLine("(PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("PAY_CATEGORY_PERIOD_CURRENT", ref strQry, ref strFieldNamesInitialised, "", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine("'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");
                    strQry.AppendLine(" AND RUN_TYPE = 'P'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                    
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                    //ERROL Added 2011-06-29
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");

                    strQry.AppendLine(" AND RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT E.EMPLOYEE_NO ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");

                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT E.EMPLOYEE_NO ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT EPCC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = 'T'");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_WEEK_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT E.EMPLOYEE_NO ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND TIMESHEET_DATE <= '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT E.EMPLOYEE_NO ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //2012-11-20
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND BREAK_DATE <= '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT E.EMPLOYEE_NO ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //Errol Added 2011-11-02
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");
                    strQry.AppendLine(" AND RUN_TYPE = 'P'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                }
                
                strQry.Clear();
                strQry.AppendLine(" UPDATE E");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" E.EMPLOYEE_LAST_RUNDATE = '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                //Errol - 2015-02-17
                strQry.AppendLine(",E.FIRST_RUN_COMPLETED_IND = 'Y'");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO IN (" + parstrTimeAttendancePayCategoryNumbers + ")");

                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'T'");
                strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T'");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //2018-09-06 Fix Duplicate PUBLIC_HOLIDAY_HISTORY Record Insert when 2 Different Runs Overlap
                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_HISTORY ");
                strQry.AppendLine("(PAY_PERIOD_DATE");
                strQry.AppendLine(",COMPANY_NO");
                strQry.AppendLine(",PUBLIC_HOLIDAY_DATE");
                strQry.AppendLine(",RUN_NO");
                strQry.AppendLine(",PUBLIC_HOLIDAY_DESC)");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine("'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(",PHC.COMPANY_NO");
                strQry.AppendLine(",PHC.PUBLIC_HOLIDAY_DATE");
                strQry.AppendLine(",PHC.RUN_NO");
                strQry.AppendLine(",PHC.PUBLIC_HOLIDAY_DESC");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_CURRENT PHC");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_HISTORY PHH");
                strQry.AppendLine(" ON PHC.COMPANY_NO = PHH.COMPANY_NO");
                strQry.AppendLine(" AND PHH.PAY_PERIOD_DATE = '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" AND PHC.PUBLIC_HOLIDAY_DATE = PHH.PUBLIC_HOLIDAY_DATE");
                strQry.AppendLine(" AND PHC.RUN_NO = PHH.RUN_NO");
               
                strQry.AppendLine(" WHERE PHC.COMPANY_NO = " + parInt64CompanyNo);

                //Record Does NOT Exist
                strQry.AppendLine(" AND PHH.COMPANY_NO IS NULL ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_PERIOD_DATE");

                //Return strings of field names that need to be Initialised
                clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_INFO_CURRENT", ref strQry, ref strFieldNamesInitialised, "EIC", parInt64CompanyNo);

                strQry.AppendLine(")");
                strQry.AppendLine(" SELECT DISTINCT");
                strQry.AppendLine(" EIC.COMPANY_NO");
                strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                //Append Initialised Numeric Fields Names
                strQry.Append(strFieldNamesInitialised);

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");
                strQry.AppendLine(" ON EIC.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND EIC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                strQry.AppendLine(" ON EIC.COMPANY_NO = EPCC.COMPANY_NO ");
                strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + parstrTimeAttendancePayCategoryNumbers + ")");

                strQry.AppendLine(" WHERE EIC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = 'T'");
                strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_PERIOD_DATE");

                //Return strings of field names that need to be Initialised
                clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_PAY_CATEGORY_CURRENT", ref strQry, ref strFieldNamesInitialised, "EPCC", parInt64CompanyNo);

                //2017-07-31
                strQry = strQry.Replace(",PAY_CATEGORY_USED_IND", "");
                //2017-07-31
                strFieldNamesInitialised = strFieldNamesInitialised.Replace(",EPCC.PAY_CATEGORY_USED_IND", "");

                strQry.AppendLine(")");
                strQry.AppendLine(" SELECT DISTINCT");
                strQry.AppendLine(" E.COMPANY_NO");
                strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                //Append Initialised Numeric Fields Names
                strQry.Append(strFieldNamesInitialised);

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                strQry.AppendLine(" ON EPCC.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND EPCC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + parstrTimeAttendancePayCategoryNumbers + ")");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P'");
                //2017-07-31
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_USED_IND = 'Y'");
                
                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T'");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
 
                //Delete These 2 Records Last - Used in Delete Queries
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");
                strQry.AppendLine(" AND RUN_TYPE = 'P'");
                strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                strQry.AppendLine("(SELECT EMPLOYEE_NO ");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");
                strQry.AppendLine(" AND RUN_TYPE = 'P'");
                strQry.AppendLine(" AND PAY_CATEGORY_NO IN (" + parstrTimeAttendancePayCategoryNumbers + "))");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                strQry.AppendLine("(SELECT EMPLOYEE_NO ");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");
                strQry.AppendLine(" AND PAY_CATEGORY_NO IN (" + parstrTimeAttendancePayCategoryNumbers + "))");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            
                //2014-03-29 (Cleanup Orphan Records)
                strQry.Clear();
                strQry.AppendLine(" DELETE ETC");

                strQry.AppendLine(" FROM  InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC   ");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'T'");
                strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EPC.COMPANY_NO IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //2014-03-29 (Cleanup Orphan Records)
                strQry.Clear();
                strQry.AppendLine(" DELETE ETC");

                strQry.AppendLine(" FROM  InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETC   ");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'T'");
                strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EPC.COMPANY_NO IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.COMPANY");

                strQry.AppendLine(" SET TIME_ATTENDANCE_RUN_IND = NULL");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //2017-08-05
                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.CLOSE_RUN_QUEUE_COMPLETED");

                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",RUN_TYPE");
                strQry.AppendLine(",USER_NO");
                strQry.AppendLine(",WAGE_PAY_PERIOD_DATE");
                strQry.AppendLine(",SALARY_PAY_PERIOD_DATE");

                strQry.AppendLine(",WAGES_PAY_CATEGORY_NUMBERS");
                strQry.AppendLine(",SALARIES_PAY_CATEGORY_NUMBERS");
                strQry.AppendLine(",CLOSE_RUN_QUEUE_IND");

                strQry.AppendLine(",START_RUN_DATE");
                strQry.AppendLine(",END_RUN_DATE)");

                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" COMPANY_NO");
                strQry.AppendLine(",RUN_TYPE");
                strQry.AppendLine(",USER_NO");
                strQry.AppendLine(",WAGE_PAY_PERIOD_DATE");
                strQry.AppendLine(",SALARY_PAY_PERIOD_DATE");

                strQry.AppendLine(",WAGES_PAY_CATEGORY_NUMBERS");
                strQry.AppendLine(",SALARIES_PAY_CATEGORY_NUMBERS");
                strQry.AppendLine(",'C'");
                strQry.AppendLine(",START_RUN_DATE");
                strQry.AppendLine(",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" FROM InteractPayroll.dbo.CLOSE_RUN_QUEUE");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND RUN_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));
                strQry.AppendLine(" AND WAGE_PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(pardtRunDate.ToString("yyyy-MM-dd")));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                //2017-08-05
                strQry.Clear();

                strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.CLOSE_RUN_QUEUE");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND RUN_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));
                strQry.AppendLine(" AND WAGE_PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(pardtRunDate.ToString("yyyy-MM-dd")));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                //Cleanup of Backups (IF Fails - Who Cares?)
                try
                {
                    FileInfo fiFileInfo;

                    strQry.Clear();
                    strQry.AppendLine(" SELECT");
                    strQry.AppendLine(" BACKUP_DATABASE_NO");
                    strQry.AppendLine(",BACKUP_FILE_NAME");

                    strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_DATETIME");

                    strQry.AppendLine(" WHERE BACKUP_DATABASE_NAME = 'InteractPayroll_" + parInt64CompanyNo.ToString("00000") + "'");
                    strQry.AppendLine(" AND BACKUP_DATETIME < '" + DateTime.Now.AddDays(-35).ToString("yyyy-MM-dd") + "'");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "File", -1);

                    for (int intRow = 0; intRow < DataSet.Tables["File"].Rows.Count; intRow++)
                    {
                        fiFileInfo = new FileInfo(DataSet.Tables["File"].Rows[intRow]["BACKUP_FILE_NAME"].ToString());

                        if (fiFileInfo.Exists == true)
                        {
                            File.Delete(DataSet.Tables["File"].Rows[intRow]["BACKUP_FILE_NAME"].ToString());
                        }

                        strQry.Clear();
                        strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.BACKUP_DATABASE_DATETIME");
                        strQry.AppendLine(" WHERE BACKUP_DATABASE_NO = " + DataSet.Tables["File"].Rows[intRow]["BACKUP_DATABASE_NO"].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                    }

                    //Cleanup Any Backup File Older than 35 Days
                    strQry.Clear();
                    strQry.AppendLine(" SELECT");
                    strQry.AppendLine(" BACKUP_DATABASE_PATH");
                    strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_PATH");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Directory", -1);

                    string strBaseDirectory = DataSet.Tables["Directory"].Rows[0]["BACKUP_DATABASE_PATH"].ToString();

                    Cleanup_Files_Older_35_Days(parInt64CompanyNo, strBaseDirectory);
                }
                catch (Exception ex1)
                {
                }

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

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.CLOSE_RUN_QUEUE");

                strQry.AppendLine(" SET ");
                strQry.AppendLine(" CLOSE_RUN_QUEUE_IND = 'F'");
                strQry.AppendLine(",END_RUN_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND RUN_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));
                strQry.AppendLine(" AND WAGE_PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(pardtRunDate.ToString("yyyy-MM-dd")));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                
                intReturnCode = 1;
            }

            return intReturnCode;
        }

        public int Close_Both_Run(Int64 parInt64CompanyNo, DateTime pardtWageRunDate, DateTime pardtSalaryRunDate, string parstrWagesPayCategoryNumbers, string parstrSalariesPayCategoryNumbers, Int64 parint64CurrentUserNo)
        {
            int intReturnCode = 0;
            StringBuilder strQry = new StringBuilder();

            //2017-08-05
            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.CLOSE_RUN_QUEUE");

            strQry.AppendLine(" SET ");
            //S=Started
            strQry.AppendLine(" CLOSE_RUN_QUEUE_IND = 'S'");
            strQry.AppendLine(",START_RUN_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND RUN_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("B"));
            strQry.AppendLine(" AND WAGE_PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(pardtWageRunDate.ToString("yyyy-MM-dd")));
            strQry.AppendLine(" AND SALARY_PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(pardtSalaryRunDate.ToString("yyyy-MM-dd")));

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            intReturnCode = Close_Wage_Run(parInt64CompanyNo, pardtWageRunDate, parstrWagesPayCategoryNumbers, parint64CurrentUserNo);

            if (intReturnCode == 0)
            {
                intReturnCode = Close_Salary_Run(parInt64CompanyNo, pardtSalaryRunDate, parstrSalariesPayCategoryNumbers, parint64CurrentUserNo);

                if (intReturnCode == 0)
                {
                    //2017-07-11
                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.CLOSE_RUN_QUEUE_COMPLETED");

                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",RUN_TYPE");
                    strQry.AppendLine(",USER_NO");
                    strQry.AppendLine(",WAGE_PAY_PERIOD_DATE");
                    strQry.AppendLine(",SALARY_PAY_PERIOD_DATE");
                    strQry.AppendLine(",WAGES_PAY_CATEGORY_NUMBERS");
                    strQry.AppendLine(",SALARIES_PAY_CATEGORY_NUMBERS");
                    strQry.AppendLine(",CLOSE_RUN_QUEUE_IND");
                    strQry.AppendLine(",START_RUN_DATE");
                    strQry.AppendLine(",END_RUN_DATE)");

                    strQry.AppendLine(" SELECT ");

                    strQry.AppendLine(" COMPANY_NO");
                    strQry.AppendLine(",RUN_TYPE");
                    strQry.AppendLine(",USER_NO");
                    strQry.AppendLine(",WAGE_PAY_PERIOD_DATE");
                    strQry.AppendLine(",SALARY_PAY_PERIOD_DATE");
                    strQry.AppendLine(",WAGES_PAY_CATEGORY_NUMBERS");
                    strQry.AppendLine(",SALARIES_PAY_CATEGORY_NUMBERS");
                    strQry.AppendLine(",'C'");
                    strQry.AppendLine(",START_RUN_DATE");
                    strQry.AppendLine(",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                    strQry.AppendLine(" FROM InteractPayroll.dbo.CLOSE_RUN_QUEUE");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND RUN_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("B"));
                    strQry.AppendLine(" AND WAGE_PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(pardtWageRunDate.ToString("yyyy-MM-dd")));
                    strQry.AppendLine(" AND SALARY_PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(pardtSalaryRunDate.ToString("yyyy-MM-dd")));

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    //2017-07-11
                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.CLOSE_RUN_QUEUE");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND RUN_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("B"));
                    strQry.AppendLine(" AND WAGE_PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(pardtWageRunDate.ToString("yyyy-MM-dd")));
                    strQry.AppendLine(" AND SALARY_PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(pardtSalaryRunDate.ToString("yyyy-MM-dd")));

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                }
                else
                {
                    //Error from Close_Salary_Run
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE InteractPayroll.dbo.CLOSE_RUN_QUEUE");

                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" CLOSE_RUN_QUEUE_IND = 'F'");
                    strQry.AppendLine(",END_RUN_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND RUN_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("B"));
                    strQry.AppendLine(" AND WAGE_PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(pardtWageRunDate.ToString("yyyy-MM-dd")));
                    strQry.AppendLine(" AND SALARY_PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(pardtSalaryRunDate.ToString("yyyy-MM-dd")));

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                }
            }
            else
            {
                //Error from Close_Wage_Run
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.CLOSE_RUN_QUEUE");

                strQry.AppendLine(" SET ");
                strQry.AppendLine(" CLOSE_RUN_QUEUE_IND = 'F'");
                strQry.AppendLine(",END_RUN_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND RUN_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("B"));
                strQry.AppendLine(" AND WAGE_PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(pardtWageRunDate.ToString("yyyy-MM-dd")));
                strQry.AppendLine(" AND SALARY_PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(pardtSalaryRunDate.ToString("yyyy-MM-dd")));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            }
            
            return intReturnCode;
        }

        public int Close_Salary_Run(Int64 parInt64CompanyNo, DateTime pardtRunDate, string parstrSalaryPayCategoryNumbers, Int64 parint64CurrentUserNo)
        {
            string strClassNameFunctionAndParameters = pvtstrClassName + " Close_Salary_Run CompanyNo=" + parInt64CompanyNo + ",pardtRunDate=" + pardtRunDate.ToString("yyyy-MM-dd") + ",parstrSalaryPayCategoryNumbers=" + parstrSalaryPayCategoryNumbers + ",parint64CurrentUserNo=" + parint64CurrentUserNo.ToString();

            int intReturnCode = 0;
            StringBuilder strQry = new StringBuilder();

            try
            {
                //2017-08-05
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.CLOSE_RUN_QUEUE");

                strQry.AppendLine(" SET ");
                //S=Started
                strQry.AppendLine(" CLOSE_RUN_QUEUE_IND = 'S'");
                strQry.AppendLine(",START_RUN_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND RUN_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("S"));
                strQry.AppendLine(" AND SALARY_PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(pardtRunDate.ToString("yyyy-MM-dd")));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                
                string[] strPayCategoryArray = parstrSalaryPayCategoryNumbers.Split(',');

                DataSet DataSet = new DataSet();
              
                StringBuilder strFieldNamesInitialised = new StringBuilder();

                DateTime dtFiscalEndDateTime;
                DateTime dtFiscalBeginDateTime;

                //Find End of Fiscal Year
                if (pardtRunDate.Month > 2)
                {
                    dtFiscalBeginDateTime = new DateTime(pardtRunDate.Year, 3, 1);
                }
                else
                {
                    dtFiscalBeginDateTime = new DateTime(pardtRunDate.Year - 1, 3, 1);
                }

                //Last Day Of Fiscal Year
                dtFiscalEndDateTime = dtFiscalBeginDateTime.AddYears(1).AddDays(-1);
       
                //Run Through Pay Categories
                for (int intRow = 0; intRow < strPayCategoryArray.Length; intRow++)
                {
                    //Insert Current Portion For Pay Period
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",EARNING_NO");

                    strQry.AppendLine(",LEAVE_DESC");

                    strQry.AppendLine(",LEAVE_FROM_DATE");
                    strQry.AppendLine(",LEAVE_TO_DATE");

                    strQry.AppendLine(",LEAVE_ACCUM_DAYS");
                    strQry.AppendLine(",LEAVE_PAID_DAYS");

                    strQry.AppendLine(",LEAVE_OPTION");
                    strQry.AppendLine(",LEAVE_DAYS_DECIMAL");
                    strQry.AppendLine(",LEAVE_HOURS_DECIMAL");
                    strQry.AppendLine(",PROCESS_NO)");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" E.COMPANY_NO");

                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",LH.EARNING_NO");

                    strQry.AppendLine(",'Accumulated Days'");

                    strQry.AppendLine(",FROM_RUNDATE = ");

                    strQry.AppendLine(" CASE ");
                    //Run Stretches over 2 Different Fiscal Years
                    strQry.AppendLine(" WHEN E.EMPLOYEE_LAST_RUNDATE < '" + dtFiscalBeginDateTime.ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(" THEN '" + dtFiscalBeginDateTime.ToString("yyyy-MM-dd") + "'");
                    
                    //2014-03-25 - TakeOn Date 1st of Month
                    strQry.AppendLine(" WHEN DATEPART(dd,E.EMPLOYEE_LAST_RUNDATE) = 1");
                    
                    strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE ");

                    strQry.AppendLine(" ELSE DATEADD(d,1,E.EMPLOYEE_LAST_RUNDATE)");

                    strQry.AppendLine(" END "); 

                    //To Date
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(",LEAVE_ACCUM_DAYS = ");

                    strQry.AppendLine(" CASE");

                    //Changed Leave Link and ALREADY Exceeds Monthly Portion
                    strQry.AppendLine(" WHEN LH.EARNING_NO = 200 AND CURRENT_YEAR_ACCUM_TABLE.LEAVE_ACCUM_DAYS >= LSC.NORM_PAID_DAYS");
                    strQry.AppendLine(" THEN 0");

                    //Normally on 12 Month of Fiscal Year
                    strQry.AppendLine(" WHEN LH.EARNING_NO = 200 AND ROUND(LSC.NORM_PAID_PER_PERIOD + CURRENT_YEAR_ACCUM_TABLE.LEAVE_ACCUM_DAYS,2) + 0.15 > LSC.NORM_PAID_DAYS");
                    strQry.AppendLine(" THEN LSC.NORM_PAID_DAYS - ROUND(CURRENT_YEAR_ACCUM_TABLE.LEAVE_ACCUM_DAYS,2) ");

                    //Most Cases
                    strQry.AppendLine(" WHEN LH.EARNING_NO = 200 AND ISNULL(CURRENT_YEAR_ACCUM_TABLE.LEAVE_ACCUM_DAYS,0) < LSC.NORM_PAID_DAYS");
                    strQry.AppendLine(" THEN ROUND(LSC.NORM_PAID_PER_PERIOD,2)");

                    //Changed Leave Link and ALREADY Exceeds Monthly Portion
                    strQry.AppendLine(" WHEN LH.EARNING_NO = 201 AND CURRENT_YEAR_ACCUM_TABLE.LEAVE_ACCUM_DAYS >= LSC.SICK_PAID_DAYS");
                    strQry.AppendLine(" THEN 0");

                    //Normally on 12 Month of Fiscal Year
                    strQry.AppendLine(" WHEN LH.EARNING_NO = 201 AND ROUND(LSC.NORM_PAID_PER_PERIOD + CURRENT_YEAR_ACCUM_TABLE.LEAVE_ACCUM_DAYS,2) + 0.15 > LSC.SICK_PAID_DAYS");
                    strQry.AppendLine(" THEN LSC.SICK_PAID_DAYS - ROUND(CURRENT_YEAR_ACCUM_TABLE.LEAVE_ACCUM_DAYS,2) ");

                    //Most Cases
                    strQry.AppendLine(" WHEN LH.EARNING_NO = 201 AND ISNULL(CURRENT_YEAR_ACCUM_TABLE.LEAVE_ACCUM_DAYS,0) < LSC.SICK_PAID_DAYS");
                    strQry.AppendLine(" THEN ROUND(LSC.SICK_PAID_PER_PERIOD,2)");

                    strQry.AppendLine(" ELSE 0");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(",0");

                    strQry.AppendLine(",'D'");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",98");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EIC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EIC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EIC.PAY_CATEGORY_TYPE");
                    //Leave Accumulated even if Employee does Not get Payslip
                    //strQry.AppendLine(" AND EIC.PAYSLIP_IND = 'Y'");
                    strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");
                    //2016-01-09 - Not Close Employee and Remove Current Leave Allocation 
                    strQry.AppendLine(" AND ISNULL(EIC.CLOSE_REMOVE_EARNING_AND_LEAVE_IND,'N') <> 'Y'");
                    
                    //2014-03-29
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC ");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");

                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE ");

                    strQry.AppendLine(" AND EIC.RUN_TYPE = EPCC.RUN_TYPE");
                    //Default For Leave
                    strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y'");
                   
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY LH");

                    strQry.AppendLine(" ON E.COMPANY_NO = LH.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = LH.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND LH.EARNING_NO IN (200,201)");
                    //Take-On
                    strQry.AppendLine(" AND LH.PROCESS_NO = 99");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT_CURRENT LSC");
                    strQry.AppendLine(" ON E.COMPANY_NO = LSC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.LEAVE_SHIFT_NO = LSC.LEAVE_SHIFT_NO ");
                    strQry.AppendLine(" AND LSC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = LSC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" LEFT JOIN ");

                    strQry.AppendLine("( ");

                    strQry.AppendLine(" SELECT ");

                    strQry.AppendLine(" LH.COMPANY_NO");
                    strQry.AppendLine(",LH.EMPLOYEE_NO");
                    strQry.AppendLine(",LH.EARNING_NO");
                    strQry.AppendLine(",SUM(LH.LEAVE_ACCUM_DAYS) AS LEAVE_ACCUM_DAYS");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY LH");

                    strQry.AppendLine(" WHERE LH.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND LH.PAY_PERIOD_DATE >= '" + dtFiscalBeginDateTime.ToString("yyyy-MM-dd") + "'");

                    //Accumulated Leave / Take-On
                    strQry.AppendLine(" AND LH.PROCESS_NO IN (98,99)");
                    //Normal Leave / Sick Leave
                    strQry.AppendLine(" AND LH.EARNING_NO IN (200,201)");

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" LH.COMPANY_NO");
                    strQry.AppendLine(",LH.EMPLOYEE_NO");
                    strQry.AppendLine(",LH.EARNING_NO) AS CURRENT_YEAR_ACCUM_TABLE");

                    strQry.AppendLine(" ON E.COMPANY_NO = CURRENT_YEAR_ACCUM_TABLE.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = CURRENT_YEAR_ACCUM_TABLE.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND LH.EARNING_NO = CURRENT_YEAR_ACCUM_TABLE.EARNING_NO ");
                    
                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" GROUP BY ");

                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",E.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(",LH.EARNING_NO");
                    strQry.AppendLine(",LSC.NORM_PAID_PER_PERIOD ");
                    strQry.AppendLine(",LSC.NORM_PAID_DAYS ");
                    strQry.AppendLine(",LSC.SICK_PAID_PER_PERIOD ");

                    strQry.AppendLine(",LSC.SICK_PAID_DAYS ");
                    strQry.AppendLine(",CURRENT_YEAR_ACCUM_TABLE.LEAVE_ACCUM_DAYS ");
                   
                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                    
                    //Pay Out Normal Leave - EMPLOYEE Closed
                    //Pay Out Normal Leave - EMPLOYEE Closed
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",EARNING_NO");

                    strQry.AppendLine(",LEAVE_DESC");
                    strQry.AppendLine(",LEAVE_FROM_DATE");
                    strQry.AppendLine(",LEAVE_TO_DATE");

                    strQry.AppendLine(",LEAVE_ACCUM_DAYS");
                    strQry.AppendLine(",LEAVE_PAID_DAYS");

                    strQry.AppendLine(",LEAVE_OPTION");
                    strQry.AppendLine(",LEAVE_DAYS_DECIMAL");
                    strQry.AppendLine(",LEAVE_HOURS_DECIMAL");
                    strQry.AppendLine(",PROCESS_NO)");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EIC.COMPANY_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",EIC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EIC.EMPLOYEE_NO");
                    strQry.AppendLine(",EEC.EARNING_NO");

                    strQry.AppendLine(",'via Payroll Run (Closed)'");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(",0");

                    strQry.AppendLine(",DAY_DECIMAL_VALUE = ");

                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN ISNULL(EIC.CLOSE_REMOVE_EARNING_AND_LEAVE_IND,'N') = 'Y' ");

                    strQry.AppendLine(" THEN EEC.DAY_DECIMAL_OTHER_VALUE_ZERO ");

                    strQry.AppendLine(" ELSE ");

                    strQry.AppendLine(" EEC.DAY_DECIMAL_OTHER_VALUE");

                    strQry.AppendLine(" END ");
                    
                    //Errol - 2012-01-11
                    strQry.AppendLine(",'D'");
                    strQry.AppendLine(",0");

                    strQry.AppendLine(",HOURS_DECIMAL_VALUE = ");

                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN ISNULL(EIC.CLOSE_REMOVE_EARNING_AND_LEAVE_IND,'N') = 'Y' ");

                    strQry.AppendLine(" THEN EEC.HOURS_DECIMAL_OTHER_VALUE_ZERO");

                    strQry.AppendLine(" ELSE ");

                    strQry.AppendLine(" EEC.HOURS_DECIMAL_OTHER_VALUE");

                    strQry.AppendLine(" END ");
                    
                    strQry.AppendLine(",97");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC");

                    //Normal Leave - Total Oustanding Amount
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC ");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = EEC.COMPANY_NO");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EEC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EEC.EARNING_NO = 200");
                    strQry.AppendLine(" AND EEC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EEC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EIC.RUN_TYPE = EEC.RUN_TYPE");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC ");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");

                    strQry.AppendLine(" AND EEC.PAY_CATEGORY_NO = EPCC.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE ");

                    strQry.AppendLine(" AND EIC.RUN_TYPE = EPCC.RUN_TYPE");
                    //Default For Leave
                    strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y'");

                    strQry.AppendLine(" WHERE EIC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = 'S'");
                    //Employee is To Be Closed
                    strQry.AppendLine(" AND EIC.CLOSE_IND = 'Y'");

                    strQry.AppendLine(" ORDER BY ");
                    strQry.AppendLine(" EPCC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EIC.EMPLOYEE_NO");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //Move LEAVE_CURRENT to LEAVE_HISTORY
                    //Move LEAVE_CURRENT to LEAVE_HISTORY
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",EARNING_NO");
                    //strQry.AppendLine(",LEAVE_REC_NO");
                    strQry.AppendLine(",LEAVE_DESC");

                    strQry.AppendLine(",LEAVE_FROM_DATE");
                    strQry.AppendLine(",LEAVE_TO_DATE");

                    strQry.AppendLine(",LEAVE_ACCUM_DAYS");
                    strQry.AppendLine(",LEAVE_PAID_DAYS");

                    strQry.AppendLine(",LEAVE_OPTION");
                    strQry.AppendLine(",LEAVE_DAYS_DECIMAL");
                    strQry.AppendLine(",LEAVE_HOURS_DECIMAL");
                    strQry.AppendLine(",PROCESS_NO)");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EIC.COMPANY_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",EIC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",L.EMPLOYEE_NO");
                    strQry.AppendLine(",L.EARNING_NO");
                    //strQry.AppendLine(",100 + L.LEAVE_REC_NO");
                    strQry.AppendLine(",L.LEAVE_DESC");

                    strQry.AppendLine(",L.LEAVE_FROM_DATE");
                    strQry.AppendLine(",L.LEAVE_TO_DATE");

                    strQry.AppendLine(",0");
                    //NB LEAVE_DAYS_DECIMAL Gets moved to LEAVE_PAID_DAYS
                    strQry.AppendLine(",ROUND(L.LEAVE_DAYS_DECIMAL,2)");

                    strQry.AppendLine(",L.LEAVE_OPTION");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    
                    strQry.AppendLine(",L.PROCESS_NO");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y'");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT L");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = L.COMPANY_NO");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = L.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = L.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND L.PROCESS_NO = 0");

                    strQry.AppendLine(" WHERE EIC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = 'S'");
                    //Gets a Payslip
                    strQry.AppendLine(" AND (EIC.PAYSLIP_IND = 'Y'");
                    strQry.AppendLine(" OR EIC.CLOSE_IND = 'Y')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //Delete LEAVE_CURRENT that was Moved to LEAVE_HISTORY
                    //Delete LEAVE_CURRENT that was Moved to LEAVE_HISTORY
                    strQry.Clear();
                    strQry.AppendLine(" DELETE LC");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT LC");

                    strQry.AppendLine(" INNER JOIN ");

                    strQry.AppendLine("(SELECT");

                    strQry.AppendLine(" L.COMPANY_NO");
                    strQry.AppendLine(",L.EMPLOYEE_NO");
                    strQry.AppendLine(",L.EARNING_NO");
                    strQry.AppendLine(",L.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",L.LEAVE_REC_NO");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y'");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT L");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = L.COMPANY_NO");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = L.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = L.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND L.PROCESS_NO = 0");

                    strQry.AppendLine(" WHERE EIC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = 'S'");

                    //Gets a Payslip
                    strQry.AppendLine(" AND (EIC.PAYSLIP_IND = 'Y'");
                    strQry.AppendLine(" OR EIC.CLOSE_IND = 'Y')) AS LEAVE_TEMP_TABLE");

                    strQry.AppendLine(" ON LC.COMPANY_NO = LEAVE_TEMP_TABLE.COMPANY_NO");
                    strQry.AppendLine(" AND LC.EMPLOYEE_NO = LEAVE_TEMP_TABLE.EMPLOYEE_NO");
                    strQry.AppendLine(" AND LC.EARNING_NO = LEAVE_TEMP_TABLE.EARNING_NO");
                    strQry.AppendLine(" AND LC.PAY_CATEGORY_TYPE = LEAVE_TEMP_TABLE.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND LC.LEAVE_REC_NO = LEAVE_TEMP_TABLE.LEAVE_REC_NO");

                    strQry.AppendLine(" WHERE LC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND LC.PAY_CATEGORY_TYPE = 'S'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                   
                    strQry.Clear();
                    strQry.AppendLine(" DELETE EEC FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC ");
                    strQry.AppendLine(" ON EEC.COMPANY_NO = EIC.COMPANY_NO ");
                    strQry.AppendLine(" AND EEC.EMPLOYEE_NO = EIC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EEC.RUN_TYPE = EIC.RUN_TYPE");
                    strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = EIC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EEC.RUN_NO = EIC.RUN_NO ");
                    //No Payslip
                    strQry.AppendLine(" AND EIC.PAYSLIP_IND = 'N'");
                    strQry.AppendLine(" AND EIC.CLOSE_IND <> 'Y'");

                    strQry.AppendLine(" WHERE EEC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EEC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND EEC.RUN_TYPE = 'P'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE EEWC FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_WEEK_CURRENT EEWC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC ");
                    strQry.AppendLine(" ON EEWC.COMPANY_NO = EIC.COMPANY_NO ");
                    strQry.AppendLine(" AND EEWC.EMPLOYEE_NO = EIC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EEWC.RUN_NO = EIC.RUN_NO ");
                    //No Payslip
                    strQry.AppendLine(" AND EIC.PAYSLIP_IND = 'N'");
                    strQry.AppendLine(" AND EIC.CLOSE_IND <> 'Y'");
                    strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");

                    strQry.AppendLine(" WHERE EEWC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EEWC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE EPCC FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC ");
                    strQry.AppendLine(" ON EPCC.COMPANY_NO = EIC.COMPANY_NO ");
                    strQry.AppendLine(" AND EPCC.EMPLOYEE_NO = EIC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = EIC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EPCC.RUN_TYPE = EIC.RUN_TYPE ");
                    strQry.AppendLine(" AND EPCC.RUN_NO = EIC.RUN_NO ");
                    //No Payslip
                    strQry.AppendLine(" AND EIC.PAYSLIP_IND = 'N'");
                    strQry.AppendLine(" AND EIC.CLOSE_IND <> 'Y'");

                    strQry.AppendLine(" WHERE EPCC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE EDC FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT EDC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC ");
                    strQry.AppendLine(" ON EDC.COMPANY_NO = EIC.COMPANY_NO ");
                    strQry.AppendLine(" AND EDC.EMPLOYEE_NO = EIC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EDC.PAY_CATEGORY_TYPE = EIC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EDC.RUN_TYPE = EIC.RUN_TYPE ");
                    strQry.AppendLine(" AND EDC.RUN_NO = EIC.RUN_NO ");
                    //No Payslip
                    strQry.AppendLine(" AND EIC.PAYSLIP_IND = 'N'");
                    strQry.AppendLine(" AND EIC.CLOSE_IND <> 'Y'");

                    strQry.AppendLine(" WHERE EDC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EDC.RUN_TYPE = 'P'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE EDEPC FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE_CURRENT EDEPC ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC ");
                    strQry.AppendLine(" ON EDEPC.COMPANY_NO = EIC.COMPANY_NO ");
                    strQry.AppendLine(" AND EDEPC.EMPLOYEE_NO = EIC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EDEPC.PAY_CATEGORY_TYPE = EIC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EDEPC.RUN_NO = EIC.RUN_NO ");
                    strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");
                    //No Payslip
                    strQry.AppendLine(" AND EIC.PAYSLIP_IND = 'N'");
                    strQry.AppendLine(" AND EIC.CLOSE_IND <> 'Y'");

                    strQry.AppendLine(" WHERE EDEPC.COMPANY_NO = " + parInt64CompanyNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" UPDATE E");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" E.EMPLOYEE_LAST_RUNDATE = '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    //Errol - 2015-02-17
                    strQry.AppendLine(",E.FIRST_RUN_COMPLETED_IND = 'Y'");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC ");
                    strQry.AppendLine(" ON E.COMPANY_NO = EIC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EIC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EIC.PAY_CATEGORY_TYPE ");
                    //No Payslip
                    strQry.AppendLine(" AND EIC.PAYSLIP_IND = 'N'");
                    strQry.AppendLine(" AND EIC.CLOSE_IND <> 'Y'");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND RUN_TYPE = 'P'");
                    //No Payslip
                    strQry.AppendLine(" AND PAYSLIP_IND = 'N'");
                    strQry.AppendLine(" AND CLOSE_IND <> 'Y'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //Delete Deduction Earning Percenatge Records Where Totals are Zero
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND STR(EMPLOYEE_NO) + STR(DEDUCTION_NO) + STR(DEDUCTION_SUB_ACCOUNT_NO) IN ");
                    strQry.AppendLine("(SELECT STR(EIC.EMPLOYEE_NO) + STR(EDC.DEDUCTION_NO) + STR(EDC.DEDUCTION_SUB_ACCOUNT_NO) ");

                    strQry.AppendLine(" FROM  InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT EDC");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = EDC.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EDC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EDC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EDC.RUN_TYPE = EIC.RUN_TYPE");
                    strQry.AppendLine(" AND EDC.TOTAL = 0 ");

                    strQry.AppendLine(" INNER JOIN ");
                    strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);

                    strQry.AppendLine(" INNER JOIN ");
                    strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" LEFT JOIN ");
                    strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY EDH");
                    strQry.AppendLine(" ON EDC.COMPANY_NO = EDH.COMPANY_NO ");
                    strQry.AppendLine(" AND EDC.EMPLOYEE_NO = EDH.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EDC.DEDUCTION_NO = EDH.DEDUCTION_NO");
                    strQry.AppendLine(" AND EDC.DEDUCTION_SUB_ACCOUNT_NO = EDH.DEDUCTION_SUB_ACCOUNT_NO");
                    //Removed to Add Take-On Totals
                    //strQry.AppendLine(" AND EDH.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EDH.PAY_PERIOD_DATE >= '" + dtFiscalBeginDateTime.ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(" WHERE EIC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND (EIC.CLOSE_IND <> 'Y'");
                    strQry.AppendLine(" OR EIC.CLOSE_IND IS NULL)");

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" EIC.EMPLOYEE_NO");
                    strQry.AppendLine(",EDC.DEDUCTION_NO");
                    strQry.AppendLine(",EDC.DEDUCTION_SUB_ACCOUNT_NO ");

                    strQry.AppendLine(" HAVING ISNULL(SUM(EDH.TOTAL),0) = 0)");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //Delete Deduction Records Where Totals are Zero
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND RUN_TYPE = 'P'");
                    //Tax Record is Used in JOIN for TAX YTD for Tax Module
                    strQry.AppendLine(" AND DEDUCTION_NO <> 1 ");
                    strQry.AppendLine(" AND STR(EMPLOYEE_NO) + STR(DEDUCTION_NO) + STR(DEDUCTION_SUB_ACCOUNT_NO) IN ");
                    strQry.AppendLine("(SELECT STR(EIC.EMPLOYEE_NO) + STR(EDC.DEDUCTION_NO) + STR(EDC.DEDUCTION_SUB_ACCOUNT_NO) ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC");

                    strQry.AppendLine(" INNER JOIN ");
                    strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT EDC");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = EDC.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EDC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EDC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EDC.RUN_TYPE = EIC.RUN_TYPE");
                    strQry.AppendLine(" AND EDC.TOTAL = 0 ");

                    strQry.AppendLine(" INNER JOIN ");
                    strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);

                    strQry.AppendLine(" INNER JOIN ");
                    strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" LEFT JOIN ");
                    strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY EDH");
                    strQry.AppendLine(" ON EDC.COMPANY_NO = EDH.COMPANY_NO ");
                    strQry.AppendLine(" AND EDC.EMPLOYEE_NO = EDH.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EDC.DEDUCTION_NO = EDH.DEDUCTION_NO");
                    strQry.AppendLine(" AND EDC.DEDUCTION_SUB_ACCOUNT_NO = EDH.DEDUCTION_SUB_ACCOUNT_NO");
                    //Removed to Add Take-On Totals
                    //strQry.AppendLine(" AND EDH.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EDH.PAY_PERIOD_DATE >= '" + dtFiscalBeginDateTime.ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(" WHERE EIC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND (EIC.CLOSE_IND <> 'Y'");
                    strQry.AppendLine(" OR EIC.CLOSE_IND IS NULL)");

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" EIC.EMPLOYEE_NO");
                    strQry.AppendLine(",EDC.DEDUCTION_NO");
                    strQry.AppendLine(",EDC.DEDUCTION_SUB_ACCOUNT_NO ");

                    strQry.AppendLine(" HAVING ISNULL(SUM(EDH.TOTAL),0) = 0)");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //Delete Leave Earning Records Where Totals are Zero
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND RUN_TYPE = 'P'");
                    //Leave Records
                    strQry.AppendLine(" AND EARNING_NO > 19");
                    strQry.AppendLine(" AND STR(EMPLOYEE_NO) + STR(EARNING_NO) IN ");
                    strQry.AppendLine("(SELECT STR(EIC.EMPLOYEE_NO) + STR(EEC.EARNING_NO) ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC");

                    strQry.AppendLine(" INNER JOIN ");
                    strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = EEC.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EEC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EEC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EEC.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EEC.RUN_TYPE = EIC.RUN_TYPE");
                    strQry.AppendLine(" AND EEC.TOTAL = 0");
                    //strQry.AppendLine(" AND EEC.TOTAL_YTD_BF = 0 ");

                    strQry.AppendLine(" INNER JOIN ");
                    strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);

                    strQry.AppendLine(" INNER JOIN ");
                    strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON EIC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
                    strQry.AppendLine(" ON EEC.COMPANY_NO = EEH.COMPANY_NO ");
                    strQry.AppendLine(" AND EEC.EMPLOYEE_NO = EEH.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EEC.EARNING_NO = EEH.EARNING_NO");
                    strQry.AppendLine(" AND EEC.PAY_CATEGORY_NO = EEH.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE");
                    //Removed to Add Take-On Totals
                    //strQry.AppendLine(" AND EEH.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EEH.PAY_PERIOD_DATE >= '" + dtFiscalBeginDateTime.ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(" WHERE EIC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND (EIC.CLOSE_IND <> 'Y'");
                    strQry.AppendLine(" OR EIC.CLOSE_IND IS NULL)");

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" EIC.EMPLOYEE_NO");
                    strQry.AppendLine(",EEC.EARNING_NO ");

                    strQry.AppendLine(" HAVING ISNULL(SUM(EEH.TOTAL),0) = 0)");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY ");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_EARNING_HISTORY", ref strQry, ref strFieldNamesInitialised, "EEC", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EEC.COMPANY_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");
                    strQry.AppendLine(" WHERE EEC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EEC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND EEC.RUN_TYPE = 'P'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK_HISTORY ");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("PAY_CATEGORY_BREAK_HISTORY", ref strQry, ref strFieldNamesInitialised, "PCBC", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" PCBC.COMPANY_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK_CURRENT PCBC");
                    strQry.AppendLine(" WHERE PCBC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PCBC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PCBC.PAY_CATEGORY_TYPE = 'S'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY ");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_DEDUCTION_CURRENT", ref strQry, ref strFieldNamesInitialised, "EDC", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EDC.COMPANY_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT EDC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON EDC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EDC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P'");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON EDC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND EDC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE EDC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EDC.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND EDC.RUN_TYPE = 'P'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE_HISTORY ");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE_CURRENT", ref strQry, ref strFieldNamesInitialised, "EDEPC", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EDEPC.COMPANY_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE_CURRENT EDEPC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON EDEPC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EDEPC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EDEPC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P' ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON EDEPC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND EDEPC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE EDEPC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EDEPC.PAY_CATEGORY_TYPE = 'S'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //ERROL still to FIX to Move Timesheet to0 Correct Month (Pay period)
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_HISTORY ");

                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT", ref strQry, ref strFieldNamesInitialised, "ETATBDC", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" ETATBDC.COMPANY_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT ETATBDC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                    strQry.AppendLine(" ON ETATBDC.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND ETATBDC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND ETATBDC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");

                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");
                    
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON ETATBDC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND ETATBDC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE ETATBDC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND ETATBDC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);

                    strQry.AppendLine(" AND ETATBDC.TIMESHEET_DATE > ");

                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN E.FIRST_RUN_COMPLETED_IND = 'Y'");

                    strQry.AppendLine(" THEN E.EMPLOYEE_LAST_RUNDATE   ");

                    strQry.AppendLine(" ELSE DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(" AND ETATBDC.TIMESHEET_DATE <= '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_HISTORY ");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_SALARY_TIMESHEET_CURRENT", ref strQry, ref strFieldNamesInitialised, "ETC", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" ETC.COMPANY_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC");

                    //Errol 2014-02-21
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                    strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");

                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");
                    //Errol 2014-02-21

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    //Errol - 2015-02-17
                    //strQry.AppendLine(" AND ETC.TIMESHEET_DATE > E.EMPLOYEE_LAST_RUNDATE");

                    strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);

                    //Errol - 2015-02-12
                    strQry.AppendLine(" AND ((ETC.TIMESHEET_DATE > E.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND E.FIRST_RUN_COMPLETED_IND = 'Y')");
                    strQry.AppendLine(" OR (ETC.TIMESHEET_DATE >= E.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                    strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //2012-11-20
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_HISTORY ");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_SALARY_BREAK_CURRENT", ref strQry, ref strFieldNamesInitialised, "ETC", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" ETC.COMPANY_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ETC");

                    //Errol 2014-02-21
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                    strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");

                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");
                    //Errol 2014-02-21

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    //Errol - 2015-02-17
                    //strQry.AppendLine(" AND ETC.BREAK_DATE > E.EMPLOYEE_LAST_RUNDATE");

                    strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);

                    //Errol - 2015-02-17
                    strQry.AppendLine(" AND ((ETC.BREAK_DATE > E.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND E.FIRST_RUN_COMPLETED_IND = 'Y')");
                    strQry.AppendLine(" OR (ETC.BREAK_DATE >= E.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                    strQry.AppendLine(" AND ETC.BREAK_DATE <= '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //Leave
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_HISTORY ");
                    strQry.AppendLine("(PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT", ref strQry, ref strFieldNamesInitialised, "EPCLAC", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine("'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT EPCLAC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON EPCLAC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND EPCLAC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCLAC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE EPCLAC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EPCLAC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EPCLAC.PAY_CATEGORY_TYPE = 'S'");
                    //Only Authorised Records
                    strQry.AppendLine(" AND EPCLAC.AUTHORISED_IND = 'Y'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_WEEK_HISTORY ");
                    strQry.AppendLine("(PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_EARNING_WEEK_CURRENT", ref strQry, ref strFieldNamesInitialised, "EEWC", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine("'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_WEEK_CURRENT EEWC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON EEWC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND EEWC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE EEWC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EEWC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.LOANS ");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",DEDUCTION_NO");
                    strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
                    strQry.AppendLine(",LOAN_PROCESSED_DATE");
                    strQry.AppendLine(",LOAN_AMOUNT_RECEIVED");
                    strQry.AppendLine(",LOAN_REC_NO");
                    strQry.AppendLine(",LOAN_DESC");
                    strQry.AppendLine(",LOAN_AMOUNT_PAID");
                    strQry.AppendLine(",PROCESS_NO");
                    strQry.AppendLine(",USER_NO_NEW_RECORD");
                    strQry.AppendLine(",DATETIME_NEW_RECORD)");

                    strQry.AppendLine(" SELECT DISTINCT");
                    strQry.AppendLine(" EDC.COMPANY_NO");
                    strQry.AppendLine(",EDC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EDC.EMPLOYEE_NO");
                    strQry.AppendLine(",EDC.DEDUCTION_NO");
                    strQry.AppendLine(",EDC.DEDUCTION_SUB_ACCOUNT_NO");
                    strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",EDC.TOTAL");
                    strQry.AppendLine(",ISNULL(MAX(L.LOAN_REC_NO),0) + 1");
                    strQry.AppendLine(",'via Payroll Run'");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",99");
                    strQry.AppendLine("," + parint64CurrentUserNo);
                    strQry.AppendLine(",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "'");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT EDC");
                    strQry.AppendLine(" ON EDC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND EDC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EDC.RUN_TYPE = 'P'");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON EDC.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND EDC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y'");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.DEDUCTION D");
                    strQry.AppendLine(" ON EDC.COMPANY_NO = D.COMPANY_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = D.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EDC.DEDUCTION_NO = D.DEDUCTION_NO");
                    strQry.AppendLine(" AND EDC.DEDUCTION_SUB_ACCOUNT_NO = D.DEDUCTION_SUB_ACCOUNT_NO");
                    strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND D.DEDUCTION_LOAN_TYPE_IND = 'Y'");

                    //All - Even Deleted Records
                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.LOANS L");
                    strQry.AppendLine(" ON EDC.COMPANY_NO = L.COMPANY_NO");
                    strQry.AppendLine(" AND EDC.PAY_CATEGORY_TYPE = L.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EDC.EMPLOYEE_NO = L.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EDC.DEDUCTION_NO = L.DEDUCTION_NO");
                    strQry.AppendLine(" AND EDC.DEDUCTION_SUB_ACCOUNT_NO = L.DEDUCTION_SUB_ACCOUNT_NO");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EDC.TOTAL <> 0");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" EDC.COMPANY_NO");
                    strQry.AppendLine(",EDC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EDC.EMPLOYEE_NO");
                    strQry.AppendLine(",EDC.DEDUCTION_NO");
                    strQry.AppendLine(",EDC.DEDUCTION_SUB_ACCOUNT_NO");
                    strQry.AppendLine(",EDC.TOTAL");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_HISTORY ");
                    strQry.AppendLine("(PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("PAY_CATEGORY_WEEK_CURRENT", ref strQry, ref strFieldNamesInitialised, "", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine("'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_CURRENT");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT_HISTORY ");
                    strQry.AppendLine("(PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("LEAVE_SHIFT_CURRENT", ref strQry, ref strFieldNamesInitialised, "", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine("'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT_CURRENT");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    //2014-03-25
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY ");
                    strQry.AppendLine("(PAY_PERIOD_DATE");

                    //Return strings of field names that need to be Initialised
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("PAY_CATEGORY_PERIOD_CURRENT", ref strQry, ref strFieldNamesInitialised, "", parInt64CompanyNo);

                    strQry.AppendLine(")");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine("'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                    //Append Initialised Numeric Fields Names
                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND RUN_TYPE = 'P'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.LOANS");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" LOAN_PROCESSED_DATE = '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND LOAN_PROCESSED_DATE IS NULL");
                    strQry.AppendLine(" AND PROCESS_NO = 0");
                    strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT EMPLOYEE_NO ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND DEFAULT_IND = 'Y')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" EMPLOYEE_LAST_RUNDATE = '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    //Errol - 2015-02-17
                    strQry.AppendLine(",FIRST_RUN_COMPLETED_IND = 'Y'");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT EMPLOYEE_NO ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND DEFAULT_IND = 'Y')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //Employee Closed Off
                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" EMPLOYEE_ENDDATE = '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT EPCC.EMPLOYEE_NO ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC");
                    strQry.AppendLine(" ON EPCC.COMPANY_NO = EIC.COMPANY_NO ");
                    strQry.AppendLine(" AND EPCC.EMPLOYEE_NO = EIC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = EIC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EIC.CLOSE_IND = 'Y'");
                    strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");

                    strQry.AppendLine(" WHERE EPCC.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                    //ERROL Added 2011-06-29
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);

                    strQry.AppendLine(" AND RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT E.EMPLOYEE_NO ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT E.EMPLOYEE_NO ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON  E.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT E.EMPLOYEE_NO ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");

                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT E.EMPLOYEE_NO ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT EPCC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = 'S'");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");

                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT E.EMPLOYEE_NO ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT EPCLAC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPCLAC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCLAC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCLAC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EPCLAC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_WEEK_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT E.EMPLOYEE_NO ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND TIMESHEET_DATE <= '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT E.EMPLOYEE_NO ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //2012-11-20
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND BREAK_DATE <= '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT E.EMPLOYEE_NO ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                     
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //Errol Added 2011-11-02
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    //2014-03-25
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND RUN_TYPE = 'P'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PROCESS_NO = 0");
                    //Normal Leave
                    strQry.AppendLine(" AND EARNING_NO = 200");
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                    strQry.AppendLine("(SELECT E.EMPLOYEE_NO ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    //Employee Closed
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EIC.COMPANY_NO ");
                    strQry.AppendLine(" AND EIC.CLOSE_IND = 'Y'");
                    strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EIC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EIC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + strPayCategoryArray[intRow]);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y'");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL)");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //END Of PAY_CATEGORY LOOP
                }
                
                //2018-09-06 Fix Duplicate PUBLIC_HOLIDAY_HISTORY Record Insert when 2 Different Runs Overlap
                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_HISTORY ");
                strQry.AppendLine("(PAY_PERIOD_DATE");
                strQry.AppendLine(",COMPANY_NO");
                strQry.AppendLine(",PUBLIC_HOLIDAY_DATE");
                strQry.AppendLine(",RUN_NO");
                strQry.AppendLine(",PUBLIC_HOLIDAY_DESC)");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine("'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(",PHC.COMPANY_NO");
                strQry.AppendLine(",PHC.PUBLIC_HOLIDAY_DATE");
                strQry.AppendLine(",PHC.RUN_NO");
                strQry.AppendLine(",PHC.PUBLIC_HOLIDAY_DESC");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_CURRENT PHC");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_HISTORY PHH");
                strQry.AppendLine(" ON PHC.COMPANY_NO = PHH.COMPANY_NO");
                strQry.AppendLine(" AND PHH.PAY_PERIOD_DATE = '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" AND PHC.PUBLIC_HOLIDAY_DATE = PHH.PUBLIC_HOLIDAY_DATE");
                strQry.AppendLine(" AND PHC.RUN_NO = PHH.RUN_NO");

                strQry.AppendLine(" WHERE PHC.COMPANY_NO = " + parInt64CompanyNo);

                //Record Does NOT Exist
                strQry.AppendLine(" AND PHH.COMPANY_NO IS NULL ");
                
                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_PERIOD_DATE");

                //Return strings of field names that need to be Initialised
                clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_INFO_CURRENT", ref strQry, ref strFieldNamesInitialised, "EIC", parInt64CompanyNo);

                strQry.AppendLine(")");
                strQry.AppendLine(" SELECT DISTINCT");
                strQry.AppendLine(" EIC.COMPANY_NO");
                strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                //Append Initialised Numeric Fields Names
                strQry.Append(strFieldNamesInitialised);

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");
                strQry.AppendLine(" ON EIC.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND EIC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                strQry.AppendLine(" ON EIC.COMPANY_NO = EPCC.COMPANY_NO ");
                strQry.AppendLine(" AND EIC.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + parstrSalaryPayCategoryNumbers + ")");

                strQry.AppendLine(" WHERE EIC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EIC.PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND EIC.RUN_TYPE = 'P'");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_PERIOD_DATE");

                //Return strings of field names that need to be Initialised
                clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_PAY_CATEGORY_CURRENT", ref strQry, ref strFieldNamesInitialised, "EPCC", parInt64CompanyNo);

                //2017-07-31
                strQry = strQry.Replace(",PAY_CATEGORY_USED_IND", "");
                //2017-07-31
                strFieldNamesInitialised = strFieldNamesInitialised.Replace(",EPCC.PAY_CATEGORY_USED_IND", "");

                strQry.AppendLine(")");
                strQry.AppendLine(" SELECT DISTINCT");
                strQry.AppendLine(" E.COMPANY_NO");
                strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");

                //Append Initialised Numeric Fields Names
                strQry.Append(strFieldNamesInitialised);

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                strQry.AppendLine(" ON EPCC.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND EPCC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + parstrSalaryPayCategoryNumbers + ")");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P'");

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //ELR 2014-05-01
                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.OCCUPATION_HISTORY");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_PERIOD_DATE");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",OCCUPATION_NO");
                strQry.AppendLine(",OCCUPATION_DESC)");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" O.COMPANY_NO");
                strQry.AppendLine(",'" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(",'S'");
                strQry.AppendLine(",O.OCCUPATION_NO");
                strQry.AppendLine(",O.OCCUPATION_DESC ");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.OCCUPATION O ");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.OCCUPATION_HISTORY OH ");
                strQry.AppendLine(" ON O.COMPANY_NO = OH.COMPANY_NO ");
                strQry.AppendLine(" AND OH.PAY_PERIOD_DATE = '" + pardtRunDate.ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" AND OH.PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND O.OCCUPATION_NO = OH.OCCUPATION_NO ");

                strQry.AppendLine(" WHERE O.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND O.DATETIME_DELETE_RECORD IS NULL");

                //Does Not Exist
                strQry.AppendLine(" AND OH.COMPANY_NO IS NULL ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //Delete Loans For Closed Employee 
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.LOANS");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" DATETIME_DELETE_RECORD = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "'");
                strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND LOAN_PROCESSED_DATE IS NULL");
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND PROCESS_NO <> 0");
                strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                strQry.AppendLine("(SELECT EMPLOYEE_NO ");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT ");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND RUN_TYPE = 'P'");
                strQry.AppendLine(" AND CLOSE_IND = 'Y')");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //Delete Leave For Closed Employee
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND PROCESS_NO <> 0");
                strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                strQry.AppendLine("(SELECT EMPLOYEE_NO ");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT ");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND RUN_TYPE = 'P'");
                strQry.AppendLine(" AND CLOSE_IND = 'Y')");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //Rollup Loans for Active Employees 
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.LOANS");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" PROCESS_NO = PROCESS_NO - 1");
                strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND LOAN_PROCESSED_DATE IS NULL");
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND PROCESS_NO > 0");
                strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                strQry.AppendLine("(SELECT EMPLOYEE_NO ");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT ");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND RUN_TYPE = 'P'");
                strQry.AppendLine(" AND CLOSE_IND = 'N')");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //Rollup Leave for Active Employees 
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" PROCESS_NO = PROCESS_NO - 1");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND PROCESS_NO > 0");
                strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                strQry.AppendLine("(SELECT EMPLOYEE_NO ");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT ");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND RUN_TYPE = 'P'");
                strQry.AppendLine(" AND CLOSE_IND = 'N')");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //Delete These 2 Records Last - Used in Delete Queries
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND RUN_TYPE = 'P'");
                strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                strQry.AppendLine("(SELECT EMPLOYEE_NO ");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND RUN_TYPE = 'P'");
                strQry.AppendLine(" AND PAY_CATEGORY_NO IN (" + parstrSalaryPayCategoryNumbers + "))");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //2013-09-26
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_BATCH_TEMP");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND PROCESS_NO = 0");
                
                strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                strQry.AppendLine("(SELECT EMPLOYEE_NO ");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND RUN_TYPE = 'P'");
                strQry.AppendLine(" AND PAY_CATEGORY_NO IN (" + parstrSalaryPayCategoryNumbers + "))");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //2013-09-26
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_BATCH_TEMP");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND PROCESS_NO = 0");

                strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                strQry.AppendLine("(SELECT EMPLOYEE_NO ");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND RUN_TYPE = 'P'");
                strQry.AppendLine(" AND PAY_CATEGORY_NO IN (" + parstrSalaryPayCategoryNumbers + "))");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                strQry.AppendLine("(SELECT EMPLOYEE_NO ");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND PAY_CATEGORY_NO IN (" + parstrSalaryPayCategoryNumbers + "))");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //2014-03-29 (Cleanup Orphan Records)
                strQry.Clear();
                strQry.AppendLine(" DELETE ETC");

                strQry.AppendLine(" FROM  InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC   ");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EPC.COMPANY_NO IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                //2014-03-29 (Cleanup Orphan Records)
                strQry.Clear();
                strQry.AppendLine(" DELETE ETC");

                strQry.AppendLine(" FROM  InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ETC   ");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EPC.COMPANY_NO IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.COMPANY");
                
                strQry.AppendLine(" SET SALARY_RUN_IND = NULL");
                
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                
                //2017-08-05
                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.CLOSE_RUN_QUEUE_COMPLETED");

                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",RUN_TYPE");
                strQry.AppendLine(",USER_NO");
                strQry.AppendLine(",WAGE_PAY_PERIOD_DATE");
                strQry.AppendLine(",SALARY_PAY_PERIOD_DATE");

                strQry.AppendLine(",WAGES_PAY_CATEGORY_NUMBERS");
                strQry.AppendLine(",SALARIES_PAY_CATEGORY_NUMBERS");
                strQry.AppendLine(",CLOSE_RUN_QUEUE_IND");

                strQry.AppendLine(",START_RUN_DATE");
                strQry.AppendLine(",END_RUN_DATE)");

                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" COMPANY_NO");
                strQry.AppendLine(",RUN_TYPE");
                strQry.AppendLine(",USER_NO");
                strQry.AppendLine(",WAGE_PAY_PERIOD_DATE");
                strQry.AppendLine(",SALARY_PAY_PERIOD_DATE");

                strQry.AppendLine(",WAGES_PAY_CATEGORY_NUMBERS");
                strQry.AppendLine(",SALARIES_PAY_CATEGORY_NUMBERS");
                strQry.AppendLine(",'C'");
                strQry.AppendLine(",START_RUN_DATE");
                strQry.AppendLine(",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" FROM InteractPayroll.dbo.CLOSE_RUN_QUEUE");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND RUN_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("S"));
                strQry.AppendLine(" AND SALARY_PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(pardtRunDate.ToString("yyyy-MM-dd")));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                //2017-08-05
                strQry.Clear();

                strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.CLOSE_RUN_QUEUE");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND RUN_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("S"));
                strQry.AppendLine(" AND SALARY_PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(pardtRunDate.ToString("yyyy-MM-dd")));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                
                //Cleanup of Backups (IF Fails - Who Cares?)
                try
                {
                    FileInfo fiFileInfo;
                    
                    strQry.Clear();
                    strQry.AppendLine(" SELECT");
                    strQry.AppendLine(" BACKUP_DATABASE_NO");
                    strQry.AppendLine(",BACKUP_FILE_NAME");

                    strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_DATETIME");

                    strQry.AppendLine(" WHERE BACKUP_DATABASE_NAME = 'InteractPayroll_" + parInt64CompanyNo.ToString("00000") + "'");
                    strQry.AppendLine(" AND BACKUP_DATETIME < '" + DateTime.Now.AddDays(-35).ToString("yyyy-MM-dd") + "'");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "File", -1);

                    for (int intRow = 0; intRow < DataSet.Tables["File"].Rows.Count; intRow++)
                    {
                        fiFileInfo = new FileInfo(DataSet.Tables["File"].Rows[intRow]["BACKUP_FILE_NAME"].ToString());

                        if (fiFileInfo.Exists == true)
                        {
                            File.Delete(DataSet.Tables["File"].Rows[intRow]["BACKUP_FILE_NAME"].ToString());
                        }

                        strQry.Clear();
                        strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.BACKUP_DATABASE_DATETIME");
                        strQry.AppendLine(" WHERE BACKUP_DATABASE_NO = " + DataSet.Tables["File"].Rows[intRow]["BACKUP_DATABASE_NO"].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                    }

                    //Cleanup Any Backup File Older than 35 Days
                    strQry.Clear();
                    strQry.AppendLine(" SELECT");
                    strQry.AppendLine(" BACKUP_DATABASE_PATH");
                    strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_PATH");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Directory", -1);

                    string strBaseDirectory = DataSet.Tables["Directory"].Rows[0]["BACKUP_DATABASE_PATH"].ToString();

                    Cleanup_Files_Older_35_Days(parInt64CompanyNo, strBaseDirectory);
                }
                catch (Exception ex1)
                {
                }

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

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.CLOSE_RUN_QUEUE");

                strQry.AppendLine(" SET ");
                strQry.AppendLine(" CLOSE_RUN_QUEUE_IND = 'F'");
                strQry.AppendLine(",END_RUN_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND RUN_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("S"));
                strQry.AppendLine(" AND SALARY_PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(pardtRunDate.ToString("yyyy-MM-dd")));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                
                intReturnCode = 1;
            }

            return intReturnCode;
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

                    string subject = "Close Payroll/Time Attendance Run Error - " + DateTime.Now.ToString("dd MMMM yyyy");

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
