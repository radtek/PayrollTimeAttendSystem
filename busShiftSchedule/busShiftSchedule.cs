using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busShiftSchedule
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busShiftSchedule()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo, string parstrFromProgram)
        {
            DataSet DataSet = new DataSet();

            StringBuilder strQry = new StringBuilder();
         
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",PAY_CATEGORY_SHIFT_SCHEDULE_NO ");
            strQry.AppendLine(",FROM_DATETIME ");
            strQry.AppendLine(",TO_DATETIME ");
            strQry.AppendLine(",ISNULL(LOCKED_IND,0) AS LOCKED_IND");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_SHIFT_SCHEDULE");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            
            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE IN ('W','S')");
            }

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",FROM_DATETIME DESC");
            strQry.AppendLine(",TO_DATETIME DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "ShiftSchedule", parint64CompanyNo);
            
            DataSet.Tables.Add("PayrollType");
            DataTable PayrollTypeDataTable = new DataTable("PayrollType");
            DataSet.Tables["PayrollType"].Columns.Add("PAYROLL_TYPE_DESC", typeof(String));

            if (parstrFromProgram == "X")
            {
                DataRow drDataRow = DataSet.Tables["PayrollType"].NewRow();

                drDataRow["PAYROLL_TYPE_DESC"] = "Time Attendance";

                DataSet.Tables["PayrollType"].Rows.Add(drDataRow);
            }
            else
            {
                DataRow drDataRow = DataSet.Tables["PayrollType"].NewRow();

                drDataRow["PAYROLL_TYPE_DESC"] = "Wages";

                DataSet.Tables["PayrollType"].Rows.Add(drDataRow);
            
                drDataRow = DataSet.Tables["PayrollType"].NewRow();

                drDataRow["PAYROLL_TYPE_DESC"] = "Salaries";

                DataSet.Tables["PayrollType"].Rows.Add(drDataRow);
            }

            DataSet.AcceptChanges();

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public int Insert_New_Record(Int64 parint64CompanyNo, string parstrPayCategoryType, string strFromDate,string strToDate,bool blnLocked)
        {
            DataSet DataSet = new DataSet();

            int intPayCategoryShiftScheduleNo = 1;
            StringBuilder strQry = new StringBuilder();
        
            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_SHIFT_SCHEDULE");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",FROM_DATETIME");
            strQry.AppendLine(",TO_DATETIME");
            strQry.AppendLine(",LOCKED_IND)");

            strQry.AppendLine(" VALUES");
            strQry.AppendLine("(" + parint64CompanyNo);
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));

            strQry.AppendLine(",'" + strFromDate + "'");
            strQry.AppendLine(",'" + strToDate + "'");

             if (blnLocked == true)
            {
                strQry.AppendLine(",1)");
            }
            else
            {
                strQry.AppendLine(",0)");
            }

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            
            strQry.Clear();

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" MAX(PAY_CATEGORY_SHIFT_SCHEDULE_NO) AS MAX_PAY_CATEGORY_SHIFT_SCHEDULE_NO ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_SHIFT_SCHEDULE");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "ShiftSchedule", parint64CompanyNo);

            intPayCategoryShiftScheduleNo = Convert.ToInt32(DataSet.Tables["ShiftSchedule"].Rows[0]["MAX_PAY_CATEGORY_SHIFT_SCHEDULE_NO"]);

            DataSet.Dispose();
            
            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            return intPayCategoryShiftScheduleNo;
        }

        public void Update_Record(Int64 parint64CompanyNo, string parstrPayCategoryType, int parintPayCategoryShiftScheduleNo, string strFromDate,string strToDate,bool blnLocked)
        {
            StringBuilder strQry = new StringBuilder();
                        
            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_SHIFT_SCHEDULE");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" FROM_DATETIME = '" + strFromDate + "'");
            strQry.AppendLine(",TO_DATETIME = '" + strToDate + "'");

            if (blnLocked == true)
            {
                strQry.AppendLine(",LOCKED_IND = 1");
            }
            else
            {
                strQry.AppendLine(",LOCKED_IND = 0");
            }
          
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
            strQry.AppendLine(" AND PAY_CATEGORY_SHIFT_SCHEDULE_NO = " + parintPayCategoryShiftScheduleNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_SHIFT_SCHEDULE");
           
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
            strQry.AppendLine(" AND PAY_CATEGORY_SHIFT_SCHEDULE_NO = " + parintPayCategoryShiftScheduleNo);
            strQry.AppendLine(" AND SHIFT_SCHEDULE_DATETIME < '" + strFromDate + "'");
            
            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_SHIFT_SCHEDULE");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
            strQry.AppendLine(" AND PAY_CATEGORY_SHIFT_SCHEDULE_NO = " + parintPayCategoryShiftScheduleNo);
            strQry.AppendLine(" AND SHIFT_SCHEDULE_DATETIME > '" + strToDate + "'");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            
            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }
              
        public void Delete_Record(Int64 parint64CompanyNo, string parstrPayCategoryType, int parintPayCategoryShiftScheduleNo)
        {
            StringBuilder strQry = new StringBuilder();
          
            strQry.Clear();
          
            strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_SHIFT_SCHEDULE");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
            strQry.AppendLine(" AND PAY_CATEGORY_SHIFT_SCHEDULE_NO = " + parintPayCategoryShiftScheduleNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_SHIFT_SCHEDULE");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
            strQry.AppendLine(" AND PAY_CATEGORY_SHIFT_SCHEDULE_NO = " + parintPayCategoryShiftScheduleNo);
         
            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }
    }
}
