using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busPasswordReset
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busPasswordReset()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(string parstrCurrentUserAccess, Int64 parint64CurrentUserNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            if (parstrCurrentUserAccess == "S")
            {
                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" USER_NO");
                strQry.AppendLine(",USER_ID");
                strQry.AppendLine(",FIRSTNAME");
                strQry.AppendLine(",SURNAME");

                strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID");
                strQry.AppendLine(" WHERE USER_NO <> " + parint64CurrentUserNo);
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");
            }
            else
            {
                strQry.Clear();
                strQry.AppendLine(" SELECT DISTINCT");
                strQry.AppendLine(" UI.USER_NO");
                strQry.AppendLine(",UI.USER_ID");
                strQry.AppendLine(",UI.FIRSTNAME");
                strQry.AppendLine(",UI.SURNAME");

                strQry.AppendLine(" FROM ");
                strQry.AppendLine(" InteractPayroll.dbo.USER_ID UI");
                strQry.AppendLine(",InteractPayroll.dbo.USER_COMPANY_ACCESS UCA ");

                strQry.AppendLine(" WHERE UI.USER_NO = UCA.USER_NO ");
                strQry.AppendLine(" AND UI.DATETIME_DELETE_RECORD IS NULL ");
                strQry.AppendLine(" AND UCA.USER_NO <> " + parint64CurrentUserNo);
                strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL ");
                strQry.AppendLine(" AND UI.SYSTEM_ADMINISTRATOR_IND <> 'Y'");
                strQry.AppendLine(" AND STR(UCA.COMPANY_NO) IN ");
                strQry.AppendLine("(SELECT STR(COMPANY_NO) ");
                strQry.AppendLine(" FROM ");
                strQry.AppendLine(" InteractPayroll.dbo.USER_COMPANY_ACCESS ");
                strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");
                strQry.AppendLine(" AND COMPANY_ACCESS_IND = 'A') ");
            }

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "User", -1);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public string Update_Password(Int64 parint64UserNo, Int64 parint64CurrentUserNo)
        {
            StringBuilder strQry = new StringBuilder();
            System.Random RandomNumber = new System.Random();
            string strPassword = RandomNumber.Next(1000, 999999).ToString();
                       
            strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_ID");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" RESET = 'Y'");
            strQry.AppendLine(",PASSWORD = " + clsDBConnectionObjects.Text2DynamicSQL(strPassword));
            strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);
            strQry.AppendLine(" WHERE USER_NO = " + parint64UserNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            return strPassword;
        }
    }
}
