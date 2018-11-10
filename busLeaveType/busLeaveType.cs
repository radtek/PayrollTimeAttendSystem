using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busLeaveType
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busLeaveType()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
           
            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EARNING_NO");
            strQry.AppendLine(",E.TIE_BREAKER");
            strQry.AppendLine(",E.EARNING_DESC");
            strQry.AppendLine(",E.LEAVE_PERCENTAGE");
            strQry.AppendLine(",E.EARNING_REPORT_HEADER1");
            strQry.AppendLine(",E.EARNING_REPORT_HEADER2");
            strQry.AppendLine(",E.EARNING_DEL_IND");
            strQry.AppendLine(",EEC.EARNING_NO AS PAYROLL_LINK");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING E");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");
            strQry.AppendLine(" ON E.COMPANY_NO = EEC.COMPANY_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EEC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND E.EARNING_NO = EEC.EARNING_NO");
            strQry.AppendLine(" AND EEC.RUN_TYPE = 'P'");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" AND E.EARNING_NO > 199");
            //Record has Not been Deleted
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" E.EARNING_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "LeaveType", parint64CompanyNo);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public int Insert_New_Record(Int64 parInt64CompanyNo, Int64 parint64CurrentUserNo, byte[] parbyteDataSet)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            int intLeaveTypeNo;
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" MAX(EARNING_NO) AS MAX_LEAVE_TYPE_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["LeaveType"].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine(" AND EARNING_NO > 199");

            DataSet DataSet = new DataSet();

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

            if (DataSet.Tables[0].Rows[0].IsNull("MAX_LEAVE_TYPE_NO") == true)
            {
                //NB 200 = Normal LeaveType 201 = Sick LeaveType
                intLeaveTypeNo = 202;
            }
            else
            {
                intLeaveTypeNo = Convert.ToInt32(DataSet.Tables[0].Rows[0]["MAX_LEAVE_TYPE_NO"]) + 1;
            }

            DataSet.Dispose();
            DataSet = null;

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EARNING");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(",TIE_BREAKER");
            strQry.AppendLine(",EARNING_DESC");
            strQry.AppendLine(",LEAVE_PERCENTAGE");
            strQry.AppendLine(",EARNING_REPORT_HEADER1");
            strQry.AppendLine(",EARNING_REPORT_HEADER2");
            strQry.AppendLine(",IRP5_CODE");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD");
            strQry.AppendLine(",EARNING_DEL_IND)");
            strQry.AppendLine(" VALUES ");
            strQry.AppendLine("(" + parDataSet.Tables["LeaveType"].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["LeaveType"].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
            strQry.AppendLine("," + intLeaveTypeNo);
            strQry.AppendLine("," + parDataSet.Tables["LeaveType"].Rows[0]["TIE_BREAKER"].ToString());
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["LeaveType"].Rows[0]["EARNING_DESC"].ToString()));
            strQry.AppendLine("," + parDataSet.Tables["LeaveType"].Rows[0]["LEAVE_PERCENTAGE"].ToString());
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["LeaveType"].Rows[0]["EARNING_REPORT_HEADER1"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["LeaveType"].Rows[0]["EARNING_REPORT_HEADER2"].ToString()));
            //20170418 - Fix IRP5Code
            strQry.AppendLine(",3601");
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine(",'Y')");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            parDataSet.Dispose();
            parDataSet = null;

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            return intLeaveTypeNo;
        }

        public int Update_Record(Int64 parInt64CompanyNo, Int64 parint64CurrentUserNo, byte[] parbyteDataSet)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
            int intReturnCode = 0;

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" EARNING_NO ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables[0].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
            strQry.AppendLine(" AND EARNING_NO = " + parDataSet.Tables[0].Rows[0]["EARNING_NO"].ToString());

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

            if (DataSet.Tables["Temp"].Rows.Count > 0)
            {
                intReturnCode = 9999;
                goto Update_Record_Continue;
            }


            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EARNING");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" EARNING_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["EARNING_DESC"].ToString()));
            strQry.AppendLine(",LEAVE_PERCENTAGE = " + Convert.ToInt32(parDataSet.Tables[0].Rows[0]["LEAVE_PERCENTAGE"]));
            strQry.AppendLine(",EARNING_REPORT_HEADER1 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["EARNING_REPORT_HEADER1"].ToString()));
            strQry.AppendLine(",EARNING_REPORT_HEADER2 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["EARNING_REPORT_HEADER2"].ToString()));
            strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);
            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables[0].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
            strQry.AppendLine(" AND EARNING_NO = " + parDataSet.Tables[0].Rows[0]["EARNING_NO"].ToString());
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

        Update_Record_Continue:

            DataSet.Dispose();
            DataSet = null;
            parDataSet.Dispose();
            parDataSet = null;

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            return intReturnCode;
        }

        public void Delete_Record(Int64 parint64CurrentUserNo, Int64 parint64CompanyNo, string parstrPayrollType, int parintLeaveTypeNo)
        {
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EARNING");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
            strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(" AND EARNING_NO = " + parintLeaveTypeNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }
    }
}
