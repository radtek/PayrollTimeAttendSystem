using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DPUruNet;

namespace InteractPayrollClient
{
    public partial class frmEmployeeClient : Form
    {
        clsISClientUtilities clsISClientUtilities;
        clsReadersToDp clsReadersToDp;
        
        ToolStripMenuItem miLinkedMenuItem;

        ColorPalette greyScalePalette;

        private ReaderCollection ReaderCollection;
        private Reader pvtCurrentReader;

        private int pvtintIdentifyThresholdScore;
        private int pvtintVerifyThresholdScore;
        private int pvtintCurrentFingerCount;

        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
        private int pvtintCompanyDataGridViewRowIndex = -1;
        private int pvtintEmployeeDataGridViewRowIndex = -1;

        DataGridViewCellStyle NoTemplateDataGridViewCellStyle;
        DataGridViewCellStyle NormalTemplateDataGridViewCellStyle;
        DataGridViewCellStyle LocalTemplateDataGridViewCellStyle;
        DataGridViewCellStyle ServerTemplateDataGridViewCellStyle;
        
        string pvtstrFingerReaderFileName = "FingerprintReaderChoice.txt";
        string pvtstrFingerprintReaderName = "None";

        private bool pvtblnFingerprintDeviceOpened = false;

        private string pvtstrInitialMessage = "To begin, place and hold your #FINGER# finger on the Fingerprint Reader until the screen indicates that the scan was successful. Repeat for each of the remaining scans.";
        private string pvtstrSuccessful = "The Scan was successful.\nPlace your #FINGER# finger on the Fingerprint Reader again.";
        private string pvtstrScanDifferent = "The finger scanned is NOT the same as the previous one or the Image is of BAD Quality. Try again. Place your #FINGER# finger flat on the fingerprint reader.";
        private string pvtstrScanBadImage = "The finger scanned is of bad Quality. Try again. Place your #FINGER# finger flat on the fingerprint reader.";
        
        private byte[] pvtbytFinger1;
        private byte[] pvtbytFinger2;
        private byte[] pvtbytFinger3;
        private byte[] pvtbyteArrayPreviousTemplate;
        private string pvtstrFingerDescription;
     
        private DataSet pvtDataSet;
        private DataView pvtEmployeeFingerTemplateDataView;
        private DataView pvtEmployeeDataView;

        private DataView pvtEmployeePayCategoryDataView;
        private DataView pvtPayCategoryDataView;
        private DataRow pvtTemplateDataRow;

        private bool pvtblnEmployeeDataGridViewLoaded = false;
        private int pvtintEmployeeKeyCol = 13;
        private int pvtintCurrentEmployeeRow = -1;

        private PictureBox pvtpicFinger;
        private Int64 pvtint64CompanyNo;
        private Int64 pvtint64EmployeeNo;
        private int pvtintFingerNo;

        private bool pvtblnCompanyDataGridViewLoaded = false;
        private int pvtintCompanyRecCountCol = 4;

        private bool pvtblnvPayrollTypeDataGridViewLoaded = false;
        private string pvtstrPayrollType = "";

        private string pvtstrFingerprintDeviceOpenedFailureMessage = "";

        MenuItem mniMenuItem24;

        private string pvtstrSoftwareToUse = "";

        public frmEmployeeClient()
        {
            InitializeComponent();

            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 114;

                this.dgvEmployeeDataGridView.Height += 114;

                this.grbFingerprints.Top += 114;
                this.grbDetails.Top += 114;
            }
        }

        private void frmEmployeeClient_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISClientUtilities = new clsISClientUtilities(this, "busEmployeeClient");

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

                switch (pvtstrFingerprintReaderName)
                {
                    case "Curve / Columbo (Integrated Biometrics)":

                        clsReadersToDp = new InteractPayrollClient.clsReadersToDp(this.picMainFinger,OnCaptured);

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

                        try
                        {
                            //DP Readers attached to Machine
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
                }

                if (pvtblnFingerprintDeviceOpened == false)
                {
                    this.lblMessage.Text = "You may delete an enrolled finger by clicking on the highlighted finger or by Keying the finger's Number.";
                }

                NoTemplateDataGridViewCellStyle = new DataGridViewCellStyle();
                NoTemplateDataGridViewCellStyle.BackColor = Color.Salmon;
                NoTemplateDataGridViewCellStyle.SelectionBackColor = Color.Salmon;

                NormalTemplateDataGridViewCellStyle = new DataGridViewCellStyle();
                NormalTemplateDataGridViewCellStyle.BackColor = SystemColors.Control;
                NormalTemplateDataGridViewCellStyle.SelectionBackColor = SystemColors.Control;
                
                LocalTemplateDataGridViewCellStyle = new DataGridViewCellStyle();
                LocalTemplateDataGridViewCellStyle.BackColor = Color.SeaGreen;
                LocalTemplateDataGridViewCellStyle.SelectionBackColor = Color.SeaGreen;

                ServerTemplateDataGridViewCellStyle = new DataGridViewCellStyle();
                ServerTemplateDataGridViewCellStyle.BackColor = Color.Aquamarine;
                ServerTemplateDataGridViewCellStyle.SelectionBackColor = Color.Aquamarine;
#if(DEBUG)
                int intWidth = this.dgvCompanyDataGridView.RowHeadersWidth;

                if (this.dgvCompanyDataGridView.ScrollBars == ScrollBars.Vertical
                    | this.dgvCompanyDataGridView.ScrollBars == ScrollBars.Both)
                {
                    intWidth += 19;
                }

                for (int intCol = 0; intCol < this.dgvCompanyDataGridView.ColumnCount; intCol++)
                {
                    if (this.dgvCompanyDataGridView.Columns[intCol].Visible == true)
                    {
                        intWidth += this.dgvCompanyDataGridView.Columns[intCol].Width;
                    }
                }

                if (intWidth != this.dgvCompanyDataGridView.Width)
                {
                    System.Windows.Forms.MessageBox.Show("Width should be " + intWidth.ToString());
                }

                int intHeight = this.dgvCompanyDataGridView.ColumnHeadersHeight + 2;
                int intNewHeight = this.dgvCompanyDataGridView.RowTemplate.Height / 2;

                for (int intRow = 0; intRow < 200; intRow++)
                {
                    intHeight += this.dgvCompanyDataGridView.RowTemplate.Height;

                    if (intHeight == this.dgvCompanyDataGridView.Height)
                    {
                        break;
                    }
                    else
                    {
                        if (intHeight > this.dgvCompanyDataGridView.Height)
                        {
                            System.Windows.Forms.MessageBox.Show("Height should be " + intHeight.ToString());
                            break;
                        }
                        else
                        {

                            if (intHeight + intNewHeight > this.dgvCompanyDataGridView.Height)
                            {
                                System.Windows.Forms.MessageBox.Show("Height should be " + intHeight.ToString());
                                break;
                            }
                        }
                    }
                }

                intWidth = this.dgvEmployeeDataGridView.RowHeadersWidth;

                if (this.dgvEmployeeDataGridView.ScrollBars == ScrollBars.Vertical
                    | this.dgvEmployeeDataGridView.ScrollBars == ScrollBars.Both)
                {
                    intWidth += 19;
                }

                for (int intCol = 0; intCol < this.dgvEmployeeDataGridView.ColumnCount; intCol++)
                {
                    if (this.dgvEmployeeDataGridView.Columns[intCol].Visible == true)
                    {
                        intWidth += this.dgvEmployeeDataGridView.Columns[intCol].Width;
                    }
                }

                if (intWidth != this.dgvEmployeeDataGridView.Width)
                {
                    System.Windows.Forms.MessageBox.Show("Employee Width should be " + intWidth.ToString());
                }

                intHeight = this.dgvEmployeeDataGridView.ColumnHeadersHeight + 2;
                intNewHeight = this.dgvEmployeeDataGridView.RowTemplate.Height / 2;

                for (int intRow = 0; intRow < 200; intRow++)
                {
                    intHeight += this.dgvEmployeeDataGridView.RowTemplate.Height;

                    if (intHeight == this.dgvEmployeeDataGridView.Height)
                    {
                        break;
                    }
                    else
                    {
                        if (intHeight > this.dgvEmployeeDataGridView.Height)
                        {
                            System.Windows.Forms.MessageBox.Show("Employee Height should be " + intHeight.ToString());
                            break;
                        }
                        else
                        {

                            if (intHeight + intNewHeight > this.dgvEmployeeDataGridView.Height)
                            {
                                System.Windows.Forms.MessageBox.Show("Employee Height should be " + intHeight.ToString());
                                break;
                            }
                        }
                    }
                }
#endif      

                this.lblCompany.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblEmployeeCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);

