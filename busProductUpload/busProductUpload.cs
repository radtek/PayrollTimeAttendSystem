using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace InteractPayroll
{
    public class busProductUpload
    {
        clsDBConnectionObjects clsDBConnectionObjects;
        clsCrc32 clsCrc32;

        private string pvtstrPayrollServerPath = "";

        public busProductUpload()
        {
            clsDBConnectionObjects = new InteractPayroll.clsDBConnectionObjects();
            clsCrc32 = new clsCrc32();

            FileInfo fiFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "ServerBinPath.txt");

            if (fiFileInfo.Exists == true)
            {
                StreamReader srStreamReader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "ServerBinPath.txt");
                pvtstrPayrollServerPath = srStreamReader.ReadLine();

                srStreamReader.Close();
            }
            else
            {
                //Try (The App Path Leaves Off The bin part "C:\\inetpub\\wwwroot\\InteractPayroll\\bin")
                pvtstrPayrollServerPath = AppDomain.CurrentDomain.BaseDirectory + "\\bin";
            }
        }

        public byte[] Get_Form_Records()
        {
            //Errol Used
            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();
            
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",COMPANY_DESC");

            strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK");
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyLink", -1);

            for (int intRow = 0; intRow < DataSet.Tables["CompanyLink"].Rows.Count; intRow++)
            {
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" C.COMPANY_NO");
                strQry.AppendLine(",C.COMPANY_DESC");
                strQry.AppendLine(",PC.PAY_CATEGORY_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C");
                strQry.AppendLine(" ON PC.COMPANY_NO = C.COMPANY_NO");
                strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");


                strQry.AppendLine(" WHERE PC.COMPANY_NO = " + DataSet.Tables["CompanyLink"].Rows[intRow]["COMPANY_NO"].ToString());
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");
                strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
                
                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", Convert.ToInt64(DataSet.Tables["CompanyLink"].Rows[intRow]["COMPANY_NO"]));
            }

            //Get System Administrators
            strQry.Clear();
            
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" 0 AS USER_NO");
            strQry.AppendLine(",'SysAdmin' AS USER_ID");
            strQry.AppendLine(",'System' AS FIRSTNAME");
            strQry.AppendLine(",'Administrator' AS SURNAME");

            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID");
            //Use Gary
            strQry.AppendLine(" WHERE USER_NO = 1");
            
            strQry.AppendLine(" UNION ");
            
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" USER_NO");
            strQry.AppendLine(",USER_ID");
            strQry.AppendLine(",FIRSTNAME");
            strQry.AppendLine(",SURNAME");
            
            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID");
            
            strQry.AppendLine(" WHERE DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 1");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Users", -1);

            //NB. This is Used to Create the MetaData for Later Inserts via DataRow Records 
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PROGRAM_ID");
            strQry.AppendLine(",FILE_NAME");
            strQry.AppendLine(",FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FILE_SIZE");
            strQry.AppendLine(",FILE_VERSION_NO");
            strQry.AppendLine(",FILE_SIZE_COMPRESSED");
            
            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS");
            
            strQry.AppendLine(" WHERE PROJECT_VERSION = 'zzzzz'");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "TempFiles", -1);
            
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" USER_NO");
          
            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID");

            strQry.AppendLine(" WHERE USER_NO = -9999");
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "TempUsers", -1);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",COMPANY_NO AS PAY_CATEGORY_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK");

            strQry.AppendLine(" WHERE COMPANY_NO = -9999");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "TempPayCategories", -1);
            
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet = null;
            return bytCompress;
        }

        public byte[] Get_Version_Records(string parstrVersion, string parstrProduct)
        {
            //Errol Used
            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" USER_NO");
            
            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_USERS ");

            if (parstrProduct == "Payroll Internet - Client"
            || parstrProduct == "Time Attendance Internet - Client"
            || parstrProduct == "Time Attendance - Client"
            || parstrProduct == "Time Attendance - Server")
            {
                strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrVersion));
            }
            else
            {
                strQry.AppendLine(" WHERE PROJECT_VERSION = 'zzzz'");
            }

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "SelectedUsers", -1);
            
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_PAY_CATEGORIES ");

            if (parstrProduct == "Payroll Internet - Client"
            || parstrProduct == "Time Attendance Internet - Client"
            || parstrProduct == "Time Attendance - Client"
            || parstrProduct == "Time Attendance - Server")
            {
                strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrVersion));
            }
            else
            {
                strQry.AppendLine(" WHERE PROJECT_VERSION = 'zzzz'");
            }

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "SelectedPayCategories", -1);
            
            strQry.Clear();
            strQry.AppendLine(" SELECT ");

            if (parstrProduct == "Payroll Internet - Client"
            | parstrProduct == "Payroll Internet - Server"
            | parstrProduct == "Time Attendance Internet - Client")
            {
                strQry.AppendLine(" PROGRAM_ID");
            }
            else
            {
                strQry.AppendLine("'' AS PROGRAM_ID");
            }

            strQry.AppendLine(",FILE_NAME");
            strQry.AppendLine(",FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FILE_SIZE");
            strQry.AppendLine(",FILE_VERSION_NO");

            if (parstrProduct == "Payroll Internet - Client")
            {
                strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS");
                strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrVersion));

                //P=Payroll B=Both Payroll and Time Attendance Internet C=Complete Set Payroll / Time Attendance Internet / Time Attendance Client  
                strQry.AppendLine(" AND PROGRAM_ID IN ('P','B','C')");
            }
            else
            {
                if (parstrProduct == "Payroll Internet - Server")
                {
                    //Empty
                    strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS");
                    strQry.AppendLine(" WHERE PROJECT_VERSION = 'zzzzz'");
                }
                else
                {
                    if (parstrProduct == "Time Attendance Internet - Client")
                    {
                        strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS");
                        strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrVersion));

                        //T=Time Attendance Internet B=Both Payroll and Time Attendance Internet C=Complete Set Payroll / Time Attendance Internet / Time Attendance Client   
                        strQry.AppendLine(" AND PROGRAM_ID IN ('T','B','C')");
                    }
                    else
                    {
                        strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS");
                        strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrVersion));

                        if (parstrProduct == "Time Attendance - Client")
                        {
                            strQry.AppendLine(" AND FILE_LAYER_IND = 'P'");
                        }
                        else
                        {
                            strQry.AppendLine(" AND FILE_LAYER_IND = 'S'");

                        }
                    }
                }
            }

            if (parstrProduct == "Time Attendance Internet - Client")
            {
                strQry.AppendLine(" UNION ");
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" 'B' AS PROGRAM_ID");
                strQry.AppendLine(",FILE_NAME");
                strQry.AppendLine(",FILE_LAST_UPDATED_DATE");
                strQry.AppendLine(",FILE_SIZE");
                strQry.AppendLine(",FILE_VERSION_NO");

                strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS");
                strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrVersion));
                strQry.AppendLine(" AND FILE_LAYER_IND = 'P' ");

                //P=Payroll B=Both Payroll and Time Attendance Internet  
                strQry.AppendLine(" AND FILE_NAME = 'clsISClientUtilities.dll'");
            }
            else
            {
                if (parstrProduct == "Time Attendance - Server")
                {
                    strQry.AppendLine(" UNION ");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" '' AS PROGRAM_ID");
                    strQry.AppendLine(",FILE_NAME");
                    strQry.AppendLine(",FILE_LAST_UPDATED_DATE");
                    strQry.AppendLine(",FILE_SIZE");
                    strQry.AppendLine(",FILE_VERSION_NO");

                    strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS");
                    strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrVersion));

                    strQry.AppendLine(" AND FILE_NAME = 'URLConfig.txt'");
                }
            }

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" FILE_NAME");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "VersionFiles", -1);

            if (parstrProduct == "Payroll Internet - Server")
            {
                DirectoryInfo di = new DirectoryInfo(pvtstrPayrollServerPath);
                FileInfo[] fiFiles = di.GetFiles("*.*");

                DataRow DataRow;

                foreach (FileInfo fi in fiFiles)
                {
                    DataRow = DataSet.Tables["VersionFiles"].NewRow();

                    DataRow["PROGRAM_ID"] = "S";
                    DataRow["FILE_NAME"] = fi.Name;
                    DataRow["FILE_LAST_UPDATED_DATE"] = fi.LastWriteTime.ToString("yyyy-MM-dd  HH:mm:ss");
                    DataRow["FILE_SIZE"] = fi.Length;
                    DataRow["FILE_VERSION_NO"] = "Current";

                    DataSet.Tables["VersionFiles"].Rows.Add(DataRow);
                }

                DataSet.AcceptChanges();
            }

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet = null;
            return bytCompress;
        }

        public bool Insert_File_Chunk(string parstrLayerID,string parstrProgramID,string parstrProjectVersion, int intBlockNumber, string parstrFileName, byte[] parbytesCompressed,
            DateTime dtFileLastUpdated, int intFileSize, int intCompressedSize, string strVersionNumber, bool blnComplete, string strProduct,string strFileCRC32Value)
        {
            StringBuilder strQry = new StringBuilder();
            bool blnFileTagRequired = false;
            
            DataSet DataSet = new DataSet();

            if (intBlockNumber == 1)
            {
                if (strProduct == "Payroll Internet - Client"
                    | strProduct == "Payroll Internet - Server"
                    | strProduct == "Time Attendance Internet - Client")
                {
                    //Make Sure Table is Empty for Criteria
                    //PROGRAM_ID is Left Out to Make Sure That Record is Deleted Even if there is a Change From 'P' to 'B' 
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS_TEMP");
                    strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                    strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                }
                else
                {
                    //Make Sure Table is Empty for Criteria
                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP");
                    strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerID));
                    strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                }

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            }

            if (blnComplete == true)
            {
                string strCRC32Value = "";

                if (strProduct == "Payroll Internet - Client"
                   | strProduct == "Payroll Internet - Server"
                   | strProduct == "Time Attendance Internet - Client")
                {
                    if (strProduct == "Payroll Internet - Client"
                    |   strProduct == "Time Attendance Internet - Client")
                    {
                        strQry.Clear();
                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" FILE_CHUNK_NO ");
                        strQry.AppendLine(",FILE_CHUNK");

                        strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS_TEMP");

                        strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                        strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                        strQry.AppendLine(" AND PROGRAM_ID = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProgramID));

                        strQry.AppendLine(" ORDER BY ");
                        strQry.AppendLine(" FILE_CHUNK_NO ");
                      
                        clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "FileCRC", -1);

                        byte[] bytBytes = new byte[intCompressedSize];
                        byte[] bytTempBytes;
                        long pvtlngDestinationFileStartIndex = 0;

                        for (int intRow = 0; intRow < DataSet.Tables["FileCRC"].Rows.Count; intRow++)
                        {
                            bytTempBytes = (byte[])DataSet.Tables["FileCRC"].Rows[intRow]["FILE_CHUNK"];

                            Array.Copy(bytTempBytes, 0, bytBytes, pvtlngDestinationFileStartIndex, bytTempBytes.Length);
                            pvtlngDestinationFileStartIndex += bytTempBytes.Length;
                        }

                        //Add Last Block To Byte Array
                        Array.Copy(parbytesCompressed, 0, bytBytes, pvtlngDestinationFileStartIndex, parbytesCompressed.Length);

                        byte[] pvtbytDecompressedBytes = new byte[intFileSize];

                        //Open Memory Stream with Compressed Data
                        MemoryStream msMemoryStream = new MemoryStream(bytBytes);

                        System.IO.Compression.GZipStream GZipStreamDecompress = new GZipStream(msMemoryStream, CompressionMode.Decompress);

                        //Decompress Bytes
                        BinaryReader pvtbrBinaryReader = new BinaryReader(GZipStreamDecompress);
                        pvtbytDecompressedBytes = pvtbrBinaryReader.ReadBytes(Convert.ToInt32(intFileSize));

                        //CRC32 Value
                        strCRC32Value = "";

                        foreach (byte b in clsCrc32.ComputeHash(pvtbytDecompressedBytes))
                        {
                            strCRC32Value += b.ToString("x2").ToLower();
                        }

                        if (strCRC32Value == strFileCRC32Value)
                        {
                            //Delete Master Chunks
                            strQry.Clear();
                            strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS");
                            strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                            strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                            
                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                            //Delete Master Detail
                            strQry.Clear();
                            strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS");
                            strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                            strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                            
                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                            //Move Chunks to Master
                            strQry.Clear();
                            strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS ");
                            strQry.AppendLine("(PROJECT_VERSION");
                            strQry.AppendLine(",PROGRAM_ID");
                            strQry.AppendLine(",FILE_NAME");
                            strQry.AppendLine(",FILE_CHUNK_NO");
                            strQry.AppendLine(",FILE_CHUNK)");

                            strQry.AppendLine(" SELECT ");
                            strQry.AppendLine(" PROJECT_VERSION");

                            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrProgramID));

                            strQry.AppendLine(",FILE_NAME");
                            strQry.AppendLine(",FILE_CHUNK_NO");
                            strQry.AppendLine(",FILE_CHUNK");

                            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS_TEMP");

                            strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                            strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                            strQry.AppendLine(" AND PROGRAM_ID = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProgramID));


                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                            //Delete Temp Chunks
                            strQry.Clear();
                            strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS_TEMP");
                            strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                            strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                            strQry.AppendLine(" AND PROGRAM_ID = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProgramID));

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                            //Insert Last Block
                            strQry.Clear();
                            strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS");
                            strQry.AppendLine(" (PROJECT_VERSION");
                            strQry.AppendLine(" ,PROGRAM_ID");
                            strQry.AppendLine(" ,FILE_NAME");
                            strQry.AppendLine(" ,FILE_CHUNK_NO)");
                            strQry.AppendLine("  VALUES");
                            strQry.AppendLine(" (" + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));

                            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrProgramID));

                            strQry.AppendLine(" ," + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                            strQry.AppendLine(" ," + intBlockNumber.ToString() + ")");

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                            strQry.Clear();
                            strQry.AppendLine(" UPDATE InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS");
                            strQry.AppendLine(" SET FILE_CHUNK = @FILE_CHUNK");
                            strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                            strQry.AppendLine(" AND PROGRAM_ID = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProgramID));
                            strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                            strQry.AppendLine(" AND FILE_CHUNK_NO = " + intBlockNumber.ToString());

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parbytesCompressed, "@FILE_CHUNK");

                            strQry.Clear();
                            strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS");

                            strQry.AppendLine("(PROJECT_VERSION");
                            strQry.AppendLine(",PROGRAM_ID");
                            strQry.AppendLine(",FILE_NAME");
                            strQry.AppendLine(",FILE_LAST_UPDATED_DATE");
                            strQry.AppendLine(",FILE_SIZE");
                            strQry.AppendLine(",FILE_SIZE_COMPRESSED");
                           
                            strQry.AppendLine(",FILE_VERSION_NO");
                            strQry.AppendLine(",FILE_CRC_VALUE)");
                            strQry.AppendLine(" VALUES ");
                            
                            strQry.AppendLine("(" + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrProgramID));
                            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                            strQry.AppendLine(",'" + dtFileLastUpdated.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                            strQry.AppendLine("," + intFileSize);
                            strQry.AppendLine("," + intCompressedSize);
                            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strVersionNumber));
                            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strFileCRC32Value) + ")");

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                            if (parstrProgramID == "C")
                            {
                                //errol need top test

                                //Delete Master Chunks
                                strQry.Clear();
                                strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS");
                                strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                                strQry.AppendLine(" AND FILE_LAYER_IND = 'P'");

                                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                                //Delete Master Detail
                                strQry.Clear();
                                strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS");
                                strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                                strQry.AppendLine(" AND FILE_LAYER_IND = 'P'");

                                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                                //Move Chunks to Master
                                strQry.Clear();
                                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS ");
                                strQry.AppendLine("(PROJECT_VERSION");
                                strQry.AppendLine(",FILE_LAYER_IND");
                                strQry.AppendLine(",FILE_NAME");
                                strQry.AppendLine(",FILE_CHUNK_NO");
                                strQry.AppendLine(",FILE_CHUNK)");
                                strQry.AppendLine(" SELECT ");
                                strQry.AppendLine(" PROJECT_VERSION");
                                strQry.AppendLine(",'P'");
                                strQry.AppendLine(",FILE_NAME");
                                strQry.AppendLine(",FILE_CHUNK_NO");
                                strQry.AppendLine(",FILE_CHUNK");
                                
                                strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS");

                                strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                                strQry.AppendLine(" AND PROGRAM_ID = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProgramID));
                                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                                strQry.Clear();
                                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS");
                                strQry.AppendLine("(PROJECT_VERSION");
                                strQry.AppendLine(",FILE_LAYER_IND");
                                strQry.AppendLine(",FILE_NAME");
                                strQry.AppendLine(",FILE_LAST_UPDATED_DATE");
                                strQry.AppendLine(",FILE_SIZE");
                                strQry.AppendLine(",FILE_SIZE_COMPRESSED");
                                strQry.AppendLine(",FILE_VERSION_NO");
                                strQry.AppendLine(",FILE_CRC_VALUE)");

                                strQry.AppendLine(" SELECT ");
                                strQry.AppendLine(" PROJECT_VERSION");
                                strQry.AppendLine(",'P'");
                                strQry.AppendLine(",FILE_NAME");
                                strQry.AppendLine(",FILE_LAST_UPDATED_DATE");
                                strQry.AppendLine(",FILE_SIZE");
                                strQry.AppendLine(",FILE_SIZE_COMPRESSED");
                                strQry.AppendLine(",FILE_VERSION_NO");
                                strQry.AppendLine(",FILE_CRC_VALUE");
                                
                                strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS");

                                strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                                strQry.AppendLine(" AND PROGRAM_ID = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProgramID));
                                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                            }

                            if (parstrFileName == "clsISUtilities.dll")
                            {
                                //clsISUtilities.dll Is Used in busTimeAttendanceRun.dll (For Tax Calculation)

                                //Create File or OverWrite File
                                FileStream FileStream = new FileStream(pvtstrPayrollServerPath + "\\" + parstrFileName, FileMode.Create);
                                BinaryWriter pvtbwBinaryWriter = new BinaryWriter(FileStream);

                                pvtbwBinaryWriter.Write(pvtbytDecompressedBytes);

                                //Write Memory Portion To Disk
                                pvtbwBinaryWriter.Close();

                                File.SetLastWriteTime(pvtstrPayrollServerPath + "\\" + parstrFileName, Convert.ToDateTime(dtFileLastUpdated));
                            }

                            strQry.Clear();
                            strQry.AppendLine(" UPDATE InteractPayroll.dbo.BACKUP_DATABASE_PATH");
                            strQry.AppendLine(" SET BACKUP_DB_IND = 'Y'");

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                        }
                    }
                    else
                    {
                        //Write File
                        strQry.Clear();
                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" FILE_CHUNK_NO");
                        strQry.AppendLine(",FILE_CHUNK");
                       
                        strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS_TEMP");

                        strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                        strQry.AppendLine(" AND PROGRAM_ID = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProgramID));
                        strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                        strQry.AppendLine(" ORDER BY ");
                        strQry.AppendLine(" FILE_CHUNK_NO");

                        clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "FileCRC", -1);

                        byte[] bytBytes = new byte[intCompressedSize];
                        byte[] bytTempBytes;
                        long pvtlngDestinationFileStartIndex = 0;

                        for (int intRow = 0; intRow < DataSet.Tables["FileCRC"].Rows.Count; intRow++)
                        {
                            bytTempBytes = (byte[])DataSet.Tables["FileCRC"].Rows[intRow]["FILE_CHUNK"];

                            Array.Copy(bytTempBytes, 0, bytBytes, pvtlngDestinationFileStartIndex, bytTempBytes.Length);
                            pvtlngDestinationFileStartIndex += bytTempBytes.Length;
                        }

                        //Add Last Block To Byte Array
                        Array.Copy(parbytesCompressed, 0, bytBytes, pvtlngDestinationFileStartIndex, parbytesCompressed.Length);

                        byte[] pvtbytDecompressedBytes = new byte[intFileSize];

                        //Open Memory Stream with Compressed Data
                        MemoryStream msMemoryStream = new MemoryStream(bytBytes);

                        System.IO.Compression.GZipStream GZipStreamDecompress = new GZipStream(msMemoryStream, CompressionMode.Decompress);

                        //Decompress Bytes
                        BinaryReader pvtbrBinaryReader = new BinaryReader(GZipStreamDecompress);
                        pvtbytDecompressedBytes = pvtbrBinaryReader.ReadBytes(Convert.ToInt32(intFileSize));

                        //CRC32 Value
                        strCRC32Value = "";

                        foreach (byte b in clsCrc32.ComputeHash(pvtbytDecompressedBytes))
                        {
                            strCRC32Value += b.ToString("x2").ToLower();
                        }

                        if (strCRC32Value == strFileCRC32Value)
                        {
                            if (parstrFileName == "clsDBConnectionObjects.dll")
                            {
                                //2013-07-16
                                //Cleanup
                                strQry.Clear();
                                strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP");
                                strQry.AppendLine(" WHERE FILE_LAYER_IND = 'S'");
                                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                                strQry.Clear();
                                strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS");
                                strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                                strQry.AppendLine(" AND FILE_LAYER_IND = 'S'");
                                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                                strQry.Clear();
                                strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS");
                                strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                                strQry.AppendLine(" AND FILE_LAYER_IND = 'S'");
                                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                                strQry.Clear();
                                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS");
                                strQry.AppendLine("(PROJECT_VERSION");
                                strQry.AppendLine(",FILE_LAYER_IND");
                                strQry.AppendLine(",FILE_NAME");
                                strQry.AppendLine(",FILE_CHUNK_NO");
                                strQry.AppendLine(",FILE_CHUNK)");

                                strQry.AppendLine(" SELECT ");
                                strQry.AppendLine(" PROJECT_VERSION");
                                strQry.AppendLine(",'S'");
                                strQry.AppendLine(",FILE_NAME");
                                strQry.AppendLine(",FILE_CHUNK_NO");
                                strQry.AppendLine(",FILE_CHUNK");
                                strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS_TEMP");
                                strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                                //Insert Last Block
                                strQry.Clear();
                                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS");
                                strQry.AppendLine(" (PROJECT_VERSION");
                                strQry.AppendLine(" ,FILE_LAYER_IND");
                                strQry.AppendLine(" ,FILE_NAME");
                                strQry.AppendLine(" ,FILE_CHUNK_NO)");
                                strQry.AppendLine("  VALUES");
                                strQry.AppendLine(" (" + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                                strQry.AppendLine(" ," + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerID));
                                strQry.AppendLine(" ," + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                                strQry.AppendLine(" ," + intBlockNumber.ToString() + ")");

                                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                                strQry.Clear();
                                strQry.AppendLine(" UPDATE InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS");
                                strQry.AppendLine(" SET FILE_CHUNK = @FILE_CHUNK");
                                strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                                strQry.AppendLine(" AND FILE_CHUNK_NO = " + intBlockNumber.ToString());
                                strQry.AppendLine(" AND FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerID));

                                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parbytesCompressed, "@FILE_CHUNK");

                                strQry.Clear();
                                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS");
                                strQry.AppendLine("(PROJECT_VERSION");
                                strQry.AppendLine(",FILE_LAYER_IND");
                                strQry.AppendLine(",FILE_NAME");
                                strQry.AppendLine(",FILE_LAST_UPDATED_DATE");
                                strQry.AppendLine(",FILE_SIZE");
                                strQry.AppendLine(",FILE_SIZE_COMPRESSED");
                                strQry.AppendLine(",FILE_VERSION_NO");
                                strQry.AppendLine(",FILE_CRC_VALUE)");
                                strQry.AppendLine(" VALUES ");
                                strQry.AppendLine("(" + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                                strQry.AppendLine(",'S'");
                                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                                strQry.AppendLine(",'" + dtFileLastUpdated.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                                strQry.AppendLine("," + intFileSize);
                                strQry.AppendLine("," + intCompressedSize);
                                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strVersionNumber));
                                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strFileCRC32Value) + ")");

                                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                                strQry.Clear();
                                strQry.AppendLine(" UPDATE InteractPayroll.dbo.BACKUP_DATABASE_PATH");
                                strQry.AppendLine(" SET BACKUP_DB_IND = 'Y'");

                                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                            }
                            else
                            {
                                if (parstrFileName == "clsTax.dll")
                                {
                                    //2013-07-16
                                    //Cleanup
                                    //PROGRAM_ID is Left Out to Make Sure That Record is Deleted Even if there is a Change From 'P' to 'B' 
                                    strQry.Clear();
                                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS");
                                    strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                                    strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                                    strQry.Clear();
                                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS");
                                    strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                                    strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                                    //Insert Previous Uploaded Blocks (NB - Server Changed to Presentation (PROGRAM_ID S to P)
                                    strQry.Clear();
                                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS");
                                    strQry.AppendLine("(PROJECT_VERSION");
                                    strQry.AppendLine(",PROGRAM_ID");
                                    strQry.AppendLine(",FILE_NAME");
                                    strQry.AppendLine(",FILE_CHUNK_NO");
                                    strQry.AppendLine(",FILE_CHUNK)");
                                    strQry.AppendLine(" SELECT ");
                                    strQry.AppendLine(" PROJECT_VERSION");
                                    strQry.AppendLine(",'P'");
                                    strQry.AppendLine(",FILE_NAME");
                                    strQry.AppendLine(",FILE_CHUNK_NO");
                                    strQry.AppendLine(",FILE_CHUNK");
                                    strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS_TEMP");
                                    strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                                    strQry.AppendLine(" AND PROGRAM_ID = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProgramID));
                                    strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                                    //Insert Last Block
                                    strQry.Clear();
                                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS ");
                                    strQry.AppendLine("(PROJECT_VERSION");
                                    strQry.AppendLine(",PROGRAM_ID");
                                    strQry.AppendLine(",FILE_NAME");
                                    strQry.AppendLine(",FILE_CHUNK_NO)");
                                    strQry.AppendLine(" VALUES");
                                    strQry.AppendLine("(" + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL("P"));
                                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                                    strQry.AppendLine("," + intBlockNumber.ToString() + ")");

                                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                                    strQry.Clear();
                                    strQry.AppendLine(" UPDATE InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS");
                                    strQry.AppendLine(" SET FILE_CHUNK = @FILE_CHUNK");
                                    strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                                    strQry.AppendLine(" AND PROGRAM_ID = " + clsDBConnectionObjects.Text2DynamicSQL("P"));
                                    strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                                    strQry.AppendLine(" AND FILE_CHUNK_NO = " + intBlockNumber.ToString());
                                    
                                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parbytesCompressed, "@FILE_CHUNK");

                                    strQry.Clear();
                                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS");
                                    strQry.AppendLine("(PROJECT_VERSION");
                                    strQry.AppendLine(",PROGRAM_ID");
                                    strQry.AppendLine(",FILE_NAME");
                                    strQry.AppendLine(",FILE_LAST_UPDATED_DATE");
                                    strQry.AppendLine(",FILE_SIZE");
                                    strQry.AppendLine(",FILE_SIZE_COMPRESSED");
                                    strQry.AppendLine(",FILE_VERSION_NO");
                                    strQry.AppendLine(",FILE_CRC_VALUE)");
                                    strQry.AppendLine(" VALUES ");
                                    strQry.AppendLine("(" + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL("P"));
                                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                                    strQry.AppendLine(",'" + dtFileLastUpdated.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                                    strQry.AppendLine("," + intFileSize);
                                    strQry.AppendLine("," + intCompressedSize);
                                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strVersionNumber));
                                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strFileCRC32Value) + ")");

                                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                                    strQry.Clear();
                                    strQry.AppendLine(" UPDATE InteractPayroll.dbo.BACKUP_DATABASE_PATH");
                                    strQry.AppendLine(" SET BACKUP_DB_IND = 'Y'");

                                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                                }
                            }
                            
                            //Create File or OverWrite File
                            FileStream FileStream = new FileStream(pvtstrPayrollServerPath + "\\" + parstrFileName, FileMode.Create);
                            BinaryWriter pvtbwBinaryWriter = new BinaryWriter(FileStream);

                            pvtbwBinaryWriter.Write(pvtbytDecompressedBytes);

                            //Write Memory Portion To Disk
                            pvtbwBinaryWriter.Close();

                            File.SetLastWriteTime(pvtstrPayrollServerPath + "\\" + parstrFileName, Convert.ToDateTime(dtFileLastUpdated));

                            //Cleanup
                            strQry.Clear();
                            strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS_TEMP");
                            strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                            strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                        }
                    }
                }
                else
                {
                    //Write File
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" FILE_CHUNK_NO");
                    strQry.AppendLine(",FILE_CHUNK");
                    strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP");

                    strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerID));
                    strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                    
                    strQry.AppendLine(" ORDER BY ");
                    strQry.AppendLine(" FILE_CHUNK_NO ");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "FileCRC", -1);

                    byte[] bytBytes = new byte[intCompressedSize];
                    byte[] bytTempBytes;
                    long pvtlngDestinationFileStartIndex = 0;

                    for (int intRow = 0; intRow < DataSet.Tables["FileCRC"].Rows.Count; intRow++)
                    {
                        bytTempBytes = (byte[])DataSet.Tables["FileCRC"].Rows[intRow]["FILE_CHUNK"];

                        Array.Copy(bytTempBytes, 0, bytBytes, pvtlngDestinationFileStartIndex, bytTempBytes.Length);
                        pvtlngDestinationFileStartIndex += bytTempBytes.Length;
                    }

                    //Add Last Block To Byte Array
                    Array.Copy(parbytesCompressed, 0, bytBytes, pvtlngDestinationFileStartIndex, parbytesCompressed.Length);

                    byte[] pvtbytDecompressedBytes = new byte[intFileSize];

                    //Open Memory Stream with Compressed Data
                    MemoryStream msMemoryStream = new MemoryStream(bytBytes);

                    System.IO.Compression.GZipStream GZipStreamDecompress = new GZipStream(msMemoryStream, CompressionMode.Decompress);

                    //Decompress Bytes
                    BinaryReader pvtbrBinaryReader = new BinaryReader(GZipStreamDecompress);
                    pvtbytDecompressedBytes = pvtbrBinaryReader.ReadBytes(Convert.ToInt32(intFileSize));

                    //CRC32 Value
                    strCRC32Value = "";

                    foreach (byte b in clsCrc32.ComputeHash(pvtbytDecompressedBytes))
                    {
                        strCRC32Value += b.ToString("x2").ToLower();
                    }

                    if (strCRC32Value == strFileCRC32Value)
                    {
                        //Delete Master Chunks
                        strQry.Clear();
                        strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS");
                        strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                        strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                        strQry.AppendLine(" AND FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerID));

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                        //Delete Master Detail
                        strQry.Clear();
                        strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS");
                        strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                        strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                        strQry.AppendLine(" AND FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerID));

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                        //Move Chunks to Master
                        strQry.Clear();
                        strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS ");
                        strQry.AppendLine("(PROJECT_VERSION");
                        strQry.AppendLine(",FILE_LAYER_IND");
                        strQry.AppendLine(",FILE_NAME");
                        strQry.AppendLine(",FILE_CHUNK_NO");
                        strQry.AppendLine(",FILE_CHUNK)");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                        strQry.AppendLine(",FILE_LAYER_IND");
                        strQry.AppendLine(",FILE_NAME");
                        strQry.AppendLine(",FILE_CHUNK_NO");
                        strQry.AppendLine(",FILE_CHUNK");
                        strQry.AppendLine(" FROM ");
                        strQry.AppendLine(" InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP");
                        strQry.AppendLine(" WHERE FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                        strQry.AppendLine(" AND FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerID));

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                        //Delete Temp Chunks
                        strQry.Clear();
                        strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP");
                        strQry.AppendLine(" WHERE FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                        strQry.AppendLine(" AND FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerID));

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                        //Insert Last Block
                        strQry.Clear();
                        strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS");
                        strQry.AppendLine("(PROJECT_VERSION");
                        strQry.AppendLine(",FILE_LAYER_IND");
                        strQry.AppendLine(",FILE_NAME");
                        strQry.AppendLine(",FILE_CHUNK_NO)");
                        strQry.AppendLine(" VALUES");
                        strQry.AppendLine("(" + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerID));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                        strQry.AppendLine("," + intBlockNumber.ToString() + ")");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                        strQry.Clear();
                        strQry.AppendLine(" UPDATE InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS");
                        strQry.AppendLine(" SET FILE_CHUNK = @FILE_CHUNK");
                        strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                        strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                        strQry.AppendLine(" AND FILE_CHUNK_NO = " + intBlockNumber.ToString());
                        strQry.AppendLine(" AND FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerID));

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parbytesCompressed, "@FILE_CHUNK");

                        strQry.Clear();
                        strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS");
                        strQry.AppendLine("(PROJECT_VERSION");
                        strQry.AppendLine(",FILE_LAYER_IND");
                        strQry.AppendLine(",FILE_NAME");
                        strQry.AppendLine(",FILE_LAST_UPDATED_DATE");
                        strQry.AppendLine(",FILE_SIZE");
                        strQry.AppendLine(",FILE_SIZE_COMPRESSED");
                        strQry.AppendLine(",FILE_VERSION_NO");
                        strQry.AppendLine(",FILE_CRC_VALUE)");
                        strQry.AppendLine(" VALUES ");
                        strQry.AppendLine("(" + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerID));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                        strQry.AppendLine(",'" + dtFileLastUpdated.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                        strQry.AppendLine("," + intFileSize);
                        strQry.AppendLine("," + intCompressedSize);
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strVersionNumber));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strFileCRC32Value) + ")");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                        strQry.Clear();
                        strQry.AppendLine(" UPDATE InteractPayroll.dbo.BACKUP_DATABASE_PATH");
                        strQry.AppendLine(" SET BACKUP_DB_IND = 'Y'");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                    }
                }
            }
            else
            {
                if (strProduct == "Payroll Internet - Client"
                  | strProduct == "Payroll Internet - Server"
                  | strProduct == "Time Attendance Internet - Client")
                {
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS_TEMP");
                    strQry.AppendLine(" (PROJECT_VERSION");
                    strQry.AppendLine(" ,PROGRAM_ID");
                    strQry.AppendLine(" ,FILE_NAME");
                    strQry.AppendLine(" ,FILE_CHUNK_NO)");
                    strQry.AppendLine("  VALUES");
                    strQry.AppendLine(" (" + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                    strQry.AppendLine(" ," + clsDBConnectionObjects.Text2DynamicSQL(parstrProgramID));
                    strQry.AppendLine(" ," + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                    strQry.AppendLine(" ," + intBlockNumber.ToString() + ")");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(),-1);

                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS_TEMP");
                    strQry.AppendLine(" SET FILE_CHUNK = @FILE_CHUNK");
                    strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                    strQry.AppendLine(" AND PROGRAM_ID = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProgramID));
                    strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                    strQry.AppendLine(" AND FILE_CHUNK_NO = " + intBlockNumber.ToString());

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parbytesCompressed, "@FILE_CHUNK");
                }
                else
                {
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP");
                    strQry.AppendLine(" (FILE_LAYER_IND");
                    strQry.AppendLine(" ,FILE_NAME");
                    strQry.AppendLine(" ,FILE_CHUNK_NO)");
                    strQry.AppendLine("  VALUES");
                    strQry.AppendLine(" (" + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerID));
                    strQry.AppendLine(" ," + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                    strQry.AppendLine(" ," + intBlockNumber.ToString() + ")");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(),-1);

                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP");
                    strQry.AppendLine(" SET FILE_CHUNK = @FILE_CHUNK");
                    strQry.AppendLine(" WHERE FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                    strQry.AppendLine(" AND FILE_CHUNK_NO = " + intBlockNumber.ToString());
                    strQry.AppendLine(" AND FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerID));

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parbytesCompressed, "@FILE_CHUNK");
                }
            }

            return blnFileTagRequired;
        }

        public void Move_Files_From_Beta_To_Current()
        {
            StringBuilder strQry = new StringBuilder();
            
            strQry.Clear();
            strQry.AppendLine(" DELETE FDLC");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FDLC");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FDDB");
            strQry.AppendLine(" ON FDDB.PROJECT_VERSION = 'Beta'");
            strQry.AppendLine(" AND FDDB.PROGRAM_ID = FDLC.PROGRAM_ID");
            strQry.AppendLine(" AND FDDB.FILE_NAME = FDLC.FILE_NAME");

            strQry.AppendLine(" WHERE FDLC.PROJECT_VERSION = 'Current'");
           
            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                
            strQry.Clear();
            strQry.AppendLine(" DELETE FDC");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS FDC");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FDDB");
            strQry.AppendLine(" ON FDDB.PROJECT_VERSION = 'Beta'");
            strQry.AppendLine(" AND FDDB.PROGRAM_ID = FDC.PROGRAM_ID");
            strQry.AppendLine(" AND FDDB.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FDC.PROJECT_VERSION = 'Current'");
            
            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS ");
            strQry.AppendLine("(PROJECT_VERSION");
            strQry.AppendLine(",PROGRAM_ID ");
            strQry.AppendLine(",FILE_NAME ");

            strQry.AppendLine(",FILE_CHUNK_NO ");
            strQry.AppendLine(",FILE_CHUNK) ");

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine("'Current'");
            strQry.AppendLine(",PROGRAM_ID ");
            strQry.AppendLine(",FILE_NAME ");

            strQry.AppendLine(",FILE_CHUNK_NO ");
            strQry.AppendLine(",FILE_CHUNK ");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS ");
                
            strQry.AppendLine(" WHERE PROJECT_VERSION = 'Beta'");
            
            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                
            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS ");
            strQry.AppendLine("(PROJECT_VERSION");
            strQry.AppendLine(",PROGRAM_ID ");
            strQry.AppendLine(",FILE_NAME ");
            strQry.AppendLine(",FILE_LAST_UPDATED_DATE ");
            strQry.AppendLine(",FILE_SIZE ");
            strQry.AppendLine(",FILE_SIZE_COMPRESSED ");
            strQry.AppendLine(",FILE_VERSION_NO ");
            strQry.AppendLine(",FILE_CRC_VALUE) ");

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine("'Current'");
            strQry.AppendLine(",PROGRAM_ID ");
            strQry.AppendLine(",FILE_NAME ");
            strQry.AppendLine(",FILE_LAST_UPDATED_DATE ");
            strQry.AppendLine(",FILE_SIZE ");
            strQry.AppendLine(",FILE_SIZE_COMPRESSED ");
            strQry.AppendLine(",FILE_VERSION_NO ");
            strQry.AppendLine(",FILE_CRC_VALUE ");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS ");

            strQry.AppendLine(" WHERE PROJECT_VERSION = 'Beta'");
            
            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                
            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS ");

            strQry.AppendLine(" WHERE PROJECT_VERSION = 'Beta'");
            
            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                
            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS ");

            strQry.AppendLine(" WHERE PROJECT_VERSION = 'Beta'");
                
            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            
            //Client Files
            strQry.Clear();
            strQry.AppendLine(" DELETE FCDD");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS FCDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS FDDB");
            strQry.AppendLine(" ON FDDB.PROJECT_VERSION = 'Beta'");
            strQry.AppendLine(" AND FDDB.FILE_LAYER_IND = FCDD.FILE_LAYER_IND");
            strQry.AppendLine(" AND FDDB.FILE_NAME = FCDD.FILE_NAME");

            strQry.AppendLine(" WHERE FCDD.PROJECT_VERSION = 'Current'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            strQry.Clear();
            strQry.AppendLine(" DELETE FCDC");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS FCDC");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS FDDB");
            strQry.AppendLine(" ON FDDB.PROJECT_VERSION = 'Beta'");
            strQry.AppendLine(" AND FDDB.FILE_LAYER_IND = FCDC.FILE_LAYER_IND");
            strQry.AppendLine(" AND FDDB.FILE_NAME = FCDC.FILE_NAME");

            strQry.AppendLine(" WHERE FCDC.PROJECT_VERSION = 'Current'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS ");
            strQry.AppendLine("(PROJECT_VERSION");
            strQry.AppendLine(",FILE_LAYER_IND ");
            strQry.AppendLine(",FILE_NAME ");
            strQry.AppendLine(",FILE_CHUNK_NO ");
            strQry.AppendLine(",FILE_CHUNK) ");
            
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine("'Current'");
            strQry.AppendLine(",FILE_LAYER_IND ");
            strQry.AppendLine(",FILE_NAME ");
            strQry.AppendLine(",FILE_CHUNK_NO ");
            strQry.AppendLine(",FILE_CHUNK ");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS ");

            strQry.AppendLine(" WHERE PROJECT_VERSION = 'Beta'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS ");
            strQry.AppendLine("(PROJECT_VERSION");
            strQry.AppendLine(",FILE_LAYER_IND ");
            strQry.AppendLine(",FILE_NAME ");
            strQry.AppendLine(",FILE_LAST_UPDATED_DATE ");
            strQry.AppendLine(",FILE_SIZE ");
            strQry.AppendLine(",FILE_SIZE_COMPRESSED ");
            strQry.AppendLine(",FILE_VERSION_NO ");
            strQry.AppendLine(",FILE_CRC_VALUE) ");

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine("'Current'");
            strQry.AppendLine(",FILE_LAYER_IND ");
            strQry.AppendLine(",FILE_NAME ");
            strQry.AppendLine(",FILE_LAST_UPDATED_DATE ");
            strQry.AppendLine(",FILE_SIZE ");
            strQry.AppendLine(",FILE_SIZE_COMPRESSED ");
            strQry.AppendLine(",FILE_VERSION_NO ");
            strQry.AppendLine(",FILE_CRC_VALUE ");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS ");

            strQry.AppendLine(" WHERE PROJECT_VERSION = 'Beta'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS ");

            strQry.AppendLine(" WHERE PROJECT_VERSION = 'Beta'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS ");

            strQry.AppendLine(" WHERE PROJECT_VERSION = 'Beta'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            
            //Remove All Beta users
            strQry.Clear();
            
            strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_DOWNLOAD_USERS ");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            
            strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_DOWNLOAD_PAY_CATEGORIES ");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }

        public void Insert_Version_Records(byte[] parBytes, string parstrFileVersion)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parBytes);

            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM  InteractPayroll.dbo.FILE_DOWNLOAD_USERS");
            strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileVersion));

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(),-1);

            for (int intRow = 0; intRow < parDataSet.Tables["TempUsers"].Rows.Count; intRow++)
            {
                strQry.Clear();
                strQry.AppendLine(" INSERT INTO  InteractPayroll.dbo.FILE_DOWNLOAD_USERS");
                strQry.AppendLine("(PROJECT_VERSION");
                strQry.AppendLine(",USER_NO)");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine("(" + clsDBConnectionObjects.Text2DynamicSQL(parstrFileVersion));
                strQry.AppendLine("," + parDataSet.Tables["TempUsers"].Rows[intRow]["USER_NO"].ToString() + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(),-1);
            }

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM  InteractPayroll.dbo.FILE_DOWNLOAD_PAY_CATEGORIES");
            strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileVersion));

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            for (int intRow = 0; intRow < parDataSet.Tables["TempPayCategories"].Rows.Count; intRow++)
            {
                strQry.Clear();
                strQry.AppendLine(" INSERT INTO  InteractPayroll.dbo.FILE_DOWNLOAD_PAY_CATEGORIES");
                strQry.AppendLine("(PROJECT_VERSION");
                strQry.AppendLine(",COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_NO)");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine("(" + clsDBConnectionObjects.Text2DynamicSQL(parstrFileVersion));
                strQry.AppendLine("," + parDataSet.Tables["TempPayCategories"].Rows[intRow]["COMPANY_NO"].ToString());
                strQry.AppendLine("," + parDataSet.Tables["TempPayCategories"].Rows[intRow]["PAY_CATEGORY_NO"].ToString() + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            }
            
            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayroll.dbo.BACKUP_DATABASE_PATH");
            strQry.AppendLine(" SET BACKUP_DB_IND = 'Y'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }
        
        public void Delete_File(string parstrLayerID,string parstrFileVersion, string strFile, string strProduct)
        {
            StringBuilder strQry = new StringBuilder();

            if (strProduct == "Payroll Internet - Client")
            {
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS");
                strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileVersion));
                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(strFile));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(),-1);

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS");
                strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileVersion));
                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(strFile));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(),-1);
            }
            else
            {
                if (strProduct.IndexOf("Time Attendance") > -1)
                {
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS");
                    strQry.AppendLine(" WHERE FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(strFile));
                    strQry.AppendLine(" AND FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerID));

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(),-1);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS");
                    strQry.AppendLine(" WHERE FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(strFile));
                    strQry.AppendLine(" AND FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerID));

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(),-1);
                }
            }

            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayroll.dbo.BACKUP_DATABASE_PATH");
            strQry.AppendLine(" SET BACKUP_DB_IND = 'Y'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }
    }
}
