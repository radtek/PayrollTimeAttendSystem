using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace InteractPayroll
{
    public class busFileUpload
    {
        clsDBConnectionObjects clsDBConnectionObjects;
        clsCrc32 clsCrc32;

        public busFileUpload()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
            clsCrc32 = new clsCrc32();
        }

        public byte[] Get_Form_Records(string parstrCurrentUserAccess, Int64 parint64CurrentUserNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();

            if (parstrCurrentUserAccess == "S")
            {
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" U.USER_NO");
                strQry.AppendLine(",U.USER_ID");
                strQry.AppendLine(",U.FIRSTNAME");
                strQry.AppendLine(",U.SURNAME");

                strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID U");

                strQry.AppendLine(" WHERE U.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" UNION ");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" 0 AS USER_NO");
                strQry.AppendLine(",'Admin' AS USER_ID");
                strQry.AppendLine(",'Admin' AS FIRSTNAME");
                strQry.AppendLine(",'Admin' AS SURNAME");

                //Get 1 Row Only
                strQry.AppendLine(" FROM InteractPayroll.dbo.COUNTRY C");

                strQry.AppendLine(" WHERE C.COUNTRY_CODE = 'ZAF' ");
            }
            else
            {
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" U.USER_NO");
                strQry.AppendLine(",U.USER_ID");
                strQry.AppendLine(",U.FIRSTNAME");
                strQry.AppendLine(",U.SURNAME");

                strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID U");

                strQry.AppendLine(" WHERE U.SYSTEM_ADMINISTRATOR_IND = 'Y' ");
                strQry.AppendLine(" AND U.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" UNION ");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" 0 AS USER_NO");
                strQry.AppendLine(",'Admin' AS USER_ID");
                strQry.AppendLine(",'Admin' AS FIRSTNAME");
                strQry.AppendLine(",'Admin' AS SURNAME");

                //Get 1 Row Only
                strQry.AppendLine(" FROM InteractPayroll.dbo.COUNTRY C");
                               
                strQry.AppendLine(" WHERE C.COUNTRY_CODE = 'ZAF' ");
            }

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 4");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "User", -1);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public bool Upload_File(Int64 parint64CurrentUserNo, string dtFileUploadDateTime, int intBlockNumber, string parstrFileName, byte[] parbytesCompressed, 
                                string dtFileLastUpdated, int intFileSize, int intCompressedSize, string strVersionNumber, bool blnComplete, string strFileCRC32Value,
                                string parstrUsers)
        {
            string[] strUser = parstrUsers.Split(',');

            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
            bool blnUploadSuccessful = false;

            if (intBlockNumber == 1)
            {
                strQry.Clear();

                strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_UPLOAD_CHUNKS_TEMP");

                strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo.ToString());

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString().ToString(), -1);
            }

            if (blnComplete == true)
            {
                string strCRC32Value = "";

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" FILE_CHUNK_NO ");
                strQry.AppendLine(",FILE_CHUNK");

                strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_UPLOAD_CHUNKS_TEMP");

                strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                strQry.AppendLine(" AND UPLOAD_DATETIME = '" + dtFileUploadDateTime + "'");

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
                    //Move Chunks to Master
                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.FILE_UPLOAD_CHUNKS ");
                    strQry.AppendLine("(USER_NO");
                    strQry.AppendLine(",FILE_NAME");
                    strQry.AppendLine(",UPLOAD_DATETIME");
                    strQry.AppendLine(",FILE_CHUNK_NO");
                    strQry.AppendLine(",FILE_CHUNK)");
                    
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" USER_NO");
                    strQry.AppendLine(",FILE_NAME");
                    strQry.AppendLine(",UPLOAD_DATETIME");
                    strQry.AppendLine(",FILE_CHUNK_NO");
                    strQry.AppendLine(",FILE_CHUNK");

                    strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_UPLOAD_CHUNKS_TEMP");

                    strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo.ToString());
                    strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                    strQry.AppendLine(" AND UPLOAD_DATETIME = '" + dtFileUploadDateTime + "'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    //Delete Temp Chunks
                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_UPLOAD_CHUNKS_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo.ToString());
                    strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                    strQry.AppendLine(" AND UPLOAD_DATETIME = '" + dtFileUploadDateTime + "'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    strQry.Clear();

                    //Insert Last Block
                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.FILE_UPLOAD_CHUNKS");
                    strQry.AppendLine("(USER_NO");
                    strQry.AppendLine(",FILE_NAME");
                    strQry.AppendLine(",UPLOAD_DATETIME");
                    strQry.AppendLine(",FILE_CHUNK_NO");
                    strQry.AppendLine(",FILE_CHUNK)");

                    strQry.AppendLine(" VALUES");
                    
                    strQry.AppendLine("(" + parint64CurrentUserNo.ToString());
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                    strQry.AppendLine(",'" + dtFileUploadDateTime + "'");
                    strQry.AppendLine("," + intBlockNumber.ToString());
                    strQry.AppendLine(",@FILE_CHUNK)");
                    
                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parbytesCompressed, "@FILE_CHUNK");

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.FILE_UPLOAD_DETAILS");
                    strQry.AppendLine("(USER_NO");
                    strQry.AppendLine(",FILE_NAME");
                    strQry.AppendLine(",UPLOAD_DATETIME");
                    strQry.AppendLine(",FILE_LAST_UPDATED_DATE");
                    strQry.AppendLine(",FILE_SIZE");
                    strQry.AppendLine(",FILE_SIZE_COMPRESSED");
                    strQry.AppendLine(",FILE_VERSION_NO");
                    strQry.AppendLine(",FILE_CRC_VALUE)");

                    strQry.AppendLine(" VALUES ");

                    strQry.AppendLine("(" + parint64CurrentUserNo.ToString());
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                    strQry.AppendLine(",'" + dtFileUploadDateTime + "'");
                    strQry.AppendLine(",'" + dtFileLastUpdated + "'");
                    strQry.AppendLine("," + intFileSize);
                    strQry.AppendLine("," + intCompressedSize);
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strVersionNumber));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strFileCRC32Value) + ")");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    for (int intUserCount = 0; intUserCount < strUser.Length; intUserCount++)
                    {
                        strQry.Clear();

                        strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.FILE_UPLOAD_DETAILS_FOR_USERS");
                        strQry.AppendLine("(USER_NO");
                        strQry.AppendLine(",FILE_NAME");
                        strQry.AppendLine(",UPLOAD_DATETIME");
                        strQry.AppendLine(",FOR_USER_NO)");

                        strQry.AppendLine(" VALUES ");

                        strQry.AppendLine("(" + parint64CurrentUserNo.ToString());
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                        strQry.AppendLine(",'" + dtFileUploadDateTime + "'");
                        strQry.AppendLine("," + strUser[intUserCount].ToString() + ")");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                    }

                    blnUploadSuccessful = true;
                }
            }
            else
            {
                strQry.Clear();

                strQry.AppendLine("INSERT INTO InteractPayroll.dbo.FILE_UPLOAD_CHUNKS_TEMP ");
                strQry.AppendLine("(USER_NO ");
                strQry.AppendLine(",FILE_NAME");
                strQry.AppendLine(",UPLOAD_DATETIME");
                strQry.AppendLine(",FILE_CHUNK_NO");
                strQry.AppendLine(",FILE_CHUNK)");

                strQry.AppendLine(" VALUES");
                
                strQry.AppendLine("(" + parint64CurrentUserNo.ToString());
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                strQry.AppendLine(",'" + dtFileUploadDateTime + "'");
                strQry.AppendLine("," + intBlockNumber.ToString());
                strQry.AppendLine(",@FILE_CHUNK)");
                
                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parbytesCompressed, "@FILE_CHUNK");

                blnUploadSuccessful = true;
            }

            return blnUploadSuccessful;
        }
    }
}
