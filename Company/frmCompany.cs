using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Security.Cryptography;

namespace InteractPayroll
{
    public partial class frmCompany : Form
    {
        clsISUtilities clsISUtilities;

        private byte[] pvtbytCompress;
        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;
        private DataView pvtCompanyDataView;
        private DataView pvtCompanyPrintHeaderDataView;
        private DataView pvtEfilingPeriodDataView;
        private DataView pvtSic7CodeGroupDataView;
        private DataRowView pvtDataRowView;

        DataGridViewCellStyle PayrollLinkDataGridViewCellStyle;
        DataGridViewCellStyle eFilingErrorDataGridViewCellStyle;
        DataGridViewCellStyle NormalDataGridViewCellStyle;

        private Int64 pvtint64CompanyNo = -1;

        private bool pvtblnCompanyDataGridViewLoaded = false;
        private bool pvtblnChosenHeaderItemDataGridViewLoaded = false;
        private bool pvtblnHeaderItemDataGridViewLoaded = false;

        private bool pvtblnSic7DataGridViewLoaded = false;
        private bool pvtblnSetViaDataGridView = false;

        private int pvtintCompanyDataGridViewRowIndex = -1;
        private int pvtintSic7DataGridViewRowIndex = -1;

        private string pvtstrSic7GroupCode = "";

        static byte[] _salt = Encoding.ASCII.GetBytes("ErrolLeRoux");
        static string sharedSecret = "Interact";
       
        public frmCompany()
        {
            InitializeComponent();

            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 118;

                this.dgvCompanyDataGridView.Height += 114;
                
                this.tabControl.Top += 118;
            }
        }

