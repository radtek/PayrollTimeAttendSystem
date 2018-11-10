using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using InteractPayroll;

namespace InteractPayrollClient
{                         
    public class busClockToEmployeeLink
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busClockToEmployeeLink()
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

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" DEVICE_NO");
            strQry.AppendLine(",DEVICE_DESC");
            strQry.AppendLine(",DEVICE_USAGE");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE D");

            strQry.AppendLine(" ORDER BY ");
            
            strQry.AppendLine(" DEVICE_DESC");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Device");
            
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" C.COMPANY_NO");
            strQry.AppendLine(",C.COMPANY_DESC");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.COMPANY C");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS  UC ");
                strQry.AppendLine(" ON UC.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND UC.COMPANY_NO = C.COMPANY_NO ");

                //2013-07-10
                strQry.AppendLine(" AND UC.COMPANY_ACCESS_IND = 'A'");
            }

            strQry.AppendLine(" ORDER BY C.COMPANY_DESC");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Company");

            if (DataSet.Tables["Company"].Rows.Count > 0)
            {
                byte[] bytTempCompress = Get_Company_Records(Convert.ToInt64(DataSet.Tables["Company"].Rows[0]["COMPANY_NO"]));

                DataSet TempDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(bytTempCompress);
                DataSet.Merge(TempDataSet);
            }

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_Company_Records(Int64 parint64CompanyNo)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" DPCL.DEVICE_NO");
            strQry.AppendLine(",E.COMPANY_NO");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",ISNULL(E.USE_EMPLOYEE_NO_IND,'N') AS USE_EMPLOYEE_NO_IND ");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
            
            //2017-06-22
            strQry.AppendLine(",HAS_FINGERPRINTS_AT_SERVER_IND = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN NOT MAX(EFTS.FINGER_NO) IS NULL THEN 'Y'");
            
            strQry.AppendLine(" ELSE 'N'");
            strQry.AppendLine(" END ");

            //2017-06-22
            strQry.AppendLine(",HAS_FINGERPRINTS_AT_LOCAL_IND = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN NOT MAX(EFTL.FINGER_NO) IS NULL THEN 'Y'");

            strQry.AppendLine(" ELSE 'N'");
            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK DPCL");
            strQry.AppendLine(" ON PC.COMPANY_NO = DPCL.COMPANY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = DPCL.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = DPCL.PAY_CATEGORY_TYPE ");

            //2017-06-02
            strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE EFTS");
            strQry.AppendLine(" ON E.COMPANY_NO = EFTS.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EFTS.EMPLOYEE_NO ");
         
            strQry.AppendLine(" AND NOT EFTS.CREATION_DATETIME IS NULL ");

            //2017-06-02
            strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE EFTL");
            strQry.AppendLine(" ON E.COMPANY_NO = EFTL.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EFTL.EMPLOYEE_NO ");

            strQry.AppendLine(" AND EFTL.CREATION_DATETIME IS NULL ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND ISNULL(E.NOT_ACTIVE_IND,'N') <> 'Y'");

            strQry.AppendLine(" GROUP BY ");

            strQry.AppendLine(" DPCL.DEVICE_NO");
            strQry.AppendLine(",E.COMPANY_NO");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",ISNULL(E.USE_EMPLOYEE_NO_IND,'N')");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
 
            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" DDL.DEVICE_NO");
            strQry.AppendLine(",E.COMPANY_NO");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",ISNULL(E.USE_EMPLOYEE_NO_IND,'N') AS USE_EMPLOYEE_NO_IND ");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
            //2017-06-22
            strQry.AppendLine(",HAS_FINGERPRINTS_AT_SERVER_IND = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN NOT MAX(EFTS.FINGER_NO) IS NULL THEN 'Y'");

            strQry.AppendLine(" ELSE 'N'");
            strQry.AppendLine(" END ");

            //2017-06-22
            strQry.AppendLine(",HAS_FINGERPRINTS_AT_LOCAL_IND = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN NOT MAX(EFTL.FINGER_NO) IS NULL THEN 'Y'");

            strQry.AppendLine(" ELSE 'N'");
            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK DDL");
            strQry.AppendLine(" ON PC.COMPANY_NO = DDL.COMPANY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = DDL.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = DDL.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND E.DEPARTMENT_NO = DDL.DEPARTMENT_NO ");

            //2017-06-02
            strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE EFTS");
            strQry.AppendLine(" ON E.COMPANY_NO = EFTS.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EFTS.EMPLOYEE_NO ");

            strQry.AppendLine(" AND NOT EFTS.CREATION_DATETIME IS NULL ");

            //2017-06-02
            strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE EFTL");
            strQry.AppendLine(" ON E.COMPANY_NO = EFTL.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EFTL.EMPLOYEE_NO ");

            strQry.AppendLine(" AND EFTL.CREATION_DATETIME IS NULL ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND ISNULL(E.NOT_ACTIVE_IND,'N') <> 'Y'");

            strQry.AppendLine(" GROUP BY");

            strQry.AppendLine(" DDL.DEVICE_NO");
            strQry.AppendLine(",E.COMPANY_NO");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",ISNULL(E.USE_EMPLOYEE_NO_IND,'N')");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" UNION ");
            
            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" DGL.DEVICE_NO");
            strQry.AppendLine(",E.COMPANY_NO");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",ISNULL(E.USE_EMPLOYEE_NO_IND,'N') AS USE_EMPLOYEE_NO_IND ");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
            //2017-06-22
            strQry.AppendLine(",HAS_FINGERPRINTS_AT_SERVER_IND = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN NOT MAX(EFTS.FINGER_NO) IS NULL THEN 'Y'");

            strQry.AppendLine(" ELSE 'N'");
            strQry.AppendLine(" END ");

            //2017-06-22
            strQry.AppendLine(",HAS_FINGERPRINTS_AT_LOCAL_IND = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN NOT MAX(EFTL.FINGER_NO) IS NULL THEN 'Y'");

            strQry.AppendLine(" ELSE 'N'");
            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.GROUP_EMPLOYEE_LINK GEL");
            strQry.AppendLine(" ON PC.COMPANY_NO = GEL.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = GEL.EMPLOYEE_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = GEL.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = GEL.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.DEVICE_GROUP_LINK DGL");
            strQry.AppendLine(" ON PC.COMPANY_NO = DGL.COMPANY_NO ");
            strQry.AppendLine(" AND GEL.GROUP_NO = DGL.GROUP_NO ");

            //2017-06-02
            strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE EFTS");
            strQry.AppendLine(" ON E.COMPANY_NO = EFTS.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EFTS.EMPLOYEE_NO ");

            strQry.AppendLine(" AND NOT EFTS.CREATION_DATETIME IS NULL ");

            //2017-06-02
            strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE EFTL");
            strQry.AppendLine(" ON E.COMPANY_NO = EFTL.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EFTL.EMPLOYEE_NO ");

            strQry.AppendLine(" AND EFTL.CREATION_DATETIME IS NULL ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND ISNULL(E.NOT_ACTIVE_IND,'N') <> 'Y'");

            strQry.AppendLine(" GROUP BY ");

            strQry.AppendLine(" DGL.DEVICE_NO");
            strQry.AppendLine(",E.COMPANY_NO");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",ISNULL(E.USE_EMPLOYEE_NO_IND,'N')");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");
           
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
    }
}
