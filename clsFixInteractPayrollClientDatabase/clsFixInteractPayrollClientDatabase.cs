using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InteractPayroll;
using System.Data;
using System.IO;

namespace InteractPayrollClient
{
    public class clsFixInteractPayrollClientDatabase
    {
        clsDBConnectionObjects clsDBConnectionObjects;
     
        public clsFixInteractPayrollClientDatabase()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public void Fix_Client_Database()
        {
            //WriteLog("Fix_Client_Database Entered");

            try
            {
                DataSet DataSet = new DataSet();
                StringBuilder strQry = new StringBuilder();

                //2018-08-09
                //2018-08-09
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.TABLES ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'VALIDITE_HOSTING_SERVICE' ");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ValiditeHostingService");

                if (DataSet.Tables["ValiditeHostingService"].Rows.Count == 0)
                {
                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE dbo.VALIDITE_HOSTING_SERVICE (");
                    
                    strQry.AppendLine("DYNAMIC_UPLOAD_TIMESHEETS_RUNNING_IND varchar(1) NULL,");
                    strQry.AppendLine("START_DYNAMIC_UPLOAD_TIMESHEETS_RUN_IND varchar(1) NULL");

                    strQry.AppendLine(") ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove(DataSet.Tables["ValiditeHostingService"]);

                //2018-07-24
                //2018-07-24
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.TABLES ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'CONSOLE_LINES' ");
               
                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ConsoleLines");

                if (DataSet.Tables["ConsoleLines"].Rows.Count == 0)
                {
                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE dbo.CONSOLE_LINES(");

                    strQry.AppendLine("CONSOLE_LINE_NO int NOT NULL,");
                    strQry.AppendLine("CONSOLE_LINE_MESSAGE varchar(max) NOT NULL,");
                   
                    strQry.AppendLine("CONSTRAINT PK_CONSOLE_LINES PRIMARY KEY CLUSTERED ");
                    strQry.AppendLine("(");
                    strQry.AppendLine("CONSOLE_LINE_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove(DataSet.Tables["ConsoleLines"]);
                
                //2017-09-28
                //2017-07-10
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'EMPLOYEE' ");
                strQry.AppendLine(" AND COLUMN_NAME = 'UPLOAD_CLOCK_PARAMETERS_IND' ");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "UploadClockParametersInd");

                if (DataSet.Tables["UploadClockParametersInd"].Rows.Count == 0)
                {
                    strQry.Clear();

                    strQry.AppendLine(" ALTER TABLE dbo.EMPLOYEE ADD ");

                    strQry.AppendLine(" EMPLOYEE_RFID_CARD_NO varchar(15) NULL ");
                    strQry.AppendLine(",UPLOAD_CLOCK_PARAMETERS_IND varchar(1) NULL ");
                   
                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                    
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE E");

                    strQry.AppendLine(" SET E.UPLOAD_CLOCK_PARAMETERS_IND = 'Y'");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" WHERE E.USE_EMPLOYEE_NO_IND = 'Y' ");
                    strQry.AppendLine(" OR ISNULL(E.EMPLOYEE_PIN,'') <> '' ");
                    
                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove(DataSet.Tables["UploadClockParametersInd"]);
                
                //2017-08-08
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.TABLES ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'FILE_CLIENT_UPLOAD_DETAILS' ");
                
                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "FileClientUploadDetails");

                if (DataSet.Tables["FileClientUploadDetails"].Rows.Count == 0)
                {
                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE dbo.FILE_CLIENT_UPLOAD_DETAILS(");
                
                    strQry.AppendLine("FILE_NAME varchar(150) NOT NULL,");
                    strQry.AppendLine("FILE_LAST_UPDATED_DATE datetime NOT NULL,");
                    strQry.AppendLine("FILE_SIZE int NOT NULL,");
                    strQry.AppendLine("FILE_SIZE_COMPRESSED int NOT NULL,");
                    strQry.AppendLine("FILE_VERSION_NO varchar(15) NOT NULL,");
                    strQry.AppendLine("FILE_CRC_VALUE varchar(15) NOT NULL,");
                    strQry.AppendLine("CONSTRAINT PK_FILE_CLIENT_UPLOAD_DETAILS PRIMARY KEY CLUSTERED ");
                    strQry.AppendLine("(");
                    strQry.AppendLine("FILE_NAME ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE dbo.FILE_CLIENT_UPLOAD_CHUNKS(");
                    strQry.AppendLine("FILE_NAME varchar(150) NOT NULL,");
                    strQry.AppendLine("FILE_CHUNK_NO int NOT NULL,");
                    strQry.AppendLine("FILE_CHUNK image NOT NULL,");
                    strQry.AppendLine("CONSTRAINT PK_FILE_CLIENT_UPLOAD_CHUNKS PRIMARY KEY CLUSTERED ");
                    strQry.AppendLine("(");
                    strQry.AppendLine("FILE_NAME ASC,");
                    strQry.AppendLine("FILE_CHUNK_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove(DataSet.Tables["FileClientUploadDetails"]);

                //2017-07-10
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'PAY_CATEGORY' ");
                strQry.AppendLine(" AND COLUMN_NAME = 'USER_INTERACT_IND' ");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "UserInteract");

                if (DataSet.Tables["UserInteract"].Rows.Count == 0)
                {
                    strQry.Clear();

                    strQry.AppendLine(" ALTER TABLE dbo.PAY_CATEGORY ADD ");

                    strQry.AppendLine(" USER_INTERACT_IND varchar(1) NULL ");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }
               
                DataSet.Tables.Remove(DataSet.Tables["UserInteract"]);

                //2017-07-10
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" name ");

                strQry.AppendLine(" FROM sys.server_principals ");

                strQry.AppendLine(" WHERE name = 'Interact' ");
                
                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "UserInteractExists");
                
                if (DataSet.Tables["UserInteractExists"].Rows.Count == 0)
                {
                    try
                    {
                        strQry.Clear();

                        strQry.AppendLine(" CREATE LOGIN [Interact] WITH PASSWORD = N'erawnacs', CHECK_EXPIRATION = OFF, CHECK_POLICY = OFF");

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                        strQry.Clear();
                        strQry.AppendLine(" EXEC master..sp_addsrvrolemember @loginame = N'Interact', @rolename = N'sysadmin'");

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                        
                        strQry.Clear();

                        strQry.AppendLine(" UPDATE PAY_CATEGORY");
                        strQry.AppendLine(" SET USER_INTERACT_IND = 'Y'");

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                    }
                    catch
                    {
                        //In Case User does Not have Admin Rights 
                    }
                }

                DataSet.Tables.Remove(DataSet.Tables["UserInteractExists"]);

                //2017-05-10
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" CHARACTER_MAXIMUM_LENGTH ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'USER_ID' ");
                strQry.AppendLine(" AND COLUMN_NAME = 'USER_CLOCK_PIN' ");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "UserClockPin");

                if (DataSet.Tables["UserClockPin"].Rows.Count == 0)
                {
                    strQry.Clear();

                    strQry.AppendLine(" ALTER TABLE dbo.USER_ID ADD ");

                    strQry.AppendLine(" USER_CLOCK_PIN varchar(15) NULL ");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }
                else
                {
                    if (Convert.ToInt32(DataSet.Tables["UserClockPin"].Rows[0]["CHARACTER_MAXIMUM_LENGTH"]) != 15)
                    {
                        strQry.Clear();

                        strQry.AppendLine(" ALTER TABLE dbo.USER_ID ALTER COLUMN ");

                        strQry.AppendLine(" USER_CLOCK_PIN varchar(15) NULL ");

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                    }
                }

                DataSet.Tables.Remove(DataSet.Tables["UserClockPin"]);
                
                //2017-05-08
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" READ_OPTION_NO ");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.READ_OPTION ");

                strQry.AppendLine(" WHERE READ_OPTION_NO  = 4 ");
                
                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ReadOption");
                           
                if (DataSet.Tables["ReadOption"].Rows.Count == 0)
                {
                    strQry.Clear();

                    strQry.AppendLine("INSERT INTO READ_OPTION");
                    strQry.AppendLine("(READ_OPTION_NO");
                    strQry.AppendLine(",READ_OPTION_DESC)");
                    strQry.AppendLine(" VALUES");
                    strQry.AppendLine("(4");
                    strQry.AppendLine(",'Employee No. And Fingerprint')");
                    
                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("INSERT INTO READ_OPTION");
                    strQry.AppendLine("(READ_OPTION_NO");
                    strQry.AppendLine(",READ_OPTION_DESC)");
                    strQry.AppendLine(" VALUES");
                    strQry.AppendLine("(5");
                    strQry.AppendLine(",'Employee No. And Pin')");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove(DataSet.Tables["ReadOption"]);

                //2017-05-01
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'PAY_CATEGORY' ");
                strQry.AppendLine(" AND COLUMN_NAME = 'LAST_DOWNLOAD_DATETIME' ");
                
                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PayCategoryLastDownloadDateTime");
                
                if (DataSet.Tables["PayCategoryLastDownloadDateTime"].Rows.Count == 0)
                {
                    strQry.Clear();

                    strQry.AppendLine(" ALTER TABLE dbo.PAY_CATEGORY ADD ");

                    strQry.AppendLine(" LAST_DOWNLOAD_DATETIME datetime NULL ");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove(DataSet.Tables["PayCategoryLastDownloadDateTime"]);

                //2017-03-29
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'COMPANY' ");
                strQry.AppendLine(" AND COLUMN_NAME = 'FINGERPRINT_ENGINE' ");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "CompanyFingerPrintEngine");

                //WriteLog("Fix_Client_Database EmployeeDepartmentTableCorrect Count = " + DataSet.Tables["EmployeeDepartmentTableCorrect"].Rows.Count);
                if (DataSet.Tables["CompanyFingerPrintEngine"].Rows.Count == 0)
                {
                    strQry.Clear();

                    strQry.AppendLine(" ALTER TABLE dbo.COMPANY ADD ");

                    strQry.AppendLine(" FINGERPRINT_ENGINE varchar(50) NULL ");
                  
                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove(DataSet.Tables["CompanyFingerPrintEngine"]);

                //2017-02-11
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'EMPLOYEE_FINGERPRINT_TEMPLATE' ");
                strQry.AppendLine(" AND COLUMN_NAME = 'CREATION_DATETIME' ");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "EmployeeFingerPrintTemplateTableCorrect");

                //WriteLog("Fix_Client_Database EmployeeDepartmentTableCorrect Count = " + DataSet.Tables["EmployeeDepartmentTableCorrect"].Rows.Count);
                if (DataSet.Tables["EmployeeFingerPrintTemplateTableCorrect"].Rows.Count == 0)
                {
                    strQry.Clear();

                    strQry.AppendLine(" ALTER TABLE dbo.EMPLOYEE_FINGERPRINT_TEMPLATE ADD ");
                    strQry.AppendLine(" CREATION_DATETIME datetime NULL ");
                    strQry.AppendLine(",KEEP_IND varchar(1) NULL ");
                    
                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE dbo.EMPLOYEE_FINGERPRINT_TEMPLATE_DELETE(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("FINGER_NO smallint NOT NULL,");
                    strQry.AppendLine("CREATION_DATETIME datetime NOT NULL,");

                    strQry.AppendLine("CONSTRAINT PK_EMPLOYEE_FINGERPRINT_TEMPLATE_DELETE PRIMARY KEY CLUSTERED ");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("FINGER_NO ASC,");
                    strQry.AppendLine("CREATION_DATETIME ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine(" ALTER TABLE dbo.USER_FINGERPRINT_TEMPLATE ADD ");
                    strQry.AppendLine(" CREATION_DATETIME datetime NULL ");
                    strQry.AppendLine(",KEEP_IND varchar(1) NULL ");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE dbo.USER_FINGERPRINT_TEMPLATE_DELETE(");
                    strQry.AppendLine("USER_NO int NOT NULL,");
                    strQry.AppendLine("FINGER_NO smallint NOT NULL,");
                    strQry.AppendLine("CREATION_DATETIME datetime NOT NULL,");

                    strQry.AppendLine("CONSTRAINT PK_USER_FINGERPRINT_TEMPLATE_DELETE PRIMARY KEY CLUSTERED ");
                    strQry.AppendLine("(");
                    strQry.AppendLine("USER_NO ASC,");
                    strQry.AppendLine("FINGER_NO ASC,");
                    strQry.AppendLine("CREATION_DATETIME ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove(DataSet.Tables["EmployeeFingerPrintTemplateTableCorrect"]);

                //2017-02-11
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'EMPLOYEE_DEPARTMENT_LINK' ");
                strQry.AppendLine(" AND COLUMN_NAME = 'PAY_CATEGORY_NO' ");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "EmployeeDepartmentTableCorrect");

                //WriteLog("Fix_Client_Database EmployeeDepartmentTableCorrect Count = " + DataSet.Tables["EmployeeDepartmentTableCorrect"].Rows.Count);
                if (DataSet.Tables["EmployeeDepartmentTableCorrect"].Rows.Count == 0)
                {
                    strQry.Clear();

                    strQry.AppendLine(" DROP TABLE dbo.EMPLOYEE_DEPARTMENT_LINK ");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE dbo.EMPLOYEE_DEPARTMENT_LINK(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO smallint NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE varchar(1) NOT NULL,");
                    strQry.AppendLine("DEPARTMENT_NO smallint NOT NULL,");

                    strQry.AppendLine("CONSTRAINT PK_EMPLOYEE_DEPARTMENT_LINK PRIMARY KEY CLUSTERED ");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE ASC,");
                    strQry.AppendLine("DEPARTMENT_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove(DataSet.Tables["EmployeeDepartmentTableCorrect"]);

                //2017-02-11
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'DEPARTMENT' ");
                strQry.AppendLine(" AND COLUMN_NAME = 'PAY_CATEGORY_NO' ");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "DepartmentTableCorrect");

                //WriteLog("Fix_Client_Database DepartmentTableCorrect Count = " + DataSet.Tables["DepartmentTableCorrect"].Rows.Count);
                if (DataSet.Tables["DepartmentTableCorrect"].Rows.Count == 0)
                {
                    strQry.Clear();

                    strQry.AppendLine(" DROP TABLE dbo.DEPARTMENT ");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE dbo.DEPARTMENT(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE varchar(1) NOT NULL,");
                    strQry.AppendLine("DEPARTMENT_NO smallint NOT NULL,");
                    strQry.AppendLine("DEPARTMENT_DESC varchar(30) NOT NULL,");
                    strQry.AppendLine("KEEP_IND varchar(1) NULL,");

                    strQry.AppendLine("CONSTRAINT PK_DEPARTMENT PRIMARY KEY CLUSTERED ");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE ASC,");
                    strQry.AppendLine("DEPARTMENT_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove(DataSet.Tables["DepartmentTableCorrect"]);

                //2017-02-11
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'EMPLOYEE' ");
                strQry.AppendLine(" AND COLUMN_NAME = 'EMPLOYEE_3RD_PARTY_CODE' ");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee3rdPartyCodeExists");

                if (DataSet.Tables["Employee3rdPartyCodeExists"].Rows.Count == 0)
                {
                    strQry.Clear();

                    strQry.AppendLine(" ALTER TABLE dbo.EMPLOYEE ADD");
                    strQry.AppendLine(" EMPLOYEE_3RD_PARTY_CODE varchar(20) NULL");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove(DataSet.Tables["Employee3rdPartyCodeExists"]);

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.TABLES ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'USER_PAY_CATEGORY_DEPARTMENT' ");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "UserPayCategoryDept", 3);

                if (DataSet.Tables["UserPayCategoryDept"].Rows.Count == 0)
                {
                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE dbo.USER_PAY_CATEGORY_DEPARTMENT(");
                    strQry.AppendLine("USER_NO int NOT NULL,");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE varchar(1) NOT NULL,");
                    strQry.AppendLine("DEPARTMENT_NO smallint NOT NULL,");
                    strQry.AppendLine("KEEP_IND varchar(1) NOT NULL,");

                    strQry.AppendLine("CONSTRAINT PK_USER_PAY_CATEGORY_DEPARTMENT PRIMARY KEY CLUSTERED ");
                    strQry.AppendLine("(");
                    strQry.AppendLine("USER_NO ASC,");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE ASC,");
                    strQry.AppendLine("DEPARTMENT_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove(DataSet.Tables["UserPayCategoryDept"]);

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'USER_COMPANY_ACCESS' ");
                strQry.AppendLine(" AND COLUMN_NAME = 'ACCESS_LAYER_IND' ");

                //60 Seconds
                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "AccessLayerIndExists");

                if (DataSet.Tables["AccessLayerIndExists"].Rows.Count == 0)
                {
                    strQry.Clear();

                    strQry.AppendLine(" ALTER TABLE dbo.USER_COMPANY_ACCESS ADD");
                    strQry.AppendLine(" ACCESS_LAYER_IND varchar(1) NULL");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove(DataSet.Tables["AccessLayerIndExists"]);

                //Fix of Database Below
                //Fix of Database Below
                //2016-06-14
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT' ");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "DaySummary", 3);

                if (DataSet.Tables["DaySummary"].Rows.Count == 0)
                {
                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE dbo.EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("TIMESHEET_DATE Date NOT NULL,");
                    strQry.AppendLine("DAY_NO tinyint NULL,");
                    strQry.AppendLine("DAY_PAID_MINUTES smallint NULL,");
                    strQry.AppendLine("BREAK_ACCUM_MINUTES smallint NULL,");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("BREAK_INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("CONSTRAINT PK_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT PRIMARY KEY CLUSTERED ");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("TIMESHEET_DATE ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE dbo.EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("TIMESHEET_DATE Date NOT NULL,");
                    strQry.AppendLine("DAY_NO tinyint NULL,");
                    strQry.AppendLine("DAY_PAID_MINUTES smallint NULL,");
                    strQry.AppendLine("BREAK_ACCUM_MINUTES smallint NULL,");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("BREAK_INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("CONSTRAINT PK_EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT PRIMARY KEY CLUSTERED ");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("TIMESHEET_DATE ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("TIMESHEET_DATE Date NOT NULL,");
                    strQry.AppendLine("DAY_NO tinyint NULL,");
                    strQry.AppendLine("DAY_PAID_MINUTES smallint NULL,");
                    strQry.AppendLine("BREAK_ACCUM_MINUTES smallint NULL,");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("BREAK_INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("CONSTRAINT PK_EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT PRIMARY KEY CLUSTERED ");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("TIMESHEET_DATE ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("ALTER TABLE dbo.EMPLOYEE_BREAK_CURRENT ADD");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("BREAK_ACCUM_MINUTES int NULL");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("ALTER TABLE dbo.EMPLOYEE_BREAK_HISTORY ADD");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("BREAK_ACCUM_MINUTES int NULL");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("ALTER TABLE dbo.EMPLOYEE_SALARY_BREAK_CURRENT ADD");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("BREAK_ACCUM_MINUTES int NULL");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("ALTER TABLE dbo.EMPLOYEE_SALARY_BREAK_HISTORY ADD");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("BREAK_ACCUM_MINUTES int NULL");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("ALTER TABLE dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ADD");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("BREAK_ACCUM_MINUTES int NULL");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("ALTER TABLE dbo.EMPLOYEE_TIME_ATTEND_BREAK_HISTORY ADD");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("BREAK_ACCUM_MINUTES int NULL");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("ALTER TABLE dbo.EMPLOYEE_TIMESHEET_CURRENT ADD");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("TIMESHEET_ACCUM_MINUTES int NULL");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("ALTER TABLE dbo.EMPLOYEE_TIMESHEET_HISTORY ADD");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("TIMESHEET_ACCUM_MINUTES int NULL");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("ALTER TABLE dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ADD");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("TIMESHEET_ACCUM_MINUTES int NULL");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("ALTER TABLE dbo.EMPLOYEE_SALARY_TIMESHEET_HISTORY ADD");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("TIMESHEET_ACCUM_MINUTES int NULL");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("ALTER TABLE dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ADD");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("TIMESHEET_ACCUM_MINUTES int NULL");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("ALTER TABLE dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_HISTORY ADD");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("TIMESHEET_ACCUM_MINUTES int NULL");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove(DataSet.Tables["DaySummary"]);

                //2016-04-02
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'REMOTE_BACKUP_SITE_NAME' ");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "RemoteBackupSiteName", 3);

                if (DataSet.Tables["RemoteBackupSiteName"].Rows.Count == 0)
                {
                    strQry.Clear();
                    strQry.AppendLine(" CREATE TABLE InteractPayrollClient.dbo.REMOTE_BACKUP_SITE_NAME (");
                    strQry.AppendLine(" SITE_NAME VARCHAR(30) NULL ");
                    strQry.AppendLine(" ) ON [PRIMARY] ");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove(DataSet.Tables["RemoteBackupSiteName"]);

                //2014-11-29 
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" DEVICE_NO ");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE ");

                strQry.AppendLine(" WHERE COMPANY_NO = -1 ");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "DeviceCompanyCheck", 3);

                if (DataSet.Tables["DeviceCompanyCheck"].Rows.Count > 0)
                {
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" COMPANY_NO ");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.COMPANY ");

                    this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Company", 3);

                    if (DataSet.Tables["Company"].Rows.Count > 0)
                    {
                        for (int intRow = 0; intRow < DataSet.Tables["DeviceCompanyCheck"].Rows.Count; intRow++)
                        {
                            strQry.Clear();
                            strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.DEVICE ");
                            strQry.AppendLine(" SET COMPANY_NO = " + DataSet.Tables["Company"].Rows[0]["COMPANY_NO"].ToString());
                            strQry.AppendLine(" WHERE DEVICE_NO = " + DataSet.Tables["DeviceCompanyCheck"].Rows[intRow]["DEVICE_NO"].ToString());

                            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                        }
                    }

                    DataSet.Tables.Remove(DataSet.Tables["Company"]);
                }

                DataSet.Tables.Remove(DataSet.Tables["DeviceCompanyCheck"]);

                //2014-08-16
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'EMPLOYEE' ");
                strQry.AppendLine(" AND COLUMN_NAME = 'EMPLOYEE_PIN' ");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "EmployeePin", 3);

                if (DataSet.Tables["EmployeePin"].Rows.Count == 0)
                {
                    strQry.Clear();
                    strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE ");
                    strQry.AppendLine(" ADD EMPLOYEE_PIN VARCHAR(10) NULL ");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove(DataSet.Tables["EmployeePin"]);

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" COLUMN_NAME");
                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");
                strQry.AppendLine(" WHERE TABLE_NAME = 'EMPLOYEE_TIME_ATTEND_BREAK_CURRENT'");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "TimeAttendTablesExist", 3);

                if (DataSet.Tables["TimeAttendTablesExist"].Rows.Count == 0)
                {
                    strQry.Clear();
                    strQry.AppendLine("CREATE TABLE InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("BREAK_DATE Date NOT NULL,");
                    strQry.AppendLine("BREAK_SEQ tinyint NOT NULL,");
                    strQry.AppendLine("BREAK_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("BREAK_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("BREAK_DATE ASC,");
                    strQry.AppendLine("BREAK_SEQ ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();
                    strQry.AppendLine("CREATE TABLE InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_HISTORY(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("PAY_PERIOD_DATE Date NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("BREAK_DATE Date NOT NULL,");
                    strQry.AppendLine("BREAK_SEQ tinyint NOT NULL,");
                    strQry.AppendLine("BREAK_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("BREAK_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("PAY_PERIOD_DATE ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("BREAK_DATE ASC,");
                    strQry.AppendLine("BREAK_SEQ ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();
                    strQry.AppendLine("CREATE TABLE InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("TIMESHEET_DATE Date NOT NULL,");
                    strQry.AppendLine("TIMESHEET_SEQ tinyint NOT NULL,");
                    strQry.AppendLine("TIMESHEET_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("TIMESHEET_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("TIMESHEET_DATE ASC,");
                    strQry.AppendLine("TIMESHEET_SEQ ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();
                    strQry.AppendLine("CREATE TABLE InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_HISTORY(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("PAY_PERIOD_DATE Date NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("TIMESHEET_DATE Date NOT NULL,");
                    strQry.AppendLine("TIMESHEET_SEQ tinyint NOT NULL,");
                    strQry.AppendLine("TIMESHEET_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("TIMESHEET_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("PAY_PERIOD_DATE ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("TIMESHEET_DATE ASC,");
                    strQry.AppendLine("TIMESHEET_SEQ ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove(DataSet.Tables["TimeAttendTablesExist"]);

                //2013-07-06
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'EMPLOYEE_TIMESHEET_CURRENT' ");
                strQry.AppendLine(" AND COLUMN_NAME = 'TIMESHEET_SEQ' ");
                strQry.AppendLine(" AND DATA_TYPE = 'smallint' ");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ColTypeExist", 3);

                if (DataSet.Tables["ColTypeExist"].Rows.Count > 0)
                {
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" DISTINCT TC.CONSTRAINT_NAME ");

                    strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC ");
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE CCU ");
                    strQry.AppendLine(" ON TC.CONSTRAINT_NAME = CCU.CONSTRAINT_NAME ");
                    strQry.AppendLine(" WHERE TC.CONSTRAINT_TYPE = 'Primary Key' ");
                    strQry.AppendLine(" AND TC.TABLE_NAME = 'EMPLOYEE_TIMESHEET_CURRENT' ");

                    this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PKExist", 3);

                    if (DataSet.Tables["PKExist"].Rows.Count > 0)
                    {
                        strQry.Clear();
                        strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ");
                        strQry.AppendLine(" DROP CONSTRAINT " + DataSet.Tables["PKExist"].Rows[0]["CONSTRAINT_NAME"].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                    }

                    DataSet.Tables.Remove("PKExist");

                    strQry.Clear();
                    strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT");
                    strQry.AppendLine(" ALTER COLUMN TIMESHEET_SEQ tinyint NOT NULL");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    //Add Primary Key Back
                    strQry.Clear();
                    strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ");
                    strQry.AppendLine(" ADD CONSTRAINT PK_EMPLOYEE_TIMESHEET_CURRENT ");
                    strQry.AppendLine(" PRIMARY KEY CLUSTERED ");
                    strQry.AppendLine(" (");
                    strQry.AppendLine(" COMPANY_NO ASC,");
                    strQry.AppendLine(" EMPLOYEE_NO ASC,");
                    strQry.AppendLine(" PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine(" TIMESHEET_DATE ASC,");
                    strQry.AppendLine(" TIMESHEET_SEQ ASC");
                    strQry.AppendLine(" ) ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove("ColTypeExist");

                //2013-09-03
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'EMPLOYEE_TIMESHEET_HISTORY' ");
                strQry.AppendLine(" AND COLUMN_NAME = 'TIMESHEET_SEQ' ");
                strQry.AppendLine(" AND DATA_TYPE = 'smallint' ");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ColTypeExist", 3);

                if (DataSet.Tables["ColTypeExist"].Rows.Count > 0)
                {
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" DISTINCT TC.CONSTRAINT_NAME ");

                    strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC ");
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE CCU ");
                    strQry.AppendLine(" ON TC.CONSTRAINT_NAME = CCU.CONSTRAINT_NAME ");
                    strQry.AppendLine(" WHERE TC.CONSTRAINT_TYPE = 'Primary Key' ");
                    strQry.AppendLine(" AND TC.TABLE_NAME = 'EMPLOYEE_TIMESHEET_HISTORY' ");

                    this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PKExist", 3);

                    if (DataSet.Tables["PKExist"].Rows.Count > 0)
                    {
                        strQry.Clear();
                        strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_HISTORY ");
                        strQry.AppendLine(" DROP CONSTRAINT " + DataSet.Tables["PKExist"].Rows[0]["CONSTRAINT_NAME"].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                    }

                    DataSet.Tables.Remove("PKExist");

                    strQry.Clear();
                    strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_HISTORY");
                    strQry.AppendLine(" ALTER COLUMN TIMESHEET_SEQ tinyint NOT NULL");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    //Add Primary Key Back
                    strQry.Clear();
                    strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_HISTORY ");
                    strQry.AppendLine(" ADD CONSTRAINT PK_EMPLOYEE_TIMESHEET_HISTORY ");
                    strQry.AppendLine(" PRIMARY KEY CLUSTERED ");
                    strQry.AppendLine(" (");
                    strQry.AppendLine(" COMPANY_NO ASC,");
                    strQry.AppendLine(" EMPLOYEE_NO ASC,");
                    strQry.AppendLine(" PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine(" TIMESHEET_DATE ASC,");
                    strQry.AppendLine(" TIMESHEET_SEQ ASC");
                    strQry.AppendLine(" ) ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove("ColTypeExist");

                //2013-07-06
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'EMPLOYEE_SALARY_TIMESHEET_CURRENT' ");
                strQry.AppendLine(" AND COLUMN_NAME = 'TIMESHEET_SEQ' ");
                strQry.AppendLine(" AND DATA_TYPE = 'smallint' ");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ColTypeExist", 3);

                if (DataSet.Tables["ColTypeExist"].Rows.Count > 0)
                {
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" DISTINCT TC.CONSTRAINT_NAME ");

                    strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC ");
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE CCU ");
                    strQry.AppendLine(" ON TC.CONSTRAINT_NAME = CCU.CONSTRAINT_NAME ");
                    strQry.AppendLine(" WHERE TC.CONSTRAINT_TYPE = 'Primary Key' ");
                    strQry.AppendLine(" AND TC.TABLE_NAME = 'EMPLOYEE_SALARY_TIMESHEET_CURRENT' ");

                    this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PKExist", 3);

                    if (DataSet.Tables["PKExist"].Rows.Count > 0)
                    {
                        strQry.Clear();
                        strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ");
                        strQry.AppendLine(" DROP CONSTRAINT " + DataSet.Tables["PKExist"].Rows[0]["CONSTRAINT_NAME"].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                    }

                    DataSet.Tables.Remove("PKExist");

                    strQry.Clear();
                    strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT");
                    strQry.AppendLine(" ALTER COLUMN TIMESHEET_SEQ tinyint NOT NULL");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    //Add Primary Key Back
                    strQry.Clear();
                    strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ");
                    strQry.AppendLine(" ADD CONSTRAINT PK_EMPLOYEE_SALARY_TIMESHEET_CURRENT ");
                    strQry.AppendLine(" PRIMARY KEY CLUSTERED ");
                    strQry.AppendLine(" (");
                    strQry.AppendLine(" COMPANY_NO ASC,");
                    strQry.AppendLine(" EMPLOYEE_NO ASC,");
                    strQry.AppendLine(" PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine(" TIMESHEET_DATE ASC,");
                    strQry.AppendLine(" TIMESHEET_SEQ ASC");
                    strQry.AppendLine(" ) ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove("ColTypeExist");

                //2013-09-03
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'EMPLOYEE_SALARY_TIMESHEET_HISTORY' ");
                strQry.AppendLine(" AND COLUMN_NAME = 'TIMESHEET_SEQ' ");
                strQry.AppendLine(" AND DATA_TYPE = 'smallint' ");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ColTypeExist", 3);

                if (DataSet.Tables["ColTypeExist"].Rows.Count > 0)
                {
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" DISTINCT TC.CONSTRAINT_NAME ");

                    strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC ");
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE CCU ");
                    strQry.AppendLine(" ON TC.CONSTRAINT_NAME = CCU.CONSTRAINT_NAME ");
                    strQry.AppendLine(" WHERE TC.CONSTRAINT_TYPE = 'Primary Key' ");
                    strQry.AppendLine(" AND TC.TABLE_NAME = 'EMPLOYEE_SALARY_TIMESHEET_HISTORY' ");

                    this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PKExist", 3);

                    if (DataSet.Tables["PKExist"].Rows.Count > 0)
                    {
                        strQry.Clear();
                        strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_HISTORY ");
                        strQry.AppendLine(" DROP CONSTRAINT " + DataSet.Tables["PKExist"].Rows[0]["CONSTRAINT_NAME"].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                    }

                    DataSet.Tables.Remove("PKExist");

                    strQry.Clear();
                    strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_HISTORY");
                    strQry.AppendLine(" ALTER COLUMN TIMESHEET_SEQ tinyint NOT NULL");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    //Add Primary Key Back
                    strQry.Clear();
                    strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_HISTORY ");
                    strQry.AppendLine(" ADD CONSTRAINT PK_EMPLOYEE_SALARY_TIMESHEET_HISTORY ");
                    strQry.AppendLine(" PRIMARY KEY CLUSTERED ");
                    strQry.AppendLine(" (");
                    strQry.AppendLine(" COMPANY_NO ASC,");
                    strQry.AppendLine(" EMPLOYEE_NO ASC,");
                    strQry.AppendLine(" PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine(" TIMESHEET_DATE ASC,");
                    strQry.AppendLine(" TIMESHEET_SEQ ASC");
                    strQry.AppendLine(" ) ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove("ColTypeExist");

                //2013-07-06
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'EMPLOYEE_BREAK_CURRENT' ");
                strQry.AppendLine(" AND COLUMN_NAME = 'BREAK_SEQ' ");
                strQry.AppendLine(" AND DATA_TYPE = 'smallint' ");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ColTypeExist", 3);

                if (DataSet.Tables["ColTypeExist"].Rows.Count > 0)
                {
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" DISTINCT TC.CONSTRAINT_NAME ");

                    strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC ");
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE CCU ");
                    strQry.AppendLine(" ON TC.CONSTRAINT_NAME = CCU.CONSTRAINT_NAME ");
                    strQry.AppendLine(" WHERE TC.CONSTRAINT_TYPE = 'Primary Key' ");
                    strQry.AppendLine(" AND TC.TABLE_NAME = 'EMPLOYEE_BREAK_CURRENT' ");

                    this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PKExist", 3);

                    if (DataSet.Tables["PKExist"].Rows.Count > 0)
                    {
                        strQry.Clear();
                        strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT ");
                        strQry.AppendLine(" DROP CONSTRAINT " + DataSet.Tables["PKExist"].Rows[0]["CONSTRAINT_NAME"].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                    }

                    DataSet.Tables.Remove("PKExist");

                    strQry.Clear();
                    strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT");
                    strQry.AppendLine(" ALTER COLUMN BREAK_SEQ tinyint NOT NULL");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    //Add Primary Key Back
                    strQry.Clear();
                    strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT ");
                    strQry.AppendLine(" ADD CONSTRAINT PK_EMPLOYEE_BREAK_CURRENT ");
                    strQry.AppendLine(" PRIMARY KEY CLUSTERED ");
                    strQry.AppendLine(" (");
                    strQry.AppendLine(" COMPANY_NO ASC,");
                    strQry.AppendLine(" EMPLOYEE_NO ASC,");
                    strQry.AppendLine(" PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine(" BREAK_DATE ASC,");
                    strQry.AppendLine(" BREAK_SEQ ASC");
                    strQry.AppendLine(" ) ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove("ColTypeExist");

                //2013-09-03
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'EMPLOYEE_BREAK_HISTORY' ");
                strQry.AppendLine(" AND COLUMN_NAME = 'BREAK_SEQ' ");
                strQry.AppendLine(" AND DATA_TYPE = 'smallint' ");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ColTypeExist", 3);

                if (DataSet.Tables["ColTypeExist"].Rows.Count > 0)
                {
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" DISTINCT TC.CONSTRAINT_NAME ");

                    strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC ");
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE CCU ");
                    strQry.AppendLine(" ON TC.CONSTRAINT_NAME = CCU.CONSTRAINT_NAME ");
                    strQry.AppendLine(" WHERE TC.CONSTRAINT_TYPE = 'Primary Key' ");
                    strQry.AppendLine(" AND TC.TABLE_NAME = 'EMPLOYEE_BREAK_HISTORY' ");

                    this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PKExist", 3);

                    if (DataSet.Tables["PKExist"].Rows.Count > 0)
                    {
                        strQry.Clear();
                        strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_BREAK_HISTORY ");
                        strQry.AppendLine(" DROP CONSTRAINT " + DataSet.Tables["PKExist"].Rows[0]["CONSTRAINT_NAME"].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                    }

                    DataSet.Tables.Remove("PKExist");

                    strQry.Clear();
                    strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_BREAK_HISTORY");
                    strQry.AppendLine(" ALTER COLUMN BREAK_SEQ tinyint NOT NULL");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    //Add Primary Key Back
                    strQry.Clear();
                    strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_BREAK_HISTORY ");
                    strQry.AppendLine(" ADD CONSTRAINT PK_EMPLOYEE_BREAK_HISTORY ");
                    strQry.AppendLine(" PRIMARY KEY CLUSTERED ");
                    strQry.AppendLine(" (");
                    strQry.AppendLine(" COMPANY_NO ASC,");
                    strQry.AppendLine(" EMPLOYEE_NO ASC,");
                    strQry.AppendLine(" PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine(" BREAK_DATE ASC,");
                    strQry.AppendLine(" BREAK_SEQ ASC");
                    strQry.AppendLine(" ) ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove("ColTypeExist");

                //2013-07-06
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'EMPLOYEE_SALARY_BREAK_CURRENT' ");
                strQry.AppendLine(" AND COLUMN_NAME = 'BREAK_SEQ' ");
                strQry.AppendLine(" AND DATA_TYPE = 'smallint' ");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ColTypeExist", 3);

                if (DataSet.Tables["ColTypeExist"].Rows.Count > 0)
                {
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" DISTINCT TC.CONSTRAINT_NAME ");

                    strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC ");
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE CCU ");
                    strQry.AppendLine(" ON TC.CONSTRAINT_NAME = CCU.CONSTRAINT_NAME ");
                    strQry.AppendLine(" WHERE TC.CONSTRAINT_TYPE = 'Primary Key' ");
                    strQry.AppendLine(" AND TC.TABLE_NAME = 'EMPLOYEE_SALARY_BREAK_CURRENT' ");

                    this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PKExist", 3);

                    if (DataSet.Tables["PKExist"].Rows.Count > 0)
                    {
                        strQry.Clear();
                        strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ");
                        strQry.AppendLine(" DROP CONSTRAINT " + DataSet.Tables["PKExist"].Rows[0]["CONSTRAINT_NAME"].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                    }

                    DataSet.Tables.Remove("PKExist");

                    strQry.Clear();
                    strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT");
                    strQry.AppendLine(" ALTER COLUMN BREAK_SEQ tinyint NOT NULL");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    //Add Primary Key Back
                    strQry.Clear();
                    strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ");
                    strQry.AppendLine(" ADD CONSTRAINT PK_EMPLOYEE_SALARY_BREAK_CURRENT ");
                    strQry.AppendLine(" PRIMARY KEY CLUSTERED ");
                    strQry.AppendLine(" (");
                    strQry.AppendLine(" COMPANY_NO ASC,");
                    strQry.AppendLine(" EMPLOYEE_NO ASC,");
                    strQry.AppendLine(" PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine(" BREAK_DATE ASC,");
                    strQry.AppendLine(" BREAK_SEQ ASC");
                    strQry.AppendLine(" ) ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove("ColTypeExist");

                //2013-09-03
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'EMPLOYEE_SALARY_BREAK_HISTORY' ");
                strQry.AppendLine(" AND COLUMN_NAME = 'BREAK_SEQ' ");
                strQry.AppendLine(" AND DATA_TYPE = 'smallint' ");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ColTypeExist", 3);

                if (DataSet.Tables["ColTypeExist"].Rows.Count > 0)
                {
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" DISTINCT TC.CONSTRAINT_NAME ");

                    strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC ");
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE CCU ");
                    strQry.AppendLine(" ON TC.CONSTRAINT_NAME = CCU.CONSTRAINT_NAME ");
                    strQry.AppendLine(" WHERE TC.CONSTRAINT_TYPE = 'Primary Key' ");
                    strQry.AppendLine(" AND TC.TABLE_NAME = 'EMPLOYEE_SALARY_BREAK_HISTORY' ");

                    this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PKExist", 3);

                    if (DataSet.Tables["PKExist"].Rows.Count > 0)
                    {
                        strQry.Clear();
                        strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_HISTORY ");
                        strQry.AppendLine(" DROP CONSTRAINT " + DataSet.Tables["PKExist"].Rows[0]["CONSTRAINT_NAME"].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                    }

                    DataSet.Tables.Remove("PKExist");

                    strQry.Clear();
                    strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_HISTORY");
                    strQry.AppendLine(" ALTER COLUMN BREAK_SEQ tinyint NOT NULL");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    //Add Primary Key Back
                    strQry.Clear();
                    strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_HISTORY ");
                    strQry.AppendLine(" ADD CONSTRAINT PK_EMPLOYEE_SALARY_BREAK_HISTORY ");
                    strQry.AppendLine(" PRIMARY KEY CLUSTERED ");
                    strQry.AppendLine(" (");
                    strQry.AppendLine(" COMPANY_NO ASC,");
                    strQry.AppendLine(" EMPLOYEE_NO ASC,");
                    strQry.AppendLine(" PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine(" BREAK_DATE ASC,");
                    strQry.AppendLine(" BREAK_SEQ ASC");
                    strQry.AppendLine(" ) ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove("ColTypeExist");

                //2013-09-03
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT' ");
                strQry.AppendLine(" AND COLUMN_NAME = 'TIMESHEET_SEQ' ");
                strQry.AppendLine(" AND DATA_TYPE = 'smallint' ");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ColTypeExist", 3);

                if (DataSet.Tables["ColTypeExist"].Rows.Count > 0)
                {
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" DISTINCT TC.CONSTRAINT_NAME ");

                    strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC ");
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE CCU ");
                    strQry.AppendLine(" ON TC.CONSTRAINT_NAME = CCU.CONSTRAINT_NAME ");
                    strQry.AppendLine(" WHERE TC.CONSTRAINT_TYPE = 'Primary Key' ");
                    strQry.AppendLine(" AND TC.TABLE_NAME = 'EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT' ");

                    this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PKExist", 3);

                    if (DataSet.Tables["PKExist"].Rows.Count > 0)
                    {
                        strQry.Clear();
                        strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ");
                        strQry.AppendLine(" DROP CONSTRAINT " + DataSet.Tables["PKExist"].Rows[0]["CONSTRAINT_NAME"].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                    }

                    DataSet.Tables.Remove("PKExist");

                    strQry.Clear();
                    strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT");
                    strQry.AppendLine(" ALTER COLUMN TIMESHEET_SEQ tinyint NOT NULL");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    //Add Primary Key Back
                    strQry.Clear();
                    strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ");
                    strQry.AppendLine(" ADD CONSTRAINT PK_EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ");
                    strQry.AppendLine(" PRIMARY KEY CLUSTERED ");
                    strQry.AppendLine(" (");
                    strQry.AppendLine(" COMPANY_NO ASC,");
                    strQry.AppendLine(" EMPLOYEE_NO ASC,");
                    strQry.AppendLine(" PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine(" TIMESHEET_DATE ASC,");
                    strQry.AppendLine(" TIMESHEET_SEQ ASC");
                    strQry.AppendLine(" ) ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove("ColTypeExist");

                //2013-09-03
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'EMPLOYEE_TIME_ATTEND_TIMESHEET_HISTORY' ");
                strQry.AppendLine(" AND COLUMN_NAME = 'TIMESHEET_SEQ' ");
                strQry.AppendLine(" AND DATA_TYPE = 'smallint' ");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ColTypeExist", 3);

                if (DataSet.Tables["ColTypeExist"].Rows.Count > 0)
                {
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" DISTINCT TC.CONSTRAINT_NAME ");

                    strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC ");
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE CCU ");
                    strQry.AppendLine(" ON TC.CONSTRAINT_NAME = CCU.CONSTRAINT_NAME ");
                    strQry.AppendLine(" WHERE TC.CONSTRAINT_TYPE = 'Primary Key' ");
                    strQry.AppendLine(" AND TC.TABLE_NAME = 'EMPLOYEE_TIME_ATTEND_TIMESHEET_HISTORY' ");

                    this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PKExist", 3);

                    if (DataSet.Tables["PKExist"].Rows.Count > 0)
                    {
                        strQry.Clear();
                        strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_HISTORY ");
                        strQry.AppendLine(" DROP CONSTRAINT " + DataSet.Tables["PKExist"].Rows[0]["CONSTRAINT_NAME"].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                    }

                    DataSet.Tables.Remove("PKExist");

                    strQry.Clear();
                    strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_HISTORY");
                    strQry.AppendLine(" ALTER COLUMN TIMESHEET_SEQ tinyint NOT NULL");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    //Add Primary Key Back
                    strQry.Clear();
                    strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_HISTORY ");
                    strQry.AppendLine(" ADD CONSTRAINT PK_EMPLOYEE_TIME_ATTEND_TIMESHEET_HISTORY ");
                    strQry.AppendLine(" PRIMARY KEY CLUSTERED ");
                    strQry.AppendLine(" (");
                    strQry.AppendLine(" COMPANY_NO ASC,");
                    strQry.AppendLine(" EMPLOYEE_NO ASC,");
                    strQry.AppendLine(" PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine(" TIMESHEET_DATE ASC,");
                    strQry.AppendLine(" TIMESHEET_SEQ ASC");
                    strQry.AppendLine(" ) ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove("ColTypeExist");

                //2013-09-03
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'EMPLOYEE_TIME_ATTEND_BREAK_CURRENT' ");
                strQry.AppendLine(" AND COLUMN_NAME = 'BREAK_SEQ' ");
                strQry.AppendLine(" AND DATA_TYPE = 'smallint' ");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ColTypeExist", 3);

                if (DataSet.Tables["ColTypeExist"].Rows.Count > 0)
                {
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" DISTINCT TC.CONSTRAINT_NAME ");

                    strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC ");
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE CCU ");
                    strQry.AppendLine(" ON TC.CONSTRAINT_NAME = CCU.CONSTRAINT_NAME ");
                    strQry.AppendLine(" WHERE TC.CONSTRAINT_TYPE = 'Primary Key' ");
                    strQry.AppendLine(" AND TC.TABLE_NAME = 'EMPLOYEE_TIME_ATTEND_BREAK_CURRENT' ");

                    this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PKExist", 3);

                    if (DataSet.Tables["PKExist"].Rows.Count > 0)
                    {
                        strQry.Clear();
                        strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ");
                        strQry.AppendLine(" DROP CONSTRAINT " + DataSet.Tables["PKExist"].Rows[0]["CONSTRAINT_NAME"].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                    }

                    DataSet.Tables.Remove("PKExist");

                    strQry.Clear();
                    strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT");
                    strQry.AppendLine(" ALTER COLUMN BREAK_SEQ tinyint NOT NULL");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    //Add Primary Key Back
                    strQry.Clear();
                    strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ");
                    strQry.AppendLine(" ADD CONSTRAINT PK_EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ");
                    strQry.AppendLine(" PRIMARY KEY CLUSTERED ");
                    strQry.AppendLine(" (");
                    strQry.AppendLine(" COMPANY_NO ASC,");
                    strQry.AppendLine(" EMPLOYEE_NO ASC,");
                    strQry.AppendLine(" PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine(" BREAK_DATE ASC,");
                    strQry.AppendLine(" BREAK_SEQ ASC");
                    strQry.AppendLine(" ) ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove("ColTypeExist");

                //2013-09-03
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'EMPLOYEE_TIME_ATTEND_BREAK_HISTORY' ");
                strQry.AppendLine(" AND COLUMN_NAME = 'BREAK_SEQ' ");
                strQry.AppendLine(" AND DATA_TYPE = 'smallint' ");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ColTypeExist", 3);

                if (DataSet.Tables["ColTypeExist"].Rows.Count > 0)
                {
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" DISTINCT TC.CONSTRAINT_NAME ");

                    strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC ");
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE CCU ");
                    strQry.AppendLine(" ON TC.CONSTRAINT_NAME = CCU.CONSTRAINT_NAME ");
                    strQry.AppendLine(" WHERE TC.CONSTRAINT_TYPE = 'Primary Key' ");
                    strQry.AppendLine(" AND TC.TABLE_NAME = 'EMPLOYEE_TIME_ATTEND_BREAK_HISTORY' ");

                    this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PKExist", 3);

                    if (DataSet.Tables["PKExist"].Rows.Count > 0)
                    {
                        strQry.Clear();
                        strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_HISTORY ");
                        strQry.AppendLine(" DROP CONSTRAINT " + DataSet.Tables["PKExist"].Rows[0]["CONSTRAINT_NAME"].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                    }

                    DataSet.Tables.Remove("PKExist");

                    strQry.Clear();
                    strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_HISTORY");
                    strQry.AppendLine(" ALTER COLUMN BREAK_SEQ tinyint NOT NULL");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    //Add Primary Key Back
                    strQry.Clear();
                    strQry.AppendLine(" ALTER TABLE InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_HISTORY ");
                    strQry.AppendLine(" ADD CONSTRAINT PK_EMPLOYEE_TIME_ATTEND_BREAK_HISTORY ");
                    strQry.AppendLine(" PRIMARY KEY CLUSTERED ");
                    strQry.AppendLine(" (");
                    strQry.AppendLine(" COMPANY_NO ASC,");
                    strQry.AppendLine(" EMPLOYEE_NO ASC,");
                    strQry.AppendLine(" PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine(" BREAK_DATE ASC,");
                    strQry.AppendLine(" BREAK_SEQ ASC");
                    strQry.AppendLine(" ) ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove("ColTypeExist");

                //2013-02-20
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" COLUMN_NAME");
                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");
                strQry.AppendLine(" WHERE TABLE_NAME = 'FILE_CLIENT_DOWNLOAD_DETAILS'");
                strQry.AppendLine(" AND COLUMN_NAME = 'FILE_LAYER_IND'");

                this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "FileLayerIndFieldExist", 3);

                if (DataSet.Tables["FileLayerIndFieldExist"].Rows.Count == 0)
                {
                    strQry.Clear();
                    strQry.AppendLine("DROP TABLE InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();
                    strQry.AppendLine("DROP TABLE InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();
                    strQry.AppendLine("DROP TABLE InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_DETAILS");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();
                    strQry.AppendLine("CREATE TABLE InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS(");
                    strQry.AppendLine("FILE_LAYER_IND varchar(1) NOT NULL,");
                    strQry.AppendLine("FILE_NAME varchar(50) NOT NULL,");
                    strQry.AppendLine("FILE_CHUNK_NO int NOT NULL,");
                    strQry.AppendLine("FILE_CHUNK image NULL,");
                    strQry.AppendLine("PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");

                    strQry.AppendLine("FILE_LAYER_IND ASC,");

                    strQry.AppendLine("FILE_NAME ASC,");
                    strQry.AppendLine("FILE_CHUNK_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();
                    strQry.AppendLine("CREATE TABLE InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP(");
                    strQry.AppendLine("FILE_LAYER_IND varchar(1) NOT NULL,");
                    strQry.AppendLine("FILE_NAME varchar(50) NOT NULL,");
                    strQry.AppendLine("FILE_CHUNK_NO int NOT NULL,");
                    strQry.AppendLine("FILE_CHUNK image NULL,");
                    strQry.AppendLine("PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("FILE_LAYER_IND ASC,");
                    strQry.AppendLine("FILE_NAME ASC,");
                    strQry.AppendLine("FILE_CHUNK_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();
                    strQry.AppendLine("CREATE TABLE InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_DETAILS(");
                    strQry.AppendLine("FILE_LAYER_IND varchar(1) NOT NULL,");
                    strQry.AppendLine("FILE_NAME varchar(50) NOT NULL,");
                    strQry.AppendLine("FILE_LAST_UPDATED_DATE datetime NULL,");
                    strQry.AppendLine("FILE_SIZE int NULL,");
                    strQry.AppendLine("FILE_SIZE_COMPRESSED int NULL,");
                    strQry.AppendLine("FILE_VERSION_NO varchar(15) NULL,");
                    strQry.AppendLine("FILE_CRC_VALUE varchar(15) NULL,");
                    strQry.AppendLine("PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("FILE_LAYER_IND ASC,");
                    strQry.AppendLine("FILE_NAME ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                DataSet.Tables.Remove(DataSet.Tables["FileLayerIndFieldExist"]);
            }
            catch(Exception ex)
            {
                string strInnerExceptionMessage = ex.Message;

                if (ex.InnerException != null)
                {
                    strInnerExceptionMessage += " " + ex.InnerException.Message;
                }

                WriteLog("Exception = " + strInnerExceptionMessage);
            }
        }

        private void WriteLog(string Message)
        {
            try
            {
                using (StreamWriter writeLog = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "clsFixInteractPayrollClientDatabase_Log.txt", true))
                {
                    writeLog.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + Message);
                }
            }
            catch (Exception ex)
            {
                string strInnerExceptionMessage = ex.Message;

                if (ex.InnerException != null)
                {
                    strInnerExceptionMessage += " " + ex.InnerException.Message;
                }
                
                using (StreamWriter writeLog = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "clsFixInteractPayrollClientDatabase_Exception_Log.txt", true))
                {
                    writeLog.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + Message + "Exception = " + strInnerExceptionMessage);
                }
            }
        }
    }
}
