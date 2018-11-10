using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

namespace InteractPayroll
{
    public class busEmployeeLeaveTakeOnActivate
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busEmployeeLeaveTakeOnActivate()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
 
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_TAKE_ON LT ");
            strQry.AppendLine(" ON E.COMPANY_NO = LT.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = LT.EMPLOYEE_NO ");
            strQry.AppendLine(" AND (LT.PREV_NORMAL_LEAVE_DAYS <> 0 ");
            strQry.AppendLine(" OR LT.NORMAL_LEAVE_DAYS <> 0 ");
            strQry.AppendLine(" OR LT.SICK_LEAVE_DAYS <> 0) ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);

            //2013-06-21 Exclude T=Time Attendance
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE IN ('W','S')");

            strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y' ");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parint64CompanyNo);

            DataSet.Tables.Add("PayrollType");
            DataTable PayrollTypeDataTable = new DataTable("PayrollType");
            DataSet.Tables["PayrollType"].Columns.Add("PAYROLL_TYPE_DESC", typeof(String));

            if (DataSet.Tables["Employee"].Rows.Count > 0)
            {
                DataView PayrollTypeDataView = new DataView(DataSet.Tables["Employee"],
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
                PayrollTypeDataView = new DataView(DataSet.Tables["Employee"],
                    "PAY_CATEGORY_TYPE = 'S'",
                    "",
                    DataViewRowState.CurrentRows);

                if (PayrollTypeDataView.Count > 0)
                {
                    DataRow drDataRow = DataSet.Tables["PayrollType"].NewRow();

                    drDataRow["PAYROLL_TYPE_DESC"] = "Salaries";

                    DataSet.Tables["PayrollType"].Rows.Add(drDataRow);
                }
            }

            DataSet.AcceptChanges();
           
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public int Backup_DataBase(Int64 parInt64CompanyNo,string parstrPayrollType, string parstrOption)
        {
            int intReturnCode = 9;

            try
            {
                DataSet pvtDataSet = new DataSet();
                StringBuilder strQry = new StringBuilder();

                strQry.Clear();
                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" BACKUP_DATABASE_PATH");
                strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_PATH");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), pvtDataSet, "Directory", -1);

                string strFileDirectory = pvtDataSet.Tables["Directory"].Rows[0]["BACKUP_DATABASE_PATH"].ToString();

                string strDataBaseName = "InteractPayroll_" + parInt64CompanyNo.ToString("00000");

                string strBackupFileName = strDataBaseName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_BeforeEmployeeLeaveActivation.bak";

                if (parstrOption == "A")
                {
                    strBackupFileName = strDataBaseName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_AfterEmployeeLeaveActivation.bak";
                }

                strQry.Clear();
                strQry.AppendLine("BACKUP DATABASE " + strDataBaseName + " TO DISK = '" + strFileDirectory + "\\" + strBackupFileName + "' WITH CHECKSUM");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                strQry.Clear();
                strQry.AppendLine("INSERT INTO InteractPayroll.dbo.BACKUP_DATABASE_DATETIME");
                strQry.AppendLine("(BACKUP_DATABASE_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",BACKUP_DATABASE_NAME");
                strQry.AppendLine(",PAYROLL_RUN_DATETIME");
                strQry.AppendLine(",BACKUP_DATETIME");
                strQry.AppendLine(",BACKUP_FILE_NAME)");
                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" ISNULL(MAX(BACKUP_DATABASE_NO),0) + 1");
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strDataBaseName));
                strQry.AppendLine(",GETDATE()");
                strQry.AppendLine(",GETDATE()");
                strQry.AppendLine(",'" + strFileDirectory + "\\" + strBackupFileName + "'");
                strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_DATETIME");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                intReturnCode = 0;
            }
            catch (Exception ex)
            {
                string strStop = "";
            }

