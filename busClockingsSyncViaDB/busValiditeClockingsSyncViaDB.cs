using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InteractPayroll;
using System.Data;

namespace InteractPayrollClient
{
    public class busValiditeClockingsSyncViaDB
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busValiditeClockingsSyncViaDB()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Insert_Records(string parstrParm)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            int intTimeMinutes = 0;
            
            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" EC.EMPLOYEE_NO");

            strQry.AppendLine(" FROM ShoutItNowTest.dbo.EMPLOYEE_CLOCKINGS EC ");

            strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.EMPLOYEE E ");
            strQry.AppendLine(" ON EC.EMPLOYEE_NO = E.EMPLOYEE_3RD_PARTY_CODE ");
            
            //No Link
            strQry.AppendLine(" WHERE E.EMPLOYEE_3RD_PARTY_CODE IS NULL");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "LinksMissing");

            strQry.Clear();

            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" EMPLOYEE_NO");
            strQry.AppendLine(",CLOCKING_TIME");
            strQry.AppendLine(",CLOCKING_TYPE");

            strQry.AppendLine(" FROM ShoutItNowTest.dbo.EMPLOYEE_CLOCKINGS ");

            strQry.AppendLine(" WHERE ISNULL(USED_IND,0) = 0");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Clockings");

            for (int intCount = 0; intCount < DataSet.Tables["Clockings"].Rows.Count; intCount++)
            {
                strQry.Clear();

                intTimeMinutes = (60 * Convert.ToDateTime(DataSet.Tables["Clockings"].Rows[intCount]["CLOCKING_TIME"]).Hour) + Convert.ToDateTime(DataSet.Tables["Clockings"].Rows[intCount]["CLOCKING_TIME"]).Minute;

                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.DEVICE_CLOCK_TIME");
                strQry.AppendLine("(DEVICE_NO");
                strQry.AppendLine(",COMPANY_NO");
                strQry.AppendLine(",EMPLOYEE_NO");
                strQry.AppendLine(",PAY_CATEGORY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",TIMESHEET_DATE");
                strQry.AppendLine(",TIMESHEET_TIME_MINUTES");
                strQry.AppendLine(",CLOCKED_BOUNDARY_TIME_MINUTES");
                strQry.AppendLine(",IN_OUT_IND)");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" 0 ");
                strQry.AppendLine(",EPC.COMPANY_NO");
                strQry.AppendLine(",EPC.EMPLOYEE_NO");
                strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
                strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

                //TIMESHEET_DATE
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(Convert.ToDateTime(DataSet.Tables["Clockings"].Rows[intCount]["CLOCKING_TIME"]).ToString("yyyy-MM-dd")));
                //TIMESHEET_TIME_MINUTES
                strQry.AppendLine("," + intTimeMinutes);

                //CLOCKED_BOUNDARY_TIME_MINUTES Same as TIMESHEET_TIME_MINUTES
                strQry.AppendLine("," + intTimeMinutes);
                
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["Clockings"].Rows[intCount]["CLOCKING_TYPE"].ToString()));
                
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E ");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                
                strQry.AppendLine(" LEFT JOIN InteractPayrollClient.dbo.DEVICE_CLOCK_TIME DCT");
                strQry.AppendLine(" ON DCT.DEVICE_NO = 0");
                strQry.AppendLine(" AND DCT.COMPANY_NO = EPC.COMPANY_NO");
                strQry.AppendLine(" AND DCT.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
                strQry.AppendLine(" AND DCT.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND DCT.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND DCT.TIMESHEET_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(Convert.ToDateTime(DataSet.Tables["Clockings"].Rows[intCount]["CLOCKING_TIME"]).ToString("yyyy-MM-dd")));
                strQry.AppendLine(" AND DCT.TIMESHEET_TIME_MINUTES = " + intTimeMinutes);
                strQry.AppendLine(" AND DCT.IN_OUT_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["Clockings"].Rows[intCount]["CLOCKING_TYPE"].ToString()));
                
                strQry.AppendLine(" WHERE E.EMPLOYEE_3RD_PARTY_CODE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["Clockings"].Rows[intCount]["EMPLOYEE_NO"].ToString()));
                //No Record Exists
                strQry.AppendLine(" AND DCT.COMPANY_NO IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                
                strQry.Clear();
                
                strQry.AppendLine(" UPDATE ShoutItNowTest.dbo.EMPLOYEE_CLOCKINGS ");

                strQry.AppendLine(" SET USED_IND = 1");

                strQry.AppendLine(" WHERE EMPLOYEE_NO = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["Clockings"].Rows[intCount]["EMPLOYEE_NO"].ToString()));
                strQry.AppendLine(" AND CLOCKING_TIME = " + clsDBConnectionObjects.Text2DynamicSQL(Convert.ToDateTime(DataSet.Tables["Clockings"].Rows[intCount]["CLOCKING_TIME"]).ToString("yyyy-MM-dd HH:mm:ss.fff")));
                strQry.AppendLine(" AND CLOCKING_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["Clockings"].Rows[intCount]["CLOCKING_TYPE"].ToString()));

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
    }
}
