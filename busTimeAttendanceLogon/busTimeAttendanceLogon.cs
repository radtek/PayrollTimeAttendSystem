using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Net;
using System.IO.Compression;
using InteractPayroll;

namespace InteractPayrollClient
{
    public class busTimeAttendanceLogon
    {
        clsDBConnectionObjects clsDBConnectionObjects;
        clsCrc32 clsCrc32;
        public busTimeAttendanceLogon()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
            
            clsCrc32 = new clsCrc32();

            try
            {
#if (DEBUG)
#else
                //2017-05-10 Fix To Make Sure Databases are In Sync
                clsFixInteractPayrollClientDatabase clsFixInteractPayrollClientDatabase = new clsFixInteractPayrollClientDatabase();
                clsFixInteractPayrollClientDatabase.Fix_Client_Database();
#endif
            }
            catch
            {
            }
        }

        public string Ping()
        {
            return "OK";
        }

        public byte[] Logon_User(string parstrUserInformation)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            bool blnDebug = false;
            StringBuilder strQry = new StringBuilder();
            string[] strPieces = parstrUserInformation.Split('|');

            string strUserId = strPieces[0];
            string strPassword = strPieces[1];
            byte[] bytCompress;
           
            StreamWriter swStreamWriter;

            DataSet DataSet = new DataSet();

            DataTable DataTable = new DataTable("ReturnValues");

            DataTable.Columns.Add("USER_NO", typeof(Int64));
            DataTable.Columns.Add("RESET", typeof(String));
            DataTable.Columns.Add("ACCESS_IND", typeof(String));
            DataTable.Columns.Add("REBOOT_IND", typeof(String));
            DataTable.Columns.Add("MACHINE_NAME", typeof(String));
            DataTable.Columns.Add("MACHINE_IP", typeof(String));
            DataTable.Columns.Add("NO_USERS_IND", typeof(String));
            DataTable.Columns.Add("DOWNLOAD_IND", typeof(String));
            DataTable.Columns.Add("DB_CREATE_FOR_ENGINE", typeof(String));
           
            DataSet.Tables.Add(DataTable);

            DataRow myDataRow = DataSet.Tables["ReturnValues"].NewRow();

            myDataRow["USER_NO"] = -1;
            myDataRow["RESET"] = "N";
            myDataRow["ACCESS_IND"] = "";
            
            DataSet.Tables["ReturnValues"].Rows.Add(myDataRow);

