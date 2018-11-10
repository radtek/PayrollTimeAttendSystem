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
    public partial class frmTimeAttendanceCompany : Form
    {
        clsISUtilities clsISUtilities;

        private byte[] pvtbytCompress;
        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;
        private DataView pvtCompanyDataView;
        private DataView pvtCompanyPrintHeaderDataView;
    
        private DataRowView pvtDataRowView;

        DataGridViewCellStyle PayrollLinkDataGridViewCellStyle;
        
        private Int64 pvtint64CompanyNo = -1;

        private int pvtintCompanyDataGridViewRowIndex = -1; 

        private bool pvtblnCompanyDataGridViewLoaded = false;
        private bool pvtblnChosenHeaderItemDataGridViewLoaded = false;
        private bool pvtblnHeaderItemDataGridViewLoaded = false;

        static byte[] _salt = Encoding.ASCII.GetBytes("ErrolLeRoux");
        static string sharedSecret = "Interact";
       
        public frmTimeAttendanceCompany()
        {
            InitializeComponent();

            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 118;

                this.dgvCompanyDataGridView.Height += 114;
                
                this.lblCompanyDescription.Top += 114;
                this.txtCompany.Top += 114;

                this.grbPhysical.Top += 114;
                this.grbPostal.Top += 114;

                this.grbWagesRates.Top += 114;
                this.grbSalaryRates.Top += 114;

                this.grbEncryptionKey.Top += 114;
                this.grbDateFormat.Top += 114;

                this.grbTel.Top += 114;
                
                this.grbCreateEmployeeNo.Top += 114;
                this.grbDateFormat.Top += 114;
            }
        }

        private void frmTimeAttendanceCompany_Load(object sender, System.EventArgs e)
        {
            try
            {
                PayrollLinkDataGridViewCellStyle = new DataGridViewCellStyle();
                PayrollLinkDataGridViewCellStyle.BackColor = Color.Magenta;
                PayrollLinkDataGridViewCellStyle.SelectionBackColor = Color.Magenta;
                                            
                clsISUtilities = new clsISUtilities(this,"busCompany");

                this.lblCompanySpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                
                pvtDataSet = new DataSet();

                object[] objParm = new object[2];
                objParm[0] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

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

                 clsISUtilities.DataBind_DataView_To_TextBox(this.txtStreetName, "RES_STREET_NAME", false, "",true);
                 clsISUtilities.DataBind_Special_Field(this.txtStreetName,1);

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
                
                 clsISUtilities.DataBind_DataView_To_RadioButton(this.rbnEmplNumYes, "GENERATE_EMPLOYEE_NUMBER_IND", "Y");
                 clsISUtilities.DataBind_DataView_To_RadioButton(this.rbnEmplNumNo, "GENERATE_EMPLOYEE_NUMBER_IND", "N");
                 clsISUtilities.DataBind_RadioButton_Default(this.rbnEmplNumNo);

                 clsISUtilities.DataBind_DataView_To_RadioButton(this.rbnYYYY, "DATE_FORMAT", "yyyy-MM-dd");
                 clsISUtilities.DataBind_DataView_To_RadioButton(this.rbnDD, "DATE_FORMAT", "dd-MM-yyyy");
                 clsISUtilities.DataBind_RadioButton_Default(this.rbnDD);
            }

            this.Clear_DataGridView(this.dgvCompanyDataGridView);

            this.pvtblnCompanyDataGridViewLoaded = false;

            int intReturnCode = 0;

            for (int intRow = 0; intRow < this.pvtCompanyDataView.Count; intRow++)
            {
                this.dgvCompanyDataGridView.Rows.Add("",
                                                     pvtCompanyDataView[intRow]["COMPANY_DESC"].ToString(),
                                                     intRow.ToString());

                if (Convert.ToInt32(pvtCompanyDataView[intRow]["COUNT_PAY_CATEGORY_NO_CURRENT"]) > 0)
                {
                    this.dgvCompanyDataGridView[0,this.dgvCompanyDataGridView.Rows.Count - 1].Style = this.PayrollLinkDataGridViewCellStyle;
                }
                
              
                if (Convert.ToInt32(pvtCompanyDataView[intRow]["COMPANY_NO"]) == pvtint64CompanyNo)
                {
                    intCurrentRow = intRow;
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

            this.txtCompany.Focus();
        }

        private void Set_Form_For_Edit()
        {
            bool blnNew = true;

            if (this.Text.EndsWith(" - Update") == true)
            {
                blnNew = false;
            }
           
            clsISUtilities.Set_Form_For_Edit(blnNew);

            this.btnNew.Enabled = false;
            this.btnUpdate.Enabled = false;
            this.btnDelete.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.dgvCompanyDataGridView.Enabled = false;
            this.picCompanyLock.Visible = true;

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
 
            Load_CurrentForm_Records();
        }

        private void Load_Current_Company()
        {
            int intCompanyRow = 0;

            for (int intRow = 0; intRow < this.dgvCompanyDataGridView.Rows.Count; intRow++)
            {
                if (Convert.ToInt32(this.pvtCompanyDataView[Convert.ToInt32(this.dgvCompanyDataGridView[2,intRow].Value)]["COMPANY_NO"]) == this.pvtint64CompanyNo)
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
              
                pvtTempDataSet = null;
                pvtTempDataSet = new DataSet();

                pvtTempDataSet.Tables.Add(this.pvtDataSet.Tables["Company"].Clone());
                pvtTempDataSet.Tables["Company"].ImportRow(this.pvtCompanyDataView[clsISUtilities.DataViewIndex].Row);

                pvtTempDataSet.Tables.Add(this.pvtDataSet.Tables["EfilingPeriod"].Clone());

                //Compress DataSet
                pvtbytCompress = clsISUtilities.Compress_DataSet(pvtTempDataSet);

                if (this.Text.EndsWith(" - New") == true)
                {
                    object[] objParm = new object[3];
                    objParm[0] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                    objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[2] = this.pvtbytCompress;

                    pvtint64CompanyNo = (Int64)clsISUtilities.DynamicFunction("Insert_New_Record", objParm,true);

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

                    clsISUtilities.DynamicFunction("Update_Record", objParm,true);

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


            this.txtRate1.Text = "1.33";
            this.txtRate2.Text = "1.50";
            this.txtRate3.Text = "2.00";

            this.txtSalaryRate1.Text = "1.33";
            this.txtSalaryRate2.Text = "1.50";
            this.txtSalaryRate3.Text = "2.00";
#endif
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

                    clsISUtilities.DynamicFunction("Delete_Record", objParm,true);

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
       
        private void dgvCompanyDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnCompanyDataGridViewLoaded == true)
            {
                if (pvtintCompanyDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintCompanyDataGridViewRowIndex = e.RowIndex;

                    clsISUtilities.DataViewIndex = Convert.ToInt32(this.dgvCompanyDataGridView[2,e.RowIndex].Value);

                    pvtint64CompanyNo = Convert.ToInt64(pvtCompanyDataView[clsISUtilities.DataViewIndex]["COMPANY_NO"]);

                    //NB clsISUtilities.DataViewIndex is used Below
                    clsISUtilities.DataBind_DataView_Record_Show();

                    if (pvtCompanyDataView[clsISUtilities.DataViewIndex]["DYNAMIC_UPLOAD_KEY"].ToString() == "")
                    {
                        this.rbnEditNo.Checked = true;
                    }
                    else
                    {
                        this.rbnEditYes.Checked = true;
                    }

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
    }
}
