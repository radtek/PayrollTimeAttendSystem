using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busPayrollMain
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busPayrollMain()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public void Update_User_Last_Company_No(Int64 parint64CurrentUserNo, Int64 parInt64CompanyNo)
        {
            string strQry = "";

            strQry = "";
            strQry += " UPDATE InteractPayroll.dbo.USER_ID ";
            strQry += " SET LAST_COMPANY_NO = " + parInt64CompanyNo;
            strQry += " WHERE USER_NO = " + parint64CurrentUserNo;

            clsDBConnectionObjects.Execute_SQLCommand(strQry, parInt64CompanyNo);
        }

        public void Update_User_Last_Pay_Category_Type(Int64 parint64CurrentUserNo, string parstrLastPayCategoryType)
        {
            string strQry = "";

            strQry = "";
            strQry += " UPDATE InteractPayroll.dbo.USER_ID ";
            strQry += " SET LAST_PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLastPayCategoryType);
            strQry += " WHERE USER_NO = " + parint64CurrentUserNo;

            clsDBConnectionObjects.Execute_SQLCommand(strQry, -1);
        }

        public string Check_If_Busy_With_Run(Int64 parInt64CompanyNo,string parstrFromProgram)
        {
            string strPayrollRun = "N";
            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            strQry.AppendLine(" AND RUN_TYPE = 'P'");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
            }

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

            if (DataSet.Tables["Temp"].Rows.Count > 0)
            {
                strPayrollRun = "Y";
            }

            return strPayrollRun;
        }

        public string Check_If_TimeAtendance_Has_Been_Run(Int64 parInt64CompanyNo, string parstrFromProgram)
        {
            string strTimeAttendanceRun = "N";

            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND TIME_ATTENDANCE_RUN_IND = 'Y'");
            }
            else
            {
                strQry.AppendLine(" AND WAGE_RUN_IND = 'Y'");
            }

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);
            
            if (DataSet.Tables["Temp"].Rows.Count > 0)
            {
                strTimeAttendanceRun = "Y";
            }

            return strTimeAttendanceRun;
        }

        public string Check_If_Busy_Wages_Salary_Run(Int64 parInt64CompanyNo)
        {
            string strBusyWagesSalaryRun = "N";

            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new DataSet();

            strQry.Clear();
            
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND (WAGE_RUN_IND = 'Y'");
            strQry.AppendLine(" OR SALARY_RUN_IND = 'Y')");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

            if (DataSet.Tables["Temp"].Rows.Count > 0)
            {
                strBusyWagesSalaryRun = "Y";
            }

            return strBusyWagesSalaryRun;
        }

        public string Check_If_Run_Completed(Int64 parInt64CompanyNo,string parstrFromProgram)
        {
            string strRunCompleted = "N";
            
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" COMPLETED_IND = ");

            strQry.AppendLine(" CASE ");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" WHEN ISNULL(TIME_ATTENDANCE_RUN_IND,'N') = 'Y' THEN 'Y'");
                strQry.AppendLine(" ELSE 'N'");
            }
            else
            {
                strQry.AppendLine(" WHEN ISNULL(WAGE_RUN_IND,'N') = 'Y' THEN 'Y'");
                strQry.AppendLine(" ELSE 'N'");
            }

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC ");
            strQry.AppendLine(" ON C.COMPANY_NO = PCPC.COMPANY_NO");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = 'W'");
            }

            strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P'");

            strQry.AppendLine(" WHERE C.COMPANY_NO = " + parInt64CompanyNo);

            if (parstrFromProgram != "X")
            {
                strQry.AppendLine(" UNION ");

                strQry.AppendLine(" SELECT DISTINCT ");
                strQry.AppendLine(" COMPLETED_IND = ");

                strQry.AppendLine(" CASE ");


                strQry.AppendLine(" WHEN ISNULL(SALARY_RUN_IND,'N') = 'Y' THEN 'Y'");
                strQry.AppendLine(" ELSE 'N'");

                strQry.AppendLine(" END ");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC ");
                strQry.AppendLine(" ON C.COMPANY_NO = PCPC.COMPANY_NO");

                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = 'S'");

                strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P'");

                strQry.AppendLine(" WHERE C.COMPANY_NO = " + parInt64CompanyNo);
            }

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

            if (DataSet.Tables["Temp"].Rows.Count > 0)
            {
                strRunCompleted = "Y";
            }

            return strRunCompleted;
        }

        public string Check_Date_Loads()
        {
            string strCurrentFlags = "";

            string strQry = "";
            DataSet DataSet = new DataSet();

            //For Current Financual Year
            strQry = "";
            strQry += " SELECT ";
            strQry += " DAY_DATE";
            strQry += " FROM InteractPayroll.dbo.DATES";
            strQry += " WHERE DAY_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd") + "'";

            clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "Temp", -1);

            if (DataSet.Tables["Temp"].Rows.Count == 0)
            {
                strCurrentFlags = "Y";
            }
            else
            {
                strCurrentFlags = "N";
            }

            DataSet.Dispose();
            DataSet = null;

            //For Next Financual Year
            if (DateTime.Now.Month == 1
                | DateTime.Now.Month == 2)
            {
                strQry = "";
                strQry += " SELECT DAY_DATE";
                strQry += " FROM InteractPayroll.dbo.DATES";
                strQry += " WHERE DAY_DATE = '" + DateTime.Now.AddMonths(3).ToString("yyyy-MM-dd") + "'";

                DataSet = new DataSet();

                clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "Temp", -1);

                if (DataSet.Tables["Temp"].Rows.Count == 0)
                {
                    strCurrentFlags += "|Y";
                }
                else
                {
                    strCurrentFlags += "|N";
                }

                strQry = "";
                strQry += " SELECT MAX(PUBLIC_HOLIDAY_DATE) AS MAX_PUBLIC_HOLIDAY_DATE";
                strQry += " FROM InteractPayroll.dbo.PUBLIC_HOLIDAY";

                clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "PaidHoliday", -1);

                if (DataSet.Tables["PaidHoliday"].Rows.Count > 0)
                {
                    if (DataSet.Tables["PaidHoliday"].Rows[0]["MAX_PUBLIC_HOLIDAY_DATE"] != System.DBNull.Value)
                    {
                        DateTime dteDateTime = new DateTime(DateTime.Now.Year, 3, 1);

                        if (Convert.ToDateTime(DataSet.Tables["PaidHoliday"].Rows[0]["MAX_PUBLIC_HOLIDAY_DATE"]) < dteDateTime)
                        {
                            strCurrentFlags += "|Y";
                        }
                        else
                        {
                            strCurrentFlags += "|N";
                        }
                    }
                    else
                    {
                        strCurrentFlags += "|N";
                    }
                }
                else
                {
                    strCurrentFlags += "|N";
                }
            }
            else
            {
                strCurrentFlags += "|N|N";
            }

            return strCurrentFlags;
        }

        public int Check_WageRun(Int64 parint64CompanyNo, string parstrWageRunType)
        {
            int intReturnCode = 0;

            string strQry = "";
            DataSet DataSet = new DataSet();

            strQry = "";
            strQry += " SELECT ";
            strQry += " PCPC.COMPANY_NO";

            strQry += " FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC";

            strQry += " INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C";
            strQry += " ON PCPC.COMPANY_NO = C.COMPANY_NO ";

            if (parstrWageRunType == "P")
            {
                strQry += " AND (C.WAGE_RUN_IND = 'Y' OR C.SALARY_RUN_IND = 'Y')";
            }

            strQry += " WHERE PCPC.COMPANY_NO = " + parint64CompanyNo;
            strQry += " AND PCPC.RUN_TYPE = '" + parstrWageRunType + "'";

            clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "Temp", parint64CompanyNo);

            if (DataSet.Tables["Temp"].Rows.Count == 0)
            {
                intReturnCode = 1;
            }

            DataSet.Dispose();
            DataSet = null;

            return intReturnCode;
        }
        
        public int Check_TakeOnActivate(Int64 parint64CompanyNo)
        {
            int intReturnCode = 0;

            string strQry = "";
            DataSet DataSet = new DataSet();

            strQry = "";
            strQry += " SELECT ";
            strQry += " COMPANY_NO";

            strQry += " FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT";

            strQry += " WHERE COMPANY_NO = " + parint64CompanyNo;
            strQry += " AND RUN_TYPE = 'T'";

            clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "Temp", parint64CompanyNo);

            if (DataSet.Tables["Temp"].Rows.Count == 0)
            {
                intReturnCode = 1;
            }

            DataSet.Dispose();
            DataSet = null;

            return intReturnCode;
        }
    }
}
