using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using InteractPayrollClient;

namespace InteractPayroll
{
    public partial class frmTimeAttendanceMain : Form
    {
        clsISUtilities clsISUtilities;

        clsISClientUtilities clsISClientUtilities;

        private string pvtstrWebServerBeenUsed = "";

        private frmDateLoad frmDateLoad;
        private frmDateLoad frmNextDateLoad;

        private DataSet pvtDataSet;
        private DataView pvtMenuDataView;

        private string pvtstrPath;
        private System.Reflection.Assembly pvtAssembly;
        private object pvtlateBoundObj;
        private System.Windows.Forms.Form pvtForm;
        private System.Windows.Forms.Form pvtHelpForm;

        private DateTime pvtDateTimeLast = DateTime.Now;

        private bool _dragging = false;
        private Point _offset;
        private Point _start_point = new Point(0, 0);

        int intHeight = 32;

        public frmTimeAttendanceMain()
        {
            InitializeComponent();

            System.Drawing.Rectangle rect = Screen.GetWorkingArea(this);
            this.MaximizedBounds = Screen.GetWorkingArea(this);
            this.WindowState = FormWindowState.Maximized;

            //Reposition Image And Menus
            int intControlsWidth = (this.MainToolStrip.Location.X + this.MainToolStrip.Width) - this.picValiditeImage.Location.X + 56;
            int intExtraMove = (this.MaximizedBounds.Width - intControlsWidth) / 2;

            this.picValiditeImage.Left = this.picValiditeImage.Left + intExtraMove;
            this.MainMenuStrip.Left = this.MainMenuStrip.Left + intExtraMove;
            this.MainToolStrip.Left = this.MainToolStrip.Left + intExtraMove;

            MainMenuStrip.Renderer = new CustomMenuStriplRenderer();
            MainToolStrip.Renderer = new MyToolStripRenderer();
        }

        private void frmTimeAttendanceInternetMain_Load(object sender, System.EventArgs e)
        {
            try
            {
                //Get Data From Logon
                pvtDataSet = (DataSet)AppDomain.CurrentDomain.GetData("DataSet");

                this.Refresh();

                clsISClientUtilities = new clsISClientUtilities(this, "busTimeAttendanceLogon");
                clsISUtilities = new clsISUtilities(this, "busTimeAttendanceInternetMain");

                Load_Companies();

                //Used in Data Upload Program (Button Click)
                AppDomain.CurrentDomain.SetData("TimerMenuClick", this.tmrMenuClick);

                //Pass To User Menu Access Form
                AppDomain.CurrentDomain.SetData("MainMenuStrip", this.MainMenuStrip);

                AppDomain.CurrentDomain.SetData("Globe", pnlGlobe);

                //Used To Close CurrentForm when Error (ErrorHandler) 
                AppDomain.CurrentDomain.SetData("TimerCloseCurrentForm", this.TimerCloseCurrentForm);

                //Used To Reload Companies in PayrollMain Company ComboBox 
                AppDomain.CurrentDomain.SetData("TimerReloadCompanies", this.TimerReloadCompanies);

                if (AppDomain.CurrentDomain.GetData("AccessInd").ToString() == "S")
                {
                    string strReturnArrayFlgs = (string)clsISUtilities.DynamicFunction("Check_Date_Loads", null);

                    string[] strReturnCurrentFlags = strReturnArrayFlgs.Split('|');

                    this.backupDatabaseToolStripMenuItem.Visible = true;
                    this.restoreDatabaseToolStripMenuItem.Visible = true;

                    this.toolStripSeparator2.Visible = true;

                    //Check Here
                    if (strReturnCurrentFlags[0] == "Y")
                    {
                        frmDateLoad = new frmDateLoad("C");
                        frmDateLoad.ShowDialog();
                    }

                    if (strReturnCurrentFlags[1] == "Y")
                    {
                        frmNextDateLoad = new frmDateLoad("N");
                        frmNextDateLoad.lblDescription.Text = "Loading Dates for the next Financial Year in progress.\nPlease be patient.";
                        frmNextDateLoad.ShowDialog();
                    }
#if(DEBUG)
#else
                    //Check If Paid Holiday has been LOaded
                    if ( strReturnCurrentFlags[2] == "Y")
                    {
                        MessageBox.Show("Remember to Load Public Holidays");
                    }
#endif
                }
                else
                {
                    //Errol Added Temporarily
                    this.backupDatabaseToolStripMenuItem.Visible = false;
                    this.restoreDatabaseToolStripMenuItem.Visible = false;

                    this.paymentOptionToolStripMenuItem.Visible = false;
                    toolStripSeparator15.Visible = false;

                    //ERROL TO FIX
                    this.toolStripSeparator2.Visible = false;
                }

                Build_Menu_And_ToolBar_Events();

                this.MainMenuStrip.Enabled = true;
                this.MainToolStrip.Enabled = true;

                //Reset Cursor
                this.Cursor = Cursors.Default;
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        public void Load_Companies()
        {
            int intCompanySelectedIndex = 0;

            this.companytoolStripComboBox.Items.Clear();
            this.companytoolStripComboBox.BeginUpdate();

            //Load Company ComboBox
            for (int intRow = 0; intRow < pvtDataSet.Tables["Company"].Rows.Count; intRow++)
            {
                if (AppDomain.CurrentDomain.GetData("AccessInd").ToString() != "S")
                {
                    if (pvtDataSet.Tables["Company"].Rows[intRow]["LOCK_IND"].ToString() == "Y")
                    {
                        continue;
                    }
                }

                if (Convert.ToInt64(AppDomain.CurrentDomain.GetData("LastCompanyNo")) == Convert.ToInt64(pvtDataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"]))
                {
                    intCompanySelectedIndex = intRow;
                }

                this.companytoolStripComboBox.Items.Add(pvtDataSet.Tables["Company"].Rows[intRow]["COMPANY_DESC"].ToString());
            }

            this.companytoolStripComboBox.EndUpdate();

            if (companytoolStripComboBox.Items.Count > 0)
            {
                companytoolStripComboBox.SelectedIndex = intCompanySelectedIndex;
            }
            else
            {
                this.Close();

                ////Disable All Menu Options Except Company
                //AppDomain.CurrentDomain.SetData("CompanyNo", "-1");
            }

            this.MainToolStrip.Refresh();
        }

        public void Menu_Click(ToolStripMenuItem mniMenuItem)
        {
            try
            {
                //NB File Name And Menu Id (tag) Must be Inserted into Table MENU_FILE_NAME for User File Download
                //NB File Name And Menu Id (tag) Must be Inserted into Table MENU_FILE_NAME for User File Download
                //NB File Name And Menu Id (tag) Must be Inserted into Table MENU_FILE_NAME for User File Download
                //NB File Name And Menu Id (tag) Must be Inserted into Table MENU_FILE_NAME for User File Download

                if (pvtDateTimeLast > DateTime.Now.AddSeconds(-1))
                {
                    return;
                }
                else
                {
                    pvtDateTimeLast = DateTime.Now;
                }

                if (mniMenuItem.DropDownItems.Count == 0)
                {
                    //Set For User Linked To MenuItem
                    AppDomain.CurrentDomain.SetData("MenuId", mniMenuItem.Tag);
#if(DEBUG)
                    if (mniMenuItem.Tag == null)
                    {
                        MessageBox.Show(mniMenuItem.Name + " Tag is NULL");
                    }
#endif
                    switch (mniMenuItem.Name)
                    {

                        case "fingerprintSetuptoolStripMenuItem":

                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "ChooseFingerprintReader.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmChooseFingerprintReader");

                            break;


                        case "rosterToolStripMenuItem":

                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "Roster.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmRoster");

                            break;

                        case "rosterDateToolStripMenuItem":

                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "ShiftSchedule.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmShiftSchedule");

                            break;

                        case "paymentOptionToolStripMenuItem":

                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "CompanyPaymentOptions.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmCompanyPaymentOptions");

                            break;

                        case "helpSystemToolStripMenuItem":

                            return;

                        case "fileDownloadToolStripMenuItem":

                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "FileDownload.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmFileDownload");

                            break;

                        case "fileUploadToolStripMenuItem":

                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "FileUpload.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmFileUpload");

                            break;

                        case "rptTimesheetTotalToolStripMenuItem":

                            //2013-04-11
                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "RptTimeSheetTotals.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmRptTimeSheetTotalsSelection");

                            break;

                        case "localWebServerToolStripMenuItem":

                            //2013-04-11
                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "SetupLocalWebServer.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmSetupLocalWebServer");

