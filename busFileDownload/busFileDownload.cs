using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace InteractPayroll
{
    public class busFileDownload
    {
        clsDBConnectionObjects clsDBConnectionObjects;
        clsCrc32 clsCrc32;

        public busFileDownload()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
            clsCrc32 = new clsCrc32();
        }

        public byte[] Get_Form_Records(string parstrCurrentUserAccess, Int64 parint64CurrentUserNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" U.USER_NO");
            strQry.AppendLine(",U.USER_ID");
            strQry.AppendLine(",U.FIRSTNAME");
            strQry.AppendLine(",U.SURNAME");
           
            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID U");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_UPLOAD_DETAILS FUD ");
            strQry.AppendLine(" ON U.USER_NO = FUD.USER_NO  ");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_UPLOAD_DETAILS_FOR_USERS FUDFU ");
            strQry.AppendLine(" ON FUD.USER_NO = FUDFU.USER_NO");
            strQry.AppendLine(" AND FUD.FILE_NAME = FUDFU.FILE_NAME");
            strQry.AppendLine(" AND FUD.UPLOAD_DATETIME = FUDFU.UPLOAD_DATETIME");

            strQry.AppendLine(" WHERE U.DATETIME_DELETE_RECORD IS NULL");

            if (parstrCurrentUserAccess != "S")
            {
                strQry.AppendLine(" AND FUDFU.FOR_USER_NO = " + parint64CurrentUserNo);
            }

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" 0 AS USER_NO");
            strQry.AppendLine(",'Admin' AS USER_ID");
            strQry.AppendLine(",'Admin' AS FIRSTNAME");
            strQry.AppendLine(",'Admin' AS SURNAME");

            //Get 1 Row Only
            strQry.AppendLine(" FROM InteractPayroll.dbo.COUNTRY C");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_UPLOAD_DETAILS FUD ");
            strQry.AppendLine(" ON FUD.USER_NO = 0");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_UPLOAD_DETAILS_FOR_USERS FUDFU ");
            strQry.AppendLine(" ON FUD.USER_NO = FUDFU.USER_NO");
            strQry.AppendLine(" AND FUD.FILE_NAME = FUDFU.FILE_NAME");
            strQry.AppendLine(" AND FUD.UPLOAD_DATETIME = FUDFU.UPLOAD_DATETIME");

            strQry.AppendLine(" WHERE C.COUNTRY_CODE = 'ZAF' ");

            if (parstrCurrentUserAccess != "S")
            {
                strQry.AppendLine(" AND FUDFU.FOR_USER_NO = " + parint64CurrentUserNo);
            }

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 4");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "User", -1);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FUD.USER_NO");
            strQry.AppendLine(",FUD.FILE_NAME");
            strQry.AppendLine(",FUD.UPLOAD_DATETIME");
            strQry.AppendLine(",FUD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FUD.FILE_SIZE");
            strQry.AppendLine(",FUD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FUD.FILE_VERSION_NO");
            strQry.AppendLine(",FUD.FILE_CRC_VALUE");
            strQry.AppendLine(",ISNULL(U.USER_ID,'Admin') AS USER_ID");
            strQry.AppendLine(",ISNULL(U.FIRSTNAME,'Admin') AS FIRSTNAME");
            strQry.AppendLine(",ISNULL(U.SURNAME,'Admin') AS SURNAME");
            strQry.AppendLine(",MAX(FUC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");
           
            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_UPLOAD_DETAILS FUD ");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_UPLOAD_DETAILS_FOR_USERS FUDFU ");
            strQry.AppendLine(" ON FUD.USER_NO = FUDFU.USER_NO");
            strQry.AppendLine(" AND FUD.FILE_NAME = FUDFU.FILE_NAME");
            strQry.AppendLine(" AND FUD.UPLOAD_DATETIME = FUDFU.UPLOAD_DATETIME");
            
            strQry.AppendLine(" LEFT JOIN InteractPayroll.dbo.USER_ID U");
            strQry.AppendLine(" ON FUDFU.FOR_USER_NO = U.USER_NO ");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_UPLOAD_CHUNKS FUC ");
            strQry.AppendLine(" ON FUD.USER_NO = FUC.USER_NO");
            strQry.AppendLine(" AND FUD.FILE_NAME = FUC.FILE_NAME");
            strQry.AppendLine(" AND FUD.UPLOAD_DATETIME = FUC.UPLOAD_DATETIME");

            if (parstrCurrentUserAccess != "S")
            {
                strQry.AppendLine(" WHERE FUDFU.FOR_USER_NO = " + parint64CurrentUserNo);
            }

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FUD.USER_NO");
            strQry.AppendLine(",FUD.FILE_NAME");
            strQry.AppendLine(",FUD.UPLOAD_DATETIME");
            strQry.AppendLine(",FUD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FUD.FILE_SIZE");
            strQry.AppendLine(",FUD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FUD.FILE_VERSION_NO");
            strQry.AppendLine(",FUD.FILE_CRC_VALUE");
            strQry.AppendLine(",U.USER_ID");
            strQry.AppendLine(",U.FIRSTNAME");
            strQry.AppendLine(",U.SURNAME");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" FUD.USER_NO");
            strQry.AppendLine(",FUD.UPLOAD_DATETIME DESC");
            strQry.AppendLine(",FUD.FILE_NAME");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "UserFile", -1);
            
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public void Delete_Record(Int64 parint64UserNo,string parstrFileUploadDatetime,string parstrFileName)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();
           
            strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_UPLOAD_DETAILS ");

            strQry.AppendLine(" WHERE USER_NO = " + parint64UserNo);
            strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
            strQry.AppendLine(" AND UPLOAD_DATETIME = '" + parstrFileUploadDatetime + "'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            strQry.Clear();

            strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_UPLOAD_DETAILS_FOR_USERS ");

            strQry.AppendLine(" WHERE USER_NO = " + parint64UserNo);
            strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
            strQry.AppendLine(" AND UPLOAD_DATETIME = '" + parstrFileUploadDatetime + "'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            strQry.Clear();

            strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_UPLOAD_CHUNKS ");

            strQry.AppendLine(" WHERE USER_NO = " + parint64UserNo);
            strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
            strQry.AppendLine(" AND UPLOAD_DATETIME = '" + parstrFileUploadDatetime + "'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }

        public byte[] Get_File_Chunk(Int64 parint64UserNo, string parstrFileUploadDatetime, string parstrFileName,int parintChunkNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FILE_CHUNK_NO");
            strQry.AppendLine(",FILE_CHUNK");
         
            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_UPLOAD_CHUNKS ");

            strQry.AppendLine(" WHERE USER_NO = " + parint64UserNo);
            strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
            strQry.AppendLine(" AND UPLOAD_DATETIME = '" + parstrFileUploadDatetime + "'");
            strQry.AppendLine(" AND FILE_CHUNK_NO = " + parintChunkNo);

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "FileChunk", -1);

            byte[] bytCompress = (byte[]) DataSet.Tables["FileChunk"].Rows[0]["FILE_CHUNK"];
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
    }
}
