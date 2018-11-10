using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InteractPayroll
{
    public partial class frmConvertEmployeeType : Form
    {
        clsISUtilities clsISUtilities;

        private DataSet pvtDataSet;

        private string pvtstrPayrollType = "";
        private string pvtstrCurrentPayrollType = "";
        private int pvtintEmployeeNo = -1;
        private int pvtintEmployeeDataTableRowIndex = -1;

        private DataView EmployeeCurrentPayCategoryDataView = null;
        private DataView PayCategoryDataView = null;
        private DataView LeaveShiftDataView = null;

        private int pvtintEmployeesDataGridViewRowIndex = -1;
        private int pvtintPayrollTypeDataGridViewRowIndex = -1;

        private bool pvtblnEmployeesDataGridViewLoaded = false;
        private bool pvtblnPayrollTypeDataGridViewLoaded = false;

        public frmConvertEmployeeType()
        {
            InitializeComponent();

            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 118;

                this.dgvPayCategoryDataGridView.Height += 76;
                this.dgvChosenPayCategoryDataGridView.Height += 76;

                this.btnPayCategoryAdd.Top += 38;
                this.btnPayCategoryRemove.Top += 38;
                
                this.lblNormalLeave.Top += 76;
                this.lblNormalLeaveSelected.Top += 76;

                dgvLeaveShiftDataGridView.Top += 76;
                dgvLeaveShiftSelectedDataGridView.Top += 76;

                this.btnLeaveAdd.Top += 95;
                this.btnLeaveRemove.Top += 95;
                
                dgvLeaveShiftDataGridView.Height += 38;
                dgvLeaveShiftSelectedDataGridView.Height += 38;
            }
        }

        private void frmConvertEmployeeType_Load(object sender, EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this, "busConvertEmployeeType");
                
                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblCostCentreSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayCategoryDesc.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayCategorySelectDesc.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblNormalLeave.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblNormalLeaveSelected.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                pvtDataSet = new DataSet();

                object[] objParm = new object[1];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));

                byte[] bytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);

                Load_CurrentForm_Records();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void Load_CurrentForm_Records()
        {
            for (int intRow = 0; intRow < pvtDataSet.Tables["Employee"].Rows.Count; intRow++)
            {
                this.dgvEmployeesDataGridView.Rows.Add(pvtDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_CODE"].ToString(),
                                                       pvtDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                       pvtDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NAME"].ToString(),
                                                       pvtDataSet.Tables["Employee"].Rows[intRow]["PAY_CATEGORY_TYPE_DESC"].ToString(),
                                                       intRow.ToString());
            }

            this.pvtblnEmployeesDataGridViewLoaded = true;

            if (this.dgvEmployeesDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeesDataGridView, 0);
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
                case "dgvEmployeesDataGridView":

                    pvtintEmployeesDataGridViewRowIndex = -1;

                    break;

                case "dgvPayrollTypeDataGridView":

                    pvtintPayrollTypeDataGridViewRowIndex = -1;

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
                    case "dgvEmployeesDataGridView":

                        this.dgvEmployeesDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPayrollTypeDataGridView":

                        this.dgvPayrollTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;


                    default:

                        CustomMessageBox.Show("No Entry for " + myDataGridView.Name + " in Set_DataGridView_SelectedRowIndex", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            myDataGridView.RowCount = 0;

            if (myDataGridView.SortedColumn != null)
            {
                myDataGridView.SortedColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            myDataGridView.Refresh();
        }

        private void dgvEmployeesDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnEmployeesDataGridViewLoaded == true)
            {
                if (pvtintEmployeesDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintEmployeesDataGridViewRowIndex = e.RowIndex;

                    pvtintEmployeeDataTableRowIndex = Convert.ToInt32(this.dgvEmployeesDataGridView[4, e.RowIndex].Value);

                    Clear_DataGridView(dgvCurrentPayCategoryDataGridView);
                    Clear_DataGridView(dgvPayrollTypeDataGridView);

                    pvtintEmployeeNo = Convert.ToInt32(pvtDataSet.Tables["Employee"].Rows[pvtintEmployeeDataTableRowIndex]["EMPLOYEE_NO"]);

                    EmployeeCurrentPayCategoryDataView = new DataView(pvtDataSet.Tables["CurrentPayCategory"],
                    "EMPLOYEE_NO = " + pvtDataSet.Tables["Employee"].Rows[pvtintEmployeeDataTableRowIndex]["EMPLOYEE_NO"].ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtDataSet.Tables["Employee"].Rows[pvtintEmployeeDataTableRowIndex]["PAY_CATEGORY_TYPE"].ToString() + "'",
                    "",
                    DataViewRowState.CurrentRows);

                    if (EmployeeCurrentPayCategoryDataView.Count > 0)
                    {
                        for (int intRow = 0; intRow < EmployeeCurrentPayCategoryDataView.Count; intRow++)
                        {
                            this.dgvCurrentPayCategoryDataGridView.Rows.Add(EmployeeCurrentPayCategoryDataView[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                                            intRow.ToString());
                        }
                    }

                    pvtblnPayrollTypeDataGridViewLoaded = false;

                    if (pvtDataSet.Tables["Employee"].Rows[pvtintEmployeeDataTableRowIndex]["PAY_CATEGORY_TYPE"].ToString() == "S")
                    {
                        pvtstrCurrentPayrollType = "S";

                        this.dgvPayrollTypeDataGridView.Rows.Add("Wages");
                        this.dgvPayrollTypeDataGridView.Rows.Add("Time Attendance");
                    }
                    else
                    {
                        if (pvtDataSet.Tables["Employee"].Rows[pvtintEmployeeDataTableRowIndex]["PAY_CATEGORY_TYPE"].ToString() == "W")
                        {
                            pvtstrCurrentPayrollType = "W";

                            this.dgvPayrollTypeDataGridView.Rows.Add("Salaries");
                            this.dgvPayrollTypeDataGridView.Rows.Add("Time Attendance");

                        }
                        else
                        {
                            if (pvtDataSet.Tables["Employee"].Rows[pvtintEmployeeDataTableRowIndex]["PAY_CATEGORY_TYPE"].ToString() == "T")
                            {
                                pvtstrCurrentPayrollType = "T";

                                this.dgvPayrollTypeDataGridView.Rows.Add("Wages");
                                this.dgvPayrollTypeDataGridView.Rows.Add("Salaries");
                            }
                        }
                    }

                    pvtblnPayrollTypeDataGridViewLoaded = true;

                    this.Set_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView, 0);
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

                    if (dgvPayrollTypeDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Substring(0, 1) == "S")
                    {
                        this.dgvChosenPayCategoryDataGridView.Columns[1].HeaderText = "Monthly Payment";
                    }
                    else
                    {
                        if (dgvPayrollTypeDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Substring(0, 1) == "W")
                        {
                            this.dgvChosenPayCategoryDataGridView.Columns[1].HeaderText = "Hourly Rate";
                        }
                    }

                    pvtstrPayrollType = dgvPayrollTypeDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Substring(0, 1);

                    this.Clear_DataGridView(dgvPayCategoryDataGridView);
                    this.Clear_DataGridView(dgvChosenPayCategoryDataGridView);

                    PayCategoryDataView = null;
                    PayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"],
                    "PAY_CATEGORY_TYPE = '" + dgvPayrollTypeDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Substring(0, 1) + "'",
                    "",
                    DataViewRowState.CurrentRows);

                    if (PayCategoryDataView.Count > 0)
                    {
                        for (int intRow = 0; intRow < PayCategoryDataView.Count; intRow++)
                        {
                            this.dgvPayCategoryDataGridView.Rows.Add(PayCategoryDataView[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                                     PayCategoryDataView[intRow]["PAY_CATEGORY_NO"].ToString());
                        }
                    }

                    this.Clear_DataGridView(this.dgvLeaveShiftDataGridView);
                    this.Clear_DataGridView(this.dgvLeaveShiftSelectedDataGridView);

                    LeaveShiftDataView = null;
                    LeaveShiftDataView = new DataView(pvtDataSet.Tables["LeaveShift"],
                    "PAY_CATEGORY_TYPE = '" + dgvPayrollTypeDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Substring(0, 1) + "'",
                    "",
                    DataViewRowState.CurrentRows);

                    if (LeaveShiftDataView.Count > 0)
                    {
                        for (int intRow = 0; intRow < LeaveShiftDataView.Count; intRow++)
                        {
                            this.dgvLeaveShiftDataGridView.Rows.Add(LeaveShiftDataView[intRow]["LEAVE_SHIFT_DESC"].ToString(),
                                                                    LeaveShiftDataView[intRow]["LEAVE_SHIFT_NO"].ToString());
                        }
                    }
                 
                    if (PayCategoryDataView.Count > 0
                    && LeaveShiftDataView.Count > 0)
                    {
                        this.btnUpdate.Enabled = true;
                    }
                    else
                    {
                        this.btnUpdate.Enabled = false;
                    }
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            this.Text += " - Update";

            this.btnUpdate.Enabled = false;
            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.dgvChosenPayCategoryDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dgvChosenPayCategoryDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;

            this.dgvEmployeesDataGridView.Enabled = false;
            this.dgvCurrentPayCategoryDataGridView.Enabled = false;
            this.dgvPayrollTypeDataGridView.Enabled = false;

            this.btnPayCategoryAdd.Enabled = true;
            this.btnPayCategoryRemove.Enabled = true;
            this.btnLeaveAdd.Enabled = true;
            this.btnLeaveRemove.Enabled = true;
            
            this.picCostCentreLock.Visible = true;
            this.picEmployeeLock.Visible = true;
            this.picPayrollType.Visible = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Text = this.Text.Substring(0, this.Text.IndexOf("- Update") - 1);

            this.btnUpdate.Enabled = true;
            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.dgvChosenPayCategoryDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvChosenPayCategoryDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

            this.dgvEmployeesDataGridView.Enabled = true;
            this.dgvCurrentPayCategoryDataGridView.Enabled = true;
            this.dgvPayrollTypeDataGridView.Enabled = true;

            this.btnPayCategoryAdd.Enabled = false;
            this.btnPayCategoryRemove.Enabled = false;
            this.btnLeaveAdd.Enabled = false;
            this.btnLeaveRemove.Enabled = false;

            this.picCostCentreLock.Visible = false;
            this.picEmployeeLock.Visible = false;
            this.picPayrollType.Visible = false;

            if (this.dgvEmployeesDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeesDataGridView, Get_DataGridView_SelectedRowIndex(dgvEmployeesDataGridView));
            }

        }

        private void btnPayCategoryAdd_Click(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
                {
                    bool blnValue = true;

                    if (this.dgvChosenPayCategoryDataGridView.Rows.Count > 0)
                    {
                        blnValue = false;
                    }

                    this.dgvChosenPayCategoryDataGridView.Rows.Add(this.dgvPayCategoryDataGridView[0, dgvPayCategoryDataGridView.CurrentRow.Index].Value,
                                                                   "0.00",
                                                                   blnValue,
                                                                   this.dgvPayCategoryDataGridView[1, dgvPayCategoryDataGridView.CurrentRow.Index].Value);


                    DataGridViewRow myDataGridViewRow = this.dgvPayCategoryDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView)];

                    this.dgvPayCategoryDataGridView.Rows.Remove(myDataGridViewRow);

                    this.dgvChosenPayCategoryDataGridView.CurrentCell = this.dgvChosenPayCategoryDataGridView[0, this.dgvChosenPayCategoryDataGridView.Rows.Count - 1];
                }
            }
        }

        private void btnPayCategoryRemove_Click(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (this.dgvChosenPayCategoryDataGridView.Rows.Count > 0)
                {
                    this.dgvPayCategoryDataGridView.Rows.Add(this.dgvChosenPayCategoryDataGridView[0, dgvChosenPayCategoryDataGridView.CurrentRow.Index].Value,
                                                             this.dgvChosenPayCategoryDataGridView[3, dgvChosenPayCategoryDataGridView.CurrentRow.Index].Value);
                    
                    DataGridViewRow myDataGridViewRow = this.dgvChosenPayCategoryDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvChosenPayCategoryDataGridView)];

                    this.dgvChosenPayCategoryDataGridView.Rows.Remove(myDataGridViewRow);

                    this.dgvPayCategoryDataGridView.CurrentCell = this.dgvPayCategoryDataGridView[0, this.dgvPayCategoryDataGridView.Rows.Count - 1];
                }
            }
        }

        private void dgvPayCategoryDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnPayCategoryAdd_Click(sender, e);
        }
        
        private void btnLeaveAdd_Click(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (this.dgvLeaveShiftDataGridView.Rows.Count > 0)
                {
                    DataGridViewRow myDataGridViewRow = this.dgvLeaveShiftDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvLeaveShiftDataGridView)];

                    this.dgvLeaveShiftDataGridView.Rows.Remove(myDataGridViewRow);

                    this.dgvLeaveShiftSelectedDataGridView.Rows.Add(myDataGridViewRow);

                    this.dgvLeaveShiftSelectedDataGridView.CurrentCell = this.dgvLeaveShiftSelectedDataGridView[0, this.dgvLeaveShiftSelectedDataGridView.Rows.Count - 1];
                }
            }
        }

        private void btnLeaveRemove_Click(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (this.dgvLeaveShiftSelectedDataGridView.Rows.Count > 0)
                {
                    DataGridViewRow myDataGridViewRow = this.dgvLeaveShiftSelectedDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvLeaveShiftSelectedDataGridView)];

                    this.dgvLeaveShiftSelectedDataGridView.Rows.Remove(myDataGridViewRow);

                    this.dgvLeaveShiftDataGridView.Rows.Add(myDataGridViewRow);

                    this.dgvLeaveShiftDataGridView.CurrentCell = this.dgvLeaveShiftDataGridView[0, this.dgvLeaveShiftDataGridView.Rows.Count - 1];
                }
            }

        }

        private void dgvLeaveShiftDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnLeaveAdd_Click(sender, e);
        }

        private void dgvLeaveShiftSelectedDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnLeaveRemove_Click(sender, e);
        }
        
        private void dgvChosenPayCategoryDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
            e.Control.KeyPress += new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
        }
        private void dgvChosenPayCategoryDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string strValue = "";

            if (e.ColumnIndex == 1)
            {
                if (this.dgvChosenPayCategoryDataGridView[1, e.RowIndex].Value != null)
                {
                    strValue = this.dgvChosenPayCategoryDataGridView[1, e.RowIndex].Value.ToString();
                }

                if (strValue.Trim() == ""
                ||  strValue.Trim() == ".")
                    {
                    this.dgvChosenPayCategoryDataGridView[1, e.RowIndex].Value = "0.00";
                }
                else
                {
                    this.dgvChosenPayCategoryDataGridView[1, e.RowIndex].Value = Convert.ToDouble(strValue).ToString("#########0.00");
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet tempDataSet = new DataSet();

                int intLeaveShiftNo = -1;
                
                //Employee Earnings
                //Employee Earnings
                tempDataSet.Tables.Add(pvtDataSet.Tables["PayCategory"].Clone());

                if (this.dgvChosenPayCategoryDataGridView.Rows.Count == 0)
                {
                    CustomMessageBox.Show("Select at least 1 Cost Centre",
                    this.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                    return;
                }
                else
                {
                    bool blnDefault = false;

                    for (int intRow = 0; intRow < dgvChosenPayCategoryDataGridView.Rows.Count; intRow++)
                    {
                        DataRow MyDataRow = tempDataSet.Tables["PayCategory"].NewRow();
                        
                        MyDataRow["PAY_CATEGORY_NO"] = this.dgvChosenPayCategoryDataGridView[3, intRow].Value.ToString();
                        
                        if (Convert.ToBoolean(this.dgvChosenPayCategoryDataGridView[2, intRow].Value) == true)
                        {
                            MyDataRow["DEFAULT_IND"] = "Y";

                            if (blnDefault == false)
                            {
                                blnDefault = true;
                            }
                            else
                            {
                                CustomMessageBox.Show("Only 1 Cost Centre can be set to Default",
                                this.Text,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);

                                return;
                            }
                        }
                        else
                        {
                            MyDataRow["DEFAULT_IND"] = "N";
                        }

                        MyDataRow["REC_RATE"] = this.dgvChosenPayCategoryDataGridView[1, intRow].Value.ToString();

                        tempDataSet.Tables["PayCategory"].Rows.Add(MyDataRow);
                    }

                    if (blnDefault == false)
                    {
                        CustomMessageBox.Show("1 Cost Centre must be set to Default",
                               this.Text,
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error);

                        return;
                    }
                }

                if (this.dgvLeaveShiftSelectedDataGridView.Rows.Count == 0)
                {
                    CustomMessageBox.Show("Select at least 1 Normal Leave / Sick Leave Category",
                    this.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                    return;
                }
                else
                {
                    if (this.dgvLeaveShiftSelectedDataGridView.Rows.Count == 1)
                    {
                        intLeaveShiftNo = Convert.ToInt32(this.dgvLeaveShiftSelectedDataGridView[1, 0].Value);
                    }
                    else
                    {
                        CustomMessageBox.Show("Only 1 Normal Leave / Sick Leave Category Record Allowed.",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                        return;
                    }
                }

                DataView CurrentPayCategoryDataView = new DataView(pvtDataSet.Tables["CurrentPayCategory"],
                "EMPLOYEE_NO = " + pvtintEmployeeNo +  " AND PAY_CATEGORY_TYPE = '" + pvtstrCurrentPayrollType + "'",
                "",
                DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < CurrentPayCategoryDataView.Count; intRow++)
                {
                    CurrentPayCategoryDataView[intRow].Delete();

                    intRow -= 1; 
                }

                DataView EmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                "EMPLOYEE_NO = " + pvtintEmployeeNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrCurrentPayrollType + "'",
                "",
                DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < EmployeeDataView.Count; intRow++)
                {
                    EmployeeDataView[intRow].Delete();

                    intRow -= 1;
                }

                byte[] bytCompress = clsISUtilities.Compress_DataSet(tempDataSet);

                object[] objParm = new object[6];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = pvtstrPayrollType;
                objParm[2] = pvtstrCurrentPayrollType;
                objParm[3] = pvtintEmployeeNo;
                objParm[4] = intLeaveShiftNo;
                objParm[5] = bytCompress;

                bytCompress = (byte[])clsISUtilities.DynamicFunction("Update_Employee", objParm);

                tempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);
                pvtDataSet.Merge(tempDataSet);

                pvtDataSet.AcceptChanges();
                
                Load_CurrentForm_Records();

                btnCancel_Click(sender, e);
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void dgvChosenPayCategoryDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnPayCategoryRemove_Click(sender, e);
        }
    }
}
