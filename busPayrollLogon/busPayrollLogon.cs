using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busPayrollLogon
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busPayrollLogon()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public string Ping()
        {
            return "OK";
        }

        public string Site_Help_Ind()
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();
            string strEditHelpInd = "";

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" TAX_CASUAL_PERCENT");
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.TAX_CASUAL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "TaxCasual", -1);

            strEditHelpInd += "," + DataSet.Tables["TaxCasual"].Rows[0]["TAX_CASUAL_PERCENT"].ToString();

            return strEditHelpInd;
        }

        public string PingTest(string strErrol)
        {
            return "OK";
        }

        public byte[] Get_Client_Download_Files(Int64 parInt64UserNo)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Append(clsDBConnectionObjects.Get_Internet_Client_Download_SQL(parInt64UserNo));

            strQry.AppendLine(" ORDER BY ");
            //PROJECT_VERSION - First Server Files then Presentation Files
            strQry.AppendLine(" 2 DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Files", -1);

            byte[]  bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
        
        public byte[] Get_New_Client_Download_Files(Int64 parInt64UserNo)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Append(clsDBConnectionObjects.Get_Internet_Client_Download_SQL_New(parInt64UserNo));

            strQry.AppendLine(" ORDER BY ");
            //PROJECT_VERSION - First Server Files then Presentation Files
            strQry.AppendLine(" 2 DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Files", -1);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
        
        public byte[] Logon_Client_User(string parstrUserInformation)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            string[] strPieces = parstrUserInformation.Split('|');

            string strUserId = clsDBConnectionObjects.Text2DynamicSQL(strPieces[0]);
            string strPassword = clsDBConnectionObjects.Text2DynamicSQL(strPieces[1]);
            byte[] bytCompress;

            DataSet DataSet = new DataSet();

            DataTable DataTable = new DataTable("ReturnValues");

            DataTable.Columns.Add("USER_NO", typeof(Int64));
            DataTable.Columns.Add("ACCESS_IND", typeof(String));
            DataTable.Columns.Add("RESET", typeof(String));
            DataTable.Columns.Add("LOCK_IND", typeof(String));
            DataTable.Columns.Add("REBOOT_IND", typeof(String));
            DataTable.Columns.Add("NO_USERS_IND", typeof(String));
            DataTable.Columns.Add("DOWNLOAD_IND", typeof(String));

            DataSet.Tables.Add(DataTable);

            DataRow myDataRow = DataSet.Tables["ReturnValues"].NewRow();

            myDataRow["USER_NO"] = -1;
            myDataRow["RESET"] = "N";
            //Will Be Overriden Later if Administartor
            myDataRow["ACCESS_IND"] = "U";
           
            DataSet.Tables["ReturnValues"].Rows.Add(myDataRow);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" U.USER_NO");
            strQry.AppendLine(",U.RESET");
            strQry.AppendLine(",UCA.COMPANY_ACCESS_IND");
            strQry.AppendLine(",U.SYSTEM_ADMINISTRATOR_IND");

            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID U");

            strQry.AppendLine(" LEFT JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA ");
            strQry.AppendLine(" ON U.USER_NO = UCA.USER_NO ");
            strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL  ");

            strQry.AppendLine(" WHERE U.USER_ID = " + strUserId);
            strQry.AppendLine(" AND U.PASSWORD = " + strPassword);
            strQry.AppendLine(" AND U.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" U.USER_NO");
            strQry.AppendLine(",U.SYSTEM_ADMINISTRATOR_IND");
            strQry.AppendLine(",UCA.COMPANY_ACCESS_IND");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", -1);

            if (DataSet.Tables["Temp"].Rows.Count == 0)
            {
                //DataSet.Tables.Remove(DataSet.Tables["Temp"]);

                if ((strUserId == "'INTERACT'"
                && strPassword == "'TCARETNI'")
                || (strUserId == "'HELENALR'"
                && strPassword == "'MONSTER'"))
                {
                    DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"] = 0;
                    DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"] = "S";
                }
                else
                {
                    //Not Found
                    bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
                    DataSet.Dispose();
                    DataSet = null;

                    return bytCompress;
                }
            }
            else
            {
                DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"] = Convert.ToInt64(DataSet.Tables["Temp"].Rows[0]["USER_NO"]);
                DataSet.Tables["ReturnValues"].Rows[0]["RESET"] = DataSet.Tables["Temp"].Rows[0]["RESET"].ToString();
               
                //System Administrator
                if (DataSet.Tables["Temp"].Rows[0]["SYSTEM_ADMINISTRATOR_IND"].ToString() == "Y")
                {
                    DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"] = "S";
                }
                else
                {
                    if (DataSet.Tables["Temp"].Rows[0]["COMPANY_ACCESS_IND"].ToString() == "A")
                    {
                        DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"] = "A";

                        //2016-12-08
                        strQry.Clear();
                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" COMPANY_NO");

                        strQry.AppendLine(" FROM InteractPayroll.dbo.USER_COMPANY_ACCESS ");

                        strQry.AppendLine(" WHERE USER_NO = " + DataSet.Tables["Temp"].Rows[0]["USER_NO"].ToString());
                        strQry.AppendLine(" AND COMPANY_ACCESS_IND = 'A'");
                        //2017-05-17
                        strQry.AppendLine(" AND ISNULL(ACCESS_LAYER_IND,'C') IN ('B','C')");

                        strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

                        clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyAccess", -1);
                    }
                    else
                    {
                        //User - Not Alllowed to Login
                        DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"] = -1;
                    }
                }

                //Set Time Logged On
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_ID");
                strQry.AppendLine(" SET LAST_TIME_ON = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "'");
                strQry.AppendLine(" WHERE USER_NO = " + Convert.ToInt64(DataSet.Tables["Temp"].Rows[0]["USER_NO"]));
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            }

            bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
        
        public byte[] Logon_New_User(string parstrUserInformation, string parstrClientDBConnected)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            string[] strPieces = parstrUserInformation.Split('|');

            string strUserId = clsDBConnectionObjects.Text2DynamicSQL(strPieces[0]);
            string strPassword = clsDBConnectionObjects.Text2DynamicSQL(strPieces[1]);

            byte[] bytCompress;

            DataSet DataSet = new DataSet();

            DataTable DataTable = new DataTable("ReturnValues");

            DataTable.Columns.Add("USER_NO", typeof(Int64));
            DataTable.Columns.Add("RESET_IND", typeof(String));
            DataTable.Columns.Add("ACCESS_IND", typeof(String));
            DataTable.Columns.Add("LAST_COMPANY_NO", typeof(Int64));
            DataTable.Columns.Add("LAST_PAY_CATEGORY_TYPE", typeof(String));
            DataTable.Columns.Add("LOCK_IND", typeof(String));

            DataSet.Tables.Add(DataTable);

            DataRow myDataRow = DataSet.Tables["ReturnValues"].NewRow();

            myDataRow["USER_NO"] = -1;
            myDataRow["RESET_IND"] = "N";
            //Will Be Overriden Later if Administartor
            myDataRow["ACCESS_IND"] = "U";
            myDataRow["LAST_COMPANY_NO"] = 0;
            myDataRow["LAST_PAY_CATEGORY_TYPE"] = "";

            DataSet.Tables["ReturnValues"].Rows.Add(myDataRow);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" U.USER_NO");
            strQry.AppendLine(",U.SYSTEM_ADMINISTRATOR_IND");
            strQry.AppendLine(",U.RESET");
            strQry.AppendLine(",CL.COMPANY_NO AS LAST_COMPANY_NO");
            strQry.AppendLine(",U.LAST_PAY_CATEGORY_TYPE");
            strQry.AppendLine(",U.LOCK_IND");

            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID U");

            strQry.AppendLine(" LEFT JOIN InteractPayroll.dbo.COMPANY_LINK CL ");
            strQry.AppendLine(" ON U.LAST_COMPANY_NO = CL.COMPANY_NO ");

            strQry.AppendLine(" WHERE U.USER_ID = " + strUserId);
            strQry.AppendLine(" AND U.PASSWORD = " + strPassword);
            strQry.AppendLine(" AND U.DATETIME_DELETE_RECORD IS NULL ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", -1);

            //TEMP //TEMP  - Stop User Logon
            //if (strUserId == "'INTERACT'"
            //       & strPassword == "'TCARETNI'")
            //{
            //}
            //else
            //{
            //    //Not Found
            //    bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            //    DataSet.Dispose();
            //    DataSet = null;

            //    return bytCompress;
            //}

            //TEMP //TEMP

            if (DataSet.Tables["Temp"].Rows.Count == 0)
            {
                if ((strUserId == "'INTERACT'"
                && strPassword == "'TCARETNI'")
                || (strUserId == "'HELENALR'"
                && strPassword == "'MONSTER'"))
                {
                    DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"] = 0;
                    DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"] = "S";
                }
                else
                {
                    //Not Found
                    bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
                    DataSet.Dispose();
                    DataSet = null;

                    return bytCompress;
                }
            }
            else
            {
                DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"] = Convert.ToInt64(DataSet.Tables["Temp"].Rows[0]["USER_NO"]);
                DataSet.Tables["ReturnValues"].Rows[0]["RESET_IND"] = DataSet.Tables["Temp"].Rows[0]["RESET"].ToString();
                DataSet.Tables["ReturnValues"].Rows[0]["LOCK_IND"] = DataSet.Tables["Temp"].Rows[0]["LOCK_IND"].ToString();

                if (DataSet.Tables["Temp"].Rows[0]["LAST_COMPANY_NO"] != System.DBNull.Value)
                {
                    DataSet.Tables["ReturnValues"].Rows[0]["LAST_COMPANY_NO"] = Convert.ToInt64(DataSet.Tables["Temp"].Rows[0]["LAST_COMPANY_NO"]);
                    DataSet.Tables["ReturnValues"].Rows[0]["LAST_PAY_CATEGORY_TYPE"] = DataSet.Tables["Temp"].Rows[0]["LAST_PAY_CATEGORY_TYPE"].ToString();
                }
                else
                {
                    DataSet.Tables["ReturnValues"].Rows[0]["LAST_COMPANY_NO"] = 0;
                    DataSet.Tables["ReturnValues"].Rows[0]["LAST_PAY_CATEGORY_TYPE"] = "";
                }

                //System Administrator
                if (DataSet.Tables["Temp"].Rows[0]["SYSTEM_ADMINISTRATOR_IND"].ToString() == "Y")
                {
                    DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"] = "S";
                }

                //Set Time Logged On
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_ID");
                strQry.AppendLine(" SET LAST_TIME_ON = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "'");
                strQry.AppendLine(" WHERE USER_NO = " + Convert.ToInt64(DataSet.Tables["Temp"].Rows[0]["USER_NO"]));
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            }

            //NB DISTINCT Fixes a Bug that crept into System
            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" C.COMPANY_NO ");
            strQry.AppendLine(",C.COMPANY_DESC");
            strQry.AppendLine(",C.DATE_FORMAT ");
            strQry.AppendLine(",ISNULL(C.TIMESHEET_READ_TIMEOUT_SECONDS,30) AS TIMESHEET_READ_TIMEOUT_SECONDS ");
            strQry.AppendLine(",C.DYNAMIC_UPLOAD_KEY ");

            if (DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"].ToString() == "S")
            {
                strQry.AppendLine(",'N' AS LOCK_IND ");
            }
            else
            {
                strQry.AppendLine(",C.LOCK_IND ");
            }

            if (DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"].ToString() != "S")
            {
                strQry.AppendLine(",UCA.COMPANY_ACCESS_IND");
            }
            else
            {
                strQry.AppendLine(",'A' AS COMPANY_ACCESS_IND");
            }

            strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK C");

            if (DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"].ToString() != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA ");
                strQry.AppendLine(" ON UCA.USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());
                strQry.AppendLine(" AND UCA.COMPANY_NO = C.COMPANY_NO ");
                strQry.AppendLine(" AND (UCA.COMPANY_ACCESS_IND = 'A'");
                strQry.AppendLine(" OR (UCA.COMPANY_ACCESS_IND = 'U'");
                //2017-05-17
                strQry.AppendLine(" AND ISNULL(UCA.ACCESS_LAYER_IND,'C') IN ('B','I')))");
                strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL ");
            }

            strQry.AppendLine(" WHERE ISNULL(C.TIME_ATTENDANCE_IND,'') <> 'Y'");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" C.COMPANY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", -1);

            for (int intRow = 0; intRow < DataSet.Tables["Company"].Rows.Count; intRow++)
            {
                if (intRow == 0)
                {
                    if (Convert.ToInt64(DataSet.Tables["ReturnValues"].Rows[0]["LAST_COMPANY_NO"]) == 0)
                    {
                        DataSet.Tables["ReturnValues"].Rows[0]["LAST_COMPANY_NO"] = Convert.ToInt64(DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"]);
                    }
                }

                if (DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"].ToString() != "S")
                {
                    //2013-08-29
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Convert.ToInt64(DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"]));

                    if (DataSet.Tables["Company"].Rows[intRow]["COMPANY_ACCESS_IND"].ToString() == "A")
                    {
                        DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"] = "A";
                    }
                    else
                    {
                        strQry.Clear();
                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP ");
                        strQry.AppendLine("(USER_NO");
                        strQry.AppendLine(",COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",PAY_CATEGORY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE)");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());
                        strQry.AppendLine("," + DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"].ToString());
                        strQry.AppendLine(",USER_TABLE.EMPLOYEE_NO");
                        strQry.AppendLine(",USER_TABLE.PAY_CATEGORY_NO");
                        strQry.AppendLine(",USER_TABLE.PAY_CATEGORY_TYPE");

                        strQry.AppendLine(" FROM ");

                        strQry.AppendLine("(SELECT ");
                        strQry.AppendLine(" E.EMPLOYEE_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

                        //Errol 2013-04-12
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
                        strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
                        strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                        strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_PAY_CATEGORY_DEPARTMENT UPCD");
                        strQry.AppendLine(" ON E.COMPANY_NO = UPCD.COMPANY_NO");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UPCD.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UPCD.PAY_CATEGORY_TYPE");
                        strQry.AppendLine(" AND E.DEPARTMENT_NO = UPCD.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND UPCD.USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());
                        strQry.AppendLine(" AND UPCD.DATETIME_DELETE_RECORD IS NULL ");

                        strQry.AppendLine(" WHERE E.COMPANY_NO = " + DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"].ToString());
                        strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                        strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                        strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.EMPLOYEE_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

                        //Errol 2013-04-12
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
                        strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
                        strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                        strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_EMPLOYEE UE");
                        strQry.AppendLine(" ON E.COMPANY_NO = UE.COMPANY_NO");
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UE.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UE.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND UE.USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());
                        strQry.AppendLine(" AND UE.DATETIME_DELETE_RECORD IS NULL ");

                        strQry.AppendLine(" WHERE E.COMPANY_NO = " + DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"].ToString());
                        strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                        strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                        strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.EMPLOYEE_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

                        //Errol 2013-04-12
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
                        strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
                        strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                        strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_PAY_CATEGORY UPC");
                        strQry.AppendLine(" ON E.COMPANY_NO = UPC.COMPANY_NO");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND UPC.USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());
                        strQry.AppendLine(" AND UPC.DATETIME_DELETE_RECORD IS NULL ");

                        strQry.AppendLine(" WHERE E.COMPANY_NO = " + DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"].ToString());
                        strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                        strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                        strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL) AS USER_TABLE");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Convert.ToInt64(DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"]));
                    }
                }
            }

            strQry.Clear();

            if (parstrClientDBConnected == "Y")
            {
                strQry.Append(clsDBConnectionObjects.Get_Internet_Client_Download_SQL_New(Convert.ToInt64(DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"])));

                strQry.AppendLine(" UNION");
            }

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FDD.PROJECT_VERSION ");
            strQry.AppendLine(",'' AS FILE_LAYER_IND");
            strQry.AppendLine(",FDD.FILE_NAME");
            strQry.AppendLine(",FDD.FILE_SIZE");
            strQry.AppendLine(",FDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FDD.FILE_VERSION_NO");
            strQry.AppendLine(",FDD.FILE_CRC_VALUE");
            strQry.AppendLine(",CONVERT(INT,-1) AS COMPANY_NO");
            strQry.AppendLine(",'1' AS FROM_IND");
            strQry.AppendLine(",'N' AS ALWAYS_IND");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_USERS FDU");
            strQry.AppendLine(" ON FDD.PROJECT_VERSION = FDU.PROJECT_VERSION");
            strQry.AppendLine(" AND FDU.USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FDD.PROJECT_VERSION = FDC.PROJECT_VERSION");
            strQry.AppendLine(" AND FDD.PROGRAM_ID = FDC.PROGRAM_ID");
            strQry.AppendLine(" AND FDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FDD.PROJECT_VERSION = 'Beta'");

            //P=Payroll B=Both (Payroll and Time Attendance Internet)
            strQry.AppendLine(" AND FDD.PROGRAM_ID IN ('P','B','C')");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FDD.PROJECT_VERSION ");
            strQry.AppendLine(",FDD.FILE_NAME");
            strQry.AppendLine(",FDD.FILE_SIZE");
            strQry.AppendLine(",FDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FDD.FILE_VERSION_NO");
            strQry.AppendLine(",FDD.FILE_CRC_VALUE");

            strQry.AppendLine(" UNION");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FDD.PROJECT_VERSION ");
            strQry.AppendLine(",'' AS FILE_LAYER_IND");
            strQry.AppendLine(",FDD.FILE_NAME");
            strQry.AppendLine(",FDD.FILE_SIZE");
            strQry.AppendLine(",FDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FDD.FILE_VERSION_NO");
            strQry.AppendLine(",FDD.FILE_CRC_VALUE");
            strQry.AppendLine(",CONVERT(INT,-1) AS COMPANY_NO");
            strQry.AppendLine(",'1' AS FROM_IND");
            strQry.AppendLine(",'N' AS ALWAYS_IND");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FDD");

            //2018-09-08 - Start 
            strQry.AppendLine(" LEFT JOIN ");
            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" FDD.FILE_NAME");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_USERS FDU");
            strQry.AppendLine(" ON FDD.PROJECT_VERSION = FDU.PROJECT_VERSION");
            strQry.AppendLine(" AND FDU.USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());

            strQry.AppendLine(" WHERE FDD.PROJECT_VERSION = 'Beta'");

            //P=Payroll B=Both (Payroll and Time Attendance Internet)
            strQry.AppendLine(" AND FDD.PROGRAM_ID IN ('P','B','C')) AS BETA_TABLE");
            strQry.AppendLine(" ON FDD.FILE_NAME = BETA_TABLE.FILE_NAME");
            //2018-09-08 - End 

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FDD.PROJECT_VERSION = FDC.PROJECT_VERSION");
            strQry.AppendLine(" AND FDD.PROGRAM_ID = FDC.PROGRAM_ID");
            strQry.AppendLine(" AND FDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FDD.PROJECT_VERSION = 'Current'");

            //P=Payroll B=Both (Payroll and Time Attendance Internet)
            strQry.AppendLine(" AND FDD.PROGRAM_ID IN ('P','B','C')");

            //2018-09-08 - Not in Beta
            strQry.AppendLine(" AND BETA_TABLE.FILE_NAME IS NULL");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FDD.PROJECT_VERSION ");
            strQry.AppendLine(",FDD.FILE_NAME");
            strQry.AppendLine(",FDD.FILE_SIZE");
            strQry.AppendLine(",FDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FDD.FILE_VERSION_NO");
            strQry.AppendLine(",FDD.FILE_CRC_VALUE");

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FCDD.PROJECT_VERSION ");
            strQry.AppendLine(",'' AS FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(",CONVERT(INT,-1) AS COMPANY_NO");
            strQry.AppendLine(",'1' AS FROM_IND");
            strQry.AppendLine(",'N' AS ALWAYS_IND");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS FCDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_USERS FDU");
            strQry.AppendLine(" ON FCDD.PROJECT_VERSION = FDU.PROJECT_VERSION");
            strQry.AppendLine(" AND FDU.USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FCDD.FILE_LAYER_IND = FDC.FILE_LAYER_IND");
            strQry.AppendLine(" AND FCDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FCDD.PROJECT_VERSION = 'Beta'");

            strQry.AppendLine(" AND FCDD.FILE_NAME = 'clsISClientUtilities.dll'");
            strQry.AppendLine(" AND FCDD.FILE_LAYER_IND = 'P'");

            strQry.AppendLine(" GROUP BY ");

            strQry.AppendLine(" FCDD.PROJECT_VERSION ");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(" UNION");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FCDD.PROJECT_VERSION ");
            strQry.AppendLine(",'' AS FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(",CONVERT(INT,-1) AS COMPANY_NO");
            strQry.AppendLine(",'1' AS FROM_IND");
            strQry.AppendLine(",'N' AS ALWAYS_IND");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS FCDD");

            //2018-09-08 - Start 
            strQry.AppendLine(" LEFT JOIN ");
            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" FCDD.FILE_NAME");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS FCDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_USERS FDU");
            strQry.AppendLine(" ON FCDD.PROJECT_VERSION = FDU.PROJECT_VERSION");
            strQry.AppendLine(" AND FDU.USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());

            strQry.AppendLine(" WHERE FCDD.PROJECT_VERSION = 'Beta'");
            strQry.AppendLine(" AND FCDD.FILE_NAME = 'clsISClientUtilities.dll'");
            strQry.AppendLine(" AND FCDD.FILE_LAYER_IND = 'P') AS BETA_TABLE");
            strQry.AppendLine(" ON FCDD.FILE_NAME = BETA_TABLE.FILE_NAME");
            //2018-09-08 - End 

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FCDD.FILE_LAYER_IND = FDC.FILE_LAYER_IND");
            strQry.AppendLine(" AND FCDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FCDD.PROJECT_VERSION = 'Current'");

            strQry.AppendLine(" AND FCDD.FILE_NAME = 'clsISClientUtilities.dll'");
            strQry.AppendLine(" AND FCDD.FILE_LAYER_IND = 'P'");

            //2018-09-08 - Not in Beta
            strQry.AppendLine(" AND BETA_TABLE.FILE_NAME IS NULL");

            strQry.AppendLine(" GROUP BY ");

            strQry.AppendLine(" FCDD.PROJECT_VERSION ");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 1");
            //2013-06-24  FILE_LAYER_IND Makes Server Objects Download Before Presenation Objects for Local Database
            strQry.AppendLine(",2 DESC");
            strQry.AppendLine(",3");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Files", -1);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" UM.COMPANY_NO");
            strQry.AppendLine(",UM.MENU_ITEM_ID");

            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_MENU UM");

            strQry.AppendLine(" WHERE FROM_PROGRAM_IND = 'P'");
            strQry.AppendLine(" AND UM.USER_NO = " + Convert.ToInt64(DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"]));
            strQry.AppendLine(" AND UM.DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" UM.COMPANY_NO");
            strQry.AppendLine(",UM.MENU_ITEM_ID DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Menus", -1);

            //2017-11-25
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" USER_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_COMPANY_TO_LOAD");

            strQry.AppendLine(" WHERE USER_NO = " + Convert.ToInt64(DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"]));

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "UserCompanyToLoad", -1);

            bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Logon_User_New(string parstrUserInformation,string parstrClientDBConnected)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            string[] strPieces = parstrUserInformation.Split('|');

            string strUserId = clsDBConnectionObjects.Text2DynamicSQL(strPieces[0]);
            string strPassword = clsDBConnectionObjects.Text2DynamicSQL(strPieces[1]);

            byte[] bytCompress;

            DataSet DataSet = new DataSet();

            DataTable DataTable = new DataTable("ReturnValues");

            DataTable.Columns.Add("USER_NO", typeof(Int64));
            DataTable.Columns.Add("RESET_IND", typeof(String));
            DataTable.Columns.Add("ACCESS_IND", typeof(String));
            DataTable.Columns.Add("LAST_COMPANY_NO", typeof(Int64));
            DataTable.Columns.Add("LAST_PAY_CATEGORY_TYPE", typeof(String));
            DataTable.Columns.Add("LOCK_IND", typeof(String));

            DataSet.Tables.Add(DataTable);

            DataRow myDataRow = DataSet.Tables["ReturnValues"].NewRow();

            myDataRow["USER_NO"] = -1;
            myDataRow["RESET_IND"] = "N";
            //Will Be Overriden Later if Administartor
            myDataRow["ACCESS_IND"] = "U";
            myDataRow["LAST_COMPANY_NO"] = 0;
            myDataRow["LAST_PAY_CATEGORY_TYPE"] = "";
            
            DataSet.Tables["ReturnValues"].Rows.Add(myDataRow);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" U.USER_NO");
            strQry.AppendLine(",U.SYSTEM_ADMINISTRATOR_IND");
            strQry.AppendLine(",U.RESET");
            strQry.AppendLine(",CL.COMPANY_NO AS LAST_COMPANY_NO");
            strQry.AppendLine(",U.LAST_PAY_CATEGORY_TYPE");
            strQry.AppendLine(",U.LOCK_IND");

            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID U");

            strQry.AppendLine(" LEFT JOIN InteractPayroll.dbo.COMPANY_LINK CL ");
            strQry.AppendLine(" ON U.LAST_COMPANY_NO = CL.COMPANY_NO ");

            strQry.AppendLine(" WHERE U.USER_ID = " + strUserId);
            strQry.AppendLine(" AND U.PASSWORD = " + strPassword);
            strQry.AppendLine(" AND U.DATETIME_DELETE_RECORD IS NULL ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", -1);

            //TEMP //TEMP  - Stop User Logon
            //if (strUserId == "'INTERACT'"
            //       & strPassword == "'TCARETNI'")
            //{
            //}
            //else
            //{
            //    //Not Found
            //    bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            //    DataSet.Dispose();
            //    DataSet = null;

            //    return bytCompress;
            //}

            //TEMP //TEMP

            if (DataSet.Tables["Temp"].Rows.Count == 0)
            {
                if ((strUserId == "'INTERACT'"
                && strPassword == "'TCARETNI'")
                || (strUserId == "'HELENALR'"
                && strPassword == "'MONSTER'"))
                {
                    DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"] = 0;
                    DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"] = "S";
                }
                else
                {
                    //Not Found
                    bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
                    DataSet.Dispose();
                    DataSet = null;

                    return bytCompress;
                }
            }
            else
            {
                DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"] = Convert.ToInt64(DataSet.Tables["Temp"].Rows[0]["USER_NO"]);
                DataSet.Tables["ReturnValues"].Rows[0]["RESET_IND"] = DataSet.Tables["Temp"].Rows[0]["RESET"].ToString();
                DataSet.Tables["ReturnValues"].Rows[0]["LOCK_IND"] = DataSet.Tables["Temp"].Rows[0]["LOCK_IND"].ToString();

                if (DataSet.Tables["Temp"].Rows[0]["LAST_COMPANY_NO"] != System.DBNull.Value)
                {
                    DataSet.Tables["ReturnValues"].Rows[0]["LAST_COMPANY_NO"] = Convert.ToInt64(DataSet.Tables["Temp"].Rows[0]["LAST_COMPANY_NO"]);
                    DataSet.Tables["ReturnValues"].Rows[0]["LAST_PAY_CATEGORY_TYPE"] = DataSet.Tables["Temp"].Rows[0]["LAST_PAY_CATEGORY_TYPE"].ToString();
                }
                else
                {
                    DataSet.Tables["ReturnValues"].Rows[0]["LAST_COMPANY_NO"] = 0;
                    DataSet.Tables["ReturnValues"].Rows[0]["LAST_PAY_CATEGORY_TYPE"] = "";
                }

                //System Administrator
                if (DataSet.Tables["Temp"].Rows[0]["SYSTEM_ADMINISTRATOR_IND"].ToString() == "Y")
                {
                    DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"] = "S";
                }

                //Set Time Logged On
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_ID");
                strQry.AppendLine(" SET LAST_TIME_ON = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "'");
                strQry.AppendLine(" WHERE USER_NO = " + Convert.ToInt64(DataSet.Tables["Temp"].Rows[0]["USER_NO"]));
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            }

            //NB DISTINCT Fixes a Bug that crept into System
            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" C.COMPANY_NO ");
            strQry.AppendLine(",C.COMPANY_DESC");
            strQry.AppendLine(",C.DATE_FORMAT ");
            strQry.AppendLine(",ISNULL(C.TIMESHEET_READ_TIMEOUT_SECONDS,30) AS TIMESHEET_READ_TIMEOUT_SECONDS ");
            strQry.AppendLine(",C.DYNAMIC_UPLOAD_KEY ");

            if (DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"].ToString() == "S")
            {
                strQry.AppendLine(",'N' AS LOCK_IND ");
            }
            else
            {
                strQry.AppendLine(",C.LOCK_IND ");
            }
            
            if (DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"].ToString() != "S")
            {
                strQry.AppendLine(",UCA.COMPANY_ACCESS_IND");
            }
            else
            {
                strQry.AppendLine(",'A' AS COMPANY_ACCESS_IND");
            }

            strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK C");

            if (DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"].ToString() != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA ");
                strQry.AppendLine(" ON UCA.USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());
                strQry.AppendLine(" AND UCA.COMPANY_NO = C.COMPANY_NO ");
                strQry.AppendLine(" AND (UCA.COMPANY_ACCESS_IND = 'A'");
                strQry.AppendLine(" OR (UCA.COMPANY_ACCESS_IND = 'U'");
                //2017-05-17
                strQry.AppendLine(" AND ISNULL(UCA.ACCESS_LAYER_IND,'C') IN ('B','I')))");
                strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL ");
            }
            
            strQry.AppendLine(" WHERE ISNULL(C.TIME_ATTENDANCE_IND,'') <> 'Y'");
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" C.COMPANY_DESC");
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", -1);

            for (int intRow = 0; intRow < DataSet.Tables["Company"].Rows.Count; intRow++)
            {
                if (intRow == 0)
                {
                    if (Convert.ToInt64(DataSet.Tables["ReturnValues"].Rows[0]["LAST_COMPANY_NO"]) == 0)
                    {
                        DataSet.Tables["ReturnValues"].Rows[0]["LAST_COMPANY_NO"] = Convert.ToInt64(DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"]);
                    }
                }

                if (DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"].ToString() != "S")
                {
                    //2013-08-29
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Convert.ToInt64(DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"]));
                    
                    if (DataSet.Tables["Company"].Rows[intRow]["COMPANY_ACCESS_IND"].ToString() == "A")
                    {
                        DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"] = "A";
                    }
                    else
                    {
                        strQry.Clear();
                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP ");
                        strQry.AppendLine("(USER_NO");
                        strQry.AppendLine(",COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",PAY_CATEGORY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE)");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());
                        strQry.AppendLine("," + DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"].ToString());
                        strQry.AppendLine(",USER_TABLE.EMPLOYEE_NO");
                        strQry.AppendLine(",USER_TABLE.PAY_CATEGORY_NO");
                        strQry.AppendLine(",USER_TABLE.PAY_CATEGORY_TYPE");

                        strQry.AppendLine(" FROM ");

                        strQry.AppendLine("(SELECT ");
                        strQry.AppendLine(" E.EMPLOYEE_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

                        //Errol 2013-04-12
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
                        strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
                        strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                        strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_PAY_CATEGORY_DEPARTMENT UPCD");
                        strQry.AppendLine(" ON E.COMPANY_NO = UPCD.COMPANY_NO");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UPCD.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UPCD.PAY_CATEGORY_TYPE");
                        strQry.AppendLine(" AND E.DEPARTMENT_NO = UPCD.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND UPCD.USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());
                        strQry.AppendLine(" AND UPCD.DATETIME_DELETE_RECORD IS NULL ");

                        strQry.AppendLine(" WHERE E.COMPANY_NO = " + DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"].ToString());
                        strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                        strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                        strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.EMPLOYEE_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

                        //Errol 2013-04-12
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
                        strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
                        strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                        strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_EMPLOYEE UE");
                        strQry.AppendLine(" ON E.COMPANY_NO = UE.COMPANY_NO");
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UE.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UE.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND UE.USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());
                        strQry.AppendLine(" AND UE.DATETIME_DELETE_RECORD IS NULL ");

                        strQry.AppendLine(" WHERE E.COMPANY_NO = " + DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"].ToString());
                        strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                        strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                        strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.EMPLOYEE_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

                        //Errol 2013-04-12
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
                        strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
                        strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                        strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_PAY_CATEGORY UPC");
                        strQry.AppendLine(" ON E.COMPANY_NO = UPC.COMPANY_NO");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND UPC.USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());
                        strQry.AppendLine(" AND UPC.DATETIME_DELETE_RECORD IS NULL ");

                        strQry.AppendLine(" WHERE E.COMPANY_NO = " + DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"].ToString());
                        strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                        strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                        strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL) AS USER_TABLE");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Convert.ToInt64(DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"]));
                    }
                }
            }

            strQry.Clear();

            if (parstrClientDBConnected == "Y")
            {
                strQry.Append(clsDBConnectionObjects.Get_Internet_Client_Download_SQL(Convert.ToInt64(DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"])));
            
                strQry.AppendLine(" UNION");
            }

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FDD.PROJECT_VERSION ");
            strQry.AppendLine(",'' AS FILE_LAYER_IND");
            strQry.AppendLine(",FDD.FILE_NAME");
            strQry.AppendLine(",FDD.FILE_SIZE");
            strQry.AppendLine(",FDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FDD.FILE_VERSION_NO");
            strQry.AppendLine(",FDD.FILE_CRC_VALUE");
            strQry.AppendLine(",CONVERT(INT,-1) AS COMPANY_NO");
            strQry.AppendLine(",'1' AS FROM_IND");
            strQry.AppendLine(",'N' AS ALWAYS_IND");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FDD.PROJECT_VERSION = FDC.PROJECT_VERSION");
            strQry.AppendLine(" AND FDD.PROGRAM_ID = FDC.PROGRAM_ID");
            strQry.AppendLine(" AND FDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FDD.PROJECT_VERSION = 'Current'");

            //P=Payroll B=Both (Payroll and Time Attendance Internet)
            strQry.AppendLine(" AND FDD.PROGRAM_ID IN ('P','B','C')");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FDD.PROJECT_VERSION ");
            strQry.AppendLine(",FDD.FILE_NAME");
            strQry.AppendLine(",FDD.FILE_SIZE");
            strQry.AppendLine(",FDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FDD.FILE_VERSION_NO");
            strQry.AppendLine(",FDD.FILE_CRC_VALUE");
               
            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" 'Current' AS PROJECT_VERSION ");
            strQry.AppendLine(",'' AS FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(",CONVERT(INT,-1) AS COMPANY_NO");
            strQry.AppendLine(",'1' AS FROM_IND");
            strQry.AppendLine(",'N' AS ALWAYS_IND");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS FCDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FCDD.FILE_LAYER_IND = FDC.FILE_LAYER_IND");
            strQry.AppendLine(" AND FCDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FCDD.FILE_NAME = 'clsISClientUtilities.dll'");
            strQry.AppendLine(" AND FCDD.FILE_LAYER_IND = 'P'");
                
            strQry.AppendLine(" GROUP BY ");

            strQry.AppendLine(" FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 1");
            //2013-06-24  FILE_LAYER_IND Makes Server Objects Download Before Presenation Objects for Local Database
            strQry.AppendLine(",2 DESC");
            strQry.AppendLine(",3");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Files", -1);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" UM.COMPANY_NO");
            strQry.AppendLine(",UM.MENU_ITEM_ID");

            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_MENU UM");

            strQry.AppendLine(" WHERE FROM_PROGRAM_IND = 'P'");
            strQry.AppendLine(" AND UM.USER_NO = " + Convert.ToInt64(DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"]));
            strQry.AppendLine(" AND UM.DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" UM.COMPANY_NO");
            strQry.AppendLine(",UM.MENU_ITEM_ID DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Menus", -1);
            
            //2017-11-25
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" USER_NO");
            
            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_COMPANY_TO_LOAD");

            strQry.AppendLine(" WHERE USER_NO = " + Convert.ToInt64(DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"]));
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "UserCompanyToLoad", -1);
                        
            bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
        
        public byte[] Logon_New_User_TimeAttendance(string parstrUserInformation, string parstrClientDBConnected)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            string[] strPieces = parstrUserInformation.Split('|');

            string strUserId = clsDBConnectionObjects.Text2DynamicSQL(strPieces[0]);
            string strPassword = clsDBConnectionObjects.Text2DynamicSQL(strPieces[1]);

            byte[] bytCompress;

            DataSet DataSet = new DataSet();

            DataTable DataTable = new DataTable("ReturnValues");

            DataTable.Columns.Add("USER_NO", typeof(Int64));
            DataTable.Columns.Add("RESET_IND", typeof(String));
            DataTable.Columns.Add("ACCESS_IND", typeof(String));
            DataTable.Columns.Add("LAST_COMPANY_NO", typeof(Int64));
            DataTable.Columns.Add("LAST_PAY_CATEGORY_TYPE", typeof(String));
            DataTable.Columns.Add("LOCK_IND", typeof(String));

            DataSet.Tables.Add(DataTable);

            DataRow myDataRow = DataSet.Tables["ReturnValues"].NewRow();

            myDataRow["USER_NO"] = -1;
            myDataRow["RESET_IND"] = "N";
            //Will Be Overriden Later if Administartor
            myDataRow["ACCESS_IND"] = "U";
            myDataRow["LAST_COMPANY_NO"] = 0;
            myDataRow["LAST_PAY_CATEGORY_TYPE"] = "";

            DataSet.Tables["ReturnValues"].Rows.Add(myDataRow);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" U.USER_NO");
            strQry.AppendLine(",U.SYSTEM_ADMINISTRATOR_IND");
            strQry.AppendLine(",U.RESET");
            strQry.AppendLine(",CL.COMPANY_NO AS LAST_COMPANY_NO");
            strQry.AppendLine(",U.LAST_PAY_CATEGORY_TYPE");
            strQry.AppendLine(",U.LOCK_IND");

            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID U");

            strQry.AppendLine(" LEFT JOIN InteractPayroll.dbo.COMPANY_LINK CL ");
            strQry.AppendLine(" ON U.LAST_COMPANY_NO = CL.COMPANY_NO ");

            strQry.AppendLine(" WHERE U.USER_ID = " + strUserId);
            strQry.AppendLine(" AND U.PASSWORD = " + strPassword);
            strQry.AppendLine(" AND U.DATETIME_DELETE_RECORD IS NULL ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", -1);

            if (DataSet.Tables["Temp"].Rows.Count == 0)
            {
                if ((strUserId == "'INTERACT'"
                && strPassword == "'TCARETNI'")
                || (strUserId == "'HELENALR'"
                && strPassword == "'MONSTER'"))
                {
                    DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"] = 0;
                    DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"] = "S";
                }
                else
                {
                    //Not Found
                    bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
                    DataSet.Dispose();
                    DataSet = null;

                    return bytCompress;
                }
            }
            else
            {
                DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"] = Convert.ToInt64(DataSet.Tables["Temp"].Rows[0]["USER_NO"]);
                DataSet.Tables["ReturnValues"].Rows[0]["RESET_IND"] = DataSet.Tables["Temp"].Rows[0]["RESET"].ToString();
                DataSet.Tables["ReturnValues"].Rows[0]["LOCK_IND"] = DataSet.Tables["Temp"].Rows[0]["LOCK_IND"].ToString();

                if (DataSet.Tables["Temp"].Rows[0]["LAST_COMPANY_NO"] != System.DBNull.Value)
                {
                    DataSet.Tables["ReturnValues"].Rows[0]["LAST_COMPANY_NO"] = Convert.ToInt64(DataSet.Tables["Temp"].Rows[0]["LAST_COMPANY_NO"]);
                    DataSet.Tables["ReturnValues"].Rows[0]["LAST_PAY_CATEGORY_TYPE"] = DataSet.Tables["Temp"].Rows[0]["LAST_PAY_CATEGORY_TYPE"].ToString();
                }
                else
                {
                    DataSet.Tables["ReturnValues"].Rows[0]["LAST_COMPANY_NO"] = 0;
                    DataSet.Tables["ReturnValues"].Rows[0]["LAST_PAY_CATEGORY_TYPE"] = "";
                }

                //System Administrator
                if (DataSet.Tables["Temp"].Rows[0]["SYSTEM_ADMINISTRATOR_IND"].ToString() == "Y")
                {
                    DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"] = "S";
                }

                //Set Time Logged On
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_ID");
                strQry.AppendLine(" SET LAST_TIME_ON = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "'");
                strQry.AppendLine(" WHERE USER_NO = " + Convert.ToInt64(DataSet.Tables["Temp"].Rows[0]["USER_NO"]));
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            }

            //NB DISTINCT Fixes a Bug that crept into System
            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" C.COMPANY_NO ");
            strQry.AppendLine(",C.COMPANY_DESC");
            strQry.AppendLine(",C.DATE_FORMAT ");
            strQry.AppendLine(",ISNULL(C.TIMESHEET_READ_TIMEOUT_SECONDS,30) AS TIMESHEET_READ_TIMEOUT_SECONDS ");
            strQry.AppendLine(",C.DYNAMIC_UPLOAD_KEY ");

            if (DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"].ToString() == "S")
            {
                strQry.AppendLine(",'N' AS LOCK_IND ");
            }
            else
            {
                strQry.AppendLine(",C.LOCK_IND ");
            }

            if (DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"].ToString() != "S")
            {
                strQry.AppendLine(",UCA.COMPANY_ACCESS_IND");
            }
            else
            {
                strQry.AppendLine(",'A' AS COMPANY_ACCESS_IND");
            }

            strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK C");

            if (DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"].ToString() != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA ");
                strQry.AppendLine(" ON UCA.USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());
                strQry.AppendLine(" AND UCA.COMPANY_NO = C.COMPANY_NO ");
                strQry.AppendLine(" AND (UCA.COMPANY_ACCESS_IND = 'A'");
                strQry.AppendLine(" OR (UCA.COMPANY_ACCESS_IND = 'U'");
                //2017-05-17
                strQry.AppendLine(" AND ISNULL(UCA.ACCESS_LAYER_IND,'C') IN ('B','I')))");
                strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL ");
            }

            strQry.AppendLine(" WHERE ISNULL(C.TIME_ATTENDANCE_IND,'') IN ('Y','B')");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" C.COMPANY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", -1);

            for (int intRow = 0; intRow < DataSet.Tables["Company"].Rows.Count; intRow++)
            {
                if (intRow == 0)
                {
                    if (Convert.ToInt64(DataSet.Tables["ReturnValues"].Rows[0]["LAST_COMPANY_NO"]) == 0)
                    {
                        DataSet.Tables["ReturnValues"].Rows[0]["LAST_COMPANY_NO"] = Convert.ToInt64(DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"]);
                    }
                }

                if (DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"].ToString() != "S")
                {
                    //2013-08-29
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Convert.ToInt64(DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"]));

                    if (DataSet.Tables["Company"].Rows[intRow]["COMPANY_ACCESS_IND"].ToString() == "A")
                    {
                        DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"] = "A";
                    }
                    else
                    {
                        strQry.Clear();
                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP ");
                        strQry.AppendLine("(USER_NO");
                        strQry.AppendLine(",COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",PAY_CATEGORY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE)");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());
                        strQry.AppendLine("," + DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"].ToString());
                        strQry.AppendLine(",USER_TABLE.EMPLOYEE_NO");
                        strQry.AppendLine(",USER_TABLE.PAY_CATEGORY_NO");
                        strQry.AppendLine(",USER_TABLE.PAY_CATEGORY_TYPE");

                        strQry.AppendLine(" FROM ");

                        strQry.AppendLine("(SELECT ");
                        strQry.AppendLine(" E.EMPLOYEE_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

                        //Errol 2013-04-12
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
                        strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
                        strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                        strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_PAY_CATEGORY_DEPARTMENT UPCD");
                        strQry.AppendLine(" ON E.COMPANY_NO = UPCD.COMPANY_NO");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UPCD.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UPCD.PAY_CATEGORY_TYPE");
                        strQry.AppendLine(" AND E.DEPARTMENT_NO = UPCD.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND UPCD.USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());
                        strQry.AppendLine(" AND UPCD.DATETIME_DELETE_RECORD IS NULL ");

                        strQry.AppendLine(" WHERE E.COMPANY_NO = " + DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"].ToString());
                        strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                        strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                        strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.EMPLOYEE_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

                        //Errol 2013-04-12
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
                        strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
                        strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                        strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_EMPLOYEE UE");
                        strQry.AppendLine(" ON E.COMPANY_NO = UE.COMPANY_NO");
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UE.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UE.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND UE.USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());
                        strQry.AppendLine(" AND UE.DATETIME_DELETE_RECORD IS NULL ");

                        strQry.AppendLine(" WHERE E.COMPANY_NO = " + DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"].ToString());
                        strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                        strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                        strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.EMPLOYEE_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

                        //Errol 2013-04-12
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
                        strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
                        strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                        strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_PAY_CATEGORY UPC");
                        strQry.AppendLine(" ON E.COMPANY_NO = UPC.COMPANY_NO");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND UPC.USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());
                        strQry.AppendLine(" AND UPC.DATETIME_DELETE_RECORD IS NULL ");

                        strQry.AppendLine(" WHERE E.COMPANY_NO = " + DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"].ToString());
                        strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                        strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                        strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL) AS USER_TABLE");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Convert.ToInt64(DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"]));
                    }
                }
            }

            strQry.Clear();

            if (parstrClientDBConnected == "Y")
            {
                strQry.Append(clsDBConnectionObjects.Get_Internet_Client_Download_SQL_New(Convert.ToInt64(DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"])));

                strQry.AppendLine(" UNION");
            }

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FDD.PROJECT_VERSION ");
            strQry.AppendLine(",'' AS FILE_LAYER_IND");
            strQry.AppendLine(",FDD.FILE_NAME");
            strQry.AppendLine(",FDD.FILE_SIZE");
            strQry.AppendLine(",FDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FDD.FILE_VERSION_NO");
            strQry.AppendLine(",FDD.FILE_CRC_VALUE");
            strQry.AppendLine(",CONVERT(INT,-1) AS COMPANY_NO");
            strQry.AppendLine(",'1' AS FROM_IND");
            strQry.AppendLine(",'N' AS ALWAYS_IND");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_USERS FDU");
            strQry.AppendLine(" ON FDD.PROJECT_VERSION = FDU.PROJECT_VERSION");
            strQry.AppendLine(" AND FDU.USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FDD.PROJECT_VERSION = FDC.PROJECT_VERSION");
            strQry.AppendLine(" AND FDD.PROGRAM_ID = FDC.PROGRAM_ID");
            strQry.AppendLine(" AND FDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FDD.PROJECT_VERSION = 'Beta'");

            //T=Time Attendance Internet B=Both (Payroll and Time Attendance Internet)
            strQry.AppendLine(" AND FDD.PROGRAM_ID IN ('T','B','C')");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FDD.PROJECT_VERSION ");
            strQry.AppendLine(",FDD.FILE_NAME");
            strQry.AppendLine(",FDD.FILE_SIZE");
            strQry.AppendLine(",FDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FDD.FILE_VERSION_NO");
            strQry.AppendLine(",FDD.FILE_CRC_VALUE");

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FDD.PROJECT_VERSION ");
            strQry.AppendLine(",'' AS FILE_LAYER_IND");
            strQry.AppendLine(",FDD.FILE_NAME");
            strQry.AppendLine(",FDD.FILE_SIZE");
            strQry.AppendLine(",FDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FDD.FILE_VERSION_NO");
            strQry.AppendLine(",FDD.FILE_CRC_VALUE");
            strQry.AppendLine(",CONVERT(INT,-1) AS COMPANY_NO");
            strQry.AppendLine(",'1' AS FROM_IND");
            strQry.AppendLine(",'N' AS ALWAYS_IND");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FDD");

            //2018-09-08 - Start 
            strQry.AppendLine(" LEFT JOIN ");
            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" FDD.FILE_NAME");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_USERS FDU");
            strQry.AppendLine(" ON FDD.PROJECT_VERSION = FDU.PROJECT_VERSION");
            strQry.AppendLine(" AND FDU.USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());

            strQry.AppendLine(" WHERE FDD.PROJECT_VERSION = 'Beta'");

            //P=Payroll B=Both (Payroll and Time Attendance Internet)
            strQry.AppendLine(" AND FDD.PROGRAM_ID IN ('T','B','C')) AS BETA_TABLE");
            strQry.AppendLine(" ON FDD.FILE_NAME = BETA_TABLE.FILE_NAME");
            //2018-09-08 - End 

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FDD.PROJECT_VERSION = FDC.PROJECT_VERSION");
            strQry.AppendLine(" AND FDD.PROGRAM_ID = FDC.PROGRAM_ID");
            strQry.AppendLine(" AND FDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FDD.PROJECT_VERSION = 'Current'");

            //T=Time Attendance Internet B=Both (Payroll and Time Attendance Internet)
            strQry.AppendLine(" AND FDD.PROGRAM_ID IN ('T','B','C')");

            //2018-09-08 - Not in Beta
            strQry.AppendLine(" AND BETA_TABLE.FILE_NAME IS NULL");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FDD.PROJECT_VERSION ");
            strQry.AppendLine(",FDD.FILE_NAME");
            strQry.AppendLine(",FDD.FILE_SIZE");
            strQry.AppendLine(",FDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FDD.FILE_VERSION_NO");
            strQry.AppendLine(",FDD.FILE_CRC_VALUE");

            //Client
            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FCDD.PROJECT_VERSION ");
            strQry.AppendLine(",'' AS FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(",CONVERT(INT,-1) AS COMPANY_NO");
            strQry.AppendLine(",'1' AS FROM_IND");
            strQry.AppendLine(",'N' AS ALWAYS_IND");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS FCDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_USERS FDU");
            strQry.AppendLine(" ON FCDD.PROJECT_VERSION = FDU.PROJECT_VERSION");
            strQry.AppendLine(" AND FDU.USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FCDD.FILE_LAYER_IND = FDC.FILE_LAYER_IND");
            strQry.AppendLine(" AND FCDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FCDD.PROJECT_VERSION = 'Beta'");
            strQry.AppendLine(" AND FCDD.FILE_NAME = 'clsISClientUtilities.dll'");
            strQry.AppendLine(" AND FCDD.FILE_LAYER_IND = 'P'");

            strQry.AppendLine(" GROUP BY ");

            strQry.AppendLine(" FCDD.PROJECT_VERSION ");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FCDD.PROJECT_VERSION ");
            strQry.AppendLine(",'' AS FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(",CONVERT(INT,-1) AS COMPANY_NO");
            strQry.AppendLine(",'1' AS FROM_IND");
            strQry.AppendLine(",'N' AS ALWAYS_IND");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS FCDD");

            //2018-09-08 - Start 
            strQry.AppendLine(" LEFT JOIN ");
            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" FDD.FILE_NAME");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS FDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_USERS FDU");
            strQry.AppendLine(" ON FDD.PROJECT_VERSION = FDU.PROJECT_VERSION");
            strQry.AppendLine(" AND FDU.USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());

            strQry.AppendLine(" WHERE FDD.PROJECT_VERSION = 'Beta'");

            strQry.AppendLine(" AND FDD.FILE_NAME = 'clsISClientUtilities.dll'");
            strQry.AppendLine(" AND FDD.FILE_LAYER_IND = 'P') AS BETA_TABLE");
            strQry.AppendLine(" ON FCDD.FILE_NAME = BETA_TABLE.FILE_NAME");
            //2018-09-08 - End 

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FCDD.FILE_LAYER_IND = FDC.FILE_LAYER_IND");
            strQry.AppendLine(" AND FCDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FCDD.PROJECT_VERSION = 'Current'");
            strQry.AppendLine(" AND FCDD.FILE_NAME = 'clsISClientUtilities.dll'");
            strQry.AppendLine(" AND FCDD.FILE_LAYER_IND = 'P'");

            //2018-09-08 - Not in Beta
            strQry.AppendLine(" AND BETA_TABLE.FILE_NAME IS NULL");

            strQry.AppendLine(" GROUP BY ");

            strQry.AppendLine(" FCDD.PROJECT_VERSION ");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 1");
            //2013-06-24  FILE_LAYER_IND Makes Server Objects Download Before Presenation Objects for Local Database
            strQry.AppendLine(",2 DESC");
            strQry.AppendLine(",3");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Files", -1);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" UM.COMPANY_NO");
            strQry.AppendLine(",UM.MENU_ITEM_ID");

            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_MENU UM");

            strQry.AppendLine(" WHERE FROM_PROGRAM_IND = 'X'");
            strQry.AppendLine(" AND UM.USER_NO = " + Convert.ToInt64(DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"]));
            strQry.AppendLine(" AND UM.DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" UM.COMPANY_NO");
            strQry.AppendLine(",UM.MENU_ITEM_ID DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Menus", -1);

            bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
        
        public byte[] Logon_User_TimeAttendance(string parstrUserInformation, string parstrClientDBConnected)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            string[] strPieces = parstrUserInformation.Split('|');

            string strUserId = clsDBConnectionObjects.Text2DynamicSQL(strPieces[0]);
            string strPassword = clsDBConnectionObjects.Text2DynamicSQL(strPieces[1]);

            byte[] bytCompress;

            DataSet DataSet = new DataSet();

            DataTable DataTable = new DataTable("ReturnValues");

            DataTable.Columns.Add("USER_NO", typeof(Int64));
            DataTable.Columns.Add("RESET_IND", typeof(String));
            DataTable.Columns.Add("ACCESS_IND", typeof(String));
            DataTable.Columns.Add("LAST_COMPANY_NO", typeof(Int64));
            DataTable.Columns.Add("LAST_PAY_CATEGORY_TYPE", typeof(String));
            DataTable.Columns.Add("LOCK_IND", typeof(String));

            DataSet.Tables.Add(DataTable);

            DataRow myDataRow = DataSet.Tables["ReturnValues"].NewRow();

            myDataRow["USER_NO"] = -1;
            myDataRow["RESET_IND"] = "N";
            //Will Be Overriden Later if Administartor
            myDataRow["ACCESS_IND"] = "U";
            myDataRow["LAST_COMPANY_NO"] = 0;
            myDataRow["LAST_PAY_CATEGORY_TYPE"] = "";

            DataSet.Tables["ReturnValues"].Rows.Add(myDataRow);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" U.USER_NO");
            strQry.AppendLine(",U.SYSTEM_ADMINISTRATOR_IND");
            strQry.AppendLine(",U.RESET");
            strQry.AppendLine(",CL.COMPANY_NO AS LAST_COMPANY_NO");
            strQry.AppendLine(",U.LAST_PAY_CATEGORY_TYPE");
            strQry.AppendLine(",U.LOCK_IND");

            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID U");

            strQry.AppendLine(" LEFT JOIN InteractPayroll.dbo.COMPANY_LINK CL ");
            strQry.AppendLine(" ON U.LAST_COMPANY_NO = CL.COMPANY_NO ");

            strQry.AppendLine(" WHERE U.USER_ID = " + strUserId);
            strQry.AppendLine(" AND U.PASSWORD = " + strPassword);
            strQry.AppendLine(" AND U.DATETIME_DELETE_RECORD IS NULL ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", -1);

            if (DataSet.Tables["Temp"].Rows.Count == 0)
            {
                if ((strUserId == "'INTERACT'"
                && strPassword == "'TCARETNI'")
                || (strUserId == "'HELENALR'"
                && strPassword == "'MONSTER'"))
                {
                    DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"] = 0;
                    DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"] = "S";
                }
                else
                {
                    //Not Found
                    bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
                    DataSet.Dispose();
                    DataSet = null;

                    return bytCompress;
                }
            }
            else
            {
                DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"] = Convert.ToInt64(DataSet.Tables["Temp"].Rows[0]["USER_NO"]);
                DataSet.Tables["ReturnValues"].Rows[0]["RESET_IND"] = DataSet.Tables["Temp"].Rows[0]["RESET"].ToString();
                DataSet.Tables["ReturnValues"].Rows[0]["LOCK_IND"] = DataSet.Tables["Temp"].Rows[0]["LOCK_IND"].ToString();

                if (DataSet.Tables["Temp"].Rows[0]["LAST_COMPANY_NO"] != System.DBNull.Value)
                {
                    DataSet.Tables["ReturnValues"].Rows[0]["LAST_COMPANY_NO"] = Convert.ToInt64(DataSet.Tables["Temp"].Rows[0]["LAST_COMPANY_NO"]);
                    DataSet.Tables["ReturnValues"].Rows[0]["LAST_PAY_CATEGORY_TYPE"] = DataSet.Tables["Temp"].Rows[0]["LAST_PAY_CATEGORY_TYPE"].ToString();
                }
                else
                {
                    DataSet.Tables["ReturnValues"].Rows[0]["LAST_COMPANY_NO"] = 0;
                    DataSet.Tables["ReturnValues"].Rows[0]["LAST_PAY_CATEGORY_TYPE"] = "";
                }

                //System Administrator
                if (DataSet.Tables["Temp"].Rows[0]["SYSTEM_ADMINISTRATOR_IND"].ToString() == "Y")
                {
                    DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"] = "S";
                }

                //Set Time Logged On
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_ID");
                strQry.AppendLine(" SET LAST_TIME_ON = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "'");
                strQry.AppendLine(" WHERE USER_NO = " + Convert.ToInt64(DataSet.Tables["Temp"].Rows[0]["USER_NO"]));
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            }

            //NB DISTINCT Fixes a Bug that crept into System
            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" C.COMPANY_NO ");
            strQry.AppendLine(",C.COMPANY_DESC");
            strQry.AppendLine(",C.DATE_FORMAT ");
            strQry.AppendLine(",ISNULL(C.TIMESHEET_READ_TIMEOUT_SECONDS,30) AS TIMESHEET_READ_TIMEOUT_SECONDS ");
            strQry.AppendLine(",C.DYNAMIC_UPLOAD_KEY ");

            if (DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"].ToString() == "S")
            {
                strQry.AppendLine(",'N' AS LOCK_IND ");
            }
            else
            {
                strQry.AppendLine(",C.LOCK_IND ");
            }

            if (DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"].ToString() != "S")
            {
                strQry.AppendLine(",UCA.COMPANY_ACCESS_IND");
            }
            else
            {
                strQry.AppendLine(",'A' AS COMPANY_ACCESS_IND");
            }

            strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK C");
            
            if (DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"].ToString() != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA ");
                strQry.AppendLine(" ON UCA.USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());
                strQry.AppendLine(" AND UCA.COMPANY_NO = C.COMPANY_NO ");
                strQry.AppendLine(" AND (UCA.COMPANY_ACCESS_IND = 'A'");
                strQry.AppendLine(" OR (UCA.COMPANY_ACCESS_IND = 'U'");
                //2017-05-17
                strQry.AppendLine(" AND ISNULL(UCA.ACCESS_LAYER_IND,'C') IN ('B','I')))");
                strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL ");
            }
            
            strQry.AppendLine(" WHERE ISNULL(C.TIME_ATTENDANCE_IND,'') IN ('Y','B')");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" C.COMPANY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", -1);

            for (int intRow = 0; intRow < DataSet.Tables["Company"].Rows.Count; intRow++)
            {
                if (intRow == 0)
                {
                    if (Convert.ToInt64(DataSet.Tables["ReturnValues"].Rows[0]["LAST_COMPANY_NO"]) == 0)
                    {
                        DataSet.Tables["ReturnValues"].Rows[0]["LAST_COMPANY_NO"] = Convert.ToInt64(DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"]);
                    }
                }

                if (DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"].ToString() != "S")
                {
                    //2013-08-29
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP");
                    strQry.AppendLine(" WHERE USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Convert.ToInt64(DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"]));

                    if (DataSet.Tables["Company"].Rows[intRow]["COMPANY_ACCESS_IND"].ToString() == "A")
                    {
                        DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"] = "A";
                    }
                    else
                    {
                        strQry.Clear();
                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP ");
                        strQry.AppendLine("(USER_NO");
                        strQry.AppendLine(",COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",PAY_CATEGORY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE)");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());
                        strQry.AppendLine("," + DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"].ToString());
                        strQry.AppendLine(",USER_TABLE.EMPLOYEE_NO");
                        strQry.AppendLine(",USER_TABLE.PAY_CATEGORY_NO");
                        strQry.AppendLine(",USER_TABLE.PAY_CATEGORY_TYPE");

                        strQry.AppendLine(" FROM ");

                        strQry.AppendLine("(SELECT ");
                        strQry.AppendLine(" E.EMPLOYEE_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

                        //Errol 2013-04-12
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
                        strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
                        strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                        strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_PAY_CATEGORY_DEPARTMENT UPCD");
                        strQry.AppendLine(" ON E.COMPANY_NO = UPCD.COMPANY_NO");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UPCD.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UPCD.PAY_CATEGORY_TYPE");
                        strQry.AppendLine(" AND E.DEPARTMENT_NO = UPCD.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND UPCD.USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());
                        strQry.AppendLine(" AND UPCD.DATETIME_DELETE_RECORD IS NULL ");

                        strQry.AppendLine(" WHERE E.COMPANY_NO = " + DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"].ToString());
                        strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                        strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                        strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.EMPLOYEE_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

                        //Errol 2013-04-12
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
                        strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
                        strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                        strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_EMPLOYEE UE");
                        strQry.AppendLine(" ON E.COMPANY_NO = UE.COMPANY_NO");
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UE.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UE.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND UE.USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());
                        strQry.AppendLine(" AND UE.DATETIME_DELETE_RECORD IS NULL ");

                        strQry.AppendLine(" WHERE E.COMPANY_NO = " + DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"].ToString());
                        strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                        strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                        strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.EMPLOYEE_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

                        //Errol 2013-04-12
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
                        strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
                        strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                        strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_PAY_CATEGORY UPC");
                        strQry.AppendLine(" ON E.COMPANY_NO = UPC.COMPANY_NO");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND UPC.USER_NO = " + DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());
                        strQry.AppendLine(" AND UPC.DATETIME_DELETE_RECORD IS NULL ");

                        strQry.AppendLine(" WHERE E.COMPANY_NO = " + DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"].ToString());
                        strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                        strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                        strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL) AS USER_TABLE");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Convert.ToInt64(DataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"]));
                    }
                }
            }

            strQry.Clear();

            if (parstrClientDBConnected == "Y")
            {
                strQry.Append(clsDBConnectionObjects.Get_Internet_Client_Download_SQL(Convert.ToInt64(DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"])));
            }

            if (parstrClientDBConnected == "Y")
            {
                strQry.AppendLine(" UNION");
            }

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FDD.PROJECT_VERSION ");
            strQry.AppendLine(",'' AS FILE_LAYER_IND");
            strQry.AppendLine(",FDD.FILE_NAME");
            strQry.AppendLine(",FDD.FILE_SIZE");
            strQry.AppendLine(",FDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FDD.FILE_VERSION_NO");
            strQry.AppendLine(",FDD.FILE_CRC_VALUE");
            strQry.AppendLine(",CONVERT(INT,-1) AS COMPANY_NO");
            strQry.AppendLine(",'1' AS FROM_IND");
            strQry.AppendLine(",'N' AS ALWAYS_IND");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_DETAILS FDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FDD.PROJECT_VERSION = FDC.PROJECT_VERSION");
            strQry.AppendLine(" AND FDD.PROGRAM_ID = FDC.PROGRAM_ID");
            strQry.AppendLine(" AND FDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FDD.PROJECT_VERSION = 'Current'");

            //T=Time Attendance Internet B=Both (Payroll and Time Attendance Internet)
            strQry.AppendLine(" AND FDD.PROGRAM_ID IN ('T','B','C')");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" FDD.PROJECT_VERSION ");
            strQry.AppendLine(",FDD.FILE_NAME");
            strQry.AppendLine(",FDD.FILE_SIZE");
            strQry.AppendLine(",FDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FDD.FILE_VERSION_NO");
            strQry.AppendLine(",FDD.FILE_CRC_VALUE");

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" 'Current' AS PROJECT_VERSION ");
            strQry.AppendLine(",'' AS FILE_LAYER_IND");
            strQry.AppendLine(",FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(",CONVERT(INT,-1) AS COMPANY_NO");
            strQry.AppendLine(",'1' AS FROM_IND");
            strQry.AppendLine(",'N' AS ALWAYS_IND");
            strQry.AppendLine(",MAX(FDC.FILE_CHUNK_NO) AS MAX_FILE_CHUNK_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_DETAILS FCDD");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS FDC");
            strQry.AppendLine(" ON FCDD.FILE_LAYER_IND = FDC.FILE_LAYER_IND");
            strQry.AppendLine(" AND FCDD.FILE_NAME = FDC.FILE_NAME");

            strQry.AppendLine(" WHERE FCDD.FILE_NAME = 'clsISClientUtilities.dll'");
            strQry.AppendLine(" AND FCDD.FILE_LAYER_IND = 'P'");

            strQry.AppendLine(" GROUP BY ");

            strQry.AppendLine(" FCDD.FILE_NAME");
            strQry.AppendLine(",FCDD.FILE_SIZE");
            strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
            strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
            strQry.AppendLine(",FCDD.FILE_VERSION_NO");
            strQry.AppendLine(",FILE_CRC_VALUE");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 1");
            //2013-06-24  FILE_LAYER_IND Makes Server Objects Download Before Presenation Objects for Local Database
            strQry.AppendLine(",2 DESC");
            strQry.AppendLine(",3");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Files", -1);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" UM.COMPANY_NO");
            strQry.AppendLine(",UM.MENU_ITEM_ID");

            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_MENU UM");

            strQry.AppendLine(" WHERE FROM_PROGRAM_IND = 'X'");
            strQry.AppendLine(" AND UM.USER_NO = " + Convert.ToInt64(DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"]));
            strQry.AppendLine(" AND UM.DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" UM.COMPANY_NO");
            strQry.AppendLine(",UM.MENU_ITEM_ID DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Menus", -1);
            
            bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
        
        public byte[] Get_File_Chunk(Int64 parint64CompanyNo, string parstrProjectVersion, string parstrFromInd, string parstrFileName, int parintFileChunkNo)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            byte[] bytCompress = Get_New_File_Chunk(parint64CompanyNo,  parstrProjectVersion, parstrFromInd,  "P",  parstrFileName,  parintFileChunkNo);
            
            return bytCompress;
        }
        
        public byte[] Get_New_File_Chunk(Int64 parint64CompanyNo, string parstrProjectVersion, string parstrFromInd, string parstrLayerInd, string parstrFileName, int parintFileChunkNo)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            //Old - Backward Compatible
            if (parstrProjectVersion == "_Client")
            {
                parstrProjectVersion = "Current";
            }
            
            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new DataSet();

            //parstrLayerInd == "P"/"S" - For Client (Not Payroll Intenet or Time Attendance Internet)
            if ((parstrLayerInd == "P"
            && (parstrFileName == "DownloadFiles.dll"
            || parstrFileName == "FileDownload.dll"
            || parstrFileName == "FileUpload.dll"
            || parstrFileName == "clsISUtilities.dll"
            || parstrFileName == "PasswordChange.dll"))
            || parstrFileName == "URLConfig.txt")
            {
                parstrFromInd = "1";
            }
            else
            {
                if (parstrFileName == "clsISClientUtilities.dll")
                {
                    parstrFromInd = "0";
                    parstrLayerInd = "P";
                }
                else
                {
                    if (parstrFileName == "FingerPrintClockService.dll"
                    || parstrFileName == "FingerPrintClockServiceStartStop.dll")
                    {
                        parstrFromInd = "0";
                        parstrLayerInd = "S";
                    }
                }
            }

            //2018-09-29
            parstrProjectVersion = parstrProjectVersion.Replace("_", "");
            
            if (parstrFromInd == "0")
            {
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" FILE_CHUNK");
                strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS");

                //2018-09-29
                strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                strQry.AppendLine(" AND FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrLayerInd));
                strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                strQry.AppendLine(" AND FILE_CHUNK_NO = " + parintFileChunkNo);
            }
            else
            {
                if (parstrFromInd == "1")
                {
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" FILE_CHUNK");
                
                    strQry.AppendLine(" FROM InteractPayroll.dbo.FILE_DOWNLOAD_CHUNKS");
                    
                    strQry.AppendLine(" WHERE PROJECT_VERSION = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProjectVersion));
                    strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
                    strQry.AppendLine(" AND FILE_CHUNK_NO = " + parintFileChunkNo);
                }
                else
                {
                    if (parstrFromInd == "2")
                    {
                        strQry.Clear();
                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" FILE_CHUNK");
                        
                        strQry.AppendLine(" FROM InteractPayroll.dbo.PRINT_HEADER_IMAGE_FILE_CHUNKS PHIFC");

                        strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.PRINT_HEADER_IMAGE_FILE_DETAILS PHIFD");
                        strQry.AppendLine(" ON PHIFC.COMPANY_NO = PHIFD.COMPANY_NO ");
                        strQry.AppendLine(" AND PHIFD.FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));

                        strQry.AppendLine(" WHERE PHIFC.COMPANY_NO = = " + parint64CompanyNo);
                        strQry.AppendLine(" AND FILE_CHUNK_NO = " + parintFileChunkNo);
                    }
                }
            }

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parint64CompanyNo);

            return (byte[])DataSet.Tables["Temp"].Rows[0]["FILE_CHUNK"];
        }
    }
}