                            break;

                        case "resetEmployeeTakeOnToolStripMenuItem":

                            //pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "ResetEmployeeTakeOn.dll";
                            //pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            //pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmResetEmployeeTakeOn");

                            break;

                        case "leaveCalculatedTimeSheetToolStripMenuItem":

                            //pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "RptLeaveTimeSheet.dll";
                            //pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            //pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmRptLeaveTimeSheet");

                            break;

                        case "fixtoolStripMenuItem":

                            //pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "Fix.dll";
                            //pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            //pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmFix");

                            break;

                        case "exitToolStripMenuItem":

                            this.Close();
                            return;

                        case "newToolStripMenuItem":
                        case "updateToolStripMenuItem":
                        case "deleteToolStripMenuItem":
                        case "saveToolStripMenuItem":
                        case "cancelToolStripMenuItem":

                            try
                            {
                                //NB Theses Procedures Need to Be Set To Public
                                System.EventArgs e = new EventArgs();
                                ActiveMdiChild.GetType().InvokeMember("btn" + mniMenuItem.Text + "_Click", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, this.ActiveMdiChild, new object[] { null, e });
                            }
                            catch
                            {
                                MessageBox.Show("'" + ActiveMdiChild.GetType().ToString() + ".btn" + mniMenuItem.Text + "_Click' Needs to be Set to Public.\n\nSpeak to System Administrator.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            return;

                        case "backupDatabaseToolStripMenuItem":

                            //2013-04-11
                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "BackupRestoreDatabase.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmBackupDatabase");

                            break;

                        case "restoreDatabaseToolStripMenuItem":

                            //2013-04-11
                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "BackupRestoreDatabase.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmRestoreDatabase");

                            break;

                        case "departmentToolStripMenuItem":

                            //2013-04-11
                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "OccupationDepartment.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmOccupationDepartment");

                            break;

                        case "occupationToolStripMenuItem":

