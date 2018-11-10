using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractPayroll;
using System.IO;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using System.Diagnostics;

namespace InteractPayrollClient
{
    public class busBackupRestoreClientDatabase
    {
        clsDBConnectionObjects clsDBConnectionObjects;
        clsCrc32 clsCrc32;

        public busBackupRestoreClientDatabase()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
            clsCrc32 = new clsCrc32();
        }

        public int Backup_DataBase()
        {
            int intReturnCode = 9;

            try
            {
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
#if(DEBUG)
                strDataBaseName = "InteractPayrollClient_Debug";
#endif
                string strBackupFileName = strDataBaseName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".bak";

                strQry.Clear();
                strQry.AppendLine("BACKUP DATABASE " + strDataBaseName + " TO DISK = '" + strFileDirectory + "\\" + strBackupFileName + "' WITH CHECKSUM ");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString(), 60);

                intReturnCode = 0;
            }
            catch (Exception ex)
            {
                WriteExceptionLog("Backup_DataBase", ex);
            }

            return intReturnCode;
        }

        public void Delete_Backup_File(string parstrFileName)
        {
            File.Delete(parstrFileName);
        }

        public int Backup_DataBase_Before_Restore()
        {
            int intReturnCode = 9;

            try
            {
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
#if(DEBUG)
                strDataBaseName = "InteractPayrollClient_Debug";
#endif
                string strBackupFileName = strDataBaseName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_Backup_Before_Restore.bak";

                strQry.Clear();
                strQry.AppendLine("BACKUP DATABASE " + strDataBaseName + " TO DISK = '" + strFileDirectory + "\\" + strBackupFileName + "' WITH CHECKSUM ");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString(), 60);

                intReturnCode = 0;
            }
            catch (Exception ex)
            {
                WriteExceptionLog("Backup_DataBase_Before_Restore", ex);
            }

            return intReturnCode;
        }

        public int Restore_DataBase(string parstrFileName)
        {
            int intReturnCode = 1;
            
            try
            {
                StringBuilder strQry = new StringBuilder();
              
                string strDataBaseName = "InteractPayrollClient";
#if(DEBUG)
                strDataBaseName = "InteractPayrollClient_Debug";
#endif
                DataSet DataSet = new DataSet();

                strQry.Clear();
               
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" SPID");

                strQry.AppendLine(" FROM MASTER.dbo.SYSPROCESSES");

                strQry.AppendLine(" WHERE DBID = DB_ID('" + strDataBaseName + "')");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" SPID DESC");
                
                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Spid",60);

                for (int intRow = 0; intRow < DataSet.Tables["Spid"].Rows.Count; intRow++)
                {
                    string strKill = "USE MASTER KILL " + DataSet.Tables["Spid"].Rows[intRow]["spid"].ToString();

                    try
                    {
                        clsDBConnectionObjects.Execute_SQLCommand_Client(strKill, 60);
                    }
                    catch
                    {
                    }
                }

                strQry.Clear();

                strQry.AppendLine("RESTORE DATABASE " + strDataBaseName + " FROM DISK = '" + parstrFileName + "' WITH REPLACE");

                clsDBConnectionObjects.Execute_SQLCommand_Restore_Client(strQry.ToString(), 60);
                
                intReturnCode = 0;
            }
            catch (Exception ex)
            {
                WriteExceptionLog("Restore_DataBase " + parstrFileName, ex);
            }

            return intReturnCode;
        }

        public byte[] Get_Restore_Files()
        {
            DataSet DataSet = new System.Data.DataSet();

            DataSet.Tables.Add("RestoreFiles");
            DataSet.Tables["RestoreFiles"].Columns.Add("RESTORE_DATETIME", typeof(DateTime));
            DataSet.Tables["RestoreFiles"].Columns.Add("RESTORE_FILE", typeof(String));

            StringBuilder strQry = new StringBuilder();

            string strFileDirectory = AppDomain.CurrentDomain.BaseDirectory + "Backup";

            if (Directory.Exists(strFileDirectory))
            {
                string strDataBaseName = "InteractPayrollClient_";
#if(DEBUG)
                strDataBaseName = "InteractPayrollClient_Debug_";
#endif
                string[] filePaths = Directory.GetFiles(@strFileDirectory, strDataBaseName + @"*.bak");
                
                if (filePaths.Length > 0)
                {
                    for (int intRow = 0; intRow < filePaths.Length; intRow++)
                    {
                        int intDateTimeOffset = filePaths[intRow].IndexOf(strDataBaseName) + strDataBaseName.Length;

                        if (intDateTimeOffset + 19 <= filePaths[intRow].Length)
                        {
                            DataRow drDataRow = DataSet.Tables["RestoreFiles"].NewRow();

                            drDataRow["RESTORE_FILE"] = filePaths[intRow];

                            try
                            {
                                DateTime myFileDateTime = DateTime.ParseExact(filePaths[intRow].Substring(intDateTimeOffset,15), "yyyyMMdd_HHmmss", null);

                                drDataRow["RESTORE_DATETIME"] = myFileDateTime;
                            }
                            catch
                            {
                            }

                            DataSet.Tables["RestoreFiles"].Rows.Add(drDataRow);
                        }
                    }
                }
            }

            DataSet.AcceptChanges();

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_File_Chunk(string parstrFileName,int parintFileChunkNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FILE_CHUNK");
                        
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.FILE_CLIENT_UPLOAD_CHUNKS");

            strQry.AppendLine(" WHERE FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
            strQry.AppendLine(" AND FILE_CHUNK_NO = " + parintFileChunkNo);
    
            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Temp");

            return (byte[])DataSet.Tables["Temp"].Rows[0]["FILE_CHUNK"];
        }

        public byte[] Load_File_Into_Database(string parstrFileName)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_UPLOAD_DETAILS ");
               
            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_UPLOAD_CHUNKS ");
               
            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                
            FileInfo fi = new FileInfo(parstrFileName);
            FileVersionInfo fivFileVersionInfo = FileVersionInfo.GetVersionInfo(parstrFileName);

            string strFileName = fi.Name;
                
            string strCRC32Value = "";
            int intNumberOfBytesToRead = 50000;
            DateTime dtUploadDateTime = DateTime.Now;

            byte[] pvtbytes = new byte[intNumberOfBytesToRead];

            DateTime dtFileLastUpdated = fi.LastWriteTime;
            int intFileSize = Convert.ToInt32(fi.Length);
            int intCompressedSize;
            string strVersionNumber = fivFileVersionInfo.FileMajorPart.ToString() + "." + fivFileVersionInfo.FileMinorPart.ToString("00");
            bool blnComplete = false;

            FileStream pvtfsFileStream = new FileStream(parstrFileName, FileMode.Open, FileAccess.Read);

            //Read FileStream To Bytes Array
            byte[] ByteArray = new byte[pvtfsFileStream.Length];
            pvtfsFileStream.Read(ByteArray, 0, Convert.ToInt32(pvtfsFileStream.Length));

            //New CRC32 Value
            strCRC32Value = "";

            foreach (byte b in clsCrc32.ComputeHash(ByteArray))
            {
                strCRC32Value += b.ToString("x2").ToLower();
            }

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

            intCompressedSize = Convert.ToInt32(msMemoryStream.Length);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.FILE_CLIENT_UPLOAD_DETAILS ");
            strQry.AppendLine("(FILE_NAME");
            strQry.AppendLine(",FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FILE_SIZE");
            strQry.AppendLine(",FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE)");

            strQry.AppendLine(" VALUES");

            strQry.AppendLine("(" + clsDBConnectionObjects.Text2DynamicSQL(strFileName));
            strQry.AppendLine(",'" + dtFileLastUpdated.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            strQry.AppendLine("," + intFileSize);
            strQry.AppendLine("," + intCompressedSize);
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strVersionNumber));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strCRC32Value) + ")");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            for (int intBlockNumber = 1; intBlockNumber <= intNumberBlocks; intBlockNumber++)
            {
                if (intBlockNumber == intNumberBlocks)
                {
                    intNumberOfBytesToRead = Convert.ToInt32(msMemoryStream.Length - intNumberBytesAlreadyRead);

                    pvtbytes = null;
                    pvtbytes = new byte[intNumberOfBytesToRead];

                    blnComplete = true;
                }

                pvtbrBinaryReader.BaseStream.Position = intNumberBytesAlreadyRead;

                intNumberBytesRead = pvtbrBinaryReader.Read(pvtbytes, 0, intNumberOfBytesToRead);

                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.FILE_CLIENT_UPLOAD_CHUNKS ");
                strQry.AppendLine("(FILE_NAME");
                strQry.AppendLine(",FILE_CHUNK_NO");
                strQry.AppendLine(",FILE_CHUNK)");

                strQry.AppendLine(" VALUES");

                strQry.AppendLine("(" + clsDBConnectionObjects.Text2DynamicSQL(strFileName));
                strQry.AppendLine("," + intBlockNumber);
                strQry.AppendLine(",@FILE_CHUNK)");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString(), pvtbytes, "@FILE_CHUNK");

                intNumberBytesAlreadyRead += intNumberBytesRead;
            }

            pvtfsFileStream.Close();
            msMemoryStream.Close();
            pvtbrBinaryReader.Close();

            pvtfsFileStream = null;
            msMemoryStream = null;
            pvtbrBinaryReader = null;
            
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FILE_NAME");

            strQry.AppendLine(",FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FILE_SIZE");
            strQry.AppendLine(",FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");
            strQry.AppendLine("," + intNumberBlocks.ToString() + " AS FILE_NUMBER_BLOCKS");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.FILE_CLIENT_UPLOAD_DETAILS");

            strQry.AppendLine(" WHERE FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(strFileName));

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "FileUploaded");

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        private void WriteExceptionLog(string Message, Exception ex)
        {
            string strInnerExceptionMessage = "";

            if (ex.InnerException != null)
            {
                strInnerExceptionMessage = ex.InnerException.Message;
            }

            WriteLog(Message + " Exception = " + ex.Message + " " + strInnerExceptionMessage);
        }

        private void WriteLog(string Message)
        {
            using (StreamWriter writeLog = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\FingerPrintClockTimeAttendanceService_Log.txt", true))
            {
                writeLog.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + Message);
            }
        }
    }
}
