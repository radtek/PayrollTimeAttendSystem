using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace InteractPayroll
{
    public partial class frmEasyFile : Form
    {
        clsISUtilities clsISUtilities;

        private DataView pvtEmployeeDataView;
        private DataView pvtEmployeeEarningDataView;
        private DataView pvtEmployeeDeductionContibutionDataView;
        private DataView pvtEmployeeDeductionTaxUifDataView;

        private DataView pvtEmployeeDeductionTaxCreditDataView;
        private DataView pvtSic7CodeGroupDataView;
        private DataView pvtEmployeePayCategoryAddressDataView;

        private DataView pvtNaturePersonDataView;

        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;
        private byte[] pvtbytCompress; 

        private int pvtintEmployeeNo = -1;

        private int pvtintEmployeeDataGridViewRowIndex = -1;
        private int pvtintPeriodDataGridViewRowIndex = -1;
        private int pvtintSic7DataGridViewRowIndex = -1;

        DataGridViewCellStyle eFilingErrorDataGridViewCellStyle;

        private bool pvtblnEmployeeDataGridViewLoaded = false;
        private bool pvtblnPeriodDataGridViewLoaded = false;
        private bool pvtblnSic7DataGridViewLoaded = false;

        private bool pvtblnSetViaDataGridView = false;

        Int64 pvtint64TotalEmployeeCodes = 0;

        private string pvtstrSic7GroupCode = "";

        private int pvtintTimerCount = 0;

        public frmEasyFile()
        {
            InitializeComponent();

            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 114;

                this.dgvEmployeeDataGridView.Height += 114;

                this.tabEmployee.Top += 114;
            }
        }

        private void frmeFiling_Load(object sender, EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this, "busEasyFile");

                string strDirectory = Directory.GetCurrentDirectory() + "\\e@syfile";

                if (Directory.Exists(strDirectory))
                {
                }
                else
                {
                    Directory.CreateDirectory(strDirectory);
                }

                this.txtDirectory.Text = strDirectory;

                eFilingErrorDataGridViewCellStyle = new DataGridViewCellStyle();
                eFilingErrorDataGridViewCellStyle.BackColor = Color.LightCoral;
                eFilingErrorDataGridViewCellStyle.SelectionBackColor = Color.LightCoral;

                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPeriod.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSic7Desc.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                object[] objParm = new object[3];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[2] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));

                pvtDataSet = new DataSet();
                //Countries DataTable
                DataTable DataTable = clsISUtilities.Get_Countries();
                pvtDataSet.Tables.Add(DataTable);

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);
                DataSet pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                pvtDataSet.Merge(pvtTempDataSet);

                for (int intRow = 0; intRow < this.pvtDataSet.Tables["Sic7CodeGroup"].Rows.Count; intRow++)
                {
                    this.cboSic7Group.Items.Add(this.pvtDataSet.Tables["Sic7CodeGroup"].Rows[intRow]["SIC7_GROUP_DESC"].ToString());
                }

                pvtNaturePersonDataView = null;
                pvtNaturePersonDataView = new DataView(pvtDataSet.Tables["NaturePerson"],
                "",
                "NATURE_PERSON_NO",
                DataViewRowState.CurrentRows);

                //Load ComboBoxes
                clsISUtilities.DataBind_ComboBox_Load(this.cboNatureOfPerson, this.pvtDataSet.Tables["NaturePerson"], "NATURE_PERSON_DESC", "NATURE_PERSON_NO");
                clsISUtilities.DataBind_ComboBox_Load(this.cboCountry, this.pvtDataSet.Tables["Country"], "COUNTRY_DESC", "COUNTRY_CODE");
                clsISUtilities.DataBind_ComboBox_Load(this.cboAddressCountry, this.pvtDataSet.Tables["Country"], "COUNTRY_DESC", "COUNTRY_CODE2");

                //ELR - 20141014
                clsISUtilities.DataBind_ComboBox_Load(this.cboPostCountry, this.pvtDataSet.Tables["Country"], "COUNTRY_DESC", "COUNTRY_CODE2");
               
                clsISUtilities.DataBind_ComboBox_Load(cboBankAccountType, this.pvtDataSet.Tables["BankAccountType"], "BANK_ACCOUNT_TYPE_DESC", "BANK_ACCOUNT_TYPE_NO");
                clsISUtilities.DataBind_ComboBox_Load(cboBankAccountRelationship, this.pvtDataSet.Tables["BankRelationshipType"], "BANK_ACCOUNT_RELATIONSHIP_TYPE_DESC", "BANK_ACCOUNT_RELATIONSHIP_TYPE_NO");
                clsISUtilities.DataBind_ComboBox_Load(this.cboBank, this.pvtDataSet.Tables["Bank"], "BANK_DESC", "BANK_NO");

                bool blnUifReferenceNoError = false;

                if (this.pvtDataSet.Tables["Company"].Rows[0]["UIF_REF_NO"].ToString() == "")
                {
                    if (pvtDataSet.Tables["EmployeeDeductionTaxUif"] != null)
                    {
                        DataView UifDataView = new DataView(pvtDataSet.Tables["EmployeeDeductionTaxUif"],
                        "IRP5_CODE = 4141",
                        "",
                        DataViewRowState.CurrentRows);

                        if (UifDataView.Count > 0)
                        {
                            this.grbUifError.Visible = true;
                            blnUifReferenceNoError = true;
                        }
                    }
                }

                if (blnUifReferenceNoError == false)
                {
                    string strCloseInd = "";

                    for (int intRow = 0; intRow < this.pvtDataSet.Tables["Period"].Rows.Count; intRow++)
                    {
                        if (this.pvtDataSet.Tables["Period"].Rows[intRow]["EFILING_CLOSED_IND"].ToString() == "Y")
                        {
                            strCloseInd = "Yes";
                        }
                        else
                        {
                            strCloseInd = "No";
                        }

                        this.dgvPeriodDataGridView.Rows.Add(Convert.ToDateTime(this.pvtDataSet.Tables["Period"].Rows[intRow]["EFILING_PERIOD"]).ToString("MMMM yyyy"),
                                                            strCloseInd,
                                                            Convert.ToDateTime(this.pvtDataSet.Tables["Period"].Rows[intRow]["EFILING_PERIOD"]).ToString("yyyy-MM-dd"),
                                                            this.pvtDataSet.Tables["Period"].Rows[intRow]["EFILING_NO"].ToString(),
                                                            intRow.ToString());
                    }

                    pvtblnPeriodDataGridViewLoaded = true;

                    this.Show();
                    this.Refresh();

                    if (this.pvtDataSet.Tables["Period"].Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvPeriodDataGridView, 0);

                        Load_CurrentForm_Records();
                    }
                    else
                    {
                        tmrTimer.Enabled = true;
                    }
                }
                else
                {
                    this.btnUpdate.Enabled = false;

                    this.rbnNormal.Enabled = false;
                    this.rbnEFilingCheck.Enabled = false;
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void Load_CurrentForm_Records()
        {
            if (pvtDataSet.Tables["Employee"].Rows.Count == 0)
            {
                //Lock Out All Buttons
                this.btnUpdate.Enabled = false;
                this.btnSave.Enabled = false;
                this.btnCancel.Enabled = false;
            }
            else
            {
                this.btnCreateEfile.Enabled = false;

                Clear_Form_Fields();

                this.tabEmployee.SelectedIndex = 0;

                pvtintEmployeeNo = -1;
                clsISUtilities.DataViewIndex = -1;
               
                Set_Form_For_Read();

                Load_Employee();
            }
        }

        private void Load_Employee()
        {
            bool blnPayCategoryAddressError = false;
            bool blnEmployeeExists = false;

            pvtEmployeeDataView = null;
            pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString(),
                "EMPLOYEE_CODE",
                DataViewRowState.CurrentRows);

            bool blnFound = false;

            for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
            {
                pvtEmployeeDataView[intRow]["OK_IND"] = "N";

#if (DEBUG)
                if (pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString() == "89")
                {
                    string STOP = "";
                }
#endif
                if (pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString() == ""
                || pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString() == ""
                || pvtEmployeeDataView[intRow]["EMPLOYEE_INITIALS"].ToString() == ""
                || pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString() == ""
                || pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString().IndexOf(",") > -1
                || pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString().IndexOf(",") > -1
                || pvtEmployeeDataView[intRow]["EMPLOYEE_INITIALS"].ToString().IndexOf(",") > -1
                || pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString().IndexOf(",") > -1)
                {
                    continue;
                }
                
                //2017-10-12
                if (pvtEmployeeDataView[intRow]["EMPLOYEE_EMAIL"].ToString().IndexOf(",") > -1)
                {
                    continue;
                }

                if (pvtEmployeeDataView[intRow]["NATURE_PERSON_NO"].ToString() == "")
                {
                    continue;
                }
                else
                {
                    if (Convert.ToInt32(pvtEmployeeDataView[intRow]["NATURE_PERSON_NO"]) == 1
                    | Convert.ToInt32(pvtEmployeeDataView[intRow]["NATURE_PERSON_NO"]) == 4)
                    {
                        //Has Identity Document
                        if (pvtEmployeeDataView[intRow]["EMPLOYEE_ID_NO"].ToString() == "")
                        {
                            continue;
                        }
                        else
                        {
                            if (clsISUtilities.SA_Identity_Number_Check(pvtEmployeeDataView[intRow]["EMPLOYEE_ID_NO"].ToString()) != 0)
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(pvtEmployeeDataView[intRow]["NATURE_PERSON_NO"]) == 2
                        || Convert.ToInt32(pvtEmployeeDataView[intRow]["NATURE_PERSON_NO"]) == 5)
                        {
                            //Has Passport
                            //Has Identity Document
                            if (pvtEmployeeDataView[intRow]["EMPLOYEE_PASSPORT_NO"].ToString() == ""
                            || pvtEmployeeDataView[intRow]["EMPLOYEE_PASSPORT_NO"].ToString().IndexOf(",") > -1)
                            {
                                continue;
                            }
                            else
                            {
                                blnFound = false;

                                for (int intCountryRow = 0; intCountryRow < this.pvtDataSet.Tables["Country"].Rows.Count; intCountryRow++)
                                {
                                    if (pvtEmployeeDataView[intRow]["EMPLOYEE_PASSPORT_COUNTRY_CODE"].ToString() == this.pvtDataSet.Tables["Country"].Rows[intCountryRow]["COUNTRY_CODE"].ToString())
                                    {
                                        blnFound = true;
                                        break;
                                    }
                                }

                                if (blnFound == false)
                                {
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            if (Convert.ToInt32(pvtEmployeeDataView[intRow]["NATURE_PERSON_NO"]) == 3)
                            {
                                //No Documents
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }

                if (pvtEmployeeDataView[intRow]["USE_RES_ADDR_COMPANY_IND"].ToString() != "Y")
                {
                    if (pvtEmployeeDataView[intRow]["EMPLOYEE_RES_STREET_NAME"].ToString().Trim() == "")
                    {
                        continue;
                    }
                    else
                    {
                        if (pvtEmployeeDataView[intRow]["EMPLOYEE_RES_SUBURB"].ToString().Trim() == ""
                        && pvtEmployeeDataView[intRow]["EMPLOYEE_RES_CITY"].ToString().Trim() == "")
                        {
                            continue;
                        }
                        else
                        {
                            if (pvtEmployeeDataView[intRow]["EMPLOYEE_RES_CODE"].ToString() != "0000"
                            && pvtEmployeeDataView[intRow]["EMPLOYEE_RES_CODE"].ToString().Length != 4)
                            {
                                continue;
                            }
                            else
                            {
                                if (pvtEmployeeDataView[intRow]["EMPLOYEE_RES_COUNTRY_CODE2"].ToString() == "")
                                {
                                    continue;
                                }
                                else
                                {
                                    //2017-10-11
                                    if (pvtEmployeeDataView[intRow]["EMPLOYEE_RES_UNIT_NUMBER"].ToString().Trim().IndexOf(",") > -1)
                                    {
                                        continue;
                                    }

                                    if (pvtEmployeeDataView[intRow]["EMPLOYEE_RES_COMPLEX"].ToString().Trim().IndexOf(",") > -1)
                                    {
                                        continue;
                                    }

                                    if (pvtEmployeeDataView[intRow]["EMPLOYEE_RES_STREET_NUMBER"].ToString().Trim().IndexOf(",") > -1)
                                    {
                                        continue;
                                    }

                                    if (pvtEmployeeDataView[intRow]["EMPLOYEE_RES_STREET_NAME"].ToString().Trim().IndexOf(",") > -1)
                                    {
                                        continue;
                                    }
                                    
                                    if (pvtEmployeeDataView[intRow]["EMPLOYEE_RES_SUBURB"].ToString().Trim().IndexOf(",") > -1)
                                    {
                                        continue;
                                    }

                                    if (pvtEmployeeDataView[intRow]["EMPLOYEE_RES_CITY"].ToString().Trim().IndexOf(",") > -1)
                                    {
                                        continue;
                                    }

                                    if (pvtEmployeeDataView[intRow]["EMPLOYEE_RES_CODE"].ToString().Trim().IndexOf(",") > -1)
                                    {
                                        continue;
                                    }

                                    if (pvtEmployeeDataView[intRow]["EMPLOYEE_RES_COUNTRY_CODE2"].ToString().Trim().IndexOf(",") > -1)
                                    {
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    pvtEmployeePayCategoryAddressDataView = null;
                    pvtEmployeePayCategoryAddressDataView = new DataView(pvtDataSet.Tables["EmployeePayCategoryAddress"],
                    "PAY_CATEGORY_NO = " + pvtEmployeeDataView[intRow]["PAY_CATEGORY_NO"].ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtEmployeeDataView[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'",
                    "",
                    DataViewRowState.CurrentRows);

                    if (pvtEmployeePayCategoryAddressDataView.Count > 0)
                    {
                        //Check Pay
                        if (pvtEmployeePayCategoryAddressDataView[0]["RES_STREET_NAME"].ToString().Trim() == "")
                        {
                            blnPayCategoryAddressError = true;

                            continue;
                        }
                        else
                        {
                            if (pvtEmployeePayCategoryAddressDataView[0]["RES_SUBURB"].ToString().Trim() == ""
                            && pvtEmployeePayCategoryAddressDataView[0]["RES_CITY"].ToString().Trim() == "")
                            {
                                blnPayCategoryAddressError = true;

                                continue;
                            }
                            else
                            {
                                if (pvtEmployeePayCategoryAddressDataView[0]["RES_ADDR_CODE"].ToString() != "0000"
                                && pvtEmployeePayCategoryAddressDataView[0]["RES_ADDR_CODE"].ToString().Length != 4)
                                {
                                    blnPayCategoryAddressError = true;

                                    continue;
                                }
                                else
                                {
                                    //2017-10-11
                                    if (pvtEmployeePayCategoryAddressDataView[0]["RES_UNIT_NUMBER"].ToString().Trim().IndexOf(",") > -1)
                                    {
                                        continue;
                                    }
                                    
                                    if (pvtEmployeePayCategoryAddressDataView[0]["RES_COMPLEX"].ToString().Trim().IndexOf(",") > -1)
                                    {
                                        continue;
                                    }

                                    if (pvtEmployeePayCategoryAddressDataView[0]["RES_STREET_NUMBER"].ToString().Trim().IndexOf(",") > -1)
                                    {
                                        continue;
                                    }

                                    if (pvtEmployeePayCategoryAddressDataView[0]["RES_STREET_NAME"].ToString().Trim().IndexOf(",") > -1)
                                    {
                                        continue;
                                    }

                                    if (pvtEmployeePayCategoryAddressDataView[0]["RES_SUBURB"].ToString().Trim().IndexOf(",") > -1)
                                    {
                                        continue;
                                    }

                                    if (pvtEmployeePayCategoryAddressDataView[0]["RES_CITY"].ToString().Trim().IndexOf(",") > -1)
                                    {
                                        continue;
                                    }

                                    if (pvtEmployeePayCategoryAddressDataView[0]["RES_ADDR_CODE"].ToString().Trim().IndexOf(",") > -1)
                                    {
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }

                if (pvtEmployeeDataView[intRow]["EMPLOYEE_POST_OPTION_IND"].ToString() == "")
                {
                    continue;
                }
                else
                {
                    if (pvtEmployeeDataView[intRow]["EMPLOYEE_POST_OPTION_IND"].ToString() == "R")
                    {
                        //Same as Residential Address
                    }
                    else
                    {
                        if (pvtEmployeeDataView[intRow]["EMPLOYEE_POST_OPTION_IND"].ToString() == "S")
                        {
                            //Street Address
                            if (pvtEmployeeDataView[intRow]["EMPLOYEE_POST_STREET_NAME"].ToString().Trim() == "")
                            {
                                continue;
                            }
                            else
                            {
                                if (pvtEmployeeDataView[intRow]["EMPLOYEE_POST_SUBURB"].ToString().Trim() == ""
                                & pvtEmployeeDataView[intRow]["EMPLOYEE_POST_CITY"].ToString().Trim() == "")
                                {
                                    continue;
                                }
                                else
                                {
                                    if (pvtEmployeeDataView[intRow]["EMPLOYEE_POST_CODE"].ToString() != "0000"
                                        & pvtEmployeeDataView[intRow]["EMPLOYEE_RES_CODE"].ToString().Length != 4)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        if (pvtEmployeeDataView[intRow]["EMPLOYEE_POST_COUNTRY_CODE2"].ToString() == "")
                                        {
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (pvtEmployeeDataView[intRow]["EMPLOYEE_POST_OPTION_IND"].ToString() == "P"
                            || pvtEmployeeDataView[intRow]["EMPLOYEE_POST_OPTION_IND"].ToString() == "B")
                            {
                                //P=PO Box
                                //B=Private Bag 
                                if (pvtEmployeeDataView[intRow]["EMPLOYEE_POST_STREET_NUMBER"].ToString() == ""
                                    || pvtEmployeeDataView[intRow]["EMPLOYEE_POST_SUBURB"].ToString() == ""
                                    || pvtEmployeeDataView[intRow]["EMPLOYEE_POST_CODE"].ToString() == ""
                                    || pvtEmployeeDataView[intRow]["EMPLOYEE_POST_COUNTRY_CODE2"].ToString() == "")
                                {
                                    continue;
                                }
                                else
                                {
                                    if (pvtEmployeeDataView[intRow]["EMPLOYEE_POST_CODE"].ToString().Length != 4)
                                    {
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }

                //if (pvtEmployeeDataView[intRow]["EMPLOYEE_POST_ADDR_LINE1"].ToString() == ""
                //    & pvtEmployeeDataView[intRow]["EMPLOYEE_POST_ADDR_LINE2"].ToString() == ""
                //    & pvtEmployeeDataView[intRow]["EMPLOYEE_POST_ADDR_LINE2"].ToString() == ""
                //    & pvtEmployeeDataView[intRow]["EMPLOYEE_POST_CODE"].ToString() == "")
                //{
                //}
                //else
                //{
                //    if (pvtEmployeeDataView[intRow]["EMPLOYEE_POST_ADDR_LINE1"].ToString() == "")
                //    {
                //        continue;

                //    }

                //    if (pvtEmployeeDataView[intRow]["EMPLOYEE_POST_CODE"].ToString() != "0000"
                //    & pvtEmployeeDataView[intRow]["EMPLOYEE_POST_CODE"].ToString().Length != 4)
                //    {
                //        continue;
                //    }

                //}

                if (pvtEmployeeDataView[intRow]["EMPLOYEE_TEL_HOME"].ToString() != "")
                {
                    if (pvtEmployeeDataView[intRow]["EMPLOYEE_TEL_HOME"].ToString().Length < 10)
                    {
                        continue;
                    }
                }

                if (pvtEmployeeDataView[intRow]["EMPLOYEE_TEL_CELL"].ToString() != "")
                {
                    if (pvtEmployeeDataView[intRow]["EMPLOYEE_TEL_CELL"].ToString().Length < 10)
                    {
                        continue;
                    }
                }

                if (pvtEmployeeDataView[intRow]["USE_WORK_TEL_IND"].ToString() != "Y")
                {
                    if (pvtEmployeeDataView[intRow]["EMPLOYEE_TEL_WORK"].ToString().Length < 10)
                    {
                        continue;
                    }
                }

                int intReturnCode = clsISUtilities.eFiling_Income_Tax_Ref_No(pvtEmployeeDataView[intRow]["EMPLOYEE_TAX_NO"].ToString());

                if (intReturnCode != 0)
                {
                    continue;
                }

                //None / Foreign Bank Account
                if (pvtEmployeeDataView[intRow]["BANK_ACCOUNT_TYPE_NO"].ToString() != "0"
                & pvtEmployeeDataView[intRow]["BANK_ACCOUNT_TYPE_NO"].ToString() != "7")
                {
                    //Own / Joint / Third Party
                    if (pvtEmployeeDataView[intRow]["BANK_ACCOUNT_RELATIONSHIP_TYPE_NO"].ToString() != "1"
                    & pvtEmployeeDataView[intRow]["BANK_ACCOUNT_RELATIONSHIP_TYPE_NO"].ToString() != "2"
                    & pvtEmployeeDataView[intRow]["BANK_ACCOUNT_RELATIONSHIP_TYPE_NO"].ToString() != "3")
                    {
                        continue;
                    }

                    if (pvtEmployeeDataView[intRow]["BRANCH_CODE"].ToString() == ""
                    || pvtEmployeeDataView[intRow]["BRANCH_DESC"].ToString() == ""
                    || pvtEmployeeDataView[intRow]["BRANCH_DESC"].ToString().IndexOf(",") > -1
                    || pvtEmployeeDataView[intRow]["ACCOUNT_NO"].ToString() == ""
                    || pvtEmployeeDataView[intRow]["ACCOUNT_NAME"].ToString() == ""
                    || pvtEmployeeDataView[intRow]["ACCOUNT_NAME"].ToString().IndexOf(",") > -1
                    || pvtEmployeeDataView[intRow]["BANK_NO"].ToString() == "")
                    {
                        continue;
                    }
                }

                if (pvtEmployeeDataView[intRow]["SIC7_GROUP_CODE"].ToString() == "")
                {
                    continue;
                }

                //Passed All Checks
                pvtEmployeeDataView[intRow]["OK_IND"] = "Y";
            }

            if (this.rbnEFilingCheck.Checked == true)
            {
                if (pvtEmployeeDataView.Count > 0)
                {
                    blnEmployeeExists = true;
                }

                pvtEmployeeDataView.RowFilter += " AND OK_IND = 'N'"; 
            }

            this.pvtDataSet.AcceptChanges();

            clsISUtilities.DataViewIndex = 0;

            if (clsISUtilities.DataBind_Form_And_DataView_To_Class() == false)
            {
                clsISUtilities.DataBind_DataView_And_Index(this, pvtEmployeeDataView, "EMPLOYEE_NO");

                //TabPage 0
                clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(txtCode, "EMPLOYEE_CODE", false, "", false);
                clsISUtilities.DataBind_Control_Required_If_Enabled(txtCode, "Enter Employee Code");

                clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(txtName, "EMPLOYEE_NAME", true, "Enter Employee Name.", true);
                clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(txtSurname, "EMPLOYEE_SURNAME", true, "Enter Employee Surname.", true);
                clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(this.txtInitials, "EMPLOYEE_INITIALS", true, "Enter Employee Initials.", true);

                clsISUtilities.DataBind_DataView_To_ComboBox(cboNatureOfPerson, "NATURE_PERSON_NO", true, "Select Nature of Person.", true);

                clsISUtilities.DataBind_DataView_To_Numeric_TextBox(txtIDNo, "EMPLOYEE_ID_NO", 0, false, "", false, 0,true);
                clsISUtilities.DataBind_Control_Required_If_Enabled(txtIDNo, "Enter Valid Employee ID. Number.");
                clsISUtilities.DataBind_Numeric_Field_SA_ID_Number(txtIDNo);

                clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(txtPassportNo, "EMPLOYEE_PASSPORT_NO", false, "", false);
                clsISUtilities.DataBind_Control_Required_If_Enabled(txtPassportNo, "Enter Passport Number.");

                clsISUtilities.DataBind_DataView_To_ComboBox(cboCountry, "EMPLOYEE_PASSPORT_COUNTRY_CODE", false, "", false);
                clsISUtilities.DataBind_Control_Required_If_Enabled(cboCountry, "Select Country where Passport was Issued.");

                clsISUtilities.DataBind_DataView_To_Date_TextBox(txtBirthDate, "EMPLOYEE_BIRTHDATE", true, "Capture Birth Date.");

                clsISUtilities.DataBind_DataView_To_Date_TextBox_ReadOnly(txtStartDate, "EMPLOYEE_TAX_STARTDATE");
                clsISUtilities.DataBind_DataView_To_Date_TextBox_ReadOnly(txtEffectiveDate, "EMPLOYEE_LAST_RUNDATE");
                
                clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(txtResAddrUnitNumber, "EMPLOYEE_RES_UNIT_NUMBER", false, "", false);
                clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(txtResAddrComplex, "EMPLOYEE_RES_COMPLEX", false, "", false);
                clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(txtResAddrStreetNumber, "EMPLOYEE_RES_STREET_NUMBER", false, "", false);
                
                clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(this.txtStreetName, "EMPLOYEE_RES_STREET_NAME", true, "Enter Enter Street Name / Farm Name.", true);
                clsISUtilities.DataBind_Special_Field(this.txtStreetName, 1);

                clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(this.txtSuburb, "EMPLOYEE_RES_SUBURB", false, "", true);
                clsISUtilities.DataBind_Special_Field(this.txtSuburb, 1);
                clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(this.txtCity, "EMPLOYEE_RES_CITY", false, "", true);
                clsISUtilities.DataBind_Special_Field(this.txtCity, 1);

                clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(this.txtPhysicalCode, "EMPLOYEE_RES_CODE", false, "", true);
                clsISUtilities.DataBind_Special_Field(this.txtPhysicalCode, 1);

                clsISUtilities.DataBind_DataView_To_ComboBox(this.cboAddressCountry, "EMPLOYEE_RES_COUNTRY_CODE2", false, "", false);
                clsISUtilities.DataBind_Control_Required_If_Enabled(this.cboAddressCountry, "Select Country of Residential Address.");

                //ELR - 20141014
                clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(txtPostAddrUnitNumber, "EMPLOYEE_POST_UNIT_NUMBER", false, "", false);
                clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(txtPostAddrComplex, "EMPLOYEE_POST_COMPLEX", false, "", false);
                clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(txtPostAddrStreetNumber, "EMPLOYEE_POST_STREET_NUMBER", false, "", false);

                clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(this.txtPostStreetName, "EMPLOYEE_POST_STREET_NAME", false, "", false);
                clsISUtilities.DataBind_Special_Field(this.txtPostStreetName, 3);

                clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(this.txtPostSuburb, "EMPLOYEE_POST_SUBURB", false, "", true);
                clsISUtilities.DataBind_Special_Field(this.txtPostSuburb, 3);
                clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(this.txtPostCity, "EMPLOYEE_POST_CITY", false, "", true);
                clsISUtilities.DataBind_Special_Field(this.txtPostCity, 3);

                clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(this.txtPostAddrCode, "EMPLOYEE_POST_CODE", false, "", true);
                clsISUtilities.DataBind_Special_Field(this.txtPostAddrCode, 3);

                clsISUtilities.DataBind_DataView_To_ComboBox(this.cboPostCountry, "EMPLOYEE_POST_COUNTRY_CODE2", false, "", false);
                clsISUtilities.DataBind_Control_Required_If_Enabled(this.cboPostCountry, "Select Country of Postal Address.");
           
                clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(txtEmail, "EMPLOYEE_EMAIL", false, "", true);

                //TabPage 1
                clsISUtilities.DataBind_DataView_To_TextBox(txtTaxDirectiveNo1, "TAX_DIRECTIVE_NO1", false, "", false);
                clsISUtilities.DataBind_Control_Required_If_Enabled(txtTaxDirectiveNo1, "Enter Tax Directive Number.");

                clsISUtilities.DataBind_DataView_To_TextBox(txtTaxDirectiveNo2, "TAX_DIRECTIVE_NO2", false, "", false);
                clsISUtilities.DataBind_DataView_To_TextBox(txtTaxDirectiveNo3, "TAX_DIRECTIVE_NO3", false, "", false);

                clsISUtilities.DataBind_DataView_To_Numeric_TextBox(txtTaxDirectivePercentAmount, "TAX_DIRECTIVE_PERCENTAGE", 2, false, "", false, 99.99,false);
                clsISUtilities.DataBind_Control_Required_If_Enabled(txtTaxDirectivePercentAmount, "Enter Tax Directive Percentage.");

                clsISUtilities.DataBind_DataView_To_Numeric_TextBox(txtTaxRefNo, "EMPLOYEE_TAX_NO", 0, false, "", true, 0, true);
                clsISUtilities.DataBind_DataView_Field_EFiling(txtTaxRefNo);

                clsISUtilities.DataBind_DataView_To_ComboBox(cboBank, "BANK_NO", true, "Select Bank.", true);
                clsISUtilities.DataBind_DataView_To_ComboBox(cboBankAccountType, "BANK_ACCOUNT_TYPE_NO", true, "Select Bank Account Type.", true);

                clsISUtilities.DataBind_DataView_To_Numeric_TextBox(txtBranchCode, "BRANCH_CODE", 0, false, "", false, 0, true);
                clsISUtilities.DataBind_Control_Required_If_Enabled(txtBranchCode, "Enter Branch Code.");

                //ELR - 2015-02-28
                clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(this.txtBranchDesc, "BRANCH_DESC", false, "", false);
                clsISUtilities.DataBind_Control_Required_If_Enabled(txtBranchDesc, "Enter Branch Name.");

                clsISUtilities.DataBind_DataView_To_Numeric_TextBox(txtAccountNo, "ACCOUNT_NO", 0, false, "", false, 0, true);
                clsISUtilities.DataBind_Control_Required_If_Enabled(txtAccountNo, "Enter Account Number.");

                clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(txtAccountName, "ACCOUNT_NAME", false, "", false);
                clsISUtilities.DataBind_Control_Required_If_Enabled(txtAccountName, "Enter Account Name.");

                clsISUtilities.DataBind_DataView_To_ComboBox(cboBankAccountRelationship, "BANK_ACCOUNT_RELATIONSHIP_TYPE_NO", false, "", false);
                clsISUtilities.DataBind_Control_Required_If_Enabled(cboBankAccountRelationship, "Select Account Relationship.");

                clsISUtilities.NotDataBound_ComboBox_EFiling(this.cboSic7Group, "Select Sic7 Group and the Sic7 Code.");

                clsISUtilities.DataBind_DataView_To_Numeric_TextBox_Min_Length(txtTelHome, "EMPLOYEE_TEL_HOME", 0, 10, false, "Enter Home Tel. Number.", true, 0, true);

                clsISUtilities.DataBind_DataView_To_Numeric_TextBox_Min_Length(txtTelWork, "EMPLOYEE_TEL_WORK", 0, 10, false, "", false, 0, true);
                clsISUtilities.DataBind_Control_Required_If_Enabled(txtTelWork, "Enter Work Telephone Number.");

                clsISUtilities.DataBind_DataView_To_Numeric_TextBox_Min_Length(txtTelCell, "EMPLOYEE_TEL_CELL", 0, 10, false, "Enter Cell Tel. Number.", true, 0, true);
            }
            else
            {
                clsISUtilities.Re_DataBind_DataView(pvtEmployeeDataView);
            }

            this.Clear_DataGridView(this.dgvEmployeeDataGridView);

            this.pvtblnEmployeeDataGridViewLoaded = false;
            pvtintEmployeeDataGridViewRowIndex = -1;

            for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
            {
                this.dgvEmployeeDataGridView.Rows.Add("",
                                                      pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                      pvtEmployeeDataView[intRow]["PAY_CATEGORY_TYPE"].ToString().Substring(0, 1),
                                                      pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                      pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                      intRow.ToString());

                if (pvtEmployeeDataView[intRow]["OK_IND"].ToString() == "N")
                {
                    this.dgvEmployeeDataGridView[0,intRow].Style = eFilingErrorDataGridViewCellStyle;
                }
            }

            this.pvtblnEmployeeDataGridViewLoaded = true;

            if (dgvEmployeeDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, 0);
            }
            else
            {
                if (this.rbnEFilingCheck.Checked == true
                    & blnEmployeeExists == true)
                {
                    this.btnCreateEfile.Enabled = true;
                }
            }

            if (blnPayCategoryAddressError == true)
            {
                this.btnUpdate.Enabled = false;
                CustomMessageBox.Show("Error in a Cost Centre Address\nFix To Continue.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

                    this.cboBank.Enabled = true;
                }
                else
                {
                    this.txtBranchCode.Enabled = false;
                    this.txtBranchDesc.Enabled = false;
                    this.txtAccountNo.Enabled = false;
                    this.txtAccountName.Enabled = false;
                    this.cboBankAccountRelationship.Enabled = false;
                    this.cboBank.Enabled = false;

                    this.txtBranchCode.Text = "0";
                    this.txtBranchDesc.Text = "";
                    this.txtAccountNo.Text = "0";
                    this.txtAccountName.Text = "";
                    this.cboBankAccountRelationship.SelectedIndex = -1;
                    this.cboBank.SelectedIndex = -1;
                }
            }
        }

        private void Clear_Form_Fields()
        {
            //TabPage 0
            this.txtCode.Text = "";
            this.txtName.Text = "";
            this.txtSurname.Text = "";
            this.txtInitials.Text = "";
            this.cboNatureOfPerson.SelectedIndex = -1;
            this.txtIDNo.Text = "";
            this.txtPassportNo.Text = "";
            this.cboCountry.SelectedIndex = -1;
            this.txtBirthDate.Text = "";
            this.txtStartDate.Text = "";
            this.txtEffectiveDate.Text = "";

            this.Clear_Postal_Fields();
           
            //TabPage 1
            this.txtTaxDirectiveNo1.Text = "";
            this.txtTaxDirectiveNo2.Text = "";
            this.txtTaxDirectiveNo3.Text = "";
            this.txtTaxDirectivePercentAmount.Text = "";
            this.txtTaxRefNo.Text = "";
            this.cboBank.SelectedIndex = -1;
            this.cboBankAccountType.SelectedIndex = -1;
            this.txtBranchCode.Text = "";
            this.txtBranchDesc.Text = "";
            this.txtAccountNo.Text = "";
            this.txtAccountName.Text = "";
            this.cboBankAccountRelationship.SelectedIndex = -1;
            this.cboSic7Group.SelectedIndex = -1;
            this.txtSic7Code.Text = "";
            this.txtTelHome.Text = "";
            this.txtTelWork.Text = "";
            this.txtTelCell.Text = "";
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
                    case "dgvEmployeeDataGridView":

                        pvtintEmployeeDataGridViewRowIndex = -1;
                        this.dgvEmployeeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPeriodDataGridView":
                        
                        pvtintPeriodDataGridViewRowIndex = -1;
                        this.dgvPeriodDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvSic7DataGridView":

                        pvtintSic7DataGridViewRowIndex = -1;
                        this.dgvSic7DataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        private void Set_Form_For_Read()
        {
            if (this.Text.IndexOf("- Update") > -1)
            {
                this.Text = this.Text.Substring(0, this.Text.IndexOf("- Update") - 1);
            }
           
            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            clsISUtilities.Set_Form_For_Read();

            //Tio Be Looked at
            this.btnUpdate.Enabled = true;

            //ELR - 2014-08-27
            this.cboSic7Group.Enabled = false;
            this.btnSetSic7Code.Enabled = false;
            this.dgvSic7DataGridView.Enabled = false;
            this.txtSic7Code.Enabled = false;

            this.dgvEmployeeDataGridView.Enabled = true;
            this.dgvPeriodDataGridView.Enabled = true;

            this.picEmployeeLock.Visible = false;
            this.picPeriodLock.Visible = false;

            this.rbnResAddrOwn.Enabled = false;
            this.rbnResAddrCompany.Enabled = false;
            this.rbnWorkTelOwn.Enabled = false;
            this.rbnWorkTelCompany.Enabled = false;

            this.rbnSameResidentialAddr.Enabled = false;
            this.rbnStreetAddr.Enabled = false;
            this.rbnPOBoxAddr.Enabled = false;
            this.rbnPrivateBagAddr.Enabled = false;

            this.btnPhsicalAddressRSA.Enabled = false;
            this.btnPostalAddressRSA.Enabled = false;

            this.txtPostAddrStreetNumber.Enabled = false;
            this.txtPostStreetName.Enabled = false;

            this.rbnTaxNormal.Enabled = false;
            this.rbnTaxPartTime.Enabled = false;
            this.rbnTaxDirective.Enabled = false;

            this.rbnNormal.Enabled = true;
            this.rbnEFilingCheck.Enabled = true;
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

        private void dgvEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnEmployeeDataGridViewLoaded == true)
            {
                if (pvtintEmployeeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintEmployeeDataGridViewRowIndex = e.RowIndex;

                    //Set DataBound Index
                    clsISUtilities.DataViewIndex = Convert.ToInt32(this.dgvEmployeeDataGridView[5, e.RowIndex].Value);

                    //NB clsISUtilities.DataViewIndex is used Below
                    clsISUtilities.DataBind_DataView_Record_Show();

                    pvtintEmployeeNo = Convert.ToInt32(pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_NO"]);

                    if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["USE_RES_ADDR_COMPANY_IND"].ToString() == "Y")
                    {
                        this.rbnResAddrCompany.Checked = true;
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
                                    else
                                    {
                                        //Not Valid Value
                                        this.rbnSameResidentialAddr.Checked = false;
                                        this.rbnStreetAddr.Checked = false;
                                        this.rbnPOBoxAddr.Checked = false;
                                        this.rbnPrivateBagAddr.Checked = false;

                                        Set_Postal_Address_Default();
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
                                this.rbnTaxNormal.Checked = false;
                                this.rbnTaxPartTime.Checked = false;
                                this.rbnTaxDirective.Checked = false;
                            }
                        }
                    }

                    //ELR 2014-08-24
                    pvtstrSic7GroupCode = "";

                    this.cboSic7Group.SelectedIndex = -1;
                    Clear_DataGridView(this.dgvSic7DataGridView);
                    this.txtSic7Code.Text = "";

                    if (pvtEmployeeDataView[clsISUtilities.DataViewIndex]["SIC7_GROUP_CODE"].ToString() != "")
                    {
                        //ELR 2014-08-24
                        pvtstrSic7GroupCode = pvtEmployeeDataView[clsISUtilities.DataViewIndex]["SIC7_GROUP_CODE"].ToString();

                        DataView myDataView = new DataView(pvtDataSet.Tables["Sic7Code"],
                                                           "SIC7_GROUP_CODE = '" + pvtstrSic7GroupCode + "'",
                                                           "",
                                                           DataViewRowState.CurrentRows);

                        if (myDataView.Count > 0)
                        {
                            for (int intRow = 0; intRow < this.pvtDataSet.Tables["Sic7CodeGroup"].Rows.Count; intRow++)
                            {
                                if (Convert.ToInt32(this.pvtDataSet.Tables["Sic7CodeGroup"].Rows[intRow]["SIC7_GROUP_NO"]) == Convert.ToInt32(myDataView[0]["SIC7_GROUP_NO"]))
                                {
                                    this.cboSic7Group.SelectedIndex = intRow;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            CustomMessageBox.Show("Cannot Find Sic7 Code\nSpewak to Administrator", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Filter_Click(object sender, EventArgs e)
        {
            if (this.rbnEFilingCheck.Checked == true)
            {
                clsISUtilities.Set_eFiling_Indicator(true);
            }
            else
            {
                clsISUtilities.Set_eFiling_Indicator(false);
            }

            this.Load_CurrentForm_Records();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            this.Text += " - Update";

            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                Set_Form_For_Edit();
            }
            else
            {
                this.btnCreateEfile.Enabled = true;
            }
        }

        private void Set_Form_For_Edit()
        {
            this.dgvEmployeeDataGridView.Enabled = false;
            this.dgvPeriodDataGridView.Enabled = false;
           
            this.picEmployeeLock.Visible = true;
            this.picPeriodLock.Visible = true;
           
            clsISUtilities.Set_Form_For_Edit(false);

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

            this.rbnResAddrOwn.Enabled = true;
            this.rbnResAddrCompany.Enabled = true;
            this.rbnWorkTelOwn.Enabled = true;
            this.rbnWorkTelCompany.Enabled = true;

            this.rbnSameResidentialAddr.Enabled = true;
            this.rbnStreetAddr.Enabled = true;
            this.rbnPOBoxAddr.Enabled = true;
            this.rbnPrivateBagAddr.Enabled = true;

            this.rbnTaxNormal.Enabled = true;
            this.rbnTaxPartTime.Enabled = true;
            this.rbnTaxDirective.Enabled = true;
        
            this.btnUpdate.Enabled = false;
            
            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.rbnNormal.Enabled = false;
            this.rbnEFilingCheck.Enabled = false;

            //ELR - 2014-08-27
            this.cboSic7Group.Enabled = true;
            this.btnSetSic7Code.Enabled = true;
            this.dgvSic7DataGridView.Enabled = true;
            this.txtSic7Code.Enabled = true;

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
                        }
                    }
                }
            }

            //Fire Work Telephone Enable
            Work_Telephone_Click(null, e);

            //Fire Tax Type
            TaxType_Click(null, e);

            ////Fire Account Type which will Enable Linked Fields
            cboAccountType_SelectedIndexChanged(null, e);

            this.txtInitials.Focus();

            this.tabEmployee.Refresh();
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


        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.pvtDataSet.RejectChanges();

            Set_Form_For_Read();

            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView));
            }
        }

        private void dgvPeriodDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 0)
            {
                if (dgvPeriodDataGridView[e.Column.Index + 2, e.RowIndex1].Value.ToString() == "")
                {
                    e.SortResult = -1;
                }
                else
                {
                    if (dgvPeriodDataGridView[e.Column.Index + 2, e.RowIndex2].Value.ToString() == "")
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(dgvPeriodDataGridView[e.Column.Index + 2, e.RowIndex1].Value.ToString().Replace("-", "")) > double.Parse(dgvPeriodDataGridView[e.Column.Index + 2, e.RowIndex2].Value.ToString().Replace("-", "")))
                        {
                            e.SortResult = 1;
                        }
                        else
                        {
                            if (double.Parse(dgvPeriodDataGridView[e.Column.Index + 2, e.RowIndex1].Value.ToString().Replace("-", "")) < double.Parse(dgvPeriodDataGridView[e.Column.Index + 2, e.RowIndex2].Value.ToString().Replace("-", "")))
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

        private void dgvPeriodDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPeriodDataGridViewLoaded == true)
            {
                if (pvtintPeriodDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPeriodDataGridViewRowIndex = e.RowIndex;

                    if (this.dgvPeriodDataGridView[1, e.RowIndex].Value.ToString() == "No")
                    {
                        if (this.dgvPeriodDataGridView[3, e.RowIndex].Value.ToString() == "Y")
                        {
                            this.btnUpdate.Enabled = true;
                        }
                        else
                        {
                            this.btnUpdate.Enabled = false;
                        }
                    }
                    else
                    {
                        this.btnUpdate.Enabled = false;
                    }

                    this.txtFileName.Text = this.pvtDataSet.Tables["Company"].Rows[0]["COMPANY_DESC"].ToString().Replace(" ", "_") + "_" + DateTime.ParseExact(this.dgvPeriodDataGridView[2, e.RowIndex].Value.ToString(), "yyyy-MM-dd", null).ToString("MMMM_yyyy") + ".csv";
                }
            }
        }

        private void btnOpenPayrollRun_Click(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.SetData("MenuClick", "companyToolStripMenuItem");

            Timer TimerMenuClick = (Timer)AppDomain.CurrentDomain.GetData("TimerMenuClick");

            TimerMenuClick.Enabled = true;

            this.Close();
        }

        private void btnCreateEfile_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.pvtDataSet.Tables["Company"].Rows[0]["SIC7_GROUP_CODE"].ToString() == "")
                {
                    CustomMessageBox.Show("Company Does NOT have a SIC7 Code.\nSpeak to Administrator", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                FileInfo fiIRP5File = new FileInfo(this.txtDirectory.Text + "\\" + this.txtFileName.Text);

                DialogResult myDialogResult;

                if (fiIRP5File.Exists == true)
                {
                    myDialogResult = CustomMessageBox.Show("File ALREADY Exists. Would you like to overwrite this file?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                }
                else
                {
                    myDialogResult = CustomMessageBox.Show("Would you like to Create this File Now?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                }

                if (myDialogResult == DialogResult.Yes)
                {
                    if (fiIRP5File.Exists == true)
                    {
                        File.Delete(this.txtDirectory.Text + "\\" + this.txtFileName.Text);
                    }

                    StreamWriter swIRP5StreamWriter = fiIRP5File.AppendText();

                    string strRecord = "";
                    string strNatureOfPerson = "";
                    bool blnIRP5Exists = false;

                    pvtint64TotalEmployeeCodes = 0;

                    Double dblTotalEmployeeAmounts = 0;

                    //Int64 int64TotalEmployeeEarnings = 0;
                    Int64 int64TotalEmployeeDeductionContributions = 0;
                    
                    int intGrossTaxableAnnualPayments = 0;
                    int intGrossNonTaxableIncome = 0;
                    double dblEmployeeDeductionTaxUif = 0;
                    int intTotalEmployeeGrossEmploymentIncome = 0;
                    double dblSDL = 0;

                    double dblEmployeeTax = 0;
                   
                    int intGrossIncome = 0;
                    
                    //Trading Name
                    strRecord += Write_Header_Code("2010");
                    strRecord += Write_Text(this.pvtDataSet.Tables["Company"].Rows[0]["COMPANY_DESC"].ToString());
                    strRecord += Write_Header_Code_End();

                    //LIVE / TEST Indicator
                    strRecord += Write_Header_Code("2015");
#if(DEBUG)
                    strRecord += Write_Text("LIVE");
#else
                    strRecord += Write_Text("LIVE");                    
#endif
                    strRecord += Write_Header_Code_End();

                    //PAYE Reference Number
                    strRecord += Write_Header_Code("2020");
                    strRecord += Write_Numeric(this.pvtDataSet.Tables["Company"].Rows[0]["PAYE_NO"].ToString());
                    strRecord += Write_Header_Code_End();

                    if (this.pvtDataSet.Tables["Company"].Rows[0]["SDL_REF_NO"].ToString() != "")
                    {
                        //SDL Reference Number
                        strRecord += Write_Header_Code("2022");
                        strRecord += Write_Text("L" + this.pvtDataSet.Tables["Company"].Rows[0]["SDL_REF_NO"].ToString());
                        strRecord += Write_Header_Code_End();
                    }

                    if (this.pvtDataSet.Tables["Company"].Rows[0]["UIF_REF_NO"].ToString() != "")
                    {
                        //UIF Reference Number
                        strRecord += Write_Header_Code("2024");
                        strRecord += Write_Text("U" + this.pvtDataSet.Tables["Company"].Rows[0]["UIF_REF_NO"].ToString());
                        strRecord += Write_Header_Code_End();
                    }

                    //Employer Name Surname
                    strRecord += Write_Header_Code("2025");
                    strRecord += Write_Text(this.pvtDataSet.Tables["Company"].Rows[0]["EFILING_NAMES"].ToString());
                    strRecord += Write_Header_Code_End();

                    //Employer Contact Number
                    strRecord += Write_Header_Code("2026");
                    strRecord += Write_Numeric(this.pvtDataSet.Tables["Company"].Rows[0]["EFILING_CONTACT_NO"].ToString());
                    strRecord += Write_Header_Code_End();

                    if (this.pvtDataSet.Tables["Company"].Rows[0]["EFILING_EMAIL"].ToString() != "")
                    {
                        //Employer Email
                        strRecord += Write_Header_Code("2027");
                        strRecord += Write_Numeric(this.pvtDataSet.Tables["Company"].Rows[0]["EFILING_EMAIL"].ToString());
                        strRecord += Write_Header_Code_End();
                    }

                    //Payroll Software
                    strRecord += Write_Header_Code("2028");
                    strRecord += Write_Text("in-house");
                    strRecord += Write_Header_Code_End();

                    DateTime dtPeriodDateTime = DateTime.ParseExact(this.dgvPeriodDataGridView[2, this.Get_DataGridView_SelectedRowIndex(this.dgvPeriodDataGridView)].Value.ToString(), "yyyy-MM-dd", null);

                    DateTime dtBeginningOfYear = DateTime.Now;
                    DateTime dtEndOfYear = DateTime.Now;

                    if (dtPeriodDateTime.Month == 8)
                    {
                        dtBeginningOfYear = new DateTime(dtPeriodDateTime.Year, 3, 1);
                        dtEndOfYear = dtBeginningOfYear.AddMonths(6).AddDays(-1);
                    }
                    else
                    {
                        dtBeginningOfYear = new DateTime(dtPeriodDateTime.Year - 1, 3, 1);
                        dtEndOfYear = dtBeginningOfYear.AddYears(1).AddDays(-1);
                    }

                    //Transaction Year
                    strRecord += Write_Header_Code("2030");

                    if (dtPeriodDateTime.Month == 8)
                    {
                        strRecord += Write_Numeric(dtPeriodDateTime.AddYears(1).ToString("yyyy"));
                    }
                    else
                    {
                        strRecord += Write_Numeric(dtPeriodDateTime.ToString("yyyy"));
                    }

                    strRecord += Write_Header_Code_End();

                    //Period Of Reconciliation
                    strRecord += Write_Header_Code("2031");
                    strRecord += Write_Numeric(dtPeriodDateTime.ToString("yyyyMM"));
                    strRecord += Write_Header_Code_End();

                    //ELR - 2014-08-27
                    //Employer SIC7 Code (Standard Industry Classification Code)
                    strRecord += Write_Header_Code("2082");
                    strRecord += Write_Text(this.pvtDataSet.Tables["Company"].Rows[0]["SIC7_GROUP_CODE"].ToString());
                    strRecord += Write_Header_Code_End();

                    //Trade Classification Code
                    strRecord += Write_Header_Code("2035");
                    strRecord += Write_Numeric(this.pvtDataSet.Tables["Company"].Rows[0]["TRADE_CLASSIFICATION_CODE"].ToString());
                    strRecord += Write_Header_Code_End();
                  
                    if (this.pvtDataSet.Tables["Company"].Rows[0]["RES_UNIT_NUMBER"].ToString() != "")
                    {
                        //Unit Number
                        strRecord += Write_Header_Code("2061");
                        strRecord += Write_Numeric(this.pvtDataSet.Tables["Company"].Rows[0]["RES_UNIT_NUMBER"].ToString());
                        strRecord += Write_Header_Code_End();
                    }

                    if (this.pvtDataSet.Tables["Company"].Rows[0]["RES_COMPLEX"].ToString() != "")
                    {
                        //Complex
                        strRecord += Write_Header_Code("2062");
                        strRecord += Write_Numeric(this.pvtDataSet.Tables["Company"].Rows[0]["RES_COMPLEX"].ToString());
                        strRecord += Write_Header_Code_End();
                    }

                    if (this.pvtDataSet.Tables["Company"].Rows[0]["RES_STREET_NUMBER"].ToString() != "")
                    {
                        //Street Number
                        strRecord += Write_Header_Code("2063");
                        strRecord += Write_Text(this.pvtDataSet.Tables["Company"].Rows[0]["RES_STREET_NUMBER"].ToString());
                        strRecord += Write_Header_Code_End();
                    }

                    //Street Name / Farm Name
                    strRecord += Write_Header_Code("2064");
                    strRecord += Write_Text(this.pvtDataSet.Tables["Company"].Rows[0]["RES_STREET_NAME"].ToString());
                    strRecord += Write_Header_Code_End();

                    if (this.pvtDataSet.Tables["Company"].Rows[0]["RES_SUBURB"].ToString() != "")
                    {
                        //Suburb / District
                        strRecord += Write_Header_Code("2065");
                        strRecord += Write_Text(this.pvtDataSet.Tables["Company"].Rows[0]["RES_SUBURB"].ToString());
                        strRecord += Write_Header_Code_End();
                    }

                    if (this.pvtDataSet.Tables["Company"].Rows[0]["RES_CITY"].ToString() != "")
                    {
                        //City
                        strRecord += Write_Header_Code("2066");
                        strRecord += Write_Text(this.pvtDataSet.Tables["Company"].Rows[0]["RES_CITY"].ToString());
                        strRecord += Write_Header_Code_End();
                    }

                    //Physical Address Post Code
                    strRecord += Write_Header_Code("2080");
                    strRecord += Write_Numeric(this.pvtDataSet.Tables["Company"].Rows[0]["RES_ADDR_CODE"].ToString());
                    strRecord += Write_Header_Code_End();

                    //2016-09-14
                    //Employer Country Code
                    strRecord += Write_Header_Code("2081");
                    strRecord += Write_Text("ZA");
                    strRecord += Write_Header_Code_End();

                    strRecord += Write_Record_End();
                    swIRP5StreamWriter.WriteLine(strRecord);

                    int intFindRow = 0;
                    bool blnHaveWrittenSDL = false;
                    bool blnMedicalSchemeFees = false; 

                    pvtEmployeeDataView.RowFilter = pvtEmployeeDataView.RowFilter.Replace(" AND OK_IND = 'N'", "");

                    //Start Of Employee Information
                    //Start Of Employee Information
                    for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
                    {
                        if (pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString() == "18")
                        {
                            string strStop = "";
                        }

                        strRecord = "";

                        strNatureOfPerson = "";
                        
                        //int64TotalEmployeeEarnings = 0;
                        int64TotalEmployeeDeductionContributions = 0;
                        
                        intGrossIncome = 0;
                        intGrossTaxableAnnualPayments = 0;
                        intGrossNonTaxableIncome = 0;
                        dblEmployeeTax = 0;
                        dblEmployeeDeductionTaxUif = 0;
                        intTotalEmployeeGrossEmploymentIncome = 0;

                        blnHaveWrittenSDL = false;
                        blnMedicalSchemeFees = false; 
                       
                        strRecord += Write_Header_Code("3010");

                        if (dtPeriodDateTime.Month == 8)
                        {
                            strRecord += Write_Text(this.pvtDataSet.Tables["Company"].Rows[0]["PAYE_NO"].ToString() + dtPeriodDateTime.AddYears(1).ToString("yyyyMM") + pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString().PadLeft(14, '0'));
                        }
                        else
                        {
                            strRecord += Write_Text(this.pvtDataSet.Tables["Company"].Rows[0]["PAYE_NO"].ToString() + dtPeriodDateTime.ToString("yyyyMM") + pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString().PadLeft(14, '0'));
                        }

                        strRecord += Write_Header_Code_End();
                                          
                        strRecord += Write_Header_Code("3015");

                        if (pvtEmployeeDataView[intRow]["EMPLOYEE_TAX_NO"].ToString() != "")
                        {
                            pvtEmployeeDeductionTaxUifDataView = new DataView(pvtDataSet.Tables["EmployeeDeductionTaxUif"],
                            "EMPLOYEE_NO = " + pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString() + " AND IRP5_CODE = '4102'",
                            "",
                            DataViewRowState.CurrentRows);

                            if (pvtEmployeeDeductionTaxUifDataView.Count == 0)
                            {
                                strRecord += Write_Text("IT3(a)");
                            }
                            else
                            {
                                strRecord += Write_Text("IRP5");
                                blnIRP5Exists = true;
                            }
                        }
                        else
                        {
                            strRecord += Write_Text("ITREG");
                        }

                        strRecord += Write_Header_Code_End();

                        //Nature Of Person
                        intFindRow = pvtNaturePersonDataView.Find(pvtEmployeeDataView[intRow]["NATURE_PERSON_NO"].ToString());
                        strNatureOfPerson = pvtNaturePersonDataView[intFindRow]["NATURE_PERSON_ID"].ToString();

                        strRecord += Write_Header_Code("3020");
                        strRecord += Write_Text(strNatureOfPerson);
                        strRecord += Write_Header_Code_End();

                        //Year Of Assessment
                        strRecord += Write_Header_Code("3025");

                        if (pvtEmployeeDataView[intRow]["EMPLOYEE_TAX_NO"].ToString() != "")
                        {
                            if (dtPeriodDateTime.Month == 8)
                            {
                                strRecord += Write_Numeric(dtPeriodDateTime.AddYears(1).ToString("yyyy"));
                            }
                            else
                            {
                                strRecord += Write_Numeric(dtPeriodDateTime.ToString("yyyy"));
                            }
                        }

                        strRecord += Write_Header_Code_End();
                   
                        //2016-09-14
                        if (pvtEmployeeDataView[intRow]["EMPLOYEE_TAX_NO"].ToString() != "")
                        {
                            //Employment Tax Incentive Indicator - Mandatory if IT3(a)/IRP5
                            strRecord += Write_Header_Code("3026");
                            strRecord += Write_Text("N");
                            strRecord += Write_Header_Code_End();
                        }

                        //Surname
                        strRecord += Write_Header_Code("3030");
                        strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString());
                        strRecord += Write_Header_Code_End();

                        //Name
                        strRecord += Write_Header_Code("3040");
                        strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString());
                        strRecord += Write_Header_Code_End();

                        //Initials
                        strRecord += Write_Header_Code("3050");
                        strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_INITIALS"].ToString());
                        strRecord += Write_Header_Code_End();

                        if (strNatureOfPerson == "B"
                            | strNatureOfPerson == "D"
                            | strNatureOfPerson == "E"
                            | strNatureOfPerson == "F"
                            | strNatureOfPerson == "G"
                            | strNatureOfPerson == "H")
                        {
                        }
                        else
                        {
                            if (pvtEmployeeDataView[intRow]["NATURE_PERSON_NO"].ToString() == "1")
                            {
                                //Identity Number
                                strRecord += Write_Header_Code("3060");
                                strRecord += Write_Numeric(pvtEmployeeDataView[intRow]["EMPLOYEE_ID_NO"].ToString());
                                strRecord += Write_Header_Code_End();
                            }
                            else
                            {
                                //Passport Number
                                strRecord += Write_Header_Code("3070");
                                strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_PASSPORT_NO"].ToString());
                                strRecord += Write_Header_Code_End();

                                //Passport Country Number
                                strRecord += Write_Header_Code("3075");
                                strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_PASSPORT_COUNTRY_CODE"].ToString());
                                strRecord += Write_Header_Code_End();
                            }
                        }

                        //Date Of Birth
                        strRecord += Write_Header_Code("3080");
                        strRecord += Write_Numeric(Convert.ToDateTime(pvtEmployeeDataView[intRow]["EMPLOYEE_BIRTHDATE"]).ToString("yyyyMMdd"));
                        strRecord += Write_Header_Code_End();

                        //Income Tax Number
                        strRecord += Write_Header_Code("3100");

                        if (pvtEmployeeDataView[intRow]["EMPLOYEE_TAX_NO"].ToString() != "")
                        {
                            strRecord += Write_Numeric(pvtEmployeeDataView[intRow]["EMPLOYEE_TAX_NO"].ToString());
                        }

                        strRecord += Write_Header_Code_End();

                        //ELR - 2014-09-27
                        if (pvtEmployeeDataView[intRow]["EMPLOYEE_TAX_NO"].ToString() != "")
                        {
                            strRecord += Write_Header_Code("3263");
                            strRecord += Write_Text(pvtEmployeeDataView[intRow]["SIC7_GROUP_CODE"].ToString());
                            strRecord += Write_Header_Code_End();
                        }

                        if (pvtEmployeeDataView[intRow]["EMPLOYEE_EMAIL"].ToString() != "")
                        {
                            strRecord += Write_Header_Code("3125");
                            strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_EMAIL"].ToString());
                            strRecord += Write_Header_Code_End();
                        }

                        if (pvtEmployeeDataView[intRow]["EMPLOYEE_TEL_HOME"].ToString() != "")
                        {
                            strRecord += Write_Header_Code("3135");
                            strRecord += Write_Numeric(pvtEmployeeDataView[intRow]["EMPLOYEE_TEL_HOME"].ToString());
                            strRecord += Write_Header_Code_End();
                        }

                        strRecord += Write_Header_Code("3136");

                        if (pvtEmployeeDataView[intRow]["USE_WORK_TEL_IND"].ToString() == "Y")
                        {
                            strRecord += Write_Numeric(this.pvtDataSet.Tables["Company"].Rows[0]["TEL_WORK"].ToString());
                        }
                        else
                        {
                            strRecord += Write_Numeric(pvtEmployeeDataView[intRow]["EMPLOYEE_TEL_WORK"].ToString());
                        }

                        strRecord += Write_Header_Code_End();

                        if (pvtEmployeeDataView[intRow]["EMPLOYEE_TEL_CELL"].ToString() != "")
                        {
                            strRecord += Write_Header_Code("3138");
                            strRecord += Write_Numeric(pvtEmployeeDataView[intRow]["EMPLOYEE_TEL_CELL"].ToString());
                            strRecord += Write_Header_Code_End();
                        }

                        pvtEmployeePayCategoryAddressDataView = null;
                        pvtEmployeePayCategoryAddressDataView = new DataView(pvtDataSet.Tables["EmployeePayCategoryAddress"],
                        "PAY_CATEGORY_NO = " + pvtEmployeeDataView[intRow]["PAY_CATEGORY_NO"].ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtEmployeeDataView[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'",
                        "",
                        DataViewRowState.CurrentRows);

                        if (pvtEmployeePayCategoryAddressDataView.Count > 0)
                        {
                            if (pvtEmployeePayCategoryAddressDataView[0]["RES_UNIT_NUMBER"].ToString() != "")
                            {
                                //Unit Number
                                strRecord += Write_Header_Code("3144");
                                strRecord += Write_Text(pvtEmployeePayCategoryAddressDataView[0]["RES_UNIT_NUMBER"].ToString());
                                strRecord += Write_Header_Code_End();
                            }

                            if (pvtEmployeePayCategoryAddressDataView[0]["RES_COMPLEX"].ToString() != "")
                            {
                                //Complex
                                strRecord += Write_Header_Code("3145");
                                strRecord += Write_Text(pvtEmployeePayCategoryAddressDataView[0]["RES_COMPLEX"].ToString());
                                strRecord += Write_Header_Code_End();
                            }

                            if (pvtEmployeePayCategoryAddressDataView[0]["RES_STREET_NUMBER"].ToString() != "")
                            {
                                //Street Number
                                strRecord += Write_Header_Code("3146");
                                strRecord += Write_Text(pvtEmployeePayCategoryAddressDataView[0]["RES_STREET_NUMBER"].ToString());
                                strRecord += Write_Header_Code_End();
                            }

                            //Street Name / Farm Name
                            strRecord += Write_Header_Code("3147");
                            strRecord += Write_Text(pvtEmployeePayCategoryAddressDataView[0]["RES_STREET_NAME"].ToString());
                            strRecord += Write_Header_Code_End();

                            if (pvtEmployeePayCategoryAddressDataView[0]["RES_SUBURB"].ToString() != "")
                            {
                                //Suburb / District
                                strRecord += Write_Header_Code("3148");
                                strRecord += Write_Text(pvtEmployeePayCategoryAddressDataView[0]["RES_SUBURB"].ToString());
                                strRecord += Write_Header_Code_End();
                            }

                            if (pvtEmployeePayCategoryAddressDataView[0]["RES_CITY"].ToString() != "")
                            {
                                //City
                                strRecord += Write_Header_Code("3149");
                                strRecord += Write_Text(pvtEmployeePayCategoryAddressDataView[0]["RES_CITY"].ToString());
                                strRecord += Write_Header_Code_End();
                            }

                            //Physical Address Post Code
                            strRecord += Write_Header_Code("3150");
                            strRecord += Write_Text(pvtEmployeePayCategoryAddressDataView[0]["RES_ADDR_CODE"].ToString());
                            strRecord += Write_Header_Code_End();

                            //2016-09-14
                            //Physical Address Country Code
                            strRecord += Write_Header_Code("3151");
                            strRecord += Write_Text(pvtEmployeePayCategoryAddressDataView[0]["RES_COUNTRY_CODE"].ToString());
                            strRecord += Write_Header_Code_End();
                        }
                        else
                        {
                            //NB This Should be Added to Cost Centre (Override)
                            //Unit Number
                            if (this.pvtDataSet.Tables["Company"].Rows[0]["RES_UNIT_NUMBER"].ToString() != "")
                            {
                                //Unit Number
                                strRecord += Write_Header_Code("3144");
                                strRecord += Write_Text(this.pvtDataSet.Tables["Company"].Rows[0]["RES_UNIT_NUMBER"].ToString());
                                strRecord += Write_Header_Code_End();
                            }

                            if (this.pvtDataSet.Tables["Company"].Rows[0]["RES_COMPLEX"].ToString() != "")
                            {
                                //Complex
                                strRecord += Write_Header_Code("3145");
                                strRecord += Write_Text(this.pvtDataSet.Tables["Company"].Rows[0]["RES_COMPLEX"].ToString());
                                strRecord += Write_Header_Code_End();
                            }

                            if (this.pvtDataSet.Tables["Company"].Rows[0]["RES_STREET_NUMBER"].ToString() != "")
                            {
                                //Street Number
                                strRecord += Write_Header_Code("3146");
                                strRecord += Write_Text(this.pvtDataSet.Tables["Company"].Rows[0]["RES_STREET_NUMBER"].ToString());
                                strRecord += Write_Header_Code_End();
                            }

                            //Street Name / Farm Name
                            strRecord += Write_Header_Code("3147");
                            strRecord += Write_Text(this.pvtDataSet.Tables["Company"].Rows[0]["RES_STREET_NAME"].ToString());
                            strRecord += Write_Header_Code_End();

                            if (this.pvtDataSet.Tables["Company"].Rows[0]["RES_SUBURB"].ToString() != "")
                            {
                                //Suburb / District
                                strRecord += Write_Header_Code("3148");
                                strRecord += Write_Text(this.pvtDataSet.Tables["Company"].Rows[0]["RES_SUBURB"].ToString());
                                strRecord += Write_Header_Code_End();
                            }

                            if (this.pvtDataSet.Tables["Company"].Rows[0]["RES_CITY"].ToString() != "")
                            {
                                //City
                                strRecord += Write_Header_Code("3149");
                                strRecord += Write_Text(this.pvtDataSet.Tables["Company"].Rows[0]["RES_CITY"].ToString());
                                strRecord += Write_Header_Code_End();
                            }

                            //Physical Address Post Code
                            strRecord += Write_Header_Code("3150");
                            strRecord += Write_Text(this.pvtDataSet.Tables["Company"].Rows[0]["RES_ADDR_CODE"].ToString());
                            strRecord += Write_Header_Code_End();

                            //2016-09-14
                            //Physical Address Country Code
                            strRecord += Write_Header_Code("3151");
                            strRecord += Write_Text("ZA");
                            strRecord += Write_Header_Code_End();
                        }

                        strRecord += Write_Header_Code("3160");
                        strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString());
                        strRecord += Write_Header_Code_End();

                        if (pvtEmployeeDataView[intRow]["EMPLOYEE_TAX_NO"].ToString() != "")
                        {
                            DateTime dtEmployeeBeginningOfYear = DateTime.Now;
                            DateTime dtEmployeeEndOfYear = DateTime.Now;

                            //Period Employed From
                            strRecord += Write_Header_Code("3170");

                            if (dtBeginningOfYear >= Convert.ToDateTime(pvtEmployeeDataView[intRow]["EMPLOYEE_TAX_STARTDATE"]))
                            {
                                dtEmployeeBeginningOfYear = dtBeginningOfYear;
                                strRecord += Write_Numeric(dtBeginningOfYear.ToString("yyyyMMdd"));
                            }
                            else
                            {
                                dtEmployeeBeginningOfYear = Convert.ToDateTime(pvtEmployeeDataView[intRow]["EMPLOYEE_TAX_STARTDATE"]);
                                strRecord += Write_Numeric(Convert.ToDateTime(pvtEmployeeDataView[intRow]["EMPLOYEE_TAX_STARTDATE"]).ToString("yyyyMMdd"));
                            }

                            strRecord += Write_Header_Code_End();

                            strRecord += Write_Header_Code("3180");

                            if (pvtEmployeeDataView[intRow]["EMPLOYEE_ENDDATE"] == System.DBNull.Value)
                            {
                                //Still Employed
                                dtEmployeeEndOfYear = dtEndOfYear;
                                strRecord += Write_Numeric(dtEndOfYear.ToString("yyyyMMdd"));
                            }
                            else
                            {
                                if (Convert.ToDateTime(pvtEmployeeDataView[intRow]["EMPLOYEE_ENDDATE"]) > dtPeriodDateTime)
                                {
                                    //Report AS Still Employed (Will Report Closed in Next Run)
                                    dtEmployeeEndOfYear = dtEndOfYear;
                                    strRecord += Write_Numeric(dtEndOfYear.ToString("yyyyMMdd"));
                                }
                                else
                                {
                                    dtEmployeeEndOfYear = Convert.ToDateTime(pvtEmployeeDataView[intRow]["EMPLOYEE_ENDDATE"]);
                                    strRecord += Write_Numeric(Convert.ToDateTime(pvtEmployeeDataView[intRow]["EMPLOYEE_ENDDATE"]).ToString("yyyyMMdd"));
                                }
                            }

                            strRecord += Write_Header_Code_End();

                            //Pay Periods In Tax Year
                            strRecord += Write_Header_Code("3200");

                            if (pvtEmployeeDataView[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                            {
                                strRecord += Write_Numeric("365.0000");
                            }
                            else
                            {
                                strRecord += Write_Numeric("12.0000");
                            }

                            strRecord += Write_Header_Code_End();

                            //Periods Worked In Tax Year
                            strRecord += Write_Header_Code("3210");

                            if (pvtEmployeeDataView[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                            {
                                TimeSpan timeSpan = dtEmployeeEndOfYear.AddDays(1) - dtEmployeeBeginningOfYear;

                                if (timeSpan.Days > 365)
                                {
                                    strRecord += Write_Numeric("365.0000");
                                }
                                else
                                {
                                    strRecord += Write_Numeric(timeSpan.Days.ToString("##0.0000"));
                                }
                            }
                            else
                            {
                                int intMonths = ((dtEmployeeEndOfYear.Year - dtEmployeeBeginningOfYear.Year) * 12) + dtEmployeeEndOfYear.Month - dtEmployeeBeginningOfYear.Month + 1;
                                
                                strRecord += Write_Numeric(intMonths.ToString("#0.0000"));
                            }

                            strRecord += Write_Header_Code_End();
                        }

                        if (pvtEmployeeDataView[intRow]["USE_RES_ADDR_COMPANY_IND"].ToString() == "Y")
                        {
                            if (pvtEmployeePayCategoryAddressDataView.Count > 0)
                            {
                                if (pvtEmployeePayCategoryAddressDataView[0]["RES_UNIT_NUMBER"].ToString() != "")
                                {
                                    //Unit Number
                                    strRecord += Write_Header_Code("3211");
                                    strRecord += Write_Text(pvtEmployeePayCategoryAddressDataView[0]["RES_UNIT_NUMBER"].ToString());
                                    strRecord += Write_Header_Code_End();
                                }

                                if (pvtEmployeePayCategoryAddressDataView[0]["RES_COMPLEX"].ToString() != "")
                                {
                                    //Complex
                                    strRecord += Write_Header_Code("3212");
                                    strRecord += Write_Text(pvtEmployeePayCategoryAddressDataView[0]["RES_COMPLEX"].ToString());
                                    strRecord += Write_Header_Code_End();
                                }

                                if (pvtEmployeePayCategoryAddressDataView[0]["RES_STREET_NUMBER"].ToString() != "")
                                {
                                    //Street Number
                                    strRecord += Write_Header_Code("3213");
                                    strRecord += Write_Text(pvtEmployeePayCategoryAddressDataView[0]["RES_STREET_NUMBER"].ToString());
                                    strRecord += Write_Header_Code_End();
                                }

                                //Street Name / Farm Name
                                strRecord += Write_Header_Code("3214");
                                strRecord += Write_Text(pvtEmployeePayCategoryAddressDataView[0]["RES_STREET_NAME"].ToString());
                                strRecord += Write_Header_Code_End();

                                if (pvtEmployeePayCategoryAddressDataView[0]["RES_SUBURB"].ToString() != "")
                                {
                                    //Suburb / District
                                    strRecord += Write_Header_Code("3215");
                                    strRecord += Write_Text(pvtEmployeePayCategoryAddressDataView[0]["RES_SUBURB"].ToString());
                                    strRecord += Write_Header_Code_End();
                                }

                                if (pvtEmployeePayCategoryAddressDataView[0]["RES_CITY"].ToString() != "")
                                {
                                    //City
                                    strRecord += Write_Header_Code("3216");
                                    strRecord += Write_Text(pvtEmployeePayCategoryAddressDataView[0]["RES_CITY"].ToString());
                                    strRecord += Write_Header_Code_End();
                                }

                                //Physical Address Post Code
                                strRecord += Write_Header_Code("3217");
                                strRecord += Write_Text(pvtEmployeePayCategoryAddressDataView[0]["RES_ADDR_CODE"].ToString());
                                strRecord += Write_Header_Code_End();
                            }
                            else
                            {
                                if (this.pvtDataSet.Tables["Company"].Rows[0]["RES_UNIT_NUMBER"].ToString() != "")
                                {
                                    //Unit Number
                                    strRecord += Write_Header_Code("3211");
                                    strRecord += Write_Text(this.pvtDataSet.Tables["Company"].Rows[0]["RES_UNIT_NUMBER"].ToString());
                                    strRecord += Write_Header_Code_End();
                                }

                                if (this.pvtDataSet.Tables["Company"].Rows[0]["RES_COMPLEX"].ToString() != "")
                                {
                                    //Complex
                                    strRecord += Write_Header_Code("3212");
                                    strRecord += Write_Text(this.pvtDataSet.Tables["Company"].Rows[0]["RES_COMPLEX"].ToString());
                                    strRecord += Write_Header_Code_End();
                                }

                                if (this.pvtDataSet.Tables["Company"].Rows[0]["RES_STREET_NUMBER"].ToString() != "")
                                {
                                    //Street Number
                                    strRecord += Write_Header_Code("3213");
                                    strRecord += Write_Text(this.pvtDataSet.Tables["Company"].Rows[0]["RES_STREET_NUMBER"].ToString());
                                    strRecord += Write_Header_Code_End();
                                }

                                //Street Name / Farm Name
                                strRecord += Write_Header_Code("3214");
                                strRecord += Write_Text(this.pvtDataSet.Tables["Company"].Rows[0]["RES_STREET_NAME"].ToString());
                                strRecord += Write_Header_Code_End();

                                if (this.pvtDataSet.Tables["Company"].Rows[0]["RES_SUBURB"].ToString() != "")
                                {
                                    //Suburb / District
                                    strRecord += Write_Header_Code("3215");
                                    strRecord += Write_Text(this.pvtDataSet.Tables["Company"].Rows[0]["RES_SUBURB"].ToString());
                                    strRecord += Write_Header_Code_End();
                                }

                                if (this.pvtDataSet.Tables["Company"].Rows[0]["RES_CITY"].ToString() != "")
                                {
                                    //City
                                    strRecord += Write_Header_Code("3216");
                                    strRecord += Write_Text(this.pvtDataSet.Tables["Company"].Rows[0]["RES_CITY"].ToString());
                                    strRecord += Write_Header_Code_End();
                                }

                                //Physical Address Post Code
                                strRecord += Write_Header_Code("3217");
                                strRecord += Write_Text(this.pvtDataSet.Tables["Company"].Rows[0]["RES_ADDR_CODE"].ToString());
                                strRecord += Write_Header_Code_End();
                            }
                        }
                        else
                        {
                            if (pvtEmployeeDataView[intRow]["EMPLOYEE_RES_UNIT_NUMBER"].ToString() != "")
                            {
                                //Unit Number
                                strRecord += Write_Header_Code("3211");
                                strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_RES_UNIT_NUMBER"].ToString());
                                strRecord += Write_Header_Code_End();
                            }

                            if (pvtEmployeeDataView[intRow]["EMPLOYEE_RES_COMPLEX"].ToString() != "")
                            {
                                //Complex
                                strRecord += Write_Header_Code("3212");
                                strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_RES_COMPLEX"].ToString());
                                strRecord += Write_Header_Code_End();
                            }

                            if (pvtEmployeeDataView[intRow]["EMPLOYEE_RES_STREET_NUMBER"].ToString() != "")
                            {
                                //Street Number
                                strRecord += Write_Header_Code("3213");
                                strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_RES_STREET_NUMBER"].ToString());
                                strRecord += Write_Header_Code_End();
                            }

                            //Street Name / Farm Name
                            strRecord += Write_Header_Code("3214");
                            strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_RES_STREET_NAME"].ToString());
                            strRecord += Write_Header_Code_End();

                            if (pvtEmployeeDataView[intRow]["EMPLOYEE_RES_SUBURB"].ToString() != "")
                            {
                                //Suburb / District
                                strRecord += Write_Header_Code("3215");
                                strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_RES_SUBURB"].ToString());
                                strRecord += Write_Header_Code_End();
                            }

                            if (pvtEmployeeDataView[intRow]["EMPLOYEE_RES_CITY"].ToString() != "")
                            {
                                //City
                                strRecord += Write_Header_Code("3216");
                                strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_RES_CITY"].ToString());
                                strRecord += Write_Header_Code_End();
                            }

                            //Suburb / District
                            strRecord += Write_Header_Code("3217");
                            strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_RES_CODE"].ToString());
                            strRecord += Write_Header_Code_End();
                        }

                        //ELR - 20141013
                        strRecord += Write_Header_Code("3285");
                        strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_RES_COUNTRY_CODE2"].ToString());
                        strRecord += Write_Header_Code_End();
                      
                        //POSTAL ADDRESS
                        //POSTAL ADDRESS
                        if (pvtEmployeeDataView[intRow]["EMPLOYEE_POST_OPTION_IND"].ToString() == "R")
                        {
                            //I THINK I WILL HAVE TO MAKE POSTAL SAME AS RESIDENTIAL
                            //Same as Residential Address
                            //strRecord += Write_Header_Code("3218");
                            //strRecord += Write_Text("X");
                            //strRecord += Write_Header_Code_End();

                            //2016-09-14
                            strRecord += Write_Header_Code("3288");
                            strRecord += Write_Numeric("1");
                            strRecord += Write_Header_Code_End();
                        }
                        else
                        {
                            if (pvtEmployeeDataView[intRow]["EMPLOYEE_POST_OPTION_IND"].ToString() == "S")
                            {
                                //Postal Code is a Street Address
                                //Postal Code is a Street Address

                                //Postal Address is a Street Address?
                                //strRecord += Write_Header_Code("3247");
                                //strRecord += Write_Text("Y");
                                //strRecord += Write_Header_Code_End();
                            
                                if (pvtEmployeeDataView[intRow]["EMPLOYEE_POST_UNIT_NUMBER"].ToString() != "")
                                {
                                    //Unit Number of Postal Address
                                    strRecord += Write_Header_Code("3255");
                                    strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_POST_UNIT_NUMBER"].ToString());
                                    strRecord += Write_Header_Code_End();
                                }

                                if (pvtEmployeeDataView[intRow]["EMPLOYEE_POST_COMPLEX"].ToString() != "")
                                {
                                    //Complex Name of Postal Address
                                    strRecord += Write_Header_Code("3256");
                                    strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_POST_COMPLEX"].ToString());
                                    strRecord += Write_Header_Code_End();
                                }

                                if (pvtEmployeeDataView[intRow]["EMPLOYEE_POST_STREET_NUMBER"].ToString() != "")
                                {
                                    //Street Number of Postal Address
                                    strRecord += Write_Header_Code("3257");
                                    strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_POST_STREET_NUMBER"].ToString());
                                    strRecord += Write_Header_Code_End();
                                }

                                //Street Name / Farm Name of Postal Code
                                strRecord += Write_Header_Code("3258");
                                strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_POST_STREET_NAME"].ToString());
                                strRecord += Write_Header_Code_End();

                                if (pvtEmployeeDataView[intRow]["EMPLOYEE_POST_SUBURB"].ToString() != "")
                                {
                                    //Suburb / District of Postal Address
                                    strRecord += Write_Header_Code("3259");
                                    strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_POST_SUBURB"].ToString());
                                    strRecord += Write_Header_Code_End();
                                }

                                if (pvtEmployeeDataView[intRow]["EMPLOYEE_POST_CITY"].ToString() != "")
                                {
                                    //City / Town of Postal Address
                                    strRecord += Write_Header_Code("3260");
                                    strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_POST_CITY"].ToString());
                                    strRecord += Write_Header_Code_End();
                                }

                                //Postal Code
                                strRecord += Write_Header_Code("3261");
                                strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_POST_CODE"].ToString());
                                strRecord += Write_Header_Code_End();

                                //Country Code
                                strRecord += Write_Header_Code("3287");
                                strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_POST_COUNTRY_CODE2"].ToString());
                                strRecord += Write_Header_Code_End();

                                //2016-09-14
                                //3=Structured Postal Address - Not Same as Residential Address
                                strRecord += Write_Header_Code("3288");
                                strRecord += Write_Numeric("3");
                                strRecord += Write_Header_Code_End();
                            }
                            else
                            {
                                //Maybe This Shouldn't be Here

                                //Postal Address is a Street Address?
                                //strRecord += Write_Header_Code("3247");
                                //strRecord += Write_Text("N");
                                //strRecord += Write_Header_Code_End();

                                if (pvtEmployeeDataView[intRow]["EMPLOYEE_POST_OPTION_IND"].ToString() == "P"
                                || pvtEmployeeDataView[intRow]["EMPLOYEE_POST_OPTION_IND"].ToString() == "B")
                                {
                                    if (pvtEmployeeDataView[intRow]["EMPLOYEE_POST_OPTION_IND"].ToString() == "P")
                                    {
                                        //PO Box
                                        strRecord += Write_Header_Code("3249");
                                        //strRecord += Write_Text("X");
                                        //PO Box (2015)
                                        strRecord += Write_Text("PO_BOX");
                                        strRecord += Write_Header_Code_End();
                                    }
                                    else
                                    {
                                        //Private Bag
                                        //strRecord += Write_Header_Code("3250");
                                        //strRecord += Write_Text("X");
                                        //strRecord += Write_Header_Code_End();
                                        //Private Bag (2015)
                                        strRecord += Write_Header_Code("3249");
                                        strRecord += Write_Text("PRIVATE_BAG");
                                        strRecord += Write_Header_Code_End();
                                    }

                                    //Indicates Employees Other Speial Postal Service. eg.Military Filed Service Address
                                    //2016-09-15 Removed - Cannot be Used with Code 3249 Above
                                    //strRecord += Write_Header_Code("3280");
                                    //strRecord += Write_Text(" ");
                                    //strRecord += Write_Header_Code_End();

                                    //PO Box Number / Private Bag Number
                                    strRecord += Write_Header_Code("3262");
                                    strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_POST_STREET_NUMBER"].ToString());
                                    strRecord += Write_Header_Code_End();

                                    if (pvtEmployeeDataView[intRow]["EMPLOYEE_POST_STREET_NAME"].ToString() != "")
                                    {
                                        //Postal Agency / Post Suite
                                        strRecord += Write_Header_Code("3251");
                                        strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_POST_STREET_NAME"].ToString());
                                        strRecord += Write_Header_Code_End();
                                    }

                                    //Post Office Branch / Employee's Postal Address
                                    strRecord += Write_Header_Code("3253");
                                    strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_POST_SUBURB"].ToString());
                                    strRecord += Write_Header_Code_End();

                                    //Postal Code
                                    strRecord += Write_Header_Code("3254");
                                    strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_POST_CODE"].ToString());
                                    strRecord += Write_Header_Code_End();

                                    //Country Code of Postal Address
                                    strRecord += Write_Header_Code("3286");
                                    strRecord += Write_Text(pvtEmployeeDataView[intRow]["EMPLOYEE_POST_COUNTRY_CODE2"].ToString());
                                    strRecord += Write_Header_Code_End();
                                    
                                    //2016-09-14
                                    //2=Structured Postal Address
                                    strRecord += Write_Header_Code("3288");
                                    strRecord += Write_Numeric("2");
                                    strRecord += Write_Header_Code_End();
                                }
                            }
                        }

                        //2016-09-14
                        //Postal Code Care Of C/O Address
                        strRecord += Write_Header_Code("3279");
                        strRecord += Write_Text("N");
                        strRecord += Write_Header_Code_End();
                        
                        //POSTAL ADDRESS END
                        //POSTAL ADDRESS END

                        //ERROL ADDED UP TO HERE
                        //ERROL ADDED UP TO HERE

                        //Only if Not ITREG
                        if (pvtEmployeeDataView[intRow]["EMPLOYEE_TAX_NO"].ToString() != "")
                        {
                            //Directive Number
                            if (pvtEmployeeDataView[intRow]["TAX_DIRECTIVE_NO1"].ToString() != "")
                            {
                                strRecord += Write_Header_Code("3230");
                                strRecord += Write_Text(pvtEmployeeDataView[intRow]["TAX_DIRECTIVE_NO1"].ToString());
                                strRecord += Write_Header_Code_End();
                            }

                            if (pvtEmployeeDataView[intRow]["TAX_DIRECTIVE_NO2"].ToString() != "")
                            {
                                strRecord += Write_Header_Code("3230");
                                strRecord += Write_Text(pvtEmployeeDataView[intRow]["TAX_DIRECTIVE_NO2"].ToString());
                                strRecord += Write_Header_Code_End();
                            }

                            if (pvtEmployeeDataView[intRow]["TAX_DIRECTIVE_NO3"].ToString() != "")
                            {
                                strRecord += Write_Header_Code("3230");
                                strRecord += Write_Text(pvtEmployeeDataView[intRow]["TAX_DIRECTIVE_NO3"].ToString());
                                strRecord += Write_Header_Code_End();
                            }
                        }

                        //Employee Bank Account Type - 0,1,2,3,4,5,6,7   (needs to be added)
                        strRecord += Write_Header_Code("3240");
                        strRecord += Write_Numeric(pvtEmployeeDataView[intRow]["BANK_ACCOUNT_TYPE_NO"].ToString());
                        strRecord += Write_Header_Code_End();

                        if (pvtEmployeeDataView[intRow]["BANK_ACCOUNT_TYPE_NO"].ToString() != "0"
                            & pvtEmployeeDataView[intRow]["BANK_ACCOUNT_TYPE_NO"].ToString() != "7")
                        {
                            //Employee Bank Account Number
                            strRecord += Write_Header_Code("3241");
                            strRecord += Write_Numeric(pvtEmployeeDataView[intRow]["ACCOUNT_NO"].ToString());
                            strRecord += Write_Header_Code_End();

                            //Employee Bank Account Number
                            strRecord += Write_Header_Code("3242");
                            strRecord += Write_Numeric(pvtEmployeeDataView[intRow]["BRANCH_CODE"].ToString());
                            strRecord += Write_Header_Code_End();

                            if (pvtEmployeeDataView[intRow]["BANK_NO"].ToString() != "")
                            {
                                DataView BankDataView = new DataView(pvtDataSet.Tables["Bank"],
                                "BANK_NO = " + pvtEmployeeDataView[intRow]["BANK_NO"].ToString(),
                                 "",
                                DataViewRowState.CurrentRows);
                              
                                if (BankDataView.Count > 0)
                                {
                                    //Employee Bank Name
                                    strRecord += Write_Header_Code("3243");
                                    strRecord += Write_Text(BankDataView[0]["BANK_DESC"].ToString());
                                    strRecord += Write_Header_Code_End();
                                }
                            }

                            //Employee Branch Desc
                            //ELR - 2015-02-18
                            strRecord += Write_Header_Code("3244");
                            strRecord += Write_Text(pvtEmployeeDataView[intRow]["BRANCH_DESC"].ToString());
                            strRecord += Write_Header_Code_End();

                            //Employee Account Name
                            strRecord += Write_Header_Code("3245");
                            strRecord += Write_Text(pvtEmployeeDataView[intRow]["ACCOUNT_NAME"].ToString());
                            strRecord += Write_Header_Code_End();

                            //Employee Account Name
                            strRecord += Write_Header_Code("3246");
                            strRecord += Write_Text(pvtEmployeeDataView[intRow]["BANK_ACCOUNT_RELATIONSHIP_TYPE_NO"].ToString());
                            strRecord += Write_Header_Code_End();
                        }

                        //Only if Not ITREG
                        if (pvtEmployeeDataView[intRow]["EMPLOYEE_TAX_NO"].ToString() != "")
                        {
                            pvtEmployeeEarningDataView = null;
                            pvtEmployeeEarningDataView = new DataView(pvtDataSet.Tables["EmployeeEarning"],
                            "EMPLOYEE_NO = " + pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString(),
                            "",
                            DataViewRowState.CurrentRows);

                            for (int intEarningRow = 0; intEarningRow < pvtEmployeeEarningDataView.Count; intEarningRow++)
                            {
                                //Employee Account Name
                                strRecord += Write_Header_Code(pvtEmployeeEarningDataView[intEarningRow]["IRP5_CODE"].ToString());
                                strRecord += Write_Numeric(Convert.ToInt32(pvtEmployeeEarningDataView[intEarningRow]["SUMMED_TOTAL"]).ToString("##############0"));
                                strRecord += Write_Header_Code_End();

                                dblTotalEmployeeAmounts += Convert.ToInt64(pvtEmployeeEarningDataView[intEarningRow]["SUMMED_TOTAL"]);

                                intGrossIncome += Convert.ToInt32(pvtEmployeeEarningDataView[intEarningRow]["SUMMED_TOTAL"]);

                                switch (pvtEmployeeEarningDataView[intEarningRow]["IRP5_CODE"].ToString())
                                {
                                    case "3602":
                                    case "3703":
                                    case "3714":
                                    case "3815":
                                    case "3821":
                                    case "3822":
                                    case "3908":
                                    case "3922":

                                        //3696
                                        intGrossNonTaxableIncome += Convert.ToInt32(pvtEmployeeEarningDataView[intEarningRow]["SUMMED_TOTAL"]);

                                        break;
                                        
                                    case "3605":

                                        //3695
                                        intGrossTaxableAnnualPayments += Convert.ToInt32(pvtEmployeeEarningDataView[intEarningRow]["SUMMED_TOTAL"]);

                                        break;

                                    default:

                                        break;
                                }
                            }

                            if (intGrossTaxableAnnualPayments != 0)
                            {
                                //Employee Account Name
                                strRecord += Write_Header_Code("3695");
                                strRecord += Write_Numeric(intGrossTaxableAnnualPayments.ToString("##############0"));
                                strRecord += Write_Header_Code_End();

                                //Not Sure
                                dblTotalEmployeeAmounts += intGrossTaxableAnnualPayments;
                            }

                            if (intGrossNonTaxableIncome != 0)
                            {
                                strRecord += Write_Header_Code("3696");
                                strRecord += Write_Numeric(intGrossNonTaxableIncome.ToString("##############0"));
                                strRecord += Write_Header_Code_End();

                                //Not Sure
                                dblTotalEmployeeAmounts += intGrossNonTaxableIncome;
                            }

                            //2016-09-15 - Removed 3697,3698
                            //2016-09-15 - Removed 3697,3698

                            //pvtEmployeeDeductionContibutionDataView = new DataView(pvtDataSet.Tables["EmployeeDeductionContribution"],
                            //"EMPLOYEE_NO = " + pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString() + " AND IRP5_CODE IN ('4001','4002','4003')",
                            //"",
                            //DataViewRowState.CurrentRows);

                            //if (pvtEmployeeDeductionContibutionDataView.Count == 0)
                            //{
                            //    strRecord += Write_Header_Code("3698");
                            //    strRecord += Write_Numeric(intGrossIncome.ToString("##############0"));
                            //    strRecord += Write_Header_Code_End();
                            //}
                            //else
                            //{
                            //    strRecord += Write_Header_Code("3697");
                            //    strRecord += Write_Numeric(intGrossIncome.ToString("##############0"));
                            //    strRecord += Write_Header_Code_End();
                            //}

                            //2016-09-15 - Removed 3697,3698 END
                            //2016-09-15 - Removed 3697,3698 END

                            //2016-09-15 - Added 3699 - Gross Employment Income (Taxable)
                            intTotalEmployeeGrossEmploymentIncome = intGrossIncome - intGrossNonTaxableIncome;
                            strRecord += Write_Header_Code("3699");
                            strRecord += Write_Numeric(intTotalEmployeeGrossEmploymentIncome.ToString("##############0"));
                            strRecord += Write_Header_Code_End();
                         
                            //Not Sure
                            dblTotalEmployeeAmounts += intGrossIncome;
                          
                            pvtEmployeeDeductionContibutionDataView = new DataView(pvtDataSet.Tables["EmployeeDeductionContribution"],
                            "EMPLOYEE_NO = " + pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString(),
                            "",
                            DataViewRowState.CurrentRows);

                            if (pvtEmployeeDeductionContibutionDataView.Count > 0)
                            {
                                for (int intDeductionRow = 0; intDeductionRow < pvtEmployeeDeductionContibutionDataView.Count; intDeductionRow++)
                                {
                                    //Employee Account Name
                                    strRecord += Write_Header_Code(pvtEmployeeDeductionContibutionDataView[intDeductionRow]["IRP5_CODE"].ToString());
                                    strRecord += Write_Numeric(Convert.ToInt32(pvtEmployeeDeductionContibutionDataView[intDeductionRow]["SUMMED_TOTAL"]).ToString("##############0"));
                                    strRecord += Write_Header_Code_End();

                                    //2016-09-14
                                    if (pvtEmployeeDeductionContibutionDataView[intDeductionRow]["IRP5_CODE"].ToString() == "4005")
                                    {
                                        blnMedicalSchemeFees = true; 
                                    }

                                    dblTotalEmployeeAmounts += Convert.ToInt64(pvtEmployeeDeductionContibutionDataView[intDeductionRow]["SUMMED_TOTAL"]);

                                    int64TotalEmployeeDeductionContributions += Convert.ToInt64(pvtEmployeeDeductionContibutionDataView[intDeductionRow]["SUMMED_TOTAL"]);
                                }

                                dblTotalEmployeeAmounts += int64TotalEmployeeDeductionContributions;
                            }

                            pvtEmployeeDeductionTaxUifDataView = new DataView(pvtDataSet.Tables["EmployeeDeductionTaxUif"],
                            "EMPLOYEE_NO = " + pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString(),
                            "",
                            DataViewRowState.CurrentRows);

                            if (pvtEmployeeDeductionTaxUifDataView.Count > 0)
                            {
                                for (int intDeductionRow = 0; intDeductionRow < pvtEmployeeDeductionTaxUifDataView.Count; intDeductionRow++)
                                {
                                    if (pvtEmployeeDeductionTaxUifDataView[intDeductionRow]["IRP5_CODE"].ToString() == "4102")
                                    {
                                        dblEmployeeTax = Math.Round(Convert.ToDouble(pvtEmployeeDeductionTaxUifDataView[intDeductionRow]["SUMMED_TOTAL"]),2);

                                        strRecord += Write_Header_Code("4102");
                                        strRecord += Write_Numeric(dblEmployeeTax.ToString("##############0.00"));
                                        strRecord += Write_Header_Code_End();

                                        dblTotalEmployeeAmounts += dblEmployeeTax;

                                        dblEmployeeDeductionTaxUif += dblEmployeeTax;
                                    }
                                    else
                                    {

                                        if (this.pvtDataSet.Tables["Company"].Rows[0]["SDL_REF_NO"].ToString() != "")
                                        {
                                            if (Convert.ToInt32(pvtEmployeeDeductionTaxUifDataView[intDeductionRow]["IRP5_CODE"]) > 4142)
                                            {
                                                strRecord += Write_Header_Code("4142");

                                                dblSDL = Math.Round(Convert.ToDouble(Convert.ToDouble(intGrossIncome) * Convert.ToDouble(this.pvtDataSet.Tables["SDL"].Rows[0]["SDL_LEVY"])),2);

                                                strRecord += Write_Numeric(dblSDL.ToString("##############0.00"));
                                                strRecord += Write_Header_Code_End();

                                                dblTotalEmployeeAmounts += dblSDL;

                                                dblEmployeeDeductionTaxUif += dblSDL;

                                                blnHaveWrittenSDL = true;
                                            }
                                        }

                                        strRecord += Write_Header_Code(pvtEmployeeDeductionTaxUifDataView[intDeductionRow]["IRP5_CODE"].ToString());
                                        strRecord += Write_Numeric(Convert.ToInt32(pvtEmployeeDeductionTaxUifDataView[intDeductionRow]["SUMMED_TOTAL"]).ToString("##############0.00"));
                                        strRecord += Write_Header_Code_End();

                                        dblTotalEmployeeAmounts += Convert.ToDouble(pvtEmployeeDeductionTaxUifDataView[intDeductionRow]["SUMMED_TOTAL"]);

                                        dblEmployeeDeductionTaxUif += Convert.ToDouble(pvtEmployeeDeductionTaxUifDataView[intDeductionRow]["SUMMED_TOTAL"]);
                                    }
                                }
                            }
                        }
                        
                        if (this.pvtDataSet.Tables["Company"].Rows[0]["SDL_REF_NO"].ToString() != "")
                        {
                            if (blnHaveWrittenSDL == false)
                            {
                                strRecord += Write_Header_Code("4142");

                                dblSDL = Math.Round(Convert.ToDouble(Convert.ToDouble(intGrossIncome) * Convert.ToDouble(this.pvtDataSet.Tables["SDL"].Rows[0]["SDL_LEVY"])), 2);

                                strRecord += Write_Numeric(dblSDL.ToString("##############0.00"));
                                strRecord += Write_Header_Code_End();

                                dblTotalEmployeeAmounts += dblSDL;

                                dblEmployeeDeductionTaxUif += dblSDL;
                            }
                        }

                        //2013-09-16
                        if (dblEmployeeDeductionTaxUif != 0)
                        {
                            strRecord += Write_Header_Code("4149");
                            strRecord += Write_Numeric(dblEmployeeDeductionTaxUif.ToString("##############0.00"));
                            strRecord += Write_Header_Code_End();

                            //2013-10-01
                            dblTotalEmployeeAmounts += dblEmployeeDeductionTaxUif;
                        }

                        if (this.pvtDataSet.Tables["EmployeeDeductionTaxCredit"] != null)
                        {
                            pvtEmployeeDeductionTaxCreditDataView = new DataView(pvtDataSet.Tables["EmployeeDeductionTaxCredit"],
                            "EMPLOYEE_NO = " + pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString(),
                             "",
                            DataViewRowState.CurrentRows);

                            if (pvtEmployeeDeductionTaxCreditDataView.Count > 0)
                            {
                                //2016-09-15

                                //NB I HARD CODED Code 4120 in busEasyFile for Willimina Nevillin to Zero (Logic for Over 65 must be added)
                                //NB I HARD CODED Code 4120 in busEasyFile for Willimina Nevillin to Zero (Logic for Over 65 must be added)

                                for (int intDeductionTaxCreditRow = 0; intDeductionTaxCreditRow < pvtEmployeeDeductionTaxCreditDataView.Count; intDeductionTaxCreditRow++)
                                {
                                    strRecord += Write_Header_Code(pvtEmployeeDeductionTaxCreditDataView[intDeductionTaxCreditRow]["IRP5_CODE"].ToString());
                                    strRecord += Write_Numeric(Convert.ToInt32(pvtEmployeeDeductionTaxCreditDataView[intDeductionTaxCreditRow]["SUMMED_TOTAL"]).ToString("##############0.00"));
                                    strRecord += Write_Header_Code_End();

                                    dblTotalEmployeeAmounts += Convert.ToDouble(pvtEmployeeDeductionTaxCreditDataView[intDeductionTaxCreditRow]["SUMMED_TOTAL"]);
                                }
                            }
                        }

                        if (dblEmployeeTax == 0)
                        {
                            strRecord += Write_Header_Code("4150");
                            strRecord += Write_Numeric("02");
                            strRecord += Write_Header_Code_End();
                        }

                        if (int64TotalEmployeeDeductionContributions != 0)
                        {
                            //Total Deduction Totals for Employee
                            strRecord += Write_Header_Code("4497");
                            strRecord += Write_Numeric(int64TotalEmployeeDeductionContributions.ToString("##############0"));
                            strRecord += Write_Header_Code_End();
                        }

                        strRecord += Write_Record_End();
                        swIRP5StreamWriter.WriteLine(strRecord);
                    }

                    //3010 (Employee Records) + 1 (2010 - Employer Header Record)
                    int intTotalRecords = pvtEmployeeDataView.Count + 1;

                    Int64 int64TotalEmployeeCodes = pvtint64TotalEmployeeCodes;

                    //Total of All Employee Records
                    strRecord = Write_Header_Code("6010");
                    strRecord += Write_Numeric(intTotalRecords.ToString("########0"));
                    strRecord += Write_Header_Code_End();

                    //Total Employer Code Values
                    strRecord += Write_Header_Code("6020");
                    strRecord += Write_Numeric(int64TotalEmployeeCodes.ToString("#############0"));
                    strRecord += Write_Header_Code_End();

                    //Total Employer Amount Values
                    strRecord += Write_Header_Code("6030");
                    strRecord += Write_Numeric(dblTotalEmployeeAmounts.ToString("#############0.00"));
                    strRecord += Write_Header_Code_End();

                    strRecord += Write_Record_End();
                    swIRP5StreamWriter.WriteLine(strRecord);

                    swIRP5StreamWriter.Close();

                    if (blnIRP5Exists == true)
                    {
                        if (this.pvtDataSet.Tables["Company"].Rows[0]["PAYE_NO"].ToString().Substring(0, 1) == "7")
                        {
                        }
                        else
                        {
                            File.Delete(this.txtDirectory.Text + "\\" + this.txtFileName.Text);

                            CustomMessageBox.Show("Employees Exist That Pay Tax.\n\nYou need to have A Company PAYE Number.\n\nAction Cancelled.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

                            return;
                        }
                    }

                    int intPeriodRow = Convert.ToInt32(dgvPeriodDataGridView[4,Get_DataGridView_SelectedRowIndex(this.dgvPeriodDataGridView)].Value);
                    this.pvtDataSet.Tables["Period"].Rows[intPeriodRow]["EFILING_CLOSED_IND"] = "Y";

                    this.dgvPeriodDataGridView[1, Get_DataGridView_SelectedRowIndex(this.dgvPeriodDataGridView)].Value = "Yes";

                    object[] objParm = new object[2];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = Convert.ToInt32(this.pvtDataSet.Tables["Period"].Rows[intPeriodRow]["EFILING_NO"]);
                
                    int intReturnCode = (int)clsISUtilities.DynamicFunction("Update_Closed_Ind", objParm);

                    myDialogResult = CustomMessageBox.Show("File Created Successfully.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private string Write_Header_Code(string parstrCode)
        {
            string strReturn = parstrCode + ",";

            pvtint64TotalEmployeeCodes += Convert.ToInt64(parstrCode);

            return strReturn;
        }

        private string Write_Text(string parstrText)
        {
            string strReturn = "\"" + parstrText.Trim() + "\"";
            
            return strReturn;
        }

        private string Write_Numeric(string parstrText)
        {
            string strReturn = parstrText.Trim();

            return strReturn;
        }

        private string Write_Header_Code_End()
        {
            string strReturn = ",";

            return strReturn;
        }

        private string Write_Record_End()
        {
            pvtint64TotalEmployeeCodes += 9999;

            string strReturn = "9999";

            return strReturn;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int intReturnCode = clsISUtilities.DataBind_Save_Check();

                if (intReturnCode != 0)
                {
                    return;
                }

                //ELR 2014-08-24
                if (this.dgvSic7DataGridView.Rows.Count > 0)
                {
                    pvtEmployeeDataView[clsISUtilities.DataViewIndex]["SIC7_GROUP_CODE"] = this.dgvSic7DataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvSic7DataGridView)].Value.ToString();
                }
                else
                {
                    pvtEmployeeDataView[clsISUtilities.DataViewIndex]["SIC7_GROUP_CODE"] = "";
                }
           
                pvtTempDataSet = null;
                pvtTempDataSet = new DataSet();

                //Add Employee Table 
                pvtTempDataSet.Tables.Add(this.pvtDataSet.Tables["Employee"].Clone());
                pvtTempDataSet.Tables["Employee"].ImportRow(pvtEmployeeDataView[clsISUtilities.DataViewIndex].Row);

                //Compress DataSet
                pvtbytCompress = clsISUtilities.Compress_DataSet(pvtTempDataSet);

                object[] objParm = new object[3];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[2] = pvtbytCompress;
               
                clsISUtilities.DynamicFunction("Update_Record", objParm, true);

                this.pvtDataSet.AcceptChanges();

                this.btnSave.Enabled = false;

                dgvEmployeeDataGridView.Enabled = true;
                
                btnCancel_Click(sender, e);

                Load_Employee();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void tmrTimer_Tick(object sender, EventArgs e)
        {
            if (pvtintTimerCount == 5)
            {
                this.tmrTimer.Enabled = false;
            }
            else
            {
                pvtintTimerCount += 1;

                if (this.grbCompanyCheck.Visible == true)
                {
                    this.grbCompanyCheck.Visible = false;
                }
                else
                {
                    this.grbCompanyCheck.Visible = true;
                }
            }
        }

        private void cboSic7Group_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboSic7Group.SelectedIndex > -1)
            {
                pvtSic7CodeGroupDataView = null;
                pvtSic7CodeGroupDataView = new DataView(pvtDataSet.Tables["Sic7Code"],
                                                        "SIC7_GROUP_NO = " + this.pvtDataSet.Tables["Sic7CodeGroup"].Rows[this.cboSic7Group.SelectedIndex]["SIC7_GROUP_NO"].ToString(),
                                                        "SIC7_GROUP_NO,SIC7_GROUP_CODE_DESC", DataViewRowState.CurrentRows);

                Clear_DataGridView(this.dgvSic7DataGridView);

                int intSelectedIndex = 0;

                pvtblnSic7DataGridViewLoaded = false;
                pvtintSic7DataGridViewRowIndex = -1;

                for (int intRow = 0; intRow < pvtSic7CodeGroupDataView.Count; intRow++)
                {
                    this.dgvSic7DataGridView.Rows.Add(pvtSic7CodeGroupDataView[intRow]["SIC7_GROUP_CODE"].ToString(),
                                                      pvtSic7CodeGroupDataView[intRow]["SIC7_GROUP_CODE_DESC"].ToString());

                    if (pvtstrSic7GroupCode == pvtSic7CodeGroupDataView[intRow]["SIC7_GROUP_CODE"].ToString())
                    {
                        intSelectedIndex = intRow;
                    }
                }

                pvtblnSic7DataGridViewLoaded = true;

                if (this.dgvSic7DataGridView.Rows.Count > 0)
                {
                    //ELR - 2014-08-24 (Fixes ScrollBar Crash)
                    this.dgvSic7DataGridView.PerformLayout();

                    this.Set_DataGridView_SelectedRowIndex(this.dgvSic7DataGridView, intSelectedIndex);
                }
            }
        }

        private void dgvSic7DataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnSic7DataGridViewLoaded == true)
            {
                if (this.pvtintSic7DataGridViewRowIndex != e.RowIndex)
                {
                    pvtintSic7DataGridViewRowIndex = e.RowIndex;

                    pvtblnSetViaDataGridView = true;
                    this.txtSic7Code.Text = dgvSic7DataGridView[0,e.RowIndex].Value.ToString();
                    pvtblnSetViaDataGridView = false;
                }
            }
        }

        private void btnSetSic7Code_Click(object sender, EventArgs e)
        {
            this.cboSic7Group.SelectedIndex = -1;

            //ELR 2014-08-24
            pvtstrSic7GroupCode = this.pvtDataSet.Tables["Company"].Rows[0]["SIC7_GROUP_CODE"].ToString();

            DataView myDataView = new DataView(pvtDataSet.Tables["Sic7Code"],
                                                          "SIC7_GROUP_CODE = '" + pvtstrSic7GroupCode + "'",
                                                          "",
                                                          DataViewRowState.CurrentRows);

            if (myDataView.Count > 0)
            {
                this.txtSic7Code.Text = pvtstrSic7GroupCode;
            }
            else
            {
                CustomMessageBox.Show("Cannot Find Sic7 Code\nSpeak to Administrator", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //ELR 2014-08-27 (Stops ComboBox Selecting Sic7 DataGridView Row
            pvtstrSic7GroupCode = "";
        }

        private void txtSic7Code_TextChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (pvtblnSetViaDataGridView == false)
                {
                    if (txtSic7Code.TextLength == 5)
                    {
                        pvtstrSic7GroupCode = this.txtSic7Code.Text;

                        this.cboSic7Group.SelectedIndex = -1;
                        Clear_DataGridView(this.dgvSic7DataGridView);

                        DataView myDataView = new DataView(pvtDataSet.Tables["Sic7Code"],
                                                            "SIC7_GROUP_CODE = '" + pvtstrSic7GroupCode + "'",
                                                            "",
                                                            DataViewRowState.CurrentRows);

                        if (myDataView.Count > 0)
                        {
                            for (int intRow = 0; intRow < this.pvtDataSet.Tables["Sic7CodeGroup"].Rows.Count; intRow++)
                            {
                                if (Convert.ToInt32(this.pvtDataSet.Tables["Sic7CodeGroup"].Rows[intRow]["SIC7_GROUP_NO"]) == Convert.ToInt32(myDataView[0]["SIC7_GROUP_NO"]))
                                {
                                    this.cboSic7Group.SelectedIndex = intRow;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            CustomMessageBox.Show("Cannot Find Sic7 Code", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.txtSic7Code.Text = "";
                        }

                        //ELR 2014-08-24 (Stops ComboBox Selecting Sic7 DataGridView Row
                        pvtstrSic7GroupCode = "";
                    }
                    else
                    {
                        this.cboSic7Group.SelectedIndex = -1;
                        Clear_DataGridView(this.dgvSic7DataGridView);
                    }
                }
            }
        }

        private void Set_Postal_Address_Default()
        {
            this.lblPostCountry.Text = "Country";
            this.lblPostCity.Text = "City / Town";
            this.lblPostSuburb.Text = "Suburb / District";
            this.lblPostStrretName.Text = "Street / Farm Name";
            this.lblPostStreetNumber.Text = "Street Number";
            this.lblPostComplex.Text = "Complex";
            this.lblPostUnitNumber.Text = "Unit Number";

            this.lblPostUnitNumber.Visible = true;
            this.txtPostAddrUnitNumber.Visible = true;
            this.lblPostComplex.Visible = true;
            this.txtPostAddrComplex.Visible = true;
            this.lblPostCity.Visible = true;
            this.txtPostCity.Visible = true;
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
                       
                        clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(txtPostAddrStreetNumber, "EMPLOYEE_POST_STREET_NUMBER", false, "", false);
                       
                        clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(this.txtPostStreetName, "EMPLOYEE_POST_STREET_NAME", true, "Enter Enter Street Name / Farm Name.", true);
                        clsISUtilities.DataBind_Special_Field(this.txtPostStreetName, 3);
                        clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(this.txtPostSuburb, "EMPLOYEE_POST_SUBURB", false, "", true);
                        clsISUtilities.DataBind_Special_Field(this.txtPostSuburb, 3);
                        clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(this.txtPostCity, "EMPLOYEE_POST_CITY", false, "", true);
                        clsISUtilities.DataBind_Special_Field(this.txtPostCity, 3);
                        clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(this.txtPostAddrCode, "EMPLOYEE_POST_CODE", false, "", true);
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

                this.lblPostUnitNumber.Visible = false;
                this.txtPostAddrUnitNumber.Visible = false;
                this.txtPostAddrUnitNumber.Text = "";

                this.lblPostComplex.Visible = false;
                this.txtPostAddrComplex.Visible = false;
                this.txtPostAddrComplex.Text = "";

                this.lblPostStrretName.Text = "Postal Agency / Suite";
                this.lblPostSuburb.Text = "Post Office / Address";

                this.lblPostCity.Visible = false;
                this.txtPostCity.Visible = false;
                this.txtPostCity.Text = "";

                if (this.btnSave.Enabled == true)
                {
                    clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(this.txtPostStreetName, "EMPLOYEE_POST_STREET_NAME", false, "", false);
                    clsISUtilities.DataBind_DataView_To_Numeric_TextBox_Min_Length(this.txtPostAddrCode, "EMPLOYEE_POST_CODE", 0, 4, true, "Enter Postal Code.", true, 9999, false);
                    clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(this.txtPostSuburb, "EMPLOYEE_POST_SUBURB", true, "Enter Post Office / Address.", true);
                }
               
                if (myRadioButton.Name == "rbnPOBoxAddr")
                {
                    this.lblPostStreetNumber.Text = "PO Box Number";

                    if (this.btnSave.Enabled == true)
                    {
                        pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_POST_OPTION_IND"] = "P";

                        clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(this.txtPostAddrStreetNumber, "EMPLOYEE_POST_STREET_NUMBER", true, "Enter PO Box Number.", true);
                    }
                }
                else
                {
                    //Private Bag
                    this.lblPostStreetNumber.Text = "Private Bag Number";

                    if (this.btnSave.Enabled == true)
                    {
                        pvtEmployeeDataView[clsISUtilities.DataViewIndex]["EMPLOYEE_POST_OPTION_IND"] = "B";

                        clsISUtilities.DataBind_DataView_NoCommas_To_TextBox(this.txtPostAddrStreetNumber, "EMPLOYEE_POST_STREET_NUMBER", true, "Enter Private Bag Number.", true);
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
    }
}
