using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InteractPayroll;
using System.Data;
using System.Text;

namespace ValiditeWebSite.Models
{
    public class User
    {
        public Int32 UserNo { get; set; }
        public string UserId { get; set; }
        public string UserNames { get; set; }
        public string UserPassword { get; set; }
        public string ReturnMessage { get; set; }
    }

    public class Users
    {
        public User GetUser(User user)
        {
            User userReturn = new User();
            userReturn.ReturnMessage = "";
            userReturn.UserNames = "";
            userReturn.UserNo = -1;

            clsDBConnectionObjects _clsDBConnectionObjects = new InteractPayroll.clsDBConnectionObjects();
           
            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" USER_NO");
            strQry.AppendLine(",FIRSTNAME + ' '  + SURNAME AS USER_NAMES");

            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID");

            strQry.AppendLine(" WHERE USER_ID = " + _clsDBConnectionObjects.Text2DynamicSQL(user.UserId));
            strQry.AppendLine(" AND PASSWORD = " + _clsDBConnectionObjects.Text2DynamicSQL(user.UserPassword));
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            _clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "User", -1);

            if (DataSet.Tables["User"].Rows.Count > 0)
            {
                userReturn.UserNames = DataSet.Tables["User"].Rows[0]["USER_NAMES"].ToString();
                userReturn.UserNo = Convert.ToInt32(DataSet.Tables["User"].Rows[0]["USER_NO"]);
            }
           
            return userReturn;
        }
    }
}