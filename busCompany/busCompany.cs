using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
//using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer;
using System.Collections.Specialized;

namespace InteractPayroll
{
    public class busCompany
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busCompany()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(string parstrCurrentUserAccess, Int64 parint64CurrentUserNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            DateTime dtPeriod;

            if (DateTime.Now.Month > 6)
            {
                dtPeriod = new DateTime(DateTime.Now.Year, 9, 1).AddDays(-1);
            }
            else
            {
                dtPeriod = new DateTime(DateTime.Now.Year, 3, 1).AddDays(-1);
            }

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" CD.COMPANY_DESC");
            strQry.AppendLine(",CD.COMPANY_NO");
            
            strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK CD");

            if (parstrCurrentUserAccess != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA");
                strQry.AppendLine(" ON UCA.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND UCA.COMPANY_NO = CD.COMPANY_NO ");
                strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL ");
            }

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" CD.COMPANY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyTemp", -1);
            
            for (int intRow = 0;intRow < DataSet.Tables["CompanyTemp"].Rows.Count;intRow++)
            {
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" CD.COMPANY_DESC");
                strQry.AppendLine(",CD.COMPANY_NO");
                strQry.AppendLine(",CD.POST_ADDR_LINE1");
                strQry.AppendLine(",CD.POST_ADDR_LINE2");
                strQry.AppendLine(",CD.POST_ADDR_LINE3");
                strQry.AppendLine(",CD.POST_ADDR_LINE4");
                strQry.AppendLine(",CD.POST_ADDR_CODE");
                
                strQry.AppendLine(",CD.TAX_REF_NO");

                strQry.AppendLine(",CD.UIF_REF_NO");
                strQry.AppendLine(",CD.VAT_NO");
                strQry.AppendLine(",CD.PAYE_REF_NO");
                strQry.AppendLine(",CD.TRADE_CLASSIFICATION_CODE");

                strQry.AppendLine(",CD.GENERATE_EMPLOYEE_NUMBER_IND");

                strQry.AppendLine(",CD.RES_UNIT_NUMBER");
                strQry.AppendLine(",CD.RES_COMPLEX");
                strQry.AppendLine(",CD.RES_STREET_NUMBER");
                strQry.AppendLine(",CD.RES_STREET_NAME");
                strQry.AppendLine(",CD.RES_SUBURB");
                strQry.AppendLine(",CD.RES_CITY");
                strQry.AppendLine(",CD.RES_ADDR_CODE");
               
                strQry.AppendLine(",CD.OVERTIME1_RATE");
                strQry.AppendLine(",CD.OVERTIME2_RATE");
                strQry.AppendLine(",CD.OVERTIME3_RATE");

                strQry.AppendLine(",CD.SALARY_OVERTIME1_RATE");
                strQry.AppendLine(",CD.SALARY_OVERTIME2_RATE");
                strQry.AppendLine(",CD.SALARY_OVERTIME3_RATE");

                strQry.AppendLine(",CD.COMPANY_DEL_IND");

                strQry.AppendLine(",CD.DIPLOMATIC_INDEMNITY_IND");
                strQry.AppendLine(",CD.SALARY_DOUBLE_CHEQUE_BIRTHDAY_IND");
                strQry.AppendLine(",CD.SDL_EXEMPT_IND");
                strQry.AppendLine(",CD.SDL_REF_NO");
                strQry.AppendLine(",CD.TEL_WORK");
                strQry.AppendLine(",CD.TEL_FAX");

                strQry.AppendLine(",CD.EFILING_NAMES");
                strQry.AppendLine(",CD.EFILING_CONTACT_NO");
                strQry.AppendLine(",CD.EFILING_EMAIL");

                strQry.AppendLine(",CD.BANK_NO");
                strQry.AppendLine(",CD.BRANCH_CODE");
                strQry.AppendLine(",CD.ACCOUNT_NO");
                strQry.AppendLine(",CD.ACCOUNT_REFERENCE_NO");

                strQry.AppendLine(",CL.DATE_FORMAT");
                strQry.AppendLine(",CD.FILE_NAME");

                strQry.AppendLine(",CL.DYNAMIC_UPLOAD_KEY");
                
                strQry.AppendLine(",SIC7_GROUP_CODE");
                strQry.AppendLine(",CL.LOCK_IND");
                strQry.AppendLine(",CD.LEAVE_BEGIN_MONTH");

                strQry.AppendLine(",COUNT(PCPH.PAY_CATEGORY_NO) AS COUNT_PAY_CATEGORY_NO");
                strQry.AppendLine(",0  AS COUNT_PAY_CATEGORY_NO_CURRENT");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY CD");

                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.COMPANY_LINK CL");
                strQry.AppendLine(" ON CD.COMPANY_NO = CL.COMPANY_NO ");

                if (parstrCurrentUserAccess != "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA");
                    strQry.AppendLine(" ON UCA.USER_NO = " + parint64CurrentUserNo);
                    strQry.AppendLine(" AND UCA.COMPANY_NO = CD.COMPANY_NO ");
                }

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");
                strQry.AppendLine(" ON CD.COMPANY_NO = PCPH.COMPANY_NO ");
                strQry.AppendLine(" AND PCPH.RUN_TYPE = 'P'");

                strQry.AppendLine(" WHERE CD.COMPANY_NO = " + DataSet.Tables["CompanyTemp"].Rows[intRow]["COMPANY_NO"].ToString());
                strQry.AppendLine(" AND CD.DATETIME_DELETE_RECORD IS NULL");

                if (parstrCurrentUserAccess != "S")
                {
                    strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL ");
                }

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" CD.COMPANY_DESC");
                strQry.AppendLine(",CD.COMPANY_NO");

                strQry.AppendLine(",CD.POST_ADDR_LINE1");
                strQry.AppendLine(",CD.POST_ADDR_LINE2");
                strQry.AppendLine(",CD.POST_ADDR_LINE3");
                strQry.AppendLine(",CD.POST_ADDR_LINE4");
                strQry.AppendLine(",CD.POST_ADDR_CODE");
                
                strQry.AppendLine(",CD.TAX_REF_NO");

                strQry.AppendLine(",CD.UIF_REF_NO");
                strQry.AppendLine(",CD.VAT_NO");
                strQry.AppendLine(",CD.PAYE_REF_NO");
                strQry.AppendLine(",CD.TRADE_CLASSIFICATION_CODE");

                strQry.AppendLine(",CD.GENERATE_EMPLOYEE_NUMBER_IND");

                strQry.AppendLine(",CD.RES_UNIT_NUMBER");
                strQry.AppendLine(",CD.RES_COMPLEX");
                strQry.AppendLine(",CD.RES_STREET_NUMBER");
                strQry.AppendLine(",CD.RES_STREET_NAME");
                strQry.AppendLine(",CD.RES_SUBURB");
                strQry.AppendLine(",CD.RES_CITY");
                strQry.AppendLine(",CD.RES_ADDR_CODE");
                
                strQry.AppendLine(",CD.OVERTIME1_RATE");
                strQry.AppendLine(",CD.OVERTIME2_RATE");
                strQry.AppendLine(",CD.OVERTIME3_RATE");

                strQry.AppendLine(",CD.SALARY_OVERTIME1_RATE");
                strQry.AppendLine(",CD.SALARY_OVERTIME2_RATE");
                strQry.AppendLine(",CD.SALARY_OVERTIME3_RATE");

                strQry.AppendLine(",CD.COMPANY_DEL_IND");
                strQry.AppendLine(",CD.DIPLOMATIC_INDEMNITY_IND");
                strQry.AppendLine(",CD.SALARY_DOUBLE_CHEQUE_BIRTHDAY_IND");
                strQry.AppendLine(",CD.SDL_EXEMPT_IND");
                strQry.AppendLine(",CD.SDL_REF_NO");
                strQry.AppendLine(",CD.TEL_WORK");
                strQry.AppendLine(",CD.TEL_FAX");

                strQry.AppendLine(",CD.EFILING_NAMES");
                strQry.AppendLine(",CD.EFILING_CONTACT_NO");
                strQry.AppendLine(",CD.EFILING_EMAIL");

                strQry.AppendLine(",CD.BANK_NO");
                strQry.AppendLine(",CD.BRANCH_CODE");
                strQry.AppendLine(",CD.ACCOUNT_NO");
                strQry.AppendLine(",CD.ACCOUNT_REFERENCE_NO");
                strQry.AppendLine(",CL.DATE_FORMAT");
                strQry.AppendLine(",CD.FILE_NAME");
                strQry.AppendLine(",CL.DYNAMIC_UPLOAD_KEY");
                //ELR - 2014-08-24
                strQry.AppendLine(",SIC7_GROUP_CODE");
                strQry.AppendLine(",CL.LOCK_IND");
                strQry.AppendLine(",CD.LEAVE_BEGIN_MONTH");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" CD.COMPANY_DESC");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", Convert.ToInt64(DataSet.Tables["CompanyTemp"].Rows[intRow]["COMPANY_NO"].ToString()));

                if (DataSet.Tables["CompanyPayrollTemp"] != null)
                {
                    DataSet.Tables.Remove("CompanyPayrollTemp");
                }

                strQry.Clear();
                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" COMPANY_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");

                strQry.AppendLine(" WHERE COMPANY_NO = " + DataSet.Tables["CompanyTemp"].Rows[intRow]["COMPANY_NO"].ToString());
                strQry.AppendLine(" AND RUN_TYPE = 'P'");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyPayrollTemp", Convert.ToInt64(DataSet.Tables["CompanyTemp"].Rows[intRow]["COMPANY_NO"].ToString()));

                if (DataSet.Tables["CompanyPayrollTemp"].Rows.Count > 0)
                {
                    DataSet.Tables["Company"].Rows[DataSet.Tables["Company"].Rows.Count - 1]["COUNT_PAY_CATEGORY_NO_CURRENT"] = 1;
                }

                //strQry.Clear();
                //strQry.AppendLine(" SELECT ");
                //strQry.AppendLine(" CPH.COMPANY_NO");
                //strQry.AppendLine(",CPH.PRINT_HEADER_NO");
                //strQry.AppendLine(",PHD.PRINT_HEADER_DESC");
                //strQry.AppendLine(",CPH.USED_IND");
                //strQry.AppendLine(",CPH.ALIGNMENT");
                //strQry.AppendLine(",CPH.P_POS_X");
                //strQry.AppendLine(",CPH.P_POS_Y");
                //strQry.AppendLine(",CPH.L_POS_X");
                //strQry.AppendLine(",CPH.L_POS_Y");
                //strQry.AppendLine(",CPH.BOLD_IND");
                //strQry.AppendLine(",CPH.UNDERLINE_IND");
                //strQry.AppendLine(",CPH.ITALIC_IND");
                //strQry.AppendLine(",CPH.PRINT_HEADER_TEXT");
                //strQry.AppendLine(",CPH.IMAGE_WIDTH");
                //strQry.AppendLine(",CPH.IMAGE_HEIGHT");
                //strQry.AppendLine(",CPH.FIXED_WIDTH");

                //strQry.AppendLine(" FROM ");
                //strQry.AppendLine(" InteractPayroll.dbo.PRINT_HEADER_DESC PHD");

                //strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY_PRINT_HEADER CPH");
                //strQry.AppendLine(" ON PHD.PRINT_HEADER_NO = CPH.PRINT_HEADER_NO");

                //if (parstrCurrentUserAccess != "S")
                //{
                //    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA");
                //    strQry.AppendLine(" ON UCA.USER_NO = " + parint64CurrentUserNo;
                //    strQry.AppendLine(" AND CPH.COMPANY_NO = UCA.COMPANY_NO ");
                //}

                //strQry.AppendLine(" WHERE CPH.COMPANY_NO = " + DataSet.Tables["CompanyTemp"].Rows[intRow]["COMPANY_NO"].ToString();

                //strQry.AppendLine(" ORDER BY ");
                //strQry.AppendLine(" CPH.COMPANY_NO");

                //clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "CompanyHeader", DataSet.Tables["CompanyTemp"].Rows[intRow]["COMPANY_NO"].ToString());

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(DataSet.Tables["CompanyTemp"].Rows[intRow]["COMPANY_NO"].ToString() + " AS COMPANY_NO");
                strQry.AppendLine(",EFILING_NO ");
                strQry.AppendLine(",EFILING_PERIOD ");
                strQry.AppendLine(",EFILING_COMPANY_CHECK_USER_NO ");
                strQry.AppendLine(",EFILING_CLOSED_IND ");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EFILING ");
                
                strQry.AppendLine(" WHERE EFILING_PERIOD = '" + dtPeriod.ToString("yyyy-MM-dd") + "'");

                strQry.AppendLine(" UNION ");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(DataSet.Tables["CompanyTemp"].Rows[intRow]["COMPANY_NO"].ToString() + " AS COMPANY_NO");
                strQry.AppendLine(",0 AS EFILING_NO ");
                strQry.AppendLine(",CONVERT(DateTime,'" + dtPeriod.ToString("yyyy-MM-dd") + "') AS EFILING_PERIOD ");
                strQry.AppendLine(",-1 AS EFILING_COMPANY_CHECK_USER_NO ");
                strQry.AppendLine(",'' AS EFILING_CLOSED_IND ");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C ");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EFILING E ");
                strQry.AppendLine(" ON E.EFILING_PERIOD = '" + dtPeriod.ToString("yyyy-MM-dd") + "'");

                strQry.AppendLine(" WHERE C.COMPANY_NO = " + DataSet.Tables["CompanyTemp"].Rows[intRow]["COMPANY_NO"].ToString());

                //No Record For This Period
                strQry.AppendLine(" AND E.EFILING_PERIOD IS NULL");

                strQry.AppendLine(" ORDER BY 3 ");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "eFilingPeriod", Convert.ToInt64(DataSet.Tables["CompanyTemp"].Rows[intRow]["COMPANY_NO"].ToString()));
            }

            DataSet.Tables.Remove("CompanyTemp");
           
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" BANK_NO");
            strQry.AppendLine(",BANK_DESC");

            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.BANK ");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" BANK_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Bank", -1);

            //strQry.Clear();
            //strQry.AppendLine(" SELECT ");
            //strQry.AppendLine(" PROVINCE_NO AS RES_ADDR_PROVINCE_NO");
            //strQry.AppendLine(",PROVINCE_DESC");

            //strQry.AppendLine(" FROM ");
            //strQry.AppendLine(" InteractPayroll.dbo.PROVINCE ");

            //strQry.AppendLine(" ORDER BY ");
            //strQry.AppendLine(" PROVINCE_DESC");

            //clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "ResProvince", -1);

            //strQry.Clear();
            //strQry.AppendLine(" SELECT ");
            //strQry.AppendLine(" PROVINCE_NO AS POST_ADDR_PROVINCE_NO");
            //strQry.AppendLine(",PROVINCE_DESC");

            //strQry.AppendLine(" FROM ");
            //strQry.AppendLine(" InteractPayroll.dbo.PROVINCE ");

            //strQry.AppendLine(" ORDER BY ");
            //strQry.AppendLine(" PROVINCE_DESC");

            //clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "PostProvince", -1);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" TRADE_CLASSIFICATION_CODE");
            strQry.AppendLine(",TRADE_CLASSIFICATION_DESC");

            strQry.AppendLine(" FROM InteractPayroll.dbo.TRADE_CLASSIFICATION ");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" TRADE_CLASSIFICATION_CODE");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "TradeClassify", -1);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" SIC7_GROUP_NO");
            strQry.AppendLine(",SIC7_GROUP_DESC");

            strQry.AppendLine(" FROM InteractPayroll.dbo.SIC7_GROUP ");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" SIC7_GROUP_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Sic7CodeGroup", -1);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" SIC7_GROUP_NO");
            strQry.AppendLine(",SIC7_GROUP_CODE");
            strQry.AppendLine(",SIC7_GROUP_CODE_DESC");

            strQry.AppendLine(" FROM InteractPayroll.dbo.SIC7_GROUP_CODE_DESC ");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" SIC7_GROUP_NO");
            strQry.AppendLine(",SIC7_GROUP_CODE_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Sic7Code", -1);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public void Insert_File_Chunk(Int64 parint64CompanyNo, int intBlockNumber, string parstrFileName, byte[] parbytesCompressed,
            DateTime dtFileLastUpdated, int intFileSize, int intCompressedSize, string strVersionNumber, bool blnComplete)
        {
            StringBuilder strQry = new StringBuilder();

            if (intBlockNumber == 1)
            {
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS_TEMP");
                strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parint64CompanyNo.ToString()));
                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            }

            if (blnComplete == true)
            {
                //Delete Master Chunks
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.PRINT_HEADER_IMAGE_FILE_CHUNKS");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                //Delete Master Detail
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.PRINT_HEADER_IMAGE_FILE_DETAILS");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                //Move Chunks to Master
                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.PRINT_HEADER_IMAGE_FILE_CHUNKS ");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",FILE_CHUNK_NO");
                strQry.AppendLine(",FILE_CHUNK)");
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PROJECT_VERSION");
                strQry.AppendLine(",FILE_CHUNK_NO");
                strQry.AppendLine(",FILE_CHUNK");
                strQry.AppendLine(" FROM ");
                strQry.AppendLine(" InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS_TEMP");
                strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parint64CompanyNo.ToString()));
                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                //Delete Temp Chunks
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS_TEMP");
                strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parint64CompanyNo.ToString()));
                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                //Insert Last Block
                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.PRINT_HEADER_IMAGE_FILE_CHUNKS");
                strQry.AppendLine(" (COMPANY_NO");
                strQry.AppendLine(" ,FILE_CHUNK_NO)");
                strQry.AppendLine("  VALUES");
                strQry.AppendLine(" (" + parint64CompanyNo);

                strQry.AppendLine(" ," + intBlockNumber.ToString() + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll.dbo.PRINT_HEADER_IMAGE_FILE_CHUNKS");
                strQry.AppendLine(" SET FILE_CHUNK = @FILE_CHUNK");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND FILE_CHUNK_NO = " + intBlockNumber.ToString());

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parbytesCompressed, "@FILE_CHUNK");

                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.PRINT_HEADER_IMAGE_FILE_DETAILS");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",FILE_NAME");
                strQry.AppendLine(",FILE_LAST_UPDATED_DATE");
                strQry.AppendLine(",FILE_SIZE");
                strQry.AppendLine(",FILE_SIZE_COMPRESSED");
                strQry.AppendLine(",FILE_VERSION_NO)");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine("(" + parint64CompanyNo);
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                strQry.AppendLine(",'" + dtFileLastUpdated.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                strQry.AppendLine("," + intFileSize);
                strQry.AppendLine("," + intCompressedSize);
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strVersionNumber) + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            }
            else
            {
                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS_TEMP");
                strQry.AppendLine(" (PROJECT_VERSION");
                strQry.AppendLine(" ,FILE_NAME");
                strQry.AppendLine(" ,FILE_CHUNK_NO)");
                strQry.AppendLine("  VALUES");
                strQry.AppendLine(" (" + clsDBConnectionObjects.Text2DynamicSQL(parint64CompanyNo.ToString()));
                strQry.AppendLine(" ," + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                strQry.AppendLine(" ," + intBlockNumber.ToString() + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS_TEMP");
                strQry.AppendLine(" SET FILE_CHUNK = @FILE_CHUNK");
                strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parint64CompanyNo.ToString()));
                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                strQry.AppendLine(" AND FILE_CHUNK_NO = " + intBlockNumber.ToString());

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parbytesCompressed, "@FILE_CHUNK");
            }
        }

        public Int64 Insert_New_Record(string parstrCurrentUserAccess, Int64 parint64CurrentUserNo, byte[] parbyteDataSet)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);
            DataSet DataSet = new DataSet();
            DataView ColumnDataView;
            DataView PrimaryKeyDataView;
            Int64 int64CompanyNo = -1;

            clsInteractPayrollTriggers clsInteractPayrollTriggers = new InteractPayroll.clsInteractPayrollTriggers();

            StringBuilder strQry = new StringBuilder();
            string strTableCreateQry = "";
            string[] strQryArray;
   
            strQryArray = new string[58];

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ISNULL(MAX(COMPANY_NO),0) + 1 AS MAX_NO");
            strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", -1);

            int64CompanyNo = Convert.ToInt64(DataSet.Tables[0].Rows[0]["MAX_NO"]);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",COMPANY_DESC");
            strQry.AppendLine(",DYNAMIC_UPLOAD_KEY");
            strQry.AppendLine(",DATE_FORMAT)");
            
            strQry.AppendLine(" VALUES ");
            
            strQry.AppendLine("(" + int64CompanyNo);
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["COMPANY_DESC"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["DYNAMIC_UPLOAD_KEY"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["DATE_FORMAT"].ToString()) + ")");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            string strDatabaseName = "InteractPayroll_" + int64CompanyNo.ToString("00000");

            strQry.Clear();
            strQry.AppendLine(" CREATE DATABASE " + strDatabaseName + " COLLATE SQL_Latin1_General_CP1_CI_AS");
            
            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            
            strQry.Clear();

            //Logon To DB - Otherwise SYSCOLUMNS Gives Invalid Results
            strQry.AppendLine(" USE InteractPayroll_Master");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" C.TABLE_NAME");
            strQry.AppendLine(",C.COLUMN_NAME");
            strQry.AppendLine(",C.DATA_TYPE");
            strQry.AppendLine(",LENGTH_FIELD = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN C.CHARACTER_MAXIMUM_LENGTH IS NULL ");
            strQry.AppendLine(" THEN ' '");

            strQry.AppendLine(" ELSE '(' + CAST(C.CHARACTER_MAXIMUM_LENGTH AS VARCHAR) + ')'");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",NULL_FIELD = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN C.IS_NULLABLE = 'NO'");

            strQry.AppendLine(" THEN ' NOT NULL,'");

            strQry.AppendLine(" ELSE ' NULL,'");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",IDENTITY_FIELD = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN NOT SC.ID IS NULL");

            strQry.AppendLine(" THEN 'IDENTITY(' + ");
            //In-Built SQL Function (IDENT_SEED and IDENT_INCR)
            strQry.AppendLine(" CAST(IDENT_SEED(C.TABLE_NAME) AS VARCHAR) + ',' + ");
            strQry.AppendLine(" CAST(IDENT_INCR(C.TABLE_NAME) AS VARCHAR) + ')' ");

            strQry.AppendLine(" ELSE ''");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",KEY_TABLE.ORDINAL_POSITION AS PRIMARY_KEY_ORDINAL_POSITION ");

            strQry.AppendLine(" FROM InteractPayroll_Master.INFORMATION_SCHEMA.COLUMNS C");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_Master.sys.SYSCOLUMNS SC");

            strQry.AppendLine(" ON C.TABLE_NAME = OBJECT_NAME(SC.ID) ");
            strQry.AppendLine(" AND C.COLUMN_NAME = SC.NAME");
            strQry.AppendLine(" AND SC.COLSTAT = 1");

            strQry.AppendLine(" LEFT JOIN ");

            strQry.AppendLine(" (SELECT ");
            strQry.AppendLine(" KCU.TABLE_NAME ");
            strQry.AppendLine(",KCU.COLUMN_NAME ");
            strQry.AppendLine(",KCU.ORDINAL_POSITION ");

            strQry.AppendLine(" FROM InteractPayroll_Master.INFORMATION_SCHEMA.TABLE_CONSTRAINTS  TC ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_Master.INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU ");

            strQry.AppendLine(" ON TC.CONSTRAINT_NAME = KCU.CONSTRAINT_NAME ");

            strQry.AppendLine(" WHERE TC.CONSTRAINT_TYPE = 'PRIMARY KEY') AS KEY_TABLE ");

            strQry.AppendLine(" ON C.TABLE_NAME = KEY_TABLE.TABLE_NAME ");
            strQry.AppendLine(" AND C.COLUMN_NAME = KEY_TABLE.COLUMN_NAME");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" C.TABLE_NAME");
            strQry.AppendLine(",C.ORDINAL_POSITION");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Column", -1);
            
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" TABLE_NAME ");
            strQry.AppendLine(" FROM InteractPayroll_Master.INFORMATION_SCHEMA.TABLES");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" TABLE_NAME ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Table", -1);

            for (int intRow = 0; intRow < DataSet.Tables["Table"].Rows.Count; intRow++)
            {
                strTableCreateQry = "";
                strTableCreateQry += " CREATE TABLE [InteractPayroll_#CompanyNo#].[dbo].[" + DataSet.Tables["Table"].Rows[intRow]["TABLE_NAME"].ToString() + "]  (";

                ColumnDataView = null;
                ColumnDataView = new DataView(DataSet.Tables["Column"]
                                                 ,"TABLE_NAME = '" + DataSet.Tables["Table"].Rows[intRow]["TABLE_NAME"].ToString() + "'"
                                                 ,""
                                                 ,DataViewRowState.CurrentRows);

                for (int intRow1 = 0; intRow1 < ColumnDataView.Count; intRow1++)
                {
                    strTableCreateQry += " [" + ColumnDataView[intRow1]["COLUMN_NAME"].ToString() + "] [" + ColumnDataView[intRow1]["DATA_TYPE"].ToString() + "] " + ColumnDataView[intRow1]["LENGTH_FIELD"].ToString();

                    if (ColumnDataView[intRow1]["IDENTITY_FIELD"].ToString() != "")
                    {
                        strTableCreateQry += ColumnDataView[intRow1]["IDENTITY_FIELD"].ToString();
                    }

                    strTableCreateQry += ColumnDataView[intRow1]["NULL_FIELD"].ToString();
                }

                PrimaryKeyDataView = null;
                PrimaryKeyDataView = new DataView(DataSet.Tables["Column"]
                                                 , "TABLE_NAME = '" + DataSet.Tables["Table"].Rows[intRow]["TABLE_NAME"].ToString() +  "' AND NOT PRIMARY_KEY_ORDINAL_POSITION IS NULL"
                                                 ,"PRIMARY_KEY_ORDINAL_POSITION"
                                                 ,DataViewRowState.CurrentRows);

                if (PrimaryKeyDataView.Count > 0)
                {
                    strTableCreateQry += " CONSTRAINT [PK_" + DataSet.Tables["Table"].Rows[intRow]["TABLE_NAME"].ToString() + "] PRIMARY KEY CLUSTERED ( ";

                    for (int intRow2 = 0; intRow2 < PrimaryKeyDataView.Count; intRow2++)
                    {
                        if (intRow2 == PrimaryKeyDataView.Count - 1)
                        {
                            strTableCreateQry += " [" + PrimaryKeyDataView[intRow2]["COLUMN_NAME"].ToString() + "] ASC ";
                        }
                        else
                        {
                            strTableCreateQry += " [" + PrimaryKeyDataView[intRow2]["COLUMN_NAME"].ToString() + "] ASC,";

                        }
                    }

                    strTableCreateQry += " ) ON [PRIMARY] ";
                }
                else
                {
                    //Remove Last Comma from Field Definitions
                    strTableCreateQry = strTableCreateQry.Substring(0, strTableCreateQry.Length - 1);
                }

                strTableCreateQry += " ) ON [PRIMARY] ";

                clsDBConnectionObjects.Execute_SQLCommand(strTableCreateQry, int64CompanyNo);
            }

            string strConnectionString = clsDBConnectionObjects.Get_ConnectionString();
            string strNewDatabase = "InteractPayroll_" + int64CompanyNo.ToString("00000");

            //Logon To Correct Database
            clsDBConnectionObjects.Set_ConnectionString(strConnectionString + "Database=" + strNewDatabase + ";");

            //Create TRIGGER tgr_Create_Payroll_Timesheet
            strQry.Clear();
            strQry.Append(clsInteractPayrollTriggers.tgr_Create_Payroll_Timesheet());

            this.clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            //Create TRIGGER tgr_Create_Payroll_Break
            strQry.Clear();
            strQry.Append(clsInteractPayrollTriggers.tgr_Create_Payroll_Break());

            this.clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            //Create TRIGGER tgr_EMPLOYEE_BREAK_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table
            strQry.Clear();
            strQry.Append(clsInteractPayrollTriggers.tgr_Create_For_Timesheet_Break_Table("EMPLOYEE_BREAK_CURRENT"));
            
            this.clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            //Create TRIGGER tgr_EMPLOYEE_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT_Table
            strQry.Clear();
            strQry.Append(clsInteractPayrollTriggers.tgr_Create_For_Timesheet_Break_Table("EMPLOYEE_TIMESHEET_CURRENT"));
            
            this.clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            //Create TRIGGER tgr_EMPLOYEE_TIME_ATTEND_BREAK_CURRENT_Maintain_EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_Table
            strQry.Clear();
            strQry.Append(clsInteractPayrollTriggers.tgr_Create_For_Timesheet_Break_Table("EMPLOYEE_TIME_ATTEND_BREAK_CURRENT"));
            
            this.clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            //Create TRIGGER tgr_EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT_Maintain_EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT_Table
            strQry.Clear();
            strQry.Append(clsInteractPayrollTriggers.tgr_Create_For_Timesheet_Break_Table("EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT"));
            
            this.clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            //Create TRIGGER tgr_EMPLOYEE_SALARY_BREAK_CURRENT_Maintain_EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT_Table
            strQry.Clear();
            strQry.Append(clsInteractPayrollTriggers.tgr_Create_For_Timesheet_Break_Table("EMPLOYEE_SALARY_BREAK_CURRENT"));
            
            this.clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            //Create TRIGGER tgr_EMPLOYEE_SALARY_TIMESHEET_CURRENT_Maintain_EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT_Table
            strQry.Clear();
            strQry.Append(clsInteractPayrollTriggers.tgr_Create_For_Timesheet_Break_Table("EMPLOYEE_SALARY_TIMESHEET_CURRENT"));
            
            this.clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            
            string strCode = "";

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.COMPANY");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",COMPANY_DESC");
            strQry.AppendLine(",POST_ADDR_LINE1");
            strQry.AppendLine(",POST_ADDR_LINE2");
            strQry.AppendLine(",POST_ADDR_LINE3");
            strQry.AppendLine(",POST_ADDR_LINE4");
            strQry.AppendLine(",POST_ADDR_CODE");
            strQry.AppendLine(",TAX_REF_NO");
            strQry.AppendLine(",UIF_REF_NO");
            strQry.AppendLine(",VAT_NO");
            strQry.AppendLine(",PAYE_REF_NO");
            strQry.AppendLine(",TRADE_CLASSIFICATION_CODE");

            strQry.AppendLine(",GENERATE_EMPLOYEE_NUMBER_IND");

            strQry.AppendLine(",RES_UNIT_NUMBER");
            strQry.AppendLine(",RES_COMPLEX");
            strQry.AppendLine(",RES_STREET_NUMBER");
            strQry.AppendLine(",RES_STREET_NAME");
            strQry.AppendLine(",RES_SUBURB");
            strQry.AppendLine(",RES_CITY");
            strQry.AppendLine(",RES_ADDR_CODE");

            strQry.AppendLine(",COMPANY_DEL_IND");
            strQry.AppendLine(",OVERTIME1_RATE");
            strQry.AppendLine(",OVERTIME2_RATE");
            strQry.AppendLine(",OVERTIME3_RATE");

            strQry.AppendLine(",SALARY_OVERTIME1_RATE");
            strQry.AppendLine(",SALARY_OVERTIME2_RATE");
            strQry.AppendLine(",SALARY_OVERTIME3_RATE");

            strQry.AppendLine(",DIPLOMATIC_INDEMNITY_IND");
            strQry.AppendLine(",SALARY_DOUBLE_CHEQUE_BIRTHDAY_IND");
            strQry.AppendLine(",TEL_WORK");
            strQry.AppendLine(",TEL_FAX");

            strQry.AppendLine(",EFILING_NAMES");
            strQry.AppendLine(",EFILING_CONTACT_NO");
            strQry.AppendLine(",EFILING_EMAIL");

            //ELR - 2014-08-24
            strQry.AppendLine(",SIC7_GROUP_CODE");

            //ELR - 2017-01-11
            strQry.AppendLine(",LEAVE_BEGIN_MONTH");
            
            strQry.AppendLine(",SDL_EXEMPT_IND");
            strQry.AppendLine(",SDL_REF_NO");
            strQry.AppendLine(",BANK_NO");
            strQry.AppendLine(",BRANCH_CODE");
            strQry.AppendLine(",ACCOUNT_NO");
            strQry.AppendLine(",ACCOUNT_REFERENCE_NO");
            strQry.AppendLine(",FILE_NAME");
            strQry.AppendLine(",VALUE_1");
            strQry.AppendLine(",VALUE_2");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD)");
            strQry.AppendLine(" VALUES");
            strQry.AppendLine("(" + int64CompanyNo);
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["COMPANY_DESC"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["POST_ADDR_LINE1"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["POST_ADDR_LINE2"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["POST_ADDR_LINE3"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["POST_ADDR_LINE4"].ToString()));

            if (parDataSet.Tables["Company"].Rows[0]["POST_ADDR_CODE"].ToString().Trim().Length > 4)
            {
                try
                {
                    int intResCode = Convert.ToInt32(parDataSet.Tables["Company"].Rows[0]["POST_ADDR_CODE"].ToString().Trim().Substring(0, 4));
                    strCode = parDataSet.Tables["Company"].Rows[0]["POST_ADDR_CODE"].ToString().Trim().Substring(0, 4);
                }
                catch
                {
                    strCode = "";
                }

                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strCode));
            }
            else
            {
                try
                {
                    int intResCode = Convert.ToInt32(parDataSet.Tables["Company"].Rows[0]["POST_ADDR_CODE"].ToString().Trim());
                    strCode = parDataSet.Tables["Company"].Rows[0]["POST_ADDR_CODE"].ToString().Trim();
                }
                catch
                {
                    strCode = "";
                }

                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strCode));
            }

            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["TAX_REF_NO"].ToString()));

            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["UIF_REF_NO"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["VAT_NO"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["PAYE_REF_NO"].ToString()));

            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["TRADE_CLASSIFICATION_CODE"].ToString()));

            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["GENERATE_EMPLOYEE_NUMBER_IND"].ToString()));

            //NB (RES_UNIT_NUMBER) 8 Chararcters Only - For Pastel Load
            if (parDataSet.Tables["Company"].Rows[0]["RES_UNIT_NUMBER"].ToString().Trim().Length > 8)
            {
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["RES_UNIT_NUMBER"].ToString().Trim().Substring(0, 8)));
            }
            else
            {
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["RES_UNIT_NUMBER"].ToString().Trim()));
            }

            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["RES_COMPLEX"].ToString()));

            //NB (RES_STREET_NUMBER) 8 Chararcters Only - For Pastel Load
            if (parDataSet.Tables["Company"].Rows[0]["RES_STREET_NUMBER"].ToString().Trim().Length > 8)
            {
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["RES_STREET_NUMBER"].ToString().Trim().Substring(0, 8)));
            }
            else
            {
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["RES_STREET_NUMBER"].ToString().Trim()));
            }

            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["RES_STREET_NAME"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["RES_SUBURB"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["RES_CITY"].ToString()));
            //NB (RES_ADDR_CODE) 4 Chararcters Only - For Pastel Load
          
            if (parDataSet.Tables["Company"].Rows[0]["RES_ADDR_CODE"].ToString().Trim().Length > 4)
            {
                try
                {
                    int intResCode = Convert.ToInt32(parDataSet.Tables["Company"].Rows[0]["RES_ADDR_CODE"].ToString().Trim().Substring(0, 4));
                    strCode = parDataSet.Tables["Company"].Rows[0]["RES_ADDR_CODE"].ToString().Trim().Substring(0, 4);
                }
                catch
                {
                    strCode = "";
                }

                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strCode));
            }
            else
            {
                try
                {
                    int intResCode = Convert.ToInt32(parDataSet.Tables["Company"].Rows[0]["RES_ADDR_CODE"].ToString().Trim());
                    strCode = parDataSet.Tables["Company"].Rows[0]["RES_ADDR_CODE"].ToString().Trim();
                }
                catch
                {
                    strCode = "";
                }

                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strCode));
            }

            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL("Y"));

            strQry.AppendLine("," + parDataSet.Tables["Company"].Rows[0]["OVERTIME1_RATE"]);
            strQry.AppendLine("," + parDataSet.Tables["Company"].Rows[0]["OVERTIME2_RATE"]);
            strQry.AppendLine("," + parDataSet.Tables["Company"].Rows[0]["OVERTIME3_RATE"]);

            strQry.AppendLine("," + parDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME1_RATE"]);
            strQry.AppendLine("," + parDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME2_RATE"]);
            strQry.AppendLine("," + parDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME3_RATE"]);

            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["DIPLOMATIC_INDEMNITY_IND"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["SALARY_DOUBLE_CHEQUE_BIRTHDAY_IND"].ToString()));
            
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["TEL_WORK"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["TEL_FAX"].ToString()));

            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["EFILING_NAMES"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["EFILING_CONTACT_NO"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["EFILING_EMAIL"].ToString()));

            //ELR - 2014-08-24
            if (parDataSet.Tables["Company"].Rows[0]["SIC7_GROUP_CODE"].ToString() != "")
            {
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["SIC7_GROUP_CODE"].ToString()));
            }
            else
            {
                strQry.AppendLine(",Null");
            }

            //ELR - 2017-01-11 
            strQry.AppendLine("," + parDataSet.Tables["Company"].Rows[0]["LEAVE_BEGIN_MONTH"].ToString());

            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["SDL_EXEMPT_IND"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["SDL_REF_NO"].ToString()));
            strQry.AppendLine("," + parDataSet.Tables["Company"].Rows[0]["BANK_NO"].ToString());

            if (parDataSet.Tables["Company"].Rows[0]["BRANCH_CODE"] != System.DBNull.Value)
            {
                strQry.AppendLine("," + parDataSet.Tables["Company"].Rows[0]["BRANCH_CODE"].ToString());
            }
            else
            {
                strQry.AppendLine(",Null");
            }

            if (parDataSet.Tables["Company"].Rows[0]["ACCOUNT_NO"] != System.DBNull.Value)
            {
                strQry.AppendLine("," + parDataSet.Tables["Company"].Rows[0]["ACCOUNT_NO"].ToString());
            }
            else
            {
                strQry.AppendLine(",Null");
            }

            if (parDataSet.Tables["Company"].Rows[0]["ACCOUNT_REFERENCE_NO"] != System.DBNull.Value)
            {
                strQry.AppendLine("," + parDataSet.Tables["Company"].Rows[0]["ACCOUNT_REFERENCE_NO"].ToString());
            }
            else
            {
                strQry.AppendLine(",Null");
            }
           
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["FILE_NAME"].ToString()));

            //Used To Set EXTRA_CHEQUES_CURRENT and Double EARNING_AMOUNT when Double Cheque on BirthDay is Set
            strQry.AppendLine(",1");
            strQry.AppendLine(",2");
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo + ")");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            if (parstrCurrentUserAccess != "S")
            {
                //Administrator
                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.USER_COMPANY_ACCESS");
                strQry.AppendLine("(USER_NO");
                strQry.AppendLine(",COMPANY_NO");
                strQry.AppendLine(",TIE_BREAKER");
                strQry.AppendLine(",EMPLOYEE_NO");
                strQry.AppendLine(",COMPANY_ACCESS_IND");
                strQry.AppendLine(",DATETIME_NEW_RECORD");
                strQry.AppendLine(",USER_NO_NEW_RECORD)");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine("(" + parint64CurrentUserNo);
                strQry.AppendLine("," + int64CompanyNo);
                strQry.AppendLine(",1");
                //No Link to an Employee
                strQry.AppendLine(",-1");
                strQry.AppendLine(",'A'");
                strQry.AppendLine(",GETDATE()");
                strQry.AppendLine("," + parint64CurrentUserNo + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);
            }

            //Pay Category for Each Company
            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PAY_CATEGORY_DESC");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD)");
            strQry.AppendLine(" VALUES");
            strQry.AppendLine("(" + int64CompanyNo);
            strQry.AppendLine(",0");
            strQry.AppendLine(",'W'");
            strQry.AppendLine(",'Employee Take-On - Wages'");
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo + ")");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PAY_CATEGORY_DESC");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD)");
            strQry.AppendLine(" VALUES");
            strQry.AppendLine("(" + int64CompanyNo);
            strQry.AppendLine(",0");
            strQry.AppendLine(",'S'");
            strQry.AppendLine(",'Employee Take-On - Salaries'");
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo + ")");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.DEDUCTION");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",DEDUCTION_DESC");
            strQry.AppendLine(",DEDUCTION_DEL_IND");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_COUNT");
            strQry.AppendLine(",DEDUCTION_LOAN_TYPE_IND");
            strQry.AppendLine(",DEDUCTION_REPORT_HEADER1");
            strQry.AppendLine(",DEDUCTION_REPORT_HEADER2");
            strQry.AppendLine(",IRP5_CODE");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD)");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",1");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'Tax'");
            strQry.AppendLine(",'N'");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'N'");
            strQry.AppendLine(",'Tax'");
            strQry.AppendLine(",Null");
            strQry.AppendLine(",'4102'");
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.PAY_CATEGORY_CONVERT");
            strQry.AppendLine(" WHERE PAY_CATEGORY_TYPE_BOTH = 'B'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",TIE_BREAKER");
            strQry.AppendLine(",DEDUCTION_TYPE_IND");
            strQry.AppendLine(",DEDUCTION_VALUE");
            strQry.AppendLine(",DEDUCTION_PERIOD_IND");
            strQry.AppendLine(",DEDUCTION_DAY_VALUE");
            strQry.AppendLine(",DEDUCTION_MIN_VALUE");
            strQry.AppendLine(",DEDUCTION_MAX_VALUE");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD)");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",1");
            strQry.AppendLine(",1");
            strQry.AppendLine(",0");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'P'");
            strQry.AppendLine(",0");
            strQry.AppendLine(",'E'");
            strQry.AppendLine(",0");
            strQry.AppendLine(",0");
            strQry.AppendLine(",0");
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.PAY_CATEGORY_CONVERT");
            strQry.AppendLine(" WHERE PAY_CATEGORY_TYPE_BOTH = 'B'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.DEDUCTION");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",DEDUCTION_DESC");

            //2013-09-16
            strQry.AppendLine(",IRP5_CODE");

            strQry.AppendLine(",DEDUCTION_DEL_IND");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_COUNT");
            strQry.AppendLine(",DEDUCTION_LOAN_TYPE_IND");
            strQry.AppendLine(",DEDUCTION_REPORT_HEADER1");
            strQry.AppendLine(",DEDUCTION_REPORT_HEADER2");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD)");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",2");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'UIF'");

            //2013-09-16
            strQry.AppendLine(",4141");

            strQry.AppendLine(",'N'");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'N'");
            strQry.AppendLine(",'UIF'");
            strQry.AppendLine(",Null");
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.PAY_CATEGORY_CONVERT");
            strQry.AppendLine(" WHERE PAY_CATEGORY_TYPE_BOTH = 'B'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",TIE_BREAKER");
            strQry.AppendLine(",DEDUCTION_TYPE_IND");
            strQry.AppendLine(",DEDUCTION_VALUE");
            strQry.AppendLine(",DEDUCTION_PERIOD_IND");
            strQry.AppendLine(",DEDUCTION_DAY_VALUE");
            strQry.AppendLine(",DEDUCTION_MIN_VALUE");
            strQry.AppendLine(",DEDUCTION_MAX_VALUE");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD)");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",PCC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",2");
            strQry.AppendLine(",1");
            strQry.AppendLine(",0");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'P'");
            strQry.AppendLine(",UT.UIF_PERCENTAGE");
            strQry.AppendLine(",'E'");
            strQry.AppendLine(",0");
            strQry.AppendLine(",0");
            strQry.AppendLine(",0");
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(" FROM  ");
            strQry.AppendLine(" InteractPayroll.dbo.UIF_THRESHOLD UT ");
            strQry.AppendLine(",InteractPayroll.dbo.PAY_CATEGORY_CONVERT PCC");
            strQry.AppendLine(" WHERE PCC.PAY_CATEGORY_TYPE_BOTH = 'B'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.DEDUCTION");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",DEDUCTION_DESC");
            strQry.AppendLine(",DEDUCTION_DEL_IND");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_COUNT");
            strQry.AppendLine(",DEDUCTION_LOAN_TYPE_IND");
            strQry.AppendLine(",DEDUCTION_REPORT_HEADER1");
            strQry.AppendLine(",DEDUCTION_REPORT_HEADER2");
            strQry.AppendLine(",IRP5_CODE");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD)");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",3");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'Pension Fund'");
            strQry.AppendLine(",'N'");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'N'");
            strQry.AppendLine(",'Pension'");
            strQry.AppendLine(",'Fund'");
            strQry.AppendLine(",'4001'");
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.PAY_CATEGORY_CONVERT");
            strQry.AppendLine(" WHERE PAY_CATEGORY_TYPE_BOTH = 'B'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",TIE_BREAKER");
            strQry.AppendLine(",DEDUCTION_TYPE_IND");
            strQry.AppendLine(",DEDUCTION_VALUE");
            strQry.AppendLine(",DEDUCTION_PERIOD_IND");
            strQry.AppendLine(",DEDUCTION_DAY_VALUE");
            strQry.AppendLine(",DEDUCTION_MIN_VALUE");
            strQry.AppendLine(",DEDUCTION_MAX_VALUE");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD)");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",3");
            strQry.AppendLine(",1");
            strQry.AppendLine(",0");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'U'");
            strQry.AppendLine(",0");
            strQry.AppendLine(",'E'");
            strQry.AppendLine(",0");
            strQry.AppendLine(",0");
            strQry.AppendLine(",0");
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.PAY_CATEGORY_CONVERT");
            strQry.AppendLine(" WHERE PAY_CATEGORY_TYPE_BOTH = 'B'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.DEDUCTION");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",DEDUCTION_DESC");
            strQry.AppendLine(",DEDUCTION_DEL_IND");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_COUNT");
            strQry.AppendLine(",DEDUCTION_LOAN_TYPE_IND");
            strQry.AppendLine(",DEDUCTION_REPORT_HEADER1");
            strQry.AppendLine(",DEDUCTION_REPORT_HEADER2");
            strQry.AppendLine(",IRP5_CODE");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD)");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",4");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'Retirement Annuity'");
            strQry.AppendLine(",'N'");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'N'");
            strQry.AppendLine(",'Retirement'");
            strQry.AppendLine(",'Annuity'");
            strQry.AppendLine(",'4006'");
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.PAY_CATEGORY_CONVERT");
            strQry.AppendLine(" WHERE PAY_CATEGORY_TYPE_BOTH = 'B'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",TIE_BREAKER");
            strQry.AppendLine(",DEDUCTION_TYPE_IND");
            strQry.AppendLine(",DEDUCTION_VALUE");
            strQry.AppendLine(",DEDUCTION_PERIOD_IND");
            strQry.AppendLine(",DEDUCTION_DAY_VALUE");
            strQry.AppendLine(",DEDUCTION_MIN_VALUE");
            strQry.AppendLine(",DEDUCTION_MAX_VALUE");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD)");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",4");
            strQry.AppendLine(",1");
            strQry.AppendLine(",0");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'U'");
            strQry.AppendLine(",0");
            strQry.AppendLine(",'E'");
            strQry.AppendLine(",0");
            strQry.AppendLine(",0");
            strQry.AppendLine(",0");
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.PAY_CATEGORY_CONVERT");
            strQry.AppendLine(" WHERE PAY_CATEGORY_TYPE_BOTH = 'B'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.DEDUCTION");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",DEDUCTION_DESC");
            strQry.AppendLine(",DEDUCTION_DEL_IND");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_COUNT");
            strQry.AppendLine(",DEDUCTION_LOAN_TYPE_IND");
            strQry.AppendLine(",DEDUCTION_REPORT_HEADER1");
            strQry.AppendLine(",DEDUCTION_REPORT_HEADER2");
            strQry.AppendLine(",IRP5_CODE");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD)");
            
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            
            strQry.AppendLine(",5");
            strQry.AppendLine(",1");
            
            strQry.AppendLine(",'Medical Aid'");
            strQry.AppendLine(",'N'");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'N'");
            strQry.AppendLine(",'Medical'");
            strQry.AppendLine(",'Aid'");
            strQry.AppendLine(",'4005'");
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.PAY_CATEGORY_CONVERT");
            strQry.AppendLine(" WHERE PAY_CATEGORY_TYPE_BOTH = 'B'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",TIE_BREAKER");
            strQry.AppendLine(",DEDUCTION_TYPE_IND");
            strQry.AppendLine(",DEDUCTION_VALUE");
            strQry.AppendLine(",DEDUCTION_PERIOD_IND");
            strQry.AppendLine(",DEDUCTION_DAY_VALUE");
            strQry.AppendLine(",DEDUCTION_MIN_VALUE");
            strQry.AppendLine(",DEDUCTION_MAX_VALUE");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD)");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",5");
            strQry.AppendLine(",1");
            strQry.AppendLine(",0");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'U'");
            strQry.AppendLine(",0");
            strQry.AppendLine(",'E'");
            strQry.AppendLine(",0");
            strQry.AppendLine(",0");
            strQry.AppendLine(",0");
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.PAY_CATEGORY_CONVERT");
            strQry.AppendLine(" WHERE PAY_CATEGORY_TYPE_BOTH = 'B'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.DEDUCTION");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",DEDUCTION_DESC");
            strQry.AppendLine(",DEDUCTION_DEL_IND");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_COUNT");
            strQry.AppendLine(",DEDUCTION_LOAN_TYPE_IND");
            strQry.AppendLine(",DEDUCTION_REPORT_HEADER1");
            strQry.AppendLine(",DEDUCTION_REPORT_HEADER2");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD)");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",6");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'Loan'");
            strQry.AppendLine(",'N'");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'Y'");
            strQry.AppendLine(",'Loan'");
            strQry.AppendLine(",Null");
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.PAY_CATEGORY_CONVERT");
            strQry.AppendLine(" WHERE PAY_CATEGORY_TYPE_BOTH = 'B'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",TIE_BREAKER");
            strQry.AppendLine(",DEDUCTION_TYPE_IND");
            strQry.AppendLine(",DEDUCTION_VALUE");
            strQry.AppendLine(",DEDUCTION_PERIOD_IND");
            strQry.AppendLine(",DEDUCTION_DAY_VALUE");
            strQry.AppendLine(",DEDUCTION_MIN_VALUE");
            strQry.AppendLine(",DEDUCTION_MAX_VALUE");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD)");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",6");
            strQry.AppendLine(",1");
            strQry.AppendLine(",0");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'U'");
            strQry.AppendLine(",0");
            strQry.AppendLine(",'E'");
            strQry.AppendLine(",0");
            strQry.AppendLine(",0");
            strQry.AppendLine(",0");
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.PAY_CATEGORY_CONVERT");
            strQry.AppendLine(" WHERE PAY_CATEGORY_TYPE_BOTH = 'B'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.DEDUCTION");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",DEDUCTION_DESC");
            strQry.AppendLine(",DEDUCTION_DEL_IND");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_COUNT");
            strQry.AppendLine(",DEDUCTION_LOAN_TYPE_IND");
            strQry.AppendLine(",DEDUCTION_REPORT_HEADER1");
            strQry.AppendLine(",DEDUCTION_REPORT_HEADER2");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD)");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",7");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'Garnashee Order'");
            strQry.AppendLine(",'N'");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'Y'");
            strQry.AppendLine(",'Garnashee'");
            strQry.AppendLine(",'Order'");
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.PAY_CATEGORY_CONVERT");
            strQry.AppendLine(" WHERE PAY_CATEGORY_TYPE_BOTH = 'B'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",TIE_BREAKER");
            strQry.AppendLine(",DEDUCTION_TYPE_IND");
            strQry.AppendLine(",DEDUCTION_VALUE");
            strQry.AppendLine(",DEDUCTION_PERIOD_IND");
            strQry.AppendLine(",DEDUCTION_DAY_VALUE");
            strQry.AppendLine(",DEDUCTION_MIN_VALUE");
            strQry.AppendLine(",DEDUCTION_MAX_VALUE");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD)");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",7");
            strQry.AppendLine(",1");
            strQry.AppendLine(",0");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'U'");
            strQry.AppendLine(",0");
            strQry.AppendLine(",'E'");
            strQry.AppendLine(",0");
            strQry.AppendLine(",0");
            strQry.AppendLine(",0");
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.PAY_CATEGORY_CONVERT");
            strQry.AppendLine(" WHERE PAY_CATEGORY_TYPE_BOTH = 'B'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.DEDUCTION");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",DEDUCTION_DESC");
            strQry.AppendLine(",DEDUCTION_DEL_IND");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_COUNT");
            strQry.AppendLine(",DEDUCTION_LOAN_TYPE_IND");
            strQry.AppendLine(",DEDUCTION_REPORT_HEADER1");
            strQry.AppendLine(",DEDUCTION_REPORT_HEADER2");
            strQry.AppendLine(",IRP5_CODE");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD)");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",8");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'Pension Fund Arrear'");
            strQry.AppendLine(",'N'");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'N'");
            strQry.AppendLine(",'Pension'");
            strQry.AppendLine(",'Fnd Arrear'");
            strQry.AppendLine(",'4002'");
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.PAY_CATEGORY_CONVERT");
            strQry.AppendLine(" WHERE PAY_CATEGORY_TYPE_BOTH = 'B'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",TIE_BREAKER");
            strQry.AppendLine(",DEDUCTION_TYPE_IND");
            strQry.AppendLine(",DEDUCTION_VALUE");
            strQry.AppendLine(",DEDUCTION_PERIOD_IND");
            strQry.AppendLine(",DEDUCTION_DAY_VALUE");
            strQry.AppendLine(",DEDUCTION_MIN_VALUE");
            strQry.AppendLine(",DEDUCTION_MAX_VALUE");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD)");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",8");
            strQry.AppendLine(",1");
            strQry.AppendLine(",0");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'U'");
            strQry.AppendLine(",0");
            strQry.AppendLine(",'E'");
            strQry.AppendLine(",0");
            strQry.AppendLine(",0");
            strQry.AppendLine(",0");
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.PAY_CATEGORY_CONVERT");
            strQry.AppendLine(" WHERE PAY_CATEGORY_TYPE_BOTH = 'B'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.DEDUCTION");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",DEDUCTION_DESC");
            strQry.AppendLine(",DEDUCTION_DEL_IND");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_COUNT");
            strQry.AppendLine(",DEDUCTION_LOAN_TYPE_IND");
            strQry.AppendLine(",DEDUCTION_REPORT_HEADER1");
            strQry.AppendLine(",DEDUCTION_REPORT_HEADER2");
            strQry.AppendLine(",IRP5_CODE");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD)");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",9");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'Retirement Annuity Arrear'");
            strQry.AppendLine(",'N'");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'N'");
            strQry.AppendLine(",'Retirement'");
            strQry.AppendLine(",'Annty Arr'");
            strQry.AppendLine(",'4007'");
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.PAY_CATEGORY_CONVERT");
            strQry.AppendLine(" WHERE PAY_CATEGORY_TYPE_BOTH = 'B'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",TIE_BREAKER");
            strQry.AppendLine(",DEDUCTION_TYPE_IND");
            strQry.AppendLine(",DEDUCTION_VALUE");
            strQry.AppendLine(",DEDUCTION_PERIOD_IND");
            strQry.AppendLine(",DEDUCTION_DAY_VALUE");
            strQry.AppendLine(",DEDUCTION_MIN_VALUE");
            strQry.AppendLine(",DEDUCTION_MAX_VALUE");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD)");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",9");
            strQry.AppendLine(",1");
            strQry.AppendLine(",0");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'U'");
            strQry.AppendLine(",0");
            strQry.AppendLine(",'E'");
            strQry.AppendLine(",0");
            strQry.AppendLine(",0");
            strQry.AppendLine(",0");
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.PAY_CATEGORY_CONVERT");
            strQry.AppendLine(" WHERE PAY_CATEGORY_TYPE_BOTH = 'B'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EARNING");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(",TIE_BREAKER");
            strQry.AppendLine(",EARNING_DESC");
            strQry.AppendLine(",LEAVE_PERCENTAGE");
            strQry.AppendLine(",IRP5_CODE");
            strQry.AppendLine(",EARNING_REPORT_HEADER1");
            strQry.AppendLine(",EARNING_REPORT_HEADER2");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD");
            strQry.AppendLine(",EARNING_DEL_IND)");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",PCC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",ED.EARNING_NO");
            strQry.AppendLine(",1");
            strQry.AppendLine(",ED.EARNING_DESC");
            strQry.AppendLine(",100");
            strQry.AppendLine(",ED.IRP5_CODE");
            strQry.AppendLine(",ED.EARNING_REPORT_HEADER1");
            strQry.AppendLine(",ED.EARNING_REPORT_HEADER2");
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(",'N'");
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.EARNING_DEFAULT ED");
            strQry.AppendLine(",InteractPayroll.dbo.PAY_CATEGORY_CONVERT PCC");
            strQry.AppendLine(" WHERE ED.PAY_CATEGORY_TYPE_BOTH = PCC.PAY_CATEGORY_TYPE_BOTH");
            strQry.AppendLine(" AND ED.EARNING_NO IN (1,2,7,8,9,200,201)");

            strQry.AppendLine(" UNION");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",PCC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",ED.EARNING_NO");
            strQry.AppendLine(",1");
            strQry.AppendLine(",ED.EARNING_DESC");
            strQry.AppendLine(",0");
            strQry.AppendLine(",ED.IRP5_CODE");
            strQry.AppendLine(",ED.EARNING_REPORT_HEADER1");
            strQry.AppendLine(",ED.EARNING_REPORT_HEADER2");
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(",'N'");
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.EARNING_DEFAULT ED");
            strQry.AppendLine(",InteractPayroll.dbo.PAY_CATEGORY_CONVERT PCC");
            strQry.AppendLine(" WHERE ED.PAY_CATEGORY_TYPE_BOTH = PCC.PAY_CATEGORY_TYPE_BOTH");
            //UnPaid Leave
            strQry.AppendLine(" AND ED.EARNING_NO = 202");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            //NB NB Dynamically Create EMPLOYEE_EARNING For Earnings ALWAYS Linked to Employees

            //Overtime 1 - Wages
            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EARNING");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(",TIE_BREAKER");
            strQry.AppendLine(",EARNING_DESC");
            strQry.AppendLine(",LEAVE_PERCENTAGE");
            strQry.AppendLine(",IRP5_CODE");
            strQry.AppendLine(",EARNING_REPORT_HEADER1");
            strQry.AppendLine(",EARNING_REPORT_HEADER2");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD");
            strQry.AppendLine(",EARNING_DEL_IND)");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",'W'");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'Overtime (" + Convert.ToDouble(parDataSet.Tables["Company"].Rows[0]["OVERTIME1_RATE"]).ToString("0.00") + ")'");
            strQry.AppendLine(",100");
            strQry.AppendLine(",IRP5_CODE");
            strQry.AppendLine(",'Overtime'");
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(Convert.ToDouble(parDataSet.Tables["Company"].Rows[0]["OVERTIME1_RATE"]).ToString("0.00")));
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(",'N'");
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.EARNING_DEFAULT ED");
            strQry.AppendLine(" WHERE EARNING_NO = 3");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            //Overtime 1 - Salaries
            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EARNING");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(",TIE_BREAKER");
            strQry.AppendLine(",EARNING_DESC");
            strQry.AppendLine(",LEAVE_PERCENTAGE");
            strQry.AppendLine(",IRP5_CODE");
            strQry.AppendLine(",EARNING_REPORT_HEADER1");
            strQry.AppendLine(",EARNING_REPORT_HEADER2");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD");
            strQry.AppendLine(",EARNING_DEL_IND)");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",'S'");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'Overtime (" + Convert.ToDouble(parDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME1_RATE"]).ToString("0.00") + ")'");
            strQry.AppendLine(",100");
            strQry.AppendLine(",IRP5_CODE");
            strQry.AppendLine(",'Overtime'");
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(Convert.ToDouble(parDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME1_RATE"]).ToString("0.00")));
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(",'N'");
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.EARNING_DEFAULT ED");
            strQry.AppendLine(" WHERE EARNING_NO = 3");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            //Overtime 2 - Wages
            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EARNING");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(",TIE_BREAKER");
            strQry.AppendLine(",EARNING_DESC");
            strQry.AppendLine(",LEAVE_PERCENTAGE");
            strQry.AppendLine(",IRP5_CODE");
            strQry.AppendLine(",EARNING_REPORT_HEADER1");
            strQry.AppendLine(",EARNING_REPORT_HEADER2");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD");
            strQry.AppendLine(",EARNING_DEL_IND)");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",'W'");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'Overtime (" + Convert.ToDouble(parDataSet.Tables["Company"].Rows[0]["OVERTIME2_RATE"]).ToString("0.00") + ")'");
            strQry.AppendLine(",100");
            strQry.AppendLine(",IRP5_CODE");
            strQry.AppendLine(",'Overtime'");
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(Convert.ToDouble(parDataSet.Tables["Company"].Rows[0]["OVERTIME2_RATE"]).ToString("0.00")));
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(",'N'");
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.EARNING_DEFAULT ED");
            strQry.AppendLine(" WHERE EARNING_NO = 4");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            //Overtime 2 - Salaries
            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EARNING");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(",TIE_BREAKER");
            strQry.AppendLine(",EARNING_DESC");
            strQry.AppendLine(",LEAVE_PERCENTAGE");
            strQry.AppendLine(",IRP5_CODE");
            strQry.AppendLine(",EARNING_REPORT_HEADER1");
            strQry.AppendLine(",EARNING_REPORT_HEADER2");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD");
            strQry.AppendLine(",EARNING_DEL_IND)");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",'S'");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(",1");

            strQry.AppendLine(",'Overtime (" + Convert.ToDouble(parDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME2_RATE"]).ToString("0.00") + ")'");

            strQry.AppendLine(",100");
            strQry.AppendLine(",IRP5_CODE");
            strQry.AppendLine(",'Overtime'");
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(Convert.ToDouble(parDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME2_RATE"]).ToString("0.00")));

            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(",'N'");
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.EARNING_DEFAULT ED");
            strQry.AppendLine(" WHERE EARNING_NO = 4");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            //Overtime 3 - Wages
            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EARNING");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(",TIE_BREAKER");
            strQry.AppendLine(",EARNING_DESC");
            strQry.AppendLine(",LEAVE_PERCENTAGE");
            strQry.AppendLine(",IRP5_CODE");
            strQry.AppendLine(",EARNING_REPORT_HEADER1");
            strQry.AppendLine(",EARNING_REPORT_HEADER2");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD");
            strQry.AppendLine(",EARNING_DEL_IND)");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",'W'");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(",1");
            strQry.AppendLine(",'Overtime (" + Convert.ToDouble(parDataSet.Tables["Company"].Rows[0]["OVERTIME3_RATE"]).ToString("0.00") + ")'");
            strQry.AppendLine(",100");
            strQry.AppendLine(",IRP5_CODE");
            strQry.AppendLine(",'Overtime'");
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(Convert.ToDouble(parDataSet.Tables["Company"].Rows[0]["OVERTIME3_RATE"]).ToString("0.00")));
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(",'N'");
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.EARNING_DEFAULT ED");
            strQry.AppendLine(" WHERE EARNING_NO = 5");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            //Overtime 3 - Salaries
            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EARNING");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(",TIE_BREAKER");
            strQry.AppendLine(",EARNING_DESC");
            strQry.AppendLine(",LEAVE_PERCENTAGE");
            strQry.AppendLine(",IRP5_CODE");
            strQry.AppendLine(",EARNING_REPORT_HEADER1");
            strQry.AppendLine(",EARNING_REPORT_HEADER2");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD");
            strQry.AppendLine(",EARNING_DEL_IND)");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",'S'");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(",1");

            strQry.AppendLine(",'Overtime (" + Convert.ToDouble(parDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME3_RATE"]).ToString("0.00") + ")'");

            strQry.AppendLine(",100");
            strQry.AppendLine(",IRP5_CODE");
            strQry.AppendLine(",'Overtime'");

            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(Convert.ToDouble(parDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME3_RATE"]).ToString("0.00")));

            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(",'N'");
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.EARNING_DEFAULT ED");
            strQry.AppendLine(" WHERE EARNING_NO = 5");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), int64CompanyNo);

            //Print Header
            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.COMPANY_PRINT_HEADER");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PRINT_HEADER_NO");
            strQry.AppendLine(",USED_IND");
            strQry.AppendLine(",ALIGNMENT");
            strQry.AppendLine(",P_POS_X");
            strQry.AppendLine(",P_POS_Y");
            strQry.AppendLine(",L_POS_X");
            strQry.AppendLine(",L_POS_Y");
            strQry.AppendLine(",BOLD_IND");
            strQry.AppendLine(",UNDERLINE_IND");
            strQry.AppendLine(",ITALIC_IND");
            strQry.AppendLine(",PRINT_HEADER_TEXT");
            strQry.AppendLine(",FIXED_WIDTH");
            strQry.AppendLine(",IMAGE_WIDTH");
            strQry.AppendLine(",IMAGE_HEIGHT)");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(int64CompanyNo.ToString());
            strQry.AppendLine(",PRINT_HEADER_NO");
            strQry.AppendLine(",USED_IND");
            strQry.AppendLine(",ALIGNMENT");
            strQry.AppendLine(",P_POS_X");
            strQry.AppendLine(",P_POS_Y");
            strQry.AppendLine(",L_POS_X");
            strQry.AppendLine(",L_POS_Y");
            strQry.AppendLine(",BOLD_IND");
            strQry.AppendLine(",UNDERLINE_IND");
            strQry.AppendLine(",ITALIC_IND");
            strQry.AppendLine(",PRINT_HEADER_TEXT");
            strQry.AppendLine(",FIXED_WIDTH");
            strQry.AppendLine(",IMAGE_WIDTH");
            strQry.AppendLine(",IMAGE_HEIGHT");
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.PRINT_HEADER_DEFAULT");

            clsDBConnectionObjects.Execute_SQLCommand_Transaction(strQryArray, int64CompanyNo);

            parDataSet.Dispose();
            parDataSet = null;

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + int64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            return int64CompanyNo;
        }

        public void Update_Record(Int64 parInt64CompanyNo, Int64 parint64CurrentUserNo, byte[] parbyteDataSet)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            StringBuilder strQry = new StringBuilder();

            if (parDataSet.Tables["EfilingPeriod"].Rows.Count > 0)
            {
                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EFILING");
                strQry.AppendLine("(EFILING_PERIOD ");
                strQry.AppendLine(",EFILING_COMPANY_CHECK_USER_NO)");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine("('" + Convert.ToDateTime(parDataSet.Tables["EfilingPeriod"].Rows[0]["EFILING_PERIOD"]).ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine("," + parDataSet.Tables["EfilingPeriod"].Rows[0]["EFILING_COMPANY_CHECK_USER_NO"].ToString() + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            }
           
            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" COMPANY_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["COMPANY_DESC"].ToString()));
            strQry.AppendLine(",DATE_FORMAT = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["DATE_FORMAT"].ToString()));
            strQry.AppendLine(",DYNAMIC_UPLOAD_KEY = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["DYNAMIC_UPLOAD_KEY"].ToString()));

            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["Company"].Rows[0]["COMPANY_NO"].ToString());

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            if (parDataSet.Tables["Company"].Rows[0]["DYNAMIC_UPLOAD_KEY"].ToString() == "")
            {
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY");
                strQry.AppendLine(" SET NO_EDIT_IND = 'N' ");
               
                strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["Company"].Rows[0]["COMPANY_NO"].ToString());
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            }

            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.COMPANY");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" COMPANY_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["COMPANY_DESC"].ToString()));
            strQry.AppendLine(",POST_ADDR_LINE1 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["POST_ADDR_LINE1"].ToString()));
            strQry.AppendLine(",POST_ADDR_LINE2 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["POST_ADDR_LINE2"].ToString()));
            strQry.AppendLine(",POST_ADDR_LINE3 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["POST_ADDR_LINE3"].ToString()));
            strQry.AppendLine(",POST_ADDR_LINE4 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["POST_ADDR_LINE4"].ToString()));
            strQry.AppendLine(",POST_ADDR_CODE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["POST_ADDR_CODE"].ToString()));

            strQry.AppendLine(",RES_UNIT_NUMBER = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["RES_UNIT_NUMBER"].ToString()));
            strQry.AppendLine(",RES_COMPLEX = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["RES_COMPLEX"].ToString()));
            strQry.AppendLine(",RES_STREET_NUMBER = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["RES_STREET_NUMBER"].ToString()));
            strQry.AppendLine(",RES_STREET_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["RES_STREET_NAME"].ToString()));
            strQry.AppendLine(",RES_SUBURB = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["RES_SUBURB"].ToString()));
            strQry.AppendLine(",RES_CITY = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["RES_CITY"].ToString()));
            strQry.AppendLine(",RES_ADDR_CODE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["RES_ADDR_CODE"].ToString()));

            strQry.AppendLine(",TAX_REF_NO = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["TAX_REF_NO"].ToString()));

            strQry.AppendLine(",UIF_REF_NO = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["UIF_REF_NO"].ToString()));
            strQry.AppendLine(",VAT_NO = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["VAT_NO"].ToString()));
            strQry.AppendLine(",PAYE_REF_NO = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["PAYE_REF_NO"].ToString()));

            strQry.AppendLine(",TRADE_CLASSIFICATION_CODE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["TRADE_CLASSIFICATION_CODE"].ToString()));

            strQry.AppendLine(",GENERATE_EMPLOYEE_NUMBER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["GENERATE_EMPLOYEE_NUMBER_IND"].ToString()));
            strQry.AppendLine(",OVERTIME1_RATE = " + parDataSet.Tables["Company"].Rows[0]["OVERTIME1_RATE"]);
            strQry.AppendLine(",OVERTIME2_RATE = " + parDataSet.Tables["Company"].Rows[0]["OVERTIME2_RATE"]);
            strQry.AppendLine(",OVERTIME3_RATE = " + parDataSet.Tables["Company"].Rows[0]["OVERTIME3_RATE"]);

            strQry.AppendLine(",SALARY_OVERTIME1_RATE = " + parDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME1_RATE"]);
            strQry.AppendLine(",SALARY_OVERTIME2_RATE = " + parDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME2_RATE"]);
            strQry.AppendLine(",SALARY_OVERTIME3_RATE = " + parDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME3_RATE"]);

            strQry.AppendLine(",DIPLOMATIC_INDEMNITY_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["DIPLOMATIC_INDEMNITY_IND"].ToString()));
            strQry.AppendLine(",SALARY_DOUBLE_CHEQUE_BIRTHDAY_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["SALARY_DOUBLE_CHEQUE_BIRTHDAY_IND"].ToString()));
            strQry.AppendLine(",TEL_WORK = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["TEL_WORK"].ToString()));
            strQry.AppendLine(",TEL_FAX = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["TEL_FAX"].ToString()));

            strQry.AppendLine(",EFILING_NAMES = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["EFILING_NAMES"].ToString()));
            strQry.AppendLine(",EFILING_CONTACT_NO = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["EFILING_CONTACT_NO"].ToString()));
            strQry.AppendLine(",EFILING_EMAIL = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["EFILING_EMAIL"].ToString()));
         
            strQry.AppendLine(",SDL_EXEMPT_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["SDL_EXEMPT_IND"].ToString()));
            strQry.AppendLine(",SDL_REF_NO = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["SDL_REF_NO"].ToString()));
            strQry.AppendLine(",BANK_NO = " + parDataSet.Tables["Company"].Rows[0]["BANK_NO"].ToString());
            
            strQry.AppendLine(",FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["FILE_NAME"].ToString()));

            if (parDataSet.Tables["Company"].Rows[0]["BRANCH_CODE"] != System.DBNull.Value)
            {
                strQry.AppendLine(",BRANCH_CODE = " + parDataSet.Tables["Company"].Rows[0]["BRANCH_CODE"].ToString());
            }
            else
            {
                strQry.AppendLine(",BRANCH_CODE = Null");
            }

            if (parDataSet.Tables["Company"].Rows[0]["ACCOUNT_NO"] != System.DBNull.Value)
            {
                strQry.AppendLine(",ACCOUNT_NO = " + parDataSet.Tables["Company"].Rows[0]["ACCOUNT_NO"].ToString());
            }
            else
            {
                strQry.AppendLine(",ACCOUNT_NO = Null");
            }

            if (parDataSet.Tables["Company"].Rows[0]["ACCOUNT_REFERENCE_NO"] != System.DBNull.Value)
            {
                strQry.AppendLine(",ACCOUNT_REFERENCE_NO = " + parDataSet.Tables["Company"].Rows[0]["ACCOUNT_REFERENCE_NO"].ToString());
            }
            else
            {
                strQry.AppendLine(",ACCOUNT_REFERENCE_NO = Null");
            }

            //ELR - 2014-08-24
            if (parDataSet.Tables["Company"].Rows[0]["SIC7_GROUP_CODE"].ToString() != "")
            {
                strQry.AppendLine(",SIC7_GROUP_CODE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["SIC7_GROUP_CODE"].ToString()));
            }
            else
            {
                strQry.AppendLine(",SIC7_GROUP_CODE = Null");
            }

            //2017-01-13
            strQry.AppendLine(",LEAVE_BEGIN_MONTH = " + parDataSet.Tables["Company"].Rows[0]["LEAVE_BEGIN_MONTH"].ToString());
           
            strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);
            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["Company"].Rows[0]["COMPANY_NO"].ToString());

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            
            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" LOCK_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["LOCK_IND"].ToString()));
            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["Company"].Rows[0]["COMPANY_NO"].ToString());
           
            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            
            //for (int intRow = 0; intRow < parDataSet.Tables["CompanyHeader"].Rows.Count; intRow++)
            //{
            //    strQry.Clear();
            //    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.COMPANY_PRINT_HEADER");
            //    strQry.AppendLine(" SET ");
            //    strQry.AppendLine(" USED_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["CompanyHeader"].Rows[intRow]["USED_IND"].ToString());
            //    strQry.AppendLine(",ALIGNMENT = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["CompanyHeader"].Rows[intRow]["ALIGNMENT"].ToString());
            //    strQry.AppendLine(",P_POS_X = " + parDataSet.Tables["CompanyHeader"].Rows[intRow]["P_POS_X"].ToString();
            //    strQry.AppendLine(",P_POS_Y = " + parDataSet.Tables["CompanyHeader"].Rows[intRow]["P_POS_Y"].ToString();
            //    strQry.AppendLine(",L_POS_X = " + parDataSet.Tables["CompanyHeader"].Rows[intRow]["L_POS_X"].ToString();
            //    strQry.AppendLine(",L_POS_Y = " + parDataSet.Tables["CompanyHeader"].Rows[intRow]["L_POS_Y"].ToString();
            //    strQry.AppendLine(",BOLD_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["CompanyHeader"].Rows[intRow]["BOLD_IND"].ToString());
            //    strQry.AppendLine(",UNDERLINE_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["CompanyHeader"].Rows[intRow]["UNDERLINE_IND"].ToString());
            //    strQry.AppendLine(",ITALIC_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["CompanyHeader"].Rows[intRow]["ITALIC_IND"].ToString());
            //    strQry.AppendLine(",PRINT_HEADER_TEXT = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["CompanyHeader"].Rows[intRow]["PRINT_HEADER_TEXT"].ToString());

            //    if (parDataSet.Tables["CompanyHeader"].Rows[intRow]["PRINT_HEADER_NO"].ToString() == "2")
            //    {
            //        strQry.AppendLine(",IMAGE_WIDTH = " + parDataSet.Tables["CompanyHeader"].Rows[intRow]["IMAGE_WIDTH"].ToString();
            //        strQry.AppendLine(",IMAGE_HEIGHT = " + parDataSet.Tables["CompanyHeader"].Rows[intRow]["IMAGE_HEIGHT"].ToString();

            //    }

            //    strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["CompanyHeader"].Rows[intRow]["COMPANY_NO"].ToString();
            //    strQry.AppendLine(" AND PRINT_HEADER_NO = " + parDataSet.Tables["CompanyHeader"].Rows[intRow]["PRINT_HEADER_NO"].ToString();

            //    clsDBConnectionObjects.Execute_SQLCommand(strQry, parInt64CompanyNo);
            //}

            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EARNING");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" EARNING_DESC = " + "'Overtime (" + Convert.ToDouble(parDataSet.Tables["Company"].Rows[0]["OVERTIME1_RATE"]).ToString("0.00") + ")'");
            strQry.AppendLine(",EARNING_REPORT_HEADER2 = " + clsDBConnectionObjects.Text2DynamicSQL(Convert.ToDouble(parDataSet.Tables["Company"].Rows[0]["OVERTIME1_RATE"]).ToString("0.00")));
            strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);
            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["Company"].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" AND EARNING_NO = 3");
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EARNING");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" EARNING_DESC = " + "'Overtime (" + Convert.ToDouble(parDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME1_RATE"]).ToString("0.00") + ")'");
            strQry.AppendLine(",EARNING_REPORT_HEADER2 = " + clsDBConnectionObjects.Text2DynamicSQL(Convert.ToDouble(parDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME1_RATE"]).ToString("0.00")));
            strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);
            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["Company"].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
            strQry.AppendLine(" AND EARNING_NO = 3");
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EARNING");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" EARNING_DESC = " + "'Overtime (" + Convert.ToDouble(parDataSet.Tables["Company"].Rows[0]["OVERTIME2_RATE"]).ToString("0.00") + ")'");
            strQry.AppendLine(",EARNING_REPORT_HEADER2 = " + clsDBConnectionObjects.Text2DynamicSQL(Convert.ToDouble(parDataSet.Tables["Company"].Rows[0]["OVERTIME2_RATE"]).ToString("0.00")));
            strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);
            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["Company"].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" AND EARNING_NO = 4");
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EARNING");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" EARNING_DESC = " + "'Overtime (" + Convert.ToDouble(parDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME2_RATE"]).ToString("0.00") + ")'");
            strQry.AppendLine(",EARNING_REPORT_HEADER2 = " + clsDBConnectionObjects.Text2DynamicSQL(Convert.ToDouble(parDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME2_RATE"]).ToString("0.00")));
            strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);
            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["Company"].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
            strQry.AppendLine(" AND EARNING_NO = 4");
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EARNING");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" EARNING_DESC = " + "'Overtime (" + Convert.ToDouble(parDataSet.Tables["Company"].Rows[0]["OVERTIME3_RATE"]).ToString("0.00") + ")'");
            strQry.AppendLine(",EARNING_REPORT_HEADER2 = " + clsDBConnectionObjects.Text2DynamicSQL(Convert.ToDouble(parDataSet.Tables["Company"].Rows[0]["OVERTIME3_RATE"]).ToString("0.00")));
            strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);
            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["Company"].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" AND EARNING_NO = 5");
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EARNING");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" EARNING_DESC = " + "'Overtime (" + Convert.ToDouble(parDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME3_RATE"]).ToString("0.00") + ")'");
            strQry.AppendLine(",EARNING_REPORT_HEADER2 = " + clsDBConnectionObjects.Text2DynamicSQL(Convert.ToDouble(parDataSet.Tables["Company"].Rows[0]["SALARY_OVERTIME3_RATE"]).ToString("0.00")));
            strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);
            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["Company"].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
            strQry.AppendLine(" AND EARNING_NO = 5");
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            parDataSet.Dispose();
            parDataSet = null;
        }

        public void Delete_Record(Int64 parint64CompanyNo)
        {
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" DROP DATABASE InteractPayroll_" + parint64CompanyNo.ToString("00000"));

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.COMPANY_LINK ");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
        }
    }
}
