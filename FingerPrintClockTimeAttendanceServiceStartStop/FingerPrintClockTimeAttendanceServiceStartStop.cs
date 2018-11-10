using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Channels;
using System.Reflection;
using Microsoft.Win32;
using System.Windows.Forms;
using System.IO.Compression;
using FingerPrintClockServer;
using System.Configuration;
using System.Timers;

namespace FingerPrintClockServer
{
    public partial class FingerPrintClockTimeAttendanceServiceStartStop : ServiceBase
    {
        System.Type typFingerPrintClockServiceStartStop;
        object FingerPrintClockServiceServiceStartStop;

        bool pvtblnMajorError = false;
        string pvtstrLogFileName = "FingerPrintClockTimeAttendanceServiceStartStop";
          
        public FingerPrintClockTimeAttendanceServiceStartStop()
        {
            try
            {
                WriteLog("");
                WriteLog("FingerPrintClockTimeAttendanceServiceStartStop (NEW Program) InitializeComponent ####################");

                InitializeComponent();
                
                AppDomain.CurrentDomain.SetData("NewFingerPrintClockTimeAttendanceService", "Y");

                //Replace Old dll Files etc with Newer ones
                string[] strFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.*_");

                foreach (string strFile in strFiles)
                {
                    if (strFile.IndexOf("FingerPrintClockTimeAttendanceServiceStartStop.exe_") > -1)
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
                
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DBConfig.txt") == false)
                {
                    pvtblnMajorError = true;
                    WriteLog("DBConfig.txt does Not Exist. **********");
                }
                
                if (pvtblnMajorError == false)
                {
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockServiceStartStop.dll") == true)
                    {
                        //Start the Service
                        Assembly asAssembly = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockServiceStartStop.dll");
                        typFingerPrintClockServiceStartStop = asAssembly.GetType("FingerPrintClockServer.FingerPrintClockServiceStartStop");
                        FingerPrintClockServiceServiceStartStop = Activator.CreateInstance(typFingerPrintClockServiceStartStop);
                    }
                    else
                    {
                        pvtblnMajorError = true;
                        WriteLog(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockServiceStartStop.dll Does NOT EXIST **********");
                    }
                }

                if (pvtblnMajorError == true)
                {
                    WriteLog("FingerPrintClockTimeAttendanceServiceStartStop CANNOT Run because of Errors Listed Above **********");
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog("FingerPrintClockTimeAttendanceServiceStartStop Exception ", ex);
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                if (pvtblnMajorError == false)
                {
                    WriteLog("OnStart Entered ####################");

                    MethodInfo mi = typFingerPrintClockServiceStartStop.GetMethod("OnStart");
                    mi.Invoke(FingerPrintClockServiceServiceStartStop, null);
                }
                else
                {
                    WriteLog("OnStart Exit with Errors - See Above ####################");
                    return;
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog("@@@@@@@@@@@@@@@@@@@@ OnStart FingerPrintClockServiceStartStop Open FAILED", ex);
            }
        }
                     
        protected override void OnStop()
        {
            try
            {
                WriteLog("OnStop Entered ####################");

                MethodInfo mi = typFingerPrintClockServiceStartStop.GetMethod("OnStop");
                mi.Invoke(FingerPrintClockServiceServiceStartStop, null);
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

                using (StreamWriter writeErrorLog = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + pvtstrLogFileName + "_Error_Log.txt", true))
                {
                    writeErrorLog.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + Message + " Exception = " + ex.Message);
                }
            }
            catch
            {
            }
        }
    }
}
