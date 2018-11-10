using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using InteractPayroll;

namespace InteractPayrollClient
{
    public class busEmployeeClient
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busEmployeeClient()
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
            strQry.AppendLine(" IDENTIFY_THRESHOLD_VALUE");
            strQry.AppendLine(",VERIFY_THRESHOLD_VALUE");
           
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.FINGERPRINT_IDENTIFY_VERIFY_THRESHOLD");

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
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" READ_OPTION_NO");
            strQry.AppendLine(",READ_OPTION_DESC");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.READ_OPTION");
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" READ_OPTION_NO");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ReadOption");

            strQry.Clear();
            strQry.AppendLine(" SELECT ");

            strQry.AppendLine("'" + clsDBConnectionObjects.Get_ClientConnectionString().Replace("InteractPayrollClient_Debug", "InteractPayrollClient") + "' AS DB_CONNECTION_STRING");

            //Will Always be Only 1 Row
            strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");
            strQry.AppendLine(" WHERE TABLE_NAME = 'COMPANY'");
            strQry.AppendLine(" AND COLUMN_NAME = 'COMPANY_NO'");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Connection");

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" PC.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.COMPANY C");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON C.COMPANY_NO = PC.COMPANY_NO");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA");
                strQry.AppendLine(" ON UCA.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND C.COMPANY_NO = UCA.COMPANY_NO");
            }

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Temp");

            DataSet.Tables.Add("PayrollType");
            DataTable PayrollTypeDataTable = new DataTable("PayrollType");
            DataSet.Tables["PayrollType"].Columns.Add("PAYROLL_TYPE_DESC", typeof(String));

            DataView PayrollTypeDataView = new DataView(DataSet.Tables["Temp"],
             "PAY_CATEGORY_TYPE = 'W'",
             "",
             DataViewRowState.CurrentRows);

            if (PayrollTypeDataView.Count > 0)
            {
                DataRow drDataRow = DataSet.Tables["PayrollType"].NewRow();

                drDataRow["PAYROLL_TYPE_DESC"] = "Wages";

                DataSet.Tables["PayrollType"].Rows.Add(drDataRow);
            }

            PayrollTypeDataView = null;
            PayrollTypeDataView = new DataView(DataSet.Tables["Temp"],
                "PAY_CATEGORY_TYPE = 'S'",
                "",
                DataViewRowState.CurrentRows);

            if (PayrollTypeDataView.Count > 0)
            {
                DataRow drDataRow = DataSet.Tables["PayrollType"].NewRow();

                drDataRow["PAYROLL_TYPE_DESC"] = "Salaries";

                DataSet.Tables["PayrollType"].Rows.Add(drDataRow);
            }

            PayrollTypeDataView = null;
            PayrollTypeDataView = new DataView(DataSet.Tables["Temp"],
                "PAY_CATEGORY_TYPE = 'T'",
                "",
                DataViewRowState.CurrentRows);

            if (PayrollTypeDataView.Count > 0)
            {
                DataRow drDataRow = DataSet.Tables["PayrollType"].NewRow();

                drDataRow["PAYROLL_TYPE_DESC"] = "Time Attendance";

                DataSet.Tables["PayrollType"].Rows.Add(drDataRow);
            }

            DataSet.AcceptChanges();
                        
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
                byte[] bytTempCompress = Get_Company_Records_New(Convert.ToInt64(DataSet.Tables["Company"].Rows[0]["COMPANY_NO"]), strSoftwareToUse);

                DataSet TempDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(bytTempCompress);
                DataSet.Merge(TempDataSet);
            }

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_Company_Records(Int64 parint64CompanyNo, string parstrSoftwareToUse)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PAY_CATEGORY_DESC");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PAY_CATEGORY_DESC");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PayCategory");

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" EMP.COMPANY_NO");
            strQry.AppendLine(",EMP.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EMP.EMPLOYEE_NO");
            strQry.AppendLine(",EMP.EMPLOYEE_CODE");
            //2017-02-11
            strQry.AppendLine(",EMP.EMPLOYEE_3RD_PARTY_CODE");
            strQry.AppendLine(",EMP.EMPLOYEE_NAME");
            strQry.AppendLine(",EMP.EMPLOYEE_SURNAME");
            //2014-08-16
            strQry.AppendLine(",EMP.EMPLOYEE_PIN");

            strQry.AppendLine(",D.DEPARTMENT_DESC");
            strQry.AppendLine(",EMP.EMPLOYEE_RFID_CARD_NO");
            strQry.AppendLine(",READ_OPTION_NO = ");
            
            strQry.AppendLine(" CASE ");
            //Has Fingerprint
            strQry.AppendLine(" WHEN NOT TEMPLATE.COMPANY_NO IS NULL THEN");

            strQry.AppendLine(" CASE ");
            strQry.AppendLine(" WHEN NOT EMP.EMPLOYEE_RFID_CARD_NO IS NULL THEN 3");
            strQry.AppendLine(" WHEN USE_EMPLOYEE_NO_IND = 'Y' THEN 4");
            strQry.AppendLine(" ELSE 1");
            strQry.AppendLine(" END ");
            //No Fingerprint
            strQry.AppendLine(" ELSE");

            strQry.AppendLine(" CASE ");
            strQry.AppendLine(" WHEN NOT EMP.EMPLOYEE_RFID_CARD_NO IS NULL THEN 2");
            strQry.AppendLine(" WHEN USE_EMPLOYEE_NO_IND = 'Y' AND NOT (EMP.EMPLOYEE_PIN IS NULL OR EMP.EMPLOYEE_PIN = '') THEN 5");
            strQry.AppendLine(" ELSE 0");
            strQry.AppendLine(" END ");

            strQry.AppendLine(" END ");
            
            strQry.AppendLine(",EMP.EMPLOYEE_LAST_RUNDATE");
            strQry.AppendLine(",EMP.USE_EMPLOYEE_NO_IND");

            strQry.AppendLine(" FROM ");

            strQry.AppendLine(" (SELECT ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            //2017-02-11
            strQry.AppendLine(",E.EMPLOYEE_3RD_PARTY_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            //2014-08-16
            strQry.AppendLine(",E.EMPLOYEE_PIN");
            strQry.AppendLine(",E.DEPARTMENT_NO");
            strQry.AppendLine(",ERC.EMPLOYEE_RFID_CARD_NO");
            strQry.AppendLine(",E.EMPLOYEE_LAST_RUNDATE");
            strQry.AppendLine(",E.USE_EMPLOYEE_NO_IND");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

            strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_RFID_CARD ERC");
            strQry.AppendLine(" ON E.COMPANY_NO = ERC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = ERC.EMPLOYEE_NO ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND ISNULL(E.NOT_ACTIVE_IND,'N') <> 'Y') AS EMP");

            strQry.AppendLine(" LEFT JOIN ");

            //Errol Changed 2011-01-30
            strQry.AppendLine(" (SELECT DISTINCT");
            strQry.AppendLine(" EFT.COMPANY_NO");
            strQry.AppendLine(",EFT.EMPLOYEE_NO");
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE EFT");
            
            strQry.AppendLine(" WHERE EFT.COMPANY_NO = " + parint64CompanyNo.ToString() + ") AS TEMPLATE");

            strQry.AppendLine(" ON EMP.COMPANY_NO = TEMPLATE.COMPANY_NO ");
            strQry.AppendLine(" AND EMP.EMPLOYEE_NO = TEMPLATE.EMPLOYEE_NO ");

            strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEPARTMENT D");
            strQry.AppendLine(" ON EMP.COMPANY_NO = D.COMPANY_NO ");
            strQry.AppendLine(" AND EMP.PAY_CATEGORY_TYPE = D.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EMP.DEPARTMENT_NO = D.DEPARTMENT_NO ");

            strQry.AppendLine(" WHERE EMP.COMPANY_NO = " + parint64CompanyNo.ToString());

            strQry.AppendLine(" ORDER BY EMP.EMPLOYEE_CODE");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
           
            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "EmployeePayCategory");

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",FINGER_NO");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND NOT FINGER_TEMPLATE IS NULL");
            
            strQry.AppendLine(" ORDER BY FINGER_NO");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "EmployeeFingerTemplate");
         
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_Company_Records_New(Int64 parint64CompanyNo, string parstrSoftwareToUse)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PAY_CATEGORY_DESC");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PAY_CATEGORY_DESC");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PayCategory");

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" EMP.COMPANY_NO");
            strQry.AppendLine(",EMP.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EMP.EMPLOYEE_NO");
            strQry.AppendLine(",EMP.EMPLOYEE_CODE");
            //2017-02-11
            strQry.AppendLine(",EMP.EMPLOYEE_3RD_PARTY_CODE");
            strQry.AppendLine(",EMP.EMPLOYEE_NAME");
            strQry.AppendLine(",EMP.EMPLOYEE_SURNAME");
            //2014-08-16
            strQry.AppendLine(",EMP.EMPLOYEE_PIN");

            strQry.AppendLine(",D.DEPARTMENT_DESC");
            strQry.AppendLine(",EMP.EMPLOYEE_RFID_CARD_NO");
            strQry.AppendLine(",READ_OPTION_NO = ");

            strQry.AppendLine(" CASE ");
            //Has Fingerprint
            strQry.AppendLine(" WHEN NOT TEMPLATE.COMPANY_NO IS NULL THEN");

            strQry.AppendLine(" CASE ");
            strQry.AppendLine(" WHEN NOT EMP.EMPLOYEE_RFID_CARD_NO IS NULL THEN 3");
            strQry.AppendLine(" WHEN USE_EMPLOYEE_NO_IND = 'Y' THEN 4");
            strQry.AppendLine(" ELSE 1");
            strQry.AppendLine(" END ");
            //No Fingerprint
            strQry.AppendLine(" ELSE");

            strQry.AppendLine(" CASE ");
            strQry.AppendLine(" WHEN NOT EMP.EMPLOYEE_RFID_CARD_NO IS NULL THEN 2");
            strQry.AppendLine(" WHEN USE_EMPLOYEE_NO_IND = 'Y' AND NOT (EMP.EMPLOYEE_PIN IS NULL OR EMP.EMPLOYEE_PIN = '') THEN 5");
            strQry.AppendLine(" ELSE 0");
            strQry.AppendLine(" END ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",EMP.EMPLOYEE_LAST_RUNDATE");
            strQry.AppendLine(",EMP.USE_EMPLOYEE_NO_IND");

            strQry.AppendLine(" FROM ");

            strQry.AppendLine(" (SELECT ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            //2017-02-11
            strQry.AppendLine(",E.EMPLOYEE_3RD_PARTY_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            //2014-08-16
            strQry.AppendLine(",E.EMPLOYEE_PIN");
            strQry.AppendLine(",E.DEPARTMENT_NO");
            strQry.AppendLine(",ERC.EMPLOYEE_RFID_CARD_NO");
            strQry.AppendLine(",E.EMPLOYEE_LAST_RUNDATE");
            strQry.AppendLine(",E.USE_EMPLOYEE_NO_IND");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

            strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_RFID_CARD ERC");
            strQry.AppendLine(" ON E.COMPANY_NO = ERC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = ERC.EMPLOYEE_NO ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND ISNULL(E.NOT_ACTIVE_IND,'N') <> 'Y') AS EMP");

            strQry.AppendLine(" LEFT JOIN ");

            //Errol Changed 2011-01-30
            strQry.AppendLine(" (SELECT DISTINCT");
            strQry.AppendLine(" EFT.COMPANY_NO");
            strQry.AppendLine(",EFT.EMPLOYEE_NO");
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE EFT");

            strQry.AppendLine(" WHERE EFT.COMPANY_NO = " + parint64CompanyNo.ToString() + ") AS TEMPLATE");

            strQry.AppendLine(" ON EMP.COMPANY_NO = TEMPLATE.COMPANY_NO ");
            strQry.AppendLine(" AND EMP.EMPLOYEE_NO = TEMPLATE.EMPLOYEE_NO ");

            strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEPARTMENT D");
            strQry.AppendLine(" ON EMP.COMPANY_NO = D.COMPANY_NO ");
            strQry.AppendLine(" AND EMP.PAY_CATEGORY_TYPE = D.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EMP.DEPARTMENT_NO = D.DEPARTMENT_NO ");

            strQry.AppendLine(" WHERE EMP.COMPANY_NO = " + parint64CompanyNo.ToString());

            strQry.AppendLine(" ORDER BY EMP.EMPLOYEE_CODE");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "EmployeePayCategory");

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",FINGER_NO");
            strQry.AppendLine(",FINGER_RESIDE_IND = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN NOT CREATION_DATETIME IS NULL ");
            //S=Server,L=Local
            strQry.AppendLine(" THEN 'S' ");

            strQry.AppendLine(" ELSE 'L' ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND NOT FINGER_TEMPLATE IS NULL");

            strQry.AppendLine(" ORDER BY FINGER_NO");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "EmployeeFingerTemplate");

            DataSet.Tables["EmployeeFingerTemplate"].Columns.Add("FINGER_TEMPLATE", typeof(System.Byte[]));

            DataSet.AcceptChanges();

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        //2014-08-16
        public void Update_Employee_New(Int64 parint64CompanyNo, Int64 parintEmployeeNo, string parstrRFIDCardNo, string parstrUseEmpNo, string parstrEmployeePin, byte[] byteCompressedDataSet)
        {
            Update_Employee(parint64CompanyNo, parintEmployeeNo, parstrRFIDCardNo, parstrUseEmpNo, byteCompressedDataSet);

            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
            
            //2017-10-05 (Moved To Internet) 
            //strQry.Clear();

            //strQry.AppendLine(" UPDATE EMPLOYEE");
            //strQry.AppendLine(" SET EMPLOYEE_PIN = " + clsDBConnectionObjects.Text2DynamicSQL(parstrEmployeePin));
            //strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            //strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo.ToString());

            //clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
        }

        public void Update_Employee(Int64 parint64CompanyNo, Int64 parintEmployeeNo, string parstrRFIDCardNo, string parstrUseEmpNo, byte[] byteCompressedDataSet)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(byteCompressedDataSet);
            StringBuilder strQry = new StringBuilder();

            for (int intRow = 0; intRow < DataSet.Tables["EmployeeFingerTemplate"].Rows.Count; intRow++)
            {
                if (DataSet.Tables["EmployeeFingerTemplate"].Rows[0].RowState == DataRowState.Added)
                {
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
                    strQry.AppendLine(" AND FINGER_NO = " + DataSet.Tables["EmployeeFingerTemplate"].Rows[intRow]["FINGER_NO"].ToString());

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",FINGER_NO");
                    strQry.AppendLine(",FINGER_TEMPLATE)");
                    strQry.AppendLine(" VALUES ");
                    strQry.AppendLine("(" + parint64CompanyNo);
                    strQry.AppendLine("," + parintEmployeeNo);
                    strQry.AppendLine("," + DataSet.Tables["EmployeeFingerTemplate"].Rows[intRow]["FINGER_NO"].ToString());
                    strQry.AppendLine(",@FINGER_TEMPLATE) ");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString(), (byte[])DataSet.Tables["EmployeeFingerTemplate"].Rows[intRow]["FINGER_TEMPLATE"], "@FINGER_TEMPLATE");
                }
                else
                {
                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE_DELETE");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",FINGER_NO");
                    strQry.AppendLine(",CREATION_DATETIME)");

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",FINGER_NO");
                    strQry.AppendLine(",CREATION_DATETIME ");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo.ToString());
                    strQry.AppendLine(" AND FINGER_NO = " + DataSet.Tables["EmployeeFingerTemplate"].Rows[intRow]["FINGER_NO", DataRowVersion.Original].ToString());

                    strQry.AppendLine(" AND NOT CREATION_DATETIME IS NULL ");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo.ToString());
                    strQry.AppendLine(" AND FINGER_NO = " + DataSet.Tables["EmployeeFingerTemplate"].Rows[intRow]["FINGER_NO",DataRowVersion.Original].ToString());

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }
            }

            //2017-10-05 (Moved To Internet) 
            //if (parstrRFIDCardNo == "")
            //{
            //    strQry.Clear();
            //    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_RFID_CARD");
            //    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            //    strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo.ToString());

            //    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            //}

            //2017-10-05 (Moved To Internet) 
            //strQry.Clear();
            //strQry.AppendLine(" UPDATE EMPLOYEE");
            //strQry.AppendLine(" SET USE_EMPLOYEE_NO_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrUseEmpNo));
            //strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            //strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo.ToString());

            //clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
        }

        public void Delete_Employee(Int64 parint64CompanyNo, Int64 parint64EmployeeNo)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = new DataSet();
            string strWhere = " WHERE COMPANY_NO = " + parint64CompanyNo + " AND EMPLOYEE_NO = " + parint64EmployeeNo.ToString();
            StringBuilder strQry = new StringBuilder();

            //Delete Rows For Link Table
            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK ");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EMPLOYEE_NO_LINK = " + parint64EmployeeNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" T.TABLE_NAME");
            strQry.AppendLine(",C1.COLUMN_NAME AS C1_COLUMN_NAME");
            strQry.AppendLine(" FROM INFORMATION_SCHEMA.TABLES T");
            
            strQry.AppendLine(" INNER JOIN INFORMATION_SCHEMA.COLUMNS C1");
            strQry.AppendLine(" ON T.TABLE_NAME = C1.TABLE_NAME");

            strQry.AppendLine(" INNER JOIN INFORMATION_SCHEMA.COLUMNS C2");
            strQry.AppendLine(" ON T.TABLE_NAME = C2.TABLE_NAME");

            strQry.AppendLine(" WHERE T.TABLE_TYPE = 'BASE TABLE'");
            strQry.AppendLine(" AND C1.COLUMN_NAME = 'EMPLOYEE_NO'");
            strQry.AppendLine(" AND C2.COLUMN_NAME = 'COMPANY_NO'");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "TableColumnName");

            for (int intRow = 0; intRow < DataSet.Tables["TableColumnName"].Rows.Count; intRow++)
            {
                strQry.Clear();

                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo." + DataSet.Tables["TableColumnName"].Rows[intRow]["TABLE_NAME"].ToString() + strWhere);

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }
        }
    }
}
