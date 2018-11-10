using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ValiditeWebSite.Helpers
{
    public class Helpers
    {
        string pvtstrLogFileName = "ValiditeWebSite";
        string pvtstrLogFileDirectory = "";
        string pvtstrFromClassName = "";
        public Helpers(string fromClassName)
        {
            try
            {
                pvtstrFromClassName = fromClassName;

                pvtstrLogFileDirectory = WebConfigurationManager.AppSettings["LogFileDirectory"].ToString();
            }
            catch(Exception ex)
            {
                string strInnerExceptionMessage = "";

                if (ex.InnerException != null)
                {
                    strInnerExceptionMessage = ex.InnerException.Message;
                }

                using (StreamWriter writeExceptionErrorLog = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + pvtstrLogFileName + "_Error_Log.txt", true))
                {
                    writeExceptionErrorLog.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Helpers" + " Exception = " + ex.Message + " " + strInnerExceptionMessage);
                }
            }
        }
        public void WriteLog(string Message)
        {
            try
            {
                using (StreamWriter writeLog = new StreamWriter(pvtstrLogFileDirectory + pvtstrLogFileName + "_Log.txt", true))
                {
                    writeLog.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "From = " + pvtstrFromClassName + " " + Message);
                }
            }
            catch (Exception ex)
            {
                WriteExceptionErrorLog(Message, ex);
            }
        }
        public void WriteExceptionErrorLog(string Message, Exception ex)
        {
            try
            {
                string strInnerExceptionMessage = "";

                if (ex.InnerException != null)
                {
                    strInnerExceptionMessage = ex.InnerException.Message;
                }

                using (StreamWriter writeExceptionErrorLog = new StreamWriter(pvtstrLogFileDirectory + pvtstrLogFileName + "_Error_Log.txt", true))
                {
                    writeExceptionErrorLog.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "From = " + pvtstrFromClassName + " " + Message + " Exception = " + ex.Message + " " + strInnerExceptionMessage);
                }
            }
            catch
            {
                string strInnerExceptionMessage = "";

                if (ex.InnerException != null)
                {
                    strInnerExceptionMessage = ex.InnerException.Message;
                }

                using (StreamWriter writeExceptionErrorLog = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + pvtstrLogFileName + "_Error_Log.txt", true))
                {
                    writeExceptionErrorLog.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + " Helpers WriteExceptionErrorLog" + " From = " + pvtstrFromClassName + " " + " Exception = " + ex.Message + " " + strInnerExceptionMessage);
                }
            }
        }


    }
}