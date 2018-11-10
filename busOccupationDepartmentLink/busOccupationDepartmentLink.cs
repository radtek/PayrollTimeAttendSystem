using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busOccupationDepartmentLink
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busOccupationDepartmentLink()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }
      
        public byte[] Get_Form_Records(Int64 parint64CompanyNo, string parstrMenuId,string parstrTimeAttendInd)
        {
            DataSet DataSet = new DataSet();

            StringBuilder strQry = new StringBuilder();
            string strTableName = "";
            
            if (parstrMenuId == "3E")
            {
                strTableName = "DEPARTMENT";
            }
            else
            {
                strTableName = "OCCUPATION";
            }

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(strTableName + "_NO ");
            strQry.AppendLine("," + strTableName + "_DESC ");
            strQry.AppendLine(" FROM ");
            strQry.AppendLine("InteractPayroll_#CompanyNo#.dbo." + strTableName);
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND  DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(strTableName + "_DESC ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "OccupationDepartment", parint64CompanyNo);
            
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",EMPLOYEE_NO ");
            strQry.AppendLine(",EMPLOYEE_CODE ");
            strQry.AppendLine(",EMPLOYEE_NAME ");
            strQry.AppendLine(",EMPLOYEE_SURNAME ");
            strQry.AppendLine(",ISNULL(" + strTableName + "_NO,0) AS " + strTableName + "_NO");
            strQry.AppendLine(" FROM ");
            strQry.AppendLine("InteractPayroll_#CompanyNo#.dbo.Employee");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" AND EMPLOYEE_ENDDATE IS NULL ");

            if (parstrTimeAttendInd == "X")
            {
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T' ");

            }
            else
            {
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE IN ('W','S') ");
            }

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" EMPLOYEE_CODE ");
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parint64CompanyNo);
            
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public void Update_Employee_Link(Int64 parint64CompanyNo, Int64 parint64CurrentUserNo, string parstrMenuId, byte[] bytCompressedDataSet)
        {
             DataSet DataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(bytCompressedDataSet);
             StringBuilder strQry = new StringBuilder();
             string strTableName = "";

             if (parstrMenuId == "3E")
             {
                 strTableName = "DEPARTMENT";
             }
             else
             {
                 strTableName = "OCCUPATION";
             }

             for (int intRow = 0; intRow < DataSet.Tables["Employee"].Rows.Count; intRow++)
             {
                 strQry.Clear();
                 strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE");
                 strQry.AppendLine(" SET " + strTableName + "_NO = " + DataSet.Tables["Employee"].Rows[intRow][strTableName + "_NO"].ToString());
                 strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);
                 strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                 strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                 strQry.AppendLine(" AND  DATETIME_DELETE_RECORD IS NULL ");

                 clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
             }

             strQry.Clear();

             strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
             strQry.AppendLine(" SET BACKUP_DB_IND = 1");
             strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

             clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }
    }
}
