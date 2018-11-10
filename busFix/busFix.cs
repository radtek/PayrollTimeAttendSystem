using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busFix
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busFix()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo)
        {
            string strQry = "";
            DataSet DataSet = new DataSet();

            strQry = "";
            strQry += " SELECT ";
            strQry += " COMPANY_NO ";
            strQry += ",EMPLOYEE_NO ";
            strQry += ",PAY_CATEGORY_TYPE ";
            strQry += ",EMPLOYEE_NAME ";
            strQry += ",EMPLOYEE_SURNAME ";
            strQry += ",EMPLOYEE_CODE ";

            strQry += " FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE";

            strQry += " WHERE DATETIME_DELETE_RECORD IS NULL ";

            strQry += " ORDER BY ";
            strQry += " COMPANY_NO ";
            strQry += ",EMPLOYEE_SURNAME ";

            clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "Employee", parint64CompanyNo);

            strQry = "";
            strQry += " SELECT ";
            strQry += " COMPANY_NO ";
            strQry += ",EMPLOYEE_NO ";
            strQry += ",PAY_CATEGORY_NO ";
            strQry += ",PAY_CATEGORY_TYPE ";
         
            strQry += " FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY";

            strQry += " WHERE DATETIME_DELETE_RECORD IS NULL ";
            strQry += " AND DEFAULT_IND = 'Y' ";

            clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "EmployeePayCategory", parint64CompanyNo);

            strQry = "";
            strQry += " SELECT ";
            strQry += " COMPANY_NO ";
            strQry += ",PAY_CATEGORY_NO ";
            strQry += ",PAY_CATEGORY_TYPE ";
            strQry += ",PAY_CATEGORY_DESC ";
       
            strQry += " FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY";

            strQry += " WHERE DATETIME_DELETE_RECORD IS NULL ";
            strQry += " AND PAY_CATEGORY_NO > 0 ";

            strQry += " ORDER BY ";
            strQry += " COMPANY_NO ";
            strQry += ",PAY_CATEGORY_DESC ";

            clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "PayCategory", parint64CompanyNo);

            strQry = "";
            strQry += " SELECT ";
            strQry += " COMPANY_NO ";
            strQry += ",DEPARTMENT_NO ";
            strQry += ",DEPARTMENT_DESC ";
            strQry += " FROM InteractPayroll_#CompanyNo#.dbo.DEPARTMENT";

            strQry += " WHERE DATETIME_DELETE_RECORD IS NULL ";

            strQry += " ORDER BY ";
            strQry += " COMPANY_NO ";
            strQry += ",DEPARTMENT_DESC ";

            clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "Department", parint64CompanyNo);

            strQry = "";
            strQry += " SELECT ";
            strQry += " COMPANY_NO ";
            strQry += ",OCCUPATION_NO ";
            strQry += ",OCCUPATION_DESC ";
                    
            strQry += " FROM InteractPayroll_#CompanyNo#.dbo.OCCUPATION";

            strQry += " WHERE DATETIME_DELETE_RECORD IS NULL ";

            strQry += " ORDER BY ";
            strQry += " COMPANY_NO ";
            strQry += ",OCCUPATION_DESC ";

            clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "Occupation", parint64CompanyNo);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Update_Records(Int64 parint64CompanyNo, string parstrEmployeeWageNoIn, string parstrEmployeeSalaryNoIn, string parstrPayCategoryWageNoIn, string parstrPayCategorySalaryNoIn, string parstrDepartmentNoIn, string parstrOccupationNoIn, string parstrCostCentrePayCategoryNo, string parstrCostCentrePayCategoryType, string parstrCostCentreEmployeeNoIN, Int64 parint64CurrentUserNo)
        {
            DataSet DataSet = new System.Data.DataSet();
            string strQry = "";

            if (parstrEmployeeWageNoIn != ""
                | parstrEmployeeSalaryNoIn != "")
            {
                strQry = "";
                strQry += " SELECT ";
                strQry += " C1.TABLE_NAME";
                strQry += ",C1.COLUMN_NAME AS COLUMN1_NAME";
                strQry += ",C2.COLUMN_NAME AS COLUMN2_NAME";

                strQry += " FROM InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.TABLES T";

                strQry += " INNER JOIN InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.COLUMNS C1";
                strQry += " ON T.TABLE_NAME = C1.TABLE_NAME";
                strQry += " AND C1.COLUMN_NAME = 'EMPLOYEE_NO'";

                strQry += " LEFT JOIN InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.COLUMNS C2";
                strQry += " ON T.TABLE_NAME = C2.TABLE_NAME";
                strQry += " AND C2.COLUMN_NAME = 'PAY_CATEGORY_TYPE'";

                strQry += " WHERE T.TABLE_TYPE = 'BASE TABLE'";

                clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "ColumnName", parint64CompanyNo);

                for (int intRow1 = 0; intRow1 < DataSet.Tables["ColumnName"].Rows.Count; intRow1++)
                {
                    strQry = "";
                    strQry += " DELETE ";

                    strQry += " FROM InteractPayroll_#CompanyNo#.dbo." + DataSet.Tables["ColumnName"].Rows[intRow1]["TABLE_NAME"].ToString();

                    strQry += " WHERE COMPANY_NO = " + parint64CompanyNo;

                    if (DataSet.Tables["ColumnName"].Rows[intRow1]["COLUMN2_NAME"] != System.DBNull.Value)
                    {
                        if (parstrEmployeeWageNoIn != ""
                            & parstrEmployeeSalaryNoIn != "")
                        {
                            strQry += " AND ((EMPLOYEE_NO IN " + parstrEmployeeWageNoIn;
                            strQry += " AND PAY_CATEGORY_TYPE = 'W') ";

                            strQry += " OR (EMPLOYEE_NO IN " + parstrEmployeeSalaryNoIn;
                            strQry += " AND PAY_CATEGORY_TYPE = 'S')) ";
                        }
                        else
                        {
                            if (parstrEmployeeWageNoIn != "")
                            {
                                strQry += " AND (EMPLOYEE_NO IN " + parstrEmployeeWageNoIn;
                                strQry += " AND PAY_CATEGORY_TYPE = 'W') ";
                            }
                            else
                            {
                                strQry += " AND (EMPLOYEE_NO IN " + parstrEmployeeSalaryNoIn;
                                strQry += " AND PAY_CATEGORY_TYPE = 'S') ";
                            }
                        }
                    }
                    else
                    {
                        if (parstrEmployeeWageNoIn != ""
                            & parstrEmployeeSalaryNoIn != "")
                        {
                            strQry += " AND (EMPLOYEE_NO IN " + parstrEmployeeWageNoIn;
                            strQry += " OR EMPLOYEE_NO IN " + parstrEmployeeSalaryNoIn + ")";
                        }
                        else
                        {
                            if (parstrEmployeeWageNoIn != "")
                            {
                                strQry += " AND EMPLOYEE_NO IN " + parstrEmployeeWageNoIn;
                            }
                            else
                            {
                                strQry += " AND EMPLOYEE_NO IN " + parstrEmployeeSalaryNoIn;
                            }
                        }
                    }

                    clsDBConnectionObjects.Execute_SQLCommand(strQry, parint64CompanyNo);
                }

                strQry = "";
                strQry += " DELETE ";

                strQry += " FROM InteractPayroll.dbo.USER_EMPLOYEE";
                strQry += " WHERE COMPANY_NO = " + parint64CompanyNo;

                if (parstrEmployeeWageNoIn != ""
                & parstrEmployeeSalaryNoIn != "")
                {
                    strQry += " AND ((EMPLOYEE_NO IN " + parstrEmployeeWageNoIn;
                    strQry += " AND PAY_CATEGORY_TYPE = 'W') ";

                    strQry += " OR (EMPLOYEE_NO IN " + parstrEmployeeSalaryNoIn;
                    strQry += " AND PAY_CATEGORY_TYPE = 'S'))";

                }
                else
                {
                    if (parstrEmployeeWageNoIn != "")
                    {
                        strQry += " AND (EMPLOYEE_NO IN " + parstrEmployeeWageNoIn;
                        strQry += " AND PAY_CATEGORY_TYPE = 'W') ";
                    }
                    else
                    {
                        strQry += " AND (EMPLOYEE_NO IN " + parstrEmployeeSalaryNoIn;
                        strQry += " AND PAY_CATEGORY_TYPE = 'S') ";
                    }
                }

                clsDBConnectionObjects.Execute_SQLCommand(strQry, parint64CompanyNo);
            }

            if (parstrPayCategoryWageNoIn != ""
            | parstrPayCategorySalaryNoIn != "")
            {
                strQry = "";
                strQry += " SELECT DISTINCT";
                strQry += " COMPANY_NO";
                strQry += ",EMPLOYEE_NO";
                strQry += ",PAY_CATEGORY_TYPE";

                strQry += " FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY ";

                strQry += " WHERE COMPANY_NO = " + parint64CompanyNo;

                if (parstrPayCategoryWageNoIn != ""
                & parstrPayCategorySalaryNoIn != "")
                {
                    strQry += " AND ((PAY_CATEGORY_NO IN " + parstrPayCategoryWageNoIn;
                    strQry += " AND PAY_CATEGORY_TYPE = 'W')";

                    strQry += " OR (PAY_CATEGORY_NO IN " + parstrPayCategorySalaryNoIn;
                    strQry += " AND PAY_CATEGORY_TYPE = 'S'))";
                }
                else
                {
                    if (parstrPayCategoryWageNoIn != "")
                    {
                        strQry += " AND PAY_CATEGORY_NO IN " + parstrPayCategoryWageNoIn;
                        strQry += " AND PAY_CATEGORY_TYPE = 'W'";
                    }
                    else
                    {
                        strQry += " AND PAY_CATEGORY_NO IN " + parstrPayCategorySalaryNoIn;
                        strQry += " AND PAY_CATEGORY_TYPE = 'S'";
                    }
                }

                clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "EmployeePayCategory", parint64CompanyNo);

                string strEmployeeWageNoIn = "";
                string strEmployeeSalaryNoIn = "";

                for (int intRow1 = 0; intRow1 < DataSet.Tables["EmployeePayCategory"].Rows.Count; intRow1++)
                {

                    if (DataSet.Tables["EmployeePayCategory"].Rows[intRow1]["PAY_CATEGORY_TYPE"].ToString() == "W")
                    {
                        if (strEmployeeWageNoIn == "")
                        {
                            strEmployeeWageNoIn = "(" + DataSet.Tables["EmployeePayCategory"].Rows[intRow1]["EMPLOYEE_NO"].ToString();
                        }
                        else
                        {
                            strEmployeeWageNoIn += "," + DataSet.Tables["EmployeePayCategory"].Rows[intRow1]["EMPLOYEE_NO"].ToString();
                        }
                    }
                    else
                    {
                        if (strEmployeeSalaryNoIn == "")
                        {
                            strEmployeeSalaryNoIn = "(" + DataSet.Tables["EmployeePayCategory"].Rows[intRow1]["EMPLOYEE_NO"].ToString();
                        }
                        else
                        {
                            strEmployeeSalaryNoIn += "," + DataSet.Tables["EmployeePayCategory"].Rows[intRow1]["EMPLOYEE_NO"].ToString();
                        }
                    }
                }

                if (strEmployeeWageNoIn != "")
                {
                    strEmployeeWageNoIn += ")";
                }

                if (strEmployeeSalaryNoIn != "")
                {
                    strEmployeeSalaryNoIn += ")";
                }

                if (strEmployeeWageNoIn != ""
                | strEmployeeSalaryNoIn != "")
                {
                    if (DataSet.Tables["ColumnName"] != null)
                    {
                        DataSet.Tables.Remove("ColumnName");
                    }

                    strQry = "";
                    strQry += " SELECT ";
                    strQry += " C1.TABLE_NAME";
                    strQry += ",C1.COLUMN_NAME AS COLUMN1_NAME";
                    strQry += ",C2.COLUMN_NAME AS COLUMN2_NAME";

                    strQry += " FROM InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.TABLES T";

                    strQry += " INNER JOIN InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.COLUMNS C1";
                    strQry += " ON T.TABLE_NAME = C1.TABLE_NAME";
                    strQry += " AND C1.COLUMN_NAME = 'EMPLOYEE_NO'";

                    strQry += " LEFT JOIN InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.COLUMNS C2";
                    strQry += " ON T.TABLE_NAME = C2.TABLE_NAME";
                    strQry += " AND C2.COLUMN_NAME = 'PAY_CATEGORY_TYPE'";

                    strQry += " WHERE T.TABLE_TYPE = 'BASE TABLE'";

                    clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "ColumnName", parint64CompanyNo);

                    for (int intRow1 = 0; intRow1 < DataSet.Tables["ColumnName"].Rows.Count; intRow1++)
                    {
                        strQry = "";
                        strQry += " DELETE ";

                        strQry += " FROM InteractPayroll_#CompanyNo#.dbo." + DataSet.Tables["ColumnName"].Rows[intRow1]["TABLE_NAME"].ToString();

                        strQry += " WHERE COMPANY_NO = " + parint64CompanyNo;

                        if (DataSet.Tables["ColumnName"].Rows[intRow1]["COLUMN2_NAME"] != System.DBNull.Value)
                        {
                            if (strEmployeeWageNoIn != ""
                                & strEmployeeSalaryNoIn != "")
                            {
                                strQry += " AND ((EMPLOYEE_NO IN " + strEmployeeWageNoIn;
                                strQry += " AND PAY_CATEGORY_TYPE = 'W') ";

                                strQry += " OR (EMPLOYEE_NO IN " + strEmployeeSalaryNoIn;
                                strQry += " AND PAY_CATEGORY_TYPE = 'S')) ";

                            }
                            else
                            {
                                if (strEmployeeWageNoIn != "")
                                {
                                    strQry += " AND (EMPLOYEE_NO IN " + strEmployeeWageNoIn;
                                    strQry += " AND PAY_CATEGORY_TYPE = 'W') ";
                                }
                                else
                                {
                                    strQry += " AND (EMPLOYEE_NO IN " + strEmployeeSalaryNoIn;
                                    strQry += " AND PAY_CATEGORY_TYPE = 'S') ";
                                }
                            }
                        }
                        else
                        {
                            if (strEmployeeWageNoIn != ""
                                & strEmployeeSalaryNoIn != "")
                            {
                                strQry += " AND (EMPLOYEE_NO IN " + strEmployeeWageNoIn;
                                strQry += " OR EMPLOYEE_NO IN " + strEmployeeSalaryNoIn + ")";
                            }
                            else
                            {
                                if (strEmployeeWageNoIn != "")
                                {
                                    strQry += " AND EMPLOYEE_NO IN " + strEmployeeWageNoIn;
                                }
                                else
                                {
                                    strQry += " AND EMPLOYEE_NO IN " + strEmployeeSalaryNoIn;
                                }
                            }
                        }

                        clsDBConnectionObjects.Execute_SQLCommand(strQry, parint64CompanyNo);
                    }

                    strQry = "";
                    strQry += " DELETE ";

                    strQry += " FROM InteractPayroll.dbo.USER_EMPLOYEE";
                    strQry += " WHERE COMPANY_NO = " + parint64CompanyNo;

                    if (strEmployeeWageNoIn != ""
                    & strEmployeeSalaryNoIn != "")
                    {
                        strQry += " AND ((EMPLOYEE_NO IN " + strEmployeeWageNoIn;
                        strQry += " AND PAY_CATEGORY_TYPE = 'W') ";

                        strQry += " OR (EMPLOYEE_NO IN " + strEmployeeSalaryNoIn;
                        strQry += " AND PAY_CATEGORY_TYPE = 'S'))";

                    }
                    else
                    {
                        if (strEmployeeWageNoIn != "")
                        {
                            strQry += " AND (EMPLOYEE_NO IN " + strEmployeeWageNoIn;
                            strQry += " AND PAY_CATEGORY_TYPE = 'W') ";
                        }
                        else
                        {
                            strQry += " AND (EMPLOYEE_NO IN " + strEmployeeSalaryNoIn;
                            strQry += " AND PAY_CATEGORY_TYPE = 'S') ";
                        }
                    }

                    clsDBConnectionObjects.Execute_SQLCommand(strQry, parint64CompanyNo);

                    if (DataSet.Tables["ColumnName"] != null)
                    {
                        DataSet.Tables.Remove("ColumnName");
                    }

                    strQry = "";
                    strQry += " SELECT ";
                    strQry += " C1.TABLE_NAME";
                    strQry += ",C1.COLUMN_NAME AS COLUMN1_NAME";
                    strQry += ",C2.COLUMN_NAME AS COLUMN2_NAME";

                    strQry += " FROM InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.TABLES T";

                    strQry += " INNER JOIN InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.COLUMNS C1";
                    strQry += " ON T.TABLE_NAME = C1.TABLE_NAME";
                    strQry += " AND C1.COLUMN_NAME = 'PAY_CATEGORY_NO'";

                    strQry += " LEFT JOIN InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.COLUMNS C2";
                    strQry += " ON T.TABLE_NAME = C2.TABLE_NAME";
                    strQry += " AND C2.COLUMN_NAME = 'PAY_CATEGORY_TYPE'";

                    strQry += " WHERE T.TABLE_TYPE = 'BASE TABLE'";

                    clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "ColumnName", parint64CompanyNo);

                    for (int intRow1 = 0; intRow1 < DataSet.Tables["ColumnName"].Rows.Count; intRow1++)
                    {
                        if (DataSet.Tables["ColumnName"].Rows[intRow1]["COLUMN2_NAME"] == System.DBNull.Value)
                        {
                            string stop = "";
                        }

                        if (parstrPayCategoryWageNoIn != "")
                        {
                            strQry = "";
                            strQry += " DELETE ";

                            strQry += " FROM InteractPayroll_#CompanyNo#.dbo." + DataSet.Tables["ColumnName"].Rows[intRow1]["TABLE_NAME"].ToString();

                            strQry += " WHERE COMPANY_NO = " + parint64CompanyNo;
                            strQry += " AND PAY_CATEGORY_NO IN " + parstrPayCategoryWageNoIn;

                            if (DataSet.Tables["ColumnName"].Rows[intRow1]["COLUMN2_NAME"] != System.DBNull.Value)
                            {
                                strQry += " AND PAY_CATEGORY_TYPE = 'W'";
                            }

                            clsDBConnectionObjects.Execute_SQLCommand(strQry, parint64CompanyNo);
                        }

                        if (parstrPayCategorySalaryNoIn != "")
                        {
                            strQry = "";
                            strQry += " DELETE ";

                            strQry += " FROM InteractPayroll_#CompanyNo#.dbo." + DataSet.Tables["ColumnName"].Rows[intRow1]["TABLE_NAME"].ToString();

                            strQry += " WHERE COMPANY_NO = " + parint64CompanyNo;
                            strQry += " AND PAY_CATEGORY_NO IN " + parstrPayCategorySalaryNoIn;

                            if (DataSet.Tables["ColumnName"].Rows[intRow1]["COLUMN2_NAME"] != System.DBNull.Value)
                            {
                                strQry += " AND PAY_CATEGORY_TYPE = 'S'";
                            }

                            clsDBConnectionObjects.Execute_SQLCommand(strQry, parint64CompanyNo);
                        }
                    }

                    if (parstrPayCategoryWageNoIn != "")
                    {
                        strQry = "";
                        strQry += " DELETE ";

                        strQry += " FROM InteractPayroll.dbo.USER_PAY_CATEGORY";

                        strQry += " WHERE COMPANY_NO = " + parint64CompanyNo;
                        strQry += " AND PAY_CATEGORY_NO IN " + parstrPayCategoryWageNoIn;
                        strQry += " AND PAY_CATEGORY_TYPE = 'W'";

                        clsDBConnectionObjects.Execute_SQLCommand(strQry, parint64CompanyNo);
                    }

                    if (parstrPayCategorySalaryNoIn != "")
                    {
                        strQry = "";
                        strQry += " DELETE ";

                        strQry += " FROM InteractPayroll.dbo.USER_PAY_CATEGORY";

                        strQry += " WHERE COMPANY_NO = " + parint64CompanyNo;
                        strQry += " AND PAY_CATEGORY_NO IN " + parstrPayCategorySalaryNoIn;
                        strQry += " AND PAY_CATEGORY_TYPE = 'S'";

                        clsDBConnectionObjects.Execute_SQLCommand(strQry, parint64CompanyNo);
                    }
                }
            }

            if (parstrDepartmentNoIn != "")
            {
                if (DataSet.Tables["ColumnName"] != null)
                {
                    DataSet.Tables.Remove("ColumnName");
                }

                strQry = "";
                strQry += " SELECT ";
                strQry += " C1.TABLE_NAME";
                strQry += ",C1.COLUMN_NAME AS COLUMN1_NAME";

                strQry += " FROM InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.TABLES T";

                strQry += " INNER JOIN InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.COLUMNS C1";
                strQry += " ON T.TABLE_NAME = C1.TABLE_NAME";
                strQry += " AND C1.COLUMN_NAME = 'DEPARTMENT_NO'";

                strQry += " WHERE T.TABLE_TYPE = 'BASE TABLE'";

                clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "ColumnName", parint64CompanyNo);

                for (int intRow1 = 0; intRow1 < DataSet.Tables["ColumnName"].Rows.Count; intRow1++)
                {
                    if (DataSet.Tables["ColumnName"].Rows[intRow1]["TABLE_NAME"].ToString() == "DEPARTMENT")
                    {
                        strQry = "";
                        strQry += " DELETE ";

                        strQry += " FROM InteractPayroll_#CompanyNo#.dbo." + DataSet.Tables["ColumnName"].Rows[intRow1]["TABLE_NAME"].ToString();

                        strQry += " WHERE COMPANY_NO = " + parint64CompanyNo;
                        strQry += " AND DEPARTMENT_NO IN " + parstrDepartmentNoIn;
                    }
                    else
                    {
                        strQry = "";
                        strQry += " UPDATE InteractPayroll_#CompanyNo#.dbo." + DataSet.Tables["ColumnName"].Rows[intRow1]["TABLE_NAME"].ToString();
                        strQry += " SET DEPARTMENT_NO = 0 ";
                        strQry += " WHERE COMPANY_NO = " + parint64CompanyNo;
                        strQry += " AND DEPARTMENT_NO IN " + parstrDepartmentNoIn;
                    }

                    clsDBConnectionObjects.Execute_SQLCommand(strQry, parint64CompanyNo);
                }

                strQry = "";
                strQry += " DELETE ";

                strQry += " FROM InteractPayroll.dbo.USER_DEPARTMENT";

                strQry += " WHERE COMPANY_NO = " + parint64CompanyNo;
                strQry += " AND DEPARTMENT_NO IN " + parstrDepartmentNoIn;

                clsDBConnectionObjects.Execute_SQLCommand(strQry, parint64CompanyNo);
            }

            if (parstrOccupationNoIn != "")
            {
                if (DataSet.Tables["ColumnName"] != null)
                {
                    DataSet.Tables.Remove("ColumnName");
                }

                strQry = "";
                strQry += " SELECT ";
                strQry += " C1.TABLE_NAME";
                strQry += ",C1.COLUMN_NAME AS COLUMN1_NAME";

                strQry += " FROM InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.TABLES T";

                strQry += " INNER JOIN InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.COLUMNS C1";
                strQry += " ON T.TABLE_NAME = C1.TABLE_NAME";
                strQry += " AND C1.COLUMN_NAME = 'OCCUPATION_NO'";

                strQry += " WHERE T.TABLE_TYPE = 'BASE TABLE'";

                clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "ColumnName", parint64CompanyNo);

                for (int intRow1 = 0; intRow1 < DataSet.Tables["ColumnName"].Rows.Count; intRow1++)
                {
                    if (DataSet.Tables["ColumnName"].Rows[intRow1]["TABLE_NAME"].ToString() == "OCCUPATION")
                    {
                        strQry = "";
                        strQry += " DELETE ";

                        strQry += " FROM InteractPayroll_#CompanyNo#.dbo." + DataSet.Tables["ColumnName"].Rows[intRow1]["TABLE_NAME"].ToString();

                        strQry += " WHERE COMPANY_NO = " + parint64CompanyNo;
                        strQry += " AND OCCUPATION_NO IN " + parstrOccupationNoIn;
                    }
                    else
                    {
                        strQry = "";
                        strQry += " UPDATE InteractPayroll_#CompanyNo#.dbo." + DataSet.Tables["ColumnName"].Rows[intRow1]["TABLE_NAME"].ToString();
                        strQry += " SET OCCUPATION_NO = 0 ";
                        strQry += " WHERE COMPANY_NO = " + parint64CompanyNo;
                        strQry += " AND OCCUPATION_NO IN " + parstrOccupationNoIn;
                    }

                    clsDBConnectionObjects.Execute_SQLCommand(strQry, parint64CompanyNo);
                }
            }

            if (parstrCostCentreEmployeeNoIN != "")
            {
                strQry = "";
                strQry += " DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY";
                strQry += " WHERE COMPANY_NO = " + parint64CompanyNo;
                strQry += " AND PAY_CATEGORY_NO = " + parstrCostCentrePayCategoryNo;
                strQry += " AND PAY_CATEGORY_TYPE = '" + parstrCostCentrePayCategoryType + "'";

                clsDBConnectionObjects.Execute_SQLCommand(strQry, parint64CompanyNo);

                strQry = "";
                strQry += " INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY";
                strQry += "(COMPANY_NO";
                strQry += ",EMPLOYEE_NO";
                strQry += ",PAY_CATEGORY_NO";
                strQry += ",PAY_CATEGORY_TYPE";
                strQry += ",TIE_BREAKER";
                strQry += ",HOURLY_RATE";
                strQry += ",DEFAULT_IND";
                strQry += ",USER_NO_NEW_RECORD";
                strQry += ",DATETIME_NEW_RECORD";
                strQry += ",LEAVE_DAY_RATE_DECIMAL)";

                strQry += " SELECT ";
                strQry += " COMPANY_NO";
                strQry += ",EMPLOYEE_NO";
                strQry += "," + parstrCostCentrePayCategoryNo;
                strQry += ",PAY_CATEGORY_TYPE";
                strQry += ",1";
                strQry += ",0";
                strQry += ",'Y'";
                strQry += "," + parint64CurrentUserNo;
                strQry += ",GETDATE()";
                strQry += ",0";

                strQry += " FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE";

                strQry += " WHERE COMPANY_NO = " + parint64CompanyNo;
                strQry += " AND EMPLOYEE_NO IN " + parstrCostCentreEmployeeNoIN;
                strQry += " AND PAY_CATEGORY_TYPE = '" + parstrCostCentrePayCategoryType + "'";
                strQry += " AND DATETIME_DELETE_RECORD IS NULL ";
        
                clsDBConnectionObjects.Execute_SQLCommand(strQry, parint64CompanyNo);
            }

            strQry = "";
            strQry += " UPDATE InteractPayroll.dbo.COMPANY_LINK";
            strQry += " SET BACKUP_DB_IND = 1";
            strQry += " WHERE COMPANY_NO = " + parint64CompanyNo;

            clsDBConnectionObjects.Execute_SQLCommand(strQry, -1);
            
            byte[] byteArray = Get_Form_Records(parint64CompanyNo);

            return byteArray;
        }
    }
}
