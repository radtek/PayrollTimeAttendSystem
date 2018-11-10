using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace InteractPayroll
{
    public class busDeduction
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busDeduction()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parInt64CompanyNo, string parstrCurrentUserAccess, Int64 parint64CurrentUserNo)
        {
            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new DataSet();
            DataRow dtDataRow;

            DataTable DataTable = new DataTable("SubAccount");

            DataTable.Columns.Add("DEDUCTION_SUB_ACCOUNT_COUNT", typeof(System.Int16));

            DataSet.Tables.Add(DataTable);

            for (int intDay = 1; intDay < 6; intDay++)
            {
                dtDataRow = DataSet.Tables["SubAccount"].NewRow();

                dtDataRow["DEDUCTION_SUB_ACCOUNT_COUNT"] = intDay;
                DataSet.Tables["SubAccount"].Rows.Add(dtDataRow);
            }
           
            if (parstrCurrentUserAccess == "S")
            {
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" 'A' AS ACCESS_IND");
                strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", parInt64CompanyNo);
            }
            else
            {
                //Administrator
                strQry.Clear();

                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" 'A' AS ACCESS_IND");
                strQry.AppendLine(" FROM ");
                strQry.AppendLine(" InteractPayroll.dbo.COMPANY_LINK C");
                strQry.AppendLine(",InteractPayroll.dbo.USER_COMPANY_ACCESS UCA ");
                strQry.AppendLine(" WHERE C.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND C.COMPANY_NO = UCA.COMPANY_NO ");
                strQry.AppendLine(" AND UCA.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND UCA.COMPANY_ACCESS_IND = 'A' ");
                strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL ");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", parInt64CompanyNo);
            }

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EARNING_NO");
            strQry.AppendLine(",E.EARNING_DESC");
            strQry.AppendLine(",E.IRP5_CODE");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING E");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C");
            strQry.AppendLine(" ON E.COMPANY_NO = C.COMPANY_NO ");
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);

            //Income/Normal Time/Bonus/Paid Holiday
            strQry.AppendLine(" AND (E.EARNING_NO IN (1,2,7)");

            //ELR - 2018-11-10
            //strQry.AppendLine(" OR (E.EARNING_NO IN (9,200,201)");
            strQry.AppendLine(" OR (E.EARNING_NO IN (9)");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W')");

            //Public Holiday - Company Paid 
            strQry.AppendLine(" OR (E.EARNING_NO = 8");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W')");

            //Overtime Wages
            strQry.AppendLine(" OR (E.EARNING_NO = 3");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" AND C.OVERTIME1_RATE <> 0)");

            strQry.AppendLine(" OR (E.EARNING_NO = 4");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" AND C.OVERTIME2_RATE <> 0)");

            strQry.AppendLine(" OR (E.EARNING_NO = 5");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" AND C.OVERTIME3_RATE <> 0)");

            //Overtime Salaries
            strQry.AppendLine(" OR (E.EARNING_NO = 3");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
            strQry.AppendLine(" AND C.SALARY_OVERTIME1_RATE <> 0)");

            strQry.AppendLine(" OR (E.EARNING_NO = 4");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
            strQry.AppendLine(" AND C.SALARY_OVERTIME2_RATE <> 0)");

            strQry.AppendLine(" OR (E.EARNING_NO = 5");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
            strQry.AppendLine(" AND C.SALARY_OVERTIME3_RATE <> 0))");

            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

            //ELR - 2018-11-10 (Leave Added to Select List)
            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EARNING_NO");
            strQry.AppendLine(",E.EARNING_DESC");
            strQry.AppendLine(",E.IRP5_CODE");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING E");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C");
            strQry.AppendLine(" ON E.COMPANY_NO = C.COMPANY_NO ");
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
            
            //Leave (Only for Wages)
            strQry.AppendLine(" AND E.EARNING_NO >= 200");
            strQry.AppendLine(" AND E.LEAVE_PERCENTAGE > 0");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
            
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

            //Own Created Earnings
            strQry.AppendLine(" UNION ");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EE.COMPANY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EE.EARNING_NO");
            strQry.AppendLine(",E.EARNING_DESC");
            strQry.AppendLine(",E.IRP5_CODE");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EE ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING E");
            strQry.AppendLine(" ON EE.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND EE.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EE.EARNING_NO = E.EARNING_NO ");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" WHERE EE.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EE.EMPLOYEE_NO = 0 ");
            strQry.AppendLine(" AND EE.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EARNING_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EarningList", parInt64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",EARNING_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EMPLOYEE_NO = 0 ");
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EarningPercentage", parInt64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" D.COMPANY_NO");
            strQry.AppendLine(",D.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",ED.EMPLOYEE_NO");
            strQry.AppendLine(",D.DEDUCTION_NO");
            strQry.AppendLine(",D.DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",D.DEDUCTION_DESC");
            strQry.AppendLine(",D.IRP5_CODE");
            strQry.AppendLine(",D.DEDUCTION_SUB_ACCOUNT_COUNT");
            strQry.AppendLine(",D.DEDUCTION_REPORT_HEADER1");
            strQry.AppendLine(",D.DEDUCTION_REPORT_HEADER2");
            strQry.AppendLine(",D.DEDUCTION_LOAN_TYPE_IND");
            strQry.AppendLine(",D.DEDUCTION_DEL_IND");
            strQry.AppendLine(",ED.DEDUCTION_TYPE_IND");
            strQry.AppendLine(",ED.DEDUCTION_VALUE");
            strQry.AppendLine(",ED.DEDUCTION_MIN_VALUE");
            strQry.AppendLine(",ED.DEDUCTION_MAX_VALUE");
            strQry.AppendLine(",ED.DEDUCTION_PERIOD_IND");
            strQry.AppendLine(",ED.DEDUCTION_DAY_VALUE");
            strQry.AppendLine(",ED.TIE_BREAKER");
            strQry.AppendLine(",EDC.DEDUCTION_NO AS PAYROLL_LINK");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.DEDUCTION D");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ED");
            strQry.AppendLine(" ON D.COMPANY_NO = ED.COMPANY_NO ");
            strQry.AppendLine(" AND D.PAY_CATEGORY_TYPE = ED.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND D.DEDUCTION_NO = ED.DEDUCTION_NO ");
            strQry.AppendLine(" AND D.DEDUCTION_SUB_ACCOUNT_NO = ED.DEDUCTION_SUB_ACCOUNT_NO ");
            strQry.AppendLine(" AND ED.EMPLOYEE_NO = 0 ");
            strQry.AppendLine(" AND ED.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT EDC");
            strQry.AppendLine(" ON D.COMPANY_NO = EDC.COMPANY_NO ");
            strQry.AppendLine(" AND D.PAY_CATEGORY_TYPE = EDC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND D.DEDUCTION_NO = EDC.DEDUCTION_NO ");
            strQry.AppendLine(" AND EDC.RUN_TYPE = 'P'");

            strQry.AppendLine(" WHERE D.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL ");

            //Only Get First One (All Others are a Duplicate of This One)
            strQry.AppendLine(" AND D.DEDUCTION_SUB_ACCOUNT_NO = 1");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" D.DEDUCTION_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Deduction", parInt64CompanyNo);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public int Insert_New_Record(Int64 parint64CurrentUserNo, byte[] parbyteDataSet)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            StringBuilder strQry = new StringBuilder();
            int intDeductionNo = -1;
            Int64 Int64CompanyNo = Convert.ToInt64(parDataSet.Tables["Deduction"].Rows[0]["COMPANY_NO"]);

            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" MAX(DEDUCTION_NO) AS MAX_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.DEDUCTION");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["Deduction"].Rows[0]["COMPANY_NO"].ToString());

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", Int64CompanyNo);

            if (DataSet.Tables["Temp"].Rows[0].IsNull("MAX_NO") == true)
            {
                intDeductionNo = 200;
            }
            else
            {
                if (Convert.ToInt32(DataSet.Tables["Temp"].Rows[0]["MAX_NO"]) < 199)
                {
                    intDeductionNo = 200;
                }
                else
                {
                    intDeductionNo = Convert.ToInt32(DataSet.Tables["Temp"].Rows[0]["MAX_NO"]) + 1;
                }
            }

            for (int intRow = 0; intRow < parDataSet.Tables["EarningPercentage"].Rows.Count; intRow++)
            {
                parDataSet.Tables["EarningPercentage"].Rows[intRow]["DEDUCTION_NO"] = intDeductionNo;
            }

            DataSet.Dispose();
            DataSet = null;

            for (int intRow = 1; intRow <= Convert.ToInt32(parDataSet.Tables["Deduction"].Rows[0]["DEDUCTION_SUB_ACCOUNT_COUNT"]); intRow++)
            {
                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.DEDUCTION");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",DEDUCTION_NO");
                strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",DEDUCTION_DESC");
                strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_COUNT");
                strQry.AppendLine(",DEDUCTION_LOAN_TYPE_IND");
                strQry.AppendLine(",DEDUCTION_REPORT_HEADER1");
                strQry.AppendLine(",DEDUCTION_REPORT_HEADER2");
                strQry.AppendLine(",USER_NO_NEW_RECORD");
                strQry.AppendLine(",DATETIME_NEW_RECORD");
                strQry.AppendLine(",DEDUCTION_DEL_IND)");
                strQry.AppendLine(" VALUES");
                strQry.AppendLine("(" + parDataSet.Tables["Deduction"].Rows[0]["COMPANY_NO"].ToString());
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Deduction"].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
                strQry.AppendLine("," + intDeductionNo);
                strQry.AppendLine("," + intRow);
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Deduction"].Rows[0]["DEDUCTION_DESC"].ToString()));
                strQry.AppendLine("," + Convert.ToInt32(parDataSet.Tables["Deduction"].Rows[0]["DEDUCTION_SUB_ACCOUNT_COUNT"]));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Deduction"].Rows[0]["DEDUCTION_LOAN_TYPE_IND"].ToString()));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Deduction"].Rows[0]["DEDUCTION_REPORT_HEADER1"].ToString()));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Deduction"].Rows[0]["DEDUCTION_REPORT_HEADER2"].ToString()));
                strQry.AppendLine("," + parint64CurrentUserNo);
                strQry.AppendLine(",GETDATE()");
                strQry.AppendLine(",'Y')");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Int64CompanyNo);
                
                strQry.Clear();

                //2017-02-24 Insert Record for opposite PAY_CATEGORY_TYPE  
                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.DEDUCTION");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",DEDUCTION_NO");
                strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",DEDUCTION_DESC");
                strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_COUNT");
                strQry.AppendLine(",DEDUCTION_LOAN_TYPE_IND");
                strQry.AppendLine(",DEDUCTION_REPORT_HEADER1");
                strQry.AppendLine(",DEDUCTION_REPORT_HEADER2");
                strQry.AppendLine(",USER_NO_NEW_RECORD");
                strQry.AppendLine(",DATETIME_NEW_RECORD");
                strQry.AppendLine(",DEDUCTION_DEL_IND)");
                strQry.AppendLine(" VALUES");
                strQry.AppendLine("(" + parDataSet.Tables["Deduction"].Rows[0]["COMPANY_NO"].ToString());

                if (parDataSet.Tables["Deduction"].Rows[0]["PAY_CATEGORY_TYPE"].ToString() == "W")
                {
                    strQry.AppendLine(",'S'");
                }
                else
                {
                    strQry.AppendLine(",'W'");
                }
               
                strQry.AppendLine("," + intDeductionNo);
                strQry.AppendLine("," + intRow);
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Deduction"].Rows[0]["DEDUCTION_DESC"].ToString()));
                strQry.AppendLine("," + Convert.ToInt32(parDataSet.Tables["Deduction"].Rows[0]["DEDUCTION_SUB_ACCOUNT_COUNT"]));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Deduction"].Rows[0]["DEDUCTION_LOAN_TYPE_IND"].ToString()));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Deduction"].Rows[0]["DEDUCTION_REPORT_HEADER1"].ToString()));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Deduction"].Rows[0]["DEDUCTION_REPORT_HEADER2"].ToString()));
                strQry.AppendLine("," + parint64CurrentUserNo);
                strQry.AppendLine(",GETDATE()");
                strQry.AppendLine(",'Y')");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Int64CompanyNo);

                strQry.Clear();
              
                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EMPLOYEE_NO");
                strQry.AppendLine(",DEDUCTION_NO");
                strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",TIE_BREAKER");
                strQry.AppendLine(",DEDUCTION_TYPE_IND");
                strQry.AppendLine(",DEDUCTION_VALUE");
                strQry.AppendLine(",DEDUCTION_PERIOD_IND");
                strQry.AppendLine(",DEDUCTION_DAY_VALUE");
                strQry.AppendLine(",DEDUCTION_MIN_VALUE");
                strQry.AppendLine(",DEDUCTION_MAX_VALUE");
                strQry.AppendLine(",DATETIME_NEW_RECORD");
                strQry.AppendLine(",USER_NO_NEW_RECORD)");
                strQry.AppendLine(" VALUES");
                strQry.AppendLine("(" + parDataSet.Tables["Deduction"].Rows[0]["COMPANY_NO"].ToString());
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Deduction"].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
                strQry.AppendLine(",0");
                strQry.AppendLine("," + intDeductionNo);
                strQry.AppendLine("," + intRow);
                strQry.AppendLine(",1");
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Deduction"].Rows[0]["DEDUCTION_TYPE_IND"].ToString()));
                strQry.AppendLine("," + parDataSet.Tables["Deduction"].Rows[0]["DEDUCTION_VALUE"].ToString());
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Deduction"].Rows[0]["DEDUCTION_PERIOD_IND"].ToString()));
                strQry.AppendLine("," + parDataSet.Tables["Deduction"].Rows[0]["DEDUCTION_DAY_VALUE"].ToString());
                strQry.AppendLine("," + parDataSet.Tables["Deduction"].Rows[0]["DEDUCTION_MIN_VALUE"].ToString());
                strQry.AppendLine("," + parDataSet.Tables["Deduction"].Rows[0]["DEDUCTION_MAX_VALUE"].ToString());
                strQry.AppendLine(",GETDATE()");
                strQry.AppendLine("," + parint64CurrentUserNo + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Int64CompanyNo);

                strQry.Clear();

                //2017-02-24 Insert Record for opposite PAY_CATEGORY_TYPE  
                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EMPLOYEE_NO");
                strQry.AppendLine(",DEDUCTION_NO");
                strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
                strQry.AppendLine(",TIE_BREAKER");
                strQry.AppendLine(",DEDUCTION_TYPE_IND");
                strQry.AppendLine(",DEDUCTION_VALUE");
                strQry.AppendLine(",DEDUCTION_PERIOD_IND");
                strQry.AppendLine(",DEDUCTION_DAY_VALUE");
                strQry.AppendLine(",DEDUCTION_MIN_VALUE");
                strQry.AppendLine(",DEDUCTION_MAX_VALUE");
                strQry.AppendLine(",DATETIME_NEW_RECORD");
                strQry.AppendLine(",USER_NO_NEW_RECORD)");
                strQry.AppendLine(" VALUES");
                strQry.AppendLine("(" + parDataSet.Tables["Deduction"].Rows[0]["COMPANY_NO"].ToString());

                if (parDataSet.Tables["Deduction"].Rows[0]["PAY_CATEGORY_TYPE"].ToString() == "W")
                {
                    strQry.AppendLine(",'S'");
                }
                else
                {
                    strQry.AppendLine(",'W'");
                }
                
                strQry.AppendLine(",0");
                strQry.AppendLine("," + intDeductionNo);
                strQry.AppendLine("," + intRow);
                strQry.AppendLine(",1");
                strQry.AppendLine(",'U'");
                strQry.AppendLine(",0");
                strQry.AppendLine(",'E'");
                strQry.AppendLine(",0");
                strQry.AppendLine(",0");
                strQry.AppendLine(",0");
                strQry.AppendLine(",GETDATE()");
                strQry.AppendLine("," + parint64CurrentUserNo + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Int64CompanyNo);
            }

            if (parDataSet.Tables["EarningPercentage"].Rows.Count > 0)
            {
                Save_Deduction_Earning_Link(Int64CompanyNo, parint64CurrentUserNo, parDataSet.Tables["EarningPercentage"], Convert.ToInt32(parDataSet.Tables["Deduction"].Rows[0]["DEDUCTION_SUB_ACCOUNT_COUNT"]));
            }

            parDataSet.Dispose();
            parDataSet = null;

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + Int64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            return intDeductionNo;
        }

        public void Save_Deduction_Earning_Link(Int64 parInt64CompanyNo, Int64 parint64CurrentUserNo, DataTable parDataTable, int intSubAccountCount)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
            DataView DataView;
            DataRowView DataRowView;

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EARNING_NO ");
            strQry.AppendLine(",MAX(TIE_BREAKER) AS MAX_TIE_BREAKER");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE");

            if (parDataTable.Rows[0].RowState == DataRowState.Deleted)
            {
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[0]["PAY_CATEGORY_TYPE", System.Data.DataRowVersion.Original].ToString()));
                strQry.AppendLine(" AND EMPLOYEE_NO = 0 ");
                strQry.AppendLine(" AND DEDUCTION_NO = " + parDataTable.Rows[0]["DEDUCTION_NO", System.Data.DataRowVersion.Original].ToString());
                strQry.AppendLine(" AND DEDUCTION_SUB_ACCOUNT_NO = " + parDataTable.Rows[0]["DEDUCTION_SUB_ACCOUNT_NO", System.Data.DataRowVersion.Original].ToString());
            }
            else
            {
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
                strQry.AppendLine(" AND EMPLOYEE_NO = 0 ");
                strQry.AppendLine(" AND DEDUCTION_NO = " + parDataTable.Rows[0]["DEDUCTION_NO"].ToString());
                strQry.AppendLine(" AND DEDUCTION_SUB_ACCOUNT_NO = " + parDataTable.Rows[0]["DEDUCTION_SUB_ACCOUNT_NO"].ToString());
            }

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" EARNING_NO ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

            //Initialise Keys
            for (int intRow = 0; intRow < DataSet.Tables["Temp"].Rows.Count; intRow++)
            {
                if (DataSet.Tables["Temp"].Rows[intRow]["MAX_TIE_BREAKER"] == System.DBNull.Value)
                {
                    DataSet.Tables["Temp"].Rows[intRow]["MAX_TIE_BREAKER"] = 1;
                }
                else
                {
                    DataSet.Tables["Temp"].Rows[intRow]["MAX_TIE_BREAKER"] = Convert.ToInt32(DataSet.Tables["Temp"].Rows[intRow]["MAX_TIE_BREAKER"]) + 1;
                }
            }

            for (int intRow = 0; intRow < parDataTable.Rows.Count; intRow++)
            {
                if (parDataTable.Rows[intRow].RowState == DataRowState.Deleted)
                {
                    for (int intCount = 1; intCount <= intSubAccountCount; intCount++)
                    {
                        strQry.Clear();

                        strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE");
                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                        strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
                        strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE", System.Data.DataRowVersion.Original].ToString()));
                        strQry.AppendLine(" AND EMPLOYEE_NO = 0 ");
                        strQry.AppendLine(" AND DEDUCTION_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["DEDUCTION_NO", System.Data.DataRowVersion.Original]));
                        strQry.AppendLine(" AND DEDUCTION_SUB_ACCOUNT_NO = " + intCount);
                        strQry.AppendLine(" AND EARNING_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["EARNING_NO", System.Data.DataRowVersion.Original]));
                        strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                    }
                }
                else
                {
                    if (parDataTable.Rows[intRow].RowState == DataRowState.Added)
                    {
                        DataView = null;
                        DataView = new DataView(DataSet.Tables["Temp"],
                            "EARNING_NO = " + parDataTable.Rows[intRow]["EARNING_NO"].ToString(),
                            "",
                            DataViewRowState.CurrentRows);

                        if (DataView.Count == 0)
                        {
                            DataRowView = DataView.AddNew();

                            DataRowView.BeginEdit();

                            DataRowView["EARNING_NO"] = Convert.ToInt32(parDataTable.Rows[intRow]["EARNING_NO"]);
                            DataRowView["MAX_TIE_BREAKER"] = 1;

                            DataRowView.EndEdit();
                        }

                        for (int intCount = 1; intCount <= intSubAccountCount; intCount++)
                        {
                            strQry.Clear();

                            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE");
                            strQry.AppendLine("(COMPANY_NO");
                            strQry.AppendLine(",PAY_CATEGORY_TYPE");
                            strQry.AppendLine(",EMPLOYEE_NO");
                            strQry.AppendLine(",DEDUCTION_NO");
                            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
                            strQry.AppendLine(",EARNING_NO");
                            strQry.AppendLine(",TIE_BREAKER");
                            strQry.AppendLine(",DATETIME_NEW_RECORD");
                            strQry.AppendLine(",USER_NO_NEW_RECORD)");
                            strQry.AppendLine(" VALUES");
                            strQry.AppendLine("(" + parInt64CompanyNo);
                            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                            strQry.AppendLine(",0");
                            strQry.AppendLine("," + Convert.ToInt32(parDataTable.Rows[intRow]["DEDUCTION_NO"]));
                            strQry.AppendLine("," + intCount);
                            strQry.AppendLine("," + Convert.ToInt32(parDataTable.Rows[intRow]["EARNING_NO"]));
                            strQry.AppendLine("," + DataView[0]["MAX_TIE_BREAKER"].ToString());
                            strQry.AppendLine(",GETDATE()");
                            strQry.AppendLine("," + parint64CurrentUserNo + ")");

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                        }
                    }
                }
            }

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            DataSet.Tables.Remove("Temp");
        }

        public int Update_Record(Int64 parInt64CompanyNo, Int64 parint64CurrentUserNo, byte[] parbyteDataSet)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
            int intReturnCode = 0;

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" DEDUCTION_NO ");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT ");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND DEDUCTION_NO = " + parDataSet.Tables[0].Rows[0]["DEDUCTION_NO"].ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
            strQry.AppendLine(" AND RUN_TYPE = 'P'");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

            if (DataSet.Tables["Temp"].Rows.Count > 0)
            {
                intReturnCode = 9999;
                goto Update_Record_Continue;
            }

            strQry.Clear();

            strQry.AppendLine(" SELECT MAX(DEDUCTION_SUB_ACCOUNT_NO) AS MAX_DEDUCTION_SUB_ACCOUNT_NO ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
            strQry.AppendLine(" AND DEDUCTION_NO = " + parDataSet.Tables[0].Rows[0]["DEDUCTION_NO"].ToString());

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT MAX(DEDUCTION_SUB_ACCOUNT_NO) AS MAX_DEDUCTION_SUB_ACCOUNT_NO ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
            strQry.AppendLine(" AND DEDUCTION_NO = " + parDataSet.Tables[0].Rows[0]["DEDUCTION_NO"].ToString());
            strQry.AppendLine(" ORDER BY 1 DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "MaxSubAccounts", parInt64CompanyNo);

            if (DataSet.Tables["MaxSubAccounts"].Rows[0]["MAX_DEDUCTION_SUB_ACCOUNT_NO"] != System.DBNull.Value)
            {
                if (Convert.ToInt32(DataSet.Tables["MaxSubAccounts"].Rows[0]["MAX_DEDUCTION_SUB_ACCOUNT_NO"]) > Convert.ToInt32(parDataSet.Tables[0].Rows[0]["DEDUCTION_SUB_ACCOUNT_COUNT"]))
                {
                    intReturnCode = Convert.ToInt32(DataSet.Tables["MaxSubAccounts"].Rows[0]["MAX_DEDUCTION_SUB_ACCOUNT_NO"]);
                    goto Update_Record_Continue;
                }
            }

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.DEDUCTION");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" DEDUCTION_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["DEDUCTION_DESC"].ToString()));
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_COUNT = " + Convert.ToDouble(parDataSet.Tables[0].Rows[0]["DEDUCTION_SUB_ACCOUNT_COUNT"]));
            strQry.AppendLine(",DEDUCTION_LOAN_TYPE_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["DEDUCTION_LOAN_TYPE_IND"].ToString()));
            strQry.AppendLine(",DEDUCTION_REPORT_HEADER1 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["DEDUCTION_REPORT_HEADER1"].ToString()));
            strQry.AppendLine(",DEDUCTION_REPORT_HEADER2 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["DEDUCTION_REPORT_HEADER2"].ToString()));
            strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
            strQry.AppendLine(" AND DEDUCTION_NO = " + parDataSet.Tables[0].Rows[0]["DEDUCTION_NO"].ToString());
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            //Set Default Deduction Value
            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" DEDUCTION_TYPE_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["DEDUCTION_TYPE_IND"].ToString()));
            strQry.AppendLine(",DEDUCTION_VALUE = " + parDataSet.Tables[0].Rows[0]["DEDUCTION_VALUE"].ToString());
            strQry.AppendLine(",DEDUCTION_MIN_VALUE = " + parDataSet.Tables[0].Rows[0]["DEDUCTION_MIN_VALUE"].ToString());
            strQry.AppendLine(",DEDUCTION_MAX_VALUE = " + parDataSet.Tables[0].Rows[0]["DEDUCTION_MAX_VALUE"].ToString());
            strQry.AppendLine(",DEDUCTION_PERIOD_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["DEDUCTION_PERIOD_IND"].ToString()));
            strQry.AppendLine(",DEDUCTION_DAY_VALUE = " + parDataSet.Tables[0].Rows[0]["DEDUCTION_DAY_VALUE"].ToString());
            strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
            strQry.AppendLine(" AND EMPLOYEE_NO = 0");
            strQry.AppendLine(" AND DEDUCTION_NO = " + parDataSet.Tables[0].Rows[0]["DEDUCTION_NO"].ToString());
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            //Set All Deductions
            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" DEDUCTION_TYPE_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["DEDUCTION_TYPE_IND"].ToString()));
            strQry.AppendLine(",DEDUCTION_VALUE = " + parDataSet.Tables[0].Rows[0]["DEDUCTION_VALUE"].ToString());
            strQry.AppendLine(",DEDUCTION_MIN_VALUE = " + parDataSet.Tables[0].Rows[0]["DEDUCTION_MIN_VALUE"].ToString());
            strQry.AppendLine(",DEDUCTION_MAX_VALUE = " + parDataSet.Tables[0].Rows[0]["DEDUCTION_MAX_VALUE"].ToString());
            strQry.AppendLine(",DEDUCTION_PERIOD_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["DEDUCTION_PERIOD_IND"].ToString()));
            strQry.AppendLine(",DEDUCTION_DAY_VALUE = " + parDataSet.Tables[0].Rows[0]["DEDUCTION_DAY_VALUE"].ToString());
            strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
            strQry.AppendLine(" AND EMPLOYEE_NO > 0");
            strQry.AppendLine(" AND DEDUCTION_NO = " + parDataSet.Tables[0].Rows[0]["DEDUCTION_NO"].ToString());

            strQry.AppendLine(" AND DEDUCTION_TYPE_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["DEDUCTION_TYPE_IND", DataRowVersion.Original].ToString()));

            //Previous Value 
            strQry.AppendLine(" AND DEDUCTION_VALUE = " + parDataSet.Tables[0].Rows[0]["DEDUCTION_VALUE",DataRowVersion.Original].ToString());

            strQry.AppendLine(" AND DEDUCTION_MIN_VALUE = " + parDataSet.Tables[0].Rows[0]["DEDUCTION_MIN_VALUE", DataRowVersion.Original].ToString());
            strQry.AppendLine(" AND DEDUCTION_MAX_VALUE = " + parDataSet.Tables[0].Rows[0]["DEDUCTION_MAX_VALUE", DataRowVersion.Original].ToString());

            strQry.AppendLine(" AND DEDUCTION_PERIOD_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["DEDUCTION_PERIOD_IND", DataRowVersion.Original].ToString()));
            strQry.AppendLine(" AND DEDUCTION_DAY_VALUE = " + parDataSet.Tables[0].Rows[0]["DEDUCTION_DAY_VALUE", DataRowVersion.Original].ToString());
            
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
      
            if (parDataSet.Tables["EarningPercentage"].Rows.Count > 0)
            {
                Save_Deduction_Earning_Link(parInt64CompanyNo, parint64CurrentUserNo, parDataSet.Tables["EarningPercentage"], Convert.ToInt32(parDataSet.Tables[0].Rows[0]["DEDUCTION_SUB_ACCOUNT_COUNT"]));
            }
            
        Update_Record_Continue:

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            DataSet.Dispose();
            DataSet = null;

            return intReturnCode;
        }

        public void Delete_Record(Int64 parint64CurrentUserNo, Int64 parInt64CompanyNo, string parstrPayCategoryType, int parintDeductionNo)
        {
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.DEDUCTION");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
            strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
            strQry.AppendLine(" AND DEDUCTION_NO = " + parintDeductionNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
            strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
            strQry.AppendLine(" AND DEDUCTION_NO = " + parintDeductionNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
            strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
            strQry.AppendLine(" AND EMPLOYEE_NO = 0 ");
            strQry.AppendLine(" AND DEDUCTION_NO = " + parintDeductionNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }
    }
}
