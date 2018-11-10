using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace InteractPayroll
{
	public class clsDBConnectionObjects
	{
        private DataSet pvtDataSet;

		private SqlConnection pvtSqlConnection;
		private SqlConnection pvtSqlConnectionClient;
        private SqlCommand pvtSqlCommand;
		private SqlCommand pvtSqlCommandClient;
        private SqlDataAdapter pvtSqlDataAdapter;
		private SqlDataAdapter pvtSqlDataAdapterClient;
        private SqlParameter pvtSqlParameter;
		private SqlParameter pvtSqlParameterClient;
		private SqlTransaction pvtSqlTransaction;
		private SqlTransaction pvtSqlTransactionClient;
        
        private string pvtstrConnection = @"Server=#Engine#;UID=Interact;PWD=erawnacs;MultipleActiveResultSets=True;";
        private string pvtstrConnectionRestore = @"Server=#Engine#;Database=Master;UID=Interact;PWD=erawnacs;MultipleActiveResultSets=True;";
#if(DEBUG)
        private string pvtstrConnectionClient = @"Server=#Engine#;Database=InteractPayrollClient_Debug;Integrated Security=true;MultipleActiveResultSets=True;";
#else
        private string pvtstrConnectionClient = @"Server=#Engine#;Database=InteractPayrollClient;Integrated Security=true;MultipleActiveResultSets=True;";
#endif
        private string pvtstrConnectionClientRestore = @"Server=#Engine#;Database=Master;Integrated Security=true;MultipleActiveResultSets=True;";
       
        public string pvtstrDBEngine = "";
        
        public clsDBConnectionObjects()
		{
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DBConfig.txt") == true)
            {
                StreamReader srStreamReader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "DBConfig.txt");

                string[] strComponents = srStreamReader.ReadLine().Split(';');

                srStreamReader.Close();

                if (strComponents.Length > 0)
                {
                    pvtstrDBEngine = strComponents[0];
                }

                if (strComponents.Length > 1)
                {
                    if (strComponents[1].IndexOf("AttachDBFilename") > -1)
                    {
                        //New AttachDBFilename
                        pvtstrConnectionClient = pvtstrConnectionClient + strComponents[1] + ";";
                    }
                }
            }
            
            pvtstrConnection = pvtstrConnection.Replace("#Engine#", pvtstrDBEngine);
            pvtstrConnectionRestore = pvtstrConnectionRestore.Replace("#Engine#", pvtstrDBEngine);

            pvtstrConnectionClient = pvtstrConnectionClient.Replace("#Engine#", pvtstrDBEngine);
            pvtstrConnectionClientRestore = pvtstrConnectionClientRestore.Replace("#Engine#", pvtstrDBEngine);
        }

        public string Get_Internet_Client_Download_SQL_New(Int64 parInt64UserNo)
        {
            StringBuilder strQry = new StringBuilder();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" '_' + FCDD.PROJECT_VERSION AS PROJECT_VERSION ");
            strQry.AppendLine(",'P' AS FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(",CONVERT(INT,-1) AS COMPANY_NO");
            strQry.AppendLine(",'0' AS FROM_IND");
            strQry.AppendLine(",'N' AS ALWAYS_IND");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FCDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_USERS FDU");
            strQry.AppendLine(" ON FCDD.PROJECT_VERSION = FDU.PROJECT_VERSION");
            strQry.AppendLine(" AND FDU.USER_NO = " + parInt64UserNo.ToString());

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FCDD.PROJECT_VERSION = FDC.PROJECT_VERSION");
            strQry.AppendLine(" AND FCDD.PROGRAM_ID = FDC.PROGRAM_ID");
            strQry.AppendLine(" AND FCDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FCDD.PROJECT_VERSION = 'Beta'");
            strQry.AppendLine(" AND ((FCDD.PROGRAM_ID = 'B'");
            strQry.AppendLine(" AND FCDD.FILE_NAME IN ('URLConfig.txt','clsISUtilities.dll','PasswordChange.dll','DownloadFiles.dll'))");

            strQry.AppendLine(" OR (FCDD.PROGRAM_ID = 'C'");
            strQry.AppendLine(" AND FCDD.FILE_NAME IN ('FileDownload.dll','FileUpload.dll')))");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FCDD.PROJECT_VERSION");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" '_' + FCDD.PROJECT_VERSION AS PROJECT_VERSION ");
            strQry.AppendLine(",'P' AS FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(",CONVERT(INT,-1) AS COMPANY_NO");
            strQry.AppendLine(",'0' AS FROM_IND");
            strQry.AppendLine(",'N' AS ALWAYS_IND");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FCDD");

            //2018-09-08 - Start 
            strQry.AppendLine(" LEFT JOIN ");
            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" FDD.FILE_NAME");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_USERS FDU");
            strQry.AppendLine(" ON FDD.PROJECT_VERSION = FDU.PROJECT_VERSION");
            strQry.AppendLine(" AND FDU.USER_NO = " + parInt64UserNo.ToString());

            strQry.AppendLine(" WHERE FDD.PROJECT_VERSION = 'Beta'");

            strQry.AppendLine(" AND ((FDD.PROGRAM_ID = 'B'");
            strQry.AppendLine(" AND FDD.FILE_NAME IN ('URLConfig.txt','clsISUtilities.dll','PasswordChange.dll','DownloadFiles.dll'))");

            strQry.AppendLine(" OR (FDD.PROGRAM_ID = 'C'");
            strQry.AppendLine(" AND FDD.FILE_NAME IN ('FileDownload.dll','FileUpload.dll')))) AS BETA_TABLE");
            strQry.AppendLine(" ON FCDD.FILE_NAME = BETA_TABLE.FILE_NAME");
            //2018-09-08 - End 

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FCDD.PROJECT_VERSION = FDC.PROJECT_VERSION");
            strQry.AppendLine(" AND FCDD.PROGRAM_ID = FDC.PROGRAM_ID");
            strQry.AppendLine(" AND FCDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FCDD.PROJECT_VERSION = 'Current'");
            strQry.AppendLine(" AND ((FCDD.PROGRAM_ID = 'B'");
            strQry.AppendLine(" AND FCDD.FILE_NAME IN ('URLConfig.txt','clsISUtilities.dll','PasswordChange.dll','DownloadFiles.dll'))");

            strQry.AppendLine(" OR (FCDD.PROGRAM_ID = 'C'");
            strQry.AppendLine(" AND FCDD.FILE_NAME IN ('FileDownload.dll','FileUpload.dll')))");

            //2018-09-08 - Not in Beta
            strQry.AppendLine(" AND BETA_TABLE.FILE_NAME IS NULL");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FCDD.PROJECT_VERSION");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(" UNION");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" '_' + FCDD.PROJECT_VERSION AS PROJECT_VERSION ");
            strQry.AppendLine(",'S' AS FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(",CONVERT(INT,-1) AS COMPANY_NO");
            strQry.AppendLine(",'0' AS FROM_IND");
            strQry.AppendLine(",'N' AS ALWAYS_IND");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FCDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_USERS FDU");
            strQry.AppendLine(" ON FCDD.PROJECT_VERSION = FDU.PROJECT_VERSION");
            strQry.AppendLine(" AND FDU.USER_NO = " + parInt64UserNo.ToString());

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FCDD.PROJECT_VERSION = FDC.PROJECT_VERSION");
            strQry.AppendLine(" AND FCDD.PROGRAM_ID = FDC.PROGRAM_ID");
            strQry.AppendLine(" AND FCDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FCDD.PROJECT_VERSION = 'Beta'");
            strQry.AppendLine(" AND FCDD.PROGRAM_ID = 'B'");
            strQry.AppendLine(" AND FCDD.FILE_NAME = 'URLConfig.txt'");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FCDD.PROJECT_VERSION");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(" UNION");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" '_' + FCDD.PROJECT_VERSION AS PROJECT_VERSION ");
            strQry.AppendLine(",'S' AS FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(",CONVERT(INT,-1) AS COMPANY_NO");
            strQry.AppendLine(",'0' AS FROM_IND");
            strQry.AppendLine(",'N' AS ALWAYS_IND");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FCDD");

            //2018-09-08 - Start 
            strQry.AppendLine(" LEFT JOIN ");
            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" FDD.FILE_NAME");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_USERS FDU");
            strQry.AppendLine(" ON FDD.PROJECT_VERSION = FDU.PROJECT_VERSION");
            strQry.AppendLine(" AND FDU.USER_NO = " + parInt64UserNo.ToString());

            strQry.AppendLine(" WHERE FDD.PROJECT_VERSION = 'Beta'");

            strQry.AppendLine(" AND FDD.PROGRAM_ID = 'B'");
            strQry.AppendLine(" AND FDD.FILE_NAME = 'URLConfig.txt') AS BETA_TABLE");
            strQry.AppendLine(" ON FCDD.FILE_NAME = BETA_TABLE.FILE_NAME");
            //2018-09-08 - End 

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FCDD.PROJECT_VERSION = FDC.PROJECT_VERSION");
            strQry.AppendLine(" AND FCDD.PROGRAM_ID = FDC.PROGRAM_ID");
            strQry.AppendLine(" AND FCDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FCDD.PROJECT_VERSION = 'Current'");
            strQry.AppendLine(" AND FCDD.PROGRAM_ID = 'B'");
            strQry.AppendLine(" AND FCDD.FILE_NAME = 'URLConfig.txt'");


            //2018-09-08 - Not in Beta
            strQry.AppendLine(" AND BETA_TABLE.FILE_NAME IS NULL");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FCDD.PROJECT_VERSION");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(" UNION");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" '_' + FCDD.PROJECT_VERSION ");
            strQry.AppendLine(",FCDD.FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(",CONVERT(INT,-1) AS COMPANY_NO");
            strQry.AppendLine(",'0' AS FROM_IND");
            strQry.AppendLine(",'N' AS ALWAYS_IND");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS FCDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_USERS FDU");
            strQry.AppendLine(" ON FCDD.PROJECT_VERSION = FDU.PROJECT_VERSION");
            strQry.AppendLine(" AND FDU.USER_NO = " + parInt64UserNo.ToString());

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FCDD.FILE_LAYER_IND = FDC.FILE_LAYER_IND");
            strQry.AppendLine(" AND FCDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FCDD.PROJECT_VERSION = 'Beta'");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FCDD.PROJECT_VERSION ");
            strQry.AppendLine(",FCDD.FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(" UNION");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" '_' + FCDD.PROJECT_VERSION ");
            strQry.AppendLine(",FCDD.FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(",CONVERT(INT,-1) AS COMPANY_NO");
            strQry.AppendLine(",'0' AS FROM_IND");
            strQry.AppendLine(",'N' AS ALWAYS_IND");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS FCDD");

            //2018-09-08 - Start 
            strQry.AppendLine(" LEFT JOIN ");
            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" FDD.FILE_NAME");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS FDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_USERS FDU");
            strQry.AppendLine(" ON FDD.PROJECT_VERSION = FDU.PROJECT_VERSION");
            strQry.AppendLine(" AND FDU.USER_NO = " + parInt64UserNo.ToString());

            strQry.AppendLine(" WHERE FDD.PROJECT_VERSION = 'Beta') AS BETA_TABLE");
            strQry.AppendLine(" ON FCDD.FILE_NAME = BETA_TABLE.FILE_NAME");
            //2018-09-08 - End 

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FCDD.FILE_LAYER_IND = FDC.FILE_LAYER_IND");
            strQry.AppendLine(" AND FCDD.FILE_NAME = FDC.FILE_NAME");


            strQry.AppendLine(" WHERE FCDD.PROJECT_VERSION = 'Current'");

            //2018-09-08 - Not in Beta
            strQry.AppendLine(" AND BETA_TABLE.FILE_NAME IS NULL");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FCDD.PROJECT_VERSION ");
            strQry.AppendLine(",FCDD.FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            //2016-08-12
            strQry.AppendLine(" UNION");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" '_' + FCDD.PROJECT_VERSION ");
            strQry.AppendLine(",'P' AS FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(",CONVERT(INT,-1) AS COMPANY_NO");
            strQry.AppendLine(",'0' AS FROM_IND");
            strQry.AppendLine(",'N' AS ALWAYS_IND");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS FCDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_USERS FDU");
            strQry.AppendLine(" ON FCDD.PROJECT_VERSION = FDU.PROJECT_VERSION");
            strQry.AppendLine(" AND FDU.USER_NO = " + parInt64UserNo.ToString());

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FCDD.FILE_LAYER_IND = FDC.FILE_LAYER_IND");
            strQry.AppendLine(" AND FCDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FCDD.PROJECT_VERSION = 'Beta'");
            strQry.AppendLine(" AND (FCDD.FILE_NAME = 'FingerPrintClockService.dll'");
            strQry.AppendLine(" OR FCDD.FILE_NAME = 'FingerPrintClockServiceStartStop.dll')");
            strQry.AppendLine(" AND FCDD.FILE_LAYER_IND = 'S'");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FCDD.PROJECT_VERSION ");
            strQry.AppendLine(",FCDD.FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            //2012-10-20 Need to Look At
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(" UNION");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" '_' + FCDD.PROJECT_VERSION ");
            strQry.AppendLine(",'P' AS FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(",CONVERT(INT,-1) AS COMPANY_NO");
            strQry.AppendLine(",'0' AS FROM_IND");
            strQry.AppendLine(",'N' AS ALWAYS_IND");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS FCDD");

            //2018-09-08 - Start 
            strQry.AppendLine(" LEFT JOIN ");
            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" FDD.FILE_NAME");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS FDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_USERS FDU");
            strQry.AppendLine(" ON FDD.PROJECT_VERSION = FDU.PROJECT_VERSION");
            strQry.AppendLine(" AND FDU.USER_NO = " + parInt64UserNo.ToString());

            strQry.AppendLine(" WHERE FDD.PROJECT_VERSION = 'Beta'");
            strQry.AppendLine(" AND (FDD.FILE_NAME = 'FingerPrintClockService.dll'");
            strQry.AppendLine(" OR FDD.FILE_NAME = 'FingerPrintClockServiceStartStop.dll')");
            strQry.AppendLine(" AND FDD.FILE_LAYER_IND = 'S') AS BETA_TABLE");
            strQry.AppendLine(" ON FCDD.FILE_NAME = BETA_TABLE.FILE_NAME");
            //2018-09-08 - End 

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FCDD.FILE_LAYER_IND = FDC.FILE_LAYER_IND");
            strQry.AppendLine(" AND FCDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FCDD.PROJECT_VERSION = 'Current'");
            strQry.AppendLine(" AND (FCDD.FILE_NAME = 'FingerPrintClockService.dll'");
            strQry.AppendLine(" OR FCDD.FILE_NAME = 'FingerPrintClockServiceStartStop.dll')");
            strQry.AppendLine(" AND FCDD.FILE_LAYER_IND = 'S'");

            //2018-09-08 - Not in Beta
            strQry.AppendLine(" AND BETA_TABLE.FILE_NAME IS NULL");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FCDD.PROJECT_VERSION ");
            strQry.AppendLine(",FCDD.FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            //2012-10-20 Need to Look At
            strQry.AppendLine(",FILE_CRC_VALUE");

            return strQry.ToString();
        }

        public string Get_Internet_Client_Download_SQL(Int64 parInt64UserNo)
        {
            StringBuilder strQry = new StringBuilder();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" '_Client' AS PROJECT_VERSION ");
            strQry.AppendLine(",'P' AS FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(",CONVERT(INT,-1) AS COMPANY_NO");
            strQry.AppendLine(",'0' AS FROM_IND");
            strQry.AppendLine(",'N' AS ALWAYS_IND");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FCDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FCDD.PROJECT_VERSION = FDC.PROJECT_VERSION");
            strQry.AppendLine(" AND FCDD.PROGRAM_ID = FDC.PROGRAM_ID");
            strQry.AppendLine(" AND FCDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FCDD.PROJECT_VERSION = 'Current'");
            strQry.AppendLine(" AND (FCDD.PROGRAM_ID = 'B'");
            strQry.AppendLine(" AND FCDD.FILE_NAME IN ('URLConfig.txt','clsISUtilities.dll','PasswordChange.dll','DownloadFiles.dll'))");

            strQry.AppendLine(" OR (FCDD.PROGRAM_ID = 'C'");
            strQry.AppendLine(" AND FCDD.FILE_NAME IN ('FileDownload.dll','FileUpload.dll'))");
            
            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(" UNION");
            
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" '_Client' AS PROJECT_VERSION ");
            strQry.AppendLine(",'S' AS FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(",CONVERT(INT,-1) AS COMPANY_NO");
            strQry.AppendLine(",'0' AS FROM_IND");
            strQry.AppendLine(",'N' AS ALWAYS_IND");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FCDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FCDD.PROJECT_VERSION = FDC.PROJECT_VERSION");
            strQry.AppendLine(" AND FCDD.PROGRAM_ID = FDC.PROGRAM_ID");
            strQry.AppendLine(" AND FCDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FCDD.PROJECT_VERSION = 'Current'");
            strQry.AppendLine(" AND FCDD.PROGRAM_ID = 'B'");
            strQry.AppendLine(" AND FCDD.FILE_NAME = 'URLConfig.txt'");
            
            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");
            
            strQry.AppendLine(" UNION");
            
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" '_Client' AS PROJECT_VERSION ");
            strQry.AppendLine(",FCDD.FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(",CONVERT(INT,-1) AS COMPANY_NO");
            strQry.AppendLine(",'0' AS FROM_IND");
            strQry.AppendLine(",'N' AS ALWAYS_IND");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS FCDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FCDD.FILE_LAYER_IND = FDC.FILE_LAYER_IND");
            strQry.AppendLine(" AND FCDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FCDD.FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            //2016-08-12
            strQry.AppendLine(" UNION");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" '_Client' AS PROJECT_VERSION ");
            strQry.AppendLine(",'P' AS FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(",CONVERT(INT,-1) AS COMPANY_NO");
            strQry.AppendLine(",'0' AS FROM_IND");
            strQry.AppendLine(",'N' AS ALWAYS_IND");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS FCDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FCDD.FILE_LAYER_IND = FDC.FILE_LAYER_IND");
            strQry.AppendLine(" AND FCDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE (FCDD.FILE_NAME = 'FingerPrintClockService.dll'");
            strQry.AppendLine(" OR FCDD.FILE_NAME = 'FingerPrintClockServiceStartStop.dll')");
            strQry.AppendLine(" AND FCDD.FILE_LAYER_IND = 'S'");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FCDD.FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            //2012-10-20 Need to Look At
            strQry.AppendLine(",FILE_CRC_VALUE");

            return strQry.ToString();
        }
        public string Get_Local_Client_Download_SQL()
        {
            string strQry = "";

            strQry = "";
            strQry += " SELECT ";
            strQry += " FILE_LAYER_IND";
            strQry += ",FILE_NAME";
            strQry += ",FILE_LAST_UPDATED_DATE";

            strQry += " FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_DETAILS";

            return strQry;
        }

        public string GetSQLConnectionString()
        {
            return pvtstrDBEngine;
        }
        
        public byte[] Compress_DataSet(DataSet parDataSet)
        {
            try
            {
                parDataSet.RemotingFormat = SerializationFormat.Binary;

                MemoryStream msMemoryStream = new MemoryStream();
                MemoryStream msMemoryStreamCompressed = new MemoryStream();
                
                BinaryFormatter bfBinaryFormatter = new BinaryFormatter();
                bfBinaryFormatter.Serialize(msMemoryStream, parDataSet);
               
                System.IO.Compression.GZipStream GZipStreamCompressed = new GZipStream(msMemoryStreamCompressed, CompressionMode.Compress, true);
                GZipStreamCompressed.Write(msMemoryStream.ToArray(), 0, (int)msMemoryStream.Length);
                GZipStreamCompressed.Flush();
                GZipStreamCompressed.Close();

                return msMemoryStreamCompressed.ToArray();
            }
            catch
            {
                byte[] byte1 = new byte[1];
                return byte1;
            }
        }

        public DataSet DeCompress_Array_To_DataSet(byte[] parbytArray)
        {
            try
            {
                pvtDataSet = new DataSet();
                pvtDataSet.RemotingFormat = SerializationFormat.Binary;

                MemoryStream msMemoryStreamCompressed = new MemoryStream(parbytArray);
                System.IO.Compression.GZipStream GZipStreamCompressed = new GZipStream(msMemoryStreamCompressed, CompressionMode.Decompress, true);

                byte[] byteOut = ReadFullStream(GZipStreamCompressed);
                GZipStreamCompressed.Flush();
                GZipStreamCompressed.Close();

                MemoryStream msMemoryStreamTest = new MemoryStream(byteOut);
                                
                BinaryFormatter bf = new BinaryFormatter();
                pvtDataSet = (DataSet)bf.Deserialize(msMemoryStreamTest, null);

                return pvtDataSet;
            }
            catch 
            {
                DataSet DataSet = new DataSet();
                return DataSet;
            }
        }

        private byte[] ReadFullStream(Stream stream)
        {
            byte[] bytBuffer = new byte[32768];

            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int intCharCount = stream.Read(bytBuffer, 0, bytBuffer.Length);
                    
                    if (intCharCount <= 0)
                    {
                        return ms.ToArray();
                    }
                    else
                    {
                        ms.Write(bytBuffer, 0, intCharCount);
                    }
                }
            }
        }

		public string Text2DynamicSQL(string parstrText)
		{
            //Remove Leading/Trailing Spaces
			string strText = parstrText.Trim();
			//Duplicate Single Quote
			strText = strText.Replace("'","''");

			if (strText == "")
			{
				return "Null";
			}
			else
			{
				return "\u0027"  + strText +  "\u0027";
			}
		}

		public string Get_ClientConnectionString()
		{
            return pvtstrConnectionClient;
		}

        public string Get_ConnectionString()
        {
            return pvtstrConnection;
        }

        public void Set_ConnectionString(string parstrConnection)
        {
            pvtstrConnection = parstrConnection;
        }

        public void Set_ClientConnectionString(string parstrConnection)
        {
            pvtstrConnectionClient = parstrConnection;
        }
		
		public void Initialise_DataSet_Numeric_Fields(string parstrTableName,ref string parstrQuery,ref string parstrFieldNamesInitialised,Int64 parInt64CompanyNo)
		{
			int intReturnCode = -1;

            string strQry = "";
            DataSet DataSet = new DataSet();

            //NB Connection To Table different (No .dbo.)
            strQry = "";
            strQry += " SELECT ";
            strQry += " C.COLUMN_NAME";
            strQry += ",C.DATA_TYPE";
            
            strQry += " FROM InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.TABLES T";
            
            strQry += " INNER JOIN InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.COLUMNS C";
            strQry += " ON T.TABLE_NAME = C.TABLE_NAME";
            
            strQry += " WHERE T.TABLE_TYPE = 'BASE TABLE'";
            strQry += " AND T.TABLE_NAME = '" + parstrTableName + "'";
            strQry += " AND NOT C.COLUMN_NAME = 'USER_NO_NEW_RECORD'";
            strQry += " AND NOT C.COLUMN_NAME = 'USER_NO_RECORD'";
            strQry += " AND DATA_TYPE IN"; 
            strQry += " ('SMALLMONEY'"; 
            strQry += " ,'MONEY'"; 
            strQry += " ,'FLOAT'"; 
            strQry += " ,'DECIMAL'"; 
            strQry += " ,'SMALLINT'"; 
            strQry += " ,'INT'"; 
            strQry += " ,'TINYINT'";
            strQry += " ,'BIGINT')"; 

            Create_DataTable(strQry, DataSet, "ColumnName",parInt64CompanyNo);

            bool blnFieldFound = false;

            for (int intRow = 0; intRow < DataSet.Tables["ColumnName"].Rows.Count; intRow++)
            {
                blnFieldFound = false;

                string[] myNames = parstrQuery.Split(',');

                if (myNames.Length > 0)
                {
                    myNames[0] = myNames[0].Trim();
                }

                for (int intRowField = 0; intRowField < myNames.Length; intRowField++)
                {
                    if (myNames[intRowField].Trim() == DataSet.Tables["ColumnName"].Rows[intRow]["COLUMN_NAME"].ToString())
                    {
                        blnFieldFound = true;
                        break;
                    }
                    else
                    {
                        if (myNames[0].Substring(myNames[0].Length - DataSet.Tables["ColumnName"].Rows[intRow]["COLUMN_NAME"].ToString().Length) == DataSet.Tables["ColumnName"].Rows[intRow]["COLUMN_NAME"].ToString())
                        {
                            blnFieldFound = true;
                            break;
                        }
                    }
                }

                if (blnFieldFound == false)
                {
                    parstrQuery += "," + DataSet.Tables["ColumnName"].Rows[intRow]["COLUMN_NAME"].ToString();
                    parstrFieldNamesInitialised += ",0";
                }
			}
		}

        public void Initialise_DataSet_Numeric_Fields(string parstrTableName, ref StringBuilder parstrQuery, ref StringBuilder parstrFieldNamesInitialised, Int64 parInt64CompanyNo)
        {
            int intReturnCode = -1;

            string strQry = "";
            DataSet DataSet = new DataSet();

            //NB Connection To Table different (No .dbo.)
            strQry = "";
            strQry += " SELECT ";
            strQry += " C.COLUMN_NAME";
            strQry += ",C.DATA_TYPE";

            strQry += " FROM InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.TABLES T";

            strQry += " INNER JOIN InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.COLUMNS C";
            strQry += " ON T.TABLE_NAME = C.TABLE_NAME";

            strQry += " WHERE T.TABLE_TYPE = 'BASE TABLE'";
            strQry += " AND T.TABLE_NAME = '" + parstrTableName + "'";
            strQry += " AND NOT C.COLUMN_NAME = 'USER_NO_NEW_RECORD'";
            strQry += " AND NOT C.COLUMN_NAME = 'USER_NO_RECORD'";
            strQry += " AND DATA_TYPE IN";
            strQry += " ('SMALLMONEY'";
            strQry += " ,'MONEY'";
            strQry += " ,'FLOAT'";
            strQry += " ,'DECIMAL'";
            strQry += " ,'SMALLINT'";
            strQry += " ,'INT'";
            strQry += " ,'TINYINT'";
            strQry += " ,'BIGINT')";

            Create_DataTable(strQry, DataSet, "ColumnName", parInt64CompanyNo);

            bool blnFieldFound = false;

            string strQuery = parstrQuery.ToString().Replace("\r", "").Replace("\n", "");

            for (int intRow = 0; intRow < DataSet.Tables["ColumnName"].Rows.Count; intRow++)
            {
                blnFieldFound = false;

                string[] myNames = strQuery.Split(',');

                if (myNames.Length > 0)
                {
                    myNames[0] = myNames[0].Trim();
                }

                for (int intRowField = 0; intRowField < myNames.Length; intRowField++)
                {
                    if (myNames[intRowField].Trim() == DataSet.Tables["ColumnName"].Rows[intRow]["COLUMN_NAME"].ToString())
                    {
                        blnFieldFound = true;
                        break;
                    }
                    else
                    {
                        if (myNames[0].Substring(myNames[0].Length - DataSet.Tables["ColumnName"].Rows[intRow]["COLUMN_NAME"].ToString().Length) == DataSet.Tables["ColumnName"].Rows[intRow]["COLUMN_NAME"].ToString())
                        {
                            blnFieldFound = true;
                            break;
                        }
                    }
                }

                if (blnFieldFound == false)
                {
                    parstrQuery.AppendLine("," + DataSet.Tables["ColumnName"].Rows[intRow]["COLUMN_NAME"].ToString());
                    parstrFieldNamesInitialised.AppendLine(",0");
                }
            }
        }

		public void Initialise_DataSet_Name_Fields(string parstrTableName,ref string parstrQuery,ref string parstrQuerySecondPart,string parstrQuerySecondPartPrefix,Int64 parInt64CompanyNo)
		{
            //Make Sure string is Empty
            parstrQuerySecondPart = "";
            string strQry = "";
            DataSet DataSet = new DataSet();

            //NB Connection To Table different (No .dbo.)
            strQry = "";
            strQry += " SELECT ";
            strQry += " C.COLUMN_NAME";
            
            strQry += " FROM InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.TABLES T";
            
            strQry += " INNER JOIN InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.COLUMNS C";
            strQry += " ON T.TABLE_NAME = C.TABLE_NAME";
            
            strQry += " WHERE T.TABLE_TYPE = 'BASE TABLE'";
            strQry += " AND T.TABLE_NAME = '" + parstrTableName +"'";

            Create_DataTable(strQry, DataSet, "ColumnName", parInt64CompanyNo);

            int intReturnCode = -1;

            for (int intRow = 0; intRow < DataSet.Tables["ColumnName"].Rows.Count; intRow++)
            {
                //ELR - 2015-03-21
                if (DataSet.Tables["ColumnName"].Rows[intRow]["COLUMN_NAME"].ToString() == "INCLUDED_IN_RUN_IND")
                {
                    continue;
                }

            	intReturnCode = parstrQuery.TrimEnd().IndexOf(DataSet.Tables["ColumnName"].Rows[intRow]["COLUMN_NAME"].ToString());

				if (intReturnCode != -1)
				{
					if (parstrQuery.TrimEnd().Length == intReturnCode + DataSet.Tables["ColumnName"].Rows[intRow]["COLUMN_NAME"].ToString().Length)
					{
						continue;
					}
					else
					{
						if (parstrQuery.Substring(intReturnCode + DataSet.Tables["ColumnName"].Rows[intRow]["COLUMN_NAME"].ToString().Length,1) == ","
							| parstrQuery.Substring(intReturnCode + DataSet.Tables["ColumnName"].Rows[intRow]["COLUMN_NAME"].ToString().Length,1) == " ")
						{
							continue;
						}
					}
				}

				parstrQuery += "," + DataSet.Tables["ColumnName"].Rows[intRow]["COLUMN_NAME"].ToString();

				if (parstrQuerySecondPartPrefix == "")
				{
					parstrQuerySecondPart += "," + DataSet.Tables["ColumnName"].Rows[intRow]["COLUMN_NAME"].ToString();
				}
				else
				{
					parstrQuerySecondPart += "," + parstrQuerySecondPartPrefix + "." + DataSet.Tables["ColumnName"].Rows[intRow]["COLUMN_NAME"].ToString();
				}
			}
		}

        public void Initialise_DataSet_Name_Fields(string parstrTableName, ref StringBuilder parstrQuery, ref StringBuilder parstrQuerySecondPart, string parstrQuerySecondPartPrefix, Int64 parInt64CompanyNo)
        {
            //Make Sure string is Empty
            parstrQuerySecondPart.Clear();
            string strQry = "";
            DataSet DataSet = new DataSet();

            //NB Connection To Table different (No .dbo.)
            strQry = "";
            strQry += " SELECT ";
            strQry += " C.COLUMN_NAME";

            strQry += " FROM InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.TABLES T";

            strQry += " INNER JOIN InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.COLUMNS C";
            strQry += " ON T.TABLE_NAME = C.TABLE_NAME";

            strQry += " WHERE T.TABLE_TYPE = 'BASE TABLE'";
            strQry += " AND T.TABLE_NAME = '" + parstrTableName + "'";

            Create_DataTable(strQry, DataSet, "ColumnName", parInt64CompanyNo);

            int intReturnCode = -1;

            string strQuery = parstrQuery.ToString().Replace("\r", "").Replace("\n", "");

            for (int intRow = 0; intRow < DataSet.Tables["ColumnName"].Rows.Count; intRow++)
            {
                //ELR - 2015-03-21
                if (DataSet.Tables["ColumnName"].Rows[intRow]["COLUMN_NAME"].ToString() == "INCLUDED_IN_RUN_IND")
                {
                    continue;
                }

                intReturnCode = strQuery.TrimEnd().IndexOf(DataSet.Tables["ColumnName"].Rows[intRow]["COLUMN_NAME"].ToString());

                if (intReturnCode != -1)
                {
                    if (strQuery.ToString().TrimEnd().Length == intReturnCode + DataSet.Tables["ColumnName"].Rows[intRow]["COLUMN_NAME"].ToString().Length)
                    {
                        continue;
                    }
                    else
                    {
                        if (strQuery.ToString().Substring(intReturnCode + DataSet.Tables["ColumnName"].Rows[intRow]["COLUMN_NAME"].ToString().Length, 1) == ","
                            | strQuery.ToString().Substring(intReturnCode + DataSet.Tables["ColumnName"].Rows[intRow]["COLUMN_NAME"].ToString().Length, 1) == " ")
                        {
                            continue;
                        }
                    }
                }

                parstrQuery.AppendLine("," + DataSet.Tables["ColumnName"].Rows[intRow]["COLUMN_NAME"].ToString());

                if (parstrQuerySecondPartPrefix == "")
                {
                    parstrQuerySecondPart.AppendLine("," + DataSet.Tables["ColumnName"].Rows[intRow]["COLUMN_NAME"].ToString());
                }   
                else
                {
                    parstrQuerySecondPart.AppendLine("," + parstrQuerySecondPartPrefix + "." + DataSet.Tables["ColumnName"].Rows[intRow]["COLUMN_NAME"].ToString());
                }
            }
        }

        public void Create_DataTable(string parstrQry, DataSet parDataSet, string parstrSourceTable, Int64 parInt64CompanyNo)
        {
            parstrQry = parstrQry.Replace("#CompanyNo#",parInt64CompanyNo.ToString("00000"));

            try
            {
                pvtSqlConnection = new SqlConnection(pvtstrConnection);

                pvtSqlCommand = new SqlCommand(parstrQry, pvtSqlConnection);

                pvtSqlDataAdapter = new SqlDataAdapter(pvtSqlCommand);
                            
                pvtSqlDataAdapter.Fill(parDataSet, parstrSourceTable);

                pvtSqlConnection.Close();

                parDataSet.AcceptChanges();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (pvtSqlConnection != null)
                {
                    pvtSqlConnection.Dispose();
                }

                if (pvtSqlCommand != null)
                {
                    pvtSqlCommand.Dispose();
                }
            }
        }

        public void Create_DataTable(string parstrQry, DataSet parDataSet, string parstrSourceTable, Int64 parInt64CompanyNo, int parintTimeOut)
        {
            parstrQry = parstrQry.Replace("#CompanyNo#", parInt64CompanyNo.ToString("00000"));

            try
            {
                pvtSqlConnection = new SqlConnection(pvtstrConnection + "Connection Timeout=" + parintTimeOut.ToString());

                pvtSqlCommand = new SqlCommand(parstrQry, pvtSqlConnection);
                pvtSqlCommand.CommandTimeout = parintTimeOut;

                pvtSqlDataAdapter = new SqlDataAdapter(pvtSqlCommand);

                //Opens and Closes the Connection object - pvtSqlConnection
                pvtSqlDataAdapter.Fill(parDataSet, parstrSourceTable);

                pvtSqlConnection.Close();

                parDataSet.AcceptChanges();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (pvtSqlConnection != null)
                {
                    pvtSqlConnection.Dispose();
                }

                if (pvtSqlCommand != null)
                {
                    pvtSqlCommand.Dispose();
                }
            }
        }

        public void Round_For_Period(int parintRoundInd, int parintRoundValue, ref int parintTotal)
        {
            if (parintRoundInd == 0)
            {
            }
            else
            {
                if (parintTotal % parintRoundValue == 0)
                {
                }
                else
                {
                    //Up
                    if (parintRoundInd == 1)
                    {
                        parintTotal = parintTotal + (parintRoundValue - (parintTotal % parintRoundValue));
                    }
                    else
                    {
                        //Down
                        if (parintRoundInd == 2)
                        {
                            parintTotal = parintTotal - (parintTotal % parintRoundValue);
                        }
                        else
                        {
                            //Closest
                            if (parintTotal % parintRoundValue >= Convert.ToDouble(parintRoundValue) / 2)
                            {
                                //Up
                                parintTotal = parintTotal + (parintRoundValue - (parintTotal % parintRoundValue));
                            }
                            else
                            {
                                //Down
                                parintTotal = parintTotal - (parintTotal % parintRoundValue);
                            }
                        }
                    }
                }
            }
        }
     
        public void Check_Client_Triggers()
        {
            DataSet DataSet = new System.Data.DataSet();

            StringBuilder strQry = new StringBuilder();

#if (DEBUG)
            strQry.AppendLine(" USE InteractPayrollClient_Debug ");
#else
            strQry.AppendLine(" USE InteractPayrollClient ");
#endif
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" TriggerName = t.name");
            strQry.AppendLine(",Defininion  = object_definition(t.object_id)");

            strQry.AppendLine(" FROM sys.triggers t ");

            strQry.AppendLine(" LEFT JOIN sys.all_objects o ");
            strQry.AppendLine(" ON t.parent_id = o.object_id ");

            strQry.AppendLine(" LEFT JOIN sys.schemas s ");
            strQry.AppendLine(" ON s.schema_id = o.schema_id ");

            Create_DataTable_Client(strQry.ToString(), DataSet, "Trigger");

            DataSet.AcceptChanges();

            bool blnCreateTrigger = true;

            DataView tempDataView = new DataView(DataSet.Tables["Trigger"],
                                                       "TriggerName = 'tgr_Create_Timesheet'",
                                                       "",
                                                      DataViewRowState.CurrentRows);

            if (tempDataView.Count > 0)
            {
                //Logic to Check Version No
                if (tempDataView[0]["Defininion"].ToString().IndexOf("--Version 1.5") > 0)
                {
                    blnCreateTrigger = false;
                }
                else
                {
                    strQry.Clear();

                    strQry.AppendLine(" USE InteractPayrollClient ");
                    strQry.AppendLine(" DROP TRIGGER dbo.tgr_Create_Timesheet ");

                    Execute_SQLCommand_Client(strQry.ToString());
                }
            }

            if (blnCreateTrigger == true)
            {
                //Create TRIGGER tgr_Create_Timesheet
                strQry.Clear();

                strQry.Append(Client_tgr_Create_Timesheet());

                Execute_SQLCommand_Client(strQry.ToString());
            }

            blnCreateTrigger = true;

            tempDataView = null;
            tempDataView = new DataView(DataSet.Tables["Trigger"],
                "TriggerName = 'tgr_Create_Break'",
                "",
               DataViewRowState.CurrentRows);

            if (tempDataView.Count > 0)
            {
                if (tempDataView[0]["Defininion"].ToString().IndexOf("--Version 1.5") > 0)
                {
                    blnCreateTrigger = false;
                }
                else
                {
                    strQry.Clear();

                    strQry.AppendLine(" USE InteractPayrollClient ");
                    strQry.AppendLine(" DROP TRIGGER dbo.tgr_Create_Break ");

                    Execute_SQLCommand_Client(strQry.ToString());
                }
            }

            if (blnCreateTrigger == true)
            {
                //Create TRIGGER tgr_Create_Break
                strQry.Clear();

                strQry.Append(Client_tgr_Create_Break());

                Execute_SQLCommand_Client(strQry.ToString());
            }

            blnCreateTrigger = true;

            tempDataView = null;
            tempDataView = new DataView(DataSet.Tables["Trigger"],
                "TriggerName = 'tgr_Client_EMPLOYEE_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table'",
                "",
               DataViewRowState.CurrentRows);

            if (tempDataView.Count > 0)
            {
                if (tempDataView[0]["Defininion"].ToString().IndexOf("--Version 1.6") > 0)
                {
                    blnCreateTrigger = false;
                }
                else
                {
                    strQry.Clear();

                    strQry.AppendLine(" USE InteractPayrollClient ");
                    strQry.AppendLine(" DROP TRIGGER dbo.tgr_Client_EMPLOYEE_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine(" USE InteractPayrollClient ");
                    strQry.AppendLine(" DROP TRIGGER dbo.tgr_Client_EMPLOYEE_BREAK_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine(" USE InteractPayrollClient ");
                    strQry.AppendLine(" DROP TRIGGER dbo.tgr_Client_EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine(" USE InteractPayrollClient ");
                    strQry.AppendLine(" DROP TRIGGER dbo.tgr_Client_EMPLOYEE_TIME_ATTEND_BREAK_CURRENT_Maintain_EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine(" USE InteractPayrollClient ");
                    strQry.AppendLine(" DROP TRIGGER dbo.tgr_Client_EMPLOYEE_SALARY_TIMESHEET_CURRENT_Maintain_EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                    Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine(" USE InteractPayrollClient ");
                    strQry.AppendLine(" DROP TRIGGER dbo.tgr_Client_EMPLOYEE_SALARY_BREAK_CURRENT_Maintain_EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT_Table ");

                    Execute_SQLCommand_Client(strQry.ToString());
                }
            }

            if (blnCreateTrigger == true)
            {
                strQry.Clear();

                strQry.Append(tgr_Client_Create_For_Timesheet_Break_Table("EMPLOYEE_TIMESHEET_CURRENT"));

                Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();

                strQry.Append(tgr_Client_Create_For_Timesheet_Break_Table("EMPLOYEE_BREAK_CURRENT"));

                Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();

                strQry.Append(tgr_Client_Create_For_Timesheet_Break_Table("EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT"));

                Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();

                strQry.Append(tgr_Client_Create_For_Timesheet_Break_Table("EMPLOYEE_TIME_ATTEND_BREAK_CURRENT"));

                Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();

                strQry.Append(tgr_Client_Create_For_Timesheet_Break_Table("EMPLOYEE_SALARY_TIMESHEET_CURRENT"));

                Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();

                strQry.Append(tgr_Client_Create_For_Timesheet_Break_Table("EMPLOYEE_SALARY_BREAK_CURRENT"));

                Execute_SQLCommand_Client(strQry.ToString());
            }
        }

        public string tgr_Client_Create_For_Timesheet_Break_Table(string triggerTableName)
        {
            string strPayCategoryType = "";
            string strDayTable = "";
            string strTimeSheetTable = "";
            string strBreakTable = "";
            string strDateField = "";
            string strTimeSheetOrBreak = "";

            if (triggerTableName == "EMPLOYEE_TIMESHEET_CURRENT"
            || triggerTableName == "EMPLOYEE_BREAK_CURRENT")
            {
                strPayCategoryType = "W";
                strDayTable = "EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT";

                strTimeSheetTable = "EMPLOYEE_TIMESHEET_CURRENT";
                strBreakTable = "EMPLOYEE_BREAK_CURRENT";

                if (triggerTableName == "EMPLOYEE_TIMESHEET_CURRENT")
                {
                    strDateField = "TIMESHEET_DATE";
                    strTimeSheetOrBreak = "TIMESHEET";
                }
                else
                {
                    strDateField = "BREAK_DATE";
                    strTimeSheetOrBreak = "BREAK";
                }
            }
            else
            {
                if (triggerTableName == "EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT"
                || triggerTableName == "EMPLOYEE_TIME_ATTEND_BREAK_CURRENT")
                {
                    strPayCategoryType = "T";
                    strDayTable = "EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT";

                    strTimeSheetTable = "EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT";
                    strBreakTable = "EMPLOYEE_TIME_ATTEND_BREAK_CURRENT";

                    if (triggerTableName == "EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT")
                    {
                        strDateField = "TIMESHEET_DATE";
                        strTimeSheetOrBreak = "TIMESHEET";
                    }
                    else
                    {
                        strDateField = "BREAK_DATE";
                        strTimeSheetOrBreak = "BREAK";
                    }
                }
                else
                {
                    if (triggerTableName == "EMPLOYEE_SALARY_TIMESHEET_CURRENT"
                    || triggerTableName == "EMPLOYEE_SALARY_BREAK_CURRENT")
                    {
                        strPayCategoryType = "S";
                        strDayTable = "EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT";

                        strTimeSheetTable = "EMPLOYEE_SALARY_TIMESHEET_CURRENT";
                        strBreakTable = "EMPLOYEE_SALARY_BREAK_CURRENT";

                        if (triggerTableName == "EMPLOYEE_SALARY_TIMESHEET_CURRENT")
                        {
                            strDateField = "TIMESHEET_DATE";
                            strTimeSheetOrBreak = "TIMESHEET";
                        }
                        else
                        {
                            strDateField = "BREAK_DATE";
                            strTimeSheetOrBreak = "BREAK";
                        }
                    }
                    else
                    {
                        return "Invalid Trigger Table Name";
                    }
                }
            }

            StringBuilder strQry = new StringBuilder();

            strQry.AppendLine("CREATE TRIGGER [dbo].[tgr_Client_#TRIGGER_TABLE_NAME#_Maintain_#DAY_TABLE#_Table]");
            strQry.AppendLine("ON [dbo].[#TRIGGER_TABLE_NAME#]");
            strQry.AppendLine("FOR INSERT, UPDATE, DELETE");
            strQry.AppendLine("AS");
            strQry.AppendLine("--Version 1.6");
            strQry.AppendLine("DECLARE @Action AS char(1)");
            strQry.AppendLine("");
            strQry.AppendLine("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");
            strQry.AppendLine("");
            strQry.AppendLine("IF EXISTS(SELECT * FROM INSERTED)");
            strQry.AppendLine("AND EXISTS(SELECT * FROM DELETED)");
            strQry.AppendLine("");
            strQry.AppendLine("    BEGIN");
            strQry.AppendLine("");
            strQry.AppendLine("    SET @Action = 'U'");
            strQry.AppendLine("");
            strQry.AppendLine("    END");
            strQry.AppendLine("");
            strQry.AppendLine("ELSE");
            strQry.AppendLine("");
            strQry.AppendLine("    IF EXISTS(SELECT * FROM INSERTED)");
            strQry.AppendLine("");
            strQry.AppendLine("        BEGIN");
            strQry.AppendLine("");
            strQry.AppendLine("        SET @Action = 'I'");
            strQry.AppendLine("");
            strQry.AppendLine("        INSERT INTO dbo.#DAY_TABLE#");
            strQry.AppendLine("       (COMPANY_NO");
            strQry.AppendLine("       ,EMPLOYEE_NO");
            strQry.AppendLine("       ,PAY_CATEGORY_NO");
            strQry.AppendLine("       ,TIMESHEET_DATE");
            strQry.AppendLine("       ,DAY_PAID_MINUTES");
            strQry.AppendLine("       ,BREAK_ACCUM_MINUTES)");
            strQry.AppendLine("");
            strQry.AppendLine("        SELECT DISTINCT");
            strQry.AppendLine("        I.COMPANY_NO");
            strQry.AppendLine("       ,I.EMPLOYEE_NO");
            strQry.AppendLine("       ,I.PAY_CATEGORY_NO");
            strQry.AppendLine("       ,I.#DATE_FIELD#");
            strQry.AppendLine("       ,0");
            strQry.AppendLine("       ,0");
            strQry.AppendLine("        FROM INSERTED I");
            strQry.AppendLine("");
            strQry.AppendLine("        LEFT JOIN dbo.#DAY_TABLE# ETBDC");
            strQry.AppendLine("        ON I.COMPANY_NO = ETBDC.COMPANY_NO");
            strQry.AppendLine("        AND I.EMPLOYEE_NO = ETBDC.EMPLOYEE_NO");
            strQry.AppendLine("        AND I.PAY_CATEGORY_NO = ETBDC.PAY_CATEGORY_NO");
            strQry.AppendLine("        AND I.#DATE_FIELD# = ETBDC.TIMESHEET_DATE");
            strQry.AppendLine("");
            strQry.AppendLine("        WHERE ETBDC.COMPANY_NO IS NULL");
            strQry.AppendLine("");
            strQry.AppendLine("        END");
            strQry.AppendLine("");
            strQry.AppendLine("    ELSE");
            strQry.AppendLine("");
            strQry.AppendLine("        IF EXISTS(SELECT * FROM DELETED)");
            strQry.AppendLine("");
            strQry.AppendLine("            BEGIN");
            strQry.AppendLine("");
            strQry.AppendLine("            SET @Action = 'D'");
            strQry.AppendLine("");
            strQry.AppendLine("            DELETE ETBDC");
            strQry.AppendLine("");
            strQry.AppendLine("            FROM dbo.#DAY_TABLE# ETBDC");
            strQry.AppendLine("");
            strQry.AppendLine("            INNER JOIN DELETED D");
            strQry.AppendLine("");
            strQry.AppendLine("            ON ETBDC.COMPANY_NO = D.COMPANY_NO");
            strQry.AppendLine("            AND ETBDC.EMPLOYEE_NO = D.EMPLOYEE_NO");
            strQry.AppendLine("            AND ETBDC.PAY_CATEGORY_NO = D.PAY_CATEGORY_NO");
            strQry.AppendLine("            AND ETBDC.TIMESHEET_DATE = D.#DATE_FIELD#");
            strQry.AppendLine("");
            strQry.AppendLine("            LEFT JOIN");
            strQry.AppendLine("");
            strQry.AppendLine("           (SELECT");
            strQry.AppendLine("            ETC.COMPANY_NO");
            strQry.AppendLine("           ,ETC.EMPLOYEE_NO");
            strQry.AppendLine("           ,ETC.PAY_CATEGORY_NO");
            strQry.AppendLine("           ,ETC.TIMESHEET_DATE");
            strQry.AppendLine("");
            strQry.AppendLine("            FROM dbo.#TIMESHEET_TABLE# ETC");
            strQry.AppendLine("");
            strQry.AppendLine("            INNER JOIN DELETED D");
            strQry.AppendLine("");
            strQry.AppendLine("            ON ETC.COMPANY_NO = D.COMPANY_NO");
            strQry.AppendLine("            AND ETC.EMPLOYEE_NO = D.EMPLOYEE_NO");
            strQry.AppendLine("            AND ETC.PAY_CATEGORY_NO = D.PAY_CATEGORY_NO");
            strQry.AppendLine("            AND ETC.TIMESHEET_DATE = D.#DATE_FIELD#");
            strQry.AppendLine("");
            strQry.AppendLine("            UNION");
            strQry.AppendLine("");
            strQry.AppendLine("            SELECT");
            strQry.AppendLine("            EBC.COMPANY_NO");
            strQry.AppendLine("           ,EBC.EMPLOYEE_NO");
            strQry.AppendLine("           ,EBC.PAY_CATEGORY_NO");
            strQry.AppendLine("           ,EBC.BREAK_DATE AS TIMESHEET_DATE");
            strQry.AppendLine("");
            strQry.AppendLine("            FROM dbo.#BREAK_TABLE# EBC");
            strQry.AppendLine("");
            strQry.AppendLine("            INNER JOIN DELETED D");
            strQry.AppendLine("");
            strQry.AppendLine("            ON EBC.COMPANY_NO = D.COMPANY_NO");
            strQry.AppendLine("            AND EBC.EMPLOYEE_NO = D.EMPLOYEE_NO");
            strQry.AppendLine("            AND EBC.PAY_CATEGORY_NO = D.PAY_CATEGORY_NO");
            strQry.AppendLine("            AND EBC.BREAK_DATE = D.#DATE_FIELD#");
            strQry.AppendLine("           ) AS TABLE_CHECK");
            strQry.AppendLine("");
            strQry.AppendLine("            ON ETBDC.COMPANY_NO = TABLE_CHECK.COMPANY_NO");
            strQry.AppendLine("            AND ETBDC.EMPLOYEE_NO = TABLE_CHECK.EMPLOYEE_NO");
            strQry.AppendLine("            AND ETBDC.PAY_CATEGORY_NO = TABLE_CHECK.PAY_CATEGORY_NO");
            strQry.AppendLine("            AND ETBDC.TIMESHEET_DATE = TABLE_CHECK.TIMESHEET_DATE");
            strQry.AppendLine("");
            strQry.AppendLine("            --No #TIMESHEET_TABLE# / #BREAK_TABLE# Records");
            strQry.AppendLine("            WHERE TABLE_CHECK.COMPANY_NO IS NULL");
            strQry.AppendLine("");
            strQry.AppendLine("            END");
            strQry.AppendLine("");
            strQry.AppendLine("IF @Action = 'I'");
            strQry.AppendLine("OR @Action = 'U'");
            strQry.AppendLine("");
            strQry.AppendLine(" BEGIN");
            strQry.AppendLine("");
            strQry.AppendLine(" UPDATE ETC");
            strQry.AppendLine("");
            strQry.AppendLine(" SET");
            strQry.AppendLine("");
            strQry.AppendLine(" #TIMESHEET_OR_BREAK#_ACCUM_MINUTES = ISNULL(TEMP_TABLE.#TIMESHEET_OR_BREAK#_ACCUM_MINUTES,0)");
            strQry.AppendLine(",INDICATOR = TEMP_TABLE.INDICATOR");
            strQry.AppendLine("");
            strQry.AppendLine(" FROM dbo.#TRIGGER_TABLE_NAME# ETC");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN");
            strQry.AppendLine("(");
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" ETC.COMPANY_NO");
            strQry.AppendLine(",ETC.EMPLOYEE_NO");
            strQry.AppendLine(",ETC.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETC.#DATE_FIELD#");
            strQry.AppendLine(",ETC.#TIMESHEET_OR_BREAK#_SEQ");
            strQry.AppendLine(",#TIMESHEET_OR_BREAK#_ACCUM_MINUTES = ");
            strQry.AppendLine(" CASE");
            strQry.AppendLine(" WHEN NOT TEMP.EMPLOYEE_NO IS NULL THEN");
            strQry.AppendLine(" CASE");
            strQry.AppendLine(" WHEN ETC.#TIMESHEET_OR_BREAK#_TIME_OUT_MINUTES IS NULL OR ETC.#TIMESHEET_OR_BREAK#_TIME_IN_MINUTES IS NULL");
            strQry.AppendLine(" THEN 0");
            strQry.AppendLine(" WHEN ETC.#TIMESHEET_OR_BREAK#_TIME_OUT_MINUTES > ETC.#TIMESHEET_OR_BREAK#_TIME_IN_MINUTES THEN");
            strQry.AppendLine(" ETC.#TIMESHEET_OR_BREAK#_TIME_OUT_MINUTES - ETC.#TIMESHEET_OR_BREAK#_TIME_IN_MINUTES");
            strQry.AppendLine(" ELSE 0");
            strQry.AppendLine(" END");
            strQry.AppendLine(" ELSE");
            strQry.AppendLine(" ETC.#TIMESHEET_OR_BREAK#_TIME_OUT_MINUTES - ETC.#TIMESHEET_OR_BREAK#_TIME_IN_MINUTES");
            strQry.AppendLine(" END");
            strQry.AppendLine(",INDICATOR = ");
            strQry.AppendLine(" CASE");
            strQry.AppendLine(" WHEN NOT TEMP.EMPLOYEE_NO IS NULL");
            strQry.AppendLine(" THEN 'X'");
            strQry.AppendLine(" ELSE ''");
            strQry.AppendLine(" END");
            strQry.AppendLine("");
            strQry.AppendLine(" FROM dbo.EMPLOYEE E");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN");
            strQry.AppendLine("(");
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",#DATE_FIELD#");
            strQry.AppendLine(" FROM INSERTED");
            strQry.AppendLine(") AS I");
            strQry.AppendLine(" ON E.COMPANY_NO = I.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = I.EMPLOYEE_NO");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN dbo.#TRIGGER_TABLE_NAME# ETC");
            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = I.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND ETC.#DATE_FIELD# = I.#TIMESHEET_OR_BREAK#_DATE");
            strQry.AppendLine("");
            strQry.AppendLine(" LEFT JOIN");
            strQry.AppendLine("(");
            strQry.AppendLine("--2Start");
            strQry.AppendLine("--1Start");
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" ETC.EMPLOYEE_NO");
            strQry.AppendLine(",ETC.#DATE_FIELD#");
            strQry.AppendLine(",ETC.#TIMESHEET_OR_BREAK#_SEQ");
            strQry.AppendLine("");
            strQry.AppendLine(" FROM dbo.EMPLOYEE E");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN");
            strQry.AppendLine("(");
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",#DATE_FIELD#");
            strQry.AppendLine(" FROM INSERTED");
            strQry.AppendLine(") AS I");
            strQry.AppendLine(" ON E.COMPANY_NO = I.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = I.EMPLOYEE_NO");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN");
            strQry.AppendLine("(");
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" ROW_NUMBER() OVER");
            strQry.AppendLine(" (ORDER BY");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",#DATE_FIELD#");
            strQry.AppendLine(",#TIMESHEET_OR_BREAK#_TIME_IN_MINUTES");
            strQry.AppendLine(",#TIMESHEET_OR_BREAK#_TIME_OUT_MINUTES) AS SORTED_REC");
            strQry.AppendLine(",COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",#DATE_FIELD#");
            strQry.AppendLine(",#TIMESHEET_OR_BREAK#_SEQ");
            strQry.AppendLine(",#TIMESHEET_OR_BREAK#_TIME_IN_MINUTES");
            strQry.AppendLine(",#TIMESHEET_OR_BREAK#_TIME_OUT_MINUTES");
            strQry.AppendLine("");
            strQry.AppendLine(" FROM dbo.#TRIGGER_TABLE_NAME#");
            strQry.AppendLine(") AS ETC");
            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = I.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND ETC.#DATE_FIELD# = I.#TIMESHEET_OR_BREAK#_DATE");
            strQry.AppendLine("");
            strQry.AppendLine("--1End");
            strQry.AppendLine(" INNER JOIN");
            strQry.AppendLine("(");
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" ROW_NUMBER() OVER");
            strQry.AppendLine(" (ORDER BY");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",#DATE_FIELD#");
            strQry.AppendLine(",#TIMESHEET_OR_BREAK#_TIME_IN_MINUTES");
            strQry.AppendLine(",#TIMESHEET_OR_BREAK#_TIME_OUT_MINUTES) AS SORTED_REC");
            strQry.AppendLine(",COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",#DATE_FIELD#");
            strQry.AppendLine(",#TIMESHEET_OR_BREAK#_SEQ");
            strQry.AppendLine(",#TIMESHEET_OR_BREAK#_TIME_IN_MINUTES");
            strQry.AppendLine(",#TIMESHEET_OR_BREAK#_TIME_OUT_MINUTES");
            strQry.AppendLine("");
            strQry.AppendLine(" FROM dbo.#TRIGGER_TABLE_NAME#");
            strQry.AppendLine(") AS ETC2");
            strQry.AppendLine(" ON E.COMPANY_NO = ETC2.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC2.EMPLOYEE_NO");
            strQry.AppendLine(" AND I.PAY_CATEGORY_NO = ETC2.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND I.#TIMESHEET_OR_BREAK#_DATE = ETC2.#DATE_FIELD#");
            strQry.AppendLine("");
            strQry.AppendLine(" WHERE E.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine(" AND ((ETC.#TIMESHEET_OR_BREAK#_TIME_IN_MINUTES IS NULL");
            strQry.AppendLine(" OR ETC.#TIMESHEET_OR_BREAK#_TIME_OUT_MINUTES IS NULL)");
            strQry.AppendLine(" OR (ETC.#TIMESHEET_OR_BREAK#_TIME_IN_MINUTES >= ETC2.#TIMESHEET_OR_BREAK#_TIME_OUT_MINUTES");
            strQry.AppendLine(" AND ETC.SORTED_REC <= ETC2.SORTED_REC)");
            strQry.AppendLine(" OR (ETC.#TIMESHEET_OR_BREAK#_TIME_IN_MINUTES < ETC2.#TIMESHEET_OR_BREAK#_TIME_OUT_MINUTES");
            strQry.AppendLine(" AND ETC.SORTED_REC > ETC2.SORTED_REC))");
            strQry.AppendLine("");
            strQry.AppendLine("--2End");
            strQry.AppendLine(") AS TEMP");
            strQry.AppendLine(" ON ETC.EMPLOYEE_NO = TEMP.EMPLOYEE_NO");
            strQry.AppendLine(" AND ETC.#DATE_FIELD# = TEMP.#TIMESHEET_OR_BREAK#_DATE");
            strQry.AppendLine(" AND ETC.#TIMESHEET_OR_BREAK#_SEQ = TEMP.#TIMESHEET_OR_BREAK#_SEQ");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON ETC.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("");
            strQry.AppendLine(" WHERE E.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine(") AS TEMP_TABLE");
            strQry.AppendLine(" ON ETC.COMPANY_NO = TEMP_TABLE.COMPANY_NO");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = TEMP_TABLE.EMPLOYEE_NO");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = TEMP_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND ETC.#DATE_FIELD# = TEMP_TABLE.#DATE_FIELD#");
            strQry.AppendLine(" AND ETC.#TIMESHEET_OR_BREAK#_SEQ = TEMP_TABLE.#TIMESHEET_OR_BREAK#_SEQ");

            strQry.AppendLine("");
            strQry.AppendLine(" UPDATE ETBDC");
            strQry.AppendLine("");
            strQry.AppendLine(" SET");
            strQry.AppendLine("");
            strQry.AppendLine(" DAY_NO = TEMP_TABLE.DAY_NO");
            strQry.AppendLine(",DAY_PAID_MINUTES = ISNULL(TEMP_TABLE.DAY_PAID_MINUTES,0)");
            strQry.AppendLine(",BREAK_ACCUM_MINUTES = ISNULL(TEMP_TABLE.BREAK_ACCUM_MINUTES,0)");
            strQry.AppendLine(",INDICATOR = TEMP_TABLE.INDICATOR");
            strQry.AppendLine(",BREAK_INDICATOR = TEMP_TABLE.BREAK_INDICATOR");
            strQry.AppendLine("");
            strQry.AppendLine(" FROM dbo.#DAY_TABLE# ETBDC");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN");
            strQry.AppendLine("(");
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",BREAK_SUMMARY_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",BREAK_SUMMARY_TABLE.DAY_DATE");
            strQry.AppendLine(",D.DAY_NO");
            strQry.AppendLine(",0 AS DAY_PAID_MINUTES");
            strQry.AppendLine(",BREAK_SUMMARY_TABLE.INDICATOR");
            strQry.AppendLine(",BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES");
            strQry.AppendLine(",'' AS BREAK_INDICATOR");
            strQry.AppendLine("");
            strQry.AppendLine(" FROM dbo.EMPLOYEE E");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN");
            strQry.AppendLine("(");
            strQry.AppendLine("--2Start Break UNION 1");
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" BREAK_TABLE.COMPANY_NO");
            strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",BREAK_TABLE.EMPLOYEE_NO");
            strQry.AppendLine(",BREAK_TABLE.DAY_DATE");
            strQry.AppendLine(",SUM(BREAK_TABLE.BREAK_ACCUM_MINUTES) AS BREAK_ACCUM_MINUTES");
            strQry.AppendLine(",'X' AS INDICATOR");
            strQry.AppendLine("");
            strQry.AppendLine(" FROM");
            strQry.AppendLine("(");
            strQry.AppendLine("--1Start Break UNION 1");
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" EBC.COMPANY_NO");
            strQry.AppendLine(",'#PAY_CATEGORY_TYPE#' AS PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EBC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBC.EMPLOYEE_NO");
            strQry.AppendLine(",EBC.BREAK_DATE AS DAY_DATE");
            strQry.AppendLine(",EBC.BREAK_SEQ");
            strQry.AppendLine(",BREAK_ACCUM_MINUTES = ");
            strQry.AppendLine(" CASE");
            strQry.AppendLine(" WHEN ((EBC.BREAK_TIME_IN_MINUTES IS NULL");
            strQry.AppendLine(" OR EBC.BREAK_TIME_OUT_MINUTES IS NULL)");
            strQry.AppendLine(" OR (EBC.BREAK_TIME_IN_MINUTES >= EBC2.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine(" AND EBC.SORTED_REC <= EBC2.SORTED_REC)");
            strQry.AppendLine(" OR (EBC.BREAK_TIME_IN_MINUTES < EBC2.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine(" AND EBC.SORTED_REC > EBC2.SORTED_REC)) THEN");
            strQry.AppendLine(" CASE");
            strQry.AppendLine(" WHEN EBC.BREAK_TIME_OUT_MINUTES IS NULL OR EBC.BREAK_TIME_IN_MINUTES IS NULL");
            strQry.AppendLine(" THEN 0");
            strQry.AppendLine(" WHEN EBC.BREAK_TIME_OUT_MINUTES > EBC.BREAK_TIME_IN_MINUTES THEN");
            strQry.AppendLine(" EBC.BREAK_TIME_OUT_MINUTES - EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(" ELSE 0");
            strQry.AppendLine(" END");
            strQry.AppendLine(" ELSE");
            strQry.AppendLine(" EBC.BREAK_TIME_OUT_MINUTES - EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(" END");
            strQry.AppendLine("");
            strQry.AppendLine(" FROM dbo.EMPLOYEE E");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN");
            strQry.AppendLine("(");
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" ROW_NUMBER() OVER");
            strQry.AppendLine(" (ORDER BY");
            strQry.AppendLine(" EBC.COMPANY_NO");
            strQry.AppendLine(",EBC.EMPLOYEE_NO");
            strQry.AppendLine(",EBC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBC.BREAK_DATE");
            strQry.AppendLine(",EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",EBC.BREAK_TIME_OUT_MINUTES) AS SORTED_REC");
            strQry.AppendLine(",EBC.COMPANY_NO");
            strQry.AppendLine(",EBC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBC.EMPLOYEE_NO");
            strQry.AppendLine(",EBC.BREAK_DATE");
            strQry.AppendLine(",EBC.BREAK_SEQ");
            strQry.AppendLine(",EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",EBC.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine("");
            strQry.AppendLine(" FROM dbo.#BREAK_TABLE# EBC");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN");
            strQry.AppendLine("(");
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",#DATE_FIELD#");
            strQry.AppendLine(" FROM INSERTED");
            strQry.AppendLine(") AS I");
            strQry.AppendLine(" ON EBC.COMPANY_NO = I.COMPANY_NO");
            strQry.AppendLine(" AND EBC.EMPLOYEE_NO = I.EMPLOYEE_NO");
            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = I.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND EBC.BREAK_DATE = I.#TIMESHEET_OR_BREAK#_DATE");
            strQry.AppendLine(") AS EBC");
            strQry.AppendLine(" ON E.COMPANY_NO = EBC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EBC.EMPLOYEE_NO");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN");
            strQry.AppendLine("(");
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" ROW_NUMBER() OVER");
            strQry.AppendLine(" (ORDER BY");
            strQry.AppendLine(" EBC.COMPANY_NO");
            strQry.AppendLine(",EBC.EMPLOYEE_NO");
            strQry.AppendLine(",EBC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBC.BREAK_DATE");
            strQry.AppendLine(",EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",EBC.BREAK_TIME_OUT_MINUTES) AS SORTED_REC");
            strQry.AppendLine(",EBC.COMPANY_NO");
            strQry.AppendLine(",EBC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBC.EMPLOYEE_NO");
            strQry.AppendLine(",EBC.BREAK_DATE");
            strQry.AppendLine(",EBC.BREAK_SEQ");
            strQry.AppendLine(",EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",EBC.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine("");
            strQry.AppendLine(" FROM dbo.#BREAK_TABLE# EBC");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN");
            strQry.AppendLine("(");
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",#DATE_FIELD#");
            strQry.AppendLine(" FROM INSERTED");
            strQry.AppendLine(") AS I");
            strQry.AppendLine(" ON EBC.COMPANY_NO = I.COMPANY_NO");
            strQry.AppendLine(" AND EBC.EMPLOYEE_NO = I.EMPLOYEE_NO");
            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = I.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND EBC.BREAK_DATE = I.#TIMESHEET_OR_BREAK#_DATE");
            strQry.AppendLine(") AS EBC2");
            strQry.AppendLine(" ON EBC.COMPANY_NO = EBC2.COMPANY_NO");
            strQry.AppendLine(" AND EBC.EMPLOYEE_NO = EBC2.EMPLOYEE_NO");
            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = EBC2.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND EBC.BREAK_DATE = EBC2.BREAK_DATE");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON EBC.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND EBC.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON EBC.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("");
            strQry.AppendLine(" WHERE E.COMPANY_NO = EBC.COMPANY_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine(" --1End Break UNION 1");
            strQry.AppendLine(") AS BREAK_TABLE");
            strQry.AppendLine("");
            strQry.AppendLine(" GROUP BY");
            strQry.AppendLine(" BREAK_TABLE.COMPANY_NO");
            strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",BREAK_TABLE.EMPLOYEE_NO");
            strQry.AppendLine(",BREAK_TABLE.DAY_DATE");
            strQry.AppendLine("--2End Break UNION 1");
            strQry.AppendLine(") AS BREAK_SUMMARY_TABLE");
            strQry.AppendLine(" ON E.COMPANY_NO = BREAK_SUMMARY_TABLE.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = BREAK_SUMMARY_TABLE.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = BREAK_SUMMARY_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine("");
            strQry.AppendLine(" LEFT JOIN dbo.#TIMESHEET_TABLE# ETC");
            strQry.AppendLine(" ON BREAK_SUMMARY_TABLE.COMPANY_NO = ETC.COMPANY_NO");
            strQry.AppendLine(" AND BREAK_SUMMARY_TABLE.EMPLOYEE_NO = ETC.EMPLOYEE_NO");
            strQry.AppendLine(" AND BREAK_SUMMARY_TABLE.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND BREAK_SUMMARY_TABLE.DAY_DATE = ETC.TIMESHEET_DATE");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN dbo.DATES D");
            strQry.AppendLine(" ON BREAK_SUMMARY_TABLE.DAY_DATE = D.DAY_DATE");
            strQry.AppendLine("");
            strQry.AppendLine(" WHERE E.COMPANY_NO = BREAK_SUMMARY_TABLE.COMPANY_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE IS NULL");
            strQry.AppendLine("");
            strQry.AppendLine(" UNION");
            strQry.AppendLine("");
            strQry.AppendLine("--3Start Major UNION 2");
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" TEMP2_TABLE.COMPANY_NO");
            strQry.AppendLine(",TEMP2_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",TEMP2_TABLE.EMPLOYEE_NO");
            strQry.AppendLine(",TEMP2_TABLE.DAY_DATE");
            strQry.AppendLine(",D.DAY_NO");
            strQry.AppendLine(",TEMP2_TABLE.DAY_PAID_MINUTES");
            strQry.AppendLine(",INDICATOR = ");
            strQry.AppendLine(" CASE");
            strQry.AppendLine(" WHEN TEMP2_TABLE.INDICATOR = 'X'");
            strQry.AppendLine(" THEN TEMP2_TABLE.INDICATOR");
            strQry.AppendLine(" WHEN D.DAY_NO = 0");
            strQry.AppendLine(" AND (TEMP2_TABLE.DAY_PAID_MINUTES > PC2.EXCEPTION_SUN_ABOVE_MINUTES");
            strQry.AppendLine(" OR TEMP2_TABLE.DAY_PAID_MINUTES < PC2.EXCEPTION_SUN_BELOW_MINUTES)");
            strQry.AppendLine(" THEN 'E'");
            strQry.AppendLine(" WHEN D.DAY_NO = 1");
            strQry.AppendLine(" AND (TEMP2_TABLE.DAY_PAID_MINUTES > PC2.EXCEPTION_MON_ABOVE_MINUTES");
            strQry.AppendLine(" OR TEMP2_TABLE.DAY_PAID_MINUTES < PC2.EXCEPTION_MON_BELOW_MINUTES)");
            strQry.AppendLine(" THEN 'E'");
            strQry.AppendLine(" WHEN D.DAY_NO = 2");
            strQry.AppendLine(" AND (TEMP2_TABLE.DAY_PAID_MINUTES > PC2.EXCEPTION_TUE_ABOVE_MINUTES");
            strQry.AppendLine(" OR TEMP2_TABLE.DAY_PAID_MINUTES < PC2.EXCEPTION_TUE_BELOW_MINUTES)");
            strQry.AppendLine(" THEN 'E'");
            strQry.AppendLine(" WHEN D.DAY_NO = 3");
            strQry.AppendLine(" AND (TEMP2_TABLE.DAY_PAID_MINUTES > PC2.EXCEPTION_WED_ABOVE_MINUTES");
            strQry.AppendLine(" OR TEMP2_TABLE.DAY_PAID_MINUTES < PC2.EXCEPTION_WED_BELOW_MINUTES)");
            strQry.AppendLine(" THEN 'E'");
            strQry.AppendLine(" WHEN D.DAY_NO = 4");
            strQry.AppendLine(" AND (TEMP2_TABLE.DAY_PAID_MINUTES > PC2.EXCEPTION_THU_ABOVE_MINUTES");
            strQry.AppendLine(" OR TEMP2_TABLE.DAY_PAID_MINUTES < PC2.EXCEPTION_THU_BELOW_MINUTES)");
            strQry.AppendLine(" THEN 'E'");
            strQry.AppendLine(" WHEN D.DAY_NO = 5");
            strQry.AppendLine(" AND (TEMP2_TABLE.DAY_PAID_MINUTES > PC2.EXCEPTION_FRI_ABOVE_MINUTES");
            strQry.AppendLine(" OR TEMP2_TABLE.DAY_PAID_MINUTES < PC2.EXCEPTION_FRI_BELOW_MINUTES)");
            strQry.AppendLine(" THEN 'E'");
            strQry.AppendLine(" WHEN D.DAY_NO = 6");
            strQry.AppendLine(" AND (TEMP2_TABLE.DAY_PAID_MINUTES > PC2.EXCEPTION_SAT_ABOVE_MINUTES");
            strQry.AppendLine(" OR TEMP2_TABLE.DAY_PAID_MINUTES < PC2.EXCEPTION_SAT_BELOW_MINUTES)");
            strQry.AppendLine(" THEN 'E'");
            strQry.AppendLine(" ELSE TEMP2_TABLE.INDICATOR");
            strQry.AppendLine(" END");
            strQry.AppendLine(",TEMP2_TABLE.BREAK_ACCUM_MINUTES");
            strQry.AppendLine(",TEMP2_TABLE.BREAK_INDICATOR");
            strQry.AppendLine("");
            strQry.AppendLine(" FROM");
            strQry.AppendLine("(");
            strQry.AppendLine("--2Start Major UNION 2");
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" TEMP1_TABLE.COMPANY_NO");
            strQry.AppendLine(",TEMP1_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TEMP1_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",TEMP1_TABLE.EMPLOYEE_NO");
            strQry.AppendLine(",TEMP1_TABLE.DAY_DATE");
            strQry.AppendLine(",DAY_PAID_MINUTES = ");
            strQry.AppendLine(" CASE");
            strQry.AppendLine(" WHEN PC1.DAILY_ROUNDING_IND = 0");
            strQry.AppendLine(" THEN TEMP1_TABLE.DAY_PAID_MINUTES");
            strQry.AppendLine(" WHEN PC1.DAILY_ROUNDING_IND = 1");
            strQry.AppendLine(" THEN CASE");
            strQry.AppendLine(" WHEN TEMP1_TABLE.DAY_PAID_MINUTES % PC1.DAILY_ROUNDING_MINUTES = 0");
            strQry.AppendLine(" THEN TEMP1_TABLE.DAY_PAID_MINUTES");
            strQry.AppendLine(" ELSE TEMP1_TABLE.DAY_PAID_MINUTES + (PC1.DAILY_ROUNDING_MINUTES - (TEMP1_TABLE.DAY_PAID_MINUTES % PC1.DAILY_ROUNDING_MINUTES))");
            strQry.AppendLine(" END");
            strQry.AppendLine(" WHEN PC1.DAILY_ROUNDING_IND = 2");
            strQry.AppendLine(" THEN CASE");
            strQry.AppendLine(" WHEN TEMP1_TABLE.DAY_PAID_MINUTES % PC1.DAILY_ROUNDING_MINUTES = 0");
            strQry.AppendLine(" THEN TEMP1_TABLE.DAY_PAID_MINUTES");
            strQry.AppendLine(" ELSE TEMP1_TABLE.DAY_PAID_MINUTES - (TEMP1_TABLE.DAY_PAID_MINUTES % PC1.DAILY_ROUNDING_MINUTES)");
            strQry.AppendLine(" END");
            strQry.AppendLine(" WHEN PC1.DAILY_ROUNDING_IND = 3");
            strQry.AppendLine(" THEN CASE");
            strQry.AppendLine(" WHEN TEMP1_TABLE.DAY_PAID_MINUTES % PC1.DAILY_ROUNDING_MINUTES = 0");
            strQry.AppendLine(" THEN TEMP1_TABLE.DAY_PAID_MINUTES");
            strQry.AppendLine(" WHEN TEMP1_TABLE.DAY_PAID_MINUTES % PC1.DAILY_ROUNDING_MINUTES > CONVERT(DECIMAL,PC1.DAILY_ROUNDING_MINUTES) / 2");
            strQry.AppendLine(" THEN TEMP1_TABLE.DAY_PAID_MINUTES + (PC1.DAILY_ROUNDING_MINUTES - (TEMP1_TABLE.DAY_PAID_MINUTES % PC1.DAILY_ROUNDING_MINUTES))");
            strQry.AppendLine(" ELSE TEMP1_TABLE.DAY_PAID_MINUTES - (TEMP1_TABLE.DAY_PAID_MINUTES % PC1.DAILY_ROUNDING_MINUTES)");
            strQry.AppendLine(" END");
            strQry.AppendLine(" END");
            strQry.AppendLine(",TEMP1_TABLE.INDICATOR");
            strQry.AppendLine(",TEMP1_TABLE.BREAK_ACCUM_MINUTES");
            strQry.AppendLine(",TEMP1_TABLE.BREAK_INDICATOR");
            strQry.AppendLine("");
            strQry.AppendLine(" FROM");
            strQry.AppendLine("(");
            strQry.AppendLine("--1Start Major UNION 2");
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" TIMESHEET_TOTAL_TABLE.COMPANY_NO");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.EMPLOYEE_NO");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.DAY_DATE");
            strQry.AppendLine(",ISNULL(BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES,0) AS BREAK_ACCUM_MINUTES");
            strQry.AppendLine(",DAY_PAID_MINUTES = ");
            strQry.AppendLine(" CASE");
            strQry.AppendLine(" WHEN BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES > TIMESHEET_TOTAL_TABLE.BREAK_MINUTES");
            strQry.AppendLine(" AND BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES < TIMESHEET_TOTAL_TABLE.TIMESHEET_ACCUM_MINUTES");
            strQry.AppendLine(" THEN TIMESHEET_TOTAL_TABLE.TIMESHEET_ACCUM_MINUTES - BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES");
            strQry.AppendLine(" ELSE TIMESHEET_TOTAL_TABLE.TIMESHEET_ACCUM_MINUTES - TIMESHEET_TOTAL_TABLE.BREAK_MINUTES");
            strQry.AppendLine(" END");
            strQry.AppendLine(",INDICATOR = ");
            strQry.AppendLine(" CASE");
            strQry.AppendLine(" WHEN BREAK_SUMMARY_TABLE.INDICATOR = 'X' OR TIMESHEET_TOTAL_TABLE.TIMESHEET_ACCUM_MINUTES < BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES");
            strQry.AppendLine(" THEN 'X'");
            strQry.AppendLine(" ELSE ISNULL(MAX(TIMESHEET_TOTAL_TABLE.INDICATOR),'')");
            strQry.AppendLine(" END");
            strQry.AppendLine(",BREAK_INDICATOR = ");
            strQry.AppendLine(" CASE");
            strQry.AppendLine(" WHEN BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES > TIMESHEET_TOTAL_TABLE.BREAK_MINUTES");
            strQry.AppendLine(" THEN 'Y'");
            strQry.AppendLine(" ELSE ''");
            strQry.AppendLine(" END");
            strQry.AppendLine("");
            strQry.AppendLine(" FROM");
            strQry.AppendLine("(");
            strQry.AppendLine("--3Start Timesheet UNION 2");
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" TIMESHEET_SUMMARY_TABLE.COMPANY_NO");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.EMPLOYEE_NO");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.DAY_DATE");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.TIMESHEET_ACCUM_MINUTES");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.INDICATOR");
            strQry.AppendLine(",ISNULL(MAX(PCB.BREAK_MINUTES),0) AS BREAK_MINUTES");
            strQry.AppendLine("");
            strQry.AppendLine(" FROM");
            strQry.AppendLine("(");
            strQry.AppendLine("--2Start Timesheet UNION 2");
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" TIMESHEET_TABLE.COMPANY_NO");
            strQry.AppendLine(",TIMESHEET_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TIMESHEET_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",TIMESHEET_TABLE.EMPLOYEE_NO");
            strQry.AppendLine(",TIMESHEET_TABLE.DAY_DATE");
            strQry.AppendLine(",SUM(TIMESHEET_ACCUM_MINUTES) AS TIMESHEET_ACCUM_MINUTES");
            strQry.AppendLine(",MAX(ISNULL(INDICATOR,'')) AS INDICATOR");
            strQry.AppendLine("");
            strQry.AppendLine(" FROM");
            strQry.AppendLine("(");
            strQry.AppendLine("--1Start Timesheet UNION 2");
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" ETC.COMPANY_NO");
            strQry.AppendLine(",'#PAY_CATEGORY_TYPE#' AS PAY_CATEGORY_TYPE");
            strQry.AppendLine(",ETC.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETC.EMPLOYEE_NO");
            strQry.AppendLine(",ETC.TIMESHEET_DATE AS DAY_DATE");
            strQry.AppendLine(",ETC.TIMESHEET_SEQ");
            strQry.AppendLine(",TIMESHEET_ACCUM_MINUTES = ");
            strQry.AppendLine(" CASE");
            strQry.AppendLine(" WHEN NOT TEMP.EMPLOYEE_NO IS NULL THEN");
            strQry.AppendLine(" CASE");
            strQry.AppendLine(" WHEN ETC.TIMESHEET_TIME_OUT_MINUTES IS NULL OR ETC.TIMESHEET_TIME_IN_MINUTES IS NULL");
            strQry.AppendLine(" THEN 0");
            strQry.AppendLine(" WHEN ETC.TIMESHEET_TIME_OUT_MINUTES > ETC.TIMESHEET_TIME_IN_MINUTES THEN");
            strQry.AppendLine(" ETC.TIMESHEET_TIME_OUT_MINUTES - ETC.TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine(" ELSE 0");
            strQry.AppendLine(" END");
            strQry.AppendLine(" ELSE");
            strQry.AppendLine(" ETC.TIMESHEET_TIME_OUT_MINUTES - ETC.TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine(" END");
            strQry.AppendLine(",INDICATOR = ");
            strQry.AppendLine(" CASE");
            strQry.AppendLine(" WHEN NOT TEMP.EMPLOYEE_NO IS NULL");
            strQry.AppendLine(" THEN 'X'");
            strQry.AppendLine(" ELSE ''");
            strQry.AppendLine(" END");
            strQry.AppendLine("");
            strQry.AppendLine(" FROM dbo.EMPLOYEE E");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN dbo.#TIMESHEET_TABLE# ETC");
            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN");
            strQry.AppendLine("(");
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",#DATE_FIELD#");
            strQry.AppendLine(" FROM INSERTED");
            strQry.AppendLine(") AS I");
            strQry.AppendLine(" ON ETC.COMPANY_NO = I.COMPANY_NO");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = I.EMPLOYEE_NO");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = I.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE = I.#TIMESHEET_OR_BREAK#_DATE");
            strQry.AppendLine("");
            strQry.AppendLine(" LEFT JOIN");
            strQry.AppendLine("(");
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" ETC.EMPLOYEE_NO");
            strQry.AppendLine(",ETC.TIMESHEET_DATE");
            strQry.AppendLine(",ETC.TIMESHEET_SEQ");
            strQry.AppendLine("");
            strQry.AppendLine(" FROM dbo.EMPLOYEE E");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN");
            strQry.AppendLine("(");
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" ROW_NUMBER() OVER");
            strQry.AppendLine(" (ORDER BY");
            strQry.AppendLine(" ETC.COMPANY_NO");
            strQry.AppendLine(",ETC.EMPLOYEE_NO");
            strQry.AppendLine(",ETC.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETC.TIMESHEET_DATE");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_OUT_MINUTES) AS SORTED_REC");
            strQry.AppendLine(",ETC.COMPANY_NO");
            strQry.AppendLine(",ETC.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETC.EMPLOYEE_NO");
            strQry.AppendLine(",ETC.TIMESHEET_DATE");
            strQry.AppendLine(",ETC.TIMESHEET_SEQ");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_OUT_MINUTES");
            strQry.AppendLine("");
            strQry.AppendLine(" FROM dbo.#TIMESHEET_TABLE# ETC");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN");
            strQry.AppendLine("(");
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",#DATE_FIELD#");
            strQry.AppendLine(" FROM INSERTED");
            strQry.AppendLine(") AS I");
            strQry.AppendLine(" ON ETC.COMPANY_NO = I.COMPANY_NO");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = I.EMPLOYEE_NO");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = I.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE = I.#TIMESHEET_OR_BREAK#_DATE");
            strQry.AppendLine(") AS ETC");
            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN");
            strQry.AppendLine("(");
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" ROW_NUMBER() OVER");
            strQry.AppendLine("(ORDER BY");
            strQry.AppendLine(" ETC.COMPANY_NO");
            strQry.AppendLine(",ETC.EMPLOYEE_NO");
            strQry.AppendLine(",ETC.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETC.TIMESHEET_DATE");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_OUT_MINUTES) AS SORTED_REC");
            strQry.AppendLine(",ETC.COMPANY_NO");
            strQry.AppendLine(",ETC.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETC.EMPLOYEE_NO");
            strQry.AppendLine(",ETC.TIMESHEET_DATE");
            strQry.AppendLine(",ETC.TIMESHEET_SEQ");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_OUT_MINUTES");
            strQry.AppendLine("");
            strQry.AppendLine(" FROM dbo.#TIMESHEET_TABLE# ETC");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN");
            strQry.AppendLine("(");
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",#DATE_FIELD#");
            strQry.AppendLine(" FROM INSERTED");
            strQry.AppendLine(") AS I");
            strQry.AppendLine(" ON ETC.COMPANY_NO = I.COMPANY_NO");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = I.EMPLOYEE_NO");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = I.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE = I.#TIMESHEET_OR_BREAK#_DATE");
            strQry.AppendLine(") AS ETC2");
            strQry.AppendLine(" ON ETC.COMPANY_NO = ETC2.COMPANY_NO");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = ETC2.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = ETC2.EMPLOYEE_NO");
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE = ETC2.TIMESHEET_DATE");
            strQry.AppendLine("");
            strQry.AppendLine(" WHERE E.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine(" AND ((ETC.TIMESHEET_TIME_IN_MINUTES IS NULL");
            strQry.AppendLine(" OR ETC.TIMESHEET_TIME_OUT_MINUTES IS NULL)");
            strQry.AppendLine(" OR (ETC.TIMESHEET_TIME_IN_MINUTES >= ETC2.TIMESHEET_TIME_OUT_MINUTES");
            strQry.AppendLine(" AND ETC.SORTED_REC <= ETC2.SORTED_REC)");
            strQry.AppendLine(" OR (ETC.TIMESHEET_TIME_IN_MINUTES < ETC2.TIMESHEET_TIME_OUT_MINUTES");
            strQry.AppendLine(" AND ETC.SORTED_REC > ETC2.SORTED_REC))");
            strQry.AppendLine(") AS TEMP");
            strQry.AppendLine(" ON ETC.EMPLOYEE_NO = TEMP.EMPLOYEE_NO");
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE = TEMP.TIMESHEET_DATE");
            strQry.AppendLine(" AND ETC.TIMESHEET_SEQ = TEMP.TIMESHEET_SEQ");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON ETC.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("");
            strQry.AppendLine(" WHERE E.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("--1End Timesheet UNION 2");
            strQry.AppendLine(") AS TIMESHEET_TABLE");
            strQry.AppendLine("");
            strQry.AppendLine(" GROUP BY");
            strQry.AppendLine(" TIMESHEET_TABLE.COMPANY_NO");
            strQry.AppendLine(",TIMESHEET_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TIMESHEET_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",TIMESHEET_TABLE.EMPLOYEE_NO");
            strQry.AppendLine(",TIMESHEET_TABLE.DAY_DATE");
            strQry.AppendLine("--2End Timesheet UNION 2");
            strQry.AppendLine(") AS TIMESHEET_SUMMARY_TABLE");
            strQry.AppendLine("");
            strQry.AppendLine(" LEFT JOIN dbo.PAY_CATEGORY_BREAK PCB");
            strQry.AppendLine(" ON TIMESHEET_SUMMARY_TABLE.COMPANY_NO = PCB.COMPANY_NO");
            strQry.AppendLine(" AND TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_NO = PCB.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_TYPE = PCB.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND TIMESHEET_SUMMARY_TABLE.TIMESHEET_ACCUM_MINUTES >= PCB.WORKED_TIME_MINUTES");
            strQry.AppendLine("");
            strQry.AppendLine(" GROUP BY");
            strQry.AppendLine(" TIMESHEET_SUMMARY_TABLE.COMPANY_NO");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.EMPLOYEE_NO");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.DAY_DATE");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.TIMESHEET_ACCUM_MINUTES");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.INDICATOR");
            strQry.AppendLine("--3End Timesheet UNION 2");
            strQry.AppendLine(") AS TIMESHEET_TOTAL_TABLE");
            strQry.AppendLine("");
            strQry.AppendLine(" LEFT JOIN");
            strQry.AppendLine("(");
            strQry.AppendLine("--3Start Break UNION 2");
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" BREAK_TABLE.COMPANY_NO");
            strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",BREAK_TABLE.EMPLOYEE_NO");
            strQry.AppendLine(",BREAK_TABLE.DAY_DATE");
            strQry.AppendLine(",SUM(BREAK_TABLE.BREAK_ACCUM_MINUTES) AS BREAK_ACCUM_MINUTES");
            strQry.AppendLine(",MAX(ISNULL(BREAK_TABLE.INDICATOR,'')) AS INDICATOR");
            strQry.AppendLine("");
            strQry.AppendLine(" FROM");
            strQry.AppendLine("(");
            strQry.AppendLine("--2Start Break UNION 2");
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" EBC.COMPANY_NO");
            strQry.AppendLine(",'#PAY_CATEGORY_TYPE#' AS PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EBC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBC.EMPLOYEE_NO");
            strQry.AppendLine(",EBC.BREAK_DATE AS DAY_DATE");
            strQry.AppendLine(",EBC.BREAK_SEQ");
            strQry.AppendLine(",BREAK_ACCUM_MINUTES = ");
            strQry.AppendLine(" CASE");
            strQry.AppendLine(" WHEN NOT TEMP.EMPLOYEE_NO IS NULL THEN");
            strQry.AppendLine(" CASE");
            strQry.AppendLine(" WHEN EBC.BREAK_TIME_OUT_MINUTES IS NULL OR EBC.BREAK_TIME_IN_MINUTES IS NULL");
            strQry.AppendLine(" THEN 0");
            strQry.AppendLine(" WHEN EBC.BREAK_TIME_OUT_MINUTES > EBC.BREAK_TIME_IN_MINUTES THEN");
            strQry.AppendLine(" EBC.BREAK_TIME_OUT_MINUTES - EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(" ELSE 0");
            strQry.AppendLine(" END");
            strQry.AppendLine(" ELSE");
            strQry.AppendLine(" EBC.BREAK_TIME_OUT_MINUTES - EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(" END");
            strQry.AppendLine(",INDICATOR = ");
            strQry.AppendLine(" CASE");
            strQry.AppendLine(" WHEN NOT TEMP.EMPLOYEE_NO IS NULL");
            strQry.AppendLine(" THEN 'X'");
            strQry.AppendLine(" ELSE ''");
            strQry.AppendLine(" END");
            strQry.AppendLine("");
            strQry.AppendLine(" FROM dbo.EMPLOYEE E");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN dbo.#BREAK_TABLE# EBC");
            strQry.AppendLine(" ON E.COMPANY_NO = EBC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EBC.EMPLOYEE_NO");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN");
            strQry.AppendLine("(");
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",#DATE_FIELD#");
            strQry.AppendLine(" FROM INSERTED");
            strQry.AppendLine(") AS I");
            strQry.AppendLine(" ON EBC.COMPANY_NO = I.COMPANY_NO");
            strQry.AppendLine(" AND EBC.EMPLOYEE_NO = I.EMPLOYEE_NO");
            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = I.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND EBC.BREAK_DATE = I.#TIMESHEET_OR_BREAK#_DATE");
            strQry.AppendLine("");
            strQry.AppendLine("LEFT JOIN");
            strQry.AppendLine("(");
            strQry.AppendLine("--1Start Break UNION 2");
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" EBC.EMPLOYEE_NO");
            strQry.AppendLine(",EBC.BREAK_DATE");
            strQry.AppendLine(",EBC.BREAK_SEQ");
            strQry.AppendLine(",EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",EBC.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine("");
            strQry.AppendLine(" FROM dbo.EMPLOYEE E");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN");
            strQry.AppendLine("(");
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" ROW_NUMBER() OVER");
            strQry.AppendLine(" (ORDER BY");
            strQry.AppendLine(" EBC.COMPANY_NO");
            strQry.AppendLine(",EBC.EMPLOYEE_NO");
            strQry.AppendLine(",EBC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBC.BREAK_DATE");
            strQry.AppendLine(",EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",EBC.BREAK_TIME_OUT_MINUTES) AS SORTED_REC");
            strQry.AppendLine(",EBC.COMPANY_NO");
            strQry.AppendLine(",EBC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBC.EMPLOYEE_NO");
            strQry.AppendLine(",EBC.BREAK_DATE");
            strQry.AppendLine(",EBC.BREAK_SEQ");
            strQry.AppendLine(",EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",EBC.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine("");
            strQry.AppendLine(" FROM dbo.#BREAK_TABLE# EBC");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN");
            strQry.AppendLine("(");
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",#DATE_FIELD#");
            strQry.AppendLine(" FROM INSERTED");
            strQry.AppendLine(") AS I");
            strQry.AppendLine(" ON EBC.COMPANY_NO = I.COMPANY_NO");
            strQry.AppendLine(" AND EBC.EMPLOYEE_NO = I.EMPLOYEE_NO");
            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = I.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND EBC.BREAK_DATE = I.#TIMESHEET_OR_BREAK#_DATE");
            strQry.AppendLine(") AS EBC");
            strQry.AppendLine(" ON E.COMPANY_NO = EBC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EBC.EMPLOYEE_NO");
            strQry.AppendLine("--1End Break UNION 2");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN");
            strQry.AppendLine("(");
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" ROW_NUMBER() OVER");
            strQry.AppendLine(" (ORDER BY");
            strQry.AppendLine(" EBC.COMPANY_NO");
            strQry.AppendLine(",EBC.EMPLOYEE_NO");
            strQry.AppendLine(",EBC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBC.BREAK_DATE");
            strQry.AppendLine(",EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",EBC.BREAK_TIME_OUT_MINUTES) AS SORTED_REC");
            strQry.AppendLine(",EBC.COMPANY_NO");
            strQry.AppendLine(",EBC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBC.EMPLOYEE_NO");
            strQry.AppendLine(",EBC.BREAK_DATE");
            strQry.AppendLine(",EBC.BREAK_SEQ");
            strQry.AppendLine(",EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",EBC.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine("");
            strQry.AppendLine(" FROM dbo.#BREAK_TABLE# EBC");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN");
            strQry.AppendLine("(");
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",#DATE_FIELD#");
            strQry.AppendLine(" FROM INSERTED");
            strQry.AppendLine(") AS I");
            strQry.AppendLine(" ON EBC.COMPANY_NO = I.COMPANY_NO");
            strQry.AppendLine(" AND EBC.EMPLOYEE_NO = I.EMPLOYEE_NO");
            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = I.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND EBC.BREAK_DATE = I.#TIMESHEET_OR_BREAK#_DATE");
            strQry.AppendLine(") AS EBC2");
            strQry.AppendLine(" ON EBC.COMPANY_NO = EBC2.COMPANY_NO");
            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = EBC2.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND EBC.EMPLOYEE_NO = EBC2.EMPLOYEE_NO");
            strQry.AppendLine(" AND EBC.BREAK_DATE = EBC2.BREAK_DATE");
            strQry.AppendLine("");
            strQry.AppendLine(" WHERE E.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine(" AND ((EBC.BREAK_TIME_IN_MINUTES IS NULL");
            strQry.AppendLine(" OR EBC.BREAK_TIME_OUT_MINUTES IS NULL)");
            //Errol 2016-06-22
            strQry.AppendLine(" OR (EBC.BREAK_TIME_IN_MINUTES >= EBC2.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine(" AND EBC.SORTED_REC <= EBC2.SORTED_REC)");
            strQry.AppendLine(" OR (EBC.BREAK_TIME_IN_MINUTES < EBC2.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine(" AND EBC.SORTED_REC > EBC2.SORTED_REC))");
            strQry.AppendLine(") AS TEMP");
            strQry.AppendLine(" ON EBC.EMPLOYEE_NO = TEMP.EMPLOYEE_NO");
            strQry.AppendLine(" AND EBC.BREAK_DATE = TEMP.BREAK_DATE");
            strQry.AppendLine(" AND EBC.BREAK_SEQ = TEMP.BREAK_SEQ");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON EBC.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND EBC.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON EBC.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("");
            strQry.AppendLine(" WHERE E.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("");
            strQry.AppendLine(" GROUP BY");
            strQry.AppendLine(" EBC.COMPANY_NO");
            strQry.AppendLine(",EBC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBC.EMPLOYEE_NO");
            strQry.AppendLine(",EBC.BREAK_DATE");
            strQry.AppendLine(",EBC.BREAK_SEQ");
            strQry.AppendLine(",TEMP.EMPLOYEE_NO");
            strQry.AppendLine(",EBC.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine(",EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine("--2End Break UNION 2");
            strQry.AppendLine(") AS BREAK_TABLE");
            strQry.AppendLine("");
            strQry.AppendLine(" GROUP BY");
            strQry.AppendLine(" BREAK_TABLE.COMPANY_NO");
            strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",BREAK_TABLE.EMPLOYEE_NO");
            strQry.AppendLine(",BREAK_TABLE.DAY_DATE");
            strQry.AppendLine("--3End Break UNION 2");
            strQry.AppendLine(") AS BREAK_SUMMARY_TABLE");
            strQry.AppendLine(" ON TIMESHEET_TOTAL_TABLE.COMPANY_NO = BREAK_SUMMARY_TABLE.COMPANY_NO");
            strQry.AppendLine(" AND TIMESHEET_TOTAL_TABLE.PAY_CATEGORY_TYPE = BREAK_SUMMARY_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND TIMESHEET_TOTAL_TABLE.PAY_CATEGORY_NO = BREAK_SUMMARY_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND TIMESHEET_TOTAL_TABLE.EMPLOYEE_NO = BREAK_SUMMARY_TABLE.EMPLOYEE_NO");
            strQry.AppendLine(" AND TIMESHEET_TOTAL_TABLE.DAY_DATE = BREAK_SUMMARY_TABLE.DAY_DATE");
            strQry.AppendLine("");
            strQry.AppendLine(" GROUP BY");
            strQry.AppendLine(" TIMESHEET_TOTAL_TABLE.COMPANY_NO");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.EMPLOYEE_NO");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.DAY_DATE");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.TIMESHEET_ACCUM_MINUTES");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.INDICATOR");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.BREAK_MINUTES");
            strQry.AppendLine(",BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES");
            strQry.AppendLine(",BREAK_SUMMARY_TABLE.INDICATOR");
            strQry.AppendLine(",BREAK_SUMMARY_TABLE.COMPANY_NO");
            strQry.AppendLine("--1End Major UNION 2");
            strQry.AppendLine(") AS TEMP1_TABLE");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN dbo.PAY_CATEGORY PC1");
            strQry.AppendLine(" ON TEMP1_TABLE.COMPANY_NO = PC1.COMPANY_NO");
            strQry.AppendLine(" AND TEMP1_TABLE.PAY_CATEGORY_NO = PC1.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND TEMP1_TABLE.PAY_CATEGORY_TYPE = PC1.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PC1.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("--2End Major UNION 2");
            strQry.AppendLine(") AS TEMP2_TABLE");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN dbo.PAY_CATEGORY PC2");
            strQry.AppendLine(" ON TEMP2_TABLE.COMPANY_NO = PC2.COMPANY_NO");
            strQry.AppendLine(" AND TEMP2_TABLE.PAY_CATEGORY_NO = PC2.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND TEMP2_TABLE.PAY_CATEGORY_TYPE = PC2.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PC2.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("");
            strQry.AppendLine(" INNER JOIN dbo.DATES D");
            strQry.AppendLine(" ON TEMP2_TABLE.DAY_DATE = D.DAY_DATE");
            strQry.AppendLine("--3End Major UNION 2");
            strQry.AppendLine(") AS TEMP_TABLE");
            strQry.AppendLine("");
            strQry.AppendLine("ON ETBDC.COMPANY_NO = TEMP_TABLE.COMPANY_NO");
            strQry.AppendLine("AND ETBDC.EMPLOYEE_NO = TEMP_TABLE.EMPLOYEE_NO");
            strQry.AppendLine("AND ETBDC.PAY_CATEGORY_NO = TEMP_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine("AND ETBDC.TIMESHEET_DATE = TEMP_TABLE.DAY_DATE");
            strQry.AppendLine("");
            strQry.AppendLine("END");
            strQry.AppendLine("");
            strQry.AppendLine("ELSE");
            strQry.AppendLine("");
            strQry.AppendLine("    IF @Action = 'D'	");
            strQry.AppendLine("");
            strQry.AppendLine("    BEGIN");
            strQry.AppendLine("");
            strQry.AppendLine("    UPDATE ETC");
            strQry.AppendLine("");
            strQry.AppendLine("    SET");
            strQry.AppendLine("");
            strQry.AppendLine("    #TIMESHEET_OR_BREAK#_ACCUM_MINUTES = ISNULL(TEMP_TABLE.#TIMESHEET_OR_BREAK#_ACCUM_MINUTES,0)");
            strQry.AppendLine("   ,INDICATOR = TEMP_TABLE.INDICATOR");
            strQry.AppendLine("");
            strQry.AppendLine("    FROM dbo.#TRIGGER_TABLE_NAME# ETC");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN");
            strQry.AppendLine("   (");
            strQry.AppendLine("    SELECT");
            strQry.AppendLine("    ETC.COMPANY_NO");
            strQry.AppendLine("   ,ETC.EMPLOYEE_NO");
            strQry.AppendLine("   ,ETC.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,ETC.#DATE_FIELD#");
            strQry.AppendLine("   ,ETC.#TIMESHEET_OR_BREAK#_SEQ");
            strQry.AppendLine("   ,#TIMESHEET_OR_BREAK#_ACCUM_MINUTES = ");
            strQry.AppendLine("    CASE");
            strQry.AppendLine("    WHEN NOT TEMP.EMPLOYEE_NO IS NULL THEN");
            strQry.AppendLine("    CASE");
            strQry.AppendLine("    WHEN ETC.#TIMESHEET_OR_BREAK#_TIME_OUT_MINUTES IS NULL OR ETC.#TIMESHEET_OR_BREAK#_TIME_IN_MINUTES IS NULL");
            strQry.AppendLine("    THEN 0");
            strQry.AppendLine("    WHEN ETC.#TIMESHEET_OR_BREAK#_TIME_OUT_MINUTES > ETC.#TIMESHEET_OR_BREAK#_TIME_IN_MINUTES THEN");
            strQry.AppendLine("    ETC.#TIMESHEET_OR_BREAK#_TIME_OUT_MINUTES - ETC.#TIMESHEET_OR_BREAK#_TIME_IN_MINUTES");
            strQry.AppendLine("    ELSE 0");
            strQry.AppendLine("    END");
            strQry.AppendLine("    ELSE");
            strQry.AppendLine("    ETC.#TIMESHEET_OR_BREAK#_TIME_OUT_MINUTES - ETC.#TIMESHEET_OR_BREAK#_TIME_IN_MINUTES");
            strQry.AppendLine("    END");
            strQry.AppendLine("   ,INDICATOR = ");
            strQry.AppendLine("    CASE");
            strQry.AppendLine("    WHEN NOT TEMP.EMPLOYEE_NO IS NULL");
            strQry.AppendLine("    THEN 'X'");
            strQry.AppendLine("    ELSE ''");
            strQry.AppendLine("    END");
            strQry.AppendLine("");
            strQry.AppendLine("    FROM dbo.EMPLOYEE E");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN");
            strQry.AppendLine("   (");
            strQry.AppendLine("    SELECT DISTINCT");
            strQry.AppendLine("    COMPANY_NO");
            strQry.AppendLine("   ,EMPLOYEE_NO");
            strQry.AppendLine("   ,PAY_CATEGORY_NO");
            strQry.AppendLine("   ,#DATE_FIELD#");
            strQry.AppendLine("    FROM DELETED");
            strQry.AppendLine("   ) AS D");
            strQry.AppendLine("    ON E.COMPANY_NO = D.COMPANY_NO");
            strQry.AppendLine("    AND E.EMPLOYEE_NO = D.EMPLOYEE_NO");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN dbo.#TRIGGER_TABLE_NAME# ETC");
            strQry.AppendLine("    ON E.COMPANY_NO = ETC.COMPANY_NO");
            strQry.AppendLine("    AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO");
            strQry.AppendLine("    AND ETC.PAY_CATEGORY_NO = D.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND ETC.#DATE_FIELD# = D.#TIMESHEET_OR_BREAK#_DATE");
            strQry.AppendLine("");
            strQry.AppendLine("    LEFT JOIN");
            strQry.AppendLine("   (");
            strQry.AppendLine("    --2Start");
            strQry.AppendLine("    --1Start");
            strQry.AppendLine("    SELECT DISTINCT");
            strQry.AppendLine("    ETC.EMPLOYEE_NO");
            strQry.AppendLine("   ,ETC.#DATE_FIELD#");
            strQry.AppendLine("   ,ETC.#TIMESHEET_OR_BREAK#_SEQ");
            strQry.AppendLine("");
            strQry.AppendLine("    FROM dbo.EMPLOYEE E");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN");
            strQry.AppendLine("   (");
            strQry.AppendLine("    SELECT DISTINCT");
            strQry.AppendLine("    COMPANY_NO");
            strQry.AppendLine("   ,PAY_CATEGORY_NO");
            strQry.AppendLine("   ,EMPLOYEE_NO");
            strQry.AppendLine("   ,#DATE_FIELD#");
            strQry.AppendLine("    FROM DELETED");
            strQry.AppendLine("   ) AS D");
            strQry.AppendLine("    ON E.COMPANY_NO = D.COMPANY_NO");
            strQry.AppendLine("    AND E.EMPLOYEE_NO = D.EMPLOYEE_NO");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN");
            strQry.AppendLine("   (");
            strQry.AppendLine("    SELECT");
            strQry.AppendLine("    ROW_NUMBER() OVER");
            strQry.AppendLine("   (ORDER BY");
            strQry.AppendLine("    COMPANY_NO");
            strQry.AppendLine("   ,EMPLOYEE_NO");
            strQry.AppendLine("   ,PAY_CATEGORY_NO");
            strQry.AppendLine("   ,#DATE_FIELD#");
            strQry.AppendLine("   ,#TIMESHEET_OR_BREAK#_TIME_IN_MINUTES");
            strQry.AppendLine("   ,#TIMESHEET_OR_BREAK#_TIME_OUT_MINUTES) AS SORTED_REC");
            strQry.AppendLine("   ,COMPANY_NO");
            strQry.AppendLine("   ,PAY_CATEGORY_NO");
            strQry.AppendLine("   ,EMPLOYEE_NO");
            strQry.AppendLine("   ,#DATE_FIELD#");
            strQry.AppendLine("   ,#TIMESHEET_OR_BREAK#_SEQ");
            strQry.AppendLine("   ,#TIMESHEET_OR_BREAK#_TIME_IN_MINUTES");
            strQry.AppendLine("   ,#TIMESHEET_OR_BREAK#_TIME_OUT_MINUTES");
            strQry.AppendLine("");
            strQry.AppendLine("    FROM dbo.#TRIGGER_TABLE_NAME#) AS ETC");
            strQry.AppendLine("    ON E.COMPANY_NO = ETC.COMPANY_NO");
            strQry.AppendLine("    AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO");
            strQry.AppendLine("    AND ETC.PAY_CATEGORY_NO = D.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND ETC.#DATE_FIELD# = D.#TIMESHEET_OR_BREAK#_DATE");
            strQry.AppendLine("");
            strQry.AppendLine("    --1End");
            strQry.AppendLine("    INNER JOIN");
            strQry.AppendLine("   (");
            strQry.AppendLine("    SELECT");
            strQry.AppendLine("    ROW_NUMBER() OVER");
            strQry.AppendLine("   (ORDER BY");
            strQry.AppendLine("    COMPANY_NO");
            strQry.AppendLine("   ,EMPLOYEE_NO");
            strQry.AppendLine("   ,PAY_CATEGORY_NO");
            strQry.AppendLine("   ,#DATE_FIELD#");
            strQry.AppendLine("   ,#TIMESHEET_OR_BREAK#_TIME_IN_MINUTES");
            strQry.AppendLine("   ,#TIMESHEET_OR_BREAK#_TIME_OUT_MINUTES) AS SORTED_REC");
            strQry.AppendLine("   ,COMPANY_NO");
            strQry.AppendLine("   ,PAY_CATEGORY_NO");
            strQry.AppendLine("   ,EMPLOYEE_NO");
            strQry.AppendLine("   ,#DATE_FIELD#");
            strQry.AppendLine("   ,#TIMESHEET_OR_BREAK#_SEQ");
            strQry.AppendLine("   ,#TIMESHEET_OR_BREAK#_TIME_IN_MINUTES");
            strQry.AppendLine("   ,#TIMESHEET_OR_BREAK#_TIME_OUT_MINUTES");
            strQry.AppendLine("");
            strQry.AppendLine("    FROM dbo.#TRIGGER_TABLE_NAME#) AS ETC2");
            strQry.AppendLine("");
            strQry.AppendLine("    ON E.COMPANY_NO = ETC2.COMPANY_NO");
            strQry.AppendLine("    AND E.EMPLOYEE_NO = ETC2.EMPLOYEE_NO");
            strQry.AppendLine("    AND D.PAY_CATEGORY_NO = ETC2.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND D.#TIMESHEET_OR_BREAK#_DATE = ETC2.#DATE_FIELD#");
            strQry.AppendLine("");
            strQry.AppendLine("    WHERE E.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("    AND ((ETC.#TIMESHEET_OR_BREAK#_TIME_IN_MINUTES IS NULL");
            strQry.AppendLine("    OR ETC.#TIMESHEET_OR_BREAK#_TIME_OUT_MINUTES IS NULL)");
            strQry.AppendLine("    OR (ETC.#TIMESHEET_OR_BREAK#_TIME_IN_MINUTES >= ETC2.#TIMESHEET_OR_BREAK#_TIME_OUT_MINUTES");
            strQry.AppendLine("    AND ETC.SORTED_REC <= ETC2.SORTED_REC)");
            strQry.AppendLine("    OR (ETC.#TIMESHEET_OR_BREAK#_TIME_IN_MINUTES < ETC2.#TIMESHEET_OR_BREAK#_TIME_OUT_MINUTES");
            strQry.AppendLine("    AND ETC.SORTED_REC > ETC2.SORTED_REC))");
            strQry.AppendLine("");
            strQry.AppendLine("    --2End");
            strQry.AppendLine("   ) AS TEMP");
            strQry.AppendLine("    ON ETC.EMPLOYEE_NO = TEMP.EMPLOYEE_NO");
            strQry.AppendLine("    AND ETC.#DATE_FIELD# = TEMP.#TIMESHEET_OR_BREAK#_DATE");
            strQry.AppendLine("    AND ETC.#TIMESHEET_OR_BREAK#_SEQ = TEMP.#TIMESHEET_OR_BREAK#_SEQ");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine("    ON ETC.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine("    AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine("    AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND EPC.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN dbo.PAY_CATEGORY PC");
            strQry.AppendLine("    ON ETC.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine("    AND ETC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND PC.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("");
            strQry.AppendLine("    WHERE E.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("   ) AS TEMP_TABLE");
            strQry.AppendLine("    ON ETC.COMPANY_NO = TEMP_TABLE.COMPANY_NO");
            strQry.AppendLine("    AND ETC.EMPLOYEE_NO = TEMP_TABLE.EMPLOYEE_NO");
            strQry.AppendLine("    AND ETC.PAY_CATEGORY_NO = TEMP_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND ETC.#DATE_FIELD# = TEMP_TABLE.#DATE_FIELD#");
            strQry.AppendLine("    AND ETC.#TIMESHEET_OR_BREAK#_SEQ = TEMP_TABLE.#TIMESHEET_OR_BREAK#_SEQ");

            strQry.AppendLine("");
            strQry.AppendLine("");
            strQry.AppendLine("    UPDATE ETBDC");
            strQry.AppendLine("");
            strQry.AppendLine("    SET");
            strQry.AppendLine("");
            strQry.AppendLine("    DAY_NO = TEMP_TABLE.DAY_NO");
            strQry.AppendLine("   ,DAY_PAID_MINUTES = ISNULL(TEMP_TABLE.DAY_PAID_MINUTES,0)");
            strQry.AppendLine("   ,BREAK_ACCUM_MINUTES = ISNULL(TEMP_TABLE.BREAK_ACCUM_MINUTES,0)");
            strQry.AppendLine("   ,INDICATOR = TEMP_TABLE.INDICATOR");
            strQry.AppendLine("   ,BREAK_INDICATOR = TEMP_TABLE.BREAK_INDICATOR");
            strQry.AppendLine("");
            strQry.AppendLine("    FROM dbo.#DAY_TABLE# ETBDC");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN");
            strQry.AppendLine("   (");
            strQry.AppendLine("    SELECT");
            strQry.AppendLine("    E.COMPANY_NO");
            strQry.AppendLine("   ,BREAK_SUMMARY_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,E.EMPLOYEE_NO");
            strQry.AppendLine("   ,BREAK_SUMMARY_TABLE.DAY_DATE");
            strQry.AppendLine("   ,D.DAY_NO");
            strQry.AppendLine("   ,0 AS DAY_PAID_MINUTES");
            strQry.AppendLine("   ,BREAK_SUMMARY_TABLE.INDICATOR");
            strQry.AppendLine("   ,BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES");
            strQry.AppendLine("   ,'' AS BREAK_INDICATOR");
            strQry.AppendLine("");
            strQry.AppendLine("    FROM dbo.EMPLOYEE E");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN");
            strQry.AppendLine("   (");
            strQry.AppendLine("    --2Start Break UNION 1");
            strQry.AppendLine("    SELECT");
            strQry.AppendLine("    BREAK_TABLE.COMPANY_NO");
            strQry.AppendLine("   ,BREAK_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine("   ,BREAK_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,BREAK_TABLE.EMPLOYEE_NO");
            strQry.AppendLine("   ,BREAK_TABLE.DAY_DATE");
            strQry.AppendLine("   ,SUM(BREAK_TABLE.BREAK_ACCUM_MINUTES) AS BREAK_ACCUM_MINUTES");
            strQry.AppendLine("   ,INDICATOR = 'X'");
            strQry.AppendLine("");
            strQry.AppendLine("    FROM");
            strQry.AppendLine("   (");
            strQry.AppendLine("    --1Start Break UNION 1");
            strQry.AppendLine("    SELECT DISTINCT");
            strQry.AppendLine("    EBC.COMPANY_NO");
            strQry.AppendLine("   ,'#PAY_CATEGORY_TYPE#' AS PAY_CATEGORY_TYPE");
            strQry.AppendLine("   ,EBC.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,EBC.EMPLOYEE_NO");
            strQry.AppendLine("   ,EBC.BREAK_DATE AS DAY_DATE");
            strQry.AppendLine("   ,EBC.BREAK_SEQ");
            strQry.AppendLine("   ,BREAK_ACCUM_MINUTES = ");
            strQry.AppendLine("    CASE");
            strQry.AppendLine("    WHEN ((EBC.BREAK_TIME_IN_MINUTES IS NULL");
            strQry.AppendLine("    OR EBC.BREAK_TIME_OUT_MINUTES IS NULL)");
            strQry.AppendLine("    OR (EBC.BREAK_TIME_IN_MINUTES >= EBC2.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine("    AND EBC.SORTED_REC <= EBC2.SORTED_REC)");
            strQry.AppendLine("    OR (EBC.BREAK_TIME_IN_MINUTES < EBC2.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine("    AND EBC.SORTED_REC > EBC2.SORTED_REC)) THEN");
            strQry.AppendLine("    CASE");
            strQry.AppendLine("    WHEN EBC.BREAK_TIME_OUT_MINUTES IS NULL OR EBC.BREAK_TIME_IN_MINUTES IS NULL");
            strQry.AppendLine("    THEN 0");
            strQry.AppendLine("    WHEN EBC.BREAK_TIME_OUT_MINUTES > EBC.BREAK_TIME_IN_MINUTES THEN");
            strQry.AppendLine("    EBC.BREAK_TIME_OUT_MINUTES - EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine("    ELSE 0");
            strQry.AppendLine("    END");
            strQry.AppendLine("    ELSE");
            strQry.AppendLine("    EBC.BREAK_TIME_OUT_MINUTES - EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine("    END");
            strQry.AppendLine("");
            strQry.AppendLine("    FROM dbo.EMPLOYEE E");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN");
            strQry.AppendLine("   (");
            strQry.AppendLine("    SELECT");
            strQry.AppendLine("    ROW_NUMBER() OVER");
            strQry.AppendLine("   (ORDER BY");
            strQry.AppendLine("    EBC.COMPANY_NO");
            strQry.AppendLine("   ,EBC.EMPLOYEE_NO");
            strQry.AppendLine("   ,EBC.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,EBC.BREAK_DATE");
            strQry.AppendLine("   ,EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine("   ,EBC.BREAK_TIME_OUT_MINUTES) AS SORTED_REC");
            strQry.AppendLine("   ,EBC.COMPANY_NO");
            strQry.AppendLine("   ,EBC.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,EBC.EMPLOYEE_NO");
            strQry.AppendLine("   ,EBC.BREAK_DATE");
            strQry.AppendLine("   ,EBC.BREAK_SEQ");
            strQry.AppendLine("   ,EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine("   ,EBC.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine("");
            strQry.AppendLine("    FROM dbo.#BREAK_TABLE# EBC");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN");
            strQry.AppendLine("   (");
            strQry.AppendLine("    SELECT DISTINCT");
            strQry.AppendLine("    COMPANY_NO");
            strQry.AppendLine("   ,PAY_CATEGORY_NO");
            strQry.AppendLine("   ,EMPLOYEE_NO");
            strQry.AppendLine("   ,#DATE_FIELD#");
            strQry.AppendLine("    FROM DELETED");
            strQry.AppendLine("   ) AS D");
            strQry.AppendLine("    ON EBC.COMPANY_NO = D.COMPANY_NO");
            strQry.AppendLine("    AND EBC.EMPLOYEE_NO = D.EMPLOYEE_NO");
            strQry.AppendLine("    AND EBC.PAY_CATEGORY_NO = D.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND EBC.BREAK_DATE = D.#TIMESHEET_OR_BREAK#_DATE");
            strQry.AppendLine("   ) AS EBC");
            strQry.AppendLine("    ON E.COMPANY_NO = EBC.COMPANY_NO");
            strQry.AppendLine("    AND E.EMPLOYEE_NO = EBC.EMPLOYEE_NO");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN");
            strQry.AppendLine("   (");
            strQry.AppendLine("    SELECT");
            strQry.AppendLine("    ROW_NUMBER() OVER");
            strQry.AppendLine("   (ORDER BY");
            strQry.AppendLine("    EBC.COMPANY_NO");
            strQry.AppendLine("   ,EBC.EMPLOYEE_NO");
            strQry.AppendLine("   ,EBC.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,EBC.BREAK_DATE");
            strQry.AppendLine("   ,EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine("   ,EBC.BREAK_TIME_OUT_MINUTES) AS SORTED_REC");
            strQry.AppendLine("   ,EBC.COMPANY_NO");
            strQry.AppendLine("   ,EBC.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,EBC.EMPLOYEE_NO");
            strQry.AppendLine("   ,EBC.BREAK_DATE");
            strQry.AppendLine("   ,EBC.BREAK_SEQ");
            strQry.AppendLine("   ,EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine("   ,EBC.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine("");
            strQry.AppendLine("    FROM dbo.#BREAK_TABLE# EBC");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN");
            strQry.AppendLine("   (");
            strQry.AppendLine("    SELECT DISTINCT");
            strQry.AppendLine("    COMPANY_NO");
            strQry.AppendLine("   ,PAY_CATEGORY_NO");
            strQry.AppendLine("   ,EMPLOYEE_NO");
            strQry.AppendLine("   ,#DATE_FIELD#");
            strQry.AppendLine("    FROM DELETED");
            strQry.AppendLine("   ) AS D");
            strQry.AppendLine("    ON EBC.COMPANY_NO = D.COMPANY_NO");
            strQry.AppendLine("    AND EBC.EMPLOYEE_NO = D.EMPLOYEE_NO");
            strQry.AppendLine("    AND EBC.PAY_CATEGORY_NO = D.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND EBC.BREAK_DATE = D.#TIMESHEET_OR_BREAK#_DATE");
            strQry.AppendLine("   ) AS EBC2");
            strQry.AppendLine("    ON EBC.COMPANY_NO = EBC2.COMPANY_NO");
            strQry.AppendLine("    AND EBC.EMPLOYEE_NO = EBC2.EMPLOYEE_NO");
            strQry.AppendLine("    AND EBC.PAY_CATEGORY_NO = EBC2.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND EBC.BREAK_DATE = EBC2.BREAK_DATE");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine("    ON EBC.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine("    AND EBC.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine("    AND EBC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND EPC.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN dbo.PAY_CATEGORY PC");
            strQry.AppendLine("    ON EBC.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine("    AND EBC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND PC.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("");
            strQry.AppendLine("    WHERE E.COMPANY_NO = EBC.COMPANY_NO");
            strQry.AppendLine("    AND E.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("    --1End Break UNION 1");
            strQry.AppendLine("   ) AS BREAK_TABLE");
            strQry.AppendLine("");
            strQry.AppendLine("    GROUP BY");
            strQry.AppendLine("    BREAK_TABLE.COMPANY_NO");
            strQry.AppendLine("   ,BREAK_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine("   ,BREAK_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,BREAK_TABLE.EMPLOYEE_NO");
            strQry.AppendLine("   ,BREAK_TABLE.DAY_DATE");
            strQry.AppendLine("    --2End Break UNION 1");
            strQry.AppendLine("   ) AS BREAK_SUMMARY_TABLE");
            strQry.AppendLine("    ON E.COMPANY_NO = BREAK_SUMMARY_TABLE.COMPANY_NO");
            strQry.AppendLine("    AND E.EMPLOYEE_NO = BREAK_SUMMARY_TABLE.EMPLOYEE_NO");
            strQry.AppendLine("    AND E.PAY_CATEGORY_TYPE = BREAK_SUMMARY_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine("");
            strQry.AppendLine("    LEFT JOIN dbo.#TIMESHEET_TABLE# ETC");
            strQry.AppendLine("    ON BREAK_SUMMARY_TABLE.COMPANY_NO = ETC.COMPANY_NO");
            strQry.AppendLine("    AND BREAK_SUMMARY_TABLE.EMPLOYEE_NO = ETC.EMPLOYEE_NO");
            strQry.AppendLine("    AND BREAK_SUMMARY_TABLE.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND BREAK_SUMMARY_TABLE.DAY_DATE = ETC.TIMESHEET_DATE");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN dbo.DATES D");
            strQry.AppendLine("    ON BREAK_SUMMARY_TABLE.DAY_DATE = D.DAY_DATE");
            strQry.AppendLine("");
            strQry.AppendLine("    WHERE E.COMPANY_NO = BREAK_SUMMARY_TABLE.COMPANY_NO");
            strQry.AppendLine("    AND E.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("    AND ETC.TIMESHEET_DATE IS NULL");
            strQry.AppendLine("");
            strQry.AppendLine("    UNION");
            strQry.AppendLine("");
            strQry.AppendLine("    --3Start Major UNION 2");
            strQry.AppendLine("    SELECT");
            strQry.AppendLine("    TEMP2_TABLE.COMPANY_NO");
            strQry.AppendLine("   ,TEMP2_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,TEMP2_TABLE.EMPLOYEE_NO");
            strQry.AppendLine("   ,TEMP2_TABLE.DAY_DATE");
            strQry.AppendLine("   ,D.DAY_NO");
            strQry.AppendLine("   ,TEMP2_TABLE.DAY_PAID_MINUTES");
            strQry.AppendLine("   ,INDICATOR = ");
            strQry.AppendLine("    CASE");
            strQry.AppendLine("    WHEN TEMP2_TABLE.INDICATOR = 'X'");
            strQry.AppendLine("    THEN TEMP2_TABLE.INDICATOR");
            strQry.AppendLine("    WHEN D.DAY_NO = 0");
            strQry.AppendLine("    AND (TEMP2_TABLE.DAY_PAID_MINUTES > PC2.EXCEPTION_SUN_ABOVE_MINUTES");
            strQry.AppendLine("    OR TEMP2_TABLE.DAY_PAID_MINUTES < PC2.EXCEPTION_SUN_BELOW_MINUTES)");
            strQry.AppendLine("    THEN 'E'");
            strQry.AppendLine("    WHEN D.DAY_NO = 1");
            strQry.AppendLine("    AND (TEMP2_TABLE.DAY_PAID_MINUTES > PC2.EXCEPTION_MON_ABOVE_MINUTES");
            strQry.AppendLine("    OR TEMP2_TABLE.DAY_PAID_MINUTES < PC2.EXCEPTION_MON_BELOW_MINUTES)");
            strQry.AppendLine("    THEN 'E'");
            strQry.AppendLine("    WHEN D.DAY_NO = 2");
            strQry.AppendLine("    AND (TEMP2_TABLE.DAY_PAID_MINUTES > PC2.EXCEPTION_TUE_ABOVE_MINUTES");
            strQry.AppendLine("    OR TEMP2_TABLE.DAY_PAID_MINUTES < PC2.EXCEPTION_TUE_BELOW_MINUTES)");
            strQry.AppendLine("    THEN 'E'");
            strQry.AppendLine("    WHEN D.DAY_NO = 3");
            strQry.AppendLine("    AND (TEMP2_TABLE.DAY_PAID_MINUTES > PC2.EXCEPTION_WED_ABOVE_MINUTES");
            strQry.AppendLine("    OR TEMP2_TABLE.DAY_PAID_MINUTES < PC2.EXCEPTION_WED_BELOW_MINUTES)");
            strQry.AppendLine("    THEN 'E'");
            strQry.AppendLine("    WHEN D.DAY_NO = 4");
            strQry.AppendLine("    AND (TEMP2_TABLE.DAY_PAID_MINUTES > PC2.EXCEPTION_THU_ABOVE_MINUTES");
            strQry.AppendLine("    OR TEMP2_TABLE.DAY_PAID_MINUTES < PC2.EXCEPTION_THU_BELOW_MINUTES)");
            strQry.AppendLine("    THEN 'E'");
            strQry.AppendLine("    WHEN D.DAY_NO = 5");
            strQry.AppendLine("    AND (TEMP2_TABLE.DAY_PAID_MINUTES > PC2.EXCEPTION_FRI_ABOVE_MINUTES");
            strQry.AppendLine("    OR TEMP2_TABLE.DAY_PAID_MINUTES < PC2.EXCEPTION_FRI_BELOW_MINUTES)");
            strQry.AppendLine("    THEN 'E'");
            strQry.AppendLine("    WHEN D.DAY_NO = 6");
            strQry.AppendLine("    AND (TEMP2_TABLE.DAY_PAID_MINUTES > PC2.EXCEPTION_SAT_ABOVE_MINUTES");
            strQry.AppendLine("    OR TEMP2_TABLE.DAY_PAID_MINUTES < PC2.EXCEPTION_SAT_BELOW_MINUTES)");
            strQry.AppendLine("    THEN 'E'");
            strQry.AppendLine("    ELSE TEMP2_TABLE.INDICATOR");
            strQry.AppendLine("    END");
            strQry.AppendLine("   ,TEMP2_TABLE.BREAK_ACCUM_MINUTES");
            strQry.AppendLine("   ,TEMP2_TABLE.BREAK_INDICATOR");
            strQry.AppendLine("");
            strQry.AppendLine("    FROM");
            strQry.AppendLine("   (");
            strQry.AppendLine("    --2Start Major UNION 2");
            strQry.AppendLine("    SELECT");
            strQry.AppendLine("    TEMP1_TABLE.COMPANY_NO");
            strQry.AppendLine("   ,TEMP1_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine("   ,TEMP1_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,TEMP1_TABLE.EMPLOYEE_NO");
            strQry.AppendLine("   ,TEMP1_TABLE.DAY_DATE");
            strQry.AppendLine("   ,DAY_PAID_MINUTES = ");
            strQry.AppendLine("    CASE");
            strQry.AppendLine("    WHEN PC1.DAILY_ROUNDING_IND = 0");
            strQry.AppendLine("    THEN TEMP1_TABLE.DAY_PAID_MINUTES");
            strQry.AppendLine("    WHEN PC1.DAILY_ROUNDING_IND = 1");
            strQry.AppendLine("    THEN CASE");
            strQry.AppendLine("    WHEN TEMP1_TABLE.DAY_PAID_MINUTES % PC1.DAILY_ROUNDING_MINUTES = 0");
            strQry.AppendLine("    THEN TEMP1_TABLE.DAY_PAID_MINUTES");
            strQry.AppendLine("    ELSE TEMP1_TABLE.DAY_PAID_MINUTES + (PC1.DAILY_ROUNDING_MINUTES - (TEMP1_TABLE.DAY_PAID_MINUTES % PC1.DAILY_ROUNDING_MINUTES))");
            strQry.AppendLine("    END");
            strQry.AppendLine("    WHEN PC1.DAILY_ROUNDING_IND = 2");
            strQry.AppendLine("    THEN CASE");
            strQry.AppendLine("    WHEN TEMP1_TABLE.DAY_PAID_MINUTES % PC1.DAILY_ROUNDING_MINUTES = 0");
            strQry.AppendLine("    THEN TEMP1_TABLE.DAY_PAID_MINUTES");
            strQry.AppendLine("    ELSE TEMP1_TABLE.DAY_PAID_MINUTES - (TEMP1_TABLE.DAY_PAID_MINUTES % PC1.DAILY_ROUNDING_MINUTES)");
            strQry.AppendLine("    END");
            strQry.AppendLine("    WHEN PC1.DAILY_ROUNDING_IND = 3");
            strQry.AppendLine("    THEN CASE");
            strQry.AppendLine("    WHEN TEMP1_TABLE.DAY_PAID_MINUTES % PC1.DAILY_ROUNDING_MINUTES = 0");
            strQry.AppendLine("    THEN TEMP1_TABLE.DAY_PAID_MINUTES");
            strQry.AppendLine("    WHEN TEMP1_TABLE.DAY_PAID_MINUTES % PC1.DAILY_ROUNDING_MINUTES > CONVERT(DECIMAL,PC1.DAILY_ROUNDING_MINUTES) / 2");
            strQry.AppendLine("    THEN TEMP1_TABLE.DAY_PAID_MINUTES + (PC1.DAILY_ROUNDING_MINUTES - (TEMP1_TABLE.DAY_PAID_MINUTES % PC1.DAILY_ROUNDING_MINUTES))");
            strQry.AppendLine("    ELSE TEMP1_TABLE.DAY_PAID_MINUTES - (TEMP1_TABLE.DAY_PAID_MINUTES % PC1.DAILY_ROUNDING_MINUTES)");
            strQry.AppendLine("    END");
            strQry.AppendLine("    END");
            strQry.AppendLine("   ,TEMP1_TABLE.INDICATOR");
            strQry.AppendLine("   ,TEMP1_TABLE.BREAK_ACCUM_MINUTES");
            strQry.AppendLine("   ,TEMP1_TABLE.BREAK_INDICATOR");
            strQry.AppendLine("");
            strQry.AppendLine("    FROM");
            strQry.AppendLine("   (");
            strQry.AppendLine("    --1Start Major UNION 2");
            strQry.AppendLine("    SELECT");
            strQry.AppendLine("    TIMESHEET_TOTAL_TABLE.COMPANY_NO");
            strQry.AppendLine("   ,TIMESHEET_TOTAL_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine("   ,TIMESHEET_TOTAL_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,TIMESHEET_TOTAL_TABLE.EMPLOYEE_NO");
            strQry.AppendLine("   ,TIMESHEET_TOTAL_TABLE.DAY_DATE");
            strQry.AppendLine("   ,ISNULL(BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES,0) AS BREAK_ACCUM_MINUTES");
            strQry.AppendLine("   ,DAY_PAID_MINUTES = ");
            strQry.AppendLine("    CASE");
            strQry.AppendLine("    WHEN BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES > TIMESHEET_TOTAL_TABLE.BREAK_MINUTES");
            strQry.AppendLine("    AND BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES < TIMESHEET_TOTAL_TABLE.TIMESHEET_ACCUM_MINUTES");
            strQry.AppendLine("    THEN TIMESHEET_TOTAL_TABLE.TIMESHEET_ACCUM_MINUTES - BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES");
            strQry.AppendLine("    ELSE TIMESHEET_TOTAL_TABLE.TIMESHEET_ACCUM_MINUTES - TIMESHEET_TOTAL_TABLE.BREAK_MINUTES");
            strQry.AppendLine("    END");
            strQry.AppendLine("   ,INDICATOR = ");
            strQry.AppendLine("    CASE");
            strQry.AppendLine("    WHEN BREAK_SUMMARY_TABLE.INDICATOR = 'X' OR TIMESHEET_TOTAL_TABLE.TIMESHEET_ACCUM_MINUTES < BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES");
            strQry.AppendLine("    THEN 'X'");
            strQry.AppendLine("    ELSE ISNULL(MAX(TIMESHEET_TOTAL_TABLE.INDICATOR),'')");
            strQry.AppendLine("    END");
            strQry.AppendLine("   ,BREAK_INDICATOR = ");
            strQry.AppendLine("    CASE");
            strQry.AppendLine("    WHEN BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES > TIMESHEET_TOTAL_TABLE.BREAK_MINUTES");
            strQry.AppendLine("    THEN 'Y'");
            strQry.AppendLine("    ELSE ''");
            strQry.AppendLine("    END");
            strQry.AppendLine("");
            strQry.AppendLine("    FROM");
            strQry.AppendLine("   (");
            strQry.AppendLine("    --3Start Timesheet UNION 2");
            strQry.AppendLine("    SELECT");
            strQry.AppendLine("    TIMESHEET_SUMMARY_TABLE.COMPANY_NO");
            strQry.AppendLine("   ,TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine("   ,TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,TIMESHEET_SUMMARY_TABLE.EMPLOYEE_NO");
            strQry.AppendLine("   ,TIMESHEET_SUMMARY_TABLE.DAY_DATE");
            strQry.AppendLine("   ,TIMESHEET_SUMMARY_TABLE.TIMESHEET_ACCUM_MINUTES");
            strQry.AppendLine("   ,TIMESHEET_SUMMARY_TABLE.INDICATOR");
            strQry.AppendLine("   ,ISNULL(MAX(PCB.BREAK_MINUTES),0) AS BREAK_MINUTES");
            strQry.AppendLine("");
            strQry.AppendLine("    FROM");
            strQry.AppendLine("   (");
            strQry.AppendLine("    --2Start Timesheet UNION 2");
            strQry.AppendLine("    SELECT");
            strQry.AppendLine("    TIMESHEET_TABLE.COMPANY_NO");
            strQry.AppendLine("   ,TIMESHEET_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine("   ,TIMESHEET_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,TIMESHEET_TABLE.EMPLOYEE_NO");
            strQry.AppendLine("   ,TIMESHEET_TABLE.DAY_DATE");
            strQry.AppendLine("   ,SUM(TIMESHEET_ACCUM_MINUTES) AS TIMESHEET_ACCUM_MINUTES");
            strQry.AppendLine("   ,MAX(ISNULL(INDICATOR,'')) AS INDICATOR");
            strQry.AppendLine("");
            strQry.AppendLine("    FROM");
            strQry.AppendLine("   (");
            strQry.AppendLine("    --1Start Timesheet UNION 2");
            strQry.AppendLine("    SELECT DISTINCT");
            strQry.AppendLine("    ETC.COMPANY_NO");
            strQry.AppendLine("   ,'#PAY_CATEGORY_TYPE#' AS PAY_CATEGORY_TYPE");
            strQry.AppendLine("   ,ETC.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,ETC.EMPLOYEE_NO");
            strQry.AppendLine("   ,ETC.TIMESHEET_DATE AS DAY_DATE");
            strQry.AppendLine("   ,ETC.TIMESHEET_SEQ");
            strQry.AppendLine("   ,TIMESHEET_ACCUM_MINUTES = ");
            strQry.AppendLine("    CASE");
            strQry.AppendLine("    WHEN NOT TEMP.EMPLOYEE_NO IS NULL THEN");
            strQry.AppendLine("    CASE");
            strQry.AppendLine("    WHEN ETC.TIMESHEET_TIME_OUT_MINUTES IS NULL OR ETC.TIMESHEET_TIME_IN_MINUTES IS NULL");
            strQry.AppendLine("    THEN 0");
            strQry.AppendLine("    WHEN ETC.TIMESHEET_TIME_OUT_MINUTES > ETC.TIMESHEET_TIME_IN_MINUTES THEN");
            strQry.AppendLine("    ETC.TIMESHEET_TIME_OUT_MINUTES - ETC.TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine("    ELSE 0");
            strQry.AppendLine("    END");
            strQry.AppendLine("    ELSE");
            strQry.AppendLine("    ETC.TIMESHEET_TIME_OUT_MINUTES - ETC.TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine("    END");
            strQry.AppendLine("   ,INDICATOR = ");
            strQry.AppendLine("    CASE");
            strQry.AppendLine("    WHEN NOT TEMP.EMPLOYEE_NO IS NULL");
            strQry.AppendLine("    THEN 'X'");
            strQry.AppendLine("    ELSE ''");
            strQry.AppendLine("    END");
            strQry.AppendLine("");
            strQry.AppendLine("    FROM dbo.EMPLOYEE E");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN dbo.#TIMESHEET_TABLE# ETC");
            strQry.AppendLine("    ON E.COMPANY_NO = ETC.COMPANY_NO");
            strQry.AppendLine("    AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN");
            strQry.AppendLine("   (");
            strQry.AppendLine("    SELECT DISTINCT");
            strQry.AppendLine("    COMPANY_NO");
            strQry.AppendLine("   ,PAY_CATEGORY_NO");
            strQry.AppendLine("   ,EMPLOYEE_NO");
            strQry.AppendLine("   ,#DATE_FIELD#");
            strQry.AppendLine("    FROM DELETED");
            strQry.AppendLine("   ) AS D");
            strQry.AppendLine("    ON ETC.COMPANY_NO = D.COMPANY_NO");
            strQry.AppendLine("    AND ETC.EMPLOYEE_NO = D.EMPLOYEE_NO");
            strQry.AppendLine("    AND ETC.PAY_CATEGORY_NO = D.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND ETC.TIMESHEET_DATE = D.#TIMESHEET_OR_BREAK#_DATE");
            strQry.AppendLine("");
            strQry.AppendLine("    LEFT JOIN");
            strQry.AppendLine("   (");
            strQry.AppendLine("    SELECT DISTINCT");
            strQry.AppendLine("    ETC.EMPLOYEE_NO");
            strQry.AppendLine("   ,ETC.TIMESHEET_DATE");
            strQry.AppendLine("   ,ETC.TIMESHEET_SEQ");
            strQry.AppendLine("");
            strQry.AppendLine("    FROM dbo.EMPLOYEE E");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN");
            strQry.AppendLine("   (");
            strQry.AppendLine("    SELECT");
            strQry.AppendLine("    ROW_NUMBER() OVER");
            strQry.AppendLine("   (ORDER BY");
            strQry.AppendLine("    ETC.COMPANY_NO");
            strQry.AppendLine("   ,ETC.EMPLOYEE_NO");
            strQry.AppendLine("   ,ETC.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,ETC.TIMESHEET_DATE");
            strQry.AppendLine("   ,ETC.TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine("   ,ETC.TIMESHEET_TIME_OUT_MINUTES) AS SORTED_REC");
            strQry.AppendLine("   ,ETC.COMPANY_NO");
            strQry.AppendLine("   ,ETC.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,ETC.EMPLOYEE_NO");
            strQry.AppendLine("   ,ETC.TIMESHEET_DATE");
            strQry.AppendLine("   ,ETC.TIMESHEET_SEQ");
            strQry.AppendLine("   ,ETC.TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine("   ,ETC.TIMESHEET_TIME_OUT_MINUTES");
            strQry.AppendLine("");
            strQry.AppendLine("    FROM dbo.#TIMESHEET_TABLE# ETC");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN");
            strQry.AppendLine("   (");
            strQry.AppendLine("    SELECT DISTINCT");
            strQry.AppendLine("    COMPANY_NO");
            strQry.AppendLine("   ,PAY_CATEGORY_NO");
            strQry.AppendLine("   ,EMPLOYEE_NO");
            strQry.AppendLine("   ,#DATE_FIELD#");
            strQry.AppendLine("    FROM DELETED");
            strQry.AppendLine("   ) AS D");
            strQry.AppendLine("    ON ETC.COMPANY_NO = D.COMPANY_NO");
            strQry.AppendLine("    AND ETC.EMPLOYEE_NO = D.EMPLOYEE_NO");
            strQry.AppendLine("    AND ETC.PAY_CATEGORY_NO = D.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND ETC.TIMESHEET_DATE = D.#TIMESHEET_OR_BREAK#_DATE");
            strQry.AppendLine("   ) AS ETC");
            strQry.AppendLine("    ON E.COMPANY_NO = ETC.COMPANY_NO");
            strQry.AppendLine("    AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN");
            strQry.AppendLine("   (");
            strQry.AppendLine("    SELECT");
            strQry.AppendLine("    ROW_NUMBER() OVER");
            strQry.AppendLine("   (ORDER BY");
            strQry.AppendLine("    ETC.COMPANY_NO");
            strQry.AppendLine("   ,ETC.EMPLOYEE_NO");
            strQry.AppendLine("   ,ETC.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,ETC.TIMESHEET_DATE");
            strQry.AppendLine("   ,ETC.TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine("   ,ETC.TIMESHEET_TIME_OUT_MINUTES) AS SORTED_REC");
            strQry.AppendLine("   ,ETC.COMPANY_NO");
            strQry.AppendLine("   ,ETC.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,ETC.EMPLOYEE_NO");
            strQry.AppendLine("   ,ETC.TIMESHEET_DATE");
            strQry.AppendLine("   ,ETC.TIMESHEET_SEQ");
            strQry.AppendLine("   ,ETC.TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine("   ,ETC.TIMESHEET_TIME_OUT_MINUTES");
            strQry.AppendLine("");
            strQry.AppendLine("    FROM dbo.#TIMESHEET_TABLE# ETC");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN");
            strQry.AppendLine("   (");
            strQry.AppendLine("    SELECT DISTINCT");
            strQry.AppendLine("    COMPANY_NO");
            strQry.AppendLine("   ,PAY_CATEGORY_NO");
            strQry.AppendLine("   ,EMPLOYEE_NO");
            strQry.AppendLine("   ,#DATE_FIELD#");
            strQry.AppendLine("    FROM DELETED");
            strQry.AppendLine("   ) AS D");
            strQry.AppendLine("    ON ETC.COMPANY_NO = D.COMPANY_NO");
            strQry.AppendLine("    AND ETC.EMPLOYEE_NO = D.EMPLOYEE_NO");
            strQry.AppendLine("    AND ETC.PAY_CATEGORY_NO = D.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND ETC.TIMESHEET_DATE = D.#TIMESHEET_OR_BREAK#_DATE");
            strQry.AppendLine("   ) AS ETC2");
            strQry.AppendLine("    ON ETC.COMPANY_NO = ETC2.COMPANY_NO");
            strQry.AppendLine("    AND ETC.PAY_CATEGORY_NO = ETC2.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND ETC.EMPLOYEE_NO = ETC2.EMPLOYEE_NO");
            strQry.AppendLine("    AND ETC.TIMESHEET_DATE = ETC2.TIMESHEET_DATE");
            strQry.AppendLine("");
            strQry.AppendLine("    WHERE E.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("    AND ((ETC.TIMESHEET_TIME_IN_MINUTES IS NULL");
            strQry.AppendLine("    OR ETC.TIMESHEET_TIME_OUT_MINUTES IS NULL)");
            strQry.AppendLine("    OR (ETC.TIMESHEET_TIME_IN_MINUTES >= ETC2.TIMESHEET_TIME_OUT_MINUTES");
            strQry.AppendLine("    AND ETC.SORTED_REC <= ETC2.SORTED_REC)");
            strQry.AppendLine("    OR (ETC.TIMESHEET_TIME_IN_MINUTES < ETC2.TIMESHEET_TIME_OUT_MINUTES");
            strQry.AppendLine("    AND ETC.SORTED_REC > ETC2.SORTED_REC))");
            strQry.AppendLine("   ) AS TEMP");
            strQry.AppendLine("    ON ETC.EMPLOYEE_NO = TEMP.EMPLOYEE_NO");
            strQry.AppendLine("    AND ETC.TIMESHEET_DATE = TEMP.TIMESHEET_DATE");
            strQry.AppendLine("    AND ETC.TIMESHEET_SEQ = TEMP.TIMESHEET_SEQ");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine("    ON ETC.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine("    AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine("    AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND EPC.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN dbo.PAY_CATEGORY PC");
            strQry.AppendLine("    ON ETC.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine("    AND ETC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND PC.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("");
            strQry.AppendLine("    WHERE E.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("    --1End Timesheet UNION 2");
            strQry.AppendLine("   ) AS TIMESHEET_TABLE");
            strQry.AppendLine("");
            strQry.AppendLine("    GROUP BY");
            strQry.AppendLine("    TIMESHEET_TABLE.COMPANY_NO");
            strQry.AppendLine("   ,TIMESHEET_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine("   ,TIMESHEET_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,TIMESHEET_TABLE.EMPLOYEE_NO");
            strQry.AppendLine("   ,TIMESHEET_TABLE.DAY_DATE");
            strQry.AppendLine("    --2End Timesheet UNION 2");
            strQry.AppendLine("   ) AS TIMESHEET_SUMMARY_TABLE");
            strQry.AppendLine("");
            strQry.AppendLine("    LEFT JOIN dbo.PAY_CATEGORY_BREAK PCB");
            strQry.AppendLine("    ON TIMESHEET_SUMMARY_TABLE.COMPANY_NO = PCB.COMPANY_NO");
            strQry.AppendLine("    AND TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_NO = PCB.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_TYPE = PCB.PAY_CATEGORY_TYPE");
            strQry.AppendLine("    AND TIMESHEET_SUMMARY_TABLE.TIMESHEET_ACCUM_MINUTES >= PCB.WORKED_TIME_MINUTES");
            strQry.AppendLine("");
            strQry.AppendLine("    GROUP BY");
            strQry.AppendLine("    TIMESHEET_SUMMARY_TABLE.COMPANY_NO");
            strQry.AppendLine("   ,TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine("   ,TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,TIMESHEET_SUMMARY_TABLE.EMPLOYEE_NO");
            strQry.AppendLine("   ,TIMESHEET_SUMMARY_TABLE.DAY_DATE");
            strQry.AppendLine("   ,TIMESHEET_SUMMARY_TABLE.TIMESHEET_ACCUM_MINUTES");
            strQry.AppendLine("   ,TIMESHEET_SUMMARY_TABLE.INDICATOR");
            strQry.AppendLine("    --3End Timesheet UNION 2");
            strQry.AppendLine("   ) AS TIMESHEET_TOTAL_TABLE");
            strQry.AppendLine("");
            strQry.AppendLine("    LEFT JOIN");
            strQry.AppendLine("   (");
            strQry.AppendLine("    --3Start Break UNION 2");
            strQry.AppendLine("    SELECT");
            strQry.AppendLine("    BREAK_TABLE.COMPANY_NO");
            strQry.AppendLine("   ,BREAK_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine("   ,BREAK_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,BREAK_TABLE.EMPLOYEE_NO");
            strQry.AppendLine("   ,BREAK_TABLE.DAY_DATE");
            strQry.AppendLine("   ,SUM(BREAK_TABLE.BREAK_ACCUM_MINUTES) AS BREAK_ACCUM_MINUTES");
            strQry.AppendLine("   ,MAX(ISNULL(BREAK_TABLE.INDICATOR,'')) AS INDICATOR");
            strQry.AppendLine("");
            strQry.AppendLine("    FROM");
            strQry.AppendLine("   (");
            strQry.AppendLine("    --2Start Break UNION 2");
            strQry.AppendLine("    SELECT");
            strQry.AppendLine("    EBC.COMPANY_NO");
            strQry.AppendLine("   ,'#PAY_CATEGORY_TYPE#' AS PAY_CATEGORY_TYPE");
            strQry.AppendLine("   ,EBC.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,EBC.EMPLOYEE_NO");
            strQry.AppendLine("   ,EBC.BREAK_DATE AS DAY_DATE");
            strQry.AppendLine("   ,EBC.BREAK_SEQ");
            strQry.AppendLine("   ,BREAK_ACCUM_MINUTES = ");
            strQry.AppendLine("    CASE");
            strQry.AppendLine("    WHEN NOT TEMP.EMPLOYEE_NO IS NULL THEN");
            strQry.AppendLine("    CASE");
            strQry.AppendLine("    WHEN EBC.BREAK_TIME_OUT_MINUTES IS NULL OR EBC.BREAK_TIME_IN_MINUTES IS NULL");
            strQry.AppendLine("    THEN 0");
            strQry.AppendLine("    WHEN EBC.BREAK_TIME_OUT_MINUTES > EBC.BREAK_TIME_IN_MINUTES THEN");
            strQry.AppendLine("    EBC.BREAK_TIME_OUT_MINUTES - EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine("    ELSE 0");
            strQry.AppendLine("    END");
            strQry.AppendLine("    ELSE");
            strQry.AppendLine("    EBC.BREAK_TIME_OUT_MINUTES - EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine("    END");
            strQry.AppendLine("   ,INDICATOR = ");
            strQry.AppendLine("    CASE");
            strQry.AppendLine("    WHEN NOT TEMP.EMPLOYEE_NO IS NULL");
            strQry.AppendLine("    THEN 'X'");
            strQry.AppendLine("    ELSE ''");
            strQry.AppendLine("    END");
            strQry.AppendLine("");
            strQry.AppendLine("    FROM dbo.EMPLOYEE E");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN dbo.#BREAK_TABLE# EBC");
            strQry.AppendLine("    ON E.COMPANY_NO = EBC.COMPANY_NO");
            strQry.AppendLine("    AND E.EMPLOYEE_NO = EBC.EMPLOYEE_NO");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN");
            strQry.AppendLine("   (");
            strQry.AppendLine("    SELECT DISTINCT");
            strQry.AppendLine("    COMPANY_NO");
            strQry.AppendLine("   ,PAY_CATEGORY_NO");
            strQry.AppendLine("   ,EMPLOYEE_NO");
            strQry.AppendLine("   ,#DATE_FIELD#");
            strQry.AppendLine("    FROM DELETED");
            strQry.AppendLine("   ) AS D");
            strQry.AppendLine("    ON EBC.COMPANY_NO = D.COMPANY_NO");
            strQry.AppendLine("    AND EBC.EMPLOYEE_NO = D.EMPLOYEE_NO");
            strQry.AppendLine("    AND EBC.PAY_CATEGORY_NO = D.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND EBC.BREAK_DATE = D.#TIMESHEET_OR_BREAK#_DATE");
            strQry.AppendLine("");
            strQry.AppendLine("    LEFT JOIN");
            strQry.AppendLine("   (");
            strQry.AppendLine("    --1Start Break UNION 2");
            strQry.AppendLine("    SELECT");
            strQry.AppendLine("    EBC.EMPLOYEE_NO");
            strQry.AppendLine("   ,EBC.BREAK_DATE");
            strQry.AppendLine("   ,EBC.BREAK_SEQ");
            strQry.AppendLine("   ,EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine("   ,EBC.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine("");
            strQry.AppendLine("    FROM dbo.EMPLOYEE E");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN");
            strQry.AppendLine("   (");
            strQry.AppendLine("    SELECT");
            strQry.AppendLine("    ROW_NUMBER() OVER");
            strQry.AppendLine("   (ORDER BY");
            strQry.AppendLine("    EBC.COMPANY_NO");
            strQry.AppendLine("   ,EBC.EMPLOYEE_NO");
            strQry.AppendLine("   ,EBC.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,EBC.BREAK_DATE");
            strQry.AppendLine("   ,EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine("   ,EBC.BREAK_TIME_OUT_MINUTES) AS SORTED_REC");
            strQry.AppendLine("   ,EBC.COMPANY_NO");
            strQry.AppendLine("   ,EBC.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,EBC.EMPLOYEE_NO");
            strQry.AppendLine("   ,EBC.BREAK_DATE");
            strQry.AppendLine("   ,EBC.BREAK_SEQ");
            strQry.AppendLine("   ,EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine("   ,EBC.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine("");
            strQry.AppendLine("    FROM dbo.#BREAK_TABLE# EBC");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN");
            strQry.AppendLine("   (");
            strQry.AppendLine("    SELECT DISTINCT");
            strQry.AppendLine("    COMPANY_NO");
            strQry.AppendLine("   ,PAY_CATEGORY_NO");
            strQry.AppendLine("   ,EMPLOYEE_NO");
            strQry.AppendLine("   ,#DATE_FIELD#");
            strQry.AppendLine("    FROM DELETED");
            strQry.AppendLine("   ) AS D");
            strQry.AppendLine("    ON EBC.COMPANY_NO = D.COMPANY_NO");
            strQry.AppendLine("    AND EBC.EMPLOYEE_NO = D.EMPLOYEE_NO");
            strQry.AppendLine("    AND EBC.PAY_CATEGORY_NO = D.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND EBC.BREAK_DATE = D.#TIMESHEET_OR_BREAK#_DATE");
            strQry.AppendLine("   ) AS EBC");
            strQry.AppendLine("    ON E.COMPANY_NO = EBC.COMPANY_NO");
            strQry.AppendLine("    AND E.EMPLOYEE_NO = EBC.EMPLOYEE_NO");
            strQry.AppendLine("    --1End Break UNION 2");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN");
            strQry.AppendLine("   (");
            strQry.AppendLine("    SELECT");
            strQry.AppendLine("    ROW_NUMBER() OVER");
            strQry.AppendLine("   (ORDER BY");
            strQry.AppendLine("    EBC.COMPANY_NO");
            strQry.AppendLine("   ,EBC.EMPLOYEE_NO");
            strQry.AppendLine("   ,EBC.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,EBC.BREAK_DATE");
            strQry.AppendLine("   ,EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine("   ,EBC.BREAK_TIME_OUT_MINUTES) AS SORTED_REC");
            strQry.AppendLine("   ,EBC.COMPANY_NO");
            strQry.AppendLine("   ,EBC.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,EBC.EMPLOYEE_NO");
            strQry.AppendLine("   ,EBC.BREAK_DATE");
            strQry.AppendLine("   ,EBC.BREAK_SEQ");
            strQry.AppendLine("   ,EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine("   ,EBC.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine("");
            strQry.AppendLine("    FROM dbo.#BREAK_TABLE# EBC");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN");
            strQry.AppendLine("   (");
            strQry.AppendLine("    SELECT DISTINCT");
            strQry.AppendLine("    COMPANY_NO");
            strQry.AppendLine("   ,PAY_CATEGORY_NO");
            strQry.AppendLine("   ,EMPLOYEE_NO");
            strQry.AppendLine("   ,#DATE_FIELD#");
            strQry.AppendLine("    FROM DELETED");
            strQry.AppendLine("   ) AS D");
            strQry.AppendLine("    ON EBC.COMPANY_NO = D.COMPANY_NO");
            strQry.AppendLine("    AND EBC.EMPLOYEE_NO = D.EMPLOYEE_NO");
            strQry.AppendLine("    AND EBC.PAY_CATEGORY_NO = D.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND EBC.BREAK_DATE = D.#TIMESHEET_OR_BREAK#_DATE");
            strQry.AppendLine("   ) AS EBC2");
            strQry.AppendLine("    ON EBC.COMPANY_NO = EBC2.COMPANY_NO");
            strQry.AppendLine("    AND EBC.PAY_CATEGORY_NO = EBC2.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND EBC.EMPLOYEE_NO = EBC2.EMPLOYEE_NO");
            strQry.AppendLine("    AND EBC.BREAK_DATE = EBC2.BREAK_DATE");
            strQry.AppendLine("");
            strQry.AppendLine("    WHERE E.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("    AND ((EBC.BREAK_TIME_IN_MINUTES IS NULL");
            strQry.AppendLine("    OR EBC.BREAK_TIME_OUT_MINUTES IS NULL)");
            //Errol 2016-06-22
            strQry.AppendLine("    OR (EBC.BREAK_TIME_IN_MINUTES >= EBC2.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine("    AND EBC.SORTED_REC <= EBC2.SORTED_REC)");
            strQry.AppendLine("    OR (EBC.BREAK_TIME_IN_MINUTES < EBC2.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine("    AND EBC.SORTED_REC > EBC2.SORTED_REC))");
            strQry.AppendLine("   ) AS TEMP");
            strQry.AppendLine("    ON EBC.EMPLOYEE_NO = TEMP.EMPLOYEE_NO");
            strQry.AppendLine("    AND EBC.BREAK_DATE = TEMP.BREAK_DATE");
            strQry.AppendLine("    AND EBC.BREAK_SEQ = TEMP.BREAK_SEQ");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine("    ON EBC.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine("    AND EBC.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine("    AND EBC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND EPC.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN dbo.PAY_CATEGORY PC");
            strQry.AppendLine("    ON EBC.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine("    AND EBC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND PC.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("");
            strQry.AppendLine("    WHERE E.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("");
            strQry.AppendLine("    GROUP BY");
            strQry.AppendLine("    EBC.COMPANY_NO");
            strQry.AppendLine("   ,EBC.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,EBC.EMPLOYEE_NO");
            strQry.AppendLine("   ,EBC.BREAK_DATE");
            strQry.AppendLine("   ,EBC.BREAK_SEQ");
            strQry.AppendLine("	  ,TEMP.EMPLOYEE_NO");
            strQry.AppendLine("   ,EBC.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine("   ,EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine("    --2End Break UNION 2");
            strQry.AppendLine("   ) AS BREAK_TABLE");
            strQry.AppendLine("");
            strQry.AppendLine("    GROUP BY");
            strQry.AppendLine("    BREAK_TABLE.COMPANY_NO");
            strQry.AppendLine("   ,BREAK_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine("   ,BREAK_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,BREAK_TABLE.EMPLOYEE_NO");
            strQry.AppendLine("   ,BREAK_TABLE.DAY_DATE");
            strQry.AppendLine("    --3End Break UNION 2");
            strQry.AppendLine("   ) AS BREAK_SUMMARY_TABLE");
            strQry.AppendLine("    ON TIMESHEET_TOTAL_TABLE.COMPANY_NO = BREAK_SUMMARY_TABLE.COMPANY_NO");
            strQry.AppendLine("    AND TIMESHEET_TOTAL_TABLE.PAY_CATEGORY_TYPE = BREAK_SUMMARY_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine("    AND TIMESHEET_TOTAL_TABLE.PAY_CATEGORY_NO = BREAK_SUMMARY_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND TIMESHEET_TOTAL_TABLE.EMPLOYEE_NO = BREAK_SUMMARY_TABLE.EMPLOYEE_NO");
            strQry.AppendLine("    AND TIMESHEET_TOTAL_TABLE.DAY_DATE = BREAK_SUMMARY_TABLE.DAY_DATE");
            strQry.AppendLine("");
            strQry.AppendLine("    GROUP BY");
            strQry.AppendLine("    TIMESHEET_TOTAL_TABLE.COMPANY_NO");
            strQry.AppendLine("   ,TIMESHEET_TOTAL_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine("   ,TIMESHEET_TOTAL_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine("   ,TIMESHEET_TOTAL_TABLE.EMPLOYEE_NO");
            strQry.AppendLine("   ,TIMESHEET_TOTAL_TABLE.DAY_DATE");
            strQry.AppendLine("   ,TIMESHEET_TOTAL_TABLE.TIMESHEET_ACCUM_MINUTES");
            strQry.AppendLine("   ,TIMESHEET_TOTAL_TABLE.INDICATOR");
            strQry.AppendLine("   ,TIMESHEET_TOTAL_TABLE.BREAK_MINUTES");
            strQry.AppendLine("   ,BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES");
            strQry.AppendLine("   ,BREAK_SUMMARY_TABLE.INDICATOR");
            strQry.AppendLine("   ,BREAK_SUMMARY_TABLE.COMPANY_NO");
            strQry.AppendLine("    --1End Major UNION 2");
            strQry.AppendLine("   ) AS TEMP1_TABLE");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN dbo.PAY_CATEGORY PC1");
            strQry.AppendLine("    ON TEMP1_TABLE.COMPANY_NO = PC1.COMPANY_NO");
            strQry.AppendLine("    AND TEMP1_TABLE.PAY_CATEGORY_NO = PC1.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND TEMP1_TABLE.PAY_CATEGORY_TYPE = PC1.PAY_CATEGORY_TYPE");
            strQry.AppendLine("    AND PC1.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("    --2End Major UNION 2");
            strQry.AppendLine("   ) AS TEMP2_TABLE");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN dbo.PAY_CATEGORY PC2");
            strQry.AppendLine("    ON TEMP2_TABLE.COMPANY_NO = PC2.COMPANY_NO");
            strQry.AppendLine("    AND TEMP2_TABLE.PAY_CATEGORY_NO = PC2.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND TEMP2_TABLE.PAY_CATEGORY_TYPE = PC2.PAY_CATEGORY_TYPE");
            strQry.AppendLine("    AND PC2.PAY_CATEGORY_TYPE = '#PAY_CATEGORY_TYPE#'");
            strQry.AppendLine("");
            strQry.AppendLine("    INNER JOIN dbo.DATES D");
            strQry.AppendLine("    ON TEMP2_TABLE.DAY_DATE = D.DAY_DATE");
            strQry.AppendLine("    --3End Major UNION 2");
            strQry.AppendLine("   ) AS TEMP_TABLE");
            strQry.AppendLine("");
            strQry.AppendLine("    ON ETBDC.COMPANY_NO = TEMP_TABLE.COMPANY_NO");
            strQry.AppendLine("    AND ETBDC.EMPLOYEE_NO = TEMP_TABLE.EMPLOYEE_NO");
            strQry.AppendLine("    AND ETBDC.PAY_CATEGORY_NO = TEMP_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine("    AND ETBDC.TIMESHEET_DATE = TEMP_TABLE.DAY_DATE");
            strQry.AppendLine("");
            strQry.AppendLine("    END");
            strQry.AppendLine("");

        tgr_Client_Create_For_Timesheet_Break_Table_Continue:

            strQry = strQry.Replace("#TRIGGER_TABLE_NAME#", triggerTableName);
            strQry = strQry.Replace("#PAY_CATEGORY_TYPE#", strPayCategoryType);
            strQry = strQry.Replace("#DAY_TABLE#", strDayTable);
            strQry = strQry.Replace("#TIMESHEET_TABLE#", strTimeSheetTable);
            strQry = strQry.Replace("#BREAK_TABLE#", strBreakTable);
            strQry = strQry.Replace("#DATE_FIELD#", strDateField);
            strQry = strQry.Replace("#TIMESHEET_OR_BREAK#", strTimeSheetOrBreak);

            return strQry.ToString();
        }

        public string Client_tgr_Create_Timesheet()
        {
            string strQry = "";
            strQry += "CREATE TRIGGER tgr_Create_Timesheet ON DEVICE_CLOCK_TIME \r\n";
            strQry += "AFTER INSERT AS \r\n";
            strQry += "DECLARE @IN_OUT_IND AS VARCHAR(1) \r\n";
            strQry += "DECLARE @TIMESHEET_DATE AS DATE \r\n";
            strQry += "DECLARE @I_TIMESHEET_DATE AS DATE \r\n";
            strQry += "DECLARE @I_TIMESHEET_TIME_MINUTES AS INT \r\n";
            strQry += "DECLARE @COMPANY_NO AS INT \r\n";
            strQry += "DECLARE @EMPLOYEE_NO AS INT \r\n";
            strQry += "DECLARE @PAY_CATEGORY_NO AS INT \r\n";
            strQry += "DECLARE @PAY_CATEGORY_TYPE AS VARCHAR(1) \r\n";
            strQry += "DECLARE @TIMESHEET_SEQ AS INT \r\n";
            strQry += "DECLARE @TIMESHEET_TIME_IN_MINUTES AS INT \r\n";
            strQry += "DECLARE @TIMESHEET_TIME_OUT_MINUTES AS INT \r\n";
            strQry += "DECLARE @CLOCKED_BOUNDARY_TIME_MINUTES AS INT \r\n";
            strQry += "DECLARE @CURRENT_IDENTITY_SEED AS INT \r\n";
            strQry += "\r\n";
            strQry += "--Version 1.5 \r\n";
            strQry += "\r\n";
            strQry += "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;\r\n";
            strQry += "\r\n";
            strQry += "--Check Identity Seed \r\n";
            strQry += "SELECT @CURRENT_IDENTITY_SEED = IDENT_CURRENT(TABLE_NAME) \r\n";  
            strQry += "FROM INFORMATION_SCHEMA.TABLES \r\n";
            strQry += "WHERE OBJECTPROPERTY(OBJECT_ID(TABLE_NAME), 'TableHasIdentity') = 1 \r\n";
            strQry += "AND TABLE_TYPE = 'BASE TABLE' \r\n";
            strQry += "AND TABLE_NAME = 'DEVICE_CLOCK_TIME' \r\n";
            strQry += "\r\n";
            strQry += "PRINT '@CURRENT_IDENTITY_SEED = ' + convert(varchar,@CURRENT_IDENTITY_SEED) \r\n";
            strQry += "\r\n";
            strQry += "IF @CURRENT_IDENTITY_SEED >= 29999 \r\n";
            strQry += "\r\n";
            strQry += "    BEGIN \r\n";
            strQry += "\r\n";	
            strQry += "    PRINT 'DEVICE_CLOCK_TIME RESEED 0' \r\n";
            strQry += "\r\n";
            strQry += "    DBCC CHECKIDENT('DEVICE_CLOCK_TIME', RESEED, 0) \r\n";
            strQry += "\r\n";
            strQry += "    END \r\n";
            strQry += "\r\n";
            strQry += "SELECT \r\n";
            strQry += " @IN_OUT_IND = IN_OUT_IND\r\n";
            strQry += ",@PAY_CATEGORY_TYPE = PAY_CATEGORY_TYPE\r\n";
            strQry += ",@CLOCKED_BOUNDARY_TIME_MINUTES = CLOCKED_BOUNDARY_TIME_MINUTES \r\n";
            strQry += "\r\n";
            strQry += "FROM INSERTED \r\n";
            strQry += "\r\n";
            strQry += "IF @IN_OUT_IND = 'I'\r\n";
            strQry += "\r\n";
            strQry += "    BEGIN\r\n";
            strQry += "\r\n";
            strQry += "    IF @PAY_CATEGORY_TYPE = 'W'\r\n";
            strQry += "\r\n";
            strQry += "        BEGIN\r\n";
            strQry += "\r\n";
            strQry += "        INSERT INTO EMPLOYEE_TIMESHEET_CURRENT\r\n";
            strQry += "       (COMPANY_NO\r\n";
            strQry += "       ,EMPLOYEE_NO\r\n";
            strQry += "       ,PAY_CATEGORY_NO\r\n";
            strQry += "       ,TIMESHEET_DATE\r\n";
            strQry += "       ,TIMESHEET_SEQ\r\n";
            strQry += "       ,TIMESHEET_TIME_IN_MINUTES\r\n";
            strQry += "       ,CLOCKED_TIME_IN_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "        SELECT TOP 1\r\n";
            strQry += "        I.COMPANY_NO\r\n";
            strQry += "       ,I.EMPLOYEE_NO\r\n";
            strQry += "       ,I.PAY_CATEGORY_NO\r\n";
            strQry += "       ,I.TIMESHEET_DATE\r\n";
            strQry += "       ,ISNULL(ETC.TIMESHEET_SEQ,0) + 1\r\n";
            strQry += "       ,I.CLOCKED_BOUNDARY_TIME_MINUTES\r\n";
            strQry += "       ,I.TIMESHEET_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "        FROM INSERTED I\r\n";
            strQry += "\r\n";
            strQry += "        LEFT JOIN EMPLOYEE_TIMESHEET_CURRENT ETC\r\n";
            strQry += "        ON I.COMPANY_NO = ETC.COMPANY_NO\r\n";
            strQry += "        AND I.EMPLOYEE_NO = ETC.EMPLOYEE_NO\r\n";
            strQry += "        AND I.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO\r\n";
            strQry += "        AND I.TIMESHEET_DATE = ETC.TIMESHEET_DATE\r\n";
            strQry += "\r\n";
            strQry += "        ORDER  BY\r\n";
            strQry += "        I.COMPANY_NO\r\n";
            strQry += "       ,I.EMPLOYEE_NO\r\n";
            strQry += "       ,I.PAY_CATEGORY_NO\r\n";
            strQry += "       ,I.TIMESHEET_DATE\r\n";
            strQry += "       ,ETC.TIMESHEET_SEQ DESC\r\n";
            strQry += "\r\n";
            strQry += "        END\r\n";
            strQry += "\r\n";
            strQry += "    ELSE\r\n";
            strQry += "\r\n";
            strQry += "        BEGIN\r\n";
            strQry += "\r\n";
            strQry += "        IF @PAY_CATEGORY_TYPE = 'S'\r\n";
            strQry += "\r\n";
            strQry += "            BEGIN\r\n";
            strQry += "\r\n";
            strQry += "            INSERT INTO EMPLOYEE_SALARY_TIMESHEET_CURRENT\r\n";
            strQry += "           (COMPANY_NO\r\n";
            strQry += "           ,EMPLOYEE_NO\r\n";
            strQry += "           ,PAY_CATEGORY_NO\r\n";
            strQry += "           ,TIMESHEET_DATE\r\n";
            strQry += "           ,TIMESHEET_SEQ\r\n";
            strQry += "           ,TIMESHEET_TIME_IN_MINUTES\r\n";
            strQry += "           ,CLOCKED_TIME_IN_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "            SELECT TOP 1\r\n";
            strQry += "            I.COMPANY_NO\r\n";
            strQry += "           ,I.EMPLOYEE_NO\r\n";
            strQry += "           ,I.PAY_CATEGORY_NO\r\n";
            strQry += "           ,I.TIMESHEET_DATE\r\n";
            strQry += "           ,ISNULL(ETC.TIMESHEET_SEQ,0) + 1\r\n";
            strQry += "           ,I.CLOCKED_BOUNDARY_TIME_MINUTES\r\n";
            strQry += "           ,I.TIMESHEET_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "            FROM INSERTED I\r\n";
            strQry += "\r\n";
            strQry += "            LEFT JOIN EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC\r\n";
            strQry += "            ON I.COMPANY_NO = ETC.COMPANY_NO\r\n";
            strQry += "            AND I.EMPLOYEE_NO = ETC.EMPLOYEE_NO\r\n";
            strQry += "            AND I.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO\r\n";
            strQry += "            AND I.TIMESHEET_DATE = ETC.TIMESHEET_DATE\r\n";
            strQry += "\r\n";
            strQry += "            ORDER  BY\r\n";
            strQry += "            I.COMPANY_NO\r\n";
            strQry += "           ,I.EMPLOYEE_NO\r\n";
            strQry += "           ,I.PAY_CATEGORY_NO\r\n";
            strQry += "           ,I.TIMESHEET_DATE\r\n";
            strQry += "           ,ETC.TIMESHEET_SEQ DESC\r\n";
            strQry += "\r\n";
            strQry += "            END\r\n";
            strQry += "\r\n";
            strQry += "        ELSE\r\n";
            strQry += "\r\n";
            strQry += "            BEGIN\r\n";
            strQry += "\r\n";
            strQry += "            INSERT INTO EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT\r\n";
            strQry += "           (COMPANY_NO\r\n";
            strQry += "           ,EMPLOYEE_NO\r\n";
            strQry += "           ,PAY_CATEGORY_NO\r\n";
            strQry += "           ,TIMESHEET_DATE\r\n";
            strQry += "           ,TIMESHEET_SEQ\r\n";
            strQry += "           ,TIMESHEET_TIME_IN_MINUTES\r\n";
            strQry += "           ,CLOCKED_TIME_IN_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "            SELECT TOP 1\r\n";
            strQry += "            I.COMPANY_NO\r\n";
            strQry += "           ,I.EMPLOYEE_NO\r\n";
            strQry += "           ,I.PAY_CATEGORY_NO\r\n";
            strQry += "           ,I.TIMESHEET_DATE\r\n";
            strQry += "           ,ISNULL(ETC.TIMESHEET_SEQ,0) + 1\r\n";
            strQry += "           ,I.CLOCKED_BOUNDARY_TIME_MINUTES\r\n";
            strQry += "           ,I.TIMESHEET_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "            FROM INSERTED I\r\n";
            strQry += "\r\n";
            strQry += "            LEFT JOIN EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC\r\n";
            strQry += "            ON I.COMPANY_NO = ETC.COMPANY_NO\r\n";
            strQry += "            AND I.EMPLOYEE_NO = ETC.EMPLOYEE_NO\r\n";
            strQry += "            AND I.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO\r\n";
            strQry += "            AND I.TIMESHEET_DATE = ETC.TIMESHEET_DATE\r\n";
            strQry += "\r\n";
            strQry += "            ORDER  BY\r\n";
            strQry += "            I.COMPANY_NO\r\n";
            strQry += "           ,I.EMPLOYEE_NO\r\n";
            strQry += "           ,I.PAY_CATEGORY_NO\r\n";
            strQry += "           ,I.TIMESHEET_DATE\r\n";
            strQry += "           ,ETC.TIMESHEET_SEQ DESC\r\n";
            strQry += "\r\n";
            strQry += "            END\r\n";
            strQry += "\r\n";
            strQry += "        END\r\n";
            strQry += "\r\n";
            strQry += "    END\r\n";
            strQry += "\r\n";
            strQry += "ELSE\r\n";
            strQry += "\r\n";
            strQry += "    --OUT\r\n";
            strQry += "\r\n";
            strQry += "    BEGIN\r\n";
            strQry += "\r\n";
            strQry += "    IF @PAY_CATEGORY_TYPE = 'W'\r\n";
            strQry += "\r\n";
            strQry += "        BEGIN\r\n";
            strQry += "\r\n";
            strQry += "        SELECT TOP 1\r\n";
            strQry += "        @COMPANY_NO = I.COMPANY_NO\r\n";
            strQry += "       ,@EMPLOYEE_NO = I.EMPLOYEE_NO\r\n";
            strQry += "       ,@PAY_CATEGORY_NO = I.PAY_CATEGORY_NO\r\n";
            strQry += "       ,@I_TIMESHEET_DATE = I.TIMESHEET_DATE\r\n";
            strQry += "       ,@TIMESHEET_DATE = ETC.TIMESHEET_DATE\r\n";
            strQry += "       ,@I_TIMESHEET_TIME_MINUTES = I.TIMESHEET_TIME_MINUTES\r\n";
            strQry += "       ,@TIMESHEET_TIME_IN_MINUTES = ETC.TIMESHEET_TIME_IN_MINUTES\r\n";
            strQry += "       ,@TIMESHEET_TIME_OUT_MINUTES = ETC.TIMESHEET_TIME_OUT_MINUTES\r\n";
            strQry += "       ,@TIMESHEET_SEQ = ISNULL(ETC.TIMESHEET_SEQ,0)\r\n";
            strQry += "\r\n";
            strQry += "        FROM INSERTED I\r\n";
            strQry += "\r\n";
            strQry += "        LEFT JOIN EMPLOYEE_TIMESHEET_CURRENT ETC\r\n";
            strQry += "        ON I.COMPANY_NO = ETC.COMPANY_NO\r\n";
            strQry += "        AND I.EMPLOYEE_NO = ETC.EMPLOYEE_NO\r\n";
            strQry += "        AND I.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO\r\n";
            strQry += "        --Go Back 1 Day\r\n";
            strQry += "        AND ETC.TIMESHEET_DATE >= DATEADD(DAY,-1,I.TIMESHEET_DATE)\r\n";
            strQry += "\r\n";
            strQry += "        ORDER BY\r\n";
            strQry += "        ETC.TIMESHEET_DATE DESC\r\n";
            strQry += "       ,ETC.TIMESHEET_SEQ DESC\r\n";
            strQry += "\r\n";
            strQry += "        PRINT '@I_TIMESHEET_DATE = ' + convert(varchar,@I_TIMESHEET_DATE)\r\n";
            strQry += "        PRINT '@I_TIMESHEET_TIME_MINUTES = ' + convert(varchar,@I_TIMESHEET_TIME_MINUTES)\r\n";
            strQry += "        PRINT '@TIMESHEET_DATE = ' + convert(varchar,@TIMESHEET_DATE)\r\n";
            strQry += "        PRINT '@TIMESHEET_TIME_IN_MINUTES = ' + convert(varchar,@TIMESHEET_TIME_IN_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "        -- Complete TimeSheet OR No Timesheets at All\r\n";
            strQry += "        IF @TIMESHEET_TIME_OUT_MINUTES IS NOT NULL\r\n";
            strQry += "        OR @TIMESHEET_DATE IS NULL\r\n";
            strQry += "\r\n";
            strQry += "            BEGIN\r\n";
            strQry += "\r\n";
            strQry += "            PRINT 'Complete TimeSheet OR No Timesheets at All'\r\n";
            strQry += "\r\n";
            strQry += "            INSERT INTO EMPLOYEE_TIMESHEET_CURRENT\r\n";
            strQry += "           (COMPANY_NO\r\n";
            strQry += "           ,EMPLOYEE_NO\r\n";
            strQry += "           ,PAY_CATEGORY_NO\r\n";
            strQry += "           ,TIMESHEET_DATE\r\n";
            strQry += "           ,TIMESHEET_SEQ\r\n";
            strQry += "           ,TIMESHEET_TIME_OUT_MINUTES\r\n";
            strQry += "           ,CLOCKED_TIME_OUT_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "            SELECT\r\n";
            strQry += "            I.COMPANY_NO\r\n";
            strQry += "           ,I.EMPLOYEE_NO\r\n";
            strQry += "           ,I.PAY_CATEGORY_NO\r\n";
            strQry += "           ,I.TIMESHEET_DATE\r\n";
            strQry += "           ,ISNULL(MAX(ETC.TIMESHEET_SEQ),0) + 1\r\n";
            strQry += "           ,I.CLOCKED_BOUNDARY_TIME_MINUTES\r\n";
            strQry += "           ,@I_TIMESHEET_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "            FROM INSERTED I\r\n";
            strQry += "\r\n";
            strQry += "            LEFT JOIN EMPLOYEE_TIMESHEET_CURRENT ETC\r\n";
            strQry += "            ON I.COMPANY_NO = ETC.COMPANY_NO\r\n";
            strQry += "            AND I.EMPLOYEE_NO = ETC.EMPLOYEE_NO\r\n";
            strQry += "            AND I.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO\r\n";
            strQry += "            AND I.TIMESHEET_DATE = ETC.TIMESHEET_DATE\r\n";
            strQry += "\r\n";
            strQry += "            GROUP BY\r\n";
            strQry += "            I.COMPANY_NO\r\n";
            strQry += "           ,I.EMPLOYEE_NO\r\n";
            strQry += "           ,I.PAY_CATEGORY_NO\r\n";
            strQry += "           ,I.TIMESHEET_DATE\r\n";
            strQry += "           ,I.CLOCKED_BOUNDARY_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "            END\r\n";
            strQry += "\r\n";
            strQry += "        ELSE\r\n";
            strQry += "\r\n";
            strQry += "            BEGIN\r\n";
            strQry += "\r\n";
            strQry += "            -- IF today\r\n";
            strQry += "            IF CONVERT(CHAR(8),@TIMESHEET_DATE,112) = CONVERT(CHAR(8),@I_TIMESHEET_DATE,112)\r\n";
            strQry += "\r\n";
            strQry += "                BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                PRINT 'Update - Today'\r\n";
            strQry += "\r\n";
            strQry += "                UPDATE EMPLOYEE_TIMESHEET_CURRENT\r\n";
            strQry += "                SET TIMESHEET_TIME_OUT_MINUTES = @CLOCKED_BOUNDARY_TIME_MINUTES\r\n";
            strQry += "               ,CLOCKED_TIME_OUT_MINUTES = @I_TIMESHEET_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                WHERE COMPANY_NO = @COMPANY_NO\r\n";
            strQry += "                AND EMPLOYEE_NO = @EMPLOYEE_NO\r\n";
            strQry += "                AND PAY_CATEGORY_NO = @PAY_CATEGORY_NO\r\n";
            strQry += "                AND TIMESHEET_DATE = @I_TIMESHEET_DATE\r\n";
            strQry += "                AND TIMESHEET_SEQ = @TIMESHEET_SEQ\r\n";
            strQry += "\r\n";
            strQry += "                END\r\n";
            strQry += "\r\n";
            strQry += "            ELSE\r\n";
            strQry += "\r\n";
            strQry += "                BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                -- Within 24 Hours\r\n";
            strQry += "                IF @I_TIMESHEET_TIME_MINUTES < @TIMESHEET_TIME_IN_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                    BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                    PRINT 'Within 24 Hours'\r\n";
            strQry += "\r\n";
            strQry += "                    --Clock Out Yesterday (Midnight)\r\n";
            strQry += "                    UPDATE EMPLOYEE_TIMESHEET_CURRENT\r\n";
            strQry += "                    SET TIMESHEET_TIME_OUT_MINUTES = 1440\r\n";
            strQry += "                   ,CLOCKED_TIME_OUT_MINUTES = 1440\r\n";
            strQry += "\r\n";
            strQry += "                    WHERE COMPANY_NO = @COMPANY_NO\r\n";
            strQry += "                    AND EMPLOYEE_NO = @EMPLOYEE_NO\r\n";
            strQry += "                    AND PAY_CATEGORY_NO = @PAY_CATEGORY_NO\r\n";
            strQry += "                    AND TIMESHEET_DATE = @TIMESHEET_DATE\r\n";
            strQry += "                    AND TIMESHEET_SEQ = @TIMESHEET_SEQ\r\n";
            strQry += "\r\n";
            strQry += "                    --Clock In 0 Hours\r\n";
            strQry += "                    INSERT INTO EMPLOYEE_TIMESHEET_CURRENT\r\n";
            strQry += "                   (COMPANY_NO\r\n";
            strQry += "                   ,EMPLOYEE_NO\r\n";
            strQry += "                   ,PAY_CATEGORY_NO\r\n";
            strQry += "                   ,TIMESHEET_DATE\r\n";
            strQry += "                   ,TIMESHEET_SEQ\r\n";
            strQry += "                   ,TIMESHEET_TIME_IN_MINUTES\r\n";
            strQry += "                   ,CLOCKED_TIME_IN_MINUTES\r\n";
            strQry += "                   ,TIMESHEET_TIME_OUT_MINUTES\r\n";
            strQry += "                   ,CLOCKED_TIME_OUT_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "                    SELECT\r\n";
            strQry += "                    @COMPANY_NO\r\n";
            strQry += "                   ,@EMPLOYEE_NO\r\n";
            strQry += "                   ,@PAY_CATEGORY_NO\r\n";
            strQry += "                   ,@I_TIMESHEET_DATE\r\n";
            strQry += "                   ,ISNULL(MAX(ETC.TIMESHEET_SEQ),0) + 1\r\n";
            strQry += "                   ,0\r\n";
            strQry += "                   ,0\r\n";
            strQry += "                   ,@CLOCKED_BOUNDARY_TIME_MINUTES\r\n";
            strQry += "                   ,@I_TIMESHEET_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                    FROM EMPLOYEE_TIMESHEET_CURRENT ETC\r\n";
            strQry += "                    WHERE ETC.COMPANY_NO = @COMPANY_NO\r\n";
            strQry += "                    AND ETC.EMPLOYEE_NO = @EMPLOYEE_NO\r\n";
            strQry += "                    AND ETC.PAY_CATEGORY_NO = @PAY_CATEGORY_NO\r\n";
            strQry += "                    AND ETC.TIMESHEET_DATE = @I_TIMESHEET_DATE\r\n";
            strQry += "\r\n";
            strQry += "                    END\r\n";
            strQry += "\r\n";
            strQry += "                ELSE\r\n";
            strQry += "\r\n";
            strQry += "                    BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                    PRINT 'NOT Within 24 Hours'\r\n";
            strQry += "\r\n";
            strQry += "                    INSERT INTO EMPLOYEE_TIMESHEET_CURRENT\r\n";
            strQry += "                   (COMPANY_NO\r\n";
            strQry += "                   ,EMPLOYEE_NO\r\n";
            strQry += "                   ,PAY_CATEGORY_NO\r\n";
            strQry += "                   ,TIMESHEET_DATE\r\n";
            strQry += "                   ,TIMESHEET_SEQ\r\n";
            strQry += "                   ,TIMESHEET_TIME_OUT_MINUTES\r\n";
            strQry += "                   ,CLOCKED_TIME_OUT_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "                    SELECT\r\n";
            strQry += "                    @COMPANY_NO\r\n";
            strQry += "                   ,@EMPLOYEE_NO\r\n";
            strQry += "                   ,@PAY_CATEGORY_NO\r\n";
            strQry += "                   ,@I_TIMESHEET_DATE\r\n";
            strQry += "                   ,ISNULL(MAX(ETC.TIMESHEET_SEQ),0) + 1\r\n";
            strQry += "                   ,@CLOCKED_BOUNDARY_TIME_MINUTES\r\n";
            strQry += "                   ,@I_TIMESHEET_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                    FROM EMPLOYEE_TIMESHEET_CURRENT ETC\r\n";
            strQry += "\r\n";
            strQry += "                    WHERE ETC.COMPANY_NO = @COMPANY_NO\r\n";
            strQry += "                    AND ETC.EMPLOYEE_NO = @EMPLOYEE_NO\r\n";
            strQry += "                    AND ETC.PAY_CATEGORY_NO = @PAY_CATEGORY_NO\r\n";
            strQry += "                    AND ETC.TIMESHEET_DATE = @I_TIMESHEET_DATE\r\n";
            strQry += "\r\n";
            strQry += "                    END\r\n";
            strQry += "\r\n";
            strQry += "                END\r\n";
            strQry += "\r\n";
            strQry += "            END\r\n";
            strQry += "\r\n";
            strQry += "        END\r\n";
            strQry += "\r\n";
            strQry += "    ELSE\r\n";
            strQry += "\r\n";
            strQry += "        BEGIN\r\n";
            strQry += "\r\n";
            strQry += "        IF @PAY_CATEGORY_TYPE = 'S'\r\n";
            strQry += "\r\n";
            strQry += "            BEGIN\r\n";
            strQry += "\r\n";
            strQry += "            --SALARIES\r\n";
            strQry += "            SELECT\r\n";
            strQry += "            @COMPANY_NO = I.COMPANY_NO\r\n";
            strQry += "           ,@EMPLOYEE_NO = I.EMPLOYEE_NO\r\n";
            strQry += "           ,@PAY_CATEGORY_NO = I.PAY_CATEGORY_NO\r\n";
            strQry += "           ,@I_TIMESHEET_DATE = I.TIMESHEET_DATE\r\n";
            strQry += "           ,@TIMESHEET_DATE = ETC.TIMESHEET_DATE\r\n";
            strQry += "           ,@I_TIMESHEET_TIME_MINUTES = I.TIMESHEET_TIME_MINUTES\r\n";
            strQry += "           ,@TIMESHEET_TIME_IN_MINUTES = ETC.TIMESHEET_TIME_IN_MINUTES\r\n";
            strQry += "           ,@TIMESHEET_TIME_OUT_MINUTES = ETC.TIMESHEET_TIME_OUT_MINUTES\r\n";
            strQry += "           ,@TIMESHEET_SEQ = ISNULL(ETC.TIMESHEET_SEQ,0)\r\n";
            strQry += "\r\n";
            strQry += "            FROM INSERTED I\r\n";
            strQry += "\r\n";
            strQry += "            LEFT JOIN EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC\r\n";
            strQry += "            ON I.COMPANY_NO = ETC.COMPANY_NO\r\n";
            strQry += "            AND I.EMPLOYEE_NO = ETC.EMPLOYEE_NO\r\n";
            strQry += "            AND I.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO\r\n";
            strQry += "            AND CONVERT(CHAR(8),ETC.TIMESHEET_DATE,112) + CONVERT(CHAR(4),ISNULL(ETC.TIMESHEET_SEQ,0)) =\r\n";
            strQry += "            --GET Only 1 Row\r\n";
            strQry += "\r\n";
            strQry += "           (SELECT MAX(CONVERT(CHAR(8),ETC.TIMESHEET_DATE,112) + CONVERT(CHAR(4),ISNULL(ETC.TIMESHEET_SEQ,0)))\r\n";
            strQry += "\r\n";
            strQry += "            FROM INSERTED I\r\n";
            strQry += "\r\n";
            strQry += "            INNER JOIN EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC\r\n";
            strQry += "            ON I.COMPANY_NO = ETC.COMPANY_NO\r\n";
            strQry += "            AND I.EMPLOYEE_NO = ETC.EMPLOYEE_NO\r\n";
            strQry += "            AND I.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO\r\n";
            strQry += "            --Go Back 1 Day\r\n";
            strQry += "            AND ETC.TIMESHEET_DATE >= DATEADD(DAY,-1,I.TIMESHEET_DATE))\r\n";
            strQry += "\r\n";
            strQry += "            PRINT '@I_TIMESHEET_DATE = ' + convert(varchar,@I_TIMESHEET_DATE)\r\n";
            strQry += "            PRINT '@I_TIMESHEET_TIME_MINUTES = ' + convert(varchar,@I_TIMESHEET_TIME_MINUTES)\r\n";
            strQry += "            PRINT '@TIMESHEET_DATE = ' + convert(varchar,@TIMESHEET_DATE)\r\n";
            strQry += "            PRINT '@TIMESHEET_TIME_IN_MINUTES = ' + convert(varchar,@TIMESHEET_TIME_IN_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "            -- Complete TimeSheet OR No Timesheets at All\r\n";
            strQry += "            IF @TIMESHEET_TIME_OUT_MINUTES IS NOT NULL\r\n";
            strQry += "            OR @TIMESHEET_DATE IS NULL\r\n";
            strQry += "\r\n";
            strQry += "                BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                PRINT 'Complete TimeSheet OR No Timesheets at All'\r\n";
            strQry += "                PRINT '@TIMESHEET_SEQ = ' + convert(varchar,@TIMESHEET_SEQ)\r\n";
            strQry += "\r\n";
            strQry += "                INSERT INTO EMPLOYEE_SALARY_TIMESHEET_CURRENT\r\n";
            strQry += "               (COMPANY_NO\r\n";
            strQry += "               ,EMPLOYEE_NO\r\n";
            strQry += "               ,PAY_CATEGORY_NO\r\n";
            strQry += "               ,TIMESHEET_DATE\r\n";
            strQry += "               ,TIMESHEET_SEQ\r\n";
            strQry += "               ,TIMESHEET_TIME_OUT_MINUTES\r\n";
            strQry += "               ,CLOCKED_TIME_OUT_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "                SELECT\r\n";
            strQry += "                I.COMPANY_NO\r\n";
            strQry += "               ,I.EMPLOYEE_NO\r\n";
            strQry += "               ,I.PAY_CATEGORY_NO\r\n";
            strQry += "               ,I.TIMESHEET_DATE\r\n";
            strQry += "               ,ISNULL(MAX(ETC.TIMESHEET_SEQ),0) + 1\r\n";
            strQry += "               ,I.TIMESHEET_TIME_MINUTES\r\n";
            strQry += "               ,I.TIMESHEET_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                FROM INSERTED I\r\n";
            strQry += "\r\n";
            strQry += "                LEFT JOIN EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC\r\n";
            strQry += "                ON I.COMPANY_NO = ETC.COMPANY_NO\r\n";
            strQry += "                AND I.EMPLOYEE_NO = ETC.EMPLOYEE_NO\r\n";
            strQry += "                AND I.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO\r\n";
            strQry += "                AND I.TIMESHEET_DATE = ETC.TIMESHEET_DATE\r\n";
            strQry += "\r\n";
            strQry += "                GROUP BY\r\n";
            strQry += "                I.COMPANY_NO\r\n";
            strQry += "               ,I.EMPLOYEE_NO\r\n";
            strQry += "               ,I.PAY_CATEGORY_NO\r\n";
            strQry += "               ,I.TIMESHEET_DATE\r\n";
            strQry += "               ,I.TIMESHEET_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                END\r\n";
            strQry += "\r\n";
            strQry += "            ELSE\r\n";
            strQry += "\r\n";
            strQry += "                BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                -- IF today\r\n";
            strQry += "                IF CONVERT(CHAR(8),@TIMESHEET_DATE,112) = CONVERT(CHAR(8),@I_TIMESHEET_DATE,112)\r\n";
            strQry += "\r\n";
            strQry += "                    BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                    PRINT 'Update - Today'\r\n";
            strQry += "\r\n";
            strQry += "                    UPDATE EMPLOYEE_SALARY_TIMESHEET_CURRENT\r\n";
            strQry += "                    SET TIMESHEET_TIME_OUT_MINUTES = @I_TIMESHEET_TIME_MINUTES\r\n";
            strQry += "                   ,CLOCKED_TIME_OUT_MINUTES = @I_TIMESHEET_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                    WHERE COMPANY_NO = @COMPANY_NO\r\n";
            strQry += "                    AND EMPLOYEE_NO = @EMPLOYEE_NO\r\n";
            strQry += "                    AND PAY_CATEGORY_NO = @PAY_CATEGORY_NO\r\n";
            strQry += "                    AND TIMESHEET_DATE = @I_TIMESHEET_DATE\r\n";
            strQry += "                    AND TIMESHEET_SEQ = @TIMESHEET_SEQ\r\n";
            strQry += "\r\n";
            strQry += "                    END\r\n";
            strQry += "\r\n";
            strQry += "                ELSE\r\n";
            strQry += "\r\n";
            strQry += "                    BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                    -- Within 24 Hours\r\n";
            strQry += "                    IF @I_TIMESHEET_TIME_MINUTES < @TIMESHEET_TIME_IN_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                        BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                        PRINT 'Within 24 Hours'\r\n";
            strQry += "\r\n";
            strQry += "                        --Clock Out Yesterday (Midnight)\r\n";
            strQry += "                        UPDATE EMPLOYEE_SALARY_TIMESHEET_CURRENT\r\n";
            strQry += "                        SET TIMESHEET_TIME_OUT_MINUTES = 1440\r\n";
            strQry += "\r\n";
            strQry += "                        WHERE COMPANY_NO = @COMPANY_NO\r\n";
            strQry += "                        AND EMPLOYEE_NO = @EMPLOYEE_NO\r\n";
            strQry += "                        AND PAY_CATEGORY_NO = @PAY_CATEGORY_NO\r\n";
            strQry += "                        AND TIMESHEET_DATE = @TIMESHEET_DATE\r\n";
            strQry += "                        AND TIMESHEET_SEQ = @TIMESHEET_SEQ\r\n";
            strQry += "\r\n";
            strQry += "                        --Clock In 0 Hours\r\n";
            strQry += "                        INSERT INTO EMPLOYEE_SALARY_TIMESHEET_CURRENT\r\n";
            strQry += "                       (COMPANY_NO\r\n";
            strQry += "                       ,EMPLOYEE_NO\r\n";
            strQry += "                       ,PAY_CATEGORY_NO\r\n";
            strQry += "                       ,TIMESHEET_DATE\r\n";
            strQry += "                       ,TIMESHEET_SEQ\r\n";
            strQry += "                       ,TIMESHEET_TIME_IN_MINUTES\r\n";
            strQry += "                       ,TIMESHEET_TIME_OUT_MINUTES\r\n";
            strQry += "                       ,CLOCKED_TIME_OUT_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "                        SELECT\r\n";
            strQry += "                        @COMPANY_NO\r\n";
            strQry += "                       ,@EMPLOYEE_NO\r\n";
            strQry += "                       ,@PAY_CATEGORY_NO\r\n";
            strQry += "                       ,@I_TIMESHEET_DATE\r\n";
            strQry += "                       ,ISNULL(MAX(ETC.TIMESHEET_SEQ),0) + 1\r\n";
            strQry += "                       ,0\r\n";
            strQry += "                       ,@I_TIMESHEET_TIME_MINUTES\r\n";
            strQry += "                       ,@I_TIMESHEET_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                        FROM EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC\r\n";
            strQry += "                        WHERE ETC.COMPANY_NO = @COMPANY_NO\r\n";
            strQry += "                        AND ETC.EMPLOYEE_NO = @EMPLOYEE_NO\r\n";
            strQry += "                        AND ETC.PAY_CATEGORY_NO = @PAY_CATEGORY_NO\r\n";
            strQry += "                        AND ETC.TIMESHEET_DATE = @I_TIMESHEET_DATE\r\n";
            strQry += "\r\n";
            strQry += "                        END\r\n";
            strQry += "\r\n";
            strQry += "                    ELSE\r\n";
            strQry += "\r\n";
            strQry += "                        BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                        PRINT 'NOT Within 24 Hours'\r\n";
            strQry += "\r\n";
            strQry += "                        INSERT INTO EMPLOYEE_SALARY_TIMESHEET_CURRENT\r\n";
            strQry += "                       (COMPANY_NO\r\n";
            strQry += "                       ,EMPLOYEE_NO\r\n";
            strQry += "                       ,PAY_CATEGORY_NO\r\n";
            strQry += "                       ,TIMESHEET_DATE\r\n";
            strQry += "                       ,TIMESHEET_SEQ\r\n";
            strQry += "                       ,TIMESHEET_TIME_OUT_MINUTES\r\n";
            strQry += "                       ,CLOCKED_TIME_OUT_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "                        SELECT\r\n";
            strQry += "                        @COMPANY_NO\r\n";
            strQry += "                       ,@EMPLOYEE_NO\r\n";
            strQry += "                       ,@PAY_CATEGORY_NO\r\n";
            strQry += "                       ,@I_TIMESHEET_DATE\r\n";
            strQry += "                       ,ISNULL(MAX(ETC.TIMESHEET_SEQ),0) + 1\r\n";
            strQry += "                       ,@I_TIMESHEET_TIME_MINUTES\r\n";
            strQry += "                       ,@I_TIMESHEET_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                        FROM EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC\r\n";
            strQry += "\r\n";
            strQry += "                        WHERE ETC.COMPANY_NO = @COMPANY_NO\r\n";
            strQry += "                        AND ETC.EMPLOYEE_NO = @EMPLOYEE_NO\r\n";
            strQry += "                        AND ETC.PAY_CATEGORY_NO = @PAY_CATEGORY_NO\r\n";
            strQry += "                        AND ETC.TIMESHEET_DATE = @I_TIMESHEET_DATE\r\n";
            strQry += "\r\n";
            strQry += "                        END\r\n";
            strQry += "\r\n";
            strQry += "                    END\r\n";
            strQry += "\r\n";
            strQry += "                END\r\n";
            strQry += "\r\n";
            strQry += "            END\r\n";
            strQry += "\r\n";
            strQry += "        ELSE\r\n";
            strQry += "\r\n";
            strQry += "            BEGIN\r\n";
            strQry += "\r\n";
            strQry += "            --TIME ATTENDANCE\r\n";
            strQry += "            SELECT\r\n";
            strQry += "            @COMPANY_NO = I.COMPANY_NO\r\n";
            strQry += "           ,@EMPLOYEE_NO = I.EMPLOYEE_NO\r\n";
            strQry += "           ,@PAY_CATEGORY_NO = I.PAY_CATEGORY_NO\r\n";
            strQry += "           ,@I_TIMESHEET_DATE = I.TIMESHEET_DATE\r\n";
            strQry += "           ,@TIMESHEET_DATE = ETC.TIMESHEET_DATE\r\n";
            strQry += "           ,@I_TIMESHEET_TIME_MINUTES = I.TIMESHEET_TIME_MINUTES\r\n";
            strQry += "           ,@TIMESHEET_TIME_IN_MINUTES = ETC.TIMESHEET_TIME_IN_MINUTES\r\n";
            strQry += "           ,@TIMESHEET_TIME_OUT_MINUTES = ETC.TIMESHEET_TIME_OUT_MINUTES\r\n";
            strQry += "           ,@TIMESHEET_SEQ = ISNULL(ETC.TIMESHEET_SEQ,0)\r\n";
            strQry += "\r\n";
            strQry += "            FROM INSERTED I\r\n";
            strQry += "\r\n";
            strQry += "            LEFT JOIN EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC\r\n";
            strQry += "            ON I.COMPANY_NO = ETC.COMPANY_NO\r\n";
            strQry += "            AND I.EMPLOYEE_NO = ETC.EMPLOYEE_NO\r\n";
            strQry += "            AND I.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO\r\n";
            strQry += "            AND CONVERT(CHAR(8),ETC.TIMESHEET_DATE,112) + CONVERT(CHAR(4),ISNULL(ETC.TIMESHEET_SEQ,0)) =\r\n";
            strQry += "            --GET Only 1 Row\r\n";
            strQry += "\r\n";
            strQry += "           (SELECT MAX(CONVERT(CHAR(8),ETC.TIMESHEET_DATE,112) + CONVERT(CHAR(4),ISNULL(ETC.TIMESHEET_SEQ,0)))\r\n";
            strQry += "\r\n";
            strQry += "            FROM INSERTED I\r\n";
            strQry += "\r\n";
            strQry += "            INNER JOIN EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC\r\n";
            strQry += "            ON I.COMPANY_NO = ETC.COMPANY_NO\r\n";
            strQry += "            AND I.EMPLOYEE_NO = ETC.EMPLOYEE_NO\r\n";
            strQry += "            AND I.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO\r\n";
            strQry += "            --Go Back 1 Day\r\n";
            strQry += "            AND ETC.TIMESHEET_DATE >= DATEADD(DAY,-1,I.TIMESHEET_DATE))\r\n";
            strQry += "\r\n";
            strQry += "            PRINT '@I_TIMESHEET_DATE = ' + convert(varchar,@I_TIMESHEET_DATE)\r\n";
            strQry += "            PRINT '@I_TIMESHEET_TIME_MINUTES = ' + convert(varchar,@I_TIMESHEET_TIME_MINUTES)\r\n";
            strQry += "            PRINT '@TIMESHEET_DATE = ' + convert(varchar,@TIMESHEET_DATE)\r\n";
            strQry += "            PRINT '@TIMESHEET_TIME_IN_MINUTES = ' + convert(varchar,@TIMESHEET_TIME_IN_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "            -- Complete TimeSheet OR No Timesheets at All\r\n";
            strQry += "            IF @TIMESHEET_TIME_OUT_MINUTES IS NOT NULL\r\n";
            strQry += "            OR @TIMESHEET_DATE IS NULL\r\n";
            strQry += "\r\n";
            strQry += "                BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                PRINT 'Complete TimeSheet OR No Timesheets at All'\r\n";
            strQry += "                PRINT '@TIMESHEET_SEQ = ' + convert(varchar,@TIMESHEET_SEQ)\r\n";
            strQry += "\r\n";
            strQry += "                INSERT INTO EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT\r\n";
            strQry += "               (COMPANY_NO\r\n";
            strQry += "               ,EMPLOYEE_NO\r\n";
            strQry += "               ,PAY_CATEGORY_NO\r\n";
            strQry += "               ,TIMESHEET_DATE\r\n";
            strQry += "               ,TIMESHEET_SEQ\r\n";
            strQry += "               ,TIMESHEET_TIME_OUT_MINUTES\r\n";
            strQry += "               ,CLOCKED_TIME_OUT_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "                SELECT\r\n";
            strQry += "                I.COMPANY_NO\r\n";
            strQry += "               ,I.EMPLOYEE_NO\r\n";
            strQry += "               ,I.PAY_CATEGORY_NO\r\n";
            strQry += "               ,I.TIMESHEET_DATE\r\n";
            strQry += "               ,ISNULL(MAX(ETC.TIMESHEET_SEQ),0) + 1\r\n";
            strQry += "               ,I.TIMESHEET_TIME_MINUTES\r\n";
            strQry += "               ,I.TIMESHEET_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                FROM INSERTED I\r\n";
            strQry += "\r\n";
            strQry += "                LEFT JOIN EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC\r\n";
            strQry += "                ON I.COMPANY_NO = ETC.COMPANY_NO\r\n";
            strQry += "                AND I.EMPLOYEE_NO = ETC.EMPLOYEE_NO\r\n";
            strQry += "                AND I.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO\r\n";
            strQry += "                AND I.TIMESHEET_DATE = ETC.TIMESHEET_DATE\r\n";
            strQry += "\r\n";
            strQry += "                GROUP BY\r\n";
            strQry += "                I.COMPANY_NO\r\n";
            strQry += "               ,I.EMPLOYEE_NO\r\n";
            strQry += "               ,I.PAY_CATEGORY_NO\r\n";
            strQry += "               ,I.TIMESHEET_DATE\r\n";
            strQry += "               ,I.TIMESHEET_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                END\r\n";
            strQry += "\r\n";
            strQry += "            ELSE\r\n";
            strQry += "\r\n";
            strQry += "                BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                -- IF today\r\n";
            strQry += "                IF CONVERT(CHAR(8),@TIMESHEET_DATE,112) = CONVERT(CHAR(8),@I_TIMESHEET_DATE,112)\r\n";
            strQry += "\r\n";
            strQry += "                    BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                    PRINT 'Update - Today'\r\n";
            strQry += "\r\n";
            strQry += "                    UPDATE EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT\r\n";
            strQry += "                    SET TIMESHEET_TIME_OUT_MINUTES = @I_TIMESHEET_TIME_MINUTES\r\n";
            strQry += "                   ,CLOCKED_TIME_OUT_MINUTES = @I_TIMESHEET_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                    WHERE COMPANY_NO = @COMPANY_NO\r\n";
            strQry += "                    AND EMPLOYEE_NO = @EMPLOYEE_NO\r\n";
            strQry += "                    AND PAY_CATEGORY_NO = @PAY_CATEGORY_NO\r\n";
            strQry += "                    AND TIMESHEET_DATE = @I_TIMESHEET_DATE\r\n";
            strQry += "                    AND TIMESHEET_SEQ = @TIMESHEET_SEQ\r\n";
            strQry += "\r\n";
            strQry += "                    END\r\n";
            strQry += "\r\n";
            strQry += "                ELSE\r\n";
            strQry += "\r\n";
            strQry += "                    BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                    -- Within 24 Hours\r\n";
            strQry += "                    IF @I_TIMESHEET_TIME_MINUTES < @TIMESHEET_TIME_IN_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                        BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                        PRINT 'Within 24 Hours'\r\n";
            strQry += "\r\n";
            strQry += "                        --Clock Out Yesterday (Midnight)\r\n";
            strQry += "                        UPDATE EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT\r\n";
            strQry += "                        SET TIMESHEET_TIME_OUT_MINUTES = 1440\r\n";
            strQry += "\r\n";
            strQry += "                        WHERE COMPANY_NO = @COMPANY_NO\r\n";
            strQry += "                        AND EMPLOYEE_NO = @EMPLOYEE_NO\r\n";
            strQry += "                        AND PAY_CATEGORY_NO = @PAY_CATEGORY_NO\r\n";
            strQry += "                        AND TIMESHEET_DATE = @TIMESHEET_DATE\r\n";
            strQry += "                        AND TIMESHEET_SEQ = @TIMESHEET_SEQ\r\n";
            strQry += "\r\n";
            strQry += "                        --Clock In 0 Hours\r\n";
            strQry += "                        INSERT INTO EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT\r\n";
            strQry += "                       (COMPANY_NO\r\n";
            strQry += "                       ,EMPLOYEE_NO\r\n";
            strQry += "                       ,PAY_CATEGORY_NO\r\n";
            strQry += "                       ,TIMESHEET_DATE\r\n";
            strQry += "                       ,TIMESHEET_SEQ\r\n";
            strQry += "                       ,TIMESHEET_TIME_IN_MINUTES\r\n";
            strQry += "                       ,TIMESHEET_TIME_OUT_MINUTES\r\n";
            strQry += "                       ,CLOCKED_TIME_OUT_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "                        SELECT\r\n";
            strQry += "                        @COMPANY_NO\r\n";
            strQry += "                       ,@EMPLOYEE_NO\r\n";
            strQry += "                       ,@PAY_CATEGORY_NO\r\n";
            strQry += "                       ,@I_TIMESHEET_DATE\r\n";
            strQry += "                       ,ISNULL(MAX(ETC.TIMESHEET_SEQ),0) + 1\r\n";
            strQry += "                       ,0\r\n";
            strQry += "                       ,@I_TIMESHEET_TIME_MINUTES\r\n";
            strQry += "                       ,@I_TIMESHEET_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                        FROM EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC\r\n";
            strQry += "                        WHERE ETC.COMPANY_NO = @COMPANY_NO\r\n";
            strQry += "                        AND ETC.EMPLOYEE_NO = @EMPLOYEE_NO\r\n";
            strQry += "                        AND ETC.PAY_CATEGORY_NO = @PAY_CATEGORY_NO\r\n";
            strQry += "                        AND ETC.TIMESHEET_DATE = @I_TIMESHEET_DATE\r\n";
            strQry += "\r\n";
            strQry += "                        END\r\n";
            strQry += "\r\n";
            strQry += "                    ELSE\r\n";
            strQry += "\r\n";
            strQry += "                        BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                        PRINT 'NOT Within 24 Hours'\r\n";
            strQry += "\r\n";
            strQry += "                        INSERT INTO EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT\r\n";
            strQry += "                       (COMPANY_NO\r\n";
            strQry += "                       ,EMPLOYEE_NO\r\n";
            strQry += "                       ,PAY_CATEGORY_NO\r\n";
            strQry += "                       ,TIMESHEET_DATE\r\n";
            strQry += "                       ,TIMESHEET_SEQ\r\n";
            strQry += "                       ,TIMESHEET_TIME_OUT_MINUTES\r\n";
            strQry += "                       ,CLOCKED_TIME_OUT_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "                        SELECT\r\n";
            strQry += "                        @COMPANY_NO\r\n";
            strQry += "                       ,@EMPLOYEE_NO\r\n";
            strQry += "                       ,@PAY_CATEGORY_NO\r\n";
            strQry += "                       ,@I_TIMESHEET_DATE\r\n";
            strQry += "                       ,ISNULL(MAX(ETC.TIMESHEET_SEQ),0) + 1\r\n";
            strQry += "                       ,@I_TIMESHEET_TIME_MINUTES\r\n";
            strQry += "                       ,@I_TIMESHEET_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                        FROM EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC\r\n";
            strQry += "\r\n";
            strQry += "                        WHERE ETC.COMPANY_NO = @COMPANY_NO\r\n";
            strQry += "                        AND ETC.EMPLOYEE_NO = @EMPLOYEE_NO\r\n";
            strQry += "                        AND ETC.PAY_CATEGORY_NO = @PAY_CATEGORY_NO\r\n";
            strQry += "                        AND ETC.TIMESHEET_DATE = @I_TIMESHEET_DATE\r\n";
            strQry += "\r\n";
            strQry += "                        END\r\n";
            strQry += "\r\n";
            strQry += "                    END\r\n";
            strQry += "\r\n";
            strQry += "                END\r\n";
            strQry += "\r\n";
            strQry += "            END\r\n";
            strQry += "\r\n";
            strQry += "        END\r\n";
            strQry += "\r\n";
            strQry += "    END\r\n";

            return strQry;
        }

        public string Client_tgr_Create_Break()
        {
            string strQry = "";
            strQry += "CREATE TRIGGER tgr_Create_Break ON DEVICE_CLOCK_TIME_BREAK \r\n";
            strQry += "AFTER INSERT AS \r\n";
            strQry += "DECLARE @IN_OUT_IND AS VARCHAR(1) \r\n";
            strQry += "DECLARE @BREAK_DATE AS DATE \r\n";
            strQry += "DECLARE @I_BREAK_DATE AS DATE \r\n";
            strQry += "DECLARE @I_BREAK_TIME_MINUTES AS INT \r\n";
            strQry += "DECLARE @COMPANY_NO AS INT \r\n";
            strQry += "DECLARE @EMPLOYEE_NO AS INT \r\n";
            strQry += "DECLARE @PAY_CATEGORY_NO AS INT \r\n";
            strQry += "DECLARE @PAY_CATEGORY_TYPE AS VARCHAR(1) \r\n";
            strQry += "DECLARE @BREAK_SEQ AS INT \r\n";
            strQry += "DECLARE @BREAK_TIME_IN_MINUTES AS INT \r\n";
            strQry += "DECLARE @BREAK_TIME_OUT_MINUTES AS INT \r\n";
            strQry += "DECLARE @CURRENT_IDENTITY_SEED AS INT \r\n";
            strQry += "\r\n";
            strQry += "--Version 1.5 \r\n";
            strQry += "\r\n";
            strQry += "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;\r\n";
            strQry += "\r\n";
            strQry += "--Check Identity Seed \r\n";
            strQry += "SELECT @CURRENT_IDENTITY_SEED = IDENT_CURRENT(TABLE_NAME) \r\n";
            strQry += "FROM INFORMATION_SCHEMA.TABLES \r\n";
            strQry += "WHERE OBJECTPROPERTY(OBJECT_ID(TABLE_NAME), 'TableHasIdentity') = 1 \r\n";
            strQry += "AND TABLE_TYPE = 'BASE TABLE' \r\n";
            strQry += "AND TABLE_NAME = 'DEVICE_CLOCK_TIME_BREAK' \r\n";
            strQry += "\r\n";
            strQry += "PRINT '@CURRENT_IDENTITY_SEED = ' + convert(varchar,@CURRENT_IDENTITY_SEED) \r\n";
            strQry += "\r\n";
            strQry += "IF @CURRENT_IDENTITY_SEED >= 29999 \r\n";
            strQry += "\r\n";
            strQry += "    BEGIN \r\n";
            strQry += "\r\n";
            strQry += "    PRINT 'DEVICE_CLOCK_TIME_BREAK RESEED 0' \r\n";
            strQry += "\r\n";
            strQry += "    DBCC CHECKIDENT('DEVICE_CLOCK_TIME_BREAK', RESEED, 0) \r\n";
            strQry += "\r\n";
            strQry += "    END \r\n";
            strQry += "\r\n";
            strQry += "SELECT \r\n";
            strQry += " @IN_OUT_IND = IN_OUT_IND\r\n";
            strQry += ",@PAY_CATEGORY_TYPE = PAY_CATEGORY_TYPE\r\n";
            strQry += "\r\n";
            strQry += "FROM INSERTED \r\n";
            strQry += "\r\n";
            strQry += "IF @IN_OUT_IND = 'I'\r\n";
            strQry += "\r\n";
            strQry += "    BEGIN\r\n";
            strQry += "\r\n";
            strQry += "    IF @PAY_CATEGORY_TYPE = 'W'\r\n";
            strQry += "\r\n";
            strQry += "        BEGIN\r\n";
            strQry += "\r\n";
            strQry += "        INSERT INTO EMPLOYEE_BREAK_CURRENT\r\n";
            strQry += "       (COMPANY_NO\r\n";
            strQry += "       ,EMPLOYEE_NO\r\n";
            strQry += "       ,PAY_CATEGORY_NO\r\n";
            strQry += "       ,BREAK_DATE\r\n";
            strQry += "       ,BREAK_SEQ\r\n";
            strQry += "       ,BREAK_TIME_IN_MINUTES\r\n";
            strQry += "       ,CLOCKED_TIME_IN_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "        SELECT TOP 1\r\n";
            strQry += "        I.COMPANY_NO\r\n";
            strQry += "       ,I.EMPLOYEE_NO\r\n";
            strQry += "       ,I.PAY_CATEGORY_NO\r\n";
            strQry += "       ,I.BREAK_DATE\r\n";
            strQry += "       ,ISNULL(ETC.BREAK_SEQ,0) + 1\r\n";
            strQry += "       ,I.BREAK_TIME_MINUTES\r\n";
            strQry += "       ,I.BREAK_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "        FROM INSERTED I\r\n";
            strQry += "\r\n";
            strQry += "        LEFT JOIN EMPLOYEE_BREAK_CURRENT ETC\r\n";
            strQry += "        ON I.COMPANY_NO = ETC.COMPANY_NO\r\n";
            strQry += "        AND I.EMPLOYEE_NO = ETC.EMPLOYEE_NO\r\n";
            strQry += "        AND I.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO\r\n";
            strQry += "        AND I.BREAK_DATE = ETC.BREAK_DATE\r\n";
            strQry += "\r\n";
            strQry += "        ORDER  BY\r\n";
            strQry += "        I.COMPANY_NO\r\n";
            strQry += "       ,I.EMPLOYEE_NO\r\n";
            strQry += "       ,I.PAY_CATEGORY_NO\r\n";
            strQry += "       ,I.BREAK_DATE\r\n";
            strQry += "       ,ETC.BREAK_SEQ DESC\r\n";
            strQry += "\r\n";
            strQry += "        END\r\n";
            strQry += "\r\n";
            strQry += "    ELSE\r\n";
            strQry += "\r\n";
            strQry += "        BEGIN\r\n";
            strQry += "\r\n";
            strQry += "        IF @PAY_CATEGORY_TYPE = 'S'\r\n";
            strQry += "\r\n";
            strQry += "            BEGIN\r\n";
            strQry += "\r\n";
            strQry += "            INSERT INTO EMPLOYEE_SALARY_BREAK_CURRENT\r\n";
            strQry += "           (COMPANY_NO\r\n";
            strQry += "           ,EMPLOYEE_NO\r\n";
            strQry += "           ,PAY_CATEGORY_NO\r\n";
            strQry += "           ,BREAK_DATE\r\n";
            strQry += "           ,BREAK_SEQ\r\n";
            strQry += "           ,BREAK_TIME_IN_MINUTES\r\n";
            strQry += "           ,CLOCKED_TIME_IN_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "            SELECT TOP 1\r\n";
            strQry += "            I.COMPANY_NO\r\n";
            strQry += "           ,I.EMPLOYEE_NO\r\n";
            strQry += "           ,I.PAY_CATEGORY_NO\r\n";
            strQry += "           ,I.BREAK_DATE\r\n";
            strQry += "           ,ISNULL(ETC.BREAK_SEQ,0) + 1\r\n";
            strQry += "           ,I.BREAK_TIME_MINUTES\r\n";
            strQry += "           ,I.BREAK_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "            FROM INSERTED I\r\n";
            strQry += "\r\n";
            strQry += "            LEFT JOIN EMPLOYEE_SALARY_BREAK_CURRENT ETC\r\n";
            strQry += "            ON I.COMPANY_NO = ETC.COMPANY_NO\r\n";
            strQry += "            AND I.EMPLOYEE_NO = ETC.EMPLOYEE_NO\r\n";
            strQry += "            AND I.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO\r\n";
            strQry += "            AND I.BREAK_DATE = ETC.BREAK_DATE\r\n";
            strQry += "\r\n";
            strQry += "            ORDER  BY\r\n";
            strQry += "            I.COMPANY_NO\r\n";
            strQry += "           ,I.EMPLOYEE_NO\r\n";
            strQry += "           ,I.PAY_CATEGORY_NO\r\n";
            strQry += "           ,I.BREAK_DATE\r\n";
            strQry += "           ,ETC.BREAK_SEQ DESC\r\n";
            strQry += "\r\n";
            strQry += "           END\r\n";
            strQry += "\r\n";
            strQry += "        ELSE\r\n";
            strQry += "\r\n";
            strQry += "            BEGIN\r\n";
            strQry += "\r\n";
            strQry += "            --TIME ATTENDANCE\r\n";
            strQry += "            INSERT INTO EMPLOYEE_TIME_ATTEND_BREAK_CURRENT\r\n";
            strQry += "           (COMPANY_NO\r\n";
            strQry += "           ,EMPLOYEE_NO\r\n";
            strQry += "           ,PAY_CATEGORY_NO\r\n";
            strQry += "           ,BREAK_DATE\r\n";
            strQry += "           ,BREAK_SEQ\r\n";
            strQry += "           ,BREAK_TIME_IN_MINUTES\r\n";
            strQry += "           ,CLOCKED_TIME_IN_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "            SELECT TOP 1\r\n";
            strQry += "            I.COMPANY_NO\r\n";
            strQry += "           ,I.EMPLOYEE_NO\r\n";
            strQry += "           ,I.PAY_CATEGORY_NO\r\n";
            strQry += "           ,I.BREAK_DATE\r\n";
            strQry += "           ,ISNULL(ETC.BREAK_SEQ,0) + 1\r\n";
            strQry += "           ,I.BREAK_TIME_MINUTES\r\n";
            strQry += "           ,I.BREAK_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "            FROM INSERTED I\r\n";
            strQry += "\r\n";
            strQry += "            LEFT JOIN EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETC\r\n";
            strQry += "            ON I.COMPANY_NO = ETC.COMPANY_NO\r\n";
            strQry += "            AND I.EMPLOYEE_NO = ETC.EMPLOYEE_NO\r\n";
            strQry += "            AND I.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO\r\n";
            strQry += "            AND I.BREAK_DATE = ETC.BREAK_DATE\r\n";
            strQry += "\r\n";
            strQry += "            ORDER  BY\r\n";
            strQry += "            I.COMPANY_NO\r\n";
            strQry += "           ,I.EMPLOYEE_NO\r\n";
            strQry += "           ,I.PAY_CATEGORY_NO\r\n";
            strQry += "           ,I.BREAK_DATE\r\n";
            strQry += "           ,ETC.BREAK_SEQ DESC\r\n";
            strQry += "\r\n";
            strQry += "            END\r\n";
            strQry += "\r\n";
            strQry += "        END\r\n";
            strQry += "\r\n";
            strQry += "    END\r\n";
            strQry += "\r\n";
            strQry += "ELSE\r\n";
            strQry += "\r\n";
            strQry += "    --OUT\r\n";
            strQry += "\r\n";
            strQry += "    BEGIN\r\n";
            strQry += "\r\n";
            strQry += "    IF @PAY_CATEGORY_TYPE = 'W'\r\n";
            strQry += "\r\n";
            strQry += "        BEGIN\r\n";
            strQry += "\r\n";
            strQry += "        SELECT TOP 1\r\n";
            strQry += "        @COMPANY_NO = I.COMPANY_NO\r\n";
            strQry += "       ,@EMPLOYEE_NO = I.EMPLOYEE_NO\r\n";
            strQry += "       ,@PAY_CATEGORY_NO = I.PAY_CATEGORY_NO\r\n";
            strQry += "       ,@I_BREAK_DATE = I.BREAK_DATE\r\n";
            strQry += "       ,@BREAK_DATE = ETC.BREAK_DATE\r\n";
            strQry += "       ,@I_BREAK_TIME_MINUTES = I.BREAK_TIME_MINUTES\r\n";
            strQry += "       ,@BREAK_TIME_IN_MINUTES = ETC.BREAK_TIME_IN_MINUTES\r\n";
            strQry += "       ,@BREAK_TIME_OUT_MINUTES = ETC.BREAK_TIME_OUT_MINUTES\r\n";
            strQry += "       ,@BREAK_SEQ = ISNULL(ETC.BREAK_SEQ,0)\r\n";
            strQry += "\r\n";
            strQry += "        FROM INSERTED I\r\n";
            strQry += "\r\n";
            strQry += "        LEFT JOIN EMPLOYEE_BREAK_CURRENT ETC\r\n";
            strQry += "        ON I.COMPANY_NO = ETC.COMPANY_NO\r\n";
            strQry += "        AND I.EMPLOYEE_NO = ETC.EMPLOYEE_NO\r\n";
            strQry += "        AND I.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO\r\n";
            strQry += "        --Go Back 1 Day\r\n";
            strQry += "        AND ETC.BREAK_DATE >= DATEADD(DAY,-1,I.BREAK_DATE)\r\n";
            strQry += "\r\n";
            strQry += "        ORDER BY\r\n";
            strQry += "        ETC.BREAK_DATE DESC\r\n";
            strQry += "       ,ETC.BREAK_SEQ DESC\r\n";
            strQry += "\r\n";
            strQry += "        PRINT '@I_BREAK_DATE = ' + convert(varchar,@I_BREAK_DATE)\r\n";
            strQry += "        PRINT '@I_BREAK_TIME_MINUTES = ' + convert(varchar,@I_BREAK_TIME_MINUTES)\r\n";
            strQry += "        PRINT '@BREAK_DATE = ' + convert(varchar,@BREAK_DATE)\r\n";
            strQry += "        PRINT '@BREAK_TIME_IN_MINUTES = ' + convert(varchar,@BREAK_TIME_IN_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "        -- Complete TimeSheet OR No Timesheets at All\r\n";
            strQry += "        IF @BREAK_TIME_OUT_MINUTES IS NOT NULL\r\n";
            strQry += "        OR @BREAK_DATE IS NULL\r\n";
            strQry += "\r\n";
            strQry += "            BEGIN\r\n";
            strQry += "\r\n";
            strQry += "            PRINT 'Complete TimeSheet OR No Timesheets at All'\r\n";
            strQry += "\r\n";
            strQry += "            INSERT INTO EMPLOYEE_BREAK_CURRENT\r\n";
            strQry += "           (COMPANY_NO\r\n";
            strQry += "           ,EMPLOYEE_NO\r\n";
            strQry += "           ,PAY_CATEGORY_NO\r\n";
            strQry += "           ,BREAK_DATE\r\n";
            strQry += "           ,BREAK_SEQ\r\n";
            strQry += "           ,BREAK_TIME_OUT_MINUTES\r\n";
            strQry += "           ,CLOCKED_TIME_OUT_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "            SELECT\r\n";
            strQry += "            I.COMPANY_NO\r\n";
            strQry += "           ,I.EMPLOYEE_NO\r\n";
            strQry += "           ,I.PAY_CATEGORY_NO\r\n";
            strQry += "           ,I.BREAK_DATE\r\n";
            strQry += "           ,ISNULL(MAX(ETC.BREAK_SEQ),0) + 1\r\n";
            strQry += "           ,I.BREAK_TIME_MINUTES\r\n";
            strQry += "           ,@I_BREAK_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "            FROM INSERTED I\r\n";
            strQry += "\r\n";
            strQry += "            LEFT JOIN EMPLOYEE_BREAK_CURRENT ETC\r\n";
            strQry += "            ON I.COMPANY_NO = ETC.COMPANY_NO\r\n";
            strQry += "            AND I.EMPLOYEE_NO = ETC.EMPLOYEE_NO\r\n";
            strQry += "            AND I.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO\r\n";
            strQry += "            AND I.BREAK_DATE = ETC.BREAK_DATE\r\n";
            strQry += "            GROUP BY\r\n";
            strQry += "            I.COMPANY_NO\r\n";
            strQry += "           ,I.EMPLOYEE_NO\r\n";
            strQry += "           ,I.PAY_CATEGORY_NO\r\n";
            strQry += "           ,I.BREAK_DATE\r\n";
            strQry += "           ,I.BREAK_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "            END\r\n";
            strQry += "\r\n";
            strQry += "        ELSE\r\n";
            strQry += "\r\n";
            strQry += "            BEGIN\r\n";
            strQry += "\r\n";
            strQry += "            -- IF today\r\n";
            strQry += "            IF CONVERT(CHAR(8),@BREAK_DATE,112) = CONVERT(CHAR(8),@I_BREAK_DATE,112)\r\n";
            strQry += "\r\n";
            strQry += "                BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                PRINT 'Update - Today'\r\n";
            strQry += "\r\n";
            strQry += "                UPDATE EMPLOYEE_BREAK_CURRENT\r\n";
            strQry += "                SET BREAK_TIME_OUT_MINUTES = @I_BREAK_TIME_MINUTES\r\n";
            strQry += "               ,CLOCKED_TIME_OUT_MINUTES = @I_BREAK_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                WHERE COMPANY_NO = @COMPANY_NO\r\n";
            strQry += "                AND EMPLOYEE_NO = @EMPLOYEE_NO\r\n";
            strQry += "                AND PAY_CATEGORY_NO = @PAY_CATEGORY_NO\r\n";
            strQry += "                AND BREAK_DATE = @I_BREAK_DATE\r\n";
            strQry += "                AND BREAK_SEQ = @BREAK_SEQ\r\n";
            strQry += "\r\n";
            strQry += "                END\r\n";
            strQry += "\r\n";
            strQry += "            ELSE\r\n";
            strQry += "\r\n";
            strQry += "                BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                -- Within 24 Hours\r\n";
            strQry += "                IF @I_BREAK_TIME_MINUTES < @BREAK_TIME_IN_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                    BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                    PRINT 'Within 24 Hours'\r\n";
            strQry += "\r\n";
            strQry += "                    --Clock Out Yesterday (Midnight)\r\n";
            strQry += "                    UPDATE EMPLOYEE_BREAK_CURRENT\r\n";
            strQry += "                    SET BREAK_TIME_OUT_MINUTES = 1440\r\n";
            strQry += "                   ,CLOCKED_TIME_OUT_MINUTES = 1440\r\n";
            strQry += "\r\n";
            strQry += "                    WHERE COMPANY_NO = @COMPANY_NO\r\n";
            strQry += "                    AND EMPLOYEE_NO = @EMPLOYEE_NO\r\n";
            strQry += "                    AND PAY_CATEGORY_NO = @PAY_CATEGORY_NO\r\n";
            strQry += "                    AND BREAK_DATE = @BREAK_DATE\r\n";
            strQry += "                    AND BREAK_SEQ = @BREAK_SEQ\r\n";
            strQry += "\r\n";
            strQry += "                    --Clock In 0 Hours\r\n";
            strQry += "                    INSERT INTO EMPLOYEE_BREAK_CURRENT\r\n";
            strQry += "                   (COMPANY_NO\r\n";
            strQry += "                   ,EMPLOYEE_NO\r\n";
            strQry += "                   ,PAY_CATEGORY_NO\r\n";
            strQry += "                   ,BREAK_DATE\r\n";
            strQry += "                   ,BREAK_SEQ\r\n";
            strQry += "                   ,BREAK_TIME_IN_MINUTES\r\n";
            strQry += "                   ,CLOCKED_TIME_IN_MINUTES\r\n";
            strQry += "                   ,BREAK_TIME_OUT_MINUTES\r\n";
            strQry += "                   ,CLOCKED_TIME_OUT_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "                    SELECT\r\n";
            strQry += "                    @COMPANY_NO\r\n";
            strQry += "                   ,@EMPLOYEE_NO\r\n";
            strQry += "                   ,@PAY_CATEGORY_NO\r\n";
            strQry += "                   ,@I_BREAK_DATE\r\n";
            strQry += "                   ,ISNULL(MAX(ETC.BREAK_SEQ),0) + 1\r\n";
            strQry += "                   ,0\r\n";
            strQry += "                   ,0\r\n";
            strQry += "                   ,@I_BREAK_TIME_MINUTES\r\n";
            strQry += "                   ,@I_BREAK_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                    FROM EMPLOYEE_BREAK_CURRENT ETC\r\n";
            strQry += "\r\n";
            strQry += "                    WHERE ETC.COMPANY_NO = @COMPANY_NO\r\n";
            strQry += "                    AND ETC.EMPLOYEE_NO = @EMPLOYEE_NO\r\n";
            strQry += "                    AND ETC.PAY_CATEGORY_NO = @PAY_CATEGORY_NO\r\n";
            strQry += "                    AND ETC.BREAK_DATE = @I_BREAK_DATE\r\n";
            strQry += "\r\n";
            strQry += "                    END\r\n";
            strQry += "\r\n";
            strQry += "                ELSE\r\n";
            strQry += "\r\n";
            strQry += "                    BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                    PRINT 'NOT Within 24 Hours'\r\n";
            strQry += "\r\n";
            strQry += "                    INSERT INTO EMPLOYEE_BREAK_CURRENT\r\n";
            strQry += "                   (COMPANY_NO\r\n";
            strQry += "                   ,EMPLOYEE_NO\r\n";
            strQry += "                   ,PAY_CATEGORY_NO\r\n";
            strQry += "                   ,BREAK_DATE\r\n";
            strQry += "                   ,BREAK_SEQ\r\n";
            strQry += "                   ,BREAK_TIME_OUT_MINUTES\r\n";
            strQry += "                   ,CLOCKED_TIME_OUT_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "                    SELECT\r\n";
            strQry += "                    @COMPANY_NO\r\n";
            strQry += "                   ,@EMPLOYEE_NO\r\n";
            strQry += "                   ,@PAY_CATEGORY_NO\r\n";
            strQry += "                   ,@I_BREAK_DATE\r\n";
            strQry += "                   ,ISNULL(MAX(ETC.BREAK_SEQ),0) + 1\r\n";
            strQry += "                   ,@I_BREAK_TIME_MINUTES\r\n";
            strQry += "                   ,@I_BREAK_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                    FROM EMPLOYEE_BREAK_CURRENT ETC\r\n";
            strQry += "\r\n";
            strQry += "                    WHERE ETC.COMPANY_NO = @COMPANY_NO\r\n";
            strQry += "                    AND ETC.EMPLOYEE_NO = @EMPLOYEE_NO\r\n";
            strQry += "                    AND ETC.PAY_CATEGORY_NO = @PAY_CATEGORY_NO\r\n";
            strQry += "                    AND ETC.BREAK_DATE = @I_BREAK_DATE\r\n";
            strQry += "\r\n";
            strQry += "                    END\r\n";
            strQry += "\r\n";
            strQry += "                END\r\n";
            strQry += "\r\n";
            strQry += "            END\r\n";
            strQry += "\r\n";
            strQry += "        END\r\n";
            strQry += "\r\n";
            strQry += "    ELSE\r\n";
            strQry += "\r\n";
            strQry += "        BEGIN\r\n";
            strQry += "\r\n";
            strQry += "        IF @PAY_CATEGORY_TYPE = 'S'\r\n";
            strQry += "\r\n";
            strQry += "            BEGIN\r\n";
            strQry += "\r\n";
            strQry += "            --SALARIES\r\n";
            strQry += "            SELECT\r\n";
            strQry += "            @COMPANY_NO = I.COMPANY_NO\r\n";
            strQry += "           ,@EMPLOYEE_NO = I.EMPLOYEE_NO\r\n";
            strQry += "           ,@PAY_CATEGORY_NO = I.PAY_CATEGORY_NO\r\n";
            strQry += "           ,@I_BREAK_DATE = I.BREAK_DATE\r\n";
            strQry += "           ,@BREAK_DATE = ETC.BREAK_DATE\r\n";
            strQry += "           ,@I_BREAK_TIME_MINUTES = I.BREAK_TIME_MINUTES\r\n";
            strQry += "           ,@BREAK_TIME_IN_MINUTES = ETC.BREAK_TIME_IN_MINUTES\r\n";
            strQry += "           ,@BREAK_TIME_OUT_MINUTES = ETC.BREAK_TIME_OUT_MINUTES\r\n";
            strQry += "           ,@BREAK_SEQ = ISNULL(ETC.BREAK_SEQ,0)\r\n";
            strQry += "\r\n";
            strQry += "            FROM INSERTED I\r\n";
            strQry += "\r\n";
            strQry += "            LEFT JOIN EMPLOYEE_SALARY_BREAK_CURRENT ETC\r\n";
            strQry += "            ON I.COMPANY_NO = ETC.COMPANY_NO\r\n";
            strQry += "            AND I.EMPLOYEE_NO = ETC.EMPLOYEE_NO\r\n";
            strQry += "            AND I.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO\r\n";
            strQry += "            AND CONVERT(CHAR(8),ETC.BREAK_DATE,112) + CONVERT(CHAR(4),ISNULL(ETC.BREAK_SEQ,0)) =\r\n";
            strQry += "            --GET Only 1 Row\r\n";
            strQry += "\r\n";
            strQry += "           (SELECT MAX(CONVERT(CHAR(8),ETC.BREAK_DATE,112) + CONVERT(CHAR(4),ISNULL(ETC.BREAK_SEQ,0)))\r\n";
            strQry += "\r\n";
            strQry += "            FROM INSERTED I\r\n";
            strQry += "\r\n";
            strQry += "            INNER JOIN EMPLOYEE_SALARY_BREAK_CURRENT ETC\r\n";
            strQry += "            ON I.COMPANY_NO = ETC.COMPANY_NO\r\n";
            strQry += "            AND I.EMPLOYEE_NO = ETC.EMPLOYEE_NO\r\n";
            strQry += "            AND I.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO\r\n";
            strQry += "            --Go Back 1 Day\r\n";
            strQry += "            AND ETC.BREAK_DATE >= DATEADD(DAY,-1,I.BREAK_DATE))\r\n";
            strQry += "\r\n";
            strQry += "            PRINT '@I_BREAK_DATE = ' + convert(varchar,@I_BREAK_DATE)\r\n";
            strQry += "            PRINT '@I_BREAK_TIME_MINUTES = ' + convert(varchar,@I_BREAK_TIME_MINUTES)\r\n";
            strQry += "            PRINT '@BREAK_DATE = ' + convert(varchar,@BREAK_DATE)\r\n";
            strQry += "            PRINT '@BREAK_TIME_IN_MINUTES = ' + convert(varchar,@BREAK_TIME_IN_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "            -- Complete TimeSheet OR No Timesheets at All\r\n";
            strQry += "            IF @BREAK_TIME_OUT_MINUTES IS NOT NULL\r\n";
            strQry += "            OR @BREAK_DATE IS NULL\r\n";
            strQry += "\r\n";
            strQry += "                BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                PRINT 'Complete TimeSheet OR No Timesheets at All'\r\n";
            strQry += "                PRINT '@BREAK_SEQ = ' + convert(varchar,@BREAK_SEQ)\r\n";
            strQry += "\r\n";
            strQry += "                INSERT INTO EMPLOYEE_SALARY_BREAK_CURRENT\r\n";
            strQry += "               (COMPANY_NO\r\n";
            strQry += "               ,EMPLOYEE_NO\r\n";
            strQry += "               ,PAY_CATEGORY_NO\r\n";
            strQry += "               ,BREAK_DATE\r\n";
            strQry += "               ,BREAK_SEQ\r\n";
            strQry += "               ,BREAK_TIME_OUT_MINUTES\r\n";
            strQry += "               ,CLOCKED_TIME_OUT_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "                SELECT\r\n";
            strQry += "                I.COMPANY_NO\r\n";
            strQry += "               ,I.EMPLOYEE_NO\r\n";
            strQry += "               ,I.PAY_CATEGORY_NO\r\n";
            strQry += "               ,I.BREAK_DATE\r\n";
            strQry += "               ,ISNULL(MAX(ETC.BREAK_SEQ),0) + 1\r\n";
            strQry += "               ,I.BREAK_TIME_MINUTES\r\n";
            strQry += "               ,I.BREAK_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                FROM INSERTED I\r\n";
            strQry += "\r\n";
            strQry += "                LEFT JOIN EMPLOYEE_SALARY_BREAK_CURRENT ETC\r\n";
            strQry += "                ON I.COMPANY_NO = ETC.COMPANY_NO\r\n";
            strQry += "                AND I.EMPLOYEE_NO = ETC.EMPLOYEE_NO\r\n";
            strQry += "                AND I.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO\r\n";
            strQry += "                AND I.BREAK_DATE = ETC.BREAK_DATE\r\n";
            strQry += "\r\n";
            strQry += "                GROUP BY\r\n";
            strQry += "                I.COMPANY_NO\r\n";
            strQry += "               ,I.EMPLOYEE_NO\r\n";
            strQry += "               ,I.PAY_CATEGORY_NO\r\n";
            strQry += "               ,I.BREAK_DATE\r\n";
            strQry += "               ,I.BREAK_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                END\r\n";
            strQry += "\r\n";
            strQry += "            ELSE\r\n";
            strQry += "\r\n";
            strQry += "                BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                -- IF today\r\n";
            strQry += "                IF CONVERT(CHAR(8),@BREAK_DATE,112) = CONVERT(CHAR(8),@I_BREAK_DATE,112)\r\n";
            strQry += "\r\n";
            strQry += "                    BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                    PRINT 'Update - Today'\r\n";
            strQry += "\r\n";
            strQry += "                    UPDATE EMPLOYEE_SALARY_BREAK_CURRENT\r\n";
            strQry += "                    SET BREAK_TIME_OUT_MINUTES = @I_BREAK_TIME_MINUTES\r\n";
            strQry += "                   ,CLOCKED_TIME_OUT_MINUTES = @I_BREAK_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                    WHERE COMPANY_NO = @COMPANY_NO\r\n";
            strQry += "                    AND EMPLOYEE_NO = @EMPLOYEE_NO\r\n";
            strQry += "                    AND PAY_CATEGORY_NO = @PAY_CATEGORY_NO\r\n";
            strQry += "                    AND BREAK_DATE = @I_BREAK_DATE\r\n";
            strQry += "                    AND BREAK_SEQ = @BREAK_SEQ\r\n";
            strQry += "\r\n";
            strQry += "                    END\r\n";
            strQry += "\r\n";
            strQry += "                ELSE\r\n";
            strQry += "\r\n";
            strQry += "                    BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                    -- Within 24 Hours\r\n";
            strQry += "                    IF @I_BREAK_TIME_MINUTES < @BREAK_TIME_IN_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                        BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                        PRINT 'Within 24 Hours'\r\n";
            strQry += "\r\n";
            strQry += "                        --Clock Out Yesterday (Midnight)\r\n";
            strQry += "                        UPDATE EMPLOYEE_SALARY_BREAK_CURRENT\r\n";
            strQry += "                        SET BREAK_TIME_OUT_MINUTES = 1440\r\n";
            strQry += "\r\n";
            strQry += "                        WHERE COMPANY_NO = @COMPANY_NO\r\n";
            strQry += "                        AND EMPLOYEE_NO = @EMPLOYEE_NO\r\n";
            strQry += "                        AND PAY_CATEGORY_NO = @PAY_CATEGORY_NO\r\n";
            strQry += "                        AND BREAK_DATE = @BREAK_DATE\r\n";
            strQry += "                        AND BREAK_SEQ = @BREAK_SEQ\r\n";
            strQry += "\r\n";
            strQry += "                        --Clock In 0 Hours\r\n";
            strQry += "                        INSERT INTO EMPLOYEE_SALARY_BREAK_CURRENT\r\n";
            strQry += "                       (COMPANY_NO\r\n";
            strQry += "                       ,EMPLOYEE_NO\r\n";
            strQry += "                       ,PAY_CATEGORY_NO\r\n";
            strQry += "                       ,BREAK_DATE\r\n";
            strQry += "                       ,BREAK_SEQ\r\n";
            strQry += "                       ,BREAK_TIME_IN_MINUTES\r\n";
            strQry += "                       ,BREAK_TIME_OUT_MINUTES\r\n";
            strQry += "                       ,CLOCKED_TIME_OUT_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "                        SELECT\r\n";
            strQry += "                        @COMPANY_NO\r\n";
            strQry += "                       ,@EMPLOYEE_NO\r\n";
            strQry += "                       ,@PAY_CATEGORY_NO\r\n";
            strQry += "                       ,@I_BREAK_DATE\r\n";
            strQry += "                       ,ISNULL(MAX(ETC.BREAK_SEQ),0) + 1\r\n";
            strQry += "                       ,0\r\n";
            strQry += "                       ,@I_BREAK_TIME_MINUTES\r\n";
            strQry += "                       ,@I_BREAK_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                        FROM EMPLOYEE_SALARY_BREAK_CURRENT ETC\r\n";
            strQry += "\r\n";
            strQry += "                        WHERE ETC.COMPANY_NO = @COMPANY_NO\r\n";
            strQry += "                        AND ETC.EMPLOYEE_NO = @EMPLOYEE_NO\r\n";
            strQry += "                        AND ETC.PAY_CATEGORY_NO = @PAY_CATEGORY_NO\r\n";
            strQry += "                        AND ETC.BREAK_DATE = @I_BREAK_DATE\r\n";
            strQry += "\r\n";
            strQry += "                        END\r\n";
            strQry += "\r\n";
            strQry += "                    ELSE\r\n";
            strQry += "\r\n";
            strQry += "                        BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                        PRINT 'NOT Within 24 Hours'\r\n";
            strQry += "\r\n";
            strQry += "                        INSERT INTO EMPLOYEE_SALARY_BREAK_CURRENT\r\n";
            strQry += "                       (COMPANY_NO\r\n";
            strQry += "                       ,EMPLOYEE_NO\r\n";
            strQry += "                       ,PAY_CATEGORY_NO\r\n";
            strQry += "                       ,BREAK_DATE\r\n";
            strQry += "                       ,BREAK_SEQ\r\n";
            strQry += "                       ,BREAK_TIME_OUT_MINUTES\r\n";
            strQry += "                       ,CLOCKED_TIME_OUT_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "                        SELECT\r\n";
            strQry += "                        @COMPANY_NO\r\n";
            strQry += "                       ,@EMPLOYEE_NO\r\n";
            strQry += "                       ,@PAY_CATEGORY_NO\r\n";
            strQry += "                       ,@I_BREAK_DATE\r\n";
            strQry += "                       ,ISNULL(MAX(ETC.BREAK_SEQ),0) + 1\r\n";
            strQry += "                       ,@I_BREAK_TIME_MINUTES\r\n";
            strQry += "                       ,@I_BREAK_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                        FROM EMPLOYEE_SALARY_BREAK_CURRENT ETC\r\n";
            strQry += "\r\n";
            strQry += "                        WHERE ETC.COMPANY_NO = @COMPANY_NO\r\n";
            strQry += "                        AND ETC.EMPLOYEE_NO = @EMPLOYEE_NO\r\n";
            strQry += "                        AND ETC.PAY_CATEGORY_NO = @PAY_CATEGORY_NO\r\n";
            strQry += "                        AND ETC.BREAK_DATE = @I_BREAK_DATE\r\n";
            strQry += "\r\n";
            strQry += "                        END\r\n";
            strQry += "\r\n";
            strQry += "                    END\r\n";
            strQry += "\r\n";
            strQry += "                END\r\n";
            strQry += "\r\n";
            strQry += "            END\r\n";
            strQry += "\r\n";
            strQry += "        ELSE\r\n";
            strQry += "\r\n";
            strQry += "            BEGIN\r\n";
            strQry += "\r\n";
            strQry += "            --TIME ATTENDANCE\r\n";
            strQry += "            SELECT\r\n";
            strQry += "            @COMPANY_NO = I.COMPANY_NO\r\n";
            strQry += "           ,@EMPLOYEE_NO = I.EMPLOYEE_NO\r\n";
            strQry += "           ,@PAY_CATEGORY_NO = I.PAY_CATEGORY_NO\r\n";
            strQry += "           ,@I_BREAK_DATE = I.BREAK_DATE\r\n";
            strQry += "           ,@BREAK_DATE = ETC.BREAK_DATE\r\n";
            strQry += "           ,@I_BREAK_TIME_MINUTES = I.BREAK_TIME_MINUTES\r\n";
            strQry += "           ,@BREAK_TIME_IN_MINUTES = ETC.BREAK_TIME_IN_MINUTES\r\n";
            strQry += "           ,@BREAK_TIME_OUT_MINUTES = ETC.BREAK_TIME_OUT_MINUTES\r\n";
            strQry += "           ,@BREAK_SEQ = ISNULL(ETC.BREAK_SEQ,0)\r\n";
            strQry += "\r\n";
            strQry += "            FROM INSERTED I\r\n";
            strQry += "\r\n";
            strQry += "            LEFT JOIN EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETC\r\n";
            strQry += "            ON I.COMPANY_NO = ETC.COMPANY_NO\r\n";
            strQry += "            AND I.EMPLOYEE_NO = ETC.EMPLOYEE_NO\r\n";
            strQry += "            AND I.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO\r\n";
            strQry += "            AND CONVERT(CHAR(8),ETC.BREAK_DATE,112) + CONVERT(CHAR(4),ISNULL(ETC.BREAK_SEQ,0)) =\r\n";
            strQry += "            --GET Only 1 Row\r\n";
            strQry += "\r\n";
            strQry += "           (SELECT MAX(CONVERT(CHAR(8),ETC.BREAK_DATE,112) + CONVERT(CHAR(4),ISNULL(ETC.BREAK_SEQ,0)))\r\n";
            strQry += "\r\n";
            strQry += "            FROM INSERTED I\r\n";
            strQry += "\r\n";
            strQry += "            INNER JOIN EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETC\r\n";
            strQry += "            ON I.COMPANY_NO = ETC.COMPANY_NO\r\n";
            strQry += "            AND I.EMPLOYEE_NO = ETC.EMPLOYEE_NO\r\n";
            strQry += "            AND I.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO\r\n";
            strQry += "            --Go Back 1 Day\r\n";
            strQry += "            AND ETC.BREAK_DATE >= DATEADD(DAY,-1,I.BREAK_DATE))\r\n";
            strQry += "\r\n";
            strQry += "            PRINT '@I_BREAK_DATE = ' + convert(varchar,@I_BREAK_DATE)\r\n";
            strQry += "            PRINT '@I_BREAK_TIME_MINUTES = ' + convert(varchar,@I_BREAK_TIME_MINUTES)\r\n";
            strQry += "            PRINT '@BREAK_DATE = ' + convert(varchar,@BREAK_DATE)\r\n";
            strQry += "            PRINT '@BREAK_TIME_IN_MINUTES = ' + convert(varchar,@BREAK_TIME_IN_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "            -- Complete TimeSheet OR No Timesheets at All\r\n";
            strQry += "            IF @BREAK_TIME_OUT_MINUTES IS NOT NULL\r\n";
            strQry += "            OR @BREAK_DATE IS NULL\r\n";
            strQry += "\r\n";
            strQry += "                BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                PRINT 'Complete TimeSheet OR No Timesheets at All'\r\n";
            strQry += "                PRINT '@BREAK_SEQ = ' + convert(varchar,@BREAK_SEQ)\r\n";
            strQry += "\r\n";
            strQry += "                INSERT INTO EMPLOYEE_TIME_ATTEND_BREAK_CURRENT\r\n";
            strQry += "               (COMPANY_NO\r\n";
            strQry += "               ,EMPLOYEE_NO\r\n";
            strQry += "               ,PAY_CATEGORY_NO\r\n";
            strQry += "               ,BREAK_DATE\r\n";
            strQry += "               ,BREAK_SEQ\r\n";
            strQry += "               ,BREAK_TIME_OUT_MINUTES\r\n";
            strQry += "               ,CLOCKED_TIME_OUT_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "                SELECT\r\n";
            strQry += "                I.COMPANY_NO\r\n";
            strQry += "               ,I.EMPLOYEE_NO\r\n";
            strQry += "               ,I.PAY_CATEGORY_NO\r\n";
            strQry += "               ,I.BREAK_DATE\r\n";
            strQry += "               ,ISNULL(MAX(ETC.BREAK_SEQ),0) + 1\r\n";
            strQry += "               ,I.BREAK_TIME_MINUTES\r\n";
            strQry += "               ,I.BREAK_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                FROM INSERTED I\r\n";
            strQry += "\r\n";
            strQry += "                LEFT JOIN EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETC\r\n";
            strQry += "                ON I.COMPANY_NO = ETC.COMPANY_NO\r\n";
            strQry += "                AND I.EMPLOYEE_NO = ETC.EMPLOYEE_NO\r\n";
            strQry += "                AND I.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO\r\n";
            strQry += "                AND I.BREAK_DATE = ETC.BREAK_DATE\r\n";
            strQry += "\r\n";
            strQry += "                GROUP BY\r\n";
            strQry += "                I.COMPANY_NO\r\n";
            strQry += "               ,I.EMPLOYEE_NO\r\n";
            strQry += "               ,I.PAY_CATEGORY_NO\r\n";
            strQry += "               ,I.BREAK_DATE\r\n";
            strQry += "               ,I.BREAK_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                END\r\n";
            strQry += "\r\n";
            strQry += "            ELSE\r\n";
            strQry += "\r\n";
            strQry += "                BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                -- IF today\r\n";
            strQry += "                IF CONVERT(CHAR(8),@BREAK_DATE,112) = CONVERT(CHAR(8),@I_BREAK_DATE,112)\r\n";
            strQry += "\r\n";
            strQry += "                    BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                    PRINT 'Update - Today'\r\n";
            strQry += "\r\n";
            strQry += "                    UPDATE EMPLOYEE_TIME_ATTEND_BREAK_CURRENT\r\n";
            strQry += "                    SET BREAK_TIME_OUT_MINUTES = @I_BREAK_TIME_MINUTES\r\n";
            strQry += "                   ,CLOCKED_TIME_OUT_MINUTES = @I_BREAK_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                    WHERE COMPANY_NO = @COMPANY_NO\r\n";
            strQry += "                    AND EMPLOYEE_NO = @EMPLOYEE_NO\r\n";
            strQry += "                    AND PAY_CATEGORY_NO = @PAY_CATEGORY_NO\r\n";
            strQry += "                    AND BREAK_DATE = @I_BREAK_DATE\r\n";
            strQry += "                    AND BREAK_SEQ = @BREAK_SEQ\r\n";
            strQry += "\r\n";
            strQry += "                    END\r\n";
            strQry += "\r\n";
            strQry += "                ELSE\r\n";
            strQry += "\r\n";
            strQry += "                    BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                    -- Within 24 Hours\r\n";
            strQry += "                    IF @I_BREAK_TIME_MINUTES < @BREAK_TIME_IN_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                        BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                        PRINT 'Within 24 Hours'\r\n";
            strQry += "\r\n";
            strQry += "                        --Clock Out Yesterday (Midnight)\r\n";
            strQry += "                        UPDATE EMPLOYEE_TIME_ATTEND_BREAK_CURRENT\r\n";
            strQry += "                        SET BREAK_TIME_OUT_MINUTES = 1440\r\n";
            strQry += "\r\n";
            strQry += "                        WHERE COMPANY_NO = @COMPANY_NO\r\n";
            strQry += "                        AND EMPLOYEE_NO = @EMPLOYEE_NO\r\n";
            strQry += "                        AND PAY_CATEGORY_NO = @PAY_CATEGORY_NO\r\n";
            strQry += "                        AND BREAK_DATE = @BREAK_DATE\r\n";
            strQry += "                        AND BREAK_SEQ = @BREAK_SEQ\r\n";
            strQry += "\r\n";
            strQry += "                        --Clock In 0 Hours\r\n";
            strQry += "                        INSERT INTO EMPLOYEE_TIME_ATTEND_BREAK_CURRENT\r\n";
            strQry += "                       (COMPANY_NO\r\n";
            strQry += "                       ,EMPLOYEE_NO\r\n";
            strQry += "                       ,PAY_CATEGORY_NO\r\n";
            strQry += "                       ,BREAK_DATE\r\n";
            strQry += "                       ,BREAK_SEQ\r\n";
            strQry += "                       ,BREAK_TIME_IN_MINUTES\r\n";
            strQry += "                       ,BREAK_TIME_OUT_MINUTES\r\n";
            strQry += "                       ,CLOCKED_TIME_OUT_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "                        SELECT\r\n";
            strQry += "                        @COMPANY_NO\r\n";
            strQry += "                       ,@EMPLOYEE_NO\r\n";
            strQry += "                       ,@PAY_CATEGORY_NO\r\n";
            strQry += "                       ,@I_BREAK_DATE\r\n";
            strQry += "                       ,ISNULL(MAX(ETC.BREAK_SEQ),0) + 1\r\n";
            strQry += "                       ,0\r\n";
            strQry += "                       ,@I_BREAK_TIME_MINUTES\r\n";
            strQry += "                       ,@I_BREAK_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                        FROM EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETC\r\n";
            strQry += "\r\n";
            strQry += "                        WHERE ETC.COMPANY_NO = @COMPANY_NO\r\n";
            strQry += "                        AND ETC.EMPLOYEE_NO = @EMPLOYEE_NO\r\n";
            strQry += "                        AND ETC.PAY_CATEGORY_NO = @PAY_CATEGORY_NO\r\n";
            strQry += "                        AND ETC.BREAK_DATE = @I_BREAK_DATE\r\n";
            strQry += "\r\n";
            strQry += "                        END\r\n";
            strQry += "\r\n";
            strQry += "                    ELSE\r\n";
            strQry += "\r\n";
            strQry += "                        BEGIN\r\n";
            strQry += "\r\n";
            strQry += "                        PRINT 'NOT Within 24 Hours'\r\n";
            strQry += "\r\n";
            strQry += "                        INSERT INTO EMPLOYEE_TIME_ATTEND_BREAK_CURRENT\r\n";
            strQry += "                       (COMPANY_NO\r\n";
            strQry += "                       ,EMPLOYEE_NO\r\n";
            strQry += "                       ,PAY_CATEGORY_NO\r\n";
            strQry += "                       ,BREAK_DATE\r\n";
            strQry += "                       ,BREAK_SEQ\r\n";
            strQry += "                       ,BREAK_TIME_OUT_MINUTES\r\n";
            strQry += "                       ,CLOCKED_TIME_OUT_MINUTES)\r\n";
            strQry += "\r\n";
            strQry += "                        SELECT\r\n";
            strQry += "                        @COMPANY_NO\r\n";
            strQry += "                       ,@EMPLOYEE_NO\r\n";
            strQry += "                       ,@PAY_CATEGORY_NO\r\n";
            strQry += "                       ,@I_BREAK_DATE\r\n";
            strQry += "                       ,ISNULL(MAX(ETC.BREAK_SEQ),0) + 1\r\n";
            strQry += "                       ,@I_BREAK_TIME_MINUTES\r\n";
            strQry += "                       ,@I_BREAK_TIME_MINUTES\r\n";
            strQry += "\r\n";
            strQry += "                        FROM EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETC\r\n";
            strQry += "\r\n";
            strQry += "                        WHERE ETC.COMPANY_NO = @COMPANY_NO\r\n";
            strQry += "                        AND ETC.EMPLOYEE_NO = @EMPLOYEE_NO\r\n";
            strQry += "                        AND ETC.PAY_CATEGORY_NO = @PAY_CATEGORY_NO\r\n";
            strQry += "                        AND ETC.BREAK_DATE = @I_BREAK_DATE\r\n";
            strQry += "\r\n";
            strQry += "                        END\r\n";
            strQry += "\r\n";
            strQry += "                    END\r\n";
            strQry += "\r\n";
            strQry += "                END\r\n";
            strQry += "\r\n";
            strQry += "            END\r\n";
            strQry += "\r\n";
            strQry += "        END\r\n";
            strQry += "\r\n";
            strQry += "    END\r\n";

            return strQry;
        }

		public void Create_DataTable_Client(string parstrQry,DataSet parDataSet,string parstrSourceTable)
        {
#if (DEBUG)
            if (parstrQry.IndexOf("InteractPayrollClient_Debug.dbo") == -1)
            {
                parstrQry = parstrQry.Replace("InteractPayrollClient.dbo", "InteractPayrollClient_Debug.dbo");
            }

            if (parstrQry.IndexOf("InteractPayrollClient.INFORMATION_SCHEMA") > -1)
            {
                parstrQry = parstrQry.Replace("InteractPayrollClient", "InteractPayrollClient_Debug");
            }
#endif
            try
            {
                pvtSqlConnectionClient = new SqlConnection(pvtstrConnectionClient);

                pvtSqlCommandClient = new SqlCommand(parstrQry, pvtSqlConnectionClient);

                pvtSqlDataAdapterClient = new SqlDataAdapter(pvtSqlCommandClient);
                
                pvtSqlDataAdapterClient.Fill(parDataSet, parstrSourceTable);

                pvtSqlConnectionClient.Close();

                parDataSet.AcceptChanges();
            }
            catch(Exception e)
            {
                throw;
            }
            finally
            {
                if (pvtSqlConnectionClient != null)
                {
                    pvtSqlConnectionClient.Dispose();
                }
                if (pvtSqlCommandClient != null)
                {
                    pvtSqlCommandClient.Dispose();
                }
            }
        }

        public void Create_DataTable_Client(string parstrQry, DataSet parDataSet, string parstrSourceTable, int parintTimeOut)
        {
#if (DEBUG)
            if (parstrQry.IndexOf("InteractPayrollClient") == -1)
            {
                string strStop = "";
            }

            if (parstrQry.IndexOf("InteractPayrollClient_Debug.dbo") == -1)
            {
                parstrQry = parstrQry.Replace("InteractPayrollClient.dbo", "InteractPayrollClient_Debug.dbo");
            }
#endif
            try
            {
                pvtSqlConnectionClient = new SqlConnection(pvtstrConnectionClient + "Connection Timeout=" + parintTimeOut.ToString());

                pvtSqlCommandClient = new SqlCommand(parstrQry, pvtSqlConnectionClient);
                pvtSqlCommandClient.CommandTimeout = parintTimeOut;

                pvtSqlDataAdapterClient = new SqlDataAdapter(pvtSqlCommandClient);

                //Opens and Closes the Connection object - pvtSqlConnection
                pvtSqlDataAdapterClient.Fill(parDataSet, parstrSourceTable);

                pvtSqlConnectionClient.Close();

                parDataSet.AcceptChanges();
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (pvtSqlConnectionClient != null)
                {
                    pvtSqlConnectionClient.Dispose();
                }

                if (pvtSqlCommandClient != null)
                {
                    pvtSqlCommandClient.Dispose();
                }
            }
        }

        public void Execute_SQLCommand(string parstrQry, Int64 parInt64CompanyNo, int parintTimeOut)
		{
            try
            {
                parstrQry = parstrQry.Replace("#CompanyNo#", parInt64CompanyNo.ToString("00000"));

                pvtSqlConnection = new SqlConnection(pvtstrConnection + "Connection Timeout=" + parintTimeOut.ToString());

                pvtSqlCommand = new SqlCommand(parstrQry, pvtSqlConnection);
                pvtSqlCommand.CommandTimeout = parintTimeOut;

                pvtSqlCommand.Connection.Open();

                pvtSqlCommand.ExecuteNonQuery();

                pvtSqlConnection.Close();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (pvtSqlConnection != null)
                {
                    if (pvtSqlConnection.State == ConnectionState.Open)
                    {
                        pvtSqlConnection.Close();
                    }

                    pvtSqlConnection.Dispose();
                }

                if (pvtSqlCommand != null)
                {
                    pvtSqlCommand.Dispose();
                }
            }
        }
        
        public void Execute_SQLCommand_Restore(string parstrQry, Int64 parInt64CompanyNo, int parintTimeOut)
        {
            parstrQry = parstrQry.Replace("#CompanyNo#", parInt64CompanyNo.ToString("00000"));

            try
            {
                pvtSqlConnection = new SqlConnection(pvtstrConnectionRestore + "Connection Timeout=" + parintTimeOut.ToString());

                pvtSqlCommand = new SqlCommand(parstrQry, pvtSqlConnection);
                pvtSqlCommand.CommandTimeout = parintTimeOut;

                pvtSqlCommand.Connection.Open();

                pvtSqlCommand.ExecuteNonQuery();

                pvtSqlConnection.Close();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (pvtSqlConnection != null)
                {
                    if (pvtSqlConnection.State == ConnectionState.Open)
                    {
                        pvtSqlConnection.Close();
                    }

                    pvtSqlConnection.Dispose();
                }

                if (pvtSqlCommand != null)
                {
                    pvtSqlCommand.Dispose();
                }
            }
        }
                
        public void Execute_SQLCommand(string parstrQry, Int64 parInt64CompanyNo)
        {
            try
            {
                parstrQry = parstrQry.Replace("#CompanyNo#", parInt64CompanyNo.ToString("00000"));

                pvtSqlConnection = new SqlConnection(pvtstrConnection);

                pvtSqlCommand = new SqlCommand(parstrQry, pvtSqlConnection);

                pvtSqlCommand.Connection.Open();

                pvtSqlCommand.ExecuteNonQuery();

                pvtSqlConnection.Close();
            }
            catch(Exception ex)
            {
                throw;
            }
            finally
            {
                if (pvtSqlConnection != null)
                {
                    if (pvtSqlConnection.State == ConnectionState.Open)
                    {
                        pvtSqlConnection.Close();
                    }

                    pvtSqlConnection.Dispose();
                }

                if (pvtSqlCommand != null)
                {
                    pvtSqlCommand.Dispose();
                }
            }
        }

        public void Execute_SQLCommand_Client(string parstrQry)
        {
#if (DEBUG)
            if (parstrQry.IndexOf("InteractPayrollClient") == -1)
            {
                string strStop = "";
            }

            if (parstrQry.IndexOf("InteractPayrollClient_Debug.dbo") == -1)
            {
                parstrQry = parstrQry.Replace("InteractPayrollClient.dbo", "InteractPayrollClient_Debug.dbo");
            }
#endif
            try
            {
                pvtSqlConnectionClient = new SqlConnection(pvtstrConnectionClient);

                pvtSqlCommandClient = new SqlCommand(parstrQry, pvtSqlConnectionClient);

                pvtSqlCommandClient.Connection.Open();

                pvtSqlCommandClient.ExecuteNonQuery();

                pvtSqlConnectionClient.Close();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (pvtSqlConnectionClient != null)
                {
                    if (pvtSqlConnectionClient.State == ConnectionState.Open)
                    {
                        pvtSqlConnectionClient.Close();
                    }

                    pvtSqlConnectionClient.Dispose();
                }

                if (pvtSqlCommandClient != null)
                {
                    pvtSqlCommandClient.Dispose();
                }
            }
        }

        public void Execute_SQLCommand_Client(string parstrQry,int parintTimeOut)
        {
#if (DEBUG)
            if (parstrQry.IndexOf("InteractPayrollClient") == -1)
            {
                string strStop = "";
            }

            if (parstrQry.IndexOf("InteractPayrollClient_Debug.dbo") == -1)
            {
                parstrQry = parstrQry.Replace("InteractPayrollClient.dbo", "InteractPayrollClient_Debug.dbo");
            }
#endif
            try
            {
                pvtSqlConnectionClient = new SqlConnection(pvtstrConnectionClient + "Connection Timeout=" + parintTimeOut.ToString());

                pvtSqlCommandClient = new SqlCommand(parstrQry, pvtSqlConnectionClient);
                pvtSqlCommandClient.CommandTimeout = parintTimeOut;

                pvtSqlCommandClient.Connection.Open();

                pvtSqlCommandClient.ExecuteNonQuery();

                pvtSqlConnectionClient.Close();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (pvtSqlConnectionClient != null)
                {
                    if (pvtSqlConnectionClient.State == ConnectionState.Open)
                    {
                        pvtSqlConnectionClient.Close();
                    }

                    pvtSqlConnectionClient.Dispose();
                }

                if (pvtSqlCommandClient != null)
                {
                    pvtSqlCommandClient.Dispose();
                }
            }
        }


        public void Execute_SQLCommand_Restore_Client(string parstrQry, int parintTimeOut)
        {
#if (DEBUG)
            if (parstrQry.IndexOf("InteractPayrollClient") == -1)
            {
                string strStop = "";
            }

            if (parstrQry.IndexOf("InteractPayrollClient_Debug.dbo") == -1)
            {
                parstrQry = parstrQry.Replace("InteractPayrollClient.dbo", "InteractPayrollClient_Debug.dbo");
            }
#endif
            try
            {
                //NB pvtstrConnectionClientRestore is Used (Database=Master)
                pvtSqlConnectionClient = new SqlConnection(pvtstrConnectionClientRestore + "Connection Timeout=" + parintTimeOut.ToString());

                pvtSqlCommandClient = new SqlCommand(parstrQry, pvtSqlConnectionClient);
                pvtSqlCommandClient.CommandTimeout = parintTimeOut;

                pvtSqlCommandClient.Connection.Open();

                pvtSqlCommandClient.ExecuteNonQuery();

                pvtSqlConnectionClient.Close();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (pvtSqlConnectionClient != null)
                {
                    if (pvtSqlConnectionClient.State == ConnectionState.Open)
                    {
                        pvtSqlConnectionClient.Close();
                    }

                    pvtSqlConnectionClient.Dispose();
                }

                if (pvtSqlCommandClient != null)
                {
                    pvtSqlCommandClient.Dispose();
                }
            }
        }
        
        public void Execute_SQLCommand(string parstrQry, byte[] bytBlob, string strFieldName)
        {
            try
            {
                pvtSqlConnection = new SqlConnection(pvtstrConnection);

                pvtSqlCommand = new SqlCommand(parstrQry, pvtSqlConnection);

                pvtSqlParameter = new SqlParameter(strFieldName, System.Data.SqlDbType.Binary, bytBlob.Length, ParameterDirection.Input, false,
                    0, 0, null, DataRowVersion.Current, bytBlob);
                pvtSqlCommand.Parameters.Add(pvtSqlParameter);

                //pvtSqlConnection.Open();
                pvtSqlCommand.Connection.Open();

                //NB This Must be Here
                pvtSqlCommand.ExecuteNonQuery();

                pvtSqlConnection.Close();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (pvtSqlConnection != null)
                {
                    if (pvtSqlConnection.State == ConnectionState.Open)
                    {
                        pvtSqlConnection.Close();
                    }

                    pvtSqlConnection.Dispose();
                }

                if (pvtSqlCommand != null)
                {
                    pvtSqlCommand.Dispose();
                }
            }
        }


        public void Execute_SQLCommand(string parstrQry, byte[] bytBlob, string strFieldName, Int64 parInt64CompanyNo)
        {
            try
            {
                parstrQry = parstrQry.Replace("#CompanyNo#", parInt64CompanyNo.ToString("00000"));
                
                pvtSqlConnection = new SqlConnection(pvtstrConnection);

                pvtSqlCommand = new SqlCommand(parstrQry, pvtSqlConnection);

                pvtSqlParameter = new SqlParameter(strFieldName, System.Data.SqlDbType.Binary, bytBlob.Length, ParameterDirection.Input, false,
                    0, 0, null, DataRowVersion.Current, bytBlob);
                pvtSqlCommand.Parameters.Add(pvtSqlParameter);

                //pvtSqlConnection.Open();
                pvtSqlCommand.Connection.Open();

                //NB This Must be Here
                pvtSqlCommand.ExecuteNonQuery();

                pvtSqlConnection.Close();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (pvtSqlConnection != null)
                {
                    if (pvtSqlConnection.State == ConnectionState.Open)
                    {
                        pvtSqlConnection.Close();
                    }

                    pvtSqlConnection.Dispose();
                }

                if (pvtSqlCommand != null)
                {
                    pvtSqlCommand.Dispose();
                }
            }
        }




        public void Execute_SQLCommand_Client(string parstrQry,byte[] bytBlob,string strFieldName)
		{
#if (DEBUG)
            if (parstrQry.IndexOf("InteractPayrollClient") == -1)
            {
                string strStop = "";
            }

            if (parstrQry.IndexOf("InteractPayrollClient_Debug.dbo") == -1)
            {
                parstrQry = parstrQry.Replace("InteractPayrollClient.dbo", "InteractPayrollClient_Debug.dbo");
            }
#endif
            try
            {
                pvtSqlConnectionClient = new SqlConnection(pvtstrConnectionClient);

                pvtSqlCommandClient = new SqlCommand(parstrQry, pvtSqlConnectionClient);

                pvtSqlParameterClient = new SqlParameter(strFieldName, System.Data.SqlDbType.Binary, bytBlob.Length, ParameterDirection.Input, false,
                    0, 0, null, DataRowVersion.Current, bytBlob);
                pvtSqlCommandClient.Parameters.Add(pvtSqlParameterClient);

                pvtSqlCommandClient.Connection.Open();

                //NB This Must be Here
                pvtSqlCommandClient.ExecuteNonQuery();

                pvtSqlConnectionClient.Close();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (pvtSqlConnectionClient != null)
                {
                    if (pvtSqlConnectionClient.State == ConnectionState.Open)
                    {
                        pvtSqlConnectionClient.Close();
                    }

                    pvtSqlConnectionClient.Dispose();
                }

                if (pvtSqlCommandClient != null)
                {
                    pvtSqlCommandClient.Dispose();
                }
            }
        }

        public void Execute_SQLCommand_Transaction(string[] parstrQry, Int64 parInt64CompanyNo)
		{
            int intCount = 0;
#if (DEBUG)
         	try
            {
#endif
                pvtSqlConnection = new SqlConnection(pvtstrConnection);
				pvtSqlConnection.Open();

				pvtSqlCommand = pvtSqlConnection.CreateCommand();
				pvtSqlTransaction = pvtSqlConnection.BeginTransaction();
				
				pvtSqlCommand.Connection = pvtSqlConnection;
				pvtSqlCommand.Transaction = pvtSqlTransaction;

				for (intCount = 0;intCount < parstrQry.Length;intCount++)
				{
					if (parstrQry[intCount] != null)
					{
						if (parstrQry[intCount] != "")
						{
                            parstrQry[intCount] = parstrQry[intCount].Replace("#CompanyNo#", parInt64CompanyNo.ToString("00000"));
							pvtSqlCommand.CommandText = parstrQry[intCount];
							pvtSqlCommand.ExecuteNonQuery();
						}
					}
				}
				
				pvtSqlTransaction.Commit();
				pvtSqlConnection.Close();
#if (DEBUG)
			}
			catch(Exception e)
			{
				Exception ex = e;
				pvtSqlCommand.CommandText = pvtSqlCommand.CommandText;
				pvtSqlTransaction.Rollback();
				pvtSqlConnection.Close();
				throw(ex);
            }
            finally
            {
                if (pvtSqlConnection != null)
                {
                    if (pvtSqlConnection.State == ConnectionState.Open)
                    {
                        pvtSqlConnection.Close();
                    }

                    pvtSqlConnection.Dispose();
                }

                if (pvtSqlCommand != null)
                {
                    pvtSqlCommand.Dispose();
                }
            }
#endif
        }
        
        public void Execute_SQLCommand_Transaction_Client(string[] parstrQry)
		{	
#if (DEBUG)
			try
            {
#endif
                pvtSqlConnectionClient = new SqlConnection(pvtstrConnectionClient);
				pvtSqlConnectionClient.Open();

				pvtSqlCommandClient = pvtSqlConnectionClient.CreateCommand();
				pvtSqlTransactionClient = pvtSqlConnectionClient.BeginTransaction();
				
				pvtSqlCommandClient.Connection = pvtSqlConnectionClient;
				pvtSqlCommandClient.Transaction = pvtSqlTransactionClient;

				for (int intCount = 0;intCount < parstrQry.Length;intCount++)
				{
					if (parstrQry[intCount] != null)
					{
#if (DEBUG)
                        if (parstrQry[intCount].IndexOf("InteractPayrollClient_Debug") == -1)
                        {
                            parstrQry[intCount] = parstrQry[intCount].Replace("InteractPayrollClient", "InteractPayrollClient_Debug");
                        }
#endif
						if (parstrQry[intCount] != "")
						{
							pvtSqlCommandClient.CommandText = parstrQry[intCount];
							pvtSqlCommandClient.ExecuteNonQuery();
						}
					}
				}
				
				pvtSqlTransactionClient.Commit();

				pvtSqlConnectionClient.Close();
#if (DEBUG)
            }
			catch(Exception e)
			{
				Exception ex = new Exception();
				ex = e;
				pvtSqlTransactionClient.Rollback();
				pvtSqlConnectionClient.Close();

				throw(ex);
			}
            finally
            {
                if (pvtSqlConnectionClient != null)
                {
                    if (pvtSqlConnectionClient.State == ConnectionState.Open)
                    {
                        pvtSqlConnectionClient.Close();
                    }

                    pvtSqlConnectionClient.Dispose();
                }

                if (pvtSqlCommandClient != null)
                {
                    pvtSqlCommandClient.Dispose();
                }
            }
#endif
        }
	}
}
