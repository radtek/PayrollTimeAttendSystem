using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Data;
using System.Data.SqlClient;
using System.Timers;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.Services;
using InteractPayrollClient;
using System.ServiceProcess;
using FingerPrintClockServer;

namespace TimeSheetDynamicUpload
{
    class Program
    {
        static Assembly asAssembly;
        static System.Type typObjectType;
        static object busDynamicService;
               
        static clsISClientUtilities clsISClientUtilities;

        static clsRestartFingerPrintClockTimeAttendanceService clsRestartFingerPrintClockTimeAttendanceService;

        static object pvtReturnObject;

        static localhost.busWebDynamicServices busWebDynamicServices;
        static bool pvtblnCallBackComplete = false;

        static bool pvtblnFromTimeSheetScheduler = true;

        static byte[] pvtbytCompress;
        static object[] pvtobjParm;
                                     
        static string[] strUploadKey = new string[100];
        static Int64[] Int64UploadKeyCompanyNo = new Int64[100];
      
        static byte[] _salt = Encoding.ASCII.GetBytes("ErrolLeRoux");
        static string sharedSecret = "Interact";

        static string pvtstrLogFileName = "TimeSheetDynamicUploadIS_Log.txt";
        static string pvtstrLogFileErrorName = "TimeSheetDynamicUploadIS_Error_Write_Log.txt";

        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    pvtblnFromTimeSheetScheduler = false;
                }

                AppDomain.CurrentDomain.SetData("FromProgramInd", "T");
#if (DEBUG)
                string strBaseDirectory = AppDomain.CurrentDomain.BaseDirectory + "bin\\";

                //Server Files with _ Renamed (Only in Debug Mode)
                string[] strFiles = Directory.GetFiles(strBaseDirectory, "*.*_");

                foreach (string strFile in strFiles)
                {
                    if (strFile.IndexOf("FingerPrintClockServiceStartStop.dll_") > -1)
                    {
                        //Leave so Can Test Lower down in Code
                        continue;
                    }

                    File.Delete(strFile.Substring(0, strFile.Length - 1));

                    File.Move(strFile, strFile.Substring(0, strFile.Length - 1));
                }
#endif
                FileInfo fiFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "TimeSheetDynamicUpload.exe");

                if (fiFileInfo.Exists == true)
                {
                    if (Convert.ToInt64(fiFileInfo.LastWriteTime.AddSeconds(1).ToString("yyyyMMddHHmmss")) < 20130304080019)
                    {
                        if (pvtblnFromTimeSheetScheduler == false)
                        {
                            Console.WriteLine("File 'TimeSheetDynamicUpload.exe' TimeStamp Invalid\nSpeak to System Administrator");
                            Console.ReadLine();
                        }

                        WriteLog("File 'TimeSheetDynamicUpload.exe' TimeStamp Invalid\nSpeak to System Administrator");

                        return;
                    }
                }

                fiFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "clsISClientUtilities.dll");

                if (fiFileInfo.Exists == false)
                {
                    if (pvtblnFromTimeSheetScheduler == false)
                    {
                        Console.WriteLine("File 'clsISClientUtilities.dll' Does NOT Exist\nSpeak to System Administrator");
                        Console.ReadLine();
                    }

                    WriteLog("File 'clsISClientUtilities.dll' Does NOT Exist\nSpeak to System Administrator");

                    return;
                }

                fiFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "URLConfig.txt_");

                if (fiFileInfo.Exists == true)
                {
                    try
                    {
                        File.Copy(AppDomain.CurrentDomain.BaseDirectory + "URLConfig.txt_", AppDomain.CurrentDomain.BaseDirectory + "\\URLConfig.txt", true);
                        File.Delete(AppDomain.CurrentDomain.BaseDirectory + "URLConfig.txt_");
                    }
                    catch(Exception ex)
                    {
                        WriteExceptionLog("URLConfig.txt Exception", ex);
                    }
                }

                fiFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "URLConfig.txt");

                if (fiFileInfo.Exists == true)
                {
                    ConsoleKey strReply = ConsoleKey.Y;
#if(DEBUG)
                    Console.WriteLine("Would you Like to Connect to the Internet?\nY=Yes");
                    strReply = Console.ReadKey(true).Key;
#endif
                    if (strReply == ConsoleKey.Y)
                    {
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
                    }
                    else
                    {
                        AppDomain.CurrentDomain.SetData("URLPath", "");
                    }
                }
                else
                {
#if(DEBUG)
                    AppDomain.CurrentDomain.SetData("URLPath", "");
#else
                if (pvtblnFromTimeSheetScheduler == false)
                {
                    Console.WriteLine("File 'URLConfig.txt' Does NOT Exist\nSpeak to System Administrator");
                    Console.ReadLine();
                }

                WriteLog("File 'URLConfig.txt' Does NOT Exist\nSpeak to System Administrator");

                return;
#endif
                }

                //2013-02-28
                fiFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "URLClientConfig.txt");

                if (fiFileInfo.Exists == true)
                {
                    using (StreamReader srStreamReader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "URLClientConfig.txt"))
                    {
                        try
                        {
                            string strURLPath = srStreamReader.ReadLine();

                            AppDomain.CurrentDomain.SetData("URLClientPath", strURLPath);
                        }
                        catch (Exception ex)
                        {
                            WriteExceptionLog("URLClientConfig.txt ReadLine Exception", ex);
                        }
                    }
                }
                else
                {
#if(DEBUG)
                    AppDomain.CurrentDomain.SetData("URLClientPath", "");
#else
                    //Set To Read Local Machine
                    string strFile = "127.0.0.1:8000";

                    using (StreamWriter swStreamWriter = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "URLClientConfig.txt"))
                    {
                        swStreamWriter.WriteLine(strFile);
                    }

                    AppDomain.CurrentDomain.SetData("URLClientPath", "127.0.0.1:8000");
#endif
                }

                clsISClientUtilities = new clsISClientUtilities(null, "busClientTimeSheetDynamicUpload");

                if (AppDomain.CurrentDomain.GetData("URLClientPath").ToString() != "")
                {
                    //Calls Web Service Internally
                    clsRestartFingerPrintClockTimeAttendanceService = new clsRestartFingerPrintClockTimeAttendanceService("");
                }
                else
                {
                    clsRestartFingerPrintClockTimeAttendanceService = new clsRestartFingerPrintClockTimeAttendanceService("FingerPrintClockServiceStartStop");
                }

                if (AppDomain.CurrentDomain.GetData("URLPath").ToString() != "")
                {
                    busWebDynamicServices = new localhost.busWebDynamicServices();

                    int intTimeOut = busWebDynamicServices.Timeout;

                    int intReturnCode = WebService_Settings(busWebDynamicServices, "busWebDynamicServices");

                    if (intReturnCode == 0)
                    {
                        busWebDynamicServices.DynamicFunctionCompleted += new localhost.DynamicFunctionCompletedEventHandler(DynamicFunctionCompleted);
                    }
                    else
                    {
                        if (pvtblnFromTimeSheetScheduler == false)
                        {
                            Console.WriteLine("Error Creating URL - WebService Settings");
                            Console.ReadLine();
                        }

                        WriteLog("Error Creating URL - WebService Settings ");

                        return;
                    }
                }
                else
                {
                    asAssembly = Assembly.LoadFrom("busTimeSheetDynamicUpload.dll");
                    typObjectType = asAssembly.GetType("InteractPayroll.busTimeSheetDynamicUpload");
                    busDynamicService = Activator.CreateInstance(typObjectType);
                }

