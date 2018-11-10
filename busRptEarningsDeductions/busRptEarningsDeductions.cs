using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busRptEarningsDeductions
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busRptEarningsDeductions()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo, Int64 parint64CurrentUserNo, string parstrCurrentUserAccess)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            //Get Empty DataTable for DataView
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",EMPLOYEE_CODE");
            strQry.AppendLine(",EMPLOYEE_NAME");
            strQry.AppendLine(",EMPLOYEE_SURNAME");
            strQry.AppendLine(",EMPLOYEE_NO");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = -1");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parint64CompanyNo);

            //Get Empty DataTable for DataView
            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" EPCH.COMPANY_NO");
            strQry.AppendLine(",EPCH.PAY_PERIOD_DATE");
            strQry.AppendLine(",EPCH.EMPLOYEE_NO");
            strQry.AppendLine(",EPCH.PAY_CATEGORY_NO");
            strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPCH.RUN_TYPE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");
            
            strQry.AppendLine(" WHERE EPCH.COMPANY_NO = -1");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeePayCategory", parint64CompanyNo);
           
            strQry.Clear();

            strQry.AppendLine(" SELECT  ");
            strQry.AppendLine(" OVERTIME1_RATE");
            strQry.AppendLine(",OVERTIME2_RATE");
            strQry.AppendLine(",OVERTIME3_RATE");
            strQry.AppendLine(",SALARY_OVERTIME1_RATE");
            strQry.AppendLine(",SALARY_OVERTIME2_RATE");
            strQry.AppendLine(",SALARY_OVERTIME3_RATE");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" EN.COMPANY_NO ");
            strQry.AppendLine(",EN.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",'Income' AS EARNING_DESC");
            strQry.AppendLine(",EN.EARNING_NO");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING EN");
            
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
            strQry.AppendLine(" ON EN.COMPANY_NO = EEH.COMPANY_NO");
            strQry.AppendLine(" AND EN.EARNING_NO = EEH.EARNING_NO");
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE");

            //2013-08-29
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND EEH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EEH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }
      
            strQry.AppendLine(" WHERE EN.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = 'S'");
            strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" AND EN.EARNING_NO = 1");

            strQry.AppendLine(" UNION");

            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" EN.COMPANY_NO ");
            strQry.AppendLine(",EN.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",'Normal Time' AS EARNING_DESC");
            strQry.AppendLine(",EN.EARNING_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING EN");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
            strQry.AppendLine(" ON EN.COMPANY_NO = EEH.COMPANY_NO");
            strQry.AppendLine(" AND EN.EARNING_NO = EEH.EARNING_NO");
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE");

            //2013-08-29
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND EEH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EEH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE EN.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" AND EN.EARNING_NO = 2");
         
            strQry.AppendLine(" UNION");
   
            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" EN.COMPANY_NO ");
            strQry.AppendLine(",EN.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",'Overtime (" + Convert.ToDouble(DataSet.Tables["Temp"].Rows[0]["OVERTIME1_RATE"]).ToString("#0.00") + "%)' AS EARNING_DESC");
            strQry.AppendLine(",EN.EARNING_NO");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING EN");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
            strQry.AppendLine(" ON EN.COMPANY_NO = EEH.COMPANY_NO");
            strQry.AppendLine(" AND EN.EARNING_NO = EEH.EARNING_NO");
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE");

            //2013-08-29
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND EEH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EEH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE EN.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" AND EN.EARNING_NO = 3");

            if (Convert.ToDouble(DataSet.Tables["Temp"].Rows[0]["OVERTIME2_RATE"]) > 0)
            {
                strQry.AppendLine(" UNION ");
                
                strQry.AppendLine(" SELECT DISTINCT ");
                strQry.AppendLine(" EN.COMPANY_NO ");
                strQry.AppendLine(",EN.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",'Overtime (" + Convert.ToDouble(DataSet.Tables["Temp"].Rows[0]["OVERTIME2_RATE"]).ToString("#0.00") + "%)' AS EARNING_DESC");
                strQry.AppendLine(",EN.EARNING_NO");


                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING EN");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
                strQry.AppendLine(" ON EN.COMPANY_NO = EEH.COMPANY_NO");
                strQry.AppendLine(" AND EN.EARNING_NO = EEH.EARNING_NO");
                strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE");

                //2013-08-29
                if (parstrCurrentUserAccess == "U")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                    strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                    strQry.AppendLine(" AND EEH.COMPANY_NO = UEPCT.COMPANY_NO");
                    strQry.AppendLine(" AND EEH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EEH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
                }

                strQry.AppendLine(" WHERE EN.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL ");
                strQry.AppendLine(" AND EN.EARNING_NO = 4");
            }

            if (Convert.ToDouble(DataSet.Tables["Temp"].Rows[0]["OVERTIME3_RATE"]) > 0)
            {
                strQry.AppendLine(" UNION ");
                strQry.AppendLine(" SELECT DISTINCT ");
                strQry.AppendLine(" EN.COMPANY_NO ");
                strQry.AppendLine(",EN.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",'Overtime (" + Convert.ToDouble(DataSet.Tables["Temp"].Rows[0]["OVERTIME3_RATE"]).ToString("#0.00") + "%)' AS EARNING_DESC");
                strQry.AppendLine(",EN.EARNING_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING EN");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
                strQry.AppendLine(" ON EN.COMPANY_NO = EEH.COMPANY_NO");
                strQry.AppendLine(" AND EN.EARNING_NO = EEH.EARNING_NO");
                strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE");

                //2013-08-29
                if (parstrCurrentUserAccess == "U")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                    strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                    strQry.AppendLine(" AND EEH.COMPANY_NO = UEPCT.COMPANY_NO");
                    strQry.AppendLine(" AND EEH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EEH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
                }

                strQry.AppendLine(" WHERE EN.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL ");
                strQry.AppendLine(" AND EN.EARNING_NO = 5");
            }

            if (Convert.ToDouble(DataSet.Tables["Temp"].Rows[0]["SALARY_OVERTIME1_RATE"]) > 0)
            {
                strQry.AppendLine(" UNION ");

                strQry.AppendLine(" SELECT DISTINCT ");
                strQry.AppendLine(" EN.COMPANY_NO ");
                strQry.AppendLine(",EN.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",'Overtime (" + Convert.ToDouble(DataSet.Tables["Temp"].Rows[0]["SALARY_OVERTIME1_RATE"]).ToString("#0.00") + "%)' AS EARNING_DESC");
                strQry.AppendLine(",EN.EARNING_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING EN");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
                strQry.AppendLine(" ON EN.COMPANY_NO = EEH.COMPANY_NO");
                strQry.AppendLine(" AND EN.EARNING_NO = EEH.EARNING_NO");
                strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE");

                //2013-08-29
                if (parstrCurrentUserAccess == "U")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                    strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                    strQry.AppendLine(" AND EEH.COMPANY_NO = UEPCT.COMPANY_NO");
                    strQry.AppendLine(" AND EEH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EEH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
                }

                strQry.AppendLine(" WHERE EN.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL ");
                strQry.AppendLine(" AND EN.EARNING_NO = 3");
            }

            if (Convert.ToDouble(DataSet.Tables["Temp"].Rows[0]["SALARY_OVERTIME2_RATE"]) > 0)
            {
                strQry.AppendLine(" UNION ");

                strQry.AppendLine(" SELECT DISTINCT ");
                strQry.AppendLine(" EN.COMPANY_NO ");
                strQry.AppendLine(",EN.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",'Overtime (" + Convert.ToDouble(DataSet.Tables["Temp"].Rows[0]["SALARY_OVERTIME2_RATE"]).ToString("#0.00") + "%)' AS EARNING_DESC");
                strQry.AppendLine(",EN.EARNING_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING EN");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
                strQry.AppendLine(" ON EN.COMPANY_NO = EEH.COMPANY_NO");
                strQry.AppendLine(" AND EN.EARNING_NO = EEH.EARNING_NO");
                strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE");

                //2013-08-29
                if (parstrCurrentUserAccess == "U")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                    strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                    strQry.AppendLine(" AND EEH.COMPANY_NO = UEPCT.COMPANY_NO");
                    strQry.AppendLine(" AND EEH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EEH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
                }

                strQry.AppendLine(" WHERE EN.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL ");
                strQry.AppendLine(" AND EN.EARNING_NO = 4");
            }

            if (Convert.ToDouble(DataSet.Tables["Temp"].Rows[0]["SALARY_OVERTIME3_RATE"]) > 0)
            {
                strQry.AppendLine(" UNION ");
                strQry.AppendLine(" SELECT DISTINCT ");
                strQry.AppendLine(" EN.COMPANY_NO ");
                strQry.AppendLine(",EN.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",'Overtime (" + Convert.ToDouble(DataSet.Tables["Temp"].Rows[0]["SALARY_OVERTIME3_RATE"]).ToString("#0.00") + "%)' AS EARNING_DESC");
                strQry.AppendLine(",EN.EARNING_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING EN");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
                strQry.AppendLine(" ON EN.COMPANY_NO = EEH.COMPANY_NO");
                strQry.AppendLine(" AND EN.EARNING_NO = EEH.EARNING_NO");
                strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE");

                //2013-08-29
                if (parstrCurrentUserAccess == "U")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                    strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                    strQry.AppendLine(" AND EEH.COMPANY_NO = UEPCT.COMPANY_NO");
                    strQry.AppendLine(" AND EEH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EEH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
                }

                strQry.AppendLine(" WHERE EN.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL ");
                strQry.AppendLine(" AND EN.EARNING_NO = 5");
            }
          
            DataSet.Tables.Remove("Temp");

            strQry.AppendLine(" UNION");

            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" EN.COMPANY_NO ");
            strQry.AppendLine(",EEH.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",EN.EARNING_DESC");
            strQry.AppendLine(",EN.EARNING_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING EN");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
            strQry.AppendLine(" ON EN.COMPANY_NO = EEH.COMPANY_NO");
            strQry.AppendLine(" AND EN.EARNING_NO = EEH.EARNING_NO");
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE");

            //2013-08-29
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND EEH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EEH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE EN.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" AND EN.EARNING_NO > 6");

            strQry.AppendLine(" UNION");

            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" EN.COMPANY_NO ");
            strQry.AppendLine(",EEH.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",EN.EARNING_DESC");
            strQry.AppendLine(",EN.EARNING_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING EN");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
            strQry.AppendLine(" ON EN.COMPANY_NO = EEH.COMPANY_NO");
            strQry.AppendLine(" AND EN.EARNING_NO = EEH.EARNING_NO");
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE");

            //2013-08-29
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND EEH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EEH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE EN.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = 'S'");
            strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" AND EN.EARNING_NO > 6");
            strQry.AppendLine(" AND EN.EARNING_NO < 200");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Earning", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" D.COMPANY_NO ");
            strQry.AppendLine(",EDH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",D.DEDUCTION_DESC");
            strQry.AppendLine(",D.DEDUCTION_NO");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.DEDUCTION D");
            
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY EDH");
            strQry.AppendLine(" ON D.COMPANY_NO = EDH.COMPANY_NO");
            strQry.AppendLine(" AND D.DEDUCTION_NO = EDH.DEDUCTION_NO");
            strQry.AppendLine(" AND D.DEDUCTION_SUB_ACCOUNT_NO = EDH.DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(" AND D.PAY_CATEGORY_TYPE = EDH.PAY_CATEGORY_TYPE");

            //2013-08-29
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND EDH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EDH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EDH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }
                       
            strQry.AppendLine(" WHERE D.COMPANY_NO = " + parint64CompanyNo);

            //2013-06-21 Exclude T=Time Attendance
            strQry.AppendLine(" AND D.PAY_CATEGORY_TYPE IN ('W','S')");

            strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL ");
           
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Deduction", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",RUN_TYPE");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PAY_PERIOD_DATE");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY");
  
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            //2013-06-21 Exclude T=Time Attendance
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE IN ('W','S')");
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PAY_PERIOD_DATE DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Dates", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PCPH.COMPANY_NO");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_NO");
            strQry.AppendLine(",PCPH.RUN_TYPE");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON PCPH.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            //2013-08-29
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND PCPH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE PCPH.COMPANY_NO = " + parint64CompanyNo);

            //2013-06-21 Exclude T=Time Attendance
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE IN ('W','S')");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PCPH.COMPANY_NO");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_NO");
            strQry.AppendLine(",PCPH.RUN_TYPE");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
         
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategory", parint64CompanyNo);

            ////Company DataTable
            //DataTable myCompanyDataTable = clsDBConnectionObjects.Get_Company_Report_Header_Details_DataTable(parint64CompanyNo);

            //DataSet.Tables.Add(myCompanyDataTable);

            ////CompanyHeaderDetails DataTable
            //DataTable myCompanyHeaderDetailsDataTable = clsDBConnectionObjects.Get_Company_Header_Report_Header_Details_Positioning_DataTable(parint64CompanyNo);

            //DataSet.Tables.Add(myCompanyHeaderDetailsDataTable);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_Employee_Records(Int64 parint64CompanyNo, Int64 parint64CurrentUserNo, string parstrCurrentUserAccess)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            //2013-08-29
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND E.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" EPCH.COMPANY_NO");
            strQry.AppendLine(",EPCH.PAY_PERIOD_DATE");
            strQry.AppendLine(",EPCH.EMPLOYEE_NO");
            strQry.AppendLine(",EPCH.PAY_CATEGORY_NO");
            strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPCH.RUN_TYPE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");

            //2013-08-29
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND EPCH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPCH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE EPCH.COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeePayCategory", parint64CompanyNo);
            
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Print_Report_SpreadSheet(Int64 parint64CompanyNo, Int64 parint64CurrentUserNo, string parstrPayrollType, string parstrCurrentUserAccess, string parstrWhere, string parstrDeductionWhere, string parstrDateOption, string parstrByPayParameterOption, string parstrEarningsSelectedIN, string parstrDeductionsSelectedIN, string parstrPayCategoryNoIN, string parstrConsolidateOption, string parstrShowGrossEarningsDeductions)
        {
            StringBuilder strQry = new StringBuilder();
            StringBuilder strZeroFields = new StringBuilder();
            string strTakeOn = "N";
            int parintNumberOfHorizontalPages = 0;
            DateTime dtBeginOfFiscalYear;

            DataSet DataSet = new DataSet();
            DataSet CategoryDataSet = new DataSet();
            
            if (parstrDateOption == "Y"
            || parstrDateOption == "B")
            {
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" ISNULL(MAX(PAY_PERIOD_DATE),GETDATE()) AS MAX_PAY_PERIOD_DATE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY ");

                strQry.AppendLine(" WHERE PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "MaxDate", parint64CompanyNo);

                if (DataSet.Tables["MaxDate"].Rows.Count > 0)
                {
                    //Position Within Current Financial Year
                    if (Convert.ToDateTime(DataSet.Tables["MaxDate"].Rows[0]["MAX_PAY_PERIOD_DATE"]).Month > 2)
                    {
                        if (parstrDateOption == "Y")
                        {
                            dtBeginOfFiscalYear = new DateTime(Convert.ToDateTime(DataSet.Tables["MaxDate"].Rows[0]["MAX_PAY_PERIOD_DATE"]).Year, 3, 1);
                        }
                        else
                        {
                            dtBeginOfFiscalYear = new DateTime(Convert.ToDateTime(DataSet.Tables["MaxDate"].Rows[0]["MAX_PAY_PERIOD_DATE"]).Year - 1, 3, 1);
                        }
                    }
                    else
                    {
                        if (parstrDateOption == "Y")
                        {
                            dtBeginOfFiscalYear = new DateTime(Convert.ToDateTime(DataSet.Tables["MaxDate"].Rows[0]["MAX_PAY_PERIOD_DATE"]).Year - 1, 3, 1);
                        }
                        else
                        {
                            dtBeginOfFiscalYear = new DateTime(Convert.ToDateTime(DataSet.Tables["MaxDate"].Rows[0]["MAX_PAY_PERIOD_DATE"]).Year - 2, 3, 1);
                        }
                    }
                }
                else
                {
                    if (DateTime.Now.Month > 2)
                    {
                        if (parstrDateOption == "Y")
                        {
                            dtBeginOfFiscalYear = new DateTime(DateTime.Now.Year, 3, 1);
                        }
                        else
                        {
                            dtBeginOfFiscalYear = new DateTime(DateTime.Now.Year - 1, 3, 1);
                        }
                    }
                    else
                    {
                        if (parstrDateOption == "Y")
                        {
                            dtBeginOfFiscalYear = new DateTime(DateTime.Now.Year - 1, 3, 1);
                        }
                        else
                        {
                            dtBeginOfFiscalYear = new DateTime(DateTime.Now.Year - 2, 3, 1);
                        }
                    }
                }

                parstrWhere = parstrWhere.Replace("BEGINOFFISCALYEAR", dtBeginOfFiscalYear.ToString("yyyy-MM-dd"));
                parstrDeductionWhere = parstrDeductionWhere.Replace("BEGINOFFISCALYEAR", dtBeginOfFiscalYear.ToString("yyyy-MM-dd"));

                if (parstrDateOption == "B")
                {
                    parstrWhere = parstrWhere.Replace("ENDOFFISCALYEAR", dtBeginOfFiscalYear.AddYears(1).ToString("yyyy-MM-dd"));
                    parstrDeductionWhere = parstrDeductionWhere.Replace("ENDOFFISCALYEAR", dtBeginOfFiscalYear.AddYears(1).ToString("yyyy-MM-dd"));
                }
            }

            //NB PAY_CATEGORY_TYPE is handled in Where Clause from Client Machine
            if (parstrDeductionWhere.IndexOf("RUN_TYPE") > -1)
            {
                strTakeOn = "Y";
            }

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" EN.COMPANY_NO ");
            strQry.AppendLine(",EN.EARNING_DESC");
            strQry.AppendLine(",EN.EARNING_NO");
            strQry.AppendLine(",EN.EARNING_REPORT_HEADER1");
            strQry.AppendLine(",EN.EARNING_REPORT_HEADER2");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING EN");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
            strQry.AppendLine(" ON EN.COMPANY_NO = EEH.COMPANY_NO");
            strQry.AppendLine(" AND EN.EARNING_NO = EEH.EARNING_NO");
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE");

            strQry.AppendLine(parstrWhere.Replace("WHERE", "AND").Replace("EPCH", "EEH"));

            if (parstrPayCategoryNoIN != "")
            {
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_NO IN (" + parstrPayCategoryNoIN + ")");
            }

            //2013-08-29
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND EEH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EEH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE EN.COMPANY_NO = " + parint64CompanyNo);

            if (parstrEarningsSelectedIN != "")
            {
                strQry.AppendLine(" AND EN.EARNING_NO IN (" + parstrEarningsSelectedIN + ")");
            }

            strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" EN.EARNING_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), CategoryDataSet, "Earning", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" D.COMPANY_NO ");
            strQry.AppendLine(",D.DEDUCTION_DESC");
            strQry.AppendLine(",D.DEDUCTION_NO");
            strQry.AppendLine(",D.DEDUCTION_REPORT_HEADER1");
            strQry.AppendLine(",D.DEDUCTION_REPORT_HEADER2");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.DEDUCTION D");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY EDH");
            strQry.AppendLine(" ON D.COMPANY_NO = EDH.COMPANY_NO");

            strQry.AppendLine(" AND D.DEDUCTION_NO = EDH.DEDUCTION_NO");
            strQry.AppendLine(" AND D.DEDUCTION_SUB_ACCOUNT_NO = EDH.DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(" AND D.PAY_CATEGORY_TYPE = EDH.PAY_CATEGORY_TYPE");

            strQry.AppendLine(parstrWhere.Replace("WHERE", "AND").Replace("EPCH", "EDH"));

            //2013-08-29
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND EDH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EDH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EDH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE D.COMPANY_NO = " + parint64CompanyNo);

            if (parstrDeductionsSelectedIN != "")
            {
                strQry.AppendLine(" AND D.DEDUCTION_NO IN (" + parstrDeductionsSelectedIN + ")");
            }

            strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL ");
            //Tax is already Included
            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" D.DEDUCTION_DESC");

        clsDBConnectionObjects.Create_DataTable(strQry.ToString(), CategoryDataSet, "Deduction", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT  ");
            strQry.AppendLine(" OVERTIME1_RATE");
            strQry.AppendLine(",OVERTIME2_RATE");
            strQry.AppendLine(",OVERTIME3_RATE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Rates", parint64CompanyNo);

            string strOverTime1 = " (" + Convert.ToDouble(DataSet.Tables["Rates"].Rows[0]["OVERTIME1_RATE"]).ToString("000.00") + "%)";
            string strOverTime2 = " (" + Convert.ToDouble(DataSet.Tables["Rates"].Rows[0]["OVERTIME2_RATE"]).ToString("000.00") + "%)";
            string strOverTime3 = " (" + Convert.ToDouble(DataSet.Tables["Rates"].Rows[0]["OVERTIME3_RATE"]).ToString("000.00") + "%)";

            int intFieldCount = 1;

            int intGrossEarningsFieldCount = 0;
            int intGrossDeductionsFieldCount = 0;

            strQry.Clear();
            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.PRINT_SPREADSHEET");
            strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();
            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.PRINT_SPREADSHEET_HEADER");
            strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine("INSERT INTO InteractPayroll_#CompanyNo#.dbo.PRINT_SPREADSHEET");
            strQry.AppendLine("(USER_NO");
            strQry.AppendLine(",COMPANY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");

            if ((parstrDateOption == "M"
            || parstrDateOption == "Y"
            //2017-01-12
            || parstrDateOption == "B"
            || parstrDateOption == "O")
            && parstrConsolidateOption != "Y")
            {
                strQry.AppendLine(",PAY_PERIOD_DATE");
            }

            clsDBConnectionObjects.Initialise_DataSet_Numeric_Fields("PRINT_SPREADSHEET", ref strQry, ref strZeroFields, parint64CompanyNo);

            strQry.AppendLine(")");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(parint64CurrentUserNo.ToString());
            strQry.AppendLine("," + parint64CompanyNo);
            strQry.AppendLine(",EPCH.EMPLOYEE_NO");

            if (parstrByPayParameterOption == "Y")
            {
                strQry.AppendLine(",EPCH.PAY_CATEGORY_NO");
                strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE");
            }
            else
            {
                strQry.AppendLine(",-1 AS PAY_CATEGORY_NO");
                strQry.AppendLine(",'B' AS PAY_CATEGORY_TYPE");
            }
            
            if ((parstrDateOption == "M"
            || parstrDateOption == "Y"
            //2017-01-12
            || parstrDateOption == "B"
            || parstrDateOption == "O")
            && parstrConsolidateOption != "Y")
            {
                strQry.AppendLine(",EPCH.PAY_PERIOD_DATE");
            }

            strQry.Append(strZeroFields);

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");

            //2013-08-29
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND EPCH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPCH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            if (parstrDateOption != "P")
            {
                if (parstrWhere.IndexOf("EMPLOYEE_NO") == -1)
                {
                    //2017-02-24 - Link to Employee for PAY_CATEGORY_TYPE
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE EMP");
                    strQry.AppendLine(" ON EPCH.COMPANY_NO = EMP.COMPANY_NO ");
                    strQry.AppendLine(" AND EPCH.EMPLOYEE_NO = EMP.EMPLOYEE_NO ");
                    strQry.AppendLine(" AND EMP.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                    strQry.AppendLine(" AND EMP.DATETIME_DELETE_RECORD IS NULL ");
                }
            }

            strQry.AppendLine(parstrWhere);
            //strQry += parstrWhere.Replace("EPCH.", "");

            if (parstrPayCategoryNoIN != "")
            {
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO IN (" + parstrPayCategoryNoIN + ")");
            }

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" EPCH.EMPLOYEE_NO");

            if (parstrByPayParameterOption == "Y")
            {
                strQry.AppendLine(",EPCH.PAY_CATEGORY_NO");
                strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE");
            }
            
            if ((parstrDateOption == "M"
            || parstrDateOption == "Y"
            //2017-01-12
            || parstrDateOption == "B"
            || parstrDateOption == "O")
            && parstrConsolidateOption != "Y")
            {
                strQry.AppendLine(",EPCH.PAY_PERIOD_DATE");
            }

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine("INSERT INTO InteractPayroll_#CompanyNo#.dbo.PRINT_SPREADSHEET_HEADER");
            strQry.AppendLine("(USER_NO");
            strQry.AppendLine(",COMPANY_NO)");
            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(parint64CurrentUserNo.ToString());
            strQry.AppendLine("," + parint64CompanyNo);
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.PRINT_SPREADSHEET ");
            strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo);
            strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            for (int intRow = 0; intRow < CategoryDataSet.Tables["Earning"].Rows.Count; intRow++)
            {
                DataSet.Dispose();
                DataSet = null;

                DataSet = new DataSet();

                strQry.Clear();
                strQry.AppendLine(" SELECT ");

                if (parstrByPayParameterOption == "Y")
                {
                    strQry.AppendLine(" EPCH.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE");
                }
                else
                {
                    strQry.AppendLine(" -1 AS PAY_CATEGORY_NO");
                    strQry.AppendLine(",'B' AS PAY_CATEGORY_TYPE");
                }
                
                if ((parstrDateOption == "M"
                || parstrDateOption == "Y"
                //2017-01-12
                || parstrDateOption == "B"
                || parstrDateOption == "O")
                && parstrConsolidateOption != "Y")
                {
                    strQry.AppendLine(",EPCH.PAY_PERIOD_DATE");
                }

                strQry.AppendLine(",EPCH.EMPLOYEE_NO");
                strQry.AppendLine(",SUM(EEH.TOTAL) AS TOTAL_FOR_EMPLOYEE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");

                //2013-08-29
                if (parstrCurrentUserAccess == "U")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                    strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                    strQry.AppendLine(" AND EPCH.COMPANY_NO = UEPCT.COMPANY_NO");
                    strQry.AppendLine(" AND EPCH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
                }
                
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
                strQry.AppendLine(" ON EPCH.COMPANY_NO = EEH.COMPANY_NO ");
                strQry.AppendLine(" AND EPCH.PAY_PERIOD_DATE = EEH.PAY_PERIOD_DATE ");
                strQry.AppendLine(" AND EPCH.EMPLOYEE_NO = EEH.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPCH.RUN_TYPE = EEH.RUN_TYPE ");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = EEH.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EEH.EARNING_NO = " + CategoryDataSet.Tables["Earning"].Rows[intRow]["EARNING_NO"].ToString());

                if (parstrDateOption != "P")
                {
                    if (parstrWhere.IndexOf("EMPLOYEE_NO") == -1)
                    {
                        //2017-02-24 - Link to Employee for PAY_CATEGORY_TYPE
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE EMP");
                        strQry.AppendLine(" ON EPCH.COMPANY_NO = EMP.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCH.EMPLOYEE_NO = EMP.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EMP.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                        strQry.AppendLine(" AND EMP.DATETIME_DELETE_RECORD IS NULL ");
                    }
                }

                //Add Where Statement
                //Add Where Statement
                strQry.AppendLine(parstrWhere);

                if (parstrPayCategoryNoIN != "")
                {
                    strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO IN (" + parstrPayCategoryNoIN + ")");
                }
              
                strQry.AppendLine(" GROUP BY ");

                if (parstrByPayParameterOption == "Y")
                {
                    strQry.AppendLine(" EPCH.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE");
                }

                if ((parstrDateOption == "M"
                || parstrDateOption == "Y"
                //2017-01-12
                || parstrDateOption == "B"
                || parstrDateOption == "O")
                && parstrConsolidateOption != "Y")
                {
                    strQry.AppendLine(",EPCH.PAY_PERIOD_DATE");
                    strQry.AppendLine(",EPCH.EMPLOYEE_NO");
                }
                else
                {
                    if (parstrByPayParameterOption == "Y")
                    {
                        strQry.AppendLine(",EPCH.EMPLOYEE_NO");
                    }
                    else
                    {
                        strQry.AppendLine(" EPCH.EMPLOYEE_NO");
                    }
                }

                if (parstrEarningsSelectedIN == ""
                    & strTakeOn == "N")
                {
                    strQry.AppendLine(" HAVING SUM(EEH.TOTAL) > 0");
                }

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parint64CompanyNo);

                for (int intRowEmployee = 0; intRowEmployee < DataSet.Tables[0].Rows.Count; intRowEmployee++)
                {
                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PRINT_SPREADSHEET");
                    strQry.AppendLine(" SET FIELD" + intFieldCount + "_AMOUNT = " + DataSet.Tables[0].Rows[intRowEmployee]["TOTAL_FOR_EMPLOYEE"]);
                    strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo);
                    strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables[0].Rows[intRowEmployee]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[0].Rows[intRowEmployee]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables[0].Rows[intRowEmployee]["EMPLOYEE_NO"].ToString());

                    if ((parstrDateOption == "M"
                    || parstrDateOption == "Y"
                    //2017-01-12
                    || parstrDateOption == "B"
                    || parstrDateOption == "O")
                    && parstrConsolidateOption != "Y")
                    {
                        strQry.AppendLine(" AND PAY_PERIOD_DATE = '" + Convert.ToDateTime(DataSet.Tables[0].Rows[intRowEmployee]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'");
                    }

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                }

                if (DataSet.Tables[0].Rows.Count > 0)
                {
                    //Set All Header Descriptions to Same Value (Even If Employee does NOT have This Deduction Link)
                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PRINT_SPREADSHEET_HEADER");
                    strQry.AppendLine(" SET ");

                    if (CategoryDataSet.Tables["Earning"].Rows[intRow]["EARNING_REPORT_HEADER2"].ToString() == "")
                    {
                        strQry.AppendLine(" FIELD" + intFieldCount + "_2_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(CategoryDataSet.Tables["Earning"].Rows[intRow]["EARNING_REPORT_HEADER1"].ToString()));
                    }
                    else
                    {
                        strQry.AppendLine(" FIELD" + intFieldCount + "_1_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(CategoryDataSet.Tables["Earning"].Rows[intRow]["EARNING_REPORT_HEADER1"].ToString()));
                        strQry.AppendLine(",FIELD" + intFieldCount + "_2_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(CategoryDataSet.Tables["Earning"].Rows[intRow]["EARNING_REPORT_HEADER2"].ToString()));
                    }
                        
                    strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo);
                    strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    intFieldCount += 1;
                }
            }

            if (parstrShowGrossEarningsDeductions == "Y"
                & parstrEarningsSelectedIN != "-1")
            {
                //Insert Gross Earnings
                DataSet.Dispose();
                DataSet = null;
                DataSet = new DataSet();

                strQry.Clear();
                strQry.AppendLine(" SELECT ");

                if (parstrByPayParameterOption == "Y")
                {
                    strQry.AppendLine(" EPCH.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE");
                }
                else
                {
                    strQry.AppendLine(" -1 AS PAY_CATEGORY_NO");
                    strQry.AppendLine(",'B' AS PAY_CATEGORY_TYPE");
                }
                
                if ((parstrDateOption == "M"
                || parstrDateOption == "Y"
                //2017-01-12
                || parstrDateOption == "B"
                || parstrDateOption == "O")
                && parstrConsolidateOption != "Y")
                {
                    strQry.AppendLine(",EPCH.PAY_PERIOD_DATE");
                }

                strQry.AppendLine(",EPCH.EMPLOYEE_NO");
                strQry.AppendLine(",SUM(EEH.TOTAL) AS TOTAL_FOR_EMPLOYEE");
                
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");

                //2013-08-29
                if (parstrCurrentUserAccess == "U")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                    strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                    strQry.AppendLine(" AND EPCH.COMPANY_NO = UEPCT.COMPANY_NO");
                    strQry.AppendLine(" AND EPCH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
                }
                
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
                strQry.AppendLine(" ON EPCH.COMPANY_NO = EEH.COMPANY_NO ");
                strQry.AppendLine(" AND EPCH.PAY_PERIOD_DATE = EEH.PAY_PERIOD_DATE ");
                strQry.AppendLine(" AND EPCH.EMPLOYEE_NO = EEH.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPCH.RUN_TYPE = EEH.RUN_TYPE ");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = EEH.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE ");

                if (parstrEarningsSelectedIN != "")
                {
                    strQry.AppendLine(" AND EEH.EARNING_NO IN (" + parstrEarningsSelectedIN + ")");
                }

                if (parstrDateOption != "P")
                {
                    if (parstrWhere.IndexOf("EMPLOYEE_NO") == -1)
                    {
                        //2017-02-24 - Link to Employee for PAY_CATEGORY_TYPE
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE EMP");
                        strQry.AppendLine(" ON EPCH.COMPANY_NO = EMP.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCH.EMPLOYEE_NO = EMP.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EMP.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                        strQry.AppendLine(" AND EMP.DATETIME_DELETE_RECORD IS NULL ");
                    }
                }

                strQry.AppendLine(parstrWhere);

                strQry.AppendLine(" GROUP BY ");

                if (parstrByPayParameterOption == "Y")
                {
                    strQry.AppendLine(" EPCH.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE");
                }

                if ((parstrDateOption == "M"
                || parstrDateOption == "Y"
                //2017-01-12
                || parstrDateOption == "B"
                || parstrDateOption == "O")
                && parstrConsolidateOption != "Y")
                {
                    strQry.AppendLine(",EPCH.PAY_PERIOD_DATE");
                    strQry.AppendLine(",EPCH.EMPLOYEE_NO");
                }
                else
                {
                    if (parstrByPayParameterOption == "Y")
                    {
                        strQry.AppendLine(",EPCH.EMPLOYEE_NO");

                    }
                    else
                    {
                        strQry.AppendLine(" EPCH.EMPLOYEE_NO");
                    }
                }

                if (parstrEarningsSelectedIN == ""
                    & strTakeOn == "N")
                {
                    strQry.AppendLine(" HAVING SUM(EEH.TOTAL) <> 0");
                }

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parint64CompanyNo);

                for (int intRowEmployee = 0; intRowEmployee < DataSet.Tables[0].Rows.Count; intRowEmployee++)
                {
                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PRINT_SPREADSHEET");
                    strQry.AppendLine(" SET FIELD" + intFieldCount + "_AMOUNT = " + DataSet.Tables[0].Rows[intRowEmployee]["TOTAL_FOR_EMPLOYEE"]);
                    strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo);
                    strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables[0].Rows[intRowEmployee]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[0].Rows[intRowEmployee]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables[0].Rows[intRowEmployee]["EMPLOYEE_NO"].ToString());

                    if ((parstrDateOption == "M"
                    || parstrDateOption == "Y"
                    //2017-01-12
                    || parstrDateOption == "B"
                    || parstrDateOption == "O")
                    && parstrConsolidateOption != "Y")
                    {
                        strQry.AppendLine(" AND PAY_PERIOD_DATE = '" + Convert.ToDateTime(DataSet.Tables[0].Rows[intRowEmployee]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'");
                    }

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                }

                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PRINT_SPREADSHEET_HEADER");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" FIELD" + intFieldCount + "_1_NAME = " + clsDBConnectionObjects.Text2DynamicSQL("Earnings"));
                strQry.AppendLine(",FIELD" + intFieldCount + "_2_NAME = " + clsDBConnectionObjects.Text2DynamicSQL("Total"));

                intGrossEarningsFieldCount = intFieldCount;

                intFieldCount += 1;

                strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            }

            //Now do Deductions
            for (int intRow = 0; intRow < CategoryDataSet.Tables["Deduction"].Rows.Count; intRow++)
            {
                DataSet = new DataSet();

                strQry.Clear();
                strQry.AppendLine(" SELECT ");

                if (parstrByPayParameterOption == "Y")
                {
                    strQry.AppendLine(" EPCH.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE");
                }
                else
                {
                    strQry.AppendLine(" -1 AS PAY_CATEGORY_NO");
                    strQry.AppendLine(",'B' AS PAY_CATEGORY_TYPE");
                }
                
                if ((parstrDateOption == "M"
                || parstrDateOption == "Y"
                //2017-01-12
                || parstrDateOption == "B"
                || parstrDateOption == "O")
                && parstrConsolidateOption != "Y")
                {
                    strQry.AppendLine(",EDH.PAY_PERIOD_DATE");
                }

                strQry.AppendLine(",EDH.EMPLOYEE_NO");
                strQry.AppendLine(",SUM(EDH.TOTAL) AS TOTAL_FOR_EMPLOYEE");
                
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");
                
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY EDH");
                strQry.AppendLine(" ON EPCH.COMPANY_NO = EDH.COMPANY_NO");
                strQry.AppendLine(" AND EDH.DEDUCTION_NO = " + CategoryDataSet.Tables["Deduction"].Rows[intRow]["DEDUCTION_NO"].ToString());
                strQry.AppendLine(" AND EPCH.EMPLOYEE_NO = EDH.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPCH.PAY_PERIOD_DATE = EDH.PAY_PERIOD_DATE ");

                //2013-08-29
                if (parstrCurrentUserAccess == "U")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                    strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                    strQry.AppendLine(" AND EPCH.COMPANY_NO = UEPCT.COMPANY_NO");
                    strQry.AppendLine(" AND EPCH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
                }

                if (parstrDateOption != "P")
                {
                    if (parstrWhere.IndexOf("EMPLOYEE_NO") == -1)
                    {
                        //2017-02-24 - Link to Employee for PAY_CATEGORY_TYPE
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE EMP");
                        strQry.AppendLine(" ON EPCH.COMPANY_NO = EMP.COMPANY_NO ");
                        strQry.AppendLine(" AND EPCH.EMPLOYEE_NO = EMP.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EMP.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                        strQry.AppendLine(" AND EMP.DATETIME_DELETE_RECORD IS NULL ");
                    }
                }

                strQry.AppendLine(" WHERE EPCH.COMPANY_NO = " + parint64CompanyNo);
               
                //Add Where Clause
                strQry.AppendLine(parstrDeductionWhere);

                strQry.AppendLine(" GROUP BY ");

                if ((parstrDateOption == "M"
                || parstrDateOption == "Y"
                //2017-01-12
                || parstrDateOption == "B"
                || parstrDateOption == "O")
                && parstrConsolidateOption != "Y")
                {
                    if (parstrByPayParameterOption == "Y")
                    {
                        strQry.AppendLine(" EPCH.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE");
                    }
                    else
                    {
                        strQry.AppendLine(" PAY_CATEGORY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    }

                  
                    strQry.AppendLine(",EDH.PAY_PERIOD_DATE");
                    strQry.AppendLine(",EDH.EMPLOYEE_NO");
                }
                else
                {
                    if (parstrByPayParameterOption == "Y")
                    {
                        strQry.AppendLine(" EPCH.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",EDH.EMPLOYEE_NO");
                    }
                    else
                    {
                        if (parstrByPayParameterOption == "Y")
                        {
                            strQry.AppendLine(",EDH.EMPLOYEE_NO");

                        }
                        else
                        {
                            strQry.AppendLine(" EDH.EMPLOYEE_NO");
                        }
                    }
                }

                if (parstrDeductionsSelectedIN == ""
                    & strTakeOn == "N")
                {
                    strQry.AppendLine(" HAVING SUM(EDH.TOTAL) <> 0");
                }

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parint64CompanyNo);

                for (int intRowEmployee = 0; intRowEmployee < DataSet.Tables[0].Rows.Count; intRowEmployee++)
                {
                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PRINT_SPREADSHEET");
                    strQry.AppendLine(" SET FIELD" + intFieldCount + "_AMOUNT = " + DataSet.Tables[0].Rows[intRowEmployee]["TOTAL_FOR_EMPLOYEE"]);
                    strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo);
                    strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables[0].Rows[intRowEmployee]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[0].Rows[intRowEmployee]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables[0].Rows[intRowEmployee]["EMPLOYEE_NO"].ToString());

                    if ((parstrDateOption == "M"
                    || parstrDateOption == "Y"
                    //2017-01-12
                    || parstrDateOption == "B"
                    || parstrDateOption == "O")
                    && parstrConsolidateOption != "Y")
                    {
                        strQry.AppendLine(" AND PAY_PERIOD_DATE = '" + Convert.ToDateTime(DataSet.Tables[0].Rows[intRowEmployee]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'");
                    }

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                }

                if (DataSet.Tables[0].Rows.Count > 0)
                {
                    //Set All Header Descriptions to Same Value (Even If Employee does NOT have This Deduction Link)
                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PRINT_SPREADSHEET_HEADER");
                    strQry.AppendLine(" SET ");

                    if (CategoryDataSet.Tables["Deduction"].Rows[intRow]["DEDUCTION_REPORT_HEADER2"].ToString() == "")
                    {
                        strQry.AppendLine(" FIELD" + intFieldCount + "_2_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(CategoryDataSet.Tables["Deduction"].Rows[intRow]["DEDUCTION_REPORT_HEADER1"].ToString()));
                    }
                    else
                    {
                        strQry.AppendLine(" FIELD" + intFieldCount + "_1_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(CategoryDataSet.Tables["Deduction"].Rows[intRow]["DEDUCTION_REPORT_HEADER1"].ToString()));
                        strQry.AppendLine(",FIELD" + intFieldCount + "_2_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(CategoryDataSet.Tables["Deduction"].Rows[intRow]["DEDUCTION_REPORT_HEADER2"].ToString()));
                    }

                    strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo);
                    strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    intFieldCount += 1;
                }

                DataSet.Dispose();
                DataSet = null;
            }

            if (parstrShowGrossEarningsDeductions == "Y"
                & parstrDeductionsSelectedIN != "-1")
            {
                //Now Do Gross Deductions and NETT
                string strGrossEarningsHaving = " SUM(FIELD" + Convert.ToString(intGrossEarningsFieldCount) + "_AMOUNT) <> 0 ";
                string strGrossDeductionsHaving = " OR SUM(";

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PAY_CATEGORY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");

                if ((parstrDateOption == "M"
                || parstrDateOption == "Y"
                //2017-01-12
                || parstrDateOption == "B"
                || parstrDateOption == "O")
                && parstrConsolidateOption != "Y")
                {
                    strQry.AppendLine(",PAY_PERIOD_DATE");
                }

                strQry.AppendLine(",EMPLOYEE_NO");

                if (parstrEarningsSelectedIN != "-1")
                {
                    strQry.AppendLine(",SUM(FIELD" + Convert.ToString(intGrossEarningsFieldCount) + "_AMOUNT");
                    strQry.AppendLine(") AS EARNINGS_TOTAL");
                }
                strQry.AppendLine(",SUM(");

                if (intGrossEarningsFieldCount + 1 >= intFieldCount)
                {
                    strQry.AppendLine(" FIELD" + intFieldCount.ToString() + "_AMOUNT");
                    strGrossDeductionsHaving += " FIELD" + intFieldCount.ToString() + "_AMOUNT";
                }
                else
                {
                    for (int intColumn = intGrossEarningsFieldCount + 1; intColumn < intFieldCount; intColumn++)
                    {
                        if (intColumn == intGrossEarningsFieldCount + 1)
                        {
                            strQry.AppendLine(" FIELD" + intColumn.ToString() + "_AMOUNT");
                            strGrossDeductionsHaving += " FIELD" + intColumn.ToString() + "_AMOUNT";
                        }
                        else
                        {
                            strQry.AppendLine(" + FIELD" + intColumn.ToString() + "_AMOUNT");
                            strGrossDeductionsHaving += " + FIELD" + intColumn.ToString() + "_AMOUNT";
                        }
                    }
                }

                strGrossDeductionsHaving += ") <> 0";

                strQry.AppendLine(" ) AS DEDUCTION_TOTAL");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PRINT_SPREADSHEET");
                strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" PAY_CATEGORY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");

                if ((parstrDateOption == "M"
                || parstrDateOption == "Y"
                //2017-01-12
                || parstrDateOption == "B"
                || parstrDateOption == "O")
                && parstrConsolidateOption != "Y")
                {
                    strQry.AppendLine(",PAY_PERIOD_DATE");
                }

                strQry.AppendLine(",EMPLOYEE_NO");

                if (parstrEarningsSelectedIN != "-1")
                {
                    strQry.AppendLine(" HAVING " + strGrossEarningsHaving + strGrossDeductionsHaving);
                }
                else
                {
                    strQry.AppendLine(" HAVING " + strGrossDeductionsHaving.Replace("OR",""));
                }

                DataSet DeductDataSet = new DataSet();

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DeductDataSet, "Temp", parint64CompanyNo);

                intGrossDeductionsFieldCount = intFieldCount;
                intFieldCount += 1;

                for (int intRowEmployee = 0; intRowEmployee < DeductDataSet.Tables[0].Rows.Count; intRowEmployee++)
                {
                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PRINT_SPREADSHEET");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" FIELD" + intGrossDeductionsFieldCount + "_AMOUNT = " + DeductDataSet.Tables[0].Rows[intRowEmployee]["DEDUCTION_TOTAL"]);

                    if (parstrEarningsSelectedIN != "-1"
                        & parstrDeductionsSelectedIN != "-1")
                    {
                        strQry.AppendLine(",FIELD" + intFieldCount + "_AMOUNT = " + (Convert.ToDouble(DeductDataSet.Tables[0].Rows[intRowEmployee]["EARNINGS_TOTAL"]) - Convert.ToDouble(DeductDataSet.Tables[0].Rows[intRowEmployee]["DEDUCTION_TOTAL"])));
                    }
                    strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo);
                    strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DeductDataSet.Tables[0].Rows[intRowEmployee]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DeductDataSet.Tables[0].Rows[intRowEmployee]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + DeductDataSet.Tables[0].Rows[intRowEmployee]["EMPLOYEE_NO"].ToString());

                    if ((parstrDateOption == "M"
                    || parstrDateOption == "Y"
                    //2017-01-12
                    || parstrDateOption == "B"
                    || parstrDateOption == "O")
                    && parstrConsolidateOption != "Y")
                    {
                        strQry.AppendLine(" AND PAY_PERIOD_DATE = '" + Convert.ToDateTime(DeductDataSet.Tables[0].Rows[intRowEmployee]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd") + "'");
                    }

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                }

                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PRINT_SPREADSHEET_HEADER");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" FIELD" + intGrossDeductionsFieldCount + "_1_NAME = 'Deductions'");
                strQry.AppendLine(",FIELD" + intGrossDeductionsFieldCount + "_2_NAME = 'Total'");

                if (parstrEarningsSelectedIN != "-1"
                & parstrDeductionsSelectedIN != "-1")
                {
                    strQry.AppendLine(",FIELD" + intFieldCount + "_1_NAME = 'Final'");
                    strQry.AppendLine(",FIELD" + intFieldCount + "_2_NAME = 'Total'");

                    //2012-11-08
                    intFieldCount += 1;
                }

                strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            }

            if (parstrEarningsSelectedIN == ""
               & parstrDeductionsSelectedIN == "")
            {
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.PRINT_SPREADSHEET");
                strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND FIELD1_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD2_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD3_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD4_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD5_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD6_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD7_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD8_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD9_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD10_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD11_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD12_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD13_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD14_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD15_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD16_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD17_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD18_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD19_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD20_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD21_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD22_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD23_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD24_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD25_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD26_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD27_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD28_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD29_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD30_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD31_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD32_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD33_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD34_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD35_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD36_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD37_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD38_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD39_AMOUNT = 0");
                strQry.AppendLine(" AND FIELD40_AMOUNT = 0");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            }

            DataSet = new DataSet();

            ////Create SQL Statement
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FIELD1_2_NAME");
            strQry.AppendLine(",FIELD2_2_NAME");
            strQry.AppendLine(",FIELD3_2_NAME");
            strQry.AppendLine(",FIELD4_2_NAME");
            strQry.AppendLine(",FIELD5_2_NAME");
            strQry.AppendLine(",FIELD6_2_NAME");
            strQry.AppendLine(",FIELD7_2_NAME");
            strQry.AppendLine(",FIELD8_2_NAME");
            strQry.AppendLine(",FIELD9_2_NAME");
            strQry.AppendLine(",FIELD10_2_NAME");
            strQry.AppendLine(",FIELD11_2_NAME");
            strQry.AppendLine(",FIELD12_2_NAME");
            strQry.AppendLine(",FIELD13_2_NAME");
            strQry.AppendLine(",FIELD14_2_NAME");
            strQry.AppendLine(",FIELD15_2_NAME");
            strQry.AppendLine(",FIELD16_2_NAME");
            strQry.AppendLine(",FIELD17_2_NAME");
            strQry.AppendLine(",FIELD18_2_NAME");
            strQry.AppendLine(",FIELD19_2_NAME");
            strQry.AppendLine(",FIELD20_2_NAME");
            strQry.AppendLine(",FIELD21_2_NAME");
            strQry.AppendLine(",FIELD22_2_NAME");
            strQry.AppendLine(",FIELD23_2_NAME");
            strQry.AppendLine(",FIELD24_2_NAME");
            strQry.AppendLine(",FIELD25_2_NAME");
            strQry.AppendLine(",FIELD26_2_NAME");
            strQry.AppendLine(",FIELD27_2_NAME");
            strQry.AppendLine(",FIELD28_2_NAME");
            strQry.AppendLine(",FIELD29_2_NAME");
            strQry.AppendLine(",FIELD30_2_NAME");
            strQry.AppendLine(",FIELD31_2_NAME");
            strQry.AppendLine(",FIELD32_2_NAME");
            strQry.AppendLine(",FIELD33_2_NAME");
            strQry.AppendLine(",FIELD34_2_NAME");
            strQry.AppendLine(",FIELD35_2_NAME");
            strQry.AppendLine(",FIELD36_2_NAME");
            strQry.AppendLine(",FIELD37_2_NAME");
            strQry.AppendLine(",FIELD38_2_NAME");
            strQry.AppendLine(",FIELD39_2_NAME");
            strQry.AppendLine(",FIELD40_2_NAME");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PRINT_SPREADSHEET_HEADER ");
            strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo);
            strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "ReportCleanup", parint64CompanyNo);

            if (DataSet.Tables["ReportCleanup"].Rows.Count > 0)
            {
                for (int intRowCount = 1; intRowCount < 41; intRowCount++)
                {
                    if (DataSet.Tables["ReportCleanup"].Rows[0]["FIELD" + intRowCount + "_2_NAME"].ToString() == "")
                    {
                        strQry.Clear();
                        strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PRINT_SPREADSHEET");
                        strQry.AppendLine(" SET  FIELD" + intRowCount + "_AMOUNT = NULL ");
                        strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo);
                        strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo);

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                    }
                }
            }

            DataSet.Tables.Remove("ReportCleanup");

            //NB intFieldCount is 1 More Than Actual 
            if (intFieldCount > 8)
            {
                if (intFieldCount > 28)
                {
                    parintNumberOfHorizontalPages = 4;
                }
                else
                {
                    if (intFieldCount > 18)
                    {
                        parintNumberOfHorizontalPages = 3;
                    }
                    else
                    {
                        parintNumberOfHorizontalPages = 2;
                    }
                }
            }
            else
            {
                parintNumberOfHorizontalPages = 1;
            }

            //Create SQL Statement
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(parintNumberOfHorizontalPages + " AS HORIZONTAL_PAGE_NUMBER");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY ");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "NumberOfHorizontalPages", parint64CompanyNo);

            if (parintNumberOfHorizontalPages == 1)
            {
                byte[] byteReport = Print_Horizontal_SpreadSheet_Page(parint64CompanyNo, parint64CurrentUserNo, 1, 1);

                DataSet TempDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(byteReport);

                DataSet.Merge(TempDataSet);
            }
          
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Print_Report_Normal(Int64 parint64CompanyNo, Int64 parint64CurrentUserNo, string parstrPayrollType, string parstrCurrentUserAccess, string parstrWhere, string parstrDeductionWhere, string parstrDateOption, string parstrByPayCategoryOption, string parstrEarningsSelectedIN, string parstrDeductionsSelectedIN, string parstrPayCategoryNoIN, string parstrConsolidateOption)
        {
            StringBuilder strQry = new StringBuilder();
            string strOverTime1 = "";
            string strOverTime2 = "";
            string strOverTime3 = "";
            string strTakeOn = "N";
            DateTime dtBeginOfFiscalYear;

            DataSet DataSet = new DataSet();
            DataSet CategoryDataSet = new DataSet();
            
            if (parstrDateOption == "Y"
            || parstrDateOption == "B")
            {
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" ISNULL(MAX(PAY_PERIOD_DATE),GETDATE()) AS MAX_PAY_PERIOD_DATE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY ");

                strQry.AppendLine(" WHERE PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "MaxDate", parint64CompanyNo);

                if (DataSet.Tables["MaxDate"].Rows.Count > 0)
                {
                    //Position Within Current Financial Year
                    if (Convert.ToDateTime(DataSet.Tables["MaxDate"].Rows[0]["MAX_PAY_PERIOD_DATE"]).Month > 2)
                    {
                        if (parstrDateOption == "Y")
                        {
                            dtBeginOfFiscalYear = new DateTime(Convert.ToDateTime(DataSet.Tables["MaxDate"].Rows[0]["MAX_PAY_PERIOD_DATE"]).Year, 3, 1);
                        }
                        else
                        {
                            dtBeginOfFiscalYear = new DateTime(Convert.ToDateTime(DataSet.Tables["MaxDate"].Rows[0]["MAX_PAY_PERIOD_DATE"]).Year - 1, 3, 1);
                        }
                    }
                    else
                    {
                        if (parstrDateOption == "Y")
                        {
                            dtBeginOfFiscalYear = new DateTime(Convert.ToDateTime(DataSet.Tables["MaxDate"].Rows[0]["MAX_PAY_PERIOD_DATE"]).Year - 1, 3, 1);
                        }
                        else
                        {
                            dtBeginOfFiscalYear = new DateTime(Convert.ToDateTime(DataSet.Tables["MaxDate"].Rows[0]["MAX_PAY_PERIOD_DATE"]).Year - 2, 3, 1);
                        }
                    }
                }
                else
                {
                    if (DateTime.Now.Month > 2)
                    {
                        if (parstrDateOption == "Y")
                        {
                            dtBeginOfFiscalYear = new DateTime(DateTime.Now.Year, 3, 1);
                        }
                        else
                        {
                            dtBeginOfFiscalYear = new DateTime(DateTime.Now.Year - 1, 3, 1);
                        }
                    }
                    else
                    {
                        if (parstrDateOption == "Y")
                        {
                            dtBeginOfFiscalYear = new DateTime(DateTime.Now.Year - 1, 3, 1);
                        }
                        else
                        {
                            dtBeginOfFiscalYear = new DateTime(DateTime.Now.Year - 2, 3, 1);
                        }
                    }
                }

                parstrWhere = parstrWhere.Replace("BEGINOFFISCALYEAR", dtBeginOfFiscalYear.ToString("yyyy-MM-dd"));
                parstrDeductionWhere = parstrDeductionWhere.Replace("BEGINOFFISCALYEAR", dtBeginOfFiscalYear.ToString("yyyy-MM-dd"));

                if (parstrDateOption == "B")
                {
                    parstrWhere = parstrWhere.Replace("ENDOFFISCALYEAR", dtBeginOfFiscalYear.AddYears(1).ToString("yyyy-MM-dd"));
                    parstrDeductionWhere = parstrDeductionWhere.Replace("ENDOFFISCALYEAR", dtBeginOfFiscalYear.AddYears(1).ToString("yyyy-MM-dd"));
                }
            }

            //NB PAY_CATEGORY_TYPE is handled in Where Clause from Client Machine
            if (parstrDeductionWhere.IndexOf("RUN_TYPE") > -1)
            {
                strTakeOn = "Y";
            }

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" EN.COMPANY_NO ");
            strQry.AppendLine(",EN.EARNING_DESC");
            strQry.AppendLine(",EN.EARNING_NO");
            strQry.AppendLine(",EN.EARNING_REPORT_HEADER1");
            strQry.AppendLine(",EN.EARNING_REPORT_HEADER2");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING EN");
            
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
            strQry.AppendLine(" ON EN.COMPANY_NO = EEH.COMPANY_NO");
            strQry.AppendLine(" AND EN.EARNING_NO = EEH.EARNING_NO");
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE");

            strQry.AppendLine(parstrWhere.Replace("WHERE", "AND").Replace("EPCH", "EEH"));

            if (parstrPayCategoryNoIN != "")
            {
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_NO IN (" + parstrPayCategoryNoIN + ")");
            }

            //2013-08-29
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND EEH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EEH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE EN.COMPANY_NO = " + parint64CompanyNo);

            if (parstrEarningsSelectedIN != "")
            {
                strQry.AppendLine(" AND EN.EARNING_NO IN (" + parstrEarningsSelectedIN + ")");
            }

            strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" EN.EARNING_DESC");
           
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), CategoryDataSet, "Earning", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" D.COMPANY_NO ");
            strQry.AppendLine(",D.DEDUCTION_DESC");
            strQry.AppendLine(",D.DEDUCTION_NO");
            strQry.AppendLine(",D.DEDUCTION_REPORT_HEADER1");
            strQry.AppendLine(",D.DEDUCTION_REPORT_HEADER2");
           
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.DEDUCTION D");
            
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY EDH");
            strQry.AppendLine(" ON D.COMPANY_NO = EDH.COMPANY_NO");
            strQry.AppendLine(" AND D.DEDUCTION_NO = EDH.DEDUCTION_NO");
            strQry.AppendLine(" AND D.DEDUCTION_SUB_ACCOUNT_NO = EDH.DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(" AND D.PAY_CATEGORY_TYPE = EDH.PAY_CATEGORY_TYPE");

            //2013-08-29
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND EDH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EDH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EDH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(parstrWhere.Replace("WHERE", "AND").Replace("EPCH", "EDH"));
        
            strQry.AppendLine(" WHERE D.COMPANY_NO = " + parint64CompanyNo);

            if (parstrDeductionsSelectedIN != "")
            {
                strQry.AppendLine(" AND D.DEDUCTION_NO IN (" + parstrDeductionsSelectedIN + ")");
            }

            strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL ");
            //Tax is already Included
            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" D.DEDUCTION_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), CategoryDataSet, "Deduction", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.PRINT_EARNINGS_DEDUCTIONS");
            strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            DataSet DataSetTemp = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" OVERTIME1_RATE");
            strQry.AppendLine(",OVERTIME2_RATE");
            strQry.AppendLine(",OVERTIME3_RATE");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSetTemp, "Rates", parint64CompanyNo);

            if (Convert.ToDouble(DataSetTemp.Tables["Rates"].Rows[0]["OVERTIME1_RATE"]) == 0)
            {
                //TakeOn
            }
            else
            {
                strOverTime1 = " (" + Convert.ToDouble(DataSetTemp.Tables["Rates"].Rows[0]["OVERTIME1_RATE"]).ToString("000.00") + "%)";
                strOverTime2 = " (" + Convert.ToDouble(DataSetTemp.Tables["Rates"].Rows[0]["OVERTIME2_RATE"]).ToString("000.00") + "%)";
                strOverTime3 = " (" + Convert.ToDouble(DataSetTemp.Tables["Rates"].Rows[0]["OVERTIME3_RATE"]).ToString("000.00") + "%)";
            }

            DataSetTemp = null;

            for (int intRow = 0; intRow < CategoryDataSet.Tables["Earning"].Rows.Count; intRow++)
            {
                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PRINT_EARNINGS_DEDUCTIONS");
                strQry.AppendLine("(USER_NO");
                strQry.AppendLine(",COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EMPLOYEE_NO");

                if ((parstrDateOption == "M"
                || parstrDateOption == "Y"
                //2017-01-12
                || parstrDateOption == "B"
                || parstrDateOption == "O")
                && parstrConsolidateOption != "Y")
                {
                    strQry.AppendLine(",PAY_PERIOD_DATE");
                }

                strQry.AppendLine(",CATEGORY_DESC");
                strQry.AppendLine(",EARNING_AMOUNT)");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(parint64CurrentUserNo.ToString());
                strQry.AppendLine("," + parint64CompanyNo);

                if (parstrByPayCategoryOption == "Y")
                {
                    strQry.AppendLine(",EPCH.PAY_CATEGORY_NO");
                }
                else
                {
                    strQry.AppendLine(",-1 AS PAY_CATEGORY_NO");
                }

                strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EPCH.EMPLOYEE_NO");

                if ((parstrDateOption == "M"
                || parstrDateOption == "Y"
                //2017-01-12
                || parstrDateOption == "B"
                || parstrDateOption == "O")
                && parstrConsolidateOption != "Y")
                {
                    strQry.AppendLine(",EPCH.PAY_PERIOD_DATE");
                }

                strQry.AppendLine(",E.EARNING_DESC");
                strQry.AppendLine(",SUM(EEH.TOTAL)");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH ");

                //2013-08-29
                if (parstrCurrentUserAccess == "U")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                    strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                    strQry.AppendLine(" AND EPCH.COMPANY_NO = UEPCT.COMPANY_NO");
                    strQry.AppendLine(" AND EPCH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
                }
                
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH ");
                strQry.AppendLine(" ON EPCH.COMPANY_NO = EEH.COMPANY_NO ");
                strQry.AppendLine(" AND EPCH.PAY_PERIOD_DATE = EEH.PAY_PERIOD_DATE ");
                strQry.AppendLine(" AND EPCH.EMPLOYEE_NO = EEH.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPCH.RUN_TYPE = EEH.RUN_TYPE ");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = EEH.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = EEH.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EPCH.RUN_NO = EEH.RUN_NO ");
                
                if (parstrDateOption != "P")
                {
                    if (parstrWhere.IndexOf("EMPLOYEE_NO") == -1)
                    {
                        //2017-02-24 - Link to Employee for PAY_CATEGORY_TYPE
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE EMP");
                        strQry.AppendLine(" ON EEH.COMPANY_NO = EMP.COMPANY_NO ");
                        strQry.AppendLine(" AND EEH.EMPLOYEE_NO = EMP.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EMP.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                        strQry.AppendLine(" AND EMP.DATETIME_DELETE_RECORD IS NULL ");
                    }
                }
                
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING E ");
                strQry.AppendLine(" ON EEH.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND EEH.EARNING_NO = E.EARNING_NO");
                strQry.AppendLine(" AND EEH.EARNING_NO = " + CategoryDataSet.Tables["Earning"].Rows[intRow]["EARNING_NO"].ToString());
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

                //Add Where Clause
                //Add Where Clause
                strQry.AppendLine(parstrWhere);

                if (parstrPayCategoryNoIN != "")
                {
                    strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO IN (" + parstrPayCategoryNoIN + ")");
                }
  
                strQry.AppendLine(" GROUP BY ");

                if (parstrByPayCategoryOption == "Y")
                {
                    strQry.AppendLine(" EPCH.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE");
                }
                else
                {
                    strQry.AppendLine(" EPCH.PAY_CATEGORY_TYPE");
                }
                strQry.AppendLine(",EPCH.EMPLOYEE_NO");

                if ((parstrDateOption == "M"
                || parstrDateOption == "Y"
                //2017-01-12
                || parstrDateOption == "B"
                || parstrDateOption == "O")
                && parstrConsolidateOption != "Y")
                {
                    strQry.AppendLine(",EPCH.PAY_PERIOD_DATE");
                }

                strQry.AppendLine(",EEH.EARNING_NO");
                strQry.AppendLine(",E.EARNING_DESC");

                if (parstrEarningsSelectedIN == ""
                    & strTakeOn == "N")
                {
                    strQry.AppendLine(" HAVING SUM(EEH.TOTAL) > 0");
                }

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            }
            
            //Now do Deductions
            for (int intRow = 0; intRow < CategoryDataSet.Tables["Deduction"].Rows.Count; intRow++)
            {
                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PRINT_EARNINGS_DEDUCTIONS");
                strQry.AppendLine("(USER_NO");
                strQry.AppendLine(",COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EMPLOYEE_NO");

                if ((parstrDateOption == "M"
                || parstrDateOption == "Y"
                //2017-01-12
                || parstrDateOption == "B"
                || parstrDateOption == "O")
                && parstrConsolidateOption != "Y")
                {
                    strQry.AppendLine(",PAY_PERIOD_DATE");
                }

                strQry.AppendLine(",CATEGORY_DESC");
                strQry.AppendLine(",DEDUCTION_AMOUNT)");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(parint64CurrentUserNo.ToString());
                strQry.AppendLine("," + parint64CompanyNo);

                if (parstrByPayCategoryOption == "Y")
                {
                    strQry.AppendLine(",EPCH.PAY_CATEGORY_NO");
                }
                else
                {
                    strQry.AppendLine(",-1 AS PAY_CATEGORY_NO");
                }

                strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EPCH.EMPLOYEE_NO");

                if ((parstrDateOption == "M"
                || parstrDateOption == "Y"
                //2017-01-12
                || parstrDateOption == "B"
                || parstrDateOption == "O")
                && parstrConsolidateOption != "Y")
                {
                    strQry.AppendLine(",EPCH.PAY_PERIOD_DATE");
                }

                strQry.AppendLine(",D.DEDUCTION_DESC");
                strQry.AppendLine(",SUM(EDH.TOTAL)");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH ");

                //2013-08-29
                if (parstrCurrentUserAccess == "U")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                    strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                    strQry.AppendLine(" AND EPCH.COMPANY_NO = UEPCT.COMPANY_NO");
                    strQry.AppendLine(" AND EPCH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
                }

                //strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                //strQry.AppendLine(" ON EPC.COMPANY_NO = EPCH.COMPANY_NO");
                //strQry.AppendLine(" AND EPC.EMPLOYEE_NO = EPCH.EMPLOYEE_NO ");
                //strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = EPCH.PAY_CATEGORY_NO ");
                //strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = EPCH.PAY_CATEGORY_TYPE ");
                //strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y' ");
                //strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY EDH ");

                strQry.AppendLine(" ON EDH.COMPANY_NO = EPCH.COMPANY_NO");
                strQry.AppendLine(" AND EDH.PAY_PERIOD_DATE = EPCH.PAY_PERIOD_DATE ");
                strQry.AppendLine(" AND EDH.PAY_CATEGORY_TYPE = EPCH.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EDH.EMPLOYEE_NO = EPCH.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EDH.RUN_TYPE = EPCH.RUN_TYPE ");
                strQry.AppendLine(" AND EDH.RUN_NO = EPCH.RUN_NO ");

                if (parstrDateOption != "P")
                {
                    if (parstrWhere.IndexOf("EMPLOYEE_NO") == -1)
                    {
                        //2017-02-24 - Link to Employee for PAY_CATEGORY_TYPE
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE EMP");
                        strQry.AppendLine(" ON EDH.COMPANY_NO = EMP.COMPANY_NO ");
                        strQry.AppendLine(" AND EDH.EMPLOYEE_NO = EMP.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EMP.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                        strQry.AppendLine(" AND EMP.DATETIME_DELETE_RECORD IS NULL ");
                    }
                }

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.DEDUCTION D ");

                strQry.AppendLine(" ON EDH.COMPANY_NO = D.COMPANY_NO ");
                strQry.AppendLine(" AND EDH.PAY_CATEGORY_TYPE = D.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EDH.DEDUCTION_NO = D.DEDUCTION_NO");
                strQry.AppendLine(" AND EDH.DEDUCTION_SUB_ACCOUNT_NO = D.DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL ");

                //strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ED ");
                //strQry.AppendLine(" ON EDH.COMPANY_NO = ED.COMPANY_NO ");
                //strQry.AppendLine(" AND EDH.EMPLOYEE_NO = ED.EMPLOYEE_NO ");
                //strQry.AppendLine(" AND EDH.DEDUCTION_NO = ED.DEDUCTION_NO");
                //strQry.AppendLine(" AND EDH.DEDUCTION_SUB_ACCOUNT_NO = ED.DEDUCTION_SUB_ACCOUNT_NO");
                //strQry.AppendLine(" AND EDH.PAY_CATEGORY_TYPE = ED.PAY_CATEGORY_TYPE ");
                //strQry.AppendLine(" AND ED.DATETIME_DELETE_RECORD IS NULL");
                    
                strQry.AppendLine(" WHERE EPCH.COMPANY_NO = " + parint64CompanyNo);

                if (parstrPayCategoryNoIN != "")
                {
                    strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO IN (" + parstrPayCategoryNoIN + ")");
                }

                //Add Where Clause
                //Add Where Clause
                strQry.AppendLine(parstrDeductionWhere);

                strQry.AppendLine(" AND EDH.DEDUCTION_NO = " + CategoryDataSet.Tables["Deduction"].Rows[intRow]["DEDUCTION_NO"].ToString());

                strQry.AppendLine(" GROUP BY ");

                if (parstrByPayCategoryOption == "Y")
                {
                    strQry.AppendLine(" EPCH.PAY_CATEGORY_NO");
                    strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE");
                }
                else
                {
                    strQry.AppendLine(" EPCH.PAY_CATEGORY_TYPE");
                }
                strQry.AppendLine(",EPCH.EMPLOYEE_NO");

                if ((parstrDateOption == "M"
                || parstrDateOption == "Y"
                //2017-01-12
                || parstrDateOption == "B"
                || parstrDateOption == "O")
                && parstrConsolidateOption != "Y")
                {
                    strQry.AppendLine(",EPCH.PAY_PERIOD_DATE");
                }

                strQry.AppendLine(",EDH.DEDUCTION_NO");
                strQry.AppendLine(",D.DEDUCTION_DESC");

                if (parstrDeductionsSelectedIN == ""
                 & strTakeOn == "N")
                {
                    strQry.AppendLine(" HAVING SUM(EDH.TOTAL) <> 0");
                }

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            }
         
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" C.COMPANY_DESC");
            strQry.AppendLine(",CL.DATE_FORMAT");
            strQry.AppendLine(",'' AS REPORT_DATETIME");
            strQry.AppendLine(",'' AS REPORT_PAY_CATEGORY_HEADER");
            strQry.AppendLine(",'' AS REPORT_DATE_HEADER");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.COMPANY_LINK CL");
            strQry.AppendLine(" ON C.COMPANY_NO = CL.COMPANY_NO ");
           
            strQry.AppendLine(" WHERE C.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "ReportHeader", parint64CompanyNo);

            if (DataSet.Tables["ReportHeader"].Rows.Count > 0)
            {
                DataSet.Tables["ReportHeader"].Rows[0]["REPORT_DATETIME"] = "Printed   " + DateTime.Now.ToString(DataSet.Tables["ReportHeader"].Rows[0]["DATE_FORMAT"].ToString() + "   HH:mm");
            }
         
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",PED.CATEGORY_DESC");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            if (DataSet.Tables["ReportHeader"].Rows[0]["DATE_FORMAT"].ToString().ToUpper().Substring(0, 4) == "YYYY")
            {
                strQry.AppendLine(",CONVERT(VARCHAR(10),PED.PAY_PERIOD_DATE,120) AS PAY_PERIOD_DATE");
            }
            else
            {
                //dd-MMMM-yyyy
                strQry.AppendLine(",CONVERT(VARCHAR(10),PED.PAY_PERIOD_DATE,105) AS PAY_PERIOD_DATE");
            }

            strQry.AppendLine(",PED.PAY_PERIOD_DATE");

            //strQry.AppendLine(",PED.PAY_PERIOD_DATE");
            strQry.AppendLine(",SUM(PED.EARNING_AMOUNT) AS EARNING_AMOUNT");
            strQry.AppendLine(",SUM(PED.DEDUCTION_AMOUNT) AS DEDUCTION_AMOUNT");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PRINT_EARNINGS_DEDUCTIONS PED");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON PED.COMPANY_NO = E.COMPANY_NO");
            strQry.AppendLine(" AND PED.EMPLOYEE_NO = E.EMPLOYEE_NO");
            //2017-01-28 (Allow Employee to Change PAY_CATEGORY_TYPE)  
            //strQry.AppendLine(" AND PED.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            
            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON PED.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND PED.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PED.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" WHERE PED.USER_NO = " + parint64CurrentUserNo);
            strQry.AppendLine(" AND PED.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",PED.CATEGORY_DESC");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PED.PAY_PERIOD_DATE");

            strQry.AppendLine(" ORDER BY ");

            strQry.AppendLine(" E.EMPLOYEE_CODE");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PED.CATEGORY_DESC");
            strQry.AppendLine(",PED.PAY_PERIOD_DATE"); 

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Report", parint64CompanyNo);

            if (DataSet.Tables["Report"].Rows.Count > 0)
            {
                if (DataSet.Tables["Report"].Rows[0]["PAY_CATEGORY_DESC"].ToString() != "")
                {
                    DataSet.Tables["ReportHeader"].Rows[0]["REPORT_PAY_CATEGORY_HEADER"] = "Cost Centre";
                }

                if (DataSet.Tables["Report"].Rows[0]["PAY_PERIOD_DATE"].ToString() != "")
                {
                    DataSet.Tables["ReportHeader"].Rows[0]["REPORT_DATE_HEADER"] = "Date";
                }
            }

            //Testing
            //DataSet.Tables["Report"].Rows[0]["CATEGORY_DESC"] = "ZZZZZZZZZZZZZZZZZZZZZZZZZ");
            //DataSet.Tables["Report"].Rows[0]["PAY_CATEGORY_DESC"] = "ZZZZZZZZZZZZZZZZZZZZZZZZZ");
            //DataSet.Tables["Report"].Rows[0]["PAY_PERIOD_DATE"] = "2012-12-31");
            //DataSet.Tables["Report"].Rows[0]["EARNING_AMOUNT"] = 999999999.99;
            //DataSet.Tables["Report"].Rows[0]["DEDUCTION_AMOUNT"] = 999999999.99;
         
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Print_Horizontal_SpreadSheet_Page(Int64 parint64CompanyNo, Int64 parint64CurrentUserNo, int parintNumberOfPage, int parintNumberOfHorizontalPages)
        {
            StringBuilder strQry = new StringBuilder();
            string strQryAddon = "";
            string strQrySUMAddon = "";
            DataSet DataSet = new DataSet();

            int intFieldStartNo = 1;
            int intFieldStartNameNo = 1;

            if (parintNumberOfPage == 2)
            {
                intFieldStartNo = 8;
            }
            else
            {
                if (parintNumberOfPage == 3)
                {
                    intFieldStartNo = 18;
                }
                else
                {
                    if (parintNumberOfPage == 4)
                    {
                        intFieldStartNo = 28;
                    }
                }
            }

            //Create SQL Statement
            strQry.Clear();
            strQry.AppendLine(" SELECT ");

            for (int intFieldCount = intFieldStartNo; intFieldCount < intFieldStartNo + 10; intFieldCount++)
            {
                if (parintNumberOfPage == 1
                    & intFieldStartNameNo >= 9)
                {
                    strQry.AppendLine(",'' AS FIELD" + intFieldStartNameNo + "_1_NAME");
                    strQry.AppendLine(",'' AS FIELD" + intFieldStartNameNo + "_2_NAME");

                    strQryAddon += ",0 AS FIELD" + intFieldStartNameNo + "_AMOUNT";
                    strQrySUMAddon += ",0 AS FIELD" + intFieldStartNameNo + "_AMOUNT";
                }
                else
                {
                    strQryAddon += ",PS.FIELD" + intFieldCount + "_AMOUNT  AS FIELD" + intFieldStartNameNo + "_AMOUNT";
                    strQrySUMAddon += ",SUM(PS.FIELD" + intFieldCount + "_AMOUNT)  AS FIELD" + intFieldStartNameNo + "_AMOUNT";

                    if (intFieldStartNameNo == 1)
                    {
                        strQry.AppendLine(" PSH.FIELD" + intFieldCount + "_1_NAME AS FIELD" + intFieldStartNameNo + "_1_NAME");
                        strQry.AppendLine(",PSH.FIELD" + intFieldCount + "_2_NAME AS FIELD" + intFieldStartNameNo + "_2_NAME");
                    }
                    else
                    {
                        strQry.AppendLine(",PSH.FIELD" + intFieldCount + "_1_NAME AS FIELD" + intFieldStartNameNo + "_1_NAME");
                        strQry.AppendLine(",PSH.FIELD" + intFieldCount + "_2_NAME AS FIELD" + intFieldStartNameNo + "_2_NAME");
                    }
                }

                intFieldStartNameNo += 1;
            }

            strQry.AppendLine(",'Horizontal Page " + parintNumberOfPage + " of " + parintNumberOfHorizontalPages + "' AS HORIZONTAL_PAGE_NUMBER");

            strQry.AppendLine(",'' AS REPORT_DATETIME");
            strQry.AppendLine(",'' AS REPORT_DATE_HEADER");
            strQry.AppendLine(",CL.DATE_FORMAT");
            strQry.AppendLine(",C.COMPANY_DESC");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PRINT_SPREADSHEET_HEADER PSH");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C");
            strQry.AppendLine(" ON PSH.COMPANY_NO = C.COMPANY_NO");
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.COMPANY_LINK CL");
            strQry.AppendLine(" ON C.COMPANY_NO = CL.COMPANY_NO ");

            strQry.AppendLine(" WHERE PSH.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PSH.USER_NO = " + parint64CurrentUserNo);

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PrintHeader", parint64CompanyNo);

            if (DataSet.Tables["PrintHeader"].Rows.Count > 0)
            {
                DataSet.Tables["PrintHeader"].Rows[0]["REPORT_DATETIME"] = "Printed   " + DateTime.Now.ToString(DataSet.Tables["PrintHeader"].Rows[0]["DATE_FORMAT"].ToString() + "   HH:mm");
            }

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME + ' ' + E.EMPLOYEE_NAME AS EMPLOYEE_NAMES");
            strQry.AppendLine(",1 AS SORT_IND");

            strQry.AppendLine(strQryAddon);

            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            if (DataSet.Tables["PrintHeader"].Rows.Count > 0)
            {

                if (DataSet.Tables["PrintHeader"].Rows[0]["DATE_FORMAT"].ToString().ToUpper().Substring(0, 4) == "YYYY")
                {
                    strQry.AppendLine(",CONVERT(VARCHAR(10),PS.PAY_PERIOD_DATE,120) AS PAY_PERIOD_DATE");
                }
                else
                {
                    //dd-MMMM-yyyy
                    strQry.AppendLine(",CONVERT(VARCHAR(10),PS.PAY_PERIOD_DATE,105) AS PAY_PERIOD_DATE");
                }
            }
            else
            {
                strQry.AppendLine(",CONVERT(VARCHAR(10),PS.PAY_PERIOD_DATE,120) AS PAY_PERIOD_DATE");
            }

            strQry.AppendLine(",1 AS SORT_NUMBER");
       
            strQry.AppendLine(" FROM (((InteractPayroll_#CompanyNo#.dbo.COMPANY C ");
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PRINT_SPREADSHEET PS");
            strQry.AppendLine(" ON C.COMPANY_NO = PS.COMPANY_NO)");
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON C.COMPANY_NO = E.COMPANY_NO");
            strQry.AppendLine(" AND PS.EMPLOYEE_NO = E.EMPLOYEE_NO)");
            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON PS.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND PS.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PS.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE)");
            strQry.AppendLine(" WHERE C.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND PS.USER_NO = " + parint64CurrentUserNo);
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" ORDER BY ");

            strQry.AppendLine(" PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",PS.PAY_PERIOD_DATE");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PrintHorizontal", parint64CompanyNo);

            if (DataSet.Tables["PrintHorizontal"].Rows.Count > 0)
            {
                if (DataSet.Tables["PrintHorizontal"].Rows[0]["PAY_PERIOD_DATE"].ToString() != "")
                {
                    DataSet.Tables["PrintHeader"].Rows[0]["REPORT_DATE_HEADER"] = "Date";
                }
            }

            //DataSet.Tables["PrintHorizontal"].Rows[0]["EMPLOYEE_CODE"] = "ZZZZZZZZZZ");
           
            //DataSet.Tables["PrintHorizontal"].Rows[0]["FIELD1_AMOUNT"] = 999999999.99;

            //DataSet.Tables["PrintHorizontal"].Rows[0]["FIELD5_AMOUNT"] = System.DBNull.Value;

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet = null;
            return bytCompress;
        }
    }
}
