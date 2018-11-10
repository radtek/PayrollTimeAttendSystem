using System;
using System.Collections.Generic;
using System.Text;
using InteractPayroll;
using System.Data;
using System.IO;
using System.IO.Compression;

namespace InteractPayrollClient
{
    public class busClientPayrollLogon
    {
        clsDBConnectionObjects clsDBConnectionObjects;
        clsCrc32 clsCrc32;
      
        public busClientPayrollLogon()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();

            clsCrc32 = new clsCrc32();
        }

        public byte[] Logon_Client_DataBase()
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            try
            {
                //2017-05-03 Fix To Make Sure Databases are In Sync
                clsFixInteractPayrollClientDatabase clsFixInteractPayrollClientDatabase = new clsFixInteractPayrollClientDatabase();
                clsFixInteractPayrollClientDatabase.Fix_Client_Database();
            }
            catch
            {
            }

            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();

            try
            {
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" FILE_NAME");
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_DETAILS");

                //2013-07-18 - Empty (This Can be Removed in 2 -3 Months Time (For Backkward Compatibility)
                strQry.AppendLine(" WHERE FILE_LAYER_IND = 'ZZ'");

                //Used to Delete (Cleanup) where File is Not Downloaded from Internet Site
                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ServerFile");

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" FILE_LAYER_IND");
                strQry.AppendLine(",FILE_NAME");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_DETAILS");

                //Empty - Used to Add Files To be Deleted from Client Database
                strQry.AppendLine(" WHERE FILE_LAYER_IND = 'Z'");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "FileToDelete");

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" FILE_LAYER_IND");
                strQry.AppendLine(",FILE_NAME");
                strQry.AppendLine(",FILE_LAST_UPDATED_DATE");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_DETAILS");

                //2013-06-27
                strQry.AppendLine(" WHERE FILE_LAYER_IND = 'P'");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ClientFile");

                string strBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
#if (DEBUG)
                strBaseDirectory += "bin\\";
