using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Web;
using DPUruNet;

namespace InteractPayrollClient
{
    public partial class frmUserClient : Form
    {
        clsISClientUtilities clsISClientUtilities;
        clsReadersToDp clsReadersToDp;

        ColorPalette greyScalePalette;

        private PictureBox pvtpicFinger;
       
        DataSet pvtDataSet;
        private DataView pvtUserFingerTemplateDataView;
        DataRow pvtTemplateDataRow;
       
        private bool pvtblnUserDataGridViewLoaded = false;
        private int pvtintUserRecCountCol = 7;

        private int pvtintUserDataGridViewRowIndex = -1;
        
        private ReaderCollection ReaderCollection;
        private Reader pvtCurrentReader;

        private int pvtintIdentifyThresholdScore;
        private int pvtintVerifyThresholdScore;
        private int pvtintCurrentFingerCount;

        DataGridViewCellStyle NoTemplateDataGridViewCellStyle;
        DataGridViewCellStyle LocalTemplateDataGridViewCellStyle;
        DataGridViewCellStyle ServerTemplateDataGridViewCellStyle;

        string pvtstrFingerReaderFileName = "FingerprintReaderChoice.txt";
        string pvtstrFingerprintReaderName = "None";

        private bool pvtblnFingerprintDeviceOpened = false;

        private string pvtstrInitialMessage = "To begin, place and hold your #FINGER# finger on the Fingerprint Reader until the screen indicates that the scan was successful. Repeat for each of the remaining scans.";
        private string pvtstrSuccessful = "The Scan was successful.\nPlace your #FINGER# finger on the Fingerprint Reader again.";
        private string pvtstrScanDifferent = "The finger scanned is NOT the same as the previous one or the Image is of BAD Quality. Try again. Place your #FINGER# finger flat on the fingerprint reader.";
        private string pvtstrScanBadImage = "The finger scanned is of bad Quality. Try again. Place your #FINGER# finger flat on the fingerprint reader.";

        private string pvtstrFingerDescription;

        private byte[] pvtbytFinger1;
        private byte[] pvtbytFinger2;
        private byte[] pvtbytFinger3;
        private byte[] pvtbyteArrayPreviousTemplate;
   
        ToolStripMenuItem miLinkedMenuItem;
        
        private string pvtstrSoftwareToUse = "";

        private string pvtstrFingerprintDeviceOpenedFailureMessage = "";

        private int pvtintFingerNo;

        private Int64 pvtint64UserNo = -1;

        public frmUserClient()
        {
            InitializeComponent();

            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 114;
                this.dgvUserDataGridView.Height += 114;
                
                this.grbMain.Top += 114;
                this.grbFingerInfo.Top += 114;
                this.grbFilter.Top += 114;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmUserClient_Load(object sender, EventArgs e)
        {
            try
            {
                this.Show();
                this.Refresh();

                miLinkedMenuItem = (ToolStripMenuItem)AppDomain.CurrentDomain.GetData("LinkedMenuItem");

                clsISClientUtilities = new clsISClientUtilities(this, "busUserClient");

                //2017-06-06 Create Greyscale Palette
                Bitmap bmpPalette = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);

                greyScalePalette = bmpPalette.Palette;

                for (int i = 0; i < bmpPalette.Palette.Entries.Length; i++)
                {
                    greyScalePalette.Entries[i] = Color.FromArgb(i, i, i);
                }

                FileInfo fiFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + pvtstrFingerReaderFileName);

                if (fiFileInfo.Exists == true)
                {
                    StreamReader srStreamReader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + pvtstrFingerReaderFileName);

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

                this.lblUser.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);

                NoTemplateDataGridViewCellStyle = new DataGridViewCellStyle();
                NoTemplateDataGridViewCellStyle.BackColor = Color.Salmon;
                NoTemplateDataGridViewCellStyle.SelectionBackColor = Color.Salmon;

