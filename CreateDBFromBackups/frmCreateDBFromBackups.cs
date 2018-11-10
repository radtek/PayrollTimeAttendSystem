using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InteractPayroll;
using System.IO;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.S3.Model;

namespace CreateDBFromBackups
{
    public partial class frmCreateDBFromBackups : Form
    {
        clsDBConnectionObjects clsDBConnectionObjects;
        clsISUtilities clsISUtilities;
        IAmazonS3 iAmazonS3;

        FileInfo fileinfo;
        StringBuilder strQry;
        DataSet pvtDataSet;
        DataView pvtLocalBackupFilesDataView;

        string pvtstrFileDirectory = "";
        string pvtstrEmail = "";
        string pvtstrEmailPassword = "";
        string pvtstrBucketName = "";

        public frmCreateDBFromBackups()
        {
            InitializeComponent();
        }

        private void frmCreateDBFromBackups_Load(object sender, EventArgs e)
        {
            try
            {
                clsDBConnectionObjects = new InteractPayroll.clsDBConnectionObjects();
                clsISUtilities = new clsISUtilities();
                iAmazonS3 = new AmazonS3Client();

                this.lblFilesHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                
                pvtDataSet = new System.Data.DataSet();

                strQry = new StringBuilder();

                strQry.Clear();

                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" BACKUP_DATABASE_PATH");
                strQry.AppendLine(",BACKUP_GMAIL_ACCOUNT");
                strQry.AppendLine(",BACKUP_GMAIL_PASSWORD");
                strQry.AppendLine(",BACKUP_S3_BUCKET_NAME");

                strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_PATH");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), pvtDataSet, "Directory", -1);

                pvtstrFileDirectory = pvtDataSet.Tables["Directory"].Rows[0]["BACKUP_DATABASE_PATH"].ToString();
                pvtstrEmail = pvtDataSet.Tables["Directory"].Rows[0]["BACKUP_GMAIL_ACCOUNT"].ToString();
                pvtstrEmailPassword = pvtDataSet.Tables["Directory"].Rows[0]["BACKUP_GMAIL_PASSWORD"].ToString();
                pvtstrBucketName = pvtDataSet.Tables["Directory"].Rows[0]["BACKUP_S3_BUCKET_NAME"].ToString();

                strQry.Clear();

                //Create Table For Compare to Remote Files (via Dataview) 
                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" BACKUP_DATABASE_PATH AS LOCAL_FILE");
                
                strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_PATH");

