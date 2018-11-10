using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InteractPayrollClient;
using DPUruNet;
using System.IO;

namespace InteractPayroll
{
    public partial class frmUser : Form
    {
        clsISUtilities clsISUtilities;
        clsReadersToDp clsReadersToDp;

        ColorPalette greyScalePalette;

        private ReaderCollection ReaderCollection;
        private Reader pvtCurrentReader;

        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;
        private DataView pvtUserDataView;
        private DataView pvtCompanyAccessDataView;
        private DataRowView pvtDataRowView;

        private DataView pvtUserFingerTemplateDataView;

        private Int64 pvtint64UserNo = -1;
        
        //user Spreadsheet
        private int pvtintUserIdCol = 1;
        private int pvtintUserSurnameCol = 2;
        private int pvtintUserNameCol = 3;
        private int pvtintUserLastTimeOnCol = 4;
        private int pvtintUserLastTimeYYYYMMDDOnCol = 5;
        private int pvtintUserNoRow = 6;

        private int pvtintReturnCode;
        private byte[] pvtbytCompress;

        private bool pvtblnUserDataGridViewLoaded = false;
        private bool pvtblnCompanyDataGridViewLoaded = false;
        private bool pvtblnChosenCompanyDataGridViewLoaded = false;

        private int pvtintUserDataGridViewRowIndex = -1;
        private int pvtintCompanyDataGridViewRowIndex = -1;
        private int pvtintChosenCompanyDataGridViewRowIndex = -1;

        string pvtstrFileName = "FingerprintReaderChoice.txt";

        private string pvtstrFingerDescription;

        string pvtstrFingerprintReaderName = "None";

        private string pvtstrInitialMessage = "To begin, place and hold your #FINGER# finger on the Fingerprint Reader until the screen indicates that the scan was successful. Repeat for each of the remaining scans.";
        private string pvtstrSuccessful = "The Scan was successful.\nPlace your #FINGER# finger on the Fingerprint Reader again.";
        private string pvtstrScanDifferent = "The finger scanned is NOT the same as the previous one or the Image is of BAD Quality. Try again. Place your #FINGER# finger flat on the fingerprint reader.";

        private bool pvtblnFingerprintDeviceOpened = false;
        
        private PictureBox pvtpicFinger;
        private int pvtintFingerNo;
        private int pvtintCurrentFingerCount;

        private DataRow pvtTemplateDataRow;

        private byte[] pvtbytFinger1;
        private byte[] pvtbytFinger2;
        private byte[] pvtbytFinger3;

        private byte[] pvtbyteArrayPreviousTemplate;

        private string pvtstrFingerprintDeviceOpenedFailureMessage = "";

        DataGridViewCellStyle NoTemplateDataGridViewCellStyle;
        DataGridViewCellStyle HasTemplateDataGridViewCellStyle;

        public frmUser()
        {
            InitializeComponent();

            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 114;
                this.tabControl.Top += 114;
                this.dgvUserDataGridView.Height += 114;
                
                this.grbFilter.Top += 114;
                this.grbLegend.Top += 114;
            }
        }

