using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Net;

namespace InteractPayroll
{
    public class busOpenPayrollRun
    {
        clsDBConnectionObjects clsDBConnectionObjects;
        clsTax Tax;
        clsTaxTableRead clsTaxTableRead;

        string pvtstrClassName = "busOpenPayrollRun";

        string pvtstrLogFileName = "";

        string pvtstrSmtpEmailAddressDescription = "";
        string pvtstrSmtpEmailAddress = "";
        string pvtstrSmtpEmailAddressPassword = "";
        string pvtstrSmtpHostName = "";
        int pvtintSmtpHostPort = 0;

        public busOpenPayrollRun()
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

        public byte[] Get_Form_Records(Int64 parint64CompanyNo, string parstrFromProgram)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
          
            int intFindRow = -1;

            strQry.Clear();

            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" C.COMPANY_NO");
            strQry.AppendLine(",C.COMPANY_DESC");
            strQry.AppendLine(",CL.DATE_FORMAT");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.COMPANY_LINK CL");
            strQry.AppendLine(" ON C.COMPANY_NO = CL.COMPANY_NO ");

            strQry.AppendLine(" WHERE C.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" C.COMPANY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", parint64CompanyNo);
            
            //2017-08-10
            strQry.Clear();

            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PAY_PERIOD_DATE");
            strQry.AppendLine(",PAY_CATEGORY_NUMBERS");
            strQry.AppendLine(",ISNULL(OPEN_RUN_QUEUE_IND,'S') AS OPEN_RUN_QUEUE_IND ");
            
            strQry.AppendLine(" FROM InteractPayroll.dbo.OPEN_RUN_QUEUE");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE IN ('W','S')");
            }
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "OpenRunQueue", -1);
            
            DataView DataViewOpenRunQueue = new DataView(DataSet.Tables["OpenRunQueue"],
                                      "",
                                      "PAY_CATEGORY_TYPE",
                                       DataViewRowState.CurrentRows);
            
            strQry.Clear();

            strQry.AppendLine(" SELECT ");

            if (parstrFromProgram == "X")
            {
                intFindRow = DataViewOpenRunQueue.Find("T");

                strQry.AppendLine(" 'Time Attendance' AS PAYROLL_TYPE ");
            }
            else
            {
                intFindRow = DataViewOpenRunQueue.Find("W");

                strQry.AppendLine(" 'Wages' AS PAYROLL_TYPE ");
            }

            if (intFindRow > -1)
            {
                strQry.AppendLine(",'" + DataViewOpenRunQueue[intFindRow]["OPEN_RUN_QUEUE_IND"].ToString() + "' AS OPEN_RUN_QUEUE_IND ");

                strQry.AppendLine(",CONVERT(DateTime,'" + Convert.ToDateTime(DataViewOpenRunQueue[intFindRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "') AS PAY_PERIOD_DATE ");
            }
            else
            {
                strQry.AppendLine(",'' AS OPEN_RUN_QUEUE_IND ");

                strQry.AppendLine(",MAX(PCPC.PAY_PERIOD_DATE) AS PAY_PERIOD_DATE");
            }

          
            strQry.AppendLine(",MAX(PCPH.PAY_PERIOD_DATE) AS PREV_PAY_PERIOD_DATE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

            //Errol 2013-04-12
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON C.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = 'W'");
            }

            strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");
            
            if (intFindRow == -1)
            {
                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC ");
                strQry.AppendLine(" ON C.COMPANY_NO = PCPC.COMPANY_NO ");
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_NO > 0");

                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = PCPC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = PCPC.PAY_CATEGORY_TYPE");

                if (parstrFromProgram == "X")
                {
                    strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = 'T'");
                }
                else
                {
                    strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = 'W'");
                }

                strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P'");
            }
            
            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH ");
            strQry.AppendLine(" ON C.COMPANY_NO = PCPH.COMPANY_NO ");
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_NO > 0");

            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = PCPH.PAY_CATEGORY_TYPE");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = 'W'");
            }
           
            strQry.AppendLine(" AND PCPH.RUN_TYPE = 'P'");
           
            strQry.AppendLine(" WHERE C.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

            if (parstrFromProgram != "X")
            {
                intFindRow = DataViewOpenRunQueue.Find("S");
                
                strQry.AppendLine(" UNION ");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" 'Salaries' AS PAYROLL_TYPE ");

                if (intFindRow > -1)
                {
                    strQry.AppendLine(",'" + DataViewOpenRunQueue[intFindRow]["OPEN_RUN_QUEUE_IND"].ToString() + "' AS OPEN_RUN_QUEUE_IND ");

                    strQry.AppendLine(",CONVERT(DateTime,'" + Convert.ToDateTime(DataViewOpenRunQueue[intFindRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "') AS PAY_PERIOD_DATE ");
                }
                else
                {
                    strQry.AppendLine(",'' AS OPEN_RUN_QUEUE_IND ");

                    strQry.AppendLine(",MAX(PCPC.PAY_PERIOD_DATE) AS PAY_PERIOD_DATE");
                }
                

                strQry.AppendLine(",MAX(PCPH.PAY_PERIOD_DATE) AS PREV_PAY_PERIOD_DATE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

                //Errol 2013-04-12
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
                strQry.AppendLine(" ON C.COMPANY_NO = PC.COMPANY_NO ");
                strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");

                if (intFindRow == -1)
                {
                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC ");
                    strQry.AppendLine(" ON C.COMPANY_NO = PCPC.COMPANY_NO ");
                    strQry.AppendLine(" AND PCPC.PAY_CATEGORY_NO > 0");
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = PCPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = PCPC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P'");
                }

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH ");
                strQry.AppendLine(" ON C.COMPANY_NO = PCPH.COMPANY_NO ");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_NO > 0");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = PCPH.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND PCPH.RUN_TYPE = 'P'");

                strQry.AppendLine(" WHERE C.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" ORDER BY 1 DESC");
            }

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayrollType", parint64CompanyNo);

            string strPayCategoryType = "W";

            if (parstrFromProgram == "X")
            {
                strPayCategoryType = "T";
            }

            byte[] bytCompress = Get_Employee_WageRun(parint64CompanyNo, strPayCategoryType);

            DataSet TempDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(bytCompress);
            DataSet.Merge(TempDataSet);

            DataSet.AcceptChanges();
                    
            bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Check_Queue(Int64 parInt64CompanyNo, string parstrPayrollType)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
            
            DataSet.Tables.Add("Reply");
            DataSet.Tables["Reply"].Columns.Add("CHECK_QUEUE_IND", typeof(String));

            DataRow drDataRow = DataSet.Tables["Reply"].NewRow();

            drDataRow["CHECK_QUEUE_IND"] = "S";

            DataSet.Tables["Reply"].Rows.Add(drDataRow);

            strQry.Clear();

            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" OPEN_RUN_QUEUE_IND");
            
            strQry.AppendLine(" FROM InteractPayroll.dbo.OPEN_RUN_QUEUE");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "OpenRunQueue", parInt64CompanyNo);

            if (DataSet.Tables["OpenRunQueue"].Rows.Count > 0)
            {
                if (DataSet.Tables["OpenRunQueue"].Rows[0]["OPEN_RUN_QUEUE_IND"].ToString() != "")
                {
                    DataSet.Tables["Reply"].Rows[0]["CHECK_QUEUE_IND"] = DataSet.Tables["OpenRunQueue"].Rows[0]["OPEN_RUN_QUEUE_IND"].ToString();
                }
            }
            else
            {
                //Completed Successfully
                DataSet.Tables["Reply"].Rows[0]["CHECK_QUEUE_IND"] = "";
            }

            DataSet.Tables.Remove("OpenRunQueue");

            if (DataSet.Tables["Reply"].Rows[0]["CHECK_QUEUE_IND"].ToString() != "S")
            {
                byte[] bytTempCompress = Get_Employee_WageRun(parInt64CompanyNo, parstrPayrollType);

                DataSet TempDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(bytTempCompress);

                DataSet.Merge(TempDataSet);
            }

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public int Backup_DataBase(Int64 parInt64CompanyNo,string parstrPayrollType)
        {
            int intReturnCode = 9;

            try
            {
#if(DEBUG)
                //Don;t Backup Database
                intReturnCode = 0;
#else
                DataSet pvtDataSet = new DataSet();

                StringBuilder strQry = new StringBuilder();

                strQry.Clear();
                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" BACKUP_DATABASE_PATH");
                strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_PATH");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), pvtDataSet, "Directory", -1);

                string strFileDirectory = pvtDataSet.Tables["Directory"].Rows[0]["BACKUP_DATABASE_PATH"].ToString();

                string strDataBaseName = "InteractPayroll_" + parInt64CompanyNo.ToString("00000");

                string strBackupFileName = strDataBaseName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_BeforeOpenPayrollRun.bak";

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
                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" ISNULL(MAX(BACKUP_DATABASE_NO),0) + 1");
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strDataBaseName));
                strQry.AppendLine(",GETDATE()");
                strQry.AppendLine(",GETDATE()");
                strQry.AppendLine(",'" + strFileDirectory + "\\" + strBackupFileName + "'");
                strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_DATETIME");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                intReturnCode = 0;