#endif
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

                DataTable DataTable = new DataTable("ReturnValues");
                DataTable.Columns.Add("MACHINE_NAME", typeof(String));
                DataTable.Columns.Add("MACHINE_IP", typeof(String));

                DataSet.Tables.Add(DataTable);

                DataRow = DataSet.Tables["ReturnValues"].NewRow();

                try
                {
                    DataRow["MACHINE_NAME"] = System.Net.Dns.GetHostName();
                }
                catch
                {
                    DataRow["MACHINE_NAME"] = "UNKNOWN";
                }

                try
                {
                    DataRow["MACHINE_IP"] = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList[0].ToString();
                }
                catch
                {
                    DataRow["MACHINE_IP"] = "UNKNOWN";
                }

                DataSet.Tables["ReturnValues"].Rows.Add(DataRow);

                DataSet.AcceptChanges();
            }
            catch(Exception ex)
            {
                string strInnerExceptionMessage = "";

                if (ex.InnerException != null)
                {
                    strInnerExceptionMessage = ex.InnerException.Message;
                }

                using (StreamWriter writeLog = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "busClientPayrollLogon_Log.txt", true))
                {
                    writeLog.WriteLine(" ********** Exception = " + ex.Message + " " + strInnerExceptionMessage);
                }
            }

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public void Cleanup_Client_DataBase_Files(byte[] parByteArrayDataSet)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet parClientDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parByteArrayDataSet);

            StringBuilder strQry = new StringBuilder();

            for (int intRow = 0; intRow < parClientDataSet.Tables["FileToDelete"].Rows.Count; intRow++)
            {
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_DETAILS");
                strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parClientDataSet.Tables["FileToDelete"].Rows[intRow]["FILE_LAYER_IND"].ToString()));
                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parClientDataSet.Tables["FileToDelete"].Rows[intRow]["FILE_NAME"].ToString()));

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS");
                strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parClientDataSet.Tables["FileToDelete"].Rows[intRow]["FILE_LAYER_IND"].ToString()));
                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parClientDataSet.Tables["FileToDelete"].Rows[intRow]["FILE_NAME"].ToString()));

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }
        }

        public int Insert_New_File_Chunk(string strFileDownloadName, string parstrLayerInd, int parintChunkNo, byte[] pvtbytChunkBytes, bool blnComplete, string parstrFileCRCValue, int intFileSizeCompressed, int intFileSize, DateTime parDtFileLastUpdatedDate, string parstrFileVersionNo)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            StringBuilder strQry = new StringBuilder();
            int intReturnCode = 0;

            if (parintChunkNo == 1)
            {
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP ");
                strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerInd));
                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(strFileDownloadName));

                this.clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            //Insert into Client
            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP ");
            strQry.AppendLine("(FILE_LAYER_IND");
            strQry.AppendLine(",FILE_NAME");
            strQry.AppendLine(",FILE_CHUNK_NO)");
            strQry.AppendLine(" VALUES ");
            strQry.AppendLine("(" + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerInd));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strFileDownloadName));
            strQry.AppendLine("," + parintChunkNo.ToString() + ")");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP");
            strQry.AppendLine(" SET FILE_CHUNK = @FILE_CHUNK");
            strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerInd));
            strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(strFileDownloadName));
            strQry.AppendLine(" AND FILE_CHUNK_NO = " + parintChunkNo.ToString());

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString(), pvtbytChunkBytes, "@FILE_CHUNK");

            if (blnComplete == true)
            {
                DataSet myDataSet = new DataSet();

                long pvtlngDestinationFileStartIndex = 0;

                byte[] pvtbytBytes = new byte[intFileSizeCompressed];
                byte[] pvtbytTempBytes;

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" FILE_CHUNK_NO");
                strQry.AppendLine(",FILE_CHUNK");
               
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP ");
                
                strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerInd));
                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(strFileDownloadName));

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" FILE_CHUNK_NO");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), myDataSet, "FileChunk", 3);

                for (int intRow1 = 0; intRow1 < myDataSet.Tables["FileChunk"].Rows.Count; intRow1++)
                {
                    pvtbytTempBytes = (byte[])myDataSet.Tables["FileChunk"].Rows[intRow1]["FILE_CHUNK"];

                    Array.Copy(pvtbytTempBytes, 0, pvtbytBytes, pvtlngDestinationFileStartIndex, pvtbytTempBytes.Length);
                    pvtlngDestinationFileStartIndex += pvtbytTempBytes.Length;
                }

                byte[] pvtbytDecompressedBytes = new byte[intFileSize];

                //Open Memory Stream with Compressed Data
                MemoryStream msMemoryStream = new MemoryStream(pvtbytBytes);

                System.IO.Compression.GZipStream GZipStreamDecompress = new GZipStream(msMemoryStream, CompressionMode.Decompress);

                //Decompress Bytes
                BinaryReader pvtbrBinaryReader = new BinaryReader(GZipStreamDecompress);
                pvtbytDecompressedBytes = pvtbrBinaryReader.ReadBytes(intFileSize);

                //CRC32 Value
                string strCRC32Value = "";

                foreach (byte b in clsCrc32.ComputeHash(pvtbytDecompressedBytes))
                {
                    strCRC32Value += b.ToString("x2").ToLower();
                }

                if (strCRC32Value == parstrFileCRCValue)
                {
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_DETAILS ");
                    strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerInd));
                    strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(strFileDownloadName));

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    if (parstrLayerInd == "P")
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
                        strQry.AppendLine("(" + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerInd));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strFileDownloadName));
                        strQry.AppendLine(",'" + Convert.ToDateTime(parDtFileLastUpdatedDate).ToString("yyyy-MM-dd HH:mm:ss") + "'");
                        strQry.AppendLine("," + intFileSize.ToString());
                        strQry.AppendLine("," + intFileSizeCompressed.ToString());
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrFileVersionNo));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrFileCRCValue) + ")");

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                    }

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS ");
                    strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerInd));
                    strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(strFileDownloadName));

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    if (parstrLayerInd == "P")
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

                        strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerInd));
                        strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(strFileDownloadName));

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                    }

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP ");
                    strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerInd));
                    strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(strFileDownloadName));

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    //Check To Write Server File
                    if (parstrLayerInd == "S")
                    {
                        string strBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
#if(DEBUG)
                        //Put Here to Stop overwrite of New Compiled Programs is Debug Directory
                        strBaseDirectory = AppDomain.CurrentDomain.BaseDirectory + "bin\\";
#endif
                       
                        FileStream fsFileStream = new FileStream(strBaseDirectory + strFileDownloadName + "_", FileMode.Create);
                        BinaryWriter bwBinaryWriter = new BinaryWriter(fsFileStream);

                        bwBinaryWriter.Write(pvtbytDecompressedBytes);

                        //Write Memory Portion To Disk
                        bwBinaryWriter.Close();

                        File.SetLastWriteTime(strBaseDirectory + strFileDownloadName + "_", parDtFileLastUpdatedDate);

                        //Need to Reboot 
                        intReturnCode = 9;
                    }
                }
                else
                {
                    //CRC Error
                    intReturnCode = 1;
                }
            }

            return intReturnCode;
        }

        public void Insert_File_Chunk(string strFileDownloadName,string parstrLayerInd,int parintChunkNo, byte[] pvtbytTempBytes)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();

            if (parintChunkNo == 1)
            {
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP ");
                strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerInd));
                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(strFileDownloadName));

                this.clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            //Insert into Client
            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP ");
            strQry.AppendLine("(FILE_LAYER_IND");
            strQry.AppendLine(",FILE_NAME");
            strQry.AppendLine(",FILE_CHUNK_NO)");
            strQry.AppendLine(" VALUES ");
            strQry.AppendLine("(" + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerInd));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strFileDownloadName));
            strQry.AppendLine("," + parintChunkNo.ToString() + ")");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP");
            strQry.AppendLine(" SET FILE_CHUNK = @FILE_CHUNK");
            strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerInd));
            strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(strFileDownloadName));
            strQry.AppendLine(" AND FILE_CHUNK_NO = " + parintChunkNo.ToString());

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString(), pvtbytTempBytes, "@FILE_CHUNK");
        }

        public int File_Write_To_Table(string parstrLayerInd, string strFileName, string parstrFileCRCValue, int intFileSizeCompressed, int intFileSize, string parstrFileLastUpdateDateTime,string parstrFileVersionNo)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            //CRC Check
            StringBuilder strQry = new StringBuilder();
            int intReturnCode = 0;
           
            if (parstrFileCRCValue != "")
            {
                DataSet myDataSet = new DataSet();

                long pvtlngDestinationFileStartIndex = 0;

                byte[] pvtbytBytes = new byte[intFileSizeCompressed];
                byte[] pvtbytTempBytes;

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" FILE_CHUNK_NO");
                strQry.AppendLine(",FILE_CHUNK");
               
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP ");
                
                strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerInd));
                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(strFileName));

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" FILE_CHUNK_NO");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), myDataSet, "FileChunk", 3);

                for (int intRow1 = 0; intRow1 < myDataSet.Tables["FileChunk"].Rows.Count; intRow1++)
                {
                    pvtbytTempBytes = (byte[])myDataSet.Tables["FileChunk"].Rows[intRow1]["FILE_CHUNK"];

                    Array.Copy(pvtbytTempBytes, 0, pvtbytBytes, pvtlngDestinationFileStartIndex, pvtbytTempBytes.Length);
                    pvtlngDestinationFileStartIndex += pvtbytTempBytes.Length;
                }

                byte[] pvtbytDecompressedBytes = new byte[intFileSize];

                //Open Memory Stream with Compressed Data
                MemoryStream msMemoryStream = new MemoryStream(pvtbytBytes);

                System.IO.Compression.GZipStream GZipStreamDecompress = new GZipStream(msMemoryStream, CompressionMode.Decompress);

                //Decompress Bytes
                BinaryReader pvtbrBinaryReader = new BinaryReader(GZipStreamDecompress);
                pvtbytDecompressedBytes = pvtbrBinaryReader.ReadBytes(intFileSize);

                //CRC32 Value
                string strCRC32Value = "";

                foreach (byte b in clsCrc32.ComputeHash(pvtbytDecompressedBytes))
                {
                    strCRC32Value += b.ToString("x2").ToLower();
                }

                if (strCRC32Value != parstrFileCRCValue)
                {
                    //Error
                    intReturnCode = 1;

                    goto File_Write_To_Table_Continue;
                }
            }

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_DETAILS ");
            strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerInd));
            strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(strFileName));

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
            strQry.AppendLine("(" + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerInd));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strFileName));
            strQry.AppendLine(",'" + Convert.ToDateTime(parstrFileLastUpdateDateTime) + "'");
            strQry.AppendLine("," + intFileSize.ToString());
            strQry.AppendLine("," + intFileSizeCompressed.ToString());
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrFileVersionNo));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrFileCRCValue) + ")");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS ");
            strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerInd));
            strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(strFileName));

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
            
            strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerInd));
            strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(strFileName));

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP ");
            strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerInd));
            strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(strFileName));

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

        File_Write_To_Table_Continue:

            return intReturnCode;
        }
    }
}