                LocalTemplateDataGridViewCellStyle = new DataGridViewCellStyle();
                LocalTemplateDataGridViewCellStyle.BackColor = Color.SeaGreen;
                LocalTemplateDataGridViewCellStyle.SelectionBackColor = Color.SeaGreen;

                ServerTemplateDataGridViewCellStyle = new DataGridViewCellStyle();
                ServerTemplateDataGridViewCellStyle.BackColor = Color.Aquamarine;
                ServerTemplateDataGridViewCellStyle.SelectionBackColor = Color.Aquamarine;

                Clear_FingerPrint_Images();
             
                object[] objParm = new object[2];
                objParm[0] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                byte[] bytCompress = (byte[])clsISClientUtilities.DynamicFunction("Get_Form_Records_New", objParm,false);
                pvtDataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(bytCompress);
                
                pvtintIdentifyThresholdScore = Convert.ToInt32(pvtDataSet.Tables["FingerprintThreshold"].Rows[0]["IDENTIFY_THRESHOLD_VALUE"]);
                pvtintVerifyThresholdScore = Convert.ToInt32(pvtDataSet.Tables["FingerprintThreshold"].Rows[0]["VERIFY_THRESHOLD_VALUE"]);

                if (pvtDataSet.Tables["SoftwareToUse"].Rows.Count == 0)
                {
                    pvtstrSoftwareToUse = "D";
                }
                else
                {
                    pvtstrSoftwareToUse = pvtDataSet.Tables["SoftwareToUse"].Rows[0]["FINGERPRINT_SOFTWARE_IND"].ToString();
                }
                
