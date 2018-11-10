using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using InteractPayroll;

namespace InteractPayrollClient
{
    public class busClock
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busClock()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(int parintCurrentUserNo, string parstrAccessInd)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = new DataSet();

            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" C.COMPANY_NO");
            strQry.AppendLine(",C.COMPANY_DESC");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.COMPANY C ");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS  UC ");
                strQry.AppendLine(" ON UC.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND UC.COMPANY_NO = C.COMPANY_NO ");
            }

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" C.COMPANY_DESC");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Company");

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" D.DEVICE_NO");
            strQry.AppendLine(",D.DEVICE_DESC");
            strQry.AppendLine(",D.DEVICE_USAGE");
            strQry.AppendLine(",D.TIME_ATTEND_CLOCK_FIRST_LAST_IND");
            strQry.AppendLine(",D.CLOCK_IN_OUT_PARM");
            strQry.AppendLine(",D.CLOCK_IN_RANGE_FROM");
            strQry.AppendLine(",D.CLOCK_IN_RANGE_TO");
            strQry.AppendLine(",D.LOCK_OUT_MINUTES");
            strQry.AppendLine(",D.COMPANY_NO");
            strQry.AppendLine(",ISNULL(D.LAN_WAN_IND,'L') AS LAN_WAN_IND");
            strQry.AppendLine(",FAR_REQUESTED");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE D");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS  UC ");
                strQry.AppendLine(" ON UC.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND UC.COMPANY_NO = D.COMPANY_NO ");
            }

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" D.DEVICE_DESC");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Clock");

            for (int intRow = 0; intRow < DataSet.Tables["Clock"].Rows.Count; intRow++)
            {
                if (DataSet.Tables["Clock"].Rows[intRow]["FAR_REQUESTED"] == System.DBNull.Value)
                {
                    // 1/10 000
                    DataSet.Tables["Clock"].Rows[intRow]["FAR_REQUESTED"] = 214748;

                    strQry.Clear();
                    strQry.AppendLine(" UPDATE DEVICE");
                    strQry.AppendLine(" SET FAR_REQUESTED = 214748");
                    strQry.AppendLine(" WHERE DEVICE_NO = " + DataSet.Tables["Clock"].Rows[intRow]["DEVICE_NO"].ToString());

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }
            }

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public int Insert_Record(byte[] byteCompressedDataSet)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(byteCompressedDataSet);

            StringBuilder strQry = new StringBuilder();
            int intDeviceNo = 0;

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" MAX(DEVICE_NO) AS MAX_NO");
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Temp");

            if (DataSet.Tables["Temp"].Rows[0].IsNull("MAX_NO") == true)
            {
                intDeviceNo = 1;
            }
            else
            {
                intDeviceNo = Convert.ToInt32(DataSet.Tables["Temp"].Rows[0]["MAX_NO"]) + 1;
            }

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.DEVICE");
            strQry.AppendLine("(DEVICE_NO");
            strQry.AppendLine(",DEVICE_DESC");
            strQry.AppendLine(",DEVICE_USAGE");
            strQry.AppendLine(",TIME_ATTEND_CLOCK_FIRST_LAST_IND");
            strQry.AppendLine(",LOCK_OUT_MINUTES");
            strQry.AppendLine(",CLOCK_IN_OUT_PARM");
            strQry.AppendLine(",CLOCK_IN_RANGE_FROM");
            strQry.AppendLine(",CLOCK_IN_RANGE_TO");
            strQry.AppendLine(",COMPANY_NO");
            strQry.AppendLine(",LAN_WAN_IND");
            strQry.AppendLine(",FAR_REQUESTED)");

            strQry.AppendLine(" VALUES");
       
            strQry.AppendLine("(" + intDeviceNo);
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["Clock"].Rows[0]["DEVICE_DESC"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["Clock"].Rows[0]["DEVICE_USAGE"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["Clock"].Rows[0]["TIME_ATTEND_CLOCK_FIRST_LAST_IND"].ToString()));
            strQry.AppendLine("," + DataSet.Tables["Clock"].Rows[0]["LOCK_OUT_MINUTES"].ToString());
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["Clock"].Rows[0]["CLOCK_IN_OUT_PARM"].ToString()));
            strQry.AppendLine("," + DataSet.Tables["Clock"].Rows[0]["CLOCK_IN_RANGE_FROM"].ToString());
            strQry.AppendLine("," + DataSet.Tables["Clock"].Rows[0]["CLOCK_IN_RANGE_TO"].ToString());
            strQry.AppendLine("," + DataSet.Tables["Clock"].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["Clock"].Rows[0]["LAN_WAN_IND"].ToString()));
            strQry.AppendLine("," + DataSet.Tables["Clock"].Rows[0]["FAR_REQUESTED"].ToString() + ")");
            
            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            return intDeviceNo;
        }

        public void Update_Record(byte[] byteCompressedDataSet)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(byteCompressedDataSet);
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" UPDATE DEVICE");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" CLOCK_IN_OUT_PARM = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["Clock"].Rows[0]["CLOCK_IN_OUT_PARM"].ToString()));
            strQry.AppendLine(",CLOCK_IN_RANGE_FROM = " + DataSet.Tables["Clock"].Rows[0]["CLOCK_IN_RANGE_FROM"].ToString());
            strQry.AppendLine(",CLOCK_IN_RANGE_TO = " + DataSet.Tables["Clock"].Rows[0]["CLOCK_IN_RANGE_TO"].ToString());
            strQry.AppendLine(",DEVICE_USAGE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["Clock"].Rows[0]["DEVICE_USAGE"].ToString()));
            strQry.AppendLine(",TIME_ATTEND_CLOCK_FIRST_LAST_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["Clock"].Rows[0]["CLOCK_IN_OUT_PARM"].ToString()));
            strQry.AppendLine(",LOCK_OUT_MINUTES = " + DataSet.Tables["Clock"].Rows[0]["LOCK_OUT_MINUTES"].ToString());
            strQry.AppendLine(",DEVICE_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["Clock"].Rows[0]["DEVICE_DESC"].ToString()));
            strQry.AppendLine(",COMPANY_NO = " + DataSet.Tables["Clock"].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine(",LAN_WAN_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["Clock"].Rows[0]["LAN_WAN_IND"].ToString()));
            strQry.AppendLine(",FAR_REQUESTED = " + DataSet.Tables["Clock"].Rows[0]["FAR_REQUESTED"].ToString());
            
            strQry.AppendLine(" WHERE DEVICE_NO = " + DataSet.Tables["Clock"].Rows[0]["DEVICE_NO"].ToString());

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
        }

        public void Delete_Record(int parintDeviceNo)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEVICE");
            strQry.AppendLine(" WHERE DEVICE_NO = " + parintDeviceNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEVICE_CLOCK_TIME");
            strQry.AppendLine(" WHERE DEVICE_NO = " + parintDeviceNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK");
            strQry.AppendLine(" WHERE DEVICE_NO = " + parintDeviceNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK_ACTIVE");
            strQry.AppendLine(" WHERE DEVICE_NO = " + parintDeviceNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            
            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEVICE_EMPLOYEE_LINK");
            strQry.AppendLine(" WHERE DEVICE_NO = " + parintDeviceNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK");
            strQry.AppendLine(" WHERE DEVICE_NO = " + parintDeviceNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK_ACTIVE");
            strQry.AppendLine(" WHERE DEVICE_NO = " + parintDeviceNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK");
            strQry.AppendLine(" WHERE DEVICE_NO = " + parintDeviceNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            strQry.Clear();
            strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK_ACTIVE");
            strQry.AppendLine(" WHERE DEVICE_NO = " + parintDeviceNo);

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
        }
    }
}
