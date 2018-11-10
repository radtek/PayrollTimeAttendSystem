using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace CreateDatabase
{
    public partial class frmISDatabaseUtility : Form
    {
        private SqlConnection pvtSqlConnectionClient;
        private SqlCommand pvtSqlCommandClient;

        private SqlConnection pvtSqlConnectionClientIntegratedSecurity;
        private SqlCommand pvtSqlCommandClientIntegratedSecurity;

        private SqlDataAdapter pvtSqlDataAdapterClientIntegratedSecurity;

        private string pvtstrConnectionClient;
        private string pvtstrConnectionClientIntegratedSecurity;

        public frmISDatabaseUtility()
        {
            InitializeComponent();
        }

        private bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void Create_DataTable_Client(string parstrQry, DataSet parDataSet, string parstrSourceTable)
        {
#if(DEBUG)
            parstrQry = parstrQry.Replace("InteractPayrollClient", "InteractPayrollClient_Debug");
#endif
            pvtSqlConnectionClient = new SqlConnection(pvtstrConnectionClientIntegratedSecurity);

            pvtSqlCommandClient = new SqlCommand(parstrQry, pvtSqlConnectionClient);

            pvtSqlDataAdapterClientIntegratedSecurity = new SqlDataAdapter(pvtSqlCommandClient);

            //Opens and Closes the Connection object - pvtSqlConnection
            pvtSqlDataAdapterClientIntegratedSecurity.Fill(parDataSet, parstrSourceTable);

            parDataSet.AcceptChanges();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string strDirectory = AppDomain.CurrentDomain.BaseDirectory;

            try
            {
                DataSet DataSet = new DataSet();
#if (DEBUG)
#else

                string strWorkDirectory = this.txtWorkDirectory.Text.Trim();
                //NB - Needs to Look at Validite Directory
                string strCompanyDirectory = @"\Validite";

                if (Directory.Exists(strWorkDirectory) == false)
                {
                    MessageBox.Show("Work Directory " + strCompanyDirectory + " Not Found.", this.Text,
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                strDirectory = strWorkDirectory + strCompanyDirectory;

                if (Directory.Exists(strDirectory) == false)
                {
                    System.IO.Directory.CreateDirectory(strDirectory);

                    DirectoryInfo d1 = new DirectoryInfo(strDirectory);
                    DirectorySecurity md1 = d1.GetAccessControl();
                    md1.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                    d1.SetAccessControl(md1);
                }

                strDirectory = strWorkDirectory + strCompanyDirectory + @"\Validite Services\";

                if (Directory.Exists(strDirectory) == false)
                {
                    System.IO.Directory.CreateDirectory(strDirectory);

                    DirectoryInfo d1 = new DirectoryInfo(strDirectory);
                    DirectorySecurity md1 = d1.GetAccessControl();
                    md1.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                    d1.SetAccessControl(md1);
                }
#endif
                if (this.cboServer.Text.Trim() != "")
                {
                    string strConnectionClient = @"Server=#Engine#;Database=InteractPayrollClient;Integrated Security=true; MultipleActiveResultSets=True;";
                    string strConnectionClientIntegratedSecurity = @"Server=#Engine#;Database=master; Integrated Security=true; MultipleActiveResultSets=True;";

#if (DEBUG)
                    strConnectionClient = strConnectionClient.Replace("InteractPayrollClient", "InteractPayrollClient_Debug");
#endif
                    pvtstrConnectionClient = strConnectionClient.Replace("#Engine#", this.cboServer.Text.Trim());
                    pvtstrConnectionClientIntegratedSecurity = strConnectionClientIntegratedSecurity.Replace("#Engine#", this.cboServer.Text.Trim());

                    this.btnDrop.Enabled = false;
                    DirectoryInfo di = new DirectoryInfo(strDirectory + @"DB");

                    if (di.Exists == false)
                    {
                        System.IO.Directory.CreateDirectory(strDirectory + @"DB");

                        DirectoryInfo d1 = new DirectoryInfo(strDirectory);
                        DirectorySecurity md1 = d1.GetAccessControl();
                        md1.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                        d1.SetAccessControl(md1);
                    }

                    //2017-07-10
                    FileInfo fiFileInfo = new FileInfo(strDirectory + @"DB\" + "InteractPayrollClient.mdf");
#if (DEBUG)
                    fiFileInfo = new FileInfo(strDirectory + @"DB\" + "InteractPayrollClient_Debug.mdf");
#endif
                    if (fiFileInfo.Exists == true)
                    {

                        return;
                    }

                    fiFileInfo = new FileInfo(strDirectory + "CreateDatabaseInfo.txt");

                    if (fiFileInfo.Exists == true)
                    {
                        fiFileInfo.Delete();
                    }

                    StreamWriter swStreamWriter = fiFileInfo.AppendText();
                    swStreamWriter.WriteLine("Before Create");
                    swStreamWriter.Close();

                    StringBuilder strQry = new StringBuilder();
                    strQry.AppendLine("  USE master CREATE DATABASE InteractPayrollClient ON (NAME=InteractPayrollClient,FILENAME='" + strDirectory + @"DB\InteractPayrollClient.mdf')");

                    Execute_SQLCommand_CreateDB(strQry.ToString());

                    //2017-07-10
                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" name ");

                    strQry.AppendLine(" FROM sys.server_principals ");

                    strQry.AppendLine(" WHERE name = 'Interact' ");

                    Create_DataTable_Client(strQry.ToString(), DataSet, "UserInteractExists");

                    if (DataSet.Tables["UserInteractExists"].Rows.Count == 0)
                    {
                        strQry.Clear();

                        strQry.AppendLine(" CREATE LOGIN [Interact] WITH PASSWORD = N'erawnacs', CHECK_EXPIRATION = OFF, CHECK_POLICY = OFF");

                        Execute_SQLCommand_Client(strQry.ToString());

                        strQry.Clear();
                        strQry.AppendLine(" EXEC master..sp_addsrvrolemember @loginame = N'Interact', @rolename = N'sysadmin'");

                        Execute_SQLCommand_Client(strQry.ToString());
                    }

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE COMPANY(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("COMPANY_DESC varchar(40) NULL,");
                    strQry.AppendLine("RES_UNIT_NUMBER varchar(8) NULL,");
                    strQry.AppendLine("RES_COMPLEX varchar(30) NULL,");
                    strQry.AppendLine("RES_STREET_NUMBER varchar(8) NULL,");
                    strQry.AppendLine("RES_STREET_NAME varchar(30) NULL,");
                    strQry.AppendLine("RES_SUBURB varchar(30) NULL,");
                    strQry.AppendLine("RES_CITY varchar(30) NULL,");
                    strQry.AppendLine("RES_ADDR_CODE varchar(8) NULL,");
                    strQry.AppendLine("POST_ADDR_LINE1 varchar(40) NULL,");
                    strQry.AppendLine("POST_ADDR_LINE2 varchar(40) NULL,");
                    strQry.AppendLine("POST_ADDR_LINE3 varchar(40) NULL,");
                    strQry.AppendLine("POST_ADDR_LINE4 varchar(40) NULL,");
                    strQry.AppendLine("POST_ADDR_CODE varchar(8) NULL,");
                    strQry.AppendLine("TEL_WORK varchar(15) NULL,");
                    strQry.AppendLine("TEL_FAX varchar(15) NULL,");
                    strQry.AppendLine("FINGERPRINT_TAG_IND varchar(1) NULL,");
                    strQry.AppendLine("WAGES_EXTRA_DAYS smallint NULL,");
                    strQry.AppendLine("FINGERPRINT_ENGINE varchar(50) NULL,");

                    strQry.AppendLine(" CONSTRAINT PK_COMPANY PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");
                    
                    Execute_SQLCommand_Client(strQry.ToString());
                    
                    //2018-11-03
                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE CONSOLE_LINES(");
                    strQry.AppendLine("CONSOLE_LINE_NO int NOT NULL,");
                    strQry.AppendLine("CONSOLE_LINE_MESSAGE varchar(max) NULL,");
                    
                    strQry.AppendLine(" CONSTRAINT PK_CONSOLE_LINES PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("CONSOLE_LINE_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());
                    
                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE DATES(");
                    strQry.AppendLine("DAY_DATE date NOT NULL,");
                    strQry.AppendLine("DAY_NO int NULL,");

                    strQry.AppendLine(" CONSTRAINT PK_DATES PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("DAY_DATE ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

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

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE DEVICE(");
                    strQry.AppendLine("DEVICE_NO smallint NOT NULL,");
                    strQry.AppendLine("DEVICE_DESC varchar(20) NULL,");
                    strQry.AppendLine("DEVICE_USAGE varchar(1) NULL,");
                    strQry.AppendLine("TIME_ATTEND_CLOCK_FIRST_LAST_IND varchar(1) NULL,");
                    strQry.AppendLine("CLOCK_IN_OUT_PARM varchar(1) NULL,");
                    strQry.AppendLine("CLOCK_IN_RANGE_FROM smallint NULL,");
                    strQry.AppendLine("CLOCK_IN_RANGE_TO smallint NULL,");
                    strQry.AppendLine("LOCK_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("COMPANY_NO int NULL,");
                    strQry.AppendLine("LAN_WAN_IND varchar(1) NULL,");
                    strQry.AppendLine("FAR_REQUESTED int NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_DEVICE PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("DEVICE_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE DEVICE_CLOCK_TIME(");
                    strQry.AppendLine("DEVICE_NO int NOT NULL,");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE varchar(1) NOT NULL,");
                    strQry.AppendLine("TIMESHEET_DATE date NOT NULL,");
                    strQry.AppendLine("TIMESHEET_TIME_MINUTES smallint NOT NULL,");
                    strQry.AppendLine("IN_OUT_IND varchar(1) NOT NULL,");
                    strQry.AppendLine("TIE_BREAKER smallint IDENTITY(1,1) NOT NULL,");
                    strQry.AppendLine("CLOCKED_BOUNDARY_TIME_MINUTES smallint NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_DEVICE_CLOCK_TIME PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("DEVICE_NO ASC,");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE ASC,");
                    strQry.AppendLine("TIMESHEET_DATE ASC,");
                    strQry.AppendLine("TIMESHEET_TIME_MINUTES ASC,");
                    strQry.AppendLine("IN_OUT_IND ASC,");
                    strQry.AppendLine("TIE_BREAKER ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE DEVICE_CLOCK_TIME_BREAK(");
                    strQry.AppendLine("DEVICE_NO int NOT NULL,");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE varchar(1) NOT NULL,");
                    strQry.AppendLine("BREAK_DATE date NOT NULL,");
                    strQry.AppendLine("BREAK_TIME_MINUTES smallint NOT NULL,");
                    strQry.AppendLine("IN_OUT_IND varchar(1) NOT NULL,");
                    strQry.AppendLine("TIE_BREAKER smallint IDENTITY(1,1) NOT NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_DEVICE_CLOCK_TIME_BREAK PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("DEVICE_NO ASC,");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE ASC,");
                    strQry.AppendLine("BREAK_DATE ASC,");
                    strQry.AppendLine("BREAK_TIME_MINUTES ASC,");
                    strQry.AppendLine("IN_OUT_IND ASC,");
                    strQry.AppendLine("TIE_BREAKER ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE DEVICE_DEPARTMENT_LINK(");
                    strQry.AppendLine("DEVICE_NO smallint NOT NULL,");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE varchar(1) NOT NULL,");
                    strQry.AppendLine("DEPARTMENT_NO smallint NOT NULL,");
                    strQry.AppendLine("MON_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("TUE_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("WED_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("THU_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("FRI_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("SAT_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("SUN_CLOCK_IN smallint NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_DEVICE_DEPARTMENT_LINK PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("DEVICE_NO ASC,");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE ASC,");
                    strQry.AppendLine("DEPARTMENT_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE DEVICE_DEPARTMENT_LINK_ACTIVE(");
                    strQry.AppendLine("DEVICE_NO smallint NOT NULL,");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE varchar(1) NOT NULL,");
                    strQry.AppendLine("DEPARTMENT_NO smallint NOT NULL,");
                    strQry.AppendLine("DEVICE_DEPARTMENT_LINK_ACTIVE_NO smallint NOT NULL,");
                    strQry.AppendLine("DEVICE_DEPARTMENT_LINK_ACTIVE_DESC varchar(30) NULL,");
                    strQry.AppendLine("MON_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("TUE_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("WED_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("THU_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("FRI_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("SAT_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("SUN_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("MON_CLOCK_OUT smallint NULL,");
                    strQry.AppendLine("TUE_CLOCK_OUT smallint NULL,");
                    strQry.AppendLine("WED_CLOCK_OUT smallint NULL,");
                    strQry.AppendLine("THU_CLOCK_OUT smallint NULL,");
                    strQry.AppendLine("FRI_CLOCK_OUT smallint NULL,");
                    strQry.AppendLine("SAT_CLOCK_OUT smallint NULL,");
                    strQry.AppendLine("SUN_CLOCK_OUT smallint NULL,");
                    strQry.AppendLine("MON_CLOCK_IN_APPLIES_IND varchar(1) NULL,");
                    strQry.AppendLine("TUE_CLOCK_IN_APPLIES_IND varchar(1) NULL,");
                    strQry.AppendLine("WED_CLOCK_IN_APPLIES_IND varchar(1) NULL,");
                    strQry.AppendLine("THU_CLOCK_IN_APPLIES_IND varchar(1) NULL,");
                    strQry.AppendLine("FRI_CLOCK_IN_APPLIES_IND varchar(1) NULL,");
                    strQry.AppendLine("SAT_CLOCK_IN_APPLIES_IND varchar(1) NULL,");
                    strQry.AppendLine("SUN_CLOCK_IN_APPLIES_IND varchar(1) NULL,");
                    strQry.AppendLine("TIME_ATTEND_ROUNDING_IND varchar(1) NULL,");
                    strQry.AppendLine("TIME_ATTEND_ROUNDING_VALUE smallint NULL,");
                    strQry.AppendLine("ACTIVE_IND varchar(1) NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_DEVICE_DEPARTMENT_LINK_ACTIVE PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("DEVICE_NO ASC,");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE ASC,");
                    strQry.AppendLine("DEPARTMENT_NO ASC,");
                    strQry.AppendLine("DEVICE_DEPARTMENT_LINK_ACTIVE_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE DEVICE_EMPLOYEE_LINK(");
                    strQry.AppendLine("DEVICE_NO smallint NOT NULL,");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE varchar(1) NOT NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_DEVICE_EMPLOYEE_LINK PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("DEVICE_NO ASC,");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE DEVICE_GROUP_LINK(");
                    strQry.AppendLine("DEVICE_NO smallint NOT NULL,");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("GROUP_NO smallint NOT NULL,");
                    strQry.AppendLine("MON_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("TUE_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("WED_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("THU_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("FRI_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("SAT_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("SUN_CLOCK_IN smallint NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_DEVICE_GROUP_LINK PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("DEVICE_NO ASC,");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("GROUP_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE DEVICE_GROUP_LINK_ACTIVE(");
                    strQry.AppendLine("DEVICE_NO smallint NOT NULL,");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("GROUP_NO smallint NOT NULL,");
                    strQry.AppendLine("DEVICE_GROUP_LINK_ACTIVE_NO smallint NOT NULL,");
                    strQry.AppendLine("DEVICE_GROUP_LINK_ACTIVE_DESC varchar(30) NULL,");
                    strQry.AppendLine("MON_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("TUE_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("WED_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("THU_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("FRI_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("SAT_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("SUN_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("MON_CLOCK_OUT smallint NULL,");
                    strQry.AppendLine("TUE_CLOCK_OUT smallint NULL,");
                    strQry.AppendLine("WED_CLOCK_OUT smallint NULL,");
                    strQry.AppendLine("THU_CLOCK_OUT smallint NULL,");
                    strQry.AppendLine("FRI_CLOCK_OUT smallint NULL,");
                    strQry.AppendLine("SAT_CLOCK_OUT smallint NULL,");
                    strQry.AppendLine("SUN_CLOCK_OUT smallint NULL,");
                    strQry.AppendLine("MON_CLOCK_IN_APPLIES_IND varchar(1) NULL,");
                    strQry.AppendLine("TUE_CLOCK_IN_APPLIES_IND varchar(1) NULL,");
                    strQry.AppendLine("WED_CLOCK_IN_APPLIES_IND varchar(1) NULL,");
                    strQry.AppendLine("THU_CLOCK_IN_APPLIES_IND varchar(1) NULL,");
                    strQry.AppendLine("FRI_CLOCK_IN_APPLIES_IND varchar(1) NULL,");
                    strQry.AppendLine("SAT_CLOCK_IN_APPLIES_IND varchar(1) NULL,");
                    strQry.AppendLine("SUN_CLOCK_IN_APPLIES_IND varchar(1) NULL,");
                    strQry.AppendLine("TIME_ATTEND_ROUNDING_IND varchar(1) NULL,");
                    strQry.AppendLine("TIME_ATTEND_ROUNDING_VALUE smallint NULL,");
                    strQry.AppendLine("ACTIVE_IND varchar(1) NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_DEVICE_GROUP_LINK_ACTIVE PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("DEVICE_NO ASC,");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("GROUP_NO ASC,");
                    strQry.AppendLine("DEVICE_GROUP_LINK_ACTIVE_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE DEVICE_PAY_CATEGORY_LINK(");
                    strQry.AppendLine("DEVICE_NO smallint NOT NULL,");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE varchar(1) NOT NULL,");
                    strQry.AppendLine("MON_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("TUE_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("WED_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("THU_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("FRI_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("SAT_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("SUN_CLOCK_IN smallint NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_DEVICE_PAY_CATEGORY_LINK PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("DEVICE_NO ASC,");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE DEVICE_PAY_CATEGORY_LINK_ACTIVE(");
                    strQry.AppendLine("DEVICE_NO smallint NOT NULL,");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE varchar(1) NOT NULL,");
                    strQry.AppendLine("DEVICE_PAY_CATEGORY_LINK_ACTIVE_NO smallint NOT NULL,");
                    strQry.AppendLine("DEVICE_PAY_CATEGORY_LINK_ACTIVE_DESC varchar(30) NULL,");
                    strQry.AppendLine("MON_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("TUE_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("WED_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("THU_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("FRI_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("SAT_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("SUN_CLOCK_IN smallint NULL,");
                    strQry.AppendLine("MON_CLOCK_OUT smallint NULL,");
                    strQry.AppendLine("TUE_CLOCK_OUT smallint NULL,");
                    strQry.AppendLine("WED_CLOCK_OUT smallint NULL,");
                    strQry.AppendLine("THU_CLOCK_OUT smallint NULL,");
                    strQry.AppendLine("FRI_CLOCK_OUT smallint NULL,");
                    strQry.AppendLine("SAT_CLOCK_OUT smallint NULL,");
                    strQry.AppendLine("SUN_CLOCK_OUT smallint NULL,");
                    strQry.AppendLine("MON_CLOCK_IN_APPLIES_IND varchar(1) NULL,");
                    strQry.AppendLine("TUE_CLOCK_IN_APPLIES_IND varchar(1) NULL,");
                    strQry.AppendLine("WED_CLOCK_IN_APPLIES_IND varchar(1) NULL,");
                    strQry.AppendLine("THU_CLOCK_IN_APPLIES_IND varchar(1) NULL,");
                    strQry.AppendLine("FRI_CLOCK_IN_APPLIES_IND varchar(1) NULL,");
                    strQry.AppendLine("SAT_CLOCK_IN_APPLIES_IND varchar(1) NULL,");
                    strQry.AppendLine("SUN_CLOCK_IN_APPLIES_IND varchar(1) NULL,");
                    strQry.AppendLine("TIME_ATTEND_ROUNDING_IND varchar(1) NULL,");
                    strQry.AppendLine("TIME_ATTEND_ROUNDING_VALUE smallint NULL,");
                    strQry.AppendLine("ACTIVE_IND varchar(1) NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_DEVICE_PAY_CATEGORY_LINK_ACTIVE PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("DEVICE_NO ASC,");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE ASC,");
                    strQry.AppendLine("DEVICE_PAY_CATEGORY_LINK_ACTIVE_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE DYNAMIC_FILE_DOWNLOAD_CHECK(");
                    strQry.AppendLine("FILE_DOWNLOAD_CHECK_DATE smalldatetime NOT NULL");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE EMPLOYEE(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE varchar(1) NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_CODE varchar(10) NULL,");
                    strQry.AppendLine("EMPLOYEE_NAME varchar(30) NULL,");
                    strQry.AppendLine("EMPLOYEE_SURNAME varchar(30) NULL,");
                    strQry.AppendLine("EMPLOYEE_LAST_RUNDATE smalldatetime NULL,");
                    strQry.AppendLine("NOT_ACTIVE_IND varchar(1) NULL,");
                    strQry.AppendLine("DEPARTMENT_NO int NULL,");
                    strQry.AppendLine("USE_EMPLOYEE_NO_IND varchar(1) NULL,");
                    strQry.AppendLine("KEEP_IND varchar(1) NULL,");
                    strQry.AppendLine("EMPLOYEE_PIN varchar(10) NULL,");
                    strQry.AppendLine("EMPLOYEE_3RD_PARTY_CODE varchar(20) NULL,");
                    strQry.AppendLine("EMPLOYEE_RFID_CARD_NO varchar(15) NULL,");
                    strQry.AppendLine("UPLOAD_CLOCK_PARAMETERS_IND varchar(1) NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_EMPLOYEE PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE EMPLOYEE_BREAK_CURRENT(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("BREAK_DATE date NOT NULL,");
                    strQry.AppendLine("BREAK_SEQ tinyint NOT NULL,");
                    strQry.AppendLine("BREAK_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("BREAK_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("BREAK_ACCUM_MINUTES smallint NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_EMPLOYEE_BREAK_CURRENT PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("BREAK_DATE ASC,");
                    strQry.AppendLine("BREAK_SEQ ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE EMPLOYEE_BREAK_HISTORY(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("BREAK_DATE date NOT NULL,");
                    strQry.AppendLine("BREAK_SEQ tinyint NOT NULL,");
                    strQry.AppendLine("BREAK_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("BREAK_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("BREAK_ACCUM_MINUTES smallint NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_EMPLOYEE_BREAK_HISTORY PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("BREAK_DATE ASC,");
                    strQry.AppendLine("BREAK_SEQ ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

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

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE EMPLOYEE_EMPLOYEE_LINK(");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO_LINK int NOT NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_EMPLOYEE_EMPLOYEE_LINK PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO_LINK ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE EMPLOYEE_FINGERPRINT_TEMPLATE(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("FINGER_NO smallint NOT NULL,");
                    strQry.AppendLine("FINGER_TEMPLATE varbinary(2500) NULL,");
                    strQry.AppendLine("CREATION_DATETIME datetime NULL,");
                    strQry.AppendLine("KEEP_IND varchar(500) NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_EMPLOYEE_FINGERPRINT_TEMPLATE PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("FINGER_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE EMPLOYEE_FINGERPRINT_TEMPLATE_DELETE(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("FINGER_NO smallint NOT NULL,");
                    strQry.AppendLine("CREATION_DATETIME datetime NOT NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_EMPLOYEE_FINGERPRINT_TEMPLATE_DELETE PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("FINGER_NO ASC,");
                    strQry.AppendLine("CREATION_DATETIME ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE EMPLOYEE_FINGERPRINT_TEMPLATE_TEMP(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("FINGER_NO smallint NOT NULL,");
                    strQry.AppendLine("FINGER_TEMPLATE_NO smallint NOT NULL,");
                    strQry.AppendLine("FINGER_TEMPLATE varbinary(2500) NULL,");
                    strQry.AppendLine("FINGER_FEATURES varbinary(500) NULL,");
                    strQry.AppendLine("FINGER_IMAGE image NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_EMPLOYEE_FINGERPRINT_TEMPLATE_TEMP PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("FINGER_NO ASC,");
                    strQry.AppendLine("FINGER_TEMPLATE_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE EMPLOYEE_PAY_CATEGORY(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE varchar(1) NOT NULL,");
                    strQry.AppendLine("KEEP_IND varchar(1) NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_EMPLOYEE_PAY_CATEGORY PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE EMPLOYEE_PAY_CATEGORY_LINK(");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE varchar(1) NOT NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_EMPLOYEE_PAY_CATEGORY_LINK PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE EMPLOYEE_RFID_CARD(");
                    strQry.AppendLine("EMPLOYEE_RFID_CARD_NO int NOT NULL,");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_EMPLOYEE_RFID_CARD_1 PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("EMPLOYEE_RFID_CARD_NO ASC,");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE EMPLOYEE_SALARY_BREAK_CURRENT(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("BREAK_DATE date NOT NULL,");
                    strQry.AppendLine("BREAK_SEQ tinyint NOT NULL,");
                    strQry.AppendLine("BREAK_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("BREAK_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("BREAK_ACCUM_MINUTES smallint NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_EMPLOYEE_SALARY_BREAK_CURRENT PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("BREAK_DATE ASC,");
                    strQry.AppendLine("BREAK_SEQ ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE EMPLOYEE_SALARY_BREAK_HISTORY(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("BREAK_DATE date NOT NULL,");
                    strQry.AppendLine("BREAK_SEQ tinyint NOT NULL,");
                    strQry.AppendLine("BREAK_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("BREAK_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("BREAK_ACCUM_MINUTES smallint NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_EMPLOYEE_SALARY_BREAK_HISTORY PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("BREAK_DATE ASC,");
                    strQry.AppendLine("BREAK_SEQ ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

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

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE EMPLOYEE_SALARY_TIMESHEET_CURRENT(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("TIMESHEET_DATE date NOT NULL,");
                    strQry.AppendLine("TIMESHEET_SEQ tinyint NOT NULL,");
                    strQry.AppendLine("TIMESHEET_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("TIMESHEET_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("TIMESHEET_ACCUM_MINUTES smallint NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_EMPLOYEE_SALARY_TIMESHEET_CURRENT PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("TIMESHEET_DATE ASC,");
                    strQry.AppendLine("TIMESHEET_SEQ ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE EMPLOYEE_SALARY_TIMESHEET_HISTORY(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("TIMESHEET_DATE date NOT NULL,");
                    strQry.AppendLine("TIMESHEET_SEQ tinyint NOT NULL,");
                    strQry.AppendLine("TIMESHEET_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("TIMESHEET_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("TIMESHEET_ACCUM_MINUTES smallint NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_EMPLOYEE_SALARY_TIMESHEET_HISTORY PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("TIMESHEET_DATE ASC,");
                    strQry.AppendLine("TIMESHEET_SEQ ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    //2013-07-09
                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE EMPLOYEE_TIME_ATTEND_BREAK_CURRENT(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("BREAK_DATE date NOT NULL,");
                    strQry.AppendLine("BREAK_SEQ tinyint NOT NULL,");
                    strQry.AppendLine("BREAK_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("BREAK_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("BREAK_ACCUM_MINUTES smallint NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_EMPLOYEE_TIME_ATTEND_BREAK_CURRENT PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("BREAK_DATE ASC,");
                    strQry.AppendLine("BREAK_SEQ ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE EMPLOYEE_TIME_ATTEND_BREAK_HISTORY(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("BREAK_DATE date NOT NULL,");
                    strQry.AppendLine("BREAK_SEQ tinyint NOT NULL,");
                    strQry.AppendLine("BREAK_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("BREAK_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("BREAK_ACCUM_MINUTES smallint NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_EMPLOYEE_TIME_ATTEND_BREAK_HISTORY PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("BREAK_DATE ASC,");
                    strQry.AppendLine("BREAK_SEQ ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

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

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("TIMESHEET_DATE date NOT NULL,");
                    strQry.AppendLine("TIMESHEET_SEQ tinyint NOT NULL,");
                    strQry.AppendLine("TIMESHEET_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("TIMESHEET_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("TIMESHEET_ACCUM_MINUTES smallint NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("TIMESHEET_DATE ASC,");
                    strQry.AppendLine("TIMESHEET_SEQ ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE EMPLOYEE_TIME_ATTEND_TIMESHEET_HISTORY(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("TIMESHEET_DATE date NOT NULL,");
                    strQry.AppendLine("TIMESHEET_SEQ tinyint NOT NULL,");
                    strQry.AppendLine("TIMESHEET_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("TIMESHEET_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("TIMESHEET_ACCUM_MINUTES smallint NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_EMPLOYEE_TIME_ATTEND_TIMESHEET_HISTORY PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("TIMESHEET_DATE ASC,");
                    strQry.AppendLine("TIMESHEET_SEQ ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

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

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE EMPLOYEE_TIMESHEET_CURRENT(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("TIMESHEET_DATE date NOT NULL,");
                    strQry.AppendLine("TIMESHEET_SEQ tinyint NOT NULL,");
                    strQry.AppendLine("TIMESHEET_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("TIMESHEET_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("TIMESHEET_ACCUM_MINUTES smallint NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_EMPLOYEE_TIMESHEET_CURRENT PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("TIMESHEET_DATE ASC,");
                    strQry.AppendLine("TIMESHEET_SEQ ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE EMPLOYEE_TIMESHEET_HISTORY(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("TIMESHEET_DATE date NOT NULL,");
                    strQry.AppendLine("TIMESHEET_SEQ tinyint NOT NULL,");
                    strQry.AppendLine("TIMESHEET_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("TIMESHEET_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_IN_MINUTES smallint NULL,");
                    strQry.AppendLine("CLOCKED_TIME_OUT_MINUTES smallint NULL,");
                    strQry.AppendLine("INDICATOR varchar(1) NULL,");
                    strQry.AppendLine("TIMESHEET_ACCUM_MINUTES smallint NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_EMPLOYEE_TIMESHEET_HISTORY PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("TIMESHEET_DATE ASC,");
                    strQry.AppendLine("TIMESHEET_SEQ ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE EMPLOYEE_USER_LINK(");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("USER_NO int NOT NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_EMPLOYEE_USER_LINK PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("USER_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE FILE_CLIENT_DOWNLOAD_CHUNKS(");
                    strQry.AppendLine("FILE_LAYER_IND varchar(1) NOT NULL,");
                    strQry.AppendLine("FILE_NAME varchar(50) NOT NULL,");
                    strQry.AppendLine("FILE_CHUNK_NO int NOT NULL,");
                    strQry.AppendLine("FILE_CHUNK image NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_FILE_CLIENT_DOWNLOAD_CHUNKS PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("FILE_LAYER_IND ASC,");
                    strQry.AppendLine("FILE_NAME ASC,");
                    strQry.AppendLine("FILE_CHUNK_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP(");
                    strQry.AppendLine("FILE_LAYER_IND varchar(1) NOT NULL,");
                    strQry.AppendLine("FILE_NAME varchar(50) NOT NULL,");
                    strQry.AppendLine("FILE_CHUNK_NO int NOT NULL,");
                    strQry.AppendLine("FILE_CHUNK image NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("FILE_LAYER_IND ASC,");
                    strQry.AppendLine("FILE_NAME ASC,");
                    strQry.AppendLine("FILE_CHUNK_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE FILE_CLIENT_DOWNLOAD_DETAILS(");
                    strQry.AppendLine("FILE_LAYER_IND varchar(1) NOT NULL,");
                    strQry.AppendLine("FILE_NAME varchar(50) NOT NULL,");
                    strQry.AppendLine("FILE_LAST_UPDATED_DATE datetime NULL,");
                    strQry.AppendLine("FILE_SIZE int NULL,");
                    strQry.AppendLine("FILE_SIZE_COMPRESSED int NULL,");
                    strQry.AppendLine("FILE_VERSION_NO varchar(15) NULL,");
                    strQry.AppendLine("FILE_CRC_VALUE varchar(15) NULL,");
                    strQry.AppendLine("CONSTRAINT PK_FILE_CLIENT_DOWNLOAD_DETAILS PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("FILE_LAYER_IND ASC,");
                    strQry.AppendLine("FILE_NAME ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());
                    
                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE FILE_CLIENT_UPLOAD_DETAILS(");

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

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE FILE_CLIENT_UPLOAD_CHUNKS(");
                    strQry.AppendLine("FILE_NAME varchar(150) NOT NULL,");
                    strQry.AppendLine("FILE_CHUNK_NO int NOT NULL,");
                    strQry.AppendLine("FILE_CHUNK image NOT NULL,");
                    strQry.AppendLine("CONSTRAINT PK_FILE_CLIENT_UPLOAD_CHUNKS PRIMARY KEY CLUSTERED ");
                    strQry.AppendLine("(");
                    strQry.AppendLine("FILE_NAME ASC,");
                    strQry.AppendLine("FILE_CHUNK_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());
                    
                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE FINGERPRINT_IDENTIFY_VERIFY_THRESHOLD(");
                    strQry.AppendLine("IDENTIFY_THRESHOLD_VALUE int NULL,");
                    strQry.AppendLine("VERIFY_THRESHOLD_VALUE int NULL");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE FINGERPRINT_SOFTWARE_TO_USE(");
                    strQry.AppendLine("FINGERPRINT_SOFTWARE_IND varchar(1) NULL");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE GROUP_EMPLOYEE_LINK(");
                    strQry.AppendLine("GROUP_NO int NOT NULL,");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE varchar(1) NOT NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_GROUP_EMPLOYEE_LINK PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("GROUP_NO ASC,");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE GROUPS(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("GROUP_NO smallint NOT NULL,");
                    strQry.AppendLine("GROUP_DESC varchar(30) NOT NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_GROUPS PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("GROUP_NO ASC,");
                    strQry.AppendLine("GROUP_DESC ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE PAY_CATEGORY(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE varchar(1) NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_DESC varchar(30) NOT NULL,");
                    strQry.AppendLine("LUNCH_TIME_MINUTES_DEDUCTED smallint NULL,");
                    strQry.AppendLine("LUNCH_TIME_WORKED_MINUTES smallint NULL,");
                    strQry.AppendLine("DAILY_ROUNDING_IND smallint NULL,");
                    strQry.AppendLine("DAILY_ROUNDING_MINUTES smallint NULL,");
                    strQry.AppendLine("EXCEPTION_SUN_ABOVE_MINUTES smallint NULL,");
                    strQry.AppendLine("EXCEPTION_SUN_BELOW_MINUTES smallint NULL,");
                    strQry.AppendLine("EXCEPTION_MON_ABOVE_MINUTES smallint NULL,");
                    strQry.AppendLine("EXCEPTION_MON_BELOW_MINUTES smallint NULL,");
                    strQry.AppendLine("EXCEPTION_TUE_ABOVE_MINUTES smallint NULL,");
                    strQry.AppendLine("EXCEPTION_TUE_BELOW_MINUTES smallint NULL,");
                    strQry.AppendLine("EXCEPTION_WED_ABOVE_MINUTES smallint NULL,");
                    strQry.AppendLine("EXCEPTION_WED_BELOW_MINUTES smallint NULL,");
                    strQry.AppendLine("EXCEPTION_THU_ABOVE_MINUTES smallint NULL,");
                    strQry.AppendLine("EXCEPTION_THU_BELOW_MINUTES smallint NULL,");
                    strQry.AppendLine("EXCEPTION_FRI_ABOVE_MINUTES smallint NULL,");
                    strQry.AppendLine("EXCEPTION_FRI_BELOW_MINUTES smallint NULL,");
                    strQry.AppendLine("EXCEPTION_SAT_ABOVE_MINUTES smallint NULL,");
                    strQry.AppendLine("EXCEPTION_SAT_BELOW_MINUTES smallint NULL,");
                    strQry.AppendLine("NO_EDIT_IND varchar(1) NULL,");
                    strQry.AppendLine("LAST_UPLOAD_DATETIME smalldatetime NULL,");
                    strQry.AppendLine("LAST_DOWNLOAD_DATETIME datetime NULL,");
                    strQry.AppendLine("USER_INTERACT_IND varchar(1) NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_PAY_CATEGORY PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE PAY_CATEGORY_BREAK(");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE varchar(1) NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_BREAK_NO smallint NOT NULL,");
                    strQry.AppendLine("WORKED_TIME_MINUTES smallint NULL,");
                    strQry.AppendLine("BREAK_MINUTES smallint NULL,");
                    strQry.AppendLine("KEEP_IND varchar(1) NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_PAY_CATEGORY_BREAK PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE ASC,");
                    strQry.AppendLine("PAY_CATEGORY_BREAK_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE READ_OPTION(");
                    strQry.AppendLine("READ_OPTION_NO tinyint NOT NULL,");
                    strQry.AppendLine("READ_OPTION_DESC varchar(40) NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_READ_OPTION PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("READ_OPTION_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE REMOTE_BACKUP_SITE_NAME(");
                    strQry.AppendLine("SITE_NAME varchar(30) NULL");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE TEMP_DATES(");
                    strQry.AppendLine("USER_NO int NOT NULL,");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("TIMESHEET_DATE date NOT NULL,");
                    strQry.AppendLine("INDICATOR varchar(1) NULL");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE USER_COMPANY_ACCESS(");
                    strQry.AppendLine("USER_NO int NOT NULL,");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("COMPANY_ACCESS_IND varchar(1) NULL,");
                    strQry.AppendLine("KEEP_IND varchar(1) NULL,");
                    strQry.AppendLine("ACCESS_LAYER_IND varchar(1) NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_USER_COMPANY_ACCESS PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("USER_NO ASC,");
                    strQry.AppendLine("COMPANY_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE USER_DEPARTMENT(");
                    strQry.AppendLine("USER_NO int NOT NULL,");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("DEPARTMENT_NO smallint NOT NULL,");
                    strQry.AppendLine("KEEP_IND varchar(1) NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_USER_DEPARTMENT PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("USER_NO ASC,");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("DEPARTMENT_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE USER_EMPLOYEE(");
                    strQry.AppendLine("USER_NO int NOT NULL,");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE varchar(1) NOT NULL,");
                    strQry.AppendLine("KEEP_IND varchar(1) NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_USER_EMPLOYEE PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("USER_NO ASC,");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE USER_EMPLOYEE_PAY_CATEGORY_TEMP(");
                    strQry.AppendLine("USER_NO int NOT NULL,");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("EMPLOYEE_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE varchar(1) NOT NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_USER_EMPLOYEE_PAY_CATEGORY_TEMP PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("USER_NO ASC,");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("EMPLOYEE_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE USER_FINGERPRINT_TEMPLATE(");
                    strQry.AppendLine("USER_NO int NOT NULL,");
                    strQry.AppendLine("FINGER_NO smallint NOT NULL,");
                    strQry.AppendLine("FINGER_TEMPLATE varbinary(2500) NULL,");
                    strQry.AppendLine("CREATION_DATETIME datetime NULL,");
                    strQry.AppendLine("KEEP_IND varchar(1) NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_USER_FINGERPRINT_TEMPLATE PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("USER_NO ASC,");
                    strQry.AppendLine("FINGER_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    //2017-05-05
                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE USER_FINGERPRINT_TEMPLATE_DELETE(");
                    strQry.AppendLine("USER_NO int NOT NULL,");
                    strQry.AppendLine("FINGER_NO smallint NOT NULL,");
                    strQry.AppendLine("CREATION_DATETIME datetime NOT NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_USER_FINGERPRINT_TEMPLATE_DELETE PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("USER_NO ASC,");
                    strQry.AppendLine("FINGER_NO ASC,");
                    strQry.AppendLine("CREATION_DATETIME ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE USER_FINGERPRINT_TEMPLATE_TEMP(");
                    strQry.AppendLine("USER_NO int NOT NULL,");
                    strQry.AppendLine("FINGER_NO smallint NOT NULL,");
                    strQry.AppendLine("FINGER_TEMPLATE_NO smallint NOT NULL,");
                    strQry.AppendLine("FINGER_TEMPLATE varbinary(2500) NULL,");
                    strQry.AppendLine("FINGER_FEATURES varbinary(500) NULL,");
                    strQry.AppendLine("FINGER_IMAGE image NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_USER_FINGERPRINT_TEMPLATE_TEMP PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("USER_NO ASC,");
                    strQry.AppendLine("FINGER_NO ASC,");
                    strQry.AppendLine("FINGER_TEMPLATE_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE USER_ID(");
                    strQry.AppendLine("USER_NO int NOT NULL,");
                    strQry.AppendLine("USER_ID varchar(10) NULL,");
                    strQry.AppendLine("FIRSTNAME varchar(25) NULL,");
                    strQry.AppendLine("SURNAME varchar(25) NULL,");
                    strQry.AppendLine("SYSTEM_ADMINISTRATOR_IND varchar(1) NULL,");
                    strQry.AppendLine("PASSWORD varchar(10) NULL,");
                    strQry.AppendLine("EMAIL varchar(50) NULL,");
                    strQry.AppendLine("RESET varchar(1) NULL,");
                    strQry.AppendLine("USER_CLOCK_PIN varchar(15) NULL,");
                    strQry.AppendLine("LAST_TIME_ON smalldatetime NULL,");
                    strQry.AppendLine("INTERNET_IND varchar(1) NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_USER_ID PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("USER_NO ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE USER_PAY_CATEGORY(");
                    strQry.AppendLine("USER_NO int NOT NULL,");
                    strQry.AppendLine("COMPANY_NO int NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_NO smallint NOT NULL,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE varchar(1) NOT NULL,");
                    strQry.AppendLine("KEEP_IND varchar(1) NULL,");
                    strQry.AppendLine(" CONSTRAINT PK_USER_PAY_CATEGORY PRIMARY KEY CLUSTERED");
                    strQry.AppendLine("(");
                    strQry.AppendLine("USER_NO ASC,");
                    strQry.AppendLine("COMPANY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_NO ASC,");
                    strQry.AppendLine("PAY_CATEGORY_TYPE ASC");
                    strQry.AppendLine(") ON [PRIMARY]");
                    strQry.AppendLine(") ON [PRIMARY]");

                    Execute_SQLCommand_Client(strQry.ToString());

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

                    Execute_SQLCommand_Client(strQry.ToString());
                    
                    //2018-11-03
                    strQry.Clear();

                    strQry.AppendLine("CREATE TABLE VALIDITE_HOSTING_SERVICE(");
                    strQry.AppendLine("DYNAMIC_UPLOAD_TIMESHEETS_RUNNING_IND varchar(1) NULL,");
                    strQry.AppendLine("START_DYNAMIC_UPLOAD_TIMESHEETS_RUN_IND varchar(1) NULL");
                    strQry.AppendLine(") ON [PRIMARY]");

                Execute_SQLCommand_Client(strQry.ToString());
                    
                    strQry.Clear();

                    strQry.AppendLine("INSERT INTO READ_OPTION");
                    strQry.AppendLine("(READ_OPTION_NO");
                    strQry.AppendLine(",READ_OPTION_DESC)");
                    strQry.AppendLine(" VALUES");
                    strQry.AppendLine("(0");
                    strQry.AppendLine(",'None')");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("INSERT INTO READ_OPTION");
                    strQry.AppendLine("(READ_OPTION_NO");
                    strQry.AppendLine(",READ_OPTION_DESC)");
                    strQry.AppendLine(" VALUES");
                    strQry.AppendLine("(1");
                    strQry.AppendLine(",'FingerPrint')");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("INSERT INTO READ_OPTION");
                    strQry.AppendLine("(READ_OPTION_NO");
                    strQry.AppendLine(",READ_OPTION_DESC)");
                    strQry.AppendLine(" VALUES");
                    strQry.AppendLine("(2");
                    strQry.AppendLine(",'RFID Card Only')");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("INSERT INTO READ_OPTION");
                    strQry.AppendLine("(READ_OPTION_NO");
                    strQry.AppendLine(",READ_OPTION_DESC)");
                    strQry.AppendLine(" VALUES");
                    strQry.AppendLine("(3");
                    strQry.AppendLine(",'RFID Card And FingerPrint')");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("INSERT INTO READ_OPTION");
                    strQry.AppendLine("(READ_OPTION_NO");
                    strQry.AppendLine(",READ_OPTION_DESC)");
                    strQry.AppendLine(" VALUES");
                    strQry.AppendLine("(4");
                    strQry.AppendLine(",'Employee No. And Fingerprint')");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("INSERT INTO READ_OPTION");
                    strQry.AppendLine("(READ_OPTION_NO");
                    strQry.AppendLine(",READ_OPTION_DESC)");
                    strQry.AppendLine(" VALUES");
                    strQry.AppendLine("(5");
                    strQry.AppendLine(",'Employee No. And Pin')");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("INSERT INTO FINGERPRINT_IDENTIFY_VERIFY_THRESHOLD");
                    strQry.AppendLine("(IDENTIFY_THRESHOLD_VALUE");
                    strQry.AppendLine(",VERIFY_THRESHOLD_VALUE)");
                    strQry.AppendLine(" VALUES");
                    strQry.AppendLine("(45");
                    strQry.AppendLine(",25)");

                    Execute_SQLCommand_Client(strQry.ToString());

                    //D=Digital Persona
                    strQry.Clear();

                    strQry.AppendLine("INSERT INTO FINGERPRINT_SOFTWARE_TO_USE");
                    strQry.AppendLine("(FINGERPRINT_SOFTWARE_IND)");
                    strQry.AppendLine(" VALUES");
                    strQry.AppendLine("('D')");

                    Execute_SQLCommand_Client(strQry.ToString());

                    swStreamWriter = fiFileInfo.AppendText();

                    swStreamWriter.WriteLine("DB Created Successfully.");

                    swStreamWriter.Close();

                    fiFileInfo = new FileInfo(strDirectory + "DBConfig.txt");

                    if (fiFileInfo.Exists == true)
                    {
                        fiFileInfo.Delete();
                    }

                    swStreamWriter = fiFileInfo.AppendText();

//                    if (this.cboServer.SelectedItem.ToString().ToUpper().IndexOf("MSSQLLOCALDB") > -1)
//                    {
//                        string strLine = this.cboServer.Text.Trim() + ";AttachDBFilename=" + di.FullName;
//#if (DEBUG)
//                        strLine += "\\InteractPayrollClient_Debug.mdf;";
//#else
//                            strLine += "\\InteractPayrollClient.mdf;";
//#endif
//                        swStreamWriter.WriteLine(strLine);
//                    }
//                    else
//                    {
                        swStreamWriter.WriteLine(this.cboServer.Text.Trim());
//                    }

                    swStreamWriter.Close();

                    MessageBox.Show("Database has been Created.", this.Text,
                                   MessageBoxButtons.OK, MessageBoxIcon.Question);

                    this.txtDatabaseExists.Text = "true";

                    this.btnDeleteDownloadFiles.Enabled = true;
                    this.btnDelete.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                string strMessage = "Create Database Exception=" + ex.Message;

                if (ex.InnerException != null)
                {
                    strMessage += "\n\n" + ex.InnerException.Message;
                }

                FileInfo fiFileInfo = new FileInfo(strDirectory + "CreateDatabaseError.txt");

                StreamWriter swStreamWriter = fiFileInfo.AppendText();

                swStreamWriter.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + strMessage);

                swStreamWriter.Close();
                
                MessageBox.Show(strMessage, "Error", MessageBoxButtons.OK);
            }
        }
   
        private void Execute_SQLCommand_CreateDB(string parstrQry)
        {
#if(DEBUG)
            if (parstrQry.IndexOf("ALTER AUTHORIZATION ON DATABASE::") == -1)
            {
                parstrQry = parstrQry.Replace("InteractPayrollClient", "InteractPayrollClient_Debug");
            }
#endif

            pvtSqlConnectionClientIntegratedSecurity = new SqlConnection(pvtstrConnectionClientIntegratedSecurity);

            pvtSqlCommandClientIntegratedSecurity = new SqlCommand(parstrQry, pvtSqlConnectionClientIntegratedSecurity);

            pvtSqlCommandClientIntegratedSecurity.Connection.Open();

            pvtSqlCommandClientIntegratedSecurity.ExecuteNonQuery();

            pvtSqlConnectionClientIntegratedSecurity.Close();
        }

        public void Execute_SQLCommand_Client(string parstrQry)
        {
#if(DEBUG)
            parstrQry = parstrQry.Replace("InteractPayrollClient", "InteractPayrollClient_Debug");
#endif
            pvtSqlConnectionClient = new SqlConnection(pvtstrConnectionClient);

            pvtSqlCommandClient = new SqlCommand(parstrQry, pvtSqlConnectionClient);

            pvtSqlCommandClient.Connection.Open();

            pvtSqlCommandClient.ExecuteNonQuery();

            pvtSqlConnectionClient.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
#if(DEBUG)
                string strConnectionClient = @"Server=#Engine#;Database=InteractPayrollClient_Debug;Integrated Security=true; MultipleActiveResultSets=True;";
#else
                string strConnectionClient = @"Server=#Engine#;Database=InteractPayrollClient;Integrated Security=true; MultipleActiveResultSets=True;";
#endif
                pvtstrConnectionClient = strConnectionClient.Replace("#Engine#", this.cboServer.Text.Trim());

                DataSet DataSet = new System.Data.DataSet();

                string strQry = "";
                strQry = "";
                strQry += " SELECT ";
                strQry += " TABLE_NAME";
                strQry += " FROM InteractPayrollClient.INFORMATION_SCHEMA.TABLES ";

                Create_DataTable_Client(strQry, DataSet, "TableDelete");

                for (int intRow = 0; intRow < DataSet.Tables["TableDelete"].Rows.Count; intRow++)
                {
                    if (DataSet.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "DATES"
                    || DataSet.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "FINGERPRINT_IDENTIFY_VERIFY_THRESHOLD"
                    || DataSet.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "READ_OPTION")
                    {
                        continue;
                    }

                    strQry = "";
                    strQry += " DELETE FROM InteractPayrollClient.dbo." + DataSet.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString();
                   
                    Execute_SQLCommand_Client(strQry);
                }

                MessageBox.Show("Clean Database Successful.",
                         "",
                         MessageBoxButtons.OK,
                         MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                          "Error",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Error);
            }
        }

        private void frmISDatabaseUtility_Load(object sender, EventArgs e)
        {
            string strX86folder = Environment.GetEnvironmentVariable("ProgramFiles(x86)");

            if (strX86folder == "")
            {
                strX86folder = Environment.GetEnvironmentVariable("ProgramFiles");

                if (strX86folder == "")
                {
                    MessageBox.Show("Cannot Find ProgramFiles(x86)\n\nSpeak to Administrator.",
                         "Error",
                         MessageBoxButtons.OK,
                         MessageBoxIcon.Error);

                    return;
                }
                else
                {
                    this.txtWorkDirectory.Text = strX86folder;
                }
            }
            else
            {
                this.txtWorkDirectory.Text = strX86folder;
            }

            this.txtWindowsLogin.Text = Environment.MachineName + "\\" + Environment.UserName;
            
            this.cboAuthentication.SelectedIndex = 0;
#if(DEBUG)
            this.lblDb.Text = "InteractPayrollClient_Debug Exists?";
#else
            if (IsAdministrator() == false)
            {
                cboServer.Enabled = false;
                
                MessageBox.Show("This Program must be Run with Administrator Rights\n", this.Text,
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
#endif
        }

        private void btnDeleteDownloadFiles_Click(object sender, EventArgs e)
        {
            try
            {
#if(DEBUG)
                string strConnectionClient = @"Server=#Engine#;Database=InteractPayrollClient_Debug;Integrated Security=true; MultipleActiveResultSets=True;";
#else
                string strConnectionClient = @"Server=#Engine#;Database=InteractPayrollClient;Integrated Security=true; MultipleActiveResultSets=True;";
#endif
                pvtstrConnectionClient = strConnectionClient.Replace("#Engine#", this.cboServer.Text.Trim());
     
                string strQry = "";
                
                strQry = "";
                strQry += " DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_DETAILS";

                Execute_SQLCommand_Client(strQry);

                strQry = "";
                strQry += " DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS";

                Execute_SQLCommand_Client(strQry);
                
                MessageBox.Show("Clear Downloaded Dll Files Successful.",
                         "",
                         MessageBoxButtons.OK,
                         MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                          "Error",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Error);
            }
        }

        private void cboServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void btnDrop_Click(object sender, EventArgs e)
        {
            try
            {
                string strConnectionClient = @"Server=#Engine#;Database=InteractPayrollClient;Integrated Security=true; MultipleActiveResultSets=True;";
#if (DEBUG)
                strConnectionClient = strConnectionClient.Replace("InteractPayrollClient", "InteractPayrollClient_Debug");
#endif
                pvtstrConnectionClient = strConnectionClient.Replace("#Engine#", this.cboServer.Text.Trim());

                string strQry = "";
                strQry += " USE master DROP DATABASE InteractPayrollClient";
                
                Execute_SQLCommand_Client(strQry);

                this.txtDatabaseFileName.Text = "";
                this.btnDrop.Enabled = false;

                MessageBox.Show("Database Dropped Successfully.",
                         "",
                         MessageBoxButtons.OK,
                         MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                string strMessage = "Exception=" + ex.Message;

                if (ex.InnerException != null)
                {
                    strMessage += "\n\n" + ex.InnerException.Message;
                }

                MessageBox.Show(strMessage, "Error", MessageBoxButtons.OK);
            }
        }

        

        private void btnAction_Click(object sender, EventArgs e)
        {
            StringBuilder strQry = new StringBuilder();

            if (this.btnSqlUserAction.Text == "Delete")
            {
                try
                {
                    DataSet DataSet = new DataSet();

                    strQry.AppendLine("EXEC master..sp_dropsrvrolemember 'Interact','sysadmin'");

                    Execute_SQLCommand_CreateDB(strQry.ToString());
                    
                    strQry.Clear();

                    strQry.AppendLine("SELECT name ");

                    strQry.AppendLine("FROM sys.databases ");

                    strQry.AppendLine("where suser_sname(owner_sid) = 'Interact' ");

                    Create_DataTable_Client(strQry.ToString(), DataSet, "UserInteractOwnsDB");
                    
                    for (int intRow = 0; intRow < DataSet.Tables["UserInteractOwnsDB"].Rows.Count; intRow++)
                    {
                        strQry.Clear();

                        strQry.AppendLine("ALTER AUTHORIZATION ON DATABASE::[" + DataSet.Tables["UserInteractOwnsDB"].Rows[intRow]["name"].ToString() + "] to sa ");

                        Execute_SQLCommand_CreateDB(strQry.ToString());
                    }
                    
                    strQry.Clear();
                    strQry.AppendLine("DROP LOGIN [Interact] ");

                    Execute_SQLCommand_CreateDB(strQry.ToString());
                    
                    this.btnSqlUserAction.Text = "Create";
                    this.txtInteractUser.Text = "";

                    MessageBox.Show("User 'Interact' Deleted Successfully.", "Success", MessageBoxButtons.OK);
                }
                catch(Exception ex)
                {
                    string strMessage = "Exception=" + ex.Message;

                    if (ex.InnerException != null)
                    {
                        strMessage += "\n\n" + ex.InnerException.Message;
                    }

                    MessageBox.Show(strMessage, "Error", MessageBoxButtons.OK);
                }
            }
            else
            {

                try
                {
                    strQry.Clear();

                    strQry.AppendLine(" CREATE LOGIN [Interact] WITH PASSWORD = N'erawnacs', CHECK_EXPIRATION = OFF, CHECK_POLICY = OFF");

                    Execute_SQLCommand_CreateDB(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine(" EXEC master..sp_addsrvrolemember @loginame = N'Interact', @rolename = N'sysadmin'");

                    Execute_SQLCommand_CreateDB(strQry.ToString());

                    this.btnSqlUserAction.Text = "Delete";
                    this.txtInteractUser.Text = "Interact";

                    MessageBox.Show("User 'Interact' Created Successfully.", "Success", MessageBoxButtons.OK);
                }
                catch (Exception ex)
                {
                    string strMessage = "Exception=" + ex.Message;

                    if (ex.InnerException != null)
                    {
                        strMessage += "\n\n" + ex.InnerException.Message;
                    }

                    MessageBox.Show(strMessage, "Error", MessageBoxButtons.OK);
                }
            }
        }

        private void cboAuthentication_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txtUserName.Text = "";
            this.txtPassword.Text = "";
            this.txtDatabaseExists.Text = "";
            this.txtDatabaseFileName.Text = "";
            this.txtSystemUser.Text = "";
            this.txtInteractUser.Text = "";

            this.btnWinUserAction.Enabled = false;
            this.btnSqlUserAction.Enabled = false;
            this.btnDeleteDownloadFiles.Enabled = false;
            this.btnDelete.Enabled = false;
            
            if (this.cboAuthentication.SelectedIndex == 0)
            {
                this.txtUserName.Enabled = false;
                this.txtPassword.Enabled = false;
            }
            else
            {
                this.txtUserName.Enabled = true;
                this.txtPassword.Enabled = true;
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (this.cboServer.Text.Trim() == "")
            {
                MessageBox.Show("Enter Server.", "Error", MessageBoxButtons.OK,MessageBoxIcon.Error);

                this.cboServer.Focus();

                return;
            }

            if (this.cboAuthentication.SelectedIndex == 1)
            {
                if (this.txtUserName.Text.Trim() == "")
                {
                    MessageBox.Show("Enter User Name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    this.txtUserName.Focus();

                    return;
                }

                if (this.txtPassword.Text.Trim() == "")
                {
                    MessageBox.Show("Enter Password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    this.txtPassword.Focus();

                    return;
                }

            }
            
            this.Refresh();

            this.btnDeleteDownloadFiles.Enabled = false;
            this.btnDelete.Enabled = false;
            this.btnOK.Enabled = false;
            this.txtDatabaseExists.Text = "false";

            try
            {
                string strConnectionClientIntegratedSecurity = @"Server=#Engine#;Database=master; MultipleActiveResultSets=True;";
                
                pvtstrConnectionClientIntegratedSecurity = strConnectionClientIntegratedSecurity.Replace("#Engine#", this.cboServer.Text.Trim());
                
                if (this.cboAuthentication.SelectedIndex == 0)
                {
                    pvtstrConnectionClientIntegratedSecurity += "Integrated Security=true;";
                }
                else
                {
                    pvtstrConnectionClientIntegratedSecurity += "UID=" + this.txtUserName.Text.Trim() + ";PWD=" + this.txtPassword.Text.Trim() + ";";
                }
                
                DataSet DataSet = new DataSet();

                StringBuilder strQry = new StringBuilder();
                
                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" NAME AS DATABASE_NAME");
                strQry.AppendLine(",FILENAME");

                strQry.AppendLine(" FROM sysdatabases");
                strQry.AppendLine(" WHERE NAME = 'InteractPayrollClient'");

                Create_DataTable_Client(strQry.ToString(), DataSet, "DataBase");

                if (DataSet.Tables["DataBase"].Rows.Count == 0)
                {
                    this.txtDatabaseExists.Text = "False";
                    this.txtDatabaseFileName.Text = "";
                    this.btnOK.Enabled = true;
                    this.btnDrop.Enabled = false;
                }
                else
                {
                    this.txtDatabaseExists.Text = "True";
                    this.txtDatabaseFileName.Text = DataSet.Tables["DataBase"].Rows[0]["FILENAME"].ToString();

                    this.btnDeleteDownloadFiles.Enabled = true;
                    this.btnDelete.Enabled = true;
                    this.btnDrop.Enabled = true;
                }

           

                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" name ");

                    strQry.AppendLine(" FROM sys.server_principals ");

                    strQry.AppendLine(" WHERE name = '" + this.txtWindowsLogin.Text.Trim() + "'");

                    Create_DataTable_Client(strQry.ToString(), DataSet, "UserLoginExists");

                    if (DataSet.Tables["UserLoginExists"].Rows.Count == 0)
                    {
                        this.txtSystemUser.Text = "";

                        this.btnWinUserAction.Text = "Create";
                    }
                    else
                    {
                        this.txtSystemUser.Text = this.txtWindowsLogin.Text.Trim();

                        this.btnWinUserAction.Text = "Delete";
                    }

                    if (this.cboAuthentication.SelectedIndex == 1)
                    {
                        this.btnWinUserAction.Enabled = true;
                    }
              
                
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" name ");

                strQry.AppendLine(" FROM sys.server_principals ");

                strQry.AppendLine(" WHERE name = 'Interact' ");

                Create_DataTable_Client(strQry.ToString(), DataSet, "UserInteractExists");

                if (DataSet.Tables["UserInteractExists"].Rows.Count == 0)
                {
                    this.txtInteractUser.Text = "";

                    this.btnSqlUserAction.Text = "Create";

                    try
                    {
                        strQry.Clear();

                        strQry.AppendLine(" CREATE LOGIN [Interact] WITH PASSWORD = N'erawnacs', CHECK_EXPIRATION = OFF, CHECK_POLICY = OFF");

                        Execute_SQLCommand_Client(strQry.ToString());

                        strQry.Clear();

                        strQry.AppendLine(" EXEC master..sp_addsrvrolemember @loginame = N'Interact', @rolename = N'sysadmin'");

                        Execute_SQLCommand_Client(strQry.ToString());

                    }
                    catch
                    {
                        //In Case User does Not have Admin Rights 
                    }
                }
                else
                {
                    this.txtInteractUser.Text = "Interact";

                    this.btnSqlUserAction.Text = "Delete";
                }

                if (this.cboAuthentication.SelectedIndex == 0)
                {
                    this.btnSqlUserAction.Enabled = true;
                }

                DataSet.Tables.Remove(DataSet.Tables["UserInteractExists"]);
                
                MessageBox.Show("Connection to Engine Successful", "Connected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                string strMessage = "Exception=" + ex.Message;

                if (ex.InnerException != null)
                {
                    strMessage += "\n\n" + ex.InnerException.Message;
                }

                MessageBox.Show(strMessage, "Error", MessageBoxButtons.OK);
            }
        }

        private void btnWinUserAction_Click(object sender, EventArgs e)
        {
            StringBuilder strQry = new StringBuilder();

            if (this.btnWinUserAction.Text == "Delete")
            {
                try
                {
                    DataSet DataSet = new DataSet();

                    strQry.AppendLine("EXEC master..sp_dropsrvrolemember '" + this.txtWindowsLogin.Text.Trim() + "','sysadmin'");

                    Execute_SQLCommand_CreateDB(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine("SELECT name ");

                    strQry.AppendLine("FROM sys.databases ");

                    strQry.AppendLine("where suser_sname(owner_sid) = '" + this.txtWindowsLogin.Text.Trim() + "' ");

                    Create_DataTable_Client(strQry.ToString(), DataSet, "UserInteractOwnsDB");

                    for (int intRow = 0; intRow < DataSet.Tables["UserInteractOwnsDB"].Rows.Count; intRow++)
                    {
                        strQry.Clear();

                        strQry.AppendLine("ALTER AUTHORIZATION ON DATABASE::[" + DataSet.Tables["UserInteractOwnsDB"].Rows[intRow]["name"].ToString() + "] to sa ");

                        Execute_SQLCommand_CreateDB(strQry.ToString());
                    }

                    strQry.Clear();
                    strQry.AppendLine("DROP LOGIN [" + this.txtWindowsLogin.Text.Trim() + "] ");

                    Execute_SQLCommand_CreateDB(strQry.ToString());

                    this.btnWinUserAction.Text = "Create";
                    this.txtInteractUser.Text = "";

                    MessageBox.Show("User '" + this.txtWindowsLogin.Text.Trim() + "' Deleted Successfully.", "Success", MessageBoxButtons.OK);
                }
                catch (Exception ex)
                {
                    string strMessage = "Exception=" + ex.Message;

                    if (ex.InnerException != null)
                    {
                        strMessage += "\n\n" + ex.InnerException.Message;
                    }

                    MessageBox.Show(strMessage, "Error", MessageBoxButtons.OK);
                }
            }
            else
            {

                try
                {
                    strQry.Clear();

                    strQry.AppendLine(" CREATE LOGIN [" + this.txtWindowsLogin.Text.Trim() + "] FROM WINDOWS WITH DEFAULT_DATABASE = [master]");

                    Execute_SQLCommand_CreateDB(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine(" ALTER SERVER ROLE[sysadmin] ADD MEMBER[" + this.txtWindowsLogin.Text.Trim() + "]");

                    Execute_SQLCommand_CreateDB(strQry.ToString());

                    this.btnWinUserAction.Text = "Delete";
                    this.txtSystemUser.Text = this.txtWindowsLogin.Text.Trim();

                    MessageBox.Show("User '" + this.txtWindowsLogin.Text.Trim() + "' Created Successfully.", "Success", MessageBoxButtons.OK);
                }
                catch (Exception ex)
                {
                    string strMessage = "Exception=" + ex.Message;

                    if (ex.InnerException != null)
                    {
                        strMessage += "\n\n" + ex.InnerException.Message;
                    }

                    MessageBox.Show(strMessage, "Error", MessageBoxButtons.OK);
                }
            }
        }
    }
}