                Load_CurrentForm_Records();
            }
            catch (Exception ex)
            {
                clsISClientUtilities.ErrorHandler(ex);
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
                    CustomClientMessageBox.Show("Bad Image",this.Text,MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
                else
                {
                    if (strOK == "E")
                    {
                        CustomClientMessageBox.Show("Error on Web Server Layer", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                    }
                    else
                    {
                        if (strOK == "L")
                        {
                            CustomClientMessageBox.Show("Griaule Licence Problem", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Close();
                        }
                        else
                        {
                            if (strOK == "F")
                            {
                                CustomClientMessageBox.Show("Enrollment Failure", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                                                this.picFinger2.Image = global::User.Properties.Resources.FingerPrintCorrect64;
                                                this.picFinger3.Image = global::User.Properties.Resources.FingerPrintQuestion64;

                                                break;

                                            case 3:

                                                this.picFinger3.Image = global::User.Properties.Resources.FingerPrintCorrect64;
                                                this.picFinger4.Image = global::User.Properties.Resources.FingerPrintQuestion64;

                                                break;

                                            case 4:

                                                this.picFinger4.Image = global::User.Properties.Resources.FingerPrintCorrect64;

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

                                                this.picFinger2.Image = global::User.Properties.Resources.FingerPrintError64;

                                                break;

                                            case 3:
                                                this.picFinger3.Image = global::User.Properties.Resources.FingerPrintError64;

                                                break;

                                            case 4:

                                                this.picFinger4.Image = global::User.Properties.Resources.FingerPrintError64;

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

                                        this.picFinger1.Image = global::User.Properties.Resources.FingerPrintCorrect64;
                                        this.picFinger2.Image = global::User.Properties.Resources.FingerPrintQuestion64;
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

                                        this.dgvUserDataGridView[0, dgvUserDataGridView.CurrentCell.RowIndex].Style = LocalTemplateDataGridViewCellStyle;
                                    }
                                    else
                                    {
                                        strMessage = "UNSUCCESSFUL.";
                                    }

                                    CustomClientMessageBox.Show("Enrollment of " + pvtstrFingerDescription + " finger " + strMessage, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

                                    this.pnlEnroll.Visible = false;
                                    this.btnClearFingers.Visible = true;
                                    this.pnlFingers.Visible = true;
                                    this.btnSave.Enabled = true;

                                    this.Set_DataGridView_SelectedRowIndex(this.dgvUserDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvUserDataGridView));
                                 
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

        private void Load_CurrentForm_Records()
        {
            Clear_FingerPrint_Images();
            
            int intCurrentRow = 0;

            pvtblnUserDataGridViewLoaded = false;

            this.Clear_DataGridView(dgvUserDataGridView);

            pvtintIdentifyThresholdScore = Convert.ToInt32(pvtDataSet.Tables["FingerprintThreshold"].Rows[0]["IDENTIFY_THRESHOLD_VALUE"]);
            pvtintVerifyThresholdScore = Convert.ToInt32(pvtDataSet.Tables["FingerprintThreshold"].Rows[0]["VERIFY_THRESHOLD_VALUE"]);

            for (int intRowCount = 0; intRowCount < pvtDataSet.Tables["User"].Rows.Count; intRowCount++)
            {
                //Set Finger Colours
                pvtUserFingerTemplateDataView = null;
                pvtUserFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["UserFingerTemplate"], "USER_NO = " + pvtDataSet.Tables["User"].Rows[intRowCount]["USER_NO"].ToString(), "FINGER_RESIDE_IND", DataViewRowState.CurrentRows);

                if (this.rbnTemplateMissing.Checked == true)
                {
                    if (pvtUserFingerTemplateDataView.Count > 0)
                    {
                        continue;
                    }
                }
                else
                {
                    if (this.rbnLocal.Checked == true
                    || this.rbnServer.Checked == true)
                    {
                        string strOption = "S";

                        if (this.rbnLocal.Checked == true)
                        {
                            strOption = "L";
                        }

                        DataView EmployeeFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["UserFingerTemplate"], "USER_NO = " + pvtDataSet.Tables["User"].Rows[intRowCount]["USER_NO"].ToString() + " AND FINGER_RESIDE_IND = '" + strOption + "'", "", DataViewRowState.CurrentRows);

                        if (EmployeeFingerTemplateDataView.Count == 0)
                        {
                            continue;
                        }
                    }
                }

                this.dgvUserDataGridView.Rows.Add("",
                                                  "",
                                                  "",
                                                  pvtDataSet.Tables["User"].Rows[intRowCount]["USER_ID"].ToString(),
                                                  pvtDataSet.Tables["User"].Rows[intRowCount]["USER_NO"].ToString(),
                                                  pvtDataSet.Tables["User"].Rows[intRowCount]["SURNAME"].ToString(), 
                                                  pvtDataSet.Tables["User"].Rows[intRowCount]["FIRSTNAME"].ToString(),
                                                  intRowCount.ToString());

                if (pvtUserFingerTemplateDataView.Count == 0)
                {
                    this.dgvUserDataGridView[0, dgvUserDataGridView.Rows.Count - 1].Style = NoTemplateDataGridViewCellStyle;
                }
                else
                {
                    for (int intRow = 0; intRow < pvtUserFingerTemplateDataView.Count; intRow++)
                    {
                        if (pvtUserFingerTemplateDataView[intRow]["FINGER_RESIDE_IND"].ToString() == "S")
                        {
                            this.dgvUserDataGridView[1, dgvUserDataGridView.Rows.Count - 1].Style = ServerTemplateDataGridViewCellStyle;
                        }
                        else
                        {
                            this.dgvUserDataGridView[2, dgvUserDataGridView.Rows.Count - 1].Style = LocalTemplateDataGridViewCellStyle;
                        }
                    }
                }

                if (pvtint64UserNo == Convert.ToInt64(pvtDataSet.Tables["User"].Rows[intRowCount]["USER_NO"]))
                {
                    intCurrentRow = this.dgvUserDataGridView.Rows.Count - 1;
                }
            }

            pvtblnUserDataGridViewLoaded = true;

            Set_Form_For_Read();

            if (this.dgvUserDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvUserDataGridView, intCurrentRow);
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

            return intReturnIndex;
        }

        public void Set_DataGridView_SelectedRowIndex(DataGridView myDataGridView, int intRow)
        {
            //Fires DataGridView RowEnter Function
            if (myDataGridView.CurrentCell.RowIndex == intRow)
            {
                DataGridViewCellEventArgs myDataGridViewCellEventArgs = new DataGridViewCellEventArgs(0, intRow);

                switch (myDataGridView.Name)
                {
                    case "dgvUserDataGridView":

                        pvtintUserDataGridViewRowIndex = -1;
                        dgvUserDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    default:

                        System.Windows.Forms.MessageBox.Show("No Entry for " + myDataGridView.Name + " in Set_DataGridView_SelectedRowIndex", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            else
            {
                pvtintUserDataGridViewRowIndex = -1;
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

        public void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.pvtDataSet.RejectChanges();

            try
            {
                if (pvtblnFingerprintDeviceOpened == true)
                {
                    if (pvtstrFingerprintReaderName == "URU4500 (Digital Persona)")
                    {
                        DP_StopCapture();
                    }
                    else
                    {
                        clsReadersToDp.StopCapture();
                    }
                }
            }
            catch
            {
            }

            Set_Form_For_Read();

            Load_CurrentForm_Records();
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
                            dgResult = CustomClientMessageBox.Show("Select Yes to Delete Fingerprint, No to Enroll Fingerprint.", this.Text, MessageBoxButtons.YesNoCancel,MessageBoxIcon.Information);
                        }
                        else
                        {
                            dgResult = CustomClientMessageBox.Show("Select OK to Delete Fingerprint.", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        }

                        if (dgResult == DialogResult.Yes
                        || dgResult == DialogResult.OK)
                        {
                            pvtUserFingerTemplateDataView = null;
                            pvtUserFingerTemplateDataView = new DataView(pvtDataSet.Tables["UserFingerTemplate"], "USER_NO = " + pvtint64UserNo + " AND FINGER_NO = " + pvtintFingerNo.ToString(), "", DataViewRowState.CurrentRows);

                            pvtUserFingerTemplateDataView[0].Row.Delete();

                            //Redraw Fingers
                            Draw_Current_User_Fingers();
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
                clsISClientUtilities.ErrorHandler(ex);
            }
        }

        private void Clicked_Finger()
        {
            this.btnSave.Enabled = false;
            btnClearFingers.Visible = false;

            this.lblFingerInformation.Text = "Place Finger on Reader";
            this.lblFingerInformation.ForeColor = Color.Black;
            
            switch (pvtintFingerNo)
            {
                case 0:

                    pvtstrFingerDescription = "Left Pinkie";

                    break;

                case 1:

                    pvtstrFingerDescription = "Left Ring";

                    break;

                case 2:

                    pvtstrFingerDescription = "Left Middle";

                    break;

                case 3:

                    pvtstrFingerDescription = "Left Index";

                    break;

                case 4:

                    pvtstrFingerDescription = "Left Thumb";

                    break;

                case 5:

                    pvtstrFingerDescription = "Right Thumb";

                    break;

                case 6:

                    pvtstrFingerDescription = "Right Index";

                    break;

                case 7:

                    pvtstrFingerDescription = "Right Middle";

                    break;

                case 8:

                    pvtstrFingerDescription = "Right Ring";

                    break;

                case 9:

                    pvtstrFingerDescription = "Right Pinkie";

                    break;
            }

            this.picFinger1.Image = global::User.Properties.Resources.FingerPrintQuestion64;
            this.picFinger2.Image = global::User.Properties.Resources.FingerPrint64;
            this.picFinger3.Image = global::User.Properties.Resources.FingerPrint64;
            this.picFinger4.Image = global::User.Properties.Resources.FingerPrint64;

            this.pnlEnroll.Visible = false;

            Draw_Current_User_Fingers();

            this.pnlFingers.Visible = true;

            this.lblEnrollMessage.ForeColor = Color.Black;
            this.pvtintCurrentFingerCount = 1;
            this.picMainFinger.Image = null;
          
            this.lblEnrollMessage.Text = pvtstrInitialMessage.Replace("#FINGER#", pvtstrFingerDescription);

            this.pnlEnroll.Visible = true;
            this.pnlFingers.Visible = false;

            if (pvtstrFingerprintReaderName == "URU4500 (Digital Persona)")
            {
                DP_StartCapture();
            }
            else
            {
                clsReadersToDp.StartCapture();
            }
        }

        private void Clear_FingerPrint_Images()
        {
            this.picLPinkie.Image = global::User.Properties.Resources.LPinkieClear;
            this.picLRing.Image = global::User.Properties.Resources.LRingClear;
            this.picLMiddle.Image = global::User.Properties.Resources.LMiddleClear;
            this.picLIndex.Image = global::User.Properties.Resources.LIndexClear;
            this.picLThumb.Image = global::User.Properties.Resources.LThumbClear;

            this.picRThumb.Image = global::User.Properties.Resources.RThumbClear;
            this.picRIndex.Image = global::User.Properties.Resources.RIndexClear;
            this.picRMiddle.Image = global::User.Properties.Resources.RMiddleClear;
            this.picRRing.Image = global::User.Properties.Resources.RRingClear;
            this.picRPinkie.Image = global::User.Properties.Resources.RPinkieClear;

            this.picLPinkie.Tag = "N";
            this.picLRing.Tag = "N";
            this.picLMiddle.Tag = "N";
            this.picLIndex.Tag = "N";
            this.picLThumb.Tag = "N";

            this.picRThumb.Tag = "N";
            this.picRIndex.Tag = "N";
            this.picRMiddle.Tag = "N";
            this.picRRing.Tag = "N";
            this.picRPinkie.Tag = "N";
        }

        private void Draw_Current_User_Fingers()
        {
            //Set Finger Colours
            pvtUserFingerTemplateDataView = null;
            pvtUserFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["UserFingerTemplate"], "USER_NO = " + pvtint64UserNo + " AND FINGER_NO = 0", "", DataViewRowState.CurrentRows);

            if (pvtUserFingerTemplateDataView.Count == 0)
            {
                picLPinkie.Image = global::User.Properties.Resources.LPinkieClear;
                picLPinkie.Tag = "N";
            }
            else
            {
                if (pvtUserFingerTemplateDataView[0]["FINGER_RESIDE_IND"].ToString() == "S")
                {
                    picLPinkie.Image = global::User.Properties.Resources.LPinkieServer;
                }
                else
                {
                    picLPinkie.Image = global::User.Properties.Resources.LPinkieLocal;
                }

                picLPinkie.Tag = "Y";
            }

            pvtUserFingerTemplateDataView = null;
            pvtUserFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["UserFingerTemplate"], "USER_NO = " + pvtint64UserNo + " AND FINGER_NO = 1", "", DataViewRowState.CurrentRows);

            if (pvtUserFingerTemplateDataView.Count == 0)
            {
                picLRing.Image = global::User.Properties.Resources.LRingClear;
                picLRing.Tag = "N";
            }
            else
            {
                if (pvtUserFingerTemplateDataView[0]["FINGER_RESIDE_IND"].ToString() == "S")
                {
                    picLRing.Image = global::User.Properties.Resources.LRingServer;
                }
                else
                {
                    picLRing.Image = global::User.Properties.Resources.LRingLocal;
                }

                picLRing.Tag = "Y";
            }

            pvtUserFingerTemplateDataView = null;
            pvtUserFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["UserFingerTemplate"], "USER_NO = " + pvtint64UserNo + " AND FINGER_NO = 2", "", DataViewRowState.CurrentRows);

            if (pvtUserFingerTemplateDataView.Count == 0)
            {
                picLMiddle.Image = global::User.Properties.Resources.LMiddleClear;
                picLMiddle.Tag = "N";
            }
            else
            {
                if (pvtUserFingerTemplateDataView[0]["FINGER_RESIDE_IND"].ToString() == "S")
                {
                    picLMiddle.Image = global::User.Properties.Resources.LMiddleServer;
                }
                else
                {
                    picLMiddle.Image = global::User.Properties.Resources.LMiddleLocal;
                }

                picLMiddle.Tag = "Y";
            }

            pvtUserFingerTemplateDataView = null;
            pvtUserFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["UserFingerTemplate"], "USER_NO = " + pvtint64UserNo + " AND FINGER_NO = 3", "", DataViewRowState.CurrentRows);

            if (pvtUserFingerTemplateDataView.Count == 0)
            {
                picLIndex.Image = global::User.Properties.Resources.LIndexClear;
                picLIndex.Tag = "N";
            }
            else
            {
                if (pvtUserFingerTemplateDataView[0]["FINGER_RESIDE_IND"].ToString() == "S")
                {
                    picLIndex.Image = global::User.Properties.Resources.LIndexServer;
                }
                else
                {
                    picLIndex.Image = global::User.Properties.Resources.LIndexLocal;
                }

                picLIndex.Tag = "Y";
            }

            pvtUserFingerTemplateDataView = null;
            pvtUserFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["UserFingerTemplate"], "USER_NO = " + pvtint64UserNo + " AND FINGER_NO = 4", "", DataViewRowState.CurrentRows);

            if (pvtUserFingerTemplateDataView.Count == 0)
            {
                picLThumb.Image = global::User.Properties.Resources.LThumbClear;
                picLThumb.Tag = "N";
            }
            else
            {
                if (pvtUserFingerTemplateDataView[0]["FINGER_RESIDE_IND"].ToString() == "S")
                {
                    picLThumb.Image = global::User.Properties.Resources.LThumbServer;
                }
                else
                {
                    picLThumb.Image = global::User.Properties.Resources.LThumbLocal;
                }

                picLThumb.Tag = "Y";
            }

            pvtUserFingerTemplateDataView = null;
            pvtUserFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["UserFingerTemplate"], "USER_NO = " + pvtint64UserNo + " AND FINGER_NO = 5", "", DataViewRowState.CurrentRows);

            if (pvtUserFingerTemplateDataView.Count == 0)
            {
                picRThumb.Image = global::User.Properties.Resources.RThumbClear;
                picRThumb.Tag = "N";
            }
            else
            {
                if (pvtUserFingerTemplateDataView[0]["FINGER_RESIDE_IND"].ToString() == "S")
                {
                    picRThumb.Image = global::User.Properties.Resources.RThumbServer;
                }
                else
                {
                    picRThumb.Image = global::User.Properties.Resources.RThumbLocal;
                }

                picRThumb.Tag = "Y";
            }

            pvtUserFingerTemplateDataView = null;
            pvtUserFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["UserFingerTemplate"], "USER_NO = " + pvtint64UserNo + " AND FINGER_NO = 6", "", DataViewRowState.CurrentRows);

            if (pvtUserFingerTemplateDataView.Count == 0)
            {
                picRIndex.Image = global::User.Properties.Resources.RIndexClear;
                picRIndex.Tag = "N";
            }
            else
            {
                if (pvtUserFingerTemplateDataView[0]["FINGER_RESIDE_IND"].ToString() == "S")
                {
                    picRIndex.Image = global::User.Properties.Resources.RIndexServer;
                }
                else
                {
                    picRIndex.Image = global::User.Properties.Resources.RIndexLocal;
                }

                picRIndex.Tag = "Y";
            }

            pvtUserFingerTemplateDataView = null;
            pvtUserFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["UserFingerTemplate"], "USER_NO = " + pvtint64UserNo + " AND FINGER_NO = 7", "", DataViewRowState.CurrentRows);

            if (pvtUserFingerTemplateDataView.Count == 0)
            {
                picRMiddle.Image = global::User.Properties.Resources.RMiddleClear;
                picRMiddle.Tag = "N";
            }
            else
            {
                if (pvtUserFingerTemplateDataView[0]["FINGER_RESIDE_IND"].ToString() == "S")
                {
                    picRMiddle.Image = global::User.Properties.Resources.RMiddleServer;
                }
                else
                {
                    picRMiddle.Image = global::User.Properties.Resources.RMiddleLocal;
                }

                picRMiddle.Tag = "Y";
            }

            pvtUserFingerTemplateDataView = null;
            pvtUserFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["UserFingerTemplate"], "USER_NO = " + pvtint64UserNo + " AND FINGER_NO = 8", "", DataViewRowState.CurrentRows);

            if (pvtUserFingerTemplateDataView.Count == 0)
            {
                picRRing.Image = global::User.Properties.Resources.RRingClear;
                picRRing.Tag = "N";
            }
            else
            {
                if (pvtUserFingerTemplateDataView[0]["FINGER_RESIDE_IND"].ToString() == "S")
                {
                    picRRing.Image = global::User.Properties.Resources.RRingServer;
                }
                else
                {
                    picRRing.Image = global::User.Properties.Resources.RRingLocal;
                }

                picRRing.Tag = "Y";
            }

            pvtUserFingerTemplateDataView = null;
            pvtUserFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["UserFingerTemplate"], "USER_NO = " + pvtint64UserNo + " AND FINGER_NO = 9", "", DataViewRowState.CurrentRows);

            if (pvtUserFingerTemplateDataView.Count == 0)
            {
                picRPinkie.Image = global::User.Properties.Resources.RPinkieClear;
                picRPinkie.Tag = "N";
            }
            else
            {
                if (pvtUserFingerTemplateDataView[0]["FINGER_RESIDE_IND"].ToString() == "S")
                {
                    picRPinkie.Image = global::User.Properties.Resources.RPinkieServer;
                }
                else
                {
                    picRPinkie.Image = global::User.Properties.Resources.RPinkieLocal;
                }

                picRPinkie.Tag = "Y";
            }
        }
        
