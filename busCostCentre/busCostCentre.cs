using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace InteractPayroll
{
    public class busCostCentre
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busCostCentre()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo, string parstrTimeAttendInd, string parstrCurrentUserAccess, Int64 parint64CurrentUserNo)
        {
            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" C.OVERTIME1_RATE");
            strQry.AppendLine(",C.OVERTIME2_RATE");
            strQry.AppendLine(",C.OVERTIME3_RATE");
            strQry.AppendLine(",C.SALARY_OVERTIME1_RATE");
            strQry.AppendLine(",C.SALARY_OVERTIME2_RATE");
            strQry.AppendLine(",C.SALARY_OVERTIME3_RATE");
            strQry.AppendLine(",CL.DYNAMIC_UPLOAD_KEY");

            strQry.AppendLine(",'A' AS ACCESS_IND");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.COMPANY_LINK CL ");
            strQry.AppendLine(" ON C.COMPANY_NO = CL.COMPANY_NO ");

            if (parstrCurrentUserAccess == "A")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA ");

                strQry.AppendLine(" ON C.COMPANY_NO = UCA.COMPANY_NO ");
                strQry.AppendLine(" AND UCA.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND UCA.COMPANY_ACCESS_IND = 'A' ");
                strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL ");
            }
           
            strQry.AppendLine(" WHERE C.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",AUTHORISE_TYPE_IND");
            strQry.AppendLine(",LEVEL_NO");
            strQry.AppendLine(",USER_NO");
        
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_AUTHORISE PCA ");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "UserAuthorise", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT");
          
            strQry.AppendLine(" U.USER_NO");
            strQry.AppendLine(",U.USER_ID");
            strQry.AppendLine(",U.FIRSTNAME");
            strQry.AppendLine(",U.SURNAME");
            
            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID U ");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA ");
            strQry.AppendLine(" ON U.USER_NO = UCA.USER_NO");
            strQry.AppendLine(" AND UCA.COMPANY_NO = " + parint64CompanyNo);
            //strQry.AppendLine(" AND UCA.COMPANY_ACCESS_IND = 'A' ");
            strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL ");
          
            strQry.AppendLine(" WHERE U.DATETIME_DELETE_RECORD IS NULL ");


            strQry.AppendLine(" ORDER BY ");

            strQry.AppendLine(" U.USER_ID");
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "User", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" P.COMPANY_NO");
            strQry.AppendLine(",P.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",P.PAY_CATEGORY_NO");
            strQry.AppendLine(",P.PAY_CATEGORY_DESC");

            strQry.AppendLine(",P.MON_TIME_MINUTES");
            strQry.AppendLine(",P.TUE_TIME_MINUTES");
            strQry.AppendLine(",P.WED_TIME_MINUTES");
            strQry.AppendLine(",P.THU_TIME_MINUTES");
            strQry.AppendLine(",P.FRI_TIME_MINUTES");
            strQry.AppendLine(",P.SAT_TIME_MINUTES");
            strQry.AppendLine(",P.SUN_TIME_MINUTES");

            strQry.AppendLine(",P.OVERTIME1_MINUTES");
            strQry.AppendLine(",P.OVERTIME2_MINUTES");
            strQry.AppendLine(",P.OVERTIME3_MINUTES");

            strQry.AppendLine(",P.EXCEPTION_SUN_ABOVE_MINUTES");
            strQry.AppendLine(",P.EXCEPTION_SUN_BELOW_MINUTES");
            strQry.AppendLine(",P.EXCEPTION_MON_ABOVE_MINUTES");
            strQry.AppendLine(",P.EXCEPTION_MON_BELOW_MINUTES");
            strQry.AppendLine(",P.EXCEPTION_TUE_ABOVE_MINUTES");
            strQry.AppendLine(",P.EXCEPTION_TUE_BELOW_MINUTES");
            strQry.AppendLine(",P.EXCEPTION_WED_ABOVE_MINUTES");
            strQry.AppendLine(",P.EXCEPTION_WED_BELOW_MINUTES");
            strQry.AppendLine(",P.EXCEPTION_THU_ABOVE_MINUTES");
            strQry.AppendLine(",P.EXCEPTION_THU_BELOW_MINUTES");
            strQry.AppendLine(",P.EXCEPTION_FRI_ABOVE_MINUTES");
            strQry.AppendLine(",P.EXCEPTION_FRI_BELOW_MINUTES");
            strQry.AppendLine(",P.EXCEPTION_SAT_ABOVE_MINUTES");
            strQry.AppendLine(",P.EXCEPTION_SAT_BELOW_MINUTES");

            strQry.AppendLine(",P.DAILY_ROUNDING_IND");
            strQry.AppendLine(",P.DAILY_ROUNDING_MINUTES");
            strQry.AppendLine(",P.PAY_PERIOD_ROUNDING_IND");
            strQry.AppendLine(",P.PAY_PERIOD_ROUNDING_MINUTES");
            strQry.AppendLine(",P.OVERTIME_IND");
            strQry.AppendLine(",P.SATURDAY_PAY_RATE");
            strQry.AppendLine(",P.SATURDAY_PAY_RATE_IND");
            strQry.AppendLine(",P.SUNDAY_PAY_RATE");
            strQry.AppendLine(",P.SUNDAY_PAY_RATE_IND");
            strQry.AppendLine(",P.EXCEPTION_SHIFT_ABOVE_PERCENT");
            strQry.AppendLine(",P.EXCEPTION_SHIFT_BELOW_PERCENT");

            strQry.AppendLine(",P.PAIDHOLIDAY_RATE");
            strQry.AppendLine(",P.PAY_PUBLIC_HOLIDAY_IND");

            strQry.AppendLine(",P.TOTAL_DAILY_TIME_OVERTIME");
            strQry.AppendLine(",P.SALARY_MINUTES_PAID_PER_DAY");
            strQry.AppendLine(",P.SALARY_DAYS_PER_YEAR");
            
            strQry.AppendLine(",P.PAY_CATEGORY_DEL_IND ");
            strQry.AppendLine(",P.CLIENT_DB_ADMIN_RIGHTS_IND ");
            
            strQry.AppendLine(",P.NO_EDIT_IND ");
        
            strQry.AppendLine(",P.POST_ADDR_LINE1 ");
            strQry.AppendLine(",P.POST_ADDR_LINE2 ");
            strQry.AppendLine(",P.POST_ADDR_LINE3 ");
            strQry.AppendLine(",P.POST_ADDR_LINE4 ");
            strQry.AppendLine(",P.POST_ADDR_CODE ");

            strQry.AppendLine(",P.RES_UNIT_NUMBER ");
            strQry.AppendLine(",P.RES_COMPLEX ");
            strQry.AppendLine(",P.RES_STREET_NUMBER ");
            strQry.AppendLine(",P.RES_STREET_NAME ");
            strQry.AppendLine(",P.RES_SUBURB ");

            strQry.AppendLine(",P.RES_CITY ");
            strQry.AppendLine(",P.RES_ADDR_CODE ");
            strQry.AppendLine(",ISNULL(P.CLOSED_IND,'N') AS CLOSED_IND ");
            
            strQry.AppendLine(",PCPC.PAY_CATEGORY_NO AS PAYROLL_LINK");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY P");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");
            strQry.AppendLine(" ON P.COMPANY_NO = PCPC.COMPANY_NO");
            strQry.AppendLine(" AND P.PAY_CATEGORY_NO = PCPC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND P.PAY_CATEGORY_TYPE = PCPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P'");

            strQry.AppendLine(" WHERE P.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND P.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND P.PAY_CATEGORY_NO > 0");

            //2013-04-10
            if (parstrTimeAttendInd == "Y")
            {
                if (parstrCurrentUserAccess != "S")
                {
                    strQry.AppendLine(" AND P.PAY_CATEGORY_TYPE = 'T'");
                }
            }
            else
            {
                if (parstrCurrentUserAccess != "S")
                {
                    strQry.AppendLine(" AND P.PAY_CATEGORY_TYPE IN ('W','S')");
                }
            }

            //Has Not Been Deleted
            strQry.AppendLine(" AND P.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" P.PAY_CATEGORY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategory", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            
            strQry.AppendLine(",PAY_CATEGORY_BREAK_NO");

            strQry.AppendLine(",WORKED_TIME_MINUTES");
            strQry.AppendLine(",BREAK_MINUTES");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND PAY_CATEGORY_NO > 0");

            //2013-04-10
            if (parstrTimeAttendInd == "Y")
            {
                if (parstrCurrentUserAccess != "S")
                {
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");
                }
            }
            else
            {
                if (parstrCurrentUserAccess != "S")
                {
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE IN ('W','S')");
                }
            }

            //Has Not Been Deleted
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" WORKED_TIME_MINUTES");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategoryBreak", parint64CompanyNo);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        private void Update_Authorise_Record(Int64 parInt64CompanyNo, Int64 parint64CurrentUserNo, DataTable parDataTable)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            for (int intRow = 0; intRow < parDataTable.Rows.Count; intRow++)
            {
                if (parDataTable.Rows[intRow].RowState == DataRowState.Deleted)
                {
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_AUTHORISE ");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                    strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo.ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parDataTable.Rows[intRow]["PAY_CATEGORY_NO", System.Data.DataRowVersion.Original].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE", System.Data.DataRowVersion.Original].ToString()));
                    strQry.AppendLine(" AND AUTHORISE_TYPE_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["AUTHORISE_TYPE_IND", System.Data.DataRowVersion.Original].ToString()));
                    strQry.AppendLine(" AND LEVEL_NO = " + parDataTable.Rows[intRow]["LEVEL_NO", System.Data.DataRowVersion.Original].ToString());
                    strQry.AppendLine(" AND USER_NO = " + parDataTable.Rows[intRow]["USER_NO", System.Data.DataRowVersion.Original].ToString());

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    if (parDataTable.Rows[intRow]["AUTHORISE_TYPE_IND", System.Data.DataRowVersion.Original].ToString() == "T")
                    {
                        //Delete Record if Exist
                        strQry.Clear();

                        strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_PAYROLL_AUTHORISE_CURRENT");
                        strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo.ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parDataTable.Rows[intRow]["PAY_CATEGORY_NO", System.Data.DataRowVersion.Original].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE", System.Data.DataRowVersion.Original].ToString()));
                        strQry.AppendLine(" AND LEVEL_NO = " + parDataTable.Rows[intRow]["LEVEL_NO", System.Data.DataRowVersion.Original].ToString());
                        strQry.AppendLine(" AND USER_NO = " + parDataTable.Rows[intRow]["USER_NO", System.Data.DataRowVersion.Original].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                    }
                    else
                    {
                        //Leave
                        //Delete Record if Exist
                        strQry.Clear();

                        strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_LEAVE_AUTHORISE_CURRENT");
                        strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo.ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parDataTable.Rows[intRow]["PAY_CATEGORY_NO", System.Data.DataRowVersion.Original].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE", System.Data.DataRowVersion.Original].ToString()));
                        strQry.AppendLine(" AND LEVEL_NO = " + parDataTable.Rows[intRow]["LEVEL_NO", System.Data.DataRowVersion.Original].ToString());
                        strQry.AppendLine(" AND USER_NO = " + parDataTable.Rows[intRow]["USER_NO", System.Data.DataRowVersion.Original].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    }
                }
                else
                {
                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_AUTHORISE ");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",AUTHORISE_TYPE_IND");
                    strQry.AppendLine(",LEVEL_NO");
                    strQry.AppendLine(",USER_NO");
                    strQry.AppendLine(",TIE_BREAKER");
                    strQry.AppendLine(",USER_NO_NEW_RECORD");
                    strQry.AppendLine(",DATETIME_NEW_RECORD)");
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(parInt64CompanyNo.ToString());
                    strQry.AppendLine("," + parDataTable.Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["AUTHORISE_TYPE_IND"].ToString()));
                    strQry.AppendLine("," + parDataTable.Rows[intRow]["LEVEL_NO"].ToString());
                    strQry.AppendLine("," + parDataTable.Rows[intRow]["USER_NO"].ToString());
                    strQry.AppendLine(",ISNULL(MAX(TIE_BREAKER) + 1,1)");
                    strQry.AppendLine("," + parint64CurrentUserNo);
                    strQry.AppendLine(",GETDATE()");
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_AUTHORISE ");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo.ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parDataTable.Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine(" AND AUTHORISE_TYPE_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["AUTHORISE_TYPE_IND"].ToString()));
                    strQry.AppendLine(" AND LEVEL_NO = " + parDataTable.Rows[intRow]["LEVEL_NO"].ToString());
                    strQry.AppendLine(" AND USER_NO = " + parDataTable.Rows[intRow]["USER_NO"].ToString());

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    //NB Leave is Dynamic (Timesheets Are Batch and Cost Centre Would Be Locked)
                    if (parDataTable.Rows[intRow]["AUTHORISE_TYPE_IND"].ToString() == "L")
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

                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" LC.COMPANY_NO");
                        strQry.AppendLine(",LC.EMPLOYEE_NO");
                        strQry.AppendLine(",TEMP_TABLE.PAY_CATEGORY_NO");
                        strQry.AppendLine(",LC.PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",LC.EARNING_NO");
                        strQry.AppendLine("," + parDataTable.Rows[intRow]["LEVEL_NO"].ToString());
                        strQry.AppendLine(",LC.LEAVE_REC_NO");
                        strQry.AppendLine("," + parDataTable.Rows[intRow]["USER_NO"].ToString());
                        strQry.AppendLine(",'N'");
                       
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT LC");

                        strQry.AppendLine(" INNER JOIN ");

                        strQry.AppendLine("(SELECT ");
                        strQry.AppendLine(" EPC.COMPANY_NO");
                        strQry.AppendLine(",EPC.EMPLOYEE_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
                      
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

                        //Administrator
                        strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA ");
                        strQry.AppendLine(" ON E.COMPANY_NO = UCA.COMPANY_NO ");
                      
                        strQry.AppendLine(" AND UCA.USER_NO = " + parDataTable.Rows[intRow]["USER_NO"].ToString());

                        strQry.AppendLine(" AND UCA.COMPANY_ACCESS_IND = 'A' ");
                        strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL ");

                        strQry.AppendLine(" WHERE EPC.COMPANY_NO = " + parInt64CompanyNo.ToString());
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + parDataTable.Rows[intRow]["PAY_CATEGORY_NO"].ToString());

                        //Default is Link for LEAVE
                        strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
                        strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL) AS TEMP_TABLE");

                        strQry.AppendLine(" ON LC.COMPANY_NO = TEMP_TABLE.COMPANY_NO");
                        strQry.AppendLine(" AND LC.EMPLOYEE_NO = TEMP_TABLE.EMPLOYEE_NO");
                        strQry.AppendLine(" AND LC.PAY_CATEGORY_TYPE = TEMP_TABLE.PAY_CATEGORY_TYPE");

                        strQry.AppendLine(" WHERE LC.COMPANY_NO = " + parInt64CompanyNo.ToString());
                        strQry.AppendLine(" AND LC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                        //Next Run
                        strQry.AppendLine(" AND LC.PROCESS_NO = 0");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                    }
                }
            }

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }
      
        private void Update_Break_Record(Int64 parInt64CompanyNo, int parintPayCategoryNo, string parstrPayCategoryType,Int64 parint64CurrentUserNo, DataTable parDataTable)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            int intPayCategoryBreakNo = -1;

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ISNULL(MAX(PAY_CATEGORY_BREAK_NO),0) AS MAX_BREAK_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "TempBreak", parInt64CompanyNo);

            intPayCategoryBreakNo = Convert.ToInt32(DataSet.Tables["TempBreak"].Rows[0]["MAX_BREAK_NO"]) + 1;
            
            for (int intRow = 0; intRow < parDataTable.Rows.Count; intRow++)
            {
                if (parDataTable.Rows[intRow].RowState == DataRowState.Deleted)
                {
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                    strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE  = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE", System.Data.DataRowVersion.Original].ToString()));
                    strQry.AppendLine(" AND PAY_CATEGORY_BREAK_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["PAY_CATEGORY_BREAK_NO", System.Data.DataRowVersion.Original]));
                    strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

                    parDataTable.Rows[intRow].Delete();
                }
                else
                {
                    if (parDataTable.Rows[intRow].RowState == DataRowState.Added)
                    {
                        parDataTable.Rows[intRow]["PAY_CATEGORY_NO"] = parintPayCategoryNo;
                        parDataTable.Rows[intRow]["PAY_CATEGORY_BREAK_NO"] = intPayCategoryBreakNo;

                        strQry.Clear();

                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK "); 
                        strQry.AppendLine("(COMPANY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",PAY_CATEGORY_BREAK_NO");
                        strQry.AppendLine(",WORKED_TIME_MINUTES");
                        strQry.AppendLine(",BREAK_MINUTES");
                        strQry.AppendLine(",USER_NO_NEW_RECORD");
                        strQry.AppendLine(",DATETIME_NEW_RECORD)");
                        strQry.AppendLine(" VALUES ");

                        strQry.AppendLine("(" + parInt64CompanyNo);
                        strQry.AppendLine("," + parintPayCategoryNo);
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                        strQry.AppendLine("," + parDataTable.Rows[intRow]["PAY_CATEGORY_BREAK_NO"].ToString());
                        strQry.AppendLine("," + Convert.ToInt16(parDataTable.Rows[intRow]["WORKED_TIME_MINUTES"]));
                        strQry.AppendLine("," + Convert.ToInt16(parDataTable.Rows[intRow]["BREAK_MINUTES"]));
                        strQry.AppendLine("," + parint64CurrentUserNo);
                        strQry.AppendLine(",GETDATE())");
                        
                        intPayCategoryBreakNo += 1;
                    }
                    else
                    {
                        if (parDataTable.Rows[intRow].RowState == DataRowState.Modified)
                        {
                            strQry.Clear();

                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK");
                            strQry.AppendLine(" SET");
                            strQry.AppendLine(" WORKED_TIME_MINUTES = " + parDataTable.Rows[intRow]["WORKED_TIME_MINUTES"].ToString());
                            strQry.AppendLine(",BREAK_MINUTES = " + parDataTable.Rows[intRow]["BREAK_MINUTES"].ToString());
                            strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);
                            
                            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                            strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
                            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                            strQry.AppendLine(" AND PAY_CATEGORY_BREAK_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["PAY_CATEGORY_BREAK_NO"]));
                            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
                        }
                    }
                }

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            }

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }

        public byte[] Update_Record(Int64 parInt64CompanyNo, Int64 parint64CurrentUserNo, byte[] parbyteDataSet, string parstrPayCategoryType, string parstrCloseCostCentreInd)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            StringBuilder strQry = new StringBuilder();
            
            DataSet DataSet = new DataSet();

            parDataSet.Tables.Add("Reply");
            parDataSet.Tables["Reply"].Columns.Add("PAYROLL_RUN_IND", typeof(String));

            DataRow drDataRow = parDataSet.Tables["Reply"].NewRow();

            drDataRow["PAYROLL_RUN_IND"] = "N";

            parDataSet.Tables["Reply"].Rows.Add(drDataRow);
           
            strQry.Clear();

            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" PAY_CATEGORY_NO ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["PayCategory"].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parDataSet.Tables["PayCategory"].Rows[0]["PAY_CATEGORY_NO"].ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
            strQry.AppendLine(" AND RUN_TYPE = 'P'");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

            if (DataSet.Tables["Temp"].Rows.Count == 0)
            {
                if (parDataSet.Tables["PayCategory"].Rows[0]["SALARY_MINUTES_PAID_PER_DAY"] == System.DBNull.Value)
                {
                    parDataSet.Tables["PayCategory"].Rows[0]["SALARY_MINUTES_PAID_PER_DAY"] = 0;
                }

                if (parDataSet.Tables["PayCategory"].Rows[0]["SALARY_DAYS_PER_YEAR"] == System.DBNull.Value)
                {
                    parDataSet.Tables["PayCategory"].Rows[0]["SALARY_DAYS_PER_YEAR"] = 0;
                }

                if (parDataSet.Tables["PayCategory"].Rows[0].RowState == DataRowState.Added)
                {
                    DataSet.Tables.Remove("Temp");

                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" ISNULL(MAX(PAY_CATEGORY_NO),0) + 1 AS MAX_NO");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["PayCategory"].Rows[0]["COMPANY_NO"].ToString());

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

                    parDataSet.Tables["PayCategory"].Rows[0]["PAY_CATEGORY_NO"] = Convert.ToInt32(DataSet.Tables["Temp"].Rows[0]["MAX_NO"]);

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",PAY_CATEGORY_DESC");
                    strQry.AppendLine(",PAIDHOLIDAY_RATE");
                    strQry.AppendLine(",PAY_PUBLIC_HOLIDAY_IND");
                    
                    strQry.AppendLine(",MON_TIME_MINUTES");
                    strQry.AppendLine(",TUE_TIME_MINUTES");
                    strQry.AppendLine(",WED_TIME_MINUTES");
                    strQry.AppendLine(",THU_TIME_MINUTES");
                    strQry.AppendLine(",FRI_TIME_MINUTES");
                    strQry.AppendLine(",SAT_TIME_MINUTES");
                    strQry.AppendLine(",SUN_TIME_MINUTES");
                    strQry.AppendLine(",OVERTIME1_MINUTES");
                    strQry.AppendLine(",OVERTIME2_MINUTES");
                    strQry.AppendLine(",OVERTIME3_MINUTES");
                    
                    strQry.AppendLine(",EXCEPTION_SUN_ABOVE_MINUTES");
                    strQry.AppendLine(",EXCEPTION_SUN_BELOW_MINUTES");
                    strQry.AppendLine(",EXCEPTION_MON_ABOVE_MINUTES");
                    strQry.AppendLine(",EXCEPTION_MON_BELOW_MINUTES");
                    strQry.AppendLine(",EXCEPTION_TUE_ABOVE_MINUTES");
                    strQry.AppendLine(",EXCEPTION_TUE_BELOW_MINUTES");
                    strQry.AppendLine(",EXCEPTION_WED_ABOVE_MINUTES");
                    strQry.AppendLine(",EXCEPTION_WED_BELOW_MINUTES");
                    strQry.AppendLine(",EXCEPTION_THU_ABOVE_MINUTES");
                    strQry.AppendLine(",EXCEPTION_THU_BELOW_MINUTES");
                    strQry.AppendLine(",EXCEPTION_FRI_ABOVE_MINUTES");
                    strQry.AppendLine(",EXCEPTION_FRI_BELOW_MINUTES");
                    strQry.AppendLine(",EXCEPTION_SAT_ABOVE_MINUTES");
                    strQry.AppendLine(",EXCEPTION_SAT_BELOW_MINUTES");

                    strQry.AppendLine(",DAILY_ROUNDING_IND");
                    strQry.AppendLine(",DAILY_ROUNDING_MINUTES");
                   
                    strQry.AppendLine(",PAY_PERIOD_ROUNDING_IND");
                    strQry.AppendLine(",PAY_PERIOD_ROUNDING_MINUTES");
                    strQry.AppendLine(",OVERTIME_IND");
                    strQry.AppendLine(",SATURDAY_PAY_RATE");
                    strQry.AppendLine(",SATURDAY_PAY_RATE_IND");
                    strQry.AppendLine(",SUNDAY_PAY_RATE");
                    strQry.AppendLine(",SUNDAY_PAY_RATE_IND");
                    strQry.AppendLine(",EXCEPTION_SHIFT_ABOVE_PERCENT");
                    strQry.AppendLine(",EXCEPTION_SHIFT_BELOW_PERCENT");
                    strQry.AppendLine(",TOTAL_DAILY_TIME_OVERTIME");
                    strQry.AppendLine(",CLIENT_DB_ADMIN_RIGHTS_IND ");
                   
                    strQry.AppendLine(",SALARY_MINUTES_PAID_PER_DAY");
                    strQry.AppendLine(",SALARY_DAYS_PER_YEAR");

                    strQry.AppendLine(",POST_ADDR_LINE1");
                    strQry.AppendLine(",POST_ADDR_LINE2");
                    strQry.AppendLine(",POST_ADDR_LINE3");
                    strQry.AppendLine(",POST_ADDR_LINE4");
                    strQry.AppendLine(",POST_ADDR_CODE");

                    strQry.AppendLine(",RES_UNIT_NUMBER");
                    strQry.AppendLine(",RES_COMPLEX");
                    strQry.AppendLine(",RES_STREET_NUMBER");
                    strQry.AppendLine(",RES_STREET_NAME");
                    strQry.AppendLine(",RES_SUBURB");
                    strQry.AppendLine(",RES_CITY");
                    strQry.AppendLine(",RES_ADDR_CODE");

                    strQry.AppendLine(",DATETIME_NEW_RECORD");
                    strQry.AppendLine(",USER_NO_NEW_RECORD");
                    strQry.AppendLine(",PAY_CATEGORY_DEL_IND");

                    strQry.AppendLine(",NO_EDIT_IND)");
                    
                    strQry.AppendLine(" VALUES ");
                    
                    strQry.AppendLine("(" + parDataSet.Tables["PayCategory"].Rows[0]["COMPANY_NO"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["PAY_CATEGORY_DESC"].ToString()));
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["PAIDHOLIDAY_RATE"].ToString());
                    strQry.AppendLine("," + this.clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["PAY_PUBLIC_HOLIDAY_IND"].ToString()));

                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["MON_TIME_MINUTES"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["TUE_TIME_MINUTES"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["WED_TIME_MINUTES"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["THU_TIME_MINUTES"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["FRI_TIME_MINUTES"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["SAT_TIME_MINUTES"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["SUN_TIME_MINUTES"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["OVERTIME1_MINUTES"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["OVERTIME2_MINUTES"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["OVERTIME3_MINUTES"].ToString());
                   
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_SUN_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_SUN_BELOW_MINUTES"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_MON_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_MON_BELOW_MINUTES"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_TUE_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_TUE_BELOW_MINUTES"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_WED_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_WED_BELOW_MINUTES"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_THU_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_THU_BELOW_MINUTES"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_FRI_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_FRI_BELOW_MINUTES"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_SAT_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_SAT_BELOW_MINUTES"].ToString());

                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["DAILY_ROUNDING_IND"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["DAILY_ROUNDING_MINUTES"].ToString());
                  
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["PAY_PERIOD_ROUNDING_IND"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["PAY_PERIOD_ROUNDING_MINUTES"].ToString());
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["OVERTIME_IND"].ToString()));
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["SATURDAY_PAY_RATE"].ToString());
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["SATURDAY_PAY_RATE_IND"].ToString()));
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["SUNDAY_PAY_RATE"].ToString());
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["SUNDAY_PAY_RATE_IND"].ToString()));
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_SHIFT_ABOVE_PERCENT"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_SHIFT_BELOW_PERCENT"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["TOTAL_DAILY_TIME_OVERTIME"].ToString());
                    strQry.AppendLine("," + this.clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["CLIENT_DB_ADMIN_RIGHTS_IND"].ToString()));
                    
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["SALARY_MINUTES_PAID_PER_DAY"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["PayCategory"].Rows[0]["SALARY_DAYS_PER_YEAR"].ToString());

                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["POST_ADDR_LINE1"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["POST_ADDR_LINE2"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["POST_ADDR_LINE3"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["POST_ADDR_LINE4"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["POST_ADDR_CODE"].ToString()));

                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["RES_UNIT_NUMBER"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["RES_COMPLEX"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["RES_STREET_NUMBER"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["RES_STREET_NAME"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["RES_SUBURB"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["RES_CITY"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["RES_ADDR_CODE"].ToString()));
                    
                    strQry.AppendLine(",GETDATE()");
                    strQry.AppendLine("," + parint64CurrentUserNo);
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["PAY_CATEGORY_DEL_IND"].ToString()));

                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["NO_EDIT_IND"].ToString()) + ")");
                }
                else
                {
                    //Updated
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" PAY_CATEGORY_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["PAY_CATEGORY_DESC"].ToString()));
                    strQry.AppendLine(",PAIDHOLIDAY_RATE = " + parDataSet.Tables["PayCategory"].Rows[0]["PAIDHOLIDAY_RATE"].ToString());
                    strQry.AppendLine(",PAY_PUBLIC_HOLIDAY_IND = " + this.clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["PAY_PUBLIC_HOLIDAY_IND"].ToString()));
                    
                    strQry.AppendLine(",EXCEPTION_SHIFT_ABOVE_PERCENT = " + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_SHIFT_ABOVE_PERCENT"].ToString());
                    strQry.AppendLine(",EXCEPTION_SHIFT_BELOW_PERCENT = " + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_SHIFT_BELOW_PERCENT"].ToString());

                    strQry.AppendLine(",MON_TIME_MINUTES = " + parDataSet.Tables["PayCategory"].Rows[0]["MON_TIME_MINUTES"].ToString());
                    strQry.AppendLine(",TUE_TIME_MINUTES = " + parDataSet.Tables["PayCategory"].Rows[0]["TUE_TIME_MINUTES"].ToString());
                    strQry.AppendLine(",WED_TIME_MINUTES = " + parDataSet.Tables["PayCategory"].Rows[0]["WED_TIME_MINUTES"].ToString());
                    strQry.AppendLine(",THU_TIME_MINUTES = " + parDataSet.Tables["PayCategory"].Rows[0]["THU_TIME_MINUTES"].ToString());
                    strQry.AppendLine(",FRI_TIME_MINUTES = " + parDataSet.Tables["PayCategory"].Rows[0]["FRI_TIME_MINUTES"].ToString());
                    strQry.AppendLine(",SAT_TIME_MINUTES = " + parDataSet.Tables["PayCategory"].Rows[0]["SAT_TIME_MINUTES"].ToString());
                    strQry.AppendLine(",SUN_TIME_MINUTES = " + parDataSet.Tables["PayCategory"].Rows[0]["SUN_TIME_MINUTES"].ToString());
                    strQry.AppendLine(",OVERTIME1_MINUTES = " + parDataSet.Tables["PayCategory"].Rows[0]["OVERTIME1_MINUTES"].ToString());
                    strQry.AppendLine(",OVERTIME2_MINUTES = " + parDataSet.Tables["PayCategory"].Rows[0]["OVERTIME2_MINUTES"].ToString());
                    strQry.AppendLine(",OVERTIME3_MINUTES = " + parDataSet.Tables["PayCategory"].Rows[0]["OVERTIME3_MINUTES"].ToString());
                    
                    strQry.AppendLine(",EXCEPTION_SUN_ABOVE_MINUTES = " + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_SUN_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_SUN_BELOW_MINUTES = " + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_SUN_BELOW_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_MON_ABOVE_MINUTES = " + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_MON_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_MON_BELOW_MINUTES = " + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_MON_BELOW_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_TUE_ABOVE_MINUTES = " + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_TUE_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_TUE_BELOW_MINUTES = " + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_TUE_BELOW_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_WED_ABOVE_MINUTES = " + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_WED_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_WED_BELOW_MINUTES = " + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_WED_BELOW_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_THU_ABOVE_MINUTES = " + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_THU_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_THU_BELOW_MINUTES = " + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_THU_BELOW_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_FRI_ABOVE_MINUTES = " + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_FRI_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_FRI_BELOW_MINUTES = " + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_FRI_BELOW_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_SAT_ABOVE_MINUTES = " + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_SAT_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_SAT_BELOW_MINUTES = " + parDataSet.Tables["PayCategory"].Rows[0]["EXCEPTION_SAT_BELOW_MINUTES"].ToString());

                    strQry.AppendLine(",DAILY_ROUNDING_IND = " + parDataSet.Tables["PayCategory"].Rows[0]["DAILY_ROUNDING_IND"].ToString());
                    strQry.AppendLine(",DAILY_ROUNDING_MINUTES = " + parDataSet.Tables["PayCategory"].Rows[0]["DAILY_ROUNDING_MINUTES"].ToString());
                    strQry.AppendLine(",PAY_PERIOD_ROUNDING_IND = " + parDataSet.Tables["PayCategory"].Rows[0]["PAY_PERIOD_ROUNDING_IND"].ToString());
                    strQry.AppendLine(",PAY_PERIOD_ROUNDING_MINUTES = " + parDataSet.Tables["PayCategory"].Rows[0]["PAY_PERIOD_ROUNDING_MINUTES"].ToString());
                    strQry.AppendLine(",OVERTIME_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["OVERTIME_IND"].ToString()));
                    strQry.AppendLine(",SATURDAY_PAY_RATE = " + parDataSet.Tables["PayCategory"].Rows[0]["SATURDAY_PAY_RATE"].ToString());
                    strQry.AppendLine(",SATURDAY_PAY_RATE_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["SATURDAY_PAY_RATE_IND"].ToString()));
                    strQry.AppendLine(",SUNDAY_PAY_RATE = " + parDataSet.Tables["PayCategory"].Rows[0]["SUNDAY_PAY_RATE"].ToString());
                    strQry.AppendLine(",SUNDAY_PAY_RATE_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["SUNDAY_PAY_RATE_IND"].ToString()));
                    strQry.AppendLine(",TOTAL_DAILY_TIME_OVERTIME = " + parDataSet.Tables["PayCategory"].Rows[0]["TOTAL_DAILY_TIME_OVERTIME"].ToString());
                    strQry.AppendLine(",CLIENT_DB_ADMIN_RIGHTS_IND = " + this.clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["CLIENT_DB_ADMIN_RIGHTS_IND"].ToString()));
                    
                    //Salaries
                    strQry.AppendLine(",SALARY_MINUTES_PAID_PER_DAY = " + parDataSet.Tables["PayCategory"].Rows[0]["SALARY_MINUTES_PAID_PER_DAY"].ToString());
                    strQry.AppendLine(",SALARY_DAYS_PER_YEAR = " + parDataSet.Tables["PayCategory"].Rows[0]["SALARY_DAYS_PER_YEAR"].ToString());

                    strQry.AppendLine(",NO_EDIT_IND = " + this.clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["NO_EDIT_IND"].ToString()));
                    
                    strQry.AppendLine(",POST_ADDR_LINE1 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["POST_ADDR_LINE1"].ToString()));
                    strQry.AppendLine(",POST_ADDR_LINE2 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["POST_ADDR_LINE2"].ToString()));
                    strQry.AppendLine(",POST_ADDR_LINE3 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["POST_ADDR_LINE3"].ToString()));
                    strQry.AppendLine(",POST_ADDR_LINE4 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["POST_ADDR_LINE4"].ToString()));
                    strQry.AppendLine(",POST_ADDR_CODE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["POST_ADDR_CODE"].ToString()));

                    strQry.AppendLine(",RES_UNIT_NUMBER = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["RES_UNIT_NUMBER"].ToString()));
                    strQry.AppendLine(",RES_COMPLEX = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["RES_COMPLEX"].ToString()));
                    strQry.AppendLine(",RES_STREET_NUMBER = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["RES_STREET_NUMBER"].ToString()));
                    strQry.AppendLine(",RES_STREET_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["RES_STREET_NAME"].ToString()));
                    strQry.AppendLine(",RES_SUBURB = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["RES_SUBURB"].ToString()));
                    strQry.AppendLine(",RES_CITY = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["RES_CITY"].ToString()));
                    strQry.AppendLine(",RES_ADDR_CODE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[0]["RES_ADDR_CODE"].ToString()));
                
                    strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);

                    if (parstrCloseCostCentreInd == "Y")
                    {
                        parDataSet.Tables["PayCategory"].Rows[0]["CLOSED_IND"] = 'Y';

                        strQry.AppendLine(",CLOSED_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrCloseCostCentreInd));
                    }

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["PayCategory"].Rows[0]["COMPANY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parDataSet.Tables["PayCategory"].Rows[0]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
                    strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
                }

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                Insert_Pay_Category_Time_Decimal_Records(parDataSet.Tables["PayCategory"]);
                
                if (parDataSet.Tables["UserAuthorise"].Rows.Count > 0)
                {
                    Update_Authorise_Record(parInt64CompanyNo, parint64CurrentUserNo, (DataTable)parDataSet.Tables["UserAuthorise"]);
                }

                if (parDataSet.Tables["PayCategoryBreak"].Rows.Count > 0)
                {
                    Update_Break_Record(parInt64CompanyNo, Convert.ToInt32(parDataSet.Tables["PayCategory"].Rows[0]["PAY_CATEGORY_NO"]),parstrPayCategoryType,parint64CurrentUserNo,parDataSet.Tables["PayCategoryBreak"]);
                }

                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY");

                strQry.AppendLine(" SET LEAVE_DAY_RATE_DECIMAL = TEMP_TABLE.LEAVE_DAY_RATE_DECIMAL");

                strQry.AppendLine(" FROM ");

                strQry.AppendLine("(SELECT");
                strQry.AppendLine(" EPC.COMPANY_NO ");
                strQry.AppendLine(",EPC.EMPLOYEE_NO ");
                strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(",EPC.TIE_BREAKER ");

                strQry.AppendLine(",ROUND(SUM(PCTD.TIME_DECIMAL) / COUNT(*),2) AS LEAVE_DAY_RATE_DECIMAL");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");

                strQry.AppendLine(" INNER JOIN  InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");
                strQry.AppendLine(" ON EPC.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_TIME_DECIMAL PCTD");
                strQry.AppendLine(" ON EPC.COMPANY_NO = PCTD.COMPANY_NO ");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PCTD.PAY_CATEGORY_NO ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT LS ");
                strQry.AppendLine(" ON EPC.COMPANY_NO = LS.COMPANY_NO");
                strQry.AppendLine(" AND E.LEAVE_SHIFT_NO = LS.LEAVE_SHIFT_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = LS.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND (((LS.LEAVE_PAID_ACCUMULATOR_IND = 1");
                strQry.AppendLine(" AND PCTD.DAY_NO IN (1,2,3,4,5))");
                //Saturday Included
                strQry.AppendLine(" OR (LS.LEAVE_PAID_ACCUMULATOR_IND = 2");
                strQry.AppendLine(" AND PCTD.DAY_NO IN (1,2,3,4,5,6)))");
                //Sunday Included
                strQry.AppendLine(" OR (LS.LEAVE_PAID_ACCUMULATOR_IND = 3");
                strQry.AppendLine(" AND PCTD.DAY_NO IN (0,1,2,3,4,5,6)))");

                strQry.AppendLine(" AND LS.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" WHERE EPC.COMPANY_NO = " + parDataSet.Tables[0].Rows[0]["COMPANY_NO"].ToString());
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + parDataSet.Tables["PayCategory"].Rows[0]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
                //Leave Related
                strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y' ");
                strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

                strQry.AppendLine(" GROUP BY");
                strQry.AppendLine(" EPC.COMPANY_NO ");
                strQry.AppendLine(",EPC.EMPLOYEE_NO ");
                strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(",EPC.TIE_BREAKER) AS TEMP_TABLE ");

                strQry.AppendLine(" WHERE EMPLOYEE_PAY_CATEGORY.COMPANY_NO = TEMP_TABLE.COMPANY_NO");
                strQry.AppendLine(" AND EMPLOYEE_PAY_CATEGORY.EMPLOYEE_NO = TEMP_TABLE.EMPLOYEE_NO");
                strQry.AppendLine(" AND EMPLOYEE_PAY_CATEGORY.PAY_CATEGORY_NO = TEMP_TABLE.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EMPLOYEE_PAY_CATEGORY.PAY_CATEGORY_TYPE = TEMP_TABLE.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EMPLOYEE_PAY_CATEGORY.TIE_BREAKER = TEMP_TABLE.TIE_BREAKER");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            }
            else
            {
                //Reply - Not Possible (Wage Date is Open)
                parDataSet.Tables["Reply"].Rows[0]["PAYROLL_RUN_IND"] = "Y";
            }

            parDataSet.AcceptChanges();
    
            DataSet.Dispose();
            DataSet = null;

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(parDataSet);
            parDataSet.Dispose();
            parDataSet = null;

            return bytCompress;
        }

        public void Insert_Pay_Category_Time_Decimal_Records(DataTable parDataTable)
        {
            StringBuilder strQry = new StringBuilder();
            double dblTimeDecimal = 0;
            Int64 Int64CompanyNo = Convert.ToInt64(parDataTable.Rows[0]["COMPANY_NO"]);

            strQry.Clear();

            strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_TIME_DECIMAL");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataTable.Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parDataTable.Rows[0]["PAY_CATEGORY_NO"].ToString());

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Int64CompanyNo);

            if (Convert.ToDouble(parDataTable.Rows[0]["MON_TIME_MINUTES"]) != 0)
            {
                dblTimeDecimal = Math.Round(Convert.ToDouble(parDataTable.Rows[0]["MON_TIME_MINUTES"]) / 60, 2);

                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_TIME_DECIMAL");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_NO");
                strQry.AppendLine(",DAY_NO");
                strQry.AppendLine(",TIME_DECIMAL)");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine("(" + parDataTable.Rows[0]["COMPANY_NO"].ToString());
                strQry.AppendLine("," + parDataTable.Rows[0]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(",1");
                strQry.AppendLine("," + dblTimeDecimal + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Int64CompanyNo);
            }

            if (Convert.ToDouble(parDataTable.Rows[0]["TUE_TIME_MINUTES"]) != 0)
            {
                dblTimeDecimal = Math.Round(Convert.ToDouble(parDataTable.Rows[0]["TUE_TIME_MINUTES"]) / 60, 2);

                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_TIME_DECIMAL");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_NO");
                strQry.AppendLine(",DAY_NO");
                strQry.AppendLine(",TIME_DECIMAL)");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine("(" + parDataTable.Rows[0]["COMPANY_NO"].ToString());
                strQry.AppendLine("," + parDataTable.Rows[0]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(",2");
                strQry.AppendLine("," + dblTimeDecimal + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Int64CompanyNo);
            }

            if (Convert.ToDouble(parDataTable.Rows[0]["WED_TIME_MINUTES"]) != 0)
            {
                dblTimeDecimal = Math.Round(Convert.ToDouble(parDataTable.Rows[0]["WED_TIME_MINUTES"]) / 60, 2);

                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_TIME_DECIMAL");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_NO");
                strQry.AppendLine(",DAY_NO");
                strQry.AppendLine(",TIME_DECIMAL)");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine("(" + parDataTable.Rows[0]["COMPANY_NO"].ToString());
                strQry.AppendLine("," + parDataTable.Rows[0]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(",3");
                strQry.AppendLine("," + dblTimeDecimal + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Int64CompanyNo);
            }

            if (Convert.ToDouble(parDataTable.Rows[0]["THU_TIME_MINUTES"]) != 0)
            {
                dblTimeDecimal = Math.Round(Convert.ToDouble(parDataTable.Rows[0]["THU_TIME_MINUTES"]) / 60, 2);

                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_TIME_DECIMAL");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_NO");
                strQry.AppendLine(",DAY_NO");
                strQry.AppendLine(",TIME_DECIMAL)");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine("(" + parDataTable.Rows[0]["COMPANY_NO"].ToString());
                strQry.AppendLine("," + parDataTable.Rows[0]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(",4");
                strQry.AppendLine("," + dblTimeDecimal + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Int64CompanyNo);
            }

            if (Convert.ToDouble(parDataTable.Rows[0]["FRI_TIME_MINUTES"]) != 0)
            {
                dblTimeDecimal = Math.Round(Convert.ToDouble(parDataTable.Rows[0]["FRI_TIME_MINUTES"]) / 60, 2);

                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_TIME_DECIMAL");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_NO");
                strQry.AppendLine(",DAY_NO");
                strQry.AppendLine(",TIME_DECIMAL)");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine("(" + parDataTable.Rows[0]["COMPANY_NO"].ToString());
                strQry.AppendLine("," + parDataTable.Rows[0]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(",5");
                strQry.AppendLine("," + dblTimeDecimal + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Int64CompanyNo);
            }

            if (Convert.ToDouble(parDataTable.Rows[0]["SAT_TIME_MINUTES"]) != 0)
            {
                dblTimeDecimal = Math.Round(Convert.ToDouble(parDataTable.Rows[0]["SAT_TIME_MINUTES"]) / 60, 2);

                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_TIME_DECIMAL");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_NO");
                strQry.AppendLine(",DAY_NO");
                strQry.AppendLine(",TIME_DECIMAL)");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine("(" + parDataTable.Rows[0]["COMPANY_NO"].ToString());
                strQry.AppendLine("," + parDataTable.Rows[0]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(",6");
                strQry.AppendLine("," + dblTimeDecimal + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Int64CompanyNo);
            }

            if (Convert.ToDouble(parDataTable.Rows[0]["SUN_TIME_MINUTES"]) != 0)
            {
                dblTimeDecimal = Math.Round(Convert.ToDouble(parDataTable.Rows[0]["SUN_TIME_MINUTES"]) / 60, 2);

                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_TIME_DECIMAL");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_NO");
                strQry.AppendLine(",DAY_NO");
                strQry.AppendLine(",TIME_DECIMAL)");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine("(" + parDataTable.Rows[0]["COMPANY_NO"].ToString());
                strQry.AppendLine("," + parDataTable.Rows[0]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(",0");
                strQry.AppendLine("," + dblTimeDecimal + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Int64CompanyNo);
            }

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + Int64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }

        public int Delete_Record(Int64 parint64CurrentUserNo, Int64 parint64CompanyNo, int parintPayCategoryNo, string parstrPayCategoryType)
        {
            int intReturnCode = 0;
            DataSet myDataSet = new DataSet();
            
            StringBuilder strQry = new StringBuilder();
            
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EMPLOYEE_NO");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY ");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), myDataSet, "Temp", parint64CompanyNo);

            if (myDataSet.Tables["Temp"].Rows.Count > 0)
            {
                intReturnCode = 99;
            }
            else
            {
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            }

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            return intReturnCode;
        }
    }
}
