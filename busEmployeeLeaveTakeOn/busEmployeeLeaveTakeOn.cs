using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busEmployeeLeaveTakeOn
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busEmployeeLeaveTakeOn()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parInt64CompanyNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            //So That Delete Will Happen
            string strEmployeeNoIn = "-1";

            //Insert Records 
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" L.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",L.EMPLOYEE_NO");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY L ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");
            strQry.AppendLine(" ON L.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND L.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND L.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL ");
     
            //Add 180 Days (Withinn 6 Months)
            strQry.AppendLine(" AND DATEADD(DD,180,L.PAY_PERIOD_DATE) >= GETDATE() ");

            strQry.AppendLine(" LEFT JOIN ");
            
            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EMPLOYEE_NO");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            //2013-06-21 Exclude T=Time Attendance
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE IN ('W','S')");

            //Take-On Balance / Take-On Balance (Previous Year)

            strQry.AppendLine(" AND PROCESS_NO IN (99,100)");
            
            //Normal Leave / Sick Leave 
            strQry.AppendLine(" AND EARNING_NO IN (200,201)");
            strQry.AppendLine(" AND LEAVE_ACCUM_DAYS > 0) AS TEMP_LEAVE");

            strQry.AppendLine(" ON L.PAY_CATEGORY_TYPE = TEMP_LEAVE.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND L.EMPLOYEE_NO = TEMP_LEAVE.EMPLOYEE_NO ");
          
            strQry.AppendLine(" WHERE L.COMPANY_NO = " + parInt64CompanyNo);

            //2013-06-21 Exclude T=Time Attendance
            strQry.AppendLine(" AND L.PAY_CATEGORY_TYPE IN ('W','S')");

            //Take-On Balance
            strQry.AppendLine(" AND L.PROCESS_NO = 99");
            
            //Normal Leave / Sick Leave 
            strQry.AppendLine(" AND L.EARNING_NO IN (200,201)");
            strQry.AppendLine(" AND L.LEAVE_ACCUM_DAYS = 0");
            strQry.AppendLine(" AND L.LEAVE_PAID_DAYS = 0");

            //No Take-On Leave Balances
            strQry.AppendLine(" AND TEMP_LEAVE.PAY_CATEGORY_TYPE IS NULL");
           
            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" L.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",L.EMPLOYEE_NO ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeTemp", parInt64CompanyNo);

            for (int intRow = 0; intRow < DataSet.Tables["EmployeeTemp"].Rows.Count; intRow++)
            {
                if (intRow == 0)
                {
                    strEmployeeNoIn = DataSet.Tables["EmployeeTemp"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                }
                else
                {
                    strEmployeeNoIn += "," + DataSet.Tables["EmployeeTemp"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                }

                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.LEAVE_TAKE_ON ");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",EMPLOYEE_NO");
                strQry.AppendLine(",PREV_NORMAL_LEAVE_DAYS");
                strQry.AppendLine(",NORMAL_LEAVE_DAYS");
                strQry.AppendLine(",SICK_LEAVE_DAYS");
                strQry.AppendLine(",LEAVE_EFFECTIVE_DATE)");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EIH.COMPANY_NO");
                strQry.AppendLine(",EIH.EMPLOYEE_NO");
                strQry.AppendLine(",0");
                strQry.AppendLine(",0");
                strQry.AppendLine(",0");
                strQry.AppendLine(",EIH.PAY_PERIOD_DATE");
                
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY EIH");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_TAKE_ON LTO ");
                strQry.AppendLine(" ON EIH.COMPANY_NO = LTO.COMPANY_NO ");
                strQry.AppendLine(" AND EIH.EMPLOYEE_NO = LTO.EMPLOYEE_NO ");

                strQry.AppendLine(" WHERE EIH.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EIH.RUN_TYPE = 'T' ");
                strQry.AppendLine(" AND EIH.EMPLOYEE_NO = " + DataSet.Tables["EmployeeTemp"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                strQry.AppendLine(" AND EIH.PAY_CATEGORY_TYPE  = '" + DataSet.Tables["EmployeeTemp"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'");

                strQry.AppendLine(" AND LTO.COMPANY_NO IS NULL ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            }

            //Delete any Employee That Have been on System for longer Than 180 Days
            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_TAKE_ON ");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND NOT EMPLOYEE_NO IN (" + strEmployeeNoIn + ")");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",LT.PREV_NORMAL_LEAVE_DAYS");
            strQry.AppendLine(",LT.NORMAL_LEAVE_DAYS");
            strQry.AppendLine(",LT.SICK_LEAVE_DAYS");
            strQry.AppendLine(",LT.LEAVE_EFFECTIVE_DATE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_TAKE_ON LT ");
            strQry.AppendLine(" ON E.COMPANY_NO = LT.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = LT.EMPLOYEE_NO ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);

            //2013-06-21 Exclude T=Time Attendance
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE IN ('W','S')");

            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
           
            strQry.AppendLine(" ORDER BY E.EMPLOYEE_CODE");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parInt64CompanyNo);

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

        public void Update_Record(Int64 parint64CompanyNo, byte[] parbyteDataSet)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            StringBuilder strQry = new StringBuilder();

            for (int intRow = 0; intRow < parDataSet.Tables["Employee"].Rows.Count; intRow++)
            {
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.LEAVE_TAKE_ON");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" PREV_NORMAL_LEAVE_DAYS = " + Convert.ToDouble(parDataSet.Tables["Employee"].Rows[intRow]["PREV_NORMAL_LEAVE_DAYS"]));
                strQry.AppendLine(",NORMAL_LEAVE_DAYS = " + Convert.ToDouble(parDataSet.Tables["Employee"].Rows[intRow]["NORMAL_LEAVE_DAYS"]));
                strQry.AppendLine(",SICK_LEAVE_DAYS = " + Convert.ToDouble(parDataSet.Tables["Employee"].Rows[intRow]["SICK_LEAVE_DAYS"]));
                strQry.AppendLine(",LEAVE_EFFECTIVE_DATE = '" + Convert.ToDateTime(parDataSet.Tables["Employee"].Rows[intRow]["LEAVE_EFFECTIVE_DATE"]).ToString("yyyy-MM-dd") + "'");

                strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(parDataSet.Tables["Employee"].Rows[intRow]["COMPANY_NO"]));
                strQry.AppendLine(" AND EMPLOYEE_NO = " + Convert.ToInt32(parDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"]));

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