        public void btnUpdate_Click(object sender, EventArgs e)
        {
            this.Text += " - Update";

            Set_Form_For_Edit();
        }

        private void Set_Form_For_Edit()
        {
            this.btnUpdate.Enabled = false;
            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;
            
            picUserLock.Visible = true;
            lblMessage.Visible = true;

            this.rbnAll.Enabled = false;
            this.rbnTemplateMissing.Enabled = false;
            this.rbnLocal.Enabled = false;
            this.rbnServer.Enabled = false;

            this.rbnAll.Checked = true;
           
            this.btnClearFingers.Visible = true;

            this.dgvUserDataGridView.Enabled = false;
        }

        private void Set_Form_For_Read()
        {
            if (this.Text.IndexOf(" - Update") > 0)
            {
                this.Text = this.Text.Substring(0, this.Text.IndexOf(" - Update"));
            }

            this.picUserLock.Visible = false;
            
            this.rbnAll.Enabled = true;
            this.rbnTemplateMissing.Enabled = true;
            this.rbnLocal.Enabled = true;
            this.rbnServer.Enabled = true;
            
            this.lblMessage.Visible = false;
            this.pnlFingers.Visible = true;
            this.pnlEnroll.Visible = false;
 
            if (this.dgvUserDataGridView.Rows.Count > 0)
            {
                this.btnUpdate.Enabled = true;
            }
            else
            {
                this.btnUpdate.Enabled = false;
            }
            
            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.dgvUserDataGridView.Enabled = true;

            this.btnClearFingers.Visible = false;
            
            this.lblMessage.Visible = false;
        }