#if (DEBUG)
                //Set to Test when Backend Flag is Set
                AppDomain.CurrentDomain.SetData("NewFingerPrintClockTimeAttendanceService", "Y");
#endif
                string strReturnValue = "N";

                try
                {
                    strReturnValue = (string)clsISClientUtilities.DynamicFunction("Check_If_Running_New_Service", pvtobjParm, false);
                }
                catch
                {
                    //Catch If New Method has not Yet been implemented
                }

                if (strReturnValue == "Y")
                {
                    if (pvtblnFromTimeSheetScheduler == false)
                    {
                        Console.WriteLine("Running NEW Windows Service");
                    }
#if (DEBUG)
#else

                    System.Threading.Thread.Sleep(2500);
#endif

                    pvtobjParm = new object[1];
                    pvtobjParm[0] = 0;

                    while (true)
                    {
                        pvtbytCompress = (byte[])clsISClientUtilities.DynamicFunction("ReadDbConsoleLines", pvtobjParm, false);

                        if (pvtbytCompress == null)
                        {
                            if (pvtblnFromTimeSheetScheduler == false)
                            {
                                Console.WriteLine("ERROR communicating with FingerPrintClockTimeAttendanceService. Check to see if it has been Started.");
                                Console.ReadLine();
                            }

                            WriteLog("ERROR communicating with FingerPrintClockTimeAttendanceService. Check to see if it has been Started.");

                            goto Main_ConnectionError;
                        }
                        else
                        {
                            DataSet dsDataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                            for (int intRow = 0; intRow < dsDataSet.Tables["ConsoleLine"].Rows.Count; intRow++)
                            {
                                if (pvtblnFromTimeSheetScheduler == false)
                                {
                                    Console.WriteLine(dsDataSet.Tables["ConsoleLine"].Rows[intRow]["CONSOLE_LINE_MESSAGE"].ToString());
                                }

                                System.Threading.Thread.Sleep(500);
                            }

                            pvtobjParm[0] = Convert.ToInt32(pvtobjParm[0]) + dsDataSet.Tables["ConsoleLine"].Rows.Count;

                            if (dsDataSet.Tables["ValiditeHostingService"].Rows.Count > 0)
                            {
                                if (dsDataSet.Tables["ValiditeHostingService"].Rows[0]["DYNAMIC_UPLOAD_TIMESHEETS_RUNNING_IND"].ToString() == "N")
                                {
                                    Console.ReadLine();
                                    break;
                                }
                            }

                            dsDataSet.Tables.Remove("ConsoleLine");
                            dsDataSet.Tables.Remove("ValiditeHostingService");
                        }
                    }
                }
                else
                {
                    if (pvtblnFromTimeSheetScheduler == false)
                    {
                        Console.WriteLine("Running old Windows Service");
                    }

                    fiFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "UploadKey.txt");

                    if (fiFileInfo.Exists == true)
                    {
                        int intKeyCount = 0;
                        int intCompanyNo = -1;

                        using (StreamReader srStreamReader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "UploadKey.txt"))
                        {
                            while (srStreamReader.EndOfStream != true)
                            {
                                strUploadKey[intKeyCount] = srStreamReader.ReadLine();

                                if (strUploadKey[intKeyCount].Trim() == "")
                                {
                                    continue;
                                }
                                
                                string strReturnedDBName = "";

                                try
                                {
                                    strReturnedDBName = DecryptStringAES(strUploadKey[intKeyCount]);
                                }
                                catch (Exception ex)
                                {
                                    if (pvtblnFromTimeSheetScheduler == false)
                                    {
                                        Console.WriteLine("UploadKey.txt Exception " + ex.Message);
                                        Console.ReadLine();
                                    }

                                    WriteExceptionLog("UploadKey.txt Exception", ex);

                                    return;
                                }

                                try
                                {
                                    if (strReturnedDBName.IndexOf("InteractPayroll_") > -1)
                                    {
                                        string strCompanyNo = strReturnedDBName.Substring(strReturnedDBName.IndexOf("_") + 1);

                                        intCompanyNo = Convert.ToInt32(strCompanyNo);

                                        Int64UploadKeyCompanyNo[intKeyCount] = intCompanyNo;

                                        intKeyCount += 1;
                                    }
                                }
                                catch
                                {
                                    if (pvtblnFromTimeSheetScheduler == false)
                                    {
                                        Console.WriteLine("Upload Key '" + strUploadKey[intKeyCount] + "' Not a Valid Key\nSpeak to System Administrator");
                                        Console.ReadLine();
                                    }

                                    WriteLog("Upload Key '" + strUploadKey[intKeyCount] + "' Not a Valid Key\nSpeak to System Administrator ");

                                    return;
                                }
                            }
                        }

                        RunTimerProcess();
                    }
                    else
                    {
                        if (pvtblnFromTimeSheetScheduler == false)
                        {
                            Console.WriteLine("File 'UploadKey.txt' Does NOT Exist\nSpeak to System Administrator");
                            Console.ReadLine();
                        }

                        WriteLog("File 'UploadKey.txt' Does NOT Exist\nSpeak to System Administrator ");

                        return;
                    }
                }

                DateTime myDateTime = DateTime.Now.AddSeconds(10);
#if (DEBUG)
                myDateTime = DateTime.Now.AddSeconds(40);
