using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InteractPayrollClient;
using System.Data;
using System.IO;

namespace ValiditeClockingsSyncViaDB
{
    class Program
    {
        static clsISClientUtilities clsISClientUtilities;

        static DataSet pvtDataSet;

        static bool pvtblnFromWindowsService = true;

        static string pvtstrLogFileName = "ValiditeClockingsService";

        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    pvtblnFromWindowsService = false;
                }
               
                if (pvtblnFromWindowsService == false)
                {
                    Console.WriteLine("Entered ValiditeClockingsSyncViaDB");
                }
                
                Check_Log_For_Archiving();
                
                string strConfig = AppDomain.CurrentDomain.BaseDirectory + "URLClientConfig.txt";

                FileInfo fiFileInfo = new FileInfo(strConfig);

                if (fiFileInfo.Exists == true)
                {
                    StreamReader srStreamReader = File.OpenText(strConfig);

                    string strURLPath = srStreamReader.ReadLine();

                    srStreamReader.Close();

                    AppDomain.CurrentDomain.SetData("URLClientPath", strURLPath);
                }
                else
                {
                    AppDomain.CurrentDomain.SetData("URLClientPath", "");
                }

                clsISClientUtilities = new clsISClientUtilities(null,"busValiditeClockingsSyncViaDB");

                pvtDataSet = new DataSet();

                object[] objParm = new object[1];
                objParm[0] = "Hullo";
                byte[] bytCompress = (byte[])clsISClientUtilities.DynamicFunction("Insert_Records", objParm, false);

                pvtDataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(bytCompress);

                if (pvtDataSet.Tables["LinksMissing"].Rows.Count > 0)
                {
                    if (pvtblnFromWindowsService == false)
                    {
                        Console.WriteLine("EMPLOYEES NOT LINKED START*********");
                    }

                    WriteLog("EMPLOYEES NOT LINKED START*********");

                    for (int intCount = 0; intCount < pvtDataSet.Tables["LinksMissing"].Rows.Count; intCount++)
                    {
                        if (pvtblnFromWindowsService == false)
                        {
                            Console.WriteLine("Employee " + pvtDataSet.Tables["LinksMissing"].Rows[intCount]["EMPLOYEE_NO"].ToString());
                        }

                        WriteLog("Employee " + pvtDataSet.Tables["LinksMissing"].Rows[intCount]["EMPLOYEE_NO"].ToString());
                    }

                    if (pvtblnFromWindowsService == false)
                    {
                        Console.WriteLine("EMPLOYEES NOT LINKED END*********");
                    }

                    WriteLog("EMPLOYEES NOT LINKED END*********");
                }

                if (pvtblnFromWindowsService == false)
                {
                    Console.WriteLine("Number of Records Inserted = " + pvtDataSet.Tables["Clockings"].Rows.Count);
                }

                WriteLog("Number of Records Inserted = " + pvtDataSet.Tables["Clockings"].Rows.Count);

                if (pvtblnFromWindowsService == false)
                {
                    Console.WriteLine("Exit ValiditeClockingsSyncViaDB");
                }
            }
            catch (Exception ex)
            {
                string strException = ex.Message;

                if (ex.InnerException != null)
                {
                    strException = strException + " " + ex.InnerException.Message;
                }

                if (pvtblnFromWindowsService == false)
                {
                    Console.WriteLine("Exception = " + strException);
                }

                WriteExceptionLog("ClockingsService", ex);
            }
            finally
            {
                if (pvtblnFromWindowsService == false)
                {
                    Console.ReadLine();
                }
            }
        }

        static void Check_Log_For_Archiving()
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
                            WriteExceptionErrorLog("Check_Log_For_Archiving Read Line", ex);
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
                                if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Backup") == false)
                                {
                                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Backup");
                                }

                                File.Copy(AppDomain.CurrentDomain.BaseDirectory + pvtstrLogFileName + "_Log.txt", AppDomain.CurrentDomain.BaseDirectory + "Backup\\" + pvtstrLogFileName + "_" + myFileDateTime.ToString("yyyyMMdd") + "_Log.txt", true);
                                File.Delete(AppDomain.CurrentDomain.BaseDirectory + pvtstrLogFileName + "_Log.txt");
                            }
                            catch (Exception ex)
                            {
                                WriteExceptionErrorLog("Check_Log_For_Archiving File Delete ", ex);
                            }
                        }

                        //Delete Files Older than 10 Days
                        //Cleanup Daily Backup Files Older than 10 Days
                        string[] strDbBackupFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "Backup", pvtstrLogFileName + "_*_Log.txt");

                        foreach (string file in strDbBackupFiles)
                        {
                            int intOffset = file.IndexOf(pvtstrLogFileName + "_");

                            if (intOffset > -1)
                            {
                                try
                                {
                                    myFileDateTime = DateTime.ParseExact(file.Substring(intOffset + 25, 8), "yyyyMMdd", null);

                                    myFileDateTime = myFileDateTime.AddDays(11);

                                    if (myFileDateTime < DateTime.Now)
                                    {
                                        File.Delete(file);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    WriteExceptionErrorLog("Check_Log_For_Archiving File Backup Delete '" + file + "'", ex);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteExceptionErrorLog("Check_Log_For_Archiving - Move Files", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteExceptionErrorLog("Check_Log_For_Archiving", ex);
            }
        }

        static private void WriteExceptionLog(string Message, Exception ex)
        {
            string strInnerExceptionMessage = "";

            if (ex.InnerException != null)
            {
                strInnerExceptionMessage = ex.InnerException.Message;
            }

            WriteLog(Message + " ********** Exception = " + ex.Message + " " + strInnerExceptionMessage);
        }

        static private void WriteLog(string Message)
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

        static private void WriteExceptionErrorLog(string Message, Exception ex)
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