        public void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                pvtUserFingerTemplateDataView = null;
                pvtUserFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["UserFingerTemplate"], "", "", DataViewRowState.Added | DataViewRowState.ModifiedOriginal | DataViewRowState.Deleted);

                if (pvtUserFingerTemplateDataView.Count == 0)
                {
                    CustomClientMessageBox.Show("Capture Finger Template/s\nClick on Finger/s.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return;
                }

                DataSet TempDataSet = new DataSet();
                //Add EmployeePayCategory Table 
                TempDataSet.Tables.Add(pvtDataSet.Tables["UserFingerTemplate"].Clone());

                for (int intRow = 0; intRow < pvtUserFingerTemplateDataView.Count; intRow++)
                {
                    TempDataSet.Tables["UserFingerTemplate"].ImportRow(pvtUserFingerTemplateDataView[intRow].Row);
                }

                //Compress DataSet
                byte[] pvtbytCompress = clsISClientUtilities.Compress_DataSet(TempDataSet);

                object[] objParm = new object[2];
                objParm[0] = pvtint64UserNo;
                objParm[1] = pvtbytCompress;

                clsISClientUtilities.DynamicFunction("Update_Records", objParm,true);

                this.pvtDataSet.AcceptChanges();

                this.Load_CurrentForm_Records();
            }
            catch (Exception ex)
            {
                clsISClientUtilities.ErrorHandler(ex);
            }
        }
        
        private void pnlFingers_VisibleChanged(object sender, EventArgs e)
        {
            if (pnlFingers.Visible == true)
            {
                Draw_Current_User_Fingers();

                if (this.btnUpdate.Enabled == false)
                {
                    if (this.dgvUserDataGridView.Rows.Count > 0)
                    {
                        this.btnSave.Enabled = true;
                    }
                }

                this.pnlEnroll.Visible = false;
            }
        }

        private void btnClearFingers_Click(object sender, EventArgs e)
        {
            pvtUserFingerTemplateDataView = null;
            pvtUserFingerTemplateDataView = new DataView(pvtDataSet.Tables["UserFingerTemplate"], "USER_NO = " + pvtint64UserNo, "", DataViewRowState.CurrentRows);
           
            if (pvtUserFingerTemplateDataView.Count != 0)
            {
                for (int intRow = 0; intRow < pvtUserFingerTemplateDataView.Count; intRow++)
                {
                    pvtUserFingerTemplateDataView[intRow].Row.Delete();
                    intRow -= 1;
                }

                //Redraw Fingers
                Draw_Current_User_Fingers();
            }
        }
        
        private void lnkCancel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.pnlEnroll.Visible = false;
            this.btnClearFingers.Visible = true;
            this.pnlFingers.Visible = true;
            this.btnSave.Enabled = true;
        }

        private void frmUserClient_FormClosing(object sender, FormClosingEventArgs e)
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

            miLinkedMenuItem.Enabled = true;
        }

        private void dgvUserDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnUserDataGridViewLoaded == true)
            {
                if (pvtintUserDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintUserDataGridViewRowIndex = e.RowIndex;

                    int intDataTableRow = Convert.ToInt32(dgvUserDataGridView[pvtintUserRecCountCol, e.RowIndex].Value);

                    pvtint64UserNo = Convert.ToInt64(pvtDataSet.Tables["User"].Rows[intDataTableRow]["USER_NO"]);
                    
                    Draw_Current_User_Fingers();
                }
            }
        }

        private void dgvUserDataGridView_Sorted(object sender, EventArgs e)
        {
            if (dgvUserDataGridView.Rows.Count > 0)
            {
                if (dgvUserDataGridView.SelectedRows.Count > 0)
                {
                    if (dgvUserDataGridView.SelectedRows[0].Selected == true)
                    {
                        dgvUserDataGridView.FirstDisplayedScrollingRowIndex = dgvUserDataGridView.SelectedRows[0].Index;
                    }
                }
            }
        }

        private void dgvUserDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 4)
            {
                if (double.Parse(e.CellValue1.ToString()) > double.Parse(e.CellValue2.ToString()))
                {
                    e.SortResult = 1;
                }
                else if (double.Parse(e.CellValue1.ToString()) < double.Parse(e.CellValue2.ToString()))
                {
                    e.SortResult = -1;
                }
                else
                {
                    e.SortResult = 0;
                }

                e.Handled = true;
            } 
        }

        private void rbnAll_Click(object sender, EventArgs e)
        {
            Load_CurrentForm_Records();
        }

        private void rbnFilter_Click(object sender, EventArgs e)
        {
            Load_CurrentForm_Records();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.Timer.Enabled = false;

            CustomClientMessageBox.Show(pvtstrFingerprintDeviceOpenedFailureMessage, "Fingerprint Open Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void frmUserClient_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.btnSave.Enabled == true)
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
    }
}
