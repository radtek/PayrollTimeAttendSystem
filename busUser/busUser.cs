using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busUser 
    {
        clsDBConnectionObjects clsDBConnectionObjects;
      
        public busUser()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(string parstrCurrentUserAccess, Int64 parint64CurrentUserNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" CL.COMPANY_NO");
            strQry.AppendLine(",CL.COMPANY_DESC");
            strQry.AppendLine(" FROM ");
            
            strQry.AppendLine(" InteractPayroll.dbo.COMPANY_LINK CL");

            if (parstrCurrentUserAccess != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA ");
                strQry.AppendLine(" ON CL.COMPANY_NO = UCA.COMPANY_NO ");
                strQry.AppendLine(" AND UCA.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND UCA.COMPANY_ACCESS_IND = 'A'");
                strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL");
            }
            
            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" CL.COMPANY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company",-1);
            
            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" U.USER_NO");
            strQry.AppendLine(",U.USER_ID");
            strQry.AppendLine(",U.FIRSTNAME");
            strQry.AppendLine(",U.SURNAME");
            strQry.AppendLine(",U.EMAIL");
            strQry.AppendLine(",U.LAST_TIME_ON");
            strQry.AppendLine(",ISNULL(U.LOCK_IND,'N') AS LOCK_IND");
            //2017-05-09
            strQry.AppendLine(",ISNULL(U.USER_CLOCK_PIN,'') AS USER_CLOCK_PIN");

            //Used To Save Value For Insert
            strQry.AppendLine(",'' AS PASSWORD");
            strQry.AppendLine(",'' AS RETURN_CODE");

            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID U");

            if (parstrCurrentUserAccess != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA ");
                strQry.AppendLine(" ON U.USER_NO = UCA.USER_NO  ");
                strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA1 ");
                strQry.AppendLine(" ON UCA.COMPANY_NO = UCA1.COMPANY_NO ");
                strQry.AppendLine(" AND UCA1.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND UCA1.COMPANY_ACCESS_IND = 'A' ");
               
                strQry.AppendLine(" AND UCA1.DATETIME_DELETE_RECORD IS NULL");
            }

            strQry.AppendLine(" WHERE U.DATETIME_DELETE_RECORD IS NULL");
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" U.SURNAME");
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "User",-1);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
        
            if (parstrCurrentUserAccess == "S")
            {
                strQry.AppendLine(" UCA.USER_NO");
                strQry.AppendLine(",UCA.COMPANY_NO");
                strQry.AppendLine(",UCA.TIE_BREAKER");
                strQry.AppendLine(",UCA.COMPANY_ACCESS_IND");

                strQry.AppendLine(",ISNULL(UCA.ACCESS_LAYER_IND,'C') AS ACCESS_LAYER_IND");
            }
            else
            {
                strQry.AppendLine(" UCA1.USER_NO");
                strQry.AppendLine(",UCA1.COMPANY_NO");
                strQry.AppendLine(",UCA1.TIE_BREAKER");
                strQry.AppendLine(",UCA1.COMPANY_ACCESS_IND");
                strQry.AppendLine(",ISNULL(UCA1.ACCESS_LAYER_IND,'C') AS ACCESS_LAYER_IND");
            }

            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_COMPANY_ACCESS UCA ");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_ID U ");
            strQry.AppendLine(" ON U.USER_NO = UCA.USER_NO  ");
            strQry.AppendLine(" AND U.DATETIME_DELETE_RECORD IS NULL");
            
            if (parstrCurrentUserAccess != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA1 ");
                strQry.AppendLine(" ON UCA1.COMPANY_NO = UCA.COMPANY_NO ");
                strQry.AppendLine(" AND UCA1.DATETIME_DELETE_RECORD IS NULL");
            }

            strQry.AppendLine(" WHERE UCA.DATETIME_DELETE_RECORD IS NULL");

            if (parstrCurrentUserAccess != "S")
            {
                strQry.AppendLine(" AND UCA.COMPANY_ACCESS_IND = 'A'");
                strQry.AppendLine(" AND UCA.USER_NO = " + parint64CurrentUserNo);
            }

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyAccess", -1);
            
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" UFT.USER_NO");
            strQry.AppendLine(",UFT.FINGER_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID U");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_FINGERPRINT_TEMPLATE UFT");
            strQry.AppendLine(" ON U.USER_NO = UFT.USER_NO  ");

            if (parstrCurrentUserAccess != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA ");
                strQry.AppendLine(" ON U.USER_NO = UCA.USER_NO  ");
                strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA1 ");
                strQry.AppendLine(" ON UCA.COMPANY_NO = UCA1.COMPANY_NO ");
                strQry.AppendLine(" AND UCA1.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND UCA1.COMPANY_ACCESS_IND = 'A' ");

                strQry.AppendLine(" AND UCA1.DATETIME_DELETE_RECORD IS NULL");
            }

            strQry.AppendLine(" WHERE U.DATETIME_DELETE_RECORD IS NULL");
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" UFT.USER_NO");
            strQry.AppendLine(",UFT.FINGER_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "UserFingerTemplate", -1);

            DataSet.Tables["UserFingerTemplate"].Columns.Add("FINGER_TEMPLATE", typeof(System.Byte[]));

            DataSet.AcceptChanges();

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
        
        public byte[] Insert_New_Record(Int64 parint64CurrentUserNo, byte[] parbyteDataSet)
        {
            StringBuilder strQry = new StringBuilder();

            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);
            DataSet DataSet = new DataSet();
            
            //Reset Value
            parDataSet.Tables["User"].Rows[0]["RETURN_CODE"] = "";

            strQry.Clear();

            strQry.AppendLine(" SELECT");

            strQry.AppendLine(" USER_ID");
            
            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID");

            strQry.AppendLine(" WHERE USER_ID = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["User"].Rows[0]["USER_ID"].ToString()));

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "UserId", -1);

            if (DataSet.Tables["UserId"].Rows.Count > 0)
            {
                parDataSet.Tables["User"].Rows[0]["RETURN_CODE"] = 9999;

                goto Insert_New_Record_Continue;
            }
            
            strQry.Clear();

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" USER_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID");

            strQry.AppendLine(" WHERE USER_CLOCK_PIN = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["User"].Rows[0]["USER_CLOCK_PIN"].ToString()));

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), parDataSet, "UserPinCheck", -1);

            if (parDataSet.Tables["UserPinCheck"].Rows.Count > 0)
            {
                goto Insert_New_Record_Continue;
            }
            
            strQry.Clear();

            strQry.AppendLine(" SELECT");

            strQry.AppendLine(" MAX(USER_NO) AS MAX_USER_NO");
            
            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", -1);

            if (DataSet.Tables["Temp"].Rows[0].IsNull("MAX_USER_NO") == true)
            {
                parDataSet.Tables["User"].Rows[0]["USER_NO"] = 1;
            }
            else
            {
                parDataSet.Tables["User"].Rows[0]["USER_NO"] = Convert.ToInt32(DataSet.Tables["Temp"].Rows[0]["MAX_USER_NO"]) + 1;
            }

            System.Random RandomNumber = new System.Random();

            parDataSet.Tables["User"].Rows[0]["PASSWORD"] = RandomNumber.Next(1000, 999999).ToString();
           
            strQry.Clear();

            strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.USER_ID ");
            strQry.AppendLine("(USER_NO");
            strQry.AppendLine(",USER_ID");
            strQry.AppendLine(",FIRSTNAME");
            strQry.AppendLine(",SURNAME");
            strQry.AppendLine(",PASSWORD");
            strQry.AppendLine(",RESET");
            strQry.AppendLine(",LAST_TIME_ON");
            strQry.AppendLine(",EMAIL");
            //2017-05-09
            strQry.AppendLine(",USER_CLOCK_PIN");
            strQry.AppendLine(",SYSTEM_ADMINISTRATOR_IND");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD)");
            
            strQry.AppendLine(" VALUES");
            
            strQry.AppendLine("(" + parDataSet.Tables["User"].Rows[0]["USER_NO"].ToString());
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["User"].Rows[0]["USER_ID"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["User"].Rows[0]["FIRSTNAME"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["User"].Rows[0]["SURNAME"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["User"].Rows[0]["PASSWORD"].ToString()));
            strQry.AppendLine(",'Y'");
            strQry.AppendLine(",Null");
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["User"].Rows[0]["EMAIL"].ToString()));
            //2017-05-09
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["User"].Rows[0]["USER_CLOCK_PIN"].ToString()));
            strQry.AppendLine(",'N'");
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo + ")");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            for (int intRow = 0; intRow < parDataSet.Tables["CompanyAccess"].Rows.Count; intRow++)
            {
                parDataSet.Tables["CompanyAccess"].Rows[intRow]["USER_NO"] = parDataSet.Tables["User"].Rows[0]["USER_NO"].ToString();

                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.USER_COMPANY_ACCESS ");
                strQry.AppendLine("(USER_NO");
                strQry.AppendLine(",COMPANY_NO");
                strQry.AppendLine(",TIE_BREAKER");
                strQry.AppendLine(",EMPLOYEE_NO");
                strQry.AppendLine(",COMPANY_ACCESS_IND");
                strQry.AppendLine(",ACCESS_LAYER_IND");
                strQry.AppendLine(",DATETIME_NEW_RECORD");
                strQry.AppendLine(",USER_NO_NEW_RECORD)");
                strQry.AppendLine(" VALUES");
                strQry.AppendLine("(" + parDataSet.Tables["User"].Rows[0]["USER_NO"].ToString());
                strQry.AppendLine("," + parDataSet.Tables["CompanyAccess"].Rows[intRow]["COMPANY_NO"].ToString());
                strQry.AppendLine(",1");
                //No Link to An Employee (Used For User Number = Employee Number)
                strQry.AppendLine(",-1");
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["CompanyAccess"].Rows[intRow]["COMPANY_ACCESS_IND"].ToString()));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["CompanyAccess"].Rows[intRow]["ACCESS_LAYER_IND"].ToString()));
                strQry.AppendLine(",GETDATE()");
                strQry.AppendLine("," + parint64CurrentUserNo + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            }

            if (parDataSet.Tables["UserFingerTemplate"].Rows.Count > 0)
            {
                Save_User_FingerTemplate(parDataSet.Tables["UserFingerTemplate"], Convert.ToInt64(parDataSet.Tables["User"].Rows[0]["USER_NO"]));
            }

        Insert_New_Record_Continue:

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(parDataSet);
            DataSet.Dispose();
            DataSet = null;
            
            return bytCompress;
        }

        public void Delete_Record(Int64 parint64CurrentUserNo, Int64 parint64UserNo)
        {
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_ID ");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
            strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
            strQry.AppendLine(" WHERE USER_NO = " + parint64UserNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_COMPANY_ACCESS ");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
            strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
            strQry.AppendLine(" WHERE USER_NO = " + parint64UserNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_MENU ");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
            strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
            strQry.AppendLine(" WHERE USER_NO = " + parint64UserNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_EMPLOYEE ");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
            strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
            strQry.AppendLine(" WHERE USER_NO = " + parint64UserNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_PAY_CATEGORY ");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
            strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
            strQry.AppendLine(" WHERE USER_NO = " + parint64UserNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_PAY_CATEGORY_DEPARTMENT ");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
            strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
            strQry.AppendLine(" WHERE USER_NO = " + parint64UserNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }

        public byte[] Update_Record(Int64 parint64CurrentUserNo, Int64 parintUserNo, byte[] parbyteDataSet)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            int intTieBreaker = 0;
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
            DataRow DataRow;

            //Create Empty Area to Add All NEW Records (Cater For TIE_BREAK)
            DataSet.Tables.Add(parDataSet.Tables["CompanyAccess"].Clone());

            string[] strQryArray = new string[1 + (5 * parDataSet.Tables["CompanyAccess"].Rows.Count)];

            if (parDataSet.Tables["User"].Rows.Count > 0)
            {
                //2017-05-13
                strQry.Clear();

                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" USER_NO");

                strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID");

                strQry.AppendLine(" WHERE USER_NO <> " + parDataSet.Tables["User"].Rows[0]["USER_NO"].ToString());
                strQry.AppendLine(" AND USER_CLOCK_PIN = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["User"].Rows[0]["USER_CLOCK_PIN"].ToString()));

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "UserPinCheck", -1);

                if (DataSet.Tables["UserPinCheck"].Rows.Count > 0)
                {
                    goto Update_Record_Continue;
                }

                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_ID");
                strQry.AppendLine(" SET");
                strQry.AppendLine(" USER_ID = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["User"].Rows[0]["USER_ID"].ToString()));
                strQry.AppendLine(",FIRSTNAME = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["User"].Rows[0]["FIRSTNAME"].ToString()));
                strQry.AppendLine(",SURNAME = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["User"].Rows[0]["SURNAME"].ToString()));
                strQry.AppendLine(",EMAIL = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["User"].Rows[0]["EMAIL"].ToString()));
                //2017-05-09
                strQry.AppendLine(",USER_CLOCK_PIN = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["User"].Rows[0]["USER_CLOCK_PIN"].ToString()));
                strQry.AppendLine(",LOCK_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["User"].Rows[0]["LOCK_IND"].ToString()));
                strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);
                strQry.AppendLine(" WHERE USER_NO = " + parDataSet.Tables["User"].Rows[0]["USER_NO"].ToString());
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
            }

            for (int intRow = 0; intRow < parDataSet.Tables["CompanyAccess"].Rows.Count; intRow++)
            {
                if (parDataSet.Tables["CompanyAccess"].Rows[intRow].RowState == System.Data.DataRowState.Added)
                {
                    if (DataSet.Tables["Temp"] != null)
                    {
                        DataSet.Tables.Remove("Temp");
                    }

                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" MAX(TIE_BREAKER) AS MAX_NO");
                    strQry.AppendLine(" FROM ");
                    strQry.AppendLine(" InteractPayroll.dbo.USER_COMPANY_ACCESS");
                    strQry.AppendLine(" WHERE USER_NO = " + parDataSet.Tables["CompanyAccess"].Rows[intRow]["USER_NO"].ToString());
                    strQry.AppendLine(" AND COMPANY_NO = " + parDataSet.Tables["CompanyAccess"].Rows[intRow]["COMPANY_NO"].ToString());

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp",-1);

                    if (DataSet.Tables["Temp"].Rows[0].IsNull("MAX_NO") == true)
                    {
                        intTieBreaker = 1;
                    }
                    else
                    {
                        intTieBreaker = Convert.ToInt32(DataSet.Tables["Temp"].Rows[0]["MAX_NO"]) + 1;
                    }

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.USER_COMPANY_ACCESS ");
                    strQry.AppendLine("(USER_NO");
                    strQry.AppendLine(",COMPANY_NO");
                    strQry.AppendLine(",TIE_BREAKER");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",COMPANY_ACCESS_IND");
                    strQry.AppendLine(",ACCESS_LAYER_IND");
                    strQry.AppendLine(",USER_NO_NEW_RECORD");
                    strQry.AppendLine(",DATETIME_NEW_RECORD)");
                    strQry.AppendLine(" VALUES");
                    strQry.AppendLine("(" + parDataSet.Tables["CompanyAccess"].Rows[intRow]["USER_NO"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables["CompanyAccess"].Rows[intRow]["COMPANY_NO"].ToString());
                    strQry.AppendLine("," + intTieBreaker);
                    //No Link to an Employee 
                    strQry.AppendLine(",-1");
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["CompanyAccess"].Rows[intRow]["COMPANY_ACCESS_IND"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["CompanyAccess"].Rows[intRow]["ACCESS_LAYER_IND"].ToString()));
                    strQry.AppendLine("," + parint64CurrentUserNo);
                    strQry.AppendLine(",GETDATE())");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    DataRow = DataSet.Tables["CompanyAccess"].NewRow();

                    //Add Row To Pass Back To GUI
                    DataRow["USER_NO"] = parDataSet.Tables["CompanyAccess"].Rows[intRow]["USER_NO"].ToString();
                    DataRow["COMPANY_NO"] = parDataSet.Tables["CompanyAccess"].Rows[intRow]["COMPANY_NO"].ToString();
                    DataRow["TIE_BREAKER"] = intTieBreaker;
                    DataRow["COMPANY_ACCESS_IND"] = parDataSet.Tables["CompanyAccess"].Rows[intRow]["COMPANY_ACCESS_IND"].ToString();
                    
                    DataSet.Tables["CompanyAccess"].Rows.Add(DataRow);
                }
                else
                {
                    if (parDataSet.Tables["CompanyAccess"].Rows[intRow].RowState == System.Data.DataRowState.Deleted)
                    {
                        strQry.Clear();
                        strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_COMPANY_ACCESS ");
                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                        strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE() ");
                        strQry.AppendLine(" WHERE USER_NO = " + parDataSet.Tables["CompanyAccess"].Rows[intRow]["USER_NO", System.Data.DataRowVersion.Original].ToString());
                        strQry.AppendLine(" AND COMPANY_NO = " + parDataSet.Tables["CompanyAccess"].Rows[intRow]["COMPANY_NO", System.Data.DataRowVersion.Original].ToString());
                        strQry.AppendLine(" AND TIE_BREAKER = " + parDataSet.Tables["CompanyAccess"].Rows[intRow]["TIE_BREAKER", System.Data.DataRowVersion.Original].ToString());
                        strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                        strQry.Clear();
                        strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_MENU ");
                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                        strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE() ");
                        strQry.AppendLine(" WHERE USER_NO = " + parDataSet.Tables["CompanyAccess"].Rows[intRow]["USER_NO", System.Data.DataRowVersion.Original].ToString());
                        strQry.AppendLine(" AND COMPANY_NO = " + parDataSet.Tables["CompanyAccess"].Rows[intRow]["COMPANY_NO", System.Data.DataRowVersion.Original].ToString());
                        strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                        strQry.Clear();
                        strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_EMPLOYEE ");
                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                        strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
                        strQry.AppendLine(" WHERE USER_NO = " + parDataSet.Tables["CompanyAccess"].Rows[intRow]["USER_NO", System.Data.DataRowVersion.Original].ToString());
                        strQry.AppendLine(" AND COMPANY_NO = " + parDataSet.Tables["CompanyAccess"].Rows[intRow]["COMPANY_NO", System.Data.DataRowVersion.Original].ToString());
                        strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                        strQry.Clear();
                        strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_PAY_CATEGORY ");
                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                        strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
                        strQry.AppendLine(" WHERE USER_NO = " + parDataSet.Tables["CompanyAccess"].Rows[intRow]["USER_NO", System.Data.DataRowVersion.Original].ToString());
                        strQry.AppendLine(" AND COMPANY_NO = " + parDataSet.Tables["CompanyAccess"].Rows[intRow]["COMPANY_NO", System.Data.DataRowVersion.Original].ToString());
                        strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                        
                        strQry.Clear();
                        strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_PAY_CATEGORY_DEPARTMENT ");
                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                        strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
                        strQry.AppendLine(" WHERE USER_NO = " + parDataSet.Tables["CompanyAccess"].Rows[intRow]["USER_NO", System.Data.DataRowVersion.Original].ToString());
                        strQry.AppendLine(" AND COMPANY_NO = " + parDataSet.Tables["CompanyAccess"].Rows[intRow]["COMPANY_NO", System.Data.DataRowVersion.Original].ToString());
                        strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                    }
                    else
                    {
                        if (parDataSet.Tables["CompanyAccess"].Rows[intRow].RowState == System.Data.DataRowState.Modified)
                        {
                            strQry.Clear();
                            strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_COMPANY_ACCESS ");
                            strQry.AppendLine(" SET ");
                            strQry.AppendLine(" COMPANY_ACCESS_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["CompanyAccess"].Rows[intRow]["COMPANY_ACCESS_IND"].ToString()));
                            strQry.AppendLine(",ACCESS_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["CompanyAccess"].Rows[intRow]["ACCESS_LAYER_IND"].ToString()));
                            strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);
                            strQry.AppendLine(" WHERE USER_NO = " + parDataSet.Tables["CompanyAccess"].Rows[intRow]["USER_NO"].ToString());
                            strQry.AppendLine(" AND COMPANY_NO = " + parDataSet.Tables["CompanyAccess"].Rows[intRow]["COMPANY_NO"].ToString());
                            strQry.AppendLine(" AND TIE_BREAKER = " + parDataSet.Tables["CompanyAccess"].Rows[intRow]["TIE_BREAKER"].ToString());
                            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                            //Administrator
                            if (parDataSet.Tables["CompanyAccess"].Rows[intRow]["COMPANY_ACCESS_IND"].ToString() == "A")
                            {
                                //Remove All Links to Menus
                                strQry.Clear();
                                strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_MENU ");
                                strQry.AppendLine(" SET ");
                                strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                                strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE() ");
                                strQry.AppendLine(" WHERE USER_NO = " + parDataSet.Tables["CompanyAccess"].Rows[intRow]["USER_NO"].ToString());
                                strQry.AppendLine(" AND COMPANY_NO = " + parDataSet.Tables["CompanyAccess"].Rows[intRow]["COMPANY_NO"].ToString());
                                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

                                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                                strQry.Clear();
                                strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_EMPLOYEE ");
                                strQry.AppendLine(" SET ");
                                strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                                strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
                                strQry.AppendLine(" WHERE USER_NO = " + parDataSet.Tables["CompanyAccess"].Rows[intRow]["USER_NO"].ToString());
                                strQry.AppendLine(" AND COMPANY_NO = " + parDataSet.Tables["CompanyAccess"].Rows[intRow]["COMPANY_NO"].ToString());
                                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

                                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                                strQry.Clear();
                                strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_PAY_CATEGORY ");
                                strQry.AppendLine(" SET ");
                                strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                                strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
                                strQry.AppendLine(" WHERE USER_NO = " + parDataSet.Tables["CompanyAccess"].Rows[intRow]["USER_NO"].ToString());
                                strQry.AppendLine(" AND COMPANY_NO = " + parDataSet.Tables["CompanyAccess"].Rows[intRow]["COMPANY_NO"].ToString());
                                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

                                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                                strQry.Clear();
                                strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_PAY_CATEGORY_DEPARTMENT ");
                                strQry.AppendLine(" SET ");
                                strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                                strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
                                strQry.AppendLine(" WHERE USER_NO = " + parDataSet.Tables["CompanyAccess"].Rows[intRow]["USER_NO"].ToString());
                                strQry.AppendLine(" AND COMPANY_NO = " + parDataSet.Tables["CompanyAccess"].Rows[intRow]["COMPANY_NO"].ToString());
                                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

                                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                            }
                        }
                    }
                }
            }

            if (parDataSet.Tables["UserFingerTemplate"].Rows.Count > 0)
            {
                Save_User_FingerTemplate(parDataSet.Tables["UserFingerTemplate"], parintUserNo);
            }

            if (DataSet.Tables["Temp"] != null)
            {
                DataSet.Tables.Remove("Temp");
            }

            Update_Record_Continue:

            //Pass NEW CompanyAccess Records Back
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public void Save_User_FingerTemplate(DataTable parDataTable, Int64 parintUserNo)
        {
            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();

            for (int intRow = 0; intRow < parDataTable.Rows.Count; intRow++)
            {
                if (parDataTable.Rows[intRow].RowState == DataRowState.Added
                ||  parDataTable.Rows[intRow].RowState == DataRowState.Unchanged)
                {
                    //2017-06-19 Unchanged comes from New User
                    //2017-04-29 - NB New Template Will Replace Old
                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.USER_FINGERPRINT_TEMPLATE");
                    strQry.AppendLine(" WHERE USER_NO = " + parintUserNo);
                    strQry.AppendLine(" AND FINGER_NO = " + parDataTable.Rows[intRow]["FINGER_NO"].ToString());

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.USER_FINGERPRINT_TEMPLATE");
                    strQry.AppendLine("(USER_NO");
                    strQry.AppendLine(",FINGER_NO");
                    strQry.AppendLine(",FINGER_TEMPLATE");
                    strQry.AppendLine(",CREATION_DATETIME) ");

                    strQry.AppendLine(" VALUES ");
                    strQry.AppendLine("(" + parintUserNo);
                    strQry.AppendLine("," + parDataTable.Rows[intRow]["FINGER_NO"].ToString());
                    strQry.AppendLine(",@FINGER_TEMPLATE");
                    strQry.AppendLine(",GETDATE())");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), (byte[])parDataTable.Rows[intRow]["FINGER_TEMPLATE"], "@FINGER_TEMPLATE", -1);
                }
                else
                {
                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.USER_FINGERPRINT_TEMPLATE");
                    strQry.AppendLine(" WHERE USER_NO = " + parDataTable.Rows[intRow]["USER_NO", DataRowVersion.Original].ToString());
                    strQry.AppendLine(" AND FINGER_NO = " + parDataTable.Rows[intRow]["FINGER_NO", DataRowVersion.Original].ToString());

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                }
            }
        }
    }
}
