using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using InteractPayroll;

namespace InteractPayrollClient
{
    public class busUserClient
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busUserClient()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(int parintCurrentUserNo, string parstrAccessInd)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = new DataSet();

            StringBuilder strQry = new StringBuilder();
            string strSoftwareToUse = "D";

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" C.COMPANY_NO");
            strQry.AppendLine(",C.COMPANY_DESC");

            strQry.AppendLine(",'" + clsDBConnectionObjects.Get_ClientConnectionString().Replace("InteractPayrollClient_Debug", "InteractPayrollClient") + "' AS DB_CONNECTION_STRING");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.COMPANY C");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Company");

            strQry.Clear();

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine("'" + clsDBConnectionObjects.Get_ClientConnectionString().Replace("InteractPayrollClient_Debug", "InteractPayrollClient") + "' AS DB_CONNECTION_STRING");


            //Will Always be Only 1 Row
            strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");
            
            strQry.AppendLine(" WHERE TABLE_NAME = 'COMPANY'");
            strQry.AppendLine(" AND COLUMN_NAME = 'COMPANY_NO'");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Connection");

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FIVT.IDENTIFY_THRESHOLD_VALUE");
            strQry.AppendLine(",FIVT.VERIFY_THRESHOLD_VALUE");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.FINGERPRINT_IDENTIFY_VERIFY_THRESHOLD FIVT");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "FingerprintThreshold");

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FINGERPRINT_SOFTWARE_IND");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.FINGERPRINT_SOFTWARE_TO_USE");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "SoftwareToUse");

            if (DataSet.Tables["SoftwareToUse"].Rows.Count > 0)
            {
                strSoftwareToUse = DataSet.Tables["SoftwareToUse"].Rows[0]["FINGERPRINT_SOFTWARE_IND"].ToString();
            }
            
            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" UI.USER_NO");
            strQry.AppendLine(",UI.USER_ID");
            strQry.AppendLine(",UI.FIRSTNAME");
            strQry.AppendLine(",UI.SURNAME");
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_ID UI");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA");
                strQry.AppendLine(" ON UI.USER_NO = UCA.USER_NO");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA1");
                strQry.AppendLine(" ON UCA.COMPANY_NO = UCA1.COMPANY_NO");
                strQry.AppendLine(" AND UCA1.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND UCA1.COMPANY_ACCESS_IND = 'A'");
            }

            strQry.AppendLine(" WHERE UI.SYSTEM_ADMINISTRATOR_IND <> 'Y'");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" UI.USER_ID");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "User");
            
            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" UFT.USER_NO");
            strQry.AppendLine(",UFT.FINGER_NO");
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE UFT");
           
            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA");
                strQry.AppendLine(" ON UFT.USER_NO = UCA.USER_NO");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA1");
                strQry.AppendLine(" ON UCA.COMPANY_NO = UCA1.COMPANY_NO");
                strQry.AppendLine(" AND UCA1.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND UCA1.COMPANY_ACCESS_IND = 'A'");
            }
  
            strQry.AppendLine(" ORDER BY UFT.FINGER_NO");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "UserFingerTemplate");
         
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_Form_Records_New(int parintCurrentUserNo, string parstrAccessInd)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = new DataSet();

            StringBuilder strQry = new StringBuilder();
            string strSoftwareToUse = "D";

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" C.COMPANY_NO");
            strQry.AppendLine(",C.COMPANY_DESC");

            strQry.AppendLine(",'" + clsDBConnectionObjects.Get_ClientConnectionString().Replace("InteractPayrollClient_Debug", "InteractPayrollClient") + "' AS DB_CONNECTION_STRING");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.COMPANY C");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Company");

            strQry.Clear();

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine("'" + clsDBConnectionObjects.Get_ClientConnectionString().Replace("InteractPayrollClient_Debug", "InteractPayrollClient") + "' AS DB_CONNECTION_STRING");


            //Will Always be Only 1 Row
            strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

            strQry.AppendLine(" WHERE TABLE_NAME = 'COMPANY'");
            strQry.AppendLine(" AND COLUMN_NAME = 'COMPANY_NO'");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Connection");

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FIVT.IDENTIFY_THRESHOLD_VALUE");
            strQry.AppendLine(",FIVT.VERIFY_THRESHOLD_VALUE");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.FINGERPRINT_IDENTIFY_VERIFY_THRESHOLD FIVT");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "FingerprintThreshold");

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FINGERPRINT_SOFTWARE_IND");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.FINGERPRINT_SOFTWARE_TO_USE");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "SoftwareToUse");

            if (DataSet.Tables["SoftwareToUse"].Rows.Count > 0)
            {
                strSoftwareToUse = DataSet.Tables["SoftwareToUse"].Rows[0]["FINGERPRINT_SOFTWARE_IND"].ToString();
            }

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" UI.USER_NO");
            strQry.AppendLine(",UI.USER_ID");
            strQry.AppendLine(",UI.FIRSTNAME");
            strQry.AppendLine(",UI.SURNAME");
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_ID UI");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA");
                strQry.AppendLine(" ON UI.USER_NO = UCA.USER_NO");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA1");
                strQry.AppendLine(" ON UCA.COMPANY_NO = UCA1.COMPANY_NO");
                strQry.AppendLine(" AND UCA1.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND UCA1.COMPANY_ACCESS_IND = 'A'");
            }

            strQry.AppendLine(" WHERE UI.SYSTEM_ADMINISTRATOR_IND <> 'Y'");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" UI.USER_ID");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "User");

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" UFT.USER_NO");
            strQry.AppendLine(",UFT.FINGER_NO");

            strQry.AppendLine(",FINGER_RESIDE_IND = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN NOT CREATION_DATETIME IS NULL ");
            //S=Server,L=Local
            strQry.AppendLine(" THEN 'S' ");

            strQry.AppendLine(" ELSE 'L' ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE UFT");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA");
                strQry.AppendLine(" ON UFT.USER_NO = UCA.USER_NO");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA1");
                strQry.AppendLine(" ON UCA.COMPANY_NO = UCA1.COMPANY_NO");
                strQry.AppendLine(" AND UCA1.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND UCA1.COMPANY_ACCESS_IND = 'A'");
            }

            strQry.AppendLine(" ORDER BY UFT.FINGER_NO");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "UserFingerTemplate");

            DataSet.Tables["UserFingerTemplate"].Columns.Add("FINGER_TEMPLATE", typeof(System.Byte[]));

            DataSet.AcceptChanges();

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public void Update_Records(Int64 parint64UserNo, byte[] byteCompressedDataSet)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(byteCompressedDataSet);

            StringBuilder strQry = new StringBuilder();

            for (int intRow = 0; intRow < DataSet.Tables["UserFingerTemplate"].Rows.Count; intRow++)
            {
                if (DataSet.Tables["UserFingerTemplate"].Rows[intRow].RowState == DataRowState.Added)
                {
                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE");
                    strQry.AppendLine(" WHERE USER_NO = " + parint64UserNo.ToString());
                    strQry.AppendLine(" AND FINGER_NO = " + DataSet.Tables["UserFingerTemplate"].Rows[intRow]["FINGER_NO"].ToString());

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    //There Should be only One - otherwise Error 
                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE");
                    strQry.AppendLine("(USER_NO");
                    strQry.AppendLine(",FINGER_NO");
                    strQry.AppendLine(",FINGER_TEMPLATE)");
                    strQry.AppendLine(" VALUES ");
                    strQry.AppendLine("(" + parint64UserNo);
                    strQry.AppendLine("," + DataSet.Tables["UserFingerTemplate"].Rows[intRow]["FINGER_NO"].ToString());
                    strQry.AppendLine(",@FINGER_TEMPLATE) ");
                    
                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString(), (byte[])DataSet.Tables["UserFingerTemplate"].Rows[intRow]["FINGER_TEMPLATE"], "@FINGER_TEMPLATE");
                }
                else
                {
                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE_DELETE");
                    strQry.AppendLine("(USER_NO");
                    strQry.AppendLine(",FINGER_NO");
                    strQry.AppendLine(",CREATION_DATETIME)");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" USER_NO");
                    strQry.AppendLine(",FINGER_NO");
                    strQry.AppendLine(",CREATION_DATETIME ");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE");

                    strQry.AppendLine(" WHERE USER_NO = " + parint64UserNo.ToString());
                    strQry.AppendLine(" AND FINGER_NO = " + DataSet.Tables["UserFingerTemplate"].Rows[intRow]["FINGER_NO", DataRowVersion.Original].ToString());

                    strQry.AppendLine(" AND NOT CREATION_DATETIME IS NULL ");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE");

                    strQry.AppendLine(" WHERE USER_NO = " + parint64UserNo.ToString());
                    strQry.AppendLine(" AND FINGER_NO = " + DataSet.Tables["UserFingerTemplate"].Rows[intRow]["FINGER_NO",DataRowVersion.Original].ToString());

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }
            }
        }
    }
}
