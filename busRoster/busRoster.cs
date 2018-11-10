using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace InteractPayroll
{
    public class busRoster
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busRoster()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo, string parstrTimeAttendInd, string parstrCurrentUserAccess, Int64 parint64CurrentUserNo)
        {
            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PUBLIC_HOLIDAY_DATE");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY PH");
         
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PublicHoliday", parint64CompanyNo);
            
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" P.COMPANY_NO");
            strQry.AppendLine(",P.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",P.PAY_CATEGORY_NO");
            strQry.AppendLine(",P.PAY_CATEGORY_DESC");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY P");

            strQry.AppendLine(" WHERE P.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND P.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND P.PAY_CATEGORY_NO > 0");

            //2013-04-10
            if (parstrTimeAttendInd == "Y")
            {
                if (parstrCurrentUserAccess != "S")
                {
                    strQry.AppendLine(" AND P.PAY_CATEGORY_TYPE = 'T'");
                }
            }
            else
            {
                if (parstrCurrentUserAccess != "S")
                {
                    strQry.AppendLine(" AND P.PAY_CATEGORY_TYPE IN ('W','S')");
                }
            }

            //Has Not Been Deleted
            strQry.AppendLine(" AND P.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" P.PAY_CATEGORY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategory", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PAY_CATEGORY_SHIFT_SCHEDULE_NO");
            strQry.AppendLine(",FROM_DATETIME");
            strQry.AppendLine(",TO_DATETIME");
            strQry.AppendLine(",ISNULL(LOCKED_IND,0) AS LOCKED_IND");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_SHIFT_SCHEDULE");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            if (parstrTimeAttendInd == "Y")
            {
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE IN ('W','S')");
            }

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",FROM_DATETIME DESC");
            strQry.AppendLine(",TO_DATETIME DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString().ToString(), DataSet, "ShiftSchedule", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PAY_CATEGORY_SHIFT_SCHEDULE_NO");
            strQry.AppendLine(",SHIFT_SCHEDULE_DATETIME");
            strQry.AppendLine(",FROM_HHMM_MINUTES");
            strQry.AppendLine(",TO_HHMM_MINUTES");
        
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_SHIFT_SCHEDULE");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            if (parstrTimeAttendInd == "Y")
            {
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE IN ('W','S')");
            }

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" EMPLOYEE_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PAY_CATEGORY_SHIFT_SCHEDULE_NO");
            strQry.AppendLine(",SHIFT_SCHEDULE_DATETIME");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString().ToString(), DataSet, "EmployeeShiftSchedule", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME + ',' + E.EMPLOYEE_NAME AS EMPLOYEE_NAMES");
            strQry.AppendLine(",O.OCCUPATION_DESC");
            strQry.AppendLine(",MAX(EPCSS.PAY_CATEGORY_SHIFT_SCHEDULE_NO) as PAY_CATEGORY_SHIFT_SCHEDULE_NO");
           
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

            //Change Later
            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.OCCUPATION O");
            strQry.AppendLine(" ON E.COMPANY_NO = O.COMPANY_NO ");
            strQry.AppendLine(" AND E.OCCUPATION_NO = O.OCCUPATION_NO ");
            strQry.AppendLine(" AND O.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_SHIFT_SCHEDULE EPCSS");
            strQry.AppendLine(" ON E.COMPANY_NO = EPCSS.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCSS.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = EPCSS.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCSS.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);

            if (parstrTimeAttendInd == "Y")
            {
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE IN ('W','S')");
            }

            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME + ',' + E.EMPLOYEE_NAME");
            strQry.AppendLine(",O.OCCUPATION_DESC");
            strQry.AppendLine(",EPCSS.PAY_CATEGORY_SHIFT_SCHEDULE_NO");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME + ',' + E.EMPLOYEE_NAME");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString().ToString(), DataSet, "Employee", parint64CompanyNo);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Update_Record(Int64 parInt64CompanyNo, Int64 parint64CurrentUserNo, byte[] parbyteDataSet, string parstrPayCategoryType)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            StringBuilder strQry = new StringBuilder();

            for (int intRow = 0; intRow < parDataSet.Tables["EmployeeShiftSchedule"].Rows.Count; intRow++)
            {
                if (intRow == 0)
                {
                    //Delete all Records For PAY_CATEGORY_SHIFT_SCHEDULE_NO 
                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_SHIFT_SCHEDULE");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parDataSet.Tables["EmployeeShiftSchedule"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EmployeeShiftSchedule"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine(" AND PAY_CATEGORY_SHIFT_SCHEDULE_NO = " + parDataSet.Tables["EmployeeShiftSchedule"].Rows[intRow]["PAY_CATEGORY_SHIFT_SCHEDULE_NO"].ToString());

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                }

                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_SHIFT_SCHEDULE ");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",EMPLOYEE_NO");
                strQry.AppendLine(",PAY_CATEGORY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",PAY_CATEGORY_SHIFT_SCHEDULE_NO");
                strQry.AppendLine(",SHIFT_SCHEDULE_DATETIME");
                strQry.AppendLine(",FROM_HHMM");
                strQry.AppendLine(",TO_HHMM");
                strQry.AppendLine(",FORECOLOR_HEX");
                strQry.AppendLine(",BACKCOLOR_HEX)");

                strQry.AppendLine(" VALUES ");

                strQry.AppendLine("(" + parInt64CompanyNo.ToString());
                strQry.AppendLine("," + parDataSet.Tables["EmployeeShiftSchedule"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                strQry.AppendLine("," + parDataSet.Tables["EmployeeShiftSchedule"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EmployeeShiftSchedule"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                strQry.AppendLine("," + parDataSet.Tables["EmployeeShiftSchedule"].Rows[intRow]["PAY_CATEGORY_SHIFT_SCHEDULE_NO"].ToString());
                strQry.AppendLine(",'" + Convert.ToDateTime(parDataSet.Tables["EmployeeShiftSchedule"].Rows[intRow]["SHIFT_SCHEDULE_DATETIME"]).ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EmployeeShiftSchedule"].Rows[intRow]["FROM_HHMM"].ToString()));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EmployeeShiftSchedule"].Rows[intRow]["TO_HHMM"].ToString()));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EmployeeShiftSchedule"].Rows[intRow]["FORECOLOR_HEX"].ToString()));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["EmployeeShiftSchedule"].Rows[intRow]["BACKCOLOR_HEX"].ToString()) + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            }

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(parDataSet);
            parDataSet.Dispose();
            parDataSet = null;

            return bytCompress;
        }

        public int Delete_Record(Int64 parint64CurrentUserNo, Int64 parint64CompanyNo, int parintPayCategoryNo, string parstrPayCategoryType)
        {
            int intReturnCode = 0;
            DataSet myDataSet = new DataSet();

            StringBuilder strQry = new StringBuilder();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EMPLOYEE_NO");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY ");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayCategoryType + "'");
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), myDataSet, "Temp", parint64CompanyNo);

            if (myDataSet.Tables["Temp"].Rows.Count > 0)
            {
                intReturnCode = 99;
            }
            else
            {
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parintPayCategoryNo);
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            }

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            return intReturnCode;
        }


        public byte[] Print_Report(Int64 parInt64CompanyNo, string parstrPayCategoryType,int parstrPayCategoryNo,int parintScheduleNo)
        {
            DataSet DataSet = new DataSet();

            StringBuilder strQry = new StringBuilder();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" C.COMPANY_DESC");
            strQry.AppendLine(",P.PAY_CATEGORY_DESC + ' Roster' AS REPORT_HEADER_DESC");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME + ',' + SUBSTRING(E.EMPLOYEE_NAME,1,1) AS EMPLOYEE_NAMES");
            //strQry.AppendLine(",O.OCCUPATION_DESC");

            strQry.AppendLine(",SHIFT_SCHEDULE_DATETIME_DESC = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN CL.DATE_FORMAT = 'yyyy-MM-dd'");
            strQry.AppendLine(" THEN CONVERT(VARCHAR(10),EPCSS.SHIFT_SCHEDULE_DATETIME,120)");

            //dd-MMMM-yyyy
            strQry.AppendLine(" ELSE CONVERT(VARCHAR(10),EPCSS.SHIFT_SCHEDULE_DATETIME,105)");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",EPCSS.SHIFT_SCHEDULE_DATETIME");
            strQry.AppendLine(",EPCSS.FROM_HHMM");
            strQry.AppendLine(",EPCSS.TO_HHMM");
            
            //White (Default)
            strQry.AppendLine(",MAX(EPCSS.FORECOLOR_HEX) AS FORECOLOR_HEX");
            //Black (Default)
            strQry.AppendLine(",MAX(EPCSS.BACKCOLOR_HEX) AS BACKCOLOR_HEX");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C");
            strQry.AppendLine(" ON C.COMPANY_NO = E.COMPANY_NO");
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.COMPANY_LINK CL");
            strQry.AppendLine(" ON C.COMPANY_NO =  CL.COMPANY_NO");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = " + parstrPayCategoryNo);
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");
            
            strQry.AppendLine(" INNER JOIN  InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY P");
            strQry.AppendLine(" ON E.COMPANY_NO = P.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = P.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = P.PAY_CATEGORY_NO");

            strQry.AppendLine(" AND P.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND P.PAY_CATEGORY_NO > 0");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_SHIFT_SCHEDULE EPCSS");
            strQry.AppendLine(" ON E.COMPANY_NO = EPCSS.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCSS.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = EPCSS.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCSS.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPCSS.PAY_CATEGORY_SHIFT_SCHEDULE_NO = " + parintScheduleNo);

            //Change Later
            //strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.OCCUPATION O");
            //strQry.AppendLine(" ON E.COMPANY_NO = O.COMPANY_NO ");
            //strQry.AppendLine(" AND E.OCCUPATION_NO = O.OCCUPATION_NO ");
            //strQry.AppendLine(" AND O.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
            
            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" C.COMPANY_DESC");
            strQry.AppendLine(",CL.DATE_FORMAT");
            strQry.AppendLine(",P.PAY_CATEGORY_DESC");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME + ',' + SUBSTRING(E.EMPLOYEE_NAME,1,1)");
            //strQry.AppendLine(",O.OCCUPATION_DESC");
            strQry.AppendLine(",EPCSS.SHIFT_SCHEDULE_DATETIME");
            strQry.AppendLine(",EPCSS.FROM_HHMM");
            strQry.AppendLine(",EPCSS.TO_HHMM");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" EPCSS.SHIFT_SCHEDULE_DATETIME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME + ',' + SUBSTRING(E.EMPLOYEE_NAME,1,1)");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString().ToString(), DataSet, "Report", parInt64CompanyNo);
                     

            //DataSet.Tables["Report"].Rows[0]["EMPLOYEE_COLOR"] = "#FF8080";


            



            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        private System.Drawing.Color GetSystemDrawingColorFromHexString(string hexString)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(hexString, @"[#]([0-9]|[a-f]|[A-F]){6}\b"))
                throw new ArgumentException();
            int red = int.Parse(hexString.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
            int green = int.Parse(hexString.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
            int blue = int.Parse(hexString.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
            return System.Drawing.Color.FromArgb(red, green, blue);
        }
    }
}
