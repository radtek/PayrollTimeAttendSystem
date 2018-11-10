using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection;
using InteractPayroll;
using System.Data.SqlClient;

namespace InteractPayrollClient
{
    public partial class frmLogonScreen : Form
    {
        clsISClientUtilities clsISClientUtilities;

        clsISUtilities clsISUtilities;

        clsCrc32 clsCrc32;

        frmPasswordChange frmPasswordChange;
        frmReadWriteFile frmReadWriteFile;

        private string pvtstrConnectionClientIntegratedSecurity;
        private SqlConnection pvtSqlConnectionClientIntegratedSecurity;
        private SqlCommand pvtSqlCommandClientIntegratedSecurity;
        private SqlDataAdapter pvtSqlDataAdapterClientIntegratedSecurity;

        Int64 pvtint64UserNo = 0;
        string pvtstrAccessInd = "S";

        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;

        private bool pvtblnLocalWebService = false;

        Cursor csrSavedCusor;

        public frmLogonScreen()
        {
            InitializeComponent();
        }

        public void frmLogonScreen_Load(object sender, System.EventArgs e)
        {
            try
            {
                this.Show();
                this.Refresh();
                Application.DoEvents();

                this.Cursor = Cursors.AppStarting;

                clsCrc32 = new clsCrc32();

                clsISClientUtilities = new clsISClientUtilities(this, "busTimeAttendanceLogon");

                this.lblHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(clsISClientUtilities.lblHeader_MouseDown);
                this.lblHeader.MouseMove += new System.Windows.Forms.MouseEventHandler(clsISClientUtilities.lblHeader_MouseMove);
                this.lblHeader.MouseUp += new System.Windows.Forms.MouseEventHandler(clsISClientUtilities.lblHeader_MouseUp);
                this.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Form_Paint);

                //Cause Paint
                this.Refresh();
                
                clsISUtilities = new clsISUtilities(this, "busPayrollLogon");
                clsISUtilities.Set_WebService_Timeout_Value(30000);

                if (AppDomain.CurrentDomain.GetData("URLClientPath").ToString() != "")
                {
                    this.txtIPAddressPortNo.Text = AppDomain.CurrentDomain.GetData("URLClientPath").ToString();
                    this.Refresh();

                    int intReturnCode = clsISClientUtilities.WebService_Ping_Test();

                    if (intReturnCode == 0)
                    {
                        this.txtUserId.Enabled = true;
                        this.txtPassword.Enabled = true;

                        this.btnOK.Enabled = true;
                    }
                }
                else
                {
                    this.txtIPAddressPortNo.Text = "Late Binding";

                    this.txtUserId.Enabled = true;
                    this.txtPassword.Enabled = true;

                    this.btnOK.Enabled = true;
                }

                pvtDataSet = new DataSet();
#if(DEBUG)
                {
                    this.txtUserId.Text = "Interact";
                    this.txtPassword.Text = "tcaretni";

                    //this.txtUserId.Text = "helenalr";
                    //this.txtPassword.Text = "monster";

                    //this.txtUserId.Text = "TyroneS";
                    //this.txtPassword.Text = "271083";

                    //this.txtUserId.Text = "ChristineG";
                    //this.txtPassword.Text = "CHRIS1";

                    //this.txtUserId.Text = "MIGS";
                    //this.txtPassword.Text = "MIGS";

                    //this.txtUserId.Text = "TREVOR";
                    //this.txtPassword.Text = "3636";
                    //this.txtUserId.Text = "JAMESM";
                    //this.txtPassword.Text = "JAMES456";
                }
#endif
                this.txtUserId.Focus();

#if(DEBUG)
                SendKeys.Send("{ENTER}");
#endif

                this.Cursor = Cursors.Default;
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
                this.txtUserId.Enabled = true;
                this.txtPassword.Enabled = true;
                this.Cursor = Cursors.Default;
                this.btnOK.Enabled = true;
            }
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (this.txtUserId.Text.Trim() == "")
                {
                    InteractPayrollClient.CustomClientMessageBox frmMessageBox = new CustomClientMessageBox("Enter User Id.", this.Text,MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    frmMessageBox.ShowDialog();
                    
                    this.txtUserId.Focus();
                }
                else
                {
                    if (this.txtPassword.Text.Trim() == "")
                    {
                        InteractPayrollClient.CustomClientMessageBox frmMessageBox = new CustomClientMessageBox("Enter Password.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        frmMessageBox.ShowDialog();
                        
                        this.txtPassword.Focus();
                    }
                    else
                    {
                        this.Cursor = Cursors.AppStarting;

                        this.txtUserId.Enabled = false;
                        this.txtPassword.Enabled = false;
                        this.btnOK.Enabled = false;
                        
                        string strUserInformation = this.txtUserId.Text.Trim().ToUpper() + "|" + this.txtPassword.Text.Trim().ToUpper();

                        object[] objParm = new object[1];
                        objParm[0] = strUserInformation;

                        byte[] bytCompress = null;

                        if (this.rbnLocal.Checked == true)
                        {
                            pvtblnLocalWebService = true;
                            bytCompress = (byte[])clsISClientUtilities.DynamicFunction("Logon_User", objParm, false);
                        }
                        else
                        {
                            pvtblnLocalWebService = false;
                            bytCompress = (byte[])clsISUtilities.DynamicFunction("Logon_Client_User", objParm, false);
                        }

                        pvtDataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(bytCompress);

                        this.Cursor = Cursors.Default;

                        this.txtUserId.Enabled = true;
                        this.txtPassword.Enabled = true;
                        this.btnOK.Enabled = true;
                        
                        if (this.rbnLocal.Checked == true)
                        {
                            if (pvtDataSet.Tables["ReturnValues"].Rows[0]["DB_CREATE_FOR_ENGINE"].ToString() == "")
                            {
                                InteractPayrollClient.CustomClientMessageBox frmMessageBox = new CustomClientMessageBox("There is Currently NO Database for SQL Server.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                frmMessageBox.ShowDialog();
                                
                                this.Close();

                                return;
                            }
                        }

                        //From Local Machine
                        if (pvtDataSet.Tables["ReturnValues"].Rows[0]["NO_USERS_IND"].ToString() == "Y")
                        {
                            //Not Found
                            InteractPayrollClient.CustomClientMessageBox frmMessageBox = new CustomClientMessageBox("There are Currently NO Users on Local Machine.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            frmMessageBox.ShowDialog();
                        }
                        else
                        {
                            if (pvtDataSet.Tables["ReturnValues"].Rows[0]["DOWNLOAD_IND"].ToString() != "")
                            {
                                //CRC Error
                                if (pvtDataSet.Tables["ReturnValues"].Rows[0]["DOWNLOAD_IND"].ToString() == "C")
                                {
                                    //Not Found
                                    InteractPayrollClient.CustomClientMessageBox frmMessageBox = new CustomClientMessageBox("Server CRC File Error\n\nTry Re-Login\n\nIf Problem Persists, Speak to Administrator.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    frmMessageBox.ShowDialog();
                                }
                                else
                                {
                                    //Not Found
                                    InteractPayrollClient.CustomClientMessageBox frmMessageBox = new CustomClientMessageBox("Server Download Error\n\nTry Re-Login\n\nIf Problem Persists, Speak to Administrator.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    frmMessageBox.ShowDialog();
                                }

                                this.Close();

                                return;
                            }
                            else
                            {
                                if (pvtDataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString() == "-1")
                                {
                                    //Not Found
                                    InteractPayrollClient.CustomClientMessageBox frmMessageBox = new CustomClientMessageBox("User Id / Password NOT Found.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    frmMessageBox.ShowDialog();
                                }
                                else
                                {
                                btnOK_Click_Continue:

                                    if (pvtDataSet.Tables["ReturnValues"].Rows[0]["RESET"].ToString() == "Y")
                                    {
                                        if (this.rbnLocal.Checked == true)
                                        {
                                            InteractPayrollClient.CustomClientMessageBox frmMessageBox = new CustomClientMessageBox("User Needs To Login To Internet Site to Change Password.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            frmMessageBox.ShowDialog();
                                        }
                                        else
                                        {
                                            AppDomain.CurrentDomain.SetData("UserNo", pvtDataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString());

                                            frmPasswordChange = null;
                                            frmPasswordChange = new frmPasswordChange();
                                            frmPasswordChange.ShowDialog();

                                            if (AppDomain.CurrentDomain.GetData("PasswordChanged").ToString() != "Y")
                                            {
                                                //Set so That Lower Down in Program it Closes in exe
                                                AppDomain.CurrentDomain.SetData("UserNo", -1);
                                            }
                                            else
                                            {
                                                pvtDataSet.Tables["ReturnValues"].Rows[0]["RESET"] = "N";
                                                goto btnOK_Click_Continue;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        pvtint64UserNo = Convert.ToInt64(pvtDataSet.Tables["ReturnValues"].Rows[0]["USER_NO"]);

                                        pvtstrAccessInd = pvtDataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"].ToString();

                                        if (this.rbnLocal.Checked == true)
                                        {
                                            string strApplicationPath = AppDomain.CurrentDomain.BaseDirectory;
#if (DEBUG)
                                            strApplicationPath = AppDomain.CurrentDomain.BaseDirectory + "bin\\";
#endif
                                            FileInfo fiFileInfo;
                                            BinaryReader brBinaryReader;
                                            FileStream fsFileStream;
                                            BinaryWriter bwBinaryWriter;

                                            long lngFileStartOffset = 0;
                                            byte[] bytFileBytes;
                                            byte[] bytFileChunkBytes;
                                            byte[] bytDecompressedBytes;
                                            byte[] bytTempBytes;
                                            string strDownLoadFileName = "";
                                            bool blnRestartProgram = false;
#if (DEBUG)
                                            //pvtDataSet.Tables["Files"].Rows.Clear();
#endif
                                            if (pvtDataSet.Tables["Files"].Rows.Count > 0)
                                            {
                                                for (int intRow = 0; intRow < pvtDataSet.Tables["Files"].Rows.Count; intRow++)
                                                {
                                                    fiFileInfo = new FileInfo(strApplicationPath + pvtDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString());

                                                    if (fiFileInfo.Exists == true)
                                                    {
                                                        if (fiFileInfo.LastWriteTime >= Convert.ToDateTime(pvtDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]).AddSeconds(-3)
                                                        & fiFileInfo.LastWriteTime <= Convert.ToDateTime(pvtDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]).AddSeconds(3))
                                                        {
                                                            continue;
                                                        }
                                                    }

                                                    if (pvtDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() == "TimeAttendanceClient.exe")
                                                    {
                                                        //Check If TimeAttendanceClient.exe_ Exists (Can only be Replaced Via Fix or Manual Process)

                                                        FileInfo fiFileInfoTemp = new FileInfo(strApplicationPath + pvtDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() + "_");

                                                        if (fiFileInfoTemp.Exists == true)
                                                        {
                                                            if (fiFileInfoTemp.LastWriteTime >= Convert.ToDateTime(pvtDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]).AddSeconds(-3)
                                                            & fiFileInfoTemp.LastWriteTime <= Convert.ToDateTime(pvtDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]).AddSeconds(3))
                                                            {
                                                                continue;
                                                            }
                                                        }
                                                    }

                                                    pvtDataSet.Tables["Files"].Rows[intRow]["DOWNLOAD_IND"] = "Y";
                                                }

                                                DataView FilesDataView = new DataView(pvtDataSet.Tables["Files"],
                                                                                      "DOWNLOAD_IND = 'Y'",
                                                                                      "",
                                                                                      DataViewRowState.CurrentRows);

                                                if (FilesDataView.Count > 0)
                                                {
                                                    int intProgressBarCombinedMaxValue = 0;
                                                    int intProgressBarMaxValue = 0;
                                                    
                                                    for (int intRow = 0; intRow < FilesDataView.Count; intRow++)
                                                    {
                                                        //strFileName = parDataTable.Rows[intRow]["FILE_NAME"].ToString();
                                                        intProgressBarCombinedMaxValue += Convert.ToInt32(FilesDataView[intRow]["MAX_FILE_CHUNK_NO"]);
                                                    }

                                                    frmReadWriteFile = new frmReadWriteFile();
                                                    frmReadWriteFile.Show();
                                                    frmReadWriteFile.Refresh();


                                                    frmReadWriteFile.prbAllFileProgress.Maximum = intProgressBarCombinedMaxValue;
                                                    frmReadWriteFile.prbAllFileProgress.Value = 0;
                                                    
                                                    //This Must Be Fixed
                                                    for (int intRow = 0; intRow < FilesDataView.Count; intRow++)
                                                    {
                                                        DateTime dtMinLoopTime = DateTime.Now.AddSeconds(1); 

                                                        strDownLoadFileName = FilesDataView[intRow]["FILE_NAME"].ToString();

                                                        if (strDownLoadFileName == "URLConfig.dll"
                                                        | strDownLoadFileName == "clsISUtilities.dll"
                                                        | strDownLoadFileName == "PasswordChange.dll"
                                                        | strDownLoadFileName == "DownloadFiles.dll"
                                                        | strDownLoadFileName == "clsISClientUtilities.dll"
                                                        | strDownLoadFileName == "TimeAttendanceLogon.dll"
                                                        | strDownLoadFileName == "TimeAttendanceClient.exe"
                                                        | strDownLoadFileName == "TimeAttendanceClientIS.exe")
                                                        {
                                                            if (strDownLoadFileName == "TimeAttendanceClient.exe")
                                                            {
                                                                //Fix Will have to Be done Manually or via Fix
                                                            }
                                                            else
                                                            {
                                                                blnRestartProgram = true;
                                                            }

                                                            strDownLoadFileName += "_";
                                                        }

                                                        intProgressBarMaxValue = Convert.ToInt32(FilesDataView[intRow]["MAX_FILE_CHUNK_NO"]);

                                                        frmReadWriteFile.prbFileProgress.Maximum = intProgressBarMaxValue;
                                                        frmReadWriteFile.prbFileProgress.Value = 0;
                                                        frmReadWriteFile.lblFileName.Text = strDownLoadFileName;

                                                        bytFileBytes = new byte[Convert.ToInt32(FilesDataView[intRow]["FILE_SIZE_COMPRESSED"])];
                                                        lngFileStartOffset = 0;

                                                        for (int intChunkRow = 1; intChunkRow <= Convert.ToInt32(FilesDataView[intRow]["MAX_FILE_CHUNK_NO"]); intChunkRow++)
                                                        {
                                                            objParm = new object[2];
                                                            objParm[0] = FilesDataView[intRow]["FILE_NAME"].ToString();
                                                            objParm[1] = intChunkRow;

                                                            pvtblnLocalWebService = true;
                                                            bytTempBytes = (byte[])clsISClientUtilities.DynamicFunction("Get_File_Chunk", objParm, false);

                                                            this.pvtTempDataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(bytTempBytes);

                                                            bytFileChunkBytes = (byte[])pvtTempDataSet.Tables["FileChunk"].Rows[0]["FILE_CHUNK"];

                                                            Array.Copy(bytFileChunkBytes, 0, bytFileBytes, lngFileStartOffset, bytFileChunkBytes.Length);
                                                            lngFileStartOffset += bytFileChunkBytes.Length;

                                                            frmReadWriteFile.prbAllFileProgress.Value += 1;
                                                            frmReadWriteFile.prbFileProgress.Value += 1;
                                                            this.Refresh();
                                                            Application.DoEvents();
                                                        }

                                                        bytDecompressedBytes = null;
                                                        bytDecompressedBytes = new byte[Convert.ToInt32(FilesDataView[intRow]["FILE_SIZE"])];

                                                        //Open Memory Stream with Compressed Data
                                                        MemoryStream msMemoryStream = new MemoryStream(bytFileBytes);

                                                        System.IO.Compression.GZipStream GZipStreamDecompress = new GZipStream(msMemoryStream, CompressionMode.Decompress);

                                                        //Decompress Bytes
                                                        brBinaryReader = new BinaryReader(GZipStreamDecompress);
                                                        bytDecompressedBytes = brBinaryReader.ReadBytes(Convert.ToInt32(FilesDataView[intRow]["FILE_SIZE"]));

                                                        if (FilesDataView[intRow]["FILE_CRC_VALUE"].ToString() != "")
                                                        {
                                                            //CRC32 Value
                                                            string strCRC32Value = "";

                                                            foreach (byte b in clsCrc32.ComputeHash(bytDecompressedBytes))
                                                            {
                                                                strCRC32Value += b.ToString("x2").ToLower();
                                                            }

                                                            if (strCRC32Value != FilesDataView[intRow]["FILE_CRC_VALUE"].ToString())
                                                            {
                                                                InteractPayrollClient.CustomClientMessageBox frmMessageBox = new CustomClientMessageBox("Client CRC File Error\n\nTry Re-Login\n\nIf Problem Persists, Speak to Administrator.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                                frmMessageBox.ShowDialog();
                                                                
                                                                this.Close();

                                                                return;
                                                            }
                                                        }

                                                        fsFileStream = null;
                                                        bwBinaryWriter = null;

                                                        fsFileStream = new FileStream(strApplicationPath + strDownLoadFileName, FileMode.Create);
                                                        bwBinaryWriter = new BinaryWriter(fsFileStream);

                                                        bwBinaryWriter.Write(bytDecompressedBytes);

                                                        //Write Memory Portion To Disk
                                                        bwBinaryWriter.Close();

                                                        File.SetLastWriteTime(strApplicationPath + strDownLoadFileName, Convert.ToDateTime(FilesDataView[intRow]["FILE_LAST_UPDATED_DATE"]));
                                                        
                                                        while (dtMinLoopTime > DateTime.Now)
                                                        {
                                                            this.Refresh();
                                                            Application.DoEvents();
                                                        }

                                                        this.Refresh();
                                                        Application.DoEvents();
                                                    }

                                                    frmReadWriteFile.Close();
                                                    frmReadWriteFile.Dispose();
                                                }
                                            }

                                            pvtDataSet.Tables.Remove(pvtDataSet.Tables["Files"]);

                                            if (blnRestartProgram == true
                                            | pvtDataSet.Tables["ReturnValues"].Rows[0]["REBOOT_IND"].ToString() == "Y")
                                            {
                                                bool blnCloseProgram = true;

                                                if (pvtDataSet.Tables["ReturnValues"].Rows[0]["REBOOT_IND"].ToString() == "Y")
                                                {
                                                    frmRestartService frmRestartService = new frmRestartService(pvtDataSet.Tables["ReturnValues"].Rows[0]["MACHINE_NAME"].ToString(),pvtDataSet.Tables["ReturnValues"].Rows[0]["MACHINE_IP"].ToString());

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
                                                                if (blnRestartProgram == false)
                                                                {
                                                                    blnCloseProgram = false;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                InteractPayrollClient.CustomClientMessageBox frmMessageBox = new CustomClientMessageBox("Failed to Restart 'FingerPrintClockTimeAttendanceService' Service.\n\nSpeak to System Administrator.\n\nReboot of Machine '" + pvtDataSet.Tables["ReturnValues"].Rows[0]["MACHINE_NAME"].ToString() + "' (IP = " + pvtDataSet.Tables["ReturnValues"].Rows[0]["MACHINE_IP"].ToString() + ")\n will Allow you to Continue.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                                frmMessageBox.ShowDialog();
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
                                                                if (blnRestartProgram == false)
                                                                {
                                                                    blnCloseProgram = false;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                InteractPayrollClient.CustomClientMessageBox frmMessageBox = new CustomClientMessageBox("Failed to Restart 'FingerPrintClockTimeAttendanceService' Service.\n\nSpeak to System Administrator.\n\nReboot of Machine '" + pvtDataSet.Tables["ReturnValues"].Rows[0]["MACHINE_NAME"].ToString() + "' (IP = " + pvtDataSet.Tables["ReturnValues"].Rows[0]["MACHINE_IP"].ToString() + ")\n will Allow you to Continue.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                                frmMessageBox.ShowDialog();
                                                            }
                                                        }
                                                    }
                                                    catch
                                                    {
                                                        InteractPayrollClient.CustomClientMessageBox frmMessageBox = new CustomClientMessageBox("Failed to Restart 'FingerPrintClockTimeAttendanceService' Service.\n\nSpeak to System Administrator.\n\nReboot of Machine '" + pvtDataSet.Tables["ReturnValues"].Rows[0]["MACHINE_NAME"].ToString() + "' (IP = " + pvtDataSet.Tables["ReturnValues"].Rows[0]["MACHINE_IP"].ToString() + ")\n will Allow you to Continue.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                        frmMessageBox.ShowDialog();
                                                    }

                                                    frmRestartService.Close();
                                                }
                                                
                                                if (blnRestartProgram == true)
                                                {
                                                    InteractPayrollClient.CustomClientMessageBox frmMessageBox = new CustomClientMessageBox("Changes Have been Made to the Main Program.\nYou need to Restart the Program.", "Program Changes", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                                    frmMessageBox.ShowDialog();
                                                }

                                                if (blnCloseProgram == true)
                                                {
                                                    this.Close();

                                                    return;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //2013-11-01
                                            objParm = new object[5];
                                            objParm[0] = Convert.ToInt64(pvtDataSet.Tables["ReturnValues"].Rows[0]["USER_NO"]);

                                            objParm[1] = this.txtUserId.Text.Trim().ToUpper();

                                            if (AppDomain.CurrentDomain.GetData("NewPassword") != null)
                                            {
                                                objParm[2] = AppDomain.CurrentDomain.GetData("NewPassword");
                                            }
                                            else
                                            {
                                                objParm[2] = this.txtPassword.Text.Trim().ToUpper();
                                            }

                                            //Access Ind
                                            objParm[3] = pvtDataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"].ToString();

                                            string strCompanies = "";

                                            if (pvtDataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"].ToString() != "S")
                                            {
                                                for (int intRow = 0; intRow < pvtDataSet.Tables["CompanyAccess"].Rows.Count; intRow++)
                                                {
                                                    if (intRow == 0)
                                                    {
                                                        strCompanies = pvtDataSet.Tables["CompanyAccess"].Rows[intRow]["COMPANY_NO"].ToString();

                                                    }
                                                    else
                                                    {
                                                        strCompanies += "#" + pvtDataSet.Tables["CompanyAccess"].Rows[intRow]["COMPANY_NO"].ToString();
                                                    }
                                                }
                                            }

                                            objParm[4] = strCompanies;

                                            clsISClientUtilities.DynamicFunction("Update_New_User_Details_From_Internet", objParm, false);
                                        }

                                        Close_SplasScreen_And_Show_Main();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception eException)
            {
                this.Cursor = Cursors.Default;

                this.txtUserId.Enabled = true;
                this.txtPassword.Enabled = true;
                this.btnOK.Enabled = true;

                if (pvtblnLocalWebService == true)
                {
                    clsISClientUtilities.ErrorHandler(eException);
                }
                else
                {
                    clsISUtilities.ErrorHandler(eException);
                }

                //this.Close();
            }
        }

        private void Create_DataTable_Client_IntegratedSecurity(string parstrQry, DataSet parDataSet, string parstrSourceTable)
        {
            pvtSqlConnectionClientIntegratedSecurity = new SqlConnection(pvtstrConnectionClientIntegratedSecurity);

            pvtSqlCommandClientIntegratedSecurity = new SqlCommand(parstrQry, pvtSqlConnectionClientIntegratedSecurity);

            pvtSqlDataAdapterClientIntegratedSecurity = new SqlDataAdapter(pvtSqlCommandClientIntegratedSecurity);

            //Opens and Closes the Connection object - pvtSqlConnection
            pvtSqlDataAdapterClientIntegratedSecurity.Fill(parDataSet, parstrSourceTable);

            parDataSet.AcceptChanges();
        }

        private void Execute_SQLCommand_CreateDB(string parstrQry)
        {
            pvtSqlConnectionClientIntegratedSecurity = new SqlConnection(pvtstrConnectionClientIntegratedSecurity);

            pvtSqlCommandClientIntegratedSecurity = new SqlCommand(parstrQry, pvtSqlConnectionClientIntegratedSecurity);

            pvtSqlCommandClientIntegratedSecurity.Connection.Open();

            pvtSqlCommandClientIntegratedSecurity.ExecuteNonQuery();

            pvtSqlConnectionClientIntegratedSecurity.Close();
        }

        // Close the splash window and show the main window
        private void Close_SplasScreen_And_Show_Main()
        {
            try
            {
                //SET ALL GLOBAL VARIABLES HERE
                AppDomain.CurrentDomain.SetData("UserNo", pvtint64UserNo);
                AppDomain.CurrentDomain.SetData("AccessInd", pvtstrAccessInd);

                AppDomain.CurrentDomain.SetData("CurrentForm", "");
                AppDomain.CurrentDomain.SetData("DataSet", this.pvtDataSet);

                try
                {
                    string strPath = "";
                    string strBasePath = "";

                    strPath = AppDomain.CurrentDomain.BaseDirectory + "TimeAttendanceMain.dll";
                    strBasePath = AppDomain.CurrentDomain.BaseDirectory;

                    this.Hide();

                    AppDomainSetup myDomainInfo = new AppDomainSetup();
                    myDomainInfo.ApplicationBase = @strBasePath;

                    // Creates the application domain.
                    AppDomain mydomain = AppDomain.CreateDomain("TimeAttendanceMain", null, myDomainInfo);

                    Assembly myAssembly = Assembly.LoadFile(@strPath);
                    System.Windows.Forms.Form frm = (System.Windows.Forms.Form)myAssembly.CreateInstance("InteractPayrollClient.frmTimeAttendanceMain");
                    frm.ShowDialog();
                    frm.Dispose();
                    AppDomain.Unload(mydomain);
                    this.Close();
                }
                catch (System.Exception ex)
                {
                    InteractPayrollClient.CustomClientMessageBox frmMessageBox = new CustomClientMessageBox("Late Binding Error " + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    frmMessageBox.ShowDialog();
                }
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            AppDomain.CurrentDomain.SetData("KillApp","Y");

            this.Close();
        }

        private void btnSetup_Click(object sender, EventArgs e)
        {
            frmConnectionSetup frmConnectionSetup = new frmConnectionSetup();

            frmConnectionSetup.ShowDialog();

            if (this.txtIPAddressPortNo.Text != AppDomain.CurrentDomain.GetData("URLClientPath").ToString())
            {
                this.txtIPAddressPortNo.Text = AppDomain.CurrentDomain.GetData("URLClientPath").ToString();

                clsISClientUtilities = null;
                clsISClientUtilities = new clsISClientUtilities(null, "busTimeAttendanceLogon");

                if (AppDomain.CurrentDomain.GetData("URLClientPath").ToString() != "")
                {
                    int intReturnCode = clsISClientUtilities.WebService_Ping_Test();

                    if (intReturnCode == 0)
                    {
                        this.txtUserId.Enabled = true;
                        this.txtPassword.Enabled = true;

                        this.btnOK.Enabled = true;
                    }
                    else
                    {
                        this.txtUserId.Enabled = false;
                        this.txtPassword.Enabled = false;

                        this.btnOK.Enabled = false;
                    }
                }
            }
        }

        private void rbnLocal_Click(object sender, EventArgs e)
        {
            lblGlobe.Visible = false;
        }

        private void rbnInternet_Click(object sender, EventArgs e)
        {
            lblGlobe.Visible = true;
        }

        private void btnCancel_MouseEnter(object sender, EventArgs e)
        {
            csrSavedCusor = this.Cursor;

            this.Cursor = Cursors.Default;
        }

        private void btnCancel_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = csrSavedCusor;
        }

        private void btnHeaderClose_Click(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.SetData("KillApp", "Y");

            this.Close();
        }
    }
}
