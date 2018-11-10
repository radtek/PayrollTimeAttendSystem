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
using InteractPayrollClient;
using DPUruNet;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace InteractPayroll
{
    public partial class frmEmployee : Form
    {
        clsISUtilities clsISUtilities;
        clsReadersToDp clsReadersToDp;

        ColorPalette greyScalePalette;

        private ReaderCollection ReaderCollection;
        private Reader pvtCurrentReader;
        
        Dictionary<string, int> dictionaryMonths = new Dictionary<string, int>();

        //DataView Index Rows
        private int pvtintEmployeeEarningDataViewIndex = -1;
        private int pvtintEmployeeDeductionDataViewIndex = -1;

        //DataGridView Index Rows
        private int pvtLeaveTypeDataGridViewRowIndex = -1;
        private int pvtintLoanDataGridViewRowIndex = -1;
        private int pvtintChosenPayCategoryDataGridViewRowIndex = -1;
        private int pvtintEmployeeDataGridViewRowIndex = -1;
        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
        private int pvtintChosenEarningDataGridViewRowIndex = -1;
        private int pvtintChosenDeductionDataGridViewRowIndex = -1;

        object sender;
        System.EventArgs e;

        private int pvtintEmployeeNo = -1;
        private int pvtintLeaveEarningNo = -1;
        private int pvtintDeductionAccountNo = -1;
        private int pvtintDeductionSubAccountNo = -1;

        private int pvtintDeductionNo = 0;
        private int pvtintDeductionSubNo = 0;

        private string pvtstrFilter = "";

        private int pvtintReturnCode = -1;

        //PayCategory SpreadSheets Column Offset
        private int pvtintPayCategoryDescCol = 0;
        private int pvtintPayCategoryRateCol = 1;
        private int pvtintPayCategoryDefaultIndCol = 2;
        private int pvtintPayCategoryNoCol = 3;

        //Earnings SreadSheets Column Offset
        private int pvtintEarningsDescCol = 2;
        private int pvtintEarningsPeriodAmtCol = 3;
        private int pvtintEarningsEarningNoCol = 4;

        //Deduction SreadSheets Column Offset
        private int pvtintDeductionsDescCol = 2;
        private int pvtintDeductionsValueCol = 3;
        private int pvtintDeductionsDeductionNoCol = 4;
        private int pvtintDeductionsDeductionSubNoCol = 5;

        //Leave Type SreadSheets Column Offset
        private int pvtintLeaveTypeDescCol = 1;
        private int pvtintLeaveTypeAccumTotalCol = 2;
        private int pvtintLeaveTypeEarningNoCol = 3;

        //Employee Leave SreadSheets Column Offset
        private int pvtintLeaveDescCol = 2;
        private int pvtintLeaveFromDateCol = 3;
        private int pvtintLeaveToDateCol = 4;
        private int pvtintLeaveDaysCol = 5;
        private int pvtintLeaveHoursCol = 6;
        private int pvtintLeaveOptionCol = 7;
        private int pvtintLeaveProcessCol = 8;
        private int pvtintLeaveAccumValueCol = 9;
        private int pvtintLeavePaidValueCol = 10;
        private int pvtintLeaveProcessDateCol = 11;
        private int pvtintLeaveRecNoCol = 12;

        //Loan Description SreadSheets Column Offset
        private int pvtintLoanDescriptionDescCol = 1;
        private int pvtintLoanDescriptionOutstandingAmtCol = 2;
        private int pvtintLoanDescriptionDeductionNoCol = 3;
        private int pvtintLoanDescriptionDeductionSubNoCol = 4;

        //Loan SreadSheets Column Offset
        private int pvtintLoanDescCol = 1;
        private int pvtintLoanProcessOptionCol = 2;
        private int pvtintLoanPaidCol = 3;
        private int pvtintLoanReceivedCol = 4;
        private int pvtintLoanProcessDateCol = 5;
        private int pvtintLoanRecNoCol = 6;

        private byte[] pvtbytFinger1;
        private byte[] pvtbytFinger2;
        private byte[] pvtbytFinger3;

        private byte[] pvtbyteArrayPreviousTemplate;
        private PictureBox pvtpicFinger;
        private int pvtintFingerNo;
        private int pvtintCurrentFingerCount;

        private byte[] pvtbytCompress;
        private DateTime dtDateTime = DateTime.Now;

        private DataTable pvtDataTable;
        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;

        private DataView pvtPayCategoryDataView;

        private DataView pvtEmployeeDataView;

        private DataView pvtEarningDataView;
        private DataView pvtEmployeeEarningDataView;

        private DataView pvtEmployeeDeductionDataView;
        private DataView pvtDeductionDataView;
        private DataView pvtDeductionEarningPercentageDataView;

        private DataView pvtEmployeeLeaveDataView;

        private DataView pvtEmployeeLoanCFDataView;
        private DataView pvtEmployeeLoanDataView;

        private DataView pvtLeaveLinkDataView;
        private DataView pvtLeaveTypeDataView;

        private DataView pvtProcessDataView;
        private DataView pvtPublicHolidayDataView;

        private DataRowView pvtDataRowView;
        private DataRow pvtTemplateDataRow;

        private object[] pvtobjDeductionFind = new object[2];

        private DataRowView pvtdrvDataRowView;
        private DataView pvtEmployeeFingerTemplateDataView;

        private object pvtobjSender;

        private string pvtstrPayrollType = "W";

        DataGridViewCellStyle LockedPayrollRunDataGridViewCellStyle;
        DataGridViewCellStyle ReadOnlyDataGridViewCellStyle;
        DataGridViewCellStyle NotActiveDataGridViewCellStyle;
        DataGridViewCellStyle NormalDataGridViewCellStyle;
        DataGridViewCellStyle ErrorDataGridViewCellStyle;
        DataGridViewCellStyle LeaveDaysExcludedDataGridViewCellStyle;
        DataGridViewCellStyle LeavePayoutDueDataGridViewCellStyle;
        DataGridViewCellStyle LeaveZerorizeDataGridViewCellStyle;
        DataGridViewCellStyle LoanTypeDataGridViewCellStyle;
        DataGridViewCellStyle HoursOptionDataGridViewCellStyle;
        DataGridViewCellStyle NoTemplateDataGridViewCellStyle;
        DataGridViewCellStyle HasTemplateDataGridViewCellStyle;

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnEmployeeDataGridViewLoaded = false;

        private bool pvtblnPayCategoryDataGridViewLoaded = false;
        private bool pvtblnChosenPayCategoryDataGridViewLoaded = false;
        private bool pvtblnEarningDataGridViewLoaded = false;
        private bool pvtblnChosenEarningDataGridViewLoaded = false;

        private bool pvtblnDeductionDataGridViewLoaded = false;
        private bool pvtblnChosenDeductionDataGridViewLoaded = false;
        private bool pvtblnChosenDeductionEarningLinkDataGridViewLoaded = false;

        private bool pvtblnLeaveTypeDataGridViewLoaded = false;
        private bool pvtblnEmployeeLeaveDataGridViewLoaded = false;

        private bool pvtblnLoanDataGridViewLoaded = false;
        private bool pvtblnEmployeeLoanDataGridViewLoaded = false;

        private bool pvtblnEmployeeDataGridViewStopFiring = false;

        string pvtstrFileName = "FingerprintReaderChoice.txt";

        string pvtstrFingerprintReaderName = "None";

        private string pvtstrInitialMessage = "To begin, place and hold your #FINGER# finger on the Fingerprint Reader until the screen indicates that the scan was successful. Repeat for each of the remaining scans.";
        private string pvtstrSuccessful = "The Scan was successful.\nPlace your #FINGER# finger on the Fingerprint Reader again.";
        private string pvtstrScanDifferent = "The finger scanned is NOT the same as the previous one or the Image is of BAD Quality. Try again. Place your #FINGER# finger flat on the fingerprint reader.";

        private string pvtstrFingerDescription = "";
        private string pvtstrFingerprintDeviceOpenedFailureMessage = "";

        private bool pvtblnFingerprintDeviceOpened = false;

        public frmEmployee()
        {
            InitializeComponent();
            
            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 114;

                this.dgvEmployeeDataGridView.Height += 114;

                this.tabEmployee.Top += 114;
            }
            
            this.grbLeaveError.Left = this.grbEmployeeLock.Left;
            this.grbLeaveError.Top = this.grbEmployeeLock.Top;
        }

        private void frmEmployee_Load(object sender, System.EventArgs e)
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

                LockedPayrollRunDataGridViewCellStyle = new DataGridViewCellStyle();
                LockedPayrollRunDataGridViewCellStyle.BackColor = Color.Magenta;
                LockedPayrollRunDataGridViewCellStyle.SelectionBackColor = Color.Magenta;

                ReadOnlyDataGridViewCellStyle = new DataGridViewCellStyle();
                ReadOnlyDataGridViewCellStyle.BackColor = Color.Aqua;
                ReadOnlyDataGridViewCellStyle.SelectionBackColor = Color.Aqua;

                NotActiveDataGridViewCellStyle = new DataGridViewCellStyle();
                NotActiveDataGridViewCellStyle.BackColor = Color.Orange;
                NotActiveDataGridViewCellStyle.SelectionBackColor = Color.Orange;

                NormalDataGridViewCellStyle = new DataGridViewCellStyle();
                NormalDataGridViewCellStyle.BackColor = SystemColors.Control;
                NormalDataGridViewCellStyle.SelectionBackColor = SystemColors.Control;

                ErrorDataGridViewCellStyle = new DataGridViewCellStyle();
                ErrorDataGridViewCellStyle.BackColor = Color.Red;
                ErrorDataGridViewCellStyle.SelectionBackColor = Color.Red;

                LeaveDaysExcludedDataGridViewCellStyle = new DataGridViewCellStyle();
                LeaveDaysExcludedDataGridViewCellStyle.BackColor = Color.Yellow;
                LeaveDaysExcludedDataGridViewCellStyle.SelectionBackColor = Color.Yellow;

                LeavePayoutDueDataGridViewCellStyle = new DataGridViewCellStyle();
                LeavePayoutDueDataGridViewCellStyle.BackColor = Color.Plum;
                LeavePayoutDueDataGridViewCellStyle.SelectionBackColor = Color.Plum;

                LeaveZerorizeDataGridViewCellStyle = new DataGridViewCellStyle();
                LeaveZerorizeDataGridViewCellStyle.BackColor = Color.CornflowerBlue;
                LeaveZerorizeDataGridViewCellStyle.SelectionBackColor = Color.CornflowerBlue;
                
                LoanTypeDataGridViewCellStyle = new DataGridViewCellStyle();
                LoanTypeDataGridViewCellStyle.BackColor = Color.Teal;
                LoanTypeDataGridViewCellStyle.SelectionBackColor = Color.Teal;

                HoursOptionDataGridViewCellStyle = new DataGridViewCellStyle();
                HoursOptionDataGridViewCellStyle.BackColor = Color.Lime;
                HoursOptionDataGridViewCellStyle.SelectionBackColor = Color.Lime;

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

                this.lblEarnings.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSelectedEarningsHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblDeductions.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSelectedDeductions.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblPercentEarnings.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblLeave.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblLeaveTypeDescription.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblLoan.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblLoanDescription.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.rbnTaxPartTime.Text = "Part Time (" + AppDomain.CurrentDomain.GetData("TaxCasual") + "%)";

                pvtDataSet = new DataSet();
                //Countries DataTable
                pvtDataTable = clsISUtilities.Get_Countries();
                pvtDataSet.Tables.Add(pvtDataTable);

                pvtDataTable = new System.Data.DataTable("NumberCheques");

                pvtDataTable.Columns.Add("NO_CHEQUES", typeof(System.Int16));

                DataRow DataRow = pvtDataTable.NewRow();
                DataRow["NO_CHEQUES"] = 12;
                pvtDataTable.Rows.Add(DataRow);

                DataRow = pvtDataTable.NewRow();
                DataRow["NO_CHEQUES"] = 13;
                pvtDataTable.Rows.Add(DataRow);

                DataRow = pvtDataTable.NewRow();
                DataRow["NO_CHEQUES"] = 14;
                pvtDataTable.Rows.Add(DataRow);

                pvtDataSet.Tables.Add(pvtDataTable);

                pvtDataSet.AcceptChanges();

                object[] objParm = new object[3];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[2] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                DataSet pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                pvtDataSet.Merge(pvtTempDataSet);

                if (pvtDataSet.Tables["Department"].Rows.Count == 0)
                {

                }

                if (pvtDataSet.Tables["Occupation"].Rows.Count == 0)
                {

                }
              
                pvtProcessDataView = null;
                pvtProcessDataView = new DataView(pvtDataSet.Tables["Process"],
                "",
                "PROCESS_NO",
                DataViewRowState.CurrentRows);

                pvtPublicHolidayDataView = null;
                pvtPublicHolidayDataView = new DataView(pvtDataSet.Tables["PublicHoliday"],
                "",
                "PUBLIC_HOLIDAY_DATE",
                DataViewRowState.CurrentRows);

                txtDeductMinValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
                txtDeductMaxValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);

                //Load ComboBoxes
                clsISUtilities.DataBind_ComboBox_Load(cboNatureOfPerson, this.pvtDataSet.Tables["NaturePerson"], "NATURE_PERSON_DESC", "NATURE_PERSON_NO");
                clsISUtilities.DataBind_ComboBox_Load(cboCountry, this.pvtDataSet.Tables["Country"], "COUNTRY_DESC", "COUNTRY_CODE");
                clsISUtilities.DataBind_ComboBox_Load(cboMaritalStatus, this.pvtDataSet.Tables["MaritalStatus"], "MARITAL_STATUS_DESC", "MARITAL_STATUS_NO");
                clsISUtilities.DataBind_ComboBox_Load(cboOccupation, this.pvtDataSet.Tables["Occupation"], "OCCUPATION_DESC", "OCCUPATION_NO");
                clsISUtilities.DataBind_ComboBox_Load(cboDepartment, this.pvtDataSet.Tables["Department"], "DEPARTMENT_DESC", "DEPARTMENT_NO");
                clsISUtilities.DataBind_ComboBox_Load(cboGender, this.pvtDataSet.Tables["Gender"], "GENDER_DESC", "GENDER_IND");
                clsISUtilities.DataBind_ComboBox_Load(cboRace, this.pvtDataSet.Tables["Race"], "RACE_DESC", "RACE_NO");
                                
                //Errol 2015-02-14
                clsISUtilities.DataBind_ComboBox_Load(this.cboAddressCountry, this.pvtDataSet.Tables["Country"], "COUNTRY_DESC", "COUNTRY_CODE2");
                clsISUtilities.DataBind_ComboBox_Load(this.cboPostCountry, this.pvtDataSet.Tables["Country"], "COUNTRY_DESC", "COUNTRY_CODE2");

                clsISUtilities.DataBind_ComboBox_Load(cboBankAccountType, this.pvtDataSet.Tables["BankAccountType"], "BANK_ACCOUNT_TYPE_DESC", "BANK_ACCOUNT_TYPE_NO");
                clsISUtilities.DataBind_ComboBox_Load(cboBankAccountRelationship, this.pvtDataSet.Tables["BankRelationshipType"], "BANK_ACCOUNT_RELATIONSHIP_TYPE_DESC", "BANK_ACCOUNT_RELATIONSHIP_TYPE_NO");

                clsISUtilities.DataBind_ComboBox_Load(this.cboEarningNoCheques, this.pvtDataSet.Tables["NumberCheques"], "NO_CHEQUES", "NO_CHEQUES");

                clsISUtilities.NotDataBound_ComboBox(cboLeaveShift, "Select Normal Leave / Sick Leave Category.");

                clsISUtilities.NotDataBound_Numeric_TextBox(txtRate, "Enter Rate.", 2, 0);

                string strComboList = "";

                for (int intCount = 0; intCount < this.pvtDataSet.Tables["Process"].Rows.Count; intCount++)
                {
                    if (strComboList == "")
                    {
                        strComboList = this.pvtDataSet.Tables["Process"].Rows[intCount]["PROCESS_DESC"].ToString();
                    }
                    else
                    {
                        strComboList += "|" + this.pvtDataSet.Tables["Process"].Rows[intCount]["PROCESS_DESC"].ToString();
                    }
                }

                DateTime dtEtiDateTimeStarted = new DateTime(2014,1,1);
                DateTime dtEtiDateTime = DateTime.Now.AddMonths(2);
                              
                this.cboEtiStartMonth.Items.Add("");

                while (true)
                {
                    this.cboEtiStartMonth.Items.Add(dtEtiDateTime.ToString("MMMM yyyy"));
                    
                    if (dictionaryMonths.ContainsKey(dtEtiDateTime.ToString("MMMM")))
                    {
                    }
                    else
                    {
                        dictionaryMonths.Add(dtEtiDateTime.ToString("MMMM"), Convert.ToInt32(dtEtiDateTime.ToString("MM")));
                    }

                    if (dtEtiDateTimeStarted > dtEtiDateTime)
                    {
                        break;
                    }

                    dtEtiDateTime = dtEtiDateTime.AddMonths(-1);
                }

                this.dgvPayrollTypeDataGridView.Rows.Add("Wages");
                this.dgvPayrollTypeDataGridView.Rows.Add("Salaries");

                this.pvtblnPayrollTypeDataGridViewLoaded = true;

                this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView, 0);

                //TextBox used To Hold new EMPLOYEE_CODE (Temporarily)
                this.txtNewCode.BringToFront();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
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

                                        CustomMessageBox.Show("Enrollment of " + pvtstrFingerDescription + " finger " + strMessage, "Enrollment", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                    else
                                    {
                                        strMessage = "UNSUCCESSFUL.";

                                        CustomMessageBox.Show("Enrollment of " + pvtstrFingerDescription + " finger " + strMessage, "Enrollment", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    
                                    if (pvtstrFingerprintReaderName == "URU4500 (Digital Persona)")
                                    {
                                        DP_StopCapture();
                                    }

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

        private void DP_StartCapture()
        {
            pvtCurrentReader.On_Captured -= new Reader.CaptureCallback(OnCaptured);
            pvtCurrentReader.On_Captured += new Reader.CaptureCallback(OnCaptured);
        }

        private void DP_StopCapture()
        {
            pvtCurrentReader.On_Captured -= new Reader.CaptureCallback(OnCaptured);
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

                case "dgvChosenPayCategoryDataGridView":

                    pvtintChosenPayCategoryDataGridViewRowIndex = -1;
                    break;

                case "dgvPayCategoryDataGridView":

                    break;

                case "dgvEarningDataGridView":

                    break;

                case "dgvChosenEarningDataGridView":

                    pvtintChosenEarningDataGridViewRowIndex = -1;
                    break;

                case "dgvDeductionDataGridView":

                    break;

                case "dgvChosenDeductionDataGridView":

                    pvtintChosenDeductionDataGridViewRowIndex = -1;
                    break;

                case "dgvLeaveTypeDataGridView":

                    pvtLeaveTypeDataGridViewRowIndex = -1;
                    break;

                case "dgvEmployeeLeaveDataGridView":

                    break;

                case "dgvLoanDataGridView":

                    pvtintLoanDataGridViewRowIndex = -1;
                    break;

                case "dgvEmployeeLoanDataGridView":

                    break;

                default:

                    MessageBox.Show("No Entry for " + myDataGridView.Name + " in Set_DataGridView_SelectedRowIndex", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
#if(DEBUG)

                        System.Diagnostics.Debug.WriteLine("Set_DataGridView_SelectedRowIndex dgvEmployeeDataGridView_RowEnter intRow = " + intRow.ToString());
#endif
                        this.dgvEmployeeDataGridView_RowEnter(myDataGridView, myDataGridViewCellEventArgs);
                        break;

                    case "dgvChosenPayCategoryDataGridView":

                        this.dgvChosenPayCategoryDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPayCategoryDataGridView":

                        this.dgvPayCategoryDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEarningDataGridView":

                        this.dgvEarningDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvChosenEarningDataGridView":

                        this.dgvChosenEarningDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvDeductionDataGridView":

                        this.dgvDeductionDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvChosenDeductionDataGridView":

                        this.dgvChosenDeductionDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvLeaveTypeDataGridView":

                        this.dgvLeaveTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEmployeeLeaveDataGridView":

                        this.dgvEmployeeLeaveDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvLoanDataGridView":

                        this.dgvLoanDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEmployeeLoanDataGridView":

                        this.dgvEmployeeLoanDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        private void dgvEarningDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvEarningDataGridView.Rows.Count > 0
            & pvtblnEarningDataGridViewLoaded == true)
            {
            }
        }

        private void dgvChosenEarningDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnChosenEarningDataGridViewLoaded == true)
            {
                if (pvtintChosenEarningDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintChosenEarningDataGridViewRowIndex = e.RowIndex;

                    pvtintEmployeeEarningDataViewIndex = pvtEmployeeEarningDataView.Find(this.dgvChosenEarningDataGridView[pvtintEarningsEarningNoCol, e.RowIndex].Value);

                    int intEarningNo = Convert.ToInt32(this.dgvChosenEarningDataGridView[pvtintEarningsEarningNoCol, e.RowIndex].Value);

                    this.txtSelectedEarningDesc.Text = this.dgvChosenEarningDataGridView[pvtintEarningsDescCol, e.RowIndex].Value.ToString();

                    this.btnResetDefaultEarning.Visible = false;
                    this.lblResetDefaultEarning.Visible = false;

                    if (this.pvtstrPayrollType == "W")
                    {
                        if (pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_TYPE_IND"].ToString() == "X")
                        {
                            //Multiple - 4 Decimal Places
                            this.txtEarningPeriodAmt.Text = Convert.ToDouble(pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["AMOUNT"]).ToString("######0.0000");
                        }
                        else
                        {
                            this.txtEarningPeriodAmt.Text = Convert.ToDouble(pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["AMOUNT"]).ToString("#######0.00");
                        }
                    }
                    else
                    {
                        //Salary Value is Calculated on the Fly
                        this.txtEarningPeriodAmt.Text = this.dgvChosenEarningDataGridView[pvtintEarningsPeriodAmtCol, e.RowIndex].Value.ToString();
                    }

                    if (this.btnSave.Enabled == true)
                    {
                        this.rbnEarningUserToEnterValue.Enabled = false;
                        this.rbnEarningFixedValue.Enabled = false;
                        this.rbnEarningMultiple.Enabled = false;

                        this.rbnEarningEachPayPeriod.Enabled = false;
                        this.rbnEarningMonthly.Enabled = false;

                        this.cboEarningDay.Enabled = false;

                        this.txtEarningPeriodAmt.Enabled = false;
                    }

                    if (intEarningNo < 10
                    || intEarningNo >= 200)
                    {
                        this.rbnEarningSystemDefined.Checked = true;
                    }
                    else
                    {
                        if (pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_TYPE_IND"].ToString() == "M")
                        {
                            this.rbnEarningMacro.Checked = true;
                        }
                        else
                        {
                            if (this.btnSave.Enabled == true)
                            {
                                this.rbnEarningUserToEnterValue.Enabled = true;
                                this.rbnEarningFixedValue.Enabled = true;
                                this.rbnEarningMultiple.Enabled = true;

                                if (pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_TYPE_IND"].ToString() != "U")
                                {
                                    this.rbnEarningMonthly.Enabled = true;
                                }
                            }

                            if (pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_TYPE_IND"].ToString() == "U")
                            {
                                this.rbnEarningUserToEnterValue.Checked = true;
                            }
                            else
                            {
                                if (pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_TYPE_IND"].ToString() == "F"
                                    | pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_TYPE_IND"].ToString() == "X")
                                {
                                    if (pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_TYPE_IND"].ToString() == "F")
                                    {
                                        this.rbnEarningFixedValue.Checked = true;
                                    }
                                    else
                                    {
                                        this.rbnEarningMultiple.Checked = true;
                                    }

                                    if (this.btnSave.Enabled == true)
                                    {
                                        this.txtEarningPeriodAmt.Enabled = true;
                                        this.rbnEarningEachPayPeriod.Enabled = true;
                                        this.rbnEarningMultiple.Enabled = true;
                                    }
                                }
                                else
                                {
                                    if (this.btnSave.Enabled == false)
                                    {
                                        MessageBox.Show("flxgChosenEarning_AfterRowColChange EARNING_TYPE_IND Not Set");
                                    }
                                    else
                                    {
                                        //In Edit Mode
                                        this.rbnEarningUserToEnterValue.Checked = false;
                                        this.rbnEarningFixedValue.Checked = false;
                                        this.rbnEarningMacro.Checked = false;

                                        this.rbnEarningEachPayPeriod.Checked = false;
                                        this.rbnEarningMonthly.Checked = false;
                                    }
                                }
                            }
                        }

                        Check_If_Earning_Default();
                    }

                    if (pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_PERIOD_IND"].ToString() == "E")
                    {
                        this.cboEarningDay.SelectedIndex = -1;
                        this.rbnEarningEachPayPeriod.Checked = true;
                    }
                    else
                    {
                        if (pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_PERIOD_IND"].ToString() == "M")
                        {
                            this.rbnEarningMonthly.Checked = true;

                            if (Convert.ToInt32(pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_DAY_VALUE"]) == 99)
                            {
                                this.cboEarningDay.SelectedIndex = this.cboEarningDay.Items.Count - 1;
                            }
                            else
                            {
                                this.cboEarningDay.SelectedIndex = Convert.ToInt32(pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_DAY_VALUE"]) - 1;
                            }

                            if (this.btnSave.Enabled == true)
                            {
                                this.cboEarningDay.Enabled = true;
                            }
                        }
                        else
                        {
                            if (this.btnSave.Enabled == false)
                            {
                                MessageBox.Show("flxgChosenEarning_AfterRowColChange EARNING_PERIOD_IND Not Set");
                            }
                        }
                    }
                }
            }
        }

        private void dgvLeaveTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnLeaveTypeDataGridViewLoaded == true)
            {
                if (pvtLeaveTypeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtLeaveTypeDataGridViewRowIndex = e.RowIndex;

                    this.lblLeaveAccumTotal.Text = "0.00";
                    this.lblLeavePaidTotal.Text = "0.00";
                    this.lblLeaveFinalTotal.Text = "0.00";

                    pvtintLeaveEarningNo = Convert.ToInt32(dgvLeaveTypeDataGridView[pvtintLeaveTypeEarningNoCol, e.RowIndex].Value);
                    
                    DataGridViewComboBoxColumn myDataGridViewComboBoxColumn = (DataGridViewComboBoxColumn)dgvEmployeeLeaveDataGridView.Columns[pvtintLeaveOptionCol];

                    myDataGridViewComboBoxColumn.Items.Clear();

                    //myDataGridViewComboBoxColumn.Items.Add("");
                    myDataGridViewComboBoxColumn.Items.Add("Day/s");
                    myDataGridViewComboBoxColumn.Items.Add("Hour/s");

                    if (pvtstrPayrollType == "W")
                    {
                        if (pvtintLeaveEarningNo == 200)
                        {
                            myDataGridViewComboBoxColumn.Items.Add("Payout");
                        }
                        else
                        {
                            myDataGridViewComboBoxColumn.Items.Add("Zerorize");
                        }
                    }

                    this.lblLeaveTypeDescription.Text = this.dgvLeaveTypeDataGridView[pvtintLeaveTypeDescCol, e.RowIndex].Value.ToString();
                    this.Clear_DataGridView(this.dgvEmployeeLeaveDataGridView);

                    //Stop Events Firing
                    pvtblnEmployeeLeaveDataGridViewLoaded = false;

                    pvtEmployeeLeaveDataView = null;
                    pvtEmployeeLeaveDataView = new DataView(pvtDataSet.Tables["EmployeeLeave"],
                         "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND EARNING_NO = " + pvtintLeaveEarningNo.ToString(),
                            "SORT_ORDER,LEAVE_PROCESSED_DATE",
                            DataViewRowState.CurrentRows);

                    //  "SORT_ORDER,PROCESS_NO DESC,LEAVE_REC_NO,LEAVE_FROM_DATE,LEAVE_TO_DATE",

                    string strFromDate = "";
                    string strToDate = "";
                    string strProcessDesc = "";
                    string strOptionDesc = "";
                    string strProcessedDate = "";
                    double dblLeaveDays = 0;
                    double dblLeaveProcessedDays = 0;

                    for (int intRow = 0; intRow < pvtEmployeeLeaveDataView.Count; intRow++)
                    {
                        strFromDate = "";
                        strToDate = "";
                        strProcessDesc = "";
                        strOptionDesc = "";
                        strProcessedDate = "";
                        dblLeaveDays = 0;
                        dblLeaveProcessedDays = 0;

                        if (this.pvtEmployeeLeaveDataView[intRow]["LEAVE_PROCESSED_DATE"] == System.DBNull.Value)
                        {
                            if (pvtEmployeeLeaveDataView[intRow]["PROCESS_NO"] != System.DBNull.Value)
                            {
                                int intFindRow = pvtProcessDataView.Find(pvtEmployeeLeaveDataView[intRow]["PROCESS_NO"].ToString());

                                if (intFindRow > -1)
                                {
                                    strProcessDesc = pvtProcessDataView[intFindRow]["PROCESS_DESC"].ToString();
                                }
                            }

                            if (this.pvtEmployeeLeaveDataView[intRow]["LEAVE_OPTION"].ToString() == "D")
                            {
                                strOptionDesc = "Day/s";
                            }
                            else
                            {
                                if (this.pvtEmployeeLeaveDataView[intRow]["LEAVE_OPTION"].ToString() == "H")
                                {
                                    strOptionDesc = "Hour/s";
                                }
                                else
                                {
                                    if (this.pvtEmployeeLeaveDataView[intRow]["LEAVE_OPTION"].ToString() == "Z")
                                    {
                                        strOptionDesc = "Zerorize";

                                    }
                                    else
                                    {
                                        strOptionDesc = "Payout";
                                    }
                                }
                            }
                        }

                        if (this.pvtEmployeeLeaveDataView[intRow]["LEAVE_PROCESSED_DATE"] != System.DBNull.Value)
                        {
                            strProcessedDate = Convert.ToDateTime(this.pvtEmployeeLeaveDataView[intRow]["LEAVE_PROCESSED_DATE"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString());

                            dblLeaveProcessedDays = Convert.ToDouble(this.pvtEmployeeLeaveDataView[intRow]["LEAVE_PAID_DAYS"]);

                            dblLeaveDays = Convert.ToDouble(this.pvtEmployeeLeaveDataView[intRow]["LEAVE_DAYS_DECIMAL"]);
                        }
                        else
                        {
                            dblLeaveDays = Convert.ToDouble(this.pvtEmployeeLeaveDataView[intRow]["LEAVE_DAYS_DECIMAL"]);
                        }

                        if (this.pvtEmployeeLeaveDataView[intRow]["LEAVE_FROM_DATE"] != System.DBNull.Value)
                        {
                            strFromDate = Convert.ToDateTime(this.pvtEmployeeLeaveDataView[intRow]["LEAVE_FROM_DATE"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString());
                        }

                        if (this.pvtEmployeeLeaveDataView[intRow]["LEAVE_TO_DATE"] != System.DBNull.Value)
                        {
                            strToDate = Convert.ToDateTime(this.pvtEmployeeLeaveDataView[intRow]["LEAVE_TO_DATE"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString());
                        }

                        //NB In Edit Mode Dates etc may Be Null
                        this.dgvEmployeeLeaveDataGridView.Rows.Add("",
                                                                   "",
                                                                   this.pvtEmployeeLeaveDataView[intRow]["LEAVE_DESC"].ToString(),
                                                                   strFromDate,
                                                                   strToDate,
                                                                   dblLeaveDays.ToString("###0.00"),
                                                                   Convert.ToDouble(this.pvtEmployeeLeaveDataView[intRow]["LEAVE_HOURS_DECIMAL"]).ToString("###0.00"),
                                                                   strOptionDesc,
                                                                   strProcessDesc,
                                                                   Convert.ToDouble(this.pvtEmployeeLeaveDataView[intRow]["LEAVE_ACCUM_DAYS"]).ToString("###0.00"),
                                                                   dblLeaveProcessedDays.ToString("###0.00"),
                                                                   strProcessedDate,
                                                                   this.pvtEmployeeLeaveDataView[intRow]["LEAVE_REC_NO"].ToString());

                        //Set Read Only Rows
                        if (pvtEmployeeLeaveDataView[intRow]["LEAVE_PROCESSED_DATE"] != System.DBNull.Value)
                        {
                            this.dgvEmployeeLeaveDataGridView[0,this.dgvEmployeeLeaveDataGridView.Rows.Count - 1].Style = ReadOnlyDataGridViewCellStyle;
                            this.dgvEmployeeLeaveDataGridView.Rows[this.dgvEmployeeLeaveDataGridView.Rows.Count - 1].ReadOnly = true;

                            if (this.pvtEmployeeLeaveDataView[intRow]["LEAVE_OPTION"].ToString() == "H")
                            {
                                dgvEmployeeLeaveDataGridView[1, this.dgvEmployeeLeaveDataGridView.Rows.Count - 1].Style = this.HoursOptionDataGridViewCellStyle;
                            }
                            else
                            {
                                if (this.pvtEmployeeLeaveDataView[intRow]["LEAVE_OPTION"].ToString() == "P")
                                {
                                    dgvEmployeeLeaveDataGridView[1, this.dgvEmployeeLeaveDataGridView.Rows.Count - 1].Style = this.LeavePayoutDueDataGridViewCellStyle;
                                }
                                else
                                {
                                    if (this.pvtEmployeeLeaveDataView[intRow]["LEAVE_OPTION"].ToString() == "Z")
                                    {
                                        dgvEmployeeLeaveDataGridView[1, this.dgvEmployeeLeaveDataGridView.Rows.Count - 1].Style = this.LeaveZerorizeDataGridViewCellStyle;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Convert.ToDouble(pvtEmployeeLeaveDataView[intRow]["LEAVE_DAYS_DECIMAL"]) == 0)
                            {
                                //New Row Is Not an Error
                                if (this.btnSave.Enabled == false)
                                {
                                    if (this.pvtEmployeeLeaveDataView[intRow]["LEAVE_OPTION"].ToString() != "P"
                                    && this.pvtEmployeeLeaveDataView[intRow]["LEAVE_OPTION"].ToString() != "Z")
                                    {
                                        this.dgvEmployeeLeaveDataGridView[0,this.dgvEmployeeLeaveDataGridView.Rows.Count - 1].Style = this.ErrorDataGridViewCellStyle;
                                    }
                                }
                            }

                            if (strOptionDesc == "Payout"
                            || strOptionDesc == "Zerorize")
                                {
                                //Lock FromDate Cell
                                this.dgvEmployeeLeaveDataGridView[pvtintLeaveFromDateCol, this.dgvEmployeeLeaveDataGridView.Rows.Count - 1].ReadOnly = true;

                                //Lock ToDate Cell
                                this.dgvEmployeeLeaveDataGridView[pvtintLeaveToDateCol, this.dgvEmployeeLeaveDataGridView.Rows.Count - 1].ReadOnly = true;

                                //Lock Hours Cell
                                this.dgvEmployeeLeaveDataGridView[pvtintLeaveHoursCol, this.dgvEmployeeLeaveDataGridView.Rows.Count - 1].ReadOnly = true;
                            }
                            else
                            { 
                                if (strOptionDesc == "Hour/s")
                                {
                                    //Lock ToDate Cell
                                    this.dgvEmployeeLeaveDataGridView[pvtintLeaveToDateCol, this.dgvEmployeeLeaveDataGridView.Rows.Count - 1].ReadOnly = true;

                                    //Unlock Hours Cell
                                    this.dgvEmployeeLeaveDataGridView[pvtintLeaveHoursCol, this.dgvEmployeeLeaveDataGridView.Rows.Count - 1].ReadOnly = false;
                                }
                                else
                                {
                                    //UnLock ToDate Cell
                                    this.dgvEmployeeLeaveDataGridView[pvtintLeaveToDateCol, this.dgvEmployeeLeaveDataGridView.Rows.Count - 1].ReadOnly = false;

                                    //Lock Hours Cell
                                    this.dgvEmployeeLeaveDataGridView[pvtintLeaveHoursCol, this.dgvEmployeeLeaveDataGridView.Rows.Count - 1].ReadOnly = true;
                                }
                            }

                            if (this.pvtEmployeeLeaveDataView[intRow]["LEAVE_FROM_DATE"] != System.DBNull.Value
                            & this.pvtEmployeeLeaveDataView[intRow]["LEAVE_TO_DATE"] != System.DBNull.Value)
                            {
                                if (Convert.ToDateTime(this.pvtEmployeeLeaveDataView[intRow]["LEAVE_FROM_DATE"]) > Convert.ToDateTime(this.pvtEmployeeLeaveDataView[intRow]["LEAVE_TO_DATE"]))
                                {
                                    this.dgvEmployeeLeaveDataGridView[0,this.dgvEmployeeLeaveDataGridView.Rows.Count - 1].Style = ErrorDataGridViewCellStyle;
                                }
                                else
                                {
                                    if (this.pvtEmployeeLeaveDataView[intRow]["LEAVE_OPTION"].ToString() == "D")
                                    {
                                        int intDaysDiff = Convert.ToDateTime(this.pvtEmployeeLeaveDataView[intRow]["LEAVE_TO_DATE"]).Subtract(Convert.ToDateTime(this.pvtEmployeeLeaveDataView[intRow]["LEAVE_FROM_DATE"])).Days + 1;

                                        if (intDaysDiff != Convert.ToInt32(dblLeaveDays))
                                        {
                                            this.dgvEmployeeLeaveDataGridView[1, this.dgvEmployeeLeaveDataGridView.Rows.Count - 1].Style = LeaveDaysExcludedDataGridViewCellStyle;
                                        }
                                    }
                                    else
                                    {
                                        if (this.pvtEmployeeLeaveDataView[intRow]["LEAVE_OPTION"].ToString() == "H")
                                        {
                                            //Lock LEAVE_TO_DATE Cell
                                            dgvEmployeeLeaveDataGridView[pvtintLeaveToDateCol, this.dgvEmployeeLeaveDataGridView.Rows.Count - 1].ReadOnly = true;

                                            //Unlock LEAVE_HOURS_DECIMAL Cell
                                            dgvEmployeeLeaveDataGridView[pvtintLeaveHoursCol, this.dgvEmployeeLeaveDataGridView.Rows.Count - 1].ReadOnly = false;

                                            dgvEmployeeLeaveDataGridView[1, this.dgvEmployeeLeaveDataGridView.Rows.Count - 1].Style = this.HoursOptionDataGridViewCellStyle;
                                        }
                                        else
                                        {
                                            if (this.pvtEmployeeLeaveDataView[intRow]["LEAVE_OPTION"].ToString() == "P")
                                            {
                                                dgvEmployeeLeaveDataGridView[1, this.dgvEmployeeLeaveDataGridView.Rows.Count - 1].Style = this.LeavePayoutDueDataGridViewCellStyle;
                                            }
                                            else
                                            {
                                                if (this.pvtEmployeeLeaveDataView[intRow]["LEAVE_OPTION"].ToString() == "Z")
                                                {
                                                    dgvEmployeeLeaveDataGridView[1, this.dgvEmployeeLeaveDataGridView.Rows.Count - 1].Style = this.LeaveZerorizeDataGridViewCellStyle;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    pvtblnEmployeeLeaveDataGridViewLoaded = true;

                    //Leave Type Cannot be (None)
                    if (this.btnSave.Enabled == true)
                    {
                        this.Check_To_Add_New_Leave_Row();
                    }

                    if (dgvEmployeeLeaveDataGridView.Rows.Count > 0)
                    {
                        Calculate_Leave_Totals();
                    }
                }
            }
        }

        private void dgvLoanDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnLoanDataGridViewLoaded == true)
            {
                if (pvtintLoanDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintLoanDataGridViewRowIndex = e.RowIndex;

                    this.lblLoanPaid.Text = "0.00";
                    this.lblLoanReceived.Text = "0.00";
                    this.lblLoanTotal.Text = "0.00";

                    this.Clear_DataGridView(this.dgvEmployeeLoanDataGridView);

                    //Stop Events Firing
                    pvtblnEmployeeLoanDataGridViewLoaded = false;

                    pvtintDeductionNo = Convert.ToInt32(this.dgvLoanDataGridView[pvtintLoanDescriptionDeductionNoCol, e.RowIndex].Value);
                    pvtintDeductionSubNo = Convert.ToInt32(this.dgvLoanDataGridView[pvtintLoanDescriptionDeductionSubNoCol, e.RowIndex].Value);

                    this.lblLoanDescription.Text = this.dgvLoanDataGridView[pvtintLoanDescriptionDescCol, e.RowIndex].Value.ToString();

                    string strProcessDesc = "";
                    string strProcessDate = "";

                    if (this.rbnLoanCurrentYear.Checked == true)
                    {
                        pvtEmployeeLoanCFDataView = null;
                        pvtEmployeeLoanCFDataView = new DataView(pvtDataSet.Tables["EmployeeLoanCF"],
                            pvtstrFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND DEDUCTION_NO = " + pvtintDeductionNo.ToString() + " AND DEDUCTION_SUB_ACCOUNT_NO = " + pvtintDeductionSubNo.ToString(),
                            "",
                            DataViewRowState.CurrentRows);

                        if (pvtEmployeeLoanCFDataView.Count > 0)
                        {
                            this.dgvEmployeeLoanDataGridView.Rows.Add("",
                                                                      this.pvtEmployeeLoanCFDataView[0]["LOAN_DESC"].ToString(),
                                                                      "",
                                                                      Convert.ToDouble(this.pvtEmployeeLoanCFDataView[0]["LOAN_AMOUNT_PAID"]).ToString("########0.00"),
                                                                      Convert.ToDouble(this.pvtEmployeeLoanCFDataView[0]["LOAN_AMOUNT_RECEIVED"]).ToString("########0.00"),
                                                                      Convert.ToDateTime(this.pvtEmployeeLoanCFDataView[0]["LOAN_PROCESSED_DATE"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()),
                                                                      "1");

                            this.dgvEmployeeLoanDataGridView[0,this.dgvEmployeeLoanDataGridView.Rows.Count - 1].Style = ReadOnlyDataGridViewCellStyle;

                            this.dgvEmployeeLoanDataGridView.Rows[this.dgvEmployeeLoanDataGridView.Rows.Count - 1].ReadOnly = true;

                            pvtEmployeeLoanDataView = null;
                            pvtEmployeeLoanDataView = new DataView(pvtDataSet.Tables["EmployeeLoan"],
                                pvtstrFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND DEDUCTION_NO = " + pvtintDeductionNo.ToString() + " AND DEDUCTION_SUB_ACCOUNT_NO = " + pvtintDeductionSubNo.ToString() + " AND (LOAN_PROCESSED_DATE >= '" + pvtDataSet.Tables["BeginYear"].Rows[0]["FISCAL_START_DATE"].ToString() + "' OR LOAN_PROCESSED_DATE IS NULL) ",
                                "SORT_ORDER,LOAN_PROCESSED_DATE,LOAN_REC_NO",
                                DataViewRowState.CurrentRows);
                        }
                        else
                        {
                            pvtEmployeeLoanDataView = null;
                            pvtEmployeeLoanDataView = new DataView(pvtDataSet.Tables["EmployeeLoan"],
                                pvtstrFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND DEDUCTION_NO = " + pvtintDeductionNo.ToString() + " AND DEDUCTION_SUB_ACCOUNT_NO = " + pvtintDeductionSubNo.ToString(),
                                "SORT_ORDER,LOAN_PROCESSED_DATE,LOAN_REC_NO",
                                DataViewRowState.CurrentRows);
                        }
                    }
                    else
                    {
                        pvtEmployeeLoanDataView = null;
                        pvtEmployeeLoanDataView = new DataView(pvtDataSet.Tables["EmployeeLoan"],
                            pvtstrFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND DEDUCTION_NO = " + pvtintDeductionNo.ToString() + " AND DEDUCTION_SUB_ACCOUNT_NO = " + pvtintDeductionSubNo.ToString(),
                            "SORT_ORDER,LOAN_PROCESSED_DATE,LOAN_REC_NO",
                            DataViewRowState.CurrentRows);
                    }

                    for (int intLoanRow = 0; intLoanRow < pvtEmployeeLoanDataView.Count; intLoanRow++)
                    {
                        strProcessDesc = "";
                        strProcessDate = "";

                        if (this.pvtEmployeeLoanDataView[intLoanRow]["LOAN_PROCESSED_DATE"] != System.DBNull.Value)
                        {
                            strProcessDate = Convert.ToDateTime(this.pvtEmployeeLoanDataView[intLoanRow]["LOAN_PROCESSED_DATE"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString());
                        }
                        else
                        {
                            if (this.pvtEmployeeLoanDataView[intLoanRow]["PROCESS_NO"] != System.DBNull.Value)
                            {
                                int intFindRow = pvtProcessDataView.Find(pvtEmployeeLoanDataView[intLoanRow]["PROCESS_NO"].ToString());

                                if (intFindRow > -1)
                                {
                                    strProcessDesc = pvtProcessDataView[intFindRow]["PROCESS_DESC"].ToString();
                                }
                            }
                        }

                        this.dgvEmployeeLoanDataGridView.Rows.Add("",
                                                                  this.pvtEmployeeLoanDataView[intLoanRow]["LOAN_DESC"].ToString(),
                                                                  strProcessDesc,
                                                                  Convert.ToDouble(this.pvtEmployeeLoanDataView[intLoanRow]["LOAN_AMOUNT_PAID"]).ToString("########0.00"),
                                                                  Convert.ToDouble(this.pvtEmployeeLoanDataView[intLoanRow]["LOAN_AMOUNT_RECEIVED"]).ToString("########0.00"),
                                                                  strProcessDate,
                                                                  this.pvtEmployeeLoanDataView[intLoanRow]["LOAN_REC_NO"].ToString());

                        if (this.pvtEmployeeLoanDataView[intLoanRow]["LOAN_PROCESSED_DATE"] != System.DBNull.Value)
                        {
                            this.dgvEmployeeLoanDataGridView[0,this.dgvEmployeeLoanDataGridView.Rows.Count - 1].Style = ReadOnlyDataGridViewCellStyle;

                            this.dgvEmployeeLoanDataGridView.Rows[this.dgvEmployeeLoanDataGridView.Rows.Count - 1].ReadOnly = true;
                        }
                    }

                    pvtblnEmployeeLoanDataGridViewLoaded = true;

                    if (this.btnSave.Enabled == true)
                    {
                        this.Check_To_Add_New_Loan_Row();
                    }

                    Calculate_Loan_Totals();
                }
            }
        }

        private void Check_If_Earning_Default()
        {
            if (Convert.ToInt32(pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_NO"]) > 9
                & Convert.ToInt32(pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_NO"]) < 200)
            {
                this.btnResetDefaultEarning.Visible = false;
                this.lblResetDefaultEarning.Visible = false;

                DataView EmployeeEarningDataView = new DataView(pvtDataSet.Tables["Earning"],
                     pvtstrFilter + " AND EARNING_NO = " + pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_NO"].ToString(),
                    "",
                    DataViewRowState.CurrentRows);

                if (EmployeeEarningDataView.Count > 0)
                {
                    if (pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_TYPE_IND"].ToString() == EmployeeEarningDataView[0]["EARNING_TYPE_IND"].ToString()
                        & pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_PERIOD_IND"].ToString() == EmployeeEarningDataView[0]["EARNING_PERIOD_IND"].ToString()
                        & pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_DAY_VALUE"].ToString() == EmployeeEarningDataView[0]["EARNING_DAY_VALUE"].ToString()
                        & Convert.ToDouble(pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["AMOUNT"]) == Convert.ToDouble(EmployeeEarningDataView[0]["AMOUNT"]))
                    {
                    }
                    else
                    {
                        this.btnResetDefaultEarning.Visible = true;
                        this.lblResetDefaultEarning.Visible = true;

                        if (this.btnSave.Enabled == true)
                        {
                            this.btnResetDefaultEarning.Enabled = true;
                        }
                    }
                }
            }
        }

        private void Check_If_Deduction_Default()
        {
            if (pvtintDeductionAccountNo > 2)
            {
                this.btnResetDefaultDeduction.Visible = false;
                this.lblResetDefault.Visible = false;

                DataView DeductionDataView = new DataView(pvtDataSet.Tables["Deduction"],
                     pvtstrFilter + " AND DEDUCTION_NO = " + pvtintDeductionAccountNo + " AND DEDUCTION_SUB_ACCOUNT_NO = " + pvtintDeductionSubAccountNo,
                    "",
                    DataViewRowState.CurrentRows);

                if (DeductionDataView.Count > 0)
                {
                    if (pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_TYPE_IND"].ToString() == DeductionDataView[0]["DEDUCTION_TYPE_IND"].ToString()
                    && pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_PERIOD_IND"].ToString() == DeductionDataView[0]["DEDUCTION_PERIOD_IND"].ToString()
                    && pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_DAY_VALUE"].ToString() == DeductionDataView[0]["DEDUCTION_DAY_VALUE"].ToString()
                    && Convert.ToDouble(pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_VALUE"]) == Convert.ToDouble(DeductionDataView[0]["DEDUCTION_VALUE"])
                    && Convert.ToDouble(pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_MIN_VALUE"]) == Convert.ToDouble(DeductionDataView[0]["DEDUCTION_MIN_VALUE"])
                    && Convert.ToDouble(pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_MAX_VALUE"]) == Convert.ToDouble(DeductionDataView[0]["DEDUCTION_MAX_VALUE"]))
                    {
                        //ELR 2018-11-10 
                        DataView DeductionEarningPercentageDefaultDataView = new DataView(pvtDataSet.Tables["DeductionEarningPercentageDefault"],
                           pvtstrFilter + " AND DEDUCTION_NO = " + pvtintDeductionAccountNo + " AND DEDUCTION_SUB_ACCOUNT_NO = " + pvtintDeductionSubAccountNo,
                        "",
                        DataViewRowState.CurrentRows);

                        if (DeductionEarningPercentageDefaultDataView.Count > 0)
                        {
                            DataView DeductionEarningPercentageDataView = new DataView(pvtDataSet.Tables["DeductionEarningPercentage"],
                               pvtstrFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND DEDUCTION_NO = " + pvtintDeductionAccountNo + " AND DEDUCTION_SUB_ACCOUNT_NO = " + pvtintDeductionSubAccountNo,
                            "",
                            DataViewRowState.CurrentRows);

                            if (DeductionEarningPercentageDefaultDataView.Count != DeductionEarningPercentageDataView.Count)
                            {
                                this.btnResetDefaultDeduction.Visible = true;
                                this.lblResetDefault.Visible = true;

                                if (this.btnSave.Enabled == true)
                                {
                                    this.btnResetDefaultDeduction.Enabled = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        this.btnResetDefaultDeduction.Visible = true;
                        this.lblResetDefault.Visible = true;

                        if (this.btnSave.Enabled == true)
                        {
                            this.btnResetDefaultDeduction.Enabled = true;
                        }
                    }
                }
            }
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

                this.tabEmployee.Enabled = false;
            }
            else
            {
                Clear_Form_Fields();
                
                this.dgvChosenEarningDataGridView.Columns[4].ReadOnly = false;

                this.tabEmployee.SelectedIndex = 0;

                pvtintEmployeeNo = -1;
                pvtintLeaveEarningNo = -1;
                pvtintDeductionAccountNo = -1;
                pvtintDeductionSubAccountNo = -1;

                this.lblLeaveTypeDescription.Text = "";
                this.txtLeaveDescription.Text = "";
                this.txtLeaveDayRate.Text = "";

                pvtLeaveTypeDataView = null;
                pvtLeaveTypeDataView = new DataView(pvtDataSet.Tables["LeaveType"],
                    pvtstrFilter,
                    "EARNING_DESC",
                    DataViewRowState.CurrentRows);

                Load_Company_Leave();

                Set_Form_For_Read();

                //if (this.rbnNone.Checked == true
                //&& rbnActive.Checked == true)
                //{
                //    this.pnlFilter.Visible = false;
                //    this.picEmployeeLock.Visible = false;
                //    this.picPayrollTypeLock.Visible = false;
                //}
                //else
                //{
                //    this.pnlFilter.Visible = true;

                //    this.picEmployeeLock.Image = global::InteractPayroll.Properties.Resources.filter16;
                //    this.picEmployeeLock.Visible = true;

                //    this.picPayrollTypeLock.Image = global::InteractPayroll.Properties.Resources.filter16;
                //    this.picPayrollTypeLock.Visible = true;
                //}

                Load_Employees();

                if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                {
                    this.Clear_DataGridView(this.dgvLeaveTypeDataGridView);

                    pvtLeaveTypeDataGridViewRowIndex = -1;

                    this.pvtblnLeaveTypeDataGridViewLoaded = false;

                    for (int intRow = 0; intRow < pvtLeaveTypeDataView.Count; intRow++)
                    {
                        this.dgvLeaveTypeDataGridView.Rows.Add("",
                                                               pvtLeaveTypeDataView[intRow]["EARNING_DESC"].ToString(),
                                                               "0.00",
                                                               pvtLeaveTypeDataView[intRow]["EARNING_NO"].ToString());
                    }

                    this.pvtblnLeaveTypeDataGridViewLoaded = true;

                    if (this.dgvLeaveTypeDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvLeaveTypeDataGridView, 0);
                    }
                }
            }
        }

        private void Check_To_Add_New_Leave_Row()
        {
            if (this.Text.EndsWith("- Update") == true)
            {
                //Only Allow Leave when Employee has been Activated (Taken-On)
                if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_LAST_RUNDATE"] != System.DBNull.Value)
                {
                    if (this.dgvEmployeeLeaveDataGridView.Rows.Count == 0)
                    {
                    }
                    else
                    {
                        DataView CurrentEmployeeLeave = new DataView(pvtDataSet.Tables["EmployeeLeave"],
                            pvtstrFilter + " AND EMPLOYEE_NO = " + this.pvtintEmployeeNo.ToString() + " AND EARNING_NO = " + this.dgvLeaveTypeDataGridView[pvtintLeaveTypeEarningNoCol, pvtLeaveTypeDataGridViewRowIndex].Value.ToString(),
                            "LEAVE_REC_NO DESC",
                            DataViewRowState.CurrentRows);

                        if (CurrentEmployeeLeave[0]["LEAVE_DESC"].ToString() == ""
                            | CurrentEmployeeLeave[0]["LEAVE_FROM_DATE"] == System.DBNull.Value
                            | CurrentEmployeeLeave[0]["LEAVE_TO_DATE"] == System.DBNull.Value
                            | CurrentEmployeeLeave[0]["PROCESS_NO"] == System.DBNull.Value
                            | CurrentEmployeeLeave[0]["LEAVE_OPTION"].ToString() == "")
                        {
                            return;
                        }
                        else
                        {
                            //Hours
                            if (CurrentEmployeeLeave[0]["LEAVE_OPTION"].ToString() == "H")
                            {
                                if (Convert.ToDouble(CurrentEmployeeLeave[0]["LEAVE_HOURS_DECIMAL"]) == 0)
                                {
                                    return;
                                }
                            }
                        }
                    }

                    if (pvtEmployeeLeaveDataView.Count == 0)
                    {
                        if (this.dgvLeaveTypeDataGridView.Rows.Count == 0)
                        {
                            return;
                        }
                    }

                    if (this.dgvLeaveTypeDataGridView.Rows.Count > 0)
                    {
                        int intLeaveRow = 1;

                        DataView CheckExistsDataView = new DataView(pvtDataSet.Tables["EmployeeLeave"],
                            pvtstrFilter + " AND EMPLOYEE_NO = " + this.pvtintEmployeeNo.ToString() + " AND EARNING_NO = " + this.dgvLeaveTypeDataGridView[pvtintLeaveTypeEarningNoCol, pvtLeaveTypeDataGridViewRowIndex].Value.ToString() + " AND LEAVE_REC_NO < 9999",
                            "LEAVE_REC_NO DESC",
                            DataViewRowState.CurrentRows);

                        if (CheckExistsDataView.Count > 0)
                        {
                            intLeaveRow = Convert.ToInt32(CheckExistsDataView[0]["LEAVE_REC_NO"]) + 1;
                        }

                        DataRowView drvDataRowView = this.pvtEmployeeLeaveDataView.AddNew();

                        drvDataRowView.BeginEdit();

                        //Set First Part of Key
                        drvDataRowView["COMPANY_NO"] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                        drvDataRowView["PAY_CATEGORY_TYPE"] = pvtstrPayrollType;
                        drvDataRowView["EMPLOYEE_NO"] = pvtintEmployeeNo;

                        drvDataRowView["EARNING_NO"] = Convert.ToInt32(this.dgvLeaveTypeDataGridView[pvtintLeaveTypeEarningNoCol, pvtLeaveTypeDataGridViewRowIndex].Value);
                        drvDataRowView["LEAVE_REC_NO"] = intLeaveRow;

                        drvDataRowView["LEAVE_OPTION"] = System.DBNull.Value;
                        drvDataRowView["LEAVE_PROCESSED_DATE"] = System.DBNull.Value;

                        //Added so that View will See added Row 
                        drvDataRowView["LEAVE_ACCUM_DAYS"] = 0;
                        drvDataRowView["LEAVE_PAID_DAYS"] = 0;
                        drvDataRowView["LEAVE_HOURS_DECIMAL"] = 0;
                        drvDataRowView["LEAVE_DAYS_DECIMAL"] = 0;
                        drvDataRowView["PROCESS_NO"] = System.DBNull.Value;

                        //Important - Make sure added to End of Dataview
                        drvDataRowView["SORT_ORDER"] = 3;

                        drvDataRowView.EndEdit();

                        //ERROL REMOVED 2013-08-23 (HOPE CORRECT)
                        //pvtintEmployeeDataGridViewRowIndex = -1;
                        pvtblnEmployeeDataGridViewLoaded = false;
                        
                        this.dgvEmployeeLeaveDataGridView.Rows.Add("",
                                                                   "",
                                                                   "",
                                                                   "",
                                                                   "",
                                                                   "0.00",
                                                                   "0.00",
                                                                   "",
                                                                   "",
                                                                   "0.00",
                                                                   "0.00",
                                                                   "",
                                                                   intLeaveRow.ToString());

                        pvtblnEmployeeDataGridViewLoaded = true;

                        //Lock Hours Cell
                        this.dgvEmployeeLeaveDataGridView[pvtintLeaveHoursCol, this.dgvEmployeeLeaveDataGridView.Rows.Count - 1].ReadOnly = true;

                        dgvEmployeeLeaveDataGridView.FirstDisplayedScrollingRowIndex = dgvEmployeeLeaveDataGridView.Rows.Count - 1;
                    }
                }
            }
        }

        private void Calculate_Leave_Totals()
        {
            double dblLeaveAccum = 0;
            double dblLeavePaid = 0;
            double dblFinalLeaveTotal = 0;

            for (int intRow = 0; intRow < this.dgvEmployeeLeaveDataGridView.Rows.Count; intRow++)
            {
                dblLeaveAccum += Convert.ToDouble(this.dgvEmployeeLeaveDataGridView[pvtintLeaveAccumValueCol, intRow].Value);
                dblLeavePaid += Convert.ToDouble(this.dgvEmployeeLeaveDataGridView[pvtintLeavePaidValueCol, intRow].Value);
            }

            this.lblLeaveAccumTotal.Text = dblLeaveAccum.ToString("####0.00");
            this.lblLeavePaidTotal.Text = dblLeavePaid.ToString("####0.00");

            dblFinalLeaveTotal = dblLeaveAccum - dblLeavePaid;

            this.lblLeaveFinalTotal.Text = dblFinalLeaveTotal.ToString("####0.00");

            if (this.dgvLeaveTypeDataGridView.Rows.Count > 0)
            {
                this.dgvLeaveTypeDataGridView[pvtintLeaveTypeAccumTotalCol, pvtLeaveTypeDataGridViewRowIndex].Value = dblFinalLeaveTotal.ToString("####0.00");
            }
        }

        private void Check_To_Add_New_Loan_Row()
        {
            if (this.dgvLoanDataGridView.Rows.Count > 0)
            {
                //Only Allow Loan when Employee has been Activated (Taken-On)
                if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_LAST_RUNDATE"] != System.DBNull.Value)
                {
                    if (this.dgvEmployeeLoanDataGridView.Rows.Count == 0)
                    {
                    }
                    else
                    {
                        //Last Record
                        int intLoanRecNo = Convert.ToInt32(dgvEmployeeLoanDataGridView[pvtintLoanRecNoCol, this.dgvEmployeeLoanDataGridView.Rows.Count - 1].Value);

                        DataView CurrentEmployeeLoan = new DataView(pvtDataSet.Tables["EmployeeLoan"],
                            pvtstrFilter + " AND EMPLOYEE_NO = " + this.pvtintEmployeeNo.ToString() + " AND DEDUCTION_NO = " + pvtintDeductionNo.ToString() + " AND DEDUCTION_SUB_ACCOUNT_NO = " + pvtintDeductionSubNo.ToString() + " AND LOAN_REC_NO = " + intLoanRecNo.ToString(),
                            "",
                            DataViewRowState.CurrentRows);

                        if (CurrentEmployeeLoan.Count > 0)
                        {
                            if (CurrentEmployeeLoan[0]["LOAN_DESC"].ToString() == ""
                            | CurrentEmployeeLoan[0]["PROCESS_NO"] == System.DBNull.Value
                            | Convert.ToDouble(CurrentEmployeeLoan[0]["LOAN_AMOUNT_PAID"]) == 0)
                            {
                                if (CurrentEmployeeLoan[0]["LOAN_PROCESSED_DATE"] == System.DBNull.Value)
                                {
                                    return;
                                }
                            }
                        }
                    }

                    int intRecNo = 1;

                    DataView pvtCheckExistsDataView = new DataView(pvtDataSet.Tables["EmployeeLoan"],
                        pvtstrFilter + " AND EMPLOYEE_NO = " + this.pvtintEmployeeNo.ToString() + " AND DEDUCTION_NO = " + pvtintDeductionNo.ToString() + " AND DEDUCTION_SUB_ACCOUNT_NO = " + pvtintDeductionSubNo.ToString(),
                        "LOAN_REC_NO DESC",
                        DataViewRowState.CurrentRows);

                    if (pvtCheckExistsDataView.Count > 0)
                    {
                        intRecNo = Convert.ToInt32(pvtCheckExistsDataView[0]["LOAN_REC_NO"]) + 1;
                    }

                    DataRowView drvDataRowView = this.pvtEmployeeLoanDataView.AddNew();

                    drvDataRowView.BeginEdit();

                    //Set First Part of Key
                    drvDataRowView["COMPANY_NO"] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    drvDataRowView["PAY_CATEGORY_TYPE"] = pvtstrPayrollType;
                    drvDataRowView["DEDUCTION_NO"] = Convert.ToInt32(pvtintDeductionNo);
                    drvDataRowView["DEDUCTION_SUB_ACCOUNT_NO"] = Convert.ToInt32(pvtintDeductionSubNo);
                    drvDataRowView["EMPLOYEE_NO"] = pvtintEmployeeNo;

                    drvDataRowView["LOAN_REC_NO"] = intRecNo;
                    drvDataRowView["PROCESS_NO"] = System.DBNull.Value;

                    drvDataRowView["LOAN_AMOUNT_RECEIVED"] = 0;
                    drvDataRowView["LOAN_AMOUNT_PAID"] = 0;
                    //Important - To Keep Record at End of Dataview
                    drvDataRowView["SORT_ORDER"] = intRecNo;

                    drvDataRowView.EndEdit();

                    this.dgvEmployeeLoanDataGridView.Rows.Add("",
                                                              "",
                                                              "",
                                                              "0.00",
                                                              "0.00",
                                                              "",
                                                              intRecNo.ToString());

                    this.dgvEmployeeLoanDataGridView.FirstDisplayedScrollingRowIndex = dgvEmployeeLoanDataGridView.Rows.Count - 1;
                }
            }
        }

        private void Calculate_Loan_Totals()
        {
            double dblLoanPaid = 0;
            double dblLoanReceived = 0;
            double dblFinalLoanTotal = 0;

            for (int intRow = 0; intRow < this.dgvEmployeeLoanDataGridView.Rows.Count; intRow++)
            {
                dblLoanPaid += Convert.ToDouble(this.dgvEmployeeLoanDataGridView[pvtintLoanPaidCol, intRow].Value);
                dblLoanReceived += Convert.ToDouble(this.dgvEmployeeLoanDataGridView[pvtintLoanReceivedCol, intRow].Value);
            }

            this.lblLoanPaid.Text = dblLoanPaid.ToString("####0.00");
            this.lblLoanReceived.Text = dblLoanReceived.ToString("####0.00");

            dblFinalLoanTotal = dblLoanPaid - dblLoanReceived;

            this.lblLoanTotal.Text = dblFinalLoanTotal.ToString("####0.00");

            this.dgvLoanDataGridView[pvtintLoanDescriptionOutstandingAmtCol, pvtintLoanDataGridViewRowIndex].Value = dblFinalLoanTotal.ToString("####0.00");
        }

        private void Clear_Form_Fields()
        {
            //Pay Category
            this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
            this.Clear_DataGridView(this.dgvChosenPayCategoryDataGridView);

            this.txtRate.Text = "";
            this.txtSelectedPayCategoryDesc.Text = "";
            this.cboEarningNoCheques.SelectedIndex = 0;

            //Earnings
            this.Clear_DataGridView(this.dgvEarningDataGridView);
            this.Clear_DataGridView(this.dgvChosenEarningDataGridView);

            this.txtSelectedEarningDesc.Text = "";
            this.txtEarningPeriodAmt.Text = "";
            this.cboEarningDay.SelectedIndex = -1;

            //Deductions
            this.Clear_DataGridView(this.dgvDeductionDataGridView);
            this.Clear_DataGridView(this.dgvChosenDeductionDataGridView);
            this.Clear_DataGridView(this.dgvChosenDeductionEarningLinkDataGridView);

            this.txtSelectedDeductionDesc.Text = "";
            this.txtDeductMaxValue.Text = "";
            this.txtDeductMinValue.Text = "";
            this.txtDeductValue.Text = "";
            this.cboDeductDay.SelectedIndex = -1;
            this.txtNumberMedicalDependents.Text = "";
            this.chkDisability.Checked = false;
            this.chkPayslip.Checked = false;
            this.chkEmpNo.Checked = false;

            this.btnResetDefaultDeduction.Visible = false;
            this.lblResetDefault.Visible = false;

            //Leave
            this.Clear_DataGridView(this.dgvLeaveTypeDataGridView);
            this.Clear_DataGridView(this.dgvEmployeeLeaveDataGridView);
            this.txtLeaveDescription.Text = "";
            this.txtLeaveDayRate.Text = "";

            //Loans
            this.Clear_DataGridView(this.dgvLoanDataGridView);
            this.Clear_DataGridView(this.dgvEmployeeLoanDataGridView);

            this.lblLeaveAccumTotal.Text = "0.00";
            this.lblLeavePaidTotal.Text = "0.00";
            this.lblLeaveFinalTotal.Text = "0.00";

            this.lblLoanPaid.Text = "0.00";
            this.lblLoanReceived.Text = "0.00";
            this.lblLoanTotal.Text = "0.00";
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

            if (rbnActive.Checked == true)
            {
                this.btnNew.Enabled = true;
            }

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.lblMessage.Visible = false;
            this.btnClearFingers.Visible = false;

            this.pnlEnroll.Visible = false;
            this.pnlFingers.Visible = true;

            clsISUtilities.Set_Form_For_Read();

            this.dgvEmployeeDataGridView.Enabled = true;
            this.dgvPayrollTypeDataGridView.Enabled = true;

            this.picEmployeeLock.Visible = false;
            this.picPayrollTypeLock.Visible = false;

            //Used to Hold New EMPLOYEE_CODE temporarily for New record
            this.txtNewCode.Visible = false;

            this.cboLeaveShift.Enabled = false;

            this.cboEtiStartMonth.Enabled = false;

            this.rbnActive.Enabled = true;
            this.rbnClosed.Enabled = true;

            this.rbnNone.Enabled = true;
            this.rbnTemplateMissing.Enabled = true;
            this.rbnEnrolledFingerprint.Enabled = true;

            this.rbnLoanCurrentYear.Enabled = true;
            this.rbnLoanAllYears.Enabled = true;

            this.rbnTaxNormal.Enabled = false;
            this.rbnTaxPartTime.Enabled = false;
            this.rbnTaxDirective.Enabled = false;

            this.rbnResAddrOwn.Enabled = false;
            this.rbnResAddrCompany.Enabled = false;
            this.rbnWorkTelOwn.Enabled = false;
            this.rbnWorkTelCompany.Enabled = false;

            this.rbnSameResidentialAddr.Enabled = false;
            this.rbnStreetAddr.Enabled = false;
            this.rbnPOBoxAddr.Enabled = false;
            this.rbnPrivateBagAddr.Enabled = false;

            this.chkPayslip.Enabled = false;
            this.chkEmpNo.Enabled = false;

            this.btnPhsicalAddressRSA.Enabled = false;
            this.btnPostalAddressRSA.Enabled = false;

            //Disable Pay Category Fields
            this.rbnDefaultYes.Enabled = false;
            this.rbnDefaultNo.Enabled = false;
            this.txtRate.Enabled = false;
            this.btnPayCategoryAdd.Enabled = false;
            this.btnPayCategoryRemove.Enabled = false;

            //Disable Earning Fields
            this.rbnEarningSystemDefined.Enabled = false;
            this.rbnEarningUserToEnterValue.Enabled = false;
            this.rbnEarningFixedValue.Enabled = false;
            this.rbnEarningMacro.Enabled = false;

            this.rbnEarningEachPayPeriod.Enabled = false;
            this.rbnEarningMultiple.Enabled = false;
            this.rbnEarningMonthly.Enabled = false;
            this.cboEarningDay.Enabled = false;

            this.txtEarningPeriodAmt.Enabled = false;
            this.cboEarningNoCheques.Enabled = false;

            this.btnEarningAdd.Enabled = false;
            this.btnEarningRemove.Enabled = false;

            this.btnResetDefaultEarning.Enabled = false;

            //Disable Deduction Fields
            this.rbnDeductUserToEnter.Enabled = false;
            this.rbnDeductFixedValue.Enabled = false;
            this.rbnDeductPercentOfEarnings.Enabled = false;

            this.rbnDeductEachPayPeriod.Enabled = false;
            this.rbnDeductMonthly.Enabled = false;

            this.cboDeductDay.Enabled = false;

            this.txtDeductValue.Enabled = false;
            this.txtDeductMinValue.Enabled = false;
            this.txtDeductMaxValue.Enabled = false;

            this.btnResetDefaultDeduction.Enabled = false;

            this.btnDeductionAdd.Enabled = false;
            this.btnDeductionRemove.Enabled = false;

            this.btnLeaveDeleteRec.Enabled = false;

            this.dgvChosenDeductionEarningLinkDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvChosenDeductionEarningLinkDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

            this.dgvEmployeeLeaveDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvEmployeeLeaveDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

            this.dgvEmployeeLoanDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvEmployeeLoanDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

            this.btnLoanDeleteRec.Enabled = false;

            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_CONFIG_ERROR_IND"].ToString() == "Y"
                           | Convert.ToInt32(pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_PAY_CATEGORY_NO"]) == 0
                           | Convert.ToInt32(pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_PAID_ACCUMULATOR_IND"]) == 0)
                {
                    this.dgvEmployeeDataGridView[0,pvtintEmployeeDataGridViewRowIndex].Style = ErrorDataGridViewCellStyle;
                }
                else
                {
                    this.dgvEmployeeDataGridView[0,pvtintEmployeeDataGridViewRowIndex].Style = NormalDataGridViewCellStyle;

                }
            }
        }

        private void Load_Employees()
        {
            string strEmployeeFilter = "";

            if (this.rbnActive.Checked == true)
            {
                strEmployeeFilter = " AND EMPLOYEE_ENDDATE IS NULL";
            }
            else
            {
                strEmployeeFilter = " AND NOT EMPLOYEE_ENDDATE IS NULL";
            }

            pvtEmployeeDataView = null;
            pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                pvtstrFilter + strEmployeeFilter,
                "EMPLOYEE_CODE",
                DataViewRowState.CurrentRows);

            //Set Here So That pvtDeductionEarningPercentageDataView NOT Null (Used in Deduction Add Button)
            pvtDeductionEarningPercentageDataView = null;
            pvtDeductionEarningPercentageDataView = new DataView(pvtDataSet.Tables["DeductionEarningPercentage"],
                pvtstrFilter + " AND EMPLOYEE_NO = " + this.pvtintEmployeeNo,
                "EARNING_NO",
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

                clsISUtilities.DataBind_DataView_To_TextBox(this.txtInitials, "EMPLOYEE_INITIALS", true, "Enter Employee Initials.", true);

                clsISUtilities.DataBind_DataView_To_ComboBox(cboNatureOfPerson, "NATURE_PERSON_NO", true, "Select Nature of Person.", true);

                clsISUtilities.DataBind_DataView_To_Numeric_TextBox(txtIDNo, "EMPLOYEE_ID_NO", 0, false, "", false, 0, true);
                clsISUtilities.DataBind_Control_Required_If_Enabled(txtIDNo, "Enter Valid Employee ID. Number.");
                clsISUtilities.DataBind_Numeric_Field_SA_ID_Number(txtIDNo);

                clsISUtilities.DataBind_DataView_To_TextBox(txtPassportNo, "EMPLOYEE_PASSPORT_NO", false, "", false);
                clsISUtilities.DataBind_Control_Required_If_Enabled(txtPassportNo, "Enter Passport Number.");

                clsISUtilities.DataBind_DataView_To_ComboBox(cboCountry, "EMPLOYEE_PASSPORT_COUNTRY_CODE", false, "", false);
                clsISUtilities.DataBind_Control_Required_If_Enabled(cboCountry, "Select Country where Passport was Issued.");

                clsISUtilities.DataBind_DataView_To_ComboBox(cboMaritalStatus, "MARITAL_STATUS_NO", true, "Select Marital Status.", true);
                clsISUtilities.DataBind_DataView_To_ComboBox(cboOccupation, "OCCUPATION_NO", true, "Select Occupation.", true);
                clsISUtilities.DataBind_DataView_To_ComboBox(cboDepartment, "DEPARTMENT_NO", true, "Select Department.", true);

                clsISUtilities.DataBind_DataView_To_ComboBox(cboGender, "GENDER_IND", true, "Select Gender.", true);
                clsISUtilities.DataBind_DataView_To_ComboBox(cboRace, "RACE_NO", true, "Select Race.", true);
                clsISUtilities.DataBind_DataView_To_Date_TextBox(txtBirthDate, "EMPLOYEE_BIRTHDATE", true, "Capture Birth Date.");

                clsISUtilities.DataBind_DataView_To_Date_TextBox_ReadOnly(txtStartDate, "EMPLOYEE_TAX_STARTDATE");
                clsISUtilities.DataBind_DataView_To_Date_TextBox_ReadOnly(txtEffectiveDate, "EMPLOYEE_LAST_RUNDATE");
                clsISUtilities.DataBind_DataView_To_Date_TextBox_ReadOnly(txtDateClosed, "EMPLOYEE_ENDDATE");

                clsISUtilities.DataBind_DataView_To_TextBox(txtResAddrUnitNumber, "EMPLOYEE_RES_UNIT_NUMBER", false, "", false);
                clsISUtilities.DataBind_DataView_To_TextBox(txtResAddrComplex, "EMPLOYEE_RES_COMPLEX", false, "", false);
                clsISUtilities.DataBind_DataView_To_TextBox(txtResAddrStreetNumber, "EMPLOYEE_RES_STREET_NUMBER", false, "", false);
                
                clsISUtilities.DataBind_DataView_To_TextBox(this.txtStreetName, "EMPLOYEE_RES_STREET_NAME", true, "Enter Street / Farm Name.", true);
                clsISUtilities.DataBind_Special_Field(this.txtStreetName, 1);
                clsISUtilities.DataBind_DataView_To_TextBox(this.txtSuburb, "EMPLOYEE_RES_SUBURB", false, "", true);
                clsISUtilities.DataBind_Special_Field(this.txtSuburb, 1);
                clsISUtilities.DataBind_DataView_To_TextBox(this.txtCity, "EMPLOYEE_RES_CITY", false, "", true);
                clsISUtilities.DataBind_Special_Field(this.txtCity, 1);

                clsISUtilities.DataBind_DataView_To_TextBox(this.txtPhysicalCode, "EMPLOYEE_RES_CODE", false, "", true);
                clsISUtilities.DataBind_Special_Field(this.txtPhysicalCode, 1);
                
                clsISUtilities.DataBind_DataView_To_ComboBox(this.cboAddressCountry, "EMPLOYEE_RES_COUNTRY_CODE2", false, "", false);
                clsISUtilities.DataBind_Control_Required_If_Enabled(this.cboAddressCountry, "Select Country of Residentail Address.");
                  
                clsISUtilities.DataBind_DataView_To_TextBox(txtPostAddrUnitNumber, "EMPLOYEE_POST_UNIT_NUMBER", false, "", false);
                clsISUtilities.DataBind_DataView_To_TextBox(txtPostAddrComplex, "EMPLOYEE_POST_COMPLEX", false, "", false);
                clsISUtilities.DataBind_DataView_To_TextBox(txtPostAddrStreetNumber, "EMPLOYEE_POST_STREET_NUMBER", false, "", false);

                clsISUtilities.DataBind_DataView_To_TextBox(this.txtPostStreetName, "EMPLOYEE_POST_STREET_NAME", false, "", false);
                clsISUtilities.DataBind_Special_Field(this.txtPostStreetName, 3);

                clsISUtilities.DataBind_DataView_To_TextBox(this.txtPostSuburb, "EMPLOYEE_POST_SUBURB", false, "", true);
                clsISUtilities.DataBind_Special_Field(this.txtPostSuburb, 3);
                clsISUtilities.DataBind_DataView_To_TextBox(this.txtPostCity, "EMPLOYEE_POST_CITY", false, "", true);
                clsISUtilities.DataBind_Special_Field(this.txtPostCity, 3);

                clsISUtilities.DataBind_DataView_To_TextBox(this.txtPostAddrCode, "EMPLOYEE_POST_CODE", false, "", true);
                clsISUtilities.DataBind_Special_Field(this.txtPostAddrCode, 3);

                clsISUtilities.DataBind_DataView_To_ComboBox(this.cboPostCountry, "EMPLOYEE_POST_COUNTRY_CODE2", false, "", false);
                clsISUtilities.DataBind_Control_Required_If_Enabled(this.cboPostCountry, "Select Country of Postal Address.");
                
                clsISUtilities.DataBind_DataView_To_Numeric_TextBox_Min_Length(txtTelHome, "EMPLOYEE_TEL_HOME", 0, 10, false, "Enter Home Tel. Number.", true, 0, true);

                clsISUtilities.DataBind_DataView_To_Numeric_TextBox_Min_Length(txtTelWork, "EMPLOYEE_TEL_WORK", 0, 10, false, "Enter Work Tel. Number.", false, 0, true);
                clsISUtilities.DataBind_Control_Required_If_Enabled(txtTelWork, "Enter Work Telephone Number.");

                clsISUtilities.DataBind_DataView_To_Numeric_TextBox_Min_Length(txtTelCell, "EMPLOYEE_TEL_CELL", 0, 10, false, "Enter Cell Number.", true, 0, true);

                clsISUtilities.DataBind_DataView_To_TextBox(txtEmail, "EMPLOYEE_EMAIL", false, "", true);

                //TabPage 1
                clsISUtilities.DataBind_DataView_To_TextBox(txtTaxDirectiveNo1, "TAX_DIRECTIVE_NO1", false, "", false);
                clsISUtilities.DataBind_Control_Required_If_Enabled(txtTaxDirectiveNo1, "Enter Tax Directive Number.");

                clsISUtilities.DataBind_DataView_To_TextBox(txtTaxDirectiveNo2, "TAX_DIRECTIVE_NO2", false, "", false);
                clsISUtilities.DataBind_DataView_To_TextBox(txtTaxDirectiveNo3, "TAX_DIRECTIVE_NO3", false, "", false);

                clsISUtilities.DataBind_DataView_To_Numeric_TextBox(txtTaxDirectivePercentAmount, "TAX_DIRECTIVE_PERCENTAGE", 2, false, "", false, 99.99, true);
                clsISUtilities.DataBind_Control_Required_If_Enabled(txtTaxDirectivePercentAmount, "Enter Tax Directive Percentage.");

                clsISUtilities.DataBind_DataView_To_Numeric_TextBox(txtTaxRefNo, "EMPLOYEE_TAX_NO", 0, false, "", true, 0, true);
                clsISUtilities.DataBind_DataView_Field_EFiling(txtTaxRefNo);

                //TabPage 2
                clsISUtilities.DataBind_DataView_To_ComboBox(cboBankAccountType, "BANK_ACCOUNT_TYPE_NO", true, "Select Bank Account Type.", true);

                clsISUtilities.DataBind_DataView_To_Numeric_TextBox(txtBranchCode, "BRANCH_CODE", 0, false, "", false, 0, true);
                clsISUtilities.DataBind_Control_Required_If_Enabled(txtBranchCode, "Enter Branch Code.");

                clsISUtilities.DataBind_DataView_To_TextBox(this.txtBranchDesc, "BRANCH_DESC", false, "", false);
                clsISUtilities.DataBind_Control_Required_If_Enabled(txtBranchDesc, "Enter Branch Description.");

                clsISUtilities.DataBind_DataView_To_Numeric_TextBox(txtAccountNo, "ACCOUNT_NO", 0, false, "", false, 0, true);
                clsISUtilities.DataBind_Control_Required_If_Enabled(txtAccountNo, "Enter Account Number.");

                clsISUtilities.DataBind_DataView_To_TextBox(txtAccountName, "ACCOUNT_NAME", false, "", false);
                clsISUtilities.DataBind_Control_Required_If_Enabled(txtAccountName, "Enter Account Name.");

                clsISUtilities.DataBind_DataView_To_ComboBox(cboBankAccountRelationship, "BANK_ACCOUNT_RELATIONSHIP_TYPE_NO", false, "", false);
                clsISUtilities.DataBind_Control_Required_If_Enabled(cboBankAccountRelationship, "Select Account Relationship.");

                clsISUtilities.DataBind_DataView_To_ComboBox(cboEarningNoCheques, "EMPLOYEE_NUMBER_CHEQUES", false, "", false);
                clsISUtilities.DataBind_Control_Required_If_Enabled(cboEarningNoCheques, "Select Number of Payments.");

                clsISUtilities.DataBind_DataView_To_TextBox(txtEmployeeCodeOther, "EMPLOYEE_3RD_PARTY_CODE", false, "", true);

                clsISUtilities.DataBind_DataView_To_TextBox(this.txtEmployeePin, "EMPLOYEE_PIN", false, "", true);
                clsISUtilities.DataBind_DataView_To_TextBox(this.txtRFIDCardNo, "EMPLOYEE_RFID_CARD_NO", false, "", true);

                //TabPage 3
                clsISUtilities.NotDataBound_ComboBox(this.cboEarningDay, "Select Earning Day Of Month.");
                clsISUtilities.NotDataBound_Numeric_TextBox(this.txtEarningPeriodAmt, "Enter Earning Value.", 4, 0);

                //TabPage 4
                clsISUtilities.NotDataBound_ComboBox(this.cboDeductDay, "Select Deduction Day Of Month.");
                clsISUtilities.NotDataBound_Numeric_TextBox(this.txtDeductValue, "Enter Deduction Value.", 2, 0);

                //Set In Form Load
                //txtDeductMinValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
                //txtDeductMaxValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);

                clsISUtilities.DataBind_DataView_To_Numeric_TextBox(txtNumberMedicalDependents, "NUMBER_MEDICAL_AID_DEPENDENTS", 0, false, "", false, 0, false);
                clsISUtilities.DataBind_Control_Required_If_Enabled(txtNumberMedicalDependents, "Enter Number of Medical Aid Dependents.");
            }
            else
            {
                clsISUtilities.Re_DataBind_DataView(pvtEmployeeDataView);
            }

            this.grbEmployeeLock.Visible = false;

            this.pvtblnEmployeeDataGridViewLoaded = false;
            int intEmployeeSelectedRow = 0;

            this.Clear_DataGridView(this.dgvEmployeeDataGridView);

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
                                                      intRow.ToString(),
                                                      Convert.ToInt32(pvtEmployeeDataView[intRow]["EMPLOYEE_NO"]).ToString("000000"));


                if (pvtEmployeeFingerTemplateDataView.Count == 0)
                {
                    this.dgvEmployeeDataGridView[1, dgvEmployeeDataGridView.Rows.Count - 1].Style = this.NoTemplateDataGridViewCellStyle;
                }
                else
                {
                    this.dgvEmployeeDataGridView[1, dgvEmployeeDataGridView.Rows.Count - 1].Style = this.HasTemplateDataGridViewCellStyle;
                }


                if (pvtEmployeeDataView[intRow]["EMPLOYEE_ENDDATE"] != System.DBNull.Value)
                {
                    this.dgvEmployeeDataGridView[0,this.dgvEmployeeDataGridView.Rows.Count - 1].Style = ReadOnlyDataGridViewCellStyle;
                }
                else
                {
                    if (pvtEmployeeDataView[intRow]["LEAVE_CONFIG_ERROR_IND"].ToString() == "Y"
                        | Convert.ToInt32(pvtEmployeeDataView[intRow]["LEAVE_PAY_CATEGORY_NO"]) == 0
                        | Convert.ToInt32(pvtEmployeeDataView[intRow]["LEAVE_PAID_ACCUMULATOR_IND"]) == 0)
                    {
                        this.dgvEmployeeDataGridView[0,this.dgvEmployeeDataGridView.Rows.Count - 1].Style = ErrorDataGridViewCellStyle;
                    }
                    else
                    {
                        if (pvtEmployeeDataView[intRow]["EMPLOYEE_TAKEON_IND"].ToString() != "Y")
                        {
                            this.dgvEmployeeDataGridView[0,this.dgvEmployeeDataGridView.Rows.Count - 1].Style = NotActiveDataGridViewCellStyle;
                        }
                        else
                        {
                            if (pvtEmployeeDataView[intRow]["PAYROLL_LINK"] != System.DBNull.Value)
                            {
                                this.dgvEmployeeDataGridView[0,this.dgvEmployeeDataGridView.Rows.Count - 1].Style = LockedPayrollRunDataGridViewCellStyle;

                                this.grbEmployeeLock.Visible = true;
                            }
                        }
                    }
                }
            }

            this.pvtblnEmployeeDataGridViewLoaded = true;
                    
            if (dgvEmployeeDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, intEmployeeSelectedRow);
                //this.dgvEmployeeDataGridView.Refresh();
            }
            else
            {
                this.cboEtiStartMonth.SelectedIndex = -1;
                this.btnUpdate.Enabled = false;
                this.btnDelete.Enabled = false;
            }
        }

        private void btnPayCategoryAdd_Click(object sender, EventArgs e)
        {
            if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
            {
                this.txtRate.Enabled = true;

                if (pvtstrPayrollType == "W")
                {
                    this.rbnDefaultYes.Enabled = true;
                    this.rbnDefaultNo.Enabled = true;
                }
                else
                {
                    this.rbnDefaultYes.Enabled = false;
                    this.rbnDefaultNo.Enabled = false;
                }

                DataView EmployeePayCategoryDataView = new DataView(pvtDataSet.Tables["EmployeePayCategory"],
                    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND EMPLOYEE_NO = " + this.pvtintEmployeeNo + " AND PAY_CATEGORY_NO = " + this.dgvPayCategoryDataGridView[pvtintPayCategoryNoCol, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Value.ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                    "",
                     DataViewRowState.CurrentRows);

                if (EmployeePayCategoryDataView.Count == 0)
                {
                    pvtintChosenPayCategoryDataGridViewRowIndex = -1;

                    this.pvtblnPayCategoryDataGridViewLoaded = false;
                    this.pvtblnChosenPayCategoryDataGridViewLoaded = false;

                    DataRowView drvDataRowView = EmployeePayCategoryDataView.AddNew();

                    drvDataRowView.BeginEdit();

                    //Set Key for Find
                    drvDataRowView["COMPANY_NO"] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    drvDataRowView["EMPLOYEE_NO"] = this.pvtintEmployeeNo;
                    drvDataRowView["PAY_CATEGORY_NO"] = Convert.ToInt32(dgvPayCategoryDataGridView[pvtintPayCategoryNoCol, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Value);
                    drvDataRowView["PAY_CATEGORY_TYPE"] = pvtstrPayrollType;
                    drvDataRowView["HOURLY_RATE"] = 0;
                    drvDataRowView["LEAVE_DAY_RATE_DECIMAL"] = 0;

                    bool blnValue = false;

                    if (this.dgvChosenPayCategoryDataGridView.Rows.Count == 0)
                    {
                        blnValue = true;
                        drvDataRowView["DEFAULT_IND"] = 'Y';

                        pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_PAY_CATEGORY_NO"] = Convert.ToInt32(dgvPayCategoryDataGridView[pvtintPayCategoryNoCol, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Value);
                    }
                    else
                    {
                        drvDataRowView["DEFAULT_IND"] = 'N';
                    }

                    drvDataRowView.EndEdit();

                    this.dgvChosenPayCategoryDataGridView.Rows.Add(this.dgvPayCategoryDataGridView[pvtintPayCategoryDescCol, this.Get_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView)].Value.ToString(),
                                                                 "0",
                                                                 blnValue,
                                                                 dgvPayCategoryDataGridView[pvtintPayCategoryNoCol, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Value.ToString());

                    //Remove Pay Category
                    DataGridViewRow myDataGridViewRow = this.dgvPayCategoryDataGridView.Rows[this.dgvPayCategoryDataGridView.CurrentRow.Index];
                    this.dgvPayCategoryDataGridView.Rows.Remove(myDataGridViewRow);

                    this.pvtblnPayCategoryDataGridViewLoaded = true;
                    this.pvtblnChosenPayCategoryDataGridViewLoaded = true;

                    //Fire Off New Row
                    this.Set_DataGridView_SelectedRowIndex(this.dgvChosenPayCategoryDataGridView, this.dgvChosenPayCategoryDataGridView.Rows.Count - 1);

                    if (this.dgvChosenPayCategoryDataGridView.Rows.Count == 1)
                    {
                        clsISUtilities.NotDataBound_Control_Paint_Remove(grbSelectedCostCentre);

                        if (this.cboLeaveShift.SelectedIndex != -1)
                        {
                            cboLeaveShift_SelectedIndexChanged(sender, e);
                        }
                    }
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
                pvtintChosenPayCategoryDataGridViewRowIndex = -1;

                this.pvtblnPayCategoryDataGridViewLoaded = false;
                this.pvtblnChosenPayCategoryDataGridViewLoaded = false;

                DataView EmployeePayCategoryDataView = new DataView(pvtDataSet.Tables["EmployeePayCategory"],
                    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND EMPLOYEE_NO = " + this.pvtintEmployeeNo + " AND PAY_CATEGORY_NO = " + this.dgvChosenPayCategoryDataGridView[pvtintPayCategoryNoCol, this.Get_DataGridView_SelectedRowIndex(this.dgvChosenPayCategoryDataGridView)].Value.ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                    "",
                    DataViewRowState.CurrentRows);

                this.dgvPayCategoryDataGridView.Rows.Add(this.dgvChosenPayCategoryDataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvChosenPayCategoryDataGridView)].Value.ToString(),
                                                            "",
                                                            "",
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
                else
                {
                    this.rbnDefaultYes.Checked = true;
                    this.txtRate.Text = "";
                    this.txtSelectedPayCategoryDesc.Text = "";

                    this.txtRate.Enabled = false;
                    this.rbnDefaultYes.Enabled = false;
                    this.rbnDefaultNo.Enabled = false;

                    clsISUtilities.NotDataBound_Control_Paint(grbSelectedCostCentre, "Select a Cost Centre.");
                }

                if (pvtstrPayrollType == "S"
                    & this.btnSave.Enabled == true)
                {
                    this.btnPayCategoryRemove.Enabled = false;
                    this.btnPayCategoryAdd.Enabled = true;
                }
            }
        }


        private void Load_Company_Leave()
        {
            pvtLeaveLinkDataView = null;
            pvtLeaveLinkDataView = new DataView(pvtDataSet.Tables["LeaveLink"],
                pvtstrFilter,
                "",
                DataViewRowState.CurrentRows);

            this.cboLeaveShift.Items.Clear();

            for (int intIndex = 0; intIndex < pvtLeaveLinkDataView.Count; intIndex++)
            {
                this.cboLeaveShift.Items.Add(pvtLeaveLinkDataView[intIndex]["LEAVE_SHIFT_DESC"].ToString());
            }
        }

        private void Load_Deduction_Earning_Percentage_Rows()
        {
            bool blnEarningChecked = false;

            pvtDeductionEarningPercentageDataView = null;
            pvtDeductionEarningPercentageDataView = new DataView(pvtDataSet.Tables["DeductionEarningPercentage"],
                pvtstrFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND DEDUCTION_NO = " + pvtintDeductionAccountNo.ToString() + " AND DEDUCTION_SUB_ACCOUNT_NO = " + pvtintDeductionSubAccountNo.ToString(),
                "EARNING_NO",
                DataViewRowState.CurrentRows);

            if (this.btnSave.Enabled == true)
            {
                if (pvtintDeductionAccountNo == 1
                | pvtintDeductionAccountNo == 2)
                {
                    this.dgvChosenDeductionEarningLinkDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    this.dgvChosenDeductionEarningLinkDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
                }
                else
                {
                    this.dgvChosenDeductionEarningLinkDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
                    this.dgvChosenDeductionEarningLinkDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;
                }
            }

            this.Clear_DataGridView(this.dgvChosenDeductionEarningLinkDataGridView);

            this.pvtblnChosenDeductionEarningLinkDataGridViewLoaded = false;

            bool blnValue = false;
            int intFindRow = -1;

            for (int intRow = 0; intRow < pvtEmployeeEarningDataView.Count; intRow++)
            {
                blnValue = false;

                //Tax / UIF
                if (pvtintDeductionAccountNo == 1
                    | pvtintDeductionAccountNo == 2)
                {
                    //Commission
                    if (pvtEmployeeEarningDataView[intRow]["IRP5_CODE"].ToString() == "3606"
                        & Convert.ToInt32(pvtEmployeeEarningDataView[intRow]["EARNING_NO"]) == 2)
                    {
                    }
                    else
                    {
                        blnValue = true;
                        blnEarningChecked = true;
                    }
                }
                else
                {
                    intFindRow = pvtDeductionEarningPercentageDataView.Find(pvtEmployeeEarningDataView[intRow]["EARNING_NO"].ToString());

                    if (intFindRow > -1)
                    {
                        blnValue = true;
                        blnEarningChecked = true;
                    }
                }

                DataView myEarningDataView = new DataView(pvtDataSet.Tables["Earning"],
                        pvtstrFilter + " AND EARNING_NO = " + pvtEmployeeEarningDataView[intRow]["EARNING_NO"].ToString(),
                        "",
                        DataViewRowState.CurrentRows);

                if (myEarningDataView.Count > 0)
                {
                    this.dgvChosenDeductionEarningLinkDataGridView.Rows.Add(myEarningDataView[0]["EARNING_DESC"].ToString(),
                                                                            blnValue,
                                                                            pvtEmployeeEarningDataView[intRow]["EARNING_NO"].ToString());
                }
            }

            this.pvtblnChosenDeductionEarningLinkDataGridViewLoaded = true;

            if (blnEarningChecked == false)
            {
                clsISUtilities.NotDataBound_Control_Paint(lblPercentEarnings, "Select 1 or more Earnings.");
            }

            if (this.dgvChosenDeductionEarningLinkDataGridView.Rows.Count > 0)
            {
                dgvChosenDeductionEarningLinkDataGridView.CurrentCell = dgvChosenDeductionEarningLinkDataGridView[1, 0];
            }
        }

        private void Load_Current_Employee_Pay_Category()
        {
            //Load Pay Category
            this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
            this.Clear_DataGridView(this.dgvChosenPayCategoryDataGridView);

            this.txtLeaveDayRate.Text = "";

            bool blnDefault = false;
            bool blnValue = false;

            pvtintChosenPayCategoryDataGridViewRowIndex = -1;

            this.pvtblnPayCategoryDataGridViewLoaded = false;
            this.pvtblnChosenPayCategoryDataGridViewLoaded = false;

            for (int intIndex = 0; intIndex < pvtPayCategoryDataView.Count; intIndex++)
            {
                DataView EmployeePayCategoryDataView = new DataView(pvtDataSet.Tables["EmployeePayCategory"],
                    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND PAY_CATEGORY_NO = " + pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_NO"].ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_TYPE"].ToString() + "'",
                    "",
                    DataViewRowState.CurrentRows);

                if (EmployeePayCategoryDataView.Count == 0)
                {
                    this.dgvPayCategoryDataGridView.Rows.Add(pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_DESC"].ToString(),
                                                             "",
                                                             "",
                                                             pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_NO"].ToString());
                }
                else
                {
                    if (EmployeePayCategoryDataView[0]["DEFAULT_IND"].ToString() == "Y")
                    {
                        blnDefault = true;
                        blnValue = true;

                        this.txtLeaveDayRate.Text = Convert.ToDouble(EmployeePayCategoryDataView[0]["LEAVE_DAY_RATE_DECIMAL"]).ToString("######0.00");
                    }
                    else
                    {
                        blnValue = false;
                    }

                    this.dgvChosenPayCategoryDataGridView.Rows.Add(pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_DESC"].ToString(),
                                                                  Convert.ToDouble(EmployeePayCategoryDataView[0]["HOURLY_RATE"]).ToString("######0.00"),
                                                                  blnValue,
                                                                  pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_NO"].ToString());
                }
            }

            this.pvtblnPayCategoryDataGridViewLoaded = true;
            this.pvtblnChosenPayCategoryDataGridViewLoaded = true;

            if (this.dgvChosenPayCategoryDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvChosenPayCategoryDataGridView, 0);

                if (blnDefault == false)
                {
                    this.tabEmployee.SelectedIndex = 2;

                    CustomMessageBox.Show("There is NO Default Selected Cost Centre for This Employee.\nYou need to Fix this Error.",
                         this.Text,
                         MessageBoxButtons.OK,
                         MessageBoxIcon.Error);

                }
            }
        }

        private void tabEmployee_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                pvtobjSender = sender;

                TabControl myTabControl = (TabControl)sender;

                if (myTabControl.SelectedIndex > 2)
                {
                    if (pvtEmployeeDeductionDataView == null)
                    {
                        pvtEmployeeEarningDataView = null;
                        pvtEmployeeEarningDataView = new DataView(pvtDataSet.Tables["EmployeeEarning"],
                             pvtstrFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo,
                            "EARNING_NO",
                            DataViewRowState.CurrentRows);

                        if (pvtEmployeeEarningDataView.Count == 0
                            & pvtintEmployeeNo > -1)
                        {
                            object[] objParm = new object[3];
                            objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                            objParm[1] = pvtintEmployeeNo;
                            objParm[2] = pvtstrPayrollType;

                            pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Employee_EarningDeductionLeaveLoans", objParm);

                            pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                            pvtDataSet.Merge(pvtTempDataSet);
                        }

                        Load_Current_Employee_Earnings();

                        Load_Current_Employee_Deductions();

                        Load_Current_Employee_Leave();
                    }
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void Load_Current_Employee_Earnings()
        {
            this.Clear_DataGridView(this.dgvEarningDataGridView);
            this.Clear_DataGridView(this.dgvChosenEarningDataGridView);

            pvtintChosenEarningDataGridViewRowIndex = -1;

            this.pvtblnEarningDataGridViewLoaded = false;
            this.pvtblnChosenEarningDataGridViewLoaded = false;

            int intEmployeeEarningRow = -1;
            double dblValueZero = 0;
            string strValue = "";

            //Loop Through All Possible Earnings
            for (int intIndex = 0; intIndex < pvtEarningDataView.Count; intIndex++)
            {
                if (this.Text.IndexOf("- New", 0) > 0)
                {
                    //Add Default Earning to Employee
                    if (Convert.ToInt32(pvtEarningDataView[intIndex]["EARNING_NO"]) < 10
                        | Convert.ToInt32(pvtEarningDataView[intIndex]["EARNING_NO"]) == 200
                        | Convert.ToInt32(pvtEarningDataView[intIndex]["EARNING_NO"]) == 201)
                    {
                        intEmployeeEarningRow = pvtEmployeeEarningDataView.Find(pvtEarningDataView[intIndex]["EARNING_NO"].ToString());

                        //Make Sure Record is Added only Once
                        if (intEmployeeEarningRow == -1)
                        {
                            DataRowView drvDataRowView = this.pvtEmployeeEarningDataView.AddNew();

                            drvDataRowView.BeginEdit();

                            Add_Employee_Earning_Fields(drvDataRowView, Convert.ToInt32(this.pvtEarningDataView[intIndex]["EARNING_NO"]));

                            if (this.pvtEarningDataView[intIndex]["IRP5_CODE"].ToString() != "")
                            {
                                drvDataRowView["IRP5_CODE"] = Convert.ToInt32(this.pvtEarningDataView[intIndex]["IRP5_CODE"]);
                            }

                            drvDataRowView["EARNING_TYPE_IND"] = "S";
                            drvDataRowView["EARNING_PERIOD_IND"] = "E";
                            drvDataRowView["EARNING_DAY_VALUE"] = 0;

                            drvDataRowView.EndEdit();
                        }
                    }
                }

                intEmployeeEarningRow = pvtEmployeeEarningDataView.Find(pvtEarningDataView[intIndex]["EARNING_NO"]);

                if (intEmployeeEarningRow > -1)
                {
                    if (this.pvtEmployeeEarningDataView[intEmployeeEarningRow]["EARNING_TYPE_IND"].ToString() == "X")
                    {
                        strValue = Convert.ToDouble(this.pvtEmployeeEarningDataView[intEmployeeEarningRow]["AMOUNT"]).ToString("######0.0000");
                    }
                    else
                    {
                        strValue = Convert.ToDouble(this.pvtEmployeeEarningDataView[intEmployeeEarningRow]["AMOUNT"]).ToString("######0.00");
                    }

                    this.dgvChosenEarningDataGridView.Rows.Add("",
                                                               pvtEmployeeEarningDataView[intEmployeeEarningRow]["IRP5_CODE"].ToString(),
                                                               this.pvtEarningDataView[intIndex]["EARNING_DESC"].ToString(),
                                                               strValue,
                                                               this.pvtEmployeeEarningDataView[intEmployeeEarningRow]["EARNING_NO"].ToString());

                    //Default Earning
                    if (Convert.ToInt32(pvtEarningDataView[intIndex]["EARNING_NO"]) < 10
                       | Convert.ToInt32(pvtEarningDataView[intIndex]["EARNING_NO"]) == 200
                       | Convert.ToInt32(pvtEarningDataView[intIndex]["EARNING_NO"]) == 201)
                    {
                        this.dgvChosenEarningDataGridView[0,this.dgvChosenEarningDataGridView.Rows.Count - 1].Style = ReadOnlyDataGridViewCellStyle;
                    }
                }
                else
                {
                    this.dgvEarningDataGridView.Rows.Add("",
                                                         pvtEarningDataView[intIndex]["IRP5_CODE"].ToString(),
                                                         pvtEarningDataView[intIndex]["EARNING_DESC"].ToString(),
                                                         dblValueZero.ToString("0.00"),
                                                         pvtEarningDataView[intIndex]["EARNING_NO"].ToString());


                    //Default Earning
                    if (Convert.ToInt32(pvtEarningDataView[intIndex]["EARNING_NO"]) < 10
                       | Convert.ToInt32(pvtEarningDataView[intIndex]["EARNING_NO"]) == 200
                       | Convert.ToInt32(pvtEarningDataView[intIndex]["EARNING_NO"]) == 201)
                    {
                        this.dgvEarningDataGridView[0,this.dgvEarningDataGridView.Rows.Count - 1].Style = ReadOnlyDataGridViewCellStyle;
                    }
                }
            }

            this.pvtblnEarningDataGridViewLoaded = true;
            this.pvtblnChosenEarningDataGridViewLoaded = true;

            if (this.dgvEarningDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvEarningDataGridView, 0);
            }

            if (this.dgvChosenEarningDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvChosenEarningDataGridView, 0);
            }
        }

        private void Load_Current_Employee_Deductions()
        {
            pvtEmployeeDeductionDataView = null;
            pvtEmployeeDeductionDataView = new DataView(pvtDataSet.Tables["EmployeeDeduction"],
                pvtstrFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo,
                "DEDUCTION_NO,DEDUCTION_SUB_ACCOUNT_NO",
                DataViewRowState.CurrentRows);

#if(DEBUG)

            System.Diagnostics.Debug.WriteLine("Load_Current_Employee_Deductions pvtintEmployeeNo = " + pvtintEmployeeNo.ToString());
#endif

            int intChosenDeductionRow = 0;

            this.lblPercent.Visible = false;
            this.btnResetDefaultDeduction.Visible = false;
            this.lblResetDefault.Visible = false;

            this.Clear_DataGridView(this.dgvDeductionDataGridView);
            this.Clear_DataGridView(this.dgvChosenDeductionDataGridView);
            this.Clear_DataGridView(this.dgvChosenDeductionEarningLinkDataGridView);
            this.Clear_DataGridView(this.dgvLoanDataGridView);
            this.Clear_DataGridView(this.dgvEmployeeLoanDataGridView);

            pvtintChosenDeductionDataGridViewRowIndex = -1;
            pvtintLoanDataGridViewRowIndex = -1;

            this.pvtblnDeductionDataGridViewLoaded = false;
            this.pvtblnChosenDeductionDataGridViewLoaded = false;
            this.pvtblnLoanDataGridViewLoaded = false;
            this.pvtblnEmployeeLoanDataGridViewLoaded = false;

            int intIndex = 0;
            double dblValue = 0;

            string strDeductionDesc = "";

            for (int intRow = 0; intRow < this.pvtDeductionDataView.Count; intRow++)
            {
                if (this.Text.IndexOf("- New", 0) > 0)
                {
                    //Tax / UIF / Loan
                    if (Convert.ToDouble(this.pvtDeductionDataView[intRow]["DEDUCTION_NO"]) == 1
                        | Convert.ToDouble(this.pvtDeductionDataView[intRow]["DEDUCTION_NO"]) == 2
                        | Convert.ToDouble(this.pvtDeductionDataView[intRow]["DEDUCTION_NO"]) == 6)
                    {
                        DataRowView drvDataRowView = this.pvtEmployeeDeductionDataView.AddNew();

                        drvDataRowView.BeginEdit();

                        //Set Key for Find
                        drvDataRowView["COMPANY_NO"] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo"));
                        drvDataRowView["PAY_CATEGORY_TYPE"] = pvtstrPayrollType;
                        drvDataRowView["EMPLOYEE_NO"] = this.pvtintEmployeeNo;
                        drvDataRowView["DEDUCTION_NO"] = Convert.ToInt32(this.pvtDeductionDataView[intRow]["DEDUCTION_NO"]);
                        drvDataRowView["DEDUCTION_SUB_ACCOUNT_NO"] = Convert.ToInt32(this.pvtDeductionDataView[intRow]["DEDUCTION_SUB_ACCOUNT_NO"]);
                        drvDataRowView["TIE_BREAKER"] = 1;
                        drvDataRowView["DEDUCTION_TYPE_IND"] = this.pvtDeductionDataView[intRow]["DEDUCTION_TYPE_IND"].ToString();
                        drvDataRowView["DEDUCTION_VALUE"] = Convert.ToDouble(this.pvtDeductionDataView[intRow]["DEDUCTION_VALUE"]);
                        drvDataRowView["DEDUCTION_MIN_VALUE"] = Convert.ToDouble(this.pvtDeductionDataView[intRow]["DEDUCTION_MIN_VALUE"]);
                        drvDataRowView["DEDUCTION_MAX_VALUE"] = Convert.ToDouble(this.pvtDeductionDataView[intRow]["DEDUCTION_MAX_VALUE"]);
                        drvDataRowView["DEDUCTION_PERIOD_IND"] = "E";
                        drvDataRowView["DEDUCTION_DAY_VALUE"] = Convert.ToDouble(this.pvtDeductionDataView[intRow]["DEDUCTION_DAY_VALUE"]);
                        drvDataRowView["DEDUCTION_LOAN_TYPE_IND"] = this.pvtDeductionDataView[intRow]["DEDUCTION_LOAN_TYPE_IND"].ToString();
                        drvDataRowView["DEDUCTION_SUB_ACCOUNT_COUNT"] = Convert.ToInt32(this.pvtDeductionDataView[intRow]["DEDUCTION_SUB_ACCOUNT_COUNT"]);
                        drvDataRowView["LOAN_OUTSTANDING"] = 0;

                        drvDataRowView.EndEdit();
                    }
                }

                pvtobjDeductionFind[0] = Convert.ToDouble(this.pvtDeductionDataView[intRow]["DEDUCTION_NO"]);
                pvtobjDeductionFind[1] = Convert.ToDouble(this.pvtDeductionDataView[intRow]["DEDUCTION_SUB_ACCOUNT_NO"]);

                intIndex = pvtEmployeeDeductionDataView.Find(pvtobjDeductionFind);

                if (intIndex > -1)
                {
                    if (pvtEmployeeDeductionDataView[intIndex]["DEDUCTION_SUB_ACCOUNT_COUNT"] == System.DBNull.Value)
                    {
                        strDeductionDesc = this.pvtDeductionDataView[intRow]["DEDUCTION_DESC"].ToString();
                    }
                    else
                    {
                        if (Convert.ToInt32(pvtEmployeeDeductionDataView[intIndex]["DEDUCTION_SUB_ACCOUNT_COUNT"]) > 1)
                        {
                            strDeductionDesc = this.pvtDeductionDataView[intRow]["DEDUCTION_DESC"].ToString() + " (" + this.pvtDeductionDataView[intRow]["DEDUCTION_SUB_ACCOUNT_NO"].ToString() + ")";
                        }
                        else
                        {
                            strDeductionDesc = this.pvtDeductionDataView[intRow]["DEDUCTION_DESC"].ToString();
                        }
                    }

                    this.dgvChosenDeductionDataGridView.Rows.Add("",
                                                                 this.pvtDeductionDataView[intRow]["IRP5_CODE"].ToString(),
                                                                 strDeductionDesc,
                                                                 Convert.ToDouble(this.pvtEmployeeDeductionDataView[intIndex]["DEDUCTION_VALUE"]).ToString("#######0.00"),
                                                                 this.pvtEmployeeDeductionDataView[intIndex]["DEDUCTION_NO"].ToString(),
                                                                 this.pvtEmployeeDeductionDataView[intIndex]["DEDUCTION_SUB_ACCOUNT_NO"].ToString());

                    //Tax / UIF
                    if (Convert.ToInt32(pvtEmployeeDeductionDataView[intIndex]["DEDUCTION_NO"]) == 1
                        | Convert.ToInt32(pvtEmployeeDeductionDataView[intIndex]["DEDUCTION_NO"]) == 2)
                    {
                        //this.dgvChosenDeductionDataGridView.Rows[this.dgvChosenDeductionDataGridView.Rows.Count - 1].HeaderCell.Style = ReadOnlyDataGridViewCellStyle;
                    }

                    if (Convert.ToInt32(pvtEmployeeDeductionDataView[intIndex]["DEDUCTION_NO"]) == pvtintDeductionAccountNo
                        & Convert.ToInt32(pvtEmployeeDeductionDataView[intIndex]["DEDUCTION_SUB_ACCOUNT_NO"]) == pvtintDeductionSubAccountNo)
                    {
                        intChosenDeductionRow = this.dgvChosenDeductionDataGridView.Rows.Count - 1;
                    }

                    if (pvtEmployeeDeductionDataView[intIndex]["DEDUCTION_LOAN_TYPE_IND"].ToString() == "Y")
                    {
                        dblValue = 0;

                        if (this.pvtEmployeeDeductionDataView[intIndex]["LOAN_OUTSTANDING"] != System.DBNull.Value)
                        {
                            dblValue = Convert.ToDouble(this.pvtEmployeeDeductionDataView[intIndex]["LOAN_OUTSTANDING"]);
                        }

                        this.dgvLoanDataGridView.Rows.Add("",
                                                          strDeductionDesc,
                                                          dblValue.ToString("######0.00"),
                                                          pvtEmployeeDeductionDataView[intIndex]["DEDUCTION_NO"].ToString(),
                                                          pvtEmployeeDeductionDataView[intIndex]["DEDUCTION_SUB_ACCOUNT_NO"].ToString());

                        DataView myDeductionLoanTypeTestDataView = new DataView(pvtDataSet.Tables["EmployeeLoan"],
                            pvtstrFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND DEDUCTION_NO = " + pvtEmployeeDeductionDataView[intIndex]["DEDUCTION_NO"].ToString() + " AND DEDUCTION_SUB_ACCOUNT_NO = " + pvtEmployeeDeductionDataView[intIndex]["DEDUCTION_SUB_ACCOUNT_NO"].ToString(),
                            "",
                            DataViewRowState.CurrentRows);

                        if (myDeductionLoanTypeTestDataView.Count > 0)
                        {
                            this.dgvChosenDeductionDataGridView[0,this.dgvChosenDeductionDataGridView.Rows.Count - 1].Style = this.LoanTypeDataGridViewCellStyle;

                            this.dgvLoanDataGridView[0,this.dgvLoanDataGridView.Rows.Count - 1].Style = this.LoanTypeDataGridViewCellStyle;
                        }
                        else
                        {
                            this.dgvChosenDeductionDataGridView[0,this.dgvChosenDeductionDataGridView.Rows.Count - 1].Style = this.NotActiveDataGridViewCellStyle;

                            this.dgvLoanDataGridView[0,this.dgvLoanDataGridView.Rows.Count - 1].Style = this.NotActiveDataGridViewCellStyle;
                        }
                    }
                }
                else
                {
                    if (Convert.ToInt32(pvtDeductionDataView[intRow]["DEDUCTION_SUB_ACCOUNT_COUNT"]) > 1)
                    {
                        strDeductionDesc = this.pvtDeductionDataView[intRow]["DEDUCTION_DESC"].ToString() + " (" + this.pvtDeductionDataView[intRow]["DEDUCTION_SUB_ACCOUNT_NO"].ToString() + ")";
                    }
                    else
                    {
                        strDeductionDesc = this.pvtDeductionDataView[intRow]["DEDUCTION_DESC"].ToString();
                    }


                    this.dgvDeductionDataGridView.Rows.Add("",
                                                           this.pvtDeductionDataView[intRow]["IRP5_CODE"].ToString(),
                                                           strDeductionDesc,
                                                           Convert.ToDouble(this.pvtDeductionDataView[intRow]["DEDUCTION_VALUE"]),
                                                           pvtDeductionDataView[intRow]["DEDUCTION_NO"].ToString(),
                                                           pvtDeductionDataView[intRow]["DEDUCTION_SUB_ACCOUNT_NO"].ToString());

                    if (pvtDeductionDataView[intRow]["DEDUCTION_LOAN_TYPE_IND"].ToString() == "Y")
                    {
                        this.dgvDeductionDataGridView[0,this.dgvDeductionDataGridView.Rows.Count - 1].Style = this.LoanTypeDataGridViewCellStyle;
                    }
                }
            }

            this.pvtblnDeductionDataGridViewLoaded = true;
            this.pvtblnChosenDeductionDataGridViewLoaded = true;

            pvtblnLoanDataGridViewLoaded = true;
            pvtblnEmployeeLoanDataGridViewLoaded = true;

            if (this.dgvLoanDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvLoanDataGridView, 0);
            }

            if (this.dgvDeductionDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvDeductionDataGridView, 0);
            }

            if (this.dgvChosenDeductionDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvChosenDeductionDataGridView, intChosenDeductionRow);
            }
        }

        private void Load_Current_Employee_Leave()
        {
            bool blnNormal = true;
            double dblLeaveAccum = 0;
            int intLeaveTypeRow = 0;
            this.lblLeaveTypeDescription.Text = "";

            this.Clear_DataGridView(this.dgvLeaveTypeDataGridView);

            pvtLeaveTypeDataGridViewRowIndex = -1;

            pvtblnLeaveTypeDataGridViewLoaded = false;

            for (int intRow = 0; intRow < pvtLeaveTypeDataView.Count; intRow++)
            {
                dblLeaveAccum = 0;

                blnNormal = false;

                pvtEmployeeLeaveDataView = null;
                pvtEmployeeLeaveDataView = new DataView(pvtDataSet.Tables["EmployeeLeave"],
                    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND EARNING_NO = " + pvtLeaveTypeDataView[intRow]["EARNING_NO"],
                    "",
                    DataViewRowState.CurrentRows);

                if (pvtEmployeeLeaveDataView.Count > 0)
                {
                    for (int intLeaveRow = 0; intLeaveRow < pvtEmployeeLeaveDataView.Count; intLeaveRow++)
                    {
                        if (pvtEmployeeLeaveDataView[intLeaveRow]["LEAVE_PROCESSED_DATE"] != System.DBNull.Value)
                        {
                            dblLeaveAccum += Convert.ToDouble(pvtEmployeeLeaveDataView[intLeaveRow]["LEAVE_ACCUM_DAYS"]);
                            dblLeaveAccum -= Convert.ToDouble(pvtEmployeeLeaveDataView[intLeaveRow]["LEAVE_PAID_DAYS"]);
                        }
                    }

                    blnNormal = true;
                }

                this.dgvLeaveTypeDataGridView.Rows.Add("",
                                                       pvtLeaveTypeDataView[intRow]["EARNING_DESC"].ToString(),
                                                       dblLeaveAccum.ToString("####0.00"),
                                                       pvtLeaveTypeDataView[intRow]["EARNING_NO"].ToString());

                if (blnNormal == true)
                {

                }
                else
                {
                    this.dgvLeaveTypeDataGridView[0,this.dgvLeaveTypeDataGridView.Rows.Count - 1].Style = this.NotActiveDataGridViewCellStyle;

                }

                if (Convert.ToInt32(pvtLeaveTypeDataView[intRow]["EARNING_NO"]) == pvtintLeaveEarningNo)
                {
                    intLeaveTypeRow = intRow;
                }
            }

            pvtblnLeaveTypeDataGridViewLoaded = true;

            if (this.dgvLeaveTypeDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvLeaveTypeDataGridView, intLeaveTypeRow);
            }
            else
            {
                this.lblLeaveAccumTotal.Text = "0.00";
                this.lblLeavePaidTotal.Text = "0.00";
                this.lblLeaveFinalTotal.Text = "0.00";
            }
        }

        private void Add_Employee_Earning_Fields(DataRowView parDataRowView, int parintEarningNo)
        {
            //Set Key for Find
            parDataRowView["COMPANY_NO"] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
            parDataRowView["PAY_CATEGORY_TYPE"] = pvtstrPayrollType;
            parDataRowView["EMPLOYEE_NO"] = this.pvtintEmployeeNo;
            parDataRowView["EARNING_NO"] = parintEarningNo;
            parDataRowView["TIE_BREAKER"] = 1;
            parDataRowView["AMOUNT"] = 0;
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        public void btnNew_Click(object sender, System.EventArgs e)
        {
            try
            {
                //Do Check Before Allowing New to Continue
                if (this.pvtDataSet.Tables["Department"].Rows.Count == 0)
                {
                    CustomMessageBox.Show("Department/s Need to be Captured.",
                           this.Text,
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Information);

                    return;
                }

                if (this.pvtDataSet.Tables["Occupation"].Rows.Count == 0)
                {
                    CustomMessageBox.Show("Occupation/s Need to be Captured.",
                           this.Text,
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Information);

                    return;
                }

                if (pvtPayCategoryDataView.Count == 0)
                {
                    CustomMessageBox.Show("Cost Centre/s Need to be Captured for " + this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView)].Value.ToString() + ".",
                           this.Text,
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Information);

                    return;
                }

                if (pvtLeaveLinkDataView.Count == 0)
                {
                    CustomMessageBox.Show("Normal Leave / Sick Leave Needs to be Captured for " + this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView)].Value.ToString() + ".",
                           this.Text,
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Information);

                    return;
                }

                this.Text = this.Text + " - New";

                pvtintEmployeeNo = -1;

                pvtDataRowView = this.pvtEmployeeDataView.AddNew();

                pvtDataRowView["COMPANY_NO"] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                pvtDataRowView["PAY_CATEGORY_TYPE"] = pvtstrPayrollType;
                pvtDataRowView["EMPLOYEE_NO"] = pvtintEmployeeNo;
                pvtDataRowView["EMPLOYEE_SURNAME"] = "";
                //Forces Record to be First (clsISUtilities.DataViewIndex = 0)
                pvtDataRowView["EMPLOYEE_CODE"] = "";

                //2012-10-24
                pvtDataRowView["LEAVE_PAY_CATEGORY_NO"] = 0;

                pvtDataRowView.EndEdit();

                int intEmployeeNo = Convert.ToInt32(pvtEmployeeDataView[0]["EMPLOYEE_NO"]);

                //Set Index to First Row of View
                clsISUtilities.DataViewIndex = 0;

                Set_Form_For_Edit();

#if (DEBUG)
                ////Used For Testing
                //if (this.Text.EndsWith(" - New") == true)
                //{
                //    if (this.pvtDataSet.Tables["Company"].Rows[0]["GENERATE_EMPLOYEE_NUMBER_IND"].ToString() == "Y")
                //    {
                //        //Already Tested
                //        this.txtCode.Text = "100018";
                //    }
                //    this.txtName.Text = "A";
                //    this.txtSurname.Text = "A";
                //    this.cboNatureOfPerson.SelectedIndex = 0;
                //    this.txtIDNo.Text = "6409085107081";

                //    cboMaritalStatus.SelectedIndex = 0;

                //    if (cboOccupation.Items.Count > 0)
                //    {
                //        cboOccupation.SelectedIndex = 0;
                //    }

                //    if (cboDepartment.Items.Count > 0)
                //    {
                //        cboDepartment.SelectedIndex = 0;
                //    }

                //    cboGender.SelectedIndex = 0;
                //    cboRace.SelectedIndex = 0;

                //    this.txtBirthDate.Text = new DateTime(1964, 9, 8).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString());

                //    this.txtTelWork.Text = "0117816646";
                //    this.cboBankAccountType.SelectedIndex = 0;
                //}

                pvtEmployeeFingerTemplateDataView = null;
                pvtEmployeeFingerTemplateDataView = new DataView(this.pvtDataSet.Tables["EmployeeFingerTemplate"],
                    "COMPANY_NO = " + Convert.ToString(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND EMPLOYEE_NO = -1",
                    "FINGER_NO",
                    DataViewRowState.CurrentRows);

                this.Draw_Current_Employee_Fingers();
#endif
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
                else
                {
                    DataView myDataView = new DataView(pvtDataSet.Tables["Employee"],
                                                     "",
                                                     "EMPLOYEE_NO DESC",
                                                     DataViewRowState.CurrentRows);

                    if (myDataView.Count == 0)
                    {
                        this.txtNewCode.Text = "00001";
                    }
                    else
                    {
                        int intCode = 1;

                        if (Convert.ToInt32(myDataView[0]["EMPLOYEE_NO"]) != -1)
                        {
                            intCode = Convert.ToInt32(myDataView[0]["EMPLOYEE_NO"]) + 1;
                        }

                        this.txtNewCode.Text = intCode.ToString("00000");
                    }

                    this.txtNewCode.Visible = true;
                }
                
                if (pvtstrPayrollType == "S")
                {
                    if (this.pvtDataSet.Tables["Company"].Rows[0]["SALARY_DOUBLE_CHEQUE_BIRTHDAY_IND"].ToString() == "Y")
                    {
                        //13 Cheques
                        this.cboEarningNoCheques.SelectedIndex = 1;
                    }
                    else
                    {
                        this.cboEarningNoCheques.SelectedIndex = 0;
                    }
                }

                this.txtTaxRefNo.Text = "0000000000";

                cboLeaveShift.SelectedIndex = -1;

                this.rbnTaxNormal.Checked = true;
                this.rbnResAddrCompany.Checked = true;
                this.rbnWorkTelCompany.Checked = true;

                this.chkPayslip.Checked = false;
                this.chkEmpNo.Checked = false;

                pvtEmployeeEarningDataView = null;
                pvtEmployeeDeductionDataView = null;
                pvtEmployeeLeaveDataView = null;

                Clear_Pay_Category();

                clsISUtilities.NotDataBound_Control_Paint(grbSelectedCostCentre, "Select a Cost Centre.");

                this.tabEmployee.SelectedIndex = 0;
            }
            else
            {
                //NB These Controls Are Enabled on The Add Button in New
                this.txtRate.Enabled = true;

                if (pvtstrPayrollType == "S")
                {
                    if (this.cboEarningNoCheques.SelectedIndex < 0)
                    {
                        this.cboEarningNoCheques.SelectedIndex = 0;
                    }

                    this.rbnDefaultYes.Enabled = false;
                    this.rbnDefaultNo.Enabled = false;
                }
                else
                {
                    this.rbnDefaultYes.Enabled = true;
                    this.rbnDefaultNo.Enabled = true;
                }

                //Enable Correct Fields For Nature of Person
                if (cboNatureOfPerson.SelectedIndex > -1)
                {
                    if (Convert.ToInt32(this.pvtDataSet.Tables["NaturePerson"].Rows[this.cboNatureOfPerson.SelectedIndex]["NATURE_PERSON_NO"]) == 1
                        | Convert.ToInt32(this.pvtDataSet.Tables["NaturePerson"].Rows[this.cboNatureOfPerson.SelectedIndex]["NATURE_PERSON_NO"]) == 4)
                    {
                        //Has Identity Document
                        this.txtIDNo.Enabled = true;
                    }
                    else
                    {
                        if (Convert.ToInt32(this.pvtDataSet.Tables["NaturePerson"].Rows[this.cboNatureOfPerson.SelectedIndex]["NATURE_PERSON_NO"]) == 2
                            | Convert.ToInt32(this.pvtDataSet.Tables["NaturePerson"].Rows[this.cboNatureOfPerson.SelectedIndex]["NATURE_PERSON_NO"]) == 5)
                        {
                            //Has Passport
                            this.txtPassportNo.Enabled = true;
                            this.cboCountry.Enabled = true;
                        }
                    }
                }

                if (this.cboBankAccountType.SelectedIndex < 1)
                {
                    this.txtBranchCode.Enabled = false;
                    this.txtBranchDesc.Enabled = false;
                    this.txtAccountNo.Enabled = false;
                    this.txtAccountName.Enabled = false;
                    this.cboBankAccountRelationship.Enabled = false;
                }

                this.dgvEmployeeLeaveDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
                this.dgvEmployeeLeaveDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;
            }

            if (pvtstrPayrollType == "S")
            {
                this.cboEarningNoCheques.Enabled = true;
            }

            this.btnPayCategoryAdd.Enabled = true;
            this.btnPayCategoryRemove.Enabled = true;

            this.cboLeaveShift.Enabled = true;

            this.cboEtiStartMonth.Enabled = true;

            this.rbnTaxNormal.Enabled = true;
            this.rbnTaxPartTime.Enabled = true;
            this.rbnTaxDirective.Enabled = true;

            this.rbnResAddrOwn.Enabled = true;
            this.rbnResAddrCompany.Enabled = true;
            this.rbnWorkTelOwn.Enabled = true;
            this.rbnWorkTelCompany.Enabled = true;

            this.rbnSameResidentialAddr.Enabled = true;
            this.rbnStreetAddr.Enabled = true;
            this.rbnPOBoxAddr.Enabled = true;
            this.rbnPrivateBagAddr.Enabled = true;

            this.chkPayslip.Enabled = true;
            this.chkEmpNo.Enabled = true;
            
            this.btnNew.Enabled = false;
            this.btnUpdate.Enabled = false;
            this.btnDelete.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.lblMessage.Visible = true;
            this.btnClearFingers.Visible = true;

            EventArgs e = new EventArgs();

            //Fire Residential Enable
            Residential_Address_Click(null, e);

            string strEmployeePostOptionInd = "";

            if (pvtEmployeeDataView.Count > 0)
            {
                //R=Use Residential
                //S=Street Address
                //P=PO Box
                //B=Private Bag
                strEmployeePostOptionInd = pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_POST_OPTION_IND"].ToString();
            }

            if (strEmployeePostOptionInd == "R")
            {
                rbnPostalOption_Click(this.rbnSameResidentialAddr, e);
            }
            else
            {
                if (strEmployeePostOptionInd == "S")
                {
                    rbnPostalOption_Click(this.rbnStreetAddr, e);
                }
                else
                {
                    if (strEmployeePostOptionInd == "P")
                    {
                        rbnPostalOption_Click(this.rbnPOBoxAddr, e);

                    }
                    else
                    {
                        if (strEmployeePostOptionInd == "B")
                        {
                            rbnPostalOption_Click(this.rbnPrivateBagAddr, e);

                        }
                        else
                        {
                            this.txtPostAddrUnitNumber.Enabled = false;
                            this.txtPostAddrComplex.Enabled = false;
                            this.txtPostAddrStreetNumber.Enabled = false;
                            this.txtPostStreetName.Enabled = false;
                            this.txtPostSuburb.Enabled = false;
                            this.txtPostCity.Enabled = false;
                            this.txtPostAddrCode.Enabled = false;
                            this.cboPostCountry.Enabled = false;
                            this.btnPostalAddressRSA.Enabled = false;
                        }
                    }
                }
            }

            //Fire Work Telephone Enable
            Work_Telephone_Click(null, e);

            //Fire Tax Type
            TaxType_Click(null, e);

            //Fire Account Type which will Enable Linked Fields
            cboAccountType_SelectedIndexChanged(null, e);

            //Earnings
            btnEarningAdd.Enabled = true;
            btnEarningRemove.Enabled = true;

            //Deductions
            this.btnDeductionAdd.Enabled = true;
            this.btnDeductionRemove.Enabled = true;

            if (this.Text.EndsWith("- New") == true)
            {
                this.rbnSameResidentialAddr.Checked = true;

                if (this.dgvPayCategoryDataGridView.Rows.Count == 1)
                {
                    sender = new object();
                    e = new EventArgs();

                    btnPayCategoryAdd_Click(sender, e);
                }

                //2012-10-23
                if (this.cboOccupation.Items.Count == 1)
                {
                    this.cboOccupation.SelectedIndex = 0;
                }

                //2012-10-23
                if (this.cboDepartment.Items.Count == 1)
                {
                    this.cboDepartment.SelectedIndex = 0;
                }

                //2012-10-23
                cboBankAccountType.SelectedIndex = 0;

                //Choose Leave Shift if Their is Only 1 Choice NB. Must be Here 
                if (cboLeaveShift.Items.Count == 1)
                {
                    cboLeaveShift.SelectedIndex = 0;
                }
            }
            else
            {
                //Fire Earnings
                if (this.dgvChosenEarningDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvChosenEarningDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvChosenEarningDataGridView));
                }

                //Fire Deductions
                if (this.dgvChosenDeductionDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvChosenDeductionDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvChosenDeductionDataGridView));
                }

                //Add Edit Row to Leave and Loans
                if (pvtEmployeeDeductionDataView != null)
                {
                    Check_To_Add_New_Leave_Row();
                    Check_To_Add_New_Loan_Row();
                }
            }

            this.btnLeaveDeleteRec.Enabled = true;
            this.btnLoanDeleteRec.Enabled = true;

            this.dgvEmployeeLoanDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dgvEmployeeLoanDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;

            this.rbnActive.Enabled = false;
            this.rbnClosed.Enabled = false;

            this.rbnNone.Enabled = false;
            this.rbnTemplateMissing.Enabled = false;
            this.rbnEnrolledFingerprint.Enabled = false;

            this.rbnLoanCurrentYear.Enabled = false;
            this.rbnLoanAllYears.Enabled = false;

            if (this.pvtDataSet.Tables["Company"].Rows[0]["GENERATE_EMPLOYEE_NUMBER_IND"].ToString() == "Y")
            {
                this.txtCode.Focus();
            }
            else
            {
                this.txtCode.Enabled = false;
                this.txtInitials.Focus();
            }

            this.tabEmployee.Refresh();
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
                                                         "",
                                                         "",
                                                         pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_NO"].ToString());

            }

            this.pvtblnPayCategoryDataGridViewLoaded = true;

            this.txtRate.Text = "";
            this.rbnDefaultYes.Checked = true;
        }

        private void Residential_Address_Click(object sender, EventArgs e)
        {
            bool blnEnabledValue = true;

            if (this.rbnResAddrCompany.Checked == true)
            {
                pvtEmployeeDataView[clsISUtilities.DataViewIndex]["USE_RES_ADDR_COMPANY_IND"] = "Y";

                blnEnabledValue = false;

                this.txtResAddrUnitNumber.Text = "";
                this.txtResAddrComplex.Text = "";
                this.txtResAddrStreetNumber.Text = "";
                this.txtStreetName.Text = "";
                this.txtSuburb.Text = "";
                this.txtCity.Text = "";
                this.txtPhysicalCode.Text = "";
                this.cboAddressCountry.SelectedIndex = -1;
            }
            else
            {
                pvtEmployeeDataView[clsISUtilities.DataViewIndex]["USE_RES_ADDR_COMPANY_IND"] = "";
            }

            this.txtResAddrUnitNumber.Enabled = blnEnabledValue;
            this.txtResAddrComplex.Enabled = blnEnabledValue;
            this.txtResAddrStreetNumber.Enabled = blnEnabledValue;
            this.txtStreetName.Enabled = blnEnabledValue;
            this.txtSuburb.Enabled = blnEnabledValue;
            this.txtCity.Enabled = blnEnabledValue;
            this.txtPhysicalCode.Enabled = blnEnabledValue;
            this.cboAddressCountry.Enabled = blnEnabledValue;
            this.btnPhsicalAddressRSA.Enabled = blnEnabledValue; 
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

        private void TaxType_Click(object sender, EventArgs e)
        {
            if (this.rbnTaxDirective.Checked == true)
            {
                this.txtTaxDirectiveNo1.Enabled = true;
                this.txtTaxDirectiveNo2.Enabled = true;
                this.txtTaxDirectiveNo3.Enabled = true;

                this.txtTaxDirectivePercentAmount.Enabled = true;

                pvtEmployeeDataView[clsISUtilities.DataViewIndex]["TAX_TYPE_IND"] = "D";
            }
            else
            {
                if (this.rbnTaxNormal.Checked == true)
                {
                    pvtEmployeeDataView[clsISUtilities.DataViewIndex]["TAX_TYPE_IND"] = "N";
                }
                else
                {
                    pvtEmployeeDataView[clsISUtilities.DataViewIndex]["TAX_TYPE_IND"] = "P";
                }

                this.txtTaxDirectiveNo1.Enabled = false;
                this.txtTaxDirectiveNo2.Enabled = false;
                this.txtTaxDirectiveNo3.Enabled = false;

                this.txtTaxDirectivePercentAmount.Enabled = false;

                this.txtTaxDirectiveNo1.Text = "";
                this.txtTaxDirectiveNo2.Text = "";
                this.txtTaxDirectiveNo3.Text = "";

                this.txtTaxDirectivePercentAmount.Text = "0";
            }

            clsISUtilities.Update_Paint_Parent_Marker(txtTaxDirectivePercentAmount);
        }

        private void cboAccountType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (this.cboBankAccountType.SelectedIndex > 0)
                {
                    this.txtBranchCode.Enabled = true;
                    this.txtBranchDesc.Enabled = true;
                    this.txtAccountNo.Enabled = true;
                    this.txtAccountName.Enabled = true;
                    this.cboBankAccountRelationship.Enabled = true;
                }
                else
                {
                    this.txtBranchCode.Enabled = false;
                    this.txtBranchDesc.Enabled = false;
                    this.txtAccountNo.Enabled = false;
                    this.txtAccountName.Enabled = false;
                    this.cboBankAccountRelationship.Enabled = false;

                    this.txtBranchCode.Text = "0";
                    this.txtBranchDesc.Text = "";
                    this.txtAccountNo.Text = "0";
                    this.txtAccountName.Text = "";
                    this.cboBankAccountRelationship.SelectedIndex = -1;
                }
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

        public void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                pvtTempDataSet = null;
                pvtTempDataSet = new DataSet();

                //DataLayer Fields are Checked
                pvtintReturnCode = this.clsISUtilities.DataBind_Save_Check();

                if (pvtintReturnCode != 0)
                {
                    return;
                }

                //NB pvtTempDataSet Tables Are Created in Save_Check
                pvtintReturnCode = Save_Check();

                if (pvtintReturnCode != 0)
                {
                    return;
                }

                //Compress DataSet
                pvtbytCompress = clsISUtilities.Compress_DataSet(pvtTempDataSet);
                
                this.rbnNone.Checked = true;

                if (this.Text.IndexOf("- New", 0) > 0)
                {
                    pvtobjSender = null;
                    this.tabEmployee.SelectedIndex = 0;

                    string strEmployeeCode = "";

                    object[] objParm = new object[4];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));

                    objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[2] = pvtbytCompress;
                    objParm[3] = pvtstrPayrollType;

                    pvtintEmployeeNo = (int)clsISUtilities.DynamicFunction("Insert_New_Record", objParm, true);

                    //Change Key From -1 to Actual Value
                    DataView EmployeePayCategoryDataView = new DataView(pvtDataSet.Tables["EmployeePayCategory"],
                        "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND EMPLOYEE_NO = -1",
                        "",
                        DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < EmployeePayCategoryDataView.Count; intRow++)
                    {
                        EmployeePayCategoryDataView[intRow]["EMPLOYEE_NO"] = pvtintEmployeeNo;

                        intRow -= 1;
                    }

                    //Change Key From -1 to Actual Value
                    DataView EmployeeFingerTemplateDataView = new DataView(pvtDataSet.Tables["EmployeeFingerTemplate"],
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
                    //Remove Leave Rows Added - Will be Passed Back via DataLayer with Correct Keys
                    DataView LeaveDataView = new DataView(pvtDataSet.Tables["EmployeeLeave"],
                    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND LEAVE_PROCESSED_DATE IS NULL ",
                    "",
                    DataViewRowState.Added);

                    for (int intRow = 0; intRow < LeaveDataView.Count; intRow++)
                    {
                        LeaveDataView[intRow].Delete();
                        intRow -= 1;
                    }

                    //Remove Loan Rows Added - Will be Passed Back via DataLayer with Correct Keys
                    DataView LoanDataView = new DataView(pvtDataSet.Tables["EmployeeLoan"],
                    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND LOAN_PROCESSED_DATE IS NULL ",
                    "",
                    DataViewRowState.Added);

                    for (int intRow = 0; intRow < LoanDataView.Count; intRow++)
                    {
                        LoanDataView[intRow].Delete();
                        intRow -= 1;
                    }

                    object[] objParm = new object[4];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[2] = pvtbytCompress;
                    objParm[3] = pvtstrPayrollType;

                    pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Update_Record", objParm, true);

                    pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                    pvtDataSet.Merge(pvtTempDataSet);

                    if (Convert.ToInt32(pvtDataSet.Tables["ReturnCode"].Rows[0]["RETURN_CODE"]) == 9999)
                    {
                        this.pvtDataSet.RejectChanges();

                        this.dgvEmployeeDataGridView[0,pvtintEmployeeDataGridViewRowIndex].Style = LockedPayrollRunDataGridViewCellStyle;

                        this.grbEmployeeLock.Visible = true;

                        pvtEmployeeDataView[clsISUtilities.DataViewIndex]["PAYROLL_LINK"] = 0;

                        CustomMessageBox.Show("This Employee is currently being used in a Payroll Run.\r\n\r\nUpdate Cancelled.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                    else
                    {
                        this.dgvEmployeeDataGridView[4, pvtintEmployeeDataGridViewRowIndex].Value = pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_SURNAME"].ToString();
                        this.dgvEmployeeDataGridView[5, pvtintEmployeeDataGridViewRowIndex].Value = pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_NAME"].ToString();
                    }
                }

                this.pvtDataSet.AcceptChanges();
                                
                //Set So That Employees will Reload
                this.btnSave.Enabled = false;

                dgvEmployeeDataGridView.Enabled = true;

                Load_Employees();
                
                btnCancel_Click(sender, e);
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        public bool IsValidEmail(string source)
        {
            if (source.Trim() == "")
            {
                return true;
            }
            else
            {
                return new EmailAddressAttribute().IsValid(source);
            }
        }

        private int Save_Check()
        {
            bool blnFound = false;
            
            //Check Email is Formed Correct
            if (IsValidEmail(this.txtEmail.Text.Trim()) == false)
            {
                ErrorMessage(this.txtEmail, "Email Address Is Invalid");

                return 1;
            }
            
            if (pvtstrPayrollType == "S")
            {
                if (cboEarningNoCheques.SelectedIndex != -1
                    & this.txtRate.Text.Trim() != "")
                {
                    pvtEmployeeDataView[clsISUtilities.DataViewIndex]["ANNUAL_SALARY"] = Convert.ToDouble(this.txtRate.Text.Trim()) * Convert.ToDouble(cboEarningNoCheques.SelectedItem.ToString());
                }
            }

            //Errol 2015-02-14
            if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_POST_OPTION_IND"].ToString() == "")
            {
                pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_POST_OPTION_IND"] = "R";
            }

            //Add Employee Table 
            pvtTempDataSet.Tables.Add(this.pvtDataSet.Tables["Employee"].Clone());
            pvtTempDataSet.Tables["Employee"].ImportRow(pvtEmployeeDataView[clsISUtilities.DataViewIndex].Row);

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
                    ErrorMessage(this.txtCode, "Employee '" + EmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString() + " " + EmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString() + "' already Exists with an Employee Code of '" + EmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString() + "'.");

                    return 1;
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
                if (EmployeePayCategoryDataView[intRow].Row.RowState == DataRowState.Deleted)
                {
                }
                else
                {
                    if (Convert.ToDouble(EmployeePayCategoryDataView[intRow]["HOURLY_RATE"]) == 0)
                    {
                        for (int intChosenPayCategoryRow = 0; intChosenPayCategoryRow < this.dgvChosenPayCategoryDataGridView.Rows.Count; intChosenPayCategoryRow++)
                        {
                            if (Convert.ToInt32(this.dgvChosenPayCategoryDataGridView[pvtintPayCategoryNoCol, intChosenPayCategoryRow].Value) == Convert.ToInt32(EmployeePayCategoryDataView[intRow]["PAY_CATEGORY_NO"]))
                            {
                                this.Set_DataGridView_SelectedRowIndex(dgvChosenPayCategoryDataGridView, intChosenPayCategoryRow);

                                break;
                            }
                        }

                        ErrorMessage(this.txtRate, "Hourly Rate cannot be Zero.");

                        return 1;
                    }

                    if (intRow == EmployeePayCategoryDataView.Count - 1)
                    {
                        //Check For Default Value
                        DataView DefaultEmployeePayCategoryDataView = new DataView(pvtDataSet.Tables["EmployeePayCategory"],
                        pvtstrFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND DEFAULT_IND = 'Y'",
                        "",
                        DataViewRowState.CurrentRows);

                        if (DefaultEmployeePayCategoryDataView.Count == 0)
                        {
                            this.tabEmployee.SelectedIndex = 2;

                            this.Set_DataGridView_SelectedRowIndex(this.dgvChosenPayCategoryDataGridView, 0);

                            ErrorMessage(this.dgvChosenPayCategoryDataGridView, "One Cost Centre must be set as the Default.");

                            return 1;
                        }
                    }
                }

                pvtTempDataSet.Tables["EmployeePayCategory"].ImportRow(EmployeePayCategoryDataView[intRow].Row);
            }

            //Employee Earnings
            //Employee Earnings
            pvtTempDataSet.Tables.Add(pvtDataSet.Tables["EmployeeEarning"].Clone());

            DataView EarningDataView = new DataView(pvtDataSet.Tables["EmployeeEarning"],
                pvtstrFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND ((EARNING_NO > 9 AND EARNING_NO < 200) OR EARNING_NO > 201)",
                "",
                 DataViewRowState.Added | DataViewRowState.ModifiedCurrent | DataViewRowState.Deleted);

            for (int intRow = 0; intRow < EarningDataView.Count; intRow++)
            {
                if (EarningDataView[intRow].Row.RowState == DataRowState.Deleted)
                {
                }
                else
                {
                    //F=Fixed X=Multiple  
                    if (EarningDataView[intRow]["EARNING_TYPE_IND"].ToString() == "F"
                        | EarningDataView[intRow]["EARNING_TYPE_IND"].ToString() == "X")
                    {
                        if (Convert.ToDouble(EarningDataView[intRow]["AMOUNT"]) == 0)
                        {
                            for (int intErrorRow = 0; intErrorRow < this.dgvChosenEarningDataGridView.Rows.Count; intErrorRow++)
                            {
                                if (this.dgvChosenEarningDataGridView[pvtintEarningsEarningNoCol, intErrorRow].Value.ToString() == EarningDataView[intRow]["EARNING_NO"].ToString())
                                {
                                    this.Set_DataGridView_SelectedRowIndex(this.dgvChosenEarningDataGridView, intErrorRow);

                                    ErrorMessage(this.txtEarningPeriodAmt, "Period Amount must be Greater Than Zero.");

                                    return 1;
                                }
                            }
                        }

                        if (EarningDataView[intRow]["EARNING_PERIOD_IND"].ToString() == "M")
                        {
                            if (Convert.ToDouble(EarningDataView[intRow]["EARNING_DAY_VALUE"]) == 0)
                            {
                                for (int intErrorRow = 0; intErrorRow < this.dgvChosenEarningDataGridView.Rows.Count; intErrorRow++)
                                {
                                    if (this.dgvChosenEarningDataGridView[pvtintEarningsEarningNoCol, intErrorRow].Value.ToString() == EarningDataView[intRow]["EARNING_NO"].ToString())
                                    {
                                        this.Set_DataGridView_SelectedRowIndex(this.dgvChosenEarningDataGridView, intErrorRow);

                                        ErrorMessage(this.cboEarningDay, "Select Day of Month.");

                                        return 1;
                                    }
                                }
                            }
                        }
                    }
                }

                pvtTempDataSet.Tables["EmployeeEarning"].ImportRow(EarningDataView[intRow].Row);
            }

            //Employee Deductions
            //Employee Deductions
            pvtTempDataSet.Tables.Add(pvtDataSet.Tables["EmployeeDeduction"].Clone());

            DataView EmployeeDeductionnDataView = new DataView(pvtDataSet.Tables["EmployeeDeduction"],
               pvtstrFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo,
               "",
                DataViewRowState.Added | DataViewRowState.ModifiedCurrent | DataViewRowState.Deleted);

            for (int intRow = 0; intRow < EmployeeDeductionnDataView.Count; intRow++)
            {
                if (EmployeeDeductionnDataView[intRow].Row.RowState == DataRowState.Deleted)
                {
                }
                else
                {
                    if (Convert.ToInt32(EmployeeDeductionnDataView[intRow]["DEDUCTION_NO"]) == 1
                        | Convert.ToInt32(EmployeeDeductionnDataView[intRow]["DEDUCTION_NO"]) == 2)
                    {
                        //Tax / UIF
                    }
                    else
                    {
                        if (EmployeeDeductionnDataView[intRow]["DEDUCTION_TYPE_IND"].ToString() == "P"
                            | EmployeeDeductionnDataView[intRow]["DEDUCTION_TYPE_IND"].ToString() == "F")
                        {
                            if (EmployeeDeductionnDataView[intRow]["DEDUCTION_PERIOD_IND"].ToString() == "M")
                            {
                                if (Convert.ToInt32(EmployeeDeductionnDataView[intRow]["DEDUCTION_DAY_VALUE"]) < 1)
                                {
                                    int intDeductionRow = Find_Deduction_Spreadsheet_Row_In_Error(Convert.ToInt32(EmployeeDeductionnDataView[intRow]["DEDUCTION_NO"]));

                                    this.Set_DataGridView_SelectedRowIndex(this.dgvChosenDeductionDataGridView, intDeductionRow);

                                    ErrorMessage(this.cboDeductDay, "Select Deduction Day Of Month.");

                                    return 1;
                                }
                            }

                            if (Convert.ToDouble(EmployeeDeductionnDataView[intRow]["DEDUCTION_VALUE"]) == 0)
                            {
                                int intDeductionRow = Find_Deduction_Spreadsheet_Row_In_Error(Convert.ToInt32(EmployeeDeductionnDataView[intRow]["DEDUCTION_NO"]));

                                this.Set_DataGridView_SelectedRowIndex(this.dgvChosenDeductionDataGridView, intDeductionRow);

                                ErrorMessage(this.txtDeductValue, "Enter Deduction Value.");

                                return 1;
                            }
                        }

                        if (EmployeeDeductionnDataView[intRow]["DEDUCTION_TYPE_IND"].ToString() == "P")
                        {
                            blnFound = false;

                            DataView DeductionEarningPercentage = new DataView(pvtDataSet.Tables["DeductionEarningPercentage"],
                            pvtstrFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND DEDUCTION_NO = " + EmployeeDeductionnDataView[intRow]["DEDUCTION_NO"].ToString() + " AND DEDUCTION_SUB_ACCOUNT_NO = " + EmployeeDeductionnDataView[intRow]["DEDUCTION_SUB_ACCOUNT_NO"].ToString(),
                            "",
                            DataViewRowState.CurrentRows);

                            for (int intDeductionRow = 0; intDeductionRow < DeductionEarningPercentage.Count; intDeductionRow++)
                            {
                                if (DeductionEarningPercentage[intDeductionRow].Row.RowState == DataRowState.Deleted)
                                {
                                }
                                else
                                {
                                    blnFound = true;
                                    break;
                                }
                            }

                            if (blnFound == false)
                            {
                                int intDeductionRow = Find_Deduction_Spreadsheet_Row_In_Error(Convert.ToInt32(EmployeeDeductionnDataView[intRow]["DEDUCTION_NO"]));

                                this.Set_DataGridView_SelectedRowIndex(this.dgvChosenDeductionDataGridView, intDeductionRow);

                                ErrorMessage(this.lblPercentEarnings, "Select 1 or more Earnings.");

                                return 1;
                            }
                        }

                        if (Convert.ToInt32(EmployeeDeductionnDataView[intRow]["DEDUCTION_NO"]) == 5)
                        {
                            if (this.txtNumberMedicalDependents.Text.Trim() == "")
                            {
                                int intDeductionRow = Find_Deduction_Spreadsheet_Row_In_Error(Convert.ToInt32(EmployeeDeductionnDataView[intRow]["DEDUCTION_NO"]));

                                this.Set_DataGridView_SelectedRowIndex(this.dgvChosenDeductionDataGridView, intDeductionRow);

                                ErrorMessage(this.txtNumberMedicalDependents, "Number of Medical Aid Dependents MUST be Numeric.");

                                return 1;
                            }
                        }
                    }
                }

                pvtTempDataSet.Tables["EmployeeDeduction"].ImportRow(EmployeeDeductionnDataView[intRow].Row);
            }

            DataTable myDataTable = pvtDataSet.Tables["DeductionEarningPercentage"].GetChanges();

            if (myDataTable != null)
            {
                pvtTempDataSet.Tables.Add(myDataTable);
            }
            else
            {
                pvtTempDataSet.Tables.Add(pvtDataSet.Tables["DeductionEarningPercentage"].Clone());
            }

            //Employee Leave
            pvtTempDataSet.Tables.Add(pvtDataSet.Tables["EmployeeLeave"].Clone());

            DataView EmployeeLeaveDataView = new DataView(pvtDataSet.Tables["EmployeeLeave"],
                pvtstrFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo,
                "",
                 DataViewRowState.Added | DataViewRowState.ModifiedCurrent | DataViewRowState.Deleted);

            for (int intRow = 0; intRow < EmployeeLeaveDataView.Count; intRow++)
            {
                if (EmployeeLeaveDataView[intRow].Row.RowState == DataRowState.Deleted)
                {
                }
                else
                {
                    if (EmployeeLeaveDataView[intRow]["LEAVE_DESC"].ToString() == ""
                    & EmployeeLeaveDataView[intRow]["LEAVE_FROM_DATE"] == System.DBNull.Value
                    & EmployeeLeaveDataView[intRow]["LEAVE_TO_DATE"] == System.DBNull.Value
                    & EmployeeLeaveDataView[intRow]["PROCESS_NO"] == System.DBNull.Value
                    & EmployeeLeaveDataView[intRow]["LEAVE_OPTION"].ToString() == "")
                    {
                        continue;
                    }
                    else
                    {
                        if (EmployeeLeaveDataView[intRow]["LEAVE_DESC"].ToString() == "")
                        {
                            int intReturnRow = Return_EmployeeLeave_Row_In_Error(Convert.ToInt32(EmployeeLeaveDataView[intRow]["LEAVE_REC_NO"]));

                            dgvEmployeeLeaveDataGridView.CurrentCell = dgvEmployeeLeaveDataGridView[pvtintLeaveDescCol, intReturnRow];

                            ErrorMessage(this.dgvEmployeeLeaveDataGridView, "Enter Description.");

                            return 1;
                        }
                        else
                        {
                            if (EmployeeLeaveDataView[intRow]["LEAVE_FROM_DATE"] == System.DBNull.Value)
                            {
                                int intReturnRow = Return_EmployeeLeave_Row_In_Error(Convert.ToInt32(EmployeeLeaveDataView[intRow]["LEAVE_REC_NO"]));

                                dgvEmployeeLeaveDataGridView.CurrentCell = dgvEmployeeLeaveDataGridView[pvtintLeaveFromDateCol, intReturnRow];

                                ErrorMessage(this.dgvEmployeeLeaveDataGridView, "Capture From Date.");

                                return 1;
                            }
                            else
                            {
                                if (EmployeeLeaveDataView[intRow]["LEAVE_TO_DATE"] == System.DBNull.Value)
                                {
                                    int intReturnRow = Return_EmployeeLeave_Row_In_Error(Convert.ToInt32(EmployeeLeaveDataView[intRow]["LEAVE_REC_NO"]));

                                    dgvEmployeeLeaveDataGridView.CurrentCell = dgvEmployeeLeaveDataGridView[pvtintLeaveToDateCol, intReturnRow];

                                    ErrorMessage(this.dgvEmployeeLeaveDataGridView, "Capture To Date.");

                                    return 1;
                                }
                                else
                                {
                                    if (Convert.ToDateTime(EmployeeLeaveDataView[intRow]["LEAVE_FROM_DATE"]) > Convert.ToDateTime(EmployeeLeaveDataView[intRow]["LEAVE_FROM_DATE"]))
                                    {
                                        int intReturnRow = Return_EmployeeLeave_Row_In_Error(Convert.ToInt32(EmployeeLeaveDataView[intRow]["LEAVE_REC_NO"]));

                                        dgvEmployeeLeaveDataGridView.CurrentCell = dgvEmployeeLeaveDataGridView[pvtintLeaveFromDateCol, intReturnRow];

                                        ErrorMessage(this.dgvEmployeeLeaveDataGridView, "From Date cannot be Greater than To Date.");

                                        return 1;
                                    }
                                    else
                                    {
                                        if (EmployeeLeaveDataView[intRow]["LEAVE_OPTION"].ToString() == "")
                                        {
                                            int intReturnRow = Return_EmployeeLeave_Row_In_Error(Convert.ToInt32(EmployeeLeaveDataView[intRow]["LEAVE_REC_NO"]));

                                            dgvEmployeeLeaveDataGridView.CurrentCell = dgvEmployeeLeaveDataGridView[pvtintLeaveOptionCol, intReturnRow];

                                            ErrorMessage(this.dgvEmployeeLeaveDataGridView, "Select Option.");

                                            return 1;
                                        }
                                        else
                                        {
                                            if (EmployeeLeaveDataView[intRow]["PROCESS_NO"] == System.DBNull.Value)
                                            {
                                                int intReturnRow = Return_EmployeeLeave_Row_In_Error(Convert.ToInt32(EmployeeLeaveDataView[intRow]["LEAVE_REC_NO"]));

                                                dgvEmployeeLeaveDataGridView.CurrentCell = dgvEmployeeLeaveDataGridView[pvtintLeaveProcessCol, intReturnRow];

                                                ErrorMessage(this.dgvEmployeeLeaveDataGridView, "Select Process.");

                                                return 1;
                                            }
                                            else
                                            {
                                                //Hours
                                                if (EmployeeLeaveDataView[intRow]["LEAVE_OPTION"].ToString() == "H")
                                                {
                                                    if (Convert.ToDouble(EmployeeLeaveDataView[intRow]["LEAVE_HOURS_DECIMAL"]) == 0)
                                                    {
                                                        int intReturnRow = Return_EmployeeLeave_Row_In_Error(Convert.ToInt32(EmployeeLeaveDataView[intRow]["LEAVE_REC_NO"]));

                                                        dgvEmployeeLeaveDataGridView.CurrentCell = dgvEmployeeLeaveDataGridView[pvtintLeaveHoursCol, intReturnRow];

                                                        ErrorMessage(this.dgvEmployeeLeaveDataGridView, "Hours Value cannot be Zero.");

                                                        return 1;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                pvtTempDataSet.Tables["EmployeeLeave"].ImportRow(EmployeeLeaveDataView[intRow].Row);
            }

            //Loans
            //Loans
            pvtTempDataSet.Tables.Add(pvtDataSet.Tables["EmployeeLoan"].Clone());

            DataView EmployeeLoanDataView = new DataView(pvtDataSet.Tables["EmployeeLoan"],
                pvtstrFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo,
                "",
                 DataViewRowState.Added | DataViewRowState.ModifiedCurrent | DataViewRowState.Deleted);

            for (int intRow = 0; intRow < EmployeeLoanDataView.Count; intRow++)
            {
                if (EmployeeLoanDataView[intRow].Row.RowState == DataRowState.Deleted)
                {
                }
                else
                {
                    if (EmployeeLoanDataView[intRow]["LOAN_DESC"].ToString() == ""
                    & EmployeeLoanDataView[intRow]["PROCESS_NO"] == System.DBNull.Value
                    & Convert.ToDouble(EmployeeLoanDataView[intRow]["LOAN_AMOUNT_PAID"]) == 0)
                    {
                        continue;
                    }
                    else
                    {
                        if (EmployeeLoanDataView[intRow]["LOAN_DESC"].ToString() == "")
                        {

                            int intReturnRow = Return_EmployeeLoan_Row_In_Error(Convert.ToInt32(EmployeeLoanDataView[intRow]["LOAN_REC_NO"]));

                            dgvEmployeeLoanDataGridView.CurrentCell = dgvEmployeeLoanDataGridView[pvtintLoanDescCol, intReturnRow];

                            ErrorMessage(this.dgvEmployeeLoanDataGridView, "Enter Description.");

                            return 1;
                        }
                        else
                        {
                            if (EmployeeLoanDataView[intRow]["PROCESS_NO"] == System.DBNull.Value)
                            {
                                int intReturnRow = Return_EmployeeLoan_Row_In_Error(Convert.ToInt32(EmployeeLoanDataView[intRow]["LOAN_REC_NO"]));

                                dgvEmployeeLoanDataGridView.CurrentCell = dgvEmployeeLoanDataGridView[pvtintLoanProcessOptionCol, intReturnRow];

                                ErrorMessage(this.dgvEmployeeLoanDataGridView, "Select Process Option.");

                                return 1;
                            }
                            else
                            {
                                if (Convert.ToDouble(EmployeeLoanDataView[intRow]["LOAN_AMOUNT_PAID"]) == 0)
                                {
                                    int intReturnRow = Return_EmployeeLoan_Row_In_Error(Convert.ToInt32(EmployeeLoanDataView[intRow]["LOAN_REC_NO"]));

                                    dgvEmployeeLoanDataGridView.CurrentCell = dgvEmployeeLoanDataGridView[pvtintLoanPaidCol, intReturnRow];

                                    ErrorMessage(this.dgvEmployeeLoanDataGridView, "Paid Out Amount must be Gtreater than Zero.");

                                    return 1;
                                }
                            }
                        }
                    }
                }

                pvtTempDataSet.Tables["EmployeeLoan"].ImportRow(EmployeeLoanDataView[intRow].Row);
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
                        
            return 0;
        }

        private void ErrorMessage(Control Control, string ErrorMessage)
        {
            Control ctlParent = Control.Parent;

        ErrorMessage_Continue:

            if (ctlParent is TabPage)
            {
                TabPage ControlTabPage = (TabPage)ctlParent;
                int intIndex = tabEmployee.TabPages.IndexOf(ControlTabPage);
                tabEmployee.SelectedIndex = intIndex;
            }
            else
            {
                if (ctlParent is Form)
                {
                }
                else
                {
                    ctlParent = ctlParent.Parent;

                    goto ErrorMessage_Continue;
                }
            }

            CustomMessageBox.Show(ErrorMessage,
                    this.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

            Control.Focus();
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

        private int Find_Deduction_Spreadsheet_Row_In_Error(int DeductionNo)
        {
            int intRow = 0;

            for (intRow = 0; intRow < this.dgvChosenDeductionDataGridView.Rows.Count; intRow++)
            {
                if (Convert.ToInt32(this.dgvChosenDeductionDataGridView[pvtintDeductionsDeductionNoCol, intRow].Value) == DeductionNo)
                {
                    break;
                }
            }

            return intRow;
        }

        private int Return_EmployeeLeave_Row_In_Error(int intLeaveRecNo)
        {
            int intReturnRow = 0;

            for (intReturnRow = 0; intReturnRow < this.dgvEmployeeLeaveDataGridView.Rows.Count; intReturnRow++)
            {
                if (Convert.ToInt32(this.dgvEmployeeLeaveDataGridView[pvtintLeaveRecNoCol, intReturnRow].Value) == intLeaveRecNo)
                {
                    break;
                }
            }

            return intReturnRow;
        }

        private int Return_EmployeeLoan_Row_In_Error(int intLoanRecNo)
        {
            int intReturnRow = 0;

            for (intReturnRow = 0; intReturnRow < this.dgvEmployeeLoanDataGridView.Rows.Count; intReturnRow++)
            {
                if (Convert.ToInt32(this.dgvEmployeeLoanDataGridView[pvtintLoanRecNoCol, intReturnRow].Value) == intLoanRecNo)
                {
                    break;
                }
            }

            return intReturnRow;
        }

        private void EmployeeFilter_Click(object sender, EventArgs e)
        {
            string strPayCategoryClosedInd = "";

            if (this.rbnActive.Checked == true)
            {
                this.btnNew.Enabled = true;
                strPayCategoryClosedInd = " AND CLOSED_IND <> 'Y' ";

                this.lblClosed.Visible = false;
            }
            else
            {
                this.btnNew.Enabled = false;

                this.lblClosed.Visible = true;
            }
           
            pvtPayCategoryDataView = null;
            pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"],
                pvtstrFilter + strPayCategoryClosedInd,
                "",
                DataViewRowState.CurrentRows);

            Load_Employees();
        }

        private void rbnFilter_Click(object sender, EventArgs e)
        {
            Load_CurrentForm_Records();
        }

        private void cboNatureOfPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (cboNatureOfPerson.SelectedIndex > -1)
                {
                    this.txtIDNo.Enabled = false;
                    this.cboCountry.Enabled = false;
                    this.txtPassportNo.Enabled = false;

                    if (Convert.ToInt32(this.pvtDataSet.Tables["NaturePerson"].Rows[this.cboNatureOfPerson.SelectedIndex]["NATURE_PERSON_NO"]) == 1
                        | Convert.ToInt32(this.pvtDataSet.Tables["NaturePerson"].Rows[this.cboNatureOfPerson.SelectedIndex]["NATURE_PERSON_NO"]) == 4)
                    {
                        //Has Identity Document
                        this.txtIDNo.Enabled = true;

                        pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_PASSPORT_NO"] = "";
                        //ComboBox Value
                        pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_PASSPORT_COUNTRY_CODE"] = "";

                        this.txtPassportNo.Text = "";

                        this.cboCountry.SelectedIndex = -1;

                        this.txtIDNo.Focus();
                    }
                    else
                    {
                        if (Convert.ToInt32(this.pvtDataSet.Tables["NaturePerson"].Rows[this.cboNatureOfPerson.SelectedIndex]["NATURE_PERSON_NO"]) == 2
                            | Convert.ToInt32(this.pvtDataSet.Tables["NaturePerson"].Rows[this.cboNatureOfPerson.SelectedIndex]["NATURE_PERSON_NO"]) == 5)
                        {
                            //Has Passport
                            this.txtPassportNo.Enabled = true;
                            this.cboCountry.Enabled = true;

                            pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_ID_NO"] = "";

                            this.txtIDNo.Text = "";

                            this.txtPassportNo.Focus();
                        }
                        else
                        {
                            if (Convert.ToInt32(this.pvtDataSet.Tables["NaturePerson"].Rows[this.cboNatureOfPerson.SelectedIndex]["NATURE_PERSON_NO"]) == 3)
                            {
                                pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_ID_NO"] = "";
                                pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_PASSPORT_NO"] = "";
                                //ComboBox Value
                                pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_PASSPORT_COUNTRY_CODE"] = "";

                                //Has Nothing
                                this.txtIDNo.Text = "";
                                this.txtPassportNo.Text = "";

                                this.cboCountry.SelectedIndex = -1;
                            }
                        }
                    }
                }
            }
        }

        private void txtIDNo_Leave(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (this.txtBirthDate.Text == "")
                {
                    int intReturnCode = clsISUtilities.SA_Identity_Number_Check(this.txtIDNo.Text.Trim());

                    if (intReturnCode == 0)
                    {
                        int intYear = Convert.ToInt32(this.txtIDNo.Text.Substring(0, 2));

                        if (intYear < 30)
                        {
                            intYear += 2000;
                        }
                        else
                        {
                            intYear += 1900;
                        }

                        int intMonth = Convert.ToInt32(this.txtIDNo.Text.Substring(2, 2));
                        int intDay = Convert.ToInt32(this.txtIDNo.Text.Substring(4, 2));

                        DateTime myDateTime = new DateTime(intYear, intMonth, intDay);

                        this.txtBirthDate.Text = myDateTime.ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString());

                        if (this.cboGender.SelectedIndex == -1)
                        {
                            if (Convert.ToInt32(this.txtIDNo.Text.Substring(6, 1)) < 5)
                            {
                                this.cboGender.SelectedIndex = 0;

                            }
                            else
                            {
                                this.cboGender.SelectedIndex = 1;
                            }
                        }
                    }
                }
            }
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

        private void Default_Option_Click(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (this.dgvChosenPayCategoryDataGridView.Rows.Count > 0)
                {
                    if (this.dgvChosenPayCategoryDataGridView.Rows.Count == 1
                        & this.rbnDefaultNo.Checked == true)
                    {
                        this.rbnDefaultYes.Checked = true;
                        System.Console.Beep();
                    }

                    DataView EmployeePayCategoryDataView = new DataView(pvtDataSet.Tables["EmployeePayCategory"],
                        "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND EMPLOYEE_NO = " + this.pvtintEmployeeNo + " AND PAY_CATEGORY_NO = " + this.dgvChosenPayCategoryDataGridView[pvtintPayCategoryNoCol, this.Get_DataGridView_SelectedRowIndex(this.dgvChosenPayCategoryDataGridView)].Value.ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                        "",
                         DataViewRowState.CurrentRows);

                    if (this.rbnDefaultYes.Checked == true)
                    {
                        this.dgvChosenPayCategoryDataGridView[pvtintPayCategoryDefaultIndCol, this.Get_DataGridView_SelectedRowIndex(this.dgvChosenPayCategoryDataGridView)].Value = true;
                        EmployeePayCategoryDataView[0]["DEFAULT_IND"] = 'Y';

                        pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_PAY_CATEGORY_NO"] = this.dgvChosenPayCategoryDataGridView[pvtintPayCategoryNoCol, this.Get_DataGridView_SelectedRowIndex(this.dgvChosenPayCategoryDataGridView)].Value.ToString();
                    }
                    else
                    {
                        this.dgvChosenPayCategoryDataGridView[pvtintPayCategoryDefaultIndCol, this.Get_DataGridView_SelectedRowIndex(this.dgvChosenPayCategoryDataGridView)].Value = false;
                        EmployeePayCategoryDataView[0]["DEFAULT_IND"] = 'N';
                    }

                    if (this.dgvChosenPayCategoryDataGridView.Rows.Count > 1)
                    {
                        EmployeePayCategoryDataView = null;
                        EmployeePayCategoryDataView = new DataView(pvtDataSet.Tables["EmployeePayCategory"],
                        "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND EMPLOYEE_NO = " + this.pvtintEmployeeNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                        "",
                         DataViewRowState.CurrentRows);

                        //Reset All Others to Default = N
                        for (int intRow = 0; intRow < EmployeePayCategoryDataView.Count; intRow++)
                        {
                            if (EmployeePayCategoryDataView[intRow]["PAY_CATEGORY_NO"].ToString() == this.dgvChosenPayCategoryDataGridView[pvtintPayCategoryNoCol, this.Get_DataGridView_SelectedRowIndex(this.dgvChosenPayCategoryDataGridView)].Value.ToString())
                            {
                                continue;
                            }

                            EmployeePayCategoryDataView[intRow]["DEFAULT_IND"] = 'N';
                        }

                        //Reset All Others to Default = N
                        for (int intRow = 0; intRow < this.dgvChosenPayCategoryDataGridView.Rows.Count; intRow++)
                        {
                            if (intRow == this.Get_DataGridView_SelectedRowIndex(this.dgvChosenPayCategoryDataGridView))
                            {
                                continue;
                            }

                            this.dgvChosenPayCategoryDataGridView[pvtintPayCategoryDefaultIndCol, intRow].Value = false;
                        }
                    }

                    this.dgvChosenPayCategoryDataGridView.Refresh();

                    if (this.cboLeaveShift.SelectedIndex > -1)
                    {
                        cboLeaveShift_SelectedIndexChanged(sender, e);
                    }
                }
            }
        }

        private void dgvEarningDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.btnEarningAdd_Click(sender, e);
            }
        }

        private void cboLeaveShift_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboLeaveShift.SelectedIndex > -1)
            {
                string strPrevCheckedOption = "";
                string strCheckedPrevOptionDesc = "";
                string strErrorText = "";

                if (this.btnSave.Enabled == true)
                {
                    if (this.txtLeaveDescription.Text == "Week Days Only")
                    {
                        strPrevCheckedOption = "W";
                        strCheckedPrevOptionDesc = "'Week Days'";
                    }
                    else
                    {
                        if (this.txtLeaveDescription.Text == "Week Days + Saturday")
                        {
                            strPrevCheckedOption = "B";
                            strCheckedPrevOptionDesc = "'Week Days + Saturday'";
                        }
                        else
                        {
                            if (this.txtLeaveDescription.Text == "ALL Days")
                            {
                                strPrevCheckedOption = "A";
                                strCheckedPrevOptionDesc = "'ALL Days'";
                            }
                        }
                    }
                }

                if (Convert.ToInt32(this.pvtLeaveLinkDataView[this.cboLeaveShift.SelectedIndex]["LEAVE_PAID_ACCUMULATOR_IND"]) == 1)
                {
                    this.txtLeaveDescription.Text = "Week Days Only";
                }
                else
                {
                    if (Convert.ToInt32(this.pvtLeaveLinkDataView[this.cboLeaveShift.SelectedIndex]["LEAVE_PAID_ACCUMULATOR_IND"]) == 2)
                    {
                        this.txtLeaveDescription.Text = "Week Days + Saturday";

                    }
                    else
                    {
                        this.txtLeaveDescription.Text = "ALL Days";
                    }
                }

                if (this.btnSave.Enabled == true)
                {
                    if (this.txtLeaveDescription.Text == "Week Days Only"
                        & strPrevCheckedOption != "W"
                        & strPrevCheckedOption != "")
                    {
                        strErrorText = "The Formula to Calculate 'Normal Leave and Sick Leave' Days has changed from\n" + strCheckedPrevOptionDesc + " to 'Weekdays Only'";
                        strErrorText += "\n\nAll Active 'Normal Leave and Sick Leave' Records Need to be Fixed Accordingly.";
                    }
                    else
                    {
                        if (this.txtLeaveDescription.Text == "Week Days + Saturday"
                            & strPrevCheckedOption != "B"
                            & strPrevCheckedOption != "")
                        {
                            strErrorText = "The Formula to Calculate 'Normal Leave and Sick Leave' Days has changed from\n" + strCheckedPrevOptionDesc + " to 'Weekdays and Saturdays'";
                            strErrorText += "\n\nAll Active 'Normal Leave and Sick Leave' Records Need to be Fixed Accordingly.";
                        }
                        else
                        {
                            if (this.txtLeaveDescription.Text == "ALL Days"
                                & strPrevCheckedOption != "A"
                                & strPrevCheckedOption != "")
                            {
                                strErrorText = "The Formula to Calculate 'Normal Leave and Sick Leave' Days has changed from\n" + strCheckedPrevOptionDesc + " to 'All Days.'";
                                strErrorText += "\n\nAll Active 'Normal Leave and Sick Leave' Records Need to be Fixed Accordingly.";
                            }
                            else
                            {
                                if (this.txtLeaveDescription.Text == ""
                                    & strPrevCheckedOption != "")
                                {
                                    strErrorText = "The Formula to Calculate 'Normal Leave and Sick Leave' Days has changed from\n" + strCheckedPrevOptionDesc + " to 'No Days'.";
                                    strErrorText += "\n\nAll Active 'Normal Leave and Sick Leave' Records will be DELETED.\n\nNB NB To Backout use Refresh Button.";
                                }
                            }
                        }
                    }

                    if (strErrorText != "")
                    {
                        CustomMessageBox.Show(strErrorText,
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        //if (pvtEmployeeLeaveDataView != null)
                        //{
                        //    for (int intRow = 0; intRow < pvtLeaveTypeDataView.Count; intRow++)
                        //    {
                        //        pvtEmployeeLeaveDataView = null;
                        //        pvtEmployeeLeaveDataView = new DataView(pvtDataSet.Tables["EmployeeLeave"],
                        //            "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND EARNING_NO = " + pvtLeaveTypeDataView[intRow]["EARNING_NO"],
                        //            "",
                        //            DataViewRowState.CurrentRows);

                        //    cboLeaveShift_Continue:

                        //        for (int intRow1 = 0; intRow1 < pvtEmployeeLeaveDataView.Count; intRow1++)
                        //        {
                        //            if (pvtEmployeeLeaveDataView[intRow1]["PROCESS_NO"].ToString() != "99")
                        //            {
                        //                if (cboLeaveShift.SelectedIndex == 0)
                        //                {
                        //                    pvtEmployeeLeaveDataView[intRow1].Delete();
                        //                    goto cboLeaveShift_Continue;
                        //                }
                        //            }
                        //        }
                        //    }

                        //    //NB ERROL TO HAVE A LOOK AT THIS
                        //    //NB ERROL TO HAVE A LOOK AT THIS
                        //    //NB ERROL TO HAVE A LOOK AT THIS
                        //    //NB ERROL TO HAVE A LOOK AT THIS

                        //    this.Set_DataGridView_SelectedRowIndex(this.dgvLeaveTypeDataGridView,this.Get_DataGridView_SelectedRowIndex(this.dgvLeaveTypeDataGridView));
                        //}
                    }

                    pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_SHIFT_NO"] = Convert.ToInt32(pvtLeaveLinkDataView[this.cboLeaveShift.SelectedIndex]["LEAVE_SHIFT_NO"]);
                    pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_PAID_ACCUMULATOR_IND"] = Convert.ToInt32(pvtLeaveLinkDataView[this.cboLeaveShift.SelectedIndex]["LEAVE_PAID_ACCUMULATOR_IND"]);

                    string strDayFilter = "";

                    //ERROL TO CHECK LOGIC
                    if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_PAID_ACCUMULATOR_IND"].ToString() == "1")
                    {
                        //Week Days Only
                        strDayFilter = " AND DAY_NO IN(1,2,3,4,5)";
                    }
                    else
                    {
                        if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_PAID_ACCUMULATOR_IND"].ToString() == "2")
                        {
                            //Week Days + Saturday
                            strDayFilter = " AND DAY_NO IN(1,2,3,4,5,6)";
                        }
                    }

                    DataView PayCategoryTimeDecimalDataView = new DataView(pvtDataSet.Tables["PayCategoryTimeDecimal"],
                    "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_NO = " + pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_PAY_CATEGORY_NO"].ToString() + strDayFilter,
                    "",
                    DataViewRowState.CurrentRows);

                    double dblLeaveDayRate = 0;

                    for (int intRow = 0; intRow < PayCategoryTimeDecimalDataView.Count; intRow++)
                    {
                        dblLeaveDayRate += Convert.ToDouble(PayCategoryTimeDecimalDataView[intRow]["TIME_DECIMAL"]);
                    }

                    dblLeaveDayRate = Math.Round(dblLeaveDayRate / PayCategoryTimeDecimalDataView.Count, 2);

                    this.txtLeaveDayRate.Text = dblLeaveDayRate.ToString("###0.00");

                    DataView EmployeePayCategoryDataView = new DataView(pvtDataSet.Tables["EmployeePayCategory"],
                      "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND EMPLOYEE_NO = " + pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_NO"].ToString() + " AND DEFAULT_IND = 'Y'",
                      "",
                      DataViewRowState.CurrentRows);

                    if (EmployeePayCategoryDataView.Count > 0)
                    {
                        EmployeePayCategoryDataView[0]["LEAVE_DAY_RATE_DECIMAL"] = dblLeaveDayRate;
                    }

                    //ERROL TO CHECK LOGIC
                    if (pvtLeaveLinkDataView[this.cboLeaveShift.SelectedIndex]["LEAVE_PAID_ACCUMULATOR_IND"].ToString() == "1")
                    {
                        if (PayCategoryTimeDecimalDataView.Count == 5)
                        {
                            this.grbLeaveError.Visible = false;

                            pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_CONFIG_ERROR_IND"] = "N";

                            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                            {
                                if (Convert.ToInt32(pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_PAY_CATEGORY_NO"]) == 0
                                | Convert.ToInt32(pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_PAID_ACCUMULATOR_IND"]) == 0)
                                {
                                    this.dgvEmployeeDataGridView[0,pvtintEmployeeDataGridViewRowIndex].Style = ErrorDataGridViewCellStyle;
                                }
                                else
                                {
                                    this.dgvEmployeeDataGridView[0,pvtintEmployeeDataGridViewRowIndex].Style = NormalDataGridViewCellStyle;
                                }
                            }
                        }
                        else
                        {
                            //Not New Record
                            if (PayCategoryTimeDecimalDataView.Count > 0)
                            {
                                pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_CONFIG_ERROR_IND"] = "Y";
                            }

                            if (this.btnSave.Enabled == false)
                            {
                                this.grbLeaveError.Visible = true;

                                this.dgvEmployeeDataGridView[0,pvtintEmployeeDataGridViewRowIndex].Style = ErrorDataGridViewCellStyle;
                            }
                        }
                    }
                    else
                    {
                        if (pvtLeaveLinkDataView[this.cboLeaveShift.SelectedIndex]["LEAVE_PAID_ACCUMULATOR_IND"].ToString() == "2")
                        {
                            if (PayCategoryTimeDecimalDataView.Count == 6)
                            {
                                this.grbLeaveError.Visible = false;

                                pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_CONFIG_ERROR_IND"] = "N";

                                if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                                {
                                    if (Convert.ToInt32(pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_PAY_CATEGORY_NO"]) == 0
                                    | Convert.ToInt32(pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_PAID_ACCUMULATOR_IND"]) == 0)
                                    {
                                        this.dgvEmployeeDataGridView[0,pvtintEmployeeDataGridViewRowIndex].Style = ErrorDataGridViewCellStyle;
                                    }
                                    else
                                    {
                                        this.dgvEmployeeDataGridView[0,pvtintEmployeeDataGridViewRowIndex].Style = NormalDataGridViewCellStyle;
                                    }
                                }
                            }
                            else
                            {
                                if (this.btnSave.Enabled == false)
                                {
                                    this.grbLeaveError.Visible = true;
                                    this.dgvEmployeeDataGridView[0,pvtintEmployeeDataGridViewRowIndex].Style = ErrorDataGridViewCellStyle;
                                }

                                pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_CONFIG_ERROR_IND"] = "Y";
                            }
                        }
                        else
                        {
                            //All Days
                            if (PayCategoryTimeDecimalDataView.Count == 7)
                            {
                                this.grbLeaveError.Visible = false;

                                pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_CONFIG_ERROR_IND"] = "N";

                                if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                                {
                                    if (Convert.ToInt32(pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_PAY_CATEGORY_NO"]) == 0
                                    | Convert.ToInt32(pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_PAID_ACCUMULATOR_IND"]) == 0)
                                    {
                                        this.dgvEmployeeDataGridView[0,pvtintEmployeeDataGridViewRowIndex].Style = ErrorDataGridViewCellStyle;
                                    }
                                    else
                                    {
                                        this.dgvEmployeeDataGridView[0,pvtintEmployeeDataGridViewRowIndex].Style = NormalDataGridViewCellStyle;
                                    }
                                }
                            }
                            else
                            {
                                pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_CONFIG_ERROR_IND"] = "Y";

                                if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                                {
                                    this.grbLeaveError.Visible = true;

                                    this.dgvEmployeeDataGridView[0,pvtintEmployeeDataGridViewRowIndex].Style = ErrorDataGridViewCellStyle;
                                }
                            }
                        }
                    }

                    Calculate_Leave_Totals();
                }
            }
        }

        private void btnEarningAdd_Click(object sender, EventArgs e)
        {
            if (this.dgvEarningDataGridView.Rows.Count > 0)
            {
                int intEarningNo = Convert.ToInt32(this.dgvEarningDataGridView[pvtintEarningsEarningNoCol, this.Get_DataGridView_SelectedRowIndex(this.dgvEarningDataGridView)].Value);

                DataView myEarningDataView = new DataView(pvtDataSet.Tables["Earning"],
                        pvtstrFilter + " AND EARNING_NO = " + intEarningNo,
                        "",
                DataViewRowState.CurrentRows);

                //Add Row
                DataRowView drvDataRowView = this.pvtEmployeeEarningDataView.AddNew();

                drvDataRowView.BeginEdit();

                Add_Employee_Earning_Fields(drvDataRowView, Convert.ToInt32(myEarningDataView[0]["EARNING_NO"]));

                if (myEarningDataView[0]["IRP5_CODE"].ToString() != "")
                {
                    drvDataRowView["IRP5_CODE"] = Convert.ToInt32(myEarningDataView[0]["IRP5_CODE"]);
                }

                drvDataRowView["EARNING_TYPE_IND"] = myEarningDataView[0]["EARNING_TYPE_IND"].ToString();
                drvDataRowView["EARNING_PERIOD_IND"] = myEarningDataView[0]["EARNING_PERIOD_IND"].ToString();
                drvDataRowView["EARNING_DAY_VALUE"] = myEarningDataView[0]["EARNING_DAY_VALUE"].ToString();
                drvDataRowView["AMOUNT"] = myEarningDataView[0]["AMOUNT"].ToString();

                drvDataRowView.EndEdit();

                pvtintChosenEarningDataGridViewRowIndex = -1;

                //Stop SpreadSheet RowEnter Firing
                this.pvtblnEarningDataGridViewLoaded = false;
                this.pvtblnChosenEarningDataGridViewLoaded = false;

                DataGridViewRow myDataGridViewRow = this.dgvEarningDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvEarningDataGridView)];

                this.dgvEarningDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvChosenEarningDataGridView.Rows.Add(myDataGridViewRow);

                this.pvtblnEarningDataGridViewLoaded = true;
                this.pvtblnChosenEarningDataGridViewLoaded = true;

                if (this.dgvEarningDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvEarningDataGridView, 0);
                }

                this.Set_DataGridView_SelectedRowIndex(this.dgvChosenEarningDataGridView, this.dgvChosenEarningDataGridView.Rows.Count - 1);
            }
        }

        private void btnEarningRemove_Click(object sender, EventArgs e)
        {
            if (this.dgvChosenEarningDataGridView.Rows.Count > 0)
            {
                int intEarningNo = Convert.ToInt32(this.dgvChosenEarningDataGridView[pvtintEarningsEarningNoCol, pvtintChosenEarningDataGridViewRowIndex].Value);

                if (intEarningNo < 10
                || intEarningNo >= 200)
                {
                    //Not Allowed 
                    CustomMessageBox.Show("Earning '" + this.dgvChosenEarningDataGridView[pvtintEarningsDescCol, pvtintChosenEarningDataGridViewRowIndex].Value.ToString() + "' cannot be Removed.\n\nIt is a System Defined Earning.",
                       this.Text,
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Information);
                }
                else
                {
                    pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex].Delete();

                    pvtintChosenEarningDataGridViewRowIndex = -1;

                    //Stop SpreadSheet RowEnter Firing
                    this.pvtblnEarningDataGridViewLoaded = false;
                    this.pvtblnChosenEarningDataGridViewLoaded = false;

                    DataGridViewRow myDataGridViewRow = this.dgvChosenEarningDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvChosenEarningDataGridView)];

                    this.dgvChosenEarningDataGridView.Rows.Remove(myDataGridViewRow);

                    this.dgvEarningDataGridView.Rows.Add(myDataGridViewRow);

                    this.pvtblnEarningDataGridViewLoaded = true;
                    this.pvtblnChosenEarningDataGridViewLoaded = true;

                    if (this.dgvChosenEarningDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvChosenEarningDataGridView, 0);
                    }

                    this.Set_DataGridView_SelectedRowIndex(this.dgvEarningDataGridView, this.dgvEarningDataGridView.Rows.Count - 1);
                }
            }
        }

        private void btnResetDefaultEarning_Click(object sender, EventArgs e)
        {
            this.btnResetDefaultEarning.Visible = false;
            this.lblResetDefaultEarning.Visible = false;

            DataView EarningDefaultDataView = new DataView(pvtDataSet.Tables["Earning"],
                 pvtstrFilter + " AND EARNING_NO = " + pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_NO"].ToString(),
                "",
                DataViewRowState.CurrentRows);

            if (EarningDefaultDataView.Count > 0)
            {
                pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_TYPE_IND"] = EarningDefaultDataView[0]["EARNING_TYPE_IND"].ToString();

                pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_PERIOD_IND"] = EarningDefaultDataView[0]["EARNING_PERIOD_IND"].ToString();
                pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_DAY_VALUE"] = EarningDefaultDataView[0]["EARNING_DAY_VALUE"].ToString();
                
                pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["AMOUNT"] = Convert.ToDouble(EarningDefaultDataView[0]["AMOUNT"]);

                //Refire Row
                this.Set_DataGridView_SelectedRowIndex(this.dgvChosenEarningDataGridView,this.Get_DataGridView_SelectedRowIndex(this.dgvChosenEarningDataGridView));
            }
        }

        private void EarningType_Click(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.rbnEarningEachPayPeriod.Checked = true;
                this.txtEarningPeriodAmt.Text = "0.00";

                this.cboEarningDay.Enabled = false;
                this.cboEarningDay.SelectedIndex = -1;

                pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_PERIOD_IND"] = "E";
                pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_DAY_VALUE"] = 0;

                RadioButton myRadioButton = (RadioButton)sender;

                if (myRadioButton.Name == "rbnEarningUserToEnterValue")
                {
                    pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_TYPE_IND"] = "U";

                    this.txtEarningPeriodAmt.Enabled = false;

                    this.rbnEarningMonthly.Enabled = false;
                    rbnEarningEachPayPeriod.Enabled = false;
                }
                else
                {
                    this.rbnEarningMonthly.Enabled = true;
                    this.rbnEarningEachPayPeriod.Enabled = true;
                    this.txtEarningPeriodAmt.Enabled = true;

                    if (myRadioButton.Name == "rbnEarningMultiple")
                    {
                        pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_TYPE_IND"] = "X";
                        this.txtEarningPeriodAmt.Text = "0.0000";
                    }
                    else
                    {
                        //Fixed Value
                        pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_TYPE_IND"] = "F";
                    }
                }

                Check_If_Earning_Default();
            }
        }

        private void txtEarningPeriodAmt_TextChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (txtEarningPeriodAmt.Text.Replace(".", "").Trim() == "")
                {
                    pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["AMOUNT"] = 0;
                }
                else
                {
                    if (Convert.ToDouble(pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["AMOUNT"]) != Convert.ToDouble(this.txtEarningPeriodAmt.Text))
                    {
                        pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["AMOUNT"] = Convert.ToDouble(this.txtEarningPeriodAmt.Text);
                        this.dgvChosenEarningDataGridView[pvtintEarningsPeriodAmtCol, pvtintChosenEarningDataGridViewRowIndex].Value = this.txtEarningPeriodAmt.Text;
                    }
                }

                Check_If_Earning_Default();
            }
        }

        private void txtRate_Leave(object sender, EventArgs e)
        {
            //Numeric Field NB DataLayer will Change (TextBoxChanged Event Fires)
            if (txtRate.Text.Trim().Replace(".", "") == "")
            {
                this.txtRate.Text = "0.00";
            }
            else
            {
                this.txtRate.Text = Convert.ToDouble(this.txtRate.Text).ToString("######0.00");
            }

            DataView EmployeePayCategoryDataView = new DataView(pvtDataSet.Tables["EmployeePayCategory"],
                "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND EMPLOYEE_NO = " + this.pvtintEmployeeNo + " AND PAY_CATEGORY_NO = " + this.dgvChosenPayCategoryDataGridView[pvtintPayCategoryNoCol, this.Get_DataGridView_SelectedRowIndex(this.dgvChosenPayCategoryDataGridView)].Value.ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                "",
                 DataViewRowState.CurrentRows);

            EmployeePayCategoryDataView[0]["HOURLY_RATE"] = Convert.ToDouble(this.txtRate.Text);
        }

        private void EarningPayPeriod_Click(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                RadioButton myRadioButton = (RadioButton)sender;

                if (myRadioButton.Name == "rbnEarningEachPayPeriod")
                {
                    pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_DAY_VALUE"] = 0;
                    pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_PERIOD_IND"] = "E";

                    this.cboEarningDay.Enabled = false;
                    this.cboEarningDay.SelectedIndex = -1;
                }
                else
                {
                    //Monthly
                    pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_PERIOD_IND"] = "M";
                    this.cboEarningDay.Enabled = true;
                }

                Check_If_Earning_Default();
            }
        }

        private void cboEarningDay_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                int intValue = 0;

                if (this.cboEarningDay.SelectedIndex == this.cboEarningDay.Items.Count - 1)
                {
                    intValue = 99;
                }
                else
                {
                    intValue = this.cboEarningDay.SelectedIndex + 1;
                }

                if (Convert.ToInt32(pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_DAY_VALUE"]) != intValue)
                {
                    pvtEmployeeEarningDataView[pvtintEmployeeEarningDataViewIndex]["EARNING_DAY_VALUE"] = intValue;
                }

                Check_If_Earning_Default();
            }
        }

        private void btnDeductionAdd_Click(object sender, EventArgs e)
        {
            if (this.dgvDeductionDataGridView.Rows.Count > 0)
            {
                int intDeductionNo = Convert.ToInt32(this.dgvDeductionDataGridView[pvtintDeductionsDeductionNoCol, this.Get_DataGridView_SelectedRowIndex(this.dgvDeductionDataGridView)].Value);
                int intDeductionSubNo = Convert.ToInt32(this.dgvDeductionDataGridView[pvtintDeductionsDeductionSubNoCol, this.Get_DataGridView_SelectedRowIndex(this.dgvDeductionDataGridView)].Value);

                DataView myDeductionDataView = new DataView(pvtDataSet.Tables["Deduction"],
                        pvtstrFilter + " AND DEDUCTION_NO = " + intDeductionNo.ToString() + " AND DEDUCTION_SUB_ACCOUNT_NO = " + intDeductionSubNo.ToString(),
                        "",
                        DataViewRowState.CurrentRows);

                //Add Row
                DataRowView drvDataRowView = pvtEmployeeDeductionDataView.AddNew();

                drvDataRowView.BeginEdit();

                //Set Key for Find
                drvDataRowView["COMPANY_NO"] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo"));
                drvDataRowView["PAY_CATEGORY_TYPE"] = pvtstrPayrollType;
                drvDataRowView["EMPLOYEE_NO"] = this.pvtintEmployeeNo;
                drvDataRowView["DEDUCTION_NO"] = Convert.ToInt32(myDeductionDataView[0]["DEDUCTION_NO"]);
                drvDataRowView["DEDUCTION_SUB_ACCOUNT_NO"] = Convert.ToInt32(myDeductionDataView[0]["DEDUCTION_SUB_ACCOUNT_NO"]);
                drvDataRowView["TIE_BREAKER"] = 1;
                drvDataRowView["DEDUCTION_TYPE_IND"] = myDeductionDataView[0]["DEDUCTION_TYPE_IND"].ToString();
                drvDataRowView["DEDUCTION_VALUE"] = Convert.ToDouble(myDeductionDataView[0]["DEDUCTION_VALUE"]);
                drvDataRowView["DEDUCTION_MIN_VALUE"] = Convert.ToDouble(myDeductionDataView[0]["DEDUCTION_MIN_VALUE"]);
                drvDataRowView["DEDUCTION_MAX_VALUE"] = Convert.ToDouble(myDeductionDataView[0]["DEDUCTION_MAX_VALUE"]);
                drvDataRowView["DEDUCTION_PERIOD_IND"] = myDeductionDataView[0]["DEDUCTION_PERIOD_IND"].ToString();
                drvDataRowView["DEDUCTION_DAY_VALUE"] = Convert.ToDouble(myDeductionDataView[0]["DEDUCTION_DAY_VALUE"]);
                drvDataRowView["DEDUCTION_LOAN_TYPE_IND"] = myDeductionDataView[0]["DEDUCTION_LOAN_TYPE_IND"].ToString();
                drvDataRowView["DEDUCTION_SUB_ACCOUNT_COUNT"] = Convert.ToInt32(myDeductionDataView[0]["DEDUCTION_SUB_ACCOUNT_COUNT"]);
                drvDataRowView["LOAN_OUTSTANDING"] = 0;

                drvDataRowView.EndEdit();

                if (myDeductionDataView[0]["DEDUCTION_TYPE_IND"].ToString() == "P")
                {
                    DataView DeductionEarningPercentageDefaultDataView = new DataView(pvtDataSet.Tables["DeductionEarningPercentageDefault"],
                    pvtstrFilter + " AND DEDUCTION_NO = " + intDeductionNo + " AND DEDUCTION_SUB_ACCOUNT_NO = " + intDeductionSubNo,
                    "",
                    DataViewRowState.CurrentRows);

                    for (int intIndex = 0; intIndex < DeductionEarningPercentageDefaultDataView.Count; intIndex++)
                    {
                        pvtdrvDataRowView = this.pvtDeductionEarningPercentageDataView.AddNew();

                        pvtdrvDataRowView["COMPANY_NO"] = Convert.ToInt32(Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")));
                        pvtdrvDataRowView["PAY_CATEGORY_TYPE"] = pvtstrPayrollType;
                        pvtdrvDataRowView["EMPLOYEE_NO"] = this.pvtintEmployeeNo;
                        pvtdrvDataRowView["DEDUCTION_NO"] = Convert.ToInt32(DeductionEarningPercentageDefaultDataView[intIndex]["DEDUCTION_NO"]);
                        pvtdrvDataRowView["DEDUCTION_SUB_ACCOUNT_NO"] = Convert.ToInt32(DeductionEarningPercentageDefaultDataView[intIndex]["DEDUCTION_SUB_ACCOUNT_NO"]);
                        pvtdrvDataRowView["EARNING_NO"] = Convert.ToInt32(DeductionEarningPercentageDefaultDataView[intIndex]["EARNING_NO"]);

                        pvtdrvDataRowView.EndEdit();
                    }
                }

                pvtintChosenDeductionDataGridViewRowIndex = -1;

                //Stop SpreadSheet RowEnter Firing
                this.pvtblnDeductionDataGridViewLoaded = false;
                this.pvtblnChosenDeductionDataGridViewLoaded = false;

                DataGridViewRow myDataGridViewRow = this.dgvDeductionDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvDeductionDataGridView)];

                this.dgvDeductionDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvChosenDeductionDataGridView.Rows.Add(myDataGridViewRow);

                this.pvtblnDeductionDataGridViewLoaded = true;
                this.pvtblnChosenDeductionDataGridViewLoaded = true;

                if (this.dgvDeductionDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvDeductionDataGridView, 0);
                }

                this.Set_DataGridView_SelectedRowIndex(this.dgvChosenDeductionDataGridView, this.dgvChosenDeductionDataGridView.Rows.Count - 1);
            }
        }

        private void btnDeductionRemove_Click(object sender, EventArgs e)
        {
            if (this.dgvChosenDeductionDataGridView.Rows.Count > 0)
            {
                //Tax / UIF
                //if (pvtintDeductionAccountNo < 3)
                //{
                //    //Not Allowed 
                //    MessageBox.Show("Earning '" + this.dgvChosenDeductionDataGridView[pvtintDeductionsDescCol, pvtintChosenDeductionDataGridViewRowIndex].Value.ToString() + "' cannot be Removed.\n\nIt is a System Defined Earning.",
                //       this.Text,
                //       MessageBoxButtons.OK,
                //       MessageBoxIcon.Information);
                //}
                //else
                //{
                    pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex].Delete();

                    //Clear Possible DeductionEarningPercentage Records
                    pvtDeductionEarningPercentageDataView = null;
                    pvtDeductionEarningPercentageDataView = new DataView(pvtDataSet.Tables["DeductionEarningPercentage"],
                        pvtstrFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND DEDUCTION_NO = " + pvtintDeductionAccountNo.ToString() + " AND DEDUCTION_SUB_ACCOUNT_NO = " + pvtintDeductionSubAccountNo.ToString(),
                        "EARNING_NO",
                        DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < pvtDeductionEarningPercentageDataView.Count; intRow++)
                    {
                        pvtDeductionEarningPercentageDataView[0].Delete();

                        intRow -= 1;
                    }

                    pvtintChosenDeductionDataGridViewRowIndex = -1;

                    //Stop SpreadSheet RowEnter Firing
                    this.pvtblnDeductionDataGridViewLoaded = false;
                    this.pvtblnChosenDeductionDataGridViewLoaded = false;

                    DataGridViewRow myDataGridViewRow = this.dgvChosenDeductionDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvChosenDeductionDataGridView)];

                    this.dgvChosenDeductionDataGridView.Rows.Remove(myDataGridViewRow);

                    this.dgvDeductionDataGridView.Rows.Add(myDataGridViewRow);

                    this.pvtblnDeductionDataGridViewLoaded = true;
                    this.pvtblnChosenDeductionDataGridViewLoaded = true;

                    if (this.dgvChosenDeductionDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvChosenDeductionDataGridView, 0);
                    }

                    this.Set_DataGridView_SelectedRowIndex(this.dgvDeductionDataGridView, this.dgvDeductionDataGridView.Rows.Count - 1);
                //}
            }
        }

        private void DeductType_Click(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {

                bool blnEnabled = dgvChosenDeductionEarningLinkDataGridView.Enabled;

                if (this.dgvChosenDeductionEarningLinkDataGridView.EditMode != DataGridViewEditMode.EditOnEnter)
                {

                }

                

                //Remove Red Marker For Percenatge Earning Select
                clsISUtilities.NotDataBound_Control_Paint_Remove(lblPercentEarnings);

                this.Clear_DataGridView(this.dgvChosenDeductionEarningLinkDataGridView);

                //Clear Possible DeductionEarningPercentage Records
                pvtDeductionEarningPercentageDataView = null;
                pvtDeductionEarningPercentageDataView = new DataView(pvtDataSet.Tables["DeductionEarningPercentage"],
                    pvtstrFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND DEDUCTION_NO = " + pvtintDeductionAccountNo.ToString() + " AND DEDUCTION_SUB_ACCOUNT_NO = " + pvtintDeductionSubAccountNo.ToString(),
                    "EARNING_NO",
                    DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < pvtDeductionEarningPercentageDataView.Count; intRow++)
                {
                    pvtDeductionEarningPercentageDataView[0].Delete();

                    intRow -= 1;
                }

                this.rbnDeductEachPayPeriod.Checked = true;
                pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_PERIOD_IND"] = "E";

                this.txtDeductValue.Text = "0.00";
                this.txtDeductMinValue.Text = "0.00";
                this.txtDeductMaxValue.Text = "0.00";

                pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_VALUE"] = 0;
                pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_MIN_VALUE"] = 0;
                pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_MAX_VALUE"] = 0;

                this.cboDeductDay.SelectedIndex = -1;
                pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_DAY_VALUE"] = 0;

                this.cboDeductDay.Enabled = false;

                RadioButton myRadioButton = (RadioButton)sender;

                if (myRadioButton.Name == "rbnDeductUserToEnter")
                {
                    this.rbnDeductEachPayPeriod.Enabled = false;
                    this.rbnDeductMonthly.Enabled = false;

                    pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_TYPE_IND"] = "U";

                    this.txtDeductValue.Enabled = false;
                    this.txtDeductMinValue.Enabled = false;
                    this.txtDeductMaxValue.Enabled = false;
                }
                else
                {
                    this.rbnDeductEachPayPeriod.Enabled = true;
                    this.rbnDeductMonthly.Enabled = true;

                    this.txtDeductValue.Enabled = true;

                    if (myRadioButton.Name == "rbnDeductFixedValue")
                    {
                        pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_TYPE_IND"] = "F";

                        this.txtDeductMinValue.Enabled = false;
                        this.txtDeductMaxValue.Enabled = false;
                    }
                    else
                    {
                        pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_TYPE_IND"] = "P";

                        this.txtDeductMinValue.Enabled = true;
                        this.txtDeductMaxValue.Enabled = true;

                        Load_Deduction_Earning_Percentage_Rows();
                    }
                }

                Check_If_Deduction_Default();
            }
        }

        private void cboDeductDay_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                int intValue = 0;

                if (this.cboDeductDay.SelectedIndex == this.cboDeductDay.Items.Count - 1)
                {
                    intValue = 99;
                }
                else
                {
                    intValue = this.cboDeductDay.SelectedIndex + 1;
                }

                if (Convert.ToInt32(pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_DAY_VALUE"]) != intValue)
                {
                    pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_DAY_VALUE"] = intValue;
                }

                Check_If_Deduction_Default();
            }
        }

        private void txtDeductValue_TextChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (txtDeductValue.Text.Replace(".", "").Trim() == "")
                {
                    if (Convert.ToDouble(pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_VALUE"]) != 0)
                    {
                        pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_VALUE"] = 0;
                    }
                }
                else
                {
                    if (Convert.ToDouble(pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_VALUE"]) != Convert.ToDouble(this.txtDeductValue.Text))
                    {
                        pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_VALUE"] = Convert.ToDouble(this.txtDeductValue.Text);

                        this.dgvChosenDeductionDataGridView[pvtintDeductionsValueCol, pvtintChosenDeductionDataGridViewRowIndex].Value = this.txtDeductValue.Text;
                    }

                }

                Check_If_Deduction_Default();
            }
        }

        private void txtDeductMinValue_TextChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (txtDeductMinValue.Text.Replace(".", "").Trim() == "")
                {
                    pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_MIN_VALUE"] = 0;
                }
                else
                {
                    if (Convert.ToDouble(pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_MIN_VALUE"]) != Convert.ToDouble(this.txtDeductMinValue.Text))
                    {
                        pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_MIN_VALUE"] = Convert.ToDouble(this.txtDeductMinValue.Text);
                    }
                }

                Check_If_Deduction_Default();
            }
        }

        private void txtDeductMaxValue_TextChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (txtDeductMaxValue.Text.Replace(".", "").Trim() == "")
                {
                    pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_MAX_VALUE"] = 0;
                }
                else
                {
                    if (Convert.ToDouble(pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_MAX_VALUE"]) != Convert.ToDouble(this.txtDeductMaxValue.Text))
                    {
                        pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_MAX_VALUE"] = Convert.ToDouble(this.txtDeductMaxValue.Text);
                    }
                }

                Check_If_Deduction_Default();
            }
        }

        private void btnResetDefaultDeduction_Click(object sender, EventArgs e)
        {
            this.btnResetDefaultDeduction.Visible = false;
            this.lblResetDefault.Visible = false;

            DataView DeductionDefaultDataView = new DataView(pvtDataSet.Tables["Deduction"],
                 pvtstrFilter + " AND DEDUCTION_NO = " + pvtintDeductionAccountNo + " AND DEDUCTION_SUB_ACCOUNT_NO = " + pvtintDeductionSubAccountNo,

                "",
                DataViewRowState.CurrentRows);

            if (DeductionDefaultDataView.Count > 0)
            {
                pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_TYPE_IND"] = DeductionDefaultDataView[0]["DEDUCTION_TYPE_IND"].ToString();
                pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_PERIOD_IND"] = DeductionDefaultDataView[0]["DEDUCTION_PERIOD_IND"].ToString();
                pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_DAY_VALUE"] = DeductionDefaultDataView[0]["DEDUCTION_DAY_VALUE"].ToString();
                pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_VALUE"] = Convert.ToDouble(DeductionDefaultDataView[0]["DEDUCTION_VALUE"]);
                pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_MIN_VALUE"] = Convert.ToDouble(DeductionDefaultDataView[0]["DEDUCTION_MIN_VALUE"]);
                pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_MAX_VALUE"] = Convert.ToDouble(DeductionDefaultDataView[0]["DEDUCTION_MAX_VALUE"]);

                //Remove All Own Created Earning Percentage Records
                for (int intRow1 = 0; intRow1 < pvtDeductionEarningPercentageDataView.Count; intRow1++)
                {
                    pvtDeductionEarningPercentageDataView[intRow1].Delete();

                    intRow1 -= 1;
                }

                DataView DeductionEarningPercentageDefaultDataView = new DataView(pvtDataSet.Tables["DeductionEarningPercentageDefault"],
                pvtstrFilter + " AND DEDUCTION_NO = " + pvtintDeductionAccountNo + " AND DEDUCTION_SUB_ACCOUNT_NO = " + pvtintDeductionSubAccountNo,
                "",
                DataViewRowState.CurrentRows);

                for (int intIndex = 0; intIndex < DeductionEarningPercentageDefaultDataView.Count; intIndex++)
                {
                    pvtdrvDataRowView = this.pvtDeductionEarningPercentageDataView.AddNew();

                    pvtdrvDataRowView["COMPANY_NO"] = Convert.ToInt32(Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")));
                    pvtdrvDataRowView["PAY_CATEGORY_TYPE"] = pvtstrPayrollType;
                    pvtdrvDataRowView["EMPLOYEE_NO"] = this.pvtintEmployeeNo;
                    pvtdrvDataRowView["DEDUCTION_NO"] = Convert.ToInt32(DeductionEarningPercentageDefaultDataView[intIndex]["DEDUCTION_NO"]);
                    pvtdrvDataRowView["DEDUCTION_SUB_ACCOUNT_NO"] = Convert.ToInt32(DeductionEarningPercentageDefaultDataView[intIndex]["DEDUCTION_SUB_ACCOUNT_NO"]);
                    pvtdrvDataRowView["EARNING_NO"] = Convert.ToInt32(DeductionEarningPercentageDefaultDataView[intIndex]["EARNING_NO"]);

                    pvtdrvDataRowView.EndEdit();
                }

                //Refire Row
                this.Set_DataGridView_SelectedRowIndex(this.dgvChosenDeductionDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvChosenDeductionDataGridView));
            }
        }

        private void dgvChosenDeductionEarningLinkDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (e.ColumnIndex == 1)
                {
                    bool blnValue = Convert.ToBoolean(dgvChosenDeductionEarningLinkDataGridView[e.ColumnIndex, e.RowIndex].Value);
                    int intEarningNo = Convert.ToInt32(dgvChosenDeductionEarningLinkDataGridView[2, e.RowIndex].Value);

                    if (blnValue == true)
                    {
                        DataRowView drvDataRowView = this.pvtDeductionEarningPercentageDataView.AddNew();

                        drvDataRowView.BeginEdit();

                        //Set Key for Find
                        drvDataRowView["COMPANY_NO"] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo"));
                        drvDataRowView["PAY_CATEGORY_TYPE"] = pvtstrPayrollType;
                        drvDataRowView["EMPLOYEE_NO"] = this.pvtintEmployeeNo;
                        drvDataRowView["DEDUCTION_NO"] = pvtintDeductionAccountNo;
                        drvDataRowView["DEDUCTION_SUB_ACCOUNT_NO"] = pvtintDeductionSubAccountNo;
                        drvDataRowView["EARNING_NO"] = intEarningNo;

                        drvDataRowView.EndEdit();

                        clsISUtilities.NotDataBound_Control_Paint_Remove(lblPercentEarnings);
                    }
                    else
                    {
                        //Remove Specific Earning
                        pvtDeductionEarningPercentageDataView = null;
                        pvtDeductionEarningPercentageDataView = new DataView(pvtDataSet.Tables["DeductionEarningPercentage"],
                            pvtstrFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND EARNING_NO = " + intEarningNo.ToString() + " AND DEDUCTION_NO = " + pvtintDeductionAccountNo.ToString() + " AND DEDUCTION_SUB_ACCOUNT_NO = " + pvtintDeductionSubAccountNo.ToString(),
                            "",
                            DataViewRowState.CurrentRows);

                        pvtDeductionEarningPercentageDataView[0].Delete();

                        //Check All Earnings
                        pvtDeductionEarningPercentageDataView = null;
                        pvtDeductionEarningPercentageDataView = new DataView(pvtDataSet.Tables["DeductionEarningPercentage"],
                            pvtstrFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND DEDUCTION_NO = " + pvtintDeductionAccountNo.ToString() + " AND DEDUCTION_SUB_ACCOUNT_NO = " + pvtintDeductionSubAccountNo.ToString(),
                            "",
                            DataViewRowState.CurrentRows);

                        if (pvtDeductionEarningPercentageDataView.Count == 0)
                        {
                            clsISUtilities.NotDataBound_Control_Paint(lblPercentEarnings, "Select 1 or more Earnings.");

                        }
                    }
                }
            }
        }

        private void dgvEmployeeLeaveDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == pvtintLeaveDescCol
            | e.ColumnIndex == pvtintLeaveHoursCol)
            {
                //Find Current Record
                int intLeaveRecNo = Convert.ToInt32(dgvEmployeeLeaveDataGridView[pvtintLeaveRecNoCol, e.RowIndex].Value);

                DataView CurrentEmployeeLeave = new DataView(pvtDataSet.Tables["EmployeeLeave"],
                    pvtstrFilter + " AND EMPLOYEE_NO = " + this.pvtintEmployeeNo.ToString() + " AND EARNING_NO = " + this.dgvLeaveTypeDataGridView[pvtintLeaveTypeEarningNoCol, pvtLeaveTypeDataGridViewRowIndex].Value.ToString() + " AND LEAVE_REC_NO = " + intLeaveRecNo.ToString(),
                    "",
                    DataViewRowState.CurrentRows);

                if (e.ColumnIndex == pvtintLeaveDescCol)
                {
                    //Leave Description
                    if (dgvEmployeeLeaveDataGridView[e.ColumnIndex, e.RowIndex].Value == null)
                    {
                        CurrentEmployeeLeave[0]["LEAVE_DESC"] = "";
                    }
                    else
                    {
                        if (CurrentEmployeeLeave[0]["LEAVE_DESC"].ToString() != dgvEmployeeLeaveDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Trim())
                        {
                            CurrentEmployeeLeave[0]["LEAVE_DESC"] = dgvEmployeeLeaveDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Trim();
                        }
                    }
                }
                else
                {
                    if (CurrentEmployeeLeave[0]["LEAVE_OPTION"].ToString() == "H")
                    {
                        //Hours Value - Uses Hour/s Option
                        if (dgvEmployeeLeaveDataGridView[e.ColumnIndex, e.RowIndex].Value == System.DBNull.Value)
                        {
                            this.dgvEmployeeLeaveDataGridView[0,e.RowIndex].Style = this.ErrorDataGridViewCellStyle;
                        }
                        else
                        {
                            if (dgvEmployeeLeaveDataGridView[e.ColumnIndex, e.RowIndex].Value != null)
                            {
                                if (dgvEmployeeLeaveDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString() != ".")
                                {
                                    if (Convert.ToDouble(CurrentEmployeeLeave[0]["LEAVE_HOURS_DECIMAL"]) != Convert.ToDouble(dgvEmployeeLeaveDataGridView[e.ColumnIndex, e.RowIndex].Value))
                                    {
                                        string strDayFilter = "";
                                        double dblPortionOfDay = 0;

                                        //ERROL TO CHECK LOGIC
                                        if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_PAID_ACCUMULATOR_IND"].ToString() == "1")
                                        {
                                            //Week Days Only
                                            strDayFilter = " AND DAY_NO IN(1,2,3,4,5)";
                                        }
                                        else
                                        {
                                            if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_PAID_ACCUMULATOR_IND"].ToString() == "2")
                                            {
                                                //Week Days + Saturday
                                                strDayFilter = " AND DAY_NO IN(1,2,3,4,5,6)";
                                            }
                                        }

                                        if (this.dgvEmployeeLeaveDataGridView[pvtintLeaveFromDateCol, e.RowIndex].Value != null
                                            & this.dgvEmployeeLeaveDataGridView[pvtintLeaveFromDateCol, e.RowIndex].Value.ToString() != "")
                                        {
                                            DateTime dtFromDateTime = DateTime.ParseExact(this.dgvEmployeeLeaveDataGridView[pvtintLeaveFromDateCol, e.RowIndex].Value.ToString(), AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);

                                            DataView PayCategoryTimeDecimalDataView = new DataView(pvtDataSet.Tables["PayCategoryTimeDecimal"],
                                            "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_NO = " + pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_PAY_CATEGORY_NO"].ToString() + strDayFilter + " AND DAY_NO = " + Convert.ToInt32(dtFromDateTime.DayOfWeek).ToString(),
                                            "",
                                            DataViewRowState.CurrentRows);

                                            if (PayCategoryTimeDecimalDataView.Count == 0)
                                            {
                                                this.dgvEmployeeLeaveDataGridView[0,e.RowIndex].Style = this.ErrorDataGridViewCellStyle;
                                            }
                                            else
                                            {
                                                int intFindPublicHolidayRow = pvtPublicHolidayDataView.Find(dtFromDateTime.ToString("yyyy-MM-dd"));

                                                if (intFindPublicHolidayRow > -1)
                                                {
                                                    this.dgvEmployeeLeaveDataGridView[0,e.RowIndex].Style = this.ErrorDataGridViewCellStyle;
                                                }
                                                else
                                                {
                                                    dblPortionOfDay = Math.Round(Convert.ToDouble(dgvEmployeeLeaveDataGridView[e.ColumnIndex, e.RowIndex].Value) / Convert.ToDouble(PayCategoryTimeDecimalDataView[0]["TIME_DECIMAL"]), 2);

                                                    this.dgvEmployeeLeaveDataGridView[0,e.RowIndex].Style = this.NormalDataGridViewCellStyle;
                                                }
                                            }

                                            CurrentEmployeeLeave[0]["LEAVE_DAYS_DECIMAL"] = dblPortionOfDay;

                                            dgvEmployeeLeaveDataGridView[pvtintLeaveDaysCol, e.RowIndex].Value = dblPortionOfDay.ToString("#0.00");
                                        }
                                    }

                                    //2012-08-14
                                    double dblValue = Convert.ToDouble(dgvEmployeeLeaveDataGridView[e.ColumnIndex, e.RowIndex].Value);

                                    dgvEmployeeLeaveDataGridView[e.ColumnIndex, e.RowIndex].Value = dblValue.ToString("#0.00");

                                    CurrentEmployeeLeave[0]["LEAVE_HOURS_DECIMAL"] = Convert.ToDouble(dgvEmployeeLeaveDataGridView[e.ColumnIndex, e.RowIndex].Value);
                                }
                                else
                                {
                                    this.dgvEmployeeLeaveDataGridView[0,e.RowIndex].Style = this.ErrorDataGridViewCellStyle;
                                }
                            }
                        }
                    }
                }

                this.Check_To_Add_New_Leave_Row();
            }
        }

        private void dgvEmployeeLeaveDataGridView_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (e.RowIndex > -1
                & e.ColumnIndex > -1)
                {
                    if (e.ColumnIndex >= pvtintLeaveAccumValueCol
                    || e.ColumnIndex == pvtintLeaveDaysCol)
                    {
                        this.Cursor = Cursors.No;
                    }
                    else
                    {
                        if (e.ColumnIndex == pvtintLeaveToDateCol)
                        {
                            //To Date
                            if (dgvEmployeeLeaveDataGridView[pvtintLeaveOptionCol, e.RowIndex].Value.ToString() == "Hour/s"
                            ||  dgvEmployeeLeaveDataGridView[pvtintLeaveOptionCol, e.RowIndex].Value.ToString() == "Payout")
                            {
                                this.Cursor = Cursors.No;
                            }
                            else
                            {
                                if (dgvEmployeeLeaveDataGridView.Rows[e.RowIndex].ReadOnly == true)
                                {
                                    this.Cursor = Cursors.No;
                                }
                                else
                                {
                                    this.Cursor = Cursors.Default;
                                }
                            }
                        }
                        else
                        {
                            if (e.ColumnIndex == pvtintLeaveHoursCol)
                            {
                                //Hours
                                if (dgvEmployeeLeaveDataGridView[pvtintLeaveOptionCol, e.RowIndex].Value.ToString() == "Hour/s")
                                {
                                    if (dgvEmployeeLeaveDataGridView.Rows[e.RowIndex].ReadOnly == true)
                                    {
                                        this.Cursor = Cursors.No;
                                    }
                                    else
                                    {
                                        this.Cursor = Cursors.Default;
                                    }
                                }
                                else
                                {
                                    this.Cursor = Cursors.No;
                                }
                            }
                            else
                            {
                                if (dgvEmployeeLeaveDataGridView.Rows[e.RowIndex].ReadOnly == true)
                                {
                                    this.Cursor = Cursors.No;
                                }
                                else
                                {
                                    if (dgvEmployeeLeaveDataGridView[e.ColumnIndex, e.RowIndex].ReadOnly == true)
                                    {
                                        this.Cursor = Cursors.No;
                                    }
                                    else
                                    {
                                        this.Cursor = Cursors.Default;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void dgvEmployeeLeaveDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (e.Control is ComboBox)
                {
                    ComboBox myComboBox = (ComboBox)e.Control;

                    if (myComboBox != null)
                    {
                        myComboBox.SelectedIndexChanged -= new EventHandler(Leave_ComboBox_SelectedIndexChanged);
                        myComboBox.SelectedIndexChanged += new EventHandler(Leave_ComboBox_SelectedIndexChanged);
                    }
                }
                else
                {
                    if (e.Control is TextBox)
                    {
                        e.Control.KeyPress -= new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);

                        if (this.dgvEmployeeLeaveDataGridView.CurrentCell.ColumnIndex == pvtintLeaveHoursCol)
                        {
                            e.Control.KeyPress += new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
                        }
                    }
                }
            }
        }

        private void dgvEmployeeLeaveDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (e.ColumnIndex == pvtintLeaveFromDateCol
                | e.ColumnIndex == pvtintLeaveToDateCol)
                {
                    bool blnDayDatesChanged = false;

                    string strDayFilter = "";
                    int intFindPublicHolidayRow = -1;
                    int intFindPayCategoryTimeDecimalRow = -1;

                    DateTime myDateTime;

                    try
                    {
                        myDateTime = DateTime.ParseExact(dgvEmployeeLeaveDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString(), AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);
                    }
                    catch
                    {
                        dgvEmployeeLeaveDataGridView[e.ColumnIndex, e.RowIndex].Value = "";
                        goto dgvEmployeeLeaveDataGridView_CellValueChanged_Error;
                    }

                    int intCurrentEmployeeLeaveRow = this.Get_DataGridView_SelectedRowIndex(this.dgvEmployeeLeaveDataGridView);

                    //Find Current Record
                    int intLeaveRecNo = Convert.ToInt32(dgvEmployeeLeaveDataGridView[pvtintLeaveRecNoCol, intCurrentEmployeeLeaveRow].Value);

                    DataView CurrentEmployeeLeave = new DataView(pvtDataSet.Tables["EmployeeLeave"],
                        pvtstrFilter + " AND EMPLOYEE_NO = " + this.pvtintEmployeeNo.ToString() + " AND EARNING_NO = " + this.dgvLeaveTypeDataGridView[pvtintLeaveTypeEarningNoCol, pvtLeaveTypeDataGridViewRowIndex].Value.ToString() + " AND LEAVE_REC_NO = " + intLeaveRecNo.ToString(),
                        "",
                        DataViewRowState.CurrentRows);

                    if (CurrentEmployeeLeave.Count != 1)
                    {
                        string strStop = "";
                    }

                    if (this.dgvEmployeeLeaveDataGridView.CurrentCell.ColumnIndex == pvtintLeaveFromDateCol)
                    {
                        string strLeaveFromDate = "";

                        if (CurrentEmployeeLeave[0]["LEAVE_FROM_DATE"] != System.DBNull.Value)
                        {
                            strLeaveFromDate = Convert.ToDateTime(CurrentEmployeeLeave[0]["LEAVE_FROM_DATE"]).ToString("yyyy-MM-dd");
                        }

                        if (strLeaveFromDate != myDateTime.ToString("yyyy-MM-dd"))
                        {
                            CurrentEmployeeLeave[0]["LEAVE_FROM_DATE"] = myDateTime.ToString("yyyy-MM-dd");

                            if (CurrentEmployeeLeave[0]["LEAVE_OPTION"].ToString() == "H")
                            {
                                bool blnDayError = false;

                                //ERROL TO CHECK LOGIC
                                if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_PAID_ACCUMULATOR_IND"].ToString() == "1")
                                {
                                    //Week Days Only
                                    strDayFilter = " AND DAY_NO IN(1,2,3,4,5)";
                                }
                                else
                                {
                                    if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_PAID_ACCUMULATOR_IND"].ToString() == "2")
                                    {
                                        //Week Days + Saturday
                                        strDayFilter = " AND DAY_NO IN(1,2,3,4,5,6)";
                                    }
                                }

                                DataView PayCategoryTimeDecimalDataView = new DataView(pvtDataSet.Tables["PayCategoryTimeDecimal"],
                                "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_NO = " + pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_PAY_CATEGORY_NO"].ToString() + strDayFilter,
                                "DAY_NO",
                                DataViewRowState.CurrentRows);

                                intFindPayCategoryTimeDecimalRow = PayCategoryTimeDecimalDataView.Find(Convert.ToInt32(myDateTime.DayOfWeek));

                                if (intFindPayCategoryTimeDecimalRow > -1)
                                {
                                    intFindPublicHolidayRow = pvtPublicHolidayDataView.Find(myDateTime.ToString("yyyy-MM-dd"));

                                    if (intFindPublicHolidayRow > -1)
                                    {
                                        blnDayError = true;
                                    }
                                }
                                else
                                {
                                    blnDayError = true;
                                }

                                //Hours Option ToDate is Same as FromDate
                                this.dgvEmployeeLeaveDataGridView[pvtintLeaveToDateCol, intCurrentEmployeeLeaveRow].Value = myDateTime.ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString());

                                CurrentEmployeeLeave[0]["LEAVE_TO_DATE"] = myDateTime.ToString("yyyy-MM-dd");

                                double dblPortionOfDay = 0;

                                if (blnDayError == false)
                                {
                                    this.dgvEmployeeLeaveDataGridView[0,intCurrentEmployeeLeaveRow].Style = this.NormalDataGridViewCellStyle;

                                    dblPortionOfDay = Math.Round(Convert.ToDouble(dgvEmployeeLeaveDataGridView[pvtintLeaveHoursCol, intCurrentEmployeeLeaveRow].Value) / Convert.ToDouble(PayCategoryTimeDecimalDataView[intFindPayCategoryTimeDecimalRow]["TIME_DECIMAL"]), 2);
                                }
                                else
                                {
                                    this.dgvEmployeeLeaveDataGridView[0,intCurrentEmployeeLeaveRow].Style = this.ErrorDataGridViewCellStyle;
                                }

                                dgvEmployeeLeaveDataGridView[pvtintLeaveDaysCol, intCurrentEmployeeLeaveRow].Value = dblPortionOfDay.ToString("#0.00");

                                CurrentEmployeeLeave[0]["LEAVE_DAYS_DECIMAL"] = dblPortionOfDay;
                            }
                            else
                            {
                                if (CurrentEmployeeLeave[0]["LEAVE_OPTION"].ToString() == ""
                                | CurrentEmployeeLeave[0]["LEAVE_TO_DATE"].ToString() == "")
                                {
                                    //Exit
                                }
                                else
                                {
                                    blnDayDatesChanged = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        string strLeaveToDate = "";

                        if (CurrentEmployeeLeave[0]["LEAVE_TO_DATE"] != System.DBNull.Value)
                        {
                            strLeaveToDate = Convert.ToDateTime(CurrentEmployeeLeave[0]["LEAVE_TO_DATE"]).ToString("yyyy-MM-dd");
                        }

                        if (strLeaveToDate != myDateTime.ToString("yyyy-MM-dd"))
                        {
                            CurrentEmployeeLeave[0]["LEAVE_TO_DATE"] = myDateTime.ToString("yyyy-MM-dd");

                            if (CurrentEmployeeLeave[0]["LEAVE_OPTION"].ToString() == ""
                                | CurrentEmployeeLeave[0]["LEAVE_FROM_DATE"].ToString() == "")
                            {
                                //Exit
                            }
                            else
                            {
                                blnDayDatesChanged = true;
                            }
                        }
                    }

                    if (blnDayDatesChanged == true)
                    {
                        double dblNumberHours = 0;
                        double dblNumberDays = 0;

                        //ERROL TO CHECK LOGIC
                        if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_PAID_ACCUMULATOR_IND"].ToString() == "1")
                        {
                            //Week Days Only
                            strDayFilter = " AND DAY_NO IN(1,2,3,4,5)";
                        }
                        else
                        {
                            if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_PAID_ACCUMULATOR_IND"].ToString() == "2")
                            {
                                //Week Days + Saturday
                                strDayFilter = " AND DAY_NO IN(1,2,3,4,5,6)";
                            }
                        }

                        DateTime dtFromDateTime = Convert.ToDateTime(CurrentEmployeeLeave[0]["LEAVE_FROM_DATE"]);
                        DateTime dtToDateTime = Convert.ToDateTime(CurrentEmployeeLeave[0]["LEAVE_TO_DATE"]);

                        DataView PayCategoryTimeDecimalDataView = new DataView(pvtDataSet.Tables["PayCategoryTimeDecimal"],
                        "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_NO = " + pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_PAY_CATEGORY_NO"].ToString() + strDayFilter,
                        "DAY_NO",
                        DataViewRowState.CurrentRows);

                        int intDaysDiff = dtToDateTime.Subtract(dtFromDateTime).Days + 1;

                        while (dtFromDateTime <= dtToDateTime)
                        {
                            intFindPayCategoryTimeDecimalRow = PayCategoryTimeDecimalDataView.Find(Convert.ToInt32(dtFromDateTime.DayOfWeek));

                            if (intFindPayCategoryTimeDecimalRow > -1)
                            {
                                intFindPublicHolidayRow = pvtPublicHolidayDataView.Find(dtFromDateTime.ToString("yyyy-MM-dd"));

                                if (intFindPublicHolidayRow == -1)
                                {
                                    dblNumberHours += Convert.ToDouble(PayCategoryTimeDecimalDataView[intFindPayCategoryTimeDecimalRow]["TIME_DECIMAL"]);
                                    dblNumberDays += 1;
                                }
                            }

                            dtFromDateTime = dtFromDateTime.AddDays(1);
                        }

                        dgvEmployeeLeaveDataGridView[pvtintLeaveDaysCol, intCurrentEmployeeLeaveRow].Value = dblNumberDays.ToString("#0.00");
                        CurrentEmployeeLeave[0]["LEAVE_DAYS_DECIMAL"] = dblNumberDays;

                        dgvEmployeeLeaveDataGridView[pvtintLeaveHoursCol, intCurrentEmployeeLeaveRow].Value = dblNumberHours.ToString("#0.00");
                        CurrentEmployeeLeave[0]["LEAVE_HOURS_DECIMAL"] = dblNumberHours;

                        CurrentEmployeeLeave[0]["DATE_DIFF_NO_DAYS"] = intDaysDiff;

                        if (dblNumberDays == 0)
                        {
                            this.dgvEmployeeLeaveDataGridView[0,intCurrentEmployeeLeaveRow].Style = this.ErrorDataGridViewCellStyle;
                        }
                        else
                        {
                            this.dgvEmployeeLeaveDataGridView[0,intCurrentEmployeeLeaveRow].Style = this.NormalDataGridViewCellStyle;

                            if (intDaysDiff == Convert.ToInt32(dblNumberDays))
                            {
                                if (CurrentEmployeeLeave[0]["LEAVE_OPTION"].ToString() == "H")
                                {
                                    dgvEmployeeLeaveDataGridView[1, intCurrentEmployeeLeaveRow].Style = this.HoursOptionDataGridViewCellStyle;
                                }
                                else
                                {
                                    dgvEmployeeLeaveDataGridView[1, intCurrentEmployeeLeaveRow].Style = this.NormalDataGridViewCellStyle;
                                }
                            }
                            else
                            {
                                dgvEmployeeLeaveDataGridView[1, intCurrentEmployeeLeaveRow].Style = this.LeaveDaysExcludedDataGridViewCellStyle;
                            }
                        }

                        dgvEmployeeLeaveDataGridView.Refresh();

                        this.Check_To_Add_New_Leave_Row();
                    }
                    else
                    {
                        if (CurrentEmployeeLeave[0]["LEAVE_FROM_DATE"] != System.DBNull.Value
                            & CurrentEmployeeLeave[0]["LEAVE_TO_DATE"] != System.DBNull.Value)
                        {
                            if (Convert.ToDateTime(CurrentEmployeeLeave[0]["LEAVE_FROM_DATE"]) > Convert.ToDateTime(CurrentEmployeeLeave[0]["LEAVE_TO_DATE"]))
                            {
                                this.dgvEmployeeLeaveDataGridView[0,intCurrentEmployeeLeaveRow].Style = this.ErrorDataGridViewCellStyle;
                            }
                            else
                            {
                                this.dgvEmployeeLeaveDataGridView[0,intCurrentEmployeeLeaveRow].Style = this.NormalDataGridViewCellStyle;
                            }
                        }
                    }

                dgvEmployeeLeaveDataGridView_CellValueChanged_Error:

                    int intError = 0;
                }
            }
        }

        private void dgvEmployeeLeaveDataGridView_MouseLeave(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void Leave_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                ComboBox myComboBox = (ComboBox)sender;

                int intCurrentEmployeeLeaveSelectedRowIndex = this.Get_DataGridView_SelectedRowIndex(this.dgvEmployeeLeaveDataGridView);

                //Find Current Record
                int intLeaveRecNo = Convert.ToInt32(dgvEmployeeLeaveDataGridView[pvtintLeaveRecNoCol, intCurrentEmployeeLeaveSelectedRowIndex].Value);

                DataView CurrentEmployeeLeave = new DataView(pvtDataSet.Tables["EmployeeLeave"],
                    pvtstrFilter + " AND EMPLOYEE_NO = " + this.pvtintEmployeeNo.ToString() + " AND EARNING_NO = " + this.dgvLeaveTypeDataGridView[pvtintLeaveTypeEarningNoCol, pvtLeaveTypeDataGridViewRowIndex].Value.ToString() + " AND LEAVE_REC_NO = " + intLeaveRecNo.ToString(),
                    "",
                    DataViewRowState.CurrentRows);

                if (this.dgvEmployeeLeaveDataGridView.CurrentCell.ColumnIndex == pvtintLeaveProcessCol)
                {
                    int intProcessNo = myComboBox.SelectedIndex - 1;

                    if (CurrentEmployeeLeave[0]["PROCESS_NO"] == System.DBNull.Value)
                    {
                        CurrentEmployeeLeave[0]["PROCESS_NO"] = intProcessNo;
                    }
                    else
                    {
                        if (Convert.ToInt32(CurrentEmployeeLeave[0]["PROCESS_NO"]) != intProcessNo)
                        {
                            CurrentEmployeeLeave[0]["PROCESS_NO"] = intProcessNo;
                        }
                    }

                    this.Check_To_Add_New_Leave_Row();
                }
                else
                {
                    this.dgvEmployeeLeaveDataGridView[pvtintLeaveFromDateCol, intCurrentEmployeeLeaveSelectedRowIndex].ReadOnly = false;

                    if (myComboBox.SelectedIndex == 0)
                    {
                        //Day/s
                        CurrentEmployeeLeave[0]["LEAVE_OPTION"] = "D";

                        //UnLock ToDate Cell
                        this.dgvEmployeeLeaveDataGridView[pvtintLeaveToDateCol, intCurrentEmployeeLeaveSelectedRowIndex].ReadOnly = false;

                        //Lock Hours Cell
                        this.dgvEmployeeLeaveDataGridView[pvtintLeaveHoursCol, intCurrentEmployeeLeaveSelectedRowIndex].ReadOnly = true;
                    }
                    else
                    {
                        if (myComboBox.SelectedIndex == 1)
                        {
                            //Hour/s
                            //ToDate = FromDate
                            CurrentEmployeeLeave[0]["LEAVE_OPTION"] = "H";

                            //Lock ToDate Cell
                            this.dgvEmployeeLeaveDataGridView[pvtintLeaveToDateCol, intCurrentEmployeeLeaveSelectedRowIndex].ReadOnly = true;

                            //Unlock Hours Cell
                            this.dgvEmployeeLeaveDataGridView[pvtintLeaveHoursCol, intCurrentEmployeeLeaveSelectedRowIndex].ReadOnly = false;

                            dgvEmployeeLeaveDataGridView[1, intCurrentEmployeeLeaveSelectedRowIndex].Style = this.HoursOptionDataGridViewCellStyle;
                        }
                        else
                        {
                            //Lock ToDate Cell
                            this.dgvEmployeeLeaveDataGridView[pvtintLeaveFromDateCol, intCurrentEmployeeLeaveSelectedRowIndex].ReadOnly = true;
                            this.dgvEmployeeLeaveDataGridView[pvtintLeaveToDateCol, intCurrentEmployeeLeaveSelectedRowIndex].ReadOnly = true;

                            //Lock Hours Cell
                            this.dgvEmployeeLeaveDataGridView[pvtintLeaveHoursCol, intCurrentEmployeeLeaveSelectedRowIndex].ReadOnly = true;

                            this.dgvEmployeeLeaveDataGridView[pvtintLeaveFromDateCol, intCurrentEmployeeLeaveSelectedRowIndex].Value = DateTime.Now.ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString());
                            this.dgvEmployeeLeaveDataGridView[pvtintLeaveToDateCol, intCurrentEmployeeLeaveSelectedRowIndex].Value = DateTime.Now.ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString());
                            
                            CurrentEmployeeLeave[0]["LEAVE_FROM_DATE"] = DateTime.Now.ToString("yyyy-MM-dd");
                            CurrentEmployeeLeave[0]["LEAVE_TO_DATE"] = DateTime.Now.ToString("yyyy-MM-dd");
                            
                            if (this.dgvLeaveTypeDataGridView[pvtintLeaveTypeEarningNoCol, pvtLeaveTypeDataGridViewRowIndex].Value.ToString() == "200")
                            {
                                CurrentEmployeeLeave[0]["LEAVE_OPTION"] = "P";
                            }
                            else
                            {
                                CurrentEmployeeLeave[0]["LEAVE_OPTION"] = "Z";
                            }
                        }
                    }

                    //0 = Day/s 1=Hours/s
                    if (myComboBox.SelectedIndex == 0)
                    {
                        if (this.dgvEmployeeLeaveDataGridView[pvtintLeaveFromDateCol, intCurrentEmployeeLeaveSelectedRowIndex].Value.ToString() != ""
                            & this.dgvEmployeeLeaveDataGridView[pvtintLeaveToDateCol, intCurrentEmployeeLeaveSelectedRowIndex].Value.ToString() != "")
                        {
                            //Errol Changed 2012-09-03
                            DateTime dtFromDateTime = DateTime.ParseExact(this.dgvEmployeeLeaveDataGridView[pvtintLeaveFromDateCol, intCurrentEmployeeLeaveSelectedRowIndex].Value.ToString(), AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);
                            DateTime dtToDateTime = DateTime.ParseExact(this.dgvEmployeeLeaveDataGridView[pvtintLeaveToDateCol, intCurrentEmployeeLeaveSelectedRowIndex].Value.ToString(), AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);

                            string strDayFilter = "";
                            double dblHoursDay = 0;
                            double dblDays = 0;
                            int intFindDayRow = -1;
                            int intFindPublicHolidayRow = -1;

                            //ERROL TO CHECK LOGIC
                            if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_PAID_ACCUMULATOR_IND"].ToString() == "1")
                            {
                                //Week Days Only
                                strDayFilter = " AND DAY_NO IN(1,2,3,4,5)";
                            }
                            else
                            {
                                if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_PAID_ACCUMULATOR_IND"].ToString() == "2")
                                {
                                    //Week Days + Saturday
                                    strDayFilter = " AND DAY_NO IN(1,2,3,4,5,6)";
                                }
                            }

                            DataView PayCategoryTimeDecimalDataView = new DataView(pvtDataSet.Tables["PayCategoryTimeDecimal"],
                            "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_NO = " + pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_PAY_CATEGORY_NO"].ToString() + strDayFilter,
                            "DAY_NO",
                            DataViewRowState.CurrentRows);

                            if (PayCategoryTimeDecimalDataView.Count == 0)
                            {
                                this.dgvEmployeeLeaveDataGridView[0,intCurrentEmployeeLeaveSelectedRowIndex].Style = this.ErrorDataGridViewCellStyle;
                            }
                            else
                            {
                                int intDaysDiff = dtToDateTime.Subtract(dtFromDateTime).Days + 1;

                                while (dtFromDateTime <= dtToDateTime)
                                {
                                    intFindDayRow = PayCategoryTimeDecimalDataView.Find(Convert.ToInt32(dtFromDateTime.DayOfWeek));

                                    if (intFindDayRow > -1)
                                    {
                                        intFindPublicHolidayRow = pvtPublicHolidayDataView.Find(dtFromDateTime.ToString("yyyy-MM-dd"));

                                        if (intFindPublicHolidayRow == -1)
                                        {
                                            dblHoursDay += Convert.ToDouble(PayCategoryTimeDecimalDataView[intFindDayRow]["TIME_DECIMAL"]);
                                            dblDays += 1;
                                        }
                                    }

                                    dtFromDateTime = dtFromDateTime.AddDays(1);
                                }

                                dgvEmployeeLeaveDataGridView[pvtintLeaveDaysCol, intCurrentEmployeeLeaveSelectedRowIndex].Value = dblDays.ToString("#0.00");
                                CurrentEmployeeLeave[0]["LEAVE_DAYS_DECIMAL"] = dblDays;

                                dgvEmployeeLeaveDataGridView[pvtintLeaveHoursCol, intCurrentEmployeeLeaveSelectedRowIndex].Value = dblHoursDay.ToString("#0.00");
                                CurrentEmployeeLeave[0]["LEAVE_HOURS_DECIMAL"] = dblHoursDay;

                                CurrentEmployeeLeave[0]["DATE_DIFF_NO_DAYS"] = intDaysDiff;

                                if (dblDays == 0)
                                {
                                    this.dgvEmployeeLeaveDataGridView[0,intCurrentEmployeeLeaveSelectedRowIndex].Style = this.ErrorDataGridViewCellStyle;
                                }
                                else
                                {
                                    if (intDaysDiff == Convert.ToInt32(dblDays))
                                    {
                                        dgvEmployeeLeaveDataGridView[1, intCurrentEmployeeLeaveSelectedRowIndex].Style = this.NormalDataGridViewCellStyle;

                                    }
                                    else
                                    {
                                        dgvEmployeeLeaveDataGridView[1, intCurrentEmployeeLeaveSelectedRowIndex].Style = this.LeaveDaysExcludedDataGridViewCellStyle;
                                    }
                                }
                            }

                            this.Check_To_Add_New_Leave_Row();
                        }
                    }
                    else
                    {
                        if (this.dgvEmployeeLeaveDataGridView[pvtintLeaveFromDateCol, intCurrentEmployeeLeaveSelectedRowIndex].Value.ToString() != "")
                        {
                            DateTime dtFromDateTime = DateTime.ParseExact(this.dgvEmployeeLeaveDataGridView[pvtintLeaveFromDateCol, intCurrentEmployeeLeaveSelectedRowIndex].Value.ToString(), AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);

                            this.dgvEmployeeLeaveDataGridView[pvtintLeaveToDateCol, intCurrentEmployeeLeaveSelectedRowIndex].Value = dtFromDateTime.ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString());

                            CurrentEmployeeLeave[0]["LEAVE_TO_DATE"] = dtFromDateTime.ToString("yyyy-MM-dd");

                            this.dgvEmployeeLeaveDataGridView[pvtintLeaveDaysCol, intCurrentEmployeeLeaveSelectedRowIndex].Value = "0.00";
                            this.dgvEmployeeLeaveDataGridView[pvtintLeaveHoursCol, intCurrentEmployeeLeaveSelectedRowIndex].Value = "0.00";

                            CurrentEmployeeLeave[0]["LEAVE_DAYS_DECIMAL"] = 0;
                            CurrentEmployeeLeave[0]["LEAVE_HOURS_DECIMAL"] = 0;

                            this.dgvEmployeeLeaveDataGridView[0,intCurrentEmployeeLeaveSelectedRowIndex].Style = this.NormalDataGridViewCellStyle;
                        }
                    }
                }
            }
        }

        private void btnLeaveDeleteRec_Click(object sender, System.EventArgs e)
        {
            if (this.dgvEmployeeLeaveDataGridView.Rows.Count > 0)
            {
                if (dgvEmployeeLeaveDataGridView[pvtintLeaveProcessDateCol, this.Get_DataGridView_SelectedRowIndex(this.dgvEmployeeLeaveDataGridView)].Value.ToString() != "")
                {
                    CustomMessageBox.Show("This Record is 'Read Only'.\n\nDelete Action Cancelled.",
                       this.Text,
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Error);
                }
                else
                {
                    //Find Current Record
                    int intLeaveRecNo = Convert.ToInt32(dgvEmployeeLeaveDataGridView[pvtintLeaveRecNoCol, this.Get_DataGridView_SelectedRowIndex(this.dgvEmployeeLeaveDataGridView)].Value);

                    DataView CurrentEmployeeLeaveDataView = new DataView(pvtDataSet.Tables["EmployeeLeave"],
                        pvtstrFilter + " AND EMPLOYEE_NO = " + this.pvtintEmployeeNo.ToString() + " AND EARNING_NO = " + pvtintLeaveEarningNo + " AND LEAVE_REC_NO = " + intLeaveRecNo.ToString(),
                    "",
                    DataViewRowState.CurrentRows);

                    //Delete Record
                    CurrentEmployeeLeaveDataView[0].Delete();

                    DataGridViewRow myDataGridViewRow = this.dgvEmployeeLeaveDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvEmployeeLeaveDataGridView)];

                    this.dgvEmployeeLeaveDataGridView.Rows.Remove(myDataGridViewRow);

                    this.Check_To_Add_New_Leave_Row();
                }
            }
        }

        private void Loan_Period_Click(object sender, System.EventArgs e)
        {
            if (this.dgvChosenDeductionDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvLoanDataGridView, pvtintLoanDataGridViewRowIndex);
            }
        }

        private void btnLoanDeleteRec_Click(object sender, System.EventArgs e)
        {
            if (this.dgvEmployeeLoanDataGridView.Rows.Count > 0)
            {
                if (dgvEmployeeLoanDataGridView[pvtintLoanProcessDateCol, this.Get_DataGridView_SelectedRowIndex(this.dgvEmployeeLoanDataGridView)].Value.ToString() != "")
                {
                    CustomMessageBox.Show("This Record is 'Read Only'.\n\nDelete Action Cancelled.",
                       this.Text,
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Error);
                }
                else
                {
                    //Find Current Record
                    int intLoanRecNo = Convert.ToInt32(dgvEmployeeLoanDataGridView[pvtintLoanRecNoCol, this.Get_DataGridView_SelectedRowIndex(this.dgvEmployeeLoanDataGridView)].Value);

                    DataView CurrentEmployeeLoanDataView = new DataView(pvtDataSet.Tables["EmployeeLoan"],
                        pvtstrFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND DEDUCTION_NO = " + pvtintDeductionNo.ToString() + " AND DEDUCTION_SUB_ACCOUNT_NO = " + pvtintDeductionSubNo.ToString() + " AND LOAN_REC_NO = " + intLoanRecNo.ToString(),
                    "",
                    DataViewRowState.CurrentRows);

                    //Delete Record
                    CurrentEmployeeLoanDataView[0].Delete();

                    DataGridViewRow myDataGridViewRow = this.dgvEmployeeLoanDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvEmployeeLoanDataGridView)];

                    this.dgvEmployeeLoanDataGridView.Rows.Remove(myDataGridViewRow);

                    this.Check_To_Add_New_Loan_Row();
                }
            }
        }

        private void DeductionPayPeriod_Click(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                RadioButton myRadioButton = (RadioButton)sender;

                if (myRadioButton.Name == "rbnDeductEachPayPeriod")
                {
                    pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_PERIOD_IND"] = "E";

                    this.cboDeductDay.Enabled = false;
                    this.cboDeductDay.SelectedIndex = -1;
                }
                else
                {
                    //Monthly
                    pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_PERIOD_IND"] = "M";
                    this.cboDeductDay.Enabled = true;
                }

                Check_If_Deduction_Default();
            }
        }

        private void dgvChosenDeductionDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                btnDeductionRemove_Click(sender, e);
            }
        }

        private void dgvDeductionDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                btnDeductionAdd_Click(sender, e);
            }
        }

        private void dgvChosenEarningDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.btnEarningRemove_Click(sender, e);
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

        public void btnUpdate_Click(object sender, System.EventArgs e)
        {
            this.Text += " - Update";

            Set_Form_For_Edit();

            if (this.pvtDataSet.Tables["Company"].Rows[0]["GENERATE_EMPLOYEE_NUMBER_IND"].ToString() == "Y")
            {
                this.txtCode.Focus();
            }
            else
            {
                this.txtName.Focus();
            }
        }

        private void Clear_TaxType_RadioButtons()
        {
            this.rbnTaxNormal.Checked = false;
            this.rbnTaxPartTime.Checked = false;
            this.rbnTaxDirective.Checked = false;
        }

        private void dgvEmployeeLoanDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvEmployeeLoanDataGridView.Rows.Count > 0
               & pvtblnEmployeeLoanDataGridViewLoaded == true)
            {
            }
        }

        private void dgvEmployeeLoanDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == pvtintLoanDescCol
                | e.ColumnIndex == pvtintLoanPaidCol)
            {
                //Find Current Record
                int intLoanRecNo = Convert.ToInt32(dgvEmployeeLoanDataGridView[pvtintLoanRecNoCol, e.RowIndex].Value);

                DataView CurrentEmployeeLoan = new DataView(pvtDataSet.Tables["EmployeeLoan"],
                    pvtstrFilter + " AND EMPLOYEE_NO = " + this.pvtintEmployeeNo.ToString() + " AND DEDUCTION_NO = " + pvtintDeductionNo.ToString() + " AND DEDUCTION_SUB_ACCOUNT_NO = " + pvtintDeductionSubNo.ToString() + " AND LOAN_REC_NO = " + intLoanRecNo.ToString(),
                    "",
                    DataViewRowState.CurrentRows);

                if (e.ColumnIndex == pvtintLoanDescCol)
                {
                    //Leave Description
                    if (dgvEmployeeLoanDataGridView[e.ColumnIndex, e.RowIndex].Value == null)
                    {
                        CurrentEmployeeLoan[0]["LOAN_DESC"] = "";
                    }
                    else
                    {
                        if (CurrentEmployeeLoan[0]["LOAN_DESC"].ToString() != dgvEmployeeLoanDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Trim())
                        {
                            CurrentEmployeeLoan[0]["LOAN_DESC"] = dgvEmployeeLoanDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Trim();
                        }
                    }
                }
                else
                {
                    if (dgvEmployeeLoanDataGridView[e.ColumnIndex, e.RowIndex].Value != null)
                    {
                        if (dgvEmployeeLoanDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString() != ".")
                        {
                            if (Convert.ToDouble(CurrentEmployeeLoan[0]["LOAN_AMOUNT_PAID"]) != Convert.ToDouble(dgvEmployeeLoanDataGridView[e.ColumnIndex, e.RowIndex].Value))
                            {
                                CurrentEmployeeLoan[0]["LOAN_AMOUNT_PAID"] = Convert.ToDouble(dgvEmployeeLoanDataGridView[e.ColumnIndex, e.RowIndex].Value);

                                dgvEmployeeLoanDataGridView[e.ColumnIndex, e.RowIndex].Value = Convert.ToDouble(CurrentEmployeeLoan[0]["LOAN_AMOUNT_PAID"]).ToString("########0.00");
                            }
                        }
                    }
                }

                this.Check_To_Add_New_Loan_Row();
            }
        }

        private void dgvEmployeeLoanDataGridView_MouseLeave(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void dgvEmployeeLoanDataGridView_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (e.RowIndex > -1
                & e.ColumnIndex > -1)
                {
                    if (e.ColumnIndex >= pvtintLoanReceivedCol)
                    {
                        this.Cursor = Cursors.No;
                    }
                    else
                    {
                        if (e.ColumnIndex == pvtintLoanDescCol)
                        {
                            //Hours

                            if (dgvEmployeeLoanDataGridView.Rows[e.RowIndex].ReadOnly == true)
                            {
                                this.Cursor = Cursors.No;
                            }
                            else
                            {
                                this.Cursor = Cursors.Default;
                            }
                        }
                        else
                        {
                            if (dgvEmployeeLoanDataGridView.Rows[e.RowIndex].ReadOnly == true)
                            {
                                this.Cursor = Cursors.No;
                            }
                            else
                            {
                                this.Cursor = Cursors.Default;
                            }
                        }
                    }
                }
                else
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void dgvEmployeeLoanDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (e.Control is ComboBox)
                {
                    ComboBox myComboBox = (ComboBox)e.Control;

                    if (myComboBox != null)
                    {
                        myComboBox.SelectedIndexChanged -= new EventHandler(Loan_ComboBox_SelectedIndexChanged);
                        myComboBox.SelectedIndexChanged += new EventHandler(Loan_ComboBox_SelectedIndexChanged);
                    }
                }
                else
                {
                    if (e.Control is TextBox)
                    {
                        e.Control.KeyPress -= new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);

                        if (this.dgvEmployeeLoanDataGridView.CurrentCell.ColumnIndex == pvtintLoanPaidCol)
                        {
                            e.Control.KeyPress += new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
                        }
                    }
                }
            }
        }

        private void dgvEmployeeLeaveDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvDeductionDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvDeductionDataGridView.Rows.Count > 0
                & pvtblnDeductionDataGridViewLoaded == true)
            {

            }
        }

        private void Loan_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                ComboBox myComboBox = (ComboBox)sender;

                //Find Current Record
                int intLoanRecNo = Convert.ToInt32(dgvEmployeeLoanDataGridView[pvtintLoanRecNoCol, this.Get_DataGridView_SelectedRowIndex(this.dgvEmployeeLoanDataGridView)].Value);

                DataView CurrentEmployeeLoan = new DataView(pvtDataSet.Tables["EmployeeLoan"],
                    pvtstrFilter + " AND EMPLOYEE_NO = " + this.pvtintEmployeeNo.ToString() + " AND DEDUCTION_NO = " + pvtintDeductionNo.ToString() + " AND DEDUCTION_SUB_ACCOUNT_NO = " + pvtintDeductionSubNo.ToString() + " AND LOAN_REC_NO = " + intLoanRecNo.ToString(),
                    "",
                    DataViewRowState.CurrentRows);

                int intProcessNo = myComboBox.SelectedIndex - 1;

                CurrentEmployeeLoan[0]["PROCESS_NO"] = intProcessNo;

                this.Check_To_Add_New_Loan_Row();
            }
        }

        private void dgvChosenDeductionDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnChosenDeductionDataGridViewLoaded == true)
            {
                if (pvtintChosenDeductionDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintChosenDeductionDataGridViewRowIndex = e.RowIndex;

                    pvtintDeductionAccountNo = Convert.ToInt32(this.dgvChosenDeductionDataGridView[pvtintDeductionsDeductionNoCol, e.RowIndex].Value);
                    pvtintDeductionSubAccountNo = Convert.ToInt32(this.dgvChosenDeductionDataGridView[pvtintDeductionsDeductionSubNoCol, e.RowIndex].Value);

                    pvtobjDeductionFind[0] = pvtintDeductionAccountNo;
                    pvtobjDeductionFind[1] = pvtintDeductionSubAccountNo;

                    pvtintEmployeeDeductionDataViewIndex = pvtEmployeeDeductionDataView.Find(pvtobjDeductionFind);

                    this.txtSelectedDeductionDesc.Text = this.dgvChosenDeductionDataGridView[pvtintDeductionsDescCol, e.RowIndex].Value.ToString();

                    this.Clear_DataGridView(this.dgvChosenDeductionEarningLinkDataGridView);

                    this.btnResetDefaultDeduction.Visible = false;
                    this.lblResetDefault.Visible = false;

                    //2013-08-30
                    this.cboDeductDay.SelectedIndex = -1;

                    this.lblPercent.Visible = false;

                    //Find Relevant Deduction Record
                    if (this.btnSave.Enabled == true)
                    {
                        clsISUtilities.NotDataBound_Control_Paint_Remove(lblPercentEarnings);
                    }

                    pvtDeductionEarningPercentageDataView = null;
                    pvtDeductionEarningPercentageDataView = new DataView(pvtDataSet.Tables["DeductionEarningPercentage"],
                    pvtstrFilter + " AND EMPLOYEE_NO = " + this.pvtintEmployeeNo + " AND DEDUCTION_NO = " + pvtintDeductionAccountNo + " AND DEDUCTION_SUB_ACCOUNT_NO = " + pvtintDeductionSubAccountNo,
                    "",
                    DataViewRowState.CurrentRows);

                    //Medical Aid
                    if (pvtintDeductionAccountNo == 5)
                    {
                        grbMedicalAidDependents.Visible = true;

                        if (this.btnSave.Enabled == true)
                        {
                            txtNumberMedicalDependents.Enabled = true;
                            this.chkDisability.Enabled = true;
                        }
                        else
                        {
                            txtNumberMedicalDependents.Enabled = false;
                            this.chkDisability.Enabled = false;
                        }
                    }
                    else
                    {
                        grbMedicalAidDependents.Visible = false;

                        txtNumberMedicalDependents.Enabled = false;
                        this.chkDisability.Enabled = false;
                    }

                    if (this.btnSave.Enabled == true)
                    {
                        //Disable All Controls
                        this.rbnDeductFixedValue.Enabled = false;
                        this.rbnDeductUserToEnter.Enabled = false;
                        this.rbnDeductPercentOfEarnings.Enabled = false;

                        this.rbnDeductEachPayPeriod.Enabled = false;
                        this.rbnDeductMonthly.Enabled = false;
                        this.cboDeductDay.Enabled = false;

                        this.txtDeductValue.Enabled = false;
                        this.txtDeductMinValue.Enabled = false;
                        this.txtDeductMaxValue.Enabled = false;
                    }

                    this.txtDeductValue.Text = Convert.ToDouble(pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_VALUE"]).ToString("#######0.00");
                    this.txtDeductMinValue.Text = Convert.ToDouble(pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_MIN_VALUE"]).ToString("#######0.00");
                    this.txtDeductMaxValue.Text = Convert.ToDouble(pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_MAX_VALUE"]).ToString("#######0.00");

                    //Tax / UIF
                    if (Convert.ToDouble(pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_NO"]) == 1
                        | Convert.ToDouble(pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_NO"]) == 2)
                    {
                        this.rbnDeductEachPayPeriod.Checked = true;
                        this.rbnDeductPercentOfEarnings.Checked = true;
                        this.lblPercent.Visible = true;

                        Load_Deduction_Earning_Percentage_Rows();
                    }
                    else
                    {
                        if (this.btnSave.Enabled == true)
                        {
                            //Enable Deduction Type Choices
                            this.rbnDeductFixedValue.Enabled = true;
                            this.rbnDeductUserToEnter.Enabled = true;
                            this.rbnDeductPercentOfEarnings.Enabled = true;
                        }

                        //User To Enter
                        if (pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_TYPE_IND"].ToString() == "U")
                        {
                            this.rbnDeductUserToEnter.Checked = true;
                        }
                        else
                        {
                            if (this.btnSave.Enabled == true)
                            {
                                this.rbnDeductEachPayPeriod.Enabled = true;
                                this.rbnDeductMonthly.Enabled = true;

                                this.txtDeductValue.Enabled = true;
                            }

                            //Fixed Amount
                            if (pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_TYPE_IND"].ToString() == "F")
                            {
                                this.rbnDeductFixedValue.Checked = true;
                            }
                            else
                            {
                                if (pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_TYPE_IND"].ToString() == "P")
                                {
                                    //Percentage Of Income
                                    if (this.btnSave.Enabled == true)
                                    {
                                        this.txtDeductMinValue.Enabled = true;
                                        this.txtDeductMaxValue.Enabled = true;
                                    }

                                    this.rbnDeductPercentOfEarnings.Checked = true;

                                    Load_Deduction_Earning_Percentage_Rows();

                                    this.lblPercent.Visible = true;
                                }
                                else
                                {
                                    CustomMessageBox.Show("Deduction Type INVALID For '" + this.dgvChosenDeductionDataGridView[pvtintDeductionsDescCol, e.RowIndex].ToString() + "'",this.Text,MessageBoxButtons.OK,MessageBoxIcon.Error);
                                }
                            }
                        }

                        if (pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_PERIOD_IND"].ToString() == "E")
                        {
                            this.rbnDeductEachPayPeriod.Checked = true;
                        }
                        else
                        {
                            if (pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_PERIOD_IND"].ToString() == "M")
                            {
                                if (this.btnSave.Enabled == true)
                                {
                                    this.cboDeductDay.Enabled = true;
                                }

                                this.rbnDeductMonthly.Checked = true;

                                if (Convert.ToInt32(pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_DAY_VALUE"]) == 99)
                                {
                                    this.cboDeductDay.SelectedIndex = this.cboDeductDay.Items.Count - 1;
                                }
                                else
                                {
                                    this.cboDeductDay.SelectedIndex = Convert.ToInt32(pvtEmployeeDeductionDataView[pvtintEmployeeDeductionDataViewIndex]["DEDUCTION_DAY_VALUE"]) - 1;
                                }
                            }
                            else
                            {
                                if (this.btnSave.Enabled == false)
                                {
                                    MessageBox.Show("flxgChosenDeduction_AfterRowColChange EARNING_PERIOD_IND Not Set");
                                }
                            }
                        }
                    }

                    Check_If_Deduction_Default();
                }
            }
        }

        private void dgvPayrollTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPayrollTypeDataGridViewLoaded == true)
            {
                if (pvtintPayrollTypeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayrollTypeDataGridViewRowIndex = e.RowIndex;

                    this.rbnActive.Checked = true;
                    this.lblClosed.Visible = false;

                    this.grbEmployeeLock.Visible = false;
                    this.grbLeaveError.Visible = false;

                    if (e.RowIndex == 0)
                    {
                        pvtstrPayrollType = "W";

                        this.lblEmployeeLock.Text = "Some Employee Records are Locked Due to Current Wage Run.";

                        this.lblLeaveDescription.Visible = true;
                        this.txtLeaveDescription.Visible = true;

                        this.grbRate.Text = "Hourly Rate";
                        this.dgvChosenPayCategoryDataGridView.Columns[1].HeaderText = "Hourly Rate";

                        this.cboLeaveShift.Top = 20;

                        this.grbEntry.Visible = true;

                        this.grbCheques.Visible = false;
                    }
                    else
                    {
                        pvtstrPayrollType = "S";

                        this.lblEmployeeLock.Text = "Some Employee Records are Locked Due to Current Salary Run.";

                        this.lblLeaveDescription.Visible = false;
                        this.txtLeaveDescription.Visible = false;

                        this.grbRate.Text = "Monthly Payment";
                        this.dgvChosenPayCategoryDataGridView.Columns[1].HeaderText = "Monthly Payment";

                        this.cboLeaveShift.Top = 32;

                        this.grbEntry.Visible = false;

                        this.grbCheques.Visible = true;
                    }

                    pvtstrFilter = "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'";

                    string strPayCategoryClosedInd = "";

                    if (this.rbnActive.Checked == true)
                    {
                        strPayCategoryClosedInd = " AND CLOSED_IND <> 'Y' ";
                    }
                  
                    pvtPayCategoryDataView = null;
                    pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"],
                        pvtstrFilter + strPayCategoryClosedInd,
                        "",
                        DataViewRowState.CurrentRows);

                    pvtEarningDataView = null;
                    pvtEarningDataView = new DataView(pvtDataSet.Tables["Earning"],
                            pvtstrFilter,
                            "EARNING_DESC",
                            DataViewRowState.CurrentRows);

                    pvtDeductionDataView = null;
                    pvtDeductionDataView = new DataView(pvtDataSet.Tables["Deduction"],
                            pvtstrFilter,
                            "DEDUCTION_DESC",
                            DataViewRowState.CurrentRows);

                    Load_CurrentForm_Records();
                }
            }
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

                    this.tabEmployee.Refresh();
                    //this.tabPage2.Refresh();
                    //this.tabPage3.Refresh();

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
#if(DEBUG)

                    System.Diagnostics.Debug.WriteLine("dgvEmployeeDataGridView_RowEnter pvtintEmployeeNo = " + pvtintEmployeeNo.ToString());
#endif
                    if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["ETI_START_DATE"] == System.DBNull.Value)
                    {
                        this.cboEtiStartMonth.SelectedIndex = 0;
                    }
                    else
                    {
                        //Find Row
                        for (int intRow = 0; intRow < this.cboEtiStartMonth.Items.Count; intRow++)
                        {
                            if (Convert.ToDateTime(pvtEmployeeDataView[clsISUtilities.DataViewIndex]["ETI_START_DATE"]).ToString("MMMM yyyy") == this.cboEtiStartMonth.Items[intRow].ToString())
                            {
                                this.cboEtiStartMonth.SelectedIndex = intRow;
                                break;
                            }
                        }
                    }

                    if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_CONFIG_ERROR_IND"].ToString() == "Y")
                    {
                        this.grbLeaveError.Visible = true;
                    }
                    else
                    {
                        this.grbLeaveError.Visible = false;
                    }

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

                    if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["USE_RES_ADDR_COMPANY_IND"].ToString() == "Y")
                    {
                        this.rbnResAddrCompany.Checked = true;

                        this.txtTelWork.Text = "";
                    }
                    else
                    {
                        this.rbnResAddrOwn.Checked = true;
                    }

                    if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_POST_OPTION_IND"].ToString() == "")
                    {
                        this.rbnSameResidentialAddr.Checked = false;
                        this.rbnStreetAddr.Checked = false;
                        this.rbnPOBoxAddr.Checked = false;
                        this.rbnPrivateBagAddr.Checked = false;

                        Set_Postal_Address_Default();
                    }
                    else
                    {
                        EventArgs ev = new EventArgs();

                        if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_POST_OPTION_IND"].ToString() == "R")
                        {
                            //R=Use Residential
                            this.rbnSameResidentialAddr.Checked = true;
                            rbnPostalOption_Click(this.rbnSameResidentialAddr, ev);
                        }
                        else
                        {
                            if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_POST_OPTION_IND"].ToString() == "S")
                            {
                                //S=Street Address
                                this.rbnStreetAddr.Checked = true;
                                rbnPostalOption_Click(this.rbnStreetAddr, ev);
                            }
                            else
                            {
                                if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_POST_OPTION_IND"].ToString() == "P")
                                {
                                    //P=PO Box
                                    this.rbnPOBoxAddr.Checked = true;
                                    rbnPostalOption_Click(this.rbnPOBoxAddr, ev);

                                }
                                else
                                {
                                    if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_POST_OPTION_IND"].ToString() == "B")
                                    {
                                        //B=Private Bag
                                        this.rbnPrivateBagAddr.Checked = true;
                                        rbnPostalOption_Click(this.rbnPrivateBagAddr, ev);
                                    }
                                }
                            }
                        }
                    }

                    if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["USE_WORK_TEL_IND"].ToString() == "Y")
                    {
                        this.rbnWorkTelCompany.Checked = true;
                    }
                    else
                    {
                        this.rbnWorkTelOwn.Checked = true;
                    }

                    if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["TAX_TYPE_IND"].ToString() == "N")
                    {
                        this.rbnTaxNormal.Checked = true;
                    }
                    else
                    {
                        if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["TAX_TYPE_IND"].ToString() == "P")
                        {
                            this.rbnTaxPartTime.Checked = true;
                        }
                        else
                        {
                            if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["TAX_TYPE_IND"].ToString() == "D")
                            {
                                this.rbnTaxDirective.Checked = true;
                            }
                            else
                            {
                                //Tax Type
                                Clear_TaxType_RadioButtons();
                            }
                        }
                    }

                    //Link To Normal / Sick Leave Category
                    this.cboLeaveShift.SelectedIndex = -1;

                    if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_SHIFT_NO"] != System.DBNull.Value)
                    {
                        for (int intIndex = 0; intIndex < pvtLeaveLinkDataView.Count; intIndex++)
                        {
                            if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["LEAVE_SHIFT_NO"].ToString() == pvtLeaveLinkDataView[intIndex]["LEAVE_SHIFT_NO"].ToString())
                            {
                                this.cboLeaveShift.SelectedIndex = intIndex;
                                break;
                            }
                        }
                    }

                    Load_Current_Employee_Pay_Category();

                    //ELR 2014-05-01
                    if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["MEDICAL_AID_DISABILITY_IND"].ToString() == "Y")
                    {
                        this.chkDisability.Checked = true;
                    }
                    else
                    {
                        this.chkDisability.Checked = false;
                    }

                    //ELR 2017-09-29
                    if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMAIL_VIA_PAYSLIP_IND"].ToString() == "Y")
                    {
                        this.chkPayslip.Checked = true;
                    }
                    else
                    {
                        this.chkPayslip.Checked = false;
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

                    //Reset Data Sets to null
                    this.pvtEmployeeEarningDataView = null;
                    this.pvtEmployeeDeductionDataView = null;
                    this.pvtEmployeeLeaveDataView = null;

                    //2012-08-14
                    this.Clear_DataGridView(this.dgvEarningDataGridView);
                    this.Clear_DataGridView(this.dgvChosenEarningDataGridView);

                    //2012-08-14
                    this.Clear_DataGridView(this.dgvChosenDeductionDataGridView);
                    this.Clear_DataGridView(this.dgvChosenDeductionEarningLinkDataGridView);

                    //2012-08-14
                    this.Clear_DataGridView(this.dgvLeaveTypeDataGridView);
                    this.Clear_DataGridView(this.dgvLoanDataGridView);

                    this.Clear_DataGridView(this.dgvEmployeeLeaveDataGridView);
                    this.Clear_DataGridView(this.dgvEmployeeLoanDataGridView);

                    if (pvtobjSender != null)
                    {
                        tabEmployee_SelectedIndexChanged(pvtobjSender, null);
                    }

                    //if (this.dgvEmployeeDataGridView.Enabled == true)
                    //{
                    //    this.dgvEmployeeDataGridView.Focus();
                    //}

                    this.Refresh();

                    //ELR - Fix For Double Row Enter
                    pvtblnEmployeeDataGridViewLoaded = true;
                }
            }
        }

        private void Set_Postal_Address_Default()
        {
            this.lblPostCountry.Text = "Country";
            this.lblPostCity.Text = "City / Town";
            this.lblPostSuburb.Text = "Suburb / District";
            this.lblPostStreetName.Text = "Street / Farm Name";
            this.lblPostStreetNumber.Text = "Street Number";
            this.lblPostComplex.Text = "Complex";
            this.lblPostUnitNumber.Text = "Unit Number";

            //this.lblPostUnitNumber.Visible = true;
            //this.txtPostAddrUnitNumber.Visible = true;
            //this.lblPostComplex.Visible = true;
            //this.txtPostAddrComplex.Visible = true;
            //this.lblPostCity.Visible = true;
            //this.txtPostCity.Visible = true;
        }

        private void rbnPostalOption_Click(object sender, EventArgs e)
        {
            RadioButton myRadioButton = (RadioButton)sender;

            if (this.btnSave.Enabled == true)
            {
                clsISUtilities.DataBind_Special_Field_Remove(3);
                clsISUtilities.DataBind_Field_Remove(this.txtPostAddrStreetNumber);
                clsISUtilities.DataBind_Field_Remove(this.txtPostStreetName);
                clsISUtilities.DataBind_Field_Remove(this.txtPostSuburb);
                clsISUtilities.DataBind_Field_Remove(this.txtPostAddrCode);
            }

            if (myRadioButton.Name == "rbnSameResidentialAddr"
            || myRadioButton.Name == "rbnStreetAddr")
            {
                Set_Postal_Address_Default();

                if (this.btnSave.Enabled == true)
                {
                    if (myRadioButton.Name == "rbnSameResidentialAddr")
                    {
                        this.Clear_Postal_Fields();
                    }

                    this.txtPostAddrStreetNumber.MaxLength = 8;
                }

                if (myRadioButton.Name == "rbnSameResidentialAddr")
                {
                    if (this.btnSave.Enabled == true)
                    {
                        pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_POST_OPTION_IND"] = "R";

                        this.txtPostAddrUnitNumber.Enabled = false;
                        this.txtPostAddrComplex.Enabled = false;
                        this.txtPostAddrStreetNumber.Enabled = false;
                        this.txtPostStreetName.Enabled = false;
                        this.txtPostSuburb.Enabled = false;
                        this.txtPostCity.Enabled = false;
                        this.txtPostAddrCode.Enabled = false;
                        this.cboPostCountry.Enabled = false;
                        this.btnPostalAddressRSA.Enabled = false;
                    }
                }
                else
                {
                    if (this.btnSave.Enabled == true)
                    {
                        pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_POST_OPTION_IND"] = "S";

                        clsISUtilities.DataBind_DataView_To_TextBox(txtPostAddrStreetNumber, "EMPLOYEE_POST_STREET_NUMBER", false, "", false);

                        clsISUtilities.DataBind_DataView_To_TextBox(this.txtPostStreetName, "EMPLOYEE_POST_STREET_NAME", true, "Enter Street / Farm Name.", true);
                        clsISUtilities.DataBind_Special_Field(this.txtPostStreetName, 3);
                        clsISUtilities.DataBind_DataView_To_TextBox(this.txtPostSuburb, "EMPLOYEE_POST_SUBURB", false, "", true);
                        clsISUtilities.DataBind_Special_Field(this.txtPostSuburb, 3);
                        clsISUtilities.DataBind_DataView_To_TextBox(this.txtPostCity, "EMPLOYEE_POST_CITY", false, "", true);
                        clsISUtilities.DataBind_Special_Field(this.txtPostCity, 3);
                        clsISUtilities.DataBind_DataView_To_TextBox(this.txtPostAddrCode, "EMPLOYEE_POST_CODE", false, "", true);
                        clsISUtilities.DataBind_Special_Field(this.txtPostAddrCode, 3);

                        this.txtPostAddrUnitNumber.Enabled = true;
                        this.txtPostAddrComplex.Enabled = true;
                        this.txtPostAddrStreetNumber.Enabled = true;
                        this.txtPostStreetName.Enabled = true;
                        this.txtPostSuburb.Enabled = true;
                        this.txtPostCity.Enabled = true;
                        this.txtPostAddrCode.Enabled = true;
                        this.cboPostCountry.Enabled = true;
                        this.btnPostalAddressRSA.Enabled = true;
                    }
                }
            }
            else
            {
                this.txtPostAddrStreetNumber.MaxLength = 12;

                //this.lblPostUnitNumber.Visible = false;
                //this.txtPostAddrUnitNumber.Visible = false;
                this.txtPostAddrUnitNumber.Text = "";

                //this.lblPostComplex.Visible = false;
                //this.txtPostAddrComplex.Visible = false;
                this.txtPostAddrComplex.Text = "";

                this.lblPostStreetName.Text = "Postal Agency / Suite";
                this.lblPostSuburb.Text = "Post Office / Address";

                //this.lblPostCity.Visible = false;
                //this.txtPostCity.Visible = false;
                this.txtPostCity.Text = "";

                if (this.btnSave.Enabled == true)
                {
                    clsISUtilities.DataBind_DataView_To_TextBox(this.txtPostStreetName, "EMPLOYEE_POST_STREET_NAME", false, "", false);
                    clsISUtilities.DataBind_DataView_To_Numeric_TextBox_Min_Length(this.txtPostAddrCode, "EMPLOYEE_POST_CODE", 0, 4, true, "Enter Postal Code.", true, 9999, false);
                    clsISUtilities.DataBind_DataView_To_TextBox(this.txtPostSuburb, "EMPLOYEE_POST_SUBURB", true, "Enter Post Office / Address.", true);
                }

                if (myRadioButton.Name == "rbnPOBoxAddr")
                {
                    this.lblPostStreetNumber.Text = "PO Box Number";

                    if (this.btnSave.Enabled == true)
                    {
                        pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_POST_OPTION_IND"] = "P";

                        clsISUtilities.DataBind_DataView_To_TextBox(this.txtPostAddrStreetNumber, "EMPLOYEE_POST_STREET_NUMBER", true, "Enter PO Box Number.", true);
                    }
                }
                else
                {
                    //Private Bag
                    this.lblPostStreetNumber.Text = "Private Bag Number";

                    if (this.btnSave.Enabled == true)
                    {
                        pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_POST_OPTION_IND"] = "B";

                        clsISUtilities.DataBind_DataView_To_TextBox(this.txtPostAddrStreetNumber, "EMPLOYEE_POST_STREET_NUMBER", true, "Enter Private Bag Number.", true);
                    }
                }

                if (this.btnSave.Enabled == true)
                {
                    this.txtPostAddrStreetNumber.Enabled = true;
                    this.txtPostStreetName.Enabled = true;
                    this.txtPostSuburb.Enabled = true;
                    this.txtPostAddrCode.Enabled = true;
                    this.cboPostCountry.Enabled = true;
                    this.btnPostalAddressRSA.Enabled = true;

                    this.txtPostAddrUnitNumber.Enabled = false;
                    this.txtPostAddrComplex.Enabled = false;
                    this.txtPostCity.Enabled = false;
                }
            }
        }

        private void Clear_Postal_Fields()
        {
            this.txtPostAddrUnitNumber.Text = "";
            this.txtPostAddrComplex.Text = "";
            this.txtPostAddrStreetNumber.Text = "";
            this.txtPostStreetName.Text = "";
            this.txtPostSuburb.Text = "";
            this.txtPostCity.Text = "";
            this.txtPostAddrCode.Text = "";
            this.cboPostCountry.SelectedIndex = -1;
        }

        private void dgvChosenPayCategoryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnChosenPayCategoryDataGridViewLoaded == true)
            {
                if (pvtintChosenPayCategoryDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintChosenPayCategoryDataGridViewRowIndex = e.RowIndex;

                    this.txtSelectedPayCategoryDesc.Text = this.dgvChosenPayCategoryDataGridView[pvtintPayCategoryDescCol, e.RowIndex].Value.ToString();

                    this.txtRate.Text = this.dgvChosenPayCategoryDataGridView[pvtintPayCategoryRateCol, e.RowIndex].Value.ToString();

                    if (Convert.ToBoolean(this.dgvChosenPayCategoryDataGridView[pvtintPayCategoryDefaultIndCol, e.RowIndex].Value) == true)
                    {
                        this.rbnDefaultYes.Checked = true;
                    }
                    else
                    {
                        this.rbnDefaultNo.Checked = true;
                    }
                }
            }
        }

        private void dgvPayCategoryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPayCategoryDataGridView.Rows.Count > 0
            & pvtblnPayCategoryDataGridViewLoaded == true)
            {
            }
        }


        private void Clear_DataGridView(DataGridView myDataGridView)
        {
            myDataGridView.RowCount = 0;

            if (myDataGridView.SortedColumn != null)
            {
                myDataGridView.SortedColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            myDataGridView.Refresh();
        }

        private void chkDisability_CheckedChanged(object sender, EventArgs e)
        {
            //ELR 2014-05-01
            if (this.btnSave.Enabled == true)
            {
                if (pvtEmployeeDataView.Count > 0)
                {
                    if (clsISUtilities.DataViewIndex > -1)
                    {
                        if (chkDisability.Checked == true)
                        {
                            pvtEmployeeDataView[clsISUtilities.DataViewIndex]["MEDICAL_AID_DISABILITY_IND"] = "Y";
                        }
                        else
                        {
                            pvtEmployeeDataView[clsISUtilities.DataViewIndex]["MEDICAL_AID_DISABILITY_IND"] = "N";
                        }
                    }
                }
            }
        }

        private void btnSetAddressRSA_Click(object sender, EventArgs e)
        {
            Button myButton = (Button)sender;

            if (myButton.Name == "btnPhsicalAddressRSA")
            {
                for (int intRow = 0; intRow < this.cboAddressCountry.Items.Count; intRow++)
                {
                    if (this.cboAddressCountry.Items[intRow].ToString() == "South Africa")
                    {
                        this.cboAddressCountry.SelectedIndex = intRow;
                        
                        break;
                    }
                }
            }
            else
            {
                for (int intRow = 0; intRow < this.cboPostCountry.Items.Count; intRow++)
                {
                    if (this.cboPostCountry.Items[intRow].ToString() == "South Africa")
                    {
                        this.cboPostCountry.SelectedIndex = intRow;
                        
                        break;
                    }
                }
            }
        }

        private void cboEtiStartMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (cboEtiStartMonth.SelectedIndex == 0)
                {
                    pvtEmployeeDataView[clsISUtilities.DataViewIndex]["ETI_START_DATE"] = System.DBNull.Value;
                }
                else
                {
                    string[] strEtiParts = this.cboEtiStartMonth.SelectedItem.ToString().Split(' ');

                    if (dictionaryMonths.ContainsKey(strEtiParts[0]))
                    {
                        pvtEmployeeDataView[clsISUtilities.DataViewIndex]["ETI_START_DATE"] = new DateTime(Convert.ToInt32(strEtiParts[1]), Convert.ToInt32(dictionaryMonths[strEtiParts[0]]),1);
                    }
                }
            }
        }

        private void dgvEmployeeDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 2)
            {
                if (double.Parse(dgvEmployeeDataGridView[7, e.RowIndex1].Value.ToString()) > double.Parse(dgvEmployeeDataGridView[7, e.RowIndex2].Value.ToString()))
                {
                    e.SortResult = 1;
                }
                else
                {
                    if (double.Parse(dgvEmployeeDataGridView[7, e.RowIndex1].Value.ToString()) < double.Parse(dgvEmployeeDataGridView[7, e.RowIndex2].Value.ToString()))
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

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.Timer.Enabled = false;

            CustomMessageBox.Show(pvtstrFingerprintDeviceOpenedFailureMessage, "Fingerprint Open Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                            dgResult = CustomMessageBox.Show("Select OK to Delete Fingerprint.", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
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

        private void frmEmployee_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.btnSave.Enabled == true
            && this.tabEmployee.SelectedIndex == 7)
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

        private void lnkCancel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.pnlEnroll.Visible = false;
            this.btnClearFingers.Visible = true;
            this.pnlFingers.Visible = true;
            this.btnSave.Enabled = true;
        }

        private void frmEmployee_FormClosing(object sender, FormClosingEventArgs e)
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

        private void chkPayslip_CheckedChanged(object sender, EventArgs e)
        {
            //ELR 2014-05-01
            if (this.btnSave.Enabled == true)
            {
                if (pvtEmployeeDataView.Count > 0)
                {
                    if (clsISUtilities.DataViewIndex > -1)
                    {
                        if (chkPayslip.Checked == true)
                        {
                            pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMAIL_VIA_PAYSLIP_IND"] = "Y";
                        }
                        else
                        {
                            pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMAIL_VIA_PAYSLIP_IND"] = "N";
                        }
                    }
                }
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

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (pvtEmployeeDataView.Count > 0)
                {
                    if (clsISUtilities.DataViewIndex > -1)
                    {
                        if (this.txtEmail.Text.Trim() == "")
                        {
                            this.chkPayslip.Checked = false;

                            pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMAIL_VIA_PAYSLIP_IND"] = "N";
                        }
                    }
                }
            }
        }
    }
}
