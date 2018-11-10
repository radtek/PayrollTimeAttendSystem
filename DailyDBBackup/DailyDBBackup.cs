using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using InteractPayroll;
using System.IO;
using System.Security.Cryptography;
using System.Net;
using System.Net.Mail;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.S3.Model;
using System.Configuration;

namespace DailyDBBackup
{
    public partial class DailyDBBackup : ServiceBase
    {
        Timer tmrTimer;
        clsDBConnectionObjects clsDBConnectionObjects;
        IAmazonS3 client;

        string pvtstrBucketName = "";
        string pvtstrLogFileDirectory = "";

        int intHour = 0;
        int intMinute = 0;
        
        public DailyDBBackup()
        {
            InitializeComponent();

            pvtstrLogFileDirectory = ConfigurationManager.AppSettings["LogFileDirectory"];
            string strTime = ConfigurationManager.AppSettings["DailyBackupTime"];
            string[] strTimeParts = strTime.Split(':');

            intHour = Convert.ToInt32(strTimeParts[0]);
            intMinute = Convert.ToInt32(strTimeParts[1]);
            
            tmrTimer = new Timer();
            tmrTimer.Elapsed += TimerEvent;
        }

        protected double Calculate_Next_Time_Timer_Fires(bool blnBackupCompletedSuccessful)
        {
            double tickTime = 0;

            if (blnBackupCompletedSuccessful == true)
            {
                //Set To Fire Next Day - 12:15 Am
                DateTime dtNextTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, intHour, intMinute, 0);

                if (DateTime.Now > dtNextTime)
                {
                    dtNextTime = dtNextTime.AddDays(1);
                }

                tickTime = (double)(dtNextTime - DateTime.Now).TotalMilliseconds;
            }
            else
            {
                DateTime dtNextTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.AddHours(1).Hour, 15, 0);
                
                tickTime = (double)(dtNextTime - DateTime.Now).TotalMilliseconds;
            }