        private void frmCompany_Load(object sender, System.EventArgs e)
        {
            try
            {
                PayrollLinkDataGridViewCellStyle = new DataGridViewCellStyle();
                PayrollLinkDataGridViewCellStyle.BackColor = Color.Magenta;
                PayrollLinkDataGridViewCellStyle.SelectionBackColor = Color.Magenta;

                eFilingErrorDataGridViewCellStyle = new DataGridViewCellStyle();
                eFilingErrorDataGridViewCellStyle.BackColor = Color.LightCoral;
                eFilingErrorDataGridViewCellStyle.SelectionBackColor = Color.LightCoral;

                NormalDataGridViewCellStyle = new DataGridViewCellStyle();
                NormalDataGridViewCellStyle.BackColor = SystemColors.Control;
                NormalDataGridViewCellStyle.SelectionBackColor = SystemColors.Control;

                if (AppDomain.CurrentDomain.GetData("AccessInd").ToString() == "S")
                {
                    this.grbLock.Visible = true;
                }

                clsISUtilities = new clsISUtilities(this,"busCompany");

                this.lblCompanySpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblTradeClassify.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSic7Desc.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                txtSic7Code.KeyPress += clsISUtilities.Numeric_KeyPress;
  
                pvtDataSet = new DataSet();

                object[] objParm = new object[2];
                objParm[0] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                clsISUtilities.DataBind_ComboBox_Load(this.cboBank, this.pvtDataSet.Tables["Bank"], "BANK_DESC", "BANK_NO");

                for (int intRow = 0; intRow < this.pvtDataSet.Tables["TradeClassify"].Rows.Count; intRow++)
                {
                    this.dgvTradeClassifyDataGridView.Rows.Add(this.pvtDataSet.Tables["TradeClassify"].Rows[intRow]["TRADE_CLASSIFICATION_CODE"].ToString(),
                                                               this.pvtDataSet.Tables["TradeClassify"].Rows[intRow]["TRADE_CLASSIFICATION_DESC"].ToString());
                }

                for (int intRow = 0; intRow < this.pvtDataSet.Tables["Sic7CodeGroup"].Rows.Count; intRow++)
                {
                    this.cboSic7Group.Items.Add(this.pvtDataSet.Tables["Sic7CodeGroup"].Rows[intRow]["SIC7_GROUP_DESC"].ToString());
                }

                Set_Form_For_Read();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void Load_CurrentForm_Records()
        {
            int intCurrentRow = 0;

            this.grbCompanyLock.Visible = false;

            pvtCompanyDataView = null;
            pvtCompanyDataView = new DataView(pvtDataSet.Tables["Company"]
                , ""
                , "COMPANY_DESC"
                , DataViewRowState.CurrentRows);

             clsISUtilities.DataViewIndex = 0;

             if (clsISUtilities.DataBind_Form_And_DataView_To_Class() == false)
             {
                clsISUtilities.DataBind_DataView_And_Index(this, pvtCompanyDataView, "COMPANY_NO");

                clsISUtilities.DataBind_DataView_To_TextBox(this.txtCompany, "COMPANY_DESC", true, "Enter Company Description.", true);
                
                //Needs To Be Looked At For EFiling
                clsISUtilities.DataBind_DataView_To_TextBox(this.txtUnitNumber, "RES_UNIT_NUMBER", false, "", true);
                clsISUtilities.DataBind_DataView_To_TextBox(this.txtComplex, "RES_COMPLEX", false, "", true);
                clsISUtilities.DataBind_DataView_To_TextBox(this.txtStreetNumber, "RES_STREET_NUMBER", false, "", true);

                clsISUtilities.DataBind_DataView_To_TextBox(this.txtCompanyStreetName, "RES_STREET_NAME", false, "",true);
                clsISUtilities.DataBind_Special_Field(this.txtCompanyStreetName,1);

                clsISUtilities.DataBind_DataView_To_TextBox(this.txtSuburb, "RES_SUBURB", false, "",true);
                clsISUtilities.DataBind_Special_Field(this.txtSuburb,1);
                clsISUtilities.DataBind_DataView_To_TextBox(this.txtCity, "RES_CITY", false, "",true);
                clsISUtilities.DataBind_Special_Field(this.txtCity,1);
                 
                clsISUtilities.DataBind_DataView_To_TextBox(this.txtPhysicalCode, "RES_ADDR_CODE",false, "",true);
                clsISUtilities.DataBind_Special_Field(this.txtPhysicalCode,1);

                clsISUtilities.DataBind_DataView_To_TextBox(txtPostAddr1, "POST_ADDR_LINE1", false, "", true);
                clsISUtilities.DataBind_Special_Field(this.txtPostAddr1, 2);
                clsISUtilities.DataBind_DataView_To_TextBox(txtPostAddr2, "POST_ADDR_LINE2", false, "", true);
                clsISUtilities.DataBind_Special_Field(this.txtPostAddr2, 2);
                clsISUtilities.DataBind_DataView_To_TextBox(txtPostAddr3, "POST_ADDR_LINE3", false, "", true);
                clsISUtilities.DataBind_Special_Field(this.txtPostAddr3, 2);
                clsISUtilities.DataBind_DataView_To_TextBox(this.txtPostAddr4, "POST_ADDR_LINE4", false, "", true);

                clsISUtilities.DataBind_DataView_To_TextBox(this.txtPostAddrCode, "POST_ADDR_CODE", false, "", true);
                clsISUtilities.DataBind_Special_Field(this.txtPostAddrCode, 2);
                 
                clsISUtilities.DataBind_DataView_To_Numeric_TextBox_Min_Length(this.txtTelWork, "TEL_WORK", 0 ,10, false, "Enter Work Number.", true, 0,true);
                clsISUtilities.DataBind_DataView_Field_EFiling(this.txtTelWork);
        
                clsISUtilities.DataBind_DataView_To_Numeric_TextBox_Min_Length(this.txtTelFax, "TEL_FAX", 0, 10, false, "Enter Fax Number.", true, 0, true);
                
                clsISUtilities.DataBind_DataView_To_Numeric_TextBox(this.txtRate1,"OVERTIME1_RATE",2,true, "Enter Wage Overtime Rate 1.",true,9.99,false);
                clsISUtilities.DataBind_DataView_To_Numeric_TextBox(this.txtRate2, "OVERTIME2_RATE", 2, true, "Enter Wage Overtime Rate 2.", true, 9.99,false);
                clsISUtilities.DataBind_DataView_To_Numeric_TextBox(this.txtRate3, "OVERTIME3_RATE", 2, true, "Enter Wage Overtime Rate 3.", true, 9.99,false);

                clsISUtilities.DataBind_DataView_To_Numeric_TextBox(this.txtSalaryRate1, "SALARY_OVERTIME1_RATE", 2, true, "Enter Salary Overtime Rate 1.", true, 9.99,false);
                clsISUtilities.DataBind_DataView_To_Numeric_TextBox(this.txtSalaryRate2, "SALARY_OVERTIME2_RATE", 2, true, "Enter Salary Overtime Rate 2.", true, 9.99, false);
                clsISUtilities.DataBind_DataView_To_Numeric_TextBox(this.txtSalaryRate3, "SALARY_OVERTIME3_RATE", 2, true, "Enter Salary Overtime Rate 3.", true, 9.99, false);
                 
                clsISUtilities.DataBind_DataView_To_ComboBox(this.cboBank, "BANK_NO", true, "Select Bank.", true);

                clsISUtilities.DataBind_DataView_To_Numeric_TextBox(this.txtBranchCode, "BRANCH_CODE", 0, false, "", false, 0,true);
                clsISUtilities.DataBind_Control_Required_If_Enabled(this.txtBranchCode, "Enter Branch Code.");

                clsISUtilities.DataBind_DataView_To_Numeric_TextBox(this.txtAccountNo, "ACCOUNT_NO", 0, false, "", false, 0,true);
                clsISUtilities.DataBind_Control_Required_If_Enabled(this.txtAccountNo, "Enter Account Number.");

                clsISUtilities.DataBind_DataView_To_RadioButton(this.rbnEmplNumYes, "GENERATE_EMPLOYEE_NUMBER_IND", "Y");
                clsISUtilities.DataBind_DataView_To_RadioButton(this.rbnEmplNumNo, "GENERATE_EMPLOYEE_NUMBER_IND", "N");
                clsISUtilities.DataBind_RadioButton_Default(this.rbnEmplNumNo);

                clsISUtilities.DataBind_DataView_To_RadioButton(this.rbnBirthDayYes, "SALARY_DOUBLE_CHEQUE_BIRTHDAY_IND", "Y");
                clsISUtilities.DataBind_DataView_To_RadioButton(this.rbnBirthDayNo, "SALARY_DOUBLE_CHEQUE_BIRTHDAY_IND", "N");
                clsISUtilities.DataBind_RadioButton_Default(this.rbnBirthDayNo);

                clsISUtilities.DataBind_DataView_To_RadioButton(this.rbnYYYY, "DATE_FORMAT", "yyyy-MM-dd");
                clsISUtilities.DataBind_DataView_To_RadioButton(this.rbnDD, "DATE_FORMAT", "dd-MM-yyyy");
                clsISUtilities.DataBind_RadioButton_Default(this.rbnDD);

                clsISUtilities.DataBind_DataView_To_TextBox_EFiling_Either_Or(this.txtPAYERefNo,this.txtTaxRefNo, "PAYE_REF_NO", true, "Enter PAYE Reference Number or Income Tax Ref. Number.");
                clsISUtilities.DataBind_DataView_To_TextBox_EFiling_Either_Or(this.txtTaxRefNo, this.txtPAYERefNo,"TAX_REF_NO", true, "Enter Income Tax Ref. Number.");

                clsISUtilities.DataBind_DataView_To_TextBox_EFiling(this.txtUIFRefNo, "UIF_REF_NO", true, "Enter UIF Reference Number.");

                clsISUtilities.DataBind_DataView_To_TextBox_EFiling(this.txtSDLRefNo, "SDL_REF_NO", true, "Enter SDL Reference Number.");
                clsISUtilities.DataBind_DataView_To_Numeric_TextBox(this.txtVATNo, "VAT_NO", 0, false, "", true, 0,true);
              
                clsISUtilities.DataBind_DataView_To_RadioButton(this.rbnYesDiplomaticIndemnity, "DIPLOMATIC_INDEMNITY_IND", "Y");
                clsISUtilities.DataBind_DataView_To_RadioButton(this.rbnNoDiplomaticIndemnity, "DIPLOMATIC_INDEMNITY_IND", "N");
                clsISUtilities.DataBind_RadioButton_Default(this.rbnNoDiplomaticIndemnity);

                clsISUtilities.DataBind_DataView_To_TextBox_EFiling(this.txtEFileNameSurname, "EFILING_NAMES",false, "Enter Name and Surname.");
                clsISUtilities.DataBind_DataView_To_TextBox_EFiling(this.txtEFileContactNo, "EFILING_CONTACT_NO",true, "Enter Contact Number.");
                clsISUtilities.DataBind_DataView_To_TextBox_EFiling(this.txtEFileEmail, "EFILING_EMAIL",false, "Enter Email Address.");

                clsISUtilities.NotDataBound_ComboBox_EFiling(this.cboSic7Group, "Select Sic7 Group and the Sic7 Code.");
                clsISUtilities.NotDataBound_ComboBox(this.cboStartLeaveYear, "Select Start of Leave Year.");
            }

            this.Clear_DataGridView(this.dgvCompanyDataGridView);

            this.pvtblnCompanyDataGridViewLoaded = false;
            pvtintCompanyDataGridViewRowIndex = -1;

            int intReturnCode = 0;

            for (int intRow = 0; intRow < this.pvtCompanyDataView.Count; intRow++)
            {
                if (Convert.ToInt32(pvtCompanyDataView[intRow]["COMPANY_NO"]) == pvtint64CompanyNo)
                {
                    intCurrentRow = intRow;
                }

                this.dgvCompanyDataGridView.Rows.Add("",
                                                     "",
                                                     pvtCompanyDataView[intRow]["COMPANY_DESC"].ToString(),
                                                     intRow.ToString());

                if (Convert.ToInt32(pvtCompanyDataView[intRow]["COUNT_PAY_CATEGORY_NO_CURRENT"]) > 0)
                {
                    this.dgvCompanyDataGridView[0,this.dgvCompanyDataGridView.Rows.Count - 1].Style = this.PayrollLinkDataGridViewCellStyle;
                }

                intReturnCode = Efiling_Check(pvtCompanyDataView[intRow]);

                if (intReturnCode != 0)
                {
                    this.dgvCompanyDataGridView[1, this.dgvCompanyDataGridView.Rows.Count - 1].Style = this.eFilingErrorDataGridViewCellStyle;
                }
                else
                {
                    this.dgvCompanyDataGridView[1, this.dgvCompanyDataGridView.Rows.Count - 1].Style = this.NormalDataGridViewCellStyle;
                }
            }

            this.pvtblnCompanyDataGridViewLoaded = true;

            if (this.dgvCompanyDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvCompanyDataGridView,intCurrentRow);
            }
        }

        private string EncryptStringAES(string plainText)
        {
            //Encrypted string to return 
            string outStr = null;
            RijndaelManaged aesAlg = null;

            //Generate the key from the shared secret and the salt 
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

            //Create a RijndaelManaged object with the specified key and IV. 
            aesAlg = new RijndaelManaged();
            aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
            aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

            //Create a decryptor to perform the stream transform. 
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption. 
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {

                        //Write all data to the stream. 
                        swEncrypt.Write(plainText);
                    }
                }
                outStr = Convert.ToBase64String(msEncrypt.ToArray());
            }

