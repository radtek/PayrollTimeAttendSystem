using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using InteractPayroll;

namespace InteractPayrollClient
{
    public class busTimeAttendanceMain
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busTimeAttendanceMain()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records()
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            byte[] bytCompress;
            bool blnInsert = false;

            int intYear = DateTime.Now.Year;

            if (DateTime.Now.Month < 3)
            {
                intYear = intYear - 1;
            }

            DateTime  dtDateTimeStart = new DateTime(intYear, 3, 1);
            DateTime  dtDateTimeEnd = new DateTime(intYear + 1, 3, 1).AddDays(-1);

            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FINGERPRINT_SOFTWARE_IND");
           
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.FINGERPRINT_SOFTWARE_TO_USE ");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Software");
           
            if (DataSet.Tables["Software"].Rows.Count == 0)
            {
                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.FINGERPRINT_SOFTWARE_TO_USE");
                strQry.AppendLine("(FINGERPRINT_SOFTWARE_IND)");
                strQry.AppendLine(" VALUES");
                strQry.AppendLine("('D')");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" IDENTIFY_THRESHOLD_VALUE");
            strQry.AppendLine(",VERIFY_THRESHOLD_VALUE");
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.FINGERPRINT_IDENTIFY_VERIFY_THRESHOLD ");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "IdentifyVerify");

            if (DataSet.Tables["IdentifyVerify"].Rows.Count == 0)
            {
                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.FINGERPRINT_IDENTIFY_VERIFY_THRESHOLD");
                strQry.AppendLine("(IDENTIFY_THRESHOLD_VALUE");
                strQry.AppendLine(",VERIFY_THRESHOLD_VALUE)");
                strQry.AppendLine(" VALUES");
                strQry.AppendLine("(45");
                strQry.AppendLine(",25)");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" READ_OPTION_NO");
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.READ_OPTION ");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "ReadOption");

            if (DataSet.Tables["ReadOption"].Rows.Count == 0)
            {
                //New Database Created
                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.READ_OPTION ");
                strQry.AppendLine("(READ_OPTION_NO");
                strQry.AppendLine(",READ_OPTION_DESC) ");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine("(0");
                strQry.AppendLine(",'None')");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.READ_OPTION ");
                strQry.AppendLine("(READ_OPTION_NO");
                strQry.AppendLine(",READ_OPTION_DESC) ");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine("(1");
                strQry.AppendLine(",'FingerPrint')");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.READ_OPTION ");
                strQry.AppendLine("(READ_OPTION_NO");
                strQry.AppendLine(",READ_OPTION_DESC) ");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine("(2");
                strQry.AppendLine(",'RFID Card Only')");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();
                strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.READ_OPTION ");
                strQry.AppendLine("(READ_OPTION_NO");
                strQry.AppendLine(",READ_OPTION_DESC) ");
                strQry.AppendLine(" VALUES ");
                strQry.AppendLine("(3");
                strQry.AppendLine(",'RFID Card And FingerPrint')");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
            }
   
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" MAX(DAY_DATE) AS MAX_DATE");
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.DATES ");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Temp");

            if (DataSet.Tables["Temp"].Rows[0]["MAX_DATE"] == System.DBNull.Value)
            {
                blnInsert = true;
            }
            else
            {
                if (Convert.ToDateTime(DataSet.Tables["Temp"].Rows[0]["MAX_DATE"]) < DateTime.Now)
                {
                    blnInsert = true;
                    dtDateTimeStart = new DateTime(Convert.ToDateTime(DataSet.Tables["Temp"].Rows[0]["MAX_DATE"]).Year, Convert.ToDateTime(DataSet.Tables["Temp"].Rows[0]["MAX_DATE"]).Month, Convert.ToDateTime(DataSet.Tables["Temp"].Rows[0]["MAX_DATE"]).Day).AddDays(1);
                }
                else
                {
                    if (DateTime.Now.Month == 1
                        | DateTime.Now.Month == 2)
                    {
                        DateTime newDateTimeCheck = new DateTime(DateTime.Now.Year, 3, 1).AddDays(-1);

                        if (newDateTimeCheck.ToString("yyyyMMdd") == Convert.ToDateTime(DataSet.Tables["Temp"].Rows[0]["MAX_DATE"]).ToString("yyyyMMdd"))
                        {
                            blnInsert = true;
                            dtDateTimeStart = dtDateTimeStart.AddYears(1);
                            dtDateTimeEnd = dtDateTimeEnd.AddYears(1);
                        }
                    }
                }
            }

            if (blnInsert == true)
            {
                while (true)
                {
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.DATES");
                    strQry.AppendLine("(DAY_DATE");
                    strQry.AppendLine(",DAY_NO)");
                    strQry.AppendLine(" VALUES ");
                    strQry.AppendLine("('" + dtDateTimeStart.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine("," + Convert.ToInt32(dtDateTimeStart.DayOfWeek) + ")");

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    if (dtDateTimeStart.Year == dtDateTimeEnd.Year
                        & dtDateTimeStart.Month == dtDateTimeEnd.Month
                        & dtDateTimeStart.Day == dtDateTimeEnd.Day)
                    {
                        break;
                    }

                    dtDateTimeStart = dtDateTimeStart.AddDays(1);
                }

            }

            DataSet = null;
            DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" FINGERPRINT_SOFTWARE_IND");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.FINGERPRINT_SOFTWARE_TO_USE");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "SoftwareToUse");
           
            bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_Local_Current_Download_Files(byte[] parbyteDataSet)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            DataSet ClientDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            DataView ClientDataSetDataview = new DataView(ClientDataSet.Tables["Files"]
                    , ""
                    , "FILE_LAYER_IND,FILE_NAME"
                    , DataViewRowState.CurrentRows);

            int intFindRow = -1;
            object[] objFind = new object[2];

            DataSet DataSet = new DataSet();

            strQry.Append(clsDBConnectionObjects.Get_Local_Client_Download_SQL());

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Files");

            for (int intRow = 0; intRow < DataSet.Tables["Files"].Rows.Count; intRow++)
            {
                objFind[0] = DataSet.Tables["Files"].Rows[intRow]["FILE_LAYER_IND"].ToString();
                objFind[1] = DataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();

                intFindRow = ClientDataSetDataview.Find(objFind);

                if (intFindRow == -1)
                {
                    //DELETE Records on InteractPayrollClient IF Not in Internet Download DataSet
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_CHUNKS");

                    strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["Files"].Rows[intRow]["FILE_LAYER_IND"].ToString()));
                    strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString()));

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.FILE_CLIENT_DOWNLOAD_DETAILS");

                    strQry.AppendLine(" WHERE FILE_LAYER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["Files"].Rows[intRow]["FILE_LAYER_IND"].ToString()));
                    strQry.AppendLine(" AND FILE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString()));

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                    DataSet.Tables["Files"].Rows.RemoveAt(intRow);

                    intRow -= 1;
                }
            }

            DataSet.AcceptChanges();

            DataView DataSetDataview = new DataView(DataSet.Tables["Files"]
                    , ""
                    , "FILE_LAYER_IND,FILE_NAME"
                    , DataViewRowState.CurrentRows);

            //Go Through Internet Download to see If Any Files Must be Downloaded
            for (int intRow = 0; intRow < ClientDataSet.Tables["Files"].Rows.Count; intRow++)
            {
                objFind[0] = ClientDataSet.Tables["Files"].Rows[intRow]["FILE_LAYER_IND"].ToString();
                objFind[1] = ClientDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();

                intFindRow = DataSetDataview.Find(objFind);

                if (intFindRow > -1)
                {
                    if (Convert.ToDateTime(ClientDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]) == Convert.ToDateTime(DataSetDataview[intFindRow]["FILE_LAST_UPDATED_DATE"]))
                    {
                        ClientDataSet.Tables["Files"].Rows.RemoveAt(intRow);

                        intRow -= 1;
                    }
                }
            }

            ClientDataSet.AcceptChanges();

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(ClientDataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
    }
}