                miLinkedMenuItem = (ToolStripMenuItem)AppDomain.CurrentDomain.GetData("LinkedMenuItem");

                Clear_FingerPrint_Images();

                //2014-08-16
                this.txtEmployeePin.KeyPress += new KeyPressEventHandler(Numeric_KeyPress);

                mniMenuItem24 = (MenuItem)AppDomain.CurrentDomain.GetData("menuItem24");

                object[] objParm = new object[2];
                objParm[0] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                byte[] bytCompress = (byte[])clsISClientUtilities.DynamicFunction("Get_Form_Records", objParm,false);
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
                
                if (pvtDataSet.Tables["Company"].Rows.Count > 0)
                {
                    for (int intRow = 0; intRow < pvtDataSet.Tables["PayrollType"].Rows.Count; intRow++)
                    {
                        this.dgvPayrollTypeDataGridView.Rows.Add(pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE_DESC"].ToString());
                    }

                    pvtblnvPayrollTypeDataGridViewLoaded = true;

                    if (this.dgvPayrollTypeDataGridView.Rows.Count > 0)
                    {
                        Set_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView, 0);
                    }

                    Load_CurrentForm_Records();
                }
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
        }

        private void Numeric_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)8
                | (e.KeyChar > (char)47
                & e.KeyChar < (char)58))
            {
            }
            else
            {
                e.Handled = true;
                System.Console.Beep();
                return;
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
                            dgResult = CustomClientMessageBox.Show("Select Yes to Delete Fingerprint, No to Enroll Fingerprint", this.Text, MessageBoxButtons.YesNoCancel,MessageBoxIcon.Information);
                        }
                        else
                        {
                            dgResult = CustomClientMessageBox.Show("Select OK to Delete Fingerprint", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        }

                        if (dgResult == DialogResult.Yes
                        || dgResult == DialogResult.OK)
                            {
                            pvtEmployeeFingerTemplateDataView = null;
                            pvtEmployeeFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["EmployeeFingerTemplate"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo + " AND FINGER_NO = " + pvtintFingerNo, "", DataViewRowState.CurrentRows);

                            for (int intRow = 0; intRow < pvtEmployeeFingerTemplateDataView.Count; intRow++)
                            {
                                pvtEmployeeFingerTemplateDataView[0].Row.Delete();

                                intRow -= 1;
                            }

                            //Redraw Fingers
                            Draw_Current_Employee_Fingers();
                        }
                        else
                        {
                            if (dgResult == DialogResult.No)
                            {
                                //Only if pvtblnFingerprintDeviceOpened == true
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
           
            if (pvtstrFingerprintReaderName == "URU4500 (Digital Persona)")
            {
                DP_StartCapture();
            }
            else
            {
                int intReturnCode = clsReadersToDp.StartCapture();
            }
            
            this.picFinger1.Image = global::InteractPayrollClient.Properties.Resources.FingerPrintQuestion64;
            this.picFinger2.Image = global::InteractPayrollClient.Properties.Resources.FingerPrint64;
            this.picFinger3.Image = global::InteractPayrollClient.Properties.Resources.FingerPrint64;
            this.picFinger4.Image = global::InteractPayrollClient.Properties.Resources.FingerPrint64;

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

                        pvtTemplateDataRow["COMPANY_NO"] = this.pvtint64CompanyNo;
                        pvtTemplateDataRow["EMPLOYEE_NO"] = this.pvtint64EmployeeNo;
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

                                                this.picFinger2.Image = global::InteractPayrollClient.Properties.Resources.FingerPrintCorrect64;
                                                this.picFinger3.Image = global::InteractPayrollClient.Properties.Resources.FingerPrintQuestion64;

                                                break;

                                            case 3:

                                                this.picFinger3.Image = global::InteractPayrollClient.Properties.Resources.FingerPrintCorrect64;
                                                this.picFinger4.Image = global::InteractPayrollClient.Properties.Resources.FingerPrintQuestion64;

                                                break;

                                            case 4:

                                                this.picFinger4.Image = global::InteractPayrollClient.Properties.Resources.FingerPrintCorrect64;

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

                                                this.picFinger2.Image = global::InteractPayrollClient.Properties.Resources.FingerPrintError64;

                                                break;

                                            case 3:
                                                this.picFinger3.Image = global::InteractPayrollClient.Properties.Resources.FingerPrintError64;

                                                break;

                                            case 4:

                                                this.picFinger4.Image = global::InteractPayrollClient.Properties.Resources.FingerPrintError64;

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

                                        this.picFinger1.Image = global::InteractPayrollClient.Properties.Resources.FingerPrintCorrect64;
                                        this.picFinger2.Image = global::InteractPayrollClient.Properties.Resources.FingerPrintQuestion64;
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

                                        this.dgvEmployeeDataGridView[0, dgvEmployeeDataGridView.CurrentCell.RowIndex].Style = LocalTemplateDataGridViewCellStyle;
                                    }
                                    else
                                    {
                                        strMessage = "UNSUCCESSFUL.";
                                    }

                                    CustomClientMessageBox.Show("Enrollment of " + pvtstrFingerDescription + " finger " + strMessage, "Enrollment",MessageBoxButtons.OK,MessageBoxIcon.Information);

                                    this.pnlEnroll.Visible = false;
                                    this.btnClearFingers.Visible = true;
                                    this.pnlFingers.Visible = true;
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

        private void DP_StartCapture()
        {
            pvtCurrentReader.On_Captured -= new Reader.CaptureCallback(OnCaptured);
            pvtCurrentReader.On_Captured += new Reader.CaptureCallback(OnCaptured);
        }

        private void DP_StopCapture()
        {
            pvtCurrentReader.On_Captured -= new Reader.CaptureCallback(OnCaptured);
        }
  
        private void Load_CurrentForm_Records()
        {
            try
            {
                pvtblnCompanyDataGridViewLoaded = false;
              
                this.Clear_DataGridView(dgvCompanyDataGridView);
                this.Clear_DataGridView(dgvEmployeeDataGridView);
                this.Clear_DataGridView(dgvPayCategoryDataGridView);

                for (int intRowCount = 0; intRowCount < pvtDataSet.Tables["Company"].Rows.Count; intRowCount++)
                {
                    this.dgvCompanyDataGridView.Rows.Add("",
                                                         "",
                                                         "",
                                                         pvtDataSet.Tables["Company"].Rows[intRowCount]["COMPANY_DESC"].ToString(), 
                                                         intRowCount.ToString());
                }

                pvtblnCompanyDataGridViewLoaded = true;

                if (dgvCompanyDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvCompanyDataGridView,0);
                }

                Set_Form_For_Read();

                this.dgvCompanyDataGridView.Focus();

                //Force ReEntry - Bug???
                this.pvtintCompanyDataGridViewRowIndex = -1;
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
        }

        public int Get_DataGridView_SelectedRowIndex(DataGridView myDataGridView)
        {
            if (myDataGridView.CurrentCell == null)
            {
                string strStop = "";
            }

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
            if (myDataGridView.CurrentCell == null)
            {
                string strStop = "";
            }

            //Fires DataGridView RowEnter Function
            if (myDataGridView.CurrentCell.RowIndex == intRow)
            {
                DataGridViewCellEventArgs myDataGridViewCellEventArgs = new DataGridViewCellEventArgs(0, intRow);

                switch (myDataGridView.Name)
                {
                    case "dgvCompanyDataGridView":

                        pvtintCompanyDataGridViewRowIndex = -1;
                        this.dgvCompanyDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEmployeeDataGridView":

                        pvtintEmployeeDataGridViewRowIndex = -1;
                        this.dgvEmployeeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPayrollTypeDataGridView":

                        pvtintPayrollTypeDataGridViewRowIndex = -1;
                        this.dgvPayrollTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    default:

                        System.Windows.Forms.MessageBox.Show("No Entry for " + myDataGridView.Name + " in Set_DataGridView_SelectedRowIndex", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void frmEmployeeClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (pvtblnFingerprintDeviceOpened == true)
                {
                    if (pvtstrFingerprintReaderName == "URU4500 (Digital Persona)")
                    {
                        pvtCurrentReader.On_Captured -= new Reader.CaptureCallback(OnCaptured);

                        //NB Don't Worry If Close Failed
                        Constants.ResultCode ResultCode = pvtCurrentReader.CancelCapture();
                        
                        pvtCurrentReader.Dispose();
                        pvtCurrentReader = null;
                    }
                    else
                    {
                        clsReadersToDp.CloseDevice();
                    }
                }
            }
            catch
            {
            }

            miLinkedMenuItem.Enabled = true; 
        }

        public void btnUpdate_Click(object sender, System.EventArgs e)
        {
            this.Text += " - Update";

            this.Set_Form_For_Edit();
        }

        private void btnClearFingers_Click(object sender, EventArgs e)
        {
            pvtEmployeeFingerTemplateDataView = null;
            pvtEmployeeFingerTemplateDataView = new DataView(pvtDataSet.Tables["EmployeeFingerTemplate"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo, "", DataViewRowState.CurrentRows);

            if (pvtEmployeeFingerTemplateDataView.Count != 0)
            {
                for (int intRow = 0; intRow < pvtEmployeeFingerTemplateDataView.Count; intRow++)
                {
                    pvtEmployeeFingerTemplateDataView[intRow].Row.Delete();
                    intRow -= 1;
                }

                //Redraw Fingers
                Draw_Current_Employee_Fingers();
            }
        }

        private void Draw_Current_Employee_Fingers()
        {
            //Set Finger Colours
            pvtEmployeeFingerTemplateDataView = null;
            pvtEmployeeFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["EmployeeFingerTemplate"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo + " AND FINGER_NO = 0", "", DataViewRowState.CurrentRows);

            if (pvtEmployeeFingerTemplateDataView.Count == 0)
            {
                picLPinkie.Image = global::InteractPayrollClient.Properties.Resources.LPinkieClear;
                this.picLPinkie.Tag = "N";
            }
            else
            {
                if (pvtEmployeeFingerTemplateDataView[0]["FINGER_RESIDE_IND"].ToString() == "S")
                {
                    picLPinkie.Image = global::InteractPayrollClient.Properties.Resources.LPinkieServer;
                }
                else
                {
                    picLPinkie.Image = global::InteractPayrollClient.Properties.Resources.LPinkieLocal;
                }

                this.picLPinkie.Tag = "Y";
            }

            pvtEmployeeFingerTemplateDataView = null;
            pvtEmployeeFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["EmployeeFingerTemplate"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo + " AND FINGER_NO = 1", "", DataViewRowState.CurrentRows);

            if (pvtEmployeeFingerTemplateDataView.Count == 0)
            {
                picLRing.Image = global::InteractPayrollClient.Properties.Resources.LRingClear;
                this.picLRing.Tag = "N";
            }
            else
            {
                if (pvtEmployeeFingerTemplateDataView[0]["FINGER_RESIDE_IND"].ToString() == "S")
                {
                    picLRing.Image = global::InteractPayrollClient.Properties.Resources.LRingServer;
                }
                else
                {
                    picLRing.Image = global::InteractPayrollClient.Properties.Resources.LRingLocal;
                }

                this.picLRing.Tag = "Y";
            }

            pvtEmployeeFingerTemplateDataView = null;
            pvtEmployeeFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["EmployeeFingerTemplate"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo + " AND FINGER_NO = 2", "", DataViewRowState.CurrentRows);

            if (pvtEmployeeFingerTemplateDataView.Count == 0)
            {
                picLMiddle.Image = global::InteractPayrollClient.Properties.Resources.LMiddleClear;
                this.picLMiddle.Tag = "N";
            }
            else
            {
                if (pvtEmployeeFingerTemplateDataView[0]["FINGER_RESIDE_IND"].ToString() == "S")
                {
                    picLMiddle.Image = global::InteractPayrollClient.Properties.Resources.LMiddleServer;
                }
                else
                {
                    picLMiddle.Image = global::InteractPayrollClient.Properties.Resources.LMiddleLocal;
                }

                this.picLMiddle.Tag = "Y";
            }

            pvtEmployeeFingerTemplateDataView = null;
            pvtEmployeeFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["EmployeeFingerTemplate"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo + " AND FINGER_NO = 3", "", DataViewRowState.CurrentRows);

            if (pvtEmployeeFingerTemplateDataView.Count == 0)
            {
                picLIndex.Image = global::InteractPayrollClient.Properties.Resources.LIndexClear;
                this.picLIndex.Tag = "N";
            }
            else
            {
                if (pvtEmployeeFingerTemplateDataView[0]["FINGER_RESIDE_IND"].ToString() == "S")
                {
                    picLIndex.Image = global::InteractPayrollClient.Properties.Resources.LIndexServer;
                }
                else
                {
                    picLIndex.Image = global::InteractPayrollClient.Properties.Resources.LIndexLocal;
                }

                this.picLIndex.Tag = "Y";
            }

            pvtEmployeeFingerTemplateDataView = null;
            pvtEmployeeFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["EmployeeFingerTemplate"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo + " AND FINGER_NO = 4", "", DataViewRowState.CurrentRows);

            if (pvtEmployeeFingerTemplateDataView.Count == 0)
            {
                picLThumb.Image = global::InteractPayrollClient.Properties.Resources.LThumbClear;
                this.picLThumb.Tag = "N";
            }
            else
            {
                if (pvtEmployeeFingerTemplateDataView[0]["FINGER_RESIDE_IND"].ToString() == "S")
                {
                    picLThumb.Image = global::InteractPayrollClient.Properties.Resources.LThumbServer;
                }
                else
                {
                    picLThumb.Image = global::InteractPayrollClient.Properties.Resources.LThumbLocal;
                }

                this.picLThumb.Tag = "Y";
            }

            pvtEmployeeFingerTemplateDataView = null;
            pvtEmployeeFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["EmployeeFingerTemplate"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo + " AND FINGER_NO = 5", "", DataViewRowState.CurrentRows);

            if (pvtEmployeeFingerTemplateDataView.Count == 0)
            {
                picRThumb.Image = global::InteractPayrollClient.Properties.Resources.RThumbClear;
                this.picRThumb.Tag = "N";
            }
            else
            {
                if (pvtEmployeeFingerTemplateDataView[0]["FINGER_RESIDE_IND"].ToString() == "S")
                {
                    picRThumb.Image = global::InteractPayrollClient.Properties.Resources.RThumbServer;
                }
                else
                {
                    picRThumb.Image = global::InteractPayrollClient.Properties.Resources.RThumbLocal;
                }

                this.picRThumb.Tag = "Y";
            }

            pvtEmployeeFingerTemplateDataView = null;
            pvtEmployeeFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["EmployeeFingerTemplate"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo + " AND FINGER_NO = 6", "", DataViewRowState.CurrentRows);

            if (pvtEmployeeFingerTemplateDataView.Count == 0)
            {
                picRIndex.Image = global::InteractPayrollClient.Properties.Resources.RIndexClear;
                this.picRIndex.Tag = "N";
            }
            else
            {
                if (pvtEmployeeFingerTemplateDataView[0]["FINGER_RESIDE_IND"].ToString() == "S")
                {
                    picRIndex.Image = global::InteractPayrollClient.Properties.Resources.RIndexServer;
                }
                else
                {
                    picRIndex.Image = global::InteractPayrollClient.Properties.Resources.RIndexLocal;
                }
               
                this.picRIndex.Tag = "Y";
            }

            pvtEmployeeFingerTemplateDataView = null;
            pvtEmployeeFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["EmployeeFingerTemplate"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo + " AND FINGER_NO = 7", "", DataViewRowState.CurrentRows);

            if (pvtEmployeeFingerTemplateDataView.Count == 0)
            {
                picRMiddle.Image = global::InteractPayrollClient.Properties.Resources.RMiddleClear;
                this.picRMiddle.Tag = "N";
            }
            else
            {
                if (pvtEmployeeFingerTemplateDataView[0]["FINGER_RESIDE_IND"].ToString() == "S")
                {
                    picRMiddle.Image = global::InteractPayrollClient.Properties.Resources.RMiddleServer;
                }
                else
                {
                    picRMiddle.Image = global::InteractPayrollClient.Properties.Resources.RMiddleLocal;
                }

                this.picRMiddle.Tag = "Y";
            }

            pvtEmployeeFingerTemplateDataView = null;
            pvtEmployeeFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["EmployeeFingerTemplate"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo + " AND FINGER_NO = 8", "", DataViewRowState.CurrentRows);

            if (pvtEmployeeFingerTemplateDataView.Count == 0)
            {
                picRRing.Image = global::InteractPayrollClient.Properties.Resources.RRingClear;
                this.picRRing.Tag = "N";
            }
            else
            {
                if (pvtEmployeeFingerTemplateDataView[0]["FINGER_RESIDE_IND"].ToString() == "S")
                {
                    picRRing.Image = global::InteractPayrollClient.Properties.Resources.RRingServer;
                }
                else
                {
                    picRRing.Image = global::InteractPayrollClient.Properties.Resources.RRingLocal;
                }

                this.picRRing.Tag = "Y";
            }

            pvtEmployeeFingerTemplateDataView = null;
            pvtEmployeeFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["EmployeeFingerTemplate"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo + " AND FINGER_NO = 9", "", DataViewRowState.CurrentRows);

            if (pvtEmployeeFingerTemplateDataView.Count == 0)
            {
                picRPinkie.Image = global::InteractPayrollClient.Properties.Resources.RPinkieClear;
                this.picRPinkie.Tag = "N";
            }
            else
            {
                if (pvtEmployeeFingerTemplateDataView[0]["FINGER_RESIDE_IND"].ToString() == "S")
                {
                    picRPinkie.Image = global::InteractPayrollClient.Properties.Resources.RPinkieServer;
                }
                else
                {
                    picRPinkie.Image = global::InteractPayrollClient.Properties.Resources.RPinkieLocal;
                }

                this.picRPinkie.Tag = "Y";
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

            this.Set_Form_For_Read();

            if (dgvCompanyDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvCompanyDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvCompanyDataGridView));
            }
        }

        public void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (this.txtEmployeePin.Text.Trim() != "")
                {
                    if (this.txtEmployeePin.Text.Trim().Length < 4)
                    {
                        CustomClientMessageBox.Show("Employee Pin must be at Least 4 Characters.", "Employee Pin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.txtEmployeePin.Focus();
                        return;
                    }
                }

                int intReadOptionNo = 0;

                if (this.txtRFIDCardNo.Text != "")
                {
                    intReadOptionNo = 2;
                }

                pvtEmployeeFingerTemplateDataView = null;
                pvtEmployeeFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["EmployeeFingerTemplate"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo, "", DataViewRowState.CurrentRows);

                if (pvtEmployeeFingerTemplateDataView.Count > 0)
                {
                    intReadOptionNo += 1;
                }

                //0=None
                //1=FingerPrint
                //2=RFID Card Only
                //3=RFID Card And FingerPrint
                //4=Employee No. And Fingerprint
                //5=Employee No. And Pin
                pvtEmployeeFingerTemplateDataView = null;
                pvtEmployeeFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["EmployeeFingerTemplate"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo, "", DataViewRowState.Added | DataViewRowState.ModifiedOriginal | DataViewRowState.Deleted);

                if (intReadOptionNo == 0
                || intReadOptionNo == 2)
                { 
                    for (int intRow = 0; intRow < pvtEmployeeFingerTemplateDataView.Count; intRow++)
                    {
                        if (pvtEmployeeFingerTemplateDataView[0].Row.RowState != DataRowState.Deleted)
                        {
                            intReadOptionNo += 1;
                            break;
                        }
                    }
                }

                if (intReadOptionNo == 0)
                {
                    if (this.chkEmpNo.Checked == true
                    && this.txtEmployeePin.Text.Trim() != "")
                    {
                        intReadOptionNo = 5;
                    }
                }
                else
                {
                    //FingerPrint
                    if (intReadOptionNo == 1)
                    {
                        if (this.chkEmpNo.Checked == true)
                        {
                            intReadOptionNo = 4;
                        }
                    }
                }

                int intDataViewIndex = Convert.ToInt32(this.dgvEmployeeDataGridView[pvtintEmployeeKeyCol,this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Value);

                pvtEmployeeDataView[intDataViewIndex]["READ_OPTION_NO"] = Convert.ToInt32(pvtDataSet.Tables["ReadOption"].Rows[intReadOptionNo]["READ_OPTION_NO"]);

                this.dgvEmployeeDataGridView[6, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Value = pvtDataSet.Tables["ReadOption"].Rows[intReadOptionNo]["READ_OPTION_DESC"].ToString();

                string strUseEmployeeNo = "";

                if (this.chkEmpNo.Checked == true)
                {
                    strUseEmployeeNo = "Y";
                   
                    this.dgvEmployeeDataGridView[7, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Value = true;

                    pvtEmployeeDataView[intDataViewIndex]["USE_EMPLOYEE_NO_IND"] = "Y";
                }
                else
                {
                    this.dgvEmployeeDataGridView[7, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Value = false;
                    pvtEmployeeDataView[intDataViewIndex]["USE_EMPLOYEE_NO_IND"] = "";
                }

                pvtEmployeeDataView[intDataViewIndex]["EMPLOYEE_PIN"] = this.txtEmployeePin.Text.Trim();

                DataSet TempDataSet = new DataSet();
                //Add EmployeePayCategory Table 
                TempDataSet.Tables.Add(pvtDataSet.Tables["EmployeeFingerTemplate"].Clone());

                for (int intRow = 0; intRow < pvtEmployeeFingerTemplateDataView.Count; intRow++)
                {
                    TempDataSet.Tables["EmployeeFingerTemplate"].ImportRow(pvtEmployeeFingerTemplateDataView[intRow].Row);
                }

                //Compress DataSet
                byte[] pvtbytCompress = clsISClientUtilities.Compress_DataSet(TempDataSet);

                object[] objParm = new object[6];
                objParm[0] = pvtint64CompanyNo;
                objParm[1] = pvtint64EmployeeNo;
                objParm[2] = this.txtRFIDCardNo.Text.Trim();
                objParm[3] = strUseEmployeeNo;
                objParm[4] = this.txtEmployeePin.Text.Trim();
                objParm[5] = pvtbytCompress;

                clsISClientUtilities.DynamicFunction("Update_Employee_New", objParm,true);

                if (this.txtRFIDCardNo.Text.Trim() == "")
                {
                    pvtEmployeeDataView[intDataViewIndex]["EMPLOYEE_RFID_CARD_NO"] = System.DBNull.Value;

                    this.dgvEmployeeDataGridView[5, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Value = "";
                }

                this.pvtDataSet.AcceptChanges();

                this.Set_Form_For_Read();

                if (this.dgvCompanyDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvCompanyDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvCompanyDataGridView));
                }
            }
            catch (Exception ex)
            {
                clsISClientUtilities.ErrorHandler(ex);
            }
        }
        
        private void Set_Form_For_Edit()
        {
            this.btnUpdate.Enabled = false;
            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;
            
            this.picCompanyLock.Visible = true;
            this.picPayrollLock.Visible = true;
            this.picEmployeeLock.Visible = true;
            this.picPayCategoryLock.Visible = true;

            this.rbnAll.Enabled = false;
            this.rbnTemplateMissing.Enabled = false;
            this.rbnLocal.Enabled = false;
            this.rbnServer.Enabled = false;

            this.btnClearFingers.Visible = true;

            this.rbnAll.Checked = true;
            this.lblMessage.Visible = true;
            
            //if (this.txtRFIDCardNo.Text != "")
            //{
            //    this.btnDeleteRfidNo.Enabled = true;
            //}

            //this.chkEmpNo.Enabled = true;

            //if (this.chkEmpNo.Checked == true)
            //{
            //    this.txtEmployeePin.Enabled = true;
            //}

            this.dgvCompanyDataGridView.Enabled = false;
            this.dgvPayrollTypeDataGridView.Enabled = false;
            this.dgvEmployeeDataGridView.Enabled = false;
            this.dgvPayCategoryDataGridView.Enabled = false;
        }

        private void Set_Form_For_Read()
        {
            if (this.Text.IndexOf(" - Update") > 0)
            {
                this.Text = this.Text.Substring(0, this.Text.IndexOf(" - Update"));
            }

            this.picCompanyLock.Visible = false;
            this.picPayrollLock.Visible = false;
            this.picEmployeeLock.Visible = false;
            this.picPayCategoryLock.Visible = false;
           
            this.chkEmpNo.Enabled = false;

            this.rbnAll.Enabled = true;
            this.rbnTemplateMissing.Enabled = true;
            this.rbnLocal.Enabled = true;
            this.rbnServer.Enabled = true;

            this.pnlFingers.Visible = true;
            this.pnlEnroll.Visible = false;

            this.dgvCompanyDataGridView.Enabled = true;
            this.dgvPayrollTypeDataGridView.Enabled = true;
            this.dgvEmployeeDataGridView.Enabled = true;
            this.dgvPayCategoryDataGridView.Enabled = true;

            this.txtRFIDCardNo.Enabled = false;
            this.txtEmployeePin.Enabled = false;

            this.lblMessage.Visible = false;

            this.btnDeleteRfidNo.Enabled = false;
            this.btnClearFingers.Visible = false;

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                this.btnUpdate.Enabled = true;
            }
            else
            {
                this.btnUpdate.Enabled = false;
            }
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            if (this.pnlEnroll.Visible == true)
            {
                LinkLabelLinkClickedEventArgs lle = new LinkLabelLinkClickedEventArgs(null);

                lnkCancel_LinkClicked(sender, lle);
            }
            else
            {
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

                this.Close();
            }
        }

        private void Clear_FingerPrint_Images()
        {
            this.picLPinkie.Image = global::InteractPayrollClient.Properties.Resources.LPinkieClear;
            this.picLRing.Image = global::InteractPayrollClient.Properties.Resources.LRingClear;
            this.picLMiddle.Image = global::InteractPayrollClient.Properties.Resources.LMiddleClear;
            this.picLIndex.Image = global::InteractPayrollClient.Properties.Resources.LIndexClear;
            this.picLThumb.Image = global::InteractPayrollClient.Properties.Resources.LThumbClear;

            this.picRThumb.Image = global::InteractPayrollClient.Properties.Resources.RThumbClear;
            this.picRIndex.Image = global::InteractPayrollClient.Properties.Resources.RIndexClear;
            this.picRMiddle.Image = global::InteractPayrollClient.Properties.Resources.RMiddleClear;
            this.picRRing.Image = global::InteractPayrollClient.Properties.Resources.RRingClear;
            this.picRPinkie.Image = global::InteractPayrollClient.Properties.Resources.RPinkieClear;

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
        
        private void lnkCancel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.pnlEnroll.Visible = false;
            this.btnClearFingers.Visible = true;
            this.pnlFingers.Visible = true;
            this.btnSave.Enabled = true;
        }

        private void pnlFingers_VisibleChanged(object sender, EventArgs e)
        {
            if (pnlFingers.Visible == true)
            {
                if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                {
                    Draw_Current_Employee_Fingers();

                    if (this.btnUpdate.Enabled == false)
                    {
                        this.btnSave.Enabled = true;
                    }
                }

                this.pnlEnroll.Visible = false;
            }
        }

        private void chkEmpNo_Click(object sender, EventArgs e)
        {
            this.dgvEmployeeDataGridView[8, this.Get_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView)].Value = chkEmpNo.Checked;

            if (chkEmpNo.Checked == true)
            {
                txtEmployeePin.Enabled = true;
            }
            else
            {
                txtEmployeePin.Enabled = false;
                txtEmployeePin.Text = "";
            }
        }

        private void dgvPayrollTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnvPayrollTypeDataGridViewLoaded == true)
            {
                if (pvtintPayrollTypeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayrollTypeDataGridViewRowIndex = e.RowIndex;

                    if (this.rbnAll.Checked == false)
                    {
                        this.rbnAll.Checked = true;

                        bool blnCompanyDataGridViewLoaded = false;

                        if (pvtblnCompanyDataGridViewLoaded == true)
                        {
                            pvtblnCompanyDataGridViewLoaded = false;
                            blnCompanyDataGridViewLoaded = true;
                        }

                        EventArgs ev = new EventArgs();
                        rbnAll_Click(sender, ev);

                        if (blnCompanyDataGridViewLoaded == true)
                        {
                            pvtblnCompanyDataGridViewLoaded = true;
                        }
                    }
                    
                    this.pvtstrPayrollType = dgvPayrollTypeDataGridView[0, e.RowIndex].Value.ToString().Substring(0, 1);

                    if (this.dgvCompanyDataGridView.SortedColumn != null)
                    {
                        dgvCompanyDataGridView.SortedColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                    }

                    if (dgvCompanyDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(dgvCompanyDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvCompanyDataGridView));
                        //Force ReEntry - Bug???
                        this.pvtintCompanyDataGridViewRowIndex = -1;
                    }
                }
            }
        }

        private void dgvCompanyDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (this.pvtblnCompanyDataGridViewLoaded == true)
                {
                    if (pvtintCompanyDataGridViewRowIndex != e.RowIndex)
                    {
                        pvtintCompanyDataGridViewRowIndex = e.RowIndex;

                        for (int intRow = 0; intRow < this.dgvCompanyDataGridView.Rows.Count; intRow++)
                        {
                            this.dgvCompanyDataGridView[0, intRow].Style = NormalTemplateDataGridViewCellStyle;
                            this.dgvCompanyDataGridView[1, intRow].Style = NormalTemplateDataGridViewCellStyle;
                            this.dgvCompanyDataGridView[2, intRow].Style = NormalTemplateDataGridViewCellStyle;
                        }
                        
                        this.Cursor = Cursors.AppStarting;

                        int intEmployeeIndex = 0;
                        int intCompanyIndex = Convert.ToInt32(this.dgvCompanyDataGridView[this.pvtintCompanyRecCountCol, e.RowIndex].Value);

                        string strReadOption = "";
                        string strEmployeeLastRundate = "";
                        bool blnUseEmployeeNoInd = false;

                        this.btnUpdate.Enabled = false;

                        this.txtRFIDCardNo.Text = "";
                     
                        pvtblnEmployeeDataGridViewLoaded = false;
                        
                        this.Clear_DataGridView(dgvEmployeeDataGridView);
                        this.Clear_DataGridView(this.dgvPayCategoryDataGridView);

                        this.btnUpdate.Enabled = false;

                        pvtint64CompanyNo = Convert.ToInt64(pvtDataSet.Tables["Company"].Rows[intCompanyIndex]["COMPANY_NO"]);

                        DataView DataView = new System.Data.DataView(pvtDataSet.Tables["PayCategory"], "COMPANY_NO = " + pvtint64CompanyNo, "", DataViewRowState.CurrentRows);

                        if (DataView.Count == 0)
                        {
                            object[] objParm = new object[2];
                            objParm[0] = pvtint64CompanyNo;
                            objParm[1] = pvtstrSoftwareToUse;

                            byte[] bytCompress = (byte[])clsISClientUtilities.DynamicFunction("Get_Company_Records_New", objParm, false);

                            DataSet pvtTempDataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(bytCompress);

                            pvtDataSet.Merge(pvtTempDataSet);
                        }

                        pvtEmployeeDataView = null;
                        pvtEmployeeDataView = new System.Data.DataView(pvtDataSet.Tables["Employee"], "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'", "", DataViewRowState.CurrentRows);

                        for (int intRowCount = 0; intRowCount < pvtEmployeeDataView.Count; intRowCount++)
                        {
                            //Set Finger Colours
                            pvtEmployeeFingerTemplateDataView = null;
                            pvtEmployeeFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["EmployeeFingerTemplate"], "COMPANY_NO = " + pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtEmployeeDataView[intRowCount]["EMPLOYEE_NO"].ToString(), "", DataViewRowState.CurrentRows);

                            if (this.rbnTemplateMissing.Checked == true)
                            {
                                if (pvtEmployeeFingerTemplateDataView.Count > 0)
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

                                    DataView EmployeeFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["EmployeeFingerTemplate"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtEmployeeDataView[intRowCount]["EMPLOYEE_NO"].ToString() + " AND FINGER_RESIDE_IND = '" + strOption + "'","", DataViewRowState.CurrentRows);

                                    if (EmployeeFingerTemplateDataView.Count == 0)
                                    {
                                        continue;
                                    }
                                }
                            }

                            blnUseEmployeeNoInd = false;
                            strReadOption = "";
                            strEmployeeLastRundate = "";
                        
                            if (pvtEmployeeDataView[intRowCount]["READ_OPTION_NO"] != System.DBNull.Value)
                            {
                                for (int intRow = 0; intRow < pvtDataSet.Tables["ReadOption"].Rows.Count; intRow++)
                                {
                                    if (Convert.ToInt32(pvtEmployeeDataView[intRowCount]["READ_OPTION_NO"]) == Convert.ToInt32(pvtDataSet.Tables["ReadOption"].Rows[intRow]["READ_OPTION_NO"]))
                                    {
                                        strReadOption = pvtDataSet.Tables["ReadOption"].Rows[intRow]["READ_OPTION_DESC"].ToString();
                                        break;
                                    }
                                }
                            }

                            if (pvtEmployeeDataView[intRowCount]["USE_EMPLOYEE_NO_IND"].ToString() == "Y")
                            {
                                blnUseEmployeeNoInd = true;
                            }

                            if (pvtEmployeeDataView[intRowCount]["EMPLOYEE_LAST_RUNDATE"] != System.DBNull.Value)
                            {
                                strEmployeeLastRundate = Convert.ToDateTime(pvtEmployeeDataView[intRowCount]["EMPLOYEE_LAST_RUNDATE"]).ToString("yyyy-MM-dd");
                            }

                            this.dgvEmployeeDataGridView.Rows.Add("",
                                                                  "",
                                                                  "",
                                                                  pvtEmployeeDataView[intRowCount]["EMPLOYEE_CODE"].ToString(),
                                                                  pvtEmployeeDataView[intRowCount]["EMPLOYEE_NO"].ToString(),
                                                                  pvtEmployeeDataView[intRowCount]["EMPLOYEE_SURNAME"].ToString(),
                                                                  pvtEmployeeDataView[intRowCount]["EMPLOYEE_NAME"].ToString(),
                                                                  pvtEmployeeDataView[intRowCount]["EMPLOYEE_3RD_PARTY_CODE"].ToString(),
                                                                  pvtEmployeeDataView[intRowCount]["DEPARTMENT_DESC"].ToString(),
                                                                  pvtEmployeeDataView[intRowCount]["EMPLOYEE_RFID_CARD_NO"].ToString(),
                                                                  strReadOption,
                                                                  blnUseEmployeeNoInd,
                                                                  strEmployeeLastRundate,
                                                                  intRowCount.ToString());

                            if (pvtEmployeeFingerTemplateDataView.Count == 0)
                            {
                                this.dgvEmployeeDataGridView[0, dgvEmployeeDataGridView.Rows.Count - 1].Style = NoTemplateDataGridViewCellStyle;
                                this.dgvCompanyDataGridView[0, pvtintCompanyDataGridViewRowIndex].Style = NoTemplateDataGridViewCellStyle;
                            }
                            else
                            {
                                pvtEmployeeFingerTemplateDataView = null;
                                pvtEmployeeFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["EmployeeFingerTemplate"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtEmployeeDataView[intRowCount]["EMPLOYEE_NO"].ToString(), "FINGER_RESIDE_IND", DataViewRowState.CurrentRows);

                                if (pvtEmployeeFingerTemplateDataView.Count > 0)
                                {
                                    for (int intRow = 0; intRow < pvtEmployeeFingerTemplateDataView.Count; intRow++)
                                    {
                                        if (pvtEmployeeFingerTemplateDataView[intRow]["FINGER_RESIDE_IND"].ToString() == "S")
                                        {
                                            this.dgvEmployeeDataGridView[1,dgvEmployeeDataGridView.Rows.Count - 1].Style = ServerTemplateDataGridViewCellStyle;

                                            this.dgvCompanyDataGridView[1,pvtintCompanyDataGridViewRowIndex].Style = ServerTemplateDataGridViewCellStyle;
                                        }
                                        else
                                        {
                                            this.dgvEmployeeDataGridView[2, dgvEmployeeDataGridView.Rows.Count - 1].Style = LocalTemplateDataGridViewCellStyle;

                                            this.dgvCompanyDataGridView[2, pvtintCompanyDataGridViewRowIndex].Style = LocalTemplateDataGridViewCellStyle;
                                        }
                                    }
                                }
                            }

                            if (Convert.ToInt32(pvtEmployeeDataView[intRowCount]["EMPLOYEE_NO"]) == pvtint64EmployeeNo)
                            {
                                intEmployeeIndex = dgvEmployeeDataGridView.Rows.Count - 1;
                            }
                        }

                        pvtblnEmployeeDataGridViewLoaded = true;

                        if (dgvEmployeeDataGridView.Rows.Count > 0)
                        {
                            this.btnUpdate.Enabled = true;
                         
                            this.Set_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView, intEmployeeIndex);
                        }
                        else
                        {
                            Clear_FingerPrint_Images();
                            this.btnUpdate.Enabled = false;
                        }

                        this.Cursor = Cursors.Default;
                    }
                    else
                    {
                        DateTime myDateTime = DateTime.Now.AddMilliseconds(200);

                        while (myDateTime > DateTime.Now)
                        {
                            Application.DoEvents();
                        }
                    }
                }
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
        }

        private void DataGrid_Sorted(object sender, EventArgs e)
        {
            DataGridView myDataGridView = (DataGridView)sender;

            if (myDataGridView.Rows.Count > 0)
            {
                if (myDataGridView.SelectedRows.Count > 0)
                {
                    if (myDataGridView.SelectedRows[0].Selected == true)
                    {
                        myDataGridView.FirstDisplayedScrollingRowIndex = myDataGridView.SelectedRows[0].Index; ;
                    }
                }
            }
        }

        private void dgvEmployeeDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 3)
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

        private void dgvEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnEmployeeDataGridViewLoaded == true)
            {
                if (pvtintEmployeeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintEmployeeDataGridViewRowIndex = e.RowIndex;

                    this.Cursor = Cursors.AppStarting;

                    this.Clear_DataGridView(dgvPayCategoryDataGridView);

                    pvtintCurrentEmployeeRow = Convert.ToInt32(this.dgvEmployeeDataGridView[pvtintEmployeeKeyCol, e.RowIndex].Value);

                    this.txtRFIDCardNo.Text = pvtEmployeeDataView[pvtintCurrentEmployeeRow]["EMPLOYEE_RFID_CARD_NO"].ToString();
                  
                    if (pvtEmployeeDataView[pvtintCurrentEmployeeRow]["USE_EMPLOYEE_NO_IND"].ToString() == "Y")
                    {
                        this.chkEmpNo.Checked = true;
                    }
                    else
                    {
                        this.chkEmpNo.Checked = false;
                    }

                    this.txtEmployeePin.Text = pvtEmployeeDataView[pvtintCurrentEmployeeRow]["EMPLOYEE_PIN"].ToString();

                    pvtint64EmployeeNo = Convert.ToInt64(pvtEmployeeDataView[pvtintCurrentEmployeeRow]["EMPLOYEE_NO"]);

                    pvtEmployeePayCategoryDataView = null;
                    pvtEmployeePayCategoryDataView = new DataView(pvtDataSet.Tables["EmployeePayCategory"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'", "", DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < pvtEmployeePayCategoryDataView.Count; intRow++)
                    {
                        pvtPayCategoryDataView = null;
                        pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + pvtEmployeePayCategoryDataView[intRow]["PAY_CATEGORY_NO"].ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'", "", DataViewRowState.CurrentRows);

                        if (pvtPayCategoryDataView.Count != 0)
                        {
                            this.dgvPayCategoryDataGridView.Rows.Add(pvtPayCategoryDataView[0]["PAY_CATEGORY_DESC"].ToString());
                        }
                    }

                    Draw_Current_Employee_Fingers();

                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void rbnFilter_Click(object sender, EventArgs e)
        {
            if (this.dgvCompanyDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvCompanyDataGridView,this.Get_DataGridView_SelectedRowIndex(dgvCompanyDataGridView));
            }
       }

        private void rbnAll_Click(object sender, EventArgs e)
        {
            if (this.dgvCompanyDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvCompanyDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvCompanyDataGridView));
            }
        }

        private void frmEmployeeClient_KeyDown(object sender, KeyEventArgs e)
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

        private void btnSend_Click(object sender, EventArgs e)
        {

            //AppDomain.CurrentDomain.SetData("SoftwareToUse","D");
            
            //FingerPrintClockServer.FingerPrintClockService fingerPrintClockService = new FingerPrintClockServer.FingerPrintClockService();
            
            //fingerPrintClockService.GetEmployeePinClocked("0", "1", "I", "N","5", "0904");
            



            //SpecificEmployee SpecificEmployee = FingerPrintClockService.GetSpecificEmployee("1", "W", "3");
#if (DEBUG)
           // CurrentUser CurrentUser = FingerPrintClockService.GetCurrentUser(this.txtEmployeeNo.Text,this.txtPin.Text);
#endif
        }

        private void txtEmployeePin_Enter(object sender, EventArgs e)
        {
            this.KeyPreview = false;
        }

        private void txtEmployeePin_Leave(object sender, EventArgs e)
        {
            this.KeyPreview = true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.Timer.Enabled = false;

            CustomClientMessageBox.Show(pvtstrFingerprintDeviceOpenedFailureMessage, "Fingerprint Open Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