#endif
                if (pvtblnFromTimeSheetScheduler == false)
                {
                    while (myDateTime > DateTime.Now)
                    {

                    }
                }

            Main_ConnectionError:

                int intError = 0;
            }
            catch (Exception ex)
            {
                string strInnerExceptionMessage = "";

                if (ex.InnerException != null)
                {
                    strInnerExceptionMessage = ex.InnerException.Message;
                }

                if (pvtblnFromTimeSheetScheduler == false)
                {
                    Console.WriteLine("Exception Error = " + ex.Message + " " + strInnerExceptionMessage);
                    Console.ReadLine();
                }

                WriteExceptionLog("Main Exception", ex);
            }
            finally
            {
                if (busWebDynamicServices != null)
                {
                    try
                    {
                        busWebDynamicServices.Dispose();
                    }
                    catch
                    {
                        busWebDynamicServices.Abort();
                    }
                }

                GC.Collect();
            }
        }

        static void RunTimerProcess()
        {
            try
            {
                WriteLog("RunTimerProcess Entered");
                
                string strBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
#if (DEBUG)
                //Put Here to Stop overwrite of New Compiled Programs is Debug Directory
                strBaseDirectory = AppDomain.CurrentDomain.BaseDirectory + "bin\\";
#endif
                object myReturnObject;
                object myClientReturnObject = null;
                object myEmployeeReturnObject;
                bool blnRebootMachine = false;

                string strEmployeeRunDateWageQry = "";
                string strEmployeeRunDateSalaryQry = "";
                string strEmployeeRunDateTimeAttendanceQry = "";
                
                bool blnRunDownloadFileCheck = false;

                for (int intCount = 0; intCount < strUploadKey.Length; intCount++)
                {
                    DataSet DataSet = new System.Data.DataSet();

                    if (strUploadKey[intCount] == null)
                    {
                        break;
                    }

                    if (intCount == 0)
                    {
                        //Get Date from FILE_DOWNLOAD_CHECK_DATE
                        pvtobjParm = null;

                        pvtbytCompress = (byte[])clsISClientUtilities.DynamicFunction("DownloadCheck", pvtobjParm, false);

                        if (pvtbytCompress == null)
                        {
                            if (pvtblnFromTimeSheetScheduler == false)
                            {
                                Console.WriteLine("ERROR communicating with FingerPrintClockTimeAttendanceService. Check to see if it has been Started.");
                                Console.ReadLine();
                            }

                            WriteLog("ERROR communicating with FingerPrintClockTimeAttendanceService. Check to see if it has been Started.");

                            goto RunTimerProcess_ConnectionError;
                        }
            
                        DataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                        //Changes to Server Layer (Need to Reboot)
                        //busClientTimeSheetDynamicUpload.dll
                        //clsDBConnectionObjects.dll 
                        //FingerPrintClockService.dll
                        //FingerPrintClockTimeAttendanceService.exe

                        if (DataSet.Tables["Reboot"].Rows.Count > 0)
                        {
                            blnRebootMachine = true;
                        }

                        blnRunDownloadFileCheck = true;
                                               
                        if (DataSet.Tables["DownloadCheck"].Rows.Count > 0)
                        {
                            if (DataSet.Tables["DownloadCheck"].Rows[0]["FILE_DOWNLOAD_CHECK_DATE"] != System.DBNull.Value)
                            {
                                if (Convert.ToDateTime(DataSet.Tables["DownloadCheck"].Rows[0]["FILE_DOWNLOAD_CHECK_DATE"]).ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd"))
                                {
                                    blnRunDownloadFileCheck = false;
                                }
                            }
                        }
                                              
                        if (blnRunDownloadFileCheck == true)
                        {
                            if (pvtblnFromTimeSheetScheduler == false)
                            {
                                Console.WriteLine("Checking for Files to Download");
                            }

                            WriteLog("Checking for Files to Download");

                            //Get All Files For Download From Internet
                            byte[] byteDecompressArray = (byte[])DynamicFunction("Get_Download_Files_Details", null);

                            DataSet DataSetFileInternetDownload = clsISClientUtilities.DeCompress_Array_To_DataSet(byteDecompressArray);

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
                                    pvtobjParm = new object[2];
                                    pvtobjParm[0] = myClientPresentationObjects[intRow]["FILE_LAYER_IND"].ToString();
                                    pvtobjParm[1] = myClientPresentationObjects[intRow]["FILE_NAME"].ToString();

                                    myClientReturnObject = clsISClientUtilities.DynamicFunction("DeleteFile", pvtobjParm, false);

                                    if (myClientReturnObject == null)
                                    {
                                        if (pvtblnFromTimeSheetScheduler == false)
                                        {
                                            Console.WriteLine("ERROR with clsISClientUtilities 'DeleteFile'.");
                                            Console.ReadLine();
                                        }

                                        WriteLog("ERROR with clsISClientUtilities 'DeleteFile'");

                                        goto RunTimerProcess_ConnectionError;
                                    }

                                    myClientPresentationObjects[intRow].Delete();

                                    intRow -= 1;
                                }
                            }

                            DataSet.Tables["ClientFile"].AcceptChanges();

                            //Check if Download Files Exist On Client Database
                            for (int intRow = 0; intRow < DataSetFileInternetDownload.Tables["Files"].Rows.Count; intRow++)
                            {
#if (DEBUG)
                                string myFileName = DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();

                                if (myFileName == "FingerPrintClockServiceStartStop.dll")
                                {
                                    string strStop = "";
                                }
#endif
                                DataView DataView = new DataView(DataSet.Tables["ClientFile"],
                                "FILE_LAYER_IND = '" + DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_LAYER_IND"].ToString() + "' AND FILE_NAME = '" + DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() + "'",
                                "",
                                DataViewRowState.CurrentRows);

                                if (DataView.Count == 0)
                                {
                                    if (DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_LAYER_IND"].ToString() == "P")
                                    {
                                        int intReturnCode = Check_If_Possible_To_Load_File_From_Disk(DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString(),
                                                                                                     DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_CRC_VALUE"].ToString(),
                                                                                                     Convert.ToDateTime(DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]),
                                                                                                     Convert.ToInt32(DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_SIZE"]),
                                                                                                     Convert.ToInt32(DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_SIZE_COMPRESSED"]));

                                        if (intReturnCode == 0
                                        || intReturnCode == 9)
                                        {
                                            DataSetFileInternetDownload.Tables["Files"].Rows[intRow].Delete();

                                            if (intReturnCode == 9)
                                            {
                                                blnRebootMachine = true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (Convert.ToDateTime(DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]) >= Convert.ToDateTime(DataView[0]["FILE_LAST_UPDATED_DATE"]).AddSeconds(-3)
                                    && Convert.ToDateTime(DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]) <= Convert.ToDateTime(DataView[0]["FILE_LAST_UPDATED_DATE"]).AddSeconds(3))
                                    {
                                        DataSetFileInternetDownload.Tables["Files"].Rows[intRow].Delete();
                                    }
                                    else
                                    {
                                        if (DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_LAYER_IND"].ToString() == "P")
                                        {
                                            int intReturnCode = Check_If_Possible_To_Load_File_From_Disk(DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString(),
                                                                                                         DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_CRC_VALUE"].ToString(),
                                                                                                         Convert.ToDateTime(DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]),
                                                                                                         Convert.ToInt32(DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_SIZE"]),
                                                                                                         Convert.ToInt32(DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_SIZE_COMPRESSED"]));

                                            if (intReturnCode == 0
                                                | intReturnCode == 9)
                                            {
                                                DataSetFileInternetDownload.Tables["Files"].Rows[intRow].Delete();

                                                if (intReturnCode == 9)
                                                {
                                                    blnRebootMachine = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            //Remove From DataTable where Records Exist On Client Database
                            DataSetFileInternetDownload.AcceptChanges();

                            bool blnComplete = false;

                            if (DataSetFileInternetDownload.Tables["Files"].Rows.Count == 0)
                            {
                                if (pvtblnFromTimeSheetScheduler == false)
                                {
                                    Console.WriteLine("No Files to Download");
                                }

                                WriteLog("No Files to Download");
                            }

                            //Download Files to Client Database
                            for (int intRow = 0; intRow < DataSetFileInternetDownload.Tables["Files"].Rows.Count; intRow++)
                            {
                                if (pvtblnFromTimeSheetScheduler == false)
                                {
                                    Console.WriteLine("Downloading File '" + DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() + "'");
                                }

                                WriteLog("Downloading File '" + DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() + "'");
#if (DEBUG)
                                string myFileName = DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();

                                if (myFileName == "clsDBConnectionObjects.dll")
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
                                    object[] objParm = new object[3];
                                    objParm[0] = DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_LAYER_IND"].ToString();
                                    objParm[1] = DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();
                                    objParm[2] = intRow1;

                                    myReturnObject = DynamicFunction("Get_File_Chunk", objParm);

                                    if (myReturnObject == null)
                                    {
                                        if (pvtblnFromTimeSheetScheduler == false)
                                        {
                                            Console.WriteLine("ERROR with Internet 'Get_File_Chunk'");
                                            Console.ReadLine();
                                        }

                                        WriteLog("ERROR with Internet 'Get_File_Chunk'");

                                        goto RunTimerProcess_ConnectionError;
                                    }

                                    //Insert Chunk into Client Database
                                    pvtobjParm = new object[9];
                                    pvtobjParm[0] = DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_LAYER_IND"].ToString();
                                    pvtobjParm[1] = DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();
                                    pvtobjParm[2] = intRow1;
                                    pvtobjParm[3] = (byte[])myReturnObject;
                                    pvtobjParm[4] = blnComplete;
                                    pvtobjParm[5] = DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_CRC_VALUE"].ToString();
                                    pvtobjParm[6] = Convert.ToDateTime(DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]);
                                    pvtobjParm[7] = Convert.ToInt32(DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_SIZE"]);
                                    pvtobjParm[8] = Convert.ToInt32(DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_SIZE_COMPRESSED"]);

                                    myClientReturnObject = clsISClientUtilities.DynamicFunction("InsertChunk", pvtobjParm, false);

                                    if (myClientReturnObject == null)
                                    {
                                        if (pvtblnFromTimeSheetScheduler == false)
                                        {
                                            Console.WriteLine("ERROR with clsISClientUtilities 'InsertChunk'");
                                            Console.ReadLine();
                                        }

                                        WriteLog("ERROR with clsISClientUtilities 'InsertChunk'");

                                        goto RunTimerProcess_ConnectionError;
                                    }

                                    if (pvtblnFromTimeSheetScheduler == false)
                                    {
                                        Console.WriteLine("Block " + intRow1.ToString() + " of " + DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["MAX_FILE_CHUNK_NO"].ToString());
                                    }

                                    WriteLog("Block " + intRow1.ToString() + " of " + DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["MAX_FILE_CHUNK_NO"].ToString());
                                }

                                int intReturnCode = (int)myClientReturnObject;

                                //CRC Check Correct
                                if (intReturnCode == 0
                                | intReturnCode == 9)
                                {
                                    if (pvtblnFromTimeSheetScheduler == false)
                                    {
                                        Console.WriteLine("File '" + DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() + "' Downloaded Successful.\n");
                                    }

                                    WriteLog("File '" + DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() + "' Downloaded Successful");

                                    if (intReturnCode == 9)
                                    {
                                        blnRebootMachine = true;
                                    }
                                }
                                else
                                {
                                    if (pvtblnFromTimeSheetScheduler == false)
                                    {
                                        Console.WriteLine("File '" + DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() + "' CRC ERROR\n\nDownload UNSUCCESSFUL.\n");
                                    }

                                    WriteLog("File '" + DataSetFileInternetDownload.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() + "' CRC ERROR. Download UNSUCCESSFUL.");
                                }
                            }

                            //Update Day Checked Timestamp
                            pvtobjParm = new object[1];
                            pvtobjParm[0] = DataSet.Tables["DownloadCheck"].Rows.Count;

                            myClientReturnObject = clsISClientUtilities.DynamicFunction("UpdateDateTimeCheck", pvtobjParm, false);

                            if (myClientReturnObject == null)
                            {
                                if (pvtblnFromTimeSheetScheduler == false)
                                {
                                    Console.WriteLine("ERROR with clsISClientUtilities 'UpdateDateTimeCheck'");
                                    Console.ReadLine();
                                }

                                WriteLog("ERROR with clsISClientUtilities 'UpdateDateTimeCheck'");

                                goto RunTimerProcess_ConnectionError;
                            }
                        }
                    }

                    if (blnRebootMachine == true)
                    {
                        //Try Restart Service
                        int intMyReturnCode = 1;

                        pvtobjParm = null;

                        try
                        {
                            myClientReturnObject = clsISClientUtilities.DynamicFunction("Get_MachineName_And_MachineIP", pvtobjParm, false);
                        }
                        catch (Exception ex)
                        {
                            string strInnerExceptionMessage = "";

                            if (ex.InnerException != null)
                            {
                                strInnerExceptionMessage = ex.InnerException.Message;
                            }

                            if (pvtblnFromTimeSheetScheduler == false)
                            {
                                Console.WriteLine("Get_MachineName_And_MachineIP Exception = " + ex.Message + " " + strInnerExceptionMessage);
                                Console.ReadLine();
                            }

                            WriteExceptionLog("Get_MachineName_And_MachineIP Exception",ex);
                        }

                        if (myClientReturnObject == null)
                        {
                            if (pvtblnFromTimeSheetScheduler == false)
                            {
                                Console.WriteLine("ERROR with clsISClientUtilities 'Get_MachineName_And_MachineIP'");
                                Console.ReadLine();
                            }

                            WriteLog("ERROR with clsISClientUtilities 'Get_MachineName_And_MachineIP' ");

                            goto RunTimerProcess_ConnectionError;
                        }
                        else
                        {
                            string[] strMachineParts = Convert.ToString(myClientReturnObject).Split(',');

                            if (strMachineParts.Length == 2)
                            {
                                //Try Restart Service
                                intMyReturnCode = Restart_FingerPrintClockTimeAttendanceService(strMachineParts[0], strMachineParts[1]);
                                //intMyReturnCode = Restart_FingerPrintClockTimeAttendanceService_New(strMachineParts[0], strMachineParts[1]);

                                if (intMyReturnCode != 0)
                                {
                                    if (pvtblnFromTimeSheetScheduler == false)
                                    {
                                        Console.WriteLine("ERROR Restarting FingerPrintClockTimeAttendanceService - See Logs");
                                        Console.ReadLine();
                                    }

                                    WriteLog("ERROR Restarting FingerPrintClockTimeAttendanceService - See Logs");

                                    goto RunTimerProcess_ConnectionError;
                                }
                                else
                                {
                                    if (pvtblnFromTimeSheetScheduler == false)
                                    {
                                        Console.WriteLine("Restarted FingerPrintClockTimeAttendanceService SUCCESSFULLY");
                                        Console.ReadLine();
                                    }

                                    WriteLog("Restarted FingerPrintClockTimeAttendanceService SUCCESSFULLY **********");
                                }
                            }
                            else
                            {
                                if (pvtblnFromTimeSheetScheduler == false)
                                {
                                    Console.WriteLine("ERROR with strMachineParts - See Logs");
                                    Console.ReadLine();
                                }

                                WriteLog("ERROR with MachineParts - See Logs");

                                goto RunTimerProcess_ConnectionError;
                            }
                        }

                        if (intMyReturnCode == 0)
                        {
                            if (pvtblnFromTimeSheetScheduler == false)
                            {
                                Console.WriteLine("FingerPrintClockTimeAttendanceService has been Restarted");
                                Console.WriteLine("");
                            }

                            WriteLog("FingerPrintClockTimeAttendanceService has been Restarted **********");

                            blnRebootMachine = false;
                        }

                        if (blnRebootMachine == true)
                        {
                            if (pvtblnFromTimeSheetScheduler == false)
                            {
                                Console.WriteLine("Changes Have been made to Local Web Server Layer.\n\nReboot to Continue");
                                Console.WriteLine("");
                            }

                            WriteLog("Changes Have been made to Local Web Server Layer.Windows Service Restart NEEDED to Continue");

                            goto RunTimerProcess_ConnectionError;
                        }
                    }

                    if (DataSet.Tables["Date"] != null)
                    {
                        DataSet.Tables.Remove("Date");
                    }

                    //Find Dates for Upload
                    pvtobjParm = new object[1];
                    pvtobjParm[0] = Int64UploadKeyCompanyNo[intCount];

                    pvtbytCompress = (byte[])clsISClientUtilities.DynamicFunction("GetDates", pvtobjParm, false);

                    DataSet Temp1DataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                    for (int intRow = 0; intRow < Temp1DataSet.Tables["UploadCheck"].Rows.Count; intRow++)
                    {
                        if (pvtblnFromTimeSheetScheduler == false)
                        {
                            Console.WriteLine("**** Cost Centre '" + Temp1DataSet.Tables["UploadCheck"].Rows[intRow]["PAY_CATEGORY_DESC"].ToString() + " (" + Temp1DataSet.Tables["UploadCheck"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() + ")' Exists but is Not flagged to be Uploaded.");
                        }

                        WriteLog("Cost Centre '" + Temp1DataSet.Tables["UploadCheck"].Rows[intRow]["PAY_CATEGORY_DESC"].ToString() + " (" + Temp1DataSet.Tables["UploadCheck"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() + ")' Exists but is Not flagged to be Uploaded.");
                    }

                    DataSet.Merge(Temp1DataSet);

                    if (DataSet.Tables["Date"].Rows.Count == 0)
                    {
                        if (pvtblnFromTimeSheetScheduler == false)
                        {
                            Console.WriteLine("No Timesheet / Break Records Exist For Upload of Company Number = " + Int64UploadKeyCompanyNo[intCount] + "\n");
                        }

                        WriteLog("No Timesheet / Break Records Exist For Upload of Company Number = " + Int64UploadKeyCompanyNo[intCount]);

                        //Find Cost Centres for Upload
                        pvtobjParm = new object[1];
                        pvtobjParm[0] = Int64UploadKeyCompanyNo[intCount];

                        pvtbytCompress = (byte[])clsISClientUtilities.DynamicFunction("GetCostCentres", pvtobjParm, false);

                        if (pvtbytCompress == null)
                        {
                            if (pvtblnFromTimeSheetScheduler == false)
                            {
                                Console.WriteLine("ERROR Calling GetCostCentres for Company Number = " + Int64UploadKeyCompanyNo[intCount] + "\n");
                            }

                            WriteLog("ERROR Calling GetCostCentres for Company Number = " + Int64UploadKeyCompanyNo[intCount]);
                        }
                        else
                        {
                            if (blnRunDownloadFileCheck == true)
                            {
                                WriteLog("Before Set_UploadDateTime_For_Company = " + Int64UploadKeyCompanyNo[intCount]);

                                pvtobjParm = new object[2];
                                pvtobjParm[0] = Int64UploadKeyCompanyNo[intCount];
                                pvtobjParm[1] = pvtbytCompress;

                                //Set UploadDatetime for Company on Internet
                                int intRecordsUpdated = (int)DynamicFunction("Set_UploadDateTime_For_Company", pvtobjParm);

                                WriteLog("After Set_UploadDateTime_For_Company = " + Int64UploadKeyCompanyNo[intCount].ToString() + " Records Updated = " + intRecordsUpdated);
                            }
                        }
                    }
                    else
                    {
                        if (pvtblnFromTimeSheetScheduler == false)
                        {
                            Console.WriteLine("Start of Upload Process...\n");
                        }

                        WriteLog("Start of Upload Process...");

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
                            pvtobjParm = new object[1];
                            pvtobjParm[0] = Int64UploadKeyCompanyNo[intCount];

                            pvtbytCompress = (byte[])clsISClientUtilities.DynamicFunction("GetPayCategoryRecords", pvtobjParm, false);

                            DataSet Temp2DataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

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
                            pvtobjParm = new object[2];
                            pvtobjParm[0] = Int64UploadKeyCompanyNo[intCount];
                            pvtobjParm[1] = Convert.ToDateTime(DataSet.Tables["Date"].Rows[intRow]["TIMESHEET_DATE"]);

                            myClientReturnObject = clsISClientUtilities.DynamicFunction("GetUploadRecords", pvtobjParm, false);

                            if (myClientReturnObject == null)
                            {
                                if (pvtblnFromTimeSheetScheduler == false)
                                {
                                    Console.WriteLine("ERROR with clsISClientUtilities 'GetUploadRecords'");
                                    Console.ReadLine();
                                }

                                WriteLog("ERROR with clsISClientUtilities 'GetUploadRecords'");

                                goto RunTimerProcess_ConnectionError;
                            }

                            string strURLConfigDateTime = "";

                            FileInfo FileInfo = new FileInfo(strBaseDirectory + "URLConfig.txt");

                            if (FileInfo.Exists == true)
                            {
                                strURLConfigDateTime = FileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
                            }

                            //Insert Timesheet Records for Specific Date to Internet Site
                            object[] objParm = new object[7];
                            objParm[0] = Int64UploadKeyCompanyNo[intCount];
                            objParm[1] = strURLConfigDateTime;
                            objParm[2] = strUploadKey[intCount];
                            objParm[3] = strEmployeeRunDateWageQry;
                            objParm[4] = strEmployeeRunDateSalaryQry;
                            objParm[5] = strEmployeeRunDateTimeAttendanceQry;
                            objParm[6] = (byte[])myClientReturnObject;

                            myEmployeeReturnObject = DynamicFunction("Upload_Timesheet_Break_For_Day", objParm);

                            if (myEmployeeReturnObject == null)
                            {
                                if (pvtblnFromTimeSheetScheduler == false)
                                {
                                    Console.WriteLine("ERROR with Internet 'Upload_Timesheet_Break_For_Day'");
                                    Console.ReadLine();
                                }

                                WriteLog("ERROR with Internet 'Upload_Timesheet_Break_For_Day'");

                                goto RunTimerProcess_ConnectionError;
                            }

                            byte[] byteDecompressArray = (byte[])myEmployeeReturnObject;

                            DataSet parDataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(byteDecompressArray);

                            if (parDataSet.Tables["Company"].Rows.Count > 0)
                            {
                                if (parDataSet.Tables["Company"].Rows[0]["URLCONFIG_CHANGE_IND"].ToString() == "")
                                {
                                    //upDate Local Site Records For Day
                                    pvtobjParm = new object[5];
                                    pvtobjParm[0] = Int64UploadKeyCompanyNo[intCount];
                                    pvtobjParm[1] = Convert.ToDateTime(DataSet.Tables["Date"].Rows[intRow]["TIMESHEET_DATE"]);
                                    pvtobjParm[2] = Convert.ToDateTime(parDataSet.Tables["Company"].Rows[0]["UPLOAD_DATETIME"]);
                                    pvtobjParm[3] = (byte[])myEmployeeReturnObject;
                                    pvtobjParm[4] = (byte[])myClientReturnObject;

                                    myClientReturnObject = clsISClientUtilities.DynamicFunction("UpdateLocalRecords", pvtobjParm, false);

                                    if (myClientReturnObject == null)
                                    {
                                        if (pvtblnFromTimeSheetScheduler == false)
                                        {
                                            Console.WriteLine("ERROR with clsISClientUtilities 'UpdateLocalRecords'");
                                            Console.ReadLine();
                                        }

                                        WriteLog("ERROR with clsISClientUtilities 'UpdateLocalRecords'");

                                        goto RunTimerProcess_ConnectionError;
                                    }

                                    if (pvtblnFromTimeSheetScheduler == false)
                                    {
                                        Console.WriteLine("Timesheets/Breaks Upload for Company Number = " + Int64UploadKeyCompanyNo[intCount] + " and Date = '" + Convert.ToDateTime(DataSet.Tables["Date"].Rows[intRow]["TIMESHEET_DATE"]).ToString("dd MMM yyyy") + "' SUCCESSFUL\n");
                                    }

                                    WriteLog("Timesheets/Breaks Upload for Company Number = " + Int64UploadKeyCompanyNo[intCount] + " and Date = '" + Convert.ToDateTime(DataSet.Tables["Date"].Rows[intRow]["TIMESHEET_DATE"]).ToString("dd MMM yyyy") + "' SUCCESSFUL");
                                }
                                else
                                {
                                    pvtobjParm = null;

                                    clsISClientUtilities.DynamicFunction("ResetDateTimeCheck", pvtobjParm, false);

                                    if (pvtblnFromTimeSheetScheduler == false)
                                    {
                                        Console.WriteLine("URLConfig.txt FILE has Changed\nDateTimeCheck Has been Reset to Activate Download.\n\n");
                                        Console.ReadLine();
                                    }

                                    WriteLog("URLConfig.txt FILE has Changed\nDateTimeCheck Has been Reset to Activate Download");

                                    goto RunTimerProcess_ConnectionError;
                                }
                            }
                            else
                            {
                                if (pvtblnFromTimeSheetScheduler == false)
                                {
                                    if (pvtblnFromTimeSheetScheduler == false)
                                    {
                                        Console.WriteLine("'" + strUploadKey[intCount] + "' Upload Key Not Authorised to Run.\nSpeak to System Administrator\n");
                                        Console.ReadLine();
                                    }
                                }

                                WriteLog("Upload Key Not Authorised to Run. Speak to System Administrator");

                                return;
                            }
                        }
                    }
                }

            RunTimerProcess_ConnectionError:
                int intError = 1;
            }
            catch (Exception ex)
            {
                string strInnerExceptionMessage = "";

                if (ex.InnerException != null)
                {
                    strInnerExceptionMessage = ex.InnerException.Message;
                }

                if (pvtblnFromTimeSheetScheduler == false)
                {
                    Console.WriteLine(ex.Message + " " + strInnerExceptionMessage);
                    Console.ReadLine();
                }

                WriteExceptionLog("RunTimerProcess Exception = ",ex);
            }
            finally
            {
                WriteLog("RunTimerProcess Exit");

                try
                {
                    //Write Blank Line - Seperates Messages
                    using (StreamWriter writeLog = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + pvtstrLogFileName, true))
                    {
                        writeLog.WriteLine("");
                    }
                }
                catch
                {
                }
            }
        }

        static int Check_If_Possible_To_Load_File_From_Disk(string parstrFileName, string parstrFileCRCValue, DateTime pardtFileLastUpdateDateTime, int parintFileSize, int parintFileSizeCompressed)
        {
            int intReturnCode = 1;

            try
            {
                object myClientReturnObject = null;

                
                string strBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

#if (DEBUG)
                strBaseDirectory += "bin\\";
#endif
                FileInfo fiFileInfo = new FileInfo(strBaseDirectory + parstrFileName);

                bool blnComplete = false;

                if (fiFileInfo.Exists == true)
                {
                    if (fiFileInfo.LastWriteTime >= pardtFileLastUpdateDateTime.AddSeconds(-3)
                    && fiFileInfo.LastWriteTime <= pardtFileLastUpdateDateTime.AddSeconds(3))
                    {
                        if (pvtblnFromTimeSheetScheduler == false)
                        {
                            Console.WriteLine("File '" + parstrFileName + "' Writing from " + strBaseDirectory + "' Directory");
                        }

                        //Load From Live File
                        int intNumberOfBytesToRead = 10000;

                        byte[] pvtbytes = new byte[intNumberOfBytesToRead];

                        FileStream pvtfsFileStream = new FileStream(strBaseDirectory + parstrFileName, FileMode.Open, FileAccess.Read);

                        //Read FileStream To Bytes Array
                        byte[] ByteArray = new byte[pvtfsFileStream.Length];
                        pvtfsFileStream.Read(ByteArray, 0, Convert.ToInt32(pvtfsFileStream.Length));

                        //Open memory stream (Compressed)
                        MemoryStream msMemoryStream = new MemoryStream();

                        System.IO.Compression.GZipStream GZipStreamCompressed = new GZipStream(msMemoryStream, CompressionMode.Compress, true);
                        GZipStreamCompressed.Write(ByteArray, 0, (int)ByteArray.Length);
                        GZipStreamCompressed.Flush();
                        GZipStreamCompressed.Close();

                        //This is The File Length
                        int intNumberBlocks = Convert.ToInt32(msMemoryStream.Length / intNumberOfBytesToRead);
                        int intNumberBytesAlreadyRead = 0;
                        int intNumberBytesRead = 0;

                        if (intNumberBlocks * intNumberOfBytesToRead != msMemoryStream.Length)
                        {
                            intNumberBlocks += 1;
                        }

                        BinaryReader pvtbrBinaryReader = new BinaryReader(msMemoryStream);

                        for (int intBlockNumber = 1; intBlockNumber <= intNumberBlocks; intBlockNumber++)
                        {
                            if (intBlockNumber == intNumberBlocks)
                            {
                                intNumberOfBytesToRead = Convert.ToInt32(msMemoryStream.Length - intNumberBytesAlreadyRead);

                                pvtbytes = null;
                                pvtbytes = new byte[intNumberOfBytesToRead];

                                blnComplete = true;
                            }
                            else
                            {
                                blnComplete = false;
                            }

                            pvtbrBinaryReader.BaseStream.Position = intNumberBytesAlreadyRead;

                            intNumberBytesRead = pvtbrBinaryReader.Read(pvtbytes, 0, intNumberOfBytesToRead);

                            //Insert Chunk into Client Database
                            pvtobjParm = new object[9];
                            pvtobjParm[0] = "P";
                            pvtobjParm[1] = parstrFileName;
                            pvtobjParm[2] = intBlockNumber;
                            pvtobjParm[3] = pvtbytes;

                            pvtobjParm[4] = blnComplete;
                            pvtobjParm[5] = parstrFileCRCValue;
                            pvtobjParm[6] = pardtFileLastUpdateDateTime;
                            pvtobjParm[7] = parintFileSize;
                            pvtobjParm[8] = parintFileSizeCompressed;
                               
                            myClientReturnObject = clsISClientUtilities.DynamicFunction("InsertChunk", pvtobjParm, false);

                            if (myClientReturnObject == null)
                            {
                                if (pvtblnFromTimeSheetScheduler == false)
                                {
                                    Console.WriteLine("ERROR with clsISClientUtilities 'InsertChunk'");
                                    Console.ReadLine();
                                }
                                break;
                            }

                            intNumberBytesAlreadyRead += intNumberBytesRead;

                            if (pvtblnFromTimeSheetScheduler == false)
                            {
                                Console.WriteLine("Block " + intBlockNumber.ToString() + " of " + intNumberBlocks.ToString());
                            }
                        }

                        if (myClientReturnObject == null)
                        {
                            if (pvtblnFromTimeSheetScheduler == false)
                            {
                                Console.WriteLine("ERROR with clsISClientUtilities 'InsertFile'");
                                Console.ReadLine();
                            }
                        }
                        else
                        {
                            intReturnCode = (int)myClientReturnObject;
                        }

                        pvtfsFileStream.Close();
                        msMemoryStream.Close();
                        pvtbrBinaryReader.Close();

                        pvtfsFileStream = null;
                        msMemoryStream = null;
                        pvtbrBinaryReader = null;
                    }
                }
            }
            catch (Exception ex)
            {
                string strInnerExceptionMessage = "";

                if (ex.InnerException != null)
                {
                    strInnerExceptionMessage = ex.InnerException.Message;
                }

                if (pvtblnFromTimeSheetScheduler == false)
                {
                    Console.WriteLine("Check_If_Possible_To_Load_File_From_Disk Error = " + ex.Message + " " + strInnerExceptionMessage);
                    Console.ReadLine();
                }

                WriteExceptionLog("Check_If_Possible_To_Load_File_From_Disk Exception = ", ex);
            }

            return intReturnCode;
        }

        static int Restart_FingerPrintClockTimeAttendanceService(string parstrMachineName, string parstrMachineIP)
        {
            if (pvtblnFromTimeSheetScheduler == false)
            {
                Console.WriteLine("FingerPrintClockTimeAttendanceService Needs to be Stopped");
            }

            int intReturnCode = 1;
            
            try
            {
                if (AppDomain.CurrentDomain.GetData("URLClientPath").ToString() == "")
                {
                    FingerPrintClockServer.Restart Restart = (FingerPrintClockServer.Restart)clsRestartFingerPrintClockTimeAttendanceService.DynamicFunction("RestartFingerPrintClockTimeAttendanceService", null);
                  
                    if (Restart.OK == "Y")
                    {
                        intReturnCode = 0;
                    }
                }
                else
                {
                    InteractPayrollClient.Restart Restart = (InteractPayrollClient.Restart)clsRestartFingerPrintClockTimeAttendanceService.DynamicFunction("RestartFingerPrintClockTimeAttendanceService", null);
                   
                    if (Restart.OK == "Y")
                    {
                        intReturnCode = 0;
                    }
                }
#if (DEBUG)
                if (intReturnCode == 0)
                {
                    string strBaseDirectory = AppDomain.CurrentDomain.BaseDirectory + "bin\\";

                    FileInfo fiFileInfo = new FileInfo(strBaseDirectory + "busClientTimeSheetDynamicUpload.dll_");

                    if (fiFileInfo.Exists == true)
                    {
                        File.Delete(strBaseDirectory + "busClientTimeSheetDynamicUpload.dll");

                        File.Move(strBaseDirectory + "busClientTimeSheetDynamicUpload.dll_", strBaseDirectory + "\\busClientTimeSheetDynamicUpload.dll");
                    }

                    fiFileInfo = new FileInfo(strBaseDirectory + "clsDBConnectionObjects.dll_");

                    if (fiFileInfo.Exists == true)
                    {
                        File.Delete(strBaseDirectory + "clsDBConnectionObjects.dll");

                        File.Move(strBaseDirectory + "clsDBConnectionObjects.dll_", strBaseDirectory + "\\clsDBConnectionObjects.dll");
                    }
                }
#endif
            }
            catch
            {
            }

            return intReturnCode;
        }

        static int Restart_FingerPrintClockTimeAttendanceService_New(string parstrMachineName, string parstrMachineIP)
        {
            if (pvtblnFromTimeSheetScheduler == false)
            {
                Console.WriteLine("FingerPrintClockTimeAttendanceService Needs to be Stopped");
            }

            int intReturnCode = 1;

            try
            {

                if (AppDomain.CurrentDomain.GetData("URLClientPath").ToString() == "")
                {
                    FingerPrintClockServer.RestartFingerPrintClockTimeAttendanceServiceNewResponse restartFingerPrintClockTimeAttendanceServiceNewResponse = (FingerPrintClockServer.RestartFingerPrintClockTimeAttendanceServiceNewResponse)clsRestartFingerPrintClockTimeAttendanceService.DynamicFunction("RestartFingerPrintClockTimeAttendanceServiceNew", null);

                    if (restartFingerPrintClockTimeAttendanceServiceNewResponse.FingerPrintClockTimeAttendanceServiceStarted == true)
                    {
                        intReturnCode = 0;
                    }
                }
                else
                {
                    FingerPrintClockServer.RestartFingerPrintClockTimeAttendanceServiceNewResponse restartFingerPrintClockTimeAttendanceServiceNewResponse = (FingerPrintClockServer.RestartFingerPrintClockTimeAttendanceServiceNewResponse)clsRestartFingerPrintClockTimeAttendanceService.DynamicFunction("RestartFingerPrintClockTimeAttendanceServiceNew", null);

                    if (restartFingerPrintClockTimeAttendanceServiceNewResponse.FingerPrintClockTimeAttendanceServiceStarted == true)
                    {
                        intReturnCode = 0;
                    }
                }
#if (DEBUG)
                if (intReturnCode == 0)
                {
                    string strBaseDirectory = AppDomain.CurrentDomain.BaseDirectory + "bin\\";

                    FileInfo fiFileInfo = new FileInfo(strBaseDirectory + "busClientTimeSheetDynamicUpload.dll_");

                    if (fiFileInfo.Exists == true)
                    {
                        File.Delete(strBaseDirectory + "busClientTimeSheetDynamicUpload.dll");

                        File.Move(strBaseDirectory + "busClientTimeSheetDynamicUpload.dll_", strBaseDirectory + "\\busClientTimeSheetDynamicUpload.dll");
                    }

                    fiFileInfo = new FileInfo(strBaseDirectory + "clsDBConnectionObjects.dll_");

                    if (fiFileInfo.Exists == true)
                    {
                        File.Delete(strBaseDirectory + "clsDBConnectionObjects.dll");

                        File.Move(strBaseDirectory + "clsDBConnectionObjects.dll_", strBaseDirectory + "\\clsDBConnectionObjects.dll");
                    }
                }
#endif
            }
            catch
            {
            }

            return intReturnCode;
        }

        static void DynamicFunctionCompleted(object sender, localhost.DynamicFunctionCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                pvtReturnObject = e.Result;
            }
            else
            {
                pvtReturnObject = null;
                if (pvtblnFromTimeSheetScheduler == false)
                {
                    Console.WriteLine("Connection Failure " + e.Error.Message);
                    Console.ReadLine();
                }
            }

            pvtblnCallBackComplete = true;
        }

        static object DynamicFunction(string FunctionName, object[] objParm)
        {
            try
            {
                if (AppDomain.CurrentDomain.GetData("URLPath").ToString() != "")
                {
                    busWebDynamicServices.DynamicFunctionAsync("busTimeSheetDynamicUpload", FunctionName, objParm);

                    pvtblnCallBackComplete = false;
                    //Loop Until Call Completed From Web Server
                    while (pvtblnCallBackComplete == false)
                    {
                    }
                }
                else
                {
                    pvtReturnObject = null;
                    MethodInfo mi = typObjectType.GetMethod(FunctionName);
                    pvtReturnObject = mi.Invoke(busDynamicService, objParm);
                }

                return pvtReturnObject;
            }
            catch (System.Net.WebException we)
            {
                return null;

            }
            catch (System.Net.Sockets.SocketException ex)
            {
                //MessageBox.Show("Connection Failure", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return null;
            }
            catch (Exception ex)
            {
                string strInnerExceptionMessage = "";

                if (ex.InnerException != null)
                {
                    strInnerExceptionMessage = ex.InnerException.Message;
                }

                string strExceptionError = "";

                if (ex.Message.IndexOf("Server Unavailable") > -1)
                {
                    strExceptionError = "Connection to Web Server Could NOT be established " + strInnerExceptionMessage;

                    WriteLog("Connection to Web Server Could NOT be established " + strExceptionError);
                }
                else
                {
                    strExceptionError = ex.Message + " " + strInnerExceptionMessage;

                    strExceptionError += "\n\n" + "Where : " + FunctionName;

                    WriteLog(strExceptionError);
                }

                return null;
            }
        }

        static int WebService_Settings(System.Web.Services.Protocols.WebClientProtocol ws, string parWebServiceName)
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

        private static string EncryptStringAES(string plainText)
        {
            //Encrypted string to return 
            string outStr = null;
            RijndaelManaged aesAlg = null;

            //Generate the key from the shared secret and the salt 
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

            //Create a RijndaelManaged object with the specified key and IV. 
            aesAlg = new RijndaelManaged();
            aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
            aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

            //Create a decryptor to perform the stream transform. 
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption. 
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {

                        //Write all data to the stream. 
                        swEncrypt.Write(plainText);
                    }
                }
                outStr = Convert.ToBase64String(msEncrypt.ToArray());
            }

            aesAlg.Clear();

            return outStr;
        }

        private static string DecryptStringAES(string cipherText)
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

        static void Check_Log_For_Archiving()
        {
            bool blnMoveFile = false;
            DateTime myFileDateTime = DateTime.Now;

            FileInfo myFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + pvtstrLogFileName);

            if (myFileInfo.Exists == true)
            {
                using (StreamReader srStreamReader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + pvtstrLogFileName))
                {
                    string strLine = "";

                    try
                    {
                        strLine = srStreamReader.ReadLine();

                        myFileDateTime = DateTime.ParseExact(strLine.Substring(0, 10), "yyyy-MM-dd", null);

                        if (myFileDateTime.ToString("yyyyMMdd") != DateTime.Now.ToString("yyyyMMdd"))
                        {
                            blnMoveFile = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteExceptionLog("Check_Log_For_Archiving line = " + strLine, ex);
                    }
                }

                if (blnMoveFile == true)
                {
                    try
                    {
                        string strFileDirectory = AppDomain.CurrentDomain.BaseDirectory + "Backup";

                        if (Directory.Exists(strFileDirectory))
                        {
                        }
                        else
                        {
                            Directory.CreateDirectory(strFileDirectory);
                        }

                        File.Copy(AppDomain.CurrentDomain.BaseDirectory + pvtstrLogFileName, AppDomain.CurrentDomain.BaseDirectory + "Backup\\TimeSheetDynamicUploadIS_" + myFileDateTime.ToString("yyyyMMdd") + "_Log.txt", true);
                        File.Delete(AppDomain.CurrentDomain.BaseDirectory + pvtstrLogFileName);

                        //Delete Files Older than 10 Days
                        //Cleanup Daily Backup Files Older than 10 Days
                        string[] strDbBackupFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "Backup", "TimeSheetDynamicUploadIS_*_Log.txt");

                        foreach (string file in strDbBackupFiles)
                        {
                            int intOffset = file.IndexOf("TimeSheetDynamicUploadIS_");

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
                                catch
                                {
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteExceptionLog("Check_Log_For_Archiving", ex);
                    }
                }
            }
        }

        static void WriteExceptionLog(string Message, Exception ex)
        {
            string strInnerExceptionMessage = "";

            if (ex.InnerException != null)
            {
                strInnerExceptionMessage = ex.InnerException.Message;
            }

            WriteLog(Message + " ********** Exception = " + ex.Message + " " + strInnerExceptionMessage);
        }

        static void WriteLog(string Message)
        {
            try
            {
                Check_Log_For_Archiving();
            }
            catch
            {
                //Carry On
            }

            int intNumberRetries = 0;

        WriteLog_Retry:

            try
            {
                using (StreamWriter writeLog = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + pvtstrLogFileName, true))
                {
                    writeLog.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + Message);
                }
            }
            catch (Exception ex)
            {
                if (intNumberRetries > 0)
                {
                    try
                    {
                        using (StreamWriter writeLog = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + pvtstrLogFileErrorName, true))
                        {
                            writeLog.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Service " + Message + " Number Tries = " + intNumberRetries + " Exception = " + ex.Message);
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
