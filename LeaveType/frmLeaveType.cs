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
    public partial class frmLeaveType : Form
    {
        clsISUtilities clsISUtilities;
       
        public DataSet pvtDataSet;
        public DataView pvtLeaveTypeDataView;
       
        private int pvtintLeaveTypeNo = -1;

        private int pvtintLeaveTypeDataGridViewRowIndex = -1;
        private int pvtintPayrollTypeDataGridViewRowIndex = -1;

        DataGridViewCellStyle PayrollLinkDataGridViewCellStyle;

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnLeaveTypeDataGridViewLoaded = false;
                
        public frmLeaveType()
        {
            InitializeComponent();
        }

        private void frmLeaveType_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this,"busLeaveType");

                this.lblLeaveType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                PayrollLinkDataGridViewCellStyle = new DataGridViewCellStyle();
                PayrollLinkDataGridViewCellStyle.BackColor = Color.Magenta;
                PayrollLinkDataGridViewCellStyle.SelectionBackColor = Color.Magenta;

                this.dgvPayrollTypeDataGridView.Rows.Add("Wages");
                this.dgvPayrollTypeDataGridView.Rows.Add("Salaries");

                this.pvtblnPayrollTypeDataGridViewLoaded = true;

                this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView, 0);

                object[] objParm = new object[1];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
               
                byte[] bytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);
                
                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);

                DataRow dtDataRow;

                //Table
                DataTable DataTable = new DataTable("PERCENT_PAID");
                DataTable.Columns.Add("PERCENT_PAID_VALUE", typeof(System.Int16));
                pvtDataSet.Tables.Add(DataTable);

                for (int intValue = 1; intValue < 101; intValue++)
                {
                    dtDataRow = pvtDataSet.Tables["PERCENT_PAID"].NewRow();
                    dtDataRow["PERCENT_PAID_VALUE"] = intValue;
                    pvtDataSet.Tables["PERCENT_PAID"].Rows.Add(dtDataRow);
                }

                pvtDataSet.AcceptChanges();
               
                clsISUtilities.DataBind_ComboBox_Load(this.cboPercentage,pvtDataSet.Tables["PERCENT_PAID"],"PERCENT_PAID_VALUE","PERCENT_PAID_VALUE");

                Load_CurrentForm_Records();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }
       
        private void Load_CurrentForm_Records()
        {
            pvtLeaveTypeDataView = null;
            pvtLeaveTypeDataView = new DataView(pvtDataSet.Tables["LeaveType"],
                    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_TYPE = '" + this.dgvPayrollTypeDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0,1) + "'",
                    "EARNING_DESC",
                    DataViewRowState.CurrentRows);
        
            clsISUtilities.DataViewIndex = 0;

            if (clsISUtilities.DataBind_Form_And_DataView_To_Class() == false)
            {
                clsISUtilities.DataBind_DataView_And_Index(this, pvtLeaveTypeDataView, "EARNING_NO");

                clsISUtilities.DataBind_DataView_To_TextBox(this.txtLeaveType, "EARNING_DESC", true, "Enter Leave Type Description.", true);

                clsISUtilities.DataBind_DataView_To_ComboBox(this.cboPercentage,"LEAVE_PERCENTAGE",false, "",false);
                clsISUtilities.DataBind_Control_Required_If_Enabled(this.cboPercentage, "Select Percentage Paid.");

                clsISUtilities.DataBind_DataView_To_TextBox(this.txtHeader1, "EARNING_REPORT_HEADER1", true, "Enter Spreadsheet Report Header - Line 1", true);
                clsISUtilities.DataBind_DataView_To_TextBox(this.txtHeader2, "EARNING_REPORT_HEADER2", false, "", true);
            }
            else
            {
                //Rebind dataView
                clsISUtilities.Re_DataBind_DataView(pvtLeaveTypeDataView);
            }

            this.Set_Form_For_Read();

            this.Clear_DataGridView(this.dgvLeaveTypeDataGridView);

            this.pvtblnLeaveTypeDataGridViewLoaded = false;
            int intLeaveTypeRow = 0;

            if (this.dgvPayrollTypeDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0,1) == "W")
            {
                this.lblLeaveTypeLockDesc.Text = "Some Leave Type Records are Locked due to Current Wage Run";
            }
            else
            {
                this.lblLeaveTypeLockDesc.Text = "Some Leave Type Records are Locked due to Current Salary Run";
            }

            this.grbLeaveLock.Visible = false;

            for (int intRow = 0; intRow < pvtLeaveTypeDataView.Count; intRow++)
            {
                this.dgvLeaveTypeDataGridView.Rows.Add("",
                                                       pvtLeaveTypeDataView[intRow]["EARNING_DESC"].ToString(),
                                                       intRow.ToString());

                if (Convert.ToInt32(pvtLeaveTypeDataView[intRow]["EARNING_NO"]) == pvtintLeaveTypeNo)
                {
                    intLeaveTypeRow = intRow;
                }

                if (pvtLeaveTypeDataView[intRow]["PAYROLL_LINK"] != System.DBNull.Value)
                {
                    this.dgvLeaveTypeDataGridView[0,this.dgvLeaveTypeDataGridView.Rows.Count - 1].Style = this.PayrollLinkDataGridViewCellStyle;
                    this.grbLeaveLock.Visible = true;
                }
            }

            this.pvtblnLeaveTypeDataGridViewLoaded = true;

            if (this.dgvLeaveTypeDataGridView.Rows.Count > 0)
            {
                this.btnUpdate.Enabled = true;
                this.btnDelete.Enabled = true;

                this.Set_DataGridView_SelectedRowIndex(dgvLeaveTypeDataGridView, intLeaveTypeRow);
            }
            else
            {
                this.Clear_Form_Fields();
            }
        }

        private void Set_Form_For_Edit()
        {
            bool blnNew = false;
            
            if (this.Text.EndsWith(" - New") == true)
            {
                blnNew = true;

                this.rbnPaid.Checked = true;

                cboPercentage.Enabled = true;
            }
            else
            {
                if (this.rbnPaid.Checked == true)
                {
                    this.cboPercentage.Enabled = true;
                }
            }

            clsISUtilities.Set_Form_For_Edit(blnNew);

            if (this.Text.EndsWith(" - New") == true)
            {
                //Set So That btnUpdate gets Enabled
                pvtLeaveTypeDataView[this.clsISUtilities.DataViewIndex]["PAYROLL_LINK"] = System.DBNull.Value;
            }
             
            this.dgvLeaveTypeDataGridView.Enabled = false;
            this.dgvPayrollTypeDataGridView.Enabled = false;

            this.picLeaveTypeLock.Visible = true;
            this.picPayrollTypeLock.Visible = true;

            this.rbnUnPaid.Enabled = true;
            this.rbnPaid.Enabled = true;
            
            this.btnNew.Enabled = false;
            this.btnUpdate.Enabled = false;
            this.btnDelete.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;
            
            this.txtLeaveType.Focus();
        }

        private void Clear_Form_Fields()
        {
            this.txtLeaveType.Text = "";

            this.cboPercentage.SelectedIndex = -1;

            this.txtHeader1.Text = "";
            this.txtHeader2.Text = "";

            this.rbnUnPaid.Checked = true;
        }

        public void btnNew_Click(object sender, System.EventArgs e)
        {
            this.Text += " - New";
            
            DataRowView pvtDataRowView = this.pvtLeaveTypeDataView.AddNew();

            pvtDataRowView.BeginEdit();

            pvtDataRowView["COMPANY_NO"] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo").ToString());
            pvtDataRowView["PAY_CATEGORY_TYPE"] = this.dgvPayrollTypeDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0,1);
            pvtDataRowView["EARNING_NO"] = -1;
            pvtDataRowView["TIE_BREAKER"] = 1;
            pvtDataRowView["EARNING_DESC"] = "";
            pvtDataRowView["EARNING_DEL_IND"] = "Y";

            pvtDataRowView.EndEdit();

            pvtintLeaveTypeNo = -1;

            clsISUtilities.DataViewIndex = 0;
           
            this.Set_Form_For_Edit();
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

            this.dgvLeaveTypeDataGridView.Enabled = true;
            this.dgvPayrollTypeDataGridView.Enabled = true;

            this.picLeaveTypeLock.Visible = false;
            this.picPayrollTypeLock.Visible = false;

            this.rbnUnPaid.Enabled = false;
            this.rbnPaid.Enabled = false;

            this.btnNew.Enabled = true;

            this.btnUpdate.Enabled = false;
            this.btnDelete.Enabled = false;
            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        public void btnUpdate_Click(object sender, System.EventArgs e)
        {
            this.Text = this.Text + " - Update";

            Set_Form_For_Edit();
        }

        public void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.pvtDataSet.RejectChanges();

            Set_Form_For_Read();

            if (this.dgvLeaveTypeDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvLeaveTypeDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvLeaveTypeDataGridView));
            }
            else
            {
                Clear_Form_Fields();
            }
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
          
                DataSet TempDataSet = new DataSet();

                TempDataSet.Tables.Add(this.pvtDataSet.Tables["LeaveType"].Clone());
                TempDataSet.Tables["LeaveType"].ImportRow(this.pvtLeaveTypeDataView[clsISUtilities.DataViewIndex].Row);

                //Compress DataSet
                byte[] bytCompress = clsISUtilities.Compress_DataSet(TempDataSet);

                if (this.Text.EndsWith(" - New") == true)
                {
                    object[] objParm = new object[3];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[2] = bytCompress;

                    pvtintLeaveTypeNo = (int)clsISUtilities.DynamicFunction("Insert_New_Record", objParm,true);
                    
                    pvtLeaveTypeDataView[clsISUtilities.DataViewIndex]["EARNING_NO"] = pvtintLeaveTypeNo;
                }
                else
                {
                    object[] objParm = new object[3];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[2] = bytCompress;

                    intReturnCode = (int)clsISUtilities.DynamicFunction("Update_Record", objParm,true);
                   
                    if (intReturnCode == 9999)
                    {
                        this.pvtDataSet.RejectChanges();

                        pvtLeaveTypeDataView[this.clsISUtilities.DataViewIndex]["PAYROLL_LINK"] = 0;

                        this.dgvLeaveTypeDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvLeaveTypeDataGridView)].Style = this.PayrollLinkDataGridViewCellStyle;

                        CustomMessageBox.Show("This Leave Type is currently being used in a Payroll Run.\r\n\r\nUpdate Cancelled.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                    else
                    {
                        this.dgvLeaveTypeDataGridView[1, this.Get_DataGridView_SelectedRowIndex(dgvLeaveTypeDataGridView)].Value = this.txtLeaveType.Text.Trim();
                    }
                }

                this.pvtDataSet.AcceptChanges();

                if (this.clsISUtilities.pubintReloadSpreadsheet == true)
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

        public void btnDelete_Click(object sender, System.EventArgs e)
        {
            try
            {
                DialogResult dlgResult = CustomMessageBox.Show("Are you sure you want to Delete '" + pvtLeaveTypeDataView[clsISUtilities.DataViewIndex]["EARNING_DESC"].ToString() + "' ?",
                this.Text,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

                if (dlgResult == DialogResult.Yes)
                {
                    object[] objParm = new object[4];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[2] = this.dgvPayrollTypeDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0,1);
                    objParm[3] = pvtintLeaveTypeNo;

                    clsISUtilities.DynamicFunction("Delete_Record", objParm,true);

                    pvtintLeaveTypeNo = -1;

                    this.pvtLeaveTypeDataView[clsISUtilities.DataViewIndex].Delete();

                    pvtDataSet.AcceptChanges();

                    Load_CurrentForm_Records();
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void txtLeaveType_Leave(object sender, System.EventArgs e)
        {
            if (this.Text.IndexOf(" - New", 0) > 0)
            {
                if (this.txtHeader1.Text.Trim() == "")
                {
                    if (this.txtLeaveType.Text.Trim().IndexOf(" ", 0) > -1)
                    {
                        if (this.txtLeaveType.Text.Trim().Substring(0, txtLeaveType.Text.Trim().IndexOf(" ", 0)).Length > 10)
                        {
                            this.txtHeader1.Text = this.txtLeaveType.Text.Trim().Substring(0, txtLeaveType.Text.Trim().IndexOf(" ", 0)).Substring(0, 10);
                        }
                        else
                        {
                            this.txtHeader1.Text = this.txtLeaveType.Text.Trim().Substring(0, txtLeaveType.Text.Trim().IndexOf(" ", 0));
                        }

                        if (this.txtLeaveType.Text.Trim().Substring(txtLeaveType.Text.Trim().IndexOf(" ", 0) + 1).Length > 10)
                        {
                            this.txtHeader2.Text = this.txtLeaveType.Text.Trim().Substring(txtLeaveType.Text.Trim().IndexOf(" ", 0) + 1).Substring(0, 10);
                        }
                        else
                        {
                            this.txtHeader2.Text = this.txtLeaveType.Text.Trim().Substring(txtLeaveType.Text.Trim().IndexOf(" ", 0) + 1);
                        }
                    }
                    else
                    {
                        if (this.txtLeaveType.Text.Trim().Length > 10)
                        {
                            this.txtHeader1.Text = this.txtLeaveType.Text.Trim().Substring(0, 10);
                        }
                        else
                        {
                            this.txtHeader1.Text = this.txtLeaveType.Text.Trim();
                        }
                    }
                }
            }
        }

        private void rbnPaid_Click(object sender, System.EventArgs e)
        {
            this.cboPercentage.Enabled = true;
        }

        private void rbnUnPaid_Click(object sender, System.EventArgs e)
        {
            this.cboPercentage.Enabled = false;
            this.cboPercentage.SelectedIndex = -1;

            if (this.btnSave.Enabled == true)
            {
                pvtLeaveTypeDataView[this.clsISUtilities.DataViewIndex]["LEAVE_PERCENTAGE"] = 0;
            }
        }

        private void dgvPayrollTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPayrollTypeDataGridViewLoaded == true)
            {
                if (pvtintPayrollTypeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayrollTypeDataGridViewRowIndex = e.RowIndex;

                    if (pvtDataSet != null)
                    {
                        Load_CurrentForm_Records();
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

                    case "dgvLeaveTypeDataGridView":

                        pvtintLeaveTypeDataGridViewRowIndex = -1;
                        this.dgvLeaveTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        private void dgvLeaveTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnLeaveTypeDataGridViewLoaded == true)
            {
                if (pvtintLeaveTypeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintLeaveTypeDataGridViewRowIndex = e.RowIndex;

                    clsISUtilities.DataViewIndex = Convert.ToInt32(this.dgvLeaveTypeDataGridView[2, e.RowIndex].Value);
                    pvtintLeaveTypeNo = Convert.ToInt32(this.pvtLeaveTypeDataView[this.clsISUtilities.DataViewIndex]["EARNING_NO"]);

                    clsISUtilities.DataBind_DataView_Record_Show();

                    if (Convert.ToDouble(pvtLeaveTypeDataView[this.clsISUtilities.DataViewIndex]["LEAVE_PERCENTAGE"]) == 0)
                    {
                        this.rbnUnPaid.Checked = true;
                    }
                    else
                    {
                        this.rbnPaid.Checked = true;
                    }

                    if (pvtLeaveTypeDataView[this.clsISUtilities.DataViewIndex]["EARNING_DEL_IND"].ToString() == "Y"
                        & Convert.ToInt32(pvtLeaveTypeDataView[this.clsISUtilities.DataViewIndex]["EARNING_NO"]) != 200
                        & Convert.ToInt32(pvtLeaveTypeDataView[this.clsISUtilities.DataViewIndex]["EARNING_NO"]) != 201)
                    {
                        this.btnDelete.Enabled = true;
                    }
                    else
                    {
                        this.btnDelete.Enabled = false;
                    }

                    if (Convert.ToInt32(pvtLeaveTypeDataView[this.clsISUtilities.DataViewIndex]["EARNING_NO"]) != 200
                        & Convert.ToInt32(pvtLeaveTypeDataView[this.clsISUtilities.DataViewIndex]["EARNING_NO"]) != 201)
                    {
                        if (pvtLeaveTypeDataView[this.clsISUtilities.DataViewIndex]["PAYROLL_LINK"] == System.DBNull.Value)
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
                }
            }
        }
    }
}