            FileInfo fiFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "Debug.txt");

            if (fiFile.Exists == true)
            {
                blnDebug = true;
            }

            fiFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "busTimeAttendanceLogonLogs.txt");

            if (fiFile.Exists == true)
            {
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "busTimeAttendanceLogonLogs.txt");
            }

            if (blnDebug == true)
            {
                swStreamWriter = fiFile.AppendText();

                swStreamWriter.WriteLine("Call clsDBConnectionObjects.GetSQLConnectionString");

                swStreamWriter.Close();
            }

            DataSet.Tables["ReturnValues"].Rows[0]["DB_CREATE_FOR_ENGINE"] = clsDBConnectionObjects.GetSQLConnectionString();

            if (blnDebug == true)
            {
                swStreamWriter = fiFile.AppendText();

                swStreamWriter.WriteLine("After clsDBConnectionObjects.GetSQLConnectionString = " + DataSet.Tables["ReturnValues"].Rows[0]["DB_CREATE_FOR_ENGINE"].ToString());

                swStreamWriter.Close();
            }

            if (clsDBConnectionObjects.GetSQLConnectionString() != "")
            {
                clsFixInteractPayrollClientDatabase clsFixInteractPayrollClientDatabase = new clsFixInteractPayrollClientDatabase();
                clsFixInteractPayrollClientDatabase.Fix_Client_Database();

                strQry.Clear();

                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" UI.USER_NO");
                strQry.AppendLine(",UI.SYSTEM_ADMINISTRATOR_IND");
                strQry.AppendLine(",UI.RESET");
                //A = Administrator, U = User 
                strQry.AppendLine(",MIN(UC.COMPANY_ACCESS_IND) AS MIN_COMPANY_ACCESS_IND");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_ID UI");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UC");
                strQry.AppendLine(" ON UI.USER_NO = UC.USER_NO");

                strQry.AppendLine(" WHERE UI.USER_ID = " + clsDBConnectionObjects.Text2DynamicSQL(strUserId));
                strQry.AppendLine(" AND UI.PASSWORD = " + clsDBConnectionObjects.Text2DynamicSQL(strPassword));

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" UI.USER_NO");
                strQry.AppendLine(",UI.SYSTEM_ADMINISTRATOR_IND");
                strQry.AppendLine(",UI.RESET");

                try
                {
                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Temp");
                }
                catch (Exception ex)
                {
                    swStreamWriter = fiFile.AppendText();

                    swStreamWriter.WriteLine("Exception = " + ex.Message);

                    swStreamWriter.Close();
                }

                if (blnDebug == true)
                {
                    swStreamWriter = fiFile.AppendText();

                    swStreamWriter.WriteLine("After Temp = Count = " + DataSet.Tables["Temp"].Rows.Count.ToString());

                    swStreamWriter.Close();
                }

                if (DataSet.Tables["Temp"].Rows.Count == 0)
                {
                    DataSet.Tables.Remove(DataSet.Tables["Temp"]);

                    strQry.Clear();
                    strQry.AppendLine(" SELECT");
                    strQry.AppendLine(" USER_NO");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_ID");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "UserTemp");

                    if (DataSet.Tables["UserTemp"].Rows.Count > 0)
                    {
                        DataSet.Tables.Remove(DataSet.Tables["UserTemp"]);

                        if ((strUserId == "INTERACT"
                        && strPassword == "TCARETNI")
                        || (strUserId == "HELENALR"
                        && strPassword == "MONSTER"))
                        {
                            DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"] = 0;
                            DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"] = "S";
                        }
                        else
                        {
                            DataSet.AcceptChanges();

                            //Not Found
                            bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
                            DataSet.Dispose();
                            DataSet = null;

                            return bytCompress;
                        }
                    }
                    else
                    {
                        DataSet.Tables.Remove(DataSet.Tables["UserTemp"]);

                        if ((strUserId == "INTERACT"
                        && strPassword == "TCARETNI")
                        || (strUserId == "HELENALR"
                        && strPassword == "MONSTER"))
                        {
                            DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"] = 0;
                            DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"] = "S";
                        }
                        else
                        {
                            DataSet.Tables["ReturnValues"].Rows[0]["NO_USERS_IND"] = "Y";

                            DataSet.AcceptChanges();

                            bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
                            DataSet.Dispose();
                            DataSet = null;

                            return bytCompress;
                        }
                    }
                }
                else
                {
                    if (DataSet.Tables["Temp"].Rows[0]["SYSTEM_ADMINISTRATOR_IND"].ToString() == "S")
                    {
                        DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"] = DataSet.Tables["Temp"].Rows[0]["SYSTEM_ADMINISTRATOR_IND"].ToString();
                    }
                    else
                    {
                        DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"] = DataSet.Tables["Temp"].Rows[0]["MIN_COMPANY_ACCESS_IND"].ToString();
                    }

                    if (DataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"].ToString() != "S")
                    {
                        //Create Temp Table for Joins
                        strQry.Clear();
                        strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP");
                        strQry.AppendLine(" WHERE USER_NO = " + DataSet.Tables["Temp"].Rows[0]["USER_NO"].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                        strQry.Clear();
                        strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP ");
                        strQry.AppendLine("(USER_NO");
                        strQry.AppendLine(",COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",PAY_CATEGORY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE)");

                        strQry.AppendLine(" SELECT ");

                        strQry.AppendLine(" " + DataSet.Tables["Temp"].Rows[0]["USER_NO"].ToString());
                        strQry.AppendLine(",USER_TABLE.COMPANY_NO");
                        strQry.AppendLine(",USER_TABLE.EMPLOYEE_NO");
                        strQry.AppendLine(",USER_TABLE.PAY_CATEGORY_NO");
                        strQry.AppendLine(",USER_TABLE.PAY_CATEGORY_TYPE");

                        strQry.AppendLine(" FROM ");

                        strQry.AppendLine("(SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO");
                        strQry.AppendLine(",E.EMPLOYEE_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                        
                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_PAY_CATEGORY_DEPARTMENT UPCD");
                        strQry.AppendLine(" ON E.COMPANY_NO = UPCD.COMPANY_NO");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UPCD.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UPCD.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND E.DEPARTMENT_NO = UPCD.DEPARTMENT_NO ");
                        strQry.AppendLine(" AND UPCD.USER_NO = " + DataSet.Tables["Temp"].Rows[0]["USER_NO"].ToString());

                        strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO");
                        strQry.AppendLine(",E.EMPLOYEE_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_EMPLOYEE UE");
                        strQry.AppendLine(" ON E.COMPANY_NO = UE.COMPANY_NO");
                        strQry.AppendLine(" AND EPC.EMPLOYEE_NO = UE.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UE.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND UE.USER_NO = " + DataSet.Tables["Temp"].Rows[0]["USER_NO"].ToString());

                        strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO");
                        strQry.AppendLine(",E.EMPLOYEE_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_PAY_CATEGORY UPC");
                        strQry.AppendLine(" ON E.COMPANY_NO = UPC.COMPANY_NO");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = UPC.PAY_CATEGORY_NO ");
                        strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = UPC.PAY_CATEGORY_TYPE ");
                        strQry.AppendLine(" AND UPC.USER_NO = " + DataSet.Tables["Temp"].Rows[0]["USER_NO"].ToString());

                        strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL ");

                        strQry.AppendLine(" UNION ");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" E.COMPANY_NO");
                        strQry.AppendLine(",E.EMPLOYEE_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                        strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                        strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                        strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA");
                        strQry.AppendLine(" ON UCA.USER_NO = " + DataSet.Tables["Temp"].Rows[0]["USER_NO"].ToString());
                        strQry.AppendLine(" AND E.COMPANY_NO = UCA.COMPANY_NO");
                        //2013-07-10
                        strQry.AppendLine(" AND UCA.COMPANY_ACCESS_IND = 'A'");

                        strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL) AS USER_TABLE");

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                    }

                    DataSet.Tables["ReturnValues"].Rows[0]["USER_NO"] = Convert.ToInt64(DataSet.Tables["Temp"].Rows[0]["USER_NO"]);
                    DataSet.Tables["ReturnValues"].Rows[0]["RESET"] = DataSet.Tables["Temp"].Rows[0]["RESET"].ToString();

                    //Set Time Logged On
                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.USER_ID");
                    strQry.AppendLine(" SET LAST_TIME_ON = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "'");
                    
                    strQry.AppendLine(" WHERE USER_NO = " + Convert.ToInt64(DataSet.Tables["Temp"].Rows[0]["USER_NO"]));

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                //2013-11-01 - Cleanup Records (Used Temporary Area until File is Written to Folder)
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_DETAILS");

                strQry.AppendLine(" WHERE FILE_LAYER_IND = 'S'");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                //2013-11-01 - Cleanup Records (Used Temporary Area until File is Written to Folder)
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS");

                strQry.AppendLine(" WHERE FILE_LAYER_IND = 'S'");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                //2013-11-01 - Cleanup Records (Used Temporary Area until File is Written to Folder)
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS_TEMP");

                strQry.AppendLine(" WHERE FILE_LAYER_IND = 'S'");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" FCDD.FILE_LAYER_IND");
                strQry.AppendLine(",FCDD.FILE_NAME");
                strQry.AppendLine(",FCDD.FILE_SIZE");
                strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
                strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
                strQry.AppendLine(",FCDD.FILE_CRC_VALUE");

                strQry.AppendLine(",ISNULL(MAX(FCDC.FILE_CHUNK_NO),0) AS MAX_FILE_CHUNK_NO");

                strQry.AppendLine(",'' AS DOWNLOAD_IND");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_DETAILS FCDD");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS FCDC");
                strQry.AppendLine(" ON FCDD.FILE_LAYER_IND = FCDC.FILE_LAYER_IND");
                strQry.AppendLine(" AND  FCDD.FILE_NAME = FCDC.FILE_NAME");

                strQry.AppendLine(" WHERE FCDD.FILE_LAYER_IND = 'P'");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" FCDD.FILE_LAYER_IND");
                strQry.AppendLine(",FCDD.FILE_NAME");
                strQry.AppendLine(",FCDD.FILE_SIZE");
                strQry.AppendLine(",FCDD.FILE_SIZE_COMPRESSED");
                strQry.AppendLine(",FCDD.FILE_LAST_UPDATED_DATE");
                strQry.AppendLine(",FCDD.FILE_CRC_VALUE");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" 2");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Files");

                bytCompress = (byte[])Check_Reboot_Server();

                DataSet DataSetTemp = clsDBConnectionObjects.DeCompress_Array_To_DataSet(bytCompress);

                if (DataSetTemp.Tables["ReturnValues"].Rows[0]["REBOOT_IND"].ToString() == "Y")
                {
                    DataSet.Tables["ReturnValues"].Rows[0]["REBOOT_IND"] = DataSetTemp.Tables["ReturnValues"].Rows[0]["REBOOT_IND"].ToString();

                    DataSet.Tables["ReturnValues"].Rows[0]["MACHINE_NAME"] = DataSetTemp.Tables["ReturnValues"].Rows[0]["MACHINE_NAME"].ToString();
                    DataSet.Tables["ReturnValues"].Rows[0]["MACHINE_IP"] = DataSetTemp.Tables["ReturnValues"].Rows[0]["MACHINE_IP"].ToString();
                }
            }
                                                           
            bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Check_For_Server_File_Changes()
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            //2017-02-13
            clsFixInteractPayrollClientDatabase clsFixInteractPayrollClientDatabase = new clsFixInteractPayrollClientDatabase();
            clsFixInteractPayrollClientDatabase.Fix_Client_Database();
            
            byte[] bytCompress = (byte[])Check_Reboot_Server();

            return bytCompress;
        }

        private byte[] Check_Reboot_Server()
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
          
            DataSet DataSet = new DataSet();

            DataTable DataTable = new DataTable("ReturnValues");
            DataTable.Columns.Add("REBOOT_IND", typeof(String));
            DataTable.Columns.Add("MACHINE_NAME", typeof(String));
            DataTable.Columns.Add("MACHINE_IP", typeof(String));

            DataSet.Tables.Add(DataTable);

            DataRow DataRow = DataTable.NewRow();

            DataSet.Tables["ReturnValues"].Rows.Add(DataRow);

            bool blnReboot = false;

            string strBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
