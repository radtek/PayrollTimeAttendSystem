using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busUserMenuAccess
    {
        clsDBConnectionObjects clsDBConnectionObjects;
       
        public busUserMenuAccess()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Users(string parstrProgramFromInd,Int64 parCurrentUserNo, string parstrCurrentUserAccessInd)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" U.USER_NO");
            strQry.AppendLine(",U.USER_ID");
            strQry.AppendLine(",U.FIRSTNAME");
            strQry.AppendLine(",U.SURNAME");

            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID U");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA");
            strQry.AppendLine(" ON U.USER_NO = UCA.USER_NO");
            strQry.AppendLine(" AND UCA.COMPANY_ACCESS_IND <> 'A' ");
            strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL ");

            if (parstrCurrentUserAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(SELECT ");
                strQry.AppendLine(" UCA.COMPANY_NO");

                strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID U");

                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA");
                strQry.AppendLine(" ON U.USER_NO = UCA.USER_NO");
                strQry.AppendLine(" AND UCA.COMPANY_ACCESS_IND = 'A' ");
                strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL ");

                strQry.AppendLine(" WHERE UCA.USER_NO = " + parCurrentUserNo);
                strQry.AppendLine(" AND U.DATETIME_DELETE_RECORD IS NULL) AS TEMP_TABLE ");

                strQry.AppendLine(" ON UCA.COMPANY_NO = TEMP_TABLE.COMPANY_NO");
            }

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" U.USER_NO");
            strQry.AppendLine(",U.USER_ID");
            strQry.AppendLine(",U.FIRSTNAME");
            strQry.AppendLine(",U.SURNAME");
         
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "User",-1);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" U.USER_NO");
            strQry.AppendLine(",UCA.COMPANY_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID U");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA");
            strQry.AppendLine(" ON U.USER_NO = UCA.USER_NO");
            strQry.AppendLine(" AND UCA.COMPANY_ACCESS_IND <> 'A' ");
            strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL ");

            if (parstrCurrentUserAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(SELECT ");
                strQry.AppendLine(" UCA.COMPANY_NO");

                strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID U");

                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA");
                strQry.AppendLine(" ON U.USER_NO = UCA.USER_NO");
                strQry.AppendLine(" AND UCA.COMPANY_ACCESS_IND = 'A' ");
                strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL ");

                strQry.AppendLine(" WHERE UCA.USER_NO = " + parCurrentUserNo);
                strQry.AppendLine(" AND U.DATETIME_DELETE_RECORD IS NULL) AS TEMP_TABLE ");

                strQry.AppendLine(" ON UCA.COMPANY_NO = TEMP_TABLE.COMPANY_NO");
            }

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" U.USER_NO");
            strQry.AppendLine(",UCA.COMPANY_NO");

            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" U.USER_NO");
            strQry.AppendLine(",UCA.COMPANY_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "UserCompany", -1);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" CL.COMPANY_NO");
            strQry.AppendLine(",CL.COMPANY_DESC");
                
            strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK CL");

            if (parstrCurrentUserAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(SELECT ");
                strQry.AppendLine(" UCA.COMPANY_NO");

                strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID U");

                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA");
                strQry.AppendLine(" ON U.USER_NO = UCA.USER_NO");
                strQry.AppendLine(" AND UCA.COMPANY_ACCESS_IND = 'A' ");
                strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL ");

                strQry.AppendLine(" WHERE UCA.USER_NO = " + parCurrentUserNo);
                strQry.AppendLine(" AND U.DATETIME_DELETE_RECORD IS NULL) AS TEMP_TABLE ");

                strQry.AppendLine(" ON CL.COMPANY_NO = TEMP_TABLE.COMPANY_NO");
            }

            strQry.AppendLine(" GROUP BY");
            strQry.AppendLine(" CL.COMPANY_NO");
            strQry.AppendLine(",CL.COMPANY_DESC");
                  
            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" CL.COMPANY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", -1);

            if (DataSet.Tables["User"].Rows.Count > 0)
            {
                byte[] bytTempCompress = Get_User_Menus(parstrProgramFromInd,Convert.ToInt32(DataSet.Tables["User"].Rows[0]["USER_NO"]));
                DataSet TempDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(bytTempCompress);
                DataSet.Merge(TempDataSet);
            }

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Update_User_Menus(string parstrProgramFromInd,Int64 parint64CurrentUserNo, byte[] parbyteDataSet)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            int intTieBreaker = 0;
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
            Int64 int64UserNo = -1;

            DateTime DateTimeNew = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            for (int intRow = 0; intRow < parDataSet.Tables[0].Rows.Count; intRow++)
            {
                if (parDataSet.Tables[0].Rows[intRow].RowState == System.Data.DataRowState.Added)
                {
                    int64UserNo = Convert.ToInt64(parDataSet.Tables[0].Rows[intRow]["USER_NO"]);

                    if (DataSet.Tables["Temp"] != null)
                    {
                        DataSet.Tables.Remove("Temp");
                    }

                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" MAX(TIE_BREAKER) AS MAX_NO");
                
                    strQry.AppendLine(" FROM InteractPayroll.dbo.USER_MENU");

                    strQry.AppendLine(" WHERE FROM_PROGRAM_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProgramFromInd));
                    strQry.AppendLine(" AND USER_NO = " + parDataSet.Tables[0].Rows[intRow]["USER_NO"].ToString());
                    strQry.AppendLine(" AND COMPANY_NO = " + parDataSet.Tables[0].Rows[intRow]["COMPANY_NO"].ToString());
                    strQry.AppendLine(" AND MENU_ITEM_ID = " + this.clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[intRow]["MENU_ITEM_ID"].ToString()));

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", -1);

                    if (DataSet.Tables["Temp"].Rows[0].IsNull("MAX_NO") == true)
                    {
                        intTieBreaker = 1;
                    }
                    else
                    {
                        intTieBreaker = Convert.ToInt32(DataSet.Tables["Temp"].Rows[0]["MAX_NO"]) + 1;
                    }

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.USER_MENU ");
                    strQry.AppendLine("(FROM_PROGRAM_IND");
                    strQry.AppendLine(",COMPANY_NO");
                    strQry.AppendLine(",USER_NO");
                    strQry.AppendLine(",MENU_ITEM_ID");
                    strQry.AppendLine(",TIE_BREAKER");
                    strQry.AppendLine(",ACCESS_IND");
                    strQry.AppendLine(",DATETIME_NEW_RECORD");
                    strQry.AppendLine(",USER_NO_NEW_RECORD)");
                    
                    strQry.AppendLine(" VALUES ");

                    strQry.AppendLine("(" + clsDBConnectionObjects.Text2DynamicSQL(parstrProgramFromInd));
                    strQry.AppendLine("," + parDataSet.Tables[0].Rows[intRow]["COMPANY_NO"].ToString());
                    strQry.AppendLine("," + parDataSet.Tables[0].Rows[intRow]["USER_NO"].ToString());
                    strQry.AppendLine("," + this.clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[intRow]["MENU_ITEM_ID"].ToString()));
                    strQry.AppendLine("," + intTieBreaker);
                    strQry.AppendLine("," + this.clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[intRow]["ACCESS_IND"].ToString()));
                    strQry.AppendLine(",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                    strQry.AppendLine("," + parint64CurrentUserNo + ")");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(),-1);
                }
                else
                {
                    if (parDataSet.Tables[0].Rows[intRow].RowState == System.Data.DataRowState.Deleted)
                    {
                        int64UserNo = Convert.ToInt64(parDataSet.Tables[0].Rows[intRow]["USER_NO", DataRowVersion.Original]);

                        strQry.Clear();
                        strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_MENU ");
                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                        strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE() ");

                        strQry.AppendLine(" WHERE FROM_PROGRAM_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProgramFromInd));
                        strQry.AppendLine(" AND USER_NO = " + parDataSet.Tables[0].Rows[intRow]["USER_NO", DataRowVersion.Original].ToString());
                        strQry.AppendLine(" AND COMPANY_NO = " + parDataSet.Tables[0].Rows[intRow]["COMPANY_NO", DataRowVersion.Original].ToString());
                        strQry.AppendLine(" AND MENU_ITEM_ID = " + parDataSet.Tables[0].Rows[intRow]["MENU_ITEM_ID", DataRowVersion.Original].ToString());
                        strQry.AppendLine(" AND TIE_BREAKER = " + parDataSet.Tables[0].Rows[intRow]["TIE_BREAKER", DataRowVersion.Original].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                    }
                }
            }

            byte[] bytCompress = Get_User_Menus(parstrProgramFromInd,int64UserNo);

            parDataSet.Dispose();
            parDataSet = null;

            return bytCompress;
        }

        public byte[] Get_User_Menus(string parstrProgramFromInd,Int64 parint64CurrentUserNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" USER_NO");
          
            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID U");

            strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo);
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "UserLoaded", -1);
            
            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",USER_NO");
            strQry.AppendLine(",MENU_ITEM_ID");
            strQry.AppendLine(",TIE_BREAKER");
            strQry.AppendLine(",ACCESS_IND");
            
            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_MENU");

            strQry.AppendLine(" WHERE FROM_PROGRAM_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parstrProgramFromInd));
            strQry.AppendLine(" AND USER_NO = " + parint64CurrentUserNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" COMPANY_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "UserMenu", -1);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
    }
}
