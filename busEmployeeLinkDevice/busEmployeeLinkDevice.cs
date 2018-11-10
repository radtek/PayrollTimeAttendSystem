using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using InteractPayroll;

namespace InteractPayrollClient
{
    public class busEmployeeLinkDevice
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busEmployeeLinkDevice()
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
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE");
            
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
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND ISNULL(E.NOT_ACTIVE_IND,'N') <> 'Y'");
            
            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");
            
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" DEVICE_NO");
            strQry.AppendLine(",COMPANY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_EMPLOYEE_LINK ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "EmployeeChosen");
            
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PAY_CATEGORY_DESC");
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            
            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PayCategory");

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" DEVICE_NO");
            strQry.AppendLine(",COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PayCategoryChosen");
          
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" D.COMPANY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",D.DEPARTMENT_NO");
            strQry.AppendLine(",D.DEPARTMENT_DESC");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEPARTMENT D");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON D.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND D.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND D.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" WHERE D.COMPANY_NO = " + parint64CompanyNo);

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" D.DEPARTMENT_DESC");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
           
            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Department");

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" DEVICE_NO");
            strQry.AppendLine(",COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEPARTMENT_NO");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "DepartmentChosen");
        
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",GROUP_NO");
            strQry.AppendLine(",GROUP_DESC");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.GROUPS ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Group");
            
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" DEVICE_NO");
            strQry.AppendLine(",COMPANY_NO");
            strQry.AppendLine(",GROUP_NO");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "GroupChosen");
            
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public void Update_Records(Int64 parint64CompanyNo, int parintDeviceNo, byte[] byteCompressedDataSet)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(byteCompressedDataSet);

            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEVICE_EMPLOYEE_LINK ");
            
            strQry.AppendLine(" WHERE DEVICE_NO = " + parintDeviceNo);
            strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            for (int intRowCount = 0; intRowCount < DataSet.Tables["EmployeeChosen"].Rows.Count; intRowCount++)
            {
                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.DEVICE_EMPLOYEE_LINK");
                strQry.AppendLine("(DEVICE_NO");
                strQry.AppendLine(",COMPANY_NO");
                strQry.AppendLine(",EMPLOYEE_NO");
                strQry.AppendLine(",PAY_CATEGORY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE)");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine("(" + parintDeviceNo);
                strQry.AppendLine("," + parint64CompanyNo);
                strQry.AppendLine("," + DataSet.Tables["EmployeeChosen"].Rows[intRowCount]["EMPLOYEE_NO"].ToString());
                strQry.AppendLine("," + DataSet.Tables["EmployeeChosen"].Rows[intRowCount]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["EmployeeChosen"].Rows[intRowCount]["PAY_CATEGORY_TYPE"].ToString()) + ")");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK ");
            
            strQry.AppendLine(" WHERE DEVICE_NO = " + parintDeviceNo);
            strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            for (int intRowCount = 0; intRowCount < DataSet.Tables["PayCategoryChosen"].Rows.Count; intRowCount++)
            {
                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK");
                strQry.AppendLine("(DEVICE_NO");
                strQry.AppendLine(",COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE)");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine("(" + parintDeviceNo);
                strQry.AppendLine("," + parint64CompanyNo);
                strQry.AppendLine("," + DataSet.Tables["PayCategoryChosen"].Rows[intRowCount]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["PayCategoryChosen"].Rows[intRowCount]["PAY_CATEGORY_TYPE"].ToString()) + ")");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            strQry.Clear();
            strQry.AppendLine(" DELETE DEL FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK_ACTIVE DEL ");

            strQry.AppendLine(" INNER JOIN ");
            
            strQry.AppendLine("(SELECT DISTINCT ");
            strQry.AppendLine(" DPCLA.DEVICE_NO");
            strQry.AppendLine(",DPCLA.COMPANY_NO");
            strQry.AppendLine(",DPCLA.PAY_CATEGORY_NO");
            strQry.AppendLine(",DPCLA.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK_ACTIVE DPCLA ");

            strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK DPCL");
            strQry.AppendLine(" ON DPCL.DEVICE_NO = DPCLA.DEVICE_NO");
            strQry.AppendLine(" AND DPCL.COMPANY_NO = DPCLA.COMPANY_NO");
            strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO = DPCLA.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND DPCL.PAY_CATEGORY_TYPE = DPCLA.PAY_CATEGORY_TYPE ");
         
            strQry.AppendLine(" WHERE DPCLA.DEVICE_NO = " + parintDeviceNo);
            strQry.AppendLine(" AND DPCLA.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND DPCL.PAY_CATEGORY_NO IS NULL) AS TEMP_TABLE");

            strQry.AppendLine(" ON DEL.DEVICE_NO = TEMP_TABLE.DEVICE_NO");
            strQry.AppendLine(" AND DEL.COMPANY_NO = TEMP_TABLE.COMPANY_NO");
            strQry.AppendLine(" AND DEL.PAY_CATEGORY_NO = TEMP_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND DEL.PAY_CATEGORY_TYPE = TEMP_TABLE.PAY_CATEGORY_TYPE ");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            
            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK ");
            strQry.AppendLine(" WHERE DEVICE_NO = " + parintDeviceNo);
            strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            for (int intRowCount = 0; intRowCount < DataSet.Tables["DepartmentChosen"].Rows.Count; intRowCount++)
            {
                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK");
                strQry.AppendLine("(DEVICE_NO");
                strQry.AppendLine(",COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",DEPARTMENT_NO)");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine("(" + parintDeviceNo);
                strQry.AppendLine("," + parint64CompanyNo);
                strQry.AppendLine("," + DataSet.Tables["DepartmentChosen"].Rows[intRowCount]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["DepartmentChosen"].Rows[intRowCount]["PAY_CATEGORY_TYPE"].ToString()));
                strQry.AppendLine("," + DataSet.Tables["DepartmentChosen"].Rows[intRowCount]["DEPARTMENT_NO"].ToString() + ")");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }
       
            strQry.Clear();
            strQry.AppendLine(" DELETE DEL FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK_ACTIVE DEL ");

            strQry.AppendLine(" INNER JOIN ");
            
            strQry.AppendLine("(SELECT DISTINCT ");
            strQry.AppendLine(" DDLA.DEVICE_NO");
            strQry.AppendLine(",DDLA.COMPANY_NO");
            strQry.AppendLine(",DDLA.PAY_CATEGORY_NO");
            strQry.AppendLine(",DDLA.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",DDLA.DEPARTMENT_NO ");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK_ACTIVE DDLA ");

            strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK DDL");
            strQry.AppendLine(" ON DDL.DEVICE_NO = DDLA.DEVICE_NO");
            strQry.AppendLine(" AND DDL.COMPANY_NO = DDLA.COMPANY_NO");
            strQry.AppendLine(" AND DDL.PAY_CATEGORY_NO = DDLA.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND DDL.PAY_CATEGORY_TYPE = DDLA.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND DDL.DEPARTMENT_NO = DDLA.DEPARTMENT_NO ");

            strQry.AppendLine(" WHERE DDLA.DEVICE_NO = " + parintDeviceNo);
            strQry.AppendLine(" AND DDLA.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND DDL.DEPARTMENT_NO IS NULL) AS TEMP_TABLE");

            strQry.AppendLine(" ON DEL.DEVICE_NO = TEMP_TABLE.DEVICE_NO");
            strQry.AppendLine(" AND DEL.COMPANY_NO = TEMP_TABLE.COMPANY_NO");
            strQry.AppendLine(" AND DEL.PAY_CATEGORY_NO = TEMP_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND DEL.PAY_CATEGORY_TYPE = TEMP_TABLE.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND DEL.DEPARTMENT_NO = TEMP_TABLE.DEPARTMENT_NO ");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
         
            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK ");
            strQry.AppendLine(" WHERE DEVICE_NO = " + parintDeviceNo);
            strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            for (int intRowCount = 0; intRowCount < DataSet.Tables["GroupChosen"].Rows.Count; intRowCount++)
            {
                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.DEVICE_GROUP_LINK");
                strQry.AppendLine("(DEVICE_NO");
                strQry.AppendLine(",COMPANY_NO");
                strQry.AppendLine(",GROUP_NO)");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine("(" + parintDeviceNo);
                strQry.AppendLine("," + parint64CompanyNo);
                strQry.AppendLine("," + DataSet.Tables["GroupChosen"].Rows[intRowCount]["GROUP_NO"].ToString() + ")");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }
        
            strQry.Clear();
            strQry.AppendLine(" DELETE DEL FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK_ACTIVE DEL ");

            strQry.AppendLine(" INNER JOIN ");
            
            strQry.AppendLine("(SELECT DISTINCT ");
            strQry.AppendLine(" DGLA.DEVICE_NO");
            strQry.AppendLine(",DGLA.COMPANY_NO");
            strQry.AppendLine(",DGLA.GROUP_NO");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK_ACTIVE DGLA ");

            strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_GROUP_LINK DGL");
            strQry.AppendLine(" ON DGLA.DEVICE_NO = DGL.DEVICE_NO");
            strQry.AppendLine(" AND DGLA.COMPANY_NO = DGL.COMPANY_NO");
            strQry.AppendLine(" AND DGLA.GROUP_NO = DGL.GROUP_NO");
       
            strQry.AppendLine(" WHERE DGLA.DEVICE_NO = " + parintDeviceNo);
            strQry.AppendLine(" AND DGLA.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND DGL.GROUP_NO IS NULL) AS TEMP_TABLE");

            strQry.AppendLine(" ON DEL.DEVICE_NO = TEMP_TABLE.DEVICE_NO");
            strQry.AppendLine(" AND DEL.COMPANY_NO = TEMP_TABLE.COMPANY_NO");
            strQry.AppendLine(" AND DEL.GROUP_NO = TEMP_TABLE.GROUP_NO");
            
            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
        }

        public void Delete_Records(Int64 parint64CompanyNo, int parintDeviceNo)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEVICE_EMPLOYEE_LINK ");
            strQry.AppendLine(" WHERE DEVICE_NO = " + parintDeviceNo);
            strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK ");
            strQry.AppendLine(" WHERE DEVICE_NO = " + parintDeviceNo);
            strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK_ACTIVE ");
            strQry.AppendLine(" WHERE DEVICE_NO = " + parintDeviceNo);
            strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK ");
            strQry.AppendLine(" WHERE DEVICE_NO = " + parintDeviceNo);
            strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK_ACTIVE ");
            strQry.AppendLine(" WHERE DEVICE_NO = " + parintDeviceNo);
            strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
     
            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK ");
            strQry.AppendLine(" WHERE DEVICE_NO = " + parintDeviceNo);
            strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK_ACTIVE ");
            strQry.AppendLine(" WHERE DEVICE_NO = " + parintDeviceNo);
            strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
        }
    }
}
