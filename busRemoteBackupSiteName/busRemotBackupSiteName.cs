using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using InteractPayroll;

namespace InteractPayrollClient
{
    public class busRemotBackupSiteName
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busRemotBackupSiteName()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }
        
        public byte[] Get_Form_Records()
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" SITE_NAME");
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.REMOTE_BACKUP_SITE_NAME");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "RemoteBackupSiteName");

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public void Update_Record(string parstrRemoteBackupSiteName)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            if (parstrRemoteBackupSiteName == "")
            {
                strQry.Clear();
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.REMOTE_BACKUP_SITE_NAME");
            }
            else
            {
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" SITE_NAME");
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.REMOTE_BACKUP_SITE_NAME");

                clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "RemoteBackupSiteName");

                if (DataSet.Tables["RemoteBackupSiteName"].Rows.Count > 0)
                {
                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayrollClient.dbo.REMOTE_BACKUP_SITE_NAME");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" SITE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parstrRemoteBackupSiteName));
                }
                else
                {
                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.REMOTE_BACKUP_SITE_NAME");
                    strQry.AppendLine(" (SITE_NAME)");
                    strQry.AppendLine(" VALUES ");
                    strQry.AppendLine("(" + clsDBConnectionObjects.Text2DynamicSQL(parstrRemoteBackupSiteName) + ")");
                }
            }

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            DataSet.Dispose();
            DataSet = null;
        }
    }
}