            aesAlg.Clear();

            return outStr;
        }

        private int Efiling_Check(DataRowView myDataRowView)
        {
            int intReturnCode = 0;

            if (myDataRowView["RES_STREET_NAME"].ToString().Trim() == ""
                | myDataRowView["EFILING_NAMES"].ToString().Trim() == ""
                | myDataRowView["EFILING_EMAIL"].ToString().Trim() == ""
                | myDataRowView["SIC7_GROUP_CODE"].ToString().Trim() == "")
            {
                intReturnCode = 1;
            }
            else
            {
                if (myDataRowView["EFILING_EMAIL"].ToString().Trim().IndexOf("@") == -1
                    | myDataRowView["EFILING_EMAIL"].ToString().Trim().IndexOf(".") == -1)
                {
                    intReturnCode = 1;
                }
                else
                {
                    if (myDataRowView["RES_SUBURB"].ToString().Trim() == ""
                    & myDataRowView["RES_CITY"].ToString().Trim() == "")
                    {
                        intReturnCode = 1;
                    }
                    else
                    {
                        if (myDataRowView["RES_ADDR_CODE"].ToString() != "0000"
                         & myDataRowView["RES_ADDR_CODE"].ToString().Length != 4)
                        {
                            intReturnCode = 1;
                        }
                        else
                        {
                            if (myDataRowView["TEL_WORK"].ToString().Length < 10)
                            {
                                intReturnCode = 1;
                            }
                            else
                            {
                                double myNumber = 0;

                                if (double.TryParse(myDataRowView["EFILING_CONTACT_NO"].ToString(), out myNumber) == false)
                                {
                                    intReturnCode = 1;
                                }
                                else
                                {
                                    if (myDataRowView["EFILING_CONTACT_NO"].ToString().Length < 10)
                                    {
                                        intReturnCode = 1;
                                    }
                                    else
                                    {
                                        if (myDataRowView["TAX_REF_NO"].ToString() == ""
                                        & myDataRowView["PAYE_REF_NO"].ToString() == "")
                                        {
                                            intReturnCode = 1;
                                        }
                                        else
                                        {
                                            if (myDataRowView["TAX_REF_NO"].ToString() != "")
                                            {
                                                int intReturn = clsISUtilities.eFiling_Income_Tax_Ref_No(myDataRowView["TAX_REF_NO"].ToString());

                                                if (intReturn != 0)
                                                {
                                                    intReturnCode = 1;
                                                }
                                            }

                                            if (myDataRowView["PAYE_REF_NO"].ToString() != "")
                                            {
                                                int intReturn = clsISUtilities.eFiling_PAYE_Ref_No(myDataRowView["PAYE_REF_NO"].ToString());

                                                if (intReturn != 0)
                                                {
                                                    intReturnCode = 1;
                                                }
                                            }

                                            if (myDataRowView["UIF_REF_NO"].ToString() != "")
                                            {
                                                int intReturn = clsISUtilities.eFiling_UIF_Ref_No(myDataRowView["UIF_REF_NO"].ToString());

                                                if (intReturn != 0)
                                                {
                                                    intReturnCode = 1;
                                                }
                                            }

                                            if (myDataRowView["SDL_REF_NO"].ToString() != "")
                                            {
                                                int intReturn = clsISUtilities.eFiling_SDL_Ref_No(myDataRowView["SDL_REF_NO"].ToString());

                                                if (intReturn != 0)
                                                {
                                                    intReturnCode = 1;
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

            return intReturnCode;
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
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
                    case "dgvCompanyDataGridView":

                        pvtintCompanyDataGridViewRowIndex = -1;
                        this.dgvCompanyDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;
                                      
                    case "dgvTradeClassifyDataGridView":

                        this.dgvTradeClassifyDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        public void btnUpdate_Click(object sender, System.EventArgs e)
        {
            this.Text = this.Text + " - Update";

            Set_Form_For_Edit();

#if(DEBUG)
            //NB FOR TESTING 
            if (this.txtPAYERefNo.Text == "")
            {
                //this.txtPAYERefNo.Text = "795072019";

            }

#endif

            this.txtCompany.Focus();
        }

        private void Set_Form_For_Edit()
        {
            bool blnNew = true;

            if (this.Text.EndsWith(" - Update") == true)
            {
                blnNew = false;

                this.rbnLockYes.Enabled = true;
                this.rbnLockNo.Enabled = true;
            }
            else
            {
                this.rbnNone.Checked = true;
            }

            clsISUtilities.Set_Form_For_Edit(blnNew);

            if (this.Text.IndexOf("- New") > -1)
            {
                this.cboStartLeaveYear.Enabled = true;
                this.cboStartLeaveYear.SelectedIndex = -1;

                this.txtTaxRefNo.Text = "0000000000";
            }
            else
            {
                if (AppDomain.CurrentDomain.GetData("AccessInd").ToString() == "S")
                {
                    this.cboStartLeaveYear.Enabled = true;
                }
            }

            this.rbnNone.Enabled = false;
            this.rbnEFilingCheck.Enabled = false;

            this.btnNew.Enabled = false;
            this.btnUpdate.Enabled = false;
            this.btnDelete.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.dgvCompanyDataGridView.Enabled = false;
            this.picCompanyLock.Visible = true;

            this.dgvTradeClassifyDataGridView.Enabled = true;
            this.dgvTradeClassifyDataGridView.Refresh();

            this.cboSic7Group.Enabled = true;
            this.dgvSic7DataGridView.Enabled = true;
            this.dgvSic7DataGridView.Refresh();

            this.txtSic7Code.Enabled = true;

            if (AppDomain.CurrentDomain.GetData("AccessInd").ToString() == "S")
            {
                if (blnNew == false)
                {
                    this.rbnEditNo.Enabled = true;
                    this.rbnEditYes.Enabled = true;
                }
            }

            if (this.Text.EndsWith(" - New") == true)
            {
                pvtCompanyDataView[clsISUtilities.DataViewIndex]["COMPANY_DEL_IND"] = "Y";

                this.txtRate2.Text = "0.00";
                this.txtRate3.Text = "2.00";

                this.txtSalaryRate2.Text = "0.00";
                this.txtSalaryRate3.Text = "2.00";

                this.cboSic7Group.SelectedIndex = -1;
                Clear_DataGridView(this.dgvSic7DataGridView);
            }
            else
            {
                if (pvtDataSet.Tables["Company"].Rows[clsISUtilities.DataViewIndex]["COUNT_PAY_CATEGORY_NO"] != System.DBNull.Value)
                {
                    if (Convert.ToInt32(pvtDataSet.Tables["Company"].Rows[clsISUtilities.DataViewIndex]["COUNT_PAY_CATEGORY_NO"]) != 0)
                    {
                        //Payroll Run for Company (Excludes TakeOn) 
                        this.txtRate1.Enabled = false;
                        this.txtRate2.Enabled = false;
                        this.txtRate3.Enabled = false;

                        this.txtSalaryRate1.Enabled = false;
                        this.txtSalaryRate2.Enabled = false;
                        this.txtSalaryRate3.Enabled = false;
                    }
                }

                if (this.cboBank.SelectedIndex > 0)
                {
                    this.txtBranchCode.Enabled = true;
                    this.txtAccountNo.Enabled = true;
                }
            }
          
            this.chkSDLExempt.Enabled = true;

            if (this.grbEfiling.Visible == true)
            {
                if (this.chkYes.Checked == false)
                {
                    this.chkYes.Enabled = true;
                }
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

            clsISUtilities.Set_Form_For_Read();

            this.rbnNone.Enabled = true;
            this.rbnEFilingCheck.Enabled = true;

            if (AppDomain.CurrentDomain.GetData("AccessInd").ToString() == "S")
            {
                this.btnNew.Enabled = true;
            }

            this.btnUpdate.Enabled = false;
            this.btnDelete.Enabled = false;
            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.rbnEditNo.Enabled = false;
            this.rbnEditYes.Enabled = false;

            this.dgvCompanyDataGridView.Enabled = true;
            this.picCompanyLock.Visible = false;

            this.dgvTradeClassifyDataGridView.Enabled = false;

            this.cboSic7Group.Enabled = false;
            this.dgvSic7DataGridView.Enabled = false;

            this.rbnLockYes.Enabled = false;
            this.rbnLockNo.Enabled = false;

            this.txtSic7Code.Enabled = false;

            this.chkSDLExempt.Enabled = false;

            this.chkYes.Enabled = false;
          
            Load_CurrentForm_Records();
        }

        private void Load_Current_Company()
        {
            int intCompanyRow = 0;

            for (int intRow = 0; intRow < this.dgvCompanyDataGridView.Rows.Count; intRow++)
            {
                if (Convert.ToInt32(this.pvtCompanyDataView[Convert.ToInt32(this.dgvCompanyDataGridView[3,intRow].Value)]["COMPANY_NO"]) == this.pvtint64CompanyNo)
                {
                    intCompanyRow = intRow;
                    break;
                }
            }

            if (this.dgvCompanyDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvCompanyDataGridView, intCompanyRow);
            }
        }

        public void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.pvtDataSet.RejectChanges();

            Set_Form_For_Read();

            Load_Current_Company();
        }

        public void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                int intReturnCode = clsISUtilities.DataBind_Save_Check();

                if (intReturnCode != 0)
                {
                    return;
                }

                if (pvtCompanyDataView[clsISUtilities.DataViewIndex]["UIF_REF_NO"].ToString() != "")
                {
                    if (pvtCompanyDataView[clsISUtilities.DataViewIndex]["PAYE_REF_NO"].ToString() == ""
                        & pvtCompanyDataView[clsISUtilities.DataViewIndex]["TAX_REF_NO"].ToString() == "")
                    {
                        CustomMessageBox.Show("Cannot Have A UIF Reference Number without\n\nPAYE Reference Number or\nIncome Tax Reference Number.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);

                        this.tabControl.TabIndex = 1;
                        this.txtUIFRefNo.Focus();
                        return;
                    }
                    else
                    {
                        if (pvtCompanyDataView[clsISUtilities.DataViewIndex]["PAYE_REF_NO"].ToString() != "")
                        {
                            if (pvtCompanyDataView[clsISUtilities.DataViewIndex]["PAYE_REF_NO"].ToString().Substring(1, 9) != pvtCompanyDataView[clsISUtilities.DataViewIndex]["UIF_REF_NO"].ToString())
                            {
                                CustomMessageBox.Show("UIF Reference Number (Last 9 Characters) must be same as PAYE Reference Number (Last 9 Characters)",
                                this.Text,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);

                                this.txtUIFRefNo.Focus();
                                return;
                            }
                        }
                    }

                    if (pvtCompanyDataView[clsISUtilities.DataViewIndex]["SDL_REF_NO"].ToString() != "")
                    {
                        if (pvtCompanyDataView[clsISUtilities.DataViewIndex]["SDL_REF_NO"].ToString() != pvtCompanyDataView[clsISUtilities.DataViewIndex]["UIF_REF_NO"].ToString())
                        {
                            CustomMessageBox.Show("UIF Reference Number (Last 9 Characters) must be same as SDL Reference Number (Last 9 Characters)",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);

                            this.txtUIFRefNo.Focus();
                            return;
                        }
                    }
                }

                if (pvtCompanyDataView[clsISUtilities.DataViewIndex]["SDL_REF_NO"].ToString() != "")
                {
                    if (pvtCompanyDataView[clsISUtilities.DataViewIndex]["PAYE_REF_NO"].ToString() == ""
                        & pvtCompanyDataView[clsISUtilities.DataViewIndex]["TAX_REF_NO"].ToString() == "")
                    {
                        CustomMessageBox.Show("Cannot Have A SDL Reference Number without\n\nPAYE Reference Number or\nIncome Tax Reference Number.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);

                        this.txtSDLRefNo.Focus();
                        return;
                    }
                    else
                    {
                        if (pvtCompanyDataView[clsISUtilities.DataViewIndex]["PAYE_REF_NO"].ToString() != "")
                        {
                            if (pvtCompanyDataView[clsISUtilities.DataViewIndex]["PAYE_REF_NO"].ToString().Substring(1, 9) != pvtCompanyDataView[clsISUtilities.DataViewIndex]["SDL_REF_NO"].ToString())
                            {
                                CustomMessageBox.Show("SDL Reference Number (Last 9 Characters) must be same as PAYE Reference Number (Last 9 Characters)",
                                this.Text,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);

                                this.txtSDLRefNo.Focus();
                                return;
                            }
                        }
                        else
                        {
                            if (pvtCompanyDataView[clsISUtilities.DataViewIndex]["TAX_REF_NO"].ToString().Substring(1, 9) != pvtCompanyDataView[clsISUtilities.DataViewIndex]["SDL_REF_NO"].ToString())
                            {
                                CustomMessageBox.Show("SDL Reference Number (Last 9 Characters) must be same as Income Tax Reference Number (Last 9 Characters)",
                                this.Text,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);

                                this.txtSDLRefNo.Focus();
                                return;
                            }
                        }
                    }
                }
  
                pvtCompanyDataView[clsISUtilities.DataViewIndex]["TRADE_CLASSIFICATION_CODE"] = this.dgvTradeClassifyDataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvTradeClassifyDataGridView)].Value.ToString();
              
                //ELR 2014-08-24
                if (this.dgvSic7DataGridView.Rows.Count > 0)
                {
                    pvtCompanyDataView[clsISUtilities.DataViewIndex]["SIC7_GROUP_CODE"] = this.dgvSic7DataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvSic7DataGridView)].Value.ToString();
                }
                else
                {
                    pvtCompanyDataView[clsISUtilities.DataViewIndex]["SIC7_GROUP_CODE"] = "";
                }

                if (this.rbnLockYes.Checked == true)
                {
                    pvtCompanyDataView[clsISUtilities.DataViewIndex]["LOCK_IND"] = "Y";
                }
                else
                {
                    pvtCompanyDataView[clsISUtilities.DataViewIndex]["LOCK_IND"] = "N";
                }

                int intYear = cboStartLeaveYear.SelectedIndex + 1;
                pvtCompanyDataView[clsISUtilities.DataViewIndex]["LEAVE_BEGIN_MONTH"] = intYear.ToString();

                if (Convert.ToDouble(this.txtRate1.Text) > Convert.ToDouble(this.txtRate2.Text)
                    & Convert.ToDouble(this.txtRate2.Text) != 0)
                {
                    CustomMessageBox.Show("Overtime Rate 1 cannot be Greater than Overtime Rate 2",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);

                    this.txtRate1.Focus();
                    return;
                }

                if (Convert.ToDouble(this.txtRate2.Text) > Convert.ToDouble(this.txtRate3.Text)
                    & Convert.ToDouble(this.txtRate3.Text) != 0)
                {
                    CustomMessageBox.Show("Overtime Rate 2 cannot be Greater than Overtime Rate 3",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);

                    this.txtRate2.Focus();
                    return;
                }

                if (Convert.ToDouble(this.txtSalaryRate1.Text) > Convert.ToDouble(this.txtSalaryRate2.Text)
                    & Convert.ToDouble(this.txtSalaryRate2.Text) != 0)
                {
                    CustomMessageBox.Show("Overtime Rate 1 cannot be Greater than Overtime Rate 2",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);

                    this.txtSalaryRate1.Focus();
                    return;
                }

                if (Convert.ToDouble(this.txtSalaryRate2.Text) > Convert.ToDouble(this.txtSalaryRate3.Text)
                    & Convert.ToDouble(this.txtSalaryRate3.Text) != 0)
                {
                    CustomMessageBox.Show("Overtime Rate 2 cannot be Greater than Overtime Rate 3",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);

                    this.txtSalaryRate2.Focus();
                    return;
                }
                                                        
                this.tabControl.SelectedIndex = 0;

                pvtTempDataSet = null;
                pvtTempDataSet = new DataSet();

                pvtTempDataSet.Tables.Add(this.pvtDataSet.Tables["Company"].Clone());
                pvtTempDataSet.Tables["Company"].ImportRow(this.pvtCompanyDataView[clsISUtilities.DataViewIndex].Row);

                pvtTempDataSet.Tables.Add(this.pvtDataSet.Tables["EfilingPeriod"].Clone());

                //pvtTempDataSet.Tables.Add(this.pvtDataSet.Tables["CompanyHeader"].Clone());

                //pvtCompanyPrintHeaderDataView = null;
                //pvtCompanyPrintHeaderDataView = new DataView(pvtDataSet.Tables["CompanyHeader"]
                //, "COMPANY_NO = " + pvtint64CompanyNo
                //, "PRINT_HEADER_NO"
                //, DataViewRowState.ModifiedCurrent);

                //for (int intRow = 0; intRow < pvtCompanyPrintHeaderDataView.Count; intRow++)
                //{
                //    pvtTempDataSet.Tables["CompanyHeader"].ImportRow(this.pvtCompanyPrintHeaderDataView[intRow].Row);
                //}

                pvtEfilingPeriodDataView = null;
                pvtEfilingPeriodDataView = new DataView(pvtDataSet.Tables["EfilingPeriod"]
                                                        ,"COMPANY_NO = " + pvtint64CompanyNo
                                                        ,""
                                                        ,DataViewRowState.ModifiedCurrent);

                if (pvtEfilingPeriodDataView.Count > 0)
                {
                    pvtTempDataSet.Tables["EfilingPeriod"].ImportRow(this.pvtEfilingPeriodDataView[0].Row);
                }

                //Compress DataSet
                pvtbytCompress = clsISUtilities.Compress_DataSet(pvtTempDataSet);

                if (this.Text.EndsWith(" - New") == true)
                {
                    object[] objParm = new object[3];
                    objParm[0] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                    objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[2] = this.pvtbytCompress;

                    pvtint64CompanyNo = (Int64)clsISUtilities.DynamicFunction("Insert_New_Record", objParm, true);

                    AppDomain.CurrentDomain.SetData("LastCompanyNo", pvtint64CompanyNo);

                    //Get Main DataSet
                    DataSet TempDataSet = (DataSet)AppDomain.CurrentDomain.GetData("DataSet");

                    DataRow dtDataRow = TempDataSet.Tables["Company"].NewRow();

                    dtDataRow["COMPANY_NO"] = pvtint64CompanyNo;
                    dtDataRow["COMPANY_DESC"] = this.txtCompany.Text;

                    if (rbnYYYY.Checked == true)
                    {
                        dtDataRow["DATE_FORMAT"] = "yyyy-MM-dd";
                    }
                    else
                    {
                        dtDataRow["DATE_FORMAT"] = "dd-MM-yyyy";
                    }

                    dtDataRow["COMPANY_ACCESS_IND"] = "A";
                 
                    TempDataSet.Tables["Company"].Rows.Add(dtDataRow);

                    TempDataSet.AcceptChanges();

                    //Fire Off Load Of Companies ComboBox on PayrollMain Form
                    Timer tmrTimerReloadCompanies = (Timer)AppDomain.CurrentDomain.GetData("TimerReloadCompanies");
                    tmrTimerReloadCompanies.Enabled = true;

                    pvtCompanyDataView[clsISUtilities.DataViewIndex]["COMPANY_NO"] = pvtint64CompanyNo;
                }
                else
                {
                    object[] objParm = new object[3];
                    objParm[0] = pvtint64CompanyNo;
                    objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[2] = this.pvtbytCompress;

                    clsISUtilities.DynamicFunction("Update_Record", objParm, true);

                    //Get Main DataSet
                    DataSet TempDataSet = (DataSet)AppDomain.CurrentDomain.GetData("DataSet");

                    for (int intRow = 0;intRow < TempDataSet.Tables["Company"].Rows.Count;intRow++)
                    {
                        if (Convert.ToInt64(TempDataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"]) == pvtint64CompanyNo)
                        {
                            if (rbnYYYY.Checked == true)
                            {
                                TempDataSet.Tables["Company"].Rows[intRow]["DATE_FORMAT"] = "yyyy-MM-dd";
                            }
                            else
                            {
                                TempDataSet.Tables["Company"].Rows[intRow]["DATE_FORMAT"] = "dd-MM-yyyy";
                            }

                            break;
                        }
                    }
                }

                //Current Company Selected Globally
                if (Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo")) == pvtint64CompanyNo)
                {
                    if (rbnYYYY.Checked == true)
                    {
                        AppDomain.CurrentDomain.SetData("DateFormat","yyyy-MM-dd");
                    }
                    else
                    {
                        AppDomain.CurrentDomain.SetData("DateFormat","dd-MM-yyyy");
                    }
                }
                
                this.pvtDataSet.AcceptChanges();

                btnCancel_Click(sender, e);
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        public void btnNew_Click(object sender, System.EventArgs e)
        {
            this.Text = this.Text + " - New";

            pvtDataRowView = this.pvtCompanyDataView.AddNew();

            this.pvtint64CompanyNo = -1;

            pvtDataRowView["COMPANY_NO"] = pvtint64CompanyNo;
            pvtDataRowView["COMPANY_DESC"] = "";
            pvtDataRowView["DYNAMIC_UPLOAD_KEY"] = "";
         
            this.txtUploadKey.Text = "";
            this.rbnEditNo.Checked = true;

            pvtDataRowView.EndEdit();

            //Set Index to First Row of View
            clsISUtilities.DataViewIndex = 0;

            Set_Form_For_Edit();
#if(DEBUG)
            this.txtCompany.Text = "Test";
#endif

            this.txtRate1.Text = "1.33";
            this.txtRate2.Text = "1.50";
            this.txtRate3.Text = "1.67";

            this.txtSalaryRate1.Text = "1.33";
            this.txtSalaryRate2.Text = "1.50";
            this.txtSalaryRate3.Text = "1.67";

            this.cboBank.SelectedIndex = 0;

            this.Set_DataGridView_SelectedRowIndex(this.dgvTradeClassifyDataGridView, 0);

            this.txtCompany.Focus();
        }

        public void btnDelete_Click(object sender, System.EventArgs e)
        {
            try
            {
                DialogResult dlgResult = CustomMessageBox.Show("Delete Company '" + pvtCompanyDataView[clsISUtilities.DataViewIndex]["COMPANY_DESC"].ToString() + "'",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dlgResult == DialogResult.Yes)
                {
                    object[] objParm = new object[1];
                    objParm[0] = pvtint64CompanyNo;

                    clsISUtilities.DynamicFunction("Delete_Record", objParm, true);

                    this.pvtCompanyDataView[clsISUtilities.DataViewIndex].Delete();

                    pvtDataSet.AcceptChanges();

                    AppDomain.CurrentDomain.SetData("LastCompanyNo", -1);

                    //Get Main DataSet
                    DataSet TempDataSet = (DataSet)AppDomain.CurrentDomain.GetData("DataSet");

                    for (int intRow = 0; intRow < TempDataSet.Tables["Company"].Rows.Count; intRow++)
                    {
                        if (Convert.ToInt64(TempDataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"]) == pvtint64CompanyNo)
                        {
                            TempDataSet.Tables["Company"].Rows[intRow].Delete();
                            break;
                        }
                    }

                    TempDataSet.AcceptChanges();

                    //Fire Off Load Of Companies ComboBox on PayrollMain Form
                    Timer tmrTimerReloadCompanies = (Timer)AppDomain.CurrentDomain.GetData("TimerReloadCompanies");
                    tmrTimerReloadCompanies.Enabled = true;

                    btnCancel_Click(sender, e);
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }
        
        private void cboBank_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (this.cboBank.SelectedIndex > 0)
                {
                    this.txtBranchCode.Text = "";
                    this.txtAccountNo.Text = "";
                    
                    this.txtBranchCode.Enabled = true;
                    this.txtAccountNo.Enabled = true;
               }
                else
                {
                    this.txtBranchCode.Enabled = false;
                    this.txtAccountNo.Enabled = false;
                }
            }
        }

        private void chkSDLExempt_CheckedChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (chkSDLExempt.Checked == true)
                {
                    this.txtSDLRefNo.Text = "";
                    this.txtSDLRefNo.Enabled = false;

                    pvtCompanyDataView[clsISUtilities.DataViewIndex]["SDL_EXEMPT_IND"] = "Y";
                }
                else
                {
                    this.txtSDLRefNo.Enabled = true;
                    pvtCompanyDataView[clsISUtilities.DataViewIndex]["SDL_EXEMPT_IND"] = System.DBNull.Value;
                }
            }
        }

        private void dgvCompanyDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnCompanyDataGridViewLoaded == true)
            {
                if (pvtintCompanyDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintCompanyDataGridViewRowIndex = e.RowIndex;

                    clsISUtilities.DataViewIndex = Convert.ToInt32(this.dgvCompanyDataGridView[3, e.RowIndex].Value);

                    pvtint64CompanyNo = Convert.ToInt64(pvtCompanyDataView[clsISUtilities.DataViewIndex]["COMPANY_NO"]);

                    pvtEfilingPeriodDataView = null;
                    pvtEfilingPeriodDataView = new DataView(pvtDataSet.Tables["eFilingPeriod"]
                                                            , "COMPANY_NO = " + pvtint64CompanyNo
                                                            , ""
                                                            , DataViewRowState.CurrentRows);

                    if (pvtEfilingPeriodDataView.Count == 0)
                    {
                        this.txtEFilingPeriod.Text = "";
                        this.chkYes.Checked = false;
                    }
                    else
                    {
                        this.txtEFilingPeriod.Text = Convert.ToDateTime(pvtEfilingPeriodDataView[0]["EFILING_PERIOD"]).ToString("d MMMM yyyy");

                        if (pvtEfilingPeriodDataView[0]["EFILING_COMPANY_CHECK_USER_NO"].ToString() == "-1")
                        {
                            this.chkYes.Checked = false;
                        }
                        else
                        {
                            this.chkYes.Checked = true;
                        }
                    }

                    //NB clsISUtilities.DataViewIndex is used Below
                    clsISUtilities.DataBind_DataView_Record_Show();

                    for (int intRow = 0; intRow < this.dgvTradeClassifyDataGridView.Rows.Count; intRow++)
                    {
                        if (this.dgvTradeClassifyDataGridView[0, intRow].Value.ToString() == pvtCompanyDataView[clsISUtilities.DataViewIndex]["TRADE_CLASSIFICATION_CODE"].ToString())
                        {
                            this.Set_DataGridView_SelectedRowIndex(this.dgvTradeClassifyDataGridView, intRow);

                            break;
                        }
                    }

                    //ELR 2014-08-24
                    pvtstrSic7GroupCode = "";

                    this.cboSic7Group.SelectedIndex = -1;
                    Clear_DataGridView(this.dgvSic7DataGridView);
                    this.txtSic7Code.Text = "";

                    if (pvtCompanyDataView[clsISUtilities.DataViewIndex]["SIC7_GROUP_CODE"].ToString() != "")
                    {
                        //ELR 2014-08-24
                        pvtstrSic7GroupCode = pvtCompanyDataView[clsISUtilities.DataViewIndex]["SIC7_GROUP_CODE"].ToString();

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
                            CustomMessageBox.Show("Cannot Find Sic7 Code\nSpeak to Administrator", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    if (pvtCompanyDataView[clsISUtilities.DataViewIndex]["LOCK_IND"].ToString() == "Y")
                    {
                        this.rbnLockYes.Checked = true;  
                    }
                    else
                    {
                        this.rbnLockNo.Checked = true;
                    }

                    //ELR 2014-08-24 (Stops ComboBox Selecting Sic7 DataGridView Row
                    pvtstrSic7GroupCode = "";

                    if (pvtCompanyDataView[clsISUtilities.DataViewIndex]["SDL_EXEMPT_IND"].ToString() == "Y")
                    {
                        this.chkSDLExempt.Checked = true;
                    }
                    else
                    {
                        this.chkSDLExempt.Checked = false;
                    }

                    if (pvtCompanyDataView[clsISUtilities.DataViewIndex]["DYNAMIC_UPLOAD_KEY"].ToString() == "")
                    {
                        this.rbnEditNo.Checked = true;
                    }
                    else
                    {
                        this.rbnEditYes.Checked = true;
                    }

                    //int intYear = Convert.ToInt32(pvtLeaveDataView[clsISUtilities.DataViewIndex]["LEAVE_BEGIN_MONTH"]) - 1;
                    this.cboStartLeaveYear.SelectedIndex = Convert.ToInt32(pvtCompanyDataView[clsISUtilities.DataViewIndex]["LEAVE_BEGIN_MONTH"]) - 1;

                    this.txtUploadKey.Text = pvtCompanyDataView[clsISUtilities.DataViewIndex]["DYNAMIC_UPLOAD_KEY"].ToString();

                    if (Convert.ToInt32(pvtCompanyDataView[clsISUtilities.DataViewIndex]["COUNT_PAY_CATEGORY_NO_CURRENT"]) == 0)
                    {
                        this.btnUpdate.Enabled = true;
                        this.btnDelete.Enabled = true;

                        this.grbCompanyLock.Visible = false;
                    }
                    else
                    {
                        this.btnUpdate.Enabled = false;
                        this.btnDelete.Enabled = false;

                        this.grbCompanyLock.Visible = true;
                    }

                    if (AppDomain.CurrentDomain.GetData("AccessInd").ToString() == "S")
                    {
                        this.btnNew.Enabled = true;
                    }
                    else
                    {
                        this.btnNew.Enabled = false;
                    }
                }
            }
        }

        private void dgvTradeClassifyDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtPAYERefNo_TextChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                int intReturnCode = clsISUtilities.eFiling_PAYE_Ref_No(this.txtPAYERefNo.Text);

                if (intReturnCode == 0)
                {
                    clsISUtilities.Paint_Parent_Marker(this.txtPAYERefNo, false);
                }
                else
                {
                    clsISUtilities.Paint_Parent_Marker(this.txtPAYERefNo, true);
                }
            }
        }

        private void EFiling_Indicator_CheckedChanged(object sender, EventArgs e)
        {
            clsISUtilities.Set_eFiling_Indicator(this.rbnEFilingCheck.Checked);

            if (this.rbnEFilingCheck.Checked == true)
            {
                this.grbEfiling.Visible = true;
            }
            else
            {
                this.grbEfiling.Visible = false;
            }
        }

        private void chkYes_CheckedChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (chkYes.Checked == true)
                {
                    pvtEfilingPeriodDataView[0]["EFILING_COMPANY_CHECK_USER_NO"] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("UserNo"));
                }
                else
                {
                    pvtEfilingPeriodDataView[0]["EFILING_COMPANY_CHECK_USER_NO"] = -1;
                }
            }
        }

        private void rbnEditNo_Click(object sender, EventArgs e)
        {
            this.txtUploadKey.Text = "";
            pvtCompanyDataView[clsISUtilities.DataViewIndex]["DYNAMIC_UPLOAD_KEY"] = "";
        }

        private void rbnEditYes_Click(object sender, EventArgs e)
        {
            this.txtUploadKey.Text = EncryptStringAES("InteractPayroll_" + pvtint64CompanyNo.ToString("00000"));

            pvtCompanyDataView[clsISUtilities.DataViewIndex]["DYNAMIC_UPLOAD_KEY"] = this.txtUploadKey.Text;
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
                    this.txtSic7Code.Text = dgvSic7DataGridView[0, e.RowIndex].Value.ToString();
                    pvtblnSetViaDataGridView = false;
                }
            }

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
    }
}
