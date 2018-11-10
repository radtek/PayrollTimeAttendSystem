using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using InteractPayrollClient;

namespace InteractPayroll
{
    public partial class frmSplashScreen : Form
    {
        frmConnectionSetup frmConnectionSetup;

        //NB Used Only in Internet Mode
        clsFileDownLoad clsFileDownLoad;

        private DataTable pvtDataTable;

        clsISUtilities clsISUtilities;

        //Local Database
        clsISClientUtilities clsISClientUtilities;
        
        frmPasswordChange frmPasswordChange;

        private bool pvtblnInternetBeingUsed = false;

        //Logon Information
        string pvtstrUserInformation = "";
        string pvtstrAccessInd = "S";
        string pvtstrEditHelpInd = "N";
        string pvtstrResetInd = "";
        private byte[] pvtbytCompress;
        Int64 pvtint64UserNo;
        Int64 pvtint64LastCompanyNo;
        string pvtstrPayCategoryType = "";
 
        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;

        public frmSplashScreen()
        {
            InitializeComponent();
        }

        public void frmSplashScreen_Load(object sender, System.EventArgs e)
        {
            try
            {
                AppDomain.CurrentDomain.SetData("KillApp", "");

                this.Show();
                this.Refresh();
                Application.DoEvents();

                this.Cursor = Cursors.AppStarting;

                clsFileDownLoad = new clsFileDownLoad();

                pvtDataTable = new DataTable();

                //Get File Version and Dates from Current Directories
                pvtDataTable = clsFileDownLoad.Get_Files_Directories();

                clsISUtilities = null;
                clsISUtilities = new clsISUtilities(this, "busPayrollLogon");
                
                pvtDataSet = new DataSet();

                //Local Database
                clsISClientUtilities = new clsISClientUtilities(this, "busClientPayrollLogon");
#if(DEBUG)
                //this.txtUserId.Text = "JOHNNYH";
                //this.txtPassword.Text = "JESUS*01";
                this.txtUserId.Text = "Interact";
                this.txtPassword.Text = "tcaretni";
                //this.txtUserId.Text = "gary";
                //this.txtPassword.Text = "garylr";
                //this.txtUserId.Text = "errol";
                //this.txtPassword.Text = "errol";
#endif
                this.txtUserId.Enabled = true;
                this.txtPassword.Enabled = true;
                this.btnOK.Enabled = true;

                this.txtUserId.Focus();
#if(DEBUG)
                SendKeys.Send("{ENTER}");
#endif

                this.Cursor = Cursors.Default;
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
                this.Close();
            }
        }
        
