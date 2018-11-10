using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using InteractPayroll;

namespace InteractPayrollClient
{
    public class busEmployeeOverride
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busEmployeeOverride()
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

            strQry.AppendLine(" ORDER BY C.COMPANY_DESC");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Company");

            if (DataSet.Tables["Company"].Rows.Count > 0)
            {
                byte[] bytTempCompress = Get_Company_Records(Convert.ToInt64(DataSet.Tables["Company"].Rows[0]["COMPANY_NO"]), parintCurrentUserNo, parstrAccessInd);

                DataSet TempDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(bytTempCompress);
                DataSet.Merge(TempDataSet);
            }

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
        
        public byte[] Get_Company_Records(Int64 parint64CompanyNo,int parintCurrentUserNo, string parstrAccessInd)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();
            
            //CostCentre
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PAY_CATEGORY_DESC");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PayCategory");
            
            //Department
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

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Department");

            //Employee
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",LINKED_IND = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN TEMP_TABLE.EMPLOYEE_NO IS NULL ");
            strQry.AppendLine(" THEN 'N' ");

            strQry.AppendLine(" ELSE 'Y' ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

            strQry.AppendLine(" LEFT JOIN ");

            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" EMPLOYEE_NO");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EMPLOYEE_NO");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EMPLOYEE_NO");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EMPLOYEE_NO");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_USER_LINK");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString() + ") AS TEMP_TABLE");

            strQry.AppendLine(" ON E.EMPLOYEE_NO = TEMP_TABLE.EMPLOYEE_NO");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND ISNULL(E.NOT_ACTIVE_IND,'N') <> 'Y'");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PeopleLink");
              
            //User
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" UCA2.COMPANY_NO");
            strQry.AppendLine(",UI.USER_NO");
            strQry.AppendLine(",UI.USER_ID");
            strQry.AppendLine(",UI.FIRSTNAME");
            strQry.AppendLine(",UI.SURNAME");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_ID UI");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA2");
            strQry.AppendLine(" ON UI.USER_NO = UCA2.USER_NO");
            
            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA");
                strQry.AppendLine(" ON UI.USER_NO = UCA.USER_NO");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA1");
                strQry.AppendLine(" ON UCA.COMPANY_NO = UCA1.COMPANY_NO");
                strQry.AppendLine(" AND UCA1.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND UCA1.COMPANY_ACCESS_IND = 'A'");
            }

            strQry.AppendLine(" WHERE UCA2.COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "User");
            
            //Employee TO Employee Link
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",EMPLOYEE_NO_LINK");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
          
            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "EmployeeLink");

            //Employee TO PayCategory Link
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
          
            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PayCategoryLink");

            //Employee TO Department Link
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",DEPARTMENT_NO");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK  ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            
            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "DepartmentLink");

            //Employee TO Employee Link
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",USER_NO");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_USER_LINK ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
           
            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "UserLink");
                        
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public void Update_Records(Int64 parint64CompanyNo, Int64 parint64EmployeeNo, byte[] byteCompressedDataSet)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(byteCompressedDataSet);

            StringBuilder strQry = new StringBuilder();

            //Employee
            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND EMPLOYEE_NO = " + parint64EmployeeNo.ToString());

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            for (int intRowCount = 0; intRowCount < DataSet.Tables["EmployeeLink"].Rows.Count; intRowCount++)
            {
                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_EMPLOYEE_LINK");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",EMPLOYEE_NO");
                strQry.AppendLine(",EMPLOYEE_NO_LINK)");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine("(" + parint64CompanyNo.ToString());
                strQry.AppendLine("," + parint64EmployeeNo.ToString());
                strQry.AppendLine("," + DataSet.Tables["EmployeeLink"].Rows[intRowCount]["EMPLOYEE_NO_LINK"].ToString() + ")");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            //Pay Category
            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND EMPLOYEE_NO = " + parint64EmployeeNo.ToString());

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            for (int intRowCount = 0; intRowCount < DataSet.Tables["PayCategoryLink"].Rows.Count; intRowCount++)
            {
                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY_LINK");
                strQry.AppendLine("(EMPLOYEE_NO");
                strQry.AppendLine(",COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE)");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine("(" + parint64EmployeeNo.ToString());
                strQry.AppendLine("," + parint64CompanyNo.ToString());
                strQry.AppendLine("," + DataSet.Tables["PayCategoryLink"].Rows[intRowCount]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["PayCategoryLink"].Rows[intRowCount]["PAY_CATEGORY_TYPE"].ToString()) + ")");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            //Department
            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND EMPLOYEE_NO = " + parint64EmployeeNo.ToString());

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            for (int intRowCount = 0; intRowCount < DataSet.Tables["DepartmentLink"].Rows.Count; intRowCount++)
            {
                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_DEPARTMENT_LINK");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",EMPLOYEE_NO");
                strQry.AppendLine(",PAY_CATEGORY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",DEPARTMENT_NO)");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine("(" + parint64CompanyNo.ToString());
                strQry.AppendLine("," + parint64EmployeeNo.ToString());
                strQry.AppendLine("," + DataSet.Tables["DepartmentLink"].Rows[intRowCount]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["DepartmentLink"].Rows[intRowCount]["PAY_CATEGORY_TYPE"].ToString()));
                strQry.AppendLine("," + DataSet.Tables["DepartmentLink"].Rows[intRowCount]["DEPARTMENT_NO"].ToString() + ")");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            //User
            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_USER_LINK");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND EMPLOYEE_NO = " + parint64EmployeeNo.ToString());

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            for (int intRowCount = 0; intRowCount < DataSet.Tables["UserLink"].Rows.Count; intRowCount++)
            {
                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_USER_LINK");
                strQry.AppendLine("(EMPLOYEE_NO");
                strQry.AppendLine(",COMPANY_NO");
                strQry.AppendLine(",USER_NO)");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine("(" + parint64EmployeeNo.ToString());
                strQry.AppendLine("," + parint64CompanyNo.ToString());
                strQry.AppendLine("," + DataSet.Tables["UserLink"].Rows[intRowCount]["USER_NO"].ToString() + ")");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }
        }
    }
}
