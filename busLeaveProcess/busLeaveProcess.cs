using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busLeaveProcess
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busLeaveProcess()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo,string parstrCurrentUserAccess, Int64 parint64CurrentUserNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
            
            strQry.Clear();
            strQry.AppendLine(" SELECT");

            if (parstrCurrentUserAccess == "S"
                |  parstrCurrentUserAccess == "A")
            {
                strQry.AppendLine(" 'A' AS ACCESS_IND");
            }
            else
            {
                strQry.AppendLine(" 'U' AS ACCESS_IND");

            }

            strQry.AppendLine(",C.WAGE_RUN_IND");
            strQry.AppendLine(",C.SALARY_RUN_IND");
            strQry.AppendLine(",MAX(PCPC.PAY_PERIOD_DATE) AS WAGE_PAY_PERIOD_DATE");
            strQry.AppendLine(",MAX(PCPC1.PAY_PERIOD_DATE) AS SALARY_PAY_PERIOD_DATE");
                
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");
            strQry.AppendLine(" ON C.COMPANY_NO = PCPC.COMPANY_NO");
            strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P'");
            strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = 'W'");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC1");
            strQry.AppendLine(" ON C.COMPANY_NO = PCPC1.COMPANY_NO");
            strQry.AppendLine(" AND PCPC1.RUN_TYPE = 'P'");
            strQry.AppendLine(" AND PCPC1.PAY_CATEGORY_TYPE = 'S'");

            strQry.AppendLine(" WHERE C.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");
                
            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" C.WAGE_RUN_IND");
            strQry.AppendLine(",C.SALARY_RUN_IND");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" LC.COMPANY_NO");
            strQry.AppendLine(",LC.EMPLOYEE_NO");
            strQry.AppendLine(",LC.EARNING_NO");
            strQry.AppendLine(",LC.LEAVE_REC_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",LC.LEAVE_DESC");
            strQry.AppendLine(",LC.PROCESS_NO");
            strQry.AppendLine(",LC.LEAVE_FROM_DATE");
            strQry.AppendLine(",LC.LEAVE_TO_DATE");

            //2013-09-11
            strQry.AppendLine(",ISNULL(LEAVE_TEMP.AUTHORISED_IND,'N') AS AUTHORISED_IND");

            strQry.AppendLine(",ISNULL(LC.LEAVE_OPTION,'D') AS LEAVE_OPTION");
            strQry.AppendLine(",ROUND(ISNULL(LC.LEAVE_HOURS_DECIMAL,0),2) AS LEAVE_HOURS_DECIMAL");
            strQry.AppendLine(",ROUND(ISNULL(LC.LEAVE_DAYS_DECIMAL,0),2) AS LEAVE_DAYS_DECIMAL");
            strQry.AppendLine(",DATEDIFF(d,LEAVE_FROM_DATE,LEAVE_TO_DATE ) + 1 AS DATE_DIFF_NO_DAYS");
          
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT LC");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON LC.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND LC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND LC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            //2017-01-24 (Employee Not Closed)
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
            //Default Pay Category Parameters Apply to Leave
            strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            //2013-08-29
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" LEFT JOIN ");

            strQry.AppendLine("(SELECT DISTINCT");
            strQry.AppendLine(" EPCLAC.EMPLOYEE_NO");
            strQry.AppendLine(",EPCLAC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EPCLAC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPCLAC.EARNING_NO");
            strQry.AppendLine(",EPCLAC.LEAVE_REC_NO");
            strQry.AppendLine(",EPCLAC.AUTHORISED_IND");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT EPCLAC");
           
            strQry.AppendLine(" WHERE EPCLAC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EPCLAC.AUTHORISED_IND = 'Y') AS LEAVE_TEMP ");

            strQry.AppendLine(" ON LC.EMPLOYEE_NO = LEAVE_TEMP.EMPLOYEE_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = LEAVE_TEMP.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = LEAVE_TEMP.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND LC.EARNING_NO = LEAVE_TEMP.EARNING_NO");
            strQry.AppendLine(" AND LC.LEAVE_REC_NO = LEAVE_TEMP.LEAVE_REC_NO");
         
            strQry.AppendLine(" WHERE LC.COMPANY_NO = " + parint64CompanyNo);
            
            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" LEAVE_FROM_DATE");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Leave", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" PROCESS_NO");
            strQry.AppendLine(",PROCESS_DESC");
            
            strQry.AppendLine(" FROM InteractPayroll.dbo.PROCESS ");
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PROCESS_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Process", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EARNING_DESC");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EARNING_NO >= 200");
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" EARNING_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "LeaveType", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            //Used For Leave Totals
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");

            strQry.AppendLine(",LS.LEAVE_PAID_ACCUMULATOR_IND");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
            //Default Pay Category Parameters Apply to Leave
            strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            //2013-09-11
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT LS");
            strQry.AppendLine(" ON E.COMPANY_NO = LS.COMPANY_NO");
            strQry.AppendLine(" AND E.LEAVE_SHIFT_NO = LS.LEAVE_SHIFT_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = LS.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND LS.DATETIME_DELETE_RECORD IS NULL");
        
            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            //Removed 2012-04-06 - Allow For Employees Clocked on Client That have Not been Activated to be seen
            //strQry.AppendLine(" AND EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" E.EMPLOYEE_CODE");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" PAY_CATEGORY_NO");
            strQry.AppendLine(",DAY_NO");
            strQry.AppendLine(",TIME_DECIMAL");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_TIME_DECIMAL");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" PAY_CATEGORY_NO");
            strQry.AppendLine(",DAY_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategoryTimeDecimal", parint64CompanyNo);

            DateTime dtDateNowAYearAgo = DateTime.Now.AddYears(-1);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" PUBLIC_HOLIDAY_DATE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY PH ");

            strQry.AppendLine(" WHERE  PUBLIC_HOLIDAY_DATE > '" + dtDateNowAYearAgo.ToString("yyyy-MM-dd") + "'");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PublicHoliday", parint64CompanyNo);

            DateTime dtStartTaxYear;

            if (DateTime.Now.Month > 2)
            {
                dtStartTaxYear = new DateTime(DateTime.Now.Year, 3, 1);
            }
            else
            {
                dtStartTaxYear = new DateTime(DateTime.Now.Year - 1, 3, 1);
            }
            
            strQry.Clear();

            //Normal Leave And Sick Leave
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" LH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",LH.EMPLOYEE_NO");
            strQry.AppendLine(",LH.EARNING_NO");
            strQry.AppendLine(",ROUND(SUM(LH.LEAVE_ACCUM_DAYS - LH.LEAVE_PAID_DAYS),2) AS TOTAL_LEAVE_DAYS");
        
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY LH");
      
            //2013-09-10
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(SELECT DISTINCT");
                strQry.AppendLine(" EMPLOYEE_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP ");
                strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND COMPANY_NO = " + parint64CompanyNo + ") AS USER_TABLE");

                strQry.AppendLine(" ON LH.EMPLOYEE_NO = USER_TABLE.EMPLOYEE_NO");
                strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = USER_TABLE.PAY_CATEGORY_TYPE");
            }
           
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON LH.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND LH.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            //2017-01-24 (Employee Not Closed)
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

            strQry.AppendLine(" WHERE LH.COMPANY_NO = " + parint64CompanyNo);

            strQry.AppendLine(" AND (LH.EARNING_NO = 200");
            strQry.AppendLine(" OR (LH.EARNING_NO = 201");
            strQry.AppendLine(" AND LH.PAY_PERIOD_DATE >= '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'))");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" LH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",LH.EMPLOYEE_NO");
            strQry.AppendLine(",LH.EARNING_NO");
                                
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeLeaveTotals", parint64CompanyNo);
                        
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public int Update_Record(string parstrCurrentUserAccess, Int64 parint64CurrentUserNo, string parstrPayrollType, byte[] parbyteDataSet, Int64 parint64CompanyNo)
        {
            int intReturnCode = 0;
            DataSet DataSet = new DataSet();
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

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
            }

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyCheck", parint64CompanyNo);

            if (DataSet.Tables["CompanyCheck"].Rows[0]["RUN_IND"].ToString() == "")
            {
                for (int intRow = 0; intRow < parDataSet.Tables["Leave"].Rows.Count; intRow++)
                {
                    if (parDataSet.Tables["Leave"].Rows[intRow].RowState == DataRowState.Deleted)
                    {
                        strQry.Clear();
                        strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT");
                        strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["COMPANY_NO", System.Data.DataRowVersion.Original]));
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["EMPLOYEE_NO", System.Data.DataRowVersion.Original]));
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Leave"].Rows[intRow]["PAY_CATEGORY_TYPE", System.Data.DataRowVersion.Original].ToString()));
                        strQry.AppendLine(" AND EARNING_NO = " + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["EARNING_NO", System.Data.DataRowVersion.Original]));
                        strQry.AppendLine(" AND LEAVE_REC_NO = " + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["LEAVE_REC_NO", System.Data.DataRowVersion.Original]));

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                        strQry.Clear();
                        strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT");
                        strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["COMPANY_NO", System.Data.DataRowVersion.Original]));
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["EMPLOYEE_NO", System.Data.DataRowVersion.Original]));
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Leave"].Rows[intRow]["PAY_CATEGORY_TYPE", System.Data.DataRowVersion.Original].ToString()));
                        strQry.AppendLine(" AND EARNING_NO = " + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["EARNING_NO", System.Data.DataRowVersion.Original]));
                        strQry.AppendLine(" AND LEAVE_REC_NO = " + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["LEAVE_REC_NO", System.Data.DataRowVersion.Original]));

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                    }
                    else
                    {
                        strQry.Clear();
                        strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT");
                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" PROCESS_NO = " + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["PROCESS_NO"]));
                        strQry.AppendLine(",LEAVE_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Leave"].Rows[intRow]["LEAVE_DESC"].ToString()));
                        strQry.AppendLine(",LEAVE_FROM_DATE = '" + Convert.ToDateTime(parDataSet.Tables["Leave"].Rows[intRow]["LEAVE_FROM_DATE"]).ToString("yyyy-MM-dd") + "'");
                        strQry.AppendLine(",LEAVE_TO_DATE = '" + Convert.ToDateTime(parDataSet.Tables["Leave"].Rows[intRow]["LEAVE_TO_DATE"]).ToString("yyyy-MM-dd") + "'");
                        strQry.AppendLine(",LEAVE_DAYS_DECIMAL = " + Convert.ToDouble(parDataSet.Tables["Leave"].Rows[intRow]["LEAVE_DAYS_DECIMAL"]));
                        strQry.AppendLine(",LEAVE_OPTION = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Leave"].Rows[intRow]["LEAVE_OPTION"].ToString()));
                                 
                        if (parDataSet.Tables["Leave"].Rows[intRow]["LEAVE_HOURS_DECIMAL"] != System.DBNull.Value)
                        {
                            strQry.AppendLine(",LEAVE_HOURS_DECIMAL = " + Convert.ToDouble(parDataSet.Tables["Leave"].Rows[intRow]["LEAVE_HOURS_DECIMAL"]));
                        }

                        strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["COMPANY_NO"]));
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["EMPLOYEE_NO"]));
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Leave"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                        strQry.AppendLine(" AND EARNING_NO = " + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["EARNING_NO"]));
                        strQry.AppendLine(" AND LEAVE_REC_NO = " + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["LEAVE_REC_NO"]));

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                        if (Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["PROCESS_NO"]) == 0)
                        {
                            //Insert
                            strQry.Clear();
                            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT");
                            strQry.AppendLine("(COMPANY_NO");
                            strQry.AppendLine(",EMPLOYEE_NO");
                            strQry.AppendLine(",PAY_CATEGORY_NO");
                            strQry.AppendLine(",PAY_CATEGORY_TYPE");
                            strQry.AppendLine(",EARNING_NO");
                            strQry.AppendLine(",LEVEL_NO");
                            strQry.AppendLine(",LEAVE_REC_NO");
                            strQry.AppendLine(",USER_NO");
                            strQry.AppendLine(",AUTHORISED_IND)");

                            strQry.AppendLine(" SELECT DISTINCT");
                            strQry.AppendLine(" EPC.COMPANY_NO");
                            strQry.AppendLine(",EPC.EMPLOYEE_NO");
                            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
                            strQry.AppendLine("," + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["EARNING_NO"]));
                            strQry.AppendLine(",PCA.LEVEL_NO");
                            strQry.AppendLine("," + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["LEAVE_REC_NO"]));
                            strQry.AppendLine(",PCA.USER_NO");

                            strQry.AppendLine(",'N'");
                            
                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");

                            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                            strQry.AppendLine(" ON EPC.COMPANY_NO = E.COMPANY_NO");
                            strQry.AppendLine(" AND EPC.EMPLOYEE_NO = E.EMPLOYEE_NO");
                            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                            //Employee Has Been Activates (Taken-On)
                            strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                            //Employee NOT Closed
                            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                            
                            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_AUTHORISE PCA");
                            strQry.AppendLine(" ON EPC.COMPANY_NO = PCA.COMPANY_NO");
                            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PCA.PAY_CATEGORY_NO");
                            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PCA.PAY_CATEGORY_TYPE");
                            //T = Timesheet, L = Leave
                            strQry.AppendLine(" AND PCA.AUTHORISE_TYPE_IND = 'L'");
                            strQry.AppendLine(" AND PCA.DATETIME_DELETE_RECORD IS NULL");

                            if (parstrCurrentUserAccess == "U")
                            {
                                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                                strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
                            }
                            
                            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT EPCLAC");
                            strQry.AppendLine(" ON EPC.COMPANY_NO = EPCLAC.COMPANY_NO");
                            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = EPCLAC.PAY_CATEGORY_NO");
                            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = EPCLAC.PAY_CATEGORY_TYPE");
                            strQry.AppendLine(" AND EPCLAC.EARNING_NO = " + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["EARNING_NO"]));
                            strQry.AppendLine(" AND PCA.LEVEL_NO = EPCLAC.LEVEL_NO");
                            strQry.AppendLine(" AND EPCLAC.LEAVE_REC_NO = " + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["LEAVE_REC_NO"]));
                            strQry.AppendLine(" AND EPCLAC.USER_NO = "  + parint64CurrentUserNo.ToString());

                            strQry.AppendLine(" WHERE EPC.COMPANY_NO = " + parint64CompanyNo);
                            strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["EMPLOYEE_NO"]));

                            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                            //Default is Link for LEAVE
                            strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
                            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                            //No Record Currently Exists
                            strQry.AppendLine(" AND EPCLAC.COMPANY_NO IS NULL");

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                        }
                        else
                        {
                            strQry.Clear();
                            strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT");
                            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["Leave"].Rows[intRow]["COMPANY_NO"].ToString());
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + parDataSet.Tables["Leave"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parDataSet.Tables["Leave"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Leave"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                            strQry.AppendLine(" AND EARNING_NO = " + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["EARNING_NO"]));
                            strQry.AppendLine(" AND LEAVE_REC_NO = " + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["LEAVE_REC_NO"]));

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                        }
                    }
                }
            }
            else
            {
                intReturnCode = 1;
            }

            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            DataSet.Dispose();
            DataSet = null;

            return intReturnCode;
        }

        public int Delete_Record(string parstrCurrentUserAccess, Int64 parint64CurrentUserNo, string parstrPayrollType, byte[] parbyteDataSet, Int64 parint64CompanyNo)
        {
            int intReturnCode = 0;
            DataSet DataSet = new DataSet();
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

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
            }

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyCheck", parint64CompanyNo);

            if (DataSet.Tables["CompanyCheck"].Rows[0]["RUN_IND"].ToString() == "")
            {
                for (int intRow = 0; intRow < parDataSet.Tables["Leave"].Rows.Count; intRow++)
                {
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["COMPANY_NO"]));
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["EMPLOYEE_NO"]));
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Leave"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine(" AND EARNING_NO = " + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["EARNING_NO"]));
                    strQry.AppendLine(" AND LEAVE_REC_NO = " + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["LEAVE_REC_NO"]));

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["COMPANY_NO"]));
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["EMPLOYEE_NO"]));
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Leave"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine(" AND EARNING_NO = " + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["EARNING_NO"]));
                    strQry.AppendLine(" AND LEAVE_REC_NO = " + Convert.ToInt32(parDataSet.Tables["Leave"].Rows[intRow]["LEAVE_REC_NO"]));

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                    
                }
            }
            else
            {
                intReturnCode = 1;
            }

            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            DataSet.Dispose();
            DataSet = null;

            return intReturnCode;
        }
   
        public byte[] Insert_Leave_Records(string parstrCurrentUserAccess, Int64 parint64CurrentUserNo, Int64 parint64CompanyNo, string parstrEmployeeNos, string parstrEmployeePayCategoryNos, Int16 parint16LeaveType, string parstrPayrollType, string parstrLeaveDescription, Int16 parint16ProcessNo, string parstrOption, DateTime pardtFromDate, DateTime pardtToDate, string parstrPortionOfDay)
        {
            string[] strEmployeeNo = parstrEmployeeNos.Split('|');
            string[] strEmployeePayCategoryNo = parstrEmployeePayCategoryNos.Split('|');
            DataSet DataSet = new DataSet();
            DataRow dtDataRow;
            DataView PayCategoryTimeDecimalDataView = null;
            DataView PublicHolidayDataView = null;
            DateTime dtDateTimeFrom;
            int intFindPayCategoryTimeDecimalRow = -1;
            int intFindPublicHolidayRow = -1;

            double dblLeavePaidDays = 0;
            double dblLeaveHoursDecimal = 0;

            DateTime dtDateNowAYearAgo = DateTime.Now.AddYears(-1);

            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" SELECT");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" WAGE_RUN_IND AS RUN_IND");
            }
            else
            {
                strQry.AppendLine(" SALARY_RUN_IND AS RUN_IND ");
            }

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyCheck", parint64CompanyNo);

            if (DataSet.Tables["CompanyCheck"].Rows[0]["RUN_IND"].ToString() == "")
            {
                strQry.Clear();
                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" PUBLIC_HOLIDAY_DATE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY PH ");

                strQry.AppendLine(" WHERE  PUBLIC_HOLIDAY_DATE > '" + dtDateNowAYearAgo.ToString("yyyy-MM-dd") + "'");
                
                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PublicHoliday", parint64CompanyNo);
                
                PublicHolidayDataView = null;
                PublicHolidayDataView = new DataView(DataSet.Tables["PublicHoliday"],
                "",
                "PUBLIC_HOLIDAY_DATE",
                DataViewRowState.CurrentRows);
                
                strQry.Clear();
                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" PAY_CATEGORY_NO");
                strQry.AppendLine(",DAY_NO");
                strQry.AppendLine(",TIME_DECIMAL");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_TIME_DECIMAL");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                strQry.AppendLine(" ORDER BY");
                strQry.AppendLine(" PAY_CATEGORY_NO");
                strQry.AppendLine(",DAY_NO");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategoryTimeDecimal", parint64CompanyNo);
                
                //Used To Merge Records on Client Machine
                strQry.Clear();
                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" COMPANY_NO");
                strQry.AppendLine(",EMPLOYEE_NO");
                strQry.AppendLine(",EARNING_NO");
                strQry.AppendLine(",LEAVE_REC_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",LEAVE_DESC");
                strQry.AppendLine(",PROCESS_NO");
                strQry.AppendLine(",LEAVE_FROM_DATE");
                strQry.AppendLine(",LEAVE_TO_DATE");
                
                strQry.AppendLine(",LEAVE_OPTION");
                strQry.AppendLine(",ROUND(LEAVE_HOURS_DECIMAL,2) AS LEAVE_HOURS_DECIMAL");

                strQry.AppendLine(",ROUND(LEAVE_DAYS_DECIMAL,2) AS LEAVE_DAYS_DECIMAL");
                strQry.AppendLine(",DATEDIFF(d,LEAVE_FROM_DATE,LEAVE_TO_DATE ) + 1 AS DATE_DIFF_NO_DAYS");
                                
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT ");
                //Empty DataTable
                strQry.AppendLine(" WHERE COMPANY_NO = -1");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Leave", parint64CompanyNo);

                for (int intRow = 0; intRow < strEmployeeNo.Length; intRow++)
                {
                    if (DataSet.Tables["Temp"] != null)
                    {
                        DataSet.Tables["Temp"].Clear();
                    }

                    dblLeavePaidDays = 0;
                    dblLeaveHoursDecimal = 0;
                    
                    dtDateTimeFrom = pardtFromDate;

                    PayCategoryTimeDecimalDataView = null;
                    PayCategoryTimeDecimalDataView = new DataView(DataSet.Tables["PayCategoryTimeDecimal"],
                    "PAY_CATEGORY_NO = " + strEmployeePayCategoryNo[intRow].ToString(),
                    "DAY_NO",
                    DataViewRowState.CurrentRows);

                    if (parstrOption == "P"
                    ||  parstrOption == "Z")
                    {
                        //Pay Out All Normal Leave Due (Wages Only) or Zerorize Balance
                        pardtFromDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                        pardtToDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        if (parstrOption == "D")
                        {
                            while (dtDateTimeFrom <= pardtToDate)
                            {
                                intFindPayCategoryTimeDecimalRow = PayCategoryTimeDecimalDataView.Find(Convert.ToInt32(dtDateTimeFrom.DayOfWeek));

                                if (intFindPayCategoryTimeDecimalRow > -1)
                                {
                                    intFindPublicHolidayRow = PublicHolidayDataView.Find(dtDateTimeFrom.ToString("yyyy-MM-dd"));

                                    if (intFindPublicHolidayRow == -1)
                                    {
                                        dblLeavePaidDays += 1;
                                        dblLeaveHoursDecimal += Math.Round(Convert.ToDouble(PayCategoryTimeDecimalDataView[intFindPayCategoryTimeDecimalRow]["TIME_DECIMAL"]), 2, MidpointRounding.AwayFromZero);
                                    }
                                }

                                dtDateTimeFrom = dtDateTimeFrom.AddDays(1);
                            }
                        }
                        else
                        {
                            intFindPayCategoryTimeDecimalRow = PayCategoryTimeDecimalDataView.Find(Convert.ToInt32(dtDateTimeFrom.DayOfWeek));

                            if (intFindPayCategoryTimeDecimalRow > -1)
                            {
                                intFindPublicHolidayRow = PublicHolidayDataView.Find(dtDateTimeFrom.ToString("yyyy-MM-dd"));

                                if (intFindPublicHolidayRow == -1)
                                {
                                    dblLeavePaidDays = Math.Round(Convert.ToDouble(parstrPortionOfDay) / Convert.ToDouble(PayCategoryTimeDecimalDataView[intFindPayCategoryTimeDecimalRow]["TIME_DECIMAL"]), 2, MidpointRounding.AwayFromZero);
                                }
                            }

                            dblLeaveHoursDecimal = Math.Round(Convert.ToDouble(parstrPortionOfDay), 2, MidpointRounding.AwayFromZero);
                        }
                    }

                    strQry.Clear();
                    strQry.AppendLine("INSERT INTO InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT ");
                    strQry.AppendLine("(COMPANY_NO ");
                    strQry.AppendLine(",EMPLOYEE_NO ");
                    strQry.AppendLine(",EARNING_NO ");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE ");
                   
                    strQry.AppendLine(",LEAVE_OPTION ");

                    strQry.AppendLine(",LEAVE_DESC ");
                    strQry.AppendLine(",PROCESS_NO ");
                    strQry.AppendLine(",LEAVE_FROM_DATE ");
                    strQry.AppendLine(",LEAVE_TO_DATE ");

                    strQry.AppendLine(",LEAVE_HOURS_DECIMAL ");
              
                    strQry.AppendLine(",LEAVE_DAYS_DECIMAL) ");
                    strQry.AppendLine(" VALUES ");
                    strQry.AppendLine("(" + parint64CompanyNo);
                    strQry.AppendLine("," + strEmployeeNo[intRow]);
                    strQry.AppendLine("," + parint16LeaveType);
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                   
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrOption));

                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrLeaveDescription));
                    strQry.AppendLine("," + parint16ProcessNo);
                    
                    strQry.AppendLine(",'" + Convert.ToDateTime(pardtFromDate).ToString("yyyy-MM-dd") + "'");
                    
                    if (parstrOption == "P")
                    {
                        //Pay Out All Normal Leave Due (Wages Only)
                        strQry.AppendLine(",'" + Convert.ToDateTime(pardtToDate).ToString("yyyy-MM-dd") + "'");
                    }
                    else
                    {
                        if (parstrOption == "D")
                        {
                            strQry.AppendLine(",'" + Convert.ToDateTime(pardtToDate).ToString("yyyy-MM-dd") + "'");
                        }
                        else
                        {
                            strQry.AppendLine(",'" + Convert.ToDateTime(pardtFromDate).ToString("yyyy-MM-dd") + "'");
                        }
                    }

                    strQry.AppendLine("," + dblLeaveHoursDecimal);
                    strQry.AppendLine("," + dblLeavePaidDays + ")");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                    
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" MAX(LEAVE_REC_NO) AS MAX_LEAVE_REC_NO ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT ");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parint64CompanyNo);

                    dtDataRow = DataSet.Tables["Leave"].NewRow();

                    dtDataRow["COMPANY_NO"] = parint64CompanyNo;
                    dtDataRow["EMPLOYEE_NO"] = strEmployeeNo[intRow];
                    dtDataRow["EARNING_NO"] = parint16LeaveType;
                    dtDataRow["LEAVE_REC_NO"] = DataSet.Tables["Temp"].Rows[0]["MAX_LEAVE_REC_NO"].ToString();

                    dtDataRow["PAY_CATEGORY_TYPE"] = parstrPayrollType;
                    dtDataRow["LEAVE_DESC"] = parstrLeaveDescription;
                    dtDataRow["PROCESS_NO"] = parint16ProcessNo;

                    dtDataRow["LEAVE_OPTION"] = parstrOption;

                    dtDataRow["LEAVE_FROM_DATE"] = pardtFromDate;

                    if (parstrOption == "D")
                    {
                        dtDataRow["LEAVE_TO_DATE"] = pardtToDate;
                    }
                    else
                    {
                        dtDataRow["LEAVE_TO_DATE"] = pardtFromDate;
                    }

                    dtDataRow["DATE_DIFF_NO_DAYS"] = pardtToDate.Subtract(pardtFromDate).Days + 1;
                    dtDataRow["LEAVE_DAYS_DECIMAL"] = dblLeavePaidDays;
                    dtDataRow["LEAVE_HOURS_DECIMAL"] = dblLeaveHoursDecimal;

                    DataSet.Tables["Leave"].Rows.Add(dtDataRow);
                   
                    //Next Run
                    if (parint16ProcessNo == 0)
                    {
                        strQry.Clear();
                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT");
                        strQry.AppendLine("(COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",PAY_CATEGORY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",EARNING_NO");
                        strQry.AppendLine(",LEVEL_NO");
                        strQry.AppendLine(",LEAVE_REC_NO");
                        strQry.AppendLine(",USER_NO)");

                        strQry.AppendLine(" SELECT DISTINCT");
                        strQry.AppendLine(" EPC.COMPANY_NO");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
                        strQry.AppendLine("," + parint16LeaveType);
                        strQry.AppendLine(",PCA.LEVEL_NO");
                        strQry.AppendLine("," + DataSet.Tables["Temp"].Rows[0]["MAX_LEAVE_REC_NO"].ToString());
                        strQry.AppendLine(",PCA.USER_NO");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                        strQry.AppendLine(" ON EPC.COMPANY_NO = E.COMPANY_NO");
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = E.EMPLOYEE_NO");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
                        //Employee Has Been Activates (Taken-On)
                        strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                        //Employee NOT Closed
                        strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                        strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_AUTHORISE PCA");
                        strQry.AppendLine(" ON EPC.COMPANY_NO = PCA.COMPANY_NO");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PCA.PAY_CATEGORY_NO");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PCA.PAY_CATEGORY_TYPE");
                        //T = Timesheet, L = Leave
                        strQry.AppendLine(" AND PCA.AUTHORISE_TYPE_IND = 'L'");
                        strQry.AppendLine(" AND PCA.DATETIME_DELETE_RECORD IS NULL");


                        if (parstrCurrentUserAccess == "U")
                        {
                            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                            strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                            strQry.AppendLine(" AND EPC.COMPANY_NO = UEPCT.COMPANY_NO");
                            strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
                        }

                        strQry.AppendLine(" WHERE EPC.COMPANY_NO = " + parint64CompanyNo);
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = " + strEmployeeNo[intRow]);

                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                        strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
                        strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                    }
                }
            }

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