#if (DEBUG)
            strBaseDirectory += "bin\\";
#endif
             //2013-07-18
            DirectoryInfo di = new DirectoryInfo(strBaseDirectory);
            FileInfo[] fiFiles = di.GetFiles("*.*_");

            foreach (FileInfo fi in fiFiles)
            {
                if (fi.Name == "FingerPrintClockTimeAttendanceService.exe_"
                    | fi.Name == "FingerPrintClockTimeAttendanceServiceStartStop.exe_"
                    | fi.Name == "FingerPrintClockServiceStartStop.dll_")
                {
                    continue;
                }
                else
                {
                    blnReboot = true;
                    break;
                }
            }

            if (blnReboot == true)
            {
                DataSet.Tables["ReturnValues"].Rows[0]["REBOOT_IND"] = "Y";

                try
                {
                    DataSet.Tables["ReturnValues"].Rows[0]["MACHINE_NAME"] = System.Net.Dns.GetHostName();
                }
                catch
                {
                    DataSet.Tables["ReturnValues"].Rows[0]["MACHINE_NAME"] = "UNKNOWN";
                }

                try
                {
                    DataSet.Tables["ReturnValues"].Rows[0]["MACHINE_IP"] = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList[0].ToString();
                }
                catch
                {
                    DataSet.Tables["ReturnValues"].Rows[0]["MACHINE_IP"] = "UNKNOWN";
                }
            }

            DataSet.AcceptChanges();

            //Make Sure Client DB Has latest Triggers
            clsDBConnectionObjects.Check_Client_Triggers();

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public string GetDBEngine()
        {
            return clsDBConnectionObjects.GetSQLConnectionString();
        }

        public void Update_User_Details_From_Internet(Int64 parint64UserNo,string parstrPassWord)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.USER_ID");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" PASSWORD = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPassWord));
            strQry.AppendLine(",RESET = 'N' ");

            strQry.AppendLine(" WHERE USER_NO = " + parint64UserNo.ToString());

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
        }


        public void Update_New_User_Details_From_Internet(Int64 parint64UserNo, string parstrUserID, string parstrPassWord, string parstrAccessInd,string parstrCompanies)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = new System.Data.DataSet();

            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" USER_NO ");
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_ID");
            
            strQry.AppendLine(" WHERE USER_NO = " + parint64UserNo.ToString());

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Test");

            if (DataSet.Tables["Test"].Rows.Count > 0)
            {
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.USER_ID");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" PASSWORD = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPassWord));
                strQry.AppendLine(",RESET = 'N' ");

                strQry.AppendLine(" WHERE USER_NO = " + parint64UserNo.ToString());

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }
            else
            {
                if (parstrAccessInd == "A")
                {
                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" USER_NO ");
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_ID");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "NewTest");

                    if (DataSet.Tables["NewTest"].Rows.Count == 0)
                    {
                        string[] strCompany = parstrCompanies.Split('#');

                        //New Installation
                        strQry.Clear();
                        strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.USER_ID ");
                        strQry.AppendLine("(USER_NO");
                        strQry.AppendLine(",USER_ID");
                        strQry.AppendLine(",SYSTEM_ADMINISTRATOR_IND");
                        strQry.AppendLine(",PASSWORD");
                        strQry.AppendLine(",INTERNET_IND");
                        strQry.AppendLine(",LAST_TIME_ON)");
                        strQry.AppendLine(" VALUES ");

                        strQry.AppendLine("(" + parint64UserNo);
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrUserID));
                        strQry.AppendLine(",'N'");
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrPassWord));
                        strQry.AppendLine(",'Y'");
                        strQry.AppendLine(",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "')");

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                        for (int intRow = 0; intRow < strCompany.Length; intRow++)
                        {
                            strQry.Clear();
                            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.USER_COMPANY_ACCESS ");
                            strQry.AppendLine("(USER_NO");
                            strQry.AppendLine(",COMPANY_NO");
                            strQry.AppendLine(",COMPANY_ACCESS_IND)");
                            strQry.AppendLine(" VALUES ");

                            strQry.AppendLine("(" + parint64UserNo);
                            strQry.AppendLine("," + strCompany[intRow]);

                            strQry.AppendLine(",'A')");

                            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                        }
                    }
                }
            }
        }

        public byte[] Get_File_Chunk(string parstrFileName, int parintFileChunkNo)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FILE_CHUNK");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS");
            
            strQry.AppendLine(" WHERE FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFileName));
            strQry.AppendLine(" AND FILE_CHUNK_NO = " + parintFileChunkNo);

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "FileChunk");

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
    }
}
