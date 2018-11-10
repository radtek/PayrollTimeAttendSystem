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
    public partial class frmNormalSickLeave : Form
    {
        clsISUtilities clsISUtilities;
        
        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;
        private DataView pvtLeaveDataView;
        private DataRowView pvtDataRowView;

        DataGridViewCellStyle PayrollLinkDataGridViewCellStyle;

        private int pvtintLeaveNo = -1;

        private int pvtintNormalSickLeaveCategoryDataGridViewRowIndex = -1;
        private int pvtintPayrollTypeDataGridViewRowIndex = -1;

        private int pvtintReturnCode = -1;
        private byte[] pvtbytCompress;

        private string pvstrPayrollType = "";

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnNormalSickLeeaveCategoryDataGridViewLoaded = false;

        public frmNormalSickLeave()
        {
            InitializeComponent();
        }

        private void frmNormalSickLeave_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this,"busNormalSickLeave");

                PayrollLinkDataGridViewCellStyle = new DataGridViewCellStyle();
                PayrollLinkDataGridViewCellStyle.BackColor = Color.Magenta;
                PayrollLinkDataGridViewCellStyle.SelectionBackColor = Color.Magenta;

                this.lblDescription.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.dgvPayrollTypeDataGridView.Rows.Add("Wages");
                this.dgvPayrollTypeDataGridView.Rows.Add("Salaries");

                pvtblnPayrollTypeDataGridViewLoaded = true;

                this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView,0);

                object[] objParm = new object[3];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[2] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                DataRow dtDataRow;

                //Table
                DataTable DataTable = new DataTable("DAYS50");
                DataTable.Columns.Add("DAYS50_DESC", typeof(System.String));
                DataTable.Columns.Add("DAYS50_VALUE", typeof(System.Int16));
                pvtDataSet.Tables.Add(DataTable);

                DataTable = new DataTable("DAYS365");
                DataTable.Columns.Add("DAYS365_VALUE", typeof(System.Int16));
                pvtDataSet.Tables.Add(DataTable);

                DataTable = new DataTable("HOURS24");
                DataTable.Columns.Add("HOURS24_VALUE", typeof(System.Int16));
                pvtDataSet.Tables.Add(DataTable);

                for (int intValue = 0; intValue < 51; intValue++)
                {
                    if (intValue < 24)
                    {
                        dtDataRow = pvtDataSet.Tables["HOURS24"].NewRow();
                        dtDataRow["HOURS24_VALUE"] = intValue;
                        pvtDataSet.Tables["HOURS24"].Rows.Add(dtDataRow);
                    }

                    if (intValue >= 0
                        & intValue < 51)
                    {
                        dtDataRow = pvtDataSet.Tables["DAYS50"].NewRow();
                        dtDataRow["DAYS50_DESC"] = intValue.ToString("00");
                        dtDataRow["DAYS50_VALUE"] = intValue;
                        pvtDataSet.Tables["DAYS50"].Rows.Add(dtDataRow);
                    }
                }

                for (int intValue = 180; intValue < 366; intValue++)
                {

                    dtDataRow = pvtDataSet.Tables["DAYS365"].NewRow();
                    dtDataRow["DAYS365_VALUE"] = intValue;
                    pvtDataSet.Tables["DAYS365"].Rows.Add(dtDataRow);
                }

                DataTable = new DataTable("MINUTE");
                DataTable.Columns.Add("MINUTE_VALUE", typeof(System.Int16));
                pvtDataSet.Tables.Add(DataTable);

                dtDataRow = pvtDataSet.Tables["MINUTE"].NewRow();
                dtDataRow["MINUTE_VALUE"] = 0;
                pvtDataSet.Tables["MINUTE"].Rows.Add(dtDataRow);

                dtDataRow = pvtDataSet.Tables["MINUTE"].NewRow();
                dtDataRow["MINUTE_VALUE"] = 15;
                pvtDataSet.Tables["MINUTE"].Rows.Add(dtDataRow);

                dtDataRow = pvtDataSet.Tables["MINUTE"].NewRow();
                dtDataRow["MINUTE_VALUE"] = 30;
                pvtDataSet.Tables["MINUTE"].Rows.Add(dtDataRow);

                dtDataRow = pvtDataSet.Tables["MINUTE"].NewRow();
                dtDataRow["MINUTE_VALUE"] = 45;
                pvtDataSet.Tables["MINUTE"].Rows.Add(dtDataRow);

                pvtDataSet.AcceptChanges();
                
                clsISUtilities.DataBind_ComboBox_Load(this.cboNormalPaid, this.pvtDataSet.Tables["DAYS50"], "DAYS50_DESC","DAYS50_VALUE");
                clsISUtilities.DataBind_ComboBox_Load(this.cboSickPaid, this.pvtDataSet.Tables["DAYS50"], "DAYS50_DESC", "DAYS50_VALUE");
                clsISUtilities.DataBind_ComboBox_Load(this.cboMaxShifts, this.pvtDataSet.Tables["DAYS365"], "DAYS365_VALUE", "DAYS365_VALUE");
                clsISUtilities.NotDataBound_ComboBox(this.cboMinShiftHours, "Select Minimum Hours for a valid shift.");
                clsISUtilities.NotDataBound_ComboBox(this.cboMinShiftMinutes, "Select Minimum Mibnutes for a valid shift.");
           
                for (int intRow = 0; intRow < this.pvtDataSet.Tables["HOURS24"].Rows.Count; intRow++)
                {
                    this.cboMinShiftHours.Items.Add(Convert.ToInt32(this.pvtDataSet.Tables["HOURS24"].Rows[intRow]["HOURS24_VALUE"]).ToString("00"));
                }

                for (int intRow = 0; intRow < this.pvtDataSet.Tables["MINUTE"].Rows.Count; intRow++)
                {
                    this.cboMinShiftMinutes.Items.Add(Convert.ToInt32(this.pvtDataSet.Tables["MINUTE"].Rows[intRow]["MINUTE_VALUE"]).ToString("00"));
                }

                Load_CurrentForm_Records();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
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

                    case "dgvNormalSickLeaveCategoryDataGridView":

                        pvtintNormalSickLeaveCategoryDataGridViewRowIndex = -1;
                        this.dgvNormalSickLeaveCategoryDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        private void Load_CurrentForm_Records()
        {
            pvtLeaveDataView = null;
            pvtLeaveDataView = new DataView(pvtDataSet.Tables["Leave"],
                "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_TYPE = '" + pvstrPayrollType + "'",
                "LEAVE_SHIFT_DESC",
                DataViewRowState.CurrentRows);
          
            clsISUtilities.DataViewIndex = 0;

            if (clsISUtilities.DataBind_Form_And_DataView_To_Class() == false)
            {
                clsISUtilities.DataBind_DataView_And_Index(this, pvtLeaveDataView, "LEAVE_SHIFT_NO");

                clsISUtilities.DataBind_DataView_To_TextBox(this.txtLeave, "LEAVE_SHIFT_DESC", true, "Enter Normal Leave / Sick Leave Category Description.", true);

                clsISUtilities.DataBind_DataView_To_ComboBox(this.cboNormalPaid, "NORM_PAID_DAYS", true, "Select Normal Leave - Number of Days Paid.", true);
                clsISUtilities.DataBind_DataView_To_ComboBox(this.cboSickPaid, "SICK_PAID_DAYS", true, "Select Sick Leave - Number of Days Paid.", true);

                clsISUtilities.DataBind_DataView_To_ComboBox(this.cboMaxShifts, "MAX_SHIFTS_YEAR", true, "Select Number of shifts in Year.", true);

                clsISUtilities.DataBind_DataView_To_Numeric_TextBox(this.txtNormLeaveAccum, "NORM_PAID_PER_PERIOD",4, false, "", false,0,false);
                clsISUtilities.DataBind_DataView_To_Numeric_TextBox(this.txtSickLeaveAccum, "SICK_PAID_PER_PERIOD", 4, false, "", false, 0,false);
               
                clsISUtilities.DataBind_DataView_To_RadioButton(this.rbnWeekDays, "LEAVE_PAID_ACCUMULATOR_IND", "1");
                clsISUtilities.DataBind_RadioButton_Default(this.rbnWeekDays);
                clsISUtilities.DataBind_DataView_To_RadioButton(this.rbnWeekDaySaturday, "LEAVE_PAID_ACCUMULATOR_IND", "2");
                clsISUtilities.DataBind_DataView_To_RadioButton(this.rbnAllDays, "LEAVE_PAID_ACCUMULATOR_IND", "3");
            }
            else
            {
                //Rebind dataView
                clsISUtilities.Re_DataBind_DataView(pvtLeaveDataView);
            }

            Set_Form_For_Read();

            this.Clear_DataGridView(this.dgvNormalSickLeaveCategoryDataGridView);

            if (pvstrPayrollType == "W")
            {
                this.lblLeaveTypeLockDesc.Text = "Some Records are Locked due to Current Wage Run";
            }
            else
            {
                this.lblLeaveTypeLockDesc.Text = "Some Records are Locked due to Current Salary Run";
            }

            this.grbLeaveLock.Visible = false;

            if (pvtLeaveDataView.Count == 0)
            {
                Clear_Form_Fields();
            }
            else
            {
                this.pvtblnNormalSickLeeaveCategoryDataGridViewLoaded = false;

                for (int intRowCount = 0; intRowCount < pvtLeaveDataView.Count; intRowCount++)
                {
                    this.dgvNormalSickLeaveCategoryDataGridView.Rows.Add("",
                                                                         pvtLeaveDataView[intRowCount]["LEAVE_SHIFT_DESC"].ToString(),
                                                                         intRowCount.ToString());

                    if (pvtLeaveDataView[intRowCount]["PAYROLL_LINK"] != System.DBNull.Value)
                    {
                        this.dgvNormalSickLeaveCategoryDataGridView[0,this.dgvNormalSickLeaveCategoryDataGridView.Rows.Count - 1].Style = this.PayrollLinkDataGridViewCellStyle;

                        this.grbLeaveLock.Visible = true;
                    }
                }

                this.pvtblnNormalSickLeeaveCategoryDataGridViewLoaded = true;

                Load_Current_Normal_Leave_Sick_Leave_Category();
            }
        }
   
        private void Load_Current_Normal_Leave_Sick_Leave_Category()
        {
            int intSelectedIndex = 0;

            if (this.dgvNormalSickLeaveCategoryDataGridView.Rows.Count == 0)
            {
                this.Clear_Form_Fields();
            }
            else
            {
                //Find Row after Edit
                for (int intRow = 0; intRow < this.dgvNormalSickLeaveCategoryDataGridView.Rows.Count; intRow++)
                {
                    if (Convert.ToInt32(pvtLeaveDataView[Convert.ToInt32(dgvNormalSickLeaveCategoryDataGridView[2,intRow].Value)]["LEAVE_SHIFT_NO"]) == this.pvtintLeaveNo)
                    {
                        intSelectedIndex = intRow;
                        break;
                    }
                }

                this.Set_DataGridView_SelectedRowIndex(this.dgvNormalSickLeaveCategoryDataGridView, intSelectedIndex);
            }
        }

        private void Set_Form_For_Edit()
        {
            bool blnNew = true;

            if (this.Text.IndexOf("- Update") > -1)
            {
                blnNew = false;
            }

            this.dgvNormalSickLeaveCategoryDataGridView.Enabled = false;
            this.dgvPayrollTypeDataGridView.Enabled = false;

            this.picLeaveLock.Visible = true;
            this.picPayrollTypeLock.Visible = true;

            clsISUtilities.Set_Form_For_Edit(blnNew);

            this.cboMinShiftHours.Enabled = true;
            this.cboMinShiftMinutes.Enabled = true;
            
            this.btnNew.Enabled = false;
            this.btnUpdate.Enabled = false;

            this.btnDelete.Enabled = true;
            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;
            
            if (this.Text.IndexOf("- New") > -1)
            {
                this.cboMinShiftHours.SelectedIndex = -1;
                this.cboMinShiftMinutes.SelectedIndex = -1;
            }

            if (pvstrPayrollType == "S")
            {
                this.cboMinShiftHours.Enabled = false;
                this.cboMinShiftMinutes.Enabled = false;

                this.cboMaxShifts.Enabled = false;
            }

            this.txtLeave.Focus();
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

            this.dgvNormalSickLeaveCategoryDataGridView.Enabled = true;
            this.dgvPayrollTypeDataGridView.Enabled = true;

            this.picLeaveLock.Visible = false;
            this.picPayrollTypeLock.Visible = false;

            clsISUtilities.Set_Form_For_Read();

            this.btnNew.Enabled = true;
            this.btnUpdate.Enabled = true;

            this.btnDelete.Enabled = false;
            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;
        }

        private void Clear_Form_Fields()
        {
            this.txtNormLeaveAccum.Text = "";
            this.txtSickLeaveAccum.Text = "";

            this.cboMinShiftHours.SelectedIndex = -1;
            this.cboMinShiftMinutes.SelectedIndex = -1;
            
            this.btnUpdate.Enabled = false;
            this.btnDelete.Enabled = false;
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
               
                pvtTempDataSet = null;
                pvtTempDataSet = new DataSet();

                pvtTempDataSet.Tables.Add(this.pvtDataSet.Tables["Leave"].Clone());
                pvtTempDataSet.Tables[0].ImportRow(pvtLeaveDataView[clsISUtilities.DataViewIndex].Row);

                //Compress DataSet
                pvtbytCompress = clsISUtilities.Compress_DataSet(pvtTempDataSet);

                if (this.Text.IndexOf(" - New", 0) > 0)
                {
                    object[] objParm = new object[3];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[2] = pvtbytCompress;

                    pvtintLeaveNo = (int)clsISUtilities.DynamicFunction("Insert_New_Record", objParm,true);
                    
                    pvtLeaveDataView[clsISUtilities.DataViewIndex]["LEAVE_SHIFT_NO"] = pvtintLeaveNo;

                    //Reset From 0 To Null
                    pvtLeaveDataView[clsISUtilities.DataViewIndex]["PAYROLL_LINK"] = System.DBNull.Value;
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
                        this.pvtDataSet.RejectChanges();

                        this.dgvNormalSickLeaveCategoryDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvNormalSickLeaveCategoryDataGridView)].Style = this.PayrollLinkDataGridViewCellStyle;

                        pvtLeaveDataView[clsISUtilities.DataViewIndex]["PAYROLL_LINK"] = 0;

                        DialogResult dlgResult = CustomMessageBox.Show("This Normal Leave and Sick Leave Category is currently being used in a Payroll Run.\r\n\r\nUpdate Cancelled.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                    else
                    {
                        this.dgvNormalSickLeaveCategoryDataGridView[1, this.Get_DataGridView_SelectedRowIndex(dgvNormalSickLeaveCategoryDataGridView)].Value = this.txtLeave.Text.Trim();
                    }
                }

                this.pvtDataSet.Tables["Leave"].AcceptChanges();

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

        public void btnNew_Click(object sender, System.EventArgs e)
        {
            this.Text += " - New";

            pvtDataRowView = this.pvtLeaveDataView.AddNew();

            pvtDataRowView.BeginEdit();

            pvtDataRowView["COMPANY_NO"] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
            pvtDataRowView["PAY_CATEGORY_TYPE"] = pvstrPayrollType;
            pvtDataRowView["LEAVE_SHIFT_NO"] = 0;
            pvtDataRowView["LEAVE_SHIFT_DESC"] = "";
            pvtDataRowView["LEAVE_SHIFT_DEL_IND"] = "Y";

            pvtDataRowView.EndEdit();

            this.clsISUtilities.DataViewIndex = 0;

            Set_Form_For_Edit();

            this.txtLeave.Focus();
        }

        public void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.pvtDataSet.RejectChanges();

            Set_Form_For_Read();

            Load_Current_Normal_Leave_Sick_Leave_Category();
        }

        public void btnUpdate_Click(object sender, System.EventArgs e)
        {
            this.Text += " - Update";

            Set_Form_For_Edit();

            this.txtLeave.Focus();
        }

        public void btnDelete_Click(object sender, System.EventArgs e)
        {
            try
            {
                DialogResult dlgResult = CustomMessageBox.Show("Delete Normal Leave / Sick Leave Category '" + pvtLeaveDataView[clsISUtilities.DataViewIndex]["LEAVE_SHIFT_DESC"].ToString() + "'",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dlgResult == DialogResult.Yes)
                {
                    object[] objParm = new object[4];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[2] = pvtintLeaveNo;
                    objParm[3] = pvstrPayrollType;

                    clsISUtilities.DynamicFunction("Delete_Record", objParm,true);

                    pvtLeaveDataView[clsISUtilities.DataViewIndex].Delete();

                    this.pvtDataSet.Tables["Leave"].AcceptChanges();

                    pvtintLeaveNo = -1;

                    Load_CurrentForm_Records();
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }
        
        private void cboNormalPaid_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (cboNormalPaid.SelectedIndex > -1)
                {
                    cboMaxShifts_SelectedIndexChanged(sender, e);
                }
            }
        }

        private void cboSickPaid_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (cboSickPaid.SelectedIndex > -1)
                {
                    cboMaxShifts_SelectedIndexChanged(sender, e);
                }
            }
        }

        private void cboMaxShifts_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (pvstrPayrollType == "W")
                {
                    //Not Zero
                    if (cboMaxShifts.SelectedIndex > 0)
                    {
                        if (cboNormalPaid.SelectedIndex > -1)
                        {
                            double dblNorm = Convert.ToDouble(cboNormalPaid.SelectedItem.ToString()) / Convert.ToDouble(cboMaxShifts.SelectedItem.ToString());

                            this.txtNormLeaveAccum.Text = dblNorm.ToString("0.0000");

                            pvtLeaveDataView[clsISUtilities.DataViewIndex]["NORM_PAID_PER_PERIOD"] = Convert.ToDouble(this.txtNormLeaveAccum.Text);
                        }

                        if (this.cboSickPaid.SelectedIndex > -1)
                        {
                            double dblSick = Convert.ToDouble(cboSickPaid.SelectedItem.ToString()) / Convert.ToDouble(cboMaxShifts.SelectedItem.ToString());

                            this.txtSickLeaveAccum.Text = dblSick.ToString("0.0000");
                            pvtLeaveDataView[clsISUtilities.DataViewIndex]["SICK_PAID_PER_PERIOD"] = Convert.ToDouble(this.txtSickLeaveAccum.Text);
                        }
                    }
                }
                else
                {
                    if (cboNormalPaid.SelectedIndex > -1)
                    {
                        double dblNorm = Convert.ToDouble(cboNormalPaid.SelectedItem.ToString()) / 12;

                        this.txtNormLeaveAccum.Text = dblNorm.ToString("0.0000");
                        pvtLeaveDataView[clsISUtilities.DataViewIndex]["NORM_PAID_PER_PERIOD"] = Convert.ToDouble(this.txtNormLeaveAccum.Text);
                    }

                    if (this.cboSickPaid.SelectedIndex > -1)
                    {
                        double dblSick = Convert.ToDouble(cboSickPaid.SelectedItem.ToString()) / 12;

                        this.txtSickLeaveAccum.Text = dblSick.ToString("0.0000");
                        pvtLeaveDataView[clsISUtilities.DataViewIndex]["SICK_PAID_PER_PERIOD"] = Convert.ToDouble(this.txtSickLeaveAccum.Text);
                    }
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MinimumShift_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (cboMinShiftHours.SelectedIndex > -1
                    & cboMinShiftMinutes.SelectedIndex > -1)
                {
                    pvtLeaveDataView[clsISUtilities.DataViewIndex]["MIN_VALID_SHIFT_MINUTES"] = (Convert.ToInt32(cboMinShiftHours.SelectedItem.ToString()) * 60) + Convert.ToInt32(cboMinShiftMinutes.SelectedItem.ToString()); 
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

                    if (this.dgvPayrollTypeDataGridView[0, e.RowIndex].Value.ToString().Substring(0, 1) == "W")
                    {
                        pvstrPayrollType = "W";

                        this.grbShiftDetails.Visible = true;

                        this.lblNormalDesc.Text = "Normal Leave Accumulated / Day";
                        this.lblSickDesc.Text = "Sick Leave Accumulated / Day";
                    }
                    else
                    {
                        pvstrPayrollType = "S";

                        this.grbShiftDetails.Visible = false;

                        this.lblNormalDesc.Text = "Normal Leave Accumulated / Month";
                        this.lblSickDesc.Text = "Sick Leave Accumulated / Month";
                    }

                    if (pvtDataSet != null)
                    {
                        Load_CurrentForm_Records();
                    }
                }
            }
        }

        private void dgvNormalSickLeaveCategoryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnNormalSickLeeaveCategoryDataGridViewLoaded == true)
            {
                if (pvtintNormalSickLeaveCategoryDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintNormalSickLeaveCategoryDataGridViewRowIndex = e.RowIndex;

                    clsISUtilities.DataViewIndex = Convert.ToInt32(this.dgvNormalSickLeaveCategoryDataGridView[2, e.RowIndex].Value);

                    clsISUtilities.DataBind_DataView_Record_Show();

                    pvtintLeaveNo = Convert.ToInt32(pvtLeaveDataView[clsISUtilities.DataViewIndex]["LEAVE_SHIFT_NO"]);

                    if (pvtLeaveDataView[clsISUtilities.DataViewIndex]["LEAVE_SHIFT_DEL_IND"].ToString() == "Y")
                    {
                        this.btnDelete.Enabled = true;
                    }
                    else
                    {
                        this.btnDelete.Enabled = false;
                    }

                    if (pvtLeaveDataView[clsISUtilities.DataViewIndex]["PAYROLL_LINK"] != System.DBNull.Value)
                    {
                        this.btnUpdate.Enabled = false;
                    }
                    else
                    {
                        this.btnUpdate.Enabled = true;
                    }

                    if (pvstrPayrollType == "W")
                    {
                        this.cboMinShiftHours.SelectedIndex = Convert.ToInt32(Convert.ToInt32(pvtLeaveDataView[clsISUtilities.DataViewIndex]["MIN_VALID_SHIFT_MINUTES"]) / 60);

                        this.cboMinShiftMinutes.SelectedIndex = -1;

                        for (int intRow = 0; intRow < this.cboMinShiftMinutes.Items.Count; intRow++)
                        {
                            if (Convert.ToInt32(this.cboMinShiftMinutes.Items[intRow]) == Convert.ToInt32(Convert.ToInt32(pvtLeaveDataView[clsISUtilities.DataViewIndex]["MIN_VALID_SHIFT_MINUTES"]) % 60))
                            {
                                this.cboMinShiftMinutes.SelectedIndex = intRow;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
