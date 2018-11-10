using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busOccupationDepartment
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busOccupationDepartment()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo, string parstrMenuId)
        {
            DataSet DataSet = new DataSet();

            StringBuilder strQry = new StringBuilder();
            string strTableName = "";

            if (parstrMenuId == "39")
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

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public int Insert_New_Record(Int64 parint64CompanyNo, Int64 parint64CurrentUserNo, string parstrMenuId, string parstrOccupationDepartmentDesc)
        {
            DataSet DataSet = new DataSet();

            int intOccupationDepartmentNo = 1;
            StringBuilder strQry = new StringBuilder();
            string strTableName = "";

            if (parstrMenuId == "39")
            {
                strTableName = "DEPARTMENT";
            }
            else
            {
                strTableName = "OCCUPATION";
            }

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" MAX(" + strTableName + "_NO) AS MAX_NO");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo." + strTableName);
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parint64CompanyNo);

            if (DataSet.Tables["Temp"].Rows[0].IsNull("MAX_NO") == true)
            {
                intOccupationDepartmentNo = 1;
            }
            else
            {
                intOccupationDepartmentNo = Convert.ToInt32(DataSet.Tables[0].Rows[0]["MAX_NO"]) + 1;
            }

            DataSet.Dispose();
            DataSet = null;

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo." + strTableName);
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine("," + strTableName + "_NO");
            strQry.AppendLine("," + strTableName + "_DESC");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD)");
            strQry.AppendLine(" VALUES");
            strQry.AppendLine("(" + parint64CompanyNo);
            strQry.AppendLine("," + intOccupationDepartmentNo);
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrOccupationDepartmentDesc));
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo + ")");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            return intOccupationDepartmentNo;
        }

        public void Update_Record(Int64 parint64CompanyNo, int parintOccupationDepartmentNo, Int64 parint64CurrentUserNo, string parstrMenuId, string parstrOccupationDepartmentDesc)
        {
            StringBuilder strQry = new StringBuilder();
            string strTableName = "";

            if (parstrMenuId == "39")
            {
                strTableName = "DEPARTMENT";
            }
            else
            {
                strTableName = "OCCUPATION";
            }

            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo." + strTableName);
            strQry.AppendLine(" SET " + strTableName + "_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(parstrOccupationDepartmentDesc));
            strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo.ToString());
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND " + strTableName + "_NO = " + parintOccupationDepartmentNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }
              
        public void Delete_Record(Int64 parint64CompanyNo, int parintOccupationDepartmentNo, Int64 parint64CurrentUserNo, string parstrMenuId)
        {
            StringBuilder strQry = new StringBuilder();
            string strTableName = "";

            if (parstrMenuId == "39")
            {
                strTableName = "DEPARTMENT";
            }
            else
            {
                strTableName = "OCCUPATION";
            }

            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo." + strTableName);
            strQry.AppendLine(" SET USER_NO_RECORD = " + parint64CurrentUserNo.ToString());
            strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE() ");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND " + strTableName + "_NO = " + parintOccupationDepartmentNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE");
            strQry.AppendLine(" SET " + strTableName + "_NO = 0");
            strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo.ToString());
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND " + strTableName + "_NO = " + parintOccupationDepartmentNo);
            strQry.AppendLine(" AND  DATETIME_DELETE_RECORD IS NULL ");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }
    }
}
