using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InteractPayroll
{
    public partial class frmDeduction : Form
    {
        clsISUtilities clsISUtilities;
        
        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;
        private DataView pvtDeductionDataView;
        private DataView pvtEarningDataView;
        private DataView pvtEarningPercentageDataView;
        private DataView pvtCheckPercentageDataView;

        private DataRowView pvtDataRowView;

        private string pvtstrPayrollType = ""; 

        private int pvtintReturnCode = -1;
        private byte[] pvtbytCompress;

        private int pvtintDeductionNo = -1;

        private int pvtintDeductionDataGridViewRowIndex = -1;
        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
       
        DataGridViewCellStyle LockedPayrollRunDataGridViewCellStyle;

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnDeductionDataGridViewLoaded = false;
        private bool pvtblnEarningDataGridViewLoaded = false;
       
        public frmDeduction()
        {
            InitializeComponent();
        }

        private void rbnFixedAmount_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rbnFixedAmount.Checked == false)
            {
                return;
            }

            this.Clear_DataGridView(this.dgvEarningDataGridView);

            this.txtMinValue.Enabled = false;
            this.txtMaxValue.Enabled = false;

            if (this.btnSave.Enabled == true)
            {
                this.txtValue.Enabled = true;
                this.txtValue.Text = "0.00";
                this.txtMinValue.Text = "0.00";
                this.txtMaxValue.Text = "0.00";

                this.rbnEachPayPeriod.Enabled = true;
                this.rbnMonthly.Enabled = true;
              
                if (this.rbnEachPayPeriod.Checked == false
                    & this.rbnMonthly.Checked == false)
                {
                    rbnEachPayPeriod.Checked = true;
                }

                Remove_Percentage_Records();
            }

            this.lblPercent.Visible = false;
        }

        private void rbnUserToEnter_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rbnUserToEnter.Checked == false)
            {
                return;
            }

            this.Clear_DataGridView(this.dgvEarningDataGridView);

            this.lblPercent.Visible = false;
            this.txtValue.Enabled = false;
            this.txtMinValue.Enabled = false;
            this.txtMaxValue.Enabled = false;

            this.rbnEachPayPeriod.Checked = true;
            this.rbnMonthly.Checked = false;
            
            this.cboDay.SelectedIndex = -1;
            this.cboDay.Enabled = false;

            if (this.btnSave.Enabled == true)
            {
                this.txtValue.Text = "0.00";
                this.txtMinValue.Text = "0.00";
                this.txtMaxValue.Text = "0.00";

                Remove_Percentage_Records();
            }

            this.rbnEachPayPeriod.Enabled = false;
            this.rbnMonthly.Enabled = false;
        }

        private void Remove_Percentage_Records()
        {
            pvtCheckPercentageDataView = null;
            pvtCheckPercentageDataView = new DataView(pvtDataSet.Tables["EarningPercentage"],
                "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND DEDUCTION_NO = " + pvtintDeductionNo,
                "",
                DataViewRowState.CurrentRows);

            for (int intRowCount = 0; intRowCount < pvtCheckPercentageDataView.Count; intRowCount++)
            {
                pvtCheckPercentageDataView[intRowCount].Delete();

                intRowCount -= 1;
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
                    case "dgvPayrollTypeDataGridView":

                        pvtintPayrollTypeDataGridViewRowIndex = -1;
                        this.dgvPayrollTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvDeductionDataGridView":

                        pvtintDeductionDataGridViewRowIndex = -1;
                        this.dgvDeductionDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEarningDataGridView":

                        this.dgvEarningDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void rbnMonthly_Click(object sender, System.EventArgs e)
        {
            this.cboDay.Enabled = true;
            this.cboDay.SelectedIndex = -1;
        }

        private void rbnEachPayPeriod_Click(object sender, System.EventArgs e)
        {
            this.cboDay.Enabled = false;
            this.cboDay.SelectedIndex = -1;
        }

        private void frmDeduction_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this,"busDeduction");

                this.lblDeductionSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayrollTypeSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPercentEarningsSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.dgvPayrollTypeDataGridView.Rows.Add("Wages");
                this.dgvPayrollTypeDataGridView.Rows.Add("Salaries");

                pvtblnPayrollTypeDataGridViewLoaded = true;

                this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView, 0);

                LockedPayrollRunDataGridViewCellStyle = new DataGridViewCellStyle();
                LockedPayrollRunDataGridViewCellStyle.BackColor = Color.Magenta;
                LockedPayrollRunDataGridViewCellStyle.SelectionBackColor = Color.Magenta;
                
                pvtDataSet = new DataSet();

                object[] objParm = new object[3];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[2] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);
             
                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                clsISUtilities.DataBind_ComboBox_Load(this.cboNumberSubAccount,pvtDataSet.Tables["SubAccount"],"DEDUCTION_SUB_ACCOUNT_COUNT","DEDUCTION_SUB_ACCOUNT_COUNT");

                clsISUtilities.NotDataBound_ComboBox(this.cboDay, "Select Day of Month.");

                Load_CurrentForm_Records();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void Load_CurrentForm_Records()
        {
            this.grbDeductionLock.Visible = false;

            this.Clear_DataGridView(this.dgvDeductionDataGridView);
            
            pvtDeductionDataView = null;

            pvtDeductionDataView = new DataView(pvtDataSet.Tables["Deduction"],
                "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                "DEDUCTION_DESC",
                DataViewRowState.CurrentRows);

            clsISUtilities.DataViewIndex = 0;

            if (clsISUtilities.DataBind_Form_And_DataView_To_Class() == false)
            {
                clsISUtilities.DataBind_DataView_And_Index(this, pvtDeductionDataView, "DEDUCTION_NO");

                clsISUtilities.DataBind_DataView_To_TextBox(this.txtDeduction, "DEDUCTION_DESC", true, "Enter Deduction Description.", true);

                clsISUtilities.DataBind_DataView_To_RadioButton(this.rbnUserToEnter, "DEDUCTION_TYPE_IND", "U");
                clsISUtilities.DataBind_RadioButton_Default(this.rbnUserToEnter);
                clsISUtilities.DataBind_DataView_To_RadioButton(this.rbnFixedAmount, "DEDUCTION_TYPE_IND", "F");
                clsISUtilities.DataBind_DataView_To_RadioButton(this.rbnPercentageOfEarnings, "DEDUCTION_TYPE_IND", "P");

                clsISUtilities.DataBind_DataView_To_RadioButton(this.rbnEachPayPeriod, "DEDUCTION_PERIOD_IND", "E");
                clsISUtilities.DataBind_RadioButton_Default(this.rbnEachPayPeriod);
                clsISUtilities.DataBind_DataView_To_RadioButton(this.rbnMonthly, "DEDUCTION_PERIOD_IND", "M");

                clsISUtilities.DataBind_DataView_To_Numeric_TextBox(this.txtValue, "DEDUCTION_VALUE", 2, false, "", false, 0,false);
                clsISUtilities.DataBind_Control_Required_If_Enabled(this.txtValue, "Enter Value.");

                clsISUtilities.DataBind_DataView_To_Numeric_TextBox(this.txtMinValue, "DEDUCTION_MIN_VALUE", 2, false, "", false, 0, false);
                //clsISUtilities.DataBind_Control_Required_If_Enabled(this.txtMinValue, "Enter Minimum Value.");

                clsISUtilities.DataBind_DataView_To_Numeric_TextBox(this.txtMaxValue, "DEDUCTION_MAX_VALUE", 2, false, "", false, 0, false);
                //clsISUtilities.DataBind_Control_Required_If_Enabled(this.txtMaxValue, "Enter Maximum Value.");

                clsISUtilities.DataBind_DataView_To_RadioButton(this.rbnNoLoanType, "DEDUCTION_LOAN_TYPE_IND", "N");
                clsISUtilities.DataBind_RadioButton_Default(this.rbnNoLoanType);
                clsISUtilities.DataBind_DataView_To_RadioButton(this.rbnYesLoanType, "DEDUCTION_LOAN_TYPE_IND", "Y");

                clsISUtilities.DataBind_DataView_To_ComboBox(this.cboNumberSubAccount, "DEDUCTION_SUB_ACCOUNT_COUNT", false, "", false);
                clsISUtilities.DataBind_Control_Required_If_Enabled(this.cboNumberSubAccount, "Select Number of Sub-Account.");

                //clsISUtilities.DataBind_DataView_To_TextBox(this.txtIRP5Code, "IRP5_CODE", false, "", false);
                clsISUtilities.DataBind_DataView_To_Numeric_TextBox(this.txtIRP5Code, "IRP5_CODE",0, false, "", false,0,true);

                clsISUtilities.DataBind_DataView_To_TextBox(this.txtHeader1, "DEDUCTION_REPORT_HEADER1", true, "Enter Spreadsheet Report Header Line 1.", true);
                clsISUtilities.DataBind_DataView_To_TextBox(this.txtHeader2, "DEDUCTION_REPORT_HEADER2", false, "", true);
            }
            else
            {
                clsISUtilities.Re_DataBind_DataView(pvtDeductionDataView);
            }
            
            if (pvtstrPayrollType == "W")
            {
                this.rbnMonthly.Visible = true;
                this.cboDay.Visible = true;
            }
            else
            {
                this.rbnMonthly.Visible = false;
                this.cboDay.Visible = false;
            }

            if (pvtDeductionDataView.Count == 0)
            {
                Clear_Form_Fields();

                this.btnUpdate.Enabled = false;
                this.btnDelete.Enabled = false;

                Set_Form_For_Read();
            }
            else
            {
                int intComboSelectedIndex = 0;

                Set_Form_For_Read();

                if (this.pvtDataSet.Tables["Company"].Rows[0]["ACCESS_IND"].ToString() == "R")
                {
                    this.btnUpdate.Enabled = false;
                }
                else
                {
                    this.btnUpdate.Enabled = true;
                }

                pvtblnDeductionDataGridViewLoaded = false;

                for (int intRowCount = 0; intRowCount < pvtDeductionDataView.Count; intRowCount++)
                {
                    this.dgvDeductionDataGridView.Rows.Add("",
                                                           pvtDeductionDataView[intRowCount]["IRP5_CODE"].ToString(),
                                                           pvtDeductionDataView[intRowCount]["DEDUCTION_DESC"].ToString(),
                                                           intRowCount.ToString());

                    if (Convert.ToInt32(pvtDeductionDataView[intRowCount]["DEDUCTION_NO"]) == pvtintDeductionNo)
                    {
                        intComboSelectedIndex = intRowCount;
                    }

                    if (pvtDeductionDataView[intRowCount]["PAYROLL_LINK"] != System.DBNull.Value)
                    {
                        this.dgvDeductionDataGridView[0,this.dgvDeductionDataGridView.Rows.Count - 1].Style = LockedPayrollRunDataGridViewCellStyle;
                        this.grbDeductionLock.Visible = true;

                        this.btnNew.Enabled = false;
                        this.btnUpdate.Enabled = false;
                        this.btnDelete.Enabled = false;
                    }
                }

                pvtblnDeductionDataGridViewLoaded = true;

                this.Set_DataGridView_SelectedRowIndex(this.dgvDeductionDataGridView, intComboSelectedIndex);
            }
        }

        public void btnNew_Click(object sender, System.EventArgs e)
        {
            this.Text += " - New";

            pvtintDeductionNo = 0;
            pvtDataRowView = this.pvtDeductionDataView.AddNew();

            pvtDataRowView.BeginEdit();

            pvtDataRowView["COMPANY_NO"] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
            pvtDataRowView["PAY_CATEGORY_TYPE"] = pvtstrPayrollType;
            pvtDataRowView["EMPLOYEE_NO"] = 0;
            pvtDataRowView["DEDUCTION_NO"] = 0;
            pvtDataRowView["DEDUCTION_SUB_ACCOUNT_NO"] = 0;
            pvtDataRowView["TIE_BREAKER"] = 0;
            pvtDataRowView["IRP5_CODE"] = System.DBNull.Value;
            pvtDataRowView["DEDUCTION_DESC"] = "";
            pvtDataRowView["DEDUCTION_SUB_ACCOUNT_COUNT"] = 1;
            pvtDataRowView["DEDUCTION_VALUE"] = 0;
            pvtDataRowView["DEDUCTION_MIN_VALUE"] = 0;
            pvtDataRowView["DEDUCTION_MAX_VALUE"] = 0;
            pvtDataRowView["DEDUCTION_DEL_IND"] = "Y";

            pvtDataRowView.EndEdit();

            //Set Index to First Row of View
            clsISUtilities.DataViewIndex = 0;

            Set_Form_For_Edit();

            //Set_Form_For_Edit Initialise all Values to Zero Except Parts of Key 
            pvtDeductionDataView[clsISUtilities.DataViewIndex]["DEDUCTION_SUB_ACCOUNT_COUNT"] = 1;

            this.txtDeduction.Focus();
        }

        private void Set_Form_For_Edit()
        {
            bool blnNew = true;

            this.dgvDeductionDataGridView.Enabled = false;
            this.dgvPayrollTypeDataGridView.Enabled = false;

            this.picDeductionLock.Visible = true;
            this.picPayrollTypeLock.Visible = true;

            this.btnNew.Enabled = false;
            this.btnUpdate.Enabled = false;
            this.btnDelete.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            if (this.Text.IndexOf(" - Update", 0) > 0)
            {
                blnNew = false;
            }

            clsISUtilities.Set_Form_For_Edit(blnNew);
            
            if (pvtDeductionDataView.Count > 0)
            {
                if (Convert.ToInt32(pvtDeductionDataView[clsISUtilities.DataViewIndex]["DEDUCTION_NO"]) <= 200
                    & this.Text.IndexOf(" - Update", 0) > 0)
                {
                    this.txtDeduction.Enabled = false;
                    this.txtHeader1.Enabled = false;
                    this.txtHeader2.Enabled = false;
                }
            }

            this.txtValue.Enabled = true;

            if (pvtDeductionDataView[clsISUtilities.DataViewIndex]["DEDUCTION_TYPE_IND"].ToString() != "P")
            {
                if (pvtDeductionDataView[clsISUtilities.DataViewIndex]["DEDUCTION_TYPE_IND"].ToString() == "U")
                {
                    this.rbnEachPayPeriod.Enabled = false;
                    this.rbnMonthly.Enabled = false;
                    this.txtValue.Enabled = false;
                }
            }
            else
            {
                this.txtMinValue.Enabled = true;
                this.txtMaxValue.Enabled = true;
            }

            if (this.rbnMonthly.Checked == true)
            {
                this.cboDay.Enabled = true;
            }
            
            if (this.rbnYesLoanType.Checked == true)
            {
                this.cboNumberSubAccount.Enabled = true;
            }

            this.dgvEarningDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dgvEarningDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;

            this.dgvEarningDataGridView.Enabled = true;

            if (this.dgvEarningDataGridView.Rows.Count > 0)
            {
                dgvEarningDataGridView.CurrentCell = dgvEarningDataGridView[2, 0];
            }

            //Display Only
            this.txtIRP5Code.Enabled = false;
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

            this.dgvDeductionDataGridView.Enabled = true;
            this.dgvPayrollTypeDataGridView.Enabled = true;

            this.picDeductionLock.Visible = false;
            this.picPayrollTypeLock.Visible = false;

            this.btnNew.Enabled = true;
            //this.btnUpdate.Enabled = false;
            this.btnDelete.Enabled = true;

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            clsISUtilities.Set_Form_For_Read();

            this.dgvEarningDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvEarningDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

            if (this.dgvDeductionDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvDeductionDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvDeductionDataGridView));
            }
        }

        private void Clear_Form_Fields()
        {
            this.txtDeduction.Text = "";
            this.txtValue.Text = "0.00";
            this.txtMinValue.Text = "0.00";
            this.txtMaxValue.Text = "0.00";
            this.cboDay.SelectedIndex = -1;

            this.Clear_DataGridView(this.dgvEarningDataGridView);
        }

        public void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.pvtDataSet.RejectChanges();

            //Errol To Look At Combination
            //int intRow = this.flxgDeduction.Row;

            this.Set_Form_For_Read();

            //this.flxgDeduction.Row = 0;
            //this.flxgDeduction.Row = intRow;
        }

        public void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                pvtintReturnCode = clsISUtilities.DataBind_Save_Check();

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

                pvtTempDataSet.Tables.Add(this.pvtDataSet.Tables["Deduction"].Clone());

                pvtTempDataSet.Tables["Deduction"].ImportRow(pvtDeductionDataView[clsISUtilities.DataViewIndex].Row);

                pvtTempDataSet.Tables.Add(this.pvtDataSet.Tables["EarningPercentage"].Clone());

                pvtEarningPercentageDataView = null;
                pvtEarningPercentageDataView = new DataView(pvtDataSet.Tables["EarningPercentage"],
                    "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND DEDUCTION_NO = " + pvtintDeductionNo,
                    "",
                    DataViewRowState.Added | DataViewRowState.Deleted | DataViewRowState.ModifiedCurrent);

                for (int intRow = 0; intRow < pvtEarningPercentageDataView.Count; intRow++)
                {
                    pvtTempDataSet.Tables["EarningPercentage"].ImportRow(pvtEarningPercentageDataView[intRow].Row);
                }

                //Compress DataSet
                pvtbytCompress = clsISUtilities.Compress_DataSet(pvtTempDataSet);

                if (this.Text.IndexOf(" - New", 0) > 0)
                {
                    object[] objParm = new object[2];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[1] = pvtbytCompress;

                    pvtintDeductionNo = (int)clsISUtilities.DynamicFunction("Insert_New_Record", objParm,true);
                    
                    pvtDeductionDataView[clsISUtilities.DataViewIndex]["DEDUCTION_NO"] = pvtintDeductionNo;
                    pvtDeductionDataView[clsISUtilities.DataViewIndex]["PAYROLL_LINK"] = System.DBNull.Value;

                    //2017-02-24 Create opposite PAY_CATEGORY_TYPE 
                    DataRowView DataRowView = pvtDeductionDataView.AddNew();

                    DataRowView.BeginEdit();

                    DataRowView["COMPANY_NO"] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));

                    if (pvtDeductionDataView[clsISUtilities.DataViewIndex]["PAY_CATEGORY_TYPE"].ToString() == "W")
                    {
                        DataRowView["PAY_CATEGORY_TYPE"] =  "S";
                    }
                    else
                    {
                        DataRowView["PAY_CATEGORY_TYPE"] = "W";
                    }

                    DataRowView["EMPLOYEE_NO"] = 0;
                    DataRowView["DEDUCTION_NO"] = Convert.ToInt16(pvtDeductionDataView[clsISUtilities.DataViewIndex]["DEDUCTION_NO"]);
                    DataRowView["DEDUCTION_SUB_ACCOUNT_NO"] = Convert.ToInt16(pvtDeductionDataView[clsISUtilities.DataViewIndex]["DEDUCTION_SUB_ACCOUNT_NO"]);

                    DataRowView["DEDUCTION_DESC"] = pvtDeductionDataView[clsISUtilities.DataViewIndex]["DEDUCTION_DESC"].ToString();

                    DataRowView["DEDUCTION_SUB_ACCOUNT_COUNT"] = Convert.ToInt16(pvtDeductionDataView[clsISUtilities.DataViewIndex]["DEDUCTION_SUB_ACCOUNT_COUNT"]);

                    if (pvtDeductionDataView[clsISUtilities.DataViewIndex]["IRP5_CODE"] != System.DBNull.Value)
                    {
                        DataRowView["IRP5_CODE"] = Convert.ToInt16(pvtDeductionDataView[clsISUtilities.DataViewIndex]["IRP5_CODE"]);
                    }

                    DataRowView["DEDUCTION_REPORT_HEADER1"] = pvtDeductionDataView[clsISUtilities.DataViewIndex]["DEDUCTION_REPORT_HEADER1"].ToString();
                    DataRowView["DEDUCTION_REPORT_HEADER2"] = pvtDeductionDataView[clsISUtilities.DataViewIndex]["DEDUCTION_REPORT_HEADER2"].ToString();
                    DataRowView["DEDUCTION_LOAN_TYPE_IND"] = pvtDeductionDataView[clsISUtilities.DataViewIndex]["DEDUCTION_LOAN_TYPE_IND"].ToString();

                    DataRowView["DEDUCTION_DEL_IND"] = "Y";

                    DataRowView["DEDUCTION_TYPE_IND"] = "U";
                    DataRowView["DEDUCTION_VALUE"] = 0;
                    DataRowView["DEDUCTION_MIN_VALUE"] = 0;
                    DataRowView["DEDUCTION_MAX_VALUE"] = 0;

                    DataRowView["DEDUCTION_PERIOD_IND"] = "E";
                    DataRowView["DEDUCTION_DAY_VALUE"] = 0;
                    DataRowView["TIE_BREAKER"] = 1;
                    
                    DataRowView.EndEdit();
                    
                    pvtEarningPercentageDataView = null;
                    pvtEarningPercentageDataView = new DataView(pvtDataSet.Tables["EarningPercentage"],
                        "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND DEDUCTION_NO = 0 ",
                        "",
                        DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < pvtEarningPercentageDataView.Count; intRow++)
                    {
                        pvtEarningPercentageDataView[intRow]["DEDUCTION_NO"] = pvtintDeductionNo;

                        intRow -= 1;
                    }
                }
                else
                {
                    object[] objParm = new object[3];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[2] = pvtbytCompress;

                    pvtintReturnCode = (int)clsISUtilities.DynamicFunction("Update_Record", objParm,true);
                    
                    if (pvtintReturnCode == 9999)
                    {
                        this.pvtDataSet.Tables["Deduction"].RejectChanges();

                        pvtDeductionDataView[clsISUtilities.DataViewIndex]["PAYROLL_LINK"] = 0;

                        this.dgvDeductionDataGridView.Rows[this.Get_DataGridView_SelectedRowIndex(this.dgvDeductionDataGridView)].HeaderCell.Style = LockedPayrollRunDataGridViewCellStyle;
  
                        CustomMessageBox.Show("This Deduction is currently being used in a Payroll Run.\r\n\r\nUpdate Cancelled.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        if (pvtintReturnCode != 0)
                        {
                            CustomMessageBox.Show("Number of Sub-Accounts used = " + pvtintReturnCode.ToString() + "\r\n\r\nYou cannot set a Value of " + this.cboNumberSubAccount.SelectedItem.ToString(),
                                this.Text,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);

                            return;
                        }
                        else
                        {
                            dgvDeductionDataGridView[2, pvtintDeductionDataGridViewRowIndex].Value = this.txtDeduction.Text.Trim();
                        }
                    }
                }


                this.pvtDataSet.AcceptChanges();

                if (this.Text.EndsWith(" - New") == true)
                {
                    Load_CurrentForm_Records();
                }
                else
                {
                    btnCancel_Click(sender, e);
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private int Save_Check()
        {
            if (this.rbnPercentageOfEarnings.Checked == true)
            {
                if (Convert.ToDouble(this.txtValue.Text.Trim()) >= 100)
                {
                    CustomMessageBox.Show("Percentage Value cannot be greater than or equal to 100.",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);

                    this.txtValue.Focus();
                    return 1;
                }

                pvtCheckPercentageDataView = null;
                pvtCheckPercentageDataView = new DataView(pvtDataSet.Tables["EarningPercentage"],
                    "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND DEDUCTION_NO = " + pvtintDeductionNo,
                    "",
                    DataViewRowState.CurrentRows);

                if (pvtCheckPercentageDataView.Count == 0)
                {
                    CustomMessageBox.Show("Select from '" + this.lblPercentEarningsSpreadsheetHeader.Text + "'",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);

                    return 1;
                }
            }

            return 0;
        }

        public void btnDelete_Click(object sender, System.EventArgs e)
        {
            try
            {
                DialogResult dlgResult = CustomMessageBox.Show("Delete Deduction '" + pvtDeductionDataView[clsISUtilities.DataViewIndex]["DEDUCTION_DESC"].ToString() + "'",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dlgResult == DialogResult.Yes)
                {
                    object[] objParm = new object[4];
                   
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[2] = pvtstrPayrollType;
                    objParm[3] = Convert.ToInt32(pvtDeductionDataView[clsISUtilities.DataViewIndex]["DEDUCTION_NO"]);

                    clsISUtilities.DynamicFunction("Delete_Record", objParm,true);
                   
                    pvtintDeductionNo = -1;

                    pvtDeductionDataView[clsISUtilities.DataViewIndex].Delete();

                    this.pvtDataSet.Tables["Deduction"].AcceptChanges();

                    if (dgvPayrollTypeDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView, pvtintPayrollTypeDataGridViewRowIndex);
                    }
                }
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

            this.txtDeduction.Focus();
        }

        private void rbnPercentageOfEarnings_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rbnPercentageOfEarnings.Checked == false)
            {
                return;
            }

            pvtEarningPercentageDataView = null;
            pvtEarningPercentageDataView = new DataView(pvtDataSet.Tables["EarningPercentage"],
                "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND DEDUCTION_NO = " + pvtintDeductionNo,
                "",
                DataViewRowState.CurrentRows);

            pvtEarningDataView = null;
            pvtEarningDataView = new DataView(pvtDataSet.Tables["EarningList"],
                "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                "",
                DataViewRowState.CurrentRows);

            this.Clear_DataGridView(this.dgvEarningDataGridView);

            this.pvtblnEarningDataGridViewLoaded = false;
            bool blnChecked = false;

            if (pvtEarningDataView.Count > 0)
            {
                for (int intRow = 0; intRow < pvtEarningDataView.Count; intRow++)
                {
                    blnChecked = false;

                    

                    if (pvtintDeductionNo == 1
                        | pvtintDeductionNo == 2)
                    {
                        //Commission
                        if (pvtEarningDataView[intRow]["IRP5_CODE"].ToString() == "3606"
                            & pvtintDeductionNo == 2)
                        {
                        }
                        else
                        {
                            blnChecked = true;
                        }
                    }
                    else
                    {
                        pvtCheckPercentageDataView = null;
                        pvtCheckPercentageDataView = new DataView(pvtDataSet.Tables["EarningPercentage"],
                            "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND DEDUCTION_NO = "
                            + pvtintDeductionNo + " AND EARNING_NO = " + pvtEarningDataView[intRow]["EARNING_NO"].ToString(),
                            "",
                            DataViewRowState.CurrentRows);

                        if (pvtCheckPercentageDataView.Count > 0)
                        {
                            blnChecked = true;
                        }
                    }

                    this.dgvEarningDataGridView.Rows.Add(pvtEarningDataView[intRow]["IRP5_CODE"].ToString(),
                                                         pvtEarningDataView[intRow]["EARNING_DESC"].ToString(),
                                                         blnChecked,
                                                         pvtEarningDataView[intRow]["EARNING_NO"].ToString());
                }
            }

            this.pvtblnEarningDataGridViewLoaded = true;

            if (this.btnSave.Enabled == true)
            {
                dgvEarningDataGridView.CurrentCell = dgvEarningDataGridView[2, 0];
            }
            else
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvEarningDataGridView, 0);
            }

            this.lblPercent.Visible = true;

            if (this.btnSave.Enabled == true)
            {
                this.txtValue.Enabled = true;
                this.txtMinValue.Enabled = true;
                this.txtMaxValue.Enabled = true;

                this.rbnEachPayPeriod.Enabled = true;
                this.rbnMonthly.Enabled = true;
                
                if (this.rbnEachPayPeriod.Checked == false
                    & this.rbnMonthly.Checked == false)
                {
                    rbnEachPayPeriod.Checked = true;
                }

                this.txtValue.Text = "0.00";
                this.txtMinValue.Text = "0.00";
                this.txtMaxValue.Text = "0.00";
            }
        }

        private void txtDeduction_Leave(object sender, System.EventArgs e)
        {
            if (this.Text.IndexOf(" - New", 0) > 0)
            {
                if (this.txtHeader1.Text.Trim() == "")
                {
                    if (txtDeduction.Text.Trim().IndexOf(" ", 0) > -1)
                    {
                        this.txtHeader1.Text = this.txtDeduction.Text.Trim().Substring(0, txtDeduction.Text.Trim().IndexOf(" ", 0));
                        this.txtHeader2.Text = this.txtDeduction.Text.Trim().Substring(txtDeduction.Text.Trim().IndexOf(" ", 0) + 1);
                    }
                    else
                    {
                        this.txtHeader1.Text = this.txtDeduction.Text.Trim();
                    }

                    if (this.txtHeader1.Text.Length > 10)
                    {
                        this.txtHeader1.Text = this.txtHeader1.Text.Substring(0, 10);
                    }

                    if (this.txtHeader2.Text.Length > 10)
                    {
                        this.txtHeader2.Text = this.txtHeader2.Text.Substring(0, 10);
                    }
                }
            }
        }

        private void rbnYesLoanType_Click(object sender, System.EventArgs e)
        {
            this.cboNumberSubAccount.SelectedIndex = 0;
            this.cboNumberSubAccount.Enabled = true;
        }

        private void rbnNoLoanType_Click(object sender, System.EventArgs e)
        {
            this.cboNumberSubAccount.SelectedIndex = 0;
            this.cboNumberSubAccount.Enabled = false;
        }

        private void dgvPayrollTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPayrollTypeDataGridViewLoaded == true)
            {
                if (pvtintPayrollTypeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayrollTypeDataGridViewRowIndex = e.RowIndex;

                    if (e.RowIndex == 0)
                    {
                        this.lblDeductionLock.Text = "Deduction Records are Locked Due to Current Wage Run.";

                        pvtstrPayrollType = "W";
                    }
                    else
                    {
                        this.lblDeductionLock.Text = "Deduction Records are Locked Due to Current Salary Run.";

                        pvtstrPayrollType = "S";
                    }

                    if (pvtDataSet != null)
                    {
                        Load_CurrentForm_Records();
                    }
                }
            }
        }

        private void dgvDeductionDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnDeductionDataGridViewLoaded == true)
            {
                if (pvtintDeductionDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintDeductionDataGridViewRowIndex = e.RowIndex;

                    clsISUtilities.DataViewIndex = Convert.ToInt32(this.dgvDeductionDataGridView[3, e.RowIndex].Value);

                    pvtintDeductionNo = Convert.ToInt32(pvtDeductionDataView[clsISUtilities.DataViewIndex]["DEDUCTION_NO"]);

                    //Load Data Bound Records
                    clsISUtilities.DataBind_DataView_Record_Show();

                    if (pvtDeductionDataView[clsISUtilities.DataViewIndex]["DEDUCTION_DEL_IND"].ToString() == "Y")
                    {
                        if (this.pvtDataSet.Tables["Company"].Rows[0]["ACCESS_IND"].ToString() == "R")
                        {
                            this.btnDelete.Enabled = false;
                        }
                        else
                        {
                            if (grbDeductionLock.Visible == false)
                            {
                                this.btnDelete.Enabled = true;
                            }
                        }
                    }
                    else
                    {
                        this.btnDelete.Enabled = false;
                    }

                    //Tax or UIF
                    if (pvtDeductionDataView[clsISUtilities.DataViewIndex]["DEDUCTION_NO"].ToString() == "1"
                        | Convert.ToInt32(pvtDeductionDataView[clsISUtilities.DataViewIndex]["DEDUCTION_NO"]) == 2)
                    {
                        this.btnUpdate.Enabled = false;

                        if (pvtDeductionDataView[clsISUtilities.DataViewIndex]["DEDUCTION_NO"].ToString() == "1")
                        {
                            this.Clear_DataGridView(this.dgvEarningDataGridView);
                        }
                    }
                    else
                    {
                        if (this.pvtDataSet.Tables["Company"].Rows[0]["ACCESS_IND"].ToString() == "R")
                        {
                            this.btnUpdate.Enabled = false;
                        }
                        else
                        {
                            if (pvtDeductionDataView[clsISUtilities.DataViewIndex]["PAYROLL_LINK"] == System.DBNull.Value)
                            {
                                if (grbDeductionLock.Visible == false)
                                {
                                    this.btnUpdate.Enabled = true;
                                }
                            }
                            else
                            {
                                this.btnUpdate.Enabled = false;
                            }
                        }
                    }

                    //Make Percentage Fire 
                    if (pvtDeductionDataView[clsISUtilities.DataViewIndex]["DEDUCTION_TYPE_IND"].ToString() == "P")
                    {
                        this.rbnPercentageOfEarnings.Checked = false;
                        this.rbnPercentageOfEarnings.Checked = true;
                    }

                    this.cboDay.SelectedIndex = -1;

                    if (Convert.ToInt32(pvtDeductionDataView[clsISUtilities.DataViewIndex]["DEDUCTION_DAY_VALUE"]) > 0)
                    {
                        if (Convert.ToInt32(pvtDeductionDataView[clsISUtilities.DataViewIndex]["DEDUCTION_DAY_VALUE"]) == 99)
                        {
                            this.cboDay.SelectedIndex = this.cboDay.Items.Count - 1;
                        }
                        else
                        {
                            this.cboDay.SelectedIndex = Convert.ToInt32(pvtDeductionDataView[clsISUtilities.DataViewIndex]["DEDUCTION_DAY_VALUE"]) - 1;
                        }
                    }
                }
            }
        }

        private void dgvEarningDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvEarningDataGridView.Rows.Count > 0
                & pvtblnEarningDataGridViewLoaded == true)
            {
            }
        }

        private void dgvEarningDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (e.RowIndex > -1
                & e.ColumnIndex == 2)
                {
                    pvtCheckPercentageDataView = null;
                    pvtCheckPercentageDataView = new DataView(pvtDataSet.Tables["EarningPercentage"],
                        "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND DEDUCTION_NO = " + pvtintDeductionNo + " AND EARNING_NO = " + this.dgvEarningDataGridView[3, e.RowIndex].Value.ToString(),
                        "",
                        DataViewRowState.CurrentRows);

                    if (pvtCheckPercentageDataView.Count == 0)
                    {
                        pvtDataRowView = this.pvtEarningPercentageDataView.AddNew();

                        pvtDataRowView.BeginEdit();

                        //Set Key for Find
                        pvtDataRowView["COMPANY_NO"] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                        pvtDataRowView["PAY_CATEGORY_TYPE"] = pvtstrPayrollType;
                        pvtDataRowView["DEDUCTION_NO"] = pvtintDeductionNo;
                        pvtDataRowView["DEDUCTION_SUB_ACCOUNT_NO"] = 1;
                        pvtDataRowView["EARNING_NO"] = this.dgvEarningDataGridView[3, e.RowIndex].Value.ToString();

                        pvtDataRowView.EndEdit();
                    }
                    else
                    {
                        pvtCheckPercentageDataView[0].Row.Delete();
                    }
                }
            }
        }

        private void cboDay_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                 int intValue = 0;

                if (this.cboDay.SelectedItem.ToString() == "End")
                {
                    intValue = 99;
                }
                else
                {
                    intValue = this.cboDay.SelectedIndex + 1;
                }

                if (Convert.ToInt32(pvtDeductionDataView[clsISUtilities.DataViewIndex]["DEDUCTION_DAY_VALUE"]) != intValue)
                {
                    pvtDeductionDataView[clsISUtilities.DataViewIndex]["DEDUCTION_DAY_VALUE"] = intValue;
                }
            }
        }
    }
}
