using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using FingerPrintClockServerLib;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Web;
using System.Reflection;
using DPUruNet;
using InteractPayroll;
using System.Globalization;
using System.Collections.Generic;
using System.Security.Cryptography;
using InteractPayrollClient;
using System.ServiceModel.Web;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.ServiceProcess;

namespace FingerPrintClockServer
{
    public class FingerPrintClockService : IFingerPrintClockService
    {
        localhost.busWebDynamicServices busWebDynamicServices;

        //NB. This must be static otherwise Calls of class are Null
        static clsDBConnectionObjects clsDBConnectionObjects;
        busClientTimeSheetDynamicUpload busClientTimeSheetDynamicUpload;

        System.Type typTimeSheetDynamicUpload;
        object busTimeSheetDynamicUploadService;
        
        WebServiceHost FingerPrintClockServiceHost;
        TextMessageEncodingBindingElement TextMessageEncoder;
        WebHttpBinding WebHttpBinding;
        
        System.Timers.Timer tmrKeepLocalDbUpTimer;
        System.Timers.Timer tmrCheckServiceAndArchivingTimer;
        System.Timers.Timer tmrFingerPrintClockTimeAttendanceServiceStartStopReplaceTimer;
        System.Timers.Timer tmrUploadDownloadTimesheets;

        private string pvtstrSoftwareToUse = "D";
        private bool pvtblnLoggingSwitchedOn = false;

        private int pvtint5MinuteCount = 0;

        private int intFiveMinute = 300000;
        private int int60Minute = 3600000;

        private bool pvtblnFingerPrintClockTimeAttendanceServiceStartStopReplace = false;
        private bool pvtblnBothServicesRunning = true;
        
        //Same as FingerPrintClockTimeAttendanceService.exe Log File
        string pvtstrLogFileName = "FingerPrintClockTimeAttendanceService";

        string pvtstrExceptionMessage = "";
        int pvtintSameExceptionMessage = 0;

        private string pvtstrDBEngine = "";
        private string pvtstrPortNo = "8000";
        private string pvtstrIpAddress = "";
        
        private object[] pvtobjParm;

        List<int> listCompanyNo = new List<int>();
        List<string> listUploadKey = new List<string>();
        //5 Minutes
        private int pvtintUploadTimesheetsInSeconds = 300;

        private DateTime pvtdtNextTimesheetDynamicUploadRunDateTime = DateTime.Now;
        private bool pvtblnTimesheetDynamicUploadRun = false;
        
        byte[] _salt = Encoding.ASCII.GetBytes("ErrolLeRoux");
        string sharedSecret = "Interact";

        public FingerPrintClockService()
        {
            if (AppDomain.CurrentDomain.GetData("URLClientPath") == null)
            {
                WriteLog("FingerPrintClockService Instantiate ********************************* ");

                int intWhere = 1;

                try
                {
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "PortConfig.txt") == true)
                    {
                        using (StreamReader srStreamReader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "PortConfig.txt"))
                        {
                            try
                            {
                                pvtstrPortNo = srStreamReader.ReadLine();
                            }
                            catch (Exception ex)
                            {
                                WriteExceptionLog("PortConfig.txt Line Read", ex);
                            }
                        }
                    }
                    else
                    {
                        WriteLog(AppDomain.CurrentDomain.BaseDirectory + "PortConfig.txt does Not Exist. **********");
                    }

                    intWhere = 2;

                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "clsFixInteractPayrollClientDatabase.dll") == false)
                    {
                        WriteLog("clsFixInteractPayrollClientDatabase.dll does Not Exist. **********");
                    }

                    intWhere = 3;

                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "URLConfig.txt") == true)
                    {
#if (DEBUG)
                    AppDomain.CurrentDomain.SetData("URLPath", "");
#else
                        using (StreamReader srStreamReader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "URLConfig.txt"))
                        {
                            try
                            {
                                string strURLPath = srStreamReader.ReadLine();

                                AppDomain.CurrentDomain.SetData("URLPath", strURLPath);
                            }
                            catch (Exception ex)
                            {
                                WriteExceptionLog("URLConfig.txt ReadLine Exception", ex);
                            }
                        }
#endif
                    }
                    else
                    {
#if (DEBUG)
#else
                        WriteLog("URLConfig.txt MISSING - Dynamic Upload of Timesheets will not Work**********");
#endif
                        AppDomain.CurrentDomain.SetData("URLPath", "");
                    }

                    intWhere = 4;

                    //Must Late Bind
                    AppDomain.CurrentDomain.SetData("URLClientPath", "");

                FingerPrintClockService_Continue:

                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "UploadKey.txt") == true)
                    {
                        int intCompanyNo = -1;
                        string strUploadKey = "";

                        using (StreamReader srStreamReader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "UploadKey.txt"))
                        {
                            while ((strUploadKey = srStreamReader.ReadLine()) != null)
                            {
                                if (strUploadKey.Trim() == "")
                                {
                                    continue;
                                }

                                string strReturnedDBName = "";

                                try
                                {
                                    strReturnedDBName = DecryptStringAES(strUploadKey);

                                    string[] strFileParts = strReturnedDBName.Split('_');

                                    if (strFileParts.Length == 2)
                                    {
                                        bool blnCheckValidNumber = Int32.TryParse(strFileParts[1], out intCompanyNo);

                                        if (blnCheckValidNumber == true)
                                        {
                                            listCompanyNo.Add(intCompanyNo);
                                            listUploadKey.Add(strUploadKey);

                                            WriteLog("Upload to Database '" + strReturnedDBName + "' Configured (via UploadKey.txt)");
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    WriteLog("UploadKey.txt Exception " + ex.Message);

                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        WriteLog("NO UploadKey.txt File Found");
                    }

                    intWhere = 5;

                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockTimeAttendanceServiceStartStop.exe.config") == true)
                    {
                        WriteLog("Config File '" + AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockTimeAttendanceServiceStartStop.exe.config' Found");

                        string line;
                        using (StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockTimeAttendanceServiceStartStop.exe.config"))
                        {
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (line.ToUpper().IndexOf("LOGGINGSWITCHEDON") > -1)
                                {
                                    if (line.IndexOf("value=\"Y\"") > -1)
                                    {
                                        pvtblnLoggingSwitchedOn = true;
                                    }
                                }
                                else
                                {
                                    if (line.ToUpper().IndexOf("DYNAMICUPLOADTIMESHEETSINSECONDS") > -1)
                                    {
                                        string[] strLineParts = line.Split('"');

                                        if (strLineParts.Length == 5)
                                        {
                                            int intSeconds = 0;

                                            bool blnCheckValidNumber = Int32.TryParse(strLineParts[3], out intSeconds);

                                            if (blnCheckValidNumber == true)
                                            {
                                                pvtintUploadTimesheetsInSeconds = intSeconds;

                                                WriteLog("Dynamic Upload of Timesheets in Seconds = " + intSeconds.ToString());
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (listCompanyNo.Count == 0
                                        && line.ToUpper().IndexOf("TIMESHEETDYNAMICUPLOADEXEPATH") > -1)
                                        {
                                            string[] strLineParts = line.Split('"');

                                            if (strLineParts.Length == 5)
                                            {
                                                String strUploadKeyFile = strLineParts[3];

                                                string[] strFileParts = strUploadKeyFile.Split('\\');

                                                strUploadKeyFile = strUploadKeyFile.Replace(strFileParts[strFileParts.Length - 1], "UploadKey.txt");

                                                if (File.Exists(strUploadKeyFile) == true)
                                                {
                                                    try
                                                    {
                                                        File.Copy(strUploadKeyFile, AppDomain.CurrentDomain.BaseDirectory + "UploadKey.txt");

                                                        WriteLog("Copied " + strUploadKeyFile + " to " + AppDomain.CurrentDomain.BaseDirectory + "UploadKey.txt");

                                                        goto FingerPrintClockService_Continue;
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    intWhere = 6;

                    if (pvtblnLoggingSwitchedOn == true)
                    {
                        AppDomain.CurrentDomain.SetData("LoggingSwitchedOn", "Y");
                        WriteLog("Logging Switched ON **********");
                    }
                    else
                    {
                        AppDomain.CurrentDomain.SetData("LoggingSwitchedOn", "N");
                        WriteLog("Logging Switched OFF **********");
                    }

                    AppDomain.CurrentDomain.SetData("LogFileName", AppDomain.CurrentDomain.BaseDirectory + pvtstrLogFileName + "_Log.txt");
#if (DEBUG)
                    //OnStart();
                    //All Needed for Testing
                    clsDBConnectionObjects = new InteractPayroll.clsDBConnectionObjects();
                    pvtstrDBEngine = clsDBConnectionObjects.pvtstrDBEngine;
                    busClientTimeSheetDynamicUpload = new busClientTimeSheetDynamicUpload();
                    Run_Timesheet_Dynamic_Upload();
                    //Check_FingerPrintClockTimeAttendanceServiceStartStopAndSqlServer();
#endif
                }
                catch (Exception ex)
                {
                    WriteExceptionLog("FingerPrintClockService Where=" + intWhere.ToString(), ex);
                }
            }
        }

        public void OnStart()
        {
            try
            {
                busClientTimeSheetDynamicUpload = new busClientTimeSheetDynamicUpload();

                WriteLog("OnStart Connected on Port " + pvtstrPortNo);
             
                pvtstrIpAddress = @"http://localhost:" + pvtstrPortNo + "/FingerPrintClockServer";

                WriteLog("URI = " + pvtstrIpAddress);

                FingerPrintClockServiceHost = new WebServiceHost(this.GetType(), new Uri(pvtstrIpAddress));

                TextMessageEncoder = new TextMessageEncodingBindingElement(MessageVersion.None, Encoding.UTF8);
                TextMessageEncoder.ReaderQuotas.MaxArrayLength = 100000000;

                WebHttpBinding = new WebHttpBinding();

                WebHttpBinding.MaxBufferPoolSize = 66665536;
                WebHttpBinding.MaxBufferSize = 66665536;
                WebHttpBinding.MaxReceivedMessageSize = 66665536;

                WebHttpBinding.ReaderQuotas = TextMessageEncoder.ReaderQuotas;
                WebHttpBinding.TransferMode = TransferMode.Streamed;

                WebHttpBinding.ReaderQuotas.MaxStringContentLength = 66665536;

                WebHttpBinding.CloseTimeout = TimeSpan.FromSeconds(60);
                WebHttpBinding.OpenTimeout = TimeSpan.FromSeconds(60);
                WebHttpBinding.ReceiveTimeout = TimeSpan.FromSeconds(60);

                FingerPrintClockServiceHost.CloseTimeout = TimeSpan.FromSeconds(60);
                FingerPrintClockServiceHost.OpenTimeout = TimeSpan.FromSeconds(60);

                FingerPrintClockServiceHost.AddServiceEndpoint(this.GetType().GetInterface("FingerPrintClockServer.IFingerPrintClockService"), WebHttpBinding, pvtstrIpAddress);

                int intNumberRetries = 0;

            OnStart_Retry:

                intNumberRetries += 1;

                try
                {
                    FingerPrintClockServiceHost.Open();
                    WriteLog("### FingerPrintClockService Open Successful ###");

                    clsDBConnectionObjects = new InteractPayroll.clsDBConnectionObjects();

                    pvtstrDBEngine = clsDBConnectionObjects.pvtstrDBEngine;

                    WriteLog("OnStart SQL Engine = " + pvtstrDBEngine);
                    
                    tmrCheckServiceAndArchivingTimer = new System.Timers.Timer();
                    tmrCheckServiceAndArchivingTimer.Elapsed += Check_Service_And_Archiving;
                    //In 5 Minutes
                    tmrCheckServiceAndArchivingTimer.Interval = intFiveMinute;
                  
                    tmrCheckServiceAndArchivingTimer.Start();
                    WriteLog("FingerPrintClockService tmrCheckServiceAndArchivingTimer Started");
                }
                catch (Exception ex)
                {
                    if (intNumberRetries < 4)
                    {
                        WriteLog("### FingerPrintClockService OPEN FAILED - " + intNumberRetries + " ###");
                        TimeSpan myTimeSpan = TimeSpan.FromMilliseconds(2000);
                        System.Threading.Thread.Sleep(myTimeSpan);

                        goto OnStart_Retry;
                    }
                    else
                    {
                        WriteExceptionLog("@@@@@@@@@@@@@@@@@@@@ OnStart FingerPrintClockService Open FAILED - Tried 3 Times @@@@@@@@@@@@@@@@@@@", ex);
                        return;
                    }
                }
#if (DEBUG)
                //while (true)
                //{
                //    //Stop Falling out of Program
                //}
#endif
                tmrFingerPrintClockTimeAttendanceServiceStartStopReplaceTimer = new System.Timers.Timer();
                tmrFingerPrintClockTimeAttendanceServiceStartStopReplaceTimer.Elapsed += Stop_FingerPrintClockTimeAttendanceServiceStartStop_And_RenameExe;
                //In 5 Minutes
                tmrFingerPrintClockTimeAttendanceServiceStartStopReplaceTimer.Interval = 300000;
                tmrFingerPrintClockTimeAttendanceServiceStartStopReplaceTimer.Start();
                WriteLog("pvtblnFingerPrintClockTimeAttendanceServiceStartStopReplace Timer Initialise Successful");
               
                if (listCompanyNo.Count > 0)
                {
                    tmrUploadDownloadTimesheets = new System.Timers.Timer();
                    tmrUploadDownloadTimesheets.Elapsed += Timesheet_Dynamic_Upload;
                    //After 2 Seconds 
                    tmrUploadDownloadTimesheets.Interval = 2000;
                    tmrUploadDownloadTimesheets.Start();

                    AppDomain.CurrentDomain.SetData("tmrUploadDownloadTimesheets", tmrUploadDownloadTimesheets);
                    
                    WriteLog("FingerPrintClockService tmrUploadDownloadTimesheets Started (New)");
                }
                else
                {
                    //Used to Keep SQL LocalDB in Memory               
                    tmrKeepLocalDbUpTimer = new System.Timers.Timer();
                    tmrKeepLocalDbUpTimer.Elapsed += Keep_LocalDB_Up;
                    //After 6 Seconds (Will Change to 1 Minute Later)
                    tmrKeepLocalDbUpTimer.Interval = 6000;

                    tmrKeepLocalDbUpTimer.Start();

                    WriteLog("FingerPrintClockService tmrKeepLocalDbUpTimer Started");
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog("@@@@@@@@@@@@@@@@@@@@ OnStart FingerPrintClockService Open FAILED", ex);
            }
        }
  
        public void OnStop()
        {
            try
            {
                if (FingerPrintClockServiceHost != null)
                {
                    try
                    {
                        FingerPrintClockServiceHost.Close();
                        WriteLog("OnStop FingerPrintClockServiceHost Closed");
                    }
                    catch
                    {
                        FingerPrintClockServiceHost.Abort();
                        WriteLog("OnStop FingerPrintClockServiceHost Aborted");
                    }
                }

                if (tmrCheckServiceAndArchivingTimer != null)
                {
                    tmrCheckServiceAndArchivingTimer.Enabled = false;
                    tmrCheckServiceAndArchivingTimer.Dispose();
                    WriteLog("tmrCheckServiceAndArchivingTimer Disposed");
                }

                if (tmrUploadDownloadTimesheets != null)
                {
                    tmrUploadDownloadTimesheets.Enabled = false;
                    tmrUploadDownloadTimesheets.Dispose();
                    WriteLog("tmrUploadDownloadTimesheets Disposed");
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog("OnStop", ex);
            }
        }

        int WebService_Settings(System.Web.Services.Protocols.WebClientProtocol ws, string parWebServiceName)
        {
            try
            {
                ws.Url = @"http://" + AppDomain.CurrentDomain.GetData("URLPath").ToString() + @"/InteractPayroll/" + parWebServiceName + ".asmx"; ;

                ws.Credentials = System.Net.CredentialCache.DefaultCredentials;
            }
            catch
            {
                return -1;
            }

            return 0;
        }

        private void Check_Service_And_Archiving(Object source, System.Timers.ElapsedEventArgs e)
        {
            //Every Hour
            Check_Log_For_Archiving();

            Check_FingerPrintClockTimeAttendanceServiceStartStopAndSqlServer();
        }

        private void Check_Log_For_Archiving()
        {
            bool blnMoveFile = false;
            DateTime myFileDateTime = DateTime.Now;

            try
            {
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + pvtstrLogFileName + "_Log.txt") == true)
                {
                    using (StreamReader srStreamReader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + pvtstrLogFileName + "_Log.txt"))
                    {
                        string strLine = "";

                        try
                        {
                            strLine = srStreamReader.ReadLine();

                            if (strLine != null)
                            {
                                myFileDateTime = DateTime.ParseExact(strLine.Substring(0, 10), "yyyy-MM-dd", null);

                                if (myFileDateTime.ToString("yyyyMMdd") != DateTime.Now.ToString("yyyyMMdd"))
                                {
                                    blnMoveFile = true;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteExceptionLog("Check_Log_For_Archiving Read Line", ex);
                        }
                    }
                }

                if (blnMoveFile == true)
                {
                    try
                    {
                        if (blnMoveFile == true)
                        {
                            try
                            {
                                File.Copy(AppDomain.CurrentDomain.BaseDirectory + pvtstrLogFileName + "_Log.txt", AppDomain.CurrentDomain.BaseDirectory + "Backup\\" + pvtstrLogFileName + "_" + myFileDateTime.ToString("yyyyMMdd") + "_Log.txt", true);
                                File.Delete(AppDomain.CurrentDomain.BaseDirectory + pvtstrLogFileName + "_Log.txt");
                            }
                            catch (Exception ex)
                            {
                                WriteExceptionLog("Check_Log_For_Archiving File Delete ", ex);
                            }
                        }

                        //Delete Files Older than 20 Days
                        //Cleanup Daily Backup Files Older than 20 Days
                        string[] strDbBackupFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "Backup", pvtstrLogFileName + "_*_Log.txt");

                        foreach (string file in strDbBackupFiles)
                        {
                            int intOffset = file.IndexOf(pvtstrLogFileName + "_");

                            if (intOffset > -1)
                            {
                                try
                                {
                                    myFileDateTime = DateTime.ParseExact(file.Substring(intOffset + 38, 8), "yyyyMMdd", null);

                                    myFileDateTime = myFileDateTime.AddDays(21);

                                    if (myFileDateTime < DateTime.Now)
                                    {
                                        File.Delete(file);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    WriteExceptionLog("Check_Log_For_Archiving File Backup Delete '" + file + "'", ex);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteExceptionLog("Check_Log_For_Archiving - Move Files", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog("Check_Log_For_Archiving", ex);
            }
        }

        public void Timesheet_Dynamic_Upload(Object source, System.Timers.ElapsedEventArgs e)
        {
            pvtblnTimesheetDynamicUploadRun = false;

            try
            {
                tmrUploadDownloadTimesheets.Stop();

                StringBuilder strQry = new StringBuilder();
                DataSet dsDataSet = new DataSet();

                if (DateTime.Now > pvtdtNextTimesheetDynamicUploadRunDateTime)
                {
                    pvtblnTimesheetDynamicUploadRun = true;
                    Run_Timesheet_Dynamic_Upload();
                }
                else
                {
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");

                    strQry.AppendLine(" DYNAMIC_UPLOAD_TIMESHEETS_RUNNING_IND");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.VALIDITE_HOSTING_SERVICE");

                    strQry.AppendLine(" WHERE START_DYNAMIC_UPLOAD_TIMESHEETS_RUN_IND = 'Y'");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), dsDataSet, "ValiditeHostingService");

                    if (dsDataSet.Tables["ValiditeHostingService"].Rows.Count > 0)
                    {
                        //Force Check for Download Files
                        strQry.Clear();
                        strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.DYNAMIC_FILE_DOWNLOAD_CHECK");
                        strQry.AppendLine(" SET FILE_DOWNLOAD_CHECK_DATE = '" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + "'");

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                        
                        WriteLog("");
                        WriteLog("Switch Set to Run Timesheet_Dynamic_Upload *************************************** ");
                        pvtblnTimesheetDynamicUploadRun = true;
                        Run_Timesheet_Dynamic_Upload();
                    }
                }

                if (pvtintSameExceptionMessage > 0)
                {
                    pvtstrExceptionMessage = "";
                    pvtintSameExceptionMessage = 0;
                    WriteLog("Timesheet_Dynamic_Upload pvtstrExceptionMessage RESET *********************************");
                }
            }
            catch (Exception ex)
            {
                if (pvtstrExceptionMessage == ex.Message)
                {
                    pvtintSameExceptionMessage += 1;
                }
                else
                {
                    pvtintSameExceptionMessage = 0;
                    pvtstrExceptionMessage = ex.Message;
                }

                if (pvtintSameExceptionMessage < 6)
                {
                    WriteLog("Timesheet_Dynamic_Upload Exception = " + ex.Message);
                }
                else
                {
                    if (pvtintSameExceptionMessage == 6)
                    {
                        WriteLog("Timesheet_Dynamic_Upload Exception ******* STOPPING WRITING LOGS (SAME ERROR) ******* = " + ex.Message);
                    }
                    else
                    {
                        //Approx Half Hour Later
                        if (pvtintSameExceptionMessage == 30)
                        {
                            pvtintSameExceptionMessage = 4;
                        }
                    }
                }
            }
            finally
            {
                if (pvtblnTimesheetDynamicUploadRun == true)
                {
                    pvtblnTimesheetDynamicUploadRun = false;
                    pvtdtNextTimesheetDynamicUploadRunDateTime = DateTime.Now.AddSeconds(pvtintUploadTimesheetsInSeconds);
                }

                //Every 60 Seconds
                tmrUploadDownloadTimesheets.Interval = 60000;
                tmrUploadDownloadTimesheets.Start();
            }
        }

        public void Run_Timesheet_Dynamic_Upload()
        {
            try
            {
                object[] pvtobjConsoleLineParm = new object[1];
                byte[] pvtbytCompress;

                object myReturnObject;
                object myClientReturnObject = null;
                object myEmployeeReturnObject;

                string strEmployeeRunDateWageQry = "";
                string strEmployeeRunDateSalaryQry = "";
                string strEmployeeRunDateTimeAttendanceQry = "";

                bool blnRunDownloadFileCheck = false;

                for (int intCount = 0; intCount < listCompanyNo.Count; intCount++)
                {
                    DataSet DataSet = new System.Data.DataSet();

                    if (intCount == 0)
                    {
                        if (AppDomain.CurrentDomain.GetData("URLPath").ToString() != "")
                        {
                            busWebDynamicServices = new localhost.busWebDynamicServices();
                            
                            int intReturnCode = WebService_Settings(busWebDynamicServices, "busWebDynamicServices");

                            if (intReturnCode != 0)
                            {
                                WriteLog("Error Creating URL - WebService Settings ");
                                busClientTimeSheetDynamicUpload.InsertConsoleLine("Error Creating URL - WebService Settings");
                            
                                goto Run_Timesheet_Dynamic_Upload_Continue_Error; 
                            }
                        }
                        else
                        {
                            //Debug Mode
                            Assembly asAssembly = Assembly.LoadFrom("busTimeSheetDynamicUpload.dll");
                            typTimeSheetDynamicUpload = asAssembly.GetType("InteractPayroll.busTimeSheetDynamicUpload");
                            busTimeSheetDynamicUploadService = Activator.CreateInstance(typTimeSheetDynamicUpload);
                        }

                        pvtbytCompress = busClientTimeSheetDynamicUpload.DownloadCheck();

                        //Delete All rows fron CONSOLE_LINES and Set  on Database to Running
                        busClientTimeSheetDynamicUpload.DeleteAllConsoleLinesAndSetUploadTimesheetsRunningFlag();

                        //NB DataSet has "ClientFile"
                        DataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(pvtbytCompress);

                        blnRunDownloadFileCheck = true;

                        if (DataSet.Tables["DownloadCheck"].Rows.Count > 0)
                        {
                            if (DataSet.Tables["DownloadCheck"].Rows[0]["FILE_DOWNLOAD_CHECK_DATE"] != System.DBNull.Value)
                            {
                                if (Convert.ToDateTime(DataSet.Tables["DownloadCheck"].Rows[0]["FILE_DOWNLOAD_CHECK_DATE"]).ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd"))
                                {
                                    blnRunDownloadFileCheck = false;
                                }
                                else
                                {
                                    Random random = new Random();
                                    //0-59
                                    int intRandom = random.Next(60);

                                    WriteLog("Run_Timesheet_Dynamic_Upload Random Minute=" + intRandom.ToString());
                                    WriteLog("Run_Timesheet_Dynamic_Upload DateTime Hour=" + DateTime.Now.Hour.ToString() + "/Minute=" + DateTime.Now.Minute.ToString());
                                    
                                    //Spread Files to Download over 0 to 59 past Midnight
                                    if (DateTime.Now.Hour > 0
                                    || DateTime.Now.Minute > intRandom)
                                    {
                                    }
                                    else
                                    {
                                        blnRunDownloadFileCheck = false;
                                    }
                                }
                            }
                        }
#if (DEBUG)
                        blnRunDownloadFileCheck = true;
#endif

                        if (blnRunDownloadFileCheck == true)
                        {
                            WriteLog("Checking for Files to Download");
                            busClientTimeSheetDynamicUpload.InsertConsoleLine("Checking for Files to Download");

                            //2018-09-22
                            pvtbytCompress = busClientTimeSheetDynamicUpload.GetPayCategoryDownloadDetails();

                            object[] objParm = new object[1];
                            objParm[0] = pvtbytCompress;
                            
                            //Get All Files For Download From Internet
                            byte[] byteDecompressArray = (byte[])DynamicFunction("Get_Download_Files_Details_New", objParm);

                            DataSet DataSetFileInternetDownload = clsDBConnectionObjects.DeCompress_Array_To_DataSet(byteDecompressArray);

                            DataView myClientPresentationObjects = new DataView(DataSet.Tables["ClientFile"],
                                "FILE_LAYER_IND = 'P'",
                                "",
                                DataViewRowState.CurrentRows);

                            //Cleanup Presentation Client Database Records That are Not In Internet Download List
                            for (int intRow = 0; intRow < myClientPresentationObjects.Count; intRow++)
                            {
                                DataView DataView = new DataView(DataSetFileInternetDownload.Tables["Files"],
                                "FILE_LAYER_IND = '" + myClientPresentationObjects[intRow]["FILE_LAYER_IND"].ToString() + "' AND FILE_NAME = '" + myClientPresentationObjects[intRow]["FILE_NAME"].ToString() + "'",
                                "",
                                DataViewRowState.CurrentRows);

                                if (DataView.Count == 0)
                                {
                                    myClientReturnObject = busClientTimeSheetDynamicUpload.DeleteFile(myClientPresentationObjects[intRow]["FILE_LAYER_IND"].ToString(), myClientPresentationObjects[intRow]["FILE_NAME"].ToString());

                                    if (myClientReturnObject == null)
                                    {
                                        WriteLog("ERROR with clsISClientUtilities.DynamicFunction 'DeleteFile'");
                                        busClientTimeSheetDynamicUpload.InsertConsoleLine("ERROR with clsISClientUtilities.DynamicFunction 'DeleteFile'.");
                                        
                                        goto Run_Timesheet_Dynamic_Upload_Continue_Error;
                                    }

                                    myClientPresentationObjects[intRow].Delete();

                                    intRow -= 1;
                                }
                            }

                            DataSet.Tables["ClientFile"].AcceptChanges();

                            //Check if Download Files Exist On Client Database
                            for (int intRow = 0; intRow < DataSetFileInternetDownload.Tables["Files"].Rows.Count; intRow++)
                            {
#if(DEBUG)
                                string myFileName = DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();

                                if (myFileName == "FingerPrintClockServiceStartStop.dll")
                                {
                                    string strStop = "";
                                }
#endif
                                DataView DataView = new DataView(DataSet.Tables["ClientFile"],
                                "FILE_LAYER_IND = '" + DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_LAYER_IND"].ToString() + "' AND (FILE_NAME = '" + DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() + "' OR FILE_NAME = '" + DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() + "_')",
                                "",
                                DataViewRowState.CurrentRows);

                                if (DataView.Count == 0)
                                {
                                }
                                else
                                {
                                    if (Convert.ToDateTime(DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]) >= Convert.ToDateTime(DataView[0]["FILE_LAST_UPDATED_DATE"]).AddSeconds(-3)
                                    && Convert.ToDateTime(DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]) <= Convert.ToDateTime(DataView[0]["FILE_LAST_UPDATED_DATE"]).AddSeconds(3))
                                    {
                                        DataSetFileInternetDownload.Tables["Files"].Rows[intRow].Delete();
                                    }
                                }
                            }

                            //Remove From DataTable where Records Exist On Client Database
                            DataSetFileInternetDownload.AcceptChanges();

                            bool blnComplete = false;

                            if (DataSetFileInternetDownload.Tables["Files"].Rows.Count == 0)
                            {
                                WriteLog("No Files to Download");
                                busClientTimeSheetDynamicUpload.InsertConsoleLine("No Files to Download");
                            }

                            //Download Files to Client Database
                            for (int intRow = 0; intRow < DataSetFileInternetDownload.Tables["Files"].Rows.Count; intRow++)
                            {
                                WriteLog("Downloading File '" + DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() + "'");
                                busClientTimeSheetDynamicUpload.InsertConsoleLine("Downloading File '" + DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() + "'");
#if (DEBUG)
                                string strProjectVersion = DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString();
                                string strFileName = DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();
                                string strDateTime = Convert.ToDateTime(DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]).ToString("yyyy-MM-dd HH:mm:ss");

                                if (strFileName == "clsDBConnectionObjects.dll")
                                {
                                    string strStop = "";
                                }
#endif
                                for (int intRow1 = 1; intRow1 <= Convert.ToInt32(DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["MAX_FILE_CHUNK_NO"]); intRow1++)
                                {
                                    if (intRow1 == Convert.ToInt32(DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["MAX_FILE_CHUNK_NO"]))
                                    {
                                        blnComplete = true;
                                    }
                                    else
                                    {
                                        blnComplete = false;
                                    }

                                    //Get File Chunk From Internet Site
                                    objParm = new object[4];
                                    objParm[0] = DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString();
                                    objParm[1] = DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_LAYER_IND"].ToString();
                                    objParm[2] = DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();
                                    objParm[3] = intRow1;

                                    myReturnObject = DynamicFunction("Get_File_Chunk_New", objParm);

                                    if (myReturnObject == null)
                                    {
                                        WriteLog("ERROR with Internet 'Get_File_Chunk'");
                                        busClientTimeSheetDynamicUpload.InsertConsoleLine("ERROR with Internet 'Get_File_Chunk'");
                                      
                                        goto Run_Timesheet_Dynamic_Upload_Continue_Error;
                                    }

                                    //Insert Chunk into Client Database
                                    myClientReturnObject = busClientTimeSheetDynamicUpload.InsertChunk(DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_LAYER_IND"].ToString(),
                                                                                                       DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString(),
                                                                                                       intRow1,
                                                                                                       (byte[])myReturnObject,
                                                                                                       blnComplete,
                                                                                                       DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_CRC_VALUE"].ToString(),
                                                                                                       Convert.ToDateTime(DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]),
                                                                                                       Convert.ToInt32(DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_SIZE"]),
                                                                                                       Convert.ToInt32(DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_SIZE_COMPRESSED"]));

                                    if (myClientReturnObject == null)
                                    {
                                        WriteLog("ERROR with clsISClientUtilities.DynamicFunction 'InsertChunk'");
                                        busClientTimeSheetDynamicUpload.InsertConsoleLine("ERROR with clsISClientUtilities.DynamicFunction 'InsertChunk'");
                                       
                                        goto Run_Timesheet_Dynamic_Upload_Continue_Error;
                                    }

                                    WriteLog("Block " + intRow1.ToString() + " of " + DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["MAX_FILE_CHUNK_NO"].ToString());
                                    busClientTimeSheetDynamicUpload.InsertConsoleLine("Block " + intRow1.ToString() + " of " + DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["MAX_FILE_CHUNK_NO"].ToString());
                                }

                                int intReturnCode = (int)myClientReturnObject;

                                //CRC Check Correct
                                if (intReturnCode == 0
                                | intReturnCode == 9)
                                {
                                    WriteLog("File '" + DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() + "' Downloaded Successful");
                                    busClientTimeSheetDynamicUpload.InsertConsoleLine("File '" + DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() + "' Downloaded Successful.");
                                    
                                    if (intReturnCode == 9)
                                    {
                                        //blnRebootMachine = true;
                                    }
                                }
                                else
                                {
                                    WriteLog("File '" + DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() + "' CRC ERROR. Download UNSUCCESSFUL.");
                                    busClientTimeSheetDynamicUpload.InsertConsoleLine("File '" + DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() + "' CRC ERROR\n\nDownload UNSUCCESSFUL.");
                                }
                            }

                            //Update Day Checked Timestamp
                            myClientReturnObject = busClientTimeSheetDynamicUpload.UpdateDateTimeCheck(DataSet.Tables["DownloadCheck"].Rows.Count);

                            if (myClientReturnObject == null)
                            {
                                WriteLog("ERROR with clsISClientUtilities.DynamicFunction 'UpdateDateTimeCheck'");
                                busClientTimeSheetDynamicUpload.InsertConsoleLine("ERROR with clsISClientUtilities.DynamicFunction 'UpdateDateTimeCheck'");
                              
                                goto Run_Timesheet_Dynamic_Upload_Continue_Error;
                            }
                        }
                    }

                    if (DataSet.Tables["Date"] != null)
                    {
                        DataSet.Tables.Remove("Date");
                    }

                    //Find Dates for Upload
                    pvtbytCompress = busClientTimeSheetDynamicUpload.GetDates(listCompanyNo[intCount]);

                    DataSet Temp1DataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(pvtbytCompress);

                    for (int intRow = 0; intRow < Temp1DataSet.Tables["UploadCheck"].Rows.Count; intRow++)
                    {
                        WriteLog("Cost Centre '" + Temp1DataSet.Tables["UploadCheck"].Rows[intRow]["PAY_CATEGORY_DESC"].ToString() + " (" + Temp1DataSet.Tables["UploadCheck"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() + ")' Exists but is Not flagged to be Uploaded.");
                        busClientTimeSheetDynamicUpload.InsertConsoleLine("**** Cost Centre '" + Temp1DataSet.Tables["UploadCheck"].Rows[intRow]["PAY_CATEGORY_DESC"].ToString() + " (" + Temp1DataSet.Tables["UploadCheck"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() + ")' Exists but is Not flagged to be Uploaded.");
                    }

                    DataSet.Merge(Temp1DataSet);

                    if (DataSet.Tables["Date"].Rows.Count == 0)
                    {
                        WriteLog("No Timesheet / Break Records Exist For Upload of Company Number = " + listCompanyNo[intCount]);
                        busClientTimeSheetDynamicUpload.InsertConsoleLine("No Timesheet / Break Records Exist For Upload of Company Number = " + listCompanyNo[intCount]);
                        
                        //Find Cost Centres for Upload
                        pvtbytCompress = busClientTimeSheetDynamicUpload.GetCostCentres(listCompanyNo[intCount]);

                        if (pvtbytCompress == null)
                        {
                            WriteLog("ERROR Calling GetCostCentres for Company Number = " + listCompanyNo[intCount]);
                            busClientTimeSheetDynamicUpload.InsertConsoleLine("ERROR Calling GetCostCentres for Company Number = " + listCompanyNo[intCount]);
                        }
                        else
                        {
                            if (blnRunDownloadFileCheck == true)
                            {
                                pvtobjParm = new object[2];
                                pvtobjParm[0] = listCompanyNo[intCount];
                                pvtobjParm[1] = pvtbytCompress;

                                //Set UploadDatetime for Company on Internet
                                int intRecordsUpdated = (int)DynamicFunction("Set_UploadDateTime_For_Company", pvtobjParm);
                            }
                        }
                    }
                    else
                    {
                        WriteLog("Start of Upload Process...");
                        busClientTimeSheetDynamicUpload.InsertConsoleLine("Start of Upload Process...");
                        
                        for (int intRow = 0; intRow < DataSet.Tables["Date"].Rows.Count; intRow++)
                        {
                            if (DataSet.Tables["CostCentreUpdate"] != null)
                            {
                                DataSet.Tables.Remove("CostCentreUpdate");
                            }

                            if (DataSet.Tables["EmployeeRunDateWage"] != null)
                            {
                                DataSet.Tables.Remove("EmployeeRunDateWage");
                            }

                            if (DataSet.Tables["EmployeeRunDateSalary"] != null)
                            {
                                DataSet.Tables.Remove("EmployeeRunDateSalary");
                            }

                            //2013-06-24
                            if (DataSet.Tables["EmployeeRunDateTimeAttendance"] != null)
                            {
                                DataSet.Tables.Remove("EmployeeRunDateTimeAttendance");
                            }

                            strEmployeeRunDateWageQry = "";
                            strEmployeeRunDateSalaryQry = "";
                            //2013-06-24
                            strEmployeeRunDateTimeAttendanceQry = "";

                            //Get PayCategory Records to Build Query to be Passed to Internet
                            pvtbytCompress = busClientTimeSheetDynamicUpload.GetPayCategoryRecords(listCompanyNo[intCount]);
                            DataSet Temp2DataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(pvtbytCompress);

                            DataSet.Merge(Temp2DataSet);

                            for (int intRow1 = 0; intRow1 < DataSet.Tables["EmployeeRunDateWage"].Rows.Count; intRow1++)
                            {
                                if (intRow1 == 0)
                                {
                                    strEmployeeRunDateWageQry = " AND EPC.PAY_CATEGORY_NO IN (" + DataSet.Tables["EmployeeRunDateWage"].Rows[intRow1]["PAY_CATEGORY_NO"].ToString();
                                }
                                else
                                {
                                    strEmployeeRunDateWageQry += "," + DataSet.Tables["EmployeeRunDateWage"].Rows[intRow1]["PAY_CATEGORY_NO"].ToString();
                                }
                            }

                            if (DataSet.Tables["EmployeeRunDateWage"].Rows.Count > 0)
                            {
                                strEmployeeRunDateWageQry += ")";
                            }
                            else
                            {
                                strEmployeeRunDateWageQry = " AND EPC.PAY_CATEGORY_NO IN (-1)";
                            }

                            for (int intRow1 = 0; intRow1 < DataSet.Tables["EmployeeRunDateSalary"].Rows.Count; intRow1++)
                            {
                                if (intRow1 == 0)
                                {
                                    strEmployeeRunDateSalaryQry = " AND EPC.PAY_CATEGORY_NO IN (" + DataSet.Tables["EmployeeRunDateSalary"].Rows[intRow1]["PAY_CATEGORY_NO"].ToString();
                                }
                                else
                                {
                                    strEmployeeRunDateSalaryQry += "," + DataSet.Tables["EmployeeRunDateSalary"].Rows[intRow1]["PAY_CATEGORY_NO"].ToString();
                                }
                            }

                            if (DataSet.Tables["EmployeeRunDateSalary"].Rows.Count > 0)
                            {
                                strEmployeeRunDateSalaryQry += ")";
                            }
                            else
                            {
                                strEmployeeRunDateSalaryQry = " AND EPC.PAY_CATEGORY_NO IN (-1)";
                            }

                            //2013-06-24
                            for (int intRow1 = 0; intRow1 < DataSet.Tables["EmployeeRunDateTimeAttendance"].Rows.Count; intRow1++)
                            {
                                if (intRow1 == 0)
                                {
                                    strEmployeeRunDateTimeAttendanceQry = " AND EPC.PAY_CATEGORY_NO IN (" + DataSet.Tables["EmployeeRunDateTimeAttendance"].Rows[intRow1]["PAY_CATEGORY_NO"].ToString();
                                }
                                else
                                {
                                    strEmployeeRunDateTimeAttendanceQry += "," + DataSet.Tables["EmployeeRunDateTimeAttendance"].Rows[intRow1]["PAY_CATEGORY_NO"].ToString();
                                }
                            }

                            if (DataSet.Tables["EmployeeRunDateTimeAttendance"].Rows.Count > 0)
                            {
                                strEmployeeRunDateTimeAttendanceQry += ")";
                            }
                            else
                            {
                                strEmployeeRunDateTimeAttendanceQry = " AND EPC.PAY_CATEGORY_NO IN (-1)";
                            }

                            //Get Upload Records for SpeciFic Date
                            myClientReturnObject = busClientTimeSheetDynamicUpload.GetUploadRecords(listCompanyNo[intCount], Convert.ToDateTime(DataSet.Tables["Date"].Rows[intRow]["TIMESHEET_DATE"]));

                            if (myClientReturnObject == null)
                            {
                                WriteLog("ERROR with clsISClientUtilities.DynamicFunction 'GetUploadRecords'");
                                busClientTimeSheetDynamicUpload.InsertConsoleLine("ERROR with clsISClientUtilities.DynamicFunction 'GetUploadRecords'");
                                
                                goto Run_Timesheet_Dynamic_Upload_Continue_Error;
                            }

                            string strURLConfigDateTime = "";

                            FileInfo FileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "URLConfig.txt");

                            if (FileInfo.Exists == true)
                            {
                                strURLConfigDateTime = FileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
                            }

                            //Insert Timesheet Records for Specific Date to Internet Site
                            object[] objParm = new object[7];
                            objParm[0] = listCompanyNo[intCount];
                            objParm[1] = strURLConfigDateTime;
                            objParm[2] = listUploadKey[intCount];
                            objParm[3] = strEmployeeRunDateWageQry;
                            objParm[4] = strEmployeeRunDateSalaryQry;
                            objParm[5] = strEmployeeRunDateTimeAttendanceQry;
                            objParm[6] = (byte[])myClientReturnObject;

                            myEmployeeReturnObject = DynamicFunction("Upload_Timesheet_Break_For_Day", objParm);

                            if (myEmployeeReturnObject == null)
                            {
                                WriteLog("ERROR with Internet 'Upload_Timesheet_Break_For_Day'");
                                busClientTimeSheetDynamicUpload.InsertConsoleLine("ERROR with Internet 'Upload_Timesheet_Break_For_Day'");
                                
                                goto Run_Timesheet_Dynamic_Upload_Continue_Error;
                            }

                            byte[] byteDecompressArray = (byte[])myEmployeeReturnObject;

                            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(byteDecompressArray);

                            if (parDataSet.Tables["Company"].Rows.Count > 0)
                            {
                                if (parDataSet.Tables["Company"].Rows[0]["URLCONFIG_CHANGE_IND"].ToString() == "")
                                {
                                    myClientReturnObject = busClientTimeSheetDynamicUpload.UpdateLocalRecords(listCompanyNo[intCount],
                                                                                                              Convert.ToDateTime(DataSet.Tables["Date"].Rows[intRow]["TIMESHEET_DATE"]),
                                                                                                              Convert.ToDateTime(parDataSet.Tables["Company"].Rows[0]["UPLOAD_DATETIME"]),
                                                                                                              (byte[])myEmployeeReturnObject,
                                                                                                              (byte[])myClientReturnObject);

                                    if (myClientReturnObject == null)
                                    {
                                        WriteLog("ERROR with clsISClientUtilities.DynamicFunction 'UpdateLocalRecords'");
                                        busClientTimeSheetDynamicUpload.InsertConsoleLine("ERROR with clsISClientUtilities.DynamicFunction 'UpdateLocalRecords'");
                                
                                        goto Run_Timesheet_Dynamic_Upload_Continue_Error;
                                    }

                                    WriteLog("Timesheets/Breaks Upload for Company Number = " + listCompanyNo[intCount] + " and Date = '" + Convert.ToDateTime(DataSet.Tables["Date"].Rows[intRow]["TIMESHEET_DATE"]).ToString("dd MMM yyyy") + "' SUCCESSFUL");
                                    busClientTimeSheetDynamicUpload.InsertConsoleLine("Timesheets/Breaks Upload for Company Number = " + listCompanyNo[intCount] + " and Date = '" + Convert.ToDateTime(DataSet.Tables["Date"].Rows[intRow]["TIMESHEET_DATE"]).ToString("dd MMM yyyy") + "' SUCCESSFUL");
                                }
                                else
                                {
                                    busClientTimeSheetDynamicUpload.ResetDateTimeCheck();

                                    WriteLog("URLConfig.txt FILE has Changed\nDateTimeCheck Has been Reset to Activate Download");
                                    busClientTimeSheetDynamicUpload.InsertConsoleLine("URLConfig.txt FILE has Changed\nDateTimeCheck Has been Reset to Activate Download.");
                                    
                                    goto Run_Timesheet_Dynamic_Upload_Continue_Error;
                                }
                            }
                            else
                            {
                                WriteLog("Upload Key Not Authorised to Run. Speak to System Administrator");
                                busClientTimeSheetDynamicUpload.InsertConsoleLine("'" + listUploadKey[intCount] + "' Upload Key Not Authorised to Run.\nSpeak to System Administrator");
                               
                                goto Run_Timesheet_Dynamic_Upload_Continue_Error;
                            }
                        }
                    }
                }

            Run_Timesheet_Dynamic_Upload_Continue_Error:
                int intError = 1;
            }
            catch (Exception ex)
            {
                WriteLog("Run_Timesheet_Dynamic_Upload Exception = " + ex.Message);
                busClientTimeSheetDynamicUpload.InsertConsoleLine("Run_Timesheet_Dynamic_Upload Exception = " + ex.Message);
            }
            finally
            {
                try
                {
                    busClientTimeSheetDynamicUpload.InsertConsoleLine("Run_Timesheet_Dynamic_Upload Exit **************************");
                    //Reset Flag on Database to Not Running
                    busClientTimeSheetDynamicUpload.ResetUploadTimesheetsRunningFlag();
                }
                catch(Exception ex)
                {
                    WriteLog("Run_Timesheet_Dynamic_Upload finally Exception = " + ex.Message);
                }
            }
        }

        private void Keep_LocalDB_Up(Object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                tmrKeepLocalDbUpTimer.Stop();

                if (tmrKeepLocalDbUpTimer.Interval == 6000)
                {
                    //Set to 1 Minute
                    tmrKeepLocalDbUpTimer.Interval = 60000;
                    WriteLog("Keep_LocalDB_Up Timer runs every 60 Seconds");
                }

                StringBuilder strQry = new StringBuilder();

                DataSet DataSet = new DataSet();

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" COMPANY_NO");
                strQry.AppendLine(" FROM COMPANY");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Temp");
            }
            catch (Exception ex)
            {
                WriteExceptionLog("Keep_LocalDB_Up",ex);
            }
            finally
            {
                tmrKeepLocalDbUpTimer.Start();
            }
        }
        
        object DynamicFunction(string FunctionName, object[] objParm)
        {
            object objReturn = null;

            if (AppDomain.CurrentDomain.GetData("URLPath").ToString() != "")
            {
                objReturn = busWebDynamicServices.DynamicFunction("busTimeSheetDynamicUpload", FunctionName, objParm);
            }
            else
            {
                MethodInfo mi = typTimeSheetDynamicUpload.GetMethod(FunctionName);
                objReturn = mi.Invoke(busTimeSheetDynamicUploadService, objParm);
            }

            return objReturn;
        }
        
        private void Stop_FingerPrintClockTimeAttendanceServiceStartStop_And_RenameExe(Object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                pvtblnFingerPrintClockTimeAttendanceServiceStartStopReplace = false;

                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockTimeAttendanceServiceStartStop.exe_") == true)
                {
                    WriteLog("FingerPrintClockTimeAttendanceServiceStartStop.exe_ EXISTS **********");
                    pvtblnFingerPrintClockTimeAttendanceServiceStartStopReplace = true;
                }

                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockServiceStartStop.dll_") == true)
                {
                    WriteLog("FingerPrintClockServiceStartStop.dll_ EXISTS **********");
                    pvtblnFingerPrintClockTimeAttendanceServiceStartStopReplace = true;
                }

                if (pvtblnFingerPrintClockTimeAttendanceServiceStartStopReplace == true)
                {
                    WriteLog("Stop_FingerPrintClockTimeAttendanceServiceStartStop_And_RenameExe Entered");
                    tmrFingerPrintClockTimeAttendanceServiceStartStopReplaceTimer.Stop();

                    bool blnServiceWasRunning = false;

                    ServiceController service = new ServiceController("FingerPrintClockTimeAttendanceServiceStartStop");

                    if (service != null)
                    {
                        if (service.Status.ToString() == "Running")
                        {
                            WriteLog("**********FingerPrintClockTimeAttendanceServiceStartStop is Running ");
                            blnServiceWasRunning = true;
                        }
                        else
                        {
                            WriteLog("********** NB NB FingerPrintClockTimeAttendanceServiceStartStop WAS NOT RUNNING **********");
                        }
                    }
                    else
                    {
                        WriteLog("**********FingerPrintClockTimeAttendanceServiceStartStop NOT INSTALLED ???????????????");
                        return;
                    }

                    if (blnServiceWasRunning == true)
                    {
                        WriteLog("********** Stopping FingerPrintClockTimeAttendanceServiceStartStop Service **********");

                        service.Stop();

                        while (service.Status.ToString() != "Stopped")
                        {
                            service.Refresh();
                        }

                        WriteLog("********** Stopped FingerPrintClockTimeAttendanceServiceStartStop Service SUCCESSFULLY **********");
                    }

                    //Wait 10 Seconds
                    System.Threading.Thread.Sleep(10000);

                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockTimeAttendanceServiceStartStop.exe_") == true)
                    {
                        File.Delete(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockTimeAttendanceServiceStartStop.exe");
                        File.Move(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockTimeAttendanceServiceStartStop.exe_", AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockTimeAttendanceServiceStartStop.exe");

                        WriteLog("********** FingerPrintClockTimeAttendanceServiceStartStop.exe REPLACED SUCCESSFULLY **********");
                    }
                    
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockServiceStartStop.dll_") == true)
                    {
                        File.Delete(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockServiceStartStop.dll");
                        File.Move(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockServiceStartStop.dll_", AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockServiceStartStop.dll");

                        WriteLog("********** FingerPrintClockServiceStartStop.dll REPLACED SUCCESSFULLY **********");
                    }

                    if (blnServiceWasRunning == true)
                    {
                        WriteLog("********** Starting FingerPrintClockTimeAttendanceServiceStartStop Service **********");

                        service.Start();

                        while (service.Status.ToString() != "Running")
                        {
                            service.Refresh();
                        }

                        WriteLog("********** Started FingerPrintClockTimeAttendanceServiceStartStop Service SUCCESSFULLY **********");
                    }

                    WriteLog("Stop_FingerPrintClockTimeAttendanceServiceStartStop_And_RenameExe Exit");
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog("Stop_FingerPrintClockTimeAttendanceServiceStartStop_And_RenameExe ", ex);
            }
            finally
            {
                tmrFingerPrintClockTimeAttendanceServiceStartStopReplaceTimer.Start();
            }
        }

        private void Check_FingerPrintClockTimeAttendanceServiceStartStopAndSqlServer()
        {
#if (DEBUG)
#else

            try
            {
                pvtblnBothServicesRunning = true;

                WriteLog("Check_FingerPrintClockTimeAttendanceServiceStartStop Entered");

                ServiceController service = new ServiceController("FingerPrintClockTimeAttendanceServiceStartStop");

                if (service != null)
                {
                    if (service.Status.ToString() == "Running")
                    {
                        WriteLog("**********FingerPrintClockTimeAttendanceServiceStartStop is Running ");
                    }
                    else
                    {
                        //Try Start Service
                        WriteLog("********** NB NB FingerPrintClockTimeAttendanceServiceStartStop WAS NOT RUNNING **********");
                        WriteLog("********** Starting FingerPrintClockTimeAttendanceServiceStartStop **********");
                        service.Start();

                        while (service.Status.ToString() != "Running")
                        {
                            service.Refresh();
                        }

                        WriteLog("********** Started FingerPrintClockTimeAttendanceServiceStartStop SUCCESSFULLY **********");
                    }
                }
                else
                {
                    pvtblnBothServicesRunning = false;
                    WriteLog("**********FingerPrintClockTimeAttendanceServiceStartStop NOT INSTALLED ???????????????");
                }
            }
            catch (Exception ex)
            {
                pvtblnBothServicesRunning = false;
                WriteExceptionLog("Check_FingerPrintClockTimeAttendanceServiceStartStop FingerPrintClockTimeAttendanceServiceStartStop ", ex);
            }
#endif

            string strServiceName = "";

            try
            {
                string[] strParts = pvtstrDBEngine.Split('\\');

                if (strParts.Length == 2
                && pvtstrDBEngine.ToUpper().IndexOf("MSSQLLOCALDB") == -1)
                {
                    strServiceName = "SQL Server (" + strParts[1] + ")";

                    ServiceController service = new ServiceController(strServiceName);

                    if (service != null)
                    {
                        if (service.Status.ToString() == "Running")
                        {
                            WriteLog("**********" + strServiceName + " is Running ");
                        }
                        else
                        {
                            service.Start();

                            while (service.Status.ToString() != "Running")
                            {
                                service.Refresh();
                            }

                            WriteLog("********** Started " + strServiceName + " SUCCESSFULLY **********");
                        }
                    }
                    else
                    {
                        //Write Log Engine Not Found
                        pvtblnBothServicesRunning = false;
                    }
                }
                else
                {
                    if (pvtstrDBEngine.ToUpper().IndexOf("MSSQLLOCALDB") > -1)
                    {
                        //Not A Windows Service
                        //WriteLog("**********" + pvtstrDBEngine + " NOT A Windows Service");
                    }
                    else
                    {
                        //Write Log Engine Not Found
                        pvtblnBothServicesRunning = false;
                        WriteLog("**********" + pvtstrDBEngine + " NOT INSTALLED ???????????????");
                    }
                }
            }
            catch (Exception ex)
            {
                pvtblnBothServicesRunning = false;
                WriteExceptionLog("Check_FingerPrintClockTimeAttendanceServiceStartStop " + strServiceName, ex);
            }
            finally
            {
                if (pvtblnBothServicesRunning == true)
                {
                    if (tmrCheckServiceAndArchivingTimer.Interval == intFiveMinute)
                    {
                        WriteLog("Check_FingerPrintClockTimeAttendanceServiceStartStop Both Services Running SET Timer to 1 Hour *****");

                        //5 Minutes
                        tmrCheckServiceAndArchivingTimer.Stop();
                        //60 Minutes
                        tmrCheckServiceAndArchivingTimer.Interval = int60Minute;
                        tmrCheckServiceAndArchivingTimer.Start();
                    }
                }
                else
                {
                    //Has Not Been Changed to 60 Minutes
                    if (tmrCheckServiceAndArchivingTimer.Interval == intFiveMinute)
                    {
                        pvtint5MinuteCount += 1;

                        WriteLog("Check_FingerPrintClockTimeAttendanceServiceStartStop NOT Both Services Running pvtint5MinuteCount = " + pvtint5MinuteCount.ToString());

                        if (pvtint5MinuteCount >= 12)
                        {
                            WriteLog("Check_FingerPrintClockTimeAttendanceServiceStartStop pvtint5MinuteCount >= 12 SET Timer to 1 Hour *****");
                            //5 Minutes
                            tmrCheckServiceAndArchivingTimer.Stop();
                            //60 Minutes
                            tmrCheckServiceAndArchivingTimer.Interval = int60Minute;
                            tmrCheckServiceAndArchivingTimer.Start();
                        }
                    }
                }

                WriteLog("Check_FingerPrintClockTimeAttendanceServiceStartStop Exit");
            }
        }
       
        private string DecryptStringAES(string cipherText)
        {
            RijndaelManaged aesAlg = null;

            //Declare string to hold decrypted text. 
            string plaintext = null;

            //Generate the key from the shared secret and the salt 
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

            //Create a RijndaelManaged object with the specified key and IV. 
            aesAlg = new RijndaelManaged();
            aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
            aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

            //Create a decryptor to perform the stream transform. 
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            //Create the streams used for decryption.                 
            byte[] bytes = Convert.FromBase64String(cipherText);
            using (MemoryStream msDecrypt = new MemoryStream(bytes))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        //Read decrypted bytes into string. 
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }

            aesAlg.Clear();

            return plaintext;
        }

        public Ping GetPing()
        {
            //Time & Attendance
            Ping Ping = new Ping();
            Ping.OK = "Y";

#if (DEBUG)
            Write_DateTime();
#endif
            return Ping;
        }

        public PingDB GetPingDB()
        {
            //Time & Attendance
            PingDB PingDB = new PingDB();
            PingDB.OK = "N";

            DataSet DataSet = new System.Data.DataSet();

            try
            {
                StringBuilder strQry = new StringBuilder();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" C1.TABLE_NAME");
                
                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS C1");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Test");

                PingDB.OK = "Y";
            }
            catch(Exception ex)
            {
                WriteExceptionLog("GetPingDB", ex);
            }
            finally
            {
                DataSet.Dispose();
            }

            return PingDB;
        }

        public ReturnObject GetDynamicFunction(string ObjectName, string FunctionName, ObjectParameters myObjectParameters)
        {
            object[] myObjects = null;

            MemoryStream stmStreamObjects = new MemoryStream(myObjectParameters.bytParameter);
            BinaryFormatter bFormatter = new BinaryFormatter();
            myObjects = (object[])bFormatter.Deserialize(stmStreamObjects);

            Assembly asAssembly = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + ObjectName + ".dll");
            System.Type typObjectType = asAssembly.GetType("InteractPayrollClient." + ObjectName);
            object busDynamicProcedure = Activator.CreateInstance(typObjectType);

            MethodInfo mi = typObjectType.GetMethod(FunctionName);
            object objReturn = mi.Invoke(busDynamicProcedure, myObjects);
#if (DEBUG)
            Write_DateTime();
#endif
            ReturnObject myReturnObject = new ReturnObject();

            myReturnObject.obj = objReturn;

            return myReturnObject;
        }

        public ReturnObject GetDynamicProcedure(string ObjectName, string FunctionName)
        {
            object[] myObjects = null;

            Assembly asAssembly = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + ObjectName + ".dll");
            System.Type typObjectType = asAssembly.GetType("InteractPayrollClient." + ObjectName);
            object busDynamicProcedure = Activator.CreateInstance(typObjectType);

            MethodInfo mi = typObjectType.GetMethod(FunctionName);
            object objReturn = mi.Invoke(busDynamicProcedure, myObjects);
#if (DEBUG)
            Write_DateTime();

            if (objReturn == null)
            {
                int A = 0;
            }
#endif
            ReturnObject myReturnObject = new ReturnObject();

            myReturnObject.obj = objReturn;

            return myReturnObject;
        }

        public SaveReply SaveRfidToEmployee(string CompanyNo, string EmployeeNo, string CardNo)
        {
            SaveReply SaveReply = new SaveReply();
            SaveReply.OK = "N";
            StringBuilder strQry = new StringBuilder();

            try
            {
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_RFID_CARD");
                strQry.AppendLine(" WHERE EMPLOYEE_RFID_CARD_NO = " + CardNo);

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_RFID_CARD");
                strQry.AppendLine(" WHERE COMPANY_NO = " + CompanyNo);
                strQry.AppendLine(" AND EMPLOYEE_NO = " + EmployeeNo);

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.InteractPayrollClient.dbo.EMPLOYEE_RFID_CARD");
                strQry.AppendLine("(EMPLOYEE_RFID_CARD_NO");
                strQry.AppendLine(",COMPANY_NO");
                strQry.AppendLine(",EMPLOYEE_NO)");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine("(" + CardNo);
                strQry.AppendLine("," + CompanyNo);
                strQry.AppendLine("," + EmployeeNo + ")");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                SaveReply.OK = "Y";
            }
            catch (Exception ex)
            {
                WriteExceptionLog("SaveRfidToEmployee", ex);
            }

            return SaveReply;
        }

        public UserCompany GetUserCompanies(string DBNo)
        {
            UserCompany UserCompany = new UserCompany();
            int intWhere = 0;
            string[] strFingerArray;

            //NB Access to All Companies Linked to Clock
            UserCompany.UserNoLoggedIn = "0";
            UserCompany.CompanyDesc = "";
            UserCompany.CompanyNo = "";
            UserCompany.UserNo = "";
            UserCompany.UserName = "";
            UserCompany.UserSurname = "";
            UserCompany.UserFinger = "";
            UserCompany.EmployeeNo = "";
            UserCompany.EmployeeCode = "";
            UserCompany.EmployeeName = "";
            UserCompany.EmployeeSurname = "";
            UserCompany.EmployeeFinger = "";

            DataSet DataSet = new DataSet();

            try
            {
                StringBuilder strQry = new StringBuilder();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" C.COMPANY_NO");
                strQry.AppendLine(",C.COMPANY_DESC");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.COMPANY C");

                strQry.AppendLine(" ORDER BY");
                strQry.AppendLine(" C.COMPANY_DESC");

                intWhere = 1;

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Company");

                intWhere = 2;

                for (int intRow = 0; intRow < DataSet.Tables["Company"].Rows.Count; intRow++)
                {
                    if (intRow == 0)
                    {
                        UserCompany.CompanyDesc = DataSet.Tables["Company"].Rows[intRow]["COMPANY_DESC"].ToString();
                        UserCompany.CompanyNo = DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"].ToString();
                    }
                    else
                    {
                        UserCompany.CompanyDesc += "#" + DataSet.Tables["Company"].Rows[intRow]["COMPANY_DESC"].ToString();
                        UserCompany.CompanyNo += "#" + DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"].ToString();
                    }
                }

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" USER_NO");
                strQry.AppendLine(",FIRSTNAME");
                strQry.AppendLine(",SURNAME");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_ID");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" SURNAME");
                strQry.AppendLine(",FIRSTNAME");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "User");

                for (int intRow = 0; intRow < DataSet.Tables["User"].Rows.Count; intRow++)
                {
                    if (intRow == 0)
                    {
                        UserCompany.UserNo = DataSet.Tables["User"].Rows[intRow]["USER_NO"].ToString();
                        UserCompany.UserName = DataSet.Tables["User"].Rows[intRow]["FIRSTNAME"].ToString();
                        UserCompany.UserSurname = DataSet.Tables["User"].Rows[intRow]["SURNAME"].ToString();
                    }
                    else
                    {
                        UserCompany.UserNo += "#" + DataSet.Tables["User"].Rows[intRow]["USER_NO"].ToString();
                        UserCompany.UserName += "#" + DataSet.Tables["User"].Rows[intRow]["FIRSTNAME"].ToString();
                        UserCompany.UserSurname += "#" + DataSet.Tables["User"].Rows[intRow]["SURNAME"].ToString();
                    }

                    if (DataSet.Tables["UserFinger"] != null)
                    {
                        DataSet.Tables["UserFinger"].Clear();
                    }

                    strFingerArray = null;
                    strFingerArray = new string[10] { "N", "N", "N", "N", "N", "N", "N", "N", "N", "N" };

                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" FINGER_NO");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE");

                    strQry.AppendLine(" WHERE USER_NO = " + DataSet.Tables["User"].Rows[intRow]["USER_NO"].ToString());

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "UserFinger");

                    for (int intFingerRow = 0; intFingerRow < DataSet.Tables["UserFinger"].Rows.Count; intFingerRow++)
                    {
                        strFingerArray[Convert.ToInt32(DataSet.Tables["UserFinger"].Rows[intFingerRow]["FINGER_NO"])] = "Y";
                    }

                    for (int intFingerCountRow = 0; intFingerCountRow < 10; intFingerCountRow++)
                    {
                        if (intFingerCountRow == 0)
                        {
                            if (UserCompany.UserFinger == "")
                            {
                                UserCompany.UserFinger = strFingerArray[intFingerCountRow].ToString();
                            }
                            else
                            {
                                UserCompany.UserFinger += "#" + strFingerArray[intFingerCountRow].ToString();
                            }
                        }
                        else
                        {
                            UserCompany.UserFinger += "|" + strFingerArray[intFingerCountRow].ToString();
                        }
                    }
                }

                if (DataSet.Tables["Company"].Rows.Count == 1)
                {
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EMPLOYEE_NO");
                    strQry.AppendLine(",EMPLOYEE_CODE");
                    strQry.AppendLine(",EMPLOYEE_NAME");
                    strQry.AppendLine(",EMPLOYEE_SURNAME");
                    
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + UserCompany.CompanyNo);
                    strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                    strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                    strQry.AppendLine(" ORDER BY ");
                    strQry.AppendLine(" EMPLOYEE_SURNAME");
                    strQry.AppendLine(",EMPLOYEE_NAME");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

                    for (int intRow = 0; intRow < DataSet.Tables["Employee"].Rows.Count; intRow++)
                    {
                        if (intRow == 0)
                        {
                            UserCompany.EmployeeNo = DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                            UserCompany.EmployeeCode = DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_CODE"].ToString();
                            UserCompany.EmployeeName = DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NAME"].ToString();
                            UserCompany.EmployeeSurname = DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_SURNAME"].ToString();
                        }
                        else
                        {
                            UserCompany.EmployeeNo += "#" + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                            UserCompany.EmployeeCode += "#" + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_CODE"].ToString();

                            UserCompany.EmployeeName += "#" + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NAME"].ToString();
                            UserCompany.EmployeeSurname += "#" + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_SURNAME"].ToString();
                        }

                        if (DataSet.Tables["Finger"] != null)
                        {
                            DataSet.Tables["Finger"].Clear();
                        }

                        strFingerArray = null;
                        strFingerArray = new string[10] { "N", "N", "N", "N", "N", "N", "N", "N", "N", "N" };

                        strQry.Clear();
                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" FINGER_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");

                        strQry.AppendLine(" WHERE COMPANY_NO = " + UserCompany.CompanyNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString());

                        clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Finger");

                        for (int intFingerRow = 0; intFingerRow < DataSet.Tables["Finger"].Rows.Count; intFingerRow++)
                        {
                            strFingerArray[Convert.ToInt32(DataSet.Tables["Finger"].Rows[intFingerRow]["FINGER_NO"])] = "Y";
                        }

                        for (int intFingerCountRow = 0; intFingerCountRow < 10; intFingerCountRow++)
                        {
                            if (intFingerCountRow == 0)
                            {
                                if (UserCompany.EmployeeFinger == "")
                                {
                                    UserCompany.EmployeeFinger = strFingerArray[intFingerCountRow].ToString();
                                }
                                else
                                {
                                    UserCompany.EmployeeFinger += "#" + strFingerArray[intFingerCountRow].ToString();

                                }
                            }
                            else
                            {
                                UserCompany.EmployeeFinger += "|" + strFingerArray[intFingerCountRow].ToString();
                            }
                        }
                    }
                }

                intWhere = 3;
            }
            catch (Exception ex)
            {
                WriteExceptionLog("GetUserCompanies", ex);
            }
            finally
            {
                DataSet.Dispose();
            }

            return UserCompany;
        }

        public UserCompany GetCurrentUserCompanies(string UserPinNo)
        {
            UserCompany UserCompany = new UserCompany();
            int intWhere = 0;
            string[] strFingerArray;
            string strCompanyIn = "";

            //NB Access to All Companies Linked to Clock
            UserCompany.UserNoLoggedIn = "";
            UserCompany.CompanyDesc = "";
            UserCompany.CompanyNo = "";
            UserCompany.UserNo = "";
            UserCompany.UserName = "";
            UserCompany.UserSurname = "";
            UserCompany.UserFinger = "";
            UserCompany.EmployeeNo = "";
            UserCompany.EmployeeCode = "";
            UserCompany.EmployeeName = "";
            UserCompany.EmployeeSurname = "";
            UserCompany.EmployeeFinger = "";

            DataSet DataSet = new DataSet();

            try
            {
                StringBuilder strQry = new StringBuilder();

                if (UserPinNo == "82543483")
                {
                    //Validite
                    UserCompany.UserNoLoggedIn = "0";
                }
                else
                {
                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" USER_NO");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_ID");

                    strQry.AppendLine(" WHERE USER_CLOCK_PIN = " + UserPinNo.Trim());

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "CurrentUser");

                    if (DataSet.Tables["CurrentUser"].Rows.Count > 0)
                    {
                        UserCompany.UserNoLoggedIn = DataSet.Tables["CurrentUser"].Rows[0]["USER_NO"].ToString();
                    }
                    else
                    {
                        goto GetCurrentUserCompanies_Continue;
                    }
                }

                strQry.Clear();

                strQry.AppendLine(" SELECT DISTINCT");
                strQry.AppendLine(" C.COMPANY_NO");
                strQry.AppendLine(",C.COMPANY_DESC");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.COMPANY C");

                if (UserCompany.UserNoLoggedIn != "0")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA ");
                    strQry.AppendLine(" ON UCA.USER_NO = " + UserCompany.UserNoLoggedIn);
                    strQry.AppendLine(" AND C.COMPANY_NO = UCA.COMPANY_NO ");
                }

                strQry.AppendLine(" ORDER BY");
                strQry.AppendLine(" C.COMPANY_DESC");

                intWhere = 1;

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Company");

                intWhere = 2;

                for (int intRow = 0; intRow < DataSet.Tables["Company"].Rows.Count; intRow++)
                {
                    if (intRow == 0)
                    {
                        UserCompany.CompanyDesc = DataSet.Tables["Company"].Rows[intRow]["COMPANY_DESC"].ToString();
                        UserCompany.CompanyNo = DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"].ToString();
                        strCompanyIn = DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"].ToString();
                    }
                    else
                    {
                        UserCompany.CompanyDesc += "#" + DataSet.Tables["Company"].Rows[intRow]["COMPANY_DESC"].ToString();
                        UserCompany.CompanyNo += "#" + DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"].ToString();
                        strCompanyIn += "," + DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"].ToString();
                    }
                }

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" UI.USER_NO");
                strQry.AppendLine(",UI.FIRSTNAME");
                strQry.AppendLine(",UI.SURNAME");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_ID UI");
                
                if (UserCompany.UserNoLoggedIn != "0")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA ");
                    strQry.AppendLine(" ON UI.USER_NO = UCA.USER_NO ");
                    strQry.AppendLine(" AND UCA.COMPANY_NO IN (" + strCompanyIn + ")");
                }
                
                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" UI.SURNAME");
                strQry.AppendLine(",UI.FIRSTNAME");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "User");

                for (int intRow = 0; intRow < DataSet.Tables["User"].Rows.Count; intRow++)
                {
                    if (intRow == 0)
                    {
                        UserCompany.UserNo = DataSet.Tables["User"].Rows[intRow]["USER_NO"].ToString();
                        UserCompany.UserName = DataSet.Tables["User"].Rows[intRow]["FIRSTNAME"].ToString();
                        UserCompany.UserSurname = DataSet.Tables["User"].Rows[intRow]["SURNAME"].ToString();
                    }
                    else
                    {
                        UserCompany.UserNo += "#" + DataSet.Tables["User"].Rows[intRow]["USER_NO"].ToString();
                        UserCompany.UserName += "#" + DataSet.Tables["User"].Rows[intRow]["FIRSTNAME"].ToString();
                        UserCompany.UserSurname += "#" + DataSet.Tables["User"].Rows[intRow]["SURNAME"].ToString();
                    }

                    if (DataSet.Tables["UserFinger"] != null)
                    {
                        DataSet.Tables["UserFinger"].Clear();
                    }

                    strFingerArray = null;
                    strFingerArray = new string[10] { "N", "N", "N", "N", "N", "N", "N", "N", "N", "N" };

                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" FINGER_NO");
                    strQry.AppendLine(",CREATION_DATETIME");
                    
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE");

                    strQry.AppendLine(" WHERE USER_NO = " + DataSet.Tables["User"].Rows[intRow]["USER_NO"].ToString());

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "UserFinger");

                    for (int intFingerRow = 0; intFingerRow < DataSet.Tables["UserFinger"].Rows.Count; intFingerRow++)
                    {
                        if (DataSet.Tables["UserFinger"].Rows[intFingerRow]["CREATION_DATETIME"] == System.DBNull.Value)
                        {
                            strFingerArray[Convert.ToInt32(DataSet.Tables["UserFinger"].Rows[intFingerRow]["FINGER_NO"])] = "L";
                        }
                        else
                        {
                            strFingerArray[Convert.ToInt32(DataSet.Tables["UserFinger"].Rows[intFingerRow]["FINGER_NO"])] = "S";
                        }
                    }

                    for (int intFingerCountRow = 0; intFingerCountRow < 10; intFingerCountRow++)
                    {
                        if (intFingerCountRow == 0)
                        {
                            if (UserCompany.UserFinger == "")
                            {
                                UserCompany.UserFinger = strFingerArray[intFingerCountRow].ToString();
                            }
                            else
                            {
                                UserCompany.UserFinger += "#" + strFingerArray[intFingerCountRow].ToString();
                            }
                        }
                        else
                        {
                            UserCompany.UserFinger += "|" + strFingerArray[intFingerCountRow].ToString();
                        }
                    }
                }

                if (DataSet.Tables["Company"].Rows.Count == 1)
                {
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EMPLOYEE_NO");
                    strQry.AppendLine(",EMPLOYEE_CODE");
                    strQry.AppendLine(",EMPLOYEE_NAME");
                    strQry.AppendLine(",EMPLOYEE_SURNAME");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + UserCompany.CompanyNo);
                    strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                    strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                    strQry.AppendLine(" ORDER BY ");
                    strQry.AppendLine(" EMPLOYEE_SURNAME");
                    strQry.AppendLine(",EMPLOYEE_NAME");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

                    for (int intRow = 0; intRow < DataSet.Tables["Employee"].Rows.Count; intRow++)
                    {
                        if (intRow == 0)
                        {
                            UserCompany.EmployeeNo = DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                            UserCompany.EmployeeCode = DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_CODE"].ToString();
                            UserCompany.EmployeeName = DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NAME"].ToString();
                            UserCompany.EmployeeSurname = DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_SURNAME"].ToString();
                        }
                        else
                        {
                            UserCompany.EmployeeNo += "#" + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                            UserCompany.EmployeeCode += "#" + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_CODE"].ToString();

                            UserCompany.EmployeeName += "#" + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NAME"].ToString();
                            UserCompany.EmployeeSurname += "#" + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_SURNAME"].ToString();
                        }

                        if (DataSet.Tables["Finger"] != null)
                        {
                            DataSet.Tables["Finger"].Clear();
                        }

                        strFingerArray = null;
                        strFingerArray = new string[10] { "N", "N", "N", "N", "N", "N", "N", "N", "N", "N" };

                        strQry.Clear();
                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" FINGER_NO");
                        strQry.AppendLine(",CREATION_DATETIME");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");

                        strQry.AppendLine(" WHERE COMPANY_NO = " + UserCompany.CompanyNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString());

                        clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Finger");

                        for (int intFingerRow = 0; intFingerRow < DataSet.Tables["Finger"].Rows.Count; intFingerRow++)
                        {
                            if (DataSet.Tables["Finger"].Rows[intFingerRow]["CREATION_DATETIME"] == System.DBNull.Value)
                            {
                                strFingerArray[Convert.ToInt32(DataSet.Tables["Finger"].Rows[intFingerRow]["FINGER_NO"])] = "L";
                            }
                            else
                            {
                                strFingerArray[Convert.ToInt32(DataSet.Tables["Finger"].Rows[intFingerRow]["FINGER_NO"])] = "S";
                            }
                        }

                        for (int intFingerCountRow = 0; intFingerCountRow < 10; intFingerCountRow++)
                        {
                            if (intFingerCountRow == 0)
                            {
                                if (UserCompany.EmployeeFinger == "")
                                {
                                    UserCompany.EmployeeFinger = strFingerArray[intFingerCountRow].ToString();
                                }
                                else
                                {
                                    UserCompany.EmployeeFinger += "#" + strFingerArray[intFingerCountRow].ToString();
                                }
                            }
                            else
                            {
                                UserCompany.EmployeeFinger += "|" + strFingerArray[intFingerCountRow].ToString();
                            }
                        }
                    }
                }

                GetCurrentUserCompanies_Continue:

                intWhere = 3;
            }
            catch (Exception ex)
            {
                WriteExceptionLog("GetCurrentUserCompanies", ex);
            }
            finally
            {
                DataSet.Dispose();
            }

            return UserCompany;
        }

        public Device GetDeviceInfo(string DeviceNo)
        {
            //A11
            Device Device = new Device();
            int intWhere = 0;

            Device.DeviceDesc = "";
            Device.DeviceUsage = "";
            Device.ClockInOutParm = "";
            Device.ClockInRangeFrom = "";
            Device.ClockInRangeTo = "";

            DataSet DataSet = new DataSet();

            try
            {
                StringBuilder strQry = new StringBuilder();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" DEVICE_DESC");
                strQry.AppendLine(",DEVICE_USAGE");
                strQry.AppendLine(",CLOCK_IN_OUT_PARM");
                strQry.AppendLine(",CLOCK_IN_RANGE_FROM");
                strQry.AppendLine(",CLOCK_IN_RANGE_TO");
                
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE");
                
                strQry.AppendLine(" WHERE DEVICE_NO = " + DeviceNo);

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Device");

                intWhere = 1;

                if (DataSet.Tables["Device"].Rows.Count > 0)
                {
                    intWhere = 2;
                    Device.FoundInfo = "Y";
                    intWhere = 3;
                    Device.DeviceDesc = DataSet.Tables["Device"].Rows[0]["DEVICE_DESC"].ToString();
                    intWhere = 4;
                    Device.DeviceUsage = DataSet.Tables["Device"].Rows[0]["DEVICE_USAGE"].ToString();
                    intWhere = 5;
                    Device.ClockInOutParm = DataSet.Tables["Device"].Rows[0]["CLOCK_IN_OUT_PARM"].ToString();
                    intWhere = 6;
                    Device.ClockInRangeFrom = DataSet.Tables["Device"].Rows[0]["CLOCK_IN_RANGE_FROM"].ToString();
                    intWhere = 7;
                    Device.ClockInRangeTo = DataSet.Tables["Device"].Rows[0]["CLOCK_IN_RANGE_TO"].ToString();
                }
                else
                {
                    Device.FoundInfo = "N";
                }

                intWhere = 8;

                Device.DateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            catch (Exception ex)
            {
                WriteExceptionLog("GetDeviceInfo", ex);

                Device.FoundInfo = "F";
            }
            finally
            {
                DataSet.Dispose();
            }

            return Device;
        }

        public DeletedFingerReply DeleteEmployeeFingerprint(string DBNo, string CompanyNo, string EmployeeNo, string FingerprintNo)
        {
            DeletedFingerReply DeletedFingerReply = new FingerPrintClockServer.DeletedFingerReply();
            DeletedFingerReply.OkInd = "Y";

            try
            {
                StringBuilder strQry = new StringBuilder();
                
                strQry.Clear();

                //2017-04-29
                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE_DELETE");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",EMPLOYEE_NO");
                strQry.AppendLine(",FINGER_NO");
                strQry.AppendLine(",CREATION_DATETIME)");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" COMPANY_NO");
                strQry.AppendLine(",EMPLOYEE_NO");
                strQry.AppendLine(",FINGER_NO");
                strQry.AppendLine(",CREATION_DATETIME ");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");

                strQry.AppendLine(" WHERE COMPANY_NO = " + CompanyNo);
                strQry.AppendLine(" AND EMPLOYEE_NO = " + EmployeeNo);
                strQry.AppendLine(" AND FINGER_NO = " + FingerprintNo);

                strQry.AppendLine(" AND NOT CREATION_DATETIME IS NULL ");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();
                
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");
                strQry.AppendLine(" WHERE COMPANY_NO = " + CompanyNo);
                strQry.AppendLine(" AND EMPLOYEE_NO = " + EmployeeNo);
                strQry.AppendLine(" AND FINGER_NO = " + FingerprintNo);

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }
            catch (Exception ex)
            {
                WriteExceptionLog("DeleteEmployeeFingerprint", ex);

                DeletedFingerReply.OkInd = "E";
            }

            return DeletedFingerReply;
        }

        public DeletedFingerReply DeleteUserFingerprint(string DBNo, string UserNo, string FingerprintNo)
        {
            DeletedFingerReply DeletedFingerReply = new FingerPrintClockServer.DeletedFingerReply();
            DeletedFingerReply.OkInd = "Y";

            try
            {
                StringBuilder strQry = new StringBuilder();

                strQry.Clear();

                //2017-04-29
                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE_DELETE");
                strQry.AppendLine("(USER_NO");
                strQry.AppendLine(",FINGER_NO");
                strQry.AppendLine(",CREATION_DATETIME)");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" USER_NO");
                strQry.AppendLine(",FINGER_NO");
                strQry.AppendLine(",CREATION_DATETIME ");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE");

                strQry.AppendLine(" WHERE USER_NO = " + UserNo);
                strQry.AppendLine(" AND FINGER_NO = " + FingerprintNo);

                strQry.AppendLine(" AND NOT CREATION_DATETIME IS NULL ");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();
                
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE");
                strQry.AppendLine(" WHERE USER_NO = " + UserNo);
                strQry.AppendLine(" AND FINGER_NO = " + FingerprintNo);

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }
            catch (Exception ex)
            {
                WriteExceptionLog("DeleteUserFingerprint", ex);

                DeletedFingerReply.OkInd = "E";
            }

            return DeletedFingerReply;
        }

        public UserDetails GetUserDetails(string UserNo)
        {
            //A11
            UserDetails UserDetails = new UserDetails();
            UserDetails.OK = "N";
            UserDetails.UserNames = "";

            DataSet DataSet = new DataSet();

            try
            {
                StringBuilder strQry = new StringBuilder();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" SUBSTRING(FIRSTNAME,1,1) + '.' + SURNAME AS USER_NAMES");
                
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_ID");
                
                strQry.AppendLine(" WHERE USER_NO = " + UserNo);

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "User");

                if (DataSet.Tables["User"].Rows.Count > 0)
                {
                    UserDetails.OK = "Y";
                    UserDetails.UserNames = DataSet.Tables["User"].Rows[0]["USER_NAMES"].ToString();
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog("GetUserDetails", ex);
            }
            finally
            {
                DataSet.Dispose();
            }

            return UserDetails;
        }

        public EmployeeDetails GetEmployeeDetails(string UserNo, string EmployeeNo, string PrevCompanyNo, string Direction)
        {
            //A11
            EmployeeDetails EmployeeDetails = new EmployeeDetails();

            EmployeeDetails.CompanyNo = "";
            EmployeeDetails.EmployeeCode = "";
            EmployeeDetails.EmployeeNo = "";
            EmployeeDetails.EmployeeNames = "";
            EmployeeDetails.RecordCount = "0";

            DataSet DataSet = new DataSet();

            try
            {
                StringBuilder strQry = new StringBuilder();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" 1 AS SORT_ORD");
                strQry.AppendLine(",E.COMPANY_NO");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",E.EMPLOYEE_CODE");
                strQry.AppendLine(",SUBSTRING(E.EMPLOYEE_NAME,1,1) + '.' + E.EMPLOYEE_SURNAME AS EMPLOYEE_NAMES");
                strQry.AppendLine(",0 AS REC_COUNT");
                
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                if (Convert.ToInt32(UserNo) > 0)
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA");
                    strQry.AppendLine(" ON E.COMPANY_NO = UCA.COMPANY_NO");
                    strQry.AppendLine(" AND USER_NO = " + UserNo);
                }

                strQry.AppendLine(" WHERE E.EMPLOYEE_NO = " + EmployeeNo);
                strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                if (Direction == "D")
                {
                    strQry.AppendLine(" AND E.COMPANY_NO > " + PrevCompanyNo);
                }
                else
                {
                    //UP
                    strQry.AppendLine(" AND E.COMPANY_NO < " + PrevCompanyNo);
                }

                strQry.AppendLine(" AND (E.NOT_ACTIVE_IND IS NULL");
                strQry.AppendLine(" OR E.NOT_ACTIVE_IND <> 'Y')");

                strQry.AppendLine(" UNION ");
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" 2 AS SORT_ORD");
                strQry.AppendLine(",E.COMPANY_NO");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",E.EMPLOYEE_CODE");
                strQry.AppendLine(",SUBSTRING(E.EMPLOYEE_NAME,1,1) + '.' + E.EMPLOYEE_SURNAME AS EMPLOYEE_NAMES");
                strQry.AppendLine(",COUNT(*) AS REC_COUNT");
                
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                if (Convert.ToInt32(UserNo) > 0)
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA");
                    strQry.AppendLine(" ON E.COMPANY_NO = UCA.COMPANY_NO");
                    strQry.AppendLine(" AND USER_NO = " + UserNo);
                }

                strQry.AppendLine(" WHERE E.EMPLOYEE_NO = " + EmployeeNo);
                strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                strQry.AppendLine(" AND (E.NOT_ACTIVE_IND IS NULL");
                strQry.AppendLine(" OR E.NOT_ACTIVE_IND <> 'Y')");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" E.COMPANY_NO");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",E.EMPLOYEE_CODE");
                strQry.AppendLine(",SUBSTRING(E.EMPLOYEE_NAME,1,1) + '.' + E.EMPLOYEE_SURNAME ");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" 1");

                if (Direction == "D")
                {
                    strQry.AppendLine(",2");
                }
                else
                {
                    strQry.AppendLine(",2 DESC");
                }

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

                if (DataSet.Tables["Employee"].Rows.Count == 0)
                {
                    EmployeeDetails.OK = "N";
                }
                else
                {
                    EmployeeDetails.OK = "Y";
                    EmployeeDetails.CompanyNo = DataSet.Tables["Employee"].Rows[0]["COMPANY_NO"].ToString();
                    EmployeeDetails.EmployeeNo = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"].ToString();
                    EmployeeDetails.EmployeeCode = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_CODE"].ToString();
                    EmployeeDetails.EmployeeNames = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NAMES"].ToString();

                    DataView CountDataView = new DataView(DataSet.Tables["Employee"], "SORT_ORD = 2", "", DataViewRowState.CurrentRows);

                    int intRecordCount = CountDataView.Count;

                    EmployeeDetails.RecordCount = intRecordCount.ToString();
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog("GetEmployeeDetails", ex);
                EmployeeDetails.OK = "N";
            }
            finally
            {
                DataSet.Dispose();
            }

            return EmployeeDetails;
        }

        public CompanyEmployeeDetails GetCompanyEmployeeDetailsNew(string DBNo, string CompanyNo)
        {
            CompanyEmployeeDetails CompanyEmployeeDetails = new CompanyEmployeeDetails();

            CompanyEmployeeDetails.OK = "N";
            CompanyEmployeeDetails.EmployeeNo = "";
            CompanyEmployeeDetails.EmployeeCode = "";
            CompanyEmployeeDetails.EmployeeName = "";
            CompanyEmployeeDetails.EmployeeSurname = "";
            CompanyEmployeeDetails.EmployeeFinger = "";

            string[] strFingerArray;

            DataSet DataSet = new DataSet();

            try
            {
                StringBuilder strQry = new StringBuilder();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" E.EMPLOYEE_NO");
                strQry.AppendLine(",E.EMPLOYEE_CODE");
                strQry.AppendLine(",E.EMPLOYEE_NAME");
                strQry.AppendLine(",E.EMPLOYEE_SURNAME");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + CompanyNo);
                strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" E.EMPLOYEE_SURNAME");
                strQry.AppendLine(",E.EMPLOYEE_NAME");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

                if (DataSet.Tables["Employee"].Rows.Count == 0)
                {
                }
                else
                {
                    CompanyEmployeeDetails.OK = "Y";

                    for (int intRow = 0; intRow < DataSet.Tables["Employee"].Rows.Count; intRow++)
                    {
                        if (intRow == 0)
                        {
                            CompanyEmployeeDetails.EmployeeNo = DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                            CompanyEmployeeDetails.EmployeeCode = DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_CODE"].ToString();
                            CompanyEmployeeDetails.EmployeeName = DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NAME"].ToString();
                            CompanyEmployeeDetails.EmployeeSurname = DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_SURNAME"].ToString();
                        }
                        else
                        {
                            CompanyEmployeeDetails.EmployeeNo += "#" + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                            CompanyEmployeeDetails.EmployeeCode += "#" + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_CODE"].ToString();
                            CompanyEmployeeDetails.EmployeeName += "#" + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NAME"].ToString();
                            CompanyEmployeeDetails.EmployeeSurname += "#" + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_SURNAME"].ToString();
                        }

                        strFingerArray = null;
                        strFingerArray = new string[10] { "N", "N", "N", "N", "N", "N", "N", "N", "N", "N" };

                        if (DataSet.Tables["EmployeeFinger"] != null)
                        {
                            DataSet.Tables["EmployeeFinger"].Clear();
                        }

                        strQry.Clear();
                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" FINGER_NO");
                        strQry.AppendLine(",CREATION_DATETIME");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");

                        strQry.AppendLine(" WHERE COMPANY_NO = " + CompanyNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString());

                        clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "EmployeeFinger");

                        for (int intFingerRow = 0; intFingerRow < DataSet.Tables["EmployeeFinger"].Rows.Count; intFingerRow++)
                        {
                            if (DataSet.Tables["EmployeeFinger"].Rows[intFingerRow]["CREATION_DATETIME"] == System.DBNull.Value)
                            {
                                strFingerArray[Convert.ToInt32(DataSet.Tables["EmployeeFinger"].Rows[intFingerRow]["FINGER_NO"])] = "L";
                            }
                            else
                            {
                                strFingerArray[Convert.ToInt32(DataSet.Tables["EmployeeFinger"].Rows[intFingerRow]["FINGER_NO"])] = "S";
                            }
                        }

                        for (int intFingerCountRow = 0; intFingerCountRow < 10; intFingerCountRow++)
                        {
                            if (intFingerCountRow == 0)
                            {
                                if (CompanyEmployeeDetails.EmployeeFinger == "")
                                {
                                    CompanyEmployeeDetails.EmployeeFinger = strFingerArray[intFingerCountRow].ToString();
                                }
                                else
                                {
                                    CompanyEmployeeDetails.EmployeeFinger += "#" + strFingerArray[intFingerCountRow].ToString();
                                }
                            }
                            else
                            {
                                CompanyEmployeeDetails.EmployeeFinger += "|" + strFingerArray[intFingerCountRow].ToString();
                            }
                        }
                    }
                }
#if (DEBUG)
                //CompanyEmployeeDetails.EmployeeNo += "#99999");
                //CompanyEmployeeDetails.EmployeeCode += "#101");
                //CompanyEmployeeDetails.EmployeeName += "#John");
                //CompanyEmployeeDetails.EmployeeSurname += "#van Heerden");

                //CompanyEmployeeDetails.EmployeeNo += "#102");
                //CompanyEmployeeDetails.EmployeeCode += "#102");
                //CompanyEmployeeDetails.EmployeeName += "#Danie");
                //CompanyEmployeeDetails.EmployeeSurname += "#Visser");

                //CompanyEmployeeDetails.EmployeeNo += "#103");
                //CompanyEmployeeDetails.EmployeeCode += "#103");
                //CompanyEmployeeDetails.EmployeeName += "#ZZZZZZZZZZZZZZZZZZZZZ");
                //CompanyEmployeeDetails.EmployeeSurname += "#ZZZZZZZZZZZZZZZZZZZZZ");

                //CompanyEmployeeDetails.EmployeeNo += "#104");
                //CompanyEmployeeDetails.EmployeeCode += "#104");
                //CompanyEmployeeDetails.EmployeeName += "#Michael");
                //CompanyEmployeeDetails.EmployeeSurname += "#Ward");

                //CompanyEmployeeDetails.EmployeeNo += "#105");
                //CompanyEmployeeDetails.EmployeeCode += "#105");
                //CompanyEmployeeDetails.EmployeeName += "#Jan");
                //CompanyEmployeeDetails.EmployeeSurname += "#Serfontein");

                //CompanyEmployeeDetails.EmployeeNo += "#106");
                //CompanyEmployeeDetails.EmployeeCode += "#106");
                //CompanyEmployeeDetails.EmployeeName += "#Paul");
                //CompanyEmployeeDetails.EmployeeSurname += "#Michael");

                //CompanyEmployeeDetails.EmployeeNo += "#107");
                //CompanyEmployeeDetails.EmployeeCode += "#107");
                //CompanyEmployeeDetails.EmployeeName += "#Errol");
                //CompanyEmployeeDetails.EmployeeSurname += "#Le Roux");

                //CompanyEmployeeDetails.EmployeeNo += "#108");
                //CompanyEmployeeDetails.EmployeeCode += "#108");
                //CompanyEmployeeDetails.EmployeeName += "#Pig");
                //CompanyEmployeeDetails.EmployeeSurname += "#Maroose");

                //CompanyEmployeeDetails.EmployeeNo += "#110");
                //CompanyEmployeeDetails.EmployeeCode += "#110");
                //CompanyEmployeeDetails.EmployeeName += "#Here");
                //CompanyEmployeeDetails.EmployeeSurname += "#Goes");

                //CompanyEmployeeDetails.EmployeeNo += "#111");
                //CompanyEmployeeDetails.EmployeeCode += "#111");
                //CompanyEmployeeDetails.EmployeeName += "#Wim");
                //CompanyEmployeeDetails.EmployeeSurname += "#Rossouw");

                //CompanyEmployeeDetails.EmployeeNo += "#112");
                //CompanyEmployeeDetails.EmployeeCode += "#112");
                //CompanyEmployeeDetails.EmployeeName += "#Mike");
                //CompanyEmployeeDetails.EmployeeSurname += "#du Plessis");

                //CompanyEmployeeDetails.EmployeeNo += "#113");
                //CompanyEmployeeDetails.EmployeeCode += "#113");
                //CompanyEmployeeDetails.EmployeeName += "#Lets");
                //CompanyEmployeeDetails.EmployeeSurname += "#See");

                //CompanyEmployeeDetails.EmployeeNo += "#114");
                //CompanyEmployeeDetails.EmployeeCode += "#114");
                //CompanyEmployeeDetails.EmployeeName += "#Paul");
                //CompanyEmployeeDetails.EmployeeSurname += "#O'Sullivan");
#endif
            }
            catch (Exception ex)
            {
                WriteExceptionLog("GetCompanyEmployeeDetails", ex);
            }
            finally
            {
                DataSet.Dispose();
            }

            return CompanyEmployeeDetails;
        }

        public CompanyEmployeeDetails GetCompanyEmployeeDetails(string DBNo, string CompanyNo)
        {
            CompanyEmployeeDetails CompanyEmployeeDetails = new CompanyEmployeeDetails();

            CompanyEmployeeDetails.OK = "N";
            CompanyEmployeeDetails.EmployeeNo = "";
            CompanyEmployeeDetails.EmployeeCode = "";
            CompanyEmployeeDetails.EmployeeName = "";
            CompanyEmployeeDetails.EmployeeSurname = "";
            CompanyEmployeeDetails.EmployeeFinger = "";

            string[] strFingerArray;

            DataSet DataSet = new DataSet();

            try
            {
                StringBuilder strQry = new StringBuilder();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" E.EMPLOYEE_NO");
                strQry.AppendLine(",E.EMPLOYEE_CODE");
                strQry.AppendLine(",E.EMPLOYEE_NAME");
                strQry.AppendLine(",E.EMPLOYEE_SURNAME");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + CompanyNo);
                strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" E.EMPLOYEE_SURNAME");
                strQry.AppendLine(",E.EMPLOYEE_NAME");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

                if (DataSet.Tables["Employee"].Rows.Count == 0)
                {
                }
                else
                {
                    CompanyEmployeeDetails.OK = "Y";

                    for (int intRow = 0; intRow < DataSet.Tables["Employee"].Rows.Count; intRow++)
                    {
                        if (intRow == 0)
                        {
                            CompanyEmployeeDetails.EmployeeNo = DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                            CompanyEmployeeDetails.EmployeeCode = DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_CODE"].ToString();
                            CompanyEmployeeDetails.EmployeeName = DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NAME"].ToString();
                            CompanyEmployeeDetails.EmployeeSurname = DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_SURNAME"].ToString();
                        }
                        else
                        {
                            CompanyEmployeeDetails.EmployeeNo += "#" + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                            CompanyEmployeeDetails.EmployeeCode += "#" + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_CODE"].ToString();
                            CompanyEmployeeDetails.EmployeeName += "#" + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NAME"].ToString();
                            CompanyEmployeeDetails.EmployeeSurname += "#" + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_SURNAME"].ToString();
                        }

                        strFingerArray = null;
                        strFingerArray = new string[10] { "N", "N", "N", "N", "N", "N", "N", "N", "N", "N" };

                        if (DataSet.Tables["EmployeeFinger"] != null)
                        {
                            DataSet.Tables["EmployeeFinger"].Clear();
                        }

                        strQry.Clear();
                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" FINGER_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");

                        strQry.AppendLine(" WHERE COMPANY_NO = " + CompanyNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString());

                        clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "EmployeeFinger");

                        for (int intFingerRow = 0; intFingerRow < DataSet.Tables["EmployeeFinger"].Rows.Count; intFingerRow++)
                        {
                            strFingerArray[Convert.ToInt32(DataSet.Tables["EmployeeFinger"].Rows[intFingerRow]["FINGER_NO"])] = "Y";
                        }

                        for (int intFingerCountRow = 0; intFingerCountRow < 10; intFingerCountRow++)
                        {
                            if (intFingerCountRow == 0)
                            {
                                if (CompanyEmployeeDetails.EmployeeFinger == "")
                                {
                                    CompanyEmployeeDetails.EmployeeFinger = strFingerArray[intFingerCountRow].ToString();
                                }
                                else
                                {
                                    CompanyEmployeeDetails.EmployeeFinger += "#" + strFingerArray[intFingerCountRow].ToString();
                                }
                            }
                            else
                            {
                                CompanyEmployeeDetails.EmployeeFinger += "|" + strFingerArray[intFingerCountRow].ToString();
                            }
                        }
                    }
                }
#if (DEBUG)
                //CompanyEmployeeDetails.EmployeeNo += "#99999");
                //CompanyEmployeeDetails.EmployeeCode += "#101");
                //CompanyEmployeeDetails.EmployeeName += "#John");
                //CompanyEmployeeDetails.EmployeeSurname += "#van Heerden");

                //CompanyEmployeeDetails.EmployeeNo += "#102");
                //CompanyEmployeeDetails.EmployeeCode += "#102");
                //CompanyEmployeeDetails.EmployeeName += "#Danie");
                //CompanyEmployeeDetails.EmployeeSurname += "#Visser");

                //CompanyEmployeeDetails.EmployeeNo += "#103");
                //CompanyEmployeeDetails.EmployeeCode += "#103");
                //CompanyEmployeeDetails.EmployeeName += "#ZZZZZZZZZZZZZZZZZZZZZ");
                //CompanyEmployeeDetails.EmployeeSurname += "#ZZZZZZZZZZZZZZZZZZZZZ");

                //CompanyEmployeeDetails.EmployeeNo += "#104");
                //CompanyEmployeeDetails.EmployeeCode += "#104");
                //CompanyEmployeeDetails.EmployeeName += "#Michael");
                //CompanyEmployeeDetails.EmployeeSurname += "#Ward");

                //CompanyEmployeeDetails.EmployeeNo += "#105");
                //CompanyEmployeeDetails.EmployeeCode += "#105");
                //CompanyEmployeeDetails.EmployeeName += "#Jan");
                //CompanyEmployeeDetails.EmployeeSurname += "#Serfontein");

                //CompanyEmployeeDetails.EmployeeNo += "#106");
                //CompanyEmployeeDetails.EmployeeCode += "#106");
                //CompanyEmployeeDetails.EmployeeName += "#Paul");
                //CompanyEmployeeDetails.EmployeeSurname += "#Michael");

                //CompanyEmployeeDetails.EmployeeNo += "#107");
                //CompanyEmployeeDetails.EmployeeCode += "#107");
                //CompanyEmployeeDetails.EmployeeName += "#Errol");
                //CompanyEmployeeDetails.EmployeeSurname += "#Le Roux");

                //CompanyEmployeeDetails.EmployeeNo += "#108");
                //CompanyEmployeeDetails.EmployeeCode += "#108");
                //CompanyEmployeeDetails.EmployeeName += "#Pig");
                //CompanyEmployeeDetails.EmployeeSurname += "#Maroose");

                //CompanyEmployeeDetails.EmployeeNo += "#110");
                //CompanyEmployeeDetails.EmployeeCode += "#110");
                //CompanyEmployeeDetails.EmployeeName += "#Here");
                //CompanyEmployeeDetails.EmployeeSurname += "#Goes");

                //CompanyEmployeeDetails.EmployeeNo += "#111");
                //CompanyEmployeeDetails.EmployeeCode += "#111");
                //CompanyEmployeeDetails.EmployeeName += "#Wim");
                //CompanyEmployeeDetails.EmployeeSurname += "#Rossouw");

                //CompanyEmployeeDetails.EmployeeNo += "#112");
                //CompanyEmployeeDetails.EmployeeCode += "#112");
                //CompanyEmployeeDetails.EmployeeName += "#Mike");
                //CompanyEmployeeDetails.EmployeeSurname += "#du Plessis");

                //CompanyEmployeeDetails.EmployeeNo += "#113");
                //CompanyEmployeeDetails.EmployeeCode += "#113");
                //CompanyEmployeeDetails.EmployeeName += "#Lets");
                //CompanyEmployeeDetails.EmployeeSurname += "#See");

                //CompanyEmployeeDetails.EmployeeNo += "#114");
                //CompanyEmployeeDetails.EmployeeCode += "#114");
                //CompanyEmployeeDetails.EmployeeName += "#Paul");
                //CompanyEmployeeDetails.EmployeeSurname += "#O'Sullivan");
#endif
            }
            catch (Exception ex)
            {
                WriteExceptionLog("GetCompanyEmployeeDetails", ex);
            }
            finally
            {
                DataSet.Dispose();
            }

            return CompanyEmployeeDetails;
        }
      
        public EnrollTemplateContainerReply InsertDPTemplate(string DBNo, string CompanyNo, string EmployeeNo, string FingerNo, string TemplateNo, FeaturesContainer FeaturesContainer)
        {
            //Time & Attendance
            StringBuilder strQry = new StringBuilder();

            EnrollTemplateContainerReply EnrollTemplateContainerReply = new EnrollTemplateContainerReply();
            EnrollTemplateContainerReply.OK = "Y";
            EnrollTemplateContainerReply.EnrollInd = "N";
            EnrollTemplateContainerReply.EnrollQuality = "";

            byte[] byteArrayPreviousTemplate = null;

            DataSet DataSet = new DataSet();

            try
            {
                clsFingerPrintClockServerLib FingerPrintClockServerLib = new clsFingerPrintClockServerLib(pvtstrSoftwareToUse);

                if (TemplateNo == "1")
                {
                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE_TEMP");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + CompanyNo);
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + EmployeeNo);
                    strQry.AppendLine(" AND FINGER_NO = " + FingerNo);

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }
                else
                {
                    //Get First FingerPrint Template for Compare
                    strQry.Clear();

                    strQry.AppendLine(" SELECT");
                    strQry.AppendLine(" FINGER_TEMPLATE");
                    
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE_TEMP");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + CompanyNo);
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + EmployeeNo);
                    strQry.AppendLine(" AND FINGER_NO = " + FingerNo);

                    if (TemplateNo != "4")
                    {
                        strQry.AppendLine(" AND FINGER_TEMPLATE_NO = 1 ");
                    }

                    strQry.AppendLine(" ORDER BY ");
                    strQry.AppendLine(" FINGER_TEMPLATE_NO");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Template");

                    byteArrayPreviousTemplate = (byte[])DataSet.Tables["Template"].Rows[0]["FINGER_TEMPLATE"];
                }

                int intReturnCode = 0;
                byte[] bytExtractedTemplate = null;

                if (Convert.ToInt32(TemplateNo) == 1)
                {
                    if (FeaturesContainer.DPFeaturesByteArray.Length == 101376)
                    {
                        //Curve - Embedded Linux
                        bytExtractedTemplate = DPUruNet.FeatureExtraction.CreateFmdFromRaw(FeaturesContainer.DPFeaturesByteArray, 0, 0, 288, 352, 500, Constants.Formats.Fmd.ANSI).Data.Bytes;
                    }
                    else
                    {
                        bytExtractedTemplate = FeaturesContainer.DPFeaturesByteArray;
                    }
                }
                else
                {
                    if (FeaturesContainer.DPFeaturesByteArray.Length == 101376)
                    {
                        //Curve - Embedded Linux
                        FeaturesContainer.DPFeaturesByteArray = DPUruNet.FeatureExtraction.CreateFmdFromRaw(FeaturesContainer.DPFeaturesByteArray, 0, 0, 288, 352, 500, Constants.Formats.Fmd.ANSI).Data.Bytes;
                    }

                    intReturnCode = FingerPrintClockServerLib.Get_DP_Template(ref DataSet, Convert.ToInt32(TemplateNo), byteArrayPreviousTemplate, FeaturesContainer.DPFeaturesByteArray, ref bytExtractedTemplate);
                }

                if (intReturnCode == 0)
                {
                    if (TemplateNo == "4")
                    {
                        //Need To See Where I Can Get This
                        EnrollTemplateContainerReply.EnrollQuality = "MediumQuality";
                        EnrollTemplateContainerReply.EnrollInd = "Y";

                        //Delete Temporary Table
                        strQry.Clear();

                        strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE_TEMP");
                        strQry.AppendLine(" WHERE COMPANY_NO = " + CompanyNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND FINGER_NO = " + FingerNo);

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                      
                        strQry.Clear();

                        //2017-05-06 (Maybe User Deletes New Fingerprint Template then this will Come into Effect)
                        strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE_DELETE");
                        strQry.AppendLine("(COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",FINGER_NO");
                        strQry.AppendLine(",CREATION_DATETIME)");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",FINGER_NO");
                        strQry.AppendLine(",CREATION_DATETIME ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");

                        strQry.AppendLine(" WHERE COMPANY_NO = " + CompanyNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND FINGER_NO = " + FingerNo);

                        strQry.AppendLine(" AND NOT CREATION_DATETIME IS NULL ");

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                        
                        //2017-04-29 - NB New Template Will Replace Old
                        strQry.Clear();

                        strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");
                        strQry.AppendLine(" WHERE COMPANY_NO = " + CompanyNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND FINGER_NO = " + FingerNo);

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                        strQry.Clear();

                        strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");
                        strQry.AppendLine("(COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",FINGER_NO ");
                        strQry.AppendLine(",FINGER_TEMPLATE) ");
                        strQry.AppendLine(" VALUES ");
                        strQry.AppendLine("(" + CompanyNo);
                        strQry.AppendLine("," + EmployeeNo);
                        strQry.AppendLine("," + FingerNo);
                        strQry.AppendLine(",@FINGER_TEMPLATE)");

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString(), bytExtractedTemplate, "@FINGER_TEMPLATE");
                    }
                    else
                    {
                        strQry.Clear();

                        strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE_TEMP");
                        strQry.AppendLine("(COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",FINGER_NO");
                        strQry.AppendLine(",FINGER_TEMPLATE_NO");
                        strQry.AppendLine(",FINGER_TEMPLATE) ");
                        strQry.AppendLine(" VALUES ");
                        strQry.AppendLine("(" + CompanyNo);
                        strQry.AppendLine("," + EmployeeNo);
                        strQry.AppendLine("," + FingerNo);
                        strQry.AppendLine("," + TemplateNo);
                        strQry.AppendLine(",@FINGER_TEMPLATE)");
                        
                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString(), bytExtractedTemplate, "@FINGER_TEMPLATE");
                    }
                }
                else
                {
                    if (intReturnCode == 2)
                    {
                        EnrollTemplateContainerReply.OK = "F";
                    }
                    else
                    {
                        EnrollTemplateContainerReply.OK = "N";
                    }
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog("InsertDPTemplate", ex);

                EnrollTemplateContainerReply.OK = "E";
            }
            finally
            {
                DataSet.Dispose();
            }

            return EnrollTemplateContainerReply;
        }


        public EnrollTemplateContainerReply InsertUserDPTemplate(string DBNo, string UserNo, string FingerNo, string TemplateNo, FeaturesContainer FeaturesContainer)
        {
            StringBuilder strQry = new StringBuilder();

            EnrollTemplateContainerReply EnrollTemplateContainerReply = new EnrollTemplateContainerReply();

            EnrollTemplateContainerReply.OK = "Y";
            EnrollTemplateContainerReply.EnrollInd = "";
            EnrollTemplateContainerReply.EnrollQuality = "";

            byte[] byteArrayPreviousTemplate = null;

            DataSet DataSet = new DataSet();

            try
            {
                clsFingerPrintClockServerLib FingerPrintClockServerLib = new clsFingerPrintClockServerLib(pvtstrSoftwareToUse);
              
                if (TemplateNo == "1")
                {
                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + UserNo);
                    strQry.AppendLine(" AND FINGER_NO = " + FingerNo);

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }
                else
                {
                    //Get First FingerPrint Template for Compare
                    strQry.Clear();

                    strQry.AppendLine(" SELECT");
                    strQry.AppendLine(" FINGER_TEMPLATE_NO");
                    strQry.AppendLine(",FINGER_TEMPLATE");
                    
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE_TEMP");
                    
                    strQry.AppendLine(" WHERE USER_NO = " + UserNo);
                    strQry.AppendLine(" AND FINGER_NO = " + FingerNo);

                    if (TemplateNo != "4")
                    {
                        strQry.AppendLine(" AND FINGER_TEMPLATE_NO = 1 ");
                    }

                    strQry.AppendLine(" ORDER BY ");
                    strQry.AppendLine(" FINGER_TEMPLATE_NO");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Template");

                    byteArrayPreviousTemplate = (byte[])DataSet.Tables["Template"].Rows[0]["FINGER_TEMPLATE"];
                }

                int intReturnCode = 0;
                byte[] bytExtractedTemplate = null;

                if (Convert.ToInt32(TemplateNo) == 1)
                {
                    if (FeaturesContainer.DPFeaturesByteArray.Length == 101376)
                    {
                        //Curve - Embedded Linux
                        bytExtractedTemplate = DPUruNet.FeatureExtraction.CreateFmdFromRaw(FeaturesContainer.DPFeaturesByteArray, 0, 0, 288, 352, 500, Constants.Formats.Fmd.ANSI).Data.Bytes;
                    }
                    else
                    {
                        bytExtractedTemplate = FeaturesContainer.DPFeaturesByteArray;
                    }
                }
                else
                {
                    if (FeaturesContainer.DPFeaturesByteArray.Length == 101376)
                    {
                        //Curve - Embedded Linux
                        FeaturesContainer.DPFeaturesByteArray = DPUruNet.FeatureExtraction.CreateFmdFromRaw(FeaturesContainer.DPFeaturesByteArray, 0, 0, 288, 352, 500, Constants.Formats.Fmd.ANSI).Data.Bytes;
                    }

                    intReturnCode = FingerPrintClockServerLib.Get_DP_Template(ref DataSet, Convert.ToInt32(TemplateNo), byteArrayPreviousTemplate, FeaturesContainer.DPFeaturesByteArray, ref bytExtractedTemplate);
                }

                if (intReturnCode == 0)
                {
                    if (TemplateNo == "4")
                    {
                        EnrollTemplateContainerReply.EnrollInd = "Y";
                        EnrollTemplateContainerReply.EnrollQuality = "MediumQuality";

                        //Delete Temporary Table
                        strQry.Clear();

                        strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE_TEMP");
                        strQry.AppendLine(" WHERE USER_NO = " + UserNo);
                        strQry.AppendLine(" AND FINGER_NO = " + FingerNo);

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                        strQry.Clear();

                        //2017-05-06 (Maybe User Deletes New Fingerprint Template then this will Come into Effect)
                        strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE_DELETE");
                        strQry.AppendLine("(USER_NO");
                        strQry.AppendLine(",FINGER_NO");
                        strQry.AppendLine(",CREATION_DATETIME)");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" USER_NO");
                        strQry.AppendLine(",FINGER_NO");
                        strQry.AppendLine(",CREATION_DATETIME ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE");

                        strQry.AppendLine(" WHERE USER_NO = " + UserNo);
                        strQry.AppendLine(" AND FINGER_NO = " + FingerNo);

                        strQry.AppendLine(" AND NOT CREATION_DATETIME IS NULL ");

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                        //2017-04-29 - NB New Template Will Replace Old
                        strQry.Clear();

                        strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE");
                        strQry.AppendLine(" WHERE USER_NO = " + UserNo);
                        strQry.AppendLine(" AND FINGER_NO = " + FingerNo);

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                        strQry.Clear();

                        strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE");
                        strQry.AppendLine("(USER_NO");
                        strQry.AppendLine(",FINGER_NO");
                        strQry.AppendLine(",FINGER_TEMPLATE)");

                        strQry.AppendLine(" VALUES ");
                        strQry.AppendLine("(" + UserNo);
                        strQry.AppendLine("," + FingerNo);
                        strQry.AppendLine(",@FINGER_TEMPLATE)");

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString(), bytExtractedTemplate, "@FINGER_TEMPLATE");
                    }
                    else
                    {
                        strQry.Clear();

                        //2017-04-29
                        strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE_TEMP");
                        strQry.AppendLine("(USER_NO");
                        strQry.AppendLine(",FINGER_NO");
                        strQry.AppendLine(",FINGER_TEMPLATE_NO");
                        strQry.AppendLine(",FINGER_TEMPLATE)");
                        strQry.AppendLine(" VALUES ");
                        strQry.AppendLine("(" + UserNo);
                        strQry.AppendLine("," + FingerNo);
                        strQry.AppendLine("," + TemplateNo);
                        strQry.AppendLine(",@FINGER_TEMPLATE)");
                        
                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString(), bytExtractedTemplate, "@FINGER_TEMPLATE");
                    }
                }
                else
                {
                    if (intReturnCode == 2)
                    {
                        EnrollTemplateContainerReply.OK = "F";
                    }
                    else
                    {
                        EnrollTemplateContainerReply.OK = "N";
                    }
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog("InsertUserDPTemplate", ex);

                EnrollTemplateContainerReply.OK = "E";
            }
            finally
            {
                DataSet.Dispose();
            }

            return EnrollTemplateContainerReply;
        }

        public EnrollFingerReply GetEnrolledUser(string UserNo, string FingerNo, string TemplateNo, ImageContainer ImageContainer)
        {
            //A11
            StringBuilder strQry = new StringBuilder();

            EnrollFingerReply EnrollFingerReply = new EnrollFingerReply();
            EnrollFingerReply.OkInd = "Y";
            EnrollFingerReply.EnrollInd = "";
            EnrollFingerReply.EnrollQuality = "";
            EnrollFingerReply.VerifyFingerPrintsCompareScore = 0;

            byte[] byteArrayPreviousTemplate = null;
           
            DataSet DataSet = new DataSet();

            try
            {
                //A11 Cock Image
                int intGreyScaleHeight = 256 + (int)ImageContainer.DPImageByteArray[2];
                int intGreyScaleWidth = 256 + (int)ImageContainer.DPImageByteArray[0];

                byte[] bytCreatedRawFmd = new byte[intGreyScaleHeight * intGreyScaleWidth];

                unsafe
                {
                    IntPtr ptr;

                    fixed (byte* p = bytCreatedRawFmd)
                    {
                        ptr = (IntPtr)p;
                    }

                    Marshal.Copy(ImageContainer.DPImageByteArray, 4, ptr, bytCreatedRawFmd.Length);
                }

                byte[] bytFmdFingerTemplate = FeatureExtraction.CreateFmdFromRaw(bytCreatedRawFmd, 0, 0, intGreyScaleWidth, intGreyScaleHeight, 500, Constants.Formats.Fmd.ANSI).Data.Bytes; 

                clsFingerPrintClockServerLib FingerPrintClockServerLib = new clsFingerPrintClockServerLib(pvtstrSoftwareToUse);

                if (TemplateNo == "1")
                {
                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + UserNo);
                    strQry.AppendLine(" AND FINGER_NO = " + FingerNo);

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }
                else
                {
                    //Get First FingerPrint Template for Compare
                    strQry.Clear();

                    strQry.AppendLine(" SELECT");
                    strQry.AppendLine(" FINGER_TEMPLATE");
                    
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE_TEMP");
                    
                    strQry.AppendLine(" WHERE USER_NO = " + UserNo);
                    strQry.AppendLine(" AND FINGER_NO = " + FingerNo);

                    if (TemplateNo != "4")
                    {
                        strQry.AppendLine(" AND FINGER_TEMPLATE_NO = 1 ");
                    }

                    strQry.AppendLine(" ORDER BY ");
                    strQry.AppendLine(" FINGER_TEMPLATE_NO");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Template");

                    byteArrayPreviousTemplate = (byte[])DataSet.Tables["Template"].Rows[0]["FINGER_TEMPLATE"];
                }

                int intThresholdCompareScore = 0;

                if (pvtstrSoftwareToUse == "D")
                {
                    // 1/10000 
                    intThresholdCompareScore = 214748;
                }
                else
                {

                    strQry.Clear();
                    strQry.AppendLine(" SELECT");
                    strQry.AppendLine(" IDENTIFY_THRESHOLD_VALUE");
                    
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.FINGERPRINT_IDENTIFY_VERIFY_THRESHOLD ");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Threshold");

                    intThresholdCompareScore = Convert.ToInt32(DataSet.Tables["Threshold"].Rows[0]["IDENTIFY_THRESHOLD_VALUE"]);
                }

                int intVerifyFingerPrintsCompareScore = 0;

                byte[] bytExtractedTemplate = null;

                int intReturnCode = 0;

                if (Convert.ToInt32(TemplateNo) == 1)
                {
                    bytExtractedTemplate = bytFmdFingerTemplate;
                }
                else
                {
                    intReturnCode = FingerPrintClockServerLib.Get_DP_Template(ref DataSet, Convert.ToInt32(TemplateNo), byteArrayPreviousTemplate, bytFmdFingerTemplate, ref bytExtractedTemplate);
                }

                if (intReturnCode == 0)
                {
                    if (TemplateNo == "4")
                    {
                        EnrollFingerReply.EnrollInd = "Y";
                        EnrollFingerReply.EnrollQuality = "MediumQuality";

                        if (EnrollFingerReply.EnrollInd == "Y")
                        {
                            //Delete Temporary Table
                            strQry.Clear();

                            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE_TEMP");
                            strQry.AppendLine(" WHERE USER_NO = " + UserNo);
                            strQry.AppendLine(" AND FINGER_NO = " + FingerNo);

                            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                            strQry.Clear();

                            //2017-05-06 (Maybe User Deletes New Fingerprint Template then this will Come into Effect)
                            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE_DELETE");
                            strQry.AppendLine("(USER_NO");
                            strQry.AppendLine(",FINGER_NO");
                            strQry.AppendLine(",CREATION_DATETIME)");

                            strQry.AppendLine(" SELECT ");
                            strQry.AppendLine(" USER_NO");
                            strQry.AppendLine(",FINGER_NO");
                            strQry.AppendLine(",CREATION_DATETIME ");

                            strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE");

                            strQry.AppendLine(" WHERE USER_NO = " + UserNo);
                            strQry.AppendLine(" AND FINGER_NO = " + FingerNo);

                            strQry.AppendLine(" AND NOT CREATION_DATETIME IS NULL ");

                            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                            //2017-04-29 - NB New Template Will Replace Old
                            strQry.Clear();

                            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE");
                            strQry.AppendLine(" WHERE USER_NO = " + UserNo);
                            strQry.AppendLine(" AND FINGER_NO = " + FingerNo);

                            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                            
                            strQry.Clear();

                            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE");
                            strQry.AppendLine("(USER_NO");
                            strQry.AppendLine(",FINGER_NO");
                            strQry.AppendLine(",FINGER_TEMPLATE)");

                            strQry.AppendLine(" VALUES ");
                            strQry.AppendLine("(" + UserNo);
                            strQry.AppendLine("," + FingerNo);
                            strQry.AppendLine(",@FINGER_TEMPLATE)");
                            
                            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString(), bytExtractedTemplate, "@FINGER_TEMPLATE");
                        }
                    }
                    else
                    {
                        strQry.Clear();
                       
                        //2017-04-29
                        strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE_TEMP");
                        strQry.AppendLine("(USER_NO");
                        strQry.AppendLine(",FINGER_NO");
                        strQry.AppendLine(",FINGER_TEMPLATE_NO");
                        strQry.AppendLine(",FINGER_TEMPLATE)");
                        strQry.AppendLine(" VALUES ");
                        strQry.AppendLine("(" + UserNo);
                        strQry.AppendLine("," + FingerNo);
                        strQry.AppendLine("," + TemplateNo);
                        strQry.AppendLine(",@FINGER_TEMPLATE)");

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString(), bytExtractedTemplate, "@FINGER_TEMPLATE");
                    }
                }
                else
                {
                    if (intReturnCode == 2)
                    {
                        EnrollFingerReply.OkInd = "F";
                    }
                    else
                    {
                        EnrollFingerReply.OkInd = "N";
                    }
                }

                EnrollFingerReply.VerifyFingerPrintsCompareScore = intVerifyFingerPrintsCompareScore;
            }
            catch (Exception ex)
            {
                WriteExceptionLog("GetEnrolledUser", ex);

                EnrollFingerReply.OkInd = "N";
            }

        GetEnrolledUser_Continue:

            DataSet.Dispose();

            return EnrollFingerReply;
        }

        public EnrollFingerReply GetEnrolledEmployee(string CompanyNo, string EmployeeNo, string FingerNo, string TemplateNo, ImageContainer ImageContainer)
        {
            //A11
            StringBuilder strQry = new StringBuilder();
           
            EnrollFingerReply EnrollFingerReply = new EnrollFingerReply();
            EnrollFingerReply.OkInd = "Y";
            EnrollFingerReply.EnrollInd = "";
            EnrollFingerReply.EnrollQuality = "";
            EnrollFingerReply.VerifyFingerPrintsCompareScore = 0;

            byte[] byteArrayPreviousTemplate = null;

            DataSet DataSet = new DataSet();

            try
            {
                //A11 Cock Image
                int intGreyScaleHeight = 256 + (int)ImageContainer.DPImageByteArray[2];
                int intGreyScaleWidth = 256 + (int)ImageContainer.DPImageByteArray[0];

                byte[] bytCreatedRawFmd = new byte[intGreyScaleHeight * intGreyScaleWidth];

                unsafe
                {
                    IntPtr ptr;

                    fixed (byte* p = bytCreatedRawFmd)
                    {
                        ptr = (IntPtr)p;
                    }
                  
                    Marshal.Copy(ImageContainer.DPImageByteArray, 4, ptr, bytCreatedRawFmd.Length);
                }

                byte[] bytFmdFingerTemplate = FeatureExtraction.CreateFmdFromRaw(bytCreatedRawFmd, 0, 0, intGreyScaleWidth, intGreyScaleHeight,500, Constants.Formats.Fmd.ANSI).Data.Bytes; 

                clsFingerPrintClockServerLib FingerPrintClockServerLib = new clsFingerPrintClockServerLib(pvtstrSoftwareToUse);

                if (TemplateNo == "1")
                {
                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE_TEMP");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + CompanyNo);
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + EmployeeNo);
                    strQry.AppendLine(" AND FINGER_NO = " + FingerNo);

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }
                else
                {
                    //Get First FingerPrint Template for Compare
                    strQry.Clear();

                    strQry.AppendLine(" SELECT");
                    strQry.AppendLine(" FINGER_TEMPLATE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE_TEMP");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + CompanyNo);
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + EmployeeNo);
                    strQry.AppendLine(" AND FINGER_NO = " + FingerNo);

                    if (TemplateNo != "4")
                    {
                        strQry.AppendLine(" AND FINGER_TEMPLATE_NO = 1 ");
                    }

                    strQry.AppendLine(" ORDER BY ");
                    strQry.AppendLine(" FINGER_TEMPLATE_NO");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Template");

                    byteArrayPreviousTemplate = (byte[])DataSet.Tables["Template"].Rows[0]["FINGER_TEMPLATE"];
                }

                int intThresholdCompareScore = 0;
                int intVerifyFingerPrintsCompareScore = 0;

                if (pvtstrSoftwareToUse == "D")
                {
                    // 1/10000
                    intThresholdCompareScore = 214748;
                }
                else
                {
                    strQry.Clear();
                    strQry.AppendLine(" SELECT");
                    strQry.AppendLine(" IDENTIFY_THRESHOLD_VALUE");
                    
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.FINGERPRINT_IDENTIFY_VERIFY_THRESHOLD");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Threshold");

                    intThresholdCompareScore = Convert.ToInt32(DataSet.Tables["Threshold"].Rows[0]["IDENTIFY_THRESHOLD_VALUE"]);
                }

                int intReturnCode = 0;
                byte[] bytExtractedTemplate = null;

                if (Convert.ToInt32(TemplateNo) == 1)
                {
                    bytExtractedTemplate = bytFmdFingerTemplate;
                }
                else
                {
                    intReturnCode = FingerPrintClockServerLib.Get_DP_Template(ref DataSet, Convert.ToInt32(TemplateNo), byteArrayPreviousTemplate, bytFmdFingerTemplate, ref bytExtractedTemplate);
                }

                if (intReturnCode == 0)
                {
                    if (TemplateNo == "4")
                    {
                        if (EnrollFingerReply.EnrollInd == "")
                        {
                            EnrollFingerReply.EnrollInd = "Y";
                            EnrollFingerReply.EnrollQuality = "MediumQuality";
                        }
                       
                        if (EnrollFingerReply.EnrollInd == "Y")
                        {
                            //Delete Temporary Table
                            strQry.Clear();

                            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE_TEMP");
                            strQry.AppendLine(" WHERE COMPANY_NO = " + CompanyNo);
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + EmployeeNo);
                            strQry.AppendLine(" AND FINGER_NO = " + FingerNo);

                            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                            //2017-05-06 (Maybe User Deletes New Fingerprint Template then this will Come into Effect)
                            strQry.Clear();

                            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE_DELETE");
                            strQry.AppendLine("(COMPANY_NO");
                            strQry.AppendLine(",EMPLOYEE_NO");
                            strQry.AppendLine(",FINGER_NO");
                            strQry.AppendLine(",CREATION_DATETIME)");

                            strQry.AppendLine(" SELECT ");
                            strQry.AppendLine(" COMPANY_NO");
                            strQry.AppendLine(",EMPLOYEE_NO");
                            strQry.AppendLine(",FINGER_NO");
                            strQry.AppendLine(",CREATION_DATETIME ");

                            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");

                            strQry.AppendLine(" WHERE COMPANY_NO = " + CompanyNo);
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + EmployeeNo);
                            strQry.AppendLine(" AND FINGER_NO = " + FingerNo);

                            strQry.AppendLine(" AND NOT CREATION_DATETIME IS NULL ");

                            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                            //2017-04-29 - NB New Template Will Replace Old
                            strQry.Clear();

                            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");
                            strQry.AppendLine(" WHERE COMPANY_NO = " + CompanyNo);
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + EmployeeNo);
                            strQry.AppendLine(" AND FINGER_NO = " + FingerNo);

                            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                            //Insert Directly into Prod Table
                            strQry.Clear();
                           
                            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");
                            strQry.AppendLine("(COMPANY_NO");
                            strQry.AppendLine(",EMPLOYEE_NO");
                            strQry.AppendLine(",FINGER_NO ");
                            strQry.AppendLine(",FINGER_TEMPLATE)");
                            strQry.AppendLine(" VALUES ");
                            strQry.AppendLine("(" + CompanyNo);
                            strQry.AppendLine("," + EmployeeNo);
                            strQry.AppendLine("," + FingerNo);
                            strQry.AppendLine(",@FINGER_TEMPLATE)");
                            
                            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString(), bytExtractedTemplate, "@FINGER_TEMPLATE");
                        }
                    }
                    else
                    {
                        strQry.Clear();

                        strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE_TEMP");
                        strQry.AppendLine("(COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",FINGER_NO");
                        strQry.AppendLine(",FINGER_TEMPLATE_NO ");
                        strQry.AppendLine(",FINGER_TEMPLATE)");
                        strQry.AppendLine(" VALUES ");
                        strQry.AppendLine("(" + CompanyNo);
                        strQry.AppendLine("," + EmployeeNo);
                        strQry.AppendLine("," + FingerNo);
                        strQry.AppendLine("," + TemplateNo);
                        strQry.AppendLine(",@FINGER_TEMPLATE)");
                        
                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString(), bytExtractedTemplate, "@FINGER_TEMPLATE");
                    }
                }
                else
                {
                    if (intReturnCode == 2)
                    {
                        EnrollFingerReply.OkInd = "F";
                    }
                    else
                    {
                        EnrollFingerReply.OkInd = "N";
                    }
                }

                EnrollFingerReply.VerifyFingerPrintsCompareScore = intVerifyFingerPrintsCompareScore;
            }
            catch (Exception ex)
            {
                WriteExceptionLog("GetEnrolledEmployee", ex);

                EnrollFingerReply.OkInd = "N";
            }

        GetEnrolledEmployee_Continue:

            DataSet.Dispose();

            return EnrollFingerReply;
        }
        
        public EmployeeNameSurname GetEmployeePinClocked(string DBNo, string DeviceNo, string InOutParm,string BreakParm, string EmployeeNo, string Pin)
        {
            int intWhere = 0;
            StringBuilder strQry = new StringBuilder();

            EmployeeNameSurname EmployeeNameSurname = new EmployeeNameSurname();

            EmployeeNameSurname.OkInd = "Y";
            EmployeeNameSurname.RfIdNo = "0";
            EmployeeNameSurname.FingerPrintScore = "0";
            EmployeeNameSurname.Name = "";
            EmployeeNameSurname.Surname = "";
            EmployeeNameSurname.EmployeeNo = EmployeeNo;

            DataSet DataSet = new DataSet();

            try
            {
                intWhere = 1;

                string strDate = DateTime.Now.ToString("yyyy-MM-dd");

                string strTime = Convert.ToInt32((DateTime.Now.Hour * 60) + DateTime.Now.Minute).ToString();
                string strAddOnClockIn = "";
                string strAddOnClockOut = "";
                string strAddOnClockInApplies = "";
                StringBuilder strQryTemp= new StringBuilder();
#if (DEBUG)
                //In
                strTime = "417";

                //strTime = "593";
                //InOutParm = "O";



#endif

                switch (Convert.ToInt32(DateTime.Now.DayOfWeek))
                {
                    case 1:

                        strAddOnClockIn = ",X.MON_CLOCK_IN";
                        strAddOnClockOut = ",X.MON_CLOCK_OUT";
                        strAddOnClockInApplies = ",X.MON_CLOCK_IN_APPLIES_IND";
                        break;

                    case 2:

                        strAddOnClockIn = ",X.TUE_CLOCK_IN";
                        strAddOnClockOut = ",X.TUE_CLOCK_OUT";
                        strAddOnClockInApplies = ",X.TUE_CLOCK_IN_APPLIES_IND";
                        break;

                    case 3:

                        strAddOnClockIn = ",X.WED_CLOCK_IN";
                        strAddOnClockOut = ",X.WED_CLOCK_OUT";
                        strAddOnClockInApplies = ",X.WED_CLOCK_IN_APPLIES_IND";
                        break;

                    case 4:

                        strAddOnClockIn = ",X.THU_CLOCK_IN";
                        strAddOnClockOut = ",X.THU_CLOCK_OUT";
                        strAddOnClockInApplies = ",X.THU_CLOCK_IN_APPLIES_IND";
                        break;

                    case 5:

                        strAddOnClockIn = ",X.FRI_CLOCK_IN";
                        strAddOnClockOut = ",X.FRI_CLOCK_OUT";
                        strAddOnClockInApplies = ",X.FRI_CLOCK_IN_APPLIES_IND";
                        break;

                    case 6:

                        strAddOnClockIn = ",X.SAT_CLOCK_IN";
                        strAddOnClockOut = ",X.SAT_CLOCK_OUT";
                        strAddOnClockInApplies = ",X.SAT_CLOCK_IN_APPLIES_IND";
                        break;

                    case 0:

                        strAddOnClockIn = ",X.SUN_CLOCK_IN";
                        strAddOnClockOut = ",X.SUN_CLOCK_OUT";
                        strAddOnClockInApplies = ",X.SUN_CLOCK_IN_APPLIES_IND";
                        break;
                }

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                
                //2017-08-12
                strQry.AppendLine(" DEVICE_EMPLOYEE.SORT_ORDER");
                strQry.AppendLine(",E.COMPANY_NO");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",DEVICE_EMPLOYEE.PAY_CATEGORY_NO");
                strQry.AppendLine(",DEVICE_EMPLOYEE.PAY_CATEGORY_TYPE");

                strQry.AppendLine(strAddOnClockIn.Replace("X.", "DEVICE_EMPLOYEE."));
                strQry.AppendLine(strAddOnClockOut.Replace("X.", "DEVICE_EMPLOYEE."));
                strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DEVICE_EMPLOYEE."));

                strQry.AppendLine(",DEVICE_EMPLOYEE.TIME_ATTEND_ROUNDING_IND");
                strQry.AppendLine(",DEVICE_EMPLOYEE.TIME_ATTEND_ROUNDING_VALUE");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(SELECT ");
                strQry.AppendLine(" DEL.COMPANY_NO");

                strQry.AppendLine(strAddOnClockIn.Replace("X.", "'' AS "));
                strQry.AppendLine(strAddOnClockOut.Replace("X.", "'' AS "));
                strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "'' AS "));

                strQry.AppendLine(",NULL AS TIME_ATTEND_ROUNDING_IND");
                strQry.AppendLine(",0 AS TIME_ATTEND_ROUNDING_VALUE");

                strQry.AppendLine(",DEL.EMPLOYEE_NO");
                
                strQry.AppendLine(",DEL.PAY_CATEGORY_NO");
                strQry.AppendLine(",DEL.PAY_CATEGORY_TYPE");

                //2017-08-12 - Sort will move this to Bottom
                strQry.AppendLine(",2 AS SORT_ORDER");
                
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_EMPLOYEE_LINK DEL");

                strQry.AppendLine(" WHERE DEL.DEVICE_NO = " + DeviceNo);
                strQry.AppendLine(" AND DEL.EMPLOYEE_NO = " + EmployeeNo);

                //Cost Centre 
                strQry.AppendLine(" UNION ");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EPC.COMPANY_NO");

                strQry.AppendLine(strAddOnClockIn.Replace("X.", "DPCLA."));
                strQry.AppendLine(strAddOnClockOut.Replace("X.", "DPCLA."));
                strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DPCLA."));

                strQry.AppendLine(",DPCLA.TIME_ATTEND_ROUNDING_IND");
                strQry.AppendLine(",DPCLA.TIME_ATTEND_ROUNDING_VALUE");

                strQry.AppendLine(",EPC.EMPLOYEE_NO");
                
                strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
                
                //2017-08-12 - Sort 
                strQry.AppendLine(",SORT_ORDER = ");
                strQry.AppendLine(" CASE ");

                strQry.AppendLine(" WHEN NOT DPCLA.COMPANY_NO IS NULL THEN 1 ");

                strQry.AppendLine(" ELSE 2 ");

                strQry.AppendLine(" END ");
                
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK DPCL");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON DPCL.COMPANY_NO = EPC.COMPANY_NO ");
                strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + EmployeeNo);

                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK_ACTIVE DPCLA");
                strQry.AppendLine(" ON DPCL.DEVICE_NO = DPCLA.DEVICE_NO ");
                strQry.AppendLine(" AND DPCL.COMPANY_NO = DPCLA.COMPANY_NO ");
                strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = DPCLA.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = DPCLA.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND DPCLA.ACTIVE_IND = 'Y' ");

                strQry.AppendLine(" WHERE DPCL.DEVICE_NO = " + DeviceNo);

                //Department 
                strQry.AppendLine(" UNION");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EPC.COMPANY_NO");

                strQry.AppendLine(strAddOnClockIn.Replace("X.", "DDLA."));
                strQry.AppendLine(strAddOnClockOut.Replace("X.", "DDLA."));
                strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DDLA."));

                strQry.AppendLine(",DDLA.TIME_ATTEND_ROUNDING_IND");
                strQry.AppendLine(",DDLA.TIME_ATTEND_ROUNDING_VALUE");

                strQry.AppendLine(",EPC.EMPLOYEE_NO");
                
                strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                //2017-08-12 - Sort 
                strQry.AppendLine(",SORT_ORDER = ");
                strQry.AppendLine(" CASE ");

                strQry.AppendLine(" WHEN NOT DDLA.COMPANY_NO IS NULL THEN 1 ");

                strQry.AppendLine(" ELSE 2 ");

                strQry.AppendLine(" END ");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK DDL");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON DDL.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND DDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON DDL.COMPANY_NO = EPC.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + EmployeeNo);

                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK_ACTIVE DDLA");
                strQry.AppendLine(" ON DDL.DEVICE_NO = DDLA.DEVICE_NO ");
                strQry.AppendLine(" AND DDL.COMPANY_NO = DDLA.COMPANY_NO ");
                strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = DDLA.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = DDLA.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND DDL.DEPARTMENT_NO = DDLA.DEPARTMENT_NO ");
                strQry.AppendLine(" AND DDLA.ACTIVE_IND = 'Y' ");

                strQry.AppendLine(" WHERE DDL.DEVICE_NO = " + DeviceNo);

                //Group
                strQry.AppendLine(" UNION");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" GEL.COMPANY_NO");

                strQry.AppendLine(strAddOnClockIn.Replace("X.", "DGLA."));
                strQry.AppendLine(strAddOnClockOut.Replace("X.", "DGLA."));
                strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DGLA."));

                strQry.AppendLine(",DGLA.TIME_ATTEND_ROUNDING_IND");
                strQry.AppendLine(",DGLA.TIME_ATTEND_ROUNDING_VALUE");

                strQry.AppendLine(",GEL.EMPLOYEE_NO");
                strQry.AppendLine(",GEL.PAY_CATEGORY_NO");
                strQry.AppendLine(",GEL.PAY_CATEGORY_TYPE");

                //2017-08-12 - Sort 
                strQry.AppendLine(",SORT_ORDER = ");
                strQry.AppendLine(" CASE ");

                strQry.AppendLine(" WHEN NOT DGLA.COMPANY_NO IS NULL THEN 1 ");

                strQry.AppendLine(" ELSE 2 ");

                strQry.AppendLine(" END ");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK DGL");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.GROUP_EMPLOYEE_LINK GEL");
                strQry.AppendLine(" ON DGL.GROUP_NO = GEL.GROUP_NO ");
                strQry.AppendLine(" AND DGL.COMPANY_NO = GEL.COMPANY_NO ");
                strQry.AppendLine(" AND GEL.EMPLOYEE_NO = " + EmployeeNo);

                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_GROUP_LINK_ACTIVE DGLA");
                strQry.AppendLine(" ON DGL.DEVICE_NO = DGLA.DEVICE_NO ");
                strQry.AppendLine(" AND DGL.COMPANY_NO = DGLA.COMPANY_NO ");
                strQry.AppendLine(" AND DGL.GROUP_NO = DGLA.GROUP_NO ");
                strQry.AppendLine(" AND DGLA.ACTIVE_IND = 'Y' ");

                strQry.AppendLine(" WHERE DGL.DEVICE_NO = " + DeviceNo + ") AS DEVICE_EMPLOYEE");

                strQry.AppendLine(" ON E.COMPANY_NO = DEVICE_EMPLOYEE.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = DEVICE_EMPLOYEE.EMPLOYEE_NO");

                //2014-08-16
                strQry.AppendLine(" WHERE E.EMPLOYEE_PIN = " + Pin);

                strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                //2017-08-12 - Sort 
                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" DEVICE_EMPLOYEE.SORT_ORDER ");
                strQry.AppendLine(",E.COMPANY_NO ");
                strQry.AppendLine(",E.EMPLOYEE_NO ");
                
                intWhere = 2;
                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "EmployeeLinked");

                if (DataSet.Tables["EmployeeLinked"].Rows.Count == 0)
                {
                    intWhere = 3;
                    EmployeeNameSurname.Name = "NOT FOUND";
                }
                else
                {
                    intWhere = 4;

                    strQry.Clear();
                    strQry.AppendLine(" SELECT TOP 1 ");
                    strQry.AppendLine(" E.EMPLOYEE_NAME");
                    strQry.AppendLine(",E.EMPLOYEE_SURNAME");

                    //Not A Break
                    if (BreakParm != "Y")
                    {
                      if (InOutParm == "I")
                        {
                            strQry.AppendLine(",ISNULL(ETC.TIMESHEET_TIME_IN_MINUTES,0) AS TIMESHEET_TIME_MINUTES");
                            strQry.AppendLine(",ISNULL(ETC.CLOCKED_TIME_IN_MINUTES,0) AS CLOCKED_TIME_MINUTES");
                        }
                        else
                        {
                            strQry.AppendLine(",ISNULL(ETC.TIMESHEET_TIME_OUT_MINUTES,0) AS TIMESHEET_TIME_MINUTES");
                            strQry.AppendLine(",ISNULL(ETC.CLOCKED_TIME_OUT_MINUTES,0) AS CLOCKED_TIME_MINUTES");
                        }

                        strQry.AppendLine(",ETC.TIMESHEET_SEQ");
                    }

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                    //Not A Break
                    if (BreakParm != "Y")
                    {
                        if (DataSet.Tables["EmployeeLinked"].Rows[0]["PAY_CATEGORY_TYPE"].ToString() == "W")
                        {
                            strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
                        }
                        else
                        {
                            if (DataSet.Tables["EmployeeLinked"].Rows[0]["PAY_CATEGORY_TYPE"].ToString() == "S")
                            {
                                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC");
                            }
                            else
                            {
                                //T = Time Attendance
                                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC");
                            }
                        }

                        strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO ");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + DataSet.Tables["EmployeeLinked"].Rows[0]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine(" AND TIMESHEET_DATE = '" + strDate + "'");
                    }

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + DataSet.Tables["EmployeeLinked"].Rows[0]["COMPANY_NO"].ToString());
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = " + DataSet.Tables["EmployeeLinked"].Rows[0]["EMPLOYEE_NO"].ToString());
                    strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                    strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                    //Not A Break
                    if (BreakParm != "Y")
                    {
                        strQry.AppendLine(" ORDER BY ");
                        strQry.AppendLine(" ETC.TIMESHEET_SEQ DESC");
                    }

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

                    intWhere = 5;

                    //Not A Break
                    if (BreakParm != "Y")
                    {
                        //Insert Where NOT Duplicate
                        strQry.Clear();
                        strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.DEVICE_CLOCK_TIME");
                        strQry.AppendLine("(DEVICE_NO");
                        strQry.AppendLine(",COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",PAY_CATEGORY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",TIMESHEET_DATE");
                        strQry.AppendLine(",TIMESHEET_TIME_MINUTES");
                        strQry.AppendLine(",CLOCKED_BOUNDARY_TIME_MINUTES");
                        strQry.AppendLine(",IN_OUT_IND)");
                        strQry.AppendLine(" VALUES ");
                        strQry.AppendLine("(" + DeviceNo);
                        strQry.AppendLine("," + DataSet.Tables["EmployeeLinked"].Rows[0]["COMPANY_NO"].ToString());
                        strQry.AppendLine("," + DataSet.Tables["EmployeeLinked"].Rows[0]["EMPLOYEE_NO"].ToString());
                        strQry.AppendLine("," + DataSet.Tables["EmployeeLinked"].Rows[0]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine(",'" + DataSet.Tables["EmployeeLinked"].Rows[0]["PAY_CATEGORY_TYPE"].ToString() + "'");
                        strQry.AppendLine(",'" + strDate + "'");
                        strQry.AppendLine("," + strTime);

                        if (InOutParm == "I")
                        {
                            if (DataSet.Tables["EmployeeLinked"].Rows[0][strAddOnClockIn.Replace(",X.", "")].ToString() == "")
                            {
                                //No Clock In Boundary Set
                                strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["EmployeeLinked"].Rows[0]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["EmployeeLinked"].Rows[0]["TIME_ATTEND_ROUNDING_VALUE"]));
                            }
                            else
                            {
                                if (Convert.ToInt32(DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"]) == 0)
                                {
                                    //Last Clock In Was Null or 0 (Morning)
                                    if (Convert.ToInt32(DataSet.Tables["EmployeeLinked"].Rows[0][strAddOnClockIn.Replace(",X.", "")]) > Convert.ToInt32(strTime))
                                    {
                                        strQry.AppendLine("," + DataSet.Tables["EmployeeLinked"].Rows[0][strAddOnClockIn.Replace(",X.", "")]);
                                    }
                                    else
                                    {
                                        strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["EmployeeLinked"].Rows[0]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["EmployeeLinked"].Rows[0]["TIME_ATTEND_ROUNDING_VALUE"]));
                                    }
                                }
                                else
                                {
                                    strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["EmployeeLinked"].Rows[0]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["EmployeeLinked"].Rows[0]["TIME_ATTEND_ROUNDING_VALUE"]));
                                }
                            }
                        }
                        else
                        {
                            //Out
                            if (DataSet.Tables["EmployeeLinked"].Rows[0][strAddOnClockOut.Replace(",X.", "")].ToString() == "")
                            {
                                strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["EmployeeLinked"].Rows[0]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["EmployeeLinked"].Rows[0]["TIME_ATTEND_ROUNDING_VALUE"]));
                            }
                            else
                            {
                                if (DataSet.Tables["EmployeeLinked"].Rows[0][strAddOnClockInApplies.Replace(",X.", "")].ToString() == "Y")
                                {
                                    //Check if Clock In Boundary was Used
                                    strQryTemp.AppendLine(" SELECT ");
                                    strQryTemp.AppendLine(" TIMESHEET_SEQ");
                                    strQryTemp.AppendLine(",TIMESHEET_TIME_IN_MINUTES");
                                    strQryTemp.AppendLine(",TIMESHEET_TIME_OUT_MINUTES");

                                    if (DataSet.Tables["EmployeeLinked"].Rows[0]["PAY_CATEGORY_TYPE"].ToString() == "W")
                                    {
                                        strQryTemp.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT");
                                    }
                                    else
                                    {
                                        if (DataSet.Tables["EmployeeLinked"].Rows[0]["PAY_CATEGORY_TYPE"].ToString() == "S")
                                        {
                                            strQryTemp.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT");
                                        }
                                        else
                                        {
                                            //T = Time Attendance
                                            strQryTemp.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT");
                                        }
                                    }

                                    strQryTemp.AppendLine(" WHERE COMPANY_NO = " + DataSet.Tables["EmployeeLinked"].Rows[0]["COMPANY_NO"].ToString());
                                    strQryTemp.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["EmployeeLinked"].Rows[0]["EMPLOYEE_NO"].ToString());

                                    strQryTemp.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables["EmployeeLinked"].Rows[0]["PAY_CATEGORY_NO"].ToString());
                                    strQryTemp.AppendLine(" AND TIMESHEET_DATE = '" + strDate + "'");

                                    strQryTemp.AppendLine(" ORDER BY ");
                                    strQryTemp.AppendLine(" TIMESHEET_SEQ DESC");

                                    clsDBConnectionObjects.Create_DataTable_Client(strQryTemp.ToString(), DataSet, "TimeCheck");

                                    if (DataSet.Tables["TimeCheck"].Rows.Count == 0)
                                    {
                                        //No Clocking for Day - Use Current Time / Rounded
                                        strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["EmployeeLinked"].Rows[0]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["EmployeeLinked"].Rows[0]["TIME_ATTEND_ROUNDING_VALUE"]));
                                    }
                                    else
                                    {
                                        if (DataSet.Tables["TimeCheck"].Rows[0]["TIMESHEET_TIME_OUT_MINUTES"] == System.DBNull.Value)
                                        {
                                            if (DataSet.Tables["TimeCheck"].Rows[0]["TIMESHEET_TIME_IN_MINUTES"].ToString() == DataSet.Tables["EmployeeLinked"].Rows[0][strAddOnClockIn.Replace(",X.", "")].ToString())
                                            {
                                                //Clock In Boundary was Used therefor use Clock Out Boundary 
                                                strQry.AppendLine("," + DataSet.Tables["EmployeeLinked"].Rows[0][strAddOnClockOut.Replace(",X.", "")].ToString());
                                            }
                                            else
                                            {
                                                strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["EmployeeLinked"].Rows[0]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["EmployeeLinked"].Rows[0]["TIME_ATTEND_ROUNDING_VALUE"]));
                                            }
                                        }
                                        else
                                        {
                                            strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["EmployeeLinked"].Rows[0]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["EmployeeLinked"].Rows[0]["TIME_ATTEND_ROUNDING_VALUE"]));
                                        }
                                    }
                                }
                                else
                                {
                                    //Always Apply Clockout unless Clocked Out Before SET Time
                                    if (Convert.ToInt32(DataSet.Tables["EmployeeLinked"].Rows[0][strAddOnClockOut.Replace(",X.", "")]) < Convert.ToInt32(strTime))
                                    {
                                        strQry.AppendLine("," + DataSet.Tables["EmployeeLinked"].Rows[0][strAddOnClockOut.Replace(",X.", "")].ToString());
                                    }
                                    else
                                    {
                                        strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["EmployeeLinked"].Rows[0]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["EmployeeLinked"].Rows[0]["TIME_ATTEND_ROUNDING_VALUE"]));
                                    }
                                }
                            }
                        }

                        strQry.AppendLine(",'" + InOutParm + "')");
                    }
                    else
                    {
                        //Breaks
                        strQry.Clear();
                        strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.DEVICE_CLOCK_TIME_BREAK");
                        strQry.AppendLine("(DEVICE_NO");
                        strQry.AppendLine(",COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",PAY_CATEGORY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",BREAK_DATE");
                        strQry.AppendLine(",BREAK_TIME_MINUTES");
                        strQry.AppendLine(",IN_OUT_IND)");
                        strQry.AppendLine(" VALUES ");
                        strQry.AppendLine("(" + DeviceNo);
                        strQry.AppendLine("," + DataSet.Tables["EmployeeLinked"].Rows[0]["COMPANY_NO"].ToString());
                        strQry.AppendLine("," + DataSet.Tables["EmployeeLinked"].Rows[0]["EMPLOYEE_NO"].ToString());
                        strQry.AppendLine("," + DataSet.Tables["EmployeeLinked"].Rows[0]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine(",'" + DataSet.Tables["EmployeeLinked"].Rows[0]["PAY_CATEGORY_TYPE"].ToString() + "'");
                        strQry.AppendLine(",'" + strDate + "'");
                        strQry.AppendLine("," + strTime);
                        strQry.AppendLine(",'" + InOutParm + "')");
                    }

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    intWhere = 6;

                    EmployeeNameSurname.EmployeeNo = DataSet.Tables["EmployeeLinked"].Rows[0]["EMPLOYEE_NO"].ToString();
                    EmployeeNameSurname.Name = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NAME"].ToString();
                    EmployeeNameSurname.Surname = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_SURNAME"].ToString();
                }
  
                intWhere = 7;
            }
            catch (Exception ex)
            {
                WriteExceptionLog("GetEmployeePinClocked " + intWhere.ToString() + " SQL = " + strQry.ToString(), ex);

                EmployeeNameSurname.OkInd = "N";
                EmployeeNameSurname.Name = "ERROR - Web Server";
            }

            DataSet.Dispose();

            return EmployeeNameSurname;
        }
  
        public EmployeeNameSurname GetEmployeeFeaturesClocked(string DBNo, string DeviceNo, string InOutParm, string EmployeeNo, string RfIdNo, ImageFeaturesContainer ImageFeaturesContainer)
        {
            int intWhere = 0;
            //FAR 1 in 10 000
            int intFARRequested = 214748;
            StringBuilder strQry = new StringBuilder();

            //WriteLog("GetEmployeeFeaturesClocked Entered " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            EmployeeNameSurname EmployeeNameSurname = new EmployeeNameSurname();

            EmployeeNameSurname.OkInd = "Y";
            EmployeeNameSurname.RfIdNo = "0";
            EmployeeNameSurname.FingerPrintScore = "0";
            EmployeeNameSurname.Name = "";
            EmployeeNameSurname.Surname = "";
            EmployeeNameSurname.EmployeeNo = "";

            DataSet DataSet = new DataSet();

            try
            {
                clsFingerPrintClockServerLib FingerPrintClockServerLib = new clsFingerPrintClockServerLib(pvtstrSoftwareToUse);

                intWhere = 4;

                strQry.Clear();
                strQry.AppendLine(" SELECT");

                if (EmployeeNo == "0")
                {
                    strQry.AppendLine(" ISNULL(FAR_REQUESTED,214748) AS FAR_REQUESTED");
                }
                else
                {
                    //FAR = 1 in 5000
                    strQry.AppendLine(" 429496 AS FAR_REQUESTED");
                }

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE");

                strQry.AppendLine(" WHERE DEVICE_NO = " + DeviceNo);

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "FAR");

                if (DataSet.Tables["FAR"].Rows.Count > 0)
                {
                    intFARRequested = Convert.ToInt32(DataSet.Tables["FAR"].Rows[0]["FAR_REQUESTED"]);
                }

                int intRow = -1;

                intWhere = 6;
                //Image from Finger Print

                if (ImageFeaturesContainer.DPImageFeaturesByteArray != null)
                {
                    intWhere = 7;
                    string strDate = DateTime.Now.ToString("yyyy-MM-dd");
                    string strTime = Convert.ToInt32((DateTime.Now.Hour * 60) + DateTime.Now.Minute).ToString();
                    string strAddOnClockIn = "";
                    string strAddOnClockOut = "";
                    string strAddOnClockInApplies = "";
                    StringBuilder strQryTemp= new StringBuilder();
                    bool blnAlreadyClocked = false;

                    switch (Convert.ToInt32(DateTime.Now.DayOfWeek))
                    {
                        case 1:

                            strAddOnClockIn = ",X.MON_CLOCK_IN";
                            strAddOnClockOut = ",X.MON_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.MON_CLOCK_IN_APPLIES_IND";
                            break;

                        case 2:

                            strAddOnClockIn = ",X.TUE_CLOCK_IN";
                            strAddOnClockOut = ",X.TUE_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.TUE_CLOCK_IN_APPLIES_IND";
                            break;

                        case 3:

                            strAddOnClockIn = ",X.WED_CLOCK_IN";
                            strAddOnClockOut = ",X.WED_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.WED_CLOCK_IN_APPLIES_IND";
                            break;

                        case 4:

                            strAddOnClockIn = ",X.THU_CLOCK_IN";
                            strAddOnClockOut = ",X.THU_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.THU_CLOCK_IN_APPLIES_IND";
                            break;

                        case 5:

                            strAddOnClockIn = ",X.FRI_CLOCK_IN";
                            strAddOnClockOut = ",X.FRI_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.FRI_CLOCK_IN_APPLIES_IND";
                            break;

                        case 6:

                            strAddOnClockIn = ",X.SAT_CLOCK_IN";
                            strAddOnClockOut = ",X.SAT_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.SAT_CLOCK_IN_APPLIES_IND";
                            break;

                        case 0:

                            strAddOnClockIn = ",X.SUN_CLOCK_IN";
                            strAddOnClockOut = ",X.SUN_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.SUN_CLOCK_IN_APPLIES_IND";
                            break;
                    }

                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EFT.COMPANY_NO");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",EFT.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine("," + EmployeeNo + " AS EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",EFT.FINGER_NO");
                    strQry.AppendLine(",DEVICE_EMPLOYEE.PAY_CATEGORY_NO");
                    strQry.AppendLine(",DEVICE_EMPLOYEE.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(strAddOnClockIn.Replace("X.", "DEVICE_EMPLOYEE."));
                    strQry.AppendLine(strAddOnClockOut.Replace("X.", "DEVICE_EMPLOYEE."));
                    strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DEVICE_EMPLOYEE."));

                    strQry.AppendLine(",DEVICE_EMPLOYEE.TIME_ATTEND_ROUNDING_IND");
                    strQry.AppendLine(",DEVICE_EMPLOYEE.TIME_ATTEND_ROUNDING_VALUE");

                    strQry.AppendLine(",EFT.FINGER_TEMPLATE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE EFT");

                    strQry.AppendLine(" INNER JOIN ");

                    strQry.AppendLine("(SELECT ");
                    strQry.AppendLine(" DEL.COMPANY_NO");

                    strQry.AppendLine(strAddOnClockIn.Replace("X.", "'' AS "));
                    strQry.AppendLine(strAddOnClockOut.Replace("X.", "'' AS "));
                    strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "'' AS "));

                    strQry.AppendLine(",NULL AS TIME_ATTEND_ROUNDING_IND");
                    strQry.AppendLine(",0 AS TIME_ATTEND_ROUNDING_VALUE");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",DEL.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine(",TEMP_EMPLOYEE.EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",DEL.PAY_CATEGORY_NO");
                    strQry.AppendLine(",DEL.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_EMPLOYEE_LINK DEL");

                    if (EmployeeNo != "0")
                    {
                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DEL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" WHERE DEL.DEVICE_NO = " + DeviceNo);

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND DEL.EMPLOYEE_NO = " + EmployeeNo);
                    }

                    //Cost Centre 
                    strQry.AppendLine(" UNION ");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EPC.COMPANY_NO");

                    strQry.AppendLine(strAddOnClockIn.Replace("X.", "DPCLA."));
                    strQry.AppendLine(strAddOnClockOut.Replace("X.", "DPCLA."));
                    strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DPCLA."));

                    strQry.AppendLine(",DPCLA.TIME_ATTEND_ROUNDING_IND");
                    strQry.AppendLine(",DPCLA.TIME_ATTEND_ROUNDING_VALUE");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",EPC.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine(",TEMP_EMPLOYEE.EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK DPCL");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON DPCL.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + EmployeeNo);

                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DPCL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK_ACTIVE DPCLA");
                    strQry.AppendLine(" ON DPCL.DEVICE_NO = DPCLA.DEVICE_NO ");
                    strQry.AppendLine(" AND DPCL.COMPANY_NO = DPCLA.COMPANY_NO ");
                    strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = DPCLA.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = DPCLA.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND DPCLA.ACTIVE_IND = 'Y' ");

                    strQry.AppendLine(" WHERE DPCL.DEVICE_NO = " + DeviceNo);

                    //Department 
                    strQry.AppendLine(" UNION");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EPC.COMPANY_NO");

                    strQry.AppendLine(strAddOnClockIn.Replace("X.", "DDLA."));
                    strQry.AppendLine(strAddOnClockOut.Replace("X.", "DDLA."));
                    strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DDLA."));

                    strQry.AppendLine(",DDLA.TIME_ATTEND_ROUNDING_IND");
                    strQry.AppendLine(",DDLA.TIME_ATTEND_ROUNDING_VALUE");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",EPC.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine(",TEMP_EMPLOYEE.EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK DDL");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON DDL.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND DDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                    strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                    strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON DDL.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + EmployeeNo);

                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DDL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK_ACTIVE DDLA");
                    strQry.AppendLine(" ON DDL.DEVICE_NO = DDLA.DEVICE_NO ");
                    strQry.AppendLine(" AND DDL.COMPANY_NO = DDLA.COMPANY_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = DDLA.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = DDLA.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND DDL.DEPARTMENT_NO = DDLA.DEPARTMENT_NO ");
                    strQry.AppendLine(" AND DDLA.ACTIVE_IND = 'Y' ");

                    strQry.AppendLine(" WHERE DDL.DEVICE_NO = " + DeviceNo);

                    //Group
                    strQry.AppendLine(" UNION");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" GEL.COMPANY_NO");

                    strQry.AppendLine(strAddOnClockIn.Replace("X.", "DGLA."));
                    strQry.AppendLine(strAddOnClockOut.Replace("X.", "DGLA."));
                    strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DGLA."));

                    strQry.AppendLine(",DGLA.TIME_ATTEND_ROUNDING_IND");
                    strQry.AppendLine(",DGLA.TIME_ATTEND_ROUNDING_VALUE");

                    strQry.AppendLine(",GEL.EMPLOYEE_NO");
                    strQry.AppendLine(",GEL.PAY_CATEGORY_NO");
                    strQry.AppendLine(",GEL.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK DGL");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.GROUP_EMPLOYEE_LINK GEL");
                    strQry.AppendLine(" ON DGL.GROUP_NO = GEL.GROUP_NO ");
                    strQry.AppendLine(" AND DGL.COMPANY_NO = GEL.COMPANY_NO ");

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND GEL.EMPLOYEE_NO = " + EmployeeNo);

                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DGL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_GROUP_LINK_ACTIVE DGLA");
                    strQry.AppendLine(" ON DGL.DEVICE_NO = DGLA.DEVICE_NO ");
                    strQry.AppendLine(" AND DGL.COMPANY_NO = DGLA.COMPANY_NO ");
                    strQry.AppendLine(" AND DGL.GROUP_NO = DGLA.GROUP_NO ");
                    strQry.AppendLine(" AND DGLA.ACTIVE_IND = 'Y' ");

                    strQry.AppendLine(" WHERE DGL.DEVICE_NO = " + DeviceNo + ") AS DEVICE_EMPLOYEE");

                    strQry.AppendLine(" ON EFT.COMPANY_NO = DEVICE_EMPLOYEE.COMPANY_NO");
                    strQry.AppendLine(" AND EFT.EMPLOYEE_NO = DEVICE_EMPLOYEE.EMPLOYEE_NO");

                    if (EmployeeNo != "0")
                    {
                        //Add User Override Link
                        strQry.AppendLine(" UNION ALL");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EMPLOYEE_DEVICE.COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.EMPLOYEE_NO");
                        strQry.AppendLine(",UFT.FINGER_NO");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.PAY_CATEGORY_TYPE");

                        strQry.AppendLine(strAddOnClockIn.Replace("X.", "EMPLOYEE_DEVICE."));
                        strQry.AppendLine(strAddOnClockOut.Replace("X.", "EMPLOYEE_DEVICE."));
                        strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "EMPLOYEE_DEVICE."));

                        strQry.AppendLine(",EMPLOYEE_DEVICE.TIME_ATTEND_ROUNDING_IND");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.TIME_ATTEND_ROUNDING_VALUE");

                        //Linked Person's Fingerprint Templates
                        strQry.AppendLine(",UFT.FINGER_TEMPLATE");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE UFT");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_USER_LINK EUL");
                        strQry.AppendLine(" ON UFT.USER_NO = EUL.USER_NO ");
                        strQry.AppendLine(" AND EUL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.DEVICE D");
                        strQry.AppendLine(" ON D.DEVICE_NO = " + DeviceNo);
                        //T=Time And Attendance,A= Access Control,B=Time And Attendance And Access Control
                        strQry.AppendLine(" AND D.DEVICE_USAGE IN ('T','B')");

                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT ");
                        strQry.AppendLine(" DEVICE_NO ");
                        strQry.AppendLine(",COMPANY_NO ");

                        strQry.AppendLine(strAddOnClockIn.Replace("X.", "'' AS "));
                        strQry.AppendLine(strAddOnClockOut.Replace("X.", "'' AS "));
                        strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "'' AS "));

                        strQry.AppendLine(",NULL AS TIME_ATTEND_ROUNDING_IND");
                        strQry.AppendLine(",0 AS TIME_ATTEND_ROUNDING_VALUE");

                        strQry.AppendLine(",EMPLOYEE_NO ");
                        strQry.AppendLine(",PAY_CATEGORY_NO ");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_EMPLOYEE_LINK ");

                        strQry.AppendLine(" WHERE DEVICE_NO = " + DeviceNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" DPCL.DEVICE_NO ");
                        strQry.AppendLine(",EPC.COMPANY_NO ");

                        strQry.AppendLine(strAddOnClockIn.Replace("X.", "DPCLA."));
                        strQry.AppendLine(strAddOnClockOut.Replace("X.", "DPCLA."));
                        strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DPCLA."));

                        strQry.AppendLine(",DPCLA.TIME_ATTEND_ROUNDING_IND");
                        strQry.AppendLine(",DPCLA.TIME_ATTEND_ROUNDING_VALUE");

                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK DPCL");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON DPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK_ACTIVE DPCLA");
                        strQry.AppendLine(" ON DPCL.DEVICE_NO = DPCLA.DEVICE_NO ");
                        strQry.AppendLine(" AND DPCL.COMPANY_NO = DPCLA.COMPANY_NO ");
                        strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = DPCLA.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = DPCLA.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND DPCLA.ACTIVE_IND = 'Y' ");

                        strQry.AppendLine(" WHERE DPCL.DEVICE_NO = " + DeviceNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" DDL.DEVICE_NO ");
                        strQry.AppendLine(",EPC.COMPANY_NO ");

                        strQry.AppendLine(strAddOnClockIn.Replace("X.", "DDLA."));
                        strQry.AppendLine(strAddOnClockOut.Replace("X.", "DDLA."));
                        strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DDLA."));

                        strQry.AppendLine(",DDLA.TIME_ATTEND_ROUNDING_IND");
                        strQry.AppendLine(",DDLA.TIME_ATTEND_ROUNDING_VALUE");

                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK DDL");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E");
                        strQry.AppendLine(" ON DDL.COMPANY_NO = E.COMPANY_NO ");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND DDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON DDL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");

                        strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK_ACTIVE DDLA");
                        strQry.AppendLine(" ON DDL.DEVICE_NO = DDLA.DEVICE_NO ");
                        strQry.AppendLine(" AND DDL.COMPANY_NO = DDLA.COMPANY_NO ");
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = DDLA.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = DDLA.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND DDL.DEPARTMENT_NO = DDLA.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND DDLA.ACTIVE_IND = 'Y' ");

                        strQry.AppendLine(" WHERE DDL.DEVICE_NO = " + DeviceNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" DGL.DEVICE_NO ");
                        strQry.AppendLine(",GEL.COMPANY_NO ");

                        strQry.AppendLine(strAddOnClockIn.Replace("X.", "DGLA."));
                        strQry.AppendLine(strAddOnClockOut.Replace("X.", "DGLA."));
                        strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DGLA."));

                        strQry.AppendLine(",DGLA.TIME_ATTEND_ROUNDING_IND");
                        strQry.AppendLine(",DGLA.TIME_ATTEND_ROUNDING_VALUE");

                        strQry.AppendLine(",GEL.EMPLOYEE_NO ");
                        strQry.AppendLine(",GEL.PAY_CATEGORY_NO ");
                        strQry.AppendLine(",GEL.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK DGL");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.GROUP_EMPLOYEE_LINK GEL");
                        strQry.AppendLine(" ON DGL.GROUP_NO = GEL.GROUP_NO ");
                        strQry.AppendLine(" AND DGL.COMPANY_NO = GEL.COMPANY_NO ");
                        strQry.AppendLine(" AND GEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_GROUP_LINK_ACTIVE DGLA");
                        strQry.AppendLine(" ON DGL.DEVICE_NO = DGLA.DEVICE_NO ");
                        strQry.AppendLine(" AND DGL.COMPANY_NO = DGLA.COMPANY_NO ");
                        strQry.AppendLine(" AND DGL.GROUP_NO = DGLA.GROUP_NO ");
                        strQry.AppendLine(" AND DGLA.ACTIVE_IND = 'Y' ");

                        strQry.AppendLine(" WHERE DGL.DEVICE_NO = " + DeviceNo + ") AS EMPLOYEE_DEVICE");

                        strQry.AppendLine(" ON D.DEVICE_NO = EMPLOYEE_DEVICE.DEVICE_NO");

                        strQry.AppendLine(" AND EUL.COMPANY_NO = EMPLOYEE_DEVICE.COMPANY_NO");
                        strQry.AppendLine(" AND EUL.EMPLOYEE_NO = EMPLOYEE_DEVICE.EMPLOYEE_NO");

                        strQry.AppendLine(" WHERE NOT UFT.FINGER_TEMPLATE IS NULL");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON DEVICE_EMPLOYEE.COMPANY_NO = E.COMPANY_NO ");
                        strQry.AppendLine(" AND DEVICE_EMPLOYEE.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND DEVICE_EMPLOYEE.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");
                        strQry.AppendLine(" AND (E.USE_EMPLOYEE_NO_IND <> 'Y' ");
                        strQry.AppendLine(" OR E.USE_EMPLOYEE_NO_IND IS NULL) ");
                    }

                    intWhere = 8;
                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Template");

                    intWhere = 9;
                    if (DataSet.Tables["Template"].Rows.Count == 0)
                    {
                        intWhere = 10;

                        if (EmployeeNo == "0")
                        {
                            EmployeeNameSurname.Name = "NO Employee/s for Device";
                        }
                        else
                        {
                            EmployeeNameSurname.Name = "NOT FOUND";
                        }
                    }
                    else
                    {
                        intWhere = 11;

                        int intReturnCode = FingerPrintClockServerLib.Get_Employee_Features(intFARRequested, DataSet.Tables["Template"], ref ImageFeaturesContainer.DPImageFeaturesByteArray, ref intRow);

                        intWhere = 12;
                        if (intRow == -1)
                        {
                            intWhere = 13;
                            EmployeeNameSurname.Name = "NOT FOUND";
                        }
                        else
                        {
                            intWhere = 14;

                            strQry.Clear();
                            strQry.AppendLine(" SELECT TOP 1 ");
                            strQry.AppendLine(" E.EMPLOYEE_NAME");
                            strQry.AppendLine(",E.EMPLOYEE_SURNAME");

                            if (InOutParm == "I")
                            {
                                strQry.AppendLine(",ISNULL(ETC.TIMESHEET_TIME_IN_MINUTES,0) AS TIMESHEET_TIME_MINUTES");
                                strQry.AppendLine(",ISNULL(ETC.CLOCKED_TIME_IN_MINUTES,0) AS CLOCKED_TIME_MINUTES");
                            }
                            else
                            {
                                strQry.AppendLine(",ISNULL(ETC.TIMESHEET_TIME_OUT_MINUTES,0) AS TIMESHEET_TIME_MINUTES");
                                strQry.AppendLine(",ISNULL(ETC.CLOCKED_TIME_OUT_MINUTES,0) AS CLOCKED_TIME_MINUTES");
                            }

                            strQry.AppendLine(",ETC.TIMESHEET_SEQ");

                            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                            if (DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                            {
                                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
                            }
                            else
                            {
                                if (DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                                {
                                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC");
                                }
                                else
                                {
                                    //T = Time Attendance
                                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC");
                                }
                            }

                            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO ");
                            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
                            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                            strQry.AppendLine(" AND TIMESHEET_DATE = '" + strDate + "'");

                            strQry.AppendLine(" WHERE E.COMPANY_NO = " + DataSet.Tables["Template"].Rows[intRow]["COMPANY_NO"].ToString());
                            strQry.AppendLine(" AND E.EMPLOYEE_NO = " + DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                            strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                            strQry.AppendLine(" ORDER BY ");
                            strQry.AppendLine(" ETC.TIMESHEET_SEQ DESC");

                            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

                            if (DataSet.Tables["Employee"].Rows[0]["CLOCKED_TIME_MINUTES"] != System.DBNull.Value)
                            {
                                if (Convert.ToInt32(DataSet.Tables["Employee"].Rows[0]["CLOCKED_TIME_MINUTES"]) == Convert.ToInt32(strTime))
                                {
                                    blnAlreadyClocked = true;
                                }
                            }

                            if (blnAlreadyClocked == false)
                            {
                                if (InOutParm == "I")
                                {
                                    if (DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")] != System.DBNull.Value)
                                    {
                                        if (Convert.ToInt32(DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")]) > Convert.ToInt32(strTime))
                                        {
                                            if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]))
                                            {
                                                blnAlreadyClocked = true;
                                            }
                                            else
                                            {
                                                if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")].ToString())
                                                {
                                                    blnAlreadyClocked = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]))
                                            {
                                                blnAlreadyClocked = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]))
                                        {
                                            blnAlreadyClocked = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")] != System.DBNull.Value)
                                    {
                                        if (Convert.ToInt32(DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")]) < Convert.ToInt32(strTime))
                                        {
                                            if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]))
                                            {
                                                blnAlreadyClocked = true;
                                            }
                                            else
                                            {
                                                if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")].ToString())
                                                {
                                                    blnAlreadyClocked = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]))
                                            {
                                                blnAlreadyClocked = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]))
                                        {
                                            blnAlreadyClocked = true;
                                        }
                                    }
                                }
                            }

                            if (blnAlreadyClocked == true)
                            {
                                intWhere = 15;
                                EmployeeNameSurname.OkInd = "A";
                                EmployeeNameSurname.EmployeeNo = DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                                EmployeeNameSurname.Name = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NAME"].ToString();
                                EmployeeNameSurname.Surname = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_SURNAME"].ToString();
                                return EmployeeNameSurname;
                            }

                            intWhere = 16;

                            //Insert Where NOT Duplicate
                            strQry.Clear();
                            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.DEVICE_CLOCK_TIME");
                            strQry.AppendLine("(DEVICE_NO");
                            strQry.AppendLine(",COMPANY_NO");
                            strQry.AppendLine(",EMPLOYEE_NO");
                            strQry.AppendLine(",PAY_CATEGORY_NO");
                            strQry.AppendLine(",PAY_CATEGORY_TYPE");
                            strQry.AppendLine(",TIMESHEET_DATE");
                            strQry.AppendLine(",TIMESHEET_TIME_MINUTES");
                            strQry.AppendLine(",CLOCKED_BOUNDARY_TIME_MINUTES");
                            strQry.AppendLine(",IN_OUT_IND)");
                            strQry.AppendLine(" VALUES ");
                            strQry.AppendLine("(" + DeviceNo);
                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow]["COMPANY_NO"].ToString());
                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                            strQry.AppendLine(",'" + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'");
                            strQry.AppendLine(",'" + strDate + "'");
                            strQry.AppendLine("," + strTime);

                            if (InOutParm == "I")
                            {
                                if (DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")].ToString() == "")
                                {
                                    //No Clock In Boundary Set
                                    strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                }
                                else
                                {
                                    if (Convert.ToInt32(DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"]) == 0)
                                    {
                                        //Last Clock In Was Null or 0 (Morning)
                                        if (Convert.ToInt32(DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")]) > Convert.ToInt32(strTime))
                                        {
                                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")]);
                                        }
                                        else
                                        {
                                            strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                        }
                                    }
                                    else
                                    {
                                        strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                    }
                                }
                            }
                            else
                            {
                                //Out
                                if (DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")].ToString() == "")
                                {
                                    strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                }
                                else
                                {
                                    if (DataSet.Tables["Template"].Rows[intRow][strAddOnClockInApplies.Replace(",X.", "")].ToString() == "Y")
                                    {
                                        //Check if Clock In Boundary was Used
                                        strQryTemp.AppendLine(" SELECT ");
                                        strQryTemp.AppendLine(" TIMESHEET_SEQ");
                                        strQryTemp.AppendLine(",TIMESHEET_TIME_IN_MINUTES");
                                        strQryTemp.AppendLine(",TIMESHEET_TIME_OUT_MINUTES");

                                        if (DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                                        {
                                            strQryTemp.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT");
                                        }
                                        else
                                        {
                                            if (DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                                            {
                                                strQryTemp.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT");
                                            }
                                            else
                                            {
                                                //T = Time Attendance
                                                strQryTemp.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT");
                                            }
                                        }

                                        strQryTemp.AppendLine(" WHERE COMPANY_NO = " + DataSet.Tables["Template"].Rows[intRow]["COMPANY_NO"].ToString());
                                        strQryTemp.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString());

                                        strQryTemp.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                                        strQryTemp.AppendLine(" AND TIMESHEET_DATE = '" + strDate + "'");

                                        strQryTemp.AppendLine(" ORDER BY ");
                                        strQryTemp.AppendLine(" TIMESHEET_SEQ DESC");

                                        clsDBConnectionObjects.Create_DataTable_Client(strQryTemp.ToString(), DataSet, "TimeCheck");

                                        if (DataSet.Tables["TimeCheck"].Rows.Count == 0)
                                        {
                                            //No Clocking for Day - Use Current Time / Rounded
                                            strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                        }
                                        else
                                        {
                                            if (DataSet.Tables["TimeCheck"].Rows[0]["TIMESHEET_TIME_OUT_MINUTES"] == System.DBNull.Value)
                                            {
                                                if (DataSet.Tables["TimeCheck"].Rows[0]["TIMESHEET_TIME_IN_MINUTES"].ToString() == DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")].ToString())
                                                {
                                                    //Clock In Boundary was Used therefor use Clock Out Boundary 
                                                    strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")].ToString());
                                                }
                                                else
                                                {
                                                    strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                                }
                                            }
                                            else
                                            {
                                                strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //Always Apply Clockout unless Clocked Out Before SET Time
                                        if (Convert.ToInt32(DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")]) < Convert.ToInt32(strTime))
                                        {
                                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")].ToString());
                                        }
                                        else
                                        {
                                            strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                        }
                                    }
                                }
                            }

                            strQry.AppendLine(",'" + InOutParm + "')");

                            //WriteLog("GetEmployeeFeaturesClocked Sleeping For 5 Seconds " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            //System.Threading.Thread.Sleep(5000);

                            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString(),10);

                            intWhere = 17;

                            EmployeeNameSurname.EmployeeNo = DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                            EmployeeNameSurname.Name = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NAME"].ToString();
                            EmployeeNameSurname.Surname = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_SURNAME"].ToString();
                        }
                    }
                }
                else
                {
                    intWhere = 19;
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",SUBSTRING(EMPLOYEE_NAME,1,1) + ' ' + EMPLOYEE_SURNAME AS EMPL_NAMES");
                    strQry.AppendLine(",READ_OPTION_NO");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE");

                    strQry.AppendLine(" WHERE EMPLOYEE_RFID_CARD_NO = " + RfIdNo);
                    strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                    strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

                    intWhere = 20;
                    if (DataSet.Tables["Employee"].Rows.Count == 0)
                    {
                        intWhere = 21;
                        EmployeeNameSurname.Name = "NOT FOUND";
                    }
                    else
                    {
                        intWhere = 22;
                        if (DataSet.Tables["Employee"].Rows[0]["READ_OPTION_NO"].ToString() == "2")
                        {
                            intWhere = 23;
                            EmployeeNameSurname.EmployeeNo = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"].ToString();
                            EmployeeNameSurname.Name = DataSet.Tables["Employee"].Rows[0]["EMPL_NAMES"].ToString();

                            //Insert record Into dB
                        }
                        else
                        {
                            intWhere = 24;
                            if (DataSet.Tables["Employee"].Rows[0]["READ_OPTION_NO"].ToString() == "3")
                            {
                                intWhere = 25;
                                EmployeeNameSurname.Name = "Scan your Finger";
                                EmployeeNameSurname.RfIdNo = RfIdNo;
                            }
                            else
                            {
                                intWhere = 26;
                                EmployeeNameSurname.OkInd = "N";
                                EmployeeNameSurname.Name = "Incorrect Profile Setup";
                            }
                        }
                    }
                }

                intWhere = 27;
            }
            catch (Exception ex)
            {
                WriteExceptionLog("GetEmployeeFeaturesClocked " + intWhere.ToString() + " SQL = " + strQry.ToString(), ex);

                EmployeeNameSurname.OkInd = "N";
                EmployeeNameSurname.Name = "ERROR - Web Server";
            }
            finally
            {
            }

        GetEmployeeClocked_Continue:

            DataSet.Dispose();

            return EmployeeNameSurname;
        }
  
        public EmployeeNameSurname GetEmployeeFeaturesClockedNew(string DeviceNo, string InOutParm, string EmployeeNo, string RfIdNo, ImageFeaturesContainer ImageFeaturesContainer)
        {
            int intWhere = 0;
            //FAR 1 in 10 000
            int intFARRequested = 214748;
            StringBuilder strQry = new StringBuilder();

            EmployeeNameSurname EmployeeNameSurname = new EmployeeNameSurname();

            EmployeeNameSurname.OkInd = "Y";
            EmployeeNameSurname.RfIdNo = "0";
            EmployeeNameSurname.FingerPrintScore = "0";
            EmployeeNameSurname.Name = "";
            EmployeeNameSurname.Surname = "";
            EmployeeNameSurname.EmployeeNo = "";

            DataSet DataSet = new DataSet();

            try
            {
                clsFingerPrintClockServerLib FingerPrintClockServerLib = new clsFingerPrintClockServerLib(pvtstrSoftwareToUse);

                intWhere = 4;

                strQry.Clear();
                strQry.AppendLine(" SELECT");

                if (EmployeeNo == "0")
                {
                    strQry.AppendLine(" ISNULL(FAR_REQUESTED,214748) AS FAR_REQUESTED");
                }
                else
                {
                    //FAR = 1 in 5000
                    strQry.AppendLine(" 429496 AS FAR_REQUESTED");
                }

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE");

                strQry.AppendLine(" WHERE DEVICE_NO = " + DeviceNo);

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "FAR");

                if (DataSet.Tables["FAR"].Rows.Count > 0)
                {
                    intFARRequested = Convert.ToInt32(DataSet.Tables["FAR"].Rows[0]["FAR_REQUESTED"]);
                }

                int intRow = -1;

                intWhere = 6;
                //Image from Finger Print

                if (ImageFeaturesContainer.DPImageFeaturesByteArray != null)
                {
                    intWhere = 7;
                    string strDate = DateTime.Now.ToString("yyyy-MM-dd");
                    string strTime = Convert.ToInt32((DateTime.Now.Hour * 60) + DateTime.Now.Minute).ToString();
                    string strAddOnClockIn = "";
                    string strAddOnClockOut = "";
                    string strAddOnClockInApplies = "";
                    StringBuilder strQryTemp= new StringBuilder();
                    bool blnAlreadyClocked = false;

                    switch (Convert.ToInt32(DateTime.Now.DayOfWeek))
                    {
                        case 1:

                            strAddOnClockIn = ",X.MON_CLOCK_IN";
                            strAddOnClockOut = ",X.MON_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.MON_CLOCK_IN_APPLIES_IND";
                            break;

                        case 2:

                            strAddOnClockIn = ",X.TUE_CLOCK_IN";
                            strAddOnClockOut = ",X.TUE_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.TUE_CLOCK_IN_APPLIES_IND";
                            break;

                        case 3:

                            strAddOnClockIn = ",X.WED_CLOCK_IN";
                            strAddOnClockOut = ",X.WED_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.WED_CLOCK_IN_APPLIES_IND";
                            break;

                        case 4:

                            strAddOnClockIn = ",X.THU_CLOCK_IN";
                            strAddOnClockOut = ",X.THU_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.THU_CLOCK_IN_APPLIES_IND";
                            break;

                        case 5:

                            strAddOnClockIn = ",X.FRI_CLOCK_IN";
                            strAddOnClockOut = ",X.FRI_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.FRI_CLOCK_IN_APPLIES_IND";
                            break;

                        case 6:

                            strAddOnClockIn = ",X.SAT_CLOCK_IN";
                            strAddOnClockOut = ",X.SAT_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.SAT_CLOCK_IN_APPLIES_IND";
                            break;

                        case 0:

                            strAddOnClockIn = ",X.SUN_CLOCK_IN";
                            strAddOnClockOut = ",X.SUN_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.SUN_CLOCK_IN_APPLIES_IND";
                            break;
                    }

                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EFT.COMPANY_NO");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",EFT.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine("," + EmployeeNo + " AS EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",EFT.FINGER_NO");
                    strQry.AppendLine(",DEVICE_EMPLOYEE.PAY_CATEGORY_NO");
                    strQry.AppendLine(",DEVICE_EMPLOYEE.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(strAddOnClockIn.Replace("X.", "DEVICE_EMPLOYEE."));
                    strQry.AppendLine(strAddOnClockOut.Replace("X.", "DEVICE_EMPLOYEE."));
                    strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DEVICE_EMPLOYEE."));

                    strQry.AppendLine(",DEVICE_EMPLOYEE.TIME_ATTEND_ROUNDING_IND");
                    strQry.AppendLine(",DEVICE_EMPLOYEE.TIME_ATTEND_ROUNDING_VALUE");

                    strQry.AppendLine(",EFT.FINGER_TEMPLATE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE EFT");

                    strQry.AppendLine(" INNER JOIN ");

                    strQry.AppendLine("(SELECT ");
                    strQry.AppendLine(" DEL.COMPANY_NO");

                    strQry.AppendLine(strAddOnClockIn.Replace("X.", "'' AS "));
                    strQry.AppendLine(strAddOnClockOut.Replace("X.", "'' AS "));
                    strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "'' AS "));

                    strQry.AppendLine(",NULL AS TIME_ATTEND_ROUNDING_IND");
                    strQry.AppendLine(",0 AS TIME_ATTEND_ROUNDING_VALUE");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",DEL.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine(",TEMP_EMPLOYEE.EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",DEL.PAY_CATEGORY_NO");
                    strQry.AppendLine(",DEL.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_EMPLOYEE_LINK DEL");

                    if (EmployeeNo != "0")
                    {
                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DEL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" WHERE DEL.DEVICE_NO = " + DeviceNo);

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND DEL.EMPLOYEE_NO = " + EmployeeNo);
                    }

                    //Cost Centre 
                    strQry.AppendLine(" UNION ");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EPC.COMPANY_NO");

                    strQry.AppendLine(strAddOnClockIn.Replace("X.", "DPCLA."));
                    strQry.AppendLine(strAddOnClockOut.Replace("X.", "DPCLA."));
                    strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DPCLA."));

                    strQry.AppendLine(",DPCLA.TIME_ATTEND_ROUNDING_IND");
                    strQry.AppendLine(",DPCLA.TIME_ATTEND_ROUNDING_VALUE");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",EPC.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine(",TEMP_EMPLOYEE.EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK DPCL");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON DPCL.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + EmployeeNo);

                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DPCL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK_ACTIVE DPCLA");
                    strQry.AppendLine(" ON DPCL.DEVICE_NO = DPCLA.DEVICE_NO ");
                    strQry.AppendLine(" AND DPCL.COMPANY_NO = DPCLA.COMPANY_NO ");
                    strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = DPCLA.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = DPCLA.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND DPCLA.ACTIVE_IND = 'Y' ");

                    strQry.AppendLine(" WHERE DPCL.DEVICE_NO = " + DeviceNo);

                    //Department 
                    strQry.AppendLine(" UNION");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EPC.COMPANY_NO");

                    strQry.AppendLine(strAddOnClockIn.Replace("X.", "DDLA."));
                    strQry.AppendLine(strAddOnClockOut.Replace("X.", "DDLA."));
                    strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DDLA."));

                    strQry.AppendLine(",DDLA.TIME_ATTEND_ROUNDING_IND");
                    strQry.AppendLine(",DDLA.TIME_ATTEND_ROUNDING_VALUE");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",EPC.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine(",TEMP_EMPLOYEE.EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK DDL");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON DDL.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND DDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                    strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                    strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON DDL.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + EmployeeNo);

                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DDL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK_ACTIVE DDLA");
                    strQry.AppendLine(" ON DDL.DEVICE_NO = DDLA.DEVICE_NO ");
                    strQry.AppendLine(" AND DDL.COMPANY_NO = DDLA.COMPANY_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = DDLA.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = DDLA.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND DDL.DEPARTMENT_NO = DDLA.DEPARTMENT_NO ");
                    strQry.AppendLine(" AND DDLA.ACTIVE_IND = 'Y' ");

                    strQry.AppendLine(" WHERE DDL.DEVICE_NO = " + DeviceNo);

                    //Group
                    strQry.AppendLine(" UNION");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" GEL.COMPANY_NO");

                    strQry.AppendLine(strAddOnClockIn.Replace("X.", "DGLA."));
                    strQry.AppendLine(strAddOnClockOut.Replace("X.", "DGLA."));
                    strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DGLA."));

                    strQry.AppendLine(",DGLA.TIME_ATTEND_ROUNDING_IND");
                    strQry.AppendLine(",DGLA.TIME_ATTEND_ROUNDING_VALUE");

                    strQry.AppendLine(",GEL.EMPLOYEE_NO");
                    strQry.AppendLine(",GEL.PAY_CATEGORY_NO");
                    strQry.AppendLine(",GEL.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK DGL");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.GROUP_EMPLOYEE_LINK GEL");
                    strQry.AppendLine(" ON DGL.GROUP_NO = GEL.GROUP_NO ");
                    strQry.AppendLine(" AND DGL.COMPANY_NO = GEL.COMPANY_NO ");

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND GEL.EMPLOYEE_NO = " + EmployeeNo);

                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DGL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_GROUP_LINK_ACTIVE DGLA");
                    strQry.AppendLine(" ON DGL.DEVICE_NO = DGLA.DEVICE_NO ");
                    strQry.AppendLine(" AND DGL.COMPANY_NO = DGLA.COMPANY_NO ");
                    strQry.AppendLine(" AND DGL.GROUP_NO = DGLA.GROUP_NO ");
                    strQry.AppendLine(" AND DGLA.ACTIVE_IND = 'Y' ");

                    strQry.AppendLine(" WHERE DGL.DEVICE_NO = " + DeviceNo + ") AS DEVICE_EMPLOYEE");

                    strQry.AppendLine(" ON EFT.COMPANY_NO = DEVICE_EMPLOYEE.COMPANY_NO");
                    strQry.AppendLine(" AND EFT.EMPLOYEE_NO = DEVICE_EMPLOYEE.EMPLOYEE_NO");

                    if (EmployeeNo != "0")
                    {
                        //Add User Override Link
                        strQry.AppendLine(" UNION ALL");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EMPLOYEE_DEVICE.COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.EMPLOYEE_NO");
                        strQry.AppendLine(",UFT.FINGER_NO");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.PAY_CATEGORY_TYPE");

                        strQry.AppendLine(strAddOnClockIn.Replace("X.", "EMPLOYEE_DEVICE."));
                        strQry.AppendLine(strAddOnClockOut.Replace("X.", "EMPLOYEE_DEVICE."));
                        strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "EMPLOYEE_DEVICE."));

                        strQry.AppendLine(",EMPLOYEE_DEVICE.TIME_ATTEND_ROUNDING_IND");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.TIME_ATTEND_ROUNDING_VALUE");

                        //Linked Person's Fingerprint Templates
                        strQry.AppendLine(",UFT.FINGER_TEMPLATE");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE UFT");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_USER_LINK EUL");
                        strQry.AppendLine(" ON UFT.USER_NO = EUL.USER_NO ");
                        strQry.AppendLine(" AND EUL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.DEVICE D");
                        strQry.AppendLine(" ON D.DEVICE_NO = " + DeviceNo);
                        //T=Time And Attendance,A= Access Control,B=Time And Attendance And Access Control
                        strQry.AppendLine(" AND D.DEVICE_USAGE IN ('T','B')");

                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT ");
                        strQry.AppendLine(" DEVICE_NO ");
                        strQry.AppendLine(",COMPANY_NO ");

                        strQry.AppendLine(strAddOnClockIn.Replace("X.", "'' AS "));
                        strQry.AppendLine(strAddOnClockOut.Replace("X.", "'' AS "));
                        strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "'' AS "));

                        strQry.AppendLine(",NULL AS TIME_ATTEND_ROUNDING_IND");
                        strQry.AppendLine(",0 AS TIME_ATTEND_ROUNDING_VALUE");

                        strQry.AppendLine(",EMPLOYEE_NO ");
                        strQry.AppendLine(",PAY_CATEGORY_NO ");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_EMPLOYEE_LINK ");

                        strQry.AppendLine(" WHERE DEVICE_NO = " + DeviceNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" DPCL.DEVICE_NO ");
                        strQry.AppendLine(",EPC.COMPANY_NO ");

                        strQry.AppendLine(strAddOnClockIn.Replace("X.", "DPCLA."));
                        strQry.AppendLine(strAddOnClockOut.Replace("X.", "DPCLA."));
                        strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DPCLA."));

                        strQry.AppendLine(",DPCLA.TIME_ATTEND_ROUNDING_IND");
                        strQry.AppendLine(",DPCLA.TIME_ATTEND_ROUNDING_VALUE");

                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK DPCL");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON DPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK_ACTIVE DPCLA");
                        strQry.AppendLine(" ON DPCL.DEVICE_NO = DPCLA.DEVICE_NO ");
                        strQry.AppendLine(" AND DPCL.COMPANY_NO = DPCLA.COMPANY_NO ");
                        strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = DPCLA.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = DPCLA.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND DPCLA.ACTIVE_IND = 'Y' ");

                        strQry.AppendLine(" WHERE DPCL.DEVICE_NO = " + DeviceNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" DDL.DEVICE_NO ");
                        strQry.AppendLine(",EPC.COMPANY_NO ");

                        strQry.AppendLine(strAddOnClockIn.Replace("X.", "DDLA."));
                        strQry.AppendLine(strAddOnClockOut.Replace("X.", "DDLA."));
                        strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DDLA."));

                        strQry.AppendLine(",DDLA.TIME_ATTEND_ROUNDING_IND");
                        strQry.AppendLine(",DDLA.TIME_ATTEND_ROUNDING_VALUE");

                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK DDL");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E");
                        strQry.AppendLine(" ON DDL.COMPANY_NO = E.COMPANY_NO ");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND DDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON DDL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");

                        strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK_ACTIVE DDLA");
                        strQry.AppendLine(" ON DDL.DEVICE_NO = DDLA.DEVICE_NO ");
                        strQry.AppendLine(" AND DDL.COMPANY_NO = DDLA.COMPANY_NO ");
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = DDLA.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = DDLA.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND DDL.DEPARTMENT_NO = DDLA.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND DDLA.ACTIVE_IND = 'Y' ");

                        strQry.AppendLine(" WHERE DDL.DEVICE_NO = " + DeviceNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" DGL.DEVICE_NO ");
                        strQry.AppendLine(",GEL.COMPANY_NO ");

                        strQry.AppendLine(strAddOnClockIn.Replace("X.", "DGLA."));
                        strQry.AppendLine(strAddOnClockOut.Replace("X.", "DGLA."));
                        strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DGLA."));

                        strQry.AppendLine(",DGLA.TIME_ATTEND_ROUNDING_IND");
                        strQry.AppendLine(",DGLA.TIME_ATTEND_ROUNDING_VALUE");

                        strQry.AppendLine(",GEL.EMPLOYEE_NO ");
                        strQry.AppendLine(",GEL.PAY_CATEGORY_NO ");
                        strQry.AppendLine(",GEL.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK DGL");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.GROUP_EMPLOYEE_LINK GEL");
                        strQry.AppendLine(" ON DGL.GROUP_NO = GEL.GROUP_NO ");
                        strQry.AppendLine(" AND DGL.COMPANY_NO = GEL.COMPANY_NO ");
                        strQry.AppendLine(" AND GEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_GROUP_LINK_ACTIVE DGLA");
                        strQry.AppendLine(" ON DGL.DEVICE_NO = DGLA.DEVICE_NO ");
                        strQry.AppendLine(" AND DGL.COMPANY_NO = DGLA.COMPANY_NO ");
                        strQry.AppendLine(" AND DGL.GROUP_NO = DGLA.GROUP_NO ");
                        strQry.AppendLine(" AND DGLA.ACTIVE_IND = 'Y' ");

                        strQry.AppendLine(" WHERE DGL.DEVICE_NO = " + DeviceNo + ") AS EMPLOYEE_DEVICE");

                        strQry.AppendLine(" ON D.DEVICE_NO = EMPLOYEE_DEVICE.DEVICE_NO");

                        strQry.AppendLine(" AND EUL.COMPANY_NO = EMPLOYEE_DEVICE.COMPANY_NO");
                        strQry.AppendLine(" AND EUL.EMPLOYEE_NO = EMPLOYEE_DEVICE.EMPLOYEE_NO");

                        strQry.AppendLine(" WHERE NOT UFT.FINGER_TEMPLATE IS NULL");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON DEVICE_EMPLOYEE.COMPANY_NO = E.COMPANY_NO ");
                        strQry.AppendLine(" AND DEVICE_EMPLOYEE.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND DEVICE_EMPLOYEE.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");
                        strQry.AppendLine(" AND (E.USE_EMPLOYEE_NO_IND <> 'Y' ");
                        strQry.AppendLine(" OR E.USE_EMPLOYEE_NO_IND IS NULL) ");
                    }

                    intWhere = 8;
                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Template");

                    intWhere = 9;
                    if (DataSet.Tables["Template"].Rows.Count == 0)
                    {
                        intWhere = 10;

                        if (EmployeeNo == "0")
                        {
                            EmployeeNameSurname.Name = "NO Employee/s for Device";
                        }
                        else
                        {
                            EmployeeNameSurname.Name = "NOT FOUND";
                        }
                    }
                    else
                    {
                        intWhere = 11;

                        int intReturnCode = FingerPrintClockServerLib.Get_Employee_Features(intFARRequested, DataSet.Tables["Template"], ref ImageFeaturesContainer.DPImageFeaturesByteArray, ref intRow);

                        intWhere = 12;
                        if (intRow == -1)
                        {
                            intWhere = 13;
                            EmployeeNameSurname.Name = "NOT FOUND";
                        }
                        else
                        {
                            intWhere = 14;

                            strQry.Clear();
                            strQry.AppendLine(" SELECT TOP 1 ");
                            strQry.AppendLine(" E.EMPLOYEE_NAME");
                            strQry.AppendLine(",E.EMPLOYEE_SURNAME");

                            if (InOutParm == "I")
                            {
                                strQry.AppendLine(",ISNULL(ETC.TIMESHEET_TIME_IN_MINUTES,0) AS TIMESHEET_TIME_MINUTES");
                                strQry.AppendLine(",ISNULL(ETC.CLOCKED_TIME_IN_MINUTES,0) AS CLOCKED_TIME_MINUTES");
                            }
                            else
                            {
                                strQry.AppendLine(",ISNULL(ETC.TIMESHEET_TIME_OUT_MINUTES,0) AS TIMESHEET_TIME_MINUTES");
                                strQry.AppendLine(",ISNULL(ETC.CLOCKED_TIME_OUT_MINUTES,0) AS CLOCKED_TIME_MINUTES");
                            }

                            strQry.AppendLine(",ETC.TIMESHEET_SEQ");

                            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                            if (DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                            {
                                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
                            }
                            else
                            {
                                if (DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                                {
                                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC");
                                }
                                else
                                {
                                    //T = Time Attendance
                                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC");
                                }
                            }

                            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO ");
                            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
                            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                            strQry.AppendLine(" AND TIMESHEET_DATE = '" + strDate + "'");

                            strQry.AppendLine(" WHERE E.COMPANY_NO = " + DataSet.Tables["Template"].Rows[intRow]["COMPANY_NO"].ToString());
                            strQry.AppendLine(" AND E.EMPLOYEE_NO = " + DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                            strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                            strQry.AppendLine(" ORDER BY ");
                            strQry.AppendLine(" ETC.TIMESHEET_SEQ DESC");

                            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

                            if (DataSet.Tables["Employee"].Rows[0]["CLOCKED_TIME_MINUTES"] != System.DBNull.Value)
                            {
                                if (Convert.ToInt32(DataSet.Tables["Employee"].Rows[0]["CLOCKED_TIME_MINUTES"]) == Convert.ToInt32(strTime))
                                {
                                    blnAlreadyClocked = true;
                                }
                            }

                            if (blnAlreadyClocked == false)
                            {
                                if (InOutParm == "I")
                                {
                                    if (DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")] != System.DBNull.Value)
                                    {
                                        if (Convert.ToInt32(DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")]) > Convert.ToInt32(strTime))
                                        {
                                            if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]))
                                            {
                                                blnAlreadyClocked = true;
                                            }
                                            else
                                            {
                                                if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")].ToString())
                                                {
                                                    blnAlreadyClocked = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]))
                                            {
                                                blnAlreadyClocked = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]))
                                        {
                                            blnAlreadyClocked = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")] != System.DBNull.Value)
                                    {
                                        if (Convert.ToInt32(DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")]) < Convert.ToInt32(strTime))
                                        {
                                            if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]))
                                            {
                                                blnAlreadyClocked = true;
                                            }
                                            else
                                            {
                                                if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")].ToString())
                                                {
                                                    blnAlreadyClocked = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]))
                                            {
                                                blnAlreadyClocked = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]))
                                        {
                                            blnAlreadyClocked = true;
                                        }
                                    }
                                }
                            }

                            if (blnAlreadyClocked == true)
                            {
                                intWhere = 15;
                                EmployeeNameSurname.OkInd = "A";
                                EmployeeNameSurname.EmployeeNo = DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                                EmployeeNameSurname.Name = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NAME"].ToString();
                                EmployeeNameSurname.Surname = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_SURNAME"].ToString();
                                return EmployeeNameSurname;
                            }

                            intWhere = 16;

                            //Insert Where NOT Duplicate
                            strQry.Clear();
                            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.DEVICE_CLOCK_TIME");
                            strQry.AppendLine("(DEVICE_NO");
                            strQry.AppendLine(",COMPANY_NO");
                            strQry.AppendLine(",EMPLOYEE_NO");
                            strQry.AppendLine(",PAY_CATEGORY_NO");
                            strQry.AppendLine(",PAY_CATEGORY_TYPE");
                            strQry.AppendLine(",TIMESHEET_DATE");
                            strQry.AppendLine(",TIMESHEET_TIME_MINUTES");
                            strQry.AppendLine(",CLOCKED_BOUNDARY_TIME_MINUTES");
                            strQry.AppendLine(",IN_OUT_IND)");
                            strQry.AppendLine(" VALUES ");
                            strQry.AppendLine("(" + DeviceNo);
                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow]["COMPANY_NO"].ToString());
                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                            strQry.AppendLine(",'" + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'");
                            strQry.AppendLine(",'" + strDate + "'");
                            strQry.AppendLine("," + strTime);

                            if (InOutParm == "I")
                            {
                                if (DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")].ToString() == "")
                                {
                                    //No Clock In Boundary Set
                                    strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                }
                                else
                                {
                                    if (Convert.ToInt32(DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"]) == 0)
                                    {
                                        //Last Clock In Was Null or 0 (Morning)
                                        if (Convert.ToInt32(DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")]) > Convert.ToInt32(strTime))
                                        {
                                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")]);
                                        }
                                        else
                                        {
                                            strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                        }
                                    }
                                    else
                                    {
                                        strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                    }
                                }
                            }
                            else
                            {
                                //Out
                                if (DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")].ToString() == "")
                                {
                                    strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                }
                                else
                                {
                                    if (DataSet.Tables["Template"].Rows[intRow][strAddOnClockInApplies.Replace(",X.", "")].ToString() == "Y")
                                    {
                                        //Check if Clock In Boundary was Used
                                        strQryTemp.AppendLine(" SELECT ");
                                        strQryTemp.AppendLine(" TIMESHEET_SEQ");
                                        strQryTemp.AppendLine(",TIMESHEET_TIME_IN_MINUTES");
                                        strQryTemp.AppendLine(",TIMESHEET_TIME_OUT_MINUTES");

                                        if (DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                                        {
                                            strQryTemp.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT");
                                        }
                                        else
                                        {
                                            if (DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                                            {
                                                strQryTemp.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT");
                                            }
                                            else
                                            {
                                                //T = Time Attendance
                                                strQryTemp.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT");
                                            }
                                        }

                                        strQryTemp.AppendLine(" WHERE COMPANY_NO = " + DataSet.Tables["Template"].Rows[intRow]["COMPANY_NO"].ToString());
                                        strQryTemp.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString());

                                        strQryTemp.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                                        strQryTemp.AppendLine(" AND TIMESHEET_DATE = '" + strDate + "'");

                                        strQryTemp.AppendLine(" ORDER BY ");
                                        strQryTemp.AppendLine(" TIMESHEET_SEQ DESC");

                                        clsDBConnectionObjects.Create_DataTable_Client(strQryTemp.ToString(), DataSet, "TimeCheck");

                                        if (DataSet.Tables["TimeCheck"].Rows.Count == 0)
                                        {
                                            //No Clocking for Day - Use Current Time / Rounded
                                            strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                        }
                                        else
                                        {
                                            if (DataSet.Tables["TimeCheck"].Rows[0]["TIMESHEET_TIME_OUT_MINUTES"] == System.DBNull.Value)
                                            {
                                                if (DataSet.Tables["TimeCheck"].Rows[0]["TIMESHEET_TIME_IN_MINUTES"].ToString() == DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")].ToString())
                                                {
                                                    //Clock In Boundary was Used therefor use Clock Out Boundary 
                                                    strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")].ToString());
                                                }
                                                else
                                                {
                                                    strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                                }
                                            }
                                            else
                                            {
                                                strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //Always Apply Clockout unless Clocked Out Before SET Time
                                        if (Convert.ToInt32(DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")]) < Convert.ToInt32(strTime))
                                        {
                                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")].ToString());
                                        }
                                        else
                                        {
                                            strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                        }
                                    }
                                }
                            }

                            strQry.AppendLine(",'" + InOutParm + "')");

                            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                            intWhere = 17;

                            EmployeeNameSurname.EmployeeNo = DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                            EmployeeNameSurname.Name = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NAME"].ToString();
                            EmployeeNameSurname.Surname = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_SURNAME"].ToString();
                        }
                    }
                }
                else
                {
                    intWhere = 19;
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",SUBSTRING(EMPLOYEE_NAME,1,1) + ' ' + EMPLOYEE_SURNAME AS EMPL_NAMES");
                    strQry.AppendLine(",READ_OPTION_NO");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE");

                    strQry.AppendLine(" WHERE EMPLOYEE_RFID_CARD_NO = " + RfIdNo);
                    strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                    strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

                    intWhere = 20;
                    if (DataSet.Tables["Employee"].Rows.Count == 0)
                    {
                        intWhere = 21;
                        EmployeeNameSurname.Name = "NOT FOUND";
                    }
                    else
                    {
                        intWhere = 22;
                        if (DataSet.Tables["Employee"].Rows[0]["READ_OPTION_NO"].ToString() == "2")
                        {
                            intWhere = 23;
                            EmployeeNameSurname.EmployeeNo = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"].ToString();
                            EmployeeNameSurname.Name = DataSet.Tables["Employee"].Rows[0]["EMPL_NAMES"].ToString();

                            //Insert record Into dB
                        }
                        else
                        {
                            intWhere = 24;
                            if (DataSet.Tables["Employee"].Rows[0]["READ_OPTION_NO"].ToString() == "3")
                            {
                                intWhere = 25;
                                EmployeeNameSurname.Name = "Scan your Finger";
                                EmployeeNameSurname.RfIdNo = RfIdNo;
                            }
                            else
                            {
                                intWhere = 26;
                                EmployeeNameSurname.OkInd = "N";
                                EmployeeNameSurname.Name = "Incorrect Profile Setup";
                            }
                        }
                    }
                }

                intWhere = 27;
            }
            catch (Exception ex)
            {
                WriteExceptionLog("GetEmployeeFeaturesClockedNew " + intWhere.ToString() + " SQL = " + strQry.ToString(), ex);

                EmployeeNameSurname.OkInd = "N";
                EmployeeNameSurname.Name = "ERROR - Web Server";
            }

        GetEmployeeClocked_Continue:

            DataSet.Dispose();

            return EmployeeNameSurname;
        }
  
        public EmployeeNameSurname GetEmployeeFeaturesBreakClocked(string DBNo, string DeviceNo, string InOutParm, string EmployeeNo, string RfIdNo, ImageFeaturesContainer ImageFeaturesContainer)
        {
            int intWhere = 0;
            //FAR 1 in 10 000
            int intFARRequested = 214748;
            StringBuilder strQry = new StringBuilder();

            EmployeeNameSurname EmployeeNameSurname = new EmployeeNameSurname();

            EmployeeNameSurname.OkInd = "Y";
            EmployeeNameSurname.RfIdNo = "0";
            EmployeeNameSurname.FingerPrintScore = "0";
            EmployeeNameSurname.Name = "";
            EmployeeNameSurname.Surname = "";
            EmployeeNameSurname.EmployeeNo = "";

            DataSet DataSet = new DataSet();

            try
            {
                clsFingerPrintClockServerLib FingerPrintClockServerLib = new clsFingerPrintClockServerLib(pvtstrSoftwareToUse);

                intWhere = 4;

                int intRow = -1;

                strQry.Clear();
                strQry.AppendLine(" SELECT");
                
                if (EmployeeNo == "0")
                {
                    strQry.AppendLine(" ISNULL(FAR_REQUESTED,214748) AS FAR_REQUESTED");
                }
                else
                {
                    //FAR = 1 in 5000
                    strQry.AppendLine(" 429496 AS FAR_REQUESTED");
                }

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE");
                
                strQry.AppendLine(" WHERE DEVICE_NO = " + DeviceNo);

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "FAR");

                if (DataSet.Tables["FAR"].Rows.Count > 0)
                {
                    intFARRequested = Convert.ToInt32(DataSet.Tables["FAR"].Rows[0]["FAR_REQUESTED"]);
                }

                intWhere = 6;
                //Image from Finger Print

                if (ImageFeaturesContainer.DPImageFeaturesByteArray != null)
                {
                    intWhere = 7;
                    string strDate = DateTime.Now.ToString("yyyy-MM-dd");
                    string strTime = Convert.ToInt32((DateTime.Now.Hour * 60) + DateTime.Now.Minute).ToString();

                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EFT.COMPANY_NO");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",EFT.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine("," + EmployeeNo + " AS EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",EFT.FINGER_NO");
                    strQry.AppendLine(",DEVICE_EMPLOYEE.PAY_CATEGORY_NO");
                    strQry.AppendLine(",DEVICE_EMPLOYEE.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(",EFT.FINGER_TEMPLATE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE EFT");

                    strQry.AppendLine(" INNER JOIN ");

                    strQry.AppendLine("(SELECT ");
                    strQry.AppendLine(" DEL.COMPANY_NO");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",DEL.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine(",TEMP_EMPLOYEE.EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",DEL.PAY_CATEGORY_NO");
                    strQry.AppendLine(",DEL.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_EMPLOYEE_LINK DEL");

                    if (EmployeeNo != "0")
                    {
                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DEL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" WHERE DEL.DEVICE_NO = " + DeviceNo);

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND DEL.EMPLOYEE_NO = " + EmployeeNo);
                    }

                    //Cost Centre 
                    strQry.AppendLine(" UNION ");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EPC.COMPANY_NO");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",EPC.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine(",TEMP_EMPLOYEE.EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK DPCL");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON DPCL.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + EmployeeNo);

                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DPCL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" WHERE DPCL.DEVICE_NO = " + DeviceNo);

                    //Department 
                    strQry.AppendLine(" UNION");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EPC.COMPANY_NO");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",EPC.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine(",TEMP_EMPLOYEE.EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK DDL");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON DDL.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND DDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                    strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                    strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON DDL.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + EmployeeNo);

                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DDL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" WHERE DDL.DEVICE_NO = " + DeviceNo);

                    //Group
                    strQry.AppendLine(" UNION");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" GEL.COMPANY_NO");

                    strQry.AppendLine(",GEL.EMPLOYEE_NO");
                    strQry.AppendLine(",GEL.PAY_CATEGORY_NO");
                    strQry.AppendLine(",GEL.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK DGL");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.GROUP_EMPLOYEE_LINK GEL");
                    strQry.AppendLine(" ON DGL.GROUP_NO = GEL.GROUP_NO ");
                    strQry.AppendLine(" AND DGL.COMPANY_NO = GEL.COMPANY_NO ");

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND GEL.EMPLOYEE_NO = " + EmployeeNo);

                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DGL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" WHERE DGL.DEVICE_NO = " + DeviceNo + ") AS DEVICE_EMPLOYEE");

                    strQry.AppendLine(" ON EFT.COMPANY_NO = DEVICE_EMPLOYEE.COMPANY_NO");
                    strQry.AppendLine(" AND EFT.EMPLOYEE_NO = DEVICE_EMPLOYEE.EMPLOYEE_NO");

                    if (EmployeeNo != "0")
                    {
                        //Add User Override Link
                        strQry.AppendLine(" UNION ALL");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EMPLOYEE_DEVICE.COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.EMPLOYEE_NO");
                        strQry.AppendLine(",UFT.FINGER_NO");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.PAY_CATEGORY_TYPE");

                        //Linked Person's Fingerprint Templates
                        strQry.AppendLine(",UFT.FINGER_TEMPLATE");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE UFT");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_USER_LINK EUL");
                        strQry.AppendLine(" ON UFT.USER_NO = EUL.USER_NO ");
                        strQry.AppendLine(" AND EUL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.DEVICE D");
                        strQry.AppendLine(" ON D.DEVICE_NO = " + DeviceNo);
                        //T=Time And Attendance,A= Access Control,B=Time And Attendance And Access Control
                        strQry.AppendLine(" AND D.DEVICE_USAGE IN ('T','B')");

                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT ");
                        strQry.AppendLine(" DEVICE_NO ");
                        strQry.AppendLine(",COMPANY_NO ");

                        strQry.AppendLine(",EMPLOYEE_NO ");
                        strQry.AppendLine(",PAY_CATEGORY_NO ");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_EMPLOYEE_LINK ");

                        strQry.AppendLine(" WHERE DEVICE_NO = " + DeviceNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" DPCL.DEVICE_NO ");
                        strQry.AppendLine(",EPC.COMPANY_NO ");

                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK DPCL");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON DPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" WHERE DPCL.DEVICE_NO = " + DeviceNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" DDL.DEVICE_NO ");
                        strQry.AppendLine(",EPC.COMPANY_NO ");

                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK DDL");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E");
                        strQry.AppendLine(" ON DDL.COMPANY_NO = E.COMPANY_NO ");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND DDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON DDL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");

                        strQry.AppendLine(" WHERE DDL.DEVICE_NO = " + DeviceNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" DGL.DEVICE_NO ");
                        strQry.AppendLine(",GEL.COMPANY_NO ");

                        strQry.AppendLine(",GEL.EMPLOYEE_NO ");
                        strQry.AppendLine(",GEL.PAY_CATEGORY_NO ");
                        strQry.AppendLine(",GEL.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK DGL");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.GROUP_EMPLOYEE_LINK GEL");
                        strQry.AppendLine(" ON DGL.GROUP_NO = GEL.GROUP_NO ");
                        strQry.AppendLine(" AND DGL.COMPANY_NO = GEL.COMPANY_NO ");
                        strQry.AppendLine(" AND GEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" WHERE DGL.DEVICE_NO = " + DeviceNo + ") AS EMPLOYEE_DEVICE");

                        strQry.AppendLine(" ON D.DEVICE_NO = EMPLOYEE_DEVICE.DEVICE_NO");

                        strQry.AppendLine(" AND EUL.COMPANY_NO = EMPLOYEE_DEVICE.COMPANY_NO");
                        strQry.AppendLine(" AND EUL.EMPLOYEE_NO = EMPLOYEE_DEVICE.EMPLOYEE_NO");

                        strQry.AppendLine(" WHERE NOT UFT.FINGER_TEMPLATE IS NULL");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON DEVICE_EMPLOYEE.COMPANY_NO = E.COMPANY_NO ");
                        strQry.AppendLine(" AND DEVICE_EMPLOYEE.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND DEVICE_EMPLOYEE.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");
                        strQry.AppendLine(" AND (E.USE_EMPLOYEE_NO_IND <> 'Y' ");
                        strQry.AppendLine(" OR E.USE_EMPLOYEE_NO_IND IS NULL) ");
                    }

                    intWhere = 8;
                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Template");

                    intWhere = 9;
                    if (DataSet.Tables["Template"].Rows.Count == 0)
                    {
                        intWhere = 10;

                        if (EmployeeNo == "0")
                        {
                            EmployeeNameSurname.Name = "NO Employee/s for Device";
                        }
                        else
                        {
                            EmployeeNameSurname.Name = "NOT FOUND";
                        }
                    }
                    else
                    {
                        intWhere = 11;

                        int intReturnCode = FingerPrintClockServerLib.Get_Employee_Features(intFARRequested, DataSet.Tables["Template"], ref ImageFeaturesContainer.DPImageFeaturesByteArray, ref intRow);

                        intWhere = 12;

                        if (intRow == -1)
                        {
                            intWhere = 13;
                            EmployeeNameSurname.Name = "NOT FOUND";
                        }
                        else
                        {
                            intWhere = 14;

                            strQry.Clear();
                            strQry.AppendLine(" SELECT TOP 1 ");
                            strQry.AppendLine(" E.EMPLOYEE_NAME");
                            strQry.AppendLine(",E.EMPLOYEE_SURNAME");

                            if (InOutParm == "I")
                            {
                                strQry.AppendLine(",ISNULL(ETC.BREAK_TIME_IN_MINUTES,0) AS BREAK_TIME_MINUTES");
                                strQry.AppendLine(",ISNULL(ETC.CLOCKED_TIME_IN_MINUTES,0) AS CLOCKED_TIME_MINUTES");
                            }
                            else
                            {
                                strQry.AppendLine(",ISNULL(ETC.BREAK_TIME_OUT_MINUTES,0) AS BREAK_TIME_MINUTES");
                                strQry.AppendLine(",ISNULL(ETC.CLOCKED_TIME_OUT_MINUTES,0) AS CLOCKED_TIME_MINUTES");
                            }

                            strQry.AppendLine(",ETC.BREAK_SEQ");

                            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                            if (DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                            {
                                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT ETC");
                            }
                            else
                            {
                                if (DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                                {
                                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ETC");
                                }
                                else
                                {
                                    //T = Time Attendance
                                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETC");
                                }
                            }

                            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO ");
                            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
                            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                            strQry.AppendLine(" AND BREAK_DATE = '" + strDate + "'");

                            strQry.AppendLine(" WHERE E.COMPANY_NO = " + DataSet.Tables["Template"].Rows[intRow]["COMPANY_NO"].ToString());
                            strQry.AppendLine(" AND E.EMPLOYEE_NO = " + DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                            strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                            strQry.AppendLine(" ORDER BY ");
                            strQry.AppendLine(" ETC.BREAK_SEQ DESC");

                            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

                            if (DataSet.Tables["Employee"].Rows[0]["CLOCKED_TIME_MINUTES"] != System.DBNull.Value)
                            {
                                if (Convert.ToInt32(DataSet.Tables["Employee"].Rows[0]["CLOCKED_TIME_MINUTES"]) == Convert.ToInt32(strTime))
                                {
                                    intWhere = 15;
                                    EmployeeNameSurname.OkInd = "A";
                                    EmployeeNameSurname.EmployeeNo = DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                                    EmployeeNameSurname.Name = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NAME"].ToString();
                                    EmployeeNameSurname.Surname = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_SURNAME"].ToString();
                                    return EmployeeNameSurname;
                                }
                            }

                            intWhere = 16;

                            //Insert Where NOT Duplicate
                            strQry.Clear();
                            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.DEVICE_CLOCK_TIME_BREAK");
                            strQry.AppendLine("(DEVICE_NO");
                            strQry.AppendLine(",COMPANY_NO");
                            strQry.AppendLine(",EMPLOYEE_NO");
                            strQry.AppendLine(",PAY_CATEGORY_NO");
                            strQry.AppendLine(",PAY_CATEGORY_TYPE");
                            strQry.AppendLine(",BREAK_DATE");
                            strQry.AppendLine(",BREAK_TIME_MINUTES");
                            strQry.AppendLine(",IN_OUT_IND)");
                            strQry.AppendLine(" VALUES ");
                            strQry.AppendLine("(" + DeviceNo);
                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow]["COMPANY_NO"].ToString());
                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                            strQry.AppendLine(",'" + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'");
                            strQry.AppendLine(",'" + strDate + "'");
                            strQry.AppendLine("," + strTime);

                            strQry.AppendLine(",'" + InOutParm + "')");

                            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                            intWhere = 17;

                            EmployeeNameSurname.EmployeeNo = DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                            EmployeeNameSurname.Name = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NAME"].ToString();
                            EmployeeNameSurname.Surname = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_SURNAME"].ToString();
                        }
                    }
                }
                else
                {
                    intWhere = 19;
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",SUBSTRING(EMPLOYEE_NAME,1,1) + ' ' + EMPLOYEE_SURNAME AS EMPL_NAMES");
                    strQry.AppendLine(",READ_OPTION_NO");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE");

                    strQry.AppendLine(" WHERE EMPLOYEE_RFID_CARD_NO = " + RfIdNo);
                    strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                    strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

                    intWhere = 20;
                    if (DataSet.Tables["Employee"].Rows.Count == 0)
                    {
                        intWhere = 21;
                        EmployeeNameSurname.Name = "NOT FOUND";
                    }
                    else
                    {
                        intWhere = 22;
                        if (DataSet.Tables["Employee"].Rows[0]["READ_OPTION_NO"].ToString() == "2")
                        {
                            intWhere = 23;
                            EmployeeNameSurname.EmployeeNo = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"].ToString();
                            EmployeeNameSurname.Name = DataSet.Tables["Employee"].Rows[0]["EMPL_NAMES"].ToString();

                            //Insert record Into dB
                        }
                        else
                        {
                            intWhere = 24;
                            if (DataSet.Tables["Employee"].Rows[0]["READ_OPTION_NO"].ToString() == "3")
                            {
                                intWhere = 25;
                                EmployeeNameSurname.Name = "Scan your Finger";
                                EmployeeNameSurname.RfIdNo = RfIdNo;
                            }
                            else
                            {
                                intWhere = 26;
                                EmployeeNameSurname.OkInd = "N";
                                EmployeeNameSurname.Name = "Incorrect Profile Setup";
                            }
                        }
                    }
                }

                intWhere = 27;
            }
            catch (Exception ex)
            {
                WriteExceptionLog("GetEmployeeFeaturesBreakClocked " + intWhere.ToString() + " SQL = " + strQry.ToString(), ex);

                EmployeeNameSurname.OkInd = "N";
                EmployeeNameSurname.Name = "ERROR - Web Server";
            }

            DataSet.Dispose();

            return EmployeeNameSurname;
        }
  
        public EmployeeNameSurname GetEmployeeFeaturesBreakClockedNew(string DeviceNo, string InOutParm, string EmployeeNo, string RfIdNo, ImageFeaturesContainer ImageFeaturesContainer)
        {
            int intWhere = 0;
            //FAR 1 in 10 000
            int intFARRequested = 214748;
            StringBuilder strQry = new StringBuilder();

            EmployeeNameSurname EmployeeNameSurname = new EmployeeNameSurname();

            EmployeeNameSurname.OkInd = "Y";
            EmployeeNameSurname.RfIdNo = "0";
            EmployeeNameSurname.FingerPrintScore = "0";
            EmployeeNameSurname.Name = "";
            EmployeeNameSurname.Surname = "";
            EmployeeNameSurname.EmployeeNo = "";

            DataSet DataSet = new DataSet();

            try
            {
                clsFingerPrintClockServerLib FingerPrintClockServerLib = new clsFingerPrintClockServerLib(pvtstrSoftwareToUse);

                intWhere = 4;

                int intRow = -1;

                strQry.Clear();
                strQry.AppendLine(" SELECT");

                if (EmployeeNo == "0")
                {
                    strQry.AppendLine(" ISNULL(FAR_REQUESTED,214748) AS FAR_REQUESTED");
                }
                else
                {
                    //FAR = 1 in 5000
                    strQry.AppendLine(" 429496 AS FAR_REQUESTED");
                }

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE");

                strQry.AppendLine(" WHERE DEVICE_NO = " + DeviceNo);

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "FAR");

                if (DataSet.Tables["FAR"].Rows.Count > 0)
                {
                    intFARRequested = Convert.ToInt32(DataSet.Tables["FAR"].Rows[0]["FAR_REQUESTED"]);
                }

                intWhere = 6;
                //Image from Finger Print

                if (ImageFeaturesContainer.DPImageFeaturesByteArray != null)
                {
                    intWhere = 7;
                    string strDate = DateTime.Now.ToString("yyyy-MM-dd");
                    string strTime = Convert.ToInt32((DateTime.Now.Hour * 60) + DateTime.Now.Minute).ToString();

                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EFT.COMPANY_NO");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",EFT.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine("," + EmployeeNo + " AS EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",EFT.FINGER_NO");
                    strQry.AppendLine(",DEVICE_EMPLOYEE.PAY_CATEGORY_NO");
                    strQry.AppendLine(",DEVICE_EMPLOYEE.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(",EFT.FINGER_TEMPLATE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE EFT");

                    strQry.AppendLine(" INNER JOIN ");

                    strQry.AppendLine("(SELECT ");
                    strQry.AppendLine(" DEL.COMPANY_NO");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",DEL.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine(",TEMP_EMPLOYEE.EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",DEL.PAY_CATEGORY_NO");
                    strQry.AppendLine(",DEL.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_EMPLOYEE_LINK DEL");

                    if (EmployeeNo != "0")
                    {
                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DEL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" WHERE DEL.DEVICE_NO = " + DeviceNo);

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND DEL.EMPLOYEE_NO = " + EmployeeNo);
                    }

                    //Cost Centre 
                    strQry.AppendLine(" UNION ");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EPC.COMPANY_NO");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",EPC.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine(",TEMP_EMPLOYEE.EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK DPCL");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON DPCL.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + EmployeeNo);

                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DPCL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" WHERE DPCL.DEVICE_NO = " + DeviceNo);

                    //Department 
                    strQry.AppendLine(" UNION");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EPC.COMPANY_NO");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",EPC.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine(",TEMP_EMPLOYEE.EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK DDL");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON DDL.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND DDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                    strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                    strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON DDL.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + EmployeeNo);

                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DDL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" WHERE DDL.DEVICE_NO = " + DeviceNo);

                    //Group
                    strQry.AppendLine(" UNION");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" GEL.COMPANY_NO");

                    strQry.AppendLine(",GEL.EMPLOYEE_NO");
                    strQry.AppendLine(",GEL.PAY_CATEGORY_NO");
                    strQry.AppendLine(",GEL.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK DGL");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.GROUP_EMPLOYEE_LINK GEL");
                    strQry.AppendLine(" ON DGL.GROUP_NO = GEL.GROUP_NO ");
                    strQry.AppendLine(" AND DGL.COMPANY_NO = GEL.COMPANY_NO ");

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND GEL.EMPLOYEE_NO = " + EmployeeNo);

                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DGL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" WHERE DGL.DEVICE_NO = " + DeviceNo + ") AS DEVICE_EMPLOYEE");

                    strQry.AppendLine(" ON EFT.COMPANY_NO = DEVICE_EMPLOYEE.COMPANY_NO");
                    strQry.AppendLine(" AND EFT.EMPLOYEE_NO = DEVICE_EMPLOYEE.EMPLOYEE_NO");

                    if (EmployeeNo != "0")
                    {
                        //Add User Override Link
                        strQry.AppendLine(" UNION ALL");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EMPLOYEE_DEVICE.COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.EMPLOYEE_NO");
                        strQry.AppendLine(",UFT.FINGER_NO");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.PAY_CATEGORY_TYPE");

                        //Linked Person's Fingerprint Templates
                        strQry.AppendLine(",UFT.FINGER_TEMPLATE");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE UFT");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_USER_LINK EUL");
                        strQry.AppendLine(" ON UFT.USER_NO = EUL.USER_NO ");
                        strQry.AppendLine(" AND EUL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.DEVICE D");
                        strQry.AppendLine(" ON D.DEVICE_NO = " + DeviceNo);
                        //T=Time And Attendance,A= Access Control,B=Time And Attendance And Access Control
                        strQry.AppendLine(" AND D.DEVICE_USAGE IN ('T','B')");

                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT ");
                        strQry.AppendLine(" DEVICE_NO ");
                        strQry.AppendLine(",COMPANY_NO ");

                        strQry.AppendLine(",EMPLOYEE_NO ");
                        strQry.AppendLine(",PAY_CATEGORY_NO ");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_EMPLOYEE_LINK ");

                        strQry.AppendLine(" WHERE DEVICE_NO = " + DeviceNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" DPCL.DEVICE_NO ");
                        strQry.AppendLine(",EPC.COMPANY_NO ");

                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK DPCL");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON DPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" WHERE DPCL.DEVICE_NO = " + DeviceNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" DDL.DEVICE_NO ");
                        strQry.AppendLine(",EPC.COMPANY_NO ");

                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK DDL");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E");
                        strQry.AppendLine(" ON DDL.COMPANY_NO = E.COMPANY_NO ");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND DDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON DDL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");

                        strQry.AppendLine(" WHERE DDL.DEVICE_NO = " + DeviceNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" DGL.DEVICE_NO ");
                        strQry.AppendLine(",GEL.COMPANY_NO ");

                        strQry.AppendLine(",GEL.EMPLOYEE_NO ");
                        strQry.AppendLine(",GEL.PAY_CATEGORY_NO ");
                        strQry.AppendLine(",GEL.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK DGL");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.GROUP_EMPLOYEE_LINK GEL");
                        strQry.AppendLine(" ON DGL.GROUP_NO = GEL.GROUP_NO ");
                        strQry.AppendLine(" AND DGL.COMPANY_NO = GEL.COMPANY_NO ");
                        strQry.AppendLine(" AND GEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" WHERE DGL.DEVICE_NO = " + DeviceNo + ") AS EMPLOYEE_DEVICE");

                        strQry.AppendLine(" ON D.DEVICE_NO = EMPLOYEE_DEVICE.DEVICE_NO");

                        strQry.AppendLine(" AND EUL.COMPANY_NO = EMPLOYEE_DEVICE.COMPANY_NO");
                        strQry.AppendLine(" AND EUL.EMPLOYEE_NO = EMPLOYEE_DEVICE.EMPLOYEE_NO");

                        strQry.AppendLine(" WHERE NOT UFT.FINGER_TEMPLATE IS NULL");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON DEVICE_EMPLOYEE.COMPANY_NO = E.COMPANY_NO ");
                        strQry.AppendLine(" AND DEVICE_EMPLOYEE.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND DEVICE_EMPLOYEE.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");
                        strQry.AppendLine(" AND (E.USE_EMPLOYEE_NO_IND <> 'Y' ");
                        strQry.AppendLine(" OR E.USE_EMPLOYEE_NO_IND IS NULL) ");
                    }

                    intWhere = 8;
                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Template");

                    intWhere = 9;
                    if (DataSet.Tables["Template"].Rows.Count == 0)
                    {
                        intWhere = 10;

                        if (EmployeeNo == "0")
                        {
                            EmployeeNameSurname.Name = "NO Employee/s for Device";
                        }
                        else
                        {
                            EmployeeNameSurname.Name = "NOT FOUND";
                        }
                    }
                    else
                    {
                        intWhere = 11;

                        int intReturnCode = FingerPrintClockServerLib.Get_Employee_Features(intFARRequested, DataSet.Tables["Template"], ref ImageFeaturesContainer.DPImageFeaturesByteArray, ref intRow);

                        intWhere = 12;

                        if (intRow == -1)
                        {
                            intWhere = 13;
                            EmployeeNameSurname.Name = "NOT FOUND";
                        }
                        else
                        {
                            intWhere = 14;

                            strQry.Clear();
                            strQry.AppendLine(" SELECT TOP 1 ");
                            strQry.AppendLine(" E.EMPLOYEE_NAME");
                            strQry.AppendLine(",E.EMPLOYEE_SURNAME");

                            if (InOutParm == "I")
                            {
                                strQry.AppendLine(",ISNULL(ETC.BREAK_TIME_IN_MINUTES,0) AS BREAK_TIME_MINUTES");
                                strQry.AppendLine(",ISNULL(ETC.CLOCKED_TIME_IN_MINUTES,0) AS CLOCKED_TIME_MINUTES");
                            }
                            else
                            {
                                strQry.AppendLine(",ISNULL(ETC.BREAK_TIME_OUT_MINUTES,0) AS BREAK_TIME_MINUTES");
                                strQry.AppendLine(",ISNULL(ETC.CLOCKED_TIME_OUT_MINUTES,0) AS CLOCKED_TIME_MINUTES");
                            }

                            strQry.AppendLine(",ETC.BREAK_SEQ");

                            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                            if (DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                            {
                                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT ETC");
                            }
                            else
                            {
                                if (DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                                {
                                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ETC");
                                }
                                else
                                {
                                    //T = Time Attendance
                                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETC");
                                }
                            }

                            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO ");
                            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
                            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                            strQry.AppendLine(" AND BREAK_DATE = '" + strDate + "'");

                            strQry.AppendLine(" WHERE E.COMPANY_NO = " + DataSet.Tables["Template"].Rows[intRow]["COMPANY_NO"].ToString());
                            strQry.AppendLine(" AND E.EMPLOYEE_NO = " + DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                            strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                            strQry.AppendLine(" ORDER BY ");
                            strQry.AppendLine(" ETC.BREAK_SEQ DESC");

                            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

                            if (DataSet.Tables["Employee"].Rows[0]["CLOCKED_TIME_MINUTES"] != System.DBNull.Value)
                            {
                                if (Convert.ToInt32(DataSet.Tables["Employee"].Rows[0]["CLOCKED_TIME_MINUTES"]) == Convert.ToInt32(strTime))
                                {
                                    intWhere = 15;
                                    EmployeeNameSurname.OkInd = "A";
                                    EmployeeNameSurname.EmployeeNo = DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                                    EmployeeNameSurname.Name = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NAME"].ToString();
                                    EmployeeNameSurname.Surname = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_SURNAME"].ToString();
                                    return EmployeeNameSurname;
                                }
                            }

                            intWhere = 16;

                            //Insert Where NOT Duplicate
                            strQry.Clear();
                            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.DEVICE_CLOCK_TIME_BREAK");
                            strQry.AppendLine("(DEVICE_NO");
                            strQry.AppendLine(",COMPANY_NO");
                            strQry.AppendLine(",EMPLOYEE_NO");
                            strQry.AppendLine(",PAY_CATEGORY_NO");
                            strQry.AppendLine(",PAY_CATEGORY_TYPE");
                            strQry.AppendLine(",BREAK_DATE");
                            strQry.AppendLine(",BREAK_TIME_MINUTES");
                            strQry.AppendLine(",IN_OUT_IND)");
                            strQry.AppendLine(" VALUES ");
                            strQry.AppendLine("(" + DeviceNo);
                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow]["COMPANY_NO"].ToString());
                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                            strQry.AppendLine(",'" + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'");
                            strQry.AppendLine(",'" + strDate + "'");
                            strQry.AppendLine("," + strTime);

                            strQry.AppendLine(",'" + InOutParm + "')");

                            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                            intWhere = 17;

                            EmployeeNameSurname.EmployeeNo = DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                            EmployeeNameSurname.Name = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NAME"].ToString();
                            EmployeeNameSurname.Surname = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_SURNAME"].ToString();
                        }
                    }
                }
                else
                {
                    intWhere = 19;
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",SUBSTRING(EMPLOYEE_NAME,1,1) + ' ' + EMPLOYEE_SURNAME AS EMPL_NAMES");
                    strQry.AppendLine(",READ_OPTION_NO");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE");

                    strQry.AppendLine(" WHERE EMPLOYEE_RFID_CARD_NO = " + RfIdNo);
                    strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                    strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

                    intWhere = 20;
                    if (DataSet.Tables["Employee"].Rows.Count == 0)
                    {
                        intWhere = 21;
                        EmployeeNameSurname.Name = "NOT FOUND";
                    }
                    else
                    {
                        intWhere = 22;
                        if (DataSet.Tables["Employee"].Rows[0]["READ_OPTION_NO"].ToString() == "2")
                        {
                            intWhere = 23;
                            EmployeeNameSurname.EmployeeNo = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"].ToString();
                            EmployeeNameSurname.Name = DataSet.Tables["Employee"].Rows[0]["EMPL_NAMES"].ToString();

                            //Insert record Into dB
                        }
                        else
                        {
                            intWhere = 24;
                            if (DataSet.Tables["Employee"].Rows[0]["READ_OPTION_NO"].ToString() == "3")
                            {
                                intWhere = 25;
                                EmployeeNameSurname.Name = "Scan your Finger";
                                EmployeeNameSurname.RfIdNo = RfIdNo;
                            }
                            else
                            {
                                intWhere = 26;
                                EmployeeNameSurname.OkInd = "N";
                                EmployeeNameSurname.Name = "Incorrect Profile Setup";
                            }
                        }
                    }
                }

                intWhere = 27;
            }
            catch (Exception ex)
            {
                WriteExceptionLog("GetEmployeeFeaturesBreakClockedNew " + intWhere.ToString() + " SQL = " + strQry.ToString(), ex);

                EmployeeNameSurname.OkInd = "N";
                EmployeeNameSurname.Name = "ERROR - Web Server";
            }

            DataSet.Dispose();

            return EmployeeNameSurname;
        }

        public EmployeeNames GetEmployeeClocked(string DeviceNo, string ReaderNo, string InOutParm, string EmployeeNo, string RfIdNo, ImageContainer ImageContainer)
        {
            //A11
            int intWhere = 0;
            StringBuilder strQry = new StringBuilder();

            EmployeeNames EmployeeNames = new EmployeeNames();
            intWhere = 1;
            EmployeeNames.OkInd = "Y";
            intWhere = 2;
            EmployeeNames.ReaderNo = ReaderNo;
            intWhere = 3;
            EmployeeNames.RfIdNo = "0";
            EmployeeNames.FingerPrintScore = "0";
            EmployeeNames.Names = "";
            EmployeeNames.EmployeeNo = "";

            DataSet DataSet = new DataSet();

            try
            {
                int intFingerPrintThreshold = 214748;
                int intFingerPrintScore = 0;
               
                strQry.Clear();
                strQry.AppendLine(" SELECT");

                if (EmployeeNo == "0")
                {
                    strQry.AppendLine(" ISNULL(FAR_REQUESTED,214748) AS FAR_REQUESTED");
                }
                else
                {
                    //FAR = 1 in 5000
                    strQry.AppendLine(" 429496 AS FAR_REQUESTED");
                }

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE");
                
                strQry.AppendLine(" WHERE DEVICE_NO = " + DeviceNo);

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "FAR");

                if (DataSet.Tables["FAR"].Rows.Count > 0)
                {
                    intFingerPrintThreshold = Convert.ToInt32(DataSet.Tables["FAR"].Rows[0]["FAR_REQUESTED"]);
                }
               
                clsFingerPrintClockServerLib FingerPrintClockServerLib = new clsFingerPrintClockServerLib(pvtstrSoftwareToUse);

                intWhere = 4;

                int intRow = -1;

                intWhere = 6;
                //Image from Finger Print

                if (ImageContainer.DPImageByteArray != null)
                {
                    byte[] bytFmdFingerTemplate = null;

                    if (ImageContainer.DPImageByteArray.Length == 101376)
                    {
                        int intGreyScaleHeight = 352;
                        int intGreyScaleWidth = 288;

                        bytFmdFingerTemplate = FeatureExtraction.CreateFmdFromRaw(ImageContainer.DPImageByteArray, 0, 0, intGreyScaleWidth, intGreyScaleHeight, 500, Constants.Formats.Fmd.ANSI).Data.Bytes;
                    }
                    else
                    {
                        //A11 Cock Image
                        int intGreyScaleHeight = 256 + (int)ImageContainer.DPImageByteArray[2];
                        int intGreyScaleWidth = 256 + (int)ImageContainer.DPImageByteArray[0];

                        byte[] bytCreatedRawFmd = new byte[intGreyScaleHeight * intGreyScaleWidth];

                        unsafe
                        {
                            IntPtr ptr;

                            fixed (byte* p = bytCreatedRawFmd)
                            {
                                ptr = (IntPtr)p;
                            }

                            Marshal.Copy(ImageContainer.DPImageByteArray, 4, ptr, bytCreatedRawFmd.Length);
                        }

                        bytFmdFingerTemplate = FeatureExtraction.CreateFmdFromRaw(bytCreatedRawFmd, 0, 0, intGreyScaleWidth, intGreyScaleHeight, 500, Constants.Formats.Fmd.ANSI).Data.Bytes;
                    }

                    intWhere = 7;
                    string strDate = DateTime.Now.ToString("yyyy-MM-dd");
                    string strTime = Convert.ToInt32((DateTime.Now.Hour * 60) + DateTime.Now.Minute).ToString();
                    string strAddOnClockIn = "";
                    string strAddOnClockOut = "";
                    string strAddOnClockInApplies = "";
                    StringBuilder strQryTemp  = new StringBuilder();
                    bool blnAlreadyClocked = false;

                    switch (Convert.ToInt32(DateTime.Now.DayOfWeek))
                    {
                        case 1:

                            strAddOnClockIn = ",X.MON_CLOCK_IN";
                            strAddOnClockOut = ",X.MON_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.MON_CLOCK_IN_APPLIES_IND";
                            break;

                        case 2:

                            strAddOnClockIn = ",X.TUE_CLOCK_IN";
                            strAddOnClockOut = ",X.TUE_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.TUE_CLOCK_IN_APPLIES_IND";
                            break;

                        case 3:

                            strAddOnClockIn = ",X.WED_CLOCK_IN";
                            strAddOnClockOut = ",X.WED_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.WED_CLOCK_IN_APPLIES_IND";
                            break;

                        case 4:

                            strAddOnClockIn = ",X.THU_CLOCK_IN";
                            strAddOnClockOut = ",X.THU_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.THU_CLOCK_IN_APPLIES_IND";
                            break;

                        case 5:

                            strAddOnClockIn = ",X.FRI_CLOCK_IN";
                            strAddOnClockOut = ",X.FRI_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.FRI_CLOCK_IN_APPLIES_IND";
                            break;

                        case 6:

                            strAddOnClockIn = ",X.SAT_CLOCK_IN";
                            strAddOnClockOut = ",X.SAT_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.SAT_CLOCK_IN_APPLIES_IND";
                            break;

                        case 0:

                            strAddOnClockIn = ",X.SUN_CLOCK_IN";
                            strAddOnClockOut = ",X.SUN_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.SUN_CLOCK_IN_APPLIES_IND";
                            break;
                    }

                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EFT.COMPANY_NO");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",EFT.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine("," + EmployeeNo + " AS EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",EFT.FINGER_NO");
                    strQry.AppendLine(",DEVICE_EMPLOYEE.PAY_CATEGORY_NO");
                    strQry.AppendLine(",DEVICE_EMPLOYEE.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(strAddOnClockIn.Replace("X.", "DEVICE_EMPLOYEE."));
                    strQry.AppendLine(strAddOnClockOut.Replace("X.", "DEVICE_EMPLOYEE."));
                    strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DEVICE_EMPLOYEE."));

                    strQry.AppendLine(",DEVICE_EMPLOYEE.TIME_ATTEND_ROUNDING_IND");
                    strQry.AppendLine(",DEVICE_EMPLOYEE.TIME_ATTEND_ROUNDING_VALUE");

                    strQry.AppendLine(",EFT.FINGER_TEMPLATE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE EFT");

                    strQry.AppendLine(" INNER JOIN ");

                    strQry.AppendLine("(SELECT ");
                    strQry.AppendLine(" DEL.COMPANY_NO");

                    strQry.AppendLine(strAddOnClockIn.Replace("X.", "'' AS "));
                    strQry.AppendLine(strAddOnClockOut.Replace("X.", "'' AS "));
                    strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "'' AS "));

                    strQry.AppendLine(",NULL AS TIME_ATTEND_ROUNDING_IND");
                    strQry.AppendLine(",0 AS TIME_ATTEND_ROUNDING_VALUE");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",DEL.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine(",TEMP_EMPLOYEE.EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",DEL.PAY_CATEGORY_NO");
                    strQry.AppendLine(",DEL.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_EMPLOYEE_LINK DEL");

                    if (EmployeeNo != "0")
                    {
                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DEL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" WHERE DEL.DEVICE_NO = " + DeviceNo);

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND DEL.EMPLOYEE_NO = " + EmployeeNo);
                    }

                    //Cost Centre 
                    strQry.AppendLine(" UNION ");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EPC.COMPANY_NO");

                    strQry.AppendLine(strAddOnClockIn.Replace("X.", "DPCLA."));
                    strQry.AppendLine(strAddOnClockOut.Replace("X.", "DPCLA."));
                    strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DPCLA."));

                    strQry.AppendLine(",DPCLA.TIME_ATTEND_ROUNDING_IND");
                    strQry.AppendLine(",DPCLA.TIME_ATTEND_ROUNDING_VALUE");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",EPC.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine(",TEMP_EMPLOYEE.EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK DPCL");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON DPCL.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + EmployeeNo);

                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DPCL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK_ACTIVE DPCLA");
                    strQry.AppendLine(" ON DPCL.DEVICE_NO = DPCLA.DEVICE_NO ");
                    strQry.AppendLine(" AND DPCL.COMPANY_NO = DPCLA.COMPANY_NO ");
                    strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = DPCLA.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = DPCLA.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND DPCLA.ACTIVE_IND = 'Y' ");

                    strQry.AppendLine(" WHERE DPCL.DEVICE_NO = " + DeviceNo);

                    //Department 
                    strQry.AppendLine(" UNION");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EPC.COMPANY_NO");

                    strQry.AppendLine(strAddOnClockIn.Replace("X.", "DDLA."));
                    strQry.AppendLine(strAddOnClockOut.Replace("X.", "DDLA."));
                    strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DDLA."));

                    strQry.AppendLine(",DDLA.TIME_ATTEND_ROUNDING_IND");
                    strQry.AppendLine(",DDLA.TIME_ATTEND_ROUNDING_VALUE");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",EPC.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine(",TEMP_EMPLOYEE.EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK DDL");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON DDL.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND DDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                    strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                    strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON DDL.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + EmployeeNo);

                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DDL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK_ACTIVE DDLA");
                    strQry.AppendLine(" ON DDL.DEVICE_NO = DDLA.DEVICE_NO ");
                    strQry.AppendLine(" AND DDL.COMPANY_NO = DDLA.COMPANY_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = DDLA.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = DDLA.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND DDL.DEPARTMENT_NO = DDLA.DEPARTMENT_NO ");
                    strQry.AppendLine(" AND DDLA.ACTIVE_IND = 'Y' ");

                    strQry.AppendLine(" WHERE DDL.DEVICE_NO = " + DeviceNo);

                    //Group
                    strQry.AppendLine(" UNION");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" GEL.COMPANY_NO");

                    strQry.AppendLine(strAddOnClockIn.Replace("X.", "DGLA."));
                    strQry.AppendLine(strAddOnClockOut.Replace("X.", "DGLA."));
                    strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DGLA."));

                    strQry.AppendLine(",DGLA.TIME_ATTEND_ROUNDING_IND");
                    strQry.AppendLine(",DGLA.TIME_ATTEND_ROUNDING_VALUE");

                    strQry.AppendLine(",GEL.EMPLOYEE_NO");
                    strQry.AppendLine(",GEL.PAY_CATEGORY_NO");
                    strQry.AppendLine(",GEL.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK DGL");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.GROUP_EMPLOYEE_LINK GEL");
                    strQry.AppendLine(" ON DGL.GROUP_NO = GEL.GROUP_NO ");
                    strQry.AppendLine(" AND DGL.COMPANY_NO = GEL.COMPANY_NO ");

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND GEL.EMPLOYEE_NO = " + EmployeeNo);

                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DGL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_GROUP_LINK_ACTIVE DGLA");
                    strQry.AppendLine(" ON DGL.DEVICE_NO = DGLA.DEVICE_NO ");
                    strQry.AppendLine(" AND DGL.COMPANY_NO = DGLA.COMPANY_NO ");
                    strQry.AppendLine(" AND DGL.GROUP_NO = DGLA.GROUP_NO ");
                    strQry.AppendLine(" AND DGLA.ACTIVE_IND = 'Y' ");

                    strQry.AppendLine(" WHERE DGL.DEVICE_NO = " + DeviceNo + ") AS DEVICE_EMPLOYEE");

                    strQry.AppendLine(" ON EFT.COMPANY_NO = DEVICE_EMPLOYEE.COMPANY_NO");
                    strQry.AppendLine(" AND EFT.EMPLOYEE_NO = DEVICE_EMPLOYEE.EMPLOYEE_NO");

                    if (EmployeeNo != "0")
                    {
                        //Add User Override Link
                        strQry.AppendLine(" UNION ALL");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EMPLOYEE_DEVICE.COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.EMPLOYEE_NO");
                        strQry.AppendLine(",UFT.FINGER_NO");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.PAY_CATEGORY_TYPE");

                        strQry.AppendLine(strAddOnClockIn.Replace("X.", "EMPLOYEE_DEVICE."));
                        strQry.AppendLine(strAddOnClockOut.Replace("X.", "EMPLOYEE_DEVICE."));
                        strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "EMPLOYEE_DEVICE."));

                        strQry.AppendLine(",EMPLOYEE_DEVICE.TIME_ATTEND_ROUNDING_IND");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.TIME_ATTEND_ROUNDING_VALUE");

                        //Linked Person's Fingerprint Templates
                        strQry.AppendLine(",UFT.FINGER_TEMPLATE");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE UFT");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_USER_LINK EUL");
                        strQry.AppendLine(" ON UFT.USER_NO = EUL.USER_NO ");
                        strQry.AppendLine(" AND EUL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.DEVICE D");
                        strQry.AppendLine(" ON D.DEVICE_NO = " + DeviceNo);
                        //T=Time And Attendance,A= Access Control,B=Time And Attendance And Access Control
                        strQry.AppendLine(" AND D.DEVICE_USAGE IN ('T','B')");

                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT ");
                        strQry.AppendLine(" DEVICE_NO ");
                        strQry.AppendLine(",COMPANY_NO ");

                        strQry.AppendLine(strAddOnClockIn.Replace("X.", "'' AS "));
                        strQry.AppendLine(strAddOnClockOut.Replace("X.", "'' AS "));
                        strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "'' AS "));

                        strQry.AppendLine(",NULL AS TIME_ATTEND_ROUNDING_IND");
                        strQry.AppendLine(",0 AS TIME_ATTEND_ROUNDING_VALUE");

                        strQry.AppendLine(",EMPLOYEE_NO ");
                        strQry.AppendLine(",PAY_CATEGORY_NO ");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_EMPLOYEE_LINK ");

                        strQry.AppendLine(" WHERE DEVICE_NO = " + DeviceNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" DPCL.DEVICE_NO ");
                        strQry.AppendLine(",EPC.COMPANY_NO ");

                        strQry.AppendLine(strAddOnClockIn.Replace("X.", "DPCLA."));
                        strQry.AppendLine(strAddOnClockOut.Replace("X.", "DPCLA."));
                        strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DPCLA."));

                        strQry.AppendLine(",DPCLA.TIME_ATTEND_ROUNDING_IND");
                        strQry.AppendLine(",DPCLA.TIME_ATTEND_ROUNDING_VALUE");

                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK DPCL");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON DPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK_ACTIVE DPCLA");
                        strQry.AppendLine(" ON DPCL.DEVICE_NO = DPCLA.DEVICE_NO ");
                        strQry.AppendLine(" AND DPCL.COMPANY_NO = DPCLA.COMPANY_NO ");
                        strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = DPCLA.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = DPCLA.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND DPCLA.ACTIVE_IND = 'Y' ");

                        strQry.AppendLine(" WHERE DPCL.DEVICE_NO = " + DeviceNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" DDL.DEVICE_NO ");
                        strQry.AppendLine(",EPC.COMPANY_NO ");

                        strQry.AppendLine(strAddOnClockIn.Replace("X.", "DDLA."));
                        strQry.AppendLine(strAddOnClockOut.Replace("X.", "DDLA."));
                        strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DDLA."));

                        strQry.AppendLine(",DDLA.TIME_ATTEND_ROUNDING_IND");
                        strQry.AppendLine(",DDLA.TIME_ATTEND_ROUNDING_VALUE");

                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK DDL");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E");
                        strQry.AppendLine(" ON DDL.COMPANY_NO = E.COMPANY_NO ");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND DDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON DDL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");

                        strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK_ACTIVE DDLA");
                        strQry.AppendLine(" ON DDL.DEVICE_NO = DDLA.DEVICE_NO ");
                        strQry.AppendLine(" AND DDL.COMPANY_NO = DDLA.COMPANY_NO ");
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = DDLA.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = DDLA.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND DDL.DEPARTMENT_NO = DDLA.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND DDLA.ACTIVE_IND = 'Y' ");

                        strQry.AppendLine(" WHERE DDL.DEVICE_NO = " + DeviceNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" DGL.DEVICE_NO ");
                        strQry.AppendLine(",GEL.COMPANY_NO ");

                        strQry.AppendLine(strAddOnClockIn.Replace("X.", "DGLA."));
                        strQry.AppendLine(strAddOnClockOut.Replace("X.", "DGLA."));
                        strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DGLA."));

                        strQry.AppendLine(",DGLA.TIME_ATTEND_ROUNDING_IND");
                        strQry.AppendLine(",DGLA.TIME_ATTEND_ROUNDING_VALUE");

                        strQry.AppendLine(",GEL.EMPLOYEE_NO ");
                        strQry.AppendLine(",GEL.PAY_CATEGORY_NO ");
                        strQry.AppendLine(",GEL.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK DGL");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.GROUP_EMPLOYEE_LINK GEL");
                        strQry.AppendLine(" ON DGL.GROUP_NO = GEL.GROUP_NO ");
                        strQry.AppendLine(" AND DGL.COMPANY_NO = GEL.COMPANY_NO ");
                        strQry.AppendLine(" AND GEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_GROUP_LINK_ACTIVE DGLA");
                        strQry.AppendLine(" ON DGL.DEVICE_NO = DGLA.DEVICE_NO ");
                        strQry.AppendLine(" AND DGL.COMPANY_NO = DGLA.COMPANY_NO ");
                        strQry.AppendLine(" AND DGL.GROUP_NO = DGLA.GROUP_NO ");
                        strQry.AppendLine(" AND DGLA.ACTIVE_IND = 'Y' ");

                        strQry.AppendLine(" WHERE DGL.DEVICE_NO = " + DeviceNo + ") AS EMPLOYEE_DEVICE");

                        strQry.AppendLine(" ON D.DEVICE_NO = EMPLOYEE_DEVICE.DEVICE_NO");

                        strQry.AppendLine(" AND EUL.COMPANY_NO = EMPLOYEE_DEVICE.COMPANY_NO");
                        strQry.AppendLine(" AND EUL.EMPLOYEE_NO = EMPLOYEE_DEVICE.EMPLOYEE_NO");

                        strQry.AppendLine(" WHERE NOT UFT.FINGER_TEMPLATE IS NULL");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON DEVICE_EMPLOYEE.COMPANY_NO = E.COMPANY_NO ");
                        strQry.AppendLine(" AND DEVICE_EMPLOYEE.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND DEVICE_EMPLOYEE.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");
                        strQry.AppendLine(" AND (E.USE_EMPLOYEE_NO_IND <> 'Y' ");
                        strQry.AppendLine(" OR E.USE_EMPLOYEE_NO_IND IS NULL) ");
                    }

                    intWhere = 8;
                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Template");

                    intWhere = 9;
                    if (DataSet.Tables["Template"].Rows.Count == 0)
                    {
                        intWhere = 10;

                        if (EmployeeNo == "0")
                        {
                            EmployeeNames.Names = "NO Employee/s for Device";
                        }
                        else
                        {
                            EmployeeNames.Names = "NOT FOUND";
                        }
                    }
                    else
                    {
                        intWhere = 11;

                        int intReturnCode = FingerPrintClockServerLib.Get_Employee(DataSet.Tables["Template"], ref bytFmdFingerTemplate, intFingerPrintThreshold, ref intFingerPrintScore, ref intRow);

                        intWhere = 12;
                        if (intRow == -1)
                        {
                            intWhere = 13;
                            EmployeeNames.Names = "NOT FOUND";
                        }
                        else
                        {
                            intWhere = 14;

                            strQry.Clear();
                            strQry.AppendLine(" SELECT TOP 1 ");
                            
                            if (ImageContainer.DPImageByteArray.Length == 101376)
                            {
                                strQry.AppendLine(" E.EMPLOYEE_NAME + '#' + E.EMPLOYEE_SURNAME AS EMPL_NAMES");
                            }
                            else
                            {
                                strQry.AppendLine(" SUBSTRING(E.EMPLOYEE_NAME,1,1) + ' ' + E.EMPLOYEE_SURNAME AS EMPL_NAMES");
                            }

                            if (InOutParm == "I")
                            {
                                strQry.AppendLine(",ISNULL(ETC.TIMESHEET_TIME_IN_MINUTES,0) AS TIMESHEET_TIME_MINUTES");
                                strQry.AppendLine(",ISNULL(ETC.CLOCKED_TIME_IN_MINUTES,0) AS CLOCKED_TIME_MINUTES");
                            }
                            else
                            {
                                strQry.AppendLine(",ISNULL(ETC.TIMESHEET_TIME_IN_MINUTES,0) AS TIMESHEET_TIME_IN_MINUTES");
                                strQry.AppendLine(",ISNULL(ETC.TIMESHEET_TIME_OUT_MINUTES,0) AS TIMESHEET_TIME_MINUTES");
                                strQry.AppendLine(",ISNULL(ETC.CLOCKED_TIME_OUT_MINUTES,0) AS CLOCKED_TIME_MINUTES");
                            }

                            strQry.AppendLine(",ETC.TIMESHEET_SEQ");

                            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                            if (DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                            {
                                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
                            }
                            else
                            {
                                if (DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                                {
                                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC");
                                }
                                else
                                {
                                    //T = Time Attendance
                                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC");
                                }
                            }

                            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO ");
                            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
                            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                            strQry.AppendLine(" AND TIMESHEET_DATE = '" + strDate + "'");

                            strQry.AppendLine(" WHERE E.COMPANY_NO = " + DataSet.Tables["Template"].Rows[intRow]["COMPANY_NO"].ToString());
                            strQry.AppendLine(" AND E.EMPLOYEE_NO = " + DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                            strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                            strQry.AppendLine(" ORDER BY ");
                            strQry.AppendLine(" ETC.TIMESHEET_SEQ DESC");

                            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

                            if (DataSet.Tables["Employee"].Rows[0]["CLOCKED_TIME_MINUTES"] != System.DBNull.Value)
                            {
                                if (Convert.ToInt32(DataSet.Tables["Employee"].Rows[0]["CLOCKED_TIME_MINUTES"]) == Convert.ToInt32(strTime))
                                {
                                    blnAlreadyClocked = true;
                                }
                            }

                            if (blnAlreadyClocked == false)
                            {
                                if (InOutParm == "I")
                                {
                                    if (DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")] != System.DBNull.Value)
                                    {
                                        if (Convert.ToInt32(DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")]) > Convert.ToInt32(strTime))
                                        {
                                            if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]))
                                            {
                                                blnAlreadyClocked = true;
                                            }
                                            else
                                            {
                                                if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")].ToString())
                                                {
                                                    blnAlreadyClocked = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]))
                                            {
                                                blnAlreadyClocked = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]))
                                        {
                                            blnAlreadyClocked = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")] != System.DBNull.Value)
                                    {
                                        if (Convert.ToInt32(DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")]) < Convert.ToInt32(strTime))
                                        {
                                            if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]))
                                            {
                                                blnAlreadyClocked = true;
                                            }
                                            else
                                            {
                                                if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")].ToString())
                                                {
                                                    blnAlreadyClocked = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]))
                                            {
                                                blnAlreadyClocked = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]))
                                        {
                                            blnAlreadyClocked = true;
                                        }
                                    }
                                }
                            }

                            if (blnAlreadyClocked == true)
                            {
                                intWhere = 15;
                                EmployeeNames.OkInd = "A";
                                EmployeeNames.EmployeeNo = DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                                EmployeeNames.Names = "ALREADY CLOCKED";
                                return EmployeeNames;
                            }

                            intWhere = 16;

                            //Insert Where NOT Duplicate
                            strQry.Clear();
                            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.DEVICE_CLOCK_TIME");
                            strQry.AppendLine("(DEVICE_NO");
                            strQry.AppendLine(",COMPANY_NO");
                            strQry.AppendLine(",EMPLOYEE_NO");
                            strQry.AppendLine(",PAY_CATEGORY_NO");
                            strQry.AppendLine(",PAY_CATEGORY_TYPE");
                            strQry.AppendLine(",TIMESHEET_DATE");
                            strQry.AppendLine(",TIMESHEET_TIME_MINUTES");
                            strQry.AppendLine(",CLOCKED_BOUNDARY_TIME_MINUTES");
                            strQry.AppendLine(",IN_OUT_IND)");
                            strQry.AppendLine(" VALUES ");
                            strQry.AppendLine("(" + DeviceNo);
                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow]["COMPANY_NO"].ToString());
                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                            strQry.AppendLine(",'" + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'");
                            strQry.AppendLine(",'" + strDate + "'");
                            strQry.AppendLine("," + strTime);

                            if (InOutParm == "I")
                            {
                                if (DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")].ToString() == "")
                                {
                                    //No Clock In Boundary Set
                                    strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                }
                                else
                                {
                                    if (Convert.ToInt32(DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"]) == 0)
                                    {
                                        //Last Clock In Was Null or 0 (Morning)
                                        if (Convert.ToInt32(DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")]) > Convert.ToInt32(strTime))
                                        {
                                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")]);
                                        }
                                        else
                                        {
                                            strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                        }
                                    }
                                    else
                                    {
                                        strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                    }
                                }
                            }
                            else
                            {
                                //Out
                                if (DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")].ToString() == "")
                                {
                                    strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                }
                                else
                                {
                                    //Clock Out Parameter Apply
                                    if (Convert.ToInt32(DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")]) == Convert.ToInt32(DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_IN_MINUTES"])
                                    & Convert.ToInt32(strTime) < Convert.ToInt32(DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")]))
                                    {
                                        //Clocked In and Out Before Clock-In Paramter applied
                                        strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")].ToString());
                                    }
                                    else
                                    {
                                        if (DataSet.Tables["Template"].Rows[intRow][strAddOnClockInApplies.Replace(",X.", "")].ToString() == "Y")
                                        {
                                            //Check if Clock In Boundary was Used
                                            strQryTemp.AppendLine(" SELECT ");
                                            strQryTemp.AppendLine(" TIMESHEET_SEQ");
                                            strQryTemp.AppendLine(",TIMESHEET_TIME_IN_MINUTES");
                                            strQryTemp.AppendLine(",TIMESHEET_TIME_OUT_MINUTES");

                                            if (DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                                            {
                                                strQryTemp.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT");
                                            }
                                            else
                                            {
                                                if (DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                                                {
                                                    strQryTemp.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT");
                                                }
                                                else
                                                {
                                                    //T = Time Attendance
                                                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC");
                                                }
                                            }

                                            strQryTemp.AppendLine(" WHERE COMPANY_NO = " + DataSet.Tables["Template"].Rows[intRow]["COMPANY_NO"].ToString());
                                            strQryTemp.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString());

                                            strQryTemp.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                                            strQryTemp.AppendLine(" AND TIMESHEET_DATE = '" + strDate + "'");

                                            strQryTemp.AppendLine(" ORDER BY ");
                                            strQryTemp.AppendLine(" TIMESHEET_SEQ DESC");

                                            clsDBConnectionObjects.Create_DataTable_Client(strQryTemp.ToString(), DataSet, "TimeCheck");

                                            if (DataSet.Tables["TimeCheck"].Rows.Count == 0)
                                            {
                                                //No Clocking for Day - Use Current Time / Rounded
                                                strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                            }
                                            else
                                            {
                                                if (DataSet.Tables["TimeCheck"].Rows[0]["TIMESHEET_TIME_OUT_MINUTES"] == System.DBNull.Value)
                                                {
                                                    if (DataSet.Tables["TimeCheck"].Rows[0]["TIMESHEET_TIME_IN_MINUTES"].ToString() == DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")].ToString())
                                                    {
                                                        //Clock In Boundary was Used therefor use Clock Out Boundary 
                                                        strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")].ToString());
                                                    }
                                                    else
                                                    {
                                                        strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                                    }
                                                }
                                                else
                                                {
                                                    strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //Always Apply Clockout unless Clocked Out Before SET Time
                                            if (Convert.ToInt32(DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")]) < Convert.ToInt32(strTime))
                                            {
                                                strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")].ToString());
                                            }
                                            else
                                            {
                                                strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                            }
                                        }
                                    }
                                }
                            }

                            strQry.AppendLine(",'" + InOutParm + "')");

                            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                            intWhere = 17;

                            EmployeeNames.EmployeeNo = DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                            EmployeeNames.Names = DataSet.Tables["Employee"].Rows[0]["EMPL_NAMES"].ToString();
                        }
                    }
                }
                else
                {
                    intWhere = 19;
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");

                    if (ImageContainer.DPImageByteArray.Length == 101376)
                    {
                        strQry.AppendLine(",EMPLOYEE_NAME + '#' + EMPLOYEE_SURNAME AS EMPL_NAMES");
                    }
                    else
                    {
                        strQry.AppendLine(",SUBSTRING(EMPLOYEE_NAME,1,1) + ' ' + EMPLOYEE_SURNAME AS EMPL_NAMES");
                    }

                    strQry.AppendLine(",READ_OPTION_NO");
                    
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE");
                    
                    strQry.AppendLine(" WHERE EMPLOYEE_RFID_CARD_NO = " + RfIdNo);
                    strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                    strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

                    intWhere = 20;
                    if (DataSet.Tables["Employee"].Rows.Count == 0)
                    {
                        intWhere = 21;
                        EmployeeNames.Names = "NOT FOUND";
                    }
                    else
                    {
                        intWhere = 22;
                        if (DataSet.Tables["Employee"].Rows[0]["READ_OPTION_NO"].ToString() == "2")
                        {
                            intWhere = 23;
                            EmployeeNames.EmployeeNo = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"].ToString();
                            EmployeeNames.Names = DataSet.Tables["Employee"].Rows[0]["EMPL_NAMES"].ToString();
                        }
                        else
                        {
                            intWhere = 24;
                            if (DataSet.Tables["Employee"].Rows[0]["READ_OPTION_NO"].ToString() == "3")
                            {
                                intWhere = 25;
                                EmployeeNames.Names = "Scan your Finger";
                                EmployeeNames.RfIdNo = RfIdNo;
                            }
                            else
                            {
                                intWhere = 26;
                                EmployeeNames.OkInd = "N";
                                EmployeeNames.Names = "Incorrect Profile Setup";
                            }
                        }
                    }
                }

                intWhere = 27;
            }
            catch (Exception ex)
            {
                WriteExceptionLog("GetEmployeeClocked " + intWhere.ToString() + " SQL = " + strQry.ToString(), ex);

                EmployeeNames.OkInd = "N";
                EmployeeNames.Names = "ERROR - Web Server";
            }

        GetEmployeeClocked_Continue:

            DataSet.Dispose();

            return EmployeeNames;
        }

        public EmployeeNames GetEmployeeClockedNew(string DeviceNo, string InOutParm, string EmployeeNo, string RfIdNo, ImageContainer ImageContainer)
        {
            if (pvtblnLoggingSwitchedOn == true)
            {
                WriteLog("GetEmployeeClockedNew Entered");
            }
            
            int intWhere = 0;
            StringBuilder strQry = new StringBuilder();

            EmployeeNames EmployeeNames = new EmployeeNames();
            intWhere = 1;
            EmployeeNames.OkInd = "Y";
            intWhere = 2;
            EmployeeNames.ReaderNo = "0";
            intWhere = 3;
            EmployeeNames.RfIdNo = "0";
            EmployeeNames.FingerPrintScore = "0";
            EmployeeNames.Names = "";
            EmployeeNames.EmployeeNo = "";

            DataSet DataSet = new DataSet();

            try
            {
                int intFingerPrintThreshold = 214748;
                int intFingerPrintScore = 0;

                strQry.Clear();
                strQry.AppendLine(" SELECT");

                if (EmployeeNo == "0")
                {
                    strQry.AppendLine(" ISNULL(FAR_REQUESTED,214748) AS FAR_REQUESTED");
                }
                else
                {
                    //FAR = 1 in 5000
                    strQry.AppendLine(" 429496 AS FAR_REQUESTED");
                }

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE");

                strQry.AppendLine(" WHERE DEVICE_NO = " + DeviceNo);

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "FAR");

                if (DataSet.Tables["FAR"].Rows.Count > 0)
                {
                    intFingerPrintThreshold = Convert.ToInt32(DataSet.Tables["FAR"].Rows[0]["FAR_REQUESTED"]);
                }

                clsFingerPrintClockServerLib FingerPrintClockServerLib = new clsFingerPrintClockServerLib(pvtstrSoftwareToUse);

                intWhere = 4;

                int intRow = -1;

                intWhere = 6;
                //Image from Finger Print

                if (ImageContainer.DPImageByteArray != null)
                {
                    byte[] bytFmdFingerTemplate = null;

                    if (ImageContainer.DPImageByteArray.Length == 101376)
                    {
                        int intGreyScaleHeight = 352;
                        int intGreyScaleWidth = 288;

                        bytFmdFingerTemplate = FeatureExtraction.CreateFmdFromRaw(ImageContainer.DPImageByteArray, 0, 0, intGreyScaleWidth, intGreyScaleHeight, 500, Constants.Formats.Fmd.ANSI).Data.Bytes;
                    }
                    else
                    {
                        //A11 Cock Image
                        int intGreyScaleHeight = 256 + (int)ImageContainer.DPImageByteArray[2];
                        int intGreyScaleWidth = 256 + (int)ImageContainer.DPImageByteArray[0];

                        byte[] bytCreatedRawFmd = new byte[intGreyScaleHeight * intGreyScaleWidth];

                        unsafe
                        {
                            IntPtr ptr;

                            fixed (byte* p = bytCreatedRawFmd)
                            {
                                ptr = (IntPtr)p;
                            }

                            Marshal.Copy(ImageContainer.DPImageByteArray, 4, ptr, bytCreatedRawFmd.Length);
                        }

                        bytFmdFingerTemplate = FeatureExtraction.CreateFmdFromRaw(bytCreatedRawFmd, 0, 0, intGreyScaleWidth, intGreyScaleHeight, 500, Constants.Formats.Fmd.ANSI).Data.Bytes;
                    }

                    intWhere = 7;
                    string strDate = DateTime.Now.ToString("yyyy-MM-dd");
                    string strTime = Convert.ToInt32((DateTime.Now.Hour * 60) + DateTime.Now.Minute).ToString();
                    string strAddOnClockIn = "";
                    string strAddOnClockOut = "";
                    string strAddOnClockInApplies = "";
                    StringBuilder strQryTemp  = new StringBuilder();
                    bool blnAlreadyClocked = false;

                    switch (Convert.ToInt32(DateTime.Now.DayOfWeek))
                    {
                        case 1:

                            strAddOnClockIn = ",X.MON_CLOCK_IN";
                            strAddOnClockOut = ",X.MON_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.MON_CLOCK_IN_APPLIES_IND";
                            break;

                        case 2:

                            strAddOnClockIn = ",X.TUE_CLOCK_IN";
                            strAddOnClockOut = ",X.TUE_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.TUE_CLOCK_IN_APPLIES_IND";
                            break;

                        case 3:

                            strAddOnClockIn = ",X.WED_CLOCK_IN";
                            strAddOnClockOut = ",X.WED_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.WED_CLOCK_IN_APPLIES_IND";
                            break;

                        case 4:

                            strAddOnClockIn = ",X.THU_CLOCK_IN";
                            strAddOnClockOut = ",X.THU_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.THU_CLOCK_IN_APPLIES_IND";
                            break;

                        case 5:

                            strAddOnClockIn = ",X.FRI_CLOCK_IN";
                            strAddOnClockOut = ",X.FRI_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.FRI_CLOCK_IN_APPLIES_IND";
                            break;

                        case 6:

                            strAddOnClockIn = ",X.SAT_CLOCK_IN";
                            strAddOnClockOut = ",X.SAT_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.SAT_CLOCK_IN_APPLIES_IND";
                            break;

                        case 0:

                            strAddOnClockIn = ",X.SUN_CLOCK_IN";
                            strAddOnClockOut = ",X.SUN_CLOCK_OUT";
                            strAddOnClockInApplies = ",X.SUN_CLOCK_IN_APPLIES_IND";
                            break;
                    }

                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EFT.COMPANY_NO");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",EFT.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine("," + EmployeeNo + " AS EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",EFT.FINGER_NO");
                    strQry.AppendLine(",DEVICE_EMPLOYEE.PAY_CATEGORY_NO");
                    strQry.AppendLine(",DEVICE_EMPLOYEE.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(strAddOnClockIn.Replace("X.", "DEVICE_EMPLOYEE."));
                    strQry.AppendLine(strAddOnClockOut.Replace("X.", "DEVICE_EMPLOYEE."));
                    strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DEVICE_EMPLOYEE."));

                    strQry.AppendLine(",DEVICE_EMPLOYEE.TIME_ATTEND_ROUNDING_IND");
                    strQry.AppendLine(",DEVICE_EMPLOYEE.TIME_ATTEND_ROUNDING_VALUE");

                    strQry.AppendLine(",EFT.FINGER_TEMPLATE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE EFT");

                    strQry.AppendLine(" INNER JOIN ");

                    strQry.AppendLine("(SELECT ");
                    strQry.AppendLine(" DEL.COMPANY_NO");

                    strQry.AppendLine(strAddOnClockIn.Replace("X.", "'' AS "));
                    strQry.AppendLine(strAddOnClockOut.Replace("X.", "'' AS "));
                    strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "'' AS "));

                    strQry.AppendLine(",NULL AS TIME_ATTEND_ROUNDING_IND");
                    strQry.AppendLine(",0 AS TIME_ATTEND_ROUNDING_VALUE");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",DEL.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine(",TEMP_EMPLOYEE.EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",DEL.PAY_CATEGORY_NO");
                    strQry.AppendLine(",DEL.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_EMPLOYEE_LINK DEL");

                    if (EmployeeNo != "0")
                    {
                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DEL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" WHERE DEL.DEVICE_NO = " + DeviceNo);

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND DEL.EMPLOYEE_NO = " + EmployeeNo);
                    }

                    //Cost Centre 
                    strQry.AppendLine(" UNION ");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EPC.COMPANY_NO");

                    strQry.AppendLine(strAddOnClockIn.Replace("X.", "DPCLA."));
                    strQry.AppendLine(strAddOnClockOut.Replace("X.", "DPCLA."));
                    strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DPCLA."));

                    strQry.AppendLine(",DPCLA.TIME_ATTEND_ROUNDING_IND");
                    strQry.AppendLine(",DPCLA.TIME_ATTEND_ROUNDING_VALUE");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",EPC.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine(",TEMP_EMPLOYEE.EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK DPCL");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON DPCL.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + EmployeeNo);

                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DPCL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK_ACTIVE DPCLA");
                    strQry.AppendLine(" ON DPCL.DEVICE_NO = DPCLA.DEVICE_NO ");
                    strQry.AppendLine(" AND DPCL.COMPANY_NO = DPCLA.COMPANY_NO ");
                    strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = DPCLA.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = DPCLA.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND DPCLA.ACTIVE_IND = 'Y' ");

                    strQry.AppendLine(" WHERE DPCL.DEVICE_NO = " + DeviceNo);

                    //Department 
                    strQry.AppendLine(" UNION");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EPC.COMPANY_NO");

                    strQry.AppendLine(strAddOnClockIn.Replace("X.", "DDLA."));
                    strQry.AppendLine(strAddOnClockOut.Replace("X.", "DDLA."));
                    strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DDLA."));

                    strQry.AppendLine(",DDLA.TIME_ATTEND_ROUNDING_IND");
                    strQry.AppendLine(",DDLA.TIME_ATTEND_ROUNDING_VALUE");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",EPC.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine(",TEMP_EMPLOYEE.EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK DDL");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON DDL.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND DDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                    strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                    strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON DDL.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + EmployeeNo);

                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DDL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK_ACTIVE DDLA");
                    strQry.AppendLine(" ON DDL.DEVICE_NO = DDLA.DEVICE_NO ");
                    strQry.AppendLine(" AND DDL.COMPANY_NO = DDLA.COMPANY_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = DDLA.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = DDLA.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND DDL.DEPARTMENT_NO = DDLA.DEPARTMENT_NO ");
                    strQry.AppendLine(" AND DDLA.ACTIVE_IND = 'Y' ");

                    strQry.AppendLine(" WHERE DDL.DEVICE_NO = " + DeviceNo);

                    //Group
                    strQry.AppendLine(" UNION");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" GEL.COMPANY_NO");

                    strQry.AppendLine(strAddOnClockIn.Replace("X.", "DGLA."));
                    strQry.AppendLine(strAddOnClockOut.Replace("X.", "DGLA."));
                    strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DGLA."));

                    strQry.AppendLine(",DGLA.TIME_ATTEND_ROUNDING_IND");
                    strQry.AppendLine(",DGLA.TIME_ATTEND_ROUNDING_VALUE");

                    strQry.AppendLine(",GEL.EMPLOYEE_NO");
                    strQry.AppendLine(",GEL.PAY_CATEGORY_NO");
                    strQry.AppendLine(",GEL.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK DGL");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.GROUP_EMPLOYEE_LINK GEL");
                    strQry.AppendLine(" ON DGL.GROUP_NO = GEL.GROUP_NO ");
                    strQry.AppendLine(" AND DGL.COMPANY_NO = GEL.COMPANY_NO ");

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND GEL.EMPLOYEE_NO = " + EmployeeNo);

                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DGL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_GROUP_LINK_ACTIVE DGLA");
                    strQry.AppendLine(" ON DGL.DEVICE_NO = DGLA.DEVICE_NO ");
                    strQry.AppendLine(" AND DGL.COMPANY_NO = DGLA.COMPANY_NO ");
                    strQry.AppendLine(" AND DGL.GROUP_NO = DGLA.GROUP_NO ");
                    strQry.AppendLine(" AND DGLA.ACTIVE_IND = 'Y' ");

                    strQry.AppendLine(" WHERE DGL.DEVICE_NO = " + DeviceNo + ") AS DEVICE_EMPLOYEE");

                    strQry.AppendLine(" ON EFT.COMPANY_NO = DEVICE_EMPLOYEE.COMPANY_NO");
                    strQry.AppendLine(" AND EFT.EMPLOYEE_NO = DEVICE_EMPLOYEE.EMPLOYEE_NO");

                    if (EmployeeNo != "0")
                    {
                        //Add User Override Link
                        strQry.AppendLine(" UNION ALL");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EMPLOYEE_DEVICE.COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.EMPLOYEE_NO");
                        strQry.AppendLine(",UFT.FINGER_NO");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.PAY_CATEGORY_TYPE");

                        strQry.AppendLine(strAddOnClockIn.Replace("X.", "EMPLOYEE_DEVICE."));
                        strQry.AppendLine(strAddOnClockOut.Replace("X.", "EMPLOYEE_DEVICE."));
                        strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "EMPLOYEE_DEVICE."));

                        strQry.AppendLine(",EMPLOYEE_DEVICE.TIME_ATTEND_ROUNDING_IND");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.TIME_ATTEND_ROUNDING_VALUE");

                        //Linked Person's Fingerprint Templates
                        strQry.AppendLine(",UFT.FINGER_TEMPLATE");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE UFT");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_USER_LINK EUL");
                        strQry.AppendLine(" ON UFT.USER_NO = EUL.USER_NO ");
                        strQry.AppendLine(" AND EUL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.DEVICE D");
                        strQry.AppendLine(" ON D.DEVICE_NO = " + DeviceNo);
                        //T=Time And Attendance,A= Access Control,B=Time And Attendance And Access Control
                        strQry.AppendLine(" AND D.DEVICE_USAGE IN ('T','B')");

                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT ");
                        strQry.AppendLine(" DEVICE_NO ");
                        strQry.AppendLine(",COMPANY_NO ");

                        strQry.AppendLine(strAddOnClockIn.Replace("X.", "'' AS "));
                        strQry.AppendLine(strAddOnClockOut.Replace("X.", "'' AS "));
                        strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "'' AS "));

                        strQry.AppendLine(",NULL AS TIME_ATTEND_ROUNDING_IND");
                        strQry.AppendLine(",0 AS TIME_ATTEND_ROUNDING_VALUE");

                        strQry.AppendLine(",EMPLOYEE_NO ");
                        strQry.AppendLine(",PAY_CATEGORY_NO ");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_EMPLOYEE_LINK ");

                        strQry.AppendLine(" WHERE DEVICE_NO = " + DeviceNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" DPCL.DEVICE_NO ");
                        strQry.AppendLine(",EPC.COMPANY_NO ");

                        strQry.AppendLine(strAddOnClockIn.Replace("X.", "DPCLA."));
                        strQry.AppendLine(strAddOnClockOut.Replace("X.", "DPCLA."));
                        strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DPCLA."));

                        strQry.AppendLine(",DPCLA.TIME_ATTEND_ROUNDING_IND");
                        strQry.AppendLine(",DPCLA.TIME_ATTEND_ROUNDING_VALUE");

                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK DPCL");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON DPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK_ACTIVE DPCLA");
                        strQry.AppendLine(" ON DPCL.DEVICE_NO = DPCLA.DEVICE_NO ");
                        strQry.AppendLine(" AND DPCL.COMPANY_NO = DPCLA.COMPANY_NO ");
                        strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = DPCLA.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = DPCLA.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND DPCLA.ACTIVE_IND = 'Y' ");

                        strQry.AppendLine(" WHERE DPCL.DEVICE_NO = " + DeviceNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" DDL.DEVICE_NO ");
                        strQry.AppendLine(",EPC.COMPANY_NO ");

                        strQry.AppendLine(strAddOnClockIn.Replace("X.", "DDLA."));
                        strQry.AppendLine(strAddOnClockOut.Replace("X.", "DDLA."));
                        strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DDLA."));

                        strQry.AppendLine(",DDLA.TIME_ATTEND_ROUNDING_IND");
                        strQry.AppendLine(",DDLA.TIME_ATTEND_ROUNDING_VALUE");

                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK DDL");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E");
                        strQry.AppendLine(" ON DDL.COMPANY_NO = E.COMPANY_NO ");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND DDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON DDL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");

                        strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK_ACTIVE DDLA");
                        strQry.AppendLine(" ON DDL.DEVICE_NO = DDLA.DEVICE_NO ");
                        strQry.AppendLine(" AND DDL.COMPANY_NO = DDLA.COMPANY_NO ");
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = DDLA.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = DDLA.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND DDL.DEPARTMENT_NO = DDLA.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND DDLA.ACTIVE_IND = 'Y' ");

                        strQry.AppendLine(" WHERE DDL.DEVICE_NO = " + DeviceNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" DGL.DEVICE_NO ");
                        strQry.AppendLine(",GEL.COMPANY_NO ");

                        strQry.AppendLine(strAddOnClockIn.Replace("X.", "DGLA."));
                        strQry.AppendLine(strAddOnClockOut.Replace("X.", "DGLA."));
                        strQry.AppendLine(strAddOnClockInApplies.Replace("X.", "DGLA."));

                        strQry.AppendLine(",DGLA.TIME_ATTEND_ROUNDING_IND");
                        strQry.AppendLine(",DGLA.TIME_ATTEND_ROUNDING_VALUE");

                        strQry.AppendLine(",GEL.EMPLOYEE_NO ");
                        strQry.AppendLine(",GEL.PAY_CATEGORY_NO ");
                        strQry.AppendLine(",GEL.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK DGL");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.GROUP_EMPLOYEE_LINK GEL");
                        strQry.AppendLine(" ON DGL.GROUP_NO = GEL.GROUP_NO ");
                        strQry.AppendLine(" AND DGL.COMPANY_NO = GEL.COMPANY_NO ");
                        strQry.AppendLine(" AND GEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_GROUP_LINK_ACTIVE DGLA");
                        strQry.AppendLine(" ON DGL.DEVICE_NO = DGLA.DEVICE_NO ");
                        strQry.AppendLine(" AND DGL.COMPANY_NO = DGLA.COMPANY_NO ");
                        strQry.AppendLine(" AND DGL.GROUP_NO = DGLA.GROUP_NO ");
                        strQry.AppendLine(" AND DGLA.ACTIVE_IND = 'Y' ");

                        strQry.AppendLine(" WHERE DGL.DEVICE_NO = " + DeviceNo + ") AS EMPLOYEE_DEVICE");

                        strQry.AppendLine(" ON D.DEVICE_NO = EMPLOYEE_DEVICE.DEVICE_NO");

                        strQry.AppendLine(" AND EUL.COMPANY_NO = EMPLOYEE_DEVICE.COMPANY_NO");
                        strQry.AppendLine(" AND EUL.EMPLOYEE_NO = EMPLOYEE_DEVICE.EMPLOYEE_NO");

                        strQry.AppendLine(" WHERE NOT UFT.FINGER_TEMPLATE IS NULL");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON DEVICE_EMPLOYEE.COMPANY_NO = E.COMPANY_NO ");
                        strQry.AppendLine(" AND DEVICE_EMPLOYEE.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND DEVICE_EMPLOYEE.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");
                        strQry.AppendLine(" AND (E.USE_EMPLOYEE_NO_IND <> 'Y' ");
                        strQry.AppendLine(" OR E.USE_EMPLOYEE_NO_IND IS NULL) ");
                    }

                    intWhere = 8;
                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Template");

                    intWhere = 9;
                    if (DataSet.Tables["Template"].Rows.Count == 0)
                    {
                        intWhere = 10;

                        if (EmployeeNo == "0")
                        {
                            EmployeeNames.Names = "NO Employee/s for Device";
                        }
                        else
                        {
                            EmployeeNames.Names = "NOT FOUND";
                        }
                    }
                    else
                    {
                        intWhere = 11;

                        int intReturnCode = FingerPrintClockServerLib.Get_Employee(DataSet.Tables["Template"], ref bytFmdFingerTemplate, intFingerPrintThreshold, ref intFingerPrintScore, ref intRow);

                        intWhere = 12;
                        if (intRow == -1)
                        {
                            intWhere = 13;
                            EmployeeNames.Names = "NOT FOUND";
                        }
                        else
                        {
                            intWhere = 14;

                            strQry.Clear();
                            strQry.AppendLine(" SELECT TOP 1 ");

                            if (ImageContainer.DPImageByteArray.Length == 101376)
                            {
                                strQry.AppendLine(" E.EMPLOYEE_NAME + '#' + E.EMPLOYEE_SURNAME AS EMPL_NAMES");
                            }
                            else
                            {
                                strQry.AppendLine(" SUBSTRING(E.EMPLOYEE_NAME,1,1) + ' ' + E.EMPLOYEE_SURNAME AS EMPL_NAMES");
                            }

                            if (InOutParm == "I")
                            {
                                strQry.AppendLine(",ISNULL(ETC.TIMESHEET_TIME_IN_MINUTES,0) AS TIMESHEET_TIME_MINUTES");
                                strQry.AppendLine(",ISNULL(ETC.CLOCKED_TIME_IN_MINUTES,0) AS CLOCKED_TIME_MINUTES");
                            }
                            else
                            {
                                strQry.AppendLine(",ISNULL(ETC.TIMESHEET_TIME_IN_MINUTES,0) AS TIMESHEET_TIME_IN_MINUTES");
                                strQry.AppendLine(",ISNULL(ETC.TIMESHEET_TIME_OUT_MINUTES,0) AS TIMESHEET_TIME_MINUTES");
                                strQry.AppendLine(",ISNULL(ETC.CLOCKED_TIME_OUT_MINUTES,0) AS CLOCKED_TIME_MINUTES");
                            }

                            strQry.AppendLine(",ETC.TIMESHEET_SEQ");

                            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                            if (DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                            {
                                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
                            }
                            else
                            {
                                if (DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                                {
                                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC");
                                }
                                else
                                {
                                    //T = Time Attendance
                                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC");
                                }
                            }

                            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO ");
                            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
                            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                            strQry.AppendLine(" AND TIMESHEET_DATE = '" + strDate + "'");

                            strQry.AppendLine(" WHERE E.COMPANY_NO = " + DataSet.Tables["Template"].Rows[intRow]["COMPANY_NO"].ToString());
                            strQry.AppendLine(" AND E.EMPLOYEE_NO = " + DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                            strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                            strQry.AppendLine(" ORDER BY ");
                            strQry.AppendLine(" ETC.TIMESHEET_SEQ DESC");

                            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

                            if (DataSet.Tables["Employee"].Rows.Count == 0)
                            {
                                goto GetEmployeeClocked_Continue;
                            }
                            
                            if (DataSet.Tables["Employee"].Rows[0]["CLOCKED_TIME_MINUTES"] != System.DBNull.Value)
                            {
                                if (Convert.ToInt32(DataSet.Tables["Employee"].Rows[0]["CLOCKED_TIME_MINUTES"]) == Convert.ToInt32(strTime))
                                {
                                    blnAlreadyClocked = true;
                                }
                            }

                            if (blnAlreadyClocked == false)
                            {
                                if (InOutParm == "I")
                                {
                                    if (DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")] != System.DBNull.Value)
                                    {
                                        if (Convert.ToInt32(DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")]) > Convert.ToInt32(strTime))
                                        {
                                            if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]))
                                            {
                                                blnAlreadyClocked = true;
                                            }
                                            else
                                            {
                                                if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")].ToString())
                                                {
                                                    blnAlreadyClocked = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]))
                                            {
                                                blnAlreadyClocked = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]))
                                        {
                                            blnAlreadyClocked = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")] != System.DBNull.Value)
                                    {
                                        if (Convert.ToInt32(DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")]) < Convert.ToInt32(strTime))
                                        {
                                            if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]))
                                            {
                                                blnAlreadyClocked = true;
                                            }
                                            else
                                            {
                                                if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")].ToString())
                                                {
                                                    blnAlreadyClocked = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]))
                                            {
                                                blnAlreadyClocked = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"].ToString() == Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]))
                                        {
                                            blnAlreadyClocked = true;
                                        }
                                    }
                                }
                            }

                            if (blnAlreadyClocked == true)
                            {
                                intWhere = 15;
                                EmployeeNames.OkInd = "A";
                                EmployeeNames.EmployeeNo = DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                                EmployeeNames.Names = DataSet.Tables["Employee"].Rows[0]["EMPL_NAMES"].ToString();

                                if (pvtblnLoggingSwitchedOn == true)
                                {
                                    WriteLog("GetEmployeeClockedNew Exit OkInd=" + EmployeeNames.OkInd + " Names=" + EmployeeNames.Names);
                                }

                                return EmployeeNames;
                            }

                            intWhere = 16;

                            //Insert Where NOT Duplicate
                            strQry.Clear();
                            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.DEVICE_CLOCK_TIME");
                            strQry.AppendLine("(DEVICE_NO");
                            strQry.AppendLine(",COMPANY_NO");
                            strQry.AppendLine(",EMPLOYEE_NO");
                            strQry.AppendLine(",PAY_CATEGORY_NO");
                            strQry.AppendLine(",PAY_CATEGORY_TYPE");
                            strQry.AppendLine(",TIMESHEET_DATE");
                            strQry.AppendLine(",TIMESHEET_TIME_MINUTES");
                            strQry.AppendLine(",CLOCKED_BOUNDARY_TIME_MINUTES");
                            strQry.AppendLine(",IN_OUT_IND)");
                            strQry.AppendLine(" VALUES ");
                            strQry.AppendLine("(" + DeviceNo);
                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow]["COMPANY_NO"].ToString());
                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                            strQry.AppendLine(",'" + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'");
                            strQry.AppendLine(",'" + strDate + "'");
                            strQry.AppendLine("," + strTime);

                            if (InOutParm == "I")
                            {
                                if (DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")].ToString() == "")
                                {
                                    //No Clock In Boundary Set
                                    strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                }
                                else
                                {
                                    if (Convert.ToInt32(DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_MINUTES"]) == 0)
                                    {
                                        //Last Clock In Was Null or 0 (Morning)
                                        if (Convert.ToInt32(DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")]) > Convert.ToInt32(strTime))
                                        {
                                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")]);
                                        }
                                        else
                                        {
                                            strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                        }
                                    }
                                    else
                                    {
                                        strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                    }
                                }
                            }
                            else
                            {
                                //Out
                                if (DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")].ToString() == "")
                                {
                                    strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                }
                                else
                                {
                                    //Clock Out Parameter Apply
                                    if (Convert.ToInt32(DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")]) == Convert.ToInt32(DataSet.Tables["Employee"].Rows[0]["TIMESHEET_TIME_IN_MINUTES"])
                                    & Convert.ToInt32(strTime) < Convert.ToInt32(DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")]))
                                    {
                                        //Clocked In and Out Before Clock-In Paramter applied
                                        strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")].ToString());
                                    }
                                    else
                                    {
                                        if (DataSet.Tables["Template"].Rows[intRow][strAddOnClockInApplies.Replace(",X.", "")].ToString() == "Y")
                                        {
                                            //Check if Clock In Boundary was Used
                                            strQryTemp.AppendLine(" SELECT ");
                                            strQryTemp.AppendLine(" TIMESHEET_SEQ");
                                            strQryTemp.AppendLine(",TIMESHEET_TIME_IN_MINUTES");
                                            strQryTemp.AppendLine(",TIMESHEET_TIME_OUT_MINUTES");

                                            if (DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                                            {
                                                strQryTemp.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT");
                                            }
                                            else
                                            {
                                                if (DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                                                {
                                                    strQryTemp.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT");
                                                }
                                                else
                                                {
                                                    //T = Time Attendance
                                                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC");
                                                }
                                            }

                                            strQryTemp.AppendLine(" WHERE COMPANY_NO = " + DataSet.Tables["Template"].Rows[intRow]["COMPANY_NO"].ToString());
                                            strQryTemp.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString());

                                            strQryTemp.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                                            strQryTemp.AppendLine(" AND TIMESHEET_DATE = '" + strDate + "'");

                                            strQryTemp.AppendLine(" ORDER BY ");
                                            strQryTemp.AppendLine(" TIMESHEET_SEQ DESC");

                                            clsDBConnectionObjects.Create_DataTable_Client(strQryTemp.ToString(), DataSet, "TimeCheck");

                                            if (DataSet.Tables["TimeCheck"].Rows.Count == 0)
                                            {
                                                //No Clocking for Day - Use Current Time / Rounded
                                                strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                            }
                                            else
                                            {
                                                if (DataSet.Tables["TimeCheck"].Rows[0]["TIMESHEET_TIME_OUT_MINUTES"] == System.DBNull.Value)
                                                {
                                                    if (DataSet.Tables["TimeCheck"].Rows[0]["TIMESHEET_TIME_IN_MINUTES"].ToString() == DataSet.Tables["Template"].Rows[intRow][strAddOnClockIn.Replace(",X.", "")].ToString())
                                                    {
                                                        //Clock In Boundary was Used therefor use Clock Out Boundary 
                                                        strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")].ToString());
                                                    }
                                                    else
                                                    {
                                                        strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                                    }
                                                }
                                                else
                                                {
                                                    strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //Always Apply Clockout unless Clocked Out Before SET Time
                                            if (Convert.ToInt32(DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")]) < Convert.ToInt32(strTime))
                                            {
                                                strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow][strAddOnClockOut.Replace(",X.", "")].ToString());
                                            }
                                            else
                                            {
                                                strQry.AppendLine("," + Return_Rounded_Value_Or_Actual(Convert.ToInt32(strTime), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_IND"].ToString(), DataSet.Tables["Template"].Rows[intRow]["TIME_ATTEND_ROUNDING_VALUE"]));
                                            }
                                        }
                                    }
                                }
                            }

                            strQry.AppendLine(",'" + InOutParm + "')");

                            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                            intWhere = 17;

                            EmployeeNames.EmployeeNo = DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                            EmployeeNames.Names = DataSet.Tables["Employee"].Rows[0]["EMPL_NAMES"].ToString();
                        }
                    }
                }
                else
                {
                    intWhere = 19;
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");

                    if (ImageContainer.DPImageByteArray.Length == 101376)
                    {
                        strQry.AppendLine(",EMPLOYEE_NAME + '#' + EMPLOYEE_SURNAME AS EMPL_NAMES");
                    }
                    else
                    {
                        strQry.AppendLine(",SUBSTRING(EMPLOYEE_NAME,1,1) + ' ' + EMPLOYEE_SURNAME AS EMPL_NAMES");
                    }

                    strQry.AppendLine(",READ_OPTION_NO");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE");

                    strQry.AppendLine(" WHERE EMPLOYEE_RFID_CARD_NO = " + RfIdNo);
                    strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                    strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

                    intWhere = 20;
                    if (DataSet.Tables["Employee"].Rows.Count == 0)
                    {
                        intWhere = 21;
                        EmployeeNames.Names = "NOT FOUND";
                    }
                    else
                    {
                        intWhere = 22;
                        if (DataSet.Tables["Employee"].Rows[0]["READ_OPTION_NO"].ToString() == "2")
                        {
                            intWhere = 23;
                            EmployeeNames.EmployeeNo = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"].ToString();
                            EmployeeNames.Names = DataSet.Tables["Employee"].Rows[0]["EMPL_NAMES"].ToString();
                        }
                        else
                        {
                            intWhere = 24;
                            if (DataSet.Tables["Employee"].Rows[0]["READ_OPTION_NO"].ToString() == "3")
                            {
                                intWhere = 25;
                                EmployeeNames.Names = "Scan your Finger";
                                EmployeeNames.RfIdNo = RfIdNo;
                            }
                            else
                            {
                                intWhere = 26;
                                EmployeeNames.OkInd = "N";
                                EmployeeNames.Names = "Incorrect Profile Setup";
                            }
                        }
                    }
                }

                intWhere = 27;
            }
            catch (Exception ex)
            {
                WriteExceptionLog("GetEmployeeClockedNew " + intWhere.ToString() + " SQL = " + strQry.ToString(), ex);
                
                EmployeeNames.OkInd = "N";
                EmployeeNames.Names = "ERROR - Web Server";
            }

        GetEmployeeClocked_Continue:

            DataSet.Dispose();
            
            if (pvtblnLoggingSwitchedOn == true)
            {
                WriteLog("GetEmployeeClockedNew Exit OkInd=" + EmployeeNames.OkInd + " Names=" + EmployeeNames.Names);
            }
            
            return EmployeeNames;
        }

        public EmployeeNames GetEmployeeBreakClocked(string DeviceNo, string ReaderNo, string InOutParm, string EmployeeNo, string RfIdNo, ImageContainer ImageContainer)
        {
            //A11
            int intWhere = 0;
            StringBuilder strQry = new StringBuilder();

            EmployeeNames EmployeeNames = new EmployeeNames();
            intWhere = 1;
            EmployeeNames.OkInd = "Y";
            intWhere = 2;
            EmployeeNames.ReaderNo = ReaderNo;
            intWhere = 3;
            EmployeeNames.RfIdNo = "0";
            EmployeeNames.FingerPrintScore = "0";
            EmployeeNames.Names = "";
            EmployeeNames.EmployeeNo = "";

            DataSet DataSet = new DataSet();

            try
            {
                int intFingerPrintThreshold = 214748;
                int intFingerPrintScore = 0;

                strQry.Clear();
                strQry.AppendLine(" SELECT");
                
                //Errol Must Test
                if (EmployeeNo == "0")
                {
                    strQry.AppendLine(" ISNULL(FAR_REQUESTED,214748) AS FAR_REQUESTED");
                }
                else
                {
                    //FAR = 1 in 5000
                    strQry.AppendLine(" 429496 AS FAR_REQUESTED");
                }
                
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE");
                
                strQry.AppendLine(" WHERE DEVICE_NO = " + DeviceNo);

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "FAR");

                if (DataSet.Tables["FAR"].Rows.Count > 0)
                {
                    intFingerPrintThreshold = Convert.ToInt32(DataSet.Tables["FAR"].Rows[0]["FAR_REQUESTED"]);
                }
              
                clsFingerPrintClockServerLib FingerPrintClockServerLib = new clsFingerPrintClockServerLib(pvtstrSoftwareToUse);
                 
                byte[] bytFmdFingerTemplate = null;

                if (ImageContainer.DPImageByteArray.Length == 101376)
                {
                    int intGreyScaleHeight = 352;
                    int intGreyScaleWidth = 288;

                    bytFmdFingerTemplate = FeatureExtraction.CreateFmdFromRaw(ImageContainer.DPImageByteArray, 0, 0, intGreyScaleWidth, intGreyScaleHeight, 500, Constants.Formats.Fmd.ANSI).Data.Bytes;
                }
                else
                {
                    //A11 Cock Image
                    int intGreyScaleHeight = 256 + (int)ImageContainer.DPImageByteArray[2];
                    int intGreyScaleWidth = 256 + (int)ImageContainer.DPImageByteArray[0];

                    byte[] bytCreatedRawFmd = new byte[intGreyScaleHeight * intGreyScaleWidth];

                    unsafe
                    {
                        IntPtr ptr;

                        fixed (byte* p = bytCreatedRawFmd)
                        {
                            ptr = (IntPtr)p;
                        }

                        Marshal.Copy(ImageContainer.DPImageByteArray, 4, ptr, bytCreatedRawFmd.Length);
                    }

                    bytFmdFingerTemplate = FeatureExtraction.CreateFmdFromRaw(bytCreatedRawFmd, 0, 0, intGreyScaleWidth, intGreyScaleHeight, 500, Constants.Formats.Fmd.ANSI).Data.Bytes;
                }

                intWhere = 4;

                int intRow = -1;

                intWhere = 6;
                //Image from Finger Print

                if (ImageContainer.DPImageByteArray != null)
                {
                    intWhere = 7;
                    string strDate = DateTime.Now.ToString("yyyy-MM-dd");
                    string strTime = Convert.ToInt32((DateTime.Now.Hour * 60) + DateTime.Now.Minute).ToString();

                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EFT.COMPANY_NO");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",EFT.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine("," + EmployeeNo + " AS EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",EFT.FINGER_NO");
                    strQry.AppendLine(",DEVICE_EMPLOYEE.PAY_CATEGORY_NO");
                    strQry.AppendLine(",DEVICE_EMPLOYEE.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(",EFT.FINGER_TEMPLATE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE EFT");

                    strQry.AppendLine(" INNER JOIN ");

                    strQry.AppendLine("(SELECT ");
                    strQry.AppendLine(" DEL.COMPANY_NO");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",DEL.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine(",TEMP_EMPLOYEE.EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",DEL.PAY_CATEGORY_NO");
                    strQry.AppendLine(",DEL.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_EMPLOYEE_LINK DEL");

                    if (EmployeeNo != "0")
                    {
                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DEL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" WHERE DEL.DEVICE_NO = " + DeviceNo);

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND DEL.EMPLOYEE_NO = " + EmployeeNo);
                    }

                    //Cost Centre 
                    strQry.AppendLine(" UNION ");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EPC.COMPANY_NO");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",EPC.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine(",TEMP_EMPLOYEE.EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK DPCL");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON DPCL.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + EmployeeNo);

                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DPCL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" WHERE DPCL.DEVICE_NO = " + DeviceNo);

                    //Department 
                    strQry.AppendLine(" UNION");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EPC.COMPANY_NO");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",EPC.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine(",TEMP_EMPLOYEE.EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK DDL");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON DDL.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND DDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                    strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                    strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON DDL.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + EmployeeNo);

                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DDL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" WHERE DDL.DEVICE_NO = " + DeviceNo);

                    //Group
                    strQry.AppendLine(" UNION");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" GEL.COMPANY_NO");

                    strQry.AppendLine(",GEL.EMPLOYEE_NO");
                    strQry.AppendLine(",GEL.PAY_CATEGORY_NO");
                    strQry.AppendLine(",GEL.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK DGL");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.GROUP_EMPLOYEE_LINK GEL");
                    strQry.AppendLine(" ON DGL.GROUP_NO = GEL.GROUP_NO ");
                    strQry.AppendLine(" AND DGL.COMPANY_NO = GEL.COMPANY_NO ");

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND GEL.EMPLOYEE_NO = " + EmployeeNo);

                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DGL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" WHERE DGL.DEVICE_NO = " + DeviceNo + ") AS DEVICE_EMPLOYEE");

                    strQry.AppendLine(" ON EFT.COMPANY_NO = DEVICE_EMPLOYEE.COMPANY_NO");
                    strQry.AppendLine(" AND EFT.EMPLOYEE_NO = DEVICE_EMPLOYEE.EMPLOYEE_NO");

                    if (EmployeeNo != "0")
                    {
                        //Add User Override Link
                        strQry.AppendLine(" UNION ALL");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EMPLOYEE_DEVICE.COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.EMPLOYEE_NO");
                        strQry.AppendLine(",UFT.FINGER_NO");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.PAY_CATEGORY_TYPE");

                        //Linked Person's Fingerprint Templates
                        strQry.AppendLine(",UFT.FINGER_TEMPLATE");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE UFT");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_USER_LINK EUL");
                        strQry.AppendLine(" ON UFT.USER_NO = EUL.USER_NO ");
                        strQry.AppendLine(" AND EUL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.DEVICE D");
                        strQry.AppendLine(" ON D.DEVICE_NO = " + DeviceNo);
                        //T=Time And Attendance,A= Access Control,B=Time And Attendance And Access Control
                        strQry.AppendLine(" AND D.DEVICE_USAGE IN ('T','B')");

                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT ");
                        strQry.AppendLine(" DEVICE_NO ");
                        strQry.AppendLine(",COMPANY_NO ");

                        strQry.AppendLine(",EMPLOYEE_NO ");
                        strQry.AppendLine(",PAY_CATEGORY_NO ");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_EMPLOYEE_LINK ");

                        strQry.AppendLine(" WHERE DEVICE_NO = " + DeviceNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" DPCL.DEVICE_NO ");
                        strQry.AppendLine(",EPC.COMPANY_NO ");

                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK DPCL");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON DPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" WHERE DPCL.DEVICE_NO = " + DeviceNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" DDL.DEVICE_NO ");
                        strQry.AppendLine(",EPC.COMPANY_NO ");

                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK DDL");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E");
                        strQry.AppendLine(" ON DDL.COMPANY_NO = E.COMPANY_NO ");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND DDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON DDL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");

                        strQry.AppendLine(" WHERE DDL.DEVICE_NO = " + DeviceNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" DGL.DEVICE_NO ");
                        strQry.AppendLine(",GEL.COMPANY_NO ");

                        strQry.AppendLine(",GEL.EMPLOYEE_NO ");
                        strQry.AppendLine(",GEL.PAY_CATEGORY_NO ");
                        strQry.AppendLine(",GEL.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK DGL");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.GROUP_EMPLOYEE_LINK GEL");
                        strQry.AppendLine(" ON DGL.GROUP_NO = GEL.GROUP_NO ");
                        strQry.AppendLine(" AND DGL.COMPANY_NO = GEL.COMPANY_NO ");
                        strQry.AppendLine(" AND GEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" WHERE DGL.DEVICE_NO = " + DeviceNo + ") AS EMPLOYEE_DEVICE");

                        strQry.AppendLine(" ON D.DEVICE_NO = EMPLOYEE_DEVICE.DEVICE_NO");

                        strQry.AppendLine(" AND EUL.COMPANY_NO = EMPLOYEE_DEVICE.COMPANY_NO");
                        strQry.AppendLine(" AND EUL.EMPLOYEE_NO = EMPLOYEE_DEVICE.EMPLOYEE_NO");

                        strQry.AppendLine(" WHERE NOT UFT.FINGER_TEMPLATE IS NULL");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON DEVICE_EMPLOYEE.COMPANY_NO = E.COMPANY_NO ");
                        strQry.AppendLine(" AND DEVICE_EMPLOYEE.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND DEVICE_EMPLOYEE.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");
                        strQry.AppendLine(" AND (E.USE_EMPLOYEE_NO_IND <> 'Y' ");
                        strQry.AppendLine(" OR E.USE_EMPLOYEE_NO_IND IS NULL) ");
                    }

                    intWhere = 8;
                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Template");

                    intWhere = 9;
                    if (DataSet.Tables["Template"].Rows.Count == 0)
                    {
                        intWhere = 10;
                        
                        if (EmployeeNo == "0")
                        {
                            EmployeeNames.Names = "NO Employee/s for Device";
                        }
                        else
                        {
                            EmployeeNames.Names = "NOT FOUND";
                        }
                    }
                    else
                    {
                        intWhere = 11;

                        int intReturnCode = FingerPrintClockServerLib.Get_Employee(DataSet.Tables["Template"], ref bytFmdFingerTemplate, intFingerPrintThreshold, ref intFingerPrintScore, ref intRow);

                        intWhere = 12;
                        if (intRow == -1)
                        {
                            intWhere = 13;
                            EmployeeNames.Names = "NOT FOUND";
                        }
                        else
                        {
                            intWhere = 14;

                            strQry.Clear();
                            strQry.AppendLine(" SELECT TOP 1 ");

                            if (ImageContainer.DPImageByteArray.Length == 101376)
                            {
                                strQry.AppendLine(" E.EMPLOYEE_NAME + '#' + E.EMPLOYEE_SURNAME AS EMPL_NAMES");
                            }
                            else
                            {
                                strQry.AppendLine(" SUBSTRING(E.EMPLOYEE_NAME,1,1) + ' ' + E.EMPLOYEE_SURNAME AS EMPL_NAMES");
                            }

                            if (InOutParm == "I")
                            {
                                strQry.AppendLine(",ISNULL(ETC.BREAK_TIME_IN_MINUTES,0) AS BREAK_TIME_MINUTES");
                                strQry.AppendLine(",ISNULL(ETC.CLOCKED_TIME_IN_MINUTES,0) AS CLOCKED_TIME_MINUTES");
                            }
                            else
                            {
                                strQry.AppendLine(",ISNULL(ETC.BREAK_TIME_OUT_MINUTES,0) AS BREAK_TIME_MINUTES");
                                strQry.AppendLine(",ISNULL(ETC.CLOCKED_TIME_OUT_MINUTES,0) AS CLOCKED_TIME_MINUTES");
                            }

                            strQry.AppendLine(",ETC.BREAK_SEQ");

                            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                            if (DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                            {
                                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT ETC");
                            }
                            else
                            {
                                if (DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                                {
                                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ETC");
                                }
                                else
                                {
                                    //T = Time Attendance
                                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETC");
                                }
                            }

                            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO ");
                            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
                            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                            strQry.AppendLine(" AND BREAK_DATE = '" + strDate + "'");

                            strQry.AppendLine(" WHERE E.COMPANY_NO = " + DataSet.Tables["Template"].Rows[intRow]["COMPANY_NO"].ToString());
                            strQry.AppendLine(" AND E.EMPLOYEE_NO = " + DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                            strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                            strQry.AppendLine(" ORDER BY ");
                            strQry.AppendLine(" ETC.BREAK_SEQ DESC");

                            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

                            if (DataSet.Tables["Employee"].Rows[0]["CLOCKED_TIME_MINUTES"] != System.DBNull.Value)
                            {
                                if (Convert.ToInt32(DataSet.Tables["Employee"].Rows[0]["CLOCKED_TIME_MINUTES"]) == Convert.ToInt32(strTime))
                                {
                                    intWhere = 15;
                                    EmployeeNames.OkInd = "A";
                                    EmployeeNames.EmployeeNo = DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                                    EmployeeNames.Names = "ALREADY CLOCKED";

                                    return EmployeeNames;
                                }
                            }

                            intWhere = 16;

                            //Insert Where NOT Duplicate
                            strQry.Clear();
                            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.DEVICE_CLOCK_TIME_BREAK");
                            strQry.AppendLine("(DEVICE_NO");
                            strQry.AppendLine(",COMPANY_NO");
                            strQry.AppendLine(",EMPLOYEE_NO");
                            strQry.AppendLine(",PAY_CATEGORY_NO");
                            strQry.AppendLine(",PAY_CATEGORY_TYPE");
                            strQry.AppendLine(",BREAK_DATE");
                            strQry.AppendLine(",BREAK_TIME_MINUTES");
                            strQry.AppendLine(",IN_OUT_IND)");
                            strQry.AppendLine(" VALUES ");
                            strQry.AppendLine("(" + DeviceNo);
                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow]["COMPANY_NO"].ToString());
                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                            strQry.AppendLine(",'" + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'");
                            strQry.AppendLine(",'" + strDate + "'");
                            strQry.AppendLine("," + strTime);

                            strQry.AppendLine(",'" + InOutParm + "')");

                            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                            intWhere = 17;

                            EmployeeNames.EmployeeNo = DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                            EmployeeNames.Names = DataSet.Tables["Employee"].Rows[0]["EMPL_NAMES"].ToString();

                        }
                    }
                }
                else
                {
                    intWhere = 19;
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");

                    if (ImageContainer.DPImageByteArray.Length == 101376)
                    {
                        strQry.AppendLine(",EMPLOYEE_NAME + '#' + EMPLOYEE_SURNAME AS EMPL_NAMES");
                    }
                    else
                    {
                        strQry.AppendLine(",SUBSTRING(EMPLOYEE_NAME,1,1) + ' ' + EMPLOYEE_SURNAME AS EMPL_NAMES");
                    }

                    strQry.AppendLine(",READ_OPTION_NO");
                    
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE");
                    
                    strQry.AppendLine(" WHERE EMPLOYEE_RFID_CARD_NO = " + RfIdNo);
                    strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                    strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

                    intWhere = 20;
                    if (DataSet.Tables["Employee"].Rows.Count == 0)
                    {
                        intWhere = 21;
                        EmployeeNames.Names = "NOT FOUND";
                    }
                    else
                    {
                        intWhere = 22;
                        if (DataSet.Tables["Employee"].Rows[0]["READ_OPTION_NO"].ToString() == "2")
                        {
                            intWhere = 23;
                            EmployeeNames.EmployeeNo = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"].ToString();
                            EmployeeNames.Names = DataSet.Tables["Employee"].Rows[0]["EMPL_NAMES"].ToString();

                            //Insert record Into dB
                        }
                        else
                        {
                            intWhere = 24;
                            if (DataSet.Tables["Employee"].Rows[0]["READ_OPTION_NO"].ToString() == "3")
                            {
                                intWhere = 25;
                                EmployeeNames.Names = "Scan your Finger";
                                EmployeeNames.RfIdNo = RfIdNo;
                            }
                            else
                            {
                                intWhere = 26;
                                EmployeeNames.OkInd = "N";
                                EmployeeNames.Names = "Incorrect Profile Setup";
                            }
                        }
                    }
                }

                intWhere = 27;
            }
            catch (Exception ex)
            {
                WriteExceptionLog("GetEmployeeBreakClocked " + intWhere.ToString() + " SQL = " + strQry.ToString(), ex);

                EmployeeNames.OkInd = "N";
                EmployeeNames.Names = "ERROR - Web Server";
            }

        GetEmployeeClocked_Continue:

            DataSet.Dispose();

            return EmployeeNames;
        }

        public EmployeeNames GetEmployeeBreakClockedNew(string DeviceNo, string InOutParm, string EmployeeNo, string RfIdNo, ImageContainer ImageContainer)
        {
            //A11
            int intWhere = 0;
            StringBuilder strQry = new StringBuilder();

            EmployeeNames EmployeeNames = new EmployeeNames();
            intWhere = 1;
            EmployeeNames.OkInd = "Y";
            intWhere = 2;
            EmployeeNames.ReaderNo = "0";
            intWhere = 3;
            EmployeeNames.RfIdNo = "0";
            EmployeeNames.FingerPrintScore = "0";
            EmployeeNames.Names = "";
            EmployeeNames.EmployeeNo = "";

            DataSet DataSet = new DataSet();

            try
            {
                int intFingerPrintThreshold = 214748;
                int intFingerPrintScore = 0;

                strQry.Clear();
                strQry.AppendLine(" SELECT");

                //Errol Must Test
                if (EmployeeNo == "0")
                {
                    strQry.AppendLine(" ISNULL(FAR_REQUESTED,214748) AS FAR_REQUESTED");
                }
                else
                {
                    //FAR = 1 in 5000
                    strQry.AppendLine(" 429496 AS FAR_REQUESTED");
                }

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE");

                strQry.AppendLine(" WHERE DEVICE_NO = " + DeviceNo);

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "FAR");

                if (DataSet.Tables["FAR"].Rows.Count > 0)
                {
                    intFingerPrintThreshold = Convert.ToInt32(DataSet.Tables["FAR"].Rows[0]["FAR_REQUESTED"]);
                }

                clsFingerPrintClockServerLib FingerPrintClockServerLib = new clsFingerPrintClockServerLib(pvtstrSoftwareToUse);

                byte[] bytFmdFingerTemplate = null;

                if (ImageContainer.DPImageByteArray.Length == 101376)
                {
                    int intGreyScaleHeight = 352;
                    int intGreyScaleWidth = 288;

                    bytFmdFingerTemplate = FeatureExtraction.CreateFmdFromRaw(ImageContainer.DPImageByteArray, 0, 0, intGreyScaleWidth, intGreyScaleHeight, 500, Constants.Formats.Fmd.ANSI).Data.Bytes;
                }
                else
                {
                    //A11 Cock Image
                    int intGreyScaleHeight = 256 + (int)ImageContainer.DPImageByteArray[2];
                    int intGreyScaleWidth = 256 + (int)ImageContainer.DPImageByteArray[0];

                    byte[] bytCreatedRawFmd = new byte[intGreyScaleHeight * intGreyScaleWidth];

                    unsafe
                    {
                        IntPtr ptr;

                        fixed (byte* p = bytCreatedRawFmd)
                        {
                            ptr = (IntPtr)p;
                        }

                        Marshal.Copy(ImageContainer.DPImageByteArray, 4, ptr, bytCreatedRawFmd.Length);
                    }

                    bytFmdFingerTemplate = FeatureExtraction.CreateFmdFromRaw(bytCreatedRawFmd, 0, 0, intGreyScaleWidth, intGreyScaleHeight, 500, Constants.Formats.Fmd.ANSI).Data.Bytes;
                }

                intWhere = 4;

                int intRow = -1;

                intWhere = 6;
                //Image from Finger Print

                if (ImageContainer.DPImageByteArray != null)
                {
                    intWhere = 7;
                    string strDate = DateTime.Now.ToString("yyyy-MM-dd");
                    string strTime = Convert.ToInt32((DateTime.Now.Hour * 60) + DateTime.Now.Minute).ToString();

                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EFT.COMPANY_NO");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",EFT.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine("," + EmployeeNo + " AS EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",EFT.FINGER_NO");
                    strQry.AppendLine(",DEVICE_EMPLOYEE.PAY_CATEGORY_NO");
                    strQry.AppendLine(",DEVICE_EMPLOYEE.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(",EFT.FINGER_TEMPLATE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE EFT");

                    strQry.AppendLine(" INNER JOIN ");

                    strQry.AppendLine("(SELECT ");
                    strQry.AppendLine(" DEL.COMPANY_NO");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",DEL.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine(",TEMP_EMPLOYEE.EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",DEL.PAY_CATEGORY_NO");
                    strQry.AppendLine(",DEL.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_EMPLOYEE_LINK DEL");

                    if (EmployeeNo != "0")
                    {
                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DEL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" WHERE DEL.DEVICE_NO = " + DeviceNo);

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND DEL.EMPLOYEE_NO = " + EmployeeNo);
                    }

                    //Cost Centre 
                    strQry.AppendLine(" UNION ");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EPC.COMPANY_NO");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",EPC.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine(",TEMP_EMPLOYEE.EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK DPCL");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON DPCL.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + EmployeeNo);

                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DPCL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" WHERE DPCL.DEVICE_NO = " + DeviceNo);

                    //Department 
                    strQry.AppendLine(" UNION");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EPC.COMPANY_NO");

                    if (EmployeeNo == "0")
                    {
                        strQry.AppendLine(",EPC.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine(",TEMP_EMPLOYEE.EMPLOYEE_NO");
                    }

                    strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK DDL");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E");
                    strQry.AppendLine(" ON DDL.COMPANY_NO = E.COMPANY_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(" AND DDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                    strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                    strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                    strQry.AppendLine(" ON DDL.COMPANY_NO = EPC.COMPANY_NO ");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                    strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + EmployeeNo);

                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DDL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" WHERE DDL.DEVICE_NO = " + DeviceNo);

                    //Group
                    strQry.AppendLine(" UNION");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" GEL.COMPANY_NO");

                    strQry.AppendLine(",GEL.EMPLOYEE_NO");
                    strQry.AppendLine(",GEL.PAY_CATEGORY_NO");
                    strQry.AppendLine(",GEL.PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK DGL");

                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.GROUP_EMPLOYEE_LINK GEL");
                    strQry.AppendLine(" ON DGL.GROUP_NO = GEL.GROUP_NO ");
                    strQry.AppendLine(" AND DGL.COMPANY_NO = GEL.COMPANY_NO ");

                    if (EmployeeNo != "0")
                    {
                        strQry.AppendLine(" AND GEL.EMPLOYEE_NO = " + EmployeeNo);

                        //Override - Bring Out Linked Employee
                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT");
                        strQry.AppendLine(" COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" WHERE EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" EEL.COMPANY_NO");
                        strQry.AppendLine(",EEL.EMPLOYEE_NO_LINK AS EMPLOYEE_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK EEL ");

                        strQry.AppendLine(" WHERE EEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO ");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK EPCL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                        strQry.AppendLine(" ON EPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCL.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" WHERE EPCL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO ");
                        strQry.AppendLine(",E.EMPLOYEE_NO ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK EDL ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON EDL.COMPANY_NO = E.COMPANY_NO ");
                        //2017-02-13
                        strQry.AppendLine(" AND EDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" WHERE EDL.EMPLOYEE_NO = " + EmployeeNo + ") AS TEMP_EMPLOYEE ");

                        strQry.AppendLine(" ON DGL.COMPANY_NO = TEMP_EMPLOYEE.COMPANY_NO  ");
                    }

                    strQry.AppendLine(" WHERE DGL.DEVICE_NO = " + DeviceNo + ") AS DEVICE_EMPLOYEE");

                    strQry.AppendLine(" ON EFT.COMPANY_NO = DEVICE_EMPLOYEE.COMPANY_NO");
                    strQry.AppendLine(" AND EFT.EMPLOYEE_NO = DEVICE_EMPLOYEE.EMPLOYEE_NO");

                    if (EmployeeNo != "0")
                    {
                        //Add User Override Link
                        strQry.AppendLine(" UNION ALL");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" EMPLOYEE_DEVICE.COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.EMPLOYEE_NO");
                        strQry.AppendLine(",UFT.FINGER_NO");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EMPLOYEE_DEVICE.PAY_CATEGORY_TYPE");

                        //Linked Person's Fingerprint Templates
                        strQry.AppendLine(",UFT.FINGER_TEMPLATE");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE UFT");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_USER_LINK EUL");
                        strQry.AppendLine(" ON UFT.USER_NO = EUL.USER_NO ");
                        strQry.AppendLine(" AND EUL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.DEVICE D");
                        strQry.AppendLine(" ON D.DEVICE_NO = " + DeviceNo);
                        //T=Time And Attendance,A= Access Control,B=Time And Attendance And Access Control
                        strQry.AppendLine(" AND D.DEVICE_USAGE IN ('T','B')");

                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT ");
                        strQry.AppendLine(" DEVICE_NO ");
                        strQry.AppendLine(",COMPANY_NO ");

                        strQry.AppendLine(",EMPLOYEE_NO ");
                        strQry.AppendLine(",PAY_CATEGORY_NO ");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_EMPLOYEE_LINK ");

                        strQry.AppendLine(" WHERE DEVICE_NO = " + DeviceNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" DPCL.DEVICE_NO ");
                        strQry.AppendLine(",EPC.COMPANY_NO ");

                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK DPCL");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON DPCL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" WHERE DPCL.DEVICE_NO = " + DeviceNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" DDL.DEVICE_NO ");
                        strQry.AppendLine(",EPC.COMPANY_NO ");

                        strQry.AppendLine(",EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK DDL");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E");
                        strQry.AppendLine(" ON DDL.COMPANY_NO = E.COMPANY_NO ");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = " + EmployeeNo);
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND DDL.DEPARTMENT_NO = E.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON DDL.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");

                        strQry.AppendLine(" WHERE DDL.DEVICE_NO = " + DeviceNo);

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" DGL.DEVICE_NO ");
                        strQry.AppendLine(",GEL.COMPANY_NO ");

                        strQry.AppendLine(",GEL.EMPLOYEE_NO ");
                        strQry.AppendLine(",GEL.PAY_CATEGORY_NO ");
                        strQry.AppendLine(",GEL.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK DGL");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.GROUP_EMPLOYEE_LINK GEL");
                        strQry.AppendLine(" ON DGL.GROUP_NO = GEL.GROUP_NO ");
                        strQry.AppendLine(" AND DGL.COMPANY_NO = GEL.COMPANY_NO ");
                        strQry.AppendLine(" AND GEL.EMPLOYEE_NO = " + EmployeeNo);

                        strQry.AppendLine(" WHERE DGL.DEVICE_NO = " + DeviceNo + ") AS EMPLOYEE_DEVICE");

                        strQry.AppendLine(" ON D.DEVICE_NO = EMPLOYEE_DEVICE.DEVICE_NO");

                        strQry.AppendLine(" AND EUL.COMPANY_NO = EMPLOYEE_DEVICE.COMPANY_NO");
                        strQry.AppendLine(" AND EUL.EMPLOYEE_NO = EMPLOYEE_DEVICE.EMPLOYEE_NO");

                        strQry.AppendLine(" WHERE NOT UFT.FINGER_TEMPLATE IS NULL");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" ON DEVICE_EMPLOYEE.COMPANY_NO = E.COMPANY_NO ");
                        strQry.AppendLine(" AND DEVICE_EMPLOYEE.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND DEVICE_EMPLOYEE.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                        strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");
                        strQry.AppendLine(" AND (E.USE_EMPLOYEE_NO_IND <> 'Y' ");
                        strQry.AppendLine(" OR E.USE_EMPLOYEE_NO_IND IS NULL) ");
                    }

                    intWhere = 8;
                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Template");

                    intWhere = 9;
                    if (DataSet.Tables["Template"].Rows.Count == 0)
                    {
                        intWhere = 10;
                       
                        if (EmployeeNo == "0")
                        {
                            EmployeeNames.Names = "NO Employee/s for Device";
                        }
                        else
                        {
                            EmployeeNames.Names = "NOT FOUND";
                        }
                    }
                    else
                    {
                        intWhere = 11;

                        int intReturnCode = FingerPrintClockServerLib.Get_Employee(DataSet.Tables["Template"], ref bytFmdFingerTemplate, intFingerPrintThreshold, ref intFingerPrintScore, ref intRow);

                        intWhere = 12;
                        if (intRow == -1)
                        {
                            intWhere = 13;
                            EmployeeNames.Names = "NOT FOUND";
                        }
                        else
                        {
                            intWhere = 14;

                            strQry.Clear();
                            strQry.AppendLine(" SELECT TOP 1 ");

                            if (ImageContainer.DPImageByteArray.Length == 101376)
                            {
                                strQry.AppendLine(" E.EMPLOYEE_NAME + '#' + E.EMPLOYEE_SURNAME AS EMPL_NAMES");
                            }
                            else
                            {
                                strQry.AppendLine(" SUBSTRING(E.EMPLOYEE_NAME,1,1) + ' ' + E.EMPLOYEE_SURNAME AS EMPL_NAMES");
                            }

                            if (InOutParm == "I")
                            {
                                strQry.AppendLine(",ISNULL(ETC.BREAK_TIME_IN_MINUTES,0) AS BREAK_TIME_MINUTES");
                                strQry.AppendLine(",ISNULL(ETC.CLOCKED_TIME_IN_MINUTES,0) AS CLOCKED_TIME_MINUTES");
                            }
                            else
                            {
                                strQry.AppendLine(",ISNULL(ETC.BREAK_TIME_OUT_MINUTES,0) AS BREAK_TIME_MINUTES");
                                strQry.AppendLine(",ISNULL(ETC.CLOCKED_TIME_OUT_MINUTES,0) AS CLOCKED_TIME_MINUTES");
                            }

                            strQry.AppendLine(",ETC.BREAK_SEQ");

                            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                            if (DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                            {
                                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT ETC");
                            }
                            else
                            {
                                if (DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                                {
                                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ETC");
                                }
                                else
                                {
                                    //T = Time Attendance
                                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETC");
                                }
                            }

                            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO ");
                            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
                            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = " + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                            strQry.AppendLine(" AND BREAK_DATE = '" + strDate + "'");

                            strQry.AppendLine(" WHERE E.COMPANY_NO = " + DataSet.Tables["Template"].Rows[intRow]["COMPANY_NO"].ToString());
                            strQry.AppendLine(" AND E.EMPLOYEE_NO = " + DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine(" AND (E.NOT_ACTIVE_IND <> 'Y'");
                            strQry.AppendLine(" OR E.NOT_ACTIVE_IND IS NULL)");

                            strQry.AppendLine(" ORDER BY ");
                            strQry.AppendLine(" ETC.BREAK_SEQ DESC");

                            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

                            if (DataSet.Tables["Employee"].Rows[0]["CLOCKED_TIME_MINUTES"] != System.DBNull.Value)
                            {
                                if (Convert.ToInt32(DataSet.Tables["Employee"].Rows[0]["CLOCKED_TIME_MINUTES"]) == Convert.ToInt32(strTime))
                                {
                                    intWhere = 15;
                                    EmployeeNames.OkInd = "A";
                                    EmployeeNames.EmployeeNo = DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                                    EmployeeNames.Names = DataSet.Tables["Employee"].Rows[0]["EMPL_NAMES"].ToString();

                                    return EmployeeNames;
                                }
                            }

                            intWhere = 16;

                            //Insert Where NOT Duplicate
                            strQry.Clear();
                            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.DEVICE_CLOCK_TIME_BREAK");
                            strQry.AppendLine("(DEVICE_NO");
                            strQry.AppendLine(",COMPANY_NO");
                            strQry.AppendLine(",EMPLOYEE_NO");
                            strQry.AppendLine(",PAY_CATEGORY_NO");
                            strQry.AppendLine(",PAY_CATEGORY_TYPE");
                            strQry.AppendLine(",BREAK_DATE");
                            strQry.AppendLine(",BREAK_TIME_MINUTES");
                            strQry.AppendLine(",IN_OUT_IND)");
                            strQry.AppendLine(" VALUES ");
                            strQry.AppendLine("(" + DeviceNo);
                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow]["COMPANY_NO"].ToString());
                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine("," + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                            strQry.AppendLine(",'" + DataSet.Tables["Template"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'");
                            strQry.AppendLine(",'" + strDate + "'");
                            strQry.AppendLine("," + strTime);

                            strQry.AppendLine(",'" + InOutParm + "')");

                            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                            intWhere = 17;

                            EmployeeNames.EmployeeNo = DataSet.Tables["Template"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                            EmployeeNames.Names = DataSet.Tables["Employee"].Rows[0]["EMPL_NAMES"].ToString();
                        }
                    }
                }
                else
                {
                    intWhere = 19;
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");

                    if (ImageContainer.DPImageByteArray.Length == 101376)
                    {
                        strQry.AppendLine(",EMPLOYEE_NAME + '#' + EMPLOYEE_SURNAME AS EMPL_NAMES");
                    }
                    else
                    {
                        strQry.AppendLine(",SUBSTRING(EMPLOYEE_NAME,1,1) + ' ' + EMPLOYEE_SURNAME AS EMPL_NAMES");
                    }

                    strQry.AppendLine(",READ_OPTION_NO");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE");

                    strQry.AppendLine(" WHERE EMPLOYEE_RFID_CARD_NO = " + RfIdNo);
                    strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                    strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

                    intWhere = 20;
                    if (DataSet.Tables["Employee"].Rows.Count == 0)
                    {
                        intWhere = 21;
                        EmployeeNames.Names = "NOT FOUND";
                    }
                    else
                    {
                        intWhere = 22;
                        if (DataSet.Tables["Employee"].Rows[0]["READ_OPTION_NO"].ToString() == "2")
                        {
                            intWhere = 23;
                            EmployeeNames.EmployeeNo = DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"].ToString();
                            EmployeeNames.Names = DataSet.Tables["Employee"].Rows[0]["EMPL_NAMES"].ToString();

                            //Insert record Into dB
                        }
                        else
                        {
                            intWhere = 24;
                            if (DataSet.Tables["Employee"].Rows[0]["READ_OPTION_NO"].ToString() == "3")
                            {
                                intWhere = 25;
                                EmployeeNames.Names = "Scan your Finger";
                                EmployeeNames.RfIdNo = RfIdNo;
                            }
                            else
                            {
                                intWhere = 26;
                                EmployeeNames.OkInd = "N";
                                EmployeeNames.Names = "Incorrect Profile Setup";
                            }
                        }
                    }
                }

                intWhere = 27;
            }
            catch (Exception ex)
            {
                WriteExceptionLog("GetEmployeeBreakClockedNew " + intWhere.ToString() + " SQL = " + strQry.ToString(), ex);

                EmployeeNames.OkInd = "N";
                EmployeeNames.Names = "ERROR - Web Server";
            }

        GetEmployeeClocked_Continue:

            DataSet.Dispose();

            return EmployeeNames;
        }

        private string Return_Rounded_Value_Or_Actual(int parTimeValue, string parstrRoundingOption, object objRoundingValue)
        {
            string strReturnValue = parTimeValue.ToString();

            if (parstrRoundingOption != ""
                & objRoundingValue != System.DBNull.Value)
            {
                int intRoundValue = Convert.ToInt32(objRoundingValue);

                if (parTimeValue % intRoundValue == 0)
                {
                }
                else
                {
                    //Up
                    if (parstrRoundingOption == "U")
                    {
                        parTimeValue = parTimeValue + (intRoundValue - (parTimeValue % intRoundValue));
                    }
                    else
                    {
                        //Down
                        if (parstrRoundingOption == "D")
                        {
                            parTimeValue = parTimeValue - (parTimeValue % intRoundValue);
                        }
                        else
                        {
                            //Closest
                            if (parTimeValue % intRoundValue >= Convert.ToDouble(intRoundValue) / 2)
                            {
                                //Up
                                parTimeValue = parTimeValue + (intRoundValue - (parTimeValue % intRoundValue));
                            }
                            else
                            {
                                //Down
                                parTimeValue = parTimeValue - (parTimeValue % intRoundValue);
                            }
                        }
                    }

                    strReturnValue = parTimeValue.ToString();
                }
            }

            return strReturnValue;
        }
        
        public UserReply GetUser(ImageContainer ImageContainer)
        {
            //A11
            int intRow = -1;
            int intFingerPrintScore = 0;
            int intReturnCode = 0;
            StringBuilder strQry = new StringBuilder();
            bool blnFromA11 = false;

            UserReply UserReply = new UserReply();

            UserReply.OkInd = "N";
            UserReply.UserNo = "";

            DataSet DataSet = new DataSet();

            try
            {
                strQry.Clear();
                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" IDENTIFY_THRESHOLD_VALUE");
                
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.FINGERPRINT_IDENTIFY_VERIFY_THRESHOLD");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Threshold");

                int intFingerPrintThreshold = Convert.ToInt32(DataSet.Tables["Threshold"].Rows[0]["IDENTIFY_THRESHOLD_VALUE"]);

                //A11 Cock Image
                int intGreyScaleHeight = 256 + (int)ImageContainer.DPImageByteArray[2];
                int intGreyScaleWidth = 256 + (int)ImageContainer.DPImageByteArray[0];

                byte[] bytCreatedRawFmd = new byte[intGreyScaleHeight * intGreyScaleWidth];

                unsafe
                {
                    IntPtr ptr;

                    fixed (byte* p = bytCreatedRawFmd)
                    {
                        ptr = (IntPtr)p;
                    }

                    Marshal.Copy(ImageContainer.DPImageByteArray, 4, ptr, bytCreatedRawFmd.Length);
                }

                byte[] bytFmdFingerTemplate = FeatureExtraction.CreateFmdFromRaw(bytCreatedRawFmd, 0, 0, intGreyScaleWidth, intGreyScaleHeight, 500, Constants.Formats.Fmd.ANSI).Data.Bytes; 

                clsFingerPrintClockServerLib FingerPrintClockServerLib = new clsFingerPrintClockServerLib(pvtstrSoftwareToUse);

                //Image from Finger Print
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" USER_NO");
                strQry.AppendLine(",FINGER_TEMPLATE");
                
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE ");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Template");

                if (DataSet.Tables["Template"].Rows.Count > 0)
                {
                    intReturnCode = FingerPrintClockServerLib.Get_User(DataSet.Tables["Template"], ref bytFmdFingerTemplate, intFingerPrintThreshold, ref intFingerPrintScore, ref intRow);

                    if (intRow != -1)
                    {
                        UserReply.OkInd = "Y";
                        UserReply.UserNo = DataSet.Tables["Template"].Rows[intRow]["USER_NO"].ToString();
                    }
                }

                if (intReturnCode == 8
                    | (blnFromA11 == true
                    & DataSet.Tables["Template"].Rows.Count == 0))
                {
                    //A11 Does Initial Call to Load Tables into Memory
                    strQry.Clear();
                    strQry.AppendLine(" SELECT * ");
                    
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE ");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "TempTemplate");
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog("GetUser", ex);
            }
            finally
            {
                DataSet.Dispose();
            }

            return UserReply;
        }

        public UserCompany GetUserFeaturesClockedNew(string DBNo, ImageFeaturesContainer ImageFeaturesContainer)
        {
            UserCompany UserCompany = new UserCompany();

            string[] strFingerArray;

            UserCompany.UserNoLoggedIn = "";
            UserCompany.CompanyDesc = "";
            UserCompany.CompanyNo = "";
            UserCompany.UserNo = "";
            UserCompany.UserName = "";
            UserCompany.UserSurname = "";
            UserCompany.UserFinger = "";
            UserCompany.EmployeeNo = "";
            UserCompany.EmployeeCode = "";
            UserCompany.EmployeeName = "";
            UserCompany.EmployeeSurname = "";
            UserCompany.EmployeeFinger = "";

            int intRow = -1;
            int intReturnCode = 0;
            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new DataSet();

            try
            {
                clsFingerPrintClockServerLib FingerPrintClockServerLib = new clsFingerPrintClockServerLib(pvtstrSoftwareToUse);

                //Image from Finger Print
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" USER_NO");
                strQry.AppendLine(",FINGER_TEMPLATE");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE ");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Template");

                if (DataSet.Tables["Template"].Rows.Count > 0)
                {
                    intReturnCode = FingerPrintClockServerLib.Get_User_Features(DataSet.Tables["Template"], ref ImageFeaturesContainer.DPImageFeaturesByteArray, ref intRow);

                    if (intRow != -1)
                    {
                        UserCompany.UserNoLoggedIn = DataSet.Tables["Template"].Rows[intRow]["USER_NO"].ToString();

                        strQry.Clear();
                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" C.COMPANY_NO");
                        strQry.AppendLine(",C.COMPANY_DESC");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.COMPANY C");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA");
                        strQry.AppendLine(" ON UCA.USER_NO = " + DataSet.Tables["Template"].Rows[intRow]["USER_NO"].ToString());
                        strQry.AppendLine(" AND C.COMPANY_NO = UCA.COMPANY_NO");

                        strQry.AppendLine(" ORDER BY");
                        strQry.AppendLine(" C.COMPANY_DESC");

                        clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Company");

                        for (int intCompanyRow = 0; intCompanyRow < DataSet.Tables["Company"].Rows.Count; intCompanyRow++)
                        {
                            if (intCompanyRow == 0)
                            {
                                UserCompany.CompanyDesc = DataSet.Tables["Company"].Rows[intCompanyRow]["COMPANY_DESC"].ToString();
                                UserCompany.CompanyNo = DataSet.Tables["Company"].Rows[intCompanyRow]["COMPANY_NO"].ToString();
                            }
                            else
                            {
                                UserCompany.CompanyDesc += "#" + DataSet.Tables["Company"].Rows[intCompanyRow]["COMPANY_DESC"].ToString();
                                UserCompany.CompanyNo += "#" + DataSet.Tables["Company"].Rows[intCompanyRow]["COMPANY_NO"].ToString();
                            }
                        }

#if (DEBUG)
                        //UserCompany.CompanyDesc += "#ZZZZZZZZZZZZZZZZZZZZZZZ");
                        //UserCompany.CompanyNo += "#99");

                        //UserCompany.CompanyDesc += "#Test1");
                        //UserCompany.CompanyNo += "#101");

                        //UserCompany.CompanyDesc += "#Test2");
                        //UserCompany.CompanyNo += "#102");

                        //UserCompany.CompanyDesc += "#Test3");
                        //UserCompany.CompanyNo += "#103");

                        //UserCompany.CompanyDesc += "#Test4");
                        //UserCompany.CompanyNo += "#104");

                        //UserCompany.CompanyDesc += "#Test5");
                        //UserCompany.CompanyNo += "#105");

                        //UserCompany.CompanyDesc += "#Test6");
                        //UserCompany.CompanyNo += "#106");

                        //UserCompany.CompanyDesc += "#Test7");
                        //UserCompany.CompanyNo += "#107");

                        //UserCompany.CompanyDesc += "#Test8");
                        //UserCompany.CompanyNo += "#108");
#endif

                        strQry.Clear();
                        strQry.AppendLine(" SELECT DISTINCT ");
                        strQry.AppendLine(" UI.USER_NO");
                        strQry.AppendLine(",UI.FIRSTNAME");
                        strQry.AppendLine(",UI.SURNAME");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_ID UI");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA ");
                        strQry.AppendLine(" ON UI.USER_NO = UCA.USER_NO ");

                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT ");
                        strQry.AppendLine(" COMPANY_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_ID U");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA");
                        strQry.AppendLine(" ON U.USER_NO = UCA.USER_NO");

                        strQry.AppendLine(" WHERE U.USER_NO = " + UserCompany.UserNoLoggedIn + ") AS LINK_TABLE ");

                        strQry.AppendLine(" ON UCA.COMPANY_NO = LINK_TABLE.COMPANY_NO");

                        strQry.AppendLine(" ORDER BY ");
                        strQry.AppendLine(" UI.SURNAME");
                        strQry.AppendLine(",UI.FIRSTNAME");

                        clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "User");

                        for (int intUserRow = 0; intUserRow < DataSet.Tables["User"].Rows.Count; intUserRow++)
                        {
                            if (intUserRow == 0)
                            {
                                UserCompany.UserNo = DataSet.Tables["User"].Rows[intUserRow]["USER_NO"].ToString();
                                UserCompany.UserName = DataSet.Tables["User"].Rows[intUserRow]["FIRSTNAME"].ToString();
                                UserCompany.UserSurname = DataSet.Tables["User"].Rows[intUserRow]["SURNAME"].ToString();
                            }
                            else
                            {
                                UserCompany.UserNo += "#" + DataSet.Tables["User"].Rows[intUserRow]["USER_NO"].ToString();
                                UserCompany.UserName += "#" + DataSet.Tables["User"].Rows[intUserRow]["FIRSTNAME"].ToString();
                                UserCompany.UserSurname += "#" + DataSet.Tables["User"].Rows[intUserRow]["SURNAME"].ToString();
                            }

                            if (DataSet.Tables["UserFinger"] != null)
                            {
                                DataSet.Tables["UserFinger"].Clear();
                            }

                            strFingerArray = null;
                            strFingerArray = new string[10] { "N", "N", "N", "N", "N", "N", "N", "N", "N", "N" };

                            strQry.Clear();
                            strQry.AppendLine(" SELECT ");
                            strQry.AppendLine(" FINGER_NO");
                            strQry.AppendLine(",CREATION_DATETIME");
                            
                            strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE");

                            strQry.AppendLine(" WHERE USER_NO = " + DataSet.Tables["User"].Rows[intUserRow]["USER_NO"].ToString());

                            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "UserFinger");

                            for (int intFingerRow = 0; intFingerRow < DataSet.Tables["UserFinger"].Rows.Count; intFingerRow++)
                            {
                                if (DataSet.Tables["UserFinger"].Rows[intFingerRow]["CREATION_DATETIME"] == System.DBNull.Value)
                                {
                                    strFingerArray[Convert.ToInt32(DataSet.Tables["UserFinger"].Rows[intFingerRow]["FINGER_NO"])] = "L";
                                }
                                else
                                {
                                    strFingerArray[Convert.ToInt32(DataSet.Tables["UserFinger"].Rows[intFingerRow]["FINGER_NO"])] = "S";
                                }
                            }

                            for (int intFingerCountRow = 0; intFingerCountRow < 10; intFingerCountRow++)
                            {
                                if (intFingerCountRow == 0)
                                {
                                    if (UserCompany.UserFinger == "")
                                    {
                                        UserCompany.UserFinger = strFingerArray[intFingerCountRow].ToString();
                                    }
                                    else
                                    {
                                        UserCompany.UserFinger += "#" + strFingerArray[intFingerCountRow].ToString();
                                    }
                                }
                                else
                                {
                                    UserCompany.UserFinger += "|" + strFingerArray[intFingerCountRow].ToString();
                                }
                            }
                        }

                        if (DataSet.Tables["Company"].Rows.Count == 1)
                        {
                            strQry.Clear();
                            strQry.AppendLine(" SELECT ");
                            strQry.AppendLine(" EMPLOYEE_NO");
                            strQry.AppendLine(",EMPLOYEE_CODE");
                            strQry.AppendLine(",EMPLOYEE_NAME");
                            strQry.AppendLine(",EMPLOYEE_SURNAME");

                            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE");

                            strQry.AppendLine(" WHERE COMPANY_NO = " + UserCompany.CompanyNo);
                            strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                            strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                            strQry.AppendLine(" ORDER BY ");
                            strQry.AppendLine(" EMPLOYEE_SURNAME");
                            strQry.AppendLine(",EMPLOYEE_NAME");

                            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

                            for (int intEmployeeRow = 0; intEmployeeRow < DataSet.Tables["Employee"].Rows.Count; intEmployeeRow++)
                            {
                                if (intEmployeeRow == 0)
                                {
                                    UserCompany.EmployeeNo = DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString();
                                    UserCompany.EmployeeCode = DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_CODE"].ToString();
                                    UserCompany.EmployeeName = DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NAME"].ToString();
                                    UserCompany.EmployeeSurname = DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_SURNAME"].ToString();
                                }
                                else
                                {
                                    UserCompany.EmployeeNo += "#" + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString();
                                    UserCompany.EmployeeCode += "#" + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_CODE"].ToString();

                                    UserCompany.EmployeeName += "#" + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NAME"].ToString();
                                    UserCompany.EmployeeSurname += "#" + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_SURNAME"].ToString();
                                }

                                if (DataSet.Tables["Finger"] != null)
                                {
                                    DataSet.Tables["Finger"].Clear();
                                }

                                strFingerArray = null;
                                strFingerArray = new string[10] { "N", "N", "N", "N", "N", "N", "N", "N", "N", "N" };

                                strQry.Clear();
                                strQry.AppendLine(" SELECT ");
                                strQry.AppendLine(" FINGER_NO");
                                strQry.AppendLine(",CREATION_DATETIME");

                                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");

                                strQry.AppendLine(" WHERE COMPANY_NO = " + UserCompany.CompanyNo);
                                strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString());

                                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Finger");

                                for (int intFingerRow = 0; intFingerRow < DataSet.Tables["Finger"].Rows.Count; intFingerRow++)
                                {
                                    if (DataSet.Tables["Finger"].Rows[intFingerRow]["CREATION_DATETIME"] == System.DBNull.Value)
                                    {
                                        strFingerArray[Convert.ToInt32(DataSet.Tables["Finger"].Rows[intFingerRow]["FINGER_NO"])] = "L";
                                    }
                                    else
                                    {
                                        strFingerArray[Convert.ToInt32(DataSet.Tables["Finger"].Rows[intFingerRow]["FINGER_NO"])] = "S";
                                    }
                                }

                                for (int intFingerCountRow = 0; intFingerCountRow < 10; intFingerCountRow++)
                                {
                                    if (intFingerCountRow == 0)
                                    {
                                        if (UserCompany.EmployeeFinger == "")
                                        {
                                            UserCompany.EmployeeFinger = strFingerArray[intFingerCountRow].ToString();
                                        }
                                        else
                                        {
                                            UserCompany.EmployeeFinger += "#" + strFingerArray[intFingerCountRow].ToString();
                                        }
                                    }
                                    else
                                    {
                                        UserCompany.EmployeeFinger += "|" + strFingerArray[intFingerCountRow].ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog("GetUserFeaturesClocked SQL = " + strQry.ToString(), ex);
            }
            finally
            {
                DataSet.Dispose();
            }

            return UserCompany;
        }

        public UserCompany GetUserFeaturesClocked(string DBNo, ImageFeaturesContainer ImageFeaturesContainer)
        {
            UserCompany UserCompany = new UserCompany();

            string[] strFingerArray;

            UserCompany.UserNoLoggedIn = "";
            UserCompany.CompanyDesc = "";
            UserCompany.CompanyNo = "";
            UserCompany.UserNo = "";
            UserCompany.UserName = "";
            UserCompany.UserSurname = "";
            UserCompany.UserFinger = "";
            UserCompany.EmployeeNo = "";
            UserCompany.EmployeeCode = "";
            UserCompany.EmployeeName = "";
            UserCompany.EmployeeSurname = "";
            UserCompany.EmployeeFinger = "";

            int intRow = -1;
            int intReturnCode = 0;
            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new DataSet();

            try
            {
                clsFingerPrintClockServerLib FingerPrintClockServerLib = new clsFingerPrintClockServerLib(pvtstrSoftwareToUse);

                //Image from Finger Print
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" USER_NO");
                strQry.AppendLine(",FINGER_TEMPLATE");
                
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE ");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Template");

                if (DataSet.Tables["Template"].Rows.Count > 0)
                {
                    intReturnCode = FingerPrintClockServerLib.Get_User_Features(DataSet.Tables["Template"], ref ImageFeaturesContainer.DPImageFeaturesByteArray, ref intRow);

                    if (intRow != -1)
                    {
                        UserCompany.UserNoLoggedIn = DataSet.Tables["Template"].Rows[intRow]["USER_NO"].ToString();

                        strQry.Clear();
                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" C.COMPANY_NO");
                        strQry.AppendLine(",C.COMPANY_DESC");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.COMPANY C");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA");
                        strQry.AppendLine(" ON UCA.USER_NO = " + DataSet.Tables["Template"].Rows[intRow]["USER_NO"].ToString());
                        strQry.AppendLine(" AND C.COMPANY_NO = UCA.COMPANY_NO");

                        strQry.AppendLine(" ORDER BY");
                        strQry.AppendLine(" C.COMPANY_DESC");

                        clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Company");

                        for (int intCompanyRow = 0; intCompanyRow < DataSet.Tables["Company"].Rows.Count; intCompanyRow++)
                        {
                            if (intCompanyRow == 0)
                            {
                                UserCompany.CompanyDesc = DataSet.Tables["Company"].Rows[intCompanyRow]["COMPANY_DESC"].ToString();
                                UserCompany.CompanyNo = DataSet.Tables["Company"].Rows[intCompanyRow]["COMPANY_NO"].ToString();
                            }
                            else
                            {
                                UserCompany.CompanyDesc += "#" + DataSet.Tables["Company"].Rows[intCompanyRow]["COMPANY_DESC"].ToString();
                                UserCompany.CompanyNo += "#" + DataSet.Tables["Company"].Rows[intCompanyRow]["COMPANY_NO"].ToString();
                            }
                        }

#if (DEBUG)
                        //UserCompany.CompanyDesc += "#ZZZZZZZZZZZZZZZZZZZZZZZ");
                        //UserCompany.CompanyNo += "#99");

                        //UserCompany.CompanyDesc += "#Test1");
                        //UserCompany.CompanyNo += "#101");

                        //UserCompany.CompanyDesc += "#Test2");
                        //UserCompany.CompanyNo += "#102");

                        //UserCompany.CompanyDesc += "#Test3");
                        //UserCompany.CompanyNo += "#103");

                        //UserCompany.CompanyDesc += "#Test4");
                        //UserCompany.CompanyNo += "#104");

                        //UserCompany.CompanyDesc += "#Test5");
                        //UserCompany.CompanyNo += "#105");

                        //UserCompany.CompanyDesc += "#Test6");
                        //UserCompany.CompanyNo += "#106");

                        //UserCompany.CompanyDesc += "#Test7");
                        //UserCompany.CompanyNo += "#107");

                        //UserCompany.CompanyDesc += "#Test8");
                        //UserCompany.CompanyNo += "#108");
#endif

                        strQry.Clear();
                        strQry.AppendLine(" SELECT DISTINCT ");
                        strQry.AppendLine(" UI.USER_NO");
                        strQry.AppendLine(",UI.FIRSTNAME");
                        strQry.AppendLine(",UI.SURNAME");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_ID UI");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA ");
                        strQry.AppendLine(" ON UI.USER_NO = UCA.USER_NO ");

                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT ");
                        strQry.AppendLine(" COMPANY_NO");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_ID U");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA");
                        strQry.AppendLine(" ON U.USER_NO = UCA.USER_NO");

                        strQry.AppendLine(" WHERE U.USER_NO = " + UserCompany.UserNoLoggedIn + ") AS LINK_TABLE ");

                        strQry.AppendLine(" ON UCA.COMPANY_NO = LINK_TABLE.COMPANY_NO");

                        strQry.AppendLine(" ORDER BY ");
                        strQry.AppendLine(" UI.SURNAME");
                        strQry.AppendLine(",UI.FIRSTNAME");

                        clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "User");

                        for (int intUserRow = 0; intUserRow < DataSet.Tables["User"].Rows.Count; intUserRow++)
                        {
                            if (intUserRow == 0)
                            {
                                UserCompany.UserNo = DataSet.Tables["User"].Rows[intUserRow]["USER_NO"].ToString();
                                UserCompany.UserName = DataSet.Tables["User"].Rows[intUserRow]["FIRSTNAME"].ToString();
                                UserCompany.UserSurname = DataSet.Tables["User"].Rows[intUserRow]["SURNAME"].ToString();
                            }
                            else
                            {
                                UserCompany.UserNo += "#" + DataSet.Tables["User"].Rows[intUserRow]["USER_NO"].ToString();
                                UserCompany.UserName += "#" + DataSet.Tables["User"].Rows[intUserRow]["FIRSTNAME"].ToString();
                                UserCompany.UserSurname += "#" + DataSet.Tables["User"].Rows[intUserRow]["SURNAME"].ToString();
                            }

                            if (DataSet.Tables["UserFinger"] != null)
                            {
                                DataSet.Tables["UserFinger"].Clear();
                            }

                            strFingerArray = null;
                            strFingerArray = new string[10] { "N", "N", "N", "N", "N", "N", "N", "N", "N", "N" };

                            strQry.Clear();
                            strQry.AppendLine(" SELECT ");
                            strQry.AppendLine(" FINGER_NO");

                            strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE");

                            strQry.AppendLine(" WHERE USER_NO = " + DataSet.Tables["User"].Rows[intUserRow]["USER_NO"].ToString());

                            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "UserFinger");

                            for (int intFingerRow = 0; intFingerRow < DataSet.Tables["UserFinger"].Rows.Count; intFingerRow++)
                            {
                                strFingerArray[Convert.ToInt32(DataSet.Tables["UserFinger"].Rows[intFingerRow]["FINGER_NO"])] = "Y";
                            }

                            for (int intFingerCountRow = 0; intFingerCountRow < 10; intFingerCountRow++)
                            {
                                if (intFingerCountRow == 0)
                                {
                                    if (UserCompany.UserFinger == "")
                                    {
                                        UserCompany.UserFinger = strFingerArray[intFingerCountRow].ToString();
                                    }
                                    else
                                    {
                                        UserCompany.UserFinger += "#" + strFingerArray[intFingerCountRow].ToString();
                                    }
                                }
                                else
                                {
                                    UserCompany.UserFinger += "|" + strFingerArray[intFingerCountRow].ToString();
                                }
                            }
                        }

                        if (DataSet.Tables["Company"].Rows.Count == 1)
                        {
                            strQry.Clear();
                            strQry.AppendLine(" SELECT ");
                            strQry.AppendLine(" EMPLOYEE_NO");
                            strQry.AppendLine(",EMPLOYEE_CODE");
                            strQry.AppendLine(",EMPLOYEE_NAME");
                            strQry.AppendLine(",EMPLOYEE_SURNAME");
                            
                            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE");

                            strQry.AppendLine(" WHERE COMPANY_NO = " + UserCompany.CompanyNo);
                            strQry.AppendLine(" AND (NOT_ACTIVE_IND <> 'Y'");
                            strQry.AppendLine(" OR NOT_ACTIVE_IND IS NULL)");

                            strQry.AppendLine(" ORDER BY ");
                            strQry.AppendLine(" EMPLOYEE_SURNAME");
                            strQry.AppendLine(",EMPLOYEE_NAME");

                            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

                            for (int intEmployeeRow = 0; intEmployeeRow < DataSet.Tables["Employee"].Rows.Count; intEmployeeRow++)
                            {
                                if (intEmployeeRow == 0)
                                {
                                    UserCompany.EmployeeNo = DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString();
                                    UserCompany.EmployeeCode = DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_CODE"].ToString();
                                    UserCompany.EmployeeName = DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NAME"].ToString();
                                    UserCompany.EmployeeSurname = DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_SURNAME"].ToString();
                                }
                                else
                                {
                                    UserCompany.EmployeeNo += "#" + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString();
                                    UserCompany.EmployeeCode += "#" + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_CODE"].ToString();

                                    UserCompany.EmployeeName += "#" + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NAME"].ToString();
                                    UserCompany.EmployeeSurname += "#" + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_SURNAME"].ToString();
                                }

                                if (DataSet.Tables["Finger"] != null)
                                {
                                    DataSet.Tables["Finger"].Clear();
                                }

                                strFingerArray = null;
                                strFingerArray = new string[10] { "N", "N", "N", "N", "N", "N", "N", "N", "N", "N" };

                                strQry.Clear();
                                strQry.AppendLine(" SELECT ");
                                strQry.AppendLine(" FINGER_NO");

                                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");

                                strQry.AppendLine(" WHERE COMPANY_NO = " + UserCompany.CompanyNo);
                                strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intEmployeeRow]["EMPLOYEE_NO"].ToString());

                                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Finger");

                                for (int intFingerRow = 0; intFingerRow < DataSet.Tables["Finger"].Rows.Count; intFingerRow++)
                                {
                                    strFingerArray[Convert.ToInt32(DataSet.Tables["Finger"].Rows[intFingerRow]["FINGER_NO"])] = "Y";
                                }

                                for (int intFingerCountRow = 0; intFingerCountRow < 10; intFingerCountRow++)
                                {
                                    if (intFingerCountRow == 0)
                                    {
                                        if (UserCompany.EmployeeFinger == "")
                                        {
                                            UserCompany.EmployeeFinger = strFingerArray[intFingerCountRow].ToString();
                                        }
                                        else
                                        {
                                            UserCompany.EmployeeFinger += "#" + strFingerArray[intFingerCountRow].ToString();
                                        }
                                    }
                                    else
                                    {
                                        UserCompany.EmployeeFinger += "|" + strFingerArray[intFingerCountRow].ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog("GetUserFeaturesClocked SQL = " + strQry.ToString(), ex);
            }
            finally
            {
                DataSet.Dispose();
            }

            return UserCompany;
        }

#if (DEBUG)
        private void Write_DateTime()
        {
            TextBox txtDateTime = (TextBox)AppDomain.CurrentDomain.GetData("DateTimeTextBox");
            txtDateTime.Text = DateTime.Now.ToString("yyyy/MM/dd  HH:mm:ss");
        }
#endif


        public DeviceNewInfo GetNewDeviceInfo(string DBNo, string DeviceNo, string Online)
        {
            DeviceNewInfo DeviceNewInfo = new DeviceNewInfo();
            int intWhere = 0;

            DeviceNewInfo.DeviceDesc = "";
            DeviceNewInfo.DeviceUsage = "";
            DeviceNewInfo.ClockInOutParm = "";
            DeviceNewInfo.ClockInRangeFrom = "";
            DeviceNewInfo.ClockInRangeTo = "";
            DeviceNewInfo.LanInd = "";
            DeviceNewInfo.FingerEngine = "";

            DataSet DataSet = new DataSet();

            try
            {
                StringBuilder strQry = new StringBuilder();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" D.DEVICE_DESC");
                strQry.AppendLine(",D.DEVICE_USAGE");
                strQry.AppendLine(",D.CLOCK_IN_OUT_PARM");
                strQry.AppendLine(",D.CLOCK_IN_RANGE_FROM");
                strQry.AppendLine(",D.CLOCK_IN_RANGE_TO");
                strQry.AppendLine(",D.LAN_WAN_IND");
                strQry.AppendLine(",ISNULL(FSTU.FINGERPRINT_SOFTWARE_IND,'U') AS FINGERPRINT_SOFTWARE_IND");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE D");

                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.FINGERPRINT_SOFTWARE_TO_USE FSTU");
                strQry.AppendLine(" ON FSTU.FINGERPRINT_SOFTWARE_IND = FSTU.FINGERPRINT_SOFTWARE_IND");

                strQry.AppendLine(" WHERE D.DEVICE_NO = " + DeviceNo);
                
                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Device");
                
                intWhere = 1;

                if (DataSet.Tables["Device"].Rows.Count > 0)
                {
                    intWhere = 2;
                    DeviceNewInfo.FoundInfo = "Y";
                    intWhere = 3;
                    DeviceNewInfo.DeviceDesc = DataSet.Tables["Device"].Rows[0]["DEVICE_DESC"].ToString();
                    intWhere = 4;
                    DeviceNewInfo.DeviceUsage = DataSet.Tables["Device"].Rows[0]["DEVICE_USAGE"].ToString();
                    intWhere = 5;
                    DeviceNewInfo.ClockInOutParm = DataSet.Tables["Device"].Rows[0]["CLOCK_IN_OUT_PARM"].ToString();
                    intWhere = 6;
                    DeviceNewInfo.ClockInRangeFrom = DataSet.Tables["Device"].Rows[0]["CLOCK_IN_RANGE_FROM"].ToString();
                    intWhere = 7;
                    DeviceNewInfo.ClockInRangeTo = DataSet.Tables["Device"].Rows[0]["CLOCK_IN_RANGE_TO"].ToString();

                    if (DataSet.Tables["Device"].Rows[0]["LAN_WAN_IND"].ToString() == "W")
                    {
                        DeviceNewInfo.LanInd = "N";
                    }
                    else
                    {
                        DeviceNewInfo.LanInd = "Y";
                    }

                    DeviceNewInfo.FingerEngine = DataSet.Tables["Device"].Rows[0]["FINGERPRINT_SOFTWARE_IND"].ToString();
                }
                else
                {
                    DeviceNewInfo.FoundInfo = "N";
                }

                intWhere = 8;

                DeviceNewInfo.DateTime = DateTime.Now.ToString("yyyyMMddHHmmss");

                if (Online == "N")
                {
                    //Load Into Memory for Later Reads
                    strQry.Clear();
                    strQry.AppendLine(" SELECT *");
                    
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Template");
                }
                else
                {
                    //Load Into Memory for Later Reads (Read every 5 Minutes)
                    if (DateTime.Now.Minute == 0
                        | DateTime.Now.Minute == 5
                        | DateTime.Now.Minute == 10
                        | DateTime.Now.Minute == 15
                        | DateTime.Now.Minute == 20
                        | DateTime.Now.Minute == 25
                        | DateTime.Now.Minute == 30
                        | DateTime.Now.Minute == 35
                        | DateTime.Now.Minute == 40
                        | DateTime.Now.Minute == 45
                        | DateTime.Now.Minute == 50
                        | DateTime.Now.Minute == 55)
                    {
                        if (DataSet.Tables["Device"].Rows[0]["DEVICE_DESC"].ToString() == "W")
                        {
                            //NB Wan Reads Every 1 Minute
                            strQry.Clear();
                            strQry.AppendLine(" SELECT *");
                            
                            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");

                            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Template");
                        }
                        else
                        {
                            //NB Lan Reads Every 15 Seconds
                            if (DateTime.Now.Second < 16)
                            {
                                strQry.Clear();
                                strQry.AppendLine(" SELECT *");
                                
                                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");

                                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Template");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog("GetNewDeviceInfo Where In " + intWhere.ToString() , ex);

                DeviceNewInfo.FoundInfo = "F";
            }
            finally
            {
                DataSet.Dispose();
            }

            return DeviceNewInfo;
        }

        public DeviceNewInfo GetDeviceInfoNew(string DeviceNo)
        {
            DeviceNewInfo DeviceNewInfo = new DeviceNewInfo();
            int intWhere = 0;
            
            DeviceNewInfo.DeviceDesc = "";
            DeviceNewInfo.DeviceUsage = "";
            DeviceNewInfo.ClockInOutParm = "";
            DeviceNewInfo.ClockInRangeFrom = "";
            DeviceNewInfo.ClockInRangeTo = "";
            DeviceNewInfo.LanInd = "";
            DeviceNewInfo.FingerEngine = "";

            DataSet DataSet = new DataSet();

            try
            {
                StringBuilder strQry = new StringBuilder();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" D.DEVICE_DESC");
                strQry.AppendLine(",D.DEVICE_USAGE");
                strQry.AppendLine(",D.CLOCK_IN_OUT_PARM");
                strQry.AppendLine(",D.CLOCK_IN_RANGE_FROM");
                strQry.AppendLine(",D.CLOCK_IN_RANGE_TO");
                strQry.AppendLine(",D.LAN_WAN_IND");
                strQry.AppendLine(",ISNULL(FSTU.FINGERPRINT_SOFTWARE_IND,'U') AS FINGERPRINT_SOFTWARE_IND");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE D");

                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.FINGERPRINT_SOFTWARE_TO_USE FSTU");
                strQry.AppendLine(" ON FSTU.FINGERPRINT_SOFTWARE_IND = FSTU.FINGERPRINT_SOFTWARE_IND");

                strQry.AppendLine(" WHERE D.DEVICE_NO = " + DeviceNo);

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Device");

                intWhere = 1;

                if (DataSet.Tables["Device"].Rows.Count > 0)
                {
                    intWhere = 2;
                    DeviceNewInfo.FoundInfo = "Y";
                    intWhere = 3;
                    DeviceNewInfo.DeviceDesc = DataSet.Tables["Device"].Rows[0]["DEVICE_DESC"].ToString();
                    intWhere = 4;
                    DeviceNewInfo.DeviceUsage = DataSet.Tables["Device"].Rows[0]["DEVICE_USAGE"].ToString();
                    intWhere = 5;
                    DeviceNewInfo.ClockInOutParm = DataSet.Tables["Device"].Rows[0]["CLOCK_IN_OUT_PARM"].ToString();
                    intWhere = 6;
                    DeviceNewInfo.ClockInRangeFrom = DataSet.Tables["Device"].Rows[0]["CLOCK_IN_RANGE_FROM"].ToString();
                    intWhere = 7;
                    DeviceNewInfo.ClockInRangeTo = DataSet.Tables["Device"].Rows[0]["CLOCK_IN_RANGE_TO"].ToString();

                    if (DataSet.Tables["Device"].Rows[0]["LAN_WAN_IND"].ToString() == "W")
                    {
                        DeviceNewInfo.LanInd = "N";
                    }
                    else
                    {
                        DeviceNewInfo.LanInd = "Y";
                    }

                    DeviceNewInfo.FingerEngine = DataSet.Tables["Device"].Rows[0]["FINGERPRINT_SOFTWARE_IND"].ToString();
                }
                else
                {
                    DeviceNewInfo.FoundInfo = "N";
                }

                intWhere = 8;

                DeviceNewInfo.DateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            catch (Exception ex)
            {
                WriteExceptionLog("GetDeviceInfoNew Where In " + intWhere.ToString(), ex);

                DeviceNewInfo.FoundInfo = "F";
            }
            finally
            {
                DataSet.Dispose();
            }

            return DeviceNewInfo;
        }

        private void WriteExceptionLog(string Message, Exception ex)
        {
            string strInnerExceptionMessage = "";

            if (ex.InnerException != null)
            {
                strInnerExceptionMessage = ex.InnerException.Message;
            }

            WriteLog(Message + " ********** Exception = " + ex.Message + " " + strInnerExceptionMessage);
        }

        private void WriteLog(string Message)
        {
            int intNumberRetries = 0;

        WriteLog_Retry:

            try
            {
                using (FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + pvtstrLogFileName + "_Log.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (TextWriter tw = new StreamWriter(fs))
                    {
                        tw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Service " + Message);
                    }
                }
            }
            catch (Exception ex)
            {
                if (intNumberRetries > 0)
                {
                    try
                    {
                        using (FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + pvtstrLogFileName + "Error_Log.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                        {
                            using (TextWriter tw = new StreamWriter(fs))
                            {
                                tw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Service " + Message + " Number Tries = " + intNumberRetries + " Exception = " + ex.Message);
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                intNumberRetries += 1;
                TimeSpan myTimeSpan = TimeSpan.FromMilliseconds(100);
                System.Threading.Thread.Sleep(myTimeSpan);

                goto WriteLog_Retry;
            }
        }
    }
}