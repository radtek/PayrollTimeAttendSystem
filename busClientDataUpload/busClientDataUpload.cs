using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using InteractPayroll;

namespace InteractPayrollClient
{
    public class busClientDataUpload
    {
        clsDBConnectionObjects clsDBConnectionObjects;
      
        public busClientDataUpload()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_PayCategory_Records(Int64 parint64CompanyNo, string parstrPayrollType)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" EPC.PAY_CATEGORY_NO");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E ");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON E.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE ");
            
            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PayCategory");

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_New_PayCategory_Records()
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" EPC.COMPANY_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E ");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON E.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE ");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PayCategory");

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_New_Company_Records(Int64 parint64CompanyNo)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" EPC.COMPANY_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E ");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON E.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo.ToString());

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PayCategory");

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_New_Company_Records_For_User(int parintCurrentUserNo, string parstrAccessInd)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" EPC.COMPANY_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E ");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON E.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE ");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS UCA");
                strQry.AppendLine(" ON UCA.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND E.COMPANY_NO = UCA.COMPANY_NO");
            }
            
            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PayCategory");

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public void Update_PayCategory_Last_Upload_DateTime(Int64 parint64CompanyNo,string strUploadDateTime, byte[] parbyteDataSet)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            DataSet ClientDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            for (int intPayCategoryRow = 0; intPayCategoryRow < ClientDataSet.Tables["PayCategoryUpload"].Rows.Count; intPayCategoryRow++)
            {
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.PAY_CATEGORY");
                strQry.AppendLine(" SET LAST_UPLOAD_DATETIME = '" + strUploadDateTime + "'");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + ClientDataSet.Tables["PayCategoryUpload"].Rows[intPayCategoryRow]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(ClientDataSet.Tables["PayCategoryUpload"].Rows[intPayCategoryRow]["PAY_CATEGORY_TYPE"].ToString()));

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }
        }

        public void Move_Data_To_History_And_Cleanup(Int64 parint64CompanyNo, string parstrPayrollType, string strPayCategoryWhere)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
           
            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_HISTORY ");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_HISTORY ");
                }
                else
                {
                    strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_HISTORY ");
                }
            }

            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO ");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",TIMESHEET_DATE");
            strQry.AppendLine(",TIMESHEET_SEQ");
            strQry.AppendLine(",TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine(",TIMESHEET_TIME_OUT_MINUTES");

            strQry.AppendLine(",CLOCKED_TIME_IN_MINUTES");
            strQry.AppendLine(",CLOCKED_TIME_OUT_MINUTES");

            //2016-08-26
            strQry.AppendLine(",INDICATOR");
            strQry.AppendLine(",TIMESHEET_ACCUM_MINUTES)");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ETC.COMPANY_NO");
            strQry.AppendLine(",ETC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",ETC.EMPLOYEE_NO");
            strQry.AppendLine(",ETC.TIMESHEET_DATE");
            strQry.AppendLine(",ETC.TIMESHEET_SEQ");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_OUT_MINUTES");
            strQry.AppendLine(",ETC.CLOCKED_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC.CLOCKED_TIME_OUT_MINUTES");

            //2016-08-26
            strQry.AppendLine(",ETC.INDICATOR");
            strQry.AppendLine(",ETC.TIMESHEET_ACCUM_MINUTES");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC ");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC ");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC ");
                }
            }

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");
            strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            //strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

            //Employee Linked to Current Pay Categories
            strQry.AppendLine(strPayCategoryWhere);

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_HISTORY ETH ");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_HISTORY ETH ");
                }
                else
                {
                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_HISTORY ETH ");
                }
            }

            strQry.AppendLine(" ON ETC.COMPANY_NO = ETH.COMPANY_NO");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = ETH.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = ETH.EMPLOYEE_NO");
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE = ETH.TIMESHEET_DATE");
            strQry.AppendLine(" AND ETC.TIMESHEET_SEQ = ETH.TIMESHEET_SEQ");
           
            strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(strPayCategoryWhere.Replace("EPC", "ETC"));

            strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= E.EMPLOYEE_LAST_RUNDATE");

            //Doesn't Exist
            strQry.AppendLine(" AND ETH.COMPANY_NO IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" DELETE ETC FROM InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC ");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine("DELETE ETC FROM  InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC ");
                }
                else
                {
                    strQry.AppendLine("DELETE ETC FROM  InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC ");
                }
            }

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");
            strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            //strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

            //Employee Linked to Current Pay Categories
            strQry.AppendLine(strPayCategoryWhere);

            strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(strPayCategoryWhere.Replace("EPC", "ETC"));

            strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= DATEADD(m,-3,E.EMPLOYEE_LAST_RUNDATE)");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            //Cleanup Broken Links
            strQry.Clear();
            strQry.AppendLine(" DELETE ETC");
            
            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC ");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC ");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC ");
                }
            }
            
            strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");

            strQry.AppendLine(" WHERE EPC.COMPANY_NO IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            //2016-08-26
            //BREAKS
            strQry.Clear();

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_BREAK_HISTORY ");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_HISTORY ");
                }
                else
                {
                    strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_HISTORY ");
                }
            }

            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO ");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",BREAK_DATE");
            strQry.AppendLine(",BREAK_SEQ");
            strQry.AppendLine(",BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",BREAK_TIME_OUT_MINUTES");

            strQry.AppendLine(",CLOCKED_TIME_IN_MINUTES");
            strQry.AppendLine(",CLOCKED_TIME_OUT_MINUTES");
            
            //2016-08-26
            strQry.AppendLine(",INDICATOR");
            strQry.AppendLine(",BREAK_ACCUM_MINUTES)");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ETC.COMPANY_NO");
            strQry.AppendLine(",ETC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",ETC.EMPLOYEE_NO");
            strQry.AppendLine(",ETC.BREAK_DATE");
            strQry.AppendLine(",ETC.BREAK_SEQ");
            strQry.AppendLine(",ETC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine(",ETC.CLOCKED_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC.CLOCKED_TIME_OUT_MINUTES");

            //2016-08-26
            strQry.AppendLine(",ETC.INDICATOR");
            strQry.AppendLine(",ETC.BREAK_ACCUM_MINUTES");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT ETC ");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ETC ");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETC ");
                }
            }

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");
            strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            //strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

            //Employee Linked to Current Pay Categories
            strQry.AppendLine(strPayCategoryWhere);

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_BREAK_HISTORY ETH ");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_HISTORY ETH ");
                }
                else
                {
                    strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_HISTORY ETH ");
                }
            }

            strQry.AppendLine(" ON ETC.COMPANY_NO = ETH.COMPANY_NO");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = ETH.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = ETH.EMPLOYEE_NO");
            strQry.AppendLine(" AND ETC.BREAK_DATE = ETH.BREAK_DATE");
            strQry.AppendLine(" AND ETC.BREAK_SEQ = ETH.BREAK_SEQ");

            strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(strPayCategoryWhere.Replace("EPC", "ETC"));

            strQry.AppendLine(" AND ETC.BREAK_DATE <= E.EMPLOYEE_LAST_RUNDATE");

            //Doesn't Exist
            strQry.AppendLine(" AND ETH.COMPANY_NO IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" DELETE ETC FROM InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT ETC ");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine("DELETE ETC FROM  InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ETC ");
                }
                else
                {
                    strQry.AppendLine("DELETE ETC FROM  InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETC ");
                }
            }

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");
            strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            //strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

            //Employee Linked to Current Pay Categories
            strQry.AppendLine(strPayCategoryWhere);

            strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(strPayCategoryWhere.Replace("EPC", "ETC"));

            strQry.AppendLine(" AND ETC.BREAK_DATE <= DATEADD(m,-3,E.EMPLOYEE_LAST_RUNDATE)");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            //Cleanup Broken Links
            strQry.Clear();
            strQry.AppendLine(" DELETE ETC");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT ETC ");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ETC ");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETC ");
                }
            }

            strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");

            strQry.AppendLine(" WHERE EPC.COMPANY_NO IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
        }

        public byte[] Get_Upload_Data(Int64 parint64CompanyNo, string parstrPayrollType, string strPayCategoryWhere, string parstrPayPeriodDate, byte[] parbyteDataSet)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            DataSet pvtDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);
            DataSet DataSet = new DataSet();

            string strCurrenDate = DateTime.Now.ToString("yyyy-MM-dd");

            //Read From Client Database Where Errors - NO UPLOAD IF TRUE
            strQry.Clear();
            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" EBC.INDICATOR ");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_BREAK_DAY_CURRENT EBC");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_CURRENT EBC");
                }
                else
                {
                    strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_BREAK_DAY_CURRENT EBC");
                }
            }

            strQry.AppendLine(" ON E.COMPANY_NO = EBC.COMPANY_NO");
            strQry.AppendLine(" AND EBC.COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EBC.EMPLOYEE_NO ");

            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");

            strQry.AppendLine(" AND EBC.TIMESHEET_DATE > ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrenDate + "'))");
            strQry.AppendLine(" AND EBC.TIMESHEET_DATE <= '" + parstrPayPeriodDate + "'");

            //Linked to Current Pay Categories
            strQry.AppendLine(strPayCategoryWhere.Replace("EPC.", "EBC."));

            strQry.AppendLine(" AND EBC.INDICATOR = 'X'");
            
            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ErrorCount");

            if (DataSet.Tables["ErrorCount"].Rows.Count > 0)
            {
                if (DataSet.Tables["ErrorCount"].Rows[0]["INDICATOR"].ToString() == "X")
                {
                    goto Get_Upload_Data_Continue;
                }
            }

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EPC.EMPLOYEE_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON EPC.COMPANY_NO = E.COMPANY_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPC.EMPLOYEE_NO = E.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

            strQry.AppendLine(" WHERE EPC.COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" EPC.EMPLOYEE_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "EmployeePayCategory");

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EMPLOYEE_NO");
            strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE");
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(" AND NOT_ACTIVE_IND IS NULL");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" EMPLOYEE_NO");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

            int intFindRow = 0;
           
            DataView pvtEmployeePayCategoryDataView = null;
            DataView pvtEmployeePayCategoryClientDataView = null;

            DataView pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                "COMPANY_NO = " + parint64CompanyNo.ToString() + " AND PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'",
                "EMPLOYEE_NO",
                DataViewRowState.CurrentRows);

            if (pvtEmployeeDataView.Count > 0)
            {
                for (int intRow = 0; intRow < DataSet.Tables["Employee"].Rows.Count; intRow++)
                {
                    intFindRow = pvtEmployeeDataView.Find(DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString());

                    //NB Only Update For Current Records - Employees Need To be Downloaded via Selection Process
                    if (intFindRow > -1)
                    {
                        if (DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_LAST_RUNDATE"] == System.DBNull.Value)
                        {
                            strQry.Clear();
                            strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.EMPLOYEE");
                            strQry.AppendLine(" SET ");
                            strQry.AppendLine(" EMPLOYEE_LAST_RUNDATE = '" + Convert.ToDateTime(pvtEmployeeDataView[intFindRow]["EMPLOYEE_LAST_RUNDATE"]).ToString("yyyy-MM-dd") + "'");
                            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString());

                            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                        }
                        else
                        {
                            if (Convert.ToDateTime(DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_LAST_RUNDATE"]) != Convert.ToDateTime(pvtEmployeeDataView[intFindRow]["EMPLOYEE_LAST_RUNDATE"]))
                            {
                                strQry.Clear();
                                strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.EMPLOYEE");
                                strQry.AppendLine(" SET ");
                                strQry.AppendLine(" EMPLOYEE_LAST_RUNDATE = '" + Convert.ToDateTime(pvtEmployeeDataView[intFindRow]["EMPLOYEE_LAST_RUNDATE"]).ToString("yyyy-MM-dd") + "'");
                                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
                                strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString());

                                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                            }
                        }

                        pvtEmployeePayCategoryDataView = null;
                        pvtEmployeePayCategoryDataView = new DataView(pvtDataSet.Tables["EmployeePayCategory"],
                            "COMPANY_NO = " + parint64CompanyNo.ToString() + " AND EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString() + " AND PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'",
                            "PAY_CATEGORY_NO",
                            DataViewRowState.CurrentRows);

                        //NB Client Gets DataSet for PAY_CATEGORY_TYPE
                        pvtEmployeePayCategoryClientDataView = null;
                        pvtEmployeePayCategoryClientDataView = new DataView(DataSet.Tables["EmployeePayCategory"],
                            "EMPLOYEE_NO = " + DataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString(),
                            "PAY_CATEGORY_NO",
                            DataViewRowState.CurrentRows);

                        //Insert/ Update Client Rows
                        for (int intEmployeePayCategoryRow = 0; intEmployeePayCategoryRow < pvtEmployeePayCategoryDataView.Count; intEmployeePayCategoryRow++)
                        {
                            intFindRow = pvtEmployeePayCategoryClientDataView.Find(pvtEmployeePayCategoryDataView[intEmployeePayCategoryRow]["PAY_CATEGORY_NO"].ToString());

                            if (intFindRow == -1)
                            {
                                strQry.Clear();
                                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY");
                                strQry.AppendLine("(COMPANY_NO");
                                strQry.AppendLine(",EMPLOYEE_NO");
                                strQry.AppendLine(",PAY_CATEGORY_NO");
                                strQry.AppendLine(",PAY_CATEGORY_TYPE)");
                                strQry.AppendLine(" VALUES ");
                                strQry.AppendLine("(" + parint64CompanyNo.ToString());
                                strQry.AppendLine("," + pvtEmployeePayCategoryDataView[intEmployeePayCategoryRow]["EMPLOYEE_NO"].ToString());
                                strQry.AppendLine("," + pvtEmployeePayCategoryDataView[intEmployeePayCategoryRow]["PAY_CATEGORY_NO"].ToString());
                                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType) + ")");

                                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                            }

                            //NB NB - nEED TO rEMOVE pAY cATEGORYS AS WELL

                            //else
                            //{
                            //    if (Convert.ToDouble(pvtEmployeePayCategoryClientDataView[0]["HOURLY_RATE"]) != Convert.ToDouble(pvtEmployeePayCategoryDataView[intFindRow]["HOURLY_RATE"])
                            //        | pvtEmployeePayCategoryClientDataView[0]["DEFAULT_IND"].ToString() != pvtEmployeePayCategoryDataView[intFindRow]["DEFAULT_IND"].ToString())
                            //    {
                            //        strQry.Clear();
                            //        strQry.AppendLine(" UPDATE ");
                            //        strQry.AppendLine(" EMPLOYEE_PAY_CATEGORY");
                            //        strQry.AppendLine(" SET ");
                            //        strQry.AppendLine(" HOURLY_RATE = " + Convert.ToDouble(pvtEmployeePayCategoryDataView[intFindRow]["HOURLY_RATE"]).ToString("#######0.00");
                            //        strQry.AppendLine(",DEFAULT_IND = " + clsDBConnectionObjects.Text2DynamicSQL(pvtEmployeePayCategoryDataView[intFindRow]["DEFAULT_IND"].ToString());
                            //        strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString();
                            //        strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["EmployeePayCategory"].Rows[intRow]["EMPLOYEE_NO"].ToString();
                            //        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + pvtEmployeePayCategoryDataView[intEmployeePayCategoryRow]["PAY_CATEGORY_NO"].ToString();

                            //        clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                            //    }
                            //}
                        }

                        //Delete Client Rows
                        for (int intEmployeePayCategoryRow = 0; intEmployeePayCategoryRow < pvtEmployeePayCategoryClientDataView.Count; intEmployeePayCategoryRow++)
                        {
                            intFindRow = pvtEmployeePayCategoryDataView.Find(pvtEmployeePayCategoryClientDataView[intEmployeePayCategoryRow]["PAY_CATEGORY_NO"].ToString());

                            if (intFindRow == -1)
                            {
                                strQry.Clear();
                                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY");
                                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
                                strQry.AppendLine(" AND EMPLOYEE_NO = " + pvtEmployeePayCategoryClientDataView[intEmployeePayCategoryRow]["EMPLOYEE_NO"].ToString());
                                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + pvtEmployeePayCategoryClientDataView[intEmployeePayCategoryRow]["PAY_CATEGORY_NO"].ToString());
                                strQry.AppendLine(" AND PAY_CATEGORY_TYPE  = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

                                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                            }
                        }
                    }
                }
            }

            //NB There would be an Error if there are Break Records Without Timesheet Record for Same Day
            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" TIMESHEET_DATE");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC ");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC ");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC ");
                }
            }

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");
            strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= '" + parstrPayPeriodDate + "'");
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE > E.EMPLOYEE_LAST_RUNDATE");

            //Employee Linked to Current Pay Categories
            strQry.AppendLine(strPayCategoryWhere);

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Dates");

            Get_Upload_Data_Continue:

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_Upload_Data_For_Day(Int64 parint64CompanyNo, string parstrPayrollType, string strPayCategoryWhere, string parstrDate)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" ETC.PAY_CATEGORY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC ");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC ");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC ");
                }
            }

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");
            strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE = '" + parstrDate + "'");

            //Employee Linked to Current Pay Categories
            strQry.AppendLine(strPayCategoryWhere);

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PayCategoryUpload");

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ETC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",ETC.EMPLOYEE_NO ");
            strQry.AppendLine(",ETC.TIMESHEET_SEQ ");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_IN_MINUTES ");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_OUT_MINUTES ");

            strQry.AppendLine(",ETC.CLOCKED_TIME_IN_MINUTES ");
            strQry.AppendLine(",ETC.CLOCKED_TIME_OUT_MINUTES ");

            if (parstrPayrollType.ToString() == "W")
            {
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC ");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC ");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC ");
                }
            }

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");
            strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
            strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
            //Employee Linked to Current Pay Categories
            strQry.AppendLine(strPayCategoryWhere);

            strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE = '" + parstrDate + "'");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" ETC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",ETC.EMPLOYEE_NO ");
            strQry.AppendLine(",ETC.TIMESHEET_SEQ ");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "TimeSheets");

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ETC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",ETC.EMPLOYEE_NO ");
            strQry.AppendLine(",ETC.BREAK_SEQ ");
            strQry.AppendLine(",ETC.BREAK_TIME_IN_MINUTES ");
            strQry.AppendLine(",ETC.BREAK_TIME_OUT_MINUTES ");

            strQry.AppendLine(",ETC.CLOCKED_TIME_IN_MINUTES ");
            strQry.AppendLine(",ETC.CLOCKED_TIME_OUT_MINUTES ");

            if (parstrPayrollType.ToString() == "W")
            {
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT ETC ");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ETC ");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETC ");
                }
            }

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE E ");
            strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
            strQry.AppendLine(" AND E.NOT_ACTIVE_IND IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
            //Employee Linked to Current Pay Categories
            strQry.AppendLine(strPayCategoryWhere);

            strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parint64CompanyNo.ToString());
            strQry.AppendLine(" AND ETC.BREAK_DATE = '" + parstrDate + "'");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" ETC.PAY_CATEGORY_NO ");
            strQry.AppendLine(",ETC.EMPLOYEE_NO ");
            strQry.AppendLine(",ETC.BREAK_SEQ ");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Breaks");

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
    }
}
