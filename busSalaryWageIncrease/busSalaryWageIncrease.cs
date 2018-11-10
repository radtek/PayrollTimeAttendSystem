using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace InteractPayroll
{
    public class busSalaryWageIncrease
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        private SqlConnection pvtSqlConnection;
        private SqlCommand pvtSqlCommand;
        private SqlDataAdapter pvtSqlDataAdapter;
        private SqlParameter pvtSqlParameter;

        public busSalaryWageIncrease()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parInt64CompanyNo, Int64 parint64CurrentUserNo, string parstrCurrentUserAccess)
        {
            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND PC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }
            
            strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategory", parInt64CompanyNo);

            strQry.Clear();
            strQry = Get_Employee_Script(parint64CurrentUserNo, parstrCurrentUserAccess);

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parInt64CompanyNo);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        private StringBuilder Get_Employee_Script(Int64 parint64CurrentUserNo, string parstrCurrentUserAccess)
        {
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" E.EMPLOYEE_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_CODE");

            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");

            //Holds Wage Hourly Rate and Salary Monthly Rate
            strQry.AppendLine(",EPC.HOURLY_RATE");
            strQry.AppendLine(",EPC.OLD_HOURLY_RATE");

            strQry.AppendLine(",EPC.SALARY_WAGE_INCREASE");
            strQry.AppendLine(",EPC.SALARY_WAGE_INCREASE_DATETIME");

            //Used to Check if Payroll Run
            strQry.AppendLine(",PCPC.COMPANY_NO AS PAYROLL_LINK");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");

            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }
            
            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");
            strQry.AppendLine(" ON E.COMPANY_NO = PCPC.COMPANY_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PCPC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = PCPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND ((PCPC.RUN_TYPE = 'T'");
            strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND <> 'Y')");
            strQry.AppendLine(" OR (PCPC.RUN_TYPE = 'P'");
            strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'))");

            strQry.AppendLine(" WHERE E.DATETIME_DELETE_RECORD IS NULL ");
            //Not Closed
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

            return strQry;
        }

        public byte[] Update_Records(Int64 parInt64CompanyNo, Int64 parint64CurrentUserNo, string parstrCurrentUserAccess, int parintPaycategoryNo, string parstrPayrollType, string parstrType, double pardblIncrease, byte[] parbyteDataSet)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);
            StringBuilder strQry = new StringBuilder();
            double dblCurrentValue = 0;
            double dblNewValue = 0;
            string strEmployeeNos = "(";

            //Remove Rows from DataTable
            for (int intRow = 0; intRow < parDataSet.Tables["Employee"].Rows.Count; intRow++)
            {
                if (parstrType == "P")
                {
                    dblCurrentValue = Convert.ToDouble(parDataSet.Tables["Employee"].Rows[intRow]["HOURLY_RATE"]);
                    dblNewValue = dblCurrentValue + Math.Round((dblCurrentValue * pardblIncrease) / 100, 2);
                }

                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY");

                strQry.AppendLine(" SET ");
                strQry.AppendLine(" OLD_HOURLY_RATE = HOURLY_RATE");

                if (parstrType == "P")
                {
                    strQry.AppendLine(",HOURLY_RATE = " + dblNewValue);
                    strQry.AppendLine(",SALARY_WAGE_INCREASE = " + pardblIncrease);
                }
                else
                {
                    strQry.AppendLine(",HOURLY_RATE = " + parDataSet.Tables["Employee"].Rows[intRow]["HOURLY_RATE"].ToString());
                    strQry.AppendLine(",SALARY_WAGE_INCREASE = " + parDataSet.Tables["Employee"].Rows[intRow]["SALARY_WAGE_INCREASE"].ToString());
                }

                strQry.AppendLine(",SALARY_WAGE_INCREASE_DATETIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo.ToString());
                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPaycategoryNo.ToString());
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND EMPLOYEE_NO = " + parDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                if (intRow == 0)
                {
                    strEmployeeNos += parDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                }
                else
                {
                    strEmployeeNos += "," + parDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                }
            }

            strEmployeeNos += ")";

            DataSet DataSet = new System.Data.DataSet();

            strQry = Get_Employee_Script(parint64CurrentUserNo, parstrCurrentUserAccess);

            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + parintPaycategoryNo);
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
            strQry.AppendLine(" AND EPC.EMPLOYEE_NO IN " + strEmployeeNos);

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parInt64CompanyNo);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            return bytCompress;
        }
    }
}