                            //2013-04-11
                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "OccupationDepartment.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmOccupationDepartment");

                            break;

                        case "employeeDepartmentLinkToolStripMenuItem":

                            //2013-04-11
                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "OccupationDepartmentLink.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmOccupationDepartmentLink");

                            break;

                        case "employeeOccupationLinkToolStripMenuItem":

                            //2013-04-11
                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "OccupationDepartmentLink.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmOccupationDepartmentLink");

                            break;

                        case "calenderToolStripMenuItem":

                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "Calender.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmCalender");

                            break;

                        case "dataDownloadToolStripMenuItem":
                        case "dataUploadTimesheetsToolStripMenuItem":

                            //Move Down Under File Download
                            object[] obj = null;

                            pvtstrWebServerBeenUsed = "L";

                            byte[] bytCompress = (byte[])clsISClientUtilities.DynamicFunction("Check_For_Server_File_Changes", obj, false);
                            DataSet DataSetClient = clsISClientUtilities.DeCompress_Array_To_DataSet(bytCompress);

                            if (DataSetClient.Tables["ReturnValues"].Rows[0]["REBOOT_IND"].ToString() == "Y")
                            {
                                //Restart Service
                                frmRestartService frmRestartService = new frmRestartService(DataSetClient.Tables["ReturnValues"].Rows[0]["MACHINE_NAME"].ToString(), DataSetClient.Tables["ReturnValues"].Rows[0]["MACHINE_IP"].ToString());

                                frmRestartService.Show();

                                try
                                {
                                    clsRestartFingerPrintClockTimeAttendanceService clsRestartFingerPrintClockTimeAttendanceService;

                                    if (AppDomain.CurrentDomain.GetData("URLClientPath").ToString() != "")
                                    {
                                        //Calls Web Service Internally
                                        clsRestartFingerPrintClockTimeAttendanceService = new clsRestartFingerPrintClockTimeAttendanceService("");

                                        InteractPayrollClient.Restart Restart = (InteractPayrollClient.Restart)clsRestartFingerPrintClockTimeAttendanceService.DynamicFunction("RestartFingerPrintClockTimeAttendanceService", null);

                                        if (Restart.OK == "Y")
                                        {
                                        }
                                        else
                                        {
                                            MessageBox.Show("Failed to Restart 'FingerPrintClockTimeAttendanceService' Service.\n\nSpeak to System Administrator.\n\nReboot of Machine '" + DataSetClient.Tables["ReturnValues"].Rows[0]["MACHINE_NAME"].ToString() + "' (IP = " + DataSetClient.Tables["ReturnValues"].Rows[0]["MACHINE_IP"].ToString() + ")\n will Allow you to Continue.",
                                            "Program Changes",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Exclamation);

                                            frmRestartService.Close();

                                            this.Close();
                                        }
                                    }
                                    else
                                    {
                                        clsRestartFingerPrintClockTimeAttendanceService = new clsRestartFingerPrintClockTimeAttendanceService("FingerPrintClockServiceStartStop");

                                        object Restart = clsRestartFingerPrintClockTimeAttendanceService.DynamicFunction("RestartFingerPrintClockTimeAttendanceService", null);

                                        string strRestart = Restart.ToString();

                                        if (strRestart.IndexOf("FingerPrintClockServer.Restart") > -1)
                                        {
                                            //Not Really True
                                        }
                                        else
                                        {
                                            MessageBox.Show("Failed to Restart 'FingerPrintClockTimeAttendanceService' Service.\n\nSpeak to System Administrator.\n\nReboot of Machine '" + pvtDataSet.Tables["ReturnValues"].Rows[0]["MACHINE_NAME"].ToString() + "' (IP = " + pvtDataSet.Tables["ReturnValues"].Rows[0]["MACHINE_IP"].ToString() + ")\n will Allow you to Continue.",
                                            "Program Changes",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Exclamation);
                                        }
                                    }
                                }
                                catch
                                {
                                    MessageBox.Show("Failed to Restart 'FingerPrintClockTimeAttendanceService' Service.\n\nSpeak to System Administrator.\n\nReboot of Machine '" + DataSetClient.Tables["ReturnValues"].Rows[0]["MACHINE_NAME"].ToString() + "' (IP = " + DataSetClient.Tables["ReturnValues"].Rows[0]["MACHINE_IP"].ToString() + ")\n will Allow you to Continue.",
                                    "Program Changes",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation);

                                    frmRestartService.Close();

                                    this.Close();
                                }

                                frmRestartService.Close();
                            }

                            if (mniMenuItem.Name == "dataDownloadToolStripMenuItem")
                            {
                                //2013-04-11
                                pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "DataDownload.dll";
                                pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                                pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmDataDownload");
                            }
                            else
                            {
                                pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "DataUpload.dll";
                                pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                                pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmDataUpload");
                            }

                            break;

                        case "companyToolStripMenuItem":

                            //2013-04-11
                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "TimeAttendanceCompany.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmTimeAttendanceCompany");

                            break;

                        case "userToolStripMenuItem":

                            //2013-04-11
                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "User.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmUser");

                            break;

                        case "userMenuAccessToolStripMenuItem":

                            //2013-04-11
                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "UserMenuAccess.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmUserMenuAccess");

                            break;

                        case "userEmployeeLinkToolStripMenuItem":

                            //2013-04-11
                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "UserEmployeeLink.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmUserEmployeeLink");

                            break;

                        case "costCentreToolStripMenuItem":

                            //2013-04-11
                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "CostCentre.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmCostCentre");

                            break;

                        case "publicHolidayToolStripMenuItem":

                            //2013-04-11
                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "PublicHoliday.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmPublicHoliday");

                            break;

                        case "employeeToolStripMenuItem":

                            //2013-04-11
                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "TimeAttendanceEmployee.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmTimeAttendanceEmployee");

                            break;

                        case "timesheetToolStripMenuItem":

                            //2013-04-12
                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "Timesheet.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmTimeSheet");

                            break;

                        //2013-04-12
                        case "timesheetBatchToolStripMenuItem":

                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "TimeSheetBatch.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmTimeSheetBatch");

                            break;

                        case "timesheetAuthorisationToolStripMenuItem":

                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "TimeSheetAuthorise.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmTimeSheetAuthorise");

                            break;


                        case "openPayrollRunToolStripMenuItem":

                            //2013-04-12
                            AppDomain.CurrentDomain.SetData("RunType", "P");

                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "OpenPayrollRun.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmOpenPayrollRun");

                            break;

                        case "runTimeAndAttendanceToolStripMenuItem":
                            
                            obj = new object[2];
                            obj[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                            obj[1] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();

                            string strBusyWithRun = (string)clsISUtilities.DynamicFunction("Check_If_Busy_With_Run", obj, false);

                            if (strBusyWithRun == "Y")
                            {
                                pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "TimeAttendanceRun.dll";
                                pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                                pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmTimeAttendanceRun");
                            }
                            else
                            {
                                frmNotBusyWithRun frmNotBusyWithRun = new frmNotBusyWithRun();
                                frmNotBusyWithRun.MdiParent = this;

                                frmNotBusyWithRun.lblHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblHeader_MouseDown);
                                frmNotBusyWithRun.lblHeader.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblHeader_MouseMove);
                                frmNotBusyWithRun.lblHeader.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblHeader_MouseUp);

                                frmNotBusyWithRun.Show();
                                return;
                            }

                            break;

                        case "timesheetTotalsToolStripMenuItem":
                            
                            obj = new object[2];
                            obj[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                            obj[1] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();

                            string strTimeAttendanceHasBeenRun = (string)clsISUtilities.DynamicFunction("Check_If_TimeAtendance_Has_Been_Run", obj, false);

                            if (strTimeAttendanceHasBeenRun == "Y")
                            {
                                pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "TimeAttendanceAnalysis.dll";
                                pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                                pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmTimeAttendanceAnalysis");
                            }
                            else
                            {
                                frmNotBusyWithRun frmNotBusyWithRun = new frmNotBusyWithRun();
                                frmNotBusyWithRun.lblMessage.Text = "Run Time and Attendance 'Run' has Not been Processed.";
                                frmNotBusyWithRun.MdiParent = this;

                                frmNotBusyWithRun.lblHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblHeader_MouseDown);
                                frmNotBusyWithRun.lblHeader.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblHeader_MouseMove);
                                frmNotBusyWithRun.lblHeader.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblHeader_MouseUp);

                                frmNotBusyWithRun.Show();
                                return;
                            }
                            
                            break;

                        case "employeeBalanceTakeOnToolStripMenuItem":

                            //pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "EmployeeBalanceTakeOn.dll";
                            //pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            //pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmEmployeeBalanceTakeOn");

                            break;

                        case "employeeActivationToolStripMenuItem":

                            //pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "EmployeeActivate.dll";
                            //pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            //pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmEmployeeActivate");

                            break;

                        case "closePayrollRunToolStripMenuItem":

                            obj = new object[2];
                            obj[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                            obj[1] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();

                            string strRunCompleted = (string)clsISUtilities.DynamicFunction("Check_If_Run_Completed", obj, false);

                            if (strRunCompleted == "Y")
                            {
                                pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "ClosePayrollRun.dll";
                                pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                                pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmClosePayrollRun");
                            }
                            else
                            {
                                frmNotBusyWithRun frmNotBusyWithRun = new frmNotBusyWithRun();

                                if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
                                {
                                    frmNotBusyWithRun.lblMessage.Text = "Run Time and Attendance 'Run' has Not been Processed.";
                                }
                                else
                                {
                                    frmNotBusyWithRun.lblMessage.Text = "Open Payroll Run (Salaries) or Run Time and Attendance 'Run' has Not been Processed.";
                                }

                                frmNotBusyWithRun.MdiParent = this;

                                frmNotBusyWithRun.lblHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblHeader_MouseDown);
                                frmNotBusyWithRun.lblHeader.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblHeader_MouseMove);
                                frmNotBusyWithRun.lblHeader.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblHeader_MouseUp);

                                frmNotBusyWithRun.Show();
                                return;
                            }

                            break;

                        case "rptTimesheetToolStripMenuItem":

                            //2013-04-24
                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "RptTimeSheet.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmRptTimeSheetSelection");

                            break;

                        case "employeeActivateSelectionToolStripMenuItem":

                            AppDomain.CurrentDomain.SetData("RunType", "T");

                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "EmployeeActivateSelection.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmEmployeeActivateSelection");

                            break;

                        case "changePasswordToolStripMenuItem":

                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "PasswordChange.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmPasswordChange");

                            break;

                        case "resetPasswordToolStripMenuItem":

                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "PasswordReset.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmPasswordReset");

                            break;

                        default:

                            //Window - Dynamic Add/Delete of Forms
                            foreach (Form form in this.MdiChildren)
                            {
                                if (mniMenuItem.Tag == form.Tag)
                                {
                                    if (this.ActiveMdiChild == form)
                                    {
                                    }
                                    else
                                    {
                                        form.Activate();
                                    }

                                    break;
                                }
                            }
                            
                            return;
                    }

                    if (mniMenuItem.Name == "helpSystemToolStripMenuItem")
                    {
                        if (pvtHelpForm == null)
                        {
                            pvtstrPath = AppDomain.CurrentDomain.BaseDirectory + "PayrollHelp.dll";
                            pvtAssembly = System.Reflection.Assembly.LoadFile(pvtstrPath);
                            pvtlateBoundObj = pvtAssembly.CreateInstance("InteractPayroll.frmHelp");

                            pvtHelpForm = (System.Windows.Forms.Form)pvtlateBoundObj;

                            pvtHelpForm.MaximizeBox = false;
                            pvtHelpForm.StartPosition = FormStartPosition.CenterScreen;
                            //Store MenuItem Tag
                            pvtHelpForm.Tag = mniMenuItem.Tag;

                            pvtHelpForm.MdiParent = this;

                            pvtHelpForm.Show();

                        }
                        else
                        {
                            pvtHelpForm.Show();
                        }

                        return;
                    }
                    else
                    {
                        if (mniMenuItem.Name == "helpAboutToolStripMenuItem")
                        {
                            //					if (frmHelp == null)
                            //					{
                            //						frmHelp = new frmHelp();
                            //	
                            //						frmHelp.MdiParent = this;
                            //						frmHelp.Show();
                            //					}
                            //					else
                            //					{
                            //						frmHelp.Show();
                            //					}
                        }
                        else
                        {
                            pvtForm = (System.Windows.Forms.Form)pvtlateBoundObj;
                            pvtForm.MdiParent = this;

                            //ELR-2018-08-18
                            if (pvtForm.FormBorderStyle == FormBorderStyle.None)
                            {
                                if (pvtForm.Name != "frmPasswordChange")
                                {
                                    pvtForm.Height += intHeight;
                                }
                            }
                            else
                            {
                                pvtForm.Icon = this.Icon;
                                pvtForm.MaximizeBox = false;
                            }
                            
                            //Set Edit Button EnableChanged Event so That we can control The MenuItems / ToolBarItems
                            foreach (Control myControl in pvtForm.Controls)
                            {
                                //ELR-2018-08-18
                                if (pvtForm.FormBorderStyle == FormBorderStyle.None
                                && pvtForm.Name != "frmPasswordChange")
                                {
                                    myControl.Top += intHeight;
                                }

                                if (myControl is Button)
                                {
                                    Button button = (Button)myControl;

#if (DEBUG)
                                    if (button.FlatStyle != FlatStyle.Flat)
                                    {
                                        MessageBox.Show(button.Name + " Not FlatStyle");
                                    }
#endif
                                    button.FlatStyle = FlatStyle.Flat;

                                    if (button.Text.ToUpper().IndexOf("CLOSE") > -1)
                                    {
                                        button.FlatAppearance.MouseOverBackColor = Color.Red;
                                    }

                                    switch (myControl.Text.Replace("&", ""))
                                    {
                                        case "New":
                                        case "Update":
                                        case "Delete":
                                        case "Save":
                                        case "Cancel":

                                            myControl.EnabledChanged += new System.EventHandler(this.MdiChild_Edit_Button_EnabledChanged);

                                            break;
                                    }
                                }
                                else
                                {
                                    if (pvtForm.Name == "frmCompanyPaymentOptions")
                                    {
                                        if (myControl is TabControl
                                        && myControl.Name == "tabControlMain")
                                        {
                                            foreach (Control myControl1 in myControl.Controls)
                                            {
                                                if (myControl1 is TabPage
                                                && myControl1.Name == "tabPageSelection")
                                                {
                                                    foreach (Control myControl2 in myControl1.Controls)
                                                    {
                                                        if (myControl2 is Button)
                                                        {
                                                            Button button = (Button)myControl2;
                                                            button.FlatStyle = FlatStyle.Flat;

                                                            if (button.Text.ToUpper().IndexOf("CLOSE") > -1)
                                                            {
                                                                button.FlatAppearance.MouseOverBackColor = Color.Red;
                                                            }

                                                            switch (myControl2.Text.Replace("&", ""))
                                                            {
                                                                case "New":
                                                                case "Update":
                                                                case "Delete":
                                                                case "Save":
                                                                case "Cancel":

                                                                    myControl2.EnabledChanged += new System.EventHandler(this.MdiChild_Edit_Button_EnabledChanged);

                                                                    break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            //ELR-2018-08-18
                            if (pvtForm.FormBorderStyle == FormBorderStyle.None
                            &&  pvtForm.Name != "frmPasswordChange")
                            {
                                Label lblHeader = new Label();
                                lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                                lblHeader.BackColor = System.Drawing.Color.DimGray;
                                lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                                lblHeader.ForeColor = System.Drawing.Color.Black;
                                lblHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                                lblHeader.Location = new System.Drawing.Point(1, 1);
                                lblHeader.Name = "lblHeader";
                                lblHeader.Size = new System.Drawing.Size(pvtForm.Width - (2 * intHeight), intHeight - 2);
                                lblHeader.Text = mniMenuItem.Text;
                                lblHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblHeader_MouseDown);
                                lblHeader.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblHeader_MouseMove);
                                lblHeader.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblHeader_MouseUp);

                                pvtForm.Text = lblHeader.Text;
                                
                                pvtForm.Controls.Add(lblHeader);

                                Button btnHeaderClose = new Button();
                                btnHeaderClose.BackColor = System.Drawing.Color.DimGray;
                                btnHeaderClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
                                btnHeaderClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                                btnHeaderClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                                btnHeaderClose.ForeColor = System.Drawing.Color.Black;
                                btnHeaderClose.Location = new System.Drawing.Point(pvtForm.Width - intHeight, 0);
                                btnHeaderClose.Name = "btnHeaderClose";
                                btnHeaderClose.Size = new System.Drawing.Size(intHeight, intHeight);
                                btnHeaderClose.TabStop = false;
                                btnHeaderClose.Text = "X";
                                btnHeaderClose.Click += new System.EventHandler(MdiChild_ButtonClose_Click);

                                pvtForm.Controls.Add(btnHeaderClose);

                                Button btnHeaderMinimize = new Button();
                                btnHeaderMinimize.BackColor = System.Drawing.Color.DimGray;
                                btnHeaderMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                                btnHeaderMinimize.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                                btnHeaderMinimize.ForeColor = System.Drawing.Color.Black;
                                btnHeaderMinimize.Location = new System.Drawing.Point(pvtForm.Width - (2 * intHeight) + 1, 0);
                                btnHeaderMinimize.Name = "btnHeaderMinimize";
                                btnHeaderMinimize.Size = new System.Drawing.Size(intHeight, intHeight);
                                btnHeaderMinimize.TabStop = false;
                                btnHeaderMinimize.Text = "_";
                                btnHeaderMinimize.Click += new System.EventHandler(MdiChild_ButtonMinimize_Click);

                                pvtForm.Controls.Add(btnHeaderMinimize);
                            }
                        }

                        //ELR-2018-08-18
                        if (pvtForm.FormBorderStyle == FormBorderStyle.None)
                        {
                            pvtForm.Paint += new System.Windows.Forms.PaintEventHandler(MdiChild_Paint);
                        }
                        else
                        {
                            pvtForm.Icon = this.Icon;
                            pvtForm.MaximizeBox = false;
                        }

                        System.Drawing.Rectangle rect = Screen.GetWorkingArea(this);

                        int intX = (rect.Width - pvtForm.Width) / 2;
                        int intY = (rect.Height - pvtForm.Height + this.pnlHeader.Height) / 2;

                        if (AppDomain.CurrentDomain.GetData("FormSmallest") != null)
                        {
                            //Errol Use For Screenshots Program
                            intY = this.pnlHeader.Height + 10;
                        }

                        pvtForm.StartPosition = FormStartPosition.Manual;
                        pvtForm.Location = new Point(intX, intY);
                      
                        //Store MenuItem Tag
                        pvtForm.Tag = mniMenuItem.Tag;
                        //Hook Up so That Menu Item Can br Enabled On Close
                        pvtForm.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MdiChild_FormClosing);
                        pvtForm.TextChanged += new System.EventHandler(MdiChild_TextChanged);
                        pvtForm.Leave += new System.EventHandler(MdiChild_Leave);
                        
                        ToolStripMenuItem newToolStripMenuItem = new ToolStripMenuItem(pvtForm.Text);
                        newToolStripMenuItem.Tag = mniMenuItem.Tag;
                       
                        windowToolStripMenuItem.DropDownItems.Add(newToolStripMenuItem);
                        newToolStripMenuItem.Click += new System.EventHandler(this.MenuItem_Click);
                        
                        //Set Form Title from Menu Text 
                        switch (mniMenuItem.Name)
                        {
                            case "changePasswordToolStripMenuItem":
                            case "resetPasswordToolStripMenuItem":

                                pvtForm.Text = "Password " + mniMenuItem.Text;
                                break;

                            default:

                                pvtForm.Text = mniMenuItem.Text;
                                break;
                        }

                        pvtForm.Show();
                    }

                    mniMenuItem.Enabled = false;
                }
            }
            catch (Exception eException)
            {
                if (pvtstrWebServerBeenUsed == "W")
                {
                    clsISUtilities.ErrorHandler(eException);
                }
                else
                {
                    this.clsISClientUtilities.ErrorHandler(eException);
                }

                this.Close();
            }
        }

        private void lblHeader_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;
            _start_point = new Point(e.X, e.Y);
        }

        private void lblHeader_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void lblHeader_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Form myForm = (Form)((Label)sender).Parent;
                Label lblHeader = (Label)sender;

                //Cursor Position relative to lblHeader (On Screen)
                Point p = PointToScreen(new Point(Cursor.Position.X - PointToScreen(lblHeader.Location).X, Cursor.Position.Y - PointToScreen(lblHeader.Location).Y));

                myForm.Location = new Point(p.X - this._start_point.X, p.Y - this._start_point.Y);
            }
        }

        private void MdiChild_Edit_Button_EnabledChanged(object sender, EventArgs e)
        {
            Button myButton = (Button)sender;

            switch (myButton.Text.Replace("&", ""))
            {
                case "New":

                    this.newToolStripMenuItem.Enabled = myButton.Enabled;
                    Set_Corresponding_ToolBar_Button(newToolStripMenuItem);

                    break;

                case "Update":

                    this.updateToolStripMenuItem.Enabled = myButton.Enabled;
                    Set_Corresponding_ToolBar_Button(updateToolStripMenuItem);

                    break;

                case "Delete":

                    this.deleteToolStripMenuItem.Enabled = myButton.Enabled;
                    Set_Corresponding_ToolBar_Button(deleteToolStripMenuItem);

                    break;

                case "Save":

                    this.saveToolStripMenuItem.Enabled = myButton.Enabled;
                    Set_Corresponding_ToolBar_Button(saveToolStripMenuItem);

                    break;

                case "Cancel":

                    this.cancelToolStripMenuItem.Enabled = myButton.Enabled;
                    Set_Corresponding_ToolBar_Button(cancelToolStripMenuItem);

                    break;
            }
        }

        private void companytoolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (companytoolStripComboBox.SelectedIndex > -1)
            {
                //Reset - Upload Is Dynamic
                dataUploadTimesheetsToolStripMenuItem.Enabled = true;
                toolStripUpload.Enabled = true;

                Disable_All_Edit_MenuItems_ToolBarItems();

                AppDomain.CurrentDomain.SetData("CompanyNo", Convert.ToInt64(pvtDataSet.Tables["Company"].Rows[companytoolStripComboBox.SelectedIndex]["COMPANY_NO"]));
                AppDomain.CurrentDomain.SetData("CompanyDesc", pvtDataSet.Tables["Company"].Rows[companytoolStripComboBox.SelectedIndex]["COMPANY_DESC"].ToString());
                AppDomain.CurrentDomain.SetData("DateFormat", pvtDataSet.Tables["Company"].Rows[companytoolStripComboBox.SelectedIndex]["DATE_FORMAT"].ToString());
                AppDomain.CurrentDomain.SetData("DateFormat", pvtDataSet.Tables["Company"].Rows[companytoolStripComboBox.SelectedIndex]["DATE_FORMAT"].ToString());
                AppDomain.CurrentDomain.SetData("TimeSheetReadTimeoutSeconds", pvtDataSet.Tables["Company"].Rows[companytoolStripComboBox.SelectedIndex]["TIMESHEET_READ_TIMEOUT_SECONDS"].ToString());

                if (Convert.ToInt64(AppDomain.CurrentDomain.GetData("LastCompanyNo")) != Convert.ToInt64(pvtDataSet.Tables["Company"].Rows[companytoolStripComboBox.SelectedIndex]["COMPANY_NO"]))
                {
                    object[] objParm = new object[2];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[1] = Convert.ToInt64(pvtDataSet.Tables["Company"].Rows[companytoolStripComboBox.SelectedIndex]["COMPANY_NO"]);

                    clsISUtilities.DynamicFunction("Update_User_Last_Company_No", objParm);

                    AppDomain.CurrentDomain.SetData("LastCompanyNo", Convert.ToInt64(pvtDataSet.Tables["Company"].Rows[companytoolStripComboBox.SelectedIndex]["COMPANY_NO"]));
                }

                //Close Down All Open Forms
                foreach (Form MdiChildForm in this.MdiChildren)
                {
                    if (MdiChildForm.Tag.ToString() == "companyToolStripMenuItem"
                        | MdiChildForm.Tag.ToString() == "userToolStripMenuItem"
                        | MdiChildForm.Tag.ToString() == "userMenuAccessToolStripMenuItem"
                        | MdiChildForm.Tag.ToString() == "calenderToolStripMenuItem"
                        | MdiChildForm.Tag.ToString() == "calculatorToolStripMenuItem"
                        | MdiChildForm.Tag.ToString() == "helpSystemToolStripMenuItem"
                        | MdiChildForm.Tag.ToString() == "helpAboutToolStripMenuItem"
                        | MdiChildForm.Tag.ToString() == "changePasswordToolStripMenuItem"
                        | MdiChildForm.Tag.ToString() == "resetPasswordToolStripMenuItem")
                    {
                        continue;
                    }

                    //Disable Save Button so That Question is not Asked To Save Form In Edit Mode
                    foreach (Control myControl in this.ActiveMdiChild.Controls)
                    {
                        if (myControl.GetType().ToString() == "System.Windows.Forms.Button")
                        {
                            if (myControl.Text.Replace("&", "") == "Save")
                            {
                                if (myControl.Enabled == true)
                                {
                                    myControl.Enabled = false;
                                }
                                break;
                            }
                        }
                    }

                    MdiChildForm.Close();
                }

                //Reset Menu Items if Not System Administrator
                if (AppDomain.CurrentDomain.GetData("AccessInd").ToString() != "S")
                {
                    bool blnPrevVisibleIsSeperator = false;
                    ToolStripSeparator tsToolStripSeparatorSaved = null;

                    //Set Menus InVisible Where Not Part Of User's Profile 
                    foreach (ToolStripMenuItem tsMenuItem in MainMenuStrip.Items)
                    {
                        if (tsMenuItem.GetType().Name == "ToolStripMenuItem")
                        {
                            if (tsMenuItem.Text == "Edit"
                                | tsMenuItem.Text == "Tools"
                                | tsMenuItem.Text == "Window"
                                | tsMenuItem.Text == "Help")
                            {
                                continue;
                            }
                            else
                            {
                                if (pvtDataSet.Tables["Company"].Rows[this.companytoolStripComboBox.SelectedIndex]["COMPANY_ACCESS_IND"].ToString() != "A")
                                {
                                    pvtMenuDataView = null;
                                    pvtMenuDataView = new DataView(pvtDataSet.Tables["Menus"], "COMPANY_NO = " + pvtDataSet.Tables["Company"].Rows[this.companytoolStripComboBox.SelectedIndex]["COMPANY_NO"].ToString() + " AND SUBSTRING(MENU_ITEM_ID,1,1) IN ('" + tsMenuItem.Tag.ToString() + "')", "", DataViewRowState.CurrentRows);

                                    if (pvtMenuDataView.Count == 0)
                                    {
                                        if (tsMenuItem.Text == "Password"
                                            | tsMenuItem.Text == "File")
                                        {
                                            tsMenuItem.Visible = true;
                                        }
                                        else
                                        {
                                            tsMenuItem.Visible = false;
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        tsMenuItem.Visible = true;
                                    }
                                }
                                else
                                {
                                    tsMenuItem.Visible = true;
                                }
                            }

                            blnPrevVisibleIsSeperator = true;

                            for (int intItem1Count = 0; intItem1Count < tsMenuItem.DropDownItems.Count; intItem1Count++)
                            {
                                if (tsMenuItem.DropDownItems[intItem1Count].GetType().Name == "ToolStripMenuItem")
                                {
                                    ToolStripMenuItem tsToolStripMenuItem1 = (ToolStripMenuItem)tsMenuItem.DropDownItems[intItem1Count];

                                    if (tsToolStripMenuItem1.Name == "changePasswordToolStripMenuItem"
                                        | tsToolStripMenuItem1.Name == "exitToolStripMenuItem")
                                    {
                                        continue;
                                    }

                                    if (pvtDataSet.Tables["Company"].Rows[this.companytoolStripComboBox.SelectedIndex]["COMPANY_ACCESS_IND"].ToString() != "A")
                                    {
                                        if (tsToolStripMenuItem1.DropDownItems.Count > 0)
                                        {
                                            pvtMenuDataView = null;
                                            pvtMenuDataView = new DataView(pvtDataSet.Tables["Menus"], "COMPANY_NO = " + pvtDataSet.Tables["Company"].Rows[this.companytoolStripComboBox.SelectedIndex]["COMPANY_NO"].ToString() + " AND MENU_ITEM_ID >= '" + tsToolStripMenuItem1.Tag.ToString() + "0' AND MENU_ITEM_ID <= '" + tsToolStripMenuItem1.Tag.ToString() + "Z'", "", DataViewRowState.CurrentRows);

                                            if (pvtMenuDataView.Count == 0)
                                            {
                                                tsToolStripMenuItem1.Visible = false;
                                            }
                                            else
                                            {
                                                blnPrevVisibleIsSeperator = false;
                                                tsToolStripMenuItem1.Visible = true;
                                                tsToolStripMenuItem1.Enabled = true;


                                            }
                                        }
                                        else
                                        {
                                            pvtMenuDataView = null;
                                            pvtMenuDataView = new DataView(pvtDataSet.Tables["Menus"], "COMPANY_NO = " + pvtDataSet.Tables["Company"].Rows[this.companytoolStripComboBox.SelectedIndex]["COMPANY_NO"].ToString() + " AND MENU_ITEM_ID = '" + tsToolStripMenuItem1.Tag.ToString() + "'", "", DataViewRowState.CurrentRows);

                                            if (pvtMenuDataView.Count == 0)
                                            {
                                                tsToolStripMenuItem1.Visible = false;
                                            }
                                            else
                                            {
                                                blnPrevVisibleIsSeperator = false;
                                                tsToolStripMenuItem1.Visible = true;
                                                tsToolStripMenuItem1.Enabled = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //Administrator
                                        blnPrevVisibleIsSeperator = false;
                                        tsToolStripMenuItem1.Visible = true;
                                        tsToolStripMenuItem1.Enabled = true;
                                    }
                                }
                                else
                                {
                                    if (tsMenuItem.DropDownItems[intItem1Count].GetType().Name == "ToolStripSeparator")
                                    {
                                        if (tsMenuItem.Visible == true)
                                        {
                                            ToolStripSeparator tsToolStripSeparator1 = (ToolStripSeparator)tsMenuItem.DropDownItems[intItem1Count];

                                            if (pvtDataSet.Tables["Company"].Rows[this.companytoolStripComboBox.SelectedIndex]["COMPANY_ACCESS_IND"].ToString() != "A")
                                            {
                                                if (blnPrevVisibleIsSeperator == true)
                                                {
                                                    tsToolStripSeparator1.Visible = false;
                                                }
                                                else
                                                {
                                                    tsToolStripSeparator1.Visible = true;
                                                    blnPrevVisibleIsSeperator = true;
                                                    tsToolStripSeparatorSaved = tsToolStripSeparator1;
                                                }
                                            }
                                            else
                                            {
                                                //Administrator
                                                tsToolStripSeparator1.Visible = true;
                                                blnPrevVisibleIsSeperator = true;
                                                tsToolStripSeparatorSaved = tsToolStripSeparator1;
                                            }
                                        }
                                    }
                                }

                                if (intItem1Count == tsMenuItem.DropDownItems.Count - 1)
                                {
                                    if (tsMenuItem.Text != "Password")
                                    {
                                        if (blnPrevVisibleIsSeperator == true)
                                        {
                                            tsToolStripSeparatorSaved.Visible = false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //NB Will Remove The First Seperator if NO Buttons are Visible
                    blnPrevVisibleIsSeperator = true;

                    ////Set ToolBar Button TooTipText and Enabled Property
                    foreach (ToolStripItem btnToolBarButton in MainToolStrip.Items)
                    {
                        if (btnToolBarButton.GetType().Name == "ToolStripButton")
                        {
                            if (pvtDataSet.Tables["Company"].Rows[this.companytoolStripComboBox.SelectedIndex]["COMPANY_ACCESS_IND"].ToString() != "A")
                            {
                                if (btnToolBarButton.ToolTipText == "New"
                                     | btnToolBarButton.ToolTipText == "Update"
                                     | btnToolBarButton.ToolTipText == "Delete"
                                     | btnToolBarButton.ToolTipText == "Save"
                                     | btnToolBarButton.ToolTipText == "Refresh")
                                {
                                    btnToolBarButton.Visible = true;
                                    blnPrevVisibleIsSeperator = false;
                                }
                                else
                                {
                                    pvtMenuDataView = null;
                                    pvtMenuDataView = new DataView(pvtDataSet.Tables["Menus"], "COMPANY_NO = " + pvtDataSet.Tables["Company"].Rows[this.companytoolStripComboBox.SelectedIndex]["COMPANY_NO"].ToString() + " AND MENU_ITEM_ID = '" + btnToolBarButton.Tag.ToString() + "'", "", DataViewRowState.CurrentRows);

                                    if (pvtMenuDataView.Count == 0)
                                    {
                                        btnToolBarButton.Visible = false;
                                    }
                                    else
                                    {
                                        btnToolBarButton.Visible = true;
                                        blnPrevVisibleIsSeperator = false;
                                    }
                                }
                            }
                            else
                            {
                                btnToolBarButton.Visible = true;
                            }
                        }
                        else
                        {
                            if (btnToolBarButton.GetType().Name == "ToolStripSeparator")
                            {
                                if (pvtDataSet.Tables["Company"].Rows[this.companytoolStripComboBox.SelectedIndex]["COMPANY_ACCESS_IND"].ToString() != "A")
                                {
                                    if (blnPrevVisibleIsSeperator == true)
                                    {
                                        btnToolBarButton.Visible = false;
                                    }
                                    else
                                    {
                                        btnToolBarButton.Visible = true;
                                        blnPrevVisibleIsSeperator = true;
                                    }
                                }
                                else
                                {
                                    btnToolBarButton.Visible = true;
                                }
                            }
                        }
                    }
                }

                if (pvtDataSet.Tables["Company"].Rows[companytoolStripComboBox.SelectedIndex]["DYNAMIC_UPLOAD_KEY"].ToString() != "")
                {
                    //Upload Is Dynamic
                    dataUploadTimesheetsToolStripMenuItem.Enabled = false;
                    toolStripUpload.Enabled = false;
                }
            }
        }

        private void Build_Menu_And_ToolBar_Events()
        {
            bool blnEditControl = false;

            foreach (ToolStripMenuItem tsMenuItem in this.MainMenuStrip.Items)
            {
                if (tsMenuItem.GetType().Name == "ToolStripMenuItem")
                {
                    if (tsMenuItem.Text == "Edit")
                    {
                        blnEditControl = true;
                    }
                    else
                    {
                        blnEditControl = false;
                    }

                    for (int intItem1Count = 0; intItem1Count < tsMenuItem.DropDownItems.Count; intItem1Count++)
                    {
                        if (tsMenuItem.DropDownItems[intItem1Count].GetType().Name == "ToolStripMenuItem")
                        {
                            ToolStripMenuItem tsToolStripMenuItem1 = (ToolStripMenuItem)tsMenuItem.DropDownItems[intItem1Count];

                            if (blnEditControl == true)
                            {
                                tsToolStripMenuItem1.Enabled = false;
                            }

                            tsToolStripMenuItem1.Click += new System.EventHandler(MenuItem_Click);

                            //Set ToolBar Button TooTipText and Enabled Property
                            foreach (ToolStripItem btnToolBarButton in this.MainToolStrip.Items)
                            {
                                if (btnToolBarButton.Tag != null)
                                {
                                    if (btnToolBarButton.Tag == tsToolStripMenuItem1.Tag)
                                    {
                                        btnToolBarButton.Click += new EventHandler(ToolBarItem_Click);

                                        btnToolBarButton.Enabled = tsToolStripMenuItem1.Enabled;

                                        if (btnToolBarButton.ToolTipText == "")
                                        {
                                            btnToolBarButton.ToolTipText = tsToolStripMenuItem1.Text;
                                            this.MainToolStrip.Refresh();
                                        }

                                        break;
                                    }
                                }
                            }

                            for (int intItem2Count = 0; intItem2Count < tsToolStripMenuItem1.DropDownItems.Count; intItem2Count++)
                            {
                                if (tsToolStripMenuItem1.DropDownItems[intItem2Count].GetType().Name == "ToolStripMenuItem")
                                {
                                    ToolStripMenuItem tsToolStripMenuItem2 = (ToolStripMenuItem)tsToolStripMenuItem1.DropDownItems[intItem2Count];

                                    tsToolStripMenuItem2.Click += new System.EventHandler(MenuItem_Click);

                                    //Set ToolBar Button TooTipText and Enabled Property
                                    foreach (ToolStripItem btnToolBarButton in MainToolStrip.Items)
                                    {
                                        if (btnToolBarButton.Tag != null)
                                        {
                                            if (btnToolBarButton.Tag == tsToolStripMenuItem2.Tag)
                                            {
                                                btnToolBarButton.Click += new EventHandler(ToolBarItem_Click);

                                                btnToolBarButton.Enabled = tsToolStripMenuItem2.Enabled;

                                                if (btnToolBarButton.ToolTipText == "")
                                                {
                                                    btnToolBarButton.ToolTipText = tsToolStripMenuItem2.Text;
                                                    MainMenuStrip.Refresh();
                                                }

                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem ToolStripMenuItem = (ToolStripMenuItem)sender;

            Menu_Click(ToolStripMenuItem);
        }

        private void ToolBarItem_Click(object sender, EventArgs e)
        {
            ToolStripButton tbrButton = (ToolStripButton)sender;

            int intIndex = 0;

            if (tbrButton.Tag.ToString().Substring(0, 1) == "A")
            {
                intIndex = 9;
            }
            else
            {
                intIndex = Convert.ToInt32(tbrButton.Tag.ToString().Substring(0, 1)) - 1;
            }

            //Get Menu Header
            ToolStripMenuItem tsMenuItem = (ToolStripMenuItem)this.MainMenuStrip.Items[intIndex];

            for (int intItem1Count = 0; intItem1Count < tsMenuItem.DropDownItems.Count; intItem1Count++)
            {
                if (tsMenuItem.DropDownItems[intItem1Count].GetType().Name == "ToolStripMenuItem")
                {
                    ToolStripMenuItem tsToolStripMenuItem1 = (ToolStripMenuItem)tsMenuItem.DropDownItems[intItem1Count];

                    if (tsToolStripMenuItem1.Tag == tbrButton.Tag)
                    {
                        this.Menu_Click(tsToolStripMenuItem1);
                        return;
                    }
                    else
                    {
                        for (int intItem2Count = 0; intItem2Count < tsToolStripMenuItem1.DropDownItems.Count; intItem2Count++)
                        {
                            if (tsToolStripMenuItem1.DropDownItems[intItem2Count].GetType().Name == "ToolStripMenuItem")
                            {
                                ToolStripMenuItem tsToolStripMenuItem2 = (ToolStripMenuItem)tsToolStripMenuItem1.DropDownItems[intItem2Count];

                                if (tsToolStripMenuItem2.Tag == tbrButton.Tag)
                                {
                                    this.Menu_Click(tsToolStripMenuItem2);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void frmPayrollMain_MdiChildActivate(object sender, EventArgs e)
        {
            Form myMDIForm = this.ActiveMdiChild;

            if (this.ActiveMdiChild == null)
            {
                //First Disable All Edit MenuItems & Edit ToolBarButtons 
                Disable_All_Edit_MenuItems_ToolBarItems();
            }
            else
            {
                if (this.ActiveMdiChild.Tag != null)
                {
                    Control mylblHeaderControl = myMDIForm.Controls["lblHeader"];

                    if (mylblHeaderControl != null)
                    {
                        mylblHeaderControl.ForeColor = Color.Black;
                    }

                    Control mybtnHeaderCloseControl = myMDIForm.Controls["btnHeaderClose"];

                    if (mybtnHeaderCloseControl != null)
                    {
                        mybtnHeaderCloseControl.ForeColor = Color.Black;
                    }

                    Control mybtnHeaderMinimizeControl = myMDIForm.Controls["btnHeaderMinimize"];

                    if (mybtnHeaderMinimizeControl != null)
                    {
                        mybtnHeaderMinimizeControl.ForeColor = Color.Black;
                    }

                    //Force Form to Redraw
                    myMDIForm.Invalidate();

                    Enable_Disable_MenuItem_ToolBarItem_From_Forms_Tag(this.ActiveMdiChild.Tag.ToString(), false);

                    //Set All Edit values to Disable

                    this.newToolStripMenuItem.Enabled = false;
                    Set_Corresponding_ToolBar_Button(newToolStripMenuItem);

                    this.updateToolStripMenuItem.Enabled = false;
                    Set_Corresponding_ToolBar_Button(updateToolStripMenuItem);

                    this.deleteToolStripMenuItem.Enabled = false;
                    Set_Corresponding_ToolBar_Button(deleteToolStripMenuItem);

                    this.saveToolStripMenuItem.Enabled = false;
                    Set_Corresponding_ToolBar_Button(saveToolStripMenuItem);

                    this.cancelToolStripMenuItem.Enabled = false;
                    Set_Corresponding_ToolBar_Button(saveToolStripMenuItem);

                    //Set Buttons Accoring to This
                    foreach (Control myControl in this.ActiveMdiChild.Controls)
                    {
                        if (myControl is Button)
                        {
                            switch (myControl.Text.Replace("&", ""))
                            {
                                case "New":

                                    this.newToolStripMenuItem.Enabled = myControl.Enabled;
                                    Set_Corresponding_ToolBar_Button(newToolStripMenuItem);

                                    break;

                                case "Update":

                                    this.updateToolStripMenuItem.Enabled = myControl.Enabled;
                                    Set_Corresponding_ToolBar_Button(updateToolStripMenuItem);

                                    break;

                                case "Delete":

                                    this.deleteToolStripMenuItem.Enabled = myControl.Enabled;
                                    Set_Corresponding_ToolBar_Button(deleteToolStripMenuItem);

                                    break;

                                case "Save":

                                    this.saveToolStripMenuItem.Enabled = myControl.Enabled;
                                    Set_Corresponding_ToolBar_Button(saveToolStripMenuItem);

                                    break;

                                case "Cancel":

                                    this.cancelToolStripMenuItem.Enabled = myControl.Enabled;
                                    Set_Corresponding_ToolBar_Button(saveToolStripMenuItem);

                                    break;
                            }
                        }
                        else
                        {
                            if (myMDIForm.Name == "frmCompanyPaymentOptions")
                            {
                                if (myControl is TabControl
                                && myControl.Name == "tabControlMain")
                                {
                                    foreach (Control myControl1 in myControl.Controls)
                                    {
                                        if (myControl1 is TabPage
                                        && myControl1.Name == "tabPageSelection")
                                        {
                                            foreach (Control myControl2 in myControl1.Controls)
                                            {
                                                if (myControl2 is Button)
                                                {
                                                    Button button = (Button)myControl2;

                                                    switch (myControl2.Text.Replace("&", ""))
                                                    {
                                                        case "New":

                                                            this.newToolStripMenuItem.Enabled = myControl2.Enabled;
                                                            Set_Corresponding_ToolBar_Button(newToolStripMenuItem);

                                                            break;

                                                        case "Update":

                                                            this.updateToolStripMenuItem.Enabled = myControl2.Enabled;
                                                            Set_Corresponding_ToolBar_Button(updateToolStripMenuItem);

                                                            break;

                                                        case "Delete":

                                                            this.deleteToolStripMenuItem.Enabled = myControl2.Enabled;
                                                            Set_Corresponding_ToolBar_Button(deleteToolStripMenuItem);

                                                            break;

                                                        case "Save":

                                                            this.saveToolStripMenuItem.Enabled = myControl2.Enabled;
                                                            Set_Corresponding_ToolBar_Button(saveToolStripMenuItem);

                                                            break;

                                                        case "Cancel":

                                                            this.cancelToolStripMenuItem.Enabled = myControl2.Enabled;
                                                            Set_Corresponding_ToolBar_Button(saveToolStripMenuItem);

                                                            break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Disable_All_Edit_MenuItems_ToolBarItems()
        {
            //First Disable All Edit MenuItems & Edit ToolBarButtons 
            if (this.newToolStripMenuItem.Enabled == true)
            {
                this.newToolStripMenuItem.Enabled = false;
                Set_Corresponding_ToolBar_Button(newToolStripMenuItem);
            }

            if (this.updateToolStripMenuItem.Enabled == true)
            {
                this.updateToolStripMenuItem.Enabled = false;
                Set_Corresponding_ToolBar_Button(updateToolStripMenuItem);
            }

            if (this.deleteToolStripMenuItem.Enabled == true)
            {
                this.deleteToolStripMenuItem.Enabled = false;
                Set_Corresponding_ToolBar_Button(deleteToolStripMenuItem);
            }

            if (this.saveToolStripMenuItem.Enabled == true)
            {
                this.saveToolStripMenuItem.Enabled = false;
                Set_Corresponding_ToolBar_Button(saveToolStripMenuItem);
            }

            if (this.cancelToolStripMenuItem.Enabled == true)
            {
                this.cancelToolStripMenuItem.Enabled = false;
                Set_Corresponding_ToolBar_Button(cancelToolStripMenuItem);
            }
        }

        private void Enable_Disable_MenuItem_ToolBarItem_From_Forms_Tag(string FormTag, bool Enable)
        {
            int intIndex = 0;

            if (FormTag.Substring(0, 1) == "A")
            {
                if (FormTag == "A1")
                {
                    //Help System - Leave Menu Enabled due to Hiding of Form
                    return;
                }
                else
                {
                    intIndex = 9;
                }
            }
            else
            {
                intIndex = Convert.ToInt32(FormTag.Substring(0, 1)) - 1;
            }

            //Get Menu Header
            ToolStripMenuItem tsMenuItem = (ToolStripMenuItem)this.MainMenuStrip.Items[intIndex];

            for (int intItem1Count = 0; intItem1Count < tsMenuItem.DropDownItems.Count; intItem1Count++)
            {
                if (tsMenuItem.DropDownItems[intItem1Count].GetType().Name == "ToolStripMenuItem")
                {
                    ToolStripMenuItem tsToolStripMenuItem1 = (ToolStripMenuItem)tsMenuItem.DropDownItems[intItem1Count];

                    if (tsToolStripMenuItem1.Tag.ToString() == FormTag)
                    {
                        tsToolStripMenuItem1.Enabled = Enable;
                        break;
                    }
                    else
                    {
                        for (int intItem2Count = 0; intItem2Count < tsToolStripMenuItem1.DropDownItems.Count; intItem2Count++)
                        {
                            if (tsToolStripMenuItem1.DropDownItems[intItem2Count].GetType().Name == "ToolStripMenuItem")
                            {
                                ToolStripMenuItem tsToolStripMenuItem2 = (ToolStripMenuItem)tsToolStripMenuItem1.DropDownItems[intItem2Count];

                                if (tsToolStripMenuItem2.Tag.ToString() == FormTag)
                                {
                                    tsToolStripMenuItem2.Enabled = Enable;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            //Set ToolBar Button TooTipText and Enabled Property
            foreach (ToolStripItem btnToolBarButton in MainToolStrip.Items)
            {
                if (btnToolBarButton.Tag != null)
                {
                    if (btnToolBarButton.Tag.ToString() == FormTag)
                    {
                        btnToolBarButton.Enabled = Enable;
                        MainToolStrip.Refresh();

                        break;
                    }
                }
            }
        }

        private void MdiChild_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form myForm = (Form)sender;
            
            Control mybtnUpdateSave = null;

            if (myForm.Name == "frmCompanyPaymentOptions")
            {
                Control mytabControlMain = myForm.Controls["tabControlMain"];
                Control mytabPageSelection = mytabControlMain.Controls["tabPageSelection"];
                mybtnUpdateSave = mytabPageSelection.Controls["btnSave"];
            }
            else
            {
                mybtnUpdateSave = myForm.Controls["btnSave"];
            }

            if (mybtnUpdateSave != null)
            {
                if (mybtnUpdateSave.Enabled == true)
                {
                    frmCloseOrSave frmCloseOrSave = new frmCloseOrSave();
                    frmCloseOrSave.Paint += new System.Windows.Forms.PaintEventHandler(MdiChild_Paint);

                    DialogResult myDialogResult = frmCloseOrSave.ShowDialog();

                    if (myDialogResult == DialogResult.Yes)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }

            //Set Form's MenuItem / ToolBarItem to Enabled
            Enable_Disable_MenuItem_ToolBarItem_From_Forms_Tag(myForm.Tag.ToString(), true);

            //Remove From Window Drop Down
            for (int intCount = 0; intCount < windowToolStripMenuItem.DropDownItems.Count; intCount++)
            {
                if (myForm.Tag == windowToolStripMenuItem.DropDownItems[intCount].Tag)
                {
                    windowToolStripMenuItem.DropDownItems.RemoveAt(intCount);
                    break;
                }
            }
        }

        private void MdiChild_Leave(object sender, EventArgs e)
        {
            Form myForm = (Form)sender;

            if (this.ActiveMdiChild != null
            && this.ActiveMdiChild.Name != myForm.Name)
            {
                //frmTimeAttendanceInternetMain Got Focus
                Color myColor = myForm.BackColor;

                Control mylblHeaderControl = myForm.Controls["lblHeader"];

                if (mylblHeaderControl != null)
                {
                    mylblHeaderControl.ForeColor = myColor;
                }

                Control mybtnHeaderCloseControl = myForm.Controls["btnHeaderClose"];

                if (mybtnHeaderCloseControl != null)
                {
                    mybtnHeaderCloseControl.ForeColor = myColor;
                }

                Control mybtnHeaderMinimizeControl = myForm.Controls["btnHeaderMinimize"];

                if (mybtnHeaderMinimizeControl != null)
                {
                    mybtnHeaderMinimizeControl.ForeColor = myColor;
                }
            }
        }

        private void MdiChild_ButtonClose_Click(object sender, EventArgs e)
        {
            Button currentButton = (Button)sender;

            Form currentForm = (Form)currentButton.Parent;

            currentForm.Close();
        }

        private void MdiChild_ButtonMinimize_Click(object sender, EventArgs e)
        {
            Button currentButton = (Button)sender;

            Form currentForm = (Form)currentButton.Parent;

            currentForm.WindowState = FormWindowState.Minimized;
        }

        public void MdiChild_Paint(object sender, PaintEventArgs e)
        {
            Form frm = (Form)sender;
            Rectangle myRectangle = new Rectangle(frm.ClientRectangle.X - 2, frm.ClientRectangle.Y - 2, frm.ClientRectangle.Width - 2, frm.ClientRectangle.Height - 2);

            if (this.ActiveMdiChild == frm
            || frm.Name == "frmCloseOrSave")
            {
                ControlPaint.DrawBorder(e.Graphics, frm.ClientRectangle,
                System.Drawing.Color.Black, 1, ButtonBorderStyle.Solid,
                System.Drawing.Color.Black, 1, ButtonBorderStyle.Solid,
                System.Drawing.Color.Black, 1, ButtonBorderStyle.Solid,
                System.Drawing.Color.Black, 1, ButtonBorderStyle.Solid);

                Pen blackPen = new Pen(Color.Black, 1);
                e.Graphics.DrawLine(blackPen, 0, 31, frm.Width, 31);
            }
            else
            {
                Color myColor = frm.BackColor;

                ControlPaint.DrawBorder(e.Graphics, frm.ClientRectangle,
                myColor, 1, ButtonBorderStyle.Solid,
                myColor, 1, ButtonBorderStyle.Solid,
                myColor, 1, ButtonBorderStyle.Solid,
                myColor, 1, ButtonBorderStyle.Solid);

                Pen myPen = new Pen(myColor, 1);
                e.Graphics.DrawLine(myPen, 0, 31, frm.Width, 31);
            }
        }

        private void MdiChild_TextChanged(object sender, EventArgs e)
        {
            Form myForm = (Form)sender;

            foreach (Control myContol in myForm.Controls)
            {
                if (myContol.Name == "lblHeader")
                {
                    myContol.Text = myForm.Text;
                    break;
                }
            }
        }

        private void Set_Corresponding_ToolBar_Button(ToolStripMenuItem ToolStripMenuEditItem)
        {
            //Set ToolBar Button TooTipText and Enabled Property
            foreach (ToolStripItem btnToolBarButton in MainToolStrip.Items)
            {
                if (btnToolBarButton.Tag == ToolStripMenuEditItem.Tag)
                {
                    btnToolBarButton.Enabled = ToolStripMenuEditItem.Enabled;

                    break;
                }
            }
        }

        private void TimerCloseCurrentForm_Tick(object sender, EventArgs e)
        {
            try
            {
                //Stop Timer
                this.TimerCloseCurrentForm.Enabled = false;

                this.pnlGlobe.Visible = false;
                this.pnlGlobe.Refresh();
                Application.DoEvents();

                Form myForm = (Form)AppDomain.CurrentDomain.GetData("FormToClose");

                if (myForm.Tag == null)
                {
                    myForm.Tag = "";
                }

                Enable_Disable_MenuItem_ToolBarItem_From_Forms_Tag(myForm.Tag.ToString(), true);

                myForm.Close();
            }
            catch
            {
            }
        }

        private void TimerReloadCompanies_Tick(object sender, EventArgs e)
        {
            TimerReloadCompanies.Enabled = false;

            Load_Companies();
        }

        private void tmrMenuClick_Tick(object sender, EventArgs e)
        {
            tmrMenuClick.Enabled = false;

            string strMenuDesc = AppDomain.CurrentDomain.GetData("MenuClick").ToString();

            if (strMenuDesc == "openPayrollRunToolStripMenuItem")
            {
                Menu_Click(this.openPayrollRunToolStripMenuItem);
            }
            else
            {
                if (strMenuDesc == "runTimeAndAttendanceToolStripMenuItem")
                {
                    Menu_Click(this.runTimeAndAttendanceToolStripMenuItem);
                }
                else
                {
                    if (strMenuDesc == "companyToolStripMenuItem")
                    {
                        Menu_Click(this.companyToolStripMenuItem);
                    }
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMinimise_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void MainMenuStrip_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void MainMenuStrip_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void MainToolStrip_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void MainToolStrip_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }
    }

    class MyToolStripRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (!e.Item.Selected)
            {
                base.OnRenderButtonBackground(e);
            }
            else
            {
                Rectangle rectangle = new Rectangle(0, 0, e.Item.Size.Width, e.Item.Size.Height);
                e.Graphics.FillRectangle(Brushes.Silver, rectangle);
                //e.Graphics.DrawRectangle(Pens.Black, rectangle);
            }
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            //base.OnRenderToolStripBorder(e);
        }

        protected override void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
        {
            // don't draw at all
        }
    }

    class CustomMenuStriplRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected == true)
            {
                if (e.Item.Name == "fileToolStripMenuItem"
                || e.Item.Name == "editToolStripMenuItem"
                || e.Item.Name == "setupToolStripMenuItem"
                || e.Item.Name == "activateToolStripMenuItem"
                || e.Item.Name == "processToolStripMenuItem"
                || e.Item.Name == "payrollRunToolStripMenuItem"
                || e.Item.Name == "reportsToolStripMenuItem"
                || e.Item.Name == "toolToolStripMenuItem"
                || e.Item.Name == "passwordToolStripMenuItem"
                || e.Item.Name == "windowToolStripMenuItem")
                {
                    e.Item.ForeColor = Color.Silver;
                    ToolStripDropDownItem menuItem = (ToolStripDropDownItem)e.Item;
                    menuItem.ShowDropDown();
                }
                else
                {
                    Rectangle rc = new Rectangle(1, 0, e.Item.Size.Width, e.Item.Size.Height);

                    if (e.Item.Enabled == true)
                    {
                        e.Graphics.FillRectangle(Brushes.DimGray, rc);
                    }
                    else
                    {
                        e.Graphics.FillRectangle(Brushes.Silver, rc);
                    }
                }
            }
            else
            {
                if (e.Item.Enabled == true)
                {
                    //Disabled
                    e.Item.ForeColor = Color.Black;
                }
                else
                {
                    e.Item.ForeColor = Color.Silver;
                }
            }
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            //Removes White Line Under ToolStrip
            //base.OnRenderToolStripBorder(e);
        }

        protected override void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
        {
            // don't draw at all
        }
    }
}