            return tickTime;
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(pvtstrLogFileDirectory + "DailyDBBackup_Info.txt", true))
                {
                    file.WriteLine("OnStart ... " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                double tickTime = Calculate_Next_Time_Timer_Fires(false);

                tmrTimer.Interval = tickTime;
                tmrTimer.Start();
            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(pvtstrLogFileDirectory + "DailyDBBackup_Error.txt", true))
                {
                    file.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Error = " + ex.Message);
                }
            }
        }

        protected override void OnStop()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(pvtstrLogFileDirectory + "DailyDBBackup_Info.txt", true))
            {
                file.WriteLine("OnStop ... " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

        private void TimerEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            //Stop Timer
            tmrTimer.Stop();

            RunBackupJob();
        }

        public bool FindFile(string fileName)
        {
            try
            {
                var objectmeta =  new GetObjectMetadataRequest();
                objectmeta.BucketName = pvtstrBucketName;
                objectmeta.Key = fileName;

                var response = client.GetObjectMetadata(objectmeta);

                return true;
            }

            catch (Amazon.S3.AmazonS3Exception ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return false;
                }

                //Throw Exception if status wasn't not found
                throw;
            }
        }

        public void RunBackupJob()
        {
            bool blnBackupCompletedSuccessful = false;
            string strEmail = "";
            string strEmailPassword = "";

            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(pvtstrLogFileDirectory + "DailyDBBackup_Info.txt", true))
                {
                    file.WriteLine("RunBackupJob ... " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                clsDBConnectionObjects = null;
                clsDBConnectionObjects = new InteractPayroll.clsDBConnectionObjects();

                DataSet DataSet = new System.Data.DataSet();

                StringBuilder strQry = new StringBuilder();

                strQry.Clear();

                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" BACKUP_DATABASE_PATH");
                strQry.AppendLine(",BACKUP_GMAIL_ACCOUNT");
                strQry.AppendLine(",BACKUP_GMAIL_PASSWORD");
                strQry.AppendLine(",BACKUP_S3_BUCKET_NAME");
                strQry.AppendLine(",BACKUP_DB_IND");
                                
                strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_PATH");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Directory", -1);

                string strFileDirectory = DataSet.Tables["Directory"].Rows[0]["BACKUP_DATABASE_PATH"].ToString();
                strEmail = DataSet.Tables["Directory"].Rows[0]["BACKUP_GMAIL_ACCOUNT"].ToString();
                strEmailPassword = DataSet.Tables["Directory"].Rows[0]["BACKUP_GMAIL_PASSWORD"].ToString();
                pvtstrBucketName = DataSet.Tables["Directory"].Rows[0]["BACKUP_S3_BUCKET_NAME"].ToString();
                
                string strDatabaseName = "InteractPayroll";
                string strBackupFileName = strDatabaseName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_Daily.bak";

                //Move Files Offshore
                client = new AmazonS3Client();

                string[] fileInteractPayrollPaths;

                FileInfo fileinfo;
                bool blnFound = false;

                if (DataSet.Tables["Directory"].Rows[0]["BACKUP_DB_IND"].ToString() == "Y"
                || DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
                {
                    //Backup InteractPayroll Master
                    fileInteractPayrollPaths = Directory.GetFiles(@strFileDirectory, @"*InteractPayroll_" + DateTime.Now.ToString("yyyyMMdd") + "_*.bak");

                    if (fileInteractPayrollPaths.Length == 0)
                    {
                        strQry.Clear();

                        strQry.AppendLine("BACKUP DATABASE " + strDatabaseName + " TO DISK = '" + strFileDirectory + "\\" + strBackupFileName + "'");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                    }

                    strQry.Clear();

                    strQry.AppendLine(" UPDATE InteractPayroll.dbo.BACKUP_DATABASE_PATH");
                    strQry.AppendLine(" SET BACKUP_DB_IND = 'N'");
                    
                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                }

                fileInteractPayrollPaths = Directory.GetFiles(@strFileDirectory, @"*InteractPayroll_" + DateTime.Now.ToString("yyyyMMdd") + "_*.bak");

                if (fileInteractPayrollPaths.Length > 0)
                {
                    fileinfo = new FileInfo(fileInteractPayrollPaths[0]);

                    blnFound = FindFile(fileinfo.Name);

                    if (blnFound == false)
                    {
#if(DEBUG)
                        //Don't Want To Write Test DBs to Backup Bucket
#else
                        TransferUtilityUploadRequest fileTransferUtilityRequest = new TransferUtilityUploadRequest
                        {
                            BucketName = pvtstrBucketName,
                            FilePath = fileInteractPayrollPaths[0]
                        };

                        fileTransferUtilityRequest.Metadata.Add("fileName", fileinfo.Name);
                        fileTransferUtilityRequest.Metadata.Add("fileDesc", fileinfo.Name);

                        TransferUtility fileTransferUtility = new TransferUtility(client);

                        fileTransferUtility.Upload(fileTransferUtilityRequest);
#endif
                    }
                }

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" COMPANY_NO ");

                strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK");

                strQry.AppendLine(" WHERE BACKUP_DB_IND = 1");
                
                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "BackUpInfo", -1);

                //First Complete Todays Backups
                for (int intRow = 0; intRow < DataSet.Tables["BackUpInfo"].Rows.Count; intRow++)
                {
                    strDatabaseName = "InteractPayroll_" + Convert.ToInt32(DataSet.Tables["BackUpInfo"].Rows[intRow]["COMPANY_NO"]).ToString("00000");

                    strBackupFileName = strDatabaseName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_Daily.bak";

                    strQry.Clear();

                    strQry.AppendLine("BACKUP DATABASE " + strDatabaseName + " TO DISK = '" + strFileDirectory + "\\" + strBackupFileName + "'");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    strQry.Clear();

                    strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
                    strQry.AppendLine(" SET BACKUP_DB_IND = 0");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + DataSet.Tables["BackUpInfo"].Rows[intRow]["COMPANY_NO"].ToString());

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                    string[] fileOldDBPaths = Directory.GetFiles(strFileDirectory,strDatabaseName + "_*_Daily.bak");

                    if (fileOldDBPaths.Length > 5)
                    {
                        //Delete Old
                        Array.Sort(fileOldDBPaths);

                        for (int intFileRow = 0; intFileRow < fileOldDBPaths.Length - 5; intFileRow++)
                        {
                            File.Delete(fileOldDBPaths[intFileRow]);
                        }
                    }
                }

                //Complete Todays Offsite DB Backups
                string[] fileTodaysPaths = Directory.GetFiles(@strFileDirectory, @"*_" + DateTime.Now.ToString("yyyyMMdd") + "*.bak");

                if (fileTodaysPaths.Length > 0)
                {
                    for (int intFileCount = 0; intFileCount < fileTodaysPaths.Length; intFileCount++)
                    {
                        fileinfo = new FileInfo(fileTodaysPaths[intFileCount]);

                        blnFound = FindFile(fileinfo.Name);

                        if (blnFound == false)
                        {
#if(DEBUG)
                            //Don't Want To Write Test DBs to Backup Bucket
#else
                            TransferUtilityUploadRequest fileTransferUtilityRequest = new TransferUtilityUploadRequest
                            {
                                BucketName = pvtstrBucketName,
                                FilePath = fileTodaysPaths[intFileCount]
                            };

                            //LifecycleConfiguration a = new LifecycleConfiguration();

                         
                            fileTransferUtilityRequest.Metadata.Add("fileName", fileinfo.Name);
                            fileTransferUtilityRequest.Metadata.Add("fileDesc", fileinfo.Name);
                            /////fileTransferUtilityRequest.Metadata.Add("expires","Tue, 16 Feb 2016 16:00:00 GMT");

                            TransferUtility fileTransferUtility = new TransferUtility(client);

                            fileTransferUtility.Upload(fileTransferUtilityRequest);
#endif
                        }
                    }
                }

                //Check Yesterdays DB Backups
                string[] fileYesterdaysPaths = Directory.GetFiles(@strFileDirectory, @"*_" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + "*.bak");

                if (fileYesterdaysPaths.Length > 0)
                {
                    for (int intFileCount = 0; intFileCount < fileYesterdaysPaths.Length; intFileCount++)
                    {
                        fileinfo = new FileInfo(fileYesterdaysPaths[intFileCount]);

                        blnFound = FindFile(fileinfo.Name);

                        if (blnFound == false)
                        {
#if(DEBUG)
                            //Don't Want To Write Test DBs to Backup Bucket
#else
                            TransferUtilityUploadRequest fileTransferUtilityRequest = new TransferUtilityUploadRequest
                            {
                                BucketName = pvtstrBucketName,
                                FilePath = fileYesterdaysPaths[intFileCount]
                            };

                            fileTransferUtilityRequest.Metadata.Add("fileName", fileinfo.Name);
                            fileTransferUtilityRequest.Metadata.Add("fileDesc", fileinfo.Name);

                            TransferUtility fileTransferUtility = new TransferUtility(client);

                            fileTransferUtility.Upload(fileTransferUtilityRequest);
#endif
                        }
                    }
                }

                bool blnFirstPass = true;
                string[] fileOldDaysPaths;

                RunBackupJob_Continue:

                if (blnFirstPass == true)
                {
                    //Clean Up Old Backups Older than 1 Month Old
                    fileOldDaysPaths = Directory.GetFiles(@strFileDirectory, @"*_" + DateTime.Now.AddMonths(-1).ToString("yyyyMM") + "*.bak");
                }
                else
                {
                    fileOldDaysPaths = Directory.GetFiles(@strFileDirectory, @"*_" + DateTime.Now.AddMonths(-2).ToString("yyyyMM") + "*.bak");
                }

                foreach (string file in fileOldDaysPaths)
                {
                    int intOffset = file.IndexOf("InteractPayroll_");

                    if (intOffset > -1)
                    {
                        if (file.Length > intOffset + 30)
                        {
                            try
                            {
                                //Client Databases
                                DateTime myFileDateTime = DateTime.ParseExact(file.Substring(intOffset + 22, 8), "yyyyMMdd", null).AddMonths(1);

                                if (myFileDateTime < DateTime.Now)
                                {
                                    File.Delete(file);
                                }
                            }
                            catch
                            {
                                //InteractPayroll Master

                                try
                                {
                                    DateTime myFileDateTime = DateTime.ParseExact(file.Substring(intOffset + 16, 8), "yyyyMMdd", null).AddMonths(1);

                                    if (myFileDateTime < DateTime.Now)
                                    {
                                        File.Delete(file);
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }

                if (blnFirstPass == true)
                {
                    blnFirstPass = false;

                    goto RunBackupJob_Continue;
                }

                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.BACKUP_DATABASE_PATH");
                strQry.AppendLine(" SET BACKUP_LAST_DATETIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                        
                blnBackupCompletedSuccessful = true;
            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(pvtstrLogFileDirectory + "DailyDBBackup_Error.txt", true))
                {
                    file.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Error = " + ex.Message);
                }
            }
            finally
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(pvtstrLogFileDirectory + "DailyDBBackup_Info.txt", true))
                {
                    file.WriteLine("RunBackupJob Setting Timer ... " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                //Set Next Time for Timer to Fire
                double tickTime = Calculate_Next_Time_Timer_Fires(blnBackupCompletedSuccessful);
                tmrTimer.Interval = tickTime;
                tmrTimer.Start();

                if (client != null)
                {
                    client.Dispose();
                    client = null;
                }

                if (blnBackupCompletedSuccessful == true
                || DateTime.Now.Hour == 7)
                {
                    var smtp = new SmtpClient();

                    try
                    {
                        //Email
                        var fromAddress = new MailAddress(strEmail, "DB Backup Server");
                        var toAddress = new MailAddress(strEmail, "Errol Le Roux");

                        string subject = "Daily DB Backups - " + DateTime.Now.ToString("dd MMMM yyyy");
                        string body = "Backup Successful";

                        if (blnBackupCompletedSuccessful == false)
                        {
                            body = "Backup UNSUCCESSFUL";
                        }
                        
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(fromAddress.Address, strEmailPassword);

                        var message = new MailMessage(fromAddress, toAddress);

                        message.Subject = subject;
                        message.Body = body;

                        smtp.Send(message);

                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(pvtstrLogFileDirectory + "DailyDBBackup_Info.txt", true))
                        {
                            file.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Email Sent Successfully");
                        }
                    }
                    catch(Exception ex)
                    {
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(pvtstrLogFileDirectory + "DailyDBBackup_Error.txt", true))
                        {
                            file.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Email Error = " + ex.Message);
                        }
                    }
                    finally
                    {
                        smtp.Dispose();
                        smtp = null;
                    }
                }
            }
        }
    }
}
