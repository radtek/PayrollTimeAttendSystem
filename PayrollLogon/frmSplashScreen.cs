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

        private bool pvtblnInternetBeingUsed = false;

        private bool _dragging = false;
        private Point _offset;
        private Point _start_point = new Point(0, 0);

        public frmSplashScreen()
        {
            InitializeComponent();

            clsISUtilities = null;
            clsISUtilities = new clsISUtilities(this, "busPayrollLogon");

            this.lblHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(clsISUtilities.lblHeader_MouseDown);
            this.lblHeader.MouseMove += new System.Windows.Forms.MouseEventHandler(clsISUtilities.lblHeader_MouseMove);
            this.lblHeader.MouseUp += new System.Windows.Forms.MouseEventHandler(clsISUtilities.lblHeader_MouseUp);
            this.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Form_Paint);

            clsISUtilities.Set_WebService_Timeout_Value(20000);
        }
        
        public void frmSplashScreen_Load(object sender, System.EventArgs e)
        {
            try
            {
                AppDomain.CurrentDomain.SetData("KillApp", "");

                this.Show();
                this.Refresh();
                Application.DoEvents();
                this.Focus();

                this.Cursor = Cursors.AppStarting;

                clsFileDownLoad = new clsFileDownLoad();

                pvtDataTable = new DataTable();

                //Get File Version and Dates from Current Directories
                pvtDataTable = clsFileDownLoad.Get_Files_Directories();
           
                pvtDataSet = new DataSet();

                //Local Database
                clsISClientUtilities = new clsISClientUtilities(this, "busClientPayrollLogon");

                //try
                //{
                //    //Try to Get DB Up (LocalDB)
                //    object[] objParm = null;

                //    clsISClientUtilities.DynamicFunction("Logon_Client_DataBase", objParm, false);
                //}
                //catch (Exception ex)
                //{
                //}
#if (DEBUG)
                //this.txtUserId.Text = "HelenaLR";
                //this.txtPassword.Text = "monster";
                this.txtUserId.Text = "Interact";
                this.txtPassword.Text = "tcaretni";

                //this.txtUserId.Text = "migs";
                //this.txtPassword.Text = "migs";

                //this.txtUserId.Text = "CasperB";
                //this.txtPassword.Text = "DRIKKIE69";

                //this.txtUserId.Text = "johnnyh";
                //this.txtPassword.Text = "jesus*01";

                //this.txtUserId.Text = "henk";
                //this.txtPassword.Text = "henk";

                //this.txtUserId.Text = "gary";
                //this.txtPassword.Text = "garylr";
                //this.txtUserId.Text = "errol";
                //this.txtPassword.Text = "errol";

#endif
                this.txtUserId.Enabled = true;
                this.txtPassword.Enabled = true;
                this.btnOK.Enabled = true;
                this.btnConnection.Enabled = true;

                this.txtUserId.Focus();
#if(DEBUG)
                btnOK_Click(sender, e);
#endif
                this.Cursor = Cursors.Default;

                this.Focus();
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

                this.txtUserId.Enabled = false;
                this.txtPassword.Enabled = false;
                this.btnOK.Enabled = false;
                this.btnConnection.Enabled = false;

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
                    this.btnConnection.Enabled = true;

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
                        this.btnConnection.Enabled = true;

                        MessageBox.Show("Enter Password.", this.Text,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        this.txtPassword.Focus();
                    }
                    else
                    {
                        pvtstrUserInformation = this.txtUserId.Text.Trim().ToUpper() + "|" + this.txtPassword.Text.Trim().ToUpper();

                        objParm = new object[2];
                        objParm[0] = pvtstrUserInformation;
                        objParm[1] = strClientDBConnected;

                        pvtblnInternetBeingUsed = true;
                        //"Logon_User_New" Changed to "Logon_New_User"
                        pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Logon_New_User", objParm);

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
                            this.btnConnection.Enabled = true;

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
                                bool blnCompanyLocked = false;
                                frmCompanyLock frmLock = new frmCompanyLock();

                                for (int intRow = 0; intRow < pvtDataSet.Tables["Company"].Rows.Count; intRow++)
                                {
                                    if (pvtDataSet.Tables["Company"].Rows[intRow]["LOCK_IND"].ToString() == "Y")
                                    {
                                        frmLock.dgvCompanyDataGridView.Rows.Add(pvtDataSet.Tables["Company"].Rows[intRow]["COMPANY_DESC"].ToString());
                                        blnCompanyLocked = true;
                                    }
                                }

                                if (blnCompanyLocked == true)
                                {
                                    frmLock.ShowDialog();
                                }

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

                                this.btnOK.Enabled = false;
                                this.btnConnection.Enabled = false;

                                AppDomain.CurrentDomain.SetData("Logoff", false);
#if (DEBUG)
                                pvtDataSet.Tables["Files"].Clear();
#endif
                                //SET ALL GLOBAL VARIABLES HERE
                                AppDomain.CurrentDomain.SetData("UserNo", pvtint64UserNo);
                                AppDomain.CurrentDomain.SetData("AccessInd", pvtstrAccessInd);
                                AppDomain.CurrentDomain.SetData("LastCompanyNo", pvtint64LastCompanyNo);

                                string strUserCompanyToLoad = "N";

                                if (pvtDataSet.Tables["UserCompanyToLoad"].Rows.Count > 0)
                                {
                                    strUserCompanyToLoad = "Y";
                                }

                                AppDomain.CurrentDomain.SetData("UserCompanyToLoad", strUserCompanyToLoad);
                           
                                if (pvtDataSet.Tables["Files"].Rows.Count > 0)
                                {
                                    //Check for any Downloads
                                    bool blnLogoff = false;

                                    int intReturnCode = clsFileDownLoad.DownLoad_Files(ref pvtDataSet, ref DataSet, ref blnLogoff, strClientDBConnected);
                                    
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
                                                    MessageBox.Show("Failed to Restart 'FingerPrintClockTimeAttendanceService' Service.\n\nSpeak to System Administrator.\n\nReboot of Machine '" + DataSet.Tables["ReturnValues"].Rows[0]["MACHINE_NAME"].ToString() + "' (IP = " + DataSet.Tables["ReturnValues"].Rows[0]["MACHINE_IP"].ToString() + ")\n will Allow you to Continue.",
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
                                                    MessageBox.Show("Failed to Restart 'FingerPrintClockTimeAttendanceService' Service.\n\nSpeak to System Administrator.\n\nReboot of Machine '" + DataSet.Tables["ReturnValues"].Rows[0]["MACHINE_NAME"].ToString() + "' (IP = " + DataSet.Tables["ReturnValues"].Rows[0]["MACHINE_IP"].ToString() + ")\n will Allow you to Continue.",
                                                    "Program Changes",
                                                    MessageBoxButtons.OK,
                                                    MessageBoxIcon.Exclamation);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show("Failed to Restart 'FingerPrintClockTimeAttendanceService' Service.\n\nSpeak to System Administrator.\n\nReboot of Machine '" + DataSet.Tables["ReturnValues"].Rows[0]["MACHINE_NAME"].ToString() + "' (IP = " + DataSet.Tables["ReturnValues"].Rows[0]["MACHINE_IP"].ToString() + ")\n will Allow you to Continue.",
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

                                    for (int intRow = 0; intRow < pvtDataTable.Rows.Count; intRow++)
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
                                    pvtstrPayCategoryType = "W";
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

                                string strPath = "";

                                //if (this.lstVersions.Visible == true)
                                //{
                                //    if (this.lstVersions.SelectedItem.ToString() == "Current")
                                //    {
                                //        strPath = AppDomain.CurrentDomain.BaseDirectory + "PayrollMain.dll";
                                //    }
                                //    else
                                //    {
                                //        strPath = AppDomain.CurrentDomain.BaseDirectory + this.lstVersions.SelectedItem.ToString() + "\\PayrollMain.dll";

                                //        string strPathExists = strPath.Substring(0, strPath.LastIndexOf("\\"));

                                //        if (Directory.Exists(strPathExists) == false)
                                //        {
                                //            MessageBox.Show(this.lstVersions.SelectedItem.ToString() + " has been Removed from your profile - Logon will continue with 'Current' Version.",
                                //            this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                                //            strPath = AppDomain.CurrentDomain.BaseDirectory + "PayrollMain.dll";
                                //        }
                                //    }
                                //}
                                //else
                                //{
                                strPath = AppDomain.CurrentDomain.BaseDirectory + "PayrollMain.dll";
                                //}

                                AppDomain.CurrentDomain.SetData("StartUpFile", strPath);

                                this.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception eException)
            {
                this.txtUserId.Enabled = true;
                this.txtPassword.Enabled = true;

                this.btnOK.Enabled = true;
                this.btnConnection.Enabled = true;

                if (pvtblnInternetBeingUsed == true)
                {
                    clsISUtilities.ErrorHandler(eException);
                }
                else
                {
                    this.clsISClientUtilities.ErrorHandler(eException);
                }
            }
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            //Set To Fall Through the Calling Program
            AppDomain.CurrentDomain.SetData("UserNo", -1);
            AppDomain.CurrentDomain.SetData("KillApp", "Y");

            this.Close();
        }

        private void btnConnection_Click(object sender, EventArgs e)
        {
            frmConnectionSetup = new frmConnectionSetup();

            frmConnectionSetup.ShowDialog();

            this.DialogResult = DialogResult.None;
            
            this.txtUserId.Enabled = true;
            this.txtPassword.Enabled = true;

            this.btnOK.Enabled = true;
            this.btnConnection.Enabled = true;
        }
        
        private void btnClose_Click(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.SetData("UserNo", -1);
            AppDomain.CurrentDomain.SetData("KillApp", "Y");

            this.Close();
        }
    }
}
