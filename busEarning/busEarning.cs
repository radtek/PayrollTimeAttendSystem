using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busEarning
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busEarning()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parInt64CompanyNo, string parstrCurrentUserAccess, Int64 parInt64CurrentUserNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" IRP5_CODE ");

            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.EARNING_DEFAULT ");

            strQry.AppendLine(" WHERE IRP5_CODE IS NOT NULL ");
            strQry.AppendLine(" AND IRP5_CODE <> 0");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "IRP5", parInt64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT");

            if (parstrCurrentUserAccess == "S"
                | parstrCurrentUserAccess == "A")
            {
                strQry.AppendLine("'A' AS ACCESS_IND");
            }
            else
            {
                strQry.AppendLine(" UM.ACCESS_IND");
            }


            strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK C");

            if (parstrCurrentUserAccess == "A")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA ");
                strQry.AppendLine(" ON C.COMPANY_NO = UCA.COMPANY_NO ");
                strQry.AppendLine(" AND UCA.USER_NO = " + parInt64CurrentUserNo);
                strQry.AppendLine(" AND UCA.COMPANY_ACCESS_IND = 'A' ");
                strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL ");
            }
           
            strQry.AppendLine(" WHERE C.COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", parInt64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT  ");
            strQry.AppendLine(parInt64CompanyNo + " AS COMPANY_NO");
            strQry.AppendLine(",PCC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",ED.EARNING_NO");
            strQry.AppendLine(",ED.EARNING_DESC");
            strQry.AppendLine(",0 AS AMOUNT");
            strQry.AppendLine(",ED.EARNING_TYPE_IND");
            
            strQry.AppendLine(",ED.EARNING_REPORT_HEADER1");
            strQry.AppendLine(",ED.EARNING_REPORT_HEADER2");
            strQry.AppendLine(",ED.IRP5_CODE");
           
            strQry.AppendLine(" FROM InteractPayroll.dbo.EARNING_DEFAULT ED");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.PAY_CATEGORY_CONVERT PCC");
            strQry.AppendLine(" ON ED.PAY_CATEGORY_TYPE_BOTH = PCC.PAY_CATEGORY_TYPE_BOTH ");

            strQry.AppendLine(" WHERE ED.EARNING_NO > 9");
            //Normal Leave / Sick Leave
            strQry.AppendLine(" AND ED.EARNING_NO NOT IN (200,201)");

            strQry.AppendLine(" AND PCC.PAY_CATEGORY_TYPE + STR(ED.EARNING_NO) NOT IN ");
            strQry.AppendLine("(SELECT PAY_CATEGORY_TYPE + STR(EARNING_NO) ");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" AND EMPLOYEE_NO = 0)");
            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" PCC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",ED.EARNING_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EarningList", parInt64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",0 AS EMPLOYEE_NO");
            strQry.AppendLine(",E.EARNING_NO");
            strQry.AppendLine(",E.EARNING_DESC");

            strQry.AppendLine(",0 AS AMOUNT");
            
            strQry.AppendLine(",E.IRP5_CODE");
            strQry.AppendLine(",E.EARNING_REPORT_HEADER1");
            strQry.AppendLine(",E.EARNING_REPORT_HEADER2");
            strQry.AppendLine(",'U' AS EARNING_TYPE_IND");
            
            strQry.AppendLine(",'Y' AS EARNING_TYPE_DEFAULT");
            strQry.AppendLine(",'E' AS EARNING_PERIOD_IND");
            strQry.AppendLine(",0 AS EARNING_DAY_VALUE");
            strQry.AppendLine(",1 AS TIE_BREAKER");
            strQry.AppendLine(",'N' AS EARNING_DEL_IND");
            strQry.AppendLine(",EEC.EARNING_NO AS PAYROLL_LINK");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING E ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C");
            strQry.AppendLine(" ON E.COMPANY_NO = C.COMPANY_NO ");
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.EARNING_DEFAULT ED ");
            strQry.AppendLine(" ON E.EARNING_NO = ED.EARNING_NO ");
            strQry.AppendLine(" AND (E.PAY_CATEGORY_TYPE = ED.PAY_CATEGORY_TYPE_BOTH ");
            strQry.AppendLine(" OR ED.PAY_CATEGORY_TYPE_BOTH = 'B') ");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");
            strQry.AppendLine(" ON E.COMPANY_NO = EEC.COMPANY_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EEC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND ED.EARNING_NO = EEC.EARNING_NO");
            strQry.AppendLine(" AND EEC.RUN_TYPE = 'P'");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

            //Income/Normal Time/Bonus/Paid Holiday
            strQry.AppendLine(" AND (E.EARNING_NO IN (1,2,7)");

            strQry.AppendLine(" OR (E.EARNING_NO IN (200,201)");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W')");

            //Public Holiday - Company Paid 
            strQry.AppendLine(" OR (E.EARNING_NO = 8");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W')");

            //Public Holiday - Worked 
            strQry.AppendLine(" OR (E.EARNING_NO = 9");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W')");

            //Overtime 
            strQry.AppendLine(" OR (E.EARNING_NO = 3");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" AND C.OVERTIME1_RATE <> 0)");

            strQry.AppendLine(" OR (E.EARNING_NO = 4");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" AND C.OVERTIME2_RATE <> 0)");

            strQry.AppendLine(" OR (E.EARNING_NO = 5");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" AND C.OVERTIME3_RATE <> 0)");

            //PreDefined Earnings
            //strQry.AppendLine(" OR (E.EARNING_NO > 9");
            //strQry.AppendLine(" AND E.EARNING_NO < 150)");

            //Overtime 
            strQry.AppendLine(" OR (E.EARNING_NO = 3");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
            strQry.AppendLine(" AND C.SALARY_OVERTIME1_RATE <> 0)");

            strQry.AppendLine(" OR (E.EARNING_NO = 4");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
            strQry.AppendLine(" AND C.SALARY_OVERTIME2_RATE <> 0)");

            strQry.AppendLine(" OR (E.EARNING_NO = 5");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
            strQry.AppendLine(" AND C.SALARY_OVERTIME3_RATE <> 0))");

            //User Created Own Earning
            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EE.COMPANY_NO");
            strQry.AppendLine(",EE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EE.EMPLOYEE_NO");
            strQry.AppendLine(",E.EARNING_NO");
            strQry.AppendLine(",E.EARNING_DESC");

            strQry.AppendLine(",EE.AMOUNT");
            
            strQry.AppendLine(",E.IRP5_CODE");
            strQry.AppendLine(",E.EARNING_REPORT_HEADER1");
            strQry.AppendLine(",E.EARNING_REPORT_HEADER2");
            strQry.AppendLine(",EE.EARNING_TYPE_IND");
            
            strQry.AppendLine(",'N' AS EARNING_TYPE_DEFAULT");
            strQry.AppendLine(",EE.EARNING_PERIOD_IND");
            strQry.AppendLine(",EE.EARNING_DAY_VALUE");
            strQry.AppendLine(",EE.TIE_BREAKER");
            strQry.AppendLine(",E.EARNING_DEL_IND");
            strQry.AppendLine(",EEC.EARNING_NO AS PAYROLL_LINK");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EE ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING E");
            strQry.AppendLine(" ON EE.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND EE.EARNING_NO = E.EARNING_NO ");
            strQry.AppendLine(" AND EE.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");
            //strQry.AppendLine(" AND STR(E.EARNING_NO) NOT IN ");
            //strQry.AppendLine("(SELECT DISTINCT STR(EARNING_NO) ");
            //strQry.AppendLine(" FROM InteractPayroll.dbo.EARNING_DEFAULT) ");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");
            strQry.AppendLine(" ON E.COMPANY_NO = EEC.COMPANY_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EEC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND E.EARNING_NO = EEC.EARNING_NO");
            strQry.AppendLine(" AND EEC.RUN_TYPE = 'P'");

            strQry.AppendLine(" WHERE EE.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EE.EMPLOYEE_NO = 0 ");
            strQry.AppendLine(" AND EE.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 2");
            strQry.AppendLine(",E.EARNING_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EarningSelected", parInt64CompanyNo);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Insert_New_Record(Int64 parInt64CurrentUserNo, byte[] parbyteDataSet)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            StringBuilder strQry = new StringBuilder();
          
            byte[] bytCompress;
            int intEarningNo = -1;
            Int64 Int64CompanyNo = Convert.ToInt64(parDataSet.Tables["EarningSelected"].Rows[0]["COMPANY_NO"]);

            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" 0 AS RETURN_CODE ");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY ");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["EarningSelected"].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Check", Int64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" EARNING_NO ");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT ");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables[0].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
            //Not a Take-On
            strQry.AppendLine(" AND PAY_CATEGORY_NO > 0 ");
            strQry.AppendLine(" AND RUN_TYPE = 'P'");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", Int64CompanyNo);

            if (DataSet.Tables["Temp"].Rows.Count > 0)
            {
                DataSet.Tables["Check"].Rows[0]["RETURN_CODE"] = 9999;
                goto Insert_New_Record_Continue;
            }

            DataSet.Tables.Remove("Temp");

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" MAX(EARNING_NO) AS MAX_NO");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["EarningSelected"].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine(" AND EARNING_NO > 149");
            //Leave Range
            strQry.AppendLine(" AND EARNING_NO < 200");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", Int64CompanyNo);

            if (DataSet.Tables["Temp"].Rows[0]["MAX_NO"] == System.DBNull.Value)
            {
                intEarningNo = 150;
            }
            else
            {
                intEarningNo = Convert.ToInt32(DataSet.Tables["Temp"].Rows[0]["MAX_NO"]) + 1;
            }

            parDataSet.Tables["EarningSelected"].Rows[0]["EARNING_NO"] = intEarningNo;
            parDataSet.Tables["EarningSelected"].Rows[0]["EARNING_TYPE_DEFAULT"] = "Y";
            
            //Returned DataTable
            DataSet.Tables.Add(parDataSet.Tables["EarningSelected"].Clone());

            DataSet.Tables["EarningSelected"].ImportRow(parDataSet.Tables["EarningSelected"].Rows[0]);

            strQry.Clear();

            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EARNING");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(",TIE_BREAKER");
            strQry.AppendLine(",EARNING_DESC");
            strQry.AppendLine(",IRP5_CODE");
            strQry.AppendLine(",LEAVE_PERCENTAGE");
            strQry.AppendLine(",EARNING_REPORT_HEADER1");
            strQry.AppendLine(",EARNING_REPORT_HEADER2");
            strQry.AppendLine(",USER_NO_NEW_RECORD");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",EARNING_DEL_IND)");
            strQry.AppendLine(" VALUES");
            strQry.AppendLine("(" + parDataSet.Tables["EarningSelected"].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EarningSelected"].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
            strQry.AppendLine("," + parDataSet.Tables["EarningSelected"].Rows[0]["EARNING_NO"].ToString());
            strQry.AppendLine(",1");
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EarningSelected"].Rows[0]["EARNING_DESC"].ToString()));
            strQry.AppendLine("," + Convert.ToInt32(parDataSet.Tables["EarningSelected"].Rows[0]["IRP5_CODE"]));
            strQry.AppendLine(",100");
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EarningSelected"].Rows[0]["EARNING_REPORT_HEADER1"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EarningSelected"].Rows[0]["EARNING_REPORT_HEADER2"].ToString()));
            strQry.AppendLine("," + parInt64CurrentUserNo);
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine(",'Y')");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Int64CompanyNo);
            
            //Create For Opposite PAY_CATEGORY_TYPE
            strQry.Clear();

            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EARNING");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(",TIE_BREAKER");
            strQry.AppendLine(",EARNING_DESC");
            strQry.AppendLine(",IRP5_CODE");
            strQry.AppendLine(",LEAVE_PERCENTAGE");
            strQry.AppendLine(",EARNING_REPORT_HEADER1");
            strQry.AppendLine(",EARNING_REPORT_HEADER2");
            strQry.AppendLine(",USER_NO_NEW_RECORD");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",EARNING_DEL_IND)");
            strQry.AppendLine(" VALUES");
            strQry.AppendLine("(" + parDataSet.Tables["EarningSelected"].Rows[0]["COMPANY_NO"].ToString());

            if (parDataSet.Tables["EarningSelected"].Rows[0]["PAY_CATEGORY_TYPE"].ToString() == "W")
            {
                strQry.AppendLine(",'S'");
            }
            else
            {
                strQry.AppendLine(",'W'");
            }

            strQry.AppendLine("," + parDataSet.Tables["EarningSelected"].Rows[0]["EARNING_NO"].ToString());
            strQry.AppendLine(",1");
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EarningSelected"].Rows[0]["EARNING_DESC"].ToString()));
            strQry.AppendLine("," + Convert.ToInt32(parDataSet.Tables["EarningSelected"].Rows[0]["IRP5_CODE"]));
            strQry.AppendLine(",100");
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EarningSelected"].Rows[0]["EARNING_REPORT_HEADER1"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EarningSelected"].Rows[0]["EARNING_REPORT_HEADER2"].ToString()));
            strQry.AppendLine("," + parInt64CurrentUserNo);
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine(",'Y')");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Int64CompanyNo);
            
            strQry.Clear();

            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(",TIE_BREAKER");

            strQry.AppendLine(",AMOUNT");
            strQry.AppendLine(",EARNING_TYPE_IND");
            strQry.AppendLine(",EARNING_PERIOD_IND");
            strQry.AppendLine(",EARNING_DAY_VALUE");

             strQry.AppendLine(",USER_NO_NEW_RECORD");
            strQry.AppendLine(",DATETIME_NEW_RECORD)");

            strQry.AppendLine(" VALUES");
            strQry.AppendLine("(" + parDataSet.Tables["EarningSelected"].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EarningSelected"].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));

            //Default EMPLOYEE_NO
            strQry.AppendLine(",0");
            strQry.AppendLine("," + parDataSet.Tables["EarningSelected"].Rows[0]["EARNING_NO"].ToString());
            strQry.AppendLine(",1");

            strQry.AppendLine("," + parDataSet.Tables["EarningSelected"].Rows[0]["AMOUNT"].ToString());
            
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EarningSelected"].Rows[0]["EARNING_TYPE_IND"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EarningSelected"].Rows[0]["EARNING_PERIOD_IND"].ToString()));
            strQry.AppendLine("," + parDataSet.Tables["EarningSelected"].Rows[0]["EARNING_DAY_VALUE"].ToString());

            strQry.AppendLine("," + parInt64CurrentUserNo);
            strQry.AppendLine(",GETDATE())");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Int64CompanyNo);
            
            strQry.Clear();

            //Create For Opposite PAY_CATEGORY_TYPE
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(",TIE_BREAKER");

            strQry.AppendLine(",AMOUNT");
            strQry.AppendLine(",EARNING_TYPE_IND");
            strQry.AppendLine(",EARNING_PERIOD_IND");
            strQry.AppendLine(",EARNING_DAY_VALUE");

            strQry.AppendLine(",USER_NO_NEW_RECORD");
            strQry.AppendLine(",DATETIME_NEW_RECORD)");

            strQry.AppendLine(" VALUES");
            strQry.AppendLine("(" + parDataSet.Tables["EarningSelected"].Rows[0]["COMPANY_NO"].ToString());

            if (parDataSet.Tables["EarningSelected"].Rows[0]["PAY_CATEGORY_TYPE"].ToString() == "W")
            {
                strQry.AppendLine(",'S'");
            }
            else
            {
                strQry.AppendLine(",'W'");
            }

            //Default EMPLOYEE_NO
            strQry.AppendLine(",0");
            strQry.AppendLine("," + parDataSet.Tables["EarningSelected"].Rows[0]["EARNING_NO"].ToString());
            strQry.AppendLine(",1");

            strQry.AppendLine(",0");
            strQry.AppendLine(",'U'");
            strQry.AppendLine(",'E'");
            strQry.AppendLine(",0");

            strQry.AppendLine("," + parInt64CurrentUserNo);
            strQry.AppendLine(",GETDATE())");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Int64CompanyNo);

            //Create Record for Opposite PAY_CATEGORY_TYPE
            DataRow myDataRow = DataSet.Tables["EarningSelected"].NewRow();

            myDataRow["COMPANY_NO"] = parDataSet.Tables["EarningSelected"].Rows[0]["COMPANY_NO"].ToString();

            if (parDataSet.Tables["EarningSelected"].Rows[0]["PAY_CATEGORY_TYPE"].ToString() == "W")
            {
                myDataRow["PAY_CATEGORY_TYPE"] = "S";
            }
            else
            {
                myDataRow["PAY_CATEGORY_TYPE"] = "W";
            }

            myDataRow["EMPLOYEE_NO"] = 0;
            myDataRow["EARNING_NO"] = Convert.ToInt16(parDataSet.Tables["EarningSelected"].Rows[0]["EARNING_NO"]);
            myDataRow["EARNING_DESC"] = parDataSet.Tables["EarningSelected"].Rows[0]["EARNING_DESC"].ToString();
            myDataRow["AMOUNT"] = 0;

            myDataRow["IRP5_CODE"] = parDataSet.Tables["EarningSelected"].Rows[0]["IRP5_CODE"].ToString();
            myDataRow["EARNING_REPORT_HEADER1"] = parDataSet.Tables["EarningSelected"].Rows[0]["EARNING_REPORT_HEADER1"].ToString();
            myDataRow["EARNING_REPORT_HEADER2"] = parDataSet.Tables["EarningSelected"].Rows[0]["EARNING_REPORT_HEADER2"].ToString();
            myDataRow["EARNING_TYPE_IND"] = "U";

            myDataRow["EARNING_TYPE_DEFAULT"] = "Y";
            myDataRow["EARNING_PERIOD_IND"] = "E";
            myDataRow["EARNING_DAY_VALUE"] = 0;
            myDataRow["EARNING_DEL_IND"] = "Y";
            myDataRow["TIE_BREAKER"] = 1;

            DataSet.Tables["EarningSelected"].Rows.Add(myDataRow);
            
        Insert_New_Record_Continue:

            DataSet.Tables.Remove("Temp");

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + Int64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            DataSet.AcceptChanges();

            bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;
            parDataSet.Dispose();
            parDataSet = null;

            return bytCompress;
        }

        public void Delete_Record(Int64 parInt64CurrentUserNo, Int64 parInt64CompanyNo, string parstrPayCategoryType, int parintEarningNo)
        {
            StringBuilder strQry = new StringBuilder();
  
            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EARNING");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" USER_NO_RECORD = " + parInt64CurrentUserNo);
            strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
            strQry.AppendLine(" AND EARNING_NO = " + parintEarningNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" USER_NO_RECORD = " + parInt64CurrentUserNo);
            strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
            strQry.AppendLine(" AND EARNING_NO = " + parintEarningNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }

        public byte[] Update_Record(Int64 parInt64CurrentUserNo, Int64 parInt64CompanyNo, string parstrPayCategoryType, byte[] parbyteDataSet)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            StringBuilder strQry = new StringBuilder();
            byte[] bytCompress;
            DataSet DataSet = new DataSet();

            int intTieBreaker = 0;

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" 0 AS RETURN_CODE ");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY ");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Check", parInt64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" EARNING_NO ");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT ");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));

            //Not a Take-On
            strQry.AppendLine(" AND PAY_CATEGORY_NO > 0 ");
            strQry.AppendLine(" AND RUN_TYPE = 'P'");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

            if (DataSet.Tables["Temp"].Rows.Count > 0)
            {
                DataSet.Tables["Check"].Rows[0]["RETURN_CODE"] = 9999;
                goto Update_Record_Continue;
            }

            //Clone DataTable
            DataSet.Tables.Add(parDataSet.Tables["EarningSelected"].Clone());

            for (int intRow = 0; intRow < parDataSet.Tables["EarningSelected"].Rows.Count; intRow++)
            {
                if (DataSet.Tables["Temp"] != null)
                {
                    DataSet.Tables.Remove("Temp");
                }

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" MAX(TIE_BREAKER) AS MAX_TIE_BREAKER ");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING ");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
                strQry.AppendLine(" AND EARNING_NO = " + parDataSet.Tables["EarningSelected"].Rows[intRow]["EARNING_NO"].ToString());

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

                if (DataSet.Tables["Temp"].Rows[0]["MAX_TIE_BREAKER"] == System.DBNull.Value)
                {
                    intTieBreaker = 1;
                }
                else
                {
                    intTieBreaker = Convert.ToInt32(DataSet.Tables["Temp"].Rows[0]["MAX_TIE_BREAKER"]) + 1;
                }

                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EARNING");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EARNING_NO");
                strQry.AppendLine(",TIE_BREAKER");
                strQry.AppendLine(",EARNING_DESC");
                strQry.AppendLine(",IRP5_CODE");
                strQry.AppendLine(",LEAVE_PERCENTAGE");
                strQry.AppendLine(",EARNING_REPORT_HEADER1");
                strQry.AppendLine(",EARNING_REPORT_HEADER2");
                strQry.AppendLine(",USER_NO_NEW_RECORD");
                strQry.AppendLine(",DATETIME_NEW_RECORD");
                strQry.AppendLine(",EARNING_DEL_IND)");
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(parInt64CompanyNo.ToString());
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
                strQry.AppendLine("," + parDataSet.Tables["EarningSelected"].Rows[intRow]["EARNING_NO"].ToString());
                strQry.AppendLine("," + intTieBreaker);
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EarningSelected"].Rows[intRow]["EARNING_DESC"].ToString()));
                strQry.AppendLine("," + Convert.ToInt32(parDataSet.Tables["EarningSelected"].Rows[intRow]["IRP5_CODE"]));
                strQry.AppendLine(",100");
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EarningSelected"].Rows[intRow]["EARNING_REPORT_HEADER1"].ToString()));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EarningSelected"].Rows[intRow]["EARNING_REPORT_HEADER2"].ToString()));
                strQry.AppendLine("," + parInt64CurrentUserNo);
                strQry.AppendLine(",GETDATE()");
                strQry.AppendLine(",'Y'");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C ");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EN");
                strQry.AppendLine(" ON C.COMPANY_NO = EN.COMPANY_NO");
                strQry.AppendLine(" AND EN.EARNING_NO = " + parDataSet.Tables["EarningSelected"].Rows[intRow]["EARNING_NO"].ToString());
                strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
                strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL ");

                strQry.AppendLine(" WHERE C.COMPANY_NO = " + parInt64CompanyNo);
                //Earning Record Does Not Exist
                strQry.AppendLine(" AND EN.COMPANY_NO IS NULL ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                DataSet.Tables.Remove("Temp");

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" MAX(ISNULL(TIE_BREAKER,0)) + 1 AS MAX_NO");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
                strQry.AppendLine(" AND EMPLOYEE_NO = 0 ");
                strQry.AppendLine(" AND EARNING_NO = " + Convert.ToInt32(parDataSet.Tables["EarningSelected"].Rows[intRow]["EARNING_NO"]));
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

                if (DataSet.Tables["Temp"].Rows[0]["MAX_NO"] == System.DBNull.Value)
                {
                    intTieBreaker = 1;

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",EARNING_NO");
                    strQry.AppendLine(",TIE_BREAKER");

                    strQry.AppendLine(",AMOUNT");

                    strQry.AppendLine(",EARNING_TYPE_IND");
                    strQry.AppendLine(",EARNING_PERIOD_IND");
                    strQry.AppendLine(",EARNING_DAY_VALUE");
                    strQry.AppendLine(",DATETIME_NEW_RECORD");
                    strQry.AppendLine(",USER_NO_NEW_RECORD)");
                    strQry.AppendLine(" VALUES");
                    strQry.AppendLine("(" + parInt64CompanyNo);
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
                    strQry.AppendLine(",0");
                    strQry.AppendLine("," + Convert.ToInt32(parDataSet.Tables["EarningSelected"].Rows[intRow]["EARNING_NO"]));
                    strQry.AppendLine("," + Convert.ToInt32(parDataSet.Tables["EarningSelected"].Rows[intRow]["TIE_BREAKER"]));

                    strQry.AppendLine("," + parDataSet.Tables["EarningSelected"].Rows[intRow]["AMOUNT"].ToString());

                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EarningSelected"].Rows[intRow]["EARNING_TYPE_IND"].ToString()));

                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EarningSelected"].Rows[intRow]["EARNING_PERIOD_IND"].ToString()));
                    strQry.AppendLine("," + Convert.ToDouble(parDataSet.Tables["EarningSelected"].Rows[intRow]["EARNING_DAY_VALUE"]));
                    strQry.AppendLine(",GETDATE()");
                    strQry.AppendLine("," + parInt64CurrentUserNo + ")");

                    DataSet.Tables["EarningSelected"].ImportRow(parDataSet.Tables["EarningSelected"].Rows[intRow]);

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                }
                else
                {
                    intTieBreaker = Convert.ToInt32(DataSet.Tables["Temp"].Rows[0]["MAX_NO"]);

                    //Update All Records Including Employees Linked to Master
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE EE");
                    strQry.AppendLine(" SET ");

                    strQry.AppendLine(" AMOUNT = " + parDataSet.Tables["EarningSelected"].Rows[intRow]["AMOUNT"].ToString());

                    strQry.AppendLine(",EARNING_PERIOD_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EarningSelected"].Rows[intRow]["EARNING_PERIOD_IND"].ToString()));
                    strQry.AppendLine(",EARNING_DAY_VALUE = " + Convert.ToInt32(parDataSet.Tables["EarningSelected"].Rows[intRow]["EARNING_DAY_VALUE"]));
                    strQry.AppendLine(",EARNING_TYPE_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EarningSelected"].Rows[intRow]["EARNING_TYPE_IND"].ToString()));
                    strQry.AppendLine(",USER_NO_RECORD = " + parInt64CurrentUserNo);

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EE");

                    strQry.AppendLine(" INNER JOIN ");

                    strQry.AppendLine("(SELECT ");

                    strQry.AppendLine(" AMOUNT");
                    strQry.AppendLine(",EARNING_PERIOD_IND");
                    strQry.AppendLine(",EARNING_DAY_VALUE");
                    strQry.AppendLine(",EARNING_TYPE_IND");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + Convert.ToInt32(parDataSet.Tables["EarningSelected"].Rows[intRow]["EMPLOYEE_NO"]));
                    strQry.AppendLine(" AND EARNING_NO = " + Convert.ToInt32(parDataSet.Tables["EarningSelected"].Rows[intRow]["EARNING_NO"]));
                    //2017-02-24
                    //strQry.AppendLine(" AND TIE_BREAKER = " + Convert.ToInt32(parDataSet.Tables["EarningSelected"].Rows[intRow]["TIE_BREAKER"]));
                    strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL) AS MASTER_TABLE");

                    strQry.AppendLine(" ON EE.AMOUNT = MASTER_TABLE.AMOUNT");
                    strQry.AppendLine(" AND EE.EARNING_PERIOD_IND = MASTER_TABLE.EARNING_PERIOD_IND");
                    strQry.AppendLine(" AND EE.EARNING_DAY_VALUE = MASTER_TABLE.EARNING_DAY_VALUE");
                    strQry.AppendLine(" AND EE.EARNING_TYPE_IND = MASTER_TABLE.EARNING_TYPE_IND");

                    strQry.AppendLine(" WHERE EE.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EE.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
                    strQry.AppendLine(" AND EE.EARNING_NO = " + Convert.ToInt32(parDataSet.Tables["EarningSelected"].Rows[intRow]["EARNING_NO"]));
                    strQry.AppendLine(" AND EE.DATETIME_DELETE_RECORD IS NULL");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    DataSet.Tables["EarningSelected"].ImportRow(parDataSet.Tables["EarningSelected"].Rows[intRow]);

                    if (parDataSet.Tables["EarningSelected"].Rows[intRow]["EARNING_TYPE_DEFAULT"].ToString() == "N")
                    {
                        strQry.Clear();

                        strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EARNING");
                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" EARNING_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EarningSelected"].Rows[intRow]["EARNING_DESC"].ToString()));
                        strQry.AppendLine(",IRP5_CODE = " + Convert.ToInt32(parDataSet.Tables["EarningSelected"].Rows[intRow]["IRP5_CODE"]));
                        strQry.AppendLine(",EARNING_REPORT_HEADER1 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EarningSelected"].Rows[intRow]["EARNING_REPORT_HEADER1"].ToString()));
                        strQry.AppendLine(",EARNING_REPORT_HEADER2 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EarningSelected"].Rows[intRow]["EARNING_REPORT_HEADER2"].ToString()));
                        strQry.AppendLine(",USER_NO_RECORD = " + parInt64CurrentUserNo);
                        strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
                        strQry.AppendLine(" AND EARNING_NO = " + Convert.ToInt32(parDataSet.Tables["EarningSelected"].Rows[intRow]["EARNING_NO"]));
                        strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                    }
                }
            }

        Update_Record_Continue:

            DataSet.Tables.Remove("Temp");

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            DataSet.AcceptChanges();

            bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;
            parDataSet.Dispose();
            parDataSet = null;

            return bytCompress;
        }
    }
}