                strQry.AppendLine(" WHERE BACKUP_DATABASE_PATH = 'ZZZZ'");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), pvtDataSet, "LocalBackupFiles", -1);
                
                Get_Local_Backup_Files();
            }
            catch(Exception ex)
            {

            }
        }

        private void Get_Local_Backup_Files()
        {
            this.dgvFilesDataGridView.Rows.Clear();

            //Backup InteractPayroll Master
            string[] fileInteractPayrollFiles = Directory.GetFiles(@pvtstrFileDirectory, @"*InteractPayroll_*.bak");

            Array.Reverse(fileInteractPayrollFiles);

            string strCompanyNo = "";
            DateTime dtDateTime = DateTime.Now;

            pvtDataSet.Tables["LocalBackupFiles"].Clear();

            for (int intFileCount = 0; intFileCount < fileInteractPayrollFiles.Length; intFileCount++)
            {
                fileinfo = null;
                fileinfo = new FileInfo(fileInteractPayrollFiles[intFileCount]);

                strCompanyNo = "";

                try
                {
                    if (fileinfo.Name.Substring(15, 1) == "_"
                    && fileinfo.Name.Substring(21, 1) == "_")
                    {
                        strCompanyNo = fileinfo.Name.Substring(16, 5);
                        string strdate = fileinfo.Name.Substring(22, 8) + fileinfo.Name.Substring(31, 6);
                        dtDateTime = DateTime.ParseExact(strdate, "yyyyMMddHHmmss", null);
                    }
                    else
                    {
                        string strdate = fileinfo.Name.Substring(16, 8) + fileinfo.Name.Substring(25, 6);
                        dtDateTime = DateTime.ParseExact(strdate, "yyyyMMddHHmmss", null);
                    }

                    this.dgvFilesDataGridView.Rows.Add(dtDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                                       strCompanyNo,
                                                       fileinfo.Name);

                    pvtDataSet.Tables["LocalBackupFiles"].Rows.Add(fileinfo.Name);
                }
                catch
                {
                    //Backup Not From Program
                }
            }



        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rbnButton_Click(object sender, EventArgs e)
        {
            RadioButton myRadioButton = (RadioButton)sender;

            if (myRadioButton.Name == "rbnLocal")
            {
                this.btnOK.Text = "Restore";
                Get_Local_Backup_Files();
            }
            else
            {
                //Remote
                this.btnOK.Text = "Download";
                this.dgvFilesDataGridView.Rows.Clear();
                this.Refresh();

                string strCompanyNo = "";
                DateTime dtDateTime = DateTime.Now;

                pvtLocalBackupFilesDataView = null;
                pvtLocalBackupFilesDataView = new DataView(pvtDataSet.Tables["LocalBackupFiles"],
                                                           "",
                                                           "LOCAL_FILE",
                                                           DataViewRowState.CurrentRows);
                
                ListObjectsRequest listObjectsRequest = new Amazon.S3.Model.ListObjectsRequest();

                listObjectsRequest.BucketName = pvtstrBucketName;
                listObjectsRequest.Prefix = "InteractPayroll";

                ListObjectsResponse listObjectsResponse = iAmazonS3.ListObjects(listObjectsRequest);

                //Reverse Order of Downloaded Files
                listObjectsResponse.S3Objects.Reverse();

                foreach (S3Object s3Object in listObjectsResponse.S3Objects)
                {
                    int intFindRow = pvtLocalBackupFilesDataView.Find(s3Object.Key);

                    if (intFindRow > -1)
                    {
                        continue;
                    }

                    strCompanyNo = "";

                    try
                    {
                        if (s3Object.Key.Substring(15, 1) == "_"
                        && s3Object.Key.Substring(21, 1) == "_")
                        {
                            strCompanyNo = s3Object.Key.Substring(16, 5);
                            string strdate = s3Object.Key.Substring(22, 8) + s3Object.Key.Substring(31, 6);
                            dtDateTime = DateTime.ParseExact(strdate, "yyyyMMddHHmmss", null);
                        }
                        else
                        {
                            string strdate = s3Object.Key.Substring(16, 8) + s3Object.Key.Substring(25, 6);
                            dtDateTime = DateTime.ParseExact(strdate, "yyyyMMddHHmmss", null);
                        }

                        this.dgvFilesDataGridView.Rows.Add(dtDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                                           strCompanyNo,
                                                           s3Object.Key);
                    }
                    catch
                    {
                        //Backup Not From Program
                    }





                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.btnOK.Text == "Restore")
            {
                if (pvtDataSet.Tables["DataBaseName"] != null)
                {
                    pvtDataSet.Tables.Remove("DataBaseName");
                }

                if (pvtDataSet.Tables["Check"] != null)
                {
                    pvtDataSet.Tables.Remove("Check");
                }

                string strDatabaseName = "InteractPayroll";
                string strDatabaseNo = this.dgvFilesDataGridView[1, dgvFilesDataGridView.CurrentRow.Index].Value.ToString();

                if (strDatabaseNo != "")
                {
                    strDatabaseName = strDatabaseName + "_" + strDatabaseNo;
                }

                strQry.Clear();

                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" NAME");

                strQry.AppendLine(" FROM MASTER.dbo.SYSDATABASES ");
                strQry.AppendLine(" WHERE NAME = " + clsDBConnectionObjects.Text2DynamicSQL(strDatabaseName));

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), pvtDataSet, "DataBaseName", -1);

                if (pvtDataSet.Tables["DataBaseName"].Rows.Count == 0)
                {
                    DialogResult myDialogResult = MessageBox.Show("Database " + strDatabaseName + " needs to be Created.\n\nWould you like to Continue?", "Create DB", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                    if (myDialogResult == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }

                    strQry.Clear();
                    strQry.AppendLine(" CREATE DATABASE " + strDatabaseName + " COLLATE SQL_Latin1_General_CP1_CI_AS");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    string parstrFileName = pvtstrFileDirectory + "\\" + this.dgvFilesDataGridView[2, dgvFilesDataGridView.CurrentRow.Index].Value.ToString();

                    strQry.Clear();

                    strQry.AppendLine("RESTORE DATABASE " + strDatabaseName + " FROM DISK = '" + parstrFileName + "' WITH REPLACE");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" COMPANY_NO ");
                    strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(strDatabaseNo));
              
                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), pvtDataSet, "Check", -1);

                    if (pvtDataSet.Tables["Check"].Rows.Count == 0)
                    {
                        strQry.Clear();

                        strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.COMPANY_LINK");
                        strQry.AppendLine("(COMPANY_NO");
                        strQry.AppendLine(",COMPANY_DESC");
                        strQry.AppendLine(",DATE_FORMAT)");

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" COMPANY_NO ");
                        strQry.AppendLine(",COMPANY_DESC ");
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL("dd-MM-yyyy"));
                     
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY");

                        strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(strDatabaseNo));

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Convert.ToInt32(strDatabaseNo));
                    }
                }
            }
            else
            {
                this.Cursor = Cursors.WaitCursor;
                this.dgvFilesDataGridView.Cursor = Cursors.WaitCursor;
                this.Refresh();
                
                string strDownloadFile = this.dgvFilesDataGridView[2, dgvFilesDataGridView.CurrentRow.Index].Value.ToString();

                GetObjectRequest request = new GetObjectRequest();
                request.BucketName = pvtstrBucketName;
                request.Key = strDownloadFile;
                GetObjectResponse response = iAmazonS3.GetObject(request);
                response.WriteResponseStreamToFile(pvtstrFileDirectory + "\\" + strDownloadFile);

                this.dgvFilesDataGridView.Rows.Remove(dgvFilesDataGridView.CurrentRow);

                this.Cursor = Cursors.Default;
                this.Refresh();

                MessageBox.Show("File Download Successful");
            }
        }
    }
}