            return intReturnCode;
        }

        public byte[] Update_Record(Int64 parint64CompanyNo, Int64 parint64CurrentUserNo, string parstrPayrollType, string parstrEmployeeNoin)
        {
            string[] parstrEmployeeNo = parstrEmployeeNoin.Split(',');
            StringBuilder strQry = new StringBuilder();

            for (int intRow = 0; intRow < parstrEmployeeNo.Length; intRow++)
            {
                //Normal Leave - Previous Year
                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_PERIOD_DATE");

                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EMPLOYEE_NO");
                strQry.AppendLine(",EARNING_NO");
              
                strQry.AppendLine(",LEAVE_DESC");
                strQry.AppendLine(",LEAVE_ACCUM_DAYS");
                strQry.AppendLine(",LEAVE_FROM_DATE");
                strQry.AppendLine(",LEAVE_TO_DATE");
                strQry.AppendLine(",LEAVE_PAID_DAYS");
            
                //Errol Added 2012-01-10
                strQry.AppendLine(",LEAVE_OPTION");

                strQry.AppendLine(",LEAVE_DAYS_DECIMAL");
                strQry.AppendLine(",LEAVE_HOURS_DECIMAL");
                
                strQry.AppendLine(",PROCESS_NO)");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" L.COMPANY_NO");
                strQry.AppendLine(",L.PAY_PERIOD_DATE");
                strQry.AppendLine(",L.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",L.EMPLOYEE_NO");
                strQry.AppendLine(",L.EARNING_NO");
               
                strQry.AppendLine(",'Take-On Balance (Previous Year)'");
                strQry.AppendLine(",LT.PREV_NORMAL_LEAVE_DAYS");
                strQry.AppendLine(",L.LEAVE_FROM_DATE");
                strQry.AppendLine(",L.LEAVE_TO_DATE");
                strQry.AppendLine(",L.LEAVE_PAID_DAYS");
               
                //Errol Added 2012-01-10
                strQry.AppendLine(",L.LEAVE_OPTION");
                strQry.AppendLine(",L.LEAVE_DAYS_DECIMAL");
                strQry.AppendLine(",L.LEAVE_HOURS_DECIMAL");

                strQry.AppendLine(",100");
                
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY L");
                
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_TAKE_ON LT ");
                strQry.AppendLine(" ON L.COMPANY_NO = LT.COMPANY_NO ");
                strQry.AppendLine(" AND L.EMPLOYEE_NO = LT.EMPLOYEE_NO ");
                strQry.AppendLine(" AND LT.PREV_NORMAL_LEAVE_DAYS > 0 ");
                
                strQry.AppendLine(" WHERE L.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND L.EMPLOYEE_NO = " + parstrEmployeeNo[intRow]);
                strQry.AppendLine(" AND L.EARNING_NO = 200 ");
                strQry.AppendLine(" AND L.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND L.PROCESS_NO = 99");
               
                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                //Normal Leave
                strQry.Clear();
                strQry.AppendLine(" UPDATE L");
                strQry.AppendLine(" SET L.LEAVE_ACCUM_DAYS = LT.NORMAL_LEAVE_DAYS");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY L");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_TAKE_ON LT ");
                strQry.AppendLine(" ON L.COMPANY_NO = LT.COMPANY_NO ");
                strQry.AppendLine(" AND L.EMPLOYEE_NO = LT.EMPLOYEE_NO ");
                strQry.AppendLine(" AND LT.NORMAL_LEAVE_DAYS > 0 ");

                strQry.AppendLine(" WHERE L.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND L.EMPLOYEE_NO = " + parstrEmployeeNo[intRow]);
                strQry.AppendLine(" AND L.EARNING_NO = 200 ");
                strQry.AppendLine(" AND L.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND L.LEAVE_DESC = 'Take-On Balance'");

                strQry.AppendLine(" AND L.PROCESS_NO = 99");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                //Sick Leave
                strQry.Clear();
                strQry.AppendLine(" UPDATE L");
                strQry.AppendLine(" SET L.LEAVE_ACCUM_DAYS = LT.SICK_LEAVE_DAYS");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY L");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_TAKE_ON LT ");
                strQry.AppendLine(" ON L.COMPANY_NO = LT.COMPANY_NO ");
                strQry.AppendLine(" AND L.EMPLOYEE_NO = LT.EMPLOYEE_NO ");
                strQry.AppendLine(" AND LT.SICK_LEAVE_DAYS > 0 ");

                strQry.AppendLine(" WHERE L.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND L.EMPLOYEE_NO = " + parstrEmployeeNo[intRow]);
                strQry.AppendLine(" AND L.EARNING_NO = 201 ");
                strQry.AppendLine(" AND L.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND L.LEAVE_DESC = 'Take-On Balance'");

                strQry.AppendLine(" AND L.PROCESS_NO = 99");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_TAKE_ON");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND EMPLOYEE_NO = " + parstrEmployeeNo[intRow]);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            }

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            byte[] bytCompress = Get_Form_Records(parint64CompanyNo);

            return bytCompress;
        }
    }
}
