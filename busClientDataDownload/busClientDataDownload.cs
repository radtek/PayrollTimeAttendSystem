using System;
using System.Collections.Generic;
using System.Text;
using InteractPayroll;
using System.Data;
using System.IO;

namespace InteractPayrollClient
{
    public class busClientDataDownload
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busClientDataDownload()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Download_Version_Records()
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY");

            //strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PayCategoryClient");

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_PayCategory_Records(Int64 parint64CompanyNo)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            byte[] bytCompress = Get_PayCategory_Records_New(parint64CompanyNo, "P");

            return bytCompress;
        }

        public byte[] Get_PayCategory_Records_New(Int64 parint64CompanyNo, string strFromProgram)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",LAST_DOWNLOAD_DATETIME");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            if (strFromProgram == "X")
            {
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                if (strFromProgram == "P")
                {
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE IN ('W','S')");
                }
            }

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PayCategoryClient");

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public void Delete_All_PayCategory_Records(Int64 parint64CompanyNo)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            Delete_All_PayCategory_Records_New(parint64CompanyNo, "B");
        }

        public void Delete_All_PayCategory_Records_New(Int64 parint64CompanyNo, string strFromProgram)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            string strQryAddOn = "";

            try
            {
                string strFileDirectory = AppDomain.CurrentDomain.BaseDirectory + "Backup";

                if (Directory.Exists(strFileDirectory))
                {
                }
                else
                {
                    Directory.CreateDirectory(strFileDirectory);
                }

                string strDataBaseName = "InteractPayrollClient";
#if(DEBUG)
                strDataBaseName = "InteractPayrollClient_Debug";
#endif
                string strBackupFileName = strDataBaseName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_Backup_Before_Download_Delete.bak";

                strQry.Clear();
                strQry.AppendLine("BACKUP DATABASE " + strDataBaseName + " TO DISK = '" + strFileDirectory + "\\" + strBackupFileName + "' WITH CHECKSUM ");
#if (DEBUG)
#else

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString(), 60);
#endif
            }
            catch (Exception ex)
            {
            }

            DataSet DataSet = new DataSet();

            if (strFromProgram == "X")
            {
                strQryAddOn = " AND PAY_CATEGORY_TYPE = 'T'";

            }
            else
            {
                if (strFromProgram == "P")
                {
                    strQryAddOn = " AND PAY_CATEGORY_TYPE IN ('W','S')";
                }
            }

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" C1.TABLE_NAME");
            strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS C1");

            strQry.AppendLine(" WHERE C1.COLUMN_NAME = 'PAY_CATEGORY_TYPE'");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "TablePayCategoryTypeDelete");

            for (int intRow = 0; intRow < DataSet.Tables["TablePayCategoryTypeDelete"].Rows.Count; intRow++)
            {
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo." + DataSet.Tables["TablePayCategoryTypeDelete"].Rows[intRow]["TABLE_NAME"].ToString());
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

                strQry.AppendLine(strQryAddOn);

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }
            
            if (strFromProgram == "X"
            || strFromProgram == "B")
            {
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_HISTORY");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_HISTORY");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                //2017-05-01
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            if (strFromProgram == "P"
            || strFromProgram == "B")
            {
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_BREAK_HISTORY");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_HISTORY");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                //2017-05-01
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_HISTORY");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_HISTORY");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                //2017-05-01
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }
            
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" T.TABLE_NAME");
            strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.TABLES T");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient_Debug.INFORMATION_SCHEMA.COLUMNS C1");
            strQry.AppendLine(" ON T.TABLE_NAME = C1.TABLE_NAME");
            strQry.AppendLine(" AND C1.COLUMN_NAME = 'COMPANY_NO'");

            strQry.AppendLine(" LEFT JOIN InteractPayrollClient_Debug.INFORMATION_SCHEMA.COLUMNS C2");
            strQry.AppendLine(" ON T.TABLE_NAME = C2.TABLE_NAME");
            strQry.AppendLine(" AND C2.COLUMN_NAME = 'PAY_CATEGORY_TYPE'");

            strQry.AppendLine(" WHERE T.TABLE_TYPE = 'BASE TABLE'");
            strQry.AppendLine(" AND T.TABLE_NAME NOT IN ");

            strQry.AppendLine("('EMPLOYEE_TIME_ATTEND_BREAK_CURRENT'");
            strQry.AppendLine(",'EMPLOYEE_TIME_ATTEND_BREAK_HISTORY'");
            strQry.AppendLine(",'EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT'");
            strQry.AppendLine(",'EMPLOYEE_TIME_ATTEND_TIMESHEET_HISTORY'");
            strQry.AppendLine(",'EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT'");
            strQry.AppendLine(",'EMPLOYEE_BREAK_CURRENT'");
            strQry.AppendLine(",'EMPLOYEE_BREAK_HISTORY'");
            strQry.AppendLine(",'EMPLOYEE_TIMESHEET_CURRENT'");
            strQry.AppendLine(",'EMPLOYEE_TIMESHEET_HISTORY'");
            strQry.AppendLine(",'EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT'");
            strQry.AppendLine(",'EMPLOYEE_SALARY_BREAK_CURRENT'");
            strQry.AppendLine(",'EMPLOYEE_SALARY_BREAK_HISTORY'");
            strQry.AppendLine(",'EMPLOYEE_SALARY_TIMESHEET_CURRENT'");
            strQry.AppendLine(",'EMPLOYEE_SALARY_TIMESHEET_HISTORY'");
            strQry.AppendLine(",'EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT')");

            strQry.AppendLine(" AND C2.TABLE_NAME IS NULL");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "TableCompanyDelete");
            
            for (int intRow = 0; intRow < DataSet.Tables["TableCompanyDelete"].Rows.Count; intRow++)
            {
                strQry.Clear();

                if (DataSet.Tables["TableCompanyDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "DEVICE")
                {
                    strQry.AppendLine(" UPDATE T");
                    strQry.AppendLine(" SET COMPANY_NO = NULL");
                }
                else
                {
                    strQry.AppendLine(" DELETE T");
                }

                strQry.AppendLine(" FROM InteractPayrollClient.dbo." + DataSet.Tables["TableCompanyDelete"].Rows[intRow]["TABLE_NAME"].ToString() + " T");

                if (strFromProgram != "B")
                {
                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC");
                    strQry.AppendLine(" ON T.COMPANY_NO = PC.COMPANY_NO");
                }

                strQry.AppendLine(" WHERE T.COMPANY_NO = " + parint64CompanyNo.ToString());

                if (strFromProgram != "B")
                {
                    //NO PayCategories For Company
                    strQry.AppendLine(" AND PC.COMPANY_NO IS NULL ");
                }
                
                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }
        }
        
        public void Delete_Certain_PayCategory_Records(Int64 parint64CompanyNo, string strPayCategoryNoWages, string strPayCategoryNoSalaries, byte[] parbyteDataSet)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            string strPayCategoryNoTimeAttendance = "(-1)";

            Delete_Certain_PayCategory_Records_New(parint64CompanyNo, strPayCategoryNoWages, strPayCategoryNoSalaries, strPayCategoryNoTimeAttendance, parbyteDataSet);
        }

        public void Delete_Certain_PayCategory_Records_New(Int64 parint64CompanyNo, string strPayCategoryNoWages, string strPayCategoryNoSalaries, string strPayCategoryNoTimeAttendance, byte[] parbyteDataSet)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            DataSet pvtDataSetClient = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" C1.TABLE_NAME");
            strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS C1");

            strQry.AppendLine(" WHERE C1.COLUMN_NAME = 'PAY_CATEGORY_NO'");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), pvtDataSetClient, "TableDelete");

            for (int intRow = 0; intRow < pvtDataSetClient.Tables["TableDelete"].Rows.Count; intRow++)
            {
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo." + pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString());
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
                strQry.AppendLine(" AND PAY_CATEGORY_NO IN " + strPayCategoryNoWages);

                if (pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_BREAK_CURRENT"
                || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_BREAK_HISTORY"
                || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_TIMESHEET_CURRENT"
                || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_TIMESHEET_HISTORY"
                || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT")
                {
                }
                else
                {
                    if (pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_SALARY_BREAK_CURRENT"
                    || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_SALARY_BREAK_HISTORY"
                    || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_SALARY_TIMESHEET_CURRENT"
                    || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_SALARY_TIMESHEET_HISTORY"
                    || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT")
                    {
                        goto btnOK_Click_Salary_Continue;
                    }
                    else
                    {
                        if (pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_TIME_ATTEND_BREAK_CURRENT"
                        || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_TIME_ATTEND_BREAK_HISTORY"
                        || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT"
                        || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_TIME_ATTEND_TIMESHEET_HISTORY"
                        || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT")
                        {
                            goto btnOK_Click_TimeAttend_Continue;
                        }
                        else
                        {
                            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                        }
                    }
                }

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            btnOK_Click_Salary_Continue:

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo." + pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString());
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
                strQry.AppendLine(" AND PAY_CATEGORY_NO IN " + strPayCategoryNoSalaries);

                if (pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_BREAK_CURRENT"
                || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_BREAK_HISTORY"
                || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_TIMESHEET_CURRENT"
                || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_TIMESHEET_HISTORY"
                || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT")
                {
                    continue;
                }
                else
                {
                    if (pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_SALARY_BREAK_CURRENT"
                    || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_SALARY_BREAK_HISTORY"
                    || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_SALARY_TIMESHEET_CURRENT"
                    || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_SALARY_TIMESHEET_HISTORY"
                    || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT")
                    {
                    }
                    else
                    {
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                    }
                }

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            btnOK_Click_TimeAttend_Continue:

                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo." + pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString());
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
                strQry.AppendLine(" AND PAY_CATEGORY_NO IN " + strPayCategoryNoTimeAttendance);

                if (pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_BREAK_CURRENT"
                || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_BREAK_HISTORY"
                || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_TIMESHEET_CURRENT"
                || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_TIMESHEET_HISTORY"
                || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT"
                || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_SALARY_BREAK_CURRENT"
                || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_SALARY_BREAK_HISTORY"
                || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_SALARY_TIMESHEET_CURRENT"
                || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_SALARY_TIMESHEET_HISTORY"
                || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT")
                {
                    continue;
                }
                else
                {
                    if (pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_TIME_ATTEND_BREAK_CURRENT"
                    || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_TIME_ATTEND_BREAK_HISTORY"
                    || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT"
                    || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_TIME_ATTEND_TIMESHEET_HISTORY"
                    || pvtDataSetClient.Tables["TableDelete"].Rows[intRow]["TABLE_NAME"].ToString() == "EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT")
                    {
                    }
                    else
                    {
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");
                    }
                }

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }
        }

        public byte[] Check_PayCategory_Records(Int64 parint64CompanyNo, string strPayCategoryFilterWages, string strPayCategoryFilterSalaries)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            string strPayCategoryFilterTimeAttendance = " AND PAY_CATEGORY_NO IN (-1)";

            byte[] bytCompress = Check_PayCategory_Records_New(parint64CompanyNo, strPayCategoryFilterWages, strPayCategoryFilterSalaries, strPayCategoryFilterTimeAttendance);

            return bytCompress;
        }


        public byte[] Check_PayCategory_Records_New(Int64 parint64CompanyNo, string strPayCategoryFilterWages, string strPayCategoryFilterSalaries, string strPayCategoryFilterTimeAttendance)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            StringBuilder strQryOther = new StringBuilder();
            DataSet DataSet = new DataSet();
            DataSet DataSetOther = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_DESC");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");

            strQry.Append(strPayCategoryFilterWages.Replace("IN", "NOT IN"));

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_DESC");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");

            strQry.Append(strPayCategoryFilterSalaries.Replace("IN", "NOT IN"));

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_DESC");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");

            strQry.Append(strPayCategoryFilterTimeAttendance.Replace("IN", "NOT IN"));

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Temp");

            //2017-09-28
            strQryOther.Clear();

            strQryOther.AppendLine(" SELECT ");
            strQryOther.AppendLine(" TABLE_NAME ");

            strQryOther.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

            strQryOther.AppendLine(" WHERE TABLE_NAME = 'EMPLOYEE' ");
            strQryOther.AppendLine(" AND COLUMN_NAME = 'UPLOAD_CLOCK_PARAMETERS_IND' ");

            this.clsDBConnectionObjects.Create_DataTable_Client(strQryOther.ToString(), DataSetOther, "UploadClockParametersInd");

            if (DataSetOther.Tables["UploadClockParametersInd"].Rows.Count > 0)
            {
                //2017-09-28
                strQryOther.Clear();

                strQryOther.AppendLine(" SELECT ");
                strQryOther.AppendLine(" E.EMPLOYEE_NO ");
                strQryOther.AppendLine(",E.PAY_CATEGORY_TYPE ");
                strQryOther.AppendLine(",E.USE_EMPLOYEE_NO_IND ");
                strQryOther.AppendLine(",E.EMPLOYEE_PIN ");
                strQryOther.AppendLine(",E.EMPLOYEE_RFID_CARD_NO ");

                strQryOther.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E ");
                
                strQryOther.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

                strQryOther.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                strQryOther.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                strQryOther.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                strQryOther.AppendLine(" INNER JOIN ");
                strQryOther.AppendLine("( ");

                //Add Top Query
                strQryOther.Append(strQry);

                strQryOther.AppendLine(") AS PAY_CATEGORY_TABLE_TEMP ");
                strQryOther.AppendLine(" ON EPC.PAY_CATEGORY_NO = PAY_CATEGORY_TABLE_TEMP.PAY_CATEGORY_NO ");
                strQryOther.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PAY_CATEGORY_TABLE_TEMP.PAY_CATEGORY_TYPE ");

                strQryOther.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo.ToString());
                strQryOther.AppendLine(" AND E.UPLOAD_CLOCK_PARAMETERS_IND = 'Y'");

                strQryOther.AppendLine(" GROUP BY ");
                strQryOther.AppendLine(" E.EMPLOYEE_NO ");
                strQryOther.AppendLine(",E.PAY_CATEGORY_TYPE ");
                strQryOther.AppendLine(",E.USE_EMPLOYEE_NO_IND ");
                strQryOther.AppendLine(",E.EMPLOYEE_PIN ");
                strQryOther.AppendLine(",E.EMPLOYEE_RFID_CARD_NO ");

                clsDBConnectionObjects.Create_DataTable_Client(strQryOther.ToString(), DataSet, "EmployeeUploadParameters");
            }

            DataSetOther.Tables.Remove(DataSetOther.Tables["UploadClockParametersInd"]);
            DataSetOther.Dispose();
            DataSetOther = null;

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public void Download_Records(Int64 parint64CompanyNo, string strPayCategoryFilterWages, string strPayCategoryFilterSalaries, string strCostCentresWages, string strCostCentresSalaries, byte[] parbyteDataSet)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            string strPayCategoryFilterTimeAttendance = " AND PAY_CATEGORY_NO IN (-1)";
            string strCostCentresTimeAttendance = "";

            Download_Records_New(parint64CompanyNo, strPayCategoryFilterWages, strPayCategoryFilterSalaries, strPayCategoryFilterTimeAttendance, strCostCentresWages, strCostCentresSalaries, strCostCentresTimeAttendance, parbyteDataSet);
        }

        public byte[] Download_Records_New(Int64 parint64CompanyNo, string strPayCategoryFilterWages, string strPayCategoryFilterSalaries, string strPayCategoryFilterTimeAttendance, string strCostCentresWages, string strCostCentresSalaries, string strCostCentresTimeAttendance, byte[] parbyteDataSet)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            DataSet pvtDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);
            DataSet DataSetClient = new DataSet();
            //20170321
            DataSet DataSetUpload = new DataSet();

            object[] objFind = new object[3];
            object[] objFind2 = new object[2];

            int intFindRow = -1;
            
            try
            {
                string strFileDirectory = AppDomain.CurrentDomain.BaseDirectory + "Backup";

                if (Directory.Exists(strFileDirectory))
                {
                }
                else
                {
                    Directory.CreateDirectory(strFileDirectory);
                }

                string strDataBaseName = "InteractPayrollClient";
#if(DEBUG)
                strDataBaseName = "InteractPayrollClient_Debug";
#endif
                string strBackupFileName = strDataBaseName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_Backup_Before_Synchronization.bak";

                strQry.Clear();
                strQry.AppendLine("BACKUP DATABASE " + strDataBaseName + " TO DISK = '" + strFileDirectory + "\\" + strBackupFileName + "' WITH CHECKSUM ");
#if (DEBUG)
#else

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString(), 60);
#endif
            }
            catch (Exception ex)
            {
            }

            //2017-09-28
           
            strQry.Clear();

            strQry.AppendLine(" UPDATE E ");

            strQry.AppendLine(" SET E.UPLOAD_CLOCK_PARAMETERS_IND = 'N'");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E ");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(strPayCategoryFilterWages.Replace("PAY_CATEGORY_NO", "EPC.PAY_CATEGORY_NO"));

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" AND E.UPLOAD_CLOCK_PARAMETERS_IND = 'Y'");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            
            //2017-09-28
            strQry.Clear();

            strQry.AppendLine(" UPDATE E ");

            strQry.AppendLine(" SET E.UPLOAD_CLOCK_PARAMETERS_IND = 'N'");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E ");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(strPayCategoryFilterSalaries.Replace("PAY_CATEGORY_NO", "EPC.PAY_CATEGORY_NO"));
                
            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");
            strQry.AppendLine(" AND E.UPLOAD_CLOCK_PARAMETERS_IND = 'Y'");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
           
            //2017-09-28
            strQry.Clear();

            strQry.AppendLine(" UPDATE E ");

            strQry.AppendLine(" SET E.UPLOAD_CLOCK_PARAMETERS_IND = 'N'");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E ");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(strPayCategoryFilterTimeAttendance.Replace("PAY_CATEGORY_NO", "EPC.PAY_CATEGORY_NO"));
                
            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T'");
            strQry.AppendLine(" AND E.UPLOAD_CLOCK_PARAMETERS_IND = 'Y'");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
           
            //20170321
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" USER_NO");
            strQry.AppendLine(",FINGER_NO");
            strQry.AppendLine(",FINGER_TEMPLATE");
            strQry.AppendLine(",CREATION_DATETIME");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE ");

            strQry.AppendLine(" WHERE USER_NO = -1 ");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSetUpload, "UserFingerPrintTemplateUpload");

            //20170321
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EMPLOYEE_NO");
            strQry.AppendLine(",FINGER_NO");
            strQry.AppendLine(",FINGER_TEMPLATE");
            strQry.AppendLine(",CREATION_DATETIME");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE ");

            strQry.AppendLine(" WHERE COMPANY_NO = -1 ");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSetUpload, "EmployeeFingerPrintTemplateUpload");

            DataView pvtCompanyDataView = new DataView(pvtDataSet.Tables["Company"],
                "COMPANY_NO = " + parint64CompanyNo.ToString(),
                "",
                DataViewRowState.CurrentRows);

            DataView pvtDepartmentDataView;

            try
            {
                pvtDepartmentDataView = new DataView(pvtDataSet.Tables["Department"],
                    "COMPANY_NO = " + parint64CompanyNo.ToString(),
                    "",
                    DataViewRowState.CurrentRows);
            }
            catch
            {
                //Cater For Old ClientDataDownload - before it gets Downloaded
                pvtDepartmentDataView = new DataView(pvtDataSet.Tables["Department"],
                "",
                "",
                DataViewRowState.CurrentRows);
            }

            DataView pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                "COMPANY_NO = " + parint64CompanyNo.ToString(),
                "EMPLOYEE_NO",
                DataViewRowState.CurrentRows);

            //20170321
            DataView pvtEmployeeFingerTemplateDataView = new DataView(pvtDataSet.Tables["EmployeeFingerTemplate"],
               "COMPANY_NO = " + parint64CompanyNo.ToString(),
               "EMPLOYEE_NO,FINGER_NO",
               DataViewRowState.CurrentRows);

            //20170420
            DataView pvtUserFingerTemplateDataView = new DataView(pvtDataSet.Tables["UserFingerTemplate"],
               "",
               "USER_NO,FINGER_NO",
               DataViewRowState.CurrentRows);

            DataView pvtEmployeePayCategoryDataView = new DataView(pvtDataSet.Tables["EmployeePayCategory"],
                "COMPANY_NO = " + parint64CompanyNo.ToString(),
                "EMPLOYEE_NO,PAY_CATEGORY_NO,PAY_CATEGORY_TYPE",
                DataViewRowState.CurrentRows);

            string strPayCategoryFilter = " AND ((PAY_CATEGORY_TYPE = 'W' " + strPayCategoryFilterWages + ") OR (PAY_CATEGORY_TYPE = 'S' " + strPayCategoryFilterSalaries + ") OR (PAY_CATEGORY_TYPE = 'T' " + strPayCategoryFilterTimeAttendance + "))";

            DataView pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"],
                "COMPANY_NO = " + parint64CompanyNo.ToString() + strPayCategoryFilter,
                "PAY_CATEGORY_NO",
                DataViewRowState.CurrentRows);

            DataView pvtPayCategoryBreakDataView = new DataView(pvtDataSet.Tables["PayCategoryBreak"],
                "COMPANY_NO = " + parint64CompanyNo.ToString(),
                "PAY_CATEGORY_NO,PAY_CATEGORY_TYPE",
                DataViewRowState.CurrentRows);

            DataView pvtUserCompanyDataView = new DataView(pvtDataSet.Tables["UserCompany"],
                "COMPANY_NO = " + parint64CompanyNo.ToString(),
                "USER_NO",
                DataViewRowState.CurrentRows);

            DataView pvtUserPayCategoryDataView = new DataView(pvtDataSet.Tables["UserPayCategory"],
                "COMPANY_NO = " + parint64CompanyNo.ToString(),
                "USER_NO",
                DataViewRowState.CurrentRows);

            //2016-12-08 Will Exist on Server
            DataView pvtUserPayCategoryDepartmentDataView = new DataView(pvtDataSet.Tables["UserPayCategoryDepartment"],
                "COMPANY_NO = " + parint64CompanyNo.ToString(),
                "USER_NO",
                DataViewRowState.CurrentRows);

            DataView pvtUserEmployeeDataView = new DataView(pvtDataSet.Tables["UserEmployee"],
                "COMPANY_NO = " + parint64CompanyNo.ToString(),
                "USER_NO",
                DataViewRowState.CurrentRows);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.PAY_CATEGORY_BREAK");

            strQry.AppendLine(" SET KEEP_IND = 'N'");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.DEPARTMENT");

            strQry.AppendLine(" SET KEEP_IND = 'N'");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.USER_PAY_CATEGORY");

            strQry.AppendLine(" SET KEEP_IND = 'N'");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.USER_PAY_CATEGORY_DEPARTMENT");

            strQry.AppendLine(" SET KEEP_IND = 'N'");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.USER_EMPLOYEE");

            strQry.AppendLine(" SET KEEP_IND = 'N'");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY");

            strQry.AppendLine(" SET KEEP_IND = 'N'");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.EMPLOYEE");

            strQry.AppendLine(" SET KEEP_IND = 'N'");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.USER_COMPANY_ACCESS");

            strQry.AppendLine(" SET KEEP_IND = 'N'");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            //20170419
            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE");

            strQry.AppendLine(" SET KEEP_IND = 'N'");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            //20170419
            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");

            strQry.AppendLine(" SET KEEP_IND = 'N'");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            //Fix Departments
            for (int intRow = 0; intRow < pvtDepartmentDataView.Count; intRow++)
            {
                if (DataSetClient.Tables["Department"] != null)
                {
                    DataSetClient.Tables.Remove("Department");
                }

                //Read From Client Database
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" DEPARTMENT_NO");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEPARTMENT ");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + pvtDepartmentDataView[intRow]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(pvtDepartmentDataView[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                strQry.AppendLine(" AND DEPARTMENT_NO = " + pvtDepartmentDataView[intRow]["DEPARTMENT_NO"].ToString());

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSetClient, "Department");

                if (DataSetClient.Tables["Department"].Rows.Count == 0)
                {
                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.DEPARTMENT");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",DEPARTMENT_NO");
                    strQry.AppendLine(",DEPARTMENT_DESC");
                    strQry.AppendLine(",KEEP_IND)");
                    strQry.AppendLine(" VALUES");
                    strQry.AppendLine("(" + parint64CompanyNo);
                    strQry.AppendLine("," + pvtDepartmentDataView[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtDepartmentDataView[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine("," + pvtDepartmentDataView[intRow]["DEPARTMENT_NO"].ToString());
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtDepartmentDataView[intRow]["DEPARTMENT_DESC"].ToString()));
                    strQry.AppendLine(",'Y')");
                }
                else
                {
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.DEPARTMENT");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" KEEP_IND = 'Y' ");
                    strQry.AppendLine(",DEPARTMENT_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(pvtDepartmentDataView[intRow]["DEPARTMENT_DESC"].ToString()));
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + pvtDepartmentDataView[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(pvtDepartmentDataView[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine(" AND DEPARTMENT_NO = " + pvtDepartmentDataView[intRow]["DEPARTMENT_NO"].ToString());
                }

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            strQry.Clear();

            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEPARTMENT");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND KEEP_IND = 'N'");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            //Extract Deleted Records on Server
            DataView pvtUserIdDataView = new DataView(pvtDataSet.Tables["UserId"],
                "COMPANY_NO = " + parint64CompanyNo.ToString() + " AND NOT DATETIME_DELETE_RECORD IS NULL",
                "USER_NO",
                DataViewRowState.CurrentRows);

            if (pvtUserIdDataView.Count != 0)
            {
                //Read From Client Database
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" USER_NO");
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_ID ");
                strQry.AppendLine(" WHERE INTERNET_IND = 'Y'");
                strQry.AppendLine(" AND USER_NO IN (");

                for (int intRow = 0; intRow < pvtUserIdDataView.Count; intRow++)
                {
                    strQry.AppendLine(pvtUserIdDataView[intRow]["USER_NO"].ToString());
                }

                strQry.AppendLine(")");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSetClient, "UserId");

                for (int intRow = 0; intRow < DataSetClient.Tables["UserId"].Rows.Count; intRow++)
                {
                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.USER_ID");
                    strQry.AppendLine(" WHERE USER_NO = " + Convert.ToInt32(DataSetClient.Tables["UserId"].Rows[0]["USER_NO"]));
                    strQry.AppendLine(" AND INTERNET_IND = 'Y'");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.USER_COMPANY_ACCESS");
                    strQry.AppendLine(" WHERE USER_NO = " + Convert.ToInt32(DataSetClient.Tables["UserId"].Rows[0]["USER_NO"]));

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }
            }

            //Extract Active Records on Server
            pvtUserIdDataView = null;
            pvtUserIdDataView = new DataView(pvtDataSet.Tables["UserId"],
                "COMPANY_NO = " + parint64CompanyNo.ToString() + " AND DATETIME_DELETE_RECORD IS NULL",
                "USER_NO",
                DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < pvtUserIdDataView.Count; intRow++)
            {
                //Read From Client Database
                if (DataSetClient.Tables["UserId"] != null)
                {
                    DataSetClient.Tables.Remove("UserId");
                }

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" USER_NO");
                strQry.AppendLine(",USER_ID");
                strQry.AppendLine(",FIRSTNAME");
                strQry.AppendLine(",SURNAME");
                strQry.AppendLine(",SYSTEM_ADMINISTRATOR_IND");
                strQry.AppendLine(",EMAIL");
                strQry.AppendLine(",PASSWORD");
                strQry.AppendLine(",RESET");
                //2017-05-10
                strQry.AppendLine(",USER_CLOCK_PIN");
                
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_ID ");

                strQry.AppendLine(" WHERE USER_NO = " + pvtUserIdDataView[intRow]["USER_NO"].ToString());
                strQry.AppendLine(" AND INTERNET_IND = 'Y'");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSetClient, "UserId");

                if (DataSetClient.Tables["UserId"].Rows.Count == 0)
                {
                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.USER_ID");
                    strQry.AppendLine("(USER_NO");
                    strQry.AppendLine(",USER_ID");
                    strQry.AppendLine(",FIRSTNAME");
                    strQry.AppendLine(",SURNAME");
                    strQry.AppendLine(",SYSTEM_ADMINISTRATOR_IND");
                    strQry.AppendLine(",PASSWORD");
                    strQry.AppendLine(",EMAIL");
                    strQry.AppendLine(",RESET");
                    //2017-05-10
                    strQry.AppendLine(",USER_CLOCK_PIN");
                    strQry.AppendLine(",INTERNET_IND)");

                    strQry.AppendLine(" VALUES");

                    strQry.AppendLine("(" + pvtUserIdDataView[intRow]["USER_NO"].ToString());
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtUserIdDataView[intRow]["USER_ID"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtUserIdDataView[intRow]["FIRSTNAME"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtUserIdDataView[intRow]["SURNAME"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtUserIdDataView[intRow]["SYSTEM_ADMINISTRATOR_IND"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtUserIdDataView[intRow]["PASSWORD"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtUserIdDataView[intRow]["EMAIL"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtUserIdDataView[intRow]["RESET"].ToString()));
                    //2017-05-10
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtUserIdDataView[intRow]["USER_CLOCK_PIN"].ToString()));
                    strQry.AppendLine(",'Y')");
                }
                else
                {
                    if (DataSetClient.Tables["UserId"].Rows[0]["USER_ID"].ToString() == pvtUserIdDataView[intRow]["USER_ID"].ToString()
                    && DataSetClient.Tables["UserId"].Rows[0]["FIRSTNAME"].ToString() == pvtUserIdDataView[intRow]["FIRSTNAME"].ToString()
                    && DataSetClient.Tables["UserId"].Rows[0]["SURNAME"].ToString() == pvtUserIdDataView[intRow]["SURNAME"].ToString()
                    && DataSetClient.Tables["UserId"].Rows[0]["SYSTEM_ADMINISTRATOR_IND"].ToString() == pvtUserIdDataView[intRow]["SYSTEM_ADMINISTRATOR_IND"].ToString()
                    && DataSetClient.Tables["UserId"].Rows[0]["EMAIL"].ToString() == pvtUserIdDataView[intRow]["EMAIL"].ToString()
                    && DataSetClient.Tables["UserId"].Rows[0]["PASSWORD"].ToString() == pvtUserIdDataView[intRow]["PASSWORD"].ToString()
                    && DataSetClient.Tables["UserId"].Rows[0]["RESET"].ToString() == pvtUserIdDataView[intRow]["RESET"].ToString()
                    //2017-05-10
                    && DataSetClient.Tables["UserId"].Rows[0]["USER_CLOCK_PIN"].ToString() == pvtUserIdDataView[intRow]["USER_CLOCK_PIN"].ToString())
                    {
                        continue;
                    }
                    else
                    {
                        strQry.Clear();

                        strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.USER_ID");
                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" USER_ID = " + clsDBConnectionObjects.Text2DynamicSQL(pvtUserIdDataView[intRow]["USER_ID"].ToString()));
                        strQry.AppendLine(",FIRSTNAME = " + clsDBConnectionObjects.Text2DynamicSQL(pvtUserIdDataView[intRow]["FIRSTNAME"].ToString()));
                        strQry.AppendLine(",SURNAME = " + clsDBConnectionObjects.Text2DynamicSQL(pvtUserIdDataView[intRow]["SURNAME"].ToString()));
                        strQry.AppendLine(",SYSTEM_ADMINISTRATOR_IND = " + clsDBConnectionObjects.Text2DynamicSQL(pvtUserIdDataView[intRow]["SYSTEM_ADMINISTRATOR_IND"].ToString()));
                        strQry.AppendLine(",PASSWORD = " + clsDBConnectionObjects.Text2DynamicSQL(pvtUserIdDataView[intRow]["PASSWORD"].ToString()));
                        strQry.AppendLine(",EMAIL = " + clsDBConnectionObjects.Text2DynamicSQL(pvtUserIdDataView[intRow]["EMAIL"].ToString()));
                        strQry.AppendLine(",RESET = " + clsDBConnectionObjects.Text2DynamicSQL(pvtUserIdDataView[intRow]["RESET"].ToString()));
                        //2017-05-10
                        strQry.AppendLine(",USER_CLOCK_PIN = " + clsDBConnectionObjects.Text2DynamicSQL(pvtUserIdDataView[intRow]["USER_CLOCK_PIN"].ToString()));

                        strQry.AppendLine(" WHERE USER_NO = " + pvtUserIdDataView[intRow]["USER_NO"].ToString());
                        strQry.AppendLine(" AND INTERNET_IND = 'Y'");
                    }
                }

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            for (int intRow = 0; intRow < pvtUserCompanyDataView.Count; intRow++)
            {
                //Read From Client Database
                if (DataSetClient.Tables["UserCompany"] != null)
                {
                    DataSetClient.Tables.Remove("UserCompany");
                }

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" USER_NO");
                strQry.AppendLine(",COMPANY_NO");
                strQry.AppendLine(",COMPANY_ACCESS_IND");
                strQry.AppendLine(",ACCESS_LAYER_IND");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_COMPANY_ACCESS ");

                strQry.AppendLine(" WHERE USER_NO = " + pvtUserCompanyDataView[intRow]["USER_NO"].ToString());
                strQry.AppendLine(" AND COMPANY_NO = " + pvtUserCompanyDataView[intRow]["COMPANY_NO"].ToString());

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSetClient, "UserCompany");

                if (DataSetClient.Tables["UserCompany"].Rows.Count == 0)
                {
                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.USER_COMPANY_ACCESS");
                    strQry.AppendLine("(USER_NO");
                    strQry.AppendLine(",COMPANY_NO");
                    strQry.AppendLine(",ACCESS_LAYER_IND");
                    strQry.AppendLine(",COMPANY_ACCESS_IND");
                    strQry.AppendLine(",KEEP_IND)");

                    strQry.AppendLine(" VALUES");

                    strQry.AppendLine("(" + pvtUserCompanyDataView[intRow]["USER_NO"].ToString());
                    strQry.AppendLine("," + pvtUserCompanyDataView[intRow]["COMPANY_NO"].ToString());
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtUserCompanyDataView[intRow]["ACCESS_LAYER_IND"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtUserCompanyDataView[intRow]["COMPANY_ACCESS_IND"].ToString()));
                    strQry.AppendLine(",'Y')");
                }
                else
                {
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.USER_COMPANY_ACCESS");

                    strQry.AppendLine(" SET ");

                    if (DataSetClient.Tables["UserCompany"].Rows[0]["USER_NO"].ToString() == pvtUserCompanyDataView[intRow]["USER_NO"].ToString()
                    && DataSetClient.Tables["UserCompany"].Rows[0]["COMPANY_NO"].ToString() == pvtUserCompanyDataView[intRow]["COMPANY_NO"].ToString()
                    && DataSetClient.Tables["UserCompany"].Rows[0]["ACCESS_LAYER_IND"].ToString() == pvtUserCompanyDataView[intRow]["ACCESS_LAYER_IND"].ToString()
                    && DataSetClient.Tables["UserCompany"].Rows[0]["COMPANY_ACCESS_IND"].ToString() == pvtUserCompanyDataView[intRow]["COMPANY_ACCESS_IND"].ToString())
                    {
                        strQry.AppendLine(" KEEP_IND = 'Y'");
                    }
                    else
                    {
                        strQry.AppendLine(" COMPANY_ACCESS_IND = " + clsDBConnectionObjects.Text2DynamicSQL(pvtUserCompanyDataView[intRow]["COMPANY_ACCESS_IND"].ToString()));
                        strQry.AppendLine(",ACCESS_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(pvtUserCompanyDataView[intRow]["ACCESS_LAYER_IND"].ToString()));
                        strQry.AppendLine(",KEEP_IND = 'Y'");
                    }

                    strQry.AppendLine(" WHERE USER_NO = " + pvtUserCompanyDataView[intRow]["USER_NO"].ToString());
                    strQry.AppendLine(" AND COMPANY_NO = " + pvtUserCompanyDataView[intRow]["COMPANY_NO"].ToString());
                }

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            //Now Cleanup
            strQry.Clear();

            strQry.AppendLine(" DELETE UCA FROM InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA");

            strQry.AppendLine(" WHERE UCA.COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND UCA.KEEP_IND = 'N'");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();

            strQry.AppendLine(" DELETE UI FROM InteractPayrollClient.dbo.USER_ID UI");

            strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA");
            strQry.AppendLine(" ON UI.USER_NO = UCA.USER_NO");

            //No USER_COMPANY_ACCESS for USER_ID
            strQry.AppendLine(" WHERE  UCA.USER_NO IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            for (int intRow = 0; intRow < pvtUserPayCategoryDataView.Count; intRow++)
            {
                //Read From Client Database
                if (DataSetClient.Tables["UserPayCategory"] != null)
                {
                    DataSetClient.Tables.Remove("UserPayCategory");
                }

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" USER_NO");
                strQry.AppendLine(",COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_PAY_CATEGORY ");

                strQry.AppendLine(" WHERE USER_NO = " + pvtUserPayCategoryDataView[intRow]["USER_NO"].ToString());
                strQry.AppendLine(" AND COMPANY_NO = " + pvtUserPayCategoryDataView[intRow]["COMPANY_NO"].ToString());
                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + pvtUserPayCategoryDataView[intRow]["PAY_CATEGORY_NO"].ToString());
                //strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + this.flxgPayrollType[this.flxgPayrollType.Row,1].ToString().Substring(0,1) + "'");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSetClient, "UserPayCategory");

                if (DataSetClient.Tables["UserPayCategory"].Rows.Count == 0)
                {
                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.USER_PAY_CATEGORY");
                    strQry.AppendLine("(USER_NO");
                    strQry.AppendLine(",COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",KEEP_IND)");

                    strQry.AppendLine(" VALUES");

                    strQry.AppendLine("(" + pvtUserPayCategoryDataView[intRow]["USER_NO"].ToString());
                    strQry.AppendLine("," + pvtUserPayCategoryDataView[intRow]["COMPANY_NO"].ToString());
                    strQry.AppendLine("," + pvtUserPayCategoryDataView[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(",'" + pvtUserPayCategoryDataView[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'");
                    strQry.AppendLine(",'Y')");
                }
                else
                {
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.USER_PAY_CATEGORY");

                    strQry.AppendLine(" SET KEEP_IND = 'Y' ");

                    strQry.AppendLine(" WHERE USER_NO = " + pvtUserPayCategoryDataView[intRow]["USER_NO"].ToString());
                    strQry.AppendLine(" AND COMPANY_NO = " + pvtUserPayCategoryDataView[intRow]["COMPANY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + pvtUserPayCategoryDataView[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + pvtUserPayCategoryDataView[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'");
                }

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            strQry.Clear();

            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.USER_PAY_CATEGORY");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");

            //Only Selected PAY_CATEGORY_NOs
            strQry.Append(strPayCategoryFilterTimeAttendance);

            strQry.AppendLine(" AND KEEP_IND = 'N'");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();

            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.USER_PAY_CATEGORY");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");

            //Only Selected PAY_CATEGORY_NOs
            strQry.Append(strPayCategoryFilterWages);

            strQry.AppendLine(" AND KEEP_IND = 'N'");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();

            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.USER_PAY_CATEGORY");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");

            //Only Selected PAY_CATEGORY_NOs
            strQry.Append(strPayCategoryFilterSalaries);

            strQry.AppendLine(" AND KEEP_IND = 'N'");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            //2016-12-08 (Check If Client Database has Table)
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" TABLE_NAME ");

            strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS ");

            strQry.AppendLine(" WHERE TABLE_NAME = 'USER_PAY_CATEGORY_DEPARTMENT' ");

            this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSetClient, "UserPayCategoryDepartmentExists");

            if (DataSetClient.Tables["UserPayCategoryDepartmentExists"].Rows.Count > 0)
            {
                for (int intRow = 0; intRow < pvtUserPayCategoryDepartmentDataView.Count; intRow++)
                {
                    //Read From Client Database
                    if (DataSetClient.Tables["UserPayCategoryDepartment"] != null)
                    {
                        DataSetClient.Tables.Remove("UserPayCategoryDepartment");
                    }

                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" USER_NO");
                    strQry.AppendLine(",COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",DEPARTMENT_NO");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_PAY_CATEGORY_DEPARTMENT ");

                    strQry.AppendLine(" WHERE USER_NO = " + pvtUserPayCategoryDepartmentDataView[intRow]["USER_NO"].ToString());
                    strQry.AppendLine(" AND COMPANY_NO = " + pvtUserPayCategoryDepartmentDataView[intRow]["COMPANY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + pvtUserPayCategoryDepartmentDataView[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(pvtUserPayCategoryDepartmentDataView[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine(" AND DEPARTMENT_NO = " + pvtUserPayCategoryDepartmentDataView[intRow]["DEPARTMENT_NO"].ToString());

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSetClient, "UserPayCategoryDepartment");

                    if (DataSetClient.Tables["UserPayCategoryDepartment"].Rows.Count == 0)
                    {
                        strQry.Clear();

                        strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.USER_PAY_CATEGORY_DEPARTMENT");
                        strQry.AppendLine("(USER_NO");
                        strQry.AppendLine(",COMPANY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",DEPARTMENT_NO");
                        strQry.AppendLine(",KEEP_IND)");

                        strQry.AppendLine(" VALUES");

                        strQry.AppendLine("(" + pvtUserPayCategoryDepartmentDataView[intRow]["USER_NO"].ToString());
                        strQry.AppendLine("," + pvtUserPayCategoryDepartmentDataView[intRow]["COMPANY_NO"].ToString());
                        strQry.AppendLine("," + pvtUserPayCategoryDepartmentDataView[intRow]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtUserPayCategoryDepartmentDataView[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                        strQry.AppendLine("," + pvtUserPayCategoryDepartmentDataView[intRow]["DEPARTMENT_NO"].ToString());
                        strQry.AppendLine(",'Y')");
                    }
                    else
                    {
                        strQry.Clear();

                        strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.USER_PAY_CATEGORY_DEPARTMENT");
                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" KEEP_IND = 'Y' ");

                        strQry.AppendLine(" WHERE USER_NO = " + pvtUserPayCategoryDepartmentDataView[intRow]["USER_NO"].ToString());
                        strQry.AppendLine(" AND COMPANY_NO = " + pvtUserPayCategoryDepartmentDataView[intRow]["COMPANY_NO"].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + pvtUserPayCategoryDepartmentDataView[intRow]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(pvtUserPayCategoryDepartmentDataView[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                        strQry.AppendLine(" AND DEPARTMENT_NO = " + pvtUserPayCategoryDepartmentDataView[intRow]["DEPARTMENT_NO"].ToString());
                    }

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }

                strQry.Clear();

                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.USER_PAY_CATEGORY_DEPARTMENT");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
                strQry.AppendLine(" AND KEEP_IND = 'N'");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            for (int intRow = 0; intRow < pvtUserEmployeeDataView.Count; intRow++)
            {
                //Read From Client Database
                if (DataSetClient.Tables["UserEmployee"] != null)
                {
                    DataSetClient.Tables.Remove("UserEmployee");
                }

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" USER_NO");
                strQry.AppendLine(",COMPANY_NO");
                strQry.AppendLine(",EMPLOYEE_NO");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_EMPLOYEE ");

                strQry.AppendLine(" WHERE USER_NO = " + pvtUserEmployeeDataView[intRow]["USER_NO"].ToString());
                strQry.AppendLine(" AND COMPANY_NO = " + pvtUserEmployeeDataView[intRow]["COMPANY_NO"].ToString());
                strQry.AppendLine(" AND EMPLOYEE_NO = " + pvtUserEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString());
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + pvtUserEmployeeDataView[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSetClient, "UserEmployee");

                if (DataSetClient.Tables["UserEmployee"].Rows.Count == 0)
                {
                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.USER_EMPLOYEE");
                    strQry.AppendLine("(USER_NO");
                    strQry.AppendLine(",COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",KEEP_IND)");

                    strQry.AppendLine(" VALUES");

                    strQry.AppendLine("(" + pvtUserEmployeeDataView[intRow]["USER_NO"].ToString());
                    strQry.AppendLine("," + pvtUserEmployeeDataView[intRow]["COMPANY_NO"].ToString());
                    strQry.AppendLine("," + pvtUserEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString());
                    strQry.AppendLine(",'" + pvtUserEmployeeDataView[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'");
                    strQry.AppendLine(",'Y')");
                }
                else
                {
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.USER_EMPLOYEE");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" KEEP_IND = 'Y' ");

                    strQry.AppendLine(" WHERE USER_NO = " + pvtUserEmployeeDataView[intRow]["USER_NO"].ToString());
                    strQry.AppendLine(" AND COMPANY_NO = " + pvtUserEmployeeDataView[intRow]["COMPANY_NO"].ToString());
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + pvtUserEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + pvtUserEmployeeDataView[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'");
                }

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            strQry.Clear();

            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.USER_EMPLOYEE");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND KEEP_IND = 'N'");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            //Read From Client Database
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",COMPANY_DESC");
            strQry.AppendLine(",POST_ADDR_LINE1");
            strQry.AppendLine(",POST_ADDR_LINE2");
            strQry.AppendLine(",POST_ADDR_LINE3");
            strQry.AppendLine(",POST_ADDR_CODE");
            strQry.AppendLine(",RES_UNIT_NUMBER");
            strQry.AppendLine(",RES_COMPLEX");
            strQry.AppendLine(",RES_STREET_NUMBER");
            strQry.AppendLine(",RES_STREET_NAME");
            strQry.AppendLine(",RES_SUBURB");
            strQry.AppendLine(",RES_CITY");
            strQry.AppendLine(",RES_ADDR_CODE");
            strQry.AppendLine(",FINGERPRINT_ENGINE");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.COMPANY");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSetClient, "Company");

            if (DataSetClient.Tables["Company"].Rows.Count == 0)
            {
                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.COMPANY");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",COMPANY_DESC");

                strQry.AppendLine(",POST_ADDR_LINE1");
                strQry.AppendLine(",POST_ADDR_LINE2");
                strQry.AppendLine(",POST_ADDR_LINE3");
                strQry.AppendLine(",POST_ADDR_CODE");

                strQry.AppendLine(",RES_UNIT_NUMBER");
                strQry.AppendLine(",RES_COMPLEX");
                strQry.AppendLine(",RES_STREET_NUMBER");
                strQry.AppendLine(",RES_STREET_NAME");
                strQry.AppendLine(",RES_SUBURB");
                strQry.AppendLine(",RES_CITY");
                strQry.AppendLine(",RES_ADDR_CODE");
                strQry.AppendLine(",FINGERPRINT_ENGINE)");

                strQry.AppendLine(" VALUES ");

                strQry.AppendLine("(" + parint64CompanyNo.ToString());
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtCompanyDataView[0]["COMPANY_DESC"].ToString()));

                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtCompanyDataView[0]["POST_ADDR_LINE1"].ToString()));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtCompanyDataView[0]["POST_ADDR_LINE2"].ToString()));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtCompanyDataView[0]["POST_ADDR_LINE3"].ToString()));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtCompanyDataView[0]["POST_ADDR_CODE"].ToString()));

                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtCompanyDataView[0]["RES_UNIT_NUMBER"].ToString()));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtCompanyDataView[0]["RES_COMPLEX"].ToString()));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtCompanyDataView[0]["RES_STREET_NUMBER"].ToString()));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtCompanyDataView[0]["RES_STREET_NAME"].ToString()));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtCompanyDataView[0]["RES_SUBURB"].ToString()));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtCompanyDataView[0]["RES_CITY"].ToString()));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtCompanyDataView[0]["RES_ADDR_CODE"].ToString()));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtCompanyDataView[0]["FINGERPRINT_ENGINE"].ToString()) + ")");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }
            else
            {
                if (DataSetClient.Tables["Company"].Rows[0]["COMPANY_DESC"].ToString() == pvtCompanyDataView[0]["COMPANY_DESC"].ToString()
                    && DataSetClient.Tables["Company"].Rows[0]["POST_ADDR_LINE1"].ToString() == pvtCompanyDataView[0]["POST_ADDR_LINE1"].ToString()
                    && DataSetClient.Tables["Company"].Rows[0]["POST_ADDR_LINE2"].ToString() == pvtCompanyDataView[0]["POST_ADDR_LINE2"].ToString()
                    && DataSetClient.Tables["Company"].Rows[0]["POST_ADDR_LINE3"].ToString() == pvtCompanyDataView[0]["POST_ADDR_LINE3"].ToString()
                    && DataSetClient.Tables["Company"].Rows[0]["POST_ADDR_CODE"].ToString() == pvtCompanyDataView[0]["POST_ADDR_CODE"].ToString()
                    && DataSetClient.Tables["Company"].Rows[0]["RES_UNIT_NUMBER"].ToString() == pvtCompanyDataView[0]["RES_UNIT_NUMBER"].ToString()
                    && DataSetClient.Tables["Company"].Rows[0]["RES_COMPLEX"].ToString() == pvtCompanyDataView[0]["RES_COMPLEX"].ToString()
                    && DataSetClient.Tables["Company"].Rows[0]["RES_STREET_NUMBER"].ToString() == pvtCompanyDataView[0]["RES_STREET_NUMBER"].ToString()
                    && DataSetClient.Tables["Company"].Rows[0]["RES_STREET_NAME"].ToString() == pvtCompanyDataView[0]["RES_STREET_NAME"].ToString()
                    && DataSetClient.Tables["Company"].Rows[0]["RES_SUBURB"].ToString() == pvtCompanyDataView[0]["RES_SUBURB"].ToString()
                    && DataSetClient.Tables["Company"].Rows[0]["RES_CITY"].ToString() == pvtCompanyDataView[0]["RES_CITY"].ToString()
                    && DataSetClient.Tables["Company"].Rows[0]["RES_ADDR_CODE"].ToString() == pvtCompanyDataView[0]["RES_ADDR_CODE"].ToString()
                     && DataSetClient.Tables["Company"].Rows[0]["FINGERPRINT_ENGINE"].ToString() == pvtCompanyDataView[0]["FINGERPRINT_ENGINE"].ToString())
                {
                }
                else
                {
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.COMPANY");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" COMPANY_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(pvtCompanyDataView[0]["COMPANY_DESC"].ToString()));
                    strQry.AppendLine(",POST_ADDR_LINE1 = " + clsDBConnectionObjects.Text2DynamicSQL(pvtCompanyDataView[0]["POST_ADDR_LINE1"].ToString()));
                    strQry.AppendLine(",POST_ADDR_LINE2 = " + clsDBConnectionObjects.Text2DynamicSQL(pvtCompanyDataView[0]["POST_ADDR_LINE2"].ToString()));
                    strQry.AppendLine(",POST_ADDR_LINE3 = " + clsDBConnectionObjects.Text2DynamicSQL(pvtCompanyDataView[0]["POST_ADDR_LINE3"].ToString()));
                    strQry.AppendLine(",POST_ADDR_CODE = " + clsDBConnectionObjects.Text2DynamicSQL(pvtCompanyDataView[0]["POST_ADDR_CODE"].ToString()));
                    strQry.AppendLine(",RES_UNIT_NUMBER = " + clsDBConnectionObjects.Text2DynamicSQL(pvtCompanyDataView[0]["RES_UNIT_NUMBER"].ToString()));
                    strQry.AppendLine(",RES_COMPLEX = " + clsDBConnectionObjects.Text2DynamicSQL(pvtCompanyDataView[0]["RES_COMPLEX"].ToString()));
                    strQry.AppendLine(",RES_STREET_NUMBER = " + clsDBConnectionObjects.Text2DynamicSQL(pvtCompanyDataView[0]["RES_STREET_NUMBER"].ToString()));
                    strQry.AppendLine(",RES_STREET_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(pvtCompanyDataView[0]["RES_STREET_NAME"].ToString()));
                    strQry.AppendLine(",RES_SUBURB = " + clsDBConnectionObjects.Text2DynamicSQL(pvtCompanyDataView[0]["RES_SUBURB"].ToString()));
                    strQry.AppendLine(",RES_CITY = " + clsDBConnectionObjects.Text2DynamicSQL(pvtCompanyDataView[0]["RES_CITY"].ToString()));
                    strQry.AppendLine(",RES_ADDR_CODE = " + clsDBConnectionObjects.Text2DynamicSQL(pvtCompanyDataView[0]["RES_ADDR_CODE"].ToString()));
                    strQry.AppendLine(",FINGERPRINT_ENGINE = " + clsDBConnectionObjects.Text2DynamicSQL(pvtCompanyDataView[0]["FINGERPRINT_ENGINE"].ToString()));

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }
            }

            //Read From Client Database
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EMPLOYEE_NO");
            strQry.AppendLine(",EMPLOYEE_CODE");
            strQry.AppendLine(",EMPLOYEE_NAME");
            strQry.AppendLine(",EMPLOYEE_SURNAME");
            strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE");
            strQry.AppendLine(",ISNULL(DEPARTMENT_NO,0) AS DEPARTMENT_NO");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            //strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + this.flxgPayrollType[this.flxgPayrollType.Row,1].ToString().Substring(0,1) + "'");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" EMPLOYEE_NO");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSetClient, "Employee");

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EMPLOYEE_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            //strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + this.flxgPayrollType[this.flxgPayrollType.Row,1].ToString().Substring(0,1) + "'");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSetClient, "EmployeePayCategory");

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PAY_CATEGORY_BREAK_NO");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY_BREAK");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            //strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + this.flxgPayrollType[this.flxgPayrollType.Row, 1].ToString().Substring(0, 1) + "'");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSetClient, "PayCategoryBreak");

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",NO_EDIT_IND");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            //strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + this.flxgPayrollType[this.flxgPayrollType.Row,1].ToString().Substring(0,1) + "'");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSetClient, "PayCategory");

            DataView pvtEmployeePayCategoryClientDataView = new DataView(DataSetClient.Tables["EmployeePayCategory"],
                "",
                "EMPLOYEE_NO,PAY_CATEGORY_NO,PAY_CATEGORY_TYPE",
                DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < pvtEmployeePayCategoryDataView.Count; intRow++)
            {
                objFind[0] = pvtEmployeePayCategoryDataView[intRow]["EMPLOYEE_NO"].ToString();
                objFind[1] = pvtEmployeePayCategoryDataView[intRow]["PAY_CATEGORY_NO"].ToString();
                objFind[2] = pvtEmployeePayCategoryDataView[intRow]["PAY_CATEGORY_TYPE"].ToString();

                intFindRow = pvtEmployeePayCategoryClientDataView.Find(objFind);

                if (intFindRow == -1)
                {
                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",KEEP_IND)");

                    strQry.AppendLine(" VALUES ");

                    strQry.AppendLine("(" + Convert.ToInt32(pvtEmployeePayCategoryDataView[intRow]["COMPANY_NO"]));
                    strQry.AppendLine("," + Convert.ToInt32(pvtEmployeePayCategoryDataView[intRow]["EMPLOYEE_NO"]));
                    strQry.AppendLine("," + Convert.ToInt32(pvtEmployeePayCategoryDataView[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtEmployeePayCategoryDataView[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine(",'Y')");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }
                else
                {
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY");
                    strQry.AppendLine(" SET KEEP_IND = 'Y'");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(pvtEmployeePayCategoryDataView[intRow]["COMPANY_NO"]));
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + Convert.ToInt32(pvtEmployeePayCategoryDataView[intRow]["EMPLOYEE_NO"]));
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToInt32(pvtEmployeePayCategoryDataView[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(pvtEmployeePayCategoryDataView[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }
            }

            strQry.Clear();

            strQry.AppendLine(" DELETE EPC FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");

            strQry.AppendLine(" WHERE EPC.COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.Append(strPayCategoryFilterTimeAttendance.Replace("AND ", "AND EPC."));
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'T'");
            strQry.AppendLine(" AND EPC.KEEP_IND = 'N'");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();

            strQry.AppendLine(" DELETE EPC FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");

            strQry.AppendLine(" WHERE EPC.COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.Append(strPayCategoryFilterWages.Replace("AND ", "AND EPC."));
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" AND EPC.KEEP_IND = 'N'");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();

            strQry.AppendLine(" DELETE EPC FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");

            strQry.AppendLine(" WHERE EPC.COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.Append(strPayCategoryFilterSalaries.Replace("AND ", "AND EPC."));
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'S'");
            strQry.AppendLine(" AND EPC.KEEP_IND = 'N'");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            DataView pvtEmployeeClientDataView = new DataView(DataSetClient.Tables["Employee"],
                "",
                "EMPLOYEE_NO",
                DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
            {
                if (pvtEmployeeDataView[intRow]["EMPLOYEE_ENDDATE"] != System.DBNull.Value)
                {
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.EMPLOYEE");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" NOT_ACTIVE_IND = 'Y'");
                    strQry.AppendLine(",EMPLOYEE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString()));
                    strQry.AppendLine(",EMPLOYEE_SURNAME = " + clsDBConnectionObjects.Text2DynamicSQL(pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString()));
                    strQry.AppendLine(",DEPARTMENT_NO = " + pvtEmployeeDataView[intRow]["DEPARTMENT_NO"].ToString());
                    //2017-09-29
                    strQry.AppendLine(",USE_EMPLOYEE_NO_IND = " + clsDBConnectionObjects.Text2DynamicSQL(pvtEmployeeDataView[intRow]["USE_EMPLOYEE_NO_IND"].ToString()));
                    strQry.AppendLine(",EMPLOYEE_PIN = " + clsDBConnectionObjects.Text2DynamicSQL(pvtEmployeeDataView[intRow]["EMPLOYEE_PIN"].ToString()));
                    strQry.AppendLine(",EMPLOYEE_RFID_CARD_NO = " + clsDBConnectionObjects.Text2DynamicSQL(pvtEmployeeDataView[intRow]["EMPLOYEE_RFID_CARD_NO"].ToString()));

                    strQry.AppendLine(",KEEP_IND = 'Y'");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(pvtEmployeeDataView[intRow]["COMPANY_NO"]));
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(pvtEmployeeDataView[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + Convert.ToInt32(pvtEmployeeDataView[intRow]["EMPLOYEE_NO"]));

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }
                else
                {
                    intFindRow = pvtEmployeeClientDataView.Find(pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString());

                    if (intFindRow == -1)
                    {
                        //Not Found
                        strQry.Clear();

                        strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE");
                        strQry.AppendLine("(COMPANY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",EMPLOYEE_CODE");
                        strQry.AppendLine(",EMPLOYEE_NAME");
                        strQry.AppendLine(",EMPLOYEE_SURNAME");
                        strQry.AppendLine(",DEPARTMENT_NO");
                        strQry.AppendLine(",KEEP_IND");

                        strQry.AppendLine(",USE_EMPLOYEE_NO_IND");
                        strQry.AppendLine(",EMPLOYEE_PIN");
                        strQry.AppendLine(",EMPLOYEE_RFID_CARD_NO");
                        
                        strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE)");

                        strQry.AppendLine(" VALUES ");

                        strQry.AppendLine("(" + Convert.ToInt32(pvtEmployeeDataView[intRow]["COMPANY_NO"]));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtEmployeeDataView[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                        strQry.AppendLine("," + Convert.ToInt32(pvtEmployeeDataView[intRow]["EMPLOYEE_NO"]));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString()));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString()));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString()));
                        strQry.AppendLine("," + pvtEmployeeDataView[intRow]["DEPARTMENT_NO"].ToString());
                        strQry.AppendLine(",'Y'");
                        
                        //2017-09-29
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtEmployeeDataView[intRow]["USE_EMPLOYEE_NO_IND"].ToString()));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtEmployeeDataView[intRow]["EMPLOYEE_PIN"].ToString()));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtEmployeeDataView[intRow]["EMPLOYEE_RFID_CARD_NO"].ToString()));
                        
                        if (pvtEmployeeDataView[intRow]["EMPLOYEE_LAST_RUNDATE"] == System.DBNull.Value)
                        {
                            strQry.AppendLine(",NULL)");
                        }
                        else
                        {
                            strQry.AppendLine(",'" + Convert.ToDateTime(pvtEmployeeDataView[intRow]["EMPLOYEE_LAST_RUNDATE"]).ToString("yyyy-MM-dd") + "')");
                        }
                    }
                    else
                    {
                        //Details Have Changed
                        strQry.Clear();

                        strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.EMPLOYEE");
                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" EMPLOYEE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString()));
                        strQry.AppendLine(",EMPLOYEE_SURNAME = " + clsDBConnectionObjects.Text2DynamicSQL(pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString()));
                        strQry.AppendLine(",DEPARTMENT_NO = " + pvtEmployeeDataView[intRow]["DEPARTMENT_NO"].ToString());
                        //2017-09-29
                        strQry.AppendLine(",USE_EMPLOYEE_NO_IND = " + clsDBConnectionObjects.Text2DynamicSQL(pvtEmployeeDataView[intRow]["USE_EMPLOYEE_NO_IND"].ToString()));
                        strQry.AppendLine(",EMPLOYEE_PIN = " + clsDBConnectionObjects.Text2DynamicSQL(pvtEmployeeDataView[intRow]["EMPLOYEE_PIN"].ToString()));
                        strQry.AppendLine(",EMPLOYEE_RFID_CARD_NO = " + clsDBConnectionObjects.Text2DynamicSQL(pvtEmployeeDataView[intRow]["EMPLOYEE_RFID_CARD_NO"].ToString()));

                        strQry.AppendLine(",KEEP_IND = 'Y'");

                        if (pvtEmployeeDataView[intRow]["EMPLOYEE_LAST_RUNDATE"] == System.DBNull.Value)
                        {
                            strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = '" + Convert.ToDateTime(pvtEmployeeDataView[intRow]["EMPLOYEE_LAST_RUNDATE"]).ToString("yyyy-MM-dd") + "'");
                        }

                        strQry.AppendLine(" WHERE COMPANY_NO = " + pvtEmployeeDataView[intRow]["COMPANY_NO"].ToString());
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(pvtEmployeeDataView[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                    }
                }

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            //Wages
            if (strCostCentresWages != "")
            {
                strQry.Clear();

                strQry.AppendLine(" DELETE E FROM InteractPayrollClient.dbo.EMPLOYEE E");

                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");

                strQry.Append(strPayCategoryFilterWages.Replace("PAY_CATEGORY_NO", "EPC.PAY_CATEGORY_NO"));

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo.ToString());
                strQry.AppendLine(" AND  E.PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND E.KEEP_IND = 'N'");

                //No EMPLOYEE_PAY_CATEGORY for EMPLOYEE
                strQry.AppendLine(" AND EPC.COMPANY_NO IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            if (strCostCentresSalaries != "")
            {
                //Salaries
                strQry.Clear();

                strQry.AppendLine(" DELETE E FROM InteractPayrollClient.dbo.EMPLOYEE E");

                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");

                strQry.Append(strPayCategoryFilterSalaries.Replace("PAY_CATEGORY_NO", "EPC.PAY_CATEGORY_NO"));

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo.ToString());
                strQry.AppendLine(" AND  E.PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND E.KEEP_IND = 'N'");

                //No EMPLOYEE_PAY_CATEGORY for EMPLOYEE
                strQry.AppendLine(" AND EPC.COMPANY_NO IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            if (strCostCentresTimeAttendance != "")
            {
                //Time Attendance
                strQry.Clear();

                strQry.AppendLine(" DELETE E FROM InteractPayrollClient.dbo.EMPLOYEE E");

                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");

                strQry.Append(strPayCategoryFilterTimeAttendance.Replace("PAY_CATEGORY_NO", "EPC.PAY_CATEGORY_NO"));

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo.ToString());
                strQry.AppendLine(" AND  E.PAY_CATEGORY_TYPE = 'T'");
                strQry.AppendLine(" AND E.KEEP_IND = 'N'");

                //No EMPLOYEE_PAY_CATEGORY for EMPLOYEE
                strQry.AppendLine(" AND EPC.COMPANY_NO IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            DataView pvtPayCategoryClientDataView = new DataView(DataSetClient.Tables["PayCategory"],
                "",
                "PAY_CATEGORY_NO",
                DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < pvtPayCategoryDataView.Count; intRow++)
            {
                intFindRow = pvtPayCategoryClientDataView.Find(pvtPayCategoryDataView[intRow]["PAY_CATEGORY_NO"].ToString());

                if (intFindRow == -1)
                {
                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.PAY_CATEGORY");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",PAY_CATEGORY_DESC");

                    strQry.AppendLine(",DAILY_ROUNDING_IND");
                    strQry.AppendLine(",DAILY_ROUNDING_MINUTES");
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
                    strQry.AppendLine(",NO_EDIT_IND)");

                    strQry.AppendLine(" VALUES ");

                    strQry.AppendLine("(" + pvtPayCategoryDataView[intRow]["COMPANY_NO"].ToString());
                    strQry.AppendLine("," + pvtPayCategoryDataView[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtPayCategoryDataView[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtPayCategoryDataView[intRow]["PAY_CATEGORY_DESC"].ToString()));

                    strQry.AppendLine("," + pvtPayCategoryDataView[intRow]["DAILY_ROUNDING_IND"].ToString());
                    strQry.AppendLine("," + pvtPayCategoryDataView[intRow]["DAILY_ROUNDING_MINUTES"].ToString());
                    strQry.AppendLine("," + pvtPayCategoryDataView[intRow]["EXCEPTION_SUN_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine("," + pvtPayCategoryDataView[intRow]["EXCEPTION_SUN_BELOW_MINUTES"].ToString());
                    strQry.AppendLine("," + pvtPayCategoryDataView[intRow]["EXCEPTION_MON_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine("," + pvtPayCategoryDataView[intRow]["EXCEPTION_MON_BELOW_MINUTES"].ToString());
                    strQry.AppendLine("," + pvtPayCategoryDataView[intRow]["EXCEPTION_TUE_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine("," + pvtPayCategoryDataView[intRow]["EXCEPTION_TUE_BELOW_MINUTES"].ToString());
                    strQry.AppendLine("," + pvtPayCategoryDataView[intRow]["EXCEPTION_WED_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine("," + pvtPayCategoryDataView[intRow]["EXCEPTION_WED_BELOW_MINUTES"].ToString());
                    strQry.AppendLine("," + pvtPayCategoryDataView[intRow]["EXCEPTION_THU_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine("," + pvtPayCategoryDataView[intRow]["EXCEPTION_THU_BELOW_MINUTES"].ToString());
                    strQry.AppendLine("," + pvtPayCategoryDataView[intRow]["EXCEPTION_FRI_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine("," + pvtPayCategoryDataView[intRow]["EXCEPTION_FRI_BELOW_MINUTES"].ToString());
                    strQry.AppendLine("," + pvtPayCategoryDataView[intRow]["EXCEPTION_SAT_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine("," + pvtPayCategoryDataView[intRow]["EXCEPTION_SAT_BELOW_MINUTES"].ToString());

                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtPayCategoryDataView[intRow]["NO_EDIT_IND"].ToString()) + ")");
                }
                else
                {
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.PAY_CATEGORY");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" PAY_CATEGORY_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(pvtPayCategoryDataView[intRow]["PAY_CATEGORY_DESC"].ToString()));

                    strQry.AppendLine(",DAILY_ROUNDING_IND = " + pvtPayCategoryDataView[intRow]["DAILY_ROUNDING_IND"].ToString());
                    strQry.AppendLine(",DAILY_ROUNDING_MINUTES = " + pvtPayCategoryDataView[intRow]["DAILY_ROUNDING_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_SUN_ABOVE_MINUTES = " + pvtPayCategoryDataView[intRow]["EXCEPTION_SUN_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_SUN_BELOW_MINUTES = " + pvtPayCategoryDataView[intRow]["EXCEPTION_SUN_BELOW_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_MON_ABOVE_MINUTES = " + pvtPayCategoryDataView[intRow]["EXCEPTION_MON_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_MON_BELOW_MINUTES = " + pvtPayCategoryDataView[intRow]["EXCEPTION_MON_BELOW_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_TUE_ABOVE_MINUTES = " + pvtPayCategoryDataView[intRow]["EXCEPTION_TUE_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_TUE_BELOW_MINUTES = " + pvtPayCategoryDataView[intRow]["EXCEPTION_TUE_BELOW_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_WED_ABOVE_MINUTES = " + pvtPayCategoryDataView[intRow]["EXCEPTION_WED_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_WED_BELOW_MINUTES = " + pvtPayCategoryDataView[intRow]["EXCEPTION_WED_BELOW_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_THU_ABOVE_MINUTES = " + pvtPayCategoryDataView[intRow]["EXCEPTION_THU_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_THU_BELOW_MINUTES = " + pvtPayCategoryDataView[intRow]["EXCEPTION_THU_BELOW_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_FRI_ABOVE_MINUTES = " + pvtPayCategoryDataView[intRow]["EXCEPTION_FRI_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_FRI_BELOW_MINUTES = " + pvtPayCategoryDataView[intRow]["EXCEPTION_FRI_BELOW_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_SAT_ABOVE_MINUTES = " + pvtPayCategoryDataView[intRow]["EXCEPTION_SAT_ABOVE_MINUTES"].ToString());
                    strQry.AppendLine(",EXCEPTION_SAT_BELOW_MINUTES = " + pvtPayCategoryDataView[intRow]["EXCEPTION_SAT_BELOW_MINUTES"].ToString());

                    strQry.AppendLine(",NO_EDIT_IND = " + clsDBConnectionObjects.Text2DynamicSQL(pvtPayCategoryDataView[intRow]["NO_EDIT_IND"].ToString()));

                    strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(pvtPayCategoryDataView[intRow]["COMPANY_NO"]));
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToInt32(pvtPayCategoryDataView[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(pvtPayCategoryDataView[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                }

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            DataView pvtPayCategoryBreakClientDataView = new DataView(DataSetClient.Tables["PayCategoryBreak"],
                "",
                " PAY_CATEGORY_NO,PAY_CATEGORY_BREAK_NO,PAY_CATEGORY_TYPE",
                DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < pvtPayCategoryBreakDataView.Count; intRow++)
            {
                objFind[0] = pvtPayCategoryBreakDataView[intRow]["PAY_CATEGORY_NO"].ToString();
                objFind[1] = pvtPayCategoryBreakDataView[intRow]["PAY_CATEGORY_BREAK_NO"].ToString();
                objFind[2] = pvtPayCategoryBreakDataView[intRow]["PAY_CATEGORY_TYPE"].ToString();

                intFindRow = pvtPayCategoryBreakClientDataView.Find(objFind);

                if (intFindRow == -1)
                {
                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.PAY_CATEGORY_BREAK");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",PAY_CATEGORY_BREAK_NO");
                    strQry.AppendLine(",WORKED_TIME_MINUTES");
                    strQry.AppendLine(",BREAK_MINUTES");
                    strQry.AppendLine(",KEEP_IND)");

                    strQry.AppendLine(" VALUES ");

                    strQry.AppendLine("(" + pvtPayCategoryBreakDataView[intRow]["COMPANY_NO"].ToString());
                    strQry.AppendLine("," + pvtPayCategoryBreakDataView[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(pvtPayCategoryBreakDataView[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine("," + pvtPayCategoryBreakDataView[intRow]["PAY_CATEGORY_BREAK_NO"].ToString());
                    strQry.AppendLine("," + pvtPayCategoryBreakDataView[intRow]["WORKED_TIME_MINUTES"].ToString());
                    strQry.AppendLine("," + pvtPayCategoryBreakDataView[intRow]["BREAK_MINUTES"].ToString());

                    strQry.AppendLine(",'Y')");
                }
                else
                {
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.PAY_CATEGORY_BREAK");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" WORKED_TIME_MINUTES = " + pvtPayCategoryBreakDataView[intRow]["WORKED_TIME_MINUTES"].ToString());
                    strQry.AppendLine(",BREAK_MINUTES = " + pvtPayCategoryBreakDataView[intRow]["BREAK_MINUTES"].ToString());
                    strQry.AppendLine(",KEEP_IND = 'Y'");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(pvtPayCategoryBreakDataView[intRow]["COMPANY_NO"]));
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToInt32(pvtPayCategoryBreakDataView[intRow]["PAY_CATEGORY_NO"]));
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(pvtPayCategoryBreakDataView[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine(" AND PAY_CATEGORY_BREAK_NO = " + Convert.ToInt32(pvtPayCategoryBreakDataView[intRow]["PAY_CATEGORY_BREAK_NO"]));
                }

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            if (strCostCentresWages != "")
            {
                strQry.Clear();

                strQry.AppendLine(" DELETE PC FROM InteractPayrollClient.dbo.PAY_CATEGORY PC");

                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");

                //Stops Delete of Current PAY_CATEGORY Download When there are No EMPLOYEE_PAY_CATEGORY Linked
                strQry.Append(strPayCategoryFilterWages.Replace("AND ", "WHERE PC.").Replace("IN", "NOT IN"));

                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = 'W'");

                strQry.AppendLine(" AND EPC.COMPANY_NO IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            if (strCostCentresSalaries != "")
            {
                strQry.Clear();

                strQry.AppendLine(" DELETE PC FROM InteractPayrollClient.dbo.PAY_CATEGORY PC");

                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");

                //Stops Delete of Current PAY_CATEGORY Download When there are No EMPLOYEE_PAY_CATEGORY Linked
                strQry.Append(strPayCategoryFilterSalaries.Replace("AND ", "WHERE PC.").Replace("IN", "NOT IN"));

                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = 'S'");

                strQry.AppendLine(" AND EPC.COMPANY_NO IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            if (strCostCentresTimeAttendance != "")
            {
                strQry.Clear();

                strQry.AppendLine(" DELETE PC FROM InteractPayrollClient.dbo.PAY_CATEGORY PC");

                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");

                //Stops Delete of Current PAY_CATEGORY Download When there are No EMPLOYEE_PAY_CATEGORY Linked
                strQry.Append(strPayCategoryFilterTimeAttendance.Replace("AND ", "WHERE PC.").Replace("IN", "NOT IN"));

                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = 'T'");

                strQry.AppendLine(" AND EPC.COMPANY_NO IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            strQry.Clear();

            strQry.AppendLine(" DELETE D FROM InteractPayrollClient.dbo.DEPARTMENT D");

            strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON D.COMPANY_NO = E.COMPANY_NO");
            strQry.AppendLine(" AND D.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND D.DEPARTMENT_NO = E.DEPARTMENT_NO");

            strQry.AppendLine(" WHERE E.COMPANY_NO IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();

            strQry.AppendLine(" DELETE E FROM InteractPayrollClient.dbo.EMPLOYEE E");

            strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" WHERE EPC.COMPANY_NO IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();

            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.PAY_CATEGORY_BREAK ");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
            strQry.Append(strPayCategoryFilterWages);
            strQry.AppendLine(" AND KEEP_IND = 'N'");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();

            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.PAY_CATEGORY_BREAK ");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
            strQry.Append(strPayCategoryFilterSalaries);
            strQry.AppendLine(" AND KEEP_IND = 'N'");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();

            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.PAY_CATEGORY_BREAK ");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");
            strQry.Append(strPayCategoryFilterTimeAttendance);
            strQry.AppendLine(" AND KEEP_IND = 'N'");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            //20170330
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EMPLOYEE_NO");
            strQry.AppendLine(",FINGER_NO");
            strQry.AppendLine(",CREATION_DATETIME");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE_DELETE ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSetUpload, "EmployeeFingerPrintTemplateDelete");

            //20170321
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EMPLOYEE_NO");
            strQry.AppendLine(",FINGER_NO");
            strQry.AppendLine(",FINGER_TEMPLATE");
            strQry.AppendLine(",CREATION_DATETIME");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" EMPLOYEE_NO");
            strQry.AppendLine(",FINGER_NO");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSetClient, "EmployeeFingerTemplate");

            //20170321
            DataView pvtEmployeeFingerTemplateClientDataView = new DataView(DataSetClient.Tables["EmployeeFingerTemplate"],
                "",
                "EMPLOYEE_NO,FINGER_NO",
                DataViewRowState.CurrentRows);

            if (pvtEmployeeFingerTemplateDataView.Count > 0)
            {
                for (int intRow = 0; intRow < pvtEmployeeFingerTemplateDataView.Count; intRow++)
                {
                    objFind2[0] = pvtEmployeeFingerTemplateDataView[intRow]["EMPLOYEE_NO"].ToString();
                    objFind2[1] = pvtEmployeeFingerTemplateDataView[intRow]["FINGER_NO"].ToString();

                    intFindRow = pvtEmployeeFingerTemplateClientDataView.Find(objFind2);

                    if (intFindRow == -1)
                    {
                        //Not Found
                        DataView EmployeeFingerPrintTemplateDeleteClientDataView = new DataView(DataSetUpload.Tables["EmployeeFingerPrintTemplateDelete"],
                        "EMPLOYEE_NO = " + pvtEmployeeFingerTemplateDataView[intRow]["EMPLOYEE_NO"].ToString() + " AND FINGER_NO = " + pvtEmployeeFingerTemplateDataView[intRow]["FINGER_NO"].ToString() + " AND CREATION_DATETIME = '" + Convert.ToDateTime(pvtEmployeeFingerTemplateDataView[intRow]["CREATION_DATETIME"]).ToString("yyyy-MM-dd HH:mm:ss") + "'",
                        "EMPLOYEE_NO,FINGER_NO",
                        DataViewRowState.CurrentRows);

                        if (EmployeeFingerPrintTemplateDeleteClientDataView.Count == 0)
                        {
                            //Not Being Deleted
                            strQry.Clear();

                            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");
                            strQry.AppendLine("(COMPANY_NO");
                            strQry.AppendLine(",EMPLOYEE_NO");
                            strQry.AppendLine(",FINGER_NO ");
                            strQry.AppendLine(",FINGER_TEMPLATE ");
                            strQry.AppendLine(",CREATION_DATETIME ");
                            strQry.AppendLine(",KEEP_IND) ");

                            strQry.AppendLine(" VALUES ");

                            strQry.AppendLine("(" + parint64CompanyNo.ToString());
                            strQry.AppendLine("," + pvtEmployeeFingerTemplateDataView[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine("," + pvtEmployeeFingerTemplateDataView[intRow]["FINGER_NO"].ToString());
                            strQry.AppendLine(",@FINGER_TEMPLATE");
                            strQry.AppendLine(",'" + Convert.ToDateTime(pvtEmployeeFingerTemplateDataView[intRow]["CREATION_DATETIME"]).ToString("yyyy-MM-dd HH:mm:ss") + "'");
                            strQry.AppendLine(",'Y')");

                            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString(), (byte[])pvtEmployeeFingerTemplateDataView[intRow]["FINGER_TEMPLATE"], "@FINGER_TEMPLATE");
                        }
                    }
                    else
                    {
                        //Keep Record
                        strQry.Clear();

                        strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");
                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" KEEP_IND = 'Y'");
                        strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + pvtEmployeeFingerTemplateDataView[intRow]["EMPLOYEE_NO"].ToString());
                        strQry.AppendLine(" AND FINGER_NO = " + pvtEmployeeFingerTemplateDataView[intRow]["FINGER_NO"].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                        if (pvtEmployeeFingerTemplateClientDataView[intFindRow]["CREATION_DATETIME"] == System.DBNull.Value)
                        {
                            //Upload - New Record
                            DataRow myDataRow = DataSetUpload.Tables["EmployeeFingerPrintTemplateUpload"].NewRow();

                            myDataRow["EMPLOYEE_NO"] = pvtEmployeeFingerTemplateClientDataView[intRow]["EMPLOYEE_NO"].ToString();
                            myDataRow["FINGER_NO"] = pvtEmployeeFingerTemplateClientDataView[intRow]["FINGER_NO"].ToString();
                            myDataRow["FINGER_TEMPLATE"] = pvtEmployeeFingerTemplateClientDataView[intRow]["FINGER_TEMPLATE"];

                            DataSetUpload.Tables["EmployeeFingerPrintTemplateUpload"].Rows.Add(myDataRow);
                        }
                        else
                        {
                            //NB Internet Database is Master
                            if (Convert.ToDateTime(pvtEmployeeFingerTemplateClientDataView[intFindRow]["CREATION_DATETIME"]).ToString("yyyyMMddHHmmss") == Convert.ToDateTime(pvtEmployeeFingerTemplateDataView[intRow]["CREATION_DATETIME"]).ToString("yyyyMMddHHmmss"))
                            {
                                //Same
                            }
                            else
                            {
                                //Update Record
                                if (Convert.ToInt64(Convert.ToDateTime(pvtEmployeeFingerTemplateClientDataView[intFindRow]["CREATION_DATETIME"]).ToString("yyyyMMddHHmmss")) > Convert.ToInt64(Convert.ToDateTime(pvtEmployeeFingerTemplateDataView[intRow]["CREATION_DATETIME"]).ToString("yyyyMMddHHmmss")))
                                {
                                    //Upload
                                    DataRow myDataRow = DataSetUpload.Tables["EmployeeFingerPrintTemplateUpload"].NewRow();

                                    myDataRow["EMPLOYEE_NO"] = pvtEmployeeFingerTemplateClientDataView[intRow]["EMPLOYEE_NO"].ToString();
                                    myDataRow["FINGER_NO"] = pvtEmployeeFingerTemplateClientDataView[intRow]["FINGER_NO"].ToString();
                                    myDataRow["FINGER_TEMPLATE"] = pvtEmployeeFingerTemplateClientDataView[intRow]["FINGER_TEMPLATE"];
                                    myDataRow["CREATION_DATETIME"] = pvtEmployeeFingerTemplateClientDataView[intRow]["CREATION_DATETIME"];

                                    DataSetUpload.Tables["EmployeeFingerPrintTemplateUpload"].Rows.Add(myDataRow);
                                }
                                else
                                {
                                    strQry.Clear();

                                    strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");
                                    strQry.AppendLine(" SET ");
                                    strQry.AppendLine(" CREATION_DATETIME = '" + Convert.ToDateTime(pvtEmployeeFingerTemplateDataView[intRow]["CREATION_DATETIME"]).ToString("yyyy-MM-dd HH:mm:ss") + "'");
                                    strQry.AppendLine(",FINGER_TEMPLATE = @FINGER_TEMPLATE");
                                    strQry.AppendLine(",KEEP_IND = 'Y'");
                                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                                    strQry.AppendLine(" AND EMPLOYEE_NO = " + pvtEmployeeFingerTemplateDataView[intRow]["EMPLOYEE_NO"].ToString());
                                    strQry.AppendLine(" AND FINGER_NO = " + pvtEmployeeFingerTemplateDataView[intRow]["FINGER_NO"].ToString());

                                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString(), (byte[])pvtEmployeeFingerTemplateDataView[intRow]["FINGER_TEMPLATE"], "@FINGER_TEMPLATE");
                                }
                            }
                        }
                    }
                }

                //New Templates on Client Machine
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EMPLOYEE_NO");
                strQry.AppendLine(",FINGER_NO");
                strQry.AppendLine(",FINGER_TEMPLATE");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
                strQry.AppendLine(" AND KEEP_IND = 'N'");
                strQry.AppendLine(" AND CREATION_DATETIME IS NULL");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSetClient, "EmployeeFingerTemplateNew");

                for (int intRow = 0; intRow < DataSetClient.Tables["EmployeeFingerTemplateNew"].Rows.Count; intRow++)
                {
                    DataRow myDataRow = DataSetUpload.Tables["EmployeeFingerPrintTemplateUpload"].NewRow();

                    myDataRow["EMPLOYEE_NO"] = DataSetClient.Tables["EmployeeFingerTemplateNew"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                    myDataRow["FINGER_NO"] = DataSetClient.Tables["EmployeeFingerTemplateNew"].Rows[intRow]["FINGER_NO"].ToString();
                    myDataRow["FINGER_TEMPLATE"] = DataSetClient.Tables["EmployeeFingerTemplateNew"].Rows[intRow]["FINGER_TEMPLATE"];

                    DataSetUpload.Tables["EmployeeFingerPrintTemplateUpload"].Rows.Add(myDataRow);
                }

                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" KEEP_IND = 'Y'");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND KEEP_IND = 'N'");
                strQry.AppendLine(" AND CREATION_DATETIME IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }
            else
            {
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" KEEP_IND = 'Y'");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                for (int intRow = 0; intRow < pvtEmployeeFingerTemplateClientDataView.Count; intRow++)
                {
                    DataRow myDataRow = DataSetUpload.Tables["EmployeeFingerPrintTemplateUpload"].NewRow();

                    myDataRow["EMPLOYEE_NO"] = pvtEmployeeFingerTemplateClientDataView[intRow]["EMPLOYEE_NO"].ToString();
                    myDataRow["FINGER_NO"] = pvtEmployeeFingerTemplateClientDataView[intRow]["FINGER_NO"].ToString();
                    myDataRow["FINGER_TEMPLATE"] = pvtEmployeeFingerTemplateClientDataView[intRow]["FINGER_TEMPLATE"];
                    myDataRow["CREATION_DATETIME"] = pvtEmployeeFingerTemplateClientDataView[intRow]["CREATION_DATETIME"];

                    DataSetUpload.Tables["EmployeeFingerPrintTemplateUpload"].Rows.Add(myDataRow);
                }
            }

            //20170321 - Cleanup Broken Links
            strQry.Clear();

            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND KEEP_IND = 'N'");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            
            //20170330
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" USER_NO");
            strQry.AppendLine(",FINGER_NO");
            strQry.AppendLine(",CREATION_DATETIME");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE_DELETE ");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSetUpload, "UserFingerPrintTemplateDelete");

            //2017-04-20
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" UFT.USER_NO");
            strQry.AppendLine(",UFT.FINGER_NO");
            strQry.AppendLine(",UFT.FINGER_TEMPLATE");
            strQry.AppendLine(",UFT.CREATION_DATETIME");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA ");
            
            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE UFT ");
            strQry.AppendLine(" ON UCA.USER_NO = UFT.USER_NO  ");

            strQry.AppendLine(" WHERE UCA.COMPANY_NO = " + parint64CompanyNo);
           
            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSetClient, "UserFingerTemplate");
            
            //20170321
            DataView pvtUserFingerTemplateClientDataView = new DataView(DataSetClient.Tables["UserFingerTemplate"],
                "",
                "USER_NO,FINGER_NO",
                DataViewRowState.CurrentRows);
            
            if (pvtUserFingerTemplateDataView.Count > 0)
            {
                for (int intRow = 0; intRow < pvtUserFingerTemplateDataView.Count; intRow++)
                {
                    objFind2[0] = pvtUserFingerTemplateDataView[intRow]["USER_NO"].ToString();
                    objFind2[1] = pvtUserFingerTemplateDataView[intRow]["FINGER_NO"].ToString();

                    intFindRow = pvtUserFingerTemplateClientDataView.Find(objFind2);

                    if (intFindRow == -1)
                    {
                        //Not Found
                        DataView UserFingerPrintTemplateDeleteClientDataView = new DataView(DataSetUpload.Tables["UserFingerPrintTemplateDelete"],
                        "USER_NO = " + pvtUserFingerTemplateDataView[intRow]["USER_NO"].ToString() + " AND FINGER_NO = " + pvtUserFingerTemplateDataView[intRow]["FINGER_NO"].ToString() + " AND CREATION_DATETIME = '" + Convert.ToDateTime(pvtUserFingerTemplateDataView[intRow]["CREATION_DATETIME"]).ToString("yyyy-MM-dd HH:mm:ss") + "'",
                        "USER_NO,FINGER_NO",
                        DataViewRowState.CurrentRows);

                        if (UserFingerPrintTemplateDeleteClientDataView.Count == 0)
                        {
                            //Not Being Deleted
                            strQry.Clear();

                            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE");
                            strQry.AppendLine("(USER_NO");
                            strQry.AppendLine(",FINGER_NO ");
                            strQry.AppendLine(",FINGER_TEMPLATE ");
                            strQry.AppendLine(",CREATION_DATETIME ");
                            strQry.AppendLine(",KEEP_IND) ");

                            strQry.AppendLine(" VALUES ");

                            strQry.AppendLine("(" + pvtUserFingerTemplateDataView[intRow]["USER_NO"].ToString());
                            strQry.AppendLine("," + pvtUserFingerTemplateDataView[intRow]["FINGER_NO"].ToString());
                            strQry.AppendLine(",@FINGER_TEMPLATE");
                            strQry.AppendLine(",'" + Convert.ToDateTime(pvtUserFingerTemplateDataView[intRow]["CREATION_DATETIME"]).ToString("yyyy-MM-dd HH:mm:ss") + "'");
                            strQry.AppendLine(",'Y')");

                            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString(), (byte[])pvtUserFingerTemplateDataView[intRow]["FINGER_TEMPLATE"], "@FINGER_TEMPLATE");
                        }
                    }
                    else
                    {
                        //Keep Record
                        strQry.Clear();

                        strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE");
                        strQry.AppendLine(" SET KEEP_IND = 'Y'");

                        strQry.AppendLine(" WHERE USER_NO = " + pvtUserFingerTemplateDataView[intRow]["USER_NO"].ToString());
                        strQry.AppendLine(" AND FINGER_NO = " + pvtUserFingerTemplateDataView[intRow]["FINGER_NO"].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                        if (pvtUserFingerTemplateClientDataView[intFindRow]["CREATION_DATETIME"] == System.DBNull.Value)
                        {
                            //2017-04-21 Tested

                            //Upload - New Record
                            DataRow myDataRow = DataSetUpload.Tables["UserFingerPrintTemplateUpload"].NewRow();

                            myDataRow["USER_NO"] = pvtUserFingerTemplateClientDataView[intRow]["USER_NO"].ToString();
                            myDataRow["FINGER_NO"] = pvtUserFingerTemplateClientDataView[intRow]["FINGER_NO"].ToString();
                            myDataRow["FINGER_TEMPLATE"] = pvtUserFingerTemplateClientDataView[intRow]["FINGER_TEMPLATE"];

                            DataSetUpload.Tables["UserFingerPrintTemplateUpload"].Rows.Add(myDataRow);
                        }
                        else
                        {
                            //NB Internet Database is Master
                            if (Convert.ToDateTime(pvtUserFingerTemplateClientDataView[intFindRow]["CREATION_DATETIME"]).ToString("yyyyMMddHHmmss") == Convert.ToDateTime(pvtUserFingerTemplateDataView[intRow]["CREATION_DATETIME"]).ToString("yyyyMMddHHmmss"))
                            {
                                //Same
                            }
                            else
                            {
                                //Update Record
                                if (Convert.ToInt64(Convert.ToDateTime(pvtUserFingerTemplateClientDataView[intFindRow]["CREATION_DATETIME"]).ToString("yyyyMMddHHmmss")) > Convert.ToInt64(Convert.ToDateTime(pvtUserFingerTemplateDataView[intRow]["CREATION_DATETIME"]).ToString("yyyyMMddHHmmss")))
                                {
                                    //2017-04-21 Tested

                                    //Upload
                                    DataRow myDataRow = DataSetUpload.Tables["UserFingerPrintTemplateUpload"].NewRow();

                                    myDataRow["USER_NO"] = pvtUserFingerTemplateClientDataView[intRow]["USER_NO"].ToString();
                                    myDataRow["FINGER_NO"] = pvtUserFingerTemplateClientDataView[intRow]["FINGER_NO"].ToString();
                                    myDataRow["FINGER_TEMPLATE"] = pvtUserFingerTemplateClientDataView[intRow]["FINGER_TEMPLATE"];
                                    myDataRow["CREATION_DATETIME"] = pvtUserFingerTemplateClientDataView[intRow]["CREATION_DATETIME"];

                                    DataSetUpload.Tables["UserFingerPrintTemplateUpload"].Rows.Add(myDataRow);
                                }
                                else
                                {
                                    //2017-04-21 Tested
                                    strQry.Clear();

                                    strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE");
                                    strQry.AppendLine(" SET ");
                                    strQry.AppendLine(" CREATION_DATETIME = '" + Convert.ToDateTime(pvtUserFingerTemplateDataView[intRow]["CREATION_DATETIME"]).ToString("yyyy-MM-dd HH:mm:ss") + "'");
                                    strQry.AppendLine(",FINGER_TEMPLATE = @FINGER_TEMPLATE");
                                    strQry.AppendLine(",KEEP_IND = 'Y'");
                                    strQry.AppendLine(" WHERE USER_NO = " + pvtUserFingerTemplateDataView[intRow]["USER_NO"].ToString());
                                    strQry.AppendLine(" AND FINGER_NO = " + pvtUserFingerTemplateDataView[intRow]["FINGER_NO"].ToString());

                                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString(), (byte[])pvtUserFingerTemplateDataView[intRow]["FINGER_TEMPLATE"], "@FINGER_TEMPLATE");
                                }
                            }
                        }
                    }
                }

                //New Templates on Client Machine
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" UFT.USER_NO");
                strQry.AppendLine(",UFT.FINGER_NO");
                strQry.AppendLine(",UFT.FINGER_TEMPLATE");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE UFT");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA");
                strQry.AppendLine(" ON UFT.USER_NO = UCA.USER_NO ");
                strQry.AppendLine(" AND UCA.COMPANY_NO = " + parint64CompanyNo);

                strQry.AppendLine(" WHERE UFT.KEEP_IND = 'N'");
                strQry.AppendLine(" AND UFT.CREATION_DATETIME IS NULL");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSetClient, "UserFingerTemplateNew");

                for (int intRow = 0; intRow < DataSetClient.Tables["UserFingerTemplateNew"].Rows.Count; intRow++)
                {
                    DataRow myDataRow = DataSetUpload.Tables["UserFingerPrintTemplateUpload"].NewRow();

                    myDataRow["USER_NO"] = DataSetClient.Tables["UserFingerTemplateNew"].Rows[intRow]["USER_NO"].ToString();
                    myDataRow["FINGER_NO"] = DataSetClient.Tables["UserFingerTemplateNew"].Rows[intRow]["FINGER_NO"].ToString();
                    myDataRow["FINGER_TEMPLATE"] = DataSetClient.Tables["UserFingerTemplateNew"].Rows[intRow]["FINGER_TEMPLATE"];

                    DataSetUpload.Tables["UserFingerPrintTemplateUpload"].Rows.Add(myDataRow);
                }

                strQry.Clear();

                strQry.AppendLine(" UPDATE UFT");
                strQry.AppendLine(" SET KEEP_IND = 'Y'");
                
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE UFT");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA");
                strQry.AppendLine(" ON UFT.USER_NO = UCA.USER_NO ");
                strQry.AppendLine(" AND UCA.COMPANY_NO = " + parint64CompanyNo);
                
                strQry.AppendLine(" WHERE UFT.KEEP_IND = 'N'");
                strQry.AppendLine(" AND UFT.CREATION_DATETIME IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }
            else
            {
                //2017-04-21 Tested
                strQry.Clear();

                strQry.AppendLine(" UPDATE UFT");

                strQry.AppendLine(" SET KEEP_IND = 'Y'");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE UFT");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA");
                strQry.AppendLine(" ON UFT.USER_NO = UCA.USER_NO ");
                strQry.AppendLine(" AND UCA.COMPANY_NO = " + parint64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                for (int intRow = 0; intRow < pvtUserFingerTemplateClientDataView.Count; intRow++)
                {
                    DataRow myDataRow = DataSetUpload.Tables["UserFingerPrintTemplateUpload"].NewRow();

                    myDataRow["USER_NO"] = pvtUserFingerTemplateClientDataView[intRow]["USER_NO"].ToString();
                    myDataRow["FINGER_NO"] = pvtUserFingerTemplateClientDataView[intRow]["FINGER_NO"].ToString();
                    myDataRow["FINGER_TEMPLATE"] = pvtUserFingerTemplateClientDataView[intRow]["FINGER_TEMPLATE"];
                    myDataRow["CREATION_DATETIME"] = pvtUserFingerTemplateClientDataView[intRow]["CREATION_DATETIME"];

                    DataSetUpload.Tables["UserFingerPrintTemplateUpload"].Rows.Add(myDataRow);
                }
            }
                        
            strQry.Clear();

            strQry.AppendLine(" DELETE UFT");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE UFT");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA");
            strQry.AppendLine(" ON UFT.USER_NO = UCA.USER_NO ");
            strQry.AppendLine(" AND UCA.COMPANY_NO = " + parint64CompanyNo);
                
            strQry.AppendLine(" WHERE UFT.KEEP_IND = 'N'");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            
            //2017-07-10
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" name ");

            strQry.AppendLine(" FROM sys.server_principals ");

            strQry.AppendLine(" WHERE name = 'Interact' ");

            this.clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSetClient, "UserInteractExists");

            if (DataSetClient.Tables["UserInteractExists"].Rows.Count == 0)
            {
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PAY_CATEGORY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY");

                //Force Empty DataTable
                strQry.AppendLine(" WHERE PAY_CATEGORY_NO = -1");
            }
            else
            {
                //2017-07-10
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PAY_CATEGORY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

                strQry.AppendLine(strPayCategoryFilter);
            }

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSetUpload, "PayCategoryInteractInd");
            
            //2017-05-01
            strQry.Clear();

            string strLastDownloadDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
            strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.PAY_CATEGORY ");

            strQry.AppendLine(" SET LAST_DOWNLOAD_DATETIME = '" + strLastDownloadDate + "'");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(strPayCategoryFilter);
            
            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSetUpload);
            DataSetUpload.Dispose();
            DataSetUpload = null;

            return bytCompress;
        }

        public void Maintain_Templates(Int64 parint64CompanyNo, byte[] parbyteDataSet)
        {
            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);
            
            for (int intRow = 0; intRow < DataSet.Tables["EmployeeFingerPrintTemplateDelete"].Rows.Count; intRow++)
            {
                strQry.Clear();

                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["EmployeeFingerPrintTemplateDelete"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                strQry.AppendLine(" AND FINGER_NO = " + DataSet.Tables["EmployeeFingerPrintTemplateDelete"].Rows[intRow]["FINGER_NO"].ToString());

                strQry.AppendLine(" AND CREATION_DATETIME = '" + Convert.ToDateTime(DataSet.Tables["EmployeeFingerPrintTemplateDelete"].Rows[intRow]["CREATION_DATETIME"]).ToString("yyyy-MM-dd HH:mm:ss") + "'");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();

                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE_DELETE");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["EmployeeFingerPrintTemplateDelete"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                strQry.AppendLine(" AND FINGER_NO = " + DataSet.Tables["EmployeeFingerPrintTemplateDelete"].Rows[intRow]["FINGER_NO"].ToString());

                strQry.AppendLine(" AND CREATION_DATETIME = '" + Convert.ToDateTime(DataSet.Tables["EmployeeFingerPrintTemplateDelete"].Rows[intRow]["CREATION_DATETIME"]).ToString("yyyy-MM-dd HH:mm:ss") + "'");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }
            
            for (int intRow = 0; intRow < DataSet.Tables["EmployeeFingerPrintTemplateUpload"].Rows.Count; intRow++)
            {
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE ");

                strQry.AppendLine(" SET CREATION_DATETIME = '" + Convert.ToDateTime(DataSet.Tables["EmployeeFingerPrintTemplateUpload"].Rows[intRow]["CREATION_DATETIME"]).ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["EmployeeFingerPrintTemplateUpload"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                strQry.AppendLine(" AND FINGER_NO = " + DataSet.Tables["EmployeeFingerPrintTemplateUpload"].Rows[intRow]["FINGER_NO"].ToString());

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            for (int intRow = 0; intRow < DataSet.Tables["UserFingerPrintTemplateDelete"].Rows.Count; intRow++)
            {
                //2017-04-21 Tested
                strQry.Clear();

                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE");
                strQry.AppendLine(" WHERE USER_NO = " + DataSet.Tables["UserFingerPrintTemplateDelete"].Rows[intRow]["USER_NO"].ToString());
                strQry.AppendLine(" AND FINGER_NO = " + DataSet.Tables["UserFingerPrintTemplateDelete"].Rows[intRow]["FINGER_NO"].ToString());

                strQry.AppendLine(" AND CREATION_DATETIME = '" + Convert.ToDateTime(DataSet.Tables["UserFingerPrintTemplateDelete"].Rows[intRow]["CREATION_DATETIME"]).ToString("yyyy-MM-dd HH:mm:ss") + "'");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();

                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE_DELETE");
                strQry.AppendLine(" WHERE USER_NO = " + DataSet.Tables["UserFingerPrintTemplateDelete"].Rows[intRow]["USER_NO"].ToString());
                strQry.AppendLine(" AND FINGER_NO = " + DataSet.Tables["UserFingerPrintTemplateDelete"].Rows[intRow]["FINGER_NO"].ToString());

                strQry.AppendLine(" AND CREATION_DATETIME = '" + Convert.ToDateTime(DataSet.Tables["UserFingerPrintTemplateDelete"].Rows[intRow]["CREATION_DATETIME"]).ToString("yyyy-MM-dd HH:mm:ss") + "'");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            for (int intRow = 0; intRow < DataSet.Tables["UserFingerPrintTemplateUpload"].Rows.Count; intRow++)
            {
                //2017-04-21 Tested
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.USER_FINGERPRINT_TEMPLATE ");

                strQry.AppendLine(" SET CREATION_DATETIME = '" + Convert.ToDateTime(DataSet.Tables["UserFingerPrintTemplateUpload"].Rows[intRow]["CREATION_DATETIME"]).ToString("yyyy-MM-dd HH:mm:ss") + "'");

                strQry.AppendLine(" WHERE USER_NO = " + DataSet.Tables["UserFingerPrintTemplateUpload"].Rows[intRow]["USER_NO"].ToString());
                strQry.AppendLine(" AND FINGER_NO = " + DataSet.Tables["UserFingerPrintTemplateUpload"].Rows[intRow]["FINGER_NO"].ToString());

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }
        }
    }
}
