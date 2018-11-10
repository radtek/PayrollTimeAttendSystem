using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InteractPayrollClient;
using DPUruNet;
using System.Runtime.Serialization;

namespace InteractPayroll
{
    public partial class frmTimeAttendanceEmployee : Form
    {
        clsISUtilities clsISUtilities;
        clsReadersToDp clsReadersToDp;

        ColorPalette greyScalePalette;

        private ReaderCollection ReaderCollection;
        private Reader pvtCurrentReader;

        DataGridViewCellStyle LockedPayrollRunDataGridViewCellStyle;
        DataGridViewCellStyle NotActiveDataGridViewCellStyle;
        DataGridViewCellStyle NoTemplateDataGridViewCellStyle;
        DataGridViewCellStyle HasTemplateDataGridViewCellStyle;
        DataGridViewCellStyle ReadOnlyDataGridViewCellStyle;

        string pvtstrFileName = "FingerprintReaderChoice.txt";

        string pvtstrFingerprintReaderName = "None";

        private string pvtstrInitialMessage = "To begin, place and hold your #FINGER# finger on the Fingerprint Reader until the screen indicates that the scan was successful. Repeat for each of the remaining scans.";
        private string pvtstrSuccessful = "The Scan was successful.\nPlace your #FINGER# finger on the Fingerprint Reader again.";
        private string pvtstrScanDifferent = "The finger scanned is NOT the same as the previous one or the Image is of BAD Quality. Try again. Place your #FINGER# finger flat on the fingerprint reader.";

        private string pvtstrFingerDescription;

        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;
        private DataRowView pvtDataRowView;
        private DataRow pvtTemplateDataRow;

        //PayCategory SpreadSheets Column Offset
        private int pvtintPayCategoryDescCol = 0;
        private int pvtintPayCategoryNoCol = 1;

        private byte[] pvtbytCompress;

        private byte[] pvtbytFinger1;
        private byte[] pvtbytFinger2;
        private byte[] pvtbytFinger3;

        private byte[] pvtbyteArrayPreviousTemplate;

        private bool pvtblnFingerprintDeviceOpened = false;

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnEmployeeDataGridViewLoaded = false;

        private bool pvtblnPayCategoryDataGridViewLoaded = false;
        private bool pvtblnChosenPayCategoryDataGridViewLoaded = false;

        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
        private int pvtintEmployeeDataGridViewRowIndex = -1;

        private int pvtintEmployeeNo = -1;
        private int pvtintReturnCode = -1;

        private string pvtstrPayrollType = "";
        private string pvtstrFilter = "";

        private PictureBox pvtpicFinger;
        private int pvtintFingerNo;
        private int pvtintCurrentFingerCount;

        private string pvtstrFingerprintDeviceOpenedFailureMessage = "";

        private DataView pvtPayCategoryDataView;
        private DataView pvtEmployeeDataView;
        private DataView pvtEmployeeFingerTemplateDataView;

        public frmTimeAttendanceEmployee()
        {
            InitializeComponent();

            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 114;

                this.grbFilter.Top += 114;
                this.grbLegend.Top += 114;

                this.tabControl.Top += 114;

                this.dgvEmployeeDataGridView.Height += 114;
            }
        }

        private void frmTimeAttendEmployee_Load(object sender, EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this, "busEmployee");

