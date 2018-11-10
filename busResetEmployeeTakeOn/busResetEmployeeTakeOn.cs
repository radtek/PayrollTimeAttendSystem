using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

namespace InteractPayroll
{
    public class busResetEmployeeTakeOn
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busResetEmployeeTakeOn()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PC.COMPANY_NO ");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");
   
            strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PC.COMPANY_NO ");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategory", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.EMPLOYEE_NAME");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EPC.COMPANY_NO");
            strQry.AppendLine(",EPC.EMPLOYEE_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON EPC.COMPANY_NO = E.COMPANY_NO");
            strQry.AppendLine(" AND EPC.EMPLOYEE_NO = E.EMPLOYEE_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");

            strQry.AppendLine(" WHERE EPC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeePayCategory", parint64CompanyNo);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Reset_Employee_TakeOn(Int64 parint64CompanyNo, string parstrPayrollType, string parstrEmployeeNoIN)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" ISC1.TABLE_NAME");
            strQry.AppendLine(",ISC2.COLUMN_NAME");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.COLUMNS ISC1");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.COLUMNS ISC2");
            strQry.AppendLine(" ON ISC1.TABLE_NAME = ISC2.TABLE_NAME");
            strQry.AppendLine(" AND ISC2.COLUMN_NAME = 'PAY_CATEGORY_TYPE'");

            strQry.AppendLine(" WHERE ISC1.COLUMN_NAME = 'EMPLOYEE_NO'");
            strQry.AppendLine(" AND ISC1.TABLE_NAME LIKE '%_HISTORY%'");
          
            this.clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "History", parint64CompanyNo);

            for (int intRow = 0; intRow < DataSet.Tables["History"].Rows.Count; intRow++)
            {
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo." + DataSet.Tables["History"].Rows[intRow]["TABLE_NAME"].ToString());
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                if (DataSet.Tables["History"].Rows[intRow]["COLUMN_NAME"] != System.DBNull.Value)
                {
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                }
                
                strQry.AppendLine(" AND EMPLOYEE_NO IN " + parstrEmployeeNoIN);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            }

            strQry.Clear();
            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" ISC1.TABLE_NAME");
            strQry.AppendLine(",ISC2.COLUMN_NAME");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.COLUMNS ISC1");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.COLUMNS ISC2");
            strQry.AppendLine(" ON ISC1.TABLE_NAME = ISC2.TABLE_NAME");
            strQry.AppendLine(" AND ISC2.COLUMN_NAME = 'PAY_CATEGORY_TYPE'");

            strQry.AppendLine(" WHERE ISC1.COLUMN_NAME = 'EMPLOYEE_NO'");
            strQry.AppendLine(" AND ISC1.TABLE_NAME LIKE '%_CURRENT%'");
            strQry.AppendLine(" AND NOT ISC1.TABLE_NAME IN ('EMPLOYEE_TIMESHEET_CURRENT','EMPLOYEE_BREAK_CURRENT','EMPLOYEE_SALARY_TIMESHEET_CURRENT','EMPLOYEE_SALARY_BREAK_CURRENT')");
            
            this.clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Current", parint64CompanyNo);

            for (int intRow = 0; intRow < DataSet.Tables["Current"].Rows.Count; intRow++)
            {
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo." + DataSet.Tables["Current"].Rows[intRow]["TABLE_NAME"].ToString());
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                if (DataSet.Tables["Current"].Rows[intRow]["COLUMN_NAME"] != System.DBNull.Value)
                {
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                }
                
                strQry.AppendLine(" AND EMPLOYEE_NO IN " + parstrEmployeeNoIN);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            }

            //Initialise Tax StartDate
            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" EMPLOYEE_TAKEON_IND = NULL");
            strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = NULL");
            strQry.AppendLine(",EMPLOYEE_TAX_STARTDATE = NULL");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(" AND EMPLOYEE_NO IN " + parstrEmployeeNoIN);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            byte[] bytCompress = Get_Form_Records(parint64CompanyNo);

            return bytCompress;
        }

        public int Backup_DataBase(Int64 parInt64CompanyNo, string parstrPayrollType)
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

                string strBackupFileName = strDataBaseName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_BeforeResetEmployeeTakeOn.bak";

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
    }
}
