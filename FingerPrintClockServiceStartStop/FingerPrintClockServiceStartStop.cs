using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Web;
using System.Reflection;
using System.ServiceProcess;
using System.Management;
using System.ServiceModel.Web;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace FingerPrintClockServer
{
    public class FingerPrintClockServiceStartStop : IFingerPrintClockServiceStartStop
    {
        WebServiceHost FingerPrintClockServiceStartStopHost;
        TextMessageEncodingBindingElement TextMessageEncoder;
        WebHttpBinding WebHttpBinding;

        System.Timers.Timer tmrDynamicUploadTimer;
        System.Timers.Timer tmrFingerPrintClockTimeAttendanceServiceReplaceTimer;

        bool pvtblnDynamicUpload = false;

        bool pvtblnIncorrectTimeSheetDynamicUploadISexe = false;
        bool pvtblnUsingNewHostingExe = false;

        bool pvtblnFirstPass = true;

        bool pvtblnFingerPrintClockTimeAttendanceServiceReplace = false;

        DateTime dtNextCheckDateTime = DateTime.Now.AddMinutes(30);

        System.Timers.Timer tmrDatabaseBackupTimer;

        string pvtstrLogFileName = "FingerPrintClockTimeAttendanceServiceStartStop";

        string pvtstrPortNo = "8000";
        string pvtstrIpAddress = "";
        string pvtstrTimeSheetDynamicUploadExePath = "";
        string pvtstrUploadKeyPath = "";
        int pvtintDynamicUploadTimerinMinutes = 5;
        string pvtstrWhere = "";

        private SqlConnection pvtSqlConnectionClient;
        private SqlCommand pvtSqlCommandClient;
        private SqlDataAdapter pvtSqlDataAdapterClient;

        private string pvtstrConnectionClient = @"Server=#Engine#;Database=InteractPayrollClient;Integrated Security=true; MultipleActiveResultSets=True;";
        private string pvtstrDBEngine = "";

        public FingerPrintClockServiceStartStop()
        {
            try
            {
                WriteLog("FingerPrintClockServiceStartStop Entered *****");

                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockTimeAttendanceServiceStartStop.exe.config") == true)
                {
                    pvtstrWhere = "Get TimeSheetDynamicUploadExePath from Config File";
                    pvtstrTimeSheetDynamicUploadExePath = ConfigurationManager.AppSettings["TimeSheetDynamicUploadExePath"];

                    if (pvtstrTimeSheetDynamicUploadExePath != "")
                    {
                        if (Directory.Exists(Path.GetDirectoryName(pvtstrTimeSheetDynamicUploadExePath)) == true)
                        {
                            if (pvtstrTimeSheetDynamicUploadExePath.IndexOf("TimeSheetDynamicUpload.exe") > -1)
                            {
                                WriteLog("Config File must use 'TimeSheetDynamicUploadIS.exe', NOT 'TimeSheetDynamicUpload.exe'. Speak to Administrator. **********");
                                pvtblnIncorrectTimeSheetDynamicUploadISexe = true;
                                return;
                            }
                            else
                            {
                                if (pvtstrTimeSheetDynamicUploadExePath.IndexOf("TimeSheetDynamicUploadIS.exe") == -1)
                                {
                                    WriteLog("Config File must use 'TimeSheetDynamicUploadIS.exe'. Speak to Administrator. **********");
                                    pvtblnIncorrectTimeSheetDynamicUploadISexe = true;
                                    return;
                                }
                            }

                            pvtstrWhere = "Get DynamicUploadTimerinMinutes from Config File";
                            pvtintDynamicUploadTimerinMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["DynamicUploadTimerinMinutes"]);

                            pvtstrUploadKeyPath = Path.GetDirectoryName(pvtstrTimeSheetDynamicUploadExePath) + "\\UploadKey.txt";

                            if (File.Exists(pvtstrUploadKeyPath) == true)
                            {
                                WriteLog("TimeSheetDynamicUploadIS.exe Set to Run Every " + pvtintDynamicUploadTimerinMinutes.ToString() + " Minutes Time.");
                                pvtblnDynamicUpload = true;
                            }
                            else
                            {
                                WriteLog("UploadKey.txt NOT Found - No Dynamic Upload.");
                            }
                        }
                        else
                        {
                            string strDirectory = Path.GetDirectoryName(pvtstrTimeSheetDynamicUploadExePath);

                            WriteLog("Directory '" + strDirectory + "' Specified in Config File 'FingerPrintClockTimeAttendanceServiceStartStop.exe.config' does NOT Exist **********");
                        }
                    }
                    else
                    {
                        WriteLog("ConfigurationManager.AppSettings['TimeSheetDynamicUploadExePath'] = '' ****** NO DYNAMIC UPLOAD OF TIMESHEETS ");
                    }
                }
                else
                {
                    WriteLog("FingerPrintClockTimeAttendanceServiceStartStop.exe.config does Not Exist.");
                }
                
                pvtstrWhere = "Check DBConfig.txt File";

                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DBConfig.txt") == true)
                {
                    using (StreamReader srStreamReader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "DBConfig.txt"))
                    {
                        try
                        {
                            string[] strComponents = srStreamReader.ReadLine().Split(';');

                            if (strComponents.Length > 0)
                            {
                                pvtstrDBEngine = strComponents[0];
                            }

                            pvtstrConnectionClient = pvtstrConnectionClient.Replace("#Engine#", pvtstrDBEngine.Trim());

                            if (strComponents.Length > 1)
                            {
                                if (strComponents[1].IndexOf("AttachDBFilename") > -1)
                                {
                                    //New AttachDBFilename
                                    pvtstrConnectionClient = pvtstrConnectionClient + strComponents[1] + ";";
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteExceptionLog("DBConfig.txt Read", ex);
                            return;
                        }
                    }
                }
                else
                {
                    WriteLog("DBConfig.txt does Not Exist. **********");
                    return;
                }

                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "PortConfig.txt") == true)
                {
                    using (StreamReader srStreamReader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "PortConfig.txt"))
                    {
                        try
                        {
                            pvtstrPortNo = srStreamReader.ReadLine();
                        }
                        catch(Exception ex)
                        {
                            WriteExceptionLog("FingerPrintClockServiceStartStop PortConfig.txt Read ", ex);
                        }
                    }
                }
#if (DEBUG)
                pvtstrConnectionClient = pvtstrConnectionClient.Replace("InteractPayrollClient", "InteractPayrollClient_Debug");

                //Run_DynamicUpload();
                OnStart();
#endif
            }
            catch (Exception ex)
            {
                WriteExceptionLog("FingerPrintClockServiceStartStop Where " + pvtstrWhere + " ", ex);
            }
        }
        
        public void OnStart()
        {
            try
            {
                WriteLog("OnStart Connected to SQL Engine '" + pvtstrDBEngine + "' on Port " + pvtstrPortNo);
                
                pvtstrIpAddress = @"http://localhost:" + pvtstrPortNo + "/FingerPrintClockServerStartStop";
                
                WriteLog("URI = " + pvtstrIpAddress);

                FingerPrintClockServiceStartStopHost = new WebServiceHost(this.GetType(), new Uri(pvtstrIpAddress));

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

                FingerPrintClockServiceStartStopHost.CloseTimeout = TimeSpan.FromSeconds(60);
                FingerPrintClockServiceStartStopHost.OpenTimeout = TimeSpan.FromSeconds(60);

                FingerPrintClockServiceStartStopHost.AddServiceEndpoint(this.GetType().GetInterface("FingerPrintClockServer.IFingerPrintClockServiceStartStop"), WebHttpBinding, pvtstrIpAddress);

                int intNumberRetries = 0;

            OnStart_Retry:

                intNumberRetries += 1;
                bool blnAllowTimers = true;

                try
                {
                    FingerPrintClockServiceStartStopHost.Open();
                    WriteLog("### FingerPrintClockServiceStartStop OPEN Successful ###");
                }
                catch (Exception ex)
                {
                    if (intNumberRetries < 4)
                    {
                        WriteLog("### FingerPrintClockServiceStartStop OPEN FAILED - " + intNumberRetries + " ###");
                        TimeSpan myTimeSpan = TimeSpan.FromMilliseconds(2000);
                        System.Threading.Thread.Sleep(myTimeSpan);

                        goto OnStart_Retry;
                    }
                    else
                    {
                        blnAllowTimers = false;
                        WriteExceptionLog("@@@@@@@@@@@@@@@@@@@@ OnStart FingerPrintClockServiceStartStop OPEN FAILED - Tried 3 Times @@@@@@@@@@@@@@@@@@@@", ex);
                    }
                }

                if (blnAllowTimers == true)
                {
                    if (pvtblnDynamicUpload == true)
                    {
                        tmrDynamicUploadTimer = new System.Timers.Timer();
                        tmrDynamicUploadTimer.Elapsed += DynamicUploadTimerEvent;
                        //In 20 Seconds - Wait for Windows Services to Start (Cold Start)
                        tmrDynamicUploadTimer.Interval = 20000;
                        tmrDynamicUploadTimer.Start();
                        WriteLog("FingerPrintClockServiceStartStop tmrDynamicUploadTimer Started");
                    }

                    tmrDatabaseBackupTimer = new System.Timers.Timer();
                    tmrDatabaseBackupTimer.Elapsed += BackupDatabase;
                    //In 30 Minutes
                    tmrDatabaseBackupTimer.Interval = 1800000;
                    tmrDatabaseBackupTimer.Start();
                    WriteLog("FingerPrintClockServiceStartStop tmrDatabaseBackupTimer Started");

                    tmrFingerPrintClockTimeAttendanceServiceReplaceTimer = new System.Timers.Timer();
                    tmrFingerPrintClockTimeAttendanceServiceReplaceTimer.Elapsed += Stop_FingerPrintClockTimeAttendanceService_And_RenameExe;
                    //In 5 Minutes
                    tmrFingerPrintClockTimeAttendanceServiceReplaceTimer.Interval = 300000;
                    tmrFingerPrintClockTimeAttendanceServiceReplaceTimer.Start();
                    WriteLog("pvtblnFingerPrintClockTimeAttendanceServiceReplace Timer Initialise Successful");
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog("@@@@@@@@@@@@@@@@@@@@ OnStart FingerPrintClockServiceStartStop Open FAILED", ex);
            }
        }

        public void OnStop()
        {
            try
            {
                WriteLog("OnStop Entered ####################");

                if (tmrDynamicUploadTimer != null)
                {
                    tmrDynamicUploadTimer.Enabled = false;
                    tmrDynamicUploadTimer.Dispose();
                    WriteLog("tmrDynamicUploadTimer Disposed");
                }

                if (tmrDatabaseBackupTimer != null)
                {
                    tmrDatabaseBackupTimer.Enabled = false;
                    tmrDatabaseBackupTimer.Dispose();
                    WriteLog("tmrDatabaseBackupTimer Disposed");
                }

                if (FingerPrintClockServiceStartStopHost != null)
                {
                    try
                    {
                        FingerPrintClockServiceStartStopHost.Close();
                        WriteLog("OnStop FingerPrintClockServiceStartStopHost Closed");
                    }
                    catch
                    {
                        FingerPrintClockServiceStartStopHost.Abort();
                        WriteLog("OnStop FingerPrintClockServiceStartStopHost Aborted");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog("OnStop", ex);
            }
        }

        private void Stop_FingerPrintClockTimeAttendanceService_And_RenameExe(Object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                tmrFingerPrintClockTimeAttendanceServiceReplaceTimer.Stop();

                pvtblnFingerPrintClockTimeAttendanceServiceReplace = false;
                
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockTimeAttendanceService.exe_") == true)
                {
                    WriteLog("FingerPrintClockTimeAttendanceService.exe_ EXISTS **********");
                    pvtblnFingerPrintClockTimeAttendanceServiceReplace = true;
                }

                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockService.dll_") == true)
                {
                    WriteLog("FingerPrintClockService.dll_ EXISTS **********");
                    pvtblnFingerPrintClockTimeAttendanceServiceReplace = true;
                }

                if (pvtblnFingerPrintClockTimeAttendanceServiceReplace == true)
                {
                    WriteLog("Stop_FingerPrintClockTimeAttendanceService_And_RenameExe Entered");

                    bool blnServiceWasRunning = false;

                    ServiceController service = new ServiceController("FingerPrintClockTimeAttendanceService");

                    if (service != null)
                    {
                        if (service.Status.ToString() == "Running")
                        {
                            WriteLog("**********FingerPrintClockTimeAttendanceService is Running ");
                            blnServiceWasRunning = true;
                        }
                        else
                        {
                            WriteLog("********** NB NB FingerPrintClockTimeAttendanceService WAS NOT RUNNING **********");
                        }
                    }
                    else
                    {
                        WriteLog("**********FingerPrintClockTimeAttendanceService NOT INSTALLED ???????????????");
                        return;
                    }

                    if (blnServiceWasRunning == true)
                    {
                        WriteLog("********** Stopping FingerPrintClockTimeAttendanceService Service **********");

                        service.Stop();

                        while (service.Status.ToString() != "Stopped")
                        {
                            service.Refresh();
                        }

                        WriteLog("********** Stopped FingerPrintClockTimeAttendanceService Service SUCCESSFULLY **********");
                    }

                    //Wait 10 Seconds
                    System.Threading.Thread.Sleep(10000);

                    //Take backup
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockTimeAttendanceService.exe_") == true)
                    {
                        File.Delete(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockTimeAttendanceService.exe");
                        File.Move(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockTimeAttendanceService.exe_", AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockTimeAttendanceService.exe");

                        WriteLog("********** FingerPrintClockTimeAttendanceService.exe REPLACED SUCCESSFULLY **********");
                    }

                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockService.dll_") == true)
                    {
                        File.Delete(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockService.dll");
                        File.Move(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockService.dll_", AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockService.dll");

                        WriteLog("********** FingerPrintClockService.dll REPLACED SUCCESSFULLY **********");
                    }

                    if (blnServiceWasRunning == true)
                    {
                        WriteLog("********** Starting FingerPrintClockTimeAttendanceService Service **********");

                        service.Start();

                        while (service.Status.ToString() != "Running")
                        {
                            service.Refresh();
                        }

                        WriteLog("********** Started FingerPrintClockTimeAttendanceService Service SUCCESSFULLY **********");
                    }

                    WriteLog("Stop_FingerPrintClockTimeAttendanceService_And_RenameExe Exit");
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog("Stop_FingerPrintClockTimeAttendanceService_And_RenameExe ", ex);
            }
            finally
            {
                tmrFingerPrintClockTimeAttendanceServiceReplaceTimer.Start();
            }
        }

        private void Check_FingerPrintClockTimeAttendanceService()
        {
            try
            {
                WriteLog("Check_FingerPrintClockTimeAttendanceService Entered");

                ServiceController service = new ServiceController("FingerPrintClockTimeAttendanceService");

                if (service != null)
                {
                    if (service.Status.ToString() == "Running")
                    {
                        WriteLog("**********FingerPrintClockTimeAttendanceService is Running ");
                    }
                    else
                    {
                        //Try Start Service
                        WriteLog("NB NB FingerPrintClockTimeAttendanceService WAS NOT RUNNING ####################");
                        WriteLog("Starting FingerPrintClockTimeAttendanceService ####################");

                        service.Start();

                        while (service.Status.ToString() != "Running")
                        {
                            service.Refresh();
                        }

                        //Wait Another 5 Seconds to Make sure Service is Up
                        DateTime myNewDateTime = DateTime.Now.AddSeconds(5);

                        while (DateTime.Now < myNewDateTime)
                        {
                        }

                        WriteLog("Started FingerPrintClockTimeAttendanceService SUCCESSFULLY ####################");
                    }
                }
                else
                {
                    WriteLog("**********FingerPrintClockTimeAttendanceService NOT INSTALLED ???????????????");
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog("Check_FingerPrintClockTimeAttendanceService", ex);
            }
            finally
            {
                try
                {
                    WriteLog("Check_FingerPrintClockTimeAttendanceService Exit");
                }
                catch
                {
                }
            }
        }

        static double Calculate_Next_Time_Timer_Fires_In_Minutes(int intAddMinutes)
        {
            double tickTime = 0;

            DateTime FromDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 54, 00);
            DateTime ToDateTime = new DateTime(DateTime.Now.AddDays(1).Year, DateTime.Now.AddDays(1).Month, DateTime.Now.AddDays(1).Day, 0, 0, 0);

            //Between 6 Minutes to Midnight to Midnight
            if (DateTime.Now > FromDateTime
            && DateTime.Now < ToDateTime)
            {
                //Set Between 12:00:05 AM and 05:00:00 AM (Next day)
                int intMinutes = DateTime.Now.Second * 5;

                DateTime dtNextTime = new DateTime(DateTime.Now.AddDays(1).Year, DateTime.Now.AddDays(1).Month, DateTime.Now.AddDays(1).Day, 0, 0, 0).AddMinutes(intMinutes);
                tickTime = (double)(dtNextTime - DateTime.Now).TotalMilliseconds;
            }
            else
            {
                DateTime dtNextTime = DateTime.Now.AddMinutes(intAddMinutes);
                tickTime = (double)(dtNextTime - DateTime.Now).TotalMilliseconds;
            }

            return tickTime;
        }

        private void DynamicUploadTimerEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                //Stop Timer
                tmrDynamicUploadTimer.Stop();

                Run_DynamicUpload();
            }
            catch (Exception ex)
            {
                WriteExceptionLog("DynamicUploadTimerEvent", ex);
            }
            finally
            {
                //Set Next Time for Timer to Fire
                double tickTime = Calculate_Next_Time_Timer_Fires_In_Minutes(pvtintDynamicUploadTimerinMinutes);
                tmrDynamicUploadTimer.Interval = tickTime;
                tmrDynamicUploadTimer.Start();
            }
        }

        private void Run_DynamicUpload()
        {
            if (pvtblnFirstPass == true)
            {
                WriteLog("DynamicUploadTimerEvent pvtblnFirstPass  = true");

                DataSet DataSet = new System.Data.DataSet();
                StringBuilder strQry = new StringBuilder();

                try
                {
                    //2018-08-09
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" TABLE_NAME ");

                    strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.TABLES ");

                    strQry.AppendLine(" WHERE TABLE_NAME = 'VALIDITE_HOSTING_SERVICE' ");

                    Create_DataTable_Client(strQry.ToString(), DataSet, "ValiditeHostingTableExists");

                    if (DataSet.Tables["ValiditeHostingTableExists"].Rows.Count > 0)
                    {
                        WriteLog("DynamicUploadTimerEvent VALIDITE_HOSTING_SERVICE Table Exists");

                        strQry.Clear();
                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" DYNAMIC_UPLOAD_TIMESHEETS_RUNNING_IND");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.VALIDITE_HOSTING_SERVICE");

                        Create_DataTable_Client(strQry.ToString(), DataSet, "ValiditeHostingService");

                        if (DataSet.Tables["ValiditeHostingService"].Rows.Count > 0)
                        {
                            pvtblnUsingNewHostingExe = true;

                            WriteLog("DynamicUploadTimerEvent **Using New Hosting Exe******************");
                        }
                    }

                    if (pvtblnUsingNewHostingExe == false)
                    {
                        strQry.Clear();
                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" FILE_DOWNLOAD_CHECK_DATE");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DYNAMIC_FILE_DOWNLOAD_CHECK");

                        Create_DataTable_Client(strQry.ToString(), DataSet, "DateCheck");

                        //Causes Download of File Check in TimeSheetDynamicUploadIS.exe By Setting FILE_DOWNLOAD_CHECK_DATE to Yesterdays Date
                        if (DataSet.Tables["DateCheck"].Rows.Count == 0)
                        {
                            strQry.Clear();
                            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.DYNAMIC_FILE_DOWNLOAD_CHECK ");
                            strQry.AppendLine("(FILE_DOWNLOAD_CHECK_DATE)");
                            strQry.AppendLine(" VALUES ");
                            strQry.AppendLine("('" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + "')");

                            Execute_SQLCommand(strQry.ToString());
                        }
                        else
                        {
                            strQry.Clear();
                            strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.DYNAMIC_FILE_DOWNLOAD_CHECK ");
                            strQry.AppendLine(" SET FILE_DOWNLOAD_CHECK_DATE = '" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + "'");

                            Execute_SQLCommand(strQry.ToString());
                        }

                        WriteLog("FILE_DOWNLOAD_CHECK_DATE = '" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + "'");
                    }

                    pvtblnFirstPass = false;

                }
                catch (Exception ex)
                {
                    WriteExceptionLog("DynamicUploadTimerEvent DateCheck", ex);

                    goto DynamicUploadTimerEvent_Continue;
                }
            }

            if (DateTime.Now.Minute < 6
            && DateTime.Now > dtNextCheckDateTime)
            {
                //Archive Logs
                Check_Log_For_Archiving();

                //Restart Service If Stopped
                Check_FingerPrintClockTimeAttendanceService();
            }

            if (pvtblnIncorrectTimeSheetDynamicUploadISexe == false)
            {
                //Rename Any *.exe_ to *.exe
                string strDirectory = Path.GetDirectoryName(pvtstrTimeSheetDynamicUploadExePath);

                string[] strFiles = Directory.GetFiles(strDirectory, "*.*_");

                foreach (string file in strFiles)
                {
                    try
                    {
                        File.Copy(file, file.Replace("_", ""), true);
                        File.Delete(file);

                        WriteLog("DynamicUploadTimerEvent File Copy Successful " + file);
                    }
                    catch (Exception ex)
                    {
                        WriteExceptionLog("DynamicUploadTimerEvent File Copy Failed " + file, ex);
                    }
                }


                if (pvtblnUsingNewHostingExe == false)
                {
                    ProcessStartInfo processStartInfo = new ProcessStartInfo();
                    processStartInfo.UseShellExecute = false;
                    processStartInfo.FileName = pvtstrTimeSheetDynamicUploadExePath;
                    processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    processStartInfo.Arguments = "1";

                    Process.Start(processStartInfo);

                    WriteLog("DynamicUploadTimerEvent Exit File " + pvtstrTimeSheetDynamicUploadExePath + " Executed");
                }
            }

        DynamicUploadTimerEvent_Continue:

            int intContinue = 0;
        }

        private void BackupDatabase(Object source, System.Timers.ElapsedEventArgs e)
        {
            bool blnBackupDatabase = false;

            try
            {
                WriteLog("BackupDatabase Started ...");

                //Stop Timer
                tmrDatabaseBackupTimer.Stop();

                StringBuilder strQry = new StringBuilder();

                string strFileDirectory = AppDomain.CurrentDomain.BaseDirectory + "Backup";

                if (Directory.Exists(strFileDirectory))
                {
                }
                else
                {
                    Directory.CreateDirectory(strFileDirectory);
                }

                string strDataBaseName = "InteractPayrollClient";

                //Cleanup Daily Backup Files Older than 10 Days
                string[] strDbBackupFiles = Directory.GetFiles(strFileDirectory, strDataBaseName + "_*_Daily.bak");

                foreach (string file in strDbBackupFiles)
                {
                    int intOffset = file.IndexOf(strDataBaseName + "_");

                    if (intOffset > -1)
                    {
                        if (file.Length > intOffset + 40)
                        {
                            try
                            {
                                DateTime myFileDateTime = DateTime.ParseExact(file.Substring(intOffset + 22, 8), "yyyyMMdd", null);

                                if (myFileDateTime.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd"))
                                {
                                    blnBackupDatabase = true;

                                    WriteLog("Database has Already been Backed Up for the day");
                                }

                                myFileDateTime = myFileDateTime.AddDays(11);

                                if (myFileDateTime < DateTime.Now)
                                {
                                    File.Delete(file);
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                if (blnBackupDatabase == false)
                {
                    string strBackupFileName = strDataBaseName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_Daily.bak";

                    strQry.Clear();
                    strQry.AppendLine("BACKUP DATABASE " + strDataBaseName + " TO DISK = '" + strFileDirectory + "\\" + strBackupFileName + "' WITH CHECKSUM ");

                    Execute_SQLCommand(strQry.ToString());

                    blnBackupDatabase = true;

                    WriteLog("BackupDatabase of Database InteractPayrollClient Successful");
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog("BackupDatabase", ex);
            }
            finally
            {
                double tickTime = 0;

                if (blnBackupDatabase == true)
                {
                    //Set To Fire Next Day - 12:55 Am
                    DateTime dtNextTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 55, 0);

                    if (DateTime.Now > dtNextTime)
                    {
                        dtNextTime = dtNextTime.AddDays(1);
                    }

                    tickTime = (double)(dtNextTime - DateTime.Now).TotalMilliseconds;
                }
                else
                {
                    //Set Next Time for Timer to Fire
                    tickTime = Calculate_Next_Time_Timer_Fires_In_Minutes(60);
                }

                tmrDatabaseBackupTimer.Interval = tickTime;
                tmrDatabaseBackupTimer.Start();
            }
        }

        private void Check_Log_For_Archiving()
        {
            bool blnMoveFile = false;
            DateTime myFileDateTime = DateTime.Now;

            try
            {
                FileInfo myFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + pvtstrLogFileName + "_Log.txt");

                if (myFileInfo.Exists == true)
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
                                    myFileDateTime = DateTime.ParseExact(file.Substring(intOffset + 47, 8), "yyyyMMdd", null);

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
        
        public Ping GetPing()
        {
            //Time & Attendance
            Ping Ping = new Ping();
            Ping.OK = "Y";

            return Ping;
        }

        public Restart RestartFingerPrintClockTimeAttendanceService()
        {
#if(DEBUG)
            if (AppDomain.CurrentDomain.GetData("URLClientPath").ToString() == "")
            {
                string strBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                strBaseDirectory += "bin\\";

                //Server Files with _ Renamed (Only in Debug Mode)
                string[] strFiles = Directory.GetFiles(strBaseDirectory, "*.*_");

                foreach (string strFile in strFiles)
                {
                    try
                    {
                        File.Copy(strFile, strFile.Substring(0, strFile.Length - 1),true);
                        File.Delete(strFile.Substring(0, strFile.Length - 1));
                    }
                    catch
                    {
                    }
                }
            }
#endif
            Restart Restart = new Restart();

            Restart.OK = "N";

            try
            {
                GetServiceInformationResponse getServiceInformationResponse = GetServiceInformation("FingerPrintClockTimeAttendanceServiceStartStop");
                
                if (getServiceInformationResponse.ServiceLoginUser != "LocalSystem")
                {
                    WriteLog("**********FingerPrintClockTimeAttendanceServiceStartStop Login = " + getServiceInformationResponse.ServiceLoginUser);
                }

                getServiceInformationResponse = GetServiceInformation("FingerPrintClockTimeAttendanceService");

                if (getServiceInformationResponse.ServiceLoginUser != "LocalSystem")
                {
                    WriteLog("**********FingerPrintClockTimeAttendanceService Login = " + getServiceInformationResponse.ServiceLoginUser);
                }
                
                ServiceController service = new ServiceController("FingerPrintClockTimeAttendanceService");

                if (service != null)
                {
                    if (service.Status.ToString() == "Running")
                    {
                        WriteLog("********** Stopping FingerPrintClockTimeAttendanceService **********");
                        service.Stop();

                        while (service.Status.ToString() != "Stopped")
                        {
                            service.Refresh();
                        }

                        GC.Collect();

                        DateTime myNewDateTime = DateTime.Now.AddSeconds(2);

                        while (DateTime.Now < myNewDateTime)
                        {
                        }

                        WriteLog("********** Starting FingerPrintClockTimeAttendanceService **********");
                        service.Start();

                        while (service.Status.ToString() != "Running")
                        {
                            service.Refresh();
                        }

                        //Wait Another 5 Seconds to Make sure Service is Up
                        myNewDateTime = DateTime.Now.AddSeconds(5);

                        while (DateTime.Now < myNewDateTime)
                        {
                        }

                        Restart.OK = "Y";
                    }
                    else
                    {
                        //Try Start Service
                        WriteLog("********** NB NB FingerPrintClockTimeAttendanceService WAS NOT RUNNING **********");
                        WriteLog("********** Starting FingerPrintClockTimeAttendanceService **********");
                        service.Start();

                        while (service.Status.ToString() != "Running")
                        {
                            service.Refresh();
                        }

                        //Wait Another 5 Seconds to Make sure Service is Up
                        DateTime myNewDateTime = DateTime.Now.AddSeconds(5);

                        while (DateTime.Now < myNewDateTime)
                        {
                        }

                        Restart.OK = "Y";
                    }
                }
                else
                {
                    WriteLog("**********FingerPrintClockTimeAttendanceService NOT INSTALLED ???????????????");
                }
            }
            catch(Exception ex)
            {
                WriteExceptionLog("RestartFingerPrintClockTimeAttendanceService", ex);
            }

            return Restart;
        }

        public RestartFingerPrintClockTimeAttendanceServiceNewResponse RestartFingerPrintClockTimeAttendanceServiceNew()
        {
            WriteLog("RestartFingerPrintClockTimeAttendanceServiceNew Entered");
            RestartFingerPrintClockTimeAttendanceServiceNewResponse response = new RestartFingerPrintClockTimeAttendanceServiceNewResponse();

            response.FingerPrintClockTimeAttendanceServiceStartStopLoginUser = "";
          
            response.FingerPrintClockTimeAttendanceServiceStarted = false;
            response.FingerPrintClockTimeAttendanceServiceLoginUser = "";

#if(DEBUG)
            if (AppDomain.CurrentDomain.GetData("URLClientPath").ToString() == "")
            {
                string strBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                strBaseDirectory += "bin\\";

                //Server Files with _ Renamed (Only in Debug Mode)
                string[] strFiles = Directory.GetFiles(strBaseDirectory, "*.*_");

                foreach (string strFile in strFiles)
                {
                    File.Copy(strFile, strFile.Substring(0, strFile.Length - 1),true);
                    File.Delete(strFile);
                }
            }
#endif
            try
            {
                GetServiceInformationResponse getServiceInformationResponse = GetServiceInformation("FingerPrintClockTimeAttendanceServiceStartStop");

                response.FingerPrintClockTimeAttendanceServiceStartStopLoginUser = getServiceInformationResponse.ServiceLoginUser;

                if (getServiceInformationResponse.ServiceLoginUser != "LocalSystem")
                {
                    WriteLog("RestartFingerPrintClockTimeAttendanceServiceNew Login = " + getServiceInformationResponse.ServiceLoginUser);
                }

                ServiceStartResponse serviceStartResponse = ServiceStart("FingerPrintClockTimeAttendanceService");

                response.FingerPrintClockTimeAttendanceServiceStarted = serviceStartResponse.ServiceStarted;
                response.FingerPrintClockTimeAttendanceServiceLoginUser = serviceStartResponse.ServiceLoginUser;

                if (serviceStartResponse.ServiceLoginUser != "LocalSystem")
                {
                    WriteLog("FingerPrintClockTimeAttendanceService Login = " + getServiceInformationResponse.ServiceLoginUser);
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog("RestartFingerPrintClockTimeAttendanceServiceNew", ex);
            }

            return response;
        }

        private ServiceStartResponse ServiceStart(string serviceName)
        {
            ServiceStartResponse serviceStartResponse = new ServiceStartResponse();
            serviceStartResponse.ServiceName = serviceName;
            serviceStartResponse.ServiceLoginUser = "";
            serviceStartResponse.ServiceInstalled = false;
            serviceStartResponse.ServiceEnabled = false;
            serviceStartResponse.ServiceStarted = false;
            
            WriteLog("ServiceStart Entered serviceName = " + serviceName);

            try
            {
                //Delay for at Least 10 Seconds
                DateTime dtMinimumDelayDateTime = DateTime.Now.AddSeconds(10);

                ServiceController serviceController = new ServiceController(serviceName);

                if (serviceController != null)
                {
                    serviceStartResponse.ServiceInstalled = true;

                    WriteLog("ServiceStart ServiceInstalled = true");

                    ManagementObject managementObjectService = new ManagementObject("Win32_Service.Name='" + serviceName + "'");
                    managementObjectService.Get();

                    serviceStartResponse.ServiceLoginUser = managementObjectService["StartName"].ToString();

                    if (managementObjectService["StartMode"].ToString() != "Disabled")
                    {
                        serviceStartResponse.ServiceEnabled = true;

                        TimeSpan timeoutStart = TimeSpan.FromSeconds(20);

                        if (serviceController.Status.ToString() == "Running")
                        {
                            //Try Restart Service
                            try
                            {
                                WriteLog("ServiceStart Before Stop");
                                serviceController.Stop();

                                while (serviceController.Status.ToString() != "Stopped")
                                {
                                    serviceController.Refresh();
                                }

                                GC.Collect();

                                DateTime myNewDateTime = DateTime.Now.AddSeconds(5);

                                while (DateTime.Now < myNewDateTime)
                                {
                                }

                                WriteLog("ServiceStart Before Start");
                                serviceController.Start();

                                while (serviceController.Status.ToString() != "Running")
                                {
                                    serviceController.Refresh();
                                }

                                WriteLog("ServiceStart Started");
                                serviceStartResponse.ServiceStarted = true;

                                while (dtMinimumDelayDateTime > DateTime.Now)
                                {
                                }
                            }
                            catch (Exception ex)
                            {
                                WriteExceptionLog("ServiceStart 1", ex);
                              
                                if (serviceController.Status.ToString() == "Running")
                                {
                                    serviceStartResponse.ServiceStarted = true;
                                }
                            }
                        }
                        else
                        {
                            //Try Start Service
                            try
                            {
                                serviceController.Start();

                                while (serviceController.Status.ToString() != "Running")
                                {
                                    serviceController.Refresh();
                                }

                                WriteLog("ServiceStart Started");

                                serviceStartResponse.ServiceStarted = true;

                                while (dtMinimumDelayDateTime > DateTime.Now)
                                {
                                }
                            }
                            catch (Exception ex)
                            {
                                WriteExceptionLog("ServiceStart 2", ex);

                                if (serviceController.Status.ToString() == "Running")
                                {
                                    serviceStartResponse.ServiceStarted = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog("ServiceStart 3", ex);
            }

            return serviceStartResponse;
        }

        private GetServiceInformationResponse GetServiceInformation(string serviceName)
        {
            GetServiceInformationResponse getServiceInformationResponse = new GetServiceInformationResponse();
            getServiceInformationResponse.ServiceName = serviceName;
            getServiceInformationResponse.ServiceLoginUser = "";
            getServiceInformationResponse.ServiceInstalled = false;
            getServiceInformationResponse.ServiceEnabled = false;
            getServiceInformationResponse.ServiceStarted = false;
           
            try
            {
                //Delay for at Least 5 Seconds
                DateTime dtMinimumDelayDateTime = DateTime.Now.AddSeconds(5);

                ServiceController serviceController = new ServiceController(serviceName);

                if (serviceController != null)
                {
                    getServiceInformationResponse.ServiceInstalled = true;
                    
                    if (serviceController.Status.ToString() == "Running")
                    {
                        getServiceInformationResponse.ServiceStarted = true;
                    }

                    ManagementObject managementObjectService = new ManagementObject("Win32_Service.Name='" + serviceName + "'");
                    managementObjectService.Get();

                    getServiceInformationResponse.ServiceLoginUser = managementObjectService["StartName"].ToString();

                    if (managementObjectService["StartMode"].ToString() != "Disabled")
                    {
                        getServiceInformationResponse.ServiceEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog("GetServiceInformation", ex);
            }

            return getServiceInformationResponse;
        }

        private void Create_DataTable_Client(string parstrQry, DataSet parDataSet, string parstrSourceTable)
        {
#if(DEBUG)
            parstrQry = parstrQry.Replace("InteractPayrollClient", "InteractPayrollClient_Debug");
#endif
            pvtSqlConnectionClient = new SqlConnection(pvtstrConnectionClient);

            pvtSqlCommandClient = new SqlCommand(parstrQry, pvtSqlConnectionClient);

            pvtSqlDataAdapterClient = new SqlDataAdapter(pvtSqlCommandClient);

            //Opens and Closes the Connection object - pvtSqlConnection
            pvtSqlDataAdapterClient.Fill(parDataSet, parstrSourceTable);

            parDataSet.AcceptChanges();
        }

        private void Execute_SQLCommand(string parstrQry)
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
                        tw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "ServiceStartStop " + Message);
                    }
                }
            }
            catch (Exception ex)
            {
                if (intNumberRetries > 0)
                {
                    try
                    {
                        using (FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + pvtstrLogFileName + "_Error_Log.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                        {
                            using (TextWriter tw = new StreamWriter(fs))
                            {
                                tw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "ServiceStartStop " + Message);
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