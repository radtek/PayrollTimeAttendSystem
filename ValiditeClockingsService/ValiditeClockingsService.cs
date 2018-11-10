using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using System.Timers;

namespace ValiditeClockingsService
{
    public partial class ValiditeClockingsService : ServiceBase
    {
        string pvtstrLogFileName = "ValiditeClockingsService";

        private bool blnCorrectToRun = false;
        
        private string pvtstrProgramToRun = "";
        private int pvtintSyncTimerInSeconds = 0;

        System.Timers.Timer tmrRunProgram;

        public ValiditeClockingsService()
        {
            string strWhere = "";

            try
            {
                WriteLog("");
                WriteLog("ValiditeClockingsService InitializeComponent ####################");

                InitializeComponent();

                FileInfo myFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "ValiditeClockingsService.exe.config");

                if (myFileInfo.Exists == true)
                {
                    strWhere = "ProgramToRun";
                    pvtstrProgramToRun = ConfigurationManager.AppSettings["ProgramToRun"].ToString();
                    strWhere = "SyncTimerInSeconds";
                    pvtintSyncTimerInSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["SyncTimerInSeconds"]);

                    myFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + pvtstrProgramToRun);

                    if (myFileInfo.Exists == true)
                    {
                        string strConfig = AppDomain.CurrentDomain.BaseDirectory + "URLClientConfig.txt";

                        FileInfo fiFileInfo = new FileInfo(strConfig);

                        if (fiFileInfo.Exists == true)
                        {
                            StreamReader srStreamReader = File.OpenText(strConfig);

                            string strURLPath = srStreamReader.ReadLine();

                            srStreamReader.Close();

                            if (Check_IP(strURLPath) != 0)
                            {
                                WriteLog("Error in 'URLClientConfig.txt'\n\nSpeak To System Administrator.");
                            }
                            else
                            {
                                blnCorrectToRun = true;
                            }
                        }
                        else
                        {
                            WriteLog(AppDomain.CurrentDomain.BaseDirectory + "URLClientConfig.txt" + " File does NOT Exist ####################");
                        }
                    }
                    else
                    {
                        WriteLog(AppDomain.CurrentDomain.BaseDirectory + pvtstrProgramToRun + " File does NOT Exist ####################");
                    }
                }
                else
                {
                    WriteLog(myFileInfo.FullName + " does Not Exist. SERVICE WILL NOT RUN CORRECTLY");
                }
            }
            catch (Exception ex)
            {
                if (strWhere == "")
                {
                    WriteExceptionLog("ValiditeClockingsService", ex);
                }
                else
                {
                    WriteExceptionLog("ValiditeClockingsService Config Error " + strWhere, ex);
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            if (blnCorrectToRun == true)
            {
                WriteLog("OnStart Entered ####################");

                tmrRunProgram = new System.Timers.Timer();
                tmrRunProgram.Elapsed += RunProgramTimerEvent;
                tmrRunProgram.Interval = 1000;
                tmrRunProgram.Start();
                WriteLog("ValiditeClockingsService Timer Started");
            }
            else
            { 
                WriteLog("**********OnStart Exit with Errors - See Above**********");
            }
        }

        private void RunProgramTimerEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                tmrRunProgram.Stop();

                if (tmrRunProgram.Interval != pvtintSyncTimerInSeconds * 1000)
                {
                    tmrRunProgram.Interval = pvtintSyncTimerInSeconds * 1000;
                    
                    WriteLog("Timer Set to " + pvtintSyncTimerInSeconds + " Seconds");
                }
                
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.UseShellExecute = false;
                processStartInfo.FileName = pvtstrProgramToRun;
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processStartInfo.Arguments = "1";

                Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                 WriteExceptionLog("RunProgramTimerEvent", ex);
            }
            finally
            {
                tmrRunProgram.Start();
            }
        }
        
        protected override void OnStop()
        {
            try
            {
                WriteLog("OnStop Entered ####################");
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

        internal void TestClockingService()
        {
            string[] args = new string[1];

            this.OnStart(args);
            Console.ReadLine();
            //this.OnStop();
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

        int Check_IP(string strIP)
        {
            int intReturn = 1;

            try
            {
                string[] strParts = strIP.Split('.');

                if (strParts.Length == 4)
                {
                    string[] strLastPart = strParts[3].Split(':');

                    int intResult = 0;

                    if (strLastPart.Length == 2)
                    {
                        if (int.TryParse(strParts[0], out intResult) == true
                            & int.TryParse(strParts[1], out intResult) == true
                            & int.TryParse(strParts[2], out intResult) == true
                            & int.TryParse(strLastPart[0], out intResult) == true
                                & int.TryParse(strLastPart[1], out intResult) == true)
                        {
                            intReturn = 0;
                        }
                    }
                }
            }
            catch
            {
            }

            return intReturn;
        }
    }
}