                //2017-06-06 Create Greyscale Palette
                Bitmap bmpPalette = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);

                greyScalePalette = bmpPalette.Palette;
                for (int i = 0; i < bmpPalette.Palette.Entries.Length; i++)
                {
                    greyScalePalette.Entries[i] = Color.FromArgb(i, i, i);
                }

                ReadOnlyDataGridViewCellStyle = new DataGridViewCellStyle();
                ReadOnlyDataGridViewCellStyle.BackColor = Color.Aqua;
                ReadOnlyDataGridViewCellStyle.SelectionBackColor = Color.Aqua;

                LockedPayrollRunDataGridViewCellStyle = new DataGridViewCellStyle();
                LockedPayrollRunDataGridViewCellStyle.BackColor = Color.Magenta;
                LockedPayrollRunDataGridViewCellStyle.SelectionBackColor = Color.Magenta;

                NotActiveDataGridViewCellStyle = new DataGridViewCellStyle();
                NotActiveDataGridViewCellStyle.BackColor = Color.Orange;
                NotActiveDataGridViewCellStyle.SelectionBackColor = Color.Orange;

                NoTemplateDataGridViewCellStyle = new DataGridViewCellStyle();
                NoTemplateDataGridViewCellStyle.BackColor = Color.Salmon;
                NoTemplateDataGridViewCellStyle.SelectionBackColor = Color.Salmon;

                HasTemplateDataGridViewCellStyle = new DataGridViewCellStyle();
                HasTemplateDataGridViewCellStyle.BackColor = Color.Aquamarine;
                HasTemplateDataGridViewCellStyle.SelectionBackColor = Color.Aquamarine;

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
                        catch (Exception ex)
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

                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayCategoryDesc.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayCategorySelectDesc.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                pvtDataSet = new DataSet();

                object[] objParm = new object[3];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[2] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records_TimeAttend", objParm);

                DataSet pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                pvtDataSet.Merge(pvtTempDataSet);

                //Load ComboBoxes
                clsISUtilities.DataBind_ComboBox_Load(cboOccupation, this.pvtDataSet.Tables["Occupation"], "OCCUPATION_DESC", "OCCUPATION_NO");
                clsISUtilities.DataBind_ComboBox_Load(cboDepartment, this.pvtDataSet.Tables["Department"], "DEPARTMENT_DESC", "DEPARTMENT_NO");

                this.dgvPayrollTypeDataGridView.Rows.Add("Time Attendance");

                this.pvtblnPayrollTypeDataGridViewLoaded = true;

                this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView, 0);
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
            if (pvtDataSet.Tables["Company"].Rows.Count == 0)
            {
                //Lock Out All Buttons
                this.btnNew.Enabled = false;
                this.btnUpdate.Enabled = false;
                this.btnDelete.Enabled = false;
                this.btnSave.Enabled = false;
                this.btnCancel.Enabled = false;
            }
            else
            {
                Clear_Form_Fields();

                //pvtintEmployeeNo = -1;

                Set_Form_For_Read();

                Load_Employees();

                if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                {
                    if (this.pvtDataSet.Tables["Department"].Rows.Count == 0)
                    {
                        CustomMessageBox.Show("Department/s Need to be Captured.",
                               this.Text,
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Information);

                        this.btnNew.Enabled = false;
                    }


                    if (pvtPayCategoryDataView.Count == 0)
                    {
                        CustomMessageBox.Show("Cost Centre/s Need to be Captured for " + this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView)].Value.ToString() + ".",
                               this.Text,
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Information);

                        this.btnNew.Enabled = false;
                    }
                }
            }
        }

        private void Load_Employees()
        {
            string strEmployeeFilter = "";

            this.chkClose.Checked = false;

            if (this.rbnActive.Checked == true)
            {
                this.btnNew.Enabled = true;

                strEmployeeFilter = " AND EMPLOYEE_ENDDATE IS NULL";
            }
            else
            {
                this.btnNew.Enabled = false;

                strEmployeeFilter = " AND NOT EMPLOYEE_ENDDATE IS NULL";
            }

            pvtEmployeeDataView = null;
            pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                pvtstrFilter + strEmployeeFilter,
                "EMPLOYEE_CODE",
                DataViewRowState.CurrentRows);

            clsISUtilities.DataViewIndex = 0;

            if (clsISUtilities.DataBind_Form_And_DataView_To_Class() == false)
            {
                clsISUtilities.DataBind_DataView_And_Index(this, pvtEmployeeDataView, "EMPLOYEE_NO");

                //TabPage 0
                clsISUtilities.DataBind_DataView_To_TextBox(txtCode, "EMPLOYEE_CODE", false, "", false);
                clsISUtilities.DataBind_Control_Required_If_Enabled(txtCode, "Enter Employee Code");

                clsISUtilities.DataBind_DataView_To_TextBox(txtName, "EMPLOYEE_NAME", true, "Enter Employee Name.", true);
                clsISUtilities.DataBind_DataView_To_TextBox(txtSurname, "EMPLOYEE_SURNAME", true, "Enter Employee Surname.", true);

                clsISUtilities.DataBind_DataView_To_ComboBox(cboOccupation, "OCCUPATION_NO", true, "Select Occupation.", true);
                clsISUtilities.DataBind_DataView_To_ComboBox(cboDepartment, "DEPARTMENT_NO", true, "Select Department.", true);

                clsISUtilities.DataBind_DataView_To_Date_TextBox_ReadOnly(txtStartDate, "EMPLOYEE_TAX_STARTDATE");
                clsISUtilities.DataBind_DataView_To_Date_TextBox_ReadOnly(txtEffectiveDate, "EMPLOYEE_LAST_RUNDATE");
                clsISUtilities.DataBind_DataView_To_Date_TextBox_ReadOnly(txtDateClosed, "EMPLOYEE_ENDDATE");

                clsISUtilities.DataBind_DataView_To_Numeric_TextBox_Min_Length(txtTelHome, "EMPLOYEE_TEL_HOME", 0, 10, false, "Enter Home Tel. Number.", true, 0, true);

                clsISUtilities.DataBind_DataView_To_Numeric_TextBox_Min_Length(txtTelWork, "EMPLOYEE_TEL_WORK", 0, 10, false, "Enter Work Tel. Number.", false, 0, true);
                clsISUtilities.DataBind_Control_Required_If_Enabled(txtTelWork, "Enter Work Telephone Number.");

                clsISUtilities.DataBind_DataView_To_Numeric_TextBox_Min_Length(txtTelCell, "EMPLOYEE_TEL_CELL", 0, 10, false, "Enter Cell Number.", true, 0, true);

                clsISUtilities.DataBind_DataView_To_TextBox(txtEmployeeCodeOther, "EMPLOYEE_3RD_PARTY_CODE", false, "", true);

                clsISUtilities.DataBind_DataView_To_TextBox(this.txtEmployeePin, "EMPLOYEE_PIN", false, "", true);
                clsISUtilities.DataBind_DataView_To_TextBox(this.txtRFIDCardNo, "EMPLOYEE_RFID_CARD_NO", false, "", true);

            }
            else
            {
                clsISUtilities.Re_DataBind_DataView(pvtEmployeeDataView);
            }

            this.grbEmployeeLock.Visible = false;

            this.Clear_DataGridView(this.dgvEmployeeDataGridView);

            this.pvtblnEmployeeDataGridViewLoaded = false;
            //pvtintEmployeeDataGridViewRowIndex = -1;
            int intEmployeeSelectedRow = 0;

            for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
            {
                if (Convert.ToInt32(pvtEmployeeDataView[intRow]["EMPLOYEE_NO"]) == pvtintEmployeeNo)
                {
                    intEmployeeSelectedRow = intRow;
                }

                //Set Finger Colours
                pvtEmployeeFingerTemplateDataView = null;
                pvtEmployeeFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["EmployeeFingerTemplate"], "COMPANY_NO = " + Convert.ToString(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND EMPLOYEE_NO = " + pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString(), "FINGER_NO", DataViewRowState.CurrentRows);

                if (this.rbnTemplateMissing.Checked == true)
                {
                    if (pvtEmployeeFingerTemplateDataView.Count > 0)
                    {
                        continue;
                    }
                }
                else
                {
                    if (this.rbnEnrolledFingerprint.Checked == true)
                    {
                        if (pvtEmployeeFingerTemplateDataView.Count == 0)
                        {
                            continue;
                        }
                    }
                }

                this.dgvEmployeeDataGridView.Rows.Add("",
                                                      "",
                                                      pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString(),
                                                      pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                      pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                      pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                      intRow.ToString());

                if (pvtEmployeeFingerTemplateDataView.Count == 0)
                {
                    this.dgvEmployeeDataGridView[1, dgvEmployeeDataGridView.Rows.Count - 1].Style = this.NoTemplateDataGridViewCellStyle;
                }
                else
                {
                    this.dgvEmployeeDataGridView[1, dgvEmployeeDataGridView.Rows.Count - 1].Style = this.HasTemplateDataGridViewCellStyle;
                }

                if (pvtEmployeeDataView[intRow]["EMPLOYEE_TAKEON_IND"].ToString() != "Y")
                {
                    this.dgvEmployeeDataGridView[0, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = NotActiveDataGridViewCellStyle;
                }
                else
                {
                    if (pvtEmployeeDataView[intRow]["PAYROLL_LINK"] != System.DBNull.Value)
                    {
                        this.dgvEmployeeDataGridView[0, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = LockedPayrollRunDataGridViewCellStyle;

                        this.grbEmployeeLock.Visible = true;
                    }
                }

                if (this.rbnClosed.Checked == true)
                {
                    this.dgvEmployeeDataGridView[0, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = this.ReadOnlyDataGridViewCellStyle;
                }
            }

            this.pvtblnEmployeeDataGridViewLoaded = true;

            if (dgvEmployeeDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, intEmployeeSelectedRow);
            }
            else
            {
                this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
                this.Clear_DataGridView(this.dgvChosenPayCategoryDataGridView);

                this.txtCode.Text = "";
                this.txtName.Text = "";
                this.txtSurname.Text = "";

                this.cboOccupation.SelectedIndex = -1;
                this.cboDepartment.SelectedIndex = -1;

                this.txtStartDate.Text = "";
                this.txtEffectiveDate.Text = "";
                this.txtDateClosed.Text = "";

                this.txtTelHome.Text = "";
                this.txtTelCell.Text = "";
                this.txtTelWork.Text = "";

                this.btnUpdate.Enabled = false;
                this.btnDelete.Enabled = false;
            }
        }

        private void Draw_Current_Employee_Fingers()
        {
            //Set Finger Colours
            int intRow = pvtEmployeeFingerTemplateDataView.Find(0);

            if (intRow == -1)
            {
                picLPinkie.Image = global::InteractPayroll.Properties.Resources.LPinkieClear;
                this.picLPinkie.Tag = "N";
            }
            else
            {
                picLPinkie.Image = global::InteractPayroll.Properties.Resources.LPinkieServer;
                this.picLPinkie.Tag = "Y";
            }

            intRow = pvtEmployeeFingerTemplateDataView.Find(1);

            if (intRow == -1)
            {
                picLRing.Image = global::InteractPayroll.Properties.Resources.LRingClear;
                this.picLRing.Tag = "N";
            }
            else
            {
                picLRing.Image = global::InteractPayroll.Properties.Resources.LRingServer;
                this.picLRing.Tag = "Y";
            }

            intRow = pvtEmployeeFingerTemplateDataView.Find(2);

            if (intRow == -1)
            {
                picLMiddle.Image = global::InteractPayroll.Properties.Resources.LMiddleClear;
                this.picLMiddle.Tag = "N";
            }
            else
            {
                picLMiddle.Image = global::InteractPayroll.Properties.Resources.LMiddleServer;
                this.picLMiddle.Tag = "Y";
            }

            intRow = pvtEmployeeFingerTemplateDataView.Find(3);

            if (intRow == -1)
            {
                picLIndex.Image = global::InteractPayroll.Properties.Resources.LIndexClear;
                this.picLIndex.Tag = "N";
            }
            else
            {
                picLIndex.Image = global::InteractPayroll.Properties.Resources.LIndexServer;
                this.picLIndex.Tag = "Y";
            }

            intRow = pvtEmployeeFingerTemplateDataView.Find(4);

            if (intRow == -1)
            {
                picLThumb.Image = global::InteractPayroll.Properties.Resources.LThumbClear;
                this.picLThumb.Tag = "N";
            }
            else
            {
                picLThumb.Image = global::InteractPayroll.Properties.Resources.LThumbServer;
                this.picLThumb.Tag = "Y";
            }

            intRow = pvtEmployeeFingerTemplateDataView.Find(5);

            if (intRow == -1)
            {
                picRThumb.Image = global::InteractPayroll.Properties.Resources.RThumbClear;
                this.picRThumb.Tag = "N";
            }
            else
            {
                picRThumb.Image = global::InteractPayroll.Properties.Resources.RThumbServer;
                this.picRThumb.Tag = "Y";
            }

            intRow = pvtEmployeeFingerTemplateDataView.Find(6);

            if (intRow == -1)
            {
                picRIndex.Image = global::InteractPayroll.Properties.Resources.RIndexClear;
                this.picRIndex.Tag = "N";
            }
            else
            {
                picRIndex.Image = global::InteractPayroll.Properties.Resources.RIndexServer;
                this.picRIndex.Tag = "Y";
            }

            intRow = pvtEmployeeFingerTemplateDataView.Find(7);

            if (intRow == -1)
            {
                picRMiddle.Image = global::InteractPayroll.Properties.Resources.RMiddleClear;
                this.picRMiddle.Tag = "N";
            }
            else
            {
                picRMiddle.Image = global::InteractPayroll.Properties.Resources.RMiddleServer;
                this.picRMiddle.Tag = "Y";
            }

            intRow = pvtEmployeeFingerTemplateDataView.Find(8);

            if (intRow == -1)
            {
                picRRing.Image = global::InteractPayroll.Properties.Resources.RRingClear;
                this.picRRing.Tag = "N";
            }
            else
            {
                picRRing.Image = global::InteractPayroll.Properties.Resources.RRingServer;
                this.picRRing.Tag = "Y";
            }

            intRow = pvtEmployeeFingerTemplateDataView.Find(9);

            if (intRow == -1)
            {
                picRPinkie.Image = global::InteractPayroll.Properties.Resources.RPinkieClear;
                this.picRPinkie.Tag = "N";
            }
            else
            {
                picRPinkie.Image = global::InteractPayroll.Properties.Resources.RPinkieServer;
                this.picRPinkie.Tag = "Y";
            }
        }

        public void btnNew_Click(object sender, System.EventArgs e)
        {
            try
            {
                this.Text = this.Text + " - New";

                pvtintEmployeeNo = -1;

                pvtDataRowView = this.pvtEmployeeDataView.AddNew();

                pvtDataRowView["COMPANY_NO"] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                pvtDataRowView["PAY_CATEGORY_TYPE"] = pvtstrPayrollType;
                pvtDataRowView["EMPLOYEE_NO"] = pvtintEmployeeNo;
                pvtDataRowView["EMPLOYEE_SURNAME"] = "";
                //Forces Record to be First (clsISUtilities.DataViewIndex = 0)
                pvtDataRowView["EMPLOYEE_CODE"] = "";

                pvtDataRowView.EndEdit();

                //Set Index to First Row of View
                clsISUtilities.DataViewIndex = 0;

                Set_Form_For_Edit();

                pvtEmployeeFingerTemplateDataView = null;
                pvtEmployeeFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["EmployeeFingerTemplate"],
                    "COMPANY_NO = " + Convert.ToString(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND EMPLOYEE_NO = -1",
                    "FINGER_NO",
                    DataViewRowState.CurrentRows);

                this.Draw_Current_Employee_Fingers();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        public void btnUpdate_Click(object sender, System.EventArgs e)
        {
            this.Text += " - Update";

            Set_Form_For_Edit();

            this.chkClose.Checked = false;
            this.chkClose.Enabled = true;

            if (this.pvtDataSet.Tables["Company"].Rows[0]["GENERATE_EMPLOYEE_NUMBER_IND"].ToString() == "Y")
            {
                this.txtCode.Focus();
            }
            else
            {
                this.txtName.Focus();
            }
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
                        pvtTemplateDataRow = pvtDataSet.Tables["EmployeeFingerTemplate"].NewRow();

                        pvtTemplateDataRow["COMPANY_NO"] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                        pvtTemplateDataRow["EMPLOYEE_NO"] = this.pvtintEmployeeNo;
                        pvtTemplateDataRow["FINGER_NO"] = pvtintFingerNo;
                        pvtTemplateDataRow["FINGER_TEMPLATE"] = bytExtractedTemplate;

                        pvtDataSet.Tables["EmployeeFingerTemplate"].Rows.Add(pvtTemplateDataRow);
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

                                        this.dgvEmployeeDataGridView[1, dgvEmployeeDataGridView.CurrentCell.RowIndex].Style = HasTemplateDataGridViewCellStyle;
                                    }
                                    else
                                    {
                                        strMessage = "UNSUCCESSFUL.";
                                    }

                                    CustomMessageBox.Show("Enrollment of " + pvtstrFingerDescription + " finger " + strMessage, this.Text,MessageBoxButtons.OK,MessageBoxIcon.Information);

                                    if (pvtstrFingerprintReaderName == "URU4500 (Digital Persona)")
                                    {
                                        DP_StopCapture();
                                    }

                                    this.pnlEnroll.Visible = false;
                                    this.btnClearFingers.Visible = true;
                                    this.pnlFingers.Visible = true;
                                    this.chkClose.Enabled = true;
                                    this.btnSave.Enabled = true;

                                    this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView));

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

        private void DP_StopCapture()
        {
            pvtCurrentReader.On_Captured -= new Reader.CaptureCallback(OnCaptured);
        }

        public void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                pvtTempDataSet = new DataSet();

                //DataLayer Fields are Checked
                pvtintReturnCode = this.clsISUtilities.DataBind_Save_Check();

                if (pvtintReturnCode != 0)
                {
                    return;
                }

                if (dgvChosenPayCategoryDataGridView.Rows.Count == 0)
                {
                    CustomMessageBox.Show("There must be at least 1 Selected Cost Centre.",
                    this.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                    return;
                }

                pvtTempDataSet.Tables.Add(this.pvtDataSet.Tables["Employee"].Clone());
                pvtTempDataSet.Tables["Employee"].ImportRow(pvtEmployeeDataView[clsISUtilities.DataViewIndex].Row);

                if (this.chkClose.Checked == true)
                {
                    DialogResult dlgResult = CustomMessageBox.Show("Are you sure you want to Close Employee '" + pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_NAME"].ToString() + " " + pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_SURNAME"].ToString() + "'?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                    if (dlgResult == DialogResult.Yes)
                    {
                        pvtTempDataSet.Tables["Employee"].Rows[0]["CLOSE_IND"] = "Y";
                        pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_ENDDATE"] = DateTime.Now;

                        this.clsISUtilities.pubintReloadSpreadsheet = true;
                    }
                    else
                    {
                        return;
                    }
                }

                //Check That Employee Does Not Exist
                if (this.Text.IndexOf("- New", 0) > 0
                    & this.pvtDataSet.Tables["Company"].Rows[0]["GENERATE_EMPLOYEE_NUMBER_IND"].ToString() == "Y")
                {
                    DataView EmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                    pvtstrFilter + " AND EMPLOYEE_NO <> " + this.pvtintEmployeeNo,
                    "EMPLOYEE_CODE",
                    DataViewRowState.CurrentRows);

                    int intRow = EmployeeDataView.Find(pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_CODE"].ToString());

                    if (intRow > -1)
                    {
                        CustomMessageBox.Show("Employee '" + EmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString() + " " + EmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString() + "' already Exists with an Employee Code of '" + EmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString() + "'.",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                        this.txtCode.Focus();

                        return;
                    }
                }

                //Employee PayCategory
                //Employee PayCategory 
                pvtTempDataSet.Tables.Add(pvtDataSet.Tables["EmployeePayCategory"].Clone());

                DataView EmployeePayCategoryDataView = new DataView(pvtDataSet.Tables["EmployeePayCategory"],
                    pvtstrFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo,
                    "",
                    DataViewRowState.Added | DataViewRowState.ModifiedCurrent | DataViewRowState.Deleted);

                for (int intRow = 0; intRow < EmployeePayCategoryDataView.Count; intRow++)
                {
                    pvtTempDataSet.Tables["EmployeePayCategory"].ImportRow(EmployeePayCategoryDataView[intRow].Row);
                }

                //Add EmployeeFingerTemplate Table 
                pvtTempDataSet.Tables.Add(pvtDataSet.Tables["EmployeeFingerTemplate"].Clone());

                DataView EmployeeFingerTemplateDataView = new DataView(pvtDataSet.Tables["EmployeeFingerTemplate"],
                  "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND EMPLOYEE_NO = " + pvtintEmployeeNo,
                  "",
                  DataViewRowState.Added | DataViewRowState.ModifiedCurrent | DataViewRowState.Deleted);

                for (int intRow = 0; intRow < EmployeeFingerTemplateDataView.Count; intRow++)
                {
                    pvtTempDataSet.Tables["EmployeeFingerTemplate"].ImportRow(EmployeeFingerTemplateDataView[intRow].Row);
                }

                pvtDataSet.AcceptChanges();

                //Compress DataSet
                pvtbytCompress = clsISUtilities.Compress_DataSet(pvtTempDataSet);

                if (this.Text.IndexOf("- New", 0) > 0)
                {
                    string strEmployeeCode = "";

                    object[] objParm = new object[4];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));

                    objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[2] = pvtbytCompress;
                    objParm[3] = pvtstrPayrollType;

                    pvtintEmployeeNo = (int)clsISUtilities.DynamicFunction("Insert_New_Record_TimeAttend", objParm, true);

                    //Change Key From -1 to Actual Value
                    EmployeePayCategoryDataView = new DataView(pvtDataSet.Tables["EmployeePayCategory"],
                        pvtstrFilter + " AND EMPLOYEE_NO = -1",
                        "",
                        DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < EmployeePayCategoryDataView.Count; intRow++)
                    {
                        EmployeePayCategoryDataView[intRow]["EMPLOYEE_NO"] = pvtintEmployeeNo;

                        intRow -= 1;
                    }

                    //Change Key From -1 to Actual Value
                    EmployeeFingerTemplateDataView = new DataView(pvtDataSet.Tables["EmployeeFingerTemplate"],
                        "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND EMPLOYEE_NO = -1",
                        "",
                        DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < EmployeeFingerTemplateDataView.Count; intRow++)
                    {
                        EmployeeFingerTemplateDataView[intRow]["EMPLOYEE_NO"] = pvtintEmployeeNo;

                        intRow -= 1;
                    }

                    //PAYROLL_LINK Was set to Zero In New Layer - Need to be Null
                    pvtEmployeeDataView[clsISUtilities.DataViewIndex]["PAYROLL_LINK"] = System.DBNull.Value;

                    pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_NO"] = pvtintEmployeeNo;

                    //NB Must Be Last Change EMPLOYEE_CODE is Sort of pvtEmployeeDataView and will Change its Sequence 
                    //NB Must Be Last Change EMPLOYEE_CODE is Sort of pvtEmployeeDataView and will Change its Sequence 
                    if (this.pvtDataSet.Tables["Company"].Rows[0]["GENERATE_EMPLOYEE_NUMBER_IND"].ToString() == "N")
                    {
                        strEmployeeCode = pvtintEmployeeNo.ToString("00000");
                        pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_CODE"] = strEmployeeCode;
                    }
                }
                else
                {
                    if (pvtDataSet.Tables["ReturnCode"] != null)
                    {
                        pvtDataSet.Tables.Remove("ReturnCode");
                    }

                    object[] objParm = new object[4];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[2] = pvtintEmployeeNo;
                    objParm[3] = pvtbytCompress;

                    pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Update_Record_TimeAttend_New", objParm, true);

                    pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                    pvtDataSet.Merge(pvtTempDataSet);

                    if (Convert.ToInt32(pvtDataSet.Tables["ReturnCode"].Rows[0]["RETURN_CODE"]) == 9999)
                    {
                        this.pvtDataSet.RejectChanges();

                        this.dgvEmployeeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView)].Style = LockedPayrollRunDataGridViewCellStyle;

                        this.grbEmployeeLock.Visible = true;

                        pvtEmployeeDataView[clsISUtilities.DataViewIndex]["PAYROLL_LINK"] = 0;

                        CustomMessageBox.Show("This Employee is currently being used in a Payroll Run.\r\n\r\nUpdate Cancelled.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                    else
                    {
                        this.dgvEmployeeDataGridView[3, pvtintEmployeeDataGridViewRowIndex].Value = pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_CODE"].ToString();
                        this.dgvEmployeeDataGridView[4, pvtintEmployeeDataGridViewRowIndex].Value = pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_SURNAME"].ToString();
                        this.dgvEmployeeDataGridView[5, pvtintEmployeeDataGridViewRowIndex].Value = pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_NAME"].ToString();
                    }
                }

                this.pvtDataSet.AcceptChanges();

                if (this.clsISUtilities.pubintReloadSpreadsheet == true)
                {
                    //Set So That Employees will Reload
                    this.btnSave.Enabled = false;

                    dgvEmployeeDataGridView.Enabled = true;

                    Load_Employees();
                }

                btnCancel_Click(sender, e);
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        public void btnDelete_Click(object sender, System.EventArgs e)
        {
            try
            {
                DialogResult dlgResult = CustomMessageBox.Show("Delete Employee '" + pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_NAME"].ToString() + " " + pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_SURNAME"].ToString() + "'",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dlgResult == DialogResult.Yes)
                {
                    object[] objParm = new object[3];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));

                    objParm[2] = pvtintEmployeeNo;

                    clsISUtilities.DynamicFunction("Delete_Record", objParm, true);

                    pvtEmployeeDataView[clsISUtilities.DataViewIndex].Delete();

                    this.pvtDataSet.AcceptChanges();

                    pvtintEmployeeNo = -1;

                    Load_Employees();
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void Set_Form_For_Edit()
        {
            bool blnNew = false;

            this.dgvEmployeeDataGridView.Enabled = false;
            this.dgvPayrollTypeDataGridView.Enabled = false;

            this.rbnNone.Checked = true;
            
            this.picEmployeeLock.Image = global::InteractPayroll.Properties.Resources.NewLock16;
            this.picPayrollTypeLock.Image = global::InteractPayroll.Properties.Resources.NewLock16;

            this.picEmployeeLock.Visible = true;
            this.picPayrollTypeLock.Visible = true;

            if (this.Text.EndsWith(" - New") == true)
            {
                blnNew = true;
            }

            clsISUtilities.Set_Form_For_Edit(blnNew);

            if (this.Text.EndsWith(" - New") == true)
            {
                if (this.pvtDataSet.Tables["Company"].Rows[0]["GENERATE_EMPLOYEE_NUMBER_IND"].ToString() == "Y")
                {
                    this.txtCode.Enabled = true;
                }

                this.rbnWorkTelCompany.Checked = true;
                this.chkEmpNo.Checked = false;

                Clear_Pay_Category();
            }
            else
            {
                //Temp Fix for Bogart
                if (this.pvtDataSet.Tables["Company"].Rows[0]["GENERATE_EMPLOYEE_NUMBER_IND"].ToString() == "Y")
                {
                    this.txtCode.Enabled = true;
                }

                //ELR 2017-09-29
                if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["USE_EMPLOYEE_NO_IND"].ToString() == "Y")
                {
                    this.chkEmpNo.Checked = true;
                }
                else
                {
                    this.chkEmpNo.Checked = false;
                }


                this.chkClose.Enabled = true;
            }

            this.chkEmpNo.Enabled = true;

            this.btnPayCategoryAdd.Enabled = true;
            this.btnPayCategoryRemove.Enabled = true;

            this.rbnWorkTelOwn.Enabled = true;
            this.rbnWorkTelCompany.Enabled = true;

            this.rbnNone.Enabled = false;
            this.rbnTemplateMissing.Enabled = false;
            this.rbnEnrolledFingerprint.Enabled = false;

            this.btnNew.Enabled = false;
            this.btnUpdate.Enabled = false;
            this.btnDelete.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.lblMessage.Visible = true;
            this.btnClearFingers.Visible = true;

            EventArgs e = new EventArgs();

            //Fire Work Telephone Enable
            Work_Telephone_Click(null, e);

            if (this.Text.EndsWith("- New") == true)
            {
                if (this.dgvPayCategoryDataGridView.Rows.Count == 1)
                {
                    object sender = new object();
                    e = new EventArgs();

                    btnPayCategoryAdd_Click(sender, e);
                }

                //2012-10-23
                if (this.cboOccupation.Items.Count == 1)
                {
                    this.cboOccupation.SelectedIndex = 0;
                }

                if (this.cboDepartment.Items.Count == 1)
                {
                    this.cboDepartment.SelectedIndex = 0;
                }
            }

            this.rbnActive.Enabled = false;
            this.rbnClosed.Enabled = false;

            if (this.pvtDataSet.Tables["Company"].Rows[0]["GENERATE_EMPLOYEE_NUMBER_IND"].ToString() == "Y")
            {
                this.txtCode.Focus();
            }
            else
            {
                this.txtCode.Enabled = false;
                this.txtName.Focus();
            }
        }

        private void btnPayCategoryAdd_Click(object sender, EventArgs e)
        {
            if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
            {
                DataView EmployeePayCategoryDataView = new DataView(pvtDataSet.Tables["EmployeePayCategory"],
                    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND EMPLOYEE_NO = " + this.pvtintEmployeeNo + " AND PAY_CATEGORY_NO = " + this.dgvPayCategoryDataGridView[pvtintPayCategoryNoCol, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Value.ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                    "",
                     DataViewRowState.CurrentRows);

                if (EmployeePayCategoryDataView.Count == 0)
                {
                    this.pvtblnPayCategoryDataGridViewLoaded = false;
                    this.pvtblnChosenPayCategoryDataGridViewLoaded = false;

                    DataRowView drvDataRowView = EmployeePayCategoryDataView.AddNew();

                    drvDataRowView.BeginEdit();

                    //Set Key for Find
                    drvDataRowView["COMPANY_NO"] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    drvDataRowView["EMPLOYEE_NO"] = this.pvtintEmployeeNo;
                    drvDataRowView["PAY_CATEGORY_NO"] = Convert.ToInt32(dgvPayCategoryDataGridView[pvtintPayCategoryNoCol, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Value);
                    drvDataRowView["PAY_CATEGORY_TYPE"] = pvtstrPayrollType;

                    drvDataRowView.EndEdit();

                    this.dgvChosenPayCategoryDataGridView.Rows.Add(this.dgvPayCategoryDataGridView[pvtintPayCategoryDescCol, this.Get_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView)].Value.ToString(),
                                                                 dgvPayCategoryDataGridView[pvtintPayCategoryNoCol, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Value.ToString());

                    //Remove Pay Category
                    DataGridViewRow myDataGridViewRow = this.dgvPayCategoryDataGridView.Rows[this.dgvPayCategoryDataGridView.CurrentRow.Index];
                    this.dgvPayCategoryDataGridView.Rows.Remove(myDataGridViewRow);

                    this.pvtblnPayCategoryDataGridViewLoaded = true;
                    this.pvtblnChosenPayCategoryDataGridViewLoaded = true;

                    //Fire Off New Row
                    this.Set_DataGridView_SelectedRowIndex(this.dgvChosenPayCategoryDataGridView, this.dgvChosenPayCategoryDataGridView.Rows.Count - 1);
                }
                else
                {
                    CustomMessageBox.Show("Error",this.Text,MessageBoxButtons.OK,MessageBoxIcon.Error);
                }

                if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, 0);
                }

                if (pvtstrPayrollType == "S"
                    & this.btnSave.Enabled == true)
                {
                    this.btnPayCategoryRemove.Enabled = true;
                    this.btnPayCategoryAdd.Enabled = false;
                }
            }
        }

        private void btnPayCategoryRemove_Click(object sender, EventArgs e)
        {
            if (this.dgvChosenPayCategoryDataGridView.Rows.Count > 0)
            {
                this.pvtblnPayCategoryDataGridViewLoaded = false;
                this.pvtblnChosenPayCategoryDataGridViewLoaded = false;

                DataView EmployeePayCategoryDataView = new DataView(pvtDataSet.Tables["EmployeePayCategory"],
                    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND EMPLOYEE_NO = " + this.pvtintEmployeeNo + " AND PAY_CATEGORY_NO = " + this.dgvChosenPayCategoryDataGridView[pvtintPayCategoryNoCol, this.Get_DataGridView_SelectedRowIndex(this.dgvChosenPayCategoryDataGridView)].Value.ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                    "",
                    DataViewRowState.CurrentRows);

                this.dgvPayCategoryDataGridView.Rows.Add(this.dgvChosenPayCategoryDataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvChosenPayCategoryDataGridView)].Value.ToString(),
                                                            EmployeePayCategoryDataView[0]["PAY_CATEGORY_NO"].ToString());

                EmployeePayCategoryDataView[0].Delete();

                //Remove Chosen Pay Category
                DataGridViewRow myDataGridViewRow = this.dgvChosenPayCategoryDataGridView.Rows[this.dgvChosenPayCategoryDataGridView.CurrentRow.Index];

                this.dgvChosenPayCategoryDataGridView.Rows.Remove(myDataGridViewRow);

                this.pvtblnPayCategoryDataGridViewLoaded = true;
                this.pvtblnChosenPayCategoryDataGridViewLoaded = true;

                this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, this.dgvPayCategoryDataGridView.Rows.Count - 1);

                if (this.dgvChosenPayCategoryDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvChosenPayCategoryDataGridView, 0);
                }

                if (pvtstrPayrollType == "S"
                    & this.btnSave.Enabled == true)
                {
                    this.btnPayCategoryRemove.Enabled = false;
                    this.btnPayCategoryAdd.Enabled = true;
                }
            }
        }

        private void Clear_Pay_Category()
        {
            //Load Pay Category
            this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
            this.Clear_DataGridView(this.dgvChosenPayCategoryDataGridView);

            this.pvtblnPayCategoryDataGridViewLoaded = false;

            for (int intIndex = 0; intIndex < pvtPayCategoryDataView.Count; intIndex++)
            {
                this.dgvPayCategoryDataGridView.Rows.Add(pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_DESC"].ToString(),
                                                         pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_NO"].ToString());

            }

            this.pvtblnPayCategoryDataGridViewLoaded = true;
        }

        public void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.pvtDataSet.RejectChanges();

            Set_Form_For_Read();

            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, pvtintEmployeeDataGridViewRowIndex);
            }
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

            this.btnNew.Enabled = true;
            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            clsISUtilities.Set_Form_For_Read();

            this.dgvEmployeeDataGridView.Enabled = true;
            this.dgvPayrollTypeDataGridView.Enabled = true;

            this.picEmployeeLock.Visible = false;
            this.picPayrollTypeLock.Visible = false;

            this.chkClose.Enabled = false;
            this.chkEmpNo.Enabled = false;

            this.rbnActive.Enabled = true;
            this.rbnClosed.Enabled = true;
            
            this.rbnNone.Enabled = true;
            this.rbnTemplateMissing.Enabled = true;
            this.rbnEnrolledFingerprint.Enabled = true;
            
            this.rbnWorkTelOwn.Enabled = false;
            this.rbnWorkTelCompany.Enabled = false;

            this.lblMessage.Visible = false;
            this.btnClearFingers.Visible = false;

            this.pnlEnroll.Visible = false;
            this.pnlFingers.Visible = true;

            //Disable Pay Category Fields
            this.btnPayCategoryAdd.Enabled = false;
            this.btnPayCategoryRemove.Enabled = false;

            if (this.rbnActive.Checked == true)
            {
                this.chkClose.Checked = false;
            }

            this.chkClose.Enabled = false;
        }

        private void Clear_Form_Fields()
        {
            //Pay Category
            this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
            this.Clear_DataGridView(this.dgvChosenPayCategoryDataGridView);
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
            switch (myDataGridView.Name)
            {
                case "dgvPayrollTypeDataGridView":

                    pvtintPayrollTypeDataGridViewRowIndex = -1;
                    break;

                case "dgvEmployeeDataGridView":

                    pvtintEmployeeDataGridViewRowIndex = -1;
                    break;
            }

            //Fires DataGridView RowEnter Function
            if (this.Get_DataGridView_SelectedRowIndex(myDataGridView) == intRow)
            {
                DataGridViewCellEventArgs myDataGridViewCellEventArgs = new DataGridViewCellEventArgs(0, intRow);

                switch (myDataGridView.Name)
                {
                    case "dgvPayrollTypeDataGridView":

                        this.dgvPayrollTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEmployeeDataGridView":

                        this.dgvEmployeeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvChosenPayCategoryDataGridView":

                        this.dgvChosenPayCategoryDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPayCategoryDataGridView":

                        this.dgvPayCategoryDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    default:

                        MessageBox.Show("No Entry for " + myDataGridView.Name + " in Set_DataGridView_SelectedRowIndex", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            else
            {
                if (myDataGridView.Rows.Count > 0)
                {
                    if (myDataGridView.Rows.Count < intRow + 1)
                    {
                        myDataGridView.CurrentCell = myDataGridView[0, 0];
                    }
                    else
                    {
                        myDataGridView.CurrentCell = myDataGridView[0, intRow];
                    }
                }
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

        private void EmployeeFilter_Click(object sender, EventArgs e)
        {
            rbnNone.Checked = true;

            if (this.rbnActive.Checked == true)
            {
                this.grbCloseEmployee.Visible = true;
                this.lblClosed.Visible = false;
            }
            else
            {
                this.grbCloseEmployee.Visible = false;
                this.lblClosed.Visible = true;
            }

            Load_Employees();
        }

        private void dgvEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnEmployeeDataGridViewLoaded == true)
            {
                if (pvtintEmployeeDataGridViewRowIndex != e.RowIndex)
                {
                    //ELR - Fix For Double Row Enter
                    pvtblnEmployeeDataGridViewLoaded = false;

                    pvtintEmployeeDataGridViewRowIndex = e.RowIndex;

                    //Set DataBound Index
                    clsISUtilities.DataViewIndex = Convert.ToInt32(this.dgvEmployeeDataGridView[6, e.RowIndex].Value);

                    //NB clsISUtilities.DataViewIndex is used Below
                    clsISUtilities.DataBind_DataView_Record_Show();

                    pvtintEmployeeNo = Convert.ToInt32(pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_NO"]);

                    //Set Finger Colours
                    pvtEmployeeFingerTemplateDataView = null;
                    pvtEmployeeFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["EmployeeFingerTemplate"], "COMPANY_NO = " + Convert.ToString(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND EMPLOYEE_NO = " + pvtintEmployeeNo, "FINGER_NO", DataViewRowState.CurrentRows);

                    if (pvtEmployeeFingerTemplateDataView.Count == 0)
                    {
                        this.dgvEmployeeDataGridView[1, e.RowIndex].Style = NoTemplateDataGridViewCellStyle;
                    }
                    else
                    {
                        this.dgvEmployeeDataGridView[1, e.RowIndex].Style = HasTemplateDataGridViewCellStyle;
                    }

                    Draw_Current_Employee_Fingers();

                    //Set Edit Buttons
                    if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_ENDDATE"] == System.DBNull.Value)
                    {
                        if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["PAYROLL_LINK"] == System.DBNull.Value)
                        {
                            this.btnUpdate.Enabled = true;
                        }
                        else
                        {
                            this.btnUpdate.Enabled = false;
                        }

                        if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_TAKEON_IND"].ToString() == "Y")
                        {
                            this.btnDelete.Enabled = false;
                        }
                        else
                        {
                            this.btnDelete.Enabled = true;
                        }
                    }
                    else
                    {
                        this.btnUpdate.Enabled = false;
                        this.btnDelete.Enabled = false;
                    }

                    if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["USE_WORK_TEL_IND"].ToString() == "Y")
                    {
                        this.rbnWorkTelCompany.Checked = true;

                        this.txtTelWork.Text = "";
                    }
                    else
                    {
                        this.rbnWorkTelOwn.Checked = true;
                    }

                    //ELR 2017-09-29
                    if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["USE_EMPLOYEE_NO_IND"].ToString() == "Y")
                    {
                        this.chkEmpNo.Checked = true;
                    }
                    else
                    {
                        this.chkEmpNo.Checked = false;
                    }

                    //Load Pay Category
                    this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
                    this.Clear_DataGridView(this.dgvChosenPayCategoryDataGridView);

                    this.pvtblnPayCategoryDataGridViewLoaded = false;
                    this.pvtblnChosenPayCategoryDataGridViewLoaded = false;

                    for (int intIndex = 0; intIndex < pvtPayCategoryDataView.Count; intIndex++)
                    {
                        if (this.rbnActive.Checked == true)
                        {
                            if (pvtPayCategoryDataView[intIndex]["CLOSED_IND"].ToString() == "Y")
                            {
                                continue;
                            }
                        }

                        DataView EmployeePayCategoryDataView = new DataView(pvtDataSet.Tables["EmployeePayCategory"],
                            "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND PAY_CATEGORY_NO = " + pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_NO"].ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_TYPE"].ToString() + "'",
                            "",
                            DataViewRowState.CurrentRows);

                        if (EmployeePayCategoryDataView.Count == 0)
                        {
                            this.dgvPayCategoryDataGridView.Rows.Add(pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_DESC"].ToString(),
                                                                     pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_NO"].ToString());
                        }
                        else
                        {
                            this.dgvChosenPayCategoryDataGridView.Rows.Add(pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_DESC"].ToString(),
                                                                          pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_NO"].ToString());
                        }
                    }

                    this.pvtblnPayCategoryDataGridViewLoaded = true;
                    this.pvtblnChosenPayCategoryDataGridViewLoaded = true;

                    if (this.dgvChosenPayCategoryDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvChosenPayCategoryDataGridView, 0);
                    }

                    //if (this.dgvEmployeeDataGridView.Enabled == true)
                    //{
                    //    this.dgvEmployeeDataGridView.Focus();
                    //}

                    //ELR - Fix For Double Row Enter
                    pvtblnEmployeeDataGridViewLoaded = true;
                }
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
                            pvtEmployeeFingerTemplateDataView = null;
                            pvtEmployeeFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["EmployeeFingerTemplate"], "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND FINGER_NO = " + pvtintFingerNo, "", DataViewRowState.CurrentRows);

                            if (pvtEmployeeFingerTemplateDataView.Count > 0)
                            {
                                pvtEmployeeFingerTemplateDataView[0].Row.Delete();
                            }

                            pvtEmployeeFingerTemplateDataView = null;
                            pvtEmployeeFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["EmployeeFingerTemplate"], "COMPANY_NO = " + Convert.ToString(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND EMPLOYEE_NO = " + pvtintEmployeeNo, "FINGER_NO", DataViewRowState.CurrentRows);

                            //Redraw Fingers
                            Draw_Current_Employee_Fingers();

                            if (pvtEmployeeFingerTemplateDataView.Count == 0)
                            {
                                this.dgvEmployeeDataGridView[1, dgvEmployeeDataGridView.CurrentCell.RowIndex].Style = this.NoTemplateDataGridViewCellStyle;
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
            this.chkClose.Checked = false;
            this.chkClose.Enabled = false;

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

            Draw_Current_Employee_Fingers();

            this.pnlFingers.Visible = true;

            this.lblEnrollMessage.ForeColor = Color.Black;
            this.pvtintCurrentFingerCount = 1;
            this.picMainFinger.Image = null;

            this.lblEnrollMessage.Text = pvtstrInitialMessage.Replace("#FINGER#", pvtstrFingerDescription);

            this.pnlEnroll.Visible = true;
            this.pnlFingers.Visible = false;
        }

        private void DP_StartCapture()
        {
            pvtCurrentReader.On_Captured -= new Reader.CaptureCallback(OnCaptured);
            pvtCurrentReader.On_Captured += new Reader.CaptureCallback(OnCaptured);
        }

        private void dgvPayrollTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPayrollTypeDataGridViewLoaded == true)
            {
                if (pvtintPayrollTypeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayrollTypeDataGridViewRowIndex = e.RowIndex;

                    this.rbnActive.Checked = true;

                    this.grbEmployeeLock.Visible = false;

                    pvtstrPayrollType = dgvPayrollTypeDataGridView[0, e.RowIndex].Value.ToString().Substring(0, 1);

                    pvtstrFilter = "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'";

                    pvtPayCategoryDataView = null;
                    pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"],
                        pvtstrFilter,
                        "",
                        DataViewRowState.CurrentRows);

                    Load_CurrentForm_Records();
                }
            }
        }

        private void dgvPayCategoryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvChosenPayCategoryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Work_Telephone_Click(object sender, EventArgs e)
        {
            bool blnEnabledValue = true;

            if (this.rbnWorkTelCompany.Checked == true)
            {
                pvtEmployeeDataView[clsISUtilities.DataViewIndex]["USE_WORK_TEL_IND"] = "Y";

                blnEnabledValue = false;

                this.txtTelWork.Text = "";
            }
            else
            {
                pvtEmployeeDataView[clsISUtilities.DataViewIndex]["USE_WORK_TEL_IND"] = "";
            }

            this.txtTelWork.Enabled = blnEnabledValue;
        }

        private void dgvPayCategoryDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                btnPayCategoryAdd_Click(sender, e);
            }
        }

        private void dgvChosenPayCategoryDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                btnPayCategoryRemove_Click(sender, e);
            }
        }

        private void lnkCancel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.pnlEnroll.Visible = false;
            this.btnClearFingers.Visible = true;
            this.pnlFingers.Visible = true;
            this.chkClose.Enabled = true;
            this.btnSave.Enabled = true;
        }

        private void frmTimeAttendanceEmployee_KeyDown(object sender, KeyEventArgs e)
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

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.btnUpdate.Enabled == false
            && this.btnSave.Enabled == false
            && this.tabControl.SelectedIndex == 0)
            {
                this.btnSave.Enabled = true;
                this.chkClose.Enabled = true;

                this.pnlEnroll.Visible = false;
                this.pnlFingers.Visible = true;
            }
        }

        private void frmTimeAttendanceEmployee_FormClosing(object sender, FormClosingEventArgs e)
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

        private void btnClearFingers_Click(object sender, EventArgs e)
        {
            if (pvtEmployeeFingerTemplateDataView.Count != 0)
            {
                for (int intRow = 0; intRow < pvtEmployeeFingerTemplateDataView.Count; intRow++)
                {
                    pvtEmployeeFingerTemplateDataView[intRow].Row.Delete();
                    intRow -= 1;
                }

                //Redraw Fingers
                Draw_Current_Employee_Fingers();

                this.dgvEmployeeDataGridView[1, dgvEmployeeDataGridView.CurrentCell.RowIndex].Style = this.NoTemplateDataGridViewCellStyle;
            }
        }

        private void rbnFilter_Click(object sender, EventArgs e)
        {
            Load_CurrentForm_Records();
        }

        private void dgvEmployeeDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 2)
            {
                if (double.Parse(dgvEmployeeDataGridView[2, e.RowIndex1].Value.ToString()) > double.Parse(dgvEmployeeDataGridView[2, e.RowIndex2].Value.ToString()))
                {
                    e.SortResult = 1;
                }
                else
                {
                    if (double.Parse(dgvEmployeeDataGridView[2, e.RowIndex1].Value.ToString()) < double.Parse(dgvEmployeeDataGridView[2, e.RowIndex2].Value.ToString()))
                    {
                        e.SortResult = -1;
                    }
                    else
                    {
                        e.SortResult = 0;
                    }
                }

                e.Handled = true;
            }
        }

        private void chkEmpNo_CheckedChanged(object sender, EventArgs e)
        {
            //ELR 2014-05-01
            if (this.btnSave.Enabled == true)
            {
                if (pvtEmployeeDataView.Count > 0)
                {
                    if (clsISUtilities.DataViewIndex > -1)
                    {
                        if (this.chkEmpNo.Checked == true)
                        {
                            pvtEmployeeDataView[clsISUtilities.DataViewIndex]["USE_EMPLOYEE_NO_IND"] = "Y";
                        }
                        else
                        {
                            pvtEmployeeDataView[clsISUtilities.DataViewIndex]["USE_EMPLOYEE_NO_IND"] = "N";

                            this.txtEmployeePin.Text = "";
                        }
                    }
                }
            }
        }
    }
}
