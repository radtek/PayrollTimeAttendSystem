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
    public partial class frmEarning : Form
    {
        clsISUtilities clsISUtilities;

        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;
        private byte[] pvtbytCompress; 

        private DataView pvtEarningListDataView;
        private DataView pvtEarningSelectedDataView;

        DataGridViewCellStyle LockedPayrollRunDataGridViewCellStyle;

        private int pvtintEarningNo;

        private int pvtintEarningSelectedDataViewIndex = -1;
        private int pvtintEarningListDataViewIndex = -1;

        private string pvtstrPayrollType = "";

        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
        private int pvtintEarningListDataGridViewRowIndex = -1;
        private int pvtintEarningSelectedDataGridViewRowIndex = -1;

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnEarningListDataGridViewLoaded = false;
        private bool pvtblnEarningSelectedDataGridViewLoaded = false;
        
        public frmEarning()
        {
            InitializeComponent();
        }

        private void frmEarning_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this,"busEarning");

                this.lblPayrollTypeSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEarningSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSelectedEarningSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                LockedPayrollRunDataGridViewCellStyle = new DataGridViewCellStyle();
                LockedPayrollRunDataGridViewCellStyle.BackColor = Color.Magenta;
                LockedPayrollRunDataGridViewCellStyle.SelectionBackColor = Color.Magenta;
                 
                this.dgvPayrollTypeDataGridView.Rows.Add("Wages");
                this.dgvPayrollTypeDataGridView.Rows.Add("Salaries");

                pvtblnPayrollTypeDataGridViewLoaded = true;

                this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView,0);

                pvtDataSet = new DataSet();

                object[] objParm = new object[3];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[2] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);
                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                //Used so That Red Markers Appear When Not Initialised
                clsISUtilities.NotDataBound_Numeric_TextBox(this.txtValue,"Enter Value.",4,0);
                clsISUtilities.NotDataBound_ComboBox(cboIRP5Code, "Enter IRP5 Value.");

                for (int intRowCount = 0; intRowCount < pvtDataSet.Tables["IRP5"].Rows.Count; intRowCount++)
                {
                    this.cboIRP5Code.Items.Add(pvtDataSet.Tables["IRP5"].Rows[intRowCount]["IRP5_CODE"].ToString());
                }

                if (this.pvtDataSet.Tables["Company"].Rows[0]["ACCESS_IND"].ToString() == "A")
                {
                    this.btnUpdate.Enabled = true;
                }

                Load_CurrentForm_Records();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void Load_CurrentForm_Records()
        {
            this.grbLeaveLock.Visible = false;
            this.btnNew.Enabled = true;

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

            pvtEarningListDataView = null;
            pvtEarningListDataView = new DataView(pvtDataSet.Tables["EarningList"],
                "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                "EARNING_DESC",
                DataViewRowState.CurrentRows);

            pvtEarningSelectedDataView = null;
            pvtEarningSelectedDataView = new DataView(pvtDataSet.Tables["EarningSelected"],
                "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                "EARNING_DESC",
                DataViewRowState.CurrentRows);

            this.Clear_DataGridView(this.dgvEarningListDataGridView);
            pvtblnEarningListDataGridViewLoaded = false;

			for (int intRowCount = 0; intRowCount < pvtEarningListDataView.Count;intRowCount++)
			{
                this.dgvEarningListDataGridView.Rows.Add(pvtEarningListDataView[intRowCount]["IRP5_CODE"].ToString(),
				                                         pvtEarningListDataView[intRowCount]["EARNING_DESC"].ToString(),
				                                         pvtEarningListDataView[intRowCount]["EARNING_NO"].ToString());
			}

            pvtblnEarningListDataGridViewLoaded = true;

            this.Set_DataGridView_SelectedRowIndex(this.dgvEarningListDataGridView, 0);

            Load_Selected_Earnings();
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
            Set_DataGridView_SelectedRowIndex_Continue:

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

                    case "dgvEarningListDataGridView":

                        pvtintEarningListDataGridViewRowIndex = -1;
                        this.dgvEarningListDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEarningSelectedDataGridView":

                        pvtintEarningSelectedDataGridViewRowIndex = -1;
                        this.dgvEarningSelectedDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    default:

                        MessageBox.Show("No Entry for " + myDataGridView.Name + " in Set_DataGridView_SelectedRowIndex", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            else
            {
                myDataGridView.CurrentCell = myDataGridView[0, intRow];

                goto Set_DataGridView_SelectedRowIndex_Continue;
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

        private void Load_Selected_Earnings()
        {
            this.Clear_DataGridView(this.dgvEarningSelectedDataGridView);

            pvtblnEarningSelectedDataGridViewLoaded = false;
            int intSelectedIndex = 0;

            for (int intRowCount = 0; intRowCount < pvtEarningSelectedDataView.Count; intRowCount++)
            {
                this.dgvEarningSelectedDataGridView.Rows.Add("",
                                                             pvtEarningSelectedDataView[intRowCount]["IRP5_CODE"].ToString(),
                                                             pvtEarningSelectedDataView[intRowCount]["EARNING_DESC"].ToString(),
                                                             pvtEarningSelectedDataView[intRowCount]["EARNING_NO"].ToString());

                if (Convert.ToInt32(pvtEarningSelectedDataView[intRowCount]["EARNING_NO"]) == pvtintEarningNo)
                {
                    intSelectedIndex = intRowCount;
                }
              
                if (pvtEarningSelectedDataView[intRowCount]["PAYROLL_LINK"] != System.DBNull.Value)
                {
                    this.dgvEarningSelectedDataGridView[0,this.dgvEarningSelectedDataGridView.Rows.Count - 1].Style = LockedPayrollRunDataGridViewCellStyle;
                    this.grbLeaveLock.Visible = true;

                    this.btnNew.Enabled = false;
                    this.btnUpdate.Enabled = false;
                    this.btnDelete.Enabled = false;
                }
            }

            pvtblnEarningSelectedDataGridViewLoaded = true;

            if (this.dgvEarningSelectedDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvEarningSelectedDataGridView, intSelectedIndex);
            }
        }

        private void cboDay_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.btnSave.Enabled == true
                & this.cboDay.SelectedIndex > -1)
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

                if (Convert.ToInt32(pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_DAY_VALUE"]) != intValue)
                {
                    pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_DAY_VALUE"] = intValue;
                }

                this.clsISUtilities.Paint_Parent_Marker(this.cboDay, false);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            DialogResult dlgResult = CustomMessageBox.Show("Are you absolutely sure you want to Create a New Earning.\n\nFirst Check to see that the Earning you require does Not exist under 'List of Earnings'.\n\nTo Access 'List of Earnings', use The Update Option.",
                                this.Text,
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Exclamation);

            if (dlgResult == DialogResult.Yes)
            {
                this.Text += " - New";

                DataRowView pvtDataRowView = this.pvtEarningSelectedDataView.AddNew();

                pvtDataRowView.BeginEdit();

                //Set Key for Find
                pvtDataRowView["COMPANY_NO"] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo").ToString());
                pvtDataRowView["PAY_CATEGORY_TYPE"] = pvtstrPayrollType;
                pvtDataRowView["EMPLOYEE_NO"] = 0;
                pvtDataRowView["EARNING_NO"] = -1;
                pvtDataRowView["TIE_BREAKER"] = 1;
                pvtDataRowView["EARNING_DESC"] = "";
                pvtDataRowView["EARNING_TYPE_IND"] = "U";
                pvtDataRowView["EARNING_TYPE_DEFAULT"] = "N";
                pvtDataRowView["EARNING_PERIOD_IND"] = "E";
                pvtDataRowView["EARNING_DAY_VALUE"] = 0;
                pvtDataRowView["AMOUNT"] = 0;
                pvtDataRowView["IRP5_CODE"] = 0;
                pvtDataRowView["EARNING_REPORT_HEADER1"] = "";
                pvtDataRowView["EARNING_REPORT_HEADER2"] = "";
                pvtDataRowView["EARNING_DEL_IND"] = "Y";

                pvtDataRowView.EndEdit();

                //Set Earning No
                pvtintEarningNo = -1;

                Set_Form_For_Edit();

                //EARNING_DESC = "" Makes it First Record
                pvtintEarningSelectedDataViewIndex = 0;

                this.txtEarningDesc.Focus();
            }
        }

        private void Set_Form_For_Edit()
        {
            bool blnNew = true;

            this.dgvPayrollTypeDataGridView.Enabled = false;
            this.picPayrollTypeLock.Visible = true;
           
            this.btnNew.Enabled = false;
            this.btnUpdate.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            if (this.Text.IndexOf("- New") > -1)
            {
                Clear_Form_Fields();

                //For Some Reason, dgvEarningListDataGridView_RowEnter  Fires
                pvtblnEarningSelectedDataGridViewLoaded = false;
                this.dgvEarningListDataGridView.Enabled = false;
                pvtblnEarningSelectedDataGridViewLoaded = true;

                this.dgvEarningSelectedDataGridView.Enabled = false;

                this.picListEarnings.Visible = true;
                this.picSelectedEarningsLock.Visible = true;

                this.txtEarningDesc.Enabled = true;
                this.txtHeader1.Enabled = true;
                this.txtHeader2.Enabled = true;
                this.cboIRP5Code.Enabled = true;

                this.rbnUserEnterValue.Checked = true;
                this.rbnEachPayPeriod.Checked = true;

                this.rbnFixedValue.Enabled = true;
                this.rbnUserEnterValue.Enabled = true;
                this.rbnMultiple.Enabled = true;
                this.rbnMacro.Enabled = true;
            }
            else
            {
                blnNew = false;
                this.btnAdd.Enabled = true;
            }

            //Set So That Field Markers Appear
            clsISUtilities.Set_Form_For_Edit(blnNew);

            if (this.Text.IndexOf("- Update") > -1)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvEarningSelectedDataGridView,this.Get_DataGridView_SelectedRowIndex(dgvEarningSelectedDataGridView));
            }
        }

        private void EarningType_Click(object sender, System.EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.grbMacro.Visible = false;

                this.rbnEachPayPeriod.Checked = true;
                this.txtValue.Text = "0.00";

                this.cboDay.Enabled = false;
                this.cboDay.SelectedIndex = -1;

                //this.clsISUtilities.Paint_Parent_Marker(this.cboDay, false);

                pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_PERIOD_IND"] = "E";
                pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_DAY_VALUE"] = 0;

                RadioButton myRadioButton = (RadioButton)sender;

                if (myRadioButton.Name == "rbnUserEnterValue"
                    | myRadioButton.Name == "rbnMacro")
                {
                    if (myRadioButton.Name == "rbnUserEnterValue")
                    {
                        pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_TYPE_IND"] = "U";
                    }
                    else
                    {
                        pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_TYPE_IND"] = "M";
                        this.grbMacro.Visible = true;
                    }

                    this.txtValue.Enabled = false;

                    this.rbnMonthly.Enabled = false;
                    rbnEachPayPeriod.Enabled = false;
                }
                else
                {
                    this.rbnMonthly.Enabled = true;
                    this.rbnEachPayPeriod.Enabled = true;
                    this.txtValue.Enabled = true;

                    if (myRadioButton.Name == "rbnMultiple")
                    {
                        pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_TYPE_IND"] = "X";
                        this.txtValue.Text = "0.0000";
                    }
                    else
                    {
                        //Fixed Value
                        pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_TYPE_IND"] = "F";
                    }
                }
            }
        }
     
        public void btnAdd_Click(object sender, System.EventArgs e)
        {
            if (this.dgvEarningListDataGridView.Rows.Count > 0)
            {
                DataRowView pvtDataRowView = this.pvtEarningSelectedDataView.AddNew();

                pvtDataRowView.BeginEdit();

                //Set Key for Find
                pvtDataRowView["COMPANY_NO"] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo").ToString());
                pvtDataRowView["PAY_CATEGORY_TYPE"] = this.pvtstrPayrollType;
                pvtDataRowView["EMPLOYEE_NO"] = 0;
                pvtDataRowView["EARNING_NO"] = Convert.ToInt32(this.pvtEarningListDataView[pvtintEarningListDataViewIndex]["EARNING_NO"]);

                pvtintEarningNo = Convert.ToInt32(this.pvtEarningListDataView[pvtintEarningListDataViewIndex]["EARNING_NO"]);

                pvtDataRowView["EARNING_DESC"] = this.pvtEarningListDataView[pvtintEarningListDataViewIndex]["EARNING_DESC"].ToString();

                if (this.pvtEarningListDataView[pvtintEarningListDataViewIndex]["IRP5_CODE"] != System.DBNull.Value)
                {
                    pvtDataRowView["IRP5_CODE"] = Convert.ToInt32(this.pvtEarningListDataView[pvtintEarningListDataViewIndex]["IRP5_CODE"]);
                }

                pvtDataRowView["EARNING_REPORT_HEADER1"] = this.pvtEarningListDataView[pvtintEarningListDataViewIndex]["EARNING_REPORT_HEADER1"].ToString();
                pvtDataRowView["EARNING_REPORT_HEADER2"] = this.pvtEarningListDataView[pvtintEarningListDataViewIndex]["EARNING_REPORT_HEADER2"].ToString();

                if (this.pvtEarningListDataView[pvtintEarningListDataViewIndex]["EARNING_TYPE_IND"] != System.DBNull.Value)
                {
                    pvtDataRowView["EARNING_TYPE_IND"] = this.pvtEarningListDataView[pvtintEarningListDataViewIndex]["EARNING_TYPE_IND"].ToString();
                }
                else
                {
                    pvtDataRowView["EARNING_TYPE_IND"] = "U";
                }

                pvtDataRowView["EARNING_TYPE_DEFAULT"] = "Y";
                pvtDataRowView["EARNING_DAY_VALUE"] = 0;
                pvtDataRowView["AMOUNT"] = 0;

                pvtDataRowView["TIE_BREAKER"] = 0;

                pvtDataRowView["EARNING_PERIOD_IND"] = "E";

                pvtDataRowView.EndEdit();

                this.pvtEarningListDataView[pvtintEarningListDataViewIndex].Delete();

                //To Fix
                DataGridViewRow myDataGridViewRow = this.dgvEarningListDataGridView.Rows[this.dgvEarningListDataGridView.CurrentRow.Index];

                this.dgvEarningListDataGridView.Rows.Remove(myDataGridViewRow);

                //Reload to Find Correct Row (DataView Could Change) 
                Load_Selected_Earnings();

                txtEarningDesc.Text = pvtDataRowView["EARNING_DESC"].ToString();
                txtEarningDesc.Enabled = false;

                if (this.dgvEarningListDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvEarningListDataGridView, 0);
                }
            }
        }

        private void PayPeriod_Click(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                RadioButton myRadioButton = (RadioButton)sender;

                if (myRadioButton.Name == "rbnEachPayPeriod")
                {
                    pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_DAY_VALUE"] = 0;
                    pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_PERIOD_IND"] = "E";
                 
                    this.cboDay.Enabled = false;
                    this.cboDay.SelectedIndex = -1;

                    this.clsISUtilities.Paint_Parent_Marker(this.cboDay, false);
                }
                else
                {
                    //Monthly
                    pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_PERIOD_IND"] = "M";
                    this.cboDay.SelectedIndex = -1;
                    this.cboDay.Enabled = true;

                    this.clsISUtilities.Paint_Parent_Marker(this.cboDay, true);
                }
            }
        }

        private void Clear_Form_Fields()
        {
            this.txtEarningDesc.Text = "";
            this.cboDay.SelectedIndex = -1;

            this.txtHeader1.Text = "";
            this.txtHeader2.Text = "";
            this.cboIRP5Code.SelectedIndex = -1;

            this.rbnFixedValue.Checked = false;
            this.rbnUserEnterValue.Checked = false;
            this.rbnMacro.Checked = false;
            this.rbnSystemDefined.Checked = false;

            this.rbnEachPayPeriod.Checked = false;
            this.rbnMonthly.Checked = false;
        }

        private void Set_Form_For_Read()
        {
            this.btnNew.Enabled = true;

            if (this.pvtDataSet.Tables["Company"].Rows[0]["ACCESS_IND"].ToString() == "A")
            {
                this.btnUpdate.Enabled = true;
            }

            this.btnAdd.Enabled = false;

            this.picListEarnings.Visible = false;
            this.picSelectedEarningsLock.Visible = false;
            this.picPayrollTypeLock.Visible = false;

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.dgvEarningListDataGridView.Enabled = true;
            this.dgvEarningSelectedDataGridView.Enabled = true;
            this.dgvPayrollTypeDataGridView.Enabled = true;

            clsISUtilities.Set_Form_For_Read();

            this.txtEarningDesc.Enabled = false;
            this.txtHeader1.Enabled = false;
            this.txtHeader2.Enabled = false;

            this.rbnUserEnterValue.Enabled = false;
            this.rbnFixedValue.Enabled = false;
            this.rbnMultiple.Enabled = false;
            this.rbnMacro.Enabled = false;

            this.rbnEachPayPeriod.Enabled = false;
            this.rbnMonthly.Enabled = false;

            this.cboDay.Enabled = false;

            this.txtValue.Enabled = false;
            
            this.cboDay.SelectedIndex = -1;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Text = this.Text.Substring(0, this.Text.LastIndexOf("-") - 1);

            this.pvtDataSet.RejectChanges();

            this.Set_Form_For_Read();

            Load_CurrentForm_Records();
        }
    
        private void txtEarningDesc_TextChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (txtEarningDesc.Text.Trim().IndexOf(" ", 0) > -1)
                {
                    this.txtHeader1.Text = this.txtEarningDesc.Text.Trim().Substring(0, txtEarningDesc.Text.Trim().IndexOf(" ", 0));

                    if (this.txtHeader1.Text.Length > 10)
                    {
                        this.txtHeader1.Text = this.txtHeader1.Text.Substring(0, 10);
                    }

                    this.txtHeader2.Text = this.txtEarningDesc.Text.Trim().Substring(txtEarningDesc.Text.Trim().IndexOf(" ", 0) + 1);

                    if (this.txtHeader2.Text.Length > 10)
                    {
                        this.txtHeader2.Text = this.txtHeader2.Text.Substring(0, 10);
                    }
                }
                else
                {
                    this.txtHeader1.Text = this.txtEarningDesc.Text.Trim();

                    if (this.txtHeader1.Text.Length > 10)
                    {
                        this.txtHeader1.Text = this.txtHeader1.Text.Substring(0, 10);
                    }

                    this.txtHeader2.Text = "";
                }

                if (this.Text.IndexOf("- New") > -1)
                {
                    for (int intRow = 0; intRow < pvtEarningSelectedDataView.Count; intRow++)
                    {
                        if (pvtEarningSelectedDataView[intRow]["EARNING_NO"].ToString() == "-1")
                        {
                            pvtEarningSelectedDataView[intRow]["EARNING_DESC"] = this.txtEarningDesc.Text.Trim();

                            break;
                        }
                    }
                }
                else
                {
                    pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_DESC"] = this.txtEarningDesc.Text.Trim();
                }

                //NB DataView Sort is on EARNING_DESC Therefore Refind Position
                for (int intRow = 0; intRow < pvtEarningSelectedDataView.Count; intRow++)
                {
                    if (pvtEarningSelectedDataView[intRow]["EARNING_NO"].ToString() == this.pvtintEarningNo.ToString())
                    {
                        pvtintEarningSelectedDataViewIndex = intRow;

                        pvtEarningSelectedDataView[intRow]["EARNING_REPORT_HEADER1"] = this.txtHeader1.Text.Trim();
                        pvtEarningSelectedDataView[intRow]["EARNING_REPORT_HEADER2"] = this.txtHeader2.Text.Trim();
                  
                        break;
                    }
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
      	    this.Text += " - Update";

			Set_Form_For_Edit();

            this.pvtDataSet.AcceptChanges();

			this.txtEarningDesc.Focus();
		}

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dlgResult = CustomMessageBox.Show("Delete Earning '" + this.pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_DESC"].ToString() + "'",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dlgResult == DialogResult.Yes)
                {
                    object[] objParm = new object[4];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[2] = pvtstrPayrollType;
                    objParm[3] = this.pvtintEarningNo;

                    clsISUtilities.DynamicFunction("Delete_Record", objParm,true);
                    
                    pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex].Delete();

                    this.pvtDataSet.AcceptChanges();

                    this.Load_CurrentForm_Records();
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                pvtTempDataSet = null;
                pvtTempDataSet = new DataSet();
                
                //Earning Rows
                pvtTempDataSet.Tables.Add(this.pvtDataSet.Tables["EarningSelected"].Clone());

                DataView EarningDataView = new DataView(pvtDataSet.Tables["EarningSelected"],
                    "(EARNING_NO > 9 AND EARNING_NO < 200) OR EARNING_NO > 201 OR EARNING_NO = -1",
                    "",
                     DataViewRowState.Added | DataViewRowState.ModifiedCurrent | DataViewRowState.Deleted);

                for (int intRow = 0; intRow < EarningDataView.Count; intRow++)
                {
                    if (this.Text.IndexOf("New") > -1)
                    {
                        if (EarningDataView[intRow]["EARNING_DESC"].ToString().Trim() == "")
                        {
                            CustomMessageBox.Show("Enter Earning Description.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

                            this.txtEarningDesc.Focus();

                            return;
                        }
                    }

                    //F=Fixed X=Multiple  
                    if (EarningDataView[intRow]["EARNING_TYPE_IND"].ToString() == "F"
                        | EarningDataView[intRow]["EARNING_TYPE_IND"].ToString() == "X")
                    {
                        if (Convert.ToDouble(EarningDataView[intRow]["AMOUNT"]) == 0)
                        {
                            if (this.Text.IndexOf("Update") > -1)
                            {
                                for (int intErrorRow = 0; intErrorRow < this.dgvEarningSelectedDataGridView.Rows.Count; intErrorRow++)
                                {
                                    if (this.dgvEarningSelectedDataGridView[3, intErrorRow].Value.ToString() == EarningDataView[intRow]["EARNING_NO"].ToString())
                                    {
                                        this.Set_DataGridView_SelectedRowIndex(this.dgvEarningSelectedDataGridView, intErrorRow);

                                        break;
                                    }
                                }
                            }

                            CustomMessageBox.Show("Value must be Greater Than Zero.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

                            return;
                        }

                        if (EarningDataView[intRow]["EARNING_PERIOD_IND"].ToString() == "M")
                        {
                            if (Convert.ToDouble(EarningDataView[intRow]["EARNING_DAY_VALUE"]) == 0)
                            {
                                if (this.Text.IndexOf("Update") > -1)
                                {
                                    for (int intErrorRow = 0; intErrorRow < this.dgvEarningSelectedDataGridView.Rows.Count; intErrorRow++)
                                    {
                                        if (this.dgvEarningSelectedDataGridView[3, intErrorRow].Value.ToString() == EarningDataView[intRow]["EARNING_NO"].ToString())
                                        {
                                            this.Set_DataGridView_SelectedRowIndex(this.dgvEarningSelectedDataGridView, intErrorRow);


                                        }
                                    }
                                }

                                CustomMessageBox.Show("Select Day of Month.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

                                return;
                            }
                        }
                    }

                    if (EarningDataView[intRow]["IRP5_CODE"].ToString() == "0")
                    {
                        if (this.Text.IndexOf("Update") > -1)
                        {
                            for (int intErrorRow = 0; intErrorRow < this.dgvEarningSelectedDataGridView.Rows.Count; intErrorRow++)
                            {
                                if (this.dgvEarningSelectedDataGridView[3, intErrorRow].Value.ToString() == EarningDataView[intRow]["EARNING_NO"].ToString())
                                {
                                    this.Set_DataGridView_SelectedRowIndex(this.dgvEarningSelectedDataGridView, intErrorRow);
                                }
                            }
                        }

                        CustomMessageBox.Show("Select IRP5 Code.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return;
                    }

                    string intMyEarningNo = EarningDataView[intRow]["EARNING_NO"].ToString();
                 
                    pvtTempDataSet.Tables["EarningSelected"].ImportRow(EarningDataView[intRow].Row);

                    EarningDataView[intRow].Delete();

                    intRow += 1;
                }

                //Compress DataSet
                pvtbytCompress = clsISUtilities.Compress_DataSet(pvtTempDataSet);

                //Must be A Change
                if (pvtTempDataSet.Tables["EarningSelected"].Rows.Count != 0)
                {
                    if (this.Text.IndexOf(" - New", 0) > 0)
                    {
                        object[] objParm = new object[2];
                        objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                        objParm[1] = pvtbytCompress;
                        
                        pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Insert_New_Record", objParm,true);
                    }
                    else
                    {
                        object[] objParm = new object[4];
                        objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                        objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo").ToString());
                        objParm[2] = pvtstrPayrollType;
                        objParm[3] = pvtbytCompress;
                        
                        pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Update_Record", objParm,true);
                    }

                    pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                    if (Convert.ToInt32(pvtTempDataSet.Tables["Check"].Rows[0]["RETURN_CODE"]) == 9999)
                    {
                        this.pvtDataSet.RejectChanges();

                        for (int intRow = 0; intRow < pvtEarningSelectedDataView.Count; intRow++)
                        {
                            pvtEarningSelectedDataView[intRow]["PAYROLL_LINK"] = 0;
                        }

                        CustomMessageBox.Show("Earnings are currently being used in a Payroll Run.\r\n\r\nUpdate Cancelled.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        if (this.Text.IndexOf(" - New", 0) > 0)
                        {
                            pvtintEarningNo = Convert.ToInt32(pvtTempDataSet.Tables["EarningSelected"].Rows[0]["EARNING_NO"]);
                        }
                    }

                    pvtTempDataSet.Tables.Remove("Check");

                    //Errol - 2015-02-20 Fix
                    this.pvtDataSet.AcceptChanges();

                    this.pvtDataSet.Merge(pvtTempDataSet);
                }

                btnCancel_Click(sender, e);
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
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
                        this.lblEarningLock.Text = "Earning Records are Locked Due to Current Wage Run.";
                        pvtstrPayrollType = "W";
                    }
                    else
                    {
                        this.lblEarningLock.Text = "Earning Records are Locked Due to Current Salary Run.";
                        pvtstrPayrollType = "S";
                    }

                    pvtintEarningNo = -1;

                    if (pvtDataSet != null)
                    {
                        Load_CurrentForm_Records();
                    }
                }
            }
        }

        private void dgvEarningListDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnEarningListDataGridViewLoaded == true)
            {
                if (pvtintEarningListDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintEarningListDataGridViewRowIndex = e.RowIndex;

                    int intEarningNo = Convert.ToInt32(this.dgvEarningListDataGridView[2, e.RowIndex].Value);

                    for (int intRowCount = 0; intRowCount < this.pvtEarningListDataView.Count; intRowCount++)
                    {
                        if (intEarningNo == Convert.ToInt32(pvtEarningListDataView[intRowCount]["EARNING_NO"]))
                        {
                            pvtintEarningListDataViewIndex = intRowCount;
                            break;
                        }
                    }
                }
            }
        }

        private void dgvEarningSelectedDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnEarningSelectedDataGridViewLoaded == true)
            {
                if (pvtintEarningSelectedDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintEarningSelectedDataGridViewRowIndex = e.RowIndex;

                    pvtintEarningNo = Convert.ToInt32(this.dgvEarningSelectedDataGridView[3, e.RowIndex].Value);

                    for (int intRowCount = 0; intRowCount < pvtEarningSelectedDataView.Count; intRowCount++)
                    {
                        if (pvtintEarningNo == Convert.ToInt32(pvtEarningSelectedDataView[intRowCount]["EARNING_NO"]))
                        {
                            pvtintEarningSelectedDataViewIndex = intRowCount;
                            break;
                        }
                    }

                    this.txtEarningDesc.Text = pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_DESC"].ToString();

                    this.txtHeader1.Text = pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_REPORT_HEADER1"].ToString();
                    this.txtHeader2.Text = pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_REPORT_HEADER2"].ToString();

                    if (pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_TYPE_IND"].ToString() == "X")
                    {
                        //Multiple - 4 Decimal Places
                        this.txtValue.Text = Convert.ToDouble(pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["AMOUNT"]).ToString("######0.0000");
                    }
                    else
                    {
                        this.txtValue.Text = Convert.ToDouble(pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["AMOUNT"]).ToString("#######0.00");
                    }

                    if (this.btnSave.Enabled == true)
                    {
                        this.rbnUserEnterValue.Enabled = false;
                        this.rbnFixedValue.Enabled = false;
                        this.rbnMultiple.Enabled = false;

                        this.rbnEachPayPeriod.Enabled = false;
                        this.rbnMonthly.Enabled = false;

                        this.cboDay.Enabled = false;

                        this.txtValue.Enabled = false;
                    }

                    if (pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_TYPE_IND"].ToString() == "S"
                    | pvtintEarningNo < 10
                    | pvtintEarningNo == 200
                    | pvtintEarningNo == 201)
                    {
                        this.rbnSystemDefined.Checked = true;
                    }
                    else
                    {
                        if (pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_TYPE_IND"].ToString() == "M")
                        {
                            this.rbnMacro.Checked = true;
                        }
                        else
                        {
                            if (this.btnSave.Enabled == true)
                            {
                                this.rbnUserEnterValue.Enabled = true;
                                this.rbnFixedValue.Enabled = true;
                                this.rbnMultiple.Enabled = true;
                                this.rbnMonthly.Enabled = true;
                            }

                            if (pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_TYPE_IND"].ToString() == "U")
                            {
                                this.rbnUserEnterValue.Checked = true;
                            }
                            else
                            {
                                if (pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_TYPE_IND"].ToString() == "F"
                                    | pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_TYPE_IND"].ToString() == "X")
                                {
                                    if (pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_TYPE_IND"].ToString() == "F")
                                    {
                                        this.rbnFixedValue.Checked = true;
                                    }
                                    else
                                    {
                                        this.rbnMultiple.Checked = true;
                                    }

                                    if (this.btnSave.Enabled == true)
                                    {
                                        this.txtValue.Enabled = true;
                                        this.rbnEachPayPeriod.Enabled = true;
                                        this.rbnMultiple.Enabled = true;
                                    }
                                }
                                else
                                {
                                    if (this.btnSave.Enabled == false)
                                    {
                                        CustomMessageBox.Show("flxgChosenEarning_AfterRowColChange EARNING_TYPE_IND Not Set",this.Text,MessageBoxButtons.OK,MessageBoxIcon.Error);
                                    }
                                    else
                                    {
                                        //In Edit Mode
                                        this.rbnUserEnterValue.Checked = false;
                                        this.rbnFixedValue.Checked = false;
                                        this.rbnMacro.Checked = false;

                                        this.rbnEachPayPeriod.Checked = false;
                                        this.rbnMonthly.Checked = false;
                                    }
                                }
                            }
                        }
                    }

                    if (pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_PERIOD_IND"].ToString() == "E")
                    {
                        this.cboDay.SelectedIndex = -1;
                        this.rbnEachPayPeriod.Checked = true;
                    }
                    else
                    {
                        if (pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_PERIOD_IND"].ToString() == "M")
                        {
                            this.rbnMonthly.Checked = true;

                            if (Convert.ToInt32(pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_DAY_VALUE"]) == 99)
                            {
                                this.cboDay.SelectedIndex = this.cboDay.Items.Count - 1;
                            }
                            else
                            {
                                this.cboDay.SelectedIndex = Convert.ToInt32(pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_DAY_VALUE"]) - 1;
                            }

                            if (this.btnSave.Enabled == true)
                            {
                                this.cboDay.Enabled = true;
                            }
                        }
                        else
                        {
                            if (this.btnSave.Enabled == false)
                            {
                                CustomMessageBox.Show("flxgChosenEarning_AfterRowColChange EARNING_PERIOD_IND Not Set",this.Text,MessageBoxButtons.OK,MessageBoxIcon.Error);
                            }
                        }
                    }

                    if (pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_PERIOD_IND"].ToString() == "M")
                    {
                        if (Convert.ToInt32(pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_DAY_VALUE"]) == 99)
                        {
                            this.cboDay.SelectedIndex = this.cboDay.Items.Count - 1;
                        }
                        else
                        {
                            this.cboDay.SelectedIndex = Convert.ToInt32(pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_DAY_VALUE"]) - 1;
                        }
                    }
                    else
                    {
                        this.cboDay.SelectedIndex = -1;
                    }

                    this.cboIRP5Code.SelectedIndex = -1;

                    for (int intRowCount = 0; intRowCount < pvtDataSet.Tables["IRP5"].Rows.Count; intRowCount++)
                    {
                        if (pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["IRP5_CODE"].ToString() == pvtDataSet.Tables["IRP5"].Rows[intRowCount]["IRP5_CODE"].ToString())
                        {
                            this.cboIRP5Code.SelectedIndex = intRowCount;
                            break;
                        }
                    }

                    //What Follows is Used for Edit Mode
                    //What Follows is Used for Edit Mode
                    this.btnDelete.Enabled = false;

                    if (this.btnSave.Enabled == true)
                    {
                        //Default Earning
                        if (pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_TYPE_DEFAULT"].ToString() == "Y")
                        {
                            this.txtEarningDesc.Enabled = false;
                            this.txtHeader1.Enabled = false;
                            this.txtHeader2.Enabled = false;
                            this.cboIRP5Code.Enabled = false;
                        }
                        else
                        {
                            this.txtEarningDesc.Enabled = true;
                            this.txtHeader1.Enabled = true;
                            this.txtHeader2.Enabled = true;
                            this.cboIRP5Code.Enabled = true;
                        }

                        this.rbnEachPayPeriod.Enabled = false;
                        this.rbnMonthly.Enabled = false;

                        if (this.rbnMonthly.Checked == true)
                        {
                            this.cboDay.Enabled = true;
                        }

                        //System Defined
                        if (pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_TYPE_IND"].ToString() == "S"
                        || pvtintEarningNo < 10
                        || pvtintEarningNo == 200
                        || pvtintEarningNo == 201)
                        {
                            this.rbnUserEnterValue.Enabled = false;
                            this.rbnFixedValue.Enabled = false;
                            this.rbnMultiple.Enabled = false;
                            this.rbnMacro.Enabled = false;

                            this.txtValue.Enabled = false;
                        }
                        else
                        {
                            this.rbnUserEnterValue.Enabled = true;
                            this.rbnFixedValue.Enabled = true;
                            this.rbnMultiple.Enabled = true;
                            this.rbnMacro.Enabled = true;

                            //All Except User to Enter
                            if (pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_TYPE_IND"].ToString() != "U")
                            {
                                this.rbnEachPayPeriod.Enabled = true;
                                this.rbnMonthly.Enabled = true;
                            }

                            if (pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_TYPE_IND"].ToString() != "M"
                                & pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_TYPE_IND"].ToString() != "U")
                            {
                                this.txtValue.Enabled = true;
                            }
                        }
                    }
                    else
                    {
                        if (pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_TYPE_DEFAULT"].ToString() != "Y")
                        {
                            if (pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_DEL_IND"].ToString() == "Y")
                            {
                                if (this.grbLeaveLock.Visible == false)
                                {
                                    this.btnDelete.Enabled = true;
                                }
                            }
                        }
                        else
                        {
                            if (pvtintEarningNo > 9
                                & pvtintEarningNo < 150)
                            {
                                if (this.grbLeaveLock.Visible == false)
                                {
                                    this.btnDelete.Enabled = true;
                                }
                            }
                        }

                        if (this.pvtDataSet.Tables["Company"].Rows[0]["ACCESS_IND"].ToString() == "R")
                        {
                            this.btnUpdate.Enabled = false;
                        }
                        else
                        {
                            if (pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["PAYROLL_LINK"] == System.DBNull.Value)
                            {
                                if (this.grbLeaveLock.Visible == false)
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
                }
            }
        }

        private void txtValue_TextChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (txtValue.Text.Replace(".", "").Trim() == "")
                {
                    pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["AMOUNT"] = 0;
                }
                else
                {
                    if (Convert.ToDouble(pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["AMOUNT"]) != Convert.ToDouble(this.txtValue.Text))
                    {
                        if (pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["EARNING_TYPE_IND"].ToString() == "X")
                        {
                            pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["AMOUNT"] = Convert.ToDouble(this.txtValue.Text);
                        }
                        else
                        {
                            pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["AMOUNT"] = Convert.ToDouble(this.txtValue.Text).ToString("######0.00");
                        }
                    }
                }
            }
        }

        private void cboDay_EnabledChanged(object sender, EventArgs e)
        {
            if (cboDay.Enabled == true
                & cboDay.SelectedIndex == -1)
            {
                this.clsISUtilities.Paint_Parent_Marker(this.cboDay, true);
            }
            else
            {
                this.clsISUtilities.Paint_Parent_Marker(this.cboDay, false);
            }
        }

        private void cboIRP5Code_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (cboIRP5Code.SelectedIndex > -1)
                {
                    if (pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["IRP5_CODE"].ToString() != cboIRP5Code.SelectedItem.ToString())
                    {
                        pvtEarningSelectedDataView[pvtintEarningSelectedDataViewIndex]["IRP5_CODE"] = cboIRP5Code.SelectedItem.ToString();
                    }
                }
            }
        }

        private void txtHeader1_TextChanged(object sender, EventArgs e)
        {
            //NB DataView Sort is on EARNING_DESC Therefore Refind Position
            for (int intRow = 0; intRow < pvtEarningSelectedDataView.Count; intRow++)
            {
                if (pvtEarningSelectedDataView[intRow]["EARNING_NO"].ToString() == this.pvtintEarningNo.ToString())
                {
                    pvtEarningSelectedDataView[intRow]["EARNING_REPORT_HEADER1"] = this.txtHeader1.Text.Trim();
                  
                    break;
                }
            }
        }

        private void txtHeader2_TextChanged(object sender, EventArgs e)
        {
            //NB DataView Sort is on EARNING_DESC Therefore Refind Position
            for (int intRow = 0; intRow < pvtEarningSelectedDataView.Count; intRow++)
            {
                if (pvtEarningSelectedDataView[intRow]["EARNING_NO"].ToString() == this.pvtintEarningNo.ToString())
                {
                    pvtEarningSelectedDataView[intRow]["EARNING_REPORT_HEADER2"] = this.txtHeader2.Text.Trim();

                    break;
                }
            }
        }

        private void dgvEarningListDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                btnAdd_Click(sender, e);
            }
        }
    }
}
