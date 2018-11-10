using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;

namespace InteractPayroll
{
    public class busBackupRestoreDatabase
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        int intTimeout = 60;

        public busBackupRestoreDatabase()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public int Backup_DataBase(Int64 parint64CompanyNo)
        {
            int intReturnCode = 9;

            try
            {
                StringBuilder strQry = new StringBuilder();
                DataSet DataSet = new DataSet();

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" ISNULL(TIMESHEET_READ_TIMEOUT_SECONDS,60) AS TIMESHEET_READ_TIMEOUT_SECONDS ");
                strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND NOT DYNAMIC_UPLOAD_KEY IS NULL");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Check", -1);

                if (DataSet.Tables["Check"].Rows.Count > 0)
                {
                    intTimeout = Convert.ToInt32(DataSet.Tables["Check"].Rows[0]["TIMESHEET_READ_TIMEOUT_SECONDS"]); 
                }

                strQry.Clear();

                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" BACKUP_DATABASE_PATH");
                strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_PATH");
                                
                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Directory", -1);

                string strFileDirectory = DataSet.Tables["Directory"].Rows[0]["BACKUP_DATABASE_PATH"].ToString();
                    
                string strDataBaseName = "InteractPayroll_" + parint64CompanyNo.ToString("00000");
                string strBackupFileName = strDataBaseName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_AdHoc.bak";

                strQry.Clear();

                strQry.AppendLine("BACKUP DATABASE " + strDataBaseName + " TO DISK = '" + strFileDirectory + "\\" + strBackupFileName + "' WITH CHECKSUM ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1, intTimeout);

                strQry.Clear();

                strQry.AppendLine("INSERT INTO InteractPayroll.dbo.BACKUP_DATABASE_DATETIME");
                strQry.AppendLine("(BACKUP_DATABASE_NO");
                strQry.AppendLine(",BACKUP_DATABASE_NAME");
                strQry.AppendLine(",PAYROLL_RUN_DATETIME");
                strQry.AppendLine(",BACKUP_DATETIME");
                strQry.AppendLine(",BACKUP_FILE_NAME)");
                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" ISNULL(MAX(BACKUP_DATABASE_NO),0) + 1");
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strDataBaseName));
                strQry.AppendLine(",GETDATE()");
                strQry.AppendLine(",GETDATE()");
                strQry.AppendLine(",'" + strFileDirectory + "\\" + strBackupFileName + "'");
                strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_DATETIME");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                intReturnCode = 0;
            }
            catch (Exception ex)
            {
                string strStop = "";
            }

            return intReturnCode;
        }

        public void Delete_Backup_File(string parstrFileName)
        {
            StringBuilder strQry = new StringBuilder();
 
            strQry.Clear();

            strQry.AppendLine("DELETE FROM InteractPayroll.dbo.BACKUP_DATABASE_DATETIME ");
            strQry.AppendLine("WHERE BACKUP_FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
            
            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            File.Delete(parstrFileName);
        }

        public int Backup_DataBase_Before_Restore(Int64 parint64CompanyNo)
        {
            int intReturnCode = 9;

            try
            {
                StringBuilder strQry = new StringBuilder();
                DataSet DataSet = new DataSet();

                strQry.Clear();

                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" BACKUP_DATABASE_PATH");
                strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_PATH");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Directory", -1);

                string strFileDirectory = DataSet.Tables["Directory"].Rows[0]["BACKUP_DATABASE_PATH"].ToString();

                string strDataBaseName = "InteractPayroll_" + parint64CompanyNo.ToString("00000");
                string strBackupFileName = strDataBaseName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_Backup_Before_Restore.bak";

                strQry.Clear();

                strQry.AppendLine("BACKUP DATABASE " + strDataBaseName + " TO DISK = '" + strFileDirectory + "\\" + strBackupFileName + "' WITH CHECKSUM");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.BACKUP_DATABASE_DATETIME");
                strQry.AppendLine("(BACKUP_DATABASE_NO");
                strQry.AppendLine(",BACKUP_DATABASE_NAME");
                strQry.AppendLine(",PAYROLL_RUN_DATETIME");
                strQry.AppendLine(",BACKUP_DATETIME");
                strQry.AppendLine(",BACKUP_FILE_NAME)");
                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" ISNULL(MAX(BACKUP_DATABASE_NO),0) + 1");
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strDataBaseName));
                strQry.AppendLine(",GETDATE()");
                strQry.AppendLine(",GETDATE()");
                strQry.AppendLine(",'" + strFileDirectory + "\\" + strBackupFileName + "'");
                strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_DATETIME");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                intReturnCode = 0;
            }
            catch (Exception ex)
            {
                string strStop = "";
            }

            return intReturnCode;
        }

        public int Restore_DataBase(Int64 parint64CompanyNo, string parstrFileName, Int64 parInt64CurrentUser, string parstrCopyTimeSheetsOver)
        {
            int intReturnCode = 1;
            string strLogPath = "";
         
            try
            {
#if(DEBUG)
#else
                strLogPath = System.Web.HttpContext.Current.Server.MapPath("RestoreDataBaseLog.txt");

                using (StreamWriter writeLog = new StreamWriter(strLogPath,true))
                {
                    writeLog.WriteLine("Start of Restore database for Company " + parint64CompanyNo.ToString() + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
#endif
                StringBuilder strQry = new StringBuilder();
                string strCurrenDate = DateTime.Now.ToString("yyyy-MM-dd");

                DataSet DataSet = new System.Data.DataSet();
                
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" COMPANY_NO ");
                strQry.AppendLine(",ISNULL(TIMESHEET_READ_TIMEOUT_SECONDS,60) AS TIMESHEET_READ_TIMEOUT_SECONDS ");
                strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND NOT DYNAMIC_UPLOAD_KEY IS NULL");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Check", -1);

                if (DataSet.Tables["Check"].Rows.Count > 0
                && parstrCopyTimeSheetsOver == "Y")
                {
                    intTimeout = Convert.ToInt32(DataSet.Tables["Check"].Rows[0]["TIMESHEET_READ_TIMEOUT_SECONDS"]); 
                    
                    //Dynamic Upload of Timesheets
                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.PAY_CATEGORY_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + parInt64CurrentUser);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.EMPLOYEE_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + parInt64CurrentUser);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + parInt64CurrentUser);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                    
                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + parInt64CurrentUser);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                    
                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + parInt64CurrentUser);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.EMPLOYEE_TIMESHEET_CURRENT_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + parInt64CurrentUser);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.EMPLOYEE_BREAK_CURRENT_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + parInt64CurrentUser);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + parInt64CurrentUser);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.EMPLOYEE_SALARY_BREAK_CURRENT_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + parInt64CurrentUser);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + parInt64CurrentUser);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + parInt64CurrentUser);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.PAY_CATEGORY_TEMP ");
                    strQry.AppendLine("(USER_NO");
                    strQry.AppendLine(",COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",LAST_UPLOAD_DATETIME)");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(parInt64CurrentUser.ToString());
                    strQry.AppendLine(",COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",LAST_UPLOAD_DATETIME");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY ");
                    strQry.AppendLine(" WHERE PAY_CATEGORY_NO > 0 ");
                    strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);
                    
                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.EMPLOYEE_TEMP ");
                    strQry.AppendLine("(USER_NO");
                    strQry.AppendLine(",COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(",FIRST_RUN_COMPLETED_IND)");
                    
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(parInt64CurrentUser.ToString());
                    strQry.AppendLine(",COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",ISNULL(EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrenDate + "'))");
                    strQry.AppendLine(",FIRST_RUN_COMPLETED_IND");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE ");
                    strQry.AppendLine(" WHERE DATETIME_DELETE_RECORD IS NULL ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT_TEMP ");
                    strQry.AppendLine("(USER_NO");
                    strQry.AppendLine(",COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",TIMESHEET_DATE");
                    strQry.AppendLine(",DAY_NO");
                    strQry.AppendLine(",DAY_PAID_MINUTES");
                    strQry.AppendLine(",BREAK_ACCUM_MINUTES");
                    strQry.AppendLine(",INDICATOR");
                    strQry.AppendLine(",BREAK_INDICATOR");
                    strQry.AppendLine(",INCLUDED_IN_RUN_INDICATOR)");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(parInt64CurrentUser.ToString());
                    strQry.AppendLine(",COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",TIMESHEET_DATE");
                    strQry.AppendLine(",DAY_NO");
                    strQry.AppendLine(",DAY_PAID_MINUTES");
                    strQry.AppendLine(",BREAK_ACCUM_MINUTES");
                    strQry.AppendLine(",INDICATOR");
                    strQry.AppendLine(",BREAK_INDICATOR");
                    strQry.AppendLine(",INCLUDED_IN_RUN_INDICATOR");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_TEMP ");
                    strQry.AppendLine("(USER_NO");
                    strQry.AppendLine(",COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",TIMESHEET_DATE");
                    strQry.AppendLine(",DAY_NO");
                    strQry.AppendLine(",DAY_PAID_MINUTES");
                    strQry.AppendLine(",BREAK_ACCUM_MINUTES");
                    strQry.AppendLine(",INDICATOR");
                    strQry.AppendLine(",BREAK_INDICATOR");
                    strQry.AppendLine(",INCLUDED_IN_RUN_INDICATOR)");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(parInt64CurrentUser.ToString());
                    strQry.AppendLine(",COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",TIMESHEET_DATE");
                    strQry.AppendLine(",DAY_NO");
                    strQry.AppendLine(",DAY_PAID_MINUTES");
                    strQry.AppendLine(",BREAK_ACCUM_MINUTES");
                    strQry.AppendLine(",INDICATOR");
                    strQry.AppendLine(",BREAK_INDICATOR");
                    strQry.AppendLine(",INCLUDED_IN_RUN_INDICATOR");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_TEMP ");
                    strQry.AppendLine("(USER_NO");
                    strQry.AppendLine(",COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",TIMESHEET_DATE");
                    strQry.AppendLine(",DAY_NO");
                    strQry.AppendLine(",DAY_PAID_MINUTES");
                    strQry.AppendLine(",BREAK_ACCUM_MINUTES");
                    strQry.AppendLine(",INDICATOR");
                    strQry.AppendLine(",BREAK_INDICATOR");
                    strQry.AppendLine(",INCLUDED_IN_RUN_INDICATOR)");

                    strQry.AppendLine(" SELECT ");
                     strQry.AppendLine(parInt64CurrentUser.ToString());
                    strQry.AppendLine(",COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",TIMESHEET_DATE");
                    strQry.AppendLine(",DAY_NO");
                    strQry.AppendLine(",DAY_PAID_MINUTES");
                    strQry.AppendLine(",BREAK_ACCUM_MINUTES");
                    strQry.AppendLine(",INDICATOR");
                    strQry.AppendLine(",BREAK_INDICATOR");
                    strQry.AppendLine(",INCLUDED_IN_RUN_INDICATOR");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.EMPLOYEE_TIMESHEET_CURRENT_TEMP ");
                    strQry.AppendLine("(USER_NO");
                    strQry.AppendLine(",COMPANY_NO");
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
                    strQry.AppendLine(parInt64CurrentUser.ToString());
                    strQry.AppendLine(",COMPANY_NO");
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
                    strQry.AppendLine(",USER_NO_TIME_OUT");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.EMPLOYEE_BREAK_CURRENT_TEMP ");
                    strQry.AppendLine("(USER_NO");
                    strQry.AppendLine(",COMPANY_NO");
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
                    strQry.AppendLine(parInt64CurrentUser.ToString());
                    strQry.AppendLine(",COMPANY_NO");
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
                    strQry.AppendLine(",USER_NO_TIME_OUT");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT_TEMP ");
                    strQry.AppendLine("(USER_NO");
                    strQry.AppendLine(",COMPANY_NO");
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
                    strQry.AppendLine(parInt64CurrentUser.ToString());
                    strQry.AppendLine(",COMPANY_NO");
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
                    strQry.AppendLine(",USER_NO_TIME_OUT");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.EMPLOYEE_SALARY_BREAK_CURRENT_TEMP ");
                    strQry.AppendLine("(USER_NO");
                    strQry.AppendLine(",COMPANY_NO");
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
                    strQry.AppendLine(parInt64CurrentUser.ToString());
                    strQry.AppendLine(",COMPANY_NO");
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
                    strQry.AppendLine(",USER_NO_TIME_OUT");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT_TEMP ");
                    strQry.AppendLine("(USER_NO");
                    strQry.AppendLine(",COMPANY_NO");
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
                    strQry.AppendLine(parInt64CurrentUser.ToString());
                    strQry.AppendLine(",COMPANY_NO");
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
                    strQry.AppendLine(",USER_NO_TIME_OUT");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT_TEMP ");
                    strQry.AppendLine("(USER_NO");
                    strQry.AppendLine(",COMPANY_NO");
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
                    strQry.AppendLine(parInt64CurrentUser.ToString());
                    strQry.AppendLine(",COMPANY_NO");
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
                    strQry.AppendLine(",USER_NO_TIME_OUT");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);
                }
              
                string strDataBaseName = "InteractPayroll_" + parint64CompanyNo.ToString("00000");

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" SPID");

                strQry.AppendLine(" FROM MASTER.dbo.SYSPROCESSES");

                strQry.AppendLine(" WHERE DBID = DB_ID('" + strDataBaseName + "')");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" SPID DESC");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "SpidTable", parint64CompanyNo);

                //Kill System Process IDs (SPIDs)
                for (int intRow = 0; intRow < DataSet.Tables["SpidTable"].Rows.Count; intRow++)
                {
                    strQry.Clear();

                    strQry.AppendLine(" KILL " + DataSet.Tables["SpidTable"].Rows[intRow]["SPID"].ToString());

                    try
                    {
                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, 60);
                    }
                    catch
                    {
                    }
                }

                strQry.Clear();

                strQry.AppendLine("RESTORE DATABASE " + strDataBaseName + " FROM DISK = '" + parstrFileName + "' WITH REPLACE");

                clsDBConnectionObjects.Execute_SQLCommand_Restore(strQry.ToString(), -1,60);

                if (DataSet.Tables["Check"].Rows.Count > 0
                && parstrCopyTimeSheetsOver == "Y")
                {
                    //Dynamic Upload of Timesheets
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE PC ");
                    strQry.AppendLine(" SET PC.LAST_UPLOAD_DATETIME = PCT.LAST_UPLOAD_DATETIME ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.PAY_CATEGORY_TEMP PCT ");
                    strQry.AppendLine(" ON PCT.USER_NO = " + parInt64CurrentUser.ToString());
                    strQry.AppendLine(" AND PC.COMPANY_NO = PCT.COMPANY_NO");
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = PCT.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = PCT.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);

                    //TIME_ATTEND has been Fixed
                    strQry.Clear();

                    strQry.AppendLine(" DELETE ETATC ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT ETATC ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.EMPLOYEE_TEMP ET ");

                    strQry.AppendLine(" ON ET.USER_NO = " + parInt64CurrentUser);
                    strQry.AppendLine(" AND ETATC.COMPANY_NO = ET.COMPANY_NO");
                    strQry.AppendLine(" AND ETATC.EMPLOYEE_NO = ET.EMPLOYEE_NO");
                    strQry.AppendLine(" AND ET.PAY_CATEGORY_TYPE = 'T' ");
                    strQry.AppendLine(" AND ((ETATC.TIMESHEET_DATE > ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ET.FIRST_RUN_COMPLETED_IND = 'Y')");
                    strQry.AppendLine(" OR (ETATC.TIMESHEET_DATE >= ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ISNULL(ET.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                    strQry.AppendLine(" WHERE ETATC.COMPANY_NO = " + parint64CompanyNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT ");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",TIMESHEET_DATE");
                    strQry.AppendLine(",DAY_NO");
                    strQry.AppendLine(",DAY_PAID_MINUTES");
                    strQry.AppendLine(",BREAK_ACCUM_MINUTES");
                    strQry.AppendLine(",INDICATOR");
                    strQry.AppendLine(",BREAK_INDICATOR");
                    strQry.AppendLine(",INCLUDED_IN_RUN_INDICATOR)");

                    strQry.AppendLine(" SELECT ");

                    strQry.AppendLine(" ETATCT.COMPANY_NO");
                    strQry.AppendLine(",ETATCT.EMPLOYEE_NO");
                    strQry.AppendLine(",ETATCT.PAY_CATEGORY_NO");
                    strQry.AppendLine(",ETATCT.TIMESHEET_DATE");
                    strQry.AppendLine(",ETATCT.DAY_NO");
                    strQry.AppendLine(",ETATCT.DAY_PAID_MINUTES");
                    strQry.AppendLine(",ETATCT.BREAK_ACCUM_MINUTES");
                    strQry.AppendLine(",ETATCT.INDICATOR");
                    strQry.AppendLine(",ETATCT.BREAK_INDICATOR");
                    strQry.AppendLine(",ETATCT.INCLUDED_IN_RUN_INDICATOR");

                    strQry.AppendLine(" FROM InteractPayroll.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_TEMP ETATCT ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON ETATCT.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND ETATCT.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.EMPLOYEE_TEMP ET ");

                    strQry.AppendLine(" ON ETATCT.USER_NO = ET.USER_NO");
                    strQry.AppendLine(" AND ETATCT.COMPANY_NO = ET.COMPANY_NO");
                    strQry.AppendLine(" AND ETATCT.EMPLOYEE_NO = ET.EMPLOYEE_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = ET.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND ((ETATCT.TIMESHEET_DATE > ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ET.FIRST_RUN_COMPLETED_IND = 'Y')");
                    strQry.AppendLine(" OR (ETATCT.TIMESHEET_DATE >= ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ISNULL(ET.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                    strQry.AppendLine(" WHERE ETATCT.USER_NO = " + parInt64CurrentUser);
                    strQry.AppendLine(" AND ETATCT.COMPANY_NO = " + parint64CompanyNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);

                    strQry.Clear();

                    strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ");
                    strQry.AppendLine(" DISABLE TRIGGER tgr_EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();

                    strQry.AppendLine(" DELETE ETATC ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETATC ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.EMPLOYEE_TEMP ET ");

                    strQry.AppendLine(" ON ET.USER_NO = " + parInt64CurrentUser);
                    strQry.AppendLine(" AND ETATC.COMPANY_NO = ET.COMPANY_NO");
                    strQry.AppendLine(" AND ETATC.EMPLOYEE_NO = ET.EMPLOYEE_NO");
                    strQry.AppendLine(" AND ET.PAY_CATEGORY_TYPE = 'T' ");
                    strQry.AppendLine(" AND ((ETATC.TIMESHEET_DATE > ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ET.FIRST_RUN_COMPLETED_IND = 'Y')");
                    strQry.AppendLine(" OR (ETATC.TIMESHEET_DATE >= ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ISNULL(ET.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                    strQry.AppendLine(" WHERE ETATC.COMPANY_NO = " + parint64CompanyNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ");
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
                    strQry.AppendLine(",TIMESHEET_ACCUM_MINUTES)");

                    strQry.AppendLine(" SELECT ");

                    strQry.AppendLine(" ETATCT.COMPANY_NO");
                    strQry.AppendLine(",ETATCT.EMPLOYEE_NO");
                    strQry.AppendLine(",ETATCT.PAY_CATEGORY_NO");
                    strQry.AppendLine(",ETATCT.TIMESHEET_DATE");
                    strQry.AppendLine(",ETATCT.TIMESHEET_SEQ");
                    strQry.AppendLine(",ETATCT.TIMESHEET_TIME_IN_MINUTES");
                    strQry.AppendLine(",ETATCT.TIMESHEET_TIME_OUT_MINUTES");
                    strQry.AppendLine(",ETATCT.CLOCKED_TIME_IN_MINUTES");
                    strQry.AppendLine(",ETATCT.CLOCKED_TIME_OUT_MINUTES");
                    strQry.AppendLine(",ETATCT.INCLUDED_IN_RUN_IND");
                    strQry.AppendLine(",ETATCT.INDICATOR");
                    strQry.AppendLine(",ETATCT.TIMESHEET_ACCUM_MINUTES");

                    strQry.AppendLine(" FROM InteractPayroll.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT_TEMP ETATCT ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON ETATCT.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND ETATCT.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.EMPLOYEE_TEMP ET ");

                    strQry.AppendLine(" ON ETATCT.USER_NO = ET.USER_NO");
                    strQry.AppendLine(" AND ETATCT.COMPANY_NO = ET.COMPANY_NO");
                    strQry.AppendLine(" AND ETATCT.EMPLOYEE_NO = ET.EMPLOYEE_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = ET.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND ((ETATCT.TIMESHEET_DATE > ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ET.FIRST_RUN_COMPLETED_IND = 'Y')");
                    strQry.AppendLine(" OR (ETATCT.TIMESHEET_DATE >= ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ISNULL(ET.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                    strQry.AppendLine(" WHERE ETATCT.USER_NO = " + parInt64CurrentUser);
                    strQry.AppendLine(" AND ETATCT.COMPANY_NO = " + parint64CompanyNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);

                    strQry.Clear();

                    strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ");
                    strQry.AppendLine(" ENABLE TRIGGER tgr_EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();

                    strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ");
                    strQry.AppendLine(" DISABLE TRIGGER tgr_EMPLOYEE_TIME_ATTEND_BREAK_CURRENT_Maintain_EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();

                    strQry.AppendLine(" DELETE ETATC ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETATC ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.EMPLOYEE_TEMP ET ");

                    strQry.AppendLine(" ON ET.USER_NO = " + parInt64CurrentUser);
                    strQry.AppendLine(" AND ETATC.COMPANY_NO = ET.COMPANY_NO");
                    strQry.AppendLine(" AND ETATC.EMPLOYEE_NO = ET.EMPLOYEE_NO");
                    strQry.AppendLine(" AND ET.PAY_CATEGORY_TYPE = 'T' ");
                    strQry.AppendLine(" AND ((ETATC.BREAK_DATE > ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ET.FIRST_RUN_COMPLETED_IND = 'Y')");
                    strQry.AppendLine(" OR (ETATC.BREAK_DATE >= ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ISNULL(ET.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                    strQry.AppendLine(" WHERE ETATC.COMPANY_NO = " + parint64CompanyNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ");
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
                    strQry.AppendLine(",BREAK_ACCUM_MINUTES)");

                    strQry.AppendLine(" SELECT ");

                    strQry.AppendLine(" ETATCT.COMPANY_NO");
                    strQry.AppendLine(",ETATCT.EMPLOYEE_NO");
                    strQry.AppendLine(",ETATCT.PAY_CATEGORY_NO");
                    strQry.AppendLine(",ETATCT.BREAK_DATE");
                    strQry.AppendLine(",ETATCT.BREAK_SEQ");
                    strQry.AppendLine(",ETATCT.BREAK_TIME_IN_MINUTES");
                    strQry.AppendLine(",ETATCT.BREAK_TIME_OUT_MINUTES");
                    strQry.AppendLine(",ETATCT.CLOCKED_TIME_IN_MINUTES");
                    strQry.AppendLine(",ETATCT.CLOCKED_TIME_OUT_MINUTES");
                    strQry.AppendLine(",ETATCT.INCLUDED_IN_RUN_IND");
                    strQry.AppendLine(",ETATCT.INDICATOR");
                    strQry.AppendLine(",ETATCT.BREAK_ACCUM_MINUTES");

                    strQry.AppendLine(" FROM InteractPayroll.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT_TEMP ETATCT ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON ETATCT.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND ETATCT.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.EMPLOYEE_TEMP ET ");
                    strQry.AppendLine(" ON ETATCT.USER_NO = ET.USER_NO");
                    strQry.AppendLine(" AND ETATCT.COMPANY_NO = ET.COMPANY_NO");
                    strQry.AppendLine(" AND ETATCT.EMPLOYEE_NO = ET.EMPLOYEE_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = ET.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND ((ETATCT.BREAK_DATE > ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ET.FIRST_RUN_COMPLETED_IND = 'Y')");
                    strQry.AppendLine(" OR (ETATCT.BREAK_DATE >= ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ISNULL(ET.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                    strQry.AppendLine(" WHERE ETATCT.USER_NO = " + parInt64CurrentUser);
                    strQry.AppendLine(" AND ETATCT.COMPANY_NO = " + parint64CompanyNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);

                    strQry.Clear();

                    strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ");
                    strQry.AppendLine(" ENABLE TRIGGER tgr_EMPLOYEE_TIME_ATTEND_BREAK_CURRENT_Maintain_EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                    //TIME_ATTEND has been Fixed

                    //TIMESHEET has been Fixed
                    strQry.Clear();

                    strQry.AppendLine(" DELETE ETATC ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT ETATC ");
                    
                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.EMPLOYEE_TEMP ET ");
                    strQry.AppendLine(" ON ET.USER_NO = " + parInt64CurrentUser);
                    strQry.AppendLine(" AND ETATC.COMPANY_NO = ET.COMPANY_NO");
                    strQry.AppendLine(" AND ETATC.EMPLOYEE_NO = ET.EMPLOYEE_NO");
                    strQry.AppendLine(" AND ET.PAY_CATEGORY_TYPE = 'W' ");
                    strQry.AppendLine(" AND ((ETATC.TIMESHEET_DATE > ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ET.FIRST_RUN_COMPLETED_IND = 'Y')");
                    strQry.AppendLine(" OR (ETATC.TIMESHEET_DATE >= ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ISNULL(ET.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                    strQry.AppendLine(" WHERE ETATC.COMPANY_NO = " + parint64CompanyNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT ");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",TIMESHEET_DATE");
                    strQry.AppendLine(",DAY_NO");
                    strQry.AppendLine(",DAY_PAID_MINUTES");
                    strQry.AppendLine(",BREAK_ACCUM_MINUTES");
                    strQry.AppendLine(",INDICATOR");
                    strQry.AppendLine(",BREAK_INDICATOR");
                    strQry.AppendLine(",INCLUDED_IN_RUN_INDICATOR)");

                    strQry.AppendLine(" SELECT ");

                    strQry.AppendLine(" ETATCT.COMPANY_NO");
                    strQry.AppendLine(",ETATCT.EMPLOYEE_NO");
                    strQry.AppendLine(",ETATCT.PAY_CATEGORY_NO");
                    strQry.AppendLine(",ETATCT.TIMESHEET_DATE");
                    strQry.AppendLine(",ETATCT.DAY_NO");
                    strQry.AppendLine(",ETATCT.DAY_PAID_MINUTES");
                    strQry.AppendLine(",ETATCT.BREAK_ACCUM_MINUTES");
                    strQry.AppendLine(",ETATCT.INDICATOR");
                    strQry.AppendLine(",ETATCT.BREAK_INDICATOR");
                    strQry.AppendLine(",ETATCT.INCLUDED_IN_RUN_INDICATOR");

                    strQry.AppendLine(" FROM InteractPayroll.dbo.EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_TEMP ETATCT ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON ETATCT.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND ETATCT.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.EMPLOYEE_TEMP ET ");
                    strQry.AppendLine(" ON ETATCT.USER_NO = ET.USER_NO");
                    strQry.AppendLine(" AND ETATCT.COMPANY_NO = ET.COMPANY_NO");
                    strQry.AppendLine(" AND ETATCT.EMPLOYEE_NO = ET.EMPLOYEE_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = ET.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND ((ETATCT.TIMESHEET_DATE > ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ET.FIRST_RUN_COMPLETED_IND = 'Y')");
                    strQry.AppendLine(" OR (ETATCT.TIMESHEET_DATE >= ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ISNULL(ET.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                    strQry.AppendLine(" WHERE ETATCT.USER_NO = " + parInt64CurrentUser);
                    strQry.AppendLine(" AND ETATCT.COMPANY_NO = " + parint64CompanyNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);

                    strQry.Clear();

                    strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ");
                    strQry.AppendLine(" DISABLE TRIGGER tgr_EMPLOYEE_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();

                    strQry.AppendLine(" DELETE ETATC ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETATC ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.EMPLOYEE_TEMP ET ");
                    strQry.AppendLine(" ON ET.USER_NO = " + parInt64CurrentUser);
                    strQry.AppendLine(" AND ETATC.COMPANY_NO = ET.COMPANY_NO");
                    strQry.AppendLine(" AND ETATC.EMPLOYEE_NO = ET.EMPLOYEE_NO");
                    strQry.AppendLine(" AND ET.PAY_CATEGORY_TYPE = 'W' ");
                    strQry.AppendLine(" AND ((ETATC.TIMESHEET_DATE > ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ET.FIRST_RUN_COMPLETED_IND = 'Y')");
                    strQry.AppendLine(" OR (ETATC.TIMESHEET_DATE >= ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ISNULL(ET.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                    strQry.AppendLine(" WHERE ETATC.COMPANY_NO = " + parint64CompanyNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ");
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
                    strQry.AppendLine(",TIMESHEET_ACCUM_MINUTES)");

                    strQry.AppendLine(" SELECT ");

                    strQry.AppendLine(" ETATCT.COMPANY_NO");
                    strQry.AppendLine(",ETATCT.EMPLOYEE_NO");
                    strQry.AppendLine(",ETATCT.PAY_CATEGORY_NO");
                    strQry.AppendLine(",ETATCT.TIMESHEET_DATE");
                    strQry.AppendLine(",ETATCT.TIMESHEET_SEQ");
                    strQry.AppendLine(",ETATCT.TIMESHEET_TIME_IN_MINUTES");
                    strQry.AppendLine(",ETATCT.TIMESHEET_TIME_OUT_MINUTES");
                    strQry.AppendLine(",ETATCT.CLOCKED_TIME_IN_MINUTES");
                    strQry.AppendLine(",ETATCT.CLOCKED_TIME_OUT_MINUTES");
                    strQry.AppendLine(",ETATCT.INCLUDED_IN_RUN_IND");
                    strQry.AppendLine(",ETATCT.INDICATOR");
                    strQry.AppendLine(",ETATCT.TIMESHEET_ACCUM_MINUTES");

                    strQry.AppendLine(" FROM InteractPayroll.dbo.EMPLOYEE_TIMESHEET_CURRENT_TEMP ETATCT ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON ETATCT.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND ETATCT.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.EMPLOYEE_TEMP ET ");
                    strQry.AppendLine(" ON ETATCT.USER_NO = ET.USER_NO");
                    strQry.AppendLine(" AND ETATCT.COMPANY_NO = ET.COMPANY_NO");
                    strQry.AppendLine(" AND ETATCT.EMPLOYEE_NO = ET.EMPLOYEE_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = ET.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND ((ETATCT.TIMESHEET_DATE > ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ET.FIRST_RUN_COMPLETED_IND = 'Y')");
                    strQry.AppendLine(" OR (ETATCT.TIMESHEET_DATE >= ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ISNULL(ET.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                    strQry.AppendLine(" WHERE ETATCT.USER_NO = " + parInt64CurrentUser);
                    strQry.AppendLine(" AND ETATCT.COMPANY_NO = " + parint64CompanyNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);
                    
                    strQry.Clear();

                    strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ");
                    strQry.AppendLine(" ENABLE TRIGGER tgr_EMPLOYEE_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();

                    strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT ");
                    strQry.AppendLine(" DISABLE TRIGGER tgr_EMPLOYEE_BREAK_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();

                    strQry.AppendLine(" DELETE ETATC ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT ETATC ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.EMPLOYEE_TEMP ET ");
                    strQry.AppendLine(" ON ET.USER_NO = " + parInt64CurrentUser);
                    strQry.AppendLine(" AND ETATC.COMPANY_NO = ET.COMPANY_NO");
                    strQry.AppendLine(" AND ETATC.EMPLOYEE_NO = ET.EMPLOYEE_NO");
                    strQry.AppendLine(" AND ET.PAY_CATEGORY_TYPE = 'W' ");
                    strQry.AppendLine(" AND ((ETATC.BREAK_DATE > ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ET.FIRST_RUN_COMPLETED_IND = 'Y')");
                    strQry.AppendLine(" OR (ETATC.BREAK_DATE >= ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ISNULL(ET.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                    strQry.AppendLine(" WHERE ETATC.COMPANY_NO = " + parint64CompanyNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT ");
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
                    strQry.AppendLine(",BREAK_ACCUM_MINUTES)");

                    strQry.AppendLine(" SELECT ");

                    strQry.AppendLine(" ETATCT.COMPANY_NO");
                    strQry.AppendLine(",ETATCT.EMPLOYEE_NO");
                    strQry.AppendLine(",ETATCT.PAY_CATEGORY_NO");
                    strQry.AppendLine(",ETATCT.BREAK_DATE");
                    strQry.AppendLine(",ETATCT.BREAK_SEQ");
                    strQry.AppendLine(",ETATCT.BREAK_TIME_IN_MINUTES");
                    strQry.AppendLine(",ETATCT.BREAK_TIME_OUT_MINUTES");
                    strQry.AppendLine(",ETATCT.CLOCKED_TIME_IN_MINUTES");
                    strQry.AppendLine(",ETATCT.CLOCKED_TIME_OUT_MINUTES");
                    strQry.AppendLine(",ETATCT.INCLUDED_IN_RUN_IND");
                    strQry.AppendLine(",ETATCT.INDICATOR");
                    strQry.AppendLine(",ETATCT.BREAK_ACCUM_MINUTES");

                    strQry.AppendLine(" FROM InteractPayroll.dbo.EMPLOYEE_BREAK_CURRENT_TEMP ETATCT ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON ETATCT.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND ETATCT.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.EMPLOYEE_TEMP ET ");
                    strQry.AppendLine(" ON ETATCT.USER_NO = ET.USER_NO");
                    strQry.AppendLine(" AND ETATCT.COMPANY_NO = ET.COMPANY_NO");
                    strQry.AppendLine(" AND ETATCT.EMPLOYEE_NO = ET.EMPLOYEE_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = ET.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND ((ETATCT.BREAK_DATE > ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ET.FIRST_RUN_COMPLETED_IND = 'Y')");
                    strQry.AppendLine(" OR (ETATCT.BREAK_DATE >= ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ISNULL(ET.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                    strQry.AppendLine(" WHERE ETATCT.USER_NO = " + parInt64CurrentUser);
                    strQry.AppendLine(" AND ETATCT.COMPANY_NO = " + parint64CompanyNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);

                    strQry.Clear();

                    strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT ");
                    strQry.AppendLine(" ENABLE TRIGGER tgr_EMPLOYEE_BREAK_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                    //TIMESHEET has been Fixed

                    //SALARY has been Fixed
                    strQry.Clear();

                    strQry.AppendLine(" DELETE ETATC ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT ETATC ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.EMPLOYEE_TEMP ET ");
                    strQry.AppendLine(" ON ET.USER_NO = " + parInt64CurrentUser);
                    strQry.AppendLine(" AND ETATC.COMPANY_NO = ET.COMPANY_NO");
                    strQry.AppendLine(" AND ETATC.EMPLOYEE_NO = ET.EMPLOYEE_NO");
                    strQry.AppendLine(" AND ET.PAY_CATEGORY_TYPE = 'S' ");
                    strQry.AppendLine(" AND ((ETATC.TIMESHEET_DATE > ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ET.FIRST_RUN_COMPLETED_IND = 'Y')");
                    strQry.AppendLine(" OR (ETATC.TIMESHEET_DATE >= ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ISNULL(ET.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                    strQry.AppendLine(" WHERE ETATC.COMPANY_NO = " + parint64CompanyNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT ");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",TIMESHEET_DATE");
                    strQry.AppendLine(",DAY_NO");
                    strQry.AppendLine(",DAY_PAID_MINUTES");
                    strQry.AppendLine(",BREAK_ACCUM_MINUTES");
                    strQry.AppendLine(",INDICATOR");
                    strQry.AppendLine(",BREAK_INDICATOR");
                    strQry.AppendLine(",INCLUDED_IN_RUN_INDICATOR)");

                    strQry.AppendLine(" SELECT ");

                    strQry.AppendLine(" ETATCT.COMPANY_NO");
                    strQry.AppendLine(",ETATCT.EMPLOYEE_NO");
                    strQry.AppendLine(",ETATCT.PAY_CATEGORY_NO");
                    strQry.AppendLine(",ETATCT.TIMESHEET_DATE");
                    strQry.AppendLine(",ETATCT.DAY_NO");
                    strQry.AppendLine(",ETATCT.DAY_PAID_MINUTES");
                    strQry.AppendLine(",ETATCT.BREAK_ACCUM_MINUTES");
                    strQry.AppendLine(",ETATCT.INDICATOR");
                    strQry.AppendLine(",ETATCT.BREAK_INDICATOR");
                    strQry.AppendLine(",ETATCT.INCLUDED_IN_RUN_INDICATOR");

                    strQry.AppendLine(" FROM InteractPayroll.dbo.EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT_TEMP ETATCT ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON ETATCT.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND ETATCT.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.EMPLOYEE_TEMP ET ");
                    strQry.AppendLine(" ON ETATCT.USER_NO = ET.USER_NO");
                    strQry.AppendLine(" AND ETATCT.COMPANY_NO = ET.COMPANY_NO");
                    strQry.AppendLine(" AND ETATCT.EMPLOYEE_NO = ET.EMPLOYEE_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = ET.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND ((ETATCT.TIMESHEET_DATE > ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ET.FIRST_RUN_COMPLETED_IND = 'Y')");
                    strQry.AppendLine(" OR (ETATCT.TIMESHEET_DATE >= ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ISNULL(ET.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                    strQry.AppendLine(" WHERE ETATCT.USER_NO = " + parInt64CurrentUser);
                    strQry.AppendLine(" AND ETATCT.COMPANY_NO = " + parint64CompanyNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);

                    strQry.Clear();

                    strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ");
                    strQry.AppendLine(" DISABLE TRIGGER tgr_EMPLOYEE_SALARY_TIMESHEET_CURRENT_Maintain_EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();

                    strQry.AppendLine(" DELETE ETATC ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETATC ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.EMPLOYEE_TEMP ET ");
                    strQry.AppendLine(" ON ET.USER_NO = " + parInt64CurrentUser);
                    strQry.AppendLine(" AND ETATC.COMPANY_NO = ET.COMPANY_NO");
                    strQry.AppendLine(" AND ETATC.EMPLOYEE_NO = ET.EMPLOYEE_NO");
                    strQry.AppendLine(" AND ET.PAY_CATEGORY_TYPE = 'S' ");
                    strQry.AppendLine(" AND ((ETATC.TIMESHEET_DATE > ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ET.FIRST_RUN_COMPLETED_IND = 'Y')");
                    strQry.AppendLine(" OR (ETATC.TIMESHEET_DATE >= ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ISNULL(ET.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                    strQry.AppendLine(" WHERE ETATC.COMPANY_NO = " + parint64CompanyNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ");
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
                    strQry.AppendLine(",TIMESHEET_ACCUM_MINUTES)");

                    strQry.AppendLine(" SELECT ");

                    strQry.AppendLine(" ETATCT.COMPANY_NO");
                    strQry.AppendLine(",ETATCT.EMPLOYEE_NO");
                    strQry.AppendLine(",ETATCT.PAY_CATEGORY_NO");
                    strQry.AppendLine(",ETATCT.TIMESHEET_DATE");
                    strQry.AppendLine(",ETATCT.TIMESHEET_SEQ");
                    strQry.AppendLine(",ETATCT.TIMESHEET_TIME_IN_MINUTES");
                    strQry.AppendLine(",ETATCT.TIMESHEET_TIME_OUT_MINUTES");
                    strQry.AppendLine(",ETATCT.CLOCKED_TIME_IN_MINUTES");
                    strQry.AppendLine(",ETATCT.CLOCKED_TIME_OUT_MINUTES");
                    strQry.AppendLine(",ETATCT.INCLUDED_IN_RUN_IND");
                    strQry.AppendLine(",ETATCT.INDICATOR");
                    strQry.AppendLine(",ETATCT.TIMESHEET_ACCUM_MINUTES");

                    strQry.AppendLine(" FROM InteractPayroll.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT_TEMP ETATCT ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON ETATCT.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND ETATCT.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.EMPLOYEE_TEMP ET ");
                    strQry.AppendLine(" ON ETATCT.USER_NO = ET.USER_NO");
                    strQry.AppendLine(" AND ETATCT.COMPANY_NO = ET.COMPANY_NO");
                    strQry.AppendLine(" AND ETATCT.EMPLOYEE_NO = ET.EMPLOYEE_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = ET.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND ((ETATCT.TIMESHEET_DATE > ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ET.FIRST_RUN_COMPLETED_IND = 'Y')");
                    strQry.AppendLine(" OR (ETATCT.TIMESHEET_DATE >= ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ISNULL(ET.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                    strQry.AppendLine(" WHERE ETATCT.USER_NO = " + parInt64CurrentUser);
                    strQry.AppendLine(" AND ETATCT.COMPANY_NO = " + parint64CompanyNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);
                    
                    strQry.Clear();

                    strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ");
                    strQry.AppendLine(" ENABLE TRIGGER tgr_EMPLOYEE_SALARY_TIMESHEET_CURRENT_Maintain_EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();

                    strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ");
                    strQry.AppendLine(" DISABLE TRIGGER tgr_EMPLOYEE_SALARY_BREAK_CURRENT_Maintain_EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();

                    strQry.AppendLine(" DELETE ETATC ");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ETATC ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.EMPLOYEE_TEMP ET ");
                    strQry.AppendLine(" ON ET.USER_NO = " + parInt64CurrentUser);
                    strQry.AppendLine(" AND ETATC.COMPANY_NO = ET.COMPANY_NO");
                    strQry.AppendLine(" AND ETATC.EMPLOYEE_NO = ET.EMPLOYEE_NO");
                    strQry.AppendLine(" AND ET.PAY_CATEGORY_TYPE = 'S' ");
                    strQry.AppendLine(" AND ((ETATC.BREAK_DATE > ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ET.FIRST_RUN_COMPLETED_IND = 'Y')");
                    strQry.AppendLine(" OR (ETATC.BREAK_DATE >= ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ISNULL(ET.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                    strQry.AppendLine(" WHERE ETATC.COMPANY_NO = " + parint64CompanyNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ");
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
                    strQry.AppendLine(",BREAK_ACCUM_MINUTES)");

                    strQry.AppendLine(" SELECT ");

                    strQry.AppendLine(" ETATCT.COMPANY_NO");
                    strQry.AppendLine(",ETATCT.EMPLOYEE_NO");
                    strQry.AppendLine(",ETATCT.PAY_CATEGORY_NO");
                    strQry.AppendLine(",ETATCT.BREAK_DATE");
                    strQry.AppendLine(",ETATCT.BREAK_SEQ");
                    strQry.AppendLine(",ETATCT.BREAK_TIME_IN_MINUTES");
                    strQry.AppendLine(",ETATCT.BREAK_TIME_OUT_MINUTES");
                    strQry.AppendLine(",ETATCT.CLOCKED_TIME_IN_MINUTES");
                    strQry.AppendLine(",ETATCT.CLOCKED_TIME_OUT_MINUTES");
                    strQry.AppendLine(",ETATCT.INCLUDED_IN_RUN_IND");
                    strQry.AppendLine(",ETATCT.INDICATOR");
                    strQry.AppendLine(",ETATCT.BREAK_ACCUM_MINUTES");

                    strQry.AppendLine(" FROM InteractPayroll.dbo.EMPLOYEE_SALARY_BREAK_CURRENT_TEMP ETATCT ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON ETATCT.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND ETATCT.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S' ");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.EMPLOYEE_TEMP ET ");
                    strQry.AppendLine(" ON ETATCT.USER_NO = ET.USER_NO");
                    strQry.AppendLine(" AND ETATCT.COMPANY_NO = ET.COMPANY_NO");
                    strQry.AppendLine(" AND ETATCT.EMPLOYEE_NO = ET.EMPLOYEE_NO");
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = ET.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND ((ETATCT.BREAK_DATE > ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ET.FIRST_RUN_COMPLETED_IND = 'Y')");
                    strQry.AppendLine(" OR (ETATCT.BREAK_DATE >= ET.EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(" AND ISNULL(ET.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

                    strQry.AppendLine(" WHERE ETATCT.USER_NO = " + parInt64CurrentUser);
                    strQry.AppendLine(" AND ETATCT.COMPANY_NO = " + parint64CompanyNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo, intTimeout);

                    strQry.Clear();

                    strQry.AppendLine(" ALTER TABLE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ");
                    strQry.AppendLine(" ENABLE TRIGGER tgr_EMPLOYEE_SALARY_BREAK_CURRENT_Maintain_EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                    //SALARY has been Fixed

                    //Cleanup
                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.PAY_CATEGORY_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + parInt64CurrentUser);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.EMPLOYEE_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + parInt64CurrentUser);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + parInt64CurrentUser);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + parInt64CurrentUser);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + parInt64CurrentUser);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.EMPLOYEE_TIMESHEET_CURRENT_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + parInt64CurrentUser);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.EMPLOYEE_BREAK_CURRENT_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + parInt64CurrentUser);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + parInt64CurrentUser);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.EMPLOYEE_SALARY_BREAK_CURRENT_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + parInt64CurrentUser);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + parInt64CurrentUser);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + parInt64CurrentUser);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                }
#if(DEBUG)
#else
                using (StreamWriter writeLog = new StreamWriter(strLogPath,true))
                {
                    writeLog.WriteLine("End of Restore Database for Company " + parint64CompanyNo.ToString() + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
#endif
                intReturnCode = 0;
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("timeout") > -1)
                {
                    intReturnCode = 9;
                }
#if(DEBUG)
#else
                using (StreamWriter writeLog = new StreamWriter(strLogPath,true))
                {
                    writeLog.WriteLine("Company " + parint64CompanyNo.ToString() + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Restore_Database Exception = " + ex.Message);
                }
#endif
            }

            return intReturnCode;
        }

        public byte[] Get_Restore_Files(Int64 parint64CompanyNo)
        {
            DataSet DataSet = new System.Data.DataSet();
            DataView DataView;
            DataRowView DataRowView;
            FileInfo fiFileInfo;
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();

            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" DYNAMIC_UPLOAD_KEY");
            strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND NOT DYNAMIC_UPLOAD_KEY IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyKey", -1);

            //string strDBEngine = System.Environment.MachineName + @"\SQLExpress");

            strQry.Clear();

            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" PAYROLL_RUN_DATETIME");
            strQry.AppendLine(",BACKUP_DATETIME");
            strQry.AppendLine(",BACKUP_FILE_NAME");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",'' AS FOUND_IND");
           
            strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_DATETIME");
            
            strQry.AppendLine(" WHERE BACKUP_DATABASE_NAME = 'InteractPayroll_" + parint64CompanyNo.ToString("00000") + "'");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "File", -1);
     
            strQry.Clear();

            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" BACKUP_DATABASE_PATH");
            strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_PATH");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Directory", -1);

            string strFileDirectory = DataSet.Tables["Directory"].Rows[0]["BACKUP_DATABASE_PATH"].ToString();

            string[] filePaths = Directory.GetFiles(@strFileDirectory, @"*_" + parint64CompanyNo.ToString("00000") + "*.bak");

            for (int intCount = 0; intCount < filePaths.Length; intCount++)
            {
                DataView = null;
                DataView = new DataView(DataSet.Tables["File"], "BACKUP_FILE_NAME = '" + filePaths[intCount].ToString() + "'", "", DataViewRowState.CurrentRows);

                if (DataView.Count == 0)
                {
                    DataRowView = DataView.AddNew();

                    DataRowView.BeginEdit();

                    DataRowView["BACKUP_FILE_NAME"] = filePaths[intCount];

                    fiFileInfo = new FileInfo(filePaths[intCount]);

                    DataRowView["BACKUP_DATETIME"] = fiFileInfo.LastWriteTime;
                    DataRowView["FOUND_IND"] = "Y";

                    DataRowView.EndEdit();
                }
                else
                {
                    DataView[0]["FOUND_IND"] = "Y";
                }
            }

            DataView = null;
            DataView = new DataView(DataSet.Tables["File"], "FOUND_IND = ''", "", DataViewRowState.CurrentRows);

            //DataBase out of Sync with Files on Disk
            for (int intRow = 0; intRow < DataView.Count; intRow++)
            {
                strQry.Clear();

                strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.BACKUP_DATABASE_DATETIME");
                strQry.AppendLine(" WHERE BACKUP_FILE_NAME = '" + DataView[intRow]["BACKUP_FILE_NAME"].ToString() + "'");
                strQry.AppendLine(" AND BACKUP_DATABASE_NAME = 'InteractPayroll_" + parint64CompanyNo.ToString("00000") + "'");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                DataView[intRow].Delete();

                intRow -= 1;
            }

            DataSet.AcceptChanges();
            
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
    }
}