        private void frmUser_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this,"busUser");

                //2017-06-06 Create Greyscale Palette
                Bitmap bmpPalette = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);

                greyScalePalette = bmpPalette.Palette;
                for (int i = 0; i < bmpPalette.Palette.Entries.Length; i++)
                {
                    greyScalePalette.Entries[i] = Color.FromArgb(i, i, i);
                }

                FileInfo fiFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + pvtstrFileName);

                if (fiFileInfo.Exists == true)
                {
                    StreamReader srStreamReader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + pvtstrFileName);

                    pvtstrFingerprintReaderName = srStreamReader.ReadLine();

                    srStreamReader.Close();
                }
                else
                {
                    pvtstrFingerprintReaderName = "None";
                }

                this.lblReader.Text = pvtstrFingerprintReaderName;

                pvtblnFingerprintDeviceOpened = false;

                switch (pvtstrFingerprintReaderName)
                {
                    case "Curve / Columbo (Integrated Biometrics)":

                        clsReadersToDp = new InteractPayrollClient.clsReadersToDp(this.picMainFinger, OnCaptured);

                        //2.Open Device
                        //Open Curve Fingerprint Reader
                        int intReturnCode = clsReadersToDp.OpenDevice();

                        if (intReturnCode == 0)
                        {
                            pvtblnFingerprintDeviceOpened = true;
                        }
                        else
                        {
                            pvtstrFingerprintDeviceOpenedFailureMessage = "Failed to Open 'Curve / Columbo' Fingerprint Reader.";

                            this.Timer.Enabled = true;
                        }

                        break;

                    case "URU4500 (Digital Persona)":

                        //DP Readers attached to Machine
                        try
                        {
                            ReaderCollection = ReaderCollection.GetReaders();
                            
                            if (ReaderCollection.Count > 0)
                            {
                                //Get First Reader
                                pvtCurrentReader = ReaderCollection[0];

                                Constants.ResultCode ResultCode = Constants.ResultCode.DP_DEVICE_FAILURE;
                                
                                //Open First Reader
                                ResultCode = pvtCurrentReader.Open(Constants.CapturePriority.DP_PRIORITY_COOPERATIVE);

                                if (ResultCode == Constants.ResultCode.DP_SUCCESS)
                                {
                                    ResultCode = pvtCurrentReader.CaptureAsync(Constants.Formats.Fid.ANSI, Constants.CaptureProcessing.DP_IMG_PROC_DEFAULT, 500);

                                    if (ResultCode == Constants.ResultCode.DP_SUCCESS)
                                    {
                                        pvtblnFingerprintDeviceOpened = true;
                                    }
                                    else
                                    {
                                        pvtstrFingerprintDeviceOpenedFailureMessage = "Failed to Open 'URU4500' Fingerprint Reader.";

                                        this.Timer.Enabled = true;
                                    }
                                }
                                else
                                {
                                    pvtstrFingerprintDeviceOpenedFailureMessage = "Failed to Open 'URU4500' Fingerprint Reader.";

                                    this.Timer.Enabled = true;
                                }
                            }
                            else
                            {
                                pvtstrFingerprintDeviceOpenedFailureMessage = "Failed to Open 'URU4500' Fingerprint Reader.";

                                this.Timer.Enabled = true;
                            }
                        }
                         catch(Exception ex)
                        {
                            pvtstrFingerprintDeviceOpenedFailureMessage = "Failed to Open 'URU4500' Fingerprint Reader.";

                            this.Timer.Enabled = true;
                        }

                        break;

                    default:

                        break;
                }

                if (pvtblnFingerprintDeviceOpened == false)
                {
                    this.lblMessage.Text = "You may delete an enrolled finger by clicking on the highlighted finger or by Keying the finger's Number.";
                }

                this.lblUserSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblCompanySpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblChosenCompanySpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                HasTemplateDataGridViewCellStyle = new DataGridViewCellStyle();
                HasTemplateDataGridViewCellStyle.BackColor = Color.Aquamarine;
                HasTemplateDataGridViewCellStyle.SelectionBackColor = Color.Aquamarine;

                NoTemplateDataGridViewCellStyle = new DataGridViewCellStyle();
                NoTemplateDataGridViewCellStyle.BackColor = Color.Salmon;
                NoTemplateDataGridViewCellStyle.SelectionBackColor = Color.Salmon;

                pvtDataSet = new DataSet();

                object[] objParm = new object[2];
                objParm[0] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                if (AppDomain.CurrentDomain.GetData("AccessInd").ToString() == "S")
                {
                    this.grbLock.Visible = true;
                }

                Load_CurrentForm_Records();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.Timer.Enabled = false;

            CustomMessageBox.Show(pvtstrFingerprintDeviceOpenedFailureMessage, "Fingerprint Open Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void Load_CurrentForm_Records()
        {
            try
            {
                pvtUserDataView = null;
                pvtUserDataView = new DataView(pvtDataSet.Tables["User"],
                    "",
                    "SURNAME",
                    DataViewRowState.CurrentRows);

                clsISUtilities.DataViewIndex = 0;

                if (clsISUtilities.DataBind_Form_And_DataView_To_Class() == false)
                {
                    clsISUtilities.DataBind_DataView_And_Index(this, pvtUserDataView, "USER_NO");

                    clsISUtilities.DataBind_DataView_To_TextBox(this.txtUser, "USER_ID",false,"",false);
                    clsISUtilities.DataBind_Control_Required_If_Enabled(this.txtUser,"Enter User Id.");
                   
                    clsISUtilities.DataBind_DataView_To_TextBox(this.txtName, "FIRSTNAME", true, "Enter Name.", true);
                    clsISUtilities.DataBind_DataView_To_TextBox(this.txtSurname, "SURNAME", true, "Enter Surname.", true);

                    clsISUtilities.DataBind_DataView_To_TextBox(this.txtEmail, "EMAIL", false, "",true);

                    clsISUtilities.DataBind_DataView_To_RadioButton(this.rbnLockYes, "LOCK_IND","Y");
                    clsISUtilities.DataBind_DataView_To_RadioButton(this.rbnLockNo, "LOCK_IND", "N");

                    clsISUtilities.DataBind_RadioButton_Default(this.rbnLockNo);

                    clsISUtilities.DataBind_DataView_To_Numeric_TextBox_Min_Length(this.txtClockPin, "USER_CLOCK_PIN", 0, 6, false, "Clock Pin", true, 0, true);
                }

                this.Clear_DataGridView(this.dgvUserDataGridView);

                this.pvtblnUserDataGridViewLoaded = false;

                int intCurrentRow = 0;
                string strLastTimeOn = "";
                string strLastTimeOnYYYYMMDD = "";

                if (pvtUserDataView.Count > 0)
                {
                    for (int intRow = 0; intRow < pvtUserDataView.Count; intRow++)
                    {
                        if (pvtUserDataView[intRow]["LAST_TIME_ON"] == System.DBNull.Value)
                        {
                            strLastTimeOn = "";
                            strLastTimeOnYYYYMMDD = "";
                        }
                        else
                        {
                            strLastTimeOn = Convert.ToDateTime(pvtUserDataView[intRow]["LAST_TIME_ON"]).ToString("dd MMM yyyy - HH:mm");
                            strLastTimeOnYYYYMMDD = Convert.ToDateTime(pvtUserDataView[intRow]["LAST_TIME_ON"]).ToString("yyyyMMddHHmm");
                        }

                        //Set Finger Colours
                        pvtUserFingerTemplateDataView = null;
                        pvtUserFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["UserFingerTemplate"], "USER_NO = " + pvtUserDataView[intRow]["USER_NO"].ToString(), "FINGER_NO", DataViewRowState.CurrentRows);

                        if (this.rbnTemplateMissing.Checked == true)
                        {
                            if (pvtUserFingerTemplateDataView.Count > 0)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (this.rbnEnrolledFingerprint.Checked == true)
                            {
                                if (pvtUserFingerTemplateDataView.Count == 0)
                                {
                                    continue;
                                }
                            }
                        }

                        this.dgvUserDataGridView.Rows.Add("",
                                                          pvtUserDataView[intRow]["USER_ID"].ToString(),
                                                          pvtUserDataView[intRow]["SURNAME"].ToString(),
                                                          pvtUserDataView[intRow]["FIRSTNAME"].ToString(),
                                                          strLastTimeOn,
                                                          strLastTimeOnYYYYMMDD,
                                                          intRow.ToString());


                    
                        if (pvtUserFingerTemplateDataView.Count == 0)
                        {
                            this.dgvUserDataGridView[0, dgvUserDataGridView.Rows.Count - 1].Style = this.NoTemplateDataGridViewCellStyle;
                        }
                        else
                        {
                            this.dgvUserDataGridView[0, dgvUserDataGridView.Rows.Count - 1].Style = this.HasTemplateDataGridViewCellStyle;
                        }

                        if (Convert.ToInt64(pvtUserDataView[intRow]["USER_NO"]) == pvtint64UserNo)
                        {
                            intCurrentRow = intRow;
                        }
                    }

                    this.btnUpdate.Enabled = true;
                    this.btnDelete.Enabled = true;
                }
                else
                {
                    this.Clear_DataGridView(this.dgvCompanyDataGridView);
                    this.Clear_DataGridView(this.dgvChosenCompanyDataGridView);
                    
                    this.rbnAdministrator.Checked = false;
                    this.rbnUser.Checked = false;

                    this.btnUpdate.Enabled = false;
                    this.btnDelete.Enabled = false;
                }

                this.pvtblnUserDataGridViewLoaded = true;

                if (this.dgvUserDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvUserDataGridView,intCurrentRow);
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        public int Get_DP_Template(int TemplateNo, byte[] bytPreviousFingerTemplate, byte[] bytCurrentFingeTemplate, ref byte[] bytEnrollmentFingerTemplate)
        {
            int intReturnCode = 0;

            DPUruNet.Fmd fmdPreviousFingerTemplate = DPUruNet.Importer.ImportFmd(bytPreviousFingerTemplate, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data;
            DPUruNet.Fmd fmdCurrentFingerTemplate = DPUruNet.Importer.ImportFmd(bytCurrentFingeTemplate, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data;

            //Compare Template
            CompareResult cmrCompareResult = Comparison.Compare(fmdCurrentFingerTemplate, 0, fmdPreviousFingerTemplate, 0);

            if (cmrCompareResult.ResultCode == Constants.ResultCode.DP_SUCCESS)
            {
                int PROBABILITY_ONE = 0x7fffffff;
                int intFarRequested = PROBABILITY_ONE / 10000;

                if (cmrCompareResult.Score < intFarRequested)
                {
                    if (TemplateNo == 4)
                    {
                        List<Fmd> preEnrollmentFmds = new List<Fmd>();

                        //Add Current Template
                        preEnrollmentFmds.Add(fmdCurrentFingerTemplate);

                        DPUruNet.Fmd fmdFingerTemplate = DPUruNet.Importer.ImportFmd(pvtbytFinger1, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data;
                        preEnrollmentFmds.Add(fmdFingerTemplate);
                        fmdFingerTemplate = DPUruNet.Importer.ImportFmd(pvtbytFinger2, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data;
                        preEnrollmentFmds.Add(fmdFingerTemplate);
                        fmdFingerTemplate = DPUruNet.Importer.ImportFmd(pvtbytFinger3, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data;
                        preEnrollmentFmds.Add(fmdFingerTemplate);

                        DataResult<Fmd> drResultEnrollment = DPUruNet.Enrollment.CreateEnrollmentFmd(Constants.Formats.Fmd.ANSI, preEnrollmentFmds);

                        if (drResultEnrollment.ResultCode == Constants.ResultCode.DP_SUCCESS)
                        {
                            bytEnrollmentFingerTemplate = drResultEnrollment.Data.Bytes;
                        }
                        else
                        {
                            intReturnCode = 2;
                        }
                    }
                    else
                    {
                        bytEnrollmentFingerTemplate = bytCurrentFingeTemplate;
                    }
                }
                else
                {
                    intReturnCode = 1;
                }
            }
            else
            {
                intReturnCode = 1;
            }

            return intReturnCode;
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.btnUpdate.Enabled == false
            && this.btnSave.Enabled == false
            && this.tabControl.SelectedIndex == 0)
            {
                this.btnSave.Enabled = true;
             
                this.pnlEnroll.Visible = false;
                this.pnlFingers.Visible = true;
            }
        }
        public int Get_DataGridView_SelectedRowIndex(DataGridView myDataGridView)
        {
            int intReturnIndex = -1;

            if (myDataGridView.SelectedRows.Count > 0)
            {
                if (myDataGridView.SelectedRows[0].Selected == true)
                {
                    intReturnIndex = myDataGridView.SelectedRows[0].Index;
                }
            }
            else
            {
                if (myDataGridView.SelectionMode == DataGridViewSelectionMode.CellSelect)
                {
                    intReturnIndex = myDataGridView.CurrentCell.RowIndex;
                }
            }

            return intReturnIndex;
        }

        public void Set_DataGridView_SelectedRowIndex(DataGridView myDataGridView, int intRow)
        {
            //Fires DataGridView RowEnter Function
            if (this.Get_DataGridView_SelectedRowIndex(myDataGridView) == intRow)
            {
                DataGridViewCellEventArgs myDataGridViewCellEventArgs = new DataGridViewCellEventArgs(0, intRow);

                switch (myDataGridView.Name)
                {
                    case "dgvUserDataGridView":

                        pvtintUserDataGridViewRowIndex = -1;
                        this.dgvUserDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvCompanyDataGridView":

                        pvtintCompanyDataGridViewRowIndex = -1;
                        this.dgvCompanyDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvChosenCompanyDataGridView":

                        pvtintChosenCompanyDataGridViewRowIndex = -1;
                        this.dgvChosenCompanyDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    default:

                        MessageBox.Show("No Entry for " + myDataGridView.Name + " in Set_DataGridView_SelectedRowIndex", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            else
            {
                myDataGridView.CurrentCell = myDataGridView[0, intRow];
            }
        }

        private void Clear_DataGridView(DataGridView myDataGridView)
        {
            myDataGridView.Rows.Clear();

            if (myDataGridView.SortedColumn != null)
            {
                myDataGridView.SortedColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
            }
        }

        private void DataGridView_Sorted(object sender, EventArgs e)
        {
            DataGridView myDataGridView = (DataGridView)sender;

            if (myDataGridView.Rows.Count > 0)
            {
                if (myDataGridView.SelectedRows.Count > 0)
                {
                    if (myDataGridView.SelectedRows[0].Selected == true)
                    {
                        myDataGridView.FirstDisplayedScrollingRowIndex = myDataGridView.SelectedRows[0].Index;
                    }
                }
            }
        }

        private void btnNew_Click(object sender, System.EventArgs e)
        {
            this.Text = this.Text + " - New";

            pvtDataRowView = this.pvtUserDataView.AddNew();

            pvtDataRowView.BeginEdit();

            pvtDataRowView["USER_NO"] = -1;
            pvtDataRowView["USER_ID"] = "";
            pvtDataRowView["FIRSTNAME"] = "";
            pvtDataRowView["SURNAME"] = "";
            pvtDataRowView["EMAIL"] = "";
            
            pvtDataRowView.EndEdit();
         
            pvtint64UserNo = -1;

            //Set Index to End of View
            clsISUtilities.DataViewIndex = 0;
         
            pvtCompanyAccessDataView = null;
            pvtCompanyAccessDataView = new DataView(pvtDataSet.Tables["CompanyAccess"],
                "USER_NO = -1",
                "",
                DataViewRowState.CurrentRows);

            pvtUserFingerTemplateDataView = null;
            pvtUserFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["UserFingerTemplate"], 
                "USER_NO = -1", 
                "FINGER_NO", 
                DataViewRowState.CurrentRows);

            Set_Form_For_Edit();

            this.Draw_Current_User_Fingers();

            this.txtUser.Focus();
        }

        public void btnUpdate_Click(object sender, System.EventArgs e)
        {
            this.Text += " - Update";

            Set_Form_For_Edit();

            this.txtName.Focus();
        }

        public void btnDelete_Click(object sender, System.EventArgs e)
        {
            try
            {
                DialogResult dlgResult = CustomMessageBox.Show("Delete User '" + pvtUserDataView[clsISUtilities.DataViewIndex]["FIRSTNAME"].ToString() + " " + pvtUserDataView[clsISUtilities.DataViewIndex]["SURNAME"].ToString() + "'",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation);

                if (dlgResult == DialogResult.Yes)
                {
                    object[] objParm = new object[2];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[1] = pvtint64UserNo;

                    clsISUtilities.DynamicFunction("Delete_Record", objParm, true);

                    //Remove Table Row
                    pvtUserDataView.Delete(clsISUtilities.DataViewIndex);

                    pvtDataSet.Tables["User"].AcceptChanges();

                    pvtint64UserNo = -1;

                    this.Load_CurrentForm_Records();
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        public void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                //DataLayer Fields are Checked
                pvtintReturnCode = this.clsISUtilities.DataBind_Save_Check();

                if (pvtintReturnCode != 0)
                {
                    return;
                }

                pvtintReturnCode = Save_Check();

                if (pvtintReturnCode != 0)
                {
                    return;
                }

                pvtTempDataSet = null;
                pvtTempDataSet = new DataSet();

                pvtTempDataSet.Tables.Add(this.pvtDataSet.Tables["User"].Clone());
                pvtTempDataSet.Tables.Add(this.pvtDataSet.Tables["CompanyAccess"].Clone());

                DataView UserDataView = new DataView(pvtDataSet.Tables["User"],
                    "",
                    "",
                    DataViewRowState.Added | DataViewRowState.ModifiedCurrent | DataViewRowState.Deleted);

                if (UserDataView.Count > 0)
                {
                    pvtTempDataSet.Tables["User"].ImportRow(UserDataView[0].Row);
                }
                
                DataView CompanyAccessDataView = new DataView(pvtDataSet.Tables["CompanyAccess"],
                    "",
                    "",
                    DataViewRowState.Added | DataViewRowState.ModifiedCurrent | DataViewRowState.Deleted);

                for (int intRow = 0; intRow < CompanyAccessDataView.Count; intRow++)
                {
                    pvtTempDataSet.Tables["CompanyAccess"].ImportRow(CompanyAccessDataView[intRow].Row);
                }

                //Add EmployeeFingerTemplate Table 
                pvtTempDataSet.Tables.Add(pvtDataSet.Tables["UserFingerTemplate"].Clone());
                
                DataView UserFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["UserFingerTemplate"], "USER_NO = " + this.pvtint64UserNo, "FINGER_NO", DataViewRowState.Added | DataViewRowState.Deleted);

                for (int intRow = 0; intRow < UserFingerTemplateDataView.Count; intRow++)
                {
                     pvtTempDataSet.Tables["UserFingerTemplate"].ImportRow(UserFingerTemplateDataView[intRow].Row);
                }

                //Compress DataSet
                pvtbytCompress = clsISUtilities.Compress_DataSet(pvtTempDataSet);

                if (this.Text.IndexOf(" - New", 0) > 0)
                {
                    object[] objParm = new object[2];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[1] = pvtbytCompress;

                    pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Insert_New_Record", objParm, true);

                    pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                    if (pvtTempDataSet.Tables["User"].Rows[0]["RETURN_CODE"].ToString() == "9999")
                    {
                        CustomMessageBox.Show("User '" + this.txtUser.Text.Trim() + "' already Exists",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);

                        return;
                    }

                    if (pvtTempDataSet.Tables["UserPinCheck"] != null)
                    {
                        if (pvtTempDataSet.Tables["UserPinCheck"].Rows.Count > 0)
                        {
                            CustomMessageBox.Show("Clock Pin is Invalid\n\nChange Clock Pin",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                            this.txtClockPin.Focus();

                            return;
                        }
                    }

                    //Get User No For Repositioning
                    pvtint64UserNo = Convert.ToInt64(pvtTempDataSet.Tables["User"].Rows[0]["USER_NO"].ToString());

                    for (int intRow = 0; intRow < UserFingerTemplateDataView.Count; intRow++)
                    {
                        if (UserFingerTemplateDataView[intRow].Row.RowState == DataRowState.Added)
                        {
                            UserFingerTemplateDataView[intRow]["USER_NO"] = pvtint64UserNo;

                            intRow -= 1;
                        }
                    }

                    //Delete (New Record is Passed back from Server)
                    pvtUserDataView[clsISUtilities.DataViewIndex].Row.Delete();

                    //Relink to New User
                    pvtUserFingerTemplateDataView = null;
                    pvtUserFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["UserFingerTemplate"], "USER_NO = " + pvtint64UserNo.ToString(), "FINGER_NO", DataViewRowState.CurrentRows);
                }
                else
                {
                    object[] objParm = new object[3];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[1] = pvtint64UserNo;
                    objParm[2] = pvtbytCompress;

                    pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Update_Record", objParm, true);

                    pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                    if (pvtTempDataSet.Tables["UserPinCheck"] != null)
                    {
                        if (pvtTempDataSet.Tables["UserPinCheck"].Rows.Count > 0)
                        {
                            CustomMessageBox.Show("Clock Pin is Invalid\n\nChange Clock Pin",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                            this.txtClockPin.Focus();

                            return;
                        }
                    }
                }
                
                //Delete Added Rows (New Record is Passed back from Server)
                for (int intRow = 0; intRow < pvtCompanyAccessDataView.Count; intRow++)
                {
                    if (pvtCompanyAccessDataView[intRow].Row.RowState == DataRowState.Added)
                    {
                        pvtCompanyAccessDataView[intRow].Delete();

                        intRow -= 1;
                    }
                }

                //Merge New Records with Correct TIE_BREAKER field
                pvtDataSet.Merge(pvtTempDataSet);

                pvtDataSet.AcceptChanges();

                if (this.Text.IndexOf(" - New", 0) > 0)
                {
                    CustomMessageBox.Show("Passord is '" + pvtTempDataSet.Tables["User"].Rows[0]["PASSWORD"].ToString() + "'",this.Text,MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
                
                btnCancel_Click(sender, e);
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        public void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.pvtDataSet.RejectChanges();

            this.Set_Form_For_Read();

            Load_CurrentForm_Records();
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void DP_StartCapture()
        {
            pvtCurrentReader.On_Captured -= new Reader.CaptureCallback(OnCaptured);
            pvtCurrentReader.On_Captured += new Reader.CaptureCallback(OnCaptured);
        }

        private void DP_StopCapture()
        {
            pvtCurrentReader.On_Captured -= new Reader.CaptureCallback(OnCaptured);
        }

        private delegate void OnCaptured_ThreadSafe(CaptureResult captureResult);
        public void OnCaptured(CaptureResult captureResult)
        {
            if (this.InvokeRequired == true)
            {
                //Message on Different Thread - Recall function on Form's Thread
                this.Invoke(new OnCaptured_ThreadSafe(OnCaptured), new object[] { captureResult });
            }
            else
            {
                System.Console.Beep();
                this.lblEnrollMessage.Text = "";

                byte[] bytFingerTemplate = null;

                if (pvtstrFingerprintReaderName == "URU4500 (Digital Persona)")
                {
                    DP_StopCapture();
                }

                string strOK = "Y";
                string strEnrollInd = "";

                //Needs To Be Here - Falls Over If Very Bad Image
                try
                {
                    if (captureResult.ResultCode == Constants.ResultCode.DP_SUCCESS)
                    {
                        if (captureResult.Quality == Constants.CaptureQuality.DP_QUALITY_GOOD)
                        {
                            //Needs To Be Here - Falls Over If Very Bad Image
                            try
                            {
                                bytFingerTemplate = FeatureExtraction.CreateFmdFromFid(captureResult.Data, Constants.Formats.Fmd.ANSI).Data.Bytes;
                            }
                            catch
                            {
                                //Bad Image
                                strOK = "B";
                                goto OnComplete_Continue;
                            }
                        }
                        else
                        {
                            //Bad Image
                            strOK = "B";
                            goto OnComplete_Continue;
                        }
                    }
                    else
                    {
                        //Bad Image
                        strOK = "B";
                        goto OnComplete_Continue;
                    }
                }
                catch
                {
                    //Bad Image
                    strOK = "B";
                    goto OnComplete_Continue;
                }

                if (pvtintCurrentFingerCount == 1)
                {
                    pvtbytFinger1 = null;
                    pvtbytFinger2 = null;
                    pvtbytFinger3 = null;
                }
                else
                {
                    //Get First FingerPrint Template for Compare
                    pvtbyteArrayPreviousTemplate = pvtbytFinger1;
                }

                int intReturnCode = 0;
                byte[] bytExtractedTemplate = null;

                if (pvtintCurrentFingerCount == 1)
                {
                    bytExtractedTemplate = bytFingerTemplate;
                }
                else
                {
                    intReturnCode = Get_DP_Template(pvtintCurrentFingerCount, pvtbyteArrayPreviousTemplate, bytFingerTemplate, ref bytExtractedTemplate);
                }

                if (intReturnCode == 0)
                {
                    if (pvtintCurrentFingerCount == 4)
                    {
                        strEnrollInd = "Y";

                        pvtTemplateDataRow = null;
                        pvtTemplateDataRow = pvtDataSet.Tables["UserFingerTemplate"].NewRow();

                        pvtTemplateDataRow["USER_NO"] = this.pvtint64UserNo;
                        pvtTemplateDataRow["FINGER_NO"] = pvtintFingerNo;
                        pvtTemplateDataRow["FINGER_TEMPLATE"] = bytExtractedTemplate;

                        pvtDataSet.Tables["UserFingerTemplate"].Rows.Add(pvtTemplateDataRow);
                    }
                    else
                    {
                        if (pvtintCurrentFingerCount == 1)
                        {

                            pvtbytFinger1 = bytExtractedTemplate;
                        }
                        else
                        {
                            if (pvtintCurrentFingerCount == 2)
                            {
                                pvtbytFinger2 = bytExtractedTemplate;
                            }
                            else
                            {
                                pvtbytFinger3 = bytExtractedTemplate;
                            }
                        }
                    }
                }
                else
                {
                    if (intReturnCode == 2)
                    {
                        strOK = "F";
                    }
                    else
                    {
                        strOK = "N";
                    }
                }

                if (pvtstrFingerprintReaderName == "URU4500 (Digital Persona)")
                {
                    if (this.picMainFinger.Image != null)
                    {
                        this.picMainFinger.Image = null;
                    }

                    Bitmap bmp = new Bitmap(captureResult.Data.Views[0].Width, captureResult.Data.Views[0].Height, PixelFormat.Format8bppIndexed);

                    //Set Palette to GreyScale
                    bmp.Palette = greyScalePalette;

                    BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

                    for (int i = 0; i <= bmp.Height - 1; i++)
                    {
                        IntPtr p = new IntPtr(data.Scan0.ToInt64() + data.Stride * i);
                        System.Runtime.InteropServices.Marshal.Copy(captureResult.Data.Bytes, (i * bmp.Width) + 50, p, bmp.Width);
                    }

                    bmp.UnlockBits(data);

                    //Remove White Space From Around Image
                    Rectangle recRectangle = new Rectangle(20, 30, 270, 300);
                    Bitmap bmpCrop = Bitmap.FromHbitmap(bmp.GetHbitmap()).Clone(recRectangle, PixelFormat.Format8bppIndexed);

                    this.picMainFinger.Image = Image.FromHbitmap(bmpCrop.GetHbitmap());

                    bmp.Dispose();
                    bmp = null;
                    bmpCrop.Dispose();
                    bmpCrop = null;
                }

            OnComplete_Continue:

                if (strOK == "B")
                {
                    CustomMessageBox.Show("Bad Image",this.Text,MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
                else
                {
                    if (strOK == "E")
                    {
                        CustomMessageBox.Show("Error on Web Server Layer", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                    }
                    else
                    {
                        if (strOK == "L")
                        {
                            CustomMessageBox.Show("Griaule Licence Problem", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Close();
                        }
                        else
                        {
                            if (strOK == "F")
                            {
                                CustomMessageBox.Show("Enrollment Failure", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.Close();
                            }
                            else
                            {
                                //SuccessFull
                                if (pvtintCurrentFingerCount > 1)
                                {
                                    if (strOK == "Y")
                                    {
                                        this.lblEnrollMessage.ForeColor = Color.Black;
                                        this.lblEnrollMessage.Text = pvtstrSuccessful.Replace("#FINGER#", pvtstrFingerDescription);

                                        switch (pvtintCurrentFingerCount)
                                        {
                                            case 2:

                                                this.picFinger2.Image = global::InteractPayroll.Properties.Resources.FingerPrintCorrect64;
                                                this.picFinger3.Image = global::InteractPayroll.Properties.Resources.FingerPrintQuestion64;

                                                break;

                                            case 3:

                                                this.picFinger3.Image = global::InteractPayroll.Properties.Resources.FingerPrintCorrect64;
                                                this.picFinger4.Image = global::InteractPayroll.Properties.Resources.FingerPrintQuestion64;

                                                break;

                                            case 4:

                                                this.picFinger4.Image = global::InteractPayroll.Properties.Resources.FingerPrintCorrect64;

                                                break;
                                        }
                                    }
                                    else
                                    {
                                        this.lblEnrollMessage.ForeColor = Color.Red;
                                        this.lblEnrollMessage.Text = pvtstrScanDifferent.Replace("#FINGER#", pvtstrFingerDescription);

                                        switch (pvtintCurrentFingerCount)
                                        {
                                            case 2:

                                                this.picFinger2.Image = global::InteractPayroll.Properties.Resources.FingerPrintError64;

                                                break;

                                            case 3:
                                                this.picFinger3.Image = global::InteractPayroll.Properties.Resources.FingerPrintError64;

                                                break;

                                            case 4:

                                                this.picFinger4.Image = global::InteractPayroll.Properties.Resources.FingerPrintError64;

                                                break;
                                        }

                                        pvtintCurrentFingerCount -= 1;
                                    }
                                }
                                else
                                {
                                    if (strOK == "Y")
                                    {
                                        this.lblEnrollMessage.ForeColor = Color.Black;
                                        this.lblEnrollMessage.Text = pvtstrSuccessful.Replace("#FINGER#", pvtstrFingerDescription);

                                        this.picFinger1.Image = global::InteractPayroll.Properties.Resources.FingerPrintCorrect64;
                                        this.picFinger2.Image = global::InteractPayroll.Properties.Resources.FingerPrintQuestion64;
                                    }
                                    else
                                    {
                                        //Bad Image 
                                        this.lblEnrollMessage.ForeColor = Color.Red;
                                        this.lblEnrollMessage.Text = pvtstrScanDifferent.Replace("#FINGER#", pvtstrFingerDescription);

                                        pvtintCurrentFingerCount -= 1;
                                    }
                                }

                                if (pvtintCurrentFingerCount == 4)
                                {
                                    System.Console.Beep();
                                    System.Console.Beep();

                                    this.lblFingerInformation.Text = "";

                                    string strMessage = "";

                                    if (strEnrollInd == "Y")
                                    {
                                        strMessage = "Successful.";

                                        this.dgvUserDataGridView[0, dgvUserDataGridView.CurrentCell.RowIndex].Style = HasTemplateDataGridViewCellStyle;
                                    }
                                    else
                                    {
                                        strMessage = "UNSUCCESSFUL.";
                                    }

                                    CustomMessageBox.Show("Enrollment of " + pvtstrFingerDescription + " finger " + strMessage, this.Text,MessageBoxButtons.OK,MessageBoxIcon.Error);
                                    
                                    this.pnlEnroll.Visible = false;
                                    this.btnClearFingers.Visible = true;
                                    this.pnlFingers.Visible = true;
                                    this.btnSave.Enabled = true;

                                    if (this.Text.IndexOf(" - New") > -1)
                                    {
                                        Draw_Current_User_Fingers();
                                    }
                                    else
                                    {
                                        this.Set_DataGridView_SelectedRowIndex(this.dgvUserDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvUserDataGridView));
                                    }

                                    this.btnUpdate.Enabled = false;
                                }
                                else
                                {
                                    pvtintCurrentFingerCount += 1;

                                    if (pvtstrFingerprintReaderName == "URU4500 (Digital Persona)")
                                    {
                                        DP_StartCapture();
                                    }
                                    else
                                    {
                                        this.lblFingerInformation.Text = "Remove Finger from Reader";
                                        this.lblFingerInformation.ForeColor = Color.Red;
                                        this.lblFingerInformation.Refresh();

                                        clsReadersToDp.StartCapture();

                                        this.lblFingerInformation.Text = "Place Finger on Reader";
                                        this.lblFingerInformation.ForeColor = Color.Black;
                                        this.lblFingerInformation.Refresh();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }

        private int Save_Check()
        {
            if (this.txtUser.Text.Trim().ToUpper() == "INTERACT")
            {
                CustomMessageBox.Show("This User Id. ALREADY Exists.",
                    this.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                this.txtUser.Focus();
                return 1;
            }

            if (this.dgvChosenCompanyDataGridView.Rows.Count == 0)
            {
                CustomMessageBox.Show("Select Company/s.",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                this.btnAdd.Focus();
                return 1;
            }

            if (this.Text.IndexOf(" - New", 0) > 0)
            {
                for (int intIndex = 0; intIndex < this.pvtUserDataView.Count; intIndex++)
                {
                    if (pvtUserDataView[intIndex]["USER_ID"].ToString().ToUpper() == this.txtUser.Text.Trim().ToUpper()
                        & clsISUtilities.DataViewIndex != intIndex)
                    {
                        CustomMessageBox.Show("User '" + pvtUserDataView[intIndex]["USER_ID"].ToString() + "' Already Exists.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        return 1;
                    }
                }
            }

            //2017-05-13
            if (this.txtClockPin.Text != "")
            {
                //46837228=Interact
                //82543483=Validite
                if (this.txtClockPin.Text == "46837228"
                || this.txtClockPin.Text == "82543483")
                {
                    CustomMessageBox.Show("Clock Pin is Invalid\n\nChange Clock Pin",
                          this.Text,
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Error);

                    this.txtClockPin.Focus();

                    return 1;
                }
                
                string strAscendingCharacters = "0123456789";
                string strDescendingCharacters = "9876543210";
                string strFirstCharacter = this.txtClockPin.Text.Substring(0,1);

                if (this.txtClockPin.Text.Length < 10)
                {
                    strAscendingCharacters = strAscendingCharacters.Substring(0, this.txtClockPin.Text.Length);
                    strDescendingCharacters = strDescendingCharacters.Substring(0, this.txtClockPin.Text.Length);
                }

                if (this.txtClockPin.Text == strAscendingCharacters)
                {
                    CustomMessageBox.Show("Ascending Characters NOT allowed for Clock Pin.",
                           this.Text,
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Error);

                    this.txtClockPin.Focus();

                    return 1;
                }

                if (this.txtClockPin.Text == strDescendingCharacters)
                {
                    CustomMessageBox.Show("Descending Characters NOT allowed for Clock Pin.",
                           this.Text,
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Error);

                    this.txtClockPin.Focus();

                    return 1;
                }

                //Check Repeating Character
                for (int intCount = 0; intCount < this.txtClockPin.Text.Length; intCount++)
                {
                    if (this.txtClockPin.Text.Substring(intCount, 1) != strFirstCharacter)
                    {
                        break;
                    }
                    else
                    {
                        if (intCount == this.txtClockPin.Text.Length - 1)
                        {
                            CustomMessageBox.Show("Repeating Characters NOT allowed for Clock Pin.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                            this.txtClockPin.Focus();

                            return 1;
                        }
                    }
                }
            }

            return 0;
        }

        private void lnkCancel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.pnlEnroll.Visible = false;
            this.btnClearFingers.Visible = true;
            this.pnlFingers.Visible = true;
            this.btnSave.Enabled = true;
        }

        private void Set_Form_For_Edit()
        {
            bool blnNew = false;
            
            if (this.Text.IndexOf(" - New", 0) > 0)
            {
                blnNew = true;
                
                this.Clear_DataGridView(this.dgvCompanyDataGridView);
                this.Clear_DataGridView(this.dgvChosenCompanyDataGridView);

                this.pvtblnCompanyDataGridViewLoaded = false;

                for (int intRowCount = 0; intRowCount < pvtDataSet.Tables["Company"].Rows.Count; intRowCount++)
                {
                    this.dgvCompanyDataGridView.Rows.Add(pvtDataSet.Tables["Company"].Rows[intRowCount]["COMPANY_DESC"].ToString(),
                                                         pvtDataSet.Tables["Company"].Rows[intRowCount]["COMPANY_NO"].ToString());
                }

                this.pvtblnCompanyDataGridViewLoaded = true;

                if (this.dgvCompanyDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvCompanyDataGridView, 0);
                }
            }
         
            clsISUtilities.Set_Form_For_Edit(blnNew);
             
            if (blnNew == true)
            {
                this.txtUser.Enabled = true;

                this.rbnAdministrator.Checked = false;
                this.rbnUser.Checked = false;
                this.rbnAdministrator.Enabled = false;
                this.rbnUser.Enabled = false;

                this.rbnInternetClient.Enabled = false;
                this.rbnInternetClient.Checked = false;
                this.rbnInternet.Enabled = false;
                this.rbnInternet.Checked = false;
                this.rbnClient.Enabled = false;
                this.rbnClient.Checked = false;

                this.txtUser.Focus();
            }
            else
            {
                if (this.dgvChosenCompanyDataGridView.Rows.Count > 0)
                {
                    this.rbnAdministrator.Enabled = true;
                    this.rbnUser.Enabled = true;
                    
                    this.rbnInternetClient.Enabled = true;
                    this.rbnInternet.Enabled = true;
                    this.rbnClient.Enabled = true;
                }
            }

            this.rbnNone.Checked = true;

            this.rbnNone.Enabled = false;
            this.rbnTemplateMissing.Enabled = false;
            this.rbnEnrolledFingerprint.Enabled = false;
            
            this.lblMessage.Visible = true;
            this.btnClearFingers.Visible = true;

            this.dgvUserDataGridView.Enabled = false;
            
            this.picUserLock.Visible = true;

            this.btnAdd.Enabled = true;
            this.btnRemove.Enabled = true;
            this.btnRemoveAll.Enabled = true;

            this.btnNew.Enabled = false;
            this.btnUpdate.Enabled = false;
            this.btnDelete.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;
        }

        private void Set_Form_For_Read()
        {
            if (this.Text.IndexOf("- New") > -1)
            {
                this.Text = this.Text.Substring(0, this.Text.IndexOf("- New") - 1);
            }
            else
            {
                if (this.Text.IndexOf("- Update") > -1)
                {
                    this.Text = this.Text.Substring(0, this.Text.IndexOf("- Update") - 1);
                }
            }

            clsISUtilities.Set_Form_For_Read();

            this.picUserLock.Visible = false;

            this.dgvUserDataGridView.Enabled = true;

            this.btnAdd.Enabled = false;
            this.btnRemove.Enabled = false;
            this.btnRemoveAll.Enabled = false;

            this.rbnAdministrator.Enabled = false;
            this.rbnUser.Enabled = false;

            this.rbnInternetClient.Enabled = false;
            this.rbnInternet.Enabled = false;
            this.rbnClient.Enabled = false;

            this.lblMessage.Visible = false;
            this.btnClearFingers.Visible = false;

            this.rbnNone.Enabled = true;
            this.rbnTemplateMissing.Enabled = true;
            this.rbnEnrolledFingerprint.Enabled = true;

            this.pnlEnroll.Visible = false;
            this.pnlFingers.Visible = true;

            this.btnNew.Enabled = true;
            this.btnUpdate.Enabled = true;
            this.btnDelete.Enabled = true;

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;
        }

        private void btnAdd_Click(object sender, System.EventArgs e)
        {
            if (this.dgvCompanyDataGridView.Rows.Count > 0)
            {
                pvtDataRowView = pvtCompanyAccessDataView.AddNew();

                pvtDataRowView.BeginEdit();

                pvtDataRowView["USER_NO"] = pvtint64UserNo;

                pvtDataRowView["COMPANY_NO"] = this.dgvCompanyDataGridView[1,this.Get_DataGridView_SelectedRowIndex(this.dgvCompanyDataGridView)].Value.ToString();
                pvtDataRowView["COMPANY_ACCESS_IND"] = "U";
                pvtDataRowView["ACCESS_LAYER_IND"] = "C";
                pvtDataRowView["TIE_BREAKER"] = 1;

                pvtDataRowView.EndEdit();

                this.pvtblnCompanyDataGridViewLoaded = false;
                this.pvtblnChosenCompanyDataGridViewLoaded = false;

                DataGridViewRow myDataGridViewRow = this.dgvCompanyDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvCompanyDataGridView)];

                this.dgvCompanyDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvChosenCompanyDataGridView.Rows.Add(myDataGridViewRow);

                this.pvtblnCompanyDataGridViewLoaded = true;
                this.pvtblnChosenCompanyDataGridViewLoaded = true;

                this.rbnAdministrator.Enabled = true;
                this.rbnUser.Enabled = true;

                this.rbnInternetClient.Enabled = true;
                this.rbnInternet.Enabled = true;
                this.rbnClient.Enabled = true;

                if (this.dgvCompanyDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvCompanyDataGridView, 0);
                }

                this.Set_DataGridView_SelectedRowIndex(this.dgvChosenCompanyDataGridView, this.dgvChosenCompanyDataGridView.Rows.Count - 1);
            }
        }

        private void btnRemove_Click(object sender, System.EventArgs e)
        {
            if (this.dgvChosenCompanyDataGridView.Rows.Count > 0)
            {
                pvtCompanyAccessDataView = null;
                pvtCompanyAccessDataView = new DataView(pvtDataSet.Tables["CompanyAccess"],
                    "USER_NO = " + pvtint64UserNo + " AND COMPANY_NO = " + this.dgvChosenCompanyDataGridView[1,this.Get_DataGridView_SelectedRowIndex(this.dgvChosenCompanyDataGridView)].Value.ToString(),
                    "",
                    DataViewRowState.CurrentRows);

                this.pvtCompanyAccessDataView[0].Delete();

                this.pvtblnCompanyDataGridViewLoaded = false;
                this.pvtblnChosenCompanyDataGridViewLoaded = false;

                DataGridViewRow myDataGridViewRow = this.dgvChosenCompanyDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvChosenCompanyDataGridView)];

                this.dgvChosenCompanyDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvCompanyDataGridView.Rows.Add(myDataGridViewRow);

                this.pvtblnCompanyDataGridViewLoaded = true;
                this.pvtblnChosenCompanyDataGridViewLoaded = true;


                if (this.dgvChosenCompanyDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvChosenCompanyDataGridView, 0);
                }
                else
                {
                    this.rbnAdministrator.Checked = false;
                    this.rbnUser.Checked = false;
                    this.rbnAdministrator.Enabled = false;
                    this.rbnUser.Enabled = false;

                    this.rbnInternetClient.Enabled = false;
                    this.rbnInternetClient.Checked = false;
                    this.rbnInternet.Enabled = false;
                    this.rbnInternet.Checked = false;
                    this.rbnClient.Enabled = false;
                    this.rbnClient.Checked = false;
                }

                this.Set_DataGridView_SelectedRowIndex(this.dgvCompanyDataGridView, this.dgvCompanyDataGridView.Rows.Count - 1);
            }
        }

        private void Administrator_User_Click(object sender, System.EventArgs e)
        {
            if (this.dgvChosenCompanyDataGridView.Rows.Count > 0)
            {
                pvtCompanyAccessDataView = null;
                pvtCompanyAccessDataView = new DataView(pvtDataSet.Tables["CompanyAccess"],
                    "USER_NO = " + pvtint64UserNo + " AND COMPANY_NO = " + this.dgvChosenCompanyDataGridView[1,this.Get_DataGridView_SelectedRowIndex(this.dgvChosenCompanyDataGridView)].Value.ToString(),
                    "",
                    DataViewRowState.CurrentRows);

                RadioButton rbnRadioButton = (RadioButton)sender;

                if (rbnRadioButton.Name == "rbnUser")
                {
                    pvtCompanyAccessDataView[0]["COMPANY_ACCESS_IND"] = "U";

                    //Client Only
                    pvtCompanyAccessDataView[0]["ACCESS_LAYER_IND"] = "C";

                    this.rbnClient.Checked = true;
                }
                else
                {
                    pvtCompanyAccessDataView[0]["COMPANY_ACCESS_IND"] = "A";

                    //Both
                    pvtCompanyAccessDataView[0]["ACCESS_LAYER_IND"] = "B";

                    this.rbnInternetClient.Checked = true;
                }
            }
        }

        private void dgvUserDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnUserDataGridViewLoaded == true)
            {
                if (this.pvtintUserDataGridViewRowIndex != e.RowIndex)
                {
                    this.pvtintUserDataGridViewRowIndex = e.RowIndex;

                    clsISUtilities.DataViewIndex = Convert.ToInt32(this.dgvUserDataGridView[pvtintUserNoRow, e.RowIndex].Value);

                    //NB clsISUtilities.DataViewIndex is used Below
                    clsISUtilities.DataBind_DataView_Record_Show();
                    
                    pvtint64UserNo = Convert.ToInt64(pvtUserDataView[clsISUtilities.DataViewIndex]["USER_NO"]);

                    //Set Finger Colours
                    pvtUserFingerTemplateDataView = null;
                    pvtUserFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["UserFingerTemplate"], "USER_NO = " + pvtint64UserNo, "FINGER_NO", DataViewRowState.CurrentRows);

                    if (pvtUserFingerTemplateDataView.Count == 0)
                    {
                        this.dgvUserDataGridView[0, e.RowIndex].Style = NoTemplateDataGridViewCellStyle;
                    }
                    else
                    {
                        this.dgvUserDataGridView[0, e.RowIndex].Style = HasTemplateDataGridViewCellStyle;
                    }

                    Draw_Current_User_Fingers();
                    
                    this.Clear_DataGridView(this.dgvCompanyDataGridView);
                    this.Clear_DataGridView(this.dgvChosenCompanyDataGridView);

                    this.pvtblnCompanyDataGridViewLoaded = false;
                    this.pvtblnChosenCompanyDataGridViewLoaded = false;


                    for (int intRowCount = 0; intRowCount < pvtDataSet.Tables["Company"].Rows.Count; intRowCount++)
                    {
                        pvtCompanyAccessDataView = null;
                        pvtCompanyAccessDataView = new DataView(pvtDataSet.Tables["CompanyAccess"],
                            "USER_NO = " + pvtint64UserNo + " AND COMPANY_NO = " + pvtDataSet.Tables["Company"].Rows[intRowCount]["COMPANY_NO"].ToString(),
                            "",
                            DataViewRowState.CurrentRows);

                        if (pvtCompanyAccessDataView.Count > 0)
                        {
                            this.dgvChosenCompanyDataGridView.Rows.Add(pvtDataSet.Tables["Company"].Rows[intRowCount]["COMPANY_DESC"].ToString(),
                                                                       pvtDataSet.Tables["Company"].Rows[intRowCount]["COMPANY_NO"].ToString());
                        }
                        else
                        {
                            this.dgvCompanyDataGridView.Rows.Add(pvtDataSet.Tables["Company"].Rows[intRowCount]["COMPANY_DESC"].ToString(),
                                                                 pvtDataSet.Tables["Company"].Rows[intRowCount]["COMPANY_NO"].ToString());
                        }
                    }

                    this.pvtblnCompanyDataGridViewLoaded = true;
                    this.pvtblnChosenCompanyDataGridViewLoaded = true;

                    if (this.dgvCompanyDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvCompanyDataGridView, 0);
                    }

                    if (this.dgvChosenCompanyDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvChosenCompanyDataGridView, 0);
                    }
                }
            }
        }

        private void Draw_Current_User_Fingers()
        {
            //Set Finger Colours
            int intRow = pvtUserFingerTemplateDataView.Find(0);

            if (intRow == -1)
            {
                picLPinkie.Image = global::InteractPayroll.Properties.Resources.LPinkieClear;
                this.picLPinkie.Tag = "N";
            }
            else
            {
                if (pvtUserFingerTemplateDataView[intRow].Row.RowState == DataRowState.Unchanged
                || pvtUserFingerTemplateDataView[intRow].Row.RowState == DataRowState.Added)
                {
                    picLPinkie.Image = global::InteractPayroll.Properties.Resources.LPinkieServer;
                    this.picLPinkie.Tag = "Y";
                }
                else
                {
                    picLPinkie.Image = global::InteractPayroll.Properties.Resources.LPinkieClear;
                    this.picLPinkie.Tag = "N";
                }
            }

            intRow = pvtUserFingerTemplateDataView.Find(1);

            if (intRow == -1)
            {
                picLRing.Image = global::InteractPayroll.Properties.Resources.LRingClear;
                this.picLRing.Tag = "N";
            }
            else
            {
                if (pvtUserFingerTemplateDataView[intRow].Row.RowState == DataRowState.Unchanged
                || pvtUserFingerTemplateDataView[intRow].Row.RowState == DataRowState.Added)
                {
                    picLRing.Image = global::InteractPayroll.Properties.Resources.LRingServer;
                    this.picLRing.Tag = "Y";
                }
                else
                {
                    picLRing.Image = global::InteractPayroll.Properties.Resources.LRingClear;
                    this.picLRing.Tag = "N";
                }
            }

            intRow = pvtUserFingerTemplateDataView.Find(2);

            if (intRow == -1)
            {
                picLMiddle.Image = global::InteractPayroll.Properties.Resources.LMiddleClear;
                this.picLMiddle.Tag = "N";
            }
            else
            {
                if (pvtUserFingerTemplateDataView[intRow].Row.RowState == DataRowState.Unchanged
                || pvtUserFingerTemplateDataView[intRow].Row.RowState == DataRowState.Added)
                {
                    picLMiddle.Image = global::InteractPayroll.Properties.Resources.LMiddleServer;
                    this.picLMiddle.Tag = "Y";
                }
                else
                {
                    picLMiddle.Image = global::InteractPayroll.Properties.Resources.LMiddleClear;
                    this.picLMiddle.Tag = "N";
                }
            }

            intRow = pvtUserFingerTemplateDataView.Find(3);

            if (intRow == -1)
            {
                picLIndex.Image = global::InteractPayroll.Properties.Resources.LIndexClear;
                this.picLIndex.Tag = "N";
            }
            else
            {
                if (pvtUserFingerTemplateDataView[intRow].Row.RowState == DataRowState.Unchanged
                || pvtUserFingerTemplateDataView[intRow].Row.RowState == DataRowState.Added)
                {
                    picLIndex.Image = global::InteractPayroll.Properties.Resources.LIndexServer;
                    this.picLIndex.Tag = "Y";
                }
                else
                {
                    picLIndex.Image = global::InteractPayroll.Properties.Resources.LIndexClear;
                    this.picLIndex.Tag = "N";
                }
            }

            intRow = pvtUserFingerTemplateDataView.Find(4);

            if (intRow == -1)
            {
                picLThumb.Image = global::InteractPayroll.Properties.Resources.LThumbClear;
                this.picLThumb.Tag = "N";
            }
            else
            {
                if (pvtUserFingerTemplateDataView[intRow].Row.RowState == DataRowState.Unchanged
                || pvtUserFingerTemplateDataView[intRow].Row.RowState == DataRowState.Added)
                {
                    picLThumb.Image = global::InteractPayroll.Properties.Resources.LThumbServer;
                    this.picLThumb.Tag = "Y";
                }
                else
                {
                    picLThumb.Image = global::InteractPayroll.Properties.Resources.LThumbClear;
                    this.picLThumb.Tag = "N";
                }
            }

            intRow = pvtUserFingerTemplateDataView.Find(5);

            if (intRow == -1)
            {
                picRThumb.Image = global::InteractPayroll.Properties.Resources.RThumbClear;
                this.picRThumb.Tag = "N";
            }
            else
            {
                if (pvtUserFingerTemplateDataView[intRow].Row.RowState == DataRowState.Unchanged
                || pvtUserFingerTemplateDataView[intRow].Row.RowState == DataRowState.Added)
                {
                    picRThumb.Image = global::InteractPayroll.Properties.Resources.RThumbServer;
                    this.picRThumb.Tag = "Y";
                }
                else
                {
                    picRThumb.Image = global::InteractPayroll.Properties.Resources.RThumbClear;
                    this.picRThumb.Tag = "N";
                }
            }

            intRow = pvtUserFingerTemplateDataView.Find(6);

            if (intRow == -1)
            {
                picRIndex.Image = global::InteractPayroll.Properties.Resources.RIndexClear;
                this.picRIndex.Tag = "N";
            }
            else
            {
                if (pvtUserFingerTemplateDataView[intRow].Row.RowState == DataRowState.Unchanged
                || pvtUserFingerTemplateDataView[intRow].Row.RowState == DataRowState.Added)
                {
                    picRIndex.Image = global::InteractPayroll.Properties.Resources.RIndexServer;
                    this.picRIndex.Tag = "Y";
                }
                else
                {
                    picRIndex.Image = global::InteractPayroll.Properties.Resources.RIndexClear;
                    this.picRIndex.Tag = "N";
                }
            }

            intRow = pvtUserFingerTemplateDataView.Find(7);

            if (intRow == -1)
            {
                picRMiddle.Image = global::InteractPayroll.Properties.Resources.RMiddleClear;
                this.picRMiddle.Tag = "N";
            }
            else
            {
                if (pvtUserFingerTemplateDataView[intRow].Row.RowState == DataRowState.Unchanged
                || pvtUserFingerTemplateDataView[intRow].Row.RowState == DataRowState.Added)
                {
                    picRMiddle.Image = global::InteractPayroll.Properties.Resources.RMiddleServer;
                    this.picRMiddle.Tag = "Y";
                }
                else
                {
                    picRMiddle.Image = global::InteractPayroll.Properties.Resources.RMiddleClear;
                    this.picRMiddle.Tag = "N";
                }
            }

            intRow = pvtUserFingerTemplateDataView.Find(8);

            if (intRow == -1)
            {
                picRRing.Image = global::InteractPayroll.Properties.Resources.RRingClear;
                this.picRRing.Tag = "N";
            }
            else
            {
                if (pvtUserFingerTemplateDataView[intRow].Row.RowState == DataRowState.Unchanged
                || pvtUserFingerTemplateDataView[intRow].Row.RowState == DataRowState.Added)
                {
                    picRRing.Image = global::InteractPayroll.Properties.Resources.RRingServer;
                    this.picRRing.Tag = "Y";
                }
                else
                {
                    picRRing.Image = global::InteractPayroll.Properties.Resources.RRingClear;
                    this.picRRing.Tag = "N";
                }
            }

            intRow = pvtUserFingerTemplateDataView.Find(9);

            if (intRow == -1)
            {
                picRPinkie.Image = global::InteractPayroll.Properties.Resources.RPinkieClear;
                this.picRPinkie.Tag = "N";
            }
            else
            {
                if (pvtUserFingerTemplateDataView[intRow].Row.RowState == DataRowState.Unchanged
                || pvtUserFingerTemplateDataView[intRow].Row.RowState == DataRowState.Added)
                {
                    picRPinkie.Image = global::InteractPayroll.Properties.Resources.RPinkieServer;
                    this.picRPinkie.Tag = "Y";
                }
                else
                {
                    picRPinkie.Image = global::InteractPayroll.Properties.Resources.RPinkieClear;
                    this.picRPinkie.Tag = "N";
                }
            }
        }

        private void dgvCompanyDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCompanyDataGridView.Rows.Count > 0
                & pvtblnCompanyDataGridViewLoaded == true)
            {
            }
        }

        private void dgvChosenCompanyDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnChosenCompanyDataGridViewLoaded == true)
            {
                if (pvtintChosenCompanyDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintChosenCompanyDataGridViewRowIndex = e.RowIndex;

                    pvtCompanyAccessDataView = null;
                    pvtCompanyAccessDataView = new DataView(pvtDataSet.Tables["CompanyAccess"],
                        "USER_NO = " + pvtint64UserNo + " AND COMPANY_NO = " + this.dgvChosenCompanyDataGridView[1, e.RowIndex].Value.ToString(),
                        "",
                        DataViewRowState.CurrentRows);

                    if (pvtCompanyAccessDataView.Count > 0)
                    {
                        if (pvtCompanyAccessDataView[0]["COMPANY_ACCESS_IND"].ToString() == "A")
                        {
                            this.rbnAdministrator.Checked = true;
                        }
                        else
                        {
                            this.rbnUser.Checked = true;
                        }

                        if (pvtCompanyAccessDataView[0]["ACCESS_LAYER_IND"].ToString() == "B")
                        {
                            this.rbnInternetClient.Checked = true;
                        }
                        else
                        {
                            if (pvtCompanyAccessDataView[0]["ACCESS_LAYER_IND"].ToString() == "I")
                            {
                                this.rbnInternet.Checked = true;
                            }
                            else
                            {
                                this.rbnClient.Checked = true;
                            }
                        }
                    }
                    else
                    {
                        this.rbnAdministrator.Checked = false;
                        this.rbnUser.Checked = false;

                        this.rbnInternetClient.Checked = false;
                        this.rbnInternet.Checked = false;
                        this.rbnClient.Checked = false;
                    }
                }
            }
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
        btnRemoveAll_Click_Continue:

            if (this.dgvChosenCompanyDataGridView.Rows.Count > 0)
            {
                btnRemove_Click(null, null);

                goto btnRemoveAll_Click_Continue;
            }
        }

        private void dgvCompanyDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                btnAdd_Click(null, null);
            }
        }

        private void dgvChosenCompanyDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                btnRemove_Click(null, null);
            }
        }

        private void rbnAccessLayer_Click(object sender, EventArgs e)
        {
            RadioButton myRadioButton = (RadioButton)sender;
            
            pvtCompanyAccessDataView = null;
            pvtCompanyAccessDataView = new DataView(pvtDataSet.Tables["CompanyAccess"],
                "USER_NO = " + pvtint64UserNo + " AND COMPANY_NO = " + this.dgvChosenCompanyDataGridView[1, this.Get_DataGridView_SelectedRowIndex(this.dgvChosenCompanyDataGridView)].Value.ToString(),
                "",
                DataViewRowState.CurrentRows);
            
            if (myRadioButton.Name == "rbnInternetClient")
            {
                pvtCompanyAccessDataView[0]["ACCESS_LAYER_IND"] = "B";
            }
            else
            {
                if (myRadioButton.Name == "rbnInternet")
                {
                    pvtCompanyAccessDataView[0]["ACCESS_LAYER_IND"] = "I";
                }
                else
                {
                    pvtCompanyAccessDataView[0]["ACCESS_LAYER_IND"] = "C";
                }
            }
        }

        private void dgvUserDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 4)
            {
                if (dgvUserDataGridView[e.Column.Index + 1, e.RowIndex1].Value.ToString() == "")
                {
                    e.SortResult = -1;
                }
                else
                {
                    if (dgvUserDataGridView[e.Column.Index + 1, e.RowIndex2].Value.ToString() == "")
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (Int64.Parse(dgvUserDataGridView[e.Column.Index + 1, e.RowIndex1].Value.ToString()) > Int64.Parse(dgvUserDataGridView[e.Column.Index + 1, e.RowIndex2].Value.ToString()))
                        {
                            e.SortResult = 1;
                        }
                        else
                        {
                            if (Int64.Parse(dgvUserDataGridView[e.Column.Index + 1, e.RowIndex1].Value.ToString()) < Int64.Parse(dgvUserDataGridView[e.Column.Index + 1, e.RowIndex2].Value.ToString()))
                            {
                                e.SortResult = -1;
                            }
                            else
                            {
                                e.SortResult = 0;
                            }
                        }
                    }
                }

                e.Handled = true;
            }
        }

        private void btnClearFingers_Click(object sender, EventArgs e)
        {
            if (pvtUserFingerTemplateDataView.Count != 0)
            {
                for (int intRow = 0; intRow < pvtUserFingerTemplateDataView.Count; intRow++)
                {
                    pvtUserFingerTemplateDataView[intRow].Row.Delete();
                    intRow -= 1;
                }

                //Redraw Fingers
                Draw_Current_User_Fingers();

                this.dgvUserDataGridView[0, dgvUserDataGridView.CurrentCell.RowIndex].Style = this.NoTemplateDataGridViewCellStyle;
            }
        }

        private void Finger_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.btnSave.Enabled == true)
                {
                    //In Edit Mode
                    pvtpicFinger = (PictureBox)sender;

                    switch (pvtpicFinger.Name)
                    {
                        case "picLPinkie":

                            this.lblFingerDescription.Text = "Left Pinkie Finger";
                            pvtintFingerNo = 0;
                            break;

                        case "picLRing":

                            this.lblFingerDescription.Text = "Left Ring Finger";
                            pvtintFingerNo = 1;
                            break;

                        case "picLMiddle":

                            this.lblFingerDescription.Text = "Left Middle Finger";
                            pvtintFingerNo = 2;
                            break;

                        case "picLIndex":

                            this.lblFingerDescription.Text = "Left Index Finger";
                            pvtintFingerNo = 3;
                            break;

                        case "picLThumb":

                            this.lblFingerDescription.Text = "Left Thumb Finger";
                            pvtintFingerNo = 4;
                            break;

                        case "picRThumb":

                            this.lblFingerDescription.Text = "Right Thumb Finger";
                            pvtintFingerNo = 5;
                            break;

                        case "picRIndex":

                            this.lblFingerDescription.Text = "Right Index Finger";
                            pvtintFingerNo = 6;
                            break;


                        case "picRMiddle":

                            this.lblFingerDescription.Text = "Right Middle Finger";
                            pvtintFingerNo = 7;
                            break;

                        case "picRRing":

                            this.lblFingerDescription.Text = "Right Ring Finger";
                            pvtintFingerNo = 8;
                            break;

                        case "picRPinkie":

                            this.lblFingerDescription.Text = "Right Pinkie Finger";
                            pvtintFingerNo = 9;
                            break;
                    }

                    if (pvtpicFinger.Tag.ToString() == "Y")
                    {
                        DialogResult dgResult = new DialogResult();

                        if (pvtblnFingerprintDeviceOpened == true)
                        {
                            dgResult = CustomMessageBox.Show("Select Yes to Delete Fingerprint, No to Enroll Fingerprint.", this.Text, MessageBoxButtons.YesNoCancel,MessageBoxIcon.Information);
                        }
                        else
                        {
                            dgResult = CustomMessageBox.Show("Select OK to Delete Fingerprint.", this.Text, MessageBoxButtons.OKCancel,MessageBoxIcon.Information);
                        }

                        if (dgResult == DialogResult.Yes
                        || dgResult == DialogResult.OK)
                        {
                            int intRow = pvtUserFingerTemplateDataView.Find(pvtintFingerNo);

                            //Must be True
                            pvtUserFingerTemplateDataView[intRow].Row.Delete();
                                                        
                            //Redraw Fingers
                            Draw_Current_User_Fingers();

                            if (pvtUserFingerTemplateDataView.Count == 0)
                            {
                                this.dgvUserDataGridView[0, dgvUserDataGridView.CurrentCell.RowIndex].Style = this.NoTemplateDataGridViewCellStyle;
                            }
                        }
                        else
                        {
                            if (dgResult == DialogResult.No)
                            {
                                Clicked_Finger();
                            }
                        }
                    }
                    else
                    {
                        if (pvtblnFingerprintDeviceOpened == true)
                        {
                            Clicked_Finger();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsISUtilities.ErrorHandler(ex);
            }
        }

        private void Clicked_Finger()
        {
            this.btnSave.Enabled = false;
            btnClearFingers.Visible = false;
       
            this.lblFingerInformation.Text = "Place Finger on Reader";
            this.lblFingerInformation.ForeColor = Color.Black;

            if (pvtstrFingerprintReaderName == "URU4500 (Digital Persona)")
            {
                DP_StartCapture();
            }
            else
            {
                clsReadersToDp.StartCapture();
            }

            this.picFinger1.Image = global::InteractPayroll.Properties.Resources.FingerPrintQuestion64;
            this.picFinger2.Image = global::InteractPayroll.Properties.Resources.FingerPrint64;
            this.picFinger3.Image = global::InteractPayroll.Properties.Resources.FingerPrint64;
            this.picFinger4.Image = global::InteractPayroll.Properties.Resources.FingerPrint64;

            this.pnlEnroll.Visible = false;

            Draw_Current_User_Fingers();

            this.pnlFingers.Visible = true;

            this.lblEnrollMessage.ForeColor = Color.Black;
            this.pvtintCurrentFingerCount = 1;
            this.picMainFinger.Image = null;

            this.lblEnrollMessage.Text = pvtstrInitialMessage.Replace("#FINGER#", pvtstrFingerDescription);

            this.pnlEnroll.Visible = true;
            this.pnlFingers.Visible = false;
        }

        private void frmUser_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.btnSave.Enabled == true
            && this.tabControl.SelectedIndex == 1)
            {
                PictureBox myFingerPictureBox;

                EventArgs ea = new EventArgs();

                switch (e.KeyValue)
                {
                    case 48:

                        myFingerPictureBox = picLPinkie;

                        Finger_Click(myFingerPictureBox, ea);

                        break;

                    case 49:

                        myFingerPictureBox = picLRing;

                        Finger_Click(myFingerPictureBox, ea);

                        break;

                    case 50:

                        myFingerPictureBox = picLMiddle;

                        Finger_Click(myFingerPictureBox, ea);

                        break;

                    case 51:

                        myFingerPictureBox = picLIndex;

                        Finger_Click(myFingerPictureBox, ea);

                        break;

                    case 52:

                        myFingerPictureBox = picLThumb;

                        Finger_Click(myFingerPictureBox, ea);

                        break;

                    case 53:

                        myFingerPictureBox = picRThumb;

                        Finger_Click(myFingerPictureBox, ea);

                        break;

                    case 54:

                        myFingerPictureBox = picRIndex;

                        Finger_Click(myFingerPictureBox, ea);

                        break;

                    case 55:

                        myFingerPictureBox = picRMiddle;

                        Finger_Click(myFingerPictureBox, ea);

                        break;

                    case 56:

                        myFingerPictureBox = picRRing;

                        Finger_Click(myFingerPictureBox, ea);

                        break;

                    case 57:

                        myFingerPictureBox = picRPinkie;

                        Finger_Click(myFingerPictureBox, ea);

                        break;
                }
            }
        }

        private void frmUser_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (pvtblnFingerprintDeviceOpened == true)
                {
                    if (pvtstrFingerprintReaderName == "URU4500 (Digital Persona)")
                    {
                        pvtCurrentReader.On_Captured -= new Reader.CaptureCallback(OnCaptured);

                        Constants.ResultCode ResultCode = pvtCurrentReader.CancelCapture();

                        pvtCurrentReader.Dispose();
                        pvtCurrentReader = null;
                    }
                    else
                    {
                        //Curve / Columbo (Integrated Biometrics)")
                        clsReadersToDp.CloseDevice();
                    }
                }
            }
            catch
            {
            }
        }

        private void rbnFilter_Click(object sender, EventArgs e)
        {
            Load_CurrentForm_Records();
        }

        private void grbFingerPrintReader_Enter(object sender, EventArgs e)
        {

        }
    }
}
