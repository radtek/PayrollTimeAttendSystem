using System;
using System.Collections.Generic;
using System.Text;

namespace InteractPayroll
{
    public class busPasswordChange
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busPasswordChange()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public void Update_Password(Int64 parint64CurrentUserNo, string parstrPassword)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_ID");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" PASSWORD = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPassword));
            strQry.AppendLine(",RESET = 'N'");
            strQry.AppendLine(",USER_NO = " + parint64CurrentUserNo);
            strQry.AppendLine(" WHERE USER_NO = " + parint64CurrentUserNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }
    }
}
