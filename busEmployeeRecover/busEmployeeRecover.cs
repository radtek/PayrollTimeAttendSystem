using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace InteractPayroll
{
    public class busEmployeeRecover
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busEmployeeRecover()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public bool Get_Form_Records(Int64 parInt64CompanyNo)
        {
            bool blnCanUpload = false;
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
           
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ALLOW_UPLOAD_OF_EMPLOYEES");

            strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
           
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

            if (DataSet.Tables["Temp"].Rows.Count > 0)
            {
                if (DataSet.Tables["Temp"].Rows[0]["ALLOW_UPLOAD_OF_EMPLOYEES"] == System.DBNull.Value)
                {
                }
                else
                {
                    if (Convert.ToBoolean(DataSet.Tables["Temp"].Rows[0]["ALLOW_UPLOAD_OF_EMPLOYEES"]) == true)
                    {
                        blnCanUpload = true;
                    }
                }
            }

            return blnCanUpload;
        }

        public void Upload_Records(Int64 parInt64CompanyNo, byte[] parbyteDataSet)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            if (parDataSet.Tables["Employee"].Rows.Count > 0)
            {
                for (int intRow = 0; intRow < parDataSet.Tables["Employee"].Rows.Count; intRow++)
                {
                    if (DataSet.Tables["Employee"] != null)
                    {
                        DataSet.Tables.Remove("Employee");
                    }

                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EMPLOYEE_NO");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + parDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString());

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parInt64CompanyNo);

                    if (DataSet.Tables["Employee"].Rows.Count == 0)
                    {
                        //Insert
                        strQry.Clear();

                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE ");
                        strQry.AppendLine("(COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",EMPLOYEE_CODE");
                        strQry.AppendLine(",EMPLOYEE_NAME");
                        strQry.AppendLine(",EMPLOYEE_SURNAME");
                        strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE");
                        strQry.AppendLine(",EMPLOYEE_TAX_STARTDATE");
                        strQry.AppendLine(",DEPARTMENT_NO");
                        strQry.AppendLine(",FIRST_RUN_COMPLETED_IND");
                        strQry.AppendLine(",EMPLOYEE_TAKEON_IND");
                        strQry.AppendLine(",UPLOAD_FROM_CLIENT_IND");
                        
                        strQry.AppendLine(",ANNUAL_SALARY");
                        strQry.AppendLine(",EMPLOYEE_TAX_OFFICE_NO");
                        strQry.AppendLine(",TAX_DIRECTIVE_PERCENTAGE");
                        strQry.AppendLine(",LEAVE_SHIFT_NO");
                        strQry.AppendLine(",EMPLOYEE_NUMBER_CHEQUES");
                        strQry.AppendLine(",NUMBER_MEDICAL_AID_DEPENDENTS");
                        strQry.AppendLine(",OCCUPATION_NO");
                        strQry.AppendLine(",BANK_ACCOUNT_TYPE_NO");
                        strQry.AppendLine(",BRANCH_CODE");
                        strQry.AppendLine(",ACCOUNT_NO");
                        strQry.AppendLine(",MARITAL_STATUS_NO");
                        strQry.AppendLine(",NATURE_PERSON_NO");
                        strQry.AppendLine(",RACE_NO");
                        strQry.AppendLine(",OCCUPATION_LEVEL_NO");
                        strQry.AppendLine(",OCCUPATION_CATEGORY_NO");
                        strQry.AppendLine(",OCCUPATION_FUNCTION_NO");
                        strQry.AppendLine(",SKILLS_EQUITY_PROVINCE_NO");
                        strQry.AppendLine(",USER_NO_ACTIVATE_RECORD");
                        strQry.AppendLine(",BANK_NO)");
                        
                        strQry.AppendLine(" VALUES ");

                        strQry.AppendLine("(" + parDataSet.Tables["Employee"].Rows[intRow]["COMPANY_NO"].ToString());
                        strQry.AppendLine("," + parDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString());

                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_CODE"].ToString()));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NAME"].ToString()));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_SURNAME"].ToString()));

                        if (parDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_LAST_RUNDATE"] != System.DBNull.Value)
                        {
                            strQry.AppendLine(",'" + Convert.ToDateTime(parDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_LAST_RUNDATE"]).ToString("yyyy-MM-dd") + "'");
                        }
                        else
                        {
                            strQry.AppendLine(",'2015-02-06'");
                        }

                        strQry.AppendLine(",'2015-02-06'");

                        if (parDataSet.Tables["Employee"].Rows[intRow]["DEPARTMENT_NO"] != System.DBNull.Value)
                        {

                            strQry.AppendLine("," + parDataSet.Tables["Employee"].Rows[intRow]["DEPARTMENT_NO"].ToString());
                        }
                        else
                        {
                            strQry.AppendLine(",NULL");
                        }

                        strQry.AppendLine(",'Y'");
                        strQry.AppendLine(",'Y'");
                        strQry.AppendLine(",'Y'");
                        strQry.AppendLine(",0");
                        strQry.AppendLine(",0");
                        strQry.AppendLine(",0");
                        strQry.AppendLine(",0");
                        strQry.AppendLine(",0");
                        strQry.AppendLine(",0");
                        strQry.AppendLine(",0");
                        strQry.AppendLine(",0");
                        strQry.AppendLine(",0");
                        strQry.AppendLine(",0");
                        strQry.AppendLine(",0");
                        strQry.AppendLine(",0");
                        strQry.AppendLine(",0");
                        strQry.AppendLine(",0");
                        strQry.AppendLine(",0");
                        strQry.AppendLine(",0");
                        strQry.AppendLine(",0");
                        strQry.AppendLine(",0");
                        strQry.AppendLine(",0)");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Convert.ToInt64(parDataSet.Tables["Employee"].Rows[intRow]["COMPANY_NO"]));
                    }
                }

                for (int intRow = 0; intRow < parDataSet.Tables["EmployeePayCategory"].Rows.Count; intRow++)
                {
                    if (DataSet.Tables["EmployeePayCategory"] != null)
                    {
                        DataSet.Tables.Remove("EmployeePayCategory");
                    }

                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" EMPLOYEE_NO");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + parDataSet.Tables["EmployeePayCategory"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parDataSet.Tables["EmployeePayCategory"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EmployeePayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeePayCategory", parInt64CompanyNo);

                    if (DataSet.Tables["EmployeePayCategory"].Rows.Count == 0)
                    {
                        //Insert
                        strQry.Clear();

                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY ");
                        strQry.AppendLine("(COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",PAY_CATEGORY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",TIE_BREAKER");
                        strQry.AppendLine(",DEFAULT_IND");
                        strQry.AppendLine(",HOURLY_RATE");
                        strQry.AppendLine(",LEAVE_DAY_RATE_DECIMAL)");
                
                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" C.COMPANY_NO");
                        strQry.AppendLine("," + parDataSet.Tables["EmployeePayCategory"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                        strQry.AppendLine("," + parDataSet.Tables["EmployeePayCategory"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());

                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EmployeePayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                        strQry.AppendLine(",ISNULL(EPC.TIE_BREAKER,0) + 1");

                        strQry.AppendLine(",'Y'");
                        strQry.AppendLine(",0");
                        strQry.AppendLine(",0");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

                        strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                        strQry.AppendLine(" ON C.COMPANY_NO = EPC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + parDataSet.Tables["EmployeePayCategory"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + parDataSet.Tables["EmployeePayCategory"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EmployeePayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                        strQry.AppendLine(" WHERE C.COMPANY_NO = " + parInt64CompanyNo);

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Convert.ToInt64(parDataSet.Tables["EmployeePayCategory"].Rows[intRow]["COMPANY_NO"]));
                    }
                }
            }

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }
    }
}
