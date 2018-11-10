using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using InteractPayroll;

namespace InteractPayrollClient
{
    public class busEmployeeGroup
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busEmployeeGroup()
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

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" C.COMPANY_DESC");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Company");

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" G.COMPANY_NO");
            strQry.AppendLine(",G.GROUP_NO");
            strQry.AppendLine(",G.GROUP_DESC");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.GROUPS G");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS  UC ");
                strQry.AppendLine(" ON UC.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND UC.COMPANY_NO = G.COMPANY_NO ");
            }

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" G.GROUP_DESC");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Group");

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
            strQry.AppendLine(" ON EPC.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND ISNULL(E.NOT_ACTIVE_IND,'N') <> 'Y'");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");
            
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" GROUP_NO");
            strQry.AppendLine(",COMPANY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.GROUP_EMPLOYEE_LINK ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "EmployeeLink");
            
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public int Insert_Group(Int64 parint64CompanyNo, string parstrGroupDesc, byte[] byteCompressedDataSet)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(byteCompressedDataSet);

            int intGroupNo = 0;
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" MAX(GROUP_NO) AS MAX_NO");
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.GROUPS");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Temp");

            if (DataSet.Tables["Temp"].Rows[0].IsNull("MAX_NO") == true)
            {
                intGroupNo = 1;
            }
            else
            {
                intGroupNo = Convert.ToInt32(DataSet.Tables["Temp"].Rows[0]["MAX_NO"]) + 1;
            }

            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.GROUPS");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",GROUP_NO");
            strQry.AppendLine(",GROUP_DESC)");
            strQry.AppendLine(" VALUES");
            strQry.AppendLine("(" + parint64CompanyNo);
            strQry.AppendLine("," + intGroupNo);
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrGroupDesc) + ")");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            Insert_Group_Employee_Link(parint64CompanyNo, intGroupNo, DataSet);
                        
            return intGroupNo;
        }

        public void Update_Group(Int64 parint64CompanyNo, int parintGroupNo, string parstrGroupDesc, byte[] byteCompressedDataSet)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(byteCompressedDataSet);
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" UPDATE GROUPS");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" GROUP_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(parstrGroupDesc));
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND GROUP_NO = " + parintGroupNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            Insert_Group_Employee_Link(parint64CompanyNo, parintGroupNo, DataSet);
        }

        public void Delete_Group(Int64 parint64CompanyNo, int parintGroupNo)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            
            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.GROUPS");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND GROUP_NO = " + parintGroupNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.GROUP_EMPLOYEE_LINK");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND GROUP_NO = " + parintGroupNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND GROUP_NO = " + parintGroupNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
        }

        public void Insert_Group_Employee_Link(Int64 parint64CompanyNo, int parintGroupNo, DataSet parEmployeeLinkDataSet)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();

            //Delete Whole Group
            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.GROUP_EMPLOYEE_LINK ");
            strQry.AppendLine(" WHERE GROUP_NO = " + parintGroupNo);
            strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            for (int intRowCount = 0; intRowCount < parEmployeeLinkDataSet.Tables["EmployeeLink"].Rows.Count; intRowCount++)
            {
                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.GROUP_EMPLOYEE_LINK");
                strQry.AppendLine("(GROUP_NO");
                strQry.AppendLine(",COMPANY_NO");
                strQry.AppendLine(",EMPLOYEE_NO");
                strQry.AppendLine(",PAY_CATEGORY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE)");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine("(" + parintGroupNo);
                strQry.AppendLine("," + parint64CompanyNo);
                strQry.AppendLine("," + parEmployeeLinkDataSet.Tables["EmployeeLink"].Rows[intRowCount]["EMPLOYEE_NO"].ToString());
                strQry.AppendLine("," + parEmployeeLinkDataSet.Tables["EmployeeLink"].Rows[intRowCount]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parEmployeeLinkDataSet.Tables["EmployeeLink"].Rows[intRowCount]["PAY_CATEGORY_TYPE"].ToString()) + ")");
               
                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }
        }
    }
}
