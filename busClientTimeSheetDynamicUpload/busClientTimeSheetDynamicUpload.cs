using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractPayroll;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.ServiceProcess;
using System.Management;

namespace InteractPayrollClient
{
    public class busClientTimeSheetDynamicUpload
    {
        clsFixInteractPayrollClientDatabase clsFixInteractPayrollClientDatabase;
        clsDBConnectionObjects clsDBConnectionObjects;
        clsCrc32 clsCrc32;
        
        string pvtstrLogFileName = "FingerPrintClockTimeAttendanceService";

        public busClientTimeSheetDynamicUpload()
        {
            clsFixInteractPayrollClientDatabase = new clsFixInteractPayrollClientDatabase();
            clsDBConnectionObjects = new clsDBConnectionObjects();
            clsCrc32 = new clsCrc32();

            if (AppDomain.CurrentDomain.GetData("LogFileName") != null)
            {
                pvtstrLogFileName = AppDomain.CurrentDomain.GetData("LogFileName").ToString();
            }
            else
            {
                pvtstrLogFileName = AppDomain.CurrentDomain.BaseDirectory + pvtstrLogFileName + "_Log.txt";
            }
        }
        
        public byte[] GetPayCategoryDownloadDetails()
        {
            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();

            //2018-09-22
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO ");
            strQry.AppendLine(",PAY_CATEGORY_NO ");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PayCategoryDownload");

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
        
        public byte[] DownloadCheck()
        {
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay

            string strBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
#if(DEBUG)
            //Put Here to Stop overwrite of New Compiled Programs is Debug Directory
            strBaseDirectory = AppDomain.CurrentDomain.BaseDirectory + "bin\\";
#endif
            StringBuilder strQry = new StringBuilder();
            
            //2017-02-11 - Make sure Client Database is Up to Date (Tables / Columns etc)
            clsFixInteractPayrollClientDatabase.Fix_Client_Database();

            //Make Sure Client DB Has latest Triggers
            clsDBConnectionObjects.Check_Client_Triggers();

            DataSet DataSet = new DataSet();

            //2018-08-09
            if (AppDomain.CurrentDomain.GetData("NewFingerPrintClockTimeAttendanceService") != null)
            {
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.TABLES ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'VALIDITE_HOSTING_SERVICE' ");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ValiditeHostingTableExists");

                if (DataSet.Tables["ValiditeHostingTableExists"].Rows.Count > 0)
                {
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");

                    strQry.AppendLine(" DYNAMIC_UPLOAD_TIMESHEETS_RUNNING_IND");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.VALIDITE_HOSTING_SERVICE");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ValiditeHostingService");

                    if (DataSet.Tables["ValiditeHostingService"].Rows.Count == 0)
                    {
                        strQry.Clear();
                        
                        strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.VALIDITE_HOSTING_SERVICE");
                        strQry.AppendLine("(DYNAMIC_UPLOAD_TIMESHEETS_RUNNING_IND)");
                        strQry.AppendLine(" VALUES ");
                        strQry.AppendLine("('N')");

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                    }
                }
            }
            
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FILE_DOWNLOAD_CHECK_DATE");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.DYNAMIC_FILE_DOWNLOAD_CHECK");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "DownloadCheck");
                       
            strQry.Clear();
            strQry.AppendLine(" SELECT  ");
            strQry.AppendLine(" FILE_LAYER_IND ");
            strQry.AppendLine(",FILE_NAME ");
            strQry.AppendLine(",FILE_LAST_UPDATED_DATE ");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_DETAILS ");

            //2013-06-27
            strQry.AppendLine(" WHERE FILE_LAYER_IND = 'P'");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" FILE_LAYER_IND ");
            strQry.AppendLine(",FILE_NAME ");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ClientFile");

            //2013-06-27
            DirectoryInfo di = new DirectoryInfo(strBaseDirectory);
            FileInfo[] fiFiles = di.GetFiles("*.*");

            DataRow DataRow;

            foreach (FileInfo fi in fiFiles)
            {
                DataRow = DataSet.Tables["ClientFile"].NewRow();

                DataRow["FILE_LAYER_IND"] = "S";
                DataRow["FILE_NAME"] = fi.Name;
                DataRow["FILE_LAST_UPDATED_DATE"] = fi.LastWriteTime;

                DataSet.Tables["ClientFile"].Rows.Add(DataRow);
            }
            
            DataRow[] tempDataRows = DataSet.Tables["ClientFile"].Select("FILE_LAYER_IND ='S' AND (FILE_NAME Like '%dll_' OR FILE_NAME Like '%exe_')");

            foreach (DataRow dr in tempDataRows)
            {
                string strName = dr["FILE_NAME"].ToString().Replace("dll_", "dll").Replace("exe_", "exe");

                //Find File with Same Name Excluding '_'
                DataView fiDataView = new DataView(DataSet.Tables["ClientFile"], "FILE_LAYER_IND ='S' AND FILE_NAME = '" + strName + "'", "", DataViewRowState.CurrentRows);

                if (fiDataView.Count == 1)
                {
                    fiDataView.Delete(0);
                }
            }

            DataSet.AcceptChanges();

            strQry.Clear();
            strQry.AppendLine(" SELECT  ");
            strQry.AppendLine(" 'N' AS REBOOT_IND ");
            strQry.AppendLine(",'' AS MACHINE_NAME ");
            strQry.AppendLine(",'' AS MACHINE_IP ");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.COMPANY ");

            strQry.AppendLine(" WHERE COMPANY_NO = -1");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Reboot");
            
            if (File.Exists(strBaseDirectory + "busClientTimeSheetDynamicUpload.dll_") == true)
            {
                DataRow myDataRow = DataSet.Tables["Reboot"].NewRow();

                myDataRow["REBOOT_IND"] = "Y";

                myDataRow["MACHINE_NAME"] = "Error Speak to System Administrator";
                myDataRow["MACHINE_IP"] = "";

                try
                {
                    myDataRow["MACHINE_NAME"] = System.Net.Dns.GetHostName();
                    myDataRow["MACHINE_IP"] = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList[0].ToString();
                }
                catch(Exception ex)
                {
                    WriteLog("DownloadCheck System.Net.Dns.GetHostName() Exception = " + ex.Message + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                DataSet.Tables["Reboot"].Rows.Add(myDataRow);

                DataSet.AcceptChanges();
            }
            else
            {
                if (File.Exists(strBaseDirectory + "clsDBConnectionObjects.dll_") == true)
                {
                    DataRow myDataRow = DataSet.Tables["Reboot"].NewRow();

                    myDataRow["REBOOT_IND"] = "Y";
                   
                    myDataRow["MACHINE_NAME"] = "Error Speak to System Administrator";
                    myDataRow["MACHINE_IP"] = "";

                    try
                    {
                        myDataRow["MACHINE_NAME"] = System.Net.Dns.GetHostName();
                        myDataRow["MACHINE_IP"] = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList[0].ToString();
                    }
                    catch (Exception ex)
                    {
                        WriteLog("DownloadCheck System.Net.Dns.GetHostName() Exception = " + ex.Message + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    }

                    DataSet.Tables["Reboot"].Rows.Add(myDataRow);

                    DataSet.AcceptChanges();
                }
                else
                {
                    if (File.Exists(strBaseDirectory + "FingerPrintClockService.dll_") == true)
                    {
                        DataRow myDataRow = DataSet.Tables["Reboot"].NewRow();

                        myDataRow["REBOOT_IND"] = "Y";
                        
                        myDataRow["MACHINE_NAME"] = "Error Speak to System Administrator";
                        myDataRow["MACHINE_IP"] = "";

                        try
                        {
                            myDataRow["MACHINE_NAME"] = System.Net.Dns.GetHostName();
                            myDataRow["MACHINE_IP"] = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList[0].ToString();
                        }
                        catch (Exception ex)
                        {
                            WriteLog("DownloadCheck System.Net.Dns.GetHostName() Exception = " + ex.Message + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        }

                        DataSet.Tables["Reboot"].Rows.Add(myDataRow);

                        DataSet.AcceptChanges();
                    }
                    else
                    {
                        if (File.Exists(strBaseDirectory + "FingerPrintClockTimeAttendanceService.exe_") == true)
                        {
                            DataRow myDataRow = DataSet.Tables["Reboot"].NewRow();

                            myDataRow["REBOOT_IND"] = "Y";
                            
                            myDataRow["MACHINE_NAME"] = "Error Speak to System Administrator";
                            myDataRow["MACHINE_IP"] = "";

                            try
                            {
                                myDataRow["MACHINE_NAME"] = System.Net.Dns.GetHostName();
                                myDataRow["MACHINE_IP"] = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList[0].ToString();
                            }
                            catch (Exception ex)
                            {
                                WriteLog("DownloadCheck System.Net.Dns.GetHostName() Exception = " + ex.Message + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            }

                            DataSet.Tables["Reboot"].Rows.Add(myDataRow);

                            DataSet.AcceptChanges();
                        }
                    }
                }
            }

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public void DeleteAllConsoleLinesAndSetUploadTimesheetsRunningFlag()
        {
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay

            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.CONSOLE_LINES ");
            
            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            
            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.VALIDITE_HOSTING_SERVICE ");

            strQry.AppendLine(" SET ");
            strQry.AppendLine(" DYNAMIC_UPLOAD_TIMESHEETS_RUNNING_IND = 'Y' ");
            strQry.AppendLine(",START_DYNAMIC_UPLOAD_TIMESHEETS_RUN_IND = 'N' ");
            
            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
        }
        
        public void ResetUploadTimesheetsRunningFlag()
        {
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay

            StringBuilder strQry = new StringBuilder();
            
            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.VALIDITE_HOSTING_SERVICE ");

            strQry.AppendLine(" SET DYNAMIC_UPLOAD_TIMESHEETS_RUNNING_IND = 'N' ");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
        }
        
        public void InsertConsoleLine(string message)
        {
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay

            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.CONSOLE_LINES ");
            strQry.AppendLine("(CONSOLE_LINE_NO ");
            strQry.AppendLine(",CONSOLE_LINE_MESSAGE) ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ISNULL(MAX(CONSOLE_LINE_NO),0) + 1 ");

            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(message));

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.CONSOLE_LINES ");
            
            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
        }

        public int DeleteFile(string parstrFileLayerInd, string parstrFileName)
        {
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay

            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_DETAILS ");
            strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileLayerInd));
            strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS ");
            strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileLayerInd));
            strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            return 0;
        }

        public string Get_MachineName_And_MachineIP()
        {
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay

            string strMachineName = "Error Speak to System Administrator";
            string strMachineIP = "";

            try
            {
                strMachineName = System.Net.Dns.GetHostName();
                strMachineIP = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList[0].ToString();
            }
            catch (Exception ex)
            {
                WriteLog("Get_MachineName_And_MachineIP System.Net.Dns.GetHostName() Exception = " + ex.Message + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
           
            return strMachineName + "," + strMachineIP;
        }

        public string Check_If_Running_New_Service()
        {
            string strReturnValue = "N";

            if (AppDomain.CurrentDomain.GetData("NewFingerPrintClockTimeAttendanceService") != null)
            {
                StringBuilder strQry = new StringBuilder();
                DataSet DataSet = new DataSet();

                //First Check That Tables have been Created
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" TABLE_NAME ");

                strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.TABLES ");

                strQry.AppendLine(" WHERE TABLE_NAME = 'VALIDITE_HOSTING_SERVICE' ");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ValiditeHostingTableExists");

                if (DataSet.Tables["ValiditeHostingTableExists"].Rows.Count > 0)
                {
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");

                    strQry.AppendLine(" DYNAMIC_UPLOAD_TIMESHEETS_RUNNING_IND");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.VALIDITE_HOSTING_SERVICE");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ValiditeHostingService");

                    if (DataSet.Tables["ValiditeHostingService"].Rows.Count > 0)
                    {
                        strQry.Clear();

                        strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.VALIDITE_HOSTING_SERVICE");

                        //Set to Start Running
                        strQry.AppendLine(" SET START_DYNAMIC_UPLOAD_TIMESHEETS_RUN_IND = 'Y'");

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                        if (AppDomain.CurrentDomain.GetData("tmrUploadDownloadTimesheets") != null)
                        {
                            System.Timers.Timer tmrUploadDownloadTimesheets = (System.Timers.Timer)AppDomain.CurrentDomain.GetData("tmrUploadDownloadTimesheets");
                          
                            try
                            {
                                tmrUploadDownloadTimesheets.Stop();
                                tmrUploadDownloadTimesheets.Interval = 1000;
                                tmrUploadDownloadTimesheets.Start();

                                WriteLog("tmrUploadDownloadTimesheets RESET to Fire Immediately ********** ");
                            }
                            catch(Exception ex)
                            {
                                WriteLog("Check_If_Running_New_Service tmrUploadDownloadTimesheets Exception = " + ex.Message + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                        }
                        else
                        {
                            WriteLog("AppDomain.CurrentDomain.GetData('tmrUploadDownloadTimesheets') == NULL");
                        }

                        strReturnValue = "Y";
                    }
                }
            }

            return strReturnValue;
        }
        
        public byte[] ReadDbConsoleLines(int startNo)
        {
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" CONSOLE_LINE_NO");
            strQry.AppendLine(",CONSOLE_LINE_MESSAGE");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.CONSOLE_LINES");
            
            strQry.AppendLine(" WHERE CONSOLE_LINE_NO > " + startNo);
            
            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ConsoleLine");

            strQry.Clear();
            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" DYNAMIC_UPLOAD_TIMESHEETS_RUNNING_IND");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.VALIDITE_HOSTING_SERVICE");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ValiditeHostingService");

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public int InsertChunk(string parstrFileLayerInd, string parstrFileName,int parintChunkNo,byte[] parbytChunk,bool blnComplete, string parstrFileCRCValue,DateTime parDtFileLastUpdatedDate,int parintFileSize,int parintFileSizeCompressed)
        {
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay

            int intReturnCode = 1;

            StringBuilder strQry = new StringBuilder();

            if (parintChunkNo == 1)
            {
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP ");
                strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileLayerInd));
                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            //Insert into Client
            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP ");
            strQry.AppendLine("(FILE_LAYER_IND");
            strQry.AppendLine(",FILE_NAME");
            strQry.AppendLine(",FILE_CHUNK_NO)");
            strQry.AppendLine(" VALUES ");
            strQry.AppendLine("(" + clsDBConnectionObjects.Text2DynamicSQL(parstrFileLayerInd));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
            strQry.AppendLine("," + parintChunkNo + ")");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP");
            strQry.AppendLine(" SET FILE_CHUNK = @FILE_CHUNK");
            strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileLayerInd));
            strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
            strQry.AppendLine(" AND FILE_CHUNK_NO = " + parintChunkNo.ToString());

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString(), parbytChunk, "@FILE_CHUNK");

            if (blnComplete == true)
            {
                DataSet DataSet = new System.Data.DataSet();

                strQry.Clear();
                strQry.AppendLine(" SELECT  ");
                strQry.AppendLine(" FILE_CHUNK_NO ");
                strQry.AppendLine(",FILE_CHUNK ");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP ");

                strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileLayerInd));
                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" FILE_CHUNK_NO ");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "TempClientFile");

                byte[] bytFileBytes = new byte[parintFileSizeCompressed];
                long lngFileStartOffset = 0;
                byte[] bytFileChunkBytes = null;

                for (int intChunkRow = 0; intChunkRow < DataSet.Tables["TempClientFile"].Rows.Count; intChunkRow++)
                {
                    bytFileChunkBytes = (byte[])DataSet.Tables["TempClientFile"].Rows[intChunkRow]["FILE_CHUNK"];

                    Array.Copy(bytFileChunkBytes, 0, bytFileBytes, lngFileStartOffset, bytFileChunkBytes.Length);
                    lngFileStartOffset += bytFileChunkBytes.Length;
                }

                byte[] bytFileDecompressedBytes = new byte[parintFileSize];

                //Open Memory Stream with Compressed Data
                MemoryStream msMemoryStream = new MemoryStream(bytFileBytes);

                System.IO.Compression.GZipStream GZipStreamDecompress = new GZipStream(msMemoryStream, CompressionMode.Decompress);

                //Decompress Bytes
                BinaryReader brBinaryReader = new BinaryReader(GZipStreamDecompress);
                bytFileDecompressedBytes = brBinaryReader.ReadBytes(parintFileSize);

                string strCRC32Value = "";

                foreach (byte b in clsCrc32.ComputeHash(bytFileDecompressedBytes))
                {
                    strCRC32Value += b.ToString("x2").ToLower();
                }

                if (strCRC32Value == parstrFileCRCValue)
                {
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_DETAILS ");
                    strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileLayerInd));
                    strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    if (parstrFileLayerInd == "P")
                    {
                        strQry.Clear();
                        strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_DETAILS ");
                        strQry.AppendLine("(FILE_LAYER_IND");
                        strQry.AppendLine(",FILE_NAME");
                        strQry.AppendLine(",FILE_LAST_UPDATED_DATE");
                        strQry.AppendLine(",FILE_SIZE");
                        strQry.AppendLine(",FILE_SIZE_COMPRESSED");
                        strQry.AppendLine(",FILE_VERSION_NO");
                        strQry.AppendLine(",FILE_CRC_VALUE)");

                        strQry.AppendLine(" VALUES ");

                        strQry.AppendLine("(" + clsDBConnectionObjects.Text2DynamicSQL(parstrFileLayerInd));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                        strQry.AppendLine(",'" + Convert.ToDateTime(parDtFileLastUpdatedDate).ToString("yyyy-MM-dd HH:mm:ss") + "'");
                        strQry.AppendLine("," + parintFileSize.ToString());
                        strQry.AppendLine("," + parintFileSizeCompressed.ToString());
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL("1.00"));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrFileCRCValue) + ")");

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                    }

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS ");
                    strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileLayerInd));
                    strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    if (parstrFileLayerInd == "P")
                    {
                        strQry.Clear();
                        strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS ");
                        strQry.AppendLine("(FILE_LAYER_IND");
                        strQry.AppendLine(",FILE_NAME");
                        strQry.AppendLine(",FILE_CHUNK_NO");
                        strQry.AppendLine(",FILE_CHUNK)");
                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" FILE_LAYER_IND");
                        strQry.AppendLine(",FILE_NAME");
                        strQry.AppendLine(",FILE_CHUNK_NO");
                        strQry.AppendLine(",FILE_CHUNK");
                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP ");
                        strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileLayerInd));
                        strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                    }

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP ");
                    strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileLayerInd));
                    strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    //Check To Write Server File
                    if (parstrFileLayerInd == "S")
                    {
                        string strBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
#if(DEBUG)
                        //Put Here to Stop overwrite of New Compiled Programs is Debug Directory
                        strBaseDirectory = AppDomain.CurrentDomain.BaseDirectory + "bin\\";
#endif
                       
                        FileStream fsFileStream = new FileStream(strBaseDirectory + parstrFileName + "_", FileMode.Create);
                        BinaryWriter bwBinaryWriter = new BinaryWriter(fsFileStream);

                        bwBinaryWriter.Write(bytFileDecompressedBytes);

                        //Write Memory Portion To Disk
                        bwBinaryWriter.Close();

                        File.SetLastWriteTime(strBaseDirectory + parstrFileName + "_", parDtFileLastUpdatedDate);

                        if (parstrFileName == "busClientTimeSheetDynamicUpload.dll"
                            | parstrFileName == "clsDBConnectionObjects.dll")
                        {
                            //Need to Reboot 
                            intReturnCode = 9;
                        }
                        else
                        {
                            intReturnCode = 0;
                        }
                    }
                    else
                    {
                        intReturnCode = 0;
                    }
                }
            }
            else
            {
                intReturnCode = 0;
            }


            return intReturnCode;
        }

        //Old Layer
        public int InsertFile(string parstrFileLayerInd, string parstrFileName, string parstrFileCRCValue,DateTime parDtFileLastUpdatedDate,int parintFileSize,int parintFileSizeCompressed)
        {
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay

            int intReturnCode = 1;

            DataSet DataSet = new System.Data.DataSet();
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" SELECT  ");
            strQry.AppendLine(" FILE_CHUNK_NO ");
            strQry.AppendLine(",FILE_CHUNK ");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP ");

            strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileLayerInd));
            strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" FILE_CHUNK_NO ");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "TempClientFile");

            byte[] bytFileBytes = new byte[parintFileSizeCompressed];
            long lngFileStartOffset = 0;
            byte[] bytFileChunkBytes = null;

            for (int intChunkRow = 0; intChunkRow < DataSet.Tables["TempClientFile"].Rows.Count; intChunkRow++)
            {
                bytFileChunkBytes = (byte[])DataSet.Tables["TempClientFile"].Rows[intChunkRow]["FILE_CHUNK"];

                Array.Copy(bytFileChunkBytes, 0, bytFileBytes, lngFileStartOffset, bytFileChunkBytes.Length);
                lngFileStartOffset += bytFileChunkBytes.Length;
            }

            byte[] bytFileDecompressedBytes = new byte[parintFileSize];

            //Open Memory Stream with Compressed Data
            MemoryStream msMemoryStream = new MemoryStream(bytFileBytes);

            System.IO.Compression.GZipStream GZipStreamDecompress = new GZipStream(msMemoryStream, CompressionMode.Decompress);

            //Decompress Bytes
            BinaryReader brBinaryReader = new BinaryReader(GZipStreamDecompress);
            bytFileDecompressedBytes = brBinaryReader.ReadBytes(parintFileSize);

            string strCRC32Value = "";

            foreach (byte b in clsCrc32.ComputeHash(bytFileDecompressedBytes))
            {
                strCRC32Value += b.ToString("x2").ToLower();
            }

            if (strCRC32Value == parstrFileCRCValue)
            {
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_DETAILS ");
                strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileLayerInd));
                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_DETAILS ");
                strQry.AppendLine("(FILE_LAYER_IND");
                strQry.AppendLine(",FILE_NAME");
                strQry.AppendLine(",FILE_LAST_UPDATED_DATE");
                strQry.AppendLine(",FILE_SIZE");
                strQry.AppendLine(",FILE_SIZE_COMPRESSED");
                strQry.AppendLine(",FILE_VERSION_NO");
                strQry.AppendLine(",FILE_CRC_VALUE)");

                strQry.AppendLine(" VALUES ");

                strQry.AppendLine("(" + clsDBConnectionObjects.Text2DynamicSQL(parstrFileLayerInd));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                strQry.AppendLine(",'" + Convert.ToDateTime(parDtFileLastUpdatedDate).ToString("yyyy-MM-dd HH:mm:ss") + "'");
                strQry.AppendLine("," + parintFileSize.ToString());
                strQry.AppendLine("," + parintFileSizeCompressed.ToString());
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL("1.00"));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrFileCRCValue) + ")");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS ");
                strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileLayerInd));
                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS ");
                strQry.AppendLine("(FILE_LAYER_IND");
                strQry.AppendLine(",FILE_NAME");
                strQry.AppendLine(",FILE_CHUNK_NO");
                strQry.AppendLine(",FILE_CHUNK)");
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" FILE_LAYER_IND");
                strQry.AppendLine(",FILE_NAME");
                strQry.AppendLine(",FILE_CHUNK_NO");
                strQry.AppendLine(",FILE_CHUNK");
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP ");
                strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileLayerInd));
                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP ");
                strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileLayerInd));
                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                intReturnCode = 0;

                //Check To Write Server File
                if (parstrFileLayerInd == "S")
                {
                    string strBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
#if(DEBUG)
                    //Put Here to Stop overwrite of New Compiled Programs is Debug Directory
                    strBaseDirectory = AppDomain.CurrentDomain.BaseDirectory + "bin\\";
#endif
                    FileStream fsFileStream = new FileStream(strBaseDirectory + parstrFileName + "_", FileMode.Create);
                    BinaryWriter bwBinaryWriter = new BinaryWriter(fsFileStream);

                    bwBinaryWriter.Write(bytFileDecompressedBytes);

                    //Write Memory Portion To Disk
                    bwBinaryWriter.Close();

                    File.SetLastWriteTime(strBaseDirectory + parstrFileName + "_", parDtFileLastUpdatedDate);

                    if (parstrFileName == "busClientTimeSheetDynamicUpload.dll"
                        | parstrFileName == "clsDBConnectionObjects.dll")
                    {
                        //Need to Reboot 
                        intReturnCode = 9;
                    }
                }
            }

            return intReturnCode;
        }

        public byte[] GetFileChunks(string parstrFileLayerInd, string parstrFileName)
        {
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay

            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT  ");
            strQry.AppendLine(" FILE_CHUNK_NO ");
            strQry.AppendLine(",FILE_CHUNK ");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS ");

            strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileLayerInd));
            strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" FILE_CHUNK_NO ");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "TempClientFile");

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public int UpdateDateTimeCheck(int parTableCount)
        {
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay

            StringBuilder strQry = new StringBuilder();
            
            if (parTableCount > 0)
            {
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.DYNAMIC_FILE_DOWNLOAD_CHECK");
                strQry.AppendLine(" SET FILE_DOWNLOAD_CHECK_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd") +        "'");
            }
            else
            {
                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.DYNAMIC_FILE_DOWNLOAD_CHECK");
                strQry.AppendLine(" (FILE_DOWNLOAD_CHECK_DATE)");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine(" ('" + DateTime.Now.ToString("yyyy-MM-dd") + "')");
            }

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            return 0;
        }

        public int ResetDateTimeCheck()
        {
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay

            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DYNAMIC_FILE_DOWNLOAD_CHECK");
            strQry.AppendLine(" WHERE FILE_DOWNLOAD_CHECK_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd") + "'");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            return 0;
        }

        public int UpdateLocalRecords(Int64 parInt64CompanyNo, DateTime pardtTimeSheetDateTime, DateTime pardtUploadDateTime, byte[] parbyteDataSet, byte[] parbyteLocalDataSet)
        {
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay

            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);
            DataSet parLocalDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteLocalDataSet);
            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new System.Data.DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" DCT.PAY_CATEGORY_NO");
            strQry.AppendLine(",DCT.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_CLOCK_TIME DCT ");

            //2013-03-02
            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON DCT.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND DCT.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND DCT.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND PC.NO_EDIT_IND = 'Y' ");

            strQry.AppendLine(" WHERE DCT.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND DCT.TIMESHEET_DATE = '" + pardtTimeSheetDateTime.ToString("yyyy-MM-dd") + "'");

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" DCTB.PAY_CATEGORY_NO");
            strQry.AppendLine(",DCTB.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_CLOCK_TIME_BREAK DCTB ");

            //2013-03-02
            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON DCTB.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND DCTB.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND DCTB.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND PC.NO_EDIT_IND = 'Y' ");

            strQry.AppendLine(" WHERE DCTB.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND DCTB.BREAK_DATE = '" + pardtTimeSheetDateTime.ToString("yyyy-MM-dd") + "'");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "CostCentreUpdate");

            DataView DataViewPayCategory;

            for (int intCostCentreRow = 0; intCostCentreRow < DataSet.Tables["CostCentreUpdate"].Rows.Count; intCostCentreRow++)
            {
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.PAY_CATEGORY ");
                strQry.AppendLine(" SET ");
               
                DataViewPayCategory = null;

                if (DataSet.Tables["CostCentreUpdate"].Rows[intCostCentreRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                {
                    DataViewPayCategory = new DataView(parDataSet.Tables["PayCategoryWages"],
                    "PAY_CATEGORY_NO = " + DataSet.Tables["CostCentreUpdate"].Rows[intCostCentreRow]["PAY_CATEGORY_NO"].ToString() + " AND NO_EDIT_IND = 'Y'",
                    "",
                    DataViewRowState.CurrentRows);
                }
                else
                {
                    if (DataSet.Tables["CostCentreUpdate"].Rows[intCostCentreRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                    {
                        DataViewPayCategory = new DataView(parDataSet.Tables["PayCategorySalaries"],
                          "PAY_CATEGORY_NO = " + DataSet.Tables["CostCentreUpdate"].Rows[intCostCentreRow]["PAY_CATEGORY_NO"].ToString() + " AND NO_EDIT_IND = 'Y'",
                          "",
                          DataViewRowState.CurrentRows);
                    }
                    else
                    {
                        DataViewPayCategory = new DataView(parDataSet.Tables["PayCategoryTimeAttendance"],
                          "PAY_CATEGORY_NO = " + DataSet.Tables["CostCentreUpdate"].Rows[intCostCentreRow]["PAY_CATEGORY_NO"].ToString() + " AND NO_EDIT_IND = 'Y'",
                          "",
                          DataViewRowState.CurrentRows);
                    }
                }

                if (DataViewPayCategory.Count > 0)
                {
                    strQry.AppendLine(" LAST_UPLOAD_DATETIME = '" + pardtUploadDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                }
                else
                {
                    //Reset Back to Edit Which is whar is on Internet Site
                    strQry.AppendLine(" NO_EDIT_IND = 'N' ");
                }

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables["CostCentreUpdate"].Rows[intCostCentreRow]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["CostCentreUpdate"].Rows[intCostCentreRow]["PAY_CATEGORY_TYPE"].ToString()));

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            for (int intRow = 0; intRow < parLocalDataSet.Tables["TimeSheet"].Rows.Count; intRow++)
            {
                DataViewPayCategory = null;

                if (parLocalDataSet.Tables["TimeSheet"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                {
                    DataViewPayCategory = new DataView(parDataSet.Tables["PayCategoryWages"],
                    "PAY_CATEGORY_NO = " + parLocalDataSet.Tables["TimeSheet"].Rows[intRow]["PAY_CATEGORY_NO"].ToString() + " AND NO_EDIT_IND = 'Y'",
                    "",
                    DataViewRowState.CurrentRows);
                }
                else
                {
                    if (parLocalDataSet.Tables["TimeSheet"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                    {
                        DataViewPayCategory = new DataView(parDataSet.Tables["PayCategorySalaries"],
                          "PAY_CATEGORY_NO = " + parLocalDataSet.Tables["TimeSheet"].Rows[intRow]["PAY_CATEGORY_NO"].ToString() + " AND NO_EDIT_IND = 'Y'",
                          "",
                          DataViewRowState.CurrentRows);
                    }
                    else
                    {
                        DataViewPayCategory = new DataView(parDataSet.Tables["PayCategoryTimeAttendance"],
                          "PAY_CATEGORY_NO = " + parLocalDataSet.Tables["TimeSheet"].Rows[intRow]["PAY_CATEGORY_NO"].ToString() + " AND NO_EDIT_IND = 'Y'",
                          "",
                          DataViewRowState.CurrentRows);
                    }
                }

                if (DataViewPayCategory.Count == 0)
                {
                    continue;
                }

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEVICE_CLOCK_TIME ");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EMPLOYEE_NO = " + parLocalDataSet.Tables["TimeSheet"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                strQry.AppendLine(" AND TIMESHEET_DATE = '" + Convert.ToDateTime(parLocalDataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_DATE"]).ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parLocalDataSet.Tables["TimeSheet"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'");

                //TIE_BREAKER is Unique
                strQry.AppendLine(" AND TIE_BREAKER = " + parLocalDataSet.Tables["TimeSheet"].Rows[intRow]["TIE_BREAKER"].ToString());
             
                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            for (int intRow = 0; intRow < parLocalDataSet.Tables["Break"].Rows.Count; intRow++)
            {
                DataViewPayCategory = null;

                if (parLocalDataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                {
                    DataViewPayCategory = new DataView(parDataSet.Tables["PayCategoryWages"],
                    "PAY_CATEGORY_NO = " + parLocalDataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_NO"].ToString() + " AND NO_EDIT_IND = 'Y'",
                    "",
                    DataViewRowState.CurrentRows);
                }
                else
                {
                    if (parLocalDataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                    {
                        DataViewPayCategory = new DataView(parDataSet.Tables["PayCategorySalaries"],
                          "PAY_CATEGORY_NO = " + parLocalDataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_NO"].ToString() + " AND NO_EDIT_IND = 'Y'",
                          "",
                          DataViewRowState.CurrentRows);
                    }
                    else
                    {
                        DataViewPayCategory = new DataView(parDataSet.Tables["PayCategoryTimeAttendance"],
                         "PAY_CATEGORY_NO = " + parLocalDataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_NO"].ToString() + " AND NO_EDIT_IND = 'Y'",
                         "",
                         DataViewRowState.CurrentRows);
                    }
                }

                if (DataViewPayCategory.Count == 0)
                {
                    continue;
                }

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEVICE_CLOCK_TIME_BREAK ");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EMPLOYEE_NO = " + parLocalDataSet.Tables["Break"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                strQry.AppendLine(" AND BREAK_DATE = '" + Convert.ToDateTime(parLocalDataSet.Tables["Break"].Rows[intRow]["BREAK_DATE"]).ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parLocalDataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'");

                //TIE_BREAKER is Unique
                strQry.AppendLine(" AND TIE_BREAKER = " + parLocalDataSet.Tables["Break"].Rows[intRow]["TIE_BREAKER"].ToString());

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            if (parDataSet.Tables["Employee"].Rows.Count > 0)
            {
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EMPLOYEE_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "EmployeeLocal");

                DataView DataViewEmployee = null; ;

                for (int intRow1 = 0; intRow1 < parDataSet.Tables["Employee"].Rows.Count; intRow1++)
                {
                    DataViewEmployee = null;
                    DataViewEmployee = new DataView(DataSet.Tables["EmployeeLocal"],
                        "EMPLOYEE_NO = " + parDataSet.Tables["Employee"].Rows[intRow1]["EMPLOYEE_NO"].ToString() + " AND PAY_CATEGORY_TYPE = '" + parDataSet.Tables["Employee"].Rows[intRow1]["PAY_CATEGORY_TYPE"].ToString() + "'",
                        "",
                        DataViewRowState.CurrentRows);

                    if (DataViewEmployee.Count > 0)
                    {
                        if (DataViewEmployee[0]["EMPLOYEE_LAST_RUNDATE"] == System.DBNull.Value)
                        {
                        }
                        else
                        {
                            if (Convert.ToDateTime(DataViewEmployee[0]["EMPLOYEE_LAST_RUNDATE"]).ToString("yyyy-MM-dd HH:mm:ss") == Convert.ToDateTime(parDataSet.Tables["Employee"].Rows[intRow1]["EMPLOYEE_LAST_RUNDATE"]).ToString("yyyy-MM-dd HH:mm:ss"))
                            {
                                continue;
                            }
                        }

                        strQry.Clear();
                        strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.EMPLOYEE ");

                        strQry.AppendLine(" SET EMPLOYEE_LAST_RUNDATE = '" + Convert.ToDateTime(parDataSet.Tables["Employee"].Rows[intRow1]["EMPLOYEE_LAST_RUNDATE"]).ToString("yyyy-MM-dd HH:mm:ss") + "'");

                        strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + DataViewEmployee[0]["EMPLOYEE_NO"].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataViewEmployee[0]["PAY_CATEGORY_TYPE"].ToString()));

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                    }
                }
            }

            return 0;
        }

        public byte[] GetDates(Int64 parInt64CompanyNo)
        {
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay

            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PAY_CATEGORY_DESC");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY ");
          
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            //Dynamic Uplaod of Timesheets Set
            strQry.AppendLine(" AND ISNULL(NO_EDIT_IND,'N') <> 'Y' ");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "UploadCheck");

            strQry.Clear();
            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" DCT.TIMESHEET_DATE");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_CLOCK_TIME DCT ");

            //2013-03-02
            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON DCT.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND DCT.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND DCT.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND PC.NO_EDIT_IND = 'Y' ");

            strQry.AppendLine(" WHERE DCT.COMPANY_NO = " + parInt64CompanyNo);

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" DCTB.BREAK_DATE AS TIMESHEET_DATE");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_CLOCK_TIME_BREAK DCTB");

            //2013-03-02
            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON DCTB.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND DCTB.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND DCTB.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND PC.NO_EDIT_IND = 'Y' ");

            strQry.AppendLine(" WHERE DCTB.COMPANY_NO = " + parInt64CompanyNo);

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 1");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Date");

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] GetCostCentres(Int64 parInt64CompanyNo)
        {
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay

            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND ISNULL(NO_EDIT_IND,'N') = 'Y' ");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "CostCentre");

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
       
        public byte[] GetPayCategoryRecords(Int64 parInt64CompanyNo)
        {
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay

            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" PAY_CATEGORY_NO");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");

            //2013-03-02
            strQry.AppendLine(" AND NO_EDIT_IND = 'Y' ");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "EmployeeRunDateWage");

            strQry.Clear();
            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" PAY_CATEGORY_NO");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");

            //2013-03-02
            strQry.AppendLine(" AND NO_EDIT_IND = 'Y' ");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "EmployeeRunDateSalary");

            strQry.Clear();
            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" PAY_CATEGORY_NO");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");

            //2013-03-02
            strQry.AppendLine(" AND NO_EDIT_IND = 'Y' ");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "EmployeeRunDateTimeAttendance");

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public int Check_Server_File(string parstrFileName,DateTime parFileLastUpdateDateTime)
        {
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay

            int intReturnCode = 1;

            string strBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
#if(DEBUG)
            //Put Here to Stop overwrite of New Compiled Programs is Debug Directory
            strBaseDirectory = AppDomain.CurrentDomain.BaseDirectory + "bin\\";
#endif
            FileInfo fiFileInfo = new FileInfo(strBaseDirectory + parstrFileName);

            if (fiFileInfo.Exists == true)
            {
                if (fiFileInfo.LastWriteTime >= parFileLastUpdateDateTime.AddSeconds(-3)
                & fiFileInfo.LastWriteTime <= parFileLastUpdateDateTime.AddSeconds(3))
                {
                    //Load From Live File
                    int intNumberOfBytesToRead = 10000;
                    
                    byte[] pvtbytes = null;

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

                    bool blnComplete = false;

                    string parstrFileCRCValue = "";
                                                       
                    //New CRC32 Value
                    foreach (byte b in clsCrc32.ComputeHash(ByteArray))
                    {
                        parstrFileCRCValue += b.ToString("x2").ToLower();
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

                        InsertChunk("S", parstrFileName, intBlockNumber, pvtbytes, blnComplete, parstrFileCRCValue, fiFileInfo.LastWriteTime, Convert.ToInt32(fiFileInfo.Length), Convert.ToInt32(msMemoryStream.Length));

                        intNumberBytesAlreadyRead += intNumberBytesRead;
                    }

                    pvtfsFileStream.Close();
                    msMemoryStream.Close();
                    pvtbrBinaryReader.Close();

                    pvtfsFileStream = null;
                    msMemoryStream = null;
                    pvtbrBinaryReader = null;

                    intReturnCode = 0;
                 }
             }

             return intReturnCode;
        }

        public byte[] GetUploadRecords(Int64 parInt64CompanyNo,DateTime parTimeSheetDateTime)
        {
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay
            //Remember Backward Compatibility if Changing This Function - Funtion / Procedure Interface must stay

            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" DCT.COMPANY_NO");
            strQry.AppendLine(",DCT.EMPLOYEE_NO");
            strQry.AppendLine(",DCT.PAY_CATEGORY_NO");
            strQry.AppendLine(",DCT.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DCT.TIMESHEET_DATE");
            strQry.AppendLine(",DCT.TIMESHEET_TIME_MINUTES");
            strQry.AppendLine(",DCT.IN_OUT_IND");
            strQry.AppendLine(",DCT.TIE_BREAKER");
            strQry.AppendLine(",DCT.CLOCKED_BOUNDARY_TIME_MINUTES");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_CLOCK_TIME DCT ");

            //2013-03-02
            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON DCT.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND DCT.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND DCT.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND PC.NO_EDIT_IND = 'Y' ");


            strQry.AppendLine(" WHERE DCT.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND DCT.TIMESHEET_DATE = '" + parTimeSheetDateTime.ToString("yyyy-MM-dd") + "'");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" DCT.COMPANY_NO");
            strQry.AppendLine(",DCT.EMPLOYEE_NO");
            strQry.AppendLine(",DCT.PAY_CATEGORY_NO");
            strQry.AppendLine(",DCT.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DCT.TIMESHEET_DATE");
            strQry.AppendLine(",DCT.TIMESHEET_TIME_MINUTES");
            strQry.AppendLine(",DCT.IN_OUT_IND");
            strQry.AppendLine(",DCT.TIE_BREAKER");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "TimeSheet");

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" DCTB.COMPANY_NO");
            strQry.AppendLine(",DCTB.EMPLOYEE_NO");
            strQry.AppendLine(",DCTB.PAY_CATEGORY_NO");
            strQry.AppendLine(",DCTB.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DCTB.BREAK_DATE");
            strQry.AppendLine(",DCTB.BREAK_TIME_MINUTES");
            strQry.AppendLine(",DCTB.IN_OUT_IND");
            strQry.AppendLine(",DCTB.TIE_BREAKER");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_CLOCK_TIME_BREAK DCTB ");

            //2013-03-02
            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON DCTB.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND DCTB.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND DCTB.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND PC.NO_EDIT_IND = 'Y' ");


            strQry.AppendLine(" WHERE DCTB.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND DCTB.BREAK_DATE = '" + parTimeSheetDateTime.ToString("yyyy-MM-dd") + "'");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" DCTB.COMPANY_NO");
            strQry.AppendLine(",DCTB.EMPLOYEE_NO");
            strQry.AppendLine(",DCTB.PAY_CATEGORY_NO");
            strQry.AppendLine(",DCTB.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DCTB.BREAK_DATE");
            strQry.AppendLine(",DCTB.BREAK_TIME_MINUTES");
            strQry.AppendLine(",DCTB.IN_OUT_IND");
            strQry.AppendLine(",DCTB.TIE_BREAKER");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Break");

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
        
        private void WriteLog(string Message)
        {
            try
            {
                using (FileStream fs = new FileStream(pvtstrLogFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (TextWriter tw = new StreamWriter(fs))
                    {
                        tw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Service " + Message);
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    using (FileStream fs = new FileStream(pvtstrLogFileName.Replace("_Log","_Log_Error"), FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    {
                        using (TextWriter tw = new StreamWriter(fs))
                        {
                            tw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Service " + Message + " Exception = " + ex.Message);
                        }
                    }
                }
                catch
                {
                }
            }
        }
    }
}
