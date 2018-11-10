using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Data;

namespace InteractPayroll
{
    public class busTimeSheetDynamicUpload
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        private byte[] _salt = Encoding.ASCII.GetBytes("ErrolLeRoux");
        string sharedSecret = "Interact";

        public busTimeSheetDynamicUpload()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Download_Files_Details()
        {
            DataSet ReturnDataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" 1 AS ORDER_NO");
            strQry.AppendLine(",'P' AS FILE_LAYER_IND");
            strQry.AppendLine(",FDD.FILE_NAME");
            strQry.AppendLine(",FDD.FILE_SIZE");
            strQry.AppendLine(",FDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FDD.FILE_VERSION_NO");
            strQry.AppendLine(",FDD.FILE_CRC_VALUE");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FDD.PROJECT_VERSION = FDC.PROJECT_VERSION");
            strQry.AppendLine(" AND FDD.PROGRAM_ID = FDC.PROGRAM_ID");
            strQry.AppendLine(" AND FDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FDD.PROJECT_VERSION = 'Current'");
            strQry.AppendLine(" AND FDD.FILE_NAME = 'URLConfig.txt'");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FDD.FILE_NAME");
            strQry.AppendLine(",FDD.FILE_SIZE");
            strQry.AppendLine(",FDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FDD.FILE_VERSION_NO");
            strQry.AppendLine(",FDD.FILE_CRC_VALUE");

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" 2 AS ORDER_NO");
            strQry.AppendLine(",'P' AS FILE_LAYER_IND");

            strQry.AppendLine(",FDD.FILE_NAME");
            strQry.AppendLine(",FDD.FILE_SIZE");
            strQry.AppendLine(",FDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FDD.FILE_VERSION_NO");
            strQry.AppendLine(",FDD.FILE_CRC_VALUE");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FDD.PROJECT_VERSION = FDC.PROJECT_VERSION");
            strQry.AppendLine(" AND FDD.PROGRAM_ID = FDC.PROGRAM_ID");
            strQry.AppendLine(" AND FDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FDD.PROJECT_VERSION = 'Current'");
            strQry.AppendLine(" AND FDD.FILE_NAME IN ('clsISUtilities.dll','PasswordChange.dll','DownloadFiles.dll')");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FDD.FILE_NAME");
            strQry.AppendLine(",FDD.FILE_SIZE");
            strQry.AppendLine(",FDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FDD.FILE_VERSION_NO");
            strQry.AppendLine(",FDD.FILE_CRC_VALUE");

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" 3 AS ORDER_NO");
            strQry.AppendLine(",FCDD.FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FCDD.FILE_CRC_VALUE");

            strQry.AppendLine(",MAX(FCDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS FCDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS FCDC");
            strQry.AppendLine(" ON FCDD.FILE_LAYER_IND = FCDC.FILE_LAYER_IND");
            strQry.AppendLine(" AND FCDD.FILE_NAME = FCDC.FILE_NAME");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FCDD.FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FCDD.FILE_CRC_VALUE");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 1");
            // 2 = FCDD.FILE_LAYER_IND - Get Server dlls First
            strQry.AppendLine(",2 DESC");
            strQry.AppendLine(",3");
           
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), ReturnDataSet, "Files", -1);
           
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(ReturnDataSet);
            ReturnDataSet.Dispose();
            ReturnDataSet = null;

            return bytCompress;
        }

        public byte[] Get_Download_Files_Details_New(byte[] parbyteArrayDataSet)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteArrayDataSet);

            StringBuilder strQryFilter = new StringBuilder();

            if (parDataSet.Tables["PayCategoryDownload"].Rows.Count == 0)
            {
                //Will NOT Find Link
                strQryFilter.AppendLine(" AND (FDPC.COMPANY_NO = -9999 AND FDPC.PAY_CATEGORY_NO = -9999)");
            }
            else
            {
                for (int intRow = 0; intRow < parDataSet.Tables["PayCategoryDownload"].Rows.Count; intRow++)
                {
                    if (intRow == 0)
                    {
                        if (parDataSet.Tables["PayCategoryDownload"].Rows.Count == 1)
                        {
                            strQryFilter.AppendLine(" AND (FDPC.COMPANY_NO = " + parDataSet.Tables["PayCategoryDownload"].Rows[intRow]["COMPANY_NO"].ToString() + " AND FDPC.PAY_CATEGORY_NO = " + parDataSet.Tables["PayCategoryDownload"].Rows[intRow]["PAY_CATEGORY_NO"].ToString() + ")");

                        }
                        else
                        {
                            strQryFilter.AppendLine(" AND ((FDPC.COMPANY_NO = " + parDataSet.Tables["PayCategoryDownload"].Rows[intRow]["COMPANY_NO"].ToString() + " AND FDPC.PAY_CATEGORY_NO = " + parDataSet.Tables["PayCategoryDownload"].Rows[intRow]["PAY_CATEGORY_NO"].ToString() + ")");
                        }
                    }
                    else
                    {
                        if (intRow == parDataSet.Tables["PayCategoryDownload"].Rows.Count - 1)
                        {
                            strQryFilter.AppendLine(" OR (FDPC.COMPANY_NO = " + parDataSet.Tables["PayCategoryDownload"].Rows[intRow]["COMPANY_NO"].ToString() + " AND FDPC.PAY_CATEGORY_NO = " + parDataSet.Tables["PayCategoryDownload"].Rows[intRow]["PAY_CATEGORY_NO"].ToString() + "))");
                        }
                        else
                        {
                            strQryFilter.AppendLine(" OR (FDPC.COMPANY_NO = " + parDataSet.Tables["PayCategoryDownload"].Rows[intRow]["COMPANY_NO"].ToString() + " AND FDPC.PAY_CATEGORY_NO = " + parDataSet.Tables["PayCategoryDownload"].Rows[intRow]["PAY_CATEGORY_NO"].ToString() + ")");
                        }
                    }
                }
            }
            
            DataSet ReturnDataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FDD.PROJECT_VERSION");
            strQry.AppendLine(",1 AS ORDER_NO");
            strQry.AppendLine(",'P' AS FILE_LAYER_IND");
            strQry.AppendLine(",FDD.FILE_NAME");
            strQry.AppendLine(",FDD.FILE_SIZE");
            strQry.AppendLine(",FDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FDD.FILE_VERSION_NO");
            strQry.AppendLine(",FDD.FILE_CRC_VALUE");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_PAY_CATEGORIES FDPC");
            strQry.AppendLine(" ON FDD.PROJECT_VERSION = FDPC.PROJECT_VERSION");

            strQry.Append(strQryFilter);

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FDD.PROJECT_VERSION = FDC.PROJECT_VERSION");
            strQry.AppendLine(" AND FDD.PROGRAM_ID = FDC.PROGRAM_ID");
            strQry.AppendLine(" AND FDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FDD.PROJECT_VERSION = 'Beta'");
            strQry.AppendLine(" AND FDD.FILE_NAME = 'URLConfig.txt'");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FDD.PROJECT_VERSION");
            strQry.AppendLine(",FDD.FILE_NAME");
            strQry.AppendLine(",FDD.FILE_SIZE");
            strQry.AppendLine(",FDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FDD.FILE_VERSION_NO");
            strQry.AppendLine(",FDD.FILE_CRC_VALUE");

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FDD.PROJECT_VERSION");
            strQry.AppendLine(",1 AS ORDER_NO");
            strQry.AppendLine(",'P' AS FILE_LAYER_IND");
            strQry.AppendLine(",FDD.FILE_NAME");
            strQry.AppendLine(",FDD.FILE_SIZE");
            strQry.AppendLine(",FDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FDD.FILE_VERSION_NO");
            strQry.AppendLine(",FDD.FILE_CRC_VALUE");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FDD");

            //2018-09-22 - Start 
            strQry.AppendLine(" LEFT JOIN ");
            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" FDD.FILE_NAME");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_PAY_CATEGORIES FDPC");
            strQry.AppendLine(" ON FDD.PROJECT_VERSION = FDPC.PROJECT_VERSION");
            strQry.Append(strQryFilter);

            strQry.AppendLine(" WHERE FDD.PROJECT_VERSION = 'Beta'");
            strQry.AppendLine(" AND FDD.FILE_NAME = 'URLConfig.txt') AS BETA_TABLE");
            strQry.AppendLine(" ON FDD.FILE_NAME = BETA_TABLE.FILE_NAME");
            //2018-09-22 - End 

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FDD.PROJECT_VERSION = FDC.PROJECT_VERSION");
            strQry.AppendLine(" AND FDD.PROGRAM_ID = FDC.PROGRAM_ID");
            strQry.AppendLine(" AND FDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FDD.PROJECT_VERSION = 'Current'");
            strQry.AppendLine(" AND FDD.FILE_NAME = 'URLConfig.txt'");

            //2018-09-22 - Not in Beta
            strQry.AppendLine(" AND BETA_TABLE.FILE_NAME IS NULL");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FDD.PROJECT_VERSION");
            strQry.AppendLine(",FDD.FILE_NAME");
            strQry.AppendLine(",FDD.FILE_SIZE");
            strQry.AppendLine(",FDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FDD.FILE_VERSION_NO");
            strQry.AppendLine(",FDD.FILE_CRC_VALUE");

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FDD.PROJECT_VERSION");
            strQry.AppendLine(",2 AS ORDER_NO");
            strQry.AppendLine(",'P' AS FILE_LAYER_IND");

            strQry.AppendLine(",FDD.FILE_NAME");
            strQry.AppendLine(",FDD.FILE_SIZE");
            strQry.AppendLine(",FDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FDD.FILE_VERSION_NO");
            strQry.AppendLine(",FDD.FILE_CRC_VALUE");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_PAY_CATEGORIES FDPC");
            strQry.AppendLine(" ON FDD.PROJECT_VERSION = FDPC.PROJECT_VERSION");

            strQry.Append(strQryFilter);

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FDD.PROJECT_VERSION = FDC.PROJECT_VERSION");
            strQry.AppendLine(" AND FDD.PROGRAM_ID = FDC.PROGRAM_ID");
            strQry.AppendLine(" AND FDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FDD.PROJECT_VERSION = 'Beta'");
            strQry.AppendLine(" AND FDD.FILE_NAME IN ('clsISUtilities.dll','PasswordChange.dll','DownloadFiles.dll')");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FDD.PROJECT_VERSION");
            strQry.AppendLine(",FDD.FILE_NAME");
            strQry.AppendLine(",FDD.FILE_SIZE");
            strQry.AppendLine(",FDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FDD.FILE_VERSION_NO");
            strQry.AppendLine(",FDD.FILE_CRC_VALUE");

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FDD.PROJECT_VERSION");
            strQry.AppendLine(",2 AS ORDER_NO");
            strQry.AppendLine(",'P' AS FILE_LAYER_IND");

            strQry.AppendLine(",FDD.FILE_NAME");
            strQry.AppendLine(",FDD.FILE_SIZE");
            strQry.AppendLine(",FDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FDD.FILE_VERSION_NO");
            strQry.AppendLine(",FDD.FILE_CRC_VALUE");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FDD");

            //2018-09-22 - Start 
            strQry.AppendLine(" LEFT JOIN ");
            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" FDD.FILE_NAME");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_PAY_CATEGORIES FDPC");
            strQry.AppendLine(" ON FDD.PROJECT_VERSION = FDPC.PROJECT_VERSION");
            strQry.Append(strQryFilter);

            strQry.AppendLine(" WHERE FDD.PROJECT_VERSION = 'Beta'");
            strQry.AppendLine(" AND FDD.FILE_NAME IN ('clsISUtilities.dll','PasswordChange.dll','DownloadFiles.dll')) AS BETA_TABLE");
            strQry.AppendLine(" ON FDD.FILE_NAME = BETA_TABLE.FILE_NAME");
            //2018-09-22 - End 

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FDD.PROJECT_VERSION = FDC.PROJECT_VERSION");
            strQry.AppendLine(" AND FDD.PROGRAM_ID = FDC.PROGRAM_ID");
            strQry.AppendLine(" AND FDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FDD.PROJECT_VERSION = 'Current'");
            strQry.AppendLine(" AND FDD.FILE_NAME IN ('clsISUtilities.dll','PasswordChange.dll','DownloadFiles.dll')");

            //2018-09-22 - Not in Beta
            strQry.AppendLine(" AND BETA_TABLE.FILE_NAME IS NULL");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FDD.PROJECT_VERSION");
            strQry.AppendLine(",FDD.FILE_NAME");
            strQry.AppendLine(",FDD.FILE_SIZE");
            strQry.AppendLine(",FDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FDD.FILE_VERSION_NO");
            strQry.AppendLine(",FDD.FILE_CRC_VALUE");

            strQry.AppendLine(" UNION ");
            
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FCDD.PROJECT_VERSION");
            strQry.AppendLine(",3 AS ORDER_NO");
            strQry.AppendLine(",FCDD.FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FCDD.FILE_CRC_VALUE");

            strQry.AppendLine(",MAX(FCDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS FCDD");
            
            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_PAY_CATEGORIES FDPC");
            strQry.AppendLine(" ON FCDD.PROJECT_VERSION = FDPC.PROJECT_VERSION");

            strQry.Append(strQryFilter);
            
            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS FCDC");
            strQry.AppendLine(" ON FCDD.FILE_LAYER_IND = FCDC.FILE_LAYER_IND");
            strQry.AppendLine(" AND FCDD.FILE_NAME = FCDC.FILE_NAME");

            strQry.AppendLine(" WHERE FCDD.PROJECT_VERSION = 'Beta'");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FCDD.PROJECT_VERSION");
            strQry.AppendLine(",FCDD.FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FCDD.FILE_CRC_VALUE");
            
            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FCDD.PROJECT_VERSION");
            strQry.AppendLine(",3 AS ORDER_NO");
            strQry.AppendLine(",FCDD.FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FCDD.FILE_CRC_VALUE");

            strQry.AppendLine(",MAX(FCDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS FCDD");
            
            //2018-09-22 - Start 
            strQry.AppendLine(" LEFT JOIN ");
            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" FCDD.FILE_NAME");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS FCDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_PAY_CATEGORIES FDPC");
            strQry.AppendLine(" ON FCDD.PROJECT_VERSION = FDPC.PROJECT_VERSION");
            strQry.Append(strQryFilter);

            strQry.AppendLine(" WHERE FCDD.PROJECT_VERSION = 'Beta') AS BETA_TABLE");
            strQry.AppendLine(" ON FCDD.FILE_NAME = BETA_TABLE.FILE_NAME");
            //2018-09-22 - End 
            
            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS FCDC");
            strQry.AppendLine(" ON FCDD.FILE_LAYER_IND = FCDC.FILE_LAYER_IND");
            strQry.AppendLine(" AND FCDD.FILE_NAME = FCDC.FILE_NAME");

            strQry.AppendLine(" WHERE FCDD.PROJECT_VERSION = 'Current'");

            //2018-09-22 - Not in Beta
            strQry.AppendLine(" AND BETA_TABLE.FILE_NAME IS NULL");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FCDD.PROJECT_VERSION");
            strQry.AppendLine(",FCDD.FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FCDD.FILE_CRC_VALUE");
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 1");
            strQry.AppendLine(",2");
            //3 = FCDD.FILE_LAYER_IND - Get Server dlls First
            strQry.AppendLine(",3 DESC");
            strQry.AppendLine(",4");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), ReturnDataSet, "Files", -1);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(ReturnDataSet);
            ReturnDataSet.Dispose();
            ReturnDataSet = null;

            return bytCompress;
        }

        public byte[] Get_File_Chunk_New(string parstrProjectVersion, string parstrLayerInd, string parstrFileName, int parintFileChunkNo)
        {
            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FILE_CHUNK");

            if (parstrFileName == "URLConfig.txt"
                | parstrFileName == "clsISUtilities.dll"
                | parstrFileName == "PasswordChange.dll"
                | parstrFileName == "DownloadFiles.dll")
            {
                strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS");
                strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
            }
            else
            {
                strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS");

                strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                strQry.AppendLine(" AND FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerInd));
                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
            }

            strQry.AppendLine(" AND FILE_CHUNK_NO = " + parintFileChunkNo);

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", -1);

            return (byte[])DataSet.Tables["Temp"].Rows[0]["FILE_CHUNK"];
        }
        
        public byte[] Get_File_Chunk(string parstrLayerInd, string parstrFileName, int parintFileChunkNo)
        {
            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FILE_CHUNK");
          
            if (parstrFileName == "URLConfig.txt"
                | parstrFileName == "clsISUtilities.dll"
                | parstrFileName == "PasswordChange.dll"
                | parstrFileName == "DownloadFiles.dll")
            {
                strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS");
                strQry.AppendLine(" WHERE PROJECT_VERSION = 'Current'");
                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
            }
            else
            {
                strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS");

                strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerInd));
                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
            }
           
            strQry.AppendLine(" AND FILE_CHUNK_NO = " + parintFileChunkNo);
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", -1);

            return (byte[])DataSet.Tables["Temp"].Rows[0]["FILE_CHUNK"];
        }

        public int Set_UploadDateTime_For_Company(Int64 parInt64CompanyNo, byte[] parbyteArrayDataSet)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteArrayDataSet);
            DataSet DataSet = new DataSet();
            bool blnChangeDateTime = false;
            StringBuilder strQry = new StringBuilder();
            int intRecordsUpdated = parDataSet.Tables["CostCentre"].Rows.Count;

            for (int intRow = 0; intRow < parDataSet.Tables["CostCentre"].Rows.Count; intRow++)
            {
                if (intRow == 0)
                {
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" GETDATE() AS CURRENTDATE");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY ");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

                    if (DataSet.Tables["Temp"].Rows.Count > 0)
                    {
                        //Allow Clocks to be Up To 10 Minutes Out
                        DateTime FromDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 50, 00);
                        DateTime ToDateTime = new DateTime(DateTime.Now.AddDays(1).Year, DateTime.Now.AddDays(1).Month, DateTime.Now.AddDays(1).Day, 0, 0, 0);

                        if (Convert.ToDateTime(DataSet.Tables["Temp"].Rows[0]["CURRENTDATE"]) > FromDateTime
                        && Convert.ToDateTime(DataSet.Tables["Temp"].Rows[0]["CURRENTDATE"]) < ToDateTime)
                        {
                            blnChangeDateTime = true;
                        }
                    }
                }

                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY");

                if (blnChangeDateTime == true)
                {
                    //Set Date to After MidNight
                    strQry.AppendLine(" SET LAST_UPLOAD_DATETIME = DATEADD(minute,15,GETDATE())");
                }
                else
                {
                    strQry.AppendLine(" SET LAST_UPLOAD_DATETIME = GETDATE()");
                }

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parDataSet.Tables["CostCentre"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["CostCentre"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            }

            return intRecordsUpdated;
        }

        public byte[] Upload_Timesheet_Break_For_Day(Int64 parInt64CompanyNo, string parstrURLConfigFileDateTime, string parstrUploadKey, string parstrEmployeeRunDateWageQry, string parstrEmployeeRunDateSalaryQry, string parstrEmployeeRunDateTimeAttendanceQry, byte[] parbyteArrayDataSet)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteArrayDataSet);
            DataSet ReturnDataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",NO_EDIT_IND");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");

            strQry.AppendLine(parstrEmployeeRunDateWageQry.Replace("EPC.", ""));

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), ReturnDataSet, "PayCategoryWages", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",NO_EDIT_IND");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");

            strQry.AppendLine(parstrEmployeeRunDateSalaryQry.Replace("EPC.", ""));

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), ReturnDataSet, "PayCategorySalaries", parInt64CompanyNo);

            //2013-06-24
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",NO_EDIT_IND");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");

            strQry.AppendLine(parstrEmployeeRunDateTimeAttendanceQry.Replace("EPC.", ""));

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), ReturnDataSet, "PayCategoryTimeAttendance", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",GETDATE() AS UPLOAD_DATETIME");
            strQry.AppendLine(",'' AS URLCONFIG_CHANGE_IND");

            strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND DYNAMIC_UPLOAD_KEY = '" + parstrUploadKey + "'");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), ReturnDataSet, "Company", -1);

            if (parstrURLConfigFileDateTime != "")
            {
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" FILE_LAST_UPDATED_DATE");

                strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS ");

                strQry.AppendLine(" WHERE PROJECT_VERSION = 'Current' ");
                strQry.AppendLine(" AND FILE_NAME = 'URLConfig.txt' ");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), ReturnDataSet, "FileTest", -1);

                if (ReturnDataSet.Tables["FileTest"].Rows.Count > 0)
                {
                    if (Convert.ToDateTime(ReturnDataSet.Tables["FileTest"].Rows[0]["FILE_LAST_UPDATED_DATE"]) >= Convert.ToDateTime(parstrURLConfigFileDateTime).AddSeconds(-3)
                    && Convert.ToDateTime(ReturnDataSet.Tables["FileTest"].Rows[0]["FILE_LAST_UPDATED_DATE"]) <= Convert.ToDateTime(parstrURLConfigFileDateTime).AddSeconds(3))
                    {
                    }
                    else
                    {
                        ReturnDataSet.Tables["Company"].Rows[0]["URLCONFIG_CHANGE_IND"] = "Y";
                    }
                }
            }

            if (ReturnDataSet.Tables["Company"].Rows.Count > 0)
            {
                if (ReturnDataSet.Tables["Company"].Rows[0]["URLCONFIG_CHANGE_IND"].ToString() == "")
                {
                    if (ReturnDataSet.Tables["Company"].Rows.Count > 0)
                    {
                        strQry.Clear();
                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.EMPLOYEE_NO");
                        strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",ISNULL(E.EMPLOYEE_LAST_RUNDATE,'" + DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd") + "') AS EMPLOYEE_LAST_RUNDATE ");
                        strQry.AppendLine(",PAYROLL_RUN.PAY_PERIOD_DATE");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");
                        strQry.AppendLine(parstrEmployeeRunDateWageQry);

                        strQry.AppendLine(" LEFT JOIN ");
                        strQry.AppendLine("(SELECT ");
                        strQry.AppendLine(" EPCC.EMPLOYEE_NO");
                        strQry.AppendLine(",PCPC.PAY_PERIOD_DATE");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C");
                        strQry.AppendLine(" ON EPCC.COMPANY_NO = C.COMPANY_NO ");
                        strQry.AppendLine(" AND  C.WAGE_RUN_IND = 'Y' ");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");
                        strQry.AppendLine(" ON EPCC.COMPANY_NO = PCPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = PCPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = PCPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EPCC.RUN_TYPE = PCPC.RUN_TYPE ");

                        strQry.AppendLine(" WHERE EPCC.COMPANY_NO = " + parInt64CompanyNo);
                        strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = 'W' ");
                        strQry.AppendLine(parstrEmployeeRunDateWageQry.Replace("EPC", "EPCC") + ") AS PAYROLL_RUN");

                        strQry.AppendLine(" ON E.EMPLOYEE_NO = PAYROLL_RUN.EMPLOYEE_NO ");

                        strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W' ");
                        strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                        strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.EMPLOYEE_NO");
                        strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",ISNULL(E.EMPLOYEE_LAST_RUNDATE,'" + DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd") + "') AS EMPLOYEE_LAST_RUNDATE ");
                        strQry.AppendLine(",PAYROLL_RUN.PAY_PERIOD_DATE");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");
                        strQry.AppendLine(parstrEmployeeRunDateSalaryQry);

                        strQry.AppendLine(" LEFT JOIN ");
                        strQry.AppendLine("(SELECT ");
                        strQry.AppendLine(" EPCC.EMPLOYEE_NO ");
                        strQry.AppendLine(",PCPC.PAY_PERIOD_DATE");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C");
                        strQry.AppendLine(" ON EPCC.COMPANY_NO = C.COMPANY_NO ");
                        strQry.AppendLine(" AND  C.SALARY_RUN_IND = 'Y' ");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");
                        strQry.AppendLine(" ON EPCC.COMPANY_NO = PCPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = PCPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = PCPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EPCC.RUN_TYPE = PCPC.RUN_TYPE ");

                        strQry.AppendLine(" WHERE EPCC.COMPANY_NO = " + parInt64CompanyNo);
                        strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = 'S' ");
                        strQry.AppendLine(parstrEmployeeRunDateSalaryQry.Replace("EPC", "EPCC") + ") AS PAYROLL_RUN");

                        strQry.AppendLine(" ON E.EMPLOYEE_NO = PAYROLL_RUN.EMPLOYEE_NO ");

                        strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S' ");
                        strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                        strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                        //2013-06-24
                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.EMPLOYEE_NO");
                        strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",ISNULL(E.EMPLOYEE_LAST_RUNDATE,'" + DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd") + "') AS EMPLOYEE_LAST_RUNDATE ");
                        strQry.AppendLine(",PAYROLL_RUN.PAY_PERIOD_DATE");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");
                        strQry.AppendLine(parstrEmployeeRunDateTimeAttendanceQry);

                        strQry.AppendLine(" LEFT JOIN ");
                        strQry.AppendLine("(SELECT ");
                        strQry.AppendLine(" EPCC.EMPLOYEE_NO ");
                        strQry.AppendLine(",PCPC.PAY_PERIOD_DATE");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCC");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C");
                        strQry.AppendLine(" ON EPCC.COMPANY_NO = C.COMPANY_NO ");
                        strQry.AppendLine(" AND  C.SALARY_RUN_IND = 'Y' ");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");
                        strQry.AppendLine(" ON EPCC.COMPANY_NO = PCPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCC.PAY_CATEGORY_NO = PCPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = PCPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EPCC.RUN_TYPE = PCPC.RUN_TYPE ");

                        strQry.AppendLine(" WHERE EPCC.COMPANY_NO = " + parInt64CompanyNo);
                        strQry.AppendLine(" AND EPCC.PAY_CATEGORY_TYPE = 'T' ");
                        strQry.AppendLine(parstrEmployeeRunDateTimeAttendanceQry.Replace("EPC", "EPCC") + ") AS PAYROLL_RUN");

                        strQry.AppendLine(" ON E.EMPLOYEE_NO = PAYROLL_RUN.EMPLOYEE_NO ");

                        strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T' ");
                        strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                        strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                        clsDBConnectionObjects.Create_DataTable(strQry.ToString(), ReturnDataSet, "Employee", parInt64CompanyNo);

                        DataView DataViewPayCategory;

                        for (int intRow = 0; intRow < parDataSet.Tables["TimeSheet"].Rows.Count; intRow++)
                        {
                            DataViewPayCategory = null;

                            if (parDataSet.Tables["TimeSheet"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                            {
                                DataViewPayCategory = new DataView(ReturnDataSet.Tables["PayCategoryWages"],
                                "PAY_CATEGORY_NO = " + parDataSet.Tables["TimeSheet"].Rows[intRow]["PAY_CATEGORY_NO"].ToString() + " AND NO_EDIT_IND = 'Y'",
                                "",
                                DataViewRowState.CurrentRows);
                            }
                            else
                            {
                                if (parDataSet.Tables["TimeSheet"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                                {
                                    DataViewPayCategory = new DataView(ReturnDataSet.Tables["PayCategorySalaries"],
                                    "PAY_CATEGORY_NO = " + parDataSet.Tables["TimeSheet"].Rows[intRow]["PAY_CATEGORY_NO"].ToString() + " AND NO_EDIT_IND = 'Y'",
                                    "",
                                    DataViewRowState.CurrentRows);
                                }
                                else
                                {
                                    DataViewPayCategory = new DataView(ReturnDataSet.Tables["PayCategoryTimeAttendance"],
                                    "PAY_CATEGORY_NO = " + parDataSet.Tables["TimeSheet"].Rows[intRow]["PAY_CATEGORY_NO"].ToString() + " AND NO_EDIT_IND = 'Y'",
                                    "",
                                    DataViewRowState.CurrentRows);
                                }
                            }

                            //Omly Cost Centres That Have been Setup for Dynamic Upload 
                            if (DataViewPayCategory.Count == 0)
                            {
                                continue;
                            }

                            DataView DataView = new DataView(ReturnDataSet.Tables["Employee"],
                                "EMPLOYEE_NO = " + parDataSet.Tables["TimeSheet"].Rows[intRow]["EMPLOYEE_NO"].ToString() + " AND PAY_CATEGORY_TYPE = '" + parDataSet.Tables["TimeSheet"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'",
                                "",
                                DataViewRowState.CurrentRows);

                            strQry.Clear();

                            if (DataView.Count > 0)
                            {
                                if (DataView[0]["PAY_PERIOD_DATE"] == System.DBNull.Value)
                                {
                                    if (Convert.ToDateTime(parDataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_DATE"]) > Convert.ToDateTime(DataView[0]["EMPLOYEE_LAST_RUNDATE"]))
                                    {
                                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.CLOCK_TIME");
                                    }
                                    else
                                    {
                                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.CLOCK_TIME_ERROR");
                                    }
                                }
                                else
                                {
                                    //Payroll Run in Progress
                                    if (Convert.ToDateTime(parDataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_DATE"]) > Convert.ToDateTime(DataView[0]["PAY_PERIOD_DATE"]))
                                    {
                                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.CLOCK_TIME");
                                    }
                                    else
                                    {
                                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.CLOCK_TIME_ERROR");
                                    }
                                }
                            }
                            else
                            {
                                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.CLOCK_TIME_ERROR");
                            }

                            strQry.AppendLine("(COMPANY_NO");
                            strQry.AppendLine(",EMPLOYEE_NO");
                            strQry.AppendLine(",PAY_CATEGORY_NO");
                            strQry.AppendLine(",PAY_CATEGORY_TYPE");
                            strQry.AppendLine(",TIMESHEET_DATE");
                            strQry.AppendLine(",TIMESHEET_TIME_MINUTES");
                            strQry.AppendLine(",IN_OUT_IND");
                            strQry.AppendLine(",CLOCKED_BOUNDARY_TIME_MINUTES)");

                            strQry.AppendLine(" SELECT ");

                            strQry.AppendLine(parDataSet.Tables["TimeSheet"].Rows[intRow]["COMPANY_NO"].ToString());
                            strQry.AppendLine("," + parDataSet.Tables["TimeSheet"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine("," + parDataSet.Tables["TimeSheet"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["TimeSheet"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                            strQry.AppendLine(",'" + Convert.ToDateTime(parDataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_DATE"]).ToString("yyyy-MM-dd HH:mm:ss") + "'");
                            strQry.AppendLine("," + parDataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_TIME_MINUTES"].ToString());
                            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["TimeSheet"].Rows[intRow]["IN_OUT_IND"].ToString()));
                            strQry.AppendLine("," + parDataSet.Tables["TimeSheet"].Rows[intRow]["CLOCKED_BOUNDARY_TIME_MINUTES"].ToString());

                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

                            if (DataView.Count > 0)
                            {
                                if (DataView[0]["PAY_PERIOD_DATE"] == System.DBNull.Value)
                                {
                                    if (Convert.ToDateTime(parDataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_DATE"]) > Convert.ToDateTime(DataView[0]["EMPLOYEE_LAST_RUNDATE"]))
                                    {
                                        strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.CLOCK_TIME CT");
                                    }
                                    else
                                    {
                                        strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.CLOCK_TIME_ERROR CT");
                                    }
                                }
                                else
                                {
                                    //Payroll Run in Progress
                                    if (Convert.ToDateTime(parDataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_DATE"]) > Convert.ToDateTime(DataView[0]["PAY_PERIOD_DATE"]))
                                    {
                                        strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.CLOCK_TIME CT");
                                    }
                                    else
                                    {
                                        strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.CLOCK_TIME_ERROR CT");
                                    }
                                }
                            }
                            else
                            {
                                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.CLOCK_TIME_ERROR CT");
                            }

                            strQry.AppendLine(" ON C.COMPANY_NO = CT.COMPANY_NO ");
                            strQry.AppendLine(" AND CT.EMPLOYEE_NO = " + parDataSet.Tables["TimeSheet"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine(" AND CT.PAY_CATEGORY_NO = " + parDataSet.Tables["TimeSheet"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                            strQry.AppendLine(" AND CT.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["TimeSheet"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                            strQry.AppendLine(" AND CT.TIMESHEET_DATE = '" + Convert.ToDateTime(parDataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_DATE"]).ToString("yyyy-MM-dd HH:mm:ss") + "'");
                            strQry.AppendLine(" AND CT.TIMESHEET_TIME_MINUTES = " + parDataSet.Tables["TimeSheet"].Rows[intRow]["TIMESHEET_TIME_MINUTES"].ToString());
                            strQry.AppendLine(" AND CT.IN_OUT_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["TimeSheet"].Rows[intRow]["IN_OUT_IND"].ToString()));
                            strQry.AppendLine(" AND CT.CLOCKED_BOUNDARY_TIME_MINUTES = " + parDataSet.Tables["TimeSheet"].Rows[intRow]["CLOCKED_BOUNDARY_TIME_MINUTES"].ToString());

                            strQry.AppendLine(" WHERE C.COMPANY_NO = " + parInt64CompanyNo);
                            //Record Does NOT Exist
                            strQry.AppendLine(" AND CT.COMPANY_NO IS NULL");

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                            strQry.Clear();
                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY");

                            strQry.AppendLine(" SET LAST_UPLOAD_DATETIME = GETDATE()");

                            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                            strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parDataSet.Tables["TimeSheet"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["TimeSheet"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                            strQry.Clear();
                            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
                            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
                            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                        }

                        for (int intRow = 0; intRow < parDataSet.Tables["Break"].Rows.Count; intRow++)
                        {
                            DataViewPayCategory = null;

                            if (parDataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                            {
                                DataViewPayCategory = new DataView(ReturnDataSet.Tables["PayCategoryWages"],
                                "PAY_CATEGORY_NO = " + parDataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_NO"].ToString() + " AND NO_EDIT_IND = 'Y'",
                                "",
                                DataViewRowState.CurrentRows);
                            }
                            else
                            {
                                if (parDataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                                {
                                    DataViewPayCategory = new DataView(ReturnDataSet.Tables["PayCategorySalaries"],
                                    "PAY_CATEGORY_NO = " + parDataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_NO"].ToString() + " AND NO_EDIT_IND = 'Y'",
                                    "",
                                    DataViewRowState.CurrentRows);
                                }
                                else
                                {
                                    DataViewPayCategory = new DataView(ReturnDataSet.Tables["PayCategoryTimeAttendance"],
                                    "PAY_CATEGORY_NO = " + parDataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_NO"].ToString() + " AND NO_EDIT_IND = 'Y'",
                                    "",
                                    DataViewRowState.CurrentRows);
                                }
                            }

                            //Only Cost Centres That Have been Setup for Dynamic Upload 
                            if (DataViewPayCategory.Count == 0)
                            {
                                continue;
                            }

                            DataView DataView = new DataView(ReturnDataSet.Tables["Employee"],
                            "EMPLOYEE_NO = " + parDataSet.Tables["Break"].Rows[intRow]["EMPLOYEE_NO"].ToString() + " AND PAY_CATEGORY_TYPE = '" + parDataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'",
                                "",
                            DataViewRowState.CurrentRows);

                            strQry.Clear();

                            if (DataView.Count > 0)
                            {
                                if (DataView[0]["PAY_PERIOD_DATE"] == System.DBNull.Value)
                                {
                                    if (Convert.ToDateTime(parDataSet.Tables["Break"].Rows[intRow]["BREAK_DATE"]) > Convert.ToDateTime(DataView[0]["EMPLOYEE_LAST_RUNDATE"]))
                                    {
                                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.CLOCK_TIME_BREAK");
                                    }
                                    else
                                    {
                                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.CLOCK_TIME_BREAK_ERROR");
                                    }
                                }
                                else
                                {
                                    //Payroll Run in Progress
                                    if (Convert.ToDateTime(parDataSet.Tables["Break"].Rows[intRow]["BREAK_DATE"]) > Convert.ToDateTime(DataView[0]["PAY_PERIOD_DATE"]))
                                    {
                                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.CLOCK_TIME_BREAK");
                                    }
                                    else
                                    {
                                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.CLOCK_TIME_BREAK_ERROR");
                                    }
                                }
                            }
                            else
                            {
                                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.CLOCK_TIME_BREAK_ERROR");
                            }

                            strQry.AppendLine("(COMPANY_NO");
                            strQry.AppendLine(",EMPLOYEE_NO");
                            strQry.AppendLine(",PAY_CATEGORY_NO");
                            strQry.AppendLine(",PAY_CATEGORY_TYPE");
                            strQry.AppendLine(",BREAK_DATE");
                            strQry.AppendLine(",BREAK_TIME_MINUTES");
                            strQry.AppendLine(",IN_OUT_IND)");

                            strQry.AppendLine(" SELECT ");

                            strQry.AppendLine(parDataSet.Tables["Break"].Rows[intRow]["COMPANY_NO"].ToString());
                            strQry.AppendLine("," + parDataSet.Tables["Break"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine("," + parDataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                            strQry.AppendLine(",'" + Convert.ToDateTime(parDataSet.Tables["Break"].Rows[intRow]["BREAK_DATE"]).ToString("yyyy-MM-dd HH:mm:ss") + "'");

                            strQry.AppendLine("," + parDataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_MINUTES"].ToString());

                            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Break"].Rows[intRow]["IN_OUT_IND"].ToString()));

                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

                            if (DataView.Count > 0)
                            {
                                if (DataView[0]["PAY_PERIOD_DATE"] == System.DBNull.Value)
                                {
                                    if (Convert.ToDateTime(parDataSet.Tables["Break"].Rows[intRow]["BREAK_DATE"]) > Convert.ToDateTime(DataView[0]["EMPLOYEE_LAST_RUNDATE"]))
                                    {
                                        strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.CLOCK_TIME_BREAK CTB");
                                    }
                                    else
                                    {
                                        strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.CLOCK_TIME_BREAK_ERROR CTB");
                                    }
                                }
                                else
                                {
                                    //Payroll Run in Progress
                                    if (Convert.ToDateTime(parDataSet.Tables["Break"].Rows[intRow]["BREAK_DATE"]) > Convert.ToDateTime(DataView[0]["PAY_PERIOD_DATE"]))
                                    {
                                        strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.CLOCK_TIME_BREAK CTB");
                                    }
                                    else
                                    {
                                        strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.CLOCK_TIME_BREAK_ERROR CTB");
                                    }
                                }
                            }
                            else
                            {
                                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.CLOCK_TIME_BREAK_ERROR CTB");
                            }

                            strQry.AppendLine(" ON C.COMPANY_NO = CTB.COMPANY_NO ");
                            strQry.AppendLine(" AND CTB.EMPLOYEE_NO = " + parDataSet.Tables["Break"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine(" AND CTB.PAY_CATEGORY_NO = " + parDataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                            strQry.AppendLine(" AND CTB.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                            strQry.AppendLine(" AND CTB.BREAK_DATE = '" + Convert.ToDateTime(parDataSet.Tables["Break"].Rows[intRow]["BREAK_DATE"]).ToString("yyyy-MM-dd HH:mm:ss") + "'");

                            strQry.AppendLine(" AND CTB.BREAK_TIME_MINUTES = " + parDataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_MINUTES"].ToString());
                            strQry.AppendLine(" AND CTB.IN_OUT_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Break"].Rows[intRow]["IN_OUT_IND"].ToString()));

                            strQry.AppendLine(" WHERE C.COMPANY_NO = " + parInt64CompanyNo);
                            //Record Does NOT Exist
                            strQry.AppendLine(" AND CTB.COMPANY_NO IS NULL");

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                            strQry.Clear();
                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY");

                            strQry.AppendLine(" SET LAST_UPLOAD_DATETIME = GETDATE()");

                            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                            strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parDataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                            strQry.Clear();
                            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
                            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
                            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                        }
                    }
                }
            }
           
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(ReturnDataSet);
            ReturnDataSet.Dispose();
            ReturnDataSet = null;

            return bytCompress;
        }

        private string DecryptStringAES(string cipherText)
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
    }
}
