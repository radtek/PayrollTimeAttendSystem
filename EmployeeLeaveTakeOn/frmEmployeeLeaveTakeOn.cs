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
    public partial class frmEmployeeLeaveTakeOn : Form
    {
        clsISUtilities clsISUtilities;

        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;

        private DataView pvtEmployeeDataView;

        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
    
        private byte[] pvtbytCompress;
        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnEmployeeDataGridViewLoaded = false;
        
        public frmEmployeeLeaveTakeOn()
        {
            InitializeComponent();
        }

        private void frmEmployeeLeaveTakeOn_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this,"busEmployeeLeaveTakeOn");

                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblNormalLeave.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSickLeave.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                object[] objParm = new object[1];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
              
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);
             
                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                for (int intRow = 0; intRow < pvtDataSet.Tables["PayrollType"].Rows.Count; intRow++)
                {
                    this.dgvPayrollTypeDataGridView.Rows.Add(pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE_DESC"].ToString());
                }

                pvtblnPayrollTypeDataGridViewLoaded = true;

                if (pvtDataSet.Tables["PayrollType"].Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView, 0);
                }
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

                    case "dgvEmployeesDataGridView":

                        this.dgvEmployeesDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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
              
        private void Load_CurrentForm_Records()
        {
            Load_Employees();
        }

        private void Load_Employees()
        {
            pvtEmployeeDataView = null;
            pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_TYPE = '" + this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) + "'",
                "EMPLOYEE_CODE",
                DataViewRowState.CurrentRows);

            this.Clear_DataGridView(this.dgvEmployeesDataGridView);

            pvtblnEmployeeDataGridViewLoaded = false;

            for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
            {
                this.dgvEmployeesDataGridView.Rows.Add(pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                       pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                       pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                       Convert.ToDateTime(pvtEmployeeDataView[intRow]["LEAVE_EFFECTIVE_DATE"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()),
                                                       Convert.ToDouble(pvtEmployeeDataView[intRow]["PREV_NORMAL_LEAVE_DAYS"]).ToString("######0.00"),
                                                       Convert.ToDouble(pvtEmployeeDataView[intRow]["NORMAL_LEAVE_DAYS"]).ToString("######0.00"),
                                                       Convert.ToDouble(pvtEmployeeDataView[intRow]["SICK_LEAVE_DAYS"]).ToString("######0.00"),
                                                       intRow.ToString());
            }

            pvtblnEmployeeDataGridViewLoaded = true;

            if (pvtEmployeeDataView.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvEmployeesDataGridView,0);

                this.btnUpdate.Enabled = true;
            }
            else
            {
                this.btnUpdate.Enabled = false;
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
            if (this.Text.IndexOf(" - Update", 0) > 0)
            {
                this.Text = this.Text.Substring(0, this.Text.LastIndexOf("-") - 1);
            }

            //this.dgvEmployeesDataGridView.Columns[0].SortMode = DataGridViewColumnSortMode.Automatic;
            //this.dgvEmployeesDataGridView.Columns[1].SortMode = DataGridViewColumnSortMode.Automatic;
            //this.dgvEmployeesDataGridView.Columns[2].SortMode = DataGridViewColumnSortMode.Automatic;
            //this.dgvEmployeesDataGridView.Columns[3].SortMode = DataGridViewColumnSortMode.Automatic;

            this.dgvEmployeesDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvEmployeesDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            

            this.dgvPayrollTypeDataGridView.Enabled = true;
            this.picPayrollTypeLock.Visible = false;

            this.btnUpdate.Enabled = true;
            
            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;
        }

        private void Set_Form_For_Edit()
        {
            //this.dgvEmployeesDataGridView.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvEmployeesDataGridView.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvEmployeesDataGridView.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            //this.dgvEmployeesDataGridView.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvEmployeesDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dgvEmployeesDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;
         
            this.dgvPayrollTypeDataGridView.Enabled = false;
            this.picPayrollTypeLock.Visible = true;

            this.btnUpdate.Enabled = false;
            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.dgvEmployeesDataGridView.CurrentCell = this.dgvEmployeesDataGridView[4,this.Get_DataGridView_SelectedRowIndex(dgvEmployeesDataGridView)];
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        public void btnUpdate_Click(object sender, System.EventArgs e)
        {
            this.Text += " - Update";

            Set_Form_For_Edit();
        }

        public void btnCancel_Click(object sender, System.EventArgs e)
        {
            Set_Form_For_Read();

            Load_Employees();
        }

        public void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                int intDataViewRow = 0;

                for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
                {
                    intDataViewRow = Convert.ToInt32(this.dgvEmployeesDataGridView[7, intRow].Value);

                    if (Convert.ToDouble(this.dgvEmployeesDataGridView[4,intRow].Value) == Convert.ToDouble(pvtEmployeeDataView[intDataViewRow]["PREV_NORMAL_LEAVE_DAYS"])
                    & Convert.ToDouble(this.dgvEmployeesDataGridView[5,intRow].Value) == Convert.ToDouble(pvtEmployeeDataView[intDataViewRow]["NORMAL_LEAVE_DAYS"])
                    & Convert.ToDouble(this.dgvEmployeesDataGridView[6,intRow].Value) == Convert.ToDouble(pvtEmployeeDataView[intDataViewRow]["SICK_LEAVE_DAYS"]))
                    {
                        continue;
                    }

                    pvtEmployeeDataView[intDataViewRow]["PREV_NORMAL_LEAVE_DAYS"] = Math.Round(Convert.ToDouble(this.dgvEmployeesDataGridView[4,intRow].Value), 2);
                    pvtEmployeeDataView[intDataViewRow]["NORMAL_LEAVE_DAYS"] = Math.Round(Convert.ToDouble(this.dgvEmployeesDataGridView[5,intRow].Value), 2);
                    pvtEmployeeDataView[intDataViewRow]["SICK_LEAVE_DAYS"] = Math.Round(Convert.ToDouble(this.dgvEmployeesDataGridView[6,intRow].Value), 2);
                }

                pvtTempDataSet = null;
                pvtTempDataSet = new DataSet();

                DataTable myDataTable = this.pvtDataSet.Tables["Employee"].Clone();
                pvtTempDataSet.Tables.Add(myDataTable);

                if (pvtEmployeeDataView != null)
                {
                    for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
                    {
                        if (pvtEmployeeDataView[intRow].Row.RowState == DataRowState.Modified)
                        {
                            pvtTempDataSet.Tables["Employee"].ImportRow(pvtEmployeeDataView[intRow].Row);
                        }
                    }
                }

                if (pvtTempDataSet.Tables["Employee"].Rows.Count > 0)
                {
                    //Compress DataSet
                    pvtbytCompress = clsISUtilities.Compress_DataSet(pvtTempDataSet);

                    object[] objParm = new object[2];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = pvtbytCompress;

                    clsISUtilities.DynamicFunction("Update_Record", objParm);
                }

                this.pvtDataSet.AcceptChanges();

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

                    if (this.pvtDataSet != null)
                    {
                        this.Load_CurrentForm_Records();
                    }
                }
            }
        }

        private void dgvEmployeesDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvEmployeesDataGridView.Rows.Count > 0
            & pvtblnEmployeeDataGridViewLoaded == true)
            {

            }
        }

        private void dgvEmployeesDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
            e.Control.KeyPress += new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
        }

        private void dgvEmployeesDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex > 3)
            {
                dgvEmployeesDataGridView[e.ColumnIndex, e.RowIndex].Value = Convert.ToDouble(dgvEmployeesDataGridView[e.ColumnIndex, e.RowIndex].Value).ToString("######0.00");
            }
        }
    }
}
