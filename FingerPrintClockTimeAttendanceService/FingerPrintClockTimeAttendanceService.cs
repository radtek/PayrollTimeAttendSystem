using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Channels;
using System.Reflection;
using InteractPayroll;
using InteractPayrollClient;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;

namespace FingerPrintClockServer
{
    public partial class FingerPrintClockTimeAttendanceService : ServiceBase
    {
        System.Type typFingerPrintClockService;
        object FingerPrintClockServiceService;

        bool pvtblnMajorError = false;
        string pvtstrLogFileName = "FingerPrintClockTimeAttendanceService";
        
        public FingerPrintClockTimeAttendanceService()
        {
            try
            {
                WriteLog("");
                WriteLog("FingerPrintClockTimeAttendanceService (NEW Program )InitializeComponent ####################");

                InitializeComponent();

                AppDomain.CurrentDomain.SetData("NewFingerPrintClockTimeAttendanceService", "Y");

                //Replace Old dll Files etc with Newer ones
                string[] strFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.*_");

                foreach (string strFile in strFiles)
                {
                    if (strFile.IndexOf("FingerPrintClockTimeAttendanceService.exe_") > -1)
                    {
                        continue;
                    }

                    try
                    {
                        File.Copy(strFile, strFile.Substring(0, strFile.Length - 1), true);
                        File.Delete(strFile);
                        WriteLog("File " + strFile.Substring(0, strFile.Length - 1) + " Replaced");
                    }
                    catch (Exception ex)
                    {
                        WriteExceptionLog("File Copy Failed " + strFile, ex);
                    }
                }
                
                //Set Digital Persona as Default Software 
                AppDomain.CurrentDomain.SetData("SoftwareToUse", "D");
                
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DBConfig.txt") == false)
                {
                    pvtblnMajorError = true;
                    WriteLog("DBConfig.txt does Not Exist. **********");
                }
                
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "clsDBConnectionObjects.dll") == false)
                {
                    pvtblnMajorError = true;
                    WriteLog("clsDBConnectionObjects.dll does Not Exist. **********");
                }
                
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "busClientTimeSheetDynamicUpload.dll") == false)
                {
                    pvtblnMajorError = true;
                    WriteLog("busClientTimeSheetDynamicUpload.dll MISSING - Dynamic Upload of Timesheets will not Work**********");
                }
  
                if (pvtblnMajorError == false)
                {
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockService.dll") == true)
                    {
                        //Start the Service
                        Assembly asAssembly = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockService.dll");
                        typFingerPrintClockService = asAssembly.GetType("FingerPrintClockServer.FingerPrintClockService");
                        FingerPrintClockServiceService = Activator.CreateInstance(typFingerPrintClockService);
                    }
                    else
                    {
                        pvtblnMajorError = true;
                        WriteLog(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockService.dll Does NOT EXIST **********");
                    }
                }

                if (pvtblnMajorError == true)
                {
                    WriteLog("FingerPrintClockTimeAttendanceService CANNOT Run because of Errors Listed Above **********");
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog("FingerPrintClockTimeAttendanceService", ex);
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                if (pvtblnMajorError == false)
                {
                    WriteLog("OnStart Entered ####################");
                    
                    MethodInfo mi = typFingerPrintClockService.GetMethod("OnStart");
                    mi.Invoke(FingerPrintClockServiceService, null);
                }
                else
                {
                    WriteLog("OnStart Exit with Errors - See Above ####################");
                    return;
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog("@@@@@@@@@@@@@@@@@@@@ OnStart FingerPrintClockService Open FAILED", ex);
            }
        }
       
        protected override void OnStop()
        {
            try
            {
                WriteLog("OnStop Entered ####################");

                MethodInfo mi = typFingerPrintClockService.GetMethod("OnStop");
                mi.Invoke(FingerPrintClockServiceService, null);
            }
            catch (Exception ex)
            {
                WriteExceptionLog("OnStop", ex);
            }
            finally
            {
                WriteLog("OnStop Exit ####################");

                GC.Collect();
            }
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
            try
            {
                using (StreamWriter writeLog = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + pvtstrLogFileName + "_Log.txt", true))
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
                string strInnerExceptionMessage = "";

                if (ex.InnerException != null)
                {
                    strInnerExceptionMessage = ex.InnerException.Message;
                }

                using (StreamWriter writeExceptionErrorLog = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + pvtstrLogFileName + "_Error_Log.txt", true))
                {
                    writeExceptionErrorLog.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + Message + " Exception = " + ex.Message);
                }
            }
            catch
            {
            }
        }
    }
}