        private void btnOK_Click(object sender, System.EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.AppStarting;

                clsISUtilities.Set_WebService_Timeout_Value(30000);

                this.txtUserId.Enabled = false;
                this.txtPassword.Enabled = false;
                this.btnOK.Enabled = false;

                object[] objParm = null;
                string strClientDBConnected = "Y";

                DataSet DataSet = new DataSet();

                try
                {
                    //Check Client Databse ia Available / Check Tables are Correct
                    pvtblnInternetBeingUsed = false;
                    pvtbytCompress = (byte[])clsISClientUtilities.DynamicFunction("Logon_Client_DataBase", objParm, false);

                    DataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                }
                catch (Exception ex)
                {
                    strClientDBConnected = "N";
                }

                if (pvtDataSet.Tables["ReturnValues"] != null)
                {
                    pvtDataSet.Tables.Remove("ReturnValues");
                }

                if (this.txtUserId.Text.Trim() == "")
                {
                    this.Cursor = Cursors.Default;

                    this.txtUserId.Enabled = true;
                    this.txtPassword.Enabled = true;
                    this.btnOK.Enabled = true;

                    MessageBox.Show("Enter " + this.lblUserEmployee.Text + ".", this.Text,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                   
                    this.txtUserId.Focus();
                }
                else
                {
                    if (this.txtPassword.Text.Trim() == "")
                    {
                        this.Cursor = Cursors.Default;

                        this.txtUserId.Enabled = true;
                        this.txtPassword.Enabled = true;
                        this.btnOK.Enabled = true;

                        MessageBox.Show("Enter Password.", this.Text,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        this.txtPassword.Focus();
                    }
                    else
                    {
                        pvtstrUserInformation = this.txtUserId.Text.Trim().ToUpper() + "|" + this.txtPassword.Text.Trim().ToUpper();

                        this.btnOK.Enabled = false;

                        this.txtUserId.Enabled = false;
                        this.txtPassword.Enabled = false;

                        objParm = new object[2];
                        objParm[0] = pvtstrUserInformation;
                        objParm[1] = strClientDBConnected;

                        pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Logon_User_TimeAttendance", objParm);

                        pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                        pvtDataSet.Merge(pvtTempDataSet);

                        pvtint64UserNo = Convert.ToInt64(pvtDataSet.Tables["ReturnValues"].Rows[0]["USER_NO"]);
                        pvtstrResetInd = pvtDataSet.Tables["ReturnValues"].Rows[0]["RESET_IND"].ToString();
                        pvtstrAccessInd = pvtDataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"].ToString();
                        pvtint64LastCompanyNo = Convert.ToInt64(pvtDataSet.Tables["ReturnValues"].Rows[0]["LAST_COMPANY_NO"]);
                        pvtstrPayCategoryType = pvtDataSet.Tables["ReturnValues"].Rows[0]["LAST_PAY_CATEGORY_TYPE"].ToString();

                        if (pvtint64UserNo == -1)
                        {
                            MessageBox.Show(this.lblUserEmployee.Text + " Id / Password NOT Found.", this.Text,
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            this.txtUserId.Enabled = true;
                            this.txtPassword.Enabled = true;

                            this.btnOK.Enabled = true;

                            return;
                        }
                        else
                        {
                            if (pvtDataSet.Tables["ReturnValues"].Rows[0]["LOCK_IND"].ToString() == "Y")
                            {
                                frmLock frmLock = new frmLock();
                                frmLock.ShowDialog();

                                //Set so That Lower Down in Program it Closes in exe
                                AppDomain.CurrentDomain.SetData("UserNo", -1);

                                this.Close();
                                return;
                            }
                            else
                            {
                                if (pvtstrResetInd == "Y")
                                {
                                    AppDomain.CurrentDomain.SetData("UserNo", pvtint64UserNo);

                                    //Password Change
                                    frmPasswordChange = new frmPasswordChange();
                                    frmPasswordChange.ShowDialog();

                                    if (AppDomain.CurrentDomain.GetData("PasswordChanged").ToString() != "Y")
                                    {
                                        //Set so That Lower Down in Program it Closes in exe
                                        AppDomain.CurrentDomain.SetData("UserNo", -1);
                                        this.Close();
                                        return;
                                    }

                                    frmPasswordChange = null;
                                }

                                //Interact System Administrator
                                if (pvtint64UserNo == 0)
                                {
                                    if (pvtDataSet.Tables["MenuTag"] != null)
                                    {
                                        if (Convert.ToInt32(pvtDataSet.Tables["MenuTag"].Rows[0]["NULL_COUNT"]) > 0)
                                        {
                                            MessageBox.Show("TABLE 'MENU_FILE_NAME' Has NO Menu Tag");
                                        }

                                        pvtDataSet.Tables.Remove("MenuTag");
                                    }
                                }

                                this.btnOK.Enabled = false;

                                AppDomain.CurrentDomain.SetData("Logoff", false);

#if(DEBUG)
                                //Errol Temp Remove
                                //if (AppDomain.CurrentDomain.GetData("URLPath").ToString() != "")
                                //{
                                    pvtDataSet.Tables["Files"].Clear();
                                //}
#endif

                                //SET ALL GLOBAL VARIABLES HERE
                                AppDomain.CurrentDomain.SetData("UserNo", pvtint64UserNo);
                                AppDomain.CurrentDomain.SetData("AccessInd", pvtstrAccessInd);
                                AppDomain.CurrentDomain.SetData("LastCompanyNo", pvtint64LastCompanyNo);

                                if (pvtDataSet.Tables["Files"].Rows.Count > 0)
			                    {
                                    //Check for any Downloads
				                    bool blnLogoff = false;

                                    int intReturnCode = clsFileDownLoad.DownLoad_Files(ref pvtDataSet,ref DataSet, ref blnLogoff, strClientDBConnected);

                                    if (intReturnCode == 99)
                                    {
                                        //Restart Windows Service
                                        frmRestartService frmRestartService = new frmRestartService(DataSet.Tables["ReturnValues"].Rows[0]["MACHINE_NAME"].ToString(), DataSet.Tables["ReturnValues"].Rows[0]["MACHINE_IP"].ToString());

                                        frmRestartService.Show();

                                        try
                                        {
                                            clsRestartFingerPrintClockTimeAttendanceService clsRestartFingerPrintClockTimeAttendanceService;

                                            if (AppDomain.CurrentDomain.GetData("URLClientPath").ToString() != "")
                                            {
                                                //Calls Web Service Internally
                                                clsRestartFingerPrintClockTimeAttendanceService = new clsRestartFingerPrintClockTimeAttendanceService("");

                                                InteractPayrollClient.Restart Restart = (InteractPayrollClient.Restart)clsRestartFingerPrintClockTimeAttendanceService.DynamicFunction("RestartFingerPrintClockTimeAttendanceService", null);

                                                if (Restart.OK == "Y")
                                                {
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Failed to Restart 'FingerPrintClockTimeAttendanceService' Service.\n\nSpeak to System Administrator.\n\nReboot of Machine '" + pvtDataSet.Tables["ReturnValues"].Rows[0]["MACHINE_NAME"].ToString() + "' (IP = " + pvtDataSet.Tables["ReturnValues"].Rows[0]["MACHINE_IP"].ToString() + ")\n will Allow you to Continue.",
                                                    "Program Changes",
                                                    MessageBoxButtons.OK,
                                                    MessageBoxIcon.Exclamation);
                                                }
                                            }
                                            else
                                            {
                                                clsRestartFingerPrintClockTimeAttendanceService = new clsRestartFingerPrintClockTimeAttendanceService("FingerPrintClockServiceStartStop");

                                                object Restart = clsRestartFingerPrintClockTimeAttendanceService.DynamicFunction("RestartFingerPrintClockTimeAttendanceService", null);

                                                string strRestart = Restart.ToString();

                                                if (strRestart.IndexOf("FingerPrintClockServer.Restart") > -1)
                                                {
                                                    //Not Really True
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Failed to Restart 'FingerPrintClockTimeAttendanceService' Service.\n\nSpeak to System Administrator.\n\nReboot of Machine '" + pvtDataSet.Tables["ReturnValues"].Rows[0]["MACHINE_NAME"].ToString() + "' (IP = " + pvtDataSet.Tables["ReturnValues"].Rows[0]["MACHINE_IP"].ToString() + ")\n will Allow you to Continue.",
                                                    "Program Changes",
                                                    MessageBoxButtons.OK,
                                                    MessageBoxIcon.Exclamation);
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            MessageBox.Show("Failed to Restart 'FingerPrintClockTimeAttendanceService' Service.\n\nSpeak to System Administrator.\n\nReboot of Machine '" + pvtDataSet.Tables["ReturnValues"].Rows[0]["MACHINE_NAME"].ToString() + "' (IP = " + pvtDataSet.Tables["ReturnValues"].Rows[0]["MACHINE_IP"].ToString() + ")\n will Allow you to Continue.",
                                            "Program Changes",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Exclamation);
                                        }

                                        frmRestartService.Close();
                                    }
                                    else
                                    {
                                        if (intReturnCode != 0)
                                        {
                                            if (AppDomain.CurrentDomain.GetData("KillApp").ToString() == "Y")
                                            {
                                            }
                                            else
                                            {
                                                MessageBox.Show("Error In Download of File.\nProgram Closing.", this.Text,
                                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }

                                            //Force Program To Close
                                            AppDomain.CurrentDomain.SetData("UserNo", -1);

                                            this.Close();
                                            return;
                                        }


                                        if (blnLogoff == true)
                                        {
                                            AppDomain.CurrentDomain.SetData("Logoff", true);
                                        }
                                    }

                                    if (strClientDBConnected == "Y")
                                    {
                                        if (DataSet.Tables["FileToDelete"].Rows.Count > 0)
                                        {
                                            DataSet myDataSet = new System.Data.DataSet();

                                            DataTable myDataTable = DataSet.Tables["FileToDelete"].Copy();
                                            myDataSet.Tables.Add(myDataTable);

                                            byte[] mybytCompress = clsISClientUtilities.Compress_DataSet(DataSet);

                                            object[] obj = new object[1];
                                            obj[0] = mybytCompress;

                                            pvtblnInternetBeingUsed = false;
                                            clsISClientUtilities.DynamicFunction("Cleanup_Client_DataBase_Files", obj, false);
                                        }
                                    }
			                    }
			                    else
			                    {
				                    //Delete Any Version Sub Directories
				                    string strDirectory = "";

				                    for (int intRow = 0;intRow < pvtDataTable.Rows.Count;intRow++)
				                    {
					                    strDirectory = AppDomain.CurrentDomain.BaseDirectory + pvtDataTable.Rows[intRow]["PROJECT_VERSION"].ToString();

					                    if (Directory.Exists(strDirectory) == true) 
					                    {
						                    // Delete the target to ensure it is not there.
						                    Directory.Delete(strDirectory, true);
					                    }
				                    }
			                    }
			
			                    pvtDataSet.Tables.Remove(pvtDataSet.Tables["Files"]);

			                    clsFileDownLoad = null;

                                if (pvtstrPayCategoryType == "")
                                {
                                    pvtstrPayCategoryType = "T";
                                }
                                
                                AppDomain.CurrentDomain.SetData("LastPayCategoryType", pvtstrPayCategoryType);
                                
                                if (pvtstrAccessInd == "S")
                                {
                                    if (pvtstrEditHelpInd == null)
                                    {
                                        //Initial Communication Failure (Reset via Connection Button)
                                        AppDomain.CurrentDomain.SetData("EditHelpInd", "N");
                                    }
                                    else
                                    {
                                        AppDomain.CurrentDomain.SetData("EditHelpInd", pvtstrEditHelpInd.Substring(0, 1));
                                    }
                                }
                                else
                                {
                                    AppDomain.CurrentDomain.SetData("EditHelpInd", "N");
                                }

                                //Used in Employee Form to Show Tax Rate
                                if (pvtstrEditHelpInd == null)
                                {
                                    AppDomain.CurrentDomain.SetData("TaxCasual", 25);
                                }
                                else
                                {
                                    AppDomain.CurrentDomain.SetData("TaxCasual", pvtstrEditHelpInd.Substring(pvtstrEditHelpInd.IndexOf(",") + 1));
                                }

                                AppDomain.CurrentDomain.SetData("CurrentForm", "");
                                AppDomain.CurrentDomain.SetData("DataSet", this.pvtDataSet);

                                string strPath = AppDomain.CurrentDomain.BaseDirectory + "TimeAttendanceMain.dll";
                                
                                AppDomain.CurrentDomain.SetData("StartUpFile", strPath);

                                this.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            //Set To Fall Through the Calling Program
            AppDomain.CurrentDomain.SetData("UserNo", -1);
            AppDomain.CurrentDomain.SetData("KillApp", "Y");

            this.Close();
        }

        private void Write_Debug_Info(string parstrMessage)
        {
            FileInfo fiDebugFile = new FileInfo("Debug.txt");

            StreamWriter swErrorStreamWriter = fiDebugFile.AppendText();

            swErrorStreamWriter.WriteLine(parstrMessage);

            swErrorStreamWriter.Close();
        }

        private void btnConnection_Click(object sender, EventArgs e)
        {
            frmConnectionSetup = new frmConnectionSetup();

            frmConnectionSetup.ShowDialog();

            this.DialogResult = DialogResult.None;

            this.txtUserId.Enabled = true;
            this.txtPassword.Enabled = true;

            this.btnOK.Enabled = true;
        }
    }
}
