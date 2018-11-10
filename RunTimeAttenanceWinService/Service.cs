using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Configuration;

namespace InteractPayroll
{
#if (DEBUG)
    public partial class Service : Form
#else
    public partial class Service : ServiceBase
#endif
    {
        clsDBConnectionObjects clsDBConnectionObjectsProcessRunQueue;
        clsDBConnectionObjects clsDBConnectionObjectsProcessCloseQueue;
        clsDBConnectionObjects clsDBConnectionObjectsProcessOpenRunQueue;
        clsDBConnectionObjects clsDBConnectionObjectsProcessEmailPayslipQueue;
        
        string pvtstrLogFileName = "";

        string pvtstrSmtpEmailAddressDescription = "";
        string pvtstrSmtpEmailAddress = "";
        string pvtstrSmtpEmailAddressPassword = "";
        string pvtstrSmtpHostPort = "";
        string pvtstrSmtpHostName = "";
        string pvtstrEmailFolder = "";
        string pvtstrSmtpSysAdminEmailAddressFirstName = "";
        string pvtstrSmtpSysAdminEmailAddressLastName = "";
        string pvtstrSmtpSysAdminEmailAddress = "";
        
        System.Timers.Timer tmrCheckToRunJobsTimer;
        System.Timers.Timer tmrCheckToRunCloseJobsTimer;
        System.Timers.Timer tmrCheckToOpenJobsTimer;
        System.Timers.Timer tmrCheckToEmailPayslipsJobsTimer;
        
        public Service()
        {
            InitializeComponent();

            pvtstrLogFileName = ConfigurationManager.AppSettings["LogFileName"];
            AppDomain.CurrentDomain.SetData("LogFileName", pvtstrLogFileName);
            
            pvtstrSmtpEmailAddressDescription = ConfigurationManager.AppSettings["SmtpEmailAddressDescription"];
            AppDomain.CurrentDomain.SetData("SmtpEmailAddressDescription", pvtstrSmtpEmailAddressDescription);

            pvtstrSmtpEmailAddress = ConfigurationManager.AppSettings["SmtpEmailAddress"];
            AppDomain.CurrentDomain.SetData("SmtpEmailAddress", pvtstrSmtpEmailAddress);

            pvtstrSmtpEmailAddressPassword = ConfigurationManager.AppSettings["SmtpEmailAddressPassword"];
            AppDomain.CurrentDomain.SetData("SmtpEmailAddressPassword", pvtstrSmtpEmailAddressPassword);
            
            pvtstrSmtpHostName = ConfigurationManager.AppSettings["SmtpHostName"];
            AppDomain.CurrentDomain.SetData("SmtpHostName", pvtstrSmtpHostName);
            
            pvtstrSmtpHostPort = ConfigurationManager.AppSettings["SmtpHostPort"];
            AppDomain.CurrentDomain.SetData("SmtpHostPort", pvtstrSmtpHostPort);

            pvtstrEmailFolder = ConfigurationManager.AppSettings["EmailFolder"];
            AppDomain.CurrentDomain.SetData("EmailFolder", pvtstrEmailFolder);

            pvtstrSmtpSysAdminEmailAddressFirstName = ConfigurationManager.AppSettings["SmtpSysAdminEmailAddressFirstName"];
            AppDomain.CurrentDomain.SetData("SmtpSysAdminEmailAddressFirstName", pvtstrSmtpSysAdminEmailAddressFirstName);

            pvtstrSmtpSysAdminEmailAddressLastName = ConfigurationManager.AppSettings["SmtpSysAdminEmailAddressLastName"];
            AppDomain.CurrentDomain.SetData("SmtpSysAdminEmailAddressLastName", pvtstrSmtpSysAdminEmailAddressLastName);

            pvtstrSmtpSysAdminEmailAddress = ConfigurationManager.AppSettings["SmtpSysAdminEmailAddress"];
            AppDomain.CurrentDomain.SetData("SmtpSysAdminEmailAddress", pvtstrSmtpSysAdminEmailAddress);
#if (DEBUG)
            //clsDBConnectionObjectsProcessOpenRunQueue = new clsDBConnectionObjects();
            //ProcessOpenRunQueue();

            //clsDBConnectionObjectsProcessCloseQueue = new clsDBConnectionObjects();
            //ProcessCloseQueue();

            //clsDBConnectionObjectsProcessRunQueue = new clsDBConnectionObjects();
            //ProcessRunQueue();

            clsDBConnectionObjectsProcessEmailPayslipQueue = new clsDBConnectionObjects();
            ProcessEmailPayslipQueue();
#endif
        }

#if (DEBUG)
#else
        protected override void OnStart(string[] args)
        {
            try
            {
                WriteLog("### RunTimeAttenanceWinService OnStart Entered ###");

                clsDBConnectionObjectsProcessRunQueue = new clsDBConnectionObjects();
                clsDBConnectionObjectsProcessCloseQueue = new clsDBConnectionObjects();
                clsDBConnectionObjectsProcessOpenRunQueue = new clsDBConnectionObjects();
                clsDBConnectionObjectsProcessEmailPayslipQueue = new clsDBConnectionObjects();

                tmrCheckToRunJobsTimer = new System.Timers.Timer();
                tmrCheckToRunJobsTimer.Elapsed += ProcessRunQueue_Elapsed;
                //Every Second
                tmrCheckToRunJobsTimer.Interval = 1000;

                tmrCheckToRunJobsTimer.Start();
                
                WriteLog("RunTimeAttenanceWinService tmrCheckToRunJobsTimer Started");

                tmrCheckToRunCloseJobsTimer = new System.Timers.Timer();
                tmrCheckToRunCloseJobsTimer.Elapsed += ProcessCloseRunQueue_Elapsed;
                //Every Second
                tmrCheckToRunCloseJobsTimer.Interval = 1000;

                tmrCheckToRunCloseJobsTimer.Start();

                WriteLog("RunTimeAttenanceWinService tmrCheckToRunCloseJobsTimer Started");

                tmrCheckToOpenJobsTimer = new System.Timers.Timer();
                tmrCheckToOpenJobsTimer.Elapsed += ProcessOpenRunQueue_Elapsed;
                //Every Second
                tmrCheckToOpenJobsTimer.Interval = 1000;

                tmrCheckToOpenJobsTimer.Start();

                WriteLog("RunTimeAttenanceWinService tmrCheckToOpenJobsTimer Started");

                tmrCheckToEmailPayslipsJobsTimer = new System.Timers.Timer();
                tmrCheckToEmailPayslipsJobsTimer.Elapsed += ProcessEmailPayslipQueue_Elapsed;
                //Every 2 Seconds
                tmrCheckToEmailPayslipsJobsTimer.Interval = 2000;

                tmrCheckToEmailPayslipsJobsTimer.Start();

                WriteLog("RunTimeAttenanceWinService tmrCheckToEmailPayslipsJobsTimer Started");
            }
            catch (Exception ex)
            {
                WriteExceptionErrorLog("@@@@@@@@@@@@@@@@@@@@ RunTimeAttenanceWinService OnStart FAILED @@@@@@@@@@@@@@@@@@@", ex);
            }
        }

        protected override void OnStop()
        {
            WriteLog("### RunTimeAttenanceWinService OnStop Entered ###");
        }
#endif
        private void ProcessRunQueueParallel(List<RunQueue> list)
        {
            ParallelOptions parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = 2;

            Parallel.ForEach(list, parallelOptions, runQueue =>
            {
                new clsProcessPayrollRunFromQueue().ProcessPayrollRunFromQueue(runQueue);
            });
        }
        
        public void ProcessRunQueue_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                tmrCheckToRunJobsTimer.Stop();

                ProcessRunQueue();
            }
            catch (Exception ex)
            {
                WriteExceptionErrorLog("ProcessRunQueue_Elapsed", ex);
            }
            finally
            {
                tmrCheckToRunJobsTimer.Start();
            }
        }

        private void ProcessRunQueue()
        {
            DataSet DataSetProcessRunQueue = new DataSet();
            int intWhere = 0;

            try
            {
                StringBuilder strQry = new StringBuilder();

                strQry.Clear();

                strQry.AppendLine(" SELECT TOP 2 ");
                strQry.AppendLine(" USER_NO");
                strQry.AppendLine(",COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",PAY_PERIOD_DATE");
                strQry.AppendLine(",PAY_CATEGORY_NUMBERS");

                strQry.AppendLine(" FROM InteractPayroll.dbo.PAYROLL_RUN_QUEUE");

                strQry.AppendLine(" WHERE PAYROLL_RUN_QUEUE_IND IS NULL");

                intWhere = 1;

                clsDBConnectionObjectsProcessRunQueue.Create_DataTable(strQry.ToString(), DataSetProcessRunQueue, "JobQueue", -1);

                intWhere = 2;

                if (DataSetProcessRunQueue.Tables["JobQueue"].Rows.Count > 0)
                {
                    intWhere = 3;

                    List<RunQueue> runQueueList = new List<RunQueue>();

                    intWhere = 4;

                    for (int intRow = 0; intRow < DataSetProcessRunQueue.Tables["JobQueue"].Rows.Count; intRow++)
                    {
                        intWhere = 5;

                        RunQueue runQueue = new RunQueue();

                        runQueue.UserNo = Convert.ToInt64(DataSetProcessRunQueue.Tables["JobQueue"].Rows[intRow]["USER_NO"]);
                        runQueue.CompanyNo = Convert.ToInt64(DataSetProcessRunQueue.Tables["JobQueue"].Rows[intRow]["COMPANY_NO"]);
                        runQueue.PayCategoryType = DataSetProcessRunQueue.Tables["JobQueue"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString();
                        runQueue.PayCategoryNumberArray = DataSetProcessRunQueue.Tables["JobQueue"].Rows[intRow]["PAY_CATEGORY_NUMBERS"].ToString();
                        runQueue.PayPeriodDate = Convert.ToDateTime(DataSetProcessRunQueue.Tables["JobQueue"].Rows[intRow]["PAY_PERIOD_DATE"]);

                        intWhere = 6;

                        runQueueList.Add(runQueue);
                    }

                    intWhere = 7;

                    ProcessRunQueueParallel(runQueueList);
                }
            }
            catch (Exception ex)
            {
                WriteExceptionErrorLog("ProcessRunQueue " + intWhere.ToString(), ex);
            }
            finally
            {
                DataSetProcessRunQueue.Dispose();
            }
        }

        private void ProcessCloseRunQueueParallel(List<CloseQueue> list)
        {
            ParallelOptions parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = 4;

            Parallel.ForEach(list, parallelOptions, closeQueue =>
            {
                new clsProcessPayrollRunFromQueue().ProcessCloseRunFromQueue(closeQueue);
            });
        }

        public void ProcessCloseRunQueue_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                this.tmrCheckToRunCloseJobsTimer.Stop();

                ProcessCloseQueue();
            }
            catch (Exception ex)
            {
                WriteExceptionErrorLog("ProcessCloseRunQueue_Elapsed", ex);
            }
            finally
            {
                tmrCheckToRunCloseJobsTimer.Start();
            }
        }
        
        private void ProcessEmailPayslipQueue()
        {
            DataSet DataSetProcessEmailQueue = new DataSet();
            int intWhere = 0;

            try
            {
                StringBuilder strQry = new StringBuilder();

                strQry.Clear();

                strQry.AppendLine(" SELECT TOP 2 ");
                strQry.AppendLine(" COMPANY_NO");
                strQry.AppendLine(",USER_NO");
                strQry.AppendLine(",PAY_PERIOD_DATE");
                strQry.AppendLine(",PAYSLIP_EMAIL_QUEUE_NO");
                
                strQry.AppendLine(" FROM InteractPayroll.dbo.PAYSLIP_EMAIL_QUEUE");

                strQry.AppendLine(" WHERE START_RUN_DATE IS NULL");
                
                intWhere = 1;

                clsDBConnectionObjectsProcessEmailPayslipQueue.Create_DataTable(strQry.ToString(), DataSetProcessEmailQueue, "EmailPayslipQueue", -1);

                if (DataSetProcessEmailQueue.Tables["EmailPayslipQueue"].Rows.Count > 0)
                {
                    intWhere = 2;

                    List<EmailPayslipQueue> emailPayslipQueueList = new List<EmailPayslipQueue>();

                    intWhere = 3;

                    for (int intRow = 0; intRow < DataSetProcessEmailQueue.Tables["EmailPayslipQueue"].Rows.Count; intRow++)
                    {
                        intWhere = 4;

                        EmailPayslipQueue emailPayslipQueue = new EmailPayslipQueue();

                        emailPayslipQueue.CompanyNo = Convert.ToInt64(DataSetProcessEmailQueue.Tables["EmailPayslipQueue"].Rows[intRow]["COMPANY_NO"]);
                        emailPayslipQueue.UserNo = Convert.ToInt64(DataSetProcessEmailQueue.Tables["EmailPayslipQueue"].Rows[intRow]["USER_NO"]);
                        emailPayslipQueue.PayPeriodDate = Convert.ToDateTime(DataSetProcessEmailQueue.Tables["EmailPayslipQueue"].Rows[intRow]["PAY_PERIOD_DATE"]);
                        emailPayslipQueue.PayslipEmailQueueNo = Convert.ToInt32(DataSetProcessEmailQueue.Tables["EmailPayslipQueue"].Rows[intRow]["PAYSLIP_EMAIL_QUEUE_NO"]);

                        intWhere = 5;

                        emailPayslipQueueList.Add(emailPayslipQueue);
                    }

                    intWhere = 6;

                    ProcessEmailPayslipQueueParallel(emailPayslipQueueList);
                }
            }
            catch (Exception ex)
            {
                WriteExceptionErrorLog("ProcessEmailPayslipQueue " + intWhere.ToString(), ex);
            }
            finally
            {
                DataSetProcessEmailQueue.Dispose();
            }
        }
        
        private void ProcessEmailPayslipQueueParallel(List<EmailPayslipQueue> list)
        {
            ParallelOptions parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = 2;

            Parallel.ForEach(list, parallelOptions, emailPayslipQueue =>
            {
                new clsProcessPayrollRunFromQueue().ProcessEmailPayslipFromQueue(emailPayslipQueue);
            });
        }

        public void ProcessEmailPayslipQueue_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                tmrCheckToEmailPayslipsJobsTimer.Stop();

                ProcessEmailPayslipQueue();
            }
            catch (Exception ex)
            {
                WriteExceptionErrorLog("ProcessEmailPayslipQueue_Elapsed", ex);
            }
            finally
            {
                tmrCheckToEmailPayslipsJobsTimer.Start();
            }
        }

        private void ProcessCloseQueue()
        {
            DataSet DataSetProcessCloseQueue = new DataSet();
            int intWhere = 0;

            try
            {
                StringBuilder strQry = new StringBuilder();

                strQry.Clear();

                strQry.AppendLine(" SELECT TOP 2 ");
                strQry.AppendLine(" USER_NO");
                strQry.AppendLine(",COMPANY_NO");
                strQry.AppendLine(",RUN_TYPE");
                strQry.AppendLine(",WAGE_PAY_PERIOD_DATE");
                strQry.AppendLine(",SALARY_PAY_PERIOD_DATE");
                strQry.AppendLine(",WAGES_PAY_CATEGORY_NUMBERS");
                strQry.AppendLine(",SALARIES_PAY_CATEGORY_NUMBERS");

                strQry.AppendLine(" FROM InteractPayroll.dbo.CLOSE_RUN_QUEUE");

                strQry.AppendLine(" WHERE CLOSE_RUN_QUEUE_IND IS NULL");

                intWhere = 1;

                clsDBConnectionObjectsProcessCloseQueue.Create_DataTable(strQry.ToString(), DataSetProcessCloseQueue, "CloseQueue", -1);

                if (DataSetProcessCloseQueue.Tables["CloseQueue"].Rows.Count > 0)
                {
                    intWhere = 2;

                    List<CloseQueue> closeQueueList = new List<CloseQueue>();

                    intWhere = 3;

                    for (int intRow = 0; intRow < DataSetProcessCloseQueue.Tables["CloseQueue"].Rows.Count; intRow++)
                    {
                        intWhere = 4;

                        CloseQueue closeQueue = new CloseQueue();

                        closeQueue.UserNo = Convert.ToInt64(DataSetProcessCloseQueue.Tables["CloseQueue"].Rows[intRow]["USER_NO"]);
                        closeQueue.CompanyNo = Convert.ToInt64(DataSetProcessCloseQueue.Tables["CloseQueue"].Rows[intRow]["COMPANY_NO"]);
                        closeQueue.RunType = DataSetProcessCloseQueue.Tables["CloseQueue"].Rows[intRow]["RUN_TYPE"].ToString();
                        closeQueue.WagesPayCategoryNumberArray = DataSetProcessCloseQueue.Tables["CloseQueue"].Rows[intRow]["WAGES_PAY_CATEGORY_NUMBERS"].ToString();
                        
                        closeQueue.SalariesPayCategoryNumberArray = DataSetProcessCloseQueue.Tables["CloseQueue"].Rows[intRow]["SALARIES_PAY_CATEGORY_NUMBERS"].ToString();

                        if (closeQueue.RunType != "S")
                        {
                            closeQueue.WagePayPeriodDate = Convert.ToDateTime(DataSetProcessCloseQueue.Tables["CloseQueue"].Rows[intRow]["WAGE_PAY_PERIOD_DATE"]);
                        }
                        
                        if (closeQueue.RunType == "B"
                        || closeQueue.RunType == "S")
                        {
                            closeQueue.SalaryPayPeriodDate = Convert.ToDateTime(DataSetProcessCloseQueue.Tables["CloseQueue"].Rows[intRow]["SALARY_PAY_PERIOD_DATE"]);
                        }

                        intWhere = 5;

                        closeQueueList.Add(closeQueue);
                    }

                    intWhere = 6;

                    ProcessCloseRunQueueParallel(closeQueueList);
                }
            }
            catch (Exception ex)
            {
                WriteExceptionErrorLog("ProcessCloseQueue " + intWhere.ToString(), ex);
            }
            finally
            {
                DataSetProcessCloseQueue.Dispose();
            }
        }

        private void ProcessOpenRunQueueParallel(List<OpenRunQueue> list)
        {
            ParallelOptions parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = 4;

            Parallel.ForEach(list, parallelOptions, openRunQueue =>
            {
                new clsProcessPayrollRunFromQueue().ProcessOpenRunFromQueue(openRunQueue);
            });
        }

        public void ProcessOpenRunQueue_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                tmrCheckToOpenJobsTimer.Stop();

                ProcessOpenRunQueue();
            }
            catch (Exception ex)
            {
                WriteExceptionErrorLog("ProcessOpenRunQueue_Elapsed", ex);
            }
            finally
            {
                tmrCheckToOpenJobsTimer.Start();
            }
        }

        private void ProcessOpenRunQueue()
        {
            DataSet DataSetProcessOpenRunQueue = new DataSet();

            int intWhere = 0;

            try
            {
                StringBuilder strQry = new StringBuilder();

                strQry.Clear();

                strQry.AppendLine(" SELECT TOP 2 ");
                strQry.AppendLine(" USER_NO");
                strQry.AppendLine(",COMPANY_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",PAY_PERIOD_DATE");
                strQry.AppendLine(",PAY_CATEGORY_NUMBERS");

                strQry.AppendLine(" FROM InteractPayroll.dbo.OPEN_RUN_QUEUE");

                strQry.AppendLine(" WHERE OPEN_RUN_QUEUE_IND IS NULL");

                intWhere = 1;

                clsDBConnectionObjectsProcessOpenRunQueue.Create_DataTable(strQry.ToString(), DataSetProcessOpenRunQueue, "OpenRunQueue", -1);

                intWhere = 2;

                if (DataSetProcessOpenRunQueue.Tables["OpenRunQueue"].Rows.Count > 0)
                {
                    List<OpenRunQueue> openRunQueueList = new List<OpenRunQueue>();

                    intWhere = 3;

                    for (int intRow = 0; intRow < DataSetProcessOpenRunQueue.Tables["OpenRunQueue"].Rows.Count; intRow++)
                    {
                        OpenRunQueue openRunQueue = new OpenRunQueue();

                        openRunQueue.UserNo = Convert.ToInt64(DataSetProcessOpenRunQueue.Tables["OpenRunQueue"].Rows[intRow]["USER_NO"]);
                        openRunQueue.CompanyNo = Convert.ToInt64(DataSetProcessOpenRunQueue.Tables["OpenRunQueue"].Rows[intRow]["COMPANY_NO"]);
                        openRunQueue.PayCategoryType = DataSetProcessOpenRunQueue.Tables["OpenRunQueue"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString();
                        openRunQueue.PayCategoryNumberArray = DataSetProcessOpenRunQueue.Tables["OpenRunQueue"].Rows[intRow]["PAY_CATEGORY_NUMBERS"].ToString();
                        openRunQueue.PayPeriodDate = Convert.ToDateTime(DataSetProcessOpenRunQueue.Tables["OpenRunQueue"].Rows[intRow]["PAY_PERIOD_DATE"]);

                        intWhere = 4;

                        openRunQueueList.Add(openRunQueue);
                    }

                    intWhere = 5;

                    ProcessOpenRunQueueParallel(openRunQueueList);
                }
            }
            catch (Exception ex)
            {
                WriteExceptionErrorLog("ProcessOpenRunQueue " + intWhere.ToString(), ex);
            }
            finally
            {
                DataSetProcessOpenRunQueue.Dispose();
            }
        }

        private void WriteLog(string Message)
        {
            try
            {
                using (StreamWriter writeLog = new StreamWriter(pvtstrLogFileName, true))
                {
                    writeLog.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + Message);
                }
            }
            catch (Exception ex)
            {
                WriteExceptionErrorLog(Message, ex);
            }
        }

        private void WriteExceptionErrorLog(string Message, Exception ex)
        {
            try
            {
                string strExceptionMessage = ex.Message;

                if (ex.InnerException != null)
                {
                    strExceptionMessage = ex.InnerException.Message;
                }

                using (StreamWriter writeExceptionErrorLog = new StreamWriter(pvtstrLogFileName.Replace(".txt","_Error.txt"), true))
                {
                    writeExceptionErrorLog.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + Message + " Exception = " + strExceptionMessage);
                }
            }
            catch
            {
            }
        }
    }
}
