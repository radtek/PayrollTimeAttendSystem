using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busEmployeeBalanceTakeOn
    {
        clsDBConnectionObjects clsDBConnectionObjects;
        
        public busEmployeeBalanceTakeOn()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parInt64CompanyNo)
        {
            DataSet DataSet = new DataSet();
            DataSet TempDataSet;

            string strEmployeeNoIn = "-1";
            StringBuilder strFieldNamesInitialised = new StringBuilder();
            StringBuilder strQry = new StringBuilder();

            //Insert Records 
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EDH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EDH.EMPLOYEE_NO");
            strQry.AppendLine(",EDH.PAY_PERIOD_DATE");
            strQry.AppendLine(",SUM(EDH.TOTAL)");
            strQry.AppendLine(",SUM(EEH.TOTAL)");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY EDH ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH ");
            strQry.AppendLine(" ON EDH.COMPANY_NO = EEH.COMPANY_NO ");
            strQry.AppendLine(" AND EDH.PAY_PERIOD_DATE = EEH.PAY_PERIOD_DATE ");
            strQry.AppendLine(" AND EDH.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EDH.EMPLOYEE_NO = EEH.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EDH.RUN_TYPE = EEH.RUN_TYPE ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");
            strQry.AppendLine(" ON EDH.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND EDH.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");

            //2013-06-21 Exclude T=Time Attendance
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE IN ('W','S') ");

            strQry.AppendLine(" AND EDH.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL ");

            strQry.AppendLine(" INNER JOIN ");

            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",MAX(PAY_PERIOD_DATE) AS MAX_PAY_PERIOD_DATE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY ");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            //2013-06-21 Exclude T=Time Attendance
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE IN ('W','S') ");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EMPLOYEE_NO) AS TEMP_EMPLOYEE_DATE ");

            strQry.AppendLine(" ON EDH.PAY_CATEGORY_TYPE = TEMP_EMPLOYEE_DATE.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EDH.EMPLOYEE_NO = TEMP_EMPLOYEE_DATE.EMPLOYEE_NO ");
     
            //Add 180 Days (Withinn 6 Months)
            strQry.AppendLine(" AND DATEADD(DD,180,EDH.PAY_PERIOD_DATE) >= MAX_PAY_PERIOD_DATE ");

            strQry.AppendLine(" WHERE EDH.COMPANY_NO = " + parInt64CompanyNo);

            //2013-06-21 Exclude T=Time Attendance
            strQry.AppendLine(" AND EDH.PAY_CATEGORY_TYPE IN ('W','S') ");

            strQry.AppendLine(" AND EDH.RUN_TYPE = 'T'");
            
            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" EDH.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",EDH.EMPLOYEE_NO ");
            strQry.AppendLine(",EDH.PAY_PERIOD_DATE");

            //Exclude where Already Have Take-On Values
            strQry.AppendLine(" HAVING SUM(EDH.TOTAL) = 0");
            strQry.AppendLine(" AND SUM(EEH.TOTAL) = 0 ");

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
                strQry.AppendLine("INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EMPLOYEE_NO");
                strQry.AppendLine(",RUN_TYPE");
                strQry.AppendLine(",DEDUCTION_NO");
                strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",RUN_NO");

                strFieldNamesInitialised.Clear();

                //Return strings for field that need to be Initialised to Zero
                clsDBConnectionObjects.Initialise_DataSet_Numeric_Fields("EMPLOYEE_DEDUCTION_CURRENT", ref strQry, ref strFieldNamesInitialised, parInt64CompanyNo);

                strQry.AppendLine(")");
               
                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" ED.COMPANY_NO");
                strQry.AppendLine(",ED.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",ED.EMPLOYEE_NO");
                strQry.AppendLine(",'T'");
                strQry.AppendLine(",ED.DEDUCTION_NO ");
                strQry.AppendLine(",ED.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",1");
                
                strQry.Append(strFieldNamesInitialised);
                
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ED ");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT EDC");
                strQry.AppendLine(" ON ED.COMPANY_NO = EDC.COMPANY_NO ");
                strQry.AppendLine(" AND ED.PAY_CATEGORY_TYPE = EDC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND ED.EMPLOYEE_NO = EDC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ED.DEDUCTION_NO = EDC.DEDUCTION_NO ");
                strQry.AppendLine(" AND ED.DEDUCTION_SUB_ACCOUNT_NO = EDC.DEDUCTION_SUB_ACCOUNT_NO ");
                strQry.AppendLine(" AND EDC.RUN_TYPE = 'T' ");

                strQry.AppendLine(" WHERE ED.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND ED.EMPLOYEE_NO = " + DataSet.Tables["EmployeeTemp"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                strQry.AppendLine(" AND ED.PAY_CATEGORY_TYPE = '" + DataSet.Tables["EmployeeTemp"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'");
                strQry.AppendLine(" AND ED.DATETIME_DELETE_RECORD IS NULL");

                //No EMPLOYEE_DEDUCTION_HISTORY Record Exists
                strQry.AppendLine(" AND EDC.COMPANY_NO IS NULL ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(),parInt64CompanyNo);
               
                strQry.Clear();
                strQry.AppendLine("INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                //NB PAY_CATEGORY_NO will be Set to Zero
                strQry.AppendLine(",EMPLOYEE_NO");
                strQry.AppendLine(",RUN_TYPE");
                strQry.AppendLine(",EARNING_NO");

                //ELR 2014-05-24
                strQry.AppendLine(",EARNING_TYPE_IND");

                strQry.AppendLine(",RUN_NO");

                strFieldNamesInitialised.Clear();

                //Return strings for field that need to be Initialised to Zero
                clsDBConnectionObjects.Initialise_DataSet_Numeric_Fields("EMPLOYEE_EARNING_CURRENT", ref strQry, ref strFieldNamesInitialised, parInt64CompanyNo);

                strQry.AppendLine(")");

                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" EN.COMPANY_NO");
                strQry.AppendLine(",EN.PAY_CATEGORY_TYPE");
                strQry.AppendLine("," + DataSet.Tables["EmployeeTemp"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                strQry.AppendLine(",'T'");
                strQry.AppendLine(",EN.EARNING_NO ");

                //ELR 2014-05-24
                strQry.AppendLine(",'U'");
                strQry.AppendLine(",1");

                //Append Initialised Numeric Fields Names
                strQry.Append(strFieldNamesInitialised);

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING EN ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C ");
                strQry.AppendLine(" ON EN.COMPANY_NO = C.COMPANY_NO ");
                strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");
                strQry.AppendLine(" ON EN.COMPANY_NO = EEC.COMPANY_NO ");
                strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = EEC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EN.EARNING_NO = EEC.EARNING_NO ");
                strQry.AppendLine(" AND EEC.RUN_TYPE = 'T' ");

                strQry.AppendLine(" WHERE EN.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = '" + DataSet.Tables["EmployeeTemp"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'");

                strQry.AppendLine(" AND (EN.EARNING_NO IN (1,2,7,8,9)");

                if (DataSet.Tables["EmployeeTemp"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                {
                    //Normal Leave / Sick Leave
                    strQry.AppendLine(" OR (EN.EARNING_NO IN (200,201))");
                    strQry.AppendLine(" OR (EN.EARNING_NO = 3 AND C.OVERTIME1_RATE > 0)");
                    strQry.AppendLine(" OR (EN.EARNING_NO = 4 AND C.OVERTIME2_RATE > 0)");
                    strQry.AppendLine(" OR (EN.EARNING_NO = 5 AND C.OVERTIME3_RATE > 0))");
                }
                else
                {
                    strQry.AppendLine(" OR (EN.EARNING_NO = 3 AND C.SALARY_OVERTIME1_RATE > 0)");
                    strQry.AppendLine(" OR (EN.EARNING_NO = 4 AND C.SALARY_OVERTIME2_RATE > 0)");
                    strQry.AppendLine(" OR (EN.EARNING_NO = 5 AND C.SALARY_OVERTIME3_RATE > 0))");
                }

                strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL");

                //No EMPLOYEE_EARNING_HISTORY Record Exists
                strQry.AppendLine(" AND EEC.COMPANY_NO IS NULL ");
               
                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
               
                strQry.Clear();
                strQry.AppendLine("INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EMPLOYEE_NO");
                strQry.AppendLine(",RUN_TYPE");
                strQry.AppendLine(",EARNING_NO");

                //ELR 2014-05-24
                strQry.AppendLine(",EARNING_TYPE_IND");
               
                strQry.AppendLine(",RUN_NO");

                strFieldNamesInitialised.Clear();

                //Return strings for field that need to be Initialised to Zero
                clsDBConnectionObjects.Initialise_DataSet_Numeric_Fields("EMPLOYEE_EARNING_CURRENT", ref strQry, ref strFieldNamesInitialised, parInt64CompanyNo);

                strQry.AppendLine(")");

                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" EE.COMPANY_NO");
                strQry.AppendLine(",EE.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EE.EMPLOYEE_NO");
                strQry.AppendLine(",'T'");
                strQry.AppendLine(",EE.EARNING_NO ");

                //ELR 2014-05-24
                strQry.AppendLine(",EE.EARNING_TYPE_IND");

                strQry.AppendLine(",1");
              
                //Append Initialised Numeric Fields Names
                strQry.Append(strFieldNamesInitialised);
                
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EE ");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");
                strQry.AppendLine(" ON EE.COMPANY_NO = EEC.COMPANY_NO ");
                strQry.AppendLine(" AND EE.PAY_CATEGORY_TYPE = EEC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EE.EMPLOYEE_NO = EEC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EE.EARNING_NO = EEC.EARNING_NO ");
                strQry.AppendLine(" AND EEC.RUN_TYPE = 'T' ");

                strQry.AppendLine(" WHERE EE.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EE.EMPLOYEE_NO = " + DataSet.Tables["EmployeeTemp"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                strQry.AppendLine(" AND EE.PAY_CATEGORY_TYPE = '" + DataSet.Tables["EmployeeTemp"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'");
                strQry.AppendLine(" AND EE.DATETIME_DELETE_RECORD IS NULL");

                //No EMPLOYEE_EARNING_HISTORY Record Exists
                strQry.AppendLine(" AND EEC.COMPANY_NO IS NULL ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            }

            //First Delete Records That are older than 180 Days
            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND RUN_TYPE = 'T' ");
            strQry.AppendLine(" AND EMPLOYEE_NO NOT IN (" + strEmployeeNoIn + ")");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            //First Delete Records That are Older than 180 Days
            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND RUN_TYPE = 'T' ");
            strQry.AppendLine(" AND EMPLOYEE_NO NOT IN (" + strEmployeeNoIn + ")");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                  
            //EERORL CARRY HERE
            DataSet.Tables.Add("PayrollType");
            DataTable PayrollTypeDataTable = new DataTable("PayrollType");
            DataSet.Tables["PayrollType"].Columns.Add("PAYROLL_TYPE_DESC", typeof(String));

            if (DataSet.Tables["EmployeeTemp"].Rows.Count > 0)
            {
                DataView PayrollTypeDataView = new DataView(DataSet.Tables["EmployeeTemp"],
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
                PayrollTypeDataView = new DataView(DataSet.Tables["EmployeeTemp"],
                    "PAY_CATEGORY_TYPE = 'S'",
                    "",
                    DataViewRowState.CurrentRows);

                if (PayrollTypeDataView.Count > 0)
                {
                    DataRow drDataRow = DataSet.Tables["PayrollType"].NewRow();

                    drDataRow["PAYROLL_TYPE_DESC"] = "Salaries";

                    DataSet.Tables["PayrollType"].Rows.Add(drDataRow);
                }

                DataSet.Tables.Remove("EmployeeTemp");

                DataSet.AcceptChanges();

                byte[] bytTempCompress = Get_Company_Records(parInt64CompanyNo, DataSet.Tables["PayrollType"].Rows[0]["PAYROLL_TYPE_DESC"].ToString().Substring(0,1));
                TempDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(bytTempCompress);
                DataSet.Merge(TempDataSet);
            }

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_Company_Records(Int64 parInt64CompanyNo, string parstrPayrollType)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
            DateTime dtStartTaxYear;
            object[] objFind = new object[2];
                      
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO ");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",MAX(EMPLOYEE_LAST_RUNDATE) AS PAY_PERIOD_DATE ");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" COMPANY_NO ");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", parInt64CompanyNo);

            if (DataSet.Tables["Company"].Rows.Count > 0)
            {
                if (Convert.ToDateTime(DataSet.Tables["Company"].Rows[0]["PAY_PERIOD_DATE"]).Month > 2)
                {
                    dtStartTaxYear = new DateTime(Convert.ToDateTime(DataSet.Tables["Company"].Rows[0]["PAY_PERIOD_DATE"]).Year, 3, 1);
                }
                else
                {
                    dtStartTaxYear = new DateTime(Convert.ToDateTime(DataSet.Tables["Company"].Rows[0]["PAY_PERIOD_DATE"]).Year - 1, 3, 1);
                }

                strQry.Clear();
                strQry.AppendLine(" SELECT DISTINCT");
                strQry.AppendLine(" E.COMPANY_NO");
                strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",E.EMPLOYEE_CODE");
                strQry.AppendLine(",E.EMPLOYEE_SURNAME");
                strQry.AppendLine(",E.EMPLOYEE_NAME");
                strQry.AppendLine(",E.EMPLOYEE_TAX_STARTDATE");
            
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC ");
                strQry.AppendLine(" ON E.COMPANY_NO = EEC.COMPANY_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EEC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EEC.EMPLOYEE_NO");
                strQry.AppendLine(" AND EEC.RUN_TYPE = 'T'");
                
                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parInt64CompanyNo);
                
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",DEDUCTION_NO");
                strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_COUNT");
                strQry.AppendLine(",DEDUCTION_DESC");
                strQry.AppendLine(",IRP5_CODE");
                
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.DEDUCTION ");
                
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" COMPANY_NO");
                strQry.AppendLine(",DEDUCTION_NO");
                strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "DeductionDesc", parInt64CompanyNo);
              
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EN.COMPANY_NO");
                strQry.AppendLine(",EN.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EN.EARNING_NO");
                strQry.AppendLine(",EN.EARNING_DESC");
                strQry.AppendLine(",EN.IRP5_CODE");
                
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING EN ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");
                strQry.AppendLine(" ON EN.COMPANY_NO = EEC.COMPANY_NO ");
                strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = EEC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EN.EARNING_NO = EEC.EARNING_NO ");
                strQry.AppendLine(" AND EEC.RUN_TYPE = 'T' ");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" EN.COMPANY_NO");
                strQry.AppendLine(",EN.EARNING_NO");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EarningDesc", parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" COMPANY_NO");
                strQry.AppendLine(",EMPLOYEE_NO");
                strQry.AppendLine(",DEDUCTION_NO");
                strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",TOTAL");
                
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT  ");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND RUN_TYPE = 'T'");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeDeduction", parInt64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" COMPANY_NO");
                strQry.AppendLine(",EMPLOYEE_NO");
                strQry.AppendLine(",EARNING_NO");
                strQry.AppendLine(",PAY_CATEGORY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",TOTAL");
               
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT ");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND RUN_TYPE = 'T'");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeEarning", parInt64CompanyNo);
            }

            DataSet.AcceptChanges();

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public void Update_Records(Int64 parInt64CompanyNo,byte[] parbyteDataSet, string parstrPayrollType)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);
            
            StringBuilder strQry = new StringBuilder();

            for (int intRow = 0; intRow < parDataSet.Tables["EmployeeEarning"].Rows.Count; intRow++)
            {
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" TOTAL = " + Convert.ToDouble(parDataSet.Tables["EmployeeEarning"].Rows[intRow]["TOTAL"]));
                strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToDouble(parDataSet.Tables["EmployeeEarning"].Rows[intRow]["COMPANY_NO"]));
                strQry.AppendLine(" AND EARNING_NO = " + Convert.ToDouble(parDataSet.Tables["EmployeeEarning"].Rows[intRow]["EARNING_NO"]));
                strQry.AppendLine(" AND EMPLOYEE_NO = " + Convert.ToDouble(parDataSet.Tables["EmployeeEarning"].Rows[intRow]["EMPLOYEE_NO"]));
                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToDouble(parDataSet.Tables["EmployeeEarning"].Rows[intRow]["PAY_CATEGORY_NO"]));
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EmployeeEarning"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                strQry.AppendLine(" AND RUN_TYPE = 'T'");


                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            }

            for (int intRow = 0; intRow < parDataSet.Tables["EmployeeDeduction"].Rows.Count; intRow++)
            {
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" TOTAL = " + Convert.ToDouble(parDataSet.Tables["EmployeeDeduction"].Rows[intRow]["TOTAL"]));
                strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToDouble(parDataSet.Tables["EmployeeDeduction"].Rows[intRow]["COMPANY_NO"]));
                strQry.AppendLine(" AND DEDUCTION_NO = " + Convert.ToDouble(parDataSet.Tables["EmployeeDeduction"].Rows[intRow]["DEDUCTION_NO"]));
                strQry.AppendLine(" AND DEDUCTION_SUB_ACCOUNT_NO = " + Convert.ToDouble(parDataSet.Tables["EmployeeDeduction"].Rows[intRow]["DEDUCTION_SUB_ACCOUNT_NO"]));
                strQry.AppendLine(" AND EMPLOYEE_NO = " + Convert.ToDouble(parDataSet.Tables["EmployeeDeduction"].Rows[intRow]["EMPLOYEE_NO"]));

                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

                strQry.AppendLine(" AND RUN_TYPE = 'T'");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            }

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }
    }
}