#endif
            }
            catch (Exception ex)
            {
                string strStop = "";
            }

            return intReturnCode;
        }

        public byte[] Get_Employee_WageRun(Int64 parint64CompanyNo, string parstrPayrollType)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
            DataSet TempDataSet = new DataSet();

            //2017-08-10
            strQry.Clear();

            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PAY_PERIOD_DATE");
            strQry.AppendLine(",PAY_CATEGORY_NUMBERS");
            strQry.AppendLine(",ISNULL(OPEN_RUN_QUEUE_IND,'S') AS OPEN_RUN_QUEUE_IND ");

            strQry.AppendLine(" FROM InteractPayroll.dbo.OPEN_RUN_QUEUE");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), TempDataSet, "RunQueue", -1);

            DataView DataViewRunQueue = new DataView(TempDataSet.Tables["RunQueue"],
                                      "",
                                      "PAY_CATEGORY_TYPE",
                                       DataViewRowState.CurrentRows);

            int intFindRow = DataViewRunQueue.Find(parstrPayrollType);

            strQry.Clear();
            strQry.AppendLine(" SELECT  ");
            strQry.AppendLine(" EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
            //strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            //Errol 2013-04-12
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");
 
            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            //2017-01-24 (Employee Not Closed)
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

            //Employee has Not Yet been Taken on
            strQry.AppendLine(" AND (E.EMPLOYEE_TAKEON_IND <> 'Y'");
            strQry.AppendLine(" OR E.EMPLOYEE_TAKEON_IND IS NULL)");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
 
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeNotInRun", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.LAST_UPLOAD_DATETIME");

            //Errol 2013-06-15
            strQry.AppendLine(",PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY = ");

            strQry.AppendLine(" DATEADD(dd,1,MIN(CASE ");

            //Errol - 2015-02-13
            strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y' ");

            strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

            strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

            strQry.AppendLine(" END)) ");

            //2017-04-22
            strQry.AppendLine(",MAX(PCPH.PAY_PERIOD_DATE) AS MAX_PAY_PERIOD_DATE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON PC.COMPANY_NO = E.COMPANY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND NOT EMPLOYEE_LAST_RUNDATE IS NULL");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            //2017-04-22
            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");
            strQry.AppendLine(" ON PC.COMPANY_NO = PCPH.COMPANY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = PCPH.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND PCPH.RUN_TYPE = 'P'");
            
            strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0 ");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

            strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");

            strQry.AppendLine(" AND NOT PC.PAY_CATEGORY_NO IN ");

            if (intFindRow != -1)
            {
                strQry.AppendLine("(" + DataViewRunQueue[intFindRow]["PAY_CATEGORY_NUMBERS"].ToString() + ")");
            }
            else
            {
                strQry.AppendLine("(SELECT PCPC.PAY_CATEGORY_NO ");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");

                //Errol 2013-04-12
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
                strQry.AppendLine(" ON PCPC.COMPANY_NO = PC.COMPANY_NO ");
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" WHERE PCPC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P'");

                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType) + ")");
            }
            
            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.LAST_UPLOAD_DATETIME");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 1");
            strQry.AppendLine(",2");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategory", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            if (intFindRow > -1)
            {
                strQry.AppendLine(",CONVERT(DateTime,'" + Convert.ToDateTime(DataViewRunQueue[intFindRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "') AS PAY_PERIOD_DATE ");
            }
            else
            {
                strQry.AppendLine(",PCC.PAY_PERIOD_DATE");
            }

            strQry.AppendLine(",PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.LAST_UPLOAD_DATETIME");
            //2017-04-22
            strQry.AppendLine(",MAX(PCPH.PAY_PERIOD_DATE) AS MAX_PAY_PERIOD_DATE");

            if (intFindRow != -1)
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            }
            else
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCC");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
                strQry.AppendLine(" ON PC.COMPANY_NO = PCC.COMPANY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = PCC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = PCC.PAY_CATEGORY_TYPE");
            }
            
            //2017-04-22
            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");
            strQry.AppendLine(" ON PC.COMPANY_NO = PCPH.COMPANY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = PCPH.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND PCPH.RUN_TYPE = 'P'");
            
            strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0 ");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

            if (intFindRow > -1)
            {
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO IN (" + DataViewRunQueue[intFindRow]["PAY_CATEGORY_NUMBERS"].ToString() + ")");
            }
            else
            { 
                strQry.AppendLine(" AND PCC.RUN_TYPE = 'P'");
            }

            strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");
            
            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            if (intFindRow == -1)
            {
                strQry.AppendLine(",PCC.PAY_PERIOD_DATE");
            }

            strQry.AppendLine(",PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.LAST_UPLOAD_DATETIME");
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategoryChosen", parint64CompanyNo);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Insert_Open_Into_Queue(Int64 parint64CompanyNo, DateTime parCurrentDateTime, string parstrPayCategoryNumbers,string parstrPayCategoryNumbersNotUsed, Int64 UserNo,string parstrPayCategoryType)
        {
            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();

            byte[] bytCompress = null;
            
            DataSet parDataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",MAX(PCPH.PAY_PERIOD_DATE) AS PREV_PAY_PERIOD_DATE ");
            strQry.AppendLine(",PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY = ");

            strQry.AppendLine(" DATEADD(dd,1,MIN(CASE ");

            //Errol - 2015-02-13
            strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y' ");

            strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

            strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

            strQry.AppendLine(" END)) ");

            strQry.AppendLine(",'' AS PUBLIC_HOLIDAYS_ERROR ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON PC.COMPANY_NO = E.COMPANY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND NOT EMPLOYEE_LAST_RUNDATE IS NULL");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");
            strQry.AppendLine(" ON PC.COMPANY_NO = PCPH.COMPANY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = PCPH.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND PCPH.RUN_TYPE = 'P'");

            strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO IN (" + parstrPayCategoryNumbers + ")");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
            strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PC.PAY_CATEGORY_NO");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 1");
            strQry.AppendLine(",2");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), parDataSet, "Upload", parint64CompanyNo);

            //ELR Check Leave Authorisation and that Public Holidays in Run don't Exceed 5
            for (int intRow = 0; intRow < parDataSet.Tables["Upload"].Rows.Count; intRow++)
            {
                if (parstrPayCategoryType != "T")
                {
                    //No Leave for Time Attendance
                    if (DataSet.Tables["LeaveCheck"] != null)
                    {
                        DataSet.Tables.Remove("LeaveCheck");
                    }

                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EPCLPAC.COMPANY_NO ");
                    strQry.AppendLine(",EPCLPAC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(",EPCLPAC.PAY_CATEGORY_TYPE ");

                    strQry.AppendLine(",COUNT(DISTINCT EPCLPAC.EMPLOYEE_NO) AS AUTHORISE_TOTAL");
                    strQry.AppendLine(",COUNT(DISTINCT EPCLPAC1.EMPLOYEE_NO) AS AUTHORISE_CURRENT");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT EPCLPAC");

                    //2017-01-24 (Employee Not Closed)
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON EPCLPAC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND EPCLPAC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPCLPAC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT EPCLPAC1");

                    strQry.AppendLine(" ON EPCLPAC.COMPANY_NO = EPCLPAC1.COMPANY_NO  ");
                    strQry.AppendLine(" AND EPCLPAC.EMPLOYEE_NO = EPCLPAC1.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPCLPAC.PAY_CATEGORY_NO = EPCLPAC1.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND EPCLPAC.PAY_CATEGORY_TYPE = EPCLPAC1.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EPCLPAC.LEVEL_NO = EPCLPAC1.LEVEL_NO");
                    strQry.AppendLine(" AND EPCLPAC1.AUTHORISED_IND = 'Y'");

                    strQry.AppendLine(" WHERE EPCLPAC.COMPANY_NO = " + parint64CompanyNo);

                    strQry.AppendLine(" AND EPCLPAC.PAY_CATEGORY_NO = " + parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND EPCLPAC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" EPCLPAC.COMPANY_NO ");
                    strQry.AppendLine(",EPCLPAC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(",EPCLPAC.PAY_CATEGORY_TYPE ");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "LeaveCheck", parint64CompanyNo);

                    for (int intLeaveRow = 0; intLeaveRow < DataSet.Tables["LeaveCheck"].Rows.Count; intLeaveRow++)
                    {
                        if (Convert.ToInt32(DataSet.Tables["LeaveCheck"].Rows[intLeaveRow]["AUTHORISE_TOTAL"]) != Convert.ToInt32(DataSet.Tables["LeaveCheck"].Rows[intLeaveRow]["AUTHORISE_CURRENT"]))
                        {

                            strQry.Clear();
                            strQry.AppendLine(" SELECT  ");
                            strQry.AppendLine(" COMPANY_NO ");

                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY");

                            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

                            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), parDataSet, "LeaveAuthorisationError", parint64CompanyNo);

                            bytCompress = clsDBConnectionObjects.Compress_DataSet(parDataSet);
                            DataSet.Dispose();
                            DataSet = null;

                            return bytCompress;
                        }
                    }
                }
            }
                               
            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.OPEN_RUN_QUEUE ");
            strQry.AppendLine("(COMPANY_NO ");
            strQry.AppendLine(",PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",PAY_PERIOD_DATE ");
            strQry.AppendLine(",PAY_CATEGORY_NUMBERS ");
            strQry.AppendLine(",PAY_CATEGORY_NUMBERS_NOT_USED ");
            strQry.AppendLine(",USER_NO) ");
            strQry.AppendLine(" VALUES ");
            strQry.AppendLine("(" + parint64CompanyNo);
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
            strQry.AppendLine(",'" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryNumbers));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryNumbersNotUsed));
            strQry.AppendLine("," + UserNo + ")");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            bytCompress = Get_Employee_WageRun(parint64CompanyNo, parstrPayCategoryType);
        
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Insert_Wage_Run_Records(Int64 parint64CompanyNo, DateTime parCurrentDateTime, string strPayCategoryNoIN)
        {
            string strClassNameFunctionAndParameters = pvtstrClassName + " Insert_Wage_Run_Records CompanyNo=" + parint64CompanyNo + ",parCurrentDateTime=" + parCurrentDateTime.ToString("yyyy-MM-dd") + ",strPayCategoryNoIN=" + strPayCategoryNoIN;

            StringBuilder strQry = new StringBuilder();
            byte[] bytCompress = null;

            try
            {
                //2018-10-27 - Start
                strQry.Clear();

                strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ");
                strQry.AppendLine(" DISABLE TRIGGER tgr_EMPLOYEE_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table ");
 
                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT ");
                strQry.AppendLine(" DISABLE TRIGGER tgr_EMPLOYEE_BREAK_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
               
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT");

                strQry.AppendLine(" SET ");
                strQry.AppendLine(" INCLUDED_IN_RUN_IND = NULL");
                
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_NO  IN (" + strPayCategoryNoIN + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                //2018-10-27
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT");

                strQry.AppendLine(" SET ");
                strQry.AppendLine(" INCLUDED_IN_RUN_IND = NULL");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_NO  IN (" + strPayCategoryNoIN + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                             
                strQry.Clear();

                strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ");
                strQry.AppendLine(" ENABLE TRIGGER tgr_EMPLOYEE_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT ");
                strQry.AppendLine(" ENABLE TRIGGER tgr_EMPLOYEE_BREAK_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                //2018-10-27 - End
                
                //2017-08-115
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.OPEN_RUN_QUEUE");

                strQry.AppendLine(" SET ");
                //S=Started
                strQry.AppendLine(" OPEN_RUN_QUEUE_IND = 'S'");
                strQry.AppendLine(",START_RUN_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("W"));
                strQry.AppendLine(" AND PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parCurrentDateTime.ToString("yyyy-MM-dd")));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                DataSet DataSet = new DataSet();
                
                DataSet parDataSet = new DataSet();

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PC.PAY_CATEGORY_NO");
                strQry.AppendLine(",MAX(PCPH.PAY_PERIOD_DATE) AS PREV_PAY_PERIOD_DATE ");
                strQry.AppendLine(",PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY = ");

                strQry.AppendLine(" DATEADD(dd,1,MIN(CASE ");

                //Errol - 2015-02-13
                strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y' ");

                strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

                strQry.AppendLine(" END)) ");

                strQry.AppendLine(",'' AS PUBLIC_HOLIDAYS_ERROR ");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON PC.COMPANY_NO = E.COMPANY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND EMPLOYEE_ENDDATE IS NULL");
                strQry.AppendLine(" AND NOT EMPLOYEE_LAST_RUNDATE IS NULL");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");
                strQry.AppendLine(" ON PC.COMPANY_NO = PCPH.COMPANY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = PCPH.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND PCPH.RUN_TYPE = 'P'");

                strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO IN (" + strPayCategoryNoIN + ")");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("W"));
                strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");
                strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" PC.PAY_CATEGORY_NO");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" 1");
                strQry.AppendLine(",2");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), parDataSet, "Upload", parint64CompanyNo);

                int intRunNo = -1;

                string strQryTemp = "";

                StringBuilder strFieldNamesInitialised = new StringBuilder();

                DateTime dtBeginFinancialYear;
                DateTime dtEndFinancialYear;
                DateTime dtPreviousDateTime;
                int intPaidHoliday = 0;
                int intPaidHolidayNumber = 0;
                int intDayNo = 0;
                int intDay = 0;
                string strFieldId = "";

                if (parCurrentDateTime.Month > 2)
                {
                    dtBeginFinancialYear = new DateTime(parCurrentDateTime.Year, 3, 1);
                }
                else
                {
                    dtBeginFinancialYear = new DateTime(parCurrentDateTime.Year - 1, 3, 1);
                }

                //Last Day Of Fiscal Year
                dtEndFinancialYear = dtBeginFinancialYear.AddYears(1).AddDays(-1);

                for (int intRow = 0; intRow < parDataSet.Tables["Upload"].Rows.Count; intRow++)
                {
                    if (DataSet.Tables["RunNo"] != null)
                    {
                        DataSet.Tables.Remove("RunNo");
                    }

                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" ISNULL(RUN_NO,0) + 1 AS RUN_NO");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_PERIOD_DATE = '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));

                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND RUN_TYPE = 'P'");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "RunNo", parint64CompanyNo);

                    if (DataSet.Tables["RunNo"].Rows.Count == 0)
                    {
                        intRunNo = 1;
                    }
                    else
                    {
                        intRunNo = Convert.ToInt32(DataSet.Tables["RunNo"].Rows[0]["RUN_NO"]);
                    }

                    //Initial PAY_CATEGORY does NOT have Value - Use Min EMPLOYEE_LAST_RUNDATE
                    if (parDataSet.Tables["Upload"].Rows[intRow]["PREV_PAY_PERIOD_DATE"] == System.DBNull.Value)
                    {
                        //Already Added 1 to Last Rundate
                        parDataSet.Tables["Upload"].Rows[intRow]["PREV_PAY_PERIOD_DATE"] = Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY"]);
                        dtPreviousDateTime = Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_PAY_PERIOD_DATE"]);
                    }
                    else
                    {
                        dtPreviousDateTime = Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_PAY_PERIOD_DATE"]).AddDays(1);
                    }

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT_CURRENT");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",RUN_NO");

                    strFieldNamesInitialised.Clear();
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("LEAVE_SHIFT_CURRENT", ref strQry, ref strFieldNamesInitialised, "", parint64CompanyNo);

                    strQry.AppendLine(")");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" COMPANY_NO");
                    strQry.AppendLine("," + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(",'W'");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.COMPANY");
                    strQry.AppendLine(" SET ");

                    strQry.AppendLine(" WAGE_RUN_IND = 'N'");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");
                    strQry.AppendLine(",RUN_TYPE");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",RUN_NO");

                    strQry.AppendLine(",PAY_PERIOD_DATE_FROM");
                    strQry.AppendLine(",SALARY_TIMESHEET_ENDDATE");

                    strFieldNamesInitialised.Clear();

                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("PAY_CATEGORY_PERIOD_CURRENT", ref strQry, ref strFieldNamesInitialised, "", parint64CompanyNo);

                    strQry.AppendLine(")");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" COMPANY_NO");
                    strQry.AppendLine(",'" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",'P'");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.AppendLine(",'" + dtPreviousDateTime.ToString("yyyy-MM-dd") + "'");
                    //SALARY_TIMESHEET_ENDDATE
                    strQry.AppendLine(",NULL ");

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK_CURRENT");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",PAY_CATEGORY_BREAK_NO");
                    strQry.AppendLine(",RUN_NO");

                    strFieldNamesInitialised.Clear();
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("PAY_CATEGORY_BREAK_CURRENT", ref strQry, ref strFieldNamesInitialised, "", parint64CompanyNo);

                    strQry.AppendLine(")");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",PAY_CATEGORY_BREAK_NO");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",RUN_TYPE");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",RUN_NO");
                    strQry.AppendLine(",HOURLY_RATE");
                    strQry.AppendLine(",LEAVE_DAY_RATE_DECIMAL");
                    strQry.AppendLine(",OVERTIME_VALUE_BF");
                    strQry.AppendLine(",OVERTIME_VALUE_CF");
                    strQry.AppendLine(",DEFAULT_IND)");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EPC.COMPANY_NO");
                    strQry.AppendLine(",EPC.EMPLOYEE_NO");
                    strQry.AppendLine(",'P'");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.AppendLine(",EPC.HOURLY_RATE");
                    strQry.AppendLine(",EPC.LEAVE_DAY_RATE_DECIMAL");

                    //NB. Value From CF is Carried to BF
                    strQry.AppendLine(",ISNULL(EPCH.OVERTIME_VALUE_CF,0)");
                    strQry.AppendLine(",0");

                    strQry.AppendLine(",EPC.DEFAULT_IND");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON EPC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND EPC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_LAST_RUNDATE < '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");
                    strQry.AppendLine(" ON EPC.COMPANY_NO = EPCH.COMPANY_NO ");
                    strQry.AppendLine(" AND EPC.EMPLOYEE_NO = EPCH.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = EPCH.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = EPCH.PAY_CATEGORY_TYPE");
                    //If Previous Fiscal Year then No C/f of History Values (Will be Zero)
                    strQry.AppendLine(" AND EPCH.PAY_PERIOD_DATE >= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND EPCH.RUN_TYPE = 'P'");

                    strQry.AppendLine(" AND STR(EPCH.EMPLOYEE_NO) + CONVERT(CHAR,EPCH.PAY_PERIOD_DATE) IN ");
                    strQry.AppendLine("(SELECT STR(EMPLOYEE_NO) + CONVERT(CHAR,MAX(PAY_PERIOD_DATE)) ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY ");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND RUN_TYPE = 'P'");
                    strQry.AppendLine(" GROUP BY EMPLOYEE_NO)");

                    strQry.AppendLine(" WHERE EPC.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",LEVEL_NO");
                    strQry.AppendLine(",USER_NO");
                    strQry.AppendLine(",AUTHORISED_IND)");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EPC.COMPANY_NO");
                    strQry.AppendLine(",EPC.EMPLOYEE_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",PCA.LEVEL_NO");
                    strQry.AppendLine(",PCA.USER_NO");
                    strQry.AppendLine(",'N'");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON EPC.COMPANY_NO = E.COMPANY_NO");
                    strQry.AppendLine(" AND EPC.EMPLOYEE_NO = E.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                    //Employee Has Been Activates (Taken-On)
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                    //Employee NOT Closed
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_AUTHORISE PCA");
                    strQry.AppendLine(" ON EPC.COMPANY_NO = PCA.COMPANY_NO");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PCA.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PCA.PAY_CATEGORY_TYPE");

                    //T = Timesheet, L = Leave
                    strQry.AppendLine(" AND PCA.AUTHORISE_TYPE_IND = 'T'");
                    strQry.AppendLine(" AND PCA.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE EPC.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'W'");

                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_CURRENT");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",RUN_NO");

                    strFieldNamesInitialised.Clear();
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("PUBLIC_HOLIDAY_CURRENT", ref strQry, ref strFieldNamesInitialised, "PH", parint64CompanyNo);

                    strQry.AppendLine(")");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(parint64CompanyNo.ToString());
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY PH");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
                    strQry.AppendLine(" ON PC.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = " + parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND PC.PAY_PUBLIC_HOLIDAY_IND = 'Y' ");

                    //Errol Changed 2011-04-26
                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_CURRENT PHC ");
                    strQry.AppendLine(" ON PC.COMPANY_NO = PHC.COMPANY_NO ");
                    strQry.AppendLine(" AND PH.PUBLIC_HOLIDAY_DATE = PHC.PUBLIC_HOLIDAY_DATE ");

                    strQry.AppendLine(" WHERE PH.PUBLIC_HOLIDAY_DATE >= '" + Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND PH.PUBLIC_HOLIDAY_DATE <= '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                    //Errol Changed 2011-04-26
                    strQry.AppendLine(" AND PHC.PUBLIC_HOLIDAY_DATE IS NULL");

                    //PAID HOLIDAY that is Included in LEAVE outside the Current Wage Run Boundaries
                    strQry.AppendLine(" UNION ");

                    strQry.AppendLine(" SELECT DISTINCT ");
                    strQry.AppendLine(parint64CompanyNo.ToString());
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY PH");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
                    strQry.AppendLine(" ON PC.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = " + parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND PC.PAY_PUBLIC_HOLIDAY_IND = 'Y' ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT L");
                    strQry.AppendLine(" ON PC.COMPANY_NO = L.COMPANY_NO ");
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = L.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND PH.PUBLIC_HOLIDAY_DATE >= L.LEAVE_FROM_DATE");
                    strQry.AppendLine(" AND PH.PUBLIC_HOLIDAY_DATE <= L.LEAVE_TO_DATE");
                    strQry.AppendLine(" AND L.PROCESS_NO = 0");

                    //Errol Changed 2011-04-26
                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_CURRENT PHC ");
                    strQry.AppendLine(" ON PC.COMPANY_NO = PHC.COMPANY_NO ");
                    strQry.AppendLine(" AND PH.PUBLIC_HOLIDAY_DATE = PHC.PUBLIC_HOLIDAY_DATE ");

                    //Errol Changed 2011-04-26
                    strQry.AppendLine(" WHERE PHC.PUBLIC_HOLIDAY_DATE IS NULL");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    //First Set Day No
                    System.TimeSpan s = parCurrentDateTime.Subtract(dtPreviousDateTime);

                    if (s.Days <= 6)
                    {
                        intDayNo = Convert.ToInt32(parCurrentDateTime.DayOfWeek);
                    }

                    //Incomplete Week up to First Week
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_CURRENT");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",WEEK_DATE");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",RUN_NO");

                    strQry.AppendLine(",WEEK_DATE_FROM");
                    //New Fields Not Found in PAY_CATEGORY
                    strQry.AppendLine(",PAIDHOLIDAY_MINUTES1");
                    strQry.AppendLine(",PAIDHOLIDAY_DAY1");
                    strQry.AppendLine(",PAIDHOLIDAY_MINUTES2");
                    strQry.AppendLine(",PAIDHOLIDAY_DAY2");
                    strQry.AppendLine(",PAIDHOLIDAY_MINUTES3");
                    strQry.AppendLine(",PAIDHOLIDAY_DAY3");
                    strQry.AppendLine(",PAIDHOLIDAY_MINUTES4");
                    strQry.AppendLine(",PAIDHOLIDAY_DAY4");
                    strQry.AppendLine(",PAIDHOLIDAY_MINUTES5");
                    strQry.AppendLine(",PAIDHOLIDAY_DAY5");
                    strQry.AppendLine(",AMOUNT_HOURS");
                    strQry.AppendLine(",AMOUNT_VALUE");
                    strQry.AppendLine(",AMOUNT_VALUE_YTD");

                    strFieldNamesInitialised.Clear();
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("PAY_CATEGORY_WEEK_CURRENT", ref strQry, ref strFieldNamesInitialised, "PC", parint64CompanyNo);

                    strQry.AppendLine(")");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" PC.COMPANY_NO");
                    strQry.AppendLine(",D.DAY_DATE");
                    strQry.AppendLine(",PC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",'W'");

                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.AppendLine(",'" + Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");
                    strQry.AppendLine(" ON D.DAY_DATE >= '" + Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND D.DAY_DATE <=  '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND D.DAY_DATE = ");
                    strQry.AppendLine("(SELECT MIN(DAY_DATE)");

                    strQry.AppendLine(" FROM InteractPayroll.dbo.DATES ");

                    strQry.AppendLine(" WHERE DAY_NO = " + intDayNo);
                    strQry.AppendLine(" AND DAY_DATE >= '" + Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND DAY_DATE <=  '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "')");

                    strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" UNION ");

                    //Full Weeks
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" PC.COMPANY_NO");
                    strQry.AppendLine(",D.DAY_DATE");
                    strQry.AppendLine(",PC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",'W'");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.AppendLine(",DATEADD(d, -6, D.DAY_DATE)");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");
                    strQry.AppendLine(" ON D.DAY_NO = " + intDayNo);
                    strQry.AppendLine(" AND DATEADD(d, -7, D.DAY_DATE) >= '" + Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND D.DAY_DATE <= '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" UNION ");

                    //Incomplete Week AFTER All Full WeekS
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" PC.COMPANY_NO");
                    strQry.AppendLine(",'" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",PC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",'W'");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.AppendLine(",D.DAY_DATE");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");
                    strQry.AppendLine(" ON D.DAY_DATE >= '" + Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND D.DAY_DATE <=  '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND D.DAY_DATE = ");
                    strQry.AppendLine("(SELECT DATEADD(d,+1,MAX(DAY_DATE))");

                    strQry.AppendLine(" FROM InteractPayroll.dbo.DATES ");

                    strQry.AppendLine(" WHERE DAY_NO = " + intDayNo);
                    strQry.AppendLine(" AND DAY_DATE >= '" + Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND DAY_DATE <=  '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "')");

                    strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    //Insert First Incomplete Week		
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_WEEK_CURRENT");
                    strQry.AppendLine("(COMPANY_NO ");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",WEEK_DATE");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",RUN_NO");

                    strQry.AppendLine(",WEEK_DATE_FROM");

                    strFieldNamesInitialised.Clear();
                    clsDBConnectionObjects.Initialise_DataSet_Numeric_Fields("EMPLOYEE_EARNING_WEEK_CURRENT", ref strQry, ref strFieldNamesInitialised, parint64CompanyNo);

                    strQry.AppendLine(")");

                    strQry.AppendLine(" SELECT DISTINCT");
                    strQry.AppendLine(" EMPLOYEE_TABLE.COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_TABLE.EMPLOYEE_NO");
                    strQry.AppendLine(",D.DAY_DATE");
                    strQry.AppendLine(",EMPLOYEE_TABLE.PAY_CATEGORY_NO");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.AppendLine(",DATEADD(dd,1,EMPLOYEE_TABLE.EMPLOYEE_LAST_RUNDATE)");

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM ");

                    strQry.AppendLine("(SELECT ");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");

                    strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

                    strQry.AppendLine(" CASE ");

                    //Errol - 2015-02-13
                    strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y' ");

                    strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" ELSE  E.EMPLOYEE_LAST_RUNDATE ");

                    strQry.AppendLine(" END  ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL) AS EMPLOYEE_TABLE");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");
                    strQry.AppendLine(" ON D.DAY_DATE > EMPLOYEE_TABLE.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND D.DAY_DATE < DATEADD(dd,8,EMPLOYEE_TABLE.EMPLOYEE_LAST_RUNDATE)");
                    strQry.AppendLine(" AND D.DAY_DATE <= '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND D.DAY_NO = " + intDayNo);

                    strQry.AppendLine(" UNION ");

                    //Insert Complete Week		
                    strQry.AppendLine(" SELECT DISTINCT");
                    strQry.AppendLine(" EMPLOYEE_TABLE.COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_TABLE.EMPLOYEE_NO");
                    strQry.AppendLine(",D.DAY_DATE");
                    strQry.AppendLine(",EMPLOYEE_TABLE.PAY_CATEGORY_NO");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.AppendLine(",DATEADD(d, -6, D.DAY_DATE)");

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM ");

                    strQry.AppendLine("(SELECT ");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");

                    strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

                    strQry.AppendLine(" CASE ");

                    //Errol - 2015-02-13
                    strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y' ");

                    strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" ELSE  E.EMPLOYEE_LAST_RUNDATE ");

                    strQry.AppendLine(" END  ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL) AS EMPLOYEE_TABLE");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");
                    strQry.AppendLine(" ON EMPLOYEE_TABLE.EMPLOYEE_LAST_RUNDATE < DATEADD(d, -7, D.DAY_DATE)");
                    strQry.AppendLine(" AND D.DAY_NO = " + intDayNo);
                    strQry.AppendLine(" AND DATEADD(d, -7, D.DAY_DATE) >= '" + Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND D.DAY_DATE <= '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(" UNION ");

                    //Insert Last Incomplete Week
                    strQry.AppendLine(" SELECT DISTINCT");
                    strQry.AppendLine(" EMPLOYEE_TABLE.COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_TABLE.EMPLOYEE_NO");
                    strQry.AppendLine(",'" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",EMPLOYEE_TABLE.PAY_CATEGORY_NO");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.AppendLine(",D.DAY_DATE");

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM ");

                    strQry.AppendLine("(SELECT ");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");

                    strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

                    strQry.AppendLine(" CASE ");

                    //Errol - 2015-02-13
                    strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y' ");

                    strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" ELSE  E.EMPLOYEE_LAST_RUNDATE ");

                    strQry.AppendLine(" END  ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL) AS EMPLOYEE_TABLE");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");
                    strQry.AppendLine(" ON EMPLOYEE_TABLE.EMPLOYEE_LAST_RUNDATE < D.DAY_DATE");
                    strQry.AppendLine(" AND D.DAY_DATE >= '" + Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND D.DAY_DATE <=  '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND D.DAY_DATE = ");
                    strQry.AppendLine("(SELECT DATEADD(d,+1,MAX(DAY_DATE))");

                    strQry.AppendLine(" FROM InteractPayroll.dbo.DATES ");

                    strQry.AppendLine(" WHERE DAY_NO = " + intDayNo);
                    strQry.AppendLine(" AND DAY_DATE >= '" + Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND DAY_DATE <=  '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",RUN_TYPE");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",RUN_NO");
                    strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(",EXTRA_CHEQUES_HISTORY");
                    strQry.AppendLine(",EXTRA_CHEQUES_CURRENT");
                    strQry.AppendLine(",CLOSE_IND");
                    strQry.AppendLine(",PAYSLIP_IND");

                    //ELR-2014-05-01
                    strQry.AppendLine(",NUMBER_MEDICAL_AID_DEPENDENTS");
                    strQry.AppendLine(",OCCUPATION_NO");
                    strQry.AppendLine(",CURRENT_YEAR_LEAVE_SHIFTS_PER_RUN");
                    strQry.AppendLine(",PREV_YEAR_LEAVE_SHIFTS_PER_RUN");

                    //2012-11-23
                    strQry.AppendLine(",LEAVE_SHIFT_NO");

                    strQry.AppendLine(",SALARY_MONTH_PAYMENT)");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",'P'");
                    strQry.AppendLine(",'W'");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

                    strQry.AppendLine(" CASE ");

                    //Errol - 2015-02-13
                    strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y' ");

                    strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

                    strQry.AppendLine(" END  ");

                    strQry.AppendLine(",ISNULL(EIH1.EXTRA_CHEQUES_HISTORY + EIH1.EXTRA_CHEQUES_CURRENT,0)");

                    strQry.AppendLine(",0");

                    strQry.AppendLine(",'N'");
                    strQry.AppendLine(",'Y'");

                    //ELR-2014-05-01
                    strQry.AppendLine(",ISNULL(E.NUMBER_MEDICAL_AID_DEPENDENTS,0)");
                    strQry.AppendLine(",E.OCCUPATION_NO");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");

                    //2012-11-23
                    strQry.AppendLine(",E.LEAVE_SHIFT_NO");

                    strQry.AppendLine(",0");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    //Errol 2014-02-20
                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT EIC ");
                    strQry.AppendLine(" ON EPC.COMPANY_NO = EIC.COMPANY_NO ");
                    strQry.AppendLine(" AND EPC.EMPLOYEE_NO = EIC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EIC.RUN_TYPE = 'P' ");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = EIC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EIC.RUN_NO = " + intRunNo.ToString());
                    //Errol 2014-02-20

                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY EIH1 ");
                    strQry.AppendLine(" ON E.COMPANY_NO = EIH1.COMPANY_NO");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EIH1.EMPLOYEE_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EIH1.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EIH1.RUN_TYPE = 'P'");

                    strQry.AppendLine(" AND STR(EIH1.EMPLOYEE_NO) + CONVERT(CHAR,EIH1.PAY_PERIOD_DATE) IN ");
                    strQry.AppendLine("(SELECT STR(EMPLOYEE_NO) + CONVERT(CHAR,MAX(PAY_PERIOD_DATE)) ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY ");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND RUN_TYPE = 'P'");
                    //If Previous Fiscal Year then No C/f of History Values (Will be Zero)
                    strQry.AppendLine(" AND PAY_PERIOD_DATE >= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" EMPLOYEE_NO) ");

                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY EIH ");
                    strQry.AppendLine(" ON E.COMPANY_NO = EIH.COMPANY_NO");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EIH.EMPLOYEE_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EIH.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EIH.RUN_TYPE = 'P'");

                    strQry.AppendLine(" AND STR(EIH.EMPLOYEE_NO) + CONVERT(CHAR,EIH.PAY_PERIOD_DATE) IN ");
                    strQry.AppendLine("(SELECT STR(EMPLOYEE_NO) + CONVERT(CHAR,MAX(PAY_PERIOD_DATE)) ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY ");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND RUN_TYPE = 'P'");
                    //If Previous Fiscal Year then No C/f of History Values (Will be Zero)
                    strQry.AppendLine(" AND PAY_PERIOD_DATE >= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");

                    //Medical Aid Deduction / Earning
                    strQry.AppendLine("(SELECT ");
                    strQry.AppendLine(" EE.EMPLOYEE_NO");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EE ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EN ");
                    strQry.AppendLine(" ON EE.COMPANY_NO = EN.COMPANY_NO ");
                    strQry.AppendLine(" AND EE.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EE.EARNING_NO = EN.EARNING_NO");
                    strQry.AppendLine(" AND EN.IRP5_CODE = 3810");
                    strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE EE.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND EE.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND EE.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" UNION ");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" ED.EMPLOYEE_NO");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ED ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.DEDUCTION D ");
                    strQry.AppendLine(" ON ED.COMPANY_NO = D.COMPANY_NO");
                    strQry.AppendLine(" AND ED.PAY_CATEGORY_TYPE = D.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND ED.DEDUCTION_NO = D.DEDUCTION_NO");
                    strQry.AppendLine(" AND D.IRP5_CODE = 4005");
                    strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE ED.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND ED.DATETIME_DELETE_RECORD IS NULL)");

                    strQry.AppendLine(" GROUP BY EMPLOYEE_NO)");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND E.EMPLOYEE_LAST_RUNDATE < '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                    //Errol 2014-02-20 - NO EMPLOYEE_INFO_CURRENT Exists
                    strQry.AppendLine(" AND EIC.COMPANY_NO IS NULL ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    //Insert Record for All Linked Earnings 
                    //(NOT NT/Income/Overtime/Bonus/Holidays/Leave/Tax Directive)		
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",RUN_TYPE");
                    strQry.AppendLine(",EARNING_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",RUN_NO");
                    strQry.AppendLine(",TOTAL");
                    //Used Salaries for Normal Leave when Employee is Closed 
                    strQry.AppendLine(",HOURS_DECIMAL_OTHER_VALUE");
                    //ELR 2014-05-24
                    strQry.AppendLine(",EARNING_TYPE_IND");

                    strFieldNamesInitialised.Clear();
                    clsDBConnectionObjects.Initialise_DataSet_Numeric_Fields("EMPLOYEE_EARNING_CURRENT", ref strQry, ref strFieldNamesInitialised, parint64CompanyNo);

                    strQry.AppendLine(")");

                    strQry.AppendLine(" SELECT DISTINCT");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",'P'");
                    strQry.AppendLine(",EN.EARNING_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.AppendLine(",TOTAL = ");
                    strQry.AppendLine(" CASE");

                    //Multiple
                    strQry.AppendLine(" WHEN EE.EARNING_TYPE_IND = 'X' ");
                    strQry.AppendLine(" THEN 0 ");

                    //Errol - 2015-02-18
                    strQry.AppendLine(" WHEN NOT EEBT.AMOUNT IS NULL ");
                    strQry.AppendLine(" THEN EEBT.AMOUNT ");

                    //Each
                    strQry.AppendLine(" WHEN EE.EARNING_PERIOD_IND = 'E' ");
                    strQry.AppendLine(" THEN ISNULL(EE.AMOUNT,0)");

                    //Monthly
                    strQry.AppendLine(" WHEN EE.EARNING_PERIOD_IND = 'M' ");
                    //Errol - 2015-02-20
                    strQry.AppendLine(" AND ISNULL(E.FIRST_RUN_COMPLETED_IND,'') = 'Y'  ");
                    //EMPLOYEE_LAST_RUNDATE YYYY-MM-   DD=EARNING_DAY_VALUE
                    strQry.AppendLine(" AND ((CONVERT(DATETIME,CONVERT(CHAR(8), E.EMPLOYEE_LAST_RUNDATE,120) + RIGHT(100 + EE.EARNING_DAY_VALUE, 2)) > E.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND CONVERT(DATETIME,CONVERT(CHAR(8), E.EMPLOYEE_LAST_RUNDATE,120) + RIGHT(100 + EE.EARNING_DAY_VALUE, 2))  <= CONVERT(DateTime,'" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'))");
                    //EMPLOYEE_LAST_RUNDATE(ADD 1 Month) YYYY-MM-   DD=EARNING_DAY_VALUE
                    strQry.AppendLine(" OR (DATEADD(M,1,CONVERT(DATETIME,CONVERT(CHAR(8), E.EMPLOYEE_LAST_RUNDATE,120) + RIGHT(100 + EE.EARNING_DAY_VALUE, 2))) > E.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND DATEADD(M,1,CONVERT(DATETIME,CONVERT(CHAR(8), E.EMPLOYEE_LAST_RUNDATE,120) + RIGHT(100 + EE.EARNING_DAY_VALUE, 2)))  <= CONVERT(DateTime,'" + parCurrentDateTime.ToString("yyyy-MM-dd") + "')))");

                    strQry.AppendLine(" THEN ISNULL(EE.AMOUNT,0)");

                    //Errol - 2015-02-20 NB FIRST_RUN_COMPLETED_IND <> 'Y' THEREFORE >= E.EMPLOYEE_LAST_RUNDATE
                    //Monthly
                    strQry.AppendLine(" WHEN EE.EARNING_PERIOD_IND = 'M'  ");
                    //EMPLOYEE_LAST_RUNDATE YYYY-MM-   DD=EARNING_DAY_VALUE
                    strQry.AppendLine(" AND ((CONVERT(DATETIME,CONVERT(CHAR(8), E.EMPLOYEE_LAST_RUNDATE,120) + RIGHT(100 + EE.EARNING_DAY_VALUE, 2)) >= E.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND CONVERT(DATETIME,CONVERT(CHAR(8), E.EMPLOYEE_LAST_RUNDATE,120) + RIGHT(100 + EE.EARNING_DAY_VALUE, 2))  <= CONVERT(DateTime,'" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'))");
                    //EMPLOYEE_LAST_RUNDATE(ADD 1 Month) YYYY-MM-   DD=EARNING_DAY_VALUE
                    strQry.AppendLine(" OR (DATEADD(M,1,CONVERT(DATETIME,CONVERT(CHAR(8), E.EMPLOYEE_LAST_RUNDATE,120) + RIGHT(100 + EE.EARNING_DAY_VALUE, 2))) >= E.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND DATEADD(M,1,CONVERT(DATETIME,CONVERT(CHAR(8), E.EMPLOYEE_LAST_RUNDATE,120) + RIGHT(100 + EE.EARNING_DAY_VALUE, 2)))  <= CONVERT(DateTime,'" + parCurrentDateTime.ToString("yyyy-MM-dd") + "')))");

                    strQry.AppendLine(" THEN ISNULL(EE.AMOUNT,0)");
                    //Errol - 2015-02-20 - Up to Here

                    strQry.AppendLine(" ELSE ");
                    strQry.AppendLine(" 0 ");

                    strQry.AppendLine(" END ");

                    //2012-08-15 Caters for Multiple (EARNING_TYPE_IND = X)
                    strQry.AppendLine(",HOURS_DECIMAL_OTHER_VALUE = ");

                    strQry.AppendLine(" CASE");

                    strQry.AppendLine(" WHEN EE.EARNING_TYPE_IND = 'X' ");

                    strQry.AppendLine(" THEN EE.AMOUNT ");

                    strQry.AppendLine(" ELSE 0");

                    strQry.AppendLine(" END ");

                    //ELR 2014-05-24
                    strQry.AppendLine(",EE.EARNING_TYPE_IND ");

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    //Link To Default Pay Category
                    strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EE");
                    strQry.AppendLine(" ON E.COMPANY_NO = EE.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EE.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EE.EMPLOYEE_NO");
                    //ELR - 2014-08-30 (Changed to Fixed Only)

                    strQry.AppendLine(" AND EE.DATETIME_DELETE_RECORD IS NULL ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EN");
                    strQry.AppendLine(" ON EE.COMPANY_NO = EN.COMPANY_NO");
                    strQry.AppendLine(" AND EE.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EE.EARNING_NO = EN.EARNING_NO");
                    strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL ");

                    //Errol - 2015-02-18
                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_BATCH_TEMP EEBT");
                    strQry.AppendLine(" ON E.COMPANY_NO = EEBT.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EEBT.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EN.EARNING_NO = EEBT.EARNING_NO");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EEBT.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EEBT.PROCESS_NO = 0");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_LAST_RUNDATE < '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                    //Create Values For NT (Wages)/Overtime/Bonus/Holidays/Normal Leave
                    strQry.AppendLine(" UNION");

                    strQry.AppendLine(" SELECT DISTINCT");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",'P'");
                    strQry.AppendLine(",EN.EARNING_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine("," + intRunNo.ToString());

                    //2013-09-26
                    strQry.AppendLine(",ISNULL(EEBT.AMOUNT,0) ");

                    strQry.AppendLine(",0");

                    //ELR 2014-05-24
                    strQry.AppendLine(",EE.EARNING_TYPE_IND");

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    //Link To All Pay Category
                    //strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    //ELR - 2014-05-24
                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EE");
                    strQry.AppendLine(" ON E.COMPANY_NO = EE.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EE.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EE.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EE.DATETIME_DELETE_RECORD IS NULL ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EN");
                    strQry.AppendLine(" ON E.COMPANY_NO = EN.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
                    strQry.AppendLine(" ON E.COMPANY_NO = PC.COMPANY_NO ");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C");
                    strQry.AppendLine(" ON E.COMPANY_NO = C.COMPANY_NO ");
                    strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL ");

                    //2013-09-23
                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_BATCH_TEMP EEBT");
                    strQry.AppendLine(" ON E.COMPANY_NO = EEBT.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EEBT.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EN.EARNING_NO = EEBT.EARNING_NO");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EEBT.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EEBT.PROCESS_NO = 0");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_LAST_RUNDATE < '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                    //2=Normal Time
                    //9=Public Holiday (Worked)
                    strQry.AppendLine(" AND (EN.EARNING_NO IN (2,9)");

                    //7=Bonus
                    //200=Normal Leave
                    //201=Sick Leave 
                    strQry.AppendLine(" OR (EN.EARNING_NO IN(7,200,201)");
                    strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y')");

                    //ERROL Changed 2011/06/29 From Above Statement
                    //ERROL Changed 2011/06/29 From Above Statement

                    //Public Holiday - Company Paid 
                    strQry.AppendLine(" OR (EN.EARNING_NO = 8");
                    strQry.AppendLine(" AND PC.PAY_PUBLIC_HOLIDAY_IND = 'Y')");

                    strQry.AppendLine(" OR (EN.EARNING_NO = 3");
                    strQry.AppendLine(" AND C.OVERTIME1_RATE <> 0)");

                    strQry.AppendLine(" OR (EN.EARNING_NO = 4");
                    strQry.AppendLine(" AND C.OVERTIME2_RATE <> 0)");

                    strQry.AppendLine(" OR (EN.EARNING_NO = 5");
                    strQry.AppendLine(" AND C.OVERTIME3_RATE <> 0))");


                    //2017-02-17
                    //strQry.AppendLine(" UNION");

                    //strQry.AppendLine(" SELECT ");
                    //strQry.AppendLine(" E.COMPANY_NO");
                    //strQry.AppendLine(",E.EMPLOYEE_NO");
                    //strQry.AppendLine(",'P'");
                    //strQry.AppendLine(",EEH.EARNING_NO");
                    //strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    //strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
                    //strQry.AppendLine("," + intRunNo.ToString());

                    ////2013-09-26
                    //strQry.AppendLine(",SUM(EEH.TOTAL) ");

                    //strQry.AppendLine(",0");

                    ////ELR 2014-05-24
                    //strQry.AppendLine(",'F' AS EARNING_TYPE_IND");

                    //strQry.Append(strFieldNamesInitialised);

                    //strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    //strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    //strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    //strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    //strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
                    //strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    ////Get Income only for Default PAY_CATEGORY
                    //strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
                    //strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    ////ELR - 2014-05-24
                    //strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
                    //strQry.AppendLine(" ON E.COMPANY_NO = EEH.COMPANY_NO ");
                    ////Income which is for Salaries
                    //strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = 'S'");
                    //strQry.AppendLine(" AND E.EMPLOYEE_NO = EEH.EMPLOYEE_NO");
                    //strQry.AppendLine(" AND EEH.PAY_PERIOD_DATE > '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'");
                    ////1=Income
                    //strQry.AppendLine(" AND EEH.EARNING_NO = 1");

                    //strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    //strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                    //strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    //strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'W'");

                    //strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    //strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    //strQry.AppendLine(" AND E.EMPLOYEE_LAST_RUNDATE < '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                    //strQry.AppendLine(" GROUP BY ");
                    //strQry.AppendLine(" E.COMPANY_NO");
                    //strQry.AppendLine(",E.EMPLOYEE_NO");
                    //strQry.AppendLine(",EEH.EARNING_NO");
                    //strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    //strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
                    //2017-02-17

                    //Insert Where New/History Leave Record - Exclude Normal Leave / Sick Leave
                    strQry.AppendLine(" UNION");

                    strQry.AppendLine(" SELECT DISTINCT");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",'P'");
                    strQry.AppendLine(",EN.EARNING_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.AppendLine(",0");
                    //strQry.AppendLine(",0");
                    strQry.AppendLine(",0");

                    //ELR 2014-05-24
                    strQry.AppendLine(",EE.EARNING_TYPE_IND");

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
                    //Default Linked To Leave
                    strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    //ELR - 2014-05-24
                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EE");
                    strQry.AppendLine(" ON E.COMPANY_NO = EE.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EE.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EE.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EE.DATETIME_DELETE_RECORD IS NULL ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EN");
                    strQry.AppendLine(" ON E.COMPANY_NO = EN.COMPANY_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE");
                    //NB Normal / Sick Leave is Carried Forward from Take-On for Life of Employee
                    strQry.AppendLine(" AND EN.EARNING_NO > 201");
                    strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL ");
                    strQry.AppendLine(" AND EN.LEAVE_PERCENTAGE > 0");

                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT L");
                    strQry.AppendLine(" ON E.COMPANY_NO = L.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = L.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EN.EARNING_NO = L.EARNING_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = L.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND L.PROCESS_NO = 0 ");

                    //HISTORY EXISTS FOR Leave
                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
                    strQry.AppendLine(" ON E.COMPANY_NO = EEH.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EEH.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EN.EARNING_NO = EEH.EARNING_NO ");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = EEH.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EEH.PAY_PERIOD_DATE >= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_LAST_RUNDATE < '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                    //Current Leave Record or History Record Exists
                    strQry.AppendLine(" AND (NOT L.COMPANY_NO IS NULL ");
                    strQry.AppendLine(" OR NOT EEH.COMPANY_NO IS NULL) ");

                    //Insert NON Linked EMPLOYEE_EARNING For Current Year. NB Currently Includes Tax Directives
                    strQry.AppendLine(" UNION ");
                    strQry.AppendLine(" SELECT DISTINCT ");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",'P'");
                    strQry.AppendLine(",EN.EARNING_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.AppendLine(",0");
                    //strQry.AppendLine(",0");
                    strQry.AppendLine(",0");

                    //ELR 2014-05-24
                    strQry.AppendLine(",EE.EARNING_TYPE_IND");

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN  InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EN");
                    strQry.AppendLine(" ON E.COMPANY_NO = EN.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
                    strQry.AppendLine(" ON E.COMPANY_NO = EEH.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EEH.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EN.EARNING_NO = EEH.EARNING_NO ");
                    //Exclude Default Earnings and Leave
                    strQry.AppendLine(" AND EN.EARNING_NO > 9 ");
                    strQry.AppendLine(" AND EN.EARNING_NO < 200 ");
                    //Could also be a Take-On
                    //strQry.AppendLine(" AND EEH.RUN_TYPE = 'P'");
                    //Could be From Another Pay Category
                    //strQry.AppendLine(" AND EEH.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE");
                    //If Previous Fiscal Year then No C/f of History Values (Will be Zero)
                    strQry.AppendLine(" AND EEH.PAY_PERIOD_DATE >= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'");

                    //NO EMPLOYEE_EARNING Link
                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EE ");
                    strQry.AppendLine(" ON E.COMPANY_NO = EE.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EE.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EE.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EN.EARNING_NO = EE.EARNING_NO ");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_LAST_RUNDATE < '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");
                    //NO EMPLOYEE_EARNING Link
                    strQry.AppendLine(" AND EE.EARNING_NO IS NULL ");

                Run_Continue:

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    //Carry Forward Values Forward from History For Current Year Otherwise Zero - Linked
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",RUN_TYPE");
                    strQry.AppendLine(",DEDUCTION_NO");
                    strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
                    strQry.AppendLine(",RUN_NO");
                    strQry.AppendLine(",TOTAL");
                    strQry.AppendLine(",TOTAL_ORIGINAL)");

                    strQry.AppendLine(" SELECT DISTINCT");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",'P'");
                    strQry.AppendLine(",ED.DEDUCTION_NO");
                    strQry.AppendLine(",ED.DEDUCTION_SUB_ACCOUNT_NO");
                    strQry.AppendLine("," + intRunNo.ToString());

                    //2013-09-26
                    strQry.AppendLine(",ISNULL(EDBT.AMOUNT,0)");
                    strQry.AppendLine(",ISNULL(EDBT.AMOUNT,0)");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
                    //Link To Default Pay Category
                    strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ED");
                    strQry.AppendLine(" ON E.COMPANY_NO = ED.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = ED.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = ED.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND ED.DATETIME_DELETE_RECORD IS NULL");

                    //2013-09-26
                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_BATCH_TEMP EDBT");
                    strQry.AppendLine(" ON ED.COMPANY_NO = EDBT.COMPANY_NO ");
                    strQry.AppendLine(" AND ED.PAY_CATEGORY_TYPE = EDBT.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND ED.DEDUCTION_NO = EDBT.DEDUCTION_NO ");
                    strQry.AppendLine(" AND ED.DEDUCTION_SUB_ACCOUNT_NO = EDBT.DEDUCTION_SUB_ACCOUNT_NO ");
                    strQry.AppendLine(" AND ED.EMPLOYEE_NO = EDBT.EMPLOYEE_NO ");
                    //Next Run
                    strQry.AppendLine(" AND EDBT.PROCESS_NO = 0 ");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_LAST_RUNDATE < '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                    //Carry Values Forward when Deduction is Delinked
                    strQry.AppendLine(" UNION ");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EDH.COMPANY_NO");
                    strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EDH.EMPLOYEE_NO");
                    strQry.AppendLine(",'P'");
                    strQry.AppendLine(",EDH.DEDUCTION_NO");
                    strQry.AppendLine(",EDH.DEDUCTION_SUB_ACCOUNT_NO");
                    strQry.AppendLine("," + intRunNo.ToString());
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY EDH");
                    strQry.AppendLine(" ON E.COMPANY_NO = EDH.COMPANY_NO");
                    strQry.AppendLine(" AND EDH.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND EDH.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");

                    strQry.AppendLine(" AND EDH.EMPLOYEE_NO = E.EMPLOYEE_NO");
                    //If Previous Fiscal Year then No C/f of History Values (Will be Zero)
                    strQry.AppendLine(" AND EDH.PAY_PERIOD_DATE >= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'");
                    //Removed To Cater for Take-On
                    //strQry.AppendLine(" AND EDH.RUN_TYPE = 'P'");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_LAST_RUNDATE < '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND STR(E.EMPLOYEE_NO) + STR(EDH.DEDUCTION_NO) + STR(EDH.DEDUCTION_SUB_ACCOUNT_NO) + CONVERT(CHAR,EDH.PAY_PERIOD_DATE) IN ");
                    strQry.AppendLine("(SELECT STR(EMPLOYEE_NO) + STR(DEDUCTION_NO) + STR(DEDUCTION_SUB_ACCOUNT_NO) + CONVERT(CHAR,MAX(PAY_PERIOD_DATE)) ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY ");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                    //Removed To Cater for Take-On
                    //strQry.AppendLine(" AND RUN_TYPE = 'P'");
                    //If Previous Fiscal Year then No C/f of History Values (Will be Zero)
                    strQry.AppendLine(" AND PAY_PERIOD_DATE >= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'");
                    //Added
                    strQry.AppendLine(" AND TOTAL <> 0 ");

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" EMPLOYEE_NO ");
                    strQry.AppendLine(",DEDUCTION_NO ");
                    strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO)");

                    //Does NOT have a Current Link
                    strQry.AppendLine(" AND STR(E.EMPLOYEE_NO) + STR(EDH.DEDUCTION_NO) + STR(EDH.DEDUCTION_SUB_ACCOUNT_NO) NOT IN ");
                    strQry.AppendLine("(SELECT STR(EMPLOYEE_NO) + STR(DEDUCTION_NO) + STR(DEDUCTION_SUB_ACCOUNT_NO) ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL)");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    //Carry Forward Values Forward from History For Current Year Otherwise Zero - Linked
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE_CURRENT");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",DEDUCTION_NO");
                    strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
                    strQry.AppendLine(",EARNING_NO");
                    strQry.AppendLine(",RUN_NO)");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",EDEP.DEDUCTION_NO");
                    strQry.AppendLine(",EDEP.DEDUCTION_SUB_ACCOUNT_NO");
                    strQry.AppendLine(",EDEP.EARNING_NO");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    //2014-03-24
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
                    //Link To Default Pay Category
                    strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE EDEP");
                    strQry.AppendLine(" ON E.COMPANY_NO = EDEP.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EDEP.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EDEP.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EDEP.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_LAST_RUNDATE < '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                }

                for (int intRow = 0; intRow < parDataSet.Tables["Upload"].Rows.Count; intRow++)
                {
                    if (DataSet.Tables["Temp"] != null)
                    {
                        DataSet.Tables.Remove("Temp");
                    }

                    //Set Paid Holiday
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" PCWC.WEEK_DATE ");
                    strQry.AppendLine(",PCWC.MON_TIME_MINUTES ");
                    strQry.AppendLine(",PCWC.TUE_TIME_MINUTES ");
                    strQry.AppendLine(",PCWC.WED_TIME_MINUTES ");
                    strQry.AppendLine(",PCWC.THU_TIME_MINUTES ");
                    strQry.AppendLine(",PCWC.FRI_TIME_MINUTES ");
                    strQry.AppendLine(",PCWC.SAT_TIME_MINUTES ");
                    strQry.AppendLine(",PCWC.SUN_TIME_MINUTES ");
                    strQry.AppendLine(",PHD.PUBLIC_HOLIDAY_DATE ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_CURRENT PCWC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_CURRENT PHD");
                    strQry.AppendLine(" ON PCWC.COMPANY_NO = PHD.COMPANY_NO ");
                    strQry.AppendLine(" AND PHD.PUBLIC_HOLIDAY_DATE >= PCWC.WEEK_DATE_FROM ");
                    strQry.AppendLine(" AND PHD.PUBLIC_HOLIDAY_DATE <= PCWC.WEEK_DATE ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");
                    strQry.AppendLine(" ON PCWC.COMPANY_NO = PCPC.COMPANY_NO ");
                    strQry.AppendLine(" AND PCWC.PAY_CATEGORY_NO = PCPC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND PCWC.PAY_CATEGORY_TYPE = PCPC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P' ");
                    strQry.AppendLine(" AND PCPC.PAY_PUBLIC_HOLIDAY_IND = 'Y' ");

                    strQry.AppendLine(" WHERE PCWC.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PCWC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND PCWC.PAY_CATEGORY_TYPE = 'W'");

                    strQry.AppendLine(" ORDER BY ");
                    strQry.AppendLine(" PCWC.WEEK_DATE ");

                    //Check if Company Pays Public Holidays
                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parint64CompanyNo);

                    for (int intRow2 = 0; intRow2 < DataSet.Tables["Temp"].Rows.Count; intRow2++)
                    {
                        intDay = Convert.ToInt32(Convert.ToDateTime(DataSet.Tables["Temp"].Rows[intRow2]["PUBLIC_HOLIDAY_DATE"]).DayOfWeek);

                        switch (intDay)
                        {
                            case 0:

                                strQryTemp = ",SUN_TIME_MINUTES = 0";
                                strFieldId = "SUN_TIME_MINUTES";
                                break;

                            case 1:

                                strQryTemp = ",MON_TIME_MINUTES = 0";
                                strFieldId = "MON_TIME_MINUTES";
                                break;

                            case 2:

                                strQryTemp = ",TUE_TIME_MINUTES = 0";
                                strFieldId = "TUE_TIME_MINUTES";
                                break;

                            case 3:

                                strQryTemp = ",WED_TIME_MINUTES = 0";
                                strFieldId = "WED_TIME_MINUTES";
                                break;

                            case 4:

                                strQryTemp = ",THU_TIME_MINUTES = 0";
                                strFieldId = "THU_TIME_MINUTES";
                                break;

                            case 5:

                                strQryTemp = ",FRI_TIME_MINUTES = 0";
                                strFieldId = "FRI_TIME_MINUTES";
                                break;

                            case 6:

                                strQryTemp = ",SAT_TIME_MINUTES = 0";
                                strFieldId = "SAT_TIME_MINUTES";
                                break;
                        }
                        
                        intPaidHoliday = Convert.ToDateTime(DataSet.Tables["Temp"].Rows[intRow2]["PUBLIC_HOLIDAY_DATE"]).Day;
                        
                        if (DataSet.Tables["PublicHolidaySlotFind"] != null)
                        {
                            DataSet.Tables.Remove("PublicHolidaySlotFind");
                        }

                        strQry.Clear();

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" PAIDHOLIDAY_DAY1 ");
                        strQry.AppendLine(",PAIDHOLIDAY_DAY2 ");
                        strQry.AppendLine(",PAIDHOLIDAY_DAY3 ");
                        strQry.AppendLine(",PAIDHOLIDAY_DAY4 ");
                        strQry.AppendLine(",PAIDHOLIDAY_DAY5 ");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_CURRENT ");

                        //Run Update
                        strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                        strQry.AppendLine(" AND WEEK_DATE = '" + Convert.ToDateTime(DataSet.Tables["Temp"].Rows[intRow2]["WEEK_DATE"]).ToString("yyyy-MM-dd") + "'");

                        clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PublicHolidaySlotFind", parint64CompanyNo);

                        if (Convert.ToUInt32(DataSet.Tables["PublicHolidaySlotFind"].Rows[0]["PAIDHOLIDAY_DAY1"]) == 0)
                        {
                            intPaidHolidayNumber = 1;
                        }
                        else
                        {
                            if (Convert.ToUInt32(DataSet.Tables["PublicHolidaySlotFind"].Rows[0]["PAIDHOLIDAY_DAY2"]) == 0)
                            {
                                intPaidHolidayNumber = 2;
                            }
                            else
                            {
                                if (Convert.ToUInt32(DataSet.Tables["PublicHolidaySlotFind"].Rows[0]["PAIDHOLIDAY_DAY3"]) == 0)
                                {
                                    intPaidHolidayNumber = 3;
                                }
                                else
                                {
                                    if (Convert.ToUInt32(DataSet.Tables["PublicHolidaySlotFind"].Rows[0]["PAIDHOLIDAY_DAY4"]) == 0)
                                    {
                                        intPaidHolidayNumber = 4;
                                    }
                                    else
                                    {
                                        intPaidHolidayNumber = 5;
                                    }
                                }
                            }
                        }

                        strQry.Clear();

                        strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_CURRENT ");
                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" PAIDHOLIDAY_DAY" + intPaidHolidayNumber.ToString() + " = " + intPaidHoliday);
                        strQry.AppendLine(",PAIDHOLIDAY_MINUTES" + intPaidHolidayNumber.ToString() + " = " + Convert.ToInt32(DataSet.Tables["Temp"].Rows[intRow2][strFieldId]));

                        //Set Relevant Day Paid Hours to Zero
                        strQry.AppendLine(strQryTemp);

                        strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                        strQry.AppendLine(" AND WEEK_DATE = '" + Convert.ToDateTime(DataSet.Tables["Temp"].Rows[intRow2]["WEEK_DATE"]).ToString("yyyy-MM-dd") + "'");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                    }

                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_WEEK_CURRENT");
                    strQry.AppendLine(" SET PAIDHOLIDAY_INDICATOR = 'Y' ");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND CONVERT(CHAR(5),EMPLOYEE_NO) + CONVERT(CHAR(8),WEEK_DATE,112) IN ");
                    strQry.AppendLine("(SELECT DISTINCT ");
                    strQry.AppendLine(" CONVERT(CHAR(5),EEWC.EMPLOYEE_NO) + CONVERT(CHAR(8),EEWC.WEEK_DATE,112) ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_WEEK_CURRENT EEWC");

                    //2017-01-24 (Employee Not Closed)
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON EEWC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND EEWC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY PH");
                    strQry.AppendLine(" ON PH.PUBLIC_HOLIDAY_DATE >= EEWC.WEEK_DATE_FROM");
                    strQry.AppendLine(" AND PH.PUBLIC_HOLIDAY_DATE <= EEWC.WEEK_DATE");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
                    strQry.AppendLine(" ON PC.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = " + parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND PC.PAY_PUBLIC_HOLIDAY_IND = 'Y'");

                    strQry.AppendLine(" WHERE EEWC.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND EEWC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]) + ")");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" PAIDHOLIDAY_INDICATOR = 'Y' ");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND STR(EMPLOYEE_NO) IN ");
                    strQry.AppendLine("(SELECT DISTINCT ");
                    strQry.AppendLine(" STR(EEWC.EMPLOYEE_NO) ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_WEEK_CURRENT EEWC");

                    //2017-01-24 (Employee Not Closed)
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON EEWC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND EEWC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                    strQry.AppendLine(" WHERE EEWC.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND EEWC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EEWC.PAIDHOLIDAY_INDICATOR = 'Y') ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                }

            Insert_Wage_Run_Records_Continue:
#if (DEBUG)
                //Release Runs off a Queue and Uses a Table as Feedback 
                bytCompress = Get_Employee_WageRun(parint64CompanyNo, "W");
                
                DataSet.Dispose();
                DataSet = null;
#endif
                //2017-08-11
                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.OPEN_RUN_QUEUE_COMPLETED");

                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",USER_NO");
                strQry.AppendLine(",PAY_PERIOD_DATE");

                strQry.AppendLine(",PAY_CATEGORY_NUMBERS");
                strQry.AppendLine(",PAY_CATEGORY_NUMBERS_NOT_USED");
                strQry.AppendLine(",OPEN_RUN_QUEUE_IND");

                strQry.AppendLine(",START_RUN_DATE");
                strQry.AppendLine(",END_RUN_DATE)");

                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",USER_NO");
                strQry.AppendLine(",PAY_PERIOD_DATE");

                strQry.AppendLine(",PAY_CATEGORY_NUMBERS");
                strQry.AppendLine(",PAY_CATEGORY_NUMBERS_NOT_USED");
                strQry.AppendLine(",'C'");
                strQry.AppendLine(",START_RUN_DATE");
                strQry.AppendLine(",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" FROM InteractPayroll.dbo.OPEN_RUN_QUEUE");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("W"));
                strQry.AppendLine(" AND PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parCurrentDateTime.ToString("yyyy-MM-dd")));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                //2017-08-11
                strQry.Clear();

                strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.OPEN_RUN_QUEUE");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("W"));
                strQry.AppendLine(" AND PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parCurrentDateTime.ToString("yyyy-MM-dd")));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
                strQry.AppendLine(" SET BACKUP_DB_IND = 1");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            }
            catch(Exception ex)
            {
                Write_Log(ex, strClassNameFunctionAndParameters, strQry.ToString(), true);

                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.OPEN_RUN_QUEUE");

                strQry.AppendLine(" SET ");
                strQry.AppendLine(" OPEN_RUN_QUEUE_IND = 'F'");
                strQry.AppendLine(",END_RUN_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("W"));
                strQry.AppendLine(" AND PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parCurrentDateTime.ToString("yyyy-MM-dd")));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            }

            return bytCompress;
        }

        public byte[] Insert_Salary_Run_Records(Int64 parint64CompanyNo, DateTime parCurrentDateTime, string strPayCategoryNoIN)
        {
            string strClassNameFunctionAndParameters = pvtstrClassName + " Insert_Salary_Run_Records CompanyNo=" + parint64CompanyNo + ",parCurrentDateTime=" + parCurrentDateTime.ToString("yyyy-MM-dd") + ",strPayCategoryNoIN=" + strPayCategoryNoIN;
            
            StringBuilder strQry = new StringBuilder();
            byte[] bytCompress = null;

            try
            {
                //2018-10-27 - Start
                strQry.Clear();

                strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ");
                strQry.AppendLine(" DISABLE TRIGGER tgr_EMPLOYEE_SALARY_TIMESHEET_CURRENT_Maintain_EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ");
                strQry.AppendLine(" DISABLE TRIGGER tgr_EMPLOYEE_SALARY_BREAK_CURRENT_Maintain_EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT");

                strQry.AppendLine(" SET ");
                strQry.AppendLine(" INCLUDED_IN_RUN_IND = NULL");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_NO  IN (" + strPayCategoryNoIN + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
              
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT");

                strQry.AppendLine(" SET ");
                strQry.AppendLine(" INCLUDED_IN_RUN_IND = NULL");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_NO  IN (" + strPayCategoryNoIN + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
               
                strQry.Clear();

                strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ");
                strQry.AppendLine(" ENABLE TRIGGER tgr_EMPLOYEE_SALARY_TIMESHEET_CURRENT_Maintain_EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ");
                strQry.AppendLine(" ENABLE TRIGGER tgr_EMPLOYEE_SALARY_BREAK_CURRENT_Maintain_EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                //2018-10-27 - End

                //2017-08-11
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.OPEN_RUN_QUEUE");

                strQry.AppendLine(" SET ");
                //S=Started
                strQry.AppendLine(" OPEN_RUN_QUEUE_IND = 'S'");
                strQry.AppendLine(",START_RUN_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("S"));
                strQry.AppendLine(" AND PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parCurrentDateTime.ToString("yyyy-MM-dd")));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                int intReturnCode = 0;
                int intRunNo = -1;

                bool blnTooManyPublicHolidays = false;

                DataSet DataSet = new DataSet();
                DataSet parDataSet = new DataSet();

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PC.PAY_CATEGORY_NO");
                strQry.AppendLine(",MAX(PCPH.PAY_PERIOD_DATE) AS PREV_PAY_PERIOD_DATE ");
                strQry.AppendLine(",PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY = ");

                strQry.AppendLine(" DATEADD(dd,1,MIN(CASE ");

                //Errol - 2015-02-13
                strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y' ");

                strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

                strQry.AppendLine(" END)) ");

                strQry.AppendLine(",'' AS PUBLIC_HOLIDAYS_ERROR ");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON PC.COMPANY_NO = E.COMPANY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND EMPLOYEE_ENDDATE IS NULL");
                strQry.AppendLine(" AND NOT EMPLOYEE_LAST_RUNDATE IS NULL");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");
                strQry.AppendLine(" ON PC.COMPANY_NO = PCPH.COMPANY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = PCPH.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND PCPH.RUN_TYPE = 'P'");

                strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO IN (" + strPayCategoryNoIN + ")");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("S"));
                strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");
                strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" PC.PAY_CATEGORY_NO");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" 1");
                strQry.AppendLine(",2");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), parDataSet, "Upload", parint64CompanyNo);
#if (DEBUG)
                //ELR Check Leave Authorisation and that Public Holidays in Run don't Exceed 5
                for (int intRow = 0; intRow < parDataSet.Tables["Upload"].Rows.Count; intRow++)
                {
                    if (DataSet.Tables["LeaveCheck"] != null)
                    {
                        DataSet.Tables.Remove("LeaveCheck");
                    }

                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EPCLPAC.COMPANY_NO ");
                    strQry.AppendLine(",EPCLPAC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(",EPCLPAC.PAY_CATEGORY_TYPE ");

                    strQry.AppendLine(",COUNT(DISTINCT EPCLPAC.EMPLOYEE_NO) AS AUTHORISE_TOTAL");
                    strQry.AppendLine(",COUNT(DISTINCT EPCLPAC1.EMPLOYEE_NO) AS AUTHORISE_CURRENT");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT EPCLPAC");

                    //2017-01-24 (Employee Not Closed)
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON EPCLPAC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND EPCLPAC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPCLPAC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT EPCLPAC1");

                    strQry.AppendLine(" ON EPCLPAC.COMPANY_NO = EPCLPAC1.COMPANY_NO  ");
                    strQry.AppendLine(" AND EPCLPAC.EMPLOYEE_NO = EPCLPAC1.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPCLPAC.PAY_CATEGORY_NO = EPCLPAC1.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND EPCLPAC.PAY_CATEGORY_TYPE = EPCLPAC1.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EPCLPAC.LEVEL_NO = EPCLPAC1.LEVEL_NO");
                    strQry.AppendLine(" AND EPCLPAC1.AUTHORISED_IND = 'Y'");

                    strQry.AppendLine(" WHERE EPCLPAC.COMPANY_NO = " + parint64CompanyNo);

                    strQry.AppendLine(" AND EPCLPAC.PAY_CATEGORY_NO = " + parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND EPCLPAC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("S"));

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" EPCLPAC.COMPANY_NO ");
                    strQry.AppendLine(",EPCLPAC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(",EPCLPAC.PAY_CATEGORY_TYPE ");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "LeaveCheck", parint64CompanyNo);

                    for (int intLeaveRow = 0; intLeaveRow < DataSet.Tables["LeaveCheck"].Rows.Count; intLeaveRow++)
                    {
                        if (Convert.ToInt32(DataSet.Tables["LeaveCheck"].Rows[intLeaveRow]["AUTHORISE_TOTAL"]) != Convert.ToInt32(DataSet.Tables["LeaveCheck"].Rows[intLeaveRow]["AUTHORISE_CURRENT"]))
                        {

                            strQry.Clear();
                            strQry.AppendLine(" SELECT  ");
                            strQry.AppendLine(" COMPANY_NO ");

                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY");

                            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

                            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), parDataSet, "LeaveAuthorisationError", parint64CompanyNo);

                            bytCompress = clsDBConnectionObjects.Compress_DataSet(parDataSet);
                            DataSet.Dispose();
                            DataSet = null;

                            return bytCompress;
                        }
                    }

                    if (DataSet.Tables["PublicHolidayTest"] != null)
                    {
                        DataSet.Tables.Remove("PublicHolidayTest");
                    }

                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" PUBLIC_HOLIDAY_DATE");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY PH");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
                    strQry.AppendLine(" ON PC.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = " + parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());

                    strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("S"));
                    strQry.AppendLine(" AND PC.PAY_PUBLIC_HOLIDAY_IND = 'Y' ");

                    strQry.AppendLine(" WHERE PH.PUBLIC_HOLIDAY_DATE >= '" + Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND PH.PUBLIC_HOLIDAY_DATE <= '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PublicHolidayTest", parint64CompanyNo);

                    if (DataSet.Tables["PublicHolidayTest"].Rows.Count > 5)
                    {
                        parDataSet.Tables["Upload"].Rows[intRow]["PUBLIC_HOLIDAYS_ERROR"] = "Y";

                        blnTooManyPublicHolidays = true;
                    }

                    if (parDataSet.Tables["Upload"].Rows[intRow]["PREV_PAY_PERIOD_DATE"] == System.DBNull.Value)
                    {
                        parDataSet.Tables["Upload"].Rows[intRow]["PREV_PAY_PERIOD_DATE"] = Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY"]);

                    }
                }

                if (blnTooManyPublicHolidays == true)
                {
                    bytCompress = clsDBConnectionObjects.Compress_DataSet(parDataSet);
                    DataSet.Dispose();
                    DataSet = null;

                    return bytCompress;
                }
#endif
                string strSelectAddonQry = "";
                StringBuilder strFieldNamesInitialised = new StringBuilder();
                DateTime dtBeginFinancialYear;
                DateTime dtEndFinancialYear;
                DateTime dtPreviousDateTime;
                DateTime dtTempDateTime;

                if (parCurrentDateTime.Month > 2)
                {
                    dtBeginFinancialYear = new DateTime(parCurrentDateTime.Year, 3, 1);
                }
                else
                {
                    dtBeginFinancialYear = new DateTime(parCurrentDateTime.Year - 1, 3, 1);
                }

                //Last Day Of Fiscal Year
                dtEndFinancialYear = dtBeginFinancialYear.AddYears(1).AddDays(-1);

                for (int intRow = 0; intRow < parDataSet.Tables["Upload"].Rows.Count; intRow++)
                {
                    if (DataSet.Tables["RunNo"] != null)
                    {
                        DataSet.Tables.Remove("RunNo");
                    }

                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" ISNULL(RUN_NO,0) + 1 AS RUN_NO");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_PERIOD_DATE = '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));

                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND RUN_TYPE = 'P'");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "RunNo", parint64CompanyNo);

                    if (DataSet.Tables["RunNo"].Rows.Count == 0)
                    {
                        intRunNo = 1;
                    }
                    else
                    {
                        intRunNo = Convert.ToInt32(DataSet.Tables["RunNo"].Rows[0]["RUN_NO"]);
                    }

                    //Initial PAY_CATEGORY does NOT have Value - Use Min EMPLOYEE_LAST_RUNDATE
                    if (parDataSet.Tables["Upload"].Rows[intRow]["PREV_PAY_PERIOD_DATE"] == System.DBNull.Value)
                    {
                        //Already Added 1 to Last Rundate
                        parDataSet.Tables["Upload"].Rows[intRow]["PREV_PAY_PERIOD_DATE"] = Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY"]);
                        dtPreviousDateTime = Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_PAY_PERIOD_DATE"]);
                    }
                    else
                    {
                        dtPreviousDateTime = Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_PAY_PERIOD_DATE"]).AddDays(1);
                    }

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT_CURRENT");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",RUN_NO");

                    strFieldNamesInitialised.Clear();
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("LEAVE_SHIFT_CURRENT", ref strQry, ref strFieldNamesInitialised, "", parint64CompanyNo);

                    strQry.AppendLine(")");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" COMPANY_NO");
                    strQry.AppendLine("," + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(",'S'");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.COMPANY");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" SALARY_RUN_IND = 'Y'");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");
                    strQry.AppendLine(",RUN_TYPE");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",RUN_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE_FROM");
                    strQry.AppendLine(",SALARY_TIMESHEET_ENDDATE");

                    strFieldNamesInitialised.Clear();
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("PAY_CATEGORY_PERIOD_CURRENT", ref strQry, ref strFieldNamesInitialised, "", parint64CompanyNo);

                    strQry.AppendLine(")");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" COMPANY_NO");
                    strQry.AppendLine(",'" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",'P'");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.AppendLine(",'" + dtPreviousDateTime.ToString("yyyy-MM-dd") + "'");
                    //SALARY_TIMESHEET_ENDDATE
                    strQry.AppendLine(",NULL ");

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK_CURRENT");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",PAY_CATEGORY_BREAK_NO");
                    strQry.AppendLine(",RUN_NO");

                    strFieldNamesInitialised.Clear();
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("PAY_CATEGORY_BREAK_CURRENT", ref strQry, ref strFieldNamesInitialised, "", parint64CompanyNo);

                    strQry.AppendLine(")");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",PAY_CATEGORY_BREAK_NO");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",RUN_TYPE");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",RUN_NO");
                    strQry.AppendLine(",HOURLY_RATE");
                    strQry.AppendLine(",OVERTIME_VALUE_BF");
                    strQry.AppendLine(",OVERTIME_VALUE_CF");
                    strQry.AppendLine(",DEFAULT_IND");
                    //2017-07-31
                    strQry.AppendLine(",PAY_CATEGORY_USED_IND)");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EPC.COMPANY_NO");
                    strQry.AppendLine(",EPC.EMPLOYEE_NO");
                    strQry.AppendLine(",'P'");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine("," + intRunNo.ToString());

                    //NB Salary EPC.HOURLY_RATE Holds Monthly value
                    strQry.AppendLine(",ROUND((EPC.HOURLY_RATE * E.EMPLOYEE_NUMBER_CHEQUES) / (PC.SALARY_DAYS_PER_YEAR * (PC.SALARY_MINUTES_PAID_PER_DAY / 60)),2)");

                    //NB. Value From CF is Carried to BF
                    strQry.AppendLine(",ISNULL(EPCH.OVERTIME_VALUE_CF,0)");
                    strQry.AppendLine(",0");

                    strQry.AppendLine(",EPC.DEFAULT_IND");

                    //2017-07-31 - PAY_CATEGORY_USED_IND
                    strQry.AppendLine(",'Y'");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");

                    //Used to Work out Hourly Rate of Employee (Salaries)
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
                    strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON EPC.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND EPC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_LAST_RUNDATE < '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");
                    strQry.AppendLine(" ON EPC.COMPANY_NO = EPCH.COMPANY_NO ");
                    strQry.AppendLine(" AND EPC.EMPLOYEE_NO = EPCH.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = EPCH.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = EPCH.PAY_CATEGORY_TYPE");
                    //If Previous Fiscal Year then No C/f of History Values (Will be Zero)
                    strQry.AppendLine(" AND EPCH.PAY_PERIOD_DATE >= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND EPCH.RUN_TYPE = 'P'");

                    strQry.AppendLine(" AND STR(EPCH.EMPLOYEE_NO) + CONVERT(CHAR,EPCH.PAY_PERIOD_DATE) IN ");
                    strQry.AppendLine("(SELECT STR(EMPLOYEE_NO) + CONVERT(CHAR,MAX(PAY_PERIOD_DATE)) ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY ");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND RUN_TYPE = 'P'");
                    strQry.AppendLine(" GROUP BY EMPLOYEE_NO)");

                    strQry.AppendLine(" WHERE EPC.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",RUN_TYPE");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",RUN_NO");
                    strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(",EXTRA_CHEQUES_HISTORY");
                    strQry.AppendLine(",EXTRA_CHEQUES_CURRENT");
                    strQry.AppendLine(",CLOSE_IND");
                    strQry.AppendLine(",PAYSLIP_IND");

                    //ELR-2014-05-01
                    strQry.AppendLine(",NUMBER_MEDICAL_AID_DEPENDENTS");
                    strQry.AppendLine(",OCCUPATION_NO");
                    strQry.AppendLine(",CURRENT_YEAR_LEAVE_SHIFTS_PER_RUN");
                    strQry.AppendLine(",PREV_YEAR_LEAVE_SHIFTS_PER_RUN");

                    //2012-11-23
                    strQry.AppendLine(",LEAVE_SHIFT_NO");

                    strQry.AppendLine(",SALARY_MONTH_PAYMENT)");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",'P'");
                    strQry.AppendLine(",'S'");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.AppendLine(",E.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(",ISNULL(EIH1.EXTRA_CHEQUES_HISTORY + EIH1.EXTRA_CHEQUES_CURRENT,0)");

                    //Double Cheque For BirthDay
                    strQry.AppendLine(",ISNULL(C.VALUE_1,0)");

                    strQry.AppendLine(",'N'");
                    strQry.AppendLine(",'Y'");

                    //ELR 2014-05-02
                    strQry.AppendLine(",ISNULL(E.NUMBER_MEDICAL_AID_DEPENDENTS,0)");
                    strQry.AppendLine(",ISNULL(E.OCCUPATION_NO,0)");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");

                    //2012-11-23
                    strQry.AppendLine(",E.LEAVE_SHIFT_NO");

                    //Monthly Payment is held in HOURLY_RATE for Salaries
                    strQry.AppendLine(",EPC.HOURLY_RATE");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C ");
                    strQry.AppendLine(" ON E.COMPANY_NO = C.COMPANY_NO");
                    strQry.AppendLine(" AND C.SALARY_DOUBLE_CHEQUE_BIRTHDAY_IND = 'Y'");
                    strQry.AppendLine(" AND E.EMPLOYEE_NUMBER_CHEQUES > 12");
                    strQry.AppendLine(" AND REPLICATE('0',2 - LEN(MONTH(E.EMPLOYEE_BIRTHDATE))) + CONVERT(VARCHAR,MONTH(E.EMPLOYEE_BIRTHDATE)) ");
                    strQry.AppendLine(" + REPLICATE('0',2 - LEN(DAY(E.EMPLOYEE_BIRTHDATE))) + CONVERT(VARCHAR,DAY(E.EMPLOYEE_BIRTHDATE)) IN ");
                    strQry.AppendLine("(SELECT  ");
                    strQry.AppendLine(" REPLICATE('0',2 - LEN(MONTH(DAY_DATE))) + CONVERT(VARCHAR,MONTH(DAY_DATE)) ");
                    strQry.AppendLine(" + REPLICATE('0',2 - LEN(DAY(DAY_DATE))) + CONVERT(VARCHAR,DAY(DAY_DATE)) ");

                    strQry.AppendLine(" FROM InteractPayroll.dbo.DATES ");

                    strQry.AppendLine(" WHERE DAY_DATE >= E.EMPLOYEE_LAST_RUNDATE ");
                    strQry.AppendLine(" AND DAY_DATE <= '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "')");

                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY EIH1 ");
                    strQry.AppendLine(" ON E.COMPANY_NO = EIH1.COMPANY_NO");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EIH1.EMPLOYEE_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EIH1.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EIH1.RUN_TYPE = 'P'");

                    strQry.AppendLine(" AND STR(EIH1.EMPLOYEE_NO) + CONVERT(CHAR,EIH1.PAY_PERIOD_DATE) IN ");
                    strQry.AppendLine("(SELECT STR(EMPLOYEE_NO) + CONVERT(CHAR,MAX(PAY_PERIOD_DATE)) ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY ");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND RUN_TYPE = 'P'");
                    //If Previous Fiscal Year then No C/f of History Values (Will be Zero)
                    strQry.AppendLine(" AND PAY_PERIOD_DATE >= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" EMPLOYEE_NO) ");

                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY EIH ");
                    strQry.AppendLine(" ON E.COMPANY_NO = EIH.COMPANY_NO");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EIH.EMPLOYEE_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EIH.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EIH.RUN_TYPE = 'P'");

                    strQry.AppendLine(" AND STR(EIH.EMPLOYEE_NO) + CONVERT(CHAR,EIH.PAY_PERIOD_DATE) IN ");
                    strQry.AppendLine("(SELECT STR(EMPLOYEE_NO) + CONVERT(CHAR,MAX(PAY_PERIOD_DATE)) ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY ");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND RUN_TYPE = 'P'");
                    //If Previous Fiscal Year then No C/f of History Values (Will be Zero)
                    strQry.AppendLine(" AND PAY_PERIOD_DATE >= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(" AND EMPLOYEE_NO IN ");

                    //Medical Aid Deduction / Earning
                    strQry.AppendLine("(SELECT ");
                    strQry.AppendLine(" EE.EMPLOYEE_NO");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EE ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EN ");
                    strQry.AppendLine(" ON EE.COMPANY_NO = EN.COMPANY_NO ");
                    strQry.AppendLine(" AND EE.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EE.EARNING_NO = EN.EARNING_NO");
                    strQry.AppendLine(" AND EN.IRP5_CODE = 3810");
                    strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE EE.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND EE.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND EE.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" UNION ");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" ED.EMPLOYEE_NO");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ED ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.DEDUCTION D ");
                    strQry.AppendLine(" ON ED.COMPANY_NO = D.COMPANY_NO");
                    strQry.AppendLine(" AND ED.PAY_CATEGORY_TYPE = D.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND ED.DEDUCTION_NO = D.DEDUCTION_NO");
                    strQry.AppendLine(" AND D.IRP5_CODE = 4005");
                    strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE ED.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND ED.DATETIME_DELETE_RECORD IS NULL)");

                    strQry.AppendLine(" GROUP BY EMPLOYEE_NO)");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND E.EMPLOYEE_LAST_RUNDATE < '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    //Insert Record for All Linked Earnings 
                    //(NOT NT/Income/Overtime/Bonus/Holidays/Leave/Tax Directive)		
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",RUN_TYPE");
                    strQry.AppendLine(",EARNING_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",RUN_NO");
                    strQry.AppendLine(",TOTAL");
                    //Used Salaries for Income when Employee is Closed and CheckBox Removes Current Earnings and Normal Leave 
                    strQry.AppendLine(",TOTAL_ORIGINAL");
                    //Used Salaries for Normal Leave when Employee is Closed 
                    strQry.AppendLine(",HOURS_DECIMAL_OTHER_VALUE");

                    //Used Salaries for Normal Leave when Employee is Closed and CheckBox Removes Current Earnings and Normal Leave 
                    strQry.AppendLine(",HOURS_DECIMAL_OTHER_VALUE_ZERO");

                    //2012-10-22 
                    strQry.AppendLine(",DAY_DECIMAL_OTHER_VALUE");
                    //2013-09-26 - EMPLOYEE_EARNING_BATCH_TEMP (Add Batch Earning for NT/OT1/OT2/OT3 
                    //Used Salaries for Normal Leave when Employee is Closed 
                    strQry.AppendLine(",DAY_DECIMAL_OTHER_VALUE_ZERO");
                    strQry.AppendLine(",HOURS_DECIMAL");

                    //ELR 2014-05-24
                    strQry.AppendLine(",EARNING_TYPE_IND");

                    strFieldNamesInitialised.Clear();
                    clsDBConnectionObjects.Initialise_DataSet_Numeric_Fields("EMPLOYEE_EARNING_CURRENT", ref strQry, ref strFieldNamesInitialised, parint64CompanyNo);

                    strQry.AppendLine(")");

                    strQry.AppendLine(" SELECT DISTINCT");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",'P'");
                    strQry.AppendLine(",EN.EARNING_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.AppendLine(",TOTAL = ");
                    strQry.AppendLine(" CASE");

                    strQry.AppendLine(" WHEN EE.EARNING_TYPE_IND = 'X' ");
                    strQry.AppendLine(" THEN 0 ");

                    //2014-03-21
                    strQry.AppendLine(" WHEN NOT EEBT.AMOUNT IS NULL ");
                    strQry.AppendLine(" THEN EEBT.AMOUNT ");

                    strQry.AppendLine(" ELSE EE.AMOUNT");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(",TOTAL_ORIGINAL = ");
                    strQry.AppendLine(" CASE");

                    strQry.AppendLine(" WHEN EE.EARNING_TYPE_IND = 'X' ");
                    strQry.AppendLine(" THEN 0 ");

                    //2014-03-21
                    strQry.AppendLine(" WHEN NOT EEBT.AMOUNT IS NULL ");
                    strQry.AppendLine(" THEN EEBT.AMOUNT ");

                    strQry.AppendLine(" ELSE EE.AMOUNT");

                    strQry.AppendLine(" END ");

                    //2012-08-15 Caters for Multiple (EARNING_TYPE_IND = X)
                    strQry.AppendLine(",HOURS_DECIMAL_OTHER_VALUE = ");

                    strQry.AppendLine(" CASE");

                    strQry.AppendLine(" WHEN EE.EARNING_TYPE_IND = 'X' ");

                    strQry.AppendLine(" THEN EE.AMOUNT ");

                    strQry.AppendLine(" ELSE 0");

                    strQry.AppendLine(" END ");

                    //2016-01-08 HOURS_DECIMAL_OTHER_VALUE_ZERO
                    strQry.AppendLine(",0");

                    //2012-10-22 DAY_DECIMAL_OTHER_VALUE
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    //2013-09-26 - EMPLOYEE_EARNING_BATCH_TEMP (Add Batch Earning for NT/OT1/OT2/OT3 
                    strQry.AppendLine(",0");

                    //ELR 2014-05-24
                    strQry.AppendLine(",EE.EARNING_TYPE_IND");

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    //Link To Default Pay Category
                    strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    //ELR - 2014-05-24
                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EE");
                    strQry.AppendLine(" ON E.COMPANY_NO = EE.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EE.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EE.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EE.EARNING_TYPE_IND <> 'M' ");
                    strQry.AppendLine(" AND EE.DATETIME_DELETE_RECORD IS NULL ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EN");
                    strQry.AppendLine(" ON EE.COMPANY_NO = EN.COMPANY_NO");
                    strQry.AppendLine(" AND EE.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EE.EARNING_NO = EN.EARNING_NO");
                    strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL ");

                    //2014-03-21
                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_BATCH_TEMP EEBT");
                    strQry.AppendLine(" ON E.COMPANY_NO = EEBT.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EEBT.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EN.EARNING_NO = EEBT.EARNING_NO");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EEBT.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EEBT.PROCESS_NO = 0");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_LAST_RUNDATE < '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                    //Income (Salary / Number of Cheques)
                    strQry.AppendLine(" UNION");

                    strQry.AppendLine(" SELECT DISTINCT");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",'P'");
                    strQry.AppendLine(",1 AS EARNING_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine("," + intRunNo.ToString());

                    //NB If Pay Double Cheque On Birthday is Set then 2 * Value
                    strQry.AppendLine(",ISNULL(C.VALUE_2,1) * EPC.HOURLY_RATE");
                    strQry.AppendLine(",ISNULL(C.VALUE_2,1) * EPC.HOURLY_RATE");

                    strQry.AppendLine(",0");

                    //2016-01-08 HOURS_DECIMAL_OTHER_VALUE_ZERO
                    strQry.AppendLine(",0");

                    //2012-10-22 DAY_DECIMAL_OTHER_VALUE
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    //2013-09-26 - EMPLOYEE_EARNING_BATCH_TEMP (Add Batch Earning for NT/OT1/OT2/OT3 
                    strQry.AppendLine(",0");

                    //ELR 2015-03-18
                    strQry.AppendLine(",'F' AS EARNING_TYPE_IND");

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    //Link To Default Pay Category
                    strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    //ELR - 2014-05-24
                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EE");
                    strQry.AppendLine(" ON E.COMPANY_NO = EE.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EE.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EE.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EE.EARNING_TYPE_IND <> 'M' ");
                    strQry.AppendLine(" AND EE.DATETIME_DELETE_RECORD IS NULL ");

                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C ");
                    strQry.AppendLine(" ON E.COMPANY_NO = C.COMPANY_NO");
                    strQry.AppendLine(" AND C.SALARY_DOUBLE_CHEQUE_BIRTHDAY_IND = 'Y'");
                    strQry.AppendLine(" AND E.EMPLOYEE_NUMBER_CHEQUES > 12");
                    strQry.AppendLine(" AND REPLICATE('0',2 - LEN(MONTH(E.EMPLOYEE_BIRTHDATE))) + CONVERT(VARCHAR,MONTH(E.EMPLOYEE_BIRTHDATE)) ");
                    strQry.AppendLine(" + REPLICATE('0',2 - LEN(DAY(E.EMPLOYEE_BIRTHDATE))) + CONVERT(VARCHAR,DAY(E.EMPLOYEE_BIRTHDATE)) IN ");
                    strQry.AppendLine("(SELECT  ");
                    strQry.AppendLine(" REPLICATE('0',2 - LEN(MONTH(DAY_DATE))) + CONVERT(VARCHAR,MONTH(DAY_DATE)) ");
                    strQry.AppendLine(" + REPLICATE('0',2 - LEN(DAY(DAY_DATE))) + CONVERT(VARCHAR,DAY(DAY_DATE)) ");

                    strQry.AppendLine(" FROM InteractPayroll.dbo.DATES ");

                    strQry.AppendLine(" WHERE DAY_DATE >= E.EMPLOYEE_LAST_RUNDATE ");
                    strQry.AppendLine(" AND DAY_DATE <= '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "')");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_LAST_RUNDATE < '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                    //Create Values For NT (Wages)/Overtime/Bonus/Holidays
                    strQry.AppendLine(" UNION");

                    strQry.AppendLine(" SELECT DISTINCT");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",'P'");
                    strQry.AppendLine(",EN.EARNING_NO");
                    strQry.AppendLine(",EPCC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPCC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine("," + intRunNo.ToString());

                    //2013-09-23
                    strQry.AppendLine(",TOTAL = ");

                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN EEBT.EARNING_NO = 7 THEN EEBT.AMOUNT ");

                    //Normal Time 
                    strQry.AppendLine(" WHEN EEBT.EARNING_NO = 2 THEN EEBT.AMOUNT * EPCC.HOURLY_RATE ");

                    //OverTime 1 
                    strQry.AppendLine(" WHEN EEBT.EARNING_NO = 3 THEN EEBT.AMOUNT * (C.SALARY_OVERTIME1_RATE * EPCC.HOURLY_RATE) ");

                    //OverTime 2 
                    strQry.AppendLine(" WHEN EEBT.EARNING_NO = 4 THEN EEBT.AMOUNT * (C.SALARY_OVERTIME2_RATE * EPCC.HOURLY_RATE) ");

                    //OverTime 3 
                    strQry.AppendLine(" WHEN EEBT.EARNING_NO = 5 THEN EEBT.AMOUNT * (C.SALARY_OVERTIME3_RATE * EPCC.HOURLY_RATE) ");

                    strQry.AppendLine(" ELSE 0 ");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(",TOTAL_ORIGINAL = ");

                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN EEBT.EARNING_NO = 7 THEN EEBT.AMOUNT ");

                    //Normal Time 
                    strQry.AppendLine(" WHEN EEBT.EARNING_NO = 2 THEN EEBT.AMOUNT * EPCC.HOURLY_RATE ");

                    //OverTime 1 
                    strQry.AppendLine(" WHEN EEBT.EARNING_NO = 3 THEN EEBT.AMOUNT * (C.SALARY_OVERTIME1_RATE * EPCC.HOURLY_RATE) ");

                    //OverTime 2 
                    strQry.AppendLine(" WHEN EEBT.EARNING_NO = 4 THEN EEBT.AMOUNT * (C.SALARY_OVERTIME2_RATE * EPCC.HOURLY_RATE) ");

                    //OverTime 3 
                    strQry.AppendLine(" WHEN EEBT.EARNING_NO = 5 THEN EEBT.AMOUNT * (C.SALARY_OVERTIME3_RATE * EPCC.HOURLY_RATE) ");

                    strQry.AppendLine(" ELSE 0 ");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(",0");

                    //2016-01-08 HOURS_DECIMAL_OTHER_VALUE_ZERO
                    strQry.AppendLine(",0");

                    //2012-10-22 DAY_DECIMAL_OTHER_VALUE
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");

                    //2013-09-26 - EMPLOYEE_EARNING_BATCH_TEMP (Add Batch Earning for NT/OT1/OT2/OT3 
                    strQry.AppendLine(",HOURS_DECIMAL = ");

                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN EEBT.EARNING_NO IN (2,3,4,5) THEN EEBT.AMOUNT");

                    strQry.AppendLine(" ELSE 0 ");

                    strQry.AppendLine(" END ");

                    //ELR 2014-05-24
                    strQry.AppendLine(",'F' AS EARNING_TYPE_IND");

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P'");

                    //Not Yet Implemented
                    //strQry.AppendLine(" AND EPCC.RUN_NO = 1");

                    //ELR - 2014-05-24
                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EE");
                    strQry.AppendLine(" ON E.COMPANY_NO = EE.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EE.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EE.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EE.EARNING_TYPE_IND <> 'M' ");
                    strQry.AppendLine(" AND EE.DATETIME_DELETE_RECORD IS NULL ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EN");
                    strQry.AppendLine(" ON E.COMPANY_NO = EN.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
                    strQry.AppendLine(" ON E.COMPANY_NO = PC.COMPANY_NO ");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL ");

                    //2013-09-23
                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_BATCH_TEMP EEBT");
                    strQry.AppendLine(" ON E.COMPANY_NO = EEBT.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EEBT.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EN.EARNING_NO = EEBT.EARNING_NO");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EEBT.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EEBT.PROCESS_NO = 0");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C");
                    strQry.AppendLine(" ON E.COMPANY_NO = C.COMPANY_NO ");
                    strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL ");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_LAST_RUNDATE < '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                    //Salaries
                    strQry.AppendLine(" AND (EN.EARNING_NO IN (7)");

                    //2013-09-23 - Normal Time
                    strQry.AppendLine(" OR (EN.EARNING_NO = 2)");

                    strQry.AppendLine(" OR (EN.EARNING_NO = 3");
                    strQry.AppendLine(" AND C.SALARY_OVERTIME1_RATE <> 0)");

                    strQry.AppendLine(" OR (EN.EARNING_NO = 4");
                    strQry.AppendLine(" AND C.SALARY_OVERTIME2_RATE <> 0)");

                    strQry.AppendLine(" OR (EN.EARNING_NO = 5");
                    strQry.AppendLine(" AND C.SALARY_OVERTIME3_RATE <> 0))");

                    //Noraml Leave (200) - Outstanding Amount to be Used when Employee is Closed
                    //NB Any Leave Not Processed or Waiting to be Processed will be ignored on Close of Employee - Is in Other Calculations
                    //NB Any Leave Not Processed or Waiting to be Processed will be ignored on Close of Employee - Is in Other Calculations
                    //NB Any Leave Not Processed or Waiting to be Processed will be ignored on Close of Employee - Is in Other Calculations
                    strQry.AppendLine(" UNION");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",'P'");
                    strQry.AppendLine(",200");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine("," + intRunNo.ToString());
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");

                    strQry.AppendLine(",HOURS_DECIMAL_OTHER_VALUE = ");

                    strQry.AppendLine(" CASE");

                    //Employee Changed Leave Category and Already ExceedS Total for Year
                    strQry.AppendLine(" WHEN CURRENT_YEAR_ACCUM_TABLE.CURRENT_YEAR_ACCUM_DAYS >= LS.NORM_PAID_DAYS");
                    strQry.AppendLine(" THEN ROUND((ISNULL(PREV_YEAR_ACCUM_TABLE.PREV_YEAR_ACCUM_DAYS,0) + CURRENT_YEAR_ACCUM_TABLE.CURRENT_YEAR_ACCUM_DAYS - ISNULL(LEAVE_TO_BE_PROCESSED_TABLE.SUM_LEAVE_DAYS_DECIMAL,0)) * (PC.SALARY_MINUTES_PAID_PER_DAY / 60),2)");

                    strQry.AppendLine(" WHEN CURRENT_YEAR_ACCUM_TABLE.CURRENT_YEAR_ACCUM_DAYS + LS.NORM_PAID_PER_PERIOD + 0.15 >= LS.NORM_PAID_DAYS");
                    strQry.AppendLine(" THEN ROUND((ISNULL(PREV_YEAR_ACCUM_TABLE.PREV_YEAR_ACCUM_DAYS,0) + LS.NORM_PAID_DAYS - ISNULL(LEAVE_TO_BE_PROCESSED_TABLE.SUM_LEAVE_DAYS_DECIMAL,0)) * (PC.SALARY_MINUTES_PAID_PER_DAY / 60),2)");

                    strQry.AppendLine(" ELSE ROUND((LS.NORM_PAID_PER_PERIOD + ISNULL(PREV_YEAR_ACCUM_TABLE.PREV_YEAR_ACCUM_DAYS,0) + ISNULL(CURRENT_YEAR_ACCUM_TABLE.CURRENT_YEAR_ACCUM_DAYS,0) - ISNULL(LEAVE_TO_BE_PROCESSED_TABLE.SUM_LEAVE_DAYS_DECIMAL,0)) * (PC.SALARY_MINUTES_PAID_PER_DAY / 60),2)");

                    strQry.AppendLine(" END ");

                    //2016-01-08 HOURS_DECIMAL_OTHER_VALUE_ZERO
                    strQry.AppendLine(",HOURS_DECIMAL_OTHER_VALUE_ZERO = ");

                    strQry.AppendLine(" CASE");

                    //Employee Changed Leave Category and Already ExceedS Total for Year
                    strQry.AppendLine(" WHEN CURRENT_YEAR_ACCUM_TABLE.CURRENT_YEAR_ACCUM_DAYS >= LS.NORM_PAID_DAYS");
                    strQry.AppendLine(" THEN ROUND((ISNULL(PREV_YEAR_ACCUM_TABLE.PREV_YEAR_ACCUM_DAYS,0) + CURRENT_YEAR_ACCUM_TABLE.CURRENT_YEAR_ACCUM_DAYS - ISNULL(LEAVE_TO_BE_PROCESSED_TABLE.SUM_LEAVE_DAYS_DECIMAL,0)) * (PC.SALARY_MINUTES_PAID_PER_DAY / 60),2)");

                    strQry.AppendLine(" WHEN CURRENT_YEAR_ACCUM_TABLE.CURRENT_YEAR_ACCUM_DAYS + LS.NORM_PAID_PER_PERIOD + 0.15 >= LS.NORM_PAID_DAYS");
                    strQry.AppendLine(" THEN ROUND((ISNULL(PREV_YEAR_ACCUM_TABLE.PREV_YEAR_ACCUM_DAYS,0) + LS.NORM_PAID_DAYS - ISNULL(LEAVE_TO_BE_PROCESSED_TABLE.SUM_LEAVE_DAYS_DECIMAL,0)) * (PC.SALARY_MINUTES_PAID_PER_DAY / 60),2)");

                    strQry.AppendLine(" ELSE ROUND((ISNULL(PREV_YEAR_ACCUM_TABLE.PREV_YEAR_ACCUM_DAYS,0) + ISNULL(CURRENT_YEAR_ACCUM_TABLE.CURRENT_YEAR_ACCUM_DAYS,0) - ISNULL(LEAVE_TO_BE_PROCESSED_TABLE.SUM_LEAVE_DAYS_DECIMAL,0)) * (PC.SALARY_MINUTES_PAID_PER_DAY / 60),2)");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(",DAY_DECIMAL_OTHER_VALUE = ");

                    strQry.AppendLine(" CASE");

                    //Employee Changed Leave Category and Already ExceedS Total for Year
                    strQry.AppendLine(" WHEN CURRENT_YEAR_ACCUM_TABLE.CURRENT_YEAR_ACCUM_DAYS >= LS.NORM_PAID_DAYS");
                    strQry.AppendLine(" THEN ROUND((ISNULL(PREV_YEAR_ACCUM_TABLE.PREV_YEAR_ACCUM_DAYS,0) + CURRENT_YEAR_ACCUM_TABLE.CURRENT_YEAR_ACCUM_DAYS - ISNULL(LEAVE_TO_BE_PROCESSED_TABLE.SUM_LEAVE_DAYS_DECIMAL,0)),2)");

                    strQry.AppendLine(" WHEN CURRENT_YEAR_ACCUM_TABLE.CURRENT_YEAR_ACCUM_DAYS + LS.NORM_PAID_PER_PERIOD + 0.15 >= LS.NORM_PAID_DAYS");
                    strQry.AppendLine(" THEN ROUND((ISNULL(PREV_YEAR_ACCUM_TABLE.PREV_YEAR_ACCUM_DAYS,0) + LS.NORM_PAID_DAYS - ISNULL(LEAVE_TO_BE_PROCESSED_TABLE.SUM_LEAVE_DAYS_DECIMAL,0)),2)");

                    strQry.AppendLine(" ELSE ROUND((LS.NORM_PAID_PER_PERIOD + ISNULL(PREV_YEAR_ACCUM_TABLE.PREV_YEAR_ACCUM_DAYS,0) + ISNULL(CURRENT_YEAR_ACCUM_TABLE.CURRENT_YEAR_ACCUM_DAYS,0) - ISNULL(LEAVE_TO_BE_PROCESSED_TABLE.SUM_LEAVE_DAYS_DECIMAL,0)),2)");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(",DAY_DECIMAL_OTHER_VALUE_ZERO = ");

                    strQry.AppendLine(" CASE");

                    //Employee Changed Leave Category and Already ExceedS Total for Year
                    strQry.AppendLine(" WHEN CURRENT_YEAR_ACCUM_TABLE.CURRENT_YEAR_ACCUM_DAYS >= LS.NORM_PAID_DAYS");
                    strQry.AppendLine(" THEN ROUND((ISNULL(PREV_YEAR_ACCUM_TABLE.PREV_YEAR_ACCUM_DAYS,0) + CURRENT_YEAR_ACCUM_TABLE.CURRENT_YEAR_ACCUM_DAYS - ISNULL(LEAVE_TO_BE_PROCESSED_TABLE.SUM_LEAVE_DAYS_DECIMAL,0)),2)");

                    //NB - There coulc be a bug Here (No Data to Test)
                    strQry.AppendLine(" WHEN CURRENT_YEAR_ACCUM_TABLE.CURRENT_YEAR_ACCUM_DAYS + 0.15 >= LS.NORM_PAID_DAYS");
                    strQry.AppendLine(" THEN ROUND((ISNULL(PREV_YEAR_ACCUM_TABLE.PREV_YEAR_ACCUM_DAYS,0) + LS.NORM_PAID_DAYS - ISNULL(LEAVE_TO_BE_PROCESSED_TABLE.SUM_LEAVE_DAYS_DECIMAL,0)),2)");

                    strQry.AppendLine(" ELSE ROUND((ISNULL(PREV_YEAR_ACCUM_TABLE.PREV_YEAR_ACCUM_DAYS,0) + ISNULL(CURRENT_YEAR_ACCUM_TABLE.CURRENT_YEAR_ACCUM_DAYS,0) - ISNULL(LEAVE_TO_BE_PROCESSED_TABLE.SUM_LEAVE_DAYS_DECIMAL,0)),2)");

                    strQry.AppendLine(" END ");

                    //2013-09-26 - EMPLOYEE_EARNING_BATCH_TEMP (Add Batch Earning for NT/OT1/OT2/OT3 
                    strQry.AppendLine(",0");

                    //ELR 2015-03-18
                    strQry.AppendLine(",'F' AS EARNING_TYPE_IND");

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
                    strQry.AppendLine(" ON E.COMPANY_NO = PC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                    //ELR - 2014-05-24
                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EE");
                    strQry.AppendLine(" ON E.COMPANY_NO = EE.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EE.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EE.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EE.EARNING_TYPE_IND <> 'M' ");
                    strQry.AppendLine(" AND EE.DATETIME_DELETE_RECORD IS NULL ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");
                    //Pay Category That Applies to Leave
                    strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT LS");
                    strQry.AppendLine(" ON E.COMPANY_NO = LS.COMPANY_NO");
                    strQry.AppendLine(" AND E.LEAVE_SHIFT_NO = LS.LEAVE_SHIFT_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = LS.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND LS.DATETIME_DELETE_RECORD IS NULL");

                    //Normal Leave Before beginning of this Fiscal Year(Processed)
                    strQry.AppendLine(" LEFT JOIN ");

                    strQry.AppendLine("(SELECT ");
                    strQry.AppendLine(" EMPLOYEE_NO");
                    strQry.AppendLine(",SUM(ROUND(LEAVE_ACCUM_DAYS,2)) - SUM(ROUND(LEAVE_PAID_DAYS,2)) AS PREV_YEAR_ACCUM_DAYS");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY L");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND EARNING_NO = 200");
                    strQry.AppendLine(" AND (L.PAY_PERIOD_DATE < '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'");
                    //Open Balance For Previous Year
                    strQry.AppendLine(" OR L.PROCESS_NO = 100)");

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" EMPLOYEE_NO) AS PREV_YEAR_ACCUM_TABLE");

                    strQry.AppendLine(" ON E.EMPLOYEE_NO = PREV_YEAR_ACCUM_TABLE.EMPLOYEE_NO");

                    //Normal Leave After beginning of this Fiscal Year (Processed)
                    strQry.AppendLine(" LEFT JOIN ");

                    strQry.AppendLine("(SELECT ");
                    strQry.AppendLine(" EMPLOYEE_NO");
                    strQry.AppendLine(",SUM(ROUND(LEAVE_ACCUM_DAYS,2)) - SUM(ROUND(LEAVE_PAID_DAYS,2)) AS CURRENT_YEAR_ACCUM_DAYS");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND EARNING_NO = 200");

                    //Accumulated Portion / Take-On
                    strQry.AppendLine(" AND PROCESS_NO in (98,99,0)");
                    strQry.AppendLine(" AND PAY_PERIOD_DATE >= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" EMPLOYEE_NO) AS CURRENT_YEAR_ACCUM_TABLE");

                    strQry.AppendLine(" ON E.EMPLOYEE_NO = CURRENT_YEAR_ACCUM_TABLE.EMPLOYEE_NO");

                    //Normal Leave to be Processed this Run
                    strQry.AppendLine(" LEFT JOIN ");

                    strQry.AppendLine("(SELECT ");
                    strQry.AppendLine(" LC.EMPLOYEE_NO");
                    strQry.AppendLine(",SUM(ROUND(LC.LEAVE_DAYS_DECIMAL,2)) AS SUM_LEAVE_DAYS_DECIMAL");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT LC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON LC.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND LC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND LC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");
                    //Pay Category That Applies to Leave
                    strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");

                    strQry.AppendLine(" WHERE LC.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND LC.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND LC.EARNING_NO = 200");
                    //To be Processed
                    strQry.AppendLine(" AND LC.PROCESS_NO = 0");

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" LC.EMPLOYEE_NO) AS LEAVE_TO_BE_PROCESSED_TABLE");

                    strQry.AppendLine(" ON E.EMPLOYEE_NO = LEAVE_TO_BE_PROCESSED_TABLE.EMPLOYEE_NO");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_LAST_RUNDATE < '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",PC.SALARY_MINUTES_PAID_PER_DAY");
                    strQry.AppendLine(",LS.NORM_PAID_DAYS");
                    strQry.AppendLine(",LS.NORM_PAID_PER_PERIOD");
                    strQry.AppendLine(",LEAVE_TO_BE_PROCESSED_TABLE.SUM_LEAVE_DAYS_DECIMAL");
                    strQry.AppendLine(",CURRENT_YEAR_ACCUM_TABLE.CURRENT_YEAR_ACCUM_DAYS");
                    strQry.AppendLine(",PREV_YEAR_ACCUM_TABLE.PREV_YEAR_ACCUM_DAYS");
                    //ELR 2014-05-24
                    strQry.AppendLine(",EE.EARNING_TYPE_IND");

                    //Insert NON Linked EMPLOYEE_EARNING For Current Year. NB Currently Includes Tax Directives
                    strQry.AppendLine(" UNION ");

                    strQry.AppendLine(" SELECT DISTINCT ");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",'P'");
                    strQry.AppendLine(",EN.EARNING_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");

                    //2016-01-08 HOURS_DECIMAL_OTHER_VALUE_ZERO
                    strQry.AppendLine(",0");

                    //2012-10-22 DAY_DECIMAL_OTHER_VALUE
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    //2013-09-26 - EMPLOYEE_EARNING_BATCH_TEMP (Add Batch Earning for NT/OT1/OT2/OT3 
                    strQry.AppendLine(",0");

                    //ELR 2014-05-24
                    strQry.AppendLine(",EE.EARNING_TYPE_IND");

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN  InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EN");
                    strQry.AppendLine(" ON E.COMPANY_NO = EN.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
                    strQry.AppendLine(" ON E.COMPANY_NO = EEH.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EEH.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EN.EARNING_NO = EEH.EARNING_NO ");
                    //Exclude Default Earnings and Leave
                    strQry.AppendLine(" AND EN.EARNING_NO > 9 ");
                    strQry.AppendLine(" AND EN.EARNING_NO < 200 ");
                    //Could also be a Take-On
                    //strQry.AppendLine(" AND EEH.RUN_TYPE = 'P'");
                    //Could be From Another Pay Category
                    //strQry.AppendLine(" AND EEH.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE");
                    //If Previous Fiscal Year then No C/f of History Values (Will be Zero)
                    strQry.AppendLine(" AND EEH.PAY_PERIOD_DATE >= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'");

                    //NO EMPLOYEE_EARNING Link
                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EE ");
                    strQry.AppendLine(" ON E.COMPANY_NO = EE.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EE.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EE.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EN.EARNING_NO = EE.EARNING_NO ");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_LAST_RUNDATE < '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");
                    //NO EMPLOYEE_EARNING Link
                    strQry.AppendLine(" AND EE.EARNING_NO IS NULL ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    //Carry Forward Values Forward from History For Current Year Otherwise Zero - Linked
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",RUN_TYPE");
                    strQry.AppendLine(",DEDUCTION_NO");
                    strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
                    strQry.AppendLine(",RUN_NO");
                    strQry.AppendLine(",TOTAL");
                    strQry.AppendLine(",TOTAL_ORIGINAL)");

                    strQry.AppendLine(" SELECT DISTINCT");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",'P'");
                    strQry.AppendLine(",ED.DEDUCTION_NO");
                    strQry.AppendLine(",ED.DEDUCTION_SUB_ACCOUNT_NO");
                    strQry.AppendLine("," + intRunNo.ToString());
                    //2013-09-26
                    strQry.AppendLine(",ISNULL(EDBT.AMOUNT,0)");
                    strQry.AppendLine(",ISNULL(EDBT.AMOUNT,0)");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
                    //Link To Default Pay Category
                    strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ED");
                    strQry.AppendLine(" ON E.COMPANY_NO = ED.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = ED.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = ED.EMPLOYEE_NO ");

                    //User To Enter
                    strQry.AppendLine(" AND (ED.DEDUCTION_TYPE_IND = 'U'");
                    strQry.AppendLine(" OR ED.DEDUCTION_NO IN (1,2))");

                    strQry.AppendLine(" AND ED.DATETIME_DELETE_RECORD IS NULL");

                    //2013-09-26
                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_BATCH_TEMP EDBT");
                    strQry.AppendLine(" ON E.COMPANY_NO = EDBT.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EDBT.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND ED.DEDUCTION_NO = EDBT.DEDUCTION_NO ");
                    strQry.AppendLine(" AND ED.DEDUCTION_SUB_ACCOUNT_NO = EDBT.DEDUCTION_SUB_ACCOUNT_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EDBT.EMPLOYEE_NO ");
                    //Next Run
                    strQry.AppendLine(" AND EDBT.PROCESS_NO = 0 ");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_LAST_RUNDATE < '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                    //errol testing
                    strQry.AppendLine(" UNION ");

                    strQry.AppendLine(" SELECT DISTINCT");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",'P'");
                    strQry.AppendLine(",ED.DEDUCTION_NO");
                    strQry.AppendLine(",ED.DEDUCTION_SUB_ACCOUNT_NO");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.AppendLine(",TOTAL = ");

                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN D.DEDUCTION_LOAN_TYPE_IND = 'Y' AND ED.DEDUCTION_VALUE > ISNULL(TEMP1_TABLE.TOTAL_LOAN,0) THEN ");

                    strQry.AppendLine(" ISNULL(TEMP1_TABLE.TOTAL_LOAN,0) ");

                    strQry.AppendLine(" ELSE ");

                    strQry.AppendLine(" ED.DEDUCTION_VALUE");

                    strQry.AppendLine(" END ");


                    strQry.AppendLine(",TOTAL_ORIGINAL = ");

                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN D.DEDUCTION_LOAN_TYPE_IND = 'Y' AND ED.DEDUCTION_VALUE > ISNULL(TEMP1_TABLE.TOTAL_LOAN,0) THEN ");

                    strQry.AppendLine(" ISNULL(TEMP1_TABLE.TOTAL_LOAN,0) ");

                    strQry.AppendLine(" ELSE ");

                    strQry.AppendLine(" ED.DEDUCTION_VALUE");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.DEDUCTION D");
                    strQry.AppendLine(" ON E.COMPANY_NO = D.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = D.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
                    //Link To Default Pay Category
                    strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ED");
                    strQry.AppendLine(" ON E.COMPANY_NO = ED.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = ED.EMPLOYEE_NO ");

                    strQry.AppendLine(" AND D.DEDUCTION_NO = ED.DEDUCTION_NO ");
                    strQry.AppendLine(" AND D.DEDUCTION_SUB_ACCOUNT_NO = ED.DEDUCTION_SUB_ACCOUNT_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = ED.PAY_CATEGORY_TYPE");
                    //Fixed Value
                    strQry.AppendLine(" AND ED.DEDUCTION_TYPE_IND = 'F'");
                    strQry.AppendLine(" AND ED.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" LEFT JOIN ");
                    strQry.AppendLine("(SELECT ");
                    strQry.AppendLine(" L.COMPANY_NO");
                    strQry.AppendLine(",L.EMPLOYEE_NO");
                    strQry.AppendLine(",L.DEDUCTION_NO");
                    strQry.AppendLine(",L.DEDUCTION_SUB_ACCOUNT_NO");
                    strQry.AppendLine(",L.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",SUM(L.LOAN_AMOUNT_PAID - L.LOAN_AMOUNT_RECEIVED) AS TOTAL_LOAN");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LOANS L");

                    strQry.AppendLine(" WHERE L.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND L.PAY_CATEGORY_TYPE = 'S'");

                    strQry.AppendLine(" AND (NOT L.LOAN_PROCESSED_DATE IS NULL");
                    strQry.AppendLine(" OR (L.LOAN_PROCESSED_DATE IS NULL");
                    strQry.AppendLine(" AND L.PROCESS_NO = 0))");

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" L.COMPANY_NO");
                    strQry.AppendLine(",L.EMPLOYEE_NO");
                    strQry.AppendLine(",L.DEDUCTION_NO");
                    strQry.AppendLine(",L.DEDUCTION_SUB_ACCOUNT_NO");
                    strQry.AppendLine(",L.PAY_CATEGORY_TYPE) AS TEMP1_TABLE");

                    strQry.AppendLine(" ON E.COMPANY_NO = TEMP1_TABLE.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = TEMP1_TABLE.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND D.DEDUCTION_NO = TEMP1_TABLE.DEDUCTION_NO ");
                    strQry.AppendLine(" AND D.DEDUCTION_SUB_ACCOUNT_NO = TEMP1_TABLE.DEDUCTION_SUB_ACCOUNT_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = TEMP1_TABLE.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_LAST_RUNDATE < '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                    //Percentage
                    strQry.AppendLine(" UNION ");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" TEMP1_TABLE.COMPANY_NO");
                    strQry.AppendLine(",TEMP1_TABLE.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",TEMP1_TABLE.EMPLOYEE_NO");
                    strQry.AppendLine(",'P'");
                    strQry.AppendLine(",TEMP1_TABLE.DEDUCTION_NO");
                    strQry.AppendLine(",TEMP1_TABLE.DEDUCTION_SUB_ACCOUNT_NO");
                    strQry.AppendLine("," + intRunNo.ToString());
                    strQry.AppendLine(",SUM(TEMP1_TABLE.DEDUCTION_VALUE)");
                    strQry.AppendLine(",SUM(TEMP1_TABLE.DEDUCTION_VALUE)");

                    strQry.AppendLine(" FROM ");

                    strQry.AppendLine("(SELECT ");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",ED.DEDUCTION_NO");
                    strQry.AppendLine(",ED.DEDUCTION_SUB_ACCOUNT_NO");
                    strQry.AppendLine(",DEDUCTION_VALUE = ROUND((ED.DEDUCTION_VALUE / 100) * EEC.TOTAL,2) ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ED");
                    strQry.AppendLine(" ON E.COMPANY_NO = ED.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = ED.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = ED.EMPLOYEE_NO ");
                    //Percentage
                    strQry.AppendLine(" AND ED.DEDUCTION_TYPE_IND = 'P'");
                    //Not Tax/UIF
                    strQry.AppendLine(" AND NOT ED.DEDUCTION_NO IN (1,2)");
                    strQry.AppendLine(" AND ED.DATETIME_DELETE_RECORD IS NULL");

                    //ELR - 2014-08-30 (Fix Bonus) 
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EEC.COMPANY_NO ");
                    strQry.AppendLine(" AND EEC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EEC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EEC.RUN_TYPE = 'P'");
                    strQry.AppendLine(" AND EEC.RUN_NO = " + intRunNo);

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE EDEP");
                    strQry.AppendLine(" ON E.COMPANY_NO = EDEP.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EDEP.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EDEP.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND ED.DEDUCTION_NO = EDEP.DEDUCTION_NO ");
                    strQry.AppendLine(" AND ED.DEDUCTION_SUB_ACCOUNT_NO = EDEP.DEDUCTION_SUB_ACCOUNT_NO ");
                    strQry.AppendLine(" AND EEC.EARNING_NO = EDEP.EARNING_NO ");
                    strQry.AppendLine(" AND ED.PAY_CATEGORY_TYPE = EDEP.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND EDEP.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_LAST_RUNDATE < '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "') AS TEMP1_TABLE");

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" TEMP1_TABLE.COMPANY_NO");
                    strQry.AppendLine(",TEMP1_TABLE.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",TEMP1_TABLE.EMPLOYEE_NO");
                    strQry.AppendLine(",TEMP1_TABLE.DEDUCTION_NO");
                    strQry.AppendLine(",TEMP1_TABLE.DEDUCTION_SUB_ACCOUNT_NO");

                    //Carry Values Forward when Deduction is Delinked
                    strQry.AppendLine(" UNION ");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EDH.COMPANY_NO");
                    strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EDH.EMPLOYEE_NO");
                    strQry.AppendLine(",'P'");
                    strQry.AppendLine(",EDH.DEDUCTION_NO");
                    strQry.AppendLine(",EDH.DEDUCTION_SUB_ACCOUNT_NO");
                    strQry.AppendLine("," + intRunNo.ToString());
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY EDH");
                    strQry.AppendLine(" ON E.COMPANY_NO = EDH.COMPANY_NO");
                    strQry.AppendLine(" AND EDH.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND EDH.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");

                    strQry.AppendLine(" AND EDH.EMPLOYEE_NO = E.EMPLOYEE_NO");
                    //If Previous Fiscal Year then No C/f of History Values (Will be Zero)
                    strQry.AppendLine(" AND EDH.PAY_PERIOD_DATE >= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'");
                    //Removed To Cater for Take-On
                    //strQry.AppendLine(" AND EDH.RUN_TYPE = 'P'");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_LAST_RUNDATE < '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND STR(E.EMPLOYEE_NO) + STR(EDH.DEDUCTION_NO) + STR(EDH.DEDUCTION_SUB_ACCOUNT_NO) + CONVERT(CHAR,EDH.PAY_PERIOD_DATE) IN ");
                    strQry.AppendLine("(SELECT STR(EMPLOYEE_NO) + STR(DEDUCTION_NO) + STR(DEDUCTION_SUB_ACCOUNT_NO) + CONVERT(CHAR,MAX(PAY_PERIOD_DATE)) ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY ");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                    //Removed To Cater for Take-On
                    //strQry.AppendLine(" AND RUN_TYPE = 'P'");
                    //If Previous Fiscal Year then No C/f of History Values (Will be Zero)
                    strQry.AppendLine(" AND PAY_PERIOD_DATE >= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'");
                    //Added
                    strQry.AppendLine(" AND TOTAL <> 0 ");

                    strQry.AppendLine(" GROUP BY ");
                    strQry.AppendLine(" EMPLOYEE_NO ");
                    strQry.AppendLine(",DEDUCTION_NO ");
                    strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO)");

                    //Does NOT have a Current Link
                    strQry.AppendLine(" AND STR(E.EMPLOYEE_NO) + STR(EDH.DEDUCTION_NO) + STR(EDH.DEDUCTION_SUB_ACCOUNT_NO) NOT IN ");
                    strQry.AppendLine("(SELECT STR(EMPLOYEE_NO) + STR(DEDUCTION_NO) + STR(DEDUCTION_SUB_ACCOUNT_NO) ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL)");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    //Carry Forward Values Forward from History For Current Year Otherwise Zero - Linked
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE_CURRENT");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",DEDUCTION_NO");
                    strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
                    strQry.AppendLine(",EARNING_NO");
                    strQry.AppendLine(",RUN_NO)");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",EDEP.DEDUCTION_NO");
                    strQry.AppendLine(",EDEP.DEDUCTION_SUB_ACCOUNT_NO");
                    strQry.AppendLine(",EDEP.EARNING_NO");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
                    //Link To Default Pay Category
                    strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE EDEP");
                    strQry.AppendLine(" ON E.COMPANY_NO = EDEP.COMPANY_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EDEP.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EDEP.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EDEP.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_LAST_RUNDATE < '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                }

                //DataView DataViewEmployeeLeaveTotal;
                DataView DataViewEmployeeYTDTotals;

                DataView DataViewEmployeeDeduction;
                DataView DataViewCommission;
                DataView DataViewBonus;
                DataView DataViewTaxSpreadSheet;

                DataRowView drvDataRowView;

                string strPayUIFInd = "";
                string strMedicalAidInd = "";

                double dblWageMonth = 0;
                double dblCommission = 0;

                double dblEmployeePortionOfYear = 0;
                double dblAgeAtTaxYearEnd = 0;
                double dblEmployeeAnnualisedFactor = 0;
                double dblDeductionFinalTotal = 0;

                double dblUIFAmount = 0;
                double dblEarningsCurrent = 0;
                double dblIncomeEarningsTotal = 0;
                double dblTaxEarningsYTD = 0;
                double dblTaxEarningsOtherYTD = 0;

                double dblTaxYTD = 0;

                double dblTaxSpreadSheetValue = 0;

                double dblTaxCalculatedRun = 0;

                double dblPensionArrearYTD = 0;
                double dblRetireAnnuityArrearYTD = 0;

                double dblEmployeeDaysWorked = 0;

                //Values Returned From Tax Module for Show on Screen
                double[] dblRetirementAnnuityAmount = new double[12];
                double dblRetirementAnnuityTotal = 0;

                double[] dblPensionFundAmount = new double[12];
                double dblPensionFundTotal = 0;

                double[] dblTaxTotal = new double[11];

                double[] dblUifTotal = new double[6];

                double dblTotalDeductions = 0;

                int intTaxTableRow = -1;
                int intEarningsTaxTableRow = -1;
                double dblTotalNormalEarnings = 0;
                double dblTotalNormalEarningsAnnualised = 0;
                double dblTotalEarnedAccumAnnualInitial = 0;

                int intTaxSpreadSheetRow = -1;
                int intMedicalAidNumberDependents = 0;
                int intIRP5 = -1;

                object[] objFindTaxSpreadSheet = new object[3];

                string strPayCategoryIn = "";

                //Create SQL For Months of EMPLOYEE_TAX_SPREADHEET
                strSelectAddonQry = "";
                dtTempDateTime = new DateTime(2007, 3, 1);

                for (int intRowMonth = dtTempDateTime.Month; intRowMonth < 20; intRowMonth++)
                {
                    if (intRowMonth == parCurrentDateTime.Month)
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

                for (int intCount = 0; intCount < parDataSet.Tables["Upload"].Rows.Count; intCount++)
                {
                    if (intCount == 0)
                    {
                        strPayCategoryIn = parDataSet.Tables["Upload"].Rows[intCount]["PAY_CATEGORY_NO"].ToString();
                    }
                    else
                    {
                        strPayCategoryIn += "," + parDataSet.Tables["Upload"].Rows[intCount]["PAY_CATEGORY_NO"].ToString();
                    }
                }

                //Currently Active Deductions
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",ED.EMPLOYEE_NO");
                strQry.AppendLine(",ED.DEDUCTION_NO");
                strQry.AppendLine(",ED.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",ED.DEDUCTION_TYPE_IND");
                strQry.AppendLine(",ED.DEDUCTION_VALUE");
                strQry.AppendLine(",ISNULL(SUM(EDH.TOTAL),0) AS  TOTAL_YTD_BF");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ED ");

                //2017-01-24 (Employee Not Closed)
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON ED.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND ED.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ED.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");
                strQry.AppendLine(" ON ED.COMPANY_NO = EPCC.COMPANY_NO");
                strQry.AppendLine(" AND ED.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE  = 'S'");
                strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P' ");
                strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y' ");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY EDH");
                strQry.AppendLine(" ON ED.COMPANY_NO = EDH.COMPANY_NO ");
                strQry.AppendLine(" AND ED.EMPLOYEE_NO = EDH.EMPLOYEE_NO");
                strQry.AppendLine(" AND ED.DEDUCTION_NO = EDH.DEDUCTION_NO");
                strQry.AppendLine(" AND ED.DEDUCTION_SUB_ACCOUNT_NO = EDH.DEDUCTION_SUB_ACCOUNT_NO");
                //If Previous Fiscal Year then No C/f of History Values (Will be Zero)
                strQry.AppendLine(" AND EDH.PAY_PERIOD_DATE >= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'");
                //RUN_TYPE = T/P
                strQry.AppendLine(" WHERE ED.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND ED.PAY_CATEGORY_TYPE  = 'S'");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",ED.EMPLOYEE_NO");
                strQry.AppendLine(",ED.DEDUCTION_NO");
                strQry.AppendLine(",ED.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",ED.DEDUCTION_TYPE_IND");
                strQry.AppendLine(",ED.DEDUCTION_VALUE");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",ED.EMPLOYEE_NO");
                strQry.AppendLine(",ED.DEDUCTION_NO");
                strQry.AppendLine(",ED.DEDUCTION_SUB_ACCOUNT_NO");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeDeduction", parint64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" SELECT DISTINCT ");
                strQry.AppendLine(" E.EMPLOYEE_NO");
                strQry.AppendLine(",E.EMPLOYEE_LAST_RUNDATE");
                strQry.AppendLine(",E.TAX_TYPE_IND");
                strQry.AppendLine(",E.TAX_DIRECTIVE_PERCENTAGE");
                strQry.AppendLine(",E.EMPLOYEE_TAX_STARTDATE");
                strQry.AppendLine(",E.EMPLOYEE_BIRTHDATE");
                //strQry.AppendLine(",E.EMPLOYEE_NUMBER_CHEQUES");
                strQry.AppendLine(",E.NUMBER_MEDICAL_AID_DEPENDENTS");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC ");
                strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE  = 'S'");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" E.EMPLOYEE_NO");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parint64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EPCC.EMPLOYEE_NO");

                //2015-05-04
                strQry.AppendLine(",SUM(ISNULL(TEMP1_TABLE.EARNING_CURRENT,0)) AS EARNING_CURRENT");

                strQry.AppendLine(",SUM(ISNULL(TEMP1_TABLE.TOTAL,0)) AS NORMAL_EARNINGS");
                strQry.AppendLine(",SUM(ISNULL(TEMP2_TABLE.TOTAL,0)) AS OTHER_EARNINGS");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC ");

                //Only Normal (Exclude Other and Directives)
                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(SELECT ");
                strQry.AppendLine(" EEC1.COMPANY_NO");
                strQry.AppendLine(",EEC1.PAY_CATEGORY_NO");
                strQry.AppendLine(",EEC1.EMPLOYEE_NO");

                //2015-05-04
                strQry.AppendLine(",SUM(EEC1.TOTAL) AS EARNING_CURRENT");

                //2017-02-20
                strQry.AppendLine(",SUM(EEC1.TOTAL) + ISNULL(TEMP1_HISTORY_TABLE.TOTAL,0) + + ISNULL(WAGES_HISTORY_TABLE.TOTAL,0) AS TOTAL");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC1 ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EN ");

                strQry.AppendLine(" ON EEC1.COMPANY_NO = EN.COMPANY_NO");
                strQry.AppendLine(" AND EEC1.EARNING_NO = EN.EARNING_NO");
                strQry.AppendLine(" AND EEC1.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE");
                //NOT Bonus
                strQry.AppendLine(" AND EN.EARNING_NO <> 7");
                strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" LEFT JOIN ");

                strQry.AppendLine("(SELECT ");
                strQry.AppendLine(" EEH1.COMPANY_NO");
                strQry.AppendLine(",EEH1.PAY_CATEGORY_NO");
                strQry.AppendLine(",EEH1.EMPLOYEE_NO");
                strQry.AppendLine(",SUM(EEH1.TOTAL) AS TOTAL");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH1 ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EN ");

                strQry.AppendLine(" ON EEH1.COMPANY_NO = EN.COMPANY_NO");
                strQry.AppendLine(" AND EEH1.EARNING_NO = EN.EARNING_NO");
                strQry.AppendLine(" AND EEH1.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE");
                //NOT Bonus
                strQry.AppendLine(" AND EN.EARNING_NO <> 7");
                strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" WHERE EEH1.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND EEH1.PAY_CATEGORY_TYPE = 'S'");

                //Includes Take-On Balances
                //strQry.AppendLine(" AND RUN_TYPE = 'P'");

                strQry.AppendLine(" AND EEH1.PAY_PERIOD_DATE >= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" EEH1.COMPANY_NO");
                strQry.AppendLine(",EEH1.PAY_CATEGORY_NO");
                strQry.AppendLine(",EEH1.EMPLOYEE_NO) AS TEMP1_HISTORY_TABLE");

                strQry.AppendLine(" ON EEC1.COMPANY_NO = TEMP1_HISTORY_TABLE.COMPANY_NO");
                strQry.AppendLine(" AND EEC1.PAY_CATEGORY_NO = TEMP1_HISTORY_TABLE.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EEC1.EMPLOYEE_NO = TEMP1_HISTORY_TABLE.EMPLOYEE_NO");

                //2017-02-20 - Start
                //Add Employee History From when was Wages
                strQry.AppendLine(" LEFT JOIN ");

                strQry.AppendLine("(SELECT ");
                strQry.AppendLine(" EEH_Old.COMPANY_NO");
                strQry.AppendLine(",EEH_Old.EMPLOYEE_NO");
                strQry.AppendLine(",SUM(EEH_Old.TOTAL) AS TOTAL");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH_Old ");

                strQry.AppendLine(" WHERE EEH_Old.COMPANY_NO = " + parint64CompanyNo);
                //2017-02-20 Employee Changes Type From Wages
                strQry.AppendLine(" AND EEH_Old.PAY_CATEGORY_TYPE = 'W'");
                //Not Bonus
                strQry.AppendLine(" AND EEH_Old.EARNING_NO <> 7");

                strQry.AppendLine(" AND EEH_Old.PAY_PERIOD_DATE >= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'");

                strQry.AppendLine(" GROUP BY");
                strQry.AppendLine(" EEH_Old.COMPANY_NO");
                strQry.AppendLine(",EEH_Old.EMPLOYEE_NO) AS WAGES_HISTORY_TABLE");

                strQry.AppendLine(" ON EEC1.COMPANY_NO = WAGES_HISTORY_TABLE.COMPANY_NO");
                strQry.AppendLine(" AND EEC1.EMPLOYEE_NO = WAGES_HISTORY_TABLE.EMPLOYEE_NO");
                //2017-02-20 - End

                strQry.AppendLine(" WHERE EEC1.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND EEC1.PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND EEC1.RUN_TYPE = 'P'");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" EEC1.COMPANY_NO");
                strQry.AppendLine(",EEC1.PAY_CATEGORY_NO");
                strQry.AppendLine(",EEC1.EMPLOYEE_NO");
                strQry.AppendLine(",TEMP1_HISTORY_TABLE.TOTAL");
                strQry.AppendLine(",WAGES_HISTORY_TABLE.TOTAL) AS TEMP1_TABLE");

                strQry.AppendLine(" ON EPCC.COMPANY_NO = TEMP1_TABLE.COMPANY_NO");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = TEMP1_TABLE.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPCC.EMPLOYEE_NO = TEMP1_TABLE.EMPLOYEE_NO");

                //Only Other and Directives

                strQry.AppendLine(" LEFT JOIN ");

                strQry.AppendLine("(SELECT ");
                strQry.AppendLine(" EEC1.COMPANY_NO");
                strQry.AppendLine(",EEC1.PAY_CATEGORY_NO");
                strQry.AppendLine(",EEC1.EMPLOYEE_NO");

                //2017-02-20
                strQry.AppendLine(",SUM(EEC1.TOTAL) + ISNULL(TEMP2_HISTORY_TABLE.TOTAL,0) + ISNULL(WAGES_HISTORY_TABLE.TOTAL,0) AS TOTAL");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC1 ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EN ");

                strQry.AppendLine(" ON EEC1.COMPANY_NO = EN.COMPANY_NO");
                strQry.AppendLine(" AND EEC1.EARNING_NO = EN.EARNING_NO");
                strQry.AppendLine(" AND EEC1.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE");
                //Bonus
                strQry.AppendLine(" AND EN.EARNING_NO = 7");
                strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" LEFT JOIN ");

                strQry.AppendLine("(SELECT ");
                strQry.AppendLine(" EEH1.COMPANY_NO");
                strQry.AppendLine(",EEH1.PAY_CATEGORY_NO");
                strQry.AppendLine(",EEH1.EMPLOYEE_NO");
                strQry.AppendLine(",SUM(EEH1.TOTAL) AS TOTAL");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH1 ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EN ");

                strQry.AppendLine(" ON EEH1.COMPANY_NO = EN.COMPANY_NO");
                strQry.AppendLine(" AND EEH1.EARNING_NO = EN.EARNING_NO");
                strQry.AppendLine(" AND EEH1.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE");
                //Bonus
                strQry.AppendLine(" AND EN.EARNING_NO = 7");
                strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" WHERE EEH1.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND EEH1.PAY_CATEGORY_TYPE = 'S'");

                //Includes Take-On Balances
                //strQry.AppendLine(" AND RUN_TYPE = 'P'");

                strQry.AppendLine(" AND EEH1.PAY_PERIOD_DATE >= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" EEH1.COMPANY_NO");
                strQry.AppendLine(",EEH1.PAY_CATEGORY_NO");
                strQry.AppendLine(",EEH1.EMPLOYEE_NO) AS TEMP2_HISTORY_TABLE");

                strQry.AppendLine(" ON EEC1.COMPANY_NO = TEMP2_HISTORY_TABLE.COMPANY_NO");
                strQry.AppendLine(" AND EEC1.PAY_CATEGORY_NO = TEMP2_HISTORY_TABLE.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EEC1.EMPLOYEE_NO = TEMP2_HISTORY_TABLE.EMPLOYEE_NO");

                //2017-02-20 - Start
                //Add Employee History From when was Wages
                strQry.AppendLine(" LEFT JOIN ");

                strQry.AppendLine("(SELECT ");
                strQry.AppendLine(" EEH_Old.COMPANY_NO");
                strQry.AppendLine(",EEH_Old.EMPLOYEE_NO");
                strQry.AppendLine(",SUM(EEH_Old.TOTAL) AS TOTAL");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH_Old ");

                strQry.AppendLine(" WHERE EEH_Old.COMPANY_NO = " + parint64CompanyNo);
                //2017-02-20 Employee Changes Type From Wages
                strQry.AppendLine(" AND EEH_Old.PAY_CATEGORY_TYPE = 'W'");
                //Bonus
                strQry.AppendLine(" AND EEH_Old.EARNING_NO = 7");

                strQry.AppendLine(" AND EEH_Old.PAY_PERIOD_DATE >= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'");

                strQry.AppendLine(" GROUP BY");
                strQry.AppendLine(" EEH_Old.COMPANY_NO");
                strQry.AppendLine(",EEH_Old.EMPLOYEE_NO) AS WAGES_HISTORY_TABLE");

                strQry.AppendLine(" ON EEC1.COMPANY_NO = WAGES_HISTORY_TABLE.COMPANY_NO");
                strQry.AppendLine(" AND EEC1.EMPLOYEE_NO = WAGES_HISTORY_TABLE.EMPLOYEE_NO");
                //2017-02-20 - End

                strQry.AppendLine(" WHERE EEC1.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND EEC1.PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND EEC1.RUN_TYPE = 'P'");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" EEC1.COMPANY_NO");
                strQry.AppendLine(",EEC1.PAY_CATEGORY_NO");
                strQry.AppendLine(",EEC1.EMPLOYEE_NO");
                strQry.AppendLine(",TEMP2_HISTORY_TABLE.TOTAL");
                strQry.AppendLine(",WAGES_HISTORY_TABLE.TOTAL) AS TEMP2_TABLE");

                strQry.AppendLine(" ON EPCC.COMPANY_NO = TEMP2_TABLE.COMPANY_NO");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = TEMP2_TABLE.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPCC.EMPLOYEE_NO = TEMP2_TABLE.EMPLOYEE_NO");

                strQry.AppendLine(" WHERE EPCC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND EPCC.RUN_TYPE = 'P'");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EPCC.EMPLOYEE_NO");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EPCC.EMPLOYEE_NO");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeYTDTotals", parint64CompanyNo);

                //2014-03-21 - Commission Used to Subtract from UIF
                strQry.Clear();
                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" EEC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EEC.EMPLOYEE_NO");
                strQry.AppendLine(",EEC.EARNING_NO");
                strQry.AppendLine(",EEC.TOTAL");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");

                strQry.AppendLine(" WHERE EEC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND EEC.RUN_TYPE = 'P' ");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE  = 'S'");

                //7=Bonus 11=Commission
                strQry.AppendLine(" AND EEC.EARNING_NO IN (7,11) ");
                strQry.AppendLine(" AND EEC.TOTAL > 0 ");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "BonusCommission", parint64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",D.IRP5_CODE");
                strQry.AppendLine(",DATEPART(yyyy,EDH.PAY_PERIOD_DATE) AS PERIOD_YEAR");
                strQry.AppendLine(",DATEPART(mm,EDH.PAY_PERIOD_DATE) AS PERIOD_MONTH");
                strQry.AppendLine(",SUM(EDH.TOTAL) AS HISTORY_TOTAL_VALUE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC ");
                strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y' ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.DEDUCTION D ");
                strQry.AppendLine(" ON E.COMPANY_NO = D.COMPANY_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = D.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND D.IRP5_CODE IN (4001,4006,4005)");
                strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY EDH");
                strQry.AppendLine(" ON E.COMPANY_NO = EDH.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EDH.EMPLOYEE_NO ");
                strQry.AppendLine(" AND D.DEDUCTION_NO = EDH.DEDUCTION_NO ");
                //If Previous Fiscal Year then No C/f of History Values (Will be Zero)
                strQry.AppendLine(" AND EDH.PAY_PERIOD_DATE >= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'");
                // NB RUN_TYPE = All

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE  = 'S'");
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

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC ");
                strQry.AppendLine(" ON E.COMPANY_NO = EPCC.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO IN (" + strPayCategoryIn + ")");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EPCC.DEFAULT_IND = 'Y' ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EN ");
                strQry.AppendLine(" ON E.COMPANY_NO = EN.COMPANY_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EN.IRP5_CODE IN (3810)");
                strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
                strQry.AppendLine(" ON E.COMPANY_NO = EEH.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EEH.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EN.EARNING_NO = EEH.EARNING_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE ");
                //If Previous Fiscal Year then No C/f of History Values (Will be Zero)
                strQry.AppendLine(" AND EEH.PAY_PERIOD_DATE >= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'");
                // NB RUN_TYPE = All

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE  = 'S'");
                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" EPCC.PAY_CATEGORY_NO");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",EN.IRP5_CODE");
                strQry.AppendLine(",DATEPART(yyyy,EEH.PAY_PERIOD_DATE)");
                strQry.AppendLine(",DATEPART(mm,EEH.PAY_PERIOD_DATE)");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "TaxSpreadSheet", parint64CompanyNo);

                for (int intEmployeeRow = 0; intEmployeeRow < DataSet.Tables["Employee"].Rows.Count; intEmployeeRow++)
                {
                    strPayUIFInd = "N";
                    strMedicalAidInd = "N";
                    intMedicalAidNumberDependents = Convert.ToInt32(DataSet.Tables["Employee"].Rows[intEmployeeRow]["NUMBER_MEDICAL_AID_DEPENDENTS"]);
                    //dblAllLeaveTotal = 0;

                    DataViewEmployeeYTDTotals = null;
                    DataViewEmployeeYTDTotals = new DataView(DataSet.Tables["EmployeeYTDTotals"],
                        "EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString(),
                        "",
                        DataViewRowState.CurrentRows);

#if (DEBUG)
                    if (DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString() == "23")
                    {
                        string strStop = "";
                    }
#endif
                    //Errol-2015-03-18
                    dblTaxEarningsYTD = Convert.ToDouble(DataViewEmployeeYTDTotals[0]["NORMAL_EARNINGS"]) + Convert.ToDouble(DataViewEmployeeYTDTotals[0]["OTHER_EARNINGS"]);
                    dblTaxEarningsOtherYTD = Convert.ToDouble(DataViewEmployeeYTDTotals[0]["OTHER_EARNINGS"]);

                    DataViewBonus = null;
                    DataViewBonus = new DataView(DataSet.Tables["BonusCommission"],
                        "PAY_CATEGORY_NO = " + DataViewEmployeeYTDTotals[0]["PAY_CATEGORY_NO"].ToString() + " AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"] + " AND EARNING_NO = 7 ",
                        "",
                        DataViewRowState.CurrentRows);

                    if (DataViewBonus.Count > 0)
                    {
                        dblEarningsCurrent = Convert.ToDouble(DataViewEmployeeYTDTotals[0]["EARNING_CURRENT"]) + Convert.ToDouble(DataViewBonus[0]["TOTAL"]);

                    }
                    else
                    {
                        dblEarningsCurrent = Convert.ToDouble(DataViewEmployeeYTDTotals[0]["EARNING_CURRENT"]);
                    }

                    //Used For Income Makro on Deductions
                    dblIncomeEarningsTotal = dblEarningsCurrent;

                    DataViewTaxSpreadSheet = null;
                    DataViewTaxSpreadSheet = new DataView(DataSet.Tables["TaxSpreadSheet"],
                        "PAY_CATEGORY_NO = " + DataViewEmployeeYTDTotals[0]["PAY_CATEGORY_NO"].ToString() + " AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"],
                        "IRP5_CODE,PERIOD_YEAR,PERIOD_MONTH",

                        DataViewRowState.CurrentRows);

                    //Employees Deductions
                    DataViewEmployeeDeduction = null;
                    DataViewEmployeeDeduction = new DataView(DataSet.Tables["EmployeeDeduction"],
                        "PAY_CATEGORY_NO = " + DataViewEmployeeYTDTotals[0]["PAY_CATEGORY_NO"].ToString() + " AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"],
                        "",
                        DataViewRowState.CurrentRows);

                    for (int intEmployeeDeductionRow = 0; intEmployeeDeductionRow < DataViewEmployeeDeduction.Count; intEmployeeDeductionRow++)
                    {
                        if (Convert.ToInt32(DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_NO"]) == 2)
                        {
                            strPayUIFInd = "Y";
                            continue;
                        }

                        if (Convert.ToInt32(DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_NO"]) == 5)
                        {
                            strMedicalAidInd = "Y";
                            continue;
                        }

                        if (DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_TYPE_IND"].ToString() != "U")
                        {
                            if (DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_NO"].ToString() == "3"
                                | DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_NO"].ToString() == "4")
                            {
                                //2014-03-15 Made Change - NB NB Not sure is correct
                                //dblTaxSpreadSheetValue = Convert.ToDouble(strQry.Substring(strQry.IndexOf("=") + 1, strQry.IndexOf(",") - (strQry.IndexOf("=") + 1)).Trim());
                                dblTaxSpreadSheetValue = Convert.ToDouble(DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_VALUE"]);

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
                                }

                                objFindTaxSpreadSheet[0] = intIRP5;
                                objFindTaxSpreadSheet[1] = parCurrentDateTime.Year;
                                objFindTaxSpreadSheet[2] = parCurrentDateTime.Month;

                                intTaxSpreadSheetRow = DataViewTaxSpreadSheet.Find(objFindTaxSpreadSheet);

                                //Only be True on a Second Run
                                if (intTaxSpreadSheetRow > -1)
                                {
                                    DataViewTaxSpreadSheet[intTaxSpreadSheetRow]["HISTORY_TOTAL_VALUE"] = dblTaxSpreadSheetValue;
                                }
                                else
                                {
                                    drvDataRowView = DataViewTaxSpreadSheet.AddNew();
                                    //Set Key for Find
                                    drvDataRowView["PAY_CATEGORY_NO"] = DataViewEmployeeYTDTotals[0]["PAY_CATEGORY_NO"].ToString();
                                    drvDataRowView["EMPLOYEE_NO"] = DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"];
                                    drvDataRowView["IRP5_CODE"] = intIRP5;
                                    drvDataRowView["PERIOD_YEAR"] = parCurrentDateTime.Year;
                                    drvDataRowView["PERIOD_MONTH"] = parCurrentDateTime.Month;
                                    drvDataRowView["HISTORY_TOTAL_VALUE"] = dblTaxSpreadSheetValue;

                                    drvDataRowView.EndEdit();
                                }
                            }

                            //Add To YTD Totals - Used in Tax Calculation
                            switch (Convert.ToInt32(DataViewEmployeeDeduction[intEmployeeDeductionRow]["DEDUCTION_NO"]))
                            {
                                case 1:

                                    dblTaxYTD = Convert.ToDouble(DataViewEmployeeDeduction[intEmployeeDeductionRow]["TOTAL_YTD_BF"]);

                                    break;

                                default:

                                    break;
                            }
                        }
                    }

                    //ELR - 2014-04-16
                    if (clsTaxTableRead == null)
                    {
                        clsTaxTableRead = new clsTaxTableRead();
                        DataSet DataSetTax = clsTaxTableRead.Get_Tax_UIF_Tables(dtEndFinancialYear.Year);
                        Tax = new clsTax(DataSetTax);
                    }

                    //Get Factors for Tax Calculation
                    Tax.Employee_Date_Calculations(parCurrentDateTime, Convert.ToDateTime(DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_BIRTHDATE"]),
                        dtBeginFinancialYear, dtEndFinancialYear,
                        Convert.ToDateTime(DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_TAX_STARTDATE"]), 12,
                        ref dblEmployeePortionOfYear, ref dblAgeAtTaxYearEnd,
                        ref dblEmployeeAnnualisedFactor, "S",
                        Convert.ToDateTime(DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_LAST_RUNDATE"]),
                        ref dblEmployeeDaysWorked);

                    dblWageMonth = parCurrentDateTime.Month;

                    //2014-03-13              
                    //Employees Commission
                    DataViewCommission = null;
                    DataViewCommission = new DataView(DataSet.Tables["BonusCommission"],
                        "PAY_CATEGORY_NO = " + DataViewEmployeeYTDTotals[0]["PAY_CATEGORY_NO"].ToString() + " AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"] + " AND EARNING_NO = 11 ",
                        "",
                        DataViewRowState.CurrentRows);

                    if (DataViewCommission.Count > 0)
                    {
                        dblCommission = Convert.ToDouble(DataViewCommission[0]["TOTAL"]);
                    }
                    else
                    {
                        dblCommission = 0;
                    }

                    //ELR - 2014-04-16
                    if (clsTaxTableRead == null)
                    {
                        clsTaxTableRead = new clsTaxTableRead();
                        DataSet DataSetTax = clsTaxTableRead.Get_Tax_UIF_Tables(dtEndFinancialYear.Year);
                        Tax = new clsTax(DataSetTax);
                    }

                    intReturnCode = Tax.Calculate_Tax(dblTaxEarningsYTD, dblTaxEarningsOtherYTD,
                        ref dblTaxCalculatedRun, dblEmployeeAnnualisedFactor,
                        dblAgeAtTaxYearEnd, dblWageMonth, dblTaxYTD,
                        "P", 0, "S", strPayUIFInd, 12, dblEmployeeDaysWorked, dblEarningsCurrent,
                        ref dblUIFAmount, parCurrentDateTime, strMedicalAidInd, intMedicalAidNumberDependents, DataViewTaxSpreadSheet,
                        null, dblPensionArrearYTD, dblRetireAnnuityArrearYTD,
                        DataSet.Tables["Employee"].Rows[intEmployeeRow]["TAX_TYPE_IND"].ToString(),
                        Convert.ToDouble(DataSet.Tables["Employee"].Rows[intEmployeeRow]["TAX_DIRECTIVE_PERCENTAGE"]),
                        dblCommission,
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

                    //Tax Deduction Number = 1
                    dblDeductionFinalTotal = dblTaxCalculatedRun + dblTaxYTD;

                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" TOTAL = " + dblTaxCalculatedRun);
                    strQry.AppendLine(",TOTAL_ORIGINAL = " + dblTaxCalculatedRun);
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"]);
                    strQry.AppendLine(" AND DEDUCTION_NO = 1");
                    strQry.AppendLine(" AND DEDUCTION_SUB_ACCOUNT_NO = 1");
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND RUN_TYPE = 'P' ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    //UIF
                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" TOTAL = " + dblUIFAmount);
                    strQry.AppendLine(",TOTAL_ORIGINAL = " + dblUIFAmount);
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"]);
                    strQry.AppendLine(" AND DEDUCTION_NO = 2");
                    strQry.AppendLine(" AND DEDUCTION_SUB_ACCOUNT_NO = 1");
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND RUN_TYPE = 'P' ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                }

                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.COMPANY");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" SALARY_RUN_IND = 'Y'");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            Insert_Salary_Run_Records_Continue:
#if (DEBUG)
                //Release Runs off a Queue and Uses a Table as Feedback 
                bytCompress = Get_Employee_WageRun(parint64CompanyNo, "S");

                DataSet.Dispose();
                DataSet = null;
#endif
                //2017-08-11
                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.OPEN_RUN_QUEUE_COMPLETED");

                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",USER_NO");
                strQry.AppendLine(",PAY_PERIOD_DATE");

                strQry.AppendLine(",PAY_CATEGORY_NUMBERS");
                strQry.AppendLine(",PAY_CATEGORY_NUMBERS_NOT_USED");
                strQry.AppendLine(",OPEN_RUN_QUEUE_IND");

                strQry.AppendLine(",START_RUN_DATE");
                strQry.AppendLine(",END_RUN_DATE)");

                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",USER_NO");
                strQry.AppendLine(",PAY_PERIOD_DATE");

                strQry.AppendLine(",PAY_CATEGORY_NUMBERS");
                strQry.AppendLine(",PAY_CATEGORY_NUMBERS_NOT_USED");
                strQry.AppendLine(",'C'");
                strQry.AppendLine(",START_RUN_DATE");
                strQry.AppendLine(",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" FROM InteractPayroll.dbo.OPEN_RUN_QUEUE");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("S"));
                strQry.AppendLine(" AND PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parCurrentDateTime.ToString("yyyy-MM-dd")));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                //2017-08-11
                strQry.Clear();

                strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.OPEN_RUN_QUEUE");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("S"));
                strQry.AppendLine(" AND PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parCurrentDateTime.ToString("yyyy-MM-dd")));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
                strQry.AppendLine(" SET BACKUP_DB_IND = 1");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            }
            catch (Exception ex)
            {
                Write_Log(ex, strClassNameFunctionAndParameters, strQry.ToString(), true);

                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.OPEN_RUN_QUEUE");

                strQry.AppendLine(" SET ");
                strQry.AppendLine(" OPEN_RUN_QUEUE_IND = 'F'");
                strQry.AppendLine(",END_RUN_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("S"));
                strQry.AppendLine(" AND PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parCurrentDateTime.ToString("yyyy-MM-dd")));
                
                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            }

            return bytCompress;
        }

        public byte[] Insert_TimeAttendance_Run_Records(Int64 parint64CompanyNo, DateTime parCurrentDateTime, string strPayCategoryNoIN)
        {
            string strClassNameFunctionAndParameters = pvtstrClassName + " Insert_TimeAttendance_Run_Records CompanyNo=" + parint64CompanyNo + ",parCurrentDateTime=" + parCurrentDateTime.ToString("yyyy-MM-dd") + ",strPayCategoryNoIN=" + strPayCategoryNoIN;
            
            StringBuilder strQry = new StringBuilder();
            byte[] bytCompress = null;

            try
            {
                //2018-10-27 - Start
                strQry.Clear();
                
                strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ");
                strQry.AppendLine(" DISABLE TRIGGER tgr_EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_Table ");
              
                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ");
                strQry.AppendLine(" DISABLE TRIGGER tgr_EMPLOYEE_TIME_ATTEND_BREAK_CURRENT_Maintain_EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_Table ");
              
                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT");

                strQry.AppendLine(" SET ");
                strQry.AppendLine(" INCLUDED_IN_RUN_IND = NULL");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_NO  IN (" + strPayCategoryNoIN + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT");

                strQry.AppendLine(" SET ");
                strQry.AppendLine(" INCLUDED_IN_RUN_IND = NULL");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_NO  IN (" + strPayCategoryNoIN + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                                
                strQry.Clear();

                strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ");
                strQry.AppendLine(" ENABLE TRIGGER tgr_EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ");
                strQry.AppendLine(" ENABLE TRIGGER tgr_EMPLOYEE_TIME_ATTEND_BREAK_CURRENT_Maintain_EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                //2018-10-27 - End
                
                //2017-08-115
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.OPEN_RUN_QUEUE");

                strQry.AppendLine(" SET ");
                //S=Started
                strQry.AppendLine(" OPEN_RUN_QUEUE_IND = 'S'");
                strQry.AppendLine(",START_RUN_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));
                strQry.AppendLine(" AND PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parCurrentDateTime.ToString("yyyy-MM-dd")));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                int intPaidHoliday = 0;
                int intPaidHolidayNumber = 0;
                int intDayNo = 0;
                int intDay = 0;
                int intRunNo = -1;
                
                string strQryTemp = "";
                string strFieldId = "";

                DataSet DataSet = new DataSet();
                DataSet parDataSet = new DataSet();

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PC.PAY_CATEGORY_NO");
                strQry.AppendLine(",MAX(PCPH.PAY_PERIOD_DATE) AS PREV_PAY_PERIOD_DATE ");
                strQry.AppendLine(",PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY = ");

                strQry.AppendLine(" DATEADD(dd,1,MIN(CASE ");

                strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y' ");

                strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

                strQry.AppendLine(" END)) ");

                strQry.AppendLine(",'' AS PUBLIC_HOLIDAYS_ERROR ");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON PC.COMPANY_NO = E.COMPANY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND EMPLOYEE_ENDDATE IS NULL");
                strQry.AppendLine(" AND NOT EMPLOYEE_LAST_RUNDATE IS NULL");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");
                strQry.AppendLine(" ON PC.COMPANY_NO = PCPH.COMPANY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = PCPH.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND PCPH.RUN_TYPE = 'P'");

                strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO IN (" + strPayCategoryNoIN + ")");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));
                strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");
                strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" PC.PAY_CATEGORY_NO");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" 1");
                strQry.AppendLine(",2");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), parDataSet, "Upload", parint64CompanyNo);

                StringBuilder strFieldNamesInitialised = new StringBuilder();
                DateTime dtBeginFinancialYear;
                DateTime dtEndFinancialYear;
                DateTime dtPreviousDateTime;

                if (parCurrentDateTime.Month > 2)
                {
                    dtBeginFinancialYear = new DateTime(parCurrentDateTime.Year, 3, 1);
                }
                else
                {
                    dtBeginFinancialYear = new DateTime(parCurrentDateTime.Year - 1, 3, 1);
                }

                //Last Day Of Fiscal Year
                dtEndFinancialYear = dtBeginFinancialYear.AddYears(1).AddDays(-1);

                for (int intRow = 0; intRow < parDataSet.Tables["Upload"].Rows.Count; intRow++)
                {
                    if (intRow == 0)
                    {
                        strQry.Clear();

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" MAX(ISNULL(RUN_NO,0)) + 1 AS RUN_NO");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY");

                        strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                        strQry.AppendLine(" AND PAY_PERIOD_DATE = '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");
                        strQry.AppendLine(" AND PAY_CATEGORY_NO IN (" + strPayCategoryNoIN + ")");

                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));
                        strQry.AppendLine(" AND RUN_TYPE = 'P'");

                        clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "RunNo", parint64CompanyNo);

                        if (DataSet.Tables["RunNo"].Rows.Count == 0)
                        {
                            intRunNo = 1;
                        }
                        else
                        {
                            if (DataSet.Tables["RunNo"].Rows[0]["RUN_NO"] == System.DBNull.Value)
                            {
                                intRunNo = 1;
                            }
                            else
                            {
                                intRunNo = Convert.ToInt32(DataSet.Tables["RunNo"].Rows[0]["RUN_NO"]);
                            }
                        }

                        strQry.Clear();

                        strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.COMPANY");
                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" TIME_ATTENDANCE_RUN_IND = 'N'");

                        strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                    }

                    //Initial PAY_CATEGORY does NOT have Value - Use Min EMPLOYEE_LAST_RUNDATE
                    if (parDataSet.Tables["Upload"].Rows[intRow]["PREV_PAY_PERIOD_DATE"] == System.DBNull.Value)
                    {
                        //Already Added 1 to Last Rundate
                        parDataSet.Tables["Upload"].Rows[intRow]["PREV_PAY_PERIOD_DATE"] = Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY"]);
                        dtPreviousDateTime = Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_PAY_PERIOD_DATE"]);
                    }
                    else
                    {
                        dtPreviousDateTime = Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_PAY_PERIOD_DATE"]).AddDays(1);
                    }

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_PERIOD_DATE");
                    strQry.AppendLine(",RUN_TYPE");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",RUN_NO");

                    strQry.AppendLine(",PAY_PERIOD_DATE_FROM");
                    strQry.AppendLine(",SALARY_TIMESHEET_ENDDATE");

                    strFieldNamesInitialised.Clear();
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("PAY_CATEGORY_PERIOD_CURRENT", ref strQry, ref strFieldNamesInitialised, "", parint64CompanyNo);

                    strQry.AppendLine(")");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" COMPANY_NO");
                    strQry.AppendLine(",'" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",'P'");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.AppendLine(",'" + dtPreviousDateTime.ToString("yyyy-MM-dd") + "'");
                    //SALARY_TIMESHEET_ENDDATE
                    strQry.AppendLine(",NULL ");

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    if (intRow == 0)
                    {
                        strQry.Clear();

                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK_CURRENT");
                        strQry.AppendLine("(COMPANY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",PAY_CATEGORY_BREAK_NO");
                        strQry.AppendLine(",RUN_NO");

                        strFieldNamesInitialised.Clear();
                        clsDBConnectionObjects.Initialise_DataSet_Name_Fields("PAY_CATEGORY_BREAK_CURRENT", ref strQry, ref strFieldNamesInitialised, "", parint64CompanyNo);

                        strQry.AppendLine(")");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",PAY_CATEGORY_BREAK_NO");
                        strQry.AppendLine("," + intRunNo.ToString());

                        strQry.Append(strFieldNamesInitialised);

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK");

                        strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                        strQry.AppendLine(" AND PAY_CATEGORY_NO IN (" + strPayCategoryNoIN + ")");
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));
                        strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                        strQry.Clear();

                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT");
                        strQry.AppendLine("(COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",PAY_CATEGORY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",LEVEL_NO");
                        strQry.AppendLine(",USER_NO");
                        strQry.AppendLine(",AUTHORISED_IND)");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",PCA.LEVEL_NO");
                        strQry.AppendLine(",PCA.USER_NO");
                        strQry.AppendLine(",'N'");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                        strQry.AppendLine(" ON EPC.COMPANY_NO = E.COMPANY_NO");
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = E.EMPLOYEE_NO");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                        //Employee Has Been Activates (Taken-On)
                        strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                        //Employee NOT Closed
                        strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                        strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_AUTHORISE PCA");
                        strQry.AppendLine(" ON EPC.COMPANY_NO = PCA.COMPANY_NO");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PCA.PAY_CATEGORY_NO");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PCA.PAY_CATEGORY_TYPE");

                        //T = Timesheet, L = Leave
                        strQry.AppendLine(" AND PCA.AUTHORISE_TYPE_IND = 'T'");
                        strQry.AppendLine(" AND PCA.DATETIME_DELETE_RECORD IS NULL");

                        strQry.AppendLine(" WHERE EPC.COMPANY_NO = " + parint64CompanyNo);
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO IN (" + strPayCategoryNoIN + ")");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'T'");

                        strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                  
                        strQry.Clear();

                        //Get Lowest PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY
                        DataView DataViewUpload = new DataView(parDataSet.Tables["Upload"],
                        "",
                        "PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY",
                        DataViewRowState.CurrentRows);

                        DateTime myDataTime = Convert.ToDateTime(DataViewUpload[0]["PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY"]);

                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_CURRENT");
                        strQry.AppendLine("(COMPANY_NO");
                        strQry.AppendLine(",RUN_NO");

                        strFieldNamesInitialised.Clear();
                        clsDBConnectionObjects.Initialise_DataSet_Name_Fields("PUBLIC_HOLIDAY_CURRENT", ref strQry, ref strFieldNamesInitialised, "PH", parint64CompanyNo);

                        strQry.AppendLine(")");

                        strQry.AppendLine(" SELECT DISTINCT ");
                        strQry.AppendLine(parint64CompanyNo.ToString());
                        strQry.AppendLine("," + intRunNo.ToString());

                        strQry.Append(strFieldNamesInitialised);

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY PH");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
                        strQry.AppendLine(" ON PC.COMPANY_NO = " + parint64CompanyNo);
                        strQry.AppendLine(" AND PC.PAY_CATEGORY_NO IN (" + strPayCategoryNoIN + ")");

                        strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));
                        strQry.AppendLine(" AND PC.PAY_PUBLIC_HOLIDAY_IND = 'Y' ");

                        //Errol Changed 2011-04-26
                        strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_CURRENT PHC ");
                        strQry.AppendLine(" ON PC.COMPANY_NO = PHC.COMPANY_NO ");
                        strQry.AppendLine(" AND PH.PUBLIC_HOLIDAY_DATE = PHC.PUBLIC_HOLIDAY_DATE ");

                        strQry.AppendLine(" WHERE PH.PUBLIC_HOLIDAY_DATE >= '" + Convert.ToDateTime(DataViewUpload[0]["PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY"]).ToString("yyyy-MM-dd") + "'");
                        strQry.AppendLine(" AND PH.PUBLIC_HOLIDAY_DATE <= '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                        //Errol Changed 2011-04-26
                        strQry.AppendLine(" AND PHC.PUBLIC_HOLIDAY_DATE IS NULL");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                        strQry.Clear();

                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT");
                        strQry.AppendLine("(COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",RUN_TYPE");
                        strQry.AppendLine(",PAY_CATEGORY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",RUN_NO");
                        strQry.AppendLine(",HOURLY_RATE");
                        strQry.AppendLine(",OVERTIME_VALUE_BF");
                        strQry.AppendLine(",OVERTIME_VALUE_CF");
                        strQry.AppendLine(",DEFAULT_IND)");

                        strQry.AppendLine(" SELECT DISTINCT");
                        strQry.AppendLine(" EPC.COMPANY_NO");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO");
                        strQry.AppendLine(",'P'");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
                        strQry.AppendLine("," + intRunNo.ToString());

                        //NB Salary EPC.HOURLY_RATE Holds Monthly value
                        strQry.AppendLine(",0");

                        //NB. Value From CF is Carried to BF
                        strQry.AppendLine(",0");
                        strQry.AppendLine(",0");

                        strQry.AppendLine(",EPC.DEFAULT_IND");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");

                        //Used to Work out Hourly Rate of Employee (Salaries)
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
                        strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                        strQry.AppendLine(" ON EPC.COMPANY_NO = E.COMPANY_NO ");
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                        strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                        strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                        strQry.AppendLine(" AND E.EMPLOYEE_LAST_RUNDATE < '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                        strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");
                        strQry.AppendLine(" ON EPC.COMPANY_NO = EPCH.COMPANY_NO ");
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = EPCH.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = EPCH.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = EPCH.PAY_CATEGORY_TYPE");
                        //If Previous Fiscal Year then No C/f of History Values (Will be Zero)
                        strQry.AppendLine(" AND EPCH.PAY_PERIOD_DATE >= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'");
                        strQry.AppendLine(" AND EPCH.RUN_TYPE = 'P'");

                        strQry.AppendLine(" AND STR(EPCH.EMPLOYEE_NO) + CONVERT(CHAR,EPCH.PAY_PERIOD_DATE) IN ");
                        strQry.AppendLine("(SELECT STR(EMPLOYEE_NO) + CONVERT(CHAR,MAX(PAY_PERIOD_DATE)) ");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY ");

                        strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                        //strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                        strQry.AppendLine(" AND PAY_CATEGORY_NO  IN (" + strPayCategoryNoIN + ")");
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));
                        strQry.AppendLine(" AND RUN_TYPE = 'P'");
                        strQry.AppendLine(" GROUP BY EMPLOYEE_NO)");

                        strQry.AppendLine(" WHERE EPC.COMPANY_NO = " + parint64CompanyNo);
                        //strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));

                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO IN (" + strPayCategoryNoIN + ")");

                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));
                        strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                    }

                    //First Set Day No
                    System.TimeSpan s = parCurrentDateTime.Subtract(dtPreviousDateTime);

                    if (s.Days <= 6)
                    {
                        intDayNo = Convert.ToInt32(parCurrentDateTime.DayOfWeek);
                    }

                    //Incomplete Week up to First Week
                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_CURRENT");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",WEEK_DATE");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",RUN_NO");

                    strQry.AppendLine(",WEEK_DATE_FROM");
                    //New Fields Not Found in PAY_CATEGORY
                    strQry.AppendLine(",PAIDHOLIDAY_MINUTES1");
                    strQry.AppendLine(",PAIDHOLIDAY_DAY1");
                    strQry.AppendLine(",PAIDHOLIDAY_MINUTES2");
                    strQry.AppendLine(",PAIDHOLIDAY_DAY2");
                    strQry.AppendLine(",PAIDHOLIDAY_MINUTES3");
                    strQry.AppendLine(",PAIDHOLIDAY_DAY3");
                    strQry.AppendLine(",PAIDHOLIDAY_MINUTES4");
                    strQry.AppendLine(",PAIDHOLIDAY_DAY4");
                    strQry.AppendLine(",PAIDHOLIDAY_MINUTES5");
                    strQry.AppendLine(",PAIDHOLIDAY_DAY5");
                    strQry.AppendLine(",AMOUNT_HOURS");
                    strQry.AppendLine(",AMOUNT_VALUE");
                    strQry.AppendLine(",AMOUNT_VALUE_YTD");

                    strFieldNamesInitialised.Clear();
                    clsDBConnectionObjects.Initialise_DataSet_Name_Fields("PAY_CATEGORY_WEEK_CURRENT", ref strQry, ref strFieldNamesInitialised, "PC", parint64CompanyNo);

                    strQry.AppendLine(")");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" PC.COMPANY_NO");
                    strQry.AppendLine(",D.DAY_DATE");
                    strQry.AppendLine(",PC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",'T'");

                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.AppendLine(",'" + Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");
                    strQry.AppendLine(" ON D.DAY_DATE >= '" + Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND D.DAY_DATE <=  '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND D.DAY_DATE = ");
                    strQry.AppendLine("(SELECT MIN(DAY_DATE)");

                    strQry.AppendLine(" FROM InteractPayroll.dbo.DATES ");

                    strQry.AppendLine(" WHERE DAY_NO = " + intDayNo);
                    strQry.AppendLine(" AND DAY_DATE >= '" + Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND DAY_DATE <=  '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "')");

                    strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));

                    strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));
                    strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" UNION ");

                    //Full Weeks
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" PC.COMPANY_NO");
                    strQry.AppendLine(",D.DAY_DATE");
                    strQry.AppendLine(",PC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",'T'");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.AppendLine(",DATEADD(d, -6, D.DAY_DATE)");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");
                    strQry.AppendLine(" ON D.DAY_NO = " + intDayNo);
                    strQry.AppendLine(" AND DATEADD(d, -7, D.DAY_DATE) >= '" + Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND D.DAY_DATE <= '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));

                    strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" UNION ");

                    //Incomplete Week AFTER All Full WeekS
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" PC.COMPANY_NO");
                    strQry.AppendLine(",'" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",PC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",'T'");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.AppendLine(",D.DAY_DATE");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");
                    strQry.AppendLine(",0");

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");
                    strQry.AppendLine(" ON D.DAY_DATE >= '" + Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND D.DAY_DATE <=  '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND D.DAY_DATE = ");
                    strQry.AppendLine("(SELECT DATEADD(d,+1,MAX(DAY_DATE))");

                    strQry.AppendLine(" FROM InteractPayroll.dbo.DATES ");

                    strQry.AppendLine(" WHERE DAY_NO = " + intDayNo);
                    strQry.AppendLine(" AND DAY_DATE >= '" + Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND DAY_DATE <=  '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "')");

                    strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));
                    strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    //Insert First Incomplete Week		
                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_WEEK_CURRENT");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",WEEK_DATE");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",RUN_NO");

                    strQry.AppendLine(",WEEK_DATE_FROM");

                    strFieldNamesInitialised.Clear();
                    clsDBConnectionObjects.Initialise_DataSet_Numeric_Fields("EMPLOYEE_EARNING_WEEK_CURRENT", ref strQry, ref strFieldNamesInitialised, parint64CompanyNo);

                    strQry.AppendLine(")");

                    strQry.AppendLine(" SELECT DISTINCT");
                    strQry.AppendLine(" EMPLOYEE_TABLE.COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_TABLE.EMPLOYEE_NO");
                    strQry.AppendLine(",D.DAY_DATE");
                    strQry.AppendLine(",EMPLOYEE_TABLE.PAY_CATEGORY_NO");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.AppendLine(",DATEADD(dd,1,EMPLOYEE_TABLE.EMPLOYEE_LAST_RUNDATE)");

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM ");

                    strQry.AppendLine("(SELECT ");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");

                    strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

                    strQry.AppendLine(" CASE ");

                    //Errol - 2015-02-13
                    strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y' ");

                    strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" ELSE  E.EMPLOYEE_LAST_RUNDATE ");

                    strQry.AppendLine(" END  ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL) AS EMPLOYEE_TABLE");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");
                    strQry.AppendLine(" ON D.DAY_DATE > EMPLOYEE_TABLE.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND D.DAY_DATE < DATEADD(dd,8,EMPLOYEE_TABLE.EMPLOYEE_LAST_RUNDATE)");
                    strQry.AppendLine(" AND D.DAY_DATE <= '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND D.DAY_NO = " + intDayNo);

                    strQry.AppendLine(" UNION ");

                    //Insert Complete Week		
                    strQry.AppendLine(" SELECT DISTINCT");
                    strQry.AppendLine(" EMPLOYEE_TABLE.COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_TABLE.EMPLOYEE_NO");
                    strQry.AppendLine(",D.DAY_DATE");
                    strQry.AppendLine(",EMPLOYEE_TABLE.PAY_CATEGORY_NO");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.AppendLine(",DATEADD(d, -6, D.DAY_DATE)");

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM ");

                    strQry.AppendLine("(SELECT ");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");

                    strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

                    strQry.AppendLine(" CASE ");

                    //Errol - 2015-02-13
                    strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y' ");

                    strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" ELSE  E.EMPLOYEE_LAST_RUNDATE ");

                    strQry.AppendLine(" END  ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL) AS EMPLOYEE_TABLE");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");
                    strQry.AppendLine(" ON EMPLOYEE_TABLE.EMPLOYEE_LAST_RUNDATE < DATEADD(d, -7, D.DAY_DATE)");
                    strQry.AppendLine(" AND D.DAY_NO = " + intDayNo);
                    strQry.AppendLine(" AND DATEADD(d, -7, D.DAY_DATE) >= '" + Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND D.DAY_DATE <= '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                    strQry.AppendLine(" UNION ");

                    //Insert Last Incomplete Week
                    strQry.AppendLine(" SELECT DISTINCT");
                    strQry.AppendLine(" EMPLOYEE_TABLE.COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_TABLE.EMPLOYEE_NO");
                    strQry.AppendLine(",'" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(",EMPLOYEE_TABLE.PAY_CATEGORY_NO");
                    strQry.AppendLine("," + intRunNo.ToString());

                    strQry.AppendLine(",D.DAY_DATE");

                    strQry.Append(strFieldNamesInitialised);

                    strQry.AppendLine(" FROM ");

                    strQry.AppendLine("(SELECT ");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");

                    strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

                    strQry.AppendLine(" CASE ");

                    //Errol - 2015-02-13
                    strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y' ");

                    strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                    strQry.AppendLine(" ELSE  E.EMPLOYEE_LAST_RUNDATE ");

                    strQry.AppendLine(" END  ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));
                    strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));
                    strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL) AS EMPLOYEE_TABLE");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D");
                    strQry.AppendLine(" ON EMPLOYEE_TABLE.EMPLOYEE_LAST_RUNDATE < D.DAY_DATE");
                    strQry.AppendLine(" AND D.DAY_DATE >= '" + Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND D.DAY_DATE <=  '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND D.DAY_DATE = ");
                    strQry.AppendLine("(SELECT DATEADD(d,+1,MAX(DAY_DATE))");

                    strQry.AppendLine(" FROM InteractPayroll.dbo.DATES ");

                    strQry.AppendLine(" WHERE DAY_NO = " + intDayNo);
                    strQry.AppendLine(" AND DAY_DATE >= '" + Convert.ToDateTime(parDataSet.Tables["Upload"].Rows[intRow]["PREV_EMPLOYEE_LAST_RUNDATE_PLUS_ONE_DAY"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND DAY_DATE <=  '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    if (intRow == 0)
                    {
                        strQry.Clear();

                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT");
                        strQry.AppendLine("(COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",RUN_TYPE");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",RUN_NO");
                        strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE");
                        strQry.AppendLine(",EXTRA_CHEQUES_HISTORY");
                        strQry.AppendLine(",EXTRA_CHEQUES_CURRENT");
                        strQry.AppendLine(",CLOSE_IND");
                        strQry.AppendLine(",PAYSLIP_IND");

                        //ELR 2014-05-02
                        strQry.AppendLine(",NUMBER_MEDICAL_AID_DEPENDENTS");
                        strQry.AppendLine(",OCCUPATION_NO");
                        strQry.AppendLine(",CURRENT_YEAR_LEAVE_SHIFTS_PER_RUN");
                        strQry.AppendLine(",PREV_YEAR_LEAVE_SHIFTS_PER_RUN");

                        //2012-11-23
                        strQry.AppendLine(",LEAVE_SHIFT_NO");

                        strQry.AppendLine(",SALARY_MONTH_PAYMENT)");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO");
                        strQry.AppendLine(",E.EMPLOYEE_NO");
                        strQry.AppendLine(",'P'");
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL("T"));
                        strQry.AppendLine("," + intRunNo.ToString());

                        strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

                        strQry.AppendLine(" CASE ");

                        //Errol - 2015-02-13
                        strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y' ");

                        strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                        strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

                        strQry.AppendLine(" END  ");

                        strQry.AppendLine(",0");

                        //Double Cheque For BirthDay
                        strQry.AppendLine(",0");

                        strQry.AppendLine(",'N'");
                        strQry.AppendLine(",'Y'");

                        //ELR 2014-05-02
                        strQry.AppendLine(",0");
                        strQry.AppendLine(",0");
                        strQry.AppendLine(",0");
                        strQry.AppendLine(",0");

                        //2012-11-23
                        strQry.AppendLine(",E.LEAVE_SHIFT_NO");

                        //Monthly Payment is held in HOURLY_RATE for Salaries
                        strQry.AppendLine(",0");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO IN (" + strPayCategoryNoIN + ")");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));
                        strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
                        strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                        strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                        strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                        strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                        strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));
                        strQry.AppendLine(" AND E.EMPLOYEE_LAST_RUNDATE < '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                        strQry.AppendLine(" GROUP BY");

                        strQry.AppendLine(" E.COMPANY_NO");
                        strQry.AppendLine(",E.EMPLOYEE_NO");

                        strQry.AppendLine(",E.FIRST_RUN_COMPLETED_IND");
                        strQry.AppendLine(",E.EMPLOYEE_LAST_RUNDATE");
                        strQry.AppendLine(",E.LEAVE_SHIFT_NO");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                        //Insert Record for All Linked Earnings 
                        strQry.Clear();

                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
                        strQry.AppendLine("(COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",RUN_TYPE");
                        strQry.AppendLine(",EARNING_NO");
                        strQry.AppendLine(",PAY_CATEGORY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",RUN_NO");
                        strQry.AppendLine(",TOTAL");
                        //Used Salaries for Normal Leave when Employee is Closed 
                        strQry.AppendLine(",HOURS_DECIMAL_OTHER_VALUE");

                        strFieldNamesInitialised.Clear();
                        clsDBConnectionObjects.Initialise_DataSet_Numeric_Fields("EMPLOYEE_EARNING_CURRENT", ref strQry, ref strFieldNamesInitialised, parint64CompanyNo);

                        strQry.AppendLine(")");

                        strQry.AppendLine(" SELECT DISTINCT");
                        strQry.AppendLine(" E.COMPANY_NO");
                        strQry.AppendLine(",E.EMPLOYEE_NO");
                        strQry.AppendLine(",'P'");
                        strQry.AppendLine(",EN.EARNING_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
                        strQry.AppendLine("," + intRunNo.ToString());
                        strQry.AppendLine(",0");
                        //strQry.AppendLine(",0");
                        strQry.AppendLine(",0");

                        strQry.Append(strFieldNamesInitialised);

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO IN (" + strPayCategoryNoIN + ")");
                        //Link To All Pay Category
                        //strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
                        strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EN");
                        strQry.AppendLine(" ON E.COMPANY_NO = EN.COMPANY_NO ");
                        //strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE");
                        //2013-06-19
                        //Link To Wages PAY_CATEGORY_TYPE
                        strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = 'W'");
                        strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL ");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
                        strQry.AppendLine(" ON E.COMPANY_NO = PC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
                        strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL ");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C");
                        strQry.AppendLine(" ON E.COMPANY_NO = C.COMPANY_NO ");
                        strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL ");

                        strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                        strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO IN (" + strPayCategoryNoIN + ")");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));
                        strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
                        strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                        strQry.AppendLine(" AND E.EMPLOYEE_LAST_RUNDATE < '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");

                        strQry.AppendLine(" AND (EN.EARNING_NO IN (2,9)");

                        strQry.AppendLine(" OR (EN.EARNING_NO = 3");
                        strQry.AppendLine(" AND C.OVERTIME1_RATE <> 0)");

                        strQry.AppendLine(" OR (EN.EARNING_NO = 4");
                        strQry.AppendLine(" AND C.OVERTIME2_RATE <> 0)");

                        strQry.AppendLine(" OR (EN.EARNING_NO = 5");
                        strQry.AppendLine(" AND C.OVERTIME3_RATE <> 0))");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                    }
                }

                //Set Paid Holiday
                strQry.Clear();

                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" PCWC.PAY_CATEGORY_NO ");
                strQry.AppendLine(",PCWC.WEEK_DATE ");
                strQry.AppendLine(",PCWC.MON_TIME_MINUTES ");
                strQry.AppendLine(",PCWC.TUE_TIME_MINUTES ");
                strQry.AppendLine(",PCWC.WED_TIME_MINUTES ");
                strQry.AppendLine(",PCWC.THU_TIME_MINUTES ");
                strQry.AppendLine(",PCWC.FRI_TIME_MINUTES ");
                strQry.AppendLine(",PCWC.SAT_TIME_MINUTES ");
                strQry.AppendLine(",PCWC.SUN_TIME_MINUTES ");
                strQry.AppendLine(",PHD.PUBLIC_HOLIDAY_DATE ");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_CURRENT PCWC");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_CURRENT PHD");
                strQry.AppendLine(" ON PCWC.COMPANY_NO = PHD.COMPANY_NO ");
                strQry.AppendLine(" AND PHD.PUBLIC_HOLIDAY_DATE >= PCWC.WEEK_DATE_FROM ");
                strQry.AppendLine(" AND PHD.PUBLIC_HOLIDAY_DATE <= PCWC.WEEK_DATE ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");
                strQry.AppendLine(" ON PCWC.COMPANY_NO = PCPC.COMPANY_NO ");
                strQry.AppendLine(" AND PCWC.PAY_CATEGORY_NO = PCPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND PCWC.PAY_CATEGORY_TYPE = PCPC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P' ");
                strQry.AppendLine(" AND PCPC.PAY_PUBLIC_HOLIDAY_IND = 'Y' ");

                strQry.AppendLine(" WHERE PCWC.COMPANY_NO = " + parint64CompanyNo);
                //strQry.AppendLine(" AND PCWC.PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));

                strQry.AppendLine(" AND PCWC.PAY_CATEGORY_NO IN (" + strPayCategoryNoIN + ")");

                strQry.AppendLine(" AND PCWC.PAY_CATEGORY_TYPE = 'T'");
                //Check if Company Pays Public Holidays

                strQry.AppendLine(" ORDER BY ");

                strQry.AppendLine(" PCWC.PAY_CATEGORY_NO ");
                strQry.AppendLine(",PCWC.WEEK_DATE ");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PublicHolidays", parint64CompanyNo);

                for (int intRow = 0; intRow < parDataSet.Tables["Upload"].Rows.Count; intRow++)
                {
                    DataView DataViewPublicHolidays = new DataView(DataSet.Tables["PublicHolidays"],
                        "PAY_CATEGORY_NO = " + parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"].ToString(),
                        "",
                        DataViewRowState.CurrentRows);

                    if (parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"].ToString() == "59")
                    {
                        int intStop = 0;
                    }

                    if (DataViewPublicHolidays.Count > 0)
                    {
                        for (int intRow2 = 0; intRow2 < DataViewPublicHolidays.Count; intRow2++)
                        {
                            intDay = Convert.ToInt32(Convert.ToDateTime(DataViewPublicHolidays[intRow2]["PUBLIC_HOLIDAY_DATE"]).DayOfWeek);

                            switch (intDay)
                            {
                                case 0:

                                    strQryTemp = ",SUN_TIME_MINUTES = 0";
                                    strFieldId = "SUN_TIME_MINUTES";
                                    break;

                                case 1:

                                    strQryTemp = ",MON_TIME_MINUTES = 0";
                                    strFieldId = "MON_TIME_MINUTES";
                                    break;

                                case 2:

                                    strQryTemp = ",TUE_TIME_MINUTES = 0";
                                    strFieldId = "TUE_TIME_MINUTES";
                                    break;

                                case 3:

                                    strQryTemp = ",WED_TIME_MINUTES = 0";
                                    strFieldId = "WED_TIME_MINUTES";
                                    break;

                                case 4:

                                    strQryTemp = ",THU_TIME_MINUTES = 0";
                                    strFieldId = "THU_TIME_MINUTES";
                                    break;

                                case 5:

                                    strQryTemp = ",FRI_TIME_MINUTES = 0";
                                    strFieldId = "FRI_TIME_MINUTES";
                                    break;

                                case 6:

                                    strQryTemp = ",SAT_TIME_MINUTES = 0";
                                    strFieldId = "SAT_TIME_MINUTES";
                                    break;
                            }

                            intPaidHoliday = Convert.ToDateTime(DataViewPublicHolidays[intRow2]["PUBLIC_HOLIDAY_DATE"]).Day;

                            if (DataSet.Tables["PublicHolidaySlotFind"] != null)
                            {
                                DataSet.Tables.Remove("PublicHolidaySlotFind");
                            }

                            strQry.Clear();

                            strQry.AppendLine(" SELECT ");

                            strQry.AppendLine(" PAIDHOLIDAY_DAY1 ");
                            strQry.AppendLine(",PAIDHOLIDAY_DAY2 ");
                            strQry.AppendLine(",PAIDHOLIDAY_DAY3 ");
                            strQry.AppendLine(",PAIDHOLIDAY_DAY4 ");
                            strQry.AppendLine(",PAIDHOLIDAY_DAY5 ");

                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_CURRENT ");

                            //Run Update
                            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                            strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");
                            strQry.AppendLine(" AND WEEK_DATE = '" + Convert.ToDateTime(DataViewPublicHolidays[intRow2]["WEEK_DATE"]).ToString("yyyy-MM-dd") + "'");

                            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PublicHolidaySlotFind", parint64CompanyNo);

                            if (Convert.ToUInt32(DataSet.Tables["PublicHolidaySlotFind"].Rows[0]["PAIDHOLIDAY_DAY1"]) == 0)
                            {
                                intPaidHolidayNumber = 1;
                            }
                            else
                            {
                                if (Convert.ToUInt32(DataSet.Tables["PublicHolidaySlotFind"].Rows[0]["PAIDHOLIDAY_DAY2"]) == 0)
                                {
                                    intPaidHolidayNumber = 2;
                                }
                                else
                                {
                                    if (Convert.ToUInt32(DataSet.Tables["PublicHolidaySlotFind"].Rows[0]["PAIDHOLIDAY_DAY3"]) == 0)
                                    {
                                        intPaidHolidayNumber = 3;
                                    }
                                    else
                                    {
                                        if (Convert.ToUInt32(DataSet.Tables["PublicHolidaySlotFind"].Rows[0]["PAIDHOLIDAY_DAY4"]) == 0)
                                        {
                                            intPaidHolidayNumber = 4;
                                        }
                                        else
                                        {
                                            intPaidHolidayNumber = 5;
                                        }
                                    }
                                }
                            }

                            strQry.Clear();

                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_CURRENT ");
                            strQry.AppendLine(" SET ");
                            strQry.AppendLine(" PAIDHOLIDAY_DAY" + intPaidHolidayNumber.ToString() + " = " + intPaidHoliday);
                            strQry.AppendLine(",PAIDHOLIDAY_MINUTES" + intPaidHolidayNumber.ToString() + " = " + Convert.ToInt32(DataViewPublicHolidays[intRow2][strFieldId]));

                            //Set Relevant Day Paid Hours to Zero
                            strQry.AppendLine(strQryTemp);

                            //Run Update
                            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                            strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["Upload"].Rows[intRow]["PAY_CATEGORY_NO"]));
                            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");
                            strQry.AppendLine(" AND WEEK_DATE = '" + Convert.ToDateTime(DataViewPublicHolidays[intRow2]["WEEK_DATE"]).ToString("yyyy-MM-dd") + "'");

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                        }
                    }
                }

#if (DEBUG)
                //Release Runs off a Queue and Uses a Table as Feedback 
                bytCompress = Get_Employee_WageRun(parint64CompanyNo, "T");
       
                DataSet.Dispose();
                DataSet = null;
#endif
                //2017-08-11
                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.OPEN_RUN_QUEUE_COMPLETED");

                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",USER_NO");
                strQry.AppendLine(",PAY_PERIOD_DATE");

                strQry.AppendLine(",PAY_CATEGORY_NUMBERS");
                strQry.AppendLine(",PAY_CATEGORY_NUMBERS_NOT_USED");
                strQry.AppendLine(",OPEN_RUN_QUEUE_IND");

                strQry.AppendLine(",START_RUN_DATE");
                strQry.AppendLine(",END_RUN_DATE)");

                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",USER_NO");
                strQry.AppendLine(",PAY_PERIOD_DATE");

                strQry.AppendLine(",PAY_CATEGORY_NUMBERS");
                strQry.AppendLine(",PAY_CATEGORY_NUMBERS_NOT_USED");
                strQry.AppendLine(",'C'");
                strQry.AppendLine(",START_RUN_DATE");
                strQry.AppendLine(",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" FROM InteractPayroll.dbo.OPEN_RUN_QUEUE");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));
                strQry.AppendLine(" AND PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parCurrentDateTime.ToString("yyyy-MM-dd")));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                //2017-08-05
                strQry.Clear();

                strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.OPEN_RUN_QUEUE");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));
                strQry.AppendLine(" AND PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parCurrentDateTime.ToString("yyyy-MM-dd")));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
                strQry.AppendLine(" SET BACKUP_DB_IND = 1");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            }
            catch (Exception ex)
            {
                Write_Log(ex, strClassNameFunctionAndParameters, strQry.ToString(), true);

                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.OPEN_RUN_QUEUE");

                strQry.AppendLine(" SET ");
                strQry.AppendLine(" OPEN_RUN_QUEUE_IND = 'F'");
                strQry.AppendLine(",END_RUN_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL("T"));
                strQry.AppendLine(" AND PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parCurrentDateTime.ToString("yyyy-MM-dd")));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            }

            return bytCompress;
        }
        
        public byte[] Delete_Records(Int64 parint64User, Int64 parint64CompanyNo, string parstrPayCategoryNos, string parstrPayrollType, DateTime parCurrentDateTime)
        {
            int intRunNo = -1;
            string[] parstrPayCategoryNo = parstrPayCategoryNos.Split('|');
            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();

            //2017-08-12
            strQry.Clear();

            strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.CLOSE_RUN_QUEUE ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            if (parstrPayrollType == "T")
            {
                strQry.AppendLine(" AND RUN_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            }
            else
            {
                strQry.AppendLine(" AND RUN_TYPE IN ('B','" + parstrPayrollType + "')");
            }

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            //2017-08-12
            if (parstrPayrollType != "S")
            {
                strQry.Clear();

                strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.PAYROLL_RUN_QUEUE ");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            }

            //2017-08-10
            strQry.Clear();

            strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.OPEN_RUN_QUEUE ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            
            //2017-08-02
            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.USER_LOGS ");
            strQry.AppendLine("(USER_NO");
            strQry.AppendLine(",LOG_DATETIME");
            strQry.AppendLine(",USER_LOG_PROCEDURE_NO");
            strQry.AppendLine(",COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PAY_CATEGORY_NUMBERS");
            strQry.AppendLine(",PAYROLL_RUNDATE)");

            strQry.AppendLine(" VALUES ");

            strQry.AppendLine("(" + parint64User.ToString());
            strQry.AppendLine(",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
          
            //1=Delete Open Wage Run Date
            //2=Delete Open Salary Run Date
            //3=Delete Open Time Attendance Run Date

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(",1");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(",2");
                }
                else
                {
                    //Time Attendance
                    strQry.AppendLine(",3");
                }
            }

            strQry.AppendLine("," + parint64CompanyNo.ToString());
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryNos.Replace("|",",").ToString()));
            strQry.AppendLine(",'" + parCurrentDateTime.ToString("yyyy-MM-dd") + "')");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            
            if (parstrPayCategoryNo.Length > 0)
            {
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" RUN_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_PERIOD_DATE = '" + parCurrentDateTime.ToString("yyyy-MM-dd") + "'");
                //2017-04-24
                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parstrPayCategoryNo[0]); 

                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND RUN_TYPE = 'P'");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "RunNo", parint64CompanyNo);

                if (DataSet.Tables["RunNo"].Rows.Count > 0)
                {
                    intRunNo = Convert.ToInt32(DataSet.Tables["RunNo"].Rows[0]["RUN_NO"]);
                }
               
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                //2017-04-24
                //strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parstrPayCategoryNo[intRow]);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND RUN_NO = " + intRunNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                //2017-04-24
                //strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parstrPayCategoryNo[intRow]);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND RUN_TYPE = 'P'");
                strQry.AppendLine(" AND RUN_NO = " + intRunNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT");
                strQry.AppendLine("	WHERE COMPANY_NO = " + parint64CompanyNo);
                //2017-04-24
                //strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parstrPayCategoryNo[intRow]);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND RUN_TYPE = 'P'");
                strQry.AppendLine(" AND RUN_NO = " + intRunNo);
                //2017-04-24
                //strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                //strQry.AppendLine("(SELECT EMPLOYEE_NO");

                //strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT ");

                //strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                //strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parstrPayCategoryNo[intRow]);
                //strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                //strQry.AppendLine(" AND RUN_TYPE = 'P'");
                //strQry.AppendLine(" AND RUN_NO = " + intRunNo + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND RUN_NO = " + intRunNo);
                //2017-04-24
                //strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                //strQry.AppendLine("(SELECT EMPLOYEE_NO");
                
                //strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT ");
                
                //strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                //strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parstrPayCategoryNo[intRow]);
                //strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                //strQry.AppendLine(" AND RUN_TYPE = 'P'");
                //strQry.AppendLine(" AND RUN_NO = " + intRunNo + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_WEEK_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND RUN_NO = " + intRunNo);
                 //2017-04-24
                strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                strQry.AppendLine("(SELECT EMPLOYEE_NO");
                
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT ");
                
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                //2017-04-24
                //strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parstrPayCategoryNo[intRow]);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND RUN_TYPE = 'P'");
                strQry.AppendLine(" AND RUN_NO = " + intRunNo + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT");
                strQry.AppendLine("	WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND RUN_TYPE = 'P'");
                strQry.AppendLine(" AND RUN_NO = " + intRunNo);
                //2017-04-24
                //strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                //strQry.AppendLine("(SELECT EMPLOYEE_NO");

                //strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT ");

                //strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                //strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parstrPayCategoryNo[intRow]);
                //strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                //strQry.AppendLine(" AND RUN_TYPE = 'P'");
                //strQry.AppendLine(" AND RUN_NO = " + intRunNo + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT");
                strQry.AppendLine("	WHERE COMPANY_NO = " + parint64CompanyNo);
                //2017-04-24
                //strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parstrPayCategoryNo[intRow]);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND RUN_TYPE = 'P'");
                strQry.AppendLine(" AND RUN_NO = " + intRunNo);
                //2017-04-24
                //strQry.AppendLine(" AND EMPLOYEE_NO IN ");
                //strQry.AppendLine("(SELECT EMPLOYEE_NO");

                //strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT ");

                //strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                //strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parstrPayCategoryNo[intRow]);
                //strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                //strQry.AppendLine(" AND RUN_TYPE = 'P'");
                //strQry.AppendLine(" AND RUN_NO = " + intRunNo + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                //2017-04-24
                //strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parstrPayCategoryNo[intRow]);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND RUN_NO = " + intRunNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                //2017-04-24
                //strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parstrPayCategoryNo[intRow]);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND RUN_TYPE = 'P'");
                strQry.AppendLine(" AND RUN_NO = " + intRunNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND RUN_NO = " + intRunNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                //Errol - 2016-07-07
                if (parstrPayrollType != "S")
                {
                    strQry.Clear();
                    strQry.AppendLine(" DELETE PHC");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_CURRENT PHC");

                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");
                    strQry.AppendLine(" ON PHC.COMPANY_NO = PCPC.COMPANY_NO");
                    strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE <> " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

                    strQry.AppendLine(" WHERE PHC.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PHC.RUN_NO = " + intRunNo);
                    //NO Records Exists for Other PAY_CATEGORY_TYPE type
                    strQry.AppendLine(" AND PCPC.COMPANY_NO IS NULL");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                }
            }

            //2017-07-11
            strQry.Clear();

            strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.PAYROLL_RUN_QUEUE");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            
            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.COMPANY ");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" SET WAGE_RUN_IND = NULL ");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" SET SALARY_RUN_IND = NULL ");
                }
                else
                {
                    strQry.AppendLine(" SET TIME_ATTENDANCE_RUN_IND = NULL ");

                }
            }

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
          
            byte[] bytCompress = Get_Employee_WageRun(parint64CompanyNo, parstrPayrollType);

            DataSet TempDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(bytCompress);

            bytCompress = clsDBConnectionObjects.Compress_DataSet(TempDataSet);
            TempDataSet.Dispose();
            TempDataSet = null;

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            return bytCompress;
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

                    string subject = "Open Payroll/Time Attendance Run Error - " + DateTime.Now.ToString("dd MMMM yyyy");

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
