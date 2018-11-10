using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace InteractPayroll
{
    public class busBatchEarning
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busBatchEarning()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo, Int64 parintCurrentUserNo, string parstrCurrentUserAccessInd, string parstrFromProgram, string parstrMenuId)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" WAGE_RUN_IND");
            strQry.AppendLine(",SALARY_RUN_IND");
            strQry.AppendLine(",TIME_ATTENDANCE_RUN_IND");
        
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY ");
        
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
        
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", parint64CompanyNo);

            if (parstrMenuId == "57")
            {
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EE.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",E.EARNING_NO");
                strQry.AppendLine(",E.EARNING_DESC");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EE");

                //2017-01-24 (Employee Not Closed)
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E1");
                strQry.AppendLine(" ON EE.COMPANY_NO = E1.COMPANY_NO ");
                strQry.AppendLine(" AND EE.EMPLOYEE_NO = E1.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EE.PAY_CATEGORY_TYPE = E1.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E1.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E1.EMPLOYEE_ENDDATE IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING E");
                strQry.AppendLine(" ON EE.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND EE.EARNING_NO = E.EARNING_NO ");
                strQry.AppendLine(" AND EE.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

                if (parstrCurrentUserAccessInd == "U")
                {
                    strQry.AppendLine(" INNER JOIN ");

                    strQry.AppendLine("(SELECT DISTINCT");
                    strQry.AppendLine(" PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EMPLOYEE_NO");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP ");
                    strQry.AppendLine(" WHERE USER_NO = " + parintCurrentUserNo);
                    strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo + ") AS USER_TABLE");

                    strQry.AppendLine(" ON EE.PAY_CATEGORY_TYPE = USER_TABLE.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EE.EMPLOYEE_NO = USER_TABLE.EMPLOYEE_NO");
                }

                strQry.AppendLine(" WHERE EE.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND EE.EMPLOYEE_NO > 0");
                strQry.AppendLine(" AND EE.EARNING_TYPE_IND = 'U' ");

                if (parstrFromProgram == "X")
                {
                    strQry.AppendLine(" AND EE.PAY_CATEGORY_TYPE = 'T'");
                }
                else
                {
                    strQry.AppendLine(" AND EE.PAY_CATEGORY_TYPE IN ('W','S')");
                }

                strQry.AppendLine(" AND EE.DATETIME_DELETE_RECORD IS NULL ");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" EE.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",E.EARNING_NO");
                strQry.AppendLine(",E.EARNING_DESC");

                strQry.AppendLine(" UNION ");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",E.EARNING_NO");
                strQry.AppendLine(",E.EARNING_DESC");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING E");

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.EARNING_NO = 7 ");

                strQry.AppendLine(" UNION ");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",E.EARNING_NO");
                strQry.AppendLine(",E.EARNING_DESC");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING E");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C");
                strQry.AppendLine(" ON E.COMPANY_NO = C.COMPANY_NO ");
                strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL ");

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");

                strQry.AppendLine(" AND ((((E.EARNING_NO = 2)");

                strQry.AppendLine(" OR (E.EARNING_NO = 3");
                strQry.AppendLine(" AND C.SALARY_OVERTIME1_RATE <> 0)");

                strQry.AppendLine(" OR (E.EARNING_NO = 4");
                strQry.AppendLine(" AND C.SALARY_OVERTIME2_RATE <> 0)");

                strQry.AppendLine(" OR (E.EARNING_NO = 5");
                strQry.AppendLine(" AND C.SALARY_OVERTIME3_RATE <> 0))))");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" 3");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Earning", parint64CompanyNo);

                DataSet.Tables.Add("PayrollType");
                DataTable PayrollTypeDataTable = new DataTable("PayrollType");
                DataSet.Tables["PayrollType"].Columns.Add("PAYROLL_TYPE_DESC", typeof(String));

                if (DataSet.Tables["Earning"].Rows.Count > 0)
                {
                    DataView PayrollTypeDataView = new DataView(DataSet.Tables["Earning"],
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
                    PayrollTypeDataView = new DataView(DataSet.Tables["Earning"],
                        "PAY_CATEGORY_TYPE = 'S'",
                        "",
                        DataViewRowState.CurrentRows);

                    if (PayrollTypeDataView.Count > 0)
                    {
                        DataRow drDataRow = DataSet.Tables["PayrollType"].NewRow();

                        drDataRow["PAYROLL_TYPE_DESC"] = "Salaries";

                        DataSet.Tables["PayrollType"].Rows.Add(drDataRow);
                    }

                    PayrollTypeDataView = null;
                    PayrollTypeDataView = new DataView(DataSet.Tables["Earning"],
                        "PAY_CATEGORY_TYPE = 'T'",
                        "",
                        DataViewRowState.CurrentRows);

                    if (PayrollTypeDataView.Count > 0)
                    {
                        DataRow drDataRow = DataSet.Tables["PayrollType"].NewRow();

                        drDataRow["PAYROLL_TYPE_DESC"] = "Time Attendance";

                        DataSet.Tables["PayrollType"].Rows.Add(drDataRow);
                    }
                }
            }
            else
            {
                //Deduction
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" ED.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",D.DEDUCTION_NO");

                strQry.AppendLine(",D.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",D.DEDUCTION_DESC");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ED");

                //2017-01-24 (Employee Not Closed)
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON ED.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND ED.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ED.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.DEDUCTION D");
                strQry.AppendLine(" ON ED.COMPANY_NO = D.COMPANY_NO ");
                strQry.AppendLine(" AND ED.DEDUCTION_NO = D.DEDUCTION_NO ");
                strQry.AppendLine(" AND ED.DEDUCTION_SUB_ACCOUNT_NO = D.DEDUCTION_SUB_ACCOUNT_NO ");
                strQry.AppendLine(" AND ED.PAY_CATEGORY_TYPE = D.PAY_CATEGORY_TYPE ");
                //Not Loan Type
                strQry.AppendLine(" AND ISNULL(D.DEDUCTION_LOAN_TYPE_IND,'N') <> 'Y' ");
                strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL ");

                if (parstrCurrentUserAccessInd == "U")
                {
                    strQry.AppendLine(" INNER JOIN ");

                    strQry.AppendLine("(SELECT DISTINCT");
                    strQry.AppendLine(" PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EMPLOYEE_NO");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP ");
                    strQry.AppendLine(" WHERE USER_NO = " + parintCurrentUserNo);
                    strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo + ") AS USER_TABLE");

                    strQry.AppendLine(" ON ED.PAY_CATEGORY_TYPE = USER_TABLE.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND ED.EMPLOYEE_NO = USER_TABLE.EMPLOYEE_NO");
                }

                strQry.AppendLine(" WHERE ED.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND ED.EMPLOYEE_NO > 0");
                strQry.AppendLine(" AND ED.DEDUCTION_TYPE_IND = 'U' ");

                if (parstrFromProgram == "X")
                {
                    strQry.AppendLine(" AND ED.PAY_CATEGORY_TYPE = 'T'");
                }
                else
                {
                    strQry.AppendLine(" AND ED.PAY_CATEGORY_TYPE IN ('W','S')");
                }

                strQry.AppendLine(" AND ED.DATETIME_DELETE_RECORD IS NULL ");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" ED.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",D.DEDUCTION_NO");
                strQry.AppendLine(",D.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",D.DEDUCTION_DESC");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Deduction", parint64CompanyNo);

                DataSet.Tables.Add("PayrollType");
                DataTable PayrollTypeDataTable = new DataTable("PayrollType");
                DataSet.Tables["PayrollType"].Columns.Add("PAYROLL_TYPE_DESC", typeof(String));

                if (DataSet.Tables["Deduction"].Rows.Count > 0)
                {
                    DataView PayrollTypeDataView = new DataView(DataSet.Tables["Deduction"],
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
                    PayrollTypeDataView = new DataView(DataSet.Tables["Deduction"],
                        "PAY_CATEGORY_TYPE = 'S'",
                        "",
                        DataViewRowState.CurrentRows);

                    if (PayrollTypeDataView.Count > 0)
                    {
                        DataRow drDataRow = DataSet.Tables["PayrollType"].NewRow();

                        drDataRow["PAYROLL_TYPE_DESC"] = "Salaries";

                        DataSet.Tables["PayrollType"].Rows.Add(drDataRow);
                    }

                    PayrollTypeDataView = null;
                    PayrollTypeDataView = new DataView(DataSet.Tables["Deduction"],
                        "PAY_CATEGORY_TYPE = 'T'",
                        "",
                        DataViewRowState.CurrentRows);

                    if (PayrollTypeDataView.Count > 0)
                    {
                        DataRow drDataRow = DataSet.Tables["PayrollType"].NewRow();

                        drDataRow["PAYROLL_TYPE_DESC"] = "Time Attendance";

                        DataSet.Tables["PayrollType"].Rows.Add(drDataRow);
                    }
                }
            }

            DataSet.AcceptChanges();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(SELECT DISTINCT");
                strQry.AppendLine(" PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EMPLOYEE_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP ");
                strQry.AppendLine(" WHERE USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo + ") AS USER_TABLE");

                strQry.AppendLine(" ON E.PAY_CATEGORY_TYPE = USER_TABLE.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = USER_TABLE.EMPLOYEE_NO");
            }

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            
            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE IN ('W','S')");
            }

            strQry.AppendLine(" AND EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" E.EMPLOYEE_CODE");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parint64CompanyNo);

            if (parstrMenuId == "57")
            {
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EE.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EE.EARNING_NO");
                strQry.AppendLine(",EE.EMPLOYEE_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EE");

                //2017-01-24 (Employee Not Closed)
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON EE.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND EE.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EE.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                if (parstrCurrentUserAccessInd == "U")
                {
                    strQry.AppendLine(" INNER JOIN ");

                    strQry.AppendLine("(SELECT DISTINCT");
                    strQry.AppendLine(" PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EMPLOYEE_NO");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP ");
                    strQry.AppendLine(" WHERE USER_NO = " + parintCurrentUserNo);
                    strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo + ") AS USER_TABLE");

                    strQry.AppendLine(" ON EE.PAY_CATEGORY_TYPE = USER_TABLE.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND EE.EMPLOYEE_NO = USER_TABLE.EMPLOYEE_NO");
                }

                strQry.AppendLine(" WHERE EE.COMPANY_NO = " + parint64CompanyNo);
                //Not Default Record - Must Be Linked
                strQry.AppendLine(" AND EE.EMPLOYEE_NO > 0");

                strQry.AppendLine(" AND EE.EARNING_TYPE_IND = 'U' ");

                if (parstrFromProgram == "X")
                {
                    strQry.AppendLine(" AND EE.PAY_CATEGORY_TYPE = 'T'");
                }
                else
                {
                    strQry.AppendLine(" AND EE.PAY_CATEGORY_TYPE IN ('W','S')");
                }

                strQry.AppendLine(" AND EE.DATETIME_DELETE_RECORD IS NULL ");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" EE.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EE.EARNING_NO");
                strQry.AppendLine(",EE.EMPLOYEE_NO");

                strQry.AppendLine(" UNION ");

                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" E.PAY_CATEGORY_TYPE");

                //7=Bonus
                strQry.AppendLine(",CONVERT(SMALLINT,7) AS EARNING_NO");
                strQry.AppendLine(",E.EMPLOYEE_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                if (parstrCurrentUserAccessInd == "U")
                {
                    strQry.AppendLine(" INNER JOIN ");

                    strQry.AppendLine("(SELECT DISTINCT");
                    strQry.AppendLine(" PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EMPLOYEE_NO");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP ");
                    strQry.AppendLine(" WHERE USER_NO = " + parintCurrentUserNo);
                    strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo + ") AS USER_TABLE");

                    strQry.AppendLine(" ON E.PAY_CATEGORY_TYPE = USER_TABLE.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = USER_TABLE.EMPLOYEE_NO");
                }

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);

                if (parstrFromProgram == "X")
                {
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T'");
                }
                else
                {
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE IN ('W','S')");
                }

                strQry.AppendLine(" AND EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

                strQry.AppendLine(" UNION ");

                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EN.EARNING_NO");
                strQry.AppendLine(",E.EMPLOYEE_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                if (parstrCurrentUserAccessInd == "U")
                {
                    strQry.AppendLine(" INNER JOIN ");

                    strQry.AppendLine("(SELECT DISTINCT");
                    strQry.AppendLine(" PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EMPLOYEE_NO");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP ");
                    strQry.AppendLine(" WHERE USER_NO = " + parintCurrentUserNo);
                    strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo + ") AS USER_TABLE");

                    strQry.AppendLine(" ON E.PAY_CATEGORY_TYPE = USER_TABLE.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND E.EMPLOYEE_NO = USER_TABLE.EMPLOYEE_NO");
                }

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EN");
                strQry.AppendLine(" ON E.COMPANY_NO = EN.COMPANY_NO ");
                strQry.AppendLine(" AND EN.EARNING_NO IN (2,3,4,5)");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL ");

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);

                if (parstrFromProgram == "X")
                {
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T'");
                }
                else
                {
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE IN ('W','S')");
                }

                strQry.AppendLine(" AND EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" 1");
                strQry.AppendLine(",2");
                strQry.AppendLine(",3");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeEarning", parint64CompanyNo);

                DataSet TempDataSet = Get_Employee_Earning_Amount(parint64CompanyNo, "", parintCurrentUserNo, parstrCurrentUserAccessInd, parstrFromProgram);
                DataSet.Merge(TempDataSet);

                TempDataSet.Dispose();
                TempDataSet = null;
            }
            else
            {
                //Deduction
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" ED.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",ED.DEDUCTION_NO");
                strQry.AppendLine(",ED.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",ED.EMPLOYEE_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ED");

                //2017-01-24 (Employee Not Closed)
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON ED.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND ED.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ED.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.DEDUCTION D");
                strQry.AppendLine(" ON ED.COMPANY_NO = D.COMPANY_NO ");
                strQry.AppendLine(" AND ED.DEDUCTION_NO = D.DEDUCTION_NO ");
                strQry.AppendLine(" AND ED.DEDUCTION_SUB_ACCOUNT_NO = D.DEDUCTION_SUB_ACCOUNT_NO ");
                strQry.AppendLine(" AND ED.PAY_CATEGORY_TYPE = D.PAY_CATEGORY_TYPE ");
                //Not Loan Type
                strQry.AppendLine(" AND ISNULL(D.DEDUCTION_LOAN_TYPE_IND,'N') <> 'Y' ");
                strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL ");

                if (parstrCurrentUserAccessInd == "U")
                {
                    strQry.AppendLine(" INNER JOIN ");

                    strQry.AppendLine("(SELECT DISTINCT");
                    strQry.AppendLine(" PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EMPLOYEE_NO");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP ");
                    strQry.AppendLine(" WHERE USER_NO = " + parintCurrentUserNo);
                    strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo + ") AS USER_TABLE");

                    strQry.AppendLine(" ON ED.PAY_CATEGORY_TYPE = USER_TABLE.PAY_CATEGORY_TYPE");
                    strQry.AppendLine(" AND ED.EMPLOYEE_NO = USER_TABLE.EMPLOYEE_NO");
                }

                strQry.AppendLine(" WHERE ED.COMPANY_NO = " + parint64CompanyNo);
                //Not Default Record - Must Be Linked
                strQry.AppendLine(" AND ED.EMPLOYEE_NO > 0");

                strQry.AppendLine(" AND ED.DEDUCTION_TYPE_IND = 'U' ");

                if (parstrFromProgram == "X")
                {
                    strQry.AppendLine(" AND ED.PAY_CATEGORY_TYPE = 'T'");
                }
                else
                {
                    strQry.AppendLine(" AND ED.PAY_CATEGORY_TYPE IN ('W','S')");
                }

                strQry.AppendLine(" AND ED.DATETIME_DELETE_RECORD IS NULL ");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" ED.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",ED.DEDUCTION_NO");
                strQry.AppendLine(",ED.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",ED.EMPLOYEE_NO");
              
                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" ED.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",ED.DEDUCTION_NO");
                strQry.AppendLine(",ED.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",ED.EMPLOYEE_NO");
              
                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeDeduction", parint64CompanyNo);

                DataSet TempDataSet = Get_Employee_Deduction_Amount(parint64CompanyNo, "", parintCurrentUserNo, parstrCurrentUserAccessInd, parstrFromProgram);
                DataSet.Merge(TempDataSet);

                TempDataSet.Dispose();
                TempDataSet = null;
            }

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        private DataSet Get_Employee_Deduction_Amount(Int64 parint64CompanyNo,string parstrPayrollTypeFilter, Int64 parInt64CurrentUserNo, string parstrCurrentUserAccessInd, string parstrFromProgram)
        {
            DataSet DataSet = new System.Data.DataSet();
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EDBT.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EDBT.DEDUCTION_NO");
            strQry.AppendLine(",EDBT.DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",EDBT.EMPLOYEE_NO");
            strQry.AppendLine(",EDBT.PROCESS_NO");
            strQry.AppendLine(",EDBT.AMOUNT");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_BATCH_TEMP EDBT");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON EDBT.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND EDBT.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EDBT.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(SELECT DISTINCT");
                strQry.AppendLine(" PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EMPLOYEE_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP ");
                strQry.AppendLine(" WHERE USER_NO = " + parInt64CurrentUserNo);
                strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo + ") AS USER_TABLE");

                strQry.AppendLine(" ON EDBT.PAY_CATEGORY_TYPE = USER_TABLE.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EDBT.EMPLOYEE_NO = USER_TABLE.EMPLOYEE_NO");
            }

            strQry.AppendLine(" WHERE EDBT.COMPANY_NO = " + parint64CompanyNo);

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND EDBT.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND EDBT.PAY_CATEGORY_TYPE IN ('W','S')");
            }

            if (parstrPayrollTypeFilter != "")
            {
                strQry.AppendLine(" AND EDBT.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollTypeFilter));
            }

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeDeductionAmount", parint64CompanyNo);

            return DataSet;
        }

        private DataSet Get_Employee_Earning_Amount(Int64 parint64CompanyNo, string parstrPayrollTypeFilter, Int64 parInt64CurrentUserNo, string parstrCurrentUserAccessInd, string parstrFromProgram)
        {
            DataSet DataSet = new System.Data.DataSet();
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EEBT.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EEBT.EARNING_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",EEBT.EMPLOYEE_NO");
            strQry.AppendLine(",EEBT.PROCESS_NO");
            strQry.AppendLine(",EEBT.AMOUNT");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_BATCH_TEMP EEBT");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON EEBT.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND EEBT.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EEBT.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

            if (parstrCurrentUserAccessInd == "U")
            {
                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(SELECT DISTINCT");
                strQry.AppendLine(" PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EMPLOYEE_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP ");
                strQry.AppendLine(" WHERE USER_NO = " + parInt64CurrentUserNo);
                strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo + ") AS USER_TABLE");

                strQry.AppendLine(" ON EEBT.PAY_CATEGORY_TYPE = USER_TABLE.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EEBT.EMPLOYEE_NO = USER_TABLE.EMPLOYEE_NO");
            }

            strQry.AppendLine(" WHERE EEBT.COMPANY_NO = " + parint64CompanyNo);

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND EEBT.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND EEBT.PAY_CATEGORY_TYPE IN ('W','S')");
            }

            if (parstrPayrollTypeFilter != "")
            {
                strQry.AppendLine(" AND EEBT.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollTypeFilter));
            }

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeEarningAmount", parint64CompanyNo);

            return DataSet;
        }

        public byte[] Insert_Records(Int64 parint64CompanyNo, string parstrPayrollType, Int64 parint64CurrentUserNo, string parstrCurrentUserAccessInd, string parstrFromProgramInd, int parintEarningDeductionNo, int parintDeductionSubAccountNo, Int16 parint16ProcessNo, double pardblAmount, string parstrEmployeeNos, string parstrMenuId)
        {
            DataSet DataSet = new DataSet();
            
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();

            strQry.AppendLine(" SELECT");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" WAGE_RUN_IND AS RUN_IND");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" SALARY_RUN_IND AS RUN_IND ");
                }
                else
                {
                    strQry.AppendLine(" TIME_ATTEND_RUN_IND AS RUN_IND ");
                }
            }

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyCheck", parint64CompanyNo);

            if (DataSet.Tables["CompanyCheck"].Rows[0]["RUN_IND"].ToString() == "")
            {
                if (parstrMenuId == "57")
                {
                    strQry.Clear();

                    strQry.AppendLine("INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_BATCH_TEMP ");
                    strQry.AppendLine("(COMPANY_NO ");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(",EARNING_NO ");
                    strQry.AppendLine(",EMPLOYEE_NO ");

                    strQry.AppendLine(",PROCESS_NO ");
                    strQry.AppendLine(",AMOUNT)");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                    strQry.AppendLine("," + parintEarningDeductionNo);
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine("," + parint16ProcessNo);
                    strQry.AppendLine("," + pardblAmount.ToString("0000000000.00"));

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

                    strQry.AppendLine(" AND E.EMPLOYEE_NO IN (" + parstrEmployeeNos + ")");

                    strQry.AppendLine(" AND EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");
                }
                else
                {
                    strQry.Clear();

                    strQry.AppendLine("INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_BATCH_TEMP ");
                    strQry.AppendLine("(COMPANY_NO ");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE ");
                    strQry.AppendLine(",DEDUCTION_NO ");
                    strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO ");
                    strQry.AppendLine(",EMPLOYEE_NO ");
                    strQry.AppendLine(",PROCESS_NO ");
                    strQry.AppendLine(",AMOUNT)");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" E.COMPANY_NO");
                    strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                    strQry.AppendLine("," + parintEarningDeductionNo);
                    strQry.AppendLine("," + parintDeductionSubAccountNo);
                    strQry.AppendLine(",E.EMPLOYEE_NO");
                    strQry.AppendLine("," + parint16ProcessNo);
                    strQry.AppendLine("," + pardblAmount.ToString("0000000000.00"));

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

                    strQry.AppendLine(" AND E.EMPLOYEE_NO IN (" + parstrEmployeeNos + ")");

                    strQry.AppendLine(" AND EMPLOYEE_TAKEON_IND = 'Y'");
                    strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");
                }

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            }

            DataSet TempDataSet = null;

            if (parstrMenuId == "57")
            {
                TempDataSet = Get_Employee_Earning_Amount(parint64CompanyNo, parstrPayrollType, parint64CurrentUserNo, parstrCurrentUserAccessInd, parstrFromProgramInd);
            }
            else
            {
                TempDataSet = Get_Employee_Deduction_Amount(parint64CompanyNo, parstrPayrollType, parint64CurrentUserNo, parstrCurrentUserAccessInd, parstrFromProgramInd);
            }

            DataSet.Merge(TempDataSet);

            TempDataSet.Dispose();
            TempDataSet = null;

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        
        //2017-08-14
        public byte[] Insert_Records_From_File_Import(Int64 parint64CompanyNo, string parstrPayrollType, Int64 parint64CurrentUserNo, string parstrCurrentUserAccessInd, string parstrFromProgramInd, int parintEarningDeductionNo, int parintDeductionSubAccountNo, Int16 parint16ProcessNo, string parstrEmployeeNos, string parstrEmployeeAmounts, string parstrMenuId)
        {
            DataSet DataSet = new DataSet();

            StringBuilder strQry = new StringBuilder();

            strQry.Clear();

            strQry.AppendLine(" SELECT");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" WAGE_RUN_IND AS RUN_IND");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" SALARY_RUN_IND AS RUN_IND ");
                }
                else
                {
                    strQry.AppendLine(" TIME_ATTEND_RUN_IND AS RUN_IND ");
                }
            }

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyCheck", parint64CompanyNo);

            if (DataSet.Tables["CompanyCheck"].Rows[0]["RUN_IND"].ToString() == "")
            {
                string[] strEmployeeNoArray = parstrEmployeeNos.Split(',');
                string[] strEmployeeAmountArray = parstrEmployeeAmounts.Split(',');

                for (int intCount = 0; intCount < strEmployeeNoArray.Length; intCount++)
                {
                    if (parstrMenuId == "57")
                    {
                        strQry.Clear();

                        strQry.AppendLine("INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_BATCH_TEMP ");
                        strQry.AppendLine("(COMPANY_NO ");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(",EARNING_NO ");
                        strQry.AppendLine(",EMPLOYEE_NO ");

                        strQry.AppendLine(",PROCESS_NO ");
                        strQry.AppendLine(",AMOUNT)");
                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO");
                        strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                        strQry.AppendLine("," + parintEarningDeductionNo);
                        strQry.AppendLine(",E.EMPLOYEE_NO");
                        strQry.AppendLine("," + parint16ProcessNo);
                        strQry.AppendLine("," + Convert.ToDecimal(strEmployeeAmountArray[intCount]).ToString("#########0.00"));

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                        strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

                        strQry.AppendLine(" AND E.EMPLOYEE_NO = " + strEmployeeNoArray[intCount]);

                        strQry.AppendLine(" AND EMPLOYEE_TAKEON_IND = 'Y'");
                        strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                        strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");
                    }
                    else
                    {
                        strQry.Clear();

                        strQry.AppendLine("INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_BATCH_TEMP ");
                        strQry.AppendLine("(COMPANY_NO ");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(",DEDUCTION_NO ");
                        strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO ");
                        strQry.AppendLine(",EMPLOYEE_NO ");
                        strQry.AppendLine(",PROCESS_NO ");
                        strQry.AppendLine(",AMOUNT)");
                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO");
                        strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                        strQry.AppendLine("," + parintEarningDeductionNo);
                        strQry.AppendLine("," + parintDeductionSubAccountNo);
                        strQry.AppendLine(",E.EMPLOYEE_NO");
                        strQry.AppendLine("," + parint16ProcessNo);
                        strQry.AppendLine("," + Convert.ToDecimal(strEmployeeAmountArray[intCount]).ToString("#########0.00"));

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                        strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

                        strQry.AppendLine(" AND E.EMPLOYEE_NO = " + strEmployeeNoArray[intCount]);

                        strQry.AppendLine(" AND EMPLOYEE_TAKEON_IND = 'Y'");
                        strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                        strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");
                    }

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                }
            }

            DataSet TempDataSet = null;

            if (parstrMenuId == "57")
            {
                TempDataSet = Get_Employee_Earning_Amount(parint64CompanyNo, parstrPayrollType, parint64CurrentUserNo, parstrCurrentUserAccessInd, parstrFromProgramInd);
            }
            else
            {
                TempDataSet = Get_Employee_Deduction_Amount(parint64CompanyNo, parstrPayrollType, parint64CurrentUserNo, parstrCurrentUserAccessInd, parstrFromProgramInd);
            }

            DataSet.Merge(TempDataSet);

            TempDataSet.Dispose();
            TempDataSet = null;

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }




















        public byte[] Delete_Records(Int64 parint64CompanyNo, string parstrPayrollType, Int64 parint64CurrentUserNo, string parstrCurrentUserAccessInd, string parstrFromProgramInd, string parstrMenuId, byte[] parbyteDataSet)
        {
            DataSet DataSet = new DataSet();

            StringBuilder strQry = new StringBuilder();

            strQry.Clear();

            strQry.AppendLine(" SELECT");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" WAGE_RUN_IND AS RUN_IND");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" SALARY_RUN_IND AS RUN_IND ");
                }
                else
                {
                    strQry.AppendLine(" TIME_ATTEND_RUN_IND AS RUN_IND ");
                }
            }

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyCheck", parint64CompanyNo);

            if (DataSet.Tables["CompanyCheck"].Rows[0]["RUN_IND"].ToString() == "")
            {
                DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

                string strTableName = "Earning";
                string strTablePrefix = "EARNING";

                if (parstrMenuId == "58")
                {
                    strTableName = "Deduction";
                    strTablePrefix = "DEDUCTION";
                }

                for (int intRow = 0; intRow < parDataSet.Tables["Employee" + strTableName + "Amount"].Rows.Count; intRow++)
                {
                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_" + strTablePrefix + "_BATCH_TEMP ");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee" + strTableName + "Amount"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine(" AND " + strTablePrefix + "_NO = " + parDataSet.Tables["Employee" + strTableName + "Amount"].Rows[intRow][strTablePrefix + "_NO"].ToString());

                    if (parstrMenuId == "58")
                    {
                        strQry.AppendLine(" AND DEDUCTION_SUB_ACCOUNT_NO = " + parDataSet.Tables["Employee" + strTableName + "Amount"].Rows[intRow]["DEDUCTION_SUB_ACCOUNT_NO"].ToString());
                    }

                    strQry.AppendLine(" AND EMPLOYEE_NO = " + parDataSet.Tables["Employee" + strTableName + "Amount"].Rows[intRow]["EMPLOYEE_NO"].ToString());

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                }
            }

            DataSet TempDataSet = null;

            if (parstrMenuId == "57")
            {
                TempDataSet = Get_Employee_Earning_Amount(parint64CompanyNo, parstrPayrollType, parint64CurrentUserNo, parstrCurrentUserAccessInd, parstrFromProgramInd);
            }
            else
            {
                TempDataSet = Get_Employee_Deduction_Amount(parint64CompanyNo, parstrPayrollType, parint64CurrentUserNo, parstrCurrentUserAccessInd, parstrFromProgramInd);
            }

            DataSet.Merge(TempDataSet);

            TempDataSet.Dispose();
            TempDataSet = null;

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Update_Record(Int64 parint64CompanyNo, string parstrPayrollType, Int64 parint64CurrentUserNo, string parstrCurrentUserAccessInd, string parstrFromProgramInd, string parstrMenuId, byte[] parbyteDataSet)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new System.Data.DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" WAGE_RUN_IND AS RUN_IND");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" SALARY_RUN_IND AS RUN_IND ");
                }
                else
                {
                    strQry.AppendLine(" TIME_ATTEND_RUN_IND AS RUN_IND ");
                }
            }

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyCheck", parint64CompanyNo);

            string strTableName = "Earning";
            string strTablePrefix = "EARNING";

            if (parstrMenuId == "58")
            {
                strTableName = "Deduction";
                strTablePrefix = "DEDUCTION";
            }

            if (DataSet.Tables["CompanyCheck"].Rows[0]["RUN_IND"].ToString() == "")
            {
                for (int intRow = 0; intRow < parDataSet.Tables["Employee" + strTableName + "Amount"].Rows.Count; intRow++)
                {
                    if (parDataSet.Tables["Employee" + strTableName + "Amount"].Rows[intRow].RowState == DataRowState.Deleted)
                    {
                        strQry.Clear();

                        strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_" + strTablePrefix + "_BATCH_TEMP");
                        strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee" + strTableName + "Amount"].Rows[intRow]["PAY_CATEGORY_TYPE", System.Data.DataRowVersion.Original].ToString()));
                        strQry.AppendLine(" AND " + strTablePrefix + "_NO = " + parDataSet.Tables["Employee" + strTableName + "Amount"].Rows[intRow][strTablePrefix + "_NO", System.Data.DataRowVersion.Original].ToString());

                        if (parstrMenuId == "58")
                        {
                            strQry.AppendLine(" AND DEDUCTION_SUB_ACCOUNT_NO = " + parDataSet.Tables["Employee" + strTableName + "Amount"].Rows[intRow]["DEDUCTION_SUB_ACCOUNT_NO", System.Data.DataRowVersion.Original].ToString());
                        }

                        strQry.AppendLine(" AND EMPLOYEE_NO = " + parDataSet.Tables["Employee" + strTableName + "Amount"].Rows[intRow]["EMPLOYEE_NO", System.Data.DataRowVersion.Original].ToString());
                    }
                    else
                    {
                        strQry.Clear();

                        strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_" + strTablePrefix + "_BATCH_TEMP");

                        strQry.AppendLine(" SET ");

                        strQry.AppendLine(" PROCESS_NO = " + parDataSet.Tables["Employee" + strTableName + "Amount"].Rows[intRow]["PROCESS_NO"].ToString());
                        strQry.AppendLine(",AMOUNT = " + parDataSet.Tables["Employee" + strTableName + "Amount"].Rows[intRow]["AMOUNT"].ToString());

                        strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee" + strTableName + "Amount"].Rows[intRow]["PAY_CATEGORY_TYPE", System.Data.DataRowVersion.Original].ToString()));
                        strQry.AppendLine(" AND " + strTablePrefix + "_NO = " + parDataSet.Tables["Employee" + strTableName + "Amount"].Rows[intRow][strTablePrefix + "_NO", System.Data.DataRowVersion.Original].ToString());

                        if (parstrMenuId == "58")
                        {
                            strQry.AppendLine(" AND DEDUCTION_SUB_ACCOUNT_NO = " + parDataSet.Tables["Employee" + strTableName + "Amount"].Rows[intRow]["DEDUCTION_SUB_ACCOUNT_NO", System.Data.DataRowVersion.Original].ToString());
                        }

                        strQry.AppendLine(" AND EMPLOYEE_NO = " + parDataSet.Tables["Employee" + strTableName + "Amount"].Rows[intRow]["EMPLOYEE_NO", System.Data.DataRowVersion.Original].ToString());
                    }

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                }
            }

            parDataSet.Dispose();
            parDataSet = null;

            DataSet TempDataSet = null;

            if (parstrMenuId == "57")
            {
                TempDataSet = Get_Employee_Earning_Amount(parint64CompanyNo, parstrPayrollType, parint64CurrentUserNo, parstrCurrentUserAccessInd, parstrFromProgramInd);
            }
            else
            {
                TempDataSet = Get_Employee_Deduction_Amount(parint64CompanyNo, parstrPayrollType, parint64CurrentUserNo, parstrCurrentUserAccessInd, parstrFromProgramInd);
            }

            DataSet.Merge(TempDataSet);

            TempDataSet.Dispose();
            TempDataSet = null;

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
    }
}
