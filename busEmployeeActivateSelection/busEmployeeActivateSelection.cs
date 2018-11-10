using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

namespace InteractPayroll
{
    public class busEmployeeActivateSelection
    {
        clsDBConnectionObjects clsDBConnectionObjects;
      
        public busEmployeeActivateSelection()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo, string parstrTimeAttendInd)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            //Used For Salary to Load Combo Box (Only 1 Salary Pay Category per Company)
            strQry.Clear();
            strQry.AppendLine(" SELECT  ");
            strQry.AppendLine(" COMPANY_NO  ");
            strQry.AppendLine(",MAX(PAY_PERIOD_DATE) AS PREV_PAY_PERIOD_DATE ");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
            strQry.AppendLine(" AND RUN_TYPE = 'P'");
            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" COMPANY_NO ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "SalaryPrevDate", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EMPLOYEE_CODE");
            strQry.AppendLine(",EMPLOYEE_NAME");
            strQry.AppendLine(",EMPLOYEE_SURNAME");
            strQry.AppendLine(",EMPLOYEE_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EMPLOYEE_TAKEON_IND IS NULL ");
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            if (parstrTimeAttendInd == "X")
            {
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE IN ('S','W')");
            }
          
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parint64CompanyNo);
            
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            
            strQry.AppendLine(" E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",MAX(PCPH.PAY_PERIOD_DATE) AS PREV_PAY_PERIOD_DATE ");
           
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");
            strQry.AppendLine(" ON E.COMPANY_NO = PCPH.COMPANY_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = PCPH.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND PCPH.RUN_TYPE = 'T' ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND (E.EMPLOYEE_TAKEON_IND IS NULL ");
            strQry.AppendLine(" OR E.EMPLOYEE_TAKEON_IND = 'C')");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

            if (parstrTimeAttendInd == "X")
            {
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE IN ('S','W')");
            }

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" E.PAY_CATEGORY_TYPE");
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", parint64CompanyNo);

            DataSet.Tables.Add("PayrollType");
            DataTable PayrollTypeDataTable = new DataTable("PayrollType");
            DataSet.Tables["PayrollType"].Columns.Add("PAYROLL_TYPE_DESC", typeof(String));
            DataSet.Tables["PayrollType"].Columns.Add("PREV_PAY_PERIOD_DATE", typeof(DateTime));

            if (DataSet.Tables["Company"].Rows.Count > 0)
            {
                DataView PayrollTypeDataView = new DataView(DataSet.Tables["Company"],
                 "PAY_CATEGORY_TYPE = 'W'",
                 "",
                 DataViewRowState.CurrentRows);

                if (PayrollTypeDataView.Count > 0)
                {
                    DataRow drDataRow = DataSet.Tables["PayrollType"].NewRow();

                    drDataRow["PAYROLL_TYPE_DESC"] = "Wages";
                    drDataRow["PREV_PAY_PERIOD_DATE"] = PayrollTypeDataView[0]["PREV_PAY_PERIOD_DATE"];

                    DataSet.Tables["PayrollType"].Rows.Add(drDataRow);
                }

                PayrollTypeDataView = null;
                PayrollTypeDataView = new DataView(DataSet.Tables["Company"],
                    "PAY_CATEGORY_TYPE = 'S'",
                    "",
                    DataViewRowState.CurrentRows);

                if (PayrollTypeDataView.Count > 0)
                {
                    DataRow drDataRow = DataSet.Tables["PayrollType"].NewRow();

                    drDataRow["PAYROLL_TYPE_DESC"] = "Salaries";
                    drDataRow["PREV_PAY_PERIOD_DATE"] = PayrollTypeDataView[0]["PREV_PAY_PERIOD_DATE"];

                    DataSet.Tables["PayrollType"].Rows.Add(drDataRow);
                }

                PayrollTypeDataView = null;
                PayrollTypeDataView = new DataView(DataSet.Tables["Company"],
                    "PAY_CATEGORY_TYPE = 'T'",
                    "",
                    DataViewRowState.CurrentRows);

                if (PayrollTypeDataView.Count > 0)
                {
                    DataRow drDataRow = DataSet.Tables["PayrollType"].NewRow();

                    drDataRow["PAYROLL_TYPE_DESC"] = "Time Attendance";
                    drDataRow["PREV_PAY_PERIOD_DATE"] = PayrollTypeDataView[0]["PREV_PAY_PERIOD_DATE"];

                    DataSet.Tables["PayrollType"].Rows.Add(drDataRow);
                }
            }

            DataSet.AcceptChanges();

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Insert_TakeOn_Records(Int64 parint64CompanyNo, string parstrPayrollType, string parstrTimeAttendInd, DateTime parDateTime, string parstrEmployeeNoIn,Int64 parint64CurrentUserNo)
        {
            StringBuilder strQry = new StringBuilder();
            StringBuilder strFieldNamesInitialised = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_PERIOD_DATE");
            strQry.AppendLine(",RUN_TYPE");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",RUN_NO");
         
            strQry.AppendLine(",PAY_PERIOD_DATE_FROM");

            //Return strings for field that need to be Initialised to Zero
            strFieldNamesInitialised.Clear();
            clsDBConnectionObjects.Initialise_DataSet_Numeric_Fields("PAY_CATEGORY_PERIOD_HISTORY", ref strQry, ref strFieldNamesInitialised, parint64CompanyNo);

            strQry.AppendLine(")");
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" EPC.COMPANY_NO");
            strQry.AppendLine(",'" + parDateTime.ToString("yyyy-MM-dd") + "'");
            strQry.AppendLine(",'T'");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",1");

            strQry.AppendLine(",'" + parDateTime.ToString("yyyy-MM-dd") + "'");

            //Append Initialised Numeric Fields
            strQry.Append(strFieldNamesInitialised);

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            
            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");
            strQry.AppendLine(" ON EPC.COMPANY_NO = PCPH.COMPANY_NO ");
            strQry.AppendLine(" AND PCPH.PAY_PERIOD_DATE = '" + parDateTime.ToString("yyyy-MM-dd") + "'");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PCPH.RUN_TYPE = 'T'");
            strQry.AppendLine(" AND PCPH.RUN_NO = 1");
          
            strQry.AppendLine(" WHERE EPC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EPC.EMPLOYEE_NO IN (" + parstrEmployeeNoIn + ")");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y' ");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            //PAY_CATEGORY_PERIOD_HISTORY Record does Not Exist
            strQry.AppendLine(" AND PCPH.COMPANY_NO IS NULL ");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            //Initialise Tax StartDate
            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = '" + parDateTime.ToString("yyyy-MM-dd") + "'");
            strQry.AppendLine(",EMPLOYEE_TAX_STARTDATE = '" + parDateTime.ToString("yyyy-MM-dd") + "'");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(" AND EMPLOYEE_NO IN (" + parstrEmployeeNoIn + ")");
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            
            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_PERIOD_DATE");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",RUN_TYPE");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",RUN_NO");
            strQry.AppendLine(",HOURLY_RATE");
            strQry.AppendLine(",OVERTIME_VALUE_BF");
            strQry.AppendLine(",OVERTIME_VALUE_CF");
            strQry.AppendLine(",DEFAULT_IND)");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",'" + parDateTime.ToString("yyyy-MM-dd") + "'");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",'T'");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",1");

            strQry.AppendLine(",HOURLY_RATE");
            strQry.AppendLine(",0");
            strQry.AppendLine(",0");
            strQry.AppendLine(",DEFAULT_IND");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EMPLOYEE_NO IN (" + parstrEmployeeNoIn + ")");
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(" AND DEFAULT_IND = 'Y' ");
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_PERIOD_DATE");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",RUN_TYPE");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",RUN_NO");

            strQry.AppendLine(",CLOSE_IND");
            strQry.AppendLine(",PAYSLIP_IND");

            //Return strings for field that need to be Initialised to Zero
            strFieldNamesInitialised.Clear();
            clsDBConnectionObjects.Initialise_DataSet_Numeric_Fields("EMPLOYEE_INFO_HISTORY", ref strQry, ref strFieldNamesInitialised, parint64CompanyNo);

            strQry.AppendLine(")");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",'" + parDateTime.ToString("yyyy-MM-dd") + "'");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",'T'");
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(",1");

            strQry.AppendLine(",'N'");
            strQry.AppendLine(",'Y'");

            //Append Initialised Numeric Fields
            strQry.Append(strFieldNamesInitialised);

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND EMPLOYEE_NO IN (");

            strQry.AppendLine(parstrEmployeeNoIn);

            strQry.AppendLine(")");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_PERIOD_DATE");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",RUN_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",RUN_NO");
          
            //Return strings for field that need to be Initialised to Zero
            strFieldNamesInitialised.Clear();
            clsDBConnectionObjects.Initialise_DataSet_Numeric_Fields("EMPLOYEE_DEDUCTION_HISTORY", ref strQry, ref strFieldNamesInitialised, parint64CompanyNo);

            strQry.AppendLine(")");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ED.COMPANY_NO");
            strQry.AppendLine(",'" + parDateTime.ToString("yyyy-MM-dd") + "'");
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(",ED.EMPLOYEE_NO");
            strQry.AppendLine(",'T'");
            strQry.AppendLine(",ED.DEDUCTION_NO");
            strQry.AppendLine(",ED.DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",1");

            //Append Initialised Numeric Fields
            strQry.Append(strFieldNamesInitialised);

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ED");
            strQry.AppendLine(",InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            
            strQry.AppendLine(" WHERE ED.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND ED.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND ED.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND ED.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(" AND ED.EMPLOYEE_NO = E.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND ED.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" AND ED.EMPLOYEE_NO IN (");

            strQry.AppendLine(parstrEmployeeNoIn);

            strQry.AppendLine(")");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_PERIOD_DATE");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",RUN_TYPE");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",RUN_NO");

            //Return strings for field that need to be Initialised to Zero
            strFieldNamesInitialised.Clear();
            clsDBConnectionObjects.Initialise_DataSet_Numeric_Fields("EMPLOYEE_EARNING_HISTORY", ref strQry, ref strFieldNamesInitialised, parint64CompanyNo);

            strQry.AppendLine(")");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",'" + parDateTime.ToString("yyyy-MM-dd") + "'");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",'T'");
            strQry.AppendLine(",EN.EARNING_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",1");

            //Append Initialised Numeric Fields
            strQry.Append(strFieldNamesInitialised);

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING EN");
            
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON EN.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");
            
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON EN.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.EMPLOYEE_NO = E.EMPLOYEE_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_NO IN (");
            strQry.AppendLine(parstrEmployeeNoIn);
            strQry.AppendLine(")");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C");
            strQry.AppendLine(" ON EN.COMPANY_NO = C.COMPANY_NO");
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" LEFT JOIN ");
            
            strQry.AppendLine("(SELECT DISTINCT ");

            strQry.AppendLine(" PAY_CATEGORY_NO ");
            strQry.AppendLine(",PAY_PUBLIC_HOLIDAY_IND ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL) AS PUBLIC_HOLIDAY_TABLE ");

            strQry.AppendLine(" ON EPC.PAY_CATEGORY_NO = PUBLIC_HOLIDAY_TABLE.PAY_CATEGORY_NO");
                       
            strQry.AppendLine(" WHERE EN.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL ");
            
            strQry.AppendLine(" AND ((EN.EARNING_NO IN (1,2,3,4,5,6,7,9))");

            //Public Holiday - Company Paid 
            strQry.AppendLine(" OR (EN.EARNING_NO = 8");
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" AND PUBLIC_HOLIDAY_TABLE.PAY_PUBLIC_HOLIDAY_IND = 'Y')");
            
            //LEAVE_PAID_IND = 'Y' for all Earning Records Except Leave Marked as UnPaid
            strQry.AppendLine(" OR (EN.EARNING_NO >= 200 AND EN.LEAVE_PERCENTAGE > 0))");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            //All Linked Records that are NOT Default Records
            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_PERIOD_DATE");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",RUN_TYPE");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",RUN_NO");

            //Return strings for field that need to be Initialised to Zero
            strFieldNamesInitialised.Clear();
            clsDBConnectionObjects.Initialise_DataSet_Numeric_Fields("EMPLOYEE_EARNING_HISTORY", ref strQry, ref strFieldNamesInitialised, parint64CompanyNo);

            strQry.AppendLine(")");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",'" + parDateTime.ToString("yyyy-MM-dd") + "'");
            strQry.AppendLine(",EE.EMPLOYEE_NO");
            strQry.AppendLine(",'T'");
            strQry.AppendLine(",EE.EARNING_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",1");

            //Append Initialised Numeric Fields
            strQry.Append(strFieldNamesInitialised);

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EE");
            strQry.AppendLine(",InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(",InteractPayroll_#CompanyNo#.dbo.EARNING E");
           
            strQry.AppendLine(" WHERE EE.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EE.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(" AND EE.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND EPC.EMPLOYEE_NO = EE.EMPLOYEE_NO");
            strQry.AppendLine(" AND EE.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" AND EE.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND EE.EARNING_NO = E.EARNING_NO ");
            strQry.AppendLine(" AND EE.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EE.DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" AND EE.EMPLOYEE_NO IN (" + parstrEmployeeNoIn + ")");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            //Normal Leave
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
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",'" + parDateTime.ToString("yyyy-MM-dd") + "'");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",200");
           
            strQry.AppendLine(",'Take-On Balance'");
            strQry.AppendLine(",0");
            strQry.AppendLine(",'" + parDateTime.ToString("yyyy-MM-dd") + "'");
            strQry.AppendLine(",'" + parDateTime.ToString("yyyy-MM-dd") + "'");
            strQry.AppendLine(",0");
           
            //Errol Added 2012-01-10
            strQry.AppendLine(",'D'");
            strQry.AppendLine(",0");
            strQry.AppendLine(",0");

            strQry.AppendLine(",99");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            
            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_NO IN (" + parstrEmployeeNoIn + ")");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
           
            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            //Sick Leave
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
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",'" + parDateTime.ToString("yyyy-MM-dd") + "'");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",201");
           
            strQry.AppendLine(",'Take-On Balance'");
            strQry.AppendLine(",0");
            strQry.AppendLine(",'" + parDateTime.ToString("yyyy-MM-dd") + "'");
            strQry.AppendLine(",'" + parDateTime.ToString("yyyy-MM-dd") + "'");
            strQry.AppendLine(",0");
           
            //Errol Added 2012-01-10
            strQry.AppendLine(",'D'");
            strQry.AppendLine(",0");
            strQry.AppendLine(",0");

            strQry.AppendLine(",99");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
           
            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_NO IN (" + parstrEmployeeNoIn + ")");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            
            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
          
            byte[] bytCompress = Get_Form_Records(parint64CompanyNo, parstrTimeAttendInd);
          
            return bytCompress;
        }

        public int Backup_DataBase(Int64 parInt64CompanyNo,string parstrPayrollType, string parstrOption)
        {
            int intReturnCode = 9;

            try
            {
                StringBuilder strQry = new StringBuilder();

                DataSet pvtDataSet = new DataSet();

                strQry.Clear();
                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" BACKUP_DATABASE_PATH");
                strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_PATH");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), pvtDataSet, "Directory", -1);

                string strFileDirectory = pvtDataSet.Tables["Directory"].Rows[0]["BACKUP_DATABASE_PATH"].ToString();

                string strDataBaseName = "InteractPayroll_" + parInt64CompanyNo.ToString("00000");

                string strBackupFileName = strDataBaseName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_BeforeEmployeeTake-On.bak";

                if (parstrOption == "A")
                {
                    strBackupFileName = strDataBaseName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_AfterEmployeeTake_On.bak";
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
    }
}
